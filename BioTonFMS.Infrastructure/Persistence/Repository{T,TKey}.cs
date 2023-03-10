namespace BioTonFMS.Infrastructure.Persistence
{
    using System;

    using BioTonFMS.Infrastructure;
    using BioTonFMS.Infrastructure.Models;
    using BioTonFMS.Infrastructure.Persistence.Providers;
    using BioTonFMS.Infrastructure.Persistence.Specifications;

    using LinqSpecs;

    /// <summary>
    /// Основная реализация репозитория.
    /// </summary>
    /// <inheritdoc />
    public class Repository<T, TDbContext, TKey> : IRepository<T, TKey>
        where T : class, IAggregateRoot, IEntity<TKey>
    {
        private readonly IKeyValueProvider<T, TKey> _keyValueProvider;

        private readonly IQueryableProvider<T> _queryableProvider;

        private readonly UnitOfWorkFactory<TDbContext> _unitOfWorkFactory;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Repository{T,TKey}"/>.
        /// </summary>
        /// <param name="keyValueProvider">Провайдер коллекции, способной выполнять выборку сущностей по их идентификаторам.</param>
        /// <param name="queryableProvider">Провайдер коллекции, способной выполнять выборку сущностей при помощи LINQ-запросов.</param>
        /// <param name="unitOfWorkFactory">Единица работы по Мартину <c>Фаулеру</c>.</param>
        /// <exception cref="ArgumentNullException">Переданный аргумент имеет значение <see langword="null"/>.</exception>
        public Repository(
            IKeyValueProvider<T, TKey> keyValueProvider,
            IQueryableProvider<T> queryableProvider,
            UnitOfWorkFactory<TDbContext> unitOfWorkFactory)
        {
            _keyValueProvider = keyValueProvider ?? throw new ArgumentNullException(nameof(keyValueProvider));
            _queryableProvider = queryableProvider ?? throw new ArgumentNullException(nameof(queryableProvider));
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        }

        protected IQueryableProvider<T> QueryableProvider => _queryableProvider;

        /// <inheritdoc />
        public T? this[TKey key]
        {
            get
            {
                return _keyValueProvider[key];
            }
        }

        /// <inheritdoc />
        public virtual void Remove(T entity)
        {
            using (_unitOfWorkFactory.GetUnitOfWork())
            {
                _keyValueProvider.Remove(entity);
            }
        }

        /// <inheritdoc />
        public virtual void Add(T entity)
        {
            using (_unitOfWorkFactory.GetUnitOfWork())
            {
                _keyValueProvider.Add(entity);
            }
        }

        /// <inheritdoc />
        public virtual void Update(T entity)
        {
            using (_unitOfWorkFactory.GetUnitOfWork())
            {
                _keyValueProvider.Update(entity);
            }
        }

        /// <inheritdoc />
        public IBeginSpecification<T> BeginSpecification(Specification<T> specification)
        {
            return new BeginSpecification<T>(_queryableProvider, specification);
        }
    }
}