using Model.NBYS;
using Model.Ortak;
using Model.Services.NBYS;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
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
        bool isFinansbankAktarildi = false;
        bool isFinansbankEkstreAktarildi = false;
        bool isGarantiAktarildi = false;
        bool isGarantiEkstreAktarildi = false;
        bool isHalkBankAktarildi = false;
        bool isHalkbank2Aktarildi = false;
        bool isIsbankAktarildi = false;
        bool isIsbankEkstreAktarildi = false;
        bool isYKBEkstreAktarildi = false;
        bool isVakifBankGunlukAktarildi = false;
        bool isVakifBank2Aktarildi = false;
        bool isZiraatAktarildi = false;
        bool isSMSVakifAktarildi = false;
        //bool isZiraatMT940Aktarildi = false;
        bool isZiraatEkstreAktarildi = false;
        bool isTEBAktarildi = false;
        //katilimlar
        bool isVakifKatilimAktarildi = false;
        bool isZiraatKatilimAktarildi = false;
        bool isEDevletAktarildi = false;
        bool isAlbarakaAktarildi = false;
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

                    NextBtn.Visible = isAkbankAktarildi || isAkbankEkstreAktarildi || isFinansbankAktarildi
                        || isFinansbankEkstreAktarildi || isGarantiAktarildi || isGarantiEkstreAktarildi
                        || isHalkBankAktarildi || isHalkbank2Aktarildi || isIsbankAktarildi
                        || isIsbankEkstreAktarildi || isYKBEkstreAktarildi
                        || isVakifBankGunlukAktarildi || isVakifBank2Aktarildi
                        || isVakifKatilimAktarildi || isTEBAktarildi
                        || isZiraatAktarildi || isZiraatEkstreAktarildi || isZiraatKatilimAktarildi
                        || isEDevletAktarildi || isSMSVakifAktarildi || isAlbarakaAktarildi;
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteTrace(ex);
                MessageHelper.PublishMessage("İşlem tamamlanamadı. Lütfen yeniden deneyin.", "alert-danger");
            }

        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            AddDateCalendar(DateTime.Today);
        }

        private void AddDateCalendar(DateTime today)
        {
            DateTime startDate = today.AddDays(-30);
            DateTime endDate = today.AddDays(1);
            HashSet<DateTime> uploadedDates = new HashSet<DateTime>(
                new EkstreAktarmaService().GetProcessDatesWithUploads(startDate, endDate));
            DateTime selectedDate = IslemTarihiTxt.Text.ConvertToDatetime().Date;
            StringBuilder markup = new StringBuilder();
            markup.Append("<style>.bdy-overview-card:not(:first-child){min-height:88px;padding:8px 14px}.bdy-calendar{display:block!important;visibility:visible!important;height:auto!important;max-height:none!important;overflow:visible!important;margin-top:14px;padding:12px;border:1px solid #dce6f1;border-radius:12px;background:#f8fbff}.bdy-calendar-head{display:flex;align-items:center;justify-content:space-between;gap:8px;margin-bottom:9px}.bdy-calendar-title{color:#29405c;font-size:12px;font-weight:800}.bdy-calendar-switch{display:flex;gap:4px}.bdy-calendar-switch button{padding:4px 7px;border:1px solid #cbd7e5;border-radius:6px;background:#fff;color:#52647b;font-size:11px;cursor:pointer}.bdy-calendar-switch button[aria-pressed=true]{border-color:#1768d2;background:#eaf3ff;color:#0d4fa8}.bdy-calendar-grid{display:grid;grid-template-columns:repeat(7,minmax(0,1fr));gap:3px}.bdy-calendar-weekday{padding:3px 0;color:#7b8da3;font-size:10px;font-weight:700;text-align:center}.bdy-calendar-day{display:flex;min-height:28px;align-items:center;justify-content:center;border:1px solid #dce6f1;border-radius:6px;background:#fff;color:#52647b;font-size:11px}.bdy-calendar-day--uploaded{border-color:#a9dbc8;background:#e7f6ef;color:#0f7652;font-weight:800}.bdy-calendar-day--selected{outline:2px solid #1768d2;outline-offset:-2px}.bdy-calendar-day--future{opacity:.45}.bdy-calendar-day--older{display:flex}.bdy-calendar-legend{display:flex;gap:10px;margin-top:8px;color:#64748b;font-size:10px}.bdy-calendar-legend span{display:flex;align-items:center;gap:4px}.bdy-calendar-legend i{display:inline-block;width:8px;height:8px;border-radius:3px;background:#e7f6ef;border:1px solid #a9dbc8}.bdy-calendar-legend .bdy-calendar-empty i{background:#fff;border-color:#dce6f1}</style>");
            markup.Append("<div id=\"bdy-calendar\" class=\"bdy-calendar\" aria-label=\"Son 1 ayın yükleme takvimi\">");
            markup.Append("<div class=\"bdy-calendar-head\"><span class=\"bdy-calendar-title\">Yükleme takvimi</span><span class=\"bdy-calendar-switch\"><button type=\"button\" data-calendar-range=\"15\" aria-pressed=\"false\">15 gün</button><button type=\"button\" data-calendar-range=\"30\" aria-pressed=\"true\">1 ay</button></span></div>");
            string[] weekdays = { "Pt", "Sa", "Ça", "Pe", "Cu", "Ct", "Pz" };
            markup.Append("<div class=\"bdy-calendar-grid\">");
            foreach (string weekday in weekdays) markup.Append("<span class=\"bdy-calendar-weekday\">").Append(weekday).Append("</span>");
            int leadingDays = ((int)startDate.DayOfWeek + 6) % 7;
            for (int i = 0; i < leadingDays; i++) markup.Append("<span></span>");
            string currentPath = Page.Request.Url.AbsolutePath;
            for (DateTime day = startDate; day <= today; day = day.AddDays(1))
            {
                bool isUploaded = uploadedDates.Contains(day.Date);
                bool isSelected = selectedDate == day.Date;
                string classes = "bdy-calendar-day" + (isUploaded ? " bdy-calendar-day--uploaded" : "") + (isSelected ? " bdy-calendar-day--selected" : "") + (day < today.AddDays(-14) ? " bdy-calendar-day--older" : "");
                string url = currentPath + "?IslemTarihi=" + HttpUtility.UrlEncode(day.ToString(ProjeConstants.DATE_TR));
                markup.Append("<a class=\"").Append(classes).Append("\" href=\"").Append(HttpUtility.HtmlAttributeEncode(url)).Append("\" title=\"").Append(isUploaded ? "Bu tarihte yükleme var" : "Bu tarihte yükleme yok").Append("\">").Append(day.Day).Append("</a>");
            }
            markup.Append("</div><div class=\"bdy-calendar-legend\"><span><i></i>Yükleme var</span><span class=\"bdy-calendar-empty\"><i></i>Boş gün</span></div></div>");
            markup.Append("<script>(function(){function moveCalendar(){var c=document.getElementById('bdy-calendar');var card=document.querySelector('.bdy .bdy-overview-card');if(c&&card&&c.parentNode!==card)card.appendChild(c);if(!c)return;var b=c.querySelectorAll('[data-calendar-range]');b.forEach(function(x){x.onclick=function(){var n=x.getAttribute('data-calendar-range');c.querySelectorAll('.bdy-calendar-day').forEach(function(d){d.style.display=n==='15'&&d.classList.contains('bdy-calendar-day--older')?'none':''});b.forEach(function(y){y.setAttribute('aria-pressed',String(y===x));});};});}if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',moveCalendar);else window.setTimeout(moveCalendar,0);}());</script>");
            Control dateCard = IslemTarihiTxt.Parent;
            dateCard.Controls.AddAt(Math.Min(2, dateCard.Controls.Count), new LiteralControl(markup.ToString()));
        }
        private void InitializeLabels()
        {
            AkbankLbl.Text = ProjeConstants.BANKA_AKBANK;
            AkbankEkstreLbl.Text = ProjeConstants.BANKA_AKBANKEKSTRE;
            FinansbankLbl.Text = ProjeConstants.BANKA_FINANSBANK;
            FinansbankEkstreLbl.Text = ProjeConstants.BANKA_FINANSBANKEKSTRE;
            GarantiLbl.Text = ProjeConstants.BANKA_GARANTI;
            GarantiEkstreLbl.Text = ProjeConstants.BANKA_GARANTIEKSTRE;
            HalkbankLbl.Text = ProjeConstants.BANKA_HALKBANK;
            IsbankLbl.Text = ProjeConstants.BANKA_ISBANK;
            IsbankEkstreLbl.Text = ProjeConstants.BANKA_ISBANKEKSTRE;
            YKBEkstreLbl.Text = ProjeConstants.BANKA_YKBEKSTRE;
            EDevletLbl.Text = ProjeConstants.BANKA_EDEVLETBAGIS;
            ZiraatLbl.Text = ProjeConstants.BANKA_ZIRAAT;
            //ZiraatMT940Lbl.Text = ProjeConstants.BANKA_ZIRAATMT940;
            ZiraatEkstreLbl.Text = ProjeConstants.BANKA_ZIRAATEKSTRE;
            //VakifbankLbl.Text = ProjeConstants.BANKA_VAKIF;
            VakifbankGunlukLbl.Text = ProjeConstants.BANKA_VAKIF;// GUNLUK;
            Vakifbank2Lbl.Text = ProjeConstants.BANKA_VAKIF2;

            Halkbank2Lbl.Text = ProjeConstants.BANKA_HALKBANK2;
            VakifKatilimLbl.Text = ProjeConstants.BANKA_VAKIF_KATILIM;
            ZiraatKatilimLbl.Text = ProjeConstants.BANKA_ZIRAAT_KATILIM;
            AlbarakaLbl.Text = ProjeConstants.BANKA_ALBARAKA;


            AkbankOkLbl.Text = string.Empty;
            AkbankEkstreOkLbl.Text = string.Empty;
            FinansbankOkLbl.Text = string.Empty;
            FinansbankEkstreOkLbl.Text = string.Empty;
            GarantiOkLbl.Text = string.Empty;
            GarantiEkstreOkLbl.Text = string.Empty;
            HalkbankOkLbl.Text = string.Empty;
            IsbankOkLbl.Text = string.Empty;
            IsbankEkstreOkLbl.Text = string.Empty;
            YKBEkstreOkLbl.Text = string.Empty;
            EDevletOkLbl.Text = string.Empty;
            ZiraatOkLbl.Text = string.Empty;
            ZiraatEkstreOkLbl.Text = string.Empty;
            //VakifbankOkLbl.Text = string.Empty;
            VakifbankGunlukOkLbl.Text = string.Empty;
            Vakifbank2OkLbl.Text = string.Empty;
            Halkbank2OkLbl.Text = string.Empty;
            VakifKatilimOkLbl.Text = string.Empty;
            ZiraatKatilimOkLbl.Text = string.Empty;
            AlbarakaOkLbl.Text = string.Empty;
        }
        private void checkSavedFiles(DateTime islemTarihi)
        {

            EkstreAktarmaService ekstreAktarma = new EkstreAktarmaService();
            isAkbankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_AKBANK, islemTarihi);
            if (isAkbankAktarildi)
            {
                AkbankFU.Enabled = false;
                if (string.IsNullOrEmpty(AkbankOkLbl.Text)) AkbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isFinansbankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_FINANSBANK, islemTarihi);
            if (isFinansbankAktarildi)
            {
                FinansbankFU.Enabled = false;
                if (string.IsNullOrEmpty(FinansbankOkLbl.Text)) FinansbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isAkbankEkstreAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_AKBANKEKSTRE, islemTarihi);
            if (isAkbankEkstreAktarildi)
            {
                AkbankEkstreFU.Enabled = false;
                if (string.IsNullOrEmpty(AkbankEkstreOkLbl.Text)) AkbankEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isGarantiAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_GARANTI, islemTarihi);
            if (isGarantiAktarildi)
            {
                GarantiFU.Enabled = false;
                if (string.IsNullOrEmpty(GarantiOkLbl.Text)) GarantiOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isGarantiEkstreAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_GARANTIEKSTRE, islemTarihi);
            if (isGarantiEkstreAktarildi)
            {
                GarantiEkstreFU.Enabled = false;
                if (string.IsNullOrEmpty(GarantiEkstreOkLbl.Text)) GarantiEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isTEBAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_TEB, islemTarihi);
            if (isTEBAktarildi)
            {
                TebFU.Enabled = false;
                if (string.IsNullOrEmpty(TebOkLbl.Text)) TebOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isHalkBankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_HALKBANK, islemTarihi);
            if (isHalkBankAktarildi)
            {
                HalkbankFU.Enabled = false;
                if (string.IsNullOrEmpty(HalkbankOkLbl.Text)) HalkbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isIsbankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ISBANK, islemTarihi);
            if (isIsbankAktarildi)
            {
                IsbankFU.Enabled = false;
                if (string.IsNullOrEmpty(IsbankOkLbl.Text)) IsbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            }

            isIsbankEkstreAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ISBANKEKSTRE, islemTarihi);
            if (isIsbankEkstreAktarildi)
            {
                IsbankEkstreFU.Enabled = false;
                if (string.IsNullOrEmpty(IsbankEkstreOkLbl.Text)) IsbankEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isYKBEkstreAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_YKBEKSTRE, islemTarihi);
            if (isYKBEkstreAktarildi)
            {
                YKBEkstreFU.Enabled = false;
                if (string.IsNullOrEmpty(YKBEkstreOkLbl.Text)) YKBEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            //isVakifBankAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_VAKIF, islemTarihi);
            //if (isVakifBankAktarildi)
            //{
            //    VakifbankFU.Enabled = false;
            //    if (string.IsNullOrEmpty(VakifbankOkLbl.Text)) VakifbankOkLbl.Text = "  " + ((char)0x221A).ToString();
            //}
            isVakifBankGunlukAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_VAKIF, islemTarihi);//GUNLUK
            if (isVakifBankGunlukAktarildi)
            {
                VakifbankGunlukFU.Enabled = false;
                if (string.IsNullOrEmpty(VakifbankGunlukOkLbl.Text)) VakifbankGunlukOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isVakifBank2Aktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_VAKIF2, islemTarihi);
            if (isVakifBank2Aktarildi)
            {
                Vakifbank2FU.Enabled = false;
                if (string.IsNullOrEmpty(Vakifbank2OkLbl.Text)) Vakifbank2OkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isVakifKatilimAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_VAKIF_KATILIM, islemTarihi);
            if (isVakifKatilimAktarildi)
            {
                VakifKatilimFU.Enabled = false;
                if (string.IsNullOrEmpty(VakifKatilimOkLbl.Text)) VakifKatilimOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isZiraatAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ZIRAAT, islemTarihi);
            if (isZiraatAktarildi)
            {
                ZiraatFU.Enabled = false;
                if (string.IsNullOrEmpty(ZiraatOkLbl.Text)) ZiraatOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            //isZiraatMT940Aktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ZIRAATMT940, islemTarihi);
            //if (isZiraatMT940Aktarildi)
            //{
            //    ZiraatMT940FU.Enabled = false;
            //    if (string.IsNullOrEmpty(ZiraatMT940OkLbl.Text)) ZiraatMT940OkLbl.Text = "  " + ((char)0x221A).ToString();
            //}
            isZiraatEkstreAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ZIRAATEKSTRE, islemTarihi);
            if (isZiraatEkstreAktarildi)
            {
                ZiraatEkstreFU.Enabled = false;
                if (string.IsNullOrEmpty(ZiraatEkstreOkLbl.Text)) ZiraatEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isZiraatKatilimAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ZIRAAT_KATILIM, islemTarihi);

            if (isZiraatKatilimAktarildi)
            {
                ZiraatKatilimFU.Enabled = false;
                if (string.IsNullOrEmpty(ZiraatKatilimOkLbl.Text)) ZiraatKatilimOkLbl.Text = "  " + ((char)0x221A).ToString();
            }

            isHalkbank2Aktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_HALKBANK2, islemTarihi);

            if (isHalkbank2Aktarildi)
            {
                Halkbank2FU.Enabled = false;
                if (string.IsNullOrEmpty(Halkbank2OkLbl.Text)) Halkbank2OkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isEDevletAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_EDEVLETBAGIS, islemTarihi);
            if (isEDevletAktarildi)
            {
                EDevletFU.Enabled = false;
                if (string.IsNullOrEmpty(EDevletOkLbl.Text)) EDevletOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isSMSVakifAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_SMSVAKIF, islemTarihi);
            if (isSMSVakifAktarildi)
            {
                SMSVakifFU.Enabled = false;
                if (string.IsNullOrEmpty(SMSVakifOkLbl.Text)) SMSVakifOkLbl.Text = "  " + ((char)0x221A).ToString();
            }
            isAlbarakaAktarildi = ekstreAktarma.CheckIsExistByBankaAdiAndIslemTarihi(ProjeConstants.BANKA_ALBARAKA, islemTarihi);
            if (isAlbarakaAktarildi)
            {
                AlbarakaFU.Enabled = false;
                if (string.IsNullOrEmpty(AlbarakaOkLbl.Text)) AlbarakaOkLbl.Text = "  " + ((char)0x221A).ToString();
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
                LogHelper.WriteTrace(ex);
                MessageHelper.PublishMessage("İşlem tamamlanamadı. Lütfen yeniden deneyin.", "alert-danger");
            }

        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            try
            {

                TryUpload(AkbankSave, "Akbank");
                TryUpload(AkbankEkstreSave, "Akbank Ekstre");
                TryUpload(FinansbankSave, "Finansbank");
                TryUpload(FinansbankEkstreSave, "Finansbank Ekstre");
                TryUpload(GarantiSave, "Garanti");
                TryUpload(GarantiEkstreSave, "Garanti Ekstre");
                TryUpload(HalkbankSave, "Halkbank");
                TryUpload(Halkbank2Save, "Halkbank 2");
                TryUpload(IsbankSave, "İş Bankası");
                TryUpload(IsbankEkstreSave, "İş Bankası Ekstre");
                TryUpload(YKBEkstreSave, "YKB Ekstre");
                TryUpload(ZiraatSave, "Ziraat");
                //ZiraatMT940Save();
                TryUpload(ZiraatEkstreSave, "Ziraat Ekstre");
                TryUpload(TEBSave, "TEB");
                TryUpload(ZiraatKatilimSave, "Ziraat Katılım");
                //VakifbankSave();
                TryUpload(VakifbankGunlukSaveTxt, "Vakıfbank Günlük");
                TryUpload(Vakifbank2Save, "Vakıfbank 2");
                TryUpload(VakifKatilimSave, "Vakıf Katılım");
                TryUpload(EDevletSave, "E-Devlet");
                TryUpload(SMSVakifSave, "Vakıf SMS");
                TryUpload(AlbarakaSave, "Albaraka");
                checkSavedFiles(IslemTarihiTxt.Text.ConvertToDatetime());
                NextBtn.Visible = (isAkbankAktarildi || isFinansbankAktarildi || isGarantiAktarildi || isGarantiEkstreAktarildi 
                    || isHalkBankAktarildi || isIsbankAktarildi || isIsbankEkstreAktarildi || isYKBEkstreAktarildi
                    || isVakifBankGunlukAktarildi || isVakifBank2Aktarildi || isTEBAktarildi
                    || isHalkbank2Aktarildi || isVakifKatilimAktarildi || isZiraatAktarildi || isZiraatEkstreAktarildi 
                    || isZiraatKatilimAktarildi || isEDevletAktarildi
                    || isSMSVakifAktarildi || isAlbarakaAktarildi);

            }
            catch (Exception ex)
            {
                LogHelper.WriteTrace(ex);
                MessageHelper.PublishMessage("İşlem tamamlanamadı. Lütfen yeniden deneyin.", "alert-danger");
            }
        }
        private static void TryUpload(Action upload, string bankName)
        {
            try
            {
                upload();
            }
            catch (Exception ex)
            {
                LogHelper.WriteTrace(ex);
                MessageHelper.PublishMessage(System.Web.HttpUtility.HtmlEncode(bankName) +
                    " dosyası yüklenemedi.", "alert-danger");
            }
        }
        private static void PublishImportOutcome(ExceptionHelper result)
        {
            result.PublishImportExceptions();
            if (result.HasFatalException())
                MessageHelper.PublishMessage("Dosya işlenirken hata oluştu; dosyadaki değişiklikler geri alındı.", "alert-danger");
            else if (result.SuccessCount > 0)
            {
                MessageHelper.PublishMessage(string.Format("{0} kayıt eklendi.", result.SuccessCount), "alert-success");
                if (result.AlreadyProcessedCount > 0 || result.SkippedCount > 0)
                    MessageHelper.PublishMessage(string.Format("{0} kayıt önceden işlenmiş, {1} satır atlandı.",
                        result.AlreadyProcessedCount, result.SkippedCount), "alert-warning");
            }
            else if (result.AlreadyProcessedCount > 0)
                MessageHelper.PublishMessage(string.Format("Yeni kayıt eklenmedi. {0} kayıt zaten işlenmiş, {1} satır atlandı.",
                    result.AlreadyProcessedCount, result.SkippedCount), "alert-warning");
            else
                MessageHelper.PublishMessage("Dosyadan uygun kayıt eklenemedi. Dosyanın boş olup olmadığını ve banka şablonunu kontrol edin.", "alert-danger");
        }
        private void AkbankSave()
        {
            if (!isAkbankAktarildi && AkbankFU.HasFile)
            {

                var exceptionHelper = EkstreAktarmaImportService.SaveAkBankFile(AkbankFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, AkbankFU.FileName);
                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    AkbankFU.Enabled = true;
                    AkbankOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    AkbankOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }

                else
                {
                    PublishImportOutcome(exceptionHelper);
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

                var exceptionHelper = EkstreAktarmaImportService.SaveAkBankEkstreFile(AkbankEkstreFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, AkbankEkstreFU.FileName);
                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    AkbankEkstreFU.Enabled = true;
                    AkbankEkstreOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    AkbankEkstreOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }

                else
                {
                    PublishImportOutcome(exceptionHelper);
                    AkbankEkstreFU.Enabled = false;
                    AkbankEkstreOkLbl.ForeColor = System.Drawing.Color.Green;
                    AkbankEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
                }

            }
        }
        private void FinansbankSave()
        {
            if (!isFinansbankAktarildi && FinansbankFU.HasFile)
            {

                var exceptionHelper = EkstreAktarmaImportService.SaveFinansbankFile(FinansbankFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, FinansbankFU.FileName);
                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    FinansbankFU.Enabled = true;
                    FinansbankOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    FinansbankOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }

                else
                {
                    PublishImportOutcome(exceptionHelper);
                    FinansbankFU.Enabled = false;
                    FinansbankOkLbl.ForeColor = System.Drawing.Color.Green;
                    FinansbankOkLbl.Text = "  " + ((char)0x221A).ToString();
                }

            }
        }
        private void FinansbankEkstreSave()
        {
            if (!isFinansbankEkstreAktarildi && FinansbankEkstreFU.HasFile)
            {
                var exceptionHelper = EkstreAktarmaImportService.SaveFinansbankEkstreFile(FinansbankEkstreFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, FinansbankEkstreFU.FileName);

                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    FinansbankEkstreFU.Enabled = true;
                    FinansbankEkstreOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    FinansbankEkstreOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }
                else
                {
                    PublishImportOutcome(exceptionHelper);
                    FinansbankEkstreFU.Enabled = false;
                    FinansbankEkstreOkLbl.ForeColor = System.Drawing.Color.Green;
                    FinansbankEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        private void GarantiSave()
        {
            if (!isGarantiAktarildi && GarantiFU.HasFile)
            {

                var exceptionHelper = EkstreAktarmaImportService.SaveGarantiBankFile(GarantiFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, GarantiFU.FileName);

                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    GarantiFU.Enabled = true;
                    GarantiOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    GarantiOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }

                else
                {
                    PublishImportOutcome(exceptionHelper);
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

                var exceptionHelper = EkstreAktarmaImportService.SaveGarantiEkstreFile(GarantiEkstreFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, GarantiEkstreFU.FileName);

                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    GarantiEkstreFU.Enabled = true;
                    GarantiEkstreOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    GarantiEkstreOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }

                else
                {
                    PublishImportOutcome(exceptionHelper);
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

                var exceptionHelper = EkstreAktarmaImportService.SaveTEBBankFile(TebFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, TebFU.FileName);

                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    TebFU.Enabled = true;
                    TebOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    TebOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }

                else
                {
                    PublishImportOutcome(exceptionHelper);
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
                var exceptionHelper = EkstreAktarmaImportService.SaveHalkbankFile(HalkbankFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, HalkbankFU.FileName);
                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    HalkbankFU.Enabled = true;
                    HalkbankOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    HalkbankOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }

                else
                {
                    PublishImportOutcome(exceptionHelper);
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
                var exceptionHelper = EkstreAktarmaImportService.SaveHalkbank2File(Halkbank2FU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, Halkbank2FU.FileName);
                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    Halkbank2FU.Enabled = true;
                    Halkbank2OkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    Halkbank2OkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }
                else
                {
                    PublishImportOutcome(exceptionHelper);
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
                var exceptionHelper = EkstreAktarmaImportService.SaveIsBankFile(IsbankFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, IsbankFU.FileName);

                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    IsbankFU.Enabled = true;
                    IsbankOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    IsbankOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }
                else
                {
                    PublishImportOutcome(exceptionHelper);
                    IsbankFU.Enabled = false;
                    IsbankOkLbl.ForeColor = System.Drawing.Color.Green;
                    IsbankOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        private void IsbankEkstreSave()
        {
            if (!isIsbankEkstreAktarildi && IsbankEkstreFU.HasFile)
            {
                var exceptionHelper = EkstreAktarmaImportService.SaveIsBankEkstreFile(IsbankEkstreFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, IsbankEkstreFU.FileName);

                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    IsbankEkstreFU.Enabled = true;
                    IsbankEkstreOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    IsbankEkstreOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }
                else
                {
                    PublishImportOutcome(exceptionHelper);
                    IsbankEkstreFU.Enabled = false;
                    IsbankEkstreOkLbl.ForeColor = System.Drawing.Color.Green;
                    IsbankEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        private void YKBEkstreSave()
        {
            if (!isYKBEkstreAktarildi && YKBEkstreFU.HasFile)
            {
                var exceptionHelper = EkstreAktarmaImportService.SaveYKBEkstreFile(YKBEkstreFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, YKBEkstreFU.FileName);

                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    YKBEkstreFU.Enabled = true;
                    YKBEkstreOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    YKBEkstreOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }
                else
                {
                    PublishImportOutcome(exceptionHelper);
                    YKBEkstreFU.Enabled = false;
                    YKBEkstreOkLbl.ForeColor = System.Drawing.Color.Green;
                    YKBEkstreOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }

        private void VakifbankGunlukSaveTxt()
        {
            if (!isVakifBankGunlukAktarildi && VakifbankGunlukFU.HasFile)
            {
                var exceptionHelper = EkstreAktarmaImportService.SaveVakifBankGunlukTextFile(VakifbankGunlukFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, VakifbankGunlukFU.FileName);
                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    VakifbankGunlukFU.Enabled = true;
                    VakifbankGunlukOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    VakifbankGunlukOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }
                else
                {
                    PublishImportOutcome(exceptionHelper);
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
                var exceptionHelper = EkstreAktarmaImportService.SaveVakifBank2File(Vakifbank2FU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, Vakifbank2FU.FileName);
                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    Vakifbank2FU.Enabled = true;
                    Vakifbank2OkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    Vakifbank2OkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }
                else
                {
                    PublishImportOutcome(exceptionHelper);
                    Vakifbank2FU.Enabled = false;
                    Vakifbank2OkLbl.ForeColor = System.Drawing.Color.Green;
                    Vakifbank2OkLbl.Text = "  " + ((char)0x221A).ToString();
                }

            }
        }
        private void VakifKatilimSave()
        {
            if (!isVakifKatilimAktarildi && VakifKatilimFU.HasFile)
            {
                var exceptionHelper = EkstreAktarmaImportService.SaveVakifKatilimFile(VakifKatilimFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, VakifKatilimFU.FileName);
                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    VakifKatilimFU.Enabled = true;
                    VakifKatilimOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    VakifKatilimOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }
                else
                {
                    PublishImportOutcome(exceptionHelper);
                    VakifKatilimFU.Enabled = false;
                    VakifKatilimOkLbl.ForeColor = System.Drawing.Color.Green;
                    VakifKatilimOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        private void ZiraatSave()
        {
            if (!isZiraatAktarildi && ZiraatFU.HasFile)
            {
                var exceptionHelper = EkstreAktarmaImportService.SaveZiraatFile(ZiraatFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, ZiraatFU.FileName);

                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    ZiraatFU.Enabled = true;
                    ZiraatOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    ZiraatOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }
                else
                {
                    PublishImportOutcome(exceptionHelper);
                    ZiraatFU.Enabled = false;
                    ZiraatOkLbl.ForeColor = System.Drawing.Color.Green;
                    ZiraatOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        private void ZiraatEkstreSave()
        {
            if (!isZiraatEkstreAktarildi && ZiraatEkstreFU.HasFile)
            {
                var exceptionHelper = EkstreAktarmaImportService.SaveZiraatEkstreFile(ZiraatEkstreFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, ZiraatEkstreFU.FileName);

                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    ZiraatEkstreFU.Enabled = true;
                    ZiraatEkstreOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    ZiraatEkstreOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }
                else
                {
                    PublishImportOutcome(exceptionHelper);
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
                var exceptionHelper = EkstreAktarmaImportService.SaveZiraatKatilimFile(ZiraatKatilimFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, ZiraatKatilimFU.FileName);
                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    ZiraatKatilimFU.Enabled = true;
                    ZiraatKatilimOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    ZiraatKatilimOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }
                else
                {
                    PublishImportOutcome(exceptionHelper);
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
                var exceptionHelper = EkstreAktarmaImportService.SaveEDevletFile(EDevletFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, EDevletFU.FileName);

                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    EDevletFU.Enabled = true;
                    EDevletOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    EDevletOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }
                else
                {
                    PublishImportOutcome(exceptionHelper);
                    EDevletFU.Enabled = false;
                    EDevletOkLbl.ForeColor = System.Drawing.Color.Green;
                    EDevletOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        private void AlbarakaSave()
        {
            if (!isAlbarakaAktarildi && AlbarakaFU.HasFile)
            {
                var exceptionHelper = EkstreAktarmaImportService.SaveAlbarakaFile(AlbarakaFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, AlbarakaFU.FileName);

                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    AlbarakaFU.Enabled = true;
                    AlbarakaOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    AlbarakaOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }
                else
                {
                    PublishImportOutcome(exceptionHelper);
                    AlbarakaFU.Enabled = false;
                    AlbarakaOkLbl.ForeColor = System.Drawing.Color.Green;
                    AlbarakaOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        private void SMSVakifSave()
        {
            if (!isSMSVakifAktarildi && SMSVakifFU.HasFile)
            {
                var exceptionHelper = EkstreAktarmaImportService.SaveSMSVakifFile(SMSVakifFU.FileContent, IslemTarihiTxt.Text.ConvertToDatetime(), CurrentUserName, SMSVakifFU.FileName);

                if (exceptionHelper.HasFatalException() || exceptionHelper.SuccessCount == 0)
                {
                    SMSVakifFU.Enabled = true;
                    SMSVakifOkLbl.ForeColor = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Red;
                    SMSVakifOkLbl.Text = exceptionHelper.AlreadyProcessedCount > 0 && !exceptionHelper.HasFatalException() ? "!" : "X";
                    PublishImportOutcome(exceptionHelper);
                }
                else
                {
                    PublishImportOutcome(exceptionHelper);
                    SMSVakifFU.Enabled = false;
                    SMSVakifOkLbl.ForeColor = System.Drawing.Color.Green;
                    SMSVakifOkLbl.Text = "  " + ((char)0x221A).ToString();
                }
            }
        }
        protected void NextBtn_Click(object sender, EventArgs e)
        {
            RedirectWithTarih(ProjeConstants.PAGE_EKSTRE_LIST);
        }
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
                LogHelper.WriteTrace(ex);
                MessageHelper.PublishMessage("İşlem tamamlanamadı. Lütfen yeniden deneyin.", "alert-danger");
            }
        }
    }
}
