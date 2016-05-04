using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSourceScrumTool.DAL;
using OpenSourceScrumTool.Models;

namespace OpenSourceScrumTool.Tests
{
    [TestClass]
    public class DalUnitTests
    {
        private Project TestProject = new Project {ProjectName = "Test Project"};

        private ProductBacklogItem testPBI = new ProductBacklogItem
        {
            Name = "Initial Setup",
            Description = "Initial Setup of the Project",
            ProjectID = 1,
            EffortScore = 5
        };

        #region Project DAL Tests
        [TestMethod]
        public void AddProject()
        {
            using (DataAccessLayer dbAccessLayer = new DataAccessLayer())
            {
                int id = dbAccessLayer.AddProject(TestProject);
                TestProject.ID = id;
                Assert.AreEqual(1,id);
            }
        }
        [TestMethod]
        public void GetProjects()
        {
            using (DataAccessLayer dbAccessLayer = new DataAccessLayer())
            {
                List<Project> projects = dbAccessLayer.GetProjects();
                Assert.AreEqual(projects[0].ProjectName, TestProject.ProjectName);
            }
        }
        [TestMethod]
        public void UpdateProject()
        {
            using (DataAccessLayer dbAccessLayer = new DataAccessLayer())
            {
                Project temp = new Project();
                temp.ProjectName = "Test Project 2";
                dbAccessLayer.UpdateProject(1, temp);
                Project temp2 = dbAccessLayer.GetProject(1);
                Assert.AreEqual(temp.ProjectName, temp2.ProjectName);
            }

        }
        [TestMethod]
        public void GetProject()
        {
            using (DataAccessLayer dbAccessLayer = new DataAccessLayer())
            {
                Project p = dbAccessLayer.GetProject(1);
                Assert.AreEqual(TestProject.ProjectName, p.ProjectName);
            }
        }
        [TestMethod]
        public void DeleteProject()
        {
            using (DataAccessLayer dbAccessLayer = new DataAccessLayer())
            {
                dbAccessLayer.ArchiveProject(1);
                Assert.AreEqual(null, dbAccessLayer.GetProject(1));
            }
        }
        #endregion

        //Not Implemented Currently
        #region Product Backlog DAL Tests

        [TestMethod]
        public void AddPBI()
        {
            using (DataAccessLayer dbAccessLayer = new DataAccessLayer())
            {
                
            }
            
        }

        [TestMethod]
        public void GetPBIS()
        {
            using (DataAccessLayer dbAccessLayer = new DataAccessLayer())
            {

            }
        }

        [TestMethod]
        public void GetPBI()
        {
            using (DataAccessLayer dbAccessLayer = new DataAccessLayer())
            {

            }
        }

        [TestMethod]
        public void UpdatePBI()
        {
            using (DataAccessLayer dbAccessLayer = new DataAccessLayer())
            {

            }
        }

        [TestMethod]
        public void RemovePBI()
        {
            using (DataAccessLayer dbAccessLayer = new DataAccessLayer())
            {

            }
        }



        #endregion
    }
}
