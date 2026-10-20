using Model.Ortak;
using Model.Services.TBYS;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.Logging;
using TBYS_WebParts.Logging;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.GecikmeZammiWP
{
    [ToolboxItemAttribute(false)]
    public partial class GecikmeZammiWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get
            {
                return PortalLogContext.FromCurrentRequest("TBYS", "GecikmeZammiWP");
            }
        }

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public GecikmeZammiWP()
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
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.GecikmeZammiWP.Page_Load", error);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
            GecikmeZammiiTablosunuDoldur();

        }


        private void GecikmeZammiiTablosunuDoldur()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            GecikmeZammiService faizOranService = new GecikmeZammiService(LoggingContext);
            List<GecikmeZammi> list = faizOranService.GetAll();
            int sira = 1;
            foreach (GecikmeZammi item in list)
            {
                TableRow row = new TableRow();

                TableCell SiraCell = new TableCell();
                SiraCell.Text = sira.ToString();

                TableCell BaslangicTarihiCell = new TableCell();

                HtmlInputText BaslangicTarihiTxt = new HtmlInputText("text");
                BaslangicTarihiTxt.ID = "BaslangicTarihiTxt" + sira++;
                BaslangicTarihiTxt.Attributes["class"] = "form-control DateTimePickerV1";
                BaslangicTarihiTxt.Value = item.BaslangicTarihi.ConvertToDatetimeEmptyIfNull();
                BaslangicTarihiTxt.Attributes["readonly"] = "readonly";
                BaslangicTarihiCell.Controls.Add(BaslangicTarihiTxt);

                TableCell BitisTarihiCell = new TableCell();
                BitisTarihiCell.Text = item.BitisTarihi.ConvertToDatetimeEmptyIfNull();

                TableCell ZamOraniCell = new TableCell();
                TextBox ZamOraniTxt = new TextBox();
                ZamOraniTxt.Text = item.ZamOrani.ToString();
                ZamOraniTxt.CssClass = "form-control input-money text-end";
                ZamOraniCell.Controls.Add(ZamOraniTxt);

                TableCell AciklamaCell = new TableCell();
                AciklamaCell.Text = item.Aciklama.ToString();
                TextBox AciklamaTxt = new TextBox();
                AciklamaTxt.Text = item.Aciklama.ToString();
                AciklamaTxt.CssClass = "form-control";
                AciklamaCell.Controls.Add(AciklamaTxt);


                TableCell GuncelleCell = new TableCell();
                LinkButton GuncelleBtn = new LinkButton();
                GuncelleBtn.Text = "Güncelle";
                GuncelleBtn.CssClass = "btn btn-outline-primary";
                GuncelleBtn.ID = "GuncelleBtn" + sira;
                GuncelleBtn.Click += delegate
                {
                    try
                    {
                        item.BaslangicTarihi = BaslangicTarihiTxt.Value.ConvertToDatetime();
                        item.ZamOrani = ZamOraniTxt.Text.ConvertToDecimal();
                        item.Aciklama = AciklamaTxt.Text;
                        GecikmeZammiService gecikmeZammiService = new GecikmeZammiService(LoggingContext);
                        if (gecikmeZammiService.UpdatePeriod(item))
                        {
                            MessageHelper.PublishMessage("Kayıt Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                            newUrl += "/" + ProjeConstants.PAGE_GECIKMEZAMMI;
                            Page.Response.Redirect(newUrl, true);
                        }

                    }
                    catch (Exception tbysError)
                    {
                        if (tbysError is System.Threading.ThreadAbortException) throw;
                        TbysWebPartLog.WriteError(LoggingContext, "TBYS.GecikmeZammiWP.GecikmeZammiiTablosunuDoldur", tbysError);

                        TbysWebPartLog.ReportError(LoggingContext, "TBYS.GecikmeZammiWP.GecikmeZammiiTablosunuDoldur", tbysError);
                    }
                };
                GuncelleCell.Controls.Add(GuncelleBtn);

                TableCell SilCell = new TableCell();
                LinkButton SilBtn = new LinkButton();
                SilBtn.Text = "Sil";
                SilBtn.CssClass = "btn btn-outline-danger";
                SilBtn.ID = "SilBtn" + sira;
                SilBtn.Click += delegate
                {
                    try
                    {
                        GecikmeZammiService gecikmeZammiService = new GecikmeZammiService(LoggingContext);
                        if (gecikmeZammiService.DeletePeriod(item.Id))
                        {
                            MessageHelper.PublishMessage("Kayıt Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                            newUrl += "/" + ProjeConstants.PAGE_GECIKMEZAMMI;
                            Page.Response.Redirect(newUrl, true);
                        }
                    }
                    catch (Exception tbysError)
                    {
                        if (tbysError is System.Threading.ThreadAbortException) throw;
                        TbysWebPartLog.WriteError(LoggingContext, "TBYS.GecikmeZammiWP.GecikmeZammiiTablosunuDoldur", tbysError);

                        TbysWebPartLog.ReportError(LoggingContext, "TBYS.GecikmeZammiWP.GecikmeZammiiTablosunuDoldur", tbysError);
                    }
                };
                SilCell.Controls.Add(SilBtn);

                row.Controls.Add(SiraCell);
                row.Controls.Add(BaslangicTarihiCell);
                row.Controls.Add(BitisTarihiCell);
                row.Controls.Add(ZamOraniCell);
                row.Controls.Add(AciklamaCell);
                row.Controls.Add(GuncelleCell);
                row.Controls.Add(SilCell);
                AyrintiTable.Controls.Add(row);
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
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.GecikmeZammiWP.RedirectToPage", ex);
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.GecikmeZammiWP.RedirectToPage", ex);
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_HOME);
        }
        protected void OdemePlaniListBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_ODEMEPLANI_LIST);
        }

        protected void EkleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GecikmeZammi yeni = new GecikmeZammi
                {
                    BaslangicTarihi = YeniBaslangicTarihiTxt.Value.ConvertToDatetime(),
                    ZamOrani = YeniZamOraniTxt.Value.ConvertToDecimal(),
                    Aciklama = YeniAciklamaTxt.Value
                };
                new GecikmeZammiService(LoggingContext).SavePeriod(yeni);
                MessageHelper.PublishMessage("Kayıt eklendi.", ProjeConstants.MESAJ_BASARILI, 2000);
                RedirectToPage(ProjeConstants.PAGE_GECIKMEZAMMI);
            }
            catch (Exception error)
            {
                if (error is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.GecikmeZammiWP.EkleBtn_Click", error);
            }
        }
    }
}
