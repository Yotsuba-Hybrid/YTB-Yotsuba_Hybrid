// =============================================================================
// GuiIO.cs  –  YotsubaEngine Immediate Mode GUI
// =============================================================================
// Estado de entrada (mouse, teclado, texto) del frame actual.
//
// DECISIÓN DE DISEÑO – Estado por frame vs estado persistente
// ─────────────────────────────────────────────────────────────
// GuiIO mantiene dos categorías de estado:
//  a) Estado del frame actual (mouse pos, botones presionados): se sobreescribe
//     completamente cada vez que el usuario llama a GuiContext.NewFrame().
//  b) Estado persistente (historial de mouse para drag): se actualiza
//     incrementalmente.
//
// BUFFER DE TEXTO SIN ALOCACIONES
// ──────────────────────────────────
// Los eventos de texto (caracteres escritos) se acumulan en un array de chars
// fijo (InputCharBuffer). El tamaño máximo es INPUT_CHAR_BUFFER_SIZE por frame.
// La API de MonoGame Window.TextInput alimenta este buffer directamente.
// No se usa string ni List<char> → cero GC pressure en el hot-path de input.
// =============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;

namespace YotsubaEngine.Graphics.Gui;

/// <summary>
/// Estado de entrada del sistema GUI para el frame actual.
/// Actualizado por <see cref="GuiContext.NewFrame"/> antes de procesar la UI.
/// </summary>
public sealed class GuiIO
{
    // ─────────────────────────────────────────────────────────────────────────
    // CONSTANTES
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Capacidad máxima de caracteres escritos acumulados por frame.</summary>
    public const int INPUT_CHAR_BUFFER_SIZE = 64;

    // ─────────────────────────────────────────────────────────────────────────
    // ESTADO DE RATÓN
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Posición del cursor en píxeles de pantalla (coord. top-left origin).</summary>
    public Vector2 MousePos;

    /// <summary>Delta de posición respecto al frame anterior.</summary>
    public Vector2 MouseDelta;

    /// <summary>Botón izquierdo presionado este frame.</summary>
    public bool MouseDown;

    /// <summary>Botón izquierdo que se acaba de presionar este frame (rising edge).</summary>
    public bool MousePressed;

    /// <summary>Botón izquierdo que se acaba de soltar este frame (falling edge).</summary>
    public bool MouseReleased;

    /// <summary>Botón derecho presionado este frame.</summary>
    public bool MouseRightDown;

    /// <summary>Botón derecho rising edge.</summary>
    public bool MouseRightPressed;

    /// <summary>Delta de scroll del ratón (positivo = hacia arriba).</summary>
    public float MouseWheelDelta;

    /// <summary>Posición donde se presionó el botón izquierdo por última vez.</summary>
    public Vector2 MousePressedPos;

    // ─────────────────────────────────────────────────────────────────────────
    // ESTADO DE TECLADO
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Estado completo del teclado del frame actual.</summary>
    public KeyboardState KeyboardState;

    /// <summary>Estado completo del teclado del frame anterior (para detección de rising/falling).</summary>
    public KeyboardState PreviousKeyboardState;

    // ─────────────────────────────────────────────────────────────────────────
    // BUFFER DE TEXTO (cero alocaciones)
    // ─────────────────────────────────────────────────────────────────────────

    // Array fijo pre-alocado. Los caracteres escritos en el frame se depositan aquí.
    // No usamos string ni StringBuilder → cero GC en la ruta de input.
    private readonly char[] _inputCharBuffer = new char[INPUT_CHAR_BUFFER_SIZE];
    private int _inputCharCount;

    /// <summary>Número de caracteres acumulados en el buffer de texto del frame.</summary>
    public int InputCharCount => _inputCharCount;

    // ─────────────────────────────────────────────────────────────────────────
    // TIEMPO
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Delta time del frame en segundos.</summary>
    public float DeltaTime;

    /// <summary>Tiempo total transcurrido en segundos (para animaciones de cursor).</summary>
    public float TotalTime;

    // ─────────────────────────────────────────────────────────────────────────
    // DISPLAY
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Ancho de la pantalla/backbuffer en píxeles.</summary>
    public int DisplayWidth;

    /// <summary>Alto de la pantalla/backbuffer en píxeles.</summary>
    public int DisplayHeight;

    // ─────────────────────────────────────────────────────────────────────────
    // ESTADO INTERNO (para comparación entre frames)
    // ─────────────────────────────────────────────────────────────────────────

    private Vector2   _prevMousePos;
    private bool      _prevMouseDown;
    private bool      _prevMouseRightDown;
    private int       _prevScrollWheelValue;

    // ─────────────────────────────────────────────────────────────────────────
    // API PÚBLICA
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Actualiza el estado de IO a partir del hardware. Llamado automáticamente
    /// por <see cref="GuiContext.NewFrame"/>.
    /// </summary>
    internal void Update(
        MouseState    mouse,
        KeyboardState keyboard,
        float         deltaTime,
        float         totalTime,
        int           displayW,
        int           displayH)
    {
        // ── Display ──────────────────────────────────────────────────────────
        DisplayWidth  = displayW;
        DisplayHeight = displayH;

        // ── Tiempo ───────────────────────────────────────────────────────────
        DeltaTime = deltaTime;
        TotalTime = totalTime;

        // ── Ratón ─────────────────────────────────────────────────────────────
        MousePos   = new Vector2(mouse.X, mouse.Y);
        MouseDelta = MousePos - _prevMousePos;

        bool newLeft  = mouse.LeftButton  == ButtonState.Pressed;
        bool newRight = mouse.RightButton == ButtonState.Pressed;

        MouseDown          = newLeft;
        MousePressed       = newLeft  && !_prevMouseDown;
        MouseReleased      = !newLeft && _prevMouseDown;
        MouseRightDown     = newRight;
        MouseRightPressed  = newRight && !_prevMouseRightDown;

        if (MousePressed)
            MousePressedPos = MousePos;

        // El scroll wheel de MonoGame es acumulativo; tomamos el delta.
        MouseWheelDelta = (mouse.ScrollWheelValue - _prevScrollWheelValue) / 120f;

        // ── Teclado ──────────────────────────────────────────────────────────
        PreviousKeyboardState = KeyboardState;
        KeyboardState         = keyboard;

        // ── Guardar estado para el siguiente frame ────────────────────────────
        _prevMousePos         = MousePos;
        _prevMouseDown        = newLeft;
        _prevMouseRightDown   = newRight;
        _prevScrollWheelValue = mouse.ScrollWheelValue;

        // El buffer de texto se limpia aquí; los chars del frame anterior
        // se descartan antes de que el sistema de input añada los nuevos.
        _inputCharCount = 0;
    }

    /// <summary>
    /// Añade un carácter al buffer de texto del frame. Llamado por el handler de
    /// <c>Window.TextInput</c> en el backend de MonoGame.
    /// Zero-allocation: escribe directamente en el array pre-alocado.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void AddInputChar(char c)
    {
        // Descartamos Tab (ImGui convention) y caracteres de control excepto backspace/delete.
        if (c == '\t') return;

        if (_inputCharCount < INPUT_CHAR_BUFFER_SIZE)
            _inputCharBuffer[_inputCharCount++] = c;
        // Si el buffer está lleno, ignoramos el carácter → no overflow, no alloc.
    }

    /// <summary>
    /// Expone el buffer de texto como <see cref="System.ReadOnlySpan{char}"/>.
    /// Zero-allocation: devuelve un slice del array interno.
    /// </summary>
    public System.ReadOnlySpan<char> GetInputChars()
        => new System.ReadOnlySpan<char>(_inputCharBuffer, 0, _inputCharCount);

    // ─────────────────────────────────────────────────────────────────────────
    // HELPERS DE TECLADO
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>True si la tecla se acaba de presionar este frame (rising edge).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsKeyPressed(Keys key)
        => KeyboardState.IsKeyDown(key) && !PreviousKeyboardState.IsKeyDown(key);

    /// <summary>True si la tecla está mantenida presionada.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsKeyDown(Keys key) => KeyboardState.IsKeyDown(key);

    /// <summary>True si el rectángulo contiene la posición actual del ratón.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsMouseOver(in Rectangle rect)
        => rect.Contains((int)MousePos.X, (int)MousePos.Y);
}
