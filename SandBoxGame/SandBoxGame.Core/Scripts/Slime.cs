using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Attributes;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Core.YotsubaGame.Scripting;
using YotsubaEngine.Events.YTBEvents;

namespace SandBoxGame.Core.Scripts
{
    [Script]
    /// <summary>
    /// Script del slime controlado por físicas y movimiento WASD.
    /// <para>Slime script controlled by physics and WASD movement.</para>
    /// </summary>
    public class Slime : BaseScript, ICollisionListener
    {

        /// <summary>
        /// Ancho actual de la pantalla en píxeles.
        /// <para>Current screen width in pixels.</para>
        /// </summary>
        public float ScreenWidth = YTBGlobalState.GraphicsDevice.Viewport.Width;
        /// <summary>
        /// Alto actual de la pantalla en píxeles.
        /// <para>Current screen height in pixels.</para>
        /// </summary>
        public float ScreenHeight = YTBGlobalState.GraphicsDevice.Viewport.Height;


        /// <summary>
        /// Inicializa el script y configura la física del slime.
        /// <para>Initializes the script and configures slime physics.</para>
        /// </summary>
        public override void Initialize()
        {

            base.Initialize();
           
            // Configure physics
            ref var rigidBody = ref GetRigidBody2D(Entity);
            rigidBody.GameType = GameType.TopDown; // or TopDown
            rigidBody.JumpForce = -14.0f;
            rigidBody.SPEED = 4f;
            rigidBody.TOP_SPEED = 8f;
            rigidBody.Gravity = 0.6f;

            // Register - movement, physics, animations now automatic
            YTBGlobalState.YTB_WASD_Movement.AddEntity(Entity);
        }

        /// <summary>
        /// Actualiza el script cada cuadro.
        /// <para>Updates the script each frame.</para>
        /// </summary>
        /// <param name="gametime">
        /// Tiempo de juego actual.
        /// <para>Current game time.</para>
        /// </param>
        public override void Update(GameTime gametime)
        {
            base.Update(gametime);
        }
        /// <summary>
        /// Dibuja el slime en 2D durante el renderizado.
        /// <para>Draws the slime in 2D during rendering.</para>
        /// </summary>
        /// <param name="spriteBatch">
        /// SpriteBatch usado para dibujar.
        /// <para>SpriteBatch used for drawing.</para>
        /// </param>
        /// <param name="gameTime">
        /// Tiempo de juego actual.
        /// <para>Current game time.</para>
        /// </param>
        public override void Draw2D(SpriteBatch spriteBatch, GameTime gameTime)
        { 
            base.Draw2D(spriteBatch, gameTime);
        }

        /// <summary>
        /// Maneja las colisiones del slime.
        /// <para>Handles slime collisions.</para>
        /// </summary>
        /// <param name="@event">
        /// Evento de colisión recibido.
        /// <para>Collision event received.</para>
        /// </param>
        public void OnCollision(OnCollitionEvent @event)
        {
            
        } 
    }
}
