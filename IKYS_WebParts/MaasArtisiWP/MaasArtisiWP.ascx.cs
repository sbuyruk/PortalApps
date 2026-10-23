using DAO.Repositories.Ortak;
using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.MaasArtisiWP
{
    [ToolboxItem(false)]
    public partial class MaasArtisiWP : WebPart
    {
        private sealed class SalaryInputException : Exception
        {
            internal SalaryInputException(string message) : base(message) { }
        }
        private PortalLogContext logContext;
        private PortalLogContext LoggingContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("IKYS", "MaasArtisiWP")); } }
        private SalaryScaleWriteService Writer { get { return new SalaryScaleWriteService(LoggingContext); } }
        private bool failed;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            ChromeType = PartChromeType.None;
        }

        private void Run(string operation, Action action)
        {
            if (failed) return;
            try { action(); }
            catch (ThreadAbortException) { throw; }
            catch (SalaryInputException warning) { failed = true; Messages.Warning(warning.Message); }
            catch (Exception error)
            {
                failed = true;
                PortalTechnicalLog.WriteError(LoggingContext, "IKYS.MaasArtisiWP." + operation, error);
                Messages.Error("Maaş artışı işlemi tamamlanamadı. Lütfen yeniden deneyin.", referenceCode: LoggingContext.OperationId.ToString("N"));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Run("Page_Load", () =>
            {
                if (!Page.IsPostBack)
                {
                    new PortalLogRepository().TryRecordWebPartOpened(LoggingContext);
                    ApplySnapshot(Writer.Load());
                }
            });
            if (failed) KaydetBtn.Enabled = SilBtn.Enabled = KaydetNowBtn.Enabled = DeleteNowBtn.Enabled = false;
        }

        private void ApplySnapshot(SalaryScaleSnapshot snapshot)
        {
            ViewState["SalaryVersion"] = snapshot.Version;
            ViewState["CanDelete"] = snapshot.CanDelete;
            KaydetBtn.Enabled = snapshot.Rows.Count > 0;
            SilBtn.Enabled = snapshot.CanDelete;
            if (snapshot.Rows.Count == 0) throw new SalaryInputException("Ücret tablosu bulunamadı. Önce mevcut ücret tanımlarını kontrol edin.");
            var culture = CultureInfo.GetCultureInfo("tr-TR");
            DateTime from = snapshot.Rows.Max(row => row.BitisTarihi).Date.AddDays(1);
            BaslangicTarihiTxt.Text = from.ToString("dd.MM.yyyy", culture);
            BitisTarihiTxt.Text = from.AddMonths(6).AddDays(-1).ToString("dd.MM.yyyy", culture);
            AgiTxt.Text = snapshot.Rows[0].Agi.ToString("0.00", culture);
            ArtisYuzdesiTxt.Text = "10,00";
        }

        private void ReadForm(out DateTime from, out DateTime to, out decimal increase, out decimal allowance)
        {
            var culture = CultureInfo.GetCultureInfo("tr-TR");
            if (!DateTime.TryParseExact(BaslangicTarihiTxt.Text, "dd.MM.yyyy", culture, DateTimeStyles.None, out from)
                || !DateTime.TryParseExact(BitisTarihiTxt.Text, "dd.MM.yyyy", culture, DateTimeStyles.None, out to)
                || from < new DateTime(1753, 1, 1) || to < from)
                throw new SalaryInputException("Geçerli bir tarih aralığı girin. Bitiş tarihi başlangıçtan önce olamaz.");
            const NumberStyles styles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
            if (!decimal.TryParse(ArtisYuzdesiTxt.Text, styles, culture, out increase) || increase < 0
                || !decimal.TryParse(AgiTxt.Text, styles, culture, out allowance) || allowance < 0)
                throw new SalaryInputException("Artış yüzdesi ve AGİ için sıfır veya pozitif bir tutar girin. Ondalık ayırıcı olarak virgül kullanın.");
        }

        private string ConfirmedVersion(string action)
        {
            string version = ViewState["ConfirmationVersion"] as string;
            if (!object.Equals(ViewState["ConfirmationAction"], action) || string.IsNullOrEmpty(version))
                throw new SalaryInputException("İşlem onayı bulunamadı. Sayfayı yenileyip işlemi yeniden onaylayın.");
            return version;
        }

        private static void RequireCompleted(SalaryScaleResult result)
        {
            switch (result)
            {
                case SalaryScaleResult.Completed: return;
                case SalaryScaleResult.Changed: throw new SalaryInputException("Maaş tabloları başka bir işlemde değişti. Sayfayı yenileyip güncel bilgileri yeniden onaylayın.");
                case SalaryScaleResult.NotFound: throw new SalaryInputException("Ücret tablosu bulunamadı. Sayfayı yenileyip mevcut tanımları kontrol edin.");
                case SalaryScaleResult.LastGroup: throw new SalaryInputException("İlk ve tek ücret tablosu silinemez.");
                case SalaryScaleResult.OverlappingPeriod: throw new SalaryInputException("Seçilen tarih aralığı mevcut bir ücret dönemiyle çakışıyor. Tarihleri kontrol edin.");
                default: throw new SalaryInputException("Tarih, artış yüzdesi ve AGİ tutarını kontrol edip yeniden deneyin.");
            }
        }

        private void Finish(SalaryScaleSnapshot snapshot, string message)
        {
            ViewState.Remove("ConfirmationVersion");
            ViewState.Remove("ConfirmationAction");
            try { ApplySnapshot(snapshot); }
            catch (ThreadAbortException) { throw; }
            catch (Exception error)
            {
                PortalTechnicalLog.WriteError(LoggingContext, "IKYS.MaasArtisiWP.AfterCommit", error);
                KaydetBtn.Enabled = SilBtn.Enabled = KaydetNowBtn.Enabled = DeleteNowBtn.Enabled = false;
                Messages.Warning(message + " Ekran yenilenemedi. İşlemi tekrarlamadan önce sayfayı yeniden açın. Başvuru kodu: " + LoggingContext.OperationId.ToString("N"));
                return;
            }
            Messages.Success(message);
        }

        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            Run("KaydetBtn_Click", () =>
            {
                DateTime from, to; decimal increase, allowance;
                ReadForm(out from, out to, out increase, out allowance);
                ViewState["ConfirmationVersion"] = ViewState["SalaryVersion"];
                ViewState["ConfirmationAction"] = "Create";
                MessageTitleLbl.Text = "Maaş Artışı";
                MessageTextLbl.Text = HttpUtility.HtmlEncode(from.ToString("dd.MM.yyyy") + "–" + to.ToString("dd.MM.yyyy")
                    + " dönemi için %" + increase.ToString("0.00", CultureInfo.GetCultureInfo("tr-TR")) + " artışla maaş tabloları oluşturulsun mu?");
                DeleteNowBtn.Visible = false; KaydetNowBtn.Visible = true;
                UtilityHelper.ScriptCalistir("OpenModal();");
            });
        }

        protected void SilBtn_Click(object sender, EventArgs e)
        {
            Run("SilBtn_Click", () =>
            {
                if (!object.Equals(ViewState["CanDelete"], true)) throw new SalaryInputException("Silinebilecek bir maaş artışı bulunamadı. İlk ücret tablosu korunur.");
                ViewState["ConfirmationVersion"] = ViewState["SalaryVersion"];
                ViewState["ConfirmationAction"] = "Delete";
                MessageTitleLbl.Text = "Son Maaş Artışını Sil";
                MessageTextLbl.Text = "Son maaş artışına ait ücret tablosu ve bağlı maaş listeleri birlikte silinsin mi?";
                DeleteNowBtn.Visible = true; KaydetNowBtn.Visible = false;
                UtilityHelper.ScriptCalistir("OpenModal();");
            });
        }

        protected void KaydetNowBtn_Click(object sender, EventArgs e)
        {
            Run("KaydetNowBtn_Click", () =>
            {
                string version = ConfirmedVersion("Create");
                DateTime from, to; decimal increase, allowance;
                ReadForm(out from, out to, out increase, out allowance);
                SalaryScaleSnapshot saved;
                RequireCompleted(Writer.Create(from, to, increase, allowance, version, out saved));
                Finish(saved, "Maaş tabloları oluşturuldu.");
            });
        }

        protected void DeleteNowBtn_Click(object sender, EventArgs e)
        {
            Run("DeleteNowBtn_Click", () =>
            {
                SalaryScaleSnapshot saved;
                RequireCompleted(Writer.Delete(ConfirmedVersion("Delete"), out saved));
                Finish(saved, "Son maaş artışı ve bağlı maaş listeleri silindi.");
            });
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            Run("CloseBtn_Click", () =>
            {
                string current = Page.Request.Url.AbsolutePath;
                Page.Response.Redirect(current.Substring(0, current.LastIndexOf('/') + 1) + ProjeConstants.PAGE_HOME);
            });
        }
    }
}
