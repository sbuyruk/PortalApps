using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.ArmaganEditWP
{
    [ToolboxItemAttribute(false)]
    public partial class ArmaganEditWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ArmaganEditWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SecilenAyQS
        {
            get
            {

                if (ViewState["SecilenAy"] == null)
                {
                    if (Page.Request.QueryString["SecilenAy"] != null)
                    {
                        ViewState["SecilenAy"] = Page.Request.QueryString["SecilenAy"];
                    }
                    else
                    {
                        ViewState["SecilenAy"] = string.Empty;
                    }
                }
                return ViewState["SecilenAy"].ToString();
            }

            set
            {
                ViewState["SecilenAy"] = value;
            }
        }
        private string SecilenYilQS
        {
            get
            {

                if (ViewState["SecilenYil"] == null)
                {
                    if (Page.Request.QueryString["SecilenYil"] != null)
                    {
                        ViewState["SecilenYil"] = Page.Request.QueryString["SecilenYil"];
                    }
                    else
                    {
                        ViewState["SecilenYil"] = string.Empty;
                    }
                }
                return ViewState["SecilenYil"].ToString();
            }

            set
            {
                ViewState["SecilenYil"] = value;
            }
        }
        private string SecilenArmaganTanimIdQS
        {
            get
            {

                if (ViewState["SecilenArmaganTanimId"] == null)
                {
                    if (Page.Request.QueryString["SecilenArmaganTanimId"] != null)
                    {
                        ViewState["SecilenArmaganTanimId"] = Page.Request.QueryString["SecilenArmaganTanimId"];
                    }
                    else
                    {
                        ViewState["SecilenArmaganTanimId"] = string.Empty;
                    }
                }
                return ViewState["SecilenArmaganTanimId"].ToString();
            }

            set
            {
                ViewState["SecilenArmaganTanimId"] = value;
            }
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
        private string ArmaganIdQS
        {
            get
            {

                if (ViewState["ArmaganId"] == null)
                {
                    if (Page.Request.QueryString["ArmaganId"] != null)
                    {
                        ViewState["ArmaganId"] = Page.Request.QueryString["ArmaganId"];
                    }
                    else
                    {
                        ViewState["ArmaganId"] = string.Empty;
                    }
                }
                return ViewState["ArmaganId"].ToString();
            }

            set
            {
                ViewState["ArmaganId"] = value;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    FillDropDownList();
                    if (!string.IsNullOrEmpty(ArmaganIdQS))
                    {
                        FillArmaganForm();
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void FillArmaganForm()
        {
            KaydetBtn.CssClass = "btn btn-outline-primary";
            KaydetBtn.Text = "Güncelle";
            if (!string.IsNullOrEmpty(ArmaganIdQS))
            {
                int armaganId = ArmaganIdQS.ConvertToInt();

                Armagan armagan = new Armagan();
                armagan = armagan.Select<Armagan>(armaganId);
                if (armagan != null)
                {
                    IdLbl.Text = armaganId.ToString();
                    NakitBagisci nakitBagisci = new NakitBagisci();
                    nakitBagisci = nakitBagisci.Select<NakitBagisci>(armagan.BagisciId);
                    if (nakitBagisci != null)
                    {
                        AdiTxt.Text = nakitBagisci.Adi.ReturnEmptyIfNull().ToString();
                    }
                    BagisMiktariTxt.Text = armagan.BagisMiktari.ReturnZeroIfNull().ToString();

                    ArmaganDDL.Items.FindByValue(armagan.ArmaganTanimId.ReturnZeroIfNull().ToString()).Selected = true;
                    DurumDDL.Items.FindByValue(armagan.Durum.ReturnZeroIfNull().ToString()).Selected = true;
                    BagisTarihiTxt.Value = armagan.Tarih.ToString("dd.MM.yyyy");
                    AciklamaTxt.Text = armagan.Aciklama.ReturnEmptyIfNull().ToString();
                    BelgedeYazanIsimTxt.Text = armagan.BelgedeYazanIsim.ReturnEmptyIfNull().ToString();
                    BagisMiktariYazmasinChk.Checked = armagan.BagisMiktariYazmasin;
                }

            }

        }
        private void FillDropDownList()
        {
            DurumDDLDoldur();
            FillArmaganTanim();
        }
        private void DurumDDLDoldur()
        {
            DurumDDL.Items.Add(ProjeConstants.DURUM_BELGE_ISTEMIYOR);
            DurumDDL.Items.Add(ProjeConstants.DURUM_GONDERILMEDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_GONDERILDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_ULASILAMADI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_KONTROLEDILDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_ERKENGONDERILDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_ERTELENDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_IADE);
            DurumDDL.Items.Add(ProjeConstants.DURUM_DAHAONCEIADE);
        }
        private void FillArmaganTanim()
        {
            if (ArmaganDDL.SelectedItem == null)
            {
                ArmaganDDL.Items.Clear();
                ArmaganTanim armaganTanim = new ArmaganTanim();
                List<ArmaganTanim> list = armaganTanim.SelectAll<ArmaganTanim>();// armaganTanim.SelectAktifArmaganTanim();
                foreach (ArmaganTanim arm in list)
                {
                    ArmaganDDL.Items.Add(new ListItem(arm.Armagan, arm.Id.ToString()));
                }
            }

        }
        protected void ArmaganDLL_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                FillArmaganTanim();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void DurumDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                FillArmaganTanim();
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
                bool isSaved = false;
                if (string.IsNullOrEmpty(ArmaganIdQS))
                {
                    //System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                    //    typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "Alert.danger('Armağan Kaydı bulunamadı.')", true);
                    MessageHelper.PublishMessage("Armağan Kaydı bulunamadı", ProjeConstants.MESAJ_HATA);
                }
                else
                {
                    int armaganId = ArmaganIdQS.ConvertToInt();
                    Armagan armagan = new Armagan();
                    armagan = armagan.Select<Armagan>(armaganId);
                    if (armaganId == 0)
                    {
                        isSaved = saveArmagan(armagan);
                    }
                    else
                    {
                        armagan = armagan.Select<Armagan>(armaganId);
                        isSaved = UpdateArmagan(armagan);
                    }

                }

                if (isSaved)
                {
                    //System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "Alert.success('Armağan Kaydedildi.')", true);
                    MessageHelper.PublishMessage("Armağan Kaydı Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                }
                else
                {
                    //System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "Alert.danger('Armağan Kaydedilemedi.')", true);
                    MessageHelper.PublishMessage("Armağan Kaydedilemedi", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

        }
        private bool saveArmagan(Armagan armagan)
        {
            bool isSaved = false;
            armagan.ArmaganTanimId = ArmaganDDL.SelectedValue.ConvertToInt();
            armagan.BagisMiktari = BagisMiktariTxt.Text.ConvertToDecimal();
            armagan.Durum = DurumDDL.SelectedValue.ReturnZeroIfNull().ToString();
            armagan.Tarih = BagisTarihiTxt.Value.ConvertToDatetime();
            armagan.Aciklama = AciklamaTxt.Text.ReturnEmptyIfNull().ToString();
            armagan.Olusturan = CurrentUserName;
            armagan.BelgedeYazanIsim = BelgedeYazanIsimTxt.Text.ReturnEmptyIfNull().ToString();
            armagan.BagisMiktariYazmasin = BagisMiktariYazmasinChk.Checked;
            armagan.Id = armagan.Save();
            if (armagan.Id > 0)
                isSaved = true;
            return isSaved;
        }
        private bool UpdateArmagan(Armagan armagan)
        {
            armagan.ArmaganTanimId = ArmaganDDL.SelectedValue.ConvertToInt();
            armagan.BagisMiktari = BagisMiktariTxt.Text.ConvertToDecimal();
            armagan.Durum = DurumDDL.SelectedValue.ReturnZeroIfNull().ToString();
            armagan.Tarih = BagisTarihiTxt.Value.ConvertToDatetime();
            armagan.Aciklama = AciklamaTxt.Text.ReturnEmptyIfNull().ToString();
            armagan.BelgedeYazanIsim = BelgedeYazanIsimTxt.Text.ReturnEmptyIfNull().ToString();
            armagan.BagisMiktariYazmasin = BagisMiktariYazmasinChk.Checked;
            armagan.Degistiren = CurrentUserName;

            bool isSaved = armagan.Update();

            return isSaved;
        }
        protected void ArmaganListesiBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string queryStr = "?SecilenId=" + ArmaganIdQS + "&SecilenAy=" + SecilenAyQS + "&SecilenYil="
                + SecilenYilQS + "&SecilenArmaganTanimId=" + SecilenArmaganTanimIdQS + "&SecilenDurum=" + SecilenDurumQS;
                RedirectToPage(ProjeConstants.PAGE_ARMAGAN_LIST + queryStr);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
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
