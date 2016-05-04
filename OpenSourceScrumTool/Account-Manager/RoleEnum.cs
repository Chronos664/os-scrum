using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenSourceScrumTool.DAL;
using OpenSourceScrumTool.Models;
using OpenSourceScrumTool.Utilities;

namespace OpenSourceScrumTool.Account_Manager
{
    public static class RoleEnum
    {
        public enum Roles
        {
            Administrator = 0,
            ScrumMaster = 1,
            TeamLeader = 2,
            TeamMember = 3,
            Stakeholder = 4,
            Viewonly = 5,
            Unauthorized = 6
        }

        public static void AddRolesToDB()
        {
            using (DataAccessLayer db = new DataAccessLayer())
            {
                if (!RoleExists(Roles.Administrator))
                {
                    db.AddRole(new Role()
                    {
                        ID = 0,
                        ADGroupName = ConfigManager.DefaultAdminGroupName(),
                        RoleName = "System Administrator",
                        RoleType = (int) Roles.Administrator
                    });
                }
                if (!RoleExists(Roles.ScrumMaster))
                {
                    db.AddRole(new Role()
                    {
                        ID = 0,
                        ADGroupName = ConfigManager.DefaultScrumMasterGroupName(),
                        RoleName = "SCRUM Master",
                        RoleType = (int)Roles.ScrumMaster
                    });
                }
                if (!RoleExists(Roles.TeamLeader))
                {
                    db.AddRole(new Role()
                    {
                        ID = 0,
                        ADGroupName = ConfigManager.DefaultTeamLeaderGroupName(),
                        RoleName = "Team Leader",
                        RoleType = (int)Roles.TeamLeader
                    });
                }
                if (!RoleExists(Roles.TeamMember))
                {
                    db.AddRole(new Role()
                    {
                        ID = 0,
                        ADGroupName = ConfigManager.DefaultTeamMemberGroupName(),
                        RoleName = "Team Member",
                        RoleType = (int)Roles.TeamMember
                    });
                }
                if (!RoleExists(Roles.Stakeholder))
                {
                    db.AddRole(new Role()
                    {
                        ID = 0,
                        ADGroupName = ConfigManager.DefaultStakeHolderGroupName(),
                        RoleName = "Stakeholder",
                        RoleType = (int)Roles.Stakeholder
                    });
                }
                if (!RoleExists(Roles.Viewonly))
                {
                    db.AddRole(new Role()
                    {
                        ID = 0,
                        ADGroupName = ConfigManager.DefaultViewOnlyGroupName(),
                        RoleName = "View Only",
                        RoleType = (int)Roles.Viewonly
                    });
                }
                if (!RoleExists(Roles.Unauthorized))
                {
                    db.AddRole(new Role()
                    {
                        ID = 0,
                        ADGroupName = ConfigManager.DefaultUnauthorizedGroupName(),
                        RoleName = "Unauthorized",
                        RoleType = (int)Roles.Unauthorized
                    });
                }

            }
        }

        private static bool RoleExists(Roles roleExists)
        {
            using (DataAccessLayer db = new DataAccessLayer())
            {
                List<Role> dbRoles = db.GetRoles();
                foreach (Role role in dbRoles)
                {
                    if (role.RoleType == (int) roleExists)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}