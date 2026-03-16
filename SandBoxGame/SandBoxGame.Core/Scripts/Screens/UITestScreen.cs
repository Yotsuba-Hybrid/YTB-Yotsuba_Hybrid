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
    /// Script de prueba que demuestra todos los controles UI del sistema.Forms.
    /// <para>Test script demonstrating all UI controls from the Forms system.</para>
    /// </summary>
    [Script(UISystem = UILibrary.GumUI)]
    public class UITestScreen : BaseScript
    {
        private IWindow _mainWindow;
        private IPanel _controlsPanel;
        private IButton _button;
        private ILabel _label;
        private ICheckBox _checkBox;
        private ITextBox _textBox;
        private ISlider _slider;
        private IComboBox _comboBox;
        private IPanel _resultPanel;
        private ILabel _resultLabel;

        private int _clickCount;
        private bool _checkBoxState;
        private float _sliderValue;
        private string _textBoxValue = "";

        public override void Initialize()
        {
            YTBGlobalState.EngineBackground = new Color(30, 30, 40, 255);

            CreateMainWindow();
            CreateControlsPanel();
            CreateResultPanel();

            base.Initialize();
        }

        private void CreateMainWindow()
        {
            _mainWindow = UI.CreateWindow("UI Test - Forms System")
                .WithTitle("Yotsuba Engine - UI Test")
                .WithPosition(50, 50)
                .WithIsOpen(true)
                .Build();

            UI.AddToRoot(_mainWindow);

            SendLog("[UITestScreen] Main window created", Color.Green);
        }

        private void CreateControlsPanel()
        {
            _controlsPanel = UI.CreatePanel()
                .WithPosition(10, 40)
                .Build();

            _mainWindow.AddChild(_controlsPanel);

            CreateButton();
            CreateLabel();
            CreateCheckBox();
            CreateTextBox();
            CreateSlider();
            CreateComboBox();
        }

        private void CreateButton()
        {
            _button = UI.CreateButton("Click Me!")
                .WithText("Click Me!")
                .WithPosition(10, 10)
                .WithOnClick(OnButtonClicked)
                .Build();

            _controlsPanel.AddChild(_button);
        }

        private void CreateLabel()
        {
            _label = UI.CreateLabel("This is a label")
                .WithText("Label: ClickCounter = 0")
                .WithPosition(10, 50)
                .WithColor(Color.White)
                .Build();

            _controlsPanel.AddChild(_label);
        }

        private void CreateCheckBox()
        {
            _checkBox = UI.CreateCheckBox("Enable Feature")
                .WithText("Enable Feature")
                .WithPosition(10, 90)
                .WithIsChecked(false)
                .WithOnCheckedChanged(OnCheckBoxChanged)
                .Build();

            _controlsPanel.AddChild(_checkBox);
        }

        private void CreateTextBox()
        {
            _textBox = UI.CreateTextBox("Type here...")
                .WithText("")
                .WithPosition(10, 130)
                .WithOnValueChanged(OnTextBoxChanged)
                .WithOnSubmit(OnTextBoxSubmit)
                .Build();

            _controlsPanel.AddChild(_textBox);
        }

        private void CreateSlider()
        {
            _slider = UI.CreateSlider("MySlider")
                .WithPosition(10, 170)
                .WithRange(0, 100)
                .WithValue(50)
                .WithDirection(SliderDirection.Horizontal)
                .WithOnValueChanged(OnSliderChanged)
                .Build();

            _controlsPanel.AddChild(_slider);
        }

        private void CreateComboBox()
        {
            _comboBox = UI.CreateComboBox()
                .WithPosition(10, 210)
                .WithItems("Option 1", "Option 2", "Option 3", "Option 4")
                .WithSelectedIndex(0)
                .WithOnSelectionChanged(OnComboBoxChanged)
                .Build();

            _controlsPanel.AddChild(_comboBox);
        }

        private void CreateResultPanel()
        {
            _resultPanel = UI.CreatePanel()
                .WithPosition(10, 250)
                .Build();

            _resultLabel = UI.CreateLabel("Results will appear here")
                .WithText("Results: Waiting for interactions...")
                .WithColor(Color.Yellow)
                .Build();

            _resultPanel.AddChild(_resultLabel);
            _mainWindow.AddChild(_resultPanel);
        }

        #region Event Handlers

        private void OnButtonClicked()
        {
            _clickCount++;
            UpdateResult($"Button clicked! Count: {_clickCount}");
            SendLog($"[UITestScreen] Button clicked {_clickCount} times", Color.Cyan);
        }

        private void OnCheckBoxChanged(bool isChecked)
        {
            _checkBoxState = isChecked;
            UpdateResult($"CheckBox {(isChecked ? "checked" : "unchecked")}");
            SendLog($"[UITestScreen] CheckBox state: {isChecked}", Color.Yellow);
        }

        private void OnTextBoxChanged(string text)
        {
            _textBoxValue = text;
        }

        private void OnTextBoxSubmit(string text)
        {
            UpdateResult($"TextBox submitted: '{text}'");
            SendLog($"[UITestScreen] TextBox submitted: {text}", Color.Magenta);
        }

        private void OnSliderChanged(float value)
        {
            _sliderValue = value;
            UpdateResult($"Slider value: {value:F1}");
        }

        private void OnComboBoxChanged(int selectedIndex)
        {
            var items = _comboBox.Items;
            var selectedText = selectedIndex >= 0 && selectedIndex < items.Count 
                ? items[selectedIndex] 
                : "None";
            UpdateResult($"ComboBox selected: {selectedText} (index: {selectedIndex})");
            SendLog($"[UITestScreen] ComboBox selection: {selectedText}", Color.Orange);
        }

        private void UpdateResult(string message)
        {
            _resultLabel.Text = $"Results: {message}";
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
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