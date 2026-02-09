using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.Graphics.ImGuiNet
{

    /// <summary>
    /// Author("Package MonoGame.ImGuiNet", "09/2025"),
    /// Author("https://contrib.rocks/image?repo=Mezo-hx/MonoGame.ImGuiNet", "09/2025"),
    /// Author("https://github.com/Mezo-hx/MonoGame.ImGuiNet/graphs/contributors", "09/2025")
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    public static class DrawVertDeclaration
    {
        public static readonly VertexDeclaration Declaration;

        public static readonly int Size;

        static DrawVertDeclaration()
        {
            unsafe { Size = sizeof(ImDrawVert); }

            Declaration = new VertexDeclaration(
                Size,

                // Position
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),

                // UV
                new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),

                // Color
                new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0)
            );
        }
    }
}