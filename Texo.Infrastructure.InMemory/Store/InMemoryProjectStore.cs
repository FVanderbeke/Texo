using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using NodaTime;
using Texo.Domain.Api.Entity;
using Texo.Domain.Api.Factory;
using Texo.Domain.Api.Repository;
using static LanguageExt.Prelude;

namespace Texo.Infrastructure.InMemory.Store
{
    public class InMemoryProjectStore : IProjectFactory, IProjectRepository
    {
        private readonly Dictionary<Guid, Project> _projects;

        public InMemoryProjectStore(Dictionary<Guid, Project> projects)
        {
            _projects = projects;
        }

        public Try<Project> Create(Guid id, string name, Instant creationDate, string? description = null)
        {
            return Try(() => new Project(id, name, creationDate, Description: Optional(description).Filter(d => !string.IsNullOrWhiteSpace(d))));
        }

        public TryOption<Project> FindOne(Guid projectId)
        {
            return TryOption(() => _projects[projectId]);
        }

        public Try<Project> Update(Project project)
        {
            return FindOne(project.Id).ToTry()
                .Map(opt => opt.IfNone(() => throw new ArgumentException($"No project found for the given GUID[{project.Id}].")))
                .Map(p =>
                {
                    _projects[p.Id] = project;
                    return project;
                });
        }

        public void Delete(Guid projectId)
        {
            _projects.Remove(projectId);
        }

        public Try<IEnumerable<Project>> FindAll()
        {
            return () => _projects.Values.ToList();
        }
    }
}