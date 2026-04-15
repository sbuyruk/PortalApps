using Model.TBYS;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.TasinmazArmaganWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazArmaganWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazArmaganWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SecilenDurumQS
        {
            get
            {

                if (ViewState["SecilenDurum"] == null)
                {
                    if (Page.Request.QueryString["SecilenDurum"] != null)
                    {
                        ViewState["SecilenDurum"] = Page.Request.QueryString["SecilenDurum"];
                    }
                    else
                    {
                        ViewState["SecilenDurum"] = string.Empty;
                    }
                }
                return ViewState["SecilenDurum"].ToString();
            }

            set
            {
                ViewState["SecilenDurum"] = value;
            }
        }
        private string BagisIdQS
        {
            get
            {

                if (ViewState["BagisId"] == null)
                {
                    if (Page.Request.QueryString["BagisId"] != null)
                    {
                        ViewState["BagisId"] = Page.Request.QueryString["BagisId"];
                    }
                    else
                    {
                        ViewState["BagisId"] = string.Empty;
                    }
                }
                return ViewState["BagisId"].ToString();
            }

            set
            {
                ViewState["BagisId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DurumDoldur();
                FormuDoldur();
            }

        }
        private void FormuDoldur()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            Bagis bagis = new Bagis();
            bagis = bagis.Select(BagisIdQS.ConvertToInt());
            if (bagis != null)
            {
                ArmaganIdTxt.Text = string.IsNullOrEmpty(bagis.ArmaganId) ? "" : bagis.ArmaganId.ToString();
                AciklamaTxt.Text = bagis.ArmaganAciklama;
                ArmaganTarihiTxt.Value = bagis.ArmaganTarihi.ConvertToDatetimeEmptyIfNull();
                BagisTarihiTxt.Text = bagis.BagisTarihi.ConvertToDatetimeEmptyIfNull();
                DurumDDL.SelectedValue = bagis.ArmaganDurumu;
                TasinmazBagisci bagisci = new TasinmazBagisci();
                bagisci = bagisci.Select<TasinmazBagisci>(bagis.BagisciId);
                if (bagisci != null)
                {
                    AdiSoyadiTxt.Text = bagisci.Adi + " " + bagisci.Soyadi;
                    TelefonTxt.Text = string.IsNullOrEmpty(bagisci.Telefon1) ? "" : bagisci.Telefon1 + " " + bagisci.Telefon2;
                    BagisciAdresiTxt.Text = bagisci.Adres + " " + bagisci.Ilcesi + "/" + bagisci.Ili;
                    IliTxt.Text = bagisci.Ili;

                }
                Tasinmaz tasinmaz = new Tasinmaz();
                tasinmaz = tasinmaz.Select<Tasinmaz>(bagis.TasinmazId);
                if (tasinmaz != null)
                {
                    TasinmazCinsiTxt.Text = tasinmaz.Cinsi;
                    TahminiRayicTxt.Text = tasinmaz.TahminiRayicDegeri.ToString("N", culturInfo);
                    TasinmazAdresiTxt.Text = tasinmaz.Adres + " " + tasinmaz.Ilcesi + "/" + tasinmaz.Ili;
                    ArmaganIdTxt.Text = bagis.BagisTarihi.Year + "-" + tasinmaz.Id;
                }

            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_HOME);
        }
        private void DurumDoldur()
        {
            DurumDDL.Items.Add(ProjeConstants.DURUM_BOS);
            DurumDDL.Items.Add(ProjeConstants.DURUM_BELGE_ISTEMIYOR);
            DurumDDL.Items.Add(ProjeConstants.DURUM_GONDERILMEDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_GONDERILDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_ULASILAMADI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_KONTROLEDILDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_ERKENGONDERILDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_ERTELENDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_PARAIADE);
            DurumDDL.Items.Add(ProjeConstants.DURUM_DAHAONCEIADE);

            //acilista durumu querystring ile gelene eşitle
            ListItem DurumItem = new ListItem();
            if (!string.IsNullOrEmpty(SecilenDurumQS))
                DurumItem = DurumDDL.Items.FindByValue(SecilenDurumQS);

            if (DurumItem != null)
                DurumDDL.SelectedValue = DurumItem.Value;
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
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            Bagis bagis = new Bagis();
            bagis = bagis.Select(BagisIdQS.ConvertToInt());
            if (bagis != null)
            {
                bagis.ArmaganId = ArmaganIdTxt.Text;
                bagis.ArmaganDurumu = DurumDDL.SelectedItem.Value;
                bagis.ArmaganTarihi = ArmaganTarihiTxt.Value.ConvertToDatetime();
                bagis.ArmaganAciklama = AciklamaTxt.Text;
                if (bagis.Update())
                {
                    RedirectToPage(ProjeConstants.PAGE_TASINMAZARMAGAN_LIST + "?ArmaganId=" + bagis.ArmaganId);
                }
            }

        }
        protected void BelgeBasimiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.RAPOR_TASINMAZARMAGANBELGESI_URL + "?BagisId=" + BagisIdQS);
        }
        protected void ArmaganListBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_TASINMAZARMAGAN_LIST + "?ArmaganId=" + ArmaganIdTxt.Text);
        }
    }
}
