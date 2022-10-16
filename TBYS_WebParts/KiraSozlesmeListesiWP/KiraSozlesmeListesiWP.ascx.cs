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

namespace TBYS_WebParts.KiraSozlesmeListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraSozlesmeListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraSozlesmeListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
                        ViewState["SecilenId"] = "0";
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }
        private string AktifQS
        {
            get
            {

                if (ViewState["Aktif"] == null)
                {
                    if (Page.Request.QueryString["Aktif"] != null)
                    {
                        ViewState["Aktif"] = Page.Request.QueryString["Aktif"];
                    }
                    else
                    {
                        ViewState["Aktif"] = "1";
                    }
                }
                return ViewState["Aktif"].ToString();
            }

            set
            {
                ViewState["Aktif"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    BolgeQS = IKYSOrtak.PersonelinBolgesiniGetir(UtilityHelper.GetCurrentUser());
                    if (!string.IsNullOrEmpty(BolgeQS))
                    {
                        TitleLbl.Text = "Kira Sözleşme Listesi" + " (" + BolgeQS + " Bölgesi)";
                    }
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
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateDataTable(string jsonData)
        {
            string duzenleGorunsun = string.IsNullOrEmpty(BolgeQS) ? "{ targets:10, visible:true}," : "{ targets:10, visible:false},";
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
                    jQuery('#CustomModalDataTable').DataTable().destroy();
                }
                jQuery('#CustomModalDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                jQuery(document).ready(function () {
                    jQuery('#CustomDataTable').DataTable({
                        'initComplete': function (settings, json) {//tablo yüklendiğinde
                            var api = this.api();
                            var row = api.row(function (idx, data, node) { //secilen toplantıya gider
                                return data['Secildi'] == true;
                            });
                            if (row.length > 0) {
                                row.select()
                                    .show()
                                    .draw(false);
                            }
                        },
                        data: " + jsonData + @",
                        columns: [
                            { data: 'DosyaNo' },
                            { data: 'SozlesmeId' },
                            { data: 'Bolge' },
                            { data: 'KiraciAdi' },
                            { data: 'SozlesmeTarihi' },
                            { data: 'TarihAraligi' },
                            { data: 'OdemeSekli' },
                            { data: 'KiraBedeli' },
                            { data: 'Adres' },
                            { data: 'SozlesmePDFDosyasi' },
                            { data: 'Sozlesme' },
                        ],
                        'columnDefs': [
                            { 'width': '20%', 'targets': 3 },
                            { 'width': '25%', 'targets': 8 },
                            " + duzenleGorunsun + @"
                        ],
                        'language': {
                            'url': 'http://tskgv-portal/OrtakBelgeler/Turkish.txt',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        dom: 'Bfrtip',
                        //colon resizable
                        //initComplete: function (settings) {
                        //    $('#CustomDataTable').colResizable({ liveDrag: true });
                        //},
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
                        ],
                        'createdRow': function(row, data, dataIndex) {
                            if ((!data.Aktif)&&(!data.Secildi))
                            {
                                $(row).addClass('pasif-kiraci');

                            }
                        },//set row color


                        });
                    });

            ";

            return tableString;
        }

        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<KiraSozlesmeListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
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
        private List<KiraSozlesmeListItem> GetDataList()
        {

            DataTable dataTable;
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select(SecilenIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                AktifQS = kiraSozlesme.Aktif?ProjeConstants.KIRASOZLESME_AKTIF_INT.ToString():ProjeConstants.KIRASOZLESME_AKTIF_DEGIL_INT.ToString();
            }

            kiraSozlesme = new KiraSozlesme();
            dataTable = kiraSozlesme.SelectKiraSozlesmeListReturnDT(0, AktifQS.ConvertToInt(),BolgeQS);

            int SiraNo = 1;
            KiraSozlesmeListItem tempSozlesmeItem = null;
            List<KiraSozlesmeListItem> list = new List<KiraSozlesmeListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            DataView dataView = new DataView(dataTable);
            int tempSozlesmeId = 0;
            int tasinmazAdedi = 0;
            string ilkAdres = string.Empty;
            foreach (DataRowView row in dataView)
            {
                string ili = string.Empty;
                string ilcesi = string.Empty;
                string adres = string.Empty;

                string dosyaNo = row["DosyaNo"].ToString();
                string bolge = row["Bolge"].ToString();
                string kiraciAdi = row["KiraciAdi"].ToString();
                string kiraciSoyadi = row["KiraciSoyadi"].ToString();
                string ilkSozlesmeTar = row["IlkSozlesmeTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                string sozBasTar = row["SozBasTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                string sozBitTar = row["SozBitTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                string odemeSekli = row["odemeSekli"].ToString();
                string artisAyi = row["ArtisAyi"].ToString();
                decimal kiraBedeli = row["KiraBedeli"].ConvertToDecimal();
                int kiraSozlesmeId = row["KiraSozlesmeId"].ConvertToInt();
                string sozlesmePDFDosyasi = row["SozlesmePDFDosyasi"].ReturnEmptyIfNull().ToString();
                string BolumNo = row["BolumNo"].ToString();
                adres = row["Adres"].ToString() + " " + BolumNo;
                bool aktif = row["Aktif"].ReturnFalseIfNull().ConvertToBool();

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

                    //tempSozlesmeItem.Adres += "@" + adres;
                    tasinmazAdedi++;
                    tempSozlesmeItem.Adres = "@" + ilkAdres + "( Toplam " + tasinmazAdedi + " adet taşınmaz.)";
                    list.Add(tempSozlesmeItem);
                }
                else
                {
                    KiraSozlesmeListItem sozlesmeItem = new KiraSozlesmeListItem();
                    sozlesmeItem.Sirano = SiraNo++.ToString();
                    sozlesmeItem.DosyaNo = dosyaNo;
                    sozlesmeItem.Bolge = bolge;
                    sozlesmeItem.SozlesmeId = kiraSozlesmeId.ToString();
                    sozlesmeItem.KiraciAdi = kiraciAdi + " " + kiraciSoyadi;
                    sozlesmeItem.SozlesmeTarihi = ilkSozlesmeTar;
                    sozlesmeItem.SozlesmeBasTar = sozBasTar.ConvertToDatetimeEmptyIfNull();
                    sozlesmeItem.SozlesmeBitTar = sozBitTar.ConvertToDatetimeEmptyIfNull();
                    sozlesmeItem.TarihAraligi = sozBasTar.ConvertToDatetimeEmptyIfNull() + "-" + sozBitTar.ConvertToDatetimeEmptyIfNull();
                    sozlesmeItem.ArtisAyi = artisAyi;
                    sozlesmeItem.OdemeSekli = odemeSekli;
                    sozlesmeItem.KiraBedeli = kiraBedeli.ToString("N", culturInfo);
                    sozlesmeItem.Adres = "- " + adres;
                    sozlesmeItem.Ilcesi = ilcesi;
                    sozlesmeItem.Ili = ili;
                    if (!string.IsNullOrEmpty(sozlesmePDFDosyasi))
                    {
                        sozlesmeItem.SozlesmePDFDosyasi = @"<a  class='btn btn-outline-secondary' data-fancybox data-type=pdf data-width=960 data-height=720 href=" + UtilityHelper.TbysBelgelerURLGetir() + "/" + sozlesmePDFDosyasi + @">Kira Sözleşmesi</a>";
                    }
                    else
                    {
                        sozlesmeItem.SozlesmePDFDosyasi = "Dosya Yüklenmedi";
                    }
                    sozlesmeItem.Sozlesme = "<a href=" + ProjeConstants.PAGE_KIRASOZLESMESI + "?KiraSozlesmeId=" + kiraSozlesmeId + " class='btn btn-outline-primary'>Sözleşme</a>";
                    sozlesmeItem.Secildi = SecilenIdQS.Equals(sozlesmeItem.SozlesmeId);
                    sozlesmeItem.Aktif = aktif;
                    list.Add(sozlesmeItem);
                    tempSozlesmeItem = sozlesmeItem;
                    tasinmazAdedi = 1;
                }
                tempSozlesmeId = kiraSozlesmeId;

            }
            return list;
        }
        private class KiraSozlesmeListItem
        {
            public string Sirano { get; set; }
            public string DosyaNo { get; set; }
            public string Bolge { get; set; }
            public string SozlesmeId { get; set; }
            public string KiraciAdi { get; set; }
            public string SozlesmeTarihi { get; set; }
            public string SozlesmeBasTar { get; set; }
            public string SozlesmeBitTar { get; set; }
            public string TarihAraligi { get; set; }
            public string ArtisAyi { get; set; }
            public string OdemeSekli { get; set; }
            public string KiraBedeli { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Adres { get; set; }
            public string Sozlesme { get; set; }
            public string SozlesmePDFDosyasi { get; set; }
            public bool Aktif { get; set; }
            public bool Secildi { get; set; }
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
             "attachment;filename=KiraSozlesmeListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
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
