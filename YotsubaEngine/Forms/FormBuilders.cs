using System;
using Microsoft.Xna.Framework;
using YotsubaEngine.Forms.Contract;
using YotsubaEngine.Forms.Implementation;

namespace YotsubaEngine.Forms
{
    public class ButtonBuilder
    {
        private readonly Button _button;
        private readonly UILibrary _library;

        public ButtonBuilder(UILibrary library, string text)
        {
            _library = library;
            _button = new Button { Text = text ?? string.Empty };
        }

        public ButtonBuilder WithText(string text)
        {
            _button.Text = text ?? string.Empty;
            return this;
        }

        public ButtonBuilder WithPosition(float x, float y)
        {
            _button.Position = new Vector2(x, y);
            return this;
        }

        public ButtonBuilder WithPosition(Vector2 position)
        {
            _button.Position = position;
            return this;
        }

        public ButtonBuilder WithColor(Color color)
        {
            _button.Color = color;
            return this;
        }

        public ButtonBuilder WithOnClick(Action handler)
        {
            _button.OnClick += handler;
            return this;
        }

        public Button Build()
        {
            FormsManager.Instance.RegisterControl(_button);
            return _button;
        }

        public TInterface Cast<TInterface>() where TInterface : class, IForm
        {
            return _button as TInterface;
        }
    }

    public class LabelBuilder
    {
        private readonly Label _label;
        private readonly UILibrary _library;

        public LabelBuilder(UILibrary library, string text)
        {
            _library = library;
            _label = new Label { Text = text ?? string.Empty };
        }

        public LabelBuilder WithText(string text)
        {
            _label.Text = text ?? string.Empty;
            return this;
        }

        public LabelBuilder WithPosition(float x, float y)
        {
            _label.Position = new Vector2(x, y);
            return this;
        }

        public LabelBuilder WithPosition(Vector2 position)
        {
            _label.Position = position;
            return this;
        }

        public LabelBuilder WithColor(Color color)
        {
            _label.Color = color;
            return this;
        }

        public Label Build()
        {
            FormsManager.Instance.RegisterControl(_label);
            return _label;
        }

        public TInterface Cast<TInterface>() where TInterface : class, IForm
        {
            return _label as TInterface;
        }
    }

    public class CheckBoxBuilder
    {
        private readonly CheckBox _checkBox;
        private readonly UILibrary _library;

        public CheckBoxBuilder(UILibrary library, string text, bool isChecked = false)
        {
            _library = library;
            _checkBox = new CheckBox { Text = text ?? string.Empty, IsChecked = isChecked };
        }

        public CheckBoxBuilder WithText(string text)
        {
            _checkBox.Text = text ?? string.Empty;
            return this;
        }

        public CheckBoxBuilder WithPosition(float x, float y)
        {
            _checkBox.Position = new Vector2(x, y);
            return this;
        }

        public CheckBoxBuilder WithPosition(Vector2 position)
        {
            _checkBox.Position = position;
            return this;
        }

        public CheckBoxBuilder WithColor(Color color)
        {
            _checkBox.Color = color;
            return this;
        }

        public CheckBoxBuilder WithIsChecked(bool isChecked)
        {
            _checkBox.IsChecked = isChecked;
            return this;
        }

        public CheckBoxBuilder WithOnCheckedChanged(Action<bool> handler)
        {
            _checkBox.OnCheckedChanged += handler;
            return this;
        }

        public CheckBox Build()
        {
            FormsManager.Instance.RegisterControl(_checkBox);
            return _checkBox;
        }

        public TInterface Cast<TInterface>() where TInterface : class, IForm
        {
            return _checkBox as TInterface;
        }
    }

    public class TextBoxBuilder
    {
        private readonly TextBox _textBox;
        private readonly UILibrary _library;

        public TextBoxBuilder(UILibrary library, string text = "")
        {
            _library = library;
            _textBox = new TextBox { Text = text ?? string.Empty };
        }

        public TextBoxBuilder WithText(string text)
        {
            _textBox.Text = text ?? string.Empty;
            return this;
        }

        public TextBoxBuilder WithPosition(float x, float y)
        {
            _textBox.Position = new Vector2(x, y);
            return this;
        }

        public TextBoxBuilder WithPosition(Vector2 position)
        {
            _textBox.Position = position;
            return this;
        }

        public TextBoxBuilder WithColor(Color color)
        {
            _textBox.Color = color;
            return this;
        }

        public TextBoxBuilder WithOnValueChanged(Action<string> handler)
        {
            _textBox.OnValueChanged += handler;
            return this;
        }

        public TextBoxBuilder WithOnSubmit(Action<string> handler)
        {
            _textBox.OnSubmit += handler;
            return this;
        }

        public TextBox Build()
        {
            FormsManager.Instance.RegisterControl(_textBox);
            return _textBox;
        }

        public TInterface Cast<TInterface>() where TInterface : class, IForm
        {
            return _textBox as TInterface;
        }
    }

    public class SliderBuilder
    {
        private readonly Slider _slider;
        private readonly UILibrary _library;

        public SliderBuilder(UILibrary library, string text)
        {
            _library = library;
            _slider = new Slider { Text = text ?? string.Empty };
        }

        public SliderBuilder WithText(string text)
        {
            _slider.Text = text ?? string.Empty;
            return this;
        }

        public SliderBuilder WithValue(float value)
        {
            _slider.Value = value;
            return this;
        }

        public SliderBuilder WithRange(float min, float max)
        {
            _slider.Min = min;
            _slider.Max = max;
            return this;
        }

        public SliderBuilder WithDirection(SliderDirection direction)
        {
            _slider.Direction = direction;
            return this;
        }

        public SliderBuilder WithPosition(float x, float y)
        {
            _slider.Position = new Vector2(x, y);
            return this;
        }

        public SliderBuilder WithPosition(Vector2 position)
        {
            _slider.Position = position;
            return this;
        }

        public SliderBuilder WithColor(Color color)
        {
            _slider.Color = color;
            return this;
        }

        public SliderBuilder WithOnValueChanged(Action<float> handler)
        {
            _slider.OnValueChanged += handler;
            return this;
        }

        public Slider Build()
        {
            FormsManager.Instance.RegisterControl(_slider);
            return _slider;
        }

        public TInterface Cast<TInterface>() where TInterface : class, IForm
        {
            return _slider as TInterface;
        }
    }

    public class ComboBoxBuilder
    {
        private readonly ComboBox _comboBox;
        private readonly UILibrary _library;

        public ComboBoxBuilder(UILibrary library, string text = "ComboBox")
        {
            _library = library;
            _comboBox = new ComboBox { Text = text ?? string.Empty };
        }

        public ComboBoxBuilder WithText(string text)
        {
            _comboBox.Text = text ?? string.Empty;
            return this;
        }

        public ComboBoxBuilder WithPosition(float x, float y)
        {
            _comboBox.Position = new Vector2(x, y);
            return this;
        }

        public ComboBoxBuilder WithPosition(Vector2 position)
        {
            _comboBox.Position = position;
            return this;
        }

        public ComboBoxBuilder WithColor(Color color)
        {
            _comboBox.Color = color;
            return this;
        }

        public ComboBoxBuilder WithItems(params string[] items)
        {
            foreach (var item in items)
            {
                _comboBox.AddItem(item);
            }
            return this;
        }

        public ComboBoxBuilder WithSelectedIndex(int index)
        {
            _comboBox.SelectedIndex = index;
            return this;
        }

        public ComboBoxBuilder WithOnSelectionChanged(Action<int> handler)
        {
            _comboBox.OnSelectionChanged += handler;
            return this;
        }

        public ComboBox Build()
        {
            FormsManager.Instance.RegisterControl(_comboBox);
            return _comboBox;
        }

        public TInterface Cast<TInterface>() where TInterface : class, IForm
        {
            return _comboBox as TInterface;
        }
    }

    public class WindowBuilder
    {
        private readonly Window _window;
        private readonly UILibrary _library;

        public WindowBuilder(UILibrary library, string title)
        {
            _library = library;
            _window = new Window { Text = title ?? "Window" };
        }

        public WindowBuilder WithTitle(string title)
        {
            _window.Text = title ?? "Window";
            return this;
        }

        public WindowBuilder WithPosition(float x, float y)
        {
            _window.Position = new Vector2(x, y);
            return this;
        }

        public WindowBuilder WithPosition(Vector2 position)
        {
            _window.Position = position;
            return this;
        }

        public WindowBuilder WithColor(Color color)
        {
            _window.Color = color;
            return this;
        }

        public WindowBuilder WithIsOpen(bool isOpen)
        {
            _window.IsOpen = isOpen;
            return this;
        }

        public WindowBuilder WithOnClosed(Action handler)
        {
            _window.OnClosed += handler;
            return this;
        }

        public WindowBuilder WithChild(IForm child)
        {
            _window.AddChild(child);
            return this;
        }

        public Window Build()
        {
            FormsManager.Instance.RegisterControl(_window);
            return _window;
        }

        public TInterface Cast<TInterface>() where TInterface : class, IForm
        {
            return _window as TInterface;
        }
    }

    public class PanelBuilder
    {
        private readonly Panel _panel;
        private readonly UILibrary _library;

        public PanelBuilder(UILibrary library, string text = "Panel")
        {
            _library = library;
            _panel = new Panel { Text = text ?? string.Empty };
        }

        public PanelBuilder WithText(string text)
        {
            _panel.Text = text ?? string.Empty;
            return this;
        }

        public PanelBuilder WithPosition(float x, float y)
        {
            _panel.Position = new Vector2(x, y);
            return this;
        }

        public PanelBuilder WithPosition(Vector2 position)
        {
            _panel.Position = position;
            return this;
        }

        public PanelBuilder WithColor(Color color)
        {
            _panel.Color = color;
            return this;
        }

        public PanelBuilder WithChild(IForm child)
        {
            _panel.AddChild(child);
            return this;
        }

        public Panel Build()
        {
            FormsManager.Instance.RegisterControl(_panel);
            return _panel;
        }

        public TInterface Cast<TInterface>() where TInterface : class, IForm
        {
            return _panel as TInterface;
        }
    }
}