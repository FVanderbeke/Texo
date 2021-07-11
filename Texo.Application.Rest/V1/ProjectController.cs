#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Texo.Application.Api.Model.V1.Dto;
using Texo.Domain.Model.Entity;
using Texo.Domain.Model.Service;
using Texo.Infrastructure.Db.Internal;

namespace Texo.Application.Rest.V1
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/v1/[controller]s")]
    public class ProjectController : Controller
    {
        private readonly ProjectManager _projects;

        public ProjectController(ProjectManager projects, DbContext context)
        {
            // Todo : change it... Ensure somew
            
            context.Database.EnsureCreated();
            
            _projects = projects;
        }

        private ProjectDto ToDto(Project project)
        {
            return new()
            {
                Id = project.Id,
                Name = project.Name
            };
        }
        
        private ProjectDetailDto ToDetailDto(Project project)
        {
            var result = new ProjectDetailDto()
            {
                Id = project.Id,
                Name = project.Name,
                CreationDate = project.CreationDate
            };

            project.Description.IfSome(description => result.Description = description);
            project.ModificationDate.IfSome(date => result.ModificationDate = date);

            return result;
        }

        [HttpPost]
        public async Task<ProjectDetailDto> Save(string name, string? description = null)
            => _projects.Declare(name, description).Map(ToDetailDto).IfFailThrow();


        [HttpGet]
        public async Task<IEnumerable<ProjectDto>> FindAll() =>
            _projects.All().Map(list => list.Map(ToDto)).IfFailThrow();

        [HttpGet("{id}")]
        public async Task<ProjectDetailDto> FindById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}