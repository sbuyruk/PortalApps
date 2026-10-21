using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Model.Ortak;
using Model.Services.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Portal_WebParts.LogIzlemeWP
{
    [ToolboxItem(false)]
    public class LogIzlemeWP : WebPart
    {
        private const int PageSize = 25;
        private TextBox fromDate, toDate, user, operationId;
        private DropDownList project;
        private Button activityTab, errorTab, previous, next;
        private Literal overview, result;
        private Panel filterPanel, tabPanel, pager;
        private string SelectedKind
        {
            get { return (string)ViewState["LogKind"] ?? "UserActivity"; }
            set { ViewState["LogKind"] = value; }
        }
        private int PageNumber
        {
            get { return ViewState["LogPage"] == null ? 1 : (int)ViewState["LogPage"]; }
            set { ViewState["LogPage"] = Math.Max(1, value); }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ChromeType = PartChromeType.None;
            EnsureChildControls();
        }

        protected override void CreateChildControls()
        {
            Controls.Clear();
            Panel root = new Panel { CssClass = "li-monitor" };
            Controls.Add(root);
            root.Controls.Add(new LiteralControl(Styles));
            root.Controls.Add(new LiteralControl("<header class='li-header'><span class='li-eyebrow'>PORTAL · İZLEME</span><h2>Log İzleme</h2><p>Kullanıcı etkinliklerini ve hataları seçtiğiniz dönem için inceleyin.</p></header>"));
            filterPanel = new Panel { CssClass = "li-filters" };
            root.Controls.Add(filterPanel);
            string today = TurkeyNow().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            fromDate = DateInput("FromDate", today);
            toDate = DateInput("ToDate", today);
            project = new DropDownList { ID = "Project" };
            project.Items.Add(new ListItem("Tüm projeler", ""));
            foreach (string name in new[] { "IKYS", "TBYS", "NBYS", "Portal", "MTS", "MFYS" })
                project.Items.Add(new ListItem(name, name));
            user = new TextBox { ID = "User", MaxLength = 256 };
            operationId = new TextBox { ID = "OperationId", MaxLength = 36 };
            Field(filterPanel, "Başlangıç tarihi", fromDate);
            Field(filterPanel, "Bitiş tarihi", toDate);
            Field(filterPanel, "Proje", project);
            Field(filterPanel, "Kullanıcı ara", user);
            Field(filterPanel, "İşlem kodu ara", operationId);
            Button apply = MakeButton("Apply", "Listele / Yenile", "li-primary");
            apply.Click += delegate { PageNumber = 1; };
            filterPanel.Controls.Add(apply);
            overview = new Literal { ID = "Overview", EnableViewState = false };
            root.Controls.Add(overview);
            tabPanel = new Panel { CssClass = "li-tabs" };
            root.Controls.Add(tabPanel);
            activityTab = MakeButton("Activities", "Kullanıcı etkinlikleri", "li-tab");
            errorTab = MakeButton("Errors", "Hatalar", "li-tab");
            activityTab.Click += delegate { SelectedKind = "UserActivity"; PageNumber = 1; };
            errorTab.Click += delegate { SelectedKind = "Error"; PageNumber = 1; };
            tabPanel.Controls.Add(activityTab);
            tabPanel.Controls.Add(errorTab);
            result = new Literal { ID = "Result", EnableViewState = false };
            root.Controls.Add(result);
            pager = new Panel { CssClass = "li-pager" };
            previous = MakeButton("Previous", "← Önceki", "li-secondary");
            next = MakeButton("Next", "Sonraki →", "li-secondary");
            previous.Click += delegate { PageNumber--; };
            next.Click += delegate { PageNumber++; };
            pager.Controls.Add(previous);
            pager.Controls.Add(next);
            root.Controls.Add(pager);
        }

        protected override void OnPreRender(EventArgs e)
        {
            EnsureChildControls();
            // Recheck permission on every request; never restore log HTML from ViewState.
            bool authorized = SPContext.Current != null && SPContext.Current.Web != null
                && SPContext.Current.Web.DoesUserHavePermissions(SPBasePermissions.ManageWeb);
            filterPanel.Visible = tabPanel.Visible = authorized;
            pager.Visible = false;
            if (!authorized) result.Text = Notice("Bu ekranı görüntüleme yetkiniz bulunmuyor.");
            else LoadResults();
            base.OnPreRender(e);
        }

        private void LoadResults()
        {
            try
            {
                DateTime start, end;
                if (!DateTime.TryParseExact(fromDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out start)
                    || !DateTime.TryParseExact(toDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out end) || end < start || end == DateTime.MaxValue.Date)
                {
                    result.Text = Notice("Geçerli bir tarih aralığı seçiniz. Bitiş tarihi başlangıçtan önce olamaz.");
                    return;
                }
                string signature = start.ToString("yyyyMMdd") + "|" + end.ToString("yyyyMMdd")
                    + "|" + project.SelectedValue + "|" + user.Text.Trim() + "|" + operationId.Text.Trim() + "|" + SelectedKind;
                if (!string.Equals(signature, ViewState["Filter"] as string, StringComparison.Ordinal)) PageNumber = 1;
                ViewState["Filter"] = signature;
                PortalLogFilter filter = new PortalLogFilter
                {
                    FromUtc = TimeZoneInfo.ConvertTimeToUtc(start, TurkeyZone()),
                    ToUtc = TimeZoneInfo.ConvertTimeToUtc(end.AddDays(1), TurkeyZone()),
                    Project = project.SelectedValue, UserLogin = user.Text.Trim(),
                    OperationId = operationId.Text.Trim(), Kind = SelectedKind, PageSize = PageSize
                };
                PortalLogDashboardService service = new PortalLogDashboardService();
                PortalLogMonitorSummary summary = service.GetMonitorSummary(filter);
                long count = SelectedKind == "Error" ? summary.ErrorCount : summary.ActivityCount;
                int pages = (int)Math.Max(1L, Math.Min(int.MaxValue / PageSize, (count + PageSize - 1) / PageSize));
                PageNumber = Math.Min(PageNumber, pages);
                filter.Page = PageNumber;
                List<PortalLogEntry> rows = service.GetLogs(filter);
                activityTab.CssClass = "li-tab" + (SelectedKind == "UserActivity" ? " li-active" : "");
                errorTab.CssClass = "li-tab" + (SelectedKind == "Error" ? " li-active" : "");
                activityTab.Attributes["aria-pressed"] = (SelectedKind == "UserActivity" ? "true" : "false");
                errorTab.Attributes["aria-pressed"] = (SelectedKind == "Error" ? "true" : "false");
                result.Text = BuildResults(summary, rows, start, end, count, pages);
                previous.Enabled = PageNumber > 1;
                next.Enabled = PageNumber < pages;
                pager.Visible = count > PageSize;
            }
            catch (Exception error)
            {
                overview.Text = string.Empty;
                string reference = Guid.NewGuid().ToString("N");
                try
                {
                    SPDiagnosticsService.Local.WriteTrace(0,
                        new SPDiagnosticsCategory("Portal Log İzleme", TraceSeverity.Unexpected, EventSeverity.Error),
                        TraceSeverity.Unexpected, "LogIzlemeWP.LoadResults [{0}]: {1}", new object[] { reference, error });
                }
                catch (Exception) { /* Diagnostics must not hide the user message. */ }
                result.Text = Notice("Log kayıtları yüklenemedi. Lütfen tekrar deneyiniz. Başvuru kodu: " + reference);
            }
        }

        private string BuildResults(PortalLogMonitorSummary summary, List<PortalLogEntry> rows,
            DateTime start, DateTime end, long count, int pages)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<div class='li-scope'>").Append(E(start.ToString("dd.MM.yyyy"))).Append(" – ")
                .Append(E(end.ToString("dd.MM.yyyy"))).Append(" · ").Append(E(project.SelectedItem.Text))
                .Append(" · Türkiye saati · Son yenileme: ").Append(E(TurkeyNow().ToString("HH:mm:ss"))).Append("</div>");
            html.Append("<div class='li-cards'>");
            Card(html, "Webpart açılışı", summary.ActivityCount.ToString("N0"));
            Card(html, "Hata kaydı", summary.ErrorCount.ToString("N0"));
            Card(html, "Aktif kullanıcı", summary.ActiveUserCount.ToString("N0"));
            Card(html, "Ortalama SQL süresi", summary.AverageSqlDurationMs.HasValue
                ? summary.AverageSqlDurationMs.Value.ToString("N0") + " ms" : "Veri yok");
            html.Append("</div><p class='li-hint'>Özetler tarih, proje ve arama filtrelerine göredir. Aktif kullanıcı sayısı webpart açılışlarından hesaplanır.</p>");
            overview.Text = html.ToString();
            html.Clear();
            html.Append("<section class='li-results'><div class='li-caption'><h3>")
                .Append(SelectedKind == "Error" ? "Hata kayıtları" : "Kullanıcı etkinlikleri")
                .Append("</h3><span>").Append(count.ToString("N0")).Append(" kayıt · Sayfa ")
                .Append(PageNumber).Append(" / ").Append(pages).Append("</span></div>");
            if (rows.Count == 0)
                html.Append("<div class='li-empty'><strong>Kayıt bulunamadı</strong><p>Seçilen tarih ve filtrelerde kayıt yok. Tarih aralığını genişletebilir veya arama alanlarını temizleyebilirsiniz.</p></div>");
            else
            {
                html.Append("<div class='li-scroll'><table><caption class='li-sr'>")
                    .Append(SelectedKind == "Error" ? "Filtrelenen hata kayıtları" : "Filtrelenen kullanıcı etkinlikleri")
                    .Append("</caption><thead><tr><th scope='col'>Tarih / saat</th><th scope='col'>Kullanıcı</th><th scope='col'>Proje / Webpart</th><th scope='col'>")
                    .Append(SelectedKind == "Error" ? "Hata" : "Etkinlik").Append("</th><th scope='col'>Ayrıntı</th></tr></thead><tbody>");
                foreach (PortalLogEntry row in rows)
                {
                    html.Append("<tr><td class='li-time'>").Append(E(TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.SpecifyKind(row.OccurredUtc, DateTimeKind.Utc), TurkeyZone()).ToString("dd.MM.yyyy HH:mm:ss")))
                        .Append("</td><td>").Append(E(row.UserLogin)).Append("</td><td><span class='li-project'>")
                        .Append(E(row.Project)).Append("</span><br/>").Append(E(row.WebPart)).Append("</td><td>");
                    if (SelectedKind == "Error")
                        html.Append("<span class='li-error'>Hata</span><p>").Append(E(row.UserMessage ?? "İşlem sırasında hata kaydedildi.")).Append("</p>");
                    else html.Append(E(row.EventName == "WebPartOpened" ? "Webpart açıldı" : row.EventName ?? row.Operation));
                    html.Append("</td><td><details><summary>Ayrıntıyı göster</summary><dl><dt>İşlem kodu</dt><dd class='li-code'>")
                        .Append(E(row.OperationId.ToString())).Append("</dd><dt>Sayfa</dt><dd>").Append(E(row.PagePath)).Append("</dd>");
                    if (SelectedKind == "Error") html.Append("<dt>Hata türü</dt><dd>").Append(E(row.ErrorType))
                        .Append("</dd><dt>Olay</dt><dd>").Append(E(row.EventName)).Append("</dd>");
                    html.Append("</dl></details></td></tr>");
                }
                html.Append("</tbody></table></div>");
            }
            html.Append("</section>");
            return html.ToString();
        }

        private static TextBox DateInput(string id, string value)
        {
            return new TextBox { ID = id, Text = value, TextMode = TextBoxMode.Date };
        }
        private static Button MakeButton(string id, string text, string css)
        {
            return new Button { ID = id, Text = text, CssClass = css, CausesValidation = false };
        }
        private static void Field(Panel parent, string title, WebControl input)
        {
            Panel field = new Panel { CssClass = "li-field" };
            field.Controls.Add(new Label { Text = title, AssociatedControlID = input.ID });
            field.Controls.Add(input);
            parent.Controls.Add(field);
        }
        private static string E(string value) { return HttpUtility.HtmlEncode(value ?? string.Empty); }
        private static string Notice(string message) { return "<div class='li-notice' role='status'>" + E(message) + "</div>"; }
        private static void Card(StringBuilder html, string title, string value)
        {
            html.Append("<div class='li-card'><span>").Append(E(title)).Append("</span><strong>").Append(E(value)).Append("</strong></div>");
        }
        private static TimeZoneInfo TurkeyZone() { return TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"); }
        private static DateTime TurkeyNow() { return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TurkeyZone()); }

        private const string Styles = @"<style>
.li-monitor{font:14px/1.5 'Segoe UI',Arial,sans-serif;color:#203747;background:#f4f7fa;padding:24px;border:1px solid #dde5ed;border-radius:16px}
.li-monitor *{box-sizing:border-box}.li-monitor .li-header{margin-bottom:22px}.li-monitor .li-eyebrow{font-size:11px;font-weight:700;letter-spacing:2px;color:#357b85}.li-monitor h2{font-size:28px;color:#163646;margin:4px 0}.li-monitor p{margin:6px 0;color:#536879}
.li-monitor .li-filters{display:flex;flex-wrap:wrap;gap:12px;align-items:flex-end;background:#fff;border:1px solid #dde5ed;border-radius:12px;padding:16px}.li-monitor .li-field{flex:1 1 140px;min-width:0}.li-monitor label{display:block;font-size:12px;font-weight:600;margin-bottom:5px}.li-monitor input[type=text],.li-monitor input[type=date],.li-monitor select{width:100%;height:38px;border:1px solid #aabcc9;border-radius:6px;padding:6px;background:#fff;color:#203747}
.li-monitor input[type=submit]{cursor:pointer;border-radius:7px;padding:9px 14px;font:600 13px 'Segoe UI',Arial,sans-serif;min-height:38px;margin:0}.li-monitor .li-primary{background:#176b78;color:#fff;border:1px solid #176b78}.li-monitor .li-tabs{display:flex;gap:8px;margin:22px 0 12px;border-bottom:1px solid #d5e0e9;padding-bottom:10px}.li-monitor .li-tab{background:transparent;color:#486171;border:1px solid transparent}.li-monitor .li-tab.li-active{background:#e0f0f1;color:#145b66;border-color:#a5cdd1}.li-monitor :focus-visible{outline:3px solid #2d7db8;outline-offset:3px}
.li-monitor .li-scope{font-size:12px;color:#536879;margin:12px 0}.li-monitor .li-cards{display:grid;grid-template-columns:repeat(4,minmax(0,1fr));gap:12px}.li-monitor .li-card{background:#fff;border:1px solid #dde5ed;border-radius:10px;padding:15px}.li-monitor .li-card span{font-size:12px;color:#536879}.li-monitor .li-card strong{display:block;font-size:25px;margin-top:6px;color:#173e4c}.li-monitor .li-hint{font-size:12px;margin:12px 0 18px}
.li-monitor .li-results{background:#fff;border:1px solid #dde5ed;border-radius:12px;overflow:hidden}.li-monitor .li-caption{display:flex;flex-wrap:wrap;align-items:center;justify-content:space-between;gap:10px;padding:16px}.li-monitor h3{font-size:17px;margin:0;color:#203747}.li-monitor .li-caption span{font-size:12px;color:#536879}.li-monitor .li-scroll{overflow-x:auto}.li-monitor table{border-collapse:collapse;width:100%;min-width:760px}.li-monitor th{background:#edf3f7;padding:11px 14px;text-align:left;font-size:12px;color:#455f70}.li-monitor td{padding:14px;border-top:1px solid #e6edf2;vertical-align:top;font-size:13px;overflow-wrap:anywhere}.li-monitor tbody tr:hover{background:#f8fbfc}.li-monitor .li-time{white-space:nowrap}.li-monitor .li-project{font-size:11px;color:#176b78;font-weight:700}.li-monitor .li-error{display:inline-block;background:#fff0ed;color:#a33329;padding:2px 8px;border-radius:12px;font-size:12px;font-weight:600}
.li-monitor summary{cursor:pointer;color:#176b78;white-space:nowrap}.li-monitor dt{font-size:11px;font-weight:600;margin-top:8px}.li-monitor dd{margin:3px 0;color:#536879;max-width:300px;overflow-wrap:anywhere}.li-monitor .li-code{font-family:Consolas,monospace;font-size:12px}.li-monitor .li-empty{padding:35px 20px;text-align:center}.li-monitor .li-notice{margin-top:16px;padding:18px;border:1px solid #e6cb8e;background:#fff9ed;border-radius:10px}.li-monitor .li-pager{display:flex;justify-content:flex-end;gap:10px;margin-top:14px}.li-monitor .li-secondary{border:1px solid #aabcc9;background:#fff;color:#244d61}.li-monitor input[disabled]{opacity:.45;cursor:default}.li-monitor .li-sr{position:absolute;width:1px;height:1px;overflow:hidden;clip:rect(0,0,0,0)}
@media(max-width:800px){.li-monitor{padding:14px}.li-monitor .li-cards{grid-template-columns:repeat(2,minmax(0,1fr))}.li-monitor .li-filters{padding:12px}}
</style>";
    }
}
