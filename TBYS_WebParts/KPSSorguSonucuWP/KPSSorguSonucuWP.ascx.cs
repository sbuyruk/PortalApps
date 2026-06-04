using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using Model.Ortak;
using Model.TBYS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Web.UI.WebControls.WebParts;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.KPSSorguSonucuWP
{
    [ToolboxItemAttribute(false)]
    public partial class KPSSorguSonucuWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KPSSorguSonucuWP()
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
        }
        private void TabloOlustur (string jsonData)
        {
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery('#CustomDataTable').DataTable({
                    data: " + jsonData + @",
                    columns: [
                        { data: 'KimlikNo' },
                        { data: 'AdiSoyadi' },
                        { data: 'SagVefat' },
                        { data: 'Aciklama' },
                    ],
                    'columnDefs': [
                        { type: 'turkish', targets: [1,2] },
                        {targets:2, render:function(data, type, row, meta){
                            if (row.SagVefat =='Açik'){
                                return 'Sag';
                            }else
                            {
                                return data;
                            }
                        }},
                       {targets:3, render:function(data, type, row, meta){
                            if (row.VefatTarihi != null){
                                return row.VefatTarihi;
                            }else
                            {
                                return data;
                            }
                        }}, 
                    ],
                    'order': [[1, 'asc']],//sort 
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
                        ],                  
                    

                });

            ";

            return tableString;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }

        protected void DosyayiYukleBtn_Click(object sender, EventArgs e)
        {
            if (DosyaYukleFU.HasFile)
            {
                
                using (Stream stream = DosyaYukleFU.FileContent)
                {

                    StreamReader reader = new StreamReader(stream);
                    string text = reader.ReadToEnd();
                    TabloOlustur(text);

                }
            }

        }
        public class SorguSonucu
        {
            private long kimlikNo;
            private string adiSoyadi;
            private string sagVefat;
            private int sagVefatKod;
            private DateTime vefatTarihi;
            private string aciklama;

            public long KimlikNo { get => kimlikNo; set => kimlikNo = value; }
            public string AdiSoyadi { get => adiSoyadi; set => adiSoyadi = value; }
            public string SagVefat { get => sagVefat; set => sagVefat = value; }
            public int SagVefatKod { get => sagVefatKod; set => sagVefatKod = value; }
            public DateTime VefatTarihi { get => vefatTarihi; set => vefatTarihi = value; }
            public string Aciklama { get => aciklama; set => aciklama = value; }
        }
    }
}