using System.Data.Common;

namespace Texo.Infrastructure.Db.Service
{
    public class DbContextFactory
    {
        private readonly DbConnection _connection;

        public DbContextFactory(DbConnection connection)
        {
            _connection = connection;
        }

        public void Activate()
        {
            // Forcing database schema creation.
            using var initContext = Create();

            initContext.Database.EnsureCreated();
        }
        
        public DbContext Create() => new(_connection);
    }
}