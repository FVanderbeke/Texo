#nullable enable
using System;
using LanguageExt;
using NodaTime;
using static LanguageExt.Prelude;

namespace Texo.Domain.Model.Entity
{
    public record Project(Guid Id, string Name, Instant CreationDate, Option<Instant> ModificationDate = default,
        Option<string> Description = default)
    {
        public Project UpdateName(Instant modificationDate, string newName)
        {
            return new(
                Id,
                newName,
                CreationDate,
                Description: Description,
                ModificationDate: Option<Instant>.Some(modificationDate)
            );
        }
        
        public Project UpdateDescription(Instant modificationDate, string? newDescription = null)
        {
            return new(
                Id,
                Name,
                CreationDate,
                Description: Optional<string>(newDescription).Filter(d => !string.IsNullOrWhiteSpace(d)),
                ModificationDate: Optional(modificationDate)
            );
        }

        public Project CleanDescription(Instant modificationDate)
        {
            return new(
                Id,
                Name,
                CreationDate,
                Description: Option<string>.None,
                ModificationDate: Optional(modificationDate)
            );
        }

        public virtual bool Equals(Project? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id) && Name == other.Name && CreationDate.Equals(other.CreationDate) && ModificationDate.Equals(other.ModificationDate) && Description.Equals(other.Description);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, CreationDate, ModificationDate, Description);
        }
    }
}