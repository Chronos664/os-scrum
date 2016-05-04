using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OpenSourceScrumTool.Models
{
    [Table("UserInRole")]
    public class UserInRole : IModelContent
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RoleID { get; set; }

        public bool isExplicit { get; set; } //If the role is explicit then it has been added by the application rather than implicitly defined by Active Directory

        public virtual Role Role { get; set; }

        public virtual User User { get; set; }
        public object ToDTO()
        {
            return new UserInRoleDTO()
            {
                isExplicit = isExplicit,
                RoleID = RoleID,
                UserID = UserID
            };
        }

        public object GetDetails()
        {
            return new UserInRoleDetailsDTO()
            {
                isExplicit = isExplicit,
                role = (RoleDTO)Role.ToDTO(),
                RoleID = RoleID,
                user = (UserDTO)User.ToDTO(),
                UserID = UserID
            };
        }

        public void UpdateItem(object item)
        {
            UserInRole tempUIR = (UserInRole)item;
            UserID = tempUIR.UserID;
            RoleID = tempUIR.RoleID;
            isExplicit = tempUIR.isExplicit;
        }
    }

    public class UserInRoleDTO
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public bool isExplicit { get; set; }
    }

    public class UserInRoleDetailsDTO : UserInRoleDTO
    {
        public RoleDTO role { get; set; }
        public UserDTO user { get; set; }
    }
}