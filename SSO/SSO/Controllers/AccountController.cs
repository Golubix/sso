using SSO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using SSO.DBAccess;
using SSO.DBAccess.Entities;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using SSO.Globals;
using SSOCommon;
using Common;

namespace SSO.Controllers
{
    public class AccountController : Controller
    {

        #region Constructors

        public AccountController()
        {
           //logger = LoggingHelper.GetLogger();
        }

        #endregion

        #region Constants
            
        #endregion

        //private ILogger logger;

        public SSO.Globals.Enums.Protocol Protocol
        {
            get
            {
                return (Request.IsSecureConnection ? SSO.Globals.Enums.Protocol.https : SSO.Globals.Enums.Protocol.http);
            }
        }

        public ActionResult Index()
        {
            //logger.Log(null, "DifficultyFactorLabour conversion", "test message", Verbosity.Trace);

            //Check for the existence of the auth cookie (on SSO) for the received domain.
            var tmpLoginModel = new ScrAccount();
            string languageCode = "nl-BE";
            tmpLoginModel.SetLanguage(languageCode);

            if (Request.QueryString.Count >= 2)
            {
                tmpLoginModel.ResponseDomain = Request.QueryString["domain"];
                tmpLoginModel.ResponseRelativePath = Request.QueryString["ReturnUrl"];

                HttpCookie currDomainCookie = HttpContext.Request.Cookies.Get(tmpLoginModel.ResponseDomain);
                if (currDomainCookie != null)
                {
                    string userName = currDomainCookie["userName"];

                    try
                    {

                        //The cookie already exists. 
                        //1. Refresh the cookies for all domains for which the user has the access rights.
                        CreateOrRefreshAllCookies(tmpLoginModel.ResponseDomain, userName);
                    }catch(Exception ex)
                    {
                        //logger.Log(null, "Index error on CreateOrRefreshAllCookies", "User:" + tmpLoginModel.UserName + " Request for domain: " + tmpLoginModel.ResponseDomain + "Error message:" + ex.Message, Verbosity.Trace);

                    }
                    //2. Navigate to the source domain create cookie handler.
                    //Redirect
                    string url = string.Format("{0}://{1}/{2}?user={3}&returnUrl={4}", Protocol.ToString(), tmpLoginModel.ResponseDomain, "auth.login", System.Net.WebUtility.UrlEncode(userName), tmpLoginModel.ResponseRelativePath);

                    //logger.Log(null, "Already logged in. Navigate to the authentication handler of the domain.", "User:" + tmpLoginModel.UserName + " Request for domain: " + tmpLoginModel.ResponseDomain, Verbosity.Trace);

                    Response.Redirect(url);
                }
            }

            if (Request.QueryString.Count >= 3)
            {
                string err = Request.QueryString["err"];

                if (err != null)
                {
                    tmpLoginModel.IsValid = false;
                    ModelState.AddModelError("Password change failed!", err);
                }
            }

            //The SSO cookie doesn't exist.
            //1. Login page SSO.
            string viewName = (tmpLoginModel.ResponseDomain == null ? "Index" : GetLoginViewName(tmpLoginModel.ResponseDomain));

            return View(viewName, tmpLoginModel);
        }

        [HttpGet]
        public ActionResult Login()
        {
            var tmpLoginModel = new ScrAccount();

            return View("Login", tmpLoginModel);
        }
        [HttpPost]
        public ActionResult Login(FormCollection formCollection)
        {
            var tmpLoginModel = new ScrAccount();
            TryUpdateModel(tmpLoginModel, formCollection);

            string token = string.Format("username={0}&password={1}&returnurl={2}&sso={3}",
                System.Net.WebUtility.UrlEncode(tmpLoginModel.UserName),
                System.Net.WebUtility.UrlEncode(tmpLoginModel.Password),
                System.Net.WebUtility.UrlEncode(tmpLoginModel.ResponseRelativePath),
                System.Net.WebUtility.UrlEncode(this.HttpContext.Request.Url.Authority));

            token = Encrypt.EncryptString(token, Parameters.ENCRYPTION_KEY);
            
            //string certificate = System.Configuration.ConfigurationManager.AppSettings.Get("SSOCerfticate");
            //Security.Cryptography.RSACrypto rsaCrypto = rsaCrypto = new Security.Cryptography.RSACrypto(certificate);
            //token = rsaCrypto.Encrypt(token);

            token = System.Net.WebUtility.UrlEncode(token);
           
            //logger.Log(null, "Call to login handler", "User:" + tmpLoginModel.UserName + " Request for domain: " + tmpLoginModel.ResponseDomain, Verbosity.Trace);

            string url = string.Format("{2}://{0}/chk.login?token={1}",
                tmpLoginModel.ResponseDomain,
                token,
                Protocol.ToString());

            //1. Navigate to the login http handler.
            return Redirect(url);
        }

        [HttpGet]
        public ActionResult ValidationResult()
        {
            var tmpLoginModel = new ScrAccount();

            //string language = System.Net.WebUtility.UrlDecode(this.HttpContext.Request["language"]);
            //if (!string.IsNullOrEmpty(language))
            //{
            //    tmpLoginModel.SetLanguage(language);
            //}

            string userName = System.Net.WebUtility.UrlDecode(this.HttpContext.Request["username"]);
            bool isValid = Convert.ToBoolean(this.HttpContext.Request["isValid"]);
            tmpLoginModel.ResponseDomain = this.HttpContext.Request["domain"];
            tmpLoginModel.ResponseRelativePath = this.HttpContext.Request["returnurl"];

            string viewName = (tmpLoginModel.ResponseDomain == null ? "Index" : GetLoginViewName(tmpLoginModel.ResponseDomain));

            //  logger.Log(null, "Validation result received", "User:" + userName + " Request for domain: " + tmpLoginModel.ResponseDomain + "Is valid? " + isValid, Verbosity.Trace);

            if (isValid == true)
            {
                try
                {
                    try
                    {
                        CreateOrRefreshAllCookies(tmpLoginModel.ResponseDomain, userName);
                    }
                    catch (Exception ex)
                    {
                        //logger.Log(null, "Validation result error on CreateOrRefreshAllCookies", "User:" + userName + " Request for domain: " + tmpLoginModel.ResponseDomain + "Error message:" + ex.Message, Verbosity.Trace);

                        string errMessage = string.Format("{0} - {1}", ex.Message, ex.StackTrace);
                        ModelState.AddModelError("error!", errMessage);
                        //return View("Index", tmpLoginModel);
                        return View(viewName, tmpLoginModel);
                    }
                    //Redirect to http handler for creating cookie on domain (e.g. dca,claimshub) and navigation to the requested domain page.
                    //redirect user to authentication handler
                    string token = string.Format("user={0}&returnUrl={1}", userName, tmpLoginModel.ResponseRelativePath);
                    token = Encrypt.EncryptString(token, Parameters.ENCRYPTION_KEY);
                    token = System.Net.WebUtility.UrlEncode(token);

                    string url = string.Format("{0}://{1}/{2}?token={3}", Protocol.ToString(), tmpLoginModel.ResponseDomain, "auth.login", token);

                    //logger.Log(null, "Validation result. Redirect to authentication handler.", "User:" + userName + " Request for domain: " + tmpLoginModel.ResponseDomain, Verbosity.Trace);

                    Response.Redirect(url);
                }
                catch (Exception ex)
                {
                    //logger.Log(null, "Validation result error. Not correctly mapped.", "User:" + userName + " Request for domain: " + tmpLoginModel.ResponseDomain + "Error message:" + ex.Message, Verbosity.Trace);

                    string errMessage = string.Format("{0} is not mapped correctly. Contact your administrator for help.", tmpLoginModel.ResponseDomain);
                    ModelState.AddModelError("Domain not mapped!", errMessage);
                }
            }

            tmpLoginModel.IsValid = false;

            ModelState.AddModelError("InvalidCredentials", "Log in failed. Invalid credentials!");

            //return View("Index", tmpLoginModel);
            return View(viewName, tmpLoginModel);
        }

        [HttpGet]
        public ActionResult Logout(string keyResponseDomainSite, string userName)
        {
            ScrLogout model = new ScrLogout(keyResponseDomainSite);
            //For each domain that user can see we need to add image tag with source that is pointing out to signout handler that will return image after clean up
            //only exception in domain list is domain that was initialy used to signout.


            IDBDataAccess db = new DBDataAccess();

            UserMappings currentAccount = db.GetUsermappings(keyResponseDomainSite, userName);

            if (currentAccount != null)
            {
                List<UserMappings> domainUserAccounts = db.GetUsermappings(currentAccount.UserAccountID);

                foreach (UserMappings userMappings in domainUserAccounts)
                {
                    if (userMappings.DomainName != keyResponseDomainSite)
                    {
                        model.DomainNames.Add(userMappings.DomainName);
                    }
                }
            }

            string[] cookies = this.Request.Cookies.AllKeys;
            foreach (var cookie in cookies)
            {
                this.Request.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
                this.Response.Cookies.Add(this.Request.Cookies[cookie]);
            }

            return View(model);
        }

        #region Helper methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainSite">E.g. localhost:63256 or dcatest.claimshub.eu</param>
        /// <param name="sDomainSource">E.g. DCA or CLAIMSHUB</param>
        /// <param name="um"></param>
        //private void CreateOrRefreshCookie(string domainSite,string sDomainSource, UserMappings um)
        private void CreateOrRefreshCookie(UserMappings um)
        {
            HttpCookie cookie = HttpContext.Request.Cookies.Get(um.DomainName);
            if (cookie == null)
            {
                //Create cookie.
                cookie = new HttpCookie(um.DomainName);
                //cookie["domainSource"] = sDomainSource;
                cookie["userName"] = um.DomainUsername;
                cookie.Expires = DateTime.Now.AddMinutes(60);

                HttpContext.Response.Cookies.Add(cookie);
            }
            else
            {
                //Refresh cookie.
                cookie.Expires = DateTime.Now.AddMinutes(60);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyResponseDomainSite">E.g. localhost:63256 or dcatest.claimshub.eu</param>
        /// <param name="userName">Username for the keyResponseDomainSite</param>
        private void CreateOrRefreshAllCookies(string keyResponseDomainSite, string userName)
        {
            IDBDataAccess db = new DBDataAccess();

            UserMappings currentAccount = db.GetUsermappings(keyResponseDomainSite, userName);

            if (currentAccount != null)
            {
                List<UserMappings> domainUserAccounts = db.GetUsermappings(currentAccount.UserAccountID);

                foreach (UserMappings userMappings in domainUserAccounts)
                {
                    CreateOrRefreshCookie(userMappings);
                }

            }
        }

        private string GetLoginViewName(string responseDomain)
        {
            System.Collections.Hashtable section =
                (System.Collections.Hashtable)ConfigurationManager.GetSection("SSODomainMappings");

            return (section[responseDomain] != null ? section[responseDomain].ToString() : "");
        }

        #endregion
    }
}

