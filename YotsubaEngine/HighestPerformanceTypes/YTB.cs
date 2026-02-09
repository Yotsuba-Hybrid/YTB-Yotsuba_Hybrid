using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using YotsubaEngine.Core.Entity;

namespace YotsubaEngine.HighestPerformanceTypes
{
    /// <summary>
    /// High-performance growable array used by the engine.
    /// Arreglo dinámico de alto rendimiento usado por el motor.
    /// </summary>
    public class YTB<T> : IEnumerable<T>
    {
        /// <summary>
        /// Tamaños predeterminados para recalcular el tamaño de YTB.
        /// </summary>
        private readonly int[] _predetSizesOfYTB = [500, 1000, 5000, 10_000, 50_000, 100_000, 500_000, 1_000_000, 5_000_000, 10_000_000, 20_000_000, 30_000_000, 40_000_000 ,50_000_000];

        /// <summary>
        /// Almacena el índice actual en el arreglo de tamaños predeterminados.
        /// </summary>
        private int predetCurrentSizeIndex;

        /// <summary>
        /// Propiedad para acceder al índice actual en el arreglo de tamaños predeterminados.
        /// </summary>
        public int Capacity { get => _predetSizesOfYTB[predetCurrentSizeIndex]; private set => predetCurrentSizeIndex = value < 14 ? value : predetCurrentSizeIndex; }

        /// <summary>
        /// Iterable interno que almacena los elementos.
        /// </summary>
        private T[] _ytb { get; set; } 

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
            _ytb = new T[Capacity];
            Count = 0;
        }

        /// <summary>
        /// Agrega un elemento al final del arreglo, redimensionando si es necesario.\
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Retorna el indice del arreglo al que se introdujo</returns>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Add(T item)
        {
            if(Count >= Capacity)
            {
                predetCurrentSizeIndex++;
                if (predetCurrentSizeIndex >= _predetSizesOfYTB.Length)
                    throw new System.IndexOutOfRangeException("YTB has reached its maximum capacity.");

                T[] newArray = new T[Capacity];
                Array.Copy(_ytb, newArray, Count);
                _ytb = newArray;
            }
            _ytb[Count] = item;
            return ((Count++));
        }

        /// <summary>
        /// Agrega un elemento al final del arreglo, redimensionando si es necesario.\
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Retorna el indice del arreglo al que se introdujo</returns>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Add(T item, out int Index)
        {
            if (Count >= Capacity)
            {
                predetCurrentSizeIndex++;
                if (predetCurrentSizeIndex >= _predetSizesOfYTB.Length)
                    throw new System.IndexOutOfRangeException("YTB has reached its maximum capacity.");

                T[] newArray = new T[Capacity];
                Array.Copy(_ytb, newArray, Count);
                _ytb = newArray;
            }
            _ytb[Count] = item;
            Index = ((Count++));
            return Index;
        }

        /// <summary>
        /// remueve el elemento en la posicion index, y desplaza los elementos posteriores una posicion hacia adelante.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Retorna true si se pudo eliminar, false si el index esta fuera de rango</returns>
        public bool RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                return false;
            int shiftCount = Count - index - 1;
            if (shiftCount > 0)
            {
                Array.Copy(_ytb, index + 1, _ytb, index, shiftCount);
            }
            _ytb[--Count] = default!;
            return true;
        }

        /// <summary>
        /// Elimina la primera ocurrencia de un elemento especificado del YTB.
        /// </summary>
        /// <param name="item">El elemento a eliminar del YTB.</param>
        /// <remarks>Si el elemento no se encuentra, el YTB permanece sin cambios.</remarks>
        /// <returns>Retorna true si el elemento fue encontrado y eliminado; de lo contrario, false.</returns>
        public bool Remove(T item)
        {
            int index = Array.IndexOf(_ytb, item, 0, Count);
            if (index < 0)
                return false;
            return RemoveAt(index);
        }

        /// <summary>
        ///Este metod es para obtener un valor. Cuando index es int, se obtiene el objeto, cuando es uint, es para reemplazar el objeto en esa posicion.
        /// <param name="index"></param>
        /// <returns>Retorna una referencia del objeto</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// </summary>
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
                return ref _ytb[index];
            }
        }

        /// <summary>
        ///Este metod es para obtener un valor. Cuando index es int, se obtiene el objeto, cuando es uint, es para reemplazar el objeto en esa posicion.
        /// <param name="index"></param>
        /// <returns>Retorna una referencia del objeto</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// </summary>
        public ref T this[Yotsuba entity]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (entity.Id < 0 || entity.Id >= Count)
                    throw new ArgumentOutOfRangeException(nameof(entity.Id), "Index is out of range.");
                return ref _ytb[entity.Id];
            }
        }

        /// <summary>
        /// Este metodo es para settear un valor. Si quieres reemplazar un objeto en una posicion especifica. El index es uint, no int, debes hacer cast (uint)index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public T this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
                _ytb[index] = value;
            }
        }


        /// <summary>
        /// Devuelve una copia del arreglo;
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            T[] result = new T[Count];
            Array.Copy(_ytb, result, Count);
            return result;
        }

        /// <summary>
        /// Metodo que convierte el contenido del YTB a una lista de tipo List<T>.
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            List<T> List = new List<T>(Count);
            List.AddRange(_ytb.AsSpan(0, Count));
            return List;
        }

        /// <summary>
        /// Devuelve una vista de solo lectura del arreglo como ReadOnlySpan<T>.
        /// </summary>
        /// <returns></returns>
        public ReadOnlySpan<T> AsReadOnlySpan()
        {
            return _ytb.AsSpan(0, Count);
        }

        /// <summary>
        /// Devuelve una vista del arreglo como Span<T>.
        /// </summary>
        /// <returns></returns>
        public Span<T> AsSpan()
        {
            return _ytb.AsSpan(0, Count);
        }

        /// <summary>
        /// Limpia el arreglo, reseteando el conteo y capacidad.
        /// </summary>
        public void Clear()
        {
            Count = 0;
            Capacity = 0;
            Array.Clear(_ytb, 0, Capacity);
        }

        /// <summary>
        /// Implementacion de la interfaz IEnumerable<T> para permitir la iteracion sobre los elementos del YTB.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator()
        {
            foreach(var item in _ytb.Take(Count)) yield return item;
        }

        /// <summary>
        /// La implementación no genérica de IEnumerable.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
