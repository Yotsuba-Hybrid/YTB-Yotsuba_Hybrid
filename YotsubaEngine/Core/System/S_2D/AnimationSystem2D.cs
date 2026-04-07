
using Microsoft.Xna.Framework;
using System;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
#if YTB
using YotsubaEngine.Core.System.YotsubaEngineUI;
#endif
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events.YTBEvents;
using YotsubaEngine.Exceptions;
using YotsubaEngine.Graphics;
using YotsubaEngine.YTB_Toolkit;

namespace YotsubaEngine.Core.System.S_2D
{
    /// <summary>
    /// Sistema encargado de gestionar todas las animaciones de las entidades del juego.
    /// <para>System that manages all entity animations in the game.</para>
    /// </summary>
    public class AnimationSystem2D : ISystem
    {
        /// <summary>
        /// Event manager reference.
        /// Referencia al EventManager para manejar eventos.
        /// </summary>
        private EventManager EventManager { get; set; }

        /// <summary>
        /// Inicializa el sistema de animaciones.
        /// <para>Initializes the animation system.</para>
        /// </summary>
        /// <param name="entities">Administrador de entidades. <para>Entity manager.</para></param>
        public override void InitializeSystem(EntityManager @entities)
        {

			EventManager = EventManager.Instance;
            EntityManager = @entities;

            EventManager.Subscribe<AnimationChangeEvent>(OnAnimationChange);
#if YTB
            EngineUISystem.SendLog(typeof(AnimationSystem2D).Name + " Se inicio correctamente");
#endif
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
        /// Actualiza las animaciones en cada frame.
        /// <para>Updates animations each frame.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public override void UpdateSystem(GameTime gameTime)
        {
//-:cnd:noEmit
#if YTB
			if (YTBGlobalState.IsDesktop)
				if (!RenderSystem2D.IsGameActive) return;

            if (GameWontRun.GameWontRunByException) return;
#endif
            //+:cnd:noEmit

            Span<AnimationComponent2D> animationsComponents = GetAnimationComponentsAsSpan();
            Span<SpriteComponent2D> spriteComponents = GetSpriteComponentsAsSpan();
            if (EntityManager == null) return;
            foreach (ref Yotsuba entity in GetEntitiesAsSpan())
            {
                if (!entity.HasComponent(YTBComponent.Animation) || !entity.HasComponent(YTBComponent.Sprite)) continue;
                ref AnimationComponent2D animationComponent = ref animationsComponents[entity.Id];
                ref SpriteComponent2D spriteComponent = ref spriteComponents[entity.Id];

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
        /// Hook de actualización compartida (no usado en este sistema).
        /// <para>Shared entity update hook (unused in this system).</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        /// <param name="time">Tiempo de juego. <para>Game time.</para></param>
        public override void SharedEntityForEachUpdate(ref Yotsuba Entidad, GameTime time)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Hook de inicialización compartida (no usado en este sistema).
        /// <para>Shared entity initialization hook (unused in this system).</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        public override void SharedEntityInitialize(ref Yotsuba Entidad)
        {
            //throw new NotImplementedException();
        }

        public override void Dispose()
        {
        }
    }
}
