#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using NodaTime;
using Texo.Domain.Model.Entity;
using Texo.Domain.Model.Factory;
using Texo.Domain.Model.Repository;

namespace Texo.Infrastructure.InMemory.Dao
{
    public class InMemoryProjectDao : IProjectFactory, IProjectRepository
    {
        private readonly Dictionary<Guid, Project> _projects;

        public InMemoryProjectDao(Dictionary<Guid, Project> projects)
        {
            _projects = projects;
        }

        public Try<Project> Create(Guid id, string name, Instant creationDate, string? description = null)
        {
            return Prelude.Try(() => new Project(id, name, creationDate, Description: Prelude.Optional(description).Filter(d => !string.IsNullOrWhiteSpace(d))));
        }

        public TryOption<Project> FindOne(Guid projectId)
        {
            return Prelude.TryOption(() => _projects[projectId]);
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

        public TryOption<Project> FindByName(string projectName)
        {
            return Prelude.TryOption(() => _projects.Values.Single(p => p.Name == projectName));
        }

        public bool Exists(string name)
        {
            return _projects.Values.Any(p => p.Name == name);
        }
    }
}