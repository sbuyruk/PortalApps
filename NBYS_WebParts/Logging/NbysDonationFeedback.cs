using Model.Services.NBYS;
using System;
using System.Web;
using Utility.HelperClasses;
using Utility.Logging;

namespace NBYS_WebParts.Logging
{
    internal static class NbysDonationFeedback
    {
        internal static void Show(PortalLogContext context, NbysDonationWriteResult result,
            string successMessage, Action refresh)
        {
            if (result == NbysDonationWriteResult.Completed || result == NbysDonationWriteResult.CompletedWithRetainedGift)
            {
                if (result == NbysDonationWriteResult.CompletedWithRetainedGift) successMessage += " Bağlı armağan korundu.";
                try { refresh(); }
                catch (System.Threading.ThreadAbortException) { throw; }
                catch (Exception error)
                {
                    if (HttpContext.Current != null)
                        HttpContext.Current.Items["PortalApps.NBYS.FailedWebPart." + context.WebPart] = true;
                    NbysWebPartLog.ReportError(context, "NBYS." + context.WebPart + ".RefreshAfterWrite", error,
                        successMessage + " Liste yenilenemedi. Sayfayı yenileyin.");
                    return;
                }
                Messages.Success(successMessage, 4000);
                return;
            }
            string message;
            switch (result)
            {
                case NbysDonationWriteResult.NotFound: message = "İşlem yapılacak kayıt bulunamadı. Listeyi yenileyip tekrar seçin."; break;
                case NbysDonationWriteResult.Forbidden: message = "Bu kayıt üzerinde işlem yapma yetkiniz yok."; break;
                case NbysDonationWriteResult.Expired:
                    message = context.WebPart == "BagisIadesiWP"
                        ? "Bağışın iki yıllık iade süresi dolmuş. İade kaydedilemez."
                        : "Bağışın 240 günlük silme süresi dolmuş. Silme yapılamadı.";
                    break;
                case NbysDonationWriteResult.AlreadyRefunded: message = "Bu bağış daha önce iade edilmiş. Değişiklik için iade düzenleme işlemini kullanın."; break;
                case NbysDonationWriteResult.NotRefunded: message = "Bu bağış için düzenlenecek bir iade kaydı bulunmuyor."; break;
                case NbysDonationWriteResult.InvalidDate: message = "Geçerli bir iade tarihi girin."; break;
                case NbysDonationWriteResult.InvalidSelection: message = "Asıl bağışçıyı ve birleştirilecek bağışçıları seçin."; break;
                case NbysDonationWriteResult.SameDonor: message = "Asıl bağışçı kendisiyle birleştirilemez. Seçiminizi kontrol edin."; break;
                case NbysDonationWriteResult.Conflict: message = "Kayıt bağlantıları değişmiş veya tutarsız. Listeyi yenileyip tekrar seçin."; break;
                default: throw new InvalidOperationException("İşlem sonucu tanınamadı.");
            }
            Messages.Warning(message);
        }
    }
}
