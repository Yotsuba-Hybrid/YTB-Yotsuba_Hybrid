using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Core.YotsubaGame.Scripting;
using YotsubaEngine.Forms;
using YotsubaEngine.Forms.Contract;

namespace SandBoxGame.Core.Scripts.Screens
{
    /// <summary>
    /// Ejemplo de UI usando ImGui como librería subyacente.
    /// Ideal para herramientas de desarrollo y debugging.
    /// <para>UI example using ImGui as the underlying library.</para>
    /// <para>Ideal for development tools and debugging.</para>
    /// </summary>
    [Script(UISystem = UILibrary.ImGui)]
    public class ImGuiTestScreen : BaseScript
    {
        private IWindow _debugWindow;
        private IPanel _statsPanel;
        private ILabel _fpsLabel;
        private ISlider _speedSlider;
        private ICheckBox _showColliders;
        private ITextBox _commandTextBox;

        private float _currentFps;
        private float _speedValue = 50f;
        private bool _showCollidersEnabled;

        public override void Initialize()
        {
            YTBGlobalState.EngineBackground = new Color(25, 25, 35, 255);

            CreateDebugWindow();
            CreateStatsPanel();

            base.Initialize();

            SendLog("[ImGuiTestScreen] Initialized - ImGui mode active", Color.Magenta);
        }

        private void CreateDebugWindow()
        {
            _debugWindow = UI.CreateWindow("Debug Tools")
                .WithTitle("### Debug Tools - ImGui")
                .WithPosition(10, 10)
                .WithIsOpen(true)
                .Build();

            UI.AddToRoot(_debugWindow);
        }

        private void CreateStatsPanel()
        {
            _statsPanel = UI.CreatePanel()
                .WithPosition(5, 5)
                .Build();

            _fpsLabel = UI.CreateLabel("FPS: 0")
                .WithText("FPS: 0")
                .WithPosition(5, 5)
                .WithColor(Color.LightGreen)
                .Build();

            _speedSlider = UI.CreateSlider()
                .WithPosition(5, 30)
                .WithRange(0,100)
                .WithValue(_speedValue)
                .WithOnValueChanged(OnSpeedChanged)
                .Build();

            _showColliders = UI.CreateCheckBox("Show Colliders")
                .WithText("Show Colliders")
                .WithPosition(5, 70)
                .WithIsChecked(_showCollidersEnabled)
                .WithOnCheckedChanged(OnShowCollidersChanged)
                .Build();

            _commandTextBox = UI.CreateTextBox("Enter command...")
                .WithPosition(5, 110)
                .WithText("")
                .WithOnSubmit(OnCommandSubmit)
                .Build();

            _statsPanel.AddChild(_fpsLabel);
            _statsPanel.AddChild(_speedSlider);
            _statsPanel.AddChild(_showColliders);
            _statsPanel.AddChild(_commandTextBox);
            _debugWindow.AddChild(_statsPanel);
        }

        private void OnSpeedChanged(float value)
        {
            _speedValue = value;
            SendLog($"[ImGuiTestScreen] Speed: {value:F1}", Color.Yellow);
        }

        private void OnShowCollidersChanged(bool isChecked)
        {
            _showCollidersEnabled = isChecked;
            SendLog($"[ImGuiTestScreen] Show Colliders: {isChecked}", Color.Orange);
        }

        private void OnCommandSubmit(string command)
        {
            SendLog($"[ImGuiTestScreen] Command: '{command}'", Color.Cyan);
        }

        public override void Update(GameTime gameTime)
        {
            _currentFps = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;
            _fpsLabel.Text = $"FPS: {_currentFps:F0}";

            base.Update(gameTime);
        }

        public override void Cleanup()
        {
            UI.Clear();
            SendLog("[ImGuiTestScreen] Cleanup completed", Color.Gray);
            base.Cleanup();
        }
    }
}