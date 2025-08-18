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
            if (!Page.IsPostBack)
            {
                UcretTanim ucretTanim = new UcretTanim();
                DateTime maxBitisTarihi = ucretTanim.SelectMaxBitisTarihi();
                decimal agi = ucretTanim.SelectAgi(maxBitisTarihi);
                AgiTxt.Text = agi.ToString();
                ArtisYuzdesiTxt.Text = "10,00";
                DateTime baslangicTarihi = maxBitisTarihi.AddDays(1);
                BaslangicTarihiTxt.Text = baslangicTarihi.ConvertToDatetimeEmptyIfNull();
                BitisTarihiTxt.Text = baslangicTarihi.AddMonths(6).AddDays(-1).ConvertToDatetimeEmptyIfNull(); 
            }
        }

        #region Methods

        private void KaydetModalAc()
        {

            MessageTitleLbl.Text = "Maaş Artışı";
            MessageTextLbl.Text = BaslangicTarihiTxt.Text + " ile " + BitisTarihiTxt.Text + " arasında geçerli olacak ve %" +ArtisYuzdesiTxt.Text+" artış yapılacak şekilde maaş tabloları kaydedilsin mi?";
            DeleteNowBtn.Visible = false;
            KaydetNowBtn.Visible = true;
            var openPopup = "OpenModal();";
            UtilityHelper.ScriptCalistir(openPopup);
        }
        private void SilModalAc()
        {
            UcretTanim ucretTanim = new UcretTanim();

            List<UcretTanim> list = ucretTanim.SelectByMaxGrupId();
            if (list.Count <= 1)
            {
                MessageHelper.PublishMessage("Silinecek bir maaş artışı bulunamadı.", ProjeConstants.MESAJ_BILGI);
                return;
            }
            ucretTanim = list[0];
            MessageTitleLbl.Text = "Son Maaş Artışı Ve Bu artışa göre yapılan Maaş Listesi Silinecek";
            MessageTextLbl.Text = ucretTanim.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + " ile " + ucretTanim.BitisTarihi.ConvertToDatetimeEmptyIfNull() + " arasında geçerli olan artış silinsin mi?";
            DeleteNowBtn.Visible = true;
            KaydetNowBtn.Visible = false;
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
        protected void SilBtn_Click(object sender, EventArgs e)
        {

            SilModalAc();
        }
        protected void KaydetNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                UcretTanim ucretTanim = new UcretTanim();

                DateTime yeniBaslangicTarihi = BaslangicTarihiTxt.Text.ConvertToDatetime();
                DateTime yeniBitisTarihi = BitisTarihiTxt.Text.ConvertToDatetime();

                List<UcretTanim> list = ucretTanim.SelectByMaxGrupId();
                decimal artis = ArtisYuzdesiTxt.Text.ConvertToDecimal();
                foreach (UcretTanim t in list)
                {
                    decimal zamliAltUcret = t.AltUcret + t.AltUcret * artis / 100;
                    decimal zamliUstUcret = t.UstUcret + t.UstUcret * artis / 100;
                    decimal zamliAskerUcret = t.AskerUcret + t.AskerUcret * artis / 100;

                    UcretTanim yeniUcretTanim = new UcretTanim();
                    yeniUcretTanim.AltUcret = zamliAltUcret;
                    yeniUcretTanim.UstUcret = zamliUstUcret;
                    yeniUcretTanim.AskerUcret = zamliAskerUcret;
                    yeniUcretTanim.Agi = AgiTxt.Text.ConvertToDecimal();
                    yeniUcretTanim.Derece = t.Derece;
                    yeniUcretTanim.Kademe = t.Kademe;
                    yeniUcretTanim.Unvan = t.Unvan;
                    yeniUcretTanim.BaslangicTarihi = yeniBaslangicTarihi;
                    yeniUcretTanim.BitisTarihi = yeniBitisTarihi;
                    yeniUcretTanim.GrupId = t.GrupId + 1;
                    int id = yeniUcretTanim.Save();
                }
                MessageHelper.PublishMessage("Tablolar oluşturuldu", ProjeConstants.MESAJ_BASARILI, 2000);
                RedirectToPage(ProjeConstants.PAGE_MAAS_TABLOLARI );
            }
            catch (Exception ex)
            {
                MessageHelper.PublishMessage("Tablolar kaydedilirken hata oluştu" + ex.Message, ProjeConstants.MESAJ_HATA);
                throw;
            }

        }
      
        protected void DeleteNowBtn_Click(object sender, EventArgs e)
        {
            UcretTanim ucretTanim = new UcretTanim();

            List<UcretTanim> list = ucretTanim.SelectByMaxGrupId();
            if (list.Count <= 1)
            {
                MessageHelper.PublishMessage("Silinecek bir maaş artışı bulunamadı.", ProjeConstants.MESAJ_BILGI);
                return;
            }
            ucretTanim = list[0];
            int grupId = ucretTanim.GrupId;
            try
            {
                bool isDeleted = ucretTanim.DeleteByGrupId(grupId);
                if (isDeleted)
                {
                    //MessageHelper.PublishMessage("Son maaş artışı silindi.", ProjeConstants.MESAJ_BASARILI, 2000);
                    MaasHareket hareket = new MaasHareket();
                    hareket.DeleteByGrupId(grupId);
                    RedirectToPage(ProjeConstants.PAGE_MAAS_TABLOLARI);
                }

            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
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
