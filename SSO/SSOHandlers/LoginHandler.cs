using SSOCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace SSOHandlers
{
    public class LoginHandler : IHttpHandler
    {
        #region constructors

        public LoginHandler()
        {
           //logger = LoggingHelper.GetLogger();
        }

        #endregion

        //private ILogger logger;

        public bool IsReusable
        {
            // To enable pooling, return true here.
            // This keeps the handler in memory.
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            //if (Debugger.IsAttached) //or if(!Debugger.IsAttached)
            //{
                //Debugger.Break();
            //}
//#if DEBUG
            //System.Diagnostics.Debugger.Break();
//#endif
            string userName = "";
            string retu = "";
            string sso = "";

            try
            {

                HttpRequest Request = context.Request;
                HttpResponse Response = context.Response;

                //string certificate = System.Configuration.ConfigurationManager.AppSettings.Get("SSOCerfticate");

                //Security.Cryptography.RSACrypto rsaCrypto = new Security.Cryptography.RSACrypto(certificate);

                string token = Request["token"];
                token = Encrypt.DecryptString(token, "MB53xOHjOGZx3JtEbD9ytomtAJJre4M7zL4DjbxJkz0=");
                //token = rsaCrypto.Decrypt(token);

                //token = HttpUtility.UrlDecode(token);

                Uri myUri = new Uri(string.Format("http://www.example.com?{0}", token));
                var queryString = HttpUtility.ParseQueryString(myUri.Query);

                userName = queryString.Get("username");
                string password = queryString.Get("password");
                retu = queryString.Get("returnurl");
                sso = queryString.Get("sso");

                //logger.Log(null, "Login handler request received",
                //    "User:" + userName + " Return url: " + retu + "SSO: " + sso, Verbosity.Trace);

                //Use membership provider.
                //bool isValid = Membership.ValidateUser(userName, password);
                bool isValid = userName == "ngolub" && password == "ngolub";

                //logger.Log(null, "Login handler request. Navigate to SSO validation result.",
                //    "User:" + userName + " Return url: " + retu + "SSO: " + sso + "Is valid? "+isValid, Verbosity.Trace);

                //string url = "http://" + sso + "/Account/ValidationResult?username=" + userName + "&isValid=" + isValid + "&domain=" + Request.Url.Authority + "&returnurl=" + retu;
                string protocol = context.Request.IsSecureConnection ? "https" : "http";
                string url =
                    string.Format(
                        "{5}://{0}/Account/ValidationResult?username={1}&isValid={2}&domain={3}&returnurl={4}",
                        sso,
                        System.Net.WebUtility.UrlEncode(userName),
                        isValid,
                        Request.Url.Authority,
                        System.Web.HttpUtility.UrlEncode(retu),
                        protocol);

                context.Response.Redirect(url, false);
            }
            catch (Exception ex)
            {
                //logger.Log(null, "Error Login Handler",
                //    "User:" + userName + " Return url: " + retu + "SSO: " + sso+" " + ex.Message, Verbosity.Trace);
            }

        }
    }
}
