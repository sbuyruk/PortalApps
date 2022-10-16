using Model.MYS;
using Model.Ortak;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using TSKGV_Utility.HelperClasses;
using TSKGV_Utility.ProjeGlobal;

namespace MYS_WebParts.MalzemeKategoriGirisWP
{
    [ToolboxItemAttribute(false)]
    public partial class MalzemeKategoriGirisWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public MalzemeKategoriGirisWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string MalzemeKategoriIdQS
        {
            get
            {

                if (ViewState["MalzemeKategoriId"] == null)
                {
                    if (Page.Request.QueryString["MalzemeKategoriId"] != null)
                    {
                        ViewState["MalzemeKategoriId"] = Page.Request.QueryString["MalzemeKategoriId"];
                    }
                    else
                    {
                        ViewState["MalzemeKategoriId"] = string.Empty;
                    }
                }
                return ViewState["MalzemeKategoriId"].ToString();
            }

            set
            {
                ViewState["MalzemeKategoriId"] = value;
            }
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
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUser();
                }
                return ViewState["CurrentUserName"].ToString();
            }

            set
            {
                ViewState["CurrentUserName"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(DestinationAppQS) || (!DestinationAppQS.Equals("MKD")))
                {
                    OpenGiris();
                }
                else if (DestinationAppQS.Equals("MKD"))
                {
                    OpenDuzenle();
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exHelper = new ExceptionHelper(exception);
                exHelper.PublishException();
            }
        }
        private void OpenDuzenle()
        {
            BackBtn.Visible = true;
            SaveBtn.Visible = false;
            UpdateBtn.Visible = true;
            DeleteBtn.Visible = true;
            TitleLbl.CssClass = "col-form-primary  btn-outline-primary mb-1";
            TitleLbl.Text = "Malzeme Kategorisi Düzenleme";
            MalzemeKategori kategori = new MalzemeKategori();
            kategori = kategori.Select<MalzemeKategori>(MalzemeKategoriIdQS.ConvertToInt());
            if (!Page.IsPostBack)
            {
                if (kategori != null)
                {
                    IdLbl.Text = kategori.Id.ToString();
                    KategoriAdiLbl.Text = kategori.Adi;
                    AdiTxt.Text = kategori.Adi;
                    KisaAdiTxt.Text = kategori.KisaAdi;
                }
            }
        }
        private void OpenGiris()
        {
            BackBtn.Visible = false;
            SaveBtn.Visible = true;
            UpdateBtn.Visible = false;
            DeleteBtn.Visible = false;

        }
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            string newUrl = string.Empty;
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            if (SenderAppQS.Equals("PL"))
                newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_PERSONEL_LIST;
            if (SenderAppQS.Equals("PD"))
                newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_PERSONEL_EDIT + "?PersonelId=" + MalzemeKategoriIdQS + "&DestinationApp=PerD";
            Page.Response.Redirect(newUrl);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }

        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                MalzemeKategori mk = new MalzemeKategori();
                mk = mk.Select<MalzemeKategori>(MalzemeKategoriIdQS.ConvertToInt());
                if (mk != null)
                {
                    mk.Adi = AdiTxt.Text;
                    mk.KisaAdi = KisaAdiTxt.Text;
                    mk.Degistiren = CurrentUserName;
                    mk.Update();
                    MessageHelper.PublishMessage("Kategori Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                    RedirectToPage(ProjeConstants.PAGE_MALZEMEKATEGORI_LIST + "?Mesaj=true");
                }

            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();
            }
        }

        protected void DeleteBtn_Click(object sender, EventArgs e)
        {
            try
            {
                MalzemeKategori mk = new MalzemeKategori();
                mk = mk.Select<MalzemeKategori>(MalzemeKategoriIdQS.ConvertToInt());
                if (mk != null)
                {
                    mk.Delete();
                    MessageHelper.PublishMessage("Kategori Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                    RedirectToPage(ProjeConstants.PAGE_MALZEMEKATEGORI_LIST + "?Mesaj=true");
                }

            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();
            }
        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                int yeniMKId = 1;
                MalzemeKategori mk = new MalzemeKategori();
                mk = mk.SelectMax();
                if (mk != null)
                {
                    yeniMKId = ++mk.Id;
                }
                MalzemeKategori yeniMK = new MalzemeKategori();
                yeniMK.Id = yeniMKId;
                yeniMK.Adi = AdiTxt.Text;
                yeniMK.KisaAdi = KisaAdiTxt.Text;
                yeniMK.Olusturan = CurrentUserName;
                yeniMK.Save();
                MessageHelper.PublishMessage("Kategori Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                RedirectToPage(ProjeConstants.PAGE_MALZEMEKATEGORI_LIST + "?Mesaj=true");
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();
            }
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
    }
}
