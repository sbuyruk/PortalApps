using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;


namespace TBYS_WebParts.KiraKartiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraKartiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraKartiWP()
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
        private string KiraciIdQS
        {
            get
            {

                if (ViewState["KiraciId"] == null)
                {
                    if (Page.Request.QueryString["KiraciId"] != null)
                    {
                        ViewState["KiraciId"] = Page.Request.QueryString["KiraciId"];
                    }
                    else
                    {
                        ViewState["KiraciId"] = string.Empty;
                    }
                }
                return ViewState["KiraciId"].ToString();
            }

            set
            {
                ViewState["KiraciId"] = value;
            }
        }
        private string KiraSozlesmeIdQS
        {
            get
            {

                if (ViewState["KiraSozlesmeId"] == null)
                {
                    if (Page.Request.QueryString["KiraSozlesmeId"] != null)
                    {
                        ViewState["KiraSozlesmeId"] = Page.Request.QueryString["KiraSozlesmeId"];
                    }
                    else
                    {
                        ViewState["KiraSozlesmeId"] = string.Empty;
                    }
                }
                return ViewState["KiraSozlesmeId"].ToString();
            }

            set
            {
                ViewState["KiraSozlesmeId"] = value;
            }
        }
        private string SelectAllQS
        {
            get
            {
                if (ViewState["SelectAll"] == null)
                {
                    if (Page.Request.QueryString["SelectAll"] != null)
                    {
                        ViewState["SelectAll"] = Page.Request.QueryString["SelectAll"];
                    }
                    else
                    {
                        ViewState["SelectAll"] = "false";
                    }
                }
                return ViewState["SelectAll"].ToString();
            }

            set
            {
                ViewState["SelectAll"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
                if (kiraci != null)
                {
                    KiraSozlesme kiraSozlesme = new KiraSozlesme();
                    kiraSozlesme = kiraSozlesme.SelectSozlesmeByKiraciId(kiraci.Id);

                    if (kiraSozlesme != null)
                    {
                        KartNoHdrCell.Text = "KART NO : " +kiraSozlesme.DosyaNo.ToString();
                        TumunuSecChk.Checked = SelectAllQS.ConvertToBool();
                        KiraSozlesmeIdQS = kiraSozlesme.Id.ToString();
                        //KartNoLbl.Text = "Dosya No : " + kiraSozlesme.DosyaNo;
                        KiraciBilgileriniDoldur(kiraci);
                        OdemePlaniDoldur();
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Kira Sözleşmesi Bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                }
                else
                {
                    MessageHelper.PublishMessage("Kiracı Bulunamadı", ProjeConstants.MESAJ_HATA);
                }
            }


        }

        private void KiraciBilgileriniDoldur(Kiraci kiraci)
        {


            if (kiraci != null)
            {
                KiraciBilgileriniTabloyaYaz(kiraci);

                KiraSozlesme aktifSozlesme = new KiraSozlesme();
                aktifSozlesme = aktifSozlesme.SelectAktifSozlesmeByKiraciId(kiraci.Id);
                if (aktifSozlesme != null)
                {
                    KiraSozlesmeIdQS = aktifSozlesme.Id.ToString();
                    SozlesmeTarCell.Text = aktifSozlesme.IlkSozlesmeTar.ConvertToDatetimeEmptyIfNull();
                    TasinmazBilgileriniTabloyaYaz(aktifSozlesme);
                    IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
                    TeminatTutariCell.Text = aktifSozlesme.TeminatTutari.ConvertToDecimal().ToString("N", culturInfo);
                    OdenenTeminatTutariCell.Text = aktifSozlesme.OdenenTeminatTutari.ConvertToDecimal().ToString("N", culturInfo);
                    string iade = aktifSozlesme.IadeTeminatTutari>0?" (Iade/Mahsup Tutari : " + aktifSozlesme.IadeTeminatTutari.ConvertToDecimal().ToString("N", culturInfo)+")":string.Empty;
                    KalanTeminatTutariCell.Text = aktifSozlesme.KalanTeminatTutari.ConvertToDecimal().ToString("N", culturInfo) + iade;
                    //KalanTeminatTutariCell.Text = aktifSozlesme.KalanTeminatTutari.ConvertToDecimal().ToString("N", culturInfo);
                    //TeminatIadeTarihiCell.Text = aktifSozlesme.TeminatIadeTarihi.ConvertToDatetimeEmptyIfNull();
                    TeminatTarihiCell.Text = aktifSozlesme.TeminatOdemeTarihi.ConvertToDatetimeEmptyIfNull();
                    KiraTeminatiCell.Text = "TEMINAT (" + aktifSozlesme.TeminatCinsi + ")";

                }
            }
            else
            {
                MessageHelper.PublishMessage("Kiraci Bulunamadi", ProjeConstants.MESAJ_BILGI);
            }
        }
        private void TasinmazBilgileriniTabloyaYaz(KiraSozlesme ks)
        {
            SozlesmeTasinmaz st = new SozlesmeTasinmaz();
            List<SozlesmeTasinmaz> stList = st.SelectBySozlesmeId(ks.Id);
            foreach (SozlesmeTasinmaz item in stList)
            {
                string adres = string.Empty;
                string ili = string.Empty;
                string ilcesi = string.Empty;
                int tasinmazId = item.TasinmazId;
                Tasinmaz tasinmaz = new Tasinmaz();
                tasinmaz = tasinmaz.Select(tasinmazId);
                if (tasinmaz != null)
                {
                    ili = tasinmaz.Ili;
                    ilcesi = tasinmaz.Ilcesi;
                    if (item.BolumId > 0)
                    {
                        BagimsizBolum bagimsizBolum = new BagimsizBolum();
                        bagimsizBolum = bagimsizBolum.Select<BagimsizBolum>(item.BolumId);
                        if (bagimsizBolum == null)
                        {
                            MessageHelper.PublishMessage("Bağımsız bölüm bulunamadı. Sözleşmeden bağımsız bölüm kaydını düzeltmeniz gerekmektedir.",ProjeConstants.MESAJ_HATA);
                        }
                        else
                        {
                            adres += tasinmaz.Adres + bagimsizBolum.BolumNo + System.Environment.NewLine;
                        }
                    }
                    else
                    {
                        adres += tasinmaz.Adres + System.Environment.NewLine;
                    }
                    YuzolcumuCell.Text = tasinmaz.Yuzolcumu;
                    //KiralamaAmaciCell.Text = tasinmaz.KullanimSekli;
                    if (tasinmaz.AltBolum)
                    {
                        BagimsizBolum bagimsizBolum = new BagimsizBolum();
                        bagimsizBolum = bagimsizBolum.Select<BagimsizBolum>(item.BolumId);
                        if (bagimsizBolum != null)
                        {
                            NiteligiCell.Text = bagimsizBolum.Nitelik;
                        }
                    }
                    else
                    {
                        NiteligiCell.Text = tasinmaz.Nitelik;
                    }
                }
                IliCell.Text = ili;
                IlcesiCell.Text = ilcesi;
                AdresCell.Text = adres;
            }

        }
        private void KiraciBilgileriniTabloyaYaz(Kiraci kiraci)
        {
            AdiCell.Text = kiraci.Adi + " " + kiraci.Soyadi;
            TelNoCell.Text = kiraci.Telefon;
            TCKimlikNoCell.Text = kiraci.TCKimlikNo;
            SemtCell.Text = kiraci.Semt;
            VergiDairesiCell.Text = kiraci.VergiDairesi;
            VergiNoCell.Text = kiraci.VergiNo;
            KiralamaAmaciCell.Text = kiraci.KiralamaAmaci;
        }

        private void OdemePlaniDoldur()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            KiraSozlesme kiraSozlesmeDao = new KiraSozlesme();

            var kiraSozlesmeList = kiraSozlesmeDao.SelectByKiraciIdReturnList(KiraciIdQS.ConvertToInt());
            Color color = System.Drawing.Color.White;

            int tempKirasozlesmeId = 0;
            foreach (var kiraSozlesme in kiraSozlesmeList)
            {
                if (kiraSozlesme != null)
                {
                    OdemePlani odemePlani = new OdemePlani();
                    List<OdemePlani> list = odemePlani.SelectBySozlesmeId(kiraSozlesme.Id);
                    List<OdemePlani> sortedOPList = list.OrderByDescending(x => x.Sira).ToList();
                    DateTime buAySonu = (new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)).AddMonths(1).AddDays(-1);
                    foreach (OdemePlani op in sortedOPList)
                    {
                        //if (op.VadeBitTar > buAySonu)
                        //{
                        //    continue;
                        //}
                        if (tempKirasozlesmeId != kiraSozlesme.Id)
                        {
                            BaslikEkle();
                            tempKirasozlesmeId = kiraSozlesme.Id;
                        }

                        TableRow row = new TableRow();

                        TableCell VadeBasTarCell = new TableCell();
                        string vadeBastar = op.VadeBasTar == DateTime.MinValue ? "" : op.VadeBasTar.ConvertToDatetimeEmptyIfNull();
                        string vadeBittar = op.VadeBasTar == DateTime.MinValue ? "" : op.VadeBasTar.AddMonths(1).AddDays(-1).ConvertToDatetimeEmptyIfNull();

                        if (kiraSozlesme.OdemeSekli.Equals(ProjeConstants.KIRA_ODMSEKLI_YILLIK))
                        {
                            vadeBastar = op.VadeBasTar == DateTime.MinValue ? "" : op.VadeBasTar.AddYears(-1).ToString("dd.MMMM.yyyy", culturInfo);///vadeBastar + " - " + vadeBittar;
                        }

                        if (op.Sira == 0)
                        {
                            VadeBasTarCell.Text = "Devir";
                            color = Color.Gray;
                        }
                        else
                        {
                            VadeBasTarCell.Text = op.VadeBitTar.ToString("dd MMMM yyyy", culturInfo);
                            color = System.Drawing.Color.White;
                        }

                        VadeBasTarCell.BorderStyle = BorderStyle.Solid;
                        VadeBasTarCell.BorderWidth = 1;
                        VadeBasTarCell.BackColor = color;
                        row.Controls.Add(VadeBasTarCell);

                        TableCell KiraBedeliCell = new TableCell();
                        KiraBedeliCell.Text = op.KiraBedeli.ToString("N", culturInfo);
                        KiraBedeliCell.HorizontalAlign = HorizontalAlign.Right;
                        KiraBedeliCell.BorderStyle = BorderStyle.Solid;
                        KiraBedeliCell.BorderWidth = 1;
                        KiraBedeliCell.BackColor = color;
                        row.Controls.Add(KiraBedeliCell);
                        string odenenler = string.Empty;
                        string aciklama = string.Empty;
                        if (op.OdenenTutar > 0)
                        {
                            Odeme odemeDao = new Odeme();
                            List<Odeme> odemeList = odemeDao.SelectBySozlesmeIdOdemePlaniId(op.SozlesmeId, op.Id);
                            foreach (Odeme item in odemeList)
                            {
                                odenenler += item.OdemeTarihi.ReturnTRDateFormat() + " (" + item.OdenenTutar.ToString("N", culturInfo) + ")" + System.Environment.NewLine;
                                aciklama += item.Aciklama + System.Environment.NewLine;
                            }
                        }
                        TableCell OdemeTarCell = new TableCell();
                        OdemeTarCell.Text = odenenler;
                        OdemeTarCell.BorderStyle = BorderStyle.Solid;
                        OdemeTarCell.BorderWidth = 1;
                        OdemeTarCell.BackColor = color;
                        row.Controls.Add(OdemeTarCell);

                        TableCell OdenenCell = new TableCell();
                        OdenenCell.Text = op.OdenenTutar.ToString("N", culturInfo);
                        OdenenCell.HorizontalAlign = HorizontalAlign.Right;
                        OdenenCell.BorderStyle = BorderStyle.Solid;
                        OdenenCell.BorderWidth = 1;
                        OdenenCell.BackColor = color;
                        row.Controls.Add(OdenenCell);

                        TableCell BakiyeCell = new TableCell();
                        BakiyeCell.Text = (op.AnaPara*-1).ToString("N", culturInfo);
                        BakiyeCell.HorizontalAlign = HorizontalAlign.Right;
                        BakiyeCell.BorderStyle = BorderStyle.Solid;
                        BakiyeCell.BorderWidth = 1;
                        BakiyeCell.BackColor = color;
                        row.Controls.Add(BakiyeCell);

                        TableCell GecikmeFaiziCell = new TableCell();
                        GecikmeFaiziCell.Text = (op.FaizTutari*-1).ToString("N", culturInfo);
                        GecikmeFaiziCell.HorizontalAlign = HorizontalAlign.Right;
                        GecikmeFaiziCell.BorderStyle = BorderStyle.Solid;
                        GecikmeFaiziCell.BorderWidth = 1;
                        GecikmeFaiziCell.BackColor = color;
                        row.Controls.Add(GecikmeFaiziCell);

                        TableCell FaizliBakiyeCell = new TableCell();
                        FaizliBakiyeCell.Text = (op.FaizliBakiye*-1).ToString("N", culturInfo);
                        FaizliBakiyeCell.HorizontalAlign = HorizontalAlign.Right;
                        FaizliBakiyeCell.BorderStyle = BorderStyle.Solid;
                        FaizliBakiyeCell.BorderWidth = 1;
                        FaizliBakiyeCell.BackColor = color;
                        row.Controls.Add(FaizliBakiyeCell);

                        TableCell AciklamaCell = new TableCell();
                        AciklamaCell.Text = aciklama;
                        AciklamaCell.BorderStyle = BorderStyle.Solid;
                        AciklamaCell.BorderWidth = 1;
                        AciklamaCell.BackColor = color;
                        row.Controls.Add(AciklamaCell);

                        KiraTable.Controls.Add(row);
                    }
                }
                if (!SelectAllQS.ConvertToBool())
                    break;
            }
        }

        private void BaslikEkle()
        {
            TableHeaderRow baslikRow = new TableHeaderRow();
            TableHeaderCell donemCell = new TableHeaderCell();
            donemCell.Text = "DÖNEM";
            TableHeaderCell kiraTutariCell = new TableHeaderCell();
            kiraTutariCell.Text = "KIRA TUTARI";
            TableHeaderCell odenenTarCell = new TableHeaderCell();
            odenenTarCell.Text = "ÖDENEN TARIH";
            TableHeaderCell odenenTutarCell = new TableHeaderCell();
            odenenTutarCell.Text = "ÖDENEN TUTAR";
            TableHeaderCell kalanAnaParaCell = new TableHeaderCell();
            kalanAnaParaCell.Text = "KALAN ANAPARA";
            TableHeaderCell gecikmeFaiziCell = new TableHeaderCell();
            gecikmeFaiziCell.Text = "GECIKME FAIZI";
            TableHeaderCell faizliBakiyeCell = new TableHeaderCell();
            faizliBakiyeCell.Text = "FAIZLI BAKIYE";
            TableHeaderCell aciklamaCell = new TableHeaderCell();
            aciklamaCell.Text = "AÇIKLAMA";

            donemCell.Width = new Unit("10%");
            kiraTutariCell.Width = new Unit("10%");
            odenenTarCell.Width = new Unit("15%");
            odenenTutarCell.Width = new Unit("10%");
            kalanAnaParaCell.Width = new Unit("10%");
            gecikmeFaiziCell.Width = new Unit("10%");
            faizliBakiyeCell.Width = new Unit("10%");
            aciklamaCell.Width = new Unit("25%");

            donemCell.BorderStyle = BorderStyle.Solid;
            donemCell.BorderWidth = 1;
            kiraTutariCell.BorderStyle = BorderStyle.Solid;
            kiraTutariCell.BorderWidth = 1;
            odenenTarCell.BorderStyle = BorderStyle.Solid;
            odenenTarCell.BorderWidth = 1;
            odenenTutarCell.BorderStyle = BorderStyle.Solid;
            odenenTutarCell.BorderWidth = 1;
            kalanAnaParaCell.BorderStyle = BorderStyle.Solid;
            kalanAnaParaCell.BorderWidth = 1;
            gecikmeFaiziCell.BorderStyle = BorderStyle.Solid;
            gecikmeFaiziCell.BorderWidth = 1;
            faizliBakiyeCell.BorderStyle = BorderStyle.Solid;
            faizliBakiyeCell.BorderWidth = 1;
            aciklamaCell.BorderStyle = BorderStyle.Solid;
            aciklamaCell.BorderWidth = 1;


            baslikRow.Controls.Add(donemCell);
            baslikRow.Controls.Add(kiraTutariCell);
            baslikRow.Controls.Add(odenenTarCell);
            baslikRow.Controls.Add(odenenTutarCell);
            baslikRow.Controls.Add(kalanAnaParaCell);
            baslikRow.Controls.Add(gecikmeFaiziCell);

            baslikRow.Controls.Add(faizliBakiyeCell);
            baslikRow.Controls.Add(aciklamaCell);

            KiraTable.Controls.Add(baslikRow);
        }

        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            OdemePlaniDoldur();
            string filename = "KiraKarti_" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_" + new Random().Next() + ".xls";

            Page.Response.Clear();
            Page.Response.AddHeader("content-disposition", "attachment;filename=" + filename + "");
            Page.Response.ContentType = "application/ms-excel";
            Page.Response.ContentEncoding = System.Text.Encoding.Unicode;
            Page.Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new HtmlTextWriter(sw);

            KiraTable.RenderControl(hw);

            Page.Response.Write(sw.ToString());
            Page.Response.End();
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void KiraciListesiBtn_Click(object sender, EventArgs e)
        {
            try
            {
                RedirectToPage(ProjeConstants.PAGE_KIRACI_LIST + "?SecilenId=" + KiraciIdQS);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void TumunuSecChk_CheckedChanged(object sender, EventArgs e)
        {
            SelectAllQS = TumunuSecChk.Checked.ToString();
            RedirectToPage(ProjeConstants.PAGE_KIRAKARTI + "?SenderApp=" + SenderAppQS + "&KiraciId=" + KiraciIdQS + "&SelectAll=" + SelectAllQS);
        }
    }
}
