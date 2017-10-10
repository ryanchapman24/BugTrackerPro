using BugTrackerPro.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerPro.Models
{
    public class ProjectsAndTicketsViewModels
    {
        public List<Project> Projects { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}