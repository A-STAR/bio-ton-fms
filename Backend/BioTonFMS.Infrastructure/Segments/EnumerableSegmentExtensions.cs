namespace BioTonFMS.Infrastructure.Segments
{
    using System;
    using System.Linq;

    /// <summary>
    /// Расширения для диапазонов.
    /// </summary>
    public static class EnumerableSegmentExtensions
    {
        /// <summary>
        /// Выделяет сегмент сущностей из общего перечисления.
        /// </summary>
        /// <typeparam name="T">Сущность, перечисление которой должно быть нарезанным на сегменты.</typeparam>
        /// <param name="queryable">Опрашиваемое перечисление.</param>
        /// <param name="skip">Сколько элементов пропустить от начала исходного перечисления.</param>
        /// <param name="take">Сколько элементов после пропуска взять из исходного перечисления.</param>
        /// <returns>Сегмент перечисления.</returns>
        /// <exception cref="ArgumentNullException">Переданный аргумент имеет значение <see langword="null"/>.</exception>
        public static IEnumerableSegment<T> AsSegment<T>(this IQueryable<T> queryable, int skip, int take)
        {
            if (queryable == null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }

            return new EnumerableSegment<T>(queryable.Skip(skip).Take(take).ToArray(), queryable.Count());
        }
    }
}