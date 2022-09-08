namespace BioTonFMS.Infrastructure.Persistence.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Поддержка Eager Loading для LINQ-запроса.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    public interface IFetchRequest<T>
    {
        /// <summary>
        /// Возвращает объект, поддерживающий запросы.
        /// </summary>
        /// <returns>
        /// Объект, поддерживающий запросы.
        /// </returns>
        IQueryable<T> Linq();

        /// <summary>
        /// Пометить ссылочное навигационное свойство для Eager Loading и вернуть его контекст.
        /// </summary>
        /// <typeparam name="TRelated">Тип сущности навигационного свойства.</typeparam>
        /// <param name="selector">Выражение получения навигационного свойства.</param>
        /// <returns>Контекст настройки Eager Loading для навигационного свойства.</returns>
        IFetchRequestContext<T, TRelated> Fetch<TRelated>(Expression<Func<T, TRelated>> selector);
    }
}