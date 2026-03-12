using Microsoft.Xna.Framework;
namespace YotsubaEngine.Forms.Contract.ChildElement
{
    public interface Forms
    {
        public string Text { get; set; }

        public Color Color { get; set; }
        public Vector2 Position { get; set; }
    }
}
