using Microsoft.SharePoint;
using Model.Ortak;
using NBYS_WebParts.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.SPDosyaListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class SPDosyaListesiWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "SPDosyaListesiWP"); }
        }

        private void ExecuteAction(string operation, Action action)
        {
            NbysReportFeedback.Execute(LoggingContext, operation, action);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            ChromeType = PartChromeType.None;
        }

        private string LibraryNameQS { get { return ProjeConstants.NBYSBELGELERI_LIB; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
                if (!Page.IsPostBack)
                {
                    TitleLbl.Text = "Teşekkür Belgesi Dosyaları";
                    DosyayiSilNowBtn.Visible = false;
                    TabloOlustur();
                }
                else
                {
                    // Keep the listed snapshot on confirmation postbacks; do not authorize a newly changed file.
                    string json = ViewState["ListedFilesJson"] as string ?? "[]";
                    UtilityHelper.ScriptCalistir(CreateDataTable(json));
                }
            });
        }

        private static bool ValidName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.IndexOfAny(new[] { '/', '\\', '|', ':', '<', '>' }) < 0
                && name.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)
                && name.IndexOf("Template", StringComparison.OrdinalIgnoreCase) < 0
                && name.IndexOf("Şablon", StringComparison.OrdinalIgnoreCase) < 0;
        }

        private SPFile FindFile(string name)
        {
            if (!ValidName(name)) throw new NbysReportInputException("Geçerli bir belge dosyası seçin.");
            SPWeb web = SPContext.Current.Web;
            SPList library = web.Lists[LibraryNameQS];
            SPFile file = web.GetFile(library.RootFolder.ServerRelativeUrl + "/" + name);
            return file != null && file.Exists ? file : null;
        }

        private static string Snapshot(SPFile file)
        {
            return file == null ? "" : file.UniqueId.ToString("N") + "-"
                + file.TimeLastModified.Ticks.ToString(CultureInfo.InvariantCulture) + "-" + file.Length.ToString(CultureInfo.InvariantCulture);
        }

        private static bool CanDelete(SPFile file)
        {
            return file != null && file.Item.DoesUserHavePermissions(SPBasePermissions.DeleteListItems);
        }

        private string DownloadLink(SPFile file)
        {
            if (file == null) return "";
            string source = SPContext.Current.Web.Url + "/" + LibraryNameQS + "/" + file.Name;
            string url = SPContext.Current.Web.Url + "/_layouts/15/download.aspx?SourceUrl=" + HttpUtility.UrlEncode(source);
            return "<a href=\"" + HttpUtility.HtmlAttributeEncode(url) + "\" class=\"btn-link fw-bold\">"
                + HttpUtility.HtmlEncode(file.Name) + "</a>";
        }

        public string ToJSON(List<SPFile> fileList, string prefix)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            List<string> pairs = new List<string>();
            foreach (SPFile file in fileList)
            {
                if (!ValidName(file.Name)) continue;
                int position = file.Name.IndexOf(prefix, StringComparison.Ordinal);
                if (position < 0) continue;
                // Only the existing exact filename convention proves a legacy label/document pair.
                string labelName = ProjeConstants.ADRESETIKETI_DOSYA + "TES" + file.Name.Remove(position, prefix.Length);
                SPFile label = FindFile(labelName);
                string selectedLabel = label == null ? "" : label.Name;
                string action = "";
                if (CanDelete(file) && (label == null || CanDelete(label)))
                {
                    string script = "CallButtonClick('" + HttpUtility.JavaScriptStringEncode(file.Name) + "','"
                        + HttpUtility.JavaScriptStringEncode(selectedLabel) + "'); return false;";
                    action = "<a href=\"#\" onclick=\"" + HttpUtility.HtmlAttributeEncode(script)
                        + "\" class=\"btn btn-outline-danger\">Dosyayı Kaldır</a>";
                    pairs.Add(file.Name + "|" + selectedLabel + "|" + Snapshot(file) + "|" + Snapshot(label));
                }
                rows.Add(new Dictionary<string, object>
                {
                    { "FileName", DownloadLink(file) }, { "LabelFileName", DownloadLink(label) },
                    { "Author", HttpUtility.HtmlEncode(file.Author == null ? "" : file.Author.Name) },
                    { "TimeLastModified", TimeZone.CurrentTimeZone.ToLocalTime(file.TimeLastModified).ConvertToDDMMYYYHHmmFormat() },
                    { "DeleteFile", action }
                });
            }
            ViewState["ListedFilePairs"] = pairs.ToArray();
            JavaScriptSerializer serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            string json = serializer.Serialize(rows);
            ViewState["ListedFilesJson"] = json;
            return json;
        }

        private void TabloOlustur()
        {
            SPList library = SPContext.Current.Web.Lists[LibraryNameQS];
            List<SPFile> files = library.RootFolder.Files.Cast<SPFile>()
                .Where(file => ValidName(file.Name) && file.Name.Contains(ProjeConstants.TESEKKUR_DOSYA))
                .OrderByDescending(file => file.TimeLastModified).ToList();
            UtilityHelper.ScriptCalistir(CreateDataTable(ToJSON(files, ProjeConstants.TESEKKUR_DOSYA)));
        }

        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                jQuery(document).ready(function () {
                    jQuery('#CustomDataTable').DataTable({
                        data: " + jsonData + @",
                        columns: [
                            { data: 'FileName' },
                            { data: 'LabelFileName'},
                            { data: 'Author'},
                            { data: 'TimeLastModified'},
                            { data: 'DeleteFile'},
                        ],
                        columnDefs: [

                        ],
                        'order': [[3, 'desc']],//sort
                        'language': {
                            'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        destroy: true,
                        pageLength:10,
                        dom: 'ftipr',

                        });
                    });

            ";

            return tableString;
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () =>
            {
                string url = Page.Request.Url.GetLeftPart(UriPartial.Path);
                Page.Response.Redirect(url.Substring(0, url.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            });
        }

        protected void DosyayiSilBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("DosyayiSilBtn_Click", () =>
            {
                ViewState.Remove("ConfirmedFilePair");
                DosyayiSilNowBtn.Visible = false;
                DosyaAdiLbl.Text = EtiketDosyaAdiLbl.Text = "";
                string[] listed = ViewState["ListedFilePairs"] as string[] ?? new string[0];
                string key = paramDosyaAdiLbl.Value + "|" + paramEtiketDosyaAdiLbl.Value + "|";
                string pair = listed.FirstOrDefault(item => item.StartsWith(key, StringComparison.Ordinal));
                if (pair == null) throw new NbysReportInputException("Dosyayı listeden yeniden seçin.");
                ValidatePair(pair);
                ViewState["ConfirmedFilePair"] = pair;
                string[] parts = pair.Split('|');
                DosyaAdiLbl.Text = HttpUtility.HtmlEncode(parts[0]);
                EtiketDosyaAdiLbl.Text = HttpUtility.HtmlEncode(parts[1]);
                DosyayiSilNowBtn.Visible = true;
                System.Web.UI.ScriptManager.RegisterStartupScript(Page, GetType(), Guid.NewGuid().ToString(), "OpenModalOnay();", true);
            });
        }

        private List<SPFile> ValidatePair(string pair)
        {
            string[] parts = pair.Split('|');
            if (parts.Length != 4 || parts[0] != paramDosyaAdiLbl.Value || parts[1] != paramEtiketDosyaAdiLbl.Value)
                throw new NbysReportInputException("Onaylanan dosya seçimi değişti. Dosyayı listeden yeniden seçin.");
            SPFile main = FindFile(parts[0]);
            SPFile label = parts[1].Length == 0 ? null : FindFile(parts[1]);
            if (main == null || Snapshot(main) != parts[2] || Snapshot(label) != parts[3])
                throw new NbysReportInputException("Dosya kaldırılmış veya değiştirilmiş. Listeyi yenileyip tekrar seçin.");
            if (!CanDelete(main) || (label != null && !CanDelete(label)))
                throw new NbysReportInputException("Seçilen dosyaları kaldırma yetkiniz bulunmuyor.");
            List<SPFile> files = new List<SPFile> { main };
            if (label != null) files.Add(label);
            return files;
        }

        protected void DosyayiSilNowBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("DosyayiSilNowBtn_Click", () =>
            {
                string pair = ViewState["ConfirmedFilePair"] as string;
                if (string.IsNullOrEmpty(pair)) throw new NbysReportInputException("Dosyayı listeden yeniden seçip kaldırma işlemini onaylayın.");
                List<SPFile> files = ValidatePair(pair);
                int removed = 0;
                try
                {
                    foreach (SPFile file in files)
                    {
                        file.Recycle();
                        removed++;
                    }
                    ViewState.Remove("ConfirmedFilePair");
                    DosyayiSilNowBtn.Visible = false;
                    TabloOlustur();
                    Messages.Success(removed + " dosya geri dönüşüm kutusuna taşındı.", 4000);
                }
                catch (System.Threading.ThreadAbortException) { throw; }
                catch (Exception error)
                {
                    ViewState.Remove("ConfirmedFilePair");
                    DosyayiSilNowBtn.Visible = false;
                    HttpContext.Current.Items["PortalApps.NBYS.FailedWebPart." + LoggingContext.WebPart] = true;
                    string message = removed == files.Count ? removed + " dosya geri dönüşüm kutusuna taşındı; liste yenilenemedi. Sayfayı yenileyin."
                        : removed > 0 ? removed + " dosya geri dönüşüm kutusuna taşındı; diğer dosyanın sonucu doğrulanamadı. Listeyi yenileyip kontrol edin."
                        : "Dosyaların kaldırıldığı doğrulanamadı. Listeyi yenileyip tekrar denemeden önce dosyaları kontrol edin.";
                    NbysWebPartLog.ReportError(LoggingContext, "NBYS.SPDosyaListesiWP.RecycleFiles", error, message);
                }
            });
        }
    }
}
