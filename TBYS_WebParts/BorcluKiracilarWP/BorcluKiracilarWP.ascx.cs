using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.BorcluKiracilarWP
{
    [ToolboxItemAttribute(false)]
    public partial class BorcluKiracilarWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BorcluKiracilarWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string AuthQS
        {
            get
            {

                if (ViewState["Auth"] == null || string.IsNullOrEmpty(ViewState["Auth"].ToString()))
                {
                    if (Page.Request.QueryString["Auth"] != null)
                    {
                        ViewState["Auth"] = Page.Request.QueryString["Auth"];
                    }
                    else
                    {
                        ViewState["Auth"] = string.Empty;
                    }
                }
                return ViewState["Auth"].ToString();
            }

            set
            {
                ViewState["Auth"] = value;
            }
        }
        private int BolgeIdQS
        {
            get
            {

                if (ViewState["BolgeId"] == null)
                {
                    if (Page.Request.QueryString["BolgeId"] != null)
                    {
                        ViewState["BolgeId"] = Page.Request.QueryString["BolgeId"];
                    }
                    else
                    {
                        ViewState["BolgeId"] = string.Empty;
                    }
                }
                return ViewState["BolgeId"].ReturnZeroIfNull().ConvertToInt();
            }

            set
            {
                ViewState["BolgeId"] = value;
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
        private string AySayisiBasQS
        {
            get
            {

                if (ViewState["AySayisiBas"] == null)
                {
                    if (Page.Request.QueryString["AySayisiBas"] != null)
                    {
                        ViewState["AySayisiBas"] = Page.Request.QueryString["AySayisiBas"];
                    }
                    else
                    {
                        ViewState["AySayisiBas"] = "1";
                    }
                }
                return ViewState["AySayisiBas"].ToString();
            }

            set
            {
                ViewState["AySayisiBas"] = value;
            }
        }
        private string AySayisiBitQS
        {
            get
            {

                if (ViewState["AySayisiBit"] == null)
                {
                    if (Page.Request.QueryString["AySayisiBit"] != null)
                    {
                        ViewState["AySayisiBit"] = Page.Request.QueryString["AySayisiBit"];
                    }
                    else
                    {
                        ViewState["AySayisiBit"] = "99999999";
                    }
                }
                return ViewState["AySayisiBit"].ToString();
            }

            set
            {
                ViewState["AySayisiBit"] = value;
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
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        private readonly int BolgeColumnSpan = 8;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                AdiLbl.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                TitleLbl.Text = "Borçlu Kiracı Listesi";
                try
                {

                    SetAyYilValues();
                    if (BolgeIdQS == ProjeConstants.BOLGE_HEPSI_INT)
                    {
                        Bolge bolge = IKYSOrtak.BolgeGetirByUserName(CurrentUserName);
                        BolgeIdQS = bolge == null ? 0 : bolge.Id;
                        TitleLbl.Text = "Borçlu Kiracı Listesi" + " (" + bolge.KisaAdi + " Bölgesi)";
                    }

                    bool yetkiliMi = !string.IsNullOrEmpty(AuthQS) && AuthQS.Equals(ProjeConstants.TBYS_YETKILI_BIRIM);

                    OdemePlanlariniGuncelleBtn.Visible = yetkiliMi;
                    BorcluKiraclariTabloyaDoldur();
                }
                catch (Exception ex)
                {

                    ExceptionHelper exh = new ExceptionHelper(ex);
                    exh.PublishException();

                } 
            }

        }
        private void SetAyYilValues()
        {
            try
            {
                //ay
                string ay = !string.IsNullOrEmpty(SecilenAyQS) ? SecilenAyQS : DateTime.Today.Month.ReturnEmptyIfNull().ToString();

                SecilenAyQS = ay;

                //yil
                string yil = !string.IsNullOrEmpty(SecilenYilQS) ? SecilenYilQS : DateTime.Today.Year.ReturnEmptyIfNull().ToString();
                SecilenYilQS = yil;
            }
            catch (Exception)
            {

                //TODO
            }
        }
        private void BorcluKiraclariTabloyaDoldur()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            //SB 10.08.2020 secilentarih ayın 1'ii yerine bugün 
            //** SB üstteki İptal 16.09.2020

            int ay = SecilenAyQS.ConvertToInt();
            int yil = SecilenYilQS.ConvertToInt();
            DateTime secilenTarih = new DateTime(yil, ay, 1);
            DateTime vadeBastar = secilenTarih;
            DateTime vadeBittar = secilenTarih.AddMonths(1).AddDays(-1);

            BorcluKiracilarTableHeaderRows();


            OdemePlani opl = new OdemePlani();
            DataTable dataTable = opl.SelectBorcluOdemePlanlariByBolgeTarih(BolgeIdQS, vadeBastar, vadeBittar, AySayisiBasQS.ConvertToInt(), AySayisiBitQS.ConvertToInt());

            if (dataTable != null)
            {

                int SiraNo = 1;
                decimal bolgeAnaParaToplami = 0;
                decimal bolgeFaizliBakiyeToplami = 0;
                decimal anaParaToplami = 0;
                decimal faizliBakiyeToplami = 0;

                string tempBolge = string.Empty;
                bool ilkKayit = true;
                int toplamKayitSayisi = dataTable.Rows.Count;
                int counter = 0;

                foreach (DataRow row in dataTable.Rows)
                {
                    int kiraSozlesmeId = row == null ? 0 : row["KiraSozlesmeId"].ReturnEmptyIfNull().ConvertToInt();
                    int taksitSayisi = row == null ? 0 : row["TaksitSayisi"].ReturnZeroIfNull().ConvertToInt();
                    int aySayisi = row == null ? 0 : (int)(Math.Round(row["AySayisi"].ReturnZeroIfNull().ConvertToDecimal()));
                    string dosyaNo = row == null ? "0" : row["DosyaNo"].ReturnEmptyIfNull().ToString();
                    string bolge = row == null ? "" : row["Bolge"].ReturnEmptyIfNull().ToString();
                    string kiraci = row == null ? "" : row["Kiraci"].ReturnEmptyIfNull().ToString();
                    string ilkSozlesmeTar = row == null ? "" : row["IlkSozlesmeTar"].ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                    decimal kiraBedeliDec = row == null ? 0 : row["KiraBedeli"].ConvertToDecimal();
                    string kiraBedeli = row == null ? "" : row["KiraBedeli"].ConvertToDecimal().ToString("N", culturInfo);
                    decimal anaParaDecimal = row == null ? 0 : row["AnaPara"].ConvertToDecimal();
                    string anaPara = anaParaDecimal.ToString("N", culturInfo);
                    decimal faizliBakiyeDec = row == null ? 0 : row["FaizliBakiye"].ConvertToDecimal();
                    string faizliBakiye = row == null ? "" : row["FaizliBakiye"].ConvertToDecimal().ToString("N", culturInfo);

                    int borcluAyAdedi = taksitSayisi > 1 ? (int)(Math.Round((Math.Abs(faizliBakiyeDec) - kiraBedeliDec) / kiraBedeliDec)):aySayisi;


                    {

                        
                        if (!bolge.Equals(tempBolge))// bolge değiştiyse başlık ekle
                        {
                            if (!ilkKayit)
                            {
                                TabloyaFooterEkle(tempBolge + " Bölge Toplamı", bolgeAnaParaToplami.ToString("N", culturInfo), bolgeFaizliBakiyeToplami.ToString("N", culturInfo));
                                bolgeAnaParaToplami = 0;
                                bolgeFaizliBakiyeToplami = 0;
                            }
                            TableHeaderRow bolgeRow = new TableHeaderRow();
                            TableHeaderCell bolgeCell = new TableHeaderCell();
                            bolgeCell.ColumnSpan = BolgeColumnSpan;
                            bolgeCell.Attributes.Add("style", "text-align:center;");
                            bolgeCell.Text = bolge + " Bölgesi ";
                            bolgeCell.BackColor = System.Drawing.Color.Gray;
                            bolgeCell.ForeColor = System.Drawing.Color.White;
                            bolgeRow.Controls.Add(bolgeCell);
                            BorcluKiracilarTable.Controls.Add(bolgeRow);
                            tempBolge = bolge;

                        }
                        anaParaToplami += anaParaDecimal;
                        faizliBakiyeToplami += faizliBakiyeDec;
                        bolgeAnaParaToplami += anaParaDecimal;
                        bolgeFaizliBakiyeToplami += faizliBakiyeDec;
                        TableRow tableRow = new TableRow();
                        TableCell siraNoCell = new TableCell();
                        siraNoCell.Text = SiraNo++ + "";

                        TableCell dosyaNoCell = new TableCell();
                        dosyaNoCell.Text = dosyaNo;

                        //TableCell bolgeCell = new TableCell();
                        //bolgeCell.Text = bolge;

                        TableCell kiraciCell = new TableCell();
                        bool yetkiliMi = !string.IsNullOrEmpty(AuthQS) || AuthQS.Equals(ProjeConstants.TBYS_YETKILI_BIRIM);
                        if (yetkiliMi)
                        {
                            HyperLink kiraciLnk = new HyperLink();
                            kiraciLnk.Text = kiraci;
                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                            //string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_ODEMEPLANI + "?KiraSozlesmeId=" + kiraSozlesmeId;
                            string newUrl = UtilityHelper.URLGetir()+ "/" + ProjeConstants.PAGE_ODEMEPLANI + "?KiraSozlesmeId=" + kiraSozlesmeId;
                            kiraciLnk.NavigateUrl = newUrl;
                            kiraciCell.Controls.Add(kiraciLnk);
                        }
                        else
                        {
                            //görüntüleyenler için
                            kiraciCell.Text = "<a href=# onclick=OpenModal(" + kiraSozlesmeId + "); type=button class=\'btn btn-link font-weight-bold\'>" + kiraci + "</a>";
                        }
                            

                        TableCell ilkSozTarCell = new TableCell();
                        ilkSozTarCell.Text = ilkSozlesmeTar;

                        TableCell kiraBedeliCell = new TableCell();
                        kiraBedeliCell.Text = kiraBedeli;
                        kiraBedeliCell.CssClass = "text-right";

                        TableCell anaParaCell = new TableCell();
                        anaParaCell.Text = anaPara;
                        anaParaCell.CssClass = "text-right";

                        TableCell faizliBakiyeCell = new TableCell();
                        faizliBakiyeCell.Text = faizliBakiye;
                        faizliBakiyeCell.CssClass = "text-right";

                        TableCell kiraBorcuCell = new TableCell();
                        kiraBorcuCell.Text = (borcluAyAdedi).ToString();
                        kiraBorcuCell.CssClass = "text-right";

                        siraNoCell.BorderStyle = BorderStyle.Solid;
                        dosyaNoCell.BorderStyle = BorderStyle.Solid;
                        kiraciCell.BorderStyle = BorderStyle.Solid;
                        ilkSozTarCell.BorderStyle = BorderStyle.Solid;
                        kiraBedeliCell.BorderStyle = BorderStyle.Solid;
                        anaParaCell.BorderStyle = BorderStyle.Solid;
                        faizliBakiyeCell.BorderStyle = BorderStyle.Solid;
                        kiraBorcuCell.BorderStyle = BorderStyle.Solid;

                        tableRow.Controls.Add(siraNoCell);
                        tableRow.Controls.Add(dosyaNoCell);
                        //tableRow.Controls.Add(bolgeCell);
                        tableRow.Controls.Add(kiraciCell);
                        tableRow.Controls.Add(ilkSozTarCell);
                        tableRow.Controls.Add(kiraBedeliCell);
                        tableRow.Controls.Add(anaParaCell);
                        tableRow.Controls.Add(faizliBakiyeCell);
                        tableRow.Controls.Add(kiraBorcuCell);

                        BorcluKiracilarTable.Controls.Add(tableRow);
                        bool sonKayit = ++counter == toplamKayitSayisi;
                        if (sonKayit)
                        {
                            TabloyaFooterEkle(tempBolge + " Bölge Toplamı", bolgeAnaParaToplami.ToString("N", culturInfo), bolgeFaizliBakiyeToplami.ToString("N", culturInfo));
                        }
                        ilkKayit = false;
                    }

                }
                TabloyaFooterEkle("Genel Toplam", anaParaToplami.ToString("N", culturInfo), faizliBakiyeToplami.ToString("N", culturInfo));
            }
            else
            {
                MessageHelper.PublishMessage("Borçlu Kiracı bulunamadı", ProjeConstants.MESAJ_BILGI, 2000);
            }

        }

        private void TabloyaFooterEkle(string title, string anaParaToplami, string faizliBakiyeToplami)
        {
            TableFooterRow toplamRow = new TableFooterRow();
            TableHeaderCell toplamCell = new TableHeaderCell();
            toplamCell.Text = string.IsNullOrEmpty(title) ? "TOPLAM" : title;
            toplamCell.ColumnSpan = 5;
            toplamCell.Attributes.Add("style", "text-align:right;");
            toplamRow.Controls.Add(toplamCell);
            TableHeaderCell borcMikCell = new TableHeaderCell();
            borcMikCell.Text = anaParaToplami;
            borcMikCell.Attributes.Add("style", "text-align:right;");
            TableHeaderCell bakiyeCell = new TableHeaderCell();
            bakiyeCell.Text = faizliBakiyeToplami;
            bakiyeCell.Attributes.Add("style", "text-align:right;");
            TableHeaderCell bosCell = new TableHeaderCell();

            toplamRow.Controls.Add(borcMikCell);
            toplamRow.Controls.Add(bakiyeCell);
            toplamRow.Controls.Add(bosCell);
            BorcluKiracilarTable.Controls.Add(toplamRow);
        }

        private void BorcluKiracilarTableHeaderRows()
        {
            BorcluKiracilarTable.Controls.Clear();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            TableHeaderRow tableTitleRow = new TableHeaderRow();
            TableHeaderCell tableTitleCell = new TableHeaderCell();

            // int sureAy = string.IsNullOrEmpty(AySayisiBitQS) ? 0 : AySayisiBitQS.ConvertToInt();
            //string baslikAy = sureAy < icraAySayisi ? AySayisiBitQS + " Ay Borçlu Kiracılar" : " Hukuki İşleme Tabi Kiracılar";

            tableTitleCell.Text = (new DateTime(SecilenYilQS.ConvertToInt(), SecilenAyQS.ConvertToInt(), 1)).ToString("MMMM yyyy", culturInfo) + " İtibarı İle Borçlu Kiracılar ";

            tableTitleCell.ColumnSpan = BolgeColumnSpan;
            tableTitleRow.Controls.Add(tableTitleCell);
            tableTitleCell.Attributes.Add("style", "text-align:center;");
            BorcluKiracilarTable.Controls.Add(tableTitleRow);

            TableHeaderRow headerRow = new TableHeaderRow();

            TableHeaderCell siraNoCell = new TableHeaderCell();
            TableHeaderCell dosyaNoCell = new TableHeaderCell();
            TableHeaderCell kiraciCell = new TableHeaderCell();
            TableHeaderCell sozlesmeTarihiCell = new TableHeaderCell();
            TableHeaderCell kiraBedeliCell = new TableHeaderCell();
            TableHeaderCell borcMiktariCell = new TableHeaderCell();
            TableHeaderCell faizliBakiyeCell = new TableHeaderCell();
            TableHeaderCell borcAdediCell = new TableHeaderCell();

            siraNoCell.Attributes.Add("style", "text-align:center;");
            dosyaNoCell.Attributes.Add("style", "text-align:center;");
            kiraciCell.Attributes.Add("style", "text-align:center;");
            sozlesmeTarihiCell.Attributes.Add("style", "text-align:center;");
            kiraBedeliCell.Attributes.Add("style", "text-align:center;");
            borcMiktariCell.Attributes.Add("style", "text-align:center;");
            faizliBakiyeCell.Attributes.Add("style", "text-align:center;");
            borcAdediCell.Attributes.Add("style", "text-align:center;");

            siraNoCell.Text = "S.No";
            dosyaNoCell.Text = "D.No";
            kiraciCell.Text = "Kiracının Adı ve Soyadı";
            sozlesmeTarihiCell.Text = "İlk Sözleşme Tarihi";
            kiraBedeliCell.Text = "Kira Bedeli (TL/Ay)";
            borcMiktariCell.Text = "Borç Miktarı (TL)";
            faizliBakiyeCell.Text = "Faizli Bakiye (TL)";
            borcAdediCell.Text = "Kira Borcu (Ay)";

            headerRow.Controls.Add(siraNoCell);
            headerRow.Controls.Add(dosyaNoCell);
            headerRow.Controls.Add(kiraciCell);
            headerRow.Controls.Add(sozlesmeTarihiCell);
            headerRow.Controls.Add(kiraBedeliCell);
            headerRow.Controls.Add(borcMiktariCell);
            headerRow.Controls.Add(faizliBakiyeCell);
            headerRow.Controls.Add(borcAdediCell);

            BorcluKiracilarTable.Controls.Add(headerRow);
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }

        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            BorcluKiraclariTabloyaDoldur();
            string filename = "BorcluKiracilar" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_" + new Random().Next() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            BorcluKiracilarTable.RenderControl(hw);
            //Write the HTML back to the browser.
            //Response.ContentType = application/vnd.ms-excel;
            Page.Response.ContentType = "application/ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            string s = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\n<head>\n<title></title>\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1254\" />\n<style>\n</style>\n</head>\n<body>\n";
            Page.Response.Write(s + tw.ToString());
            Page.Response.End();

        }
        protected void OdemePlanlariniGuncelleBtn_Click(object sender, EventArgs e)
        {
            TBYSOrtak.AktifSozleslemelerinBakiyeBorcunuHesapla();
        }
        protected void OdemePlaniGoruntuleBtn_Click(object sender, EventArgs e)
        {
            int kiraSozlesmeId=paramKiraSozlesmeIdLbl.Value.ConvertToInt();
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.Select(kiraSozlesmeId);
                if (kiraSozlesme != null) //bu sozlesme varsa
                {
                    OdemePlani odemePlani = new OdemePlani();
                    bool odemePlaniVarMi = odemePlani.OdemePlaniVarMi(kiraSozlesme.Id);
                    if (odemePlaniVarMi)
                    {
                        OdemePlaniGoruntule(kiraSozlesme);
                    }
                }
            
        }
        private void OdemePlaniGoruntule(KiraSozlesme kiraSozlesme)
        {
            

            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select(kiraSozlesme.KiraciId);
            if (kiraci != null)
            {
                KiraciTitleLbl.Text = kiraci.Adi + " " + kiraci.Soyadi;
            }
            
            OdemePlani odemePlaniDao = new OdemePlani();
            List<OdemePlani> list = odemePlaniDao.SelectBySozlesmeId(kiraSozlesme.Id);
            if (list.Count > 0)
            {
                DevirLbl.Text = "Devir Anapara : " + kiraSozlesme.DevirAnaPara.ToString("N", culturInfo) + "      "
                    + "Devir Faiz : " + kiraSozlesme.DevirFaizTutari.ToString("N", culturInfo)
                    + "Devir FaizliBakiye : " + kiraSozlesme.DevirFaizliBakiye.ToString("N", culturInfo);
            }
            foreach (OdemePlani odemePlani in list)
            {
                if (odemePlani.Sira == 0)
                {
                    continue;
                }
                TableRow row = new TableRow();
                TableCell SiraNoCell = new TableCell();
                SiraNoCell.Text = odemePlani.Sira.ToString(); ;
                row.Controls.Add(SiraNoCell);

                TableCell YilCell = new TableCell();
                YilCell.CssClass = "text-right";
                YilCell.Text = odemePlani.Yil.ToString();
                row.Controls.Add(YilCell);

                TableCell AyCell = new TableCell();
                AyCell.Text = odemePlani.Ay.ToString();
                row.Controls.Add(AyCell);

                TableCell KiraBedeliCell = new TableCell();
                KiraBedeliCell.CssClass = "text-right";
                KiraBedeliCell.Text = odemePlani.KiraBedeli.ToString("N", culturInfo);
                row.Controls.Add(KiraBedeliCell);

                TableCell OdenenTutarCell = new TableCell();
                OdenenTutarCell.CssClass = "text-right";
                OdenenTutarCell.Text = odemePlani.OdenenTutar.ToString("N", culturInfo);
                row.Controls.Add(OdenenTutarCell);
                
                TableCell OdemeTarihiCell = new TableCell();
                OdemeTarihiCell.CssClass = "text-right";
                OdemeTarihiCell.Text = OdemeGetir(kiraSozlesme,odemePlani);
                row.Controls.Add(OdemeTarihiCell);

                OdemePlaniTable.Controls.Add(row);
            }
            var jsString = " $('#OdemePlaniModal').modal({ backdrop: false });";
            UtilityHelper.ScriptCalistir(jsString);
        }

        private string OdemeGetir(KiraSozlesme kiraSozlesme, OdemePlani odemePlani)
        {
            Odeme odeme = new Odeme();
            List<Odeme> odemeler = odeme.SelectBySozlesmeIdOdemePlaniId(kiraSozlesme.Id, odemePlani.Id) ;
            string sonuc=string.Empty;
            foreach (Odeme item in odemeler)
            {
                sonuc+=item.OdemeTarihi.ConvertToDatetimeEmptyIfNull() +  ((odemeler.Count > 1 ? " (" + item.OdenenTutar.ToString("N", culturInfo) + ")</br>" : String.Empty) );
            }
            return sonuc;
        }
    }
}


