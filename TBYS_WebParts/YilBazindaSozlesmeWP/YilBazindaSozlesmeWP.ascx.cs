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


                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        private string CreateDataTable(string jsonData, string sozlesmeTahliye)
        {
            string sozlesmeTahliyeSutunu = sozlesmeTahliye.Equals("Sözleşme") ? "{ data: 'SozlesmeTarihi', 'width': '10%' },"
               : "{ Data: 'DurumDegismeTar', 'width': '10%' },";
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
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
                            { data: 'DosyaNo' ,'width':'5%'},
                            { data: 'KiraciAdi','width':'20%' },
                            " + sozlesmeTahliyeSutunu + @"
                            { data: 'Aktif' },
                            { data: 'SozlesmeDurumu' },
                            { data: 'KiraBedeli' },
                            { data: 'Adres' },
                            { data: 'SozlesmeId' },
                        ],
                        'columnDefs': [
                             {
                                targets:0, render:function(data, type, row, meta)
                                {
                                    var sirano=parseInt(row.Sirano);
                                    var currentPage=Math.ceil(sirano/8);
                                    return ('<a href=" + ProjeConstants.PAGE_KIRASOZLESMESI + @"?DestinationApp=KS&SenderApp=OPL&KiraSozlesmeId='+row.SozlesmeId +'"+ @" class=\'btn btn-outline-primary \'>'+row.DosyaNo +'</a>');

                                }
                            },
                            {
                                targets:7, render:function(data, type, row, meta)
                                {
                                    var sirano=parseInt(row.Sirano);
                                    var currentPage=Math.ceil(sirano/8);
                                    return ('<a href=" + ProjeConstants.PAGE_KIRASOZLESMESI + @"?DestinationApp=KS&SenderApp=KSL&KiraSozlesmeId='+row.SozlesmeId +'" + @" class=\'btn btn-outline-primary \'>Sözleşme</a>');
                                            
                                }
                            },
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

        private string TabloJson(string sozlesmeTahliye)
        {
            string jSon = string.Empty;

            try
            {
                List<KiraSozlesmeListItem> list = GetDataList(sozlesmeTahliye);
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
        private void TabloOlustur()
        {
            string sozlesmeTahliye = SozlesmeTahliyeDDL.SelectedItem.Value;
            var jsonData = TabloJson(sozlesmeTahliye); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData,sozlesmeTahliye); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
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
            TabloOlustur();
        }

        protected void SozlesmeTahliyeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
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
