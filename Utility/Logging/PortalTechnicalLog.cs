using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Web.Configuration;

namespace Utility.Logging
{
    /// <summary>
    /// Writes technical events to the Portal Logging store. The write is intentionally
    /// independent from the business transaction so a failed transaction can still leave
    /// an error or SQL trace record for the monitoring WebPart.
    /// </summary>
    public static class PortalTechnicalLog
    {
        public const string DefaultUserMessage = "İşlem tamamlanamadı. Lütfen tekrar deneyiniz.";

        public static void WriteError(PortalLogContext context, string name, Exception error)
        {
            try
            {
                if (context == null || error == null || !PortalLogSettings.IsEnabled(context)) return;
                PortalLogRecord record = new PortalLogRecord(PortalLogKind.Error, context, name, "Failed");
                string detail = error.ToString();
                string technicalDetail = detail.Length > 8000 ? detail.Substring(0, 8000) : detail;
                WriteErrorRecord(record, error, technicalDetail);
            }
            catch (Exception)
            {
                // Logging must not replace the original application exception.
            }
        }

        public static void WriteSqlTrace(PortalLogContext context, string operation, string queryName,
            long durationMilliseconds, bool succeeded)
        {
            try
            {
                if (context == null || !PortalLogSettings.IsEnabled(context)) return;
                if (operation != "SELECT" && operation != "INSERT" && operation != "UPDATE" && operation != "DELETE") return;
                PortalLogRecord record = new PortalLogRecord(PortalLogKind.SqlTrace, context,
                    queryName, succeeded ? "Succeeded" : "Failed", durationMilliseconds);
                WriteSqlRecord(record, operation, queryName, succeeded);
            }
            catch (Exception)
            {
                // Logging must not replace the SQL result or exception.
            }
        }

        /// <summary>Flushes TBYS traces in bounded batches after the business connection is closed.</summary>
        public static void WriteSqlTraces(IEnumerable<PortalSqlTrace> traces)
        {
            try
            {
                if (traces == null) return;
                // Each row has 14 parameters; 50 rows stay below SQL Server's parameter limit.
                const int batchSize = 50;
                List<PortalSqlTrace> batch = new List<PortalSqlTrace>(batchSize);
                foreach (PortalSqlTrace trace in traces)
                {
                    if (trace == null || trace.Context == null) continue;
                    if (trace.Context.Project != "TBYS")
                    {
                        WriteSqlTraceBatch(batch);
                        batch.Clear();
                        WriteSqlTrace(trace.Context, trace.Operation, trace.QueryName,
                            trace.DurationMilliseconds, trace.Succeeded);
                        continue;
                    }
                    if (!PortalLogSettings.IsEnabled(trace.Context)) continue;
                    if (trace.Operation != "SELECT" && trace.Operation != "INSERT"
                        && trace.Operation != "UPDATE" && trace.Operation != "DELETE") continue;
                    batch.Add(trace);
                    if (batch.Count == batchSize)
                    {
                        WriteSqlTraceBatch(batch);
                        batch.Clear();
                    }
                }
                WriteSqlTraceBatch(batch);
            }
            catch (Exception)
            {
                // Logging must not replace the business result or exception.
            }
        }

        private static void WriteSqlTraceBatch(List<PortalSqlTrace> traces)
        {
            if (traces.Count == 0) return;
            bool written = false;
            bool canRetry = true;
            try
            {
                // Resolve web.config and open a connection once per batch, not once per trace.
                using (SqlConnection connection = new SqlConnection(GetConnectionString(traces[0].Context)))
                using (SqlCommand command = connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(@"
                        INSERT INTO PortalLog_Table
                            (OccurredUtc, Kind, Project, WebPart, UserLogin, PagePath, OperationId,
                             EventName, Operation, Outcome, SqlOperation, RepositoryMethod, DurationMs, Detail)
                        VALUES ");
                    for (int i = 0; i < traces.Count; i++)
                    {
                        PortalSqlTrace trace = traces[i];
                        PortalLogRecord record = new PortalLogRecord(PortalLogKind.SqlTrace, trace.Context,
                            trace.QueryName, trace.Succeeded ? "Succeeded" : "Failed", trace.DurationMilliseconds);
                        string suffix = i.ToString(CultureInfo.InvariantCulture);
                        if (i > 0) sql.Append(",");
                        sql.Append("(@OccurredUtc" + suffix + ", @Kind" + suffix + ", @Project" + suffix
                            + ", @WebPart" + suffix + ", @UserLogin" + suffix + ", @PagePath" + suffix
                            + ", @OperationId" + suffix + ", @EventName" + suffix + ", @Operation" + suffix
                            + ", @Outcome" + suffix + ", @SqlOperation" + suffix + ", @RepositoryMethod" + suffix
                            + ", @DurationMs" + suffix + ", @Detail" + suffix + ")");
                        AddSqlRecordParameters(command, record, trace.Operation, trace.QueryName, trace.Succeeded, suffix);
                    }
                    command.CommandText = sql.ToString();
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        command.Transaction = transaction;
                        // Retry individually only if no batch may have committed.
                        canRetry = false;
                        try
                        {
                            command.ExecuteNonQuery();
                            transaction.Commit();
                            written = true;
                        }
                        catch (Exception)
                        {
                            try
                            {
                                transaction.Rollback();
                                canRetry = true;
                            }
                            catch (Exception)
                            {
                                // An uncertain commit must not produce duplicate traces through a retry.
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Technical logging is best effort and independent from the business transaction.
            }
            if (!written && canRetry)
                foreach (PortalSqlTrace trace in traces)
                    WriteSqlTrace(trace.Context, trace.Operation, trace.QueryName,
                        trace.DurationMilliseconds, trace.Succeeded);
        }

        private static void WriteErrorRecord(PortalLogRecord record, Exception error, string detail)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString(record.Context)))
                using (SqlCommand command = new SqlCommand(@"
                    INSERT INTO PortalLog_Table
                        (OccurredUtc, Kind, Project, WebPart, UserLogin, PagePath, OperationId,
                         EventName, Outcome, Detail, ErrorType, ErrorLevel, TechnicalSummary,
                         StackTrace, InnerException, UserMessage, EntityName, EntityId)
                    VALUES
                        (@OccurredUtc, @Kind, @Project, @WebPart, @UserLogin, @PagePath, @OperationId,
                         @EventName, @Outcome, @Detail, @ErrorType, @ErrorLevel, @TechnicalSummary,
                         @StackTrace, @InnerException, @UserMessage, @EntityName, @EntityId)", connection))
                {
                    AddCommonParameters(command, record);
                    PortalLogContext entity = PortalLogContext.ErrorEntity(error, record.Context);
                    command.Parameters.AddWithValue("@EntityName", (object)entity.EntityName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EntityId", (object)entity.EntityId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EventName", record.Name);
                    command.Parameters.AddWithValue("@Outcome", record.Outcome);
                    command.Parameters.AddWithValue("@Detail", (object)detail ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ErrorType", error.GetType().Name);
                    command.Parameters.AddWithValue("@ErrorLevel", "Error");
                    string summary = error.Message;
                    if (record.Context.Project == "TBYS" && summary != null && summary.Length > 2000)
                        summary = summary.Substring(0, 2000);
                    command.Parameters.AddWithValue("@TechnicalSummary", (object)summary ?? DBNull.Value);
                    command.Parameters.AddWithValue("@StackTrace", (object)error.StackTrace ?? DBNull.Value);
                    command.Parameters.AddWithValue("@InnerException", (object)(error.InnerException == null ? null : error.InnerException.ToString()) ?? DBNull.Value);
                    command.Parameters.AddWithValue("@UserMessage", record.Context.Project == "TBYS"
                        ? (object)DefaultUserMessage : DBNull.Value);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                // Logging must not replace the original application exception.
            }
        }

        private static void WriteSqlRecord(PortalLogRecord record, string operation, string queryName, bool succeeded)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString(record.Context)))
                using (SqlCommand command = new SqlCommand(@"
                    INSERT INTO PortalLog_Table
                        (OccurredUtc, Kind, Project, WebPart, UserLogin, PagePath, OperationId,
                         EventName, Operation, Outcome, SqlOperation, RepositoryMethod, DurationMs, Detail)
                    VALUES
                        (@OccurredUtc, @Kind, @Project, @WebPart, @UserLogin, @PagePath, @OperationId,
                         @EventName, @Operation, @Outcome, @SqlOperation, @RepositoryMethod, @DurationMs, @Detail)", connection))
                {
                    AddSqlRecordParameters(command, record, operation, queryName, succeeded, "");
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                // Logging must not replace the SQL result or exception.
            }
        }

        private static void AddSqlRecordParameters(SqlCommand command, PortalLogRecord record,
            string operation, string queryName, bool succeeded, string suffix)
        {
            AddCommonParameters(command, record, suffix);
            command.Parameters.AddWithValue("@EventName" + suffix, queryName);
            // SELECT belongs to SqlOperation; the business Operation constraint excludes it.
            command.Parameters.AddWithValue("@Operation" + suffix, operation == "SELECT" ? (object)DBNull.Value : operation);
            command.Parameters.AddWithValue("@Outcome" + suffix, succeeded ? "Succeeded" : "Failed");
            command.Parameters.AddWithValue("@SqlOperation" + suffix, operation);
            command.Parameters.AddWithValue("@RepositoryMethod" + suffix, queryName);
            command.Parameters.AddWithValue("@DurationMs" + suffix, record.DurationMilliseconds.HasValue ? (object)record.DurationMilliseconds.Value : DBNull.Value);
            command.Parameters.AddWithValue("@Detail" + suffix, "SQL izi kaydedildi; sorgu metni ve parametreler kaydedilmedi.");
        }

        private static void AddCommonParameters(SqlCommand command, PortalLogRecord record, string suffix = "")
        {
            command.Parameters.AddWithValue("@OccurredUtc" + suffix, record.OccurredUtc);
            command.Parameters.AddWithValue("@Kind" + suffix, record.Kind.ToString());
            command.Parameters.AddWithValue("@Project" + suffix, record.Context.Project);
            command.Parameters.AddWithValue("@WebPart" + suffix, record.Context.WebPart);
            command.Parameters.AddWithValue("@UserLogin" + suffix, record.Context.UserLogin);
            command.Parameters.AddWithValue("@PagePath" + suffix, record.Context.PagePath);
            command.Parameters.AddWithValue("@OperationId" + suffix, record.Context.OperationId);
        }

        private static string GetConnectionString(PortalLogContext context)
        {
            System.Configuration.Configuration configuration = WebConfigurationManager.OpenWebConfiguration("/");
            System.Configuration.ConnectionStringSettings connection =
                configuration.ConnectionStrings.ConnectionStrings["TSKGV_Connection"];
            if (connection == null || string.IsNullOrWhiteSpace(connection.ConnectionString))
                throw new System.Configuration.ConfigurationErrorsException(
                    "'TSKGV_Connection' bağlantı dizesi web.config içinde bulunamadı.");
            if (context.Project == "TBYS")
            {
                // Technical events must survive rollback of a legacy ambient TransactionScope.
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connection.ConnectionString);
                builder.Enlist = false;
                return builder.ConnectionString;
            }
            return connection.ConnectionString;
        }
    }
}
