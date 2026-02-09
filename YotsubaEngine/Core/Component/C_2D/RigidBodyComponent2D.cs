
using Microsoft.Xna.Framework;
using static YotsubaEngine.Core.Component.C_AGNOSTIC.RigidBody;

namespace YotsubaEngine.Core.Component.C_2D
{
    /// <summary>
    /// Component that adds basic 2D physics behavior.
    /// Componente que añade la funcionalidad de fisica basica a un objeto.
    /// </summary>
    /// <param name="gameType">Game type used for physics behavior. Tipo de juego usado para la física.</param>
    /// <param name="mass">Mass level for the body. Nivel de masa del cuerpo.</param>
    public struct RigidBodyComponent2D(GameType gameType, MassLevel mass)
    {
        /// <summary>
        /// Base movement speed.
        /// Velocidad base del objeto.
        /// </summary>
        public float SPEED = 1.0f;

        /// <summary>
        /// Maximum movement speed.
        /// Velocidad maxima del objeto.
        /// </summary>
        public float TOP_SPEED = 3.0f;

        /// <summary>
        /// Collision offset.
        /// Desfase respecto a la colision.
        /// </summary>
        public Vector2 OffSetCollision { get; set; } = Vector2.Zero;

        /// <summary>
        /// Determines physics behavior based on game type.
        /// Determina el comportamiento de la fisica segun el tipo de juego.
        /// </summary>
        public GameType GameType { get; set; } = gameType;

        /// <summary>
        /// Current velocity in 3D space.
        /// Velocidad del objeto en el espacio 3D.
        /// </summary>
        public Vector3 Velocity { get; set; } = Vector3.Zero;

        /// <summary>
        /// Mass used to determine inertia and resistance.
        /// Masa del objeto para determinar su inercia y resistencia a fuerzas.
        /// </summary>
        public MassLevel Mass { get; set; } = mass;

        // ========== PLATFORM PHYSICS PROPERTIES ==========

        /// <summary>
        /// Gravity strength applied in Platform mode.
        /// Fuerza de gravedad aplicada en modo Platform.
        /// </summary>
        public float Gravity { get; set; } = 0.5f;

        /// <summary>
        /// Jump force applied when jumping in Platform mode.
        /// Fuerza de salto aplicada al saltar en modo Platform.
        /// </summary>
        public float JumpForce { get; set; } = -12.0f;

        /// <summary>
        /// Maximum fall speed (terminal velocity) in Platform mode.
        /// Velocidad máxima de caída (velocidad terminal) en modo Platform.
        /// </summary>
        public float MaxFallSpeed { get; set; } = 15.0f;

        /// <summary>
        /// Fast fall multiplier when pressing down in Platform mode.
        /// Multiplicador de caída rápida al presionar abajo en modo Platform.
        /// </summary>
        public float FastFallMultiplier { get; set; } = 2.5f;

        /// <summary>
        /// Whether the entity is currently on the ground (Platform mode).
        /// Si la entidad está actualmente en el suelo (modo Platform).
        /// </summary>
        public bool IsGrounded { get; set; } = false;

        /// <summary>
        /// Whether the entity is currently jumping (Platform mode).
        /// Si la entidad está actualmente saltando (modo Platform).
        /// </summary>
        public bool IsJumping { get; set; } = false;

        /// <summary>
        /// Whether the entity is currently fast falling (Platform mode).
        /// Si la entidad está actualmente cayendo en picado (modo Platform).
        /// </summary>
        public bool IsFastFalling { get; set; } = false;

        /// <summary>
        /// Direction the entity last faced (-1 left, 1 right). Used for animations.
        /// Dirección en la que la entidad miró por última vez (-1 izq, 1 der). Usado para animaciones.
        /// </summary>
        public int FacingDirection { get; set; } = 1;
    }

    /// <summary>
    /// Game type used to determine physics behavior.
    /// Tipo de juego para determinar el comportamiento de la fisica.
    /// </summary>
    public enum GameType
    {
        /// <summary>
        /// Top-down game behavior.
        /// Para videojuegos con vista de arriba hacia abajo (top-down).
        /// </summary>
        TopDown,
        /// <summary>
        /// Side-scroller/platformer behavior.
        /// Para videojuegos con vista lateral, plataformas, etc.
        /// </summary>
        Platform
    }
}
