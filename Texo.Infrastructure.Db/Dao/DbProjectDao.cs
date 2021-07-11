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
using Texo.Infrastructure.Db.Internal;
using static LanguageExt.Prelude;

namespace Texo.Infrastructure.Db.Dao
{
    public class DbProjectDao : IProjectFactory, IProjectRepository
    {
        private readonly DbContext _context;
        private readonly ILogger _logger;

        public DbProjectDao(DbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public Try<Project> Create(Guid id, string name, Instant creationDate, string? description = null) => () =>
        {
            ProjectEntity entity = new()
            {
                Gid = id,
                Name = name,
                CreationDate = creationDate.ToDateTimeUtc(),
                Description = description
            };
            _context.Projects.Add(entity);

            // Forcing save to update the internal entity Id.
            _context.SaveChanges();

            return entity.ToProject();
        };

        public TryOption<Project> FindOne(Guid projectId) => () =>
            (_context.Projects.SingleOrDefault(p => p.Gid.Equals(projectId))?.ToProject() ?? null)!;

        public Try<Project> Update(Project project)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid projectId)
        {
            throw new NotImplementedException();
        }

        public Try<IEnumerable<Project>> FindAll() => () => (from p in _context.Projects select p.ToProject()).ToList();
    }
}