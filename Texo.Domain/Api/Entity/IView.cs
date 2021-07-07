using LanguageExt;
using Texo.Domain.Api.ValueObject;

namespace Texo.Domain.Api.Entity
{
    public interface IView
    {
        ViewId Id { get; }
        string Name { get; }
        ViewType Type { get; }
        Option<string> Description { get; }
    }
}
