using Portal_WebParts.Logging;
using Utility.Logging;
using Microsoft.SharePoint;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.GunlukYemekListesiMoveWP
{
    [ToolboxItemAttribute(false)]
    public partial class GunlukYemekListesiMoveWP : WebPart, System.Web.UI.ICallbackEventHandler
    {
        private PortalModalActivity modalActivity;
        void System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
        {
            modalActivity.RaiseCallbackEvent(eventArgument);
        }
        string System.Web.UI.ICallbackEventHandler.GetCallbackResult()
        {
            return modalActivity.GetCallbackResult();
        }
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("Portal", "GunlukYemekListesiMoveWP")); } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public GunlukYemekListesiMoveWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            modalActivity = PortalModalActivity.Register(this, LogContext, "#YemekPopupDiv", ViewState);
            this.ChromeType = PartChromeType.None;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                YemekTablosunuDoldur();
            });
        }
        protected void YemekListesiBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "YemekListesiBtn_Click", () =>
            {
                YemekTablosunuDoldur();
            });
        }
        private SortedDictionary<DateTime, SPListItem> MenuleriGetir(DateTime today)
        {
            SPWeb web = SPContext.Current.Web;
            if (web == null) throw new InvalidOperationException("SharePoint web bağlamı bulunamadı.");
            var list = web.Lists["YemekMenusu"];
            var menus = new SortedDictionary<DateTime, SPListItem>();
            menus.Add(today, null);
            string dateValue = today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var query = new SPQuery
            {
                RowLimit = 50,
                Query = "<Where><Geq><FieldRef Name='Tarih' /><Value Type='DateTime' IncludeTimeValue='FALSE'>"
                    + dateValue + "</Value></Geq></Where><OrderBy><FieldRef Name='Tarih' Ascending='TRUE' />"
                    + "<FieldRef Name='ID' Ascending='FALSE' /></OrderBy>"
            };
            // Read complete date groups, including duplicates across page boundaries.
            do
            {
                var items = list.GetItems(query);
                foreach (SPListItem item in items)
                {
                    DateTime date = Convert.ToDateTime(item["Tarih"]).Date;
                    if (date < today) continue;
                    SPListItem existing;
                    if (menus.TryGetValue(date, out existing))
                    {
                        if (existing == null || item.ID > existing.ID) menus[date] = item;
                    }
                    else
                    {
                        if (menus.Count == 6) return menus;
                        menus.Add(date, item);
                    }
                }
                query.ListItemCollectionPosition = items.ListItemCollectionPosition;
            }
            while (query.ListItemCollectionPosition != null);
            return menus;
        }

        private void YemekTablosunuDoldur()
        {
            DateTime today = DateTime.Today;
            var menus = MenuleriGetir(today);
            var culture = CultureInfo.GetCultureInfo("tr-TR");
            ResimleriYukle();
            YokDiv.Visible = false;
            ToplamLbl.Text = string.Empty;
            ToplamLbl.Visible = false;
            TabloDiv.Attributes["style"] = "display:block";
            TabloDiv.Controls.Clear();
            var tabs = new HtmlGenericControl("div");
            tabs.Attributes["class"] = "nav nav-pills flex-nowrap overflow-auto mb-3";
            tabs.Attributes["role"] = "tablist";
            tabs.Attributes["aria-label"] = "Yemek menüsü tarihleri";
            var content = new HtmlGenericControl("div");
            content.Attributes["class"] = "tab-content";
            TabloDiv.Controls.Add(tabs);
            TabloDiv.Controls.Add(content);

            foreach (var menu in menus)
            {
                bool isToday = menu.Key == today;
                string key = ClientID + "_Yemek_" + menu.Key.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                var button = new HtmlGenericControl("button");
                button.Attributes["id"] = key + "_tab";
                button.Attributes["type"] = "button";
                button.Attributes["class"] = "nav-link text-nowrap flex-shrink-0" + (isToday ? " active" : "");
                button.Attributes["role"] = "tab";
                button.Attributes["data-bs-toggle"] = "pill";
                button.Attributes["data-bs-target"] = "#" + key;
                button.Attributes["aria-controls"] = key;
                button.Attributes["aria-selected"] = isToday ? "true" : "false";
                button.InnerText = isToday ? "Bugün" : menu.Key.ToString("dd.MM dddd", culture);
                tabs.Controls.Add(button);
                var pane = new HtmlGenericControl("div");
                pane.Attributes["id"] = key;
                pane.Attributes["class"] = "tab-pane" + (isToday ? " show active" : "");
                pane.Attributes["role"] = "tabpanel";
                pane.Attributes["aria-labelledby"] = key + "_tab";
                content.Controls.Add(pane);
                var table = new Table { CssClass = "table table-striped table-bordered" };
                YemekTableHeaders(table, menu.Key);
                int count = 0;
                double total = 0;
                if (menu.Value != null)
                {
                    for (int i = 1; i <= 8; i++)
                    {
                        string name = Convert.ToString(menu.Value["Yemek" + i]);
                        if (string.IsNullOrWhiteSpace(name)) continue;
                        object value = menu.Value["Yemek" + i + "Kalori"];
                        double calories = value == null ? 0 : Convert.ToDouble(value);
                        var row = new TableRow();
                        row.Cells.Add(new TableCell { Text = (++count).ToString(culture) });
                        row.Cells.Add(new TableCell { Text = System.Web.HttpUtility.HtmlEncode(name) });
                        row.Cells.Add(new TableCell { Text = calories == 0 ? "-" : calories.ToString("0.##", culture) + " kalori" });
                        table.Rows.Add(row);
                        total += calories;
                    }
                }
                if (count == 0)
                {
                    var message = new HtmlGenericControl("div");
                    message.Attributes["class"] = "text-danger fw-bold";
                    message.InnerText = menu.Key.ToString("dd.MM.yyyy", culture) + " tarihi için yemek menüsü tanımlanmamış.";
                    pane.Controls.Add(message);
                }
                else
                {
                    pane.Controls.Add(table);
                    var summary = new HtmlGenericControl("div");
                    summary.Attributes["class"] = "fw-bold";
                    summary.InnerText = "Toplam " + total.ToString("0.##", culture) + " kalori";
                    pane.Controls.Add(summary);
                }
            }
        }

        private void ResimleriYukle()
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string rawUrl = System.Web.HttpContext.Current.Request.RawUrl.ToString();
            int index = currentUrl.IndexOf(rawUrl);
            string rootUrl = (index < 0) ? currentUrl : currentUrl.Remove(index, rawUrl.Length);
            string imgUrl = rootUrl + "/" + ProjeConstants.RESIMLER_ORTAK;
            YemekImg.Src = imgUrl + "/yemek.gif";
            YemekImg1.Src = imgUrl + "/yemek1.gif";
            YemekImg2.Src = imgUrl + "/yemek2.gif";
        }

        private void YemekTableHeaders(Table table, DateTime date)
        {
            table.Rows.Clear();
            TableHeaderRow thbaslik = new TableHeaderRow();
            TableHeaderCell baslikCell = new TableHeaderCell();

            string today = date.ConvertToDatetimeEmptyIfNull();
            baslikCell.Text = " Yemek Listesi " + "(" + today + ")";
            baslikCell.Font.Bold = true;
            baslikCell.ColumnSpan = 3;
            thbaslik.HorizontalAlign = HorizontalAlign.Center;
            thbaslik.Controls.Add(baslikCell);
            table.Rows.Add(thbaslik);

            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell yemekCell = new TableHeaderCell();
            yemekCell.Text = "Yemek Adı";
            TableHeaderCell kaloriCell = new TableHeaderCell();
            kaloriCell.Text = "Kalori";

            th.Controls.Add(siraCell);
            th.Controls.Add(yemekCell);
            th.Controls.Add(kaloriCell);

            table.Rows.Add(th);
        }
    }
}
