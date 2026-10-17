using DAO.Ortak;
using System;
using System.Data.SqlClient;
using Utility.Logging;

namespace DAO.Repositories.Ortak
{
    /// <summary>
    /// Persistence for the new logging tables. Business audit writes use the caller's
    /// transaction; opening a WebPart is recorded outside a business transaction.
    /// </summary>
    public sealed class PortalLogRepository
    {
        public bool TryRecordWebPartOpened(PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context", "Log bağlamı gereklidir.");
            try
            {
                if (!PortalLogSettings.IsEnabled(context)) return false;
                ValidateLength(context.Project, 64, "Proje adı");
                ValidateLength(context.WebPart, 256, "WebPart adı");
                ValidateLength(context.UserLogin, 256, "Kullanıcı adı");
                PortalLogRecord record = new PortalLogRecord(
                    PortalLogKind.UserActivity, context, "WebPartOpened", "Succeeded");
                SqlQuery query = new SqlQuery(@"
                    INSERT INTO PortalActivityLog_Table
                        (OccurredUtc, Project, WebPart, UserLogin, PagePath, OperationId, EventName)
                    VALUES
                        (@OccurredUtc, @Project, @WebPart, @UserLogin, @PagePath, @OperationId, @EventName)");
                query.AddParameter("@OccurredUtc", record.OccurredUtc);
                query.AddParameter("@Project", context.Project);
                query.AddParameter("@WebPart", context.WebPart);
                query.AddParameter("@UserLogin", context.UserLogin);
                query.AddParameter("@PagePath", context.PagePath);
                query.AddParameter("@OperationId", context.OperationId);
                query.AddParameter("@EventName", record.Name);
                using (SqlConnection connection = new SqlConnection(DBProcess.getConnectString()))
                using (SqlCommand command = new SqlCommand(query.Sql, connection))
                {
                    command.Parameters.AddRange(query.Parameters.ToArray());
                    connection.Open();
                    return command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception error)
            {
                PortalTechnicalLog.WriteError(context, "WebPartActivityWrite", error);
                return false;
            }
        }

        public int RecordAudit(PortalLogContext context, string operation, string entityName,
            string entityId, SqlTransactionContext transaction)
        {
            if (context == null) throw new ArgumentNullException("context", "Log bağlamı gereklidir.");
            if (transaction == null) throw new ArgumentNullException("transaction", "Veritabanı işlem bağlamı gereklidir.");
            if (operation != "INSERT" && operation != "UPDATE" && operation != "DELETE")
                throw new ArgumentException("Desteklenmeyen denetim işlemi.", "operation");
            if (string.IsNullOrWhiteSpace(entityName))
                throw new ArgumentException("Kayıt türü gereklidir.", "entityName");
            if (string.IsNullOrWhiteSpace(entityId))
                throw new ArgumentException("Kayıt kimliği gereklidir.", "entityId");
            if (!PortalLogSettings.IsEnabled(context)) return 0;

            ValidateLength(context.Project, 64, "Proje adı");
            ValidateLength(context.WebPart, 256, "WebPart adı");
            ValidateLength(context.UserLogin, 256, "Kullanıcı adı");
            ValidateLength(entityName, 128, "Kayıt türü");
            ValidateLength(entityId, 256, "Kayıt kimliği");

            PortalLogRecord record = new PortalLogRecord(
                PortalLogKind.Audit, context, operation, "Succeeded");
            SqlQuery query = new SqlQuery(@"
                INSERT INTO PortalAuditLog_Table
                    (OccurredUtc, Project, WebPart, UserLogin, PagePath, OperationId,
                     Operation, EntityName, EntityId)
                VALUES
                    (@OccurredUtc, @Project, @WebPart, @UserLogin, @PagePath, @OperationId,
                     @Operation, @EntityName, @EntityId)");
            query.AddParameter("@OccurredUtc", record.OccurredUtc);
            query.AddParameter("@Project", context.Project);
            query.AddParameter("@WebPart", context.WebPart);
            query.AddParameter("@UserLogin", context.UserLogin);
            query.AddParameter("@PagePath", context.PagePath);
            query.AddParameter("@OperationId", context.OperationId);
            query.AddParameter("@Operation", operation);
            query.AddParameter("@EntityName", entityName);
            query.AddParameter("@EntityId", entityId);

            int id = transaction.Insert(query);
            if (id <= 0)
                throw new InvalidOperationException("İşlem denetimi kaydı oluşturulamadı.");
            return id;
        }

        private static void ValidateLength(string value, int limit, string label)
        {
            if (value != null && value.Length > limit)
                throw new ArgumentException(label + " izin verilen uzunluğu aşıyor.");
        }
    }
}
