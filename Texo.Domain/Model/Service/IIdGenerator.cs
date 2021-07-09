using System;

namespace Texo.Domain.Model.Service
{
    public interface IIdGenerator
    {
        Guid NewGuid();
    }
}