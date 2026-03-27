using Benchmarks.Core.Types;

namespace Benchmarks.Core.Components
{
    public struct AnimationData
    {
        public int CurrentFrame { get; set; }
        public int TotalFrames { get; set; }
        public float FrameTime { get; set; }
        public float ElapsedTime { get; set; }
        public bool IsFinished { get; set; }
        public bool FinishedWasMarked { get; set; }
        public bool Loop { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public int TextureId { get; set; }

        public void Reset()
        {
            CurrentFrame = 0;
            ElapsedTime = 0;
            IsFinished = false;
            FinishedWasMarked = false;
        }

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

    public struct AnimationComponent2D
    {
        private Dictionary<AnimationType, AnimationData> Animations;
        public ValueTuple<AnimationType, AnimationData> CurrentAnimationType { get; set; }

        public AnimationComponent2D()
        {
            Animations = new Dictionary<AnimationType, AnimationData>();
            CurrentAnimationType = (AnimationType.None, default);
        }

        public void AddAnimation(AnimationType type, AnimationData animation)
        {
            Animations ??= new Dictionary<AnimationType, AnimationData>();
            Animations[type] = animation;
        }

        public void RemoveAnimation(AnimationType type)
        {
            Animations?.Remove(type);
        }

        public AnimationData GetAnimation(AnimationType type)
        {
            if (Animations != null && Animations.TryGetValue(type, out var anim))
                return anim;
            return default;
        }

        public bool ContainsAnimation(AnimationType type)
        {
            return Animations != null && Animations.ContainsKey(type);
        }

        public void ActivateAnimation(AnimationType type)
        {
            if (Animations != null && Animations.TryGetValue(type, out var anim))
                CurrentAnimationType = (type, anim);
        }
    }
}
