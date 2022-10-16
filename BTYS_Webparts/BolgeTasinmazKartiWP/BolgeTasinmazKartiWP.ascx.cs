using Microsoft.SharePoint;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;


namespace BTYS_Webparts.BolgeTasinmazKartiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BolgeTasinmazKartiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BolgeTasinmazKartiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string TasinmazIdQS
        {
            get
            {

                if (ViewState["TasinmazId"] == null)
                {
                    if (Page.Request.QueryString["TasinmazId"] != null)
                    {
                        ViewState["TasinmazId"] = Page.Request.QueryString["TasinmazId"];
                    }
                    else
                    {
                        ViewState["TasinmazId"] = string.Empty;
                    }
                }
                return ViewState["TasinmazId"].ToString();
            }

            set
            {
                ViewState["TasinmazId"] = value;
            }
        }
        private string BolgeQS
        {
            get
            {

                if (ViewState["Bolge"] == null)
                {
                    if (Page.Request.QueryString["Bolge"] != null)
                    {
                        ViewState["Bolge"] = Page.Request.QueryString["Bolge"];
                    }
                    else
                    {
                        ViewState["Bolge"] = string.Empty;
                    }
                }
                return ViewState["Bolge"].ToString();
            }

            set
            {
                ViewState["Bolge"] = value;
            }
        }
        private string CurrentUserName
        {
            get
            {

                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUser();
                }
                return ViewState["CurrentUserName"].ToString();
            }

            set
            {
                ViewState["CurrentUserName"] = value;
            }
        }
        private string EnvanterdeMiQS
        {
            get
            {

                if (ViewState["EnvanterdeMi"] == null)
                {
                    if (Page.Request.QueryString["EnvanterdeMi"] != null)
                    {
                        ViewState["EnvanterdeMi"] = Page.Request.QueryString["EnvanterdeMi"];
                    }
                    else
                    {
                        ViewState["EnvanterdeMi"] = string.Empty;
                    }
                }
                return ViewState["EnvanterdeMi"].ToString();
            }

            set
            {
                ViewState["EnvanterdeMi"] = value;
            }
        }
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    BolgeQS = IKYSOrtak.PersonelinBolgesiniGetir(CurrentUserName);
                    if (!string.IsNullOrEmpty(BolgeQS))
                    {
                        Tasinmaz tasinmaz = new Tasinmaz();
                        tasinmaz = tasinmaz.Select<Tasinmaz>(TasinmazIdQS.ConvertToInt());
                        if (tasinmaz != null)
                        {
                            TumTablolariDoldur(tasinmaz);
                        }
                        else
                        {
                            MessageHelper.PublishMessage("Taşınmaz Bulunamadı", ProjeConstants.MESAJ_HATA);
                        } 
                    }
                    else
                        MessageHelper.PublishMessage("Bölgeniz Belirlenemedi", ProjeConstants.MESAJ_HATA);


                }

            }
            catch (Exception exception)
            {
                ExceptionHelper exHelper = new ExceptionHelper(exception);
                exHelper.PublishException();
            }
        }

        private void TumTablolariDoldur(Tasinmaz tasinmaz)
        {
            TasinmazBilgileriniDoldur(tasinmaz);
            Bagis bagis = new Bagis();
            bagis = bagis.SelectByTasinmazId(tasinmaz.Id);
            if (bagis != null)
            {
                TasinmazBagisci bagisci = new TasinmazBagisci();
                bagisci = bagisci.Select<TasinmazBagisci>(bagis.BagisciId);
                if (bagisci != null)
                {
                    BagisciBilgileriniDoldur(bagisci);
                }


            }
            TasinmazResimleriniDoldur(tasinmaz);
            KiraciBilgileriniDoldur(tasinmaz.Id);
            OnarimBilgileriniDoldur(tasinmaz.Id);
        }

        protected void ExportToExcel()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select<Tasinmaz>(TasinmazIdQS.ConvertToInt());
            string filename = "TasinmazKartiNo" + tasinmaz.Id.ReplaceTrChars() + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            MainCardDiv.RenderControl(hw);
            //Write the HTML back to the browser.
            //Response.ContentType = application/vnd.ms-excel;
            Page.Response.ContentType = "application/ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            string s = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\n<head>\n<title></title>\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1254\" />\n<style>\n</style>\n</head>\n<body>\n";
            Page.Response.Write(s + tw.ToString());
            Page.Response.End();

        }
        private void TasinmazBilgileriniDoldur(Tasinmaz tasinmaz)
        {
            AdiLbl.Text = tasinmaz.Id.ToString();

            TasinmazBilgileriTable.Rows.Clear();
            TasinmazBilgileriTable.BorderWidth = 2;
            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.BackColor = System.Drawing.Color.LightGray; ;
            TableHeaderCell TabloBaslikCell = new TableHeaderCell();
            TabloBaslikCell.ColumnSpan = 4;
            TabloBaslikCell.Text = "Taşınmaz Bilgileri";
            headerRow.Controls.Add(TabloBaslikCell);

            TableHeaderRow headerRow1 = new TableHeaderRow();
            headerRow1.CssClass = "font-weight-bold";
            TableHeaderCell TabloBaslikCell1 = new TableHeaderCell();
            TabloBaslikCell1.ColumnSpan = 4;
            TabloBaslikCell1.Text = "(" + tasinmaz.KullanimSekli + ") " + tasinmaz.Adres + " " + tasinmaz.Ili + " " + tasinmaz.Ilcesi;
            headerRow1.Controls.Add(TabloBaslikCell1);
            TasinmazBilgileriTable.Controls.Add(headerRow);
            TasinmazBilgileriTable.Controls.Add(headerRow1);
            TableRow row1 = new TableRow();
            TableRow row2 = new TableRow();
            TableRow row3 = new TableRow();
            TableRow row4 = new TableRow();
            TableRow row5 = new TableRow();

            TableCell r1c1 = new TableCell();
            TableCell r1c2 = new TableCell();
            TableCell r1c3 = new TableCell();
            TableCell r1c4 = new TableCell();

            row1.Controls.Add(r1c1);
            row1.Controls.Add(r1c2);
            row1.Controls.Add(r1c3);
            row1.Controls.Add(r1c4);
            TableCell r2c1 = new TableCell();
            TableCell r2c2 = new TableCell();
            TableCell r2c3 = new TableCell();
            TableCell r2c4 = new TableCell();
            row2.Controls.Add(r2c1);
            row2.Controls.Add(r2c2);
            row2.Controls.Add(r2c3);
            row2.Controls.Add(r2c4);
            TableCell r3c1 = new TableCell();
            TableCell r3c2 = new TableCell();
            TableCell r3c3 = new TableCell();
            TableCell r3c4 = new TableCell();
            row3.Controls.Add(r3c1);
            row3.Controls.Add(r3c2);
            row3.Controls.Add(r3c3);
            row3.Controls.Add(r3c4);
            TableCell r4c1 = new TableCell();
            TableCell r4c2 = new TableCell();
            TableCell r4c3 = new TableCell();
            TableCell r4c4 = new TableCell();
            row4.Controls.Add(r4c1);
            row4.Controls.Add(r4c2);
            row4.Controls.Add(r4c3);
            row4.Controls.Add(r4c4);
            TableCell r5c1 = new TableCell();
            TableCell r5c2 = new TableCell();
            TableCell r5c3 = new TableCell();
            TableCell r5c4 = new TableCell();
            row5.Controls.Add(r5c1);
            row5.Controls.Add(r5c2);
            row5.Controls.Add(r5c3);
            row5.Controls.Add(r5c4);

            r1c1.Text = "Bölge : " + tasinmaz.SorumluBolge;
            r2c1.Text = "Mülkiyet Şekli : " + tasinmaz.MulkiyetSekli;
            r3c1.Text = "" + tasinmaz.KullanimDurumu;
            r4c1.Text = "Sigorta : " + tasinmaz.SigortaDurumu;
            r5c1.Text = "Kat Mülkiyeti : " + tasinmaz.KatMulkiyeti;

            r1c2.Text = "Env.Gir.Tar. : " + tasinmaz.EnvantereGirisTarihi.ConvertToDatetimeEmptyIfNull();
            r2c2.Text = "Bağış Yılı : " + tasinmaz.BagisYili;
            r3c2.Text = "Eml.Sic.No : " + tasinmaz.EmlakSicilNo;
            r4c2.Text = "Emlak Bey.Değ. : " + tasinmaz.EmlakBeyanDegeri.ReturnZeroIfNull().ConvertToDecimal().ToString("N", culturInfo);
            r5c2.Text = "T.Rayiç Değ. : " + tasinmaz.TahminiRayicDegeri.ReturnZeroIfNull().ConvertToDecimal().ToString("N", culturInfo);

            r1c3.Text = "Tapu Tarihi : " + tasinmaz.TapuTarihi.ConvertToDatetimeEmptyIfNull();
            r2c3.Text = "Ada No : " + tasinmaz.AdaNo;
            r3c3.Text = "Parsel No : " + tasinmaz.ParselNo;
            r4c3.Text = "Pafta No : " + tasinmaz.PaftaNo;
            r5c3.Text = "Yüzölçümü : " + tasinmaz.Yuzolcumu;

            r1c4.Text = "Arsa Payı : " + tasinmaz.ArsaPayi;
            r2c4.Text = "Vakıf Hissesi : ".PadRight(15, '-') + tasinmaz.VakifHissesi;
            r3c4.Text = "Yevmiye No : " + tasinmaz.YevmiyeNo;
            r4c4.Text = "Cilt No : " + tasinmaz.CiltNo;
            r5c4.Text = "Sahife No : " + tasinmaz.SahifeNo;

            BorderEkle(row1);
            BorderEkle(row2);
            BorderEkle(row3);
            BorderEkle(row4);
            BorderEkle(row5);

            r1c1.Width = 260;
            r1c2.Width = 260;
            r1c3.Width = 260;
            r1c4.Width = 260;

            TasinmazBilgileriTable.Controls.Add(row1);
            TasinmazBilgileriTable.Controls.Add(row2);
            TasinmazBilgileriTable.Controls.Add(row3);
            TasinmazBilgileriTable.Controls.Add(row4);
            TasinmazBilgileriTable.Controls.Add(row5);
        }
        private void BorderEkle(TableRow row)
        {
            for (int n = 0; n < row.Cells.Count; n++)
            {
                row.Cells[n].BorderWidth = 1;
            }
        }
        private void AlignCells(TableRow row)
        {
            for (int n = 0; n < row.Cells.Count; n++)
            {
                row.VerticalAlign = VerticalAlign.Middle;
                row.HorizontalAlign = HorizontalAlign.Center;
            }

        }
        private void BagisciBilgileriniDoldur(TasinmazBagisci bagisci)
        {
            BagisciBilgileriTable.Rows.Clear();
            BagisciBilgileriTable.BorderWidth = 2;
            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.BackColor = System.Drawing.Color.LightGray; ;
            TableHeaderCell TabloBaslikCell = new TableHeaderCell();
            TabloBaslikCell.ColumnSpan = 4;
            TabloBaslikCell.Text = "Bağışçı Bilgileri";
            headerRow.Controls.Add(TabloBaslikCell);

            TableRow row1 = new TableRow();
            TableRow row2 = new TableRow();
            TableRow row3 = new TableRow();

            TableCell r1c1 = new TableCell();
            TableCell r1c2 = new TableCell();
            TableCell r1c3 = new TableCell();
            TableCell r1c4 = new TableCell();
            r1c4.RowSpan = 3;

            string imageFileName = bagisci.Adi.ReplaceTrChars() + bagisci.Soyadi.ReplaceTrChars() + ".jpg";
            string imgUrl = UtilityHelper.TbysURLGetir()+"/"+ProjeConstants.RESIMLER_BAGISCI + "/_t/" + imageFileName;
            System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
            bool dosyaVarmi = UtilityHelper.DosyaVarMi(UtilityHelper.TbysURLGetir(),ProjeConstants.RESIMLER_BAGISCI, imageFileName);
            if (dosyaVarmi)
            {
                imageFileName = bagisci.Adi.ReplaceTrChars() + bagisci.Soyadi.ReplaceTrChars() + "_jpg.jpg";
                imgUrl = UtilityHelper.TbysBagisciResimleriURLGetir() +"/_t/"+ imageFileName;
                image.ImageUrl = imgUrl;
            }
            else
            {
                image.ImageUrl = dosyaVarmi ? imgUrl : UtilityHelper.TbysBagisciResimleriURLGetir() + "/_t/bagisci_jpg.jpg"; ;
            }


            r1c4.Controls.Add(image);
            row1.Controls.Add(r1c1);
            row1.Controls.Add(r1c2);
            row1.Controls.Add(r1c3);
            row1.Controls.Add(r1c4);
            TableCell r2c1 = new TableCell();
            TableCell r2c2 = new TableCell();
            TableCell r2c3 = new TableCell();
            TableCell r2c4 = new TableCell();
            row2.Controls.Add(r2c1);
            row2.Controls.Add(r2c2);
            row2.Controls.Add(r2c3);
            TableCell r3c1 = new TableCell();
            TableCell r3c2 = new TableCell();
            TableCell r3c3 = new TableCell();
            TableCell r3c4 = new TableCell();
            row3.Controls.Add(r3c1);
            row3.Controls.Add(r3c2);
            row3.Controls.Add(r3c3);

            r1c1.Text = bagisci.Adi + " " + bagisci.Soyadi;
            r2c1.Text = "Adresi : " + bagisci.Adres;
            r3c1.Text = "" + bagisci.Ili + " " + bagisci.Ilcesi;

            r1c2.Text = "Telefon1 : " + bagisci.Telefon1;
            r2c2.Text = "Telefon2: " + bagisci.Telefon2;
            r3c2.Text = "TC Kimlik No : " + bagisci.TCKimlikNo;

            r1c3.Text = "Sağ mı : " + bagisci.Sag_vefat;
            r2c3.Text = "Doğum Tarihi : " + bagisci.DogumTarihi.ConvertToDatetimeEmptyIfNull();
            r3c3.Text = "Doğum Yeri : " + bagisci.DogumYeri;

            BorderEkle(row1);
            BorderEkle(row2);
            BorderEkle(row3);
            r1c1.Height = 40;
            r2c1.Height = 70;
            r3c1.Height = 60;
            r1c4.Width = 100;

            BagisciBilgileriTable.Controls.Add(headerRow);
            BagisciBilgileriTable.Controls.Add(row1);
            BagisciBilgileriTable.Controls.Add(row2);
            BagisciBilgileriTable.Controls.Add(row3);

        }
        private void KiraciBilgileriniDoldur(int tasinmazId)
        {
            KiraciBilgileriTable.Rows.Clear();
            KiraciBilgileriTable.BorderWidth = 2;
            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.BackColor = System.Drawing.Color.LightGray; ;
            TableHeaderCell TabloBaslikCell = new TableHeaderCell();
            TabloBaslikCell.ColumnSpan = 4;
            TabloBaslikCell.Text = "Kiracı Bilgileri";
            headerRow.Controls.Add(TabloBaslikCell);
            TableHeaderRow headerRow1 = new TableHeaderRow();
            KiraciBilgileriTable.Controls.Add(headerRow);
            KiraciBilgileriTable.Controls.Add(headerRow1);

            SozlesmeTasinmaz st = new SozlesmeTasinmaz();
            KiraSozlesme ks = new KiraSozlesme();
            List<KiraSozlesme> kiraSozlesmeListesi = ks.SelectByTasinmazId(tasinmazId);
            if (kiraSozlesmeListesi == null)
            {
                TableHeaderCell cell1 = new TableHeaderCell();
                cell1.Text = "Kiracı bulunmamaktadır.";
                headerRow1.Controls.Add(cell1);
            }
            else
            {
                TableHeaderCell cell1 = new TableHeaderCell();
                cell1.Text = "Adi Soyadı";
                TableHeaderCell cell2 = new TableHeaderCell();
                cell2.Text = "Kira Bedeli";
                TableHeaderCell cell3 = new TableHeaderCell();
                cell3.Text = "Sözleşme Tarihi";
                TableHeaderCell cell4 = new TableHeaderCell();
                cell4.Text = "Adresi";
                headerRow1.Controls.Add(cell1);
                headerRow1.Controls.Add(cell2);
                headerRow1.Controls.Add(cell3);
                headerRow1.Controls.Add(cell4);
                foreach (KiraSozlesme item in kiraSozlesmeListesi)
                {
                    Kiraci kiraci = new Kiraci();
                    kiraci = kiraci.Select<Kiraci>(item.KiraciId);
                    TableRow row = new TableRow();
                    TableCell adiCell = new TableCell();
                    adiCell.Text = kiraci.Adi + " " + kiraci.Soyadi;
                    TableCell bedelCell = new TableCell();
                    bedelCell.Text = item.KiraBedeli.ToString("N", culturInfo);
                    TableCell tarihCell = new TableCell();
                    tarihCell.Text = item.SozBasTar.ConvertToDatetimeEmptyIfNull() + "-" + item.SozBitTar.ConvertToDatetimeEmptyIfNull();
                    TableCell adresCell = new TableCell();
                    adresCell.Text = kiraci.Adres;

                    row.Controls.Add(adiCell);
                    row.Controls.Add(bedelCell);
                    row.Controls.Add(tarihCell);
                    row.Controls.Add(adresCell);
                    BorderEkle(row);

                    KiraciBilgileriTable.Controls.Add(row);
                }
            }
        }
        private void OnarimBilgileriniDoldur(int tasinmazId)
        {
            OnarimTable.Rows.Clear();
            OnarimTable.BorderWidth = 2;
            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.BackColor = System.Drawing.Color.LightGray; ;
            TableHeaderCell TabloBaslikCell = new TableHeaderCell();
            TabloBaslikCell.ColumnSpan = 4;
            TabloBaslikCell.Text = "Onarım Bilgileri";
            headerRow.Controls.Add(TabloBaslikCell);
            TableHeaderRow headerRow1 = new TableHeaderRow();
            OnarimTable.Controls.Add(headerRow);
            OnarimTable.Controls.Add(headerRow1);

            Onarim onarim = new Onarim();
            List<Onarim> onarimListesi = onarim.SelectByTasinmazId(tasinmazId);
            if (onarimListesi == null)
            {
                TableHeaderCell cell1 = new TableHeaderCell();
                cell1.Text = "Onarım bulunmamaktadır.";
                headerRow1.Controls.Add(cell1);
            }
            else
            {
                TableHeaderCell cell1 = new TableHeaderCell();
                cell1.Text = "Yapılan İş";
                TableHeaderCell cell2 = new TableHeaderCell();
                cell2.Text = "Harcama Usulü";
                TableHeaderCell cell3 = new TableHeaderCell();
                cell3.Text = "Onay Tarihi";
                TableHeaderCell cell4 = new TableHeaderCell();
                cell4.Text = "Tutar";
                headerRow1.Controls.Add(cell1);
                headerRow1.Controls.Add(cell2);
                headerRow1.Controls.Add(cell3);
                headerRow1.Controls.Add(cell4);
                foreach (Onarim item in onarimListesi)
                {
                    TableRow row = new TableRow();
                    TableCell yapilanIsCell = new TableCell();
                    yapilanIsCell.Text = item.YapilanIs;
                    TableCell harcamaUsuluCell = new TableCell();
                    harcamaUsuluCell.Text = item.HarcamaUsulu;
                    TableCell tarihCell = new TableCell();
                    tarihCell.Text = item.OnayTarihi.ConvertToDatetimeEmptyIfNull();
                    TableCell tutarCell = new TableCell();
                    tutarCell.Text = item.Tutar.ToString("N", culturInfo);

                    row.Controls.Add(yapilanIsCell);
                    row.Controls.Add(harcamaUsuluCell);
                    row.Controls.Add(tarihCell);
                    row.Controls.Add(tutarCell);
                    BorderEkle(row);

                    OnarimTable.Controls.Add(row);
                }
            }

        }
        private void TasinmazResimleriniDoldur(Tasinmaz tasinmaz)
        {

            ResimTable.Rows.Clear();
            ResimTable.BorderWidth = 2;
            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.BackColor = System.Drawing.Color.LightGray; ;
            TableHeaderCell TabloBaslikCell = new TableHeaderCell();
            TabloBaslikCell.ColumnSpan = 4;
            TabloBaslikCell.Text = "Taşınmaz Resimleri";
            headerRow.Controls.Add(TabloBaslikCell);

            TableRow row0 = new TableRow();
            TableCell r0c1 = new TableCell();
            TableCell r0c2 = new TableCell();
            TableCell r0c3 = new TableCell();
            TableCell r0c4 = new TableCell();
            r0c1.ForeColor = System.Drawing.Color.White;
            r0c2.ForeColor = System.Drawing.Color.White;
            r0c3.ForeColor = System.Drawing.Color.White;
            r0c4.ForeColor = System.Drawing.Color.White;
            r0c1.Text = "TSKGV TSKGV TSKGV TSKGV TSKGV TSKGV TSKGV TSKGV ";
            r0c2.Text = "TSKGV TSKGV TSKGV TSKGV TSKGV TSKGV TSKGV TSKGV ";
            r0c3.Text = "TSKGV TSKGV TSKGV TSKGV TSKGV TSKGV TSKGV TSKGV ";
            r0c4.Text = "TSKGV TSKGV TSKGV TSKGV TSKGV TSKGV TSKGV TSKGV ";

            row0.Controls.Add(r0c1);
            row0.Controls.Add(r0c2);
            row0.Controls.Add(r0c3);
            row0.Controls.Add(r0c4);

            TableRow row1 = new TableRow();
            TableRow row2 = new TableRow();

            TableCell r1c1 = new TableCell();
            TableCell r1c2 = new TableCell();
            TableCell r1c3 = new TableCell();
            TableCell r1c4 = new TableCell();

            row1.Controls.Add(r1c1);
            row1.Controls.Add(r1c2);
            row1.Controls.Add(r1c3);
            row1.Controls.Add(r1c4);


            TableCell r2c1 = new TableCell();
            TableCell r2c2 = new TableCell();
            TableCell r2c3 = new TableCell();
            TableCell r2c4 = new TableCell();
            row2.Controls.Add(r2c1);
            row2.Controls.Add(r2c2);
            row2.Controls.Add(r2c3);
            row2.Controls.Add(r2c4);

            string newUrl = UtilityHelper.TbysURLGetir()+"/"+ProjeConstants.RESIMLER_TBYS + "/";
            string imageFileName1 = (string.IsNullOrEmpty(tasinmaz.TasinmazFoto) ? ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO : tasinmaz.TasinmazFoto) + "_jpg.jpg";
            string imageFileName2 = (string.IsNullOrEmpty(tasinmaz.TasinmazFoto1) ? ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO1 : tasinmaz.TasinmazFoto1) + "_jpg.jpg";
            string imageFileName3 = (string.IsNullOrEmpty(tasinmaz.TasinmazFoto2) ? ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO2 : tasinmaz.TasinmazFoto2) + "_jpg.jpg";
            string imageFileName4 = (string.IsNullOrEmpty(tasinmaz.TahkikatFoto) ? ProjeConstants.PARAM_TASINMAZ_TAHKIKATFOTO : tasinmaz.TahkikatFoto) + "_jpg.jpg";
            string imageFileName5 = (string.IsNullOrEmpty(tasinmaz.KrokiFoto) ? ProjeConstants.PARAM_TASINMAZ_KROKIFOTO : tasinmaz.KrokiFoto) + "_jpg.jpg";
            string imageFileName6 = (string.IsNullOrEmpty(tasinmaz.KrokiFoto) ? ProjeConstants.PARAM_TASINMAZ_TAPUFOTO : tasinmaz.TapuFoto) + "_jpg.jpg";

            string imgUrl1 = newUrl + "/_t/" + imageFileName1;
            string imgUrl2 = newUrl + "/_t/" + imageFileName2;
            string imgUrl3 = newUrl + "/_t/" + imageFileName3;
            string imgUrl4 = newUrl + "/_t/" + imageFileName4;
            string imgUrl5 = newUrl + "/_t/" + imageFileName5;
            string imgUrl6 = newUrl + "/_t/" + imageFileName6;

            System.Web.UI.WebControls.Image image1 = new System.Web.UI.WebControls.Image();
            System.Web.UI.WebControls.Image image2 = new System.Web.UI.WebControls.Image();
            System.Web.UI.WebControls.Image image3 = new System.Web.UI.WebControls.Image();
            System.Web.UI.WebControls.Image image4 = new System.Web.UI.WebControls.Image();
            System.Web.UI.WebControls.Image image5 = new System.Web.UI.WebControls.Image();
            System.Web.UI.WebControls.Image image6 = new System.Web.UI.WebControls.Image();

            image1.Attributes["onerror"] = "this.src='" + imgUrl1 + "';";
            image2.Attributes["onerror"] = "this.src='" + imgUrl2 + "';";
            image3.Attributes["onerror"] = "this.src='" + imgUrl3 + "';";
            image4.Attributes["onerror"] = "this.src='" + imgUrl4 + "';";
            image5.Attributes["onerror"] = "this.src='" + imgUrl5 + "';";
            image6.Attributes["onerror"] = "this.src='" + imgUrl6 + "';";

            image1.ImageUrl = imgUrl1;
            image2.ImageUrl = imgUrl2;
            image3.ImageUrl = imgUrl3;
            image4.ImageUrl = imgUrl4;
            image5.ImageUrl = imgUrl5;
            image6.ImageUrl = imgUrl6;

            r1c1.Controls.Add(image1);
            r1c2.Controls.Add(image2);
            r1c3.Controls.Add(image3);
            r1c4.Controls.Add(image4);
            r2c1.Controls.Add(image5);
            r2c2.Controls.Add(image6);

            BorderEkle(row1);
            BorderEkle(row2);
            AlignCells(row1);
            AlignCells(row2);
            r1c1.Height = 180;
            r2c1.Height = 180;

            ResimTable.Controls.Add(headerRow);
            ResimTable.Controls.Add(row0);
            ResimTable.Controls.Add(row1);
            ResimTable.Controls.Add(row2);
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

        protected void TasinmazListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_BOLGETASINMAZ_LIST+ "?EnvanterdeMi=1" );
        }
        protected void TasinmazaGitBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_TASINMAZ_GIRIS + "?TasinmazId=" + TasinmazIdQS + "&DestinationApp=TD" + "&EnvanterdeMi=" + EnvanterdeMiQS);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select<Tasinmaz>(TasinmazIdQS.ConvertToInt());
            if (tasinmaz != null)
            {
                TumTablolariDoldur(tasinmaz);
                ExportToExcel();
            }
            else
                MessageHelper.PublishMessage("Taşınmaz bulunamadı", ProjeConstants.MESAJ_HATA, 2000);
        }
    }
}
