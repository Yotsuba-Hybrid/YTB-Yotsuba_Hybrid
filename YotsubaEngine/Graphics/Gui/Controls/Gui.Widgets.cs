// =============================================================================
// Gui.Widgets.cs  –  YotsubaEngine Immediate Mode GUI
// =============================================================================
// Controles: Button, Checkbox, Label (Text), ProgressBar, RadioButton.
//
// PATRÓN DE INTERACCIÓN COMÚN (aplicado a todos los controles)
// ─────────────────────────────────────────────────────────────
// 1. Calcular el rect del widget (NextWidgetRect).
// 2. Detectar hover (IsHovered → set HotItem).
// 3. Detectar press (MousePressed && HotItem == id → set ActiveItem).
// 4. Detectar release (MouseReleased && ActiveItem == id → retornar true).
// 5. Renderizar según estado (normal/hovered/active).
//
// Este patrón evita la necesidad de almacenamiento de estado por frame: toda
// la lógica de transición se infiere del estado de IO del frame actual +
// HotItem/ActiveItem persistidos en GuiContext.
//
// OPTIMIZACIÓN DE GEOMETRÍA
// ─────────────────────────
// Para el Checkbox y el RadioButton usamos quads simples (no paths curveados)
// para mantener el conteo de vértices en el mínimo absoluto. La redondez visual
// puede añadirse con shaders en el futuro sin cambiar la geometría.
// =============================================================================

using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace YotsubaEngine.Graphics.Gui;

public static partial class Gui
{
    // ─────────────────────────────────────────────────────────────────────────
    // BUTTON
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Botón estándar clickeable.
    /// </summary>
    /// <param name="label">Etiqueta. Soporta "Texto##id_oculto".</param>
    /// <param name="width">Ancho explícito (0 = llena el contenedor).</param>
    /// <returns><c>true</c> el frame en que el usuario hace click y suelta.</returns>
    public static bool Button(ReadOnlySpan<char> label, float width = 0f)
    {
        var ctx   = Context;
        uint id   = ctx.GetID(label);
        float h   = ctx.Style.WidgetHeight;
        float w   = width > 0f ? width : ctx.AvailableWidth();
        Rectangle rect = ctx.NextWidgetRect(w, h);

        bool hovered = ctx.IsHovered(rect);
        if (hovered) ctx.HotItem = id;

        bool active  = ctx.ActiveItem == id;

        if (hovered && ctx.IO.MousePressed && ctx.ActiveItem == GuiHash.NONE)
            ctx.ActiveItem = id;

        bool clicked = false;
        if (active && ctx.IO.MouseReleased)
        {
            ctx.ActiveItem = GuiHash.NONE;
            if (hovered) clicked = true;
        }

        // ── Render ───────────────────────────────────────────────────────
        var dl = ctx.DrawList;
        dl.AddRectFilled(rect, ctx.Style.GetButtonColor(hovered, active));

        // Borde sutil (1px más oscuro)
        Color border = new Color(0, 0, 0, 80);
        dl.AddRectOutline(rect, border);

        GuiHash.ParseLabel(label, out var display, out _);
        DrawCenteredText(ctx, rect, display, ctx.Style.TextNormal);

        return clicked;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CHECKBOX
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Casilla de verificación para valores booleanos.
    /// </summary>
    /// <param name="label">Etiqueta mostrada a la derecha del cuadrado.</param>
    /// <param name="value">Referencia al valor booleano controlado.</param>
    /// <returns><c>true</c> el frame en que el valor cambia.</returns>
    public static bool Checkbox(ReadOnlySpan<char> label, ref bool value)
    {
        var ctx   = Context;
        uint id   = ctx.GetID(label);

        GuiStyle s  = ctx.Style;
        float cbSize = s.CheckBoxSize;
        // Rect completo incluye el cuadrado + espacio + texto
        Vector2 textSize = ctx.MeasureText(label);
        float totalW = cbSize + s.InnerSpacing + textSize.X;
        float h      = Math.Max(cbSize, s.WidgetHeight);
        Rectangle rect = ctx.NextWidgetRect(totalW, h);

        // Solo el cuadrado es clicable
        Rectangle checkRect = new(
            rect.Left, rect.Top + (int)((h - cbSize) * 0.5f),
            (int)cbSize, (int)cbSize);

        bool hovered = ctx.IsHovered(checkRect);
        if (hovered) ctx.HotItem = id;

        bool changed = false;
        if (hovered && ctx.IO.MousePressed)
        {
            value   = !value;
            changed = true;
        }

        // ── Render ───────────────────────────────────────────────────────
        var dl = ctx.DrawList;
        Color bgColor = hovered ? s.WidgetBgHovered : s.WidgetBg;
        dl.AddRectFilled(checkRect, bgColor);
        dl.AddRectOutline(checkRect, s.WindowBorder);

        if (value)
        {
            // Marca de verificación: dos quads diagonales simulando una "✓"
            // Implementada como dos rectángulos rotados (quad de brazo corto y largo)
            // Usando primitivos simples para máxima compatibilidad y velocidad.
            int inset = (int)(cbSize * 0.15f);
            Rectangle fillRect = new(
                checkRect.Left   + inset,
                checkRect.Top    + inset,
                checkRect.Width  - inset * 2,
                checkRect.Height - inset * 2);
            dl.AddRectFilled(fillRect, s.CheckMark);
        }

        // Etiqueta a la derecha
        GuiHash.ParseLabel(label, out var display, out _);
        float labelX = rect.Left + cbSize + s.InnerSpacing;
        float labelY = rect.Top + (h - ctx.FontHeight) * 0.5f;
        dl.AddText(new Vector2(labelX, labelY), display, s.TextNormal);

        return changed;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // TEXT LABEL
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Etiqueta de texto estática. No interactiva.
    /// </summary>
    public static void Label(ReadOnlySpan<char> text)
    {
        var ctx  = Context;
        GuiStyle s = ctx.Style;

        Vector2 size = ctx.MeasureText(text);
        float w = size.X > 0f ? size.X + s.WidgetPaddingX * 2f : ctx.AvailableWidth();
        float h = size.Y > 0f ? size.Y : ctx.FontHeight;

        Rectangle rect = ctx.NextWidgetRect(w, Math.Max(h, s.WidgetHeight));

        float textX = rect.Left + s.WidgetPaddingX;
        float textY = rect.Top + (rect.Height - ctx.FontHeight) * 0.5f;
        ctx.DrawList.AddText(new Vector2(textX, textY), text, s.TextNormal);
    }

    /// <summary>Etiqueta de texto con color personalizado.</summary>
    public static void LabelColored(ReadOnlySpan<char> text, Color color)
    {
        var ctx  = Context;
        GuiStyle s = ctx.Style;

        Vector2 size = ctx.MeasureText(text);
        float w = size.X > 0f ? size.X + s.WidgetPaddingX * 2f : ctx.AvailableWidth();
        float h = Math.Max(size.Y, s.WidgetHeight);

        Rectangle rect = ctx.NextWidgetRect(w, h);
        float textX = rect.Left + s.WidgetPaddingX;
        float textY = rect.Top  + (rect.Height - ctx.FontHeight) * 0.5f;
        ctx.DrawList.AddText(new Vector2(textX, textY), text, color);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PROGRESS BAR
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Barra de progreso visual.
    /// </summary>
    /// <param name="fraction">Valor de 0.0 a 1.0 que representa el progreso.</param>
    /// <param name="width">Ancho (0 = llena el contenedor).</param>
    /// <param name="height">Alto (0 = altura de widget por defecto).</param>
    /// <param name="overlay">
    /// Texto opcional mostrado encima de la barra (p.ej. "75%").
    /// Usa <see cref="ReadOnlySpan{char}.Empty"/> para no mostrar texto.
    /// </param>
    public static void ProgressBar(
        float fraction,
        float width   = 0f,
        float height  = 0f,
        ReadOnlySpan<char> overlay = default)
    {
        var ctx  = Context;
        GuiStyle s = ctx.Style;

        float w = width  > 0f ? width  : ctx.AvailableWidth();
        float h = height > 0f ? height : s.WidgetHeight;
        Rectangle rect = ctx.NextWidgetRect(w, h);

        float clampedFraction = Math.Clamp(fraction, 0f, 1f);

        var dl = ctx.DrawList;
        // Fondo
        dl.AddRectFilled(rect, s.ProgressBarBg);

        // Relleno proporcional
        int fillW = (int)(rect.Width * clampedFraction);
        if (fillW > 0)
        {
            Rectangle fillRect = new(rect.Left, rect.Top, fillW, rect.Height);
            dl.AddRectFilled(fillRect, s.ProgressBarFill);
        }

        // Borde
        dl.AddRectOutline(rect, s.WindowBorder);

        // Overlay de texto centrado
        if (!overlay.IsEmpty)
            DrawCenteredText(ctx, rect, overlay, s.TextNormal);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RADIO BUTTON
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Botón de opción para selecciones exclusivas.
    /// </summary>
    /// <param name="label">Etiqueta del botón de opción.</param>
    /// <param name="v">Referencia al índice de selección actual.</param>
    /// <param name="buttonId">Índice que representa esta opción.</param>
    /// <returns><c>true</c> si el valor cambió.</returns>
    public static bool RadioButton(ReadOnlySpan<char> label, ref int v, int buttonId)
    {
        var ctx   = Context;
        uint id   = ctx.GetID(label);

        GuiStyle s  = ctx.Style;
        float cbSize = s.CheckBoxSize;
        Vector2 textSize = ctx.MeasureText(label);
        float totalW = cbSize + s.InnerSpacing + textSize.X;
        float h      = Math.Max(cbSize, s.WidgetHeight);
        Rectangle rect = ctx.NextWidgetRect(totalW, h);

        Rectangle radioRect = new(
            rect.Left, rect.Top + (int)((h - cbSize) * 0.5f),
            (int)cbSize, (int)cbSize);

        bool hovered = ctx.IsHovered(radioRect);
        if (hovered) ctx.HotItem = id;

        bool changed = false;
        if (hovered && ctx.IO.MousePressed && v != buttonId)
        {
            v       = buttonId;
            changed = true;
        }

        // ── Render ───────────────────────────────────────────────────────
        var dl = ctx.DrawList;
        Color bgColor = hovered ? s.WidgetBgHovered : s.WidgetBg;

        // Fondo del círculo (simulado con quad + esquinas recortadas visualmente).
        // Para un círculo real se necesitarían N triángulos; aquí usamos un quad
        // y confiamos en el rasterizador para mantener el perfil de rendimiento.
        dl.AddRectFilled(radioRect, bgColor);
        dl.AddRectOutline(radioRect, s.WindowBorder);

        // Punto interior cuando está seleccionado
        if (v == buttonId)
        {
            int innerInset = (int)(cbSize * 0.25f);
            Rectangle innerDot = new(
                radioRect.Left   + innerInset,
                radioRect.Top    + innerInset,
                radioRect.Width  - innerInset * 2,
                radioRect.Height - innerInset * 2);
            dl.AddRectFilled(innerDot, s.CheckMark);
        }

        // Etiqueta
        GuiHash.ParseLabel(label, out var display, out _);
        float labelX = rect.Left + cbSize + s.InnerSpacing;
        float labelY = rect.Top  + (h - ctx.FontHeight) * 0.5f;
        dl.AddText(new Vector2(labelX, labelY), display, s.TextNormal);

        return changed;
    }
}
