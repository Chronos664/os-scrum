using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenSourceScrumTool.DAL;
using OpenSourceScrumTool.Models;

namespace OpenSourceScrumTool.Account_Manager
{
    public class TeamAuthenticate : AuthorizeAttribute
    {
        private RoleEnum.Roles _validRole;
        private Project _projectToValidate;
        private UserAuthenticate _authenticate = new UserAuthenticate();

        public TeamAuthenticate(Project projectToValidate)
        {
            _projectToValidate = projectToValidate;
        }

        public TeamAuthenticate(RoleEnum.Roles validRole, Project projectToValidate)
        {
            _validRole = validRole;
            _projectToValidate = projectToValidate;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (_authenticate.CheckAuth(httpContext.User, RoleEnum.Roles.ScrumMaster))
            {
                return true;
            }
            if (_authenticate.CheckAuth(httpContext.User, _validRole))
            {
                using (DataAccessLayer db = new DataAccessLayer())
                {
                    List<Team> teamsOnProject = db.GetTeamsOnProject(_projectToValidate.ID);
                    User userToValidate = _authenticate.GetUserFromPriciple(httpContext.User, db);
                    if (teamsOnProject.Any(t => t.Users.Any(u => u.ID == userToValidate.ID)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool isAuthenticated(HttpContextBase context)
        {
            return AuthorizeCore(context);
        }
    }
}