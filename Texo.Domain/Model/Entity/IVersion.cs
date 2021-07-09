using System;

namespace Texo.Domain.Model.Entity
{
    /// <summary>
    /// Model version. 
    /// </summary>
    public interface IVersion : IComparable<IVersion>, IEquatable<IVersion>
    {
        string Label { get; }
    }
}
