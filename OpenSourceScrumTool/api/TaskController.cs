using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    [RoutePrefix("api/BacklogTasks")]

    public class TaskController : ApiController
    {
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [Route("")]
        [HttpGet]
        // GET api/<controller>
        public IEnumerable<BacklogItemTaskDTO> Get()
        {
            try
            {
                using (DataAccessLayer model = new DataAccessLayer())
                {
                    return model.GetTasks().ToDTO<BacklogItemTaskDTO>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error Getting Tasks: " + ex.ToString());
                return null;
            }
        }

        // GET api/<controller> for gettings tasks within a set PBI
        [ApiAuthenticate(RoleEnum.Roles.TeamMember)]
        [Route("BacklogItem/{BacklogItemID:int}")]
        [HttpGet]
        public IEnumerable<BacklogItemTaskDTO> GetTasksForBacklogItem(int BacklogItemID)
        {
            try
            {
                using (DataAccessLayer model = new DataAccessLayer())
                {
                    return model.GetTasks().Where(t => t.ProductBacklogID == BacklogItemID).ToDTO<BacklogItemTaskDTO>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting tasks for Backlog Item with ID: " + BacklogItemID + " With error: " + ex.ToString());
                return null;
            }
        }

        [ApiAuthenticate(RoleEnum.Roles.TeamMember)]
        [Route("{id:int}")]
        [HttpGet]
        // GET api/<controller>/5
        public BacklogItemTaskDTO Get(int id)
        {
            try
            {
                using (DataAccessLayer model = new DataAccessLayer())
                {
                    return (BacklogItemTaskDTO)model.GetTask(id).ToDTO();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting Backlog Item with ID: " + id + " With error: " + ex.ToString());
                return null;
            }
        }

        [ApiAuthenticate(RoleEnum.Roles.TeamMember)]
        [Route("")]
        [HttpPost]
        // POST api/<controller>
        public int Post([FromBody]BacklogItemTaskDTO taskDto)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.InfoLog("Attempting to add Task for Backlog Item ID: " + taskDto.ProductBacklogID);
                    return modelAccess.AddTask(new BacklogItemTask
                    {
                        ID = 0,
                        CurrentUserID = taskDto.CurrentUserID == 0 ? null : taskDto.CurrentUserID,
                        Description = taskDto.Description,
                        Name = taskDto.Name,
                        ProductBacklogID = taskDto.ProductBacklogID,
                        RemainingTime = taskDto.RemainingTime,
                        State = taskDto.State,
                        Archived = false
                    });
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error Adding Task: " + ex.ToString());
                    return 0;
                }
            }
        }

        [ApiAuthenticate(RoleEnum.Roles.TeamMember)]
        [Route("{id:int}")]
        [HttpPut]
        // PUT api/<controller>/5
        public int Put(int id, [FromBody]BacklogItemTaskDTO taskDto)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.InfoLog("Attempting to update information for task ID: " + taskDto.BacklogItemTaskID);
                    modelAccess.UpdateTask(id, new BacklogItemTask
                    {
                        ID = taskDto.BacklogItemTaskID,
                        CurrentUserID = taskDto.CurrentUserID == 0 ? null : taskDto.CurrentUserID,
                        Description = taskDto.Description,
                        Name = taskDto.Name,
                        ProductBacklogID = taskDto.ProductBacklogID,
                        RemainingTime = taskDto.RemainingTime,
                        State = taskDto.State,
                        Archived = taskDto.Archived
                    });
                    return 1;
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error updating information for task: " + ex.ToString());
                    return 0;
                }
            }
        }

        [ApiAuthenticate(RoleEnum.Roles.TeamMember)]
        [Route("{id:int}")]
        [HttpDelete]
        // DELETE api/<controller>/5
        public string Delete(int id)
        {
            using (DataAccessLayer model = new DataAccessLayer())
            {
                try
                {
                    LogHelper.WarnLog("Attempting to Archive Task with ID: " + id);
                    model.ArchiveTask(id);
                    return "Complete";
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error Deleting Task: " + ex.ToString());
                    return "Error";
                }
            }
        }
    }
}