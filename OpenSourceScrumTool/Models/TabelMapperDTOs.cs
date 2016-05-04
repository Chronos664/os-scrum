using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenSourceScrumTool.Models
{
    public class UserToRole
    {
        public int userID { get; set; }
        public int roleID { get; set; }
        public bool isEplicit { get; set; }
    }

    public class UserToTeam
    {
        public int userID { get; set; }
        public int teamID { get; set; }
    }

    public class TeamToProject
    {
        public int teamID { get; set; }
        public int ProjectID { get; set; }
        
    }

    public class ChangePBIPriority
    {
        public string[] pbiIDs { get; set; }
    }

    public class SetIteration
    {
        public int PbiId { get; set; }
        public int? IterationId { get; set; }
    }

    public enum TypeOfDBAction
    {
        Add,
        Update,
        Delete
    }
}