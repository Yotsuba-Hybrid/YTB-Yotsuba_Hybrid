using Benchmarks.Core.Types;

namespace Benchmarks.Core.Components
{
    /// <summary>
    /// Action delegate replaced with function pointer (delegate* managed&lt;void&gt;).
    /// Stored as nint for unmanaged struct compatibility.
    /// </summary>
    public unsafe struct ButtonComponent2D
    {
        public bool IsActive;
        public Rectangle EffectiveArea;
        public nint ActionPtr; // delegate* managed<void>

        public static void NoOp() { }

        public static readonly nint NoOpPtr = (nint)(delegate* managed<void>)&NoOp;
    }
}
