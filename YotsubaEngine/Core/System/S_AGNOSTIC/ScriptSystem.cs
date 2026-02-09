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
    /// System that runs script components for entities.
    /// Sistema que ejecuta componentes de script para entidades.
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
        /// Initializes the script system.
        /// Inicializa el sistema de scripts.
        /// </summary>
        /// <param name="entities">Entity manager. Administrador de entidades.</param>
        public void InitializeSystem(EntityManager entities)
        {
#if YTB
            if (GameWontRun.GameWontRunByException) return;

#endif
            EntityManager = entities;
            EventManager = EventManager.Instance;
        }

        /// <summary>
        /// Updates scripts for a single entity.
        /// Actualiza scripts para una entidad.
        /// </summary>
        /// <param name="entity">Entity instance. Instancia de entidad.</param>
        /// <param name="time">Game time. Tiempo de juego.</param>
        public void SharedEntityForEachUpdate(Yotsuba entity, GameTime time)
        {
#if YTB
			if (GameWontRun.GameWontRunByException || !RenderSystem2D.IsGameActive) return;
#endif
			if (!entity.HasComponent(YTBComponent.Script)) return;
            ref ScriptComponent component = ref EntityManager.ScriptComponents[entity.Id];
            foreach(var script in component.Scripts)
            {
                script.Update(time);
            }
        }

        /// <summary>
        /// Initializes scripts for a single entity.
        /// Inicializa scripts para una entidad.
        /// </summary>
        /// <param name="entity">Entity instance. Instancia de entidad.</param>
        public void SharedEntityInitialize(Yotsuba entity)
        {

#if YTB
			if (GameWontRun.GameWontRunByException) return;
#endif
			if (!entity.HasComponent(YTBComponent.Script)) return;

            ref ScriptComponent component = ref EntityManager.ScriptComponents[entity.Id];

            try
            {
                if (component.ScriptLanguaje.TryGetValue(ScriptComponentType.CSHARP, out string scriptName))
                {
                    string basePath = AppContext.BaseDirectory;


                    string scriptPath = Path.Combine(basePath, scriptName);

                    BaseScript scriptCompilado = ScriptLoader.LoadScriptInstance(scriptPath);
                    scriptCompilado.Entity = entity;
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
        /// Updates the system each frame (required by ISystem interface).
        /// Actualiza el sistema en cada frame (requerido por interfaz ISystem).
        /// </summary>
        /// <param name="gameTime">Game time. Tiempo de juego.</param>
        public void DrawSystem3D(GameTime gameTime)
        {
#if YTB
            if (GameWontRun.GameWontRunByException) return;
#endif


            foreach (var entities in EntityManager.YotsubaEntities)
            {
                if (!entities.HasComponent(YTBComponent.Script)) continue;

                ref var script = ref EntityManager.ScriptComponents[entities];
                script.Scripts.FirstOrDefault()?.Draw3D(gameTime);
            }
        }

        /// <summary>
        /// Updates the system each frame and draws scripts.
        /// Actualiza el sistema en cada frame y dibuja scripts.
        /// </summary>
        /// <param name="gameTime">Game time. Tiempo de juego.</param>
        /// <param name="spriteBatch">Sprite batch for drawing. Sprite batch para dibujar.</param>
        public void DrawSystem2D(GameTime gameTime, SpriteBatch spriteBatch)
        {
#if YTB
            if (GameWontRun.GameWontRunByException) return;
#endif

            foreach(var entities in EntityManager.YotsubaEntities)
            {
                if (!entities.HasComponent(YTBComponent.Script)) continue;

                ref var script = ref EntityManager.ScriptComponents[entities];
                script.Scripts.FirstOrDefault()?.Draw2D(spriteBatch, gameTime);
            }
		}
	}
}
