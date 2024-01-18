using BTLClient_Server.EF;
using LTTH_UI_UX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BTLClient_Server.Areas.Admin.Security
{
    public class IsAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            User u = (User)filterContext.HttpContext.Session.Contents["user"];
            if (u.quyen != 0)
            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "AdminUser", action = "Profile", Area = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }
}