using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.SharePoint;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.KiraBorcuYazisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraBorcuYazisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraBorcuYazisiWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        private string SecilenBolgeQS
        {
            get
            {

                if (ViewState["SecilenBolge"] == null)
                {
                    if (Page.Request.QueryString["SecilenBolge"] != null)
                    {
                        ViewState["SecilenBolge"] = Page.Request.QueryString["SecilenBolge"];
                    }
                    else
                    {
                        ViewState["SecilenBolge"] = string.Empty;
                    }
                }
                return ViewState["SecilenBolge"].ToString();
            }

            set
            {
                ViewState["SecilenBolge"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillDropDownList();
                SetDDLValues(); //ay ve yılı querystringden al
                ParametreleriDoldur();

            }
            KayitGetir();
        }
        private void FillDropDownList()
        {
            AyDDLDoldur();
            YilDDLDoldur();
            BolgeDDLDoldur();
        }
        private void BolgeDDLDoldur()
        {
            BolgeDDL.Items.Clear();
            System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem(ProjeConstants.BOLGE_GENELMUDURLUK);
            System.Web.UI.WebControls.ListItem li1 = new System.Web.UI.WebControls.ListItem(ProjeConstants.BOLGE_ISTANBUL);
            System.Web.UI.WebControls.ListItem li2 = new System.Web.UI.WebControls.ListItem(ProjeConstants.BOLGE_IZMIR);
            System.Web.UI.WebControls.ListItem li3 = new System.Web.UI.WebControls.ListItem(ProjeConstants.BOLGE_MERSIN);
            BolgeDDL.Items.Add(li);
            BolgeDDL.Items.Add(li1);
            BolgeDDL.Items.Add(li2);
            BolgeDDL.Items.Add(li3);
            SecilenBolgeQS = string.IsNullOrEmpty(SecilenBolgeQS) ? BolgeDDL.SelectedItem.Text : SecilenBolgeQS;
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
                //acilista ay ve yili querystring ile gelen ay ve yıla eşitle boş geldiyse gecen aya/yila eşitle               
                //ay
                string ay = !string.IsNullOrEmpty(SecilenAyQS) ? SecilenAyQS : DateTime.Today.Month.ToString();
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
                #endregion tarih
                #region bolge
                //bolge
                System.Web.UI.WebControls.ListItem bolgeItem = new System.Web.UI.WebControls.ListItem();
                if (!string.IsNullOrEmpty(SecilenBolgeQS))
                    bolgeItem = BolgeDDL.Items.FindByValue(SecilenBolgeQS);

                if (bolgeItem != null)
                {
                    BolgeDDL.SelectedValue = bolgeItem.Value;
                    SecilenBolgeQS = bolgeItem.Value;
                }
                #endregion
            }
            catch (Exception)
            {

                //TODO
            }
        }
        private void ParametreleriDoldur()
        {
            DateTime bugun = DateTime.Today;
            DateTime buAyIlkGun = new DateTime(bugun.Year, bugun.Month, 1);
            DateTime buAySonGun = new DateTime(bugun.Year, bugun.Month, 1).AddMonths(1).AddDays(-1);
            Parafe1Txt.Text = @"…./" + bugun.ToString("MM") + @"/" + bugun.Year + " Eml.Ynt.Kd.Uzm.Z.ÇALIŞ";
            Parafe2Txt.Text = @"…./" + bugun.ToString("MM") + @"/" + bugun.Year + " İnş.Eml.Ynt.Ş.Md.H.ŞENEL";
            ImzalayanTxt.Text = @"Tolga DURUTUNA";
            ImzalayanMakamTxt.Text = @"TSKGV Baş Hukuk Müşaviri";
            EvrakTarihiTxt.Text = bugun.ToString("dd") + " " + bugun.ToString("MMMM") + " " + bugun.Year;
            GecerlilikTarihiTxt.Text = buAyIlkGun.ToString("dd") + " " + buAyIlkGun.ToString("MMMM") + " " + buAyIlkGun.Year;
            
            SonOdemeTarihiTxt.Text = buAySonGun.ToString("dd") + " " + buAySonGun.ToString("MMMM") + " " + buAySonGun.Year;
            EvrakSayisiYiliTxt.Text = bugun.ToString("yy");

        }
        private void KayitGetir()
        {
            var jsonData = GetData(); //veri çekilip json a çeviriliyor

            bool jasonDataBosMu = string.IsNullOrWhiteSpace(jsonData.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", ""));
            if (!jasonDataBosMu)
            {
                TableDiv.Attributes["style"] = "display:block";
                YaziyiOlusturBtn.Visible = true;
                var jsString = CreateJsString(jsonData); //javascript kodu hazırlanıyor.
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
            }
            else
            {
                TableDiv.Attributes["style"] = "display:none";
                TableDataLbl.Text = "Kira borcu olan kiracı bulunmamaktadır.";
                YaziyiOlusturBtn.Visible = false;
            }
        }
        private string GetData()
        {
            int ay = SecilenAyQS.ConvertToInt();
            int yil = SecilenYilQS.ConvertToInt();
            DateTime secilenTarih = new DateTime(yil, ay, 1);
            DateTime vadeBastar = secilenTarih;
            DateTime vadeBittar = secilenTarih.AddMonths(1).AddDays(-1);

            //DateTime secilenTarih = DateTime.Today.AddMonths(-1);
            //DateTime ilkTarih = secilenTarih;//.AddDays(1);
            //DateTime sonTarih = secilenTarih.AddMonths(1).AddDays(-1); //AddDays(-1);
            OdemePlani opl = new OdemePlani();
            int kayitSayisi = 0;
            string json = opl.SelectBorcluOdemePlanlariByBolgeTarihJson(SecilenBolgeQS, vadeBastar, vadeBittar, 2, 2, ref kayitSayisi);
            TableDataLbl.Text = "Toplam " + kayitSayisi + " kayıt bulundu";
            return json;
        }
        private string CreateJsString(string jsonData)
        {
            string ekstretablestr = @"   
                                        $('#tblfilter').puidatatable({
                                        caption: '',
                                        editMode: 'cell',
                                        paginator: {
                                                    rows: 8
                                                    },
                                        columns: [
                                            { field: 'Kiraci', headerText: 'Kiracı', sortable:true,filter: true,headerStyle:'width: 60%' },
                                            { field: 'Bolge', headerText: 'Bolge', sortable:true,headerStyle:'width: 20%' },
                                            { field: 'FaizliBakiyeFormat', headerText: 'Borç',bodyClass:'text-right',headerStyle:'width: 20%'}

                                                ],
                                       datasource:" + jsonData + @",
                                       resizableColumns: true,
                                       globalFilter:'#globalFilter'
                                       });
                                    ";


            return ekstretablestr;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenAyQS = AyDDL.SelectedItem.Value.ToString();
            KayitGetir();
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
            KayitGetir();
        }
        protected void BolgeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenBolgeQS = BolgeDDL.SelectedItem.Text;
            KayitGetir();
        }
        protected void YaziyiOlusturBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a copy of the template file and open the copy 
                string zaman = DateTime.Now.ToString("dd-MM-yyyy-HH-mm");
                string yaziDosyaAdi = "Kira-Borcu-" + SecilenBolgeQS + "-(" + zaman + ").docx";
                string etiketDosyaAdi = "Adres-EtiketiKB-" + SecilenBolgeQS + "-(" + zaman + ").docx";
                bool isYaziOlusturuldu = YeniYaziOlustur(yaziDosyaAdi);
                if (isYaziOlusturuldu)
                {
                    YeniAdresEtiketDosyasiOlustur(etiketDosyaAdi);
                    MessageHelper.PublishMessage("Dosyalar hazırlandı, Dosya ismine basarak yazıyı indirebilirsiniz", ProjeConstants.MESAJ_BASARILI, 2000);
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

                string dosyaUrl = SPContext.Current.Web.Url + @"/" + ProjeConstants.TBYSBELGELERI_LIB + @"/" + dosyaAdi;

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
        private bool YeniYaziOlustur(string dosyaAdi)
        {
            bool isYaziOlusturuldu = false;
            try
            {
                MemoryStream templateStream = GetTemplateStream("KiraBorcuTemplate.docx");
                IEnumerable<Paragraph> templateParagraphs = GetTemplateParagraphs(templateStream);

                MemoryStream destinationStream = AddData2DestinationStream(templateStream, templateParagraphs);

                destinationStream.Seek(0, SeekOrigin.Begin);
                destinationStream.Position = 0;

                AddToSharePoint(destinationStream, dosyaAdi);
                string dosyaUrl = SPContext.Current.Web.Url + @"/" + ProjeConstants.TBYSBELGELERI_LIB + @"/" + dosyaAdi;

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
            }
            catch (Exception)
            {
                isYaziOlusturuldu = false;
                throw;
            }
            return isYaziOlusturuldu;
        }
        private MemoryStream AddData2DestinationStream(MemoryStream templateStream, IEnumerable<Paragraph> templateParagraphs)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            MemoryStream destinationStream = null;
            int ay = SecilenAyQS.ConvertToInt();
            int yil = SecilenYilQS.ConvertToInt();
            DateTime secilenTarih = new DateTime(yil, ay, 1);
            DateTime vadeBastar = secilenTarih;
            DateTime vadeBittar = secilenTarih.AddMonths(1).AddDays(-1);
            DateTime bugun = DateTime.Today;
            ////DateTime gecenAySonGun = new DateTime(bugun.Year, bugun.Month, 1).AddDays(-1);
            ////DateTime gecenAyIlkGun = new DateTime(bugun.Year, bugun.AddMonths(-1).Month, 1);
            //DateTime secilenTarih = DateTime.Today.AddMonths(-1);
            //DateTime ilkTarih = secilenTarih;//.AddDays(1);
            //DateTime sonTarih = secilenTarih.AddMonths(1).AddDays(-1); //AddDays(-1);
            OdemePlani opl = new OdemePlani();
            DataTable dataTable = opl.SelectBorcluOdemePlanlariByBolgeTarih(SecilenBolgeQS, vadeBastar, vadeBittar, 2, 2);
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string kiraci = row == null ? "" : row["Kiraci"].ToString();
                    string adres = row == null ? "" : row["Adres"].ToString();
                    string semt = row == null ? "" : row["Semt"].ToString();
                    string ili = row == null ? "" : row["Ili"].ToString();
                    string ilcesi = row == null ? "" : row["Ilcesi"].ToString();
                    string semtIlIlce = (string.IsNullOrEmpty(semt) ? "" : semt + @"-") + ilcesi + @"/" + ili;
                    string ilkSozlesmeTar = row["IlkSozlesmeTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();

                    decimal faizliBakiye = row == null ? 0 : row["FaizliBakiye"].ConvertToDecimal();
                    string borcStr = Math.Abs(faizliBakiye).ToString("N", culturInfo) + " TL";
                    string vadeBitTar = row == null ? "" : row["VadeBitTar"].ConvertToDatetime().ToString("dd MMMM yyyy");

                    //create key value pair, key represents words to be replace and 
                    //values represent values in document in place of keys.
                    Dictionary<string, string> keyValues = new Dictionary<string, string>();
                    keyValues.Add("EvrakYilVar", EvrakSayisiYiliTxt.Text);
                    keyValues.Add("EvrakTarihiVar", EvrakTarihiTxt.Text);
                    keyValues.Add("AdSoyadVar", kiraci);
                    keyValues.Add("AdresVar", adres);
                    keyValues.Add("SemtIlceIlVar", semtIlIlce);
                    keyValues.Add("IlkSozlesmeTarihiVar", ilkSozlesmeTar);
                    keyValues.Add("GecerlilikTarihiVar", GecerlilikTarihiTxt.Text);

                    keyValues.Add("AyYilVar", bugun.ToString("MMMM") + " " + bugun.ToString("yyyy") + " ayı ");

                    keyValues.Add("VadeBitTarVar", vadeBitTar);// SonOdemeTarihiTxt.Text);
                    keyValues.Add("TutarVar", borcStr);
                    keyValues.Add("ImzaVar", ImzalayanTxt.Text);
                    keyValues.Add("UnvanVar", ImzalayanMakamTxt.Text);
                    keyValues.Add("Paraf1Var", Parafe1Txt.Text);
                    keyValues.Add("Paraf2Var", Parafe2Txt.Text);

                    destinationStream = SearchAndReplace(templateStream, keyValues);
                    destinationStream = AddParagraph2DestinationStream(destinationStream, templateParagraphs);

                    keyValues.Remove("Paraf1Var");
                    keyValues.Remove("Paraf2Var");
                    keyValues.Add("Paraf1Var", "");
                    keyValues.Add("Paraf2Var", "");

                    destinationStream = SearchAndReplace(templateStream, keyValues);
                    destinationStream = AddParagraph2DestinationStream(destinationStream, templateParagraphs);
                }
            }


            return destinationStream;
        }
        private MemoryStream AddAdresEtiketData2DestinationStream(MemoryStream templateStream, IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Table> templateTables)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            MemoryStream destinationStream = null;
            DateTime bugun = DateTime.Today;

            int ay = SecilenAyQS.ConvertToInt();
            int yil = SecilenYilQS.ConvertToInt();
            DateTime secilenTarih = new DateTime(yil, ay, 1);
            DateTime vadeBastar = secilenTarih;
            DateTime vadeBittar = secilenTarih.AddMonths(1).AddDays(-1);
            //DateTime secilenTarih = DateTime.Today.AddMonths(-1);
            //DateTime ilkTarih = secilenTarih;//.AddDays(1);
            //DateTime sonTarih = secilenTarih.AddMonths(1).AddDays(-1); //AddDays(-1);
            OdemePlani opl = new OdemePlani();
            DataTable dataTable = opl.SelectBorcluOdemePlanlariByBolgeTarih(SecilenBolgeQS, vadeBastar, vadeBittar, 2, 2);
            if (dataTable != null)
            {
                int index = 1;
                List<string> uzunAdresliler = new List<string>();
                foreach (DataRow row in dataTable.Rows)
                {
                    string kiraci = row == null ? "" : row["Kiraci"].ToString();
                    string adres = row == null ? "" : row["Adres"].ToString();
                    string semt = row == null ? "" : row["Semt"].ToString();
                    string ili = row == null ? "" : row["Ili"].ToString();
                    string ilcesi = row == null ? "" : row["Ilcesi"].ToString();
                    string semtIlIlce = (string.IsNullOrEmpty(semt) ? "" : semt + @"-") + ilcesi + @"/" + ili;

                    //create key value pair, key represents words to be replace and 
                    //values represent values in document in place of keys.
                    Dictionary<string, string> keyValues = new Dictionary<string, string>();
                    keyValues.Add("AdSoyad" + index + "Var", kiraci);
                    keyValues.Add("Adres" + index + "Var", string.IsNullOrEmpty(adres) ? "Adresi yok" : adres.Substring(0, adres.Length > 110 ? 110 : adres.Length - 1));
                    keyValues.Add("SemtIlceIl" + index + "Var", semtIlIlce);
                    destinationStream = SearchAndReplace(templateStream, keyValues);
                    destinationStream = SearchAndReplace(templateStream, keyValues);
                    if (index++ >= 21)//sayfa bitti yeni sayfa ekle
                    {
                        destinationStream = AddTable2DestinationStream(destinationStream, templateTables);
                        index = 1;
                    }
                    if (!string.IsNullOrEmpty(adres) && adres.Trim().Length > 110)
                    {
                        uzunAdresliler.Add(kiraci);
                    }
                }
                if (index < 21)
                {

                    for (int i = index; i < 22; i++)
                    {
                        Dictionary<string, string> keyValues = new Dictionary<string, string>();
                        keyValues.Add("AdSoyad" + i + "Var", "");
                        keyValues.Add("Adres" + i + "Var", "");
                        keyValues.Add("SemtIlceIl" + i + "Var", "");
                        destinationStream = SearchAndReplace(templateStream, keyValues);
                    }

                }
                if (uzunAdresliler.Count > 0)
                {
                    string message = string.Join(Environment.NewLine, uzunAdresliler);
                    MessageHelper.PublishMessage(message + Environment.NewLine +
                        " adresi çok uzun olduğundan kesilerek kısaltıldı. Lütfen etiketini kontrol ediniz. ", ProjeConstants.MESAJ_BILGI);
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
        private MemoryStream GetTemplateStream(string templateFileName)
        {
            string newFileUrl = string.Empty;

            string siteUrl = SPContext.Current.Web.Url;
            using (SPSite spSite = new SPSite(siteUrl))
            {
                SPList list = SPContext.Current.Web.Lists[ProjeConstants.TBYSBELGELERI_LIB];
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
                SPList docLib = SPContext.Current.Web.Lists[ProjeConstants.TBYSBELGELERI_LIB];

                SPFile file = docLib.RootFolder.Files.Add(fileName, memStream, true);

                file.Update();

            }
        }

    }
}
