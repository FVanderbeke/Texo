using System;
using NodaTime;
using Texo.Domain.Api.Factory;

namespace Texo.Domain.Default.SemVer
{
    public record SemVerStoredVersion(Guid Id, Guid ProjectId, Instant CreationDate, SemVerVersion Version) : IStoredVersion<SemVerVersion>
    {
    }
}