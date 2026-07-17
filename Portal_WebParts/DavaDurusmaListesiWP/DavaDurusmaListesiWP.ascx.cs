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
            if (!Page.IsPostBack)
            {
                DavaDurumuDDLDoldur();
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
                List<DavaDurusmaListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
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
        private List<DavaDurusmaListItem> GetDataList()
        {
            List<DavaDurusmaListItem> davaDurusmaList = new List<DavaDurusmaListItem>();
            try
            {
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
                                        <Value Type='Text'>" + DavaDurumuDDL.SelectedItem.Value + @" </Value>
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
            }
            catch (Exception)
            {
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
                        columnDefs: [                            
                            
                        ],
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
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void DavaDurumuDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
    }
}
