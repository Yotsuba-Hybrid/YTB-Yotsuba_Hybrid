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
                    _text = value ?? string.Empty;
                    UpdateBuffers();
                    GumControl.Text = _text;
                    MyraControl.Text = _text;
                    OnValueChanged?.Invoke(_text);
                }
            }
        }

        public Color Color { get; set; }
        public Vector2 Position { get; set; }

        public event Action<string> OnValueChanged;
        public event Action<string> OnSubmit;

        private MyraUi.TextBox MyraControl { get; set; }
        private GumUi.TextBox GumControl { get; set; }

        public TextBox()
        {
            _imguiBuffer = new byte[MaxBufferLength];
            UpdateBuffers();

            MyraControl = new() { Tag = _text, Tooltip = _text };
            GumControl = new() { Text = _text, Width = 200 };

            MyraControl.TextChanged += (_, _) =>
            {
                _text = MyraControl.Text ?? string.Empty;
                UpdateBuffers();
                OnValueChanged?.Invoke(_text);
            };

            GumControl.TextChanged += (_, _) =>
            {
                _text = GumControl.Text ?? string.Empty;
                UpdateBuffers();
                OnValueChanged?.Invoke(_text);
            };
        }

        private void UpdateBuffers()
        {
            var bytes = Encoding.UTF8.GetBytes(_text ?? string.Empty);
            Array.Clear(_imguiBuffer, 0, _imguiBuffer.Length);
            Array.Copy(bytes, _imguiBuffer, Math.Min(bytes.Length, _imguiBuffer.Length - 1));
        }

        void IGum.DrawGumUI()
        {
            GumControl.X = Position.X;
            GumControl.Y = Position.Y;
        }

        void IImGui.DrawImGuI()
        {
            string label = Text ?? string.Empty;
            if (ImUi.InputText(label, _imguiBuffer, MaxBufferLength))
            {
                string currentText = Encoding.UTF8.GetString(_imguiBuffer).TrimEnd('\0');
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
            return () => _text ?? string.Empty;
        }
    }
}