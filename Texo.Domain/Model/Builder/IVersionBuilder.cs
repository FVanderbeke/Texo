using Texo.Domain.Model.Entity;

namespace Texo.Domain.Model.Builder
{
    public interface IVersionBuilder<V> where V : IVersion
    {
        V Build();
    }
}
