using System.Runtime.CompilerServices;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Components
{
    public struct AnimationData
    {
        public int CurrentFrame;
        public int TotalFrames;
        public float FrameTime;
        public float ElapsedTime;
        public bool IsFinished;
        public bool FinishedWasMarked;
        public bool Loop;
        public int FrameWidth;
        public int FrameHeight;
        public int TextureId;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            CurrentFrame = 0;
            ElapsedTime = 0;
            IsFinished = false;
            FinishedWasMarked = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle CurrentFrameRect(GameTime gameTime)
        {
            ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (ElapsedTime >= FrameTime)
            {
                ElapsedTime -= FrameTime;
                if (CurrentFrame < TotalFrames - 1)
                    CurrentFrame++;
                else if (Loop)
                    CurrentFrame = 0;
                else
                    IsFinished = true;
            }
            return new Rectangle(CurrentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
        }
    }

    /// <summary>
    /// InlineArray replaces Dictionary&lt;AnimationType, AnimationData&gt;.
    /// AnimationType has 9 values (0-8) → fixed 9 slots, bitmask for presence.
    /// </summary>
    [InlineArray(9)]
    public struct AnimationSlots
    {
        private AnimationData _element0;
    }

    public struct AnimationComponent2D
    {
        private AnimationSlots _animations;
        private int _animationMask;
        public AnimationType CurrentType;
        public AnimationData CurrentData;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddAnimation(AnimationType type, AnimationData animation)
        {
            _animations[(int)type] = animation;
            _animationMask |= (1 << (int)type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAnimation(AnimationType type)
        {
            _animationMask &= ~(1 << (int)type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AnimationData GetAnimation(AnimationType type) => _animations[(int)type];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsAnimation(AnimationType type) => (_animationMask & (1 << (int)type)) != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ActivateAnimation(AnimationType type)
        {
            if (ContainsAnimation(type))
            {
                CurrentType = type;
                CurrentData = _animations[(int)type];
            }
        }
    }
}
