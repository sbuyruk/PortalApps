using Portal_WebParts.Logging;
using Utility.Logging;
using Microsoft.SharePoint;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.DavaDurusmaListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class DavaDurusmaListesiWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("Portal", "DavaDurusmaListesiWP")); } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public DavaDurusmaListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.ChromeType = PartChromeType.None;
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) PortalWebPartLog.RecordOpened(LogContext);
                if (!Page.IsPostBack)
                {
                    DavaDurumuDDLDoldur();
                    TabloOlustur();
                }
            });
        }
        private void TabloOlustur()
        {
            try
            {
                var jsonData = TabloJson();
                UtilityHelper.ScriptCalistir("jQuery('#CustomDataTable').show();" + CreateDataTable(jsonData));
            }
            catch
            {
                // Hide stale results; the outer boundary reports the read failure once.
                UtilityHelper.ScriptCalistir("jQuery('#CustomDataTable').hide();");
                throw;
            }
        }
        private string TabloJson()
        {
            var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            return serializer.Serialize(GetDataList()).Replace("<", "\\u003c").Replace(">", "\\u003e").Replace("&", "\\u0026");
        }
        private List<DavaDurusmaListItem> GetDataList()
        {
            string status = DavaDurumuDDL.SelectedValue;
            if (status != ProjeConstants.HUKUK_DAVA_DURUMU_DEVAM && status != ProjeConstants.HUKUK_DAVA_DURUMU_KARARACIKTI
                && status != ProjeConstants.HUKUK_DAVA_DURUMU_ARSIV) throw new PortalInputException("Geçerli bir dava durumu seçin.");
            List<DavaDurusmaListItem> davaDurusmaList = new List<DavaDurusmaListItem>();

            using (SPSite site = new SPSite(UtilityHelper.HukukURLGetir()))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    if (web != null)
                    {
                        SPList list = web.Lists["Dava Duruşma Listesi"];

                        SPQuery query = new SPQuery();
                        query.Query = string.Format(@"

                                <Where>
                                <Eq>
                                    <FieldRef Name='Sonu_x00e7_' />
                                    <Value Type='Text'>" + System.Security.SecurityElement.Escape(status) + @"</Value>
                                </Eq>
                                </Where>
                                <OrderBy>
                                <FieldRef Name='Created' Ascending='False' />
                                </OrderBy>
                            ");
                        SPListItemCollection davalar = list.GetItems(query);
                        DataTable dataTable = davalar.GetDataTable();
                        if (dataTable != null)
                        {
                            foreach (DataRow datarow in dataTable.Rows)
                            {
                                string sira = Convert.ToString(datarow["S_x0131_ra_x0020_No"]);
                                string title = Convert.ToString(datarow["Title"]);
                                string gorevliPersonel = Convert.ToString(datarow["G_x00f6_revli_x0020_Personel"]);
                                string durusmaIli = Convert.ToString(datarow["Dava_x002f_Duru_x015f_ma_x0020_M"]);
                                string mahkeme = Convert.ToString(datarow["Mahkeme"]);
                                DateTime tarihSaat = datarow["Tarih_x0020_ve_x0020_Saati"].ConvertToDatetime();
                                string karsiTaraf = Convert.ToString(datarow["Kar_x015f__x0131__x0020_Taraf"]);
                                string esasNo = Convert.ToString(datarow["Esas_x0020_No"]);
                                string konusu = Convert.ToString(datarow["Konusu"]);
                                string sonuc = Convert.ToString(datarow["Sonu_x00e7_"]);
                                string aciklama = Convert.ToString(datarow["A_x00e7__x0131_klama"]);
                                DavaDurusmaListItem dava = new DavaDurusmaListItem();
                                dava.SiraNo = sira;
                                dava.GorevliPersonel = gorevliPersonel;
                                dava.DurusmaIli = durusmaIli;
                                dava.Mahkeme = mahkeme;
                                dava.TarihSaat = tarihSaat.ConvertToDDMMYYYHHmmFormat();
                                dava.KarsiTaraf = karsiTaraf;
                                dava.EsasNo = esasNo;
                                dava.DavaTuru = konusu;
                                dava.Sonuc = sonuc;
                                dava.Aciklama = aciklama;
                                davaDurusmaList.Add(dava);
                            }
                        }
                    }
                }
            }

            return davaDurusmaList;
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
                            { data: 'SiraNo' },
                            { data: 'DurusmaIli'},
                            { data: 'EsasNo'},
                            { data: 'DavaTuru'},
                            { data: 'Mahkeme'},
                            { data: 'KarsiTaraf'},
                            { data: 'TarihSaat'},
                            { data: 'Sonuc'},
                            { data: 'Aciklama'},
                        ],
                        columnDefs: [{ targets: '_all', render: jQuery.fn.dataTable.render.text() }],
                        'order': [[6, 'desc']],//sort
                        'language': {
                            'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        destroy: true,
                        pageLength:10,
                        dom: 'ftipr',

                        });
                    });

            ";

            return tableString;
        }
        private void DavaDurumuDDLDoldur()
        {
            DavaDurumuDDL.Items.Clear();

            ListItem li0 = new ListItem(ProjeConstants.HUKUK_DAVA_DURUMU_DEVAM);
            ListItem li1 = new ListItem(ProjeConstants.HUKUK_DAVA_DURUMU_KARARACIKTI);
            ListItem li2 = new ListItem(ProjeConstants.HUKUK_DAVA_DURUMU_ARSIV);
            DavaDurumuDDL.Items.Add(li0);
            DavaDurumuDDL.Items.Add(li1);
            DavaDurumuDDL.Items.Add(li2);

        }
        private class DavaDurusmaListItem
        {
            public string SiraNo { get; set; }
            public string GorevliPersonel { get; set; }
            public string DurusmaIli { get; set; }
            public string EsasNo { get; set; }
            public string Konusu { get; set; }
            public string DavaTuru { get; set; }
            public string TarihSaat { get; set; }
            public string Sonuc { get; set; }
            public string Mahkeme { get; set; }
            public string KarsiTaraf { get; set; }
            public string Aciklama { get; set; }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            });
        }
        protected void DavaDurumuDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "DavaDurumuDDL_SelectedIndexChanged", () =>
            {
                TabloOlustur();
            });
        }
    }
}
