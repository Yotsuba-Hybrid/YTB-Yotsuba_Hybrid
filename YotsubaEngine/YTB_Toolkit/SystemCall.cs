#nullable enable

using Microsoft.Xna.Framework;
using System;
using System.Linq;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Audio;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events.YTBEvents;
using YotsubaEngine.Events.YTBEvents.EngineEvents;

namespace YotsubaEngine.YTB_Toolkit
{
    /// <summary>
    /// Proporciona llamadas auxiliares para acciones comunes del motor.
    /// <para>Provides helper calls for common engine actions.</para>
    /// </summary>
    public abstract class SystemCall
    {
        /// <summary>
        /// Activa una animación de una entidad.
        /// <para>Activates an entity animation.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <param name="animationType">Tipo de animación. <para>Animation type.</para></param>
        /// <param name="IsLooping">Indica si la animación hace loop. <para>Whether the animation is looping.</para></param>
        public static void ActivateAnimation(Yotsuba entity, AnimationType animationType, bool IsLooping = true) => ActivateAnimation(entity.Id, animationType, IsLooping);

        /// <summary>
        /// Activa una animación de una entidad.
        /// <para>Activates an entity animation.</para>
        /// </summary>
        /// <param name="entityId">Identificador de la entidad. <para>Entity identifier.</para></param>
        /// <param name="animationType">Tipo de animación. <para>Animation type.</para></param>
        /// <param name="IsLooping">Indica si la animación hace loop. <para>Whether the animation is looping.</para></param>
        public static void ActivateAnimation(int entityId, AnimationType animationType, bool IsLooping)
        {
            YTBGame game = (YTBGame)YTBGame.Instance;
            Scene? current = game.SceneManager.CurrentScene;
            ref var animation = ref current.EntityManager.Animation2DComponents[entityId];

            animation.GetAnimation(animationType).IsLooping = IsLooping;

            EventManager.Instance.Publish(new AnimationChangeEvent(entityId, animationType));

        }

        /// <summary>
        /// Método para cambiar de escena en el juego.
        /// <para>Changes the current scene in the game.</para>
        /// </summary>
        /// <param name="sceneName">Nombre de la escena. <para>Scene name.</para></param>
        /// <returns>True si la escena existe y se solicita el cambio; de lo contrario, false. <para>True if the scene exists and the change is requested; otherwise, false.</para></returns>
        public static bool ChangeScene(string sceneName)
        {
            EventManager.Instance.Publish(new StopEvents());

            YTBGame game = (YTBGame)YTBGame.Instance;
            Scene? newScene = game.SceneManager.Scenes
                .FirstOrDefault(x => x.SceneName == sceneName);

            if (newScene == null)
                return false;

            Action handler = null!;

            handler = () =>
            {
                EventManager.Instance.EventManagerWasPaused -= handler;

                game.SceneManager.CurrentScene = newScene;
                newScene.Initialize(YTBFileToGameData.ContentManager);
                EventManager.StopEvents = false;
//-:cnd:noEmit
#if YTB
                EventManager.Instance.Publish(new OnShowGameUIHiddeEngineEditor(true, false));
#endif
//+:cnd:noEmit
            };

            EventManager.Instance.EventManagerWasPaused += handler;
            return true;
        }

        /// <summary>
        /// Metodo para cambiar de scena en el juego
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        internal static bool ChangeScenePreview(string sceneName)
        {
            EventManager.Instance.Publish(new StopEvents());

            YTBGame game = (YTBGame)YTBGame.Instance;
            Scene? newScene = game.SceneManager.Scenes
                .FirstOrDefault(x => x.SceneName == sceneName);

            if (newScene == null)
                return false;

            Action handler = null!;

            handler = () =>
            {
                EventManager.Instance.EventManagerWasPaused -= handler;

                game.SceneManager.CurrentScene = newScene;
                newScene.Initialize(YTBFileToGameData.ContentManager);
                EventManager.StopEvents = false; AudioSystem.PauseAll();

//-:cnd:noEmit
#if YTB
                EventManager.Instance.Publish(new OnShowGameUIHiddeEngineEditor(false, true));
#endif
//+:cnd:noEmit
            };

            EventManager.Instance.EventManagerWasPaused += handler;
            return true;
        }

        /// <summary>
        /// Método que ajusta el tamaño del CollisionBox al tamaño del Sprite de la entidad.
        /// <para>Adjusts the CollisionBox size to match the entity's sprite size.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        /// <returns>True si el ajuste fue aplicado; de lo contrario, false. <para>True if the adjustment was applied; otherwise, false.</para></returns>
        public static bool CollisionBoxIsSpriteSize(Yotsuba entity)
        {
            if (!entity.HasComponent(YTBComponent.Sprite) || !entity.HasComponent(YTBComponent.Transform)) return false;
            return CollisionBoxIsSpriteSize(entity.Id);
        }

        /// <summary>
        /// Metodo que ajusta el tamaño del CollisionBox al tamaño del Sprite de la entidad.
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        private static bool CollisionBoxIsSpriteSize(int entityId)
        {
            var entityManager = YTBGlobalState.Game.SceneManager.CurrentScene.EntityManager;
            ref var transform = ref entityManager.TransformComponents[entityId];
            ref var sprite = ref entityManager.Sprite2DComponents[entityId];

            float width = sprite.Texture.Width;
            float height = sprite.Texture.Height;

            transform.Size = new Vector3(width, height, transform.Size.Z);

            return true;
        }

    }
}
