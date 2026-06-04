using Microsoft.Office.Audit.Schema.SharePoint;
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
            if (!Page.IsPostBack)
            {
                PanelTextBox.Focus();
                myTimer.Interval = 3000;
                myTimer.Enabled = true;
            }
        }

        protected void myTimer_Tick(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            TimeLiteral.Text = DateTime.UtcNow.Ticks.ToString();
        }

        protected void TakvimeEkleBtn_Click(object sender, EventArgs e)
        {
            Toplanti toplanti= new Toplanti();
            toplanti = toplanti.Select(1773);

            string from = "Makam Takip Sistemi <mts@tskgv.local>";
            string userto = "asbuyruk@tskgv.org.tr";
            string baslik = "Deneme Randevu";
            string yer = "Makam Odasi";
            MailHelper.TakvimeEkle(toplanti.UniqueId, from, userto, baslik, DateTime.Now.AddDays(-10), DateTime.Now.AddHours(1), yer, toplanti.Aciklama, ProjeConstants.PARAM_INTERNET_SMTP_IP_ADRESI);
        }
        protected void TakvimDegistirBtn_Click(object sender, EventArgs e)
        {
            Toplanti toplanti = new Toplanti();
            toplanti = toplanti.Select(1773);

            string from = "Makam Takip Sistemi <mts@tskgv.local>";
            string userto = "asbuyruk@tskgv.org.tr";
            string baslik = "Deneme Randevu";
            string yer = "Makam Odasi";
            DateTime bastar = toplanti.BaslangicTarihi.AddDays(1);
            DateTime bittar = toplanti.BitisTarihi.AddDays(1);
            MailHelper.TakvimeEkle(toplanti.UniqueId, from, userto, baslik, bastar, bittar, yer, toplanti.Aciklama, ProjeConstants.PARAM_INTERNET_SMTP_IP_ADRESI);
        }
        protected void TakvimSilBtn_Click(object sender, EventArgs e)
        {
            Toplanti toplanti = new Toplanti();
            toplanti = toplanti.Select(1773);

            string from = "Makam Takip Sistemi <mts@tskgv.local>";
            string userto = "asbuyruk@tskgv.org.tr";
            string baslik = "Deneme Randevu Degisti";
            string yer = "Öbür Oda";
            DateTime bastar = toplanti.BaslangicTarihi.AddDays(1);
            DateTime bittar = toplanti.BitisTarihi.AddDays(1);
            MailHelper.TakvimdenSil(toplanti.UniqueId, from, userto, baslik, bastar, bittar, yer, toplanti.Aciklama, ProjeConstants.PARAM_INTERNET_SMTP_IP_ADRESI);
        }
    }
}
