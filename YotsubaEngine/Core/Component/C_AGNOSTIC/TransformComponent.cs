using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{

    /// <summary>
    /// Component that controls sprite transform properties on screen.
    /// Componente que añade la funcionalidad de Transformar como se ve el sprite, en la pantalla.
    /// Constructor principal que recibe posicion, escala, efecto de reflejo y tinte del sprite.
    /// </summary>
    /// <param name="position">Initial position. Posición inicial.</param>
    /// <param name="scale">Initial scale. Escala inicial.</param>
    /// <param name="spriteEffects">Sprite effects. Efectos del sprite.</param>
    /// <param name="color">Initial tint color. Color de tinte inicial.</param>
    public struct TransformComponent(Vector3 position, Vector3 size, float scale, SpriteEffects spriteEffects, Color color)
    {
        /// <summary>
        /// Sets the position of the transform.
        /// Establece la posición de la transformación.
        /// </summary>
        public void SetPosition(float x, float y, float z) => Position = new Vector3(x, y, z);

        /// <summary>
        /// Gets or sets the size of the sprite.
        /// Obtiene o establece el tamaño del sprite.
        /// </summary>
        public Vector3 Size { get; set; } = size;

        /// <summary>
        /// Scale applied to the sprite.
        /// Propiedad para escalar el sprite
        /// </summary>
        public float Scale { get; set; } = scale;

        /// <summary>
        /// Rotation applied to the sprite.
        /// Propiedad para rotar el sprite
        /// </summary>
        public float Rotation { get; set; } = 0f;

        /// <summary>
        /// Depth layer for rendering order.
        /// Propiedad para definir la capa de profundidad del sprite
        /// </summary>
        public float LayerDepth { get; set; } = 0f;

        /// <summary>
        /// Position of the sprite in world space.
        /// Propiedad para posicionar el sprite en pantalla
        /// </summary>
        public Vector3 Position { get; set; } = position;

        /// <summary>
        /// Sprite effects such as flipping.
        /// Propiedad para darle efecto de reflejo vertical u horizontal al sprite
        /// </summary>
        public SpriteEffects SpriteEffects { get; set; } = spriteEffects;

        /// <summary>
        /// Tint color applied to the sprite.
        /// Propiedad que le añade un filtro, o tono de color al sprite
        /// </summary>
        public Color Color { get; set; } = color;

        /// <summary>
        /// Creates a transform with position and size, defaulting other values.
        /// Constructor que recibe una Posicion, y coloca la escala en 1,
        /// no aplica efectos de reflejo, y no añade tinte de color.
        /// </summary>
        /// <param name="position">Receives the position and size of the sprite. Recibe la Posicion y size del sprite.</param>
        public TransformComponent(Vector3 position, Vector3 size) : this(position, size, 1f, SpriteEffects.None, Color.White) { }

        /// <summary>
        /// Creates a transform with position, size, and scale.
        /// Constructor que recibe la posicion, size y escala del sprite
        /// </summary>
        /// <param name="position">Sprite position. Posición del sprite.</param>
        /// <param name="scale">Sprite scale. Escala del sprite.</param>
        public TransformComponent(Vector3 position, Vector3 size, float scale): this(position, size ,scale, SpriteEffects.None, Color.White) { }
    }
}
