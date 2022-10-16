using DocumentFormat.OpenXml.Packaging;
using Microsoft.SharePoint;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.TasinmazBagisciAdresListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazBagisciAdresListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazBagisciAdresListesiWP()
        {
        }
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
                        ViewState["LibraryName"] = ProjeConstants.NBYSBELGELERI_LIB;
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
                    TabloOlustur();

                }
                
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
        protected void CloseBtn_Click(object sender, EventArgs e)
        {

        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
            if ($.fn.DataTable.isDataTable('#CustomDataTable')) {
                $('#CustomDataTable').DataTable().destroy();
            }

            $('#CustomDataTable tbody').empty();

            jQuery('#CustomDataTable').DataTable({
            data: " + jsonData + @",
                columns: [
                    { data: 'TasinmazBagisciId' },
                    { data: 'AdiSoyadi' },
                    { data: 'ToplamBagisAdedi' },
                    { data: 'ToplamTahminiRayic' },
                    { data: 'Adres' },
                    { data: 'Ili' },
                    { data: 'Ilcesi' },
                    { data: 'Telefon1' },
                ],
                'columnDefs': [
                    { 'orderData': [0], 'targets': [1] },
                    {
                    'targets': [0],
                    'visible': false
                    }
                ],
                'order': [[0, 'asc']],
                'language': {
                    'url': 'http://tskgv-portal/OrtakBelgeler/Turkish.txt',
                    'decimal': ',',
                    'thousands': '.'
                },
                responsive: true,
                dom: 'Bfrtip',
                buttons:
                [
                    {
                extend: 'print',
                        exportOptions:
                    {
                    columns: ':visible'
                        }
                },
                    {
                extend: 'excel',
                        exportOptions:
                    {
                    columns: ':visible'
                        }
                },
                    {
                extend: 'pdf',
                        exportOptions:
                    {
                    columns: ':visible'
                        }
                },
                    {
                extend: 'copy',
                        exportOptions:
                    {
                    columns: ':visible'
                        }
                },
                    , 'pageLength', 'colvis'
                ]
            });
            ";

            return tableString;
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<AdresListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                jSon = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        private void TasinmazBagisciFormunuDoldur(string tasinmazBagisciIdStr)
        {
            if (!string.IsNullOrEmpty(tasinmazBagisciIdStr))
            {
                int tasinmazBagisciId = tasinmazBagisciIdStr.ConvertToInt();

                TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
                tasinmazBagisci = tasinmazBagisci.Select<TasinmazBagisci>(tasinmazBagisciId);
                if (tasinmazBagisci != null)
                {
                    TableRow row = new TableRow();
                    TableCell AdiCell = new TableCell();
                    TableCell TCKimlikNoCell = new TableCell();
                    TableCell AdresCell = new TableCell();
                    TableCell IlIlceCell = new TableCell();
                    TableCell TelefonCell = new TableCell();

                    AdiCell.Text = tasinmazBagisci.Adi.ReturnEmptyIfNull().ToString() + " " + tasinmazBagisci.Soyadi.ReturnEmptyIfNull().ToString();
                    TCKimlikNoCell.Text = tasinmazBagisci.TCKimlikNo.ReturnEmptyIfNull().ToString();
                    AdresCell.Text = tasinmazBagisci.Adres.ReturnEmptyIfNull().ToString();

                    //int ilId = tasinmazBagisci.Ili.ConvertToInt();
                    //Il il = new Il();
                    //il = il.Select<Il>(ilId);
                    //if (il != null)
                    //{

                    //    IlIlceCell.Text = il.IlAdi.ReturnEmptyIfNull().ToString();
                    //}


                    //int ilceId = tasinmazBagisci.Ilcesi.ConvertToInt();
                    //Ilce ilce = new Ilce();
                    //ilce = ilce.Select<Ilce>(ilceId);
                    //if (ilce != null)
                    //{

                    //    IlIlceCell.Text += " " + ilce.IlceAdi.ReturnEmptyIfNull().ToString();
                    //}

                    IlIlceCell.Text = tasinmazBagisci.Ilcesi + " " + tasinmazBagisci.Ili;
                    TelefonCell.Text = tasinmazBagisci.Telefon1.ReturnEmptyIfNull().ToString();
                    row.Controls.Add(AdiCell);
                    row.Controls.Add(TCKimlikNoCell);
                    row.Controls.Add(AdresCell);
                    row.Controls.Add(IlIlceCell);
                    row.Controls.Add(TelefonCell);
                    BagisciTable.Controls.Add(row);

                }

            }

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
            DataTable dataTable = GetDataTable();
            if (dataTable != null)
            {
                int index = 1;
                List<string> uzunAdresliler = new List<string>();
                foreach (DataRow row in dataTable.Rows)
                {
                    string adiSoyadi = row["AdiSoyadi"].ToString();
                    string adres = row == null ? "" : row["Adres"].ToString();
                    string ili = row == null ? "" : row["Ili"].ToString();
                    string ilcesi = row == null ? "" : row["Ilcesi"].ToString();
                    string semtIlceIl = ilcesi + @"/" + ili;
                    int etiket = EtiketAdediDDL.SelectedItem.Value.ConvertToInt();
                    bool gizli = row["Gizli"].ReturnEmptyIfNull().ConvertToBool();
                    for (int i = 0; i < etiket; i++)
                    {
                        //create key value pair, key represents words to be replace and 
                        //values represent values in document in place of keys.
                        Dictionary<string, string> keyValues = new Dictionary<string, string>();
                        if (gizli)
                        {
                            keyValues.Add("AdSoyad" + index + "Var", ProjeConstants.GIZLI_STRING);
                            keyValues.Add("Adres" + index + "Var", ProjeConstants.GIZLI_STRING);
                            keyValues.Add("SemtIlceIl" + index + "Var", ProjeConstants.GIZLI_STRING);
                        }
                        else
                        {
                            keyValues.Add("AdSoyad" + index + "Var", adiSoyadi);
                            keyValues.Add("Adres" + index + "Var", string.IsNullOrEmpty(adres) ? "Adresi yok" : adres.Substring(0, adres.Length > 110 ? 110 : adres.Length - 1));
                            keyValues.Add("SemtIlceIl" + index + "Var", semtIlceIl);
                        }
                        SearchAndReplace(destinationStream, keyValues);
                        index++;
                    }

                    if (!string.IsNullOrEmpty(adres) && adres.Trim().Length > 110)
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
                        keyValues.Add("AdSoyad" + i + "Var", "");
                        keyValues.Add("Adres" + i + "Var", "");
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
            bool isYaziOlusturuldu;
            try
            {
                MemoryStream templateStream = GetTemplateStream(ProjeConstants.NBYS_TASINMAZBAGISCI_ADRESETIKETI_TEMPLATE);
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
                AdresEtiketLnk.Text = "Dosya Oluşturulamadı";
                AdresEtiketLnk.ForeColor = Color.Red;
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
                AdresEtiketLnk.Visible = true;
                AdresEtiketLnk.Text = "Dosya Oluşturuluyor...";
                // Dosya adları 
                string zaman = DateTime.Now.ToString("dd-MM-yyyy-HH-mm");
                string etiketDosyaAdi = "Bagisci-Adres-Etiketi(" + zaman + ").docx";
                bool etiketOlustuMu = YeniAdresEtiketDosyasiOlustur(etiketDosyaAdi);
                if (etiketOlustuMu)
                    MessageHelper.PublishMessage("Bağışçı adres etiketleri hazırlandı, Dosya ismine basarak yazıyı indirebilirsiniz.", ProjeConstants.MESAJ_BASARILI, 2000);
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
            DataTable dataTable = GetDataTable();
            int SiraNo = 0;
            List<AdresListItem> list = new List<AdresListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            DataView dataView = new DataView(dataTable);
            foreach (DataRowView row in dataView)
            {
                
                int tasinmazBagisciId = row["TasinmazBagisciId"].ConvertToInt();
                int toplamBagisAdedi = row["ToplamBagisAdedi"].ConvertToInt();
                decimal toplamTahminiRayic = row["ToplamTahminiRayic"].ConvertToDecimal();
                string adiSoyadi = row["AdiSoyadi"].ToString();
                string telefon1 = row["Telefon1"].ToString();
                string telefon2 = row["Telefon2"].ToString();

                string adres = row["Adres"].ToString();
                string ili = row["Ili"].ToString();
                string ilcesi = row["Ilcesi"].ToString();
                
                AdresListItem adresItem = new AdresListItem();
                adresItem.Sirano = SiraNo++.ToString();
                adresItem.TasinmazBagisciId = tasinmazBagisciId.ToString();
                adresItem.ToplamBagisAdedi = toplamBagisAdedi.ToString();
                adresItem.ToplamTahminiRayic = toplamTahminiRayic.ToString("N", culturInfo);
                adresItem.AdiSoyadi = "<a href=# onclick=OpenModal(" + tasinmazBagisciId + "); class=\'btn-link  text-primary\'>" + adiSoyadi + "</a>";
                adresItem.Telefon1 = telefon1;
                adresItem.Telefon2 = telefon2;
                adresItem.Adres = "- " + adres;
                adresItem.Ilcesi = ilcesi;
                adresItem.Ili = ili;
                bool gizli = row["Gizli"].ReturnEmptyIfNull().ConvertToBool();
                if (gizli)
                {
                    adresItem.AdiSoyadi = adiSoyadi;
                    adresItem.Telefon1 = adresItem.ToplamBagisAdedi = adresItem.ToplamTahminiRayic = 
                        adresItem.Telefon1 = adresItem.Telefon2 = adresItem.Adres = adresItem.Ili = adresItem.Ilcesi = ProjeConstants.GIZLI_STRING;
                }
                list.Add(adresItem);
            }
            return list;
        }
        private DataTable GetDataTable()
        {
            TasinmazBagisci bagisci = new TasinmazBagisci();
            DataTable dataTable = bagisci.SelectAllCountBagisAdediReturnDataTable(VefatEdenlerHaricChk.Checked, GizliBagislarHaricChk.Checked);
            return dataTable;
        }
        protected void ExportToExcel()
        {
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;
            GridView1.DataSource = GetDataTable();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=TasinmazBagisciAdresListesi.xls");
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
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            try
            {
                TabloModalOlustur(paramTasinmazBagisciIdLbl.Value.ConvertToInt());

                TasinmazBagisciFormunuDoldur(paramTasinmazBagisciIdLbl.Value);
                //tabloda modal açılırken seçili olan pagination degerini pageIndex degiskeninde saklar ve modal açıldıktan sonra pageload sırasında sayfayı pageIndex degerine getirir
                UtilityHelper.ScriptCalistir("SetPageIndex();");
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }
        }
        private class AdresListItem
        {
            public string Sirano { get; set; }
            public string TasinmazBagisciId { get; set; }
            public string ToplamBagisAdedi { get; set; }
            public string ToplamTahminiRayic { get; set; }
            public string AdiSoyadi { get; set; }
            public string Telefon1 { get; set; }
            public string Telefon2 { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Adres { get; set; }
        }

        protected void VefatEdenlerHaricChk_CheckedChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        } 
        protected void GizliBagislarHaricChk_CheckedChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        
        //modal
        private void TabloModalOlustur(int tasinmazBagisciId)
        {
            var jsonData = TabloModalJson(tasinmazBagisciId); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloModalJson(int tasinmazBagisciId)
        {
            string jSon = string.Empty;

            try
            {
                Bagis bagis = new Bagis();
                jSon = bagis.SelectTasinmazByBagisciIdReturnJson(tasinmazBagisciId);

            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        private string CreateModalDataTable(string jsonData)
        {
            string tableString = @"
            if ($.fn.DataTable.isDataTable('#CustomModalDataTable')) {
                $('#CustomModalDataTable').DataTable().destroy();
            }

            $('#CustomModalDataTable tbody').empty();

            jQuery('#CustomModalDataTable').DataTable({
            data: " + jsonData + @",
                columns: [
                    { data: 'BagisYili' },
                    { data: 'Cinsi' },
                    { data: 'TahminiRayicDegeri' },
                    { data: 'MulkiyetSekli' },
                    { data: 'Adres' },
                    { data: 'IlIlce' },
                ],
                'order': [[0, 'asc']],
                'language': {
                    'url': 'http://tskgv-portal/OrtakBelgeler/Turkish.txt',
                    'decimal': ',',
                    'thousands': '.'
                },
                responsive: true,
                dom: 'rtip',                
            });
            ";

            return tableString;
        }
        

    }
}