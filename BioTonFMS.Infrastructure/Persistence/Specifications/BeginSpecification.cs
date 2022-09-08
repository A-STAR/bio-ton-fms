namespace BioTonFMS.Infrastructure.Persistence.Specifications
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using BioTonFMS.Infrastructure.Persistence.Providers;

    using LinqSpecs;

    /// <inheritdoc />
    internal sealed class BeginSpecification<T> : IBeginSpecification<T>
    {
        private readonly IQueryableProvider<T> _queryableProvider;

        private readonly Specification<T> _specification;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="BeginSpecification{T}"/>.
        /// </summary>
        /// <param name="queryableProvider">Провайдер коллекции, способной выполнять выборку сущностей при помощи LINQ-запросов.</param>
        /// <param name="specification">Спецификация запроса.</param>
        /// <exception cref="ArgumentNullException">Переданный аргумент имеет значение <see langword="null"/>.</exception>
        public BeginSpecification(
            IQueryableProvider<T> queryableProvider, Specification<T> specification)
        {
            _queryableProvider = queryableProvider ?? throw new ArgumentNullException(nameof(queryableProvider));
            _specification = specification ?? throw new ArgumentNullException(nameof(specification));
        }

        /// <inheritdoc />
        public IEndSpecification<TProjectionResult> Projection<TProjectionResult>(
            Expression<Func<T, TProjectionResult>> selector)
        {
            return new EndSpecification<TProjectionResult>(
                _queryableProvider.SpecifiedBy(_specification).Select(selector));
        }

        /// <inheritdoc />
        public IEndSpecification<T> Entity()
        {
            return new EndSpecification<T>(_queryableProvider.SpecifiedBy(_specification));
        }
    }
}