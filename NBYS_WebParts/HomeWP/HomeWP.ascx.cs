using NBYS_WebParts.Logging;
using Utility.Logging;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;

namespace NBYS_WebParts.HomeWP
{
    [ToolboxItemAttribute(false)]
    public partial class HomeWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "HomeWP"); }
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
        public HomeWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }
        private string MesajQS
        {
            get
            {

                if (ViewState["Mesaj"] == null)
                {
                    if (Page.Request.QueryString["Mesaj"] != null)
                    {
                        ViewState["Mesaj"] = Page.Request.QueryString["Mesaj"];
                    }
                    else
                    {
                        ViewState["Mesaj"] = string.Empty;
                    }
                }
                return ViewState["Mesaj"].ToString();
            }

            set
            {
                ViewState["Mesaj"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
                if (!Page.IsPostBack && !string.IsNullOrEmpty(MesajQS))
                {
                    Messages.Info("İlgili sayfada düzenleme yapılıyor. Lütfen daha sonra tekrar deneyin.");
                    MesajQS = string.Empty;
                }
            });
        }
    }
}
