using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenSourceScrumTool.Account_Manager;
using OpenSourceScrumTool.DAL;
using OpenSourceScrumTool.Models;
using OpenSourceScrumTool.Utilities;

namespace OpenSourceScrumTool.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [UserAuthenticate(RoleEnum.Roles.Viewonly)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult ScrumBoard(int id)
        {
            using (DataAccessLayer modelAccess = new DataAccessLayer())
            {
                IterationDetailsDTO data =
                    (IterationDetailsDTO) modelAccess.GetCurrentIterationForProject(id).GetDetails();
                return PartialView("_ScrumBoard", data);
            }
        }
    }
}