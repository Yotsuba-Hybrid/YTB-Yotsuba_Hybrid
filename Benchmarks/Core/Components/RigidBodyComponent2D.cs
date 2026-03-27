using Benchmarks.Core.Types;

namespace Benchmarks.Core.Components
{
    public struct RigidBodyComponent2D(GameType gameType, MassLevel mass)
    {
        public float SPEED = 1.0f;
        public float TOP_SPEED = 3.0f;
        public Vector2 OffSetCollision { get; set; } = Vector2.Zero;
        public GameType GameType { get; set; } = gameType;
        public Vector3 Velocity { get; set; } = Vector3.Zero;
        public MassLevel Mass { get; set; } = mass;

        // Platform physics properties
        public float Gravity { get; set; } = 0.5f;
        public float JumpForce { get; set; } = -12.0f;
        public float MaxFallSpeed { get; set; } = 15.0f;
        public float FastFallMultiplier { get; set; } = 2.5f;
        public bool IsGrounded { get; set; } = false;
        public bool IsJumping { get; set; } = false;
        public bool IsFastFalling { get; set; } = false;
        public int FacingDirection { get; set; } = 1;
    }
}
