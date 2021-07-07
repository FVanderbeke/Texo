using System;
using LanguageExt;
using Texo.Domain.Api.Entity;

namespace Texo.Domain.Api.Repository
{
    public interface IProjectRepository
    {
        TryOption<Project> FindOne(Guid projectId);
        
        Try<Project> Update(Project project);
        
        void Delete(Guid projectId);
    }
}