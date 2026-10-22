using Utility.Logging;
using NBYS_WebParts.Logging;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.SharePoint;
using Model.NBYS;
using Model.Ortak;
using Model.Services.Ortak;
using Model.Services.NBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
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
        private PortalLogContext LoggingContext { get { return PortalLogContext.FromCurrentRequest("NBYS", "BeratBelgesiYazisiWP"); } }
        private Bolge userRegion;
        private bool canProcess = true;
        private NbysDocumentBatch documentBatch;
        private int shortenedAddressCount;
        private bool documentSaved;
        private NbysDocumentBatch GetDocumentBatch()
        {
            if (documentBatch == null) documentBatch = new NbysDocumentWriteService(LoggingContext).Prepare(GetBasTar(), GetBitTar(), SecilenMadalyaQS.ConvertToInt(), BolgeDDL.SelectedValue.ConvertToInt());
            return documentBatch;
        }
        private string AddressNote()
        {
            return shortenedAddressCount > 0 ? " " + shortenedAddressCount + " adres kısaltıldı; etiketleri kontrol edin." : string.Empty;
        }

        private void ExecuteAction(string operation, Action action)
        {
            NbysWebPartLog.ExecuteAction(LoggingContext, operation, () => { if (canProcess) action(); });
        }
        private void SetFilter(System.Web.UI.WebControls.DropDownList list, string value, string fallback)
        {
            var item = list.Items.FindByValue(value) ?? list.Items.FindByValue(fallback) ?? list.Items.Cast<System.Web.UI.WebControls.ListItem>().FirstOrDefault();
            if (item == null) throw new InvalidOperationException("Filtre seçeneği bulunamadı.");
            list.ClearSelection(); item.Selected = true;
        }

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
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
                userRegion = new NbysBagisciWriteService(LoggingContext).GetUserRegion();
                if (userRegion == null) { canProcess = false; Messages.Warning("Bölge bilginiz bulunamadı. Yetki bilgilerinizi kontrol ettirin."); return; }
                if (Page.IsPostBack && !NbysDocumentWriteService.IsCentral(userRegion.Id)
                    && BolgeDDL.SelectedValue != userRegion.Id.ToString())
                { canProcess = false; Messages.Warning("Bölge yetkiniz değişmiş. Sayfayı yeniden açıp seçiminizi kontrol edin."); return; }
                if (!Page.IsPostBack)
                {
                    //time out olmasın diye
                    ScriptManager _scriptMan = ScriptManager.GetCurrent(Page);
                    _scriptMan.AsyncPostBackTimeout = 36000;

                    FillDropDownList();
                    SetDDLValues(); //ay ve yılı querystring'den al
                    SetSecilenBasTarBitTar();
                    FormuDoldur();

                    BeratDurumTablosunuDoldur();
                    DosyaTablosunuDoldur();
                }
            });
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
            List<Bolge> list = new BolgeService().GetActive(userRegion.Id, LoggingContext);
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
            SetFilter(AyDDL, SecilenAyQS, DateTime.Today.Month.ToString());
            SetFilter(YilDDL, SecilenYilQS, DateTime.Today.Year.ToString());
            SecilenAyQS = AyDDL.SelectedValue;
            SecilenYilQS = YilDDL.SelectedValue;
            SetFilter(BolgeDDL, userRegion.Id.ToString(), userRegion.Id.ToString());
            SetFilter(MadalyaDDL, SecilenMadalyaQS, ProjeConstants.ARMAGAN_BRONZID.ToString());
            SecilenMadalyaQS = MadalyaDDL.SelectedValue;
            SetFilter(BolgeDDL, NbysDocumentWriteService.IsCentral(userRegion.Id) ? SecilenBolgeIdQS.ToString() : userRegion.Id.ToString(), BolgeDDL.SelectedValue);
            SecilenBolgeIdQS = Convert.ToInt32(BolgeDDL.SelectedValue);
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("AyDDL_SelectedIndexChanged", () =>
            {
                SecilenAyQS = AyDDL.SelectedItem.Value.ToString();
                SetSecilenBasTarBitTar();
                BeratDurumTablosunuDoldur();
                DosyaTablosunuDoldur();
            });
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("YilDDL_SelectedIndexChanged", () =>
            {
                SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
                SetSecilenBasTarBitTar();
                BeratDurumTablosunuDoldur();
                DosyaTablosunuDoldur();
            });
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
            SetSecilenBasTarBitTar();
            return SecilenBastarQS.ConvertToDatetime();
        }
        private DateTime GetBitTar()
        {
            SetSecilenBasTarBitTar();
            return SecilenBittarQS.ConvertToDatetime();
        }
        private void BeratDurumTablosunuDoldur()
        {
            DataTable dataTable = new ArmaganService(LoggingContext).CountDurumByBolge(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), SecilenMadalyaQS.ConvertToInt(), SecilenBolgeIdQS);

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
            ExecuteAction("CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl, false);
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
            });
        }
        protected void DosyaOlusturBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("DosyaOlusturBtn_Click", () =>
            {
                var batch = GetDocumentBatch();
                NbysDocumentFeedback.Run(LoggingContext, batch, () =>
                {
                    Bolge region = new BolgeService().GetSelected(BolgeDDL.SelectedValue.ConvertToInt(), LoggingContext);
                    if (region == null) throw new InvalidOperationException("Bölge bilgisi bulunamadı.");
                    string stamp = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss-fff") + "-" + Guid.NewGuid().ToString("N").Substring(0, 8);
                    string prefix = region.KisaAdi + "-" + MadalyaDDL.SelectedItem.Text;
                    string period = GetBasTar().ToString("-MM_yyyy");
                    string fileName = (prefix + period + "-(" + stamp + ").docx").Replace(" ", "-");
                    string labelName = (region.KisaAdi + "_" + MadalyaDDL.SelectedItem.Text + "_" + ProjeConstants.ADRESETIKETI_DOSYA + period + "-(" + stamp + ").docx").Replace(" ", "_");
                    BeratBelgesiOlustur(fileName);
                    try { YeniAdresEtiketDosyasiOlustur(labelName); }
                    catch (Exception error) { throw new InvalidOperationException("Berat dosyası kaydedildi; etiket dosyası hazırlanamadı.", error); }
                }, BeratDurumChk.Checked, () => { BeratDurumTablosunuDoldur(); DosyaTablosunuDoldur(); },
                    "Berat belgeleri ve adres etiketleri hazırlandı. Dosya listesinden indirebilirsiniz.", AddressNote, () => documentSaved);
            });
        }
        private bool YeniAdresEtiketDosyasiOlustur(string dosyaAdi)
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

            return true;
        }
        private MemoryStream GetTemplateStream(string templateFileName)
        {
            string siteUrl = SPContext.Current.Web.Url;
            using (SPSite spSite = new SPSite(siteUrl))
            using (SPWeb web = spSite.OpenWeb())
            {
                string fileUrl = web.Url + "/" + ProjeConstants.NBYSBELGELERI_LIB + "/" + templateFileName;
                MemoryStream memStr = new MemoryStream();
                SPFile file = web.GetFile(fileUrl);
                if (file != null && file.Exists)
                {
                    byte[] byteArray = file.OpenBinary();
                    memStr.Write(byteArray, 0, byteArray.Length);
                }
                if (memStr.Length == 0) throw new InvalidOperationException("Belge şablonu bulunamadı veya boş. Şablon kitaplığını kontrol edin.");
                memStr.Position = 0;
                return memStr;
            }

        }
        private bool BeratBelgesiOlustur(string dosyaAdi)
        {
            MemoryStream templateStream = GetTemplateStream();
            IEnumerable<Paragraph> templateParagraphs = GetTemplateParagraphs(templateStream);

            MemoryStream destinationStream = AddData2DestinationStream(templateStream, templateParagraphs);

            destinationStream.Seek(0, SeekOrigin.Begin);
            destinationStream.Position = 0;

            AddToSharePoint(destinationStream, dosyaAdi);

            //SonDosyaLinkiniKoy(dosyaAdi);

            return true;
        }

        private MemoryStream AddData2DestinationStream(MemoryStream templateStream, IEnumerable<Paragraph> templateParagraphs)
        {
            MemoryStream destinationStream = null;
            DateTime bastar = GetBasTar();
            DateTime bittar = GetBitTar();
            DataTable dataTable = GetDocumentBatch().Rows;

            if (dataTable != null)
            {
                int count = dataTable.Rows.Count;
                foreach (DataRow row in dataTable.Rows)
                {
                    string nakitBagisciAdi = row["NakitBagisciAdi"].ToString();
                    string belgedeYazanIsim = row["BelgedeYazanIsim"].ToString();
                    string belgeNo = row["ArmaganId"].ToString();
                    string nakitBagisciTC = row["NakitBagisciTC"].ToString();
                    string cokluBagis = row["CokluBagis"].ToString();
                    DateTime tarih = row["Tarih"].ConvertToDatetime();

                    string tarihstr = tarih.ToString("dd.MM.yyyy") + (cokluBagis.Equals("Çoklu Bağış") ? " tarihine kadar" : " tarihinde");
                    decimal tutar = row["Tutar"].ConvertToDecimal();

                    //create key value pair, key represents words to be replace and 
                    //values represent values in document in place of keys.
                    Dictionary<string, string> keyValues = new Dictionary<string, string>();
                    keyValues.Add("BelgeTarihiVar", EvrakTarihiTxt.Text);
                    keyValues.Add("BelgeNoVar", belgeNo);
                    keyValues.Add("BagisciAdiVar", string.IsNullOrEmpty(belgedeYazanIsim) ? nakitBagisciAdi : belgedeYazanIsim);
                    keyValues.Add("BagisTarihiVar", tarihstr);
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
            DataTable dataTable = GetDocumentBatch().Rows;
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
                    //Sayfada 21 den az kayit varsa template alanlarini temizle adsoyad vs
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
                    shortenedAddressCount++;
                }
            }

            return destinationStream;
        }
        private MemoryStream AddParagraph2DestinationStream(MemoryStream destinationStream, IEnumerable<Paragraph> templateParagraphs)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(destinationStream, true))
            {
                //bos sayfa ekle
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
                //bos sayfa ekle
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
            templateStream.Position = 0;
            using (WordprocessingDocument document = WordprocessingDocument.Open(templateStream, false))
            {
                if (document.MainDocumentPart == null || document.MainDocumentPart.Document.Body == null)
                    throw new InvalidOperationException("Belge şablonu geçerli bir belge gövdesi içermiyor.");
                return document.MainDocumentPart.Document.Body.Elements<Paragraph>()
                    .Select(item => (Paragraph)item.CloneNode(true)).ToList();
            }
        }
        private IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Table> GetTemplateTables(MemoryStream templateStream)
        {
            templateStream.Position = 0;
            using (WordprocessingDocument document = WordprocessingDocument.Open(templateStream, false))
            {
                if (document.MainDocumentPart == null || document.MainDocumentPart.Document.Body == null)
                    throw new InvalidOperationException("Belge şablonu geçerli bir belge gövdesi içermiyor.");
                return document.MainDocumentPart.Document.Body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>()
                    .Select(item => (DocumentFormat.OpenXml.Wordprocessing.Table)item.CloneNode(true)).ToList();
            }
        }
        private MemoryStream GetTemplateStream()
        {
            MemoryStream memStr = new MemoryStream();

                string siteUrl = SPContext.Current.Web.Url;
                using (SPSite spSite = new SPSite(siteUrl))
                using (SPWeb web = spSite.OpenWeb())
                {
                    string fileUrl = web.Url + "/" + ProjeConstants.NBYSBELGELERI_LIB + "/BeratBelgesiTemplate.docx";
                    SPFile file = web.GetFile(fileUrl);
                    if (file != null && file.Exists)
                    {
                        byte[] byteArray = file.OpenBinary();
                        memStr.Write(byteArray, 0, byteArray.Length);
                    }
                }

            if (memStr.Length == 0) throw new InvalidOperationException("Belge şablonu bulunamadı veya boş. Şablon kitaplığını kontrol edin.");
            memStr.Position = 0;
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
            if (memStream == null || memStream.Length == 0) throw new InvalidOperationException("Boş belge kaydedilemez.");
            memStream.Position = 0;
            SPList library = SPContext.Current.Web.Lists[ProjeConstants.NBYSBELGELERI_LIB];
            SPFile file = library.RootFolder.Files.Add(fileName, memStream, false);
            if (file == null || !file.Exists) throw new InvalidOperationException("Belge kitaplığa kaydedilemedi.");
            documentSaved = true;
        }
        protected void MadalyaDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("MadalyaDDL_SelectedIndexChanged", () =>
            {
                SecilenMadalyaQS = MadalyaDDL.SelectedItem.Value;
                BeratDurumTablosunuDoldur();
                DosyaTablosunuDoldur();
            });
        }
        protected void BolgeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("BolgeDDL_SelectedIndexChanged", () =>
            {
                SecilenBolgeIdQS = BolgeDDL.SelectedItem.Value.ConvertToInt();
                BeratDurumTablosunuDoldur();
                DosyaTablosunuDoldur();
            });
        }

        //Dosya islemleri
        private void DosyaTablosunuDoldur()
        {
            Bolge bolge = new BolgeService().GetSelected(SecilenBolgeIdQS, LoggingContext);
            string yaziDosyaAdi = bolge==null?SecilenBolgeIdQS.ToString():bolge.KisaAdi + "-" + MadalyaDDL.SelectedItem.Text;
            yaziDosyaAdi = yaziDosyaAdi.Replace(" ", "-");
            List<SPFile> fileList = DosyaListesiniGetir(ProjeConstants.NBYSBELGELERI_LIB, yaziDosyaAdi);
            var jsonData = ToJSON(fileList, yaziDosyaAdi); //veri çekilip json'a çevriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);

        }
        public string ToJSON(List<SPFile> fileList, string pDosyaAdi)
        {
            ViewState["DocumentFilePairs"] = new string[0];
            string json = "[]";

                if (fileList != null)
                {

                    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                    List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
                    Dictionary<string, object> childRow;
                    Bolge bolge = new BolgeService().GetSelected(SecilenBolgeIdQS, LoggingContext);
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
                        string zaman = new DateTime(SecilenYilQS.ConvertToInt(), SecilenAyQS.ConvertToInt(), 1).ToString("-MM_yyyy");
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
                    ViewState["DocumentFilePairs"] = parentRow.Select(row => Convert.ToString(row["FileName"]) + "|" + Convert.ToString(row["LabelFileName"])).ToArray();
                }

            return json;
        }
        private bool DosyaVarMi(string libName, string fileName)
        {
            SPWeb web = SPContext.Current.Web;
            string fileUrl = web.Url + "/" + libName + "/" + fileName;
            SPFile file = web.GetFile(fileUrl);
            return file != null && file.Exists;
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
                                return('<a href=# onclick=CallButtonClick(\''+row.FileName + '\',\''+row.LabelFileName + '\'); class=\'btn btn-outline-danger \'>Dosyalari Sil</a>');
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

        private bool DosyaSil(string libName, string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;
            SPList list = SPContext.Current.Web.Lists[libName];
            SPFile file = list.RootFolder.Files.Cast<SPFile>().FirstOrDefault(item => item.Name == fileName);
            if (file == null || !file.Exists) return false;
            file.Item.Recycle();
            return true;
        }
        private List<SPFile> DosyaListesiniGetir(string libName, string fileName)
        {
            List<SPFile> lstFile = new List<SPFile>();
            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists[libName];
            SPFolder folder = list.RootFolder;

            var matchedFiles = folder.Files.Cast<SPFile>()
                .Where(f => f.Name.Contains(fileName))
                .OrderByDescending(f => f.TimeLastModified);

            lstFile.AddRange(matchedFiles);
            return lstFile;
        }
        private bool IsListedFilePair(string name, string label)
        {
            var pairs = ViewState["DocumentFilePairs"] as string[];
            return !string.IsNullOrEmpty(name) && pairs != null && pairs.Contains(name + "|" + (label ?? string.Empty));
        }
        protected void DosyayiSilBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("DosyayiSilBtn_Click", () =>
            {
                if (!IsListedFilePair(paramDosyaAdiLbl.Value, paramEtiketDosyaAdiLbl.Value))
                { Messages.Warning("Silinecek belgeyi güncel dosya listesinden seçin."); return; }
                DosyaAdiLbl.Text = System.Web.HttpUtility.HtmlEncode(paramDosyaAdiLbl.Value);
                EtiketDosyaAdiLbl.Text = System.Web.HttpUtility.HtmlEncode(paramEtiketDosyaAdiLbl.Value);
                ViewState["ConfirmedFilePair"] = paramDosyaAdiLbl.Value + "|" + paramEtiketDosyaAdiLbl.Value;
                DosyayiSilNowBtn.Visible = true;
                var openPopup = "OpenModalOnay();";
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
            });
        }
        protected void DosyayiSilNowBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("DosyayiSilNowBtn_Click", () =>
            {
                string name = paramDosyaAdiLbl.Value, label = paramEtiketDosyaAdiLbl.Value;
                if (!IsListedFilePair(name, label) || Convert.ToString(ViewState["ConfirmedFilePair"]) != name + "|" + label)
                { Messages.Warning("Silinecek belge değişmiş. Güncel listeden yeniden seçip onaylayın."); return; }
                int deleted = 0;
                try
                {
                    if (DosyaSil(ProjeConstants.NBYSBELGELERI_LIB, name)) deleted++;
                    if (DosyaSil(ProjeConstants.NBYSBELGELERI_LIB, label)) deleted++;
                    ViewState["ConfirmedFilePair"] = null;
                    DosyayiSilNowBtn.Visible = false;
                    DosyaTablosunuDoldur();
                    if (deleted > 0) Messages.Success(deleted + " dosya geri dönüşüm kutusuna taşındı.", 4000);
                    else Messages.Info("Seçilen dosyalar bulunamadı. Dosya listesi yenilendi.");
                }
                catch (Exception error)
                {
                    NbysWebPartLog.ReportError(LoggingContext, "NBYS.BeratBelgesiYazisiWP.DeleteDocuments", error,
                        deleted + " dosya geri dönüşüm kutusuna taşındı; işlem tamamlanamadı. Dosya listesini yenileyip kontrol edin.");
                }
            });
        }
    }
}
