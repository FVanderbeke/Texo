using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using NodaTime;
using Texo.Domain.Api.Entity;
using Texo.Domain.Api.Factory;
using Texo.Domain.Api.Repository;
using Texo.Infrastructure.Db.Entity;
using Texo.Infrastructure.Db.Internal;
using Serilog;
using Serilog.Core;
using static LanguageExt.Prelude;

namespace Texo.Infrastructure.Db.Store
{
    public class DbProjectStore : IProjectFactory, IProjectRepository
    {
        private readonly DbContext _context;
        private readonly Logger _logger;

        public DbProjectStore(DbContext context, Logger logger)
        {
            _context = context;
            _logger = logger;
        }

        public Try<Project> Create(Guid id, string name, Instant creationDate, string? description = null)
        {
            return Try(() =>
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
            return Try<IEnumerable<Project>>(() => from p in _context.Projects select p.ToProject());
        }
    }
}