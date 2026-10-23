using MFYS_WebParts.Logging;
using Model.Services.MFYS;
using Model.Ortak;
using Utility.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace MFYS_WebParts.TarihBazindaGunlukBagisListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TarihBazindaGunlukBagisListesiWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("MFYS", "TarihBazindaGunlukBagisListesiWP")); } }
        private MfysReportService Reports { get { return new MfysReportService(LogContext); } }
        private void Run(string operation, Action action) { MfysWebPartLog.ExecuteAction(LogContext, operation, action, ClearReport); }
        private void ClearReport()
        {
            ToplamLbl.Text = string.Empty;
            UtilityHelper.ScriptCalistir(@"(function() {
                var table = jQuery('#CustomDataTable');
                table.closest('.dataTables_wrapper,.dt-container').hide();
                table.hide();
                if (jQuery.fn.DataTable && jQuery.fn.DataTable.isDataTable('#CustomDataTable')) {
                    var api = table.DataTable();
                    api.clear().draw();
                    if (typeof api.buttons === 'function') api.buttons().disable();
                }
            })();");
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Run("RenderReport", () =>
            {
                TabloOlustur();
            });
        }

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TarihBazindaGunlukBagisListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            Run("Page_Load", () =>
            {
                if (!Page.IsPostBack)
                {
                    MfysWebPartLog.RecordOpened(LogContext);
                    BagisTarihiTxt.Text = DateTime.Today.AddDays(-1).ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("tr-TR"));
                    BankaDDLDoldur();
                    DovizCinsiDDLDoldur();
                }
            });
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson();
            UtilityHelper.ScriptCalistir(CreateDataTable(jsonData));
        }
        private string TabloJson()
        {
            return MfysWebPartLog.Json(GetDataList());
        }
        private string CreateDataTable(string jsonData)
        {
            return MfysWebPartLog.DataTableScript(jsonData,
                new[] { "HesapKodu", "HesapAdi", "FaturaNo", "Aciklama", "ParaBirimi", "ToplamBagis", "Alacak", "DovizliTutar" }, new[] { 5, 7 },
                "Tarih Bazında Günlük Bağışlar", "Tarih-Gunluk-Bagislari", 50);
        }
        private List<BagisListItem> GetDataList()
        {
            DateTime day = MfysWebPartLog.ReadDay(BagisTarihiTxt.Text);
            string bank = MfysWebPartLog.RequireSelection(BankaDDL, "Bir banka seçin.", allowEmpty: true);
            string currency = MfysWebPartLog.RequireSelection(DovizCinsiDDL, "Bir döviz cinsi seçin.");
            DataTable table = Reports.DailyDonations(day, bank, currency);
            var list = new List<BagisListItem>();
            decimal totalTl = 0, totalCurrency = 0;
            if (table != null) foreach (DataRow row in table.Rows)
            {
                decimal tl = row["ToplamBagis"].ReturnZeroIfNull().ConvertToDecimal();
                decimal amount = currency == ProjeConstants.DOVIZ_TL ? tl : row["DovizTutari"].ReturnZeroIfNull().ConvertToDecimal();
                list.Add(new BagisListItem { HesapKodu = Convert.ToString(row["HesapKodu"]), HesapAdi = Convert.ToString(row["HesapAdi"]),
                    ToplamBagis = tl.ToString("N", culturInfo), ParaBirimi = currency, DovizliTutar = currency + " " + amount.ToString("N", culturInfo) });
                totalTl += tl; totalCurrency += amount;
            }
            ToplamLbl.Text = "Toplam: " + totalTl.ToString("N", culturInfo) + " TL"
                + (currency == ProjeConstants.DOVIZ_TL ? string.Empty : " (" + totalCurrency.ToString("N", culturInfo) + " " + currency + ")");
            return list;
        }
        private void BankaDDLDoldur()
        {

            if (BankaDDL.SelectedItem == null)
            {
                BankaDDL.Items.Clear();
                BankaDDL.Items.Add(new ListItem("Tüm Bankalar", string.Empty));
                List<string> list = Reports.BankGroups(false);
                foreach (string bankaGrup in list)
                {
                    BankaDDL.Items.Add(new ListItem(bankaGrup, bankaGrup));
                }
            }
        }
        private void DovizCinsiDDLDoldur()
        {
            if (DovizCinsiDDL.SelectedItem == null)
            {
                DovizCinsiDDL.Items.Clear();
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_TL, ProjeConstants.DOVIZ_TL));
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_EURO, ProjeConstants.DOVIZ_EURO));
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_USD, ProjeConstants.DOVIZ_USD));
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_GBP, ProjeConstants.DOVIZ_GBP));
            }

        }
        protected void BagisTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            // Filters are applied together during OnPreRender.
        }
        protected void BankaDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Filters are applied together during OnPreRender.
        }
        protected void DovizCinsiDDLIli_SelectedIndexChanged(object sender, EventArgs e)
        {
            Run("DovizCinsiDDLIli_SelectedIndexChanged", () =>
            {
                BankaDDLDoldur();
            });
        }
        protected void YenileBtn_Click(object sender, EventArgs e)
        {
            // Filters are applied together during OnPreRender.
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            Run("CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            });
        }
        private class BagisListItem
        {
            public string HesapKodu { get; set; }
            public string HesapAdi { get; set; }
            public string FaturaNo { get; set; } = "";
            public string Aciklama { get; set; } = "";
            public string ParaBirimi { get; set; }
            public string ToplamBagis { get; set; }
            public string Alacak { get; set; } = "";
            public string DovizliTutar { get; set; }
        }
    }
}
