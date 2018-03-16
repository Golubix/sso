using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSO.DBAccess.Entities
{
    public class UserMappings
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

        public Int32 DomainUserAccountID
        {
            get;
            set;
        }

        public String DomainUsername
        {
            get;
            set;
        }

        public Int32 UserAccountID
        {
            get;
            set;
        }

        public String UserName
        {
            get;
            set;
        }

    }
}