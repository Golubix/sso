using SSO.DBAccess.Entities;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace SSO.DBAccess
{
    public class DBDataAccess : IDBDataAccess
    {

        public DBDataAccess()
        {
            connectionString = ConfigurationManager.ConnectionStrings["SSOSqlConnection"].ConnectionString;
        }

        private string connectionString;
        private IDbConnection dbConnection;

        #region Implementation of IDBDataAccess

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainName">Domain.DomainName e.g. localhost:62650 or dcatest.claimshub.eu .</param>
        /// <param name="domainUsername">DomainUserAccount.DomainUsername is user's username on the domain. </param>
        /// <returns></returns>
        public UserMappings GetUsermappings(string domainName, string domainUsername)
        {
            UserMappings tmpUserMappings = null;

            using (IDbConnection tmpConnection = GetConnection(null))
            {
                var parameters = new
                {
                    DomainName = domainName,
                    DomainUsername = domainUsername
                };

                tmpUserMappings = tmpConnection.Query<UserMappings>(StoredProcedures.GetUsermappings,
                                                        parameters,
                                                        commandType: CommandType.StoredProcedure
                                                      ).FirstOrDefault();
            }

            return tmpUserMappings;
        }

        public List<UserMappings> GetUsermappings(int userAccountID)
        {
            List<UserMappings> tmpUserMappings = null;

            using (IDbConnection tmpConnection = GetConnection(null))
            {
                var parameters = new
                {
                    UserAccountID = userAccountID
                };

                tmpUserMappings = tmpConnection.Query<UserMappings>(StoredProcedures.GetUsermappingsByUser,
                                                        parameters,
                                                        commandType: CommandType.StoredProcedure
                                                      ).ToList();
            }
            return tmpUserMappings;
        }

        #endregion

        #region Helper Methods

        public IDbConnection GetConnection(string connectionString)
        {
            var tmpConnectionString = connectionString ?? this.connectionString;
            var tmpConnection = new SqlConnection(tmpConnectionString);
            dbConnection = new StackExchange.Profiling.Data.ProfiledDbConnection(tmpConnection, MiniProfiler.Current);

            try
            {
                dbConnection.Open();
            }
            catch (Exception ex)
            {
                dbConnection.Dispose();
                throw;
            }
            return dbConnection;
        }

        #endregion

    }
}