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
    /// Context-free report compatibility uses this reader without legacy log writes.
    /// </summary>
    internal sealed class TbysSqlExecutor
    {
        private readonly PortalLogContext context;
        private readonly string repositoryName;

        internal TbysSqlExecutor(string repositoryName)
        {
            if (string.IsNullOrWhiteSpace(repositoryName))
                throw new ArgumentException("Repository adı gereklidir.", "repositoryName");
            this.repositoryName = repositoryName;
        }

        internal TbysSqlExecutor(PortalLogContext context, string repositoryName)
        {
            if (context == null) throw new ArgumentNullException("context", "Log bağlamı gereklidir.");
            if (context.Project != "TBYS" && context.Project != "NBYS")
                throw new ArgumentException("Bu veritabanı erişimi TBYS veya NBYS log bağlamını kabul eder.", "context");
            if (string.IsNullOrWhiteSpace(repositoryName))
                throw new ArgumentException("Repository adı gereklidir.", "repositoryName");
            this.context = context;
            this.repositoryName = repositoryName;
        }

        internal DataTable SelectFromDb(SqlQuery query, string identifier, [CallerMemberName] string method = "")
        {
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

        internal DataTable SelectFromDb(SqlQuery query, SqlTransactionContext transaction,
            PortalLogContext logContext, [CallerMemberName] string method = "")
        {
            ValidateTransaction(transaction, logContext);
            return Trace(logContext, "SELECT", method, () => transaction.Select(query), result => true);
        }

        internal int Insert(SqlQuery query, SqlTransactionContext transaction,
            PortalLogContext logContext, [CallerMemberName] string method = "")
        {
            ValidateTransaction(transaction, logContext);
            return Trace(logContext, "INSERT", method, () => transaction.Insert(query), result => result > 0);
        }

        internal bool Update2Db(SqlQuery query, SqlTransactionContext transaction,
            PortalLogContext logContext, [CallerMemberName] string method = "")
        {
            ValidateTransaction(transaction, logContext);
            return Trace(logContext, "UPDATE", method, () => transaction.Update(query), result => result);
        }

        internal bool DeleteFromDb(SqlQuery query, SqlTransactionContext transaction,
            PortalLogContext logContext, [CallerMemberName] string method = "")
        {
            ValidateTransaction(transaction, logContext);
            return Trace(logContext, "DELETE", method, () => transaction.Update(query), result => result);
        }

        private static void ValidateTransaction(SqlTransactionContext transaction, PortalLogContext logContext)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "Veritabanı işlem bağlamı gereklidir.");
            if (logContext == null) throw new ArgumentNullException("logContext", "Log bağlamı gereklidir.");
            if (logContext.Project != "TBYS") throw new ArgumentException("TBYS log bağlamı gereklidir.", "logContext");
        }

        private T Trace<T>(string operation, string method, Func<T> execute, Predicate<T> isSuccessful)
        {
            return Trace(context, operation, method, execute, isSuccessful);
        }

        private T Trace<T>(PortalLogContext logContext, string operation, string method,
            Func<T> execute, Predicate<T> isSuccessful)
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
                PortalTechnicalLog.WriteSqlTrace(logContext, operation, repositoryName + "." + method,
                    clock.ElapsedMilliseconds, succeeded);
            }
        }
    }
}
