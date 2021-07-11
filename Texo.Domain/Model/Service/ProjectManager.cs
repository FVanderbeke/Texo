#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using NodaTime;
using Texo.Domain.Model.Entity;
using Texo.Domain.Model.Factory;
using Texo.Domain.Model.Repository;
using static LanguageExt.Prelude;

namespace Texo.Domain.Model.Service
{
    public class ProjectManager
    {
        private readonly TransactionManager _txManager;
        private readonly IClock _clock;
        private readonly IIdGenerator _idGenerator;
        private readonly IProjectFactory _factory;
        private readonly IProjectRepository _repository;

        public ProjectManager(
            TransactionManager txManager, 
            IClock clock, 
            IIdGenerator idGenerator,
            IProjectFactory factory,
            IProjectRepository repository)
        {
            _txManager = txManager;
            _clock = clock;
            _idGenerator = idGenerator;
            _factory = factory;
            _repository = repository;
        }

        public Try<Project> Declare(string name, string? description = null) => _txManager.Submit(() => _factory.Create(
                    _idGenerator.NewGuid(),
                    name,
                    _clock.GetCurrentInstant(),
                    Optional(description).Filter(d => !string.IsNullOrWhiteSpace(d)).IfNoneUnsafe(() => null!)));

        private TryOption<Project> FindAndUpdate(Guid projectId, Func<Project, Project> updateFunc) => _txManager.Submit(() => _repository.FindOne(projectId).Map(updateFunc).Bind(p => _repository.Update(p).ToTryOption()));

        public TryOption<Project> ModifyName(Guid projectId, string newName) => FindAndUpdate(projectId, p => p.UpdateName(_clock.GetCurrentInstant(), newName));

        public TryOption<Project> ModifyDescription(Guid projectId, string? newDescription = null) => FindAndUpdate(projectId, p => p.UpdateDescription(_clock.GetCurrentInstant(), newDescription));

        public Try<List<Project>> All() => _txManager.Submit(() => _repository.FindAll().Map(projects => projects.ToList()));

        public TryOption<Project> One(Guid id) => _txManager.Submit(() => _repository.FindOne(id));

        public TryOption<Project> One(string name)
        {
            throw new NotImplementedException();
        }
    }
}