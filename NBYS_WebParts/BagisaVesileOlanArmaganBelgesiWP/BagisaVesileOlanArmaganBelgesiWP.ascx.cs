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
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;


namespace NBYS_WebParts.BagisaVesileOlanArmaganBelgesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BagisaVesileOlanArmaganBelgesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BagisaVesileOlanArmaganBelgesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

        protected void Page_Load(object sender, EventArgs e)
        {
            //time out olmasın diye
            ScriptManager _scriptMan = ScriptManager.GetCurrent(Page);
            _scriptMan.AsyncPostBackTimeout = 36000;
            if (!Page.IsPostBack)
            {
                FormuDoldur();
            }
            TabloOlustur();
        }
        private void FormuDoldur()
        {
            DateTime bugun = DateTime.Today;

            ImzalayanTxt.Text = @"Bilal TOPÇU";
            ImzalayanUnvanTxt.Text = string.Empty;
            ImzalayanMakamTxt.Text = @"Genel Müdür";
            BelgeTarihiTxt.Text = bugun.ToString("dd") + " " + bugun.ToString("MMMM") + " " + bugun.Year;
        }



        private void TabloOlustur()
        {
            var jsonData = GetData(); //veri çekilip json'a çevriliyor

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
                //DosyaOlusturBtn.Visible = false;
            }
        }
        private string GetData()
        {
            BagisaVesileOlanTesekkur bagisaVesileOlanTesekkur = new BagisaVesileOlanTesekkur();
            string json = bagisaVesileOlanTesekkur.SelectReturnJson();
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
                        { data: 'Adi'},
                        { data: 'Soyadi'},
                        { data: 'VerilmeSebebi'},
                        { data: 'BelgeTarihiDDMMYYYY' },
                        { data: 'ImzalayanAdiSoyadi'},
                        { data: 'Aciklama'},

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
                BagisaVesileOlanTesekkur bagisaVesileOlanTesekkur = BagisaVesileOlanTesekkurKaydet();

                // Dosya adları 
                string zaman = DateTime.Now.ToString("dd-MM-yyyy-HH-mm");
                string yaziDosyaAdi = "BagisaVesileOlanTesekkurBelgesi(" + zaman + ").docx";
                bool isYaziOlusturuldu = TesekkurBelgesiDosyasiOlustur(yaziDosyaAdi, bagisaVesileOlanTesekkur);
                if (isYaziOlusturuldu)
                {
                    MessageHelper.PublishMessage("Teşekkür belgesi hazırlandı, Dosya ismine basarak yazıyı indirebilirsiniz.", ProjeConstants.MESAJ_BASARILI, 2000);
                }

            }
            catch (Exception ex)
            {
                Exception ex1 = new Exception("Yazı oluşturmada hata");
                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.PublishException();
            }
        }

        private BagisaVesileOlanTesekkur BagisaVesileOlanTesekkurKaydet()
        {
            BagisaVesileOlanTesekkur bagisaVesileOlanTesekkur = new BagisaVesileOlanTesekkur() {
                Adi=AdiTxt.Text,
                Soyadi=SoyadiTxt.Text,
                Unvan=UnvanTxt.Text,
                BelgeTarihi=BelgeTarihiTxt.Text.ConvertToDatetime(),
                Aciklama=AciklamaTxt.Text,
                BelgeMetni1=BelgeMetni1Txt.Text,
                BelgeMetni2=BelgeMetni2Txt.Text,
                ImzalayanAdiSoyadi=ImzalayanTxt.Text,
                ImzalayanMakam=ImzalayanMakamTxt.Text,
                ImzalayanUnvan=ImzalayanUnvanTxt.Text,
                Olusturan=UtilityHelper.GetCurrentUserName(),
                TCKimlikNo=TCKimlikNoTxt.Text.ConvertToLong(),
                VerilmeSebebi=VerilmeSebebiTxt.Text,
            };
            int id=bagisaVesileOlanTesekkur.Save();
            bagisaVesileOlanTesekkur.Id= id;
            return bagisaVesileOlanTesekkur;
        }

        private bool TesekkurBelgesiDosyasiOlustur(string dosyaAdi, BagisaVesileOlanTesekkur bagisaVesileOlanTesekkur)
        {
            bool isYaziOlusturuldu = false;
            try
            {
                MemoryStream templateStream = GetTemplateStream();
                IEnumerable<Paragraph> templateParagraphs = GetTemplateParagraphs(templateStream);
                MemoryStream destinationStream = AddData2DestinationStream(templateStream, templateParagraphs, bagisaVesileOlanTesekkur);
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


            }
            catch (Exception ex)
            {
                isYaziOlusturuldu = false;
                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.PublishException();
            }
            return isYaziOlusturuldu;
        }
        private MemoryStream AddData2DestinationStream(MemoryStream templateStream, IEnumerable<Paragraph> templateParagraphs, BagisaVesileOlanTesekkur bagisaVesileOlanTesekkur)
        {
            MemoryStream destinationStream = null;

            //create key value pair, key represents words to be replace and 
            //values represent values in document in place of keys.
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            keyValues.Add("imzalayanAdiSoyadiVar", bagisaVesileOlanTesekkur.ImzalayanAdiSoyadi.ReturnEmptyIfNull().ToString().Trim());
            keyValues.Add("imzalayanUnvanVar", bagisaVesileOlanTesekkur.ImzalayanUnvan.ReturnEmptyIfNull().ToString().Trim());
            keyValues.Add("imzalayanMakamVar", bagisaVesileOlanTesekkur.ImzalayanMakam.ReturnEmptyIfNull().ToString().Trim());

            keyValues.Add("belgeTarihiVar", bagisaVesileOlanTesekkur.BelgeTarihi.ToString("dd MMMM yyyy"));
            keyValues.Add("belgeNoVar", bagisaVesileOlanTesekkur.Id.ReturnZeroIfNull().ToString().Trim());
            keyValues.Add("soyadiVar", bagisaVesileOlanTesekkur.Soyadi.ReturnEmptyIfNull().ToString().Trim());
            keyValues.Add("adiVar", bagisaVesileOlanTesekkur.Adi.ReturnEmptyIfNull().ToString().Trim());
            keyValues.Add("unvanVar", bagisaVesileOlanTesekkur.Unvan.ReturnEmptyIfNull().ToString().Trim());
            keyValues.Add("metin1Var", bagisaVesileOlanTesekkur.BelgeMetni1.ReturnEmptyIfNull().ToString().Trim());
            keyValues.Add("metin2Var", bagisaVesileOlanTesekkur.BelgeMetni2.ReturnEmptyIfNull().ToString().Trim());

            destinationStream = CopyAndSearchAndReplace(templateStream, templateParagraphs, keyValues);

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
                //Paragraph PageBreakParagraph = new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break() { Type = BreakValues.Page }));
                //wordDoc.MainDocumentPart.Document.Body.Append(PageBreakParagraph);

                //template yazıyı ekle
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
                            <Value Type='File'>TemplateBagisaVesile.docx</Value>
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
