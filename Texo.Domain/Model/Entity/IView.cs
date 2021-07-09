using LanguageExt;
using Texo.Domain.Model.ValueObject;

namespace Texo.Domain.Model.Entity
{
    public interface IView
    {
        ViewId Id { get; }
        string Name { get; }
        ViewType Type { get; }
        Option<string> Description { get; }
    }
}
