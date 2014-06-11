using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;
using Dapper;
using System.Data.SqlServerCe;

namespace mahamudra.it.slides.dal
{
    /// <summary>
    /// Classe Base per la gestione della repository e il DAL
    /// </summary>
    public abstract class BaseRepository
    {
        /// <summary>
        /// Sets the identity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="setId">The set identifier.</param>
        protected static void setIdentity<T>(IDbConnection connection, Action<T> setId)
        {
            dynamic identity = connection.Query("SELECT @@IDENTITY AS Ordine").Single();
            T newId = (T)identity.Ordine;
            setId(newId);
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        /// <returns></returns>
        protected static IDbConnection openConnection()
        {
            string root_path = CustomRootPathProvider.rootPath();
            var db_provider = Xmlconfig.get("provider", root_path);
            IDbConnection connection = null;

            if (db_provider.Value == "sql_compact")
            {
                 connection = new SqlCeConnection(
                   Xmlconfig.get(db_provider.Value, root_path).Value
              );
            }
            else
            {
                 connection = new SqlConnection(
                  Xmlconfig.get(db_provider.Value, root_path).Value
             );
            }

            connection.Open();
            return connection;
        }
    }
}