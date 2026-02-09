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
    /// Provides helper calls for common engine actions.
    /// Proporciona llamadas auxiliares para acciones comunes del motor.
    /// </summary>
    public abstract class SystemCall
    {
        /// <summary>
        /// Metodo que setea una animacion de una entidad.
        /// Devuelve true si existe esa animacion, de lo contrario, false,
        /// </summary>
        /// <returns></returns>
        public static void ActivateAnimation(Yotsuba entity, AnimationType animationType, bool IsLooping = true) => ActivateAnimation(entity.Id, animationType, IsLooping);

        /// <summary>
        /// Metodo que setea una animacion de una entidad.
        /// Devuelve true si existe esa animacion, de lo contrario, false,
        /// </summary>
        /// <returns></returns>
        public static void ActivateAnimation(int entityId, AnimationType animationType, bool IsLooping)
        {
            YTBGame game = (YTBGame)YTBGame.Instance;
            Scene? current = game.SceneManager.CurrentScene;
            ref var animation = ref current.EntityManager.Animation2DComponents[entityId];

            animation.GetAnimation(animationType).IsLooping = IsLooping;

            EventManager.Instance.Publish(new AnimationChangeEvent(entityId, animationType));

        }

        /// <summary>
        /// Metodo para cambiar de scena en el juego
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
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
#if YTB
                EventManager.Instance.Publish(new OnShowGameUIHiddeEngineEditor(true, false));
#endif
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

#if YTB
                EventManager.Instance.Publish(new OnShowGameUIHiddeEngineEditor(false, true));
#endif
            };

            EventManager.Instance.EventManagerWasPaused += handler;
            return true;
        }

        /// <summary>
        /// Metodo que ajusta el tamaño del CollisionBox al tamaño del Sprite de la entidad.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
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
