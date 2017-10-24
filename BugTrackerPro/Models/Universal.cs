using BugTrackerPro.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTrackerPro.Models
{
    public class Universal : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                ViewBag.FirstName = user.FirstName;
                ViewBag.LastName = user.LastName;
                ViewBag.FullName = user.FullName;
                ViewBag.ProfilePic = user.ProfilePic;

                var tickets = new List<Ticket>();

                if (User.IsInRole("Admin"))
                {
                    ViewBag.MyProjects = db.Projects.OrderBy(p => p.Title).ToList();
                }
                else
                {
                    ViewBag.MyProjects = user.Projects.OrderBy(p => p.Title).ToList();
                }

                if (User.IsInRole("Admin"))
                {
                    tickets = db.Tickets.ToList();
                }
                else if (User.IsInRole("Project Manager"))
                {
                    tickets = user.Projects.SelectMany(p => p.Tickets).ToList();
                }
                else if (User.IsInRole("Developer"))
                {
                    tickets = db.Tickets.Where(t => t.AssignToUserId == user.Id).ToList();
                }
                else if (User.IsInRole("Submitter"))
                {
                    tickets = db.Tickets.Where(t => t.OwnerUserId == user.Id).ToList();
                }

                ViewBag.MyTickets = tickets;

                if (tickets.Count > 0)
                {
                    ViewBag.TicketCompletion = Convert.ToInt32(100 * (Convert.ToDecimal(tickets.Where(t => t.TicketStatusId == 4).Count()) / Convert.ToDecimal(tickets.Count())));
                }
                else
                {
                    ViewBag.TicketCompletion = 0;
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}