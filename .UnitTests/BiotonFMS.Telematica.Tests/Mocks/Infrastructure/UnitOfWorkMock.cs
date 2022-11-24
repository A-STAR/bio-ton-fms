using BioTonFMS.Infrastructure.Persistence.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiotonFMS.Telematica.Tests.Mocks.Infrastructure
{
    public class UnitOfWorkMock : IUnitOfWork
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
