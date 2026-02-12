using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{

    /// <summary>
    /// Componente que controla las propiedades de transformación del sprite en pantalla. El constructor principal recibe posición, tamaño, escala, efecto de reflejo y tinte del sprite.
    /// <para>Component that controls sprite transform properties on screen. The primary constructor receives position, size, scale, flip effect, and sprite tint.</para>
    /// </summary>
    /// <param name="position">Posición inicial.<para>Initial position.</para></param>
    /// <param name="size">Tamaño inicial.<para>Initial size.</para></param>
    /// <param name="scale">Escala inicial.<para>Initial scale.</para></param>
    /// <param name="spriteEffects">Efectos del sprite.<para>Sprite effects.</para></param>
    /// <param name="color">Color de tinte inicial.<para>Initial tint color.</para></param>
    public struct TransformComponent(Vector3 position, Vector3 size, float scale, SpriteEffects spriteEffects, Color color)
    {
        /// <summary>
        /// Establece la posición de la transformación.
        /// <para>Sets the transform position.</para>
        /// </summary>
        /// <param name="x">Coordenada X de la posición.<para>X coordinate of the position.</para></param>
        /// <param name="y">Coordenada Y de la posición.<para>Y coordinate of the position.</para></param>
        /// <param name="z">Coordenada Z de la posición.<para>Z coordinate of the position.</para></param>
        public void SetPosition(float x, float y, float z) => Position = new Vector3(x, y, z);

        /// <summary>
        /// Obtiene o establece el tamaño del sprite.
        /// <para>Gets or sets the sprite size.</para>
        /// </summary>
        public Vector3 Size { get; set; } = size;

        /// <summary>
        /// Escala aplicada al sprite.
        /// <para>Scale applied to the sprite.</para>
        /// </summary>
        public float Scale { get; set; } = scale;

        /// <summary>
        /// Rotación aplicada al sprite.
        /// <para>Rotation applied to the sprite.</para>
        /// </summary>
        public float Rotation { get; set; } = 0f;

        /// <summary>
        /// Capa de profundidad para el orden de renderizado.
        /// <para>Depth layer for rendering order.</para>
        /// </summary>
        public float LayerDepth { get; set; } = 0f;

        /// <summary>
        /// Posición del sprite en el espacio del mundo.
        /// <para>Sprite position in world space.</para>
        /// </summary>
        public Vector3 Position { get; set; } = position;

        /// <summary>
        /// Efectos del sprite como reflejo.
        /// <para>Sprite effects such as flipping.</para>
        /// </summary>
        public SpriteEffects SpriteEffects { get; set; } = spriteEffects;

        /// <summary>
        /// Color de tinte aplicado al sprite.
        /// <para>Tint color applied to the sprite.</para>
        /// </summary>
        public Color Color { get; set; } = color;

        /// <summary>
        /// Crea una transformación con posición y tamaño, usando valores por defecto para escala, efecto de reflejo y tinte.
        /// <para>Creates a transform with position and size, defaulting scale, flip effects, and tint.</para>
        /// </summary>
        /// <param name="position">Posición del sprite.<para>Sprite position.</para></param>
        /// <param name="size">Tamaño del sprite.<para>Sprite size.</para></param>
        public TransformComponent(Vector3 position, Vector3 size) : this(position, size, 1f, SpriteEffects.None, Color.White) { }

        /// <summary>
        /// Crea una transformación con posición, tamaño y escala.
        /// <para>Creates a transform with position, size, and scale.</para>
        /// </summary>
        /// <param name="position">Posición del sprite.<para>Sprite position.</para></param>
        /// <param name="size">Tamaño del sprite.<para>Sprite size.</para></param>
        /// <param name="scale">Escala del sprite.<para>Sprite scale.</para></param>
        public TransformComponent(Vector3 position, Vector3 size, float scale): this(position, size ,scale, SpriteEffects.None, Color.White) { }
    }
}
