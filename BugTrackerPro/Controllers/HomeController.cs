using BugTrackerPro.Models;
using BugTrackerPro.Models.CodeFirst;
using BugTrackerPro.Models.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BugTrackerPro.Controllers
{
    public class LayoutNotifs
    {
        public int Count { get; set; }
        public IEnumerable<Notification> Notifications { get; set; }
    }

    [Authorize]
    [RequireUnlockedAccount]
    public class HomeController : Universal
    {
        private ApplicationSignInManager _signInManager;

        public HomeController()
        {
        }

        public HomeController(ApplicationSignInManager signInManager)
        {
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ProjectsAndTicketsViewModels model = new ProjectsAndTicketsViewModels();

            if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
            {
                model.Projects = db.Projects.OrderBy(p => p.Title).ToList();

                if (User.IsInRole("Admin"))
                {
                    model.Tickets = db.Tickets.OrderByDescending(t => t.Id).ToList();
                }
                else
                {
                    model.Tickets = user.Projects.SelectMany(p => p.Tickets).OrderByDescending(t => t.Id).ToList();
                }
            }
            else if (User.IsInRole("Developer") || User.IsInRole("Submitter"))
            {
                model.Projects = user.Projects.OrderBy(p => p.Title).ToList();

                if (User.IsInRole("Developer"))
                {
                    model.Tickets = db.Tickets.Where(t => t.Project.Users.Any(u => u.Id == user.Id)).OrderByDescending(t => t.Id).ToList();
                }
                else
                {
                    model.Tickets = db.Tickets.Where(t => t.OwnerUserId == user.Id).OrderByDescending(t => t.Id).ToList();
                }
            }
            return View(model);
        }

        public ActionResult Error()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetNotifications(int nCount)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            
            var notifs = user.Notifications.Where(n => n.Seen == false).OrderByDescending(n => n.Id);
            LayoutNotifs ln = new LayoutNotifs();
            ln.Count = notifs.Count();
            ln.Notifications = notifs.Take(ln.Count - nCount);

            return Content(JsonConvert.SerializeObject(ln, Formatting.Indented, new JsonSerializerSettings {PreserveReferencesHandling = PreserveReferencesHandling.Objects}), "application/json");
        }

        public ActionResult ViewNotification(int id)
        {
            var notification = db.Notifications.Find(id);
            notification.Seen = true;
            db.SaveChanges();

            if (notification.Type.Contains("PROJECT"))
            {
                return RedirectToAction("Details", "Projects", new { id = notification.ProjectId });
            }
            else
            {
                return RedirectToAction("Details", "Tickets", new { id = notification.TicketId });
            }
        }

        [HttpPost]
        public ActionResult ClearNotifications()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            foreach (var notification in user.Notifications.Where(n => n.Seen == false))
            {
                notification.Seen = true;
                db.SaveChanges();
            }

            var response = true;

            return Content(JsonConvert.SerializeObject(response), "application/json");
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