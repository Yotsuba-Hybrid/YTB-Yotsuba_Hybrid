using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    /// <summary>
    /// Function pointer vtable for dynamically registered systems.
    /// Replaces abstract class hierarchy with unmanaged dispatch.
    /// </summary>
    public unsafe struct SystemVTable
    {
        public delegate* managed<nint, EntityManager*, void> Initialize;
        public delegate* managed<nint, Yotsuba*, void> SharedEntityInit;
        public delegate* managed<nint, GameTime, void> Update;
        public delegate* managed<nint, Yotsuba*, GameTime, void> SharedEntityForEach;
        public delegate* managed<nint, GameTime, void> Render2D;
        public delegate* managed<nint, GameTime, void> Render3D;
        public delegate* managed<nint, void> Dispose;
    }

    public unsafe struct SystemEntry
    {
        public nint SystemPtr; // Pointer to system struct on native heap
        public SystemVTable VTable;
    }

    /// <summary>
    /// SystemBuilder using NativeMemory for dynamic system storage.
    /// Function pointers replace Func&lt;ISystem&gt; delegates.
    /// </summary>
    public unsafe struct SystemBuilder
    {
        private nint _emPtr;
        private nint _entriesPtr; // SystemEntry*
        private int _count;
        private int _capacity;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InitializeSystem(EntityManager* entities)
        {
            _emPtr = (nint)entities;
            SystemEntry* entries = (SystemEntry*)_entriesPtr;
            for (int i = 0; i < _count; i++)
            {
                ref SystemEntry e = ref Unsafe.Add(ref Unsafe.AsRef<SystemEntry>(entries), i);
                e.VTable.Initialize(e.SystemPtr, entities);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SharedEntityInitialize(Yotsuba* entity)
        {
            SystemEntry* entries = (SystemEntry*)_entriesPtr;
            for (int i = 0; i < _count; i++)
            {
                ref SystemEntry e = ref Unsafe.Add(ref Unsafe.AsRef<SystemEntry>(entries), i);
                e.VTable.SharedEntityInit(e.SystemPtr, entity);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateSystem(GameTime gameTime)
        {
            SystemEntry* entries = (SystemEntry*)_entriesPtr;
            for (int i = 0; i < _count; i++)
            {
                ref SystemEntry e = ref Unsafe.Add(ref Unsafe.AsRef<SystemEntry>(entries), i);
                e.VTable.Update(e.SystemPtr, gameTime);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SharedEntityForEachUpdate(Yotsuba* entity, GameTime gameTime)
        {
            SystemEntry* entries = (SystemEntry*)_entriesPtr;
            for (int i = 0; i < _count; i++)
            {
                ref SystemEntry e = ref Unsafe.Add(ref Unsafe.AsRef<SystemEntry>(entries), i);
                e.VTable.SharedEntityForEach(e.SystemPtr, entity, gameTime);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Render2D(GameTime gameTime)
        {
            SystemEntry* entries = (SystemEntry*)_entriesPtr;
            for (int i = 0; i < _count; i++)
            {
                ref SystemEntry e = ref Unsafe.Add(ref Unsafe.AsRef<SystemEntry>(entries), i);
                e.VTable.Render2D(e.SystemPtr, gameTime);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Render3D(GameTime gameTime)
        {
            SystemEntry* entries = (SystemEntry*)_entriesPtr;
            for (int i = 0; i < _count; i++)
            {
                ref SystemEntry e = ref Unsafe.Add(ref Unsafe.AsRef<SystemEntry>(entries), i);
                e.VTable.Render3D(e.SystemPtr, gameTime);
            }
        }

        public void Dispose()
        {
            SystemEntry* entries = (SystemEntry*)_entriesPtr;
            for (int i = 0; i < _count; i++)
            {
                ref SystemEntry e = ref Unsafe.Add(ref Unsafe.AsRef<SystemEntry>(entries), i);
                e.VTable.Dispose(e.SystemPtr);
                NativeMemory.Free((void*)e.SystemPtr);
            }
            if (_entriesPtr != 0)
            {
                NativeMemory.Free((void*)_entriesPtr);
                _entriesPtr = 0;
            }
            _count = 0;
        }
    }
}
