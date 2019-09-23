using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace FiiiPay.Foundation.Data
{
    /// <summary>
    /// Class FiiiPay.Foundation.Data.BaseFoundationDataAccess
    /// </summary>
    public abstract class BaseFoundationDataAccess
    {
        private static readonly string _foundationConStr = ConfigurationManager.ConnectionStrings["foundation"].ConnectionString;

        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        protected int PageSize => Convert.ToInt32(ConfigurationManager.AppSettings.Get("PageSize") ?? "10");

        /// <summary>
        /// Reads the connection.
        /// </summary>
        /// <returns></returns>
        public virtual IDbConnection ReadConnection()
        {
            var sqlConnection = new SqlConnection(_foundationConStr);
            sqlConnection.Open();

            return sqlConnection;
        }

        /// <summary>
        /// Writes the connection.
        /// </summary>
        /// <returns></returns>
        public virtual IDbConnection WriteConnection()
        {
            var sqlConnection = new SqlConnection(_foundationConStr);
            sqlConnection.Open();

            return sqlConnection;
        }
        /// <summary>
        /// Reads the connection.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IDbConnection> ReadConnectionAsync()
        {
            var sqlConnection = new SqlConnection(_foundationConStr);
            await sqlConnection.OpenAsync();

            return sqlConnection;
        }

        /// <summary>
        /// Writes the connection.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IDbConnection> WriteConnectionAsync()
        {
            var sqlConnection = new SqlConnection(_foundationConStr);
            await sqlConnection.OpenAsync();

            return sqlConnection;
        }
    }
}
