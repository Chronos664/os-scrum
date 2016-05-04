using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenSourceScrumTool.Account_Manager;
using OpenSourceScrumTool.DAL;
using OpenSourceScrumTool.Models;

namespace OpenSourceScrumTool.Controllers
{
    public class ProjectController : Controller
    {
        private int errorState;
        // GET: Project
        [UserAuthenticate(RoleEnum.Roles.Viewonly)]
        public ActionResult Index()
        {
            using (DataAccessLayer db = new DataAccessLayer())
            {
                if (TempData["errorState"] != null)
                {
                    int value = (int)TempData["errorState"];
                    switch (value)
                    {
                        case 401:
                            ViewBag.Error =
                                "You are not a team member working on this project or a Scrum Master or higher, therefore cannot access the requested project. If you believe this to be in error, please contact your administrator.";
                            break;
                        case 500:
                            ViewBag.Error = "An Internal Error has Ocured, Please contact your Administrator.";
                            break;
                        case 404:
                            if (TempData["404-Error"] != null)
                            {
                                ViewBag.Error = TempData["404-Error"];
                                TempData.Remove("404-Error");
                            }
                            break;
                    }
                    TempData.Remove("errorState");
                }
                return View(db.GetAllProjectDetails().Where(p=>!p.Archived));
            }
        }
        [UserAuthenticate(RoleEnum.Roles.Viewonly)]
        public ActionResult ProjectDetails(int? id)
        {
            if (id != null)
            {
                using (DataAccessLayer db = new DataAccessLayer())
                {
                    Project p = db.GetProject(id.Value);
                    if (p == null)
                    {
                        TempData["errorState"] = 404;
                        TempData["404-Error"] = "Unable to find Project with ID: " + id;
                        return RedirectToAction("Index");
                    }
                    TeamAuthenticate teamCheck = new TeamAuthenticate(RoleEnum.Roles.Viewonly, p);
                    if (teamCheck.isAuthenticated(HttpContext))
                    {
                        return View(p.GetDetails());
                    }
                }
                TempData["errorState"] = 401;
                return RedirectToAction("Index");
            }
            TempData["errorState"] = 500;
            return RedirectToAction("Index");
        }

        [UserAuthenticate(RoleEnum.Roles.Viewonly)]
        public ActionResult BacklogItemDetails(int? id)
        {
            if (id != null)
            {
                using (DataAccessLayer db = new DataAccessLayer())
                {                   
                    ProductBacklogItem pbi = db.GetProductBacklogItem(id.Value);
                    if (pbi == null)
                    {
                        TempData["errorState"] = 404;
                        TempData["404-Error"] = "Unable to find Backlog Item with ID: " + id;
                        return RedirectToAction("Index");
                    }
                    TeamAuthenticate teamCheck = new TeamAuthenticate(RoleEnum.Roles.Viewonly, pbi.Project);
                    if (teamCheck.isAuthenticated(HttpContext))
                    {
                        ViewBag.ProjectID = pbi.Project.ID;
                        return View(pbi.GetDetails());
                    }
                }
                TempData["errorState"] = 401;
                return RedirectToAction("Index");
            }
            TempData["errorState"] = 500;
            return RedirectToAction("Index");
        }
    }
}