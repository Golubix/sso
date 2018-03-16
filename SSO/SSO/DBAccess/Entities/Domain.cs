using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSO.DBAccess.Entities
{
    public class Domain
    {
        public Int32 DomainID
        {
            get;
            set;
        }

        public String DomainName
        {
            get;
            set;
        }

        public String Description
        {
            get;
            set;
        }
    }
}