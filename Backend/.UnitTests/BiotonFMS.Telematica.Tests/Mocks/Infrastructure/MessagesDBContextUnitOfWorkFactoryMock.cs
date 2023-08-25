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
    public class MessagesDBContextUnitOfWorkFactoryMock : UnitOfWorkFactory<MessagesDBContext>
    {
        public MessagesDBContextUnitOfWorkFactoryMock() : base(null!)
        {
        }

        public MessagesDBContextUnitOfWorkFactoryMock(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override IUnitOfWork<MessagesDBContext> GetUnitOfWork()
        {
            return new MessagesDBContextUnitOfWorkMock();
        }

    }
}
