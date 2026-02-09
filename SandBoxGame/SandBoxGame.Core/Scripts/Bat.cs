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
    public class Bat : BaseScript, ICollisionListener
    {

        public float ScreenWidth = YTBGlobalState.GraphicsDevice.Viewport.Width;
        public float ScreenHeight = YTBGlobalState.GraphicsDevice.Viewport.Height;

        public static float MaxWorldXPoint = (int)(YTBGlobalState.GraphicsDevice.Viewport.Width * 0.5f) - 180;
        public static float MaxWorldYPoint = (int)(YTBGlobalState.GraphicsDevice.Viewport.Height * 0.5f) - 180;

        const int MAX_VEL_X = 10;
        const int MAX_VEL_Y = 10;

        private Yotsuba Font;
        private int Eated = 0;
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

        public void OnCollision(OnCollitionEvent @event)
        {
            if (@event.EntityImpediment.Name == "Slime" || @event.EntityTryMove.Name == "Slime")
            {
                PlaySound("collect");
                ApplyRandomMovement();
                GetFontComponent(Font).Texto = "Score: " + (++Eated).ToString();
            }

        }

        public override void Draw2D(SpriteBatch spriteBatch, GameTime gameTime)
        {
           
            base.Draw2D(spriteBatch, gameTime);
        }

        public override void Cleanup()
        {
            StopMusic();
            base.Cleanup();
        }
    }
}
