using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace BioTonFMS.Infrastructure
{
    public class UnitOfWorkFactory<TDbContext>
    {
        private IServiceProvider _serviceProvider;

        public UnitOfWorkFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public virtual IUnitOfWork<TDbContext> GetUnitOfWork()
        {
            return _serviceProvider.GetRequiredService<IUnitOfWork<TDbContext>>();
        }
    }
}
