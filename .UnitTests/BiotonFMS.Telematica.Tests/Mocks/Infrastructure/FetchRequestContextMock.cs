using BioTonFMS.Infrastructure.Persistence.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BiotonFMS.Telematica.Tests.Mocks.Infrastructure
{
    public class FetchRequestContextMock<TQueried, TFetched> : IFetchRequestContext<TQueried, TFetched>
    {
        private readonly IQueryable<TQueried> _query;

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Переданный аргумент имеет значение <see langword="null"/>.</exception>
        public FetchRequestContextMock(IQueryable<TQueried> query)
        {
            _query = query ?? throw new ArgumentNullException(nameof(query));
        }

        /// <inheritdoc />
        public IQueryable<TQueried> Linq()
        {
            return _query;
        }

        /// <inheritdoc />
        public IFetchRequestContext<TQueried, TRelated> Fetch<TRelated>(
            Expression<Func<TQueried, TRelated>> selector)
        {
            return new FetchRequestContextMock<TQueried, TRelated>(_query);
        }

        /// <inheritdoc />
        public IFetchRequestContext<TQueried, TRelated> ThenFetch<TRelated>(
            Expression<Func<TFetched, TRelated>> selector)
        {
            return new FetchRequestContextMock<TQueried, TRelated>(_query);
        }
    }
}
