using Model.NBYS;
using NBYS_WebParts.Logging;
using Utility.Logging;
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

namespace NBYS_WebParts.DovizleYapilanBagislarWP
{
    [ToolboxItemAttribute(false)]
    public partial class DovizleYapilanBagislarWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "DovizleYapilanBagislarWP"); }
        }

        private void ExecuteAction(string operation, Action action)
        {
            try { action(); }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                NbysWebPartLog.ReportError(LoggingContext, "NBYS.DovizleYapilanBagislarWP." + operation, error,
                    "Bağış raporu görüntülenemedi. Lütfen tekrar deneyin.");
            }
        }

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
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
                if (!Page.IsPostBack)
                {
                    BankaDDLDoldur();
                    AyDDLDoldur();
                    YilDDLDoldur();
                    TabloOlustur();
                }
            });
        }
        private void TabloOlustur()
        {
            int year;
            int month;
            if (YilDDL.SelectedItem == null || AyDDL.SelectedItem == null || BankaDDL.SelectedItem == null ||
                !int.TryParse(YilDDL.SelectedValue, out year) || year < 1901 || year > 9998 ||
                !int.TryParse(AyDDL.SelectedValue, out month) || month < 0 || month > 12)
            {
                Messages.Warning("Raporu görüntülemek için geçerli bir yıl, ay ve banka seçin.");
                return;
            }
            var jsonData = TabloJson(); //veri çekilip json'a çevriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
            if (jsonData == "[]") Messages.Info("Seçilen filtrelere uygun döviz bağışı bulunamadı.");
        }
        private string TabloJson()
        {
            List<DovizleBagisListItem> list = GetDataList();
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(list);
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
                            { data: 'Banka' },
                            { data: 'DovizCinsi' },
                            { data: 'DovizTutari' },
                            { data: 'TlKarsiligiToplamBagis'},
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

            int ay = AyDDL.SelectedItem.Value.ConvertToInt() < 1 ? 1 : AyDDL.SelectedItem.Value.ConvertToInt();
            DateTime bastar = new DateTime(YilDDL.SelectedItem.Value.ConvertToInt(), ay, 1);
            DateTime bitisHaric = AyDDL.SelectedItem.Value.ConvertToInt() < 1
                ? bastar.AddYears(1) : bastar.AddMonths(1);
            string bankaGrup = BankaDDL.SelectedItem.Value;
            NakitBagisHareketService nakitBagisHareket = new NakitBagisHareketService();
            DataTable dataTable = nakitBagisHareket.GetCurrencyTotalsByDateBank(bastar, bitisHaric, bankaGrup, LoggingContext, endExclusive: true);

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
                    bagisListItem.DovizTutari = dovizTutari.ToString("N", culturInfo) + " " + dovizCinsi;
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
                List<string> list = new BankaTanimService().GetGroups(LoggingContext);
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
            ExecuteAction("BagisTarihiTxt_TextChanged", () =>
            {
                TabloOlustur();
            });
        }
        protected void BankaDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("BankaDDL_SelectedIndexChanged", () =>
            {
                TabloOlustur();
            });
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("AyDDL_SelectedIndexChanged", () =>
            {
                TabloOlustur();
            });
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("YilDDL_SelectedIndexChanged", () =>
            {
                TabloOlustur();
            });
        }
        protected void YenileBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("YenileBtn_Click", () =>
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
                Page.Response.Redirect(newUrl);
            });
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
