using Model.Services.NBYS;
using System;
using Utility.HelperClasses;
using Utility.Logging;

namespace NBYS_WebParts.Logging
{
    internal static class NbysFtkFeedback
    {
        internal static void Show(PortalLogContext context, NbysFtkWriteResult result, string success, Action refresh)
        {
            if (result == NbysFtkWriteResult.Completed)
            {
                try { refresh(); }
                catch (System.Threading.ThreadAbortException) { throw; }
                catch (Exception error)
                {
                    if (System.Web.HttpContext.Current != null)
                        System.Web.HttpContext.Current.Items["PortalApps.NBYS.FailedWebPart." + context.WebPart] = true;
                    NbysWebPartLog.ReportError(context, "NBYS." + context.WebPart + ".RefreshAfterWrite", error,
                        success + " Ekran yenilenemedi. Sayfayı yenileyin.");
                    return;
                }
                Messages.Success(success, 4000);
                return;
            }
            string message;
            switch (result)
            {
                case NbysFtkWriteResult.NotFound: message = "Kişi kaydı bulunamadı. Listeyi yenileyip tekrar seçin."; break;
                case NbysFtkWriteResult.InvalidName: message = "Kişinin adını girin."; break;
                case NbysFtkWriteResult.InvalidSurname: message = "Kişinin soyadını girin."; break;
                case NbysFtkWriteResult.InvalidLocation: message = "Geçerli bir il ve bu ile ait ilçe veya valilik seçin."; break;
                case NbysFtkWriteResult.InvalidDate: message = "Geçerli bir kuruluş tarihi ve güncelleme tarihi girin."; break;
                case NbysFtkWriteResult.DuplicateIdentity: message = "Bu T.C. kimlik numarasıyla kayıtlı başka bir kişi var. Mevcut kaydı kontrol edin."; break;
                case NbysFtkWriteResult.DuplicateUpdateDate: message = "Bu tarihte FTK kaydı zaten var. Son kaydı düzeltmek için 'Bu Bir Kayıt Düzeltmesidir' seçeneğini kullanın."; break;
                case NbysFtkWriteResult.NoSnapshotToCorrect: message = "Düzeltilecek FTK geçmişi bulunamadı. Yeni kayıt için kayıt düzeltmesi seçimini kaldırın."; break;
                case NbysFtkWriteResult.NoActiveMembers: message = "İşlem yapılacak aktif üye bulunamadı. Üye listesini kontrol edin."; break;
                case NbysFtkWriteResult.ReferencedPerson: message = "Kişinin FTK geçmişi bulunduğu için kayıt silinmedi. Gerekirse üyelik durumunu aktif değil olarak güncelleyin."; break;
                case NbysFtkWriteResult.Conflict: message = "FTK veya üye bilgileri değişmiş ya da tutarsız. Listeyi yenileyip tekrar deneyin."; break;
                default: throw new InvalidOperationException("FTK işlem sonucu tanınamadı.");
            }
            Messages.Warning(message);
        }
    }
}
