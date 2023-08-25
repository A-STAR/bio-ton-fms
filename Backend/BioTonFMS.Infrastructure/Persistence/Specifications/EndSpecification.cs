namespace BioTonFMS.Infrastructure.Persistence.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using BioTonFMS.Infrastructure.Segments;

    /// <inheritdoc />
    internal sealed class EndSpecification<T> : IEndSpecification<T>
    {
        private readonly IQueryable<T> _queryableProvider;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="EndSpecification{T}"/>.
        /// </summary>
        /// <param name="queryableProvider">Провайдер коллекции, способной выполнять выборку сущностей при помощи LINQ-запросов.</param>
        /// <exception cref="ArgumentNullException">Переданный аргумент имеет значение <see langword="null"/>.</exception>
        public EndSpecification(IQueryable<T> queryableProvider)
        {
            _queryableProvider = queryableProvider ?? throw new ArgumentNullException(nameof(queryableProvider));
        }

        /// <inheritdoc />
        public IEnumerable<T> All()
        {
            return _queryableProvider.ToArray();
        }

        /// <inheritdoc />
        public IEnumerableSegment<T> Segment(int skip, int take)
        {
            return _queryableProvider.AsSegment(skip, take);
        }

        /// <inheritdoc />
        public T? One()
        {
            return _queryableProvider.FirstOrDefault();
        }

        /// <inheritdoc />
        public T? Single()
        {
            return _queryableProvider.SingleOrDefault();
        }

        /// <inheritdoc />
        public IEndSpecification<T> OrderBy(Expression<Func<T, string>> expression)
        {
            return new EndSpecification<T>(_queryableProvider.OrderBy(expression));
        }

        /// <inheritdoc />
        public IEndSpecification<T> OrderByDescending(Expression<Func<T, string>> expression)
        {
            return new EndSpecification<T>(_queryableProvider.OrderByDescending(expression));
        }
    }
}