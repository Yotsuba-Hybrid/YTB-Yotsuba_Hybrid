using Microsoft.Xna.Framework;

namespace YotsubaEngine.Events
{
    /// <summary>
    /// Evento agnóstico que se dispara cuando dos entidades 3D colisionan.
    /// </summary>
    public struct OnEntity3DCollide
    {
        public int EntityA_Id { get; }
        public int EntityB_Id { get; }
        public Vector3 CollisionNormal { get; }
        public float PenetrationDepth { get; }

        /// <summary>Nombre del hueso impactado (o genérico si es YTB/Sprite)</summary>
        public string HitPartA { get; }
        public string HitPartB { get; }

        public OnEntity3DCollide(int entityA, int entityB, Vector3 normal, float penetration, string hitA = null, string hitB = null)
        {
            EntityA_Id = entityA;
            EntityB_Id = entityB;
            CollisionNormal = normal;
            PenetrationDepth = penetration;
            HitPartA = hitA;
            HitPartB = hitB;
        }
    }
}