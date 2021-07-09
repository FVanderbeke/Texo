#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Texo.Domain.Model.Entity;
using static LanguageExt.Prelude;

namespace Texo.Infrastructure.Db.Entity
{
    [Table("project")]
    [Index(nameof(Gid), IsUnique = true, Name = "texo_project_idx_gid")]
    [Index(nameof(Name), IsUnique = true, Name = "texo_project_idx_name")]
    public class ProjectEntity
    {
 
        public static ProjectEntity From(Project project)
        {
            return new()
            {
                Gid = project.Id,
                Name = project.Name,
                CreationDate = project.CreationDate.ToDateTimeUtc(),
                ModificationDate = project.ModificationDate.Map(d => d.ToDateTimeUtc()).ToNullable(),
                Description = project.Description.IfNone(() => null!)
            };
        }

        
        public long Id { get; set; }
        
        [Required]
        public Guid Gid { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        [Column("creation_date")]
        public DateTime CreationDate { get; set; }
        
        [Column("modification_date")]
        public DateTime? ModificationDate { get; set; }
        
        [MaxLength(255)]
        public string? Description { get; set; }

        public Project ToProject()
        {
            Option<Instant> modifDate = Optional(ModificationDate).Map(Instant.FromDateTimeUtc);
            Option<string> description = Optional(Description);
            
            return new Project(Gid, Name, Instant.FromDateTimeUtc(CreationDate), modifDate, description);
        }
        
    }
}