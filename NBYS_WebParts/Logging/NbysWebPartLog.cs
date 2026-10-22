using DAO.Repositories.Ortak;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using Utility.HelperClasses;
using Utility.Logging;

namespace NBYS_WebParts.Logging
{
    internal static class NbysWebPartLog
    {
        private const string LoggedKey = "PortalApps.NBYS.LoggedErrors";
        private const string ReportedKey = "PortalApps.NBYS.ReportedErrors";

        internal static void RecordOpened(PortalLogContext context)
        {
            new PortalLogRepository().TryRecordWebPartOpened(context);
        }

        internal static void ReportError(PortalLogContext context, string operation, Exception error,
            string userMessage = "İşlem tamamlanamadı. Lütfen tekrar deneyin.")
        {
            if (error == null || error is ThreadAbortException) return;
            if (!WasHandled(LoggedKey, error)) PortalTechnicalLog.WriteError(context, operation, error);
            if (!WasHandled(ReportedKey, error))
                Messages.Error(userMessage, referenceCode: context.OperationId.ToString("N"));
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
            bool handled = false;
            for (Exception current = error; current != null; current = current.InnerException)
                if (!errors.Add(current)) handled = true;
            return handled;
        }
    }
}
