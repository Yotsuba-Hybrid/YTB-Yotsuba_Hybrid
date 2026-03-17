// ============================================================================
// YotsubaEngine – Immediate Mode GUI System
// GuiContext.cs – Contexto central que mantiene todo el estado del IMGUI.
//
// ARQUITECTURA:
// GuiContext es el "cerebro" del sistema.  Contiene:
//   1. Estado de input (ratón, teclado) capturado al inicio del frame.
//   2. IDs de los elementos Hot (bajo cursor) y Active (siendo interactuado).
//   3. La pila (stack) de layout para posicionar controles jerárquicamente.
//   4. El DrawList donde los controles empujan geometría.
//   5. Estado persistente entre frames (valores de TextInput, estados de
//      collapse, posiciones de windows, etc.) en un diccionario estable.
//
// DECISIONES DE RENDIMIENTO:
// • El layout stack usa un array fijo de tamaño razonable (32 niveles)
//   en lugar de un List<T> para evitar la indirección de heap y garantizar
//   que el JIT pueda acceder por offset constante.
// • El diccionario de estado persistente usa uint keys (GuiId) → O(1) lookup.
// • Toda la captura de input se hace UNA vez por frame (snapshot pattern)
//   para evitar inconsistencias si el estado del hardware cambia mid-frame.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace YotsubaEngine.IMGUI
{
    /// <summary>
    /// Contexto central del sistema IMGUI.  Debe existir una sola instancia
    /// por ventana de juego (singleton lógico, no estático forzado).
    /// </summary>
    public sealed class GuiContext
    {
        // ================================================================
        // Hot / Active state – corazón del paradigma Immediate Mode
        //
        // HotItem:    el control que está bajo el cursor ESTE frame.
        // ActiveItem: el control que está siendo interactuado (mouse down).
        //
        // Regla de transición (idéntica a Dear ImGui):
        //   - Mouse move → cualquier control bajo cursor se marca Hot.
        //   - Mouse press → si hay Hot, se convierte en Active.
        //   - Mouse release → Active se limpia; si el release ocurrió
        //     sobre el mismo control, es un "click".
        // ================================================================
        public GuiId HotItem;
        public GuiId ActiveItem;

        /// <summary>
        /// ID del item que tenía foco de teclado (para TextInput).
        /// Se persiste entre frames hasta que el usuario hace click en otro control.
        /// </summary>
        public GuiId FocusedItem;

        /// <summary>
        /// ID del dropdown/combobox actualmente abierto (solo uno a la vez).
        /// </summary>
        public GuiId OpenPopupId;

        // ================================================================
        // Input state – snapshot inmutable durante el frame
        // ================================================================
        public GuiMouseState Mouse;
        public GuiKeyboardState Keyboard;
        private MouseState _prevMouseState;
        private KeyboardState _prevKeyboardState;
        private bool _firstFrame = true;

        // ================================================================
        // Layout stack
        //
        // Usa un array fijo para evitar allocaciones.  32 niveles de
        // anidamiento es más que suficiente para cualquier UI práctica.
        // ================================================================
        private const int MaxLayoutDepth = 32;
        private readonly GuiLayoutEntry[] _layoutStack = new GuiLayoutEntry[MaxLayoutDepth];
        private int _layoutDepth;

        // ================================================================
        // Draw list
        // ================================================================
        public readonly GuiDrawList DrawList = new();

        // ================================================================
        // Style / Theme
        // ================================================================
        public GuiStyle Style;

        // ================================================================
        // Estado persistente entre frames
        //
        // Se usa un Dictionary<uint, T> para almacenar estado que debe
        // sobrevivir entre frames (posiciones de windows, texto editado,
        // estados de collapse, etc.).
        //
        // Alternativa considerada: flat array con open addressing.
        // Se eligió Dictionary por simplicidad; el overhead de GC es nulo
        // porque nunca se eliminan entradas durante gameplay normal.
        // ================================================================

        /// <summary>Estado de colapso para TreeNodes y Collapsible Headers.</summary>
        internal readonly Dictionary<uint, bool> CollapseState = new(64);

        /// <summary>Posición de arrastre de Windows (offset desde posición inicial).</summary>
        internal readonly Dictionary<uint, Vector2> WindowPositions = new(16);

        /// <summary>Buffers de texto para TextInput (pre-alocados por control).</summary>
        internal readonly Dictionary<uint, TextInputBuffer> TextBuffers = new(16);

        /// <summary>Posición del cursor de texto dentro de TextInput.</summary>
        internal readonly Dictionary<uint, int> TextCursorPositions = new(16);

        // ================================================================
        // Viewport
        // ================================================================
        public int ScreenWidth;
        public int ScreenHeight;

        // ================================================================
        // Textura blanca 1×1 para primitivas sin textura
        // ================================================================
        public Texture2D? WhitePixelTexture;

        // ================================================================
        // Constructor
        // ================================================================
        public GuiContext()
        {
            Style = GuiStyle.Default();
            HotItem = GuiId.None;
            ActiveItem = GuiId.None;
            FocusedItem = GuiId.None;
            OpenPopupId = GuiId.None;
        }

        // ================================================================
        // Frame lifecycle
        // ================================================================

        /// <summary>
        /// Se invoca al INICIO de cada frame, antes de evaluar cualquier control.
        /// Captura el estado de input y resetea el estado transitorio.
        /// </summary>
        public void BeginFrame(int screenWidth, int screenHeight)
        {
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;

            // Capturar input
            CaptureInput();

            // Resetear hot item – cada frame los controles "compiten" por ser hot
            HotItem = GuiId.None;

            // Limpiar el draw list para acumular geometría nueva
            DrawList.Clear();

            // Establecer textura por defecto (blanca 1×1 para primitivas sólidas)
            DrawList.SetTexture(WhitePixelTexture);
            DrawList.PushClipRect(new GuiRect(0, 0, screenWidth, screenHeight));

            // Resetear layout stack
            _layoutDepth = 0;

            // Empujar un layout raíz que cubre toda la pantalla
            PushLayout(new GuiRect(0, 0, screenWidth, screenHeight), GuiId.None);
        }

        /// <summary>
        /// Se invoca al FINAL de cada frame, después de evaluar todos los controles.
        /// Resuelve el estado de Active según los inputs capturados.
        /// </summary>
        public void EndFrame()
        {
            // Si el usuario hizo click pero ningún control reclamó ser Active,
            // liberar el focus de teclado (click en espacio vacío = desfocus).
            if (Mouse.LeftPressed && ActiveItem == GuiId.None)
            {
                FocusedItem = GuiId.None;
            }

            // Si el botón izquierdo no está presionado, liberar Active
            if (!Mouse.LeftDown)
            {
                ActiveItem = GuiId.None;
            }

            // Si se hizo click fuera del popup abierto, cerrarlo
            if (Mouse.LeftPressed && OpenPopupId != GuiId.None)
            {
                // El popup se cierra a menos que un control del popup haya
                // reclamado ser Hot (lo cual se maneja en el propio control).
                // Aquí simplemente dejamos que la lógica del Dropdown maneje esto.
            }

            // Pop del layout raíz
            if (_layoutDepth > 0)
                _layoutDepth = 0;
        }

        // ================================================================
        // Input capture
        // ================================================================

        private void CaptureInput()
        {
            var ms = Microsoft.Xna.Framework.Input.Mouse.GetState();
            var ks = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            if (_firstFrame)
            {
                _prevMouseState = ms;
                _prevKeyboardState = ks;
                _firstFrame = false;
            }

            // ---- Mouse ----
            Mouse.Position = new Vector2(ms.X, ms.Y);
            Mouse.LeftDown = ms.LeftButton == ButtonState.Pressed;
            Mouse.LeftPressed = ms.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Released;
            Mouse.LeftReleased = ms.LeftButton == ButtonState.Released && _prevMouseState.LeftButton == ButtonState.Pressed;
            Mouse.RightDown = ms.RightButton == ButtonState.Pressed;
            Mouse.RightPressed = ms.RightButton == ButtonState.Pressed && _prevMouseState.RightButton == ButtonState.Released;
            Mouse.RightReleased = ms.RightButton == ButtonState.Released && _prevMouseState.RightButton == ButtonState.Pressed;
            Mouse.ScrollDelta = (ms.ScrollWheelValue - _prevMouseState.ScrollWheelValue) / 120f;

            // ---- Keyboard ----
            Keyboard.TypedCharCount = 0; // Se llenará vía TextInput event de MonoGame
            Keyboard.BackspacePressed = ks.IsKeyDown(Keys.Back) && !_prevKeyboardState.IsKeyDown(Keys.Back);
            Keyboard.DeletePressed = ks.IsKeyDown(Keys.Delete) && !_prevKeyboardState.IsKeyDown(Keys.Delete);
            Keyboard.EnterPressed = ks.IsKeyDown(Keys.Enter) && !_prevKeyboardState.IsKeyDown(Keys.Enter);
            Keyboard.TabPressed = ks.IsKeyDown(Keys.Tab) && !_prevKeyboardState.IsKeyDown(Keys.Tab);
            Keyboard.EscapePressed = ks.IsKeyDown(Keys.Escape) && !_prevKeyboardState.IsKeyDown(Keys.Escape);
            Keyboard.LeftArrowPressed = ks.IsKeyDown(Keys.Left) && !_prevKeyboardState.IsKeyDown(Keys.Left);
            Keyboard.RightArrowPressed = ks.IsKeyDown(Keys.Right) && !_prevKeyboardState.IsKeyDown(Keys.Right);
            Keyboard.HomePressed = ks.IsKeyDown(Keys.Home) && !_prevKeyboardState.IsKeyDown(Keys.Home);
            Keyboard.EndPressed = ks.IsKeyDown(Keys.End) && !_prevKeyboardState.IsKeyDown(Keys.End);
            Keyboard.CtrlDown = ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl);
            Keyboard.ShiftDown = ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift);

            _prevMouseState = ms;
            _prevKeyboardState = ks;
        }

        /// <summary>
        /// Callback a conectar con GameWindow.TextInput para capturar caracteres
        /// tecleados (necesario para TextInput controls).
        /// Debe invocarse: window.TextInput += ctx.OnTextInput;
        /// </summary>
        public void OnTextInput(object? sender, TextInputEventArgs e)
        {
            char c = e.Character;
            // Filtrar caracteres de control excepto los que manejamos explícitamente
            if (c >= 32 || c == '\t')
            {
                Keyboard.PushChar(c);
            }
        }

        // ================================================================
        // Layout stack
        // ================================================================

        /// <summary>
        /// Empuja un nuevo contexto de layout (usado por Window, Panel, Group, TreeNode).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushLayout(GuiRect bounds, GuiId ownerId)
        {
            if (_layoutDepth >= MaxLayoutDepth)
                return; // Silenciosamente ignorar anidamientos excesivos

            float padding = Style.WindowPadding;
            ref var entry = ref _layoutStack[_layoutDepth++];
            entry.Bounds = bounds;
            entry.CursorX = bounds.X + padding;
            entry.CursorY = bounds.Y + padding;
            entry.ContentWidth = bounds.W - padding * 2f;
            entry.Padding = padding;
            entry.ItemSpacing = Style.ItemSpacing;
            entry.IndentX = 0;
            entry.OwnerId = ownerId;
        }

        /// <summary>
        /// Empuja un layout con un offset Y específico (para windows con title bar).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushLayoutWithOffset(GuiRect bounds, GuiId ownerId, float yOffset)
        {
            if (_layoutDepth >= MaxLayoutDepth)
                return;

            float padding = Style.WindowPadding;
            ref var entry = ref _layoutStack[_layoutDepth++];
            entry.Bounds = bounds;
            entry.CursorX = bounds.X + padding;
            entry.CursorY = bounds.Y + yOffset + padding;
            entry.ContentWidth = bounds.W - padding * 2f;
            entry.Padding = padding;
            entry.ItemSpacing = Style.ItemSpacing;
            entry.IndentX = 0;
            entry.OwnerId = ownerId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PopLayout()
        {
            if (_layoutDepth > 0) _layoutDepth--;
        }

        /// <summary>
        /// Referencia al layout actual (top of stack).  Permite a los controles
        /// leer y modificar CursorY directamente sin copia.
        /// </summary>
        public ref GuiLayoutEntry CurrentLayout
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _layoutStack[_layoutDepth - 1];
        }

        public int LayoutDepth => _layoutDepth;

        /// <summary>
        /// Reserva un rectángulo del layout actual para un control con la
        /// altura especificada.  Devuelve la posición y avanza el cursor.
        /// Este es el hot-path de todo el sistema de layout.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GuiRect AllocateRect(float height)
        {
            ref var layout = ref _layoutStack[_layoutDepth - 1];
            float x = layout.CursorX + layout.IndentX;
            float w = layout.ContentWidth - layout.IndentX;
            float y = layout.CursorY;

            var rect = new GuiRect(x, y, w, height);

            // Avanzar cursor para el siguiente control
            layout.CursorY = y + height + layout.ItemSpacing;

            return rect;
        }

        /// <summary>
        /// Reserva un rectángulo con ancho específico (para controles inline).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GuiRect AllocateRect(float width, float height)
        {
            ref var layout = ref _layoutStack[_layoutDepth - 1];
            float x = layout.CursorX + layout.IndentX;
            float y = layout.CursorY;

            var rect = new GuiRect(x, y, width, height);
            layout.CursorY = y + height + layout.ItemSpacing;

            return rect;
        }

        // ================================================================
        // Interaction helpers
        // ================================================================

        /// <summary>
        /// Intenta marcar un control como Hot si el cursor está dentro de su rect.
        /// Devuelve true si el control es ahora el HotItem.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetHot(GuiId id, GuiRect rect)
        {
            if (rect.Contains(Mouse.Position))
            {
                HotItem = id;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Intenta marcar un control como Active (cuando hay click sobre él).
        /// Debe llamarse después de TrySetHot.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetActive(GuiId id)
        {
            if (HotItem == id && Mouse.LeftPressed)
            {
                ActiveItem = id;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifica si un control fue "clickeado" (mouse released sobre él
        /// mientras era Active).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WasClicked(GuiId id)
        {
            return ActiveItem == id && HotItem == id && Mouse.LeftReleased;
        }

        /// <summary>
        /// Genera un GuiId jerárquico para un control, combinando el ID del
        /// layout propietario con el hash del label.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GuiId MakeId(ReadOnlySpan<char> label)
        {
            if (_layoutDepth > 0)
            {
                ref var layout = ref _layoutStack[_layoutDepth - 1];
                if (layout.OwnerId != GuiId.None)
                    return GuiHash.HashWithSeed(label, layout.OwnerId);
            }
            return GuiHash.Hash(label);
        }

        // ================================================================
        // Persistent state helpers
        // ================================================================

        /// <summary>
        /// Obtiene o crea un TextInputBuffer para un control de texto.
        /// El buffer se reutiliza entre frames (zero allocation per frame).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal TextInputBuffer GetOrCreateTextBuffer(uint id, int maxLength = 256)
        {
            if (!TextBuffers.TryGetValue(id, out var buffer))
            {
                buffer = new TextInputBuffer(maxLength);
                TextBuffers[id] = buffer;
            }
            return buffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetTextCursorPos(uint id)
        {
            TextCursorPositions.TryGetValue(id, out int pos);
            return pos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetTextCursorPos(uint id, int pos)
        {
            TextCursorPositions[id] = pos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool GetCollapseState(uint id, bool defaultOpen)
        {
            if (CollapseState.TryGetValue(id, out bool collapsed))
                return collapsed;
            CollapseState[id] = !defaultOpen;
            return !defaultOpen;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ToggleCollapseState(uint id)
        {
            CollapseState.TryGetValue(id, out bool current);
            CollapseState[id] = !current;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Vector2 GetWindowPosition(uint id, Vector2 defaultPos)
        {
            if (WindowPositions.TryGetValue(id, out var pos))
                return pos;
            WindowPositions[id] = defaultPos;
            return defaultPos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetWindowPosition(uint id, Vector2 pos)
        {
            WindowPositions[id] = pos;
        }
    }

    // ========================================================================
    // TextInputBuffer – Buffer de caracteres pre-alocado para controles de texto.
    //
    // Se aloca UNA vez cuando el usuario interactúa con un TextInput por primera
    // vez, y se reutiliza en todos los frames subsiguientes.  Las operaciones
    // de inserción/borrado operan in-place sin crear strings temporales.
    // ========================================================================
    public sealed class TextInputBuffer
    {
        private readonly char[] _buffer;
        private int _length;
        public int MaxLength { get; }

        public TextInputBuffer(int maxLength)
        {
            MaxLength = maxLength;
            _buffer = new char[maxLength];
            _length = 0;
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _length;
        }

        public ReadOnlySpan<char> AsSpan()
        {
            return _buffer.AsSpan(0, _length);
        }

        /// <summary>
        /// Copia el contenido actual a un string.  Solo llamar cuando
        /// el usuario necesita el valor final (ej. al presionar Enter).
        /// </summary>
        public override string ToString() => new string(_buffer, 0, _length);

        /// <summary>
        /// Establece el contenido desde un string existente.
        /// </summary>
        public void SetFrom(ReadOnlySpan<char> text)
        {
            int len = Math.Min(text.Length, MaxLength);
            text[..len].CopyTo(_buffer);
            _length = len;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InsertChar(int position, char c)
        {
            if (_length >= MaxLength || position < 0 || position > _length)
                return;

            // Desplazar caracteres hacia la derecha
            Array.Copy(_buffer, position, _buffer, position + 1, _length - position);
            _buffer[position] = c;
            _length++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DeleteChar(int position)
        {
            if (position < 0 || position >= _length)
                return;

            // Desplazar caracteres hacia la izquierda
            Array.Copy(_buffer, position + 1, _buffer, position, _length - position - 1);
            _length--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char GetChar(int index)
        {
            if (index < 0 || index >= _length) return '\0';
            return _buffer[index];
        }
    }
}
