
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
    /// <param name="collide">Nivel de colisión: Solid o Trigger.<para>Collision level: Solid or Trigger.</para></param>
    /// <param name="mass">Masa física real. Valor por defecto: 1.0f.<para>Physical mass. Default: 1.0f.</para></param>
    public struct RigidBodyComponent2D(CollisionLevel collide, float mass = 1.0f)
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

    }
}