using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using System;
using System.Diagnostics;
using System.IO;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.YotsubaGame;
namespace SandBoxGame.Core.Systems
{
    public class CustomExampleSystem : IRenderSystem
    {

        GumService GumUI => GumService.Default;
        public override void InitializeSystem(EntityManager entities)
        {
            EntityManager = entities;

            try
            {
                if (!GumUI.IsInitialized)
                    GumService.Default.Initialize(YTBGlobalState.Game, "GumProject/GumProject.gumx");

            }catch(System.Exception ex)
            {
                System.Console.WriteLine($"Error initializing Gum: {ex.Message}");
                Debug.WriteLine($"Error initializing Gum: {ex.Message}");
            }
            var screen = new DemoScreenGumRuntime();
            screen.AddToRoot();
        }

        public override void SharedEntityForEachUpdate(ref Yotsuba Entidad, GameTime time) { }

        public override void UpdateSystem(GameTime gameTime)
        {
            YTBGlobalState.ColorBackground = Color.Turquoise;

            GumUI.Update(gameTime);
        }

        public override void Render2D(SpriteBatch spriteBatch, GameTime gameTime)
        {

                GumUI.Draw();

        }
        public override void Dispose() { }
    }
}