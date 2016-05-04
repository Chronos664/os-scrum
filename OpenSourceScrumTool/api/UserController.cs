using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Http;
using Microsoft.Ajax.Utilities;
using OpenSourceScrumTool.Account_Manager;
using OpenSourceScrumTool.DAL;
using OpenSourceScrumTool.Models;
using OpenSourceScrumTool.Utilities;

namespace OpenSourceScrumTool.api
{
    [RoutePrefix("api/Users")]
    public class UserController : ApiController
    {
        // GET api/<controller>
        [ApiAuthenticate(RoleEnum.Roles.Viewonly)]
        [Route("")]
        [HttpGet]
        public IEnumerable<UserDTO> Get()
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    return modelAccess.GetUsers().ToDTO<UserDTO>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting users: " + ex.ToString());
                return null;
            }
        }

        // GET api/<controller>/5
        [ApiAuthenticate(RoleEnum.Roles.Viewonly)]
        [Route("{id:int}")]
        [HttpGet]
        public UserDTO Get(int id)
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    return (UserDTO)modelAccess.GetUser(id).ToDTO();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error gettting user with ID: " + id + ".Error: " + ex.ToString());
                return null;
            }
        }

        [ApiAuthenticate(RoleEnum.Roles.Viewonly)]
        [Route("AvailableTeams/{userID:int}")]
        [HttpGet]
        public IEnumerable<TeamDTO> AvailableTeams(int userID)
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    User u = modelAccess.GetUser(userID);
                    if (u == null)
                    {
                        LogHelper.InfoLog("No User with ID: " + userID + " present in the System, cannot get available teams.");
                        return null;
                    }
                    return modelAccess.GetTeams().Where(t => !t.Users.Contains(u) && !t.Archived).ToList().ToDTO<TeamDTO>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting available teams for user id: " + userID + " with Error: " + ex.ToString());
                return null;
            }
        }

        [ApiAuthenticate(RoleEnum.Roles.Viewonly)]
        [Route("UserDetails/{userID:int}")]
        [HttpGet]
        public UserDetailsDTO UserDetails(int userID)
        {
            using (DataAccessLayer db = new DataAccessLayer())
            {
                return db.GetUser(userID).GetDetails() as UserDetailsDTO;
            }
        }

        // POST api/<controller>
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [HttpPost]
        [Route("")]
        public int Post([FromBody]UserDTO user)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.InfoLog("Adding New User with Username: " + user.UserName);
                    return modelAccess.AddUser(new User
                    {
                        ID = 0,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        emailAddress = user.EmailAddress,
                        UserName = user.UserName
                    });
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error adding new user. Error: " + ex.ToString());
                    return 0;
                }
            }
        }

        // POST api/Users/AddRoleToUser
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [HttpPost]
        [Route("AddRoleToUser")]
        public int AddRoleToUser([FromBody] UserToRole usrtoRoleMap)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.WarnLog("Attempting to add Role with ID: " + usrtoRoleMap.roleID + " to user with ID: " + usrtoRoleMap.userID);
                    modelAccess.AddUserToRole(usrtoRoleMap.userID, usrtoRoleMap.roleID, true);
                    return 1;
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error Attempting to add Role with ID: " + usrtoRoleMap.roleID + " to user with ID: " + usrtoRoleMap.userID + " with Error: " + ex.ToString());
                    return 0;
                }
            }
        }

        // POST api/Users/RemoveRoleFromUser
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [HttpPost]
        [Route("RemoveRoleFromUser")]
        public int RemoveRoleFromUser([FromBody] UserToRole usrtoRoleMap)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.WarnLog("Attempting to remove Role with ID: " + usrtoRoleMap.roleID + " from User with ID: " + usrtoRoleMap.userID);
                    modelAccess.DeleteUserFromRole(usrtoRoleMap.userID, usrtoRoleMap.roleID);
                    return 1;
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error Removing Role from User: " + ex.ToString());
                    return 0; 
                }
            }
        }
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [HttpPost]
        [Route("AddUserToTeam")]
        public int AddUserToTeam([FromBody] UserToTeam usrToTeamMap)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.InfoLog("Attempting to add User with ID: " + usrToTeamMap.userID + " to Team with ID: " + usrToTeamMap.teamID);
                    modelAccess.AddUserToTeam(usrToTeamMap.userID, usrToTeamMap.teamID);
                    return 1;
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error adding user to team: " + ex.ToString());
                    return 0;
                }
            }
        }
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [HttpPost]
        [Route("RemoveUserFromTeam")]
        public int RemoveUserFromTeam([FromBody] UserToTeam usrToTeamMap)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.WarnLog("Attempting to remove User with ID: " + usrToTeamMap.userID + " from Team with ID: " + usrToTeamMap.teamID);
                    modelAccess.RemoveUserFromTeam(usrToTeamMap.userID, usrToTeamMap.teamID);
                    return 1;
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error removing user from team: " + ex.ToString());
                    return 0;
                }
            }
        }

        // PUT api/<controller>/5
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [HttpPut]
        [Route("{id:int}")]
        public int Put(int id, [FromBody]UserDTO user)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.InfoLog("Attempting to update User information for User ID: " + user.UserID);
                    modelAccess.UpdateUser(id, new User
                    {
                        ID = 0,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        emailAddress = user.EmailAddress,
                        UserName = user.UserName
                    });
                    return 1;
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error Updating User Information: " + ex.ToString());
                    return 0;
                }
            }
        }

        // DELETE api/Users/BlockUser/5
        [ApiAuthenticate(RoleEnum.Roles.Administrator)]
        [Route("BlockUser/{id:int}")]
        [HttpDelete]
        public string BlockUser(int id)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.WarnLog("Attempting to Block User with ID: " + id);
                    modelAccess.BlockUser(id);
                    return "Complete";
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error Blocking User: " + ex.ToString());
                    return "Error";
                }
            }
        }
        // POST api/Users/UnblockUser/id
        [ApiAuthenticate(RoleEnum.Roles.Administrator)]
        [Route("UnblockUser/{id:int}")]
        [HttpPost]
        public string UnblockUser(int id)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.WarnLog("Attempting to Unblock User with ID: " + id);
                    modelAccess.UnblockUser(id);
                    return "Complete";
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error Unblocking User: " + ex.ToString());
                    return "Error";
                }
            }
        }
    }
}