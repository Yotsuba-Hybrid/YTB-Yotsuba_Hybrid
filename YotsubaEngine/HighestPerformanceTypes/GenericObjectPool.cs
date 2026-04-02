using System;
using System.Collections.Generic;
using System.Text;

namespace YotsubaEngine.HighestPerformanceTypes
{
    public class GenericObjectPool<T> where T : new()
    {
        private readonly Stack<T> _pool = new Stack<T>();

        public GenericObjectPool(int Capacity)
        {
            for(int i = 0; i < Capacity; i++)
            {
                _pool.Push(new T());
            }
        }

        public T Rent()
        {
            if(_pool.Count == 0)
            {
                return new T();
            }

            return _pool.Pop();
        }

        public void Return(T obj)
        {
            _pool.Push(obj);
        }
    }
}
