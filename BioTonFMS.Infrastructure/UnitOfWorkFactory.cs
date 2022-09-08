
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace BioTonFMS.Infrastructure
{
    public class UnitOfWorkFactory
    {
        private IServiceProvider _serviceProvider;

        public UnitOfWorkFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IUnitOfWork GetUnitOfWork()
        {
            return _serviceProvider.GetRequiredService<IUnitOfWork>();
        }
    }
}
