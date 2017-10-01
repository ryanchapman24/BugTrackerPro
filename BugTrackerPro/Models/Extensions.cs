using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace BugTrackerPro.Models
{
    public static class Extensions
    {
        public static string GetUserFullName(this IIdentity user)
        {
            var claimsIdentity = (ClaimsIdentity)user;
            var FullNameClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "UserFullName");

            if (FullNameClaim != null)
            {
                return FullNameClaim.Value;
            }
            else
            {
                return null;
            }
        }
    }
}