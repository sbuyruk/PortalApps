using Model.NBYS;
using Model.Ortak;
using Model.Services.NBYS;
using NBYS_WebParts.Logging;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.BagisaVesileOlanTesekkurGirisiWP
{
    [ToolboxItem(false)]
    public partial class BagisaVesileOlanTesekkurGirisiWP : WebPart
    {
        private PortalLogContext LoggingContext { get { return PortalLogContext.FromCurrentRequest("NBYS", "BagisaVesileOlanTesekkurGirisiWP"); } }
        private bool canProcess = true;
        private void ExecuteAction(string operation, Action action)
        {
            NbysWebPartLog.ExecuteAction(LoggingContext, operation, () => { if (canProcess) action(); });
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            ChromeType = PartChromeType.None;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () => RedirectToPage(ProjeConstants.PAGE_HOME));
        }
        private void RedirectToPage(string pageUrl)
        {
            string current = Page.Request.Url.GetLeftPart(UriPartial.Path);
            string target = current.Substring(0, current.LastIndexOf('/') + 1) + pageUrl;
            Page.Response.Redirect(target, false);
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        private string SecilenIdQS
        {
            get
            {

                if (ViewState["SecilenId"] == null)
                {
                    if (Page.Request.QueryString["SecilenId"] != null)
                    {
                        ViewState["SecilenId"] = Page.Request.QueryString["SecilenId"];
                    }
                    else
                    {
                        ViewState["SecilenId"] = string.Empty;
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }

        private bool TrySelectedId(out int id)
        {
            id = 0;
            return string.IsNullOrWhiteSpace(SecilenIdQS) || (int.TryParse(SecilenIdQS, out id) && id >= 0);
        }
        private void ShowButtons(int id)
        {
            KaydetBtn.Visible = id == 0;
            GuncelleBtn.Visible = id > 0;
            DosyaOlusturBtn.Visible = id > 0;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
                int id;
                if (!TrySelectedId(out id))
                { canProcess = false; ShowButtons(0); KaydetBtn.Visible = false; Messages.Warning("Geçerli bir teşekkür kaydı seçin."); return; }
                ShowButtons(id);
                if (id > 0 && new BagisaVesileOlanTesekkurService(LoggingContext).GetById(id) == null)
                { canProcess = false; ShowButtons(0); KaydetBtn.Visible = false; Messages.Warning("Teşekkür kaydı bulunamadı. Listeyi yenileyip tekrar seçin."); return; }
                if (!Page.IsPostBack) FormuDoldur(id);
            });
        }
        private void FormuDoldur(int secilenId = 0)
        {
            if (secilenId < 1)
            {
                DateTime bugun = DateTime.Today;
                ImzalayanTxt.Text = @"Bilal TOPÇU";
                ImzalayanUnvanTxt.Text = string.Empty;
                ImzalayanMakamTxt.Text = @"Genel Müdür";
                BelgeTarihiTxt.Text = bugun.ToString("dd MMMM yyyy", new CultureInfo("tr-TR"));
            }else
            {
                BagisaVesileOlanTesekkur bagisaVesileOlanTesekkur = new BagisaVesileOlanTesekkurService(LoggingContext).GetById(secilenId);
                if (bagisaVesileOlanTesekkur != null && bagisaVesileOlanTesekkur.Id > 0)
                {
                    BelgeNoTxt.Text =bagisaVesileOlanTesekkur.Id.ToString();
                    AdiTxt.Text = bagisaVesileOlanTesekkur.Adi;
                    SoyadiTxt.Text = bagisaVesileOlanTesekkur.Soyadi;
                    UnvanTxt.Text = bagisaVesileOlanTesekkur.Unvan;
                    BelgeTarihiTxt.Text = bagisaVesileOlanTesekkur.BelgeTarihi.ToString("dd MMMM yyyy", new CultureInfo("tr-TR"));
                    AciklamaTxt.Text = bagisaVesileOlanTesekkur.Aciklama;
                    BelgeMetni1Txt.Text = bagisaVesileOlanTesekkur.BelgeMetni1;
                    BelgeMetni2Txt.Text = bagisaVesileOlanTesekkur.BelgeMetni2;
                    ImzalayanTxt.Text = bagisaVesileOlanTesekkur.ImzalayanAdiSoyadi;
                    ImzalayanMakamTxt.Text = bagisaVesileOlanTesekkur.ImzalayanMakam;
                    ImzalayanUnvanTxt.Text = bagisaVesileOlanTesekkur.ImzalayanUnvan;
                    TCKimlikNoTxt.Text = bagisaVesileOlanTesekkur.TCKimlikNo.ToString();
                    VerilmeSebebiTxt.Text = bagisaVesileOlanTesekkur.VerilmeSebebi;
                }
                else
                {
                    throw new InvalidOperationException("Teşekkür kaydı ekran yenilenirken okunamadı.");
                }
            }

        }
        private bool TryReadForm(out BagisaVesileOlanTesekkur item)
        {
            item = null;
            if (!Page.IsValid) return false;
            if (string.IsNullOrWhiteSpace(AdiTxt.Text)) { Messages.Warning("Belge düzenlenecek kişinin adını girin."); return false; }
            DateTime date;
            if (!DateTime.TryParseExact(BelgeTarihiTxt.Text.Trim(), new[] { "d MMMM yyyy", "dd MMMM yyyy", "dd.MM.yyyy", "d.M.yyyy", "yyyy-MM-dd" },
                new CultureInfo("tr-TR"), DateTimeStyles.AllowWhiteSpaces, out date) || date < new DateTime(1753, 1, 1))
            { Messages.Warning("Geçerli bir belge tarihi girin. Örnek: 22 Ekim 2026 veya 22.10.2026."); return false; }
            long identity = 0;
            string identityText = TCKimlikNoTxt.Text.Trim();
            if (identityText.Length > 0 && !long.TryParse(identityText, NumberStyles.None, CultureInfo.InvariantCulture, out identity))
            { Messages.Warning("Kimlik numarasını yalnızca rakamlarla girin veya boş bırakın."); return false; }
            item = new BagisaVesileOlanTesekkur
            {
                Adi = AdiTxt.Text.Trim(), Soyadi = SoyadiTxt.Text.Trim(), Unvan = UnvanTxt.Text,
                TCKimlikNo = identity, BelgeTarihi = date, Aciklama = AciklamaTxt.Text,
                BelgeMetni1 = BelgeMetni1Txt.Text, BelgeMetni2 = BelgeMetni2Txt.Text,
                ImzalayanAdiSoyadi = ImzalayanTxt.Text, ImzalayanUnvan = ImzalayanUnvanTxt.Text,
                ImzalayanMakam = ImzalayanMakamTxt.Text, VerilmeSebebi = VerilmeSebebiTxt.Text
            };
            return true;
        }

        private void PersistForm()
        {
            int id;
            if (!TrySelectedId(out id)) { Messages.Warning("Geçerli bir teşekkür kaydı seçin."); return; }
            BagisaVesileOlanTesekkur item;
            if (!TryReadForm(out item)) return;
            item.Id = id;
            NbysReferralWriteResult result = new BagisaVesileOlanTesekkurService(LoggingContext).Persist(item);
            if (result == NbysReferralWriteResult.Completed || result == NbysReferralWriteResult.Unchanged)
            {
                SecilenIdQS = item.Id.ToString(CultureInfo.InvariantCulture);
                DosyaLnk.Visible = false;
            }
            NbysReferralFeedback.Write(LoggingContext, result, id == 0 ? "Teşekkür kaydı oluşturuldu." : "Teşekkür kaydı güncellendi.",
                () => { FormuDoldur(item.Id); ShowButtons(item.Id); });
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("KaydetBtn_Click", PersistForm);
        }
        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("GuncelleBtn_Click", PersistForm);
        }
        protected void DosyaOlusturBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("DosyaOlusturBtn_Click", () =>
            {
                DosyaLnk.Visible = false;
                int id;
                if (!TrySelectedId(out id) || id <= 0) { Messages.Warning("Önce teşekkür kaydını kaydedin."); return; }
                BagisaVesileOlanTesekkur item = new BagisaVesileOlanTesekkurService(LoggingContext).GetById(id);
                if (item == null) { Messages.Warning("Teşekkür kaydı bulunamadı. Listeyi yenileyin."); return; }
                NbysReferralFeedback.Document(LoggingContext, item,
                    document => { DosyaLnk.Text = document.FileName; DosyaLnk.NavigateUrl = document.DownloadUrl; DosyaLnk.Visible = true; },
                    "Kayıtlı bilgiler kullanıldı (belge no: " + item.Id + ").");
            });
        }
        protected void ListeyeGitBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ListeyeGitBtn_Click", () =>
            {
                int id;
                TrySelectedId(out id);
                RedirectToPage(ProjeConstants.PAGE_BAGISAVESILE_LIST + "?SecilenId=" + id);
            });
        }
    }
}
