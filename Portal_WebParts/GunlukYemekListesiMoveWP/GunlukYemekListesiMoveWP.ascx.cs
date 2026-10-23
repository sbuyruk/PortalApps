using Portal_WebParts.Logging;
using Utility.Logging;
using Microsoft.SharePoint;
using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.GunlukYemekListesiMoveWP
{
    [ToolboxItemAttribute(false)]
    public partial class GunlukYemekListesiMoveWP : WebPart
    {
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
            this.ChromeType = PartChromeType.None;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) PortalWebPartLog.RecordOpened(LogContext);
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
        private DataTable VeriGetir()
        {
            var table = new DataTable();
            table.Columns.Add("YemekAdi", typeof(string));
            table.Columns.Add("Kalori", typeof(string));
            SPWeb web = SPContext.Current.Web;
            if (web == null) throw new InvalidOperationException("SharePoint web bağlamı bulunamadı.");
            var list = web.Lists["YemekMenusu"];
            var query = new SPQuery { RowLimit = 1,
                Query = "<Where><Eq><FieldRef Name='Tarih' /><Value Type='DateTime'><Today /></Value></Eq></Where><OrderBy><FieldRef Name='ID' Ascending='FALSE' /></OrderBy>" };
            var menus = list.GetItems(query);
            if (menus.Count == 0) return table;
            double total = 0;
            var menu = menus[0];
            for (int i = 1; i <= 8; i++)
            {
                string name = Convert.ToString(menu["Yemek" + i]);
                if (string.IsNullOrWhiteSpace(name)) continue;
                object value = menu["Yemek" + i + "Kalori"];
                double calories = value == null ? 0 : Convert.ToDouble(value);
                table.Rows.Add(name, calories == 0 ? "-" : calories.ToString("0.##") + " kalori");
                total += calories;
            }
            if (table.Rows.Count > 0) ToplamLbl.Text = "Toplam " + total.ToString("0.##") + " kalori";
            return table;
        }
        private void YemekTablosunuDoldur()
        {
            YokDiv.Attributes["style"] = "display:none";
            TabloDiv.Attributes["style"] = "display:none";
            ToplamLbl.Text = string.Empty;
            ResimleriYukle();
            YemekTableHeaders();

            DataTable dataTable = VeriGetir();
            int sira = 0;
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                YokDiv.Attributes["style"] = "display : none";
                TabloDiv.Attributes["style"] = "display : block";
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    TableRow row = new TableRow();

                    string yemekAdi = dataRow["YemekAdi"].ToString();
                    string kalori = dataRow["Kalori"].ToString();

                    TableCell siraCell = new TableCell();
                    siraCell.Text = (++sira).ToString();
                    row.Controls.Add(siraCell);

                    TableCell yemekAdiCell = new TableCell();
                    yemekAdiCell.Text = System.Web.HttpUtility.HtmlEncode(yemekAdi);
                    row.Controls.Add(yemekAdiCell);

                    TableCell kaloriCell = new TableCell();
                    kaloriCell.Text = kalori.Equals("0") ? "-":kalori;
                    row.Controls.Add(kaloriCell);
                    YemekTable.Controls.Add(row);

                }
            }
            else
            {
                YokDiv.Attributes["style"] = "display : block";
                TabloDiv.Attributes["style"] = "display : none";
                YokLbl.Text = DateTime.Now.ConvertToDatetimeEmptyIfNull() + " tarihi için yemek menüsü tanımlanmamış.";

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

        private void YemekTableHeaders()
        {
            YemekTable.Rows.Clear();
            TableHeaderRow thbaslik = new TableHeaderRow();
            TableHeaderCell baslikCell = new TableHeaderCell();

            string today = DateTime.Today.ConvertToDatetimeEmptyIfNull();
            baslikCell.Text = " Yemek Listesi " + "(" + today + ")";
            baslikCell.Font.Bold = true;
            baslikCell.ColumnSpan = 6;
            thbaslik.HorizontalAlign = HorizontalAlign.Center;
            thbaslik.Controls.Add(baslikCell);
            YemekTable.Controls.Add(thbaslik);

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

            YemekTable.Controls.Add(th);
        }
    }
}
