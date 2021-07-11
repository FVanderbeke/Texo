using System.Net.Http;
using Texo.Application.Api.Model.V1;
using Texo.Application.Api.Model.V1.Adapter;
using Texo.Deliverable.Client.Rest.Adapter;

namespace Texo.Deliverable.Client.Rest
{
    public class RestClient : IClient
    {
        private readonly RestProjectAdapter _projects;
        
        public RestClient(HttpClient httpClient)
        {
            _projects = new RestProjectAdapter(httpClient);
        }

        public IProjectAdapter Projects => _projects;

    }
}