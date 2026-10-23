using MTS_WebParts.Logging;
using Utility.Logging;
using Model.MTS;
using Model.Services.MTS;
using Model.Portal;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace MTS_WebParts.TestWP
{
    [ToolboxItemAttribute(false)]
    public partial class TestWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("MTS", "TestWP")); } }
        private Toplanti ReadSampleMeeting()
        {
            var meeting = new Model.Services.Portal.ToplantiService().GetById(1773, LogContext);
            if (meeting == null) throw new MtsInputException("Örnek toplantı bulunamadı. Deneme verisini kontrol edin.");
            return meeting;
        }
        private void SendSample(Toplanti meeting, string subject, string place, DateTime start, DateTime finish, bool cancel)
        {
            MtsActivityNotificationService.Send(new Faaliyet { UniqueId = meeting.UniqueId, FaaliyetKonusu = subject,
                FaaliyetYeriStr = place, BaslangicTarihi = start, BitisTarihi = finish, Aciklama = meeting.Aciklama },
                "asbuyruk@tskgv.org.tr", cancel);
            Utility.HelperClasses.Messages.Success("Örnek takvim bildirimi gönderildi.");
        }

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TestWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack)
                {
                    MtsWebPartLog.RecordOpened(LogContext);
                    PanelTextBox.Focus();
                    myTimer.Interval = 3000;
                    myTimer.Enabled = true;
                }

            });
        }

        protected void myTimer_Tick(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "myTimer_Tick", () =>
            {
                TimeLiteral.Text = DateTime.UtcNow.Ticks.ToString();

            });
        }

        protected void TakvimeEkleBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "TakvimeEkleBtn_Click", () =>
            {
                Toplanti toplanti = ReadSampleMeeting();

                string baslik = "Deneme Randevu";
                string yer = "Makam Odasi";
                SendSample(toplanti, baslik, yer, DateTime.Now.AddDays(-10), DateTime.Now.AddHours(1), false);

            });
        }
        protected void TakvimDegistirBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "TakvimDegistirBtn_Click", () =>
            {
                Toplanti toplanti = ReadSampleMeeting();

                string baslik = "Deneme Randevu";
                string yer = "Makam Odasi";
                DateTime bastar = toplanti.BaslangicTarihi.AddDays(1);
                DateTime bittar = toplanti.BitisTarihi.AddDays(1);
                SendSample(toplanti, baslik, yer, bastar, bittar, false);

            });
        }
        protected void TakvimSilBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "TakvimSilBtn_Click", () =>
            {
                Toplanti toplanti = ReadSampleMeeting();

                string baslik = "Deneme Randevu Degisti";
                string yer = "Öbür Oda";
                DateTime bastar = toplanti.BaslangicTarihi.AddDays(1);
                DateTime bittar = toplanti.BitisTarihi.AddDays(1);
                SendSample(toplanti, baslik, yer, bastar, bittar, true);

            });
        }
    }
}
