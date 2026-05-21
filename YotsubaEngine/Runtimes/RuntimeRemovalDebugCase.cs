using System;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_3D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Runtime.CPR;
using YotsubaEngine.Runtime.RPR;

namespace YotsubaEngine.Runtimes
{
    internal static class RuntimeRemovalDebugCase
    {
        internal static bool ValidateRemovedIdsAreNotPersisted()
        {
            EntityManager manager = new();
            Yotsuba a = new(0);
            manager.AddEntity(ref a);
            manager.AddTransformComponent(a, new TransformComponent());
            manager.AddRigidbody3DComponent(a, new RigidBodyComponent3D());
            manager.AddModelComponent3D(a, new ModelComponent3D());

            var cpr = new Collision_Prediction_Runtime_3D();
            cpr.InitializeSystem(manager);
            var rpr = new Render_Prediction_Runtime_3D();
            rpr.InitializeSystem(manager);

            manager.RemoveRigidbody3DComponent(a.Id);
            manager.SetSprite2_5D(a.Id, false);
            manager.RemoveModelComponent3D(a.Id);

            bool removedFromCPR = !cpr.Entities.AsReadOnlySpan().Contains(a.Id);
            bool removedFromRPR = !rpr.GetEntitieIdsCanRender3D().Contains(a.Id);

            cpr.Dispose();
            rpr.Dispose();
            return removedFromCPR && removedFromRPR;
        }
    }
}
