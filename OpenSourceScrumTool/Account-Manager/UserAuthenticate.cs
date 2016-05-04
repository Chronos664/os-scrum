using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using OpenSourceScrumTool.DAL;
using OpenSourceScrumTool.Models;
using OpenSourceScrumTool.Utilities;

namespace OpenSourceScrumTool.Account_Manager
{
    public class UserAuthenticate : AuthorizeAttribute
    {
        private RoleEnum.Roles _validRole;

        public UserAuthenticate()
        {
        }

        public UserAuthenticate(RoleEnum.Roles roles)
        {
            _validRole = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = httpContext.User;
            return CheckAuth(user);
        }

        //Used for testing within code if a user holds a set permission
        public bool CheckAuth(IPrincipal user, RoleEnum.Roles validRoles)
        {
            _validRole = validRoles;
            return CheckAuth(user);
        }

        private bool CheckAuth(IPrincipal user)
        {
            RoleEnum.Roles[] currentRoles = GetRole(user).ToArray();
            if (currentRoles.Count() == 0)
                return false;
            return IsValidRole(currentRoles.ToList());
        }

        private bool IsValidRole(List<RoleEnum.Roles> userRoles)
        {
            if (userRoles.Contains(RoleEnum.Roles.Unauthorized))
            {
                return false;
            }
            switch (_validRole)
            {
                case RoleEnum.Roles.Administrator:
                    if (userRoles.Contains(RoleEnum.Roles.Administrator))
                        return true;
                    break;
                case RoleEnum.Roles.ScrumMaster:
                    if (userRoles.Contains(RoleEnum.Roles.Administrator) ||
                        userRoles.Contains(RoleEnum.Roles.ScrumMaster))
                        return true;
                    break;
                case RoleEnum.Roles.TeamLeader:
                    if (userRoles.Contains(RoleEnum.Roles.Administrator) ||
                        userRoles.Contains(RoleEnum.Roles.ScrumMaster) || userRoles.Contains(RoleEnum.Roles.TeamLeader))
                        return true;
                    break;
                case RoleEnum.Roles.TeamMember:
                    if (userRoles.Contains(RoleEnum.Roles.Administrator) ||
                        userRoles.Contains(RoleEnum.Roles.ScrumMaster) ||
                        userRoles.Contains(RoleEnum.Roles.TeamLeader) | userRoles.Contains(RoleEnum.Roles.TeamMember))
                        return true;
                    break;
                case RoleEnum.Roles.Stakeholder:
                    if (userRoles.Contains(RoleEnum.Roles.Administrator) ||
                        userRoles.Contains(RoleEnum.Roles.ScrumMaster) ||
                        userRoles.Contains(RoleEnum.Roles.TeamLeader) | userRoles.Contains(RoleEnum.Roles.TeamMember) ||
                        userRoles.Contains(RoleEnum.Roles.Stakeholder))
                        return true;
                    break;
                case RoleEnum.Roles.Viewonly:
                    if (userRoles.Contains(RoleEnum.Roles.Administrator) ||
                        userRoles.Contains(RoleEnum.Roles.ScrumMaster) ||
                        userRoles.Contains(RoleEnum.Roles.TeamLeader) | userRoles.Contains(RoleEnum.Roles.TeamMember) ||
                        userRoles.Contains(RoleEnum.Roles.Stakeholder) || userRoles.Contains(RoleEnum.Roles.Viewonly))
                        return true;
                    break;
            }
            return false;
        }

        public List<RoleEnum.Roles> GetRole(IPrincipal user)
        {
            string username = user.Identity.Name.ToLower();
            using (DataAccessLayer db = new DataAccessLayer())
            {
                bool explicitAuth = false;
                User requestedUser = db.GetUsers().FirstOrDefault(u => u.UserName.ToLower() == username) ?? AddNewUserFromAD(user);
                if (requestedUser.UserInRoles.Where(uir => uir.isExplicit).ToList().Count > 0)
                {
                    explicitAuth = true;
                }
                return !explicitAuth ? GetRoleImplicit(user, requestedUser) : GetRoleExplicit(requestedUser);
            }
        }

        private List<RoleEnum.Roles> GetRoleExplicit(User authUser)
        {
            List<RoleEnum.Roles> result = new List<RoleEnum.Roles>();
            using (DataAccessLayer db = new DataAccessLayer())
            {
                if (authUser == null)
                {
                    result.Add(RoleEnum.Roles.Unauthorized);
                    return result;
                }
                List<UserInRole> userRoles = authUser.UserInRoles.Where(uir => uir.isExplicit).ToList();
                foreach (UserInRole uir in userRoles)
                {
                    switch (uir.Role.RoleType)
                    {
                        case 0:
                            result.Add(RoleEnum.Roles.Administrator);
                            break;
                        case 1:
                            result.Add(RoleEnum.Roles.ScrumMaster);
                            break;
                        case 2:
                            result.Add(RoleEnum.Roles.TeamLeader);
                            break;
                        case 3:
                            result.Add(RoleEnum.Roles.TeamMember);
                            break;
                        case 4:
                            result.Add(RoleEnum.Roles.Stakeholder);
                            break;
                        case 5:
                            result.Add(RoleEnum.Roles.Viewonly);
                            break;
                        default:
                            result.Add(RoleEnum.Roles.Unauthorized);
                            break;
                    }
                }
            }
            return result;
        }

        private List<RoleEnum.Roles> GetRoleImplicit(IPrincipal user, User authUser)
        {
            List<RoleEnum.Roles> result = new List<RoleEnum.Roles>();
            PrincipalContext domainContext = new PrincipalContext(ContextType.Domain,
                System.Configuration.ConfigurationManager.AppSettings["ADDomainName"]);
            UserPrincipal requestedUserData = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName,
                user.Identity.Name);
            List<Principal> userGroups = requestedUserData.GetGroups().ToList();
            using (DataAccessLayer db = new DataAccessLayer())
            {
                foreach (Role r in db.GetRoles((int)_validRole))
                {
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(domainContext, r.ADGroupName);
                    if (group == null) break;
                    if (requestedUserData == null) break;
                    if (!requestedUserData.IsMemberOf(@group) && isRolePresentForUser(authUser.ID, r.ID))
                    {
                        db.DeleteUserToRoleMap(authUser.ID, r.ID);
                    }
                    if (userGroups.FirstOrDefault(g => g.Name == @group.Name) != null)
                    {
                        result.Add((RoleEnum.Roles)r.RoleType);
                        if (!isRolePresentForUser(authUser.ID, r.ID))
                        {
                            db.AddUserToRole(authUser.ID, r.ID, false);
                        }
                        return result;
                    }
                }
            }
            result.Add(RoleEnum.Roles.Unauthorized);
            return result;
        }

        private bool isRolePresentForUser(int userID, int roleID)
        {
            using (DataAccessLayer db = new DataAccessLayer())
            {
                return db.isUserInRole(userID, roleID);
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            string requestedURL = filterContext.RequestContext.HttpContext.Request.Path;
            filterContext.Controller.TempData["requestedURL"] = requestedURL;
            LogHelper.ErrorLog("Invalid Access to the following url from website " + requestedURL);
            filterContext.Result = new RedirectResult("/Unauthorized");
        }

        private User AddNewUserFromAD(IPrincipal user)
        {
            PrincipalContext domainContext = new PrincipalContext(ContextType.Domain,
                System.Configuration.ConfigurationManager.AppSettings["ADDomainName"]);
            UserPrincipal requestedUserData = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName,
                user.Identity.Name);
            User newUser = new User
            {
                FirstName = requestedUserData.GivenName,
                LastName = requestedUserData.Surname,
                UserName = user.Identity.Name,
                emailAddress = requestedUserData.EmailAddress ?? ""
            };
            using (DataAccessLayer db = new DataAccessLayer())
            {
                db.AddUser(newUser);
            }
            return newUser;
        }

        public User GetUserFromPriciple(IPrincipal user, DataAccessLayer db)
        {
            string username = user.Identity.Name.ToLower();
            User requestedUser = db.GetUsers().FirstOrDefault(u => u.UserName.ToLower() == username);
            return requestedUser ?? AddNewUserFromAD(user);
        }

        public string GetUserAndRoleFromPriciple(IPrincipal user)
        {
            using (DataAccessLayer db = new DataAccessLayer())
            {
                User requestedUser = GetUserFromPriciple(user, db);
                List<Role> userRoles;
                Role lowestRole;
                if (userHasExplicitRole(user))
                {
                    userRoles = db.GetRolesForUser(requestedUser.ID).Where(ur=>ur.UserInRoles.Any(uir=>uir.isExplicit)).ToList();
                    lowestRole = userRoles.First(ur => ur.RoleType == userRoles.Min(r => r.RoleType));
                }
                else
                {
                    userRoles = db.GetRolesForUser(requestedUser.ID);
                    lowestRole = userRoles.First(ur => ur.RoleType == userRoles.Min(r => r.RoleType));
                }
            string result = requestedUser.FirstName + " " + requestedUser.LastName + " (" + lowestRole.RoleName + ")";
                return result;
            }
        }

        private bool userHasExplicitRole(IPrincipal user)
        {
            using (DataAccessLayer db = new DataAccessLayer())
            {
                User u = GetUserFromPriciple(user, db);
                List<UserInRole> rolesforUser = u.UserInRoles.ToList();
                if (rolesforUser.Any(uir => uir.isExplicit))
                {
                    return true;
                }
            }
            return false;
        }
    }
}