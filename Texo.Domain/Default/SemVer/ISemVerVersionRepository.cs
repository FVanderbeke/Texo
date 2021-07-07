using Texo.Domain.Api.Repository;

namespace Texo.Domain.Default.SemVer
{
    public interface ISemVerVersionRepository : IVersionRepository<SemVerVersion, SemVerStoredVersion>
    {
        
    }
}