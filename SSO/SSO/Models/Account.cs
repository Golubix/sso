using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSO.Models
{
    public class ScrAccount: ScrBase
    {
        public ScrAccount() : base() {

        }
        public string UserName { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// Site domain e.g. "localhost:62650" or "dcatest.claimshub.eu".
        /// </summary>
        public string ResponseDomain { get; set; }

        private string responseDomainUrl;

        public string ResponseDomainUrl
        {
            get {
                if (string.IsNullOrEmpty(this.responseDomainUrl))
                {
                    string protocol = System.Web.HttpContext.Current.Request.IsSecureConnection ? "https" : "http";
                    this.responseDomainUrl = string.Format("{0}://{1}", protocol, this.ResponseDomain);
                }
                return this.responseDomainUrl;
            }
        }

        /// <summary>
        /// "/Claim/CalculationBasket/1061"
        /// </summary>
        public string ResponseRelativePath { get; set; }

        public bool IsValid { get; set; }

    }
}