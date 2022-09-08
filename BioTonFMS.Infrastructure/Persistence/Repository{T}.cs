namespace BioTonFMS.Infrastructure.Persistence
{
    using BioTonFMS.Infrastructure;
    using BioTonFMS.Infrastructure.Models;
    using BioTonFMS.Infrastructure.Persistence.Providers;

    /// <inheritdoc cref="Repository{T,TKey}"/>
    public class Repository<T> : Repository<T, int>, IRepository<T>
        where T : class, IAggregateRoot, IEntity
    {
        /// <inheritdoc />
        public Repository(
            IKeyValueProvider<T, int> keyValueProvider,
            IQueryableProvider<T> queryableProvider,
            UnitOfWorkFactory unitOfWorkFactory)
            : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
        {
        }
    }
}