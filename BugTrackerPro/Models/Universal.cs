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

                if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
                {
                    ViewBag.MyProjects = db.Projects.OrderBy(p => p.Title).ToList();
                }
                else
                {
                    ViewBag.MyProjects = user.Projects.OrderBy(p => p.Title).ToList();
                }
            }
            base.OnActionExecuted(filterContext);
        }
    }
}