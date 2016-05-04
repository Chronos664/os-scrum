using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using OpenSourceScrumTool.Utilities;

namespace OpenSourceScrumTool.Models
{
    [Serializable]
    [Table("Team")]
    public partial class Team : IModelContent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Team()
        {
            Users = new HashSet<User>();
            Projects = new HashSet<Project>();
        }
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(255)]
        public string TeamName { get; set; }

        [IgnoreDataMember]
        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }
        
        [JsonIgnore]
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Project> Projects { get; set; }
        public bool Archived { get; set; }

        public object ToDTO()
        {
            return new TeamDTO
            {
                TeamID = ID,
                TeamName = TeamName,
                Archived = Archived
            };
        }

        public object GetDetails()
        {
            return new TeamDetailsDTO
            {
                TeamID = ID,
                TeamName = TeamName,
                Users = Users.ToDTO<UserDTO>(),
                Archived = Archived,
                AssignedProjects = Projects.ToDTO<ProjectDTO>()
            };
        }

        public void UpdateItem(object item)
        {
            Team tempTeam = (Team) item;
            TeamName = tempTeam.TeamName;
            Archived = tempTeam.Archived;
        }
    }

    public class TeamDTO
    {
        public int TeamID { get; set; }
        public string TeamName { get; set;}
        public bool Archived { get; set; }
    }

    public class TeamDetailsDTO : TeamDTO
    {
        public IEnumerable<UserDTO> Users { get; set; }
        public IEnumerable<ProjectDTO> AssignedProjects { get; set; } 
    }
}
