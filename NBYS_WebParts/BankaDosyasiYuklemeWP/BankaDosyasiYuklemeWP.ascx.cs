using Model.NBYS;
using Model.Ortak;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.BankaDosyasiYuklemeWP
{
    [ToolboxItemAttribute(false)]
    public partial class BankaDosyasiYuklemeWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BankaDosyasiYuklemeWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        bool isAkbankAktarildi = false;
        bool isAkbankEkstreAktarildi = false;
        bool isGarantiAktarildi = false;
        bool isGarantiEkstreAktarildi = false;
        bool isHalkBankAktarildi = false;
        bool isHalkbank2Aktarildi = false;
        bool isIsbankAktarildi = false;
        bool isIsbankEkstreAktarildi = false;
        bool isVakifBankAktarildi = false;
        bool isVakifBankGunlukAktarildi = false;
        bool isVakifBank2Aktarildi = false;
        bool isZiraatAktarildi = false;
        //bool isZiraatMT940Aktarildi = false;
        bool isZiraatEkstreAktarildi = false;
        bool isTEBAktarildi = false;
        //katilimlar
        bool isVakifKatilimAktarildi = false;
        bool isZiraatKatilimAktarildi = false;
        bool isKartIleAktarildi = false;
        bool isEDevletAktarildi = false;
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
                    NextBtn.Visible = (isAkbankAktarildi || isAkbankEkstreAktarildi || isGarantiAktarildi || isGarantiEkstreAktarildi || isHalkBankAktarildi || isHalkbank2Aktarildi || isIsbankAktarildi || isIsbankEkstreAktarildi
                        || isVakifBankAktarildi || isVakifBankGunlukAktarildi || isVakifBank2Aktarildi
                        || isVakifKatilimAktarildi || isZiraatAktarildi || isZiraatEkstreAktarildi || isZiraatKatilimAktarildi) || isKartIleAktarildi || isEDevletAktarildi; //isZiraatMT940Aktarildi || 
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
            AkbankEkstreLbl.Text = ProjeConstants.BANKA_AKBANKEKSTRE;
            GarantiLbl.Text = ProjeConstants.BANKA_GARANTI;
            GarantiEkstreLbl.Text = ProjeConstants.BANKA_GARANTIEKSTRE;
            HalkbankLbl.Text = ProjeConstants.BANKA_HALKBANK;
            IsbankLbl.Text = ProjeConstants.BANKA_ISBANK;
            IsbankEkstreLbl.Text = ProjeConstants.BANKA_ISBANKEKSTRE;
            KartIleLbl.Text = ProjeConstants.BANKA_KARTILEBAGIS;
            EDevletLbl.Text = ProjeConstants.BANKA_EDEVLETBAGIS;
            ZiraatLbl.Text = ProjeConstants.BANKA_ZIRAAT;
            //ZiraatMT940Lbl.Text = ProjeConstants.BANKA_ZIRAATMT940;
            ZiraatEkstreLbl.Text = ProjeConstants.BANKA_ZIRAATEKSTRE;
            VakifbankLbl.Text = ProjeConstants.BANKA_VAKIF;
            VakifbankGunlukLbl.Text = ProjeConstants.BANKA_VAKIF;// GUNLUK;
            Vakifbank2Lbl.Text = ProjeConstants.BANKA_VAKIF2;

            Halkbank2Lbl.Text = ProjeConstants.BANKA_HALKBANK2;
            VakifKatilimLbl.Text = ProjeConstants.BANKA_VAKIF_KATILIM;
            ZiraatKatilimLbl.Text = ProjeConstants.BANKA_ZIRAAT_KATILIM;


            AkbankOkLbl.Text = string.Empty;
            AkbankEkstreOkLbl.Text = string.Empty;
            GarantiOkLbl.Text = string.Empty;
            GarantiEkstreOkLbl.Text = string.Empty;
            HalkbankOkLbl.Text = string.Empty;
            IsbankOkLbl.Text = string.Empty;
            IsbankEkstreOkLbl.Text = string.Empty;
            KartIleOkLbl.Text = string.Empty;
            EDevletOkLbl.Text = string.Empty;
            ZiraatOkLbl.Text = string.Empty;
            ZiraatEkstreOkLbl.Text = string.Empty;
            VakifbankOkLbl.Text = string.Empty;
            VakifbankGunlukOkLbl.Text = string.Empty;
            Vakifbank2OkLbl.Text = string.Empty;
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
            isAkbankEkstreAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_AKBANKEKSTRE, islemTarihi);
            if (isAkbankEkstreAktarildi)
            {
                AkbankEkstreFU.Enabled = false;
                AkbankEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isGarantiAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_GARANTI, islemTarihi);
            if (isGarantiAktarildi)
            {
                GarantiFU.Enabled = false;
                GarantiOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isGarantiEkstreAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_GARANTIEKSTRE, islemTarihi);
            if (isGarantiEkstreAktarildi)
            {
                GarantiEkstreFU.Enabled = false;
                GarantiEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
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

            isIsbankEkstreAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ISBANKEKSTRE, islemTarihi);
            if (isIsbankEkstreAktarildi)
            {
                IsbankEkstrebankFU.Enabled = false;
                IsbankEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isKartIleAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_KARTILEBAGIS, islemTarihi);
            if (isKartIleAktarildi)
            {
                KartIleFU.Enabled = false;
                KartIleOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isVakifBankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_VAKIF, islemTarihi);
            if (isVakifBankAktarildi)
            {
                VakifbankFU.Enabled = false;
                VakifbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isVakifBankGunlukAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_VAKIF, islemTarihi);//GUNLUK
            if (isVakifBankGunlukAktarildi)
            {
                VakifbankGunlukFU.Enabled = false;
                VakifbankGunlukOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isVakifBank2Aktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_VAKIF2, islemTarihi);
            if (isVakifBank2Aktarildi)
            {
                Vakifbank2FU.Enabled = false;
                Vakifbank2OkLbl.Text = "  " + ((char)0x221A).ToString();
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
            //isZiraatMT940Aktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ZIRAATMT940, islemTarihi);
            //if (isZiraatMT940Aktarildi)
            //{
            //    ZiraatMT940FU.Enabled = false;
            //    ZiraatMT940OkLbl.Text = "  " + ((char)0x221A).ToString();
            //}
            isZiraatEkstreAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ZIRAATEKSTRE, islemTarihi);
            if (isZiraatEkstreAktarildi)
            {
                ZiraatEkstreFU.Enabled = false;
                ZiraatEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
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
            isEDevletAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_EDEVLETBAGIS, islemTarihi);
            if (isEDevletAktarildi)
            {
                EDevletFU.Enabled = false;
                EDevletOkLbl.Text = "  " + ((char)0x221A).ToString();
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
                AkbankEkstreSave();
                GarantiSave();
                GarantiEkstreSave();
                HalkbankSave();
                Halkbank2Save();
                IsbankSave();
                IsbankEkstreSave();
                KartIleSave(BagisTarihiTxt.Value.ConvertToDatetime());
                ZiraatSave();
                //ZiraatMT940Save();
                ZiraatEkstreSave();
                TEBSave();
                ZiraatKatilimSave();
                VakifbankSave();
                VakifbankGunlukSaveTxt();
                Vakifbank2Save();
                VakifKatilimSave();
                EDevletSave();
                checkSavedFiles(IslemTarihiTxt.Text.ConvertToDatetime());
                NextBtn.Visible = (isAkbankAktarildi || isGarantiAktarildi || isGarantiEkstreAktarildi || isHalkBankAktarildi || isIsbankAktarildi || isIsbankEkstreAktarildi | isKartIleAktarildi
                    || isVakifBankAktarildi || isVakifBankGunlukAktarildi || isVakifBank2Aktarildi || isTEBAktarildi
                    || isHalkbank2Aktarildi || isVakifKatilimAktarildi || isZiraatAktarildi || isZiraatEkstreAktarildi || isZiraatKatilimAktarildi || isEDevletAktarildi);

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
                    AkbankFU.Enabled = true;
                    AkbankOkLbl.ForeColor = System.Drawing.Color.Red;
                    AkbankOkLbl.Text = "X";
                    exceptionHelper.PublishException();
                }

                else
                {
                    AkbankFU.Enabled = false;
                    AkbankOkLbl.ForeColor = System.Drawing.Color.Green;
                    AkbankOkLbl.Text = "  " + ((char)0x221A).ToString();
                }

            }
        }
        private void AkbankEkstreSave()
        {
            if (!isAkbankEkstreAktarildi && AkbankEkstreFU.HasFile)
            {

                var exceptionHelper = EkstreAktarma.SaveAkBankEkstreFile(AkbankEkstreFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);
                if (exceptionHelper.Exceptions.Count > 0)
                {
                    AkbankEkstreFU.Enabled = true;
                    AkbankEkstreOkLbl.ForeColor = System.Drawing.Color.Red;
                    AkbankEkstreOkLbl.Text = "X";
                    exceptionHelper.PublishException();
                }

                else
                {
                    AkbankEkstreFU.Enabled = false;
                    AkbankEkstreOkLbl.ForeColor = System.Drawing.Color.Green;
                    AkbankEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
                }

            }
        }
        private void GarantiSave()
        {
            if (!isGarantiAktarildi && GarantiFU.HasFile)
            {

                var exceptionHelper = EkstreAktarma.SaveGarantiBankFile(GarantiFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);

                if (exceptionHelper.Exceptions.Count > 0)
                {
                    GarantiFU.Enabled = true;
                    GarantiOkLbl.ForeColor = System.Drawing.Color.Red;
                    GarantiOkLbl.Text = "X";
                    exceptionHelper.PublishException();
                }

                else
                {
                    GarantiFU.Enabled = false;
                    GarantiOkLbl.ForeColor = System.Drawing.Color.Green;
                    GarantiOkLbl.Text = "  " + ((char)0x221A).ToString();
                }


            }
        }
        private void GarantiEkstreSave()
        {
            if (!isGarantiEkstreAktarildi && GarantiEkstreFU.HasFile)
            {

                var exceptionHelper = EkstreAktarma.SaveGarantiEkstreFile(GarantiEkstreFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);

                if (exceptionHelper.Exceptions.Count > 0)
                {
                    GarantiEkstreFU.Enabled = true;
                    GarantiEkstreOkLbl.ForeColor = System.Drawing.Color.Red;
                    GarantiEkstreOkLbl.Text = "X";
                    exceptionHelper.PublishException();
                }

                else
                {
                    GarantiEkstreFU.Enabled = false;
                    GarantiEkstreOkLbl.ForeColor = System.Drawing.Color.Green;
                    GarantiEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
                }


            }
        }
        private void TEBSave()
        {
            if (!isTEBAktarildi && TebFU.HasFile)
            {

                var exceptionHelper = EkstreAktarma.SaveTEBBankFile(TebFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);

                if (exceptionHelper.Exceptions.Count > 0)
                {
                    TebFU.Enabled = true;
                    TebOkLbl.ForeColor = System.Drawing.Color.Red;
                    TebOkLbl.Text = "X";
                    exceptionHelper.PublishException();
                }

                else
                {
                    TebFU.Enabled = false;
                    TebOkLbl.ForeColor = System.Drawing.Color.Green;
                    TebOkLbl.Text = "  " + ((char)0x221A).ToString();
                }


            }
        }
        private void HalkbankSave()
        {
            if (!isHalkBankAktarildi && HalkbankFU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveHalkbankFile(HalkbankFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);
                if (exceptionHelper.Exceptions.Count > 0)
                {
                    HalkbankFU.Enabled = true;
                    HalkbankOkLbl.ForeColor = System.Drawing.Color.Red;
                    HalkbankOkLbl.Text = "X";
                    exceptionHelper.PublishException();
                }

                else
                {
                    HalkbankFU.Enabled = false;
                    HalkbankOkLbl.ForeColor = System.Drawing.Color.Green;
                    HalkbankOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        private void Halkbank2Save()
        {
            if (!isHalkbank2Aktarildi && Halkbank2FU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveHalkbank2File(Halkbank2FU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);
                if (exceptionHelper.Exceptions.Count > 0)
                {
                    Halkbank2FU.Enabled = true;
                    Halkbank2OkLbl.ForeColor = System.Drawing.Color.Red;
                    Halkbank2OkLbl.Text = "X";
                    exceptionHelper.PublishException();
                }
                else
                {
                    Halkbank2FU.Enabled = false;
                    Halkbank2OkLbl.ForeColor = System.Drawing.Color.Green;
                    Halkbank2OkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        private void IsbankSave()
        {
            if (!isIsbankAktarildi && IsbankFU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveIsBankFile(IsbankFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);

                if (exceptionHelper.Exceptions.Count > 0)
                {
                    IsbankFU.Enabled = true;
                    IsbankOkLbl.ForeColor = System.Drawing.Color.Red;
                    IsbankOkLbl.Text = "X";
                    exceptionHelper.PublishException();
                }
                else
                {
                    IsbankFU.Enabled = false;
                    IsbankOkLbl.ForeColor = System.Drawing.Color.Green;
                    IsbankOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        private void IsbankEkstreSave()
        {
            if (!isIsbankEkstreAktarildi && IsbankEkstrebankFU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveIsBankEkstreFile(IsbankEkstrebankFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);

                if (exceptionHelper.Exceptions.Count > 0)
                {
                    IsbankEkstrebankFU.Enabled = true;
                    IsbankEkstreOkLbl.ForeColor = System.Drawing.Color.Red;
                    IsbankEkstreOkLbl.Text = "X";
                    exceptionHelper.PublishException();
                }
                else
                {
                    IsbankEkstrebankFU.Enabled = false;
                    IsbankEkstreOkLbl.ForeColor = System.Drawing.Color.Green;
                    IsbankEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        private void KartIleSave(DateTime bagisTarihi)
        {
            if (!isKartIleAktarildi && KartIleFU.HasFile)
            {
                if (string.IsNullOrEmpty(BagisTarihiTxt.Value))
                {
                    MessageHelper.PublishMessage("Lütfen Kart ile Bağış Tarihini giriniz!", ProjeConstants.MESAJ_HATA, 3000);
                    return;
                }
                var exceptionHelper = EkstreAktarma.SaveKartIleFile(KartIleFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, bagisTarihi);

                if (exceptionHelper.Exceptions.Count > 0)
                {
                    KartIleFU.Enabled = true;
                    KartIleOkLbl.ForeColor = System.Drawing.Color.Red;
                    KartIleOkLbl.Text = "X";
                    exceptionHelper.PublishException();
                }
                else
                {
                    KartIleFU.Enabled = false;
                    KartIleOkLbl.ForeColor = System.Drawing.Color.Green;
                    KartIleOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        private void VakifbankSave()
        {
            if (!isVakifBankAktarildi && VakifbankFU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveVakifBankFile(VakifbankFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);
                if (exceptionHelper.Exceptions.Count > 0)
                {
                    VakifbankFU.Enabled = true;
                    VakifbankOkLbl.ForeColor = System.Drawing.Color.Red;
                    VakifbankOkLbl.Text = "X";
                    exceptionHelper.PublishException();
                }
                else
                {
                    VakifbankFU.Enabled = false;
                    VakifbankOkLbl.ForeColor = System.Drawing.Color.Green;
                    VakifbankOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        private void VakifbankGunlukSaveTxt()
        {
            if (!isVakifBankGunlukAktarildi && VakifbankGunlukFU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveVakifBankGunlukTextFile(VakifbankGunlukFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);
                if (exceptionHelper.Exceptions.Count > 0)
                {
                    VakifbankGunlukFU.Enabled = true;
                    VakifbankGunlukOkLbl.ForeColor = System.Drawing.Color.Red;
                    VakifbankGunlukOkLbl.Text = "X";
                    exceptionHelper.PublishException();
                }
                else
                {
                    VakifbankGunlukFU.Enabled = false;
                    VakifbankGunlukOkLbl.ForeColor = System.Drawing.Color.Green;
                    VakifbankGunlukOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        private void Vakifbank2Save()
        {
            if (!isVakifBank2Aktarildi && Vakifbank2FU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveVakifBank2File(Vakifbank2FU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);
                if (exceptionHelper.Exceptions.Count > 0)
                {
                    Vakifbank2FU.Enabled = true;
                    Vakifbank2OkLbl.ForeColor = System.Drawing.Color.Red;
                    Vakifbank2OkLbl.Text = "X";
                    exceptionHelper.PublishException();
                }
                else
                {
                    Vakifbank2FU.Enabled = false;
                    Vakifbank2OkLbl.ForeColor = System.Drawing.Color.Green;
                    Vakifbank2OkLbl.Text = "  " + ((char)0x221A).ToString();
                }

            }
        }
        private void VakifKatilimSave()
        {
            if (!isVakifBankAktarildi && VakifKatilimFU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveVakifKatilimFile(VakifKatilimFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);
                if (exceptionHelper.Exceptions.Count > 0)
                {
                    VakifKatilimFU.Enabled = true;
                    VakifKatilimOk.Text = "X";
                    exceptionHelper.PublishException();
                }
                else
                {
                    VakifKatilimFU.Enabled = false;
                    VakifKatilimOk.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        private void ZiraatSave()
        {
            if (!isZiraatAktarildi && ZiraatFU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveZiraatFile(ZiraatFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);

                if (exceptionHelper.Exceptions.Count > 0)
                {
                    ZiraatFU.Enabled = true;
                    ZiraatOkLbl.ForeColor = System.Drawing.Color.Red;
                    ZiraatOkLbl.Text = "X";
                    exceptionHelper.PublishException();
                }
                else
                {
                    ZiraatFU.Enabled = false;
                    ZiraatOkLbl.ForeColor = System.Drawing.Color.Green;
                    ZiraatOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        //private void ZiraatMT940Save()
        //{
        //    if (!isZiraatMT940Aktarildi && ZiraatMT940FU.HasFile)
        //    {
        //        var exceptionHelper = EkstreAktarma.SaveZiraatMT940File(ZiraatMT940FU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);

        //        if (exceptionHelper.Exceptions.Count > 0)
        //        {
        //            ZiraatMT940FU.Enabled = true;
        //            ZiraatMT940OkLbl.ForeColor = System.Drawing.Color.Red;
        //            ZiraatMT940OkLbl.Text = "X";
        //            exceptionHelper.PublishException();
        //        }
        //        else
        //        {
        //            ZiraatMT940FU.Enabled = false;
        //            ZiraatMT940OkLbl.ForeColor = System.Drawing.Color.Green;
        //            ZiraatMT940OkLbl.Text = "  " + ((char)0x221A).ToString();
        //        }
        //    }
        //}
        private void ZiraatEkstreSave()
        {
            if (!isZiraatEkstreAktarildi && ZiraatEkstreFU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveZiraatEkstreFile(ZiraatEkstreFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);

                if (exceptionHelper.Exceptions.Count > 0)
                {
                    ZiraatEkstreFU.Enabled = true;
                    ZiraatEkstreOkLbl.ForeColor = System.Drawing.Color.Red;
                    ZiraatEkstreOkLbl.Text = "X";
                    exceptionHelper.PublishException();
                }
                else
                {
                    ZiraatEkstreFU.Enabled = false;
                    ZiraatEkstreOkLbl.ForeColor = System.Drawing.Color.Green;
                    ZiraatEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        private void ZiraatKatilimSave()
        {
            if (!isZiraatKatilimAktarildi && ZiraatKatilimFU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveZiraatKatilimFile(ZiraatKatilimFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);
                if (exceptionHelper.Exceptions.Count > 0)
                {
                    ZiraatKatilimFU.Enabled = false;
                    ZiraatKatilimOkLbl.ForeColor = System.Drawing.Color.Red;
                    ZiraatKatilimOkLbl.Text = "X";
                    exceptionHelper.PublishException();
                }
                else
                {
                    ZiraatKatilimFU.Enabled = false;
                    ZiraatKatilimOkLbl.ForeColor = System.Drawing.Color.Green;
                    ZiraatKatilimOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        private void EDevletSave()
        {
            if (!isEDevletAktarildi && EDevletFU.HasFile)
            {
                var exceptionHelper = EkstreAktarma.SaveEDevletFile(EDevletFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName);

                if (exceptionHelper.Exceptions.Count > 0)
                {
                    EDevletFU.Enabled = true;
                    EDevletOkLbl.ForeColor = System.Drawing.Color.Red;
                    EDevletOkLbl.Text = "X";
                    exceptionHelper.PublishException();
                }
                else
                {
                    EDevletFU.Enabled = false;
                    EDevletOkLbl.ForeColor = System.Drawing.Color.Green;
                    EDevletOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
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
            RedirectWithTarih(ProjeConstants.PAGE_BANKA_DOSYAYUKLEME);
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
