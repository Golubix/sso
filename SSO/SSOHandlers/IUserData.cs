using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSOHandlers
{
    public interface IAccountProvider
    {
        System.Security.Principal.IPrincipal GetUserData(string userName, out string userData);
    }
}
