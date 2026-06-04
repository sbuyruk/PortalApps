using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.MulkiyetiOlmayanTasinmazListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class MulkiyetiOlmayanTasinmazListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public MulkiyetiOlmayanTasinmazListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    Bolge bolge = IKYSOrtak.BolgeGetirByUserName(CurrentUserName);
                    BolgeIdQS = bolge == null ? 0 : bolge.Id;
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }

        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<MulkiyetiOlmayanTasinmazListesiListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
                jSon = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        private List<MulkiyetiOlmayanTasinmazListesiListItem> GetDataList()
        {

            DataTable dataTable = GetData();

            List<MulkiyetiOlmayanTasinmazListesiListItem> list = new List<MulkiyetiOlmayanTasinmazListesiListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            foreach (DataRow row in dataTable.Rows)
            {

                string kullanimSekli = row["KullanimSekli"].ToString();
                string adres = row["Adres"].ToString();
                string iliIlcesi = row["IliIlcesi"].ToString();
                string sorumluBolge = row["SorumluBolge"].ToString();

                string tasinmazId = row["TasinmazId"].ToString();

                MulkiyetiOlmayanTasinmazListesiListItem mulkiyetiOlmayanTasinmazListesiListItem = new MulkiyetiOlmayanTasinmazListesiListItem();
                mulkiyetiOlmayanTasinmazListesiListItem.KullanimSekli = kullanimSekli;
                mulkiyetiOlmayanTasinmazListesiListItem.Adres = adres;
                mulkiyetiOlmayanTasinmazListesiListItem.IliIlcesi = iliIlcesi;
                mulkiyetiOlmayanTasinmazListesiListItem.SorumluBolge = sorumluBolge;
                bool duzenleGorunsunMu = BolgeIdQS == ProjeConstants.HEPSI_INT || BolgeIdQS == ProjeConstants.BOLGE_GENELMUDURLUK_INT;
                if (duzenleGorunsunMu)
                {
                    mulkiyetiOlmayanTasinmazListesiListItem.Duzenle = "<a href=" + ProjeConstants.PAGE_MULKIYETIOLMAYANTASINMAZ_GIRIS + "?EnvanterdeMi=2&DestinationApp=TD&SenderApp=TL&TasinmazId=" + tasinmazId + " class='btn btn-outline-primary'>Düzenle</a>";
                }
                 list.Add(mulkiyetiOlmayanTasinmazListesiListItem);
            }
            return list;
        }

        private DataTable GetData()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            DataTable dataTable = tasinmaz.SelectEnvanterdeOlmayanTasinmazReturnDataTable();
            return dataTable;
        }
        private string CreateDataTable(string jsonData)
        {
            string duzenleGorunsun = BolgeIdQS == ProjeConstants.BOLGE_HEPSI_INT || BolgeIdQS == ProjeConstants.BOLGE_GENELMUDURLUK_INT
                ? "{ targets:4, visible:true},"
                : "{ targets:4, visible:false},";
            string tableString = @"
            jQuery(document).ready(function () {

                jQuery('#CustomDataTable').DataTable({
                    data: " + jsonData + @",
                    columns: [
                        { data: 'KullanimSekli' },
                        { data: 'Adres' },
                        { data: 'IliIlcesi' },
                        { data: 'SorumluBolge' },
                        { data: 'Duzenle' },

                    ],
                    'order': [[1, 'asc']],
                     columnDefs:
                        [
                        " + duzenleGorunsun + @"
                        ],
                    'language': {
                                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    dom: 'Bfrtip',
                    buttons: [
                        {
                            extend: 'print',
                            exportOptions: {
                                columns: ':visible'
                            }
                        },
                        {
                            extend: 'excel',
                            exportOptions: {
                                columns: ':visible'
                            }
                        },
                        {
                            extend: 'pdf',
                            exportOptions: {
                                columns: ':visible'
                            }
                        },
                        {
                            extend: 'copy',
                            exportOptions: {
                                columns: ':visible'
                            }
                        },
                        , 'pageLength', 'colvis'
                    ]

                });
            });";
            return tableString;
        }
        private class MulkiyetiOlmayanTasinmazListesiListItem
        {
            public string KullanimSekli { get; set; }
            public string Adres { get; set; }
            public string IliIlcesi { get; set; }
            public string SorumluBolge { get; set; }
            public string Duzenle { get; set; }

        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToExcel();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void ExportToExcel()
        {
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;

            GridView1.DataSource = GetData();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content -disposition",
             "attachment;filename=MulkiyetiOlmayanTasinmazListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
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
