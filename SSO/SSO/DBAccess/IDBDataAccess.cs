using SSO.DBAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSO.DBAccess
{
    public interface IDBDataAccess
    {
        //UserMappings GetUsermappings(string username, SSO.Globals.Enums.DomainSource source);

        UserMappings GetUsermappings(string domainName, string domainUsername);

        List<UserMappings> GetUsermappings(int userAccountID);
    }
}
