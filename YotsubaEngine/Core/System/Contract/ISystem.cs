using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.YotsubaGame;

namespace YotsubaEngine.Core.System.Contract
{
    /// <summary>
    /// Define una interfaz común para los sistemas del motor.
    /// <para>Defines a common interface for engine systems.</para>
    /// </summary>
    public interface ISystem
    {
        /// <summary>
        /// Inicializa el sistema con las referencias de entidades necesarias.
        /// <para>Initializes the system with required entity references.</para>
        /// </summary>
        /// <param name="entities">Administrador de entidades. <para>Entity manager.</para></param>
        void InitializeSystem(EntityManager @entities);

        /// <summary>
        /// Actualiza el sistema en cada frame.
        /// <para>Updates the system each frame.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        void UpdateSystem(GameTime @gameTime);

        /// <summary>
        /// Se ejecuta en un bucle compartido que recorre todas las entidades para mejorar el rendimiento.
        /// <para>Runs during a shared loop across all entities for performance.</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        /// <param name="time">Tiempo de juego. <para>Game time.</para></param>
        void SharedEntityForEachUpdate(Yotsuba @Entidad, GameTime time);

        /// <summary>
        /// Se ejecuta una vez por entidad durante la inicialización.
        /// <para>Runs once per entity during initialization.</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        void SharedEntityInitialize(Yotsuba @Entidad);

    }
}
