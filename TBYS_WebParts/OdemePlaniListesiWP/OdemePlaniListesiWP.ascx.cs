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
namespace TBYS_WebParts.OdemePlaniListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class OdemePlaniListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public OdemePlaniListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string KiraciIdQS
        {
            get
            {

                if (ViewState["KiraciId"] == null)
                {
                    if (Page.Request.QueryString["KiraciId"] != null)
                    {
                        ViewState["KiraciId"] = Page.Request.QueryString["KiraciId"];
                    }
                    else
                    {
                        ViewState["KiraciId"] = string.Empty;
                    }
                }
                return ViewState["KiraciId"].ToString();
            }

            set
            {
                ViewState["KiraciId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
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
            //var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "setDataSet(" + jsonData + ");", true);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<OdemePlaniListItem> list = GetDataList();
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
        private List<OdemePlaniListItem> GetDataList()
        {
            DataTable dataTable = GetData();

            List<OdemePlaniListItem> list = new List<OdemePlaniListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            foreach (DataRow row in dataTable.Rows)
            {
                string dosyaNo = row["DosyaNo"].ToString();
                string kiraciAdi = row["KiraciAdi"].ToString();
                string adres = string.Empty;
                string odemeBasTar = row["OdemeBasTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                string odemeBitTar = row["OdemeBitTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                decimal kiraBedeli = row["KiraBedeli"].ConvertToDecimal();
                decimal odenenTutar = row["OdenenTutar"].ConvertToDecimal();
                decimal faizliBakiye = row["FaizliBakiye"].ConvertToDecimal();


                int kiraSozlesmeId = row["SozlesmeId"].ConvertToInt();

                SozlesmeTasinmaz st = new SozlesmeTasinmaz();
                List<SozlesmeTasinmaz> stList = st.SelectBySozlesmeId(kiraSozlesmeId);
                foreach (SozlesmeTasinmaz item in stList)
                {
                    int tasinmazId = item.TasinmazId;
                    Tasinmaz tasinmaz = new Tasinmaz();
                    tasinmaz = tasinmaz.SelectById(tasinmazId);
                    if (tasinmaz != null)
                    {
                        string ili = tasinmaz.Ili;
                        string ilcesi = tasinmaz.Ilcesi;

                        if (item.BolumId > 0)
                        {
                            BagimsizBolum bagimsizBolum = new BagimsizBolum();
                            bagimsizBolum = bagimsizBolum.Select<BagimsizBolum>(item.BolumId);
                            if (bagimsizBolum != null)
                                adres += "<br>-" + tasinmaz.Adres + bagimsizBolum.BolumNo + " " + ilcesi + "/" + ili + System.Environment.NewLine;
                        }
                        else
                        {
                            adres += "<br>-" + tasinmaz.Adres + " " + ilcesi + "/" + ili + System.Environment.NewLine;
                        }
                    }
                }

                OdemePlaniListItem odemePlaniItem = new OdemePlaniListItem();

                odemePlaniItem.DosyaNo = dosyaNo;
                odemePlaniItem.KiraciAdi = kiraciAdi;
                odemePlaniItem.Adres = adres.Length > 4 ? (adres.Substring(0, 4).Equals("<br>") ? adres.Remove(0, 4) : adres) : adres;
                odemePlaniItem.OdemeBasBittar = odemeBasTar + '-' + odemeBitTar;
                odemePlaniItem.KiraBedeli = kiraBedeli.ToString("N", culturInfo);
                odemePlaniItem.OdenenTutar = odenenTutar.ToString("N", culturInfo);
                odemePlaniItem.FaizliBakiye = faizliBakiye.ToString("N", culturInfo);

                odemePlaniItem.Duzenle = "<a href=" + ProjeConstants.PAGE_ODEMEPLANI + "?DestinationApp=OPE&SenderApp=OPL&KiraSozlesmeId=" + kiraSozlesmeId + " class='btn btn-outline-primary'>Düzenle</a>";
                list.Add(odemePlaniItem);
            }

            return list;
        }

        private DataTable GetData()
        {
            DateTime bugun = DateTime.Today;
            OdemePlani odemePlani = new OdemePlani();
            DataTable dataTable = odemePlani.SelectOdemePlaniListByTarihReturnDT(bugun);
            return dataTable;
        }

        private class OdemePlaniListItem
        {
            public string DosyaNo { get; set; }
            public string KiraciAdi { get; set; }
            public string Adres { get; set; }
            public string OdemeBasBittar { get; set; }
            public string KiraBedeli { get; set; }
            public string OdenenTutar { get; set; }
            public string FaizliBakiye { get; set; }
            public string Duzenle { get; set; }

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

            GridView1.DataSource = GetData();//SozlesmeListesiGetirDT();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=OdemePlaniListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
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
