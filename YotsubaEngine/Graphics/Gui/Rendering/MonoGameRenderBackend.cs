// =============================================================================
// MonoGameRenderBackend.cs  –  YotsubaEngine Immediate Mode GUI
// =============================================================================
// Adaptador entre el DrawList y la GPU de MonoGame.
//
// DISEÑO GENERAL
// ──────────────
// Cada frame:
//  1. Se copian los vértices e índices del DrawList a DynamicVertexBuffer /
//     DynamicIndexBuffer usando SetDataOptions.Discard (sin stall de GPU).
//  2. Se itera sobre los DrawCmds, aplicando scissor rect y textura para cada uno.
//  3. Se emite una sola llamada DrawIndexedPrimitives por cada DrawCmd que cambia
//     textura o clip → mínimos state changes.
//  4. En un post-pass, el SpriteBatch procesa los TextCmds para renderizar texto.
//
// DynamicVertexBuffer vs VertexBuffer
// ─────────────────────────────────────
// DynamicVertexBuffer usa SetDataOptions.Discard (GPU buffer re-use) o
// SetDataOptions.NoOverwrite (append sin invalida). Discard es el indicado
// cuando se reemplaza todo el buffer cada frame, que es nuestro caso.
// Un VertexBuffer estático requeriría glBufferSubData con sincronización,
// causando bubbles de pipeline. DynamicVB evita esto.
//
// EFECTO (BasicEffect)
// ─────────────────────
// BasicEffect con:
//   • VertexColorEnabled = true     (usamos el canal Alpha para transparencia)
//   • TextureEnabled     = true     (textura 1×1 blanca para quads sólidos)
//   • Projection ortogonal top-left (0,0) → (W,H)
//
// La proyección ortogonal 2D mapea píxeles directamente a coordenadas NDC:
//   NDC.x = (px / W) * 2 - 1
//   NDC.y = 1 - (py / H) * 2
// CreateOrthographicOffCenter hace este mapeo internamente.
//
// SCISSOR / CLIPPING
// ───────────────────
// El RasterizerState tiene ScissorTestEnable = true.
// Cambiamos GraphicsDevice.ScissorRectangle por DrawCmd.
// Guardamos y restauramos el viewport y scissor originales para no interferir
// con el resto del juego.
// =============================================================================

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.Graphics.Gui;

/// <summary>
/// Backend de renderizado MonoGame para el sistema de UI inmediato.
/// Consume el <see cref="DrawList"/> de un <see cref="GuiContext"/> y envía
/// la geometría a la GPU de manera eficiente.
/// </summary>
public sealed class MonoGameRenderBackend : IDisposable
{
    // ─────────────────────────────────────────────────────────────────────────
    // CONSTANTES
    // ─────────────────────────────────────────────────────────────────────────

    // Capacidad inicial de los buffers GPU. Se re-alocan si el DrawList crece
    // más allá de estos límites (sucede solo al inicio, luego steady-state).
    private const int INITIAL_VTX_CAPACITY = 8_192;
    private const int INITIAL_IDX_CAPACITY = 16_384;

    // ─────────────────────────────────────────────────────────────────────────
    // RECURSOS GPU
    // ─────────────────────────────────────────────────────────────────────────

    private readonly GraphicsDevice        _gd;
    private          DynamicVertexBuffer   _vb;
    private          DynamicIndexBuffer    _ib;
    private          int                   _vbCapacity;
    private          int                   _ibCapacity;

    private readonly BasicEffect           _effect;
    private readonly RasterizerState       _rasterizerState;
    private readonly BlendState            _blendState;
    private readonly DepthStencilState     _depthStencilState;
    private          SpriteBatch           _spriteBatch;

    /// <summary>
    /// Textura 1×1 blanca usada para los DrawCmds sin textura asignada.
    /// Todos los rectángulos sólidos la referencian → máximo batching.
    /// </summary>
    private readonly Texture2D _whiteTex;

    // ─────────────────────────────────────────────────────────────────────────
    // CONSTRUCTOR
    // ─────────────────────────────────────────────────────────────────────────

    /// <param name="graphicsDevice">GraphicsDevice de MonoGame.</param>
    public MonoGameRenderBackend(GraphicsDevice graphicsDevice)
    {
        _gd = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));

        // ── Pixel blanco 1×1 ─────────────────────────────────────────────
        _whiteTex = new Texture2D(_gd, 1, 1, false, SurfaceFormat.Color);
        _whiteTex.SetData(new[] { Color.White });

        // ── DynamicBuffers ────────────────────────────────────────────────
        _vbCapacity = INITIAL_VTX_CAPACITY;
        _ibCapacity = INITIAL_IDX_CAPACITY;
        _vb = new DynamicVertexBuffer(_gd,
            VertexPositionColorTexture.VertexDeclaration,
            _vbCapacity, BufferUsage.WriteOnly);
        _ib = new DynamicIndexBuffer(_gd, IndexElementSize.SixteenBits,
            _ibCapacity, BufferUsage.WriteOnly);

        // ── BasicEffect ───────────────────────────────────────────────────
        _effect = new BasicEffect(_gd)
        {
            VertexColorEnabled = true,
            TextureEnabled     = true,
            LightingEnabled    = false,
        };

        // ── Estado de rasterización ───────────────────────────────────────
        // ScissorTestEnable es imprescindible para el clipping por DrawCmd.
        _rasterizerState = new RasterizerState
        {
            CullMode             = CullMode.None,
            FillMode             = FillMode.Solid,
            ScissorTestEnable    = true,
            MultiSampleAntiAlias = false,
        };

        // Alpha blending premultiplicado es el estándar de MonoGame.
        // NonPremultiplied es más intuitivo para colores RGBA de UI.
        _blendState        = BlendState.NonPremultiplied;
        _depthStencilState = DepthStencilState.None;

        // SpriteBatch para el post-pass de texto
        _spriteBatch = new SpriteBatch(_gd);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RENDER
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Renderiza el frame completo de UI a partir del DrawList del contexto.
    /// Llama al final del frame, después de <see cref="Gui.EndFrame"/>.
    /// </summary>
    public void Render(GuiContext ctx)
    {
        DrawList dl = ctx.DrawList;

        if (dl.VtxCount == 0 && dl.TextCmdCount == 0) return;

        // ── Guardar estado GPU ────────────────────────────────────────────
        Viewport       savedViewport = _gd.Viewport;
        Rectangle      savedScissor  = _gd.ScissorRectangle;
        BlendState     savedBlend    = _gd.BlendState;
        DepthStencilState savedDS    = _gd.DepthStencilState;
        RasterizerState savedRS      = _gd.RasterizerState;

        // ── Setup de proyección ortogonal ─────────────────────────────────
        int vpW = _gd.PresentationParameters.BackBufferWidth;
        int vpH = _gd.PresentationParameters.BackBufferHeight;

        _gd.Viewport = new Viewport(0, 0, vpW, vpH);

        _effect.Projection = Matrix.CreateOrthographicOffCenter(
            0f, vpW, vpH, 0f, -1f, 1f);
        _effect.View  = Matrix.Identity;
        _effect.World = Matrix.Identity;

        // ── Aplicar estados ───────────────────────────────────────────────
        _gd.BlendState        = _blendState;
        _gd.DepthStencilState = _depthStencilState;
        _gd.RasterizerState   = _rasterizerState;

        // ── Subir geometría a la GPU ──────────────────────────────────────
        if (dl.VtxCount > 0)
        {
            UploadGeometry(dl);
            RenderGeometryCommands(dl);
        }

        // ── Post-pass de texto (vía SpriteBatch) ──────────────────────────
        if (dl.TextCmdCount > 0 && ctx.Font != null)
            RenderTextCommands(dl, ctx.Font, vpW, vpH);

        // ── Restaurar estado GPU ──────────────────────────────────────────
        _gd.Viewport          = savedViewport;
        _gd.ScissorRectangle  = savedScissor;
        _gd.BlendState        = savedBlend;
        _gd.DepthStencilState = savedDS;
        _gd.RasterizerState   = savedRS;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // UPLOAD GEOMETRY
    // ─────────────────────────────────────────────────────────────────────────

    private void UploadGeometry(DrawList dl)
    {
        // Expandir buffers GPU si la DrawList creció
        if (dl.VtxCount > _vbCapacity)
        {
            _vb.Dispose();
            _vbCapacity = dl.VtxCount * 2; // over-provision ×2
            _vb = new DynamicVertexBuffer(_gd,
                VertexPositionColorTexture.VertexDeclaration,
                _vbCapacity, BufferUsage.WriteOnly);
        }
        if (dl.IdxCount > _ibCapacity)
        {
            _ib.Dispose();
            _ibCapacity = dl.IdxCount * 2;
            _ib = new DynamicIndexBuffer(_gd, IndexElementSize.SixteenBits,
                _ibCapacity, BufferUsage.WriteOnly);
        }

        // SetDataOptions.Discard: el driver descarta el buffer anterior y
        // retorna uno nuevo sin esperar a que la GPU termine de usarlo.
        // Esto elimina el pipeline stall que ocurriría con None.
        _vb.SetData(dl.Vertices, 0, dl.VtxCount, SetDataOptions.Discard);
        _ib.SetData(dl.Indices,  0, dl.IdxCount, SetDataOptions.Discard);

        _gd.SetVertexBuffer(_vb);
        _gd.Indices = _ib;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RENDER GEOMETRY COMMANDS
    // ─────────────────────────────────────────────────────────────────────────

    private void RenderGeometryCommands(DrawList dl)
    {
        int vpW = _gd.PresentationParameters.BackBufferWidth;
        int vpH = _gd.PresentationParameters.BackBufferHeight;

        for (int i = 0; i < dl.CmdCount; i++)
        {
            ref DrawCmd cmd = ref dl.Commands[i];

            if (cmd.ElemCount == 0) continue;

            // Scissor rect
            Rectangle scissor = cmd.ClipRect;
            // Intersectar con el viewport para evitar scissor fuera de pantalla
            scissor = Rectangle.Intersect(scissor, new Rectangle(0, 0, vpW, vpH));
            if (scissor.Width <= 0 || scissor.Height <= 0) continue;

            _gd.ScissorRectangle = scissor;

            // Textura: usar whiteTex para quads sólidos (DrawCmd.Texture == null)
            Texture2D tex = cmd.Texture ?? _whiteTex;
            _effect.Texture = tex;

            // Aplicar pases del efecto y dibujar
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                int triangleCount = cmd.ElemCount / 3;
                if (triangleCount <= 0) continue;

                // DrawIndexedPrimitives es la llamada más eficiente disponible:
                // no hace copia de datos (los buffers ya están en VRAM).
                //
                // NOTA: baseVertex = 0 porque los índices en el DrawList son
                // ABSOLUTOS (incluyen el VtxOffset del comando). Pasar cmd.VtxOffset
                // aquí causaría un doble-offset → vértices incorrectos.
                _gd.DrawIndexedPrimitives(
                    primitiveType:  PrimitiveType.TriangleList,
                    baseVertex:     0,
                    startIndex:     cmd.IdxOffset,
                    primitiveCount: triangleCount);
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RENDER TEXT COMMANDS (post-pass vía SpriteBatch)
    // ─────────────────────────────────────────────────────────────────────────

    private void RenderTextCommands(DrawList dl, SpriteFont font, int vpW, int vpH)
    {
        // DECISIÓN: SpriteBatch agrupa los DrawString en una sola batch.
        // Usamos un RasterizerState con scissor para que SpriteBatch respete
        // los clip rects de los TextCmds.
        //
        // Limitación: SpriteBatch.Begin/End es necesario cambiar cuando el
        // scissor cambia. Para minimizar los cambios, agrupamos los TextCmds
        // consecutivos con el mismo scissor rect.

        Rectangle currentClip = new(-1, -1, -1, -1); // sentinel inválido
        bool batchOpen = false;

        for (int i = 0; i < dl.TextCmdCount; i++)
        {
            ref TextCmd tc = ref dl.TextCommands[i];

            if (tc.TextLength <= 0) continue;

            Rectangle clip = Rectangle.Intersect(tc.ClipRect, new Rectangle(0, 0, vpW, vpH));
            if (clip.Width <= 0 || clip.Height <= 0) continue;

            // Si el scissor cambió, cerramos y reabrimos el SpriteBatch
            if (clip != currentClip)
            {
                if (batchOpen) _spriteBatch.End();

                _gd.ScissorRectangle = clip;
                _spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.NonPremultiplied,
                    SamplerState.LinearClamp,
                    DepthStencilState.None,
                    _rasterizerState); // ScissorTestEnable = true

                currentClip = clip;
                batchOpen   = true;
            }

            // Convertir el TextPool slice a string para SpriteBatch.
            // Esta es la única alocación del pipeline: una string por TextCmd.
            // Futura optimización: usar un pool de strings o SpriteBatch sobrecargado.
            ReadOnlySpan<char> textSpan = dl.GetTextPoolSlice(tc.TextOffset, tc.TextLength);
            string textStr = textSpan.ToString(); // alocación aceptada en post-pass

            _spriteBatch.DrawString(
                font,
                textStr,
                tc.Position,
                tc.Color,
                rotation:   0f,
                origin:     Vector2.Zero,
                scale:      tc.Scale,
                effects:    SpriteEffects.None,
                layerDepth: 0f);
        }

        if (batchOpen) _spriteBatch.End();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // IDisposable
    // ─────────────────────────────────────────────────────────────────────────

    private bool _disposed;

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _vb.Dispose();
        _ib.Dispose();
        _effect.Dispose();
        _whiteTex.Dispose();
        _spriteBatch.Dispose();
        _rasterizerState.Dispose();
    }
}
