using System;
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
        }

        protected void lnkSearch_Click(object sender, EventArgs e)
        {
            lblStatus.Text = string.Empty;
            gvResults.DataSource = null;

            string siteUrl = (txtSiteUrl.Text ?? string.Empty).Trim();
            string webPartName = (txtWebPartName.Text ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(siteUrl) || string.IsNullOrEmpty(webPartName))
            {
                lblStatus.Text = "Lütfen site URL ve web part adını giriniz.";
                return;
            }

            try
            {
                var results = FindWebPartUsages(siteUrl, webPartName);
                gvResults.DataSource = results;
                gvResults.DataBind();

                if (results.Count == 0)
                {
                    lblStatus.Text = "Belirtilen web part hiçbir sayfada bulunamadı.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Arama sırasında hata oluştu: " + ex.Message;
            }
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
                catch
                {
                    // Sayfa web part yöneticisine erişilemiyorsa atla
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
