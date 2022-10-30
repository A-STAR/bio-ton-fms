namespace BioTonFMS.Infrastructure.EF.Providers
{
    using System;
    using System.Runtime.InteropServices;

    using BioTonFMS.Infrastructure.Persistence.Providers;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.Logging;
    using ICommonLog = global::Microsoft.Extensions.Logging.ILogger;

    /// <summary>
    /// Всегда передается фабрикой. Единица работы. Удобная абстракция для работы с транзакциями БД без раздумий о том,
    /// открыта ли она была или еще нет.
    /// </summary>
    
    public sealed class UnitOfWork : IUnitOfWork
    {
        private static int curId = 0;
        private readonly Factory<DbContext> _dbContextFactory;
        private readonly DbContext _dbContext;

        private IDbContextTransaction? _transaction;
        private bool _disposed;
        private int id;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="UnitOfWork"/>.
        /// </summary>
        /// <param name="session">Сессия NHibernate.</param>
        /// <exception cref="ArgumentNullException">Переданный аргумент имеет значение <see langword="null"/>.</exception>
        public UnitOfWork(Factory<DbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _dbContext = _dbContextFactory.Invoke();

            if (_dbContext.Database.CurrentTransaction == null)
            {
                _transaction = _dbContext.Database.BeginTransaction();
            }
            else
            {
                _transaction = null;
            }
            _disposed = false;
            id = ++curId;
        }

        /// <inheritdoc />
        public void Rollback()
        {
            if (_transaction == null)
            {
                return;
            }

            _transaction.Rollback();
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_transaction == null)
                {
#pragma warning disable CS0618 // Тип или член устарел
                    if (Marshal.GetExceptionCode() == 0)
#pragma warning restore CS0618 // Тип или член устарел
                    {
                        _dbContext.SaveChanges();
                        _disposed = true;
                    }
                    return;
                }
#pragma warning disable CS0618 // Тип или член устарел
                if (Marshal.GetExceptionCode() == 0)
#pragma warning restore CS0618 // Тип или член устарел
                {
                    try
                    {
                        _dbContext.SaveChanges();
                        _transaction.Commit();
                    }
                    catch
                    {
                        _transaction.Rollback();
                        _disposed = true;
                        throw;
                    }
                }
                else
                {
                    _transaction.Rollback();
                }
                _transaction.Dispose();
                _transaction = null;
            }
            _disposed = true;
        }
    }
}