using DAO.Repositories.Ortak;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using Utility.HelperClasses;
using Utility.Logging;

namespace MTS_WebParts.Logging
{
    internal sealed class MtsInputException : Exception
    {
        internal MtsInputException(string message) : base(message) { }
    }

    internal static class MtsWebPartLog
    {
        private const string LoggedKey = "PortalApps.MTS.LoggedErrors";
        private const string ReportedKey = "PortalApps.MTS.ReportedErrors";

        internal static bool HasFailed(PortalLogContext context)
        {
            HttpContext http = HttpContext.Current;
            return http != null && object.Equals(http.Items["PortalApps.MTS.FailedWebPart." + context.WebPart], true);
        }

        internal static void ExecuteAction(PortalLogContext context, string operation, Action action)
        {
            HttpContext http = HttpContext.Current;
            string failedKey = "PortalApps.MTS.FailedWebPart." + context.WebPart;
            if (http != null && object.Equals(http.Items[failedKey], true)) return;
            try { action(); }
            catch (MtsInputException warning)
            {
                if (http != null) http.Items[failedKey] = true;
                Messages.Warning(warning.Message);
            }
            catch (ThreadAbortException) { throw; }
            catch (Exception error)
            {
                // Do not run later postback handlers after this WebPart failed during load.
                if (http != null) http.Items[failedKey] = true;
                ReportError(context, context.Project + "." + context.WebPart + "." + operation, error);
            }
        }

        internal static void ReadDateRange(string from, string to, out DateTime start, out DateTime end)
        {
            var culture = System.Globalization.CultureInfo.GetCultureInfo("tr-TR");
            if (!DateTime.TryParse(from, culture, System.Globalization.DateTimeStyles.None, out start)
                || !DateTime.TryParse(to, culture, System.Globalization.DateTimeStyles.None, out end)
                || start < new DateTime(1753, 1, 1) || end < start || end.Date == DateTime.MaxValue.Date)
                throw new MtsInputException("Geçerli bir tarih aralığı girin. Bitiş tarihi başlangıçtan önce olamaz.");
            start = start.Date;
            // SQL datetime rounds finer values up; .997 stays within the selected day.
            end = end.Date.AddDays(1).AddMilliseconds(-3);
        }

        internal static void RecordOpened(PortalLogContext context)
        {
            new PortalLogRepository().TryRecordWebPartOpened(context);
        }

        internal static void ReportError(PortalLogContext context, string operation, Exception error,
            string userMessage = "İşlem tamamlanamadı. Lütfen tekrar deneyin.")
        {
            if (error == null || error is ThreadAbortException) return;
            if (HttpContext.Current != null)
                HttpContext.Current.Items["PortalApps.MTS.FailedWebPart." + context.WebPart] = true;
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
