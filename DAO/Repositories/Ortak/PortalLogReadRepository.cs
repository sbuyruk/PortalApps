using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.Ortak
{
    public sealed class PortalLogReadRepository
    {
        private readonly DbClass db;

        public PortalLogReadRepository()
            : this(new DbClass())
        {
        }

        public PortalLogReadRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
        }

        public DataTable SelectLogs(DateTime? fromUtc, DateTime? toUtc, string project,
            string webPart, string userLogin, string operationId, string kind,
            string operation, string outcome, string errorLevel, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 50 : (pageSize > 200 ? 200 : pageSize);
            const string sql = @"
                SELECT Id, OccurredUtc, Kind, Project, WebPart, UserLogin, PagePath,
                       OperationId, EventName, Operation, Outcome, Detail, EntityName,
                       EntityId, ErrorType, ErrorLevel, UserMessage, TechnicalSummary,
                       StackTrace, InnerException, SqlOperation, RepositoryMethod, DurationMs
                FROM PortalLog_Table
                WHERE (@FromUtc IS NULL OR OccurredUtc >= @FromUtc)
                  AND (@ToUtc IS NULL OR OccurredUtc < @ToUtc)
                  AND (@Project IS NULL OR Project = @Project)
                  AND (@WebPart IS NULL OR WebPart = @WebPart)
                  AND (@UserLogin IS NULL OR UserLogin LIKE @UserLogin)
                  AND (@OperationId IS NULL OR CONVERT(nvarchar(36), OperationId) LIKE @OperationId)
                  AND (@Kind IS NULL OR Kind = @Kind)
                  AND (@Operation IS NULL OR Operation = @Operation)
                  AND (@Outcome IS NULL OR Outcome = @Outcome)
                  AND (@ErrorLevel IS NULL OR ErrorLevel = @ErrorLevel)
                ORDER BY OccurredUtc DESC, Id DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            SqlQuery query = new SqlQuery(sql);
            AddFilterParameters(query, fromUtc, toUtc, project, webPart, userLogin,
                operationId, kind, operation, outcome, errorLevel);
            query.AddParameter("@Offset", (page - 1) * pageSize);
            query.AddParameter("@PageSize", pageSize);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectOperation(Guid operationId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT Id, OccurredUtc, Kind, Project, WebPart, UserLogin, PagePath,
                       OperationId, EventName, Operation, Outcome, Detail, EntityName,
                       EntityId, ErrorType, ErrorLevel, UserMessage, TechnicalSummary,
                       StackTrace, InnerException, SqlOperation, RepositoryMethod, DurationMs
                FROM PortalLog_Table
                WHERE OperationId = @OperationId
                ORDER BY OccurredUtc ASC, Id ASC");
            query.AddParameter("@OperationId", operationId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectChanges(long portalLogId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT Id, PortalLogId, AuditLogId AS LegacyAuditLogId,
                       FieldName, OldValue, NewValue
                FROM PortalAuditChange_Table
                WHERE PortalLogId = @PortalLogId
                ORDER BY Id ASC");
            query.AddParameter("@PortalLogId", portalLogId);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectSummary(DateTime fromUtc, DateTime toUtc)
        {
            SqlQuery query = new SqlQuery(@"
                WITH Activity AS
                (
                    SELECT * FROM PortalLog_Table
                    WHERE Kind = 'UserActivity'
                      AND OccurredUtc >= @FromUtc AND OccurredUtc < @ToUtc
                ),
                Errors AS
                (
                    SELECT WebPart, COUNT(*) AS ErrorCount
                    FROM PortalLog_Table
                    WHERE Kind = 'Error'
                      AND OccurredUtc >= @FromUtc AND OccurredUtc < @ToUtc
                    GROUP BY WebPart
                )
                SELECT
                    (SELECT COUNT(*) FROM Activity) AS TotalActivityCount,
                    (SELECT COUNT(*) FROM Activity WHERE Outcome = 'Succeeded') AS SuccessfulActivityCount,
                    (SELECT COUNT(*) FROM Activity WHERE Outcome = 'Failed') AS FailedActivityCount,
                    (SELECT COUNT(DISTINCT UserLogin) FROM Activity) AS ActiveUserCount,
                    (SELECT COUNT(*) FROM Activity WHERE OccurredUtc >= DATEADD(hour, -1, @ToUtc)) AS LastHourActivityCount,
                    (SELECT AVG(CAST(DurationMs AS decimal(18,2))) FROM PortalLog_Table
                     WHERE Kind = 'SqlTrace' AND DurationMs IS NOT NULL
                       AND OccurredUtc >= @FromUtc AND OccurredUtc < @ToUtc) AS AverageSqlDurationMs,
                    (SELECT TOP 1 WebPart FROM Errors ORDER BY ErrorCount DESC, WebPart) AS TopErrorWebPart,
                    (SELECT ISNULL(MAX(ErrorCount), 0) FROM Errors) AS TopErrorCount");
            query.AddParameter("@FromUtc", fromUtc);
            query.AddParameter("@ToUtc", toUtc);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectMonitorSummary(DateTime fromUtc, DateTime toUtc,
            string project, string userLogin, string operationId)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT COUNT_BIG(CASE WHEN Kind = 'UserActivity' THEN 1 END) AS ActivityCount,
                       COUNT_BIG(CASE WHEN Kind = 'Error' THEN 1 END) AS ErrorCount,
                       COUNT_BIG(DISTINCT CASE WHEN Kind = 'UserActivity' THEN UserLogin END) AS ActiveUserCount,
                       AVG(CASE WHEN Kind = 'SqlTrace' THEN CAST(DurationMs AS decimal(18,2)) END) AS AverageSqlDurationMs
                FROM dbo.PortalLog_Table
                WHERE OccurredUtc >= @FromUtc AND OccurredUtc < @ToUtc
                  AND (@Project IS NULL OR Project = @Project)
                  AND (@UserLogin IS NULL OR UserLogin LIKE @UserLogin)
                  AND (@OperationId IS NULL OR CONVERT(nvarchar(36), OperationId) LIKE @OperationId)");
            query.AddParameter("@FromUtc", fromUtc);
            query.AddParameter("@ToUtc", toUtc);
            query.AddParameter("@Project", NullIfEmpty(project));
            query.AddParameter("@UserLogin", LikeValue(userLogin));
            query.AddParameter("@OperationId", LikeValue(operationId));
            return db.SelectFromDb(query, "");
        }

        private static void AddFilterParameters(SqlQuery query, DateTime? fromUtc, DateTime? toUtc,
            string project, string webPart, string userLogin, string operationId, string kind,
            string operation, string outcome, string errorLevel)
        {
            query.AddParameter("@FromUtc", (object)fromUtc ?? DBNull.Value);
            query.AddParameter("@ToUtc", (object)toUtc ?? DBNull.Value);
            query.AddParameter("@Project", NullIfEmpty(project));
            query.AddParameter("@WebPart", NullIfEmpty(webPart));
            query.AddParameter("@UserLogin", LikeValue(userLogin));
            query.AddParameter("@OperationId", LikeValue(operationId));
            query.AddParameter("@Kind", NullIfEmpty(kind));
            query.AddParameter("@Operation", NullIfEmpty(operation));
            query.AddParameter("@Outcome", NullIfEmpty(outcome));
            query.AddParameter("@ErrorLevel", NullIfEmpty(errorLevel));
        }

        private static object NullIfEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value;
        }

        private static object LikeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : "%" + value.Trim() + "%";
        }
    }
}
