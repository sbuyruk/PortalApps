using Utility.Logging;
using NBYS_WebParts.Logging;
using Model.NBYS;
using Model.Ortak;
using Model.Services.Ortak;
using Model.Services.NBYS;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.ArmaganAraWP
{
    [ToolboxItemAttribute(false)]
    public partial class ArmaganAraWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "ArmaganAraWP"); }
        }
        private bool canProcess = true;
        private Bolge userRegion;
        private void ExecuteAction(string operation, Action action)
        {
            NbysWebPartLog.ExecuteAction(LoggingContext, operation, () => { if (canProcess) action(); });
        }
        private void StopForWarning(string message)
        {
            canProcess = false;
            Messages.Warning(message);
        }
        private bool CanReadDonor(int donorId)
        {
            return new NbysBagisciWriteService(LoggingContext).CanReadDonor(donorId, userRegion);
        }

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ArmaganAraWP()
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
                int selectedId;
                return int.TryParse(Convert.ToString(ViewState["SecilenId"]), out selectedId) && selectedId > 0 ? selectedId.ToString(CultureInfo.InvariantCulture) : "0";
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }
        private string ParamQS//nakit bağışçı düzenlemeden dönüyorsa aranan metni tekrar arasın
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
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
                userRegion = new NbysBagisciWriteService(LoggingContext).GetUserRegion();
                if (userRegion == null) { StopForWarning("Bölge bilginiz bulunamadı. Yetki bilgilerinizi kontrol ettirin."); return; }
                if (!Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty(ParamQS))
                    {
                        ArmaganAraTxt.Text = ParamQS.Trim();
                        TabloOlustur();
                    }
                }
            });
        }
        private string GetArmaganData()
        {
            int id;
            if (!int.TryParse(ArmaganAraTxt.Text.Trim(), out id) || id <= 0)
            { Messages.Warning("Aramak için pozitif bir armağan numarası girin."); return "[]"; }
            string json = new ArmaganService(LoggingContext).SearchJson(id.ToString(CultureInfo.InvariantCulture), 0, userRegion.Id);
            if (string.IsNullOrEmpty(json) || json == "[]")
            { Messages.Info("Yetki alanınızda bu numarayla eşleşen armağan bulunamadı."); return "[]"; }
            return json;
        }
        private bool searchPerformed;
        private void TabloOlustur()
        {
            if (searchPerformed) return;
            searchPerformed = true;

            var jsonData = GetArmaganData(); //veri çekilip json'a çevriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);

        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
            jQuery(document).ready(function () {

            jQuery('#CustomDataTable').DataTable({
                'initComplete': function (settings, json) {//tablo yüklendiğinde
                    var api = this.api();
                    var row = api.row(function(idx, data, node) { //seçilen satıra gider
                        return data['ArmaganId'] ==" + SecilenIdQS + @";
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
                    { data: 'ArmaganId' },
                    { data: 'Adi'},
                    { data: 'Armagan' },
                    { data: 'BelgeTarihi' },
                    { data: 'TCKimlikNo' },
                    { data: 'Ili' },
                    { data: 'Ilcesi' },
                    { data: 'Telefon1' },
                    { data: 'Adres'},


                ],
                'order': [[2, 'desc']],
                columnDefs:
                [
                {
                    targets: 1, render: function(data, type, row, meta) {
                    var link= '<a href=# onclick=OpenModal('+row.NakitBagisciId+'); class=\'btn btn-link \'>'+(row.Adi+' '+ row.Soyadi).trim() + '</a>';
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
        protected void ArmaganAraTxt_TextChanged(object sender, EventArgs e)
        {
            ExecuteAction("ArmaganAraTxt_TextChanged", () =>
            {
                TabloOlustur();
            });
        }
        protected void ArmaganAraBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ArmaganAraBtn_Click", () =>
            {
                TabloOlustur();
            });
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl, false);
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
            });
        }

        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl, false);
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        private void TabloModalOlustur(string nakitBagisciId)
        {
            var jsonData = GetModalDataJson(nakitBagisciId); //veri çekilip json'a çevriliyor
            var jsString = CreateModalDataTable(jsonData, nakitBagisciId.ConvertToInt()); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateModalDataTable(string jsonData, int nakitBagisciId)
        {
            NakitBagisci nb = new NakitBagisciService(LoggingContext).GetById(nakitBagisciId);
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
            int rowCount = 0;
            var json = new NakitBagisHareketService(LoggingContext).GetDonorDetailJson(nakitBagisciId.ConvertToInt(), ref rowCount);
            decimal toplamTutar = new NakitBagisHareketService(LoggingContext).GetTotalByBagisciIdDateRange(
                ProjeConstants.BAGIS_SORGU_BASTAR.ConvertToDatetime(), DateTime.Today, nakitBagisciId.ConvertToInt());
            BagisBilgileriLbl.Text = rowCount < 1 ? "Bağış bulunmamaktadır" :
                "Bağışçının " + rowCount + " defada yaptığı toplam " + toplamTutar.ToString("N", culturInfo) + " TL bağışı bulunmaktadır";
            return json;
        }
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ModalDoldurBtn_Click", () =>
            {
                int donorId;
                if (!int.TryParse(paramNakitBagisciIdLbl.Value, out donorId) || donorId <= 0)
                { Messages.Warning("Detayını görmek istediğiniz bağışçıyı seçin."); return; }
                if (!CanReadDonor(donorId)) { Messages.Warning("Bu bağışçının detayını görüntüleme yetkiniz yok."); return; }
                if (new NakitBagisciService(LoggingContext).GetById(donorId) == null)
                { Messages.Warning("Bağışçı kaydı bulunamadı. Yeniden arama yapın."); return; }
                TabloModalOlustur(donorId.ToString());
                NakitBagisciFormunuDoldur(donorId.ToString());
            });
        }
        private void NakitBagisciFormunuDoldur(string nakitBagisciIdStr)
        {
            if (!string.IsNullOrEmpty(nakitBagisciIdStr))
            {
                int nakitBagisciId = nakitBagisciIdStr.ConvertToInt();

                NakitBagisci nakitBagisci = new NakitBagisciService(LoggingContext).GetById(nakitBagisciId);
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
                    Il il = new IlService().GetById(ilId, LoggingContext);
                    if (il != null)
                    {

                        IlIlceCell.Text = il.IlAdi.ReturnEmptyIfNull().ToString();
                    }
                    int ilceId = nakitBagisci.Ilcesi.ConvertToInt();
                    Ilce ilce = new IlceService().GetById(ilceId, LoggingContext);
                    if (ilce != null)
                    {

                        IlIlceCell.Text += " " + ilce.IlceAdi.ReturnEmptyIfNull().ToString();
                    }
                    TelefonCell.Text = nakitBagisci.Telefon1.ReturnEmptyIfNull().ToString();
                    TuzelKisiCell.Text = nakitBagisci.TuzelKisi.ConvertToBool() ? "Evet" : "Hayır";
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
