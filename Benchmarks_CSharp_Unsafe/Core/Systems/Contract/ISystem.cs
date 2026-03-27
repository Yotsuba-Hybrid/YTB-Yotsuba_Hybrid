using System.Runtime.CompilerServices;
using Benchmarks.Core.Scene;

namespace Benchmarks.Core.Systems.Contract
{
    /// <summary>
    /// Static helper for systems. Replaces abstract base class with zero-cost inlined methods.
    /// Systems are structs that store EntityManager* as nint.
    /// </summary>
    public static unsafe class SystemHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EntityManager* GetEM(nint emPtr) => (EntityManager*)emPtr;
    }
}
