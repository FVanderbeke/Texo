using System;
using LanguageExt;
using NodaTime;
using Texo.Domain.Api.Entity;

namespace Texo.Domain.Api.Factory
{
    public interface IStoredVersion<out V> where V : IVersion
    {
        Guid Id { get; }
        Guid ProjectId { get; }
        Instant CreationDate { get; }
        V Version { get; }
    }

    public interface IVersionFactory<V, S>
        where V : IVersion
        where S : IStoredVersion<V>
    {
        Try<S> Create(Guid projectId, V version);
    }
}