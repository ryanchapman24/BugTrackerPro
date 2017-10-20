using BugTrackerPro.Models;
using BugTrackerPro.Models.CodeFirst;
using BugTrackerPro.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTrackerPro.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Universal
    {
        // GET: Admin
        public ActionResult Index()
        {
            List<AdminUserViewModels> users = new List<AdminUserViewModels>();
            UserRolesHelper helper = new UserRolesHelper();
            foreach (var user in db.Users.ToList())
            {
                var eachUser = new AdminUserViewModels();
                eachUser.User = user;
                eachUser.SelectedRoles = helper.ListUserRoles(user.Id).ToArray();

                users.Add(eachUser);
            }
            return View(users.OrderBy(u => u.User.FirstName).ToList());
        }

        //GET: EditUserRoles
        public ActionResult EditUserRoles(string id)
        {
            var user = db.Users.Find(id);
            var helper = new UserRolesHelper();
            var model = new AdminUserViewModels();
            model.User = user;
            model.SelectedRoles = helper.ListUserRoles(id).ToArray();
            model.Roles = new MultiSelectList(db.Roles, "Name", "Name", model.SelectedRoles);

            return View(model);
        }

        //POST: EditUserRoles
        [HttpPost]
        public ActionResult EditUserRoles(AdminUserViewModels model)
        {
            var user = db.Users.Find(model.User.Id);
            UserRolesHelper helper = new UserRolesHelper();
            var wasDev = helper.IsUserInRole(user.Id, "Developer");

            foreach (var role in db.Roles.Select(r => r.Name).ToList())
            {
                helper.RemoveUserFromRole(user.Id, role);
            }

            if (model.SelectedRoles != null)
            {
                foreach (var role in model.SelectedRoles)
                {
                    helper.AddUserToRole(user.Id, role);
                }
            }

            if (wasDev == true && (model.SelectedRoles == null || !model.SelectedRoles.Contains("Developer")))
            {
                foreach (var ticket in db.Tickets.AsNoTracking().Where(t => t.AssignToUserId == user.Id).ToList())
                {
                    var oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
                    ticket.AssignToUserId = null;

                    TicketHistory th = new TicketHistory();
                    th.Property = "ASSIGNMENT REMOVED (Developer no longer in role)";
                    th.AuthorId = user.Id;
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