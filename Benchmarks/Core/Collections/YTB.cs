using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Benchmarks.Core.Entity;

namespace Benchmarks.Core.Collections
{
    /// <summary>
    /// Arreglo dinamico de alto rendimiento usado por el motor.
    /// <para>High-performance growable array used by the engine.</para>
    /// </summary>
    public class YTB<T>
    {
        /// <summary>
        /// Tamanos predeterminados para recalcular el tamano de YTB.
        /// </summary>
        private readonly int[] _predetSizesOfYTB = [500, 1000, 5000, 10_000, 50_000, 100_000, 500_000, 1_000_000, 5_000_000, 10_000_000, 20_000_000, 30_000_000, 40_000_000, 50_000_000];

        /// <summary>
        /// Almacena el indice actual en el arreglo de tamanos predeterminados.
        /// </summary>
        private int predetCurrentSizeIndex;

        /// <summary>
        /// Propiedad para acceder al indice actual en el arreglo de tamanos predeterminados.
        /// </summary>
        public int Capacity { get => _predetSizesOfYTB[predetCurrentSizeIndex]; private set => predetCurrentSizeIndex = value < 14 ? value : predetCurrentSizeIndex; }

        /// <summary>
        /// Iterable interno que almacena los elementos.
        /// </summary>
        internal T[] _arr { get; set; }

        public IEnumerable<T> _ytb => _arr.Take(Count);

        /// <summary>
        /// Numero de elementos en el arreglo.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase YTB con capacidad inicial predeterminada.
        /// </summary>
        public YTB()
        {
            predetCurrentSizeIndex = 0;
            _arr = new T[Capacity];
            Count = 0;
        }

        /// <summary>
        /// Agrega un elemento al final del arreglo, redimensionando si es necesario.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Add(T item)
        {
            if (Count >= Capacity)
            {
                predetCurrentSizeIndex++;
                if (predetCurrentSizeIndex >= _predetSizesOfYTB.Length)
                    throw new System.IndexOutOfRangeException("YTB has reached its maximum capacity.");

                T[] newArray = new T[Capacity];
                Array.Copy(_arr, newArray, Count);
                _arr = newArray;
            }
            _arr[Count] = item;
            return ((Count++));
        }

        /// <summary>
        /// Agrega un elemento al final del arreglo, redimensionando si es necesario.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Add(T item, out int Index)
        {
            if (Count >= Capacity)
            {
                predetCurrentSizeIndex++;
                if (predetCurrentSizeIndex >= _predetSizesOfYTB.Length)
                    throw new System.IndexOutOfRangeException("YTB has reached its maximum capacity.");

                T[] newArray = new T[Capacity];
                Array.Copy(_arr, newArray, Count);
                _arr = newArray;
            }
            _arr[Count] = item;
            Index = ((Count++));
            return Index;
        }

        /// <summary>
        /// Remueve el elemento en la posicion indicada y desplaza los elementos posteriores.
        /// </summary>
        public bool RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                return false;
            int shiftCount = Count - index - 1;
            if (shiftCount > 0)
            {
                Array.Copy(_arr, index + 1, _arr, index, shiftCount);
            }
            _arr[--Count] = default!;
            return true;
        }

        /// <summary>
        /// Elimina la primera ocurrencia de un elemento especificado del YTB.
        /// </summary>
        public bool Remove(T item)
        {
            int index = Array.IndexOf(_arr, item, 0, Count);
            if (index < 0)
                return false;
            return RemoveAt(index);
        }

        /// <summary>
        /// Obtiene una referencia del objeto en la posicion indicada.
        /// </summary>
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
                return ref _arr[index];
            }
        }

        /// <summary>
        /// Obtiene una referencia del objeto asociado a la entidad indicada.
        /// </summary>
        public ref T this[Yotsuba entity]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (entity.Id < 0 || entity.Id >= Count)
                    throw new ArgumentOutOfRangeException(nameof(entity.Id), "Index is out of range.");
                return ref _arr[entity.Id];
            }
        }

        /// <summary>
        /// Establece un valor en una posicion especifica usando indice uint.
        /// </summary>
        public T this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
                _arr[index] = value;
            }
        }

        public T[] ToArray()
        {
            T[] result = new T[Count];
            Array.Copy(_arr, result, Count);
            return result;
        }

        public List<T> ToList()
        {
            List<T> List = new List<T>(Count);
            List.AddRange(_arr.AsSpan(0, Count));
            return List;
        }

        public ReadOnlySpan<T> AsReadOnlySpan()
        {
            return _arr.AsSpan(0, Count);
        }

        public Span<T> AsSpan()
        {
            return _arr.AsSpan(0, Count);
        }

        public void Clear()
        {
            Count = 0;
            Capacity = 0;
            Array.Clear(_arr, 0, Capacity);
        }

        T DefaultValue = default(T);
        public ref T Find(Predicate<T> predicate)
        {
            int index = Array.FindIndex(_arr, 0, Count, predicate);
            if (index == -1)
            {
                return ref DefaultValue;
            }
            return ref _arr[index];
        }
    }
}
