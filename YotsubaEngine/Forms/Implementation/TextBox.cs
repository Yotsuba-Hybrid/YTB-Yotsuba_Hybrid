using Microsoft.Xna.Framework;
using System;
using System.Text;
using YotsubaEngine.Forms.Contract;
using YotsubaEngine.Forms.Contract.GumUI;
using YotsubaEngine.Forms.Contract.ImGUI;
using YotsubaEngine.Forms.Contract.Myra;
using GumUi = Gum.Forms.Controls;
using ImUi = ImGuiNET.ImGui;
using MyraUi = Myra.Graphics2D.UI;

namespace YotsubaEngine.Forms.Implementation
{
    public class TextBox : ITextBox, IMyraTextBox, IGumTextBox, IImGuiTextBox
    {
        public IContainer Parent { get; set; }
        private string _text = string.Empty;
        private byte[] _imguiBuffer;
        private const int MaxBufferLength = 256;

        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    UpdateBuffers();
                    GumControl.Text = value;
                    MyraControl.Text = value;
                    OnValueChanged?.Invoke(value);
                }
            }
        }

        public Color Color { get; set; }
        public Vector2 Position { get; set; }

        public event Action<string> OnValueChanged;
        public event Action<string> OnSubmit;

        private MyraUi.TextBox MyraControl { get; set; }
        private GumUi.TextBox GumControl { get; set; }
        private Func<string> ImGuIControl { get; set; }

        public TextBox()
        {
            _imguiBuffer = new byte[MaxBufferLength];
            UpdateBuffers();

            MyraControl = new() { Tag = Text, Tooltip = Text };
            GumControl = new() { Text = Text, Width = 200 };

            MyraControl.TextChanged += (_, _) =>
            {
                _text = MyraControl.Text;
                UpdateBuffers();
                OnValueChanged?.Invoke(_text);
            };

            GumControl.TextChanged += (_, _) =>
            {
                _text = GumControl.Text;
                UpdateBuffers();
                OnValueChanged?.Invoke(_text);
            };

            ImGuIControl = () =>
            {
                string currentText = _text;
                if (ImUi.InputText(Text, _imguiBuffer, MaxBufferLength))
                {
                    currentText = Encoding.UTF8.GetString(_imguiBuffer).TrimEnd('\0');
                    if (currentText != _text)
                    {
                        _text = currentText;
                        OnValueChanged?.Invoke(_text);
                    }
                }
                if (ImUi.IsItemDeactivatedAfterEdit())
                {
                    OnSubmit?.Invoke(_text);
                }
                return currentText;
            };
        }

        private void UpdateBuffers()
        {
            var bytes = Encoding.UTF8.GetBytes(_text);
            Array.Clear(_imguiBuffer, 0, _imguiBuffer.Length);
            Array.Copy(bytes, _imguiBuffer, Math.Min(bytes.Length, _imguiBuffer.Length - 1));
        }

        void IGum.DrawGumUI()
        {
            GumControl.X = Position.X;
            GumControl.Y = Position.Y;
            GumControl.UpdateState();
        }

        void IImGui.DrawImGuI()
        {
            ImGuIControl?.Invoke();
        }

        void IMyra.DrawMyra()
        {
        }

        MyraUi.TextBox IMyraTextBox.GetMyraTextBox()
        {
            return MyraControl;
        }

        GumUi.TextBox IGumTextBox.GetGumTextBox()
        {
            return GumControl;
        }

        Func<string> IImGuiTextBox.GetImGuiTextBoxAsFunc()
        {
            return ImGuIControl;
        }
    }
}