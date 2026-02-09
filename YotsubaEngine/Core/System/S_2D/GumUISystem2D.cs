using Microsoft.Xna.Framework;
using System;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.System.GumUI;
using YotsubaEngine.Core.YotsubaGame;

namespace YotsubaEngine.Core.System.S_2D
{
    /// <summary>
    /// System responsible for managing and rendering GumUI user interface components.
    /// Sistema responsable de gestionar y renderizar componentes de interfaz GumUI.
    /// </summary>
    /// <remarks>
    /// This system integrates with YTBGum to provide a lightweight UI framework.
    /// In DEBUG mode on Windows, it respects the game's active state to prevent rendering when minimized.
    /// 
    /// Este sistema se integra con YTBGum para proporcionar un framework de UI ligero.
    /// En modo DEBUG en Windows, respeta el estado activo del juego para evitar renderizado cuando está minimizado.
    /// </remarks>
    public class GumUISystem2D : ISystem
    {
        /// <summary>
        /// Initializes the GumUI system by clearing all existing UI elements.
        /// Inicializa el sistema GumUI limpiando todos los elementos de UI existentes.
        /// </summary>
        /// <param name="entities">The entity manager instance. Instancia del gestor de entidades.</param>
        public void InitializeSystem(EntityManager entities)
        {
            YTBGum.Clear();
        }

        /// <summary>
        /// Updates the GumUI system logic each frame.
        /// Actualiza la lógica del sistema GumUI en cada frame.
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values. Instantánea de valores de tiempo.</param>
        public void UpdateSystem(GameTime gameTime)
        {
            YTBGum.Update(gameTime);
        }

        /// <summary>
        /// Renders all GumUI components to the screen.
        /// Renderiza todos los componentes GumUI en la pantalla.
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values. Instantánea de valores de tiempo.</param>
        /// <remarks>
        /// In DEBUG builds on Windows, skips rendering if the game window is not active.
        /// En builds DEBUG en Windows, omite el renderizado si la ventana del juego no está activa.
        /// </remarks>
        public void DrawSystem(GameTime gameTime)
        {
#if YTB
            if (OperatingSystem.IsWindows())
                if (!RenderSystem2D.IsGameActive) return;
#endif
            YTBGum.Draw();
        }

        /// <summary>
        /// Per-entity update callback (not used by GumUI system).
        /// Callback de actualización por entidad (no usado por el sistema GumUI).
        /// </summary>
        /// <param name="Entidad">The entity to update. Entidad a actualizar.</param>
        /// <param name="time">Game timing information. Información de tiempo del juego.</param>
        public void SharedEntityForEachUpdate(Yotsuba Entidad, GameTime time)
        {
        }

        /// <summary>
        /// Per-entity initialization callback (not used by GumUI system).
        /// Callback de inicialización por entidad (no usado por el sistema GumUI).
        /// </summary>
        /// <param name="Entidad">The entity to initialize. Entidad a inicializar.</param>
        public void SharedEntityInitialize(Yotsuba Entidad)
        {
        }
    }
}
