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

namespace NBYS_WebParts.BagisaVesileOlanArmaganBelgesiWP
{
    [ToolboxItem(false)]
    public partial class BagisaVesileOlanArmaganBelgesiWP : WebPart
    {
        private PortalLogContext LoggingContext { get { return PortalLogContext.FromCurrentRequest("NBYS", "BagisaVesileOlanArmaganBelgesiWP"); } }
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

        private int SavedReferralId
        {
            get { return ViewState["SavedReferralId"] is int ? (int)ViewState["SavedReferralId"] : 0; }
            set { ViewState["SavedReferralId"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
                var manager = System.Web.UI.ScriptManager.GetCurrent(Page);
                if (manager != null) manager.AsyncPostBackTimeout = 36000;
                if (!Page.IsPostBack) { FormuDoldur(); TabloOlustur(); }
            });
        }
        private void FormuDoldur()
        {
            DateTime bugun = DateTime.Today;

            ImzalayanTxt.Text = @"Bilal TOPÇU";
            ImzalayanUnvanTxt.Text = string.Empty;
            ImzalayanMakamTxt.Text = @"Genel Müdür";
            BelgeTarihiTxt.Text = bugun.ToString("dd MMMM yyyy", new CultureInfo("tr-TR"));
        }
        private void TabloOlustur()
        {
            string json = GetData();
            TableDataLbl.Text = json == "[]" ? "Teşekkür kaydı bulunmamaktadır." : string.Empty;
            // The form can create its first record even when the list is empty.
            DosyaOlusturBtn.Visible = true;
            UtilityHelper.ScriptCalistir(CreateDataTable(json));
        }
        private string GetData()
        {
            string json = new BagisaVesileOlanTesekkurService(LoggingContext).GetListJson();
            return json;
        }
        private string CreateDataTable(string jsonData)
        {

            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date

                jQuery('#CustomDataTable').DataTable({ 
                    data: " + jsonData + @",
                    columns: [
                        { data: 'Adi'},
                        { data: 'Soyadi'},
                        { data: 'VerilmeSebebi'},
                        { data: 'BelgeTarihiDDMMYYYY' },
                        { data: 'ImzalayanAdiSoyadi'},
                        { data: 'Aciklama'},

                    ],
                    columnDefs: [{ targets: [0, 1, 2, 3, 4, 5], render: $.fn.dataTable.render.text() }],
                    pageLength: 8,
                    'order': [[3, 'desc']],//sort date desc
                    'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    dom: 'frtip',   

                });

            ";

            return tableString;
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

        protected void DosyaOlusturBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("DosyaOlusturBtn_Click", () =>
            {
                DosyaLnk.Visible = false;
                BagisaVesileOlanTesekkur item;
                if (!TryReadForm(out item)) return;
                item.Id = SavedReferralId;
                NbysReferralWriteResult result = new BagisaVesileOlanTesekkurService(LoggingContext).Persist(item);
                if (!NbysReferralFeedback.CanContinue(result)) return;
                // Persist sets Id only after commit. Keep it before any SharePoint operation can fail.
                SavedReferralId = item.Id;
                NbysReferralFeedback.Document(LoggingContext, item, document =>
                {
                    DosyaLnk.Text = document.FileName;
                    DosyaLnk.NavigateUrl = document.DownloadUrl;
                    DosyaLnk.Visible = true;
                    TabloOlustur();
                }, (result == NbysReferralWriteResult.Completed ? "Teşekkür kaydı kaydedildi" : "Mevcut teşekkür kaydı kullanıldı")
                    + " (belge no: " + item.Id + ").");
            });
        }
    }
}
