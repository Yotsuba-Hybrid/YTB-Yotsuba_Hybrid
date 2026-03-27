using Benchmarks.Core.Types;

namespace Benchmarks.Core.Components
{
    public struct RigidBodyComponent2D
    {
        public float SPEED;
        public float TOP_SPEED;
        public Vector2 OffSetCollision;
        public GameType GameType;
        public Vector3 Velocity;
        public MassLevel Mass;
        public float Gravity;
        public float JumpForce;
        public float MaxFallSpeed;
        public float FastFallMultiplier;
        public bool IsGrounded;
        public bool IsJumping;
        public bool IsFastFalling;
        public int FacingDirection;

        public RigidBodyComponent2D(GameType gameType, MassLevel mass)
        {
            SPEED = 1.0f;
            TOP_SPEED = 3.0f;
            OffSetCollision = Vector2.Zero;
            GameType = gameType;
            Velocity = Vector3.Zero;
            Mass = mass;
            Gravity = 0.5f;
            JumpForce = -12.0f;
            MaxFallSpeed = 15.0f;
            FastFallMultiplier = 2.5f;
            IsGrounded = false;
            IsJumping = false;
            IsFastFalling = false;
            FacingDirection = 1;
        }
    }
}
