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
    /// Sistema encargado de aplicar las cámaras a la escena.
    /// <para>System responsible for applying cameras to the scene.</para>
    /// </summary>
    /// <param name="graphicsDevice">Administrador de dispositivo gráfico. <para>Graphics device manager.</para></param>
    public class CameraSystem(GraphicsDeviceManager graphicsDevice) : ISystem
    {
        /// <summary>
        /// Obtiene el administrador de entidades actual.
        /// <para>Gets the current entity manager.</para>
        /// </summary>
        public EntityManager EntityManager { get; private set; }

        /// <summary>
        /// Obtiene la instancia del administrador de eventos.
        /// <para>Gets the event manager instance.</para>
        /// </summary>
        public EventManager EventManager { get; private set; }

        /// <summary>
        /// Obtiene el administrador de gráficos usado para actualizar la cámara.
        /// <para>Gets the graphics device manager used for camera updates.</para>
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; } = graphicsDevice;

        /// <summary>
        /// Inicializa el sistema de cámara.
        /// <para>Initializes the camera system.</para>
        /// </summary>
        /// <param name="entities">Administrador de entidades. <para>Entity manager.</para></param>
        public void InitializeSystem(EntityManager entities)
        {

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
        /// Actualiza la cámara activa en cada frame.
        /// <para>Updates the active camera each frame.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public void UpdateSystem(GameTime gameTime)
        {
//-:cnd:noEmit
#if YTB
			if (OperatingSystem.IsWindows())
				if (!RenderSystem2D.IsGameActive) return;

			if (GameWontRun.GameWontRunByException) return;
#endif
//+:cnd:noEmit

			if (EntityManager == null) return;
			ref TransformComponent transform = ref EntityManager.TransformComponents[EntityManager.Camera.EntityToFollow];
            EntityManager.Camera.Update();
        }


        /// <summary>
        /// Hook de actualización compartida (no usado en este sistema).
        /// <para>Shared entity update hook (unused in this system).</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        /// <param name="time">Tiempo de juego. <para>Game time.</para></param>
        public void SharedEntityForEachUpdate(ref Yotsuba Entidad, GameTime time)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Hook de inicialización compartida (no usado en este sistema).
        /// <para>Shared entity initialization hook (unused in this system).</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        public void SharedEntityInitialize(ref Yotsuba Entidad)
        {
            //throw new NotImplementedException();
        }
    }
}
