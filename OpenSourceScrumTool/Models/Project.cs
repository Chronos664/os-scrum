using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Web;
using System.Collections.Generic;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using OpenSourceScrumTool.Utilities;

namespace OpenSourceScrumTool.Models
{
    [Serializable]
    [Table("Project")]
    public partial class Project : IModelContent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Project()
        {
            Iterations = new HashSet<Iteration>();
            ProductBacklogItems = new HashSet<ProductBacklogItem>();
            Teams = new HashSet<Team>();
        }

        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(255)]
        public string ProjectName { get; set; }

        public string ProjectDetails { get; set; }
        public bool Archived { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductBacklogItem> ProductBacklogItems { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Team> Teams { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Iteration> Iterations { get; set; } 

        public object ToDTO()
        {
            return new ProjectDTO
            {
                ProjectId = ID,
                ProjectName = ProjectName,
                ProjectDetails = ProjectDetails,
                Archived = Archived
            };
        }

        public object GetDetails()
        {
            return new ProjectDetailsDTO
            {
                ProjectId = ID,
                ProjectName = ProjectName,
                ProjectDetails = ProjectDetails,
                ProductBacklogItems = ProductBacklogItems.GetDetails<ProductBacklogItemDetailsDTO>(),
                Teams = Teams.GetDetails<TeamDetailsDTO>(),
                Iterations = Iterations.GetDetails<IterationDetailsDTO>(),
                Archived = Archived
            };
        }

        public void UpdateItem(object item)
        {
            Project tempProject = (Project)item;
            ProjectName = tempProject.ProjectName;
            ProjectDetails = tempProject.ProjectDetails;
            Archived = tempProject.Archived;
        }
    }

    public class ProjectDTO
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDetails { get; set; }
        public bool Archived { get; set; }
    }

    public class ProjectDetailsDTO : ProjectDTO
    {
        public IEnumerable<ProductBacklogItemDetailsDTO> ProductBacklogItems { get; set; }
        public IEnumerable<TeamDetailsDTO> Teams { get; set; }
        public IEnumerable<IterationDTO> Iterations { get; set; } 
    }

}
