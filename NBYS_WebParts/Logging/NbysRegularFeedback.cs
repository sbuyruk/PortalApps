using Model.Services.NBYS;
using System;
using System.Linq;
using Utility.HelperClasses;
using Utility.Logging;

namespace NBYS_WebParts.Logging
{
    internal static class NbysRegularFeedback
    {
        internal static void Show(PortalLogContext context, NbysRegularWriteResult result, string success, Action refresh)
        {
            if (result == NbysRegularWriteResult.Completed || result == NbysRegularWriteResult.ExistingCertificate || result == NbysRegularWriteResult.AlreadyMatched)
            {
                string message = result == NbysRegularWriteResult.ExistingCertificate ? "Belge zaten mevcut; yeniden oluşturulmadı. Bağlantılar kontrol edildi."
                    : result == NbysRegularWriteResult.AlreadyMatched ? "Kayıt zaten seçilen bağışçıyla eşleşmiş." : success;
                try { refresh(); }
                catch (System.Threading.ThreadAbortException) { throw; }
                catch (Exception error)
                {
                    if (System.Web.HttpContext.Current != null)
                        System.Web.HttpContext.Current.Items["PortalApps.NBYS.FailedWebPart." + context.WebPart] = true;
                    NbysWebPartLog.ReportError(context, "NBYS." + context.WebPart + ".RefreshAfterWrite", error, message + " Liste yenilenemedi. Sayfayı yenileyin.");
                    return;
                }
                if (result == NbysRegularWriteResult.Completed) Messages.Success(message, 4000);
                else Messages.Info(message);
                return;
            }
            string warning;
            switch (result)
            {
                case NbysRegularWriteResult.NotFound: warning = "Düzenli bağışçı veya bağışçı kaydı bulunamadı. Listeyi yenileyip tekrar seçin."; break;
                case NbysRegularWriteResult.InvalidSelection: warning = "Geçerli bir düzenli bağışçı ve bağışçı seçin."; break;
                case NbysRegularWriteResult.Inactive: warning = "Düzenli bağışçı kaydı aktif değil. İşlem yapılmadı."; break;
                case NbysRegularWriteResult.NoCertificateWanted: warning = "Bağışçı belge istemiyor. Belge oluşturulmadı."; break;
                case NbysRegularWriteResult.NotEligible: warning = "Belge yalnızca geçerli bir başlangıç tarihiyle 12 bağış ve katlarında oluşturulabilir."; break;
                case NbysRegularWriteResult.NoDonation: warning = "İlgili bağış yılında nakit bağış hareketi bulunamadı. Bağış kayıtlarını kontrol edin."; break;
                case NbysRegularWriteResult.Conflict: warning = "Eşleştirme veya bağlantı bilgileri değişmiş ya da tutarsız. Listeyi yenileyip kontrol edin; bağlı belgesi olan kaydı başka bağışçıya taşımayın."; break;
                default: throw new InvalidOperationException("Düzenli bağış işlem sonucu tanınamadı.");
            }
            Messages.Warning(warning);
        }

        internal static void Import(PortalLogContext context, NbysRegularImportResult result)
        {
            if (result.InvalidWorkbook) { Messages.Warning("Dosya okunamadı veya beklenen sütunlar bulunamadı. Düzenli bağışçı Excel şablonunu kontrol edin."); return; }
            string summary = string.Format("Yükleme sonucu: {0} kayıt eklendi, {1} kayıt güncellendi, {2} satır tutarı 1'den az olduğu için atlandı, {3} satır geçersiz, {4} satırda hata oluştu.",
                result.Created, result.Updated, result.Skipped, result.InvalidRows.Count, result.Errors.Count);
            if (result.Unmatched > 0) summary += " " + result.Unmatched + " kayıt için listeden bağışçı eşleştirmesi yapın.";
            if (result.InvalidRows.Count > 0) summary += " Kontrol edilecek veri satırları: " + string.Join(", ", result.InvalidRows.Take(20)) + (result.InvalidRows.Count > 20 ? "…" : ".") + " Tarih, sayı biçimleri ve aynı bağışçıya ait aktif kayıtları kontrol edin.";
            if (result.Errors.Count > 0)
            {
                NbysWebPartLog.ReportError(context, "NBYS.DuzenliBagisciDosyaYuklemeWP.Import", new AggregateException(result.Errors), summary + " Tamamlanan satırlar korundu. Sorunlu satırları kontrol edin.");
                return;
            }
            if (result.InvalidRows.Count > 0 || result.Unmatched > 0 || result.Created + result.Updated == 0) Messages.Warning(summary);
            else Messages.Success(summary, 4000);
        }
    }
}
