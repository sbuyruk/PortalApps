using DAO.Repositories.Ortak;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using Utility.HelperClasses;
using Utility.Logging;

namespace TBYS_WebParts.Logging
{
    internal static class TbysWebPartLog
    {
        private const string LoggedErrorsKey = "PortalApps.TBYS.LoggedErrors";
        private const string ReportedErrorsKey = "PortalApps.TBYS.ReportedErrors";

        internal static void RecordOpened(PortalLogContext context)
        {
            new PortalLogRepository().TryRecordWebPartOpened(context);
        }

        internal static void WriteError(PortalLogContext context, string operation, Exception error)
        {
            if (error == null || error is ThreadAbortException) return;
            if (WasHandled(LoggedErrorsKey, error)) return;
            PortalTechnicalLog.WriteError(context, operation, error);
        }

        internal static void ReportError(PortalLogContext context, string operation, Exception error)
        {
            if (error == null || error is ThreadAbortException) return;
            WriteError(context, operation, error);
            if (!WasHandled(ReportedErrorsKey, error)) ShowError(context);
        }

        internal static void ReportErrors(PortalLogContext context, string operation, IEnumerable<Exception> errors)
        {
            bool showMessage = false;
            foreach (Exception error in errors)
            {
                if (error == null || error is ThreadAbortException) continue;
                WriteError(context, operation, error);
                if (!WasHandled(ReportedErrorsKey, error)) showMessage = true;
            }
            if (showMessage) ShowError(context);
        }

        private static void ShowError(PortalLogContext context)
        {
            Messages.Error(PortalTechnicalLog.DefaultUserMessage + " Başvuru kodu: " + context.OperationId.ToString("N"));
        }

        private static bool WasHandled(string key, Exception error)
        {
            HttpContext http = HttpContext.Current;
            if (http == null) return false;
            HashSet<Exception> errors = http.Items[key] as HashSet<Exception>;
            if (errors == null)
            {
                errors = new HashSet<Exception>();
                http.Items[key] = errors;
            }
            // A wrapper and its inner exception describe the same failure chain.
            bool handled = false;
            for (Exception current = error; current != null; current = current.InnerException)
                if (!errors.Add(current)) handled = true;
            return handled;
        }
    }
}
