using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Configuration;

namespace OpenSourceScrumTool.Utilities
{
    public static class ConfigManager
    {
        private static NameValueCollection Configuration = System.Configuration.ConfigurationManager.AppSettings;

        public static string ADDomainName()
        {
            return Configuration["ADDomainName"];
        }

        public static string DefaultAdminGroupName()
        {
            return Configuration["AdminGroup"];
        }

        public static string DefaultScrumMasterGroupName()
        {
            return Configuration["ScrumMasterGroup"];            
        }

        public static string DefaultTeamLeaderGroupName()
        {
            return Configuration["TeamLeaderGroup"];
        }

        public static string DefaultTeamMemberGroupName()
        {
            return Configuration["TeamMemberGroup"];
        }

        public static string DefaultStakeHolderGroupName()
        {
            return Configuration["StakeHolderGroup"];
        }

        public static string DefaultViewOnlyGroupName()
        {
            return Configuration["ViewOnlyGroup"];
        }

        public static string DefaultUnauthorizedGroupName()
        {
            return Configuration["UnauthorizedGroup"];
        }
    }
}