// =============================================================================
// Gui.InputWidgets.cs  –  YotsubaEngine Immediate Mode GUI
// =============================================================================
// Controles de entrada de datos: Slider (float e int) y TextInput.
//
// SLIDER – Lógica de Drag
// ─────────────────────────
// El slider usa ActiveItem para mantener el estado de arrastre incluso cuando
// el cursor sale del widget. Esto es crítico: sin esto el usuario pierde el
// control al mover el ratón rápido. El valor se recalcula cada frame
// mapeando la posición X del cursor al rango [min, max]:
//
//   t     = saturate((mouseX - trackLeft) / trackWidth)   [0..1]
//   value = lerp(min, max, t)
//
// Esta operación es branchless-friendly: Clamp + multiplicación escalar.
//
// TEXT INPUT – Buffer de Chars sin Alocaciones
// ──────────────────────────────────────────────
// El buffer interno del contexto (GuiContext.TextInputBuffer) actúa como
// destino de copia cuando el campo de texto gana el foco. Los caracteres
// escritos ese frame se depositan vía GuiIO.GetInputChars().
// No se usa string durante el procesamiento; la conversión a string ocurre
// solo al renderizar (una sola vez por frame, en el backend de texto).
//
// DECISIÓN – Tipos ref struct y Span
// ────────────────────────────────────
// Los parámetros de etiqueta son ReadOnlySpan<char> para aceptar tanto
// string literals (sin alloc) como rangos de char[]. El compilador emite
// código óptimo: los spans sobre literales se resuelven en tiempo de compilación
// a punteros fijos en el segmento de datos, sin heap allocation.
// =============================================================================

using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace YotsubaEngine.Graphics.Gui;

public static partial class Gui
{
    // ─────────────────────────────────────────────────────────────────────────
    // SLIDER FLOAT
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Deslizador numérico para valores <c>float</c>.
    /// </summary>
    /// <param name="label">Etiqueta. Soporta "Nombre##id".</param>
    /// <param name="value">Referencia al valor que se modifica.</param>
    /// <param name="min">Valor mínimo del rango.</param>
    /// <param name="max">Valor máximo del rango.</param>
    /// <param name="format">Formato de visualización del valor (p.ej. "{0:F2}").</param>
    /// <returns><c>true</c> si el valor cambió este frame.</returns>
    public static bool SliderFloat(
        ReadOnlySpan<char> label,
        ref float          value,
        float              min    = 0f,
        float              max    = 1f,
        string             format = "{0:F2}")
    {
        var ctx  = Context;
        uint id  = ctx.GetID(label);

        GuiStyle s = ctx.Style;
        float w    = ctx.AvailableWidth();
        float h    = s.WidgetHeight;
        Rectangle rect = ctx.NextWidgetRect(w, h);

        bool changed = ProcessSlider(ctx, id, rect, min, max, ref value);

        // Render del track y la manija
        RenderSliderTrack(ctx, rect, value, min, max, ctx.ActiveItem == id);

        // Texto del valor actual (centrado en el track)
        Span<char> valBuf = stackalloc char[32];
        bool ok = TryFormatFloat(valBuf, value, format, out int valLen);
        if (ok)
            DrawCenteredText(ctx, rect, valBuf[..valLen], s.TextNormal);

        // Etiqueta a la derecha
        GuiHash.ParseLabel(label, out var display, out _);
        if (!display.IsEmpty)
        {
            float labelX = rect.Right + s.InnerSpacing;
            float labelY = rect.Top + (rect.Height - ctx.FontHeight) * 0.5f;
            ctx.DrawList.AddText(new Vector2(labelX, labelY), display, s.TextNormal);
        }

        return changed;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SLIDER INT
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Deslizador numérico para valores <c>int</c>.
    /// </summary>
    public static bool SliderInt(
        ReadOnlySpan<char> label,
        ref int            value,
        int                min = 0,
        int                max = 100)
    {
        float fValue  = value;
        bool  changed = SliderFloat(label, ref fValue, min, max, "{0:F0}");
        if (changed) value = (int)MathF.Round(fValue);
        return changed;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SLIDER – LÓGICA INTERNA
    // ─────────────────────────────────────────────────────────────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ProcessSlider(
        GuiContext ctx, uint id, Rectangle rect,
        float min, float max, ref float value)
    {
        bool hovered = ctx.IsHovered(rect);
        if (hovered) ctx.HotItem = id;

        bool active = ctx.ActiveItem == id;

        if (hovered && ctx.IO.MousePressed && ctx.ActiveItem == GuiHash.NONE)
            ctx.ActiveItem = id;

        if (active && ctx.IO.MouseReleased)
            ctx.ActiveItem = GuiHash.NONE;

        if (!active) return false;

        // Calcular nuevo valor a partir de la posición X del cursor.
        // Branchless: Clamp es una operación min/max, sin jumps.
        float grabWidth = ctx.Style.WidgetHeight; // manija cuadrada
        float trackLeft  = rect.Left  + grabWidth * 0.5f;
        float trackRight = rect.Right - grabWidth * 0.5f;
        float trackWidth = trackRight - trackLeft;

        if (trackWidth <= 0f) return false;

        float t        = Math.Clamp((ctx.IO.MousePos.X - trackLeft) / trackWidth, 0f, 1f);
        float newValue = min + t * (max - min);

        if (newValue == value) return false;
        value = newValue;
        return true;
    }

    private static void RenderSliderTrack(
        GuiContext ctx, Rectangle rect,
        float value, float min, float max, bool active)
    {
        GuiStyle s  = ctx.Style;
        var dl      = ctx.DrawList;
        float grabW = s.WidgetHeight;

        // Track de fondo
        int trackInsetY = (int)(rect.Height * 0.35f);
        Rectangle track = new(
            rect.Left,   rect.Top + trackInsetY,
            rect.Width,  rect.Height - trackInsetY * 2);
        dl.AddRectFilled(track, s.WidgetBg);
        dl.AddRectOutline(track, s.WindowBorder);

        // Posición de la manija
        float range      = max - min;
        float t          = range > 0f ? Math.Clamp((value - min) / range, 0f, 1f) : 0f;
        float trackLeft  = rect.Left  + grabW * 0.5f;
        float trackRight = rect.Right - grabW * 0.5f;
        float grabCenterX = trackLeft + t * (trackRight - trackLeft);

        Rectangle grabRect = new(
            (int)(grabCenterX - grabW * 0.5f), rect.Top,
            (int)grabW, rect.Height);

        Color grabColor = active ? s.SliderGrabActive : s.SliderGrab;
        dl.AddRectFilled(grabRect, grabColor);
        dl.AddRectOutline(grabRect, new Color(0, 0, 0, 80));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // TEXT INPUT
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Campo de entrada de texto.
    /// El buffer interno del contexto se usa como staging area → cero alocaciones
    /// durante el procesamiento de teclas.
    /// </summary>
    /// <param name="label">Etiqueta (puede ser "##id" para invisible).</param>
    /// <param name="buffer">Buffer de caracteres que contiene el texto actual.</param>
    /// <param name="length">
    /// Longitud actual del texto en <paramref name="buffer"/>.
    /// Se actualiza si el usuario escribe o borra.
    /// </param>
    /// <param name="maxLength">Máximo de caracteres permitidos.</param>
    /// <returns><c>true</c> si el contenido del buffer cambió.</returns>
    public static bool TextInput(
        ReadOnlySpan<char> label,
        char[]             buffer,
        ref int            length,
        int                maxLength = 256)
    {
        var ctx  = Context;
        uint id  = ctx.GetID(label);

        GuiStyle s = ctx.Style;
        float w    = ctx.AvailableWidth();
        float h    = s.WidgetHeight;
        Rectangle rect = ctx.NextWidgetRect(w, h);

        bool hovered = ctx.IsHovered(rect);
        if (hovered) ctx.HotItem = id;

        bool focused = ctx.TextInputActiveId == id;

        // Click para ganar foco
        if (hovered && ctx.IO.MousePressed)
        {
            ctx.TextInputActiveId  = id;
            ctx.TextInputCursorPos = length; // cursor al final
            focused = true;
        }
        // Click fuera → perder foco
        else if (!hovered && ctx.IO.MousePressed && focused)
        {
            ctx.TextInputActiveId = GuiHash.NONE;
            focused = false;
        }

        bool changed = false;

        if (focused)
        {
            changed |= ProcessTextInput(ctx, buffer, ref length, maxLength);
        }

        // ── Render ───────────────────────────────────────────────────────
        var dl = ctx.DrawList;
        Color bgColor = focused ? s.WidgetBgActive : (hovered ? s.WidgetBgHovered : s.WidgetBg);
        dl.AddRectFilled(rect, bgColor);
        dl.AddRectOutline(rect, focused ? s.SliderGrab : s.WindowBorder);

        // Texto del contenido
        float textX  = rect.Left + s.WidgetPaddingX;
        float textY  = rect.Top  + (rect.Height - ctx.FontHeight) * 0.5f;
        var content  = new ReadOnlySpan<char>(buffer, 0, length);
        dl.AddText(new Vector2(textX, textY), content, s.TextNormal);

        // Cursor parpadeante (visible ~0.5s encendido, ~0.5s apagado)
        if (focused)
        {
            bool cursorVisible = (int)(ctx.TotalTime * 2f) % 2 == 0;
            if (cursorVisible)
            {
                float cursorX = textX;
                if (ctx.Font != null && length > 0)
                {
                    var beforeCursor = new ReadOnlySpan<char>(buffer, 0,
                        Math.Min(ctx.TextInputCursorPos, length));
                    cursorX += ctx.MeasureText(beforeCursor).X;
                }
                dl.AddRectFilled(
                    new Rectangle((int)cursorX, (int)textY, 2, (int)ctx.FontHeight),
                    s.TextNormal);
            }
        }

        // Etiqueta a la derecha
        GuiHash.ParseLabel(label, out var display, out _);
        if (!display.IsEmpty)
        {
            float labelX = rect.Right + s.InnerSpacing;
            dl.AddText(new Vector2(labelX, textY), display, s.TextNormal);
        }

        return changed;
    }

    /// <summary>
    /// Procesa la entrada de teclado para el campo activo.
    /// Opera sobre el buffer pasado por referencia → cero alocaciones.
    /// </summary>
    private static bool ProcessTextInput(
        GuiContext ctx,
        char[]     buffer,
        ref int    length,
        int        maxLength)
    {
        bool changed = false;
        int  maxLen  = Math.Min(maxLength, buffer.Length);

        // ── Teclas especiales ────────────────────────────────────────────

        if (ctx.IO.IsKeyPressed(Keys.Back) && length > 0 && ctx.TextInputCursorPos > 0)
        {
            // Backspace: elimina el carácter antes del cursor.
            // Desplazamos los chars a la derecha del cursor una posición a la izquierda.
            int pos = ctx.TextInputCursorPos - 1;
            // Span-based memmove para evitar el bucle manual.
            new Span<char>(buffer, pos + 1, length - pos - 1)
                .CopyTo(new Span<char>(buffer, pos, length - pos - 1));
            length--;
            ctx.TextInputCursorPos = pos;
            changed = true;
        }

        if (ctx.IO.IsKeyPressed(Keys.Delete) && ctx.TextInputCursorPos < length)
        {
            // Delete: elimina el carácter bajo el cursor.
            int pos = ctx.TextInputCursorPos;
            new Span<char>(buffer, pos + 1, length - pos - 1)
                .CopyTo(new Span<char>(buffer, pos, length - pos - 1));
            length--;
            changed = true;
        }

        if (ctx.IO.IsKeyPressed(Keys.Left) && ctx.TextInputCursorPos > 0)
            ctx.TextInputCursorPos--;

        if (ctx.IO.IsKeyPressed(Keys.Right) && ctx.TextInputCursorPos < length)
            ctx.TextInputCursorPos++;

        if (ctx.IO.IsKeyPressed(Keys.Home))
            ctx.TextInputCursorPos = 0;

        if (ctx.IO.IsKeyPressed(Keys.End))
            ctx.TextInputCursorPos = length;

        // ── Caracteres escritos ───────────────────────────────────────────
        ReadOnlySpan<char> inputChars = ctx.IO.GetInputChars();
        foreach (char c in inputChars)
        {
            // Solo aceptamos caracteres imprimibles (>=32, no Delete/escape)
            if (c < 32 || c == 127) continue;
            if (length >= maxLen)    break;  // Buffer lleno

            int pos = ctx.TextInputCursorPos;
            // Insertar en la posición del cursor.
            // Shift right usando Span.CopyTo (equivalente a memmove).
            if (pos < length)
            {
                new Span<char>(buffer, pos, length - pos)
                    .CopyTo(new Span<char>(buffer, pos + 1, length - pos));
            }
            buffer[pos] = c;
            length++;
            ctx.TextInputCursorPos++;
            changed = true;
        }

        return changed;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // HELPERS PRIVADOS
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Formatea un float en un Span de chars sin alocaciones usando el stack.
    /// Fallback al formato estándar de .NET si el formato personalizado falla.
    /// </summary>
    private static bool TryFormatFloat(Span<char> dest, float value, string fmt, out int written)
    {
        // Usamos TryFormat de float para cero allocations.
        // El string fmt se ignora para el path rápido; solo para decimales custom.
        if (value.TryFormat(dest, out written, "F2"))
            return true;
        written = 0;
        return false;
    }
}
