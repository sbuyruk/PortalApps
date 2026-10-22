using System.Security;
using System.Text;
using System.Xml;
using System.Web;
using NBYS_WebParts.Logging;
using Utility.Logging;
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.FTKYazilariWP
{
    [ToolboxItemAttribute(false)]
    public partial class FTKYazilariWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "FTKYazilariWP"); }
        }

        private void ExecuteAction(string operation, Action action)
        {
            NbysReportFeedback.Execute(LoggingContext, operation, action);
        }

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FTKYazilariWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        private int IliIdQS
        {
            get
            {

                if (ViewState["IliId"] == null)
                {
                    if (Page.Request.QueryString["IliId"] != null)
                    {
                        ViewState["IliId"] = Page.Request.QueryString["IliId"];
                    }
                    else
                    {
                        ViewState["IliId"] = string.Empty;
                    }
                }
                return ViewState["IliId"].ConvertToInt();
            }

            set
            {
                ViewState["IliId"] = value;
            }
        }
        private int IlcesiIdQS
        {
            get
            {

                if (ViewState["IlcesiId"] == null)
                {
                    if (Page.Request.QueryString["IlcesiId"] != null)
                    {
                        ViewState["IlcesiId"] = Page.Request.QueryString["IlcesiId"];
                    }
                    else
                    {
                        ViewState["IlcesiId"] = string.Empty;
                    }
                }
                return ViewState["IlcesiId"].ConvertToInt();
            }

            set
            {
                ViewState["IlcesiId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
                //time out olmasin diye
                ScriptManager _scriptMan = ScriptManager.GetCurrent(Page);
                if (_scriptMan != null) _scriptMan.AsyncPostBackTimeout = 36000;
                if (!Page.IsPostBack)
                {
                    int requestedProvince, requestedDistrict;
                    string provinceQuery = Page.Request.QueryString["IliId"];
                    string districtQuery = Page.Request.QueryString["IlcesiId"];
                    if ((!string.IsNullOrEmpty(provinceQuery) && (!int.TryParse(provinceQuery, out requestedProvince) || requestedProvince < 0))
                        || (!string.IsNullOrEmpty(districtQuery) && (!int.TryParse(districtQuery, out requestedDistrict)
                            || (requestedDistrict < 0 && requestedDistrict != ProjeConstants.VALILIK_INT))))
                        throw new NbysReportInputException("Geçerli bir il ve ilçe seçimiyle sayfayı yeniden açın.");
                    YonergeLnk.HRef = UtilityHelper.YonergeURLGetir(ProjeConstants.PARAM_FTKYONERGE, ProjeConstants.NBYSBELGELERI_LIB, ProjeConstants.PAGE_FTK_YAZILARI, LoggingContext);
                    IlDDLDoldur();
                    if (IliIdQS > 0 && IliDDL.Items.FindByValue(IliIdQS.ToString()) == null)
                        throw new NbysReportInputException("Seçilen il için FTK kaydı bulunamadı. FTK listesinden tekrar seçin.");
                    UtilityHelper.SetDDLValue(IliDDL, IliIdQS.ToString());
                    IlceDDLDoldur();
                    if (IlcesiIdQS > 0 && IlcesiDDL.Items.FindByValue(IlcesiIdQS.ToString()) == null)
                        throw new NbysReportInputException("Seçilen ilçe bu ilde bulunamadı. FTK listesinden tekrar seçin.");
                    UtilityHelper.SetDDLValue(IlcesiDDL, IlcesiIdQS.ToString());
                    FormuDoldur();
                }
            });
        }
        private void FormuDoldur()
        {
            DateTime bugun = DateTime.Today;
            EvrakSayisiTxt.Text = "TSKGV.62-14-" + bugun.Year + "/";
            CultureInfo culture = new CultureInfo(ProjeConstants.CULTUREINFO);
            EvrakTarihiTxt.Text = bugun.ToString("dd MMMM yyyy", culture).ToUpper(culture);

            ImzalayanTxt.Text = "Erhan SİPAHİOĞLU";
            ImzalayanUnvanTxt.Text = "Genel Müdür Yardımcısı";
            ImzalayanMakamTxt.Text = string.Empty;

            EkImzaTxt.Text = "Murat DİRİCAN";
            EkImzaUnvanTxt.Text = "B.H.Direktörü";

            string irtibat = ReadContactParameter();
            IrtibatNoktasiTxt.Text = string.IsNullOrEmpty(irtibat) ? "Dorukhan GÜNDÜR (Dâhili Tel:261)" : irtibat;

        }

        private string ReadContactParameter()
        {
            NBYSParametre parameter = new NBYSParametreService(LoggingContext).GetByGroupAndKey(ProjeConstants.PARAM_FTKYAZI, ProjeConstants.PARAM_FTKYAZI_IRTIBAT);
            return parameter == null ? string.Empty : parameter.Deger;
        }

        private void IlDDLDoldur()
        {
            IliDDL.Items.Clear();
            List<Il> list = new NbysSupportReadService(LoggingContext).GetFtkProvinces();
            foreach (Il il in list)
            {
                if (string.IsNullOrEmpty(il.IlAdi.Trim()))
                    continue;
                IliDDL.Items.Add(new System.Web.UI.WebControls.ListItem(il.IlAdi, il.Id.ToString()));
            }
        }
        private void IlceDDLDoldur()
        {
            IlcesiDDL.Items.Clear();
            IlcesiDDL.Items.Add(new System.Web.UI.WebControls.ListItem(ProjeConstants.VALILIK, ProjeConstants.VALILIK_INT.ToString()));
            if (IliDDL.SelectedItem != null)
            {
                List<Ilce> list = new NbysSupportReadService(LoggingContext).GetFtkDistricts(IliDDL.SelectedValue.ConvertToInt());
                foreach (Ilce ilce in list)
                {
                    if (ilce.IlceAdi.ToUpper().Equals(ProjeConstants.ILCE_MERKEZ.ToUpper()))
                        continue;
                    IlcesiDDL.Items.Add(new System.Web.UI.WebControls.ListItem(ilce.IlceAdi, ilce.Id.ToString()));
                }
            }
        }
        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
            Page.Response.Redirect(currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl, false);
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        private MemoryStream GetTemplateStream(string templateFileName)
        {
            SPWeb web = SPContext.Current.Web;
            SPFile file = web.GetFile(web.Url + "/" + ProjeConstants.NBYSBELGELERI_LIB + "/" + templateFileName);
            if (file == null || !file.Exists) throw new InvalidOperationException("FTK yazı şablonu bulunamadı.");
            byte[] bytes = file.OpenBinary();
            if (bytes.Length == 0) throw new InvalidOperationException("FTK yazı şablonu boş.");
            MemoryStream stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;
            return stream;
        }
        private bool YaziOlustur(string dosyaAdi, string templateFileName)
        {
            using (MemoryStream stream = GetTemplateStream(templateFileName))
            {
                AddData2DestinationStream(stream, null);
                stream.Position = 0;
                AddToSharePoint(stream, dosyaAdi);
            }
            string url = SPContext.Current.Web.Url + "/" + ProjeConstants.NBYSBELGELERI_LIB + "/" + dosyaAdi;
            DosyaLnk.Text = dosyaAdi;
            DosyaLnk.NavigateUrl = SPContext.Current.Web.Url + "/_layouts/15/download.aspx?SourceUrl=" + HttpUtility.UrlEncode(url);
            DosyaLnk.Visible = true;
            return true;
        }
        private string kaymakamAdi = string.Empty;
        private string ftkBaskaniAdi = string.Empty;
        private List<FTK> letterMembers;
        private bool letterSaved;
        private bool letterUploadStarted;
        private MemoryStream AddData2DestinationStream(MemoryStream templateStream, IEnumerable<Paragraph> templateParagraphs)
        {

            string ilAdi = IliDDL.SelectedItem.Text;
            string ilceAdi = IlcesiDDL.SelectedItem.Text;
            string bolge = BolgeGetir(ilAdi);
            CultureInfo culture = new CultureInfo(ProjeConstants.CULTUREINFO);
            string ilAdiBuyukHarf = ilAdi.ToUpper(culture);
            string ilceAdiBuyukHarf = ilceAdi.ToUpper(culture);
            //create key value pair, key represents words to be replace and
            //values represent values in document in place of keys.
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            UyeListesiniDoldur(templateStream);

            keyValues.Add("BelgeSayisiVar", EvrakSayisiTxt.Text);
            keyValues.Add("BelgeTarihiVar", EvrakTarihiTxt.Text);

            string ilgiVar = (ilceAdi.Equals(ProjeConstants.VALILIK) ? ilAdi + " Valiliğinin " : ilceAdi + " Kaymakamlığının ") + IlgiTarihiTxt.Text + " tarih ve " + IlgiSayisiTxt.Text + " sayılı yazısı.";
            keyValues.Add("IlgiVar", ilgiVar);

            keyValues.Add("BaslikDosyaVar", "(DOSYA)");
            keyValues.Add("BaslikIlceVar", ilceAdiBuyukHarf + " KAYMAKAMLIK MAKAMINA ");
            keyValues.Add("BaslikIlVar", ilAdiBuyukHarf + " VALİLİK MAKAMINA ");
            keyValues.Add("BaslikBolgeVar", bolge.ToUpper(culture) + " BÖLGE TEMSİLCİLİĞİNE ");

            keyValues.Add("IlAdiVar", ilAdi);
            keyValues.Add("IlBuyukHarfVar", ilAdiBuyukHarf);

            keyValues.Add("IlceAdiVar", ilceAdi);
            keyValues.Add("IlceBuyukHarfVar", ilceAdiBuyukHarf);

            keyValues.Add("IlceIIAdiBuyukHarfVar", ilceAdiBuyukHarf + "/" + ilAdiBuyukHarf);
            keyValues.Add("BolgeAdiVar", bolge);

            keyValues.Add("KaymakamAdiVar", kaymakamAdi);
            keyValues.Add("FTKBskVar", ftkBaskaniAdi);

            keyValues.Add("IrtibatVar", IrtibatNoktasiTxt.Text);
            keyValues.Add("EkImzaVar", EkImzaTxt.Text);
            keyValues.Add("EkImzaUnvanVar", EkImzaUnvanTxt.Text);

            keyValues.Add("ImzaVar", ImzalayanTxt.Text);
            keyValues.Add("UnvanVar", ImzalayanUnvanTxt.Text);
            keyValues.Add("MakamVar", ImzalayanMakamTxt.Text);

            MemoryStream destinationStream = CopyAndSearchAndReplace(templateStream, templateParagraphs, keyValues);
            return destinationStream;
        }

        private string BolgeGetir(string ilAdi)
        {
            Il province = new IlService().GetById(IliDDL.SelectedValue.ConvertToInt(), LoggingContext);
            if (province == null) throw new NbysReportInputException("Seçilen il bulunamadı. Sayfayı yenileyip tekrar seçin.");
            Bolge region = new BolgeService().GetById(province.BolgeId, LoggingContext);
            if (region == null) throw new InvalidOperationException("İlin bölge bilgisi bulunamadı.");
            return region.KisaAdi ?? string.Empty;
        }

        private void UyeListesiniDoldur(MemoryStream templateStream)
        {
            using (WordprocessingDocument wordDoc2 = WordprocessingDocument.Open(templateStream, true))
            {
                int ilId = IliDDL.SelectedItem.Value.ConvertToInt();
                int ilcesiId = IlcesiDDL.SelectedItem.Value.ConvertToInt();
                var doc = wordDoc2.MainDocumentPart.Document;
                var body = doc.Body;
                var paras = body.Elements<Paragraph>();

                List<FTK> list = letterMembers;
                int counter = 0;
                foreach (var item in list)
                {

                    switch (item.FTKGorevi)
                    {
                        case ProjeConstants.FTK_GOREVI_FAHRIBASKAN:
                            {
                                kaymakamAdi = (item.Adi ?? string.Empty).Trim() + " " + (item.Soyadi ?? string.Empty).Trim();
                                break;
                            }
                        case ProjeConstants.FTK_GOREVI_BASKAN:
                            {
                                ftkBaskaniAdi = (item.Adi ?? string.Empty).Trim() + " " + (item.Soyadi ?? string.Empty).Trim();
                                break;
                            }

                        default:
                            break;
                    }

                    counter++;
                    string ftkGorevi = item.FTKGorevi;
                    string adiSoyadi = ((item.Adi ?? string.Empty).Trim() + " " + (item.Soyadi ?? string.Empty).Trim());
                    string unvani = item.Unvani;
                    string kartno = item.KartNo;

                    string ftkGoreviVar = string.IsNullOrEmpty(ftkGorevi) ? "" : ftkGorevi;
                    string adiSoyadiVar = string.IsNullOrEmpty(adiSoyadi) ? "" : adiSoyadi;
                    string unvaniVar = string.IsNullOrEmpty(unvani) ? "" : unvani;
                    string kartnoVar = string.IsNullOrEmpty(kartno) ? "" : kartno;

                    Table table1 = doc.Body.Descendants<Table>().ElementAt(0);
                    //Table table2 = doc.Body.Descendants<Table>().ElementAt(1);
                    //Table table3 = doc.Body.Descendants<Table>().ElementAt(2);


                    TabloyaUyeEkle(table1, ftkGoreviVar, adiSoyadiVar, unvaniVar, kartnoVar);
                    //TabloyaUyeEkle(table2, ftkGoreviVar, adiSoyadiVar, unvaniVar, kartnoVar);
                    //TabloyaUyeEkle(table3, ftkGoreviVar, adiSoyadiVar, unvaniVar, kartnoVar);
                    //Bölgelere yazi gitmeyecek o yüzden kapatildi 05.06.2024
                    //if (!bolge.Equals(ProjeConstants.BOLGE_GENELMUDURLUK))
                    //{
                    //    Table table4 = doc.Body.Descendants<Table>().ElementAt(3);
                    //    TabloyaUyeEkle(table4, ftkGoreviVar, adiSoyadiVar, unvaniVar, kartnoVar);
                    //}


                }
            }
        }
        private void TabloyaUyeEkle(Table myTable, string ftkGoreviVar, string adiSoyadiVar, string unvaniVar, string kartnoVar)
        {
            TableRow theRow = myTable.Elements<TableRow>().First();

            TableRow rowCopy = (TableRow)theRow.CloneNode(true);

            var runPropGorevi = GetRunPropertyFromTableCell(rowCopy, 0);

            var runPropAdiSoyadi = GetRunPropertyFromTableCell(rowCopy, 0);
            var runPropUnvani = GetRunPropertyFromTableCell(rowCopy, 0);
            var runPropKartNo = GetRunPropertyFromTableCell(rowCopy, 0);

            var runFtkGorevi = new Run(new Text(ftkGoreviVar));
            runFtkGorevi.PrependChild<RunProperties>(runPropGorevi);

            var runAdiSoyadi = new Run(new Text(adiSoyadiVar));
            runAdiSoyadi.PrependChild<RunProperties>(runPropAdiSoyadi);

            var runUnvani = new Run(new Text(unvaniVar));
            runUnvani.PrependChild<RunProperties>(runPropUnvani);

            var runKartNo = new Run(new Text(kartnoVar));
            runKartNo.PrependChild<RunProperties>(runPropKartNo);

            rowCopy.Descendants<TableCell>().ElementAt(0).RemoveAllChildren<Paragraph>();//removes that text of the copied cell
            rowCopy.Descendants<TableCell>().ElementAt(0).Append(new Paragraph(new ParagraphProperties(new Justification() { Val = JustificationValues.Left }), runFtkGorevi));
            rowCopy.Descendants<TableCell>().ElementAt(1).RemoveAllChildren<Paragraph>();
            rowCopy.Descendants<TableCell>().ElementAt(1).Append(new Paragraph(new ParagraphProperties(new Justification() { Val = JustificationValues.Left }), runAdiSoyadi));
            rowCopy.Descendants<TableCell>().ElementAt(2).RemoveAllChildren<Paragraph>();
            rowCopy.Descendants<TableCell>().ElementAt(2).Append(new Paragraph(new ParagraphProperties(new Justification() { Val = JustificationValues.Left }), runUnvani));
            rowCopy.Descendants<TableCell>().ElementAt(3).RemoveAllChildren<Paragraph>();
            rowCopy.Descendants<TableCell>().ElementAt(3).Append(new Paragraph(new ParagraphProperties(new Justification() { Val = JustificationValues.Left }), runKartNo));

            myTable.AppendChild(rowCopy);
        }
        private static RunProperties GetRunPropertyFromTableCell(TableRow rowCopy, int cellIndex)
        {
            RunFonts fonts = rowCopy.Descendants<TableCell>().ElementAt(cellIndex).Descendants<RunFonts>().FirstOrDefault();
            string name = fonts != null && fonts.Ascii != null ? fonts.Ascii.Value : "Arial";
            RunProperties properties = new RunProperties();
            properties.AppendChild(new RunFonts { Ascii = name, HighAnsi = name, ComplexScript = name });
            properties.AppendChild(new FontSize { Val = "22" });
            properties.AppendChild(new Languages { Val = "tr-TR" });
            return properties;
        }
        public MemoryStream CopyAndSearchAndReplace(MemoryStream destinationStream, IEnumerable<Paragraph> templateParagraphs, Dictionary<string, string> dict)
        {
            using (WordprocessingDocument document = WordprocessingDocument.Open(destinationStream, true))
            {
                if (document.MainDocumentPart == null) throw new InvalidOperationException("FTK şablonunda belge bölümü bulunamadı.");
                string xml;
                using (StreamReader reader = new StreamReader(document.MainDocumentPart.GetStream())) xml = reader.ReadToEnd();
                // Replace once as literal XML text; input must not become XML or replacement syntax.
                xml = Regex.Replace(xml, string.Join("|", dict.Keys.Select(Regex.Escape)),
                    match => SecurityElement.Escape(dict[match.Value] ?? string.Empty));
                XmlDocument content = new XmlDocument { PreserveWhitespace = true, XmlResolver = null };
                using (XmlReader reader = XmlReader.Create(new StringReader(xml),
                    new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null })) content.Load(reader);
                using (XmlWriter writer = XmlWriter.Create(document.MainDocumentPart.GetStream(FileMode.Create),
                    new XmlWriterSettings { Encoding = new UTF8Encoding(false), Indent = false, CloseOutput = true })) content.Save(writer);
            }
            return destinationStream;
        }

        protected void AddToSharePoint(MemoryStream memStream, string fileName)
        {
            memStream.Position = 0;
            SPList library = SPContext.Current.Web.Lists[ProjeConstants.NBYSBELGELERI_LIB];
            letterUploadStarted = true;
            SPFile file = library.RootFolder.Files.Add(fileName, memStream, false);
            if (file != null) letterSaved = true;
            if (file == null || !file.Exists) throw new InvalidOperationException("FTK yazısı kitaplığa kaydedilemedi.");
        }
        protected void BolgeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("BolgeDDL_SelectedIndexChanged", () =>
            {
                IlDDLDoldur();
            });
        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("IliDDL_SelectedIndexChanged", () =>
            {
                IliIdQS = IliDDL.SelectedItem.Value.ConvertToInt();
                IlceDDLDoldur();
            });
        }
        protected void IlcesiDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("IlcesiDDL_SelectedIndexChanged", () =>
            {
                IlcesiIdQS = IlcesiDDL.SelectedItem.Value.ConvertToInt();
            });
        }
        protected void KurulusYazisiOlusturBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("KurulusYazisiOlusturBtn_Click", () =>
            {
                CreateLetter(false);
            });
        }
        protected void GuncellemeYazisiOlusturBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("GuncellemeYazisiOlusturBtn_Click", () =>
            {
                CreateLetter(true);
            });
        }
        private void RequireSelection()
        {
            int provinceId, districtId;
            if (IliDDL.SelectedItem == null || IlcesiDDL.SelectedItem == null
                || !int.TryParse(IliDDL.SelectedValue, out provinceId) || provinceId <= 0
                || !int.TryParse(IlcesiDDL.SelectedValue, out districtId)
                || (districtId <= 0 && districtId != ProjeConstants.VALILIK_INT))
                throw new NbysReportInputException("Geçerli bir il ve ilçe seçin.");
            IliIdQS = provinceId;
            IlcesiIdQS = districtId;
            Ilce district = districtId == ProjeConstants.VALILIK_INT ? null : new IlceService().GetById(districtId, LoggingContext);
            if (districtId != ProjeConstants.VALILIK_INT && (district == null || district.IlId != provinceId))
                throw new NbysReportInputException("İlçe seçilen ile ait değil. İl ve ilçeyi yeniden seçin.");
        }

        private void CreateLetter(bool update)
        {
            DosyaLnk.Visible = false;
            letterSaved = false;
            letterUploadStarted = false;
            RequireSelection();
            if (!update && IlcesiIdQS == ProjeConstants.VALILIK_INT)
                throw new NbysReportInputException("İl kuruluş yazısı bu ekranda hazırlanamaz. İlçe seçin veya güncelleme yazısını kullanın.");
            if (string.IsNullOrWhiteSpace(EvrakSayisiTxt.Text) || string.IsNullOrWhiteSpace(EvrakTarihiTxt.Text)
                || string.IsNullOrWhiteSpace(ImzalayanTxt.Text))
                throw new NbysReportInputException("Evrak sayısını, tarihini ve imzalayan bilgisini girin.");
            letterMembers = new FTKService(LoggingContext).GetLatest(IliIdQS, IlcesiIdQS);
            if (letterMembers.Count == 0) throw new NbysReportInputException("Seçilen il veya ilçede kayıtlı FTK üyesi bulunamadı.");
            bool notifying = false;
            try
            {
                string kind = update ? "Guncelleme" : "Kurulus";
                string name = "FTK-" + kind + "-" + IliIdQS + "-" + IlcesiIdQS + "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss")
                    + "-" + Guid.NewGuid().ToString("N") + ".docx";
                string template = update ? (IlcesiIdQS == ProjeConstants.VALILIK_INT
                    ? ProjeConstants.FTK_IL_GUNCELLEME_YAZI_TEMPLATE : ProjeConstants.FTK_ILCE_GUNCELLEME_YAZI_TEMPLATE)
                    : ProjeConstants.FTK_ILCE_KURLUMANAYAZI_TEMPLATE;
                YaziOlustur(name, template);
                string prepared = update ? "FTK güncelleme yazısı hazırlandı." : "FTK kuruluş yazısı hazırlandı.";
                if (!EPostaGonderChk.Checked)
                {
                    Messages.Success(prepared + " Dosya bağlantısından indirebilirsiniz. E-posta gönderilmedi.", 5000);
                    return;
                }
                notifying = true;
                string listUrl = new Uri(Page.Request.Url, ProjeConstants.PAGE_FTK_LIST).GetLeftPart(UriPartial.Path)
                    + "?IliId=" + IliIdQS + "&IlcesiId=" + IlcesiIdQS;
                NbysFtkNotificationResult result = new NbysFtkNotificationService(LoggingContext).Send(IliIdQS, IlcesiIdQS, update, listUrl);
                if (result == NbysFtkNotificationResult.NoRecipients)
                    Messages.Warning(prepared + " Bildirim alıcısı bulunamadığı için e-posta gönderilmedi. Bölge iletişim bilgilerini kontrol edin.");
                else Messages.Success(prepared + " E-posta bildirimi gönderim sunucusuna iletildi. Dosya bağlantısından indirebilirsiniz.", 5000);
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                System.Web.HttpContext.Current.Items["PortalApps.NBYS.FailedWebPart." + LoggingContext.WebPart] = true;
                string message = notifying ? "Yazı hazırlandı. E-posta gönderiminin tamamlandığı doğrulanamadı. Tekrar göndermeden önce alıcıları kontrol edin."
                    : letterSaved ? "Yazı kitaplığa kaydedildi; ekran tamamlanamadı. Belge kitaplığını kontrol edin. E-posta gönderilmedi."
                    : letterUploadStarted ? "Yazının kitaplığa kaydedildiği doğrulanamadı. Yeniden oluşturmadan önce belge kitaplığını kontrol edin. E-posta gönderilmedi."
                    : "FTK yazısı hazırlanamadı. Lütfen tekrar deneyin. E-posta gönderilmedi.";
                NbysWebPartLog.ReportError(LoggingContext, "NBYS.FTKYazilariWP.CreateLetter", error, message);
            }
        }

        protected void FTKIslemleriBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("FTKIslemleriBtn_Click", () =>
            {
                RequireSelection();
                RedirectToPage(ProjeConstants.PAGE_FTKISLEMLERI + "?IliId=" + IliIdQS + "&IlcesiId=" + IlcesiIdQS);
            });
        }
        protected void FTKListesiBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("FTKListesiBtn_Click", () =>
            {
                RequireSelection();
                RedirectToPage(ProjeConstants.PAGE_FTK_LIST + "?IliId=" + IliIdQS + "&IlcesiId=" + IlcesiIdQS);
            });
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_HOME);
            });
        }
    }
}
