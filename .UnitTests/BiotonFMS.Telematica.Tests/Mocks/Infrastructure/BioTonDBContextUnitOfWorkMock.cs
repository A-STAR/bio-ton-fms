using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure.Persistence.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiotonFMS.Telematica.Tests.Mocks.Infrastructure
{
    public class BioTonDBContextUnitOfWorkMock : IUnitOfWork<BioTonDBContext>
    {
        /// <summary>
        /// Commit транзакции.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Откат транзакции.
        /// </summary>
        public void Rollback()
        {
        }
    }
}
