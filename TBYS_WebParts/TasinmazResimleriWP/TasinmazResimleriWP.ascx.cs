using Microsoft.SharePoint;
using Model.Ortak;
using Model.TBYS;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;


namespace TBYS_WebParts.TasinmazResimleriWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazResimleriWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazResimleriWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        public string SPImageListName { get; private set; }
        private string TasinmazIdQS
        {
            get
            {

                if (ViewState["TasinmazId"] == null)
                {
                    if (Page.Request.QueryString["TasinmazId"] != null)
                    {
                        ViewState["TasinmazId"] = Page.Request.QueryString["TasinmazId"];
                    }
                    else
                    {
                        ViewState["TasinmazId"] = string.Empty;
                    }
                }
                return ViewState["TasinmazId"].ToString();
            }

            set
            {
                ViewState["TasinmazId"] = value;
            }
        }
        private string MesajQS
        {
            get
            {

                if (ViewState["Mesaj"] == null)
                {
                    if (Page.Request.QueryString["Mesaj"] != null)
                    {
                        ViewState["Mesaj"] = Page.Request.QueryString["Mesaj"];
                    }
                    else
                    {
                        ViewState["Mesaj"] = string.Empty;
                    }
                }
                return ViewState["Mesaj"].ToString();
            }

            set
            {
                ViewState["Mesaj"] = value;
            }
        }
        private string EnvanterdeMiQS
        {
            get
            {

                if (ViewState["EnvanterdeMi"] == null)
                {
                    if (Page.Request.QueryString["EnvanterdeMi"] != null)
                    {
                        ViewState["EnvanterdeMi"] = Page.Request.QueryString["EnvanterdeMi"];
                    }
                    else
                    {
                        ViewState["EnvanterdeMi"] = string.Empty;
                    }
                }
                return ViewState["EnvanterdeMi"].ToString();
            }

            set
            {
                ViewState["EnvanterdeMi"] = value;
            }
        }
        private string PageIndexQS
        {
            get
            {

                if (ViewState["PageIndex"] == null)
                {
                    if (Page.Request.QueryString["PageIndex"] != null)
                    {
                        ViewState["PageIndex"] = Page.Request.QueryString["PageIndex"];
                    }
                    else
                    {
                        ViewState["PageIndex"] = string.Empty;
                    }
                }
                return ViewState["PageIndex"].ToString();
            }

            set
            {
                ViewState["PageIndex"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(MesajQS))
            //{
            //    MessageHelper.PublishMessage("Resimler Kaydedildi.", ProjeConstants.MESAJ_BASARILI, 2000);
            //    MesajQS = string.Empty;
            //    ResimleriDoldur();
            //}
            if (!Page.IsPostBack)
            {
                if (MesajQS.ToLower().Equals("true"))
                    MessageHelper.PublishMessage("Resimler kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                else if (MesajQS.ToLower().Equals("false"))
                    MessageHelper.PublishMessage("Resimler kaydedilemedi", ProjeConstants.MESAJ_HATA);
                ResimleriDoldur();
            }

        }
        private void ResimleriDoldur()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select(TasinmazIdQS.ConvertToInt());
            if (tasinmaz != null)
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                //string imgUrl1 = newUrl + "/../" + ProjeConstants.RESIMLER_TASINMAZ + "/_t/" + tasinmaz.TasinmazFoto + "_jpg.jpg";
                //string imgUrl2 = newUrl + "/../" + ProjeConstants.RESIMLER_TASINMAZ + "/_t/" + tasinmaz.TasinmazFoto1 + "_jpg.jpg";
                //string imgUrl3 = newUrl + "/../" + ProjeConstants.RESIMLER_TASINMAZ + "/_t/" + tasinmaz.TasinmazFoto2 + "_jpg.jpg";
                //string imgUrl4 = newUrl + "/../" + ProjeConstants.RESIMLER_TASINMAZ + "/_t/" + tasinmaz.TahkikatFoto + "_jpg.jpg";
                //string imgUrl5 = newUrl + "/../" + ProjeConstants.RESIMLER_TASINMAZ + "/_t/" + tasinmaz.KrokiFoto + "_jpg.jpg";
                //string imgUrl6 = newUrl + "/../" + ProjeConstants.RESIMLER_TASINMAZ + "/_t/" + tasinmaz.TapuFoto + "_jpg.jpg";

                string imgUrl1 = newUrl + "/../" + ProjeConstants.RESIMLER_TASINMAZ + "/" + (!string.IsNullOrEmpty(tasinmaz.TasinmazFoto) ? tasinmaz.TasinmazFoto : ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO) + ".jpg";
                string imgUrl2 = newUrl + "/../" + ProjeConstants.RESIMLER_TASINMAZ + "/" + (!string.IsNullOrEmpty(tasinmaz.TasinmazFoto1) ? tasinmaz.TasinmazFoto1 : ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO1) + ".jpg";
                string imgUrl3 = newUrl + "/../" + ProjeConstants.RESIMLER_TASINMAZ + "/" + (!string.IsNullOrEmpty(tasinmaz.TasinmazFoto2) ? tasinmaz.TasinmazFoto2 : ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO2) + ".jpg";
                string imgUrl4 = newUrl + "/../" + ProjeConstants.RESIMLER_TASINMAZ + "/" + (!string.IsNullOrEmpty(tasinmaz.TahkikatFoto) ? tasinmaz.TahkikatFoto : ProjeConstants.PARAM_TASINMAZ_TAHKIKATFOTO) + ".jpg";
                string imgUrl5 = newUrl + "/../" + ProjeConstants.RESIMLER_TASINMAZ + "/" + (!string.IsNullOrEmpty(tasinmaz.KrokiFoto) ? tasinmaz.KrokiFoto : ProjeConstants.PARAM_TASINMAZ_KROKIFOTO) + ".jpg";
                string imgUrl6 = newUrl + "/../" + ProjeConstants.RESIMLER_TASINMAZ + "/" + (!string.IsNullOrEmpty(tasinmaz.TapuFoto) ? tasinmaz.TapuFoto : ProjeConstants.PARAM_TASINMAZ_TAPUFOTO) + ".jpg";
                string imgUrl7 = newUrl + "/../" + ProjeConstants.RESIMLER_TASINMAZ + "/" + (!string.IsNullOrEmpty(tasinmaz.TasinmazFoto3) ? tasinmaz.TasinmazFoto3: ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO3) + ".jpg";
                string imgUrl8 = newUrl + "/../" + ProjeConstants.RESIMLER_TASINMAZ + "/" + (!string.IsNullOrEmpty(tasinmaz.TasinmazFoto4) ? tasinmaz.TasinmazFoto4: ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO4) + ".jpg";

                Image1.ImageUrl = imgUrl1;
                Image2.ImageUrl = imgUrl2;
                Image3.ImageUrl = imgUrl3;
                Image4.ImageUrl = imgUrl4;
                Image5.ImageUrl = imgUrl5;
                Image6.ImageUrl = imgUrl6;
                Image7.ImageUrl = imgUrl7;
                Image8.ImageUrl = imgUrl8;
                PDFGoster(tasinmaz.Id);
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
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
            newUrl += "/" + ProjeConstants.PAGE_TASINMAZ_GIRIS + "?DestinationApp=TD&TasinmazId=" + TasinmazIdQS + "&EnvanterdeMi=" + EnvanterdeMiQS + "&PageIndex=" + PageIndexQS;
            if (EnvanterdeMiQS.Equals(ProjeConstants.MULKIYETTE_OLMAYAN_TASINMAZ.ToString()))
                newUrl += "/" + ProjeConstants.PAGE_MULKIYETIOLMAYANTASINMAZ_GIRIS + "?DestinationApp=TD&TasinmazId=" + TasinmazIdQS + "&EnvanterdeMi=" + EnvanterdeMiQS + "&PageIndex=" + PageIndexQS;
            Page.Response.Redirect(newUrl, true);
        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select(TasinmazIdQS.ConvertToInt());
            if (tasinmaz != null)
            {
                ExceptionHelper exhelper = updateTasinmazFoto2Db(tasinmaz);
                PDFKaydet(tasinmaz.Id);
                PDFGoster(tasinmaz.Id);
                if (exhelper.HasException())
                {
                    Exception ex = new Exception("Resim Kaydedilemedi.");
                    exhelper.Exceptions.Add(ex);
                    exhelper.PublishException();
                }
                else
                {
                    RedirectToPage(ProjeConstants.PAGE_TASINMAZ_RESIMLER + "?Mesaj=true&DestinationApp=TD&TasinmazId=" + TasinmazIdQS + "&EnvanterdeMi=" + EnvanterdeMiQS + "&PageIndex=" + PageIndexQS);
                }
            }
        }
        private ExceptionHelper updateTasinmazFoto2Db(Tasinmaz tasinmaz)
        {
            bool isSaved = false;
            ExceptionHelper exhelper = new ExceptionHelper();
            if (FileUpload0.HasFile)
            {
                tasinmaz.TasinmazFoto = tasinmaz.Id + ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO;
                if (!string.IsNullOrEmpty(tasinmaz.TasinmazFoto))
                {
                    exhelper = saveImageFiles2SP(tasinmaz, tasinmaz.TasinmazFoto, FileUpload0, exhelper);
                }
            }
            if (FileUpload1.HasFile)
            {
                tasinmaz.TasinmazFoto1 = tasinmaz.Id + ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO1;
                if (!string.IsNullOrEmpty(tasinmaz.TasinmazFoto1))
                {
                    exhelper = saveImageFiles2SP(tasinmaz, tasinmaz.TasinmazFoto1, FileUpload1, exhelper);
                }
            }
            if (FileUpload2.HasFile)
            {
                tasinmaz.TasinmazFoto2 = tasinmaz.Id + ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO2;
                if (!string.IsNullOrEmpty(tasinmaz.TasinmazFoto2))
                {
                    exhelper = saveImageFiles2SP(tasinmaz, tasinmaz.TasinmazFoto2, FileUpload2, exhelper);
                }
            }
            if (FileUpload3.HasFile)
            {
                tasinmaz.TahkikatFoto = tasinmaz.Id + ProjeConstants.PARAM_TASINMAZ_TAHKIKATFOTO;
                if (!string.IsNullOrEmpty(tasinmaz.TahkikatFoto))
                {
                    exhelper = saveImageFiles2SP(tasinmaz, tasinmaz.TahkikatFoto, FileUpload3, exhelper);
                }
            }
            if (FileUpload4.HasFile)
            {
                tasinmaz.KrokiFoto = tasinmaz.Id + ProjeConstants.PARAM_TASINMAZ_KROKIFOTO;
                if (!string.IsNullOrEmpty(tasinmaz.KrokiFoto))
                {
                    exhelper = saveImageFiles2SP(tasinmaz, tasinmaz.KrokiFoto, FileUpload4, exhelper);
                }
            }
            if (FileUpload5.HasFile)
            {
                tasinmaz.TapuFoto = tasinmaz.Id + ProjeConstants.PARAM_TASINMAZ_TAPUFOTO;
                if (!string.IsNullOrEmpty(tasinmaz.TapuFoto))
                {
                    exhelper = saveImageFiles2SP(tasinmaz, tasinmaz.TapuFoto, FileUpload5, exhelper);
                }
            }
            if (FileUpload6.HasFile)
            {
                tasinmaz.TasinmazFoto3 = tasinmaz.Id + ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO3;
                if (!string.IsNullOrEmpty(tasinmaz.TasinmazFoto3))
                {
                    exhelper = saveImageFiles2SP(tasinmaz, tasinmaz.TasinmazFoto3, FileUpload6, exhelper);
                }
            }
            if (FileUpload7.HasFile)
            {
                tasinmaz.TasinmazFoto4 = tasinmaz.Id + ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO4;
                if (!string.IsNullOrEmpty(tasinmaz.TasinmazFoto4))
                {
                    exhelper = saveImageFiles2SP(tasinmaz, tasinmaz.TasinmazFoto4, FileUpload7, exhelper);
                }
            }
            isSaved = tasinmaz.Update();
            if (!isSaved)
            {
                Exception exception = new Exception("Resim Veri Tabanına Kayıt edilemedi");
                exhelper.Exceptions.Add(exception);
            }
            return exhelper;
        }
        private ExceptionHelper saveImageFiles2SP(Tasinmaz tasinmaz, string fotoFile, FileUpload FotoFileBrowser, ExceptionHelper exhelper)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                string imgPath = newUrl + "/../" + ProjeConstants.RESIMLER_TASINMAZ;//SPImageListName
                SPImageListName = ProjeConstants.RESIMLER_TASINMAZ;

                SPWeb web = Microsoft.SharePoint.SPContext.Current.Web;
                SPList listExists = web.Lists.TryGetList(SPImageListName);

                exhelper = UtilityHelper.uploadFile2SP(FotoFileBrowser, "", fotoFile + "", SPImageListName, exhelper, ProjeConstants.RESIM_DIGER_EN, ProjeConstants.RESIM_DIGER_BOY);
            }
            catch (Exception ex)
            {

                exhelper.Exceptions.Add(ex); ;
            }
            return exhelper;

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

        protected void ResimSil1Btn_Click(object sender, EventArgs e)
        {
            ResmiSil(ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO);
        }
        protected void ResimSil2Btn_Click(object sender, EventArgs e)
        {
            ResmiSil(ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO1);
        }
        protected void ResimSil3Btn_Click(object sender, EventArgs e)
        {
            ResmiSil(ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO2);
        }
        protected void ResimSil4Btn_Click(object sender, EventArgs e)
        {
            ResmiSil(ProjeConstants.PARAM_TASINMAZ_TAHKIKATFOTO);
        }
        protected void ResimSil5Btn_Click(object sender, EventArgs e)
        {
            ResmiSil(ProjeConstants.PARAM_TASINMAZ_KROKIFOTO);
        }
        protected void ResimSil6Btn_Click(object sender, EventArgs e)
        {
            ResmiSil(ProjeConstants.PARAM_TASINMAZ_TAPUFOTO);
        } 
        protected void ResimSil7Btn_Click(object sender, EventArgs e)
        {
            ResmiSil(ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO3);
        }
        protected void ResimSil8Btn_Click(object sender, EventArgs e)
        {
            ResmiSil(ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO4);
        }
        private void ResmiSil(string foto)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select(TasinmazIdQS.ConvertToInt());
            if (tasinmaz != null)
            {
                
                
                string imgUrl = "/../" + ProjeConstants.RESIMLER_TASINMAZ + "/" + foto + ".jpg";
                switch (foto)
                {
                    case ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO:
                        {
                            tasinmaz.TasinmazFoto = foto;
                            Image1.ImageUrl = imgUrl;
                            break;
                        }

                    case ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO1:
                        {
                            tasinmaz.TasinmazFoto1 = foto;
                            Image2.ImageUrl = imgUrl;
                            break;
                        }
                    case ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO2:
                        {
                            tasinmaz.TasinmazFoto2 = foto;
                            Image3.ImageUrl = imgUrl;
                            break;
                        }
                    case ProjeConstants.PARAM_TASINMAZ_TAHKIKATFOTO:
                        {
                            tasinmaz.TahkikatFoto = foto;
                            Image4.ImageUrl = imgUrl;
                            break;
                        }
                    case ProjeConstants.PARAM_TASINMAZ_KROKIFOTO:
                        {
                            tasinmaz.KrokiFoto = foto;
                            Image5.ImageUrl = imgUrl;
                            break;
                        }
                    case ProjeConstants.PARAM_TASINMAZ_TAPUFOTO:
                        {
                            tasinmaz.TapuFoto = foto;
                            Image6.ImageUrl = imgUrl;
                            break;
                        }
                    case ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO3:
                        {
                            tasinmaz.TasinmazFoto3 = foto;
                            Image7.ImageUrl = imgUrl;
                            break;
                        }
                    case ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO4:
                        {
                            tasinmaz.TasinmazFoto4 = foto;
                            Image8.ImageUrl = imgUrl;
                            break;
                        }
                    default:
                        break;
                }
                if (tasinmaz.Update())
                {
                    MessageHelper.PublishMessage("Resim Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                }
                else
                {
                    MessageHelper.PublishMessage("Resim Silinemedi", ProjeConstants.MESAJ_HATA);
                    
                }
                ResimleriDoldur();
            }

        }
        #region pdf yukle goster
        private void PDFKaydet(int tasinmazId)
        {
            try
            {
                if (EmlakBeyaniYukleFU.HasFile)
                {
                    string hedefDosyaAdi = ProjeConstants.DOSYA_EMLAKBEYAN_FORMU + tasinmazId + ".pdf";
                    bool isOk = UtilityHelper.UploadFileToSharePoint(EmlakBeyaniYukleFU, ProjeConstants.TBYSBELGELERI_LIB, hedefDosyaAdi);
                    if (isOk)
                    {
                        EmlakBeyaniDosyaLnk.Visible = true;
                        MessageHelper.PublishMessage("Emlak Beyan Formu Yüklendi", ProjeConstants.MESAJ_BASARILI, 2000);
                    }
                }
                else
                {
                    EmlakBeyaniDosyaLnk.Visible = false;
                    EmlakBeyaniSilBtn.Visible = false;
                    MessageHelper.PublishMessage("Lütfen Emlak Beyan Formu yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
                }
                if (YapiKayitYukleFU.HasFile)
                {
                    string hedefDosyaAdi = ProjeConstants.DOSYA_YAPIKAYIT_BELGESI + tasinmazId + ".pdf";
                    bool isOk = UtilityHelper.UploadFileToSharePoint(YapiKayitYukleFU, ProjeConstants.TBYSBELGELERI_LIB, hedefDosyaAdi);
                    if (isOk)
                    {
                        YapiKayitDosyaLnk.Visible = true;
                        MessageHelper.PublishMessage("Yapı Kayıt Belgesi Yüklendi", ProjeConstants.MESAJ_BASARILI, 2000);
                    }
                }
                else
                {
                    YapiKayitDosyaLnk.Visible = false;
                    YapiKayitSilBtn.Visible = false;
                    MessageHelper.PublishMessage("Lütfen Yapı Kayıt Belgesi yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
                }
            }
            catch (Exception exception)
            {
                Exception ex = new Exception("Dosya Yüklenemedi");
                ExceptionHelper exhelper = new ExceptionHelper(exception);
                exhelper.Exceptions.Add(ex);
                exhelper.PublishException();
            }
        }
        private void PDFGoster(int tasinmazId)
        {
            try
            {
                if (tasinmazId >0)
                {
                    string emlakBeyaniDosyaAdi = ProjeConstants.DOSYA_EMLAKBEYAN_FORMU + tasinmazId + ".pdf";
                    string emlakBeyaniDosyaUrl = UtilityHelper.TbysBelgelerURLGetir() + "/" + emlakBeyaniDosyaAdi;
                    bool emlakBeyaniDosyaVarMi = UtilityHelper.DosyaVarMi(UtilityHelper.TbysBelgelerURLGetir(), ProjeConstants.TBYSBELGELERI_LIB, emlakBeyaniDosyaAdi);
                    if (emlakBeyaniDosyaVarMi)
                    {
                        string belgePdfLink = @"'<a class=\'btn btn-secondary\' data-fancybox data-type=pdf data-width=960 data-height=720 href=" + emlakBeyaniDosyaUrl + @"> Belge Görüntüle </a>'";

                        EmlakBeyaniDosyaLnk.Target = "_blank";
                        EmlakBeyaniDosyaLnk.HRef = emlakBeyaniDosyaUrl;

                        EmlakBeyaniDosyaLnk.Visible = true;
                        EmlakBeyaniSilBtn.Visible = true;
                        EmlakBeyaniYukleFU.Visible = false;
                    }
                    else
                    {
                        EmlakBeyaniDosyaLnk.Visible = false;
                        EmlakBeyaniSilBtn.Visible = false;
                        EmlakBeyaniYukleFU.Visible = true;
                        MessageHelper.PublishMessage("Lütfen Emlak Beyan Formunu pdf olarak yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
                    }
                    string yapiKayitDosyaAdi = ProjeConstants.DOSYA_YAPIKAYIT_BELGESI + tasinmazId + ".pdf";
                    string yapiKayitDosyaUrl = UtilityHelper.TbysBelgelerURLGetir() + "/" + yapiKayitDosyaAdi;
                    bool yapiKayitdosyaVarMi = UtilityHelper.DosyaVarMi(UtilityHelper.TbysBelgelerURLGetir(), ProjeConstants.TBYSBELGELERI_LIB, yapiKayitDosyaAdi);
                    if (yapiKayitdosyaVarMi)
                    {
                        string belgePdfLink = @"'<a class=\'btn btn-secondary\' data-fancybox data-type=pdf data-width=960 data-height=720 href=" + yapiKayitDosyaUrl + @"> Belge Görüntüle </a>'";

                        YapiKayitDosyaLnk.Target = "_blank";
                        YapiKayitDosyaLnk.HRef = yapiKayitDosyaUrl;

                        YapiKayitDosyaLnk.Visible = true;
                        YapiKayitSilBtn.Visible = true;
                        YapiKayitYukleFU.Visible = false;
                    }
                    else
                    {
                        YapiKayitDosyaLnk.Visible = false;
                        YapiKayitSilBtn.Visible = false;
                        YapiKayitYukleFU.Visible = true;
                        MessageHelper.PublishMessage("Lütfen Yapı Kayıt Belgesini pdf olarak yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
                    }
                }
                else
                {
                    MessageHelper.PublishMessage("Bağışçı bulunamadı.", ProjeConstants.MESAJ_HATA);
                }

            }
            catch (Exception exception)
            {
                Exception ex = new Exception("PDF Yüklenemedi");
                ExceptionHelper exhelper = new ExceptionHelper(exception);
                exhelper.Exceptions.Add(ex);
                exhelper.PublishException();
            }
        }

        protected void EmlakBeyaniSilBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string dosyaAdi = ProjeConstants.DOSYA_EMLAKBEYAN_FORMU + TasinmazIdQS + ".pdf";
                if (UtilityHelper.DeleteFileFromSharePointLib(ProjeConstants.PATH_TBYS_URL, ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi))
                {
                    MessageHelper.PublishMessage(dosyaAdi + " Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                    EmlakBeyaniSilBtn.Visible = false;
                    EmlakBeyaniDosyaLnk.Visible = false;
                    EmlakBeyaniYukleFU.Visible = true;
                }
                else
                {
                    MessageHelper.PublishMessage(dosyaAdi + " Silinemedi", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper eh = new ExceptionHelper(ex);
                eh.PublishException();
            }
        }
        protected void YapiKayitSilBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string dosyaAdi = ProjeConstants.DOSYA_YAPIKAYIT_BELGESI + TasinmazIdQS + ".pdf";
                if (UtilityHelper.DeleteFileFromSharePointLib(ProjeConstants.PATH_TBYS_URL, ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi))
                {
                    MessageHelper.PublishMessage(dosyaAdi + " Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                    YapiKayitSilBtn.Visible = false;
                    YapiKayitDosyaLnk.Visible = false;
                    YapiKayitYukleFU.Visible = true;
                }
                else
                {
                    MessageHelper.PublishMessage(dosyaAdi + " Silinemedi", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper eh = new ExceptionHelper(ex);
                eh.PublishException();
            }
        }


        #endregion
    }
}
