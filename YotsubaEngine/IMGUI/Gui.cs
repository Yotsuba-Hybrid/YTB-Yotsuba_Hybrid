// ============================================================================
// YotsubaEngine – Immediate Mode GUI System
// Gui.cs – Fachada estática pública (API principal para el usuario del motor).
//
// PROPÓSITO:
// Provee una API estática fluida idéntica en espíritu a Dear ImGui:
//
//   Gui.Begin("Debug");
//   if (Gui.Button("Reset")) { ... }
//   Gui.SliderFloat("Speed", ref speed, 0f, 10f);
//   Gui.End();
//
// Internamente delega todo al GuiContext activo.  Solo puede haber un
// contexto activo a la vez (single-threaded, como Dear ImGui).
//
// DECISIONES DE DISEÑO:
// • API estática con contexto implícito: la inmensa mayoría del código de
//   gameplay solo necesita llamar Gui.Button(), Gui.Checkbox(), etc.
//   No debe preocuparse por pasar el contexto manualmente.
// • Los métodos aceptan string directamente (se convierte implícitamente
//   a ReadOnlySpan<char> por el compilador → zero-copy).
// ============================================================================

using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.IMGUI
{
    /// <summary>
    /// API estática de Immediate Mode GUI para YotsubaEngine.
    /// Inspirada directamente en la API de Dear ImGui.
    ///
    /// Uso típico:
    /// <code>
    /// // En Update():
    /// Gui.BeginFrame(screenWidth, screenHeight);
    ///
    /// Gui.Begin("Panel de Debug", 10, 10, 300, 400);
    /// Gui.Label("FPS: 60");
    /// if (Gui.Button("Reiniciar"))
    ///     ResetGame();
    /// Gui.SliderFloat("Velocidad", ref speed, 0, 100);
    /// Gui.End();
    ///
    /// Gui.EndFrame();
    ///
    /// // En Draw():
    /// guiRenderer.Render(Gui.GetDrawList());
    /// </code>
    /// </summary>
    public static class Gui
    {
        // ================================================================
        // Contexto singleton
        // ================================================================
        private static GuiContext? _ctx;

        /// <summary>
        /// Inicializa el sistema GUI.  Debe llamarse una vez al inicio del juego
        /// (ej. en LoadContent), pasando el GraphicsDevice para crear la textura
        /// blanca 1×1.
        /// </summary>
        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            _ctx = new GuiContext();

            // Crear textura blanca 1×1 para primitivas sin textura
            var whiteTex = new Texture2D(graphicsDevice, 1, 1);
            whiteTex.SetData(new[] { Microsoft.Xna.Framework.Color.White });
            _ctx.WhitePixelTexture = whiteTex;
        }

        /// <summary>
        /// Acceso al contexto para configuración avanzada (ej. conectar TextInput events).
        /// </summary>
        public static GuiContext Context => _ctx ?? throw new InvalidOperationException(
            "Gui.Initialize() must be called before using the GUI system.");

        /// <summary>
        /// Acceso al estilo para personalización del tema visual.
        /// </summary>
        public static ref GuiStyle Style => ref Context.Style;

        // ================================================================
        // Frame lifecycle
        // ================================================================

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BeginFrame(int screenWidth, int screenHeight)
        {
            Context.BeginFrame(screenWidth, screenHeight);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EndFrame()
        {
            Context.EndFrame();
        }

        /// <summary>
        /// Obtiene el DrawList del frame actual para pasarlo al renderer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GuiDrawList GetDrawList() => Context.DrawList;

        // ================================================================
        // Window
        // ================================================================

        /// <summary>
        /// Inicia un window/panel.  Debe cerrarse con End().
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Begin(string title, float x = 0, float y = 0, float width = 300, float height = 400)
        {
            return GuiControls.BeginWindow(Context, title, x, y, width, height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void End()
        {
            GuiControls.EndWindow(Context);
        }

        // ================================================================
        // Button
        // ================================================================

        /// <summary>
        /// Botón clickeable.  Retorna true el frame del click.
        /// <code>if (Gui.Button("Play")) StartGame();</code>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Button(string label)
        {
            return GuiControls.Button(Context, label);
        }

        // ================================================================
        // Checkbox
        // ================================================================

        /// <summary>
        /// Checkbox.  Retorna true si el valor cambió.
        /// <code>Gui.Checkbox("Fullscreen", ref isFullscreen);</code>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Checkbox(string label, ref bool value)
        {
            return GuiControls.Checkbox(Context, label, ref value);
        }

        // ================================================================
        // Sliders
        // ================================================================

        /// <summary>
        /// Slider de float.  Retorna true si el valor cambió.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SliderFloat(string label, ref float value, float min, float max)
        {
            return GuiControls.SliderFloat(Context, label, ref value, min, max);
        }

        /// <summary>
        /// Slider de int.  Retorna true si el valor cambió.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SliderInt(string label, ref int value, int min, int max)
        {
            return GuiControls.SliderInt(Context, label, ref value, min, max);
        }

        // ================================================================
        // Text Input
        // ================================================================

        /// <summary>
        /// Campo de entrada de texto.  Retorna true si el contenido cambió.
        /// El buffer se crea automáticamente y se reutiliza entre frames.
        /// </summary>
        public static bool TextInput(string label, TextInputBuffer buffer)
        {
            return GuiControls.TextInput(Context, label, buffer);
        }

        /// <summary>
        /// Campo de entrada de texto con buffer gestionado automáticamente.
        /// Usa un ID derivado del label para almacenar el buffer internamente.
        /// </summary>
        public static bool TextInput(string label, ref string text, int maxLength = 256)
        {
            var id = GuiHash.Hash(label);
            var buffer = Context.GetOrCreateTextBuffer(id.Value, maxLength);

            // Sincronizar string → buffer al primer uso o si el string externo cambió
            var bufSpan = buffer.AsSpan();
            if (!bufSpan.SequenceEqual(text.AsSpan()))
            {
                buffer.SetFrom(text);
            }

            bool changed = GuiControls.TextInput(Context, label, buffer);

            if (changed)
            {
                text = buffer.ToString(); // Solo aloca cuando realmente hay cambio
            }

            return changed;
        }

        // ================================================================
        // Label
        // ================================================================

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Label(string text)
        {
            GuiControls.Label(Context, text);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Label(string text, GuiColor32 color)
        {
            GuiControls.Label(Context, text, color);
        }

        // ================================================================
        // Radio Button
        // ================================================================

        /// <summary>
        /// Botón de radio.  Retorna true si fue clickeado.
        /// Uso típico:
        /// <code>
        /// if (Gui.RadioButton("Easy", difficulty == 0)) difficulty = 0;
        /// if (Gui.RadioButton("Hard", difficulty == 1)) difficulty = 1;
        /// </code>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RadioButton(string label, bool isSelected)
        {
            return GuiControls.RadioButton(Context, label, isSelected);
        }

        // ================================================================
        // Dropdown / ComboBox
        // ================================================================

        /// <summary>
        /// Lista desplegable.  Retorna true si la selección cambió.
        /// <code>
        /// string[] resolutions = { "1080p", "1440p", "4K" };
        /// Gui.Dropdown("Resolution", resolutions, ref selectedRes);
        /// </code>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Dropdown(string label, string[] items, ref int selectedIndex)
        {
            return GuiControls.Dropdown(Context, label, items, ref selectedIndex);
        }

        // ================================================================
        // Progress Bar
        // ================================================================

        /// <summary>
        /// Barra de progreso.  fraction debe estar entre 0.0 y 1.0.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ProgressBar(string label, float fraction)
        {
            GuiControls.ProgressBar(Context, label, fraction);
        }

        // ================================================================
        // Tree Node / Collapsible Header
        // ================================================================

        /// <summary>
        /// Encabezado colapsable.  Retorna true si está expandido.
        /// Los controles hijos solo deben evaluarse si retorna true.
        /// Debe cerrarse con EndTreeNode() si retornó true.
        /// <code>
        /// if (Gui.BeginTreeNode("Advanced Settings"))
        /// {
        ///     Gui.SliderFloat("Gamma", ref gamma, 0.5f, 3.0f);
        ///     Gui.EndTreeNode();
        /// }
        /// </code>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool BeginTreeNode(string label, bool defaultOpen = false)
        {
            return GuiControls.BeginTreeNode(Context, label, defaultOpen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EndTreeNode()
        {
            GuiControls.EndTreeNode(Context);
        }

        // ================================================================
        // Layout helpers
        // ================================================================

        /// <summary>
        /// Añade espacio vertical entre controles.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Space(float pixels = 8f)
        {
            Context.AllocateRect(pixels);
        }

        /// <summary>
        /// Añade un separador horizontal (línea fina).
        /// </summary>
        public static void Separator()
        {
            var rect = Context.AllocateRect(1f);
            Context.DrawList.AddQuad(rect, Context.Style.BorderColor);
        }

        /// <summary>
        /// Incrementa la indentación para los controles siguientes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Indent(float amount = 0f)
        {
            float indent = amount > 0 ? amount : Context.Style.IndentSpacing;
            ref var layout = ref Context.CurrentLayout;
            layout.IndentX += indent;
        }

        /// <summary>
        /// Decrementa la indentación.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unindent(float amount = 0f)
        {
            float indent = amount > 0 ? amount : Context.Style.IndentSpacing;
            ref var layout = ref Context.CurrentLayout;
            layout.IndentX = MathF.Max(0f, layout.IndentX - indent);
        }
    }
}
