using DAO.Ortak;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Utility.Logging;

namespace DAO.Repositories.MTS
{
    // Explicit-context path: no legacy SQL writer, and no SQL text/values in traces.
    internal sealed class MtsSqlExecutor
    {
        private readonly PortalLogContext context;
        private readonly string repositoryName;

        internal MtsSqlExecutor(PortalLogContext context, string repositoryName)
        {
            if (context == null || context.Project != "MTS")
                throw new ArgumentException("MTS log bağlamı gereklidir.", "context");
            this.context = context;
            this.repositoryName = repositoryName;
        }

        internal DataTable Select(SqlQuery query, SqlTransactionContext transaction = null,
            [CallerMemberName] string method = "")
        {
            return Trace("SELECT", method, () =>
            {
                if (transaction != null) return transaction.Select(query);
                using (SqlConnection connection = new SqlConnection(DBProcess.getConnectString()))
                using (SqlCommand command = new SqlCommand(query.Sql, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    command.Parameters.AddRange(query.Parameters.ToArray());
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    return table.Rows.Count == 0 ? null : table;
                }
            }, result => true);
        }

        internal int Insert(SqlQuery query, SqlTransactionContext transaction,
            [CallerMemberName] string method = "")
        {
            RequireTransaction(transaction);
            return Trace("INSERT", method, () => transaction.Insert(query), result => result > 0);
        }

        internal bool Write(SqlQuery query, string operation, SqlTransactionContext transaction,
            [CallerMemberName] string method = "")
        {
            RequireTransaction(transaction);
            return Trace(operation, method, () => transaction.Update(query), result => result);
        }

        private static void RequireTransaction(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
        }

        private T Trace<T>(string operation, string method, Func<T> execute, Predicate<T> succeeded)
        {
            Stopwatch clock = Stopwatch.StartNew();
            bool success = false;
            try
            {
                T result = execute();
                success = succeeded(result);
                return result;
            }
            finally
            {
                clock.Stop();
                PortalTechnicalLog.WriteSqlTrace(context, operation, repositoryName + "." + method,
                    clock.ElapsedMilliseconds, success);
            }
        }
    }
}
