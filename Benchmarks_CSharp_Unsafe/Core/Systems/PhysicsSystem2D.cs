using System.Runtime.CompilerServices;
using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    public unsafe struct PhysicsSystem2D
    {
        private nint _emPtr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InitializeSystem(EntityManager* entities) => _emPtr = (nint)entities;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateSystem(GameTime gameTime)
        {
            EntityManager* em = (EntityManager*)_emPtr;
            if (em == null) return;

            Yotsuba* entities = em->YotsubaEntities.Ptr;
            int count = em->YotsubaEntities.Count;
            TransformComponent* transforms = em->TransformComponents.Ptr;
            RigidBodyComponent2D* rigidbodies = em->Rigidbody2DComponents.Ptr;

            ApplyPlatformPhysics(entities, rigidbodies, count);
            MoveEntities(em, entities, transforms, rigidbodies, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ApplyPlatformPhysics(Yotsuba* entities, RigidBodyComponent2D* rigidbodies, int count)
        {
            for (int i = 0; i < count; i++)
            {
                ref Yotsuba entity = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
                if (!entity.HasComponent(YTBComponent.Rigibody)) continue;

                ref RigidBodyComponent2D rb = ref Unsafe.Add(ref Unsafe.AsRef<RigidBodyComponent2D>(rigidbodies), entity.Id);
                if (rb.GameType != GameType.Platform) continue;

                float newVelY = rb.Velocity.Y + rb.Gravity;
                if (rb.IsFastFalling && rb.Velocity.Y >= 0)
                    newVelY = rb.Velocity.Y + (rb.Gravity * rb.FastFallMultiplier);
                newVelY = Math.Min(newVelY, rb.MaxFallSpeed);
                rb.Velocity = new Vector3(rb.Velocity.X, newVelY, rb.Velocity.Z);
            }
        }

        private static void MoveEntities(EntityManager* em, Yotsuba* entities, TransformComponent* transforms, RigidBodyComponent2D* rigidbodies, int count)
        {
            for (int i = 0; i < count; i++)
            {
                ref Yotsuba entity = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
                if (!entity.HasComponent(YTBComponent.Transform) || !entity.HasComponent(YTBComponent.Rigibody)) continue;

                bool collisionBottom = false, collisionTop = false, collisionLeft = false, collisionRight = false;

                ref RigidBodyComponent2D rb = ref Unsafe.Add(ref Unsafe.AsRef<RigidBodyComponent2D>(rigidbodies), entity.Id);
                ref TransformComponent transform = ref Unsafe.Add(ref Unsafe.AsRef<TransformComponent>(transforms), entity.Id);
                bool wasGrounded = rb.IsGrounded;

                Unsafe.SkipInit(out Vector2 nextPos);
                nextPos.X = transform.Position.X + rb.Velocity.X;
                nextPos.Y = transform.Position.Y + rb.Velocity.Y;

                Unsafe.SkipInit(out Rectangle entityRect);
                entityRect.X = (int)(nextPos.X + rb.OffSetCollision.X);
                entityRect.Y = (int)(nextPos.Y + rb.OffSetCollision.Y);
                entityRect.Width = (int)(transform.Size.X * transform.Scale);
                entityRect.Height = (int)(transform.Size.Y * transform.Scale);

                bool sizeZero = transform.Size == Vector3.Zero;

                for (int j = 0; j < count; j++)
                {
                    ref Yotsuba other = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), j);
                    if (other.Id == entity.Id) continue;
                    if (!other.HasComponent(YTBComponent.Transform) || !other.HasComponent(YTBComponent.Rigibody)) continue;

                    ref RigidBodyComponent2D otherRb = ref Unsafe.Add(ref Unsafe.AsRef<RigidBodyComponent2D>(rigidbodies), other.Id);
                    ref TransformComponent otherT = ref Unsafe.Add(ref Unsafe.AsRef<TransformComponent>(transforms), other.Id);

                    if (other.HasComponent(YTBComponent.TileMap))
                        CheckTileMapCollision(em, ref rb, ref otherRb, ref transform, ref otherT, ref other, entityRect, sizeZero, ref collisionBottom, ref collisionTop, ref collisionLeft, ref collisionRight);
                    else
                        CheckEntityCollision(ref rb, ref otherRb, ref transform, ref otherT, entityRect, sizeZero, ref collisionBottom, ref collisionTop, ref collisionLeft, ref collisionRight);
                }

                ApplyMovement(ref rb, ref transform, collisionBottom, collisionTop, collisionLeft, collisionRight, wasGrounded);
            }
        }

        private static void CheckTileMapCollision(
            EntityManager* em,
            ref RigidBodyComponent2D rb, ref RigidBodyComponent2D otherRb,
            ref TransformComponent transform, ref TransformComponent otherT,
            ref Yotsuba tilemapEntity,
            Rectangle entityRect, bool sizeZero,
            ref bool cBottom, ref bool cTop, ref bool cLeft, ref bool cRight)
        {
            if (rb.Mass == MassLevel.NoCollision || otherRb.Mass == MassLevel.NoCollision) return;

            ref TileMapComponent2D tilemap = ref em->TileMapComponent2Ds[tilemapEntity.Id];
            float ofsX = otherT.Size.X * 0.5f * otherT.Scale;
            float ofsY = otherT.Size.Y * 0.5f * otherT.Scale;

            TileLayer* layers = tilemap.TileLayers.Ptr;
            int layerCount = tilemap.TileLayers.Count;

            for (int li = 0; li < layerCount; li++)
            {
                ref TileLayer layer = ref Unsafe.Add(ref Unsafe.AsRef<TileLayer>(layers), li);
                ReadOnlySpan<char> collision = "Collision";
                bool isCollisionLayer = layer.Name.Contains(collision, StringComparison.OrdinalIgnoreCase);

                int* data = layer.Data;
                for (int idx = 0; idx < layer.DataLength; idx++)
                {
                    int gid = Unsafe.Add(ref Unsafe.AsRef<int>(data), idx);
                    if (gid == 0) continue;

                    int tileX = idx % tilemap.Width;
                    int tileY = idx / tilemap.Width;
                    float worldX = otherT.Scale * (tileX * tilemap.TileWidth) + otherT.Position.X - ofsX;
                    float worldY = otherT.Scale * (tileY * tilemap.TileHeight) + otherT.Position.Y - ofsY;

                    Rectangle* nativeRects;
                    int nativeRectCount;
                    bool hasNative = tilemap.Collisions.TryGetValue(gid, out nativeRects, out nativeRectCount);

                    if (isCollisionLayer && !hasNative)
                    {
                        Unsafe.SkipInit(out Rectangle tileRect);
                        tileRect.X = (int)worldX; tileRect.Y = (int)worldY;
                        tileRect.Width = (int)(tilemap.TileWidth * otherT.Scale);
                        tileRect.Height = (int)(tilemap.TileHeight * otherT.Scale);

                        if (entityRect.Intersects(tileRect) && !sizeZero)
                            DetermineCollisionDirection(entityRect, tileRect, ref rb, ref cBottom, ref cTop, ref cLeft, ref cRight);
                    }

                    if (hasNative)
                    {
                        for (int ri = 0; ri < nativeRectCount; ri++)
                        {
                            ref Rectangle nr = ref Unsafe.Add(ref Unsafe.AsRef<Rectangle>(nativeRects), ri);
                            Unsafe.SkipInit(out Rectangle cr);
                            cr.X = (int)(worldX + nr.X * otherT.Scale);
                            cr.Y = (int)(worldY + nr.Y * otherT.Scale);
                            cr.Width = (int)(nr.Width * otherT.Scale);
                            cr.Height = (int)(nr.Height * otherT.Scale);

                            if (entityRect.Intersects(cr) && !sizeZero)
                                DetermineCollisionDirection(entityRect, cr, ref rb, ref cBottom, ref cTop, ref cLeft, ref cRight);
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckEntityCollision(
            ref RigidBodyComponent2D rb, ref RigidBodyComponent2D otherRb,
            ref TransformComponent transform, ref TransformComponent otherT,
            Rectangle entityRect, bool sizeZero,
            ref bool cBottom, ref bool cTop, ref bool cLeft, ref bool cRight)
        {
            if (rb.Mass == MassLevel.NoCollision || otherRb.Mass == MassLevel.NoCollision) return;
            if (otherT.Size == Vector3.Zero) return;

            Unsafe.SkipInit(out Rectangle otherRect);
            otherRect.X = (int)(otherT.Position.X + otherRb.OffSetCollision.X);
            otherRect.Y = (int)(otherT.Position.Y + otherRb.OffSetCollision.Y);
            otherRect.Width = (int)(otherT.Size.X * otherT.Scale);
            otherRect.Height = (int)(otherT.Size.Y * otherT.Scale);

            if (entityRect.Intersects(otherRect) && !sizeZero)
                DetermineCollisionDirection(entityRect, otherRect, ref rb, ref cBottom, ref cTop, ref cLeft, ref cRight);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DetermineCollisionDirection(
            Rectangle entityRect, Rectangle obstacleRect, ref RigidBodyComponent2D rb,
            ref bool cBottom, ref bool cTop, ref bool cLeft, ref bool cRight)
        {
            int overlapLeft = entityRect.Right - obstacleRect.Left;
            int overlapRight = obstacleRect.Right - entityRect.Left;
            int overlapTop = entityRect.Bottom - obstacleRect.Top;
            int overlapBottom = obstacleRect.Bottom - entityRect.Top;

            int minX = Math.Min(overlapLeft, overlapRight);
            int minY = Math.Min(overlapTop, overlapBottom);

            if (minY < minX)
            {
                if (overlapTop < overlapBottom && rb.Velocity.Y > 0) cBottom = true;
                else if (overlapBottom < overlapTop && rb.Velocity.Y < 0) cTop = true;
            }
            else
            {
                if (overlapLeft < overlapRight && rb.Velocity.X > 0) cRight = true;
                else if (overlapRight < overlapLeft && rb.Velocity.X < 0) cLeft = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ApplyMovement(
            ref RigidBodyComponent2D rb, ref TransformComponent transform,
            bool cBottom, bool cTop, bool cLeft, bool cRight, bool wasGrounded)
        {
            Unsafe.SkipInit(out Vector2 finalVel);
            finalVel.X = rb.Velocity.X;
            finalVel.Y = rb.Velocity.Y;

            if (cBottom) { finalVel.Y = 0; rb.IsGrounded = true; rb.IsJumping = false; rb.IsFastFalling = false; }
            else if (cTop) { finalVel.Y = 0; }
            else if (rb.GameType == GameType.Platform && wasGrounded) { rb.IsGrounded = false; }

            if (cLeft || cRight) finalVel.X = 0;

            if (!(cBottom || cTop) || !(cLeft || cRight))
            {
                float moveX = (cLeft || cRight) ? 0 : finalVel.X;
                float moveY = (cBottom || cTop) ? 0 : finalVel.Y;
                transform.Position = transform.Position + new Vector3(moveX, moveY, 0);
            }

            rb.Velocity = new Vector3(finalVel.X, finalVel.Y, rb.Velocity.Z);
        }

        public void SharedEntityForEachUpdate(ref Yotsuba e, GameTime t) { }
        public void SharedEntityInitialize(ref Yotsuba e) { }
        public void Dispose() { }
    }
}
