namespace BioTonFMS.Infrastructure.Persistence
{
    using System.Collections.Generic;

    using BioTonFMS.Infrastructure.Models;
    using BioTonFMS.Infrastructure.Persistence.Specifications;

    using LinqSpecs;

    /// <summary>
    /// Предоставляет доступ к данным в виде коллекции с запросами, представленными <see cref="Specification{T}"/> .
    /// </summary>
    /// <remarks>
    /// DDD is slightly different, after all, remember “There is no database”.  The primary thing that differentiates a
    /// IRepository from a traditional data access layer is that it is to all intents and purposes a Collection semantic
    /// – just like <see cref="IList{T}"/> in .Net.  IRepository provides us with a mechanism to achieve Persistence
    /// Ignorance in our Domain.
    /// </remarks>
    /// <typeparam name="T">
    /// Искомая сущность.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// Однозначно определяющий ключ.
    /// </typeparam>
    public interface IRepository<T, in TKey>
        where T : IEntity<TKey>, IAggregateRoot
    {
        /// <summary>
        /// Возвращает сущность по ключу.
        /// </summary>
        /// <param name="key">Ключ сущности.</param>
        /// <returns>Запрашиваемая сущность.</returns>
        T this[TKey key] { get; }

        /// <summary>
        /// Удаляет сущность из репозитория.
        /// </summary>
        /// <param name="entity">Удаляемая сущность.</param>
        void Remove(T entity);

        /// <summary>
        /// Добавляет сущность в репозиторий.
        /// </summary>
        /// <param name="entity">Сохраняемая сущность.</param>
        void Put(T entity);

        /// <summary>
        /// Обновляет сущность в репозитории.
        /// </summary>
        /// <param name="entity">Обновляемая сущность.</param>
        void Update(T entity);

        /// <summary>
        /// Начинает синтаксис запроса по спецификации.
        /// </summary>
        /// <param name="specification">Спецификация запроса.</param>
        /// <returns>Запрос по спецификации.</returns>
        IBeginSpecification<T> BeginSpecification(Specification<T> specification);
    }
}