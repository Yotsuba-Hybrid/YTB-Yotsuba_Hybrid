
namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    /// <summary>
    /// Clase base para definiciones compartidas de cuerpos rígidos.
    /// <para>Base class for shared rigid body definitions.</para>
    /// </summary>
    public abstract class RigidBody
    {

        /// <summary>
        /// Niveles de masa para determinar la inercia y resistencia a fuerzas. Dos niveles debajo pueden mover un objeto dos niveles por encima, pero con menos fuerza. A excepción del último, el cual es para objetos inamovibles bajo ninguna excepción.
        /// <para>Mass levels used to determine inertia and resistance to forces. Two levels below can move an object two levels above, but with less force. The last level is for immovable objects under any circumstance.</para>
        /// </summary>
        public enum MassLevel
        {
            /// <summary>
            /// Nivel de masa con colisión.
            /// <para>Collidable mass level.</para>
            /// </summary>
            Collision = 1,
            /// <summary>
            /// Nivel de masa sin colisión.
            /// <para>Non-collidable mass level.</para>
            /// </summary>
            NoCollision = 2,
            /// <summary>
            /// Nivel de masa de movimiento lento.
            /// <para>Slow movement mass level.</para>
            /// </summary>
            Slow = 3,
        }


    }
}
