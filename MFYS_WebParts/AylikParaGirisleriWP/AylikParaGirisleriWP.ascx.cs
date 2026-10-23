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

namespace MFYS_WebParts.AylikParaGirisleriWP
{
    [ToolboxItemAttribute(false)]
    public partial class AylikParaGirisleriWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("MFYS", "AylikParaGirisleriWP")); } }
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
        public AylikParaGirisleriWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private readonly IFormatProvider cultureInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            Run("Page_Load", () =>
            {
                if (!Page.IsPostBack)
                {
                    MfysWebPartLog.RecordOpened(LogContext);
                    IslemDDLDoldur();
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
                new[] { "IslemTarihi", "Islem", "IslemYapan", "IslemTutari", "Aciklama" }, new[] { 3 },
                "Aylık Para Girişleri", "Aylik-Para-Girisleri", 10);
        }
        private List<BankaIslemListItem> GetDataList()
        {
            DateTime from, until;
            MfysWebPartLog.ReadPeriod(YilDDL, AyDDL, out from, out until);
            int reason;
            if (!int.TryParse(MfysWebPartLog.RequireSelection(IslemDDL, "Bir işlem türü seçin."), out reason))
                throw new MfysInputException("Geçerli bir işlem türü seçin.");
            var list = new List<BankaIslemListItem>();
            decimal total = 0;
            DataTable table = Reports.Payments(from, until, reason);
            if (table != null) foreach (DataRow row in table.Rows)
            {
                string nameB = Convert.ToString(row["AdiB"]), reasonB = Convert.ToString(row["OdemeSebebiB"]);
                decimal amount = (row.IsNull("TutarB") ? row["TutarA"] : row["TutarB"]).ReturnZeroIfNull().ConvertToDecimal();
                list.Add(new BankaIslemListItem {
                    IslemTarihi = Convert.ToDateTime(row["OdemeTarihi"]).ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("tr-TR")),
                    Islem = string.IsNullOrEmpty(reasonB) ? Convert.ToString(row["OdemeSebebiA"]) : reasonB,
                    IslemYapan = string.IsNullOrEmpty(nameB) ? Convert.ToString(row["AdiA"]) : nameB,
                    IslemTutari = amount.ToString("N", cultureInfo) + " TL",
                    Aciklama = Convert.ToString(row["AciklamaA"]) + " # " + Convert.ToString(row["AciklamaB"]) });
                total += amount;
            }
            ToplamLbl.Text = "Toplam: " + total.ToString("N", cultureInfo) + " TL";
            return list;
        }
        private void IslemDDLDoldur()
        {
            IslemDDL.Items.Clear();
            IslemDDL.Items.Add(new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI_INT.ToString()));
            DataTable table = Reports.PaymentReasons();
            if (table != null) foreach (DataRow row in table.Rows)
                IslemDDL.Items.Add(new ListItem(Convert.ToString(row["OdemeSebebi"]), Convert.ToString(row["Id"])));
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
        protected void IslemDDL_SelectedIndexChanged(object sender, EventArgs e)
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
        private class BankaIslemListItem
        {
            public string IslemTarihi { get; set; }
            public string Islem { get; set; }
            public string IslemYapan { get; set; }
            public string IslemTutari { get; set; }
            public string B_A { get; set; }
            public string Aciklama { get; set; }
        }
    }
}
