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
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.DuzenliBagisciBelgesiBasimiWP
{
    [ToolboxItemAttribute(false)]
    public partial class DuzenliBagisciBelgesiBasimiWP : WebPart
    {
        private PortalLogContext LoggingContext { get { return PortalLogContext.FromCurrentRequest("NBYS", "DuzenliBagisciBelgesiBasimiWP"); } }
        private Bolge userRegion;
        private bool canProcess = true;
        private NbysDocumentBatch documentBatch;
        private int shortenedAddressCount;
        private bool documentSaved;
        private NbysDocumentBatch GetDocumentBatch()
        {
            if (documentBatch == null) documentBatch = new NbysDocumentWriteService(LoggingContext).Prepare(GetBasTar(), GetBitTar(), ProjeConstants.ARMAGAN_DUZENLIBAGISCIBELGESIID, BolgeDDL.SelectedValue.ConvertToInt());
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
        public DuzenliBagisciBelgesiBasimiWP()
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
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
                userRegion = new NbysBagisciWriteService(LoggingContext).GetUserRegion();
                if (userRegion == null) { canProcess = false; Messages.Warning("Bölge bilginiz bulunamadı. Yetki bilgilerinizi kontrol ettirin."); return; }
                if (Page.IsPostBack && !NbysDocumentWriteService.IsCentral(userRegion.Id)
                    && BolgeDDL.SelectedValue != userRegion.Id.ToString())
                { canProcess = false; Messages.Warning("Bölge yetkiniz değişmiş. Sayfayı yeniden açıp seçiminizi kontrol edin."); return; }
                //time out olmasin diye
                ScriptManager _scriptMan = ScriptManager.GetCurrent(Page);
                _scriptMan.AsyncPostBackTimeout = 36000;
                if (!Page.IsPostBack)
                {
                    YonergeLnk.HRef = UtilityHelper.YonergeURLGetir(ProjeConstants.PARAM_NBYSYONERGE, ProjeConstants.NBYSBELGELERI_LIB, ProjeConstants.PAGE_DUZENLIBAGISCI_LIST);
                    FillDropDownList();
                    SetDDLValues(); //ay ve yili querystringden al
                    SetSecilenBasTarBitTar();
                    FormuDoldur();
                    FillDurumValues();
                }
                if (!Page.IsPostBack) TabloOlustur();
            });
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
            AyDDLDoldur();
            YilDDLDoldur();
            BolgeDDLDoldur();
        }
        private void AyDDLDoldur()
        {

            AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Hepsi", "0"));
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
            YilDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Hepsi", "0"));
            for (int i = year-3; i <= year; i++)
            {
                YilDDL.Items.Add(new System.Web.UI.WebControls.ListItem(i.ToString(), i.ToString()));
            }
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

        }
        private void SetDDLValues()
        {
            SetFilter(AyDDL, SecilenAyQS, DateTime.Today.Month.ToString());
            SetFilter(YilDDL, SecilenYilQS, DateTime.Today.Year.ToString());
            SecilenAyQS = AyDDL.SelectedValue;
            SecilenYilQS = YilDDL.SelectedValue;
            SetFilter(BolgeDDL, userRegion.Id.ToString(), userRegion.Id.ToString());
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("AyDDL_SelectedIndexChanged", () =>
            {
                SecilenAyQS = AyDDL.SelectedItem.Value.ToString();
                SetSecilenBasTarBitTar();
                FillDurumValues();
                TabloOlustur();
                //KayitGetir();
            });
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("YilDDL_SelectedIndexChanged", () =>
            {
                SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
                SetSecilenBasTarBitTar();
                FillDurumValues();
                TabloOlustur();
                //KayitGetir();
            });
        }
        protected void GunDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("GunDDL_SelectedIndexChanged", () =>
            {
                SetSecilenBasTarBitTar();
                FillDurumValues();
                TabloOlustur();
            });
        }
        protected void BolgeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("BolgeDDL_SelectedIndexChanged", () =>
            {
                SetSecilenBasTarBitTar();
                FillDurumValues();
                TabloOlustur();
            });
        }
        private void SetSecilenBasTarBitTar()
        {
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();

            DateTime bastar = DateTime.Today;
            DateTime bittar = DateTime.Today;

            if (yil == 0)
            {
                // Hepsi: son 3 yılı kapsa
                bastar = new DateTime(DateTime.Today.Year - 2, 1, 1);
                bittar = new DateTime(DateTime.Today.Year, 12, 31);
            }
            else if (ay == 0)
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
        private void TabloOlustur()
        {
            var jsonData = GetData(); //veri çekilip json'a çevriliyor

            bool jasonDataBosMu = string.IsNullOrWhiteSpace(jsonData.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", ""));
            if (!jasonDataBosMu)
            {
                DosyaOlusturBtn.Visible = true;
                AdresOlusturBtn.Visible = true;
                var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
                UtilityHelper.ScriptCalistir(jsString);
            }
            else
            {
                TableDataLbl.Text = "Düzenli Bağışçı Belgesi bulunmamaktadır.";
                DosyaOlusturBtn.Visible = false;
                AdresOlusturBtn.Visible = false;
            }
        }
        private string GetData()
        {
            int rowCount = 0;
            DateTime bastar = GetBasTar();
            DateTime bittar = GetBitTar();

            var json = new ArmaganService(LoggingContext).ListByDurumTarihJson(ProjeConstants.DURUM_KONTROLEDILDI, bastar, bittar, ProjeConstants.ARMAGAN_DUZENLIBAGISCIBELGESIID.ToString(), ref rowCount, BolgeDDL.SelectedItem.Value.ConvertToInt(), ProjeConstants.HEPSI_INT);
            TableDataLbl.Text = rowCount + " adet Düzenli Bağışçı Belgesi mevcut";
            if (rowCount > 0)
            {
                DosyaOlusturBtn.Visible = true;
                AdresOlusturBtn.Visible = true;
                DurumChk.Visible = true;
            }
            else
            {
                DosyaOlusturBtn.Visible = false;
                AdresOlusturBtn.Visible = false;
                DurumChk.Visible = false;
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
                        { data: 'ArmaganTutari', 'width':'10%', 'className': 'text-end'},

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
            SetSecilenBasTarBitTar();
            return SecilenBastarQS.ConvertToDatetime();
        }
        private DateTime GetBitTar()
        {
            SetSecilenBasTarBitTar();
            return SecilenBittarQS.ConvertToDatetime();
        }
        private void FillDurumValues()
        {
            BelgeTable.Controls.Clear();
            DosyaOlusturBtn.Visible = false;
            DurumChk.Visible = false;

            DataTable dataTable = new ArmaganService(LoggingContext).CountDurumByBolge(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_DUZENLIBAGISCIBELGESIID, BolgeDDL.SelectedItem.Value.ConvertToInt());
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
                            AdresOlusturBtn.Visible = true;
                            DurumChk.Visible = true;
                            //durumChk.Enabled = true;
                        }
                        else
                        {
                            DosyaOlusturBtn.Visible = false;
                            AdresOlusturBtn.Visible = false;
                            DurumChk.Visible = false;
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
                    BelgeTable.Controls.Add(tblrow);
                }
            }

        }
        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            int index = currentUrl.LastIndexOf("?") < 0 ? currentUrl.Length : currentUrl.LastIndexOf("?");

            string rawUrl = currentUrl.Substring(0, index);
            string newUrl = rawUrl.Substring(0, rawUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl, false);
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
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
                DosyaLnk.Visible = false;
                var batch = GetDocumentBatch();
                string name = "DuzenliBagisciBelgesi-" + BolgeDDL.SelectedValue + "-(" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss-fff") + ")-" + Guid.NewGuid().ToString("N").Substring(0, 8) + ".docx";
                NbysDocumentFeedback.Run(LoggingContext, batch, () => { DuzenliBagisciBelgesiDosyasiOlustur(name); }, DurumChk.Checked,
                    () => { FillDurumValues(); TabloOlustur(); }, "Belgeler hazırlandı. Dosya bağlantısından indirebilirsiniz.", AddressNote, () => documentSaved);
            });
        }
        protected void AdresOlusturBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("AdresOlusturBtn_Click", () =>
            {
                AdresEtiketLnk.Visible = false;
                var batch = GetDocumentBatch();
                string name = "Adres-Etiketi-" + BolgeDDL.SelectedValue + "-(" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss-fff") + ")-" + Guid.NewGuid().ToString("N").Substring(0, 8) + ".docx";
                NbysDocumentFeedback.Run(LoggingContext, batch, () => { YeniAdresEtiketDosyasiOlustur(name); }, false,
                    () => { }, "Adres etiketleri hazırlandı. Dosya bağlantısından indirebilirsiniz.", AddressNote, () => documentSaved);
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
        private bool DuzenliBagisciBelgesiDosyasiOlustur(string dosyaAdi)
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
                foreach (DataRow row in dataTable.Rows)
                {
                    string nakitBagisciAdi = row["NakitBagisciAdi"].ToString();
                    string belgedeYazanIsim = row["BelgedeYazanIsim"].ToString();
                    bool bagisMiktariYazmasin = row["BagisMiktariYazmasin"].ReturnFalseIfNull().ConvertToBool();
                    string belgeNo = row["ArmaganId"].ToString();
                    string nakitBagisciTC = row["NakitBagisciTC"].ToString();
                    DateTime tarih = row["Tarih"].ConvertToDatetime();
                    int kacinciBelge = row["KacinciBelge"].ReturnZeroIfNull().ConvertToInt();

                    decimal tutar = row["Tutar"].ConvertToDecimal();
                    string tutarStr = bagisMiktariYazmasin ? string.Empty : tutar.ToString("N", culturInfo) + " TL'lik";

                    //create key value pair, key represents words to be replace and 
                    //values represent values in document in place of keys.
                    Dictionary<string, string> keyValues = new Dictionary<string, string>();
                    keyValues.Add("BelgeTarihiVar", EvrakTarihiTxt.Text);
                    keyValues.Add("BelgeNoVar", belgeNo);
                    keyValues.Add("BagisciAdiVar", string.IsNullOrEmpty(belgedeYazanIsim) ? nakitBagisciAdi : belgedeYazanIsim);
                    keyValues.Add("BagisTarihiVar", tarih.ToString("dd MMMM yyyy"));
                    keyValues.Add("TutarVar", tutarStr);
                    keyValues.Add("YilVar", BelgeYilMetniGetir(kacinciBelge));

                    keyValues.Add("ImzaVar", ImzalayanTxt.Text);
                    keyValues.Add("UnvanVar", ImzalayanUnvanTxt.Text);
                    keyValues.Add("MakamVar", ImzalayanMakamTxt.Text);
                    //destinationStream = SearchAndReplace(templateStream, keyValues);

                    destinationStream = CopyAndSearchAndReplace(templateStream, templateParagraphs, keyValues);
                    // Dokümanin sonuna bos sayfa yani fazladan sayfa atiyor
                    if (row != dataTable.Rows[dataTable.Rows.Count - 1])
                    {
                        destinationStream = AddParagraph2DestinationStream(destinationStream, templateParagraphs);
                    }
                }
            }

            return destinationStream;
        }

        private static string BelgeYilMetniGetir(int kacinciBelge)
        {
            // Eski kayıtlardaki boş/0 belge sırası ilk bağış yılına aittir.
            string yilMetni = SayiyiYaziyaCevir(Math.Max(1, kacinciBelge));
            return char.ToUpper(yilMetni[0], CultureInfo.GetCultureInfo("tr-TR"))
                + yilMetni.Substring(1) + " yıl";
        }

        private static string SayiyiYaziyaCevir(int sayi)
        {
            string[] birler = { "", "bir", "iki", "üç", "dört", "beş", "altı", "yedi", "sekiz", "dokuz" };
            string[] onlar = { "", "on", "yirmi", "otuz", "kırk", "elli", "altmış", "yetmiş", "seksen", "doksan" };
            int[] basamaklar = { 1000000000, 1000000, 1000, 100 };
            string[] basamakAdlari = { "milyar", "milyon", "bin", "yüz" };

            for (int i = 0; i < basamaklar.Length; i++)
            {
                int basamak = basamaklar[i];
                if (sayi < basamak)
                    continue;

                int adet = sayi / basamak;
                string onEk = adet == 1 && basamak <= 1000 ? string.Empty : SayiyiYaziyaCevir(adet) + " ";
                string kalan = sayi % basamak == 0 ? string.Empty : " " + SayiyiYaziyaCevir(sayi % basamak);
                return onEk + basamakAdlari[i] + kalan;
            }

            if (sayi >= 10)
                return onlar[sayi / 10] + (sayi % 10 == 0 ? string.Empty : " " + birler[sayi % 10]);

            return birler[sayi];
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
                return destinationStream;
            }
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
                ////bos sayfa ekle
                //Paragraph PageBreakParagraph = new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break() { Type = BreakValues.Page }));
                //wordDoc.MainDocumentPart.Document.Body.Append(PageBreakParagraph);

                ////template yaziyi ekle
                //foreach (var paragraph in templateParagraphs)
                //{
                //    Paragraph newPara = (Paragraph)paragraph.CloneNode(true);// new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break() { Type = BreakValues.Page }));
                //    wordDoc.MainDocumentPart.Document.Body.Append(newPara);
                //}
                return destinationStream;
            }

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
                    string telefon = row == null ? "" : row["Telefon"].ToString();
                    string adres = row == null ? "" : row["Adres"].ToString() + " " +telefon;
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
                    string fileUrl = web.Url + "/" + ProjeConstants.NBYSBELGELERI_LIB + "/DuzenliBagisciBelgesiTemplate.docx";
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
    }
}
