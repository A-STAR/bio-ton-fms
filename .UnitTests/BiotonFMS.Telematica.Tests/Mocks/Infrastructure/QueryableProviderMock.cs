using BioTonFMS.Infrastructure.Persistence.Providers;
using System.Linq.Expressions;

namespace BiotonFMS.Telematica.Tests.Mocks.Infrastructure
{
    /// <summary>
    /// Мок для провайдера LINQ на основе обыкновенной коллекции.
    /// </summary>
    public class QueryableProviderMock<TEntity> : IQueryableProvider<TEntity>
    {
        private readonly ICollection<TEntity> _collection;

        public QueryableProviderMock(ICollection<TEntity> collection)
        {
            _collection = collection ?? throw new ArgumentNullException("collection");
        }

        public IQueryable<TEntity> Linq()
        {
            return _collection.AsQueryable().OfType<TEntity>();
        }

        public IFetchRequestContext<TEntity, TRelated> Fetch<TRelated>(Expression<Func<TEntity, TRelated>> selector)
        {
            return new FetchRequestContextMock<TEntity, TRelated>(Linq());
        }
    }

}
