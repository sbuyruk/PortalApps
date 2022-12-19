using Model.NBYS;
using Model.Ortak;
using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.NakitBagisciByTarihWP
{
    [ToolboxItemAttribute(false)]
    public partial class NakitBagisciByTarihWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NakitBagisciByTarihWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SenderAppQS
        {
            get
            {
                if (ViewState["SenderApp"] == null)
                {
                    if (Page.Request.QueryString["SenderApp"] != null)
                    {
                        ViewState["SenderApp"] = Page.Request.QueryString["SenderApp"];
                    }
                    else
                    {
                        ViewState["SenderApp"] = string.Empty;
                    }
                }
                return ViewState["SenderApp"].ToString();
            }

            set
            {
                ViewState["SenderApp"] = value;
            }
        }
        private string DestinationAppQS
        {
            get
            {

                if (ViewState["DestinationApp"] == null)
                {
                    if (Page.Request.QueryString["DestinationApp"] != null)
                    {
                        ViewState["DestinationApp"] = Page.Request.QueryString["DestinationApp"];
                    }
                    else
                    {
                        ViewState["DestinationApp"] = string.Empty;
                    }
                }
                return ViewState["DestinationApp"].ToString();
            }

            set
            {
                ViewState["DestinationApp"] = value;
            }
        }
        private string CurrentUserName
        {
            get
            {

                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUserLoginName();
                }
                return ViewState["CurrentUserName"].ToString();
            }

            set
            {
                ViewState["CurrentUserName"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    setDefaultValues();
                    FillSonucTable();
                    //SMSTablosunuDoldur();
                }

            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void setDefaultValues()
        {
            DateTime today = (DateTime.Today).AddMonths(-1);
            DateTime ayinIlkGunu = new DateTime(today.Year, today.Month, 1);
            DateTime ayinSonGunu = ayinIlkGunu.AddMonths(1).AddDays(-1);
            BasTarTxt.Value = ayinIlkGunu.ConvertToDatetimeEmptyIfNull();
            BitTarTxt.Value = ayinSonGunu.ConvertToDatetimeEmptyIfNull();
        }
        private void FillSonucTable()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            SonucTable.Rows.Clear();
            TableHeaderRow headerThRow = new TableHeaderRow();
            TableHeaderCell headerCell = new TableHeaderCell();
            headerCell.Text = BasTarTxt.Value.ConvertToDatetimeEmptyIfNull() + "-" + BitTarTxt.Value.ConvertToDatetimeEmptyIfNull() + " TARİHLERİ ARASINDA YAPILAN NAKİT BAĞIŞLAR";
            headerCell.ColumnSpan = 7;
            headerThRow.Controls.Add(headerCell);
            SonucTable.Controls.Add(headerThRow);

            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell bolgeThCell = new TableHeaderCell();
            bolgeThCell.Text = "Bölge";
            bolgeThCell.RowSpan = 2;

            TableHeaderCell bagisciCell = new TableHeaderCell();
            bagisciCell.Text = "Bağışçı Sayısı";
            bagisciCell.ColumnSpan = 3;

            TableHeaderCell bagiMiktariCell = new TableHeaderCell();
            bagiMiktariCell.Text = "Bağış Miktarı";
            bagiMiktariCell.ColumnSpan = 3;

            th.Controls.Add(bolgeThCell);
            th.Controls.Add(bagisciCell);
            th.Controls.Add(bagiMiktariCell);

            TableHeaderRow th1 = new TableHeaderRow();
            TableHeaderCell eskiBagisciCell = new TableHeaderCell();
            eskiBagisciCell.Text = "Eski";
            TableHeaderCell yeniBagisciCell = new TableHeaderCell();
            yeniBagisciCell.Text = "Yeni";
            TableHeaderCell toplamBagisciCell = new TableHeaderCell();
            toplamBagisciCell.Text = "Toplam";

            TableHeaderCell eskiBagisMiktariCell = new TableHeaderCell();
            eskiBagisMiktariCell.Text = "Eski";
            TableHeaderCell yeniBagisMiktariCell = new TableHeaderCell();
            yeniBagisMiktariCell.Text = "Yeni";
            TableHeaderCell toplamBagisMiktariCell = new TableHeaderCell();
            toplamBagisMiktariCell.Text = "Toplam";

            th1.Controls.Add(eskiBagisciCell);
            th1.Controls.Add(yeniBagisciCell);
            th1.Controls.Add(toplamBagisciCell);
            th1.Controls.Add(eskiBagisMiktariCell);
            th1.Controls.Add(yeniBagisMiktariCell);
            th1.Controls.Add(toplamBagisMiktariCell);

            headerThRow.HorizontalAlign = HorizontalAlign.Center;
            th.HorizontalAlign = HorizontalAlign.Center;
            th1.HorizontalAlign = HorizontalAlign.Right;
            SonucTable.Controls.Add(th);
            SonucTable.Controls.Add(th1);

            DateTime bastar = BasTarTxt.Value.ConvertToDatetime();
            DateTime bittar = BitTarTxt.Value.ConvertToDatetime();
            NakitBagisHareket nbh = new NakitBagisHareket();
            DataTable dataTable = nbh.SelectCountSumByBagisTarihi(bastar, bittar);
            if (dataTable != null)
            {
                int eskiAdetToplam1 = 0;
                decimal eskiTutarToplam1 = 0;

                int yeniAdetToplam1 = 0;
                decimal yeniTutarToplam1 = 0;

                int toplamAdetToplam = 0;
                decimal toplamTutarToplam = 0;


                foreach (DataRow row in dataTable.Rows)
                {
                    TableRow tableRow = new TableRow();
                    tableRow.HorizontalAlign = HorizontalAlign.Right;

                    TableCell bolgeCell = new TableCell();
                    string bolge = row == null ? "" : row["Bolge"].ReturnEmptyIfNull().ToString();

                    bolgeCell.Text = string.IsNullOrEmpty(bolge) ? "" : bolge;
                    tableRow.Controls.Add(bolgeCell);

                    int toplamAdet = 0;
                    NakitBagisHareket nbh1 = new NakitBagisHareket();

                    Decimal toplamTutar = nbh1.SelectSumBagisMiktariByBagisTarihiBolge(bastar, bittar, bolge, ref toplamAdet);
                    toplamAdetToplam += toplamAdet;
                    toplamTutarToplam += toplamTutar;

                    int eskiAdet1 = row == null ? 0 : Int32.Parse(row["Adet"].ReturnZeroIfNull().ToString());
                    eskiAdetToplam1 += eskiAdet1;
                    Decimal eskiTutar1 = row == null ? 0 : Decimal.Parse(row["Toplam"].ReturnZeroIfNull().ToString());
                    eskiTutarToplam1 += eskiTutar1;

                    int yeniAdet1 = toplamAdet - eskiAdet1;
                    yeniAdetToplam1 += yeniAdet1;
                    Decimal yeniTutar1 = toplamTutar - eskiTutar1;
                    yeniTutarToplam1 += yeniTutar1;

                    TableCell eskiAdetCell = new TableCell();
                    TableCell yeniAdetCell = new TableCell();
                    TableCell toplamAdetCell = new TableCell();

                    eskiAdetCell.Text = eskiAdet1.ToString();
                    yeniAdetCell.Text = yeniAdet1.ToString();
                    toplamAdetCell.Text = toplamAdet.ToString();

                    tableRow.Controls.Add(eskiAdetCell);
                    tableRow.Controls.Add(yeniAdetCell);
                    tableRow.Controls.Add(toplamAdetCell);

                    TableCell eskiTutarCell = new TableCell();
                    TableCell yeniTutarCell = new TableCell();
                    TableCell toplamTutarCell = new TableCell();

                    eskiTutarCell.Text = eskiTutar1.ToString("N", culturInfo);
                    yeniTutarCell.Text = yeniTutar1.ToString("N", culturInfo);
                    toplamTutarCell.Text = toplamTutar.ToString("N", culturInfo);
                    eskiTutarCell.CssClass = "text-right";
                    yeniTutarCell.CssClass = "text-right";
                    toplamTutarCell.CssClass = "text-right";

                    tableRow.Controls.Add(eskiTutarCell);
                    tableRow.Controls.Add(yeniTutarCell);
                    tableRow.Controls.Add(toplamTutarCell);

                    SonucTable.Controls.Add(tableRow);
                }
                TableRow toplamTableRow = new TableRow();
                toplamTableRow.HorizontalAlign = HorizontalAlign.Right;
                TableCell labelToplamCell = new TableCell();

                TableCell eskiAdetToplamCell = new TableCell();
                TableCell yeniAdetToplamCell = new TableCell();
                TableCell toplamAdetToplamCell = new TableCell();

                labelToplamCell.Text = "Toplam";
                eskiAdetToplamCell.Text = yeniAdetToplam1.ToString();
                yeniAdetToplamCell.Text = eskiAdetToplam1.ToString();
                toplamAdetToplamCell.Text = toplamAdetToplam.ToString();

                toplamTableRow.Controls.Add(labelToplamCell);
                toplamTableRow.Controls.Add(eskiAdetToplamCell);
                toplamTableRow.Controls.Add(yeniAdetToplamCell);
                toplamTableRow.Controls.Add(toplamAdetToplamCell);

                TableCell eskiTutarToplamCell = new TableCell();
                TableCell yeniTutarToplamCell = new TableCell();
                TableCell toplamTutarToplamCell = new TableCell();

                eskiTutarToplamCell.Text = yeniTutarToplam1.ToString("N", culturInfo);
                yeniTutarToplamCell.Text = eskiTutarToplam1.ToString("N", culturInfo);
                toplamTutarToplamCell.Text = toplamTutarToplam.ToString("N", culturInfo);
                eskiTutarToplamCell.CssClass = "text-right";
                yeniTutarToplamCell.CssClass = "text-right";
                toplamTutarToplamCell.CssClass = "text-right";

                toplamTableRow.Controls.Add(eskiTutarToplamCell);
                toplamTableRow.Controls.Add(yeniTutarToplamCell);
                toplamTableRow.Controls.Add(toplamTutarToplamCell);

                labelToplamCell.Font.Bold = true;
                eskiAdetToplamCell.Font.Bold = true;
                yeniAdetToplamCell.Font.Bold = true;
                toplamAdetToplamCell.Font.Bold = true;
                eskiTutarToplamCell.Font.Bold = true;
                yeniTutarToplamCell.Font.Bold = true;
                toplamTutarToplamCell.Font.Bold = true;

                SonucTable.Controls.Add(toplamTableRow);
            }
            //
        }
        //private void SMSTablosunuDoldur()
        //{
        //    IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

        //    SMSTable.Rows.Clear();
        //    TableHeaderRow headerThRow = new TableHeaderRow();
        //    TableHeaderCell headerCell = new TableHeaderCell();
        //    headerCell.Text = BasTarTxt.Value.ConvertToDatetimeEmptyIfNull() + "-" + BitTarTxt.Value.ConvertToDatetimeEmptyIfNull() + " TARİHLERİ ARASINDA YAPILAN SMS BAĞIŞLARI";
        //    headerCell.ColumnSpan = 8;
        //    headerThRow.Controls.Add(headerCell);
        //    SMSTable.Controls.Add(headerThRow);

        //    TableHeaderRow th = new TableHeaderRow();
        //    TableHeaderCell smsOperatorThCell = new TableHeaderCell();
        //    smsOperatorThCell.Text = "GSM Operatörü";

        //    TableHeaderCell bagisciCell = new TableHeaderCell();
        //    bagisciCell.Text = "Bağışçı Sayısı";

        //    TableHeaderCell tutarCell = new TableHeaderCell();
        //    tutarCell.Text = "Bağış Tutarı";
        //    tutarCell.CssClass = "text-right";

        //    th.Controls.Add(smsOperatorThCell);
        //    th.Controls.Add(bagisciCell);
        //    th.Controls.Add(tutarCell);

        //    headerThRow.HorizontalAlign = HorizontalAlign.Center;
        //    th.HorizontalAlign = HorizontalAlign.Center;
        //    SMSTable.Controls.Add(th);

        //    DateTime bastar = BasTarTxt.Value.ConvertToDatetime();
        //    DateTime bittar = BitTarTxt.Value.ConvertToDatetime();

        //    SMSBagis smsBagisDao = new SMSBagis();
        //    DataTable dataTable = smsBagisDao.SelectCountSumByBagisSMSOperator(bastar, bittar);
        //    if (dataTable != null)
        //    {
        //        int bagisciToplam = 0;
        //        decimal tutarToplam = 0;

        //        foreach (DataRow row in dataTable.Rows)
        //        {
        //            TableRow tableRow = new TableRow();
        //            tableRow.HorizontalAlign = HorizontalAlign.Right;

        //            TableCell smsOperatorCell = new TableCell();
        //            string smsOperator = row == null ? "" : row["SMSOperator"].ReturnEmptyIfNull().ToString();
        //            smsOperatorCell.CssClass = "text-center";

        //            smsOperatorCell.Text = string.IsNullOrEmpty(smsOperator) ? "" : smsOperator;
        //            tableRow.Controls.Add(smsOperatorCell);

        //            int bagisciAdet = row == null ? 0 : Int32.Parse(row["ToplamAdet"].ReturnZeroIfNull().ToString());
        //            bagisciToplam += bagisciAdet;

        //            TableCell bagisciAdetCell = new TableCell();
        //            bagisciAdetCell.Text = bagisciAdet.ToString();
        //            bagisciAdetCell.CssClass = "text-center";
        //            tableRow.Controls.Add(bagisciAdetCell);

        //            decimal bagisTutari = row == null ? 0 : row["ToplamTutar"].ReturnZeroIfNull().ConvertToDecimal();
        //            tutarToplam += bagisTutari;

        //            TableCell toplamTutarCell = new TableCell();
        //            toplamTutarCell.Text = bagisTutari.ToString("N", culturInfo);
        //            toplamTutarCell.CssClass = "text-right";

        //            tableRow.Controls.Add(toplamTutarCell);
        //            SMSTable.Controls.Add(tableRow);
        //        }
        //        TableRow toplamTableRow = new TableRow();
        //        toplamTableRow.HorizontalAlign = HorizontalAlign.Right;
        //        TableCell labelToplamCell = new TableCell();

        //        TableCell toplamTutarFTCell = new TableCell();
        //        TableCell toplamAdetFTCell = new TableCell();

        //        labelToplamCell.Text = "Toplam";

        //        toplamTutarFTCell.Text = tutarToplam.ToString("N", culturInfo);
        //        toplamAdetFTCell.Text = bagisciToplam.ToString();

        //        toplamTableRow.Controls.Add(labelToplamCell);
        //        toplamTableRow.Controls.Add(toplamAdetFTCell);
        //        toplamTableRow.Controls.Add(toplamTutarFTCell);


        //        labelToplamCell.CssClass = "text-center";
        //        toplamAdetFTCell.CssClass = "text-center";
        //        toplamTutarFTCell.CssClass = "text-right";

        //        labelToplamCell.Font.Bold = true;
        //        toplamAdetFTCell.Font.Bold = true;
        //        toplamTutarFTCell.Font.Bold = true;

        //        SMSTable.Controls.Add(toplamTableRow);
        //    }
        //    //
        //}
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }

        protected void ListeleBtn_Click(object sender, EventArgs e)
        {
            FillSonucTable();
            //SMSTablosunuDoldur();
        }
    }
}
