
using Microsoft.Xna.Framework;
using YotsubaEngine.Physics;
using YotsubaEngine.Physics.RigidBody;

namespace YotsubaEngine.Core.Component.C_2D
{

    /// <summary>
    /// Componente que añade la funcionalidad de física básica a un objeto 2D.
    /// <para>Component that adds basic 2D physics behavior.</para>
    /// </summary>
    /// <param name="gameType">Tipo de juego usado para la física.<para>Game type used for physics behavior.</para></param>
    /// <param name="mass">Nivel de masa del cuerpo.<para>Mass level for the body.</para></param>
    public struct RigidBodyComponent2D(MassLevel mass)
    {
        /// <summary>
        /// Capa de colisión de la entidad (solo colisionara con entidades en su misma capa, o que su capa sea "All")
        /// </summary>
        public CollisionLayer CollisionLayer { get; set; } = CollisionLayer.Main;

        /// <summary>
        /// Velocidad base del objeto.
        /// <para>Base movement speed.</para>
        /// </summary>
        public float SPEED = 1.0f;

        /// <summary>
        /// Velocidad máxima del objeto.
        /// <para>Maximum movement speed.</para>
        /// </summary>
        public float TOP_SPEED = 3.0f;

        /// <summary>
        /// Desfase respecto a la colisión.
        /// <para>Collision offset.</para>
        /// </summary>
        public Vector2 OffSetCollision { get; set; } = Vector2.Zero;

        /// <summary>
        /// Velocidad del objeto en el espacio 3D.
        /// <para>Current velocity in 3D space.</para>
        /// </summary>
        public Vector3 Velocity { get; set; } = Vector3.Zero;

        /// <summary>
        /// Masa del objeto para determinar su inercia y resistencia a fuerzas.
        /// <para>Mass used to determine inertia and resistance.</para>
        /// </summary>
        public MassLevel Mass { get; set; } = mass;

    }
}