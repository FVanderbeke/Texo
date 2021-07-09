#nullable enable
using System;

namespace Texo.Domain.Model.Service
{
    public class TransactionManager
    {
        private readonly GlobalTransactionService _tx;

        public TransactionManager(GlobalTransactionService tx)
        {
            _tx = tx;
        }

        public R Submit<R>(Func<R> function)
        {
            try
            {
                _tx.Begin();
                var result = function.Invoke();
                _tx.Commit();
                return result;
            }
            catch (Exception)
            {
                _tx.Rollback();
                throw;
            }
        }
        

        public void Execute(Action action, Action<Exception>? onError = null)
        {
            try
            {
                _tx.Begin();
                action.Invoke();
                _tx.Commit();
            }
            catch (Exception e)
            {
                if (onError is null)
                {
                    _tx.Rollback();
                    throw;
                }
                else
                {
                    FallBack(onError, e);
                }
            }
        }

        
        private void FallBack(Action<Exception> onError, Exception e)
        {
            try
            {
                onError.Invoke(e);
                _tx.Commit();
            }
            catch (Exception)
            {
                _tx.Rollback();
                throw;
            }
        }

        /*
        void ExecuteAsync(Action action, Action<Exception>? onError = null);
        
        R Submit<R>(Func<R> function, Action<Exception>? onError = null);
        
        R SubmitAsync<R>(Func<R> function, Action<Exception>? onError = null);
        
        private R Fallback<R>(Func<Exception, R> onError, Exception e)
        {
            throw new NotImplementedException();
        }
 */
        
    }
}