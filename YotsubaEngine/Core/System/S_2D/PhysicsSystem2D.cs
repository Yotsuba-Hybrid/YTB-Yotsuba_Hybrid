

using Microsoft.Xna.Framework;
using System;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.System.YotsubaEngineUI.UI;
using YotsubaEngine.Core.YTBMath;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events.YTBEvents;
using YotsubaEngine.Exceptions;
using YotsubaEngine.HighestPerformanceTypes;
using static YotsubaEngine.Core.Component.C_AGNOSTIC.RigidBody;

namespace YotsubaEngine.Core.System.S_2D
{
    /// <summary>
    /// System responsible for 2D physics updates and movement.
    /// Sistema que se encarga de toda la física del mundo (mover a las entidades)
    /// </summary>
    public class PhysicsSystem2D : ISystem
    {

        /// <summary>
        /// Event manager reference.
        /// Referencia al administrador de eventos.
        /// </summary>
        private EventManager EventManager { get; set; }

        /// <summary>
        /// Entity manager reference.
        /// Referencia al administrador de entidades.
        /// </summary>
        private EntityManager EntityManager { get; set; }

        /// <summary>
        /// Initializes the physics system.
        /// Implementacion de la interfaz ISystem
        /// </summary>
        /// <param name="entities">Entity manager. Administrador de entidades.</param>
        public void InitializeSystem(EntityManager @entities)
        {
#if YTB
			if (GameWontRun.GameWontRunByException) return;
#endif
			EventManager = EventManager.Instance;
            EntityManager = @entities;
            EngineUISystem.SendLog(typeof(PhysicsSystem2D).Name + " Se inicio correctamente");
        }

        /// <summary>
        /// Updates physics and moves entities.
        /// Implementacion de la interfaz ISystem
        /// </summary>
        /// <param name="gameTime">Game time. Tiempo de juego.</param>
        public void UpdateSystem(GameTime gameTime)
        {
#if YTB
			if (OperatingSystem.IsWindows())
				if (!RenderSystem2D.IsGameActive) return;

			if (GameWontRun.GameWontRunByException) return;

#endif

			YTB<Yotsuba> entities = EntityManager.YotsubaEntities;
            YTB<TransformComponent> transformComponents = EntityManager.TransformComponents;
            YTB<RigidBodyComponent2D> rigidbodyComponents = EntityManager.Rigidbody2DComponents;

            // First apply gravity and physics for Platform mode entities
            ApplyPlatformPhysics(entities, rigidbodyComponents, gameTime);

            // Then handle collisions and movement
            MoveEntities(entities, transformComponents, rigidbodyComponents, gameTime);
        }

        /// <summary>
        /// Applies gravity and platform physics to entities in Platform mode.
        /// Aplica gravedad y física de plataformas a entidades en modo Platform.
        /// </summary>
        private void ApplyPlatformPhysics(YTB<Yotsuba> entities, YTB<RigidBodyComponent2D> rigidbodyComponents, GameTime gameTime)
        {
            foreach (var entity in entities)
            {
                if (!entity.HasComponent(YTBComponent.Rigibody)) continue;

                ref RigidBodyComponent2D rigidBody = ref rigidbodyComponents[entity.Id];

                // Only apply gravity in Platform mode
                if (rigidBody.GameType != GameType.Platform) continue;

                // Apply gravity
                float newVelocityY = rigidBody.Velocity.Y + rigidBody.Gravity;

                // Apply fast fall multiplier if fast falling
                if (rigidBody.IsFastFalling && rigidBody.Velocity.Y >= 0)
                {
                    newVelocityY = rigidBody.Velocity.Y + (rigidBody.Gravity * rigidBody.FastFallMultiplier);
                }

                // Clamp to max fall speed
                newVelocityY = Math.Min(newVelocityY, rigidBody.MaxFallSpeed);

                rigidBody.Velocity = new Vector3(rigidBody.Velocity.X, newVelocityY, rigidBody.Velocity.Z);
            }
        }

        /// <summary>
        /// Moves entities while checking for collisions.
        /// Método que verifica cada entidad y si en el siguiente frame colisionara, de ser asi, no lo mueve de posición.
        /// </summary>
        /// <param name="entities">Entities collection. Colección de entidades.</param>
        /// <param name="transformComponents">Transform components. Componentes de transformación.</param>
        /// <param name="rigibodyComponents">Rigid body components. Componentes de cuerpo rígido.</param>
        /// <param name="gameTime">Game time. Tiempo de juego.</param>
        private void MoveEntities(YTB<Yotsuba> entities, YTB<TransformComponent> transformComponents, YTB<RigidBodyComponent2D> rigibodyComponents, GameTime gameTime)
        {
            foreach (var entity in entities)
            {
                if (!entity.HasComponent(YTBComponent.Transform) || !entity.HasComponent(YTBComponent.Rigibody)) continue;
                
                // Collision flags for each direction
                bool collisionBottom = false;
                bool collisionTop = false;
                bool collisionLeft = false;
                bool collisionRight = false;

                ref RigidBodyComponent2D rigidBody = ref rigibodyComponents[entity.Id];
                ref TransformComponent transform = ref transformComponents[entity.Id];

                bool wasGrounded = rigidBody.IsGrounded;

                // Calculate next position
                Vector2 nextPosition = new Vector2(transform.Position.X, transform.Position.Y) + YTBCartessian.Vector3ToVector2(rigidBody.Velocity);

                Rectangle entityRect = new Rectangle(
                    (int)(nextPosition.X + rigidBody.OffSetCollision.X),
                    (int)(nextPosition.Y + rigidBody.OffSetCollision.Y),
                    (int)(transform.Size.X * transform.Scale), 
                    (int)(transform.Size.Y * transform.Scale)
                );

                bool sizeZero = transform.Size == Vector3.Zero;

                // Check collisions with other entities
                foreach (var otherEntity in entities)
                {
                    if (otherEntity.Id == entity.Id) continue;
                    if (!otherEntity.HasComponent(YTBComponent.Transform) || !otherEntity.HasComponent(YTBComponent.Rigibody)) continue;

                    ref RigidBodyComponent2D otherRigidBody = ref rigibodyComponents[otherEntity.Id];
                    ref TransformComponent otherTransform = ref transformComponents[otherEntity.Id];

                    // Handle TileMap collisions
                    if (otherEntity.HasComponent(YTBComponent.TileMap))
                    {
                        CheckTileMapCollision(
                            entity, otherEntity, ref rigidBody, ref otherRigidBody, 
                            ref transform, ref otherTransform, entityRect, sizeZero, gameTime,
                            ref collisionBottom, ref collisionTop, ref collisionLeft, ref collisionRight);
                    }
                    else
                    {
                        // Regular entity collision
                        CheckEntityCollision(
                            entity, otherEntity, ref rigidBody, ref otherRigidBody,
                            ref transform, ref otherTransform, entityRect, sizeZero, gameTime,
                            ref collisionBottom, ref collisionTop, ref collisionLeft, ref collisionRight);
                    }
                }

                // Apply movement based on collision results
                ApplyMovement(entity.Id, ref rigidBody, ref transform, collisionBottom, collisionTop, collisionLeft, collisionRight, wasGrounded, gameTime);
            }
        }

        /// <summary>
        /// Checks collision with a TileMap entity.
        /// </summary>
        private void CheckTileMapCollision(
            Yotsuba entity, Yotsuba tilemapEntity,
            ref RigidBodyComponent2D rigidBody, ref RigidBodyComponent2D otherRigidBody,
            ref TransformComponent transform, ref TransformComponent otherTransform,
            Rectangle entityRect, bool sizeZero, GameTime gameTime,
            ref bool collisionBottom, ref bool collisionTop, ref bool collisionLeft, ref bool collisionRight)
        {
            if (rigidBody.Mass == MassLevel.NoCollision || otherRigidBody.Mass == MassLevel.NoCollision) return;

            ref TileMapComponent2D tilemap = ref EntityManager.TileMapComponent2Ds[tilemapEntity.Id];

            float originOffsetX = otherTransform.Size.X * 0.5f * otherTransform.Scale;
            float originOffsetY = otherTransform.Size.Y * 0.5f * otherTransform.Scale;

            foreach (var layer in tilemap.TileLayers)
            {
                if (!layer.Name.Contains("Collision", StringComparison.OrdinalIgnoreCase))
                    continue;

                for (int i = 0; i < layer.Data.Length; i++)
                {
                    int gid = layer.Data[i];
                    if (gid == 0) continue;

                    int tileX = i % tilemap.Width;
                    int tileY = i / tilemap.Width;

                    float worldX = otherTransform.Scale * (tileX * tilemap.TileWidth) + otherTransform.Position.X - originOffsetX;
                    float worldY = otherTransform.Scale * (tileY * tilemap.TileHeight) + otherTransform.Position.Y - originOffsetY;

                    Rectangle tileRect = new Rectangle(
                        (int)worldX,
                        (int)worldY,
                        (int)(tilemap.TileWidth * otherTransform.Scale),
                        (int)(tilemap.TileHeight * otherTransform.Scale)
                    );

                    if (entityRect.Intersects(tileRect) && !sizeZero)
                    {
                        // Determine collision direction
                        DetermineCollisionDirection(entityRect, tileRect, ref rigidBody,
                            ref collisionBottom, ref collisionTop, ref collisionLeft, ref collisionRight);

                        EventManager.Publish(new OnCollitionEvent()
                        {
                            EntityImpediment = tilemapEntity,
                            EntityTryMove = entity,
                            GameTime = gameTime
                        });

#if YTB
                        DebugOverlayUI.AddCollision(entity.Name, $"{tilemapEntity.Name} (Tile [{tileX},{tileY}])", gameTime);
#endif
                    }
                }
            }
        }

        /// <summary>
        /// Checks collision with another entity.
        /// </summary>
        private void CheckEntityCollision(
            Yotsuba entity, Yotsuba otherEntity,
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

                EventManager.Publish(new OnCollitionEvent()
                {
                    EntityImpediment = otherEntity,
                    EntityTryMove = entity,
                    GameTime = gameTime
                });

#if YTB
                DebugOverlayUI.AddCollision(entity.Name, otherEntity.Name, gameTime);
#endif
            }
        }

        /// <summary>
        /// Determines the collision direction based on rectangles and velocity.
        /// </summary>
        private void DetermineCollisionDirection(
            Rectangle entityRect, Rectangle obstacleRect,
            ref RigidBodyComponent2D rigidBody,
            ref bool collisionBottom, ref bool collisionTop, ref bool collisionLeft, ref bool collisionRight)
        {
            // Calculate overlap on each axis
            int overlapLeft = entityRect.Right - obstacleRect.Left;
            int overlapRight = obstacleRect.Right - entityRect.Left;
            int overlapTop = entityRect.Bottom - obstacleRect.Top;
            int overlapBottom = obstacleRect.Bottom - entityRect.Top;

            // Find minimum overlap to determine collision direction
            int minOverlapX = Math.Min(overlapLeft, overlapRight);
            int minOverlapY = Math.Min(overlapTop, overlapBottom);

            if (minOverlapY < minOverlapX)
            {
                // Vertical collision
                if (overlapTop < overlapBottom && rigidBody.Velocity.Y > 0)
                {
                    collisionBottom = true;
                }
                else if (overlapBottom < overlapTop && rigidBody.Velocity.Y < 0)
                {
                    collisionTop = true;
                }
            }
            else
            {
                // Horizontal collision
                if (overlapLeft < overlapRight && rigidBody.Velocity.X > 0)
                {
                    collisionRight = true;
                }
                else if (overlapRight < overlapLeft && rigidBody.Velocity.X < 0)
                {
                    collisionLeft = true;
                }
            }
        }

        /// <summary>
        /// Applies movement based on collision results and publishes grounded events.
        /// </summary>
        private void ApplyMovement(
            int entityId,
            ref RigidBodyComponent2D rigidBody, 
            ref TransformComponent transform,
            bool collisionBottom, bool collisionTop, bool collisionLeft, bool collisionRight,
            bool wasGrounded, GameTime gameTime)
        {
            Vector2 finalVelocity = YTBCartessian.Vector3ToVector2(rigidBody.Velocity);

            // Handle vertical collisions
            if (collisionBottom)
            {
                finalVelocity.Y = 0;
                rigidBody.IsGrounded = true;
                rigidBody.IsJumping = false;
                rigidBody.IsFastFalling = false;

                // Publish grounded event if just landed
                if (!wasGrounded)
                {
                    EventManager.Publish(new OnEntityGroundedEvent
                    {
                        EntityId = entityId,
                        GameTime = gameTime
                    });
                }
            }
            else if (collisionTop)
            {
                finalVelocity.Y = 0;
            }
            else if (rigidBody.GameType == GameType.Platform)
            {
                // No vertical collision - entity is airborne
                if (wasGrounded)
                {
                    rigidBody.IsGrounded = false;
                    EventManager.Publish(new OnEntityAirborneEvent
                    {
                        EntityId = entityId,
                        GameTime = gameTime
                    });
                }
            }

            // Handle horizontal collisions
            if (collisionLeft || collisionRight)
            {
                finalVelocity.X = 0;
            }

            // Apply final velocity to position
            if (!(collisionBottom || collisionTop) || !((collisionLeft || collisionRight)))
            {
                // Apply movement - only on axes that didn't collide
                float moveX = (collisionLeft || collisionRight) ? 0 : finalVelocity.X;
                float moveY = (collisionBottom || collisionTop) ? 0 : finalVelocity.Y;
                
                transform.Position += new Vector3(moveX, moveY, 0);
            }

            rigidBody.Velocity = new Vector3(finalVelocity.X, finalVelocity.Y, rigidBody.Velocity.Z);
        }

        /// <summary>
        /// Shared entity update hook (unused in this system).
        /// Hook de actualización compartida (no usado en este sistema).
        /// </summary>
        /// <param name="Entidad">Entity instance. Instancia de entidad.</param>
        /// <param name="time">Game time. Tiempo de juego.</param>
        public void SharedEntityForEachUpdate(Yotsuba Entidad, GameTime time)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Shared entity initialization hook (unused in this system).
        /// Hook de inicialización compartida (no usado en este sistema).
        /// </summary>
        /// <param name="Entidad">Entity instance. Instancia de entidad.</param>
        public void SharedEntityInitialize(Yotsuba Entidad)
        {
            //throw new NotImplementedException();
        }
    }
}
