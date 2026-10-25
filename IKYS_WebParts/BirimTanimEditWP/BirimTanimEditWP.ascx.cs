using Model.IKYS;
using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.BirimTanimEditWP
{
    [ToolboxItemAttribute(false)]
    public partial class BirimTanimEditWP : WebPart
    {
        private PortalLogContext LoggingContext { get { return PortalLogContext.FromCurrentRequest("IKYS", "BirimTanimEditWP"); } }

        private void ReportError(string operation, Exception error)
        {
            PortalLogContext context = LoggingContext;
            if (error is System.Threading.ThreadAbortException) return;
            PortalTechnicalLog.WriteError(context, operation, error);
            Messages.Error("İşlem tamamlanamadı. Lütfen tekrar deneyin. Başvuru kodu: " + context.OperationId.ToString("N"));
        }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BirimTanimEditWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string BirimIdQS
        {
            get
            {

                if (ViewState["BirimId"] == null)
                {
                    if (Page.Request.QueryString["BirimId"] != null)
                    {
                        ViewState["BirimId"] = Page.Request.QueryString["BirimId"];
                    }
                    else
                    {
                        ViewState["BirimId"] = string.Empty;
                    }
                }
                return ViewState["BirimId"].ToString();
            }

            set
            {
                ViewState["BirimId"] = value;
            }
        }
        private string SecilenIdQS
        {
            get
            {

                if (ViewState["SecilenId"] == null)
                {
                    if (Page.Request.QueryString["SecilenId"] != null)
                    {
                        ViewState["SecilenId"] = Page.Request.QueryString["SecilenId"];
                    }
                    else
                    {
                        ViewState["SecilenId"] = BirimIdQS;
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
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
                if (!Page.IsPostBack) new BirimTanimService().TryRecordWebPartOpened(LoggingContext);
                BirimTanim birim = new BirimTanimService().GetById(BirimIdQS.ConvertToInt(), LoggingContext);
                if (!Page.IsPostBack)
                {
                    FillUstBirimDDL();
                    FillAmirDDL();
                    FillBirimToForm();
                }
                if (birim == null)
                {
                    TitleLbl.Text = "Birim/Şube/Dir. Girişi";
                    UpdateBtn.Visible = false;
                    SaveBtn.Visible = true;
                    AktifChk.Checked = true;
                    BirimKaldirildiChk.Checked = false;
                }
                else
                {
                    TitleLbl.Text = "Birim/Şube/Dir. Düzenleme";
                    UpdateBtn.Visible = true;
                    SaveBtn.Visible = false;
                }
            }
            catch (Exception exception)
            {
                ReportError("BirimTanim.PageLoad", exception);
            }
        }
        private void FillBirimToForm()
        {
            BirimTanim birim = new BirimTanimService().GetById(BirimIdQS.ConvertToInt(), LoggingContext);
            if (birim != null)
            {
                AdiTxt.Text = birim.Adi;
                KisaAdiTxt.Text = birim.KisaAdi;
                AktifChk.Checked = birim.Aktif;
                BirimKaldirildiChk.Checked = birim.BirimKaldirildi;
                if (UstBirimDDL.Items.FindByValue(birim.ParentId.ReturnZeroIfNull().ToString()) != null)
                    UstBirimDDL.SelectedValue = UstBirimDDL.Items.FindByValue(birim.ParentId.ReturnZeroIfNull().ToString()).Value;
                if (AmirDDL.Items.FindByValue(birim.AmirId.ReturnZeroIfNull().ToString()) != null)
                    AmirDDL.SelectedValue = AmirDDL.Items.FindByValue(birim.AmirId.ReturnZeroIfNull().ToString()).Value;
            }

        }
        private void FillUstBirimDDL()
        {
            UstBirimDDL.Items.Clear();
            List<BirimTanim> list = new BirimTanimService().GetAll(LoggingContext);
            ListItem bosLi = new ListItem("", "0");
            UstBirimDDL.Items.Add(bosLi);
            foreach (BirimTanim gr in list)
            {
                ListItem li = new ListItem(gr.Adi.ReturnEmptyIfNull().ToString(), gr.Id.ReturnZeroIfNull().ToString());
                UstBirimDDL.Items.Add(li);
            }
        }
        private void FillAmirDDL()
        {
            AmirDDL.Items.Clear();
            List<Personel> list = new PersonelService().GetActiveEmployees(Personel.PersonelTipi.Kadrolu, LoggingContext);
            ListItem bosLi = new ListItem("", "0");
            AmirDDL.Items.Add(bosLi);
            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                AmirDDL.Items.Add(li);
            }
        }
        protected void BirimListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_BIRIM_LIST + "?SecilenId=" + SecilenIdQS);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                BirimTanim birim = new BirimTanim();
                if (birim != null)
                {
                    birim.Adi = AdiTxt.Text;
                    birim.KisaAdi = KisaAdiTxt.Text;
                    birim.ParentId = UstBirimDDL.SelectedItem.Value.ConvertToInt();
                    birim.AmirId = AmirDDL.SelectedItem.Value.ConvertToInt();
                    birim.Aktif = AktifChk.Checked;
                    birim.BirimKaldirildi = BirimKaldirildiChk.Checked;
                    birim.Degistiren = CurrentUserName;
                    int id = new BirimTanimService().Save(birim, LoggingContext);
                    if (id > 0)
                    {
                        Messages.Success("Yeni birim kaydedildi", 2000);
                        SecilenIdQS = BirimIdQS = id.ToString();
                        SaveBtn.Visible = false;
                        UpdateBtn.Visible = true;
                    }
                    else
                    {
                        Messages.Error("Birim kaydedilemedi");
                        UpdateBtn.Visible = false;
                        SaveBtn.Visible = true;
                    }

                }
            }
            catch (Exception exception)
            {
                ReportError("BirimTanim.Save", exception);
            }
        }
        protected void DeleteBtn_Click(object sender, EventArgs e)
        {
            //iki kosulu saglarsa silebilir aksi halde sildirmemeli
            // 1. bu birim BirimTanim_Table'da baska bir birimin parentId'si ise silinemez
            // 2. IsBilgileri_Table'da birimId'si bu birim olan varsa sildirme
        }
        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                BirimTanim birim = new BirimTanimService().GetById(BirimIdQS.ConvertToInt(), LoggingContext);
                if (birim != null)
                {
                    birim.Adi = AdiTxt.Text;
                    birim.KisaAdi = KisaAdiTxt.Text;
                    birim.ParentId = UstBirimDDL.SelectedItem.Value.ConvertToInt();
                    birim.AmirId = AmirDDL.SelectedItem.Value.ConvertToInt();
                    birim.Aktif = AktifChk.Checked;
                    birim.BirimKaldirildi = BirimKaldirildiChk.Checked;
                    birim.Degistiren = CurrentUserName;
                    bool isupdated = new BirimTanimService().Update(birim, LoggingContext);
                    if (isupdated)
                        Messages.Success("Birim Güncellendi", 2000);
                    else
                        Messages.Error("Birim Güncellenemedi");
                }
            }
            catch (Exception exception)
            {

                ReportError("BirimTanim.Update", exception);
            }
        }

        protected void BirimSemasiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_BIRIM_SEMA);
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ReportError("BirimTanim.Redirect", ex);
            }
        }
    }
}
