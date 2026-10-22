using Model.Services.NBYS;
using System.Linq;
using NBYS_WebParts.Logging;
using Utility.Logging;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.NBYSYetkilendirmeWP
{
    [ToolboxItemAttribute(false)]
    public partial class NBYSYetkilendirmeWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "NBYSYetkilendirmeWP"); }
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
        public NBYSYetkilendirmeWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
                KaydetBtn.Enabled = false;
                BelgeIstemiyorChk.Enabled = false;
                UlasilamiyorChk.Enabled = false;
                if (!Page.IsPostBack) YetkileriGetir();
            });
        }

        private void YetkileriGetir()
        {
            List<ProgramYetki> list = new NbysSupportReadService(LoggingContext).GetPermissionSettings();
            var declined = list.Where(item => string.Equals(item.Kosul, "Belge Istemiyor", StringComparison.Ordinal)).ToList();
            var unreachable = list.Where(item => string.Equals(item.Kosul, "Ulasilamiyor", StringComparison.Ordinal)).ToList();
            if (declined.Select(item => item.Deger).Distinct().Count() > 1 || unreachable.Select(item => item.Deger).Distinct().Count() > 1)
            {
                BelgeIstemiyorChk.Visible = UlasilamiyorChk.Visible = false;
                Messages.Warning("Bölge yetkileri farklı olduğundan ortak bir seçim gösterilemiyor. Yetki kayıtlarını bölge bazında kontrol edin.");
                return;
            }
            BelgeIstemiyorChk.Checked = declined.Any() && declined[0].Deger;
            UlasilamiyorChk.Checked = unreachable.Any() && unreachable[0].Deger;
            BelgeIstemiyorChk.Visible = declined.Any();
            UlasilamiyorChk.Visible = unreachable.Any();
            if (list.Count == 0) Messages.Info("Bu kapsam için kayıtlı yetki ayarı bulunamadı.");
            else if (!declined.Any() || !unreachable.Any()) Messages.Info("Bazı yetki ayarları kayıtlı olmadığı için gösterilmedi. Yetki kayıtlarını kontrol edin.");
        }

        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("KaydetBtn_Click", () =>
            {
                Messages.Info("Bu ekran yalnızca mevcut yetkileri gösterir. Yetki değişikliği kaydedilmedi.");
            });
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
                Page.Response.Redirect(currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME, false);
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
            });
        }
    }
}
