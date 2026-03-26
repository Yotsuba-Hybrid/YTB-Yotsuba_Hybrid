using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Systems.Contract;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    /// <summary>
    /// Sistema de renderizado 2D. Replica el multi-pass rendering del engine.
    /// En vez de SpriteBatch.Draw, acumula en un counter para simular el trabajo equivalente.
    /// </summary>
    public class RenderSystem2D : IRenderSystem
    {
        public int DrawCallCount { get; private set; }
        public int CulledCount { get; private set; }

        private EventManager EventManager { get; set; }

        public override void InitializeSystem(EntityManager entities)
        {
            EventManager = EventManager.Instance;
            EntityManager = entities;
        }

        public override void UpdateSystem(GameTime gameTime)
        {
            DrawCallCount = 0;
            CulledCount = 0;

            EntityManager entityManager = EntityManager;
            if (entityManager is null) return;

            int cameraEntityId = entityManager.CameraEntityId;
            Matrix viewMatrix = Matrix.Identity;
            Rectangle cameraWorldBounds = Rectangle.Empty;

            Span<TransformComponent> transformComponents = GetTransformComponentsAsSpan();

            if (cameraEntityId >= 0)
            {
                ref readonly var camTransform = ref transformComponents[cameraEntityId];

                float currentZoom = entityManager.CameraZoom;
                if (currentZoom <= 0.001f) currentZoom = 0.001f;

                Vector2 offset = Vector2.Zero;
                float viewportWidth = 1920f;
                float viewportHeight = 1080f;

                Vector2 screenCenter = new Vector2(viewportWidth / 2f, viewportHeight / 2f);

                // Simulate Get2DViewMatrix calculation
                Vector2 camPos = new Vector2(camTransform.Position.X, camTransform.Position.Y) + offset;
                viewMatrix = Matrix.CreateTranslation(-camPos.X, -camPos.Y, 0)
                    * Matrix.CreateScale(currentZoom)
                    * Matrix.CreateTranslation(screenCenter.X, screenCenter.Y, 0);

                // Frustum culling bounds
                float visibleWorldWidth = viewportWidth / currentZoom;
                float visibleWorldHeight = viewportHeight / currentZoom;

                Vector2 cameraCenter = new Vector2(camTransform.Position.X, camTransform.Position.Y) + offset;
                float camLeft = cameraCenter.X - (visibleWorldWidth / 2f);
                float camTop = cameraCenter.Y - (visibleWorldHeight / 2f);

                int padding = 100;
                cameraWorldBounds = new Rectangle(
                    (int)(camLeft - padding),
                    (int)(camTop - padding),
                    (int)(visibleWorldWidth + (padding * 2)),
                    (int)(visibleWorldHeight + (padding * 2))
                );
            }
            else
            {
                cameraWorldBounds = new Rectangle(0, 0, 1920, 1080);
            }

            Span<SpriteComponent2D> spriteComponent2Ds = entityManager.Sprite2DComponents.AsSpan();
            Span<Yotsuba> SpanEntities = entityManager.YotsubaEntities.AsSpan();
            Span<ShaderComponent> shaderComponents = entityManager.ShaderComponents.AsSpan();

            // --- PASS 1: UI Elements (absolute screen coordinates) ---
            foreach (ref Yotsuba entity in SpanEntities)
            {
                if (!entity.HasComponent(YTBComponent.YTBUIElement)) continue;

                ref readonly SpriteComponent2D Sprite = ref spriteComponent2Ds[entity.Id];
                if (!Sprite.IsVisible || Sprite.Is2_5D) continue;

                ref readonly TransformComponent Transform = ref transformComponents[entity.Id];

                Rectangle destinationRect = new Rectangle(
                    (int)Transform.Position.X,
                    (int)Transform.Position.Y,
                    (int)Transform.Size.X,
                    (int)Transform.Size.Y
                );

                // Simulate draw call
                DrawCallCount++;
            }

            // --- PASS 2: World Sprites (affected by camera, with culling) ---
            foreach (ref Yotsuba entity in SpanEntities)
            {
                if ((!entity.HasComponent(YTBComponent.Sprite) || !entity.HasComponent(YTBComponent.Transform)) || entity.HasComponent(YTBComponent.Button2D)) continue;

                if (entity.HasComponent(YTBComponent.Shader))
                {
                    ShaderComponent shaderComp = shaderComponents[entity.Id];
                    if (shaderComp.IsActive && shaderComp.ShaderId != 0)
                        continue;
                }

                ref readonly SpriteComponent2D Sprite = ref spriteComponent2Ds[entity.Id];
                if (!Sprite.IsVisible || Sprite.Is2_5D) continue;

                ref readonly TransformComponent Transform = ref transformComponents[entity.Id];

                float entWidth = Transform.Size.X * Transform.Scale;
                float entHeight = Transform.Size.Y * Transform.Scale;

                Rectangle entityBounds = new Rectangle(
                    (int)(Transform.Position.X - entWidth / 2f),
                    (int)(Transform.Position.Y - entHeight / 2f),
                    (int)entWidth,
                    (int)entHeight
                );

                if (!cameraWorldBounds.Intersects(entityBounds))
                {
                    CulledCount++;
                    continue;
                }

                // Simulate draw call (equivalent work to spriteBatch.Draw)
                DrawCallCount++;
            }

            // --- PASS 3: Shader Entities ---
            foreach (ref Yotsuba entity in SpanEntities)
            {
                if ((!entity.HasComponent(YTBComponent.Sprite) || !entity.HasComponent(YTBComponent.Transform)) || entity.HasComponent(YTBComponent.Button2D)) continue;
                if (!entity.HasComponent(YTBComponent.Shader)) continue;

                ref readonly ShaderComponent shaderComp = ref shaderComponents[entity.Id];
                if (!shaderComp.IsActive || shaderComp.ShaderId == 0) continue;

                ref readonly SpriteComponent2D Sprite = ref spriteComponent2Ds[entity.Id];
                if (!Sprite.IsVisible || Sprite.Is2_5D) continue;

                ref readonly TransformComponent Transform = ref transformComponents[entity.Id];

                float entWidth = Transform.Size.X * Transform.Scale;
                float entHeight = Transform.Size.Y * Transform.Scale;
                Rectangle entityBounds = new Rectangle(
                    (int)(Transform.Position.X - entWidth / 2f),
                    (int)(Transform.Position.Y - entHeight / 2f),
                    (int)entWidth,
                    (int)entHeight
                );

                if (!cameraWorldBounds.Intersects(entityBounds))
                {
                    CulledCount++;
                    continue;
                }

                // Simulate shader draw call (separate Begin/End per entity)
                DrawCallCount++;
            }

            // --- PASS 4: Buttons/HUD (absolute screen coordinates) ---
            foreach (ref Yotsuba entity in SpanEntities)
            {
                if ((!entity.HasComponent(YTBComponent.Sprite) || !entity.HasComponent(YTBComponent.Transform)) || !entity.HasComponent(YTBComponent.Button2D) || entity.HasComponent(YTBComponent.YTBUIElement)) continue;

                ref readonly SpriteComponent2D Sprite = ref spriteComponent2Ds[entity.Id];
                if (!Sprite.IsVisible || Sprite.Is2_5D) continue;

                ref readonly TransformComponent Transform = ref transformComponents[entity.Id];

                // Simulate draw call
                DrawCallCount++;
            }
        }

        public override void Dispose() { }
    }
}
