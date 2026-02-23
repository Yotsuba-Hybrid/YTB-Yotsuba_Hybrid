using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using YotsubaEngine.Audio;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.System.GumUI;
using YotsubaEngine.Core.System.S_2D;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Core.YotsubaGame.Scripting;
using YotsubaEngine.Exceptions;
using YotsubaEngine.HighestPerformanceTypes;

namespace YotsubaEngine.Core.System.S_AGNOSTIC
{
    /// <summary>
    /// Sistema que ejecuta componentes de script para entidades.
    /// <para>System that runs script components for entities.</para>
    /// </summary>
    public class ScriptSystem
    {

        /// <summary>
        /// Event manager reference.
        /// Referencia al EventManager para manejar eventos.
        /// </summary>
        private EventManager EventManager { get; set; }

        /// <summary>
        /// Entity manager reference.
        /// Referencia al EntityManager para manejar entidades y componentes.
        /// </summary>
        private EntityManager EntityManager { get; set; }

        /// <summary>
        /// Inicializa el sistema de scripts.
        /// <para>Initializes the script system.</para>
        /// </summary>
        /// <param name="entities">Administrador de entidades. <para>Entity manager.</para></param>
        public void InitializeSystem(EntityManager entities)
        {
//-:cnd:noEmit
#if YTB
            if (GameWontRun.GameWontRunByException) return;

#endif
//+:cnd:noEmit
            EntityManager = entities;
            EventManager = EventManager.Instance;
        }

        /// <summary>
        /// Actualiza scripts para una entidad.
        /// <para>Updates scripts for a single entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de entidad. <para>Entity instance.</para></param>
        /// <param name="time">Tiempo de juego. <para>Game time.</para></param>
        public void SharedEntityForEachUpdate(ref Yotsuba entity, GameTime time)
        {
//-:cnd:noEmit
#if YTB
			if (GameWontRun.GameWontRunByException || !RenderSystem2D.IsGameActive) return;
#endif
//+:cnd:noEmit
			if (EntityManager == null) return;
			if (!entity.HasComponent(YTBComponent.Script)) return;
            ref ScriptComponent component = ref EntityManager.ScriptComponents[entity.Id];
            foreach(ref var script in component.Scripts.AsSpan())
            {
                script.Update(time);
            }
        }

        /// <summary>
        /// Inicializa scripts para una entidad.
        /// <para>Initializes scripts for a single entity.</para>
        /// </summary>
        /// <param name="entity">Instancia de entidad. <para>Entity instance.</para></param>
        public void SharedEntityInitialize(ref Yotsuba entity)
        {

//-:cnd:noEmit
#if YTB
			if (GameWontRun.GameWontRunByException) return;
#endif
//+:cnd:noEmit
			if (EntityManager == null) return;
			if (!entity.HasComponent(YTBComponent.Script)) return;

            ref ScriptComponent component = ref EntityManager.ScriptComponents[entity.Id];

            try
            {
                if (component.ScriptLanguaje.TryGetValue(ScriptComponentType.CSHARP, out string scriptName))
                {
                    string basePath = AppContext.BaseDirectory;


                    string scriptPath = Path.Combine(basePath, scriptName);

                    BaseScript scriptCompilado = ScriptLoader.LoadScriptInstance(scriptPath);
                    scriptCompilado.EntityId = entity.Id;
                    scriptCompilado.EntityManager = EntityManager;
                    component.Scripts.Add(scriptCompilado);

                    scriptCompilado.Initialize();

                }

            }
            catch (Exception ex)
            {
               _ = new GameWontRun(ex, GameWontRun.YTBErrors.ScriptHasError);
			}
        }

        /// <summary>
        /// Actualiza el sistema en cada frame y ejecuta dibujo 3D de scripts.
        /// <para>Updates the system each frame and runs 3D script drawing.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public void DrawSystem3D(GameTime gameTime)
        {
//-:cnd:noEmit
#if YTB
            if (GameWontRun.GameWontRunByException) return;
#endif
//+:cnd:noEmit

            if (EntityManager == null) return;

            foreach (ref Yotsuba entities in EntityManager.YotsubaEntities.AsSpan())
            {
                if (!entities.HasComponent(YTBComponent.Script)) continue;

                ref ScriptComponent script = ref EntityManager.ScriptComponents[entities];
                script.Scripts[0].Draw3D(gameTime);
            }
        }

        /// <summary>
        /// Actualiza el sistema en cada frame y dibuja scripts en 2D.
        /// <para>Updates the system each frame and draws scripts in 2D.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        /// <param name="spriteBatch">Sprite batch para dibujar. <para>Sprite batch for drawing.</para></param>
        public void DrawSystem2D(GameTime gameTime, SpriteBatch spriteBatch)
        {
//-:cnd:noEmit
#if YTB
            if (GameWontRun.GameWontRunByException) return;
#endif
//+:cnd:noEmit

            if (EntityManager == null) return;

            foreach(ref Yotsuba entities in EntityManager.YotsubaEntities.AsSpan())
            {
                if (!entities.HasComponent(YTBComponent.Script)) continue;

                ref ScriptComponent script = ref EntityManager.ScriptComponents[entities];
                script.Scripts[0].Draw2D(spriteBatch, gameTime);
            }
		}
	}
}
