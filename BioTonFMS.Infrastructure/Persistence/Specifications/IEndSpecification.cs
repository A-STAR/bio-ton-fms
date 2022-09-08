namespace BioTonFMS.Infrastructure.Persistence.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using BioTonFMS.Infrastructure.Segments;

    /// <summary>
    /// Завершение запроса по спецификации, можно выбрать количество возвращаемых сущностей или указать только одну.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    public interface IEndSpecification<T>
    {
        /// <summary>
        /// Возвращает все элементы.
        /// </summary>
        /// <returns>Набор всех сущности.</returns>
        IEnumerable<T> All();

        /// <summary>
        /// Возвращает набор элементов.
        /// </summary>
        /// <param name="skip">Сколько пропустить.</param>
        /// <param name="take">Сколько запросить после пропуска.</param>
        /// <returns>Набор сущностей в заданном диапазоне.</returns>
        IEnumerableSegment<T> Segment(int skip, int take);

        /// <summary>
        /// Возвращает первый элемент. Вернет <see langword="null"/>, если найти не удалось.
        /// </summary>
        /// <returns>Первая сущность или null.</returns>
        T One();

        /// <summary>
        /// Возвращает первый элемент. Вызовет исключение, если набор содержит более одной сущности.
        /// </summary>
        /// <returns>Единственная сущность.</returns>
        T Single();

        /// <summary>
        /// Сортирует элементы по возрастанию.
        /// </summary>
        /// <param name="expression">Выражение для сортировки.</param>
        /// <returns>Экземпляр класса <see cref="IEndSpecification{T}"/>.</returns>
        IEndSpecification<T> OrderBy(Expression<Func<T, string>> expression);

        /// <summary>
        /// Сортирует элементы по убыванию.
        /// </summary>
        /// <param name="expression">Выражение для сортировки.</param>
        /// <returns>Экземпляр класса <see cref="IEndSpecification{T}"/>.</returns>
        IEndSpecification<T> OrderByDescending(Expression<Func<T, string>> expression);
    }
}