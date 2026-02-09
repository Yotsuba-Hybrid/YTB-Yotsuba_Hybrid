using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.Graphics
{
    /// <summary>
    /// Represents a texture region to render from a larger texture.
    /// Es una region de Textura, en vez de contener un Texture2D solamente, almacena tambien la region especifica a renderizar
    /// </summary>
    public readonly struct TextureRegion(Texture2D texture2D, Rectangle sourceRectangle)
    {
        /// <summary>
        /// Source texture for the region.
        /// Textura de la cual se sacara la region.
        /// </summary>
        public readonly Texture2D Texture { get; } = texture2D;

        /// <summary>
        /// Specific rectangle within the texture.
        /// Zona especifica de la textura.
        /// </summary>
        public readonly Rectangle SourceRectangle { get; } = sourceRectangle;

        /// <summary>
        /// Gets the region width.
        /// Helper para obtener el ancho.
        /// </summary>
        public int Width => SourceRectangle.Width;

        /// <summary>
        /// Gets the region height.
        /// Helper para obtener el alto.
        /// </summary>
        public int Height => SourceRectangle.Height;

        /// <summary>
        /// Creates a region from rectangle coordinates.
        /// Constructor secundario que recibe los parametros de un rectangle
        /// </summary>
        /// <param name="texture">Source texture. Textura origen.</param>
        /// <param name="x">X coordinate. Coordenada X.</param>
        /// <param name="y">Y coordinate. Coordenada Y.</param>
        /// <param name="width">Width of the region. Ancho de la región.</param>
        /// <param name="height">Height of the region. Alto de la región.</param>
        public TextureRegion(Texture2D texture, int x, int y, int width, int height) : this(texture, new Rectangle(x, y, width, height)) { }

    }
}
