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
    [ApiAuthenticate(RoleEnum.Roles.ScrumMaster)]
    [RoutePrefix("api/Logs")]
    public class LogController : ApiController
    {
        // GET api/<controller>
        [Route("")]
        public IEnumerable<LogDetailsDTO> Get()
        {
            try
            {
                using (DataAccessLayer model = new DataAccessLayer())
                {
                    return model.GetLogs().GetDetails<LogDetailsDTO>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Failed Getting Logs: " + ex.ToString());
                return null;
            }
        }

        // GET api/<controller>/5
        [Route("{id:int}")]
        public LogDetailsDTO Get(int id)
        {
            try
            {
                using (DataAccessLayer model = new DataAccessLayer())
                {
                    return (LogDetailsDTO)model.GetLog(id).GetDetails();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Failed to get Log with ID: " + id + " with Error: " + ex.ToString());
                return null;
            }
        }

    }
}