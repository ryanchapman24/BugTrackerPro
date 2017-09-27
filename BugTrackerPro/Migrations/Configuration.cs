namespace BugTrackerPro.Migrations
{
    using BugTrackerPro.Models;
    using BugTrackerPro.Models.CodeFirst;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTrackerPro.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTrackerPro.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            var userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "rchapman@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "rchapman@coderfoundry.com",
                    Email = "rchapman@coderfoundry.com",
                    FirstName = "Ryan",
                    LastName = "Chapman",
                }, "Password1!");
            }
            var userId_Admin = userManager.FindByEmail("rchapman@coderfoundry.com").Id;
            userManager.AddToRole(userId_Admin, "Admin");

            if (!context.Users.Any(u => u.Email == "admin@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "admin@coderfoundry.com",
                    Email = "admin@coderfoundry.com",
                    FirstName = "Admin",
                    LastName = "Demo",
                }, "Password1!");
            }
            var userId_a = userManager.FindByEmail("admin@coderfoundry.com").Id;
            userManager.AddToRole(userId_a, "Admin");

            if (!context.Users.Any(u => u.Email == "projectmanager@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "projectmanager@coderfoundry.com",
                    Email = "projectmanager@coderfoundry.com",
                    FirstName = "Project Manager",
                    LastName = "Demo",
                }, "Password1!");
            }
            var userId_pm = userManager.FindByEmail("projectmanager@coderfoundry.com").Id;
            userManager.AddToRole(userId_pm, "Project Manager");

            if (!context.Users.Any(u => u.Email == "developer@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "developer@coderfoundry.com",
                    Email = "developer@coderfoundry.com",
                    FirstName = "Developer",
                    LastName = "Demo",
                }, "Password1!");
            }
            var userId_d = userManager.FindByEmail("developer@coderfoundry.com").Id;
            userManager.AddToRole(userId_d, "Developer");

            if (!context.Users.Any(u => u.Email == "submitter@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "submitter@coderfoundry.com",
                    Email = "submitter@coderfoundry.com",
                    FirstName = "Submitter",
                    LastName = "Demo",
                }, "Password1!");
            }
            var userId_s = userManager.FindByEmail("submitter@coderfoundry.com").Id;
            userManager.AddToRole(userId_s, "Submitter");

            if (!context.TicketPriorities.Any(p => p.Name == "Low"))
            {
                var priority = new TicketPriority();
                priority.Name = "Low";
                context.TicketPriorities.Add(priority);
            }

            if (!context.TicketPriorities.Any(p => p.Name == "Medium"))
            {
                var priority = new TicketPriority();
                priority.Name = "Medium";
                context.TicketPriorities.Add(priority);
            }

            if (!context.TicketPriorities.Any(p => p.Name == "High"))
            {
                var priority = new TicketPriority();
                priority.Name = "High";
                context.TicketPriorities.Add(priority);
            }

            if (!context.TicketPriorities.Any(p => p.Name == "Critical"))
            {
                var priority = new TicketPriority();
                priority.Name = "Critical";
                context.TicketPriorities.Add(priority);
            }

            if (!context.TicketStatuses.Any(p => p.Name == "Unassigned"))
            {
                var status = new TicketStatus();
                status.Name = "Unassigned";
                context.TicketStatuses.Add(status);
            }

            if (!context.TicketStatuses.Any(p => p.Name == "Assigned"))
            {
                var status = new TicketStatus();
                status.Name = "Assigned";
                context.TicketStatuses.Add(status);
            }

            if (!context.TicketStatuses.Any(p => p.Name == "In Progress"))
            {
                var status = new TicketStatus();
                status.Name = "In Progress";
                context.TicketStatuses.Add(status);
            }

            if (!context.TicketStatuses.Any(p => p.Name == "Complete"))
            {
                var status = new TicketStatus();
                status.Name = "Complete";
                context.TicketStatuses.Add(status);
            }

            if (!context.TicketTypes.Any(p => p.Name == "Hardware"))
            {
                var type = new TicketType();
                type.Name = "Hardware";
                context.TicketTypes.Add(type);
            }

            if (!context.TicketTypes.Any(p => p.Name == "Software"))
            {
                var type = new TicketType();
                type.Name = "Software";
                context.TicketTypes.Add(type);
            }
        }
    }
}
