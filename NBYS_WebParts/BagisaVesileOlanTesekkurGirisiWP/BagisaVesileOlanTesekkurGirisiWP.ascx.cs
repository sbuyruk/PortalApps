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
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.BagisaVesileOlanTesekkurGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BagisaVesileOlanTesekkurGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BagisaVesileOlanTesekkurGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        private string SecilenIdQS
        {
            get
            {

                if (ViewState["SecilenId"] == null)
                {
                    if (Page.Request.QueryString["SecilenId"] != null)
                    {
                        ViewState["SecilenId"] = Page.Request.QueryString["SecilenId"];
                    }
                    else
                    {
                        ViewState["SecilenId"] = string.Empty;
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int secilenId = SecilenIdQS.ConvertToInt();
            KaydetBtn.Visible = secilenId < 1;
            GuncelleBtn.Visible = secilenId > 0;
            DosyaOlusturBtn.Visible = secilenId > 0;
            if (!Page.IsPostBack)
            {
                FormuDoldur(secilenId);
            }
        }
        private void FormuDoldur(int secilenId=0)
        {
            if (secilenId < 1)
            {
                DateTime bugun = DateTime.Today;
                ImzalayanTxt.Text = @"Bilal TOPÇU";
                ImzalayanUnvanTxt.Text = string.Empty;
                ImzalayanMakamTxt.Text = @"Genel Müdür";
                BelgeTarihiTxt.Text = bugun.ToString("dd") + " " + bugun.ToString("MMMM") + " " + bugun.Year;
            }else
            {
                BagisaVesileOlanTesekkur bagisaVesileOlanTesekkur = new BagisaVesileOlanTesekkur() { Id = secilenId };
                bagisaVesileOlanTesekkur = bagisaVesileOlanTesekkur.Select<BagisaVesileOlanTesekkur>(secilenId);
                if (bagisaVesileOlanTesekkur != null && bagisaVesileOlanTesekkur.Id > 0)
                {
                    BelgeNoTxt.Text =bagisaVesileOlanTesekkur.Id.ToString();
                    AdiTxt.Text = bagisaVesileOlanTesekkur.Adi;
                    SoyadiTxt.Text = bagisaVesileOlanTesekkur.Soyadi;
                    UnvanTxt.Text = bagisaVesileOlanTesekkur.Unvan;
                    BelgeTarihiTxt.Text = bagisaVesileOlanTesekkur.BelgeTarihi.ToString("dd") + " " + bagisaVesileOlanTesekkur.BelgeTarihi.ToString("MMMM") + " " + bagisaVesileOlanTesekkur.BelgeTarihi.Year;
                    AciklamaTxt.Text = bagisaVesileOlanTesekkur.Aciklama;
                    BelgeMetni1Txt.Text = bagisaVesileOlanTesekkur.BelgeMetni1;
                    BelgeMetni2Txt.Text = bagisaVesileOlanTesekkur.BelgeMetni2;
                    ImzalayanTxt.Text = bagisaVesileOlanTesekkur.ImzalayanAdiSoyadi;
                    ImzalayanMakamTxt.Text = bagisaVesileOlanTesekkur.ImzalayanMakam;
                    ImzalayanUnvanTxt.Text = bagisaVesileOlanTesekkur.ImzalayanUnvan;
                    TCKimlikNoTxt.Text = bagisaVesileOlanTesekkur.TCKimlikNo.ToString();
                    VerilmeSebebiTxt.Text = bagisaVesileOlanTesekkur.VerilmeSebebi;
                }
            }
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                BagisaVesileOlanTesekkur bagisaVesileOlanTesekkur = BagisaVesileOlanTesekkurKaydet();
                string queryStr = "?SecilenId=" + bagisaVesileOlanTesekkur.Id;
                RedirectToPage(ProjeConstants.PAGE_BAGISAVESILE_GIRIS + queryStr);
            }
            catch (Exception ex)
            {
                Exception ex1 = new Exception("Kaydetme başarısız oldu");
                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.PublishException();
            }
        }
        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                BagisaVesileOlanTesekkur bagisaVesileOlanTesekkur = BagisaVesileOlanTesekkurGuncelle(SecilenIdQS.ConvertToInt());
            }
            catch (Exception ex)
            {
                Exception ex1 = new Exception("Kaydetme başarısız oldu");
                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.PublishException();
            }
        }
        protected void DosyaOlusturBtn_Click(object sender, EventArgs e)
        {
            try
            {
                BagisaVesileOlanTesekkur bagisaVesileOlanTesekkur = new BagisaVesileOlanTesekkur() { Id = SecilenIdQS.ConvertToInt() };
                bagisaVesileOlanTesekkur = bagisaVesileOlanTesekkur.Select<BagisaVesileOlanTesekkur>(SecilenIdQS.ConvertToInt());
                if (bagisaVesileOlanTesekkur == null || bagisaVesileOlanTesekkur.Id < 1)
                {
                    throw new Exception("Teşekkür belgesi bulunamadı");
                }

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
        protected void ListeyeGitBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string queryStr = "?SecilenId=" + SecilenIdQS;
                RedirectToPage(ProjeConstants.PAGE_BAGISAVESILE_LIST + queryStr);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private BagisaVesileOlanTesekkur BagisaVesileOlanTesekkurKaydet()
        {
            BagisaVesileOlanTesekkur bagisaVesileOlanTesekkur = new BagisaVesileOlanTesekkur()
            {
                Adi = AdiTxt.Text,
                Soyadi = SoyadiTxt.Text,
                Unvan = UnvanTxt.Text,
                BelgeTarihi = BelgeTarihiTxt.Text.ConvertToDatetime(),
                Aciklama = AciklamaTxt.Text,
                BelgeMetni1 = BelgeMetni1Txt.Text,
                BelgeMetni2 = BelgeMetni2Txt.Text,
                ImzalayanAdiSoyadi = ImzalayanTxt.Text,
                ImzalayanMakam = ImzalayanMakamTxt.Text,
                ImzalayanUnvan = ImzalayanUnvanTxt.Text,
                Olusturan = UtilityHelper.GetCurrentUserName(),
                TCKimlikNo = TCKimlikNoTxt.Text.ConvertToLong(),
                VerilmeSebebi = VerilmeSebebiTxt.Text,
            };
            int id = bagisaVesileOlanTesekkur.Save();
            bagisaVesileOlanTesekkur.Id = id;
            return bagisaVesileOlanTesekkur;
        }
        private BagisaVesileOlanTesekkur BagisaVesileOlanTesekkurGuncelle(int secilenId)
        {
            BagisaVesileOlanTesekkur bagisaVesileOlanTesekkur = new BagisaVesileOlanTesekkur() { Id = secilenId };
            bagisaVesileOlanTesekkur = bagisaVesileOlanTesekkur.Select<BagisaVesileOlanTesekkur>(secilenId);
            if (bagisaVesileOlanTesekkur == null || bagisaVesileOlanTesekkur.Id < 1)
            {
                throw new Exception("Güncellenecek teşekkür belgesi bulunamadı");
            } else
            {
                bagisaVesileOlanTesekkur.Adi = AdiTxt.Text;
                bagisaVesileOlanTesekkur.Soyadi = SoyadiTxt.Text;
                bagisaVesileOlanTesekkur.Unvan = UnvanTxt.Text;
                bagisaVesileOlanTesekkur.BelgeTarihi = BelgeTarihiTxt.Text.ConvertToDatetime();
                bagisaVesileOlanTesekkur.Aciklama = AciklamaTxt.Text;
                bagisaVesileOlanTesekkur.BelgeMetni1 = BelgeMetni1Txt.Text;
                bagisaVesileOlanTesekkur.BelgeMetni2 = BelgeMetni2Txt.Text;
                bagisaVesileOlanTesekkur.ImzalayanAdiSoyadi = ImzalayanTxt.Text;
                bagisaVesileOlanTesekkur.ImzalayanMakam = ImzalayanMakamTxt.Text;
                bagisaVesileOlanTesekkur.ImzalayanUnvan = ImzalayanUnvanTxt.Text;
                bagisaVesileOlanTesekkur.Olusturan = UtilityHelper.GetCurrentUserName();
                bagisaVesileOlanTesekkur.TCKimlikNo = TCKimlikNoTxt.Text.ConvertToLong();
                bagisaVesileOlanTesekkur.VerilmeSebebi = VerilmeSebebiTxt.Text;
            };
            bool updated=bagisaVesileOlanTesekkur.Update();
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

                string rootUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

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
                string siteUrl = SPContext.Current.Web.Url;
                using (SPSite spSite = new SPSite(siteUrl))
                using (SPWeb web = spSite.OpenWeb())
                {
                    string fileUrl = web.Url + "/" + ProjeConstants.NBYSBELGELERI_LIB + "/TemplateBagisaVesile.docx";
                    SPFile file = web.GetFile(fileUrl);
                    if (file != null && file.Exists)
                    {
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
    }
}
