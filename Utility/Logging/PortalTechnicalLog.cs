using System;
using System.Data.SqlClient;
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

        private static void WriteErrorRecord(PortalLogRecord record, Exception error, string detail)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString(record.Context)))
                using (SqlCommand command = new SqlCommand(@"
                    INSERT INTO PortalLog_Table
                        (OccurredUtc, Kind, Project, WebPart, UserLogin, PagePath, OperationId,
                         EventName, Outcome, Detail, ErrorType, ErrorLevel, TechnicalSummary,
                         StackTrace, InnerException, UserMessage)
                    VALUES
                        (@OccurredUtc, @Kind, @Project, @WebPart, @UserLogin, @PagePath, @OperationId,
                         @EventName, @Outcome, @Detail, @ErrorType, @ErrorLevel, @TechnicalSummary,
                         @StackTrace, @InnerException, @UserMessage)", connection))
                {
                    AddCommonParameters(command, record);
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
                    AddCommonParameters(command, record);
                    command.Parameters.AddWithValue("@EventName", queryName);
                    // SELECT belongs to SqlOperation; the business Operation constraint excludes it.
                    command.Parameters.AddWithValue("@Operation", operation == "SELECT" ? (object)DBNull.Value : operation);
                    command.Parameters.AddWithValue("@Outcome", succeeded ? "Succeeded" : "Failed");
                    command.Parameters.AddWithValue("@SqlOperation", operation);
                    command.Parameters.AddWithValue("@RepositoryMethod", queryName);
                    command.Parameters.AddWithValue("@DurationMs", record.DurationMilliseconds.HasValue ? (object)record.DurationMilliseconds.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@Detail", "SQL izi kaydedildi; sorgu metni ve parametreler kaydedilmedi.");
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                // Logging must not replace the SQL result or exception.
            }
        }

        private static void AddCommonParameters(SqlCommand command, PortalLogRecord record)
        {
            command.Parameters.AddWithValue("@OccurredUtc", record.OccurredUtc);
            command.Parameters.AddWithValue("@Kind", record.Kind.ToString());
            command.Parameters.AddWithValue("@Project", record.Context.Project);
            command.Parameters.AddWithValue("@WebPart", record.Context.WebPart);
            command.Parameters.AddWithValue("@UserLogin", record.Context.UserLogin);
            command.Parameters.AddWithValue("@PagePath", record.Context.PagePath);
            command.Parameters.AddWithValue("@OperationId", record.Context.OperationId);
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
