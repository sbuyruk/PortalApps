using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;


namespace NBYS_WebParts.NakitBagisciEditWP
{
    [ToolboxItemAttribute(false)]
    public partial class NakitBagisciEditWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NakitBagisciEditWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        private string NakitBagisciIdQS
        {
            get
            {

                if (ViewState["NakitBagisciId"] == null)
                {
                    if (Page.Request.QueryString["NakitBagisciId"] != null)
                    {
                        ViewState["NakitBagisciId"] = Page.Request.QueryString["NakitBagisciId"];
                    }
                    else
                    {
                        ViewState["NakitBagisciId"] = string.Empty;
                    }
                }
                return ViewState["NakitBagisciId"].ToString();
            }

            set
            {
                ViewState["NakitBagisciId"] = value;
            }
        }
        private string ParamQS//bagisci birlestirme webparttan gelen dönüste searc texti de götürsün ki aranan texti tekrar arayabilsin
        {
            get
            {

                if (ViewState["Param"] == null)
                {
                    if (Page.Request.QueryString["Param"] != null)
                    {
                        ViewState["Param"] = Page.Request.QueryString["Param"];
                    }
                    else
                    {
                        ViewState["Param"] = string.Empty;
                    }
                }
                return ViewState["Param"].ToString();
            }

            set
            {
                ViewState["Param"] = value;
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

            try
            {
                if (!Page.IsPostBack)
                {
                    FillIlData();
                    FillIlceData();
                    if (!string.IsNullOrEmpty(NakitBagisciIdQS))
                    {
                        NakitBagisciFormunuDoldur();
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void FillIlData()
        {
            if (IliDDL.SelectedItem == null)
            {
                IliDDL.Items.Clear();
                IliDDL.Items.Add(new ListItem("", ProjeConstants.IL_BOS.ToString()));
                Il pIl = new Il();
                List<Il> list = pIl.SelectAll<Il>();
                TextInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true).TextInfo;

                foreach (Il il in list)
                {
                    string UpperCaseIl = culturInfo.ToUpper(il.IlAdi);
                    IliDDL.Items.Add(new ListItem(UpperCaseIl, il.Id.ToString()));
                }
            }

        }
        private void FillIlceData()
        {

            if (IliDDL.SelectedItem != null)
            {
                int ilId = IliDDL.SelectedValue.ConvertToInt();
                IlcesiDDL.Items.Clear();
                IlcesiDDL.Items.Add(new ListItem("", ProjeConstants.ILCE_BOS.ToString()));
                Ilce pIlce = new Ilce();
                List<Ilce> list = pIlce.SelectByIlId(ilId);
                foreach (Ilce ilce in list)
                {
                    IlcesiDDL.Items.Add(new ListItem(ilce.IlceAdi, ilce.Id.ToString()));
                }
            }


        }
        private void NakitBagisciFormunuDoldur()
        {
            KaydetBtn.Attributes["Class"] = "btn btn-outline-primary";
            KaydetBtn.Text = "Güncelle";
            if (!string.IsNullOrEmpty(NakitBagisciIdQS))
            {
                int nakitBagisciId = NakitBagisciIdQS.ConvertToInt();

                NakitBagisci nakitBagisci = new NakitBagisci();
                nakitBagisci = nakitBagisci.Select<NakitBagisci>(nakitBagisciId);
                if (nakitBagisci != null)
                {
                    IdLbl.Text = nakitBagisciId.ToString();
                    AdiTxt.Text = nakitBagisci.Adi.ReturnEmptyIfNull().ToString();
                    TCKimlikNoTxt.Text = nakitBagisci.TCKimlikNo.ReturnEmptyIfNull().ToString();
                    AdresTxt.Text = nakitBagisci.Adres.ReturnEmptyIfNull().ToString();

                    //TextInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true).TextInfo;
                    //string upperCaseIl = culturInfo.ToUpper(nakitBagisci.Ili.ReturnEmptyIfNull().ToString());
                    string ilstr = nakitBagisci.Ili.ReturnEmptyIfNull().ToString();
                    if (IliDDL.Items.FindByValue(ilstr) != null)
                    {
                        IliDDL.SelectedValue = IliDDL.Items.FindByValue(ilstr).Value;
                    }
                    FillIlceData();
                    string ilcestr = nakitBagisci.Ilcesi.ReturnEmptyIfNull().ToString();
                    if (IlcesiDDL.Items.FindByValue(ilcestr) != null)
                    {
                        IlcesiDDL.SelectedValue = IlcesiDDL.Items.FindByValue(ilcestr).Value;
                    }
                    Telefon1Txt.Text = nakitBagisci.Telefon1.ReturnEmptyIfNull().ToString();
                    Telefon2Txt.Text = nakitBagisci.Telefon2.ReturnEmptyIfNull().ToString();
                    EPostaTxt.Text = nakitBagisci.Eposta.ReturnEmptyIfNull().ToString();
                    PostaKoduTxt.Text = nakitBagisci.PostaKodu.ReturnEmptyIfNull().ToString();
                    TuzelKisiChk.Checked = nakitBagisci.TuzelKisi.ConvertToBool();
                    SagChk.Checked = nakitBagisci.Sag.ConvertToBool();
                    AciklamaTxt.Text = nakitBagisci.Aciklama.ReturnEmptyIfNull().ToString();
                    UlasilamiyorChk.Checked = nakitBagisci.Ulasilamiyor.ConvertToBool();
                    BelgeIstemiyorChk.Checked = nakitBagisci.BelgeIstemiyor.ConvertToBool();
                    DergiGonderilmesinChk.Checked = nakitBagisci.DergiGonderilmesin.ConvertToBool();
                }

            }

        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool isSaved = false;
                if (string.IsNullOrWhiteSpace(AdiTxt.Text))
                {
                    //System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                    //    typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "Alert.danger('Bağışçı Adı boş olamaz. Lütfen bağışçı adını giriniz.')", true);
                    MessageHelper.PublishMessage("Bağışçı Adı boş olamaz. Lütfen bağışçı adını giriniz.", ProjeConstants.MESAJ_HATA);
                }
                else
                {
                    int nakitBagisciId = NakitBagisciIdQS.ConvertToInt();
                    NakitBagisci nakitBagisci = new NakitBagisci();

                    if (nakitBagisciId == 0)
                    {
                        isSaved = saveNakitBagisci(nakitBagisci);
                    }
                    else
                    {
                        nakitBagisci = nakitBagisci.Select<NakitBagisci>(nakitBagisciId);
                        isSaved = updateNakitBagisci(nakitBagisci);
                    }

                }

                if (isSaved)
                {
                    //System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "Alert.success('Nakit Bağışçı Bilgisi Kaydedildi.')", true);
                    MessageHelper.PublishMessage("Nakit Bağışçı Bilgisi Kaydedildi.", ProjeConstants.MESAJ_BASARILI, 4000);

                }
                else
                {
                    //System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "Alert.danger('Nakit Bağışçı Bilgisi Kaydedilemedi.')", true);
                    MessageHelper.PublishMessage("Nakit Bağışçı Bilgisi Kaydedilemedi.", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

        }
        private bool saveNakitBagisci(NakitBagisci nakitBagisci)
        {
            bool isSaved = false;
            nakitBagisci.Adi = AdiTxt.Text.ReturnEmptyIfNull().ToString();
            nakitBagisci.TCKimlikNo = TCKimlikNoTxt.Text.ConvertToLong();
            nakitBagisci.Adres = AdresTxt.Text.ReturnEmptyIfNull().ToString();
            nakitBagisci.Ili = IliDDL.SelectedItem.Value.ConvertToInt().ToString();
            nakitBagisci.Ilcesi = IlcesiDDL.SelectedItem.Value.ConvertToInt().ToString();
            nakitBagisci.Telefon1 = Telefon1Txt.Text.ReturnEmptyIfNull().ToString();
            nakitBagisci.Telefon2 = Telefon2Txt.Text.ReturnEmptyIfNull().ToString();
            nakitBagisci.Eposta = EPostaTxt.Text.ReturnEmptyIfNull().ToString();
            nakitBagisci.PostaKodu = PostaKoduTxt.Text.ReturnEmptyIfNull().ToString();
            if (nakitBagisci.TuzelKisi != TuzelKisiChk.Checked)
            {
                nakitBagisci.TuzelKisi = TuzelKisiChk.Checked;
            }

            nakitBagisci.Sag = SagChk.Checked;
            nakitBagisci.Aciklama = AciklamaTxt.Text.ReturnEmptyIfNull().ToString();
            nakitBagisci.Ulasilamiyor = UlasilamiyorChk.Checked;
            nakitBagisci.BelgeIstemiyor = BelgeIstemiyorChk.Checked;
            nakitBagisci.DergiGonderilmesin = DergiGonderilmesinChk.Checked;
            var user = UtilityHelper.GetCurrentUser();
            nakitBagisci.Degistiren = user;

            nakitBagisci.Id = nakitBagisci.Save();
            if (nakitBagisci.Id > 0)
                isSaved = true;
            return isSaved;
        }
        private bool updateNakitBagisci(NakitBagisci nakitBagisci)
        {
            bool statusChanged = false;
            bool isSaved = false;
            nakitBagisci.Adi = AdiTxt.Text.ReturnEmptyIfNull().ToString();
            nakitBagisci.TCKimlikNo = TCKimlikNoTxt.Text.ConvertToLong();
            nakitBagisci.Adres = AdresTxt.Text.ReturnEmptyIfNull().ToString();
            nakitBagisci.Ili = IliDDL.SelectedItem.Value.ConvertToInt().ToString();
            nakitBagisci.Ilcesi = IlcesiDDL.SelectedItem.Value.ConvertToInt().ToString();
            nakitBagisci.Telefon1 = Telefon1Txt.Text.ReturnEmptyIfNull().ToString();
            nakitBagisci.Telefon2 = Telefon2Txt.Text.ReturnEmptyIfNull().ToString();
            nakitBagisci.Eposta = EPostaTxt.Text.ReturnEmptyIfNull().ToString();
            nakitBagisci.PostaKodu = PostaKoduTxt.Text.ReturnEmptyIfNull().ToString();
            if (nakitBagisci.TuzelKisi != TuzelKisiChk.Checked)
            {
                nakitBagisci.TuzelKisi = TuzelKisiChk.Checked;
                statusChanged = true;
            }

            nakitBagisci.Sag = SagChk.Checked;
            nakitBagisci.Aciklama = AciklamaTxt.Text.ReturnEmptyIfNull().ToString();
            nakitBagisci.Ulasilamiyor = UlasilamiyorChk.Checked;
            nakitBagisci.BelgeIstemiyor = BelgeIstemiyorChk.Checked;
            nakitBagisci.DergiGonderilmesin = DergiGonderilmesinChk.Checked;
            var user = UtilityHelper.GetCurrentUser();
            nakitBagisci.Degistiren = user;

            isSaved = nakitBagisci.Update();
            if (statusChanged)
            {
                Armagan armagan = new Armagan();
                var armaganList = armagan.SelectByBagisciIdAndDurum(nakitBagisci.Id, ProjeConstants.DURUM_GONDERILMEDI);//bu bagisci üzerinde gönderilmemiş armagan öğesi varsa tekrar hesaplanması gerekir. Çünkü Tüzel kişi ve özel kişinin armagan aralıkları farklıdır
                foreach (Armagan item in armaganList)
                {
                    EkstreAktarma.SaveArmagan(item.Tarih, nakitBagisci.TuzelKisi, item.BagisciId, 0, user);
                }

            }
            return isSaved;
        }
        protected void NakitBagisciListesiBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));

                if (string.IsNullOrEmpty(SenderAppQS))
                {
                    newUrl += "/" + ProjeConstants.PAGE_NAKITBAGISCI_LIST + "?NakitBagisciId=" + NakitBagisciIdQS + "&SecilenId=" + NakitBagisciIdQS;
                }
                else if (SenderAppQS.Equals("NBL"))
                {
                    newUrl += "/" + ProjeConstants.PAGE_NAKITBAGISCI_LIST + "?NakitBagisciId=" + NakitBagisciIdQS + "&SecilenId=" + NakitBagisciIdQS;
                }
                else if (SenderAppQS.Equals("BB"))
                {
                    newUrl += "/" + ProjeConstants.PAGE_BAGISCI_BIRLESTIRME + "?NakitBagisciId=" + NakitBagisciIdQS + "&Param=" + ParamQS + "&SecilenId=" + NakitBagisciIdQS;
                }
                else if (SenderAppQS.Equals("NBB"))
                {
                    newUrl += "/" + ProjeConstants.PAGE_NAKITBAGISCI_BULMA + "?NakitBagisciId=" + NakitBagisciIdQS + "&Param=" + ParamQS + "&SecilenId=" + NakitBagisciIdQS;
                }
                else if (SenderAppQS.Equals("NBAL"))
                {

                    int index = currentUrl.LastIndexOf("?") < 0 ? currentUrl.Length : currentUrl.LastIndexOf("?");
                    string queryString = currentUrl.Substring(index + 1, currentUrl.Length - index - 1);
                    newUrl += "/" + ProjeConstants.PAGE_NAKITBAGISCI_ADRESLIST + "?Param=" + ParamQS + "&PageIndex=" + PageIndexQS + "&" + queryString;
                }

                Page.Response.Redirect(newUrl, true);
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
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillIlceData();
        }
    }
}
