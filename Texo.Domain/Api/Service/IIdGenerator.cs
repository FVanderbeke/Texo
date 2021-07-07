using System;

namespace Texo.Domain.Api.Service
{
    public interface IIdGenerator
    {
        Guid NewGuid();
    }
}