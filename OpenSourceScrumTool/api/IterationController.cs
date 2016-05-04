using System;
using System.Collections.Generic;
using System.Globalization;
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
    [RoutePrefix("api/Iterations")]
    public class IterationController : ApiController
    {
        // GET api/<controller>
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [Route("")]
        [HttpGet]
        public IEnumerable<IterationDTO> Get()
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    return modelAccess.GetIterations().ToDTO<IterationDTO>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error Getting All Iterations: " + ex.ToString());
                return null;
            }
        }

        [ApiAuthenticate(RoleEnum.Roles.Stakeholder)]
        [Route("GetProjectIteration/{projID:int}")]
        [HttpGet]
        public IEnumerable<IterationDTO> GetIterationsForProject(int projID)
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    return modelAccess.GetIterations().Where(i => i.ProjectID == projID).ToDTO<IterationDTO>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error Getting Iterations for Project with ID: " + projID);
                return null;
            }
        }

        // GET api/<controller>/5
        [ApiAuthenticate(RoleEnum.Roles.Stakeholder)]
        [Route("{id:int}")]
        [HttpGet]
        public IterationDTO Get(int id)
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    return (IterationDTO)modelAccess.GetIteration(id).ToDTO();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting single iteration with ID: " + id);
                return null;
            }
        }

        // POST api/<controller>
        [ApiAuthenticate(RoleEnum.Roles.TeamLeader)]
        [Route("")]
        [HttpPost]
        public int Post([FromBody]IterationDTO iteration)
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    LogHelper.InfoLog("Adding Iteration to Project " + iteration.ProjectID);
                    return modelAccess.AddIteration(new Iteration
                    {
                        ID = 0,
                        ProjectID = iteration.ProjectID,
                        SprintName = iteration.SprintName,
                        SprintStartDate = DateTime.ParseExact(iteration.SprintStartDate, "dd/MM/yyyy", null, DateTimeStyles.None),
                        SprintEndDate = DateTime.ParseExact(iteration.SprintEndDate, "dd/MM/yyyy", null, DateTimeStyles.None),
                        Archived = false
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("The Following Error occured when adding an iteration: " + ex.ToString());
                return 0;
            }
        }

        [ApiAuthenticate(RoleEnum.Roles.TeamMember)]
        [Route("AddPBItoIteration")]
        [HttpPost]
        public int? AddPBItoIteration([FromBody] SetIteration iterationSet)
        {
            try
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    LogHelper.InfoLog("Adding PBI with ID: " + iterationSet.PbiId + " ,To Iteration with ID: " + iterationSet.IterationId);
                    ProductBacklogItem pbi = modelAccess.GetProductBacklogItem(iterationSet.PbiId);
                    if (pbi == null) return null;
                    pbi.SprintID = iterationSet.IterationId ?? null;
                    modelAccess.UpdateProductBacklogItem(pbi.ID, pbi);
                    return 1;

                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("The following error occured when adding the pbi with id: " + iterationSet.PbiId + " to iteration with id " + iterationSet.IterationId + ": " + ex.ToString());
                return null;
            }
        }

        // PUT api/<controller>/5
        [Route("{id:int}")]
        [ApiAuthenticate(RoleEnum.Roles.TeamLeader)]
        [HttpPut]
        public int Put(int id, [FromBody]IterationDTO iteration)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.InfoLog("Updating iteration with id: " + iteration.IterationID);
                    modelAccess.UpdateIteration(id, new Iteration
                    {
                        ID = iteration.IterationID,
                        ProjectID = iteration.ProjectID,
                        SprintName = iteration.SprintName,
                        SprintStartDate = DateTime.Parse(iteration.SprintStartDate),
                        SprintEndDate = DateTime.Parse(iteration.SprintEndDate),
                        Archived = iteration.Archived
                    });
                    return 1;

                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error Updating Iteration: " + iteration.IterationID + " with error: " + ex.ToString());
                    return 0;
                }
            }
        }

        // DELETE api/<controller>/5
        [ApiAuthenticate(RoleEnum.Roles.TeamLeader)]
        [Route("{id:int}")]
        [HttpDelete]
        public string Delete(int id)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.WarnLog("Archiving Iteration with ID: " + id);
                    modelAccess.ArchiveIteration(id);
                    return "Complete";
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error Archiving iteration with id: " + id + " ,with error: " + ex.ToString());
                    return "Error";
                }
            }
        }

        [Route("GetCurrentIterationForProject/{projID:int}")]
        [HttpGet]
        [ApiAuthenticate(RoleEnum.Roles.TeamMember)]
        public IterationDetailsDTO GetCurrentIterationForProject(int projID)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    return (IterationDetailsDTO)modelAccess.GetCurrentIterationForProject(projID).GetDetails();
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("No Iteration for project with ID: " + projID);
                    return null;
                }

            }

        }
    }
}