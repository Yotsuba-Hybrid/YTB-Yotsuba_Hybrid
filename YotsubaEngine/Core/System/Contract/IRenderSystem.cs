using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.Core.System.Contract
{
    /// <summary>
    /// Define una interfaz común para los sistemas de renderizado del motor.
    /// <para>Defines a common interface for engine rendering systems.</para>
    /// </summary>
    public interface IRenderSystem : ISystem
    {

        /// <summary>
        /// Ejecuta el render del frame actual.
        /// <para>Renders the current frame.</para>
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch utilizado para dibujar. <para>Sprite batch.</para></param>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        void Render2D(SpriteBatch spriteBatch, GameTime gameTime);

        /// <summary>
        /// Ejecuta en el frame de renderizado. Se utiliza para dibujar modelos 3D o elementos con configuracion 3D.
        /// </summary>
        /// <param name="gameTime"></param>
        void Render3D(GameTime gameTime);
    }
}

