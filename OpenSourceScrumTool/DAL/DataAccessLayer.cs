using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using OpenSourceScrumTool.Account_Manager;
using OpenSourceScrumTool.Models;
using OpenSourceScrumTool.Utilities;

namespace OpenSourceScrumTool.DAL
{
    public class DataAccessLayer : IDisposable
    {
        private SCRUMToolModel db;
        public DataAccessLayer()
        {
            db = new SCRUMToolModel();
        }

        #region Project Access
        #region Get Projetcs
        public List<Project> GetProjects()
        {
            return db.Projects.ToList();
        }
        public Project GetProject(int id)
        {
            return db.Projects.First(p => p.ID == id);

        }

        public IEnumerable<ProjectDetailsDTO> GetAllProjectDetails()
        {
            IEnumerable<Project> Projects = db.Projects.ToList();
            return Projects.GetDetails<ProjectDetailsDTO>();
        }

        public ProjectDetailsDTO GetProjectDetails(int id)
        {
            try
            {
                return (ProjectDetailsDTO)db.Projects.First(p => p.ID == id).GetDetails();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        public int AddProject(Project p)
        {
            if (p.ID == 0)
            {
                db.Projects.Add(p);
                db.SaveChanges();
                return p.ID;
            }
            return 0;
        }
        public void UpdateProject(int id, Project p)
        {
            Project temp = GetProject(id);
            temp.UpdateItem(p);
            db.SaveChanges();
        }
        public void ArchiveProject(int id)
        {
            Project p = GetProject(id);
            if (p == null) return;
            List<int> pbiIds = new List<int>();
            List<int> pbiTasks = new List<int>();
            List<int> iterationIds = p.Iterations.Select(i => i.ID).ToList();
            foreach (ProductBacklogItem pbi in p.ProductBacklogItems)
            {
                pbiIds.Add(pbi.ID);
                pbiTasks.AddRange(pbi.Tasks.Select(pbiTask => pbiTask.ID));
            }
            foreach (int i in pbiTasks)
            {
                ArchiveTask(i);
            }
            foreach (int i in pbiIds)
            {
                ArchiveProductBacklogItem(i);
            }
            foreach (int i in iterationIds)
            {
                ArchiveIteration(i);
            }
            p.Archived = true;
            db.SaveChanges();
        }
        public void RestoreProject(int id)
        {
            Project p = GetProject(id);
            if (p == null) return;
            List<int> pbiIds = new List<int>();
            List<int> pbiTasks = new List<int>();
            List<int> iterationIds = p.Iterations.Select(i => i.ID).ToList();
            foreach (ProductBacklogItem pbi in p.ProductBacklogItems)
            {
                pbiIds.Add(pbi.ID);
                pbiTasks.AddRange(pbi.Tasks.Select(pbiTask => pbiTask.ID));
            }
            foreach (int i in pbiTasks)
            {
                RestoreTask(i);
            }
            foreach (int i in pbiIds)
            {
                RestoreProductBacklogItem(i);
            }
            foreach (int i in iterationIds)
            {
                RestoreIteration(i);
            }
            p.Archived = false;
            db.SaveChanges();
        }

        public void SetTeamToProjectMap(TeamToProject tpmap, TypeOfDBAction type)
        {
            Project p = GetProject(tpmap.ProjectID);
            if (p == null) return;
            Team t = GetTeam(tpmap.teamID);
            if (t == null) return;
            switch (type)
            {
                case TypeOfDBAction.Add:
                    p.Teams.Add(t);
                    break;
                case TypeOfDBAction.Delete:
                    p.Teams.Remove(t);
                    break;
            }
            db.SaveChanges();
        }
        #endregion

        #region Product Backlog Item Access

        public ProductBacklogItem GetProductBacklogItem(int id)
        {
            return db.ProductBacklogItems.First(p => p.ID == id);
        }

        public List<ProductBacklogItem> GetProductBacklogItems()
        {
            return db.ProductBacklogItems.ToList();
        }

        public int AddProductBacklogItem(ProductBacklogItem pbi)
        {
            db.ProductBacklogItems.Add(pbi);
            db.SaveChanges();
            if (pbi.ID != 0)
            {
                return pbi.ID;
            }
            return 0;
        }

        public void UpdateProductBacklogItem(int id, ProductBacklogItem pbi)
        {
            ProductBacklogItem temp = GetProductBacklogItem(id);
            temp.UpdateItem(pbi);
            db.SaveChanges();
        }

        public void ArchiveProductBacklogItem(int id)
        {
            ProductBacklogItem pbi = GetProductBacklogItem(id);
            if (pbi != null)
            {
                pbi.Archived = true;
                List<BacklogItemTask> taskList = pbi.Tasks.ToList();
                foreach (BacklogItemTask t in taskList)
                {
                    ArchiveTask(t.ID);
                }
                db.SaveChanges();
            }
        }

        public void RestoreProductBacklogItem(int id)
        {
            ProductBacklogItem pbi = GetProductBacklogItem(id);
            if (pbi != null)
            {
                pbi.Archived = false;
                List<BacklogItemTask> taskList = pbi.Tasks.ToList();
                foreach (BacklogItemTask t in taskList)
                {
                    RestoreTask(t.ID);
                }
                db.SaveChanges();
            }
        }

        public int GetNextPBIPriority(int projectId)
        {
            Project p = GetProject(projectId);
            int lowestPriority = p.ProductBacklogItems.Where(pbi => !pbi.Archived).Select(pbi => pbi.Priority).Concat(new[] { 0 }).Max();
            return lowestPriority + 1;
        }
        #endregion

        #region BacklogItemTask Access

        public BacklogItemTask GetTask(int id)
        {
            return db.Tasks.First(t => t.ID == id);
        }
        public List<BacklogItemTask> GetTasks()
        {
            return db.Tasks.ToList();
        }

        public int AddTask(BacklogItemTask t)
        {
            if (t.ID == 0)
            {
                db.Tasks.Add(t);
                db.SaveChanges();
                return t.ID;
            }
            return 0;
        }

        public void UpdateTask(int id, BacklogItemTask t)
        {
            BacklogItemTask tempTask = GetTask(id);
            tempTask.UpdateItem(t);
            db.SaveChanges();
        }

        public void ArchiveTask(int id)
        {
            BacklogItemTask t = GetTask(id);
            if (t == null) return;
            t.Archived = true;
            db.SaveChanges();
        }
        public void RestoreTask(int id)
        {
            BacklogItemTask t = GetTask(id);
            if (t == null) return;
            t.Archived = false;
            db.SaveChanges();
        }

        #endregion

        #region Iteration Access
        public Iteration GetIteration(int id)
        {
            return db.Iterations.First(i => i.ID == id);
        }
        public List<Iteration> GetIterations()
        {
            return db.Iterations.ToList();
        }

        public int AddIteration(Iteration i)
        {
            if (i.ID == 0)
            {
                db.Iterations.Add(i);
                db.SaveChanges();
                return i.ID;
            }
            return 0;
        }

        public void UpdateIteration(int id, Iteration i)
        {
            Iteration tempIteration = GetIteration(id);
            tempIteration.UpdateItem(i);
            db.SaveChanges();
        }

        public void ArchiveIteration(int id)
        {
            Iteration i = GetIteration(id);
            if (i == null) return;
            i.Archived = true;
            db.SaveChanges();
        }

        public void RestoreIteration(int id)
        {
            Iteration i = GetIteration(id);
            if (i == null) return;
            i.Archived = false;
            db.SaveChanges();
        }

        #endregion


        #region Team Access
        public Team GetTeam(int id)
        {
            return db.Teams.First(t => t.ID == id);
        }

        public List<Team> GetTeamsOnProject(int projectID)
        {
            return db.Projects.First(p => p.ID == projectID).Teams.ToList();
        }
        public List<Team> GetTeams()
        {
            return db.Teams.ToList();
        }

        public int AddTeam(Team t)
        {
            if (t.ID == 0)
            {
                db.Teams.Add(t);
                db.SaveChanges();
                return t.ID;
            }
            return 0;
        }

        public void UpdateTeam(int id, Team t)
        {
            Team tempTeam = GetTeam(id);
            tempTeam.UpdateItem(t);
            db.SaveChanges();
        }

        public void ArchiveTeam(int id)
        {
            Team t = GetTeam(id);
            if (t == null) return;
            t.Archived = true;
            db.SaveChanges();
        }

        public void RestoreTeam(int id)
        {
            Team t = GetTeam(id);
            if (t == null) return;
            t.Archived = false;
            db.SaveChanges();
        }


        #endregion

        #region Role Access
        public Role GetRole(int id)
        {
            return db.Roles.First(r => r.ID == id);
        }
        public List<Role> GetRoles()
        {
            return db.Roles.ToList();
        }

        public int AddRole(Role r)
        {
            if (r.ID == 0)
            {
                db.Roles.Add(r);
                db.SaveChanges();
                return r.ID;
            }
            return 0;
        }

        public void UpdateRole(int id, Role r)
        {
            Role tempRole = GetRole(id);
            tempRole.UpdateItem(r);
            db.SaveChanges();
        }

        public void DeleteRole(int id)
        {
            if (GetRole(id) != null)
            {
                db.Roles.Remove(GetRole(id));
                db.SaveChanges();
            }
        }
        #endregion

        #region User Access
        public User GetUser(int id)
        {
            return db.Users.First(u => u.ID == id);
        }
        public List<User> GetUsers()
        {
            return db.Users.ToList();
        }

        public List<User> GetUsersOnProject(int ProjectID)
        {
            Project p = GetProject(ProjectID);
            if (p == null) throw new Exception("Invalid Project", new Exception("No Project with ID: " + ProjectID + " Present in the system"));
            List<User> users = new List<User>();
            foreach (Team t in p.Teams)
            {
                users.AddRange(t.Users);
            }
            return users.Distinct().ToList();
        }

        public List<Project> GetProjectsForUser(int UserID)
        {
            User user = GetUser(UserID);
            if (user == null) throw new Exception("Invalid User", new Exception("No User with ID: " + UserID + " Present in the system"));
            List<Project> result = new List<Project>();
            foreach (Team t in user.Teams)
            {
                result.AddRange(t.Projects);
            }
            return result.Distinct().ToList();
        }

        public int AddUser(User u)
        {
            if (u.ID == 0)
            {
                db.Users.Add(u);
                db.SaveChanges();
                return u.ID;
            }
            return 0;
        }

        public void UpdateUser(int id, User u)
        {
            User tempUser = GetUser(id);
            tempUser.UpdateItem(u);
            db.SaveChanges();
        }

        public void BlockUser(int id)
        {
            User u = GetUser(id);
            if (u == null) throw new Exception("Invalid User", new Exception("No User with ID: " + id + " Present in the system."));
            u.UserInRoles.Add(new UserInRole()
            {
                isExplicit = true,
                UserID = id,
                RoleID = GetRoles().First(r => r.RoleType == (int)RoleEnum.Roles.Unauthorized).ID
            });
        }
        public void UnblockUser(int id)
        {
            User u = GetUser(id);
            if (u == null) throw new Exception("Invalid User", new Exception("No User with ID: " + id + " Present in the system."));
            List<UserInRole> userinRoles = u.UserInRoles.ToList();
            foreach (UserInRole uir in userinRoles)
            {
                if (uir.Role.RoleType == (int)RoleEnum.Roles.Unauthorized)
                {
                    db.UserInRoles.Remove(uir);
                }
            }
        }
        #endregion

        public void AddUserToRole(int usrID, int roleID, bool isExplicit)
        {
            User CurrentUser = GetUser(usrID);
            Role requestedRole = GetRole(roleID);
            UserInRole userInRole = new UserInRole()
            {
                isExplicit = isExplicit,
                RoleID = roleID,
                UserID = usrID,
            };
            db.UserInRoles.Add(userInRole);
            db.SaveChanges();
        }

        public void DeleteUserFromRole(int usrID, int roleID)
        {
            UserInRole uir = db.UserInRoles.First(r => r.UserID == usrID && r.RoleID == roleID);
            if (uir != null)
            {
                db.UserInRoles.Remove(uir);
                db.SaveChanges();
            }
        }

        public bool isUserInRole(int userID, int roleID)
        {
            UserInRole result = db.UserInRoles.FirstOrDefault(uir => uir.UserID == userID && uir.RoleID == roleID);
            if (result == null)
            {
                return false;
            }
            return true;
        }

        public List<Role> GetRolesForUser(int UserID)
        {
            return db.UserInRoles.Where(uir => uir.UserID == UserID).ToList().Select(uir => uir.Role).ToList();
        }

        public List<Role> GetRoles(int RoleLevel)
        {
            return db.Roles.Where(r => r.RoleType <= RoleLevel).ToList();
        }

        public void DeleteUserToRoleMap(int usrId, int roleId)
        {
            UserInRole result = db.UserInRoles.First(uir => uir.UserID == usrId && uir.RoleID == roleId);
            db.UserInRoles.Remove(result);
            db.SaveChanges();
        }

        public void AddUserToTeam(int usrID, int teamID)
        {
            User CurrentUser = GetUser(usrID);
            Team requestedTeam = GetTeam(teamID);
            if (CurrentUser != null && requestedTeam != null)
            {
                requestedTeam.Users.Add(CurrentUser);
            }
            db.SaveChanges();
        }
        public void RemoveUserFromTeam(int usrID, int teamID)
        {
            User CurrentUser = GetUser(usrID);
            Team requestedTeam = GetTeam(teamID);
            if (CurrentUser != null && requestedTeam != null)
            {
                requestedTeam.Users.Remove(CurrentUser);
            }
            db.SaveChanges();
        }
        public void AddLog(Log log)
        {
            db.Logs.Add(log);
            db.SaveChanges();
        }

        public List<Log> GetLogs()
        {
            return db.Logs.ToList();
        }

        public Log GetLog(int id)
        {
            return db.Logs.First(l => l.ID == id);
        }

        public Iteration GetCurrentIterationForProject(int projID)
        {
            Project p = GetProject(projID);
            if (p == null) return null;
            List<Iteration> projIterations = p.Iterations.ToList();
            foreach (Iteration i in projIterations)
            {
                if (i.SprintStartDate < DateTime.Now && i.SprintEndDate > DateTime.Now)
                {
                    return i;
                }
            }
            return null;
        }
        public void Dispose()
        {
            db.SaveChanges();
            db.Dispose();
            db = null;
        }


    }
}