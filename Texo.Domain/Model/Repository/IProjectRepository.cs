using System;
using System.Collections.Generic;
using LanguageExt;
using Texo.Domain.Model.Entity;

namespace Texo.Domain.Model.Repository
{
    public interface IProjectRepository
    {
        TryOption<Project> FindOne(Guid projectId);
        
        Try<Project> Update(Project project);
        
        void Delete(Guid projectId);
        Try<IEnumerable<Project>> FindAll();
    }
}