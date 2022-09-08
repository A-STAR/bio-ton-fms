namespace BioTonFMS.Infrastructure.Segments
{
    using System.Collections.Generic;

    /// <summary>
    /// Сегмент перечисления, используется при нарезке исходного перечисления на части. Пример: постраничная загрузка.
    /// </summary>
    /// <typeparam name="T">
    /// Сущность, перечисление которой может быть нарезанным на сегменты.
    /// </typeparam>
    public interface IEnumerableSegment<out T> : IEnumerable<T>
    {
        /// <summary>
        /// Возвращает число элементов в исходном перечислении.
        /// </summary>
        int FullCount { get; }

        /// <summary>
        /// Возвращает число элементов в сегменте.
        /// </summary>
        int SegmentCount { get; }
    }
}