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
using System.IO;

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
                var path = Server.MapPath("~/TicketAttachments/" + ticket.Id);
                Directory.CreateDirectory(path);

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

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAttachment(IEnumerable<HttpPostedFileBase> file, int ticketId)
        {
            foreach (var doc in file)
            {
                if (FileUploadValidator.IsWebFriendlyFile(doc))
                {
                    //Counter
                    var num = 0;
                    //Gets Filename without the extension
                    var fileName = Path.GetFileNameWithoutExtension(doc.FileName);
                    var attach = fileName + Path.GetExtension(doc.FileName);
                    var ext = Path.GetExtension(doc.FileName);
                    //Checks if pPic matches any of the current attachments, 
                    //if so it will loop and add a (number) to the end of the filename
                    while (db.TicketAttachments.Where(a => a.TicketId == ticketId).Any(a => a.FileUrl == attach))
                    {
                        //Sets "filename" back to the default value
                        fileName = Path.GetFileNameWithoutExtension(doc.FileName);
                        //Add's parentheses after the name with a number ex. filename(4)
                        fileName = string.Format(fileName + "(" + ++num + ")");
                        //Makes sure pPic gets updated with the new filename so it could check
                        attach = fileName + Path.GetExtension(doc.FileName);
                    }
                    doc.SaveAs(Path.Combine(Server.MapPath("~/TicketAttachments/" + ticketId), fileName + Path.GetExtension(doc.FileName)));

                    TicketAttachment attachment = new TicketAttachment();
                    attachment.Created = DateTime.Now;
                    attachment.AuthorId = User.Identity.GetUserId();
                    attachment.TicketId = ticketId;
                    attachment.FileUrl = attach;
                    attachment.FileName = fileName;
                    attachment.Extension = ext.Substring(1, ext.Length -1);

                    db.TicketAttachments.Add(attachment);
                    db.SaveChanges();              
                }
            }

            return RedirectToAction("Details", new { id = ticketId });
        }

        // GET: Tickets/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAttachment(int id)
        {
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            db.TicketAttachments.Remove(ticketAttachment);
            db.SaveChanges();

            if ((System.IO.File.Exists("~/TicketAttachments/" + ticketAttachment.TicketId + "/" + ticketAttachment.FileUrl)))
            {
                System.IO.File.Delete("~/TicketAttachments/" + ticketAttachment.TicketId + "/" + ticketAttachment.FileUrl);
            }

            return Redirect(Url.Action("Details", "Tickets", new { id = ticketAttachment.TicketId }) + "#Comments");
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(string body, int ticketId)
        {
            if (body != null)
            {              
                TicketComment comment = new TicketComment();
                comment.Created = DateTime.Now;
                comment.AuthorId = User.Identity.GetUserId();
                comment.TicketId = ticketId;
                comment.Body = body;

                db.TicketComments.Add(comment);
                db.SaveChanges();
            }

            return Redirect(Url.Action("Details", "Tickets", new { id = ticketId }) + "#Comments");
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
