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

namespace TBYS_WebParts.YilBazindaSozlesmeWP
{
    [ToolboxItemAttribute(false)]
    public partial class YilBazindaSozlesmeWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public YilBazindaSozlesmeWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SecilenYilQS
        {
            get
            {

                if (ViewState["SecilenYil"] == null)
                {
                    if (Page.Request.QueryString["SecilenYil"] != null)
                    {
                        ViewState["SecilenYil"] = Page.Request.QueryString["SecilenYil"];
                    }
                    else
                    {
                        ViewState["SecilenYil"] = string.Empty;
                    }
                }
                return ViewState["SecilenYil"].ToString();
            }

            set
            {
                ViewState["SecilenYil"] = value;
            }
        }
        private string SecilenSozlesmeTahliyeQS
        {
            get
            {

                if (ViewState["SecilenSozlesmeTahliye"] == null)
                {
                    if (Page.Request.QueryString["SecilenSozlesmeTahliye"] != null)
                    {
                        ViewState["SecilenSozlesmeTahliye"] = Page.Request.QueryString["SecilenSozlesmeTahliye"];
                    }
                    else
                    {
                        ViewState["SecilenSozlesmeTahliye"] = string.Empty;
                    }
                }
                return ViewState["SecilenSozlesmeTahliye"].ToString();
            }

            set
            {
                ViewState["SecilenSozlesmeTahliye"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    YilDDLDoldur();
                    SozlesmeTahliyeDDLDoldur();
                    DateTime today = DateTime.Today;
                    int yil = today.Year;

                    UtilityHelper.SetDDLValue(YilDDL, string.IsNullOrEmpty(SecilenYilQS) ? yil.ToString() : SecilenYilQS);
                    UtilityHelper.SetDDLValue(SozlesmeTahliyeDDL, SecilenYilQS);


                    KayitGetir();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void YilDDLDoldur()
        {
            var year = DateTime.Now.Year;
            for (int i = 1987; i <= year; i++)
            {
                YilDDL.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }
        private void SozlesmeTahliyeDDLDoldur()
        {
            SozlesmeTahliyeDDL.Items.Add(new ListItem("Sözleşme", "Sözleşme"));
            SozlesmeTahliyeDDL.Items.Add(new ListItem("Tahliye", "Tahliye"));
        }
        private void KayitGetir()
        {
            string sozlesmeTahliye = SozlesmeTahliyeDDL.SelectedItem.Value;
            var jsonData = KiraSozlesmeJson(sozlesmeTahliye); //veri çekilip json a çeviriliyor
            var jsString = CreateJsString(jsonData, sozlesmeTahliye); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        private string CreateJsString(string jsonData, string sozlesmeTahliye)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string rawUrl = System.Web.HttpContext.Current.Request.RawUrl.ToString();
            int index = currentUrl.IndexOf(rawUrl);
            string rootUrl = (index < 0) ? currentUrl : currentUrl.Remove(index, rawUrl.Length);
            string imgUrl = rootUrl + "/" + ProjeConstants.RESIMLER_TASINMAZ + "/_t/";
            string pageUrl = rootUrl + "/" + ProjeConstants.PAGE_TASINMAZ_GIRIS;

            string sozlesmeTahliyeSutunu = sozlesmeTahliye.Equals("Sözleşme") ? "{ field: 'SozlesmeTarihi', headerText: 'İlk Söz.Tar.',filter: true,headerStyle: 'width: 10%' },"
                : "{ field: 'DurumDegismeTar', headerText: 'Tahliye Tar',filter: true,headerStyle:'width: 10%' },";
            string queryStr = "&PageIndex='+currentPage+'";
            //{ field: 'Aktif', headerText: 'Aktif',filter: true,sortable:true,headerStyle:'width: 9%' }, 
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
                    { field: 'DosyaNo',filter: true, headerText: 'D.No',bodyClass:'text-center',headerStyle:'width: 5%', content: function (rowData)
                        { 
                            var sirano=parseInt(rowData.Sirano);
                            var currentPage=Math.ceil(sirano/8);
                            return $('<a href=" + ProjeConstants.PAGE_KIRASOZLESMESI + @"?DestinationApp=KS&SenderApp=OPL&KiraSozlesmeId='+rowData.SozlesmeId +'" + queryStr + @" class=\'btn btn-otline-primary \'>'+rowData.DosyaNo +'</a>')
                        }
                    },
                    { field: 'KiraciAdi', headerText: 'Kiracı',filter: true,sortable:true,headerStyle:'width: 20%' },                    
                    " + sozlesmeTahliyeSutunu + @"
                    { field: 'Aktif', headerText: 'Aktif',filter: true,sortable:true,headerStyle:'width: 9%' }, 
                    { field: 'SozlesmeDurumu', headerText: 'Söz.Durumu',filter: true,sortable:true,headerStyle:'width: 12%' },                    
                    { field: 'KiraBedeli', headerText: 'Kira Bedeli',filter: true,bodyClass:'text-right',headerStyle:'width: 10%' },
                    { field: 'Adres', headerText: 'Adres',filter: true,headerStyle:'width: 25%', content: function (rowData)
                        { 
                            return rowData.Adres.replace(/@/g,'<br>- ');
                        }
                    },
                    { field: 'SozlesmeId',headerStyle:'width: 9%', content: function (rowData)
                        { 
                            var sirano=parseInt(rowData.Sirano);
                            var currentPage=Math.ceil(sirano/8);
                            return $('<a href=" + ProjeConstants.PAGE_KIRASOZLESMESI + @"?DestinationApp=KS&SenderApp=KSL&KiraSozlesmeId='+rowData.SozlesmeId +'" + queryStr + @" class=\'btn btn-outline-primary \'>Sözleşme</a>')
                        }
                    }
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
            string sozlesmeTahliye = SozlesmeTahliyeDDL.SelectedItem.Value;
            GridView1.DataSource = GetDataList(sozlesmeTahliye);//SozlesmeListesiGetirDT();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=" + SecilenYilQS + "_" + SecilenSozlesmeTahliyeQS + "_" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
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
        private string KiraSozlesmeJson(string sozlesmeTahliye)
        {
            string jSon = string.Empty;

            List<KiraSozlesmeListItem> list = GetDataList(sozlesmeTahliye);
            var serializer = new JavaScriptSerializer();
            jSon = serializer.Serialize(list);
            return jSon;
        }
        private List<KiraSozlesmeListItem> GetDataList(string sozlesmeTahliye)
        {

            KiraSozlesme kiraSozlesme = new KiraSozlesme();

            DataTable dataTableSozlesme = kiraSozlesme.SelectSozlesmeListByYilReturnDT(YilDDL.SelectedItem.Value.ConvertToInt());
            DataTable dataTableTahliye = kiraSozlesme.SelectBitenSozlesmeListByYilReturnDT(YilDDL.SelectedItem.Value.ConvertToInt());
            DataTable dataTable = sozlesmeTahliye.Equals("Sözleşme") ? dataTableSozlesme : dataTableTahliye;

            int SiraNo = 1;
            string tempIli = string.Empty;
            string tempIlcesi = string.Empty;
            string tempAdres = string.Empty;
            KiraSozlesmeListItem tempSozlesmeItem = null;
            List<KiraSozlesmeListItem> list = new List<KiraSozlesmeListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            DataView dataView = new DataView(dataTable);
            int tempSozlesmeId = 0;
            foreach (DataRowView row in dataView)
            {
                string ili = string.Empty;
                string ilcesi = string.Empty;
                string adres = string.Empty;

                string dosyaNo = row["DosyaNo"].ToString();
                string kiraciAdi = row["KiraciAdi"].ToString();
                string kiraciSoyadi = row["KiraciSoyadi"].ToString();
                string ilkSozlesmeTar = row["IlkSozlesmeTar"].ToString();
                string sozBasTar = row["SozBasTar"].ToString();
                string sozBitTar = row["SozBitTar"].ToString();
                string odemeSekli = row["odemeSekli"].ToString();
                decimal kiraBedeli = row["KiraBedeli"].ConvertToDecimal();
                int kiraSozlesmeId = row["KiraSozlesmeId"].ConvertToInt();
                string sozlesmeDurumu = row["SozlesmeDurumu"].ToString();
                int aktif = row["Aktif"].ConvertToInt();
                string BolumNo = row["BolumNo"].ToString();
                adres = row["Adres"].ToString() + " " + BolumNo;

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
                    sozlesmeItem.Sirano = SiraNo++.ToString();
                    sozlesmeItem.DosyaNo = dosyaNo;
                    sozlesmeItem.SozlesmeId = kiraSozlesmeId.ToString();
                    sozlesmeItem.KiraciAdi = kiraciAdi + " " + kiraciSoyadi;
                    sozlesmeItem.SozlesmeTarihi = ilkSozlesmeTar;
                    sozlesmeItem.SozlesmeBasTar = sozBasTar.ConvertToDatetimeEmptyIfNull();
                    sozlesmeItem.SozlesmeBitTar = sozBitTar.ConvertToDatetimeEmptyIfNull();
                    sozlesmeItem.SozlesmeDurumu = string.IsNullOrEmpty(sozlesmeDurumu) ? "" : sozlesmeDurumu;
                    sozlesmeItem.Aktif = aktif == 0 ? "Aktif Değil" : "Aktif";
                    sozlesmeItem.TarihAraligi = sozBasTar.ConvertToDatetimeEmptyIfNull() + "-" + sozBitTar.ConvertToDatetimeEmptyIfNull();
                    sozlesmeItem.OdemeSekli = odemeSekli;
                    sozlesmeItem.KiraBedeli = kiraBedeli.ToString("N", culturInfo);
                    sozlesmeItem.Adres = "- " + adres;
                    sozlesmeItem.Ilcesi = ilcesi;
                    sozlesmeItem.Ili = ili;
                    list.Add(sozlesmeItem);
                    tempSozlesmeItem = sozlesmeItem;
                }
                tempSozlesmeId = kiraSozlesmeId;

            }
            return list;
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            KayitGetir();
        }

        protected void SozlesmeTahliyeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            KayitGetir();
        }
        private class KiraSozlesmeListItem
        {
            public string Sirano { get; set; }
            public string DosyaNo { get; set; }
            public string SozlesmeId { get; set; }
            public string KiraciAdi { get; set; }
            public string SozlesmeTarihi { get; set; }
            public string SozlesmeBasTar { get; set; }
            public string SozlesmeBitTar { get; set; }
            public string SozlesmeDurumu { get; set; }
            public string Aktif { get; set; }
            public string TarihAraligi { get; set; }
            public string OdemeSekli { get; set; }
            public string KiraBedeli { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Adres { get; set; }
        }


    }
}
