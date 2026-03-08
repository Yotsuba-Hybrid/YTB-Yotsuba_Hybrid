using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Graphics;

namespace YotsubaEngine.Core.System.Contract
{
    /// <summary>
    /// Define una clase abstracta común para los sistemas de renderizado del motor.
    /// <para>Defines a common abstract class for engine rendering systems.</para>
    /// </summary>
    public abstract class IRenderSystem : ISystem
    {

        /// <summary>
        /// Inicializa una nueva instancia del sistema de renderizado y crea el contexto de gráficos 3D.
        /// <para>Initializes a new instance of the render system and creates the 3D graphics context.</para>
        /// </summary>
        protected IRenderSystem()
        {
            _graphics3D = new Graphics3D();
        }

        /// <summary>
        /// Propiedad para acceder a las funcionalidades de renderizado 3D nativo del motor.
        /// Con esta propiedad, se puede renderizar cajas, rectángulos, figuras geométricas y sprites 2D en un entorno 3D.
        /// <para>Property to access native 3D rendering features of the engine.
        /// With this property, boxes, rectangles, geometric shapes, and 2D sprites can be rendered in a 3D environment.</para>
        /// </summary>
        public Graphics3D _graphics3D { get; private set; }

        /// <summary>
        /// Ejecuta el render del frame actual.
        /// <para>Renders the current frame.</para>
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch utilizado para dibujar. <para>Sprite batch.</para></param>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public virtual void Render2D(SpriteBatch spriteBatch, GameTime gameTime) { }

        /// <summary>
        /// Ejecuta el renderizado 3D del frame actual. Se utiliza para dibujar modelos 3D o elementos con configuración 3D.
        /// <para>Executes the 3D rendering of the current frame. Used to draw 3D models or elements with 3D configuration.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public virtual void Render3D(GameTime gameTime) { }
    }
}

