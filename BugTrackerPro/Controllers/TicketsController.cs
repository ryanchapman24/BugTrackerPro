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
using Newtonsoft.Json;

namespace BugTrackerPro.Controllers
{
    public class CreatedComment
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public string Created { get; set; }
        public string FullName { get; set; }
        public string ProfilePic { get; set; }
    }

    [Authorize]
    [RequireUnlockedAccount]
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
            var user = db.Users.Find(User.Identity.GetUserId());
            ProjectAssignmentsHelper helper = new ProjectAssignmentsHelper();
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            if (User.IsInRole("Admin") || (User.IsInRole("Project Manager") && helper.IsUserOnProject(user.Id, ticket.ProjectId) == true) || (User.IsInRole("Developer") && ticket.AssignToUserId == user.Id) || (User.IsInRole("Submitter") && ticket.OwnerUserId == user.Id))
            {
                return View(ticket);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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

                TicketHistory th = new TicketHistory();
                th.Property = "TICKET CREATED";
                th.AuthorId = user.Id;
                th.TicketId = ticket.Id;
                th.Created = DateTime.Now;
                db.TicketHistories.Add(th);
                db.SaveChanges();

                foreach (var u in db.Users.Where(u => u.Roles.Any(r => r.RoleId == "dec84673-970c-4770-aa44-8fb51f70e2b7") || (u.Roles.Any(r => r.RoleId == "b9ab4f3d-4e8b-42b9-8b63-8326c768934a") && u.Projects.Any(p => p.Id == ticket.ProjectId))))
                {
                    Notification n = new Notification();
                    n.Type = "UNASSIGNED TICKET";
                    n.Created = DateTime.Now;
                    n.CreatedString = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt");
                    n.Description = "Ticket requires assignment [#" +ticket.Id + "]";
                    n.TicketId = ticket.Id;
                    n.ProjectId = ticket.ProjectId;
                    n.NotifyUserId = u.Id;
                    n.Seen = false;
                    db.Notifications.Add(n);
                }
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
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
            ProjectAssignmentsHelper helper = new ProjectAssignmentsHelper();
            UserRolesHelper urh = new UserRolesHelper();
            var oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
            var changed = false;

            if (ModelState.IsValid)
            {
                db.Tickets.Attach(ticket);
                db.Entry(ticket).Property("Title").IsModified = true;
                db.Entry(ticket).Property("Description").IsModified = true;
                db.Entry(ticket).Property("Updated").IsModified = true;
                db.Entry(ticket).Property("TicketTypeId").IsModified = true;
                db.Entry(ticket).Property("TicketPriorityId").IsModified = true;
                db.Entry(ticket).Property("TicketStatusId").IsModified = true;
                db.Entry(ticket).Property("ProjectId").IsModified = true;
                db.Entry(ticket).Property("AssignToUserId").IsModified = true;
                ticket.Updated = DateTime.Now;

                if (oldTicket.Title != ticket.Title)
                {
                    changed = true;

                    TicketHistory th = new TicketHistory();
                    th.Property = "TITLE";
                    th.AuthorId = user.Id;
                    th.TicketId = ticket.Id;
                    th.Created = DateTime.Now;
                    th.OldValue = oldTicket.Title;
                    th.NewValue = ticket.Title;
                    db.TicketHistories.Add(th);
                }

                if (oldTicket.Description != ticket.Description)
                {
                    changed = true;

                    TicketHistory th = new TicketHistory();
                    th.Property = "DESCRIPTION";
                    th.AuthorId = user.Id;
                    th.TicketId = ticket.Id;
                    th.Created = DateTime.Now;
                    th.OldValue = oldTicket.Description;
                    th.NewValue = ticket.Description;
                    db.TicketHistories.Add(th);
                }

                if (oldTicket.TicketStatusId != ticket.TicketStatusId)
                {
                    changed = true;

                    TicketHistory th = new TicketHistory();
                    th.Property = "STATUS";
                    th.AuthorId = user.Id;
                    th.TicketId = ticket.Id;
                    th.Created = DateTime.Now;
                    th.OldValue = oldTicket.TicketStatus.Name;
                    th.NewValue = db.TicketStatuses.Find(ticket.TicketStatusId).Name;
                    db.TicketHistories.Add(th);
                }

                if (oldTicket.TicketPriorityId != ticket.TicketPriorityId)
                {
                    changed = true;

                    TicketHistory th = new TicketHistory();
                    th.Property = "PRIORITY";
                    th.AuthorId = user.Id;
                    th.TicketId = ticket.Id;
                    th.Created = DateTime.Now;
                    th.OldValue = oldTicket.TicketPriority.Name;
                    th.NewValue = db.TicketPriorities.Find(ticket.TicketPriorityId).Name;
                    db.TicketHistories.Add(th);
                }

                if (oldTicket.TicketTypeId != ticket.TicketTypeId)
                {
                    changed = true;

                    TicketHistory th = new TicketHistory();
                    th.Property = "TYPE";
                    th.AuthorId = user.Id;
                    th.TicketId = ticket.Id;
                    th.Created = DateTime.Now;
                    th.OldValue = oldTicket.TicketType.Name;
                    th.NewValue = db.TicketTypes.Find(ticket.TicketTypeId).Name;
                    db.TicketHistories.Add(th);
                }

                if (oldTicket.ProjectId != ticket.ProjectId)
                {
                    changed = true;
                    var newProj = db.Projects.Find(ticket.ProjectId);

                    if (helper.IsUserOnProject(oldTicket.OwnerUserId, newProj.Id))
                    {
                        helper.AddUserToProject(oldTicket.OwnerUserId, newProj.Id);
                    }
                    db.SaveChanges();
                    //foreach (var u in db.Users.Where(u => u.Notifications.Any(n => n.TicketId == ticket.Id)))
                    //{
                    //    foreach (var n in u.Notifications)
                    //    {
                    //        if (!helper.IsUserOnProject(u.Id,newProj.Id) && !urh.IsUserInRole(u.Id,"Admin"))
                    //        {
                    //            db.Notifications.Remove(n);
                    //        }
                    //        else
                    //        {
                    //            n.ProjectId = newProj.Id;
                    //        }
                    //    }
                    //}
                    TicketHistory th = new TicketHistory();
                    th.Property = "PROJECT";
                    th.AuthorId = user.Id;
                    th.TicketId = ticket.Id;
                    th.Created = DateTime.Now;
                    th.OldValue = oldTicket.Project.Title;
                    th.NewValue = db.Projects.Find(ticket.ProjectId).Title;
                    db.TicketHistories.Add(th);

                    if (oldTicket.AssignToUserId != null)
                    {
                        if (!newProj.Users.Any(u => u.Id == oldTicket.AssignToUserId))
                        {
                            ticket.AssignToUserId = null;
                            ticket.TicketStatusId = 1;

                            TicketHistory th2 = new TicketHistory();
                            th2.Property = "ASSIGNMENT REMOVED (Developer not on ticket Project)";
                            th2.AuthorId = user.Id;
                            th2.TicketId = ticket.Id;
                            th2.Created = DateTime.Now;
                            th2.OldValue = oldTicket.AssignToUser.FullName;
                            db.TicketHistories.Add(th2);

                            foreach (var u in db.Users.Where(u => urh.IsUserInRole(u.Id, "Admin") || (urh.IsUserInRole(u.Id, "Project Manager")) && helper.IsUserOnProject(u.Id, ticket.ProjectId)))
                            {
                                Notification n = new Notification();
                                n.Type = "UNASSIGNED TICKET";
                                n.Created = DateTime.Now;
                                n.CreatedString = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt");
                                n.Description = "Ticket requires assignment [#" + ticket.Id + "]";
                                n.TicketId = ticket.Id;
                                n.ProjectId = newProj.Id;
                                n.NotifyUserId = u.Id;
                                n.Seen = false;
                                db.Notifications.Add(n);
                            }

                            if (ticket.TicketStatusId != 4)
                            {
                                ticket.TicketStatusId = 1;
                                TicketHistory th3 = new TicketHistory();
                                th3.Property = "STATUS";
                                th3.AuthorId = user.Id;
                                th3.TicketId = ticket.Id;
                                th3.Created = DateTime.Now;
                                th3.OldValue = oldTicket.TicketStatus.Name;
                                th3.NewValue = db.TicketStatuses.Find(1).Name;
                                db.TicketHistories.Add(th3);
                            }
                        }
                    }

                }

                if (changed == true)
                {
                    if (ticket.AssignToUserId != null)
                    {
                        Notification n1 = new Notification();
                        n1.Type = "TICKET EDITED";
                        n1.Created = DateTime.Now;
                        n1.CreatedString = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt");
                        n1.Description = "Ticket edited [#" + oldTicket.Id + "]";
                        n1.TicketId = oldTicket.Id;
                        n1.ProjectId = ticket.ProjectId;
                        n1.NotifyUserId = ticket.AssignToUserId;
                        n1.Seen = false;
                        db.Notifications.Add(n1);
                    }

                    Notification n2 = new Notification();
                    n2.Type = "TICKET EDITED";
                    n2.Created = DateTime.Now;
                    n2.CreatedString = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt");
                    n2.Description = "Ticket edited [#" + oldTicket.Id + "]";
                    n2.TicketId = oldTicket.Id;
                    n2.ProjectId = ticket.ProjectId;
                    n2.NotifyUserId = oldTicket.OwnerUserId;
                    n2.Seen = false;
                    db.Notifications.Add(n2);
                }
                db.SaveChanges();

                return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
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
            var user = db.Users.Find(User.Identity.GetUserId());
            var oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
            UserRolesHelper urh = new UserRolesHelper();

            if (ModelState.IsValid)
            {
                db.Tickets.Attach(ticket);
                db.Entry(ticket).Property("Updated").IsModified = true;
                db.Entry(ticket).Property("AssignToUserId").IsModified = true;
                db.Entry(ticket).Property("TicketStatusId").IsModified = true;
                ticket.Updated = DateTime.Now;
                if (ticket.TicketStatusId == 1)
                {
                    ticket.TicketStatusId = 2;
                }

                if (oldTicket.AssignToUserId != ticket.AssignToUserId)
                {
                    if (oldTicket.AssignToUserId != null && ticket.AssignToUserId != null)
                    {
                        //foreach (var n in oldTicket.AssignToUser.Notifications.Where(n => n.TicketId == oldTicket.Id))
                        //{
                        //    if (!urh.IsUserInRole(oldTicket.AssignToUserId,"Admin") && !urh.IsUserInRole(oldTicket.AssignToUserId, "Project Manager") && oldTicket.OwnerUserId != oldTicket.AssignToUserId)
                        //    {
                        //        db.Notifications.Remove(n);
                        //    }
                        //}

                        TicketHistory th = new TicketHistory();
                        th.Property = "ASSIGNMENT CHANGE";
                        th.AuthorId = user.Id;
                        th.TicketId = ticket.Id;
                        th.Created = DateTime.Now;
                        th.OldValue = oldTicket.AssignToUser.FullName;
                        th.NewValue = db.Users.Find(ticket.AssignToUserId).FullName;
                        db.TicketHistories.Add(th);
                        db.SaveChanges();

                        Notification n1 = new Notification();
                        n1.Type = "NEW ASSIGNMENT";
                        n1.Created = DateTime.Now;
                        n1.CreatedString = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt");
                        n1.Description = "New ticket assignment [#" + oldTicket.Id + "]";
                        n1.TicketId = oldTicket.Id;
                        n1.ProjectId = oldTicket.ProjectId;
                        n1.NotifyUserId = ticket.AssignToUserId;
                        n1.Seen = false;
                        db.Notifications.Add(n1);

                        Notification n2 = new Notification();

                        n2.Type = "ASSIGNMENT CHANGE";
                        n2.Created = DateTime.Now;
                        n2.CreatedString = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt");
                        n2.Description = "Ticket assignment changed [#" + oldTicket.Id + "]";
                        n2.TicketId = oldTicket.Id;
                        n2.ProjectId = oldTicket.ProjectId;
                        n2.NotifyUserId = oldTicket.OwnerUserId;
                        n2.Seen = false;
                        db.Notifications.Add(n2);
                    }
                    else if (oldTicket.AssignToUserId == null && ticket.AssignToUserId != null)
                    {
                        TicketHistory th = new TicketHistory();
                        th.Property = "NEW ASSIGNMENT";
                        th.AuthorId = user.Id;
                        th.TicketId = ticket.Id;
                        th.Created = DateTime.Now;
                        th.NewValue = db.Users.Find(ticket.AssignToUserId).FullName;
                        db.TicketHistories.Add(th);
                        db.SaveChanges();

                        Notification n1 = new Notification();
                        n1.Type = "NEW ASSIGNMENT";
                        n1.Created = DateTime.Now;
                        n1.CreatedString = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt");
                        n1.Description = "New ticket assignment [#" + oldTicket.Id + "]";
                        n1.TicketId = oldTicket.Id;
                        n1.ProjectId = oldTicket.ProjectId;
                        n1.NotifyUserId = ticket.AssignToUserId;
                        n1.Seen = false;
                        db.Notifications.Add(n1);

                        Notification n2 = new Notification();
                        n2.Type = "NEW ASSIGNMENT";
                        n2.Created = DateTime.Now;
                        n2.CreatedString = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt");
                        n2.Description = "Your ticket now assigned [#" + oldTicket.Id + "]";
                        n2.TicketId = oldTicket.Id;
                        n2.ProjectId = oldTicket.ProjectId;
                        n2.NotifyUserId = oldTicket.OwnerUserId;
                        n2.Seen = false;
                        db.Notifications.Add(n2);
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
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
            var user = db.Users.Find(User.Identity.GetUserId());
            var oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticketId);

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

                    TicketHistory th = new TicketHistory();
                    th.Property = "NEW ATTACHMENT";
                    th.AuthorId = user.Id;
                    th.TicketId = ticketId;
                    th.Created = DateTime.Now;
                    th.NewValue = attachment.FileUrl;
                    db.TicketHistories.Add(th);

                    Notification n1 = new Notification();
                    n1.Type = "NEW ATTACHMENT";
                    n1.Created = DateTime.Now;
                    n1.CreatedString = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt");
                    n1.Description = "New ticket attachment [#" + oldTicket.Id + "]";
                    n1.TicketId = oldTicket.Id;
                    n1.ProjectId = oldTicket.ProjectId;
                    n1.NotifyUserId = oldTicket.AssignToUserId;
                    n1.Seen = false;
                    db.Notifications.Add(n1);

                    Notification n2 = new Notification();
                    n2.Type = "NEW ATTACHMENT";
                    n2.Created = DateTime.Now;
                    n2.CreatedString = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt");
                    n2.Description = "New ticket attachment [#" + oldTicket.Id + "]";
                    n2.TicketId = oldTicket.Id;
                    n2.ProjectId = oldTicket.ProjectId;
                    n2.NotifyUserId = oldTicket.OwnerUserId;
                    n2.Seen = false;
                    db.Notifications.Add(n2);

                    db.SaveChanges();
                }
            }

            return RedirectToAction("Details", new { id = ticketId });
        }

        // GET: Tickets/Delete/5
        [HttpPost]
        public ActionResult DeleteAttachment(int attachmentId)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(attachmentId);
            if (ticketAttachment != null)
            {
                var oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticketAttachment.TicketId);
                var fileUrl = ticketAttachment.FileUrl;

                TicketHistory th = new TicketHistory();
                th.Property = "ATTACHMENT DELETED";
                th.AuthorId = user.Id;
                th.TicketId = oldTicket.Id;
                th.Created = DateTime.Now;
                th.OldValue = fileUrl;
                db.TicketHistories.Add(th);

                db.TicketAttachments.Remove(ticketAttachment);
                db.SaveChanges();

                if (fileUrl != null)
                {
                    var filePath = Server.MapPath("~/TicketAttachments/" + oldTicket.Id + "/" + fileUrl);
                    System.IO.File.Delete(filePath);
                }
            }

            //Had to do this because Serializing the comment was a problem.
            //Comment had an Author that had comments that all had authors (endless loop)
            TicketAttachment deletedAttachment = new TicketAttachment();
            deletedAttachment.Id = attachmentId;

            return Content(JsonConvert.SerializeObject(deletedAttachment), "application/json");
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddComment(string body, int ticketId)
        //{
        //    var user = db.Users.Find(User.Identity.GetUserId());
        //    var oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticketId);

        //    if (body != null)
        //    {              
        //        TicketComment comment = new TicketComment();
        //        comment.Created = DateTime.Now;
        //        comment.AuthorId = User.Identity.GetUserId();
        //        comment.TicketId = ticketId;
        //        comment.Body = body;
        //        db.TicketComments.Add(comment);
        //        db.SaveChanges();

        //        TicketHistory th = new TicketHistory();
        //        th.Property = "NEW COMMENT";
        //        th.AuthorId = user.Id;
        //        th.TicketId = ticketId;
        //        th.Created = DateTime.Now;
        //        th.NewValue = comment.Body;
        //        db.TicketHistories.Add(th);
        //        db.SaveChanges();
        //    }

        //    return Redirect(Url.Action("Details", "Tickets", new { id = ticketId }) + "#Comments");
        //}

        [HttpPost]
        public ActionResult AddComment(int ticketId, string body)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticketId);

            TicketComment comment = new TicketComment();
            comment.Created = DateTime.Now;
            comment.AuthorId = User.Identity.GetUserId();
            comment.TicketId = ticketId;
            comment.Body = body;
            db.TicketComments.Add(comment);
            db.SaveChanges();

            TicketHistory th = new TicketHistory();
            th.Property = "NEW COMMENT";
            th.AuthorId = user.Id;
            th.TicketId = ticketId;
            th.Created = DateTime.Now;
            th.NewValue = comment.Body;
            db.TicketHistories.Add(th);

            Notification n1 = new Notification();
            n1.Type = "NEW COMMENT";
            n1.Created = DateTime.Now;
            n1.CreatedString = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt");
            n1.Description = "New ticket comment [#" + oldTicket.Id + "]";
            n1.TicketId = oldTicket.Id;
            n1.ProjectId = oldTicket.ProjectId;
            n1.NotifyUserId = oldTicket.AssignToUserId;
            n1.Seen = false;
            db.Notifications.Add(n1);

            Notification n2 = new Notification();
            n2.Type = "NEW COMMENT";
            n2.Created = DateTime.Now;
            n2.CreatedString = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt");
            n2.Description = "New ticket comment [#" + oldTicket.Id + "]";
            n2.TicketId = oldTicket.Id;
            n2.ProjectId = oldTicket.ProjectId;
            n2.NotifyUserId = oldTicket.OwnerUserId;
            n2.Seen = false;
            db.Notifications.Add(n2);

            db.SaveChanges();

            //Simulated comment for Ajax with relational properties
            CreatedComment createdComment = new CreatedComment();
            createdComment.Id = comment.Id;
            createdComment.Body = comment.Body;
            createdComment.Created = comment.Created.ToString("MM/dd/yyyy") + " at " + comment.Created.ToString("h:mm tt");
            createdComment.FullName = user.FullName;
            createdComment.ProfilePic = user.ProfilePic;

            return Content(JsonConvert.SerializeObject(createdComment), "application/json");
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditComment(int commentId, string body)
        //{
        //    var user = db.Users.Find(User.Identity.GetUserId());
        //    TicketComment comment = db.TicketComments.Find(commentId);
        //    var oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == comment.TicketId);

        //    if (body != null)
        //    {
        //        comment.Body = body;
        //        db.TicketComments.Attach(comment);
        //        db.Entry(comment).Property("Body").IsModified = true;
        //        db.SaveChanges();
        //    }
        //    return Redirect(Url.Action("Details", "Tickets", new { id = comment.TicketId }) + "#Comments");
        //}

        [HttpPost]
        public ActionResult EditComment(int commentId, string body)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            TicketComment comment = db.TicketComments.Find(commentId);
            if (comment != null)
            {
                var oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == comment.TicketId);

                if (comment.Body != body)
                {
                    TicketHistory th = new TicketHistory();
                    th.Property = "COMMENT EDITED";
                    th.AuthorId = user.Id;
                    th.TicketId = oldTicket.Id;
                    th.Created = DateTime.Now;
                    th.OldValue = comment.Body;
                    th.NewValue = body;
                    db.TicketHistories.Add(th);
                }

                comment.Body = body;
                comment.Updated = DateTime.Now;
                db.TicketComments.Attach(comment);
                db.Entry(comment).Property("Body").IsModified = true;
                db.Entry(comment).Property("Updated").IsModified = true;
                db.SaveChanges();
            }

            //Had to do this because Serializing the comment was a problem.
            //Comment had an Author that had comments that all had authors (endless loop)
            //MUCH FASTER FOR SOME REASON!
            TicketComment editedComment = new TicketComment();
            editedComment.Id = commentId;
            editedComment.Body = body;

            //return Content(JsonConvert.SerializeObject(comment, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects }), "application/json");
            return Content(JsonConvert.SerializeObject(editedComment), "application/json");
        }

        [HttpPost]
        public ActionResult DeleteComment(int commentId)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            TicketComment comment = db.TicketComments.Find(commentId);
            if (comment != null)
            {
                var oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == comment.TicketId);

                TicketHistory th = new TicketHistory();
                th.Property = "COMMENT DELETED";
                th.AuthorId = user.Id;
                th.TicketId = oldTicket.Id;
                th.Created = DateTime.Now;
                th.OldValue = comment.Body;
                db.TicketHistories.Add(th);

                db.TicketComments.Remove(comment);
                db.SaveChanges();
            }

            return Content(JsonConvert.SerializeObject(comment, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects }), "application/json");
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
            var ticketNotifs = db.Notifications.Where(n => n.TicketId == ticket.Id);
            foreach (var n in ticketNotifs)
            {
                db.Notifications.Remove(n);
            }
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index","Home");
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
