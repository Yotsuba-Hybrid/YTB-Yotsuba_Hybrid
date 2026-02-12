using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events.YTBEvents;
using YotsubaEngine.Exceptions;
using YotsubaEngine.HighestPerformanceTypes;
using static YotsubaEngine.Exceptions.GameWontRun;

namespace YotsubaEngine.YTB_Toolkit
{
	/// <summary>
	/// Provee controles direccionales automáticos (WASD, flechas, gamepad) y está integrado con PhysicsSystem2D para modos Platform y TopDown.
	/// <para>Provides automatic directional controls (WASD, arrow keys, gamepad) and is fully integrated with PhysicsSystem2D for Platform and TopDown modes.</para>
	/// </summary>
	public class WASDControl
	{
		/// <summary>
		/// Instancia única del WASD Control.
		/// <para>Singleton instance of WASDControl.</para>
		/// </summary>
		public static WASDControl Instance { get; set; }

		private readonly EntityManager _entityManager;
		private readonly MovementHandler _movementHandler;

		/// <summary>
		/// Event manager reference.
		/// Referencia al EventManager.
		/// </summary>
		private readonly EventManager _eventManager = EventManager.Instance;

		/// <summary>
		/// Collection of entities using this toolkit.
		/// Colección de entidades que usan este toolkit.
		/// </summary>
		private readonly YTB<Yotsuba> _entities = new();

		/// <summary>
		/// Inicializa una nueva instancia de WASDControl.
		/// <para>Initializes a new WASDControl instance.</para>
		/// </summary>
		/// <param name="entityManager">Administrador de entidades. <para>Entity manager.</para></param>
		public WASDControl(EntityManager entityManager)
		{
			_entityManager = entityManager;
			_movementHandler = new MovementHandler(_entityManager, _entities);
		}

		/// <summary>
		/// Añade una entidad a este toolkit de movimiento.
		/// <para>Adds an entity to this movement toolkit.</para>
		/// </summary>
		/// <param name="entity">Entidad a añadir. <para>Entity to add.</para></param>
		/// <exception cref="GameWontRun">Si la entidad no tiene InputComponent o ya existe. <para>If the entity lacks InputComponent or already exists.</para></exception>
		public void AddEntity(Yotsuba entity)
		{
			if (_entities.Contains(entity))
			{
				throw new GameWontRun(
					$"Entity {entity.Name} is already registered in WASDControl. Check your script.",
					YTBErrors.EntityCannotAddToWasdYTB_toolkit);
			}

			if (!entity.HasComponent(YTBComponent.Input))
			{
				throw new GameWontRun(
					$"Entity {entity.Name} requires InputComponent for WASDControl.",
					YTBErrors.EntityCannotAddToWasdYTB_toolkit);
			}

			if (!entity.HasComponent(YTBComponent.Transform))
			{
				throw new GameWontRun(
					$"Entity {entity.Name} requires TransformComponent for WASDControl.",
					YTBErrors.EntityCannotAddToWasdYTB_toolkit);
			}

			_entities.Add(entity);
		}

		/// <summary>
		/// Remueve una entidad de este toolkit.
		/// <para>Removes an entity from this toolkit.</para>
		/// </summary>
		/// <param name="entity">Entidad a remover. <para>Entity to remove.</para></param>
		/// <exception cref="GameWontRun">Si la entidad no existe. <para>If the entity doesn't exist.</para></exception>
		public void RemoveEntity(Yotsuba entity)
		{
			if (!_entities.Contains(entity))
			{
				throw new GameWontRun(
					$"Entity {entity.Name} is not registered in WASDControl.",
					YTBErrors.EntityCannotAddToWasdYTB_toolkit);
			}
			_entities.Remove(entity);
		}

		/// <summary>
		/// Inicializa el sistema de control WASD y se suscribe a eventos.
		/// <para>Initializes the WASD control system and subscribes to events.</para>
		/// </summary>
		public void Initialize()
		{
			Instance = this;

			// Subscribe to animation end events
			_eventManager.Subscribe<OnAnimationDontLoopReleaseEvent>(_movementHandler.HandleAnimationEnd);

			// Subscribe to input events
			_eventManager.Subscribe<OnKeyBoardEvent>(_movementHandler.HandleKeyboardInput);
			_eventManager.Subscribe<OnGamePadEvent>(_movementHandler.HandleGamePadInput);
			_eventManager.Subscribe<OnThumbstickEvent>(_movementHandler.HandleThumbstickInput);

			// Subscribe to physics events
			_eventManager.Subscribe<OnEntityGroundedEvent>(_movementHandler.HandleEntityGrounded);
			_eventManager.Subscribe<OnEntityAirborneEvent>(_movementHandler.HandleEntityAirborne);
		}
	}

	/// <summary>
	/// Handles all movement logic for entities.
	/// Maneja toda la lógica de movimiento para entidades.
	/// </summary>
	internal class MovementHandler
	{
		private readonly EntityManager _entityManager;
		private readonly YTB<Yotsuba> _entities;
		private readonly EventManager _eventManager = EventManager.Instance;

		public MovementHandler(EntityManager entityManager, YTB<Yotsuba> entities)
		{
			_entityManager = entityManager;
			_entities = entities;
		}

		/// <summary>
		/// Checks if an entity has 2.5D mode enabled in its sprite component.
		/// Verifica si una entidad tiene el modo 2.5D habilitado en su componente de sprite.
		/// </summary>
		private bool IsEntity2_5D(int entityId)
		{
			var entity = _entities.FirstOrDefault(e => e.Id == entityId);
			if (entity == null || !entity.HasComponent(YTBComponent.Sprite)) return false;
			return _entityManager.Sprite2DComponents[entityId].Is2_5D;
		}

		#region Animation Helpers

		/// <summary>
		/// Safely sets an animation if it exists, avoiding KeyNotFoundException.
		/// Establece una animación de forma segura si existe, evitando KeyNotFoundException.
		/// </summary>
		private bool TrySetAnimation(ref AnimationComponent2D animation, AnimationType type, bool isLooping = true)
		{
			if (!animation.ContainsAnimation(type))
				return false;

			var anim = animation.GetAnimation(type);
			anim.IsLooping = isLooping;
			animation.CurrentAnimationType = (type, anim);
			return true;
		}

		/// <summary>
		/// Gets the appropriate idle animation based on game type.
		/// Obtiene la animación idle apropiada según el tipo de juego.
		/// </summary>
		private void SetIdleAnimation(ref AnimationComponent2D animation, ref RigidBodyComponent2D rigidBody)
		{
			TrySetAnimation(ref animation, AnimationType.idle, true);
		}

		/// <summary>
		/// Gets the appropriate movement animation based on game type and state.
		/// Obtiene la animación de movimiento apropiada según el tipo de juego y estado.
		/// </summary>
		private void SetMovementAnimation(ref AnimationComponent2D animation, ref RigidBodyComponent2D rigidBody)
		{
			if (rigidBody.GameType == GameType.Platform && !rigidBody.IsGrounded)
			{
				// In air - show jump/fall animation
				if (!TrySetAnimation(ref animation, AnimationType.jump, false))
				{
					TrySetAnimation(ref animation, AnimationType.idle, true);
				}
			}
			else
			{
				// On ground or TopDown - show walk animation
				if (!TrySetAnimation(ref animation, AnimationType.walk, true))
				{
					TrySetAnimation(ref animation, AnimationType.idle, true);
				}
			}
		}

		#endregion

		#region Movement Logic

		/// <summary>
		/// Processes horizontal movement (left/right).
		/// Procesa movimiento horizontal (izquierda/derecha).
		/// </summary>
		private void ProcessHorizontalMovement(
			int entityId,
			int direction, // -1 = left, 1 = right
			InputEventType eventType,
			ref RigidBodyComponent2D rigidBody,
			ref TransformComponent transform,
			ref AnimationComponent2D animation)
		{
			switch (eventType)
			{
				case InputEventType.JustPressed:
				case InputEventType.HoldDown:
					// Apply horizontal velocity
					float targetVelocityX = direction * rigidBody.TOP_SPEED;
					rigidBody.Velocity = new Vector3(targetVelocityX, rigidBody.Velocity.Y, rigidBody.Velocity.Z);
					rigidBody.FacingDirection = direction;

					// Set sprite facing direction
					transform.SpriteEffects = direction > 0 
						? SpriteEffects.None 
						: SpriteEffects.FlipHorizontally;

					// Set movement animation
					SetMovementAnimation(ref animation, ref rigidBody);
					break;
			}
		}

		/// <summary>
		/// Processes vertical movement based on game type.
		/// Procesa movimiento vertical según el tipo de juego.
		/// </summary>
		private void ProcessVerticalMovement(
			int entityId,
			int direction, // -1 = up, 1 = down
			InputEventType eventType,
			ref RigidBodyComponent2D rigidBody,
			ref TransformComponent transform,
			ref AnimationComponent2D animation,
			GameTime gameTime)
		{
			if (rigidBody.GameType == GameType.TopDown)
			{
				// TopDown: Up/Down moves the entity
				ProcessTopDownVertical(entityId, direction, eventType, ref rigidBody, ref animation);
			}
			else // Platform
			{
				ProcessPlatformVertical(entityId, direction, eventType, ref rigidBody, ref animation, gameTime);
			}
		}

		/// <summary>
		/// Handles vertical movement in TopDown mode.
		/// Maneja movimiento vertical en modo TopDown.
		/// </summary>
		private void ProcessTopDownVertical(
			int entityId,
			int direction,
			InputEventType eventType,
			ref RigidBodyComponent2D rigidBody,
			ref AnimationComponent2D animation)
		{
			bool is2_5D = IsEntity2_5D(entityId);

			switch (eventType)
			{
				case InputEventType.JustPressed:
				case InputEventType.HoldDown:
					if (is2_5D)
					{
						// 2.5D: Mover en el eje Z en lugar del eje Y
						rigidBody.Velocity = new Vector3(
							rigidBody.Velocity.X,
							rigidBody.Velocity.Y,
							direction * rigidBody.TOP_SPEED);
					}
					else
					{
						float targetVelocityY = direction * rigidBody.TOP_SPEED;
						rigidBody.Velocity = new Vector3(rigidBody.Velocity.X, targetVelocityY, rigidBody.Velocity.Z);
					}
					SetMovementAnimation(ref animation, ref rigidBody);
					break;
			}
		}

		/// <summary>
		/// Handles vertical input in Platform mode (jump/fast fall).
		/// Maneja input vertical en modo Platform (salto/caída rápida).
		/// </summary>
		private void ProcessPlatformVertical(
			int entityId,
			int direction,
			InputEventType eventType,
			ref RigidBodyComponent2D rigidBody,
			ref AnimationComponent2D animation,
			GameTime gameTime)
		{
			if (direction == -1) // Up - Jump
			{
				if (eventType == InputEventType.JustPressed && rigidBody.IsGrounded)
				{
					// Apply jump force
					rigidBody.Velocity = new Vector3(rigidBody.Velocity.X, rigidBody.JumpForce, rigidBody.Velocity.Z);
					rigidBody.IsGrounded = false;
					rigidBody.IsJumping = true;

					// Publish jump event
					_eventManager.Publish(new OnEntityJumpEvent
					{
						EntityId = entityId,
						GameTime = gameTime
					});

					// Set jump animation
					if (!TrySetAnimation(ref animation, AnimationType.jump, false))
					{
						TrySetAnimation(ref animation, AnimationType.idle, true);
					}
				}
			}
			else if (direction == 1) // Down - Fast fall / Crouch
			{
				if (rigidBody.IsGrounded)
				{
					// On ground - crouch
					if (eventType == InputEventType.JustPressed)
					{
						if (!TrySetAnimation(ref animation, AnimationType.crouch, false))
						{
							TrySetAnimation(ref animation, AnimationType.idle, true);
						}
					}
				}
				else
				{
					// In air - fast fall
					if (eventType == InputEventType.JustPressed || eventType == InputEventType.HoldDown)
					{
						rigidBody.IsFastFalling = true;
					}
				}
			}
		}

		/// <summary>
		/// Handles input release to stop movement.
		/// Maneja la liberación de input para detener el movimiento.
		/// </summary>
		private void ProcessInputRelease(
			int entityId,
			ActionEntityInput action,
			ref RigidBodyComponent2D rigidBody,
			ref AnimationComponent2D animation)
		{
			var inputManager = InputManager.Instance;
			bool anyHorizontalHeld = false;
			bool anyVerticalHeld = false;

			// Check if any movement keys are still held (keyboard)
			if (inputManager?.Keyboard != null)
			{
				anyHorizontalHeld = inputManager.Keyboard.IsKeyDown(Keys.A) ||
				                     inputManager.Keyboard.IsKeyDown(Keys.D) ||
				                     inputManager.Keyboard.IsKeyDown(Keys.Left) ||
				                     inputManager.Keyboard.IsKeyDown(Keys.Right);

				anyVerticalHeld = inputManager.Keyboard.IsKeyDown(Keys.W) ||
				                  inputManager.Keyboard.IsKeyDown(Keys.S) ||
				                  inputManager.Keyboard.IsKeyDown(Keys.Up) ||
				                  inputManager.Keyboard.IsKeyDown(Keys.Down);
			}

			// Check gamepad input as well
			if (inputManager?.GamePads != null)
			{
				foreach (var gamePad in inputManager.GamePads)
				{
					if (!gamePad.IsConnected) continue;

					// Check D-pad and thumbstick for horizontal movement
					anyHorizontalHeld = anyHorizontalHeld ||
						gamePad.IsButtonDown(Buttons.DPadLeft) ||
						gamePad.IsButtonDown(Buttons.DPadRight) ||
						gamePad.IsButtonDown(Buttons.LeftThumbstickLeft) ||
						gamePad.IsButtonDown(Buttons.LeftThumbstickRight) ||
						Math.Abs(gamePad.LeftThumbStick.X) > 0.1f;

					// Check D-pad and thumbstick for vertical movement
					anyVerticalHeld = anyVerticalHeld ||
						gamePad.IsButtonDown(Buttons.DPadUp) ||
						gamePad.IsButtonDown(Buttons.DPadDown) ||
						gamePad.IsButtonDown(Buttons.LeftThumbstickUp) ||
						gamePad.IsButtonDown(Buttons.LeftThumbstickDown) ||
						Math.Abs(gamePad.LeftThumbStick.Y) > 0.1f;
				}
			}

			// Stop horizontal movement if no horizontal keys held
			if (!anyHorizontalHeld && (action == ActionEntityInput.MoveLeft || action == ActionEntityInput.MoveRight))
			{
				rigidBody.Velocity = new Vector3(0, rigidBody.Velocity.Y, rigidBody.Velocity.Z);
			}

			// Stop vertical movement in TopDown mode if no vertical keys held
			if (rigidBody.GameType == GameType.TopDown)
			{
				if (!anyVerticalHeld && (action == ActionEntityInput.MoveUp || action == ActionEntityInput.MoveDown))
				{
					if (IsEntity2_5D(entityId))
					{
						// 2.5D: Detener movimiento en Z
						rigidBody.Velocity = new Vector3(rigidBody.Velocity.X, rigidBody.Velocity.Y, 0);
					}
					else
					{
						rigidBody.Velocity = new Vector3(rigidBody.Velocity.X, 0, rigidBody.Velocity.Z);
					}
				}
			}
			else // Platform
			{
				// Stop fast falling when down is released
				if (action == ActionEntityInput.MoveDown)
				{
					rigidBody.IsFastFalling = false;
				}
			}

			// Set idle animation if no movement
			if (!anyHorizontalHeld && !anyVerticalHeld && rigidBody.IsGrounded)
			{
				SetIdleAnimation(ref animation, ref rigidBody);
			}
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Maneja eventos de entrada de teclado.
		/// <para>Handles keyboard input events.</para>
		/// </summary>
		/// <param name="evt">Evento de teclado a procesar. <para>Keyboard event to process.</para></param>
		public void HandleKeyboardInput(OnKeyBoardEvent evt)
		{
			var entity = _entities.FirstOrDefault(e => e.Id == evt.EntityId);
			if (entity == null) return;

			if (!entity.HasComponent(YTBComponent.Transform)) return;

			ref var transform = ref _entityManager.TransformComponents[entity.Id];
			ref var rigidBody = ref _entityManager.Rigidbody2DComponents[entity.Id];
			
			// Animation component is optional
			bool hasAnimation = entity.HasComponent(YTBComponent.Animation);
			ref var animation = ref _entityManager.Animation2DComponents[entity.Id];

			// Map key to action
			ActionEntityInput? action = evt.Key switch
			{
				Keys.W or Keys.Up => ActionEntityInput.MoveUp,
				Keys.S or Keys.Down => ActionEntityInput.MoveDown,
				Keys.A or Keys.Left => ActionEntityInput.MoveLeft,
				Keys.D or Keys.Right => ActionEntityInput.MoveRight,
				Keys.Space => ActionEntityInput.Jump,
				_ => null
			};

			if (action == null) return;

			ProcessInput(entity.Id, action.Value, evt.Type, ref rigidBody, ref transform, ref animation, hasAnimation, evt.GameTime);
		}

		/// <summary>
		/// Maneja eventos de entrada de gamepad.
		/// <para>Handles gamepad input events.</para>
		/// </summary>
		/// <param name="evt">Evento de gamepad a procesar. <para>Gamepad event to process.</para></param>
		public void HandleGamePadInput(OnGamePadEvent evt)
		{
			var entity = _entities.FirstOrDefault(e => e.Id == evt.EntityId);
			if (entity == null) return;

			if (!entity.HasComponent(YTBComponent.Transform)) return;

			ref var transform = ref _entityManager.TransformComponents[entity.Id];
			ref var rigidBody = ref _entityManager.Rigidbody2DComponents[entity.Id];
			
			bool hasAnimation = entity.HasComponent(YTBComponent.Animation);
			ref var animation = ref _entityManager.Animation2DComponents[entity.Id];

			// Map gamepad button to action
			ActionEntityInput? action = evt.Button switch
			{
				Buttons.DPadUp or Buttons.LeftThumbstickUp => ActionEntityInput.MoveUp,
				Buttons.DPadDown or Buttons.LeftThumbstickDown => ActionEntityInput.MoveDown,
				Buttons.DPadLeft or Buttons.LeftThumbstickLeft => ActionEntityInput.MoveLeft,
				Buttons.DPadRight or Buttons.LeftThumbstickRight => ActionEntityInput.MoveRight,
				Buttons.A => ActionEntityInput.Jump,
				_ => null
			};

			if (action == null) return;

			ProcessInput(entity.Id, action.Value, evt.Type, ref rigidBody, ref transform, ref animation, hasAnimation, evt.GameTime);
		}

		/// <summary>
		/// Maneja eventos de thumbstick para movimiento analógico.
		/// <para>Handles thumbstick input events for analog movement.</para>
		/// </summary>
		/// <param name="evt">Evento de thumbstick a procesar. <para>Thumbstick event to process.</para></param>
		public void HandleThumbstickInput(OnThumbstickEvent evt)
		{
			var entity = _entities.FirstOrDefault(e => e.Id == evt.EntityId);
			if (entity == null) return;

			if (!entity.HasComponent(YTBComponent.Transform)) return;

			ref var transform = ref _entityManager.TransformComponents[entity.Id];
			ref var rigidBody = ref _entityManager.Rigidbody2DComponents[entity.Id];

			bool hasAnimation = entity.HasComponent(YTBComponent.Animation);
			ref var animation = ref _entityManager.Animation2DComponents[entity.Id];

			// Create a dummy animation component if entity doesn't have one
			if (!hasAnimation)
			{
				animation = default;
			}

			Vector2 leftStick = evt.LeftThumbstick;
			const float deadzone = 0.1f;

			// Check keyboard state to avoid overriding keyboard input
			var inputManager = InputManager.Instance;
			bool keyboardHorizontalHeld = false;
			bool keyboardVerticalHeld = false;

			if (inputManager?.Keyboard != null)
			{
				keyboardHorizontalHeld = inputManager.Keyboard.IsKeyDown(Keys.A) ||
				                          inputManager.Keyboard.IsKeyDown(Keys.D) ||
				                          inputManager.Keyboard.IsKeyDown(Keys.Left) ||
				                          inputManager.Keyboard.IsKeyDown(Keys.Right);

				keyboardVerticalHeld = inputManager.Keyboard.IsKeyDown(Keys.W) ||
				                        inputManager.Keyboard.IsKeyDown(Keys.S) ||
				                        inputManager.Keyboard.IsKeyDown(Keys.Up) ||
				                        inputManager.Keyboard.IsKeyDown(Keys.Down);
			}

			// Process horizontal movement from left thumbstick
			if (Math.Abs(leftStick.X) > deadzone)
			{
				float targetVelocityX = leftStick.X * rigidBody.TOP_SPEED;
				rigidBody.Velocity = new Vector3(targetVelocityX, rigidBody.Velocity.Y, rigidBody.Velocity.Z);
				rigidBody.FacingDirection = leftStick.X > 0 ? 1 : -1;

				// Set sprite facing direction
				transform.SpriteEffects = leftStick.X > 0 
					? SpriteEffects.None 
					: SpriteEffects.FlipHorizontally;

				// Set movement animation
				SetMovementAnimation(ref animation, ref rigidBody);
			}
			else if (!keyboardHorizontalHeld)
			{
				// No thumbstick or keyboard horizontal input - stop horizontal movement
				rigidBody.Velocity = new Vector3(0, rigidBody.Velocity.Y, rigidBody.Velocity.Z);
			}

			// Process vertical movement from left thumbstick based on game type
			if (rigidBody.GameType == GameType.TopDown)
			{
				bool is2_5D = IsEntity2_5D(entity.Id);

				// TopDown: Up/Down moves the entity
				if (Math.Abs(leftStick.Y) > deadzone)
				{
					if (is2_5D)
					{
						// 2.5D: Mover en el eje Z en lugar del eje Y
						float moveZ = -leftStick.Y * rigidBody.TOP_SPEED;
						rigidBody.Velocity = new Vector3(rigidBody.Velocity.X, rigidBody.Velocity.Y, moveZ);
					}
					else
					{
						// Note: Y is inverted in thumbstick (up is positive)
						float targetVelocityY = -leftStick.Y * rigidBody.TOP_SPEED;
						rigidBody.Velocity = new Vector3(rigidBody.Velocity.X, targetVelocityY, rigidBody.Velocity.Z);
					}
					SetMovementAnimation(ref animation, ref rigidBody);
				}
				else if (!keyboardVerticalHeld)
				{
					if (is2_5D)
					{
						// 2.5D: Detener movimiento en Z
						rigidBody.Velocity = new Vector3(rigidBody.Velocity.X, rigidBody.Velocity.Y, 0);
					}
					else
					{
						// No thumbstick or keyboard vertical input - stop vertical movement
						rigidBody.Velocity = new Vector3(rigidBody.Velocity.X, 0, rigidBody.Velocity.Z);
					}
				}

				// Set idle if no movement from any source
				bool noHorizontalInput = Math.Abs(leftStick.X) <= deadzone && !keyboardHorizontalHeld;
				bool noVerticalInput = Math.Abs(leftStick.Y) <= deadzone && !keyboardVerticalHeld;
				if (noHorizontalInput && noVerticalInput)
				{
					SetIdleAnimation(ref animation, ref rigidBody);
				}
			}
			else // Platform mode
			{
				// Check for down input (fast fall)
				if (leftStick.Y < -deadzone && !rigidBody.IsGrounded)
				{
					rigidBody.IsFastFalling = true;
				}
				else if (!keyboardVerticalHeld || !(inputManager?.Keyboard?.IsKeyDown(Keys.S) == true || inputManager?.Keyboard?.IsKeyDown(Keys.Down) == true))
				{
					// Only stop fast falling if not holding down on keyboard either
					rigidBody.IsFastFalling = false;
				}

				// Set idle if no horizontal movement and grounded
				bool noHorizontalInput = Math.Abs(leftStick.X) <= deadzone && !keyboardHorizontalHeld;
				if (noHorizontalInput && rigidBody.IsGrounded)
				{
					SetIdleAnimation(ref animation, ref rigidBody);
				}
			}
		}

		/// <summary>
		/// Central input processing logic.
		/// Lógica central de procesamiento de input.
		/// </summary>
		private void ProcessInput(
			int entityId,
			ActionEntityInput action,
			InputEventType eventType,
			ref RigidBodyComponent2D rigidBody,
			ref TransformComponent transform,
			ref AnimationComponent2D animation,
			bool hasAnimation,
			GameTime gameTime)
		{
			// Create a dummy animation component if entity doesn't have one
			if (!hasAnimation)
			{
				animation = default;
			}

			switch (action)
			{
				case ActionEntityInput.MoveLeft:
					if (eventType == InputEventType.JustReleased)
						ProcessInputRelease(entityId, action, ref rigidBody, ref animation);
					else
						ProcessHorizontalMovement(entityId, -1, eventType, ref rigidBody, ref transform, ref animation);
					break;

				case ActionEntityInput.MoveRight:
					if (eventType == InputEventType.JustReleased)
						ProcessInputRelease(entityId, action, ref rigidBody, ref animation);
					else
						ProcessHorizontalMovement(entityId, 1, eventType, ref rigidBody, ref transform, ref animation);
					break;

				case ActionEntityInput.MoveUp:
					if (eventType == InputEventType.JustReleased)
						ProcessInputRelease(entityId, action, ref rigidBody, ref animation);
					else
						ProcessVerticalMovement(entityId, -1, eventType, ref rigidBody, ref transform, ref animation, gameTime);
					break;

				case ActionEntityInput.MoveDown:
					if (eventType == InputEventType.JustReleased)
						ProcessInputRelease(entityId, action, ref rigidBody, ref animation);
					else
						ProcessVerticalMovement(entityId, 1, eventType, ref rigidBody, ref transform, ref animation, gameTime);
					break;

				case ActionEntityInput.Jump:
					// Jump is handled as MoveUp in Platform mode
					if (rigidBody.GameType == GameType.Platform && eventType == InputEventType.JustPressed)
					{
						ProcessVerticalMovement(entityId, -1, eventType, ref rigidBody, ref transform, ref animation, gameTime);
					}
					break;
			}
		}

		/// <summary>
		/// Maneja eventos de fin de animación para volver a idle.
		/// <para>Handles animation end events to return to idle.</para>
		/// </summary>
		/// <param name="evt">Evento de fin de animación a procesar. <para>Animation end event to process.</para></param>
		public void HandleAnimationEnd(OnAnimationDontLoopReleaseEvent evt)
		{
			var entity = _entities.FirstOrDefault(e => e.Id == evt.EntityId);
			if (entity == null) return;

			if (!entity.HasComponent(YTBComponent.Animation)) return;

			ref var animation = ref _entityManager.Animation2DComponents[entity.Id];
			ref var rigidBody = ref _entityManager.Rigidbody2DComponents[entity.Id];

			// Return to idle after non-looping animations complete
			if (evt.AnimationName is AnimationType.jump or AnimationType.crouch or AnimationType.attack)
			{
				// Only go to idle if grounded (for platform games)
				if (rigidBody.GameType == GameType.TopDown || rigidBody.IsGrounded)
				{
					SetIdleAnimation(ref animation, ref rigidBody);
				}
			}
		}

		/// <summary>
		/// Maneja eventos de entidad en el suelo.
		/// <para>Handles entity grounded events.</para>
		/// </summary>
		/// <param name="evt">Evento de entidad en el suelo a procesar. <para>Grounded entity event to process.</para></param>
		public void HandleEntityGrounded(OnEntityGroundedEvent evt)
		{
			var entity = _entities.FirstOrDefault(e => e.Id == evt.EntityId);
			if (entity == null) return;

			if (!entity.HasComponent(YTBComponent.Animation)) return;

			ref var animation = ref _entityManager.Animation2DComponents[entity.Id];
			ref var rigidBody = ref _entityManager.Rigidbody2DComponents[entity.Id];

			// Reset jumping state
			rigidBody.IsJumping = false;
			rigidBody.IsFastFalling = false;

			// Check if moving horizontally
			if (Math.Abs(rigidBody.Velocity.X) > 0.1f)
			{
				SetMovementAnimation(ref animation, ref rigidBody);
			}
			else
			{
				SetIdleAnimation(ref animation, ref rigidBody);
			}
		}

		/// <summary>
		/// Maneja eventos de entidad en el aire.
		/// <para>Handles entity airborne events.</para>
		/// </summary>
		/// <param name="evt">Evento de entidad en el aire a procesar. <para>Airborne entity event to process.</para></param>
		public void HandleEntityAirborne(OnEntityAirborneEvent evt)
		{
			var entity = _entities.FirstOrDefault(e => e.Id == evt.EntityId);
			if (entity == null) return;

			if (!entity.HasComponent(YTBComponent.Animation)) return;

			ref var animation = ref _entityManager.Animation2DComponents[entity.Id];
			ref var rigidBody = ref _entityManager.Rigidbody2DComponents[entity.Id];

			// Show jump/fall animation when airborne
			if (!TrySetAnimation(ref animation, AnimationType.jump, false))
			{
				TrySetAnimation(ref animation, AnimationType.idle, true);
			}
		}

		#endregion
	}
}
