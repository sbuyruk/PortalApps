using Utility.Logging;
using NBYS_WebParts.Logging;
using System.Globalization;
using Model.NBYS;
using Model.Ortak;
using Model.Services.NBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.ArmaganEditWP
{
    [ToolboxItemAttribute(false)]
    public partial class ArmaganEditWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "ArmaganEditWP"); }
        }
        private bool canProcess = true;
        private Bolge userRegion;
        private void ExecuteAction(string operation, Action action)
        {
            NbysWebPartLog.ExecuteAction(LoggingContext, operation, () => { if (canProcess) action(); });
        }
        private void StopForWarning(string message)
        {
            canProcess = false;
            Messages.Warning(message);
        }
        private bool CanReadDonor(int donorId)
        {
            return new NbysBagisciWriteService(LoggingContext).CanReadDonor(donorId, userRegion);
        }

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ArmaganEditWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SecilenAyQS
        {
            get
            {

                if (ViewState["SecilenAy"] == null)
                {
                    if (Page.Request.QueryString["SecilenAy"] != null)
                    {
                        ViewState["SecilenAy"] = Page.Request.QueryString["SecilenAy"];
                    }
                    else
                    {
                        ViewState["SecilenAy"] = BagisTarihiTxt.Value.ConvertToDatetime().ToString("MM");
                    }
                }
                return ViewState["SecilenAy"].ToString();
            }

            set
            {
                ViewState["SecilenAy"] = value;
            }
        }
        private string SecilenYilQS
        {
            get
            {

                if (ViewState["SecilenYil"] == null)
                {
                    if (Page.Request.QueryString["SecilenYil"] != null)
                    {
                        ViewState["SecilenYil"] = Page.Request.QueryString["SecilenYil"];
                    }
                    else
                    {
                        ViewState["SecilenYil"] = string.Empty;
                    }
                }
                return ViewState["SecilenYil"].ToString();
            }

            set
            {
                ViewState["SecilenYil"] = value;
            }
        }
        private string SecilenArmaganTanimIdQS
        {
            get
            {

                if (ViewState["SecilenArmaganTanimId"] == null)
                {
                    if (Page.Request.QueryString["SecilenArmaganTanimId"] != null)
                    {
                        ViewState["SecilenArmaganTanimId"] = Page.Request.QueryString["SecilenArmaganTanimId"];
                    }
                    else
                    {
                        ViewState["SecilenArmaganTanimId"] = string.Empty;
                    }
                }
                return ViewState["SecilenArmaganTanimId"].ToString();
            }

            set
            {
                ViewState["SecilenArmaganTanimId"] = value;
            }
        }
        private string SecilenDurumQS
        {
            get
            {

                if (ViewState["SecilenDurum"] == null)
                {
                    if (Page.Request.QueryString["SecilenDurum"] != null)
                    {
                        ViewState["SecilenDurum"] = Page.Request.QueryString["SecilenDurum"];
                    }
                    else
                    {
                        ViewState["SecilenDurum"] = ProjeConstants.HEPSI.ToString();
                    }
                }
                return ViewState["SecilenDurum"].ToString();
            }

            set
            {
                ViewState["SecilenDurum"] = value;
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
                        ViewState["SecilenIl"] = ProjeConstants.HEPSI_INT.ToString();
                    }
                }
                return ViewState["SecilenIl"].ToString();
            }

            set
            {
                ViewState["SecilenIl"] = value;
            }
        }
        private string ArmaganIdQS
        {
            get
            {

                if (ViewState["ArmaganId"] == null)
                {
                    if (Page.Request.QueryString["ArmaganId"] != null)
                    {
                        ViewState["ArmaganId"] = Page.Request.QueryString["ArmaganId"];
                    }
                    else
                    {
                        ViewState["ArmaganId"] = string.Empty;
                    }
                }
                return ViewState["ArmaganId"].ToString();
            }

            set
            {
                ViewState["ArmaganId"] = value;
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
        private string DuzenliBagisciIdQS
        {
            get
            {

                if (ViewState["DuzenliBagisciId"] == null)
                {
                    if (Page.Request.QueryString["DuzenliBagisciId"] != null)
                    {
                        ViewState["DuzenliBagisciId"] = Page.Request.QueryString["DuzenliBagisciId"];
                    }
                    else
                    {
                        ViewState["DuzenliBagisciId"] = "-2";
                    }
                }
                return ViewState["DuzenliBagisciId"].ToString();
            }

            set
            {
                ViewState["DuzenliBagisciId"] = value;
            }
        }
        private int BolgeIdQS
        {
            get { return ViewState["BolgeId"] is int ? (int)ViewState["BolgeId"] : -1; }
            set { ViewState["BolgeId"] = value; }
        }
        private string GiftEditVersion
        {
            get { return ViewState["GiftEditVersion"] as string; }
            set { ViewState["GiftEditVersion"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
                userRegion = new NbysBagisciWriteService(LoggingContext).GetUserRegion();
                BolgeIdQS = userRegion == null ? -1 : userRegion.Id;
                int id;
                if (!int.TryParse(ArmaganIdQS, out id) || id <= 0)
                {
                    KaydetBtn.Visible = false;
                    StopForWarning("Geçerli bir armağan kaydı seçin."); return;
                }
                Armagan gift;
                if (!Page.IsPostBack)
                {
                    NbysGiftEditSnapshot snapshot;
                    NbysGiftWriteResult result = new NbysGiftWriteService(LoggingContext).LoadForEdit(id, out snapshot);
                    if (result != NbysGiftWriteResult.Completed)
                    {
                        KaydetBtn.Visible = false;
                        canProcess = false;
                        NbysGiftFeedback.Show(LoggingContext, result, string.Empty, () => { });
                        return;
                    }
                    gift = snapshot.Gift;
                    GiftEditVersion = snapshot.Version;
                }
                else
                {
                    gift = new ArmaganService(LoggingContext).GetById(id);
                }
                if (gift == null)
                {
                    KaydetBtn.Visible = false;
                    StopForWarning("Armağan kaydı bulunamadı. Listeyi yenileyip tekrar seçin."); return;
                }
                if (!CanReadDonor(gift.BagisciId))
                {
                    KaydetBtn.Visible = false;
                    StopForWarning("Bu armağan kaydını görüntüleme veya düzenleme yetkiniz yok."); return;
                }
                KaydetBtn.Visible = true;
                DurumDDL.Parent.Controls.Add(new System.Web.UI.LiteralControl(
                    "<small class=\"form-text text-muted\">Para iadesi için Bağış İadesi ekranını kullanın.</small>"));
                if (!Page.IsPostBack) { FillDropDownList(); FillArmaganForm(gift); }
                DurumDDL.Enabled = gift.Durum != ProjeConstants.DURUM_PARAIADE;
                ListItem refundItem = DurumDDL.Items.FindByValue(ProjeConstants.DURUM_PARAIADE);
                if (refundItem != null) refundItem.Attributes["disabled"] = "disabled";
            });
        }
        private void FillArmaganForm(Armagan armagan)
        {
            KaydetBtn.CssClass = "btn btn-outline-primary";
            KaydetBtn.Text = "Güncelle";
            if (armagan != null)
            {
                IdLbl.Text = armagan.Id.ToString();
                NakitBagisci nakitBagisci = new NakitBagisciService(LoggingContext).GetById(armagan.BagisciId);
                if (nakitBagisci != null)
                {
                    AdiTxt.Text = nakitBagisci.Adi.ReturnEmptyIfNull().ToString();
                }
                BagisMiktariTxt.Text = armagan.BagisMiktari.ToString(new CultureInfo(ProjeConstants.CULTUREINFO));

                ListItem definition = ArmaganDDL.Items.FindByValue(armagan.ArmaganTanimId.ToString());
                if (definition == null) throw new InvalidOperationException("Kayıtlı armağan türü bulunamadı.");
                ArmaganDDL.ClearSelection(); definition.Selected = true;
                ListItem status = DurumDDL.Items.FindByValue(armagan.Durum ?? string.Empty);
                if (status == null)
                {
                    status = new ListItem(armagan.Durum ?? string.Empty, armagan.Durum ?? string.Empty);
                    DurumDDL.Items.Add(status);
                }
                DurumDDL.ClearSelection(); status.Selected = true;
                BagisTarihiTxt.Value = armagan.Tarih.ToString("dd.MM.yyyy");
                AciklamaTxt.Text = armagan.Aciklama.ReturnEmptyIfNull().ToString();
                BelgedeYazanIsimTxt.Text = armagan.BelgedeYazanIsim.ReturnEmptyIfNull().ToString();
                BagisMiktariYazmasinChk.Checked = armagan.BagisMiktariYazmasin;
            }

        }
        private void FillDropDownList()
        {
            DurumDDLDoldur();
            FillArmaganTanim();
        }
        private void DurumDDLDoldur()
        {
            DurumDDL.Items.Add(ProjeConstants.DURUM_BELGE_ISTEMIYOR);
            DurumDDL.Items.Add(ProjeConstants.DURUM_GONDERILMEDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_GONDERILDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_ULASILAMADI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_KONTROLEDILDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_ERKENGONDERILDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_ERTELENDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_PARAIADE);
            DurumDDL.Items.Add(ProjeConstants.DURUM_DAHAONCEIADE);
            DurumDDL.Items.Add(ProjeConstants.DURUM_AFETNEDENIYLE_GONDERILMEDI);
            DurumDDL.Items.Add(ProjeConstants.DURUM_EDEVLETTENBELGEGONDERILDI);
        }
        private void FillArmaganTanim()
        {
            if (ArmaganDDL.SelectedItem == null)
            {
                ArmaganDDL.Items.Clear();
                List<ArmaganTanim> list = new ArmaganTanimService(LoggingContext).GetAll();
                foreach (ArmaganTanim arm in list)
                {
                    ArmaganDDL.Items.Add(new ListItem(arm.Armagan, arm.Id.ToString()));
                }
            }

        }
        protected void ArmaganDLL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("ArmaganDLL_SelectedIndexChanged", () =>
            {
                FillArmaganTanim();
            });
        }
        protected void DurumDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("DurumDDL_SelectedIndexChanged", () =>
            {
                FillArmaganTanim();
            });
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("KaydetBtn_Click", () =>
            {
                if (!Page.IsValid) return;
                int id, definitionId;
                decimal amount;
                DateTime date;
                if (!int.TryParse(ArmaganIdQS, out id) || id <= 0)
                { Messages.Warning("Geçerli bir armağan kaydı seçin."); return; }
                if (!int.TryParse(ArmaganDDL.SelectedValue, out definitionId))
                { Messages.Warning("Geçerli bir armağan türü seçin."); return; }
                if (!decimal.TryParse(BagisMiktariTxt.Text, NumberStyles.Number, new CultureInfo(ProjeConstants.CULTUREINFO), out amount) || amount < 0)
                { Messages.Warning("Bağış miktarı sıfır veya pozitif bir sayı olmalıdır."); return; }
                if (!DateTime.TryParseExact(BagisTarihiTxt.Value, "dd.MM.yyyy", new CultureInfo(ProjeConstants.CULTUREINFO), DateTimeStyles.None, out date)
                    || date < new DateTime(1753, 1, 1))
                { Messages.Warning("Geçerli bir bağış tarihi girin."); return; }
                Armagan input = new Armagan { Id = id, ArmaganTanimId = definitionId, BagisMiktari = amount,
                    Durum = DurumDDL.SelectedValue, Tarih = date, Aciklama = AciklamaTxt.Text,
                    BelgedeYazanIsim = BelgedeYazanIsimTxt.Text, BagisMiktariYazmasin = BagisMiktariYazmasinChk.Checked };
                NbysGiftEditSnapshot saved;
                NbysGiftWriteResult result = new NbysGiftWriteService(LoggingContext).Update(input, GiftEditVersion, out saved);
                NbysGiftFeedback.Show(LoggingContext, result, "Armağan kaydı güncellendi.", () =>
                {
                    if (saved == null) throw new InvalidOperationException("Güncellenen armağan okunamadı.");
                    FillArmaganForm(saved.Gift);
                    GiftEditVersion = saved.Version;
                    DurumDDL.Enabled = saved.Gift.Durum != ProjeConstants.DURUM_PARAIADE;
                });
            });
        }
        protected void ArmaganListesiBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ArmaganListesiBtn_Click", () =>
            {
                string queryStr = "?SecilenId=" + ArmaganIdQS + "&SecilenAy=" + SecilenAyQS + "&SecilenYil="
                + SecilenYilQS + "&SecilenArmaganTanimId=" + SecilenArmaganTanimIdQS + "&SecilenDurum=" + System.Web.HttpUtility.UrlEncode(SecilenDurumQS) + "&SecilenIl=" + SecilenIlQS;
                RedirectToPage(ProjeConstants.PAGE_ARMAGAN_LIST + queryStr);
            });
        }
        protected void DuzenliBagisciListesiBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("DuzenliBagisciListesiBtn_Click", () =>
            {
                string queryStr = "?SecilenId=" + DuzenliBagisciIdQS + "&SecilenAy=" + SecilenAyQS + "&SecilenYil="
                + SecilenYilQS;
                RedirectToPage(ProjeConstants.PAGE_DUZENLIBAGISCI_LIST + queryStr);
            });
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl, false);
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
            });
        }
        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl, false);
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}
