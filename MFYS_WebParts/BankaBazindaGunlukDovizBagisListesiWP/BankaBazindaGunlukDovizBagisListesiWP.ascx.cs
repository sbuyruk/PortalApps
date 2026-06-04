using Model.NBYS;
using Model.Ortak;
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

namespace MFYS_WebParts.BankaBazindaGunlukDovizBagisListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BankaBazindaGunlukDovizBagisListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BankaBazindaGunlukDovizBagisListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                AyDDLDoldur();
                YilDDLDoldur();
                DovizCinsiDDLDoldur();
                BankaDDLDoldur();
                TabloOlustur();
            }
        }
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
                List<BagisListItem> list = GetDataList();
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
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                jQuery(document).ready(function () {
                    jQuery('#CustomDataTable').DataTable({
                        'initComplete': function (settings, json) {//tablo yüklendiginde
                            var api = this.api();
                            var row = api.row(function (idx, data, node) { //secilen toplantiya gider
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
                            { data: 'BagisTarihi' },
                            { data: 'DovizTutari' },
                            { data: 'DovizKuru' },
                            { data: 'TLKarsiligi'},
                        ],
                        columnDefs: [
                            {
                                targets: [1,2,3],
                                className: 'dt-body-right'
                            }
                          ],
                        'language': {
                            'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        pageLength:50,
                        dom: 'Bprti',
                        buttons: [
                            {
                                extend: 'print',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'excel',
                                title: 'Banka Bazinda Günlük Bagislar (Döviz)',
                                filename: function(){
                                    var d = new Date();
                                    var n = d.getTime();
                                    return 'BankaBazindaGunluk-Doviz-Bagislar' + n;
                                },
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'pdf',
                                title: 'Banka Bazinda Günlük Bagislar (Doviz)',
                                filename: function(){
                                    var d = new Date();
                                    var n = d.getTime();
                                    return 'BankaBazindaGunluk-Doviz-Bagislar' + n;
                                },
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
//
                        rowGroup: {
                            startRender: null,
                            endRender: function ( rows, group ) {
                            var toplamTutar = rows.data().pluck(4).reduce( function (a, b) {
                                    return a + b.replace(/[^\d]/g, '')*1;
                            }, 0) / rows.count();
                salaryAvg = $.fn.dataTable.render.number(',', '.', 0, '$').display( salaryAvg );
 
                var ageAvg = rows
                    .data()
                    .pluck(3)
                    .reduce( function (a, b) {
                        return a + b*1;
                    }, 0) / rows.count();
 
                return $('<tr/>')
                    .append( '<td colspan=""3"">Averages for '+group+'</td>' )
                    .append( '<td>'+ageAvg.toFixed(0)+'</td>' )
                    .append( '<td/>' )
                    .append( '<td>'+salaryAvg+'</td>' );
            },
            dataSrc: 2
        }
//
                        });
                    });

            ";

            return tableString;
        }
        private List<BagisListItem> GetDataList()
        {
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            List<BagisListItem> list = new List<BagisListItem>();
            DateTime bastar= new DateTime(yil, ay, 1);
            DateTime bittar= bastar.AddMonths(1).AddDays(-1);
            string secilenBanka = BankaDDL.SelectedItem!=null?BankaDDL.SelectedItem.Text:string.Empty;
            string dovizCinsi = DovizCinsiDDL.SelectedItem.Text;

            NakitBagisHareket nakitBagisHareket = new NakitBagisHareket();
            DataTable dataTable = nakitBagisHareket.SelectDovizBagisByTarihBankaGrup2(bastar,bittar, secilenBanka, dovizCinsi);
            if (dataTable != null)
            {
                decimal toplamTL = 0;
                decimal toplamDoviz = 0;
                foreach (DataRow row in dataTable.Rows)
                {
                    BagisListItem bagisListItem = new BagisListItem();

                    string bagisTarihiStr = row["BagisTarihi"].ReturnEmptyIfNull().ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                    decimal dovizTutari = row["DovizTutari"].ReturnEmptyIfNull().ConvertToDecimal();
                    decimal dovizKuru = row["DovizKuru"].ReturnEmptyIfNull().ConvertToDecimal();
                    decimal tlKarsiligi = row["BagisMiktari"].ReturnZeroIfNull().ConvertToDecimal();
                    string banka = row["Banka"].ToString();
                    bagisListItem.BagisTarihi= bagisTarihiStr;
                    bagisListItem.DovizTutari= dovizTutari.ToString("N",culturInfo) + " " + dovizCinsi;
                    bagisListItem.DovizKuru= dovizKuru.ToString("N4",culturInfo) + " " + ProjeConstants.DOVIZ_TL;
                    bagisListItem.TLKarsiligi= tlKarsiligi.ToString("N",culturInfo) + " " + ProjeConstants.DOVIZ_TL;
                    bagisListItem.Banka= banka;
                    toplamTL += tlKarsiligi;
                    toplamDoviz += dovizTutari;
                    list.Add(bagisListItem);
                }
                ToplamLbl.Text="Toplam = " +toplamDoviz.ToString("N",culturInfo) +" " + dovizCinsi + " (" +toplamTL.ToString("N",culturInfo) + " " +ProjeConstants.DOVIZ_TL+") ";
            }
            return list;
        }
        private void BankaDDLDoldur()
        {
            var selected = DovizCinsiDDL.SelectedItem != null ? DovizCinsiDDL.SelectedItem.Value : string.Empty;
            BankaDDL.Items.Clear();

            int yil = YilDDL.SelectedItem.Value.ConvertToInt();
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            string dovizCinsi = DovizCinsiDDL.SelectedItem != null ? DovizCinsiDDL.SelectedItem.Text : string.Empty;
            DateTime bastar = new DateTime(yil, ay, 1);
            DateTime bittar = bastar.AddMonths(1).AddDays(-1);

            NakitBagisHareket nakitBagisHareket = new NakitBagisHareket();
            List<string> bankaGrup2List = nakitBagisHareket.SelectBankaGrup2ByTarihDovizCinsi(bastar, bittar, dovizCinsi);
            foreach (var item in bankaGrup2List)
            {
                BankaDDL.Items.Add(new ListItem(item, item));
            }
            UtilityHelper.SetDDLValue(BankaDDL, selected);

            //if (BankaDDL.SelectedItem == null)
            //{
            //    BankaDDL.Items.Clear();
            //    BankaTanim pBanka = new BankaTanim();
            //    List<string> list = pBanka.SelectByBankaGrup2();
            //    foreach (string banka in list)
            //    {
            //        BankaDDL.Items.Add(new ListItem(banka, banka));
            //    }
            //    //BankaDDL.Items.Add(new ListItem("Tüm Bankalar", "0"));

            //}
        }
        private void DovizCinsiDDLDoldur()
        {
            if (DovizCinsiDDL.SelectedItem == null)
            {
                DovizCinsiDDL.Items.Clear();
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_EURO, ProjeConstants.DOVIZ_EURO));
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_USD, ProjeConstants.DOVIZ_USD));
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_GBP, ProjeConstants.DOVIZ_GBP));
            }
            
        }
        private void AyDDLDoldur()
        {
            AyDDL.Items.Clear();
            DateTime bugun = DateTime.Today;

            for (int i = 0; i < 12; i++)
            {
                DateTime tarih = bugun.AddMonths(-i);
                ListItem li = new ListItem(tarih.ToString("MMMM"), tarih.ToString("MM"));
                AyDDL.Items.Add(li);
            }
            AyDDL.Items.Add(new ListItem("Tüm Aylar", "0"));
        }
        private void YilDDLDoldur()
        {
            YilDDL.Items.Clear();
            DateTime bugun = DateTime.Today;

            for (int i = 0; i < 2; i++)
            {
                DateTime tarih = bugun.AddYears(-i);
                ListItem li = new ListItem(tarih.Year.ToString(), tarih.Year.ToString());
                YilDDL.Items.Add(li);
            }
        }
        protected void BagisTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            BankaDDLDoldur();
            TabloOlustur();
        }
        protected void DovizCinsiDDLIli_SelectedIndexChanged(object sender, EventArgs e)
        {
            BankaDDLDoldur();
            TabloOlustur();
        }
        protected void BankaDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            BankaDDLDoldur();
            TabloOlustur();
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            BankaDDLDoldur();
            TabloOlustur();
        }
        protected void YenileBtn_Click(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        private class BagisListItem
        {
            public string Sirano { get; set; }
            public string BagisTarihi { get; set; }
            public string DovizTutari { get; set; }
            public string DovizKuru { get; set; }
            public string KurTarihi { get; set; }
            public string TLKarsiligi { get; set; }
            public string Banka { get; set; }
        }
    }
}
