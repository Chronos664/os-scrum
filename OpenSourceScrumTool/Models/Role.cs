using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Newtonsoft.Json;
using OpenSourceScrumTool.Utilities;

namespace OpenSourceScrumTool.Models
{
    [Serializable]
    public partial class Role : IModelContent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Role()
        {
            UserInRoles = new HashSet<UserInRole>();
        }
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(255)]
        public string RoleName { get; set; }
        [StringLength(255)]
        public string ADGroupName { get; set; }
        public int RoleType { get; set; }

        [IgnoreDataMember]
        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserInRole> UserInRoles { get; set; }

        public object ToDTO()
        {
            return new RoleDTO
            {
                RoleID = ID,
                RoleName = RoleName,
                AdGroupName = ADGroupName,
                RoleType = RoleType
            };
        }

        public object GetDetails()
        {
            return new RoleDetailsDTO
            {
                RoleID = ID,
                RoleName = RoleName,
                AdGroupName = ADGroupName,
                UsersInRole = UserInRoles.ToDTO<UserInRoleDTO>(),
                RoleType = RoleType
            };
        }

        public void UpdateItem(object item)
        {
            Role tempRole = (Role) item;
            RoleName = tempRole.RoleName;
            ADGroupName = tempRole.ADGroupName;
            RoleType = tempRole.RoleType;
        }
    }

    public class RoleDTO
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string AdGroupName { get; set; }
        public int RoleType { get; set; }
    }

    public class RoleDetailsDTO : RoleDTO
    {
        public IEnumerable<UserInRoleDTO> UsersInRole { get; set; }
    }
}
