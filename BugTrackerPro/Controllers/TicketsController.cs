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
using Microsoft.AspNet.Identity;
using BugTrackerPro.Models.Helpers;

namespace BugTrackerPro.Controllers
{
    [Authorize]
    public class TicketsController : Universal
    {
        // GET: Tickets
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.AssignToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            List<Project> projects = new List<Project>();
            if (User.IsInRole("Admin"))
            {
                projects = db.Projects.OrderBy(p => p.Title).ToList();
            }
            else
            {
                projects = user.Projects.OrderBy(p => p.Title).ToList();
            }
            ViewBag.ProjectId = new SelectList(projects, "Id", "Title");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Submitter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId")] Ticket ticket)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                ticket.Created = DateTime.Now;
                ticket.TicketStatusId = 1;
                ticket.OwnerUserId = user.Id;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Details", "Projects", new { id = ticket.ProjectId });
            }
            List<Project> projects = new List<Project>();
            if (User.IsInRole("Admin"))
            {
                projects = db.Projects.OrderBy(p => p.Title).ToList();
            }
            else
            {
                projects = user.Projects.OrderBy(p => p.Title).ToList();
            }
            ViewBag.ProjectId = new SelectList(projects, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            List<Project> projects = new List<Project>();
            if (User.IsInRole("Admin"))
            {
                projects = db.Projects.OrderBy(p => p.Title).ToList();
            }
            else
            {
                projects = user.Projects.OrderBy(p => p.Title).ToList();
            }
            ViewBag.ProjectId = new SelectList(projects, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignToUserId")] Ticket ticket)
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            var oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);

            //if (oldTicket.Title != ticket.Title)
            //{

            //}

            if (ModelState.IsValid)
            {
                db.Tickets.Attach(ticket);
                db.Entry(ticket).Property("Title").IsModified = true;
                db.Entry(ticket).Property("Description").IsModified = true;
                db.Entry(ticket).Property("Updated").IsModified = true;
                db.Entry(ticket).Property("ProjectId").IsModified = true;
                db.Entry(ticket).Property("TicketTypeId").IsModified = true;
                db.Entry(ticket).Property("TicketPriorityId").IsModified = true;
                db.Entry(ticket).Property("TicketStatusId").IsModified = true;
                ticket.Updated = System.DateTime.Now;
                db.SaveChanges();

                return RedirectToAction("Details", "Projects", new { id = ticket.ProjectId });
            }
            List<Project> projects = new List<Project>();
            if (User.IsInRole("Admin"))
            {
                projects = db.Projects.OrderBy(p => p.Title).ToList();
            }
            else
            {
                projects = user.Projects.OrderBy(p => p.Title).ToList();
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult AssignDeveloper(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            UserRolesHelper helper = new UserRolesHelper();
            var developers = helper.UsersInRole("Developer");
            var devsOnProj = developers.Where(d => d.Projects.Any(p => p.Id == ticket.ProjectId));
            ViewBag.AssignToUserId = new SelectList(devsOnProj, "Id", "FullName", ticket.AssignToUserId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignDeveloper([Bind(Include = "Id,Title,Description,Created,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignToUserId")] Ticket ticket)
        {
            var oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);

            //if (oldTicket.Title != ticket.Title)
            //{

            //}

            if (ModelState.IsValid)
            {
                db.Tickets.Attach(ticket);
                db.Entry(ticket).Property("Updated").IsModified = true;
                db.Entry(ticket).Property("AssignToUserId").IsModified = true;
                db.Entry(ticket).Property("TicketStatusId").IsModified = true;
                ticket.Updated = System.DateTime.Now;
                ticket.TicketStatusId = 2;
                db.SaveChanges();

                return RedirectToAction("Details", "Projects", new { id = oldTicket.ProjectId });
            }

            UserRolesHelper helper = new UserRolesHelper();
            var developers = helper.UsersInRole("Developer");
            var devsOnProj = developers.Where(d => d.Projects.Any(p => p.Id == ticket.ProjectId));
            ViewBag.AssignToUserId = new SelectList(devsOnProj, "Id", "FullName", ticket.AssignToUserId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
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
