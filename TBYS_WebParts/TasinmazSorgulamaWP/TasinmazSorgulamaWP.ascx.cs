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


namespace TBYS_WebParts.TasinmazSorgulamaWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazSorgulamaWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazSorgulamaWP()
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
                TabloOlustur();
            }
            catch (Exception ex)
            {
                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
            }
        }
        private void TabloOlustur()
        {
            List<Tasinmaz> list = new List<Tasinmaz>();
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            //var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "setDataSet(" + jsonData + ");", true);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<TasinmazSorgulamaListItem> list = GetDataList();
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
        private List<TasinmazSorgulamaListItem> GetDataList()
        {

            Tasinmaz tasinmaz = new Tasinmaz();
            DataTable dataTable = tasinmaz.SelectAllReturnDataTable();



            List<TasinmazSorgulamaListItem> list = new List<TasinmazSorgulamaListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            foreach (DataRow row in dataTable.Rows)
            {
                string kullanimSekli = row["KullanimSekli"].ToString();
                string mulkiyetSekli = row["MulkiyetSekli"].ToString();
                string kiraDurumu = row["KiraDurumu"].ToString();
                string ili = row["Ili"].ToString();
                string ilcesi = row["Ilcesi"].ToString();
                string adres = row["Adres"].ToString();
                string bagisci = row["Bagisci"].ToString();
                string bagisYili = row["BagisYili"].ToString();


                TasinmazSorgulamaListItem tasinmazSorgulamaListItem = new TasinmazSorgulamaListItem();
                tasinmazSorgulamaListItem.KullanimSekli = kullanimSekli;
                tasinmazSorgulamaListItem.MulkiyetSekli = mulkiyetSekli;
                tasinmazSorgulamaListItem.KiraDurumu = kiraDurumu;
                tasinmazSorgulamaListItem.Ili = ili;
                tasinmazSorgulamaListItem.Ilcesi = ilcesi;
                tasinmazSorgulamaListItem.Adres = adres;
                tasinmazSorgulamaListItem.Bagisci = bagisci;
                tasinmazSorgulamaListItem.BagisYili = bagisYili;


                list.Add(tasinmazSorgulamaListItem);
            }
            return list;
        }
        private class TasinmazSorgulamaListItem
        {
            public string KullanimSekli { get; set; }
            public string MulkiyetSekli { get; set; }
            public string KiraDurumu { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Adres { get; set; }
            public string Bagisci { get; set; }
            public string BagisYili { get; set; }

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
            //Get the data from database into datatable
            Tasinmaz tasinmaz = new Tasinmaz();
            DataTable dt = tasinmaz.SelectAllReturnDataTable();


            //Create a dummy GridView
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;
            GridView1.DataSource = dt;
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
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
    }
}
