using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Attributes;

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
    [UIComponent("Transformación", nameof(TransformComponent))]
    public partial struct TransformComponent(Vector3 position, Vector3 size, float scale, SpriteEffects spriteEffects, Color color)
    {
        /// <summary>
        /// Establece la posición de la transformación.
        /// <para>Sets the transform position.</para>
        /// </summary>
        public void SetPosition(float x, float y, float z) => Position = new Vector3(x, y, z);

        /// <summary>
        /// Obtiene o establece el tamaño del sprite.
        /// </summary>
        [UIComponentValue("Tamaño", nameof(Size), "Tamaño del sprite en el mundo", "Formato: X,Y,Z (3 números decimales separados por comas).",
            defaultValue: "100,100,0", inactiveValue: ",,")]
        public Vector3 Size { get; set; } = size;

        /// <summary>
        /// Escala aplicada al sprite.
        /// </summary>
        [UIComponentValue("Escala", nameof(Scale), "Multiplicador de escala uniforme.", "La escala debe ser un número decimal válido (ej: 1.0).",
            defaultValue: "1", inactiveValue: "")]
        public float Scale { get; set; } = scale;

        /// <summary>
        /// Rotación aplicada al sprite.
        /// </summary>
        [UIComponentValue("Rotación", nameof(Rotation), "Ángulo de rotación en radianes.", "La rotación debe ser un número decimal válido.",
            defaultValue: "0", inactiveValue: "")]
        public float Rotation { get; set; } = 0f;

        /// <summary>
        /// Posición del sprite en el espacio del mundo.
        /// </summary>
        [UIComponentValue("Posición", nameof(Position), "Posición 3D de la entidad en el mundo.", "Formato: X,Y,Z (3 números decimales separados por comas).",
            defaultValue: "0,0,1", inactiveValue: ",,")]
        public Vector3 Position { get; set; } = position;

        /// <summary>
        /// Efectos del sprite como reflejo.
        /// </summary>
        [UIComponentValue("Efectos del sprite", nameof(SpriteEffects), "Efectos de volteo (None, FlipHorizontally, FlipVertically).", "Valor de SpriteEffects no válido.",
            defaultValue: "None", inactiveValue: "")]
        public SpriteEffects SpriteEffects { get; set; } = spriteEffects;

        /// <summary>
        /// Color de tinte aplicado al sprite.
        /// </summary>
        [UIComponentValue("Color", nameof(Color), "Color de tinte aplicado al sprite.", "Use un nombre de color válido de XNA (ej: Red, Blue, White).",
            defaultValue: "White", inactiveValue: "")]
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
