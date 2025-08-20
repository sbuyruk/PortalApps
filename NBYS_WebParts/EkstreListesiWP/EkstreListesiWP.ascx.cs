using Model.NBYS;
using Model.Ortak;
using Model.TBYS;
using NBYS_WebParts.EkstreAktarmaEditWP;
using NBYS_WebParts.NakitBagisciEslestirWP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.EkstreListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class EkstreListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public EkstreListesiWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        private string IslemQS
        {
            get
            {
                if (ViewState["Islem"] == null)
                {
                    if (Page.Request.QueryString["Islem"] != null)
                    {
                        ViewState["Islem"] = Page.Request.QueryString["Islem"];
                    }
                    else
                    {
                        ViewState["Islem"] = string.Empty;
                    }
                }
                return ViewState["Islem"].ToString();
            }

            set
            {
                ViewState["Islem"] = value;
            }
        }
        private string AktarilanlarHaricQS
        {
            get
            {
                if (ViewState["AktarilanlarHaric"] == null)
                {
                    if (Page.Request.QueryString["AktarilanlarHaric"] != null)
                    {
                        ViewState["AktarilanlarHaric"] = Page.Request.QueryString["AktarilanlarHaric"];
                    }
                    else
                    {
                        ViewState["AktarilanlarHaric"] = AktarilanlarHaricChk.Checked;
                    }
                }
                return ViewState["AktarilanlarHaric"].ToString();
            }

            set
            {
                ViewState["AktarilanlarHaric"] = value;
            }
        }
        private string BankaQS
        {
            get
            {
                if (ViewState["Banka"] == null)
                {
                    if (Page.Request.QueryString["Banka"] != null)
                    {
                        ViewState["Banka"] = Page.Request.QueryString["Banka"];
                    }
                    else
                    {
                        ViewState["Banka"] = BankaDDL.SelectedItem.Value;
                    }
                }
                return ViewState["Banka"].ToString();
            }

            set
            {
                ViewState["Banka"] = value;
            }
        }
        private string MesajQS
        {
            get
            {

                if (ViewState["Mesaj"] == null)
                {
                    if (Page.Request.QueryString["Mesaj"] != null)
                    {
                        ViewState["Mesaj"] = Page.Request.QueryString["Mesaj"];
                    }
                    else
                    {
                        ViewState["Mesaj"] = string.Empty;
                    }
                }
                return ViewState["Mesaj"].ToString();
            }

            set
            {
                ViewState["Mesaj"] = value;
            }
        }
        private string SelectAllQS
        {
            get
            {
                if (ViewState["SelectAll"] == null)
                {
                    if (Page.Request.QueryString["SelectAll"] != null)
                    {
                        ViewState["SelectAll"] = Page.Request.QueryString["SelectAll"];
                    }
                    else
                    {
                        ViewState["SelectAll"] = false;
                    }
                }
                return ViewState["SelectAll"].ToString();
            }

            set
            {
                ViewState["SelectAll"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager sm = ScriptManager.GetCurrent(this.Page);
            if (sm != null)
            {
                sm.AsyncPostBackTimeout = 600; // saniye cinsinden (örnek: 5 dakika)
            }
            try
                {
                    if (!Page.IsPostBack)
                    {

                    if (!string.IsNullOrEmpty(MesajQS))
                        {
                            if (IslemQS.ToLower().Equals("silme"))
                            {
                                if (MesajQS.ToLower().Equals("true"))
                                    MessageHelper.PublishMessage("Kayıt Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                                else
                                    MessageHelper.PublishMessage("Kayıt Silinemedi", ProjeConstants.MESAJ_HATA);
                            }
                            else if (IslemQS.ToLower().Equals("kaydet"))
                            {
                                if (MesajQS.ToLower().Equals("true"))
                                    MessageHelper.PublishMessage("Aktarma tamamlandı.", ProjeConstants.MESAJ_BASARILI, 2000);
                                else
                                    MessageHelper.PublishMessage("Aktarma yapılamadı.", ProjeConstants.MESAJ_HATA);
                            }
                            MesajQS = string.Empty;
                        }
                        TumunuSecChk.Checked = SelectAllQS.ConvertToBool();
                        if (!string.IsNullOrEmpty(IslemTarihiQS))
                        {
                            IslemTarihiTxt.Text = IslemTarihiQS;
                        }
                        else
                        {
                            DateTime today = DateTime.Now;
                            IslemTarihiTxt.Text = today.ToString(ProjeConstants.DATE_TR);
                        }

                        BankaDDLDoldur();
                        if (!string.IsNullOrEmpty(BankaQS))
                        {
                            if (BankaDDL.Items.FindByValue(BankaQS) != null)
                                BankaDDL.SelectedValue = BankaDDL.Items.FindByValue(BankaQS).Value;
                        }
                        BankaEtiketleriniBaslat();
                        AktarilanBankalariOkLe(IslemTarihiTxt.Text.ConvertToDatetime());

                        AktarilanlarHaricChk.Checked = AktarilanlarHaricQS.ConvertToBool();
                        TabloOlustur();
                    }

                }
                catch (Exception ex)
                {
                    ExceptionHelper exHelper = new ExceptionHelper(ex);
                    exHelper.PublishException();
                }
            

        }
        private void BankaEtiketleriniBaslat()
        {
            AkbankLbl.Text = ProjeConstants.BANKA_AKBANK;
            AkbankEkstreLbl.Text = ProjeConstants.BANKA_AKBANKEKSTRE;
            EDevletLbl.Text = ProjeConstants.BANKA_EDEVLETBAGIS;
            GarantiLbl.Text = ProjeConstants.BANKA_GARANTI;
            HalkbankLbl.Text = ProjeConstants.BANKA_HALKBANK;
            Halkbank2Lbl.Text = ProjeConstants.BANKA_HALKBANK2;
            IsbankLbl.Text = ProjeConstants.BANKA_ISBANK;
            YKBEkstreLbl.Text = ProjeConstants.BANKA_YKBEKSTRE_KISA;
            ZiraatBankLbl.Text = ProjeConstants.BANKA_ZIRAAT;
            ZiraatBankEkstreLbl.Text = ProjeConstants.BANKA_ZIRAATEKSTRE;
            ZiraatKatilimLbl.Text = ProjeConstants.BANKA_ZIRAAT_KATILIM;
            VakifbankLbl.Text = ProjeConstants.BANKA_VAKIF;
            Vakifbank2Lbl.Text = ProjeConstants.BANKA_VAKIF2;
            KartIleLbl.Text = ProjeConstants.BANKA_KARTILEBAGIS;
            SMSVakifLbl.Text = ProjeConstants.BANKA_SMSVAKIF;
            KioskLbl.Text = ProjeConstants.BANKA_KIOSK;

            AkbankOkLbl.Text = string.Empty;
            AkbankEkstreOkLbl.Text = string.Empty;
            EDevletOkLbl.Text = string.Empty;
            GarantiOkLbl.Text = string.Empty;
            HalkbankOkLbl.Text = string.Empty;
            Halkbank2OkLbl.Text = string.Empty;
            IsbankOkLbl.Text = string.Empty;
            YKBEkstreOkLbl.Text = string.Empty;
            ZiraatBankOkLbl.Text = string.Empty;
            ZiraatBankEkstreOkLbl.Text = string.Empty;
            ZiraatKatilimOkLbl.Text = string.Empty;
            KartIleOkLbl.Text = string.Empty;
            SMSVakifOkLbl.Text = string.Empty;
            KioskOkLbl.Text = string.Empty;

        }
        private void AktarilanBankalariOkLe(DateTime islemTarihi)
        {

            EkstreAktarma ekstreAktarma = new EkstreAktarma();
            bool isAkbankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_AKBANK, islemTarihi);
            if (isAkbankAktarildi)
            {
                AkbankOkLbl.ForeColor = System.Drawing.Color.Green;
                AkbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                AkbankOkLbl.ForeColor = System.Drawing.Color.Red;
                AkbankOkLbl.Text = "X";
            }
            bool isAkbankEkstreAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_AKBANKEKSTRE, islemTarihi);
            if (isAkbankEkstreAktarildi)
            {
                AkbankEkstreOkLbl.ForeColor = System.Drawing.Color.Green;
                AkbankEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                AkbankEkstreOkLbl.ForeColor = System.Drawing.Color.Red;
                AkbankEkstreOkLbl.Text = "X";
            }
            bool isEDevletAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_EDEVLETBAGIS, islemTarihi);
            if (isEDevletAktarildi)
            {
                EDevletOkLbl.ForeColor = System.Drawing.Color.Green;
                EDevletOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                EDevletOkLbl.ForeColor = System.Drawing.Color.Red;
                EDevletOkLbl.Text = "X";
            }
            bool isGarantiAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_GARANTI, islemTarihi);
            if (isGarantiAktarildi)
            {
                GarantiOkLbl.ForeColor = System.Drawing.Color.Green;
                GarantiOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                GarantiOkLbl.ForeColor = System.Drawing.Color.Red;
                GarantiOkLbl.Text = "X";
            }
            bool isGarantiEkstreAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_GARANTIEKSTRE, islemTarihi);
            if (isGarantiEkstreAktarildi)
            {
                GarantiEkstreOkLbl.ForeColor = System.Drawing.Color.Green;
                GarantiEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                GarantiEkstreOkLbl.ForeColor = System.Drawing.Color.Red;
                GarantiEkstreOkLbl.Text = "X";
            }


            bool isHalkBankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_HALKBANK, islemTarihi);
            if (isHalkBankAktarildi)
            {
                HalkbankOkLbl.ForeColor = System.Drawing.Color.Green;
                HalkbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                HalkbankOkLbl.ForeColor = System.Drawing.Color.Red;
                HalkbankOkLbl.Text = "X";
            }
            bool isHalkbank2Aktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_HALKBANK2, islemTarihi);
            if (isHalkbank2Aktarildi)
            {
                Halkbank2OkLbl.ForeColor = System.Drawing.Color.Green;
                Halkbank2OkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                Halkbank2OkLbl.ForeColor = System.Drawing.Color.Red;
                Halkbank2OkLbl.Text = "X";
            }
            bool isIsbankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ISBANK, islemTarihi);
            if (isIsbankAktarildi)
            {
                IsbankOkLbl.ForeColor = System.Drawing.Color.Green;
                IsbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                IsbankOkLbl.ForeColor = System.Drawing.Color.Red;
                IsbankOkLbl.Text = "X";
            }
            bool isIsbankEkstreAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ISBANKEKSTRE, islemTarihi);
            if (isIsbankEkstreAktarildi)
            {
                IsbankEkstreOkLbl.ForeColor = System.Drawing.Color.Green;
                IsbankEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                IsbankEkstreOkLbl.ForeColor = System.Drawing.Color.Red;
                IsbankEkstreOkLbl.Text = "X";
            }
            bool isYKBEkstreAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_YKBEKSTRE, islemTarihi);
            if (isYKBEkstreAktarildi)
            {
                YKBEkstreOkLbl.ForeColor = System.Drawing.Color.Green;
                YKBEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                YKBEkstreOkLbl.ForeColor = System.Drawing.Color.Red;
                YKBEkstreOkLbl.Text = "X";
            }

            bool isVakifBankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_VAKIF, islemTarihi);
            if (isVakifBankAktarildi)
            {
                VakifbankOkLbl.ForeColor = System.Drawing.Color.Green;
                VakifbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                VakifbankOkLbl.ForeColor = System.Drawing.Color.Red;
                VakifbankOkLbl.Text = "X";
            }
            bool isVakifBank2Aktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_VAKIF2, islemTarihi);
            if (isVakifBank2Aktarildi)
            {
                Vakifbank2OkLbl.ForeColor = System.Drawing.Color.Green;
                Vakifbank2OkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                Vakifbank2OkLbl.ForeColor = System.Drawing.Color.Red;
                Vakifbank2OkLbl.Text = "X";
            }
            bool isVakifKatilimAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_VAKIF_KATILIM, islemTarihi);
            if (isVakifKatilimAktarildi)
            {
                VakifKatilimOkLbl.ForeColor = System.Drawing.Color.Green;
                VakifKatilimOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                VakifKatilimOkLbl.ForeColor = System.Drawing.Color.Red;
                VakifKatilimOkLbl.Text = "X";
            }
            bool isZiraatAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ZIRAAT, islemTarihi);
            if (isZiraatAktarildi)
            {
                ZiraatBankOkLbl.ForeColor = System.Drawing.Color.Green;
                ZiraatBankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                ZiraatBankOkLbl.ForeColor = System.Drawing.Color.Red;
                ZiraatBankOkLbl.Text = "X";
            }
            bool isZiraatEkstreAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ZIRAATEKSTRE, islemTarihi);
            if (isZiraatEkstreAktarildi)
            {
                ZiraatBankEkstreOkLbl.ForeColor = System.Drawing.Color.Green;
                ZiraatBankEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                ZiraatBankEkstreOkLbl.ForeColor = System.Drawing.Color.Red;
                ZiraatBankEkstreOkLbl.Text = "X";
            }
            bool isZiraatKatilimAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ZIRAAT_KATILIM, islemTarihi);
            if (isZiraatKatilimAktarildi)
            {
                ZiraatKatilimOkLbl.ForeColor = System.Drawing.Color.Green;
                ZiraatKatilimOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                ZiraatKatilimOkLbl.ForeColor = System.Drawing.Color.Red;
                ZiraatKatilimOkLbl.Text = "X";
            }

            bool isTebAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_TEB, islemTarihi);
            if (isTebAktarildi)
            {
                TebOkLbl.ForeColor = System.Drawing.Color.Green;
                TebOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                TebOkLbl.ForeColor = System.Drawing.Color.Red;
                TebOkLbl.Text = "X";
            }

            bool isKartIleAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_KARTILEBAGIS, islemTarihi);
            if (isKartIleAktarildi)
            {
                KartIleOkLbl.ForeColor = System.Drawing.Color.Green;
                KartIleOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                KartIleOkLbl.ForeColor = System.Drawing.Color.Red;
                KartIleOkLbl.Text = "X";
            }

            bool isSMSVakifAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_SMSVAKIF, islemTarihi);
            if (isSMSVakifAktarildi)
            {
                SMSVakifOkLbl.ForeColor = System.Drawing.Color.Green;
                SMSVakifOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                SMSVakifOkLbl.ForeColor = System.Drawing.Color.Red;
                SMSVakifOkLbl.Text = "X";
            }

            bool isKioskAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_KIOSK, islemTarihi);
            if (isKioskAktarildi)
            {
                KioskOkLbl.ForeColor = System.Drawing.Color.Green;
                KioskOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            else
            {
                KioskOkLbl.ForeColor = System.Drawing.Color.Red;
                KioskOkLbl.Text = "X";
            }
        }
        protected void SecilenListeyiKaydet()
        {
            string value = paramArray.Value;

            string currentUser = UtilityHelper.GetCurrentUserLoginName();
            if (!string.IsNullOrEmpty(value))
            {
                EkstreAktarma eaDao = new EkstreAktarma();
                int rowCount = 0;
                List<EkstreAktarma> aktarilmayanlar = eaDao.selectByEkstreIdList(value, ref rowCount);
                if (aktarilmayanlar.Count > 0)
                {
                    //var numbers = value?.Split(',')?.Select(Int32.Parse)?.ToList();
                    var exceptionHelper = EkstreAktarma.SaveAll(aktarilmayanlar, currentUser); //seçilenler diğer tablolara dağıtılıyor
                   
                    if (exceptionHelper.Exceptions.Count > 0)
                    {
                       
                        exceptionHelper.PublishException();
                    }
                    else
                    {
                        UtilityHelper.ScriptCalistir("CloseModalOnay();");
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "HideLoader", "$('#customLoader').hide();", true);
                        //RedirectToPage(ProjeConstants.PAGE_EKSTRE_LIST + "?Mesaj=true&Islem=kaydet&basarili=true&IslemTarihi=" + IslemTarihiQS + "&AktarilanlarHaric=" + AktarilanlarHaricQS + "&Banka=" + BankaQS);
                    }
                    RedirectToPage(ProjeConstants.PAGE_EKSTRE_LIST);
                }
            }
            else
            {
                MessageHelper.PublishMessage("Hiç kayıt seçilmedi. Devam etmek için en az bir kayıt seçiniz.", ProjeConstants.MESAJ_BILGI);
            }
        }
        protected void SecilenListeyiSil()
        {
            string value = paramArray.Value;
            //string[] idList = value.Split(',');
            //foreach (string item in idList)
            //{

            //}

            string currentUser = UtilityHelper.GetCurrentUserLoginName();
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    EkstreAktarma eaDao = new EkstreAktarma();
                    List<EkstreAktarma> silinecekler = eaDao.selectByIdList(value);
                    string mesaj = string.Empty;
                    int counter = 0;
                    foreach (EkstreAktarma item in silinecekler)
                    {
                        mesaj += " #" + counter + ":" + item.Adi;// + " BagisTarihi:" + item.BagisTarihi + " BagisMiktari:" + item.Tutar + " IslemTarihi:" + item.IslemTarihi.ConvertToDatetimeEmptyIfNull();
                        item.Delete();

                    }
                    SilinenKayit sk = new SilinenKayit();
                    sk.Silen = currentUser;
                    sk.SilinmeSebebi = counter + " adet kayıt silindi";
                    sk.TabloAdi = "EkstreAktarma_Table";
                    sk.SilinmeTarihi = DateTime.Now.ReturnTRDateFormat();
                    sk.SilinenKayitBilgisi = mesaj;
                    sk.Save();

                    RedirectToPage(ProjeConstants.PAGE_EKSTRE_LIST + "?IslemTarihi=" + IslemTarihiQS + "&AktarilanlarHaric=" + AktarilanlarHaricQS + "&Banka=" + BankaQS + "&Mesaj=true&Islem=Silme&Basarili=true");
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "CloseModalOnay();", true);
                    ExceptionHelper exHelper = new ExceptionHelper(ex);
                    exHelper.PublishException();
                }
            }
            else
            {
                MessageHelper.PublishMessage("Hiç kayıt seçilmedi. Devam etmek için en az bir kayıt seçiniz.", ProjeConstants.MESAJ_BILGI);
            }
        }
        private void BankaDDLDoldur()
        {
            List<EkstreAktarmaListItem> returnlist = new List<EkstreAktarmaListItem>();

            if (BankaDDL.SelectedItem == null)
            {
                BankaDDL.Items.Clear();
                ListItem li = new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI_INT.ToString());
                BankaDDL.Items.Add(li);
                BankaTanim pBanka = new BankaTanim();
                List<BankaTanim> list = pBanka.SelectAll<BankaTanim>();
                //TextInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true).TextInfo;
                foreach (BankaTanim banka in list)
                {
                    //string UpperCaseBanka = culturInfo.ToUpper(banka.Banka);
                    BankaDDL.Items.Add(new ListItem(banka.Banka, banka.Id.ToString()));
                }
            }
            if (BankaDDL.Items.FindByValue(ProjeConstants.HEPSI_INT.ToString()) != null)
                BankaDDL.SelectedValue = BankaDDL.Items.FindByValue(ProjeConstants.HEPSI_INT.ToString()).Value;
        }
        private List<EkstreAktarmaListItem> GetDataList()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            DateTime islemTarihiDateTime = IslemTarihiQS.ConvertToDatetime();
            if (string.IsNullOrEmpty(IslemTarihiQS))
            {
                islemTarihiDateTime = DateTime.Today;
            }
            EkstreAktarma ea = new EkstreAktarma();
            int rowCount = 0;
            string banka = BankaDDL.SelectedItem.Text;
            DataTable dataTable = ea.SelectByIslemTarihi(islemTarihiDateTime, ref rowCount, AktarilanlarHaricQS.ConvertToBool(), banka);

            //RowCountLbl.Text = "Kayıt Sayısı : " + rowCount.ToString();
            List<EkstreAktarmaListItem> returnlist = new List<EkstreAktarmaListItem>();
            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    EkstreAktarmaListItem ekstreAktarmaListItem = new EkstreAktarmaListItem();

                    ekstreAktarmaListItem.EkstreAktarmaId = dataRow["EkstreAktarmaId"].ToString();
                    ekstreAktarmaListItem.BankaAdi = dataRow["BankaAdi"].ToString();
                    ekstreAktarmaListItem.TCKimlikNo = dataRow["TCKimlikNo"].ToString();

                    ekstreAktarmaListItem.AdiSoyadi = dataRow["AdiSoyadi"].ToString();
                    ekstreAktarmaListItem.Telefon = dataRow["Telefon"].ToString();
                    ekstreAktarmaListItem.Telefon1 = dataRow["Telefon1"].ToString();
                    ekstreAktarmaListItem.Telefon2 = dataRow["Telefon2"].ToString();
                    ekstreAktarmaListItem.Aciklama = dataRow["Aciklama"].ToString();
                    ekstreAktarmaListItem.BagisTarihi = dataRow["BagisTarihi"].ToString().ConvertToDatetimeEmptyIfNull();
                    decimal tutar = dataRow["Tutar"].ConvertToDecimal();
                    ekstreAktarmaListItem.Tutar = tutar > 0 ? tutar.ToString("N", culturInfo) : "";
                    string dovizCinsi = dataRow["DovizCinsi"].ToString();
                    ekstreAktarmaListItem.DovizCinsi = dovizCinsi;
                    if (!dovizCinsi.Equals(ProjeConstants.DOVIZ_TL))
                    {
                        decimal dovizTutari = dataRow["DovizTutari"].ConvertToDecimal();
                        ekstreAktarmaListItem.DovizTutari = dovizTutari > 0 ? dovizTutari.ToString("N", culturInfo) : "";
                        decimal dovizKuru = dataRow["DovizKuru"].ConvertToDecimal();
                        ekstreAktarmaListItem.DovizKuru = dovizKuru > 0 ? dovizKuru.ToString("N", culturInfo) : "";
                        ekstreAktarmaListItem.KurTarihi = dataRow["KurTarihi"].ToString().ConvertToDatetimeEmptyIfNull();
                    }
                    ekstreAktarmaListItem.Duzenle = "<a href=" + ProjeConstants.PAGE_EKSTRE_AKTARMAEDIT + "?SenderApp=KD&SenderApp=KL&EkstreAktarmaId=" + ekstreAktarmaListItem.EkstreAktarmaId + " class='btn btn-outline-primary'>Düzenle</a>";
                    ekstreAktarmaListItem.Eslestir = "<a href=" + ProjeConstants.PAGE_NAKITBAGISCI_ESLESTIR + "?SenderApp=KD&SenderApp=KL&EkstreAktarmaId=" + ekstreAktarmaListItem.EkstreAktarmaId + " class='btn btn-outline-primary'>Eşleştir</a>";

                   bool aktarildiMi = dataRow["AktarildiMi"].ConvertToBool();
                    ekstreAktarmaListItem.AktarildiMi = aktarildiMi.ToString();
                    if (aktarildiMi)
                    {
                        ekstreAktarmaListItem.SecKaydet = string.Empty;
                        ekstreAktarmaListItem.Duzenle = string.Empty;
                        ekstreAktarmaListItem.Eslestir = string.Empty;
                    }
                    else
                    {
                        ekstreAktarmaListItem.SecKaydet = SecCheckBoxLinki(ProjeConstants.KAYDET,
                            aktarildiMi, ekstreAktarmaListItem.EkstreAktarmaId.ConvertToInt());
                    }
                    returnlist.Add(ekstreAktarmaListItem);
                }
            }
            return returnlist;
        }
        private string SecCheckBoxLinki(string kaydetSil, bool aktarildiMi, int ekstreAktarmaId)
        {
            string retVal;
            if (aktarildiMi)
            {
                
                if (kaydetSil.Equals(ProjeConstants.KAYDET))
                {
                    retVal = "<input id=chk type=checkbox checked disabled class='ekstre-aktarildi row-checkbox' />";
                    
                }
                else
                {
                    retVal = string.Empty;
                }
            }
            else
            {
                string tumusecildi = TumunuSecChk.Checked ? " checked " : "";

                if (kaydetSil.Equals(ProjeConstants.KAYDET))
                {
                    retVal = @"<input id=chk class='ekstre-aktarilmadi row-checkbox' onchange=addRemoveEkstreIdToList(" + ekstreAktarmaId + ",this); type=checkbox "+ tumusecildi +" />";
                }
                else
                {
                    retVal = @"<input id=chk class='ekstre-aktarilmadi row-checkbox' onchange=addRemoveEkstreIdToDeleteList(" + ekstreAktarmaId + ",this); type=checkbox  "+ tumusecildi +" />";
                }
                
            }
            return retVal;
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
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToExcel();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void ExportToExcel()
        {
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;
            List<NakitBagisci> list = new List<NakitBagisci>();
            GridView1.DataSource = GetDataList(); ;
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=EkstreAktarmaListesi_" + IslemTarihiQS.ConvertToDatetimeEmptyIfNull() + ".xls");
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            Page.Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                //Apply text style to each Row
                GridView1.Rows[i].Attributes.Add("class", "textmode");
            }
            GridView1.RenderControl(hw);

            //style to format numbers to string
            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Page.Response.Write(style);
            Page.Response.Output.Write(sw.ToString());
            Page.Response.Flush();
            Page.Response.End();

        }
        private class EkstreAktarmaListItem
        {
            public string EkstreAktarmaId { get; set; }
            public string TCKimlikNo { get; set; }
            public string Telefon { get; set; }
            public string Telefon1 { get; set; }
            public string Telefon2 { get; set; }
            public string AdiSoyadi { get; set; }
            public string Adres { get; set; }
            public string Aciklama { get; set; }
            public string BagisTarihi { get; set; }
            public string BankaAdi { get; set; }
            public string Tutar { get; set; }
            public string AktarildiMi { get; set; }
            public string DovizCinsi { get; set; }
            public string DovizTutari { get; set; }
            public string DovizKuru { get; set; }
            public string KurTarihi { get; set; }
            public string SecKaydet { get; set; }
            public string Duzenle { get; set; }
            public string Eslestir { get; set; }

        }
        protected void IslemTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            IslemTarihiQS = IslemTarihiTxt.Text.ConvertToDatetimeEmptyIfNull();
            RedirectToPage(ProjeConstants.PAGE_EKSTRE_LIST + "?IslemTarihi=" + IslemTarihiQS + "&AktarilanlarHaric=" + AktarilanlarHaricQS + "&Banka=" + BankaQS);
        }
        protected void AktarilanlarHaricChk_CheckedChanged(object sender, EventArgs e)
        {
            AktarilanlarHaricQS = AktarilanlarHaricChk.Checked.ToString();
            RedirectToPage(ProjeConstants.PAGE_EKSTRE_LIST + "?IslemTarihi=" + IslemTarihiQS + "&AktarilanlarHaric=" + AktarilanlarHaricQS + "&Banka=" + BankaQS);
        }
        protected void BankaDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            BankaQS = BankaDDL.SelectedItem.Value;
            RedirectToPage(ProjeConstants.PAGE_EKSTRE_LIST + "?IslemTarihi=" + IslemTarihiQS + "&AktarilanlarHaric=" + AktarilanlarHaricQS + "&Banka=" + BankaQS);
        }
        protected void SecilenleriSilBtn_Click(object sender, EventArgs e)
        {

            string value = paramArray.Value;
            string[] idList = value.Split(',');
            if (idList.Length < 1)
            {
                MessageHelper.PublishMessage("Silmek için kayıt seçiniz.", ProjeConstants.MESAJ_BILGI, 2000);
            }
            else
            {
                ModalTitleLbl.Text = "Seçilen Kayıtlar Silinecek";
                ModalSubTitleLbl.Text = idList.Length + " Adet kaydı silmek için seçtiniz.";
                UyariMesajiLbl.Text = "Lütfen silmeden önce dikkatle inceleyiniz.";
                SilNowBtn.Visible = true;
                KaydetNowBtn.Visible = false;
                var openPopup = "OpenModalOnay();";
                UtilityHelper.ScriptCalistir( openPopup);
            }

        }
        protected void SilNowBtn_Click(object sender, EventArgs e)
        {
            SecilenListeyiSil();
        }
        protected void TumunuSecChk_CheckedChanged(object sender, EventArgs e)
        {
            SelectAllQS = TumunuSecChk.Checked.ToString();
            //paramArray.Value = TumunuSecChk.Checked ? GetAllIds() : string.Empty;
            TabloOlustur();
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<EkstreAktarmaListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                jSon = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                jQuery(document).ready(function () {
                    jQuery('#CustomDataTable').DataTable({
                        'initComplete': function (settings, json) {//tablo yüklendiğinde
                            var api = this.api();
                            var row = api.row(function (idx, data, node) { //secilen toplantıya gider
                                return data['Secildi'] == true;
                            });
                            if (row.length > 0) {
                                row.select()
                                    .show()
                                    .draw(false);
                            }
                        },
                        data: " + jsonData + @",
                        columns: [
                            { data: 'SecKaydet' },
                            { data: 'EkstreAktarmaId' },
                            { data: 'BankaAdi' },
                            { data: 'TCKimlikNo' },
                            { data: 'AdiSoyadi' },
                            { data: 'Telefon' },
                            { data: 'BagisTarihi' },
                            { data: 'Tutar' },
                            { data: 'Aciklama' },               
                            { data: 'Duzenle' },               
                            { data: 'Eslestir' },               
                        ],
                        'columnDefs': [
                            { 'width': '20%', 'targets': 4 },
                            { 'width': '25%', 'targets': 8 },
                            { targets: 7, className: 'dt-body-right'},
                            { targets: 0, render:function(data,type,row,data){
                                
                                if (row.AktarildiMi == 'True')
                                {
                                    return '<input id=chk type=checkbox checked disabled class=ekstre-aktarildi />';
                                }
                                else
                                {
                                    var isChecked='';
                                    if ('" + SelectAllQS.ConvertToBool() + @"'=='True')
                                    {
                                    
                                        isChecked='checked';
                                        EkleCikar(row.EkstreAktarmaId,isChecked);
                                    }else
                                    {
                                        isChecked='';
                                        EkleCikar(row.EkstreAktarmaId,isChecked);
                                    }
                                    
                                    return ('<input id=chk class=ekstre-aktarilmadi '+isChecked+' onchange=addRemoveEkstreIdToList('+row.EkstreAktarmaId+',this); type=checkbox />');
                                }

                                return moment(data).format('DD.MM.YYYY');
                            }},
                        ],
                        'language': {
                             'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        dom: 'frtip',
                        pageLength: 10,
                        'createdRow': function(row, data, dataIndex) {
                            if (data.AktarildiMi=='True')
                            {
                                $(row).addClass('ekstre-aktarildi');

                            }else{
                                $(row).addClass('ekstre-aktarilmadi');
                            }
                        },//set row color


                        });
                    });

            ";

            return tableString;
        }

        protected void SecilenleriKaydetBtn_Click(object sender, EventArgs e)
        {

            string value = paramArray.Value;
            string[] idList = value.Split(',');

            if (idList.Length < 1)
            {
                MessageHelper.PublishMessage("Kaydetmek için kayıt seçiniz.", ProjeConstants.MESAJ_BILGI, 2000);
            }
            else
            {
                ModalTitleLbl.Text = "Seçilen Kayıtlar Aktarılacak";
                ModalSubTitleLbl.Text = idList.Length + " Adet satırı kaydetmek için seçtiniz.";
                UyariMesajiLbl.Text = "Lütfen kaydetmeden önce dikkatle inceleyiniz.";
                KaydetNowBtn.Visible = true;
                var openPopup = "OpenModalOnay();";
                UtilityHelper.ScriptCalistir(openPopup);
            }
        }
        protected void KaydetNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                
                SecilenListeyiKaydet();


            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

    }
}