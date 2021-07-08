#nullable enable
using System;
using System.Data.Common;
using System.Data.SQLite;
using Autofac;
using FluentAssertions;
using LanguageExt;
using NodaTime;
using NUnit.Framework;
using Serilog;
using Serilog.Core;
using Texo.Domain.Api.Entity;
using Texo.Domain.Api.Factory;
using Texo.Domain.Api.Repository;
using Texo.Domain.Api.Service;
using Texo.Domain.Module;
using Texo.Infrastructure.Db.Internal;
using Texo.Infrastructure.Db.Module;
using Texo.Infrastructure.Db.Service;
using Texo.Infrastructure.Db.Store;

using static LanguageExt.Prelude;

namespace Texo.Infrastructure.Db.Tests.Store
{
    public class DbProjectStoreTests
    {
        private SQLiteConnection? _connection;
        private TransactionManager? _txManager;
        private DbProjectStore? _store;
        private IContainer? _container;
        private DbContext? _dbContext;

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
            
            builder.RegisterInstance(logger).As<Logger>();
            builder.RegisterInstance(_connection).As<DbConnection>();
            builder.RegisterInstance(TexoUtils.DefaultClock).As<IClock>();
            builder.RegisterInstance(TexoUtils.DefaultIdGenerator).As<IIdGenerator>();
            builder.RegisterModule(new DomainModule());
            builder.RegisterModule(new DbModule());

            _container = builder.Build();
            _txManager = _container.Resolve<TransactionManager>();
            _dbContext = _container.Resolve<DbContext>();
            _store = _container.Resolve<IProjectFactory>() as DbProjectStore;

            // Forcing SQLite file creation to work. 
            _dbContext.Database.EnsureCreated();
            
            // Checking instantiation in container...
            _container.Resolve<IProjectRepository>().Should().Be(_store);
        }

        [TearDown]
        public void TearDown()
        {
            _connection?.Dispose();
            _dbContext?.Dispose();
            _container?.Dispose();
        }

        private void AssertOnProject(Project project, Guid expectedId, string expectedName,
            Instant expectedCreationDate, string? expectedDescription)
        {
            project.Should().NotBeNull();

            project.Id.Should().Be(expectedId);
            project.Name.Should().Be(expectedName);
            project.CreationDate.Should().Be(expectedCreationDate);

            if (string.IsNullOrWhiteSpace(expectedDescription))
            {
                project.Description.Should<Option<string>>().Be(Option<string>.None);
            }
            else
            {
                project.Description.Should<Option<string>>().Be(Optional(expectedDescription));
            }
        }

        [Test]
        public void Create_Should_ReturnNewProject()
        {
            // Arrange
            var id = TexoUtils.DefaultIdGenerator.NewGuid();
            var creationDate = TexoUtils.DefaultClock.GetCurrentInstant();
            var name = "test";
            var description = "test description";

            var result = _txManager?.Submit(() => _store?.Create(id, name, creationDate, description));

            result.Match(
                Succ: project => AssertOnProject(project, id, name, creationDate, description),
                Fail: error => Assert.Fail($"Error occured : {error.Message}\n\nStack : {error.StackTrace}")
            );
        }

        [Test]
        public void FindAll_Should_ReturnAllProjects()
        {
            var id1 = TexoUtils.DefaultIdGenerator.NewGuid();
            var id2 = TexoUtils.DefaultIdGenerator.NewGuid();

            var name1 = "project1";
            var name2 = "project2";

            var date1 = TexoUtils.DefaultClock.GetCurrentInstant();
            var date2 = TexoUtils.DefaultClock.GetCurrentInstant();
            Project? project1 = null, project2 = null;

            _txManager?.Execute(() =>
            {
                project1 = _store?.Create(id1, name1, date1).IfFail(e =>
                {
                    Assert.Fail(e.Message);
                    return null;
                });
                project2 = _store?.Create(id2, name2, date2).IfFail(e =>
                {
                    Assert.Fail(e.Message);
                    return null;
                });
            });

            var result = _txManager?.Submit(() => _store?.FindAll());

            result.Match(
                projects =>
                {
                    projects.Should().HaveCount(2);
                    projects.Should().Contain(project1).And.Contain(project2);
                },
                e => Assert.Fail(e.Message));

        }
    }
}