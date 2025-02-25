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

namespace TBYS_WebParts.GenelSozlesmeListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class GenelSozlesmeListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public GenelSozlesmeListesiWP()
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
        private string PageIndexQS
        {
            get
            {

                if (ViewState["PageIndex"] == null)
                {
                    if (Page.Request.QueryString["PageIndex"] != null)
                    {
                        ViewState["PageIndex"] = Page.Request.QueryString["PageIndex"];
                    }
                    else
                    {
                        ViewState["PageIndex"] = string.Empty;
                    }
                }
                return ViewState["PageIndex"].ToString();
            }

            set
            {
                ViewState["PageIndex"] = value;
            }
        }
        private string BolgeQS
        {
            get
            {

                if (ViewState["Bolge"] == null)
                {
                    if (Page.Request.QueryString["Bolge"] != null)
                    {
                        ViewState["Bolge"] = Page.Request.QueryString["Bolge"];
                    }
                    else
                    {
                        ViewState["Bolge"] = string.Empty;
                    }
                }
                return ViewState["Bolge"].ToString();
            }

            set
            {
                ViewState["Bolge"] = value;
            }
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
            var jsonData = KiraSozlesmeJson(); //veri çekilip json a çeviriliyor
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

            string queryStr = "&PageIndex='+currentPage+'";
            string tableString = @"
                // paginationa tıklandığında page degerini pageIndex degiskeninde saklar 
                $(document).on('click', '.ui-paginator-page', function () {
                    pageIndex = parseInt($(this).text());
                });

                $('#tblfilter').puidatatable({
                caption: '',
                editMode: 'cell',
                paginator: {
                            rows: 8
                            },
                columns: [
                    { field: 'Sirano', headerText: 'S. No',bodyClass:'text-center',headerStyle:'width: 4%',bodyClass:'small-font'},
                    { field: 'KiraciAdi', headerText: 'Kiracı',filter: true,sortable:true,headerStyle:'width: 13%',bodyClass:'small-font', content: function (rowData)
                        { 
                            var sirano=parseInt(rowData.Sirano);
                            var currentPage=Math.ceil(sirano/8);
                            return $('<a href=" + ProjeConstants.PAGE_KIRASOZLESMESI + @"?DestinationApp=KS&SenderApp=KSL&KiraSozlesmeId='+rowData.SozlesmeId +'" + queryStr + @" class=\'text-link \'>'+rowData.KiraciAdi+'</a>')
                        }
                    }, 
                    { field: 'Adres', headerText: 'Adres',filter: true,sortable:true,headerStyle:'width: 20%',bodyClass:'small-font'},                   
                    { field: 'SozlesmeTarihi', headerText: 'Söz. Tar.',headerStyle:'width: 8%',bodyClass:'text-end small-font' },
                    { field: 'SozBasTar', headerText: 'Söz. Baş.',headerStyle:'width: 8%', bodyClass:'text-end small-font'}, 
                    { field: 'SozBitTar', headerText: 'Söz. Bit.',headerStyle:'width: 8%', bodyClass:'text-end small-font'},                    
                    { field: 'KiraBedeli', headerText: 'Kira Bedeli',bodyClass:'text-end small-font',headerStyle:'width: 7%' },
                    { field: 'FaizliBakiye', headerText: 'Faizli Bakiye',bodyClass:'text-end small-font',headerStyle:'width: 7%' },
{ field: 'AnaPara', headerText: 'Ana Para',bodyClass:'text-end small-font',headerStyle:'width: 7%' },
{ field: 'FaizTutari', headerText: 'Faiz Tutarı',bodyClass:'text-end small-font',headerStyle:'width: 7%' },
                    { field: 'KiraBorcu', headerText: 'Kira Bor.',bodyClass:'text-end small-font',headerStyle:'width: 4%' },
                    { field: 'TeminatTutari', headerText: 'Tem. Tutarı',bodyClass:'text-end small-font',headerStyle:'width: 7%' },                    
                ],
                datasource:" + jsonData + @",
                resizableColumns: true,
                globalFilter:'#globalFilter'
                });
            ";

            return tableString;
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

            GridView1.DataSource = GetDataList();//SozlesmeListesiGetirDT();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=GenelSozlesmeListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
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
        private string KiraSozlesmeJson()
        {
            string jSon = string.Empty;

            List<KiraSozlesmeListItem> list = GetDataList();
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            jSon = serializer.Serialize(list);
            return jSon;
        }

        private List<KiraSozlesmeListItem> GetDataList()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            DateTime today = DateTime.Today;
            DateTime vadeBastar = today.AddMonths(-1).AddDays(1);
            DateTime vadeBittar = today;

            KiraSozlesmeListItem tempSozlesmeItem = null;
            List<KiraSozlesmeListItem> list = new List<KiraSozlesmeListItem>();

            OdemePlani opl = new OdemePlani();
            DataTable dataTable = opl.SelectMevcutOdemePlanlariByTarih(vadeBastar, vadeBittar, BolgeQS);
            DataView dataView = new DataView(dataTable);
            dataView.Sort = "Bolge,DosyaNo,Kiraci";
            int tempSozlesmeId = 0;
            int sirano = 1;
            if (dataTable != null)
            {

                foreach (DataRowView row in dataView)
                {
                    string ili = string.Empty;
                    string ilcesi = string.Empty;
                    string adres = string.Empty;

                    int kiraSozlesmeId = row == null ? 0 : row["KiraSozlesmeId"].ReturnEmptyIfNull().ConvertToInt();
                    string bolge = row == null ? "" : row["Bolge"].ReturnEmptyIfNull().ToString();
                    string kiraci = row == null ? "" : row["Kiraci"].ReturnEmptyIfNull().ToString();
                    string ilkSozlesmeTar = row == null ? "" : row["IlkSozlesmeTar"].ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                    string sozBasTar = row == null ? "" : row["SozBasTar"].ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                    string sozBitTar = row == null ? "" : row["SozBitTar"].ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                    string kullanimSekli = row == null ? "" : row["KullanimSekli"].ReturnEmptyIfNull().ToString();
                    string artisAyi = row == null ? "" : row["ArtisAyi"].ReturnEmptyIfNull().ToString();
                    string odemeSekli = row == null ? "" : row["OdemeSekli"].ReturnEmptyIfNull().ToString();
                    decimal kiraBedeliDec = row == null ? 1 : row["KiraBedeli"].ConvertToDecimal() == 0 ? 1 : row["KiraBedeli"].ConvertToDecimal();
                    string kiraBedeli = row == null ? "" : row["KiraBedeli"].ConvertToDecimal().ToString("N", culturInfo);
                    string anaPara = row == null ? "" : row["AnaPara"].ConvertToDecimal().ToString("N", culturInfo);
                    string faizTutari = row == null ? "" : row["FaizTutari"].ConvertToDecimal().ToString("N", culturInfo);
                    decimal faizliBakiyeDec = row == null ? 0 : row["FaizliBakiye"].ConvertToDecimal();
                    string faizliBakiye = row == null ? "" : row["FaizliBakiye"].ConvertToDecimal().ToString("N", culturInfo);
                    string teminatOdemeTar = row == null ? "" : row["TeminatOdemeTarihi"].ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                    string teminatTutari = row == null ? "" : row["TeminatTutari"].ConvertToDecimal().ToString("N", culturInfo);

                    int borcluAyAdedi = (int)(Math.Abs(faizliBakiyeDec) / kiraBedeliDec) - 1;// -1 çünkü içinde bulunduğumu ayı ödenmemiş kabul etmemeli, henuz vade bitmedi
                    //string BolumNo = row["BolumNo"].ToString();
                    adres = row["Adres"].ToString();// + " " + BolumNo;

                    ili = row["Ili"].ToString();
                    ilcesi = row["Ilcesi"].ToString();

                    if (tempSozlesmeId == kiraSozlesmeId)
                    {
                        tempSozlesmeId = kiraSozlesmeId;

                        list.Remove(tempSozlesmeItem);
                        tempSozlesmeItem.Adres += "@" + adres;
                        list.Add(tempSozlesmeItem);
                    }
                    else
                    {
                        KiraSozlesmeListItem sozlesmeItem = new KiraSozlesmeListItem();
                        sozlesmeItem.Sirano = sirano++.ToString();
                        sozlesmeItem.Bolge = bolge;
                        sozlesmeItem.Adres = "- " + adres;
                        sozlesmeItem.Ilcesi = ilcesi;
                        sozlesmeItem.Ili = ili;
                        sozlesmeItem.KiraciAdi = kiraci;
                        sozlesmeItem.SozlesmeTarihi = ilkSozlesmeTar;
                        sozlesmeItem.SozBasTar = sozBasTar;
                        sozlesmeItem.SozBitTar = sozBitTar;
                        sozlesmeItem.KullanimSekli = kullanimSekli;
                        sozlesmeItem.ArtisAyi = artisAyi;
                        sozlesmeItem.OdemeSekli = odemeSekli;
                        sozlesmeItem.KiraBedeli = kiraBedeli;
                        sozlesmeItem.AnaPara = anaPara;
                        sozlesmeItem.FaizTutari = faizTutari;
                        sozlesmeItem.FaizliBakiye = faizliBakiye;
                        sozlesmeItem.KiraBorcu = borcluAyAdedi < 1 ? "-" : borcluAyAdedi.ToString();
                        sozlesmeItem.TeminatOdemeTarihi = teminatOdemeTar;
                        sozlesmeItem.TeminatTutari = teminatTutari;
                        list.Add(sozlesmeItem);
                        tempSozlesmeItem = sozlesmeItem;
                    }
                    tempSozlesmeId = kiraSozlesmeId;


                }
            }
            else
            {
                MessageHelper.PublishMessage("Borçlu Kiracı bulunamadı", ProjeConstants.MESAJ_BILGI, 2000);
            }
            return list;
        }
        private class KiraSozlesmeListItem
        {
            public string Sirano { get; set; }
            public string Bolge { get; set; }
            public string KiraciAdi { get; set; }
            public string Adres { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string SozlesmeTarihi { get; set; }
            public string SozBasTar { get; set; }
            public string SozBitTar { get; set; }
            public string KullanimSekli { get; set; }
            public string ArtisAyi { get; set; }
            public string OdemeSekli { get; set; }
            public string KiraBedeli { get; set; }
            public string AnaPara { get; set; }
            public string FaizTutari { get; set; }
            public string FaizliBakiye { get; set; }
            public string KiraBorcu { get; set; }
            public string HukukiIslem { get; set; }
            public string TeminatOdemeTarihi { get; set; }
            public string TeminatTutari { get; set; }
        }
    }
}
