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
    public partial class User : IModelContent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            Logs = new HashSet<Log>();
            Teams = new HashSet<Team>();
            UserInRoles = new HashSet<UserInRole>();
        }
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(255)]
        public string UserName { get; set; }

        [Required]
        [StringLength(255)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(255)]
        public string LastName { get; set; }

        [StringLength(255)]
        public string emailAddress { get; set; }
        
        [JsonIgnore]
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Log> Logs { get; set; }
        [JsonIgnore]        
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Team> Teams { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserInRole> UserInRoles { get; set; }

        public object ToDTO()
        {
            return new UserDTO
            {
                UserID = ID,
                UserName = UserName,
                FirstName = FirstName,
                LastName = LastName,
                EmailAddress = emailAddress
            };
        }

        public object GetDetails()
        {
            return new UserDetailsDTO{
                UserID = ID,
                UserName = UserName,
                FirstName = FirstName,
                LastName = LastName,
                EmailAddress = emailAddress,
                UserInRoleMap = UserInRoles.ToDTO<UserInRoleDTO>().ToList(),
                CurrentRoles = UserInRoles.Select(uir => (RoleDTO) uir.Role.ToDTO()).ToList(),
                CurrentTeams = Teams.ToDTO<TeamDTO>().ToList()
            };
        }

        public void UpdateItem(object item)
        {
            User tempUser = (User) item;
            UserName = tempUser.UserName;
            FirstName = tempUser.FirstName;
            LastName = tempUser.LastName;
            emailAddress = tempUser.emailAddress;
        }
    }

    public class UserDTO
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
    }

    public class UserDetailsDTO : UserDTO
    {
        public List<UserInRoleDTO> UserInRoleMap { get; set; }
        public List<RoleDTO> CurrentRoles { get; set; }
        public List<TeamDTO> CurrentTeams { get; set; } 
    }
}
