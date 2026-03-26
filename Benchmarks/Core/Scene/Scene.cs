using Benchmarks.Core.Entity;
using Benchmarks.Core.Systems;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Scene
{
    /// <summary>
    /// La representacion de una escena del juego.
    /// Replica exacta del lifecycle del engine: Initialize → Update → Draw → Dispose.
    /// </summary>
    public class Scene : IDisposable
    {
        public string SceneName { get; set; }
        public EntityManager EntityManager { get; set; }
        public EventManager EventManager { get; set; }
        public SystemBuilder SystemBuilder { get; set; }

        internal FontSystem2D FontSystem2D;
        private AnimationSystem2D AnimationSystem2D;
        private ButtonSystem2D ButtonSystem2D;
        private PhysicsSystem2D PhysicsSystem2D;
        private RenderSystem2D RenderSystem2D;
        private CameraSystem CameraSystem;
        private InputSystem InputSystem;
        public ScriptSystem ScriptSystem;
        private TileMapSystem2D TilemapSystem;

        public Scene()
        {
            EventManager = EventManager.Instance;
            EntityManager = new();
            AnimationSystem2D = new AnimationSystem2D();
            ButtonSystem2D = new ButtonSystem2D();
            PhysicsSystem2D = new PhysicsSystem2D();
            RenderSystem2D = new RenderSystem2D();
            CameraSystem = new CameraSystem();
            InputSystem = new InputSystem();
            ScriptSystem = new();
            TilemapSystem = new();
            FontSystem2D = new();
            SystemBuilder = new SystemBuilder();
        }

        /// <summary>
        /// Primer metodo que se ejecuta y prepara todos los sistemas.
        /// </summary>
        public void Initialize()
        {
            AnimationSystem2D.InitializeSystem(EntityManager);
            ButtonSystem2D.InitializeSystem(EntityManager);
            PhysicsSystem2D.InitializeSystem(EntityManager);
            RenderSystem2D.InitializeSystem(EntityManager);
            CameraSystem.InitializeSystem(EntityManager);
            InputSystem.InitializeSystem(EntityManager);
            ScriptSystem.InitializeSystem(EntityManager);
            TilemapSystem.InitializeSystem(EntityManager);
            FontSystem2D.InitializeSystem(EntityManager);
            SystemBuilder.InitializeSystem(EntityManager);

            foreach (ref Yotsuba entity in EntityManager.YotsubaEntities.AsSpan())
            {
                SystemBuilder.SharedEntityInitialize(ref entity);
                ScriptSystem.SharedEntityInitialize(ref entity);
                FontSystem2D.SharedEntityInitialize(ref entity);
            }
        }

        /// <summary>
        /// Metodo que se ejecuta en cada frame para actualizar el estado del juego.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            InputSystem.UpdateSystem(gameTime);

            if (EntityManager.YotsubaEntities == null || EntityManager.YotsubaEntities.Count == 0) return;
            PhysicsSystem2D.UpdateSystem(gameTime);
            ButtonSystem2D.UpdateSystem(gameTime);
            AnimationSystem2D.UpdateSystem(gameTime);
            CameraSystem.UpdateSystem(gameTime);
            SystemBuilder.UpdateSystem(gameTime);

            foreach (ref Yotsuba entity in EntityManager.YotsubaEntities.AsSpan())
            {
                ScriptSystem.SharedEntityForEachUpdate(ref entity, gameTime);
                SystemBuilder.SharedEntityForEachUpdate(ref entity, gameTime);
            }
        }

        /// <summary>
        /// Metodo que se ejecuta en cada frame para dibujar la escena.
        /// </summary>
        public void Draw(GameTime gameTime)
        {
            if (EntityManager.YotsubaEntities != null && EntityManager.YotsubaEntities.Count > 0)
            {
                Rectangle cameraBounds = new Rectangle(0, 0, 1920, 1080);
                TilemapSystem.UpdateSystem(gameTime, cameraBounds);

                RenderSystem2D.UpdateSystem(gameTime);

                FontSystem2D.DrawSystem(gameTime);

                // Draw3D equivalent
                ScriptSystem.DrawSystem3D(gameTime);
                SystemBuilder.Render3D(gameTime);

                // Draw2D equivalent
                ScriptSystem.DrawSystem2D(gameTime);
                SystemBuilder.Render2D(gameTime);
            }
        }

        public void Clear()
        {
            ScriptSystem.Clear();
            SystemBuilder.Dispose();
            AnimationSystem2D.Dispose();
            ButtonSystem2D.Dispose();
            PhysicsSystem2D.Dispose();
            CameraSystem.Dispose();
            InputSystem.Dispose();
            FontSystem2D.Dispose();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
