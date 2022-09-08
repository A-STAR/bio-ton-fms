namespace BioTonFMS.Infrastructure.Persistence.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Контекст настройки Eager Loading для LINQ-запроса.
    /// </summary>
    /// <typeparam name="TQueried">Тип запрашиваемой сущности.</typeparam>
    /// <typeparam name="TFetch">Тип сущности навигационного свойства.</typeparam>
    public interface IFetchRequestContext<TQueried, TFetch> : IFetchRequest<TQueried>
    {
        /// <summary>
        /// Пометить ссылочное навигационное свойство для Eager Loading и вернуть новый контекст.
        /// </summary>
        /// <typeparam name="TRelated">Тип сущности навигационного свойства.</typeparam>
        /// <param name="selector">Выражение получения навигационного свойства.</param>
        /// <returns>Новый контекст.</returns>
        IFetchRequestContext<TQueried, TRelated> ThenFetch<TRelated>(
            Expression<Func<TFetch, TRelated>> selector);
        /*
        /// <summary>
        /// Пометить перечислимое навигационное свойство для Eager Loading и вернуть новый контекст.
        /// </summary>
        /// <typeparam name="TRelated">Тип сущности навигационного свойства.</typeparam>
        /// <param name="selector">Выражение получения навигационного свойства.</param>
        /// <returns>Новый контекст.</returns>
        IFetchRequestContext<TQueried, TRelated> ThenFetchMany<TRelated>(
            Expression<Func<TFetch, IEnumerable<TRelated>>> selector);*/
    }
}