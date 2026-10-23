using System;
using Portal_WebParts.Logging;
using Utility.Logging;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;
using System.Web.UI.WebControls.WebParts;
using WebPart = System.Web.UI.WebControls.WebParts.WebPart;

namespace Portal_WebParts.WebPartSearchWP
{
    [ToolboxItemAttribute(false)]
    public partial class WebPartSearchWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("Portal", "WebPartSearchWP")); } }
        private readonly List<Exception> searchErrors = new List<Exception>();
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public WebPartSearchWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack) PortalWebPartLog.RecordOpened(LogContext);
        }

        protected void lnkSearch_Click(object sender, EventArgs e)
        {
            try { Search(); }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                PortalTechnicalLog.WriteError(LogContext, "Portal.WebPartSearch.Search", error);
                lblStatus.Text = "Arama tamamlanamadı. Site adresini ve erişim izinlerini kontrol edip yeniden deneyin. Başvuru kodu: " + LogContext.OperationId.ToString("N");
            }
        }

        private void Search()
        {
            lblStatus.Text = string.Empty;
            gvResults.DataSource = null;
            gvResults.DataBind();
            searchErrors.Clear();

            string siteUrl = (txtSiteUrl.Text ?? string.Empty).Trim();
            string webPartName = (txtWebPartName.Text ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(siteUrl) || string.IsNullOrEmpty(webPartName))
            {
                lblStatus.Text = "Lütfen site URL ve web part adını giriniz.";
                return;
            }
            Uri siteUri;
            if (!Uri.TryCreate(siteUrl, UriKind.Absolute, out siteUri)
                || (siteUri.Scheme != Uri.UriSchemeHttp && siteUri.Scheme != Uri.UriSchemeHttps))
            {
                lblStatus.Text = "Geçerli bir SharePoint site adresi girin.";
                return;
            }

            var results = FindWebPartUsages(siteUrl, webPartName);
            gvResults.DataSource = results;
            gvResults.DataBind();

            if (searchErrors.Count > 0)
            {
                PortalTechnicalLog.WriteError(LogContext, "Portal.WebPartSearch.Partial", new AggregateException(searchErrors));
                lblStatus.Text = "Bazı sayfalar incelenemedi; sonuçlar eksik olabilir. Erişim izinlerini kontrol edip yeniden arayın. Başvuru kodu: " + LogContext.OperationId.ToString("N");
            }
            else if (results.Count == 0)
            {
                lblStatus.Text = "Belirtilen web part hiçbir sayfada bulunamadı.";
            }
            else lblStatus.Text = "Arama tamamlandı. " + results.Count + " sayfa bulundu.";

        }

        private List<WebPartUsage> FindWebPartUsages(string siteUrl, string webPartName)
        {
            var results = new List<WebPartUsage>();

            using (var site = new SPSite(siteUrl))
            {
                foreach (SPWeb web in site.AllWebs)
                {
                    try
                    {
                        SearchWebForWebPart(web, webPartName, results);
                    }
                    catch (System.Threading.ThreadAbortException) { throw; }
                    catch (Exception error)
                    {
                        searchErrors.Add(error);
                    }
                    finally
                    {
                        web.Dispose();
                    }
                }
            }

            return results;
        }

        private void SearchWebForWebPart(SPWeb web, string webPartName, List<WebPartUsage> results)
        {
            foreach (SPFile file in EnumerateAspxFiles(web))
            {
                SPLimitedWebPartManager wpManager = null;
                try
                {
                    wpManager = file.GetLimitedWebPartManager(System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared);

                    foreach (WebPart wp in wpManager.WebParts)
                    {
                        if (wp == null)
                        {
                            continue;
                        }

                        bool titleMatches = !string.IsNullOrEmpty(wp.Title) &&
                            wp.Title.IndexOf(webPartName, StringComparison.OrdinalIgnoreCase) >= 0;
                        bool typeMatches = wp.GetType().Name.IndexOf(webPartName, StringComparison.OrdinalIgnoreCase) >= 0;

                        if (titleMatches || typeMatches)
                        {
                            results.Add(new WebPartUsage
                            {
                                PageUrl = file.Url,
                                WebUrl = web.Url
                            });
                            break;
                        }
                    }
                }
                catch (System.Threading.ThreadAbortException) { throw; }
                catch (Exception error)
                {
                    searchErrors.Add(error);
                }
                finally
                {
                    if (wpManager != null)
                    {
                        wpManager.Web.Dispose();
                    }
                }
            }
        }

        private IEnumerable<SPFile> EnumerateAspxFiles(SPWeb web)
        {
            return EnumerateAspxFilesInFolder(web.RootFolder);
        }

        private IEnumerable<SPFile> EnumerateAspxFilesInFolder(SPFolder folder)
        {
            foreach (SPFile file in folder.Files)
            {
                if (file.Url.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
                {
                    yield return file;
                }
            }

            foreach (SPFolder subFolder in folder.SubFolders)
            {
                foreach (SPFile file in EnumerateAspxFilesInFolder(subFolder))
                {
                    yield return file;
                }
            }
        }

        private class WebPartUsage
        {
            public string PageUrl { get; set; }
            public string WebUrl { get; set; }
        }
    }
}
