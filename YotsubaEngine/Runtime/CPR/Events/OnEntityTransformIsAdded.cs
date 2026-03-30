using System;
using System.Collections.Generic;
using System.Text;
using YotsubaEngine.Core.Entity;

namespace YotsubaEngine.Runtime.CPR.Events
{
    public readonly struct OnEntityTransformIsAdded(Yotsuba entity)
    {
        public readonly Yotsuba Entity { get; } = entity;
    }
}
