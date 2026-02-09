using Microsoft.Xna.Framework;
using static YotsubaEngine.Core.Component.C_AGNOSTIC.RigidBody;

namespace YotsubaEngine.Core.Component.C_3D
{
    /// <summary>
    /// Component that adds 3D rigid body data.
    /// Componente que añade datos de cuerpo rígido 3D.
    /// </summary>
    public struct RigidBodyComponent3D(BoundingSphere sphere, MassLevel mass)
    {
        /// <summary>
        /// Bounding sphere used for collisions.
        /// Cuerpo 3D del componente
        /// </summary>
        public BoundingSphere BoundingSphere { get; set; } = sphere;

        /// <summary>
        /// Velocity of the 3D object.
        /// Velocidad del objetp 3d
        /// </summary>
        public Vector3 Velocity { get; set; } = Vector3.Zero;

        /// <summary>
        /// Mass used to determine inertia and resistance.
        /// Masa del objeto para determinar su inercia y resistencia a fuerzas.
        /// </summary>
        public MassLevel Mass { get; set; } = mass;

        /// <summary>
        /// Collision offset.
        /// Desfase de colisión
        /// </summary>
        public Vector3 OffSetCollision { get; set; } = Vector3.Zero;
    }
}
