using System.Collections.Generic;
using System.Threading;
using System.Transactions;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using Texo.Domain.Model.Service;
using Texo.Infrastructure.Db.Internal;

namespace Texo.Infrastructure.Db.Service
{
    public class DbTransactionService : ITransactionService
    {
        private const string EfCoreContextKey = "EF_CORE_CONTEXT";
        
        private readonly DbContext _dbContext;
        private readonly ILogger _logger;
        private readonly ThreadLocal<uint> _reentrantCount = new(() => 0);
        private readonly ThreadLocal<IDbContextTransaction> _currentTransaction = new ThreadLocal<IDbContextTransaction>();
        
        public DbTransactionService(DbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public int Priority => 1;
        public void OnLoad(Dictionary<string, object> context)
        {
            context.Add(EfCoreContextKey, _dbContext);
        }

        public void Begin(Dictionary<string, object> context)
        {
            if (_reentrantCount.Value == 0)
            {
                _currentTransaction.Value = _dbContext.Database.BeginTransaction();
            }
            _reentrantCount.Value++;
        }

        public void Commit(Dictionary<string, object> context)
        {
            if (IsNullOrDisposed(_currentTransaction))
            {
                _reentrantCount.Value = 0;
                throw new TransactionException("No active transaction found.");
            }

            if (_reentrantCount.Value == 0)
            {
                throw new TransactionException("Bad reentrant count. Check if the transaction has currently begun.");
            }
            
            _dbContext.SaveChanges();
            
            if (_reentrantCount.Value == 1)
            {
                _currentTransaction.Value.Commit();
                _reentrantCount.Value = 0;
            }
            else
            {
                // Reentrant case.
                _reentrantCount.Value--;
            }
        }

        public void Rollback(Dictionary<string, object> context)
        {
            if (!IsNullOrDisposed(_reentrantCount) && _reentrantCount.Value >= 1 && !IsNullOrDisposed(_currentTransaction))
            {
                try
                {
                    _currentTransaction.Value.Rollback();
                }
                finally
                {
                    _reentrantCount.Value = 0;
                    _currentTransaction.Dispose();
                }
            }
        }

        private bool IsNullOrDisposed<A>(ThreadLocal<A> instance)
        {
            if (instance is null)
            {
                return true;
            }

            try
            {
                return !instance.IsValueCreated;
            }
            catch
            {
                return true;
            }
        }
        
        public void OnDispose(Dictionary<string, object> context)
        {
            context.Remove(EfCoreContextKey);
            if (!IsNullOrDisposed(_currentTransaction))
            {
                try
                {
                    _currentTransaction.Value.Rollback();                    
                }
                catch
                {
                    _logger.Debug("Transaction rollback failed on dispose");
                }

                _currentTransaction.Dispose();
            }

            if (_reentrantCount is not null && _reentrantCount.IsValueCreated)
            {
                _reentrantCount.Dispose();
            }
        }
    }
}