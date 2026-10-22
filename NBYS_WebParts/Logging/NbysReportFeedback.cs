using Model.Services.NBYS;
using System;
using Utility.HelperClasses;
using Utility.Logging;

namespace NBYS_WebParts.Logging
{
    internal sealed class NbysReportInputException : Exception
    {
        internal NbysReportInputException(string message) : base(message) { }
    }

    internal static class NbysReportFeedback
    {
        internal static void Execute(PortalLogContext context, string operation, Action action)
        {
            NbysWebPartLog.ExecuteAction(context, operation, () =>
            {
                try { action(); }
                catch (NbysReportInputException warning)
                {
                    if (System.Web.HttpContext.Current != null)
                        System.Web.HttpContext.Current.Items["PortalApps.NBYS.FailedWebPart." + context.WebPart] = true;
                    Messages.Warning(warning.Message);
                }
            });
        }

        internal static bool CanContinue(NbysReportWriteResult result)
        {
            if (result == NbysReportWriteResult.Completed || result == NbysReportWriteResult.Unchanged) return true;
            string message;
            switch (result)
            {
                case NbysReportWriteResult.InvalidSelection: message = "Geçerli bir kayıt seçin."; break;
                case NbysReportWriteResult.NotFound: message = "Kayıt bulunamadı. Listeyi yenileyip tekrar seçin."; break;
                case NbysReportWriteResult.Forbidden: message = "Bu kayıt üzerinde işlem yapma yetkiniz bulunmuyor."; break;
                case NbysReportWriteResult.Changed: message = "Armağan hak edişi değişti. Listeyi yenileyip tekrar seçin."; break;
                case NbysReportWriteResult.NoEntitlement: message = "Bağışçının bu işlem için uygun armağan hak edişi bulunmuyor."; break;
                case NbysReportWriteResult.DeclinedCertificate: message = "Bağışçı belge istemiyor. Armağan oluşturulmadı."; break;
                case NbysReportWriteResult.RefundRequiresReview: message = "Bağışlar arasında iade kaydı var. Madalya oluşturmadan önce bağış ve iade kayıtlarını kontrol edin."; break;
                case NbysReportWriteResult.AlreadyAwarded: message = "Bu madalya daha önce oluşturulmuş. Mevcut armağan kaydını kontrol edin."; break;
                case NbysReportWriteResult.InvalidDate: message = "Geçerli bir armağan tarihi girin."; break;
                case NbysReportWriteResult.InvalidStatus: message = "Geçerli bir armağan durumu seçin."; break;
                default: throw new InvalidOperationException("Rapor işlem sonucu tanınamadı.");
            }
            Messages.Warning(message);
            return false;
        }

        internal static void Write(PortalLogContext context, NbysReportWriteResult result, string success, Action refresh)
        {
            if (!CanContinue(result)) return;
            string message = result == NbysReportWriteResult.Unchanged ? "Kayıt zaten bu bilgileri içeriyor. Değişiklik yapılmadı." : success;
            try { refresh(); }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                if (System.Web.HttpContext.Current != null)
                    System.Web.HttpContext.Current.Items["PortalApps.NBYS.FailedWebPart." + context.WebPart] = true;
                NbysWebPartLog.ReportError(context, "NBYS." + context.WebPart + ".RefreshAfterWrite", error,
                    message + " Ekran yenilenemedi. Kaydı yeniden eklemeyin; sayfayı yenileyin.");
                return;
            }
            if (result == NbysReportWriteResult.Completed) Messages.Success(message, 4000);
            else Messages.Info(message);
        }
    }
}
