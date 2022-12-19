using Model.Ortak;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.ResmiTatilGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class ResmiTatilGirisiWP : WebPart
    {
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
                ExceptionHelper exHelper = new ExceptionHelper(exception);
                exHelper.PublishException();
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
            resmiTatil = resmiTatil.Select<ResmiTatil>(ResmiTatilIdQS.ConvertToInt());
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
                MessageHelper.PublishMessage("Resmi Tatil Açılamadı", ProjeConstants.MESAJ_BILGI);
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
            if (gecerlilik.Equals(TUM_YILLAR_GECERLI))//gecerli yıl
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
            int tatilId = resmiTatil.Save();
            isSaved = tatilId > 0 ? true : false;
            return isSaved;
        }
        private bool ResmiTatiliGüncelle()
        {
            bool isSaved = false;
            ResmiTatil resmiTatil = new ResmiTatil();
            resmiTatil = resmiTatil.Select<ResmiTatil>(ResmiTatilIdQS.ConvertToInt());
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
                if (gecerlilik.Equals(TUM_YILLAR_GECERLI))//gecerli yıl
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
                isSaved = resmiTatil.Update();
            }

            return isSaved;
        }
        private bool ResmiTatiliSil(ResmiTatil resmiTatil)
        {
            bool isSaved = false;
            try
            {
                if (resmiTatil != null)
                {
                    isSaved = resmiTatil.Delete();

                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

            return isSaved;
        }
        private bool KontrolIslemleri()
        {
            string bastarih = GunTxt.Text + "." + BasAyDDL.SelectedItem.Value.ConvertToInt() + "." + 1800;
            string bittarih = BitGunTxt.Text + "." + BitAyDDL.SelectedItem.Value.ConvertToInt() + "." + 1800;
            
            string bastar = GecerlilikDDL.SelectedItem.Value.Equals(TUM_YILLAR_GECERLI) ?
                bastarih : BaslangicTarihiTxt.Text;

            string bittar = GecerlilikDDL.SelectedItem.Value.Equals(TUM_YILLAR_GECERLI) ?
                bittarih : BitisTarihiTxt.Text;

            bool isOK = false;
            isOK = (!string.IsNullOrEmpty(TatilTxt.Text.ReturnEmptyIfNull().ToString()) &&
                !string.IsNullOrEmpty(bastar.ConvertToDatetimeEmptyIfNull()) &&
                !string.IsNullOrEmpty(bittar.ConvertToDatetimeEmptyIfNull()));
            return isOK;
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
            ListItem li0 = new ListItem("Sadece girilen yıl için geçerli", GIRILEN_YIL_GECERLI);
            GecerlilikDDL.Items.Add(li0);
            ListItem li1 = new ListItem("Tüm yıllar için geçerli", TUM_YILLAR_GECERLI);
            GecerlilikDDL.Items.Add(li1);
        }
        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            bool isValid = KontrolIslemleri();
            if (isValid)
            {
                SilLbl.Text = "Lütfen Dikkat";
                SilmeMesajiLbl.Text = "Tatil kaydını değiştirdiğiniz takdirde, söz konusu tarihlerde kullanılan izinlerin sürelerinde tutarsızlık oluşabilir."
                   + System.Environment.NewLine + "Yine de kaydetmek istiyor musunuz?";
                DeleteNowBtn.Visible = false;
                GuncelleNowBtn.Visible = true;
                var openPopup = "OpenModal();";
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
            }
            else
            {
                MessageHelper.PublishMessage("Kaydetmeden önce Başlama/Bitiş Tarihi ve Tatil alanlarını doldurunuz.",ProjeConstants.MESAJ_HATA,2000);
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
                bool isValid = KontrolIslemleri();
                if (isValid)
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
                    MessageHelper.PublishMessage("Kaydetmeden önce Başlama/Bitiş Tarihi ve Tatil alanlarını doldurunuz.", ProjeConstants.MESAJ_HATA, 2000);
                }


            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Tatil Bilgileri kaydedilemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
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
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void DeleteBtn_Click(object sender, EventArgs e)
        {
            SilLbl.Text = "Lütfen Dikkat: Tatil Kaydı Silinecek";
            SilmeMesajiLbl.Text = "Tatil kaydını sildiğiniz takdirde, söz konusu tarihlerde kullanılan izin sürelerinde tutarsızlıklar oluşabilir."
                + System.Environment.NewLine + "Yine de bu tatil kaydını silmek musunuz?";
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
                resmiTatil = resmiTatil.Select<ResmiTatil>(ResmiTatilIdQS.ConvertToInt());
                if (resmiTatil != null)
                {
                    bool isDeleted = ResmiTatiliSil(resmiTatil);
                    if (isDeleted)
                    {
                        MessageHelper.PublishMessage("Tatil Kaydı Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                        RedirectToPage(ProjeConstants.PAGE_RESMITATIL_LIST + "?Mesaj=true");
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Tatil Kaydı Silinemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
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
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Tatil Bilgileri Güncellenemdi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }
        protected void GecerlilikDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            BastarBittarAyarla();

        }

        private void BastarBittarAyarla()
        {
            if (GecerlilikDDL.SelectedItem.Value.Equals(GIRILEN_YIL_GECERLI)) //yanlızca girilen yıl
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
