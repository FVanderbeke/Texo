using Texo.Domain.Api.Builder;
using Texo.Domain.Api.Entity;
using Texo.Domain.Api.Factory;
using Texo.Domain.Api.Repository;

namespace Texo.Domain.Api.Provider
{

    public interface IVersionProvider {
        string Name { get; }
    }

    public interface IVersionProvider<V, out B, S, out F, out R> : IVersionProvider
        where V : IVersion
        where B : IVersionBuilder<V>
        where S : IStoredVersion<V>
        where F : IVersionFactory<V, S>
        where R : IVersionRepository<V, S>
    {

        B Builder();

        F Factory { get; }

        R Repository { get; }
    }
    
}
