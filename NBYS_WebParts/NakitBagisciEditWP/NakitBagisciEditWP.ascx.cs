using NBYS_WebParts.Logging;
using Utility.Logging;
using Model.NBYS;
using Model.Ortak;
using Model.Services.Ortak;
using Model.Services.NBYS;
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
        private bool canEditCurrentDonor;

        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "NakitBagisciEditWP"); }
        }

        private void ExecuteAction(string operation, Action action)
        {
            NbysWebPartLog.ExecuteAction(LoggingContext, operation, action);
        }
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
        protected void Page_Load(object sender, EventArgs e)
        {
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
                if (NakitBagisciIdQS.ConvertToInt()>0)
                {
                    Bolge kullanicininBolgesi = new NbysBagisciWriteService(LoggingContext).GetUserRegion();
                    BolgeIdQS = kullanicininBolgesi == null ? 0 : kullanicininBolgesi.Id;
                    if (kullanicininBolgesi == null)
                    {
                        KaydetBtn.Visible = false;
                        Messages.Warning("Bölge yetkiniz bulunamadı. Lütfen sistem yöneticisine başvurun.");
                        return;
                    }
                    bool kullaniciGenelMdluktenMi = (kullanicininBolgesi.Id == ProjeConstants.BOLGE_HEPSI_INT || kullanicininBolgesi.Id == ProjeConstants.BOLGE_GENELMUDURLUK_INT);
                    bool kullaniciYetkiliMi = new NbysBagisciWriteService(LoggingContext).CanReadDonor(NakitBagisciIdQS.ConvertToInt(), kullanicininBolgesi);
                    if (kullaniciGenelMdluktenMi || kullaniciYetkiliMi)
                    {
                        canEditCurrentDonor = true;
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
                    else
                    {
                        KaydetBtn.Visible = false;
                        Messages.Warning("Bu bağışçının bilgilerini görüntüleme yetkiniz yok.");
                    }
                }
                else
                {
                    KaydetBtn.Visible = false;
                    Messages.Warning("Bağışçı bulunamadı. Listeyi yenileyip tekrar seçin.");
                }
            });
        }

        

        private void FillIlData()
        {
            if (IliDDL.SelectedItem == null)
            {
                IliDDL.Items.Clear();
                IliDDL.Items.Add(new ListItem("", ProjeConstants.IL_BOS.ToString()));
                List<Il> list = new IlService().GetAll(LoggingContext);
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
                List<Ilce> list = new IlceService().GetByProvinceId(ilId, LoggingContext);
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

                NakitBagisciService service = new NakitBagisciService(LoggingContext);
                NakitBagisci nakitBagisci = service.GetById(nakitBagisciId);
                if (nakitBagisci != null)
                {
                    IdLbl.Text = nakitBagisciId.ToString();
                    AdiTxt.Text = nakitBagisci.Adi.ReturnEmptyIfNull().ToString();
                    TCKimlikNoTxt.Text = nakitBagisci.TCKimlikNo.ReturnEmptyIfNull().ToString();
                    AdresTxt.Text = nakitBagisci.Adres.ReturnEmptyIfNull().ToString();
                    MeslekTxt.Text = nakitBagisci.Meslek.ReturnEmptyIfNull().ToString();

                    //TextInfo culturInfo
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
            ExecuteAction("KaydetBtn_Click", () =>
            {
                if (!canEditCurrentDonor) return;
                if (string.IsNullOrWhiteSpace(AdiTxt.Text))
                {
                    Messages.Warning("Bağışçı adını girin.");
                    return;
                }
                long identityNumber;
                if (!string.IsNullOrWhiteSpace(TCKimlikNoTxt.Text) && (!System.Text.RegularExpressions.Regex.IsMatch(TCKimlikNoTxt.Text, "^[0-9]+$") || !long.TryParse(TCKimlikNoTxt.Text, out identityNumber)))
                {
                    Messages.Warning("T.C. kimlik numarasını yalnızca rakam kullanarak girin.");
                    return;
                }
                int id = NakitBagisciIdQS.ConvertToInt();
                NakitBagisci item = id > 0 ? new NakitBagisciService(LoggingContext).GetById(id) : new NakitBagisci();
                if (item == null)
                {
                    Messages.Warning("Bağışçı bulunamadı. Listeyi yenileyip tekrar seçin.");
                    return;
                }
                if (IliDDL.SelectedItem == null || IlcesiDDL.SelectedItem == null)
                {
                    Messages.Warning("İl ve ilçe seçenekleri yüklenemedi. Sayfayı yenileyip tekrar deneyin.");
                    return;
                }
                FillDonorFromForm(item);
                NbysBagisciSaveResult result = new NbysBagisciWriteService(LoggingContext).Save(item);
                if (result == NbysBagisciSaveResult.NotFound)
                    Messages.Warning("Bağışçı bulunamadı. Listeyi yenileyip tekrar seçin.");
                else if (result == NbysBagisciSaveResult.Forbidden)
                    Messages.Warning("Bu bağışçının bilgilerini değiştirme yetkiniz yok.");
                else if (result == NbysBagisciSaveResult.NameRequired)
                    Messages.Warning("Bağışçı adını girin.");
                else
                    Messages.Success("Bağışçı bilgileri kaydedildi.", 4000);
            });
        }
        private void FillDonorFromForm(NakitBagisci nakitBagisci)
        {
            nakitBagisci.Adi = AdiTxt.Text.ReturnEmptyIfNull().ToString();
            nakitBagisci.TCKimlikNo = TCKimlikNoTxt.Text.ConvertToLong();
            nakitBagisci.Adres = AdresTxt.Text.ReturnEmptyIfNull().ToString();
            nakitBagisci.Meslek = MeslekTxt.Text.ReturnEmptyIfNull().ToString();
            nakitBagisci.Ili = IliDDL.SelectedItem.Value.ConvertToInt();
            nakitBagisci.Ilcesi = IlcesiDDL.SelectedItem.Value.ConvertToInt();
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
            var user = UtilityHelper.GetCurrentUserLoginName();
            nakitBagisci.Degistiren = user;

        }

      
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            });
        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("IliDDL_SelectedIndexChanged", () =>
            {
                FillIlceData();
            });
        }
    }
}
