using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.SharePoint;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.TesekkurBelgesiYazisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TesekkurBelgesiYazisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TesekkurBelgesiYazisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        private string SecilenGunQS
        {
            get
            {

                if (ViewState["SecilenGun"] == null)
                {
                    if (Page.Request.QueryString["SecilenGun"] != null)
                    {
                        ViewState["SecilenGun"] = Page.Request.QueryString["SecilenGun"];
                    }
                    else
                    {
                        ViewState["SecilenGun"] = string.Empty;
                    }
                }
                return ViewState["SecilenGun"].ToString();
            }

            set
            {
                ViewState["SecilenGun"] = value;
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
        private string SecilenBastarQS
        {
            get
            {

                if (ViewState["SecilenBastar"] == null)
                {
                    if (Page.Request.QueryString["SecilenBastar"] != null)
                    {
                        ViewState["SecilenBastar"] = Page.Request.QueryString["SecilenBastar"];
                    }
                    else
                    {
                        ViewState["SecilenBastar"] = string.Empty;
                    }
                }
                return ViewState["SecilenBastar"].ToString();
            }

            set
            {
                ViewState["SecilenBastar"] = value;
            }
        }
        private string SecilenBittarQS
        {
            get
            {

                if (ViewState["SecilenBittar"] == null)
                {
                    if (Page.Request.QueryString["SecilenBittar"] != null)
                    {
                        ViewState["SecilenBittar"] = Page.Request.QueryString["SecilenBittar"];
                    }
                    else
                    {
                        ViewState["SecilenBittar"] = string.Empty;
                    }
                }
                return ViewState["SecilenBittar"].ToString();
            }

            set
            {
                ViewState["SecilenBittar"] = value;
            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            //time out olmasın diye
            ScriptManager _scriptMan = ScriptManager.GetCurrent(Page);
            _scriptMan.AsyncPostBackTimeout = 36000;
            if (!Page.IsPostBack)
            {
                FillDropDownList();
                SetDDLValues(); //ay ve yılı querystringden al
                SetSecilenBasTarBitTar();
                FormuDoldur();
                FillDurumValues();
            }
            TabloOlustur();
        }
        private void FormuDoldur()
        {
            DateTime bugun = DateTime.Today;

            ImzalayanTxt.Text = @"Bilal TOPÇU";
            ImzalayanUnvanTxt.Text = string.Empty;
            ImzalayanMakamTxt.Text = @"Genel Müdür";
            EvrakTarihiTxt.Text = bugun.ToString("dd") + " " + bugun.ToString("MMMM") + " " + bugun.Year;
        }
        private void FillDropDownList()
        {
            GunDDLDoldur();
            AyDDLDoldur();
            YilDDLDoldur();
            BolgeDDLDoldur();
        }
        private void GunDDLDoldur()
        {
            //ARMAGAN PERIODU
            //10 Günde bir
            GunDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Tüm Ay", "0"));
            GunDDL.Items.Add(new System.Web.UI.WebControls.ListItem("1-7", "1"));
            GunDDL.Items.Add(new System.Web.UI.WebControls.ListItem("8-14", "2"));
            GunDDL.Items.Add(new System.Web.UI.WebControls.ListItem("15-22", "3"));
            GunDDL.Items.Add(new System.Web.UI.WebControls.ListItem("23-Ay Sonu", "4"));

            //15 Günde bir
            //GunDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Tüm Ay", "0"));
            //GunDDL.Items.Add(new System.Web.UI.WebControls.ListItem("1-15", "1"));
            //GunDDL.Items.Add(new System.Web.UI.WebControls.ListItem("16-Ay Sonu", "2"));

            //Ayda bir
            //GunDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Tüm Ay", "0"));
        }
        private void AyDDLDoldur()
        {

            AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Ocak", "1"));
            AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Şubat", "2"));
            AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Mart", "3"));
            AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Nisan", "4"));
            AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Mayıs", "5"));
            AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Haziran", "6"));
            AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Temmuz", "7"));
            AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Ağustos", "8"));
            AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Eylül", "9"));
            AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Ekim", "10"));
            AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Kasım", "11"));
            AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Aralık", "12"));

        }
        private void YilDDLDoldur()
        {
            var year = DateTime.Now.Year;
            for (int i = 1987; i <= year; i++)
            {
                YilDDL.Items.Add(new System.Web.UI.WebControls.ListItem(i.ToString(), i.ToString()));
            }
        }
        private void BolgeDDLDoldur()
        {
            BolgeDDL.Items.Clear();
            Bolge bolgeDao = new Bolge();
            List<Bolge> list = bolgeDao.SelectAktifBolgeler(ProjeConstants.BOLGE_HEPSI_INT);
            foreach (Bolge item in list)
            {
                if (string.IsNullOrEmpty(item.Adi.Trim()))
                    continue;
                BolgeDDL.Items.Add(new System.Web.UI.WebControls.ListItem(item.Adi, item.Id.ToString()));
            }

        }
        private void SetDDLValues()
        {
            try
            {
                //acilista ay ve yili querystring ile gelen ay ve yıla eşitle boş geldiyse gecen aya/yila eşitle
                //gün
                int gunBolumu = DateTime.Today.Day < 15 ? 1 : 2;
                // Periyod 15 ise
                //string gun = !string.IsNullOrEmpty(SecilenGunQS) ? SecilenGunQS : gunBolumu.ToString();
                //System.Web.UI.WebControls.ListItem gunItem = new System.Web.UI.WebControls.ListItem();
                //if (!string.IsNullOrEmpty(gun))
                //    gunItem = GunDDL.Items.FindByValue(gun);

                //if (gunItem != null)
                //{
                //    GunDDL.SelectedValue = gunItem.Value;
                //    SecilenGunQS = gunItem.Value;
                //}
                //ay
                string ay = !string.IsNullOrEmpty(SecilenAyQS) ? SecilenAyQS : (gunBolumu == 1 ? DateTime.Today.AddMonths(-1).Month.ToString() : DateTime.Today.Month.ToString());
                System.Web.UI.WebControls.ListItem AyItem = new System.Web.UI.WebControls.ListItem();
                if (!string.IsNullOrEmpty(ay))
                    AyItem = AyDDL.Items.FindByValue(ay);

                if (AyItem != null)
                {
                    AyDDL.SelectedValue = AyItem.Value;
                    SecilenAyQS = AyItem.Value;
                }

                //yil
                string yil = !string.IsNullOrEmpty(SecilenYilQS) ? SecilenYilQS : DateTime.Today.Year.ReturnEmptyIfNull().ToString();
                System.Web.UI.WebControls.ListItem YilItem = new System.Web.UI.WebControls.ListItem();
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
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenAyQS = AyDDL.SelectedItem.Value.ToString();
            SetSecilenBasTarBitTar();
            FillDurumValues();
            //KayitGetir();
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
            SetSecilenBasTarBitTar();
            FillDurumValues();
            //KayitGetir();
        }
        protected void GunDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenGunQS = GunDDL.SelectedItem.Value.ToString();
            SetSecilenBasTarBitTar();
            FillDurumValues();
        } 
        protected void BolgeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetSecilenBasTarBitTar();
            FillDurumValues();
        }
        private void SetSecilenBasTarBitTar()
        {
            string gunStr = GunDDL.SelectedItem.Value;
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();

            DateTime basTar = DateTime.Today;
            DateTime bitTar = DateTime.Today;
            //ARMAGAN PERIODU
            //15 Günde bir
            //if (gunStr.Equals("0"))
            //{
            //    basTar = new DateTime(yil, ay, 1);
            //    int songun = basTar.AddMonths(1).AddDays(-1).Day;
            //    bitTar = new DateTime(yil, ay, songun);
            //}
            //else if (gunStr.Equals("1"))
            //{
            //    basTar = new DateTime(yil, ay, 1);
            //    bitTar = new DateTime(yil, ay, 10);
            //}
            //else if (gunStr.Equals("2"))
            //{
            //    basTar = new DateTime(yil, ay, 11);
            //    bitTar = new DateTime(yil, ay, 20);
            //}
            //else if (gunStr.Equals("3"))
            //{
            //    basTar = new DateTime(yil, ay, 21);
            //    DateTime basGun = new DateTime(yil, ay, 1).AddMonths(1).AddDays(-1);
            //    bitTar = new DateTime(yil, ay, basGun.Day);
            //}

            //15 Günde bir
            if (gunStr.Equals("0"))
            {
                basTar = new DateTime(yil, ay, 1);
                int songun = basTar.AddMonths(1).AddDays(-1).Day;
                bitTar = new DateTime(yil, ay, songun);
            }
            else if (gunStr.Equals("1"))
            {
                basTar = new DateTime(yil, ay, 1);
                bitTar = new DateTime(yil, ay, 15);
            }
            else if (gunStr.Equals("2"))
            {
                basTar = new DateTime(yil, ay, 16);
                DateTime basGun = new DateTime(yil, ay, 1).AddMonths(1).AddDays(-1);
                bitTar = new DateTime(yil, ay, basGun.Day);
            }
            SecilenBastarQS = basTar.ConvertToDatetimeEmptyIfNull();
            SecilenBittarQS = bitTar.ConvertToDatetimeEmptyIfNull();
        }
        private void TabloOlustur()
        {
            var jsonData = GetData(); //veri çekilip json a çeviriliyor

            bool jasonDataBosMu = string.IsNullOrWhiteSpace(jsonData.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", ""));
            if (!jasonDataBosMu)
            {
                DosyaOlusturBtn.Visible = true;
                var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
                UtilityHelper.ScriptCalistir(jsString);
            }
            else
            {
                TableDataLbl.Text = "Teşekkür Belgesi bulunmamaktadır.";
                DosyaOlusturBtn.Visible = false;
            }
        }
        private string GetData()
        {
            Armagan armagan = new Armagan();

            int rowCount = 0;
            DateTime bastar = GetBasTar();
            DateTime bittar = GetBitTar();

            var json = armagan.SelectByDurumTarih(ProjeConstants.DURUM_KONTROLEDILDI, bastar, bittar, ProjeConstants.ARMAGAN_TESEKKURID.ToString(), ref rowCount, BolgeDDL.SelectedItem.Value.ConvertToInt(), ProjeConstants.HEPSI_INT);
            TableDataLbl.Text = rowCount + " adet Teşekkür Belgesi mevcut";
            if (rowCount > 0)
            {
                DosyaOlusturBtn.Visible = true;
                TesekkurDurumChk.Visible = true;
            }
            else
            {
                DosyaOlusturBtn.Visible = false;
                TesekkurDurumChk.Visible = false;
            }
            return json;
        }

       
        private string CreateDataTable(string jsonData)
        {

            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date

                jQuery('#CustomDataTable').DataTable({ 
                    data: " + jsonData + @",
                    columns: [
                        { data: 'NakitBagisciAdi'},
                        { data: 'BelgedeYazanIsim', 'width':'20%' },
                        { data: 'NakitBagisciTC' },
                        { data: 'ArmaganTarihi' },
                        { data: 'ArmaganTutari', 'width':'10%', 'className': 'text-right'},

                    ],
                    pageLength: 8,
                    'order': [[3, 'desc']],//sort date desc
                    'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    dom: 'frtip',   

                });

            ";

            return tableString;
        }

        //
        private DateTime GetBasTar()
        {

            string gunStr = GunDDL.SelectedItem.Value;
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();

            DateTime bastar = DateTime.Today;

            if (ay == 0)
            {
                bastar = new DateTime(yil, 1, 1);
            }
            else
            {
                //ARMAGAN PERIODU
                //15 Günde bir
                //if (gunStr.Equals("0"))
                //{
                //    bastar = new DateTime(yil, ay, 1);
                //}
                //else if (gunStr.Equals("1"))
                //{
                //    bastar = new DateTime(yil, ay, 1);
                //}
                //else
                //{
                //    bastar = new DateTime(yil, ay, 16);
                //}

                // 10 Günde bir
                if (gunStr.Equals("0"))
                {
                    bastar = new DateTime(yil, ay, 1);//hepsi
                }
                else if (gunStr.Equals("1"))
                {
                    bastar = new DateTime(yil, ay, 1);
                }
                else if (gunStr.Equals("2"))
                {
                    bastar = new DateTime(yil, ay, 8);
                }
                else if (gunStr.Equals("3"))
                {
                    bastar = new DateTime(yil, ay, 15);
                } 
                else if (gunStr.Equals("4"))
                {
                    bastar = new DateTime(yil, ay, 23);
                }
            }
            return bastar;
        }
        private DateTime GetBitTar()
        {
            string gunStr = GunDDL.SelectedItem.Value;
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();

            DateTime bittar = DateTime.Today;

            if (ay == 0)
            {
                bittar = (new DateTime(yil, 1, 1)).AddYears(1).AddDays(-1);
            }
            else
            {
                //ARMAGAN PERIODU
                //15 Günde bir
                //if (gunStr.Equals("0"))
                //{
                //    int songun = (new DateTime(yil, ay, 1)).AddMonths(1).AddDays(-1).Day;
                //    bittar = new DateTime(yil, ay, songun);
                //}
                //else if (gunStr.Equals("1"))
                //{
                //    bittar = new DateTime(yil, ay, 15);
                //}
                //else
                //{
                //    DateTime sonGun = new DateTime(yil, ay, 1).AddMonths(1).AddDays(-1);
                //    bittar = new DateTime(yil, ay, sonGun.Day);
                //}

                // 10 Günde bir
                if (gunStr.Equals("0"))
                {
                    bittar = new DateTime(yil, ay, 1).AddMonths(1).AddDays(-1);
                }
                else if (gunStr.Equals("1"))
                {
                    bittar = new DateTime(yil, ay, 7);
                }
                else if (gunStr.Equals("2"))
                {
                    bittar = new DateTime(yil, ay, 14);
                }
                else if (gunStr.Equals("3"))
                {
                    bittar = new DateTime(yil, ay, 22);
                }
                else if (gunStr.Equals("4"))
                {
                    bittar = new DateTime(yil, ay, 1).AddMonths(1).AddDays(-1);
                }

            }
            return bittar;
        }
        private void FillDurumValues()
        {

            Armagan armagan = new Armagan();
            //DataTable tesekkur = armagan.SelectCountDurumByBolge(SecilenAyQS, SecilenYilQS, ProjeConstants.ARMAGAN_TESEKKURID, "");
            DataTable dataTable = armagan.SelectCountDurumByBolgeTarih(ProjeConstants.ARMAGAN_TESEKKURID, BolgeDDL.SelectedItem.Value.ConvertToInt(), SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime());
            FillTable(dataTable);

        }
        private void FillTable(DataTable dataTable)
        {
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string durum = row["Durum"].ReturnEmptyIfNull().ToString();
                    string adet = row["Adet"].ReturnEmptyIfNull().ToString();
                    if (durum == ProjeConstants.DURUM_KONTROLEDILDI)
                    {
                        if (adet.ConvertToInt() > 0)
                        {
                            DosyaOlusturBtn.Visible = true;
                            TesekkurDurumChk.Visible = true;
                            //durumChk.Enabled = true;
                        }
                        else
                        {
                            DosyaOlusturBtn.Visible = false;
                            TesekkurDurumChk.Visible = false;
                        }

                    }
                    else if (durum == ProjeConstants.DURUM_GONDERILDI)
                    {
                        if (adet.ConvertToInt() > 0)
                        {
                            //etiketBtn.Enabled = true;
                        }

                    }

                    System.Web.UI.WebControls.TableRow tblrow = new System.Web.UI.WebControls.TableRow();
                    System.Web.UI.WebControls.TableCell durumCell = new System.Web.UI.WebControls.TableCell();
                    durumCell.Text = durum;
                    tblrow.Controls.Add(durumCell);

                    System.Web.UI.WebControls.TableCell adetCell = new System.Web.UI.WebControls.TableCell();
                    adetCell.Text = adet;
                    tblrow.Controls.Add(adetCell);
                    TesekkurTable.Controls.Add(tblrow);
                }
            }

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
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void DosyaOlusturBtn_Click(object sender, EventArgs e)
        {
            try
            {

                // Dosya adları 
                string bolge = BolgeDDL.SelectedItem.Value;
                string zaman = DateTime.Now.ToString("dd-MM-yyyy-HH-mm");
                string yaziDosyaAdi = "TesekkurBelgesi-" + bolge + "-(" + zaman + ").docx";
                bool isYaziOlusturuldu = TesekkurBelgesiDosyasiOlustur(yaziDosyaAdi);
                if (isYaziOlusturuldu)
                {
                    MessageHelper.PublishMessage("Teşekkür belgeleri hazırlandı, Dosya ismine basarak yazıyı indirebilirsiniz.", ProjeConstants.MESAJ_BASARILI, 2000);
                }
                else
                {
                    MessageHelper.PublishMessage("Teşekkür belgeleri oluşturulamadı.", ProjeConstants.MESAJ_HATA, 3000);
                }
            }
            catch (Exception ex)
            {
                Exception ex1 = new Exception("Belge oluşturmada hata");
                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.PublishException();
            }
        }
        protected void AdresOlusturBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Dosya adları 
                string bolge = BolgeDDL.SelectedItem.Value;
                string zaman = DateTime.Now.ToString("dd-MM-yyyy-HH-mm");
                string etiketDosyaAdi = "Adres-EtiketiTES-"+bolge+"-(" + zaman + ").docx";

                bool etiketOlustuMu = YeniAdresEtiketDosyasiOlustur(etiketDosyaAdi);
                if (etiketOlustuMu)
                    MessageHelper.PublishMessage("TAdres etiketleri hazırlandı, Dosya ismine basarak yazıyı indirebilirsiniz.", ProjeConstants.MESAJ_BASARILI, 2000);
                else
                {
                    MessageHelper.PublishMessage("Adres etiketleri oluşturulamadı.", ProjeConstants.MESAJ_HATA, 3000);
                }
            }
            catch (Exception ex)
            {
                Exception ex1 = new Exception("Yazı ve Adres oluşturmada hata");
                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.PublishException();
            }
        }
        private bool YeniAdresEtiketDosyasiOlustur(string dosyaAdi)
        {
            bool isYaziOlusturuldu = false;
            try
            {
                MemoryStream templateStream = GetTemplateStream("AdresEtiketiTemplate.docx");
                IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Table> templateTables = GetTemplateTables(templateStream);
                MemoryStream destinationStream = AddAdresEtiketData2DestinationStream(templateStream, templateTables);

                destinationStream.Seek(0, SeekOrigin.Begin);
                destinationStream.Position = 0;

                AddToSharePoint(destinationStream, dosyaAdi);

                string dosyaUrl = SPContext.Current.Web.Url + @"/" + ProjeConstants.NBYSBELGELERI_LIB + @"/" + dosyaAdi;

                string sourceString = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                string removeString = System.Web.HttpContext.Current.Request.Url.AbsolutePath;

                int index = sourceString.IndexOf(removeString);
                string rootUrl = (index < 0)
                    ? sourceString
                    : sourceString.Remove(index, removeString.Length);

                AdresEtiketLnk.Text = dosyaAdi;
                AdresEtiketLnk.NavigateUrl = rootUrl + "/_layouts/15/download.aspx?SourceUrl=" + dosyaUrl;

                AdresEtiketLnk.Visible = true;
                isYaziOlusturuldu = true;
            }
            catch (Exception ex)
            {
                isYaziOlusturuldu = false;
                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.PublishException();
            }
            return isYaziOlusturuldu;
        }
        private MemoryStream GetTemplateStream(string templateFileName)
        {
            string newFileUrl = string.Empty;

            string siteUrl = SPContext.Current.Web.Url;
            using (SPSite spSite = new SPSite(siteUrl))
            {
                SPList list = SPContext.Current.Web.Lists[ProjeConstants.NBYSBELGELERI_LIB];
                SPQuery query = new SPQuery();
                query.ViewFields = @"<FieldRef Name='FileLeafRef' />";
                query.Query =
                  @"<Where>
                          <Eq>
                            <FieldRef Name='FileLeafRef' />
                            <Value Type='File'>" + templateFileName + @"</Value>
                          </Eq>
                        </Where>";
                SPListItemCollection collection = list.GetItems(query);
                MemoryStream memStr = new MemoryStream();
                if (collection.Count > 0)
                {
                    SPFile file = collection[0].File;
                    byte[] byteArray = file.OpenBinary();
                    memStr.Write(byteArray, 0, byteArray.Length);
                }
                return memStr;
            }
        }
        private bool TesekkurBelgesiDosyasiOlustur(string dosyaAdi)
        {
            bool isYaziOlusturuldu = false;
            try
            {
                MemoryStream templateStream = GetTemplateStream();
                IEnumerable<Paragraph> templateParagraphs = GetTemplateParagraphs(templateStream);
                MemoryStream destinationStream = AddData2DestinationStream(templateStream, templateParagraphs);
                destinationStream.Seek(0, SeekOrigin.Begin);
                destinationStream.Position = 0;

                AddToSharePoint(destinationStream, dosyaAdi);
                string dosyaUrl = SPContext.Current.Web.Url + @"/" + ProjeConstants.NBYSBELGELERI_LIB + @"/" + dosyaAdi;

                string sourceString = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                string removeString = System.Web.HttpContext.Current.Request.Url.AbsolutePath;

                int index = sourceString.IndexOf(removeString);
                string rootUrl = (index < 0)
                    ? sourceString
                    : sourceString.Remove(index, removeString.Length);

                DosyaLnk.Text = dosyaAdi;
                DosyaLnk.NavigateUrl = rootUrl + "/_layouts/15/download.aspx?SourceUrl=" + dosyaUrl;
                DosyaLnk.Visible = true;
                isYaziOlusturuldu = true;
                if (TesekkurDurumChk.Checked)
                {
                    Armagan armagan = new Armagan();
                    try
                    {
                        bool isUpdated = armagan.UpdateDurumByBolge(ProjeConstants.DURUM_KONTROLEDILDI, ProjeConstants.DURUM_GONDERILDI, SecilenBastarQS, SecilenBittarQS, ProjeConstants.ARMAGAN_TESEKKURID, ProjeConstants.BOLGE_HEPSI_INT);
                        if (isUpdated)
                        {
                            MessageHelper.PublishMessage("Belgelerin durumu '" + ProjeConstants.DURUM_GONDERILDI + "' olarak değiştirildi.", ProjeConstants.MESAJ_BASARILI, 2000);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHelper exHelper = new ExceptionHelper(ex);
                        exHelper.PublishException();
                    }
                }

                FillDurumValues();

            }
            catch (Exception ex)
            {
                isYaziOlusturuldu = false;
                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.PublishException();
            }
            return isYaziOlusturuldu;
        }
        private MemoryStream AddData2DestinationStream(MemoryStream templateStream, IEnumerable<Paragraph> templateParagraphs)
        {
            MemoryStream destinationStream = null;
            DateTime bastar = GetBasTar();
            DateTime bittar = GetBitTar();
            Armagan armagan = new Armagan();
            DataTable dataTable = armagan.SelectByDurumTarihReturnDT(ProjeConstants.DURUM_KONTROLEDILDI, bastar, bittar, ProjeConstants.ARMAGAN_TESEKKURID.ToString(), BolgeDDL.SelectedItem.Value.ConvertToInt(), ProjeConstants.HEPSI_INT);

            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string nakitBagisciAdi = row["NakitBagisciAdi"].ToString();
                    string belgedeYazanIsim = row["BelgedeYazanIsim"].ToString();
                    bool bagisMiktariYazmasin = row["BagisMiktariYazmasin"].ReturnFalseIfNull().ConvertToBool();
                    string belgeNo = row["ArmaganId"].ToString();
                    string nakitBagisciTC = row["NakitBagisciTC"].ToString();
                    DateTime tarih = row["Tarih"].ConvertToDatetime();

                    decimal tutar = row["Tutar"].ConvertToDecimal();
                    string tutarStr = bagisMiktariYazmasin ? string.Empty : tutar.ToString("N", culturInfo) + " TL'lık";

                    //create key value pair, key represents words to be replace and 
                    //values represent values in document in place of keys.
                    Dictionary<string, string> keyValues = new Dictionary<string, string>();
                    keyValues.Add("BelgeTarihiVar", EvrakTarihiTxt.Text);
                    keyValues.Add("BelgeNoVar", belgeNo);
                    keyValues.Add("BagisciAdiVar", string.IsNullOrEmpty(belgedeYazanIsim) ? nakitBagisciAdi : belgedeYazanIsim);
                    keyValues.Add("BagisTarihiVar", tarih.ToString("dd MMMM yyyy"));
                    keyValues.Add("TutarVar", tutarStr);

                    keyValues.Add("ImzaVar", ImzalayanTxt.Text);
                    keyValues.Add("UnvanVar", ImzalayanUnvanTxt.Text);
                    keyValues.Add("MakamVar", ImzalayanMakamTxt.Text);
                    //destinationStream = SearchAndReplace(templateStream, keyValues);

                    //destinationStream = AddParagraph2DestinationStream(destinationStream, templateParagraphs);
                    destinationStream = CopyAndSearchAndReplace(templateStream, templateParagraphs, keyValues);
                }
            }


            return destinationStream;
        }
        public MemoryStream CopyAndSearchAndReplace(MemoryStream destinationStream, IEnumerable<Paragraph> templateParagraphs, Dictionary<string, string> dict)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(destinationStream, true))
            {

                string docText = null;
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }
                foreach (KeyValuePair<string, string> item in dict)
                {
                    Regex regexText = new Regex(item.Key);
                    docText = regexText.Replace(docText, item.Value);
                }
                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
                //boş sayfa ekle
                Paragraph PageBreakParagraph = new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break() { Type = BreakValues.Page }));
                wordDoc.MainDocumentPart.Document.Body.Append(PageBreakParagraph);

                //template yaziyi ekle
                foreach (var paragraph in templateParagraphs)
                {
                    Paragraph newPara = (Paragraph)paragraph.CloneNode(true);// new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break() { Type = BreakValues.Page }));
                    wordDoc.MainDocumentPart.Document.Body.Append(newPara);
                }
                return destinationStream;
            }

        }
        private MemoryStream AddAdresEtiketData2DestinationStream(MemoryStream destinationStream, IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Table> templateTables)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            DateTime bastar = GetBasTar();
            DateTime bittar = GetBitTar();
            Armagan armagan = new Armagan();
            DataTable dataTable = armagan.SelectByDurumTarihReturnDT(ProjeConstants.DURUM_KONTROLEDILDI, bastar, bittar, ProjeConstants.ARMAGAN_TESEKKURID.ToString(), BolgeDDL.SelectedItem.Value.ConvertToInt(), ProjeConstants.HEPSI_INT);
            if (dataTable != null)
            {
                int index = 1;
                List<string> uzunAdresliler = new List<string>();
                foreach (DataRow row in dataTable.Rows)
                {
                    string nakitBagisciAdi = row["NakitBagisciAdi"].ToString();
                    string armaganId = row["ArmaganId"].ToString();
                    string belgedeYazanIsim = row["BelgedeYazanIsim"].ToString();
                    string adres = row == null ? "" : row["Adres"].ToString();
                    string ili = row == null ? "" : row["IlAdi"].ToString();
                    string ilcesi = row == null ? "" : row["IlceAdi"].ToString();
                    string semtIlceIl = ilcesi + @"/" + ili;

                    //create key value pair, key represents words to be replace and 
                    //values represent values in document in place of keys.
                    Dictionary<string, string> keyValues = new Dictionary<string, string>();
                    keyValues.Add("AdSoyad" + index + "Var", nakitBagisciAdi + " -" + armaganId);
                    keyValues.Add("Adres" + index + "Var", adres.Substring(0, adres.Length > 110 ? 110 : adres.Length));
                    keyValues.Add("SemtIlceIl" + index + "Var", semtIlceIl);
                    if (adres.Trim().Length > 100)
                    {
                        uzunAdresliler.Add(nakitBagisciAdi);
                    }
                    SearchAndReplace(destinationStream, keyValues);
                    if (index++ >= 21)//sayfa bitti yeni sayfa ekle
                    {
                        destinationStream = AddTable2DestinationStream(destinationStream, templateTables);
                        index = 1;
                    }

                }
                if (index < 21)
                {
                    //Sayfada 21 den az kayıt varsa template alanlarını temizle adsoyad vs
                    for (int i = index; i < 22; i++)
                    {
                        Dictionary<string, string> keyValues = new Dictionary<string, string>();
                        keyValues.Add("AdSoyad" + i + "Var", "");
                        keyValues.Add("Adres" + i + "Var", "");
                        keyValues.Add("SemtIlceIl" + i + "Var", "");
                        destinationStream = SearchAndReplace(destinationStream, keyValues);
                    }

                }
                if (uzunAdresliler.Count > 0)
                {
                    string message = string.Join(Environment.NewLine, uzunAdresliler);
                    MessageHelper.PublishMessage(message + " adresi çok uzun olduğundan kesilerek kısaltıldı. Lütfen etiketini kontrol ediniz. ", ProjeConstants.MESAJ_BILGI);
                }
            }


            return destinationStream;
        }
        private MemoryStream AddTable2DestinationStream(MemoryStream destinationStream, IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Table> templateTables)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(destinationStream, true))
            {
                //boş sayfa ekle
                Paragraph PageBreakParagraph = new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break() { Type = BreakValues.Page }));
                wordDoc.MainDocumentPart.Document.Body.Append(PageBreakParagraph);

                //template yaziyi ekle
                foreach (var table in templateTables)
                {
                    DocumentFormat.OpenXml.Wordprocessing.Table newPara = (DocumentFormat.OpenXml.Wordprocessing.Table)table.CloneNode(true);// new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break() { Type = BreakValues.Page }));
                    wordDoc.MainDocumentPart.Document.Body.Append(newPara);
                }
            }
            return destinationStream;
        }
        private IEnumerable<Paragraph> GetTemplateParagraphs(MemoryStream templateStream)
        {
            //using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(templateStream, true))
            //{
            try
            {
                WordprocessingDocument wordDoc = WordprocessingDocument.Open(templateStream, true);
                var body = wordDoc.MainDocumentPart.Document.Body;
                var paras = body.Elements<Paragraph>();
                return paras;
            }
            catch (Exception ex)
            {

                ExceptionHelper eh = new ExceptionHelper(ex);
                eh.PublishException();
                return null;
            }
            //}

        }
        private IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Table> GetTemplateTables(MemoryStream templateStream)
        {
            //using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(templateStream, true))
            //{
            WordprocessingDocument wordDoc = WordprocessingDocument.Open(templateStream, true);
            var body = wordDoc.MainDocumentPart.Document.Body;
            var tables = body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>();
            return tables;
            //}

        }
        private MemoryStream GetTemplateStream()
        {
            MemoryStream memStr = new MemoryStream();
            try
            {
                string newFileUrl = string.Empty;

                string siteUrl = SPContext.Current.Web.Url;
                using (SPSite spSite = new SPSite(siteUrl))
                {
                    //Console.WriteLine("Querying for template.docx");
                    SPList list = SPContext.Current.Web.Lists[ProjeConstants.NBYSBELGELERI_LIB];
                    SPQuery query = new SPQuery();
                    query.ViewFields = @"<FieldRef Name='FileLeafRef' />";
                    query.Query =
                      @"<Where>
                          <Eq>
                            <FieldRef Name='FileLeafRef' />
                            <Value Type='File'>TesekkurBelgesiTemplate.docx</Value>
                          </Eq>
                        </Where>";
                    SPListItemCollection collection = list.GetItems(query);
                    if (collection.Count > 0)
                    {
                        SPFile file = collection[0].File;
                        byte[] byteArray = file.OpenBinary();
                        memStr.Write(byteArray, 0, byteArray.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Exception e1 = new Exception("GetTemplateStream() hatası");
                ExceptionHelper eh = new ExceptionHelper(ex);
                eh.Exceptions.Add(e1);
                eh.PublishException();
            }
            return memStr;
        }
        // To search and replace content in a document part.
        public MemoryStream SearchAndReplace(MemoryStream templateStream, Dictionary<string, string> dict)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(templateStream, true))
            {
                string docText = null;
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                foreach (KeyValuePair<string, string> item in dict)
                {
                    Regex regexText = new Regex(item.Key);
                    docText = regexText.Replace(docText, item.Value);
                }

                using (StreamWriter sw = new StreamWriter(
                          wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
                return templateStream;
            }
        }
        protected void AddToSharePoint(MemoryStream memStream, string fileName)
        {
            //string url = System.Web.HttpContext.Current.Request.Url.ToString();
            using (SPSite spSite = new SPSite(SPContext.Current.Web.Url))
            {
                //Get the document library object
                SPList docLib = SPContext.Current.Web.Lists[ProjeConstants.NBYSBELGELERI_LIB];
                SPFile file = docLib.RootFolder.Files.Add(fileName, memStream, true);
                file.Update();
            }
        }
    }
}
