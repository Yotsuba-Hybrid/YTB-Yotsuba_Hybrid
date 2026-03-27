using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Benchmarks.Core.Collections;
using Benchmarks.Core.Components;
using Benchmarks.Core.Types;

namespace Benchmarks.Benchmarks
{
    /// <summary>
    /// Benchmarks para la coleccion YTB&lt;T&gt; vs List&lt;T&gt; para comparacion.
    /// </summary>
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.HostProcess)]
    public class CollectionBenchmarks
    {
        [Params(100, 1000, 5000, 10000)]
        public int Count;

        private YTB<TransformComponent> _ytb;
        private List<TransformComponent> _list;
        private TransformComponent _sample;

        [GlobalSetup]
        public void Setup()
        {
            _sample = new TransformComponent(new Vector3(100, 200, 0), new Vector3(32, 32, 0));

            _ytb = new YTB<TransformComponent>();
            _list = new List<TransformComponent>(Count);

            for (int i = 0; i < Count; i++)
            {
                _ytb.Add(new TransformComponent(
                    new Vector3(i * 10f, i * 5f, 0),
                    new Vector3(32, 32, 0),
                    1f, SpriteEffects.None, Color.White
                ));
                _list.Add(new TransformComponent(
                    new Vector3(i * 10f, i * 5f, 0),
                    new Vector3(32, 32, 0),
                    1f, SpriteEffects.None, Color.White
                ));
            }
        }

        [Benchmark(Description = "YTB<T>.Add N elements")]
        public YTB<TransformComponent> YTB_Add()
        {
            var ytb = new YTB<TransformComponent>();
            for (int i = 0; i < Count; i++)
                ytb.Add(_sample);
            return ytb;
        }

        [Benchmark(Description = "List<T>.Add N elements")]
        public List<TransformComponent> List_Add()
        {
            var list = new List<TransformComponent>(500); // Same initial capacity as YTB
            for (int i = 0; i < Count; i++)
                list.Add(_sample);
            return list;
        }

        [Benchmark(Description = "YTB<T>.AsSpan() iterate")]
        public float YTB_IterateSpan()
        {
            float sum = 0;
            foreach (ref TransformComponent t in _ytb.AsSpan())
            {
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

        [Benchmark(Description = "YTB<T> ref index access")]
        public float YTB_IndexAccess()
        {
            float sum = 0;
            for (int i = 0; i < _ytb.Count; i++)
            {
                ref TransformComponent t = ref _ytb[i];
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
