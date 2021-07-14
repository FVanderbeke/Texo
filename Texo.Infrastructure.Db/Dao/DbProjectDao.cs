#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using NodaTime;
using Serilog;
using Texo.Domain.Model.Entity;
using Texo.Domain.Model.Factory;
using Texo.Domain.Model.Repository;
using Texo.Infrastructure.Db.Entity;
using Texo.Infrastructure.Db.Service;

using static LanguageExt.Prelude;

namespace Texo.Infrastructure.Db.Dao
{
    public class DbProjectDao : IProjectFactory, IProjectRepository
    {
        private readonly DbTransactionService _txService;
        private readonly ILogger _logger;

        public DbProjectDao(DbTransactionService service, ILogger logger)
        {
            _txService = service;
            _logger = logger;
        }

        public Try<Project> Create(Guid id, string name, Instant creationDate, string? description = null) 
        {
            var context = _txService.CurrentDbContext();
            ProjectEntity entity = new()
            {
                Gid = id,
                Name = name,
                CreationDate = creationDate.ToDateTimeUtc(),
                Description = description
            };
            context.Projects.Add(entity);

            // Forcing save to update the internal entity Id.
            context.SaveChanges();

            return Try(entity.ToProject());
        }

        public TryOption<Project> FindOne(Guid projectId)
        {
            return TryOption
            (_txService.CurrentDbContext().Projects.SingleOrDefault(p => p.Gid.Equals(projectId))?.ToProject() ??
             null)!;
        }

        public Try<Project> Update(Project project)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid projectId)
        {
            throw new NotImplementedException();
        }

        public Try<IEnumerable<Project>> FindAll()
        {
            return Try<IEnumerable<Project>>((from p in _txService.CurrentDbContext().Projects select p.ToProject()).ToList());
        }
    }
}