using Microsoft.Xna.Framework;
using Gum.Wireframe;
using Gum.Forms;
using MonoGameGum;
using System;

namespace YotsubaEngine.Core.System.GumUI
{
    /// <summary>
    /// Main service for GumUI integration in YotsubaEngine.
    /// Servicio principal para la integración de GumUI en YotsubaEngine.
    /// </summary>
    /// <remarks>
    /// This class provides a simplified API to initialize and manage GumUI in your game.
    /// Esta clase proporciona una API simplificada para inicializar y gestionar GumUI en tu juego.
    /// </remarks>
    public static class YTBGumService
    {
        private static bool _isInitialized;
        private static Game _game;

        /// <summary>
        /// Gets whether GumUI has been initialized.
        /// Obtiene si GumUI ha sido inicializado.
        /// </summary>
        public static bool IsInitialized => _isInitialized;

        /// <summary>
        /// Gets the root container for all Gum UI elements.
        /// Obtiene el contenedor raíz para todos los elementos de UI de Gum.
        /// </summary>
        public static GraphicalUiElement Root => GumService.Default.Root;

        /// <summary>
        /// Initializes GumUI with default visuals V3.
        /// Inicializa GumUI con los visuales predeterminados V3.
        /// </summary>
        /// <param name="game">The MonoGame Game instance. / La instancia del juego MonoGame.</param>
        public static void Initialize(Game game)
        {
            Initialize(game, DefaultVisualsVersion.V3);
        }

        /// <summary>
        /// Initializes GumUI with a specific visual style version.
        /// Inicializa GumUI con una versión específica de estilo visual.
        /// </summary>
        /// <param name="game">The MonoGame Game instance. / La instancia del juego MonoGame.</param>
        /// <param name="version">The visual style version to use. / La versión de estilo visual a usar.</param>
        public static void Initialize(Game game, DefaultVisualsVersion version)
        {
            if (_isInitialized)
            {
                return;
            }

            _game = game ?? throw new ArgumentNullException(nameof(game));


            GumService.Default.Initialize(game, version);
 
            _isInitialized = true;
        }

        /// <summary>
        /// Updates GumUI. Call this in your game's Update method.
        /// Actualiza GumUI. Llama esto en el método Update de tu juego.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values. / Proporciona una instantánea de valores de tiempo.</param>
        public static void Update(GameTime gameTime)
        {
            if (!_isInitialized) return;
            GumService.Default.Update(gameTime);
        }

        /// <summary>
        /// Draws all GumUI elements. Call this in your game's Draw method.
        /// Dibuja todos los elementos de GumUI. Llama esto en el método Draw de tu juego.
        /// </summary>
        public static void Draw()
        {
            if (!_isInitialized) return;
            GumService.Default.Draw();
        }

        /// <summary>
        /// Clears all UI elements from the root container.
        /// Limpia todos los elementos de UI del contenedor raíz.
        /// </summary>
        public static void ClearAll()
        {
            if (!_isInitialized) return;
            Root?.Children?.Clear();
        }

        /// <summary>
        /// Gets the width of the screen/window.
        /// Obtiene el ancho de la pantalla/ventana.
        /// </summary>
        public static float ScreenWidth => _game?.GraphicsDevice?.Viewport.Width ?? 0;

        /// <summary>
        /// Gets the height of the screen/window.
        /// Obtiene la altura de la pantalla/ventana.
        /// </summary>
        public static float ScreenHeight => _game?.GraphicsDevice?.Viewport.Height ?? 0;
    }
}
