using System;
using System.Collections.Generic;
using System.Linq;
using CefSharp;
using CefSharp.OffScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using YotsubaEngine.Core.YotsubaGame;

namespace YotsubaEngine.Web
{
    /// <summary>
    /// Gestor estático principal para web views dentro del motor. Administra instancias de navegadores
    /// off-screen CefSharp, sus texturas y la entrada del usuario.
    /// <para>Main static manager for web views within the engine. Manages CefSharp off-screen browser
    /// instances, their textures, and user input.</para>
    /// </summary>
    public static class YTBWebView
    {
        private static Dictionary<string, WebViewInstance> _instances = new Dictionary<string, WebViewInstance>();
        private static bool _cefInitialized;
        private static int _autoIdCounter;
        private static string _focusedInstanceId;
        private static MouseState _previousMouseState;
        private static KeyboardState _previousKbState;
        private static bool _textInputHooked;

        #region Lifecycle

        /// <summary>
        /// Inicializa el framework CEF. Solo se ejecuta una vez por aplicación.
        /// Debe llamarse antes de crear cualquier web view.
        /// <para>Initializes the CEF framework. Only runs once per application.
        /// Must be called before creating any web views.</para>
        /// </summary>
        public static void Initialize()
        {
            if (_cefInitialized) return;

            if (!YTBGlobalState.IsDesktop || YTBGlobalState.IsMacOS)
            {
                Console.WriteLine("[YTBWebView] Web views are only supported on Windows and Linux.");
                return;
            }

            if (!Cef.IsInitialized)
            {
                var settings = new CefSettings();
                Cef.Initialize(settings);
            }

            _cefInitialized = true;

            // Registrar el shutdown de CEF al salir del juego
            YTBGlobalState.Game.Exiting += OnGameExiting;
        }

        /// <summary>
        /// Cierra CEF y libera todos los recursos. Se llama automáticamente al salir del juego.
        /// <para>Shuts down CEF and releases all resources. Called automatically on game exit.</para>
        /// </summary>
        public static void Shutdown()
        {
            DestroyAll();

            if (_cefInitialized && Cef.IsInitialized)
            {
                Cef.Shutdown();
            }

            _cefInitialized = false;
        }

        private static void OnGameExiting(object sender, EventArgs e)
        {
            Shutdown();
        }

        #endregion

        #region Instance Management

        /// <summary>
        /// Crea una nueva instancia de web view con la URL, posición y tamaño especificados.
        /// <para>Creates a new web view instance with the specified URL, position, and size.</para>
        /// </summary>
        /// <param name="url">URL a cargar. <para>URL to load.</para></param>
        /// <param name="position">Posición en pantalla. <para>Screen position.</para></param>
        /// <param name="width">Ancho en píxeles. <para>Width in pixels.</para></param>
        /// <param name="height">Alto en píxeles. <para>Height in pixels.</para></param>
        /// <param name="id">Identificador opcional. Si es null, se genera automáticamente. <para>Optional identifier. If null, auto-generated.</para></param>
        /// <returns>Identificador de la instancia creada. <para>Created instance identifier.</para></returns>
        public static string Create(string url, Vector2 position, int width, int height, string id = null)
        {
            if (!_cefInitialized) return null;

            if (id == null)
            {
                id = $"webview_{_autoIdCounter++}";
            }

            if (_instances.ContainsKey(id))
            {
                Console.WriteLine($"[YTBWebView] Instance '{id}' already exists. Destroying old instance.");
                Destroy(id);
            }

            var instance = new WebViewInstance(id, YTBGlobalState.GraphicsDevice, url, width, height, position);
            _instances[id] = instance;

            // Hook TextInput si es la primera instancia
            HookTextInput();

            return id;
        }

        /// <summary>
        /// Destruye una instancia de web view y libera sus recursos.
        /// <para>Destroys a web view instance and releases its resources.</para>
        /// </summary>
        /// <param name="id">Identificador de la instancia. <para>Instance identifier.</para></param>
        public static void Destroy(string id)
        {
            if (_instances.TryGetValue(id, out var instance))
            {
                instance.Dispose();
                _instances.Remove(id);

                if (_focusedInstanceId == id)
                    _focusedInstanceId = null;
            }

            // Unhook TextInput si no quedan instancias
            if (_instances.Count == 0)
                UnhookTextInput();
        }

        /// <summary>
        /// Destruye todas las instancias de web view.
        /// <para>Destroys all web view instances.</para>
        /// </summary>
        public static void DestroyAll()
        {
            foreach (var instance in _instances.Values)
            {
                instance.Dispose();
            }
            _instances.Clear();
            _focusedInstanceId = null;

            UnhookTextInput();
        }

        /// <summary>
        /// Verifica si existe una instancia con el identificador especificado.
        /// <para>Checks if an instance with the specified identifier exists.</para>
        /// </summary>
        /// <param name="id">Identificador a verificar. <para>Identifier to check.</para></param>
        /// <returns>True si la instancia existe. <para>True if the instance exists.</para></returns>
        public static bool Exists(string id)
        {
            return _instances.ContainsKey(id);
        }

        #endregion

        #region Navigation

        /// <summary>
        /// Navega una instancia de web view a una nueva URL.
        /// <para>Navigates a web view instance to a new URL.</para>
        /// </summary>
        /// <param name="id">Identificador de la instancia. <para>Instance identifier.</para></param>
        /// <param name="url">URL destino. <para>Target URL.</para></param>
        public static void Navigate(string id, string url)
        {
            if (_instances.TryGetValue(id, out var instance))
            {
                instance.Navigate(url);
            }
        }

        /// <summary>
        /// Obtiene la URL actual de una instancia de web view.
        /// <para>Gets the current URL of a web view instance.</para>
        /// </summary>
        /// <param name="id">Identificador de la instancia. <para>Instance identifier.</para></param>
        /// <returns>URL actual o null si la instancia no existe. <para>Current URL or null if the instance doesn't exist.</para></returns>
        public static string GetCurrentUrl(string id)
        {
            return _instances.TryGetValue(id, out var instance) ? instance.CurrentUrl : null;
        }

        /// <summary>
        /// Ejecuta código JavaScript en una instancia de web view.
        /// <para>Executes JavaScript code in a web view instance.</para>
        /// </summary>
        /// <param name="id">Identificador de la instancia. <para>Instance identifier.</para></param>
        /// <param name="script">Código JavaScript. <para>JavaScript code.</para></param>
        public static void ExecuteScript(string id, string script)
        {
            if (_instances.TryGetValue(id, out var instance))
            {
                instance.ExecuteScript(script);
            }
        }

        #endregion

        #region Visibility & Focus

        /// <summary>
        /// Establece la visibilidad de una instancia de web view.
        /// <para>Sets the visibility of a web view instance.</para>
        /// </summary>
        /// <param name="id">Identificador de la instancia. <para>Instance identifier.</para></param>
        /// <param name="visible">True para mostrar, false para ocultar. <para>True to show, false to hide.</para></param>
        public static void SetVisible(string id, bool visible)
        {
            if (_instances.TryGetValue(id, out var instance))
            {
                instance.IsVisible = visible;
            }
        }

        /// <summary>
        /// Verifica si una instancia de web view es visible.
        /// <para>Checks if a web view instance is visible.</para>
        /// </summary>
        /// <param name="id">Identificador de la instancia. <para>Instance identifier.</para></param>
        /// <returns>True si es visible. <para>True if visible.</para></returns>
        public static bool IsVisible(string id)
        {
            return _instances.TryGetValue(id, out var instance) && instance.IsVisible;
        }

        /// <summary>
        /// Establece una instancia como la que recibe el foco de entrada.
        /// <para>Sets an instance as the one receiving input focus.</para>
        /// </summary>
        /// <param name="id">Identificador de la instancia. <para>Instance identifier.</para></param>
        public static void SetFocused(string id)
        {
            if (_instances.ContainsKey(id))
            {
                _focusedInstanceId = id;
            }
        }

        /// <summary>
        /// Habilita o deshabilita la entrada de usuario para una instancia.
        /// <para>Enables or disables user input for an instance.</para>
        /// </summary>
        /// <param name="id">Identificador de la instancia. <para>Instance identifier.</para></param>
        /// <param name="enabled">True para habilitar, false para deshabilitar. <para>True to enable, false to disable.</para></param>
        public static void SetInputEnabled(string id, bool enabled)
        {
            if (_instances.TryGetValue(id, out var instance))
            {
                instance.IsInputEnabled = enabled;
            }
        }

        #endregion

        #region Position & Size

        /// <summary>
        /// Establece la posición en pantalla de una instancia de web view.
        /// <para>Sets the screen position of a web view instance.</para>
        /// </summary>
        /// <param name="id">Identificador de la instancia. <para>Instance identifier.</para></param>
        /// <param name="position">Nueva posición. <para>New position.</para></param>
        public static void SetPosition(string id, Vector2 position)
        {
            if (_instances.TryGetValue(id, out var instance))
            {
                instance.Position = position;
            }
        }

        /// <summary>
        /// Redimensiona una instancia de web view. Recrea la textura y el buffer internos.
        /// <para>Resizes a web view instance. Recreates the internal texture and buffer.</para>
        /// </summary>
        /// <param name="id">Identificador de la instancia. <para>Instance identifier.</para></param>
        /// <param name="width">Nuevo ancho. <para>New width.</para></param>
        /// <param name="height">Nuevo alto. <para>New height.</para></param>
        public static void SetSize(string id, int width, int height)
        {
            if (_instances.TryGetValue(id, out var instance))
            {
                instance.Resize(YTBGlobalState.GraphicsDevice, width, height);
            }
        }

        /// <summary>
        /// Obtiene la textura actual de una instancia para dibujo manual.
        /// <para>Gets the current texture of an instance for manual drawing.</para>
        /// </summary>
        /// <param name="id">Identificador de la instancia. <para>Instance identifier.</para></param>
        /// <returns>Textura del web view o null. <para>Web view texture or null.</para></returns>
        public static Texture2D GetTexture(string id)
        {
            return _instances.TryGetValue(id, out var instance) ? instance.Texture : null;
        }

        #endregion

        #region Internal Update & Render

        /// <summary>
        /// Actualiza todas las texturas de las instancias y procesa la entrada del usuario.
        /// Debe llamarse cada frame desde el sistema del motor.
        /// <para>Updates all instance textures and processes user input.
        /// Must be called every frame from the engine system.</para>
        /// </summary>
        internal static void UpdateAll()
        {
            if (!_cefInitialized || _instances.Count == 0) return;

            var currentMouse = Mouse.GetState();
            var currentKb = Keyboard.GetState();

            // Actualizar texturas
            foreach (var instance in _instances.Values)
            {
                instance.UpdateTexture();
            }

            // Procesar entrada de mouse - encontrar el web view bajo el cursor
            string hoveredId = null;
            foreach (var kvp in _instances.Reverse())
            {
                if (kvp.Value.IsVisible && kvp.Value.IsInputEnabled && kvp.Value.HitTest(currentMouse.X, currentMouse.Y))
                {
                    hoveredId = kvp.Key;
                    kvp.Value.HandleMouse(currentMouse, _previousMouseState);

                    // Auto-focus al hacer clic
                    if (currentMouse.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                    {
                        _focusedInstanceId = kvp.Key;
                    }
                    break;
                }
            }

            // Procesar entrada de teclado - solo al web view con foco
            if (_focusedInstanceId != null && _instances.TryGetValue(_focusedInstanceId, out var focused))
            {
                focused.HandleKeyboard(currentKb, _previousKbState);
            }

            _previousMouseState = currentMouse;
            _previousKbState = currentKb;
        }

        /// <summary>
        /// Dibuja todas las instancias visibles de web view.
        /// Debe llamarse dentro de un bloque SpriteBatch.Begin/End.
        /// <para>Draws all visible web view instances.
        /// Must be called within a SpriteBatch.Begin/End block.</para>
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch para dibujar. <para>SpriteBatch to draw with.</para></param>
        internal static void DrawAll(SpriteBatch spriteBatch)
        {
            if (!_cefInitialized || _instances.Count == 0) return;

            foreach (var instance in _instances.Values)
            {
                instance.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Reenvía un evento de entrada de texto al web view con foco.
        /// <para>Forwards a text input event to the focused web view.</para>
        /// </summary>
        /// <param name="e">Evento de entrada de texto. <para>Text input event.</para></param>
        internal static void HandleTextInput(object sender, TextInputEventArgs e)
        {
            if (_focusedInstanceId != null && _instances.TryGetValue(_focusedInstanceId, out var focused))
            {
                focused.HandleTextInput(e.Character);
            }
        }

        #endregion

        #region TextInput Hook Management

        private static void HookTextInput()
        {
            if (_textInputHooked) return;

            var game = YTBGlobalState.Game;
            if (game?.Window != null)
            {
                game.Window.TextInput += HandleTextInput;
                _textInputHooked = true;
            }
        }

        private static void UnhookTextInput()
        {
            if (!_textInputHooked) return;

            var game = YTBGlobalState.Game;
            if (game?.Window != null)
            {
                game.Window.TextInput -= HandleTextInput;
                _textInputHooked = false;
            }
        }

        #endregion
    }
}
