﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerPro.Models.CodeFirst
{
    public class Project
    {
        protected internal ApplicationDbContext db = new ApplicationDbContext();

        public Project()
        {
            Users = new HashSet<ApplicationUser>();
            Tickets = new HashSet<Ticket>();
        }

        public int Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AuthorId { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }

        public string AuthorFullName
        {
            get
            {
                var author = db.Users.FirstOrDefault(u => u.Id == AuthorId);
                if (author != null)
                {
                    return author.FullName;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}