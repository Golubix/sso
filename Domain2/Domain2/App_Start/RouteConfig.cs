using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Domain1
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*chklogin}", new { chklogin = @"(.*/)?chk.login(/.*)?" });
            routes.IgnoreRoute("{*authlogin}", new { authlogin = @"(.*/)?auth.login(/.*)?" });
            routes.IgnoreRoute("{*authlogout}", new { authlogout = @"(.*/)?auth.logout(/.*)?" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
