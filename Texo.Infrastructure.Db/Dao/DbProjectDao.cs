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

        private Project AddEntity(ProjectEntity entity)
        {
            var context = _txService.CurrentDbContext();
           
            context.Projects.Add(entity);

            // Forcing save to update the internal entity Id.
            context.SaveChanges();
            
            return entity.ToProject();
        }
        
        public Try<Project> Create(Guid id, string name, Instant creationDate, string? description = null) 
        {
            ProjectEntity entity = new()
            {
                Gid = id,
                Name = name,
                CreationDate = creationDate.ToDateTimeUtc(),
                Description = description
            };
            return Try(AddEntity(entity));
        }

        public TryOption<Project> FindOne(Guid projectId)
        {
            return TryOption
            (_txService.CurrentDbContext().Projects.SingleOrDefault(p => p.Gid.Equals(projectId))?.ToProject() ??
             null)!;
        }

        private Project UpdateEntity(Project updated)
        {
            var entity = _txService.CurrentDbContext().Projects.Single(p => p.Gid.Equals(updated.Id));
            _txService.CurrentDbContext().Projects.Update(entity);
            entity.FromProject(updated);
            _txService.CurrentDbContext().SaveChanges();
            return entity.ToProject();
        }

        public Try<Project> Update(Project project)
        {
            return Try(UpdateEntity(project));
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