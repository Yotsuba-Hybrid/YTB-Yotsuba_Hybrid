using System;
using System.Collections.Generic;
using System.Text;
using YotsubaEngine.Core.Entity;

namespace YotsubaEngine.Runtime.CPR.Events
{
    public readonly struct OnEntityRigidBody3DIsAdded(Yotsuba entity)
    {
        public Yotsuba Entity { get; } = entity;
    }
}

