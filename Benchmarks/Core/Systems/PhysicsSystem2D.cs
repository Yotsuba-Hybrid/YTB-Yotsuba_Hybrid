using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Systems.Contract;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    /// <summary>
    /// Sistema que se encarga de toda la fisica del mundo (mover a las entidades).
    /// Replica completa de la logica del engine: gravity, collision detection N×N, DetermineCollisionDirection.
    /// </summary>
    public class PhysicsSystem2D : ISystem
    {
        private EventManager EventManager { get; set; }

        public override void InitializeSystem(EntityManager @entities)
        {
            EventManager = EventManager.Instance;
            EntityManager = @entities;
        }

        public override void UpdateSystem(GameTime gameTime)
        {
            if (EntityManager == null) return;
            Span<Yotsuba> entities = GetEntitiesAsSpan();
            Span<TransformComponent> transformComponents = GetTransformComponentsAsSpan();
            Span<RigidBodyComponent2D> rigidbodyComponents = GetRigidBodyComponentsAsSpan();

            ApplyPlatformPhysics(entities, rigidbodyComponents, gameTime);
            MoveEntities(entities, transformComponents, rigidbodyComponents, gameTime);
        }

        private void ApplyPlatformPhysics(Span<Yotsuba> entities, Span<RigidBodyComponent2D> rigidbodyComponents, GameTime gameTime)
        {
            foreach (ref Yotsuba entity in entities)
            {
                if (!entity.HasComponent(YTBComponent.Rigibody)) continue;

                ref RigidBodyComponent2D rigidBody = ref rigidbodyComponents[entity.Id];

                if (rigidBody.GameType != GameType.Platform) continue;

                float newVelocityY = rigidBody.Velocity.Y + rigidBody.Gravity;

                if (rigidBody.IsFastFalling && rigidBody.Velocity.Y >= 0)
                {
                    newVelocityY = rigidBody.Velocity.Y + (rigidBody.Gravity * rigidBody.FastFallMultiplier);
                }

                newVelocityY = Math.Min(newVelocityY, rigidBody.MaxFallSpeed);

                rigidBody.Velocity = new Vector3(rigidBody.Velocity.X, newVelocityY, rigidBody.Velocity.Z);
            }
        }

        private void MoveEntities(Span<Yotsuba> entities, Span<TransformComponent> transformComponents, Span<RigidBodyComponent2D> rigibodyComponents, GameTime gameTime)
        {
            foreach (ref Yotsuba entity in entities)
            {
                if (!entity.HasComponent(YTBComponent.Transform) || !entity.HasComponent(YTBComponent.Rigibody)) continue;

                bool collisionBottom = false;
                bool collisionTop = false;
                bool collisionLeft = false;
                bool collisionRight = false;

                ref RigidBodyComponent2D rigidBody = ref rigibodyComponents[entity.Id];
                ref TransformComponent transform = ref transformComponents[entity.Id];

                bool wasGrounded = rigidBody.IsGrounded;

                Vector2 nextPosition = new Vector2(transform.Position.X, transform.Position.Y)
                    + new Vector2(rigidBody.Velocity.X, rigidBody.Velocity.Y);

                Rectangle entityRect = new Rectangle(
                    (int)(nextPosition.X + rigidBody.OffSetCollision.X),
                    (int)(nextPosition.Y + rigidBody.OffSetCollision.Y),
                    (int)(transform.Size.X * transform.Scale),
                    (int)(transform.Size.Y * transform.Scale)
                );

                bool sizeZero = transform.Size == Vector3.Zero;

                foreach (ref Yotsuba otherEntity in entities)
                {
                    if (otherEntity.Id == entity.Id) continue;
                    if (!otherEntity.HasComponent(YTBComponent.Transform) || !otherEntity.HasComponent(YTBComponent.Rigibody)) continue;

                    ref RigidBodyComponent2D otherRigidBody = ref rigibodyComponents[otherEntity.Id];
                    ref TransformComponent otherTransform = ref transformComponents[otherEntity.Id];

                    if (otherEntity.HasComponent(YTBComponent.TileMap))
                    {
                        CheckTileMapCollision(
                            ref entity, ref otherEntity, ref rigidBody, ref otherRigidBody,
                            ref transform, ref otherTransform, entityRect, sizeZero, gameTime,
                            ref collisionBottom, ref collisionTop, ref collisionLeft, ref collisionRight);
                    }
                    else
                    {
                        CheckEntityCollision(
                            ref entity, ref otherEntity, ref rigidBody, ref otherRigidBody,
                            ref transform, ref otherTransform, entityRect, sizeZero, gameTime,
                            ref collisionBottom, ref collisionTop, ref collisionLeft, ref collisionRight);
                    }
                }

                ApplyMovement(entity.Id, ref rigidBody, ref transform, collisionBottom, collisionTop, collisionLeft, collisionRight, wasGrounded, gameTime);
            }
        }

        private void CheckTileMapCollision(
            ref Yotsuba entity, ref Yotsuba tilemapEntity,
            ref RigidBodyComponent2D rigidBody, ref RigidBodyComponent2D otherRigidBody,
            ref TransformComponent transform, ref TransformComponent otherTransform,
            Rectangle entityRect, bool sizeZero, GameTime gameTime,
            ref bool collisionBottom, ref bool collisionTop, ref bool collisionLeft, ref bool collisionRight)
        {
            if (rigidBody.Mass == MassLevel.NoCollision || otherRigidBody.Mass == MassLevel.NoCollision) return;

            ref TileMapComponent2D tilemap = ref EntityManager.TileMapComponent2Ds[tilemapEntity.Id];

            float originOffsetX = otherTransform.Size.X * 0.5f * otherTransform.Scale;
            float originOffsetY = otherTransform.Size.Y * 0.5f * otherTransform.Scale;

            foreach (ref TileLayer layer in tilemap.TileLayers.AsSpan())
            {
                bool isCollisionLayer = layer.Name.Contains("Collision", StringComparison.OrdinalIgnoreCase) || layer.Name.Contains("Collider", StringComparison.OrdinalIgnoreCase);

                for (int i = 0; i < layer.Data.Length; i++)
                {
                    int gid = layer.Data[i];
                    if (gid == 0) continue;

                    int tileX = i % tilemap.Width;
                    int tileY = i / tilemap.Width;

                    float worldX = otherTransform.Scale * (tileX * tilemap.TileWidth) + otherTransform.Position.X - originOffsetX;
                    float worldY = otherTransform.Scale * (tileY * tilemap.TileHeight) + otherTransform.Position.Y - originOffsetY;

                    bool hasNativeCollision = tilemap.Collisions.TryGetValue(gid, out List<Rectangle> nativeRects);

                    if (isCollisionLayer && !hasNativeCollision)
                    {
                        Rectangle tileRect = new Rectangle(
                            (int)worldX,
                            (int)worldY,
                            (int)(tilemap.TileWidth * otherTransform.Scale),
                            (int)(tilemap.TileHeight * otherTransform.Scale)
                        );

                        if (entityRect.Intersects(tileRect) && !sizeZero)
                        {
                            DetermineCollisionDirection(entityRect, tileRect, ref rigidBody,
                                ref collisionBottom, ref collisionTop, ref collisionLeft, ref collisionRight);
                        }
                    }

                    if (hasNativeCollision)
                    {
                        foreach (Rectangle nativeRect in nativeRects)
                        {
                            Rectangle collisionRect = new Rectangle(
                                (int)(worldX + nativeRect.X * otherTransform.Scale),
                                (int)(worldY + nativeRect.Y * otherTransform.Scale),
                                (int)(nativeRect.Width * otherTransform.Scale),
                                (int)(nativeRect.Height * otherTransform.Scale)
                            );

                            if (entityRect.Intersects(collisionRect) && !sizeZero)
                            {
                                DetermineCollisionDirection(entityRect, collisionRect, ref rigidBody,
                                    ref collisionBottom, ref collisionTop, ref collisionLeft, ref collisionRight);
                            }
                        }
                    }
                }
            }
        }

        private void CheckEntityCollision(
            ref Yotsuba entity, ref Yotsuba otherEntity,
            ref RigidBodyComponent2D rigidBody, ref RigidBodyComponent2D otherRigidBody,
            ref TransformComponent transform, ref TransformComponent otherTransform,
            Rectangle entityRect, bool sizeZero, GameTime gameTime,
            ref bool collisionBottom, ref bool collisionTop, ref bool collisionLeft, ref bool collisionRight)
        {
            if (rigidBody.Mass == MassLevel.NoCollision || otherRigidBody.Mass == MassLevel.NoCollision) return;
            if (otherTransform.Size == Vector3.Zero) return;

            Rectangle otherRect = new Rectangle(
                (int)(otherTransform.Position.X + otherRigidBody.OffSetCollision.X),
                (int)(otherTransform.Position.Y + otherRigidBody.OffSetCollision.Y),
                (int)(otherTransform.Size.X * otherTransform.Scale),
                (int)(otherTransform.Size.Y * otherTransform.Scale)
            );

            if (entityRect.Intersects(otherRect) && !sizeZero)
            {
                DetermineCollisionDirection(entityRect, otherRect, ref rigidBody,
                    ref collisionBottom, ref collisionTop, ref collisionLeft, ref collisionRight);
            }
        }

        private void DetermineCollisionDirection(
            Rectangle entityRect, Rectangle obstacleRect,
            ref RigidBodyComponent2D rigidBody,
            ref bool collisionBottom, ref bool collisionTop, ref bool collisionLeft, ref bool collisionRight)
        {
            int overlapLeft = entityRect.Right - obstacleRect.Left;
            int overlapRight = obstacleRect.Right - entityRect.Left;
            int overlapTop = entityRect.Bottom - obstacleRect.Top;
            int overlapBottom = obstacleRect.Bottom - entityRect.Top;

            int minOverlapX = Math.Min(overlapLeft, overlapRight);
            int minOverlapY = Math.Min(overlapTop, overlapBottom);

            if (minOverlapY < minOverlapX)
            {
                if (overlapTop < overlapBottom && rigidBody.Velocity.Y > 0)
                    collisionBottom = true;
                else if (overlapBottom < overlapTop && rigidBody.Velocity.Y < 0)
                    collisionTop = true;
            }
            else
            {
                if (overlapLeft < overlapRight && rigidBody.Velocity.X > 0)
                    collisionRight = true;
                else if (overlapRight < overlapLeft && rigidBody.Velocity.X < 0)
                    collisionLeft = true;
            }
        }

        private void ApplyMovement(
            int entityId,
            ref RigidBodyComponent2D rigidBody,
            ref TransformComponent transform,
            bool collisionBottom, bool collisionTop, bool collisionLeft, bool collisionRight,
            bool wasGrounded, GameTime gameTime)
        {
            Vector2 finalVelocity = new Vector2(rigidBody.Velocity.X, rigidBody.Velocity.Y);

            if (collisionBottom)
            {
                finalVelocity.Y = 0;
                rigidBody.IsGrounded = true;
                rigidBody.IsJumping = false;
                rigidBody.IsFastFalling = false;
            }
            else if (collisionTop)
            {
                finalVelocity.Y = 0;
            }
            else if (rigidBody.GameType == GameType.Platform)
            {
                if (wasGrounded)
                {
                    rigidBody.IsGrounded = false;
                }
            }

            if (collisionLeft || collisionRight)
            {
                finalVelocity.X = 0;
            }

            if (!(collisionBottom || collisionTop) || !((collisionLeft || collisionRight)))
            {
                float moveX = (collisionLeft || collisionRight) ? 0 : finalVelocity.X;
                float moveY = (collisionBottom || collisionTop) ? 0 : finalVelocity.Y;

                transform.Position += new Vector3(moveX, moveY, 0);
            }

            rigidBody.Velocity = new Vector3(finalVelocity.X, finalVelocity.Y, rigidBody.Velocity.Z);
        }

        public override void SharedEntityForEachUpdate(ref Yotsuba Entidad, GameTime time) { }
        public override void SharedEntityInitialize(ref Yotsuba Entidad) { }
        public override void Dispose() { }
    }
}
