using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BioTonFMS.Infrastructure.EF.Providers
{
    /// <summary>
    /// Реализация для NHibernate провайдера коллекции, способной выполнять выборку сущностей по их идентификаторам.
    /// </summary>
    /// <typeparam name="T">
    /// Выбираемая сущность.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// Ключ выборки, может быть <see cref="int"/> или пользовательский ссылаемый тип.
    /// </typeparam>
    public sealed class KeyValueProvider<T, TDbContext, TKey> : IKeyValueProvider<T, TKey>
        where T : class
    {
        private readonly Factory<TDbContext> _dbContextFactory;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="KeyValueProvider{T, TDbContext, TKey}"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Переданный аргумент имеет значение <see langword="null"/>.</exception>
        public KeyValueProvider(Factory<TDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        private DbContext DbContext { 
            get
            {
                DbContext? dbContext = _dbContextFactory.Invoke() as DbContext;
                if (dbContext == null)
                {
                    throw new InvalidOperationException($"DbContext типа {typeof(TDbContext)} не зарегистрирован");
                }
                return dbContext;
            }
        }

        /// <inheritdoc />
        public T? this[TKey key] => Get(key);

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Аргумент <c><paramref name="load"/></c> имеет значние вне допустимого диапазона.</exception>
        public T? Get(TKey key)
        {
            return DbContext.Set<T>().Find(key);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Переданный аргумент имеет значение <see langword="null"/>.</exception>
        public void Remove(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            DbContext.Set<T>().Remove(entity);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Переданный аргумент имеет значение <see langword="null"/>.</exception>
        public void Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            DbContext.Set<T>().Add(entity);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Переданный аргумент имеет значение <see langword="null"/>.</exception>
        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            DbContext.Set<T>().Update(entity);
        }
    }
}