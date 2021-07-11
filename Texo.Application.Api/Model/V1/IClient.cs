using Texo.Application.Api.Model.V1.Adapter;

namespace Texo.Application.Api.Model.V1
{
    public interface IClient
    {
        IProjectAdapter Projects { get; }
    }
}