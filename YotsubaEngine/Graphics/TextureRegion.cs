using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.Graphics
{
    /// <summary>
    /// Representa una región de textura para renderizar desde una textura mayor.
    /// <para>Represents a texture region to render from a larger texture.</para>
    /// </summary>
    /// <param name="texture2D">Textura origen de la región. <para>Source texture for the region.</para></param>
    /// <param name="sourceRectangle">Rectángulo de la región. <para>Region rectangle.</para></param>
    public readonly struct TextureRegion(Texture2D texture2D, Rectangle sourceRectangle)
    {
        /// <summary>
        /// Textura de la cual se extrae la región.
        /// <para>Source texture for the region.</para>
        /// </summary>
        public readonly Texture2D Texture { get; } = texture2D;

        /// <summary>
        /// Rectángulo específico dentro de la textura.
        /// <para>Specific rectangle within the texture.</para>
        /// </summary>
        public readonly Rectangle SourceRectangle { get; } = sourceRectangle;

        /// <summary>
        /// Obtiene el ancho de la región.
        /// <para>Gets the region width.</para>
        /// </summary>
        public int Width => SourceRectangle.Width;

        /// <summary>
        /// Obtiene el alto de la región.
        /// <para>Gets the region height.</para>
        /// </summary>
        public int Height => SourceRectangle.Height;

        /// <summary>
        /// Crea una región a partir de coordenadas de rectángulo.
        /// <para>Creates a region from rectangle coordinates.</para>
        /// </summary>
        /// <param name="texture">Textura origen. <para>Source texture.</para></param>
        /// <param name="x">Coordenada X. <para>X coordinate.</para></param>
        /// <param name="y">Coordenada Y. <para>Y coordinate.</para></param>
        /// <param name="width">Ancho de la región. <para>Width of the region.</para></param>
        /// <param name="height">Alto de la región. <para>Height of the region.</para></param>
        public TextureRegion(Texture2D texture, int x, int y, int width, int height) : this(texture, new Rectangle(x, y, width, height)) { }

    }
}
