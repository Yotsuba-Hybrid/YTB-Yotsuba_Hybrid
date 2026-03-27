using System.Runtime.CompilerServices;
using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    public unsafe struct RenderSystem2D
    {
        private nint _emPtr;
        public int DrawCallCount;
        public int CulledCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InitializeSystem(EntityManager* entities) => _emPtr = (nint)entities;

        public void UpdateSystem(GameTime gameTime)
        {
            DrawCallCount = 0;
            CulledCount = 0;

            EntityManager* em = (EntityManager*)_emPtr;
            if (em == null) return;

            int cameraEntityId = em->CameraEntityId;
            Unsafe.SkipInit(out Matrix viewMatrix);
            viewMatrix = Matrix.Identity;
            Unsafe.SkipInit(out Rectangle cameraWorldBounds);

            TransformComponent* transforms = em->TransformComponents.Ptr;

            if (cameraEntityId >= 0)
            {
                ref TransformComponent camT = ref Unsafe.Add(ref Unsafe.AsRef<TransformComponent>(transforms), cameraEntityId);
                float zoom = em->CameraZoom;
                if (zoom <= 0.001f) zoom = 0.001f;

                float vpW = 1920f, vpH = 1080f;
                Unsafe.SkipInit(out Vector2 screenCenter);
                screenCenter.X = vpW / 2f; screenCenter.Y = vpH / 2f;

                Unsafe.SkipInit(out Vector2 camPos);
                camPos.X = camT.Position.X; camPos.Y = camT.Position.Y;

                viewMatrix = Matrix.CreateTranslation(-camPos.X, -camPos.Y, 0)
                    * Matrix.CreateScale(zoom)
                    * Matrix.CreateTranslation(screenCenter.X, screenCenter.Y, 0);

                float visW = vpW / zoom, visH = vpH / zoom;
                float camLeft = camPos.X - visW / 2f;
                float camTop = camPos.Y - visH / 2f;
                int pad = 100;
                cameraWorldBounds.X = (int)(camLeft - pad);
                cameraWorldBounds.Y = (int)(camTop - pad);
                cameraWorldBounds.Width = (int)(visW + pad * 2);
                cameraWorldBounds.Height = (int)(visH + pad * 2);
            }
            else
            {
                cameraWorldBounds = new Rectangle(0, 0, 1920, 1080);
            }

            SpriteComponent2D* sprites = em->Sprite2DComponents.Ptr;
            Yotsuba* entities = em->YotsubaEntities.Ptr;
            int count = em->YotsubaEntities.Count;
            ShaderComponent* shaders = em->ShaderComponents.Ptr;

            // PASS 1: UI Elements
            for (int i = 0; i < count; i++)
            {
                ref Yotsuba e = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
                if (!e.HasComponent(YTBComponent.YTBUIElement)) continue;
                ref SpriteComponent2D sp = ref Unsafe.Add(ref Unsafe.AsRef<SpriteComponent2D>(sprites), e.Id);
                if (!sp.IsVisible || sp.Is2_5D) continue;
                DrawCallCount++;
            }

            // PASS 2: World Sprites (with culling)
            for (int i = 0; i < count; i++)
            {
                ref Yotsuba e = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
                if ((!e.HasComponent(YTBComponent.Sprite) || !e.HasComponent(YTBComponent.Transform)) || e.HasComponent(YTBComponent.Button2D)) continue;

                if (e.HasComponent(YTBComponent.Shader))
                {
                    ref ShaderComponent sc = ref Unsafe.Add(ref Unsafe.AsRef<ShaderComponent>(shaders), e.Id);
                    if (sc.IsActive && sc.ShaderId != 0) continue;
                }

                ref SpriteComponent2D sp = ref Unsafe.Add(ref Unsafe.AsRef<SpriteComponent2D>(sprites), e.Id);
                if (!sp.IsVisible || sp.Is2_5D) continue;

                ref TransformComponent t = ref Unsafe.Add(ref Unsafe.AsRef<TransformComponent>(transforms), e.Id);
                float entW = t.Size.X * t.Scale, entH = t.Size.Y * t.Scale;
                Unsafe.SkipInit(out Rectangle bounds);
                bounds.X = (int)(t.Position.X - entW / 2f);
                bounds.Y = (int)(t.Position.Y - entH / 2f);
                bounds.Width = (int)entW; bounds.Height = (int)entH;

                if (!cameraWorldBounds.Intersects(bounds)) { CulledCount++; continue; }
                DrawCallCount++;
            }

            // PASS 3: Shader Entities
            for (int i = 0; i < count; i++)
            {
                ref Yotsuba e = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
                if ((!e.HasComponent(YTBComponent.Sprite) || !e.HasComponent(YTBComponent.Transform)) || e.HasComponent(YTBComponent.Button2D)) continue;
                if (!e.HasComponent(YTBComponent.Shader)) continue;

                ref ShaderComponent sc = ref Unsafe.Add(ref Unsafe.AsRef<ShaderComponent>(shaders), e.Id);
                if (!sc.IsActive || sc.ShaderId == 0) continue;

                ref SpriteComponent2D sp = ref Unsafe.Add(ref Unsafe.AsRef<SpriteComponent2D>(sprites), e.Id);
                if (!sp.IsVisible || sp.Is2_5D) continue;

                ref TransformComponent t = ref Unsafe.Add(ref Unsafe.AsRef<TransformComponent>(transforms), e.Id);
                float entW = t.Size.X * t.Scale, entH = t.Size.Y * t.Scale;
                Unsafe.SkipInit(out Rectangle bounds);
                bounds.X = (int)(t.Position.X - entW / 2f);
                bounds.Y = (int)(t.Position.Y - entH / 2f);
                bounds.Width = (int)entW; bounds.Height = (int)entH;

                if (!cameraWorldBounds.Intersects(bounds)) { CulledCount++; continue; }
                DrawCallCount++;
            }

            // PASS 4: Buttons/HUD
            for (int i = 0; i < count; i++)
            {
                ref Yotsuba e = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
                if ((!e.HasComponent(YTBComponent.Sprite) || !e.HasComponent(YTBComponent.Transform)) || !e.HasComponent(YTBComponent.Button2D) || e.HasComponent(YTBComponent.YTBUIElement)) continue;

                ref SpriteComponent2D sp = ref Unsafe.Add(ref Unsafe.AsRef<SpriteComponent2D>(sprites), e.Id);
                if (!sp.IsVisible || sp.Is2_5D) continue;
                DrawCallCount++;
            }
        }

        public void Render2D(GameTime gameTime) { }
        public void Render3D(GameTime gameTime) { }
        public void Dispose() { }
    }
}
