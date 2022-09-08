namespace BioTonFMS.Infrastructure.EF.Providers
{
    using System;


    using BioTonFMS.Infrastructure;
    using BioTonFMS.Infrastructure.Persistence;
    using BioTonFMS.Infrastructure.Persistence.Providers;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Реализация для NHibernate провайдера коллекции, способной выполнять выборку сущностей по их идентификаторам.
    /// </summary>
    /// <typeparam name="T">
    /// Выбираемая сущность.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// Ключ выборки, может быть <see cref="int"/> или пользовательский ссылаемый тип.
    /// </typeparam>
    public sealed class KeyValueProvider<T, TKey> : IKeyValueProvider<T, TKey>
        where T : class
    {
        private readonly Factory<DbContext> _dbContextFactory;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="KeyValueProvider{T, TKey}"/>.
        /// </summary>
        /// <param name="dbContextFactory">Сессия NHibernate.</param>
        /// <exception cref="ArgumentNullException">Переданный аргумент имеет значение <see langword="null"/>.</exception>
        public KeyValueProvider(Factory<DbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        private DbContext DbContext => _dbContextFactory.Invoke();

        /// <inheritdoc />
        public T this[TKey key] => Get(key);

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Аргумент <c><paramref name="load"/></c> имеет значние вне допустимого диапазона.</exception>
        public T Get(TKey key)
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
        public void Put(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            DbContext.Set<T>().Add(entity);
            //DbContext.SaveChanges();
        }
    }
}