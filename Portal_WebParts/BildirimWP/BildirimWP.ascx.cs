using Portal_WebParts.Logging;
using Utility.Logging;
using Model.IKYS;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.ProjeGlobal;
using static Model.IKYS.GorevOnay;

namespace Portal_WebParts.BildirimWP
{
    [ToolboxItemAttribute(false)]
    public partial class BildirimWP : WebPart, System.Web.UI.ICallbackEventHandler
    {
        private PortalModalActivity modalActivity;
        void System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
        {
            modalActivity.RaiseCallbackEvent(eventArgument);
        }
        string System.Web.UI.ICallbackEventHandler.GetCallbackResult()
        {
            return modalActivity.GetCallbackResult();
        }
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("Portal", "BildirimWP")); } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BildirimWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            modalActivity = PortalModalActivity.Register(this, LogContext, "#ModalAcDiv", ViewState);
            this.ChromeType = PartChromeType.None;
        }
        protected override void Render(HtmlTextWriter writer)
        {
            if (ProjeConstants.AMIRONAYIETKINMI)
            {
                base.Render(writer);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                AmirOnayLbl.Text = string.Empty;
                string login = LogContext.UserLogin ?? string.Empty;
                Personel personel = new PersonelService().GetByUserName(login.Substring(login.LastIndexOf('\\') + 1), LogContext);
                if (personel == null || personel.Id <= 0) throw new PortalInputException("Personel kaydınız bulunamadı. Bildirimler için sistem yöneticisine başvurun.");
                GorevOnayService gorevOnayService = new GorevOnayService();
                int kisiselBekleyenSayisi = gorevOnayService.GetOnayBekleyenGorevSayisiByPersonelId(personel.Id, LogContext);
                if (kisiselBekleyenSayisi > 0 && !Page.IsPostBack)
                {
                    string kisiselGorevMesaji = "Amir onayı bekleyen "
                        + kisiselBekleyenSayisi.ToString(CultureInfo.InvariantCulture) + " göreviniz bulunmaktadır.";
                    string target = new Uri(Page.Request.Url, ProjeConstants.PAGE_BEKLEYENISLEMLER).AbsoluteUri;
                    Utility.HelperClasses.Messages.Info(kisiselGorevMesaji, "Görevlerimi görüntüle", target);
                }

                if (!ProjeConstants.AMIRONAYIETKINMI) return;
                var pending = new HashSet<int>();
                foreach (var unit in new BirimTanimService().GetByAmirId(personel.Id, LogContext))
                    foreach (var item in gorevOnayService.SelectByBirimIdAndDurum(unit.Id, AmirOnayDurumu.OnayBekliyor, LogContext))
                        pending.Add(item.Id);
                if (pending.Count == 0) return;
                AmirOnayLbl.Text = "Onayınızı bekleyen " + pending.Count + " görev var.";
                if (!Page.IsPostBack)
                {
                    string target = new Uri(Page.Request.Url, ProjeConstants.PAGE_BEKLEYENISLEMLER).AbsoluteUri;
                    Utility.HelperClasses.Messages.Info(AmirOnayLbl.Text, "Bekleyen görevleri aç", target);
                }
            });
        }
    }
}
