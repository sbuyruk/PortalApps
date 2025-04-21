using DocumentFormat.OpenXml.Packaging;
using Microsoft.SharePoint;
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
using System.Web.UI;
using Model.TBYS;
using Model.Ortak;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

namespace TBYS_WebParts.VasiyetciAdresListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class VasiyetciAdresListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }

        private string LibraryNameQS
        {
            get
            {

                if (ViewState["LibraryName"] == null)
                {
                    if (Page.Request.QueryString["LibraryName"] != null)
                    {
                        ViewState["LibraryName"] = Page.Request.QueryString["LibraryName"];
                    }
                    else
                    {
                        ViewState["LibraryName"] = ProjeConstants.TBYSBELGELERI_LIB;
                    }
                }
                return ViewState["LibraryName"].ToString();
            }

            set
            {
                ViewState["LibraryName"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //time out olmasın diye
                ScriptManager _scriptMan = ScriptManager.GetCurrent(Page);
                _scriptMan.AsyncPostBackTimeout = 36000;
                if (!Page.IsPostBack)
                {
                    EtiketAdediDDLDoldur();
                }
                TabloOlustur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        private void EtiketAdediDDLDoldur()
        {
            ListItem li1 = new ListItem("1'er Adet", "1");
            ListItem li2 = new ListItem("3'er Adet", "3");
            ListItem li3 = new ListItem("1'er Sayfa", "21");
            EtiketAdediDDL.Items.Add(li1);
            EtiketAdediDDL.Items.Add(li2);
            EtiketAdediDDL.Items.Add(li3);
        }

        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
                    jQuery('#CustomModalDataTable').DataTable().destroy();
                }
                jQuery('#CustomModalDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
                jQuery(document).ready(function () {
                    jQuery('#CustomDataTable').DataTable({
                        'initComplete': function (settings, json) {//tablo yüklendiğinde
                            var api = this.api();
                            var row = api.row(function (idx, data, node) { //secilen kayda gider
                                return data['Secildi'] == true;
                            });
                            if (row.length > 0) {
                                row.select()
                                    .show()
                                    .draw(false);
                            }
                        },
                        data: " + jsonData + @",
                        columns: [
                            { data: 'Bolge' },
                            { data: 'Adi' },
                            { data: 'Soyadi' },
                            { data: 'IkametAdresi' },
                            { data: 'IkametIli' },
                            { data: 'IkametIlcesi' },
                            { data: 'Telefon1' },
                        ],
                        'columnDefs': [
                            { 'width': '20%', 'targets': 1 },
                            { 'width': '25%', 'targets': 2 },
                        ],
                        'language': {
                            'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        destroy: true,
                        dom: 'Bfrtip',
                        buttons: [
                            {
                                extend: 'print',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'excel',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'pdf',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'copy',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            , 'pageLength', 'colvis'
                        ],

                        });
                    });

            ";

            return tableString;
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
        private void TabloOlustur()
        {
            var jsonData = VasiyetciJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }

        private string VasiyetciJson()
        {
            string jSon = string.Empty;

            List<AdresListItem> list = GetDataList();
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            jSon = serializer.Serialize(list);
            return jSon;
        }


        private MemoryStream GetTemplateStream(string templateFileName)
        {
            string newFileUrl = string.Empty;

            string siteUrl = SPContext.Current.Web.Url;
            using (SPSite spSite = new SPSite(siteUrl))
            {
                SPList list = SPContext.Current.Web.Lists[LibraryNameQS];
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
        private MemoryStream AddAdresEtiketData2DestinationStream(MemoryStream destinationStream, IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Table> templateTables)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            Vasiyetci vasiyetci = new Vasiyetci();
            int rowCount = 0;
            DataTable dataTable = vasiyetci.SelectAllVasiyetciReturnDataTable(VefatEdenlerHaricChk.Checked, ref rowCount);
            if (dataTable != null)
            {
                int index = 1;
                List<string> uzunAdresliler = new List<string>();
                foreach (DataRow row in dataTable.Rows)
                {
                    string adiSoyadi = row["Adi"].ToString() + " " + row["Soyadi"].ToString();
                    string ikametAdresi = row == null ? "" : row["IkametAdresi"].ToString();
                    string ikametIli = row == null ? "" : row["IlAdi"].ToString();
                    string ikametIlcesi = row == null ? "" : row["IlceAdi"].ToString();
                    string semtIlceIl = ikametIlcesi + @"/" + ikametIli;
                    int etiket = EtiketAdediDDL.SelectedItem.Value.ConvertToInt();
                    for (int i = 0; i < etiket; i++)
                    {
                        //create key value pair, key represents words to be replace and 
                        //values represent values in document in place of keys.
                        Dictionary<string, string> keyValues = new Dictionary<string, string>();
                        keyValues.Add("AdSoyad" + index + "Var", adiSoyadi);
                        keyValues.Add("Adres" + index + "Var", string.IsNullOrEmpty(ikametAdresi) ? "Adresi yok" : ikametAdresi.Substring(0, ikametAdresi.Length > 110 ? 110 : ikametAdresi.Length - 1));
                        keyValues.Add("SemtIlceIl" + index + "Var", semtIlceIl);
                        SearchAndReplace(destinationStream, keyValues);
                        index++;
                    }

                    if (!string.IsNullOrEmpty(ikametAdresi) && ikametAdresi.Trim().Length > 110)
                    {
                        uzunAdresliler.Add(adiSoyadi);
                    }

                    if (index >= 21)//sayfa bitti yeni sayfa ekle
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
                        keyValues.Add("AdiSoyadi" + i + "Var", "");
                        keyValues.Add("IkametAdresi" + i + "Var", "");
                        keyValues.Add("SemtIlceIl" + i + "Var", "");
                        destinationStream = SearchAndReplace(destinationStream, keyValues);
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
                DocumentFormat.OpenXml.Wordprocessing.Paragraph PageBreakParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break() { Type = DocumentFormat.OpenXml.Wordprocessing.BreakValues.Page }));
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
        protected void AddToSharePoint(MemoryStream memStream, string fileName)
        {
            //string url = System.Web.HttpContext.Current.Request.Url.ToString();
            using (SPSite spSite = new SPSite(SPContext.Current.Web.Url))
            {
                //Get the document library object
                SPList docLib = SPContext.Current.Web.Lists[LibraryNameQS];
                SPFile file = docLib.RootFolder.Files.Add(fileName, memStream, true);
                file.Update();
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

                string dosyaUrl = SPContext.Current.Web.Url + @"/" + LibraryNameQS + @"/" + dosyaAdi;

                string sourceString = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                string removeString = System.Web.HttpContext.Current.Request.Url.AbsolutePath;

                int index = sourceString.IndexOf(removeString);
                string rootUrl = (index < 0)
                    ? sourceString
                    : sourceString.Substring(0, index);

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
        protected void AdresEtiketiBtn_Click(object sender, EventArgs e)
        {
            try
            {

                // Dosya adları 
                string zaman = DateTime.Now.ToString("dd-MM-yyyy-HH-mm");
                string etiketDosyaAdi = "Vasiyetci-Adres-Etiketi(" + zaman + ").docx";
                bool etiketOlustuMu = YeniAdresEtiketDosyasiOlustur(etiketDosyaAdi);
                if (etiketOlustuMu)
                    MessageHelper.PublishMessage("Vasiyetçi adres etiketleri hazırlandı, Dosya ismine basarak yazıyı indirebilirsiniz.", ProjeConstants.MESAJ_BASARILI, 2000);
                else
                {
                    MessageHelper.PublishMessage("Adres etiketleri oluşturulamadı.", ProjeConstants.MESAJ_BILGI, 3000);
                }

            }
            catch (Exception ex)
            {
                Exception ex1 = new Exception("Adres etiketleri oluşturmada hata");
                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.PublishException();
            }
        }
        protected void VefatEdenlerHaricChk_TextChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToExcel();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private List<AdresListItem> GetDataList()
        {
            DateTime today = DateTime.Today;
            int rowCount = 0;
            Vasiyetci vasiyetci = new Vasiyetci();
            DataTable dataTable = vasiyetci.SelectAllVasiyetciReturnDataTable(VefatEdenlerHaricChk.Checked, ref rowCount);
            int SiraNo = 0;
            List<AdresListItem> list = new List<AdresListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            DataView dataView = new DataView(dataTable);
            foreach (DataRowView row in dataView)
            {
                int vasiyetciId = row["VasiyetciId"].ConvertToInt();
                string bolge = row["Bolge"].ToString();
                string adi = row["Adi"].ToString();
                string soyadi = row["Soyadi"].ToString();
                string telefon1 = row["Telefon1"].ToString();
                string ikametAdresi = row["IkametAdresi"].ToString();
                string ikametIli = row["IlAdi"].ToString();
                string ikametIlcesi = row["IlceAdi"].ToString();

                AdresListItem adresItem = new AdresListItem();
                adresItem.Sirano = SiraNo++.ToString();
                adresItem.VasiyetciId = vasiyetciId.ToString();
                adresItem.Adi = adi;
                adresItem.Soyadi = soyadi;
                adresItem.Telefon1 = telefon1;
                adresItem.IkametAdresi = ikametAdresi;
                adresItem.IkametIlcesi = ikametIlcesi;
                adresItem.IkametIli = ikametIli;
                adresItem.Bolge = bolge;
                list.Add(adresItem);
            }
            return list;
        }
        protected void ExportToExcel()
        {
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;
            List<Vasiyetci> list = new List<Vasiyetci>();
            GridView1.DataSource = GetDataList();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=VasiyetciListesi.xls");
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            Page.Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                //Apply text style to each Row
                GridView1.Rows[i].Attributes.Add("class", "textmode");
            }
            GridView1.RenderControl(hw);

            //style to format numbers to string
            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Page.Response.Write(style);
            Page.Response.Output.Write(sw.ToString());
            Page.Response.Flush();
            Page.Response.End();

        }

        private class AdresListItem
        {
            public string Sirano { get; set; }
            public string VasiyetciId { get; set; }
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public string Telefon1 { get; set; }
            public string IkametIli { get; set; }
            public string IkametIlcesi { get; set; }
            public string IkametAdresi { get; set; }
            public string Bolge { get; set; }
        }
    }
}
