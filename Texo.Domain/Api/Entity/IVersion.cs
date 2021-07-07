using System;

namespace Texo.Domain.Api.Entity
{
    /// <summary>
    /// Model version. 
    /// </summary>
    public interface IVersion : IComparable<IVersion>, IEquatable<IVersion>
    {
        string Label { get; }
    }
}
