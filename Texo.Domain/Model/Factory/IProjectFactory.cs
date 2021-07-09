#nullable enable
using System;
using LanguageExt;
using NodaTime;
using Texo.Domain.Model.Entity;

namespace Texo.Domain.Model.Factory
{
    public interface IProjectFactory
    {
        Try<Project> Create(Guid id, string name, Instant creationDate, string? description = null);
    }
}