using Model.Ortak;
using Model.TBYS;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.EnvanterdenCikarmaWP
{
    [ToolboxItemAttribute(false)]
    public partial class EnvanterdenCikarmaWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public EnvanterdenCikarmaWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
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
            if (!Page.IsPostBack)
            {
                Tasinmaz tasinmaz = null;
                if (String.IsNullOrEmpty(DestinationAppQS) || String.Equals(DestinationAppQS, ""))
                {
                    tasinmaz = new Tasinmaz();
                    tasinmaz = tasinmaz.Select<Tasinmaz>(TasinmazIdQS.ConvertToInt());
                    if (tasinmaz != null)
                    {
                        //envanterden çikar btn yi visible yap
                        //
                        UpdateBtn.Visible = false;
                        EnvanterdenCikarBtn.Visible = true;
                    }
                }
                else if (String.Equals(DestinationAppQS, "ECD"))
                {
                    tasinmaz = new Tasinmaz();
                    tasinmaz = tasinmaz.SelectEnvanterdenCikanTasinmaz(TasinmazIdQS.ConvertToInt());
                    if (tasinmaz != null)
                    {
                        UpdateBtn.Visible = true;
                        EnvanterdenCikarBtn.Visible = false;

                    }

                }
                EnvanterdenCikarmaFormunuDuzenle(tasinmaz);
            }
        }
        private bool EnvanterdenCikarmaFormunuDuzenle(Tasinmaz tasinmaz)
        {
            bool formDolduMu = false;
            try
            {
                if (tasinmaz != null)
                {
                    AdiLbl.Text = tasinmaz.KullanimSekli + " - " + tasinmaz.Ilcesi + "/" + tasinmaz.Ili;
                    KullanimSekliLbl.Text = tasinmaz.KullanimSekli + " - " + tasinmaz.MulkiyetSekli + " - " + tasinmaz.KiraDurumu;
                    AdresLbl.Text = tasinmaz.Adres;
                    Il_IlceLbl.Text = tasinmaz.Ilcesi + " / " + tasinmaz.Ili;
                    EnvanterdenCikarmaSebebiDDLDoldur();
                    UtilityHelper.SetDDLValue(CikarmaSebebiDDL, tasinmaz.EnvanterdenCikmaSebebi);
                    BedelTxt.Value = tasinmaz.EnvanterdenCikmaBedeli.ReturnZeroIfNull().ToString();
                    EnvanterdenCikmaTarTxt.Value = tasinmaz.EnvanterdenCikmaTarihi.ConvertToDatetimeEmptyIfNull();
                    AciklamaTxt.Text = tasinmaz.Aciklama;
                    formDolduMu = true;
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exh = new ExceptionHelper(exception);
                formDolduMu = false;
            }
            return formDolduMu;
        }
        private void EnvanterdenCikarmaSebebiDDLDoldur()
        {
            CikarmaSebebiDDL.Items.Clear();
            CikarmaSebebiDDL.Items.Add(ProjeConstants.ENVANTERDEN_CIKARMA_KAMU);
            CikarmaSebebiDDL.Items.Add(ProjeConstants.ENVANTERDEN_CIKARMA_SATIS);
            CikarmaSebebiDDL.Items.Add(ProjeConstants.ENVANTERDEN_CIKARMA_INTIFA_HT);
            CikarmaSebebiDDL.Items.Add(ProjeConstants.ENVANTERDEN_CIKARMA_PLAN_DEG);
            CikarmaSebebiDDL.Items.Add(ProjeConstants.ENVANTERDEN_CIKARMA_TAPU_BD);
            CikarmaSebebiDDL.Items.Add(ProjeConstants.ENVANTERDEN_CIKARMA_KAT_KI);
            CikarmaSebebiDDL.Items.Add(ProjeConstants.ENVANTERDEN_CIKARMA_DIGER);

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
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            if (SenderAppQS.Equals("TD"))
                RedirectToPage(ProjeConstants.PAGE_TASINMAZ_GIRIS + "?EnvanterdeMi=0&TasinmazId=" + TasinmazIdQS + "&DestinationApp=TD");
            else if (SenderAppQS.Equals("STL"))
                RedirectToPage(ProjeConstants.PAGE_ENVANTERDENCIKANTASINMAZ_LIST + "?TasinmazId=" + TasinmazIdQS);

        }
        protected void EnvanterdenCikarBtn_Click(object sender, EventArgs e)
        {

            try
            {
                //Onay Popup Aç

                EnvanterdenCikarNowBtn.Visible = true;
                var openPopup = "OpenModal();";
                UtilityHelper.ScriptCalistir(openPopup);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void UpdateBtn_Click(object sender, EventArgs e)
        {

            try
            {
                Tasinmaz tasinmaz = new Tasinmaz();
                tasinmaz = tasinmaz.SelectEnvanterdenCikanTasinmaz(TasinmazIdQS.ConvertToInt());
                tasinmaz.EnvanterdeMi = ProjeConstants.TASINMAZ_ENVANTERDEN_CIKTI;//
                tasinmaz.EnvanterdenCikmaSebebi = CikarmaSebebiDDL.SelectedItem.Value;
                tasinmaz.EnvanterdenCikmaBedeli = BedelTxt.Value.ConvertToDecimal();
                tasinmaz.EnvanterdenCikmaTarihi = EnvanterdenCikmaTarTxt.Value.ConvertToDatetime();
                Bagis bagis = new Bagis();
                bagis = bagis.SelectByTasinmazId(tasinmaz.Id);
                if (bagis != null)
                {
                    tasinmaz.BagisciId = bagis.BagisciId;
                }
                tasinmaz.Aciklama = AciklamaTxt.Text;
                bool guncellendiMi = tasinmaz.Update();
                if (guncellendiMi)
                {
                    UpdateBtn.Visible = true;
                    EnvanterdenCikarBtn.Visible = false;
                    MessageHelper.PublishMessage("(Envanterde olmayan) Tasinmaz Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                Exception guncellemeExc = new Exception("(Envanterde olmayan) Tasinmaz Güncellenemedi1");
                exHelper.PublishException();
            }
            finally {
                var closePopup = "CloseModal();";
                UtilityHelper.ScriptCalistir(closePopup);
            }
        }
        protected void EnvanterdenCikarNowBtn_Click(object sender, EventArgs e)
        {
            //tasinmaz.EnvanterdeMi=0 yap
            //EnvanterdenCikarmaWP'i reload et SenderApp
            //envanterden çikarildi mesaji ver
            bool envanterdenCikarildiMi = false;
            bool bagisGuncellendiMi = false;
            try
            {
                Tasinmaz tasinmaz = new Tasinmaz();
                tasinmaz = tasinmaz.Select<Tasinmaz>(TasinmazIdQS.ConvertToInt());
                tasinmaz.EnvanterdeMi = ProjeConstants.TASINMAZ_ENVANTERDEN_CIKTI;//false;
                tasinmaz.EnvanterdenCikmaSebebi = CikarmaSebebiDDL.SelectedItem.Value;
                tasinmaz.EnvanterdenCikmaBedeli = BedelTxt.Value.ConvertToDecimal();
                tasinmaz.EnvanterdenCikmaTarihi = EnvanterdenCikmaTarTxt.Value.ConvertToDatetime();
                tasinmaz.Degistiren = UtilityHelper.GetCurrentUserLoginName();
                Bagis bagis = new Bagis();
                bagis = bagis.SelectByTasinmazId(tasinmaz.Id);
                if (bagis != null)
                {
                    tasinmaz.BagisciId = bagis.BagisciId;
                    bagis.Envanterde = ProjeConstants.ENVANTERDEN_CIKTI;
                    bagis.Degistiren = UtilityHelper.GetCurrentUserLoginName();
                    
                }
                tasinmaz.Aciklama = AciklamaTxt.Text;
                envanterdenCikarildiMi = tasinmaz.Update();

                if (envanterdenCikarildiMi)
                {
                    if (bagis != null)
                    {
                        bagisGuncellendiMi = bagis.Update();
                    }
                    RedirectToPage(ProjeConstants.PAGE_ENVANTERDENCIKANTASINMAZ_LIST + "?SecilenId=" + TasinmazIdQS);
                }
                else
                {
                    MessageHelper.PublishMessage("Envanterden çikarma islemi basarisiz oldu", ProjeConstants.MESAJ_HATA, 2000);
                }
            }
            catch (Exception ex)
            {

                ExceptionHelper eh= new ExceptionHelper(ex);
                eh.PublishException();
            }


        }
    }
}
