using System.Collections.Generic;
using Texo.Domain.Api.Provider;

namespace Texo.Infrastructure.InMemory.Service
{
    public class InMemoryTransactionProvider : ITransactionProvider
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