using BugTrackerPro.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerPro.Models.Helpers
{
    public class ProjectAssignmentsHelper
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserOnProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var flag = project.Users.Any(u => u.Id == userId);
            return (flag);
        }

        public void AddUserToProject(string userId, int projectId)
        {
            ApplicationUser user = db.Users.Find(userId);
            Project project = db.Projects.Find(projectId);
            project.Users.Add(user);
            db.SaveChanges();
        }

        public void RemoveUserFromProject(string userId, int projectId)
        {
            ApplicationUser user = db.Users.Find(userId);
            Project project = db.Projects.Find(projectId);
            project.Users.Remove(user);
            db.SaveChanges();
        }

        public IList<Project> ListUserProjects(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);
            var projects = user.Projects.ToList();
            return (projects);
        }

        public IList<ApplicationUser> ListUsersOnProject(int projectId)
        {
            Project project = db.Projects.Find(projectId);
            var users = project.Users.ToList();
            return (users);
        }

        public IList<ApplicationUser> ListUsersNotOnProject(int projectId)
        {
            Project project = db.Projects.Find(projectId);
            var users = project.Users;
            var notusers = project.Users.Where(u => !users.Contains(u)).ToList();
            return (notusers);
        }
    }
}