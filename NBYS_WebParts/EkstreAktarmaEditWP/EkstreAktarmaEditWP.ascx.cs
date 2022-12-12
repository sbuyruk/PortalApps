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

namespace NBYS_WebParts.EkstreAktarmaEditWP
{
    [ToolboxItemAttribute(false)]
    public partial class EkstreAktarmaEditWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public EkstreAktarmaEditWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string EkstreAktarmaIdQS
        {
            get
            {
                if (ViewState["EkstreAktarmaId"] == null)
                {
                    if (Page.Request.QueryString["EkstreAktarmaId"] != null)
                    {
                        ViewState["EkstreAktarmaId"] = Page.Request.QueryString["EkstreAktarmaId"];
                    }
                    else
                    {
                        ViewState["EkstreAktarmaId"] = string.Empty;
                    }
                }
                return ViewState["EkstreAktarmaId"].ToString();
            }

            set
            {
                ViewState["EkstreAktarmaId"] = value;
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
        private string SecilenIlQS
        {
            get
            {

                if (ViewState["SecilenIl"] == null)
                {
                    if (Page.Request.QueryString["SecilenIl"] != null)
                    {
                        ViewState["SecilenIl"] = Page.Request.QueryString["SecilenIl"];
                    }
                    else
                    {
                        ViewState["SecilenIl"] = string.Empty;
                    }
                }
                return ViewState["SecilenIl"].ToString();
            }

            set
            {
                ViewState["SecilenIl"] = value;
            }
        }
        private string BankaQS
        {
            get
            {
                if (ViewState["Banka"] == null)
                {
                    if (Page.Request.QueryString["Banka"] != null)
                    {
                        ViewState["Banka"] = Page.Request.QueryString["Banka"];
                    }
                    else
                    {
                        ViewState["Banka"] = BankaDDL.SelectedItem.Value;
                    }
                }
                return ViewState["Banka"].ToString();
            }

            set
            {
                ViewState["Banka"] = value;
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
        private string ParamQS//nakit bagisci düzenlemeden dönüyorsa aranan texti tekrar arasın
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
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    FillBanka();
                    FillIlData();
                    FillIlceData();
                    if (string.IsNullOrEmpty(EkstreAktarmaIdQS))
                    {
                        OpenGiris();
                    }
                    else
                    {
                        OpenDuzenle();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void OpenDuzenle()
        {
            //CardHeader.Attributes["Class"] = "btn-primary";
            KaydetBtn.CssClass = "btn btn-outline-primary float-left";
            KaydetBtn.Text = "Güncelle";
            if (SenderAppQS.Equals("EkstreListesi") || SenderAppQS.Equals("NBE"))
            {
                EslestirBtn.Visible = true;
            }
            else
            {
                EslestirBtn.Visible = false;
            }

            FillExtreAktarmaForm();
            FillNakitBagisciBilgileri();
        }
        private void OpenGiris()
        {
            //CardHeader.Attributes["Class"] = "btn-success";
            KaydetBtn.CssClass = "btn btn-outline-success float-left";
            KaydetBtn.Text = "Kaydet";
            DateTime today = DateTime.Now;
            IslemTarihiTxt.Text = today.ToString(ProjeConstants.DATE_TR);
            NakitBagisciIdLbl.Text = "0";
            FillNakitBagisciBilgileri();
        }
        private void FillNakitBagisciBilgileri()
        {
            if (!string.IsNullOrEmpty(NakitBagisciIdQS))
            {
                long ektreAktarmaTCKimlikNo = 0;
                EkstreAktarma ekstreAktarma = new EkstreAktarma();
                ekstreAktarma = ekstreAktarma.Select<EkstreAktarma>(EkstreAktarmaIdQS.ConvertToInt());
                ektreAktarmaTCKimlikNo = ekstreAktarma == null ? 0 : ekstreAktarma.TCKimlikNo.ReturnZeroIfNull().ConvertToInt();
                NakitBagisci nb = new NakitBagisci();
                nb = nb.Select<NakitBagisci>(NakitBagisciIdQS.ConvertToInt());
                if (nb != null)
                {
                    NakitBagisciIdLbl.Text = nb.Id.ToString();
                    AdiTxt.Text = nb.Adi;
                    AdiTxt.Enabled = false;
                    TCKimlikNoTxt.Text = (nb.TCKimlikNo < 1) && ektreAktarmaTCKimlikNo.ConvertToLong() > 0 ? ektreAktarmaTCKimlikNo.ReturnZeroIfNull().ToString() : nb.TCKimlikNo.ReturnEmptyIfZeroOrNull().ToString();
                    TCKimlikNoTxt.Enabled = false;
                    AdresTxt.Text = nb.Adres;
                    Telefon1Txt.Text = nb.Telefon1;
                    Telefon2Txt.Text = nb.Telefon2;
                    TuzelKisiChk.Checked = nb.TuzelKisi;
                    string ilstr = nb.Ili;
                    if (IliDDL.Items.FindByValue(ilstr) != null)
                    {
                        IliDDL.SelectedValue = IliDDL.Items.FindByValue(ilstr).Value;
                    }
                    FillIlceData();
                    string ilcestr = nb.Ilcesi;
                    if (IlcesiDDL.Items.FindByValue(ilcestr) != null)
                    {
                        IlcesiDDL.SelectedValue = IlcesiDDL.Items.FindByValue(ilcestr).Value;
                    }
                }
            }
        }
        private void FillExtreAktarmaForm()
        {
            int ekstreAktarmaId = EkstreAktarmaIdQS.ConvertToInt();
            EkstreAktarma ekstreAktarma = new EkstreAktarma();
            ekstreAktarma = ekstreAktarma.Select<EkstreAktarma>(ekstreAktarmaId);

            NakitBagisciIdLbl.Text = ekstreAktarma.NakitBagisciId.ToString();
            AdiTxt.Text = ekstreAktarma.Adi.ReturnEmptyIfNull().ToString().TrimStart().TrimEnd();
            TCKimlikNoTxt.Text = ekstreAktarma.TCKimlikNo.ReturnEmptyIfNull().ToString();
            Telefon1Txt.Text = ekstreAktarma.Telefon1.ReturnEmptyIfNull().ToString();
            Telefon2Txt.Text = ekstreAktarma.Telefon2.ReturnEmptyIfNull().ToString();
            AdresTxt.Text = ekstreAktarma.Adres.ReturnEmptyIfNull().ToString();
            //IliTxt.Text = ekstreAktarma.Ili.ReturnEmptyIfNull().ToString();
            //IlcesiTxt.Text = ekstreAktarma.Ilcesi.ReturnEmptyIfNull().ToString();
            TextInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true).TextInfo;
            //string ilstr = ekstreAktarma.Ili.ReturnEmptyIfNull().ToString();
            string ilstr = culturInfo.ToUpper(ekstreAktarma.Ili.ReturnEmptyIfNull().ToString());
            if (IliDDL.Items.FindByText(ilstr) != null)
            {
                IliDDL.SelectedValue = IliDDL.Items.FindByText(ilstr).Value;
            }
            FillIlceData();
            string ilcestr = culturInfo.ToUpper(ekstreAktarma.Ilcesi.ReturnEmptyIfNull().ToString());
            if (IlcesiDDL.Items.FindByText(ilcestr) != null)
            {
                IlcesiDDL.SelectedValue = IlcesiDDL.Items.FindByText(ilcestr).Value;
            }
            //BankaTxt.Text = ekstreAktarma.BankaAdi.ReturnEmptyIfNull().ToString();
            string bankaStr = ekstreAktarma.BankaAdi.ReturnEmptyIfNull().ToString();//culturInfo.ToUpper(ekstreAktarma.BankaAdi.ReturnEmptyIfNull().ToString()); 
            if (BankaDDL.Items.FindByText(bankaStr) != null)
            {
                BankaDDL.SelectedValue = BankaDDL.Items.FindByText(bankaStr).Value;
            }
            EPostaTxt.Text = ekstreAktarma.Eposta.ReturnEmptyIfNull().ToString();
            PostaKoduTxt.Text = ekstreAktarma.PostaKodu.ReturnEmptyIfNull().ToString();
            TutarTxt.Text = ekstreAktarma.Tutar.ReturnEmptyIfNull().ToString();
            FisNoTxt.Text = ekstreAktarma.FisNo.ReturnEmptyIfNull().ToString();
            TuzelKisiChk.Checked = ekstreAktarma.TuzelKisi.ConvertToBool();
            BelgeIstemiyorChk.Checked = ekstreAktarma.BelgeIstemiyor.ConvertToBool();
            IslemTarihiTxt.Text = ekstreAktarma.IslemTarihi.ToString(ProjeConstants.DATE_TR);
            BagisTarihiTxt.Value = ekstreAktarma.BagisTarihi.ToString(ProjeConstants.DATE_TR);
            AciklamaTxt.Text = ekstreAktarma.Aciklama.ReturnEmptyIfNull().ToString();
            NakitBagisciIdLbl.Text = ekstreAktarma.NakitBagisciId.ReturnZeroIfNull().ToString();
        }
        private void ClearExtreAktarmaForm()
        {

            AdiTxt.Text = string.Empty;
            TCKimlikNoTxt.Text = string.Empty;
            Telefon1Txt.Text = string.Empty;
            Telefon2Txt.Text = string.Empty;
            AdresTxt.Text = string.Empty;
            EPostaTxt.Text = string.Empty;
            PostaKoduTxt.Text = string.Empty;
            TutarTxt.Text = string.Empty;
            FisNoTxt.Text = string.Empty;
            TuzelKisiChk.Checked = false;
            BelgeIstemiyorChk.Checked = false;
            IslemTarihiTxt.Text = DateTime.Today.ToString(ProjeConstants.DATE_TR);
            // BagisTarihiTxt.Value = DateTime.Today.ToString(ProjeConstants.DATE_TR);
            AciklamaTxt.Text = string.Empty;
            BagisTarihiTxt.Value = string.Empty;
            if (BankaDDL.Items.FindByValue(ProjeConstants.BANKA_BOS_INT.ToString()) != null)
                BankaDDL.SelectedValue = BankaDDL.Items.FindByValue(ProjeConstants.BANKA_BOS_INT.ToString()).Value;
            if (IliDDL.Items.FindByValue(ProjeConstants.IL_BOS.ToString()) != null)
                IliDDL.SelectedValue = IliDDL.Items.FindByValue(ProjeConstants.IL_BOS.ToString()).Value;
            if (IlcesiDDL.Items.FindByValue(ProjeConstants.ILCE_BOS.ToString()) != null)
                IlcesiDDL.SelectedValue = IlcesiDDL.Items.FindByValue(ProjeConstants.ILCE_BOS.ToString()).Value;
            NakitBagisciIdLbl.Text = "0";
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
            if (IliDDL.Items.FindByValue(ProjeConstants.IL_BOS.ToString()) != null)
                IliDDL.SelectedValue = IliDDL.Items.FindByValue(ProjeConstants.IL_BOS.ToString()).Value;
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
                TextInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true).TextInfo;
                foreach (Ilce ilce in list)
                {
                    string UpperCaseIlce = culturInfo.ToUpper(ilce.IlceAdi);
                    IlcesiDDL.Items.Add(new ListItem(UpperCaseIlce, ilce.Id.ToString()));
                }
            }
            if (IlcesiDDL.Items.FindByValue(ProjeConstants.ILCE_BOS.ToString()) != null)
                IlcesiDDL.SelectedValue = IlcesiDDL.Items.FindByValue(ProjeConstants.ILCE_BOS.ToString()).Value;

        }
        private void FillBanka()
        {

            if (BankaDDL.SelectedItem == null)
            {
                BankaDDL.Items.Clear();
                BankaTanim pBanka = new BankaTanim();
                List<BankaTanim> list = pBanka.SelectAll<BankaTanim>();
                //TextInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true).TextInfo;
                foreach (BankaTanim banka in list)
                {
                    //string UpperCaseBanka = culturInfo.ToUpper(banka.Banka);
                    BankaDDL.Items.Add(new ListItem(banka.Banka, banka.Id.ToString()));
                }
            }
            if (BankaDDL.Items.FindByValue(ProjeConstants.BANKA_BOS_INT.ToString()) != null)
                BankaDDL.SelectedValue = BankaDDL.Items.FindByValue(ProjeConstants.BANKA_BOS_INT.ToString()).Value;
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
                    int ekstreAktarmaId = EkstreAktarmaIdQS.ConvertToInt();
                    EkstreAktarma ekstreAktarma = new EkstreAktarma();
                    ekstreAktarma = ekstreAktarma.Select<EkstreAktarma>(ekstreAktarmaId);
                    if (ekstreAktarma == null || ekstreAktarmaId == 0)
                    {
                        isSaved = SaveEkstreAktarma();
                        if (isSaved)
                        {
                            ClearExtreAktarmaForm();
                            FillBanka();
                            FillIlData();
                        }
                    }
                    else
                    {
                        isSaved = updateEksterAktarma(ekstreAktarma);
                    }
                }

                if (isSaved)
                {
                    if (SenderAppQS.Equals("EkstreListesi") || SenderAppQS.Equals("NBE"))
                    {
                        RedirectToPage(ProjeConstants.PAGE_EKSTRE_LIST + "?EkstreAktarmaId=" + EkstreAktarmaIdQS + "&IslemTarihi=" + IslemTarihiTxt.Text.ConvertToDatetimeEmptyIfNull() +
                            "&Banka=" + BankaQS + "&Mesaj=true?Islem=kaydet");
                    }
                    MessageHelper.PublishMessage("Ekstre Aktarma Bilgisi Kaydedildi.", ProjeConstants.MESAJ_BASARILI, 2000);

                }
                else
                {
                    //System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "Alert.danger('Ekstre Aktarma Bilgisi Kaydedilemedi.')", true);
                    MessageHelper.PublishMessage("Ekstre Aktarma Bilgisi Kaydedilemedi.", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

        }

        private bool SaveEkstreAktarma()
        {
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            bool isSaved = false;
            EkstreAktarma ekstreAktarma = new EkstreAktarma();
            ekstreAktarma.Adi = AdiTxt.Text.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
            ekstreAktarma.TCKimlikNo = TCKimlikNoTxt.Text.ConvertToLong();
            ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(Telefon1Txt.Text.ReturnEmptyIfNull().ToString());
            ekstreAktarma.Telefon2 = UtilityHelper.TelefonFormatla(Telefon2Txt.Text.ReturnEmptyIfNull().ToString());
            ekstreAktarma.Adres = AdresTxt.Text.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
            ekstreAktarma.Ili = IliDDL.SelectedItem.Text.ReturnEmptyIfNull().ToString();
            ekstreAktarma.Ilcesi = IlcesiDDL.SelectedItem.Text.ReturnEmptyIfNull().ToString();
            ekstreAktarma.BankaAdi = BankaDDL.SelectedItem.Text.ReturnEmptyIfNull().ToString();
            ekstreAktarma.DovizCinsi = "TL";
            ekstreAktarma.Eposta = EPostaTxt.Text.ReturnEmptyIfNull().ToString();
            ekstreAktarma.PostaKodu = PostaKoduTxt.Text.ReturnEmptyIfNull().ToString();
            ekstreAktarma.Tutar = TutarTxt.Text.ConvertToDecimal();
            ekstreAktarma.FisNo = FisNoTxt.Text;
            ekstreAktarma.TuzelKisi = TuzelKisiChk.Checked;
            ekstreAktarma.BelgeIstemiyor = BelgeIstemiyorChk.Checked;
            ekstreAktarma.Olusturan = UtilityHelper.GetCurrentUser();
            ekstreAktarma.Aciklama = AciklamaTxt.Text;
            ekstreAktarma.BagisTarihi = BagisTarihiTxt.Value.ConvertToDatetime();
            ekstreAktarma.IslemTarihi = IslemTarihiTxt.Text.ConvertToDatetime();
            ekstreAktarma.ElleKayit = true;
            ekstreAktarma.NakitBagisciId = NakitBagisciIdLbl.Text.ConvertToInt();
            int saveId = ekstreAktarma.Save();
            if (saveId > 0)
                isSaved = true;
            return isSaved;
        }
        private bool updateEksterAktarma(EkstreAktarma ekstreAktarma)
        {
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            bool isSaved = false;
            ekstreAktarma.Adi = AdiTxt.Text.ReturnEmptyIfNull().ToString().TrimStart().TrimEnd().ToUpper(culturInfo);
            ekstreAktarma.TCKimlikNo = TCKimlikNoTxt.Text.ConvertToLong();
            ekstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(Telefon1Txt.Text.ReturnEmptyIfNull().ToString());
            ekstreAktarma.Telefon2 = UtilityHelper.TelefonFormatla(Telefon2Txt.Text.ReturnEmptyIfNull().ToString());
            ekstreAktarma.Adres = AdresTxt.Text.ReturnEmptyIfNull().ToString().ToUpper(culturInfo);
            ekstreAktarma.Ili = IliDDL.SelectedItem.Text.ReturnEmptyIfNull().ToString();
            ekstreAktarma.Ilcesi = IlcesiDDL.SelectedItem.Text.ReturnEmptyIfNull().ToString();
            ekstreAktarma.BankaAdi = BankaDDL.SelectedItem.Text.ReturnEmptyIfNull().ToString();
            ekstreAktarma.DovizCinsi = "TL";
            ekstreAktarma.Eposta = EPostaTxt.Text.ReturnEmptyIfNull().ToString();
            ekstreAktarma.PostaKodu = PostaKoduTxt.Text.ReturnEmptyIfNull().ToString();
            ekstreAktarma.Tutar = TutarTxt.Text.ConvertToDecimal();
            ekstreAktarma.FisNo = FisNoTxt.Text;
            ekstreAktarma.TuzelKisi = TuzelKisiChk.Checked;
            ekstreAktarma.BelgeIstemiyor = BelgeIstemiyorChk.Checked;
            ekstreAktarma.Degistiren = UtilityHelper.GetCurrentUser();
            ekstreAktarma.Aciklama = AciklamaTxt.Text;
            ekstreAktarma.BagisTarihi = BagisTarihiTxt.Value.ConvertToDatetime();
            ekstreAktarma.IslemTarihi = IslemTarihiTxt.Text.ConvertToDatetime();
            ekstreAktarma.NakitBagisciId = NakitBagisciIdLbl.Text.ConvertToInt();
            isSaved = ekstreAktarma.Update();
            return isSaved;
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
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                EkstreAktarma ekstreAktarma = new EkstreAktarma();
                ekstreAktarma = ekstreAktarma.Select<EkstreAktarma>(EkstreAktarmaIdQS.ConvertToInt());
                if (SenderAppQS.Equals("NBB"))//NakitBagisciBulma'dan geldiyse
                {
                    newUrl += "/" + ProjeConstants.PAGE_NAKITBAGISCI_BULMA + "?Param=" + ParamQS;
                }
                else if (ekstreAktarma != null)
                {
                    newUrl += "/" + ProjeConstants.PAGE_EKSTRE_LIST + "?EkstreAktarmaId=" + EkstreAktarmaIdQS + "&IslemTarihi=" + ekstreAktarma.IslemTarihi.ConvertToDatetimeEmptyIfNull() + "&Banka=" + BankaQS;
                }
                else
                {
                    newUrl += "/" + ProjeConstants.PAGE_EKSTRE_LIST;
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

        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillIlceData();
        }
        protected void chkBilinmeyen_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBilinmeyen.Checked)
            {
                AdiTxt.Text = ProjeConstants.NAKITBAGISCI_BILINMEYEN;
                AdiTxt.Attributes["class"] += " makeDisabled ";
                TCKimlikNoTxt.Attributes["class"] += " makeDisabled ";
            }
            else
            {
                AdiTxt.Text = string.Empty;
                AdiTxt.Attributes["class"] = AdiTxt.Attributes["class"].ReturnEmptyIfNull().ToString().Replace("makeDisabled", "");
                TCKimlikNoTxt.Attributes["class"] = TCKimlikNoTxt.Attributes["class"].ReturnEmptyIfNull().ToString().Replace("makeDisabled", "");
            }

        }
        protected void NakitBagisciSecildiBtn_Click(object sender, EventArgs e)
        {
            NakitBagisci nb = new NakitBagisci();
            nb = nb.Select<NakitBagisci>(NakitBagisciIdQS.ConvertToInt());
        }
        protected void NakitBagisciSecBtn_Click(object sender, EventArgs e)
        {

            try
            {
                string queryString = "&EkstreAktarmaId=" + EkstreAktarmaIdQS;
                var jsString = "OpenSPPopup('" + queryString + "');";
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);

            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        protected void EkstreListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_EKSTRE_LIST + "?SenderApp=EAE&IslemTarihi=" + IslemTarihiTxt.Text + "&EkstreAktarmaId=" + EkstreAktarmaIdQS + "&Banka=" + BankaQS);
        }

        protected void EslestirBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_NAKITBAGISCI_ESLESTIR + "?IslemTarihi=" + IslemTarihiTxt.Text + "&EkstreAktarmaId =" + EkstreAktarmaIdQS + "&Banka=" + BankaQS);
        }
    }
}
