using System;
using Microsoft.JSInterop;
using Microsoft.Xna.Framework;
using SandBoxGame.Core;

namespace SandBoxGame.BlazorGL.Pages
{
    public partial class Index
    {
        YTBProgram _game;

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                JsRuntime.InvokeAsync<object>("initRenderJS", DotNetObjectReference.Create(this));
            }
        }

        [JSInvokable]
        public void TickDotNet()
        {
            // init game
            if (_game == null)
            {
                _game = new YTBProgram(YotsubaEngine.Core.YotsubaGame.Platforms.Web_BlazorGL);
                _game.Run();
            }

            // run gameloop
            _game.Tick();
        }

    }
}
