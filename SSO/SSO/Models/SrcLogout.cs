using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSO.Models
{
    public class ScrLogout : ScrBase
    {
        public ScrLogout() : base()
        {
            this.DomainNames = new List<string>();
        }

        public ScrLogout(string keyResponseDomainSite)
            : this()
        {
            this.KeyResponseDomainSite = keyResponseDomainSite;
        }

        public List<string> DomainNames { get; set; }
        //public string GetLogoutLink(string domain)
        //{

        //}

        public string KeyResponseDomainSite { get; set; }
    }
}