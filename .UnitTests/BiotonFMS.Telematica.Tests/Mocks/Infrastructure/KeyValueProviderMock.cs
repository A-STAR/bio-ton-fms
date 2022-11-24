using BioTonFMS.Infrastructure.Models;
using BioTonFMS.Infrastructure.Persistence.Providers;

namespace BiotonFMS.Telematica.Tests.Mocks.Infrastructure
{
    public class KeyValueProviderMock<TEntity, TKey> : IKeyValueProvider<TEntity, TKey>
        where TEntity : class, IEntity<TKey> 
    {
        private readonly ICollection<TEntity> _collection;

        public KeyValueProviderMock(ICollection<TEntity> collection)
        {
            _collection = collection ?? throw new ArgumentNullException("collection");
        }

        /// <inheritdoc />
        public TEntity? this[TKey key]
        {
            get
            {
                return _collection.OfType<TEntity>().FirstOrDefault(x => x.Id!.Equals(key));
            }
        }

        public void Remove(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            _collection.Remove(entity);
        }

        public void Add(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (!_collection.Contains(entity))
            {
                _collection.Add(entity);
            }
        }

        public void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
        }

        /// <inheritdoc />
        public TEntity? Get(TKey key)
        {
            return this[key];
        }
    }
}
