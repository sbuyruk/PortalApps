using DAO.Ortak;
using DAO.Repositories.TBYS;
using Microsoft.SharePoint;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.Logging;

namespace TBYS_WebParts.Logging
{
    internal static class TbysLogDiagnostics
    {
        internal static void Attach(WebPart owner, PortalLogContext context)
        {
            HttpContext http = HttpContext.Current;
            if (http == null || http.Request.QueryString["tbysLogDiagnostics"] != "1") return;
            try
            {
                if (!IsAuthorized()) return;
                http.Items[PortalLogWriteDiagnostics.EnabledKey] = true;
                owner.PreRender += delegate
                {
                    // Recheck before rendering technical diagnostic metadata.
                    if (!IsAuthorized()) return;
                    try
                    {
                        owner.Controls.Add(new LiteralControl(Render(owner, context)));
                    }
                    catch (Exception)
                    {
                        owner.Controls.Add(new LiteralControl("<p>TBYS log tanılaması görüntülenemedi.</p>"));
                    }
                };
            }
            catch (Exception)
            {
                // An unavailable diagnostic panel must not prevent the normal workflow.
            }
        }

        private static bool IsAuthorized()
        {
            try
            {
                return SPContext.Current != null && SPContext.Current.Web != null
                    && SPContext.Current.Web.DoesUserHavePermissions(SPBasePermissions.ManageWeb);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string Render(WebPart owner, PortalLogContext context)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<section style=\"margin:20px 0;padding:16px;border:2px solid #b7791f;background:#fffaf0;color:#222\">");
            html.Append("<h3>TBYS log tanılaması — TBYS-LOG-DIAG-1</h3><dl>");
            Field(html, "Uygulama yolu", () => HttpRuntime.AppDomainAppVirtualPath);
            Field(html, "Proje / WebPart", () => context.Project + " / " + context.WebPart);
            Field(html, "OperationId", () => context.OperationId.ToString());
            Field(html, "İstek türü", () => owner.Page.IsPostBack ? "Postback; yeni açılış kaydı denenmez." : "İlk istek; açılış kaydı denenir.");
            Field(html, "Bu bağlam için loglama", () => PortalLogSettings.IsEnabled(context) ? "Etkin" : "Kapalı");
            Field(html, "Etkin EnabledProjects değeri", () => ConfigurationManager.AppSettings["PortalLogging.EnabledProjects"]);
            Field(html, "Etkin EnabledWebParts değeri", () => ConfigurationManager.AppSettings["PortalLogging.EnabledWebParts"]);
            Field(html, "DAO hedef veritabanı", () => new SqlConnectionStringBuilder(DBProcess.getConnectString()).InitialCatalog);
            Field(html, "Teknik log hedef veritabanı", () =>
            {
                Configuration config = WebConfigurationManager.OpenWebConfiguration("/");
                ConnectionStringSettings connection = config.ConnectionStrings.ConnectionStrings["TSKGV_Connection"];
                return connection == null ? "Bağlantı ayarı yok." : new SqlConnectionStringBuilder(connection.ConnectionString).InitialCatalog;
            });
            Field(html, "WebPart DLL kimliği", () => AssemblyIdentity(owner.GetType().Assembly));
            Field(html, "Model DLL kimliği", () => AssemblyIdentity(typeof(YasalFaizService).Assembly));
            Field(html, "DAO DLL kimliği", () => AssemblyIdentity(typeof(YasalFaizRepository).Assembly));
            Field(html, "Utility DLL kimliği", () => AssemblyIdentity(typeof(PortalLogContext).Assembly));
            html.Append("</dl><h4>Bu istekteki log yazma sonuçları</h4><ul>");
            List<string> entries = HttpContext.Current.Items[PortalLogWriteDiagnostics.EntriesKey] as List<string>;
            if (entries == null || entries.Count == 0)
                html.Append("<li>Tanılama sonucu alınmadı. İstek türü ve yüklü DLL kimlikleri kontrol edilmeli.</li>");
            else
                foreach (string entry in entries)
                    html.Append("<li>").Append(HttpUtility.HtmlEncode(entry)).Append("</li>");
            html.Append("</ul><p>Bu panel yalnızca bu isteği gösterir; sorgu metni, bağlantı parolası ve ham hata metni göstermez.</p></section>");
            return html.ToString();
        }

        private static string AssemblyIdentity(Assembly assembly)
        {
            return assembly.GetName().Name + " / " + assembly.GetName().Version + " / MVID: " + assembly.ManifestModule.ModuleVersionId;
        }

        private static void Field(StringBuilder html, string label, Func<string> read)
        {
            string value;
            try { value = read(); }
            catch (Exception) { value = "Okunamadı."; }
            if (string.IsNullOrEmpty(value)) value = "Boş";
            if (value.Length > 4000) value = value.Substring(0, 4000) + "…";
            html.Append("<dt><b>").Append(HttpUtility.HtmlEncode(label)).Append("</b></dt><dd>")
                .Append(HttpUtility.HtmlEncode(value)).Append("</dd>");
        }
    }
}
