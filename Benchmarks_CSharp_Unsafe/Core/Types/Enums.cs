namespace Benchmarks.Core.Types
{
    public enum SpriteEffects
    {
        None = 0,
        FlipHorizontally = 1,
        FlipVertically = 2,
    }

    public enum GameType
    {
        TopDown,
        Platform,
    }

    public enum MassLevel
    {
        Collision = 1,
        NoCollision = 2,
        Slow = 3,
    }

    public enum AnimationType
    {
        None,
        Idle,
        Walk,
        Run,
        Jump,
        Crouch,
        Attack,
        Hurt,
        Die,
    }
}
