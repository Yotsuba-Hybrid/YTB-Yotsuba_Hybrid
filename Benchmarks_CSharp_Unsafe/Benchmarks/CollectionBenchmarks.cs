using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Benchmarks.Core.Collections;
using Benchmarks.Core.Components;
using Benchmarks.Core.Types;

namespace Benchmarks.Benchmarks
{
    /// <summary>
    /// Benchmarks: NativeMemory-backed YTB&lt;T&gt; vs managed List&lt;T&gt;.
    /// Iteration via Unsafe.Add pointer traversal vs foreach.
    /// </summary>
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.HostProcess)]
    public unsafe class CollectionBenchmarks
    {
        [Params(100, 1000, 5000, 10000)]
        public int Count;

        private nint _ytbNint; // YTB<TransformComponent>* stored for benchmark class embedding
        private List<TransformComponent> _list;
        private TransformComponent _sample;

        [GlobalSetup]
        public void Setup()
        {
            _sample = new TransformComponent(new Vector3(100, 200, 0), new Vector3(32, 32, 0));

            YTB<TransformComponent>* ytbPtr = (YTB<TransformComponent>*)System.Runtime.InteropServices.NativeMemory.AllocZeroed(
                (nuint)sizeof(YTB<TransformComponent>));
            *ytbPtr = YTB<TransformComponent>.Create();
            _ytbNint = (nint)ytbPtr;

            _list = new List<TransformComponent>(Count);

            for (int i = 0; i < Count; i++)
            {
                var tc = new TransformComponent(
                    new Vector3(i * 10f, i * 5f, 0),
                    new Vector3(32, 32, 0),
                    1f, SpriteEffects.None, Color.White
                );
                ytbPtr->Add(tc);
                _list.Add(tc);
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            if (_ytbNint != 0)
            {
                YTB<TransformComponent>* ytbPtr = (YTB<TransformComponent>*)_ytbNint;
                ytbPtr->Dispose();
                System.Runtime.InteropServices.NativeMemory.Free(ytbPtr);
                _ytbNint = 0;
            }
        }

        [Benchmark(Description = "YTB<T>.Add N elements")]
        public nint YTB_Add()
        {
            var ytb = YTB<TransformComponent>.Create();
            for (int i = 0; i < Count; i++)
                ytb.Add(_sample);
            nint ptr = (nint)ytb.Ptr;
            ytb.Dispose();
            return ptr;
        }

        [Benchmark(Description = "List<T>.Add N elements")]
        public List<TransformComponent> List_Add()
        {
            var list = new List<TransformComponent>(500);
            for (int i = 0; i < Count; i++)
                list.Add(_sample);
            return list;
        }

        [Benchmark(Description = "YTB<T> pointer iterate (Unsafe.Add)")]
        public float YTB_IteratePointer()
        {
            YTB<TransformComponent>* ytbPtr = (YTB<TransformComponent>*)_ytbNint;
            float sum = 0;
            TransformComponent* ptr = ytbPtr->Ptr;
            int count = ytbPtr->Count;
            for (int i = 0; i < count; i++)
            {
                ref TransformComponent t = ref Unsafe.Add(ref Unsafe.AsRef<TransformComponent>(ptr), i);
                sum += t.Position.X + t.Position.Y;
            }
            return sum;
        }

        [Benchmark(Description = "List<T> foreach iterate")]
        public float List_Iterate()
        {
            float sum = 0;
            foreach (TransformComponent t in _list)
            {
                sum += t.Position.X + t.Position.Y;
            }
            return sum;
        }

        [Benchmark(Description = "YTB<T> ref index access (Unsafe.Add)")]
        public float YTB_IndexAccess()
        {
            YTB<TransformComponent>* ytbPtr = (YTB<TransformComponent>*)_ytbNint;
            float sum = 0;
            int count = ytbPtr->Count;
            for (int i = 0; i < count; i++)
            {
                ref TransformComponent t = ref (*ytbPtr)[i];
                sum += t.Position.X;
            }
            return sum;
        }

        [Benchmark(Description = "List<T> index access")]
        public float List_IndexAccess()
        {
            float sum = 0;
            for (int i = 0; i < _list.Count; i++)
            {
                var t = _list[i];
                sum += t.Position.X;
            }
            return sum;
        }
    }
}
