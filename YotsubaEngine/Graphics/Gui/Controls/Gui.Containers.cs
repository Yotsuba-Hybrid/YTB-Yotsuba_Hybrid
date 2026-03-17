// =============================================================================
// Gui.Containers.cs  –  YotsubaEngine Immediate Mode GUI
// =============================================================================
// Controles contenedores: Window/Panel, Collapsible Header, Dropdown/ComboBox.
//
// VENTANA (Window / Panel)
// ─────────────────────────
// Un "window" en IMGUI es un panel arrastrable que envuelve otros widgets.
// Implementación sin alocaciones por frame:
//  • El estado (pos, size, collapsed) vive en GuiContext.WindowStates (dict persistente).
//  • Drag se detecta con ActiveItem == title_bar_id && MouseDown.
//  • Begin() retorna false si la ventana está colapsada → el usuario omite el cuerpo.
//  • End() hace pop del layout stack.
//
// COLLAPSIBLE HEADER
// ───────────────────
// Equivalente a ImGui::CollapsingHeader. Estado (abierto/cerrado) guardado en
// BoolStates. Retorna true si el header está expandido.
//
// DROPDOWN / COMBO BOX
// ──────────────────────
// Patrón Begin/End idéntico al de Dear ImGui:
//   if (Gui.BeginDropdown("Mode", items[idx])) {
//       foreach (item) if (Gui.SelectableItem(item)) ...
//       Gui.EndDropdown();
//   }
// El popup se renderiza al final del frame en un DrawList separado (encima de todo).
// =============================================================================

using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace YotsubaEngine.Graphics.Gui;

public static partial class Gui
{
    // ─────────────────────────────────────────────────────────────────────────
    // WINDOW / PANEL
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Inicia una ventana/panel arrastrable.
    /// </summary>
    /// <param name="title">Título de la ventana (también su ID).</param>
    /// <param name="defaultPos">Posición inicial (solo se aplica la primera vez).</param>
    /// <param name="defaultSize">Tamaño inicial.</param>
    /// <returns>
    /// <c>true</c> si la ventana está abierta y expandida (el usuario debe llamar <see cref="End"/>).
    /// <c>false</c> si está colapsada (NO llamar <see cref="End"/> en este caso).
    /// </returns>
    public static bool Begin(
        ReadOnlySpan<char> title,
        Vector2 defaultPos  = default,
        Vector2 defaultSize = default)
    {
        var ctx   = Context;
        uint id   = ctx.GetID(title);

        Vector2 initPos  = defaultPos  == default ? new Vector2(10, 10)   : defaultPos;
        Vector2 initSize = defaultSize == default ? new Vector2(300, 200) : defaultSize;

        WindowState ws = ctx.GetOrCreateWindowState(id, initPos, initSize);

        // ── Drag de la barra de título ────────────────────────────────────
        GuiStyle s     = ctx.Style;
        Rectangle titleBar = new(
            (int)ws.Position.X,
            (int)ws.Position.Y,
            (int)ws.Size.X,
            (int)s.TitleBarHeight);

        // ID único para la barra de título: combina el ID de la ventana con un sufijo fijo.
        // Esto garantiza que el drag de la barra no colisione con otros controles.
        uint titleId = GuiHash.HashWithSeed("TITLEBAR", id);

        bool titleHovered = ctx.IsHovered(titleBar);
        if (titleHovered)
            ctx.HotItem = titleId;

        // Iniciar drag
        if (titleHovered && ctx.IO.MousePressed && ctx.ActiveItem == GuiHash.NONE)
        {
            ctx.ActiveItem  = titleId;
            ws.IsDragging   = true;
            ws.DragOffset   = ctx.IO.MousePos - ws.Position;
        }

        // Actualizar posición durante drag
        if (ctx.ActiveItem == titleId && ctx.IO.MouseDown)
        {
            ws.Position = ctx.IO.MousePos - ws.DragOffset;
            // Constrain dentro de pantalla
            ws.Position.X = Math.Clamp(ws.Position.X, 0f, ctx.IO.DisplayWidth  - ws.Size.X);
            ws.Position.Y = Math.Clamp(ws.Position.Y, 0f, ctx.IO.DisplayHeight - s.TitleBarHeight);
        }
        else if (ctx.ActiveItem == titleId && ctx.IO.MouseReleased)
        {
            ctx.ActiveItem = GuiHash.NONE;
            ws.IsDragging  = false;
        }

        // Doble-click en barra de título → colapsar/expandir
        // (Simplificado: usamos doble-click detectado por tiempo entre clicks)
        // Por ahora: click derecho colapsa
        if (titleHovered && ctx.IO.MouseRightPressed)
            ws.IsCollapsed = !ws.IsCollapsed;

        // ── Renderizado de la ventana ─────────────────────────────────────
        var dl = ctx.DrawList;

        // Fondo completo de la ventana
        Rectangle windowRect = new(
            (int)ws.Position.X,
            (int)ws.Position.Y,
            (int)ws.Size.X,
            ws.IsCollapsed ? (int)s.TitleBarHeight : (int)ws.Size.Y);

        dl.AddRectFilled(windowRect, s.WindowBg);
        dl.AddRectOutline(windowRect, s.WindowBorder);

        // Barra de título
        Color titleColor = (ctx.ActiveItem == titleId) ? s.TitleBarBgActive : s.TitleBarBg;
        dl.AddRectFilled(titleBar, titleColor);

        // Texto del título
        GuiHash.ParseLabel(title, out ReadOnlySpan<char> displayTitle, out _);
        DrawLeftAlignedText(ctx, titleBar, displayTitle, s.TextNormal);

        // Indicador colapso: triángulo en la esquina izquierda
        float arrowSize = s.TitleBarHeight * 0.35f;
        float arrowX = ws.Position.X + s.WindowPaddingX * 0.3f;
        float arrowY = ws.Position.Y + s.TitleBarHeight * 0.5f;
        if (ws.IsCollapsed)
        {
            // Triángulo apuntando a la derecha (colapsado)
            dl.AddTriangleFilled(
                new Vector2(arrowX,              arrowY - arrowSize),
                new Vector2(arrowX + arrowSize * 1.3f, arrowY),
                new Vector2(arrowX,              arrowY + arrowSize),
                s.TextNormal);
        }
        else
        {
            // Triángulo apuntando hacia abajo (expandido)
            dl.AddTriangleFilled(
                new Vector2(arrowX,              arrowY - arrowSize * 0.5f),
                new Vector2(arrowX + arrowSize,  arrowY - arrowSize * 0.5f),
                new Vector2(arrowX + arrowSize * 0.5f, arrowY + arrowSize * 0.5f),
                s.TextNormal);
        }

        if (ws.IsCollapsed) return false;

        // ── Push de layout ────────────────────────────────────────────────
        float contentX    = ws.Position.X + s.WindowPaddingX;
        float contentY    = ws.Position.Y + s.TitleBarHeight + s.WindowPaddingY;
        float contentMaxX = ws.Position.X + ws.Size.X - s.WindowPaddingX;
        float contentMaxY = ws.Position.Y + ws.Size.Y - s.WindowPaddingY;

        Rectangle clipRect = new(
            (int)contentX, (int)contentY,
            (int)(contentMaxX - contentX), (int)(contentMaxY - contentY));

        dl.PushClipRect(clipRect);

        ctx.PushLayout(new LayoutState
        {
            Cursor      = new Vector2(contentX, contentY),
            ContentMaxX = contentMaxX,
            ContentMaxY = contentMaxY,
            ParentId    = id,
            ClipRect    = clipRect,
        });

        return true;
    }

    /// <summary>
    /// Cierra el bloque de ventana iniciado por <see cref="Begin"/>.
    /// SIEMPRE debe emparejarse con un <see cref="Begin"/> que retornó <c>true</c>.
    /// </summary>
    public static void End()
    {
        var ctx = Context;
        if (!ctx.HasActiveLayout) return;
        ctx.DrawList.PopClipRect();
        ctx.PopLayout();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // COLLAPSIBLE HEADER / TREE NODE
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Encabezado colapsable. Retorna <c>true</c> si está expandido.
    /// </summary>
    /// <param name="label">Etiqueta del encabezado.</param>
    /// <param name="defaultOpen">Si debe estar abierto por defecto.</param>
    /// <returns><c>true</c> si el contenido debe renderizarse.</returns>
    public static bool CollapsingHeader(ReadOnlySpan<char> label, bool defaultOpen = false)
    {
        var ctx   = Context;
        uint id   = ctx.GetID(label);

        // Obtener o crear estado persistente
        if (!ctx.BoolStates.ContainsKey(id))
            ctx.BoolStates[id] = defaultOpen;

        bool isOpen = ctx.GetBoolState(id, defaultOpen);

        float w    = ctx.AvailableWidth();
        float h    = ctx.Style.WidgetHeight;
        Rectangle rect = ctx.NextWidgetRect(w, h);

        bool hovered = ctx.IsHovered(rect);
        if (hovered) ctx.HotItem = id;

        if (hovered && ctx.IO.MousePressed)
        {
            isOpen = !isOpen;
            ctx.SetBoolState(id, isOpen);
        }

        // Renderizado
        var dl = ctx.DrawList;
        Color bg = ctx.Style.GetHeaderColor(hovered, ctx.ActiveItem == id);
        dl.AddRectFilled(rect, bg);

        // Triángulo indicador de estado
        float ax = rect.Left + ctx.Style.WidgetPaddingX;
        float ay = rect.Top  + rect.Height * 0.5f;
        float as2 = h * 0.22f;
        if (isOpen)
        {
            dl.AddTriangleFilled(
                new Vector2(ax, ay - as2 * 0.5f),
                new Vector2(ax + as2 * 2f, ay - as2 * 0.5f),
                new Vector2(ax + as2,      ay + as2),
                ctx.Style.TextNormal);
        }
        else
        {
            dl.AddTriangleFilled(
                new Vector2(ax,            ay - as2),
                new Vector2(ax + as2 * 1.5f, ay),
                new Vector2(ax,            ay + as2),
                ctx.Style.TextNormal);
        }

        // Texto del encabezado
        GuiHash.ParseLabel(label, out var display, out _);
        float textX = rect.Left + ctx.Style.WidgetPaddingX + as2 * 2f + ctx.Style.InnerSpacing;
        float textY = rect.Top + (rect.Height - ctx.FontHeight) * 0.5f;
        dl.AddText(new Vector2(textX, textY), display, ctx.Style.TextNormal);

        return isOpen;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // DROPDOWN / COMBO BOX
    // ─────────────────────────────────────────────────────────────────────────

    private const float DROPDOWN_ARROW_WIDTH = 20f;

    /// <summary>
    /// Inicia un Dropdown/ComboBox.
    /// </summary>
    /// <param name="label">Etiqueta del control (puede llevar ##id).</param>
    /// <param name="previewValue">Texto que se muestra cuando el dropdown está cerrado.</param>
    /// <returns><c>true</c> si el popup está abierto. En ese caso llamar <see cref="EndDropdown"/>.</returns>
    public static bool BeginDropdown(ReadOnlySpan<char> label, ReadOnlySpan<char> previewValue)
    {
        var ctx  = Context;
        uint id  = ctx.GetID(label);

        float w  = ctx.AvailableWidth();
        float h  = ctx.Style.WidgetHeight;
        Rectangle rect = ctx.NextWidgetRect(w, h);

        bool hovered = ctx.IsHovered(rect);
        if (hovered) ctx.HotItem = id;

        bool isOpen = ctx.OpenPopupId == id;

        if (hovered && ctx.IO.MousePressed)
        {
            ctx.OpenPopupId = isOpen ? GuiHash.NONE : id;
            isOpen = !isOpen;
        }

        // Cerrar si se hace click fuera
        if (isOpen && ctx.IO.MousePressed && !hovered)
        {
            ctx.OpenPopupId = GuiHash.NONE;
            isOpen = false;
        }

        // Renderizado del control base
        var dl    = ctx.DrawList;
        GuiStyle s = ctx.Style;

        Color bgColor = s.GetWidgetBg(hovered, isOpen);
        dl.AddRectFilled(rect, bgColor);
        dl.AddRectOutline(rect, s.WindowBorder);

        // Preview del valor seleccionado
        float textY = CenterTextY(rect, ctx.FontHeight);
        float textX = rect.Left + s.WidgetPaddingX;
        dl.AddText(new Vector2(textX, textY), previewValue, s.TextNormal);

        // Flecha indicadora (triángulo apuntando abajo)
        float ax = rect.Right - DROPDOWN_ARROW_WIDTH * 0.5f;
        float ay = rect.Top + rect.Height * 0.5f;
        float as2 = 4f;
        dl.AddTriangleFilled(
            new Vector2(ax - as2, ay - as2 * 0.5f),
            new Vector2(ax + as2, ay - as2 * 0.5f),
            new Vector2(ax,       ay + as2),
            s.TextNormal);

        // Etiqueta a la derecha del control
        GuiHash.ParseLabel(label, out var displayLabel, out _);
        if (!displayLabel.IsEmpty)
        {
            float labelX = rect.Right + s.InnerSpacing;
            dl.AddText(new Vector2(labelX, textY), displayLabel, s.TextNormal);
        }

        if (!isOpen) return false;

        // ── Push layout para los items del popup ─────────────────────────
        // El popup se renderiza justo debajo del control base.
        float popupX    = rect.Left;
        float popupY    = rect.Bottom;
        float popupMaxX = rect.Right;
        float popupMaxY = popupY + 200f; // Max altura del popup; items lo rellenan

        Rectangle popupBg = new(
            (int)popupX, (int)popupY,
            (int)(popupMaxX - popupX), 200);
        dl.AddRectFilled(popupBg, s.PopupBg);
        dl.AddRectOutline(popupBg, s.WindowBorder);

        Rectangle clipRect = new(
            (int)popupX + 1, (int)popupY + 1,
            (int)(popupMaxX - popupX) - 2, 198);

        dl.PushClipRect(clipRect);
        ctx.PushLayout(new LayoutState
        {
            Cursor      = new Vector2(popupX + s.WindowPaddingX, popupY + s.WindowPaddingY),
            ContentMaxX = popupMaxX - s.WindowPaddingX,
            ContentMaxY = popupMaxY,
            ParentId    = id,
            ClipRect    = clipRect,
        });

        return true;
    }

    /// <summary>
    /// Cierra el bloque de Dropdown iniciado por <see cref="BeginDropdown"/>.
    /// </summary>
    public static void EndDropdown()
    {
        var ctx = Context;
        if (!ctx.HasActiveLayout) return;
        ctx.DrawList.PopClipRect();
        ctx.PopLayout();
    }

    /// <summary>
    /// Item seleccionable dentro de un Dropdown.
    /// </summary>
    /// <param name="label">Texto del item.</param>
    /// <param name="selected">Si este item es el actualmente seleccionado.</param>
    /// <returns><c>true</c> si el usuario clickeó este item.</returns>
    public static bool SelectableItem(ReadOnlySpan<char> label, bool selected = false)
    {
        var ctx   = Context;
        uint id   = ctx.GetID(label);

        float w   = ctx.AvailableWidth();
        float h   = ctx.Style.WidgetHeight;
        Rectangle rect = ctx.NextWidgetRect(w, h);

        bool hovered = ctx.IsHovered(rect);
        if (hovered) ctx.HotItem = id;

        bool clicked = false;
        if (hovered && ctx.IO.MousePressed)
        {
            clicked = true;
            // Cerrar el dropdown al seleccionar
            ctx.OpenPopupId = GuiHash.NONE;
        }

        // Renderizado
        Color bg = selected
            ? ctx.Style.ButtonBg
            : (hovered ? ctx.Style.WidgetBgHovered : Color.Transparent);

        if (bg != Color.Transparent)
            ctx.DrawList.AddRectFilled(rect, bg);

        DrawLeftAlignedText(ctx, rect, label, ctx.Style.TextNormal);

        return clicked;
    }
}
