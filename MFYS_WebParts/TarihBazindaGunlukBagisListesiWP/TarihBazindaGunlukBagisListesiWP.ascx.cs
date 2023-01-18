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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BagisTarihiTxt.Text = DateTime.Today.AddDays(-1).ConvertToDatetimeEmptyIfNull();
                BankaDDLDoldur();
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
                        data: " + jsonData + @",
                        columns: [
                            { data: 'Banka' },
                            { data: 'ToplamBagis'},
                            { data: 'BagisTarihi' },
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
            int bankaId = BankaDDL.SelectedItem.Value.ConvertToInt();
            NakitBagisHareket nakitBagisHareket = new NakitBagisHareket();
            DataTable dataTable = nakitBagisHareket.SelectByBagisTarihiBankaId(bagisTarihi, bankaId);

            List<BagisListItem> list = new List<BagisListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            if (dataTable != null)
            {
                decimal vakifToplam = 0;
                foreach (DataRow row in dataTable.Rows)
                {
                    string bagisTarihiStr = row["BagisTarihi"].ReturnEmptyIfNull().ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                    decimal toplamBagis = row["ToplamBagis"].ReturnZeroIfNull().ConvertToDecimal();
                    string banka = row["Banka"].ToString();

                    BagisListItem bagisListItem = new BagisListItem();
                    bagisListItem.BagisTarihi = bagisTarihiStr;
                    bagisListItem.ToplamBagis = toplamBagis.ToString("N",culturInfo) + " TL";
                    bagisListItem.Banka = banka;
                    if (banka.Equals(ProjeConstants.BANKA_VAKIF))
                    {
                        vakifToplam += toplamBagis;
                    }
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
                List<BankaTanim> list = pBanka.SelectAll<BankaTanim>();
                foreach (BankaTanim banka in list)
                {
                    BankaDDL.Items.Add(new ListItem(banka.Banka, banka.Id.ToString()));
                }
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
            public string ToplamBagis { get; set; }
            public string Banka { get; set; }
        }
    }
}
