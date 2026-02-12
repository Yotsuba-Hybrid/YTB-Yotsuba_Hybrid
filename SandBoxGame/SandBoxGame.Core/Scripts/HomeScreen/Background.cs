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
    /// <summary>
    /// Script que dibuja el fondo animado del menú principal.
    /// <para>Script that draws the animated background for the main menu.</para>
    /// </summary>
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
        /// <summary>
        /// Inicializa el fondo y carga la textura del patrón.
        /// <para>Initializes the background and loads the pattern texture.</para>
        /// </summary>
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

       
        /// <summary>
        /// Actualiza el estado del fondo cada cuadro.
        /// <para>Updates the background state each frame.</para>
        /// </summary>
        /// <param name="gameTime">
        /// Tiempo de juego actual.
        /// <para>Current game time.</para>
        /// </param>
        public override void Update(GameTime gameTime)
        {


            base.Update(gameTime);

            
        }


        /// <summary>
        /// Dibuja el fondo en 2D durante el renderizado.
        /// <para>Draws the background in 2D during rendering.</para>
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

            // Draw the background pattern first using the PointWrap sampler state.
            spriteBatch.Begin(samplerState: SamplerState.PointWrap/*, effect: effect*/);
            spriteBatch.Draw(_backgroundPattern, _backgroundDestination, new Rectangle(_backgroundOffset.ToPoint(), _backgroundDestination.Size), Color.White * 0.5f);
            spriteBatch.End();

            base.Draw2D(spriteBatch, gameTime);
        }
        
    }
}
