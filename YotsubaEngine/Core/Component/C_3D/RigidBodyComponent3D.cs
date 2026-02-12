using Microsoft.Xna.Framework;
using static YotsubaEngine.Core.Component.C_AGNOSTIC.RigidBody;

namespace YotsubaEngine.Core.Component.C_3D
{
    /// <summary>
    /// Componente que añade datos de cuerpo rígido 3D.
    /// <para>Component that adds 3D rigid body data.</para>
    /// </summary>
    /// <param name="sphere">Esfera de colisión inicial.<para>Initial collision sphere.</para></param>
    /// <param name="mass">Nivel de masa del cuerpo.<para>Mass level for the body.</para></param>
    public struct RigidBodyComponent3D(BoundingSphere sphere, MassLevel mass)
    {
        /// <summary>
        /// Cuerpo 3D del componente.
        /// <para>Bounding sphere used for collisions.</para>
        /// </summary>
        public BoundingSphere BoundingSphere { get; set; } = sphere;

        /// <summary>
        /// Velocidad del objeto 3D.
        /// <para>Velocity of the 3D object.</para>
        /// </summary>
        public Vector3 Velocity { get; set; } = Vector3.Zero;

        /// <summary>
        /// Masa del objeto para determinar su inercia y resistencia a fuerzas.
        /// <para>Mass used to determine inertia and resistance.</para>
        /// </summary>
        public MassLevel Mass { get; set; } = mass;

        /// <summary>
        /// Desfase de colisión.
        /// <para>Collision offset.</para>
        /// </summary>
        public Vector3 OffSetCollision { get; set; } = Vector3.Zero;
    }
}
