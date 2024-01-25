using Microsoft.SharePoint;
using Model.IKYS;
using Model.MTS;
using Model.NBYS;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace MTS_WebParts.KisiKartiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KisiKartiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KisiKartiWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string KatilimciIdQS
        {
            get
            {

                if (ViewState["KatilimciId"] == null)
                {
                    if (Page.Request.QueryString["KatilimciId"] != null)
                    {
                        ViewState["KatilimciId"] = Page.Request.QueryString["KatilimciId"];
                    }
                    else
                    {
                        ViewState["KatilimciId"] = string.Empty;
                    }
                }
                return ViewState["KatilimciId"].ToString();
            }

            set
            {
                ViewState["KatilimciId"] = value;
            }
        }
        private string KatilimciTipiQS
        {
            get
            {

                if (ViewState["KatilimciTipi"] == null)
                {
                    if (Page.Request.QueryString["KatilimciTipi"] != null)
                    {
                        ViewState["KatilimciTipi"] = Page.Request.QueryString["KatilimciTipi"];
                    }
                    else
                    {
                        ViewState["KatilimciTipi"] = string.Empty;
                    }
                }
                return ViewState["KatilimciTipi"].ToString();
            }

            set
            {
                ViewState["KatilimciTipi"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    KisiKartiniOlustur();

                }

            }
            catch (Exception exception)
            {
                ExceptionHelper exHelper = new ExceptionHelper(exception);
                exHelper.PublishException();
            }
        }

        private void KisiKartiniOlustur()
        {
            switch (KatilimciTipiQS.ConvertToInt())
            {
                case ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI_INT:
                    {
                        TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
                        tasinmazBagisci = tasinmazBagisci.Select<TasinmazBagisci>(KatilimciIdQS.ConvertToInt());
                        if (tasinmazBagisci != null)
                        {
                            KatilimciItem katilimci = new KatilimciItem();
                            katilimci.KatilimciTipiStr = ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI;
                            katilimci.KatilimciId = tasinmazBagisci.Id + "/" + katilimci.KatilimciTipiStr;

                            katilimci.AdiSoyadi = tasinmazBagisci.Adi + " " + tasinmazBagisci.Soyadi;
                            katilimci.Adresi = tasinmazBagisci.Adres;
                            katilimci.IlcesiIli = (string.IsNullOrEmpty(tasinmazBagisci.Ilcesi) ? "" : tasinmazBagisci.Ilcesi) + " " + (string.IsNullOrEmpty(tasinmazBagisci.Ili) ? "" : tasinmazBagisci.Ili.ToUpper());
                            string telefon1 = string.IsNullOrEmpty(tasinmazBagisci.Telefon1.ToString()) ? "" : String.Format("{0:(###) ### ####}", tasinmazBagisci.Telefon1);
                            string telefon2 = string.IsNullOrEmpty(tasinmazBagisci.Telefon2.ToString()) ? "" : String.Format("{0:(###) ### ####}", tasinmazBagisci.Telefon2);

                            katilimci.Telefon = telefon1 + " - " + telefon2;
                            katilimci.Unvani = katilimci.KatilimciTipiStr;
                            TumTablolariDoldur(katilimci);
                        }
                        else
                        {
                            MessageHelper.PublishMessage("Kişi Bulunamadı", ProjeConstants.MESAJ_HATA);
                        }
                        break;
                    }

                case ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI_INT:
                    {
                        NakitBagisci nakitBagisci = new NakitBagisci();
                        nakitBagisci = nakitBagisci.Select<NakitBagisci>(KatilimciIdQS.ConvertToInt());
                        if (nakitBagisci != null)
                        {
                            KatilimciItem katilimci = new KatilimciItem();
                            katilimci.KatilimciTipiStr = ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI;
                            katilimci.KatilimciId = nakitBagisci.Id + "/" + katilimci.KatilimciTipiStr;
                            katilimci.AdiSoyadi = nakitBagisci.Adi + " " + nakitBagisci.Soyadi;
                            katilimci.Adresi = nakitBagisci.Adres;
                            Il il = new Il();
                            il = il.Select<Il>(nakitBagisci.Ili.ConvertToInt());
                            Ilce ilce = new Ilce();
                            ilce = ilce.Select<Ilce>(nakitBagisci.Ilcesi.ConvertToInt());

                            katilimci.IlcesiIli = (ilce == null ? "" : ilce.IlceAdi) + " " + (il == null ? "" : il.IlAdi.ToUpper());
                            string telefon1 = string.IsNullOrEmpty(nakitBagisci.Telefon1.ToString()) ? "" : String.Format("{0:(###) ### ####}", nakitBagisci.Telefon1);
                            string telefon2 = string.IsNullOrEmpty(nakitBagisci.Telefon2.ToString()) ? "" : String.Format("{0:(###) ### ####}", nakitBagisci.Telefon2);

                            katilimci.Telefon = telefon1 + " - " + telefon2;
                            katilimci.Unvani = katilimci.KatilimciTipiStr;

                            TumTablolariDoldur(katilimci);
                        }
                        else
                        {
                            MessageHelper.PublishMessage("Kişi Bulunamadı", ProjeConstants.MESAJ_HATA);
                        }
                        break;
                    }
                case ProjeConstants.FAALIYET_KATILIMCI_DIS_INT:
                    {
                        Kisi kisi = new Kisi();
                        kisi = kisi.Select(KatilimciIdQS.ConvertToInt());
                        if (kisi != null)
                        {
                            KatilimciItem katilimci = new KatilimciItem();
                            katilimci.KatilimciTipiStr = ProjeConstants.FAALIYET_KATILIMCI_DIS;
                            katilimci.KatilimciId = kisi.Id + "/" + katilimci.KatilimciTipiStr;
                            katilimci.AdiSoyadi = kisi.Adi + " " + kisi.Soyadi;
                            katilimci.Adresi = kisi.Adres;
                            Il il = new Il();
                            il = il.Select<Il>(kisi.Ili);
                            Ilce ilce = new Ilce();
                            ilce = ilce.Select<Ilce>(kisi.Ilcesi);
                            katilimci.IlcesiIli = ilce == null ? "" : ilce.IlceAdi + " " + (il == null ? "" : il.IlAdi.ToUpper());
                            string telefon1 = string.IsNullOrEmpty(kisi.Telefon1.ToString()) ? "" : String.Format("{0:(###) ### ####}", kisi.Telefon1)
                                + (string.IsNullOrEmpty(kisi.Dahili1) ? "" : " /" + kisi.Dahili1.Trim());
                            string telefon2 = string.IsNullOrEmpty(kisi.Telefon2.ToString()) ? "" : String.Format("{0:(###) ### ####}", kisi.Telefon2)
                                    + (string.IsNullOrEmpty(kisi.Dahili2) ? "" : " /" + kisi.Dahili2.Trim());
                            string telefon3 = string.IsNullOrEmpty(kisi.Telefon3.ToString()) ? "" : String.Format("{0:(###) ### ####}", kisi.Telefon3)
                                    + (string.IsNullOrEmpty(kisi.Dahili3) ? "" : " /" + kisi.Dahili3.Trim());
                            katilimci.Telefon = telefon1 + " - " + telefon2 + " - " + telefon3;
                            katilimci.Unvani = kisi.Unvani;
                            katilimci.Gorevi = kisi.Gorevi;
                            TumTablolariDoldur(katilimci);
                        }
                        else
                        {
                            MessageHelper.PublishMessage("Kişi Bulunamadı", ProjeConstants.MESAJ_HATA);
                        }
                        break;
                    }
                case ProjeConstants.FAALIYET_KATILIMCI_IC_INT:
                    {
                        Personel personel = new Personel();
                        personel = personel.SelectCalisanPersonel(KatilimciIdQS.ConvertToInt());
                        if (personel != null)
                        {
                            KatilimciItem katilimci = new KatilimciItem();
                            katilimci.KatilimciTipiStr = ProjeConstants.FAALIYET_KATILIMCI_IC;
                            katilimci.KatilimciId = personel.Id + "/" + katilimci.KatilimciTipiStr;
                            katilimci.AdiSoyadi = personel.Adi + " " + personel.Soyadi;

                            #region İş bilgileri
                            IsBilgileri isBilgisi = new IsBilgileri();
                            isBilgisi = isBilgisi.SelectByPersonelId(personel.Id);
                            if (isBilgisi != null)
                            {
                                GorevTanim gt = new GorevTanim();
                                gt = gt.Select<GorevTanim>(isBilgisi.GorevId);
                                UnvanTanim unvan = new UnvanTanim();
                                unvan = unvan.Select<UnvanTanim>(isBilgisi.UnvanId);

                                katilimci.Unvani = gt != null ? gt.Adi : "";
                                katilimci.Gorevi = unvan != null ? unvan.Adi : "";
                            }
                            #endregion
                            IletisimBilgileri iletisimBilgileri = new IletisimBilgileri();
                            iletisimBilgileri = iletisimBilgileri.SelectByPersonelId(personel.Id);

                            if (iletisimBilgileri != null)
                            {
                                katilimci.Adresi = iletisimBilgileri != null ? iletisimBilgileri.Adres : "";

                                Il il = new Il();
                                il = il.Select<Il>(iletisimBilgileri.Ili.ConvertToInt());
                                Ilce ilce = new Ilce();
                                ilce = ilce.Select<Ilce>(iletisimBilgileri.Ilcesi.ConvertToInt());

                                katilimci.IlcesiIli = (string.IsNullOrEmpty(ilce.IlceAdi) ? "" : ilce.IlceAdi) + " " + (string.IsNullOrEmpty(il.IlAdi) ? "" : il.IlAdi.ToUpper());
                                string telefon1 = string.IsNullOrEmpty(iletisimBilgileri.EvTelefonu.ToString()) ? "" : String.Format("{0:(###) ### ####}", iletisimBilgileri.EvTelefonu);
                                string telefon2 = string.IsNullOrEmpty(iletisimBilgileri.CepTelefonu.ToString()) ? "" : String.Format("{0:(###) ### ####}", iletisimBilgileri.CepTelefonu);
                                string telefon3 = string.IsNullOrEmpty(iletisimBilgileri.CepTelefonu2.ToString()) ? "" : String.Format("{0:(###) ### ####}", iletisimBilgileri.CepTelefonu2);
                                katilimci.Telefon = telefon1 + " - " + telefon2 + " - " + telefon3;
                            }

                            TumTablolariDoldur(katilimci);
                        }
                        else
                        {
                            MessageHelper.PublishMessage("Personel Bulunamadı", ProjeConstants.MESAJ_HATA);
                        }
                        break;
                    }
            }
        }

        private void TumTablolariDoldur(KatilimciItem katilimci)
        {

            if (katilimci != null)
            {
                IdLbl.Text = " ( " + katilimci.KatilimciId.ToString() + " )";
                TabloyaKatilimciBilgileriniDoldur(katilimci);
                TabloyaFaaliyetBilgileriniDoldur();
                TabloyaAramaBilgileriniDoldur();
            }
        }
        protected void ExportToExcel()
        {

            string filename = "KisiKarti" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
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
        private void BorderEkle(TableRow row)
        {
            for (int n = 0; n < row.Cells.Count; n++)
            {
                row.Cells[n].BorderStyle = BorderStyle.Solid;
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
        private void TabloyaKatilimciBilgileriniDoldur(KatilimciItem katilimci)
        {
            BaslikTH.Text = (katilimci.AdiSoyadi).ToUpper();
            BaslikTH.HorizontalAlign = HorizontalAlign.Center;
            BaslikTarihCell.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            TableRow row1 = new TableRow();
            TableRow row2 = new TableRow();
            TableRow row3 = new TableRow();

            TableCell r1c1 = new TableCell();
            r1c1.ColumnSpan = 2;
            TableCell r1c2 = new TableCell();
            r1c2.ColumnSpan = 2;
            TableCell r1c3 = new TableCell();
            TableCell r1c4 = new TableCell();

            row1.Controls.Add(r1c1);
            row1.Controls.Add(r1c2);
            row1.Controls.Add(r1c3);
            row1.Controls.Add(r1c4);

            TableCell r2c1 = new TableCell();
            r2c1.ColumnSpan = 2;
            TableCell r2c2 = new TableCell();
            r2c2.ColumnSpan = 2;
            TableCell r2c3 = new TableCell();
            TableCell r2c4 = new TableCell();

            row2.Controls.Add(r2c1);
            row2.Controls.Add(r2c2);
            row2.Controls.Add(r2c3);
            row2.Controls.Add(r2c4);

            TableCell r3c1 = new TableCell();
            r3c1.ColumnSpan = 2;
            TableCell r3c2 = new TableCell();
            r3c2.ColumnSpan = 2;
            TableCell r3c3 = new TableCell();
            TableCell r3c4 = new TableCell();

            row3.Controls.Add(r3c1);
            row3.Controls.Add(r3c2);
            row3.Controls.Add(r3c3);
            row3.Controls.Add(r3c4);


            r1c1.Text = "Ünvanı : " + katilimci.Unvani;
            r2c1.Text = "Görevi : " + katilimci.Gorevi;
            r3c1.Text = "";

            r1c2.Text = "Kurumu : " + katilimci.Kurumu;
            r2c2.Text = "Adresi : " + katilimci.Adresi;
            r3c2.Text = "İlçe/İl :" + katilimci.IlcesiIli;

            r1c3.Text = katilimci.Telefon;

            BorderEkle(row1);
            BorderEkle(row2);
            BorderEkle(row3);

            KisiBilgileriTable.Controls.Add(row1);
            KisiBilgileriTable.Controls.Add(row2);
            KisiBilgileriTable.Controls.Add(row3);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            KisiKartiniOlustur();
            ExportToExcel();
        }
        private void TabloyaFaaliyetBilgileriniDoldur()
        {
            List<FaaliyetListItem> list = GetFaaliyetDataList();
            foreach (var item in list)
            {
                TableRow row = new TableRow();
                TableCell nocell = new TableCell();
                nocell.Text = item.FaaliyetId;

                TableCell tarihcell = new TableCell();
                tarihcell.Text = item.FaaliyetTarihi;

                TableCell yercell = new TableCell();
                yercell.Text = item.FaaliyetYeri + " / " + item.FaaliyetKonusu;

                //TableCell konucell = new TableCell();
                //konucell.Text = item.FaaliyetKonusu;

                TableCell amaccell = new TableCell();
                amaccell.Text = item.FaaliyetAmaci + " / " + item.FaaliyetDurumu;

                //TableCell durumcell = new TableCell();
                //durumcell.Text = item.FaaliyetDurumu;

                TableCell verilenaniobjesicell = new TableCell();
                verilenaniobjesicell.Text = item.VerilenAniObjesi.Replace(";", "<br />"); ;

                TableCell getirilenaniobjesicell = new TableCell();
                getirilenaniobjesicell.Text = item.GetirilenAniObjesi.Replace(";", "<br />"); ;

                TableCell katilimcicell = new TableCell();
                katilimcicell.Text = item.Katilimci.Replace(";", "<br />");

                TableCell faaliyetkarticell = new TableCell();
                faaliyetkarticell.Text = item.FaaliyetKarti;

                row.Controls.Add(nocell);
                row.Controls.Add(tarihcell);
                row.Controls.Add(yercell);
                //row.Controls.Add(konucell);
                row.Controls.Add(amaccell);
                //row.Controls.Add(durumcell);
                row.Controls.Add(verilenaniobjesicell);
                row.Controls.Add(getirilenaniobjesicell);
                row.Controls.Add(katilimcicell);
                row.Controls.Add(faaliyetkarticell);
                BorderEkle(row);
                if (item.FaaliyetTipi.Equals(ProjeConstants.RANDEVU_VERILEN))
                {
                    VerilenFaaliyetBilgileriTable.Rows.Add(row);
                }
                else
                {
                    AlınanFaaliyetBilgileriTable.Rows.Add(row);
                }

            }
        }
        private List<FaaliyetListItem> GetFaaliyetDataList()
        {
            List<FaaliyetListItem> faaliyetList = new List<FaaliyetListItem>();
            Faaliyet faaliyetDao = new Faaliyet();
            DataTable dataTable = faaliyetDao.SelectByKatilimciReturnDataTable(KatilimciIdQS.ConvertToInt(), KatilimciTipiQS.ConvertToInt(), ProjeConstants.HEPSI_INT);

            if (dataTable != null)
            {
                int tempFaaliyetId = 0;
                int katilimciAdedi = 0;

                FaaliyetListItem tempFaaliyetListItem = new FaaliyetListItem();
                foreach (DataRow row in dataTable.Rows)
                {
                    int faaliyetId = row["FaaliyetId"].ConvertToInt();
                    string faaliyetTipi = row["FaaliyetTipi"].ToString();
                    string faaliyetYeri = row["FaaliyetYeri"].ToString(); ;
                    string faaliyetKonusu = row["FaaliyetKonusu"].ToString();
                    string faaliyetAmaci = row["FaaliyetAmaci"].ToString();
                    string faaliyetDurumu = row["FaaliyetDurumu"].ToString();
                    string katilimciId = row["KatilimciId"].ToString();
                    string katilimciTipi = row["KatilimciTipi"].ToString();


                    DateTime basTar = row["BaslangicTarihi"].ConvertToDatetime();
                    DateTime bitTar = row["BitisTarihi"].ConvertToDatetime();

                    string faaliyetTarihiStr = basTar.Year == bitTar.Year && basTar.Month == bitTar.Month && basTar.Day == bitTar.Day ?
                        basTar.ToString("dd.MM.yyyy") + " " + basTar.ToString("HH:mm") + "-" + bitTar.ToString("HH:mm") :
                        basTar.ToString("dd.MM.yyyy HH:mm") + " - " + bitTar.ToString("dd.MM.yyyy HH:mm");


                    string faaliyetAmaciStr = ParseFaaliyetAmaci(faaliyetAmaci);
                    string faaliyetDurumuStr = MTSOrtak.ParseFaaliyetDurumu(faaliyetDurumu.ConvertToInt());
                    string verilenAniObjesiStr = VerilenAniOjesiGetir(faaliyetId, KatilimciIdQS.ConvertToInt(), KatilimciTipiQS.ConvertToInt());
                    string getirilenAniObjesiStr = GetirilenAniOjesiGetir(faaliyetId, KatilimciIdQS.ConvertToInt(), KatilimciTipiQS.ConvertToInt());
                    string buKatilimci = katilimciId.Equals(KatilimciIdQS) && katilimciTipi.Equals(KatilimciTipiQS) ? "*" : string.Empty;
                    string katilimci = buKatilimci + row["Adi"].ToString() + " " + row["Soyadi"].ToString();


                    if (tempFaaliyetId == faaliyetId)
                    {
                        tempFaaliyetId = faaliyetId;
                        //faaliyetList.Remove(tempFaaliyetListItem);

                        //tempFaaliyetListItem.Katilimci += "@" + adiSoyadi;
                        katilimciAdedi++;
                        tempFaaliyetListItem.Katilimci += katilimci + "; ";
                        //faaliyetList.Add(tempFaaliyetListItem);
                    }


                    if (tempFaaliyetId != faaliyetId)
                    {
                        FaaliyetListItem faaliyetListItem = new FaaliyetListItem();
                        faaliyetListItem.FaaliyetId = faaliyetId.ToString();
                        faaliyetListItem.FaaliyetTipi = faaliyetTipi;
                        faaliyetListItem.FaaliyetYeri = faaliyetYeri;
                        faaliyetListItem.FaaliyetKonusu = faaliyetKonusu;
                        faaliyetListItem.FaaliyetTarihi = faaliyetTarihiStr;
                        faaliyetListItem.FaaliyetAmaci = faaliyetAmaciStr;
                        faaliyetListItem.FaaliyetDurumu = faaliyetDurumuStr;
                        faaliyetListItem.VerilenAniObjesi = verilenAniObjesiStr;
                        faaliyetListItem.GetirilenAniObjesi = getirilenAniObjesiStr;

                        faaliyetListItem.Katilimci += katilimci + "; ";
                        faaliyetListItem.FaaliyetKarti = "<a href=" + ProjeConstants.PAGE_FAALIYET_KARTI + "?FaaliyetId=" + faaliyetId + " class='btn btn-outline-primary'>Faaliyet Kartı</a>";

                        tempFaaliyetListItem = faaliyetListItem;

                    }
                    tempFaaliyetId = faaliyetId;
                    if (!faaliyetList.Contains(tempFaaliyetListItem))
                        faaliyetList.Add(tempFaaliyetListItem);
                }
            }
            //faaliyetList = faaliyetList.OrderBy(r => r.FaaliyetTarihi).ToList();
            return faaliyetList;
        }

        private string VerilenAniOjesiGetir(int faaliyetId, int katilimciId, int katilimciTipi)
        {
            string objeStr = string.Empty;
            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            DataTable dataTable = aniObjesiDagitim.SelectReturnDT(faaliyetId, katilimciId, katilimciTipi);
            if (dataTable != null)
            {

                foreach (DataRow row in dataTable.Rows)
                {
                    int adet = row["Adet"].ReturnZeroIfNull().ConvertToInt();
                    if (adet > 0)
                    {
                        string deger = row["Adi"].ToString();
                        objeStr += deger + "(" + adet + ");";
                    }
                }

            }

            return objeStr;
        }

        private string GetirilenAniOjesiGetir(int faaliyetId, int katilimciId, int katilimciTipi)
        {
            string aniobjeStr = string.Empty;
            AniObjesiDagitim getirilenAniObjesi = new AniObjesiDagitim();
            getirilenAniObjesi = getirilenAniObjesi.SelectGetirilenAniObjesi(faaliyetId, katilimciId, katilimciTipi);
            if (getirilenAniObjesi != null)
            {
                aniobjeStr = getirilenAniObjesi.GetirilenAniObjesi;
            }
            return aniobjeStr;
        }

        private string ParseFaaliyetAmaci(string amac)
        {
            string amacStr = string.Empty;
            switch (amac)
            {
                case ProjeConstants.FAALIYET_AMACI_DAVET_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_DAVET;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_IZIN_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_IZIN;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_OZELCALISMA_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_OZELCALISMA;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_RESMITATIL_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_RESMITATIL;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_TOPLANTI_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_TOPLANTI;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_YILDONUMU_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_YILDONUMU;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_ZIYARET_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_ZIYARET;
                        break;
                    }
                default:
                    break;
            }
            return amacStr;
        }
        private void TabloyaAramaBilgileriniDoldur()
        {
            List<AramaGorusme> list = GetAramaDataList();
            foreach (var item in list)
            {
                TableRow row = new TableRow();
                TableCell nocell = new TableCell();
                nocell.Text = item.Id.ToString();

                TableCell tarihcell = new TableCell();
                tarihcell.Text = item.Tarih.ToString("dd.MM.yyyy HH:mm");
                tarihcell.HorizontalAlign = HorizontalAlign.Left;
                TableCell gorusmeSeklicell = new TableCell();
                gorusmeSeklicell.Text = item.GorusmeSekli;

                TableCell konucell = new TableCell();
                konucell.Text = item.Konu;

                TableCell faaliyetcell = new TableCell();
                if (item.RandevuId > 0)
                {
                    Faaliyet faaliyet = new Faaliyet();
                    faaliyet = faaliyet.Select(item.RandevuId);
                    if (faaliyet != null)
                    {
                        string faaliyetTarihiStr = faaliyet.BaslangicTarihi.Year == faaliyet.BitisTarihi.Year &&
                            faaliyet.BaslangicTarihi.Month == faaliyet.BitisTarihi.Month && faaliyet.BaslangicTarihi.Day == faaliyet.BitisTarihi.Day ?
                                faaliyet.BaslangicTarihi.ToString("dd.MM.yyyy") + " " + faaliyet.BaslangicTarihi.ToString("HH:mm") + "-" + faaliyet.BitisTarihi.ToString("HH:mm") :
                                faaliyet.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + " - " + faaliyet.BitisTarihi.ToString("dd.MM.yyyy HH:mm");
                        faaliyetcell.Text = faaliyetTarihiStr;
                    }

                }
                else
                {
                    if (item.RandevuIstendi)
                        faaliyetcell.Text = "İstendi";
                    else
                    {
                        faaliyetcell.Text = "-";
                    }
                }
                TableCell gorusmecell = new TableCell();
                gorusmecell.Text = item.GorusmeSaglandi ? "Evet" : "Hayır";

                row.Controls.Add(nocell);
                row.Controls.Add(tarihcell);
                row.Controls.Add(gorusmeSeklicell);
                row.Controls.Add(konucell);
                row.Controls.Add(faaliyetcell);
                row.Controls.Add(gorusmecell);
                BorderEkle(row);

                AramaGorusmeTable.Controls.Add(row);
            }
        }
        private List<AramaGorusme> GetAramaDataList()
        {
            AramaGorusme aramaDao = new AramaGorusme();
            List<AramaGorusme> aramaList = new List<AramaGorusme>();
            if (KatilimciTipiQS.ConvertToInt() > 0)
            {
                aramaList = aramaDao.SelectAllByArayanIdReturnList(KatilimciIdQS.ConvertToInt(), KatilimciTipiQS.ConvertToInt());
            }

            return aramaList;
        }
        protected void KisiListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KISI_LIST);
        }
        protected void FaaliyetListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_LIST);
        }
        protected void FaaliyetTakvimiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM);
        }
        protected void AramaListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_ARAMAGORUSME_LIST);
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
        private class FaaliyetListItem
        {
            public string FaaliyetId { get; set; }
            public string FaaliyetTipi { get; set; }
            public string FaaliyetYeri { get; set; }
            public string FaaliyetKonusu { get; set; }
            public string FaaliyetAmaci { get; set; }
            public string FaaliyetDurumu { get; set; }
            public string FaaliyetTarihi { get; set; }
            public string VerilenAniObjesi { get; set; }
            public string GetirilenAniObjesi { get; set; }
            public string Katilimci { get; set; }
            public string FaaliyetKarti { get; set; }
        }
        private class KatilimciItem
        {
            public string Sirano { get; set; }
            //public string Id { get; set; }
            public string KatilimciId { get; set; }
            public string KatilimciTipi { get; set; }
            public string KatilimciTipiStr { get; set; }
            public string AdiSoyadi { get; set; }
            public string Unvani { get; set; }
            public string Gorevi { get; set; }
            public string Kurumu { get; set; }
            public string Adresi { get; set; }
            public string IlcesiIli { get; set; }
            public string Telefon { get; set; }
            public string AniObjesi { get; set; }
            public string KisiKarti { get; set; }
            public string Cikar { get; set; }

        }
    }
}
