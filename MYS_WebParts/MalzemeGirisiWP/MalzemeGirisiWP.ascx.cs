using Microsoft.SharePoint;
using Model.MYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using TSKGV_Utility.HelperClasses;
using TSKGV_Utility.ProjeGlobal;

namespace MYS_WebParts.MalzemeGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class MalzemeGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public MalzemeGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string MalzemeIdQS
        {
            get
            {

                if (ViewState["MalzemeId"] == null)
                {
                    if (Page.Request.QueryString["MalzemeId"] != null)
                    {
                        ViewState["MalzemeId"] = Page.Request.QueryString["MalzemeId"];
                    }
                    else
                    {
                        ViewState["MalzemeId"] = string.Empty;
                    }
                }
                return ViewState["MalzemeId"].ToString();
            }

            set
            {
                ViewState["MalzemeId"] = value;
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
            TitleLbl.Text = "Malzeme Düzenleme";
            Malzeme malzeme = new Malzeme();
            malzeme = malzeme.Select<Malzeme>(MalzemeIdQS.ConvertToInt());
            if (!Page.IsPostBack)
            {
                if (malzeme != null)
                {
                    int malzemeCinsi = malzeme.MalzemeCinsiId;
                    MalzemeCinsi mc = new MalzemeCinsi();
                    mc = mc.Select<MalzemeCinsi>(malzemeCinsi);
                    fillKategoriDDL();
                    if (mc != null)
                    {
                        UtilityHelper.SetDDLValue(KategoriDDL, mc.KategoriId.ToString());
                    }
                    fillCinsiDDL();
                    UtilityHelper.SetDDLValue(CinsiDDL, malzemeCinsi.ToString());

                    IdLbl.Text = malzeme.Id.ToString();
                    MalzemeLbl.Text = malzeme.Adi;
                    AdiTxt.Text = malzeme.Adi;
                    SeriNoTxt.Text = malzeme.SeriNo;
                    BirimFiyatTxt.Text = malzeme.BirimFiyat.ConvertToDecimal().ToString();
                    AdetTxt.Text = malzeme.Adet.ConvertToInt().ToString();
                    EnvantereGirisTarTxt.Value = malzeme.EnvantereGirisTar.ConvertToDatetimeEmptyIfNull();
                    AciklamaTxt.Text = malzeme.Aciklama;

                    //string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    //string rawUrl = System.Web.HttpContext.Current.Request.RawUrl.ToString();
                    //int index = currentUrl.IndexOf(rawUrl);
                    //string rootUrl = (index < 0) ? currentUrl : currentUrl.Remove(index, rawUrl.Length);
                    //string imgUrl = rootUrl + "/" + ProjeConstants.RESIMLER_MALZEME + "/_t/" + malzeme.Id + "_jpg.jpg";
                    //DisplayImage.ImageUrl = imgUrl;
                    string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                    string imgUrl = newUrl + "/../" + ProjeConstants.RESIMLER_MALZEME + "/_t/" + malzeme.Id + "_jpg.jpg";

                    DisplayImage.ImageUrl = imgUrl;

                }
            }
        }
        private void OpenGiris()
        {
            fillKategoriDDL();
            fillCinsiDDL();
            BackBtn.Visible = false;
            SaveBtn.Visible = true;
            UpdateBtn.Visible = false;
            DeleteBtn.Visible = false;

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
        private void fillCinsiDDL()
        {
            CinsiDDL.Items.Clear();
            MalzemeCinsi mc = new MalzemeCinsi();
            int kategori = KategoriDDL.SelectedItem.Value.ConvertToInt();
            List<MalzemeCinsi> list = mc.SelectByKategori(kategori);
            foreach (MalzemeCinsi item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                CinsiDDL.Items.Add(li);
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

        private void saveImageFiles2SP(string fotoFile)
        {
            //string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            //string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
            //string imgPath = newUrl + "/../" + ProjeConstants.RESIMLER_MALZEME;//SPImageListName
            string SPImageListName = ProjeConstants.RESIMLER_MALZEME;

            SPWeb web = Microsoft.SharePoint.SPContext.Current.Web;
            SPList listExists = web.Lists.TryGetList(SPImageListName);
            ExceptionHelper exhelper = new ExceptionHelper();
            exhelper= UtilityHelper.uploadFile2SP(xFileUpload, "", fotoFile + "", SPImageListName, exhelper, ProjeConstants.RESIM_DIGER_EN, ProjeConstants.RESIM_DIGER_BOY);
            // then use the following to add the file to the list
            //listExists.RootFolder.Files.Add(fotoFile, UtilityHelper.StreamFile(xFileUpload.PostedFile.FileName));//MyUploadtoSharepoint(ProjeConstants.RESIMLER_MALZEME, xFileUpload.PostedFile.FileName, "2.jpg");
            if (exhelper.HasException())
            {
                Exception ex = new Exception("Resim Kaydedilemedi.");
                exhelper.Exceptions.Add(ex);
                exhelper.PublishException();
            }
            else
            {
                MessageHelper.PublishMessage("İşlem Tamamlandı.Resim yüklendi.", ProjeConstants.MESAJ_BASARILI);
            }
        }
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_MALZEME_LIST);
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
                Malzeme malzeme = new Malzeme();
                malzeme = malzeme.Select<Malzeme>(MalzemeIdQS.ConvertToInt());
                if (malzeme != null)
                {
                    malzeme.Adi = AdiTxt.Text;
                    malzeme.MalzemeCinsiId = CinsiDDL.SelectedItem.Value.ConvertToInt();
                    malzeme.Aciklama = AciklamaTxt.Text;
                    malzeme.Adet = AdetTxt.Text.ConvertToInt();
                    malzeme.BirimFiyat = BirimFiyatTxt.ConvertToDecimal();
                    malzeme.Envanterde = true;
                    malzeme.EnvantereGirisTar = EnvantereGirisTarTxt.Value.ConvertToDatetime();
                    malzeme.SeriNo = SeriNoTxt.Text;
                    malzeme.Degistiren = CurrentUserName;
                    bool isSaved = malzeme.Update();
                    // eger bir resim seçildi ise o resmi Sharepointteki MalzemeResimleri listesine ekle
                    if (xFileUpload.HasFile)
                    {
                        saveImageFiles2SP(malzeme.Id.ToString());

                    }
                    MessageHelper.PublishMessage("Malzeme Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                    RedirectToPage(ProjeConstants.PAGE_MALZEME_LIST + "?Mesaj=true");
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
                Malzeme mk = new Malzeme();
                mk = mk.Select<Malzeme>(MalzemeIdQS.ConvertToInt());
                if (mk != null)
                {
                    mk.Delete();
                    MessageHelper.PublishMessage("Malzeme Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                    RedirectToPage(ProjeConstants.PAGE_MALZEME_LIST + "?Mesaj=true");
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
                Malzeme malzeme = new Malzeme();
                malzeme.Adi = AdiTxt.Text;
                malzeme.MalzemeCinsiId = CinsiDDL.SelectedItem.Value.ConvertToInt();
                malzeme.Aciklama = AciklamaTxt.Text;
                malzeme.Adet = AdetTxt.Text.ConvertToInt();
                malzeme.BirimFiyat = BirimFiyatTxt.ConvertToDecimal();
                malzeme.Envanterde = true;
                malzeme.EnvantereGirisTar = EnvantereGirisTarTxt.Value.ConvertToDatetime();
                malzeme.SeriNo = SeriNoTxt.Text;
                malzeme.Olusturan = CurrentUserName;
                int malzemeId = malzeme.Save();
                if (malzemeId > 0)
                {
                    // eger bir resim seçildi ise o resmi Sharepointteki MalzemeResimleri listesine ekle
                    if (xFileUpload.HasFile)
                    {
                        saveImageFiles2SP(malzemeId.ToString());
                    }

                    MessageHelper.PublishMessage("Malzeme Cinsi Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                    RedirectToPage(ProjeConstants.PAGE_MALZEME_LIST + "?Mesaj=true");
                }

            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();
            }
        }
        protected void KategoriDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillCinsiDDL();
        }
    }
}
