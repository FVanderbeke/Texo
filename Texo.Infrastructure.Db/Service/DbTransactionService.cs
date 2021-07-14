using System;
using System.Collections.Generic;
using System.Threading;
using System.Transactions;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using Texo.Domain.Model.Service;

namespace Texo.Infrastructure.Db.Service
{
    public class DbTransactionService : ITransactionService
    {
        private readonly DbContextFactory _contextFactory;
        private readonly ILogger _logger;
        private readonly ThreadLocal<DbContext> _currentContext = new ThreadLocal<DbContext>();

        private readonly ThreadLocal<IDbContextTransaction> _currentTransaction =
            new ThreadLocal<IDbContextTransaction>();
        private readonly ThreadLocal<int> _reentrantCounter = new ThreadLocal<int>();
        
        public DbTransactionService(DbContextFactory contextFactory, ILogger logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public int Priority => 1;
        public void OnLoad(Dictionary<string, object> context)
        {
        }

        public void Begin(Dictionary<string, object> context)
        {

            _logger.Debug("[Begin start] Trying to begin a new transaction...");

            if (_currentContext.Value.IsNull())
            {
                _reentrantCounter.Value = 0;
                _currentContext.Value = _contextFactory.Create();
                _currentTransaction.Value = _currentContext.Value.Database.BeginTransaction();
            }
            else
            {
                _reentrantCounter.Value++;
            }
            

            _logger.Debug("[Begin end]");
        }

        public void Commit(Dictionary<string, object> context)
        {
            _logger.Debug("[Commit start]");
            
            if (_currentContext.Value.IsNull())
            {
                throw new TransactionException("Can't save changes. No active context found.");
            }
            
            if (_currentTransaction.Value.IsNull())
            {
                throw new TransactionException("Can't perform any commit. No active transaction found.");
            }

            if (_reentrantCounter.Value == 0)
            {
                _currentContext.Value.SaveChanges();
                _currentTransaction.Value.Commit();
                DisposeContent();
            }
            else
            {
                _reentrantCounter.Value--;
            }
        }

        private void DisposeContent()
        {
            try
            {
                if (_currentTransaction.Value is not null)
                {
                    _currentTransaction.Value.Dispose();
                }
            }
            catch (Exception e)
            {
                _logger.Debug(e,"Error occurred when trying to dispose current transaction");
            }

            try
            {
                if (_currentContext.Value is not null)
                {
                    _currentContext.Value.Dispose();                    
                }
            }
            catch (Exception e)
            {
                _logger.Debug(e,"Error occurred when trying to dispose current EF context");
            }

            _currentTransaction.Value = null;
            _currentContext.Value = null;
            _reentrantCounter.Value = 0;
        }
        
        public void Rollback(Dictionary<string, object> context)
        {
            try
            {
                if (_currentTransaction.Value.IsNull())
                {
                    throw new TransactionException("Can't rollback changes. No active transaction found.");
                }

                if (_currentContext.Value.IsNull())
                {
                    throw new TransactionException("Can't rollback changes. No active transaction found.");
                }
                _currentTransaction.Value.Rollback();
            }
            finally
            {
                DisposeContent();
            }
        }
        
        public void OnDispose(Dictionary<string, object> context)
        {
            DisposeContent();
        }

        public DbContext CurrentDbContext()
        {
            return _currentContext.Value;
        }
    }
}