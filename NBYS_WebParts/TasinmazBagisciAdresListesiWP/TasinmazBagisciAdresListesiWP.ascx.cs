using System.Linq;
using System.Security;
using System.Web;
using NBYS_WebParts.Logging;
using Utility.Logging;
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
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "TasinmazBagisciAdresListesiWP"); }
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
        public TasinmazBagisciAdresListesiWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string LibraryNameQS { get { return ProjeConstants.NBYSBELGELERI_LIB; } }
        private DataTable labelRows;
        private int shortenedAddressCount;
        private bool labelFileSaved;

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
                    EtiketAdediDDLDoldur();
                    TabloOlustur();

                }
            });
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
            ExecuteAction("CloseBtn_Click", () =>
            {

            });
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
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
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
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

            List<AdresListItem> list = GetDataList();
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            jSon = serializer.Serialize(list);
            return jSon;
        }
        private void TasinmazBagisciFormunuDoldur(string tasinmazBagisciIdStr)
        {
            if (!string.IsNullOrEmpty(tasinmazBagisciIdStr))
            {
                int tasinmazBagisciId = tasinmazBagisciIdStr.ConvertToInt();

                TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
                tasinmazBagisci = new Model.Services.TBYS.TasinmazBagisciService(LoggingContext).GetById(tasinmazBagisciId);
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
            using (SPSite site = new SPSite(SPContext.Current.Web.Url))
            using (SPWeb web = site.OpenWeb())
            {
                SPFile file = web.GetFile(web.Url + "/" + LibraryNameQS + "/" + templateFileName);
                if (file == null || !file.Exists) throw new InvalidOperationException("Adres etiketi şablonu bulunamadı.");
                byte[] bytes = file.OpenBinary();
                MemoryStream stream = new MemoryStream();
                stream.Write(bytes, 0, bytes.Length);
                stream.Position = 0;
                return stream;
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
                    docText = docText.Replace(item.Key, SecurityElement.Escape(item.Value ?? string.Empty));
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
            using (WordprocessingDocument document = WordprocessingDocument.Open(templateStream, false))
            {
                return document.MainDocumentPart.Document.Body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>()
                    .Select(table => (DocumentFormat.OpenXml.Wordprocessing.Table)table.CloneNode(true)).ToList();
            }
        }
        private MemoryStream AddAdresEtiketData2DestinationStream(MemoryStream destinationStream, IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Table> templateTables)
        {
            int copies;
            if (!int.TryParse(EtiketAdediDDL.SelectedValue, out copies) || (copies != 1 && copies != 3 && copies != 21))
                throw new NbysReportInputException("Geçerli bir etiket adedi seçin.");
            int index = 1;
            shortenedAddressCount = 0;
            foreach (DataRow row in labelRows.Rows)
            {
                string name = Convert.ToString(row["AdiSoyadi"]);
                string address = Convert.ToString(row["Adres"]);
                string province = Convert.ToString(row["Ili"]);
                string district = Convert.ToString(row["Ilcesi"]);
                bool hidden = row["Gizli"].ReturnEmptyIfNull().ConvertToBool();
                if (!hidden && address.Length > 110) shortenedAddressCount++;
                for (int copy = 0; copy < copies; copy++)
                {
                    if (index > 21)
                    {
                        AddTable2DestinationStream(destinationStream, templateTables);
                        index = 1;
                    }
                    Dictionary<string, string> values = new Dictionary<string, string>();
                    values.Add("AdSoyad" + index + "Var", hidden ? ProjeConstants.GIZLI_STRING : name);
                    values.Add("Adres" + index + "Var", hidden ? ProjeConstants.GIZLI_STRING :
                        (string.IsNullOrEmpty(address) ? "Adresi yok" : address.Substring(0, Math.Min(110, address.Length))));
                    values.Add("SemtIlceIl" + index + "Var", hidden ? ProjeConstants.GIZLI_STRING : district + "/" + province);
                    SearchAndReplace(destinationStream, values);
                    index++;
                }
            }
            for (; index <= 21; index++)
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                values.Add("AdSoyad" + index + "Var", "");
                values.Add("Adres" + index + "Var", "");
                values.Add("SemtIlceIl" + index + "Var", "");
                SearchAndReplace(destinationStream, values);
            }
            return destinationStream;
        }
        private MemoryStream AddTable2DestinationStream(MemoryStream destinationStream, IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Table> templateTables)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(destinationStream, true))
            {
                //bos sayfa ekle
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
            memStream.Position = 0;
            SPList library = SPContext.Current.Web.Lists[LibraryNameQS];
            library.RootFolder.Files.Add(fileName, memStream, false);
            labelFileSaved = true;
        }
        private bool YeniAdresEtiketDosyasiOlustur(string dosyaAdi)
        {
            using (MemoryStream template = GetTemplateStream(ProjeConstants.NBYS_TASINMAZBAGISCI_ADRESETIKETI_TEMPLATE))
            {
                if (template.Length == 0) throw new InvalidOperationException("Adres etiketi şablonu bulunamadı.");
                var tables = GetTemplateTables(template).ToList();
                if (tables.Count == 0) throw new InvalidOperationException("Adres etiketi şablonunda tablo bulunamadı.");
                AddAdresEtiketData2DestinationStream(template, tables);
                AddToSharePoint(template, dosyaAdi);
            }
            string url = SPContext.Current.Web.Url + "/" + LibraryNameQS + "/" + dosyaAdi;
            AdresEtiketLnk.Text = dosyaAdi;
            AdresEtiketLnk.NavigateUrl = new Uri(SPContext.Current.Web.Url).GetLeftPart(UriPartial.Authority)
                + "/_layouts/15/download.aspx?SourceUrl=" + HttpUtility.UrlEncode(url);
            AdresEtiketLnk.Visible = true;
            return true;
        }
        protected void AdresEtiketiBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("AdresEtiketiBtn_Click", () =>
            {
                AdresEtiketLnk.Visible = false;
                labelFileSaved = false;
                labelRows = GetDataTable();
                if (labelRows == null || labelRows.Rows.Count == 0)
                    throw new NbysReportInputException("Seçilen filtrelere uygun bağışçı bulunamadı. Filtreleri değiştirip tekrar deneyin.");
                int copies;
                if (!int.TryParse(EtiketAdediDDL.SelectedValue, out copies) || (copies != 1 && copies != 3 && copies != 21))
                    throw new NbysReportInputException("Geçerli bir etiket adedi seçin.");
                try
                {
                    string name = "Bagisci-Adres-Etiketi-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N") + ".docx";
                    YeniAdresEtiketDosyasiOlustur(name);
                    string message = "Adres etiketleri hazırlandı. Dosya bağlantısından indirebilirsiniz.";
                    if (shortenedAddressCount > 0)
                        Messages.Warning(message + " " + shortenedAddressCount + " adres kısaltıldı; yazdırmadan önce etiketleri kontrol edin.");
                    else Messages.Success(message, 5000);
                }
                catch (System.Threading.ThreadAbortException) { throw; }
                catch (Exception error)
                {
                    System.Web.HttpContext.Current.Items["PortalApps.NBYS.FailedWebPart." + LoggingContext.WebPart] = true;
                    NbysWebPartLog.ReportError(LoggingContext, "NBYS.TasinmazBagisciAdresListesiWP.CreateLabels", error,
                        labelFileSaved ? "Dosya kitaplığa kaydedildi; ekran tamamlanamadı. Belge kitaplığını kontrol edin."
                        : "Adres etiketleri hazırlanamadı. Lütfen tekrar deneyin.");
                }
            });
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ExcelBtn_Click", () =>
            {
                ExportToExcel();
            });
        }
        private List<AdresListItem> GetDataList()
        {
            DataTable dataTable = GetDataTable();
            int SiraNo = 0;
            List<AdresListItem> list = new List<AdresListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            if (dataTable == null) return list;
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
            DataTable dataTable = new Model.Services.TBYS.TasinmazBagisciReportService(LoggingContext).GetAllCountBagisAdedi(VefatEdenlerHaricChk.Checked, GizliBagislarHaricChk.Checked);
            return dataTable;
        }
        protected void ExportToExcel()
        {
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;
            GridView1.DataSource = GetDataTable();
            GridView1.DataBind();
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                //Apply text style to each Row
                GridView1.Rows[i].Attributes.Add("class", "textmode");
            }
            GridView1.RenderControl(hw);

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=TasinmazBagisciAdresListesi.xls");
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            Page.Response.ContentType = "application/vnd.ms-excel";


            //style to format numbers to string
            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Page.Response.Write(style);
            Page.Response.Output.Write(sw.ToString());
            Page.Response.Flush();
            Page.Response.End();

        }
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ModalDoldurBtn_Click", () =>
            {
                int selectedId;
                if (!int.TryParse(paramTasinmazBagisciIdLbl.Value, out selectedId) || selectedId <= 0)
                    throw new NbysReportInputException("Bağışçıyı listeden yeniden seçin.");

                TabloModalOlustur(paramTasinmazBagisciIdLbl.Value.ConvertToInt());
                TasinmazBagisciFormunuDoldur(paramTasinmazBagisciIdLbl.Value);
            });
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
            ExecuteAction("VefatEdenlerHaricChk_CheckedChanged", () =>
            {
                TabloOlustur();
            });
        }
        protected void GizliBagislarHaricChk_CheckedChanged(object sender, EventArgs e)
        {
            ExecuteAction("GizliBagislarHaricChk_CheckedChanged", () =>
            {
                TabloOlustur();
            });
        }

        //modal
        private void TabloModalOlustur(int tasinmazBagisciId)
        {
            var jsonData = TabloModalJson(tasinmazBagisciId); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloModalJson(int tasinmazBagisciId)
        {
            string jSon = string.Empty;

            Bagis bagis = new Bagis();
            jSon = new Model.Services.TBYS.BagisService(LoggingContext).GetTasinmazByBagisciIdAsJson(tasinmazBagisciId);
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
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
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
