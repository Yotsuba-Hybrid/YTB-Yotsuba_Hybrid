
using System.Collections.Generic;
using YotsubaEngine.HighestPerformanceTypes;

namespace YotsubaEngine.Core.Component.C_3D
{
    public struct ListObject3D
    {
        
        public YTB<int> Object3Ds { get; set; }

        public bool IsVisible { get; set; }

        public ListObject3D()
        {
            Object3Ds = new();
        }
    }
}
