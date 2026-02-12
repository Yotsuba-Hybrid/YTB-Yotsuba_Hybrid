using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using YotsubaEngine.Core.Entity;

namespace YotsubaEngine.HighestPerformanceTypes
{
    /// <summary>
    /// Arreglo dinámico de alto rendimiento usado por el motor.
    /// <para>High-performance growable array used by the engine.</para>
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
        /// <para>Property to access the current index in the preset size array.</para>
        /// </summary>
        public int Capacity { get => _predetSizesOfYTB[predetCurrentSizeIndex]; private set => predetCurrentSizeIndex = value < 14 ? value : predetCurrentSizeIndex; }

        /// <summary>
        /// Iterable interno que almacena los elementos.
        /// </summary>
        private T[] _ytb { get; set; } 

        /// <summary>
        /// Número de elementos en el arreglo.
        /// <para>Number of elements in the array.</para>
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase YTB con capacidad inicial predeterminada.
        /// <para>Initializes a new YTB instance with the default initial capacity.</para>
        /// </summary>
        public YTB()
        {
            predetCurrentSizeIndex = 0;
            _ytb = new T[Capacity];
            Count = 0;
        }

        /// <summary>
        /// Agrega un elemento al final del arreglo, redimensionando si es necesario.
        /// <para>Adds an element to the end of the array, resizing if needed.</para>
        /// </summary>
        /// <param name="item">Elemento a agregar. <para>Item to add.</para></param>
        /// <returns>Retorna el índice del arreglo al que se introdujo. <para>Returns the index where the item was inserted.</para></returns>
        /// <exception cref="System.IndexOutOfRangeException">Se excede la capacidad máxima de YTB. <para>Thrown when YTB exceeds its maximum capacity.</para></exception>
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
        /// Agrega un elemento al final del arreglo, redimensionando si es necesario.
        /// <para>Adds an element to the end of the array, resizing if needed.</para>
        /// </summary>
        /// <param name="item">Elemento a agregar. <para>Item to add.</para></param>
        /// <param name="Index">Índice donde se insertó. <para>Index where the item was inserted.</para></param>
        /// <returns>Retorna el índice del arreglo al que se introdujo. <para>Returns the index where the item was inserted.</para></returns>
        /// <exception cref="System.IndexOutOfRangeException">Se excede la capacidad máxima de YTB. <para>Thrown when YTB exceeds its maximum capacity.</para></exception>
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
        /// Remueve el elemento en la posición indicada y desplaza los elementos posteriores una posición hacia adelante.
        /// <para>Removes the element at the specified position and shifts subsequent elements forward.</para>
        /// </summary>
        /// <param name="index">Índice del elemento a eliminar. <para>Index of the element to remove.</para></param>
        /// <returns>Retorna true si se pudo eliminar; false si el índice está fuera de rango. <para>Returns true if removed; otherwise, false when the index is out of range.</para></returns>
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
        /// <para>Removes the first occurrence of the specified element from the YTB.</para>
        /// </summary>
        /// <param name="item">El elemento a eliminar del YTB. <para>The element to remove from the YTB.</para></param>
        /// <remarks>Si el elemento no se encuentra, el YTB permanece sin cambios. <para>If the element is not found, the YTB remains unchanged.</para></remarks>
        /// <returns>Retorna true si el elemento fue encontrado y eliminado; de lo contrario, false. <para>Returns true if the element was found and removed; otherwise, false.</para></returns>
        public bool Remove(T item)
        {
            int index = Array.IndexOf(_ytb, item, 0, Count);
            if (index < 0)
                return false;
            return RemoveAt(index);
        }

        /// <summary>
        /// Obtiene una referencia del objeto en la posición indicada.
        /// <para>Gets a reference to the object at the specified position.</para>
        /// </summary>
        /// <param name="index">Índice del elemento. <para>Element index.</para></param>
        /// <returns>Referencia al objeto. <para>Reference to the object.</para></returns>
        /// <exception cref="ArgumentOutOfRangeException">El índice está fuera del rango válido. <para>The index is outside the valid range.</para></exception>
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
        /// Obtiene una referencia del objeto asociado a la entidad indicada.
        /// <para>Gets a reference to the object associated with the specified entity.</para>
        /// </summary>
        /// <param name="entity">Entidad cuyo id se usa como índice. <para>Entity whose id is used as the index.</para></param>
        /// <returns>Referencia al objeto. <para>Reference to the object.</para></returns>
        /// <exception cref="ArgumentOutOfRangeException">El id de la entidad está fuera del rango válido. <para>The entity id is outside the valid range.</para></exception>
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
        /// Establece un valor en una posición específica usando índice uint.
        /// <para>Sets a value at a specific position using a uint index.</para>
        /// </summary>
        /// <param name="index">Índice en el arreglo. <para>Array index.</para></param>
        /// <param name="value">Valor a asignar. <para>Value to assign.</para></param>
        /// <exception cref="ArgumentOutOfRangeException">El índice está fuera del rango válido. <para>The index is outside the valid range.</para></exception>
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
        /// Devuelve una copia del arreglo.
        /// <para>Returns a copy of the array.</para>
        /// </summary>
        /// <returns>Copia del arreglo. <para>Copy of the array.</para></returns>
        public T[] ToArray()
        {
            T[] result = new T[Count];
            Array.Copy(_ytb, result, Count);
            return result;
        }

        /// <summary>
        /// Método que convierte el contenido del YTB a una lista de tipo List&lt;T&gt;.
        /// <para>Method that converts the YTB contents into a List&lt;T&gt;.</para>
        /// </summary>
        /// <returns>Lista con los elementos. <para>List containing the elements.</para></returns>
        public List<T> ToList()
        {
            List<T> List = new List<T>(Count);
            List.AddRange(_ytb.AsSpan(0, Count));
            return List;
        }

        /// <summary>
        /// Devuelve una vista de solo lectura del arreglo como ReadOnlySpan&lt;T&gt;.
        /// <para>Returns a read-only view of the array as ReadOnlySpan&lt;T&gt;.</para>
        /// </summary>
        /// <returns>Vista de solo lectura del arreglo. <para>Read-only view of the array.</para></returns>
        public ReadOnlySpan<T> AsReadOnlySpan()
        {
            return _ytb.AsSpan(0, Count);
        }

        /// <summary>
        /// Devuelve una vista del arreglo como Span&lt;T&gt;.
        /// <para>Returns a view of the array as Span&lt;T&gt;.</para>
        /// </summary>
        /// <returns>Vista del arreglo. <para>View of the array.</para></returns>
        public Span<T> AsSpan()
        {
            return _ytb.AsSpan(0, Count);
        }

        /// <summary>
        /// Limpia el arreglo, reseteando el conteo y la capacidad.
        /// <para>Clears the array, resetting the count and capacity.</para>
        /// </summary>
        public void Clear()
        {
            Count = 0;
            Capacity = 0;
            Array.Clear(_ytb, 0, Capacity);
        }

        /// <summary>
        /// Implementación de la interfaz IEnumerable&lt;T&gt; para permitir la iteración sobre los elementos del YTB.
        /// <para>Implementation of IEnumerable&lt;T&gt; to allow iteration over YTB elements.</para>
        /// </summary>
        /// <returns>Enumerador de los elementos. <para>Enumerator of the elements.</para></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator()
        {
            foreach(var item in _ytb.Take(Count)) yield return item;
        }

        /// <summary>
        /// La implementación no genérica de IEnumerable.
        /// <para>The non-generic IEnumerable implementation.</para>
        /// </summary>
        /// <returns>Enumerador no genérico. <para>Non-generic enumerator.</para></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
