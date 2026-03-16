using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Core.YotsubaGame.Scripting;
using YotsubaEngine.Forms;
using YotsubaEngine.Forms.Contract;

namespace SandBoxGame.Core.Scripts.Screens
{
    /// <summary>
    /// Test extensivo de UI usando GumUI como libreria subyacente.
    /// Demuestra 20+ controles organizados en multiples paneles.
    /// <para>Extensive UI test using GumUI as the underlying library.</para>
    /// </summary>
    [Script(UISystem = UILibrary.ImGui)]
    public class UITestScreen : BaseScript
    {
        // Windows
        private IWindow _mainWindow;
        private IWindow _configWindow;

        // Panels
        private IPanel _headerPanel;
        private IPanel _controlsPanel;
        private IPanel _formPanel;
        private IPanel _resultPanel;
        private IPanel _audioPanel;
        private IPanel _graphicsPanel;

        // Header controls
        private ILabel _titleLabel;
        private ILabel _fpsLabel;
        private ILabel _statusLabel;

        // Action controls
        private IButton _clickBtn;
        private IButton _resetBtn;
        private IButton _saveBtn;
        private IButton _loadBtn;
        private ILabel _clickCountLabel;

        // Form controls
        private ITextBox _usernameTxt;
        private ITextBox _emailTxt;
        private IComboBox _countryCmb;
        private IComboBox _themeCmb;
        private ICheckBox _agreeTos;
        private ICheckBox _newsletter;

        // Result
        private ILabel _resultLabel;

        // Audio settings
        private ILabel _audioTitleLabel;
        private ISlider _masterVolSlider;
        private ISlider _musicVolSlider;
        private ISlider _sfxVolSlider;
        private ICheckBox _muteChk;

        // Graphics settings
        private ILabel _gfxTitleLabel;
        private ISlider _fovSlider;
        private IComboBox _qualityCmb;
        private ICheckBox _antiAliasingChk;
        private ICheckBox _shadowsChk;
        private ICheckBox _bloomChk;

        // State
        private int _clickCount;
        private float _masterVol = 80f;
        private float _musicVol = 60f;
        private float _sfxVol = 70f;
        private float _fov = 90f;

        public override void Initialize()
        {
            YTBGlobalState.EngineBackground = new Color(30, 30, 40, 255);

            CreateMainWindow();
            CreateConfigWindow();

            base.Initialize();
            SendLog("[UITestScreen] Initialized - GumUI mode active with 20+ controls", Color.Green);
        }

        private void CreateMainWindow()
        {
            _mainWindow = UI.CreateWindow("GumUI Main")
                .WithTitle("GumUI - Main Dashboard")
                .WithPosition(20, 20)
                .WithIsOpen(true)
                .Build();

            // --- Header Panel ---
            _headerPanel = UI.CreatePanel("Header")
                .WithPosition(5, 5)
                .Build();

            _titleLabel = UI.CreateLabel("title")
                .WithText("GumUI Library Test")
                .WithPosition(5, 5)
                .WithColor(Color.Gold)
                .Build();

            _fpsLabel = UI.CreateLabel("fps")
                .WithText("FPS: 0")
                .WithPosition(250, 5)
                .WithColor(Color.LightGreen)
                .Build();

            _statusLabel = UI.CreateLabel("status")
                .WithText("Status: Ready")
                .WithPosition(5, 25)
                .WithColor(Color.LightGray)
                .Build();

            _headerPanel.AddChild(_titleLabel);
            _headerPanel.AddChild(_fpsLabel);
            _headerPanel.AddChild(_statusLabel);

            // --- Controls Panel ---
            _controlsPanel = UI.CreatePanel("Controls")
                .WithPosition(5, 55)
                .Build();

            _clickBtn = UI.CreateButton("click")
                .WithText("Click Me!")
                .WithPosition(5, 5)
                .WithOnClick(OnClickBtn)
                .Build();

            _resetBtn = UI.CreateButton("reset")
                .WithText("Reset")
                .WithPosition(120, 5)
                .WithOnClick(OnResetBtn)
                .Build();

            _saveBtn = UI.CreateButton("save")
                .WithText("Save")
                .WithPosition(220, 5)
                .WithOnClick(() => UpdateStatus("Saved!"))
                .Build();

            _loadBtn = UI.CreateButton("load")
                .WithText("Load")
                .WithPosition(310, 5)
                .WithOnClick(() => UpdateStatus("Loaded!"))
                .Build();

            _clickCountLabel = UI.CreateLabel("clicks")
                .WithText("Clicks: 0")
                .WithPosition(5, 40)
                .WithColor(Color.Cyan)
                .Build();

            _controlsPanel.AddChild(_clickBtn);
            _controlsPanel.AddChild(_resetBtn);
            _controlsPanel.AddChild(_saveBtn);
            _controlsPanel.AddChild(_loadBtn);
            _controlsPanel.AddChild(_clickCountLabel);

            // --- Form Panel ---
            _formPanel = UI.CreatePanel("Form")
                .WithPosition(5, 115)
                .Build();

            _usernameTxt = UI.CreateTextBox("user")
                .WithText("Username")
                .WithPosition(5, 5)
                .WithOnValueChanged(v => UpdateStatus($"Typing: {v}"))
                .Build();

            _emailTxt = UI.CreateTextBox("email")
                .WithText("user@example.com")
                .WithPosition(5, 35)
                .WithOnSubmit(v => SendLog($"[GumUI] Email submitted: {v}", Color.Cyan))
                .Build();

            _countryCmb = UI.CreateComboBox("Country")
                .WithPosition(5, 65)
                .WithItems("United States", "Japan", "Germany", "Brazil", "Australia")
                .WithSelectedIndex(0)
                .WithOnSelectionChanged(idx => SendLog($"[GumUI] Country: {_countryCmb.Items[idx]}", Color.Yellow))
                .Build();

            _themeCmb = UI.CreateComboBox("Theme")
                .WithPosition(5, 95)
                .WithItems("Dark", "Light", "Solarized", "Monokai")
                .WithSelectedIndex(0)
                .WithOnSelectionChanged(idx => UpdateStatus($"Theme: {_themeCmb.Items[idx]}"))
                .Build();

            _agreeTos = UI.CreateCheckBox("TOS")
                .WithText("I agree to the Terms")
                .WithPosition(5, 125)
                .WithIsChecked(false)
                .WithOnCheckedChanged(v => SendLog($"[GumUI] TOS agreed: {v}", Color.White))
                .Build();

            _newsletter = UI.CreateCheckBox("Newsletter")
                .WithText("Subscribe to newsletter")
                .WithPosition(5, 150)
                .WithIsChecked(true)
                .WithOnCheckedChanged(v => SendLog($"[GumUI] Newsletter: {v}", Color.White))
                .Build();

            _formPanel.AddChild(_usernameTxt);
            _formPanel.AddChild(_emailTxt);
            _formPanel.AddChild(_countryCmb);
            _formPanel.AddChild(_themeCmb);
            _formPanel.AddChild(_agreeTos);
            _formPanel.AddChild(_newsletter);

            // --- Result Panel ---
            _resultPanel = UI.CreatePanel("Results")
                .WithPosition(5, 300)
                .Build();

            _resultLabel = UI.CreateLabel("result")
                .WithText("Results: Waiting for interactions...")
                .WithPosition(5, 5)
                .WithColor(Color.Yellow)
                .Build();

            _resultPanel.AddChild(_resultLabel);

            // Assemble main window
            _mainWindow.AddChild(_headerPanel);
            _mainWindow.AddChild(_controlsPanel);
            _mainWindow.AddChild(_formPanel);
            _mainWindow.AddChild(_resultPanel);

            UI.AddToRoot(_mainWindow);
        }

        private void CreateConfigWindow()
        {
            _configWindow = UI.CreateWindow("GumUI Config")
                .WithTitle("Configuration")
                .WithPosition(450, 20)
                .WithIsOpen(true)
                .Build();

            // --- Audio Panel ---
            _audioPanel = UI.CreatePanel("Audio")
                .WithPosition(5, 5)
                .Build();

            _audioTitleLabel = UI.CreateLabel("audiotitle")
                .WithText("Audio Settings")
                .WithPosition(5, 5)
                .WithColor(Color.Gold)
                .Build();

            _masterVolSlider = UI.CreateSlider("MasterVol")
                .WithPosition(5, 30)
                .WithRange(0, 100)
                .WithValue(_masterVol)
                .WithOnValueChanged(v => { _masterVol = v; UpdateAudioLabel(); })
                .Build();

            _musicVolSlider = UI.CreateSlider("MusicVol")
                .WithPosition(5, 55)
                .WithRange(0, 100)
                .WithValue(_musicVol)
                .WithOnValueChanged(v => { _musicVol = v; UpdateAudioLabel(); })
                .Build();

            _sfxVolSlider = UI.CreateSlider("SFXVol")
                .WithPosition(5, 80)
                .WithRange(0, 100)
                .WithValue(_sfxVol)
                .WithOnValueChanged(v => { _sfxVol = v; UpdateAudioLabel(); })
                .Build();

            _muteChk = UI.CreateCheckBox("Mute")
                .WithText("Mute All")
                .WithPosition(5, 105)
                .WithIsChecked(false)
                .WithOnCheckedChanged(v => SendLog($"[GumUI] Mute: {v}", Color.Red))
                .Build();

            _audioPanel.AddChild(_audioTitleLabel);
            _audioPanel.AddChild(_masterVolSlider);
            _audioPanel.AddChild(_musicVolSlider);
            _audioPanel.AddChild(_sfxVolSlider);
            _audioPanel.AddChild(_muteChk);

            // --- Graphics Panel ---
            _graphicsPanel = UI.CreatePanel("Graphics")
                .WithPosition(5, 145)
                .Build();

            _gfxTitleLabel = UI.CreateLabel("gfxtitle")
                .WithText("Graphics Settings")
                .WithPosition(5, 5)
                .WithColor(Color.Gold)
                .Build();

            _fovSlider = UI.CreateSlider("FOV")
                .WithPosition(5, 30)
                .WithRange(60, 120)
                .WithValue(_fov)
                .WithOnValueChanged(v => { _fov = v; SendLog($"[GumUI] FOV: {v:F0}", Color.White); })
                .Build();

            _qualityCmb = UI.CreateComboBox("Quality")
                .WithPosition(5, 55)
                .WithItems("Low", "Medium", "High", "Ultra")
                .WithSelectedIndex(2)
                .WithOnSelectionChanged(idx => SendLog($"[GumUI] Quality: {_qualityCmb.Items[idx]}", Color.Orange))
                .Build();

            _antiAliasingChk = UI.CreateCheckBox("AA")
                .WithText("Anti-Aliasing")
                .WithPosition(5, 85)
                .WithIsChecked(true)
                .WithOnCheckedChanged(v => SendLog($"[GumUI] AA: {v}", Color.White))
                .Build();

            _shadowsChk = UI.CreateCheckBox("Shadows")
                .WithText("Shadows")
                .WithPosition(5, 110)
                .WithIsChecked(true)
                .WithOnCheckedChanged(v => SendLog($"[GumUI] Shadows: {v}", Color.White))
                .Build();

            _bloomChk = UI.CreateCheckBox("Bloom")
                .WithText("Bloom")
                .WithPosition(5, 135)
                .WithIsChecked(false)
                .WithOnCheckedChanged(v => SendLog($"[GumUI] Bloom: {v}", Color.White))
                .Build();

            _graphicsPanel.AddChild(_gfxTitleLabel);
            _graphicsPanel.AddChild(_fovSlider);
            _graphicsPanel.AddChild(_qualityCmb);
            _graphicsPanel.AddChild(_antiAliasingChk);
            _graphicsPanel.AddChild(_shadowsChk);
            _graphicsPanel.AddChild(_bloomChk);

            // Assemble config window
            _configWindow.AddChild(_audioPanel);
            _configWindow.AddChild(_graphicsPanel);

            UI.AddToRoot(_configWindow);
        }

        #region Event Handlers

        private void OnClickBtn()
        {
            _clickCount++;
            _clickCountLabel.Text = $"Clicks: {_clickCount}";
            UpdateStatus($"Button clicked! Count: {_clickCount}");
            SendLog($"[GumUI] Click #{_clickCount}", Color.Cyan);
        }

        private void OnResetBtn()
        {
            _clickCount = 0;
            _clickCountLabel.Text = "Clicks: 0";
            UpdateStatus("Counter reset");
            SendLog("[GumUI] Counter reset", Color.Yellow);
        }

        private void UpdateStatus(string message)
        {
            _statusLabel.Text = $"Status: {message}";
            _resultLabel.Text = $"Results: {message}";
        }

        private void UpdateAudioLabel()
        {
            _audioTitleLabel.Text = $"Audio - M:{_masterVol:F0} Mus:{_musicVol:F0} SFX:{_sfxVol:F0}";
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            float fps = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;
            _fpsLabel.Text = $"FPS: {fps:F0}";
            base.Update(gameTime);
        }

        public override void Draw2D(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw2D(spriteBatch, gameTime);
        }

        public override void Cleanup()
        {
            UI.Clear();
            SendLog("[UITestScreen] Cleanup completed", Color.Gray);
            base.Cleanup();
        }
    }
}
