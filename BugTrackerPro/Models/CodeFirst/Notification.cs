using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerPro.Models.CodeFirst
{
    public class Notification
    {
        public int Id { get; set; }
        public int? TicketId { get; set; }
        public int? ProjectId { get; set; }
        public DateTimeOffset Created { get; set; }
        public string CreatedString { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string NotifyUserId { get; set; }
        public bool Seen { get; set; }

        public virtual ApplicationUser NotifyUser { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual Project Project { get; set; }
    }
}