using BioTonFMS.Infrastructure;
using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiotonFMS.Telematica.Tests.Mocks.Infrastructure
{
    public class BioTonDBContextUnitOfWorkFactoryMock : UnitOfWorkFactory<BioTonDBContext>
    {
        public BioTonDBContextUnitOfWorkFactoryMock() : base(null!)
        {
        }

        public BioTonDBContextUnitOfWorkFactoryMock(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override IUnitOfWork<BioTonDBContext> GetUnitOfWork()
        {
            return new BioTonDBContextUnitOfWorkMock();
        }

    }
}
