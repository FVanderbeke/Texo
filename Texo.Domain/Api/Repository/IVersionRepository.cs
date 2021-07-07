using System;
using LanguageExt;
using Texo.Domain.Api.Entity;
using Texo.Domain.Api.Factory;

namespace Texo.Domain.Api.Repository
{
    public interface IVersionRepository
    {
    }

    public interface IVersionRepository<V, S> : IVersionRepository
        where V : IVersion
        where S : IStoredVersion<V>
    {
        Option<S> FindOne(Guid versionId);
        Option<S> FindByProjectAndVersion(Guid projectId, V version);
        Option<S> FindByProjectAndLabel(Guid projectId, string versionLabel);

        Lst<S> FindAllByProject(Guid projectId);

        void Remove(Guid versionId);
        void RemoveByProjectAndLabel(Guid projectId, string versionLabel);
    }
}
