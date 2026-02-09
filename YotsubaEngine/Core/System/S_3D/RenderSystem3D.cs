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
using Num = System.Numerics;
namespace YotsubaEngine.Core.System.S_3D
{
    /// <summary>
    /// Sistema que gestiona todo lo que se ve en pantalla y que sea 3D especificamente, renderizar modelos 3D.
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
        public void UpdateSystem(GameTime gameTime)
        {


#if YTB
            if (OperatingSystem.IsWindows())
			if (GameWontRun.GameWontRunByException) return;
#endif

            CreateObjects3DExaples();

            ref var entities = ref EntityManager.YotsubaEntities;
            ref var Models = ref EntityManager.ModelComponents3D;

            // TODO: 3D rendering is currently incomplete (Coming Soon)
            // Skip if no 3D models are loaded to avoid errors
            if (Models == null || Models.Count == 0) return;

            CameraComponent3D camera = EntityManager.Camera;
            camera.Update();
            foreach (var entity in entities)
            {
                //ref var storageObjects3D = ref EntityManager.StorageObjectS3D[entity];
                //if(!storageObjects3D.IsVisible) continue;
                //foreach (var entityObject3D in storageObjects3D.Object3Ds)
                //{

                if (entity.HasNotComponent(YTBComponent.Model3D)) continue;
                ref var model = ref Models[entity.Id];
                camera.DrawModel(model, ref EntityManager.TransformComponents[entity.Id], entity.HasComponent(YTBComponent.Shader) ? EntityManager.ShaderComponents[entity.Id] : null);
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

        public void SharedEntityForEachUpdate(Yotsuba Entidad, GameTime time)
        {
            // 3D render system does not require per-entity updates; handled in UpdateSystem
        }

        public void SharedEntityInitialize(Yotsuba Entidad)
        {
            // 3D entity initialization is currently handled externally (Coming Soon)
        }
    }
}
