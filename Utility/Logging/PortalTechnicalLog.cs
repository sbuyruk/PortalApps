using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Utility.HelperClasses;

namespace Utility.Logging
{
    /// <summary>
    /// Writes technical events to SharePoint ULS. Database audit and activity sinks are added
    /// separately so they can honor the caller's SQL transaction.
    /// </summary>
    public static class PortalTechnicalLog
    {
        public static void WriteError(PortalLogContext context, string name, Exception error)
        {
            try
            {
                if (context == null || error == null || !PortalLogSettings.IsEnabled(context)) return;
                PortalLogRecord record = new PortalLogRecord(PortalLogKind.Error, context, name, "Failed");
                Write(record, TraceSeverity.Unexpected,
                    error.GetType().FullName + " " + (error.StackTrace ?? string.Empty));
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
                Write(record, succeeded ? TraceSeverity.Medium : TraceSeverity.Unexpected,
                    "SqlOperation=" + operation);
            }
            catch (Exception)
            {
                // Logging must not replace the SQL result or exception.
            }
        }

        private static void Write(PortalLogRecord record, TraceSeverity severity, string detail)
        {
            Dictionary<string, object> fields = new Dictionary<string, object>
            {
                { "OccurredUtc", record.OccurredUtc },
                { "Kind", record.Kind.ToString() },
                { "Project", record.Context.Project },
                { "WebPart", record.Context.WebPart },
                { "User", record.Context.UserLogin },
                { "Page", record.Context.PagePath },
                { "OperationId", record.Context.OperationId.ToString("N") },
                { "Name", record.Name },
                { "Outcome", record.Outcome },
                { "Detail", detail }
            };
            if (record.DurationMilliseconds.HasValue)
                fields.Add("DurationMs", record.DurationMilliseconds.Value);

            LogHelper.WritePortalTrace(severity, new JavaScriptSerializer().Serialize(fields));
        }
    }
}
