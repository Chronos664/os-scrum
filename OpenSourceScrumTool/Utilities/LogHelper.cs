using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using OpenSourceScrumTool.DAL;
using OpenSourceScrumTool.Models;

namespace OpenSourceScrumTool.Utilities
{
    public static class LogHelper
    {
        public static void InfoLog(string message)
        {
            _log(message, LogLevel.Info);
        }
        public static void DebugLog(string message)
        {
            _log(message, LogLevel.Debug);
        }
        public static void WarnLog(string message)
        {
            _log(message, LogLevel.Warn);
        }
        public static void ErrorLog(string message)
        {
            _log(message, LogLevel.Error);
        }
        private static void _log(string message, LogLevel level)
        {
            HttpContext Context = HttpContext.Current;
            try
            {
                using (DataAccessLayer model = new DataAccessLayer())
                {
                    User actionedUser = UserHelper.GetUser(Context.User, model);
                    Log newLog = new Log()
                    {
                        ID = 0,
                        LogLevel = (int) level,
                        LogMessage = message,
                        Time = DateTime.Now,
                        UserIDForAction = actionedUser.ID
                    };
                    model.AddLog(newLog);
                    string state = "";
                    switch (level)
                    {
                        case LogLevel.Info:
                            state = "Information";
                            break;
                        case LogLevel.Debug:
                            state = "Debug";
                            break;
                        case LogLevel.Warn:
                            state = "Warn";
                            break;
                        case LogLevel.Error:
                            state = "Error";
                            break;
                    }
                    Trace.WriteLine("[OS-Scrum] " + state + ": " + newLog.LogMessage + " Initiated by user: " + newLog.UserIDForAction);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[OS-Scrum]: Error connecting to database to write Log Message: " + ex.ToString());
            }
        }

    }

    public enum LogLevel
    {
        Info = 0,
        Debug = 1,
        Warn = 2,
        Error = 3
    }
}