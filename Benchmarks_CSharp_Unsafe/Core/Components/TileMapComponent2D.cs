using System.Runtime.InteropServices;
using Benchmarks.Core.Collections;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Components
{
    /// <summary>
    /// TileLayer with native int* data instead of int[].
    /// Name is FixedString32 instead of string.
    /// </summary>
    public unsafe struct TileLayer
    {
        public FixedString32 Name;
        public nint DataPtr; // int*
        public int DataLength;

        public int* Data
        {
            get => (int*)DataPtr;
        }
    }

    /// <summary>
    /// Collision entry for native collision map.
    /// </summary>
    public unsafe struct CollisionEntry
    {
        public int Gid;
        public nint RectsPtr; // Rectangle*
        public int RectCount;
    }

    /// <summary>
    /// Native collision map replaces Dictionary&lt;int, List&lt;Rectangle&gt;&gt;.
    /// Linear scan - efficient for small collision sets.
    /// </summary>
    public unsafe struct NativeCollisionMap : IDisposable
    {
        public nint EntriesPtr; // CollisionEntry*
        public int Count;

        public bool TryGetValue(int gid, out Rectangle* rects, out int rectCount)
        {
            CollisionEntry* entries = (CollisionEntry*)EntriesPtr;
            for (int i = 0; i < Count; i++)
            {
                ref CollisionEntry entry = ref Unsafe.Add(ref Unsafe.AsRef<CollisionEntry>(entries), i);
                if (entry.Gid == gid)
                {
                    rects = (Rectangle*)entry.RectsPtr;
                    rectCount = entry.RectCount;
                    return true;
                }
            }
            rects = null;
            rectCount = 0;
            return false;
        }

        public void Dispose()
        {
            if (EntriesPtr != 0)
            {
                CollisionEntry* entries = (CollisionEntry*)EntriesPtr;
                for (int i = 0; i < Count; i++)
                {
                    if (entries[i].RectsPtr != 0)
                        NativeMemory.Free((void*)entries[i].RectsPtr);
                }
                NativeMemory.Free((void*)EntriesPtr);
                EntriesPtr = 0;
            }
        }
    }

    public struct TileMapComponent2D
    {
        public int Width;
        public int Height;
        public int TileWidth;
        public int TileHeight;
        public YTB<TileLayer> TileLayers;
        public NativeCollisionMap Collisions;
    }
}
