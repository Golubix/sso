using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSO.DBAccess
{
    public static class StoredProcedures
    {
        public const string GetUsermappings = "[dbo].[GetUsermappings]";
        public const string GetUsermappingsByUser = "[dbo].[GetUsermappingsByUser]";
    }
}