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

        public Button()
        {
            ImGuIControl = () => ImUi.Button(Text);
            GumControl = new() { Text = Text };
            MyraControl = new(Text) { Tag = Text, Tooltip = Text };

            GumControl.Click += GumControl_Click;
        }

        private void GumControl_Click(object sender, EventArgs e)
        {
            OnClick?.Invoke();
        }

        public string Text { get; set; }
        public Color Color { get; set; }
        public Vector2 Position { get; set; }

        public event Action OnClick;

        private MyraUi.Button MyraControl { get; set; }
        private GumUi.Button GumControl { get; set; }

        private Func<bool> ImGuIControl;
        void IGum.DrawGumUI()
        {
            GumControl.UpdateState();
        }

        void IImGui.DrawImGuI()
        {
            if (ImGuIControl.Invoke())
            {
                OnClick?.Invoke();
            }
        }

        void IMyra.DrawMyra()
        {
            if (MyraControl.IsPressed)
            {
                OnClick?.Invoke();
            }
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
            return ImGuIControl;
        }
    }
}
