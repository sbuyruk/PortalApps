using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.BirimTanimEditWP
{
    [ToolboxItemAttribute(false)]
    public partial class BirimTanimEditWP : WebPart
    {
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
            BirimTanim birim = new BirimTanim();
            birim = birim.Select<BirimTanim>(BirimIdQS.ConvertToInt());
            if (!Page.IsPostBack)
            {
                FillUstBirimDDL();
                FillAmirDDL();
                FillBirimToForm();
            }
            if (birim == null)
            {
                TitleLbl.Text = "Birim/Sube/Dir. Girisi";
                UpdateBtn.Visible = false;
                SaveBtn.Visible = true;
                AktifChk.Checked = true;
                BirimKaldirildiChk.Checked = false;
            }
            else
            {
                TitleLbl.Text = "Birim/Sube/Dir. Düzenleme";
                UpdateBtn.Visible = true;
                SaveBtn.Visible = false;
            }
        }
        private void FillBirimToForm()
        {
            BirimTanim birim = new BirimTanim();
            birim = birim.Select<BirimTanim>(BirimIdQS.ConvertToInt());
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
            BirimTanim birimDao = new BirimTanim();
            List<BirimTanim> list = birimDao.SelectAll<BirimTanim>();
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
            Personel personel = new Personel();
            List<Personel> list = personel.SelectCalisanPersonel();
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
                    int id = birim.Save();
                    if (id > 0)
                    {
                        MessageHelper.PublishMessage("Yeni birim kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                        SecilenIdQS = BirimIdQS = id.ToString();
                        SaveBtn.Visible = false;
                        UpdateBtn.Visible = true;
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Birim kaydedilemedi", ProjeConstants.MESAJ_HATA);
                        UpdateBtn.Visible = false;
                        SaveBtn.Visible = true;
                    }
                        
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.Exceptions.Add(new Exception("Birim kaydedilemedi!"));
                ex.PublishException();
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
                BirimTanim birim = new BirimTanim();
                birim = birim.Select<BirimTanim>(BirimIdQS.ConvertToInt());
                if (birim != null)
                {
                    birim.Adi = AdiTxt.Text;
                    birim.KisaAdi = KisaAdiTxt.Text;
                    birim.ParentId = UstBirimDDL.SelectedItem.Value.ConvertToInt();
                    birim.AmirId = AmirDDL.SelectedItem.Value.ConvertToInt();
                    birim.Aktif = AktifChk.Checked;
                    birim.BirimKaldirildi = BirimKaldirildiChk.Checked;
                    birim.Degistiren = CurrentUserName;
                    bool isupdated = birim.Update();
                    if (isupdated)
                        MessageHelper.PublishMessage("Birim Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                    else
                        MessageHelper.PublishMessage("Birim Güncellenemedi", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception exception)
            {

                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();
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
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
    }
}
