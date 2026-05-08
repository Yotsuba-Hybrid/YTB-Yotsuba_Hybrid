namespace YotsubaEngine.Physics.RigidBody
{
    /// <summary>
    /// Niveles de colisión para determinar si un objeto es sólido o trigger (atraviesa).
    /// <para>Collision levels to determine if an object is solid or trigger (pass-through).</para>
    /// </summary>
    public enum CollisionLevel
    {
        /// <summary>
        /// Objeto sólido que colisiona.
        /// <para>Solid object that collides.</para>
        /// </summary>
        Solid = 1,

        /// <summary>
        /// Objeto trigger que atraviesa sin colisión física.
        /// <para>Trigger object that passes through without physical collision.</para>
        /// </summary>
        Trigger = 2,
    }
}
