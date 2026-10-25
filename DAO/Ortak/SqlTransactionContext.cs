using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Utility.Logging;

namespace DAO.Ortak
{
    /// <summary>
    /// Explicit transaction context for a bounded workflow.
    /// Existing DbClass calls remain unchanged.
    /// </summary>
    public sealed class SqlTransactionContext : IDisposable
    {
        private readonly SqlConnection connection;
        private readonly SqlTransaction transaction;
        private bool completed;
        private readonly List<Action> pendingSqlTraces = new List<Action>();

        // Capture only metadata here; technical logging runs after the connection is disposed.
        public void DeferSqlTrace(PortalLogContext context, string operation, string method,
            long durationMilliseconds, bool succeeded)
        {
            pendingSqlTraces.Add(() => PortalTechnicalLog.WriteSqlTrace(
                context, operation, method, durationMilliseconds, succeeded));
        }

        public SqlTransactionContext()
        {
            connection = new SqlConnection(DBProcess.getConnectString());
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }

        public int Insert(SqlQuery query)
        {
            if (query == null) throw new ArgumentNullException("query");
            using (SqlCommand command = new SqlCommand(
                query.Sql + ";SELECT SCOPE_IDENTITY()", connection, transaction))
            {
                command.Parameters.AddRange(query.Parameters.ToArray());
                object value = command.ExecuteScalar();
                return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
            }
        }

        public bool Update(SqlQuery query)
        {
            if (query == null) throw new ArgumentNullException("query");
            using (SqlCommand command = new SqlCommand(query.Sql, connection, transaction))
            {
                command.Parameters.AddRange(query.Parameters.ToArray());
                command.ExecuteNonQuery();
                return true;
            }
        }

        public DataTable Select(SqlQuery query)
        {
            if (query == null) throw new ArgumentNullException("query");
            using (SqlCommand command = new SqlCommand(query.Sql, connection, transaction))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                command.Parameters.AddRange(query.Parameters.ToArray());
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                DataTable table = dataSet.Tables[0];
                return table.Rows.Count == 0 ? null : table;
            }
        }

        public void AcquireEkstreImportLock()
        {
            using (SqlCommand command = new SqlCommand(@"
                DECLARE @result int;
                EXEC @result = sp_getapplock
                    @Resource = N'PortalApps.EkstreAktarma.Import',
                    @LockMode = N'Exclusive',
                    @LockOwner = N'Transaction',
                    @LockTimeout = 30000;
                SELECT @result;", connection, transaction))
            {
                command.CommandTimeout = 35;
                int result = Convert.ToInt32(command.ExecuteScalar());
                if (result < 0)
                    throw new InvalidOperationException("Ekstre yüklemesi başka bir işlem nedeniyle bekledi; lütfen tekrar deneyin.");
            }
        }

        public void Complete()
        {
            completed = true;
        }

        public void Dispose()
        {
            try
            {
                if (completed)
                    transaction.Commit();
                else
                    transaction.Rollback();
            }
            finally
            {
                try
                {
                    transaction.Dispose();
                }
                finally
                {
                    connection.Dispose();
                    foreach (Action trace in pendingSqlTraces) trace();
                    pendingSqlTraces.Clear();
                }
            }
        }
    }
}
