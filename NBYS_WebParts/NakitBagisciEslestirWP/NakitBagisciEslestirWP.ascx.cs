using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.NakitBagisciEslestirWP
{
    [ToolboxItemAttribute(false)]
    public partial class NakitBagisciEslestirWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NakitBagisciEslestirWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string EkstreAktarmaIdQS
        {
            get
            {
                if (ViewState["EkstreAktarmaId"] == null)
                {
                    if (Page.Request.QueryString["EkstreAktarmaId"] != null)
                    {
                        ViewState["EkstreAktarmaId"] = Page.Request.QueryString["EkstreAktarmaId"];
                    }
                    else
                    {
                        ViewState["EkstreAktarmaId"] = string.Empty;
                    }
                }
                return ViewState["EkstreAktarmaId"].ToString();
            }

            set
            {
                ViewState["EkstreAktarmaId"] = value;
            }
        }
        private string IslemTarihiQS
        {
            get
            {

                if (ViewState["IslemTarihi"] == null)
                {
                    if (Page.Request.QueryString["IslemTarihi"] != null)
                    {
                        ViewState["IslemTarihi"] = Page.Request.QueryString["IslemTarihi"];
                    }
                    else
                    {
                        ViewState["IslemTarihi"] = string.Empty;
                    }
                }
                return ViewState["IslemTarihi"].ToString();
            }

            set
            {
                ViewState["IslemTarihi"] = value;
            }
        }
        private string BankaQS
        {
            get
            {
                if (ViewState["Banka"] == null)
                {
                    if (Page.Request.QueryString["Banka"] != null)
                    {
                        ViewState["Banka"] = Page.Request.QueryString["Banka"];
                    }
                    else
                    {
                        ViewState["Banka"] = string.Empty;
                    }
                }
                return ViewState["Banka"].ToString();
            }

            set
            {
                ViewState["Banka"] = value;
            }
        }
        private string SenderAppQS
        {
            get
            {

                if (ViewState["SenderApp"] == null)
                {
                    if (Page.Request.QueryString["SenderApp"] != null)
                    {
                        ViewState["SenderApp"] = Page.Request.QueryString["SenderApp"];
                    }
                    else
                    {
                        ViewState["SenderApp"] = string.Empty;
                    }
                }
                return ViewState["SenderApp"].ToString();
            }

            set
            {
                ViewState["SenderApp"] = value;
            }
        }
        private string BagisciAdiQS
        {
            get
            {

                if (ViewState["BagisciAdi"] == null)
                {
                    if (Page.Request.QueryString["BagisciAdi"] != null)
                    {
                        ViewState["BagisciAdi"] = Page.Request.QueryString["BagisciAdi"];
                    }
                    else
                    {
                        ViewState["BagisciAdi"] = string.Empty;
                    }
                }
                return ViewState["BagisciAdi"].ToString();
            }

            set
            {
                ViewState["BagisciAdi"] = value;
            }
        }
        private string DuzenliBagisciIdQS
        {
            get
            {

                if (ViewState["DuzenliBagisciId"] == null)
                {
                    if (Page.Request.QueryString["DuzenliBagisciId"] != null)
                    {
                        ViewState["DuzenliBagisciId"] = Page.Request.QueryString["DuzenliBagisciId"];
                    }
                    else
                    {
                        ViewState["DuzenliBagisciId"] = string.Empty;
                    }
                }
                return ViewState["DuzenliBagisciId"].ToString();
            }

            set
            {
                ViewState["DuzenliBagisciId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty(EkstreAktarmaIdQS))
                    {
                        EkstreAktarma ekstreAktarma = new EkstreAktarma();
                        ekstreAktarma = ekstreAktarma.Select<EkstreAktarma>(EkstreAktarmaIdQS.ConvertToInt());
                        BagisciAraTxt.Text = ekstreAktarma != null ? ekstreAktarma.Adi : "";

                    }
                    else if (!string.IsNullOrEmpty(BagisciAdiQS))
                    {
                        BagisciAraTxt.Text = BagisciAdiQS.Replace("@@"," ");
                    }
                    else
                    {
                        BagisciAraTxt.Text = "";
                    }
                }
                TabloOlustur(true);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void BagisciAraTxt_TextChanged(object sender, EventArgs e)
        {
            TabloOlustur(true);
        }
        protected void BagisciAraBtn_Click(object sender, EventArgs e)
        {
            TabloOlustur(true);
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
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                EkstreAktarma ekstreAktarma = new EkstreAktarma();
                ekstreAktarma = ekstreAktarma.Select<EkstreAktarma>(EkstreAktarmaIdQS.ConvertToInt());
                if ((ekstreAktarma != null) && (SenderAppQS.Equals("EkstreListesi")))//ekstrelistesinden'dan geldiyse
                {
                    newUrl += "/" + ProjeConstants.PAGE_EKSTRE_LIST + "?EkstreAktarmaId=" + EkstreAktarmaIdQS + "&IslemTarihi=" + ekstreAktarma.IslemTarihi.ConvertToDatetimeEmptyIfNull() + "&Banka=" + BankaQS;
                }
                else if ((ekstreAktarma != null) && (SenderAppQS.Equals("EAE")))//ekstreaktarmaedit ten geldiyse
                {
                    newUrl += "/" + ProjeConstants.PAGE_EKSTRE_AKTARMAEDIT + "?EkstreAktarmaId=" + EkstreAktarmaIdQS + "&IslemTarihi=" + ekstreAktarma.IslemTarihi.ConvertToDatetimeEmptyIfNull() + "&Banka=" + BankaQS;
                }
                else
                {
                    newUrl += "/" + ProjeConstants.PAGE_EKSTRE_LIST;
                }
                Page.Response.Redirect(newUrl, true);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void EkstreListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_EKSTRE_LIST + "?IslemTarihi=" + IslemTarihiQS + "&EkstreAktarmaId =" + EkstreAktarmaIdQS + "&Banka=" + BankaQS);
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
        #region Bagisci CustomDataTable
        private void TabloOlustur(bool isSelectable)
        {
            var jsonData = TabloJson(isSelectable); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
            BagisciSecTableDiv.Attributes["style"] = "display:block";
        }
        private string TabloJson(bool isSelectable)
        {
            string json = string.Empty;

            try
            {
                List<NakitBagisciListItem> list = GetDataList(isSelectable);
                var serializer = new JavaScriptSerializer();
                json = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return string.IsNullOrEmpty(json) ? "[{}]" : json;
        }
        private List<NakitBagisciListItem> GetDataList(bool isSelectable)
        {
            string spaceStr = HttpUtility.UrlEncode(BagisciAraTxt.Text.ToString());
            DataTable dataTable = BagisciGetData();
            List<NakitBagisciListItem> list = new List<NakitBagisciListItem>();
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    
                    int nakitBagisciId = row["NakitBagisciId"].ReturnZeroIfNull().ConvertToInt();
                    string adi = "<a href=# onclick=OpenModal(" + nakitBagisciId + "); class='text-link'>" + row["Adi"].ToString() + "</a>";
                    string TcKimlik = row["TcKimlikNo"].ToString();
                    string ili = row["Ili"].ToString();
                    string ilcesi = row["Ilcesi"].ToString();
                    string telefon = row["Telefon1"].ToString();
                    string adres = row["Adres"].ToString();
                    string secUrl = !string.IsNullOrEmpty(EkstreAktarmaIdQS) ?"<a href=EkstreAktarmaEdit.aspx?SenderApp=NBE&EkstreAktarmaId=" + EkstreAktarmaIdQS + "&NakitBagisciId=" + nakitBagisciId + "&Param=" + spaceStr + "class='btn btn-outline-success'>Seç</a>"
                        : "<a href=DuzenliNakitBagisciListesi.aspx?SenderApp=NBE&NakitBagisciId=" + nakitBagisciId + "&DuzenliBagisciId=" + DuzenliBagisciIdQS + "&Param=" + spaceStr + "class='btn btn-outline-success'>Seç</a>";


                    NakitBagisciListItem nakitBagisciListItem = new NakitBagisciListItem();
                    nakitBagisciListItem.NakitBagisciId = nakitBagisciId;
                    nakitBagisciListItem.Adi = adi.Trim();
                    nakitBagisciListItem.TCKimlikNo = TcKimlik;
                    nakitBagisciListItem.Ili = ili;
                    nakitBagisciListItem.Ilcesi = ilcesi;
                    nakitBagisciListItem.Telefon = telefon;
                    nakitBagisciListItem.Adres = adres;

                    if (isSelectable)
                        nakitBagisciListItem.Sec = adi.IndexOf("BILINMEYEN") >= 0 ? string.Empty : secUrl;

                    list.Add(nakitBagisciListItem);
                }
            }
            return list;
        }
        private DataTable BagisciGetData()
        {
            DataTable dataTable = null;
            if (!string.IsNullOrEmpty(BagisciAraTxt.Text) && BagisciAraTxt.Text.Length > 3)
            {
                NakitBagisci nakitBagisci = new NakitBagisci();
                dataTable = nakitBagisci.SelectByFilterReturnDataTable(BagisciAraTxt.Text, 0);
            }
            return dataTable;
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
            jQuery(document).ready(function() {

                jQuery('#CustomDataTable').DataTable({
                    'initComplete': function(settings, json) {//tablo yüklendiginde
                        var api = this.api();
                        var row = api.row(function(idx, data, node) { //secilen Id'ye gider
                            return data['Secildi'] == true;
                        });
                        if (row.length > 0)
                        {
                            row.select()
                                .show()
                                .draw(false);
                        }
                    },
            data: " + jsonData + @",
            columns:
                    [
                { data: 'Adi' },
                { data: 'TCKimlikNo' },
                { data: 'Ili' },
                { data: 'Ilcesi' },
                { data: 'Telefon', 'width': '14%' },
                { data: 'Adres' },
                { data: 'Sec' },

            ],
            'order': [[0, 'asc']],//AdiSoyadi Sirali
            columnDefs:
                [
                ],
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                'decimal': ',',
                'thousands': '.'
            },
            responsive: true,
            destroy: true,
            autoWidth: false,
            dom: 'frtip',
            
                });
            });";
            return tableString;
        }
        #endregion
        #region modal
        private void TabloModalOlustur(string nakitBagisciId)
        {
            var jsonData = GetModalDataJson(nakitBagisciId); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData, nakitBagisciId.ConvertToInt()); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateModalDataTable(string jsonData, int nakitBagisciId)
        {
            NakitBagisci nb = new NakitBagisci();
            nb = nb.Select<NakitBagisci>(nakitBagisciId);
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
            BagisBilgileriLbl.Text = rowCount < 1 ? "Bagis bulunmamaktadir" :
                "Bagisçinin " + rowCount + " defada yaptigi toplam " + toplamTutar.ToString("N", culturInfo) + "TL bagisi bulunmaktadir";
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

                NakitBagisci nakitBagisci = new NakitBagisci();
                nakitBagisci = nakitBagisci.Select<NakitBagisci>(nakitBagisciId);
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
        #endregion
        private class NakitBagisciListItem
        {
            public int NakitBagisciId { get; set; }
            public string Adi { get; set; }
            public string TCKimlikNo { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Telefon { get; set; }
            public string Adres { get; set; }
            public string Sec { get; set; }
        }
    }
}

