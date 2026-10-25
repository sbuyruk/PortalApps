using Model.IKYS;
using Model.Services.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.EskiPersonelListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class EskiPersonelListesiWP : WebPart
    {
        private PortalLogContext LoggingContext { get { return PortalLogContext.FromCurrentRequest("IKYS", "EskiPersonelListesiWP"); } }
        private void ReportError(string operation, Exception error)
        {
            PortalLogContext context = LoggingContext;
            if (error is System.Threading.ThreadAbortException) return;
            PortalTechnicalLog.WriteError(context, operation, error);
            Messages.Error("İşlem tamamlanamadı. Lütfen tekrar deneyin. Başvuru kodu: " + context.OperationId.ToString("N"));
        }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public EskiPersonelListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SecilenIdQS
        {
            get
            {

                if (ViewState["SecilenId"] == null)
                {
                    if (Page.Request.QueryString["SecilenId"] != null)
                    {
                        ViewState["SecilenId"] = Page.Request.QueryString["SecilenId"];
                    }
                    else
                    {
                        ViewState["SecilenId"] = string.Empty;
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    new PersonelService().TryRecordWebPartOpened(LoggingContext);
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ReportError("EskiPersonelListesi.PageLoad", ex);
            }
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            UtilityHelper.ScriptCalistir("setDataSet(" + jsonData + ");");
        }

        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<PersonelListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                jSon = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ReportError("EskiPersonelListesi.Data", exception);
            }
            return jSon;
        }
        private List<PersonelListItem> GetDataList()
        {
            List<PersonelListItem> list = new List<PersonelListItem>();
            DataTable table = GetDataTable();
            if (table == null) return list;
            foreach (DataRow row in table.Rows)
            {
                PersonelListItem item = new PersonelListItem();
                item.PersonelId = row["PersonelId"].ToString();
                item.ProtokolSiraNo = row["ProtokolSiraNo"].ToString();
                item.Adi = row["Adi"].ToString();
                item.Soyadi = row["Soyadi"].ToString();
                item.Unvan = row["Unvan"].ToString();
                item.BirimSube = row["BirimSube"].ToString();
                item.Secildi = SecilenIdQS.Equals(item.PersonelId);
                item.Duzenle = "<a href=" + ProjeConstants.PAGE_ESKIPERSONEL_EDIT + "?DestinationApp=PerD&PersonelId=" + item.PersonelId + " class='btn btn-outline-primary'>Düzenle</a>";
                list.Add(item);
            }
            return list;
        }
        private class PersonelListItem
        {
            public string PersonelId { get; set; }
            public string ProtokolSiraNo { get; set; }
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public string Unvan { get; set; }
            public string BirimSube { get; set; }
            public string PersonelKarti { get; set; }
            public string KisiselSayfa { get; set; }
            public string Duzenle { get; set; }
            public bool Secildi { get; set; }
        }
        private DataTable GetDataTable()
        {
            Personel personel = new Personel();
            PersonelService service = new PersonelService();
            DataTable dataTable = service.GetFormerEmployeeListReturnDataTable(LoggingContext);
            return dataTable;
        }

        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToExcel();
            }
            catch (Exception ex)
            {
                ReportError("EskiPersonelListesi.Excel", ex);
            }
        }
        protected void ExportToExcel()
        {

            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;

            GridView1.DataSource = GetDataTable();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=PersonelListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            Page.Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                //Apply text style to each Row
                GridView1.Rows[i].Attributes.Add("class", "textmode");
            }
            GridView1.RenderControl(hw);

            //style to format numbers to string
            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Page.Response.Write(style);
            Page.Response.Output.Write(sw.ToString());
            Page.Response.Flush();
            Page.Response.End();
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl, false);
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                ReportError("EskiPersonelListesi.Redirect", ex);
            }
        }
    }
}

