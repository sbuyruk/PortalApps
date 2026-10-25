using Model.Ortak;
using Model.TBYS;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Utility.Logging;
using TBYS_WebParts.Logging;

namespace TBYS_WebParts.BagisciTalepleriWP
{
    [ToolboxItemAttribute(false)]
    public partial class BagisciTalepleriWP : WebPart
    {
        private PortalLogContext LoggingContext { get { return PortalLogContext.FromCurrentRequest("TBYS", "BagisciTalepleriWP"); } }
        private void ReportError(string operation, Exception error)
        {
            TbysWebPartLog.ReportError(LoggingContext, "TBYS.BagisciTalepleriWP." + operation, error);
        }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BagisciTalepleriWP()
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
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.BagisciTalepleriWP.Page_Load", error);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string BagisciIdQS
        {
            get
            {

                if (ViewState["BagisciId"] == null)
                {
                    if (Page.Request.QueryString["BagisciId"] != null)
                    {
                        ViewState["BagisciId"] = Page.Request.QueryString["BagisciId"];
                    }
                    else
                    {
                        ViewState["BagisciId"] = string.Empty;
                    }
                }
                return ViewState["BagisciId"].ToString();
            }

            set
            {
                ViewState["BagisciId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    TasinmazBagisci bagisci = new TasinmazBagisci();
                    bagisci = new Model.Services.TBYS.TasinmazBagisciService(LoggingContext).GetById(BagisciIdQS.ConvertToInt());
                    if (bagisci != null)
                    {
                        BagisciTalepleriTablosunuDoldur(bagisci);
                    } 
                }
            }
            catch (Exception tbysError)
            {
                if (tbysError is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.BagisciTalepleriWP.Page_Load", tbysError);

                throw;
            }
        }
        private void BagisciTalepleriTableHeaders()
        {
            BagisciTalepleriTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sira";
            TableHeaderCell TalepCell = new TableHeaderCell();
            TalepCell.Text = "Talep";
            TableHeaderCell IrtibatCell = new TableHeaderCell();
            IrtibatCell.Text = "Irtibat";
            TableHeaderCell TarihCell = new TableHeaderCell();
            TarihCell.Text = "Uygulanacagi Zaman";
            TableHeaderCell AciklamaCell = new TableHeaderCell();
            AciklamaCell.Text = "Açiklama";
            TableHeaderCell DuzenleCell = new TableHeaderCell();
            DuzenleCell.Text = "Düzenle";
            TableHeaderCell SilCell = new TableHeaderCell();
            SilCell.Text = "Sil";

            th.Controls.Add(siraCell);
            th.Controls.Add(TalepCell);
            th.Controls.Add(IrtibatCell);
            th.Controls.Add(TarihCell);
            th.Controls.Add(AciklamaCell);
            th.Controls.Add(DuzenleCell);
            th.Controls.Add(SilCell);
            BagisciTalepleriTable.Controls.Add(th);
        }
        private void BagisciTalepleriTablosunuDoldur(TasinmazBagisci bagisci)
        {
            AdiLbl.Text = " Bağışçı : " + bagisci.Adi + " " + bagisci.Soyadi;
            BagisciIdLbl.Text = bagisci.Id + "";
            //Column headers
            BagisciTalepleriTableHeaders();
            BagisciTalepleri bt = new BagisciTalepleri();
            List<BagisciTalepleri> list = new BagisciTalepleriService(LoggingContext).GetByBagisciId(bagisci.Id);
            int SiraNo = 1;
            foreach (BagisciTalepleri item in list)
            {
                TableRow row = new TableRow();

                TableCell SiranoCell = new TableCell();

                SiranoCell.Text = SiraNo++ + "";
                row.Controls.Add(SiranoCell);

                TableCell TalepCell = new TableCell();
                TalepCell.Text = item.Talep;
                row.Controls.Add(TalepCell);

                TableCell IrtibatCell = new TableCell();
                IrtibatCell.Text = item.Irtibat;
                row.Controls.Add(IrtibatCell);

                TableCell TarihCell = new TableCell();
                TarihCell.Text = item.Tarih;
                row.Controls.Add(TarihCell);

                TableCell AciklamaCell = new TableCell();
                AciklamaCell.Text = item.Aciklama;
                AciklamaCell.Width = new Unit("40%");
                row.Controls.Add(AciklamaCell);

                TableCell DuzenleCell = new TableCell();
                string duzenleLink = "<a href=# onclick=OpenModalTalep("+item.Id+ "); class=\'btn btn-outline-primary \'> Düzenle</a>";
                DuzenleCell.Text=duzenleLink;
                row.Controls.Add(DuzenleCell);
                TableCell SilCell = new TableCell();
                string silLink = "<a href=# onclick=OpenTalepSilModal(" + item.Id + "); class=\'btn btn-outline-danger \'> Sil</a>";
                SilCell.Text=silLink;
                row.Controls.Add(SilCell);

                BagisciTalepleriTable.Controls.Add(row);
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
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.BagisciTalepleriWP.CloseBtn_Click", ex);
                ReportError("Unhandled", ex);
            }
        }
        protected void BagisciBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_GIRIS + "?DestinationApp=TBD&BagisciId=" + BagisciIdQS);
        }

        protected void TalepModalDoldurBtnBtn_Click(object sender, EventArgs e)
        {
            TalepKaydetBtn.Visible = false;
            TalepGuncelleBtn.Visible = false;
            int talepId = ParamTalepIdLbl.Value.ConvertToInt();
            BagisciTalepleri bt = new BagisciTalepleri();
            bt = new BagisciTalepleriService(LoggingContext).GetById(talepId);
            if (bt != null)
            {
                TalepTxt.Text = bt.Talep;
                IrtibatTxt.Text = bt.Irtibat;
                TarihTxt.Text = bt.Tarih;
                TalepAciklamaTxt.Text = bt.Aciklama;

                TalepGuncelleBtn.Visible = true;
            }
            else
            {
                TalepTxt.Text = IrtibatTxt.Text = TarihTxt.Text = TalepAciklamaTxt.Text = string.Empty;
                TalepKaydetBtn.Visible = true;
            }

        }
        protected void TalepKaydetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                int talepId = ParamTalepIdLbl.Value.ConvertToInt();
                BagisciTalepleri bt = new BagisciTalepleri();

                bt.Aciklama = TalepAciklamaTxt.Text;
                bt.BagisciId = BagisciIdQS.ConvertToInt();
                bt.Degistiren = UtilityHelper.GetCurrentUserLoginName();
                bt.DegistirmeTarihi = DateTime.Today;
                bt.Irtibat = IrtibatTxt.Text;
                bt.Talep = TalepTxt.Text;
                bt.Tarih = TarihTxt.Text;

                int id = new BagisciTalepleriService(LoggingContext).Save(bt);
                if (id > 0)
                    Messages.Success("Kaydedildi", 2000);
                RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_TALEPLERI + "?BagisciId=" + BagisciIdQS);

            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.BagisciTalepleriWP.TalepKaydetBtn_Click", ex);

                ReportError("Unhandled", ex);
            }
        }
        protected void TalepGuncelleBtn_Click(object sender, EventArgs e)
        {
            try
            {

                int talepId = ParamTalepIdLbl.Value.ConvertToInt();
                BagisciTalepleri bt = new BagisciTalepleri();
                bt = new BagisciTalepleriService(LoggingContext).GetById(talepId);
                if (bt == null || bt.BagisciId != BagisciIdQS.ConvertToInt())
                {
                    Messages.Error("Kaydın bağışçı ilişkisi değişmiş veya kayıt bulunamıyor. Sayfayı yenileyiniz.");
                    return;
                }
                if (bt != null)
                {
                    bt.Aciklama = TalepAciklamaTxt.Text;
                    bt.Degistiren = UtilityHelper.GetCurrentUserLoginName();
                    bt.DegistirmeTarihi = DateTime.Today;
                    bt.Irtibat = IrtibatTxt.Text;
                    bt.Talep = TalepTxt.Text;
                    bt.Tarih = TarihTxt.Text;
                    if (new BagisciTalepleriService(LoggingContext).Update(bt))
                        Messages.Success("Kaydedildi", 2000);
                    else
                        Messages.Error("Kaydedilemedi");
                    RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_TALEPLERI + "?BagisciId=" + BagisciIdQS);
                }
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.BagisciTalepleriWP.TalepGuncelleBtn_Click", ex);

                ReportError("Unhandled", ex);
            }

        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                int index = currentUrl.LastIndexOf("?") < 0 ? currentUrl.Length : currentUrl.LastIndexOf("?");

                string rawUrl = currentUrl.Substring(0, index);
                string newUrl = rawUrl.Substring(0, rawUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.BagisciTalepleriWP.RedirectToPage", ex);
                ReportError("Unhandled", ex);
            }
        }
        protected void TalepSilNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                BagisciTalepleri item = new Model.Services.TBYS.BagisciTalepleriService(LoggingContext).GetById(ParamTalepIdLbl.Value.ConvertToInt());
                if (item == null || item.BagisciId != BagisciIdQS.ConvertToInt())
                {
                    Messages.Error("Talep kaydı değişmiş veya bulunamıyor. Sayfayı yenileyip tekrar deneyiniz.");
                    return;
                }
                if (!new Model.Services.TBYS.BagisciTalepleriService(LoggingContext).Delete(item))
                {
                    Messages.Error("Talep kaydı silinemedi.");
                    return;
                }
                RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_TALEPLERI + "?BagisciId=" + BagisciIdQS);
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.BagisciTalepleriWP.TalepSilNowBtn_Click", error);
            }
        }
    }
}
