using Model.Services.NBYS;
using System;
using Utility.HelperClasses;
using Utility.Logging;

namespace NBYS_WebParts.Logging
{
    internal static class NbysSmsFeedback
    {
        internal static bool CanContinue(NbysSmsWriteResult result)
        {
            if (result == NbysSmsWriteResult.Completed || result == NbysSmsWriteResult.Unchanged) return true;
            string message;
            switch (result)
            {
                case NbysSmsWriteResult.InvalidYear: message = "Listeden geçerli bir yıl seçin."; break;
                case NbysSmsWriteResult.InvalidSelection: message = "Güncellenecek ayı çizelgeden yeniden seçin."; break;
                case NbysSmsWriteResult.InvalidCounts: message = "SMS adetlerini 0 ile 2.147.483.647 arasında tam sayı olarak girin."; break;
                case NbysSmsWriteResult.NotFound: message = "Aylık kayıt bulunamadı. Çizelgeyi yenileyip tekrar seçin."; break;
                case NbysSmsWriteResult.Forbidden: message = "SMS çizelgesini değiştirme yetkiniz bulunmuyor."; break;
                case NbysSmsWriteResult.Conflict: message = "Aylık kayıt siz düzenlerken değişti. Çizelgeyi yenileyip güncel kaydı tekrar açın."; break;
                case NbysSmsWriteResult.InvalidYearData: message = "Çizelgede aynı aya ait birden fazla kayıt veya geçersiz ay var. Kayıtları kontrol ettirin; değişiklik yapılmadı."; break;
                case NbysSmsWriteResult.Busy: message = "Bu yılın çizelgesi başka bir işlem tarafından kullanılıyor. Biraz sonra tekrar deneyin."; break;
                default: throw new InvalidOperationException("SMS çizelgesi işlem sonucu tanınamadı.");
            }
            Messages.Warning(message);
            return false;
        }

        internal static void Write(PortalLogContext context, NbysSmsWriteResult result, string success, Action refresh, bool showUnchanged = true)
        {
            if (!CanContinue(result)) return;
            string message = result == NbysSmsWriteResult.Unchanged ? "SMS adetleri zaten bu değerlerde. Değişiklik yapılmadı." : success;
            try { refresh(); }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                if (System.Web.HttpContext.Current != null)
                    System.Web.HttpContext.Current.Items["PortalApps.NBYS.FailedWebPart." + context.WebPart] = true;
                NbysWebPartLog.ReportError(context, "NBYS." + context.WebPart + ".RefreshAfterWrite", error,
                    (result == NbysSmsWriteResult.Completed ? success : "Kayıtlar değiştirilmedi.") + " Çizelge yenilenemedi. Sayfayı yenileyin.");
                return;
            }
            if (result == NbysSmsWriteResult.Completed) Messages.Success(message, 4000);
            else if (showUnchanged) Messages.Info(message);
        }
    }
}
