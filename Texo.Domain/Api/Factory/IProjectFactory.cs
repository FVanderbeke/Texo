#nullable enable
using System;
using LanguageExt;
using NodaTime;
using Texo.Domain.Api.Entity;

namespace Texo.Domain.Api.Factory
{
    public interface IProjectFactory
    {
        Try<Project> Create(Guid id, string name, Instant creationDate, string? description = null);
    }
}