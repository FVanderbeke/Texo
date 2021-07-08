using System;
using System.Collections.Generic;
using Autofac;
using LanguageExt;
using Moq;
using NodaTime;
using NUnit.Framework;
using Texo.Domain.Api.Entity;
using Texo.Domain.Api.Factory;
using Texo.Domain.Api.Provider;
using Texo.Domain.Api.Repository;
using Texo.Domain.Api.Service;
using Texo.Domain.Module;
using FluentAssertions;
using Serilog;
using Serilog.Core;
using static LanguageExt.List;

namespace Texo.Domain.Tests
{
    public class ProjectManagerTests
    {
        private ILifetimeScope _autofacScope;
        private ProjectManager _projects;
        private IClock _clock;
        private IIdGenerator _idGenerator;
        private ITransactionProvider _txProvider;
        private IProjectFactory _factory;
        private IProjectRepository _repository;

        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();

            // Declaring the logger
            var logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            
            // Creating mock instances.
            _txProvider = Mock.Of<ITransactionProvider>();
            _clock = Mock.Of<IClock>();
            _idGenerator = Mock.Of<IIdGenerator>();
            _factory = Mock.Of<IProjectFactory>();
            _repository = Mock.Of<IProjectRepository>();
            
            // Mocking specific method calls.
            Mock.Get(_txProvider)
                .Setup(tx => tx.Priority)
                .Returns(1);

            // Registering in container mock instances.
            builder.RegisterInstance(logger).As<Logger>();
            builder.RegisterInstance(_clock).As<IClock>();
            builder.RegisterInstance(_idGenerator).As<IIdGenerator>();
            builder.RegisterInstance(_txProvider).As<ITransactionProvider>();
            builder.RegisterInstance(_factory).As<IProjectFactory>();
            builder.RegisterInstance(_repository).As<IProjectRepository>();

            // Adding default Domain module.
            builder.RegisterModule<DomainModule>();

            // Creating the container.
            var container = builder.Build();
            _autofacScope = container.BeginLifetimeScope();

            // Getting back created instances.
            _projects = _autofacScope.Resolve<ProjectManager>();
        }

        [TearDown]
        public void TearDown()
        {
            _autofacScope.Dispose();
        }

        [Test]
        public void CallDeclareWithDescription_Should_ReturnNewProjectWithDescription()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            const string name = "Project name";
            var currentTime = Instant.FromUtc(2021, 07, 03, 01, 00, 00);
            const string description = "this is a test";

            // Setting default mocks behaviors.

            Mock.Get(_clock).Setup(c => c.GetCurrentInstant()).Returns(currentTime);
            Mock.Get(_idGenerator).Setup(g => g.NewGuid()).Returns(projectId);

            Mock.Get(_factory)
                .Setup(factory =>
                    factory.Create(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Instant>(), It.IsAny<string>()))
                .Returns((Guid id, string n, Instant i, string d) => () =>
                    new Project(Id: id, Name: n, CreationDate: i, Description: Option<string>.Some(d)));

            // Act
            var project = _projects.Declare(name, description);
            // Warning ! Needs to call match method. Try is lazy.

            // Assert
            project.Match(Succ: p =>
            {
                Assert.AreEqual(projectId, p.Id);
                Assert.AreEqual(name, p.Name);
                Assert.AreEqual(currentTime, p.CreationDate);
                Assert.AreEqual(Option<string>.Some(description), p.Description);
                Assert.IsTrue(p.ModificationDate.IsNone);

                Mock.Get(_factory).Verify(
                    f => f.Create(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Instant>(), It.IsAny<string>()),
                    Times.Once);
                Mock.Get(_txProvider).Verify(t => t.Begin(It.IsAny<Dictionary<string, object>>()), Times.Once);
                Mock.Get(_txProvider).Verify(t => t.Commit(It.IsAny<Dictionary<string, object>>()), Times.Once);
                Mock.Get(_txProvider).Verify(t => t.Rollback(It.IsAny<Dictionary<string, object>>()), Times.Never);
            }, Fail: e => { Assert.Fail("Error occurred: " + e.Message); });
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("   ")]
        public void CallDeclareWithoutDescription_Should_ReturnNewProjectWithEmptyDescription(string description)
        {
            // Arrange
            var projectId = Guid.NewGuid();
            const string name = "Project name";
            var currentTime = Instant.FromUtc(2021, 07, 03, 01, 00, 00);

            // Setting default mocks behaviors.

            Mock.Get(_clock).Setup(c => c.GetCurrentInstant()).Returns(currentTime);
            Mock.Get(_idGenerator).Setup(g => g.NewGuid()).Returns(projectId);

            Mock.Get(_factory)
                .Setup(factory =>
                    factory.Create(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Instant>(), It.IsAny<string>()))
                .Returns((Guid id, string n, Instant i, string d) =>
                {
                    // Asserting that the factory is called with a null value as description.
                    Assert.IsNull(d);
                    return () => new Project(id, n, i, Description: Option<string>.None);
                });

            // Act
            var project = _projects.Declare(name, description);
            // Warning ! Needs to call match method. Try is lazy.

            // Assert
            project.Match(Succ: p =>
            {
                Assert.AreEqual(projectId, p.Id);
                Assert.AreEqual(name, p.Name);
                Assert.AreEqual(currentTime, p.CreationDate);
                Assert.IsTrue(p.Description.IsNone);
                Assert.IsTrue(p.ModificationDate.IsNone);

                Mock.Get(_factory)
                    .Verify(f => f.Create(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Instant>(), null), Times.Once);
                Mock.Get(_txProvider).Verify(t => t.Begin(It.IsAny<Dictionary<string, object>>()), Times.Once);
                Mock.Get(_txProvider).Verify(t => t.Commit(It.IsAny<Dictionary<string, object>>()), Times.Once);
                Mock.Get(_txProvider).Verify(t => t.Rollback(It.IsAny<Dictionary<string, object>>()), Times.Never);
            }, Fail: e => { Assert.Fail("Error occurred: " + e.Message); });
        }

        [Test]
        public void CallModifyName_Should_ReturnUpdatedProject()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            const string name = "Project name";
            const string newName = "Project name 2";
            var creationDate = Instant.FromUtc(2021, 07, 03, 01, 00, 00);
            var modificationDate = Instant.FromUtc(2021, 07, 04, 01, 00, 00);

            Mock.Get(_clock).Setup(c => c.GetCurrentInstant()).Returns(modificationDate);

            // Setting default mocks behaviors.
            Mock.Get(_repository)
                .Setup(repo => repo.FindOne(projectId))
                .Returns((Guid _) => () =>
                    new Project(projectId, name, creationDate, Option<Instant>.None, Option<string>.None));

            Mock.Get(_repository)
                .Setup(repo => repo.Update(It.IsAny<Project>()))
                .Returns<Project>((p) => () => p);

            // Act
            var project = _projects.ModifyName(projectId, newName);
            // Warning ! Needs to call match method. Try is lazy.

            // Assert
            project.Match(Some: p =>
                {
                    Assert.AreEqual(projectId, p.Id);
                    Assert.AreEqual(newName, p.Name);
                    Assert.AreEqual(creationDate, p.CreationDate);
                    Assert.IsTrue(p.Description.IsNone);
                    Assert.AreEqual(modificationDate, p.ModificationDate.IfNone(creationDate));

                    Mock.Get(_repository).Verify(r => r.FindOne(projectId), Times.Once);
                    Mock.Get(_repository).Verify(r => r.Update(It.IsAny<Project>()), Times.Once);
                    Mock.Get(_clock).Verify(c => c.GetCurrentInstant(), Times.Once);
                },
                None: () => Assert.Fail("Project should be found."),
                Fail: e => { Assert.Fail("Error occurred: " + e.Message); });
        }


        [Test]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase(null)]
        public void CallModifyDescription_Should_ReturnUpdatedProject(string newDescription)
        {
            // Arrange
            var projectId = Guid.NewGuid();
            const string name = "Project name";
            var creationDate = Instant.FromUtc(2021, 07, 03, 01, 00, 00);
            var modificationDate = Instant.FromUtc(2021, 07, 04, 01, 00, 00);

            Mock.Get(_clock).Setup(c => c.GetCurrentInstant()).Returns(modificationDate);

            // Setting default mocks behaviors.
            Mock.Get(_repository)
                .Setup(repo => repo.FindOne(projectId))
                .Returns((Guid _) => () => new Project(projectId, name, creationDate, Option<Instant>.None,
                    Option<string>.Some("old description")));

            Mock.Get(_repository)
                .Setup(repo => repo.Update(It.IsAny<Project>()))
                .Returns<Project>((p) => () => p);

            // Act
            var project = _projects.ModifyDescription(projectId, newDescription);
            // Warning ! Needs to call match method. Try is lazy.

            // Assert
            project.Match(Some: (p) =>
                {
                    Assert.AreEqual(projectId, p.Id);
                    Assert.AreEqual(name, p.Name);
                    Assert.AreEqual(creationDate, p.CreationDate);
                    Assert.IsTrue(p.Description.IsNone);
                    Assert.AreEqual(modificationDate, p.ModificationDate.IfNone(creationDate));

                    Mock.Get(_repository).Verify(r => r.FindOne(projectId), Times.Once);
                    Mock.Get(_repository).Verify(r => r.Update(It.IsAny<Project>()), Times.Once);
                    Mock.Get(_clock).Verify(c => c.GetCurrentInstant(), Times.Once);
                },
                None: () => Assert.Fail("Project should be found."),
                Fail: (e) => Assert.Fail("Error occurred: " + e.Message));
        }

        [Test]
        public void CallAll_Should_ReturnAllProjects()
        {
            // Arrange
            var creationDate = NodaTime.SystemClock.Instance.GetCurrentInstant();

            var project1 = new Project(Guid.NewGuid(), "project1", creationDate);
            var project2 = new Project(Guid.NewGuid(), "project1", creationDate);
            
            var projects = create(project1, project2);
            
            Mock.Get(_repository).Setup(repo => repo.FindAll()).Returns(() => projects);

            // Act
            var result = _projects.All();

            result.Match(
                p => p.Should().Contain(project1).And.Contain(project2),
                error => Assert.Fail("Error : " + error.Message));
        }
    }
}