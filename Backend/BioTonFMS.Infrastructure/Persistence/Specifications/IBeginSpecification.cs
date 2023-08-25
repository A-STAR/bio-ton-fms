namespace BioTonFMS.Infrastructure.Persistence.Specifications
{
    using System;
    using System.Linq.Expressions;

    using BioTonFMS.Infrastructure.Persistence.Providers;

    using LinqSpecs;

    /// <summary>
    /// Интерфейс запроса по спецификации <see cref="Specification{T}"/>.
    /// </summary>
    /// <typeparam name="TEntity">
    /// Корневая сущность спецификации.
    /// </typeparam>
    public interface IBeginSpecification<TEntity>
    {
        /// <summary>
        /// Выполняет проекцию сущности в другую сущность на уровне выборки.
        /// Из источника данных будут получены только те данные, которые необходимы проецируемой сущности.
        /// Вся корневая сущность не должна подгружаться, однако это зависит от реализации провайдера <see cref="IQueryableProvider{T}"/>.
        /// </summary>
        /// <param name="selector">Выражение проекции.</param>
        /// <typeparam name="TProjectionResult">Проецируемая сущность выборки.</typeparam>
        /// <returns>Завершения запроса по спецификации.</returns>
        IEndSpecification<TProjectionResult> Projection<TProjectionResult>(
            Expression<Func<TEntity, TProjectionResult>> selector);

        /// <summary>
        /// Возвращает сущности без проецирования. Загрузить данные корневых сущностей целиком.
        /// </summary>
        /// <returns>Завершения запроса по спецификации.</returns>
        IEndSpecification<TEntity> Entity();
    }
}