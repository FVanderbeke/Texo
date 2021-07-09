using System;
using NodaTime;

namespace Texo.Application.Api.Model.V1
{
    public class ProjectDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; }
        public Instant CreationDate { get; set; }
        public Instant ModificationDate { get; set; }
    }
}