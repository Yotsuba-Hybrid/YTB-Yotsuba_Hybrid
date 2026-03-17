// ============================================================================
// YotsubaEngine – Immediate Mode GUI System
// GuiRenderer.cs – Adaptador de renderizado para MonoGame.
//
// PROPÓSITO:
// Toma el DrawList generado por los controles durante el frame y lo envía
// a la GPU mediante la API de bajo nivel de MonoGame.
//
// PIPELINE:
//   1. Copiar vértices al DynamicVertexBuffer.
//   2. Copiar índices al DynamicIndexBuffer.
//   3. Configurar el estado de la GPU (blend, depth, rasterizer, sampler).
//   4. Iterar los draw commands: para cada comando, bindear textura,
//      establecer scissor rect, y emitir DrawIndexedPrimitives.
//
// DECISIONES DE RENDIMIENTO:
// • Se usan DynamicVertexBuffer/DynamicIndexBuffer con SetDataOptions.Discard
//   para indicar al driver que puede orphanar el buffer anterior, evitando
//   pipeline stalls (la GPU puede seguir leyendo el viejo mientras la CPU
//   escribe el nuevo).
// • Los buffers se crean con un tamaño generoso y solo se recrean si el
//   DrawList crece más allá de la capacidad.  Esto amortiza las creaciones.
// • Se configura un BasicEffect ortográfico una vez.  Cada frame solo se
//   actualiza la matriz de proyección si el viewport cambió.
// • Se evita SpriteBatch completamente: llamadas directas a
//   GraphicsDevice.DrawIndexedPrimitives para control total del batching.
// ============================================================================

using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.IMGUI
{
    /// <summary>
    /// Renderizador que traduce el GuiDrawList a llamadas de GPU de MonoGame.
    /// </summary>
    public sealed class GuiRenderer : IDisposable
    {
        private readonly GraphicsDevice _device;
        private BasicEffect _effect;

        // Buffers dinámicos de GPU
        private DynamicVertexBuffer? _vertexBuffer;
        private DynamicIndexBuffer? _indexBuffer;
        private int _vbCapacity;
        private int _ibCapacity;

        // Estado de renderizado pre-configurado (se crea una vez, se reutiliza)
        private readonly RasterizerState _rasterizerState;
        private readonly BlendState _blendState;
        private readonly DepthStencilState _depthState;
        private readonly SamplerState _samplerState;

        // Capacidades mínimas iniciales
        private const int MinVertexCapacity = 8192;
        private const int MinIndexCapacity  = 16384;

        private bool _disposed;

        public GuiRenderer(GraphicsDevice device)
        {
            _device = device;

            // ---- BasicEffect con proyección ortográfica ----
            // Se usa BasicEffect porque:
            //   1. No requiere shaders custom (compatible con todas las plataformas MonoGame).
            //   2. Soporta VertexColor y textura simultáneamente.
            //   3. La proyección ortográfica mapea pixeles de pantalla a NDC directamente.
            _effect = new BasicEffect(device)
            {
                VertexColorEnabled = true,
                TextureEnabled = true,
                LightingEnabled = false,
                FogEnabled = false,
            };
            UpdateProjection();

            // ---- Estado de renderizado optimizado para UI ----

            // Alpha blending estándar (SrcAlpha, InvSrcAlpha).
            // Premultiplied alpha sería más eficiente pero requeriría que las
            // texturas estén pre-multiplicadas, complicando el pipeline de assets.
            _blendState = new BlendState
            {
                ColorSourceBlend = Blend.SourceAlpha,
                AlphaSourceBlend = Blend.SourceAlpha,
                ColorDestinationBlend = Blend.InverseSourceAlpha,
                AlphaDestinationBlend = Blend.InverseSourceAlpha,
            };

            // Sin depth testing: la UI se dibuja de atrás hacia adelante
            // (painter's algorithm) mediante el orden de los draw commands.
            _depthState = DepthStencilState.None;

            // Sin culling: algunos triángulos de la UI pueden tener winding
            // invertido (ej. flechas de dropdown) y el overhead de culling
            // no aporta nada en 2D.
            // Scissor test habilitado para clip rects.
            _rasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                ScissorTestEnable = true,
                MultiSampleAntiAlias = false,
            };

            // Point sampling: la UI usa la textura blanca 1×1 mayormente,
            // y las fuentes bitmap se ven mejor sin filtrado bilinear.
            _samplerState = SamplerState.PointClamp;
        }

        /// <summary>
        /// Renderiza el DrawList completo.  Llamar en la fase Draw del game loop.
        /// </summary>
        public void Render(GuiDrawList drawList)
        {
            if (drawList.VertexCount == 0 || drawList.CommandCount == 0)
                return;

            // ---- Actualizar buffers de GPU ----
            EnsureBufferCapacity(drawList.VertexCount, drawList.IndexCount);
            UploadBuffers(drawList);

            // ---- Configurar pipeline de GPU ----
            UpdateProjection();

            _device.BlendState = _blendState;
            _device.DepthStencilState = _depthState;
            _device.RasterizerState = _rasterizerState;
            _device.SamplerStates[0] = _samplerState;

            _device.SetVertexBuffer(_vertexBuffer);
            _device.Indices = _indexBuffer;

            // ---- Ejecutar draw commands ----
            var commands = drawList.Commands;
            int totalVertices = drawList.VertexCount;

            foreach (ref readonly GuiDrawCmd cmd in commands)
            {
                if (cmd.IndexCount == 0)
                    continue;

                // Scissor rect (clipping)
                _device.ScissorRectangle = new Rectangle(
                    (int)cmd.ClipRect.X,
                    (int)cmd.ClipRect.Y,
                    (int)cmd.ClipRect.W,
                    (int)cmd.ClipRect.H
                );

                // Textura
                _effect.Texture = cmd.Texture;
                _effect.TextureEnabled = cmd.Texture != null;

                // Dibujar con BasicEffect
                foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    // DrawIndexedPrimitives es la ruta más directa a la GPU:
                    // - minVertexIndex: 0 (usamos todo el VB)
                    // - numVertices: total de vértices en el buffer
                    // - startIndex: offset en el index buffer para este comando
                    // - primitiveCount: número de triángulos (indexCount / 3)
                    _device.DrawIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        baseVertex: 0,
                        minVertexIndex: 0,
                        numVertices: totalVertices,
                        startIndex: cmd.IndexOffset,
                        primitiveCount: cmd.IndexCount / 3
                    );
                }
            }
        }

        // ================================================================
        // Buffer management
        // ================================================================

        /// <summary>
        /// Asegura que los buffers de GPU tengan capacidad suficiente.
        /// Si no, los recrea con capacidad duplicada.
        ///
        /// Los buffers de GPU nunca se reducen durante la vida de la aplicación.
        /// Esto evita la creación/destrucción cíclica que causaría micro-stalls.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureBufferCapacity(int vertexCount, int indexCount)
        {
            // Vertex buffer
            if (_vertexBuffer == null || vertexCount > _vbCapacity)
            {
                _vertexBuffer?.Dispose();
                _vbCapacity = Math.Max(MinVertexCapacity, NextPowerOfTwo(vertexCount));
                _vertexBuffer = new DynamicVertexBuffer(
                    _device,
                    GuiDrawVert.VertexDeclaration,
                    _vbCapacity,
                    BufferUsage.WriteOnly
                );
            }

            // Index buffer
            if (_indexBuffer == null || indexCount > _ibCapacity)
            {
                _indexBuffer?.Dispose();
                _ibCapacity = Math.Max(MinIndexCapacity, NextPowerOfTwo(indexCount));
                _indexBuffer = new DynamicIndexBuffer(
                    _device,
                    IndexElementSize.SixteenBits,
                    _ibCapacity,
                    BufferUsage.WriteOnly
                );
            }
        }

        /// <summary>
        /// Sube los datos del DrawList a los buffers de GPU.
        ///
        /// SetDataOptions.Discard le indica al driver de GPU que invalide el
        /// contenido anterior del buffer.  Esto permite al driver hacer "buffer
        /// orphaning": asignar un nuevo bloque de memoria sin esperar a que la
        /// GPU termine de leer el anterior.  Es la técnica estándar para evitar
        /// pipeline stalls con buffers dinámicos.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UploadBuffers(GuiDrawList drawList)
        {
            // SetData con array + offset + count: la ruta más directa.
            // MonoGame internamente hace Marshal.Copy, que es un memcpy nativo.
            _vertexBuffer!.SetData(
                drawList.VertexArray, 0, drawList.VertexCount,
                SetDataOptions.Discard
            );

            _indexBuffer!.SetData(
                drawList.IndexArray, 0, drawList.IndexCount,
                SetDataOptions.Discard
            );
        }

        /// <summary>
        /// Actualiza la matriz de proyección ortográfica para que coincida
        /// con el viewport actual.  Cada pixel de pantalla = 1 unidad.
        /// </summary>
        private void UpdateProjection()
        {
            var vp = _device.Viewport;
            _effect.Projection = Matrix.CreateOrthographicOffCenter(
                0, vp.Width, vp.Height, 0, -1f, 1f
            );
            _effect.View = Matrix.Identity;
            _effect.World = Matrix.Identity;
        }

        // ================================================================
        // Utilities
        // ================================================================

        /// <summary>
        /// Calcula la siguiente potencia de 2 >= value.
        /// Usa bit manipulation: más rápido que log2 + ceil.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int NextPowerOfTwo(int value)
        {
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
        }

        // ================================================================
        // IDisposable
        // ================================================================

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _vertexBuffer?.Dispose();
            _indexBuffer?.Dispose();
            _effect?.Dispose();
            _blendState?.Dispose();
            // DepthStencilState.None es estático, no disponer.
            _rasterizerState?.Dispose();
            // SamplerState.PointClamp es estático, no disponer.
        }
    }
}
