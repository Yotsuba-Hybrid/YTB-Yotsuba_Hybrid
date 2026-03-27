namespace Benchmarks.Core.Scene
{
    /// <summary>
    /// Simple pub/sub event manager that replicates the engine's EventManager pattern.
    /// </summary>
    public class EventManager
    {
        private static EventManager _instance;
        public static EventManager Instance => _instance ??= new EventManager();

        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        public void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!_subscribers.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                _subscribers[type] = list;
            }
            list.Add(handler);
        }

        public void Publish<T>(T eventData)
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var list))
            {
                foreach (var handler in list)
                {
                    ((Action<T>)handler)(eventData);
                }
            }
        }

        public void Clear()
        {
            _subscribers.Clear();
        }

        public static void Reset()
        {
            _instance = new EventManager();
        }
    }
}
