using System;
using System.Collections.Generic;
using System.Text;

namespace YotsubaEngine.Core.YotsubaGame
{
    public class ScriptExample
    {
        public const string Script =
@"
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Attributes;
using YotsubaEngine.Core.YotsubaGame.Scripting;

namespace YTBNamespace;

[Script]
public class Template : BaseScript 
{
    public override void Initialize()
    {
       // Code Here
       base.Initialize();
    }

    public override void Update(GameTime gametime)
    {
       // Code Here
       base.Update(gametime);
    }
    
    public override void Draw2D(SpriteBatch spriteBatch, GameTime gameTime)
    {
       // Code Here
       base.Draw2D(spriteBatch, gameTime);
    }

    public override void Draw3D(GameTime gameTime)
    {
       // Code Here
       base.Draw3D(gameTime);
    }

    public override void Cleanup()
    {
       base.Cleanup();
    }
}
";
    }
}
