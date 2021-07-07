using Texo.Domain.Api.Entity;

namespace Texo.Domain.Api.Builder
{
    public interface IVersionBuilder<V> where V : IVersion
    {
        V Build();
    }
}
