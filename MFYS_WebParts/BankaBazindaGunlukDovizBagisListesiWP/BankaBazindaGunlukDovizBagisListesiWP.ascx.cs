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

namespace MFYS_WebParts.BankaBazindaGunlukDovizBagisListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BankaBazindaGunlukDovizBagisListesiWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("MFYS", "BankaBazindaGunlukDovizBagisListesiWP")); } }
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
                if (refreshBanks) BankaDDLDoldur();
                TabloOlustur();
            });
        }
        private bool refreshBanks;

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BankaBazindaGunlukDovizBagisListesiWP()
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
                    AyDDLDoldur();
                    YilDDLDoldur();
                    DovizCinsiDDLDoldur();
                    refreshBanks = true;
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
                new[] { "BagisTarihi", "DovizTutari", "DovizKuru", "TLKarsiligi" }, new[] { 1, 2, 3 },
                "Banka Bazında Günlük Döviz Bağışları", "Banka-Gunluk-Doviz-Bagislari", 50);
        }
        private List<BagisListItem> GetDataList()
        {
            DateTime from, until;
            MfysWebPartLog.ReadPeriod(YilDDL, AyDDL, out from, out until);
            string bank = MfysWebPartLog.RequireSelection(BankaDDL, "Bir banka seçin.", allowEmpty: true);
            string currency = MfysWebPartLog.RequireSelection(DovizCinsiDDL, "Bir döviz cinsi seçin.");
            var list = new List<BagisListItem>();
            decimal totalTl = 0, totalCurrency = 0;
            DataTable table = Reports.CurrencyDonations(from, until, bank, currency);
            if (table != null) foreach (DataRow row in table.Rows)
            {
                decimal amount = row["DovizTutari"].ReturnZeroIfNull().ConvertToDecimal();
                decimal rate = row["DovizKuru"].ReturnZeroIfNull().ConvertToDecimal();
                decimal tl = row["BagisMiktari"].ReturnZeroIfNull().ConvertToDecimal();
                list.Add(new BagisListItem { BagisTarihi = Convert.ToDateTime(row["BagisTarihi"]).ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("tr-TR")),
                    DovizTutari = amount.ToString("N", culturInfo) + " " + currency,
                    DovizKuru = rate.ToString("N4", culturInfo) + " TL", TLKarsiligi = tl.ToString("N", culturInfo) + " TL", Banka = Convert.ToString(row["Banka"]) });
                totalTl += tl; totalCurrency += amount;
            }
            ToplamLbl.Text = "Toplam: " + totalCurrency.ToString("N", culturInfo) + " " + currency + " (" + totalTl.ToString("N", culturInfo) + " TL)";
            return list;
        }
        private void BankaDDLDoldur()
        {
            string selected = BankaDDL.SelectedValue;
            DateTime from, until;
            MfysWebPartLog.ReadPeriod(YilDDL, AyDDL, out from, out until);
            string currency = MfysWebPartLog.RequireSelection(DovizCinsiDDL, "Bir döviz cinsi seçin.");
            List<string> banks = Reports.CurrencyBankGroups(from, until, currency);
            BankaDDL.Items.Clear();
            foreach (string bank in banks) BankaDDL.Items.Add(new ListItem(bank, bank));
            if (BankaDDL.Items.Count == 0) BankaDDL.Items.Add(new ListItem("Tüm Bankalar", string.Empty));
            ListItem previous = BankaDDL.Items.FindByValue(selected);
            if (previous != null) { BankaDDL.ClearSelection(); previous.Selected = true; }
            refreshBanks = false;
        }
        private void DovizCinsiDDLDoldur()
        {
            if (DovizCinsiDDL.SelectedItem == null)
            {
                DovizCinsiDDL.Items.Clear();
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_EURO, ProjeConstants.DOVIZ_EURO));
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_USD, ProjeConstants.DOVIZ_USD));
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_GBP, ProjeConstants.DOVIZ_GBP));
            }

        }
        private void AyDDLDoldur()
        {
            MfysWebPartLog.FillMonths(AyDDL);
        }
        private void YilDDLDoldur()
        {
            MfysWebPartLog.FillYears(YilDDL);
        }
        protected void BagisTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            Run("BagisTarihiTxt_TextChanged", () =>
            {
                refreshBanks = true;
            });
        }
        protected void DovizCinsiDDLIli_SelectedIndexChanged(object sender, EventArgs e)
        {
            Run("DovizCinsiDDLIli_SelectedIndexChanged", () =>
            {
                refreshBanks = true;
            });
        }
        protected void BankaDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Filters are applied together during OnPreRender.
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            Run("AyDDL_SelectedIndexChanged", () =>
            {
                refreshBanks = true;
            });
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            Run("YilDDL_SelectedIndexChanged", () =>
            {
                refreshBanks = true;
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
            public string Sirano { get; set; }
            public string BagisTarihi { get; set; }
            public string DovizTutari { get; set; }
            public string DovizKuru { get; set; }
            public string KurTarihi { get; set; }
            public string TLKarsiligi { get; set; }
            public string Banka { get; set; }
        }
    }
}
