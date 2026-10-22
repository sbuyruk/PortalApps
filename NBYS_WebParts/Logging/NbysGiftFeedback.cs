using Model.Services.NBYS;
using System;
using Utility.HelperClasses;
using Utility.Logging;

namespace NBYS_WebParts.Logging
{
    internal static class NbysGiftFeedback
    {
        internal static void Show(PortalLogContext context, NbysGiftWriteResult result, string success, Action refresh)
        {
            if (result == NbysGiftWriteResult.Completed || result == NbysGiftWriteResult.Unchanged)
            {
                string message = result == NbysGiftWriteResult.Unchanged ? "Kayıt zaten bu bilgileri içeriyor. Değişiklik yapılmadı." : success;
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
                if (result == NbysGiftWriteResult.Completed) Messages.Success(message, 4000);
                else Messages.Info(message);
                return;
            }
            string warning;
            switch (result)
            {
                case NbysGiftWriteResult.InvalidSelection: warning = "Geçerli bir armağan kaydı seçin."; break;
                case NbysGiftWriteResult.NotFound: warning = "Armağan veya bağışçı kaydı bulunamadı. Listeyi yenileyip tekrar seçin."; break;
                case NbysGiftWriteResult.Forbidden: warning = "Bu armağan kaydı için işlem yetkiniz yok. Bölge bilginizi kontrol edin."; break;
                case NbysGiftWriteResult.InvalidDefinition: warning = "Armağan türü bulunamadı. Sayfayı yenileyip tekrar seçin."; break;
                case NbysGiftWriteResult.InvalidAmount: warning = "Bağış miktarı sıfır veya pozitif bir sayı olmalıdır."; break;
                case NbysGiftWriteResult.InvalidDate: warning = "Geçerli bir bağış tarihi girin."; break;
                case NbysGiftWriteResult.InvalidStatus: warning = "Geçerli bir armağan durumu seçin."; break;
                case NbysGiftWriteResult.RefundProtected: warning = "Para iadesi durumunu bu ekrandan değiştiremezsiniz. Para iadesi için Bağış İadesi ekranını kullanın."; break;
                case NbysGiftWriteResult.NotSent: warning = "Belge iadesi yalnızca durumu ‘Gönderildi’ olan armağanlarda yapılabilir. Listeyi yenileyip durumu kontrol edin."; break;
                case NbysGiftWriteResult.Conflict: warning = "Kaydın bağışçı bağlantısı değişmiş. Sayfayı yenileyip tekrar deneyin."; break;
                default: throw new InvalidOperationException("Armağan işlem sonucu tanınamadı.");
            }
            Messages.Warning(warning);
        }
    }
}
