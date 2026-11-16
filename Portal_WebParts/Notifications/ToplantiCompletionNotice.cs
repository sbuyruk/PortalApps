using Model.Ortak;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Utility.HelperClasses;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Portal_WebParts.Notifications
{
    internal enum ToplantiCompletion { Saved = 1, Updated = 2, Deleted = 3 }

    internal static class ToplantiCompletionNotice
    {
        private const string Parameter = "ToplantiSonuc";
        private const string CookiePrefix = "PortalApps.ToplantiSonuc.";
        private const string Purpose = "PortalApps.ToplantiCompletion.v1";

        internal static string Message(ToplantiCompletion completion)
        {
            switch (completion)
            {
                case ToplantiCompletion.Saved: return "Toplantı kaydedildi.";
                case ToplantiCompletion.Updated: return "Toplantı güncellendi.";
                case ToplantiCompletion.Deleted: return "Toplantı silindi.";
                default: throw new ArgumentOutOfRangeException("completion", "Toplantı işlem sonucu geçersiz.");
            }
        }

        internal static void Redirect(Page page, PortalLogContext context, ToplantiCompletion completion, int meetingId)
        {
            Message(completion); // Validate the fixed result before issuing a notice.
            if (meetingId <= 0) throw new ArgumentOutOfRangeException("meetingId", "Toplantı numarası geçersiz.");
            if (context == null || string.IsNullOrWhiteSpace(context.UserLogin))
                throw new InvalidOperationException("Toplantı işlem bildirimi için kullanıcı bilgisi bulunamadı.");

            string listPath = VirtualPathUtility.Combine(VirtualPathUtility.GetDirectory(page.Request.Path),
                ProjeConstants.PAGE_TOPLANTI_LIST);
            string token = Guid.NewGuid().ToString("N");
            DateTime expires = DateTime.UtcNow.AddMinutes(5);
            string payload = ((int)completion).ToString(CultureInfo.InvariantCulture) + "|"
                + meetingId.ToString(CultureInfo.InvariantCulture) + "|" + expires.Ticks.ToString(CultureInfo.InvariantCulture);
            byte[] protectedData = MachineKey.Protect(Encoding.UTF8.GetBytes(payload), Purpose,
                "User:" + context.UserLogin, "Path:" + listPath, "Token:" + token);

            // One cookie per operation avoids two browser tabs overwriting each other's result.
            // It is scoped to the list page; no SharePoint session state or server-local cache is needed.
            HttpCookie cookie = Cookie(page, token, listPath, HttpServerUtility.UrlTokenEncode(protectedData));
            cookie.Expires = expires;
            page.Response.Cookies.Set(cookie);

            string url = listPath + "?" + Parameter + "=" + token;
            if (completion != ToplantiCompletion.Deleted)
                url += "&SecilenToplantiId=" + meetingId.ToString(CultureInfo.InvariantCulture);
            page.Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        internal static void ShowOnce(Page page, PortalLogContext context)
        {
            Guid id;
            if (context == null || string.IsNullOrWhiteSpace(context.UserLogin)
                || !Guid.TryParseExact(page.Request.QueryString[Parameter], "N", out id)) return;
            string token = id.ToString("N");
            string key = CookiePrefix + token;
            if (page.Items[key] != null) return;
            page.Items[key] = true;

            HttpCookie cookie = page.Request.Cookies[key];
            if (cookie == null) return;
            string value = cookie.Value;
            // Consume on the destination GET. Refresh and later visits no longer carry this notice.
            HttpCookie expired = Cookie(page, token, page.Request.Path, string.Empty);
            expired.Expires = DateTime.UtcNow.AddDays(-1);
            page.Response.Cookies.Set(expired);

            ToplantiCompletion completion;
            if (!TryRead(value, token, page.Request.Path, context.UserLogin, out completion)) return;
            page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            page.Response.Cache.SetNoStore();
            Messages.Success(Message(completion) + " E-posta bildirimleri gönderim sırasına alındı.");
            UtilityHelper.ScriptCalistir(@"(function () {
                try {
                    if (!window.URL || !window.history || !window.history.replaceState) return;
                    var url = new URL(window.location.href);
                    url.searchParams.delete('ToplantiSonuc');
                    window.history.replaceState(window.history.state, '', url.pathname + url.search + url.hash);
                } catch (ignored) { }
            })();");
        }

        private static bool TryRead(string value, string token, string path, string user,
            out ToplantiCompletion completion)
        {
            completion = default(ToplantiCompletion);
            if (string.IsNullOrEmpty(value) || value.Length > 2048) return false;
            try
            {
                byte[] data = HttpServerUtility.UrlTokenDecode(value);
                if (data == null) return false;
                byte[] payload = MachineKey.Unprotect(data, Purpose, "User:" + user, "Path:" + path, "Token:" + token);
                if (payload == null) return false;
                string[] parts = Encoding.UTF8.GetString(payload).Split('|');
                int operation, meetingId;
                long expires;
                if (parts.Length != 3
                    || !int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out operation)
                    || !Enum.IsDefined(typeof(ToplantiCompletion), operation)
                    || !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out meetingId) || meetingId <= 0
                    || !long.TryParse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture, out expires)
                    || expires <= DateTime.UtcNow.Ticks) return false;
                completion = (ToplantiCompletion)operation;
                return true;
            }
            catch (CryptographicException) { return false; }
            catch (FormatException) { return false; }
            catch (ArgumentException) { return false; }
        }

        private static HttpCookie Cookie(Page page, string token, string path, string value)
        {
            return new HttpCookie(CookiePrefix + token, value)
            {
                Path = path,
                HttpOnly = true,
                Secure = page.Request.IsSecureConnection,
                SameSite = SameSiteMode.Lax
            };
        }
    }
}
