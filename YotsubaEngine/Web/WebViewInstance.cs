using System;
using System.Runtime.InteropServices;
using CefSharp;
using CefSharp.OffScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace YotsubaEngine.Web
{
    /// <summary>
    /// Encapsula una instancia individual de navegador web off-screen usando CefSharp.
    /// Mantiene el browser, la textura de MonoGame y el buffer de píxeles con acceso thread-safe.
    /// <para>Encapsulates a single off-screen web browser instance using CefSharp.
    /// Maintains the browser, MonoGame texture, and pixel buffer with thread-safe access.</para>
    /// </summary>
    internal class WebViewInstance : IDisposable
    {
        private ChromiumWebBrowser _browser;
        private Texture2D _webTexture;
        private byte[] _pixelBuffer;
        private readonly object _bufferLock = new object();
        private bool _needsTextureUpdate;
        private int _width;
        private int _height;
        private Vector2 _position;

        private MouseState _lastMouseState;
        private KeyboardState _lastKeyboardState;

        /// <summary>
        /// Identificador único de esta instancia de web view.
        /// <para>Unique identifier for this web view instance.</para>
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Textura de MonoGame que contiene el contenido renderizado del navegador.
        /// <para>MonoGame texture containing the rendered browser content.</para>
        /// </summary>
        public Texture2D Texture => _webTexture;

        /// <summary>
        /// Posición en pantalla donde se dibuja el web view.
        /// <para>Screen position where the web view is drawn.</para>
        /// </summary>
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// Ancho del navegador y la textura en píxeles.
        /// <para>Width of the browser and texture in pixels.</para>
        /// </summary>
        public int Width => _width;

        /// <summary>
        /// Alto del navegador y la textura en píxeles.
        /// <para>Height of the browser and texture in pixels.</para>
        /// </summary>
        public int Height => _height;

        /// <summary>
        /// Indica si el web view es visible y debe dibujarse.
        /// <para>Indicates whether the web view is visible and should be drawn.</para>
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Indica si el web view acepta entrada del usuario (mouse y teclado).
        /// <para>Indicates whether the web view accepts user input (mouse and keyboard).</para>
        /// </summary>
        public bool IsInputEnabled { get; set; } = true;

        /// <summary>
        /// Rectángulo de límites para hit testing.
        /// <para>Bounding rectangle for hit testing.</para>
        /// </summary>
        public Rectangle Bounds => new Rectangle((int)_position.X, (int)_position.Y, _width, _height);

        /// <summary>
        /// URL actual del navegador.
        /// <para>Current browser URL.</para>
        /// </summary>
        public string CurrentUrl => _browser?.Address;

        /// <summary>
        /// Crea una nueva instancia de web view con el navegador off-screen de CefSharp.
        /// <para>Creates a new web view instance with CefSharp's off-screen browser.</para>
        /// </summary>
        /// <param name="id">Identificador único. <para>Unique identifier.</para></param>
        /// <param name="graphicsDevice">Dispositivo gráfico de MonoGame. <para>MonoGame graphics device.</para></param>
        /// <param name="url">URL inicial a cargar. <para>Initial URL to load.</para></param>
        /// <param name="width">Ancho en píxeles. <para>Width in pixels.</para></param>
        /// <param name="height">Alto en píxeles. <para>Height in pixels.</para></param>
        /// <param name="position">Posición en pantalla. <para>Screen position.</para></param>
        public WebViewInstance(string id, GraphicsDevice graphicsDevice, string url, int width, int height, Vector2 position)
        {
            Id = id;
            _width = width;
            _height = height;
            _position = position;

            _webTexture = new Texture2D(graphicsDevice, width, height, false, SurfaceFormat.Bgra32);
            _pixelBuffer = new byte[width * height * 4];

            _browser = new ChromiumWebBrowser(url);
            _browser.Size = new System.Drawing.Size(width, height);
            _browser.Paint += OnBrowserPaint;
        }

        /// <summary>
        /// Callback del evento Paint de CefSharp. Copia los píxeles BGRA al buffer de forma thread-safe.
        /// <para>CefSharp Paint event callback. Copies BGRA pixels to the buffer in a thread-safe manner.</para>
        /// </summary>
        private void OnBrowserPaint(object sender, OnPaintEventArgs e)
        {
            lock (_bufferLock)
            {
                Marshal.Copy(e.BufferHandle, _pixelBuffer, 0, _pixelBuffer.Length);
                _needsTextureUpdate = true;
            }
        }

        /// <summary>
        /// Actualiza la textura si hay nuevos píxeles disponibles. Debe llamarse desde el hilo principal del juego.
        /// <para>Updates the texture if new pixels are available. Must be called from the main game thread.</para>
        /// </summary>
        public void UpdateTexture()
        {
            if (!_needsTextureUpdate) return;

            lock (_bufferLock)
            {
                _webTexture.SetData(_pixelBuffer);
                _needsTextureUpdate = false;
            }
        }

        /// <summary>
        /// Envía eventos de mouse al navegador, ajustando las coordenadas por la posición del web view.
        /// <para>Sends mouse events to the browser, adjusting coordinates by the web view position.</para>
        /// </summary>
        /// <param name="currentMouse">Estado actual del mouse. <para>Current mouse state.</para></param>
        /// <param name="previousMouse">Estado previo del mouse. <para>Previous mouse state.</para></param>
        public void HandleMouse(MouseState currentMouse, MouseState previousMouse)
        {
            if (_browser == null || !_browser.IsBrowserInitialized) return;

            int localX = currentMouse.X - (int)_position.X;
            int localY = currentMouse.Y - (int)_position.Y;

            var host = _browser.GetBrowserHost();

            // Movimiento del ratón
            if (currentMouse.X != previousMouse.X || currentMouse.Y != previousMouse.Y)
            {
                host.SendMouseMoveEvent(localX, localY, false, CefEventFlags.None);
            }

            // Clic izquierdo
            if (currentMouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released)
            {
                host.SendMouseClickEvent(localX, localY, MouseButtonType.Left, false, 1, CefEventFlags.None);
            }
            else if (currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed)
            {
                host.SendMouseClickEvent(localX, localY, MouseButtonType.Left, true, 1, CefEventFlags.None);
            }

            // Clic derecho
            if (currentMouse.RightButton == ButtonState.Pressed && previousMouse.RightButton == ButtonState.Released)
            {
                host.SendMouseClickEvent(localX, localY, MouseButtonType.Right, false, 1, CefEventFlags.None);
            }
            else if (currentMouse.RightButton == ButtonState.Released && previousMouse.RightButton == ButtonState.Pressed)
            {
                host.SendMouseClickEvent(localX, localY, MouseButtonType.Right, true, 1, CefEventFlags.None);
            }

            // Clic medio
            if (currentMouse.MiddleButton == ButtonState.Pressed && previousMouse.MiddleButton == ButtonState.Released)
            {
                host.SendMouseClickEvent(localX, localY, MouseButtonType.Middle, false, 1, CefEventFlags.None);
            }
            else if (currentMouse.MiddleButton == ButtonState.Released && previousMouse.MiddleButton == ButtonState.Pressed)
            {
                host.SendMouseClickEvent(localX, localY, MouseButtonType.Middle, true, 1, CefEventFlags.None);
            }

            // Rueda del ratón
            int scrollDelta = currentMouse.ScrollWheelValue - previousMouse.ScrollWheelValue;
            if (scrollDelta != 0)
            {
                host.SendMouseWheelEvent(localX, localY, 0, scrollDelta, CefEventFlags.None);
            }
        }

        /// <summary>
        /// Envía un evento de carácter al navegador (para escritura de texto).
        /// <para>Sends a character event to the browser (for text input).</para>
        /// </summary>
        /// <param name="character">Carácter ingresado. <para>Input character.</para></param>
        public void HandleTextInput(char character)
        {
            if (_browser == null || !_browser.IsBrowserInitialized) return;
            if (char.IsControl(character)) return;

            var keyEvent = new KeyEvent
            {
                Type = KeyEventType.Char,
                WindowsKeyCode = character,
                Character = character,
                UnmodifiedCharacter = character
            };

            _browser.GetBrowserHost().SendKeyEvent(keyEvent);
        }

        /// <summary>
        /// Envía eventos de teclas de control al navegador (Backspace, Enter, Tab, flechas).
        /// <para>Sends control key events to the browser (Backspace, Enter, Tab, arrows).</para>
        /// </summary>
        /// <param name="currentKb">Estado actual del teclado. <para>Current keyboard state.</para></param>
        /// <param name="previousKb">Estado previo del teclado. <para>Previous keyboard state.</para></param>
        public void HandleKeyboard(KeyboardState currentKb, KeyboardState previousKb)
        {
            if (_browser == null || !_browser.IsBrowserInitialized) return;

            var host = _browser.GetBrowserHost();

            SendControlKey(host, currentKb, previousKb, Keys.Back, 0x08);
            SendControlKey(host, currentKb, previousKb, Keys.Enter, 0x0D);
            SendControlKey(host, currentKb, previousKb, Keys.Tab, 0x09);
            SendControlKey(host, currentKb, previousKb, Keys.Delete, 0x2E);
            SendControlKey(host, currentKb, previousKb, Keys.Left, 0x25);
            SendControlKey(host, currentKb, previousKb, Keys.Up, 0x26);
            SendControlKey(host, currentKb, previousKb, Keys.Right, 0x27);
            SendControlKey(host, currentKb, previousKb, Keys.Down, 0x28);
            SendControlKey(host, currentKb, previousKb, Keys.Home, 0x24);
            SendControlKey(host, currentKb, previousKb, Keys.End, 0x23);
            SendControlKey(host, currentKb, previousKb, Keys.PageUp, 0x21);
            SendControlKey(host, currentKb, previousKb, Keys.PageDown, 0x22);
            SendControlKey(host, currentKb, previousKb, Keys.Escape, 0x1B);
        }

        private static void SendControlKey(IBrowserHost host, KeyboardState current, KeyboardState previous, Keys key, int windowsKeyCode)
        {
            if (current.IsKeyDown(key) && previous.IsKeyUp(key))
            {
                host.SendKeyEvent(new KeyEvent { Type = KeyEventType.KeyDown, WindowsKeyCode = windowsKeyCode });
                host.SendKeyEvent(new KeyEvent { Type = KeyEventType.KeyUp, WindowsKeyCode = windowsKeyCode });
            }
        }

        /// <summary>
        /// Navega el navegador a una nueva URL.
        /// <para>Navigates the browser to a new URL.</para>
        /// </summary>
        /// <param name="url">URL destino. <para>Target URL.</para></param>
        public void Navigate(string url)
        {
            _browser?.LoadUrl(url);
        }

        /// <summary>
        /// Ejecuta código JavaScript en el navegador.
        /// <para>Executes JavaScript code in the browser.</para>
        /// </summary>
        /// <param name="script">Código JavaScript. <para>JavaScript code.</para></param>
        public void ExecuteScript(string script)
        {
            if (_browser != null && _browser.IsBrowserInitialized)
            {
                _browser.EvaluateScriptAsync(script);
            }
        }

        /// <summary>
        /// Dibuja la textura del web view en la posición especificada.
        /// <para>Draws the web view texture at the specified position.</para>
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch para dibujar. <para>SpriteBatch to draw with.</para></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible || _webTexture == null) return;
            spriteBatch.Draw(_webTexture, _position, Color.White);
        }

        /// <summary>
        /// Verifica si un punto de pantalla está dentro de los límites de este web view.
        /// <para>Checks if a screen point is within the bounds of this web view.</para>
        /// </summary>
        /// <param name="screenX">Coordenada X en pantalla. <para>Screen X coordinate.</para></param>
        /// <param name="screenY">Coordenada Y en pantalla. <para>Screen Y coordinate.</para></param>
        /// <returns>True si el punto está dentro del web view. <para>True if the point is within the web view.</para></returns>
        public bool HitTest(int screenX, int screenY)
        {
            return Bounds.Contains(screenX, screenY);
        }

        /// <summary>
        /// Redimensiona el navegador y recrea la textura.
        /// <para>Resizes the browser and recreates the texture.</para>
        /// </summary>
        /// <param name="graphicsDevice">Dispositivo gráfico. <para>Graphics device.</para></param>
        /// <param name="width">Nuevo ancho. <para>New width.</para></param>
        /// <param name="height">Nuevo alto. <para>New height.</para></param>
        public void Resize(GraphicsDevice graphicsDevice, int width, int height)
        {
            _width = width;
            _height = height;

            _webTexture?.Dispose();
            _webTexture = new Texture2D(graphicsDevice, width, height, false, SurfaceFormat.Bgra32);

            lock (_bufferLock)
            {
                _pixelBuffer = new byte[width * height * 4];
                _needsTextureUpdate = false;
            }

            if (_browser != null)
            {
                _browser.Size = new System.Drawing.Size(width, height);
            }
        }

        /// <summary>
        /// Libera los recursos del navegador y la textura.
        /// <para>Releases browser and texture resources.</para>
        /// </summary>
        public void Dispose()
        {
            if (_browser != null)
            {
                _browser.Paint -= OnBrowserPaint;
                _browser.Dispose();
                _browser = null;
            }
            _webTexture?.Dispose();
            _webTexture = null;
        }
    }
}
