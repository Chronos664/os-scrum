using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OpenSourceScrumTool.DAL;
using OpenSourceScrumTool.Models;
using System.Data.Entity;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using OpenSourceScrumTool.Account_Manager;
using OpenSourceScrumTool.Controllers;
using OpenSourceScrumTool.Utilities;
using WebGrease.Css.Extensions;

namespace OpenSourceScrumTool.api
{
    [RoutePrefix("api/Project")]
    public class ProjectController : ApiController
    {
        // GET api/<controller>
        [ApiAuthenticate(RoleEnum.Roles.Stakeholder)]
        [Route("")]
        [HttpGet]
        public IEnumerable<ProjectDTO> Get()
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    return modelAccess.GetProjects().ToDTO<ProjectDTO>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting Projects: " + ex.ToString());
                return null;
            }
        }

        // GET api/<controller>/5
        [ApiAuthenticate(RoleEnum.Roles.Stakeholder)]
        [Route("{id:int}")]
        [HttpGet]
        public ProjectDTO Get(int id)
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    return (ProjectDTO)modelAccess.GetProject(id).ToDTO();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting project with ID: " + id + " with error: " + ex.ToString());
                return null;
            }
        }
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [Route("GetAllProjectDetails")]
        [HttpGet]
        public IEnumerable<ProjectDetailsDTO> GetProjectsWithDetails()
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    return modelAccess.GetAllProjectDetails();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting all project details: " + ex.ToString());
                return null;
            }
        }

        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [Route("GetProjectDetails/{ProjectID:int}")]
        [HttpGet]
        public ProjectDetailsDTO GetProjectDetails(int ProjectID)
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    return modelAccess.GetProjectDetails(ProjectID);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting individual project details for project: " + ProjectID +
                                   " With error: " + ex.ToString());
                return null;
            }
        }

        [Route("GetUsersOnProject/{ProjectID:int}")]
        [HttpGet]
        [ApiAuthenticate(RoleEnum.Roles.TeamMember)]
        public IEnumerable<UserDTO> GetUsersOnProject(int ProjectID)
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    return modelAccess.GetUsersOnProject(ProjectID).ToDTO<UserDTO>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error Getting Users for Project with ID: " + ProjectID + " With error: " +
                                   ex.ToString());
                return null;
            }
        }

        [Route("GetAvailableProjectsForUser")]
        [HttpGet]
        [ApiAuthenticate(RoleEnum.Roles.Stakeholder)]
        public IEnumerable<ProjectDTO> GetAvailableProjectsForUser()
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    if (UserHelper.isUserInRole(HttpContext.Current.User, RoleEnum.Roles.ScrumMaster))
                    {
                        return modelAccess.GetProjects().Where(p => p.Archived == false).ToDTO<ProjectDTO>();
                    }
                    else
                    {
                        int userID = UserHelper.GetUserID(HttpContext.Current.User);
                        return modelAccess.GetProjectsForUser(userID).Where(p => p.Archived == false).ToDTO<ProjectDTO>();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Unable to get available projects for current user with Error: " + ex.ToString());
                return null;
            }
        }


        [Route("GetAvailableProjectsForTeam/{teamID:int}")]
        [HttpGet]
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        public IEnumerable<ProjectDTO> AvailableProjectsForTeam(int teamID)
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    Team t = modelAccess.GetTeam(teamID);
                    if (t == null) return null;
                    List<Project> availableProjects =
                        modelAccess.GetProjects().Where(p => !p.Teams.Contains(t) && !p.Archived).ToList();
                    return availableProjects.ToDTO<ProjectDTO>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting available projects for Team with ID: " + teamID + " With error: " + ex.ToString());
                return null;
            }
        }

        // POST api/<controller>
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [HttpPost]
        [Route("")]
        public int Post([FromBody] ProjectDTO project)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.InfoLog("Attempting to add new Project: " + project.ProjectName);
                    return modelAccess.AddProject(new Project
                    {
                        ID = 0,
                        ProjectName = project.ProjectName,
                        ProjectDetails = project.ProjectDetails,
                        Archived = false
                    });
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error Adding Project: " + ex.ToString());
                    return 0;
                }
            }
        }

        // PUT api/<controller>/5
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [Route("{id:int}")]
        [HttpPut]
        public int Put(int id, [FromBody]ProjectDTO project)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.InfoLog("Attempting to update Project with ID: " + project.ProjectId);
                    modelAccess.UpdateProject(id, new Project
                    {
                        ProjectName = project.ProjectName,
                        ProjectDetails = project.ProjectDetails,
                        Archived = project.Archived
                    });
                    return 1;
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error Updating Project: " + ex.ToString());
                    return 0;
                }
            }

        }

        // DELETE api/<controller>/5
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [Route("{id:int}")]
        [HttpDelete]
        public string Archive(int id)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.WarnLog("Attempting to Archive Project with ID: " + id);
                    modelAccess.ArchiveProject(id);
                    return "Complete";
                }
                catch (Exception ex)
                {
                    string errorMessage = "Error Archiving Project: " + ex.ToString();
                    LogHelper.ErrorLog(errorMessage);
                    return "Error";
                }
            }
        }

        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [Route("Restore/{id:int}")]
        [HttpPost]
        public string RestoreProject(int id)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.WarnLog("Attempting to Restore Project with ID: " + id);
                    modelAccess.RestoreProject(id);
                    return "Complete";
                }
                catch (Exception ex)
                {
                    string errorMessage = "Error Restoring Project: " + ex.ToString();
                    LogHelper.ErrorLog(errorMessage);
                    return "Error";
                }
            }
        }

        [ApiAuthenticate(RoleEnum.Roles.Stakeholder)]
        [Route("Report/{projectID:int}")]
        [HttpGet]
        public Report GetReport(int projectID)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                Project p = modelAccess.GetProject(projectID);
                if (p == null) return null;
                Report report = new Report();
                report.ProjectName = p.ProjectName;
                report.sprintNames.Add("Start");
                report.sprintNames.AddRange(p.Iterations.Select(i => i.SprintName).ToList());
                report.TotalEffort = p.ProductBacklogItems.Sum(pbi => pbi.EffortScore ?? 0);
                report.projectBurndownData.Add(report.TotalEffort);
                report.sprintVelocities.Add(0);
                foreach (Iteration i in p.Iterations)
                {
                    List<ProductBacklogItem> pbis =
                        i.ProductBacklogItems.Where(pbi => pbi.DateFinished.HasValue)
                            .Where(
                                result =>
                                    result.DateFinished.Value > i.SprintStartDate &&
                                    result.DateFinished.Value < i.SprintEndDate)
                            .ToList();
                    int sum = pbis.Where(item => item.EffortScore.HasValue).Sum(pbi => pbi.EffortScore.Value);
                    report.projectBurndownData.Add((report.projectBurndownData.Last() - sum));
                    report.sprintVelocities.Add(sum);
                    report.TotalBacklogItemsFinished += pbis.Count;
                    foreach (ProductBacklogItem pbi in pbis)
                    {
                        foreach (BacklogItemTask t in pbi.Tasks)
                        {
                            if (t.CurrentUserID.HasValue)
                            {
                                User u = modelAccess.GetUser(t.CurrentUserID.Value);
                                if (report.TotalTasksFinishedByUser.Count(item => item.UserName == u.UserName) == 1)
                                {
                                    UserTaskCount userTaskCount =
                                        report.TotalTasksFinishedByUser.First(utc => utc.UserName == u.UserName);
                                    userTaskCount.TaskCount++;
                                }
                                else
                                {
                                    report.TotalTasksFinishedByUser.Add(new UserTaskCount()
                                    {
                                        TaskCount = 1,
                                        UserName = u.UserName
                                    });
                                }
                            }
                        }
                    }
                }
                int sumOfVelocities = report.sprintVelocities.Where(vel => vel != 0).Sum();
                int countOfVelocities = report.sprintVelocities.Count(vel => vel != 0);
                if (countOfVelocities == 0 || sumOfVelocities == 0)
                {
                    report.CurrentAverageVelocity = 0;
                }
                else
                {
                    report.CurrentAverageVelocity = sumOfVelocities / countOfVelocities;
                }
                report.VelocityChart = ChartHelper.BarChart(
                    ChartHelper.ClearBlue("rgba"),
                    ChartHelper.Black("rgba"),
                    ChartHelper.ClearBlue("rgba"),
                    ChartHelper.ClearWhite("rgba"),
                    report.sprintNames,
                    "Project Velocities",
                    report.sprintVelocities
                    );
                report.BurndownChart = ChartHelper.LineGraph(
                    ChartHelper.ClearBlue("rgba"),
                    ChartHelper.Black("rgba"),
                    ChartHelper.ClearWhite("rgba"),
                    ChartHelper.Black("hex"),
                    ChartHelper.DarkGreen("hex"),
                    ChartHelper.Black("rgba"),
                    report.sprintNames,
                    "Burndown Chart",
                    report.projectBurndownData
                    );
                return report;
            }
        }
    }
}