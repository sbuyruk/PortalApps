using Model.IKYS;
using Model.MTS;
using Model.NBYS;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace MTS_WebParts.RandevuKartiWP
{
    [ToolboxItemAttribute(false)]
    public partial class RandevuKartiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public RandevuKartiWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string RandevuIdQS
        {
            get
            {

                if (ViewState["RandevuId"] == null)
                {
                    if (Page.Request.QueryString["RandevuId"] != null)
                    {
                        ViewState["RandevuId"] = Page.Request.QueryString["RandevuId"];
                    }
                    else
                    {
                        ViewState["RandevuId"] = string.Empty;
                    }
                }
                return ViewState["RandevuId"].ToString();
            }

            set
            {
                ViewState["RandevuId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            IdLbl.Text = " ( Faaliyet No: " + RandevuIdQS.ToString() + " )";
            RandevuBilgileriniDoldur();
            AniObjesiBilgileriTableDoldur();
        }
        private void AniObjesiBilgileriTableDoldur()
        {
            /***
             * RandevuKatılım_Table'dan Bu Randevuya katılanları getir
             * **/
            RandevuKatilim randevuKatilim = new RandevuKatilim();
            List<RandevuKatilim> liste = randevuKatilim.SelectByrandevuId(RandevuIdQS.ConvertToInt());
            foreach (var item in liste)
            {
                TableRow tableRow = new TableRow();
                TableCell katilimciAdiSoyadiCell = new TableCell();
                TableCell katilimciTipiCell = new TableCell();
                TableCell verilenAniObjesiCell = new TableCell();
                TableCell getirilenAniObjesiCell = new TableCell();
                ////////////////

                switch (item.KatilimciTipi)
                {
                    case ProjeConstants.RANDEVU_KATILIMCI_TASINMAZBAGISCI_INT:
                        {
                            TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
                            tasinmazBagisci = tasinmazBagisci.Select<TasinmazBagisci>(item.KatilimciId);
                            if (tasinmazBagisci != null)
                            {
                                katilimciAdiSoyadiCell.Text = tasinmazBagisci.Adi + " " + tasinmazBagisci.Soyadi;
                                katilimciTipiCell.Text = ProjeConstants.RANDEVU_KATILIMCI_TASINMAZBAGISCI;

                                //Burada verilen ani objeleri alınıyor
                                AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
                                DataTable dataTable = aniObjesiDagitim.SelectReturnDT(RandevuIdQS.ConvertToInt(), tasinmazBagisci.Id, ProjeConstants.RANDEVU_KATILIMCI_TASINMAZBAGISCI_INT);
                                if (dataTable != null)
                                {
                                    string objeStr = string.Empty;
                                    foreach (DataRow row in dataTable.Rows)
                                    {
                                        int adet = row["Adet"].ReturnZeroIfNull().ConvertToInt();
                                        if (adet > 0)
                                        {
                                            string deger = row["Adi"].ToString();
                                            objeStr += " - " + deger + "(" + adet + ")";
                                        }
                                    }

                                    verilenAniObjesiCell.Text = objeStr;
                                }
                                //Getirilen Ani Objeleri ayrıca alınıyor
                                AniObjesiDagitim getirilenAniObjesi = new AniObjesiDagitim();
                                getirilenAniObjesi = getirilenAniObjesi.SelectGetirilenAniObjesi(RandevuIdQS.ConvertToInt(), tasinmazBagisci.Id, ProjeConstants.RANDEVU_KATILIMCI_TASINMAZBAGISCI_INT);
                                if (getirilenAniObjesi != null)
                                {
                                    getirilenAniObjesiCell.Text = getirilenAniObjesi.GetirilenAniObjesi;
                                }
                            }
                            else
                            {
                                MessageHelper.PublishMessage("Kişi Bulunamadı", ProjeConstants.MESAJ_HATA);
                            }
                            break;
                        }

                    case ProjeConstants.RANDEVU_KATILIMCI_NAKITBAGISCI_INT:
                        {
                            NakitBagisci nakitBagisci = new NakitBagisci();
                            nakitBagisci = nakitBagisci.Select<NakitBagisci>(item.KatilimciId);
                            if (nakitBagisci != null)
                            {
                                katilimciAdiSoyadiCell.Text = nakitBagisci.Adi + " " + nakitBagisci.Soyadi;
                                katilimciTipiCell.Text = ProjeConstants.RANDEVU_KATILIMCI_NAKITBAGISCI;

                                //Burada verilen ani objeleri alınıyor
                                AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
                                DataTable dataTable = aniObjesiDagitim.SelectReturnDT(RandevuIdQS.ConvertToInt(), nakitBagisci.Id, ProjeConstants.RANDEVU_KATILIMCI_NAKITBAGISCI_INT);
                                if (dataTable != null)
                                {
                                    string objeStr = string.Empty;
                                    foreach (DataRow row in dataTable.Rows)
                                    {
                                        int adet = row["Adet"].ReturnZeroIfNull().ConvertToInt();
                                        if (adet > 0)
                                        {
                                            string deger = row["Adi"].ToString();
                                            objeStr += " - " + deger + "(" + adet + ")";
                                        }
                                    }

                                    verilenAniObjesiCell.Text = objeStr;
                                }

                                //Getirilen Ani Objeleri ayrıca alınıyor
                                AniObjesiDagitim getirilenAniObjesi = new AniObjesiDagitim();
                                getirilenAniObjesi = getirilenAniObjesi.SelectGetirilenAniObjesi(RandevuIdQS.ConvertToInt(), nakitBagisci.Id, ProjeConstants.RANDEVU_KATILIMCI_NAKITBAGISCI_INT);
                                if (getirilenAniObjesi != null)
                                {
                                    getirilenAniObjesiCell.Text = getirilenAniObjesi.GetirilenAniObjesi;
                                }
                            }
                            else
                            {
                                MessageHelper.PublishMessage("Kişi Bulunamadı", ProjeConstants.MESAJ_HATA);
                            }
                            break;
                        }

                    case ProjeConstants.RANDEVU_KATILIMCI_DIS_INT:
                        {
                            Kisi kisi = new Kisi();
                            kisi = kisi.Select<Kisi>(item.KatilimciId);
                            if (kisi != null)
                            {
                                katilimciAdiSoyadiCell.Text = kisi.Adi + " " + kisi.Soyadi;
                                katilimciTipiCell.Text = ProjeConstants.RANDEVU_KATILIMCI_DIS;

                                //Burada verilen ani objeleri alınıyor
                                AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
                                DataTable dataTable = aniObjesiDagitim.SelectReturnDT(RandevuIdQS.ConvertToInt(), kisi.Id, ProjeConstants.RANDEVU_KATILIMCI_DIS_INT);
                                if (dataTable != null)
                                {
                                    string objeStr = string.Empty;
                                    foreach (DataRow row in dataTable.Rows)
                                    {
                                        int adet = row["Adet"].ReturnZeroIfNull().ConvertToInt();
                                        if (adet > 0)
                                        {
                                            string deger = row["Adi"].ToString();
                                            objeStr += " - " + deger + "(" + adet + ")";
                                        }

                                    }
                                    verilenAniObjesiCell.Text = objeStr;
                                }

                                //Getirilen Ani Objeleri ayrıca alınıyor
                                AniObjesiDagitim getirilenAniObjesi = new AniObjesiDagitim();
                                getirilenAniObjesi = getirilenAniObjesi.SelectGetirilenAniObjesi(RandevuIdQS.ConvertToInt(), kisi.Id, ProjeConstants.RANDEVU_KATILIMCI_DIS_INT);
                                if (getirilenAniObjesi != null)
                                {
                                    getirilenAniObjesiCell.Text = getirilenAniObjesi.GetirilenAniObjesi;
                                }
                            }
                            else
                            {
                                MessageHelper.PublishMessage("Kişi Bulunamadı", ProjeConstants.MESAJ_HATA);
                            }
                            break;
                        }


                    case ProjeConstants.RANDEVU_KATILIMCI_IC_INT:
                        {
                            Personel personel = new Personel();
                            personel = personel.Select<Personel>(item.KatilimciId);
                            if (personel != null)
                            {
                                katilimciAdiSoyadiCell.Text = personel.Adi + " " + personel.Soyadi;
                                katilimciTipiCell.Text = ProjeConstants.RANDEVU_KATILIMCI_IC;
                                verilenAniObjesiCell.Text = string.Empty;
                                getirilenAniObjesiCell.Text = string.Empty;
                            }
                            else
                            {
                                MessageHelper.PublishMessage("Kişi Bulunamadı", ProjeConstants.MESAJ_HATA);
                            }
                            break;
                        }

                        ///////////////////
                }
                tableRow.Controls.Add(katilimciAdiSoyadiCell);
                tableRow.Controls.Add(katilimciTipiCell);
                tableRow.Controls.Add(verilenAniObjesiCell);
                tableRow.Controls.Add(getirilenAniObjesiCell);
                AniObjesiBilgileriTable.Rows.Add(tableRow);
            }

        }
        private void RandevuBilgileriniDoldur()
        {
            Randevu randevu = new Randevu();
            randevu = randevu.Select(RandevuIdQS.ConvertToInt());


            if (randevu != null)
            {
                TableRow row1 = new TableRow();
                TableRow row2 = new TableRow();
                TableRow row3 = new TableRow();

                TableCell r1c1 = new TableCell();
                TableCell r1c2 = new TableCell();
                TableCell r1c3 = new TableCell();


                row1.Controls.Add(r1c1);
                row1.Controls.Add(r1c2);
                row1.Controls.Add(r1c3);


                TableCell r2c1 = new TableCell();
                TableCell r2c2 = new TableCell();
                TableCell r2c3 = new TableCell();


                row2.Controls.Add(r2c1);
                row2.Controls.Add(r2c2);
                row2.Controls.Add(r2c3);


                TableCell r3c1 = new TableCell();
                TableCell r3c2 = new TableCell();

                row3.Controls.Add(r3c1);
                row3.Controls.Add(r3c2);


                r1c1.Text = "Faaliyet Tipi : " + randevu.RandevuTipi;
                r1c2.Text = "Faaliyet Tarihi : " + randevu.BaslangicTarihi.ToString("dd-MM-yyyy") + "-" + randevu.BitisTarihi.ToString("dd-MM-yyyy");
                r1c3.Text = "Faaliyet Saati : " + randevu.BaslangicSaati + "-" + randevu.BitisSaati;

                FaaliyetYeri faaliyetYeri = new FaaliyetYeri();
                faaliyetYeri = faaliyetYeri.Select(randevu.RandevuYeri.ConvertToInt());

                r2c1.Text = "Faaliyet Yeri : " + (faaliyetYeri == null ? "" : faaliyetYeri.Adi);
                r2c2.Text = "Faaliyet Amacı : " + ParseRandevuAmaci(randevu.RandevuAmaci.ToString());
                r2c3.Text = "Faaliyet Durumu : " + ParseRandevuDurumu(randevu.RandevuDurumu.ConvertToInt());

                r3c1.Text = "Faaliyet Konusu : " + randevu.RandevuKonusu;
                r3c2.Text = "Açıklama : " + randevu.Aciklama;
                r3c2.ColumnSpan = 2;

                BorderEkle(row1);
                BorderEkle(row2);
                BorderEkle(row3);

                RandevuKartiBilgileriTable.Controls.Add(row1);
                RandevuKartiBilgileriTable.Controls.Add(row2);
                RandevuKartiBilgileriTable.Controls.Add(row3);
            }
            else
            {
                MessageHelper.PublishMessage("Faaliyet Bulunamadı", ProjeConstants.MESAJ_HATA);
            }
        }
        private string ParseRandevuAmaci(string amac)
        {
            string amacStr = string.Empty;
            switch (amac)
            {
                case ProjeConstants.RANDEVU_AMACI_DAVET_INT:
                    {
                        amacStr = ProjeConstants.RANDEVU_AMACI_DAVET;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_IZIN_INT:
                    {
                        amacStr = ProjeConstants.RANDEVU_AMACI_IZIN;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_OZELCALISMA_INT:
                    {
                        amacStr = ProjeConstants.RANDEVU_AMACI_OZELCALISMA;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_RESMITATIL_INT:
                    {
                        amacStr = ProjeConstants.RANDEVU_AMACI_RESMITATIL;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_TOPLANTI_INT:
                    {
                        amacStr = ProjeConstants.RANDEVU_AMACI_TOPLANTI;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_YILDONUMU_INT:
                    {
                        amacStr = ProjeConstants.RANDEVU_AMACI_YILDONUMU;
                        break;
                    }
                case ProjeConstants.RANDEVU_AMACI_ZIYARET_INT:
                    {
                        amacStr = ProjeConstants.RANDEVU_AMACI_ZIYARET;
                        break;
                    }
                default:
                    break;
            }
            return amacStr;
        }
        private string ParseRandevuDurumu(int durum)
        {
            string durumStr = string.Empty;
            switch (durum)
            {
                case ProjeConstants.RANDEVU_DURUMU_PLANLANDI_INT:
                    {
                        durumStr = ProjeConstants.RANDEVU_DURUMU_PLANLANDI;
                        break;
                    }
                case ProjeConstants.RANDEVU_DURUMU_ONAYLANDI_INT:
                    {
                        durumStr = ProjeConstants.RANDEVU_DURUMU_ONAYLANDI;
                        break;
                    }
                case ProjeConstants.RANDEVU_DURUMU_IPTALEDILDI_INT:
                    {
                        durumStr = ProjeConstants.RANDEVU_DURUMU_IPTALEDILDI;
                        break;
                    }
                default:
                    break;
            }
            return durumStr;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToExcel();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void ExportToExcel()
        {

            string filename = "FaaliyetKarti" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
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
        protected void RandevuListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_RANDEVU_LIST);
        }
        protected void RandevuTakvimiBtn_Click(object sender, EventArgs e)
        {
            Randevu randevu = new Randevu();
            randevu = randevu.Select(RandevuIdQS.ConvertToInt());
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM + "?InitialDate=" + randevu.BaslangicTarihi.ToString("yyyy-MM-dd"));
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
        protected void RandevuyaGitBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_RANDEVU_GIRIS + "?RandevuId=" + RandevuIdQS);
        }
    }
}
