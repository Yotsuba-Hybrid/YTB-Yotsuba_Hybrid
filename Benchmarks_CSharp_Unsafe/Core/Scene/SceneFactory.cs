using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Benchmarks.Core.Collections;
using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Scene
{
    /// <summary>
    /// Factory using XorShift32 PRNG and FixedString32 names.
    /// Zero heap allocations during entity creation.
    /// </summary>
    public static unsafe class SceneFactory
    {
        public static nint CreateTestScene(int entityCount)
        {
            EventManager.Reset();
            nint scenePtr = SceneStruct.Create();
            SceneStruct* scene = (SceneStruct*)scenePtr;
            PopulateEntities(scene->EmPtr, entityCount);
            scene->Initialize();
            return scenePtr;
        }

        public static EntityManager* CreateTestEntityManager(int entityCount)
        {
            EntityManager* em = EntityManager.Create();
            PopulateEntities((nint)em, entityCount);
            return em;
        }

        private static void PopulateEntities(nint emNint, int entityCount)
        {
            EntityManager* em = (EntityManager*)emNint;
            var rng = new XorShift32(42);

            int worldEntities = (int)(entityCount * 0.70);
            int npcEntities = (int)(entityCount * 0.10);
            int uiEntities = (int)(entityCount * 0.10);
            int shaderEntities = (int)(entityCount * 0.05);
            int playerEntities = entityCount - worldEntities - npcEntities - uiEntities - shaderEntities;

            int id = 0;

            // 70% - World entities: Transform + Sprite + RigidBody
            for (int i = 0; i < worldEntities; i++)
            {
                Unsafe.SkipInit(out Yotsuba entity);
                entity.Id = id;
                entity.Name = FixedString32.FromInt("World_", id);
                entity.Components = 0;
                em->AddEntity(ref entity);

                em->AddTransformComponent(entity, new TransformComponent(
                    new Vector3(rng.Next(-1000, 1000), rng.Next(-1000, 1000), rng.NextSingle()),
                    new Vector3(rng.Next(1, 4) * 32, rng.Next(1, 4) * 32, 0),
                    rng.NextSingle() * 2 + 0.5f,
                    SpriteEffects.None,
                    Color.White
                ));

                em->AddSpriteComponent(entity, new SpriteComponent2D(
                    rng.Next(1, 100),
                    new Rectangle(0, 0, 32, 32)
                ) { IsVisible = true });

                var gameType = rng.Next(0, 2) == 0 ? GameType.TopDown : GameType.Platform;
                em->AddRigidbodyComponent(entity, new RigidBodyComponent2D(gameType, MassLevel.Collision)
                {
                    Velocity = new Vector3(rng.NextSingle() * 2 - 1, rng.NextSingle() * 2 - 1, 0)
                });

                id++;
            }

            // 10% - NPC entities: Transform + Sprite + Animation + RigidBody
            for (int i = 0; i < npcEntities; i++)
            {
                Unsafe.SkipInit(out Yotsuba entity);
                entity.Id = id;
                entity.Name = FixedString32.FromInt("NPC_", id);
                entity.Components = 0;
                em->AddEntity(ref entity);

                em->AddTransformComponent(entity, new TransformComponent(
                    new Vector3(rng.Next(-1000, 1000), rng.Next(-1000, 1000), rng.NextSingle()),
                    new Vector3(64, 64, 0),
                    1f, SpriteEffects.None, Color.White
                ));

                em->AddSpriteComponent(entity, new SpriteComponent2D(
                    rng.Next(1, 100),
                    new Rectangle(0, 0, 64, 64)
                ) { IsVisible = true });

                Unsafe.SkipInit(out AnimationComponent2D animComp);
                animComp = default;

                Unsafe.SkipInit(out AnimationData idleAnim);
                idleAnim.CurrentFrame = 0;
                idleAnim.TotalFrames = 8;
                idleAnim.FrameTime = 0.1f;
                idleAnim.ElapsedTime = 0f;
                idleAnim.IsFinished = false;
                idleAnim.FinishedWasMarked = false;
                idleAnim.FrameWidth = 64;
                idleAnim.FrameHeight = 64;
                idleAnim.TextureId = rng.Next(1, 100);
                idleAnim.Loop = true;
                animComp.AddAnimation(AnimationType.Idle, idleAnim);

                Unsafe.SkipInit(out AnimationData walkAnim);
                walkAnim.CurrentFrame = 0;
                walkAnim.TotalFrames = 6;
                walkAnim.FrameTime = 0.12f;
                walkAnim.ElapsedTime = 0f;
                walkAnim.IsFinished = false;
                walkAnim.FinishedWasMarked = false;
                walkAnim.FrameWidth = 64;
                walkAnim.FrameHeight = 64;
                walkAnim.TextureId = rng.Next(1, 100);
                walkAnim.Loop = true;
                animComp.AddAnimation(AnimationType.Walk, walkAnim);
                animComp.ActivateAnimation(AnimationType.Idle);

                em->AddAnimationComponent(entity, animComp);

                em->AddRigidbodyComponent(entity, new RigidBodyComponent2D(GameType.TopDown, MassLevel.Collision)
                {
                    Velocity = new Vector3(rng.NextSingle() - 0.5f, rng.NextSingle() - 0.5f, 0)
                });

                id++;
            }

            // 10% - UI entities: Transform + Sprite + Button2D
            for (int i = 0; i < uiEntities; i++)
            {
                Unsafe.SkipInit(out Yotsuba entity);
                entity.Id = id;
                entity.Name = FixedString32.FromInt("UI_", id);
                entity.Components = 0;
                em->AddEntity(ref entity);

                int bx = rng.Next(0, 1800);
                int by = rng.Next(0, 1000);
                int bw = rng.Next(50, 200);
                int bh = rng.Next(30, 80);

                em->AddTransformComponent(entity, new TransformComponent(
                    new Vector3(bx, by, 0.9f),
                    new Vector3(bw, bh, 0),
                    1f, SpriteEffects.None, Color.White
                ));

                em->AddSpriteComponent(entity, new SpriteComponent2D(
                    rng.Next(1, 100),
                    new Rectangle(0, 0, bw, bh)
                ) { IsVisible = true });

                em->AddButtonComponent2D(entity, new ButtonComponent2D
                {
                    IsActive = true,
                    EffectiveArea = new Rectangle(bx, by, bw, bh),
                    ActionPtr = ButtonComponent2D.NoOpPtr,
                });

                id++;
            }

            // 5% - Shader entities: Transform + Sprite + Shader
            for (int i = 0; i < shaderEntities; i++)
            {
                Unsafe.SkipInit(out Yotsuba entity);
                entity.Id = id;
                entity.Name = FixedString32.FromInt("Shader_", id);
                entity.Components = 0;
                em->AddEntity(ref entity);

                em->AddTransformComponent(entity, new TransformComponent(
                    new Vector3(rng.Next(-1000, 1000), rng.Next(-1000, 1000), rng.NextSingle()),
                    new Vector3(128, 128, 0),
                    1f, SpriteEffects.None, Color.White
                ));

                em->AddSpriteComponent(entity, new SpriteComponent2D(
                    rng.Next(1, 100),
                    new Rectangle(0, 0, 128, 128)
                ) { IsVisible = true });

                em->AddShaderComponent2D(entity, new ShaderComponent(rng.Next(1, 10), true));

                id++;
            }

            // 5% - Player entities: Transform + Sprite + Input + RigidBody
            for (int i = 0; i < playerEntities; i++)
            {
                Unsafe.SkipInit(out Yotsuba entity);
                entity.Id = id;
                entity.Name = FixedString32.FromInt("Player_", id);
                entity.Components = 0;
                em->AddEntity(ref entity);

                em->AddTransformComponent(entity, new TransformComponent(
                    new Vector3(rng.Next(-500, 500), rng.Next(-500, 500), 0.5f),
                    new Vector3(48, 48, 0),
                    1f, SpriteEffects.None, Color.White
                ));

                em->AddSpriteComponent(entity, new SpriteComponent2D(
                    rng.Next(1, 100),
                    new Rectangle(0, 0, 48, 48)
                ) { IsVisible = true });

                Unsafe.SkipInit(out InputComponent inputComp);
                inputComp = default;
                inputComp.SetKeyBoardMapping(ActionEntityInput.MoveUp, 0);
                inputComp.SetKeyBoardMapping(ActionEntityInput.MoveDown, 1);
                inputComp.SetKeyBoardMapping(ActionEntityInput.MoveLeft, 2);
                inputComp.SetKeyBoardMapping(ActionEntityInput.MoveRight, 3);
                inputComp.SetKeyBoardMapping(ActionEntityInput.Jump, 4);
                inputComp.AddInput(InputInUse.HasKeyboard);
                em->AddInputComponent(entity, inputComp);

                em->AddRigidbodyComponent(entity, new RigidBodyComponent2D(GameType.Platform, MassLevel.Collision)
                {
                    Velocity = new Vector3(0, 0, 0),
                    SPEED = 2.0f,
                    TOP_SPEED = 5.0f,
                });

                if (i == 0)
                {
                    em->CameraEntityId = entity.Id;
                    em->CameraZoom = 1.0f;
                }

                id++;
            }
        }
    }
}
