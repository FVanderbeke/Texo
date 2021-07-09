using System.Collections.Generic;

namespace Texo.Domain.Model.Service
{
    public interface ITransactionService
    {
        int Priority { get; }

        void OnLoad(Dictionary<string, object> context);

        void OnDispose(Dictionary<string, object> context);
        
        void Begin(Dictionary<string, object> context);
        void Commit(Dictionary<string, object> context);
        void Rollback(Dictionary<string, object> context);
    }
}