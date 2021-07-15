#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Texo.Application.Api.Model.V1.Dto;
using Texo.Domain.Model.Entity;
using Texo.Domain.Model.Service;

namespace Texo.Application.Rest.V1
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/v1/[controller]s")]
    public class ProjectController : Controller
    {
        private readonly ProjectManager _projects;

        public ProjectController(ProjectManager projects)
        {
           
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
        public async Task<IActionResult> FindById(Guid id)
        {
            var result = _projects.One(id);
            if (result.IsNone())
            {
                return NotFound();
            }
            
            return _projects.One(id).Map(ToDetailDto).Map(Ok).IfFailThrow();
        }

        [HttpDelete]
        public async Task<IActionResult> Remove(Guid id)
        {
            _projects.Remove(id);

            return Ok();
        }

        [HttpGet("exists/{name}")]
        public async Task<bool> Exists(string name)
        {
            return _projects.Contains(name);
        }
    }
}