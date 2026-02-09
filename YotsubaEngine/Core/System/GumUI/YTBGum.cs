using Microsoft.Xna.Framework;
using Gum.Forms;
using Gum.Forms.Controls;
using System;

namespace YotsubaEngine.Core.System.GumUI
{
    /// <summary>
    /// Unified facade for GumUI providing a simple, fluent API.
    /// Fachada unificada para GumUI proporcionando una API simple y fluida.
    /// </summary>
    /// <remarks>
    /// This is the main entry point for using GumUI in YotsubaEngine.
    /// Use this class for the simplest way to add UI to your game.
    /// 
    /// Este es el punto de entrada principal para usar GumUI en YotsubaEngine.
    /// Usa esta clase para la forma más simple de agregar UI a tu juego.
    /// </remarks>
    /// <example>
    /// <code>
    /// // In Initialize:
    /// YTBGum.Initialize(this);
    /// YTBGum.AddButton("Click Me!", () => Console.WriteLine("Clicked!"), 100, 50);
    /// 
    /// // In Update:
    /// YTBGum.Update(gameTime);
    /// 
    /// // In Draw:
    /// YTBGum.Draw();
    /// </code>
    /// </example>
    public static class YTBGum
    {
        #region Initialization

        /// <summary>
        /// Gets whether GumUI has been initialized.
        /// Obtiene si GumUI ha sido inicializado.
        /// </summary>
        public static bool IsInitialized => YTBGumService.IsInitialized;

        /// <summary>
        /// Initializes GumUI with default settings.
        /// Inicializa GumUI con configuración predeterminada.
        /// </summary>
        /// <param name="game">The MonoGame Game instance. / La instancia del juego MonoGame.</param>
        public static void Initialize(Game game)
        {
            YTBGumService.Initialize(game);
        }

        /// <summary>
        /// Initializes GumUI with a specific visual version.
        /// Inicializa GumUI con una versión visual específica.
        /// </summary>
        /// <param name="game">The MonoGame Game instance. / La instancia del juego MonoGame.</param>
        /// <param name="version">Visual version. / Versión visual.</param>
        public static void Initialize(Game game, DefaultVisualsVersion version)
        {
            YTBGumService.Initialize(game, version);
        }

        /// <summary>
        /// Updates GumUI. Call in your game's Update method.
        /// Actualiza GumUI. Llama en el método Update de tu juego.
        /// </summary>
        public static void Update(GameTime gameTime) => YTBGumService.Update(gameTime);

        /// <summary>
        /// Draws GumUI. Call in your game's Draw method.
        /// Dibuja GumUI. Llama en el método Draw de tu juego.
        /// </summary>
        public static void Draw() => YTBGumService.Draw();

        /// <summary>
        /// Clears all UI elements.
        /// Limpia todos los elementos de UI.
        /// </summary>
        public static void Clear() => YTBGumService.ClearAll();

        #endregion

        #region Quick Control Creation

        /// <summary>
        /// Adds a button to the UI.
        /// Agrega un botón a la UI.
        /// </summary>
        public static Button AddButton(string text, Action onClick = null, float x = 0, float y = 0, float? width = null, float? height = null)
            => YTBGumControls.AddButton(text, onClick, x, y, width, height);

        /// <summary>
        /// Adds a label to the UI.
        /// Agrega una etiqueta a la UI.
        /// </summary>
        public static Label AddLabel(string text, float x = 0, float y = 0)
            => YTBGumControls.AddLabel(text, x, y);

        /// <summary>
        /// Adds a text box to the UI.
        /// Agrega una caja de texto a la UI.
        /// </summary>
        public static TextBox AddTextBox(string placeholder = "", float x = 0, float y = 0, float width = 200, Action<string> onTextChanged = null)
            => YTBGumControls.AddTextBox(placeholder, x, y, width, onTextChanged);

        /// <summary>
        /// Adds a password box to the UI.
        /// Agrega una caja de contraseña a la UI.
        /// </summary>
        public static PasswordBox AddPasswordBox(float x = 0, float y = 0, float width = 200)
            => YTBGumControls.AddPasswordBox(x, y, width);

        /// <summary>
        /// Adds a checkbox to the UI.
        /// Agrega una casilla de verificación a la UI.
        /// </summary>
        public static CheckBox AddCheckBox(string text, bool isChecked = false, float x = 0, float y = 0, Action<bool> onCheckedChanged = null)
            => YTBGumControls.AddCheckBox(text, isChecked, x, y, onCheckedChanged);

        /// <summary>
        /// Adds a radio button to the UI.
        /// Agrega un botón de radio a la UI.
        /// </summary>
        public static RadioButton AddRadioButton(string text, string groupName = "default", float x = 0, float y = 0, Action<bool> onCheckedChanged = null)
            => YTBGumControls.AddRadioButton(text, groupName, x, y, onCheckedChanged);

        /// <summary>
        /// Adds a combo box (dropdown) to the UI.
        /// Agrega un combo box (desplegable) a la UI.
        /// </summary>
        public static ComboBox AddComboBox(string[] items = null, float x = 0, float y = 0, float width = 150, Action<int> onSelectionChanged = null)
            => YTBGumControls.AddComboBox(items, x, y, width, onSelectionChanged);

        /// <summary>
        /// Adds a slider to the UI.
        /// Agrega un slider/deslizador a la UI.
        /// </summary>
        public static Slider AddSlider(double min = 0, double max = 100, double value = 50, float x = 0, float y = 0, float width = 200, Action<double> onValueChanged = null)
            => YTBGumControls.AddSlider(min, max, value, x, y, width, onValueChanged);

        /// <summary>
        /// Adds a list box to the UI.
        /// Agrega una lista a la UI.
        /// </summary>
        public static ListBox AddListBox(string[] items = null, float x = 0, float y = 0, float width = 200, float height = 150, Action<int> onSelectionChanged = null)
            => YTBGumControls.AddListBox(items, x, y, width, height, onSelectionChanged);

        /// <summary>
        /// Adds a scroll viewer to the UI.
        /// Agrega un scroll viewer a la UI.
        /// </summary>
        public static ScrollViewer AddScrollViewer(float x = 0, float y = 0, float width = 300, float height = 200)
            => YTBGumControls.AddScrollViewer(x, y, width, height);

        #endregion

        #region Quick Layout Creation

        /// <summary>
        /// Adds a vertical stack panel to the UI.
        /// Agrega un panel de apilado vertical a la UI.
        /// </summary>
        public static StackPanel AddVerticalStack(float x = 0, float y = 0)
            => YTBGumLayouts.AddVerticalStack(x, y);

        /// <summary>
        /// Adds a horizontal stack panel to the UI.
        /// Agrega un panel de apilado horizontal a la UI.
        /// </summary>
        public static StackPanel AddHorizontalStack(float x = 0, float y = 0)
            => YTBGumLayouts.AddHorizontalStack(x, y);

        /// <summary>
        /// Adds a panel container to the UI.
        /// Agrega un panel contenedor a la UI.
        /// </summary>
        public static Panel AddPanel(float x = 0, float y = 0, float width = 300, float height = 200)
            => YTBGumLayouts.AddPanel(x, y, width, height);

        #endregion

        #region Screen Properties

        /// <summary>
        /// Gets the screen width.
        /// Obtiene el ancho de la pantalla.
        /// </summary>
        public static float ScreenWidth => YTBGumService.ScreenWidth;

        /// <summary>
        /// Gets the screen height.
        /// Obtiene la altura de la pantalla.
        /// </summary>
        public static float ScreenHeight => YTBGumService.ScreenHeight;

        /// <summary>
        /// Gets the center X position of the screen.
        /// Obtiene la posición X central de la pantalla.
        /// </summary>
        public static float CenterX => ScreenWidth / 2;

        /// <summary>
        /// Gets the center Y position of the screen.
        /// Obtiene la posición Y central de la pantalla.
        /// </summary>
        public static float CenterY => ScreenHeight / 2;

        #endregion
    }
}
