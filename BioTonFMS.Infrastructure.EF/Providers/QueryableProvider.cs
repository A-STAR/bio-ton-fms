namespace BioTonFMS.Infrastructure.EF.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using BioTonFMS.Infrastructure;
    using BioTonFMS.Infrastructure.Persistence.Providers;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;


    /// <summary>
    /// Реализация для NHibernate провайдера коллекции, способной выполнять выборку сущностей при помощи LINQ-запросов.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    public sealed class QueryableProvider<T> : IQueryableProvider<T> where T : class
    {
        private readonly Factory<DbContext> _dbContextFactory;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="QueryableProvider{T}"/>.
        /// </summary>
        /// <param name="dbContextFactory">Контекст</param>
        /// <exception cref="ArgumentNullException">Переданный аргумент имеет значение <see langword="null"/>.</exception>
        public QueryableProvider(Factory<DbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        /// <inheritdoc />
        public IQueryable<T> Linq()
        {
            var context = _dbContextFactory.Invoke();
            return context.Set<T>();
        }

        /// <inheritdoc />
        public IFetchRequestContext<T, TRelated> Fetch<TRelated>(Expression<Func<T, TRelated>> selector)
        {
            return new FetchRequestContext<T, TRelated>(Linq().Include(selector));
        }

        private class FetchRequestContext<TQueried, TFetch> : IFetchRequestContext<TQueried, TFetch> where TQueried : class
        {
            private readonly IIncludableQueryable<TQueried, TFetch> _query;

            public FetchRequestContext(IIncludableQueryable<TQueried, TFetch> query)
            {
                _query = query;
            }

            public IQueryable<TQueried> Linq()
            {
                return _query;
            }

            public IFetchRequestContext<TQueried, TRelated> Fetch<TRelated>(
                Expression<Func<TQueried, TRelated>> selector)
            {
                return new FetchRequestContext<TQueried, TRelated>(_query.Include(selector));
            }

            public IFetchRequestContext<TQueried, TRelated> ThenFetch<TRelated>(
                Expression<Func<TFetch, TRelated>> selector)
            {
                return new FetchRequestContext<TQueried, TRelated>(_query.ThenInclude(selector));
            }
        }
    }
}