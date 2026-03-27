using System.Runtime.CompilerServices;
using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    public unsafe struct CameraSystem
    {
        private nint _emPtr;
        public Matrix ViewMatrix;
        public Matrix ProjectionMatrix;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InitializeSystem(EntityManager* entities)
        {
            _emPtr = (nint)entities;
            ViewMatrix = Matrix.Identity;
            ProjectionMatrix = Matrix.Identity;
        }

        public void UpdateSystem(GameTime gameTime)
        {
            EntityManager* em = (EntityManager*)_emPtr;
            if (em == null) return;

            int camId = em->CameraEntityId;
            if (camId < 0) return;

            ref TransformComponent camT = ref Unsafe.Add(
                ref Unsafe.AsRef<TransformComponent>(em->TransformComponents.Ptr), camId);

            float zoom = em->CameraZoom;
            if (zoom <= 0.001f) zoom = 0.001f;

            float vpW = 1920f, vpH = 1080f;
            Unsafe.SkipInit(out Vector2 screenCenter);
            screenCenter.X = vpW / 2f;
            screenCenter.Y = vpH / 2f;

            Unsafe.SkipInit(out Vector2 camPos);
            camPos.X = camT.Position.X;
            camPos.Y = camT.Position.Y;

            ViewMatrix = Matrix.CreateTranslation(-camPos.X, -camPos.Y, 0)
                * Matrix.CreateRotationZ(0f)
                * Matrix.CreateScale(zoom)
                * Matrix.CreateTranslation(screenCenter.X, screenCenter.Y, 0);
        }

        public void SharedEntityForEachUpdate(ref Yotsuba e, GameTime t) { }
        public void SharedEntityInitialize(ref Yotsuba e) { }
        public void Dispose() { }
    }
}
