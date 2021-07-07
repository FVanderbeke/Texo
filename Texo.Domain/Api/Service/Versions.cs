using System.Collections.Generic;
using Texo.Domain.Api.Provider;

namespace Texo.Domain.Api.Service
{
    public sealed class Versions
    {
        private readonly Dictionary<string, IVersionProvider> _providers;

        public Versions(IEnumerable<IVersionProvider> providers)
        {
            _providers = new Dictionary<string, IVersionProvider>();
            
        }

    }
}
