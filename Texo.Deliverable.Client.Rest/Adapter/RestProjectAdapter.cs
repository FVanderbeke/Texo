#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Texo.Application.Api.Model.V1.Adapter;
using Texo.Application.Api.Model.V1.Dto;
using Texo.Application.Api.Model.V1.Query;

namespace Texo.Deliverable.Client.Rest.Adapter
{
    public class RestProjectAdapter: IProjectAdapter
    {
        private HttpClient _client;

        public RestProjectAdapter(HttpClient client)
        {
            _client = client;
        }

        public Task<ProjectDetailDto> AddAsync(string name, string? description = null)
        {
            throw new NotImplementedException();
        }

        public Task<ProjectDetailDto?> FindByIdAsync(Guid projectId)
        {
            throw new NotImplementedException();
        }

        public Task<ProjectDetailDto?> FindByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProjectDto>> FindAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProjectDto>> FindBy(ProjectQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<ProjectDetailDto> ModifyAsync(Guid projectId, string? newName = null, string? newDescription = null)
        {
            throw new NotImplementedException();
        }
    }
}