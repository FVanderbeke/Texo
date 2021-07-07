using Texo.Domain.Api.Factory;

namespace Texo.Domain.Default.SemVer
{
    public interface ISemVerVersionFactory : IVersionFactory<SemVerVersion, SemVerStoredVersion>
    {
    }
}