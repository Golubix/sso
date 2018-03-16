using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;

namespace SSOHandlers
{
    public class SignOutHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                // To enable pooling, return true here.
                // This keeps the handler in memory.
                return true;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            //if (context.Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            //{
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //FormsAuthentication.SignOut();
            //if (context.Session != null)
            //{
            //    context.Session.Abandon();
            //}

            context.Response.ContentType = "text/image"; ;
            var absolutePath = context.Server.MapPath("~/Content/Images/logout.png");
            byte[] buffer = System.IO.File.ReadAllBytes(absolutePath);

            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            //}
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }
    }
}
