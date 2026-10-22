using Model.Services.NBYS;
using System;
using Utility.HelperClasses;
using Utility.Logging;

namespace NBYS_WebParts.Logging
{
    internal static class NbysDocumentFeedback
    {
        internal static void Run(PortalLogContext context, NbysDocumentBatch batch, Action create,
            bool markSent, Action refresh, string prepared, Func<string> note = null, Func<bool> saved = null)
        {
            if (batch.Forbidden) { Messages.Warning("Seçilen bölge için belge işlemi yetkiniz yok."); return; }
            if (batch.Count == 0) { Messages.Warning("Seçilen dönemde belge hazırlanacak geçerli, kontrol edilmiş armağan bulunamadı."); return; }
            bool created = false, committed = false, refreshing = false;
            try
            {
                create();
                created = true;
                if (note != null) prepared += note();
                if (markSent)
                {
                    NbysDocumentWriteResult result = new NbysDocumentWriteService(context).MarkSent(batch);
                    if (result != NbysDocumentWriteResult.Completed)
                    {
                        Messages.Warning(prepared + " Gönderim durumu değiştirilmedi. Kayıtlar veya bölge yetkisi değişmiş olabilir; listeyi yenileyip belgeyi kontrol edin.");
                        return;
                    }
                    committed = true;
                }
                refreshing = true;
                refresh();
                Messages.Success(prepared + (committed ? " Seçiminize göre " + batch.Count + " kaydın durumu ‘Gönderildi’ olarak güncellendi." : " Gönderim durumu değiştirilmedi."), 5000);
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                if (System.Web.HttpContext.Current != null)
                    System.Web.HttpContext.Current.Items["PortalApps.NBYS.FailedWebPart." + context.WebPart] = true;
                string message = refreshing ? prepared + (committed ? " Gönderim durumu güncellendi." : " Gönderim durumu değiştirilmedi.") + " Ekran yenilenemedi. Sayfayı yenileyin."
                    : created ? prepared + " Gönderim durumu güncellenmedi. Dosyayı ve kayıtları kontrol edin."
                    : saved != null && saved() ? "Dosya kitaplığa kaydedildi; belge/etiket işlemi tamamlanamadı. Gönderim durumu güncellenmedi. Dosya listesini kontrol edin."
                    : "Belge işlemi tamamlanamadı. Dosya listesini kontrol edip tekrar deneyin.";
                NbysWebPartLog.ReportError(context, "NBYS." + context.WebPart + ".DocumentOperation", error, message);
            }
        }
    }
}
