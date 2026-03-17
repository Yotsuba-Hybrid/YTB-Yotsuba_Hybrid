// =============================================================================
// GuiContext.cs  –  YotsubaEngine Immediate Mode GUI
// =============================================================================
// Núcleo del sistema de UI inmediato. Mantiene todo el estado mutable:
//  • IDs de control activo/caliente (HotItem / ActiveItem)
//  • Stack de layout para posicionamiento automático de widgets
//  • Stack de IDs para generación jerárquica de identificadores
//  • Estado persistente de ventanas, collapsibles y dropdowns
//  • Buffer de entrada de texto
//  • DrawList del frame actual
//
// DISEÑO DE ESTADOS HOT / ACTIVE (paradigma ImGui)
// ─────────────────────────────────────────────────
//  HotItem   = elemento bajo el cursor (candidato a interacción).
//  ActiveItem = elemento siendo activamente interactuado (ej. botón presionado,
//               slider siendo arrastrado, campo de texto enfocado).
//
//  Reglas de transición:
//    • HotItem se establece cada frame para el primer control bajo el cursor.
//    • ActiveItem se establece cuando MouseDown && HotItem == widget_id.
//    • ActiveItem se limpia cuando MouseReleased (para click-controls).
//    • Para drag-controls (Slider, Window title), ActiveItem persiste
//      mientras MouseDown sin importar si el cursor salió del widget.
//
// LAYOUT STACK (sin alocaciones por frame)
// ──────────────────────────────────────────
// Se usa un array de tamaño fijo de LayoutState structs. El "puntero" de
// escritura (_layoutDepth) se gestiona con push/pop. Máximo 64 niveles de
// anidamiento (suficiente para cualquier UI práctica).
//
// ESTADO PERSISTENTE
// ──────────────────
// WindowState, bool-states (collapsibles) y text-input state viven en
// Dictionary<uint, T>. Estos dicts se llenan una sola vez (primera vez que
// aparece un control) y luego solo se hacen lookups → mínimas alocaciones
// tras warm-up.
// =============================================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace YotsubaEngine.Graphics.Gui;

// ─────────────────────────────────────────────────────────────────────────────
// ESTRUCTURAS DE ESTADO INTERNO
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>Estado de una ventana (posición, tamaño, drag). Persistente entre frames.</summary>
internal sealed class WindowState
{
    public Vector2 Position;
    public Vector2 Size;
    public bool    IsCollapsed;
    public bool    IsDragging;
    public Vector2 DragOffset;     // Offset cursor→esquina TL en el momento del drag start
    public bool    WasUpdatedThisFrame;
}

/// <summary>Estado de layout de una sección activa (ventana o contenedor).</summary>
internal struct LayoutState
{
    /// <summary>Posición donde se dibujará el próximo widget (top-left).</summary>
    public Vector2 Cursor;

    /// <summary>Límite derecho del área de layout (para widgets que llenan el ancho).</summary>
    public float ContentMaxX;

    /// <summary>Límite inferior del área de layout (para clipping).</summary>
    public float ContentMaxY;

    /// <summary>Si true, el siguiente widget comparte línea con el anterior.</summary>
    public bool SameLine;

    /// <summary>Posición X guardada por SameLine().</summary>
    public float SameLineX;

    /// <summary>Altura de la línea actual (para avance correcto de cursor).</summary>
    public float LineHeight;

    /// <summary>ID del contenedor padre (para hashing jerárquico de IDs).</summary>
    public uint ParentId;

    /// <summary>Rectángulo de clip del contenedor.</summary>
    public Rectangle ClipRect;
}

// ─────────────────────────────────────────────────────────────────────────────
// GUICONTEXT
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Contexto central del sistema de UI inmediato de YotsubaEngine.
/// Una aplicación típicamente crea una instancia y la registra con <see cref="Gui.SetContext"/>.
/// </summary>
public sealed class GuiContext
{
    // ─────────────────────────────────────────────────────────────────────────
    // ESTADO DE INTERACCIÓN
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>ID del control bajo el cursor este frame.</summary>
    public uint HotItem    { get; internal set; } = GuiHash.NONE;

    /// <summary>ID del control siendo interactuado actualmente.</summary>
    public uint ActiveItem { get; internal set; } = GuiHash.NONE;

    // ─────────────────────────────────────────────────────────────────────────
    // IO, ESTILO Y DRAWLIST
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Estado de entrada del frame.</summary>
    public GuiIO IO { get; } = new GuiIO();

    /// <summary>Tema visual activo. Modificable en caliente.</summary>
    public GuiStyle Style = GuiStyle.Default;

    /// <summary>Lista de geometría del frame. Consumida por el render backend.</summary>
    public DrawList DrawList { get; } = new DrawList();

    /// <summary>
    /// Fuente utilizada para medir y renderizar texto.
    /// Debe asignarse antes de llamar a NewFrame().
    /// </summary>
    public SpriteFont? Font { get; set; }

    // ─────────────────────────────────────────────────────────────────────────
    // STACKS DE LAYOUT E ID (arrays fijos, cero GC)
    // ─────────────────────────────────────────────────────────────────────────

    private const int MAX_LAYOUT_DEPTH = 64;
    private const int MAX_ID_STACK     = 64;

    private LayoutState[] _layoutStack = new LayoutState[MAX_LAYOUT_DEPTH];
    private int           _layoutDepth = 0;

    // ID stack para scope manual (equivalente a ImGui.PushID / PopID)
    private uint[]        _idStack     = new uint[MAX_ID_STACK];
    private int           _idDepth     = 0;

    // ─────────────────────────────────────────────────────────────────────────
    // ESTADO PERSISTENTE
    // ─────────────────────────────────────────────────────────────────────────

    // Estos dicts son O(1) lookup (uint key → struct/class value).
    // Se poblan una vez; las queries subsecuentes no alocan.
    internal readonly Dictionary<uint, WindowState> WindowStates = new();
    internal readonly Dictionary<uint, bool>        BoolStates   = new(); // collapsibles, dropdowns

    // ─────────────────────────────────────────────────────────────────────────
    // TEXT INPUT STATE
    // ─────────────────────────────────────────────────────────────────────────

    private const int TEXT_INPUT_BUF_SIZE = 512;

    /// <summary>Buffer del campo de texto activo. Cero alocaciones por frame.</summary>
    internal readonly char[] TextInputBuffer = new char[TEXT_INPUT_BUF_SIZE];

    /// <summary>Número de caracteres actualmente en el buffer de texto.</summary>
    internal int TextInputLength;

    /// <summary>ID del campo de texto con foco activo (0 = ninguno).</summary>
    internal uint TextInputActiveId;

    /// <summary>Posición del cursor de inserción (índice dentro del buffer).</summary>
    internal int TextInputCursorPos;

    // ─────────────────────────────────────────────────────────────────────────
    // POPUP / DROPDOWN STATE
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>ID del popup/dropdown abierto. 0 = ninguno.</summary>
    internal uint OpenPopupId;

    // ─────────────────────────────────────────────────────────────────────────
    // ESTADO DE TIEMPO (para animaciones, cursor blink)
    // ─────────────────────────────────────────────────────────────────────────

    private float _totalTime;

    // ─────────────────────────────────────────────────────────────────────────
    // CONSTRUCTOR
    // ─────────────────────────────────────────────────────────────────────────

    /// <param name="font">SpriteFont para renderizar texto en la UI.</param>
    public GuiContext(SpriteFont? font = null)
    {
        Font = font;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // FRAME LIFECYCLE
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Inicia un nuevo frame de UI. Debe llamarse ANTES de cualquier widget.
    /// Actualiza IO, resetea HotItem y limpia el DrawList.
    /// </summary>
    public void NewFrame(
        MouseState    mouse,
        KeyboardState keyboard,
        GameTime      gameTime,
        int           displayW,
        int           displayH)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _totalTime += dt;

        IO.Update(mouse, keyboard, dt, _totalTime, displayW, displayH);

        // HotItem se recalcula cada frame por los controles.
        // Lo reseteamos aquí; los controles lo setearán si el cursor está encima.
        HotItem = GuiHash.NONE;

        // Si el ratón no está presionado y no hay item activo, limpiamos ActiveItem.
        // Esto maneja el caso de "click fuera de todo control".
        if (!IO.MouseDown && ActiveItem != GuiHash.NONE)
        {
            // Dejamos que los controles gestionen su propio limpiado de ActiveItem
            // en MouseReleased. Aquí no lo hacemos para no interferir con drags.
        }

        Rectangle screenRect = new(0, 0, displayW, displayH);
        DrawList.Reset(screenRect);

        // Reset layout stack
        _layoutDepth = 0;

        // Marcar todas las ventanas como no-actualizadas para detectar ventanas cerradas
        foreach (var ws in WindowStates.Values)
            ws.WasUpdatedThisFrame = false;
    }

    /// <summary>
    /// Finaliza el frame de UI. Debe llamarse DESPUÉS de todos los widgets.
    /// Aquí podríamos hacer validaciones o post-processing de la DrawList.
    /// </summary>
    public void EndFrame()
    {
        // Si quedó algún layout sin cerrar, lo descartamos silenciosamente.
        _layoutDepth = 0;
        _idDepth     = 0;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SISTEMA DE IDs
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calcula el ID de un control en el contexto del layout actual.
    /// El seed incluye el ID del contenedor padre para unicidad jerárquica.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal uint GetID(ReadOnlySpan<char> label)
    {
        uint seed = _layoutDepth > 0 ? _layoutStack[_layoutDepth - 1].ParentId : 0u;
        if (_idDepth > 0) seed ^= _idStack[_idDepth - 1];
        return GuiHash.ComputeId(label, seed);
    }

    /// <summary>Empuja un ID manual al stack (para diferenciación dentro de loops).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void PushId(uint id)
    {
        if (_idDepth < MAX_ID_STACK)
            _idStack[_idDepth++] = id;
    }

    /// <summary>Saca el último ID del stack.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void PopId()
    {
        if (_idDepth > 0) _idDepth--;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // LAYOUT STACK – API INTERNA
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Empuja un nuevo contexto de layout (usado por Begin() de ventanas/paneles).
    /// </summary>
    internal void PushLayout(in LayoutState state)
    {
        if (_layoutDepth < MAX_LAYOUT_DEPTH)
            _layoutStack[_layoutDepth++] = state;
    }

    /// <summary>Saca el contexto de layout actual (usado por End()).</summary>
    internal void PopLayout()
    {
        if (_layoutDepth > 0) _layoutDepth--;
    }

    /// <summary>
    /// Referencia al estado de layout activo. Permite modificación in-place.
    /// Devolver ref evita copias del struct.
    /// </summary>
    internal ref LayoutState CurrentLayout
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _layoutStack[_layoutDepth - 1];
    }

    internal bool HasActiveLayout => _layoutDepth > 0;

    // ─────────────────────────────────────────────────────────────────────────
    // LAYOUT – API DE POSICIONAMIENTO DE WIDGETS
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calcula y avanza el cursor para el próximo widget de altura <paramref name="height"/>.
    /// Devuelve el Rectangle donde debe dibujarse el widget.
    /// Si hay un SameLine pendiente, coloca el widget al lado del anterior.
    /// </summary>
    internal Rectangle NextWidgetRect(float preferredWidth, float height)
    {
        if (!HasActiveLayout)
            return new Rectangle(0, 0, (int)preferredWidth, (int)height);

        ref LayoutState L = ref CurrentLayout;

        float x, y;
        float w = preferredWidth > 0 ? preferredWidth : (L.ContentMaxX - L.Cursor.X);

        if (L.SameLine)
        {
            x = L.SameLineX;
            y = L.Cursor.Y - L.LineHeight - Style.ItemSpacing; // retrocedemos a Y anterior
            L.SameLine   = false;
            L.SameLineX  = x + w + Style.InnerSpacing;
        }
        else
        {
            x = L.Cursor.X;
            y = L.Cursor.Y;
            L.SameLineX = x + w + Style.InnerSpacing;
        }

        // Aseguramos que el ancho no rebase el límite del contenedor
        w = Math.Min(w, L.ContentMaxX - x);

        Rectangle rect = new((int)x, (int)y, (int)Math.Max(1f, w), (int)Math.Max(1f, height));

        // Avanzar cursor para el siguiente widget
        L.Cursor.Y  = y + height + Style.ItemSpacing;
        L.LineHeight = height;

        return rect;
    }

    /// <summary>
    /// Ancho disponible para el próximo widget (hasta el borde derecho del contenedor).
    /// </summary>
    internal float AvailableWidth()
        => HasActiveLayout ? CurrentLayout.ContentMaxX - CurrentLayout.Cursor.X : 100f;

    /// <summary>
    /// Fuerza que el próximo widget se coloque en la misma línea que el anterior.
    /// </summary>
    internal void SetNextSameLine()
    {
        if (!HasActiveLayout) return;
        ref LayoutState L = ref CurrentLayout;
        L.SameLine  = true;
        // SameLineX ya fue calculado en el último NextWidgetRect
    }

    // ─────────────────────────────────────────────────────────────────────────
    // HELPERS DE ESTADO PERSISTENTE
    // ─────────────────────────────────────────────────────────────────────────

    internal WindowState GetOrCreateWindowState(uint id, Vector2 defaultPos, Vector2 defaultSize)
    {
        if (!WindowStates.TryGetValue(id, out WindowState? ws))
        {
            ws = new WindowState { Position = defaultPos, Size = defaultSize };
            WindowStates[id] = ws;
        }
        ws.WasUpdatedThisFrame = true;
        return ws;
    }

    internal bool GetBoolState(uint id, bool defaultValue = false)
    {
        BoolStates.TryGetValue(id, out bool v);
        return BoolStates.ContainsKey(id) ? v : defaultValue;
    }

    internal void SetBoolState(uint id, bool value) => BoolStates[id] = value;

    // ─────────────────────────────────────────────────────────────────────────
    // HELPERS DE MEDICIÓN DE TEXTO
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Mide el ancho de un texto con la fuente activa.
    /// Devuelve 0 si no hay fuente asignada.
    /// </summary>
    internal Vector2 MeasureText(ReadOnlySpan<char> text, float scale = 1f)
    {
        if (Font is null || text.IsEmpty) return Vector2.Zero;
        // SpriteFont.MeasureString acepta string; convertimos el span.
        // Esta alocación solo ocurre durante el layout de texto.
        return Font.MeasureString(text.ToString()) * scale;
    }

    /// <summary>
    /// Altura de línea de la fuente activa (para sizing automático de widgets de texto).
    /// </summary>
    internal float FontHeight => Font?.LineSpacing ?? Style.WidgetHeight;

    // ─────────────────────────────────────────────────────────────────────────
    // HELPERS DE INTERACCIÓN
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Verifica si el cursor está sobre un rectángulo, considerando el clip rect activo.
    /// Integra el clip check para evitar "clicks fantasma" en áreas recortadas.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsHovered(Rectangle rect)
    {
        if (!IO.IsMouseOver(rect)) return false;
        // Si hay layout activo, el cursor debe estar dentro del clip del contenedor.
        if (HasActiveLayout)
        {
            Rectangle clip = CurrentLayout.ClipRect;
            if (!clip.Contains((int)IO.MousePos.X, (int)IO.MousePos.Y))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Tiempo total del contexto (para animaciones como parpadeo de cursor de texto).
    /// </summary>
    internal float TotalTime => _totalTime;
}
