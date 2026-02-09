using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.YotsubaGame;

namespace YotsubaEngine.Core.System.Contract
{
    /// <summary>
    /// Defines a common interface for engine systems.
    /// Define una interfaz común para los sistemas del motor.
    /// </summary>
    public interface ISystem
    {
        /// <summary>
        /// Initializes the system with required entity references.
        /// Metodo para inicializar el sistema con referencias al EventManager y EntityManager.
        /// </summary>
        /// <param name="entities">Entity manager. Administrador de entidades.</param>
        void InitializeSystem(EntityManager @entities);

        /// <summary>
        /// Updates the system each frame.
        /// Implementacion de la interfaz ISystem
        /// </summary>
        /// <param name="gameTime">Game time. Tiempo de juego.</param>
        void UpdateSystem(GameTime @gameTime);

        /// <summary>
        /// Runs during a shared loop across all entities for performance.
        /// Metodo que se ejecuta en un bucle compartido que recorre todas las entidades 
        /// y de los que todos los componentes de benefician para mejorar el performance
        /// </summary>
        /// <param name="Entidad">Entity instance. Instancia de entidad.</param>
        /// <param name="time">Game time. Tiempo de juego.</param>
        void SharedEntityForEachUpdate(Yotsuba @Entidad, GameTime time);

        /// <summary>
        /// Runs once per entity during initialization.
        /// Metodo que se ejecuta en un bucle compartido que recorre todas las entidades 
        /// y de los que todos los componentes de benefician para mejorar el performance.
        /// Solo se ejecuta 1 vez por entidad, al inicio.
        /// </summary>
        /// <param name="Entidad">Entity instance. Instancia de entidad.</param>
        void SharedEntityInitialize(Yotsuba @Entidad);

    }
}
