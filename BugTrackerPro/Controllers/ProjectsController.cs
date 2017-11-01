using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTrackerPro.Models;
using BugTrackerPro.Models.CodeFirst;
using BugTrackerPro.Models.Helpers;
using Microsoft.AspNet.Identity;

namespace BugTrackerPro.Controllers
{
    [Authorize]
    [RequireUnlockedAccount]
    public class ProjectsController : Universal
    {
        // GET: Projects
        [Authorize]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            IList<Project> projects = new List<Project>();

            if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
            {
                projects = db.Projects.OrderBy(p => p.Title).ToList();
            }
            else
            {
                projects = user.Projects.OrderBy(p => p.Title).ToList();
            }

            return View(projects);
        }

        // GET: Projects/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ProjectAssignmentsHelper helper = new ProjectAssignmentsHelper();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            if (!User.IsInRole("Admin") && helper.IsUserOnProject(user.Id, project.Id) == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Created = DateTime.Now;
                project.AuthorId = db.Users.Find(User.Identity.GetUserId()).Id;
                db.Projects.Add(project);
                db.SaveChanges();

                ApplicationUser attachUser = db.Users.Find(User.Identity.GetUserId());
                Project attachProject = db.Projects.Find(project.Id);
                ProjectAssignmentsHelper helper = new ProjectAssignmentsHelper();
                helper.AddUserToProject(attachUser.Id, attachProject.Id);

                UserRolesHelper urh = new UserRolesHelper();
                foreach (var user in urh.UsersInRole("Admin"))
                {
                    Notification n = new Notification();
                    n.Type = "NEW PROJECT";
                    n.Created = DateTime.Now;
                    n.CreatedString = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt");
                    n.Description = "New project created.";
                    n.ProjectId = project.Id;
                    n.NotifyUserId = user.Id;
                    n.Seen = false;
                    db.Notifications.Add(n);
                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult Edit(int? id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ProjectAssignmentsHelper helper = new ProjectAssignmentsHelper();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            if (!User.IsInRole("Admin") && helper.IsUserOnProject(user.Id, project.Id) == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Title,Description,AuthorId")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                project.Updated = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/EditProjectAssignments
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult EditProjectAssignments(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var project = db.Projects.Find(id);
            ProjectAssignmentsHelper helper = new ProjectAssignmentsHelper();
            if (!User.IsInRole("Admin") && helper.IsUserOnProject(user.Id, project.Id) == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = new ProjectUserViewModels();
            model.Project = project;
            model.SelectedUsers = helper.ListUsersOnProject(id).OrderBy(u => u.FirstName).Select(u => u.Id).ToArray();
            model.Users = new MultiSelectList(db.Users.OrderBy(u => u.FirstName), "Id", "FullName", model.SelectedUsers);

            return View(model);
        }

        // Post: Projects/EditProjectAssignments
        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult EditProjectAssignments(ProjectUserViewModels model)
        {
            var userId = db.Users.Find(User.Identity.GetUserId()).Id;
            var project = db.Projects.Find(model.Project.Id);
            db.Entry(project).Property("Updated").IsModified = true;
            project.Updated = DateTime.Now;
            ProjectAssignmentsHelper helper = new ProjectAssignmentsHelper();
            var projUsers = project.Users;

            foreach (var user in projUsers)
            {
                helper.RemoveUserFromProject(user.Id, project.Id);
            }

            if (model.SelectedUsers != null)
            {
                foreach (var user in model.SelectedUsers)
                {
                    helper.AddUserToProject(user, project.Id);
                    var addedUser = db.Users.Find(user);
                    projUsers.Remove(addedUser);
                }
            }

            foreach (var user in projUsers)
            {
                foreach (var ticket in project.Tickets.Where(t => t.AssignToUserId == user.Id).ToList())
                {
                    var oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
                    ticket.AssignToUserId = null;

                    TicketHistory th = new TicketHistory();
                    th.Property = "ASSIGNMENT REMOVED (Developer removed from Project)";
                    th.AuthorId = userId;
                    th.TicketId = ticket.Id;
                    th.Created = DateTime.Now;
                    th.OldValue = oldTicket.AssignToUser.FullName;
                    db.TicketHistories.Add(th);

                    if (ticket.TicketStatusId != 4)
                    {
                        ticket.TicketStatusId = 1;

                        TicketHistory th2 = new TicketHistory();
                        th2.Property = "STATUS";
                        th2.AuthorId = user.Id;
                        th2.TicketId = ticket.Id;
                        th2.Created = DateTime.Now;
                        th2.OldValue = oldTicket.TicketStatus.Name;
                        th2.NewValue = db.TicketStatuses.Find(1).Name;
                        db.TicketHistories.Add(th2);
                    }

                    db.Tickets.Attach(ticket);
                    db.Entry(ticket).Property("AssignToUserId").IsModified = true;
                    db.Entry(ticket).Property("TicketStatusId").IsModified = true;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            Project project = db.Projects.Find(id);
            var projectNotifs = db.Notifications.Where(n => n.ProjectId == project.Id);
            foreach (var n in projectNotifs)
            {
                db.Notifications.Remove(n);
            }
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
