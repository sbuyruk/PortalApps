using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.SharePoint;
using Model.NBYS;
using Model.Ortak;
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
        private string IliIdQS
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
                return ViewState["IliId"].ToString();
            }

            set
            {
                ViewState["IliId"] = value;
            }
        }
        private string IlcesiIdQS
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
                return ViewState["IlcesiId"].ToString();
            }

            set
            {
                ViewState["IlcesiId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //time out olmasın diye
            ScriptManager _scriptMan = ScriptManager.GetCurrent(Page);
            _scriptMan.AsyncPostBackTimeout = 36000;
            if (!Page.IsPostBack)
            {
                YonergeLnk.HRef = NBYSOrtak.YonergeURLGetir(ProjeConstants.PARAM_FTKYONERGE, ProjeConstants.PAGE_FTK_YAZILARI);
                IlDDLDoldur();
                UtilityHelper.SetDDLValue(IliDDL, IliIdQS);
                IlceDDLDoldur();
                UtilityHelper.SetDDLValue(IlcesiDDL, IlcesiIdQS);
                FormuDoldur();
            }

        }
        private void FormuDoldur()
        {
            DateTime bugun = DateTime.Today;
            EvrakSayisiTxt.Text = "TSKGV.62-14-" + bugun.Year + "/";
            EvrakTarihiTxt.Text = bugun.ToString("dd") + " " + bugun.ToString("MMMM").ToUpper() + " " + bugun.Year;

            ImzalayanTxt.Text = @"Bilal TOPÇU";
            ImzalayanUnvanTxt.Text = string.Empty;// @"(E)Tümgeneral";
            ImzalayanMakamTxt.Text = @"TSKGV Genel Müdürü";

            string parafe1 = NBYSOrtak.ParametreGetir(ProjeConstants.PARAM_FTKYAZI, ProjeConstants.PARAM_FTKYAZI_PARAFE1);
            string parafe2 = NBYSOrtak.ParametreGetir(ProjeConstants.PARAM_FTKYAZI, ProjeConstants.PARAM_FTKYAZI_PARAFE2);
            string irtibat = NBYSOrtak.ParametreGetir(ProjeConstants.PARAM_FTKYAZI, ProjeConstants.PARAM_FTKYAZI_IRTIBAT);
            string imza1 = NBYSOrtak.ParametreGetir(ProjeConstants.PARAM_FTKYAZI, ProjeConstants.PARAM_FTKYAZI_IMZAADSOYAD);
            string imza2 = NBYSOrtak.ParametreGetir(ProjeConstants.PARAM_FTKYAZI, ProjeConstants.PARAM_FTKYAZI_IMZAUNVAN);
            string imza3 = NBYSOrtak.ParametreGetir(ProjeConstants.PARAM_FTKYAZI, ProjeConstants.PARAM_FTKYAZI_IMZAMAKAM);

            string parafeTarihi = ".../" + DateTime.Today.ToString("MM") + "/" + DateTime.Today.ToString("yyyy");

            Parafe1Txt.Text = parafeTarihi + (string.IsNullOrEmpty(parafe1) ? " BTHİ.Ş.Md. K.KARABABA" : " " + parafe1);
            Parafe2Txt.Text = parafeTarihi + (string.IsNullOrEmpty(parafe2) ? " Vakıf Hiz.Grp.Bşk. Z. YAĞCI" : " " + parafe2);

            IrtibatNoktasiTxt.Text = string.IsNullOrEmpty(irtibat) ? "Dorukhan GÜNDÜR (Dâhili Tel:261)" : irtibat;
            ImzalayanTxt.Text = string.IsNullOrEmpty(imza1) ? "SBilal TOPÇU" : imza1;
            ImzalayanUnvanTxt.Text = string.IsNullOrEmpty(imza2) ? string.Empty: imza2;
            ImzalayanMakamTxt.Text = string.IsNullOrEmpty(imza3) ? "TSKGV Genel Müdürü" : imza3;
        }

        private void IlDDLDoldur()
        {
            IliDDL.Items.Clear();
            Il newil = new Il();
            List<Il> list = newil.SelectFTKKuruluOlanIller();
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
            Ilce pilce = new Ilce();

            if (IliDDL.SelectedItem != null)
            {
                List<Ilce> list = pilce.SelectFTKKuruluOlanIlceler(IliDDL.SelectedValue.ConvertToInt());
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
        private MemoryStream GetTemplateStream(string templateFileName)
        {
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
        private bool YaziOlustur(string dosyaAdi, string templateFileName)
        {
            bool isYaziOlusturuldu;
            try
            {
                MemoryStream templateStream = GetTemplateStream(templateFileName);
                IEnumerable<Paragraph> templateParagraphs = GetTemplateParagraphs(templateStream);
                MemoryStream destinationStream = AddData2DestinationStream(templateStream, templateParagraphs);
                destinationStream.Seek(0, SeekOrigin.Begin);
                destinationStream.Position = 0;

                AddToSharePoint(destinationStream, dosyaAdi);
                string dosyaUrl = SPContext.Current.Web.Url + @"/" + ProjeConstants.NBYSBELGELERI_LIB + @"/" + dosyaAdi;


                string rootUrl = UtilityHelper.RootURLGetir();

                DosyaLnk.Text = dosyaAdi;
                DosyaLnk.NavigateUrl = rootUrl + "/_layouts/15/download.aspx?SourceUrl=" + dosyaUrl;
                DosyaLnk.Visible = true;
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
        private string kaymakamAdi = "BAŞKAN ADI BOŞ";
        private string ftkBaskaniAdi = "BAŞKAN ADI BOŞ";
        private MemoryStream AddData2DestinationStream(MemoryStream templateStream, IEnumerable<Paragraph> templateParagraphs)
        {

            string ilAdi = IliDDL.SelectedItem.Text;
            string ilceAdi = IlcesiDDL.SelectedItem.Text;
            string bolge = BolgeGetir(ilAdi);
            string ilAdiBuyukHarf = ilAdi.ToUpper();
            string ilceAdiBuyukHarf = ilceAdi.ToUpper();
            //create key value pair, key represents words to be replace and 
            //values represent values in document in place of keys.
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            UyeListesiniDoldur(templateStream);

            keyValues.Add("BelgeSayisiVar", EvrakSayisiTxt.Text);
            keyValues.Add("BelgeTarihiVar", EvrakTarihiTxt.Text);

            string ilgiVar = ilceAdi.Equals(ProjeConstants.VALILIK) ? ilAdi + " Valiliğinin " : ilceAdi + " Kaymakamlığı'nın " + IlgiTarihiTxt.Text + " tarih ve " + IlgiSayisiTxt.Text + " sayılı yazısı.";
            keyValues.Add("IlgiVar", ilgiVar);

            keyValues.Add("BaslikDosyaVar", "(DOSYA)");
            keyValues.Add("BaslikIlceVar", ilceAdiBuyukHarf + " KAYMAKAMLIK MAKAMINA ");
            keyValues.Add("BaslikIlVar", ilAdiBuyukHarf + " VALİLİK MAKAMINA ");
            keyValues.Add("BaslikBolgeVar", bolge.ToUpper() + " BÖLGE TEMSİLCİLİĞİNE ");

            keyValues.Add("IlAdiVar", ilAdi);
            keyValues.Add("IlBuyukHarfVar", ilAdiBuyukHarf);

            keyValues.Add("IlceAdiVar", ilceAdi);
            keyValues.Add("IlceBuyukHarfVar", ilceAdiBuyukHarf);

            keyValues.Add("IlceIIAdiBuyukHarfVar", ilceAdiBuyukHarf + "/" + ilAdiBuyukHarf);
            keyValues.Add("BolgeAdiVar", bolge);

            keyValues.Add("KaymakamAdiVar", kaymakamAdi);
            keyValues.Add("FTKBskVar", ftkBaskaniAdi);

            keyValues.Add("IrtibatVar", IrtibatNoktasiTxt.Text);
            keyValues.Add("ParafeVakHizVar", Parafe1Txt.Text);
            keyValues.Add("ParafeBTHIVar", Parafe2Txt.Text);

            keyValues.Add("ImzaVar", ImzalayanTxt.Text);
            keyValues.Add("UnvanVar", ImzalayanUnvanTxt.Text);
            keyValues.Add("MakamVar", ImzalayanMakamTxt.Text);

            MemoryStream destinationStream = CopyAndSearchAndReplace(templateStream, templateParagraphs, keyValues);
            return destinationStream;
        }

        private string BolgeGetir(string ilAdi)
        {
            Il il = new Il();
            il = il.SelectByIlAdi(ilAdi);
            return (il != null ? il.Bolge : string.Empty);
        }

        private void UyeListesiniDoldur(MemoryStream templateStream)
        {
            using (WordprocessingDocument wordDoc2 = WordprocessingDocument.Open(templateStream, true))
            {
                int ilId = IliDDL.SelectedItem.Value.ConvertToInt();
                int ilcesiId = IlcesiDDL.SelectedItem.Value.ConvertToInt();
                string bolge = BolgeGetir(IliDDL.SelectedItem.Text);
                var doc = wordDoc2.MainDocumentPart.Document;
                var body = doc.Body;
                var paras = body.Elements<Paragraph>();

                FTK ftk = new FTK();
                List<FTK> list = ftk.SelectSonFTKListesiByIliIlcesiReturnList(ilId, ilcesiId);
                int counter = 0;
                foreach (var item in list)
                {

                    switch (item.FTKGorevi)
                    {
                        case ProjeConstants.FTK_GOREVI_FAHRIBASKAN:
                            {
                                kaymakamAdi = item.Adi.Trim() + " " + item.Soyadi.Trim();
                                break;
                            }
                        case ProjeConstants.FTK_GOREVI_BASKAN:
                            {
                                ftkBaskaniAdi = item.Adi.Trim() + " " + item.Soyadi.Trim();
                                break;
                            }

                        default:
                            break;
                    }

                    counter++;
                    string ftkGorevi = item.FTKGorevi;
                    string adiSoyadi = (item.Adi.Trim() + " " + item.Soyadi.Trim());
                    string unvani = item.Unvani;
                    string kartno = item.KartNo;

                    string ftkGoreviVar = string.IsNullOrEmpty(ftkGorevi) ? "" : ftkGorevi;
                    string adiSoyadiVar = string.IsNullOrEmpty(adiSoyadi) ? "" : adiSoyadi;
                    string unvaniVar = string.IsNullOrEmpty(unvani) ? "" : unvani;
                    string kartnoVar = string.IsNullOrEmpty(kartno) ? "" : kartno;

                    Table table1 = doc.Body.Descendants<Table>().ElementAt(0);
                    Table table2 = doc.Body.Descendants<Table>().ElementAt(1);
                    Table table3 = doc.Body.Descendants<Table>().ElementAt(2);


                    TabloyaUyeEkle(table1, ftkGoreviVar, adiSoyadiVar, unvaniVar, kartnoVar);
                    TabloyaUyeEkle(table2, ftkGoreviVar, adiSoyadiVar, unvaniVar, kartnoVar);
                    TabloyaUyeEkle(table3, ftkGoreviVar, adiSoyadiVar, unvaniVar, kartnoVar);
                    if (!bolge.Equals(ProjeConstants.BOLGE_GENELMUDURLUK))
                    {
                        Table table4 = doc.Body.Descendants<Table>().ElementAt(3);
                        TabloyaUyeEkle(table4, ftkGoreviVar, adiSoyadiVar, unvaniVar, kartnoVar);
                    }


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
            var runProperties = new RunProperties();
            var fontname = "Arial";
            var fontSize = "22";
            try
            {
                fontname =
                    rowCopy.Descendants<TableCell>()
                       .ElementAt(cellIndex)
                       .GetFirstChild<Paragraph>()
                       .GetFirstChild<ParagraphProperties>()
                       .GetFirstChild<ParagraphMarkRunProperties>()
                       .GetFirstChild<RunFonts>()
                       .Ascii;
            }
            catch
            {
                //swallow
            }
            //try
            //{
            //    fontSize =
            //           rowCopy.Descendants<TableCell>()
            //              .ElementAt(cellIndex)
            //              .GetFirstChild<Paragraph>()
            //              .GetFirstChild<ParagraphProperties>()
            //              .GetFirstChild<ParagraphMarkRunProperties>()
            //              .GetFirstChild<FontSize>()
            //              .Val;
            //}
            //catch
            //{
            //    //swallow
            //}
            runProperties.AppendChild(new RunFonts() { Ascii = fontname, HighAnsi = fontname, ComplexScript = fontname });//türkçe karakterler düzgün çıksın diye
            runProperties.AppendChild(new FontSize() { Val = fontSize });
            runProperties.AppendChild(new Languages() { Val = "tr-TR" });

            return runProperties;
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
        protected void BolgeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            IlDDLDoldur();
        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            IliIdQS = IliDDL.SelectedItem.Value;
            IlceDDLDoldur();
        }
        protected void IlcesiDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            IlcesiIdQS = IlcesiDDL.SelectedItem.Value;
        }
        protected void KurulusYazisiOlusturBtn_Click(object sender, EventArgs e)
        {
            try
            {

                string ilAdi = IliDDL.SelectedItem.Text;
                string ilcesiAdi = IlcesiDDL.SelectedItem.Text;
                string bolge = BolgeGetir(ilAdi);
                // Dosya adları 
                string zaman = DateTime.Now.ToString("dd-MM-yyyy-HH-mm");
                string yaziDosyaAdi = ilAdi + "-" + ilcesiAdi + "-FTK-kurulus-yazisi(" + zaman + ").docx";
                string templateFileName = bolge.Equals(ProjeConstants.BOLGE_GENELMUDURLUK) ?
                    ProjeConstants.FTK_ILCE_KURULUMGMYAZI_TEMPLATE :
                    ProjeConstants.FTK_ILCE_KURLUMANAYAZI_TEMPLATE;
                bool isYaziOlusturuldu = YaziOlustur(yaziDosyaAdi, templateFileName);
                if (isYaziOlusturuldu)
                {
                    MessageHelper.PublishMessage("Kurulum Yazısı hazırlandı, Dosya ismine basarak yazıyı indirebilirsiniz.", ProjeConstants.MESAJ_BASARILI, 2000);
                }
                //else
                //    MessageHelper.PublishMessage("Hata Oluştu", ProjeConstants.MESAJ_HATA);
            }
            catch (Exception ex)
            {
                Exception ex1 = new Exception("Yazı ve Adres oluşturmada hata");
                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.Exceptions.Add(ex1);
                exh.PublishException();
            }
        }
        protected void GuncellemeYazisiOlusturBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string ilAdi = IliDDL.SelectedItem.Text;
                string ilcesiAdi = IlcesiDDL.SelectedItem.Text;
                string bolge = BolgeGetir(ilAdi);
                // Dosya adları 
                string zaman = DateTime.Now.ToString("dd-MM-yyyy-HH-mm");
                string yaziDosyaAdi = ilAdi + "-" + ilcesiAdi + "-FTK-guncelleme-yazisi(" + zaman + ").docx";
                string templateFileName = bolge.Equals(ProjeConstants.BOLGE_GENELMUDURLUK) ?
                    ProjeConstants.FTK_ILCE_GUNCELLEMEGMYAZI_TEMPLATE :
                    ProjeConstants.FTK_ILCE_GUNCELLEMEANAYAZI_TEMPLATE;

                bool isYaziOlusturuldu = YaziOlustur(yaziDosyaAdi, templateFileName);
                if (isYaziOlusturuldu)
                {
                    MessageHelper.PublishMessage("Güncelleme Yazısı hazırlandı, Dosya ismine basarak yazıyı indirebilirsiniz.", ProjeConstants.MESAJ_BASARILI, 2000);
                }
                //else
                //    MessageHelper.PublishMessage("Hata Oluştu", ProjeConstants.MESAJ_HATA);
            }
            catch (Exception ex)
            {
                Exception ex1 = new Exception("Yazı ve Adres oluşturmada hata");
                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.Exceptions.Add(ex1);
                exh.PublishException();
            }
        }
        protected void FTKIslemleriBtn_Click(object sender, EventArgs e)
        {
            IliIdQS = IliDDL.SelectedItem == null ? "1" : IliDDL.SelectedItem.Value;
            IlcesiIdQS = IlcesiDDL.SelectedItem == null ? ProjeConstants.VALILIK_INT.ToString() : IlcesiDDL.SelectedItem.Value;
            RedirectToPage(ProjeConstants.PAGE_FTKISLEMLERI + "?IliId=" + IliDDL.SelectedItem.Value + "&IlcesiId=" + IlcesiIdQS);
        }
        protected void FTKListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FTK_LIST + "?IliId=" + IliDDL.SelectedItem.Value + "&IlcesiId=" + IlcesiDDL.SelectedItem.Value);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
    }
}