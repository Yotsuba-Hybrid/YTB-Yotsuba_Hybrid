using Microsoft.Xna.Framework;
using YotsubaEngine.Forms.Contract;
using MyraUi = Myra.Graphics2D.UI;
using ImUi = ImGuiNET.ImGui;
using GumUi = Gum.Forms.Controls;

namespace YotsubaEngine.Forms.Implementation
{
    public class Label : ILabel
    {
        public string Text { get; set; }
        public Color Color { get; set; }
        private MyraUi.Button MyraControl { get; set; }
        private GumUi.Button GumControl { get; set; }
        private void ImGuIControl() => ImUi.Text(Text);

        public Vector2 Position { get ; set; }


        public void DrawGumUI()
        {
            
        }

        public void DrawImGuI()
        {
            ImGuIControl();
        }

        public void DrawMyra()
        {
            
        }
    }
}
