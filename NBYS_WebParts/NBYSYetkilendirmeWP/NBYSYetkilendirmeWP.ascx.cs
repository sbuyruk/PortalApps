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
            YetkileriGetir();
        }

        private void YetkileriGetir()
        {
            ProgramYetki programYetki = new ProgramYetki();
            string bolgeler = "(7,8,9)";
            List<ProgramYetki> list = programYetki.SelectByProgramModul(ProjeConstants.NBYS, ProjeConstants.NBYS_BOLGE_NAKITBAGISCILISTESI, bolgeler);
            foreach (var item in list)
            {
                if (item.Kosul.Equals("Belge Istemiyor"))
                    BelgeIstemiyorChk.Checked = item.Deger;
                if (item.Kosul.Equals("Ulasilamiyor"))
                    UlasilamiyorChk.Checked = item.Deger;
            }
        }

        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
    }
}
