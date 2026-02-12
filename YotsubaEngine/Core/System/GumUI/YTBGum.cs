using Microsoft.Xna.Framework;
using Gum.Forms;
using Gum.Forms.Controls;
using System;

namespace YotsubaEngine.Core.System.GumUI
{
    /// <summary>
    /// Fachada unificada para GumUI proporcionando una API simple y fluida.
    /// <para>Unified facade for GumUI providing a simple, fluent API.</para>
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
        /// Obtiene si GumUI ha sido inicializado.
        /// <para>Gets whether GumUI has been initialized.</para>
        /// </summary>
        public static bool IsInitialized => YTBGumService.IsInitialized;

        /// <summary>
        /// Inicializa GumUI con configuración predeterminada.
        /// <para>Initializes GumUI with default settings.</para>
        /// </summary>
        /// <param name="game">Instancia del juego MonoGame. <para>The MonoGame Game instance.</para></param>
        public static void Initialize(Game game)
        {
            YTBGumService.Initialize(game);
        }

        /// <summary>
        /// Inicializa GumUI con una versión visual específica.
        /// <para>Initializes GumUI with a specific visual version.</para>
        /// </summary>
        /// <param name="game">Instancia del juego MonoGame. <para>The MonoGame Game instance.</para></param>
        /// <param name="version">Versión visual. <para>Visual version.</para></param>
        public static void Initialize(Game game, DefaultVisualsVersion version)
        {
            YTBGumService.Initialize(game, version);
        }

        /// <summary>
        /// Actualiza GumUI. Llama en el método Update de tu juego.
        /// <para>Updates GumUI. Call in your game's Update method.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public static void Update(GameTime gameTime) => YTBGumService.Update(gameTime);

        /// <summary>
        /// Dibuja GumUI. Llama en el método Draw de tu juego.
        /// <para>Draws GumUI. Call in your game's Draw method.</para>
        /// </summary>
        public static void Draw() => YTBGumService.Draw();

        /// <summary>
        /// Limpia todos los elementos de UI.
        /// <para>Clears all UI elements.</para>
        /// </summary>
        public static void Clear() => YTBGumService.ClearAll();

        #endregion

        #region Quick Control Creation

        /// <summary>
        /// Agrega un botón a la UI.
        /// <para>Adds a button to the UI.</para>
        /// </summary>
        /// <param name="text">Texto del botón. <para>Button text.</para></param>
        /// <param name="onClick">Acción al hacer clic. <para>Click action.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho opcional. <para>Optional width.</para></param>
        /// <param name="height">Alto opcional. <para>Optional height.</para></param>
        /// <returns>Botón creado. <para>Created button.</para></returns>
        public static Button AddButton(string text, Action onClick = null, float x = 0, float y = 0, float? width = null, float? height = null)
            => YTBGumControls.AddButton(text, onClick, x, y, width, height);

        /// <summary>
        /// Agrega una etiqueta a la UI.
        /// <para>Adds a label to the UI.</para>
        /// </summary>
        /// <param name="text">Texto de la etiqueta. <para>Label text.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <returns>Etiqueta creada. <para>Created label.</para></returns>
        public static Label AddLabel(string text, float x = 0, float y = 0)
            => YTBGumControls.AddLabel(text, x, y);

        /// <summary>
        /// Agrega una caja de texto a la UI.
        /// <para>Adds a text box to the UI.</para>
        /// </summary>
        /// <param name="placeholder">Texto inicial. <para>Placeholder text.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <param name="onTextChanged">Acción al cambiar el texto. <para>Action when text changes.</para></param>
        /// <returns>TextBox creado. <para>Created text box.</para></returns>
        public static TextBox AddTextBox(string placeholder = "", float x = 0, float y = 0, float width = 200, Action<string> onTextChanged = null)
            => YTBGumControls.AddTextBox(placeholder, x, y, width, onTextChanged);

        /// <summary>
        /// Agrega una caja de contraseña a la UI.
        /// <para>Adds a password box to the UI.</para>
        /// </summary>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <returns>PasswordBox creado. <para>Created password box.</para></returns>
        public static PasswordBox AddPasswordBox(float x = 0, float y = 0, float width = 200)
            => YTBGumControls.AddPasswordBox(x, y, width);

        /// <summary>
        /// Agrega una casilla de verificación a la UI.
        /// <para>Adds a checkbox to the UI.</para>
        /// </summary>
        /// <param name="text">Texto de la casilla. <para>Checkbox text.</para></param>
        /// <param name="isChecked">Estado inicial. <para>Initial checked state.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="onCheckedChanged">Acción al cambiar. <para>Action when checked changes.</para></param>
        /// <returns>CheckBox creado. <para>Created checkbox.</para></returns>
        public static CheckBox AddCheckBox(string text, bool isChecked = false, float x = 0, float y = 0, Action<bool> onCheckedChanged = null)
            => YTBGumControls.AddCheckBox(text, isChecked, x, y, onCheckedChanged);

        /// <summary>
        /// Agrega un botón de radio a la UI.
        /// <para>Adds a radio button to the UI.</para>
        /// </summary>
        /// <param name="text">Texto del botón. <para>Radio button text.</para></param>
        /// <param name="groupName">Nombre del grupo. <para>Group name.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="onCheckedChanged">Acción al cambiar. <para>Action when checked changes.</para></param>
        /// <returns>RadioButton creado. <para>Created radio button.</para></returns>
        public static RadioButton AddRadioButton(string text, string groupName = "default", float x = 0, float y = 0, Action<bool> onCheckedChanged = null)
            => YTBGumControls.AddRadioButton(text, groupName, x, y, onCheckedChanged);

        /// <summary>
        /// Agrega un combo box (desplegable) a la UI.
        /// <para>Adds a combo box (dropdown) to the UI.</para>
        /// </summary>
        /// <param name="items">Items del combo. <para>Combo box items.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <param name="onSelectionChanged">Acción al cambiar selección. <para>Action when selection changes.</para></param>
        /// <returns>ComboBox creado. <para>Created combo box.</para></returns>
        public static ComboBox AddComboBox(string[] items = null, float x = 0, float y = 0, float width = 150, Action<int> onSelectionChanged = null)
            => YTBGumControls.AddComboBox(items, x, y, width, onSelectionChanged);

        /// <summary>
        /// Agrega un slider/deslizador a la UI.
        /// <para>Adds a slider to the UI.</para>
        /// </summary>
        /// <param name="min">Valor mínimo. <para>Minimum value.</para></param>
        /// <param name="max">Valor máximo. <para>Maximum value.</para></param>
        /// <param name="value">Valor inicial. <para>Initial value.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <param name="onValueChanged">Acción al cambiar el valor. <para>Action when value changes.</para></param>
        /// <returns>Slider creado. <para>Created slider.</para></returns>
        public static Slider AddSlider(double min = 0, double max = 100, double value = 50, float x = 0, float y = 0, float width = 200, Action<double> onValueChanged = null)
            => YTBGumControls.AddSlider(min, max, value, x, y, width, onValueChanged);

        /// <summary>
        /// Agrega una lista a la UI.
        /// <para>Adds a list box to the UI.</para>
        /// </summary>
        /// <param name="items">Items de la lista. <para>List items.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <param name="height">Alto del control. <para>Control height.</para></param>
        /// <param name="onSelectionChanged">Acción al cambiar selección. <para>Action when selection changes.</para></param>
        /// <returns>ListBox creado. <para>Created list box.</para></returns>
        public static ListBox AddListBox(string[] items = null, float x = 0, float y = 0, float width = 200, float height = 150, Action<int> onSelectionChanged = null)
            => YTBGumControls.AddListBox(items, x, y, width, height, onSelectionChanged);

        /// <summary>
        /// Agrega un scroll viewer a la UI.
        /// <para>Adds a scroll viewer to the UI.</para>
        /// </summary>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <param name="height">Alto del control. <para>Control height.</para></param>
        /// <returns>ScrollViewer creado. <para>Created scroll viewer.</para></returns>
        public static ScrollViewer AddScrollViewer(float x = 0, float y = 0, float width = 300, float height = 200)
            => YTBGumControls.AddScrollViewer(x, y, width, height);

        #endregion

        #region Quick Layout Creation

        /// <summary>
        /// Agrega un panel de apilado vertical a la UI.
        /// <para>Adds a vertical stack panel to the UI.</para>
        /// </summary>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <returns>StackPanel creado. <para>Created stack panel.</para></returns>
        public static StackPanel AddVerticalStack(float x = 0, float y = 0)
            => YTBGumLayouts.AddVerticalStack(x, y);

        /// <summary>
        /// Agrega un panel de apilado horizontal a la UI.
        /// <para>Adds a horizontal stack panel to the UI.</para>
        /// </summary>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <returns>StackPanel creado. <para>Created stack panel.</para></returns>
        public static StackPanel AddHorizontalStack(float x = 0, float y = 0)
            => YTBGumLayouts.AddHorizontalStack(x, y);

        /// <summary>
        /// Agrega un panel contenedor a la UI.
        /// <para>Adds a panel container to the UI.</para>
        /// </summary>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del panel. <para>Panel width.</para></param>
        /// <param name="height">Alto del panel. <para>Panel height.</para></param>
        /// <returns>Panel creado. <para>Created panel.</para></returns>
        public static Panel AddPanel(float x = 0, float y = 0, float width = 300, float height = 200)
            => YTBGumLayouts.AddPanel(x, y, width, height);

        #endregion

        #region Screen Properties

        /// <summary>
        /// Obtiene el ancho de la pantalla.
        /// <para>Gets the screen width.</para>
        /// </summary>
        public static float ScreenWidth => YTBGumService.ScreenWidth;

        /// <summary>
        /// Obtiene la altura de la pantalla.
        /// <para>Gets the screen height.</para>
        /// </summary>
        public static float ScreenHeight => YTBGumService.ScreenHeight;

        /// <summary>
        /// Obtiene la posición X central de la pantalla.
        /// <para>Gets the center X position of the screen.</para>
        /// </summary>
        public static float CenterX => ScreenWidth / 2;

        /// <summary>
        /// Obtiene la posición Y central de la pantalla.
        /// <para>Gets the center Y position of the screen.</para>
        /// </summary>
        public static float CenterY => ScreenHeight / 2;

        #endregion
    }
}
