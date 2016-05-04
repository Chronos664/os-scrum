using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Helpers;
using System.Web.Http;
using OpenSourceScrumTool.Account_Manager;
using OpenSourceScrumTool.DAL;
using OpenSourceScrumTool.Models;
using OpenSourceScrumTool.Utilities;

namespace OpenSourceScrumTool.api
{
    [RoutePrefix("api/ProductBacklog")]
    public class ProductBacklogController : ApiController
    {
        // GET api/<controller>
        [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
        [Route("")]
        [HttpGet]
        public IEnumerable<ProductBacklogItemDTO> Get()
        {
            try
            {
                using (DataAccessLayer model = new DataAccessLayer())
                {
                    return model.GetProductBacklogItems().ToDTO<ProductBacklogItemDTO>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting all product backlog items: " + ex.ToString());
                return null;
            }
        }

        //GET api/ProductBacklog Will get all BacklogItems for project
        [ApiAuthenticate(RoleEnum.Roles.Stakeholder)]
        [Route("Project/{ProjectID:int}")]
        [HttpGet]
        public IEnumerable<ProductBacklogItemDTO> GetBacklogItemsForProject(int ProjectID)
        {
            try
            {
                using (DataAccessLayer model = new DataAccessLayer())
                {
                    return
                        model.GetProductBacklogItems()
                            .Where(pbi => pbi.ProjectID == ProjectID)
                            .ToDTO<ProductBacklogItemDTO>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting product backlog items for Project with ID: " + ProjectID +
                                   " with Error: " + ex.ToString());
                return null;
            }
        }

        // GET api/<controller>/5
        [ApiAuthenticate(RoleEnum.Roles.Stakeholder)]
        [Route("{id:int}")]
        [HttpGet]
        public ProductBacklogItemDTO Get(int id)
        {
            try
            {
                using (DataAccessLayer model = new DataAccessLayer())
                {
                    return (ProductBacklogItemDTO)model.GetProductBacklogItem(id).ToDTO();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Error getting Product Backlog Item with ID: " + id + " with Error; " + ex.ToString());
                return null;
            }
        }

        // POST api/<controller>
        [ApiAuthenticate(RoleEnum.Roles.TeamMember)]
        [Route("")]
        [HttpPost]
        public int Post([FromBody] ProductBacklogItemDTO pbi)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.InfoLog("Attempting to Add new Product backlog item for Project: " + pbi.ProjectID);
                    return modelAccess.AddProductBacklogItem(new ProductBacklogItem
                    {
                        ProjectID = pbi.ProjectID,
                        Name = pbi.Name,
                        Description = pbi.Description,
                        Data = pbi.Data,
                        SprintID = pbi.SprintID,
                        EffortScore = pbi.EffortScore,
                        Priority = modelAccess.GetNextPBIPriority(pbi.ProjectID),
                        State = pbi.State,
                        Archived = false,
                        DateFinished = null
                    });
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error Adding new Product Backlog Item: " + ex.ToString());
                    return 0;
                }
            }
        }

        // PUT api/<controller>/5
        [ApiAuthenticate(RoleEnum.Roles.TeamMember)]
        [Route("{id:int}")]
        [HttpPut]
        public int Put(int id, [FromBody] ProductBacklogItemDTO pbi)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.InfoLog("Attempting to update Product Backlog Item with ID: " + pbi.ProductBacklogItemID);
                    modelAccess.UpdateProductBacklogItem(id, new ProductBacklogItem
                    {
                        ProjectID = 0,
                        Name = pbi.Name,
                        Description = pbi.Description,
                        Data = pbi.Data,
                        SprintID = pbi.SprintID,
                        EffortScore = pbi.EffortScore,
                        Priority = pbi.Priority,
                        State = pbi.State,
                        Archived = pbi.Archived,
                        DateFinished = pbi.State == 2 ? (DateTime?)DateTime.Today : null
                    });
                    return 1;
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error Updating Product Backlog Item: " + ex.ToString());
                    return 0;
                }
            }
        }

        // DELETE api/<controller>/5
        [ApiAuthenticate(RoleEnum.Roles.TeamMember)]
        [Route("{id:int}")]
        [HttpDelete]
        public string Delete(int id)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                try
                {
                    LogHelper.WarnLog("Attempting to Archive Product Backlog Item with ID: " + id);
                    modelAccess.ArchiveProductBacklogItem(id);
                    return "Complete";
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("Error Archiving Product Backlog Item: " + ex.ToString());
                    return "Error";
                }
            }
        }

        [ApiAuthenticate(RoleEnum.Roles.Stakeholder)]
        [Route("UpdatePriority")]
        [HttpPost]
        public string UpdatePriority([FromBody] ChangePBIPriority TableSnapshot)
        {
            try
            {
                using (DataAccessLayer model = new DataAccessLayer())
                {
                    LogHelper.InfoLog("Attempting to Update Priorities for Product Backlog Items");
                    for (int i = 0; i < TableSnapshot.pbiIDs.Count(); i++)
                    {
                        ProductBacklogItem pbi = model.GetProductBacklogItem(int.Parse(TableSnapshot.pbiIDs[i]));
                        if (pbi.Priority != i + 1)
                        {
                            pbi.Priority = i + 1;
                            model.UpdateProductBacklogItem(pbi.ID, pbi);
                        }
                    }
                }
                return "Complete";
            }
            catch (Exception ex)
            {
                string errorMessage = "Error updating backlog item priorities: " + ex.ToString();
                LogHelper.ErrorLog(errorMessage);
                return "Error";
            }
        }
    }
}