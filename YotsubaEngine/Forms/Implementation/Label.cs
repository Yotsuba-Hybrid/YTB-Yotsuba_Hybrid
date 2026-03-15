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
            ImGuIControl = () => { ImUi.Text(Text); return Text; };
            GumControl = new() { Text = Text };
            MyraControl = new(Text) { Tag = Text, Tooltip = Text };
        }

        public string Text { get; set; }
        public Color Color { get; set; }
        private MyraUi.Label MyraControl { get; set; }
        private GumUi.Label GumControl { get; set; }

        private Func<string> ImGuIControl;
        public Vector2 Position { get ; set; }


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
