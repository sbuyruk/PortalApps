using Model.Ortak;
using Model.Services.TBYS;
using Model.TBYS;
using System;
using Utility.Logging;
using TBYS_WebParts.Logging;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using TBYS_WebParts.BagimsizBolumWP;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.TasinmazSigortaGirisWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazSigortaGirisWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get
            {
                return PortalLogContext.FromCurrentRequest("TBYS", "TasinmazSigortaGirisWP");
            }
        }

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazSigortaGirisWP()
        {
        }
        protected override void OnLoad(EventArgs e)
        {
            if (!Page.IsPostBack) TbysWebPartLog.RecordOpened(LoggingContext);
            try
            {
                base.OnLoad(e);
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch (Exception error)
            {
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.TasinmazSigortaGirisWP.Page_Load", error);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SigortaIdQS
        {
            get
            {

                if (ViewState["SigortaId"] == null)
                {
                    if (Page.Request.QueryString["SigortaId"] != null)
                    {
                        ViewState["SigortaId"] = Page.Request.QueryString["SigortaId"];
                    }
                    else
                    {
                        ViewState["SigortaId"] = string.Empty;
                    }
                }
                return ViewState["SigortaId"].ToString();
            }

            set
            {
                ViewState["SigortaId"] = value;
            }
        }
        private string AdresKoduQS
        {
            get
            {

                if (ViewState["AdresKodu"] == null)
                {
                    if (Page.Request.QueryString["AdresKodu"] != null)
                    {
                        ViewState["AdresKodu"] = Page.Request.QueryString["AdresKodu"];
                    }
                    else
                    {
                        ViewState["AdresKodu"] = string.Empty;
                    }
                }
                return ViewState["AdresKodu"].ToString();
            }

            set
            {
                ViewState["AdresKodu"] = value;
            }
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
        private int BolgeIdQS
        {
            get
            {

                if (ViewState["BolgeId"] == null)
                {
                    if (Page.Request.QueryString["BolgeId"] != null)
                    {
                        ViewState["BolgeId"] = Page.Request.QueryString["BolgeId"];
                    }
                    else
                    {
                        ViewState["BolgeId"] = string.Empty;
                    }
                }
                return ViewState["BolgeId"].ReturnZeroIfNull().ConvertToInt();
            }

            set
            {
                ViewState["BolgeId"] = value;
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
            //önceki sayfayi tut, geri tusuna basildiginda gerekli
            if (!Page.IsPostBack)
            {
                Bolge bolge = IKYSOrtak.BolgeGetirByUserName(CurrentUserName, LoggingContext);
                BolgeIdQS = bolge == null ? 0 : bolge.Id;
                if (String.IsNullOrEmpty(DestinationAppQS) || String.Equals(DestinationAppQS, ""))
                {
                    SigortaGirisi();
                }
                else if (String.Equals(DestinationAppQS, "SigortaD"))
                {
                    SigortaDuzenle();
                }
                ButonlariGosterGizle();
            }
        }
        private void ButonlariGosterGizle()
        {
            bool isVisible = BolgeIdQS == ProjeConstants.HEPSI_INT || BolgeIdQS == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? true : false;
            if (isVisible)
            {
                UtilityHelper.SetControlState(true, true, TasinmazBtn, BelgeSilBtn,
                    NextBtn, PrevBtn, SilBtn);//UpdateBtn,

            }
            else
            {
                UtilityHelper.SetControlState(false, false, TasinmazBtn, BelgeSilBtn,
                    NextBtn, PrevBtn, SilBtn); //UpdateBtn,
            }
        }
        private void SigortaGirisi()
        {
            SaveBtn.Visible = true;
            UpdateBtn.Visible = false;
            SilBtn.Visible = false;
            BackBtn.Visible = false;
            NextBtn.Visible = false;
            PrevBtn.Visible = false;
        }
        private void SigortaDuzenle()
        {
            SaveBtn.Visible = false;
            UpdateBtn.Visible = true;
            SilBtn.Visible = true;
            BackBtn.Visible = true;
            NextBtn.Visible = true;
            PrevBtn.Visible = true;

            TitleLbl.Attributes["Class"] = "text-success";
            TitleLbl.Text = "Sigorta Güncelleme";

            Sigorta sigorta = new Sigorta();
            sigorta = new Model.Services.TBYS.SigortaService(LoggingContext).GetById(SigortaIdQS.ConvertToInt());
            if (sigorta != null)
            {

                SigortaFormunuDoldur(sigorta);
                PDFGoster(sigorta);
                TasinmazIdQS = sigorta.TasinmazId.ToString();
                DateTime sigortaBitisTarihi = sigorta.SigortaBitTar.ConvertToDatetime();
                if (sigortaBitisTarihi <= DateTime.MinValue)
                {
                    MessageHelper.PublishMessage("Sigorta Bitis Tarihi Bos!", ProjeConstants.MESAJ_HATA);
                }
                else if (sigortaBitisTarihi <= DateTime.Today.AddMonths(1))
                {
                    MessageHelper.PublishMessage("Sigorta Vadesi " + sigorta.SigortaBitTar.ToString("dd.MM.yyyy") + " Tarihinde dolacaktır", ProjeConstants.MESAJ_BILGI);
                }
            }

        }
        private bool SigortaFormunuDoldur(Sigorta sigorta)
        {

            bool formDolduMu = false;
            try
            {
                SigortaCinsiDDLDoldur();
                BagimsizBolumDDLDoldur(sigorta);

                AdresKoduTxt.Text = sigorta.AdresKodu.ToString();
                AdresKoduQS = AdresKoduTxt.Text;
                BBBrutAlanTxt.Text = sigorta.BBBrutAlan;
                BulunduguKatTxt.Text = sigorta.BulunduguKat;
                BBNetAlanTxt.Text = sigorta.BBNetAlan;
                ToplamKatSayisiTxt.Text = sigorta.ToplamKatSayisi;
                InsaYiliTxt.Text = sigorta.InsaYili;
                PoliceNoTxt.Text = sigorta.PoliceNo.ToString();
                DaskPoliceNoTxt.Text = sigorta.DaskPoliceNo;
                SigBasTarTxt.Text = sigorta.SigortaBasTar.ConvertToDatetimeEmptyIfNull();
                SigBitTarTxt.Text = sigorta.SigortaBitTar.ConvertToDatetimeEmptyIfNull();
                SigortaCinsiDDL.SelectedValue = sigorta.SigortaCinsi;
                IdLbl.Text = sigorta.Id + " #TasinmazId:" + sigorta.TasinmazId.ToString();
                SigortaBedeliTxt.Text = sigorta.SigortaBedeli.ToString();
                PrimTxt.Text = sigorta.Prim.ToString();

                TeminatListCheckBoxDoldur(sigorta.TeminatListesi);
                AciklamaTxt.Text = sigorta.Aciklama;

                BagimsizBolumNoTxt.Text = string.IsNullOrEmpty(sigorta.BagimsizBolumNo)?BagimsizBolumDDL.SelectedItem.Text: sigorta.BagimsizBolumNo;
                Tasinmaz tasinmaz = new Tasinmaz();
                tasinmaz = new TasinmazService(LoggingContext).Select(sigorta.TasinmazId);
                if (tasinmaz != null)
                {
                    AdiLbl.Text = tasinmaz.KullanimSekli + " - " + tasinmaz.Adres + " - " + tasinmaz.Ilcesi + "/" + tasinmaz.Ili;


                    BulunduguKatTxt.Text = string.IsNullOrEmpty(sigorta.BulunduguKat) ? tasinmaz.BulunduguKat : sigorta.BulunduguKat;
                    TapuTasinmazNoTxt.Text = tasinmaz.TapuTasinmazNo;
                    YapiTarziTxt.Text = string.IsNullOrEmpty(sigorta.YapiTarzi) ? tasinmaz.YapiTarzi : sigorta.YapiTarzi;
                    KullanimAmaciTxt.Text = sigorta.KullanimSekli;
                    if (String.IsNullOrEmpty(sigorta.KullanimSekli))
                    {
                        if (tasinmaz.AltBolum) // Bu siorta Bağımsız bölüme ait
                        {
                            BagimsizBolum bagimsizBolum = new BagimsizBolum();
                            bagimsizBolum = new BagimsizBolumService(LoggingContext).GetById(sigorta.BolumId);
                            if (bagimsizBolum != null)
                            {
                                KullanimAmaciTxt.Text = bagimsizBolum.KullanimAmaci;
                                BBBrutAlanTxt.Text = string.IsNullOrEmpty(sigorta.BBBrutAlan) ? bagimsizBolum.BBBrutAlan.ToString("N", new System.Globalization.CultureInfo("tr-TR"))
                                    : sigorta.BBBrutAlan;
                                BBNetAlanTxt.Text = string.IsNullOrEmpty(sigorta.BBNetAlan) ? bagimsizBolum.BBNetAlan.ToString("N", new System.Globalization.CultureInfo("tr-TR"))
                                    : sigorta.BBNetAlan;
                            }
                        }
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(KullanimAmaciTxt.Text.Trim()))
                            KullanimAmaciTxt.Text = tasinmaz.KullanimSekli;
                        BBBrutAlanTxt.Text = string.IsNullOrEmpty(sigorta.BBBrutAlan) ? tasinmaz.BBBrutAlan.ToString("N", new System.Globalization.CultureInfo("tr-TR"))
                            : sigorta.BBBrutAlan;

                        BBNetAlanTxt.Text = string.IsNullOrEmpty(sigorta.BBNetAlan) ? tasinmaz.BBNetAlan.ToString("N", new System.Globalization.CultureInfo("tr-TR"))
                            : sigorta.BBNetAlan;
                    }
                }

            }
            catch (Exception exception)
            {
                if (exception is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazSigortaGirisWP.SigortaFormunuDoldur", exception);
                ExceptionHelper exh = new ExceptionHelper(exception);
                formDolduMu = false;
            }
            return formDolduMu;
        }
        private void TeminatListCheckBoxDoldur(string teminatListesi)
        {
            DepremChk.Checked = teminatListesi.Contains("1");
            YanginChk.Checked = teminatListesi.Contains("2");
            Makine100000Chk.Checked = teminatListesi.Contains("3");
            Makine5000Chk.Checked = teminatListesi.Contains("4");
            JeneratorChk.Checked = teminatListesi.Contains("5");
            AsansorChk.Checked = teminatListesi.Contains("6");
            KazanChk.Checked = teminatListesi.Contains("7");
        }
        private void BagimsizBolumDDLDoldur(Sigorta sigorta)
        {
            BagimsizBolumDDL.Items.Clear();
            //ListItem li0 = new ListItem("", "0");
            //BagimsizBolumDDL.Items.Add(li0);

            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = new TasinmazService(LoggingContext).Select(sigorta.TasinmazId);
            if (tasinmaz != null)
            {
                if (tasinmaz.AltBolum == ProjeConstants.ALTBOLUM_YOK)
                {
                    // No child section exists; the number is display text, not a section ID.
                    ListItem li = new ListItem(tasinmaz.BagimsizBolumNo, "0");
                    BagimsizBolumDDL.Items.Add(li);
                    BagimsizBolumNoTxt.Text = tasinmaz.BagimsizBolumNo;
                    KullanimAmaciTxt.Text = sigorta.KullanimSekli;
                }
                else
                {
                    KullanimAmaciTxt.Text = sigorta.KullanimSekli;
                    BagimsizBolum bb = new BagimsizBolum();
                    List<BagimsizBolum> list = new BagimsizBolumService(LoggingContext).GetByTasinmazId(sigorta.TasinmazId);
                    foreach (BagimsizBolum item in list)
                    {
                        ListItem li = new ListItem(item.BolumNo, item.Id.ToString());
                        BagimsizBolumDDL.Items.Add(li);
                    }
                    if (list.Count > 0)
                    {
                        if (BagimsizBolumDDL.Items.FindByValue(sigorta.BolumId.ReturnZeroIfNull().ToString()) != null)
                        {
                            BagimsizBolumDDL.SelectedValue = BagimsizBolumDDL.Items.FindByValue(sigorta.BolumId.ReturnZeroIfNull().ToString()).Value;
                            BagimsizBolumNoTxt.Text = BagimsizBolumDDL.SelectedItem.Text;
                        }
                    }

                    else {
                        BagimsizBolumNoTxt.Text = string.Empty;
                        MessageHelper.PublishMessage("Taşınmazın ALTBÖLÜMÜ VAR seçilmesine rağmen taşınmaza ait bölüm tanımlanmamıştır.", ProjeConstants.MESAJ_HATA);
                    }

                }
            }



        }
        private void SigortaCinsiDDLDoldur()
        {
            SigortaCinsiDDL.Items.Clear();
            SigortaCinsiDDL.Items.Add(ProjeConstants.SIGORTA_YOK);
            SigortaCinsiDDL.Items.Add(ProjeConstants.SIGORTA_DASK);
            SigortaCinsiDDL.Items.Add(ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            SigortaCinsiDDL.Items.FindByValue(ProjeConstants.SIGORTA_DEPREM_IHTIYARI).Attributes.Add("Disabled", "Disabled");
        }
        private bool UpdateSigorta(Sigorta sigorta)
        {
            bool guncellendiMi = false;

            if (sigorta != null)
            {
                sigorta.AdresKodu = AdresKoduTxt.Text;
                AdresKoduQS = AdresKoduTxt.Text;
                sigorta.BBBrutAlan = BBBrutAlanTxt.Text;
                sigorta.BulunduguKat = BulunduguKatTxt.Text;
                sigorta.BBNetAlan = BBNetAlanTxt.Text;
                sigorta.ToplamKatSayisi = ToplamKatSayisiTxt.Text;
                sigorta.InsaYili = InsaYiliTxt.Text;
                sigorta.PoliceNo = PoliceNoTxt.Text;
                sigorta.DaskPoliceNo = DaskPoliceNoTxt.Text;
                sigorta.SigortaBasTar = SigBasTarTxt.Text.ConvertToDatetime();
                sigorta.SigortaBitTar = SigBitTarTxt.Text.ConvertToDatetime();
                sigorta.SigortaCinsi = SigortaCinsiDDL.SelectedValue;
                sigorta.TasinmazId = TasinmazIdQS.ConvertToInt();
                sigorta.Prim = PrimTxt.Text.ConvertToDecimal();
                sigorta.SigortaBedeli = SigortaBedeliTxt.Text.ConvertToDecimal();
                sigorta.YapiTarzi = YapiTarziTxt.Text;
                sigorta.Degistiren = CurrentUserName;
                sigorta.BolumId = BagimsizBolumDDL.SelectedItem.Value.ConvertToInt();
                sigorta.TeminatListesi = SecilenTeminatlariGetir();
                sigorta.TeminatAciklama = SecilenTeminatAciklamalariniGetir();
                sigorta.BagimsizBolumNo = BagimsizBolumNoTxt.Text;
                sigorta.Aciklama = AciklamaTxt.Text;
                sigorta.KullanimSekli = KullanimAmaciTxt.Text;
                guncellendiMi = new Model.Services.TBYS.SigortaService(LoggingContext).UpdateWithPropertyStatus(sigorta);

            }
            return guncellendiMi;
        }
        private Sigorta SaveSigorta()
        {
            Sigorta sigorta = new Sigorta();
            sigorta.AdresKodu = AdresKoduTxt.Text;
            AdresKoduQS = AdresKoduTxt.Text;
            sigorta.BBBrutAlan = BBBrutAlanTxt.Text;
            sigorta.BulunduguKat = BulunduguKatTxt.Text;
            sigorta.BBNetAlan = BBNetAlanTxt.Text;
            sigorta.ToplamKatSayisi = ToplamKatSayisiTxt.Text;
            sigorta.InsaYili = InsaYiliTxt.Text;
            sigorta.PoliceNo = PoliceNoTxt.Text;
            sigorta.DaskPoliceNo = DaskPoliceNoTxt.Text;
            sigorta.SigortaBasTar = SigBasTarTxt.Text.ConvertToDatetime();
            sigorta.SigortaBitTar = SigBitTarTxt.Text.ConvertToDatetime();
            sigorta.SigortaCinsi = SigortaCinsiDDL.SelectedItem.Value;
            sigorta.TasinmazId = TasinmazIdQS.ConvertToInt();
            sigorta.Prim = PrimTxt.Text.ConvertToDecimal();
            sigorta.SigortaBedeli = SigortaBedeliTxt.Text.ConvertToDecimal();
            sigorta.YapiTarzi = YapiTarziTxt.Text;
            sigorta.Olusturan = CurrentUserName;
            sigorta.BolumId = BagimsizBolumDDL.SelectedItem.Value.ConvertToInt();
            sigorta.TeminatListesi = SecilenTeminatlariGetir();
            sigorta.TeminatAciklama = SecilenTeminatAciklamalariniGetir();
            sigorta.BagimsizBolumNo = BagimsizBolumNoTxt.Text;
            sigorta.Aciklama = AciklamaTxt.Text;
            sigorta.KullanimSekli = KullanimAmaciTxt.Text;
            int sigortaId = new Model.Services.TBYS.SigortaService(LoggingContext).SaveWithPropertyStatus(sigorta);
            sigorta.Id = sigortaId;
            if (sigortaId > 0)
            {
                string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                int queryIndex = newUrl.IndexOf("?");
                if (queryIndex > 0)
                    newUrl = newUrl.Substring(0, queryIndex);
                RedirectToPage(newUrl + "?DestinationApp=SigortaD&sigortaId=" + sigorta.Id);

            }
            return sigorta;
        }
        private string SecilenTeminatlariGetir()
        {
            string teminatListesi = string.Empty;
            teminatListesi = DepremChk.Checked ? "1," : "";
            teminatListesi += YanginChk.Checked ? "2," : "";
            teminatListesi += Makine100000Chk.Checked ? "3," : "";
            teminatListesi += Makine5000Chk.Checked ? "4," : "";
            teminatListesi += JeneratorChk.Checked ? "5," : "";
            teminatListesi += AsansorChk.Checked ? "6," : "";
            teminatListesi += KazanChk.Checked ? "7," : "";
            teminatListesi = string.IsNullOrEmpty(teminatListesi) ? "" : teminatListesi.Substring(0, teminatListesi.Length - 1); // sondaki virgülü at
            return teminatListesi;
        }
        private string SecilenTeminatAciklamalariniGetir()
        {
            string teminatListesi = string.Empty;
            teminatListesi = DepremChk.Checked ? " #1." + DepremChk.ToolTip.ToString() : "";
            teminatListesi += YanginChk.Checked ? " #2." + YanginChk.ToolTip.ToString() : "";
            teminatListesi += Makine100000Chk.Checked ? " #3." + Makine100000Chk.ToolTip.ToString() : "";
            teminatListesi += Makine5000Chk.Checked ? " #4." + Makine5000Chk.ToolTip.ToString() : "";
            teminatListesi += JeneratorChk.Checked ? " #5." + JeneratorChk.ToolTip.ToString() : "";
            teminatListesi += AsansorChk.Checked ? " #6." + AsansorChk.ToolTip.ToString() : "";
            teminatListesi += KazanChk.Checked ? " #7." + KazanChk.ToolTip.ToString() : "";
            teminatListesi = string.IsNullOrEmpty(teminatListesi) ? "" : teminatListesi.Substring(0, teminatListesi.Length);
            return teminatListesi;
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
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazSigortaGirisWP.RedirectToPage", ex);
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.TasinmazSigortaGirisWP.RedirectToPage", ex);
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
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazSigortaGirisWP.CloseBtn_Click", ex);
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.TasinmazSigortaGirisWP.CloseBtn_Click", ex);
            }
        }
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            if (SenderAppQS.Equals("SigortaL"))
                RedirectToPage(ProjeConstants.PAGE_TASINMAZSIGORTA_LIST + "?SigortaId=" + SigortaIdQS);
            else
                if (SenderAppQS.Equals("SigortaES"))
                RedirectToPage(ProjeConstants.PAGE_TASINMAZSIGORTA_EKLESIL + "?TasinmazId=" + TasinmazIdQS + "&EnvanterdeMi=" + EnvanterdeMiQS);
            else if (EnvanterdeMiQS.Equals("2"))
                RedirectToPage(ProjeConstants.PAGE_MULKIYETIOLMAYANTASINMAZ_GIRIS + "?TasinmazId=" + TasinmazIdQS + "&DestinationApp=TD" + "&EnvanterdeMi=" + EnvanterdeMiQS);
            else if (SenderAppQS.Equals("TD"))
                RedirectToPage(ProjeConstants.PAGE_TASINMAZ_GIRIS + "?TasinmazId=" + TasinmazIdQS + "&DestinationApp=TD");
        }
        protected void SaveBtn_Click(object sender, EventArgs e)
        {

            try
            {
                Sigorta sigorta = SaveSigorta();
                if (sigorta != null) //kaydettikten sonra önceki sayfaya dön
                {
                    MessageHelper.PublishMessage("Sigorta Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                    RedirectToPage(ProjeConstants.PAGE_TASINMAZSIGORTA_LIST + "?Mesaj=true");

                }
                else
                {
                    MessageHelper.PublishMessage("Sigorta kaydedilemedi.", ProjeConstants.MESAJ_HATA);
                }


            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazSigortaGirisWP.SaveBtn_Click", ex);
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.TasinmazSigortaGirisWP.SaveBtn_Click", ex);
            }
        }
        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            bool committed = false;
            try
            {
                Sigorta sigorta = new Model.Services.TBYS.SigortaService(LoggingContext).GetById(SigortaIdQS.ConvertToInt());
                if (sigorta == null)
                {
                    MessageHelper.PublishMessage("Sigorta kaydı bulunamadı.", ProjeConstants.MESAJ_HATA);
                    return;
                }
                if (!UpdateSigorta(sigorta))
                {
                    MessageHelper.PublishMessage("Sigorta güncellenemedi.", ProjeConstants.MESAJ_HATA);
                    return;
                }
                committed = true;
                PDFKaydet(sigorta, true);
                PDFGoster(sigorta, true);
                MessageHelper.PublishMessage("Sigorta güncellendi.", ProjeConstants.MESAJ_BASARILI, 2000);
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                if (committed)
                {
                    TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazSigortaGirisWP.DocumentAfterUpdate", error);
                    Utility.HelperClasses.Messages.Warning("Sigorta güncellendi; poliçe belgesi işlemi tamamlanamadı. Tekrar güncellemeden önce kaydı ve belgeyi kontrol edin. Başvuru kodu: " + LoggingContext.OperationId.ToString("N"));
                }
                else TbysWebPartLog.ReportError(LoggingContext, "TBYS.TasinmazSigortaGirisWP.UpdateBtn_Click", error);
            }
        }
        protected void SilBtn_Click(object sender, EventArgs e)
        {
            Sigorta sigorta = new Sigorta();
            sigorta = new Model.Services.TBYS.SigortaService(LoggingContext).GetById(SigortaIdQS.ConvertToInt());
            if (sigorta != null)
            {
                Tasinmaz tasinmaz = new Tasinmaz();
                string adres = new TasinmazService(LoggingContext).SelectByIdBolumId(sigorta.TasinmazId, sigorta.BolumId);
                SilmeMesajiLbl.Text = adres + " adresindeki taşınmaza ait Sigorta silinecek";
                ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "ModalOnay();", true);
            }

        }
        protected void SilNowBtn_Click(object sender, EventArgs e)
        {
            bool silindi = false;
            try
            {
                Sigorta sigorta = new Sigorta();
                sigorta = new Model.Services.TBYS.SigortaService(LoggingContext).GetById(SigortaIdQS.ConvertToInt());
                if (sigorta != null)
                {
                    silindi = new Model.Services.TBYS.SigortaService(LoggingContext).Delete(sigorta);
                }
                ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "CloseModal();", true);
                if (silindi)
                {
                    RedirectToPage(ProjeConstants.PAGE_TASINMAZSIGORTA_LIST + "?Mesaj=true" );
                }
                else
                {
                    MessageHelper.PublishMessage("Sigorta Silinemedi", ProjeConstants.MESAJ_BILGI);
                }
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazSigortaGirisWP.SilNowBtn_Click", ex);

                ExceptionHelper exhelper = new ExceptionHelper();
                exhelper.Exceptions.Add(new Exception("Sigorta Silinemedi"));
                exhelper.Exceptions.Add(ex);
                TbysWebPartLog.ReportErrors(LoggingContext, "TBYS.TasinmazSigortaGirisWP.SilNowBtn_Click", exhelper.Exceptions);
            }

        }
        protected void TasinmazBtn_Click(object sender, EventArgs e)
        {

            if (EnvanterdeMiQS.Equals("2"))
                RedirectToPage(ProjeConstants.PAGE_MULKIYETIOLMAYANTASINMAZ_GIRIS + "?TasinmazId=" + TasinmazIdQS + "&DestinationApp=TD");
            else
                RedirectToPage(ProjeConstants.PAGE_TASINMAZ_GIRIS + "?DestinationApp=TD&SenderApp=TL&TasinmazId=" + TasinmazIdQS);
        }
        protected void KullanimAmaciGetirBtn_Click(object sender, EventArgs e)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = new TasinmazService(LoggingContext).Select(TasinmazIdQS.ConvertToInt());
            if (tasinmaz != null)
            {
                KullanimAmaciTxt.Text = tasinmaz.KullanimSekli;

            }
        }
        protected void TasinmazListesiBtn_Click(object sender, EventArgs e)
        {
            if (EnvanterdeMiQS.Equals("2"))
                RedirectToPage(ProjeConstants.PAGE_PAGE_MULKIYETIOLMAYANTASINMAZ_LIST);
            else RedirectToPage(ProjeConstants.PAGE_TASINMAZ_LIST);
        }
        protected void SigortaListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_TASINMAZSIGORTA_LIST);
        }

        protected void NextBtn_Click(object sender, EventArgs e)
        {
            Sigorta sigorta = new Sigorta();
            sigorta = new Model.Services.TBYS.SigortaService(LoggingContext).GetById(SigortaIdQS.ConvertToInt());
            Sigorta sonrakiSigorta = new Model.Services.TBYS.SigortaService(LoggingContext).GetNext(sigorta.Id);
            string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            int queryIndex = newUrl.IndexOf("?");
            if (queryIndex > 0)
                newUrl = newUrl.Substring(0, queryIndex);
            newUrl = newUrl + "?DestinationApp=SigortaD&SenderApp=SigortaL&SigortaId=" + sonrakiSigorta.Id + "&TasinmazId=" + sonrakiSigorta.TasinmazId ;

            Page.Response.Redirect(newUrl, true);
        }
        protected void PrevBtn_Click(object sender, EventArgs e)
        {
            Sigorta sigorta = new Sigorta();
            sigorta = new Model.Services.TBYS.SigortaService(LoggingContext).GetById(SigortaIdQS.ConvertToInt());
            Sigorta oncekiSigorta = new Model.Services.TBYS.SigortaService(LoggingContext).GetPrev(sigorta.Id);
            string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            int queryIndex = newUrl.IndexOf("?");
            if (queryIndex > 0)
                newUrl = newUrl.Substring(0, queryIndex);
            newUrl = newUrl + "?DestinationApp=SigortaD&SenderApp=SigortaL&SigortaId=" + oncekiSigorta.Id + "&TasinmazId=" + oncekiSigorta.TasinmazId;
            Page.Response.Redirect(newUrl, true);
        }

        protected void MulkiyetiOlmayanTasinmazBtn_Click(object sender, EventArgs e)
        {

        }

        protected void SigBasTarTxt_TextChanged(object sender, EventArgs e)
        {
            DateTime bastar = SigBasTarTxt.Text.ConvertToDatetime();
            SigBitTarTxt.Text = bastar.AddYears(1).ToString("dd.MM.yyyy");
        }

        protected void BagimsizBolumDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            BagimsizBolumNoTxt.Text = BagimsizBolumDDL.SelectedItem.Text;
        }
        #region dosya yukle/goruntule
        protected void BelgeSilBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Sigorta sigorta = new Sigorta();
                sigorta = new Model.Services.TBYS.SigortaService(LoggingContext).GetById(SigortaIdQS.ConvertToInt());
                if (sigorta != null)
                {
                    string dosyaAdi = sigorta.PDFDosyasi;// ProjeConstants.DOSYA_SIGORTAPOLICESI_DASK + AdresKoduQS + ".pdf";
                    if (!string.IsNullOrEmpty(dosyaAdi) )
                    {

                        if (UtilityHelper.DeleteFileFromSharePointLib(ProjeConstants.PATH_TBYS_URL, ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi, LoggingContext))
                        {
                            sigorta.PDFDosyasi = null;
                            if (!new Model.Services.TBYS.SigortaService(LoggingContext).UpdateDocument(sigorta))
                                throw new InvalidOperationException("Silinen belgenin sigorta bağlantısı güncellenemedi.");
                            MessageHelper.PublishMessage(dosyaAdi + " Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                            BelgeSilBtn.Visible = false;
                            DosyaLnk.Visible = false;
                            BelgeYukleFU.Visible = true;

                            if (sigorta != null)
                                SigortaFormunuDoldur(sigorta);
                        }
                        else
                        {
                            MessageHelper.PublishMessage(dosyaAdi + " Silinemedi", ProjeConstants.MESAJ_HATA);
                        }
                    }
                    else
                    {
                        MessageHelper.PublishMessage(dosyaAdi + " Bulunamadı", ProjeConstants.MESAJ_HATA);
                    }



                }

            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazSigortaGirisWP.BelgeSilBtn_Click", ex);
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.TasinmazSigortaGirisWP.BelgeSilBtn_Click", ex);
            }
        }
        private void PDFKaydet(Sigorta sigorta, bool throwOnError = false)
        {
            try
            {
                if (sigorta == null || sigorta.Id <= 0) return;
                if (BelgeYukleFU.HasFile)
                {
                    string hedefDosyaAdi = (ProjeConstants.DOSYA_SIGORTAPOLICESI_DASK + AdresKoduQS).Trim() + ".pdf";
                    bool isOk = UtilityHelper.UploadFileToSharePoint(BelgeYukleFU, ProjeConstants.TBYSBELGELERI_LIB, hedefDosyaAdi, LoggingContext);
                    if (isOk)
                    {
                        sigorta.PDFDosyasi = (hedefDosyaAdi).Trim();
                        if (!new Model.Services.TBYS.SigortaService(LoggingContext).UpdateDocument(sigorta))
                            throw new InvalidOperationException("Yüklenen belgenin sigorta bağlantısı kaydedilemedi.");
                        DosyaLnk.Visible = true;
                        BelgeSilBtn.Visible = true;
                        BelgeYukleFU.Visible = false;
                    }
                    else
                    {
                        if (throwOnError) throw new InvalidOperationException("Poliçe belgesi yüklenemedi.");
                        DosyaLnk.Visible = false;
                        BelgeSilBtn.Visible = false;
                        BelgeYukleFU.Visible = true;

                    }
                }
                else
                {
                    DosyaLnk.Visible = false;
                    BelgeSilBtn.Visible = false;
                }
            }
            catch (Exception exception)
            {
                if (exception is System.Threading.ThreadAbortException) throw;
                if (throwOnError) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazSigortaGirisWP.PDFKaydet", exception);
                Exception ex = new Exception("Dosya Yüklenemedi");
                ExceptionHelper exhelper = new ExceptionHelper(exception);
                exhelper.Exceptions.Add(ex);
                TbysWebPartLog.ReportErrors(LoggingContext, "TBYS.TasinmazSigortaGirisWP.PDFKaydet", exhelper.Exceptions);
            }
        }
        private void PDFGoster(Sigorta sigorta, bool throwOnError = false)
        {
            try
            {
                if (sigorta != null)
                {
                    string dosyaAdi = (sigorta.PDFDosyasi ?? string.Empty).Trim();
                    if (string.IsNullOrEmpty(dosyaAdi))
                    {
                        DosyaLnk.Visible = false;
                        BelgeSilBtn.Visible = false;
                        BelgeYukleFU.Visible = true;
                        return;
                    }
                    string dosyaUrl = UtilityHelper.TbysBelgelerURLGetir() + "/" + dosyaAdi;
                    bool dosyaVarMi = UtilityHelper.DosyaVarMi(UtilityHelper.TbysBelgelerURLGetir(), ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi);
                    if (dosyaVarMi)
                    {
                        string belgePdfLink = @"'<a class=\'btn btn-secondary\' data-fancybox data-type=pdf data-width=960 data-height=720 href=" + dosyaUrl + @"> Poliçe Görüntüle </a>'";

                        DosyaLnk.Target = "_blank";
                        DosyaLnk.HRef = dosyaUrl;

                        DosyaLnk.Visible = true;
                        BelgeSilBtn.Visible = true;
                        BelgeYukleFU.Visible = false;
                    }
                    else
                    {
                        DosyaLnk.Visible = false;
                        BelgeSilBtn.Visible = false;
                        BelgeYukleFU.Visible = true;
                    }

                }
                else
                {
                    MessageHelper.PublishMessage("Poliçe bulunamadı.", ProjeConstants.MESAJ_HATA);
                }

            }
            catch (Exception exception)
            {
                if (exception is System.Threading.ThreadAbortException) throw;
                if (throwOnError) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.TasinmazSigortaGirisWP.PDFGoster", exception);
                Exception ex = new Exception("PDF Yüklenemedi");
                ExceptionHelper exhelper = new ExceptionHelper(exception);
                exhelper.Exceptions.Add(ex);
                TbysWebPartLog.ReportErrors(LoggingContext, "TBYS.TasinmazSigortaGirisWP.PDFGoster", exhelper.Exceptions);
            }
        }
        #endregion
    }
}
