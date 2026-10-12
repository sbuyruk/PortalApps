using System;
using System.Data;
using System.Data.SqlClient;

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
                transaction.Dispose();
                connection.Dispose();
            }
        }
    }
}
