using Model.NBYS;
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

        private int BolgeIdQS
        {
            get
            {

                if (ViewState["BolgeId"] == null)
                {
                    if (Page.Request.QueryString["BolgeId"] != null)
                    {
                        ViewState["BolgeId"] = Page.Request.QueryString["BolgeId"];
                    }
                    else
                    {
                        ViewState["BolgeId"] = string.Empty;
                    }
                }
                return ViewState["BolgeId"].ReturnZeroIfNull().ConvertToInt();
            }

            set
            {
                ViewState["BolgeId"] = value;
            }
        }
        private string CurrentUserName
        {
            get
            {

                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUserLoginName();
                }
                return ViewState["CurrentUserName"].ToString();
            }

            set
            {
                ViewState["CurrentUserName"] = value;
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
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    Bolge bolge = IKYSOrtak.BolgeGetirByUserName(CurrentUserName);
                    BolgeIdQS = bolge == null ? 0 : bolge.Id;
                    TabloOlustur();
                }
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
        #region DataTable
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
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
        private DataTable GetDataListDT()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            DataTable dataTable = tasinmaz.SelectAllReturnDataTable();
            SatisPlaniKolonunuDisplayAdinaCevir(dataTable);
            return dataTable;
        }
        private void SatisPlaniKolonunuDisplayAdinaCevir(DataTable dataTable)
        {
            if (dataTable == null || !dataTable.Columns.Contains("SatisPlani"))
                return;

            int columnIndex = dataTable.Columns["SatisPlani"].Ordinal;
            dataTable.Columns.Add("SatisPlaniText", typeof(string));

            foreach (DataRow row in dataTable.Rows)
            {
                if (row["SatisPlani"] == DBNull.Value)
                    continue;

                int satisPlaniInt = row["SatisPlani"].ConvertToInt();
                Tasinmaz.SatisPlaniDurumu satisPlaniDurumu = (Tasinmaz.SatisPlaniDurumu)satisPlaniInt;
                row["SatisPlaniText"] = UtilityHelper.GetEnumDisplayName(satisPlaniDurumu);
            }

            dataTable.Columns.Remove("SatisPlani");
            dataTable.Columns["SatisPlaniText"].SetOrdinal(columnIndex);
            dataTable.Columns["SatisPlaniText"].ColumnName = "Taşınmaza Yapılacak İşlem";
            if (dataTable.Columns.Contains("SatisPlaniAciklama"))
                dataTable.Columns["SatisPlaniAciklama"].ColumnName = "Taşınmaza Yapılacak İşlem Açıklaması";
        }
        private string CreateDataTable(string jsonData)
        {
            string duzenleGorunsun = string.IsNullOrEmpty(AuthQS) || !AuthQS.Equals(ProjeConstants.TBYS_YETKILI_BIRIM) || 
                (BolgeIdQS != ProjeConstants.HEPSI_INT && BolgeIdQS != ProjeConstants.BOLGE_GENELMUDURLUK_INT )
                ? "{ targets:11, visible:false},"
                : "{ targets:11, visible:true},";
            string tableString = @"

                $(document).ready(function () {
                    //// Setup - add a text input to each footer cell
                    $('#CustomDataTable tfoot tr')
                        .clone(true)
                        .addClass('filters')
                        .appendTo('#CustomDataTable thead');

                      var table =  $('#CustomDataTable').DataTable({
                        'initComplete': function (settings, json) {//tablo yüklendiginde
                                var api = this.api();
                                var row = api.row(function (idx, data, node) { //secilen satira gider
                                    return data['Id'] == " + SecilenIdQS + @";
                                });
                                if (row.length > 0) {
                                    row.select()
                                        .show()
                                        .draw(false);
                                }
                            },
                        data: " + jsonData + @",
                        columns: [
                            { data: 'Id'},
                            { data: 'Bolge'},                            
                            { data: 'Bagisci' },
                            { data: 'Adres',},
                            { data: 'Ili',},
                            { data: 'Ilcesi',},
                            { data: 'MulkiyetSekli'},
                            { data: 'Cinsi'},
                            { data: 'KullanimSekli'},
                            { data: 'BagisYili',},
                            { data: 'TasinmazKarti'},
                            { data: 'Resimler'},
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
                        'order': [[0, 'asc']],//Id Sirali
                        columnDefs:
                            [
                            " + duzenleGorunsun + @"
                            { 'visible': false, targets: [13,14,15,16,17,18,19,20]},
                            {  targets : [13,14],className: 'dt-body-right'},
                            { width: '15%', targets: 2 },
                            { width: '20%', targets: 3 }
                            ],
                        'language': {
                            'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        dom: 'Bfrtip',
                        destroy:true,
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
                                .columns([9])
                                .eq(0)
                                .each(function (colIdx) {
                                    // Set the header cell to contain the input element
                                    var cell = $('.filters th').eq(
                                        $(api.column(colIdx).header()).index()
                                    );
                                    var title = $(cell).text();
                                    $(cell).html('<input type=text style=\'max-width: 80px\' placeholder=' + title + ' />');

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
        private List<TasinmazListesiListItem> GetDataList()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            DataTable dataTable = tasinmaz.SelectByBolgeReturnJson(BolgeIdQS);

            List<TasinmazListesiListItem> list = new List<TasinmazListesiListItem>();
            

            foreach (DataRow row in dataTable.Rows)
            {
                string tasinmazId = row["Id"].ToString();
                string kullanimSekli = row["KullanimSekli"].ToString();
                string mulkiyetSekli = row["MulkiyetSekli"].ToString();
                string ili = row["Ili"].ToString();
                string ilcesi = row["Ilcesi"].ToString();
                string adres = row["Adres"].ToString();
                string katMulkiyeti = row["KatMulkiyeti"].ToString();
                string bagisci = row["Bagisci"].ToString();
                string bagisYili = row["BagisYili"].ToString();

                string kiraDurumu = row["KiraDurumu"].ToString();
                string bolge = row["Bolge"].ToString();
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
                string yapiTarzi = row["YapiTarzi"].ToString();
                string insaatinSinifi = row["InsaatinSinifi"].ToString();
                string arazininCinsi = row["ArazininCinsi"].ToString();

                TasinmazListesiListItem tasinmazListesiListItem = new TasinmazListesiListItem();
                tasinmazListesiListItem.Id = tasinmazId;
                tasinmazListesiListItem.KullanimSekli = kullanimSekli;
                tasinmazListesiListItem.MulkiyetSekli = mulkiyetSekli;
                tasinmazListesiListItem.Ili = ili;
                tasinmazListesiListItem.Ilcesi = ilcesi;

                if (katMulkiyeti.Equals(ProjeConstants.KAT_MULKIYETI_YOK))
                {
                    tasinmazListesiListItem.Adres =  "<a class='btn btn-link' onclick=OpenModal(" + tasinmazId + ");>" + adres.Trim() + "</a>";
                }
                else
                {
                    tasinmazListesiListItem.Adres = adres;
                }
                tasinmazListesiListItem.Bagisci = bagisci;
                tasinmazListesiListItem.BagisYili = bagisYili.Trim();

                tasinmazListesiListItem.TasinmazKarti = "<a target='_blank' href=" + ProjeConstants.PAGE_TASINMAZ_KARTI + "?SenderApp=TL&TasinmazId=" + tasinmazId + " class='btn btn-outline-info'>Tasinmaz Karti</a>";
                tasinmazListesiListItem.Resimler = "<a target='_blank' href=" + ProjeConstants.PAGE_TASINMAZ_RESIMLER + "?TasinmazId=" + tasinmazId + " class='btn btn-outline-info'>Resimler/Belgeler</a>";

                bool duzenleGorunsunMu = !string.IsNullOrEmpty(AuthQS) ||
                    AuthQS.Equals(ProjeConstants.TBYS_YETKILI_BIRIM) ||
                    (BolgeIdQS != ProjeConstants.HEPSI_INT && BolgeIdQS != ProjeConstants.BOLGE_GENELMUDURLUK_INT);
                if (duzenleGorunsunMu)
                {
                    tasinmazListesiListItem.Duzenle = "<a href=" + ProjeConstants.PAGE_TASINMAZ_GIRIS + "?DestinationApp=TD&TasinmazId=" + tasinmazId + " class='btn btn-outline-primary'>Düzenle</a>"; 
                }
                else
                {
                    tasinmazListesiListItem.Duzenle = string.Empty;
                }

                tasinmazListesiListItem.KiraDurumu = kiraDurumu;
                tasinmazListesiListItem.Bolge = bolge;
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
                tasinmazListesiListItem.YapiTarzi = yapiTarzi;
                tasinmazListesiListItem.InsaatinSinifi = insaatinSinifi;
                tasinmazListesiListItem.ArazininCinsi = arazininCinsi;

                list.Add(tasinmazListesiListItem);
            }
            return list;
        }
        #endregion
        #region Modal
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ModalTabloOlustur();
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }
        }
        private void ModalTabloOlustur()
        {

            var jsonData = ModalTabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string ModalTabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<BagimsizBolumListItem> list = GetModalDataList();
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
        private string CreateModalDataTable(string jsonData)
        {
            string tableString = @"
                
                if ( jQuery.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
                    jQuery('#CustomModalDataTable').DataTable().destroy();
                }
                jQuery('#CustomModalDataTable tbody').empty();
                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
                jQuery(document).ready(function () {
                    jQuery('#CustomModalDataTable').DataTable({
                        'initComplete': function (settings, json) {//tablo yüklendiginde
                            var api = this.api();
                            var row = api.row(function (idx, data, node) { //secilen kayda gider
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
                            { data: 'BagisizBolumId' },
                            { data: 'Adres' },
                            { data: 'BolumNo' },
                            { data: 'Nitelik' },
                            { data: 'Metrekare' },
                            { data: 'KullanimAmaci' },
                            { data: 'Aciklama' },
                        ],
                        'order': [[0, 'desc']],//sort

                        'language': {
                            'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        dom: 'rtp',
                        });
                    });

            ";

            return tableString;
        }
        private List<BagimsizBolumListItem> GetModalDataList()
        {
            Tasinmaz tasinmaz = new Tasinmaz();

            DataTable dataTable = tasinmaz.SelectBolumByTasinmazId(paramTasinmazIdLbl.Value.ConvertToInt());

            List<BagimsizBolumListItem> list = new List<BagimsizBolumListItem>();

            if (dataTable != null)
            {
                DataRow row0 = dataTable.Rows[0];
                string adres = row0["Adres"].ReturnEmptyIfNull().ToString();
                string il = row0["Ili"].ReturnEmptyIfNull().ToString();
                string ilce = row0["Ilcesi"].ReturnEmptyIfNull().ToString();
                string adi0 = row0["Adi"].ToString();
                string soyadi0 = row0["Soyadi"].ToString();
                
                BagisciLbl.Text = "Bağışçı : "+ (adi0 + " " + soyadi0).Trim() ;
                AdresLbl.Text = "Adres : "+ adres +" " +ilce+"/"+il ;
                foreach (DataRow row in dataTable.Rows)
                {
                    int bolumId = row["BolumId"].ConvertToInt();
                    string bolumNo = row["BolumNo"].ToString();
                    string aciklama = row["Aciklama"].ToString();
                    string nitelik = row["Nitelik"].ToString();
                    string metrekare = row["Metrekare"].ReturnZeroIfNull().ConvertToDecimal().ToString("N", culturInfo);
                    string kullanimAmaci = row["KullanimAmaci"].ToString();

                    BagimsizBolumListItem bagimsizBolum = new BagimsizBolumListItem();
                    bagimsizBolum.Bagisci =(adi0 + " " + soyadi0).Trim() ;
                    bagimsizBolum.Adres = adres;
                    bagimsizBolum.Ili = il;
                    bagimsizBolum.Ilcesi = ilce;

                    bagimsizBolum.BolumNo = bolumNo;
                    bagimsizBolum.BagisizBolumId = bolumId;
                    bagimsizBolum.Nitelik = nitelik;
                    bagimsizBolum.Metrekare = metrekare;
                    bagimsizBolum.KullanimAmaci = kullanimAmaci;
                    bagimsizBolum.Aciklama = aciklama;

                    //bagimsizBolum.Secildi = SecilenIdQS.Equals(bolumId);
                    list.Add(bagimsizBolum);
                }
            }
            return list;
        }
        #endregion
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
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Adres { get; set; }
            public string Bagisci { get; set; }
            public string BagisYili { get; set; }
            public string TasinmazKarti { get; set; }
            public string Resimler { get; set; }
            public string Duzenle { get; set; }
            
            public string Bolge { get; set; }
            public string EmlakSicilNo { get; set; }
            public string YevmiyeNo { get; set; }
            public string CiltNo { get; set; }
            public string SahifeNo { get; set; }
            public string Cinsi { get; set; }
            public string KiraDurumu { get; set; }
            public string EmlakBeyanDegeri { get; set; }
            public string TahminiRayicDegeri { get; set; }
            public string AdaNo { get; set; }
            public string ParselNo { get; set; }
            public string PaftaNo { get; set; }
            public string Yuzolcumu { get; set; }
            public string ArsaPayi { get; set; }
            public string VakifHissesi { get; set; }
            public string YapiTarzi { get; set; }
            public string InsaatinSinifi { get; set; }
            public string ArazininCinsi { get; set; }
        }
        private class BagimsizBolumListItem
        {
            public int TasinmazId { get; set; }
            public int BagisizBolumId { get; set; }
            public string Bagisci { get; set; }
            public string Adres { get; set; }
            public string BolumNo { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Ada { get; set; }
            public string Parsel { get; set; }
            public string Yuzolcumu { get; set; }
            public string ArsaPayi { get; set; }
            public string Nitelik { get; set; }
            public string Metrekare { get; set; }
            public string KullanimAmaci { get; set; }
            public string Aciklama { get; set; }
            public bool Secildi { get; set; }
        }
    }
}
