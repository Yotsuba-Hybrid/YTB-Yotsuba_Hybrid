// =============================================================================
// DrawList.cs  –  YotsubaEngine Immediate Mode GUI
// =============================================================================
// Pipeline de geometría de la UI. Acumula vértices, índices y comandos de
// dibujado en arrays pre-alocados. El backend de MonoGame los consume al final
// del frame sin alocaciones adicionales.
//
// DECISIÓN DE DISEÑO – Arrays pre-alocados vs listas dinámicas
// ─────────────────────────────────────────────────────────────
// Se usan arrays de tamaño fijo con índices de escritura en lugar de
// List<T> / ArrayPool porque:
//  • List<T> hace resize (copia+alloc) cuando rebasa la capacidad.
//  • ArrayPool tiene overhead de alquiler/devolución cada frame.
//  • Arrays fijos garantizan cero GC una vez inicializados,
//    con resize manual (duplicar capacidad) cuando realmente sea necesario.
//
// VÉRTICES – VertexPositionColorTexture
// ──────────────────────────────────────
// Usamos el tipo nativo de MonoGame para evitar conversiones. El campo
// Position es Vector3 (Z=0 siempre en UI 2D). Color es un uint RGBA packed.
// TextureCoordinate apunta a (0,0) del pixel blanco cuando dibujamos sólidos,
// o a UVs reales cuando usamos texturas.
//
// ÍNDICES – ushort (16 bits)
// ──────────────────────────
// 65535 vértices máx por draw call. Si una ventana necesita más, el DrawList
// emite un nuevo comando automáticamente (vertex buffer flush mid-frame).
// Esto es el mismo límite que usa Dear ImGui por defecto.
//
// TEXT POOL
// ─────────
// Para los comandos de texto se usa un char pool plano. Cada TextCmd almacena
// un offset+longitud en ese pool. Cero alocaciones durante el frame.
// =============================================================================

using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.Graphics.Gui;

// ─────────────────────────────────────────────────────────────────────────────
// ESTRUCTURAS DE DATOS
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Comando de dibujado: define una región de índices que comparten textura y
/// scissor rect. Un cambio de textura o clip rect emite un nuevo DrawCmd.
/// </summary>
public struct DrawCmd
{
    /// <summary>Número de índices en este comando (siempre múltiplo de 3).</summary>
    public int  ElemCount;

    /// <summary>Rectángulo de recorte (scissor) en píxeles de pantalla.</summary>
    public Rectangle ClipRect;

    /// <summary>
    /// Textura usada para esta batch. <c>null</c> indica que el backend debe usar
    /// el pixel blanco 1×1 generado en inicialización (para rectángulos sólidos).
    /// </summary>
    public Texture2D? Texture;

    /// <summary>Offset base de vértices para este comando (vertex buffer offset).</summary>
    public int VtxOffset;

    /// <summary>Offset base de índices para este comando (index buffer offset).</summary>
    public int IdxOffset;
}

/// <summary>
/// Comando de texto. El texto es almacenado en el TextPool del DrawList;
/// este struct solo guarda offset+longitud → cero alocaciones por frame.
/// </summary>
public struct TextCmd
{
    /// <summary>Offset en el TextPool donde empieza el texto.</summary>
    public int   TextOffset;

    /// <summary>Longitud del texto en caracteres.</summary>
    public int   TextLength;

    /// <summary>Posición top-left del texto en píxeles de pantalla.</summary>
    public Vector2 Position;

    /// <summary>Color del texto.</summary>
    public Color Color;

    /// <summary>Scissor rect al que este texto está sujeto.</summary>
    public Rectangle ClipRect;

    /// <summary>
    /// Escala del texto (1f = tamaño nativo de SpriteFont).
    /// Útil para UI a alta resolución o para títulos grandes.
    /// </summary>
    public float Scale;
}

// ─────────────────────────────────────────────────────────────────────────────
// DRAW LIST
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Buffer de geometría 2D para un frame completo de UI.
/// Diseñado para cero alocaciones en la ruta caliente (Update/Draw por frame).
/// </summary>
public sealed class DrawList
{
    // Capacidades iniciales: dimensionadas para una UI moderada sin resize.
    // Si se supera la capacidad, el array crece ×2 (una sola alocación).
    private const int INITIAL_VTX_CAPACITY  = 8_192;
    private const int INITIAL_IDX_CAPACITY  = 16_384;   // índices = vtx × ~2
    private const int INITIAL_CMD_CAPACITY  = 256;
    private const int INITIAL_TEXT_CAPACITY = 512;
    private const int TEXT_POOL_SIZE        = 32_768;   // 32K chars por frame

    // ─── Buffers de geometría ──────────────────────────────────────────────
    internal VertexPositionColorTexture[] Vertices;
    internal ushort[]                     Indices;

    /// <summary>Número de vértices escritos en este frame.</summary>
    public int VtxCount { get; private set; }

    /// <summary>Número de índices escritos en este frame.</summary>
    public int IdxCount { get; private set; }

    // ─── Comandos de dibujado ──────────────────────────────────────────────
    internal DrawCmd[] Commands;
    internal int       CmdCount;

    // ─── Comandos de texto ────────────────────────────────────────────────
    internal TextCmd[] TextCommands;
    internal int       TextCmdCount;

    // ─── Pool de texto (cero-alloc) ───────────────────────────────────────
    // Chars de todos los textos del frame se acumulan aquí.
    // Tamaño fijo; si se agota se trunca el texto silenciosamente.
    private readonly char[] _textPool = new char[TEXT_POOL_SIZE];
    private int             _textPoolPos;

    // ─── Scissor rect stack ───────────────────────────────────────────────
    private Rectangle[] _clipStack = new Rectangle[64];
    private int         _clipDepth;

    // ─────────────────────────────────────────────────────────────────────────
    // CONSTRUCTOR
    // ─────────────────────────────────────────────────────────────────────────

    public DrawList()
    {
        Vertices     = new VertexPositionColorTexture[INITIAL_VTX_CAPACITY];
        Indices      = new ushort[INITIAL_IDX_CAPACITY];
        Commands     = new DrawCmd[INITIAL_CMD_CAPACITY];
        TextCommands = new TextCmd[INITIAL_TEXT_CAPACITY];
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RESET POR FRAME
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Descarta toda la geometría del frame anterior. O(1): solo reinicia contadores.
    /// Los arrays NO se liberan → cero GC.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset(Rectangle fullScreenClip)
    {
        VtxCount     = 0;
        IdxCount     = 0;
        CmdCount     = 0;
        TextCmdCount = 0;
        _textPoolPos = 0;
        _clipDepth   = 0;

        // Empujamos el scissor de pantalla completa como base del stack.
        PushClipRect(fullScreenClip);
        // Añadimos el primer DrawCmd vacío que se irá llenando.
        AddDrawCmd(null, fullScreenClip);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SCISSOR RECT STACK
    // ─────────────────────────────────────────────────────────────────────────

    public void PushClipRect(Rectangle rect)
    {
        if (_clipDepth < _clipStack.Length)
            _clipStack[_clipDepth++] = rect;
    }

    public void PopClipRect()
    {
        if (_clipDepth > 1)
            _clipDepth--;
    }

    private ref Rectangle CurrentClip => ref _clipStack[_clipDepth - 1];

    // ─────────────────────────────────────────────────────────────────────────
    // GESTIÓN DE DRAW COMMANDS
    // ─────────────────────────────────────────────────────────────────────────

    private void AddDrawCmd(Texture2D? texture, Rectangle clip)
    {
        EnsureCmdCapacity();
        Commands[CmdCount++] = new DrawCmd
        {
            ElemCount = 0,
            ClipRect  = clip,
            Texture   = texture,
            VtxOffset = VtxCount,
            IdxOffset = IdxCount,
        };
    }

    /// <summary>
    /// Cambia la textura o el clip rect del comando activo.
    /// Si la textura o el clip cambian respecto al comando actual,
    /// se emite un nuevo DrawCmd → mínimos state changes en GPU.
    /// </summary>
    private void EnsureDrawCmd(Texture2D? texture, Rectangle clip)
    {
        if (CmdCount == 0)
        {
            AddDrawCmd(texture, clip);
            return;
        }

        ref DrawCmd cur = ref Commands[CmdCount - 1];
        if (cur.Texture != texture || cur.ClipRect != clip)
            AddDrawCmd(texture, clip);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void IncrementCmdElemCount(int count)
    {
        if (CmdCount > 0)
            Commands[CmdCount - 1].ElemCount += count;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PRIMITIVAS GEOMÉTRICAS
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Dibuja un rectángulo sólido (sin textura).
    /// Genera 4 vértices y 6 índices (2 triángulos).
    /// </summary>
    public void AddRectFilled(Rectangle rect, Color color)
    {
        AddRectFilled(
            new Vector2(rect.Left,  rect.Top),
            new Vector2(rect.Right, rect.Bottom),
            color,
            null);
    }

    /// <summary>
    /// Dibuja un rectángulo sólido con precisión de punto flotante.
    /// Ruta caliente: no hay bifurcaciones adicionales.
    /// </summary>
    public void AddRectFilled(Vector2 topLeft, Vector2 bottomRight, Color color, Texture2D? texture)
    {
        EnsureDrawCmd(texture, CurrentClip);
        EnsureVtxCapacity(4);
        EnsureIdxCapacity(6);

        ushort baseVtx = (ushort)VtxCount;

        // 4 vértices en orden TL→TR→BR→BL.
        // UV (0,0) → apunta al pixel blanco del atlas cuando texture==null.
        ref var v0 = ref Vertices[VtxCount++];
        v0.Position          = new Vector3(topLeft.X,      topLeft.Y,     0f);
        v0.Color             = color;
        v0.TextureCoordinate = Vector2.Zero;

        ref var v1 = ref Vertices[VtxCount++];
        v1.Position          = new Vector3(bottomRight.X,  topLeft.Y,     0f);
        v1.Color             = color;
        v1.TextureCoordinate = Vector2.UnitX;

        ref var v2 = ref Vertices[VtxCount++];
        v2.Position          = new Vector3(bottomRight.X,  bottomRight.Y, 0f);
        v2.Color             = color;
        v2.TextureCoordinate = Vector2.One;

        ref var v3 = ref Vertices[VtxCount++];
        v3.Position          = new Vector3(topLeft.X,      bottomRight.Y, 0f);
        v3.Color             = color;
        v3.TextureCoordinate = Vector2.UnitY;

        // Índices (winding CCW en coordenadas de pantalla top-left origin).
        Indices[IdxCount++] = baseVtx;
        Indices[IdxCount++] = (ushort)(baseVtx + 1);
        Indices[IdxCount++] = (ushort)(baseVtx + 2);
        Indices[IdxCount++] = baseVtx;
        Indices[IdxCount++] = (ushort)(baseVtx + 2);
        Indices[IdxCount++] = (ushort)(baseVtx + 3);

        IncrementCmdElemCount(6);
    }

    /// <summary>
    /// Dibuja un quad texturizado con UVs explícitas.
    /// Permite mapear subregiones de un atlas de texturas.
    /// </summary>
    public void AddImageQuad(
        Texture2D texture,
        Vector2 topLeft, Vector2 bottomRight,
        Vector2 uvTL, Vector2 uvBR,
        Color tint)
    {
        EnsureDrawCmd(texture, CurrentClip);
        EnsureVtxCapacity(4);
        EnsureIdxCapacity(6);

        ushort baseVtx = (ushort)VtxCount;

        ref var v0 = ref Vertices[VtxCount++];
        v0.Position          = new Vector3(topLeft.X,      topLeft.Y,     0f);
        v0.Color             = tint;
        v0.TextureCoordinate = uvTL;

        ref var v1 = ref Vertices[VtxCount++];
        v1.Position          = new Vector3(bottomRight.X,  topLeft.Y,     0f);
        v1.Color             = tint;
        v1.TextureCoordinate = new Vector2(uvBR.X, uvTL.Y);

        ref var v2 = ref Vertices[VtxCount++];
        v2.Position          = new Vector3(bottomRight.X,  bottomRight.Y, 0f);
        v2.Color             = tint;
        v2.TextureCoordinate = uvBR;

        ref var v3 = ref Vertices[VtxCount++];
        v3.Position          = new Vector3(topLeft.X,      bottomRight.Y, 0f);
        v3.Color             = tint;
        v3.TextureCoordinate = new Vector2(uvTL.X, uvBR.Y);

        Indices[IdxCount++] = baseVtx;
        Indices[IdxCount++] = (ushort)(baseVtx + 1);
        Indices[IdxCount++] = (ushort)(baseVtx + 2);
        Indices[IdxCount++] = baseVtx;
        Indices[IdxCount++] = (ushort)(baseVtx + 2);
        Indices[IdxCount++] = (ushort)(baseVtx + 3);

        IncrementCmdElemCount(6);
    }

    /// <summary>
    /// Dibuja el borde de un rectángulo como 4 quads delgados.
    /// <paramref name="thickness"/> es el ancho del borde en píxeles.
    /// </summary>
    public void AddRectOutline(Rectangle rect, Color color, float thickness = 1f)
    {
        float t  = thickness;
        float x0 = rect.Left;
        float y0 = rect.Top;
        float x1 = rect.Right;
        float y1 = rect.Bottom;

        // Top, Bottom, Left, Right – 4 quads delgados.
        AddRectFilled(new Vector2(x0,     y0),     new Vector2(x1, y0 + t), color, null); // Top
        AddRectFilled(new Vector2(x0,     y1 - t), new Vector2(x1, y1),     color, null); // Bottom
        AddRectFilled(new Vector2(x0,     y0 + t), new Vector2(x0 + t, y1 - t), color, null); // Left
        AddRectFilled(new Vector2(x1 - t, y0 + t), new Vector2(x1,     y1 - t), color, null); // Right
    }

    /// <summary>
    /// Dibuja una línea horizontal (optimizado: un solo quad).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddHorizontalLine(float x0, float x1, float y, Color color, float thickness = 1f)
        => AddRectFilled(new Vector2(x0, y), new Vector2(x1, y + thickness), color, null);

    /// <summary>
    /// Dibuja un triángulo sólido (3 vértices, 3 índices).
    /// Usado internamente para las flechas de Dropdown.
    /// </summary>
    public void AddTriangleFilled(Vector2 a, Vector2 b, Vector2 c, Color color)
    {
        EnsureDrawCmd(null, CurrentClip);
        EnsureVtxCapacity(3);
        EnsureIdxCapacity(3);

        ushort baseVtx = (ushort)VtxCount;

        ref var v0 = ref Vertices[VtxCount++];
        v0.Position = new Vector3(a.X, a.Y, 0f); v0.Color = color; v0.TextureCoordinate = Vector2.Zero;

        ref var v1 = ref Vertices[VtxCount++];
        v1.Position = new Vector3(b.X, b.Y, 0f); v1.Color = color; v1.TextureCoordinate = Vector2.Zero;

        ref var v2 = ref Vertices[VtxCount++];
        v2.Position = new Vector3(c.X, c.Y, 0f); v2.Color = color; v2.TextureCoordinate = Vector2.Zero;

        Indices[IdxCount++] = baseVtx;
        Indices[IdxCount++] = (ushort)(baseVtx + 1);
        Indices[IdxCount++] = (ushort)(baseVtx + 2);

        IncrementCmdElemCount(3);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // COMANDOS DE TEXTO (cero-alloc pool)
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Encola un comando de texto. El texto se copia al TextPool interno;
    /// no se guarda ninguna referencia externa → cero GC.
    /// </summary>
    public void AddText(Vector2 position, ReadOnlySpan<char> text, Color color, float scale = 1f)
    {
        if (text.IsEmpty) return;

        int available = _textPool.Length - _textPoolPos;
        int copyLen   = Math.Min(text.Length, available);

        if (copyLen <= 0) return; // Pool agotado; silently drop.

        int offset = _textPoolPos;
        text[..copyLen].CopyTo(new Span<char>(_textPool, _textPoolPos, copyLen));
        _textPoolPos += copyLen;

        EnsureTextCmdCapacity();

        TextCommands[TextCmdCount++] = new TextCmd
        {
            TextOffset = offset,
            TextLength = copyLen,
            Position   = position,
            Color      = color,
            ClipRect   = CurrentClip,
            Scale      = scale,
        };
    }

    /// <summary>
    /// Permite al backend leer un span de chars del pool de texto.
    /// Cero-allocation: slice directo del array interno.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<char> GetTextPoolSlice(int offset, int length)
        => new ReadOnlySpan<char>(_textPool, offset, length);

    // ─────────────────────────────────────────────────────────────────────────
    // GESTIÓN DE CAPACIDAD (resize manual, cero GC en steady-state)
    // ─────────────────────────────────────────────────────────────────────────

    // DECISIÓN: multiplicamos por 2 en cada resize, igual que List<T> y std::vector.
    // Esto amortiza el costo de copia a O(1) por inserción.
    // Los resizes ocurren solo al inicio (warm-up) o tras cambios drásticos de escena.

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureVtxCapacity(int needed)
    {
        if (VtxCount + needed > Vertices.Length)
        {
            int newSize = Math.Max(Vertices.Length * 2, VtxCount + needed);
            Array.Resize(ref Vertices, newSize);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureIdxCapacity(int needed)
    {
        if (IdxCount + needed > Indices.Length)
        {
            int newSize = Math.Max(Indices.Length * 2, IdxCount + needed);
            Array.Resize(ref Indices, newSize);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureCmdCapacity()
    {
        if (CmdCount >= Commands.Length)
            Array.Resize(ref Commands, Commands.Length * 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureTextCmdCapacity()
    {
        if (TextCmdCount >= TextCommands.Length)
            Array.Resize(ref TextCommands, TextCommands.Length * 2);
    }
}
