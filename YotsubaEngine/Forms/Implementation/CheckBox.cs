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
    public class CheckBox : ICheckBox, IMyraCheckBox, IGumCheckBox, IImGuiCheckBox
    {
        public IContainer Parent { get; set; }
        private bool _isChecked;
        private string _text = string.Empty;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    GumControl.IsChecked = value;
                    OnCheckedChanged?.Invoke(value);
                }
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value ?? string.Empty;
                MyraControl.Text = _text;
                GumControl.Text = _text;
            }
        }

        public Color Color { get; set; }
        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                MyraControl.Left = (int)value.X;
                MyraControl.Top = (int)value.Y;
            }
        }

        public event Action<bool> OnCheckedChanged;

        private MyraUi.CheckBox MyraControl { get; set; }
        private GumUi.CheckBox GumControl { get; set; }

        public CheckBox()
        {
            MyraControl = new() { Tag = _text };
            GumControl = new() { Text = _text };

            GumControl.Checked += (_, _) =>
            {
                _isChecked = true;
                OnCheckedChanged?.Invoke(true);
            };
            GumControl.Unchecked += (_, _) =>
            {
                _isChecked = false;
                OnCheckedChanged?.Invoke(false);
            };
        }

        void IGum.DrawGumUI()
        {
            GumControl.X = Position.X;
            GumControl.Y = Position.Y;
        }

        void IImGui.DrawImGuI()
        {
            string label = _text ?? string.Empty;
            bool changed = ImUi.Checkbox(label, ref _isChecked);
            if (changed)
            {
                OnCheckedChanged?.Invoke(_isChecked);
            }
        }

        void IMyra.DrawMyra()
        {
        }

        MyraUi.CheckBox IMyraCheckBox.GetMyraCheckBox()
        {
            return MyraControl;
        }

        GumUi.CheckBox IGumCheckBox.GetGumCheckBox()
        {
            return GumControl;
        }

        Func<bool, bool> IImGuiCheckBox.GetImGuiCheckBoxAsFunc()
        {
            return (_) =>
            {
                string label = _text ?? string.Empty;
                bool changed = ImUi.Checkbox(label, ref _isChecked);
                return changed;
            };
        }
    }
}