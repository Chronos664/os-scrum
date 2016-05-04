using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OpenSourceScrumTool.Account_Manager;
using OpenSourceScrumTool.DAL;
using OpenSourceScrumTool.Models;
using OpenSourceScrumTool.Utilities;

namespace OpenSourceScrumTool.api
{
    [RoutePrefix("api/Roles")]
    public class RoleController : ApiController
    {
        // GET api/<controller>
        [ApiAuthenticate(RoleEnum.Roles.Viewonly)]
        [Route("")]
        [HttpGet]
        public IEnumerable<RoleDTO> Get()
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    return modelAccess.GetRoles().ToDTO<RoleDTO>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting Roles: " + ex.ToString());
                return null;
            }
        }

        // GET api/<controller>/5
        [ApiAuthenticate(RoleEnum.Roles.Viewonly)]
        [Route("{id:int}")]
        [HttpGet]
        public RoleDTO Get(int id)
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    return (RoleDTO)modelAccess.GetRole(id).ToDTO();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting Role with ID: " + id + " with Error: " + ex.ToString());
                return null;
            }
        }

        [ApiAuthenticate(RoleEnum.Roles.Viewonly)]
        [Route("Details/{id:int}")]
        [HttpGet]
        public RoleDetailsDTO GetDetails(int id)
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    return (RoleDetailsDTO)modelAccess.GetRole(id).GetDetails();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting Role with ID: " + id + " with Error: " + ex.ToString());
                return null;
            }
        }

        // POST api/<controller>
        [ApiAuthenticate(RoleEnum.Roles.Administrator)]
        [Route("")]
        [HttpPost]
        public int Post([FromBody]RoleDTO role)
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    return modelAccess.AddRole(new Role
                    {
                        ID = 0,
                        RoleName = role.RoleName,
                        ADGroupName = role.AdGroupName,
                        RoleType = role.RoleType,
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error Adding Role: " + ex.ToString());
                return 0;
            }
        }

        // PUT api/<controller>/5
        [ApiAuthenticate(RoleEnum.Roles.Administrator)]
        [Route("{id:int}")]
        [HttpPut]
        public int Put(int id, [FromBody]RoleDTO role)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.WarnLog("Attempting to Update Role with ID: " + role.RoleID);
                    modelAccess.UpdateRole(id, new Role
                    {
                        ID = role.RoleID,
                        RoleName = role.RoleName,
                        ADGroupName = role.AdGroupName,
                    });
                    return 1;

                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error updating Role: " + ex.ToString());
                    return 0;
                }
            }
        }

        // DELETE api/<controller>/5
        [ApiAuthenticate(RoleEnum.Roles.Administrator)]
        [Route("{id:int}")]
        [HttpDelete]
        public string Delete(int id)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.WarnLog("Attempting to Delete Role with ID: " + id);
                    modelAccess.DeleteRole(id);
                    return "Complete";
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error Deleting Role: " + ex.ToString());
                    return "Error";
                }
            }
        }

        [ApiAuthenticate(RoleEnum.Roles.Viewonly)]
        [Route("GetRolesForUser/{userid:int}")]
        [HttpGet]
        public List<RoleDTO> AvailableRolesForUser(int userid)
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    User currentUser = modelAccess.GetUser(userid);
                    int minRole = currentUser.UserInRoles.Select(uir => uir).Min(r => r.Role.RoleType);
                    List<RoleDTO> availableRoles = modelAccess.GetRoles().Where(r => r.RoleType >= minRole && r.RoleType != (int)RoleEnum.Roles.Unauthorized).ToDTO<RoleDTO>().ToList();
                    return availableRoles;
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting Available Roles for User with ID: " + userid + " with Error: " + ex.ToString());
                return null;
            }
        }
    }
}