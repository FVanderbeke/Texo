using System;

namespace Texo.Domain.Api.Service
{
    public class TransactionManager
    {
        private readonly GlobalTransactionProvider _tx;

        public TransactionManager(GlobalTransactionProvider tx)
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
        
/*
 * void Execute(Action action);
        void Execute(Action action, Action<Exception> onError);
        
        void ExecuteAsync(Action action);
        void ExecuteAsync(Action action, Action<Exception> onError);
        
        Try<R> Submit<R>(Func<R> function);
        Try<R> Submit<R>(Func<Try<R>> function);
        Try<R> Submit<R>(Func<R> function, Action<Exception> onError);
        
        TryAsync<R> SubmitAsync<R>(Func<R> function);
        TryAsync<R> SubmitAsync<R>(Func<Try<R>> function);
        TryAsync<R> SubmitAsync<R>(Func<R> function, Action<Exception> onError);
 */
        
    }
}