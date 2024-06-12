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


namespace TBYS_WebParts.TasinmazBagisciKartiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazBagisciKartiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazBagisciKartiWP()
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
        private string BagisciIdQS
        {
            get
            {

                if (ViewState["BagisciId"] == null)
                {
                    if (Page.Request.QueryString["BagisciId"] != null)
                    {
                        ViewState["BagisciId"] = Page.Request.QueryString["BagisciId"];
                    }
                    else
                    {
                        ViewState["BagisciId"] = string.Empty;
                    }
                }
                return ViewState["BagisciId"].ToString();
            }

            set
            {
                ViewState["BagisciId"] = value;
            }
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

        protected void Page_Load(object sender, EventArgs e)
        {
            TasinmazBagisci bagisci = new TasinmazBagisci();
            bagisci = bagisci.Select<TasinmazBagisci>(BagisciIdQS.ConvertToInt());
            if (bagisci != null)
            {
                BagisciBilgileriniDoldur(bagisci);
                TasinmazListesiniDoldur(bagisci);
                BagisciTalepleriniDoldur(bagisci);
                BagisciYakinlariniDoldur(bagisci);
                TaahhutleriDoldur(bagisci);
                if (!SenderAppQS.Equals("TBL") )
                {
                    TasinmazBagisciListesiBtn.Visible = false;
                }
            }
        }
        private void BagisciBilgileriniDoldur(TasinmazBagisci bagisci)
        {

            BagisciBilgileriTable.Rows.Clear();
            BagisciBilgileriTable.BorderWidth = 2;
            BagisciTalepleriTable.Rows.Clear();
            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.CssClass = "table-dark";
            TableHeaderCell TabloBaslikCell = new TableHeaderCell();
            TabloBaslikCell.ColumnSpan = 4;
            TabloBaslikCell.Text = "Bağışçı Bilgileri";
            headerRow.Controls.Add(TabloBaslikCell);

            headerRow.Controls.Add(TabloBaslikCell);
            TableRow row1 = new TableRow();
            TableRow row2 = new TableRow();
            TableRow row3 = new TableRow();

            TableCell r1c1 = new TableCell();
            TableCell r1c2 = new TableCell();
            TableCell r1c3 = new TableCell();
            TableCell r1c4 = new TableCell();
            r1c4.RowSpan = 3;

            
            Image DisplayImage = new Image();
            DisplayImage.ImageUrl =UtilityHelper.GetImageUrl(ProjeConstants.RESIMLER_BAGISCI) + "/_t/" + bagisci.Adi.ReplaceTrChars() + bagisci.Soyadi.ReplaceTrChars() + "_jpg.jpg";
            DisplayImage.Attributes["onerror"] = "this.src='" + UtilityHelper.GetImageUrl(ProjeConstants.RESIMLER_BAGISCI) + "/_t/bagisci_jpg.jpg';";
            r1c4.Controls.Add(DisplayImage);
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

            r1c3.Text = SagVefatGetir(bagisci);// "Sağ mı : " + bagisci.Sag_vefat;
            r2c3.Text = "Doğum Tarihi : " + bagisci.DogumTarihi.ConvertToDatetimeEmptyIfNull();
            r3c3.Text = "Doğum Yeri : " + bagisci.DogumYeri;

            BagisciBilgileriTable.Controls.Add(headerRow);
            BagisciBilgileriTable.Controls.Add(row1);
            BagisciBilgileriTable.Controls.Add(row2);
            BagisciBilgileriTable.Controls.Add(row3);

        }

        private string SagVefatGetir(TasinmazBagisci bagisci)
        {
            string retval=string.Empty;
            string sag_vefat = bagisci.Sag_vefat;
            if (sag_vefat.Equals(ProjeConstants.BAGISCI_SAG))
            {
                retval = "Sağ mı : " + bagisci.Sag_vefat;
            }
            else
            {
                string vefatTar = string.IsNullOrEmpty(bagisci.VefatTarihi.ConvertToDatetimeEmptyIfNull()) ? string.Empty : " (Tarih: " + bagisci.VefatTarihi.ConvertToDatetimeEmptyIfNull() +")";
                string definBilgisi = string.IsNullOrEmpty(bagisci.DefinIli.Trim()+bagisci.DefinIlcesi.Trim()+bagisci.DefinYeri.Trim() + bagisci.DefinAciklama.Trim()) ? 
                    string.Empty :
                    "<br>" + "Defin Bilgisi: " + bagisci.Ili + " - " + bagisci.Ilcesi + " " + bagisci.DefinYeri + " " + bagisci.Aciklama;
                retval = "Sağ mı : Vefat " + vefatTar + definBilgisi;
            }
            return retval;
        }

        private void TasinmazlarListesiHeaders()
        {
            TasinmazTable.Rows.Clear();
            TasinmazTable.BorderWidth = 2;
            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.CssClass = "table-dark";
            TableHeaderCell TabloBaslikCell = new TableHeaderCell();
            TabloBaslikCell.ColumnSpan = 4;
            TabloBaslikCell.Text = "Bağışlanan Taşınmazlar";
            headerRow.Controls.Add(TabloBaslikCell);
            TableHeaderRow headerRow1 = new TableHeaderRow();

            //TableHeaderCell siranoCell = new TableHeaderCell();
            //siranoCell.Text = "Sırano";

            TableHeaderCell cinsiCell = new TableHeaderCell();
            cinsiCell.Text = "Cinsi / Kullanım Şekli";
            
            TableHeaderCell iliCell = new TableHeaderCell();
            iliCell.Text = "İl-İlçe";

            TableHeaderCell adresCell = new TableHeaderCell();
            adresCell.Text = "Adres";

            TableHeaderCell mulkiyetCell = new TableHeaderCell();
            mulkiyetCell.Text = "Mülkiyet Şekli";

            TableHeaderCell kullanimCell = new TableHeaderCell();
            kullanimCell.Text = "Mülk.Şekli/Kira Durumu";

            //headerRow1.Controls.Add(siranoCell);
            headerRow1.Controls.Add(cinsiCell);
            headerRow1.Controls.Add(iliCell);
            headerRow1.Controls.Add(adresCell);
            //headerRow1.Controls.Add(mulkiyetCell);
            headerRow1.Controls.Add(kullanimCell);

            TasinmazTable.Controls.Add(headerRow);
            TasinmazTable.Controls.Add(headerRow1);

        }
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        private void TasinmazListesiniDoldur(TasinmazBagisci bagisci)
        {
            TitleLbl.Text = bagisci.Adi + " " + bagisci.Soyadi + " Bağışçı Bilgileri";

            Bagis bagis = new Bagis();
            DataTable dataTable = bagis.SelectTasinmazByBagisciIdReturnDT(bagisci.Id);
            if (dataTable != null)
            {
                TasinmazTable.Rows.Clear();
                TasinmazlarListesiHeaders();
                foreach (DataRow dataRow in dataTable.Rows)
                {

                    string sira = dataRow["Sirano"].ToString();
                    string cinsi = dataRow["Cinsi"].ToString();
                    string ilIlce = dataRow["IlIlce"].ToString();
                    string adres = dataRow["Adres"].ToString();
                    string mulkiyetSekli = dataRow["MulkiyetSekli"].ToString();
                    string kullanimSekli = dataRow["KullanimSekli"].ToString();
                    string kiraDurumu = dataRow["KiraDurumu"].ToString();

                    TableRow row = new TableRow();

                    //TableCell SiraNoCell = new TableCell();
                    //SiraNoCell.Text = sira;
                    //row.Controls.Add(SiraNoCell);

                    TableCell CinsiCell = new TableCell();
                    CinsiCell.Text = cinsi + " / " +kullanimSekli;
                    row.Controls.Add(CinsiCell); 
                    

                    TableCell IlICell = new TableCell();
                    IlICell.Text = ilIlce;
                    row.Controls.Add(IlICell);

                    TableCell AdresCell = new TableCell();
                    AdresCell.Text = adres;
                    row.Controls.Add(AdresCell);

                    //TableCell MulkiyetCell = new TableCell();
                    //MulkiyetCell.Text = mulkiyetSekli;
                    //row.Controls.Add(MulkiyetCell);

                    TableCell KullanimCell = new TableCell();
                    KullanimCell.Text = mulkiyetSekli + " * " + kiraDurumu;
                    row.Controls.Add(KullanimCell);

                    TasinmazTable.Controls.Add(row);

                }
                TableFooterRow footerRow = new TableFooterRow();
                footerRow.CssClass = "table-dark";
                TableCell tahminiRayicCell = new TableCell();
                tahminiRayicCell.ColumnSpan = 4;
                tahminiRayicCell.Text = "Tahmini Rayiç Bedelleri Toplamı : " + bagis.SelectSumTahminiRayicByBagisciId(bagisci.Id).ReturnZeroIfNull().ConvertToDecimal().ToString("N", culturInfo);
                footerRow.Controls.Add(tahminiRayicCell);
                TasinmazTable.Controls.Add(footerRow);
            }
        }
        private void BagisciTalepleriniDoldur(TasinmazBagisci bagisci)
        {
            BagisciTalepleri bt = new BagisciTalepleri();
            List<BagisciTalepleri> bagisciListesi = bt.SelectByBagisciId(bagisci.Id);
            BagisciTalepleriTable.Rows.Clear();
            BagisciTalepleriTable.BorderWidth = 2;
            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.CssClass = "table-dark";
            TableHeaderCell TabloBaslikCell = new TableHeaderCell();
            TabloBaslikCell.ColumnSpan = 4;
            TabloBaslikCell.Text = "Bağışçı Talepleri";
            headerRow.Controls.Add(TabloBaslikCell);
            TableHeaderRow headerRow1 = new TableHeaderRow();
            if (bagisciListesi.Count < 1)
            {
                TableHeaderCell cell1 = new TableHeaderCell();
                cell1.Text = "Bağışçı talebi bulunmamaktadır.";
                headerRow1.Controls.Add(cell1);
            }
            else
            {
                TableHeaderCell cell1 = new TableHeaderCell();
                cell1.Text = "Talep";
                TableHeaderCell cell2 = new TableHeaderCell();
                cell2.Text = "İrtibat";
                TableHeaderCell cell3 = new TableHeaderCell();
                cell3.Text = "Uyg.Zamanı";
                TableHeaderCell cell4 = new TableHeaderCell();
                cell4.Text = "Açıklama";
                headerRow1.Controls.Add(cell1);
                headerRow1.Controls.Add(cell2);
                headerRow1.Controls.Add(cell3);
                headerRow1.Controls.Add(cell4);
            }


            BagisciTalepleriTable.Controls.Add(headerRow);
            BagisciTalepleriTable.Controls.Add(headerRow1);
            foreach (BagisciTalepleri item in bagisciListesi)
            {
                TableRow row = new TableRow();
                TableCell talepCell = new TableCell();
                talepCell.Text = item.Talep;
                TableCell irtibatCell = new TableCell();
                irtibatCell.Text = item.Irtibat;
                TableCell zamanCell = new TableCell();
                zamanCell.Text = item.Tarih;
                TableCell aciklamaCell = new TableCell();
                aciklamaCell.Text = item.Aciklama;
                row.Controls.Add(talepCell);
                row.Controls.Add(irtibatCell);
                row.Controls.Add(zamanCell);
                row.Controls.Add(aciklamaCell);
                BagisciTalepleriTable.Controls.Add(row);
            }
        }
        private void BagisciYakinlariniDoldur(TasinmazBagisci bagisci)
        {
            BagisciYakinlari bt = new BagisciYakinlari();
            List<BagisciYakinlari> bagisciListesi = bt.SelectByBagisciId(bagisci.Id);
            BagisciYakinlariTable.Rows.Clear();
            BagisciYakinlariTable.BorderWidth = 2;
            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.CssClass = "table-dark";
            TableHeaderCell TabloBaslikCell = new TableHeaderCell();
            TabloBaslikCell.ColumnSpan = 4;
            TabloBaslikCell.Text = "Bağışçı Yakınları";
            headerRow.Controls.Add(TabloBaslikCell);
            TableHeaderRow headerRow1 = new TableHeaderRow();
            if (bagisciListesi.Count < 1)
            {
                TableHeaderCell cell1 = new TableHeaderCell();
                cell1.Text = "Bağışçı yakınlarına ait bilgi bulunmamaktadır.";
                headerRow1.Controls.Add(cell1);
            }
            else
            {
                TableHeaderCell cell1 = new TableHeaderCell();
                cell1.Text = "Ad-Soyad";
                TableHeaderCell cell2 = new TableHeaderCell();
                cell2.Text = "Yakınlık Derecesi";
                TableHeaderCell cell3 = new TableHeaderCell();
                cell3.Text = "Telefon";
                headerRow1.Controls.Add(cell1);
                headerRow1.Controls.Add(cell2);
                headerRow1.Controls.Add(cell3);
            }

            BagisciYakinlariTable.Controls.Add(headerRow);
            BagisciYakinlariTable.Controls.Add(headerRow1);
            foreach (BagisciYakinlari item in bagisciListesi)
            {
                TableRow row = new TableRow();
                TableCell talepCell = new TableCell();
                talepCell.Text = item.AdSoyad;
                TableCell irtibatCell = new TableCell();
                irtibatCell.Text = item.Telefon;
                TableCell yakinlikDerecesiCell = new TableCell();
                yakinlikDerecesiCell.Text = item.YakinlikDerecesi;

                row.Controls.Add(talepCell);
                row.Controls.Add(yakinlikDerecesiCell);
                row.Controls.Add(irtibatCell);

                BagisciYakinlariTable.Controls.Add(row);
            }
        }
        private void TaahhutleriDoldur(TasinmazBagisci bagisci)
        {
            TasinmazTaahhut bt = new TasinmazTaahhut();
            List<TasinmazTaahhut> taahhutListesi = bt.SelectByBagisciId(bagisci.Id);
            TaahhutTable.Rows.Clear();
            TaahhutTable.BorderWidth = 2;
            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.CssClass = "table-dark";
            TableHeaderCell TabloBaslikCell = new TableHeaderCell();
            TabloBaslikCell.ColumnSpan = 4;
            TabloBaslikCell.Text = "Bağışçıya Verilen Taahhütler";
            headerRow.Controls.Add(TabloBaslikCell);
            TableHeaderRow headerRow1 = new TableHeaderRow();
            if (taahhutListesi.Count < 1)
            {
                TableHeaderCell cell1 = new TableHeaderCell();
                cell1.Text = "Taahhüt bulunmamaktadır.";
                headerRow1.Controls.Add(cell1);
            }
            else
            {
                TableHeaderCell cell1 = new TableHeaderCell();
                cell1.Text = "Adı Soyadı";
                TableHeaderCell cell2 = new TableHeaderCell();
                cell2.Text = "İl-İlçe";
                TableHeaderCell cell3 = new TableHeaderCell();
                cell3.Text = "Adres";
                TableHeaderCell cell4 = new TableHeaderCell();
                cell4.Text = "Açıklama";
                headerRow1.Controls.Add(cell1);
                headerRow1.Controls.Add(cell2);
                headerRow1.Controls.Add(cell3);
                headerRow1.Controls.Add(cell4);
            }


            TaahhutTable.Controls.Add(headerRow);
            TaahhutTable.Controls.Add(headerRow1);

            TasinmazTaahhut tt = new TasinmazTaahhut();
            List<TasinmazTaahhut> list = tt.SelectByBagisciId(bagisci.Id);
            foreach (TasinmazTaahhut item in list)
            {
                TableRow row = new TableRow();

                TableCell AdiSoyadiCell = new TableCell();
                AdiSoyadiCell.Text = item.Adi + " " + item.Soyadi;
                row.Controls.Add(AdiSoyadiCell);

                TableCell IliIlcesiCell = new TableCell();
                Ilce ilce = new Ilce();
                ilce = ilce.Select<Ilce>(item.Ilcesi);
                IliIlcesiCell.Text = ilce != null ? ilce.IlAdi + "/" + ilce.IlceAdi : string.Empty;
                row.Controls.Add(IliIlcesiCell);

                TableCell AdresCell = new TableCell();
                AdresCell.Text = item.Adres;
                row.Controls.Add(AdresCell);

                TableCell AciklamaCell = new TableCell();
                AciklamaCell.Text = item.TaahhutAciklama;
                AciklamaCell.Width = new Unit("40%");
                row.Controls.Add(AciklamaCell);

                TaahhutTable.Controls.Add(row);
            }
        }
        protected void ExportToExcel()
        {
            TasinmazBagisci bagisci = new TasinmazBagisci();
            bagisci = bagisci.Select<TasinmazBagisci>(BagisciIdQS.ConvertToInt());
            string filename = "TasinmazBagisciKarti" + bagisci.Adi.ReplaceTrChars() + bagisci.Soyadi.ReplaceTrChars() + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            BagisciCard.RenderControl(hw);
            //Write the HTML back to the browser.
            //Response.ContentType = application/vnd.ms-excel;
            Page.Response.ContentType = "application/ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            string s = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\n<head>\n<title></title>\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1254\" />\n<style>\n</style>\n</head>\n<body>\n";
            Page.Response.Write(s + tw.ToString());
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
        protected void TasinmazBagisciListesiBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (SenderAppQS.Equals("TD") && !string.IsNullOrEmpty(TasinmazIdQS))
                {
                    RedirectToPage(ProjeConstants.PAGE_TASINMAZ_GIRIS + "?DestinationApp=TD&TasinmazId=" + TasinmazIdQS);
                }
                else
                {
                    RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_LIST + "?SecilenId=" + BagisciIdQS);
                }

            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            TasinmazBagisci bagisci = new TasinmazBagisci();
            bagisci = bagisci.Select<TasinmazBagisci>(BagisciIdQS.ConvertToInt());
            BagisciBilgileriniDoldur(bagisci);
            TasinmazListesiniDoldur(bagisci);
            BagisciTalepleriniDoldur(bagisci);
            BagisciYakinlariniDoldur(bagisci);
            ExportToExcel();
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                int index = currentUrl.LastIndexOf("?") < 0 ? currentUrl.Length : currentUrl.LastIndexOf("?");

                string rawUrl = currentUrl.Substring(0, index);
                string newUrl = rawUrl.Substring(0, rawUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
    }
}
