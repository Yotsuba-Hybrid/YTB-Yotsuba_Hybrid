using Microsoft.Xna.Framework;
using System;
using YotsubaEngine.Forms.Contract;

namespace YotsubaEngine.Forms.Implementation
{
    public class Button : IButton
    {
        public string Text { get; set; }
        public Color Color { get; set; }
        public Vector2 Position { get; set; }

        public event Action OnClick;
    }
}
