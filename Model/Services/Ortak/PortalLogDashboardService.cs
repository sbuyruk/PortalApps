using DAO.Repositories.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Model.Services.Ortak
{
    public sealed class PortalLogDashboardService
    {
        private readonly PortalLogReadRepository repository;

        public PortalLogDashboardService()
            : this(new PortalLogReadRepository())
        {
        }

        public PortalLogDashboardService(PortalLogReadRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public List<PortalLogEntry> GetLogs(PortalLogFilter filter)
        {
            filter = Normalize(filter);
            return MapLogs(repository.SelectLogs(filter.FromUtc, filter.ToUtc, filter.Project,
                filter.WebPart, filter.UserLogin, filter.OperationId, filter.Kind,
                filter.Operation, filter.Outcome, filter.ErrorLevel, filter.Page, filter.PageSize));
        }

        public List<PortalLogEntry> GetOperation(Guid operationId)
        {
            if (operationId == Guid.Empty) throw new ArgumentException("OperationId gereklidir.", "operationId");
            return MapLogs(repository.SelectOperation(operationId));
        }

        public List<PortalAuditChangeItem> GetChanges(long portalLogId)
        {
            if (portalLogId <= 0) return new List<PortalAuditChangeItem>();
            return MapChanges(repository.SelectChanges(portalLogId));
        }

        public PortalLogSummary GetSummary(DateTime fromUtc, DateTime toUtc)
        {
            if (toUtc <= fromUtc) throw new ArgumentException("Tarih aralığı geçersiz.");
            DataTable table = repository.SelectSummary(fromUtc, toUtc);
            PortalLogSummary summary = new PortalLogSummary();
            if (table == null || table.Rows.Count == 0) return summary;
            DataRow row = table.Rows[0];
            summary.TotalActivityCount = IntValue(row, "TotalActivityCount");
            summary.SuccessfulActivityCount = IntValue(row, "SuccessfulActivityCount");
            summary.FailedActivityCount = IntValue(row, "FailedActivityCount");
            summary.ActiveUserCount = IntValue(row, "ActiveUserCount");
            summary.LastHourActivityCount = IntValue(row, "LastHourActivityCount");
            summary.AverageSqlDurationMs = NullableDecimal(row, "AverageSqlDurationMs");
            summary.TopErrorWebPart = StringValue(row, "TopErrorWebPart");
            summary.TopErrorCount = IntValue(row, "TopErrorCount");
            return summary;
        }

        private static PortalLogFilter Normalize(PortalLogFilter filter)
        {
            filter = filter ?? new PortalLogFilter();
            filter.Page = filter.Page < 1 ? 1 : filter.Page;
            filter.PageSize = filter.PageSize < 1 ? 50 : (filter.PageSize > 200 ? 200 : filter.PageSize);
            return filter;
        }

        private static List<PortalLogEntry> MapLogs(DataTable table)
        {
            List<PortalLogEntry> result = new List<PortalLogEntry>();
            if (table == null) return result;
            foreach (DataRow row in table.Rows)
            {
                PortalLogEntry item = new PortalLogEntry
                {
                    Id = LongValue(row, "Id"),
                    OccurredUtc = DateTimeValue(row, "OccurredUtc"),
                    Kind = StringValue(row, "Kind"),
                    Project = StringValue(row, "Project"),
                    WebPart = StringValue(row, "WebPart"),
                    UserLogin = StringValue(row, "UserLogin"),
                    PagePath = StringValue(row, "PagePath"),
                    OperationId = GuidValue(row, "OperationId"),
                    EventName = StringValue(row, "EventName"),
                    Operation = StringValue(row, "Operation"),
                    Outcome = StringValue(row, "Outcome"),
                    Detail = StringValue(row, "Detail"),
                    EntityName = StringValue(row, "EntityName"),
                    EntityId = StringValue(row, "EntityId"),
                    ErrorType = StringValue(row, "ErrorType"),
                    ErrorLevel = StringValue(row, "ErrorLevel"),
                    UserMessage = StringValue(row, "UserMessage"),
                    TechnicalSummary = StringValue(row, "TechnicalSummary"),
                    StackTrace = StringValue(row, "StackTrace"),
                    InnerException = StringValue(row, "InnerException"),
                    SqlOperation = StringValue(row, "SqlOperation"),
                    RepositoryMethod = StringValue(row, "RepositoryMethod"),
                    DurationMs = NullableLong(row, "DurationMs")
                };
                result.Add(item);
            }
            return result;
        }

        private static List<PortalAuditChangeItem> MapChanges(DataTable table)
        {
            List<PortalAuditChangeItem> result = new List<PortalAuditChangeItem>();
            if (table == null) return result;
            foreach (DataRow row in table.Rows)
            {
                result.Add(new PortalAuditChangeItem
                {
                    Id = LongValue(row, "Id"),
                    PortalLogId = NullableLong(row, "PortalLogId"),
                    LegacyAuditLogId = NullableInt(row, "LegacyAuditLogId"),
                    FieldName = StringValue(row, "FieldName"),
                    OldValue = StringValue(row, "OldValue"),
                    NewValue = StringValue(row, "NewValue")
                });
            }
            return result;
        }

        private static string StringValue(DataRow row, string column) { return row[column] == DBNull.Value ? null : Convert.ToString(row[column], CultureInfo.InvariantCulture); }
        private static int IntValue(DataRow row, string column) { return row[column] == DBNull.Value ? 0 : Convert.ToInt32(row[column], CultureInfo.InvariantCulture); }
        private static int? NullableInt(DataRow row, string column) { return row[column] == DBNull.Value ? (int?)null : Convert.ToInt32(row[column], CultureInfo.InvariantCulture); }
        private static long LongValue(DataRow row, string column) { return row[column] == DBNull.Value ? 0L : Convert.ToInt64(row[column], CultureInfo.InvariantCulture); }
        private static long? NullableLong(DataRow row, string column) { return row[column] == DBNull.Value ? (long?)null : Convert.ToInt64(row[column], CultureInfo.InvariantCulture); }
        private static decimal? NullableDecimal(DataRow row, string column) { return row[column] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row[column], CultureInfo.InvariantCulture); }
        private static DateTime DateTimeValue(DataRow row, string column) { return row[column] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[column], CultureInfo.InvariantCulture); }
        private static Guid GuidValue(DataRow row, string column) { return row[column] == DBNull.Value ? Guid.Empty : (Guid)row[column]; }
    }
}
