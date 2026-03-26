using Benchmarks.Core.Collections;
using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Scene
{
    /// <summary>
    /// Factory que crea escenas de test con entidades realistas para benchmarking.
    /// Distribucion: 70% mundo, 10% NPCs animados, 10% UI, 5% shaders, 5% jugadores.
    /// </summary>
    public static class SceneFactory
    {
        public static Scene CreateTestScene(int entityCount)
        {
            EventManager.Reset();
            var scene = new Scene();
            PopulateEntities(scene.EntityManager, entityCount);
            scene.Initialize();
            return scene;
        }

        public static EntityManager CreateTestEntityManager(int entityCount)
        {
            var entityManager = new EntityManager();
            PopulateEntities(entityManager, entityCount);
            return entityManager;
        }

        private static void PopulateEntities(EntityManager em, int entityCount)
        {
            var rng = new Random(42); // Deterministic seed

            int worldEntities = (int)(entityCount * 0.70);
            int npcEntities = (int)(entityCount * 0.10);
            int uiEntities = (int)(entityCount * 0.10);
            int shaderEntities = (int)(entityCount * 0.05);
            int playerEntities = entityCount - worldEntities - npcEntities - uiEntities - shaderEntities;

            int id = 0;

            // 70% - World entities: Transform + Sprite + RigidBody
            for (int i = 0; i < worldEntities; i++)
            {
                var entity = new Yotsuba(id) { Name = $"World_{id}" };
                em.AddEntity(ref entity);

                em.AddTransformComponent(entity, new TransformComponent(
                    new Vector3(rng.Next(-1000, 1000), rng.Next(-1000, 1000), rng.NextSingle()),
                    new Vector3(rng.Next(1, 4) * 32, rng.Next(1, 4) * 32, 0),
                    rng.NextSingle() * 2 + 0.5f,
                    SpriteEffects.None,
                    Color.White
                ));

                em.AddSpriteComponent(entity, new SpriteComponent2D(
                    rng.Next(1, 100),
                    new Rectangle(0, 0, 32, 32)
                ) { IsVisible = true });

                var gameType = rng.Next(2) == 0 ? GameType.TopDown : GameType.Platform;
                em.AddRigidbodyComponent(entity, new RigidBodyComponent2D(gameType, MassLevel.Collision)
                {
                    Velocity = new Vector3(rng.NextSingle() * 2 - 1, rng.NextSingle() * 2 - 1, 0)
                });

                id++;
            }

            // 10% - NPC entities: Transform + Sprite + Animation + RigidBody
            for (int i = 0; i < npcEntities; i++)
            {
                var entity = new Yotsuba(id) { Name = $"NPC_{id}" };
                em.AddEntity(ref entity);

                em.AddTransformComponent(entity, new TransformComponent(
                    new Vector3(rng.Next(-1000, 1000), rng.Next(-1000, 1000), rng.NextSingle()),
                    new Vector3(64, 64, 0),
                    1f, SpriteEffects.None, Color.White
                ));

                em.AddSpriteComponent(entity, new SpriteComponent2D(
                    rng.Next(1, 100),
                    new Rectangle(0, 0, 64, 64)
                ) { IsVisible = true });

                var animComp = new AnimationComponent2D();
                var idleAnim = new AnimationData
                {
                    CurrentFrame = 0,
                    TotalFrames = 8,
                    FrameTime = 0.1f,
                    FrameWidth = 64,
                    FrameHeight = 64,
                    TextureId = rng.Next(1, 100),
                    Loop = true,
                };
                animComp.AddAnimation(AnimationType.Idle, idleAnim);

                var walkAnim = new AnimationData
                {
                    CurrentFrame = 0,
                    TotalFrames = 6,
                    FrameTime = 0.12f,
                    FrameWidth = 64,
                    FrameHeight = 64,
                    TextureId = rng.Next(1, 100),
                    Loop = true,
                };
                animComp.AddAnimation(AnimationType.Walk, walkAnim);
                animComp.ActivateAnimation(AnimationType.Idle);

                em.AddAnimationComponent(entity, animComp);

                em.AddRigidbodyComponent(entity, new RigidBodyComponent2D(GameType.TopDown, MassLevel.Collision)
                {
                    Velocity = new Vector3(rng.NextSingle() - 0.5f, rng.NextSingle() - 0.5f, 0)
                });

                id++;
            }

            // 10% - UI entities: Transform + Sprite + Button2D
            for (int i = 0; i < uiEntities; i++)
            {
                var entity = new Yotsuba(id) { Name = $"UI_{id}" };
                em.AddEntity(ref entity);

                int bx = rng.Next(0, 1800);
                int by = rng.Next(0, 1000);
                int bw = rng.Next(50, 200);
                int bh = rng.Next(30, 80);

                em.AddTransformComponent(entity, new TransformComponent(
                    new Vector3(bx, by, 0.9f),
                    new Vector3(bw, bh, 0),
                    1f, SpriteEffects.None, Color.White
                ));

                em.AddSpriteComponent(entity, new SpriteComponent2D(
                    rng.Next(1, 100),
                    new Rectangle(0, 0, bw, bh)
                ) { IsVisible = true });

                em.AddButtonComponent2D(entity, new ButtonComponent2D
                {
                    IsActive = true,
                    EffectiveArea = new Rectangle(bx, by, bw, bh),
                    Action = () => { _ = 0; }
                });

                id++;
            }

            // 5% - Shader entities: Transform + Sprite + Shader
            for (int i = 0; i < shaderEntities; i++)
            {
                var entity = new Yotsuba(id) { Name = $"Shader_{id}" };
                em.AddEntity(ref entity);

                em.AddTransformComponent(entity, new TransformComponent(
                    new Vector3(rng.Next(-1000, 1000), rng.Next(-1000, 1000), rng.NextSingle()),
                    new Vector3(128, 128, 0),
                    1f, SpriteEffects.None, Color.White
                ));

                em.AddSpriteComponent(entity, new SpriteComponent2D(
                    rng.Next(1, 100),
                    new Rectangle(0, 0, 128, 128)
                ) { IsVisible = true });

                em.AddShaderComponent2D(entity, new ShaderComponent(rng.Next(1, 10), true));

                id++;
            }

            // 5% - Player entities: Transform + Sprite + Input + RigidBody
            for (int i = 0; i < playerEntities; i++)
            {
                var entity = new Yotsuba(id) { Name = $"Player_{id}" };
                em.AddEntity(ref entity);

                em.AddTransformComponent(entity, new TransformComponent(
                    new Vector3(rng.Next(-500, 500), rng.Next(-500, 500), 0.5f),
                    new Vector3(48, 48, 0),
                    1f, SpriteEffects.None, Color.White
                ));

                em.AddSpriteComponent(entity, new SpriteComponent2D(
                    rng.Next(1, 100),
                    new Rectangle(0, 0, 48, 48)
                ) { IsVisible = true });

                var inputComp = new InputComponent()
                {
                    KeyBoard = new Dictionary<ActionEntityInput, int>
                    {
                        { ActionEntityInput.MoveUp, 0 },
                        { ActionEntityInput.MoveDown, 1 },
                        { ActionEntityInput.MoveLeft, 2 },
                        { ActionEntityInput.MoveRight, 3 },
                        { ActionEntityInput.Jump, 4 },
                    }
                };
                inputComp.AddInput(InputInUse.HasKeyboard);
                em.AddInputComponent(entity, inputComp);

                em.AddRigidbodyComponent(entity, new RigidBodyComponent2D(GameType.Platform, MassLevel.Collision)
                {
                    Velocity = new Vector3(0, 0, 0),
                    SPEED = 2.0f,
                    TOP_SPEED = 5.0f,
                });

                // Set first player as camera target
                if (i == 0)
                {
                    em.CameraEntityId = entity.Id;
                    em.CameraZoom = 1.0f;
                }

                id++;
            }
        }
    }
}
