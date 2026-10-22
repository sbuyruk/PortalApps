using System.Linq;
using Utility.Logging;
using NBYS_WebParts.Logging;
using Microsoft.SharePoint.JSGrid;
using Model.NBYS;
using Model.Ortak;
using Model.Services.NBYS;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.DuzenliBagisciDosyaYuklemeWP
{
    [ToolboxItemAttribute(false)]
    public partial class DuzenliBagisciDosyaYuklemeWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "DuzenliBagisciDosyaYuklemeWP"); }
        }
        private void ExecuteAction(string operation, Action action)
        {
            NbysWebPartLog.ExecuteAction(LoggingContext, operation, action);
        }

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public DuzenliBagisciDosyaYuklemeWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string CurrentUserName
        {
            get
            {
                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUserLoginName();
                }
                return ViewState["CurrentUserName"].ToString();
            }

            set
            {
                ViewState["CurrentUserName"] = value;
            }
        }
        private CultureInfo cultureInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);

                if (!Page.IsPostBack)
                {
                    YonergeLnk.HRef = UtilityHelper.YonergeURLGetir(ProjeConstants.PARAM_NBYSYONERGE, ProjeConstants.NBYSBELGELERI_LIB, ProjeConstants.PAGE_DUZENLIBAGISCI_LIST, LoggingContext);
                }
            });
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl,false);
            });
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("KaydetBtn_Click", () =>
            {
                DosyayiKaydet();
            });
        }
        protected void DuzenliBagisciListesiBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("DuzenliBagisciListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_DUZENLIBAGISCI_LIST);
            });
        }
        private void DosyayiKaydet()
        {
            if (!DosyaFU.HasFile) { Messages.Warning("Yüklenecek Excel dosyasını seçin."); return; }
            if (!string.Equals(Path.GetExtension(DosyaFU.FileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
            { Messages.Warning("Düzenli bağışçı şablonuna uygun .xlsx dosyası seçin."); return; }
            NbysRegularImportResult result = new NbysRegularWriteService(LoggingContext).Import(DosyaFU.FileContent);
            NbysRegularFeedback.Import(LoggingContext, result);
        }

        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            int index = currentUrl.LastIndexOf("?") < 0 ? currentUrl.Length : currentUrl.LastIndexOf("?");

            string rawUrl = currentUrl.Substring(0, index);
            string newUrl = rawUrl.Substring(0, rawUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl);

        }
    }

}
