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

namespace NBYS_WebParts.BeratBelgesiYazisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BeratBelgesiYazisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BeratBelgesiYazisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

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
        private string SecilenMadalyaQS
        {
            get
            {

                if (ViewState["SecilenMadalya"] == null)
                {
                    if (Page.Request.QueryString["SecilenMadalya"] != null)
                    {
                        ViewState["SecilenMadalya"] = Page.Request.QueryString["SecilenMadalya"];
                    }
                    else
                    {
                        ViewState["SecilenMadalya"] = string.Empty;
                    }
                }
                return ViewState["SecilenMadalya"].ToString();
            }

            set
            {
                ViewState["SecilenMadalya"] = value;
            }
        }
       
        private int SecilenBolgeIdQS
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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //time out olmasın diye
                ScriptManager _scriptMan = ScriptManager.GetCurrent(Page);
                _scriptMan.AsyncPostBackTimeout = 36000;

                FillDropDownList();
                SetDDLValues(); //ay ve yılı querystringden al
                SetSecilenBasTarBitTar();
                FormuDoldur();

                BeratDurumTablosunuDoldur();
                DosyaTablosunuDoldur();
            }
        }
        private void FormuDoldur()
        {
            DateTime bugun = DateTime.Today;

            ImzalayanTxt.Text = @"Bilal TOPÇU";
            ImzalayanUnvanTxt.Text = string.Empty;// @"(E)Tümgeneral";
            ImzalayanMakamTxt.Text = @"Genel Müdür";
            EvrakTarihiTxt.Text = bugun.ToString("dd") + " " + bugun.ToString("MMMM") + " " + bugun.Year;
        }
        private void FillDropDownList()
        {

            AyDDLDoldur();
            YilDDLDoldur();
            MadalyaDDLDoldur();
            BolgeDDLDoldur();
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
            SecilenBolgeIdQS = SecilenBolgeIdQS==0 ? BolgeDDL.SelectedItem.Value.ConvertToInt() : SecilenBolgeIdQS;

        }
        private void MadalyaDDLDoldur()
        {
            MadalyaDDL.Items.Clear();

            System.Web.UI.WebControls.ListItem li1 = new System.Web.UI.WebControls.ListItem(ProjeConstants.ARMAGAN_ALTIN, ProjeConstants.ARMAGAN_ALTINID.ToString());
            System.Web.UI.WebControls.ListItem li2 = new System.Web.UI.WebControls.ListItem(ProjeConstants.ARMAGAN_GUMUS.ToString(), ProjeConstants.ARMAGAN_GUMUSID.ToString());
            System.Web.UI.WebControls.ListItem li3 = new System.Web.UI.WebControls.ListItem(ProjeConstants.ARMAGAN_BRONZ.ToString(), ProjeConstants.ARMAGAN_BRONZID.ToString());

            MadalyaDDL.Items.Add(li1);
            MadalyaDDL.Items.Add(li2);
            MadalyaDDL.Items.Add(li3);
            SecilenMadalyaQS = string.IsNullOrEmpty(SecilenMadalyaQS) ? MadalyaDDL.SelectedItem.Value : SecilenMadalyaQS;
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
        private void SetDDLValues()
        {
            try
            {
                #region tarih
                

                string ay = !string.IsNullOrEmpty(SecilenAyQS) ? SecilenAyQS : DateTime.Today.Month.ToString();
                UtilityHelper.SetDDLValue(AyDDL, ay);
                SecilenAyQS = ay;
                //yil
                string yil = !string.IsNullOrEmpty(SecilenYilQS) ? SecilenYilQS : DateTime.Today.Year.ReturnEmptyIfNull().ToString();
                UtilityHelper.SetDDLValue(YilDDL, yil);
                SecilenYilQS = yil;

                #endregion tarih
                #region bolge
                //bolge
                System.Web.UI.WebControls.ListItem bolgeItem = new System.Web.UI.WebControls.ListItem();
                
                bolgeItem = BolgeDDL.Items.FindByValue(SecilenBolgeIdQS.ToString());

                if (bolgeItem != null)
                {
                    BolgeDDL.SelectedValue = bolgeItem.Value;
                    SecilenBolgeIdQS = bolgeItem.Value.ConvertToInt();
                }
                #endregion
                #region madalya
                //madalya
                System.Web.UI.WebControls.ListItem madalyaItem = new System.Web.UI.WebControls.ListItem();
                if (!string.IsNullOrEmpty(SecilenMadalyaQS))
                    madalyaItem = MadalyaDDL.Items.FindByValue(SecilenMadalyaQS);

                if (madalyaItem != null)
                {
                    MadalyaDDL.SelectedValue = madalyaItem.Value;
                    SecilenMadalyaQS = madalyaItem.Value;
                }
                #endregion
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
            BeratDurumTablosunuDoldur();
            DosyaTablosunuDoldur();
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
            SetSecilenBasTarBitTar();
            BeratDurumTablosunuDoldur();
            DosyaTablosunuDoldur();
        }
        
        private void SetSecilenBasTarBitTar()
        {
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();

            DateTime bastar = DateTime.Today;
            DateTime bittar = DateTime.Today;

            if (ay == 0)
            {
                bastar = new DateTime(yil, 1, 1);
                bittar = bastar.AddYears(1).AddDays(-1);
            }
            else
            {

                bastar = new DateTime(yil, ay, 1);
                DateTime basGun = new DateTime(yil, ay, 1).AddMonths(1).AddDays(-1);
                bittar = new DateTime(yil, ay, basGun.Day);

            }
            SecilenBastarQS = bastar.ConvertToDatetimeEmptyIfNull();
            SecilenBittarQS = bittar.ConvertToDatetimeEmptyIfNull();
        }
        private DateTime GetBasTar()
        {

            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();

            DateTime bastar = DateTime.Today;

            if (ay == 0)
            {
                bastar = new DateTime(yil, 1, 1);
            }
            else
            {
                bastar = new DateTime(yil, ay, 1);

            }
            return bastar;
        }
        private DateTime GetBitTar()
        {
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();

            DateTime bastar = DateTime.Today;
            DateTime bittar = DateTime.Today;

            if (ay == 0)
            {
                bastar = new DateTime(yil, 1, 1);
                bittar = bastar.AddYears(1).AddDays(-1);
            }
            else
            {

                bastar = new DateTime(yil, ay, 1);
                DateTime basGun = new DateTime(yil, ay, 1).AddMonths(1).AddDays(-1);
                bittar = new DateTime(yil, ay, basGun.Day);

            }
            return bittar;
        }
        private void BeratDurumTablosunuDoldur()
        {
            Armagan armagan = new Armagan();
            DataTable dataTable = armagan.SelectCountDurumByBolgeTarih(SecilenMadalyaQS.ConvertToInt(), SecilenBolgeIdQS, SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime());

            DurumTable.Controls.Clear();
            DosyaOlusturBtn.Visible = false;
            BeratDurumChk.Visible = false;
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
                            BeratDurumChk.Visible = true;
                            //durumChk.Enabled = true;
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
                    DurumTable.Controls.Add(tblrow);
                }
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
                Bolge bolge = new Bolge();
                bolge = bolge.Select(SecilenBolgeIdQS);
                string zaman = DateTime.Now.ToString("dd-MM-yyyy-HH-mm");
                string yaziDosyaAdi = bolge == null ? SecilenBolgeIdQS.ToString() : bolge.KisaAdi + "-" + MadalyaDDL.SelectedItem.Text + "-(" + zaman + ").docx";
                string etiketDosyaAdi = bolge == null ? SecilenBolgeIdQS.ToString() : bolge.KisaAdi + "_" + MadalyaDDL.SelectedItem.Text + "_" + ProjeConstants.ADRESETIKETI_DOSYA + "-(" + zaman + ").docx";
                yaziDosyaAdi = yaziDosyaAdi.Replace(" ", "-");
                etiketDosyaAdi = etiketDosyaAdi.Replace(" ", "_");
                bool isYaziOlusturuldu = BeratBelgesiOlustur(yaziDosyaAdi);
                if (isYaziOlusturuldu)
                {
                    bool etiketOlustuMu = YeniAdresEtiketDosyasiOlustur(etiketDosyaAdi);
                    if (etiketOlustuMu)
                        MessageHelper.PublishMessage("Berat belgeleri ve adres etiketleri hazırlandı, Dosya ismine basarak yazıyı indirebilirsiniz.", ProjeConstants.MESAJ_BASARILI, 2000);
                    else
                    {
                        MessageHelper.PublishMessage("Berat belgeleri hazırlandı, Dosya ismine basarak yazıyı indirebilirsiniz.", ProjeConstants.MESAJ_BASARILI, 2000);
                        MessageHelper.PublishMessage("Adres etiketleri oluşturulamadı.", ProjeConstants.MESAJ_BILGI, 3000);
                    }

                }
                else
                    MessageHelper.PublishMessage("Hata Oluştu", ProjeConstants.MESAJ_HATA);
            }
            catch (Exception ex)
            {
                Exception ex1 = new Exception("Yazı ve Adres oluşturmada hata");
                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.PublishException();
            }
            DosyaTablosunuDoldur();
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

                //string dosyaUrl = SPContext.Current.Web.Url + @"/" + ProjeConstants.NBYSBELGELERI_LIB + @"/" + dosyaAdi;

                //string sourceString = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                //string removeString = System.Web.HttpContext.Current.Request.Url.AbsolutePath;

                //int index = sourceString.IndexOf(removeString);
                //string rootUrl = (index < 0)
                //    ? sourceString
                //    : sourceString.Remove(index, removeString.Length);

                //AdresEtiketLnk.Text = dosyaAdi;
                //AdresEtiketLnk.NavigateUrl = rootUrl + "/_layouts/15/download.aspx?SourceUrl=" + dosyaUrl;

                //AdresEtiketLnk.Visible = true;
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
        private bool BeratBelgesiOlustur(string dosyaAdi)
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

                //SonDosyaLinkiniKoy(dosyaAdi);
                isYaziOlusturuldu = true;
                if (BeratDurumChk.Checked)
                {
                    Armagan armagan = new Armagan();
                    try
                    {
                        bool isUpdated = armagan.UpdateDurumByBolge(ProjeConstants.DURUM_KONTROLEDILDI, ProjeConstants.DURUM_GONDERILDI, SecilenBastarQS, SecilenBittarQS, SecilenMadalyaQS.ConvertToInt(), ProjeConstants.BOLGE_HEPSI_INT);
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
                BeratDurumTablosunuDoldur();
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
            DataTable dataTable = armagan.SelectByDurumTarihReturnDT(ProjeConstants.DURUM_KONTROLEDILDI, bastar, bittar, SecilenMadalyaQS, SecilenBolgeIdQS, ProjeConstants.HEPSI_INT);

            if (dataTable != null)
            {
                int count = dataTable.Rows.Count;
                foreach (DataRow row in dataTable.Rows)
                {
                    string nakitBagisciAdi = row["NakitBagisciAdi"].ToString();
                    string belgedeYazanIsim = row["BelgedeYazanIsim"].ToString();
                    string belgeNo = row["ArmaganId"].ToString();
                    string nakitBagisciTC = row["NakitBagisciTC"].ToString();
                    DateTime tarih = row["Tarih"].ConvertToDatetime();

                    decimal tutar = row["Tutar"].ConvertToDecimal();

                    //create key value pair, key represents words to be replace and 
                    //values represent values in document in place of keys.
                    Dictionary<string, string> keyValues = new Dictionary<string, string>();
                    keyValues.Add("BelgeTarihiVar", EvrakTarihiTxt.Text);
                    keyValues.Add("BelgeNoVar", belgeNo);
                    keyValues.Add("BagisciAdiVar", string.IsNullOrEmpty(belgedeYazanIsim) ? nakitBagisciAdi : belgedeYazanIsim);
                    keyValues.Add("BagisTarihiVar", tarih.ToString("dd.MM.yyyy"));
                    keyValues.Add("TutarVar", tutar.ToString("N0", culturInfo) + " TL");

                    keyValues.Add("ImzaVar", ImzalayanTxt.Text);
                    keyValues.Add("UnvanVar", ImzalayanUnvanTxt.Text);
                    keyValues.Add("MakamVar", ImzalayanMakamTxt.Text);

                    destinationStream = SearchAndReplace(templateStream, keyValues);
                    bool addTemplateAtTheEnd=--count > 0;
                    if (addTemplateAtTheEnd)
                    {
                        destinationStream = AddParagraph2DestinationStream(destinationStream, templateParagraphs); 
                    }
                }
            }


            return destinationStream;
        }
        private MemoryStream AddAdresEtiketData2DestinationStream(MemoryStream destinationStream, IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Table> templateTables)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            DateTime bastar = GetBasTar();
            DateTime bittar = GetBitTar();
            Armagan armagan = new Armagan();
            DataTable dataTable = armagan.SelectByDurumTarihReturnDT(ProjeConstants.DURUM_KONTROLEDILDI, bastar, bittar, SecilenMadalyaQS, SecilenBolgeIdQS, ProjeConstants.HEPSI_INT);
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
                    string telefon = row == null ? "" : row["Telefon"].ToString();
                    string ili = row == null ? "" : row["IlAdi"].ToString();
                    string ilcesi = row == null ? "" : row["IlceAdi"].ToString();
                    string semtIlceIl = ilcesi + @"/" + ili;

                    //create key value pair, key represents words to be replace and 
                    //values represent values in document in place of keys.
                    Dictionary<string, string> keyValues = new Dictionary<string, string>();
                    keyValues.Add("AdSoyad" + index + "Var", nakitBagisciAdi + " -" + armaganId);
                    if (string.IsNullOrEmpty(adres))
                    {
                        keyValues.Add("Adres" + index + "Var", string.Empty + (string.IsNullOrEmpty(telefon) ? "" : " (" + telefon + ")"));
                    }
                    else
                    {
                        keyValues.Add("Adres" + index + "Var", adres.Substring(0, adres.Length > 100 ? 100 : adres.Length - 1) + (string.IsNullOrEmpty(telefon) ? "" : " (" + telefon + ")"));
                    }
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
        private MemoryStream AddParagraph2DestinationStream(MemoryStream destinationStream, IEnumerable<Paragraph> templateParagraphs)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(destinationStream, true))
            {
                //boş sayfa ekle
                Paragraph PageBreakParagraph = new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break() { Type = BreakValues.Page }));
                wordDoc.MainDocumentPart.Document.Body.Append(PageBreakParagraph);

                //template yaziyi ekle
                foreach (var paragraph in templateParagraphs)
                {
                    Paragraph newPara = (Paragraph)paragraph.CloneNode(true);// new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break() { Type = BreakValues.Page }));
                    wordDoc.MainDocumentPart.Document.Body.Append(newPara);
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
            WordprocessingDocument wordDoc = WordprocessingDocument.Open(templateStream, true);
            var body = wordDoc.MainDocumentPart.Document.Body;
            var paras = body.Elements<Paragraph>();
            return paras;
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
                    Console.WriteLine("Querying for Test.docx");
                    SPList list = SPContext.Current.Web.Lists[ProjeConstants.NBYSBELGELERI_LIB];
                    SPQuery query = new SPQuery();
                    query.ViewFields = @"<FieldRef Name='FileLeafRef' />";
                    query.Query =
                      @"<Where>
                          <Eq>
                            <FieldRef Name='FileLeafRef' />
                            <Value Type='File'>BeratBelgesiTemplate.docx</Value>
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
        protected void MadalyaDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenMadalyaQS = MadalyaDDL.SelectedItem.Value;
            BeratDurumTablosunuDoldur();
            DosyaTablosunuDoldur();
        }
        protected void BolgeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenBolgeIdQS = BolgeDDL.SelectedItem.Value.ConvertToInt();
            BeratDurumTablosunuDoldur();
            DosyaTablosunuDoldur();
        }

        //Dosya işlemleri
        private void DosyaTablosunuDoldur()
        {
            Bolge bolge= new Bolge();
            bolge = bolge.Select(SecilenBolgeIdQS);
            string yaziDosyaAdi = bolge==null?SecilenBolgeIdQS.ToString():bolge.KisaAdi + "-" + MadalyaDDL.SelectedItem.Text;
            yaziDosyaAdi = yaziDosyaAdi.Replace(" ", "-");
            List<SPFile> fileList = DosyaListesiniGetir(ProjeConstants.NBYSBELGELERI_LIB, yaziDosyaAdi);
            var jsonData = ToJSON(fileList, yaziDosyaAdi); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);

        }
        public string ToJSON(List<SPFile> fileList, string pDosyaAdi)
        {
            string json = "[]";
            try
            {
                if (fileList != null)
                {

                    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                    List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
                    Dictionary<string, object> childRow;
                    Bolge bolge = new Bolge();
                    bolge = bolge.Select(SecilenBolgeIdQS);
                    foreach (var file in fileList)
                    {
                        childRow = new Dictionary<string, object>();
                        childRow.Add("FileName", file.Name);
                        childRow.Add("Author", file.Author.Name.ToString());
                        childRow.Add("ModifiedBy", file.ModifiedBy.Name.ToString());
                        childRow.Add("TimeLastModified", file.TimeLastModified.ToString());
                        childRow.Add("Sil", file.Item.ID.ToString());

                        string sourceString = file.Name;
                        string removeString = pDosyaAdi;
                        int index = sourceString.IndexOf(removeString);
                        string zamanEki = (index < 0)
                            ? sourceString
                            : sourceString.Remove(index, removeString.Length);

                        string etiketDosyaAdi = bolge == null ? SecilenBolgeIdQS.ToString() : bolge.KisaAdi + "_" + MadalyaDDL.SelectedItem.Text + "_" + ProjeConstants.ADRESETIKETI_DOSYA + zamanEki;
                        etiketDosyaAdi = etiketDosyaAdi.Replace(" ", "_");
                        string zaman = new DateTime(SecilenYilQS.ConvertToInt(), SecilenAyQS.ConvertToInt(), 1).ToString("-MM-yyyy-");
                        if (!etiketDosyaAdi.Contains(zaman))
                        {
                            continue;
                        }
                        bool isDosyaVarMi = DosyaVarMi(ProjeConstants.NBYSBELGELERI_LIB, etiketDosyaAdi);

                        etiketDosyaAdi = isDosyaVarMi ? etiketDosyaAdi : string.Empty;
                        childRow.Add("LabelFileName", etiketDosyaAdi);

                        parentRow.Add(childRow);
                    }
                    jsSerializer.MaxJsonLength = Int32.MaxValue;
                    json = jsSerializer.Serialize(parentRow);
                }
            }
            catch (Exception e)
            {
                ExceptionHelper eh = new ExceptionHelper(e);
                eh.PublishException();
                throw;
            }
            return json;
        }
        private bool DosyaVarMi(string libName, string fileName)
        {

            bool isDosyaBulundu = false;
            SPList list = SPContext.Current.Web.Lists[libName];
            SPQuery query = new SPQuery();
            query.ViewFields = @"<FieldRef Name='FileLeafRef' />";

            query.Query = @"<Where>
                          <Eq>
                            <FieldRef Name='FileLeafRef' />
                            <Value Type='File'>" + fileName + @"</Value>
                          </Eq>
                        </Where>";
            //query.Query = @"
            //    <Where>
            //        <Contains>
            //            <FieldRef Name='FileLeafRef' />
            //            <Value Type='File'>" + fileName + @"</Value>
            //        </Contains>
            //    </Where>";
            SPListItemCollection collection = list.GetItems(query);

            if (collection.Count > 0)
            {
                isDosyaBulundu = true;
            }
            return isDosyaBulundu;
        }
        private string CreateDataTable(string jsonData)
        {
            string rootUrl = UtilityHelper.RootURLGetir();
            string dosyaUrl = SPContext.Current.Web.Url + @"/" + ProjeConstants.NBYSBELGELERI_LIB + @"/";
            string tableString = @"
                jQuery(document).ready(function () {

                    jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                    jQuery('#CustomDataTable').DataTable({
                        data: " + jsonData + @",
                        columns: [
                            { data: 'FileName' },
                            { data: 'LabelFileName'},
                            { data: 'Author' },
                            { data: 'TimeLastModified' },
                            { data: 'Sil' },
                        ],
                        'order': [[3, 'desc']],
                        'language': {
                            'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        columnDefs:[
                            {targets:0, render:function(data, type, row, meta){
                                return ('<a href=" + rootUrl + "/_layouts/15/download.aspx?SourceUrl=" + dosyaUrl +
                                @"'+row.FileName+' class=\'btn-link \'>'+row.FileName+'</a>')
                            }},
                            {targets:1, render:function(data, type, row, meta){
                                return ('<a href=" + rootUrl + "/_layouts/15/download.aspx?SourceUrl=" + dosyaUrl +
                                @"'+row.LabelFileName+' class=\'btn-link \'>'+row.LabelFileName+'</a>')
                            }},
                            {targets:4, render:function(data, type, row, meta){
                                return('<a href=# onclick=CallButtonClick(\''+row.FileName + '\',\''+row.LabelFileName + '\'); class=\'btn btn-outline-danger \'>Dosyaları Sil</a>');
                            }},
                        ],    
                        responsive: true,
                        destroy: true,
                        dom: 'rtip',
                    });
                });
                ";
            return tableString;
        }
        private bool DosyalariSPListesindenSil(string libName, string dosyaAdi, string etiketDosyaAdi)
        {
            bool isDeleted = false;
            string newFileUrl = string.Empty;
            List<SPFile> lstFile = new List<SPFile>();
            string siteUrl = SPContext.Current.Web.Url;
            using (SPSite spSite = new SPSite(siteUrl))
            {
                isDeleted = DosyaSil(spSite, libName, dosyaAdi);
                isDeleted = DosyaSil(spSite, libName, etiketDosyaAdi);
            }
            return isDeleted;
        }
        private bool DosyaSil(SPSite spSite, string libName, string fileName)
        {
            bool isDeleted = false;
            SPList list = SPContext.Current.Web.Lists[libName];
            SPQuery query = new SPQuery();
            query.Query = @"<Where>
                          <Eq>
                            <FieldRef Name='FileLeafRef' />
                            <Value Type='File'>" + fileName + @"</Value>
                          </Eq>
                        </Where>";
            SPListItemCollection collection = list.GetItems(query);
            foreach (SPListItem item in collection)
            {
                if (item.Name.Equals(fileName))
                {
                    item.Delete();
                    isDeleted = true;
                    break;
                }
            }
            return isDeleted;
        }
        private List<SPFile> DosyaListesiniGetir(string libName, string fileName)
        {
            string newFileUrl = string.Empty;
            List<SPFile> lstFile = new List<SPFile>();
            string siteUrl = SPContext.Current.Web.Url;
            using (SPSite spSite = new SPSite(siteUrl))
            {
                SPList list = SPContext.Current.Web.Lists[libName];
                SPQuery query = new SPQuery();
                query.ViewFields = @"<FieldRef Name='FileLeafRef' />";
                query.Query =
                  @"< Where >
                        < Contains >
                            < FieldRef Name = 'Title' />
                            < Value Type = 'File' > " + fileName + @" </ Value >
                        </ Contains >
                    </ Where >
                    <OrderBy>
                        <FieldRef Name='Modified' Ascending='False'/>
                    </OrderBy>";
                SPListItemCollection collection = list.GetItems(query);
                foreach (SPListItem item in collection)
                {
                    if (item.Name.Contains(fileName))
                    {
                        lstFile.Add(item.File);
                    }
                }

                return lstFile;
            }
        }
        protected void DosyayiSilBtn_Click(object sender, EventArgs e)
        {
            DosyaAdiLbl.Text = paramDosyaAdiLbl.Value;
            EtiketDosyaAdiLbl.Text = paramEtiketDosyaAdiLbl.Value;
            DosyayiSilNowBtn.Visible = true;
            var openPopup = "OpenModalOnay();";
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
        }
        protected void DosyayiSilNowBtn_Click(object sender, EventArgs e)
        {
            DosyalariSPListesindenSil(ProjeConstants.NBYSBELGELERI_LIB, paramDosyaAdiLbl.Value, paramEtiketDosyaAdiLbl.Value);
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            int index = currentUrl.LastIndexOf("?") < 0 ? currentUrl.Length : currentUrl.LastIndexOf("?");

            string rawUrl = currentUrl.Substring(0, index);


            string pageUrl = rawUrl +
                "?SecilenAy=" + SecilenAyQS + "&SecilenYil=" + SecilenYilQS +
                "&SecilenMadalya=" + SecilenMadalyaQS + "&BolgeId=" + SecilenBolgeIdQS +
                "&SecilenBastar=" + SecilenBastarQS + "&SecilenBittar=" + SecilenBittarQS;
            Page.Response.Redirect(pageUrl);
        }
    }
}
