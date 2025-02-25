using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.HukukiTakipListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class HukukiTakipListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public HukukiTakipListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    KayitGetir();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void KayitGetir()
        {
            List<Tasinmaz> list = new List<Tasinmaz>();
            var jsonData = HukukiTakipJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateJsString(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        private string CreateJsString(string jsonData)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string rawUrl = System.Web.HttpContext.Current.Request.RawUrl.ToString();
            int index = currentUrl.IndexOf(rawUrl);
            string rootUrl = (index < 0) ? currentUrl : currentUrl.Remove(index, rawUrl.Length);
            string imgUrl = rootUrl + "/" + ProjeConstants.RESIMLER_TASINMAZ + "/_t/";
            string pageUrl = rootUrl + "/" + ProjeConstants.PAGE_TASINMAZ_GIRIS;
            //string onerrorStr = " onerror = imgError(this);";// "onerror=this.src=" + imgUrl + "bagisci_jpg.jpg;";
            //return $('<img src=" + imgUrl + @"' + rowData.Foto + '_jpg.jpg height=50px " + onerrorStr + @"  />')

            string tableString = @"

                $('#tblfilter').puidatatable({
                caption: '',
                editMode: 'cell',
                paginator: {
                            rows: 8
                            },
                columns: [
                    { field: 'DosyaNo', headerText: 'Dos.No',filter: true,sortable:true,headerStyle:'width: 8%' },
                    { field: 'SozlesmeId', headerText: 'Söz.No',filter: true,sortable:true,headerStyle:'width:8%',content: function (rowData)
                        { 
                            return $('<a href=" + ProjeConstants.PAGE_KIRASOZLESMESI + @"?DestinationApp=KS&SenderApp=KSL&KiraSozlesmeId='+rowData.SozlesmeId +'\'>'+rowData.SozlesmeId+'</a>')
                        }
                    },
                    { field: 'KiraciAdiSoyadi', headerText: 'Kiracı',sortable:true,headerStyle:'width: 24%' },
                    { field: 'IlkSozlesmeTar', headerText: 'İlk Söz.Tar.',headerStyle:'width: 10%' },
                    { field: 'BorcAnaPara', headerText: 'Borç (Anapara)',headerStyle:'width: 10%' },                    
                    { field: 'BorcFaiz', headerText: 'Borç (Faiz)',sortable:true,headerStyle:'width: 10%' },
                    { field: 'IslemTarihi', headerText: 'İşlem Tarihi',filter: true,bodyClass:'text-end',headerStyle:'width: 10%' },
                    { field: 'Aciklama', headerText: 'Açıklama',headerStyle:'width: 20%' }
                ],
                datasource:" + jsonData + @",
                resizableColumns: true,
                globalFilter:'#globalFilter'
                });
            ";

            return tableString;
        }
        private string HukukiTakipJson()
        {
            HukukiTakip hukukiTakip = new HukukiTakip();
            string json = hukukiTakip.SelectAllReturnJson();

            return json;

        }
        private DataTable HukukiTakipDataTable()
        {
            HukukiTakip hukukiTakip = new HukukiTakip();
            DataTable dataTable = hukukiTakip.SelectAllReturnDataTable();

            return dataTable;

        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
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

            GridView1.DataSource = HukukiTakipDataTable();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=TasinmazListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
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

    }
}
