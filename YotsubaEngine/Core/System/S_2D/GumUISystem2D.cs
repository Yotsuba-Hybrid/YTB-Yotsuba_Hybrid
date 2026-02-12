using Microsoft.Xna.Framework;
using System;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.System.GumUI;
using YotsubaEngine.Core.YotsubaGame;

namespace YotsubaEngine.Core.System.S_2D
{
    /// <summary>
    /// Sistema responsable de gestionar y renderizar componentes de interfaz GumUI.
    /// <para>System responsible for managing and rendering GumUI user interface components.</para>
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
        /// Inicializa el sistema GumUI limpiando todos los elementos de UI existentes.
        /// <para>Initializes the GumUI system by clearing all existing UI elements.</para>
        /// </summary>
        /// <param name="entities">Instancia del gestor de entidades. <para>The entity manager instance.</para></param>
        public void InitializeSystem(EntityManager entities)
        {
            YTBGum.Clear();
        }

        /// <summary>
        /// Actualiza la lógica del sistema GumUI en cada frame.
        /// <para>Updates the GumUI system logic each frame.</para>
        /// </summary>
        /// <param name="gameTime">Instantánea de valores de tiempo. <para>Snapshot of timing values.</para></param>
        public void UpdateSystem(GameTime gameTime)
        {
            YTBGum.Update(gameTime);
        }

        /// <summary>
        /// Renderiza todos los componentes GumUI en la pantalla.
        /// <para>Renders all GumUI components to the screen.</para>
        /// </summary>
        /// <param name="gameTime">Instantánea de valores de tiempo. <para>Snapshot of timing values.</para></param>
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
        /// Callback de actualización por entidad (no usado por el sistema GumUI).
        /// <para>Per-entity update callback (not used by GumUI system).</para>
        /// </summary>
        /// <param name="Entidad">Entidad a actualizar. <para>The entity to update.</para></param>
        /// <param name="time">Información de tiempo del juego. <para>Game timing information.</para></param>
        public void SharedEntityForEachUpdate(Yotsuba Entidad, GameTime time)
        {
        }

        /// <summary>
        /// Callback de inicialización por entidad (no usado por el sistema GumUI).
        /// <para>Per-entity initialization callback (not used by GumUI system).</para>
        /// </summary>
        /// <param name="Entidad">Entidad a inicializar. <para>The entity to initialize.</para></param>
        public void SharedEntityInitialize(Yotsuba Entidad)
        {
        }
    }
}
