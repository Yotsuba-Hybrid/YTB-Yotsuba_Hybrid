
using Microsoft.Xna.Framework;
using static YotsubaEngine.Core.Component.C_AGNOSTIC.RigidBody;

namespace YotsubaEngine.Core.Component.C_2D
{
    /// <summary>
    /// Componente que añade la funcionalidad de física básica a un objeto 2D.
    /// <para>Component that adds basic 2D physics behavior.</para>
    /// </summary>
    /// <param name="gameType">Tipo de juego usado para la física.<para>Game type used for physics behavior.</para></param>
    /// <param name="mass">Nivel de masa del cuerpo.<para>Mass level for the body.</para></param>
    public struct RigidBodyComponent2D(GameType gameType, MassLevel mass)
    {
        /// <summary>
        /// Velocidad base del objeto.
        /// <para>Base movement speed.</para>
        /// </summary>
        public float SPEED = 1.0f;

        /// <summary>
        /// Velocidad máxima del objeto.
        /// <para>Maximum movement speed.</para>
        /// </summary>
        public float TOP_SPEED = 3.0f;

        /// <summary>
        /// Desfase respecto a la colisión.
        /// <para>Collision offset.</para>
        /// </summary>
        public Vector2 OffSetCollision { get; set; } = Vector2.Zero;

        /// <summary>
        /// Determina el comportamiento de la física según el tipo de juego.
        /// <para>Determines physics behavior based on game type.</para>
        /// </summary>
        public GameType GameType { get; set; } = gameType;

        /// <summary>
        /// Velocidad del objeto en el espacio 3D.
        /// <para>Current velocity in 3D space.</para>
        /// </summary>
        public Vector3 Velocity { get; set; } = Vector3.Zero;

        /// <summary>
        /// Masa del objeto para determinar su inercia y resistencia a fuerzas.
        /// <para>Mass used to determine inertia and resistance.</para>
        /// </summary>
        public MassLevel Mass { get; set; } = mass;

        // ========== PLATFORM PHYSICS PROPERTIES ==========

        /// <summary>
        /// Fuerza de gravedad aplicada en modo Platform.
        /// <para>Gravity strength applied in Platform mode.</para>
        /// </summary>
        public float Gravity { get; set; } = 0.5f;

        /// <summary>
        /// Fuerza de salto aplicada al saltar en modo Platform.
        /// <para>Jump force applied when jumping in Platform mode.</para>
        /// </summary>
        public float JumpForce { get; set; } = -12.0f;

        /// <summary>
        /// Velocidad máxima de caída (velocidad terminal) en modo Platform.
        /// <para>Maximum fall speed (terminal velocity) in Platform mode.</para>
        /// </summary>
        public float MaxFallSpeed { get; set; } = 15.0f;

        /// <summary>
        /// Multiplicador de caída rápida al presionar abajo en modo Platform.
        /// <para>Fast fall multiplier when pressing down in Platform mode.</para>
        /// </summary>
        public float FastFallMultiplier { get; set; } = 2.5f;

        /// <summary>
        /// Si la entidad está actualmente en el suelo (modo Platform).
        /// <para>Whether the entity is currently on the ground (Platform mode).</para>
        /// </summary>
        public bool IsGrounded { get; set; } = false;

        /// <summary>
        /// Si la entidad está actualmente saltando (modo Platform).
        /// <para>Whether the entity is currently jumping (Platform mode).</para>
        /// </summary>
        public bool IsJumping { get; set; } = false;

        /// <summary>
        /// Si la entidad está actualmente cayendo en picado (modo Platform).
        /// <para>Whether the entity is currently fast falling (Platform mode).</para>
        /// </summary>
        public bool IsFastFalling { get; set; } = false;

        /// <summary>
        /// Dirección en la que la entidad miró por última vez (-1 izq, 1 der). Usado para animaciones.
        /// <para>Direction the entity last faced (-1 left, 1 right). Used for animations.</para>
        /// </summary>
        public int FacingDirection { get; set; } = 1;
    }

    /// <summary>
    /// Tipo de juego para determinar el comportamiento de la física.
    /// <para>Game type used to determine physics behavior.</para>
    /// </summary>
    public enum GameType
    {
        /// <summary>
        /// Para videojuegos con vista de arriba hacia abajo (top-down).
        /// <para>Top-down game behavior.</para>
        /// </summary>
        TopDown,
        /// <summary>
        /// Para videojuegos con vista lateral, plataformas, etc.
        /// <para>Side-scroller/platformer behavior.</para>
        /// </summary>
        Platform
    }
}
