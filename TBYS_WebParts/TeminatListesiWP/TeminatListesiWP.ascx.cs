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
namespace TBYS_WebParts.TeminatListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TeminatListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TeminatListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string AuthQS
        {
            get
            {

                if (ViewState["Auth"]==null || string.IsNullOrEmpty(ViewState["Auth"].ToString()))
                {
                    if (Page.Request.QueryString["Auth"] != null)
                    {
                        ViewState["Auth"] = Page.Request.QueryString["Auth"];
                    }
                    else
                    {
                        ViewState["Auth"] = string.Empty;
                    }
                }
                return ViewState["Auth"].ToString();
            }

            set
            {
                ViewState["Auth"] = value;
            }
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
            List<Tasinmaz> list = new List<Tasinmaz>();
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<TeminatListItem> list = GetDataList(false);
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
        private string CreateDataTable(string jsonData)
        {
            string duzenleGorunsun = string.IsNullOrEmpty(AuthQS) || !AuthQS.Equals(ProjeConstants.TBYS_YETKILI_BIRIM)
                ? "{ targets:9, visible:false},{ targets:10, visible:false},"
                : "{ targets:9, visible:true},{ targets:10, visible:true},";

            string tableString = @"
        jQuery(document).ready(function () {

            jQuery('#CustomDataTable').DataTable({
                data: " + jsonData + @",
                columns: [
                    { data: 'KiraciAdi' },
                    { data: 'Bolge' },
                    { data: 'TeminatOdemeTarihi' },
                    { data: 'KiraBedeli' },
                    { data: 'TeminatTutari' },
                    { data: 'OdenenTeminatTutari' },
                    { data: 'IadeTeminatTutari' },
                    { data: 'KalanTeminatTutari' },
                    { data: 'Adres', 'width': '20%' },
                    { data: 'Sozlesme' },
                    { data: 'Teminat' },

                ],
                'order': [[0, 'asc']],//AdiSoyadi Sıralı
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
                buttons:
                    [
                    {
                    extend: 'print',
                        exportOptions:
                        {
                        columns: ':visible'
                        }
                    },
                    {
                    extend: 'excel',
                        exportOptions:
                        {
                        columns: ':visible'
                        }
                    },
                    {
                    extend: 'pdf',
                        exportOptions:
                        {
                        columns: ':visible'
                        }
                    },
                    {
                    extend: 'copy',
                        exportOptions:
                        {
                        columns: ':visible'
                        }
                    },
                    , 'pageLength', 'colvis'
                ]
            });
        });";
            return tableString;
        }
        private List<TeminatListItem> GetDataList(bool isExcel)
        {
            int SiraNo = 1;
            string tempIli = string.Empty;
            string tempIlcesi = string.Empty;
            string tempAdres = string.Empty;


            DataTable dataTable = GetData();

            TeminatListItem tempSozlesmeItem = null;
            List<TeminatListItem> list = new List<TeminatListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            int tempSozlesmeId = 0;
            int tasinmazAdedi = 0;
            string ilkAdres = string.Empty;

            if (dataTable!=null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string ili = string.Empty;
                    string ilcesi = string.Empty;
                    string adres = string.Empty;

                    string kiraciAdi = row["KiraciAdi"].ToString();
                    string kiraciSoyadi = row["KiraciSoyadi"].ToString();
                    string bolge = row["Bolge"].ToString();
                    string ilkSozlesmeTar = row["IlkSozlesmeTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                    string teminatOdemeTarihi = row["TeminatOdemeTarihi"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                    string artisAyi = row["ArtisAyi"].ToString();
                    decimal kiraBedeli = row["KiraBedeli"].ConvertToDecimal();
                    decimal teminatTutari = row["TeminatTutari"].ConvertToDecimal();
                    decimal odenenTeminatTutari = row["OdenenTeminatTutari"].ConvertToDecimal();
                    decimal iadeTeminatTutari = row["IadeTeminatTutari"].ConvertToDecimal();
                    decimal kalanTeminatTutari = row["KalanTeminatTutari"].ConvertToDecimal();


                    int kiraSozlesmeId = row["KiraSozlesmeId"].ConvertToInt();
                    string BolumNo = row["BolumNo"].ToString();
                    adres = row["Adres"].ToString() + " " + BolumNo;
                    ili = row["Ili"].ToString();
                    ilcesi = row["Ilcesi"].ToString();
                    if (tasinmazAdedi == 0)
                    {
                        ilkAdres = adres;
                    }
                    if (tempSozlesmeId == kiraSozlesmeId)
                    {
                        tempSozlesmeId = kiraSozlesmeId;
                        list.Remove(tempSozlesmeItem);

                        tasinmazAdedi++;
                        tempSozlesmeItem.Adres = "@" + ilkAdres + "( Toplam " + tasinmazAdedi + " adet taşınmaz.)";
                        list.Add(tempSozlesmeItem);
                    }
                    else
                    {
                        TeminatListItem teminatItem = new TeminatListItem();
                        teminatItem.Sirano = SiraNo++.ToString();
                        teminatItem.SozlesmeId = kiraSozlesmeId.ToString();
                        teminatItem.KiraciAdi = kiraciAdi + " " + kiraciSoyadi;
                        teminatItem.Bolge = bolge;
                        teminatItem.SozlesmeTarihi = ilkSozlesmeTar;
                        teminatItem.TeminatOdemeTarihi = teminatOdemeTarihi.ConvertToDatetimeEmptyIfNull();
                        teminatItem.KiraBedeli = kiraBedeli.ToString("N", culturInfo);
                        teminatItem.TeminatTutari = teminatTutari.ToString("N", culturInfo);
                        teminatItem.OdenenTeminatTutari = odenenTeminatTutari.ToString("N", culturInfo);
                        teminatItem.IadeTeminatTutari = iadeTeminatTutari.ToString("N", culturInfo);
                        teminatItem.KalanTeminatTutari = kalanTeminatTutari.ToString("N", culturInfo);

                        teminatItem.Adres = "- " + adres;
                        teminatItem.Ilcesi = ilcesi;
                        teminatItem.Ili = ili;
                        list.Add(teminatItem);
                        tempSozlesmeItem = teminatItem;
                        tasinmazAdedi = 1;

                        if (!isExcel)
                        {
                            teminatItem.Sozlesme = "<a href=" + ProjeConstants.PAGE_KIRASOZLESMESI + "?KiraSozlesmeId=" + kiraSozlesmeId + " class='btn btn-outline-primary'>Sözleşme</a>";
                            teminatItem.Teminat = "<a href=" + ProjeConstants.PAGE_TEMINAT_ISLEMLERI + "?KiraSozlesmeId=" + kiraSozlesmeId + " class='btn btn-outline-primary'>Teminat</a>";
                        }

                    }
                    tempSozlesmeId = kiraSozlesmeId;

                } 
            }
            return list;
        }


        private DataTable GetData()
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            DataTable dataTable = kiraSozlesme.SelectKiraSozlesmeListReturnDT(KiraciIdQS.ConvertToInt(), ProjeConstants.KIRASOZLESME_AKTIF_INT,ProjeConstants.BOLGE_HEPSI);
            return dataTable;
        }

        private class TeminatListItem
        {
            public string Sirano { get; internal set; }
            public string KiraciAdi { get; set; }
            public string Bolge { get; set; }
            public string TeminatOdemeTarihi { get; set; }
            public string KiraBedeli { get; set; }
            public string TeminatTutari { get; set; }
            public string OdenenTeminatTutari { get; set; }
            public string IadeTeminatTutari { get; set; }
            public string KalanTeminatTutari { get; set; }
            public string Adres { get; set; }
            public string Sozlesme { get; set; }
            public string Teminat { get; set; }
            public string SozlesmeId { get; internal set; }
            public string SozlesmeTarihi { get; internal set; }
            public string Ilcesi { get; internal set; }
            public string Ili { get; internal set; }
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

            GridView1.DataSource = GetDataList(true);
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=TeminatListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
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
