namespace BioTonFMS.Infrastructure.Persistence.Providers
{
    using System;

    /// <summary>
    /// Единица работы по Мартину <c>Фаулеру</c>.  Автоматически выполняет Commit на Dispose, однако в случае
    /// исключения, Commit не будет выполнен.
    /// </summary>
    /// TODO: Доработать UnitOfWork и все места его использования
    public interface IUnitOfWork<TDbContext> : IDisposable
    {
        /// <summary>
        /// Отменяет набор операций.
        /// </summary>
        void Rollback();
    }
}