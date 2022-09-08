namespace BioTonFMS.Infrastructure.Segments
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <inheritdoc />
    internal class EnumerableSegment<T> : IEnumerableSegment<T>
    {
        private readonly T[] _list;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="EnumerableSegment{T}"/>.
        /// </summary>
        /// <param name="list">Список элементов сегмента.</param>
        /// <param name="fullCount">Полное количество сущностей. </param>
        public EnumerableSegment(T[] list, int fullCount)
        {
            _list = list;

            FullCount = fullCount;
        }

        /// <inheritdoc />
        public int FullCount { get; private set; }

        /// <inheritdoc />
        public int SegmentCount
        {
            get
            {
                return _list.Length;
            }
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return _list.AsEnumerable().GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}