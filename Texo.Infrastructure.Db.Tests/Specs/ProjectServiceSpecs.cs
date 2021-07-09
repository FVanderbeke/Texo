#nullable enable
using System.Data.Common;
using System.Data.SQLite;
using Autofac;
using NodaTime;
using NUnit.Framework;
using Texo.Domain.Module;
using Texo.Infrastructure.Db.Internal;
using Texo.Infrastructure.Db.Module;
using Texo.Infrastructure.Db.Service;
using FluentAssertions;
using LanguageExt;
using Serilog;
using Texo.Domain.Model.Entity;
using Texo.Domain.Model.Service;
using static LanguageExt.Prelude;

namespace Texo.Infrastructure.Db.Tests.Specs
{
    public class ProjectServiceSpecs
    {
        private SQLiteConnection? _connection;
        private ProjectManager? _manager;
        private Instant _startDate;
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

            builder.RegisterInstance(logger).As<ILogger>();
            builder.RegisterInstance(_connection).As<DbConnection>();
            builder.RegisterInstance(TexoUtils.DefaultClock).As<IClock>();
            builder.RegisterInstance(TexoUtils.DefaultIdGenerator).As<IIdGenerator>();
            builder.RegisterModule(new DomainModule());
            builder.RegisterModule(new DbModule());

            _container = builder.Build();
            _dbContext = _container.Resolve<DbContext>();
            _manager = _container.Resolve<ProjectManager>();
            
            // Mandatory! Forcing SQLite file creation to work. 
            _dbContext.Database.EnsureCreated();
            
            _startDate = TexoUtils.DefaultClock.GetCurrentInstant();
        }

        [TearDown]
        public void TearDown()
        {
            _connection?.Dispose();
        }

        private void AssertOnProject(Project project, string expectedName, string? expectedDescription)
        {
            var endDate = TexoUtils.DefaultClock.GetCurrentInstant();
            
            project.Should().NotBeNull();
            project.Id.Should().NotBeEmpty();
            project.Name.Should().Be(expectedName, "because name has been defined in the 'Arrange' step.");
            project.CreationDate.Should().BeGreaterOrEqualTo(_startDate, "because created during this test");
            project.CreationDate.Should().BeLessOrEqualTo(endDate, "because created during this test");
            project.ModificationDate.Should<Option<Instant>>().Be(Option<Instant>.None);

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
        [TestCase("test", "")]
        [TestCase("test", "     ")]
        [TestCase("test", null)]
        [TestCase("test", "with description")]
        public void TwoDeclare_Should_ReturnTwoNewProjects(string projectName, string? description)
        {
            // Arrange
            var name1 = $"{projectName}_1";
            var name2 = $"{projectName}_2";
            var description1 = string.IsNullOrWhiteSpace(description) ? description : $"{description}_1";
            var description2 = string.IsNullOrWhiteSpace(description) ? description : $"{description}_2";
            
            // Act 
            var result1 = _manager?.Declare(name1, description1);
            var result2 = _manager?.Declare(name2, description2);

            // Assert
            result1.Match(
                Succ: project => AssertOnProject(project, name1, description1),
                Fail: error => Assert.Fail(error.Message)
            );
            
            result2.Match(
                Succ: project => AssertOnProject(project, name2, description2),
                Fail: error => Assert.Fail(error.Message));

            result2.IfSucc(project2 => result1.IfSucc(project1 =>
                project1.CreationDate.Should().BeLessOrEqualTo(project2.CreationDate,
                    "because project1 has been created before project2.")
            ));
        }
    }
}