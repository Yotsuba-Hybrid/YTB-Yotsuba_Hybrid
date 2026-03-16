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
    public class Label : ILabel, IMyraLabel, IGumLabel, IImGuiLabel
    {
        public IContainer Parent { get; set; }

        public Label()
        {
            ImGuIControl = () => { ImUi.Text(Text ?? string.Empty); return Text ?? string.Empty; };
            GumControl = new() { Text = Text ?? string.Empty, Width = 200 };
            MyraControl = new(Text ?? string.Empty) { Tag = Text ?? string.Empty };
        }

        private string _text = string.Empty;
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
        private MyraUi.Label MyraControl { get; set; }
        private GumUi.Label GumControl { get; set; }

        private Func<string> ImGuIControl;
        public Vector2 Position { get; set; }

        void IGum.DrawGumUI()
        {
            GumControl.X = Position.X;
            GumControl.Y = Position.Y;
        }

        void IImGui.DrawImGuI()
        {
            ImGuIControl?.Invoke();
        }

        void IMyra.DrawMyra()
        {
        }

        MyraUi.Label IMyraLabel.GetMyraLabel()
        {
            return MyraControl;
        }

        GumUi.Label IGumLabel.GetGumLabel()
        {
            return GumControl;
        }

        Func<string> IImGuiLabel.GetImGuiLabelAsFunc()
        {
            return ImGuIControl;
        }
    }
}