using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using OpenSourceScrumTool.Account_Manager;
using OpenSourceScrumTool.DAL;
using OpenSourceScrumTool.Models;
using OpenSourceScrumTool.Utilities;

namespace OpenSourceScrumTool.Controllers
{
    [UserAuthenticate(RoleEnum.Roles.ScrumMaster)]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserManagement()
        {
            using (DataAccessLayer model = new DataAccessLayer())
            {
                return PartialView("_UserManagement", model.GetUsers().ToList().GetDetails<UserDetailsDTO>());
            }
        }

        public ActionResult TeamManagement()
        {
            using (DataAccessLayer model = new DataAccessLayer())
            {
                return PartialView("_TeamManagement", model.GetTeams().ToList().GetDetails<TeamDetailsDTO>());
            }
        }

        public ActionResult LogViewer(string sortOrder, string searchString)
        {
            using (DataAccessLayer model = new DataAccessLayer())
            {
                List <Log> logs = model.GetLogs();
                ViewBag.ErrorLevelSort = String.IsNullOrEmpty(sortOrder) ? "error_desc" : "";
                ViewBag.DateSortParam = sortOrder == "Date" ? "date_desc" : "Date";
                if (!String.IsNullOrEmpty(searchString))
                {
                    logs = logs.Where(l => l.LogMessage.ToLower().Contains(searchString.ToLower())).ToList();
                }
                switch (sortOrder)
                {
                    case "Date" :
                        logs = logs.OrderBy(l => l.Time).ToList();
                        break;
                    case "date_desc":
                        logs = logs.OrderByDescending(l => l.Time).ToList();
                        break;
                    case "error_desc":
                        logs = logs.OrderBy(l => l.LogLevel).ToList();
                        break;
                    default:
                        logs = logs.OrderByDescending(l => l.LogLevel).ToList();
                        break;
                }
                return PartialView("_LogViewer", logs.GetDetails<LogDetailsDTO>().ToList());
            }
        }
    }
}