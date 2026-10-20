using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

namespace Utility.Logging
{
    /// <summary>Opt-in, request-local diagnostics. Never persists or exposes exception messages.</summary>
    public static class PortalLogWriteDiagnostics
    {
        public const string EnabledKey = "PortalApps.TBYS.LogDiagnostics.Enabled";
        public const string EntriesKey = "PortalApps.TBYS.LogDiagnostics.Entries";

        public static void Record(string step, string outcome)
        {
            try
            {
                HttpContext http = HttpContext.Current;
                if (http == null || !object.Equals(http.Items[EnabledKey], true)) return;
                List<string> entries = http.Items[EntriesKey] as List<string>;
                if (entries == null)
                {
                    entries = new List<string>();
                    http.Items[EntriesKey] = entries;
                }
                if (entries.Count < 50) entries.Add(step + ": " + outcome);
            }
            catch (Exception)
            {
                // Diagnostics must not change the application result.
            }
        }

        public static void RecordFailure(string step, Exception error)
        {
            try
            {
                SqlException sql = error as SqlException;
                if (sql != null)
                {
                    string reason;
                    switch (sql.Number)
                    {
                        case 229: reason = "Veritabanı erişim yetkisi yetersiz."; break;
                        case 208: reason = "Hedef tablo veya veritabanı nesnesi bulunamadı."; break;
                        case 207: reason = "Beklenen veritabanı alanı bulunamadı."; break;
                        case 547: reason = "Veritabanı kısıtı kaydı reddetti."; break;
                        case 8152:
                        case 2628: reason = "Bir alanın değeri izin verilen uzunluğu aşıyor."; break;
                        case 18456: reason = "Veritabanı oturumu açılamadı."; break;
                        case 4060: reason = "Hedef veritabanı açılamadı."; break;
                        case -2: reason = "Veritabanı işlemi zaman aşımına uğradı."; break;
                        default: reason = "Veritabanı işlemi başarısız oldu."; break;
                    }
                    Record(step, reason + " SQL hata kodu: " + sql.Number);
                }
                else if (error is ConfigurationErrorsException)
                    Record(step, "Uygulama yapılandırması okunamadı veya eksik.");
                else if (error is ArgumentException)
                    Record(step, "Log alanı doğrulaması başarısız oldu.");
                else
                    Record(step, "İşlem başarısız oldu; hata türü: " + (error == null ? "Bilinmiyor" : error.GetType().Name));
            }
            catch (Exception)
            {
                // Never replace an application or logging exception.
            }
        }
    }
}
