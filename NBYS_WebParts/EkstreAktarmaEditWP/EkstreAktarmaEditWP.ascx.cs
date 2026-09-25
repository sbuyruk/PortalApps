using Model.NBYS;
using Model.Ortak;
using Model.Services.NBYS;
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
        private string ParamQS//nakit bağışçı düzenlemeden dönüyorsa aranan metni tekrar arasın
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
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    BankaDDLDoldur();
                    IlDDLoldur();
                    IlceDDLDoldur();
                    DovizCinsiDDLDoldur();
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
            KaydetBtn.CssClass = "btn btn-outline-primary m-2";
            KaydetBtn.Text = "Güncelle";
            if (SenderAppQS.Equals("EkstreListesi") || SenderAppQS.Equals("NBE"))
            {
                EslestirBtn.Visible = true;
            }
            else
            {
                EslestirBtn.Visible = false;
            }

            ExtreAktarmaFormunuDoldur();
            NakitBagisciBilgileriniDoldur();
        }
        private void OpenGiris()
        {
            //CardHeader.Attributes["Class"] = "btn-success";
            KaydetBtn.CssClass = "btn btn-outline-success float-left";
            KaydetBtn.Text = "Güncelle";
            DateTime today = DateTime.Now;
            IslemTarihiTxt.Text = today.ToString(ProjeConstants.DATE_TR);
            NakitBagisciIdLbl.Text = "0";
            NakitBagisciBilgileriniDoldur();
        }
        private void NakitBagisciBilgileriniDoldur()
        {
            if (!string.IsNullOrEmpty(NakitBagisciIdQS))
            {
                long ektreAktarmaTCKimlikNo = 0;
                EkstreAktarma ekstreAktarma = new EkstreAktarma();
                ekstreAktarma = ekstreAktarma.Select<EkstreAktarma>(EkstreAktarmaIdQS.ConvertToInt());
                ektreAktarmaTCKimlikNo = ekstreAktarma == null ? 0 : ekstreAktarma.TCKimlikNo.ReturnZeroIfNull().ConvertToInt();
                NakitBagisci nb = new NakitBagisciService().GetById(NakitBagisciIdQS.ConvertToInt());
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
                    string ilstr = nb.Ili.ToString();
                    if (IliDDL.Items.FindByValue(ilstr) != null)
                    {
                        IliDDL.SelectedValue = IliDDL.Items.FindByValue(ilstr).Value;
                    }
                    IlceDDLDoldur();
                    string ilcestr = nb.Ilcesi.ToString();
                    if (IlcesiDDL.Items.FindByValue(ilcestr) != null)
                    {
                        IlcesiDDL.SelectedValue = IlcesiDDL.Items.FindByValue(ilcestr).Value;
                    }
                }
            }
        }
        private void ExtreAktarmaFormunuDoldur()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            int ekstreAktarmaId = EkstreAktarmaIdQS.ConvertToInt();
            EkstreAktarma ekstreAktarma = new EkstreAktarma();
            ekstreAktarma = ekstreAktarma.Select<EkstreAktarma>(ekstreAktarmaId);

            NakitBagisciIdLbl.Text = ekstreAktarma.NakitBagisciId.ToString();
            AdiTxt.Text = ekstreAktarma.Adi.ReturnEmptyIfNull().ToString().TrimStart().TrimEnd();
            TCKimlikNoTxt.Text = ekstreAktarma.TCKimlikNo.ReturnEmptyIfNull().ToString();
            Telefon1Txt.Text = ekstreAktarma.Telefon1.ReturnEmptyIfNull().ToString();
            Telefon2Txt.Text = ekstreAktarma.Telefon2.ReturnEmptyIfNull().ToString();
            AdresTxt.Text = ekstreAktarma.Adres.ReturnEmptyIfNull().ToString();

            TextInfo culturInfoTR = new CultureInfo(ProjeConstants.CULTUREINFO, true).TextInfo;
            string ilstr = culturInfoTR.ToUpper(ekstreAktarma.Ili.ReturnEmptyIfNull().ToString());
            if (IliDDL.Items.FindByText(ilstr) != null)
            {
                IliDDL.SelectedValue = IliDDL.Items.FindByText(ilstr).Value;
            }
            IlceDDLDoldur();
            string ilcestr = culturInfoTR.ToUpper(ekstreAktarma.Ilcesi.ReturnEmptyIfNull().ToString());
            if (IlcesiDDL.Items.FindByText(ilcestr) != null)
            {
                IlcesiDDL.SelectedValue = IlcesiDDL.Items.FindByText(ilcestr).Value;
            }
            string bankaStr = ekstreAktarma.BankaAdi.ReturnEmptyIfNull().ToString();//culturInfo.ToUpper(ekstreAktarma.BankaAdi.ReturnEmptyIfNull().ToString()); 
            if (BankaDDL.Items.FindByText(bankaStr) != null)
            {
                BankaDDL.SelectedValue = BankaDDL.Items.FindByText(bankaStr).Value;
            }
            string dovizCinsiStr = ekstreAktarma.DovizCinsi.ReturnEmptyIfNull().ToString();
            if (DovizCinsiDDL.Items.FindByText(dovizCinsiStr) != null)
            {
                DovizCinsiDDL.SelectedValue = DovizCinsiDDL.Items.FindByText(dovizCinsiStr).Value;
            }
            if (DovizCinsiDDL.SelectedItem.Value.Equals(ProjeConstants.DOVIZ_TL))
            {
                DovizDiv.Attributes["style"] = "display:none";
            }
            else
            {
                DovizDiv.Attributes["style"] = "display:block";
            }
            DovizKuruTxt.Text = ekstreAktarma.DovizKuru.ReturnZeroIfNull().ConvertToDecimal().ToString("N", culturInfo);
            DovizTutariTxt.Text = ekstreAktarma.DovizTutari.ReturnZeroIfNull().ConvertToDecimal().ToString("N", culturInfo);
            HesaplananTlLbl.Text = (ekstreAktarma.DovizTutari.ReturnZeroIfNull().ConvertToDecimal() * ekstreAktarma.DovizKuru.ReturnZeroIfNull().ConvertToDecimal()).ToString("N", culturInfo);

            KurTarihiTxt.Text = ekstreAktarma.KurTarihi.ConvertToDatetimeEmptyIfNull();

            EPostaTxt.Text = ekstreAktarma.Eposta.ReturnEmptyIfNull().ToString();
            PostaKoduTxt.Text = ekstreAktarma.PostaKodu.ReturnEmptyIfNull().ToString();
            TutarTlTxt.Text = ekstreAktarma.Tutar.ReturnEmptyIfNull().ToString();
            FisNoTxt.Text = ekstreAktarma.FisNo.ReturnEmptyIfNull().ToString();
            TuzelKisiChk.Checked = ekstreAktarma.TuzelKisi.ConvertToBool();
            BelgeIstemiyorChk.Checked = ekstreAktarma.BelgeIstemiyor.ConvertToBool();
            IslemTarihiTxt.Text = ekstreAktarma.IslemTarihi.ToString(ProjeConstants.DATE_TR);
            BagisTarihiTxt.Value = ekstreAktarma.BagisTarihi.ToString(ProjeConstants.DATE_TR);
            AciklamaTxt.Text = ekstreAktarma.Aciklama.ReturnEmptyIfNull().ToString();
            NakitBagisciIdLbl.Text = ekstreAktarma.NakitBagisciId.ReturnZeroIfNull().ToString();
        }
        private void ExtreAktarmaFormunuTemizle()
        {

            AdiTxt.Text = string.Empty;
            TCKimlikNoTxt.Text = string.Empty;
            Telefon1Txt.Text = string.Empty;
            Telefon2Txt.Text = string.Empty;
            AdresTxt.Text = string.Empty;
            EPostaTxt.Text = string.Empty;
            PostaKoduTxt.Text = string.Empty;
            TutarTlTxt.Text = string.Empty;
            FisNoTxt.Text = string.Empty;
            TuzelKisiChk.Checked = false;
            BelgeIstemiyorChk.Checked = false;
            IslemTarihiTxt.Text = DateTime.Today.ToString(ProjeConstants.DATE_TR);
            // BagisTarihiTxt.Value = DateTime.Today.ToString(ProjeConstants.DATE_TR);
            AciklamaTxt.Text = string.Empty;
            BagisTarihiTxt.Value = string.Empty;
            UtilityHelper.SetDDLValue(BankaDDL, ProjeConstants.BANKA_BOS_INT.ToString());
            UtilityHelper.SetDDLValue(IliDDL, ProjeConstants.IL_BOS.ToString());
            UtilityHelper.SetDDLValue(IlcesiDDL, ProjeConstants.ILCE_BOS.ToString());

            NakitBagisciIdLbl.Text = "0";
        }
        private void IlDDLoldur()
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
            UtilityHelper.SetDDLValue(IliDDL,ProjeConstants.IL_BOS.ToString());
        }
        private void IlceDDLDoldur()
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
            UtilityHelper.SetDDLValue(IlcesiDDL, ProjeConstants.ILCE_BOS.ToString());

        }
        private void BankaDDLDoldur()
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
            UtilityHelper.SetDDLValue(BankaDDL, ProjeConstants.BANKA_BOS_INT.ToString());

        }
        private void DovizCinsiDDLDoldur()
        {
            if (DovizCinsiDDL.SelectedItem == null)
            {
                DovizCinsiDDL.Items.Clear();
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_TL, ProjeConstants.DOVIZ_TL));
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_EURO, ProjeConstants.DOVIZ_EURO));
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_USD, ProjeConstants.DOVIZ_USD));
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_GBP, ProjeConstants.DOVIZ_GBP));
            }
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool isSaved = false;
                if (string.IsNullOrWhiteSpace(AdiTxt.Text))
                {
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
                            ExtreAktarmaFormunuTemizle();
                            BankaDDLDoldur();
                            IlDDLoldur();
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
                    MessageHelper.PublishMessage("Baðýþçý Adý boþ olamaz. Lütfen baðýþçý adýný giriniz.", ProjeConstants.MESAJ_HATA);
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
            ekstreAktarma.DovizCinsi = DovizCinsiDDL.SelectedItem == null ? ProjeConstants.DOVIZ_TL : DovizCinsiDDL.SelectedItem.Value;
            ekstreAktarma.Eposta = EPostaTxt.Text.ReturnEmptyIfNull().ToString();
            ekstreAktarma.PostaKodu = PostaKoduTxt.Text.ReturnEmptyIfNull().ToString();
            ekstreAktarma.Tutar = TutarTlTxt.Text.ConvertToDecimal();
            ekstreAktarma.DovizTutari = DovizTutariTxt.Text.ConvertToDecimal();
            ekstreAktarma.DovizKuru = DovizKuruTxt.Text.ConvertToDecimal();
            ekstreAktarma.KurTarihi = KurTarihiTxt.Text.ConvertToDatetime();
            ekstreAktarma.FisNo = FisNoTxt.Text;
            ekstreAktarma.TuzelKisi = TuzelKisiChk.Checked;
            ekstreAktarma.BelgeIstemiyor = BelgeIstemiyorChk.Checked;
            ekstreAktarma.Olusturan = UtilityHelper.GetCurrentUserLoginName();
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
            ekstreAktarma.Eposta = EPostaTxt.Text.ReturnEmptyIfNull().ToString();
            ekstreAktarma.PostaKodu = PostaKoduTxt.Text.ReturnEmptyIfNull().ToString();
            ekstreAktarma.Tutar = TutarTlTxt.Text.ConvertToDecimal();
            ekstreAktarma.DovizCinsi = DovizCinsiDDL.SelectedItem == null ? ProjeConstants.DOVIZ_TL : DovizCinsiDDL.SelectedItem.Value;
            ekstreAktarma.DovizTutari = DovizTutariTxt.Text.ConvertToDecimal();
            ekstreAktarma.DovizKuru = DovizKuruTxt.Text.ConvertToDecimal();
            ekstreAktarma.KurTarihi = KurTarihiTxt.Text.ConvertToDatetime();
            ekstreAktarma.FisNo = FisNoTxt.Text;
            ekstreAktarma.TuzelKisi = TuzelKisiChk.Checked;
            ekstreAktarma.BelgeIstemiyor = BelgeIstemiyorChk.Checked;
            ekstreAktarma.Degistiren = UtilityHelper.GetCurrentUserLoginName();
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
            IlceDDLDoldur();
        }
        protected void ChkBilinmeyen_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkBilinmeyen.Checked)
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
            NakitBagisci nb = new NakitBagisciService().GetById(NakitBagisciIdQS.ConvertToInt());
        }

        protected void EkstreListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_EKSTRE_LIST + "?SenderApp=EAE&IslemTarihi=" + IslemTarihiTxt.Text + "&EkstreAktarmaId=" + EkstreAktarmaIdQS + "&Banka=" + BankaQS);
        }
        protected void HesaplaBtn_Click(object sender, EventArgs e)
        {
            decimal dtutar = DovizTutariTxt.Text.ConvertToDecimal();
            decimal dkur = DovizKuruTxt.Text.ConvertToDecimal();

            var tutarTl = dkur * dtutar;
            HesaplananTlLbl.Text = tutarTl.ToString("N", culturInfo);
            TutaraYazBtn.Visible = tutarTl > 0;
        }

        protected void EslestirBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_NAKITBAGISCI_ESLESTIR + "?IslemTarihi=" + IslemTarihiTxt.Text + "&EkstreAktarmaId =" + EkstreAktarmaIdQS + "&Banka=" + BankaQS);
        }
        protected void DovizCinsiDDLIli_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DovizCinsiDDL.SelectedItem.Value.Equals(ProjeConstants.DOVIZ_TL))
            {
                DovizDiv.Attributes["style"] = "display:none";
            }
            else
            {
                DovizDiv.Attributes["style"] = "display:block";
            }
        }
    }
}
