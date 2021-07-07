using System;
using LanguageExt;
using NodaTime;
using Texo.Domain.Api.Entity;
using Texo.Domain.Api.Factory;
using Texo.Domain.Api.Repository;
using Texo.Infrastructure.Db.Entity;
using Texo.Infrastructure.Db.Internal;
using static LanguageExt.Prelude;

namespace Texo.Infrastructure.Db.Store
{
    public class DbProjectStore : IProjectFactory, IProjectRepository
    {
        private readonly DbContext _context;

        public DbProjectStore(DbContext context)
        {
            _context = context;
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
                    Console.WriteLine(e.Message);
                    throw;
                }
            });
        }

        public TryOption<Project> FindOne(Guid projectId)
        {
            _context.Projects.Find();
            return null;
        }

        public Try<Project> Update(Project project)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid projectId)
        {
            throw new NotImplementedException();
        }
    }
}