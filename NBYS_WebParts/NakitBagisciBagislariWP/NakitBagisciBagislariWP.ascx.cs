using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.NakitBagisciBagislariWP
{
    [ToolboxItemAttribute(false)]
    public partial class NakitBagisciBagislariWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NakitBagisciBagislariWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string ParamQS//nakit bagisci düzenlemeden dönüyorsa aranan texti tekrar arasın
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
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty(ParamQS))
                    {
                        BagisciAraTxt.Text = ParamQS;

                    }
                }
                //TabloOlustur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
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

        #region Bagisci listesi CustomDataTable
        private void TabloOlustur()
        {
            if (!string.IsNullOrEmpty(BagisciAraTxt.Text) && BagisciAraTxt.Text.Length > 3)
            {
                var jsonData = BagisciTabloJson(); //veri çekilip json a çeviriliyor
                var jsString = BagisciCreateDataTable(jsonData); //javascript kodu hazırlanıyor.
                UtilityHelper.ScriptCalistir(jsString);
                BagisciSecTableDiv.Attributes["style"] = "display:block";

            }
            BagisBilgileriLbl.Text = string.Empty;
            BagisciAdiLbl.Text = string.Empty;
            BagisTableDiv.Attributes["style"] = "display:none";
        }
        private string BagisciTabloJson()
        {
            string json = string.Empty;

            try
            {
                List<NakitBagisciListItem> list = BagisciGetDataList();
                var serializer = new JavaScriptSerializer();
                json = list.Count > 0 ? serializer.Serialize(list) : string.Empty;
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return json;
        }
        private List<NakitBagisciListItem> BagisciGetDataList()
        {

            DataTable dataTable = BagisciGetData();
            List<NakitBagisciListItem> list = new List<NakitBagisciListItem>();
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    int nakitBagisciId = row["Id"].ReturnZeroIfNull().ConvertToInt();
                    string adi = "<a href=# onclick=BagisListesiGoster(" + nakitBagisciId + "); class='text-link'>" + row["Adi"].ToString() + "</a>";
                    string TcKimlik = row["TcKimlikNo"].ToString();
                    string ili = row["Ili"].ToString();
                    string ilcesi = row["Ilcesi"].ToString();
                    string telefon = row["Telefon1"].ToString();
                    string adres = row["Adres"].ToString();

                    string secUrl = "<a class='btn btn-outline-info' onclick=BagisListesiGoster(" + nakitBagisciId + ");>Seç</>";

                    NakitBagisciListItem nakitBagisciListItem = new NakitBagisciListItem();
                    nakitBagisciListItem.NakitBagisciId = nakitBagisciId;
                    nakitBagisciListItem.Adi = adi.Trim();
                    nakitBagisciListItem.TCKimlikNo = TcKimlik;
                    nakitBagisciListItem.Ili = ili;
                    nakitBagisciListItem.Ilcesi = ilcesi;
                    nakitBagisciListItem.Telefon = telefon;
                    nakitBagisciListItem.Adres = adres;


                    nakitBagisciListItem.Sec = adi.IndexOf("BİLİNMEYEN") >= 0 ? string.Empty : secUrl;

                    nakitBagisciListItem.Secildi = SecilenIdQS.Equals(nakitBagisciListItem.NakitBagisciId);
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
                dataTable = nakitBagisci.SelectByAd(BagisciAraTxt.Text);
            }
            return dataTable;
        }
        private string BagisciCreateDataTable(string jsonData)
        {
            string jsonDataStr = string.IsNullOrEmpty(jsonData) ? string.Empty : "data: " + jsonData + ",";
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();
                jQuery(document).ready(function() {

                    jQuery('#CustomDataTable').DataTable({
                        'initComplete': function(settings, json) {//tablo yüklendiğinde
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
                " + jsonDataStr + @"
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
                'order': [0],//AdiSoyadi Sıralı
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
                pageLength : 5,
                dom: 'frtip',
            
                    });
                });";
            return tableString;
        }
        #endregion
        #region Bagis listesi
        private void BagisTablosuOlustur(string nakitBagisciId)
        {
            var jsonData = BagisDataJson(nakitBagisciId); //veri çekilip json a çeviriliyor
            var jsString = CreateBagisDataTable(jsonData, nakitBagisciId.ConvertToInt()); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
            BagisciSecTableDiv.Attributes["style"] = "display:none";
            BagisTableDiv.Attributes["style"] = "display:block";
        }
        private string CreateBagisDataTable(string jsonData, int nakitBagisciId)
        {
            NakitBagisci nb = new NakitBagisci();
            nb = nb.Select<NakitBagisci>(nakitBagisciId);

            string bagisciIdi = nb == null ? string.Empty : nb.Adi + " " + nb.Soyadi + " ";
            BagisciAdiLbl.Text = bagisciIdi;
            string tableString = @"
        jQuery(document).ready(function () {
            if ( jQuery.fn.DataTable.isDataTable('#BagisDataTable') ) {
                jQuery('#BagisDataTable').DataTable().destroy();
            }
            jQuery('#BagisDataTable tbody').empty();

            jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
            jQuery('#BagisDataTable').DataTable({
                data: " + jsonData + @",
                columns: [
                    { data: 'BagisTarihi' },
                    { data: 'BagisMiktari', 'width': '10%', 'className': 'text-right' },
                    { data: 'Banka' },
                    { data: 'Armagan' },
                    { data: 'ArmaganTutari', 'width': '10%', 'className': 'text-right'},
                    { data: 'Durum' },
                ],
                'order': [[0, 'desc']],

                'language': {
                    'url': 'http://tskgv-portal/OrtakBelgeler/Turkish.txt',
                    'decimal': ',',
                    'thousands': '.'
                },
                columnDefs:[
                    {targets:0, render:function(data){
                        return moment(data).format('DD.MM.YYYY');
                    }},
                ],
                responsive: true,
                dom: 'frtip',
             });
        });
        ";
            return tableString;
        }
        private string BagisDataJson(string nakitBagisciId)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            NakitBagisHareket nbh = new NakitBagisHareket();
            int rowCount = 0;
            var json = nbh.SelectByBagisciIdReturnJSon(nakitBagisciId, ref rowCount);

            decimal toplamTutar = nbh.GetSumBagisMiktariByNakitBagisciIdBetweenBasTarBitTar(
                ProjeConstants.BAGIS_SORGU_BASTAR.ConvertToDatetime(), DateTime.Today, nakitBagisciId.ConvertToInt());



            BagisBilgileriLbl.Text = rowCount < 1 ? "Bağış bulunmamaktadır." :
                "(Bağışçının " + rowCount + " defada yaptığı toplam " + toplamTutar.ToString("N", culturInfo) + "TL bağışı bulunmaktadır.)";
            return json;
        }
        protected void BagisListesiGosterBtn_Click(object sender, EventArgs e)
        {
            try
            {
                int nakitBagisciId = paramNakitBagisciIdLbl.Value.ConvertToInt();
                if (nakitBagisciId > 0)
                {
                    SecilenIdQS = nakitBagisciId.ToString();

                    BagisTablosuOlustur(paramNakitBagisciIdLbl.Value);
                }

            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }
        }
        #endregion
        #region class
        private class NakitBagisciListItem
        {
            public int NakitBagisciId { get; set; }
            public string Adi { get; set; }
            public string TCKimlikNo { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Telefon { get; set; }
            public string Adres { get; set; }
            public string Duzenle { get; set; }
            public string Sec { get; set; }
            public bool Secildi { get; set; }
        }
        #endregion
    }
}
