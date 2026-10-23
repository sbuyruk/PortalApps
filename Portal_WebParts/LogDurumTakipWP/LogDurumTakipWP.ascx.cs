using Portal_WebParts.Logging;
using Utility.Logging;
using Microsoft.SharePoint;
using Model.Ortak;
using Model.Services.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Portal_WebParts.LogDurumTakipWP
{
    [ToolboxItemAttribute(false)]
    public partial class LogDurumTakipWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("Portal", "LogDurumTakipWP")); } }
        public LogDurumTakipWP() { }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            ChromeType = PartChromeType.None;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack) RenderDashboard();
        }

        private void RenderDashboard()
        {
            try
            {
                if (!IsAuthorized()) { SetDashboardText(Message("Bu ekranı görüntüleme yetkiniz bulunmuyor.")); return; }
                PortalWebPartLog.RecordOpened(LogContext);
                DateTime localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TurkeyTimeZone());
                DateTime localStart = localNow.Date;
                DateTime fromUtc = TimeZoneInfo.ConvertTimeToUtc(localStart, TurkeyTimeZone());
                DateTime toUtc = TimeZoneInfo.ConvertTimeToUtc(localStart.AddDays(1), TurkeyTimeZone());
                PortalLogDashboardService service = new PortalLogDashboardService();
                List<PortalLogEntry> logs = service.GetLogs(new PortalLogFilter { FromUtc = fromUtc, ToUtc = toUtc, Page = 1, PageSize = 200 });
                SetDashboardText(BuildDashboard(service, logs, service.GetSummary(fromUtc, toUtc)));
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                PortalTechnicalLog.WriteError(LogContext, "Portal.LogDurumTakip.Load", error);
                SetDashboardText(Message("Log kayıtları yüklenemedi. Lütfen yeniden deneyin. Başvuru kodu: " + LogContext.OperationId.ToString("N")));
            }
        }

        private void SetDashboardText(string text)
        {
            Literal dashboardLiteral = FindControl("DashboardLiteral") as Literal;
            if (dashboardLiteral != null) dashboardLiteral.Text = text;
        }

        private static bool IsAuthorized()
        {
            return SPContext.Current != null && SPContext.Current.Web != null
                && SPContext.Current.Web.DoesUserHavePermissions(SPBasePermissions.ManageWeb);
        }

        private static TimeZoneInfo TurkeyTimeZone()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"); }
            catch (TimeZoneNotFoundException) { return TimeZoneInfo.Utc; }
            catch (InvalidTimeZoneException) { return TimeZoneInfo.Utc; }
        }

        private static string BuildDashboard(PortalLogDashboardService service, List<PortalLogEntry> logs, PortalLogSummary summary)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<div class=\"pl-cards\">");
            Card(html, "Toplam işlem", summary.TotalActivityCount.ToString(CultureInfo.InvariantCulture));
            Card(html, "Başarılı işlem", summary.SuccessfulActivityCount.ToString(CultureInfo.InvariantCulture));
            Card(html, "Hatalı işlem", summary.FailedActivityCount.ToString(CultureInfo.InvariantCulture));
            Card(html, "İşlem yapan kullanıcı", summary.ActiveUserCount.ToString(CultureInfo.InvariantCulture));
            Card(html, "Ortalama SQL süresi", summary.AverageSqlDurationMs.HasValue ? Math.Round(summary.AverageSqlDurationMs.Value) + " ms" : "Veri yok");
            html.Append("</div><div class=\"pl-filter\"><input data-pl-filter=\"user\" placeholder=\"Kullanıcı\" /><input data-pl-filter=\"operationid\" placeholder=\"OperationId\" /><select data-pl-filter=\"outcome\"><option value=\"\">Tüm durumlar</option><option value=\"Succeeded\">Başarılı</option><option value=\"Failed\">Hata</option><option value=\"Warning\">Uyarı</option></select></div>");
            AppendActivityTable(html, logs.Where(x => x.Kind == "UserActivity").ToList());
            AppendErrorTable(html, logs.Where(x => x.Kind == "Error").ToList());
            AppendAuditTable(html, service, logs.Where(x => x.Kind == "Audit").ToList());
            AppendSqlTable(html, logs.Where(x => x.Kind == "SqlTrace").ToList());
            html.Append(@"<script>(function(){var root=document.currentScript.parentElement;var inputs=root.querySelectorAll('[data-pl-filter]');function apply(){var rows=root.querySelectorAll('tr[data-pl-row]');for(var i=0;i<rows.length;i++){var ok=true;for(var j=0;j<inputs.length;j++){var input=inputs[j],value=(input.value||'').toLowerCase(),field=input.getAttribute('data-pl-filter');if(value&&((rows[i].getAttribute('data-'+field)||'').toLowerCase().indexOf(value)<0)){ok=false;break;}}rows[i].style.display=ok?'':'none';}}for(var i=0;i<inputs.length;i++){inputs[i].addEventListener('input',apply);inputs[i].addEventListener('change',apply);}})();</script>");
            return html.ToString();
        }

        private static void Card(StringBuilder html, string label, string value) { html.Append("<div class=\"pl-card\"><small>").Append(Encode(label)).Append("</small><strong>").Append(Encode(value)).Append("</strong></div>"); }

        private static void AppendActivityTable(StringBuilder html, List<PortalLogEntry> rows)
        {
            html.Append("<section class=\"pl-section\"><h3>Son kullanıcı etkinlikleri</h3><table class=\"pl-table\"><thead><tr><th>Tarih-saat</th><th>Kullanıcı</th><th>Proje / WebPart</th><th>İşlem</th><th>Durum</th><th>OperationId</th></tr></thead><tbody>");
            if (rows.Count == 0) html.Append(Empty(6));
            foreach (PortalLogEntry row in rows) AppendRow(html, row, row.EventName ?? row.Operation);
            html.Append("</tbody></table></section>");
        }

        private static void AppendErrorTable(StringBuilder html, List<PortalLogEntry> rows)
        {
            html.Append("<section class=\"pl-section\"><h3>Hata kayıtları</h3><table class=\"pl-table\"><thead><tr><th>Tarih-saat</th><th>WebPart</th><th>İşlem</th><th>Hata tipi</th><th>Kullanıcı mesajı</th><th>OperationId</th><th>Detay</th></tr></thead><tbody>");
            if (rows.Count == 0) html.Append(Empty(7));
            foreach (PortalLogEntry row in rows)
            {
                html.Append("<tr data-pl-row data-user=\"").Append(Encode(row.UserLogin)).Append("\" data-operationid=\"").Append(Encode(row.OperationId.ToString())).Append("\" data-outcome=\"").Append(Encode(row.Outcome)).Append("\"><td>").Append(Date(row.OccurredUtc)).Append("</td><td>").Append(Encode(row.WebPart)).Append("</td><td>").Append(Encode(row.EventName)).Append("</td><td>").Append(Encode(row.ErrorType)).Append("</td><td>").Append(Encode(row.UserMessage ?? "Kullanıcı mesajı kaydedilmemiş.")).Append("</td><td class=\"pl-code\">").Append(Encode(row.OperationId.ToString())).Append("</td><td><details><summary>Aç</summary><b>Teknik özet:</b> ").Append(Encode(row.TechnicalSummary)).Append("<div class=\"pl-code\">").Append(Encode(row.StackTrace)).Append("\n").Append(Encode(row.InnerException)).Append("</div></details></td></tr>");
            }
            html.Append("</tbody></table></section>");
        }

        private static void AppendAuditTable(StringBuilder html, PortalLogDashboardService service, List<PortalLogEntry> rows)
        {
            html.Append("<section class=\"pl-section\"><h3>Audit kayıtları</h3><table class=\"pl-table\"><thead><tr><th>Tarih-saat</th><th>Entity</th><th>İşlem</th><th>Değişen alan</th><th>OperationId</th></tr></thead><tbody>");
            if (rows.Count == 0) html.Append(Empty(5));
            foreach (PortalLogEntry row in rows)
            {
                int changes = service.GetChanges(row.Id).Count;
                html.Append("<tr data-pl-row data-user=\"").Append(Encode(row.UserLogin)).Append("\" data-operationid=\"").Append(Encode(row.OperationId.ToString())).Append("\" data-outcome=\"").Append(Encode(row.Outcome)).Append("\"><td>").Append(Date(row.OccurredUtc)).Append("</td><td>").Append(Encode(row.EntityName)).Append(" #").Append(Encode(row.EntityId)).Append("</td><td>").Append(Encode(row.Operation)).Append("</td><td>").Append(changes.ToString(CultureInfo.InvariantCulture)).Append("</td><td class=\"pl-code\">").Append(Encode(row.OperationId.ToString())).Append("</td></tr>");
            }
            html.Append("</tbody></table></section>");
        }

        private static void AppendSqlTable(StringBuilder html, List<PortalLogEntry> rows)
        {
            html.Append("<section class=\"pl-section\"><h3>SQL izleri</h3><table class=\"pl-table\"><thead><tr><th>Tarih-saat</th><th>WebPart</th><th>SQL işlemi</th><th>Repository / metot</th><th>Süre</th><th>Başarılı mı?</th></tr></thead><tbody>");
            if (rows.Count == 0) html.Append(Empty(6));
            foreach (PortalLogEntry row in rows)
            {
                html.Append("<tr data-pl-row data-user=\"").Append(Encode(row.UserLogin))
                    .Append("\" data-operationid=\"").Append(Encode(row.OperationId.ToString()))
                    .Append("\" data-outcome=\"").Append(Encode(row.Outcome)).Append("\"><td>")
                    .Append(Date(row.OccurredUtc)).Append("</td><td>").Append(Encode(row.WebPart))
                    .Append("</td><td>").Append(Encode(row.SqlOperation)).Append("</td><td>")
                    .Append(Encode(row.RepositoryMethod)).Append("</td><td>")
                    .Append(row.DurationMs.HasValue ? row.DurationMs.Value.ToString(CultureInfo.InvariantCulture) + " ms" : "Veri yok")
                    .Append("</td><td>").Append(row.Outcome == "Succeeded" ? "Başarılı" : row.Outcome == "Failed" ? "Başarısız" : "Bilinmiyor")
                    .Append("</td></tr>");
            }
            html.Append("</tbody></table></section>");
        }

        private static void AppendRow(StringBuilder html, PortalLogEntry row, string operation)
        {
            string badge = row.Outcome == "Failed" ? "pl-err" : row.Outcome == "Warning" ? "pl-warn" : "pl-ok";
            html.Append("<tr data-pl-row data-user=\"").Append(Encode(row.UserLogin)).Append("\" data-operationid=\"").Append(Encode(row.OperationId.ToString())).Append("\" data-outcome=\"").Append(Encode(row.Outcome)).Append("\"><td>").Append(Date(row.OccurredUtc)).Append("</td><td>").Append(Encode(row.UserLogin)).Append("</td><td>").Append(Encode(row.Project)).Append(" / ").Append(Encode(row.WebPart)).Append("</td><td>").Append(Encode(operation)).Append("</td><td><span class=\"pl-badge ").Append(badge).Append("\">").Append(Encode(row.Outcome)).Append("</span></td><td class=\"pl-code\">").Append(Encode(row.OperationId.ToString())).Append("</td></tr>");
        }

        private static string Empty(int columns) { return "<tr><td colspan=\"" + columns.ToString(CultureInfo.InvariantCulture) + "\" class=\"pl-empty\">Kayıt bulunamadı.</td></tr>"; }
        private static string Date(DateTime utc)
        {
            DateTime turkeyTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), TurkeyTimeZone());
            return Encode(turkeyTime.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture));
        }
        private static string Message(string text) { return "<div class=\"pl-section\"><div class=\"pl-empty\">" + Encode(text) + "</div></div>"; }
        private static string Encode(string value) { return HttpUtility.HtmlEncode(value ?? string.Empty); }
    }
}
