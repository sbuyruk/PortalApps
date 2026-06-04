using Model.MYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using TSKGV_Utility.HelperClasses;
using TSKGV_Utility.ProjeGlobal;

namespace MYS_WebParts.MalzemeCinsiGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class MalzemeCinsiGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public MalzemeCinsiGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string MalzemeCinsiIdQS
        {
            get
            {

                if (ViewState["MalzemeCinsiId"] == null)
                {
                    if (Page.Request.QueryString["MalzemeCinsiId"] != null)
                    {
                        ViewState["MalzemeCinsiId"] = Page.Request.QueryString["MalzemeCinsiId"];
                    }
                    else
                    {
                        ViewState["MalzemeCinsiId"] = string.Empty;
                    }
                }
                return ViewState["MalzemeCinsiId"].ToString();
            }

            set
            {
                ViewState["MalzemeCinsiId"] = value;
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
                if (!Page.IsPostBack)
                {
                    fillKategoriDDL();
                    if (string.IsNullOrEmpty(DestinationAppQS) || (!DestinationAppQS.Equals("MKD")))
                    {
                        OpenGiris();
                    }
                    else if (DestinationAppQS.Equals("MKD"))
                    {
                        OpenDuzenle();
                    }
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
            TitleLbl.Text = "Malzeme Cinsi Düzenleme";
            MalzemeCinsi cinsi = new MalzemeCinsi();
            cinsi = cinsi.Select<MalzemeCinsi>(MalzemeCinsiIdQS.ConvertToInt());
            if (!Page.IsPostBack)
            {
                if (cinsi != null)
                {
                    IdLbl.Text = cinsi.Id.ToString();
                    CinsiLbl.Text = cinsi.Adi;
                    AdiTxt.Text = cinsi.Adi;
                    KisaAdiTxt.Text = cinsi.KisaAdi;
                    SetDDLValue(KategoriDDL, cinsi.KategoriId.ToString());
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
        private void SetDDLValue(DropDownList ddl, string value)
        {
            try
            {
                ListItem listItem = new ListItem();
                if (!string.IsNullOrEmpty(value))
                    listItem = ddl.Items.FindByValue(value);

                if (listItem != null)
                {
                    ddl.SelectedValue = listItem.Value;
                }
            }
            catch (Exception)
            {

                //TODO
            }



        }
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            string newUrl = string.Empty;
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();

            newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_MALZEMECINSI_LIST;

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
                MalzemeCinsi mk = new MalzemeCinsi();
                mk = mk.Select<MalzemeCinsi>(MalzemeCinsiIdQS.ConvertToInt());
                if (mk != null)
                {
                    mk.Adi = AdiTxt.Text;
                    mk.KisaAdi = KisaAdiTxt.Text;
                    mk.KategoriId = KategoriDDL.SelectedItem.Value.ConvertToInt();
                    mk.Degistiren = CurrentUserName;
                    mk.Update();
                    MessageHelper.PublishMessage("Malzeme Cinsi Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                    RedirectToPage(ProjeConstants.PAGE_MALZEMECINSI_LIST + "?Mesaj=true");
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
                MalzemeCinsi mk = new MalzemeCinsi();
                mk = mk.Select<MalzemeCinsi>(MalzemeCinsiIdQS.ConvertToInt());
                if (mk != null)
                {
                    mk.Delete();
                    MessageHelper.PublishMessage("Cinsi Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                    RedirectToPage(ProjeConstants.PAGE_MALZEMECINSI_LIST + "?Mesaj=true");
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
                MalzemeCinsi mk = new MalzemeCinsi();
                mk = mk.SelectMax();
                if (mk != null)
                {
                    yeniMKId = ++mk.Id;
                }
                MalzemeCinsi yeniMK = new MalzemeCinsi();
                yeniMK.Id = yeniMKId;
                yeniMK.Adi = AdiTxt.Text;
                yeniMK.KisaAdi = KisaAdiTxt.Text;
                yeniMK.KategoriId = KategoriDDL.SelectedItem.Value.ConvertToInt();
                yeniMK.Olusturan = CurrentUserName;
                yeniMK.Save();
                MessageHelper.PublishMessage("Malzeme Cinsi Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                RedirectToPage(ProjeConstants.PAGE_MALZEMECINSI_LIST + "?Mesaj=true");
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

        private void fillKategoriDDL()
        {
            KategoriDDL.Items.Clear();
            MalzemeKategori mk = new MalzemeKategori();
            List<MalzemeKategori> list = mk.SelectAll<MalzemeKategori>();
            //ListItem bosLi = new ListItem("", "0");
            //KategoriDDL.Items.Add(bosLi);
            foreach (MalzemeKategori gr in list)
            {
                ListItem li = new ListItem(gr.Adi.ReturnEmptyIfNull().ToString(), gr.Id.ReturnZeroIfNull().ToString());
                KategoriDDL.Items.Add(li);
            }
        }
    }
}
