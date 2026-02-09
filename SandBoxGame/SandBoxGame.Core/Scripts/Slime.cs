using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Attributes;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.YotsubaGame.Scripting;
using YotsubaEngine.Events.YTBEvents;

namespace SandBoxGame.Core.Scripts
{
    [Script]
    public class Slime : BaseScript, ICollisionListener
    {

        public float ScreenWidth = YTBGlobalState.GraphicsDevice.Viewport.Width;
        public float ScreenHeight = YTBGlobalState.GraphicsDevice.Viewport.Height;


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

        public override void Update(GameTime gametime)
        {
            base.Update(gametime);
        }
        public override void Draw2D(SpriteBatch spriteBatch, GameTime gameTime)
        { 
            base.Draw2D(spriteBatch, gameTime);
        }

        public void OnCollision(OnCollitionEvent @event)
        {
            
        } 
    }
}
