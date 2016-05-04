using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenSourceScrumTool.Account_Manager;
using OpenSourceScrumTool.DAL;
using OpenSourceScrumTool.Models;
using OpenSourceScrumTool.Utilities;
using WebGrease.Css.Extensions;

namespace OpenSourceScrumTool.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        [UserAuthenticate(RoleEnum.Roles.Stakeholder)]
        public ActionResult Index()
        {
            if (TempData["errorState"] != null && TempData["errorState"].ToString() == "404")
            {
                ViewBag.Error = "Unable to Find Project";
            }
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                if (UserHelper.isUserInRole(User, RoleEnum.Roles.ScrumMaster))
                {
                    return View(modelAccess.GetProjects().ToDTO<ProjectDTO>());
                }
                IEnumerable<ProjectDTO> result =
                    modelAccess.GetProjectsForUser(UserHelper.GetUserID(User)).ToDTO<ProjectDTO>();
                return View(result);
            }
        }
    }
}