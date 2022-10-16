using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.BagisciTalepleriWP
{
    [ToolboxItemAttribute(false)]
    public partial class BagisciTalepleriWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BagisciTalepleriWP()
        {
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
                    bagisci = bagisci.Select<TasinmazBagisci>(BagisciIdQS.ConvertToInt());
                    if (bagisci != null)
                    {
                        BagisciTalepleriTablosunuDoldur(bagisci);
                    } 
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void BagisciTalepleriTableHeaders()
        {
            BagisciTalepleriTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell TalepCell = new TableHeaderCell();
            TalepCell.Text = "Talep";
            TableHeaderCell IrtibatCell = new TableHeaderCell();
            IrtibatCell.Text = "İrtibat";
            TableHeaderCell TarihCell = new TableHeaderCell();
            TarihCell.Text = "Uygulanacağı Zaman";
            TableHeaderCell AciklamaCell = new TableHeaderCell();
            AciklamaCell.Text = "Açıklama";
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
            List<BagisciTalepleri> list = bt.SelectByBagisciId(bagisci.Id);
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
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
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
            bt = bt.Select<BagisciTalepleri>(talepId);
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
                bt.Degistiren = UtilityHelper.GetCurrentUser();
                bt.DegistirmeTarihi = DateTime.Today;
                bt.Irtibat = IrtibatTxt.Text;
                bt.Talep = TalepTxt.Text;
                bt.Tarih = TarihTxt.Text;

                int id = bt.Save();
                if (id > 0)
                    MessageHelper.PublishMessage("Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_TALEPLERI + "?BagisciId=" + BagisciIdQS);

            }
            catch (Exception ex)
            {

                ExceptionHelper eh = new ExceptionHelper(ex);
                eh.PublishException();
            }
        }
        protected void TalepGuncelleBtn_Click(object sender, EventArgs e)
        {
            try
            {

                int talepId = ParamTalepIdLbl.Value.ConvertToInt();
                BagisciTalepleri bt = new BagisciTalepleri();
                bt = bt.Select<BagisciTalepleri>(talepId);
                if (bt != null)
                {
                    bt.Aciklama = TalepAciklamaTxt.Text;
                    bt.Degistiren = UtilityHelper.GetCurrentUser();
                    bt.DegistirmeTarihi = DateTime.Today;
                    bt.Irtibat = IrtibatTxt.Text;
                    bt.Talep = TalepTxt.Text;
                    bt.Tarih = TarihTxt.Text;
                    if (bt.Update())
                        MessageHelper.PublishMessage("Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                    else
                        MessageHelper.PublishMessage("Kaydedilemedi", ProjeConstants.MESAJ_HATA);
                    RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_TALEPLERI + "?BagisciId=" + BagisciIdQS);
                }
            }
            catch (Exception ex)
            {

                ExceptionHelper eh = new ExceptionHelper(ex);
                eh.PublishException();
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
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void TalepSilNowBtn_Click(object sender, EventArgs e)
        {
            int talepId = ParamTalepIdLbl.Value.ConvertToInt();
            BagisciTalepleri bt = new BagisciTalepleri();
            bt = bt.Select<BagisciTalepleri>(talepId);
            if (bt != null)
            {
                if (bt.Delete())
                    RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_TALEPLERI + "?BagisciId=" + BagisciIdQS);

            }
            else
                MessageHelper.PublishMessage("Talep bulunamadı",ProjeConstants.MESAJ_BILGI);

        }
    }
}
