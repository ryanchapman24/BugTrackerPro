using BugTrackerPro.Models;
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
            return View(users.OrderBy(u => u.User.LastName).ToList());
        }

        //GET: EditUserRoles
        public ActionResult EditUserRoles(string id)
        {
            var user = db.Users.Find(id);
            AdminUserViewModels AdminModel = new AdminUserViewModels();
            UserRolesHelper helper = new UserRolesHelper();
            var selected = helper.ListUserRoles(id);
            AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
            AdminModel.User = user;

            return View(AdminModel);
        }

        //POST: EditUserRoles
        [HttpPost]
        public ActionResult EditUserRoles(AdminUserViewModels model)
        {
            var user = model.User;
            UserRolesHelper helper = new UserRolesHelper();
            foreach (var rolermv in db.Roles.Select(r => r.Id).ToList())
            {
                helper.RemoveUserFromRole(user.Id, rolermv);
            }

            foreach (var roleadd in db.Roles.Select(r => r.Id).ToList())
            {
                helper.AddUserToRole(user.Id, roleadd);
            }

            return RedirectToAction("Index");
        }
    }
}