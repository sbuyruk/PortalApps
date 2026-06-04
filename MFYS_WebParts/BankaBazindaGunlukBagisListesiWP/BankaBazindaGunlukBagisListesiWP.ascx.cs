using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace MFYS_WebParts.BankaBazindaGunlukBagisListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BankaBazindaGunlukBagisListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BankaBazindaGunlukBagisListesiWP()
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
                            { data: 'ToplamBagis'},
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
                                title: 'Banka Bazinda Günlük Bagislar (TL)',
                                filename: function(){
                                    var d = new Date();
                                    var n = d.getTime();
                                    return 'BankaBazindaGunluk-TL-Bagislar' + n;
                                },
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'pdf',
                                title: 'Banka Bazinda Günlük Bagislar (TL)',
                                filename: function(){
                                    var d = new Date();
                                    var n = d.getTime();
                                    return 'BankaBazindaGunluk-TL-Bagislar' + n;
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
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int gunSayisi = DateTime.DaysInMonth(yil,ay);
            string banka = BankaDDL.SelectedItem.Text;
            decimal genelToplam = 0;
            List<BagisListItem> list = new List<BagisListItem>();
            for (int i = 1; i <= gunSayisi;i++)
            {
                DateTime tarih = new DateTime(yil,ay,i);
                NakitBagisHareket nakitBagisHareket = new NakitBagisHareket();
                DataTable dataTable = nakitBagisHareket.SelectTlBagisByTarihBankaGrup2(tarih, banka);
                IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
                
                string bagisTarihiStr = tarih.ConvertToDatetimeEmptyIfNull();
                decimal toplamBagis = 0;
                banka = BankaDDL.SelectedItem.Text;
                if (dataTable != null)
                {
                    DataRow row = dataTable.Rows[0];
                    bagisTarihiStr = row["BagisTarihi"].ReturnEmptyIfNull().ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                    toplamBagis = row["ToplamBagis"].ReturnZeroIfNull().ConvertToDecimal();
                    banka = row["Banka"].ToString();
                }
                else { 
                }
                BagisListItem bagisListItem = new BagisListItem();
                bagisListItem.BagisTarihi = bagisTarihiStr;
                bagisListItem.ToplamBagis = toplamBagis.ToString("N", culturInfo) + " TL";
                bagisListItem.Banka = banka;
                genelToplam += toplamBagis;
                list.Add(bagisListItem);

                ToplamLbl.Text = "Toplam   : " + genelToplam.ToString("N", culturInfo) + " TL";
            }
            return list;
        }
        private void BankaDDLDoldur()
        {

            if (BankaDDL.SelectedItem == null)
            {
                BankaDDL.Items.Clear();
                BankaTanim pBanka = new BankaTanim();
                List<string> list = pBanka.SelectByBankaGrup2();
                foreach (string banka in list)
                {
                    BankaDDL.Items.Add(new ListItem(banka, banka));
                }
                //BankaDDL.Items.Add(new ListItem("Tüm Bankalar", "0"));
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
        private class BagisListItem
        {
            public string Sirano { get; set; }
            public string BagisTarihi { get; set; }
            public string ToplamBagis { get; set; }
            public string Banka { get; set; }
        }
    }
}
