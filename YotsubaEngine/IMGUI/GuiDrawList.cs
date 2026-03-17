// ============================================================================
// YotsubaEngine – Immediate Mode GUI System
// GuiDrawList.cs – Pipeline de geometría independiente.
//
// ARQUITECTURA:
// Los controles nunca tocan SpriteBatch ni GraphicsDevice.  En su lugar,
// empujan vértices e índices a esta lista de dibujo.  Al final del frame,
// el renderer (GuiRenderer) toma los buffers resultantes y los envía a la
// GPU en un solo batch (o pocos batches agrupados por textura).
//
// DECISIONES DE RENDIMIENTO:
// • Los buffers de vértices e índices se pre-alocan con capacidad generosa
//   (64K verts, 96K indices).  Si se excede, se duplican (amortized O(1)).
//   Nunca se reducen durante la vida de la aplicación → cero GC per-frame.
// • Se usa unsafe Span slicing para copias rápidas de geometría (ej. quads).
// • Los draw commands se fusionan cuando comparten la misma textura y clip rect,
//   minimizando state changes en la GPU.
// ============================================================================

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.IMGUI
{
    /// <summary>
    /// Lista de dibujo acumulativa.  Cada frame se limpia (Clear) y se llena
    /// de nuevo.  Los arrays internos permanecen vivos entre frames.
    /// </summary>
    public sealed class GuiDrawList
    {
        // ---- Buffers pre-alocados ----
        private GuiDrawVert[] _vertices;
        private ushort[] _indices;        // 16-bit indices: suficiente para UI (< 65K verts por draw call)
        private GuiDrawCmd[] _commands;

        private int _vertexCount;
        private int _indexCount;
        private int _cmdCount;

        // ---- Estado de batching ----
        private Texture2D? _currentTexture;
        private GuiRect _currentClipRect;

        // ---- Constantes de capacidad inicial ----
        // Se eligen potencias de 2 cercanas a los máximos típicos de una UI
        // compleja para minimizar realocaciones.
        private const int InitialVertexCapacity = 65536;   // 64K verts × 24 bytes = 1.5 MB
        private const int InitialIndexCapacity  = 131072;  // 128K indices × 2 bytes = 256 KB
        private const int InitialCmdCapacity    = 256;

        public GuiDrawList()
        {
            _vertices = new GuiDrawVert[InitialVertexCapacity];
            _indices  = new ushort[InitialIndexCapacity];
            _commands = new GuiDrawCmd[InitialCmdCapacity];
            _currentClipRect = new GuiRect(0, 0, 4096, 4096);
        }

        // ==== Acceso de solo lectura para el renderer ====
        public ReadOnlySpan<GuiDrawVert> Vertices => _vertices.AsSpan(0, _vertexCount);
        public ReadOnlySpan<ushort> Indices => _indices.AsSpan(0, _indexCount);
        public ReadOnlySpan<GuiDrawCmd> Commands => _commands.AsSpan(0, _cmdCount);
        public int VertexCount => _vertexCount;
        public int IndexCount => _indexCount;
        public int CommandCount => _cmdCount;

        // ================================================================
        // Frame lifecycle
        // ================================================================

        /// <summary>
        /// Limpia todos los contadores sin liberar la memoria subyacente.
        /// El costo es O(1), no O(N).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            _vertexCount = 0;
            _indexCount = 0;
            _cmdCount = 0;
            _currentTexture = null;
        }

        // ================================================================
        // State management
        // ================================================================

        /// <summary>
        /// Establece la textura activa.  Si difiere de la actual, se fuerza
        /// un nuevo draw command en la próxima primitiva.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTexture(Texture2D? texture)
        {
            _currentTexture = texture;
        }

        /// <summary>
        /// Establece el rectángulo de clip activo (scissor rect).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushClipRect(GuiRect rect)
        {
            _currentClipRect = rect;
        }

        // ================================================================
        // Primitive emission – Quad (rectángulo sólido / texturizado)
        // ================================================================

        /// <summary>
        /// Emite un quad (2 triángulos, 4 vértices, 6 índices).
        /// Esta es la primitiva más usada: botones, fondos, barras, etc.
        ///
        /// El orden de vértices es:
        ///   0──1       Triángulo 1: 0-1-2
        ///   │ ╱│       Triángulo 2: 2-1-3
        ///   2──3
        ///
        /// Se reserva espacio y se escribe mediante indexación directa,
        /// evitando llamadas a Add() con bounds-checking redundante.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddQuad(GuiRect rect, GuiColor32 color)
        {
            AddQuad(rect, color, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1));
        }

        /// <summary>
        /// Quad con coordenadas UV explícitas (para texto o iconos texturizados).
        /// </summary>
        public void AddQuad(GuiRect rect, GuiColor32 color, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3)
        {
            EnsureVertexCapacity(4);
            EnsureIndexCapacity(6);

            int vi = _vertexCount;
            Color xnaColor = color.ToXnaColor();

            // Escribir vértices directamente en el array – sin bounds check redundante
            _vertices[vi + 0] = new GuiDrawVert(rect.X, rect.Y, xnaColor, uv0.X, uv0.Y);
            _vertices[vi + 1] = new GuiDrawVert(rect.Right, rect.Y, xnaColor, uv1.X, uv1.Y);
            _vertices[vi + 2] = new GuiDrawVert(rect.X, rect.Bottom, xnaColor, uv2.X, uv2.Y);
            _vertices[vi + 3] = new GuiDrawVert(rect.Right, rect.Bottom, xnaColor, uv3.X, uv3.Y);
            _vertexCount += 4;

            // Escribir índices
            int ii = _indexCount;
            _indices[ii + 0] = (ushort)vi;
            _indices[ii + 1] = (ushort)(vi + 1);
            _indices[ii + 2] = (ushort)(vi + 2);
            _indices[ii + 3] = (ushort)(vi + 2);
            _indices[ii + 4] = (ushort)(vi + 1);
            _indices[ii + 5] = (ushort)(vi + 3);
            _indexCount += 6;

            // Intentar fusionar con el último command si comparten textura
            AppendToCurrentCmd(6);
        }

        /// <summary>
        /// Quad con 4 colores diferentes (gradient).  Útil para barras de progreso
        /// y fondos con degradado vertical.
        /// </summary>
        public void AddQuadGradient(GuiRect rect, GuiColor32 topLeft, GuiColor32 topRight, GuiColor32 bottomLeft, GuiColor32 bottomRight)
        {
            EnsureVertexCapacity(4);
            EnsureIndexCapacity(6);

            int vi = _vertexCount;
            Vector2 uvZero = Vector2.Zero;

            _vertices[vi + 0] = new GuiDrawVert(new Vector2(rect.X, rect.Y), topLeft.ToXnaColor(), uvZero);
            _vertices[vi + 1] = new GuiDrawVert(new Vector2(rect.Right, rect.Y), topRight.ToXnaColor(), uvZero);
            _vertices[vi + 2] = new GuiDrawVert(new Vector2(rect.X, rect.Bottom), bottomLeft.ToXnaColor(), uvZero);
            _vertices[vi + 3] = new GuiDrawVert(new Vector2(rect.Right, rect.Bottom), bottomRight.ToXnaColor(), uvZero);
            _vertexCount += 4;

            int ii = _indexCount;
            _indices[ii + 0] = (ushort)vi;
            _indices[ii + 1] = (ushort)(vi + 1);
            _indices[ii + 2] = (ushort)(vi + 2);
            _indices[ii + 3] = (ushort)(vi + 2);
            _indices[ii + 4] = (ushort)(vi + 1);
            _indices[ii + 5] = (ushort)(vi + 3);
            _indexCount += 6;

            AppendToCurrentCmd(6);
        }

        /// <summary>
        /// Borde de 1px alrededor de un rectángulo (4 quads finos).
        /// Se usa para frames de controles (text input, combobox, etc).
        /// </summary>
        public void AddRectBorder(GuiRect rect, GuiColor32 color, float thickness = 1f)
        {
            // Top
            AddQuad(new GuiRect(rect.X, rect.Y, rect.W, thickness), color);
            // Bottom
            AddQuad(new GuiRect(rect.X, rect.Bottom - thickness, rect.W, thickness), color);
            // Left
            AddQuad(new GuiRect(rect.X, rect.Y + thickness, thickness, rect.H - thickness * 2), color);
            // Right
            AddQuad(new GuiRect(rect.Right - thickness, rect.Y + thickness, thickness, rect.H - thickness * 2), color);
        }

        /// <summary>
        /// Emite un triángulo individual (para flechas de dropdown, checkmarks, etc).
        /// </summary>
        public void AddTriangle(Vector2 a, Vector2 b, Vector2 c, GuiColor32 color)
        {
            EnsureVertexCapacity(3);
            EnsureIndexCapacity(3);

            int vi = _vertexCount;
            Color xnaColor = color.ToXnaColor();
            Vector2 uvZero = Vector2.Zero;

            _vertices[vi + 0] = new GuiDrawVert(a, xnaColor, uvZero);
            _vertices[vi + 1] = new GuiDrawVert(b, xnaColor, uvZero);
            _vertices[vi + 2] = new GuiDrawVert(c, xnaColor, uvZero);
            _vertexCount += 3;

            int ii = _indexCount;
            _indices[ii + 0] = (ushort)vi;
            _indices[ii + 1] = (ushort)(vi + 1);
            _indices[ii + 2] = (ushort)(vi + 2);
            _indexCount += 3;

            AppendToCurrentCmd(3);
        }

        // ================================================================
        // Internal: command merging y capacity management
        // ================================================================

        /// <summary>
        /// Intenta añadir índices al último comando si textura y clip coinciden;
        /// si no, crea un nuevo comando.  Esto minimiza draw calls.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AppendToCurrentCmd(int indexCount)
        {
            if (_cmdCount > 0)
            {
                ref GuiDrawCmd last = ref _commands[_cmdCount - 1];
                // Fusionar si misma textura y mismo clip rect
                if (last.Texture == _currentTexture &&
                    last.ClipRect.X == _currentClipRect.X &&
                    last.ClipRect.Y == _currentClipRect.Y &&
                    last.ClipRect.W == _currentClipRect.W &&
                    last.ClipRect.H == _currentClipRect.H)
                {
                    last.IndexCount += indexCount;
                    return;
                }
            }

            // Nuevo comando
            EnsureCmdCapacity(1);
            _commands[_cmdCount++] = new GuiDrawCmd
            {
                IndexCount = indexCount,
                IndexOffset = _indexCount - indexCount,
                VertexOffset = 0, // No usamos baseVertex en 16-bit mode
                Texture = _currentTexture,
                ClipRect = _currentClipRect,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureVertexCapacity(int additional)
        {
            int required = _vertexCount + additional;
            if (required <= _vertices.Length) return;

            // Duplicar capacidad (amortized growth, igual que List<T>)
            int newCap = _vertices.Length * 2;
            while (newCap < required) newCap *= 2;

            var newArr = new GuiDrawVert[newCap];
            Array.Copy(_vertices, newArr, _vertexCount);
            _vertices = newArr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureIndexCapacity(int additional)
        {
            int required = _indexCount + additional;
            if (required <= _indices.Length) return;

            int newCap = _indices.Length * 2;
            while (newCap < required) newCap *= 2;

            var newArr = new ushort[newCap];
            Array.Copy(_indices, newArr, _indexCount);
            _indices = newArr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCmdCapacity(int additional)
        {
            int required = _cmdCount + additional;
            if (required <= _commands.Length) return;

            int newCap = _commands.Length * 2;
            while (newCap < required) newCap *= 2;

            var newArr = new GuiDrawCmd[newCap];
            Array.Copy(_commands, newArr, _cmdCount);
            _commands = newArr;
        }

        // ==== Acceso directo a arrays internos (para el renderer que necesita
        //      pasar arrays, no spans, a las APIs de MonoGame) ====
        internal GuiDrawVert[] VertexArray => _vertices;
        internal ushort[] IndexArray => _indices;
    }
}
