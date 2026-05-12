using Microsoft.Xna.Framework;
using YotsubaEngine.Attributes;
using YotsubaEngine.Physics;
using YotsubaEngine.Physics.RigidBody;

namespace YotsubaEngine.Core.Component.C_3D
{
    /// <summary>
    /// Componente que añade datos de cuerpo rígido 3D.
    /// <para>Component that adds 3D rigid body data.</para>
    /// </summary>
    /// <param name="collide">Nivel de colisión: Solid o Trigger.<para>Collision level: Solid or Trigger.</para></param>
    /// <param name="mass">Masa física real del objeto. Valor por defecto: 1.0f.<para>Physical mass of the object. Default: 1.0f.</para></param>
    [UIComponent("Cuerpo Rigido 3D", nameof(ModelComponent3D))]
    public struct RigidBodyComponent3D(CollisionLevel collide, float mass = 1.0f)
    {
        /// </summary>
        [UIComponentValue("Capa de Colision", "CollisionLayer", "",
            "")]
        public CollisionLayer CollisionLayer { get; set; } = CollisionLayer.Main;

        /// <summary>
        /// Velocidad del objeto 3D.
        /// <para>Velocity of the 3D object.</para>
        /// </summary>
        [UIComponentValue("Velocidad de la entidad", "Velocity", "",
          "")]
        public Vector3 Velocity { get; set; } = Vector3.Zero;

        /// <summary>
        /// Nivel de colisión: Solid (colisiona) o Trigger (atraviesa).
        /// <para>Collision level: Solid (collides) or Trigger (passes through).</para>
        /// </summary>
        public CollisionLevel Collide { get; set; } = collide;

        /// <summary>
        /// Masa física real del objeto. Determina distribución de fuerza en colisiones.
        /// Valor por defecto: 1.0f. Permite cualquier número (0 para triggers).
        /// <para>Physical mass that determines force distribution in collisions. Default: 1.0f.</para>
        /// </summary>
        public float Mass { get; set; } = mass;

        /// <summary>
        /// Desfase de colisión.
        /// <para>Collision offset.</para>
        /// </summary>
        public Vector3 OffSetCollision { get; set; } = Vector3.Zero;
    }
}
