using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace OpenSourceScrumTool.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            "~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/chartjs").Include("~/Scripts/Chart.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/Utilities").Include(
                "~/Resources/Scripts/Helpers.js",
                "~/Resources/Scripts/Dialogs.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").IncludeDirectory("~/Content/themes/base", "*.css"));
        }
    }
}