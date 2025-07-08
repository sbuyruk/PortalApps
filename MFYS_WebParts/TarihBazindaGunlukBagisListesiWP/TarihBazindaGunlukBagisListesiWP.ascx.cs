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

namespace MFYS_WebParts.TarihBazindaGunlukBagisListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TarihBazindaGunlukBagisListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TarihBazindaGunlukBagisListesiWP()
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
                BagisTarihiTxt.Text = DateTime.Today.AddDays(-1).ConvertToDatetimeEmptyIfNull();
                BankaDDLDoldur();
                DovizCinsiDDLDoldur();
                TabloOlustur(); 
            }
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
            NakitBagisHareket nbh= new NakitBagisHareket();
            DateTime bagisTarihi = BagisTarihiTxt.Text.ConvertToDatetime();
            string dovizCinsi = DovizCinsiDDL.SelectedItem.Value;
            string bankaGrup = BankaDDL.SelectedItem.Value;
            decimal tltoplam = nbh.SelectSumByBagisTarihi(bagisTarihi,bankaGrup,dovizCinsi);
            ToplamLbl.Text ="Toplam TL : " + tltoplam.ToString("N", culturInfo) + "TL (" + dovizCinsi+")";
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
                        'pageLength': 50,
                        data: " + jsonData + @",
                        columns: [
                            { data: 'HesapKodu' },
                            { data: 'HesapAdi' },
                            { data: 'FaturaNo' },
                            { data: 'Aciklama' },
                            { data: 'ParaBirimi' },
                            { data: 'ToplamBagis'},
                            { data: 'Alacak'},
                            { data: 'DovizliTutar' },
                        ],
                        columnDefs: [
                            {
                                targets: [5,7],
                                className: 'dt-body-right'
                            }
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
                                title: 'Tarih Bazında Günlük Bağışlar',
                                filename: function(){
                                    var d = new Date();
                                    var n = d.getTime();
                                    return 'TarihBazindaGunlukBagislar' + n;
                                },
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'pdf',
                                title: 'Tarih Bazında Günlük Bağışlar',
                                filename: function(){
                                    var d = new Date();
                                    var n = d.getTime();
                                    return 'TarihBazindaGunlukBagislar' + n;
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
                        });
                    });

            ";

            return tableString;
        }
        private List<BagisListItem> GetDataList()
        {
            DateTime bagisTarihi = BagisTarihiTxt.Text.ConvertToDatetime();
            string bankaGrup = BankaDDL.SelectedItem.Value;
            string dovizCinsi = DovizCinsiDDL.SelectedItem.Value;
            string dovizCinsiText = DovizCinsiDDL.SelectedItem.Text;
            NakitBagisHareket nakitBagisHareket = new NakitBagisHareket();
            DataTable dataTable = nakitBagisHareket.SelectByBagisTarihiBankaId(bagisTarihi, bankaGrup,dovizCinsi);

            List<BagisListItem> list = new List<BagisListItem>();
            
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string hesapKodu = row["HesapKodu"].ReturnEmptyIfNull().ToString();
                    string hesapAdi = row["HesapAdi"].ReturnEmptyIfNull().ToString();
                    decimal toplamBagis = row["ToplamBagis"].ReturnZeroIfNull().ConvertToDecimal();
                    //string banka = row["Banka"].ToString();

                    BagisListItem bagisListItem = new BagisListItem();
                    bagisListItem.HesapKodu = hesapKodu;
                    bagisListItem.HesapAdi = hesapAdi;
                    bagisListItem.ToplamBagis = toplamBagis.ToString("N",culturInfo);
                    bagisListItem.ParaBirimi = dovizCinsiText;
                    bagisListItem.DovizliTutar = dovizCinsiText + " "+ toplamBagis.ToString("N",culturInfo);

                    list.Add(bagisListItem);
                }
            }
            return list;
        }
        private void BankaDDLDoldur()
        {

            if (BankaDDL.SelectedItem == null)
            {
                BankaDDL.Items.Clear();
                BankaDDL.Items.Add(new ListItem("Tüm Bankalar", string.Empty));
                BankaTanim pBanka = new BankaTanim();
                List<string> list = pBanka.SelectByBankaGrup();
                foreach (string bankaGrup in list)
                {
                    BankaDDL.Items.Add(new ListItem(bankaGrup, bankaGrup));
                }
            }
        }
        private void DovizCinsiDDLDoldur()
        {
            if (DovizCinsiDDL.SelectedItem == null)
            {
                DovizCinsiDDL.Items.Clear();
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_TL, ProjeConstants.DOVIZ_TL));
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_EURO, ProjeConstants.DOVIZ_EURO));
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_USD, ProjeConstants.DOVIZ_USD));
                DovizCinsiDDL.Items.Add(new ListItem(ProjeConstants.DOVIZ_GBP, ProjeConstants.DOVIZ_GBP));
            }

        }
        protected void BagisTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void BankaDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void DovizCinsiDDLIli_SelectedIndexChanged(object sender, EventArgs e)
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
            public string HesapKodu { get; set; }
            public string HesapAdi { get; set; }
            public string FaturaNo { get; set; } = "";
            public string Aciklama { get; set; } = "";
            public string ParaBirimi { get; set; }
            public string ToplamBagis { get; set; }
            public string Alacak { get; set; } = "";
            public string DovizliTutar { get; set; }
        }
    }
}
