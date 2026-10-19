using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace Utility.HelperClasses
{
    /// <summary>
    /// Web Forms kullanıcı bildirimleri. Bütün mesajlar düz metindir; HTML çalıştırılmaz.
    /// İş servisleri sonuç/istisna döndürür; bu yardımcı sunum katmanından çağrılır.
    /// </summary>
    public static class Messages
    {
        private static readonly object QueueKey = new object();
        private static readonly Lazy<string> ClientScript = new Lazy<string>(
            () => ReadResource("Utility.HelperClasses.Messages.js"));
        private static readonly Lazy<string> ClientStyle = new Lazy<string>(
            () => ReadResource("Utility.HelperClasses.Messages.css"));

        /// <summary>Süre sıfırsa kullanıcı kapatana kadar, pozitifse belirtilen milisaniye kadar gösterir.</summary>
        public static void Success(string message, int autoCloseMilliseconds = 0)
        {
            Show(message, "Success", autoCloseMilliseconds);
        }

        public static void Info(string message, int autoCloseMilliseconds = 0)
        {
            Show(message, "Information", autoCloseMilliseconds);
        }

        /// <summary>Bilgi mesajını güvenli, ayrı bir bağlantıyla gösterir.</summary>
        public static void Info(string message, string linkText, string linkUrl, int autoCloseMilliseconds = 0)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            if (string.IsNullOrWhiteSpace(linkText))
                throw new ArgumentException("Bağlantı metni gereklidir.", "linkText");
            if (!IsSafeLink(linkUrl))
                throw new ArgumentException("Bağlantı adresi geçerli bir HTTP/HTTPS veya kökten başlayan yerel adres olmalıdır.", "linkUrl");

            Show(message, "Information", autoCloseMilliseconds, linkText, linkUrl);
        }

        public static void Warning(string message, int autoCloseMilliseconds = 0)
        {
            Show(message, "Warning", autoCloseMilliseconds);
        }

        public static void Error(string message, int autoCloseMilliseconds = 0, string referenceCode = null)
        {
            Show(message, "Error", autoCloseMilliseconds, referenceCode: referenceCode);
        }

        private static void Show(string message, string severity, int autoCloseMilliseconds,
            string linkText = null, string linkUrl = null, string referenceCode = null)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            if (autoCloseMilliseconds < 0)
                throw new ArgumentOutOfRangeException("autoCloseMilliseconds", "Kapanma süresi negatif olamaz.");

            HttpContext context = HttpContext.Current;
            Page page = context == null ? null : (context.CurrentHandler as Page ?? context.Handler as Page);
            if (page == null)
                throw new InvalidOperationException("Mesaj yalnızca bir Web Forms sayfasında gösterilebilir. İş katmanından sonuç veya istisna döndürün.");

            var queue = page.Items[QueueKey] as MessageQueue;
            if (queue == null)
            {
                queue = new MessageQueue();
                page.Items[QueueKey] = queue;
                page.PreRenderComplete += Flush;
            }
            if (queue.Flushed)
                throw new InvalidOperationException("Sayfanın mesaj hazırlama aşaması tamamlandı. Mesajı daha erken bir sayfa olayında oluşturun.");

            queue.Items.Add(new
            {
                id = "portalapps-message-" + Guid.NewGuid().ToString("N"),
                text = message,
                severity = severity,
                autoDismissMilliseconds = autoCloseMilliseconds,
                referenceCode = referenceCode,
                link = linkUrl == null ? null : new { text = linkText, url = linkUrl }
            });
        }

        private static void Flush(object sender, EventArgs args)
        {
            var page = (Page)sender;
            page.PreRenderComplete -= Flush;
            var queue = (MessageQueue)page.Items[QueueKey];
            queue.Flushed = true;

            // JSON is additionally encoded for an inline JavaScript string (including script end tags).
            string messages = HttpUtility.JavaScriptStringEncode(
                new JavaScriptSerializer().Serialize(queue.Items), true);
            string styles = HttpUtility.JavaScriptStringEncode(ClientStyle.Value, true);
            string script = ClientScript.Value + "\nwindow.PortalAppsMessagesV1.showMany(JSON.parse("
                + messages + ")," + styles + ");";

            // Initialization and all messages are emitted together, also for UpdatePanel responses.
            ScriptManager.RegisterStartupScript(page, typeof(Messages), "PortalApps.Messages.v1", script, true);
        }

        private static bool IsSafeLink(string url)
        {
            if (string.IsNullOrWhiteSpace(url) || url != url.Trim() || url.IndexOf('\\') >= 0)
                return false;
            foreach (char character in url)
                if (char.IsControl(character)) return false;

            string decoded = Uri.UnescapeDataString(url);
            if (decoded.IndexOf('\\') >= 0) return false;
            foreach (char character in decoded)
                if (char.IsControl(character)) return false;

            if (url.StartsWith("/", StringComparison.Ordinal))
                return !decoded.StartsWith("//", StringComparison.Ordinal)
                    && Uri.IsWellFormedUriString(url, UriKind.Relative);

            Uri absolute;
            return Uri.IsWellFormedUriString(url, UriKind.Absolute)
                && Uri.TryCreate(url, UriKind.Absolute, out absolute)
                && (absolute.Scheme == Uri.UriSchemeHttp || absolute.Scheme == Uri.UriSchemeHttps)
                && !string.IsNullOrEmpty(absolute.Host)
                && string.IsNullOrEmpty(absolute.UserInfo);
        }

        private static string ReadResource(string name)
        {
            using (Stream stream = typeof(Messages).Assembly.GetManifestResourceStream(name))
            {
                if (stream == null)
                    throw new InvalidOperationException("Mesaj bileşeninin uygulama kaynağı bulunamadı.");
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return reader.ReadToEnd();
            }
        }

        private sealed class MessageQueue
        {
            public readonly List<object> Items = new List<object>();
            public bool Flushed;
        }
    }
}
