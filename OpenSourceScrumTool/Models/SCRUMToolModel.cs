using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OpenSourceScrumTool.Models
{
    public partial class SCRUMToolModel : DbContext
    {
        public SCRUMToolModel()
            : base("name=SCRUMToolModel")
        {
        }

        public virtual DbSet<Iteration> Iterations { get; set; }
        public virtual DbSet<ProductBacklogItem> ProductBacklogItems { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<BacklogItemTask> Tasks { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<UserInRole> UserInRoles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Iteration>()
                .Property(e => e.SprintName)
                .IsUnicode(false);

            modelBuilder.Entity<Iteration>()
                .HasMany(e => e.ProductBacklogItems)
                .WithOptional(e => e.CurrentIteration)
                .HasForeignKey(e => e.SprintID);

            modelBuilder.Entity<ProductBacklogItem>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<ProductBacklogItem>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<ProductBacklogItem>()
                .HasMany(e => e.Tasks)
                .WithRequired(e => e.ParentProductBacklogItem)
                .HasForeignKey(e => e.ProductBacklogID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Project>()
                .Property(e => e.ProjectName)
                .IsUnicode(false);

            modelBuilder.Entity<Project>()
                .HasMany(e => e.ProductBacklogItems)
                .WithRequired(e => e.Project)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Project>()
                .HasMany(e => e.ProductBacklogItems)
                .WithRequired(e => e.Project)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Project>()
                .HasMany(e => e.Teams)
                .WithMany(e => e.Projects)
                .Map(m => m.ToTable("TeamsOnProject").MapLeftKey("ProjectID").MapRightKey("TeamID"));

            modelBuilder.Entity<Role>()
                .Property(e => e.RoleName)
                .IsUnicode(false);

            modelBuilder.Entity<Role>()
                .Property(e => e.ADGroupName)
                .IsUnicode(false);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.UserInRoles)
                .WithRequired(e => e.Role)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BacklogItemTask>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<BacklogItemTask>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Team>()
                .Property(e => e.TeamName)
                .IsUnicode(false);

            modelBuilder.Entity<Team>()
                .HasMany(e => e.Users)
                .WithMany(e => e.Teams)
                .Map(m => m.ToTable("TeamMembers").MapLeftKey("TeamID").MapRightKey("UserID"));

            modelBuilder.Entity<User>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.emailAddress)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Logs)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserIDForAction)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserInRoles)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);
            
            modelBuilder.Entity<Log>()
                .Property(e => e.LogMessage)
                .IsUnicode(false);



        }
    }
}
