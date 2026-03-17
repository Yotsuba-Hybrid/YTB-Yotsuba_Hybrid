// ============================================================================
// YotsubaEngine – Immediate Mode GUI System
// GuiControls.cs – Implementación de los 10 controles core.
//
// Cada control sigue el mismo patrón IMGUI:
//   1. Generar ID a partir del label.
//   2. Obtener rect del layout automático.
//   3. Evaluar interacción (hot/active) basándose en el input capturado.
//   4. Determinar estado visual (normal/hovered/active).
//   5. Emitir geometría al DrawList.
//   6. Retornar resultado (bool clicked, valor cambiado, etc).
//
// DECISIONES DE RENDIMIENTO:
// • Todos los métodos son estáticos para evitar dispatch virtual y permitir
//   que el JIT los inline cuando es posible.
// • Se pasan ReadOnlySpan<char> para labels; las constantes string se
//   convierten implícitamente sin copias.
// • La geometría se emite como quads directos al DrawList (4 verts + 6 idx)
//   sin intermediarios.
// • El texto se "simula" con quads proporcionales al largo del string.
//   Un sistema de font rendering real se integraría reemplazando estas
//   funciones por SpriteFont glyph quads, pero la interfaz del DrawList
//   permanece idéntica.
// ============================================================================

using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.IMGUI
{
    /// <summary>
    /// Implementación estática de todos los controles IMGUI.
    /// Cada método opera directamente sobre el GuiContext pasado por referencia.
    /// </summary>
    public static class GuiControls
    {
        // ================================================================
        // Helpers internos de texto
        //
        // NOTA: YotsubaEngine usa un atlas de fuentes externo.  Estas helpers
        // aproximan el tamaño del texto basándose en el conteo de caracteres
        // y el fontSize del Style.  Cuando se integre un SpriteFont real,
        // solo hay que reemplazar estas dos funciones.
        // ================================================================

        /// <summary>
        /// Estima el ancho en píxeles de un texto.
        /// Usa una aproximación monospace: cada carácter ocupa ~0.6 × fontSize.
        /// Para fuentes proporcionales reales, reemplazar por SpriteFont.MeasureString.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float MeasureTextWidth(ReadOnlySpan<char> text, float fontSize)
        {
            return text.Length * fontSize * 0.6f;
        }

        /// <summary>
        /// Emite un quad "placeholder" para cada carácter del texto.
        /// En un motor real, esto emitiría glyph quads desde el atlas de fuentes.
        /// Los quads se posicionan horizontalmente uno al lado del otro.
        ///
        /// IMPORTANTE: Esta es una implementación placeholder que dibuja un
        /// rectángulo semi-transparente donde iría el texto.  El sistema de
        /// font rendering real usaría el mismo DrawList.AddQuad() pero con
        /// UVs del atlas de glifos.  La API del DrawList NO cambia.
        /// </summary>
        internal static void EmitTextGeometry(GuiDrawList drawList, ReadOnlySpan<char> text,
            float x, float y, float fontSize, GuiColor32 color)
        {
            // Cada carácter se representa como un quad delgado.
            // El ancho de cada glifo es ~0.6 × fontSize (aproximación monospace).
            float charWidth = fontSize * 0.6f;
            float charHeight = fontSize;

            for (int i = 0; i < text.Length; i++)
            {
                float cx = x + i * charWidth;
                // Emitir un quad fino por cada carácter.
                // En un sistema real, los UVs apuntarían a la región del glifo en el atlas.
                drawList.AddQuad(
                    new GuiRect(cx, y, charWidth - 1f, charHeight),
                    color,
                    Vector2.Zero, Vector2.UnitX, Vector2.UnitY, Vector2.One
                );
            }
        }

        // ================================================================
        // 1. WINDOW / PANEL
        //
        // Contenedor base con:
        //   - Barra de título arrastreable.
        //   - Fondo semi-transparente.
        //   - Push de layout para posicionar controles hijos.
        //
        // Retorna true si el window está visible (no colapsado).
        // El caller debe invocar EndWindow() al terminar.
        // ================================================================

        public static bool BeginWindow(GuiContext ctx, ReadOnlySpan<char> title,
            float defaultX, float defaultY, float width, float height)
        {
            GuiId id = ctx.MakeId(title);
            ReadOnlySpan<char> visibleTitle = GuiHash.GetVisibleLabel(title);
            ref GuiStyle style = ref ctx.Style;

            // Obtener o inicializar posición persistente del window
            Vector2 pos = ctx.GetWindowPosition(id.Value, new Vector2(defaultX, defaultY));

            // ---- Rect de la barra de título ----
            var titleBarRect = new GuiRect(pos.X, pos.Y, width, style.WindowTitleBarHeight);

            // ---- Interacción: arrastre de la barra de título ----
            ctx.TrySetHot(id, titleBarRect);
            ctx.TrySetActive(id);

            if (ctx.ActiveItem == id && ctx.Mouse.LeftDown)
            {
                // Arrastre: mover el window con el delta del mouse
                // Se calcula el delta entre la posición actual del mouse y la
                // posición del frame anterior (almacenada implícitamente).
                // Usamos un enfoque simple: el window sigue al mouse.
                pos += new Vector2(
                    ctx.Mouse.Position.X - (titleBarRect.X + width * 0.5f),
                    ctx.Mouse.Position.Y - (titleBarRect.Y + style.WindowTitleBarHeight * 0.5f)
                ) * 0.15f; // Factor de suavizado para evitar saltos bruscos

                ctx.SetWindowPosition(id.Value, pos);
            }

            // Recalcular rects con la posición potencialmente actualizada
            titleBarRect = new GuiRect(pos.X, pos.Y, width, style.WindowTitleBarHeight);
            var windowRect = new GuiRect(pos.X, pos.Y, width, height);

            // ---- Geometría: fondo del window ----
            ctx.DrawList.AddQuad(windowRect, style.WindowBackground);

            // ---- Geometría: barra de título ----
            GuiColor32 titleColor = ctx.ActiveItem == id ? style.TitleBarActive : style.TitleBarBackground;
            ctx.DrawList.AddQuad(titleBarRect, titleColor);

            // ---- Geometría: texto del título ----
            float textX = pos.X + style.FramePadding;
            float textY = pos.Y + (style.WindowTitleBarHeight - style.FontSize) * 0.5f;
            EmitTextGeometry(ctx.DrawList, visibleTitle, textX, textY, style.FontSize, style.TextColor);

            // ---- Geometría: borde ----
            ctx.DrawList.AddRectBorder(windowRect, style.BorderColor);

            // ---- Push layout para contenido del window ----
            ctx.PushLayoutWithOffset(windowRect, id, style.WindowTitleBarHeight);

            return true;
        }

        public static void EndWindow(GuiContext ctx)
        {
            ctx.PopLayout();
        }

        // ================================================================
        // 2. BUTTON
        //
        // Botón clickeable estándar.
        // Retorna true el frame en que se produce el click (release sobre el botón).
        // ================================================================

        public static bool Button(GuiContext ctx, ReadOnlySpan<char> label)
        {
            GuiId id = ctx.MakeId(label);
            ReadOnlySpan<char> visibleLabel = GuiHash.GetVisibleLabel(label);
            ref GuiStyle style = ref ctx.Style;

            // Layout: reservar espacio
            GuiRect rect = ctx.AllocateRect(style.ControlHeight);

            // Interacción
            bool hovered = ctx.TrySetHot(id, rect);
            ctx.TrySetActive(id);
            bool clicked = ctx.WasClicked(id);

            // Color según estado
            GuiColor32 bgColor;
            if (ctx.ActiveItem == id)
                bgColor = style.ButtonActive;
            else if (hovered && ctx.HotItem == id)
                bgColor = style.ButtonHovered;
            else
                bgColor = style.ButtonNormal;

            // Geometría
            ctx.DrawList.AddQuad(rect, bgColor);
            ctx.DrawList.AddRectBorder(rect, style.BorderColor);

            // Texto centrado
            float textW = MeasureTextWidth(visibleLabel, style.FontSize);
            float textX = rect.X + (rect.W - textW) * 0.5f;
            float textY = rect.Y + (rect.H - style.FontSize) * 0.5f;
            EmitTextGeometry(ctx.DrawList, visibleLabel, textX, textY, style.FontSize, style.TextColor);

            return clicked;
        }

        // ================================================================
        // 3. CHECKBOX
        //
        // Casilla de verificación para valores booleanos.
        // Retorna true si el valor cambió este frame.
        // ================================================================

        public static bool Checkbox(GuiContext ctx, ReadOnlySpan<char> label, ref bool value)
        {
            GuiId id = ctx.MakeId(label);
            ReadOnlySpan<char> visibleLabel = GuiHash.GetVisibleLabel(label);
            ref GuiStyle style = ref ctx.Style;

            GuiRect rect = ctx.AllocateRect(style.ControlHeight);

            // El checkbox es un cuadrado a la izquierda + label a la derecha
            float boxSize = style.ControlHeight - 4f;
            var boxRect = new GuiRect(rect.X, rect.Y + 2f, boxSize, boxSize);

            // Interacción sobre el área completa (box + label)
            bool hovered = ctx.TrySetHot(id, rect);
            ctx.TrySetActive(id);
            bool clicked = ctx.WasClicked(id);

            if (clicked)
                value = !value;

            // Geometría: fondo del box
            GuiColor32 bgColor = (hovered && ctx.HotItem == id) ? style.FrameBackgroundHovered : style.FrameBackground;
            ctx.DrawList.AddQuad(boxRect, bgColor);
            ctx.DrawList.AddRectBorder(boxRect, style.BorderColor);

            // Geometría: checkmark (diagonal simplificada como un quad interior más pequeño)
            if (value)
            {
                var checkRect = new GuiRect(boxRect.X + 3f, boxRect.Y + 3f, boxRect.W - 6f, boxRect.H - 6f);
                ctx.DrawList.AddQuad(checkRect, style.CheckMark);
            }

            // Label a la derecha del box
            float labelX = boxRect.Right + style.ItemInnerSpacing;
            float labelY = rect.Y + (rect.H - style.FontSize) * 0.5f;
            EmitTextGeometry(ctx.DrawList, visibleLabel, labelX, labelY, style.FontSize, style.TextColor);

            return clicked;
        }

        // ================================================================
        // 4. SLIDER (float)
        //
        // Deslizador numérico horizontal.
        // Retorna true si el valor cambió este frame.
        // ================================================================

        public static bool SliderFloat(GuiContext ctx, ReadOnlySpan<char> label,
            ref float value, float min, float max)
        {
            GuiId id = ctx.MakeId(label);
            ReadOnlySpan<char> visibleLabel = GuiHash.GetVisibleLabel(label);
            ref GuiStyle style = ref ctx.Style;

            GuiRect rect = ctx.AllocateRect(style.ControlHeight);

            // Dividir el rect: 40% label izquierda, 60% slider derecha
            float sliderWidth = rect.W * 0.6f;
            float labelWidth = rect.W - sliderWidth - style.ItemInnerSpacing;

            var sliderRect = new GuiRect(rect.X + labelWidth + style.ItemInnerSpacing, rect.Y, sliderWidth, rect.H);

            // Interacción
            bool hovered = ctx.TrySetHot(id, sliderRect);
            ctx.TrySetActive(id);

            bool changed = false;
            float range = max - min;

            // Si está activo (arrastrando), calcular nuevo valor desde posición del mouse
            if (ctx.ActiveItem == id && ctx.Mouse.LeftDown && range > 0f)
            {
                // Cálculo branchless del ratio: clamp manual con MathF.Min/Max
                float ratio = (ctx.Mouse.Position.X - sliderRect.X) / sliderRect.W;
                ratio = MathF.Max(0f, MathF.Min(1f, ratio));

                float newValue = min + ratio * range;
                if (newValue != value)
                {
                    value = newValue;
                    changed = true;
                }
            }

            // Geometría: fondo del slider track
            ctx.DrawList.AddQuad(sliderRect, style.FrameBackground);

            // Geometría: grab (indicador de posición actual)
            if (range > 0f)
            {
                float ratio = (value - min) / range;
                ratio = MathF.Max(0f, MathF.Min(1f, ratio));
                float grabWidth = 12f;
                float grabX = sliderRect.X + ratio * (sliderRect.W - grabWidth);
                var grabRect = new GuiRect(grabX, sliderRect.Y, grabWidth, sliderRect.H);

                GuiColor32 grabColor = ctx.ActiveItem == id ? style.SliderGrabActive : style.SliderGrab;
                ctx.DrawList.AddQuad(grabRect, grabColor);
            }

            ctx.DrawList.AddRectBorder(sliderRect, style.BorderColor);

            // Label
            float labelY = rect.Y + (rect.H - style.FontSize) * 0.5f;
            EmitTextGeometry(ctx.DrawList, visibleLabel, rect.X, labelY, style.FontSize, style.TextColor);

            // Valor numérico superpuesto en el slider (como texto)
            // Formateamos el float a un span usando stackalloc para zero-alloc
            Span<char> valueBuf = stackalloc char[16];
            int written = FormatFloat(value, valueBuf);
            ReadOnlySpan<char> valueText = valueBuf[..written];

            float valTextW = MeasureTextWidth(valueText, style.FontSize);
            float valTextX = sliderRect.X + (sliderRect.W - valTextW) * 0.5f;
            EmitTextGeometry(ctx.DrawList, valueText, valTextX, labelY, style.FontSize, style.TextColor);

            return changed;
        }

        /// <summary>
        /// Slider para enteros.  Delega al slider float y redondea.
        /// </summary>
        public static bool SliderInt(GuiContext ctx, ReadOnlySpan<char> label,
            ref int value, int min, int max)
        {
            float fval = value;
            bool changed = SliderFloat(ctx, label, ref fval, min, max);
            int newVal = (int)MathF.Round(fval);
            if (newVal != value)
            {
                value = newVal;
                return true;
            }
            return changed;
        }

        // ================================================================
        // 5. TEXT INPUT
        //
        // Campo de entrada de texto con cursor.
        // Retorna true si el contenido cambió este frame.
        //
        // El buffer de caracteres se almacena en GuiContext.TextBuffers
        // y se reutiliza entre frames (zero allocation per frame).
        // ================================================================

        public static bool TextInput(GuiContext ctx, ReadOnlySpan<char> label,
            TextInputBuffer buffer)
        {
            GuiId id = ctx.MakeId(label);
            ReadOnlySpan<char> visibleLabel = GuiHash.GetVisibleLabel(label);
            ref GuiStyle style = ref ctx.Style;

            GuiRect rect = ctx.AllocateRect(style.ControlHeight);

            // Dividir: label izquierda (30%), input derecha (70%)
            float inputWidth = rect.W * 0.7f;
            float labelWidth = rect.W - inputWidth - style.ItemInnerSpacing;
            var inputRect = new GuiRect(rect.X + labelWidth + style.ItemInnerSpacing, rect.Y, inputWidth, rect.H);

            // Interacción
            bool hovered = ctx.TrySetHot(id, inputRect);
            ctx.TrySetActive(id);

            // Click da foco de teclado
            if (ctx.WasClicked(id))
            {
                ctx.FocusedItem = id;
            }

            bool hasFocus = ctx.FocusedItem == id;
            bool changed = false;

            // Procesar input de teclado si tenemos foco
            if (hasFocus)
            {
                int cursorPos = ctx.GetTextCursorPos(id.Value);

                // Caracteres tecleados
                unsafe
                {
                    fixed (char* chars = ctx.Keyboard.TypedChars)
                    {
                        for (int i = 0; i < ctx.Keyboard.TypedCharCount; i++)
                        {
                            buffer.InsertChar(cursorPos, chars[i]);
                            cursorPos++;
                            changed = true;
                        }
                    }
                }

                // Backspace
                if (ctx.Keyboard.BackspacePressed && cursorPos > 0)
                {
                    buffer.DeleteChar(cursorPos - 1);
                    cursorPos--;
                    changed = true;
                }

                // Delete
                if (ctx.Keyboard.DeletePressed && cursorPos < buffer.Length)
                {
                    buffer.DeleteChar(cursorPos);
                    changed = true;
                }

                // Navegación con flechas
                if (ctx.Keyboard.LeftArrowPressed && cursorPos > 0)
                    cursorPos--;
                if (ctx.Keyboard.RightArrowPressed && cursorPos < buffer.Length)
                    cursorPos++;
                if (ctx.Keyboard.HomePressed)
                    cursorPos = 0;
                if (ctx.Keyboard.EndPressed)
                    cursorPos = buffer.Length;

                ctx.SetTextCursorPos(id.Value, cursorPos);
            }

            // Geometría: fondo del input
            GuiColor32 bgColor = hasFocus ? style.FrameBackgroundActive
                : (hovered && ctx.HotItem == id) ? style.FrameBackgroundHovered
                : style.FrameBackground;
            ctx.DrawList.AddQuad(inputRect, bgColor);
            ctx.DrawList.AddRectBorder(inputRect, hasFocus ? style.CheckMark : style.BorderColor);

            // Texto del buffer
            float textX = inputRect.X + style.FramePadding;
            float textY = inputRect.Y + (inputRect.H - style.FontSize) * 0.5f;
            EmitTextGeometry(ctx.DrawList, buffer.AsSpan(), textX, textY, style.FontSize, style.TextColor);

            // Cursor parpadeante (cada ~30 frames cambia de visible a invisible)
            if (hasFocus)
            {
                int cp = ctx.GetTextCursorPos(id.Value);
                float cursorX = textX + cp * style.FontSize * 0.6f;
                // Dibujamos siempre el cursor (sin parpadeo por simplicidad; se podría
                // agregar un frame counter para alternar).
                ctx.DrawList.AddQuad(new GuiRect(cursorX, textY, 1.5f, style.FontSize), style.TextColor);
            }

            // Label
            float labelY = rect.Y + (rect.H - style.FontSize) * 0.5f;
            EmitTextGeometry(ctx.DrawList, visibleLabel, rect.X, labelY, style.FontSize, style.TextColor);

            return changed;
        }

        // ================================================================
        // 6. TEXT LABEL
        //
        // Etiqueta de texto estática (solo lectura).
        // No es interactiva, no retorna nada útil.
        // ================================================================

        public static void Label(GuiContext ctx, ReadOnlySpan<char> text)
        {
            ref GuiStyle style = ref ctx.Style;
            GuiRect rect = ctx.AllocateRect(style.FontSize + style.FramePadding * 2f);

            float textY = rect.Y + (rect.H - style.FontSize) * 0.5f;
            EmitTextGeometry(ctx.DrawList, text, rect.X, textY, style.FontSize, style.TextColor);
        }

        /// <summary>
        /// Label con color personalizado.
        /// </summary>
        public static void Label(GuiContext ctx, ReadOnlySpan<char> text, GuiColor32 color)
        {
            ref GuiStyle style = ref ctx.Style;
            GuiRect rect = ctx.AllocateRect(style.FontSize + style.FramePadding * 2f);

            float textY = rect.Y + (rect.H - style.FontSize) * 0.5f;
            EmitTextGeometry(ctx.DrawList, text, rect.X, textY, style.FontSize, color);
        }

        // ================================================================
        // 7. RADIO BUTTON
        //
        // Botón de opción para selecciones mutuamente exclusivas.
        // Retorna true si se seleccionó este radio (el caller debe asignar
        // el nuevo valor).
        // ================================================================

        public static bool RadioButton(GuiContext ctx, ReadOnlySpan<char> label, bool isSelected)
        {
            GuiId id = ctx.MakeId(label);
            ReadOnlySpan<char> visibleLabel = GuiHash.GetVisibleLabel(label);
            ref GuiStyle style = ref ctx.Style;

            GuiRect rect = ctx.AllocateRect(style.ControlHeight);

            // Área del "círculo" (representado como cuadrado; un sistema con
            // tesselación de arcos lo haría redondo).
            float boxSize = style.ControlHeight - 4f;
            var boxRect = new GuiRect(rect.X, rect.Y + 2f, boxSize, boxSize);

            bool hovered = ctx.TrySetHot(id, rect);
            ctx.TrySetActive(id);
            bool clicked = ctx.WasClicked(id);

            // Geometría: fondo circular (cuadrado como aproximación)
            GuiColor32 bgColor = (hovered && ctx.HotItem == id) ? style.FrameBackgroundHovered : style.FrameBackground;
            ctx.DrawList.AddQuad(boxRect, bgColor);
            ctx.DrawList.AddRectBorder(boxRect, style.BorderColor);

            // Indicador de selección (dot interior)
            if (isSelected)
            {
                float inset = boxSize * 0.25f;
                var dotRect = new GuiRect(boxRect.X + inset, boxRect.Y + inset,
                    boxSize - inset * 2f, boxSize - inset * 2f);
                ctx.DrawList.AddQuad(dotRect, style.CheckMark);
            }

            // Label
            float labelX = boxRect.Right + style.ItemInnerSpacing;
            float labelY = rect.Y + (rect.H - style.FontSize) * 0.5f;
            EmitTextGeometry(ctx.DrawList, visibleLabel, labelX, labelY, style.FontSize, style.TextColor);

            return clicked;
        }

        // ================================================================
        // 8. DROPDOWN / COMBOBOX
        //
        // Lista desplegable.  Solo un dropdown puede estar abierto a la vez.
        //
        // items:        array de labels (pre-alocado, no se crea cada frame).
        // selectedIndex: índice actualmente seleccionado (ref).
        // Retorna true si la selección cambió.
        // ================================================================

        public static bool Dropdown(GuiContext ctx, ReadOnlySpan<char> label,
            string[] items, ref int selectedIndex)
        {
            GuiId id = ctx.MakeId(label);
            ReadOnlySpan<char> visibleLabel = GuiHash.GetVisibleLabel(label);
            ref GuiStyle style = ref ctx.Style;

            GuiRect rect = ctx.AllocateRect(style.ControlHeight);

            // Dividir: label izquierda, combo derecha
            float comboWidth = rect.W * 0.6f;
            float labelWidth = rect.W - comboWidth - style.ItemInnerSpacing;
            var comboRect = new GuiRect(rect.X + labelWidth + style.ItemInnerSpacing, rect.Y, comboWidth, rect.H);

            bool hovered = ctx.TrySetHot(id, comboRect);
            ctx.TrySetActive(id);
            bool clicked = ctx.WasClicked(id);

            // Toggle del popup al hacer click
            bool isOpen = ctx.OpenPopupId == id;
            if (clicked)
            {
                ctx.OpenPopupId = isOpen ? GuiId.None : id;
                isOpen = !isOpen;
            }

            // Geometría: fondo del combo
            GuiColor32 bgColor = (hovered && ctx.HotItem == id) ? style.FrameBackgroundHovered : style.FrameBackground;
            ctx.DrawList.AddQuad(comboRect, bgColor);
            ctx.DrawList.AddRectBorder(comboRect, style.BorderColor);

            // Texto del item seleccionado
            if (selectedIndex >= 0 && selectedIndex < items.Length)
            {
                float textX = comboRect.X + style.FramePadding;
                float textY = comboRect.Y + (comboRect.H - style.FontSize) * 0.5f;
                EmitTextGeometry(ctx.DrawList, items[selectedIndex], textX, textY, style.FontSize, style.TextColor);
            }

            // Flecha de dropdown (triángulo apuntando hacia abajo)
            float arrowSize = 8f;
            float arrowX = comboRect.Right - arrowSize - style.FramePadding;
            float arrowY = comboRect.Y + (comboRect.H - arrowSize) * 0.5f;
            ctx.DrawList.AddTriangle(
                new Vector2(arrowX, arrowY),
                new Vector2(arrowX + arrowSize, arrowY),
                new Vector2(arrowX + arrowSize * 0.5f, arrowY + arrowSize),
                style.TextColor
            );

            // Label
            float labelY = rect.Y + (rect.H - style.FontSize) * 0.5f;
            EmitTextGeometry(ctx.DrawList, visibleLabel, rect.X, labelY, style.FontSize, style.TextColor);

            // Popup desplegable
            bool changed = false;
            if (isOpen && items.Length > 0)
            {
                float itemHeight = style.ControlHeight;
                float popupHeight = items.Length * itemHeight;
                var popupRect = new GuiRect(comboRect.X, comboRect.Bottom, comboRect.W, popupHeight);

                // Fondo del popup
                ctx.DrawList.AddQuad(popupRect, style.DropdownBackground);
                ctx.DrawList.AddRectBorder(popupRect, style.BorderColor);

                // Items del popup
                for (int i = 0; i < items.Length; i++)
                {
                    var itemRect = new GuiRect(popupRect.X, popupRect.Y + i * itemHeight, popupRect.W, itemHeight);

                    // Cada item del popup necesita su propio ID
                    // Usamos el ID del dropdown combinado con el índice
                    GuiId itemId = new GuiId(id.Value ^ (uint)(i + 1) * 2654435761u);

                    bool itemHovered = ctx.TrySetHot(itemId, itemRect);
                    ctx.TrySetActive(itemId);

                    // Highlight del item bajo el cursor
                    if (itemHovered && ctx.HotItem == itemId)
                    {
                        ctx.DrawList.AddQuad(itemRect, style.HeaderHovered);
                    }
                    else if (i == selectedIndex)
                    {
                        ctx.DrawList.AddQuad(itemRect, style.HeaderNormal);
                    }

                    // Texto del item
                    float itemTextX = itemRect.X + style.FramePadding;
                    float itemTextY = itemRect.Y + (itemRect.H - style.FontSize) * 0.5f;
                    EmitTextGeometry(ctx.DrawList, items[i], itemTextX, itemTextY, style.FontSize, style.TextColor);

                    // Click en item → seleccionar y cerrar
                    if (ctx.WasClicked(itemId))
                    {
                        if (i != selectedIndex)
                        {
                            selectedIndex = i;
                            changed = true;
                        }
                        ctx.OpenPopupId = GuiId.None;
                    }
                }
            }

            return changed;
        }

        // ================================================================
        // 9. PROGRESS BAR
        //
        // Barra de progreso visual (solo lectura, no interactiva).
        // fraction: valor entre 0.0 y 1.0.
        // ================================================================

        public static void ProgressBar(GuiContext ctx, ReadOnlySpan<char> label, float fraction)
        {
            GuiHash.GetVisibleLabel(label); // Consume el label para consistencia con otros controles
            ref GuiStyle style = ref ctx.Style;

            GuiRect rect = ctx.AllocateRect(style.ControlHeight);

            // Clamp sin branch
            fraction = MathF.Max(0f, MathF.Min(1f, fraction));

            // Fondo
            ctx.DrawList.AddQuad(rect, style.FrameBackground);

            // Barra de relleno
            if (fraction > 0f)
            {
                var fillRect = new GuiRect(rect.X, rect.Y, rect.W * fraction, rect.H);
                ctx.DrawList.AddQuad(fillRect, style.ProgressBarFill);
            }

            ctx.DrawList.AddRectBorder(rect, style.BorderColor);

            // Porcentaje como texto centrado
            Span<char> percentBuf = stackalloc char[8];
            int written = FormatPercent(fraction, percentBuf);
            ReadOnlySpan<char> percentText = percentBuf[..written];

            float textW = MeasureTextWidth(percentText, style.FontSize);
            float textX = rect.X + (rect.W - textW) * 0.5f;
            float textY = rect.Y + (rect.H - style.FontSize) * 0.5f;
            EmitTextGeometry(ctx.DrawList, percentText, textX, textY, style.FontSize, style.TextColor);

            // Label debajo de la barra (si hay espacio)
            ReadOnlySpan<char> visibleLabel = GuiHash.GetVisibleLabel(label);
            if (visibleLabel.Length > 0)
            {
                float labelY = rect.Bottom + 2f;
                EmitTextGeometry(ctx.DrawList, visibleLabel, rect.X, labelY, style.FontSize * 0.85f, style.TextDisabled);
                // Avanzar cursor manualmente para acomodar el label extra
                ref var layout = ref ctx.CurrentLayout;
                layout.CursorY += style.FontSize * 0.85f + style.ItemSpacing;
            }
        }

        // ================================================================
        // 10. COLLAPSIBLE HEADER / TREE NODE
        //
        // Encabezado colapsable que muestra/oculta controles anidados.
        // Retorna true si el nodo está expandido (los controles hijos deben
        // evaluarse solo si retorna true).
        // El caller debe invocar EndTreeNode() cuando termine de declarar hijos.
        // ================================================================

        public static bool BeginTreeNode(GuiContext ctx, ReadOnlySpan<char> label, bool defaultOpen = false)
        {
            GuiId id = ctx.MakeId(label);
            ReadOnlySpan<char> visibleLabel = GuiHash.GetVisibleLabel(label);
            ref GuiStyle style = ref ctx.Style;

            GuiRect rect = ctx.AllocateRect(style.ControlHeight);

            // Interacción
            bool hovered = ctx.TrySetHot(id, rect);
            ctx.TrySetActive(id);

            if (ctx.WasClicked(id))
            {
                ctx.ToggleCollapseState(id.Value);
            }

            bool collapsed = ctx.GetCollapseState(id.Value, defaultOpen);
            bool isOpen = !collapsed;

            // Geometría: fondo del header
            GuiColor32 bgColor;
            if (ctx.ActiveItem == id)
                bgColor = style.HeaderActive;
            else if (hovered && ctx.HotItem == id)
                bgColor = style.HeaderHovered;
            else
                bgColor = style.HeaderNormal;

            ctx.DrawList.AddQuad(rect, bgColor);

            // Flecha indicadora de estado (► cerrado, ▼ abierto)
            float arrowSize = 10f;
            float arrowX = rect.X + style.FramePadding;
            float arrowCenterY = rect.Y + rect.H * 0.5f;

            if (isOpen)
            {
                // Triángulo apuntando hacia abajo ▼
                ctx.DrawList.AddTriangle(
                    new Vector2(arrowX, arrowCenterY - arrowSize * 0.3f),
                    new Vector2(arrowX + arrowSize, arrowCenterY - arrowSize * 0.3f),
                    new Vector2(arrowX + arrowSize * 0.5f, arrowCenterY + arrowSize * 0.4f),
                    style.TextColor
                );
            }
            else
            {
                // Triángulo apuntando hacia la derecha ►
                ctx.DrawList.AddTriangle(
                    new Vector2(arrowX, arrowCenterY - arrowSize * 0.5f),
                    new Vector2(arrowX + arrowSize * 0.7f, arrowCenterY),
                    new Vector2(arrowX, arrowCenterY + arrowSize * 0.5f),
                    style.TextColor
                );
            }

            // Texto del header
            float textX = arrowX + arrowSize + style.ItemInnerSpacing;
            float textY = rect.Y + (rect.H - style.FontSize) * 0.5f;
            EmitTextGeometry(ctx.DrawList, visibleLabel, textX, textY, style.FontSize, style.TextColor);

            // Si está abierto, indentar los controles hijos
            if (isOpen)
            {
                ref var layout = ref ctx.CurrentLayout;
                layout.IndentX += style.IndentSpacing;
            }

            return isOpen;
        }

        public static void EndTreeNode(GuiContext ctx)
        {
            ref var layout = ref ctx.CurrentLayout;
            layout.IndentX -= ctx.Style.IndentSpacing;
            // Clamp para evitar indent negativo por llamadas desbalanceadas
            if (layout.IndentX < 0f) layout.IndentX = 0f;
        }

        // ================================================================
        // Utilidades de formateo zero-alloc
        //
        // Estas funciones escriben representaciones de texto directamente
        // en un Span<char> pre-alocado (stackalloc), evitando el boxing
        // y la generación de strings temporales que float.ToString() haría.
        // ================================================================

        /// <summary>
        /// Formatea un float con 2 decimales en un Span<char>.
        /// Retorna el número de caracteres escritos.
        /// </summary>
        internal static int FormatFloat(float value, Span<char> buffer)
        {
            // Manejar signo negativo
            int pos = 0;
            if (value < 0)
            {
                buffer[pos++] = '-';
                value = -value;
            }

            // Parte entera
            int integer = (int)value;
            int decimals = (int)((value - integer) * 100f + 0.5f);

            // Si el redondeo produce 100, ajustar
            if (decimals >= 100)
            {
                integer++;
                decimals -= 100;
            }

            // Escribir parte entera
            pos = WriteInt(integer, buffer, pos);

            // Punto decimal
            buffer[pos++] = '.';

            // Dos dígitos decimales (con padding de cero a la izquierda)
            buffer[pos++] = (char)('0' + decimals / 10);
            buffer[pos++] = (char)('0' + decimals % 10);

            return pos;
        }

        /// <summary>
        /// Formatea un porcentaje (ej. "73%") en un Span<char>.
        /// </summary>
        internal static int FormatPercent(float fraction, Span<char> buffer)
        {
            int pct = (int)(fraction * 100f + 0.5f);
            if (pct > 100) pct = 100;
            if (pct < 0) pct = 0;

            int pos = WriteInt(pct, buffer, 0);
            buffer[pos++] = '%';
            return pos;
        }

        /// <summary>
        /// Escribe un entero no-negativo en un buffer de caracteres.
        /// Retorna la nueva posición después del último dígito.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int WriteInt(int value, Span<char> buffer, int pos)
        {
            if (value == 0)
            {
                buffer[pos++] = '0';
                return pos;
            }

            // Calcular dígitos en orden inverso usando stackalloc
            Span<char> digits = stackalloc char[10];
            int count = 0;
            while (value > 0)
            {
                digits[count++] = (char)('0' + value % 10);
                value /= 10;
            }

            // Copiar en orden correcto
            for (int i = count - 1; i >= 0; i--)
            {
                buffer[pos++] = digits[i];
            }

            return pos;
        }
    }
}
