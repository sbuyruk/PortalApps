using Model.Services.NBYS;
using NBYS_WebParts.Logging;
using Utility.Logging;
using Model.Services.TBYS;
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
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "TasinmazArmaganWP"); }
        }

        private void ExecuteAction(string operation, Action action)
        {
            NbysReportFeedback.Execute(LoggingContext, operation, action);
        }

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
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
                if (!Page.IsPostBack)
                {
                    DurumDoldur();
                    FormuDoldur();
                }
            });
        }
        private void FormuDoldur()
        {
            int selectedId;
            if (!int.TryParse(BagisIdQS, out selectedId) || selectedId <= 0) throw new NbysReportInputException("Geçerli bir bağış kaydı seçin.");
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            Bagis bagis = new Bagis();
            bagis = new Model.Services.TBYS.BagisService(LoggingContext).GetById(BagisIdQS.ConvertToInt());
            if (bagis != null)
            {
                ArmaganIdTxt.Text = string.IsNullOrEmpty(bagis.ArmaganId) ? "" : bagis.ArmaganId.ToString();
                AciklamaTxt.Text = bagis.ArmaganAciklama;
                ArmaganTarihiTxt.Value = bagis.ArmaganTarihi.ConvertToDatetimeEmptyIfNull();
                BagisTarihiTxt.Text = bagis.BagisTarihi.ConvertToDatetimeEmptyIfNull();
                if (!string.IsNullOrEmpty(bagis.ArmaganDurumu))
                {
                    if (DurumDDL.Items.FindByValue(bagis.ArmaganDurumu) == null)
                        DurumDDL.Items.Add(new ListItem(bagis.ArmaganDurumu, bagis.ArmaganDurumu));
                    DurumDDL.SelectedValue = bagis.ArmaganDurumu;
                }
                TasinmazBagisci bagisci = new TasinmazBagisci();
                bagisci = new Model.Services.TBYS.TasinmazBagisciService(LoggingContext).GetById(bagis.BagisciId);
                if (bagisci != null)
                {
                    AdiSoyadiTxt.Text = bagisci.Adi + " " + bagisci.Soyadi;
                    TelefonTxt.Text = string.IsNullOrEmpty(bagisci.Telefon1) ? "" : bagisci.Telefon1 + " " + bagisci.Telefon2;
                    BagisciAdresiTxt.Text = bagisci.Adres + " " + bagisci.Ilcesi + "/" + bagisci.Ili;
                    IliTxt.Text = bagisci.Ili;

                }
                Tasinmaz tasinmaz = new Tasinmaz();
                tasinmaz = new TasinmazService(LoggingContext).GetById(bagis.TasinmazId);
                if (tasinmaz != null)
                {
                    TasinmazCinsiTxt.Text = tasinmaz.Cinsi;
                    TahminiRayicTxt.Text = tasinmaz.TahminiRayicDegeri.ToString("N", culturInfo);
                    TasinmazAdresiTxt.Text = tasinmaz.Adres + " " + tasinmaz.Ilcesi + "/" + tasinmaz.Ili;
                    if (string.IsNullOrEmpty(bagis.ArmaganId)) ArmaganIdTxt.Text = bagis.BagisTarihi.Year + "-" + tasinmaz.Id;
                }

            }
            else throw new NbysReportInputException("Bağış kaydı bulunamadı. Listeyi yenileyip tekrar seçin.");
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_HOME);
            });
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

            //acilista durumu querystring ile gelene esitle
            ListItem DurumItem = null;
            if (!string.IsNullOrEmpty(SecilenDurumQS))
                DurumItem = DurumDDL.Items.FindByValue(SecilenDurumQS);

            if (DurumItem != null)
                DurumDDL.SelectedValue = DurumItem.Value;
        }
        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(System.UriPartial.Path);
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl, false);
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("KaydetBtn_Click", () =>
            {
                int id;
                DateTime date;
                if (!int.TryParse(BagisIdQS, out id) || id <= 0) throw new NbysReportInputException("Geçerli bir bağış kaydı seçin.");
                if (!DateTime.TryParse(ArmaganTarihiTxt.Value, new CultureInfo(ProjeConstants.CULTUREINFO), DateTimeStyles.None, out date))
                    throw new NbysReportInputException("Geçerli bir armağan tarihi girin.");
                NbysReportWriteResult result = new NbysReportWriteService(LoggingContext).SetPropertyGift(id,
                    ArmaganIdTxt.Text.Trim(), DurumDDL.SelectedValue, date, AciklamaTxt.Text);
                NbysReportFeedback.Write(LoggingContext, result, "Taşınmaz armağan bilgileri kaydedildi.", FormuDoldur);
            });
        }
        protected void BelgeBasimiBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("BelgeBasimiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.RAPOR_TASINMAZARMAGANBELGESI_URL + "?BagisId=" + System.Web.HttpUtility.UrlEncode(BagisIdQS));
            });
        }
        protected void ArmaganListBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ArmaganListBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_TASINMAZARMAGAN_LIST + "?ArmaganId=" + System.Web.HttpUtility.UrlEncode(ArmaganIdTxt.Text));
            });
        }
    }
}
