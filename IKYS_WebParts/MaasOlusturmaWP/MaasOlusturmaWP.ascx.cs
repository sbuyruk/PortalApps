using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Utility.ProjeGlobal;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Text;
using Model.IKYS;
using Model.Ortak;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Globalization;
using static Model.IKYS.Personel;
using System.Web.Script.Serialization;
using Utility.HelperClasses;

namespace IKYS_WebParts.MaasOlusturmaWP
{
    [ToolboxItemAttribute(false)]
    public partial class MaasOlusturmaWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public MaasOlusturmaWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        #region Global Variables
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            TabloOlustur();
            UygulamaTarihiTxt.Text = DateTime.Today.ToString("dd.MM.yyyy");
        }
        #region Methods
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
                List<MaasListItem> list = GetDataList();
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
        private DataTable GetDataTable()
        {
            UcretTanim ucretTanim = new UcretTanim();
            DataTable dataTable = ucretTanim.SelectMaasListesi();
            return dataTable;
        }
        private List<MaasListItem> GetDataList()
        {

            DataTable dataTable = GetDataTable();

            List<MaasListItem> list = new List<MaasListItem>();
            if (dataTable != null)
            {

                foreach (DataRow row in dataTable.Rows)
                {
                    int derece = row["Derece"].ConvertToInt();
                    int kademe = row["Kademe"].ConvertToInt();
                    int protokolSiraNo = row["ProtokolSiraNo"].ReturnZeroIfNull().ConvertToInt();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();
                    string unvan = row["Unvan"].ToString();
                    decimal ucret = row["Ucret"].ConvertToDecimal();

                    string dereceKademeIlerlemeTarihi = row["DereceKademeIlerlemeTarihi"].ReturnEmptyIfNull().ConvertToDatetime().ConvertToDatetimeEmptyIfNull();

                    MaasListItem listItem = new MaasListItem();
                    listItem.AdiSoyadi = adi + " " + soyadi;
                    listItem.Unvan = unvan;
                    listItem.DereceKademeIlerlemeTarihi = dereceKademeIlerlemeTarihi;
                    listItem.DereceKademe = derece.ToString() +"/" +kademe.ToString();
                    listItem.ProtokolSiraNo = protokolSiraNo;
                    listItem.Ucret = ucret.ToString("N", culturInfo);
                    decimal agi = 3000;
                    listItem.Agi = agi.ToString("N", culturInfo); 
                    listItem.Toplam=(ucret+agi).ToString("N", culturInfo);
                    //
                    list.Add(listItem);
                }
            }
            return list;
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();
                 jQuery(document).ready(function () {

                        jQuery('#CustomDataTable').DataTable({
                            data: " + jsonData + @",
                            columns: [
                                { data: 'ProtokolSiraNo' },
                                { data: 'AdiSoyadi' },
                                { data: 'Unvan' },
                                { data: 'DereceKademeIlerlemeTarihi' },
                                { data: 'DereceKademe' },
                                { data: 'Ucret' },
                                { data: 'Agi' },
                                { data: 'Toplam'},
                            ],
                            columnDefs: [
                                { type: 'turkish', targets:[1,2] },
                                { type: 'num', targets: [5,6,7] },
                                
                            ],
                            'order': [[0, 'asc']],// Sıralı
                            'language': {
                                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                                'decimal': ',',
                                'thousands': '.'
                            },
                            responsive: true,
                            destroy: true,
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
                                    exportOptions: {
                                        columns: ':visible'
                                    }
                                },
                                {
                                    extend: 'pdf',
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
                            ]
                        });
                    });";
            return tableString;
        }
        #endregion
        #region Events
        protected void MaasOlusturBtn_Click(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        #endregion
        #region class
        private class MaasListItem
        {
            public string AdiSoyadi { get; set; }
            public string Unvan { get; set; }
            public int ProtokolSiraNo { get; set; }
            public string DereceKademeIlerlemeTarihi { get; set; }
            public string DereceKademe { get; set; }
            public string Ucret { get; set; }
            public string Agi { get; set; }
            public string Toplam { get; set; }
        }
        #endregion
    }
}
