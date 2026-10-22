using Model.NBYS;
using Model.Services.NBYS;
using NBYS_WebParts.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.SMSAylikCizelgeReadOnlyWP
{
    [ToolboxItemAttribute(false)]
    public partial class SMSAylikCizelgeReadOnlyWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "SMSAylikCizelgeReadOnlyWP"); }
        }

        private void ExecuteAction(string operation, Action action)
        {
            NbysReportFeedback.Execute(LoggingContext, operation, action);
        }

        private List<SMSAylikBagis> yearRows;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            ChromeType = PartChromeType.None;
        }

        private int SelectedYear
        {
            get
            {
                string value = Convert.ToString(ViewState["SmsYear"] ?? Page.Request.QueryString["SecilenYil"]
                    ?? DateTime.Today.Year.ToString(CultureInfo.InvariantCulture));
                int year;
                if (!int.TryParse(value, out year) || !NbysSmsWriteService.ValidYear(year)
                    || YilDDL.Items.FindByValue(year.ToString(CultureInfo.InvariantCulture)) == null)
                    throw new NbysReportInputException("Listeden geçerli bir yıl seçin.");
                return year;
            }
        }

        private List<SMSAylikBagis> GetYearRows()
        {
            if (yearRows == null)
            {
                yearRows = new SMSAylikBagisService(LoggingContext).GetByYear(SelectedYear);
                if (!NbysSmsWriteService.ValidRows(yearRows))
                    throw new NbysReportInputException("Çizelgede aynı aya ait birden fazla kayıt veya geçersiz ay var. Kayıtları kontrol ettirin.");
            }
            return yearRows;
        }

        private void RenderYear()
        {
            NbysSmsTable.Fill(AyrintiTable, GetYearRows(), SelectedYear, false);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack)
                {
                    NbysWebPartLog.RecordOpened(LoggingContext);
                    YilDDL.Items.Clear();
                    for (int year = Math.Max(2019, DateTime.Today.Year - 1); year <= DateTime.Today.Year; year++)
                        YilDDL.Items.Add(new ListItem(year.ToString(CultureInfo.InvariantCulture), year.ToString(CultureInfo.InvariantCulture)));
                    YilDDL.SelectedValue = SelectedYear.ToString(CultureInfo.InvariantCulture);
                    ViewState["SmsYear"] = SelectedYear;
                }
                ScriptManager manager = ScriptManager.GetCurrent(Page);
                if (manager != null) manager.RegisterPostBackControl(ExcelBtn);
                RenderYear();
                if (!Page.IsPostBack)
                {
                    int missing = 12 - GetYearRows().Count;
                    if (missing > 0) Messages.Warning(missing + " ayın kaydı bulunmuyor. Kayıtların tamamlanması için yetkili birime başvurun.");
                }
            });
        }

        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("YilDDL_SelectedIndexChanged", () =>
            {
                int year;
                if (!int.TryParse(YilDDL.SelectedValue, out year) || !NbysSmsWriteService.ValidYear(year))
                    throw new NbysReportInputException("Listeden geçerli bir yıl seçin.");
                var query = HttpUtility.ParseQueryString(Page.Request.Url.Query);
                query["SecilenYil"] = year.ToString(CultureInfo.InvariantCulture);
                UriBuilder url = new UriBuilder(Page.Request.Url) { Query = query.ToString() };
                Page.Response.Redirect(url.Uri.AbsoluteUri, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            });
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () =>
            {
                string url = Page.Request.Url.GetLeftPart(UriPartial.Path);
                Page.Response.Redirect(url.Substring(0, url.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            });
        }

        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ExcelBtn_Click", ExportToExcel);
        }

        protected void ExportToExcel()
        {
            string content;
            try
            {
                ExcelTableDiv.Attributes["style"] = "display: block";
                NbysSmsTable.Fill(ExcelTable, GetYearRows(), SelectedYear, false);
                using (StringWriter writer = new StringWriter())
                using (HtmlTextWriter html = new HtmlTextWriter(writer))
                {
                    ExcelTableDiv.RenderControl(html);
                    content = "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1254\" /></head><body>"
                        + writer.ToString() + "</body></html>";
                }
            }
            finally { ExcelTableDiv.Attributes["style"] = "display: none"; }
            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";
            Page.Response.ContentType = "application/vnd.ms-excel";
            Page.Response.AddHeader("Content-Disposition", "attachment;filename=SMSAylikCizelge-" + SelectedYear + ".xls");
            Page.Response.Write(content);
            Page.Response.End();
        }
    }
}
