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

namespace MFYS_WebParts.BankaBazindaGunlukBagisListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BankaBazindaGunlukBagisListesiWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("MFYS", "BankaBazindaGunlukBagisListesiWP")); } }
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
        public BankaBazindaGunlukBagisListesiWP()
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
            Run("Page_Load", () =>
            {
                if (!Page.IsPostBack)
                {
                    MfysWebPartLog.RecordOpened(LogContext);
                    BankaDDLDoldur();
                    AyDDLDoldur();
                    YilDDLDoldur();
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
                new[] { "BagisTarihi", "ToplamBagis" }, new[] { 1 },
                "Banka Bazında Günlük Bağışlar (TL)", "Banka-Gunluk-TL-Bagislari", 31);
        }
        private List<BagisListItem> GetDataList()
        {
            DateTime from, until;
            MfysWebPartLog.ReadPeriod(YilDDL, AyDDL, out from, out until);
            string bank = MfysWebPartLog.RequireSelection(BankaDDL, "Bir banka seçin.", allowEmpty: true);
            DataTable table = Reports.DailyTlTotals(from, until, bank);
            var totals = new Dictionary<DateTime, decimal>();
            if (table != null) foreach (DataRow row in table.Rows)
                totals.Add(Convert.ToDateTime(row["BagisTarihi"]).Date, row["ToplamBagis"].ReturnZeroIfNull().ConvertToDecimal());
            var list = new List<BagisListItem>();
            decimal grandTotal = 0;
            var culture = CultureInfo.GetCultureInfo("tr-TR");
            for (DateTime day = from; day < until; day = day.AddDays(1))
            {
                decimal amount;
                totals.TryGetValue(day, out amount);
                list.Add(new BagisListItem { BagisTarihi = day.ToString("dd.MM.yyyy", culture),
                    ToplamBagis = amount.ToString("N", culture) + " TL", Banka = bank });
                grandTotal += amount;
            }
            ToplamLbl.Text = "Toplam: " + grandTotal.ToString("N", culture) + " TL";
            return list;
        }
        private void BankaDDLDoldur()
        {
            BankaDDL.Items.Clear();
            foreach (string bank in Reports.BankGroups(true)) BankaDDL.Items.Add(new ListItem(bank, bank));
            if (BankaDDL.Items.Count == 0) BankaDDL.Items.Add(new ListItem("Tüm Bankalar", string.Empty));
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
            // Filters are applied together during OnPreRender.
        }
        protected void BankaDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Filters are applied together during OnPreRender.
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Filters are applied together during OnPreRender.
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Filters are applied together during OnPreRender.
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
            public string ToplamBagis { get; set; }
            public string Banka { get; set; }
        }
    }
}
