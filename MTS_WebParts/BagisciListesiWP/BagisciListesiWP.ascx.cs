using MTS_WebParts.Logging;
using Utility.Logging;
using Model.MTS;
using Model.Services.NBYS;
using Model.Services.TBYS;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace MTS_WebParts.BagisciListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BagisciListesiWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("MTS", "BagisciListesiWP")); } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BagisciListesiWP()
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
            MtsWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) MtsWebPartLog.RecordOpened(LogContext);
                TabloOlustur();
            });
        }
        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl);

        }

        private void TabloOlustur()
        {
            List<Kisi> list = new List<Kisi>();
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            //var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "setDataSet(" + jsonData + ");", true);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            List<BagisciListItem> list = GetDataList();
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            jSon = serializer.Serialize(list);

            return jSon.Replace("<", "\\u003c").Replace(">", "\\u003e").Replace("&", "\\u0026");
        }
        private List<BagisciListItem> GetDataList()
        {
            NakitBagisciReportService reportService = new NakitBagisciReportService();
            DataTable dataTableNakit = reportService.GetSecilmemisFaaliyetKatilimcilari(LogContext);
            DataTable dataTableTasinmaz = new TasinmazBagisciReportService().GetUnselectedParticipants(LogContext);

            if (dataTableTasinmaz == null) dataTableTasinmaz = dataTableNakit;
            else if (dataTableNakit != null) dataTableTasinmaz.Merge(dataTableNakit);
            int SiraNo = 1;
            List<BagisciListItem> list = new List<BagisciListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            if (dataTableTasinmaz != null)
            {
                foreach (DataRow row in dataTableTasinmaz.Rows)
                {
                    string katilimciId = row["KatilimciId"].ToString();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();
                    string telefon = row["Telefon"].ToString();
                    string adres = row["Adres"].ToString();
                    string il = row["Il"].ToString();
                    string ilce = row["Ilce"].ToString();

                    BagisciListItem katilimciItem = new BagisciListItem();
                    katilimciItem.Sirano = SiraNo++.ToString();
                    katilimciItem.KatilimciId = katilimciId;
                    katilimciItem.AdiSoyadi = (adi + " " + soyadi.Trim());
                    katilimciItem.Adi = adi;
                    katilimciItem.Soyadi = soyadi;
                    katilimciItem.Telefon = telefon;
                    katilimciItem.Adres = adres;
                    katilimciItem.Il = il;
                    katilimciItem.Ilce = ilce;


                    list.Add(katilimciItem);
                }
            }
            return list;
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            });
        }
        protected void RandevuTakvimiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "RandevuTakvimiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM);
            });
        }
        private class BagisciListItem
        {
            public string Sirano { get; set; }
            public string KatilimciId { get; set; }
            public string AdiSoyadi { get; set; }
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public string Telefon { get; set; }
            public string Adres { get; set; }
            public string Il { get; set; }
            public string Ilce { get; set; }

        }
    }
}
