using System;
using System.Data.Common;
using System.Data.SQLite;
using Autofac;
using NodaTime;
using NUnit.Framework;
using Serilog;
using Texo.Domain.Model.Factory;
using Texo.Domain.Model.Repository;
using Texo.Domain.Model.Service;
using Texo.Domain.Module;
using Texo.Infrastructure.Db.Dao;
using Texo.Infrastructure.Db.Module;
using Texo.Infrastructure.Db.Service;
using FluentAssertions;
using Texo.Domain.Model.Entity;
using Texo.Infrastructure.Db.Entity;

namespace Texo.Infrastructure.Db.Tests.Specs
{
    public class DbTransactionServiceSpecs
    {
        private SQLiteConnection? _connection;
        private DbContextFactory? _contextFactory;
        private DbProjectDao? _projectDao;
        private IContainer? _container;
        private ProjectManager? _projects;

        [SetUp]
        public void Setup()
        {
            // Instantiating the database connection (mandatory for SQLite).
            _connection = new SQLiteConnection(@"Data Source=:memory:");
            _connection.Open();

            // Declaring the logger
            var logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            // Now, creating the IOC container.
            var builder = new ContainerBuilder();

            builder.RegisterInstance(logger).As<ILogger>();
            builder.RegisterInstance(_connection).As<DbConnection>();
            builder.RegisterInstance(TexoUtils.DefaultClock).As<IClock>();
            builder.RegisterInstance(TexoUtils.DefaultIdGenerator).As<IIdGenerator>();
            builder.RegisterModule(new DomainModule());
            builder.RegisterModule(new DbModule());

            _container = builder.Build();
            _contextFactory = _container.Resolve<DbContextFactory>();
            _projectDao = _container.Resolve<IProjectFactory>() as DbProjectDao;
            _projects = _container.Resolve<ProjectManager>();

            // Checking instantiation in container...
            _container.Resolve<IProjectRepository>().Should().Be(_projectDao);
        }

        [TearDown]
        public void TearDown()
        {
            _container?.Dispose();
            _connection?.Dispose();
        }

        [Test]
        public void Submit_Should_CloseTransactionAndCleanMemory()
        {
            Project firstProject = null;
            try
            {
                firstProject = _projects?.Declare("project", "test").IfFailThrow();
                var second = _projects?.Declare("project", "error").IfFailThrow();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error = " + e.Message);
            }

            Assert.IsNotNull(firstProject);

            _projects.One(firstProject.Id).Match(
                Some: val => { },
                None: () => { },
                Fail: err => Console.WriteLine(err.Message)
            );

            var result = _projects.One(firstProject.Id).IfFailThrow();

            Assert.IsNotNull(result);
            result.Should().Be(firstProject);
        }
        
        [Test]
        public void ContextFactory_Should_GivePossibilityToManageTransaction()
        {
            
            var dbContext = _contextFactory?.Create();
            ProjectEntity entity = new()
            {
                Gid = Guid.NewGuid(),
                Name = "project",
                CreationDate = SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc(),
                Description = ""
            };
            dbContext?.Projects.Add(entity);

            dbContext?.SaveChanges();

            using (var tx = dbContext?.Database.BeginTransaction())
            {
                tx?.CreateSavepoint("BeforeMoreBlogs");
                try
                {
                    entity = new()
                    {
                        Gid = Guid.NewGuid(),
                        Name = "project",
                        CreationDate = SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc(),
                        Description = ""
                    };
                    dbContext?.Projects.Add(entity);
                    dbContext?.SaveChanges();
                    tx?.Commit();
                }
                catch (Exception e)
                {
                    tx?.RollbackToSavepoint("BeforeMoreBlogs");
                    Console.WriteLine(e.Message);
                    tx?.Dispose();
                    dbContext?.Dispose();
                    dbContext = _contextFactory?.Create();
                }
            }

            entity = new()
            {
                Gid = Guid.NewGuid(),
                Name = "project2",
                CreationDate = SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc(),
                Description = ""
            };
            dbContext?.Projects.Add(entity);
            dbContext?.SaveChanges();
        }
    }
}