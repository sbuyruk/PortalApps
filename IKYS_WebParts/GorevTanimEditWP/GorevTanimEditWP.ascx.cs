using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.GorevTanimEditWP
{
    [ToolboxItemAttribute(false)]
    public partial class GorevTanimEditWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public GorevTanimEditWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string GorevTanimIdQS
        {
            get
            {
                if (ViewState["GorevTanimId"] == null)
                {
                    if (Page.Request.QueryString["GorevTanimId"] != null)
                    {
                        ViewState["GorevTanimId"] = Page.Request.QueryString["GorevTanimId"];
                    }
                    else
                    {
                        ViewState["GorevTanimId"] = string.Empty;
                    }
                }
                return ViewState["GorevTanimId"].ToString();
            }
            set
            {
                ViewState["GorevTanimId"] = value;
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
                        ViewState["SecilenId"] = GorevTanimIdQS;
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillBirimDDL();
                FillPersonelDDL();
                if (GorevTanimIdQS.ConvertToInt() > 0)
                {
                    OpenDuzenle();
                }
                else
                {
                    OpenGiris();
                }
            }
        }

        private void OpenGiris()
        {
            SaveBtn.Visible = true;
            UpdateBtn.Visible = false;
            DeleteBtn.Visible = false;
            
        }
        private void OpenDuzenle()
        {
            SaveBtn.Visible = false;
            UpdateBtn.Visible = true;
            DeleteBtn.Visible = true;
            FillGorevToForm();
        }
        private void FillGorevToForm()
        {
            GorevTanim gorev = new GorevTanim();
            gorev = gorev.Select<GorevTanim>(GorevTanimIdQS.ConvertToInt());
            if (gorev != null)
            {
                AdiTxt.Text = gorev.Adi;
                KisaAdiTxt.Text = gorev.KisaAdi;
                if (BirimDDL.Items.FindByValue(gorev.BirimId.ReturnZeroIfNull().ToString()) != null)
                    BirimDDL.SelectedValue = BirimDDL.Items.FindByValue(gorev.BirimId.ReturnZeroIfNull().ToString()).Value;
                if (PersonelDDL.Items.FindByValue(gorev.PersonelId.ReturnZeroIfNull().ToString()) != null)
                    PersonelDDL.SelectedValue = PersonelDDL.Items.FindByValue(gorev.PersonelId.ReturnZeroIfNull().ToString()).Value;
                VekilChk.Checked = gorev.Vekil;
                AktifChk.Checked = gorev.Aktif;
            }
        }
        private void FillBirimDDL()
        {
            BirimDDL.Items.Clear();
            BirimTanim birimDao = new BirimTanim();
            List<BirimTanim> list = birimDao.SelectAll<BirimTanim>();
            ListItem bosLi = new ListItem("", "0");
            BirimDDL.Items.Add(bosLi);
            foreach (BirimTanim gr in list)
            {
                ListItem li = new ListItem(gr.Adi.ReturnEmptyIfNull().ToString(), gr.Id.ReturnZeroIfNull().ToString());
                BirimDDL.Items.Add(li);
            }
        }
        private void FillPersonelDDL()
        {
            PersonelDDL.Items.Clear();
            Personel personel = new Personel();
            List<Personel> list = personel.SelectCalisanPersonel();
            ListItem bosLi = new ListItem("", "0");
            PersonelDDL.Items.Add(bosLi);
            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                PersonelDDL.Items.Add(li);
            }
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
                GorevTanim gorev = new GorevTanim();

                gorev.Adi = AdiTxt.Text;
                gorev.KisaAdi = KisaAdiTxt.Text;
                gorev.BirimId = BirimDDL.SelectedItem.Value.ConvertToInt();
                gorev.PersonelId = PersonelDDL.SelectedItem.Value.ConvertToInt();
                gorev.Vekil = VekilChk.Checked;
                gorev.Aktif = AktifChk.Checked;
                int id = gorev.Save();
                if (id > 0)
                {
                    MessageHelper.PublishMessage("Yeni gorev kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                    GorevTanimIdQS = id.ToString();
                    SaveBtn.Visible = false;
                    UpdateBtn.Visible = true;
                    RedirectToPage(ProjeConstants.PAGE_GOREVTANIM_LIST + "?SecilenId=" + GorevTanimIdQS);
                }
                else
                {
                    MessageHelper.PublishMessage("Gorev kaydedilemedi", ProjeConstants.MESAJ_HATA);
                    UpdateBtn.Visible = false;
                    SaveBtn.Visible = true;
                }


            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.Exceptions.Add(new Exception("Gorev kaydedilemedi!"));
                ex.PublishException();
            }

        }
        protected void DeleteBtn_Click(object sender, EventArgs e)
        {
            IsBilgileri isBilgileri = new IsBilgileri();
            isBilgileri = isBilgileri.SelectByGorevId(GorevTanimIdQS.ConvertToInt());
            if (isBilgileri == null)
            {
                GorevTanim gorevTanim = new GorevTanim();
                gorevTanim = gorevTanim.SelectByGorevId(GorevTanimIdQS.ConvertToInt());
                if (gorevTanim != null)
                {
                    if (gorevTanim.Delete())
                        RedirectToPage(ProjeConstants.PAGE_GOREVTANIM_LIST + "?Mesaj=true");
                    else
                        MessageHelper.PublishMessage("Kadroyu silinemedi", ProjeConstants.MESAJ_HATA);
                }
            }
            else
            {
                Personel personel = new Personel();
                personel = personel.Select(isBilgileri.PersonelId);
                string adi = personel!=null? "(" + personel.Adi + " " + personel.Soyadi +")":string.Empty;
                MessageHelper.PublishMessage("Bu kadroda tanımlı personel bulunmaktadır.\n+" + adi +
                    " Kadroyu silmek için önce kadroyu boşaltın", ProjeConstants.MESAJ_HATA);
            }
        }
        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GorevTanim gorev = new GorevTanim();
                gorev = gorev.Select<GorevTanim>(GorevTanimIdQS.ConvertToInt());

                if (gorev != null)
                {
                    int oncekiPersonelId = gorev.PersonelId;
                    gorev.Adi = AdiTxt.Text;
                    gorev.KisaAdi = KisaAdiTxt.Text;
                    gorev.BirimId = BirimDDL.SelectedItem.Value.ConvertToInt();
                    gorev.PersonelId = PersonelDDL.SelectedItem.Value.ConvertToInt();
                    gorev.Vekil = VekilChk.Checked;
                    gorev.Aktif = AktifChk.Checked;
                    gorev.Degistiren = CurrentUserName;
                    bool isupdated = gorev.Update();
                    if (isupdated)
                    {
                        IsBilgileri ib = new IsBilgileri();
                        ib = ib.SelectByPersonelId(oncekiPersonelId);
                        if (ib != null)
                        {
                            ib.GorevId = 0;
                            ib.Update();
                        }
                        ib = new IsBilgileri();
                        ib = ib.SelectByPersonelId(gorev.PersonelId);
                        if (ib != null)
                        {
                            ib.GorevId = gorev.Id;
                            ib.Update();
                        }

                        MessageHelper.PublishMessage("Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                    }

                }
            }
            catch (Exception exception)
            {

                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();
            }
        }

        protected void GorevTanimListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_GOREVTANIM_LIST + "?SecilenId=" + SecilenIdQS);
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
