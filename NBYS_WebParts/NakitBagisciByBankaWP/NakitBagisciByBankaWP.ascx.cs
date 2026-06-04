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

namespace NBYS_WebParts.NakitBagisciByBankaWP
{
    [ToolboxItemAttribute(false)]
    public partial class NakitBagisciByBankaWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NakitBagisciByBankaWP()
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
        private string SecilenAyQS
        {
            get
            {

                if (ViewState["SecilenAy"] == null)
                {
                    if (Page.Request.QueryString["SecilenAy"] != null)
                    {
                        ViewState["SecilenAy"] = Page.Request.QueryString["SecilenAy"];
                    }
                    else
                    {
                        ViewState["SecilenAy"] = string.Empty;
                    }
                }
                return ViewState["SecilenAy"].ToString();
            }

            set
            {
                ViewState["SecilenAy"] = value;
            }
        }
        private string SecilenYilQS
        {
            get
            {

                if (ViewState["SecilenYil"] == null)
                {
                    if (Page.Request.QueryString["SecilenYil"] != null)
                    {
                        ViewState["SecilenYil"] = Page.Request.QueryString["SecilenYil"];
                    }
                    else
                    {
                        ViewState["SecilenYil"] = string.Empty;
                    }
                }
                return ViewState["SecilenYil"].ToString();
            }

            set
            {
                ViewState["SecilenYil"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    DateTime today = DateTime.Today;

                    DateTime bastar = new DateTime(today.Year, today.Month, 1);
                    DateTime bittar = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1);


                    SecilenAyQS = bastar.Month.ToString();
                    SecilenYilQS = bastar.Year.ToString();

                    FillMonth();
                    FillYear();
                    SetDDLValues();
                    NakitBagisciTablosunuDoldur();
                    //SMSTablosunuDoldur();
                }

            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void SetDDLValues()
        {
            try
            {
                //acilista ay ve yili querystring ile gelen ay ve yila esitle bos geldiyse gecen aya/yila esitle

                //ay
                string ay = !string.IsNullOrEmpty(SecilenAyQS) ? SecilenAyQS : DateTime.Today.Month.ReturnEmptyIfNull().ToString();
                ListItem AyItem = new ListItem();
                if (!string.IsNullOrEmpty(ay))
                    AyItem = AyDDL.Items.FindByValue(ay);

                if (AyItem != null)
                {
                    AyDDL.SelectedValue = AyItem.Value;
                    SecilenAyQS = AyItem.Value;
                }

                //yil
                string yil = !string.IsNullOrEmpty(SecilenYilQS) ? SecilenYilQS : DateTime.Today.Year.ReturnEmptyIfNull().ToString();
                ListItem YilItem = new ListItem();
                if (!string.IsNullOrEmpty(yil))
                    YilItem = YilDDL.Items.FindByValue(yil);

                if (YilItem != null)
                {
                    YilDDL.SelectedValue = YilItem.Value;
                    SecilenYilQS = YilItem.Value;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }



        }
        private void NakitBagisciTablosunuDoldur()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            NakitBagisciTable.Rows.Clear();
            TableHeaderRow headerThRow = new TableHeaderRow();
            TableHeaderCell headerCell = new TableHeaderCell();
            headerCell.Text = AyDDL.SelectedItem.Text + " " + YilDDL.SelectedItem.Text + " Nakit Bagis ve Bagisçi Durumu";
            headerCell.ColumnSpan = 8;
            headerThRow.Controls.Add(headerCell);
            NakitBagisciTable.Controls.Add(headerThRow);

            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell bankaThCell = new TableHeaderCell();
            bankaThCell.Text = "Banka";
            bankaThCell.RowSpan = 2;

            TableHeaderCell bagisciCell = new TableHeaderCell();
            bagisciCell.Text = "Bagisçi Sayisi";
            bagisciCell.ColumnSpan = 3;

            TableHeaderCell bagiMiktariCell = new TableHeaderCell();
            bagiMiktariCell.Text = "Bagis Miktari";
            bagiMiktariCell.ColumnSpan = 3;

            th.Controls.Add(bankaThCell);
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
            TableHeaderCell enYuksekBagisCell = new TableHeaderCell();
            enYuksekBagisCell.Text = "En Yüksek Bagis";

            th1.Controls.Add(eskiBagisciCell);
            th1.Controls.Add(yeniBagisciCell);
            th1.Controls.Add(toplamBagisciCell);
            th1.Controls.Add(eskiBagisMiktariCell);
            th1.Controls.Add(yeniBagisMiktariCell);
            th1.Controls.Add(toplamBagisMiktariCell);
            th1.Controls.Add(enYuksekBagisCell);

            headerThRow.HorizontalAlign = HorizontalAlign.Center;
            th.HorizontalAlign = HorizontalAlign.Center;
            th1.HorizontalAlign = HorizontalAlign.Right;
            NakitBagisciTable.Controls.Add(th);
            NakitBagisciTable.Controls.Add(th1);


            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();



            DateTime bastar = new DateTime(yil, ay==0?1:ay, 1);
            DateTime bittar = bastar.AddMonths(1).AddDays(-1);

            if (AyDDL.SelectedItem.Text.Equals(ProjeConstants.HEPSI))
            {
                bastar = new DateTime(yil, 1, 1);
                bittar = new DateTime(yil, 12, 31);
            }

            NakitBagisHareket nbh = new NakitBagisHareket();
            DataTable dataTable = nbh.SelectCountSumByBagisBanka(bastar, bittar);
            if (dataTable != null)
            {
                int yeniBagisAdetToplam1 = 0;
                decimal yeniBagisTutarToplam1 = 0;

                int hepsiAdetToplam1 = 0;
                decimal hepsiTutarToplam1 = 0;

                int eskiBagisAdetToplam1 = 0;
                decimal eskiBagisTutarToplam1 = 0;


                foreach (DataRow row in dataTable.Rows)
                {
                    TableRow tableRow = new TableRow();
                    tableRow.HorizontalAlign = HorizontalAlign.Right;

                    TableCell bankaCell = new TableCell();
                    string banka = row == null ? "" : row["Banka"].ReturnEmptyIfNull().ToString();

                    bankaCell.Text = string.IsNullOrEmpty(banka) ? "" : banka;
                    tableRow.Controls.Add(bankaCell);

                    int eskiBagisAdet1 = 0;
                    NakitBagisHareket nbh1 = new NakitBagisHareket();

                    Decimal eskiBagisTutar1 = nbh1.SelectSumBagisMiktariByBagisTarihiBanka(bastar, bittar, banka, ref eskiBagisAdet1);
                    eskiBagisAdetToplam1 += eskiBagisAdet1;
                    eskiBagisTutarToplam1 += eskiBagisTutar1;

                    int hepsiAdet1 = row == null ? 0 : Int32.Parse(row["Adet"].ReturnZeroIfNull().ToString());
                    hepsiAdetToplam1 += hepsiAdet1;

                    Decimal hepsiTutar1 = row == null ? 0 : Decimal.Parse(row["Toplam"].ReturnZeroIfNull().ToString());
                    hepsiTutarToplam1 += hepsiTutar1;

                    int yeniBagisAdet1 = hepsiAdet1 - eskiBagisAdet1;
                    yeniBagisAdetToplam1 += yeniBagisAdet1;

                    Decimal yeniBagisTutar1 = hepsiTutar1 - eskiBagisTutar1;
                    yeniBagisTutarToplam1 += yeniBagisTutar1;

                    TableCell eskiAdetCell = new TableCell();
                    TableCell yeniAdetCell = new TableCell();
                    TableCell toplamAdetCell = new TableCell();

                    eskiAdetCell.Text = eskiBagisAdet1.ToString();
                    yeniAdetCell.Text = yeniBagisAdet1.ToString();
                    toplamAdetCell.Text = hepsiAdet1.ToString();

                    tableRow.Controls.Add(eskiAdetCell);
                    tableRow.Controls.Add(yeniAdetCell);
                    tableRow.Controls.Add(toplamAdetCell);

                    TableCell eskiTutarCell = new TableCell();
                    TableCell yeniTutarCell = new TableCell();
                    TableCell toplamTutarCell = new TableCell();

                    eskiTutarCell.Text = eskiBagisTutar1.ToString("N", culturInfo);
                    yeniTutarCell.Text = yeniBagisTutar1.ToString("N", culturInfo);
                    toplamTutarCell.Text = hepsiTutar1.ToString("N", culturInfo);
                    eskiTutarCell.CssClass = "text-end";
                    yeniTutarCell.CssClass = "text-end";
                    toplamTutarCell.CssClass = "text-end";

                    tableRow.Controls.Add(eskiTutarCell);
                    tableRow.Controls.Add(yeniTutarCell);
                    tableRow.Controls.Add(toplamTutarCell);

                    TableCell enYuksekCell = new TableCell();
                    Decimal enYuksekBagis = nbh1.SelectMaxBagisMiktariByBagisTarihiBanka(bastar, bittar, banka);
                    enYuksekCell.Text = enYuksekBagis.ToString("N", culturInfo);
                    tableRow.Controls.Add(enYuksekCell);

                    NakitBagisciTable.Controls.Add(tableRow);
                }
                TableRow toplamTableRow = new TableRow();
                toplamTableRow.HorizontalAlign = HorizontalAlign.Right;
                TableCell labelToplamCell = new TableCell();

                TableCell eskiAdetToplamCell = new TableCell();
                TableCell yeniAdetToplamCell = new TableCell();
                TableCell toplamAdetToplamCell = new TableCell();

                labelToplamCell.Text = "Toplam";
                eskiAdetToplamCell.Text = eskiBagisAdetToplam1.ToString();
                yeniAdetToplamCell.Text = yeniBagisAdetToplam1.ToString();
                toplamAdetToplamCell.Text = hepsiAdetToplam1.ToString();

                toplamTableRow.Controls.Add(labelToplamCell);
                toplamTableRow.Controls.Add(eskiAdetToplamCell);
                toplamTableRow.Controls.Add(yeniAdetToplamCell);
                toplamTableRow.Controls.Add(toplamAdetToplamCell);

                TableCell eskiTutarToplamCell = new TableCell();
                TableCell yeniTutarToplamCell = new TableCell();
                TableCell hepsiTutarToplamCell = new TableCell();

                eskiTutarToplamCell.Text = eskiBagisTutarToplam1.ToString("N", culturInfo);
                yeniTutarToplamCell.Text = yeniBagisTutarToplam1.ToString("N", culturInfo);
                hepsiTutarToplamCell.Text = hepsiTutarToplam1.ToString("N", culturInfo);
                eskiTutarToplamCell.CssClass = "text-end";
                yeniTutarToplamCell.CssClass = "text-end";
                hepsiTutarToplamCell.CssClass = "text-end";

                toplamTableRow.Controls.Add(eskiTutarToplamCell);
                toplamTableRow.Controls.Add(yeniTutarToplamCell);
                toplamTableRow.Controls.Add(hepsiTutarToplamCell);

                labelToplamCell.Font.Bold = true;
                eskiAdetToplamCell.Font.Bold = true;
                yeniAdetToplamCell.Font.Bold = true;
                toplamAdetToplamCell.Font.Bold = true;
                eskiTutarToplamCell.Font.Bold = true;
                yeniTutarToplamCell.Font.Bold = true;
                hepsiTutarToplamCell.Font.Bold = true;

                NakitBagisciTable.Controls.Add(toplamTableRow);
            }
            //
        }
        //private void SMSTablosunuDoldur()
        //{
        //    IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

        //    SMSTable.Rows.Clear();
        //    TableHeaderRow headerThRow = new TableHeaderRow();
        //    TableHeaderCell headerCell = new TableHeaderCell();
        //    headerCell.Text = AyDDL.SelectedItem.Text + " " + YilDDL.SelectedItem.Text + " SMS Bagis Durumu";
        //    headerCell.ColumnSpan = 8;
        //    headerThRow.Controls.Add(headerCell);
        //    SMSTable.Controls.Add(headerThRow);

        //    TableHeaderRow th = new TableHeaderRow();
        //    TableHeaderCell smsOperatorThCell = new TableHeaderCell();
        //    smsOperatorThCell.Text = "GSM Operatörü";

        //    TableHeaderCell bagisciCell = new TableHeaderCell();
        //    bagisciCell.Text = "Bagisçi Sayisi";

        //    TableHeaderCell tutarCell = new TableHeaderCell();
        //    tutarCell.Text = "Bagis Tutari";
        //    tutarCell.CssClass = "text-end";

        //    th.Controls.Add(smsOperatorThCell);
        //    th.Controls.Add(bagisciCell);
        //    th.Controls.Add(tutarCell);

        //    headerThRow.HorizontalAlign = HorizontalAlign.Center;
        //    th.HorizontalAlign = HorizontalAlign.Center;
        //    SMSTable.Controls.Add(th);

        //    int ay = AyDDL.SelectedItem.Value.ConvertToInt();
        //    int yil = YilDDL.SelectedItem.Value.ConvertToInt();
        //    DateTime bastar = new DateTime(yil, ay, 1);
        //    DateTime bittar = bastar.AddMonths(1).AddDays(-1);

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
        //            toplamTutarCell.CssClass = "text-end";

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
        //        toplamTutarFTCell.CssClass = "text-end";

        //        labelToplamCell.Font.Bold = true;
        //        toplamAdetFTCell.Font.Bold = true;
        //        toplamTutarFTCell.Font.Bold = true;

        //        SMSTable.Controls.Add(toplamTableRow);
        //    }
        //    //
        //}
        private void FillMonth()
        {

            AyDDL.Items.Add(new ListItem("Ocak", "1"));
            AyDDL.Items.Add(new ListItem("Subat", "2"));
            AyDDL.Items.Add(new ListItem("Mart", "3"));
            AyDDL.Items.Add(new ListItem("Nisan", "4"));
            AyDDL.Items.Add(new ListItem("Mayis", "5"));
            AyDDL.Items.Add(new ListItem("Haziran", "6"));
            AyDDL.Items.Add(new ListItem("Temmuz", "7"));
            AyDDL.Items.Add(new ListItem("Agustos", "8"));
            AyDDL.Items.Add(new ListItem("Eylül", "9"));
            AyDDL.Items.Add(new ListItem("Ekim", "10"));
            AyDDL.Items.Add(new ListItem("Kasim", "11"));
            AyDDL.Items.Add(new ListItem("Aralik", "12"));
            AyDDL.Items.Add(new ListItem("Hepsi", "0"));


        }
        private void FillYear()
        {
            var year = DateTime.Now.Year;
            for (int i = 2018; i <= year; i++)
            {
                YilDDL.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            NakitBagisciTablosunuDoldur();
            //SMSTablosunuDoldur();
        }

        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            NakitBagisciTablosunuDoldur();
            //SMSTablosunuDoldur();
        }
    }
}
