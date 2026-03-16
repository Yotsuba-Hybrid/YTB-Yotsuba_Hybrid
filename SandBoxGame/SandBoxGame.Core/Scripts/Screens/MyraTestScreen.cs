using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Core.YotsubaGame.Scripting;
using YotsubaEngine.Forms;
using YotsubaEngine.Forms.Contract;
using YotsubaEngine.Forms.Contract.Myra;

namespace SandBoxGame.Core.Scripts.Screens
{
    /// <summary>
    /// Ejemplo de UI usando Myra como librería subyacente.
    /// <para>UI example using Myra as the underlying library.</para>
    /// </summary>
    [Script(UISystem = UILibrary.Myra)]
    public class MyraTestScreen : BaseScript
    {
        private IWindow _window;
        private IPanel _panel;
        private IButton _button;
        private ILabel _label;
        private ISlider _slider;

        public override void Initialize()
        {
            YTBGlobalState.EngineBackground = new Color(40, 44, 52,255);

            _window = UI.CreateWindow("Myra UI Test")
                .WithTitle("Myra UI Library Test")
                .WithPosition(100, 100)
                .Build();

            _panel = UI.CreatePanel()
                .WithPosition(10, 10)
                .Build();
            IMyraContainer mc = (IMyraContainer)_panel;
            mc.GetMyraContainer().Height = 30;

            //_panel = (IPanel)mc;

            _button = UI.CreateButton("Myra Button")
                .WithText("Click Me")
                .WithPosition(10, 35)
                .WithOnClick(() => SendLog("[MyraTestScreen] Button clicked!", Color.Cyan))
                .Build();

            _label = UI.CreateLabel("Myra Label")
                .WithText("This is a Myra label")
                .WithPosition(50, 150)
                .WithColor(Color.White)
                .Build();

            _slider = UI.CreateSlider("MYSLIDER")
                .WithPosition(new Vector2(100, 100))
                .WithRange(0, 30)
                .WithValue(15)
                .Build();

            _panel.AddChild(_slider);
            _panel.AddChild(_button);
            _panel.AddChild(_label);
            _window.AddChild(_panel);

            UI.AddToRoot(_window);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Cleanup()
        {
            UI.Clear();
            base.Cleanup();
        }
    }
}