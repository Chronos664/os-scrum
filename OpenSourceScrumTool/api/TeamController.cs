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
    [RoutePrefix("api/Teams")]
    public class TeamController : ApiController
    {
        // GET api/<controller>
        [ApiAuthenticate(RoleEnum.Roles.Viewonly)]
        [Route("")]
        [HttpGet]
        public IEnumerable<TeamDTO> Get()
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    return modelAccess.GetTeams().ToDTO<TeamDTO>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error Getting Teams: " + ex.ToString());
                return null;
            }
        }

        // GET api/<controller>/5
        [ApiAuthenticate(RoleEnum.Roles.Viewonly)]
        [Route("{id:int}")]
        [HttpGet]
        public TeamDTO Get(int id)
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    return (TeamDTO)modelAccess.GetTeam(id).ToDTO();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting team with id: " + id + " with error: " + ex.ToString());
                return null;
            }
        }

        [ApiAuthenticate(RoleEnum.Roles.Viewonly)]
        [Route("Details/{id:int}")]
        [HttpGet]
        public TeamDetailsDTO GetDetails(int id)
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    return (TeamDetailsDTO)modelAccess.GetTeam(id).GetDetails();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting team with id: " + id + " with error: " + ex.ToString());
                return null;
            }
        }
        // POST api/<controller>
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [Route("")]
        [HttpPost]
        public int Post([FromBody]TeamDTO team)
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    LogHelper.InfoLog("Attempting to add new Team with name: " + team.TeamName);
                    return modelAccess.AddTeam(new Team
                    {
                        ID = 0,
                        TeamName = team.TeamName,
                        Archived = false
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error Adding new team: " + ex.ToString());
                return 0;
            }
        }

        // PUT api/<controller>/5
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [Route("{id:int}")]
        [HttpPut]
        public int Put(int id, [FromBody]TeamDTO team)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    modelAccess.UpdateTeam(id, new Team
                    {
                        ID = 0,
                        TeamName = team.TeamName,
                        Archived = team.Archived
                    });
                    return 1;

                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error updating team with ID: " + team.TeamID + " with error: " + ex.ToString());
                    return 0;
                }
            }
        }

        // DELETE api/<controller>/5
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [Route("ArchiveTeam/{id:int}")]
        [HttpDelete]
        public string ArchiveTeam(int id)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.WarnLog("Archiving Team: " + id);
                    modelAccess.ArchiveTeam(id);
                    return "Complete";
                }
                catch (Exception ex)
                {
                    LogHelper.WarnLog("Error Archiving Team: " + ex.ToString());
                    return "Error";
                }
            }
        }

        // POST api/<controller>/5
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [Route("RestoreTeam/{id:int}")]
        [HttpPost]
        public string RestoreTeam(int id)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.WarnLog("Reinstating Team: " + id);
                    modelAccess.RestoreTeam(id);
                    return "Complete";
                }
                catch (Exception ex)
                {
                    LogHelper.WarnLog("Error Reinstating Team: " + ex.ToString());
                    return "Error";
                }
            }
        }

        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [Route("TeamToProject")]
        [HttpPost]
        public string AddTeamToProject([FromBody]TeamToProject teamToProject)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.InfoLog("Adding Team: " + teamToProject.teamID + " To Project: " + teamToProject.ProjectID);
                    modelAccess.SetTeamToProjectMap(teamToProject, TypeOfDBAction.Add);
                    return "Ok";
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error Adding team: " + teamToProject.teamID + " to project: " + teamToProject.ProjectID + " with error: " + ex.ToString());
                    return "Error";
                }
            }

        }

        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [Route("TeamToProject")]
        [HttpDelete]
        public string RemoveTeamFromProject([FromBody]TeamToProject teamToProject)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.WarnLog("Removing Team: " + teamToProject.teamID + " From Project: " + teamToProject.ProjectID);
                    modelAccess.SetTeamToProjectMap(teamToProject, TypeOfDBAction.Delete);
                    return "Ok";
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error Removing Team: " + teamToProject.teamID + " from Project: " + teamToProject.ProjectID);
                    return "Error";
                }
            }
        }


    }
}