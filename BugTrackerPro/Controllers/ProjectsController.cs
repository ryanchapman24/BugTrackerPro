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

                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult Edit(int? id)
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
            var project = db.Projects.Find(id);
            var helper = new ProjectAssignmentsHelper();
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

            var project = db.Projects.Find(model.Project.Id);
            db.Entry(project).Property("Updated").IsModified = true;
            project.Updated = System.DateTime.Now;
            ProjectAssignmentsHelper helper = new ProjectAssignmentsHelper();

            foreach (var user in db.Users.Select(u => u.Id).ToList())
            {
                helper.RemoveUserFromProject(user, project.Id);
            }

            if (model.SelectedUsers != null)
            {
                foreach (var user in model.SelectedUsers)
                {
                    helper.AddUserToProject(user, project.Id);
                }

                return RedirectToAction("Index");
            }

            else
            {
                return RedirectToAction("Index");
            }

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
