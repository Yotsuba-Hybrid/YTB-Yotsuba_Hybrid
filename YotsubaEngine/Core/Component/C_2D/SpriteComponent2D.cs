using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.Core.Component.C_2D
{

    /// <summary>
    /// Component that renders a sprite to the screen.
    /// Componente que añade funcionalidad de mostrar algo en la pantalla.
    /// </summary>
    /// <param name="texture">Receives the Texture2D to render. Recibe una Textura2D para renderizar.</param>
    /// <param name="sourceRectangle">Receives the texture region to render. Recibe una region limitada de la textura a renderizar.</param>
    public struct SpriteComponent2D(Texture2D texture, Rectangle sourceRectangle)
    {
        /// <summary>
        /// Texture used to render the entity.
        /// Imagen que renderizara la entidad
        /// </summary>
        public Texture2D Texture { get; set; } = texture;

        /// <summary>
        /// Restricts the area of the texture to draw.
        /// Propiedad que permite restringir el area que se dibujara dibujar
        /// </summary>
        public Rectangle SourceRectangle { get; set; } = sourceRectangle;

        /// <summary>
        /// Indicates whether the sprite should be rendered.
        /// Flag que marca si el sprite se renderizara o no
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Flag que indica la dimencion en que se renderizara el sprite.
        /// </summary>
        public bool Is2_5D { get; set; } = false;
    }

}
