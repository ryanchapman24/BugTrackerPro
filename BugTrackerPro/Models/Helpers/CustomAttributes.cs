﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BugTrackerPro.Models.Helpers;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace BugTrackerPro.Models.Helpers
{
    public class RequireUnlockedAccount : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }
            using (var db = new ApplicationDbContext())
            {
                var userId = httpContext.User.Identity.GetUserId();
                var user = db.Users.AsNoTracking().First(u => u.Id == userId);
                var userStatus = user.Locked;
                if (userStatus == false)
                {
                    return true;
                }
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new
                {
                    controller = "Account",
                    action = "Locked",
                    returnUrl = filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
                }));
            }
        }
    }

    public class AjaxOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
                filterContext.HttpContext.Response.Redirect("/Home/Error");
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }
    }
}