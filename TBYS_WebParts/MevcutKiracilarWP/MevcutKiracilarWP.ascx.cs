using Model.Ortak;
using Model.TBYS;
using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.MevcutKiracilarWP
{
    [ToolboxItemAttribute(false)]
    public partial class MevcutKiracilarWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public MevcutKiracilarWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string KiraSozlesmeIdQS
        {
            get
            {

                if (ViewState["KiraSozlesmeId"] == null)
                {
                    if (Page.Request.QueryString["KiraSozlesmeId"] != null)
                    {
                        ViewState["KiraSozlesmeId"] = Page.Request.QueryString["KiraSozlesmeId"];
                    }
                    else
                    {
                        ViewState["KiraSozlesmeId"] = string.Empty;
                    }
                }
                return ViewState["KiraSozlesmeId"].ToString();
            }

            set
            {
                ViewState["KiraSozlesmeId"] = value;
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
                        ViewState["SecilenAy"] = string.Empty;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            AdiLbl.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
            try
            {
                if (BolgeIdQS==ProjeConstants.BOLGE_HEPSI_INT)
                {
                    Bolge bolge = IKYSOrtak.BolgeGetirByUserName(CurrentUserName);
                    BolgeIdQS = bolge == null ? 0 : bolge.Id;
                }
                SetAyYilValues();
                MevcutKiraclariTabloyaDoldur();
            }
            catch (Exception ex)
            {

                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.PublishException();

            }
        }
        private void MevcutKiraclariTabloyaDoldur()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            DateTime secilenTarih = new DateTime(SecilenYilQS.ConvertToInt(), SecilenAyQS.ConvertToInt(), 1);
            DateTime vadeBastar = secilenTarih.AddMonths(-1).AddDays(1);
            DateTime vadeBittar = secilenTarih.AddDays(-1);

            int ay = SecilenAyQS.ConvertToInt();
            int yil = SecilenYilQS.ConvertToInt();
            KiraSozlesme kiraSozlesmeDao = new KiraSozlesme();
            DataTable dataTable = kiraSozlesmeDao.SelectKiraciSayisiByBolgeTarih(BolgeIdQS, ay, yil);
            TableTitleCell.Text = (new DateTime(SecilenYilQS.ConvertToInt(), SecilenAyQS.ConvertToInt(), 1)).ToString("MMMM yyyy", culturInfo) + " İtibarı ile Kiracı Listesi";
            if (dataTable != null)
            {
                int SiraNo = 1;

                foreach (DataRow row in dataTable.Rows)
                {
                    int kiraSozlesmeId = row == null ? 0 : row["KiraSozlesmeId"].ReturnEmptyIfNull().ConvertToInt();
                    int taksitSayisi= row == null ? 0 : row["TaksitSayisi"].ReturnZeroIfNull().ConvertToInt();
                    int aySayisi = row == null ? 0 : (int)(Math.Round(row["AySayisi"].ReturnZeroIfNull().ConvertToDecimal()));
                    string dosyaNo = row == null ? "0" : row["DosyaNo"].ReturnEmptyIfNull().ToString();
                    string bolge = row == null ? "" : row["Bolge"].ReturnEmptyIfNull().ToString();
                    string kiraci = row == null ? "" : row["Kiraci"].ReturnEmptyIfNull().ToString();
                    string odemeSekli = row == null ? "" : row["OdemeSekli"].ReturnEmptyIfNull().ToString();
                    string kiralamaAmaci = row == null ? "" : row["KiralamaAmaci"].ReturnEmptyIfNull().ToString();
                    string ilkSozlesmeTar = row == null ? "" : row["IlkSozlesmeTar"].ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                    decimal kiraBedeliDec = row == null ? 1 : row["KiraBedeli"].ConvertToDecimal() == 0 ? 1 : row["KiraBedeli"].ConvertToDecimal();
                    string kiraBedeli = kiraBedeliDec.ToString("N", culturInfo);
                    string anaPara = row == null ? "" : row["AnaPara"].ConvertToDecimal().ToString("N", culturInfo);
                    decimal faizliBakiyeDec = row == null ? 0 : row["FaizliBakiye"].ConvertToDecimal();
                    string faizliBakiye = row == null ? "" : row["FaizliBakiye"].ConvertToDecimal().ToString("N", culturInfo);

                    int borcluAyAdedi = taksitSayisi>1?(int)Math.Round((faizliBakiyeDec + kiraBedeliDec) / kiraBedeliDec) * (-1):aySayisi;

                    TableRow tableRow = new TableRow();
                    TableCell SiraNoCell = new TableCell();
                    SiraNoCell.Text = SiraNo++ + "";

                    TableCell dosyaNoCell = new TableCell();
                    dosyaNoCell.Text = dosyaNo;

                    TableCell bolgeCell = new TableCell();
                    bolgeCell.Text = bolge;

                    TableCell kiraciCell = new TableCell();
                    HyperLink kiraciLnk = new HyperLink();
                    kiraciLnk.Text = kiraci;
                    string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_ODEMEPLANI + "?KiraSozlesmeId=" + kiraSozlesmeId;
                    kiraciLnk.NavigateUrl = newUrl;
                    kiraciCell.Controls.Add(kiraciLnk);

                    TableCell kiralamaAmaciCell = new TableCell();
                    kiralamaAmaciCell.Text = kiralamaAmaci;
                    
                    TableCell odemeSekliCell = new TableCell();
                    odemeSekliCell.Text = odemeSekli;

                    TableCell metrekareCell = new TableCell();
                    metrekareCell.Text = MetrekareToplami(kiraSozlesmeId).ToString("N", culturInfo); ;

                    TableCell ilkSozTarCell = new TableCell();
                    ilkSozTarCell.Text = ilkSozlesmeTar;

                    TableCell kiraBedeliCell = new TableCell();
                    kiraBedeliCell.Text = kiraBedeli;
                    kiraBedeliCell.CssClass = "text-end";

                    TableCell anaParaCell = new TableCell();
                    anaParaCell.Text = anaPara;
                    anaParaCell.CssClass = "text-end";

                    TableCell faizliBakiyeCell = new TableCell();
                    faizliBakiyeCell.Text = faizliBakiye;
                    faizliBakiyeCell.CssClass = "text-end";

                    TableCell kiraBorcuCell = new TableCell();
                    kiraBorcuCell.Text = borcluAyAdedi < 1 ? "-" : borcluAyAdedi.ToString();
                    kiraBorcuCell.CssClass = "text-end";

                    tableRow.Controls.Add(SiraNoCell);
                    tableRow.Controls.Add(dosyaNoCell);
                    tableRow.Controls.Add(bolgeCell);
                    tableRow.Controls.Add(kiraciCell);
                    tableRow.Controls.Add(ilkSozTarCell);
                    tableRow.Controls.Add(kiralamaAmaciCell);
                    tableRow.Controls.Add(odemeSekliCell);
                    tableRow.Controls.Add(metrekareCell);
                    tableRow.Controls.Add(kiraBedeliCell);
                    tableRow.Controls.Add(anaParaCell);
                    tableRow.Controls.Add(faizliBakiyeCell);
                    tableRow.Controls.Add(kiraBorcuCell);

                    BorcluKiracilarTable.Controls.Add(tableRow);
                }
            }
            else
            {
                MessageHelper.PublishMessage("Borçlu Kiracı bulunamadı", ProjeConstants.MESAJ_BILGI, 2000);
            }

        }

        private decimal MetrekareToplami(int kiraSozlesmeId)
        {
            SozlesmeTasinmaz sozlesmeTasinmaz = new SozlesmeTasinmaz();
            decimal metrekare = sozlesmeTasinmaz.SelectSumMetrekareBySozlesmeId(kiraSozlesmeId);
            return metrekare;
        }

        private void SetAyYilValues()
        {
            try
            {
                //ay
                string ay = !string.IsNullOrEmpty(SecilenAyQS) ? SecilenAyQS : DateTime.Today.Month.ReturnEmptyIfNull().ToString();

                SecilenAyQS = ay;

                //yil
                string yil = !string.IsNullOrEmpty(SecilenYilQS) ? SecilenYilQS : DateTime.Today.Year.ReturnEmptyIfNull().ToString();
                SecilenYilQS = yil;
            }
            catch (Exception)
            {

                //TODO
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            string filename = "MevcutKiracilar" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            BorcluKiracilarTable.RenderControl(hw);
            //Write the HTML back to the browser.
            //Response.ContentType = application/vnd.ms-excel;
            Page.Response.ContentType = "application/vnd.ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            Page.Response.Write(tw.ToString());
            Page.Response.End();
        }
        protected void OdemePlanlariniGuncelleBtn_Click(object sender, EventArgs e)
        {
            TBYSOrtak.AktifSozleslemelerinBakiyeBorcunuHesapla();
        }

    }
}