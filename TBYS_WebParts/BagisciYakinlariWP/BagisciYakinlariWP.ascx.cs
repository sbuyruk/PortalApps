using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.BagisciYakinlariWP
{
    [ToolboxItemAttribute(false)]
    public partial class BagisciYakinlariWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BagisciYakinlariWP()
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

            //önceki sayfayı tut, geri tuşuna basıldığında gerekli

            TasinmazBagisci bagisci = new TasinmazBagisci();
            bagisci = bagisci.Select<TasinmazBagisci>(BagisciIdQS.ConvertToInt());
            if (bagisci != null)
            {
                FillBagisciYakinlari2Table(bagisci);
            }
        }
        private void BagisciYakinlariTableHeaders()
        {
            BagisciYakinlariTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell AdisoyadCell = new TableHeaderCell();
            AdisoyadCell.Text = "Adı Soyadı";
            TableHeaderCell IrtibatCell = new TableHeaderCell();
            IrtibatCell.Text = "Telefon";
            TableHeaderCell YakinlikDerecesiCell = new TableHeaderCell();
            YakinlikDerecesiCell.Text = "Yakınlık Derecesi";


            th.Controls.Add(siraCell);
            th.Controls.Add(AdisoyadCell);
            th.Controls.Add(IrtibatCell);
            th.Controls.Add(YakinlikDerecesiCell);
            BagisciYakinlariTable.Controls.Add(th);
        }
        private void FillBagisciYakinlari2Table(TasinmazBagisci bagisci)
        {
            AdiLbl.Text = " Bağışçı : " + bagisci.Adi + " " + bagisci.Soyadi;
            BagisciIdLbl.Text = bagisci.Id + "";
            //Column headers
            BagisciYakinlariTableHeaders();
            BagisciYakinlari bt = new BagisciYakinlari();
            List<BagisciYakinlari> list = bt.SelectByBagisciId(bagisci.Id);
            int SiraNo = 1;
            foreach (BagisciYakinlari bagisciYakini in list)
            {
                TableRow row = new TableRow();

                TableCell SiranoCell = new TableCell();

                SiranoCell.Text = SiraNo++ + "";
                row.Controls.Add(SiranoCell);

                TableCell AdisoyadCell = new TableCell();
                AdisoyadCell.Text = bagisciYakini.AdSoyad;
                row.Controls.Add(AdisoyadCell);

                TableCell TelefonCell = new TableCell();
                TelefonCell.Text = bagisciYakini.Telefon;
                row.Controls.Add(TelefonCell);

                TableCell YakinlikDerecesiCell = new TableCell();
                YakinlikDerecesiCell.Text = bagisciYakini.YakinlikDerecesi;
                row.Controls.Add(YakinlikDerecesiCell);

                TableCell SilCell = new TableCell();
                LinkButton SilBtn = new LinkButton();
                SilBtn.Text = "Sil";
                SilBtn.CssClass = "btn btn-outline-danger";
                SilBtn.Click += delegate
                {
                    bagisciYakini.Delete();
                    string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    int queryIndex = newUrl.IndexOf("?");
                    if (queryIndex > 0)
                        newUrl = newUrl.Substring(0, queryIndex);
                    Page.Response.Redirect(newUrl + "?DestinationApp=TBD&BagisciId=" + bagisciYakini.BagisciId, true);
                };
                SilCell.Controls.Add(SilBtn);
                row.Controls.Add(SilCell);

                BagisciYakinlariTable.Controls.Add(row);
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
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_TASINMAZBAGISCI_GIRIS + "?DestinationApp=TBD&BagisciId=" + BagisciIdQS;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            BagisciYakinlari bagisciYakinlari = new BagisciYakinlari();
            bagisciYakinlari.AdSoyad = AdsoyadTxt.Text;
            bagisciYakinlari.Telefon = TelefonTxt.Text;
            bagisciYakinlari.YakinlikDerecesi = YakinlikDerecesiTxt.Text;
            bagisciYakinlari.BagisciId = BagisciIdQS.ConvertToInt();
            int id = bagisciYakinlari.Save();
            bagisciYakinlari.Id = id;
            string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            int queryIndex = newUrl.IndexOf("?");
            if (queryIndex > 0)
                newUrl = newUrl.Substring(0, queryIndex);

            Page.Response.Redirect(newUrl + "?BagisciId=" + BagisciIdQS, true);
        }
    }
}
