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

namespace NBYS_WebParts.DovizleYapilanBagislarWP
{
    [ToolboxItemAttribute(false)]
    public partial class DovizleYapilanBagislarWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public DovizleYapilanBagislarWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BankaDDLDoldur();
                AyDDLDoldur();
                YilDDLDoldur();
                TabloOlustur();
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
                List<DovizleBagisListItem> list = GetDataList();
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
                        data: " + jsonData + @",
                        columns: [
                            { data: 'Banka' },
                            { data: 'DovizCinsi' },
                            { data: 'DovizTutari' },
                            { data: 'TlKarsiligiToplamBagis'},
                        ],
                        columnDefs: [
                            {
                                targets: 1,
                                className: 'dt-body-right'
                            }
                          ],
                        'language': {
                            'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        pageLength:31,
                        dom: 'Brti',
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
                                    return 'BankaBazindaGunlukBagislar' + n;
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
                                    return 'BankaBazindaGunlukBagislar' + n;
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
        private List<DovizleBagisListItem> GetDataList()
        {
            DateTime bastar = new DateTime(YilDDL.SelectedItem.Value.ConvertToInt(), AyDDL.SelectedItem.Value.ConvertToInt(), 1);
            DateTime bittar = bastar.AddMonths(1).AddDays(-1);
            string bankaGrup = BankaDDL.SelectedItem.Value;
            NakitBagisHareket nakitBagisHareket = new NakitBagisHareket();
            DataTable dataTable = nakitBagisHareket.SelectDovizleBagisByTarihBankaId(bastar, bittar, bankaGrup);

            List<DovizleBagisListItem> list = new List<DovizleBagisListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string banka = row["Banka"].ToString();
                    string dovizCinsi = row["DovizCinsi"].ReturnEmptyIfNull().ToString();
                    decimal tlKarsiligiToplamBagis = row["TlKarsiligiToplamBagis"].ReturnZeroIfNull().ConvertToDecimal();
                    decimal dovizTutari = row["ToplamDovizTutari"].ReturnZeroIfNull().ConvertToDecimal();

                    DovizleBagisListItem bagisListItem = new DovizleBagisListItem();
                    bagisListItem.Banka = banka;
                    bagisListItem.DovizCinsi = dovizCinsi;
                    bagisListItem.DovizTutari = dovizTutari.ToString("N", culturInfo) + " TL";
                    bagisListItem.TlKarsiligiToplamBagis = tlKarsiligiToplamBagis.ToString("N", culturInfo) + " TL";
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
                BankaDDL.Items.Add(new ListItem("Tüm Bankalar", "0"));
                BankaTanim pBanka = new BankaTanim();
                List<string> list = pBanka.SelectByBankaGrup();
                foreach (string banka in list)
                {
                    BankaDDL.Items.Add(new ListItem(banka, banka));
                }
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
            TabloOlustur();
        }
        protected void BankaDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
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
        private class DovizleBagisListItem
        {
            public string Banka { get; set; }
            public string DovizCinsi { get; set; }
            public string DovizTutari { get; set; }
            public string TlKarsiligiToplamBagis { get; set; }
        }
    }
}
