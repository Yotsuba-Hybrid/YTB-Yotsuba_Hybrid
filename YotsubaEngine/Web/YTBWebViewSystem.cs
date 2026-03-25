using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.YotsubaGame;

namespace YotsubaEngine.Web
{
    /// <summary>
    /// Sistema del motor que integra los web views en el ciclo de Update/Draw del juego.
    /// Registra este sistema usando <c>SystemBuilder.AddSystem&lt;YTBWebViewSystem&gt;()</c>.
    /// <para>Engine system that integrates web views into the game's Update/Draw cycle.
    /// Register this system using <c>SystemBuilder.AddSystem&lt;YTBWebViewSystem&gt;()</c>.</para>
    /// </summary>
    public class YTBWebViewSystem : IRenderSystem
    {
        /// <summary>
        /// Inicializa el sistema y el framework CEF.
        /// <para>Initializes the system and the CEF framework.</para>
        /// </summary>
        /// <param name="entities">Administrador de entidades. <para>Entity manager.</para></param>
        public override void InitializeSystem(EntityManager entities)
        {
            EntityManager = entities;
            YTBWebView.Initialize();
        }

        /// <summary>
        /// Actualiza las texturas de los web views y procesa la entrada del usuario cada frame.
        /// <para>Updates web view textures and processes user input each frame.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public override void UpdateSystem(GameTime gameTime)
        {
            YTBWebView.UpdateAll();
        }

        /// <summary>
        /// Dibuja todas las instancias visibles de web view.
        /// <para>Draws all visible web view instances.</para>
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch para dibujar. <para>SpriteBatch to draw with.</para></param>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public override void Render2D(SpriteBatch spriteBatch, GameTime gameTime)
        {
            YTBWebView.DrawAll(spriteBatch);
        }

        /// <summary>
        /// Destruye todas las instancias de web view al descargar el sistema.
        /// <para>Destroys all web view instances when the system is unloaded.</para>
        /// </summary>
        public override void Dispose()
        {
            YTBWebView.DestroyAll();
        }
    }
}
