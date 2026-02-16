using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_3D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Exceptions;
using YotsubaEngine.Graphics;
using YotsubaEngine.HighestPerformanceTypes;
using YotsubaEngine.Input;
using Num = System.Numerics;
using static YotsubaEngine.Core.System.S_AGNOSTIC.InputSystem;
namespace YotsubaEngine.Core.System.S_3D
{
    /// <summary>
    /// Sistema que gestiona todo lo que se ve en pantalla y que sea 3D específicamente, renderizar modelos 3D.
    /// <para>System that manages everything rendered in 3D, including 3D models.</para>
    /// </summary>
    public class RenderSystem3D : ISystem
    {
        /// <summary>
        /// Referencia al EventManager para manejar eventos.
        /// </summary>
        private EventManager EventManager { get; set; }

        /// <summary>
        /// Referencia al EntityManager para manejar entidades y componentes.
        /// </summary>
        private EntityManager EntityManager { get; set; }

        /// <summary>
        /// Referencia a la clase Graphics3D para realizar operaciones de renderizado 3D.
        /// </summary>
        private Graphics3D Graphics3D;

        private Texture2D Texture2D;

        
        /// <summary>
        /// Inicializa el sistema de renderizado 3D.
        /// <para>Initializes the 3D render system.</para>
        /// </summary>
        /// <param name="entities">Administrador de entidades. <para>Entity manager.</para></param>
        public void InitializeSystem(EntityManager entities)
        {
            

#if YTB
            if (GameWontRun.GameWontRunByException) return;
#endif

            EntityManager = entities;

            Graphics3D = new();
			EventManager = EventManager.Instance;
            EngineUISystem.SendLog(typeof(RenderSystem3D).Name + " Se inicio correctamente");
            Texture2D = YTBGlobalState.ContentManager.Load<Texture2D>("moho/mohoanimation_00001");

        }

        void CreateObjects3DExaples()
        {
            if (EntityManager == null) return;
            Yotsuba storageObjectS3DEntity = new(0);
            ListObject3D storageObjectS3D = new();
            storageObjectS3D.IsVisible = true;
            for (int i = 0; i < 1; i++)
            {
                Object3D object3D = new();
                TransformComponent transform = new();
                transform.SetPosition(1300, -200, 750);

                float red = YTBGlobalState.Random.Next(0, 255) / 255f;
                float blue = YTBGlobalState.Random.Next(0, 255) / 255f;
                float green = YTBGlobalState.Random.Next(0, 255) / 255f;

                float alpha = (float)YTBGlobalState.Random.NextDouble();
                transform.Color = new Num.Vector4(red, green, blue, alpha);
                transform.Size = new Num.Vector3(1920, 1080, 800);
                transform.Rotation = 0f;

                Yotsuba EntidadObj = new(0);
                EntityManager.AddEntity(EntidadObj);
                EntityManager.AddTransformComponent(EntidadObj, transform);
                storageObjectS3D.Object3Ds.Add(EntidadObj.Id);
            }

            EntityManager.AddEntity(storageObjectS3DEntity);
            EntityManager.AddStorageObject3D(storageObjectS3DEntity, storageObjectS3D);
        }
        /// <summary>
        /// Actualiza el renderizado 3D en cada frame.
        /// <para>Updates 3D rendering each frame.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public void UpdateSystem(GameTime gameTime)
        {


#if YTB
            if (OperatingSystem.IsWindows())
			if (GameWontRun.GameWontRunByException) return;
#endif

            ref var entities = ref EntityManager.YotsubaEntities;
            ref var Models = ref EntityManager.ModelComponents3D;

            // TODO: 3D rendering is currently incomplete (Coming Soon)
            // Skip if no 3D models are loaded to avoid errors
            if (Models == null || Models.Count == 0) return;

            CameraComponent3D camera = EntityManager.Camera;
            camera.Update();

#if YTB
            // Selección múltiple de modelos 3D en modo engine via ray picking
            if (YTBGlobalState.EngineShortcutsMode)
            {
                MouseInfo mouse = InputManager.Instance.Mouse;
                KeyboardInfo keyboard = InputManager.Instance.Keyboard;
                // Click izquierdo: toggle selección del modelo bajo el cursor
                if (mouse.WasButtonJustPressed(MouseButton.Left) && keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                {
                    Viewport viewport = YTBGlobalState.GraphicsDevice.Viewport;
                    Vector3 nearPoint = viewport.Unproject(
                        new Vector3(mouse.X, mouse.Y, 0f),
                        camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
                    Vector3 farPoint = viewport.Unproject(
                        new Vector3(mouse.X, mouse.Y, 1f),
                        camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);

                    Vector3 direction = farPoint - nearPoint;
                    direction.Normalize();
                    Ray ray = new Ray(nearPoint, direction);

                    float closestDistance = float.MaxValue;
                    int closestEntityId = -1;

                    foreach (var entity in entities)
                    {
                        if (entity.HasNotComponent(YTBComponent.Model3D)) continue;
                        ref ModelComponent3D model = ref Models[entity];
                        ref TransformComponent transform = ref EntityManager.TransformComponents[entity];

                        float sphereRadius = model.RadiusSphere;
                        foreach (ModelMesh mesh in model.Model.Meshes)
                        {
                            BoundingSphere sphere = mesh.BoundingSphere;
                            sphere = sphere.Transform(
                                Matrix.CreateTranslation(transform.Position))
                                ;
                            if(sphereRadius >= 0)
                            {
                                sphere.Radius = sphereRadius;
                            }

                            sphere.Center += model.SphereOffset;

                            float? dist = ray.Intersects(sphere);
                            if (dist.HasValue && dist.Value < closestDistance)
                            {
                                closestDistance = dist.Value;
                                closestEntityId = entity.Id;
                            }
                        }
                    }

                    if (closestEntityId != -1)
                    {
                        var selected = YTBGlobalState.SelectedModel3DEntityIds;
                        if (selected.Contains(closestEntityId))
                        {
                            selected.Remove(closestEntityId);
                            EngineUISystem._instance?.ShowModeSwitchAlert("Modelo deseleccionado");
                        }
                        else
                        {
                            selected.Add(closestEntityId);
                            EngineUISystem._instance?.ShowModeSwitchAlert(
                                $"{selected.Count} modelo(s) seleccionado(s)");
                        }
                    }
                }
            }
            else
            {
                // Al salir de modo engine, limpiar selección
                YTBGlobalState.SelectedModel3DEntityIds.Clear();
            }
#endif

            foreach (var entity in entities)
            {
                if (entity.HasNotComponent(YTBComponent.Model3D)) continue;
                ref var model = ref Models[entity.Id];
                camera.DrawModel(model, ref EntityManager.TransformComponents[entity.Id],
                    entity.HasComponent(YTBComponent.Shader) ? EntityManager.ShaderComponents[entity.Id] : null,
                    entity.Id);
            }

            foreach (var entity in entities)
            {
                if (!entity.HasComponent(YTBComponent.Sprite)) continue;

                ref SpriteComponent2D sprite = ref EntityManager.Sprite2DComponents[entity];
                if (!sprite.Is2_5D) continue;

                ref TransformComponent transform = ref EntityManager.TransformComponents[entity];
                Graphics3D.DrawSprite2_5D(sprite, transform.Position, transform.Color, camera.ViewMatrix, camera.ProjectionMatrix, transform.Rotation);
            }
        }

        /// <summary>
        /// Hook de actualización compartida por entidad (no usado).
        /// <para>Shared per-entity update hook (unused).</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        /// <param name="time">Tiempo de juego. <para>Game time.</para></param>
        public void SharedEntityForEachUpdate(Yotsuba Entidad, GameTime time)
        {
            // 3D render system does not require per-entity updates; handled in UpdateSystem
        }

        /// <summary>
        /// Hook de inicialización compartida por entidad (no usado).
        /// <para>Shared per-entity initialization hook (unused).</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        public void SharedEntityInitialize(Yotsuba Entidad)
        {
            // 3D entity initialization is currently handled externally (Coming Soon)
        }
    }
}
