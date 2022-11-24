using BioTonFMS.Infrastructure;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiotonFMS.Telematica.Tests.Mocks.Infrastructure
{
    public class UnitOfWorkFactoryMock : UnitOfWorkFactory
    {
        public UnitOfWorkFactoryMock() : base(null!)
        {
        }

        public UnitOfWorkFactoryMock(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override IUnitOfWork GetUnitOfWork()
        {
            return new UnitOfWorkMock();
        }

    }
}
