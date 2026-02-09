using Microsoft.Xna.Framework;
using System;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.System.S_2D;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events.YTBEvents;
using YotsubaEngine.Exceptions;

namespace YotsubaEngine.Core.System.S_AGNOSTIC
{
    /// <summary>
    /// System responsible for applying cameras to the scene.
    /// Sistema encargado de colocar aplicar las camaras a la escena
    /// </summary>
    /// <param name="graphicsDevice">Graphics device manager. Administrador de dispositivo gráfico.</param>
    public class CameraSystem(GraphicsDeviceManager graphicsDevice) : ISystem
    {
        /// <summary>
        /// Gets the current entity manager.
        /// Obtiene el administrador de entidades actual.
        /// </summary>
        public EntityManager EntityManager { get; private set; }

        /// <summary>
        /// Gets the event manager instance.
        /// Obtiene la instancia del administrador de eventos.
        /// </summary>
        public EventManager EventManager { get; private set; }

        /// <summary>
        /// Gets the graphics device manager used for camera updates.
        /// Obtiene el administrador de gráficos usado para actualizar la cámara.
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; } = graphicsDevice;

        /// <summary>
        /// Initializes the camera system.
        /// Inicializa el sistema de cámara.
        /// </summary>
        /// <param name="entities">Entity manager. Administrador de entidades.</param>
        public void InitializeSystem(EntityManager entities)
        {
#if YTB
			if (GameWontRun.GameWontRunByException) return;
#endif
			EntityManager = entities;
            EventManager = EventManager.Instance;
            EventManager.Subscribe<OnCameraSet>(CameraSetEvent);
            EngineUISystem.SendLog(typeof(CameraSystem).Name + " Se inicio correctamente");

        }

        /// <summary>
        /// Handles camera set events.
        /// Evento para colocar una nueva camara en la escena
        /// </summary>
        /// <param name="set">Camera set event. Evento de cámara.</param>
        private void CameraSetEvent(OnCameraSet set)
        {
            EntityManager.Camera = set.camera;
        }

        /// <summary>
        /// Updates the active camera each frame.
        /// Actualiza la cámara activa en cada frame.
        /// </summary>
        /// <param name="gameTime">Game time. Tiempo de juego.</param>
        public void UpdateSystem(GameTime gameTime)
        {
#if YTB
			if (OperatingSystem.IsWindows())
				if (!RenderSystem2D.IsGameActive) return;

			if (GameWontRun.GameWontRunByException) return;
#endif

			ref TransformComponent transform = ref EntityManager.TransformComponents[EntityManager.Camera.EntityToFollow];
            EntityManager.Camera.Update();
        }


        /// <summary>
        /// Shared entity update hook (unused in this system).
        /// Hook de actualización compartida (no usado en este sistema).
        /// </summary>
        /// <param name="Entidad">Entity instance. Instancia de entidad.</param>
        /// <param name="time">Game time. Tiempo de juego.</param>
        public void SharedEntityForEachUpdate(Yotsuba Entidad, GameTime time)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Shared entity initialization hook (unused in this system).
        /// Hook de inicialización compartida (no usado en este sistema).
        /// </summary>
        /// <param name="Entidad">Entity instance. Instancia de entidad.</param>
        public void SharedEntityInitialize(Yotsuba Entidad)
        {
            //throw new NotImplementedException();
        }
    }
}
