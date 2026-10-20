using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.Logging;
using TBYS_WebParts.Logging;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.EmptyWP
{
    [ToolboxItemAttribute(false)]
    public partial class EmptyWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get
            {
                return PortalLogContext.FromCurrentRequest("TBYS", "EmptyWP");
            }
        }

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public EmptyWP()
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!Page.IsPostBack) TbysWebPartLog.RecordOpened(LoggingContext);
            try
            {
                base.OnLoad(e);
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch (Exception error)
            {
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.EmptyWP.Page_Load", error);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
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
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.EmptyWP.RedirectToPage", ex);
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.EmptyWP.RedirectToPage", ex);
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
    }
}
