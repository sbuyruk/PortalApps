using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;


namespace IKYS_WebParts.AylikYoklamaRaporuWP
{
    [ToolboxItemAttribute(false)]
    public partial class AylikYoklamaRaporuWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public AylikYoklamaRaporuWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string PersonelIdQS
        {
            get
            {

                if (ViewState["PersonelId"] == null)
                {
                    if (Page.Request.QueryString["PersonelId"] != null)
                    {
                        ViewState["PersonelId"] = Page.Request.QueryString["PersonelId"];
                    }
                    else
                    {
                        ViewState["PersonelId"] = string.Empty;
                    }
                }
                return ViewState["PersonelId"].ToString();
            }

            set
            {
                ViewState["PersonelId"] = value;
            }
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
                    YilDDLDoldur();
                    AyDDLDoldur();
                    SetDDLValues();

                    KayitGetir();
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exHelper = new ExceptionHelper(exception);
                exHelper.PublishException();
            }
        }
        private void KayitGetir()
        {

            List<AylikYoklamaListItem> yoklamaList = YoklamaListesiniDoldur();
            List<AylikYoklamaListItem> mazeretIzinList = MazeretIzinListesiniDoldur();
            List<AylikYoklamaListItem> toplamListe = new List<AylikYoklamaListItem>();
            toplamListe.AddRange(yoklamaList);
            toplamListe.AddRange(mazeretIzinList);

            List<AylikYoklamaListItem> SortedList = toplamListe.OrderBy(o => o.ProtokolSiraNo).ToList();

            AylikYoklamaTable.Controls.Clear();
            AylikYoklamaTableHeaders();
            TabloyuDoldur(SortedList);
        }
        private void TabloyuDoldur(List<AylikYoklamaListItem> sortedList)
        {
            int sayac = 1;
            string tempAdSoyad = string.Empty;
            Color renk = Color.LightGray;
            foreach (var item in sortedList)
            {
                TableRow row = new TableRow();

                TableCell siraCell = new TableCell();
                siraCell.Text = sayac.ToString();

                TableCell adiSoyadiCell = new TableCell();
                adiSoyadiCell.Text = item.AdiSoyadi;

                TableCell bulunmamaSebebiCell = new TableCell();
                bulunmamaSebebiCell.Text = item.BulunmamaSebebi;

                TableCell baslamaTarihiCell = new TableCell();
                baslamaTarihiCell.Text = item.BaslangicTarihi;
                TableCell baslamaSaatiCell = new TableCell();
                baslamaSaatiCell.Text = item.BaslamaSaati;

                TableCell bitisSaatiCell = new TableCell();
                bitisSaatiCell.Text = item.BitisSaati;
                TableCell sureCell = new TableCell();
                sureCell.Text = item.Sure;
                TableCell aciklamaCell = new TableCell();
                aciklamaCell.Text = item.Aciklama;

                TableCell mesaiyeGelisSaatiCell = new TableCell();
                mesaiyeGelisSaatiCell.Text = string.Empty;
                TableCell mesaidenCikisSaatiCell = new TableCell();
                mesaidenCikisSaatiCell.Text = string.Empty;

                bulunmamaSebebiCell.BorderWidth = 1;
                baslamaTarihiCell.BorderWidth = 1;
                baslamaSaatiCell.BorderWidth = 1;
                bitisSaatiCell.BorderWidth = 1;
                sureCell.BorderWidth = 1;
                aciklamaCell.BorderWidth = 1;
                mesaiyeGelisSaatiCell.BorderWidth = 1;
                mesaidenCikisSaatiCell.BorderWidth = 1;

                siraCell.BorderWidth = 1;
                adiSoyadiCell.BorderWidth = 1;

                if (item.AdiSoyadi.Equals(tempAdSoyad))
                {
                    siraCell.Text = string.Empty;
                }
                else
                {
                    TableRow bosSatirRow = new TableRow();
                    TableCell bosSatirCell = new TableCell();
                    bosSatirCell.ColumnSpan = 10;
                    bosSatirRow.Controls.Add(bosSatirCell);
                    AylikYoklamaTable.Controls.Add(bosSatirRow);
                    sayac++;

                    //renk = renk.ToArgb().Equals(Color.White.ToArgb()) ? Color.LightGray : Color.White;
                }
                
                renk = item.BulunmamaSebebi.Contains(ProjeConstants.IZINTIPI_MAZERET) ? Color.FromArgb(221, 235, 247): Color.LightGray;
                siraCell.BackColor = renk;
                adiSoyadiCell.BackColor = renk;
                bulunmamaSebebiCell.BackColor = renk;
                baslamaTarihiCell.BackColor = renk;
                baslamaSaatiCell.BackColor = renk;
                bitisSaatiCell.BackColor = renk;
                sureCell.BackColor = renk;
                aciklamaCell.BackColor = renk;
                mesaiyeGelisSaatiCell.BackColor = renk;
                mesaidenCikisSaatiCell.BackColor = renk;

                tempAdSoyad = item.AdiSoyadi;

                row.Controls.Add(siraCell);
                row.Controls.Add(adiSoyadiCell);
                row.Controls.Add(bulunmamaSebebiCell);
                row.Controls.Add(baslamaTarihiCell);
                row.Controls.Add(baslamaSaatiCell);
                row.Controls.Add(bitisSaatiCell);
                row.Controls.Add(sureCell);
                row.Controls.Add(aciklamaCell);

                row.Controls.Add(mesaiyeGelisSaatiCell);
                row.Controls.Add(mesaidenCikisSaatiCell);

                AylikYoklamaTable.Controls.Add(row);
            }
        }
        private void AylikYoklamaTableHeaders()
        {

            AylikYoklamaTable.Rows.Clear();

            TableHeaderRow thbaslik = new TableHeaderRow();
            TableHeaderCell baslikCell = new TableHeaderCell();
            DateTime secilenTarih = new DateTime(SecilenYilQS.ConvertToInt(), SecilenAyQS.ConvertToInt(), 1);
            baslikCell.Text = "GÜN IÇINDE VAKIF DISINDA BULUNAN PERSONEL LISTESI (" + secilenTarih.ToString("MMMM").ToUpper() + " " + secilenTarih.ToString("yyyy") + ")";
            baslikCell.ColumnSpan = 10;
            thbaslik.CssClass = "alert-secondary text-center";
            thbaslik.Controls.Add(baslikCell);
            AylikYoklamaTable.Controls.Add(thbaslik);

            TableHeaderRow th = new TableHeaderRow();
            th.CssClass = "alert-secondary text-center";
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sira";
            TableHeaderCell adiSoyadiCell = new TableHeaderCell();
            adiSoyadiCell.Text = "Adi Soyadi";
            TableHeaderCell bulunmamaSebebiCell = new TableHeaderCell();
            bulunmamaSebebiCell.Text = "Bulunmama Sebebi";
            TableHeaderCell baslangicTarCell = new TableHeaderCell();
            baslangicTarCell.Text = "Tarih";
            TableHeaderCell baslangicSaatiCell = new TableHeaderCell();
            baslangicSaatiCell.Text = "Gidis Saati";

            TableHeaderCell bitisSaatiCell = new TableHeaderCell();
            bitisSaatiCell.Text = "Dönüs Saati";
            TableHeaderCell sureCell = new TableHeaderCell();
            sureCell.Text = "Süre";

            TableHeaderCell aciklamaCell = new TableHeaderCell();
            aciklamaCell.Text = "Açiklama";

            TableHeaderCell mesaiyeGelisSaatiCell = new TableHeaderCell();
            mesaiyeGelisSaatiCell.Text = "Mesaiye Gelis";
            TableHeaderCell mesaidenCikisSaatiCell = new TableHeaderCell();
            mesaidenCikisSaatiCell.Text = "Mesaiden Çikis";

            baslikCell.BorderWidth = 1;
            siraCell.BorderWidth = 1;
            adiSoyadiCell.BorderWidth = 1;
            bulunmamaSebebiCell.BorderWidth = 1;
            baslangicTarCell.BorderWidth = 1;
            baslangicSaatiCell.BorderWidth = 1;
            bitisSaatiCell.BorderWidth = 1;
            sureCell.BorderWidth = 1;
            aciklamaCell.BorderWidth = 1;

            mesaiyeGelisSaatiCell.BorderWidth = 1;
            mesaidenCikisSaatiCell.BorderWidth = 1;

            th.Controls.Add(siraCell);
            th.Controls.Add(adiSoyadiCell);
            th.Controls.Add(bulunmamaSebebiCell);
            th.Controls.Add(baslangicTarCell);
            th.Controls.Add(baslangicSaatiCell);
            th.Controls.Add(bitisSaatiCell);

            th.Controls.Add(sureCell);
            th.Controls.Add(aciklamaCell);

            th.Controls.Add(mesaiyeGelisSaatiCell);
            th.Controls.Add(mesaidenCikisSaatiCell);
            AylikYoklamaTable.Controls.Add(th);
        }
        private void YilDDLDoldur()
        {
            int buYil = DateTime.Today.Year;
            int gecenYil = DateTime.Today.AddYears(-1).Year;
            int oncekiYil = DateTime.Today.AddYears(-2).Year;
            ListItem li = new ListItem(buYil.ToString());
            ListItem li1 = new ListItem(gecenYil.ToString());
            ListItem li2 = new ListItem(oncekiYil.ToString());
            YilDDL.Items.Add(li);
            YilDDL.Items.Add(li1);
            YilDDL.Items.Add(li2);
        }
        private void AyDDLDoldur()
        {
            AyDDL.Items.Add(new ListItem("Ocak", "1"));
            AyDDL.Items.Add(new ListItem("Şubat", "2"));
            AyDDL.Items.Add(new ListItem("Mart", "3"));
            AyDDL.Items.Add(new ListItem("Nisan", "4"));
            AyDDL.Items.Add(new ListItem("Mayıs", "5"));
            AyDDL.Items.Add(new ListItem("Haziran", "6"));
            AyDDL.Items.Add(new ListItem("Temmuz", "7"));
            AyDDL.Items.Add(new ListItem("Ağustos", "8"));
            AyDDL.Items.Add(new ListItem("Eylül", "9"));
            AyDDL.Items.Add(new ListItem("Ekim", "10"));
            AyDDL.Items.Add(new ListItem("Kasım", "11"));
            AyDDL.Items.Add(new ListItem("Aralık", "12"));

        }
        private void SetDDLValues()
        {
            try
            {
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
            catch (Exception)
            {

                //TODO
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        private List<AylikYoklamaListItem> YoklamaListesiniDoldur()
        {
            DataTable dataTable = null;
            Yoklama yoklamaDao = new Yoklama();
            int ay = SecilenAyQS.ConvertToInt();
            int yil = SecilenYilQS.ConvertToInt();
            DateTime basTar = new DateTime(yil, ay, 1);
            DateTime bitTar = basTar.AddMonths(1).AddDays(-1) + ProjeConstants.MESAI_BITIS_SAATI;
            string bulunmamaSebebiIds = "2,3";//hastanede, görevli
            dataTable = yoklamaDao.SelectByTarihReturnDataTable(bulunmamaSebebiIds, basTar, bitTar);
            List<AylikYoklamaListItem> yoklamaList = new List<AylikYoklamaListItem>();
            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    int protokolSiraNo = dataRow["ProtokolSiraNo"].ReturnZeroIfNull().ConvertToInt();
                    string bulunmamaSebebi = dataRow["BulunmamaSebebi"].ReturnEmptyIfNull().ToString();
                    string adiSoyadi = dataRow["AdiSoyadi"].ReturnEmptyIfNull().ToString();
                    DateTime baslangicTarihi = dataRow["BaslangicTarihi"].ConvertToDatetime();
                    DateTime bitisTarihi = dataRow["BitisTarihi"].ConvertToDatetime();
                    string aciklama = dataRow["Aciklama"].ReturnEmptyIfNull().ToString();
                    AylikYoklamaListItem item = new AylikYoklamaListItem();
                    item.ProtokolSiraNo = protokolSiraNo;
                    item.AdiSoyadi = adiSoyadi;
                    item.BulunmamaSebebi = bulunmamaSebebi;
                    item.BaslangicTarihi = baslangicTarihi.ConvertToDatetimeEmptyIfNull(); ;
                    item.BitisTarihi = bitisTarihi.ConvertToDatetimeEmptyIfNull();
                    item.BaslamaSaati = baslangicTarihi.ToString("HH:mm");
                    item.BitisSaati = bitisTarihi.ToString("HH:mm");
                    item.Sure = SureHesapla(baslangicTarihi, bitisTarihi);
                    item.Aciklama = aciklama.ToUpper();
                    yoklamaList.Add(item);

                }
            }
            return yoklamaList;
        }
        private List<AylikYoklamaListItem> MazeretIzinListesiniDoldur()
        {
            DataTable dataTable = null;

            int ay = SecilenAyQS.ConvertToInt();
            int yil = SecilenYilQS.ConvertToInt();
            DateTime basTar = new DateTime(yil, ay, 1);
            DateTime bitTar = basTar.AddMonths(1).AddMinutes(-1);
            IzinHareket izinHarekeDao = new IzinHareket();
            dataTable = izinHarekeDao.SelectByIzinTipiTarihReturnDataTable(ProjeConstants.IZINTIPI_MAZERET_INT, basTar, bitTar);
            List<AylikYoklamaListItem> mazeretList = new List<AylikYoklamaListItem>();
            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    int protokolSiraNo = dataRow["ProtokolSiraNo"].ReturnZeroIfNull().ConvertToInt();
                    string bulunmamaSebebi = ProjeConstants.IZINTIPI_MAZERET + "  izni";
                    string adiSoyadi = dataRow["AdiSoyadi"].ReturnEmptyIfNull().ToString();
                    DateTime baslangicTarihi = dataRow["BaslangicTarihi"].ConvertToDatetime();
                    DateTime bitisTarihi = dataRow["BitisTarihi"].ConvertToDatetime();
                    string aciklama = dataRow["Aciklama"].ReturnEmptyIfNull().ToString();
                    AylikYoklamaListItem item = new AylikYoklamaListItem();
                    item.ProtokolSiraNo = protokolSiraNo;
                    item.AdiSoyadi = adiSoyadi;
                    item.BulunmamaSebebi = bulunmamaSebebi;
                    item.BaslangicTarihi = baslangicTarihi.ConvertToDatetimeEmptyIfNull(); ;
                    item.BitisTarihi = bitisTarihi.ConvertToDatetimeEmptyIfNull();
                    item.BaslamaSaati = baslangicTarihi.ToString("HH:mm");
                    item.BitisSaati = bitisTarihi.ToString("HH:mm");
                    item.Sure = SureHesapla(baslangicTarihi, bitisTarihi);
                    item.Aciklama = aciklama.ToUpper();
                    mazeretList.Add(item);
                }
            }
            return mazeretList;
        }
        private string SureHesapla(DateTime bas, DateTime bit)
        {
            string sure = string.Empty;
            int gun = (bit - bas).Days;

            int saat = (bit - bas).Hours;//artan saat var mi
            if (saat >= 9)
            {
                gun++;
                saat = 0;
            }
            string gunStr = gun > 0 ? gun + " Gün " : "";
            string saatstr = saat > 0 ? saat + " Saat" : "";
            if (gun > 0)
                sure = gunStr + saatstr;
            else
                sure = (bit - bas).ConvertToTimeSpanReturnInHHmm();
            return sure;
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
            KayitGetir();
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenAyQS = AyDDL.SelectedItem.Value.ToString();
            KayitGetir();
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            KayitGetir();
            ExportToExcel();
        }
        protected void ExportToExcel()
        {
            string filename = "AylikGunIciBulunmayanPersonel" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            AylikYoklamaTable.RenderControl(hw);
            //Write the HTML back to the browser.
            //Response.ContentType = application/vnd.ms-excel;
            Page.Response.ContentType = "application/ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            string s = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\n<head>\n<title></title>\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1254\" />\n<style>\n</style>\n</head>\n<body>\n";
            Page.Response.Write(s + tw.ToString());
            Page.Response.End();

        }
        public class AylikYoklamaListItem
        {
            public int ProtokolSiraNo { get; set; }
            public string PersonelId { get; set; }
            public string AdiSoyadi { get; set; }
            public string BulunmamaSebebi { get; set; }
            public string BaslangicTarihi { get; set; }
            public string BitisTarihi { get; set; }
            public string BaslamaSaati { get; set; }
            public string BitisSaati { get; set; }
            public string Sure { get; set; }
            public string Aciklama { get; set; }
        }
    }
}
