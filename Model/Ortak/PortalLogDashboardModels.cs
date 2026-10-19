using System;

namespace Model.Ortak
{
    [Serializable]
    public sealed class PortalLogFilter
    {
        public DateTime? FromUtc { get; set; }
        public DateTime? ToUtc { get; set; }
        public string Project { get; set; }
        public string WebPart { get; set; }
        public string UserLogin { get; set; }
        public string OperationId { get; set; }
        public string Kind { get; set; }
        public string Operation { get; set; }
        public string Outcome { get; set; }
        public string ErrorLevel { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public PortalLogFilter()
        {
            Page = 1;
            PageSize = 50;
        }
    }

    [Serializable]
    public sealed class PortalLogEntry
    {
        public long Id { get; set; }
        public DateTime OccurredUtc { get; set; }
        public string Kind { get; set; }
        public string Project { get; set; }
        public string WebPart { get; set; }
        public string UserLogin { get; set; }
        public string PagePath { get; set; }
        public Guid OperationId { get; set; }
        public string EventName { get; set; }
        public string Operation { get; set; }
        public string Outcome { get; set; }
        public string Detail { get; set; }
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public string ErrorType { get; set; }
        public string ErrorLevel { get; set; }
        public string UserMessage { get; set; }
        public string TechnicalSummary { get; set; }
        public string StackTrace { get; set; }
        public string InnerException { get; set; }
        public string SqlOperation { get; set; }
        public string RepositoryMethod { get; set; }
        public long? DurationMs { get; set; }
    }

    [Serializable]
    public sealed class PortalAuditChangeItem
    {
        public long Id { get; set; }
        public long? PortalLogId { get; set; }
        public int? LegacyAuditLogId { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }

    [Serializable]
    public sealed class PortalLogSummary
    {
        public int TotalActivityCount { get; set; }
        public int SuccessfulActivityCount { get; set; }
        public int FailedActivityCount { get; set; }
        public int ActiveUserCount { get; set; }
        public int LastHourActivityCount { get; set; }
        public decimal? AverageSqlDurationMs { get; set; }
        public string TopErrorWebPart { get; set; }
        public int TopErrorCount { get; set; }
    }
}
