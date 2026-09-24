using Model.NBYS;
using Model.Ortak;
using Model.Services.NBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
namespace NBYS_WebParts.NakitBagisciBulmaWP
{
    [ToolboxItemAttribute(false)]
    public partial class NakitBagisciBulmaWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NakitBagisciBulmaWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        private string ParamQS//nakit bagisci düzenlemeden dönüyorsa aranan texti tekrar arasin
        {
            get
            {

                if (ViewState["Param"] == null)
                {
                    if (Page.Request.QueryString["Param"] != null)
                    {
                        ViewState["Param"] = Page.Request.QueryString["Param"];
                    }
                    else
                    {
                        ViewState["Param"] = string.Empty;
                    }
                }
                return ViewState["Param"].ToString();
            }

            set
            {
                ViewState["Param"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty(ParamQS))
                    {
                        BagisciAraTxt.Text = ParamQS.Trim();
                        TabloOlustur();
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private string GetBagisciData()
        {
            string json = string.Empty;
            if (!string.IsNullOrEmpty(BagisciAraTxt.Text) && BagisciAraTxt.Text.Trim().Length > 3)
            {
                NakitBagisciService service = new NakitBagisciService();
                DataTable dataTable = service.Search(BagisciAraTxt.Text.Trim(), 0);
                json = ToJson(dataTable);
            }
            return string.IsNullOrEmpty(json) ? "[{}]" : json;
        }
        private static string ToJson(DataTable table)
        {
            if (table == null)
                return "[]";

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            foreach (DataRow row in table.Rows)
            {
                Dictionary<string, object> values = new Dictionary<string, object>();
                foreach (DataColumn column in table.Columns)
                {
                    values.Add(column.ColumnName, row[column]);
                }
                rows.Add(values);
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            return serializer.Serialize(rows);
        }
        private void TabloOlustur()
        {
            var jsonData = GetBagisciData(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
            jQuery(document).ready(function () {

            jQuery('#CustomDataTable').DataTable({
                'initComplete': function (settings, json) {//tablo yüklendiginde
                    var api = this.api();
                    var row = api.row(function(idx, data, node) { //secilen satira gider
                        return data['NakitBagisciId'] ==" + SecilenIdQS + @";
                    });
                    if (row.length > 0)
                    {
                        row.select()
                            .show()
                            .draw(false);
                    }
                },
                data: " + jsonData + @",
                columns: [
                    { data: 'NakitBagisciId' },
                    { data: 'Adi'},
                    { data: 'TCKimlikNo' },
                    { data: 'Ili' },
                    { data: 'Ilcesi' },
                    { data: 'Telefon1' },
                    { data: 'Adres'},
                    { data: 'NakitBagisciId' },
                    { data: 'NakitBagisciId' },

                ],
                'order': [[2, 'desc']],
                columnDefs:
                [
                {
                    targets: 1, render: function(data, type, row, meta) {
                    var link= '<a href=# onclick=OpenModal('+row.NakitBagisciId+'); class=\'btn btn-link \'>'+(row.Adi+' '+ row.Soyadi).trim() + '</a>';
                    return link;
                }},
                {
                    targets: 7, render: function(data, type, row, meta) {
                    var link= '<a href=' + '" + ProjeConstants.PAGE_NAKITBAGISCI_EDIT + @"?NakitBagisciId=' + row.NakitBagisciId + ' class=\' btn btn-outline-primary\' >Düzenle</a>';
                    return link;
                }},
                {
                    targets: 8, render: function(data, type, row, meta) {
                    var link= '<a href=' + '" + ProjeConstants.PAGE_EKSTRE_AKTARMAEDIT + @"?NakitBagisciId=' + row.NakitBagisciId + ' class=\' btn btn-outline-success\' >Bağış Gir</a>';
                    return link;
                }},

                ],
                'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                    'decimal': ',',
                    'thousands': '.'
                },
                responsive: true,
                destroy: true,
                autoWidth: false,
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
                      exportOptions: {
                          columns: ':visible',
                          format: {
                              body: function(data, row, column, node) {
                                  data = $('<p>' + data + '</p>').text();
                                  return $.isNumeric(data.replace(',', '.')) ? data.replace(',', '.') : data;
                              }
                          }
                      },
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
        });
        ";
            return tableString;
        }
        protected void BagisciAraTxt_TextChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void BagisciAraBtn_Click(object sender, EventArgs e)
        {
            TabloOlustur();
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
        protected void YeniBagisGirisiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_EKSTRE_AKTARMAEDIT);
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
        private void TabloModalOlustur(string nakitBagisciId)
        {
            var jsonData = GetModalDataJson(nakitBagisciId); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData, nakitBagisciId.ConvertToInt()); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateModalDataTable(string jsonData, int nakitBagisciId)
        {
            NakitBagisci nb = new NakitBagisciService().GetById(nakitBagisciId);
            string bagisciAdi = nb != null ? (nb.Adi + nb.Soyadi).ReplaceTrChars() : "Bagisci";
            string filename = bagisciAdi + "-" + DateTime.Today.Day + "-" + DateTime.Today.Month + "-" + DateTime.Today.Year;
            string tableString = @"
        jQuery(document).ready(function () {
            if ( jQuery.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
                jQuery('#CustomModalDataTable').DataTable().destroy();
            }
            jQuery('#CustomModalDataTable tbody').empty();

            jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
            jQuery('#CustomModalDataTable').DataTable({
                data: " + jsonData + @",
                columns: [
                    { data: 'BagisTarihi' },
                    { data: 'BagisMiktari', 'width': '10%', 'className': 'text-end' },
                    { data: 'Banka' },
                    { data: 'Armagan' },
                    { data: 'ArmaganTutari' },
                    { data: 'Durum' },
                ],
                'order': [[0, 'desc']],

                'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                    'decimal': ',',
                    'thousands': '.'
                },
                columnDefs:[
                    {targets:0, render:function(data){
                        return moment(data).format('DD.MM.YYYY');
                    }},
                ],
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
                      title:'" + filename + @"',
                      exportOptions: {
                          columns: ':visible',
                          format: {
                              body: function(data, row, column, node) {
                                  data = $('<p>' + data + '</p>').text();
                                    
                                  return $.isNumeric(data.replace(',', '.')) ? data.replace(',', '.') : data;
                              }
                          }
                      },
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
        });
        ";
            return tableString;
        }
        private string GetModalDataJson(string nakitBagisciId)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            NakitBagisHareket nbh = new NakitBagisHareket();
            int rowCount = 0;
            var json = nbh.SelectByBagisciIdReturnJSon(nakitBagisciId, ref rowCount);
            decimal toplamTutar = nbh.GetSumBagisMiktariByNakitBagisciIdBetweenBasTarBitTar(
                ProjeConstants.BAGIS_SORGU_BASTAR.ConvertToDatetime(), DateTime.Today, nakitBagisciId.ConvertToInt());
            BagisBilgileriLbl.Text = rowCount < 1 ? "Bağış bulunmamaktadır" :
                "Bağışçının " + rowCount + " defada yaptığı toplam " + toplamTutar.ToString("N", culturInfo) + "TL bağışı bulunmaktadır";
            return json;
        }
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            try
            {

                TabloModalOlustur(paramNakitBagisciIdLbl.Value);
                NakitBagisciFormunuDoldur(paramNakitBagisciIdLbl.Value);
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }
        }
        private void NakitBagisciFormunuDoldur(string nakitBagisciIdStr)
        {
            if (!string.IsNullOrEmpty(nakitBagisciIdStr))
            {
                int nakitBagisciId = nakitBagisciIdStr.ConvertToInt();

                NakitBagisci nakitBagisci = new NakitBagisciService().GetById(nakitBagisciId);
                if (nakitBagisci != null)
                {
                    //NakitBagisciIdLbl.Text = nakitBagisciId.ToString();
                    TableRow row = new TableRow();
                    TableCell AdiCell = new TableCell();
                    TableCell TCKimlikNoCell = new TableCell();
                    TableCell AdresCell = new TableCell();
                    TableCell IlIlceCell = new TableCell();
                    TableCell TelefonCell = new TableCell();
                    TableCell TuzelKisiCell = new TableCell();

                    AdiCell.Text = nakitBagisci.Adi.ReturnEmptyIfNull().ToString();
                    TCKimlikNoCell.Text = nakitBagisci.TCKimlikNo.ReturnEmptyIfNull().ToString();
                    AdresCell.Text = nakitBagisci.Adres.ReturnEmptyIfNull().ToString();

                    int ilId = nakitBagisci.Ili.ConvertToInt();
                    Il il = new Il();
                    il = il.Select<Il>(ilId);
                    if (il != null)
                    {

                        IlIlceCell.Text = il.IlAdi.ReturnEmptyIfNull().ToString();
                    }
                    int ilceId = nakitBagisci.Ilcesi.ConvertToInt();
                    Ilce ilce = new Ilce();
                    ilce = ilce.Select<Ilce>(ilceId);
                    if (ilce != null)
                    {

                        IlIlceCell.Text += " " + ilce.IlceAdi.ReturnEmptyIfNull().ToString();
                    }
                    TelefonCell.Text = nakitBagisci.Telefon1.ReturnEmptyIfNull().ToString();
                    TuzelKisiCell.Text = nakitBagisci.TuzelKisi.ConvertToBool() ? "Evet" : "Hayir";
                    row.Controls.Add(AdiCell);
                    row.Controls.Add(TCKimlikNoCell);
                    row.Controls.Add(AdresCell);
                    row.Controls.Add(IlIlceCell);
                    row.Controls.Add(TelefonCell);
                    row.Controls.Add(TuzelKisiCell);
                    BagisciTable.Controls.Add(row);

                }
            }
        }

    }
}
