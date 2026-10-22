using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Utility.Logging;

namespace DAO.Ortak
{
    // Explicit-context reads bypass the legacy SQL logger.
    internal static class PortalSqlReadExecutor
    {
        internal static DataTable Select(SqlQuery query, PortalLogContext context, string repositoryMethod, SqlTransactionContext transaction = null)
        {
            if (context == null) throw new ArgumentNullException("context", "Log bağlamı gereklidir.");
            Stopwatch clock = Stopwatch.StartNew();
            bool succeeded = false;
            try
            {
                if (transaction != null)
                {
                    DataTable result = transaction.Select(query);
                    succeeded = true;
                    return result;
                }
                using (SqlConnection connection = new SqlConnection(DBProcess.getConnectString()))
                using (SqlCommand command = new SqlCommand(query.Sql, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    command.Parameters.AddRange(query.Parameters.ToArray());
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    succeeded = true;
                    return table.Rows.Count == 0 ? null : table;
                }
            }
            finally
            {
                clock.Stop();
                PortalTechnicalLog.WriteSqlTrace(context, "SELECT", repositoryMethod,
                    clock.ElapsedMilliseconds, succeeded);
            }
        }
    }
}
