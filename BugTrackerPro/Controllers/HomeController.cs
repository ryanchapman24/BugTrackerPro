using BugTrackerPro.Models;
using BugTrackerPro.Models.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BugTrackerPro.Controllers
{
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