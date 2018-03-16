using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using SSOCommon;

namespace SSOHandlers
{
    /// <summary>
    /// This handler is used to signal current domain that 
    /// authenitcation has successed and that SSO site was
    /// ablo to authenticate user well
    /// </summary>
    public class AuthenticationHandler : IHttpHandler
    {

         #region constructors

        public AuthenticationHandler()
        {
           //logger = LoggingHelper.GetLogger();
        }

        #endregion

        //private ILogger logger;
        private HttpContext context;

        public bool IsReusable
        {
            // To enable pooling, return true here.
            // This keeps the handler in memory.
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            this.context = context;

            string userName = null;
            string returnUrl = null;

            // token from SSO hold data needed for
            //string token = this.context.Request["token"];
            string token = this.context.Request["token"];
            token = Encrypt.DecryptString(token, "MB53xOHjOGZx3JtEbD9ytomtAJJre4M7zL4DjbxJkz0=");

            Uri myUri = new Uri(string.Format("http://www.example.com?{0}", token));
            var queryString = HttpUtility.ParseQueryString(myUri.Query);
            userName = queryString.Get("user");
            returnUrl = queryString.Get("returnUrl");

            this.SetCookie(userName);
            this.context.Response.Redirect(returnUrl!=""?returnUrl: this.context.Request.Url.AbsoluteUri.Replace(this.context.Request.Url.PathAndQuery, ""));

            ////Security.Cryptography.RSACrypto rsaCrypto = new Security.Cryptography.RSACrypto(SSOHandlers.Constants.CertificateName);
            //string rawData = this.context.Request.Url.Query.TrimStart(new char['?']); //rsaCrypto.Decrypt(token);

            //rawData = rawData.TrimStart('?');
            //string[] data = new string[2];

            //int splitIndex = rawData.IndexOf('&');

            //if (splitIndex > 1)
            //{
            //    data[0] = rawData.Substring(0, splitIndex);
            //    data[1] = rawData.Substring(splitIndex + 1, rawData.Length - splitIndex - 1);

            //    string keyValue = System.Net.WebUtility.UrlDecode(data[0]);

            //    userName = System.Net.WebUtility.UrlDecode(keyValue.Split(new char[] { '=' })[1].Trim());

            //    keyValue = System.Net.WebUtility.UrlDecode(data[1]);

            //    if (data[1].Length > 0)
            //    {
            //        returnUrl = data[1].Replace("returnUrl=", "");
            //    }
            //    else
            //    {
            //        returnUrl = this.context.Request.Url.AbsoluteUri.Replace(this.context.Request.Url.PathAndQuery, "");
            //    }

            //    this.SetCookie(userName);

            //    //logger.Log(null, "Authentication handler", "User:" + userName + " ReturnUrl: " + returnUrl, Verbosity.Trace);


            //    // redirect user to page where he wants to be, this url is set in beginig of process of authenitication
            //    //this.context.Response.Redirect("http://localhost:49508/Home/Contact");
            //    this.context.Response.Redirect(returnUrl);
            //}
            //else
            //{
            //    //logger.Log(null, "Authentication handler error.", "User: unknown" + "Error message: splitIndex<1 => returnUrl unknown " , Verbosity.Trace);

            //    throw new NotSupportedException();
            //}
        }

        private void SetCookie(string userName)
        {
            ////get account provider, create instance that is set in config of web site
            //string accountProviderType = System.Configuration.ConfigurationManager.AppSettings.Get(SSOHandlers.Constants.SSOAccountProviderNameKey);
            //IAccountProvider accountProvider = (IAccountProvider)System.Activator.CreateInstance(Type.GetType(accountProviderType));

            //////get user and user data for cookie
            //string userDataForCookie = string.Empty;
            ////System.Security.Principal.IPrincipal principal = accountProvider.GetUserData(userName, out userDataForCookie);
            //var cookieExpirationTime = DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes);

            ////create cookie
            ////var ticket = new FormsAuthenticationTicket(1, principal.Identity.Name, DateTime.Now, cookieExpirationTime, false, userDataForCookie);
            //var ticket = new FormsAuthenticationTicket(1, userName, DateTime.Now, cookieExpirationTime, false, userDataForCookie);

            //string hashTicket = FormsAuthentication.Encrypt(ticket);
            //var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, hashTicket);

            //// set cookie into response
            //this.context.Response.Cookies.Add(authCookie);
            //this.context.Response.Headers.Add("Authorization", "Basic " + hashTicket);




            IdentitySignin("1", userName, null, false);
        }

        

        public void IdentitySignin(string userId, string name, string providerKey = null, bool isPersistent = false)
        {
            var claims = new List<Claim>();

            // create *required* claims
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            claims.Add(new Claim(ClaimTypes.Name, name));

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            // add to user here!
            AuthenticationManager.SignIn(new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = isPersistent,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30)//DateTime.UtcNow.AddDays(7)
            }, identity);
        }

        public void IdentitySignout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie,
                                          DefaultAuthenticationTypes.ExternalCookie);
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
