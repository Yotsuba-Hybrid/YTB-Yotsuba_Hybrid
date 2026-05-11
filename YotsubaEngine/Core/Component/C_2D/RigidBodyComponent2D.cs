
using Microsoft.Xna.Framework;
using YotsubaEngine.Attributes;
using YotsubaEngine.Physics;
using YotsubaEngine.Physics.RigidBody;

namespace YotsubaEngine.Core.Component.C_2D
{

    /// <summary>
    /// Componente que añade la funcionalidad de física básica a un objeto 2D.
    /// <para>Component that adds basic 2D physics behavior.</para>
    /// </summary>
    [UIComponent("Cuerpo Rígido 2D", nameof(RigidBodyComponent2D))]
    public struct RigidBodyComponent2D(CollisionLevel collide, float mass = 1.0f)
    {
        /// <summary>
        /// Capa de colisión de la entidad (no serializada por defecto).
        /// </summary>
        public CollisionLayer CollisionLayer { get; set; } = CollisionLayer.Main;

        /// <summary>
        /// Velocidad base del objeto.
        /// </summary>
        public float SPEED = 1.0f;

        /// <summary>
        /// Velocidad máxima del objeto.
        /// </summary>
        public float TOP_SPEED = 3.0f;

        /// <summary>
        /// Desfase respecto a la colisión.
        /// </summary>
        [UIComponentValue("Desfase de colisión", nameof(OffSetCollision), "Offset 2D del rectángulo de colisión.", "Formato: X,Y (2 números decimales).")]
        public Vector2 OffSetCollision { get; set; } = Vector2.Zero;

        /// <summary>
        /// Velocidad del objeto en el espacio 3D.
        /// </summary>
        [UIComponentValue("Velocidad", nameof(Velocity), "Velocidad inicial en el espacio 3D.", "Formato: X,Y,Z (3 números decimales).")]
        public Vector3 Velocity { get; set; } = Vector3.Zero;

        /// <summary>
        /// Nivel de colisión: Solid (colisiona) o Trigger (atraviesa).
        /// </summary>
        [UIComponentValue("Tipo de colisión", nameof(Collide), "Solid colisiona, Trigger atraviesa.", "Valor de CollisionLevel no válido.")]
        public CollisionLevel Collide { get; set; } = collide;

        /// <summary>
        /// Masa física real del objeto.
        /// </summary>
        [UIComponentValue("Masa", nameof(Mass), "Masa física. Determina distribución de fuerza.", "La masa debe ser un número decimal válido.")]
        public float Mass { get; set; } = mass;

    }
}