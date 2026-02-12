using Microsoft.Xna.Framework;
using Gum.Wireframe;
using Gum.Forms;
using MonoGameGum;
using System;

namespace YotsubaEngine.Core.System.GumUI
{
    /// <summary>
    /// Servicio principal para la integración de GumUI en YotsubaEngine.
    /// <para>Main service for GumUI integration in YotsubaEngine.</para>
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
        /// Obtiene si GumUI ha sido inicializado.
        /// <para>Gets whether GumUI has been initialized.</para>
        /// </summary>
        public static bool IsInitialized => _isInitialized;

        /// <summary>
        /// Obtiene el contenedor raíz para todos los elementos de UI de Gum.
        /// <para>Gets the root container for all Gum UI elements.</para>
        /// </summary>
        public static GraphicalUiElement Root => GumService.Default.Root;

        /// <summary>
        /// Inicializa GumUI con los visuales predeterminados V3.
        /// <para>Initializes GumUI with default visuals V3.</para>
        /// </summary>
        /// <param name="game">Instancia del juego MonoGame. <para>The MonoGame Game instance.</para></param>
        public static void Initialize(Game game)
        {
            Initialize(game, DefaultVisualsVersion.V3);
        }

        /// <summary>
        /// Inicializa GumUI con una versión específica de estilo visual.
        /// <para>Initializes GumUI with a specific visual style version.</para>
        /// </summary>
        /// <param name="game">Instancia del juego MonoGame. <para>The MonoGame Game instance.</para></param>
        /// <param name="version">Versión de estilo visual a usar. <para>The visual style version to use.</para></param>
        public static void Initialize(Game game, DefaultVisualsVersion version)
        {
            if (_isInitialized)
            {
                return;
            }

            _game = game ?? throw new ArgumentNullException(nameof(game));


            try
            {
                GumService.Default.Initialize(game, version);
            }
            catch (Microsoft.Xna.Framework.Content.ContentLoadException)
            {
            }
 
            _isInitialized = true;
        }

        /// <summary>
        /// Actualiza GumUI. Llama esto en el método Update de tu juego.
        /// <para>Updates GumUI. Call this in your game's Update method.</para>
        /// </summary>
        /// <param name="gameTime">Proporciona una instantánea de valores de tiempo. <para>Provides a snapshot of timing values.</para></param>
        public static void Update(GameTime gameTime)
        {
            if (!_isInitialized) return;
            GumService.Default.Update(gameTime);
        }

        /// <summary>
        /// Dibuja todos los elementos de GumUI. Llama esto en el método Draw de tu juego.
        /// <para>Draws all GumUI elements. Call this in your game's Draw method.</para>
        /// </summary>
        public static void Draw()
        {
            if (!_isInitialized) return;
            GumService.Default.Draw();
        }

        /// <summary>
        /// Limpia todos los elementos de UI del contenedor raíz.
        /// <para>Clears all UI elements from the root container.</para>
        /// </summary>
        public static void ClearAll()
        {
            if (!_isInitialized) return;
            Root?.Children?.Clear();
        }

        /// <summary>
        /// Obtiene el ancho de la pantalla/ventana.
        /// <para>Gets the width of the screen/window.</para>
        /// </summary>
        public static float ScreenWidth => _game?.GraphicsDevice?.Viewport.Width ?? 0;

        /// <summary>
        /// Obtiene la altura de la pantalla/ventana.
        /// <para>Gets the height of the screen/window.</para>
        /// </summary>
        public static float ScreenHeight => _game?.GraphicsDevice?.Viewport.Height ?? 0;
    }
}
