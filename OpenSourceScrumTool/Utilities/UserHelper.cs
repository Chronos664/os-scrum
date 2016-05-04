using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using OpenSourceScrumTool.Account_Manager;
using OpenSourceScrumTool.DAL;
using OpenSourceScrumTool.Models;

namespace OpenSourceScrumTool.Utilities
{
    public static class UserHelper
    {
        public static bool isUserInRole(IPrincipal user, RoleEnum.Roles role)
        {
            UserAuthenticate check = new UserAuthenticate();
            return check.CheckAuth(user, role);
        }

        public static string GetUsernameAndRoles(IPrincipal user)
        {
            UserAuthenticate userAuth = new UserAuthenticate(RoleEnum.Roles.Viewonly);
            return userAuth.GetUserAndRoleFromPriciple(user);
        }

        public static string GetFullNameFromUser(int id)
        {
            using (DataAccessLayer model = new DataAccessLayer())
            {
                User user = model.GetUser(id);
                if (user == null) return null;
                string username = user.FirstName + " " + user.LastName;
                return username;
            }
        }

        public static string GetUserName(IPrincipal user)
        {
            return user.Identity.Name;
        }

        public static User GetUser(IPrincipal user, DataAccessLayer model)
        {
            UserAuthenticate helper = new UserAuthenticate();
            return helper.GetUserFromPriciple(user, model);
        }

        public static bool IsUserUnauthorized(IPrincipal user)
        {
            using (DataAccessLayer model = new DataAccessLayer())
            {
                User current = GetUser(user, model);
                return current.UserInRoles.Any(uir => uir.Role.RoleType == (int) RoleEnum.Roles.Unauthorized);
            }
        }

        public static int GetUserID(IPrincipal user)
        {
            using (DataAccessLayer model = new DataAccessLayer())
            {
                return GetUser(user, model).ID;
            }
        }
    }
}
