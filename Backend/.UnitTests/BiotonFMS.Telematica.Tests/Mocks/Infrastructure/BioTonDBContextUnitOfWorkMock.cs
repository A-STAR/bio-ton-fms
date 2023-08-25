using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure.Persistence.Providers;

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
