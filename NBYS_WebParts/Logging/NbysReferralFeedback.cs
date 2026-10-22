using Model.NBYS;
using Model.Services.NBYS;
using System;
using Utility.HelperClasses;
using Utility.Logging;

namespace NBYS_WebParts.Logging
{
    internal static class NbysReferralFeedback
    {
        internal static bool CanContinue(NbysReferralWriteResult result)
        {
            if (result == NbysReferralWriteResult.Completed || result == NbysReferralWriteResult.Unchanged) return true;
            string message;
            switch (result)
            {
                case NbysReferralWriteResult.NotFound: message = "Teşekkür kaydı bulunamadı. Listeyi yenileyip tekrar seçin."; break;
                case NbysReferralWriteResult.InvalidSelection: message = "Geçerli bir teşekkür kaydı seçin."; break;
                case NbysReferralWriteResult.NameRequired: message = "Belge düzenlenecek kişinin adını girin."; break;
                case NbysReferralWriteResult.InvalidDate: message = "Geçerli bir belge tarihi girin."; break;
                case NbysReferralWriteResult.InvalidIdentity: message = "Kimlik numarasını yalnızca rakamlarla girin veya boş bırakın."; break;
                default: throw new InvalidOperationException("Teşekkür işlem sonucu tanınamadı.");
            }
            Messages.Warning(message);
            return false;
        }

        internal static void Write(PortalLogContext context, NbysReferralWriteResult result, string success, Action refresh)
        {
            if (!CanContinue(result)) return;
            string message = result == NbysReferralWriteResult.Unchanged ? "Kayıt zaten bu bilgileri içeriyor. Değişiklik yapılmadı." : success;
            try { refresh(); }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                if (System.Web.HttpContext.Current != null)
                    System.Web.HttpContext.Current.Items["PortalApps.NBYS.FailedWebPart." + context.WebPart] = true;
                NbysWebPartLog.ReportError(context, "NBYS." + context.WebPart + ".RefreshAfterWrite", error,
                    message + " Ekran yenilenemedi. Sayfayı yenileyin.");
                return;
            }
            if (result == NbysReferralWriteResult.Completed) Messages.Success(message, 4000);
            else Messages.Info(message);
        }

        internal static void Document(PortalLogContext context, BagisaVesileOlanTesekkur item,
            Action<NbysReferralDocumentResult> show, string savedRecordMessage)
        {
            bool fileSaved = false;
            try
            {
                NbysReferralDocumentResult document = NbysReferralDocument.Create(item, () => fileSaved = true);
                show(document);
                Messages.Success(savedRecordMessage + " Teşekkür belgesi hazırlandı. Dosya bağlantısından indirebilirsiniz.", 5000);
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                if (System.Web.HttpContext.Current != null)
                    System.Web.HttpContext.Current.Items["PortalApps.NBYS.FailedWebPart." + context.WebPart] = true;
                string message = savedRecordMessage + (fileSaved
                    ? " Dosya kitaplığa kaydedildi; ekran tamamlanamadı. İndirme bağlantısını veya belge kitaplığını kontrol edin."
                    : " Belge dosyası hazırlanamadı. Kaydı yeniden eklemeyin; aynı kayıt için belge oluşturmayı tekrar deneyin.");
                NbysWebPartLog.ReportError(context, "NBYS." + context.WebPart + ".CreateDocument", error, message);
            }
        }
    }
}
