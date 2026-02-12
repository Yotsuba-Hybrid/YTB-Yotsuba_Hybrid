using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.Core.Component.C_2D
{

    /// <summary>
    /// Componente que añade funcionalidad de mostrar un sprite en la pantalla.
    /// <para>Component that renders a sprite to the screen.</para>
    /// </summary>
    /// <param name="texture">Textura a renderizar.<para>Texture2D to render.</para></param>
    /// <param name="sourceRectangle">Región de la textura a renderizar.<para>Texture region to render.</para></param>
    public struct SpriteComponent2D(Texture2D texture, Rectangle sourceRectangle)
    {
        /// <summary>
        /// Textura usada para renderizar la entidad.
        /// <para>Texture used to render the entity.</para>
        /// </summary>
        public Texture2D Texture { get; set; } = texture;

        /// <summary>
        /// Restringe el área de la textura a dibujar.
        /// <para>Restricts the area of the texture to draw.</para>
        /// </summary>
        public Rectangle SourceRectangle { get; set; } = sourceRectangle;

        /// <summary>
        /// Indica si el sprite debe renderizarse.
        /// <para>Indicates whether the sprite should be rendered.</para>
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Indica la dimensión en la que se renderizará el sprite.
        /// <para>Indicates the dimension in which the sprite will be rendered.</para>
        /// </summary>
        public bool Is2_5D { get; set; } = false;
    }

}
