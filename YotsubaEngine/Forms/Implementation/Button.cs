using Microsoft.Xna.Framework;
using System;
using YotsubaEngine.Forms.Contract;
using YotsubaEngine.Forms.Contract.GumUI;
using YotsubaEngine.Forms.Contract.ImGUI;
using YotsubaEngine.Forms.Contract.Myra;
using GumUi = Gum.Forms.Controls;
using ImUi = ImGuiNET.ImGui;
using MyraUi = Myra.Graphics2D.UI;

namespace YotsubaEngine.Forms.Implementation
{
    public class Button : IButton, IMyraButton, IGumButton, IImGuiButton
    {
        public IContainer Parent { get; set; }

        private string _text = string.Empty;
        private Vector2 _position;
        private MyraUi.Label _myraLabel;

        public Button()
        {
            GumControl = new GumUi.Button
            {
                Width = 100,
                Height = 30
            };

            _myraLabel = new MyraUi.Label { Text = _text };
            MyraControl = new MyraUi.Button { Content = _myraLabel };

            GumControl.Click += GumControl_Click;
        }

        private void GumControl_Click(object sender, EventArgs e)
        {
            OnClick?.Invoke();
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                GumControl.Text = value;
                _myraLabel.Text = value;
            }
        }

        public Color Color { get; set; }

        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                GumControl.X = value.X;
                GumControl.Y = value.Y;
                MyraControl.Left = (int)value.X;
                MyraControl.Top = (int)value.Y;
            }
        }

        public event Action OnClick;

        private MyraUi.Button MyraControl { get; set; }
        private GumUi.Button GumControl { get; set; }

        private Func<bool> _imguiControl;

        void IGum.DrawGumUI()
        {
            GumControl.X = _position.X;
            GumControl.Y = _position.Y;
        }

        void IImGui.DrawImGuI()
        {
            if (ImUi.Button(_text))
            {
                OnClick?.Invoke();
            }
        }

        void IMyra.DrawMyra()
        {
        }

        MyraUi.Button IMyraButton.GetMyraButton()
        {
            return MyraControl;
        }

        GumUi.Button IGumButton.GetGumButton()
        {
            return GumControl;
        }

        Func<bool> IImGuiButton.GetImGuiButtonAsFunc()
        {
            return () => ImUi.Button(Text);
        }
    }
}