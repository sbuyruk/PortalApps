using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using Utility.ProjeGlobal;
using Model.TBYS;
using Utility.HelperClasses;
using Model.Ortak;
using Model.IKYS;
using System.Web.Security;
using System.Collections.Generic;

namespace IKYS_WebParts.MaasArtisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class MaasArtisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public MaasArtisiWP()
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
            ButtonTableDoldur();
            UcretTanim ucretTanim = new UcretTanim();
            DateTime maxBitisTarihi = ucretTanim.SelectMaxBitisTarihi();
            decimal agi = ucretTanim.SelectAgi(maxBitisTarihi);
            AgiTxt.Text = agi.ToString();
            ArtisYuzdesiTxt.Text = "10,00";
            BaslangicTarihiTxt.Text = maxBitisTarihi.AddDays(1).ConvertToDatetimeEmptyIfNull();
            BitisTarihiTxt.Text = maxBitisTarihi.AddMonths(6).ConvertToDatetimeEmptyIfNull();
        }

        #region Methods
        private void ButtonTableDoldur()
        {
            UcretTanim ucretTanim = new UcretTanim();
            List<Array> list = ucretTanim.SelectArtisTarihleri();
        }
        private void KaydetModalAc()
        {

            MessageTitleLbl.Text = "Maaş Artışı";
            MessageTextLbl.Text = BaslangicTarihiTxt.Text + " ile " + BitisTarihiTxt.Text + " arasında geçerli olacak ve %" +ArtisYuzdesiTxt.Text+" artış yapılacak şekilde maaş tabloları kaydedilsin mi?";
            DeleteNowBtn.Visible = false;
            KaydetNowBtn.Visible = true;
            var openPopup = "OpenModal();";
            UtilityHelper.ScriptCalistir(openPopup);
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        #endregion
        #region Events

        protected void KaydetBtn_Click(object sender, EventArgs e)
        {

            KaydetModalAc();
        }
        protected void KaydetNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                UcretTanim ucretTanim = new UcretTanim();
                DateTime maxBaslangicTarihi = ucretTanim.SelectMaxBaslangicTarihi();
                DateTime maxBitisTarihi = ucretTanim.SelectMaxBitisTarihi();
                DateTime yeniBaslangicTarihi = BaslangicTarihiTxt.Text.ConvertToDatetime();
                DateTime yeniBitisTarihi = BitisTarihiTxt.Text.ConvertToDatetime();

                List<UcretTanim> list = ucretTanim.SelectByBaslangicTarihiBitistarihi(maxBaslangicTarihi, maxBitisTarihi);
                decimal artis = ArtisYuzdesiTxt.Text.ConvertToDecimal();
                foreach (UcretTanim t in list)
                {
                    decimal zamliAltUcret = t.AltUcret + t.AltUcret * artis / 100;
                    decimal zamliUstUcret = t.UstUcret + t.UstUcret + artis / 100;
                    decimal zamliAskerUcret = t.AskerUcret + t.AskerUcret * artis / 100;

                    UcretTanim yeniUcretTanim = new UcretTanim();
                    yeniUcretTanim.AltUcret = zamliAltUcret;
                    yeniUcretTanim.AskerUcret = zamliAskerUcret;
                    yeniUcretTanim.Derece = t.Derece;
                    yeniUcretTanim.Kademe = t.Kademe;
                    yeniUcretTanim.Unvan = t.Unvan;
                    yeniUcretTanim.BaslangicTarihi = yeniBaslangicTarihi;
                    yeniUcretTanim.BitisTarihi = yeniBitisTarihi;
                    int id = yeniUcretTanim.Save();
                }
                MessageHelper.PublishMessage("Tablolar oluşturuldu", ProjeConstants.MESAJ_BASARILI, 2000);
            }
            catch (Exception ex)
            {
                MessageHelper.PublishMessage("Tablolar kaydedilirken hata oluştu" + ex.Message, ProjeConstants.MESAJ_HATA);
                throw;
            }

        }
        protected void DeleteBtn_Click(object sender, EventArgs e)
        {

        }
        protected void DeleteNowBtn_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Artış Silinemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }

        #endregion
    }
}
