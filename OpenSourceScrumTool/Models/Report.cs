using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenSourceScrumTool.Utilities;

namespace OpenSourceScrumTool.Models
{
    public class Report
    {
        public Report()
        {
            TotalTasksFinishedByUser = new List<UserTaskCount>();
            sprintNames = new List<string>();
            sprintVelocities = new List<int>();
            projectBurndownData = new List<int>();
        }
        public string ProjectName { get; set; }
        public int TotalBacklogItemsFinished { get; set; }
        public List<UserTaskCount> TotalTasksFinishedByUser { get; set; } 
        public int TotalEffort { get; set; }
        public List<string> sprintNames { get; set; }
        public List<int> sprintVelocities { get; set; }
        public List<int> projectBurndownData { get; set; }
        public int CurrentAverageVelocity { get; set; }
        public ChartData VelocityChart { get; set; }
        public ChartData BurndownChart { get; set; }
    }

    public class UserTaskCount
    {
        public string UserName { get; set; }
        public int TaskCount { get; set; }
    }
}