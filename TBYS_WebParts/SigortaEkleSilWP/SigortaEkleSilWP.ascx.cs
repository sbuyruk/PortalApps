using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.SigortaEkleSilWP
{
    [ToolboxItemAttribute(false)]
    public partial class SigortaEkleSilWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public SigortaEkleSilWP()
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
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select(TasinmazIdQS.ConvertToInt());
            if (tasinmaz != null)
            {
                SigortaTablosunuDoldur(tasinmaz);

                if (!Page.IsPostBack)
                {
                    SigortaCinsiDDLDoldur();
                    BagimsizBolumDDLDoldur(tasinmaz);
                }
            }
        }
        private void SigortaCinsiDDLDoldur()
        {
            SigortaCinsiDDL.Items.Clear();
            SigortaCinsiDDL.Items.Add(ProjeConstants.SIGORTA_YOK);
            SigortaCinsiDDL.Items.Add(ProjeConstants.SIGORTA_DASK);
            SigortaCinsiDDL.Items.Add(ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
        }
        private void BagimsizBolumDDLDoldur(Tasinmaz tasinmaz)
        {
            BagimsizBolumDDL.Items.Clear();
            ListItem li0 = new ListItem("", "0");
            BagimsizBolumDDL.Items.Add(li0);
            if (tasinmaz.KatMulkiyeti.Equals(ProjeConstants.KAT_MULKIYETI_YOK))
            {
                BagimsizBolum bb = new BagimsizBolum();
                List<BagimsizBolum> list = bb.SelectByTasinmazId(tasinmaz.Id);
                foreach (BagimsizBolum item in list)
                {
                    ListItem li = new ListItem(item.BolumNo, item.Id.ToString());
                    BagimsizBolumDDL.Items.Add(li);
                }
            }
        }
        protected void SigortaEkleBtn_Click(object sender, EventArgs e)
        {
            Sigorta sigorta = new Sigorta();
            sigorta.TasinmazId = TasinmazIdQS.ConvertToInt();

            sigorta.SigortaCinsi = SigortaCinsiDDL.SelectedValue;
            sigorta.SigortaBasTar = SigortaBasTarTxt.Value.ConvertToDatetime();
            sigorta.SigortaBitTar = SigortaBitTarTxt.Value.ConvertToDatetime();
            sigorta.BulunduguKat = BulunduguKatTxt.Text;
            sigorta.SigortaBedeli = SigortaBedeliTxt.Text.ConvertToDecimal();
            sigorta.Prim = PrimTxt.Text.ConvertToDecimal();
            sigorta.AdresKodu = AdresKoduTxt.Text;
            sigorta.PoliceNo = PoliceNoTxt.Text;
            sigorta.DaskPoliceNo = DaskPoliceNoTxt.Text;
            sigorta.YapiTarzi = YapiTarziTxt.Text;
            sigorta.InsaYili = InsaYiliTxt.Text;
            sigorta.Olusturan = CurrentUserName;
            sigorta.BolumId = BagimsizBolumDDL.SelectedItem.Value.ConvertToInt();
            int sigortaId = sigorta.Save();
            string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            int queryIndex = newUrl.IndexOf("?");
            if (queryIndex > 0)
                newUrl = newUrl.Substring(0, queryIndex);

            Page.Response.Redirect(newUrl + "?TasinmazId=" + sigorta.TasinmazId + "&SenderApp=" + SenderAppQS, true);
        }
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select<Tasinmaz>(TasinmazIdQS.ConvertToInt());
            if (EnvanterdeMiQS.Equals("2"))
                RedirectToPage(ProjeConstants.PAGE_PAGE_MULKIYETIOLMAYANTASINMAZ_LIST);

            else if (SenderAppQS.Equals("TD"))
            {
                //Page.Response.Redirect("/pages/TasinmazGirisi.aspx?visible=TD&tId=" + tasinmazId);
                RedirectToPage(ProjeConstants.PAGE_TASINMAZ_GIRIS + "?DestinationApp=TD&TasinmazId=" + TasinmazIdQS);
            }

            else if (SenderAppQS.Equals("SigortaL"))
            {
                RedirectToPage(ProjeConstants.PAGE_TASINMAZSIGORTA_LIST);
            }

        }
        private void SigortaTablosunuDoldur(Tasinmaz tasinmaz)
        {
            AdiLbl.Text = " Tasinmaz : " + tasinmaz.Adres + " - " + tasinmaz.Ili + " / " + tasinmaz.Ilcesi;
            IdLbl.Text = tasinmaz.Id + "";
            //Column headers
            HeaderCell0.Text = "Sigorta Cinsi";
            HeaderCell0.Visible = true;
            HeaderCell1.Text = "Bas.Tar.";
            HeaderCell1.Visible = true;
            HeaderCell2.Text = "Bit.Tar";
            HeaderCell2.Visible = true;
            HeaderCell3.Text = "Bul.Kat";
            HeaderCell3.Visible = true;
            HeaderCell4.Text = "AdresKodu";
            HeaderCell4.Visible = true;
            HeaderCell5.Text = "Poliçe No";
            HeaderCell5.Visible = true;
            HeaderCell6.Text = "Sig.Bedeli";
            HeaderCell6.Visible = true;
            HeaderCell7.Text = "Prim";
            HeaderCell7.Visible = true;
            HeaderCell8.Text = "Düzenle";
            HeaderCell8.Visible = true;
            HeaderCell9.Text = "Sil";
            HeaderCell9.Visible = true;

            List<Sigorta> list = new Sigorta().selectByTasinmazId(tasinmaz.Id);
            foreach (Sigorta sigorta in list)
            {
                TableRow row = new TableRow();

                TableCell SigortaCinsiCell = new TableCell();
                SigortaCinsiCell.Text = sigorta.SigortaCinsi;
                row.Controls.Add(SigortaCinsiCell);

                TableCell SigortaBasTarCell = new TableCell();
                SigortaBasTarCell.Text = sigorta.SigortaBasTar.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(SigortaBasTarCell);

                TableCell SigortaBitTarCell = new TableCell();
                SigortaBitTarCell.Text = sigorta.SigortaBitTar.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(SigortaBitTarCell);

                TableCell BulunduguKatCell = new TableCell();
                BulunduguKatCell.Text = string.IsNullOrEmpty(tasinmaz.BulunduguKat) ? sigorta.BulunduguKat : tasinmaz.BulunduguKat;
                row.Controls.Add(BulunduguKatCell);

                TableCell AdresKoduCell = new TableCell();
                AdresKoduCell.Text = sigorta.AdresKodu.ToString();
                row.Controls.Add(AdresKoduCell);

                TableCell PoliceNoCell = new TableCell();
                PoliceNoCell.Text = sigorta.PoliceNo.ToString();
                row.Controls.Add(PoliceNoCell);

                TableCell SigortaBedeliCell = new TableCell();
                SigortaBedeliCell.Attributes.Add("class", "text-end");
                SigortaBedeliCell.Text = sigorta.SigortaBedeli.ToString("N", culturInfo);
                row.Controls.Add(SigortaBedeliCell);

                TableCell PrimCell = new TableCell();
                PrimCell.Attributes.Add("class", "text-end");
                PrimCell.Text = sigorta.Prim.ToString("N", culturInfo);
                row.Controls.Add(PrimCell);

                TableCell DuzenleCell = new TableCell();

                LinkButton DuzenleBtn = new LinkButton();
                DuzenleBtn.Text = "Düzenle";
                DuzenleBtn.CssClass = "btn btn-outline-primary";
                DuzenleBtn.Click += delegate
                {
                    RedirectToPage(ProjeConstants.PAGE_TASINMAZSIGORTA_GIRIS + "?SenderApp=SigortaES&DestinationApp=SigortaD&TasinmazId=" + tasinmaz.Id + "&SigortaId=" + sigorta.Id + "&EnvanterdeMi=" + EnvanterdeMiQS);
                };
                DuzenleCell.Controls.Add(DuzenleBtn);
                row.Controls.Add(DuzenleCell);

                TableCell SilCell = new TableCell();
                LinkButton SilBtn = new LinkButton();
                SilBtn.Text = "Sil";
                SilBtn.CssClass = "btn btn-outline-danger";
                SilBtn.Click += delegate
                {
                    int tasinmazId = sigorta.TasinmazId;
                    sigorta.Delete();
                    string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    int queryIndex = newUrl.IndexOf("?");
                    if (queryIndex > 0)
                        newUrl = newUrl.Substring(0, queryIndex);
                    Page.Response.Redirect(newUrl + "?TasinmazId=" + tasinmazId + "&SenderApp=" + SenderAppQS, true);
                };
                SilCell.Controls.Add(SilBtn);
                row.Controls.Add(SilCell);

                SigortaTable.Controls.Add(row);
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

        protected void TasinmazBtn_Click(object sender, EventArgs e)
        {

            if (EnvanterdeMiQS.Equals("2"))
                RedirectToPage(ProjeConstants.PAGE_MULKIYETIOLMAYANTASINMAZ_GIRIS + "?TasinmazId=" + TasinmazIdQS + "&DestinationApp=TD");
            else RedirectToPage(ProjeConstants.PAGE_TASINMAZ_GIRIS + "?DestinationApp=TD&SenderApp=TD&TasinmazId=" + TasinmazIdQS);

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
    }
}
