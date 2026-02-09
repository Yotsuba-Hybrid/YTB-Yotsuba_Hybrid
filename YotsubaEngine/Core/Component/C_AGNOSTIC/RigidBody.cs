
namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    /// <summary>
    /// Base class for shared rigid body definitions.
    /// Clase base para definiciones compartidas de cuerpos rígidos.
    /// </summary>
    public abstract class RigidBody
    {

        /// <summary>
        /// Mass levels used to determine inertia and resistance.
        /// Niveles de masa para determinar la inercia y resistencia a fuerzas.
        /// Dos niveles debajo pueden mover un objeto 2 niveles por encima, pero mientras mas bajo
        /// con menos fuerza. A excepcion del ultimo, el cual es para objetos inamovibles bajo ninguna excepcion
        /// </summary>
        public enum MassLevel
        {
            /// <summary>
            /// Collidable mass level.
            /// Nivel de masa con colisión.
            /// </summary>
            Collision = 1,
            /// <summary>
            /// Non-collidable mass level.
            /// Nivel de masa sin colisión.
            /// </summary>
            NoCollision = 2,
            /// <summary>
            /// Slow movement mass level.
            /// Nivel de masa de movimiento lento.
            /// </summary>
            Slow = 3,
        }


    }
}
