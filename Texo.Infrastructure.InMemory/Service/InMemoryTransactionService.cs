using System.Collections.Generic;
using Texo.Domain.Model.Service;

namespace Texo.Infrastructure.InMemory.Service
{
    public class InMemoryTransactionService : ITransactionService
    {
        public int Priority => 100;
        public void OnLoad(Dictionary<string, object> context)
        {
        }

        public void OnDispose(Dictionary<string, object> context)
        {
        }

        public void Begin(Dictionary<string, object> context)
        {
        }

        public void Commit(Dictionary<string, object> context)
        {
        }

        public void Rollback(Dictionary<string, object> context)
        {
        }
    }
}