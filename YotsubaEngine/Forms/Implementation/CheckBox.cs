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
                _text = value;
                MyraControl.Text = value;
                GumControl.Text = value;
            }
        }

        public Color Color { get; set; }
        public Vector2 Position { get; set; }

        public event Action<bool> OnCheckedChanged;

        private MyraUi.CheckBox MyraControl { get; set; }
        private GumUi.CheckBox GumControl { get; set; }
        private Func<bool, bool> ImGuIControl { get; set; }

        public CheckBox()
        {
            MyraControl = new() { Tag = Text };
            GumControl = new() { Text = Text };

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

            ImGuIControl = (value) =>
            {
                bool changed = ImUi.Checkbox(Text, ref _isChecked);
                if (changed)
                {
                    OnCheckedChanged?.Invoke(_isChecked);
                }
                return changed;
            };
        }

        void IGum.DrawGumUI()
        {
            GumControl.UpdateState();
        }

        void IImGui.DrawImGuI()
        {
            ImGuIControl.Invoke(_isChecked);
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
            return ImGuIControl;
        }
    }
}