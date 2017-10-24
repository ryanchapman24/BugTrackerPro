using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BugTrackerPro.Models.CodeFirst;
using System.Collections.Generic;

namespace BugTrackerPro.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePic { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public ApplicationUser()
        {
            Projects = new HashSet<Project>();
            Histories = new HashSet<TicketHistory>();
            Comments = new HashSet<TicketComment>();
            Attachments = new HashSet<TicketAttachment>();
            //OwnerUserTickets = new HashSet<Ticket>();
            //AssignToUserTickets = new HashSet<Ticket>();
        }

        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<TicketHistory> Histories { get; set; }
        public virtual ICollection<TicketComment> Comments { get; set; }
        public virtual ICollection<TicketAttachment> Attachments { get; set; }
        //public virtual ICollection<Ticket> OwnerUserTickets { get; set; }
        //public virtual ICollection<Ticket> AssignToUserTickets { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("UserFullName", FullName));
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Ticket>()
        //                .HasRequired(t => t.OwnerUser)
        //                .WithMany(t => t.OwnerUserTickets)
        //                .HasForeignKey(t => t.OwnerUserId)
        //                .WillCascadeOnDelete(false);

        //    modelBuilder.Entity<Ticket>()
        //                .HasRequired(t => t.AssignToUser)
        //                .WithMany(t => t.AssignToUserTickets)
        //                .HasForeignKey(t => t.AssignToUserId)
        //                .WillCascadeOnDelete(false);
        //}

        public DbSet<Project> Projects { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketAttachment> TicketAttachments { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
        public DbSet<TicketHistory> TicketHistories { get; set; }
        public DbSet<TicketPriority> TicketPriorities { get; set; }
        public DbSet<TicketStatus> TicketStatuses { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }
    }
}