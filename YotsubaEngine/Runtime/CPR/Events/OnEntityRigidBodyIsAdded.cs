using System;
using System.Collections.Generic;
using System.Text;
using YotsubaEngine.Core.Entity;

namespace YotsubaEngine.Runtime.CPR.Events
{
    public struct OnEntityRigidBodyIsAdded(Yotsuba entity)
    {
        public Yotsuba Entity { get; } = entity;
    }
}
