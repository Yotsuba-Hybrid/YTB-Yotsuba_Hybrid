using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Core.YotsubaGame.Scripting;
using YotsubaEngine.Forms;
using YotsubaEngine.Forms.Contract;

namespace SandBoxGame.Core.Scripts.Screens
{
    /// <summary>
    /// Test extensivo de UI usando ImGui como libreria subyacente.
    /// Demuestra 20+ controles organizados en multiples paneles.
    /// <para>Extensive UI test using ImGui as the underlying library.</para>
    /// </summary>
    [Script(UISystem = UILibrary.ImGui)]
    public class ImGuiTestScreen : BaseScript
    {
        // Windows
        private IWindow _toolsWindow;
        private IWindow _inspectorWindow;

        // Panels
        private IPanel _metricsPanel;
        private IPanel _controlPanel;
        private IPanel _logPanel;
        private IPanel _transformPanel;
        private IPanel _renderPanel;

        // Metrics controls
        private ILabel _fpsLabel;
        private ILabel _frameTimeLabel;
        private ILabel _entityCountLabel;

        // Control controls
        private IButton _playBtn;
        private IButton _pauseBtn;
        private IButton _stepBtn;
        private IButton _resetBtn;
        private ICheckBox _showGridChk;
        private ICheckBox _showCollidersChk;
        private ICheckBox _showWireframeChk;

        // Log controls
        private ITextBox _commandTxt;
        private ILabel _lastCommandLabel;
        private IComboBox _logLevelCmb;

        // Inspector controls
        private ILabel _posLabel;
        private ISlider _posXSlider;
        private ISlider _posYSlider;
        private ISlider _rotationSlider;
        private ISlider _scaleSlider;
        private IComboBox _layerCmb;
        private ICheckBox _visibleChk;

        // State
        private float _currentFps;
        private float _posX = 0f;
        private float _posY = 0f;
        private float _rotation = 0f;
        private float _scale = 1f;
        private bool _isPlaying;

        public override void Initialize()
        {
            YTBGlobalState.EngineBackground = new Color(25, 25, 35, 255);

            CreateToolsWindow();
            CreateInspectorWindow();

            base.Initialize();
            SendLog("[ImGuiTestScreen] Initialized - ImGui mode active with 20+ controls", Color.Magenta);
        }

        private void CreateToolsWindow()
        {
            _toolsWindow = UI.CreateWindow("Dev Tools")
                .WithTitle("Developer Tools")
                .WithPosition(10, 10)
                .WithIsOpen(true)
                .Build();

            // --- Metrics Panel ---
            _metricsPanel = UI.CreatePanel("Metrics")
                .WithPosition(0, 0)
                .Build();

            _fpsLabel = UI.CreateLabel("fps")
                .WithText("FPS: 0")
                .WithPosition(5, 5)
                .WithColor(Color.LightGreen)
                .Build();

            _frameTimeLabel = UI.CreateLabel("frametime")
                .WithText("Frame: 0.0ms")
                .WithPosition(5, 25)
                .WithColor(Color.LightBlue)
                .Build();

            _entityCountLabel = UI.CreateLabel("entities")
                .WithText("Entities: 0")
                .WithPosition(5, 45)
                .WithColor(Color.White)
                .Build();

            _metricsPanel.AddChild(_fpsLabel);
            _metricsPanel.AddChild(_frameTimeLabel);
            _metricsPanel.AddChild(_entityCountLabel);

            // --- Control Panel ---
            _controlPanel = UI.CreatePanel("Controls")
                .WithPosition(0, 75)
                .Build();

            _playBtn = UI.CreateButton("play")
                .WithText("Play")
                .WithPosition(5, 5)
                .WithOnClick(OnPlayClicked)
                .Build();

            _pauseBtn = UI.CreateButton("pause")
                .WithText("Pause")
                .WithPosition(75, 5)
                .WithOnClick(OnPauseClicked)
                .Build();

            _stepBtn = UI.CreateButton("step")
                .WithText("Step")
                .WithPosition(155, 5)
                .WithOnClick(() => SendLog("[ImGui] Step frame", Color.Cyan))
                .Build();

            _resetBtn = UI.CreateButton("reset")
                .WithText("Reset Scene")
                .WithPosition(225, 5)
                .WithOnClick(() => SendLog("[ImGui] Scene reset", Color.Red))
                .Build();

            _showGridChk = UI.CreateCheckBox("Grid")
                .WithText("Show Grid")
                .WithPosition(5, 40)
                .WithIsChecked(true)
                .WithOnCheckedChanged(v => SendLog($"[ImGui] Grid: {v}", Color.White))
                .Build();

            _showCollidersChk = UI.CreateCheckBox("Colliders")
                .WithText("Show Colliders")
                .WithPosition(5, 65)
                .WithIsChecked(false)
                .WithOnCheckedChanged(v => SendLog($"[ImGui] Colliders: {v}", Color.White))
                .Build();

            _showWireframeChk = UI.CreateCheckBox("Wireframe")
                .WithText("Wireframe Mode")
                .WithPosition(5, 90)
                .WithIsChecked(false)
                .WithOnCheckedChanged(v => SendLog($"[ImGui] Wireframe: {v}", Color.White))
                .Build();

            _controlPanel.AddChild(_playBtn);
            _controlPanel.AddChild(_pauseBtn);
            _controlPanel.AddChild(_stepBtn);
            _controlPanel.AddChild(_resetBtn);
            _controlPanel.AddChild(_showGridChk);
            _controlPanel.AddChild(_showCollidersChk);
            _controlPanel.AddChild(_showWireframeChk);

            // --- Log Panel ---
            _logPanel = UI.CreatePanel("Log")
                .WithPosition(0, 200)
                .Build();

            _commandTxt = UI.CreateTextBox("cmd")
                .WithText("")
                .WithPosition(5, 5)
                .WithOnSubmit(OnCommandSubmit)
                .Build();

            _lastCommandLabel = UI.CreateLabel("lastcmd")
                .WithText("Last command: (none)")
                .WithPosition(5, 35)
                .WithColor(Color.Gray)
                .Build();

            _logLevelCmb = UI.CreateComboBox("LogLevel")
                .WithPosition(5, 55)
                .WithItems("Verbose", "Info", "Warning", "Error")
                .WithSelectedIndex(1)
                .WithOnSelectionChanged(idx => SendLog($"[ImGui] Log level: {_logLevelCmb.Items[idx]}", Color.Yellow))
                .Build();

            _logPanel.AddChild(_commandTxt);
            _logPanel.AddChild(_lastCommandLabel);
            _logPanel.AddChild(_logLevelCmb);

            // Assemble tools window
            _toolsWindow.AddChild(_metricsPanel);
            _toolsWindow.AddChild(_controlPanel);
            _toolsWindow.AddChild(_logPanel);

            UI.AddToRoot(_toolsWindow);
        }

        private void CreateInspectorWindow()
        {
            _inspectorWindow = UI.CreateWindow("Inspector")
                .WithTitle("Entity Inspector")
                .WithPosition(400, 10)
                .WithIsOpen(true)
                .Build();

            // --- Transform Panel ---
            _transformPanel = UI.CreatePanel("Transform")
                .WithPosition(0, 0)
                .Build();

            _posLabel = UI.CreateLabel("poslbl")
                .WithText("Position: (0, 0)")
                .WithPosition(5, 5)
                .WithColor(Color.White)
                .Build();

            _posXSlider = UI.CreateSlider("PosX")
                .WithPosition(5, 30)
                .WithRange(-500, 500)
                .WithValue(_posX)
                .WithOnValueChanged(OnPosXChanged)
                .Build();

            _posYSlider = UI.CreateSlider("PosY")
                .WithPosition(5, 55)
                .WithRange(-500, 500)
                .WithValue(_posY)
                .WithOnValueChanged(OnPosYChanged)
                .Build();

            _rotationSlider = UI.CreateSlider("Rotation")
                .WithPosition(5, 85)
                .WithRange(0, 360)
                .WithValue(_rotation)
                .WithOnValueChanged(v => { _rotation = v; UpdateTransformLabel(); })
                .Build();

            _scaleSlider = UI.CreateSlider("Scale")
                .WithPosition(5, 115)
                .WithRange(0.1f, 5f)
                .WithValue(_scale)
                .WithOnValueChanged(v => { _scale = v; UpdateTransformLabel(); })
                .Build();

            _transformPanel.AddChild(_posLabel);
            _transformPanel.AddChild(_posXSlider);
            _transformPanel.AddChild(_posYSlider);
            _transformPanel.AddChild(_rotationSlider);
            _transformPanel.AddChild(_scaleSlider);

            // --- Render Panel ---
            _renderPanel = UI.CreatePanel("Render")
                .WithPosition(0, 150)
                .Build();

            _layerCmb = UI.CreateComboBox("Layer")
                .WithPosition(5, 5)
                .WithItems("Default", "Background", "Foreground", "UI", "Overlay")
                .WithSelectedIndex(0)
                .WithOnSelectionChanged(idx => SendLog($"[ImGui] Layer: {_layerCmb.Items[idx]}", Color.Orange))
                .Build();

            _visibleChk = UI.CreateCheckBox("Visible")
                .WithText("Visible")
                .WithPosition(5, 35)
                .WithIsChecked(true)
                .WithOnCheckedChanged(v => SendLog($"[ImGui] Visible: {v}", Color.White))
                .Build();

            _renderPanel.AddChild(_layerCmb);
            _renderPanel.AddChild(_visibleChk);

            // Assemble inspector window
            _inspectorWindow.AddChild(_transformPanel);
            _inspectorWindow.AddChild(_renderPanel);

            UI.AddToRoot(_inspectorWindow);
        }

        #region Event Handlers

        private void OnPlayClicked()
        {
            _isPlaying = true;
            SendLog("[ImGui] Playing", Color.Green);
        }

        private void OnPauseClicked()
        {
            _isPlaying = false;
            SendLog("[ImGui] Paused", Color.Yellow);
        }

        private void OnCommandSubmit(string command)
        {
            _lastCommandLabel.Text = $"Last command: {command}";
            SendLog($"[ImGui] > {command}", Color.Cyan);
        }

        private void OnPosXChanged(float value)
        {
            _posX = value;
            UpdateTransformLabel();
        }

        private void OnPosYChanged(float value)
        {
            _posY = value;
            UpdateTransformLabel();
        }

        private void UpdateTransformLabel()
        {
            _posLabel.Text = $"Pos:({_posX:F0},{_posY:F0}) Rot:{_rotation:F0} Scl:{_scale:F1}";
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            _currentFps = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;
            float frameTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            _fpsLabel.Text = $"FPS: {_currentFps:F0}";
            _frameTimeLabel.Text = $"Frame: {frameTime:F1}ms";

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
