using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Core.YotsubaGame.Scripting;
using YotsubaEngine.Forms;
using YotsubaEngine.Forms.Contract;

namespace SandBoxGame.Core.Scripts.Screens
{
    /// <summary>
    /// Test extensivo de UI usando Myra como libreria subyacente.
    /// Demuestra 20+ controles organizados en multiples paneles.
    /// <para>Extensive UI test using Myra as the underlying library.</para>
    /// </summary>
    [Script(UISystem = UILibrary.Myra)]
    public class MyraTestScreen : BaseScript
    {
        // Windows
        private IWindow _mainWindow;
        private IWindow _settingsWindow;

        // Panels
        private IPanel _headerPanel;
        private IPanel _actionsPanel;
        private IPanel _inputPanel;
        private IPanel _slidersPanel;
        private IPanel _statusPanel;

        // Controls
        private ILabel _titleLabel;
        private ILabel _subtitleLabel;
        private ILabel _fpsLabel;
        private ILabel _clickCountLabel;
        private ILabel _statusLabel;
        private ILabel _sliderValueLabel;

        private IButton _actionBtn;
        private IButton _resetBtn;
        private IButton _toggleBtn;

        private ICheckBox _enableSoundChk;
        private ICheckBox _fullscreenChk;
        private ICheckBox _vsyncChk;

        private ISlider _volumeSlider;
        private ISlider _brightnessSlider;

        private ITextBox _playerNameTxt;
        private ITextBox _chatTxt;

        private IComboBox _resolutionCmb;
        private IComboBox _difficultyCmb;

        // State
        private int _clickCount;
        private float _volume = 75f;
        private float _brightness = 50f;
        private bool _toggleState;

        public override void Initialize()
        {
            YTBGlobalState.EngineBackground = new Color(40, 44, 52, 255);

            CreateMainWindow();
            CreateSettingsWindow();

            base.Initialize();
            SendLog("[MyraTestScreen] Initialized - Myra mode active with 20+ controls", Color.Green);
        }

        private void CreateMainWindow()
        {
            _mainWindow = UI.CreateWindow("Myra Main")
                .WithTitle("Myra UI - Main Dashboard")
                .WithPosition(20, 20)
                .WithIsOpen(true)
                .Build();

            // --- Header Panel ---
            _headerPanel = UI.CreatePanel("HeaderPanel")
                .WithPosition(5, 5)
                .Build();

            _titleLabel = UI.CreateLabel("title")
                .WithText("Myra UI Library Test")
                .WithPosition(5, 5)
                .WithColor(Color.Gold)
                .Build();

            _subtitleLabel = UI.CreateLabel("subtitle")
                .WithText("20+ controls demo")
                .WithPosition(5, 25)
                .WithColor(Color.LightGray)
                .Build();

            _fpsLabel = UI.CreateLabel("fps")
                .WithText("FPS: 0")
                .WithPosition(250, 5)
                .WithColor(Color.LightGreen)
                .Build();

            _headerPanel.AddChild(_titleLabel);
            _headerPanel.AddChild(_subtitleLabel);
            _headerPanel.AddChild(_fpsLabel);

            // --- Actions Panel ---
            _actionsPanel = UI.CreatePanel("ActionsPanel")
                .WithPosition(5, 55)
                .Build();

            _actionBtn = UI.CreateButton("action")
                .WithText("Click Me!")
                .WithPosition(5, 5)
                .WithOnClick(OnActionClicked)
                .Build();

            _resetBtn = UI.CreateButton("reset")
                .WithText("Reset Counter")
                .WithPosition(120, 5)
                .WithOnClick(OnResetClicked)
                .Build();

            _toggleBtn = UI.CreateButton("toggle")
                .WithText("Toggle: OFF")
                .WithPosition(260, 5)
                .WithOnClick(OnToggleClicked)
                .Build();

            _clickCountLabel = UI.CreateLabel("clickcount")
                .WithText("Clicks: 0")
                .WithPosition(5, 40)
                .WithColor(Color.Cyan)
                .Build();

            _actionsPanel.AddChild(_actionBtn);
            _actionsPanel.AddChild(_resetBtn);
            _actionsPanel.AddChild(_toggleBtn);
            _actionsPanel.AddChild(_clickCountLabel);

            // --- Input Panel ---
            _inputPanel = UI.CreatePanel("InputPanel")
                .WithPosition(5, 125)
                .Build();

            _playerNameTxt = UI.CreateTextBox("name")
                .WithText("Player1")
                .WithPosition(5, 5)
                .WithOnSubmit(name => SendLog($"[Myra] Player name: {name}", Color.Cyan))
                .Build();

            _chatTxt = UI.CreateTextBox("chat")
                .WithText("")
                .WithPosition(5, 35)
                .WithOnSubmit(msg => SendLog($"[Myra] Chat: {msg}", Color.White))
                .Build();

            _resolutionCmb = UI.CreateComboBox("Resolution")
                .WithPosition(5, 65)
                .WithItems("1920x1080", "1280x720", "2560x1440", "3840x2160")
                .WithSelectedIndex(0)
                .WithOnSelectionChanged(idx => SendLog($"[Myra] Resolution: {_resolutionCmb.Items[idx]}", Color.Yellow))
                .Build();

            _difficultyCmb = UI.CreateComboBox("Difficulty")
                .WithPosition(5, 95)
                .WithItems("Easy", "Normal", "Hard", "Nightmare")
                .WithSelectedIndex(1)
                .WithOnSelectionChanged(idx => SendLog($"[Myra] Difficulty: {_difficultyCmb.Items[idx]}", Color.Orange))
                .Build();

            _inputPanel.AddChild(_playerNameTxt);
            _inputPanel.AddChild(_chatTxt);
            _inputPanel.AddChild(_resolutionCmb);
            _inputPanel.AddChild(_difficultyCmb);

            // --- Status Panel ---
            _statusPanel = UI.CreatePanel("StatusPanel")
                .WithPosition(5, 255)
                .Build();

            _statusLabel = UI.CreateLabel("status")
                .WithText("Status: Ready")
                .WithPosition(5, 5)
                .WithColor(Color.LightGreen)
                .Build();

            _statusPanel.AddChild(_statusLabel);

            // Assemble main window
            _mainWindow.AddChild(_headerPanel);
            _mainWindow.AddChild(_actionsPanel);
            _mainWindow.AddChild(_inputPanel);
            _mainWindow.AddChild(_statusPanel);

            UI.AddToRoot(_mainWindow);
        }

        private void CreateSettingsWindow()
        {
            _settingsWindow = UI.CreateWindow("Myra Settings")
                .WithTitle("Settings")
                .WithPosition(450, 20)
                .WithIsOpen(true)
                .Build();

            _slidersPanel = UI.CreatePanel("SlidersPanel")
                .WithPosition(5, 5)
                .Build();

            var volumeLabel = UI.CreateLabel("vollbl")
                .WithText("Volume")
                .WithPosition(5, 5)
                .WithColor(Color.White)
                .Build();

            _volumeSlider = UI.CreateSlider("volume")
                .WithPosition(5, 25)
                .WithRange(0, 100)
                .WithValue(_volume)
                .WithOnValueChanged(OnVolumeChanged)
                .Build();

            var brightnessLabel = UI.CreateLabel("brtlbl")
                .WithText("Brightness")
                .WithPosition(5, 55)
                .WithColor(Color.White)
                .Build();

            _brightnessSlider = UI.CreateSlider("brightness")
                .WithPosition(5, 75)
                .WithRange(0, 100)
                .WithValue(_brightness)
                .WithOnValueChanged(OnBrightnessChanged)
                .Build();

            _sliderValueLabel = UI.CreateLabel("slidervals")
                .WithText($"Vol: {_volume:F0}% | Brt: {_brightness:F0}%")
                .WithPosition(5, 105)
                .WithColor(Color.Yellow)
                .Build();

            _enableSoundChk = UI.CreateCheckBox("Sound")
                .WithText("Enable Sound")
                .WithPosition(5, 135)
                .WithIsChecked(true)
                .WithOnCheckedChanged(v => SendLog($"[Myra] Sound: {v}", Color.White))
                .Build();

            _fullscreenChk = UI.CreateCheckBox("Fullscreen")
                .WithText("Fullscreen")
                .WithPosition(5, 165)
                .WithIsChecked(false)
                .WithOnCheckedChanged(v => SendLog($"[Myra] Fullscreen: {v}", Color.White))
                .Build();

            _vsyncChk = UI.CreateCheckBox("VSync")
                .WithText("VSync")
                .WithPosition(5, 195)
                .WithIsChecked(true)
                .WithOnCheckedChanged(v => SendLog($"[Myra] VSync: {v}", Color.White))
                .Build();

            _slidersPanel.AddChild(volumeLabel);
            _slidersPanel.AddChild(_volumeSlider);
            _slidersPanel.AddChild(brightnessLabel);
            _slidersPanel.AddChild(_brightnessSlider);
            _slidersPanel.AddChild(_sliderValueLabel);
            _slidersPanel.AddChild(_enableSoundChk);
            _slidersPanel.AddChild(_fullscreenChk);
            _slidersPanel.AddChild(_vsyncChk);

            _settingsWindow.AddChild(_slidersPanel);

            UI.AddToRoot(_settingsWindow);
        }

        #region Event Handlers

        private void OnActionClicked()
        {
            _clickCount++;
            _clickCountLabel.Text = $"Clicks: {_clickCount}";
            _statusLabel.Text = $"Status: Button clicked ({_clickCount})";
            SendLog($"[Myra] Click #{_clickCount}", Color.Cyan);
        }

        private void OnResetClicked()
        {
            _clickCount = 0;
            _clickCountLabel.Text = "Clicks: 0";
            _statusLabel.Text = "Status: Counter reset";
            SendLog("[Myra] Counter reset", Color.Yellow);
        }

        private void OnToggleClicked()
        {
            _toggleState = !_toggleState;
            _toggleBtn.Text = _toggleState ? "Toggle: ON" : "Toggle: OFF";
            _statusLabel.Text = $"Status: Toggle {(_toggleState ? "ON" : "OFF")}";
        }

        private void OnVolumeChanged(float value)
        {
            _volume = value;
            _sliderValueLabel.Text = $"Vol: {_volume:F0}% | Brt: {_brightness:F0}%";
        }

        private void OnBrightnessChanged(float value)
        {
            _brightness = value;
            _sliderValueLabel.Text = $"Vol: {_volume:F0}% | Brt: {_brightness:F0}%";
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            float fps = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;
            _fpsLabel.Text = $"FPS: {fps:F0}";
            base.Update(gameTime);
        }

        public override void Cleanup()
        {
            UI.Clear();
            base.Cleanup();
        }
    }
}
