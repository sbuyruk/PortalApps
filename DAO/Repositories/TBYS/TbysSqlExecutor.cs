using DAO.Ortak;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Utility.Logging;

namespace DAO.Repositories.TBYS
{
    /// <summary>
    /// Explicit-context SQL path for migrated TBYS repositories. Business connections
    /// retain normal ambient transaction enlistment; technical traces are independent.
    /// Legacy callers keep their injected DbClass until their workflow is migrated.
    /// </summary>
    internal sealed class TbysSqlExecutor
    {
        private readonly DbClass legacy;
        private readonly PortalLogContext context;
        private readonly string repositoryName;

        internal TbysSqlExecutor(DbClass legacy)
        {
            if (legacy == null) throw new ArgumentNullException("legacy", "Veritabanı erişimi gereklidir.");
            this.legacy = legacy;
        }

        internal TbysSqlExecutor(PortalLogContext context, string repositoryName)
        {
            if (context == null) throw new ArgumentNullException("context", "Log bağlamı gereklidir.");
            if (context.Project != "TBYS")
                throw new ArgumentException("Bu veritabanı erişimi yalnızca TBYS log bağlamını kabul eder.", "context");
            if (string.IsNullOrWhiteSpace(repositoryName))
                throw new ArgumentException("Repository adı gereklidir.", "repositoryName");
            this.context = context;
            this.repositoryName = repositoryName;
        }

        internal DataTable SelectFromDb(SqlQuery query, string identifier, [CallerMemberName] string method = "")
        {
            if (legacy != null) return legacy.SelectFromDb(query, identifier);
            return Trace("SELECT", method, () =>
            {
                using (SqlConnection connection = new SqlConnection(DBProcess.getConnectString()))
                using (SqlCommand command = new SqlCommand(query.Sql, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    command.Parameters.AddRange(query.Parameters.ToArray());
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);
                    DataTable table = dataSet.Tables[0];
                    return table.Rows.Count == 0 ? null : table;
                }
            }, result => true);
        }

        internal int Insert(SqlQuery query, [CallerMemberName] string method = "")
        {
            if (legacy != null) return legacy.Insert(query);
            return Trace("INSERT", method, () =>
            {
                using (SqlConnection connection = new SqlConnection(DBProcess.getConnectString()))
                using (SqlCommand command = new SqlCommand(query.Sql + ";SELECT SCOPE_IDENTITY()", connection))
                {
                    command.Parameters.AddRange(query.Parameters.ToArray());
                    connection.Open();
                    object value = command.ExecuteScalar();
                    return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
                }
            }, result => result > 0);
        }

        internal bool Update2Db(SqlQuery query, [CallerMemberName] string method = "")
        {
            if (legacy != null) return legacy.Update2Db(query);
            return Trace("UPDATE", method, () => ExecuteNonQuery(query), result => result);
        }

        internal bool DeleteFromDb(SqlQuery query, string description, [CallerMemberName] string method = "")
        {
            if (legacy != null) return legacy.DeleteFromDb(query, description);
            return Trace("DELETE", method, () => ExecuteNonQuery(query), result => result);
        }

        private static bool ExecuteNonQuery(SqlQuery query)
        {
            using (SqlConnection connection = new SqlConnection(DBProcess.getConnectString()))
            using (SqlCommand command = new SqlCommand(query.Sql, connection))
            {
                command.Parameters.AddRange(query.Parameters.ToArray());
                connection.Open();
                command.ExecuteNonQuery();
                return true;
            }
        }

        private T Trace<T>(string operation, string method, Func<T> execute, Predicate<T> isSuccessful)
        {
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try
            {
                T result = execute();
                succeeded = isSuccessful(result);
                return result;
            }
            finally
            {
                clock.Stop();
                // Only the static repository/method names are passed, never SQL or parameter values.
                PortalTechnicalLog.WriteSqlTrace(context, operation, repositoryName + "." + method,
                    clock.ElapsedMilliseconds, succeeded);
            }
        }
    }
}
