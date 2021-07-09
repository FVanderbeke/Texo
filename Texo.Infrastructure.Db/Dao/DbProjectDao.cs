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

        public Try<Project> Create(Guid id, string name, Instant creationDate, string? description = null)
        {
            return Prelude.Try(() =>
            {
                try
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
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error when trying to create new project '{Name}' with id '{Id}'", name, id);
                    throw;
                }
            });
        }

        public TryOption<Project> FindOne(Guid projectId)
        {
            throw new NotImplementedException();
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
            return Prelude.Try<IEnumerable<Project>>(() => from p in _context.Projects select p.ToProject());
        }
    }
}