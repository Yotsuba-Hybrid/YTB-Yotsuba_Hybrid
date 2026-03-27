using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Benchmarks.Core.Collections
{
    /// <summary>
    /// Fully unmanaged growable array using NativeMemory.
    /// Zero heap allocations. Pointer-based iteration. No bounds checking.
    /// </summary>
    public unsafe struct YTB<T> : IDisposable where T : unmanaged
    {
        private static ReadOnlySpan<int> PresetSizes =>
            [500, 1000, 5000, 10_000, 50_000, 100_000, 500_000, 1_000_000, 5_000_000, 10_000_000, 20_000_000, 30_000_000, 40_000_000, 50_000_000];

        private nint _ptr; // T* stored as nint for managed-class embedding
        private int _count;
        private int _capacityIndex;

        public T* Ptr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (T*)_ptr;
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _count;
        }

        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => PresetSizes[_capacityIndex];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YTB<T> Create()
        {
            Unsafe.SkipInit(out YTB<T> ytb);
            ytb._capacityIndex = 0;
            int cap = PresetSizes[0];
            ytb._ptr = (nint)NativeMemory.AllocZeroed((nuint)cap, (nuint)sizeof(T));
            ytb._count = 0;
            return ytb;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Add(T item)
        {
            if (_count >= Capacity)
            {
                _capacityIndex++;
                if (_capacityIndex >= PresetSizes.Length)
                    throw new IndexOutOfRangeException("YTB has reached its maximum capacity.");

                T* newPtr = (T*)NativeMemory.AllocZeroed((nuint)Capacity, (nuint)sizeof(T));
                Unsafe.CopyBlock(newPtr, (T*)_ptr, (uint)(_count * sizeof(T)));
                NativeMemory.Free((T*)_ptr);
                _ptr = (nint)newPtr;
            }
            ref T slot = ref Unsafe.Add(ref Unsafe.AsRef<T>((T*)_ptr), _count);
            slot = item;
            return _count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Add(T item, out int index)
        {
            if (_count >= Capacity)
            {
                _capacityIndex++;
                if (_capacityIndex >= PresetSizes.Length)
                    throw new IndexOutOfRangeException("YTB has reached its maximum capacity.");

                T* newPtr = (T*)NativeMemory.AllocZeroed((nuint)Capacity, (nuint)sizeof(T));
                Unsafe.CopyBlock(newPtr, (T*)_ptr, (uint)(_count * sizeof(T)));
                NativeMemory.Free((T*)_ptr);
                _ptr = (nint)newPtr;
            }
            ref T slot = ref Unsafe.Add(ref Unsafe.AsRef<T>((T*)_ptr), _count);
            slot = item;
            index = _count;
            return _count++;
        }

        public bool RemoveAt(int index)
        {
            if (index < 0 || index >= _count)
                return false;
            int shiftCount = _count - index - 1;
            if (shiftCount > 0)
            {
                T* src = (T*)_ptr + index + 1;
                T* dst = (T*)_ptr + index;
                Unsafe.CopyBlock(dst, src, (uint)(shiftCount * sizeof(T)));
            }
            --_count;
            Unsafe.Add(ref Unsafe.AsRef<T>((T*)_ptr), _count) = default;
            return true;
        }

        /// <summary>
        /// Indexer with NO bounds checking. Raw pointer access via Unsafe.Add.
        /// </summary>
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Unsafe.Add(ref Unsafe.AsRef<T>((T*)_ptr), index);
        }

        /// <summary>
        /// Set by uint index. No bounds checking.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(uint index, T value)
        {
            Unsafe.Add(ref Unsafe.AsRef<T>((T*)_ptr), (int)index) = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan() => new Span<T>((T*)_ptr, _count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> AsReadOnlySpan() => new ReadOnlySpan<T>((T*)_ptr, _count);

        /// <summary>
        /// Find using pointer traversal. Returns null if not found.
        /// </summary>
        public T* Find(delegate* managed<T*, bool> predicate)
        {
            T* ptr = (T*)_ptr;
            for (int i = 0; i < _count; i++)
            {
                if (predicate(ptr + i))
                    return ptr + i;
            }
            return null;
        }

        public void Clear()
        {
            _count = 0;
            _capacityIndex = 0;
            NativeMemory.Clear((T*)_ptr, (nuint)(PresetSizes[0] * sizeof(T)));
        }

        public void Dispose()
        {
            if (_ptr != 0)
            {
                NativeMemory.Free((void*)_ptr);
                _ptr = 0;
            }
            _count = 0;
        }
    }
}
