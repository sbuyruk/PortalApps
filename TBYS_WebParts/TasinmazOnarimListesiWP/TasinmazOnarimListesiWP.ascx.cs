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

namespace TBYS_WebParts.TasinmazOnarimListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazOnarimListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazOnarimListesiWP()
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
            //var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "setDataSet(" + jsonData + ");", true);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<OnarimListItem> list = GetDataList();
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

        private List<OnarimListItem> GetDataList()
        {


            Onarim onarim = new Onarim();
            DataTable dataTable = onarim.SelectAllReturnDataTable();



            List<OnarimListItem> list = new List<OnarimListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            foreach (DataRow row in dataTable.Rows)
            {
                string onarimId = row["OnarimId"].ToString();
                string yapilanIs = row["YapilanIs"].ToString();
                string harcamaUsulu = row["HarcamaUsulu"].ToString();
                string onayTarihi = row["OnayTarihi"].ToString();
                string tutar = row["Tutar"].ToString();
                string aciklama = row["Aciklama"].ToString();
                string iliIlcesi = row["IliIlcesi"].ToString();
                string adres = row["Adres"].ToString();

                string tasinmazId = row["TasinmazId"].ToString();



                OnarimListItem onarimListItem = new OnarimListItem();
                onarimListItem.OnarimId = onarimId;
                onarimListItem.YapilanIs = yapilanIs;
                onarimListItem.HarcamaUsulu = harcamaUsulu;
                onarimListItem.OnayTarihi = onayTarihi;
                onarimListItem.Tutar = tutar;
                onarimListItem.Aciklama = aciklama;
                onarimListItem.IliIlcesi = iliIlcesi;
                onarimListItem.Adres = adres;


                onarimListItem.TasinmazKarti = "<a target='_blank' href=" + ProjeConstants.PAGE_TASINMAZ_KARTI + "?SenderApp=TL&TasinmazId=" + tasinmazId + " class='btn btn-outline-info'>Tasinmaz Karti</a>";
                onarimListItem.Duzenle = "<a href=" + ProjeConstants.PAGE_TASINMAZ_GIRIS + "?DestinationApp=TD&TasinmazId=" + tasinmazId + " class='btn btn-outline-primary'>Düzenle</a>";
                list.Add(onarimListItem);
            }
            return list;
        }
        private class OnarimListItem
        {
            public string OnarimId { get; set; }
            public string YapilanIs { get; set; }
            public string HarcamaUsulu { get; set; }
            public string OnayTarihi { get; set; }
            public string Tutar { get; set; }
            public string Aciklama { get; set; }
            public string IliIlcesi { get; set; }
            public string Adres { get; set; }
            public string TasinmazKarti { get; set; }
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

            GridView1.DataSource = GetDataList();
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
