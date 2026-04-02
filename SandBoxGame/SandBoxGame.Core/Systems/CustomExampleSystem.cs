using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.YotsubaGame;
using MonoGameGum;

namespace SandBoxGame.Core.Systems
{
    public class CustomExampleSystem : IRenderSystem
    {
        GumService GumUI => GumService.Default;
        public CustomExampleSystem()
        {
           
        }

        public override void InitializeSystem(EntityManager entities)
        {
            EntityManager = entities;
            if (!GumUI.IsInitialized)
            {
                var gumProject = GumUI.Initialize(YTBGlobalState.Game, "Gum/GumProject.gumx");
            }

            var screen = new YTBTESTRuntime();
            screen.AddToRoot();
        }

        public override void SharedEntityInitialize(ref Yotsuba Entidad) { }
        public override void SharedEntityForEachUpdate(ref Yotsuba Entidad, GameTime time) { }

        public override void UpdateSystem(GameTime gameTime)
        {
            GumUI.Update(gameTime);
        }

        public override void Render2D(SpriteBatch spriteBatch, GameTime gameTime)
        {

            GumUI.Draw();
        }

        public override void Render3D(GameTime gameTime) { }
        public override void Dispose() { }
    }
}