using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Texo.Application.Api.Util.Query;

#nullable enable
namespace Texo.Application.Api.Model.V1
{
    public interface IProjectProvider
    {
        Task<ProjectDetailDto> AddAsync(string name, string? description = null);
        
        Task<ProjectDetailDto?> FindByIdAsync(Guid projectId);
        Task<ProjectDetailDto?> FindByNameAsync(string name);
        
        Task<IEnumerable<ProjectDto>> FindAll();
        
        Task<IEnumerable<ProjectDto>> FindBy(ProjectQuery query);

        Task<ProjectDetailDto> ModifyAsync(Guid projectId, string? newName = null, string? newDescription = null);
    }
}