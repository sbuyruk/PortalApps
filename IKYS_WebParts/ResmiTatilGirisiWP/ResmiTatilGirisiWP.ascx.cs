using Model.IKYS;
using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.ResmiTatilGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class ResmiTatilGirisiWP : WebPart
    {
        private PortalLogContext LoggingContext { get { return PortalLogContext.FromCurrentRequest("IKYS", "ResmiTatilGirisiWP"); } }

        private void ReportError(string operation, Exception error)
        {
            PortalLogContext context = LoggingContext;
            if (error is System.Threading.ThreadAbortException) return;
            PortalTechnicalLog.WriteError(context, operation, error);
            MessageHelper.PublishMessage("İşlem tamamlanamadı. Lütfen tekrar deneyin. Başvuru kodu: " + context.OperationId.ToString("N"), ProjeConstants.MESAJ_HATA);
        }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ResmiTatilGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }

        private string SenderAppQS
        {
            get
            {

                if (ViewState["SenderApp"] == null)
                {
                    if (Page.Request.QueryString["SenderApp"] != null)
                    {
                        ViewState["SenderApp"] = Page.Request.QueryString["SenderApp"];
                    }
                    else
                    {
                        ViewState["SenderApp"] = string.Empty;
                    }
                }
                return ViewState["SenderApp"].ToString();
            }

            set
            {
                ViewState["SenderApp"] = value;
            }
        }
        private string DestinationAppQS
        {
            get
            {

                if (ViewState["DestinationApp"] == null)
                {
                    if (Page.Request.QueryString["DestinationApp"] != null)
                    {
                        ViewState["DestinationApp"] = Page.Request.QueryString["DestinationApp"];
                    }
                    else
                    {
                        ViewState["DestinationApp"] = string.Empty;
                    }
                }
                return ViewState["DestinationApp"].ToString();
            }

            set
            {
                ViewState["DestinationApp"] = value;
            }
        }
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
        private string ResmiTatilIdQS
        {
            get
            {

                if (ViewState["ResmiTatilId"] == null)
                {
                    if (Page.Request.QueryString["ResmiTatilId"] != null)
                    {
                        ViewState["ResmiTatilId"] = Page.Request.QueryString["ResmiTatilId"];
                    }
                    else
                    {
                        ViewState["ResmiTatilId"] = string.Empty;
                    }
                }
                return ViewState["ResmiTatilId"].ToString();
            }

            set
            {
                ViewState["ResmiTatilId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack) new ResmiTatilService().TryRecordWebPartOpened(LoggingContext);
                if (string.IsNullOrEmpty(DestinationAppQS) || (!DestinationAppQS.Equals("RTD")))
                {
                    OpenTatilGiris();
                }
                else if (DestinationAppQS.Equals("RTD"))
                {
                    OpenTatilDuzenle();
                }
                BastarBittarAyarla();
            }
            catch (Exception exception)
            {
                ReportError("ResmiTatil.PageLoad", exception);
            }
        }
        private void OpenTatilDuzenle()
        {
            KaydetBtn.Visible = false;
            GuncelleBtn.Visible = true;
            DeleteBtn.Visible = true;
            if (!Page.IsPostBack)
            {
                AyDDLDoldur(BasAyDDL);
                AyDDLDoldur(BitAyDDL);
                FillBasSaatDDL();
                FillBitSaatDDL();
                FillGecerlilikDDL();
                FillResmiTatilForm();
            }
        }
        private void OpenTatilGiris()
        {
            KaydetBtn.Visible = true;
            GuncelleBtn.Visible = false;
            DeleteBtn.Visible = false;
            if (!Page.IsPostBack)
            {

                FillBasSaatDDL();
                FillBitSaatDDL();
                FillGecerlilikDDL();
                AyDDLDoldur(BasAyDDL);
                AyDDLDoldur(BitAyDDL);
            }
        }
        private void FillResmiTatilForm()
        {

            ResmiTatil resmiTatil = new ResmiTatil();
            resmiTatil = new ResmiTatilService().GetById(ResmiTatilIdQS.ConvertToInt(), LoggingContext);
            if (resmiTatil != null)
            {
                ResmiTatilIdLbl.Text = resmiTatil.Id.ReturnEmptyIfNull().ToString();
                TatilTxt.Text = resmiTatil.Tatil;

                DateTime bastar = resmiTatil.BaslamaTarihi.ConvertToDatetime();
                DateTime bittar = resmiTatil.BitisTarihi.ConvertToDatetime();

                BaslangicTarihiTxt.Text = bastar.ConvertToDatetimeEmptyIfNull();
                BitisTarihiTxt.Text = bittar.ConvertToDatetimeEmptyIfNull();

                string bassaatstr = bastar.ToString("HH:mm");
                string bitsaatstr = bittar.ToString("HH:mm");

                ListItem li0 = BasSaatDDL.Items.FindByValue(bassaatstr);
                BasSaatDDL.SelectedValue = li0.Value;
                ListItem li1 = BitSaatDDL.Items.FindByValue(bitsaatstr.Equals("00:00") ? "23:59" : bitsaatstr);
                BitSaatDDL.SelectedValue = li1.Value;

                IlanTarihiTxt.Text = resmiTatil.IlanTarihi.ConvertToDatetimeEmptyIfNull();
                IptalTarihiTxt.Text = resmiTatil.IptalTarihi.ConvertToDatetimeEmptyIfNull();

                int yil = resmiTatil.Yil;
                if (yil <= 1900)
                {
                    string value = TUM_YILLAR_GECERLI;
                    ListItem li = GecerlilikDDL.Items.FindByValue(value);
                    GecerlilikDDL.SelectedValue = li.Value;
                    string bittarYYYY = bittar.Day + "." + bittar.Month + ".YYYY";
                    GunTxt.Text = bastar.Day.ToString();
                    UtilityHelper.SetDDLValue(BasAyDDL,bastar.Month.ToString());
                    BitGunTxt.Text = bittar.Day.ToString();
                    UtilityHelper.SetDDLValue(BitAyDDL, bittar.Month.ToString());
                }
                else
                {
                    string value = GIRILEN_YIL_GECERLI;
                    ListItem li = GecerlilikDDL.Items.FindByValue(value);
                    GecerlilikDDL.SelectedValue = li.Value;
                }

            }
            else
            {
                MessageHelper.PublishMessage("Resmi Tatil Açilamadi", ProjeConstants.MESAJ_BILGI);
            }
        }
        private bool YeniTatiliKaydet()
        {
            bool isSaved = false;
            ResmiTatil resmiTatil = new ResmiTatil();
            string gecerlilik = GecerlilikDDL.SelectedItem.Value;
            DateTime bastar = BaslangicTarihiTxt.Text.ConvertToDatetime();
            DateTime bittar = BitisTarihiTxt.Text.ConvertToDatetime();
            TimeSpan basSaatTS = new TimeSpan(0, 0, 0);
            string basSaatStr = BasSaatDDL.SelectedItem.Value;
            if (basSaatStr.Equals("13:00"))
            {
                basSaatTS = new TimeSpan(13, 0, 0);
            }
            bastar = bastar + basSaatTS;

            TimeSpan bitSaatTS = new TimeSpan(23, 59, 0);
            string bitSaatStr = BasSaatDDL.SelectedItem.Value;
            if (bitSaatStr.Equals("13:00"))
            {
                bitSaatTS = new TimeSpan(13, 0, 0);
            }
            bittar = bittar + bitSaatTS;

            int yil = bastar.Year;
            resmiTatil.Yil = yil;
            if (gecerlilik.Equals(TUM_YILLAR_GECERLI))//gecerli yil
            {
                yil = 1800;
                bastar = new DateTime(yil, BasAyDDL.SelectedItem.Value.ConvertToInt(), GunTxt.Text.ConvertToInt());
                bittar = new DateTime(yil, BitAyDDL.SelectedItem.Value.ConvertToInt(), BitGunTxt.Text.ConvertToInt());
                bastar = bastar + basSaatTS;
                bittar = bittar + bitSaatTS;
                yil = 1800;
                bastar = new DateTime(yil, bastar.Month, bastar.Day) + basSaatTS;
                bittar = new DateTime(yil, bittar.Month, bittar.Day) + bitSaatTS;
                resmiTatil.Yil = 0;
            }

            resmiTatil.Gun = bastar.Day;
            resmiTatil.Ay = bastar.Month;

            resmiTatil.BaslamaTarihi = bastar;
            resmiTatil.BitisTarihi = bittar;

            resmiTatil.Olusturan = CurrentUserName;
            resmiTatil.Tatil = TatilTxt.Text;

            resmiTatil.IlanTarihi = IlanTarihiTxt.Text.ConvertToDatetime();
            resmiTatil.IptalTarihi = IptalTarihiTxt.Text.ConvertToDatetime();
            int tatilId = new ResmiTatilService().Save(resmiTatil, LoggingContext);
            isSaved = tatilId > 0 ? true : false;
            return isSaved;
        }
        private bool ResmiTatiliGüncelle()
        {
            bool isSaved = false;
            ResmiTatil resmiTatil = new ResmiTatil();
            resmiTatil = new ResmiTatilService().GetById(ResmiTatilIdQS.ConvertToInt(), LoggingContext);
            if (resmiTatil != null)
            {
                string gecerlilik = GecerlilikDDL.SelectedItem.Value;
                DateTime bastar = BaslangicTarihiTxt.Text.ConvertToDatetime();
                DateTime bittar = BitisTarihiTxt.Text.ConvertToDatetime();
                TimeSpan basSaatTS = new TimeSpan(0, 0, 0);
                string basSaatStr = BasSaatDDL.SelectedItem.Value;
                if (basSaatStr.Equals("13:00"))
                {
                    basSaatTS = new TimeSpan(13, 0, 0);
                }
                bastar = bastar + basSaatTS;

                TimeSpan bitSaatTS = new TimeSpan(23, 59, 0);
                string bitSaatStr = BitSaatDDL.SelectedItem.Value;
                if (bitSaatStr.Equals("13:00"))
                {
                    bitSaatTS = new TimeSpan(13, 0, 0);
                }
                bittar = bittar + bitSaatTS;

                int yil = bastar.Year;
                resmiTatil.Yil = yil;
                if (gecerlilik.Equals(TUM_YILLAR_GECERLI))//gecerli yil
                {
                    yil = 1800;
                    bastar =new DateTime(yil, BasAyDDL.SelectedItem.Value.ConvertToInt(), GunTxt.Text.ConvertToInt());
                    bittar = new DateTime(yil, BitAyDDL.SelectedItem.Value.ConvertToInt(), BitGunTxt.Text.ConvertToInt());
                    bastar = bastar + basSaatTS;
                    bittar = bittar + bitSaatTS;

                    bastar = new DateTime(yil, bastar.Month, bastar.Day) + basSaatTS;
                    bittar = new DateTime(yil, bittar.Month, bittar.Day) + bitSaatTS;
                    resmiTatil.Yil = 0;
                }

                resmiTatil.Gun = bastar.Day;
                resmiTatil.Ay = bastar.Month;

                resmiTatil.BaslamaTarihi = bastar;
                resmiTatil.BitisTarihi = bittar;

                resmiTatil.Degistiren = CurrentUserName;
                resmiTatil.Tatil = TatilTxt.Text;

                resmiTatil.IlanTarihi = IlanTarihiTxt.Text.ConvertToDatetime();
                resmiTatil.IptalTarihi = IptalTarihiTxt.Text.ConvertToDatetime();
                isSaved = new ResmiTatilService().Update(resmiTatil, LoggingContext);
            }

            return isSaved;
        }
        private bool ResmiTatiliSil(ResmiTatil resmiTatil)
        {
            return resmiTatil != null && new ResmiTatilService().Delete(resmiTatil, LoggingContext);
        }
        private string KontrolHatasi()
        {
            if (string.IsNullOrWhiteSpace(TatilTxt.Text)) return "Tatil adını giriniz.";
            if (GecerlilikDDL.SelectedItem == null) return "Geçerlilik seçiniz.";

            if (GecerlilikDDL.SelectedItem.Value.Equals(TUM_YILLAR_GECERLI))
            {
                int basGun, bitGun;
                if (BasAyDDL.SelectedItem == null || !int.TryParse(GunTxt.Text, out basGun))
                    return "Başlangıç Tarihi Giriniz.";
                if (BitAyDDL.SelectedItem == null || !int.TryParse(BitGunTxt.Text, out bitGun))
                    return "Bitiş Tarihi Giriniz";

                int basAy = BasAyDDL.SelectedItem.Value.ConvertToInt();
                int bitAy = BitAyDDL.SelectedItem.Value.ConvertToInt();
                if (!GecerliGunMu(1800, basAy, basGun)) return "Geçerli bir başlangıç tarihi giriniz.";
                if (!GecerliGunMu(1800, bitAy, bitGun)) return "Geçerli bir bitiş tarihi giriniz.";
                return null;
            }

            if (BaslangicTarihiTxt.Text.ConvertToDatetime() == DateTime.MinValue)
                return "Başlangıç Tarihi Giriniz.";
            if (BitisTarihiTxt.Text.ConvertToDatetime() == DateTime.MinValue)
                return "Bitiş Tarihi Giriniz";
            return null;
        }

        private static bool GecerliGunMu(int yil, int ay, int gun)
        {
            return ay >= 1 && ay <= 12 && gun >= 1 && gun <= DateTime.DaysInMonth(yil, ay);
        }
        private void FillBasSaatDDL()
        {
            BasSaatDDL.Items.Clear();
            ListItem li1 = new ListItem("00:00", "00:00");
            BasSaatDDL.Items.Add(li1);
            ListItem li2 = new ListItem("13:00", "13:00");
            BasSaatDDL.Items.Add(li2);
        }
        private void FillBitSaatDDL()
        {
            BitSaatDDL.Items.Clear();
            ListItem li2 = new ListItem("23:59", "23:59");
            BitSaatDDL.Items.Add(li2);
            ListItem li1 = new ListItem("13:00", "13:00");
            BitSaatDDL.Items.Add(li1);

        }
        private const string GIRILEN_YIL_GECERLI = "0";
        private const string TUM_YILLAR_GECERLI = "1";
        private void FillGecerlilikDDL()
        {
            GecerlilikDDL.Items.Clear();
            ListItem li0 = new ListItem("Sadece girilen yil için geçerli", GIRILEN_YIL_GECERLI);
            GecerlilikDDL.Items.Add(li0);
            ListItem li1 = new ListItem("Tüm yillar için geçerli", TUM_YILLAR_GECERLI);
            GecerlilikDDL.Items.Add(li1);
        }
        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            string kontrolHatasi = KontrolHatasi();
            if (kontrolHatasi == null)
            {
                SilLbl.Text = "Lütfen Dikkat";
                SilmeMesajiLbl.Text = "Tatil kaydini degistirdiginiz takdirde, söz konusu tarihlerde kullanilan izinlerin sürelerinde tutarsizlik olusabilir."
                   + System.Environment.NewLine + "Yine de kaydetmek istiyor musunuz?";
                DeleteNowBtn.Visible = false;
                GuncelleNowBtn.Visible = true;
                var openPopup = "OpenModal();";
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
            }
            else
            {
                MessageHelper.PublishMessage(kontrolHatasi, ProjeConstants.MESAJ_HATA, 2000);
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/";

            newUrl += ProjeConstants.PAGE_RESMITATIL_LIST;

            Page.Response.Redirect(newUrl);
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string kontrolHatasi = KontrolHatasi();
                if (kontrolHatasi == null)
                {
                    bool isSaved = YeniTatiliKaydet();
                    if (isSaved)
                    {
                        MessageHelper.PublishMessage("Tatil Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                        //RedirectToPage(ProjeConstants.PAGE_RESMITATIL_LIST + "?Mesaj=true");
                    }
                }
                else
                {
                    MessageHelper.PublishMessage(kontrolHatasi, ProjeConstants.MESAJ_HATA, 2000);
                }


            }
            catch (Exception exception)
            {
                ReportError("ResmiTatil.Save", exception);
            }
        }
        private void AyDDLDoldur(DropDownList ddl)
        {
            ddl.Items.Add(new ListItem("Ocak", "1"));
            ddl.Items.Add(new ListItem("Şubat", "2"));
            ddl.Items.Add(new ListItem("Mart", "3"));
            ddl.Items.Add(new ListItem("Nisan", "4"));
            ddl.Items.Add(new ListItem("Mayıs", "5"));
            ddl.Items.Add(new ListItem("Haziran", "6"));
            ddl.Items.Add(new ListItem("Temmuz", "7"));
            ddl.Items.Add(new ListItem("Ağustos", "8"));
            ddl.Items.Add(new ListItem("Eylül", "9"));
            ddl.Items.Add(new ListItem("Ekim", "10"));
            ddl.Items.Add(new ListItem("Kasım", "11"));
            ddl.Items.Add(new ListItem("Aralık", "12"));

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
                ReportError("ResmiTatil.Redirect", ex);
            }
        }
        protected void DeleteBtn_Click(object sender, EventArgs e)
        {
            SilLbl.Text = "Lütfen Dikkat: Tatil Kaydi Silinecek";
            SilmeMesajiLbl.Text = "Tatil kaydini sildiginiz takdirde, söz konusu tarihlerde kullanilan izin sürelerinde tutarsizliklar olusabilir."
                + System.Environment.NewLine + "Yine de bu tatil kaydini silmek musunuz?";
            DeleteNowBtn.Visible = true;
            GuncelleNowBtn.Visible = false;
            var openPopup = "OpenModal();";
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
        }

        protected void DeleteNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ResmiTatil resmiTatil = new ResmiTatil();
                resmiTatil = new ResmiTatilService().GetById(ResmiTatilIdQS.ConvertToInt(), LoggingContext);
                if (resmiTatil != null)
                {
                    bool isDeleted = ResmiTatiliSil(resmiTatil);
                    if (isDeleted)
                    {
                        MessageHelper.PublishMessage("Tatil Kaydi Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                        RedirectToPage(ProjeConstants.PAGE_RESMITATIL_LIST + "?Mesaj=true");
                    }
                }
            }
            catch (Exception exception)
            {
                ReportError("ResmiTatil.Delete", exception);
            }
        }

        protected void GuncelleNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool isSaved = ResmiTatiliGüncelle();

                if (isSaved)
                {
                    UtilityHelper.ScriptCalistir("CloseModal();");
                    MessageHelper.PublishMessage("Tatil Bilgileri Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                }


            }
            catch (Exception exception)
            {
                ReportError("ResmiTatil.Update", exception);
            }
        }
        protected void GecerlilikDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            BastarBittarAyarla();

        }

        private void BastarBittarAyarla()
        {
            if (GecerlilikDDL.SelectedItem.Value.Equals(GIRILEN_YIL_GECERLI)) //yanlizca girilen yil
            {
                GirilenYilGecerliDiv.Attributes["style"] = "display:block";
                GirilenYilGecerliBitTarDiv.Attributes["style"] = "display:block";

                TumYillarGecerliDiv.Attributes["style"] = "display:none";
                TumYillarGecerliBitTarDiv.Attributes["style"] = "display:none";
            }
            else
            {
                GirilenYilGecerliDiv.Attributes["style"] = "display:none";
                GirilenYilGecerliBitTarDiv.Attributes["style"] = "display:none";

                TumYillarGecerliDiv.Attributes["style"] = "display:block";
                TumYillarGecerliBitTarDiv.Attributes["style"] = "display:block";
            }
        }

        protected void ResmiTatilListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_RESMITATIL_LIST + "?SecilenId="+ ResmiTatilIdQS);
        }
    }
}
