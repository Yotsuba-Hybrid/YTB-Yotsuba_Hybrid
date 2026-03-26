using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Systems.Contract;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    /// <summary>
    /// Sistema encargado de la camara.
    /// Replica el calculo de matrices de vista del engine.
    /// </summary>
    public class CameraSystem : ISystem
    {
        public Matrix ViewMatrix { get; private set; } = Matrix.Identity;
        public Matrix ProjectionMatrix { get; private set; } = Matrix.Identity;

        public override void InitializeSystem(EntityManager @entities)
        {
            EntityManager = @entities;
        }

        public override void UpdateSystem(GameTime gameTime)
        {
            if (EntityManager == null) return;

            int cameraEntityId = EntityManager.CameraEntityId;
            if (cameraEntityId < 0) return;

            Span<TransformComponent> transforms = GetTransformComponentsAsSpan();
            ref readonly TransformComponent camTransform = ref transforms[cameraEntityId];

            float zoom = EntityManager.CameraZoom;
            if (zoom <= 0.001f) zoom = 0.001f;

            float viewportWidth = 1920f;
            float viewportHeight = 1080f;
            Vector2 screenCenter = new Vector2(viewportWidth / 2f, viewportHeight / 2f);
            Vector2 camPos = new Vector2(camTransform.Position.X, camTransform.Position.Y);

            // Simulate view matrix calculation
            ViewMatrix = Matrix.CreateTranslation(-camPos.X, -camPos.Y, 0)
                * Matrix.CreateRotationZ(0f)
                * Matrix.CreateScale(zoom)
                * Matrix.CreateTranslation(screenCenter.X, screenCenter.Y, 0);
        }

        public override void SharedEntityForEachUpdate(ref Yotsuba Entidad, GameTime time) { }
        public override void SharedEntityInitialize(ref Yotsuba Entidad) { }
        public override void Dispose() { }
    }
}
