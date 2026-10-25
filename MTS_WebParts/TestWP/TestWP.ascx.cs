using MTS_WebParts.Logging;
using Utility.Logging;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;

namespace MTS_WebParts.TestWP
{
    // Keep the deployed type and generated event bindings for existing page instances.
    [ToolboxItemAttribute(false)]
    public partial class TestWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("MTS", "TestWP")); } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            DisableSample();
        }

        private void DisableSample()
        {
            Title = "Devre Dışı Örnek Ekran";
            myTimer.Enabled = false;
            TakvimeEkleBtn.Visible = false;
            TakvimDegistirBtn.Visible = false;
            TakvimSilBtn.Visible = false;
            PanelTextBox.Visible = false;
            TimeLiteral.Text = "Bu örnek ekran devre dışı bırakılmıştır. Faaliyet işlemleri için MTS iş ekranlarını kullanın.";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                DisableSample();
                if (!Page.IsPostBack) MtsWebPartLog.RecordOpened(LogContext);
            });
        }

        protected void myTimer_Tick(object sender, EventArgs e) { DisableSample(); }
        protected void TakvimeEkleBtn_Click(object sender, EventArgs e) { DisableSample(); }
        protected void TakvimDegistirBtn_Click(object sender, EventArgs e) { DisableSample(); }
        protected void TakvimSilBtn_Click(object sender, EventArgs e) { DisableSample(); }
    }
}