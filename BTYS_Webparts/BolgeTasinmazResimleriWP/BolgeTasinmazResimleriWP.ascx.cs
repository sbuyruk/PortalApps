using Model.Ortak;
using Model.TBYS;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace BTYS_Webparts.BolgeTasinmazResimleriWP
{
    [ToolboxItemAttribute(false)]
    public partial class BolgeTasinmazResimleriWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BolgeTasinmazResimleriWP()
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
                


                string newUrl = UtilityHelper.TbysURLGetir() + "/" + ProjeConstants.RESIMLER_TASINMAZ + "/";
                string imageFileName1 = (string.IsNullOrEmpty(tasinmaz.TasinmazFoto) ? ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO : tasinmaz.TasinmazFoto) + ".jpg";
                string imageFileName2 = (string.IsNullOrEmpty(tasinmaz.TasinmazFoto1) ? ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO1 : tasinmaz.TasinmazFoto1) + ".jpg";
                string imageFileName3 = (string.IsNullOrEmpty(tasinmaz.TasinmazFoto2) ? ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO2 : tasinmaz.TasinmazFoto2) + ".jpg";
                string imageFileName4 = (string.IsNullOrEmpty(tasinmaz.TahkikatFoto) ? ProjeConstants.PARAM_TASINMAZ_TAHKIKATFOTO : tasinmaz.TahkikatFoto) + ".jpg";
                string imageFileName5 = (string.IsNullOrEmpty(tasinmaz.KrokiFoto) ? ProjeConstants.PARAM_TASINMAZ_KROKIFOTO : tasinmaz.KrokiFoto) + ".jpg";
                string imageFileName6 = (string.IsNullOrEmpty(tasinmaz.TapuFoto) ? ProjeConstants.PARAM_TASINMAZ_TAPUFOTO : tasinmaz.TapuFoto) + ".jpg";
                string imageFileName7 = (string.IsNullOrEmpty(tasinmaz.TasinmazFoto3) ? ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO : tasinmaz.TasinmazFoto3) + ".jpg";
                string imageFileName8 = (string.IsNullOrEmpty(tasinmaz.TasinmazFoto4) ? ProjeConstants.PARAM_TASINMAZ_TASINMAZFOTO : tasinmaz.TasinmazFoto4) + ".jpg";

                string imgUrl1 = newUrl + imageFileName1;
                string imgUrl2 = newUrl + imageFileName2;
                string imgUrl3 = newUrl + imageFileName3;
                string imgUrl4 = newUrl + imageFileName4;
                string imgUrl5 = newUrl + imageFileName5;
                string imgUrl6 = newUrl + imageFileName6;
                string imgUrl7 = newUrl + imageFileName7;
                string imgUrl8 = newUrl + imageFileName8;
                
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

      
        #region pdf yukle goster
        private void PDFGoster(int tasinmazId)
        {
            try
            {
                if (tasinmazId > 0)
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
                    }
                    else
                    {
                        EmlakBeyaniDosyaLnk.Visible = false;
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
                    }
                    else
                    {
                        YapiKayitDosyaLnk.Visible = false;
                    }
                    string tapuKayitDosyaAdi = ProjeConstants.DOSYA_TAPUKAYIT_BELGESI + tasinmazId + ".pdf";
                    string tapuKayitDosyaUrl = UtilityHelper.TbysBelgelerURLGetir() + "/" + tapuKayitDosyaAdi;
                    bool tapuKayitdosyaVarMi = UtilityHelper.DosyaVarMi(UtilityHelper.TbysBelgelerURLGetir(), ProjeConstants.TBYSBELGELERI_LIB, tapuKayitDosyaAdi);
                    if (tapuKayitdosyaVarMi)
                    {
                        string belgePdfLink = @"'<a class=\'btn btn-secondary\' data-fancybox data-type=pdf data-width=960 data-height=720 href=" + tapuKayitDosyaUrl + @"> Belge Görüntüle </a>'";

                        TapuKayitDosyaLnk.Target = "_blank";
                        TapuKayitDosyaLnk.HRef = tapuKayitDosyaUrl;

                        TapuKayitDosyaLnk.Visible = true;
                    }
                    else
                    {
                        TapuKayitDosyaLnk.Visible = false;
                    }
                    string imarDurumuDosyaAdi = ProjeConstants.DOSYA_IMARDURUMU_BELGESI + tasinmazId + ".pdf";
                    string imarDurumuDosyaUrl = UtilityHelper.TbysBelgelerURLGetir() + "/" + imarDurumuDosyaAdi;
                    bool imarDurumudosyaVarMi = UtilityHelper.DosyaVarMi(UtilityHelper.TbysBelgelerURLGetir(), ProjeConstants.TBYSBELGELERI_LIB, imarDurumuDosyaAdi);
                    if (imarDurumudosyaVarMi)
                    {
                        string belgePdfLink = @"'<a class=\'btn btn-secondary\' data-fancybox data-type=pdf data-width=960 data-height=720 href=" + imarDurumuDosyaUrl + @"> Belge Görüntüle </a>'";

                        ImarDurumuDosyaLnk.Target = "_blank";
                        ImarDurumuDosyaLnk.HRef = imarDurumuDosyaUrl;

                        ImarDurumuDosyaLnk.Visible = true;
                    }
                    else
                    {
                        ImarDurumuDosyaLnk.Visible = false;
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

        


        #endregion
    }
}
