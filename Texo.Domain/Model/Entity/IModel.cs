using System;
using NodaTime;
using Texo.Domain.Model.ValueObject;

namespace Texo.Domain.Model.Entity
{
    public interface IModel<V> :  IComparable<IModel<V>>, IEquatable<IModel<V>>
        where V : IVersion
    {
        /// <summary>
        /// Unique ID.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Model version.
        /// </summary>
        V Version { get; }

        Project Project { get; }

        /// <summary>
        /// Used label.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Gets the view to use to display this model.
        /// </summary>
        ViewId ViewId { get; }

        Instant CreationDate { get; }
    }
}
