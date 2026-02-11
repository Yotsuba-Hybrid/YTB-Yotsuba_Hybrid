
using Microsoft.Xna.Framework;
using System;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events.YTBEvents;
using YotsubaEngine.Exceptions;
using YotsubaEngine.Graphics;
using YotsubaEngine.YTB_Toolkit;

namespace YotsubaEngine.Core.System.S_2D
{
    /// <summary>
    /// System that manages all entity animations in the game.
    /// Sistema encargado de gestionar todas las animaciones de las entidades del juego
    /// </summary>
    public class AnimationSystem2D : ISystem
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
        /// Initializes the animation system.
        /// Implementacion de la interfaz ISystem
        /// </summary>
        /// <param name="entities">Entity manager. Administrador de entidades.</param>
        public void InitializeSystem(EntityManager @entities)
        {
#if YTB
			if (GameWontRun.GameWontRunByException) return;
#endif
			EventManager = EventManager.Instance;
            EntityManager = @entities;

            EventManager.Subscribe<AnimationChangeEvent>(OnAnimationChange);
            EngineUISystem.SendLog(typeof(AnimationSystem2D).Name + " Se inicio correctamente");
        }

        /// <summary>
        /// Handles animation change events.
        /// Metodo que maneja el evento de cambio de animacion.
        /// </summary>
        /// <param name="event">Animation event. Evento de animación.</param>
        private void OnAnimationChange(AnimationChangeEvent @event)
        {
            int id = @event.EntityId;

            // Validar bounds primero
            if (id < 0 || id >= EntityManager.YotsubaEntities.Count) return;

            // Validar que exista componente de animación en la colección de components
            if (id >= EntityManager.Animation2DComponents.Count) return;

            ref var entity = ref EntityManager.YotsubaEntities[id];
            if (!entity.HasComponent(YTBComponent.Animation)) return;

            ref AnimationComponent2D animationComponent = ref EntityManager.Animation2DComponents[id];
            var animation = animationComponent.GetAnimation(@event.AnimationName);
            animation.Reset();
            animationComponent.CurrentAnimationType = (@event.AnimationName, animation);
        }

        /// <summary>
        /// Updates animations each frame.
        /// Implementación de la interfaz ISystem
        /// </summary>
        /// <param name="gameTime">Game time. Tiempo de juego.</param>
        public void UpdateSystem(GameTime gameTime)
        {
#if YTB
			if (OperatingSystem.IsWindows())
				if (!RenderSystem2D.IsGameActive) return;

            if (GameWontRun.GameWontRunByException) return;
#endif

            if (EntityManager == null) return;
            foreach (Yotsuba entity in EntityManager.YotsubaEntities)
            {
                if (!entity.HasComponent(YTBComponent.Animation) || !entity.HasComponent(YTBComponent.Sprite)) continue;
                ref AnimationComponent2D animationComponent = ref EntityManager.Animation2DComponents[entity.Id];
                ref SpriteComponent2D spriteComponent = ref EntityManager.Sprite2DComponents[entity.Id];

                if (animationComponent.CurrentAnimationType.Item2.IsFinished)
                {
                    if (!animationComponent.CurrentAnimationType.Item2.FinishedWasMarked)
                    {
                        EventManager.Publish(new OnAnimationDontLoopReleaseEvent()
                        {
                            EntityId = entity.Id,
                            AnimationName = animationComponent.CurrentAnimationType.Item1
                        });
                        animationComponent.CurrentAnimationType.Item2.FinishedWasMarked = true;
                    }
                }
                else
                {
                    TextureRegion region = animationComponent.CurrentAnimationType.Item2.CurrentFrame(gameTime);
                    spriteComponent.Texture = region.Texture;
                    spriteComponent.SourceRectangle = region.SourceRectangle;
                }
            }
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
