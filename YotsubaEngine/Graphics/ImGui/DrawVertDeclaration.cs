using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.Graphics.ImGuiNet
{

    /// <summary>
    /// Declaración de vértices para ImGui.
    /// <para>Vertex declaration for ImGui.</para>
    /// </summary>
    /// <remarks>
    /// Autores originales del paquete MonoGame.ImGuiNet.
    /// <para>Original authors of the MonoGame.ImGuiNet package.</para>
    /// Author("Package MonoGame.ImGuiNet", "09/2025"),
    /// Author("https://contrib.rocks/image?repo=Mezo-hx/MonoGame.ImGuiNet", "09/2025"),
    /// Author("https://github.com/Mezo-hx/MonoGame.ImGuiNet/graphs/contributors", "09/2025")
    /// </remarks>
    public static class DrawVertDeclaration
    {
        /// <summary>
        /// Declaración de vértices para ImGui.
        /// <para>Vertex declaration used by ImGui.</para>
        /// </summary>
        public static readonly VertexDeclaration Declaration;

        /// <summary>
        /// Tamaño en bytes de un vértice ImGui.
        /// <para>Size in bytes of an ImGui vertex.</para>
        /// </summary>
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
