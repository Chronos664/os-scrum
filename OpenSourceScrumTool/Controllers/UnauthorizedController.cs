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
    public class UnauthorizedController : Controller
    {
        // GET: Unauthorized
        public ActionResult Index()
        {
            if (TempData["requestedURL"] != null)
            {
                    ViewBag.Reason =
                        UserHelper.IsUserUnauthorized(HttpContext.User)
                            ? "have been blocked by the System Administrator"
                            : "lack the required permissions to access the requested page";
                    ViewBag.requestedURL = TempData["requestedURL"];
                    TempData.Remove("requestedURL");
            }
            return View();
        }
    }
}