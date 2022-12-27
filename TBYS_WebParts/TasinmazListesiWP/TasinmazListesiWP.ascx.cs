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


namespace TBYS_WebParts.TasinmazListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazListesiWP()
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

                if (ViewState["Auth"] == null || string.IsNullOrEmpty(ViewState["Auth"].ToString()))
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
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
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
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<TasinmazListesiListItem> list = GetDataList();
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
        private List<TasinmazListesiListItem> GetDataList()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            DataTable dataTable = tasinmaz.SelectByBolgeReturnJson(AuthQS);

            List<TasinmazListesiListItem> list = new List<TasinmazListesiListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            foreach (DataRow row in dataTable.Rows)
            {
                string tasinmazId = row["Id"].ToString();
                string kullanimSekli = row["KullanimSekli"].ToString();
                string mulkiyetSekli = row["MulkiyetSekli"].ToString();
                string iliIlcesi = row["IliIlcesi"].ToString();
                string adres = row["Adres"].ToString();
                string bagisci = row["Bagisci"].ToString();
                string bagisYili = row["BagisYili"].ToString();

                string kullanimDurumu = row["KullanimDurumu"].ToString();
                string sorumluBolge = row["SorumluBolge"].ToString();
                string emlakSicilNo = row["EmlakSicilNo"].ToString();

                string adaNo = row["AdaNo"].ToString();
                string parselNo = row["ParselNo"].ToString();
                string paftaNo = row["PaftaNo"].ToString();
                string yevmiyeNo = row["YevmiyeNo"].ToString();
                string ciltNo = row["CiltNo"].ToString();
                string sahifeNo = row["SahifeNo"].ToString();
                string cinsi = row["Cinsi"].ToString();
                string yuzolcumu = row["Yuzolcumu"].ToString();
                string arsaPayi = row["ArsaPayi"].ToString();
                string vakifHissesi = row["VakifHissesi"].ToString();
                string emlakBeyanDegeri = row["EmlakBeyanDegeri"].ReturnZeroIfNull().ConvertToDecimal().ToString("N", culturInfo);
                string tahminiRayicDegeri = row["TahminiRayicDegeri"].ReturnZeroIfNull().ConvertToDecimal().ToString("N", culturInfo);


                TasinmazListesiListItem tasinmazListesiListItem = new TasinmazListesiListItem();
                tasinmazListesiListItem.Id = tasinmazId;
                tasinmazListesiListItem.KullanimSekli = kullanimSekli;
                tasinmazListesiListItem.MulkiyetSekli = mulkiyetSekli;
                tasinmazListesiListItem.IliIlcesi = iliIlcesi;
                tasinmazListesiListItem.Adres = adres;
                tasinmazListesiListItem.Bagisci = bagisci;
                tasinmazListesiListItem.BagisYili = bagisYili.Trim();

                tasinmazListesiListItem.TasinmazKarti = "<a target='_blank' href=" + ProjeConstants.PAGE_TASINMAZ_KARTI + "?SenderApp=TL&TasinmazId=" + tasinmazId + " class='btn btn-outline-info'>Taşınmaz Kartı</a>";
                bool duzenleGorunsunMu = !string.IsNullOrEmpty(AuthQS) && AuthQS.Equals(ProjeConstants.TBYS_YETKILI_BIRIM);
                if (duzenleGorunsunMu)
                {
                    tasinmazListesiListItem.Duzenle = "<a href=" + ProjeConstants.PAGE_TASINMAZ_GIRIS + "?DestinationApp=TD&TasinmazId=" + tasinmazId + " class='btn btn-outline-primary'>Düzenle</a>"; 
                }

                tasinmazListesiListItem.KullanimDurumu = kullanimDurumu;
                tasinmazListesiListItem.SorumluBolge = sorumluBolge;
                tasinmazListesiListItem.EmlakSicilNo = emlakSicilNo;
                tasinmazListesiListItem.AdaNo = adaNo;
                tasinmazListesiListItem.ParselNo = parselNo;
                tasinmazListesiListItem.PaftaNo = paftaNo;
                tasinmazListesiListItem.YevmiyeNo = yevmiyeNo;
                tasinmazListesiListItem.CiltNo = ciltNo;
                tasinmazListesiListItem.SahifeNo = sahifeNo;
                tasinmazListesiListItem.Cinsi = cinsi;
                tasinmazListesiListItem.EmlakBeyanDegeri = emlakBeyanDegeri;
                tasinmazListesiListItem.TahminiRayicDegeri = tahminiRayicDegeri;
                tasinmazListesiListItem.AdaNo = adaNo;
                tasinmazListesiListItem.ParselNo = parselNo;
                tasinmazListesiListItem.PaftaNo = paftaNo;
                tasinmazListesiListItem.Yuzolcumu = yuzolcumu;
                tasinmazListesiListItem.ArsaPayi = arsaPayi;
                tasinmazListesiListItem.VakifHissesi = vakifHissesi;



                list.Add(tasinmazListesiListItem);
            }
            return list;
        }
        protected void YeniKayitBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_TASINMAZ_GIRIS);
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
        private DataTable GetDataListDT()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            DataTable dataTable = tasinmaz.SelectAllReturnDataTable();
            return dataTable;
        }
        private string CreateDataTable(string jsonData)
        {
            string duzenleGorunsun = string.IsNullOrEmpty(AuthQS) || !AuthQS.Equals(ProjeConstants.TBYS_YETKILI_BIRIM)
                ? "{ targets:9, visible:false},"
                : "{ targets:9, visible:true},";
            string tableString = @"
                $(document).ready(function () {
                    // Setup - add a text input to each footer cell
                    $('#CustomDataTable tfoot tr')
                        .clone(true)
                        .addClass('filters')
                        .appendTo('#CustomDataTable thead');

                      var table =  $('#CustomDataTable').DataTable({
                        data: " + jsonData + @",
                        columns: [
                            { data: 'Id', 'width': '4%'   },
                            { data: 'KullanimSekli' , 'width': '10%' },
                            { data: 'MulkiyetSekli' },
                            { data: 'IliIlcesi', 'width': '10%'},
                            { data: 'Adres', 'width': '15%'},
                            { data: 'Bagisci' },
                            { data: 'BagisYili' },
                            { data: 'SorumluBolge' },                            
                            { data: 'TasinmazKarti' },
                            { data: 'Duzenle' },
                            { data: 'EmlakBeyanDegeri' },
                            { data: 'TahminiRayicDegeri' },
                            { data: 'AdaNo' },
                            { data: 'ParselNo' },
                            { data: 'PaftaNo' },
                            { data: 'Yuzolcumu' },
                            { data: 'ArsaPayi' },
                            { data: 'VakifHissesi' },
                        ],
                        'order': [[0, 'asc']],//Id Sıralı
                        columnDefs:
                            [
                            " + duzenleGorunsun + @"
                            { 'visible': false, targets: [10,11,12,13,14,15,16,17]},
                            {  targets : [10,11],className: 'dt-body-right'},
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
                        initComplete: function () {
                            var api = this.api();
 
                            // For each column
                            api
                                .columns([6])
                                .eq(0)
                                .each(function (colIdx) {
                                    // Set the header cell to contain the input element
                                    var cell = $('.filters th').eq(
                                        $(api.column(colIdx).header()).index()
                                    );
                                    var title = $(cell).text();
                                    $(cell).html('<input type=text  placeholder=' + title + ' />');
 
                                    // On every keypress in this input
                                    $(
                                        'input',
                                        $('.filters th').eq($(api.column(colIdx).header()).index())
                                    )
                                        .off('keyup change')
                                        .on('change', function (e) {
                                            // Get the search value
                                            $(this).attr('title', $(this).val());
                                            var regexr = '({search})'; //$(this).parents('th').find('select').val();
 
                                            var cursorPosition = this.selectionStart;
                                            // Search the column for that value
                                            api
                                                .column(colIdx)
                                                .search(
                                                    this.value != ''
                                                        ? regexr.replace('{search}', '(((' + this.value + ')))')
                                                        : '',
                                                    this.value != '',
                                                    this.value == ''
                                                )
                                                .draw();
                                        })
                                        .on('keyup', function (e) {
                                            e.stopPropagation();
 
                                            $(this).trigger('change');
                                            $(this)
                                                .focus()[0]
                                                .setSelectionRange(cursorPosition, cursorPosition);
                                        });
                                });
                            table.columns.adjust().draw();
                        },
                    });
                   

                });";
            return tableString;
        }
        protected void ExportToExcel()
        {
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;

            GridView1.DataSource = GetDataListDT();
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
        private class TasinmazListesiListItem
        {
            public string Id { get; set; }
            public string KullanimSekli { get; set; }
            public string MulkiyetSekli { get; set; }
            public string IliIlcesi { get; set; }
            public string Adres { get; set; }
            public string Bagisci { get; set; }
            public string BagisYili { get; set; }
            public string TasinmazKarti { get; set; }
            public string Duzenle { get; set; }
            
            public string SorumluBolge { get; set; }
            public string EmlakSicilNo { get; set; }
            public string YevmiyeNo { get; set; }
            public string CiltNo { get; set; }
            public string SahifeNo { get; set; }
            public string Cinsi { get; set; }
            public string KullanimDurumu { get; set; }
            public string EmlakBeyanDegeri { get; set; }
            public string TahminiRayicDegeri { get; set; }
            public string AdaNo { get; set; }
            public string ParselNo { get; set; }
            public string PaftaNo { get; set; }
            public string Yuzolcumu { get; set; }
            public string ArsaPayi { get; set; }
            public string VakifHissesi { get; set; }
        }
    }
}
