using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using YotsubaEngine.Attributes;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.YotsubaGame.Scripting;
using YotsubaEngine.Events.YTBEvents;

namespace SandBoxGame.Core.Scripts
{
    [Script]
    /// <summary>
    /// Script del murciélago que rebota en los límites y recolecta slimes.
    /// <para>Bat script that bounces within bounds and collects slimes.</para>
    /// </summary>
    public class Bat : BaseScript, ICollisionListener
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
        /// Límite máximo en X del mundo para el rebote.
        /// <para>Maximum world X bound used for bouncing.</para>
        /// </summary>
        public static float MaxWorldXPoint = (int)(YTBGlobalState.GraphicsDevice.Viewport.Width * 0.5f) - 180;
        /// <summary>
        /// Límite máximo en Y del mundo para el rebote.
        /// <para>Maximum world Y bound used for bouncing.</para>
        /// </summary>
        public static float MaxWorldYPoint = (int)(YTBGlobalState.GraphicsDevice.Viewport.Height * 0.5f) - 180;

        const int MAX_VEL_X = 10;
        const int MAX_VEL_Y = 10;

        private Yotsuba Font;
        private int Eated = 0;

        /// <summary>
        /// Inicializa el script, sonidos y posición inicial.
        /// <para>Initializes the script, sounds, and initial position.</para>
        /// </summary>
        public override void Initialize()
        {
            CreateSoundInstance("bounce");
            CreateSoundInstance("collect");
            PlayMusic("theme", true);

            SendLog("Bat Script Initialized", Color.Green);
            base.Initialize();

            Font = EntityManager.YotsubaEntities.FirstOrDefault(Entity => Entity.Name == "ScoreText");
            GetFontComponent(Font).Texto = "Score: 0";

            ApplyRandomMovement();
            
        }

        /// <summary>
        /// Actualiza el movimiento del murciélago cada cuadro.
        /// <para>Updates the bat movement each frame.</para>
        /// </summary>
        /// <param name="gametime">
        /// Tiempo de juego actual.
        /// <para>Current game time.</para>
        /// </param>
        public override void Update(GameTime gametime)
        {
            ref var transform = ref GetTransformComponent();
            if (transform.Position.X >= MaxWorldXPoint || transform.Position.X <= -MaxWorldXPoint ||
               transform.Position.Y >= MaxWorldYPoint || transform.Position.Y <= -MaxWorldYPoint)
            {
                float clampedX = MathHelper.Clamp(transform.Position.X, -MaxWorldXPoint, MaxWorldXPoint);
                float clampedY = MathHelper.Clamp(transform.Position.Y, -MaxWorldYPoint, MaxWorldYPoint);

                transform.SetPosition(clampedX, clampedY, transform.Position.Z);
                ReflectVelocity();
            }
        }

        /// <summary>
        /// Refleja la velocidad del murciélago al tocar los límites.
        /// <para>Reflects the bat velocity when hitting the bounds.</para>
        /// </summary>
        public void ReflectVelocity()
        {
            PlaySound("bounce");

            ref var rigidbody = ref GetRigidBody2D();
            Vector3 velocity = rigidbody.Velocity;

            ref var transform = ref GetTransformComponent();

            // Invierte velocidad X solo si está en el borde derecho y yendo hacia la derecha, o en el izquierdo y yendo hacia la izquierda
            if ((transform.Position.X >= MaxWorldXPoint && velocity.X > 0) || 
                (transform.Position.X <= -MaxWorldXPoint && velocity.X < 0))
                velocity.X = -velocity.X;

            // Invierte velocidad Y solo si está en el borde superior y yendo hacia arriba, o en el inferior y yendo hacia abajo
            if ((transform.Position.Y >= MaxWorldYPoint && velocity.Y > 0) || 
                (transform.Position.Y <= -MaxWorldYPoint && velocity.Y < 0))
                velocity.Y = -velocity.Y;

            rigidbody.Velocity = velocity;      
        }



        /// <summary>
        /// Aplica una posición y velocidad aleatorias al murciélago.
        /// <para>Applies a random position and velocity to the bat.</para>
        /// </summary>
        public void ApplyRandomMovement()
        {
            ref var transform = ref GetTransformComponent();
            transform.SetPosition(
                YTBGlobalState.Random.Next(-(int)MaxWorldXPoint, (int)MaxWorldXPoint),
                YTBGlobalState.Random.Next(-(int)MaxWorldYPoint, (int)MaxWorldYPoint),
                transform.Position.Z
            );

            int movementX = YTBGlobalState.Random.Next(-MAX_VEL_X, MAX_VEL_X);
            int movementY = YTBGlobalState.Random.Next(-MAX_VEL_Y, MAX_VEL_Y);


            Vector3 movement = new Vector3(movementX, movementY, 0);

            if(movement == Vector3.Zero)
            {
                movement = new Vector3(1, 1, 0);
            }

            GetRigidBody2D().Velocity = movement;
        }

        /// <summary>
        /// Maneja las colisiones y actualiza el puntaje.
        /// <para>Handles collisions and updates the score.</para>
        /// </summary>
        /// <param name="@event">
        /// Evento de colisión recibido.
        /// <para>Collision event received.</para>
        /// </param>
        public void OnCollision(OnCollitionEvent @event)
        {
            if (@event.EntityImpediment.Name == "Slime" || @event.EntityTryMove.Name == "Slime")
            {
                PlaySound("collect");
                ApplyRandomMovement();
                GetFontComponent(Font).Texto = "Score: " + (++Eated).ToString();
            }

        }

        /// <summary>
        /// Dibuja el script en 2D durante el renderizado.
        /// <para>Draws the script in 2D during rendering.</para>
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
        /// Limpia recursos del script y detiene la música.
        /// <para>Cleans up script resources and stops music.</para>
        /// </summary>
        public override void Cleanup()
        {
            StopMusic();
            base.Cleanup();
        }
    }
}
