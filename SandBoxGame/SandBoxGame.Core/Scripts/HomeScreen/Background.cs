using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using YotsubaEngine.Attributes;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.YotsubaGame.Scripting;

namespace SandBoxGame.Core.Scripts.HomeScreen
{
    [Script]
    public class Background : BaseScript
    {
        // The texture used for the background pattern.
        private Texture2D _backgroundPattern;

        // The destination rectangle for the background pattern to fill.
        private Rectangle _backgroundDestination;

        // The offset to apply when drawing the background pattern so it appears to
        // be scrolling.
        private Vector2 _backgroundOffset;

        float _progress = 0;
        bool ASC = true;
        public override void Initialize()
        {
            // Initialize the offset of the background pattern at zero.
            _backgroundOffset = Vector2.Zero;

            // Set the background pattern destination rectangle to fill the entire
            // screen background.
            _backgroundDestination = YTBGlobalState.GraphicsDevice.PresentationParameters.Bounds;
            base.Initialize();

            //effect = YTBGlobalState.ContentManager.Load<Effect>("Shaders/Transition");
            // Load the background pattern texture.
            _backgroundPattern = YTBGlobalState.ContentManager.Load<Texture2D>("MonoGameTutorial/background-pattern");

            //effect.Parameters["Progress"].SetValue(_progress);
            //effect.Parameters["TransitionType"].SetValue(0);
        }

       
        public override void Update(GameTime gameTime)
        {


            base.Update(gameTime);

            
        }


        public override void Draw2D(SpriteBatch spriteBatch, GameTime gameTime)
        {

            // Draw the background pattern first using the PointWrap sampler state.
            spriteBatch.Begin(samplerState: SamplerState.PointWrap/*, effect: effect*/);
            spriteBatch.Draw(_backgroundPattern, _backgroundDestination, new Rectangle(_backgroundOffset.ToPoint(), _backgroundDestination.Size), Color.White * 0.5f);
            spriteBatch.End();

            base.Draw2D(spriteBatch, gameTime);
        }
        
    }
}
