using Model.NBYS;
using Model.Ortak;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.ExtreDosyaYuklemeWP
{
    [ToolboxItemAttribute(false)]
    public partial class ExtreDosyaYuklemeWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ExtreDosyaYuklemeWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        bool isAkbankAktarildi = false;
        bool isGarantiAktarildi = false;
        bool isHalkBankAktarildi = false;
        bool isIsbankAktarildi = false;
        bool isVakifBankAktarildi = false;
        bool isZiraatAktarildi = false;
        bool isTEBAktarildi = false;
        //katilimlar
        bool isVakifKatilimAktarildi = false;
        bool isZiraatKatilimAktarildi = false;
        bool isHalkbank2Aktarildi = false;
        private string CurrentUserName
        {
            get
            {

                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUserLoginName();
                }
                return ViewState["CurrentUserName"].ToString();
            }

            set
            {
                ViewState["CurrentUserName"] = value;
            }
        }
        private string IslemTarihiQS
        {
            get
            {

                if (ViewState["IslemTarihi"] == null)
                {
                    if (Page.Request.QueryString["IslemTarihi"] != null)
                    {
                        ViewState["IslemTarihi"] = Page.Request.QueryString["IslemTarihi"];
                    }
                    else
                    {
                        ViewState["IslemTarihi"] = string.Empty;
                    }
                }
                return ViewState["IslemTarihi"].ToString();
            }

            set
            {
                ViewState["IslemTarihi"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    InitializeLabels();

                    if (!string.IsNullOrEmpty(IslemTarihiQS))
                    {
                        IslemTarihiTxt.Text = IslemTarihiQS;
                    }
                    else
                    {
                        DateTime today = DateTime.Now;
                        IslemTarihiTxt.Text = today.ToString(ProjeConstants.DATE_TR);
                    }
                    checkSavedFiles(IslemTarihiTxt.Text.ConvertToDatetime());
                    NextBtn.Visible = (isAkbankAktarildi || isGarantiAktarildi || isHalkBankAktarildi || isIsbankAktarildi || isVakifBankAktarildi || isZiraatAktarildi
                        || isHalkbank2Aktarildi || isVakifKatilimAktarildi || isZiraatKatilimAktarildi);
                }

            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

        }
        private void InitializeLabels()
        {
            AkbankLbl.Text = ProjeConstants.BANKA_AKBANK;
            GarantiLbl.Text = ProjeConstants.BANKA_GARANTI;
            HalkbankLbl.Text = ProjeConstants.BANKA_HALKBANK;
            IsbankLbl.Text = ProjeConstants.BANKA_ISBANK;
            ZiraatLbl.Text = ProjeConstants.BANKA_ZIRAAT;
            VakifbankLbl.Text = ProjeConstants.BANKA_VAKIF;

            Halkbank2Lbl.Text = ProjeConstants.BANKA_HALKBANK2;
            VakifKatilimLbl.Text = ProjeConstants.BANKA_VAKIF_KATILIM;
            ZiraatKatilimLbl.Text = ProjeConstants.BANKA_ZIRAAT_KATILIM;


            AkbankOkLbl.Text = string.Empty;
            GarantiOkLbl.Text = string.Empty;
            HalkbankOkLbl.Text = string.Empty;
            IsbankOkLbl.Text = string.Empty;
            ZiraatOkLbl.Text = string.Empty;
            VakifKatilimOk.Text = string.Empty;
            Halkbank2OkLbl.Text = string.Empty;
            VakifKatilimOk.Text = string.Empty;
            ZiraatKatilimOkLbl.Text = string.Empty;
        }
        private void checkSavedFiles(DateTime islemTarihi)
        {

            EkstreAktarma ekstreAktarma = new EkstreAktarma();
            isAkbankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_AKBANK, islemTarihi);
            if (isAkbankAktarildi)
            {
                AkbankFU.Enabled = false;
                AkbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }

            isGarantiAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_GARANTI, islemTarihi);
            if (isGarantiAktarildi)
            {
                GarantiFU.Enabled = false;
                GarantiOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isTEBAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_TEB, islemTarihi);
            if (isTEBAktarildi)
            {
                TebFU.Enabled = false;
                TebOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isHalkBankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_HALKBANK, islemTarihi);
            if (isHalkBankAktarildi)
            {
                HalkbankFU.Enabled = false;
                HalkbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isIsbankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ISBANK, islemTarihi);
            if (isIsbankAktarildi)
            {
                IsbankFU.Enabled = false;
                IsbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isVakifBankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_VAKIF, islemTarihi);
            if (isVakifBankAktarildi)
            {
                VakifbankFU.Enabled = false;
                VakifbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isVakifKatilimAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_VAKIF_KATILIM, islemTarihi);
            if (isVakifKatilimAktarildi)
            {
                VakifKatilimFU.Enabled = false;
                VakifKatilimOk.Text = "  " + ((char)0x221A).ToString();
            }
            isZiraatAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ZIRAAT, islemTarihi);
            if (isZiraatAktarildi)
            {
                ZiraatFU.Enabled = false;
                ZiraatOkLbl.Text = "  " + ((char)0x221A).ToString();
            }

            isZiraatKatilimAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ZIRAAT_KATILIM, islemTarihi);

            if (isZiraatKatilimAktarildi)
            {
                ZiraatKatilimFU.Enabled = false;
                ZiraatKatilimOkLbl.Text = "  " + ((char)0x221A).ToString();
            }

            isHalkbank2Aktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_HALKBANK2, islemTarihi);

            if (isHalkbank2Aktarildi)
            {
                Halkbank2FU.Enabled = false;
                Halkbank2OkLbl.Text = "  " + ((char)0x221A).ToString();
            }
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
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                AkbankSave();
                GarantiSave();
                HalkbankSave();
                Halkbank2Save();
                IsbankSave();
                ZiraatSave();
                TEBSave();
                ZiraatKatilimSave();
                VakifbankSave();
                VakifKatilimSave();
                checkSavedFiles(IslemTarihiTxt.Text.ConvertToDatetime());
                NextBtn.Visible = (isAkbankAktarildi || isGarantiAktarildi || isHalkBankAktarildi || isIsbankAktarildi || isVakifBankAktarildi || isZiraatAktarildi || isTEBAktarildi
                    || isHalkbank2Aktarildi || isVakifKatilimAktarildi || isZiraatKatilimAktarildi);

            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void AkbankSave()
        {
            if (!isAkbankAktarildi && AkbankFU.HasFile)
            {

                var exceptionHelper = EkstreAktarma.SaveAkBankFile(AkbankFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);
                if (exceptionHelper.Exceptions.Count > 0)
                {
                    exceptionHelper.PublishException();
                }

                AkbankFU.Enabled = false;
                AkbankOkLbl.Text = "  " + ((char)0x221A).ToString();
                isAkbankAktarildi = true;

            }
        }
        private void GarantiSave()
        {
            if (!isGarantiAktarildi && GarantiFU.HasFile)
            {

                var exceptionHelper = EkstreAktarma.SaveGarantiBankFile(GarantiFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);

                if (exceptionHelper.Exceptions.Count > 0)
                {
                    exceptionHelper.PublishException();
                }

                GarantiFU.Enabled = false;
                GarantiOkLbl.Text = "  " + ((char)0x221A).ToString();


            }
        }
        private void TEBSave()
        {
            if (!isTEBAktarildi && TebFU.HasFile)
            {

                var exceptionHelper = EkstreAktarma.SaveTEBBankFile(TebFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);

                if (exceptionHelper.Exceptions.Count > 0)
                {
                    exceptionHelper.PublishException();
                }

                TebFU.Enabled = false;
                TebOkLbl.Text = "  " + ((char)0x221A).ToString();


            }
        }
        private void HalkbankSave()
        {
            if (!isHalkBankAktarildi && HalkbankFU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveHalkbankFile(HalkbankFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);
                if (exceptionHelper.Exceptions.Count > 0)
                {
                    exceptionHelper.PublishException();
                }

                HalkbankFU.Enabled = false;
                HalkbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
        }
        private void IsbankSave()
        {
            if (!isIsbankAktarildi && IsbankFU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveIsBankFile(IsbankFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);

                if (exceptionHelper.Exceptions.Count > 0)
                {
                    exceptionHelper.PublishException();
                }
                IsbankFU.Enabled = false;
                IsbankOkLbl.Text = "  " + ((char)0x221A).ToString();


            }
        }
        private void VakifbankSave()
        {
            if (!isVakifBankAktarildi && VakifbankFU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveVakifBankFile(VakifbankFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);
                if (exceptionHelper.Exceptions.Count > 0)
                {
                    exceptionHelper.PublishException();
                }
                VakifbankFU.Enabled = false;
                VakifbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
        }
        private void VakifKatilimSave()
        {
            if (!isVakifBankAktarildi && VakifKatilimFU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveVakifKatilimFile(VakifKatilimFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);
                if (exceptionHelper.Exceptions.Count > 0)
                {
                    exceptionHelper.PublishException();
                }
                VakifKatilimFU.Enabled = false;
                VakifKatilimOk.Text = "  " + ((char)0x221A).ToString();
            }
        }
        private void ZiraatSave()
        {
            bool ziraatFileOk = false;

            if (!isZiraatAktarildi && ZiraatFU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveZiraatFile(ZiraatFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);

                if (exceptionHelper.Exceptions.Count > 0)
                {
                    exceptionHelper.PublishException();
                }
                ziraatFileOk = true;
                ZiraatFU.Enabled = false;
                ZiraatOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            if (isZiraatAktarildi || ziraatFileOk)
            {

            }
        }
        private void Halkbank2Save()
        {
            if (!isHalkbank2Aktarildi && Halkbank2FU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveHalkbank2File(Halkbank2FU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);
                if (exceptionHelper.Exceptions.Count > 0)
                {
                    exceptionHelper.PublishException();
                }
                Halkbank2FU.Enabled = false;
                Halkbank2OkLbl.Text = "  " + ((char)0x221A).ToString();
            }
        }
        private void ZiraatKatilimSave()
        {
            if (!isZiraatKatilimAktarildi && ZiraatKatilimFU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveZiraatKatilimFile(ZiraatKatilimFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);
                if (exceptionHelper.Exceptions.Count > 0)
                {
                    exceptionHelper.PublishException();
                }
                ZiraatKatilimFU.Enabled = false;
                ZiraatKatilimOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
        }
        protected void NextBtn_Click(object sender, EventArgs e)
        {
            RedirectWithTarih(ProjeConstants.PAGE_EKSTRE_LIST);
        }
        //protected void IslemTarihiSelectedBtn_Click(object sender, EventArgs e)
        //{
        //    RedirectWithTarih(ProjeConstants.PAGE_EKSTRE_DOSYAYUKLEME);
        //}
        protected void islemTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            RedirectWithTarih(ProjeConstants.PAGE_EKSTRE_DOSYAYUKLEME);
        }
        private void RedirectWithTarih(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl + "?IslemTarihi=" + IslemTarihiTxt.Text);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
    }
}
