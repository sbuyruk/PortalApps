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

namespace TBYS_WebParts.TasinmazOnarimGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazOnarimGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazOnarimGirisiWP()
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

            if (!string.IsNullOrEmpty(TasinmazIdQS))
            {
                Tasinmaz tasinmaz = new Tasinmaz();
                tasinmaz = tasinmaz.Select<Tasinmaz>(TasinmazIdQS.ConvertToInt());
                if (tasinmaz != null)
                {
                    OnarimFormunuDoldur(tasinmaz);
                }
            }
            if (!Page.IsPostBack)
            {
                HarcamaUsuluDoldur();
            }
        }
        private void HarcamaUsuluDoldur()
        {
            HarcamaUsuluDDL.Items.Clear();
            HarcamaUsuluDDL.Items.Add(ProjeConstants.HARCAMAUSULU_PIYASADANALIM);
            HarcamaUsuluDDL.Items.Add(ProjeConstants.HARCAMAUSULU_YONETIMEKATILMA);

        }
        protected void OnarimEkleBtn_Click(object sender, EventArgs e)
        {
            Onarim onarim = new Onarim();
            onarim.YapilanIs = YapilanIsTxt.Text;
            onarim.HarcamaUsulu = HarcamaUsuluDDL.SelectedValue;
            onarim.OnayTarihi = OnayTarihiTxt.Value.ConvertToDatetime();
            onarim.Tutar = TutarTxt.Text.ConvertToDecimal();
            onarim.Aciklama = AciklamaTxt.Text;
            onarim.TasinmazId = TasinmazIdQS.ConvertToInt();
            onarim.Olusturan = CurrentUserName;
            int onarimId = onarim.Save();
            string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            int queryIndex = newUrl.IndexOf("?");
            if (queryIndex > 0)
                newUrl = newUrl.Substring(0, queryIndex);

            Page.Response.Redirect(newUrl + "?TasinmazId=" + onarim.TasinmazId + "&SenderApp=" + SenderAppQS, true);
        }
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select<Tasinmaz>(TasinmazIdQS.ConvertToInt());
            if (SenderAppQS.Equals("TD"))
            {
                //Page.Response.Redirect("/pages/TasinmazGirisi.aspx?visible=TD&tId=" + tasinmazId);
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                newUrl += "/" + ProjeConstants.PAGE_TASINMAZ_GIRIS + "?DestinationApp=TD&TasinmazId=" + TasinmazIdQS;
                Page.Response.Redirect(newUrl, true);
            }

            else if (SenderAppQS.Equals("OL"))
            {
                //Page.Response.Redirect("/pages/OnarimListesi.aspx?tId=" + tasinmazId);
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                newUrl += "/" + ProjeConstants.PAGE_TASINMAZONARIM_LIST;
                Page.Response.Redirect(newUrl, true);
            }

        }
        private void OnarimFormunuDoldur(Tasinmaz tasinmaz)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            AdiLbl.Text = "</br> Tasinmaz : " + tasinmaz.Adres + System.Environment.NewLine +
                " - " + tasinmaz.Ili + " / " + tasinmaz.Ilcesi + System.Environment.NewLine +
                tasinmaz.Cinsi;
            IdLbl.Text = "(Onarim No:" + tasinmaz.Id + ")";
            //Column headers
            HeaderCell1.Text = "Yapilan Is";
            HeaderCell1.Visible = true;
            HeaderCell2.Text = "Harcama Usulü";
            HeaderCell2.Visible = true;
            HeaderCell3.Text = "Onay Tarihi";
            HeaderCell3.Visible = true;
            HeaderCell4.Text = "Tutar";
            HeaderCell4.Visible = true;
            HeaderCell5.Text = "Açiklama";
            HeaderCell5.Visible = true;

            List<Onarim> list = new Onarim().SelectOnarimByTasinmazId(tasinmaz.Id);
            int SiraNo = 1;
            foreach (Onarim onarim in list)
            {
                TableRow row = new TableRow();

                TableCell SiraNoCell = new TableCell();

                SiraNoCell.Text = SiraNo++ + "";
                row.Controls.Add(SiraNoCell);

                TableCell YapilanIsCell = new TableCell();
                YapilanIsCell.Text = onarim.YapilanIs;
                row.Controls.Add(YapilanIsCell);

                TableCell HarcamaUsuluCell = new TableCell();
                HarcamaUsuluCell.Text = onarim.HarcamaUsulu;
                row.Controls.Add(HarcamaUsuluCell);

                TableCell OnayTarihiCell = new TableCell();
                OnayTarihiCell.Text = onarim.OnayTarihi.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(OnayTarihiCell);

                TableCell TutarCell = new TableCell();
                //TutarCell.Attributes = "text-end";
                TutarCell.Text = onarim.Tutar.ToString("N", culturInfo);
                row.Controls.Add(TutarCell);

                TableCell AciklamaCell = new TableCell();
                AciklamaCell.Text = onarim.Aciklama;
                row.Controls.Add(AciklamaCell);

                TableCell SilCell = new TableCell();
                LinkButton SilBtn = new LinkButton();
                SilBtn.Text = "Sil";
                SilBtn.CssClass = "btn btn-outline-danger";
                SilBtn.Click += delegate
                {
                    int tasinmazId = onarim.TasinmazId;
                    onarim.Delete();
                    string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    int queryIndex = newUrl.IndexOf("?");
                    if (queryIndex > 0)
                        newUrl = newUrl.Substring(0, queryIndex);
                    Page.Response.Redirect(newUrl + "?TasinmazId=" + tasinmazId + "&SenderApp=" + SenderAppQS, true);
                };
                SilCell.Controls.Add(SilBtn);
                row.Controls.Add(SilCell);

                OnarimTable.Controls.Add(row);
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
    }
}
