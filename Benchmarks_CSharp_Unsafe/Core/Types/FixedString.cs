using System.Runtime.CompilerServices;

namespace Benchmarks.Core.Types
{
    /// <summary>
    /// Fixed-size char buffer. Replaces string with zero heap allocation.
    /// </summary>
    public unsafe struct FixedString32
    {
        public fixed char Chars[32];
        public int Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedString32(ReadOnlySpan<char> value)
        {
            Length = Math.Min(value.Length, 32);
            fixed (char* dst = Chars)
            {
                for (int i = 0; i < Length; i++)
                    dst[i] = Unsafe.Add(ref Unsafe.AsRef<char>(in value.GetPinnableReference()), i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(ReadOnlySpan<char> value, StringComparison comparison)
        {
            fixed (char* ptr = Chars)
            {
                var span = new ReadOnlySpan<char>(ptr, Length);
                return span.Contains(value, comparison);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedString32 FromInt(ReadOnlySpan<char> prefix, int number)
        {
            Unsafe.SkipInit(out FixedString32 result);
            Span<char> buffer = stackalloc char[32];
            prefix.CopyTo(buffer);
            int len = prefix.Length;
            if (number.TryFormat(buffer.Slice(len), out int written))
                len += written;
            result.Length = Math.Min(len, 32);
            fixed (char* dst = result.Chars)
            {
                for (int i = 0; i < result.Length; i++)
                    dst[i] = buffer[i];
            }
            return result;
        }
    }

    /// <summary>
    /// Deterministic PRNG. Replaces System.Random with zero heap allocation.
    /// </summary>
    public struct XorShift32
    {
        private uint _state;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public XorShift32(uint seed) => _state = seed == 0 ? 1 : seed;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Next()
        {
            uint x = _state;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            _state = x;
            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Next(int min, int max) => min + (int)(Next() % (uint)(max - min));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float NextSingle() => (Next() & 0x7FFFFF) / (float)0x800000;
    }
}
