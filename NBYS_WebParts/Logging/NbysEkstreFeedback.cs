using Model.Services.NBYS;
using System;
using System.Linq;
using Utility.HelperClasses;
using Utility.Logging;

namespace NBYS_WebParts.Logging
{
    internal static class NbysEkstreFeedback
    {
        internal static void WriteResult(PortalLogContext context, NbysEkstreWriteResult result, string success, Action refresh)
        {
            if (result == NbysEkstreWriteResult.Completed)
            {
                Batch(context, new ExceptionHelper { SuccessCount = 1 }, success, refresh);
                return;
            }
            switch (result)
            {
                case NbysEkstreWriteResult.NotFound: Messages.Warning("Ekstre kaydı bulunamadı. Listeyi yenileyip tekrar seçin."); break;
                case NbysEkstreWriteResult.AlreadyTransferred: Messages.Warning("Seçilen kayıtlardan biri bağışa aktarılmış. İşlem yapılmadı; listeyi yenileyin."); break;
                case NbysEkstreWriteResult.InvalidSelection: Messages.Warning("İşlem için geçerli bir kayıt ve gerekiyorsa bağışçı seçin."); break;
                case NbysEkstreWriteResult.InvalidDonor: Messages.Warning("Seçilen bağışçı bulunamadı. Bağışçıyı tekrar seçin."); break;
                default: throw new InvalidOperationException("Ekstre işlem sonucu tanınamadı.");
            }
        }

        internal static void Batch(PortalLogContext context, ExceptionHelper result, string summary, Action refresh)
        {
            Exception refreshError = null;
            try { refresh(); }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error) { refreshError = error; }
            if (refreshError != null && System.Web.HttpContext.Current != null)
                System.Web.HttpContext.Current.Items["PortalApps.NBYS.FailedWebPart." + context.WebPart] = true;
            NbysImportValidationException[] validation = result.Exceptions.OfType<NbysImportValidationException>().ToArray();
            foreach (NbysImportValidationException warning in validation)
                PortalTechnicalLog.WriteError(context, "NBYS." + context.WebPart + ".FileValidation", warning);
            if (validation.Length > 0) summary += " " + string.Join(" ", validation.Select(error => error.Message).Distinct());
            Exception[] errors = result.Exceptions.Where(error => error != null && !(error is ImportAlreadyProcessedException)
                && !(error is NbysImportValidationException)).ToArray();
            if (errors.Length > 0 || (result.ErrorCount > 0 && result.Exceptions.Count == 0) || refreshError != null)
            {
                string message = summary + (refreshError == null ? " İşlenemeyen kayıtları kontrol edip tekrar deneyin." : " Liste yenilenemedi. Sayfayı yenileyin.");
                var allErrors = errors.Concat(refreshError == null ? new Exception[0] : new[] { refreshError }).ToArray();
                NbysWebPartLog.ReportError(context, "NBYS." + context.WebPart + ".Batch", new AggregateException(allErrors), message);
                return;
            }
            if (validation.Length > 0 || result.AlreadyProcessedCount > 0 || result.SkippedCount > 0 || result.SuccessCount == 0)
                Messages.Warning(summary);
            else Messages.Success(summary, 4000);
        }
    }
}
