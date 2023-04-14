using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.KiraSozlesmeSiralamaWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraSozlesmeSiralamaWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraSozlesmeSiralamaWP()
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
                TabloOlustur();
            }
        }
        
        private string TabloJson()
        {
            string jSon = string.Empty;

            List<KiraSozlesmeListItem> list = GetDataList();
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            jSon = serializer.Serialize(list);
            return jSon;
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }

       
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                 jQuery(document).ready(function () {
$('#CustomDataTable').on( 'draw.dt', function () {
    //alert( 'Table redrawn' );
} );
                        jQuery('#CustomDataTable').DataTable({                            
                            data: " + jsonData + @",
                            columns: [
                                { data: 'DosyaNo' },
                                { data: 'SozlesmeId' },
                                { data: 'KiraciAdi' },
                                { data: 'TarihAraligi' },
                                { data: 'KiraBedeli' },
                                { data: 'Adres' },

                            ],
                            columnDefs: [
                                { type: 'turkish', targets:[2,3,4,5] },
                                { type: 'num', targets: 0 },
                                { targets: 4, className: 'dt-body-right'},
                            ],
                            'language': {
                                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                                'decimal': ',',
                                'thousands': '.'
                            },
                            responsive: true,
                            paging:false,
                            dom: 'Bfrti',
                            rowReorder: {
                                selector: 'tr',
                                update: false
                            },
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
        private List<KiraSozlesmeListItem> GetDataList()
        {

            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            DataTable dataTable = kiraSozlesme.SelectKiraSozlesmeListReturnDT(0, ProjeConstants.KIRASOZLESME_AKTIF_INT,ProjeConstants.BOLGE_HEPSI);
            string tempIli = string.Empty;
            string tempIlcesi = string.Empty;
            string tempAdres = string.Empty;
            KiraSozlesmeListItem tempSozlesmeItem = null;
            List<KiraSozlesmeListItem> list = new List<KiraSozlesmeListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            //DataView dataView = new DataView(dataTable);
            int tempSozlesmeId = 0;
            int sozlesmeAdedi = 0;
            foreach (DataRow row in dataTable.Rows)
            {
                string ili = string.Empty;
                string ilcesi = string.Empty;
                string adres = string.Empty;

                string dosyaNo = row["DosyaNo"].ToString();
                string kiraciAdi = row["KiraciAdi"].ToString();
                string kiraciSoyadi = row["KiraciSoyadi"].ToString();
                string ilkSozlesmeTar = row["IlkSozlesmeTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                string sozBasTar = row["SozBasTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                string sozBitTar = row["SozBitTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                string odemeSekli = row["odemeSekli"].ToString();
                decimal kiraBedeli = row["KiraBedeli"].ConvertToDecimal();
                int kiraSozlesmeId = row["KiraSozlesmeId"].ConvertToInt();
                string BolumNo = row["BolumNo"].ToString();
                string sozlesmeDurumu = row["SozlesmeDurumu"].ToString();
                string durumDegismeTar = row["DurumDegismeTar"].ToString();

                adres = row["Adres"].ToString() + " " + BolumNo;
                ili = row["Ili"].ToString();
                ilcesi = row["Ilcesi"].ToString();

                if (tempSozlesmeId == kiraSozlesmeId)
                {
                    sozlesmeAdedi++;
                    tempSozlesmeId = kiraSozlesmeId;
                    list.Remove(tempSozlesmeItem);
                    //tempSozlesmeItem.Adres += "@" + adres;
                    tempSozlesmeItem.Adres = adres + (sozlesmeAdedi > 1 ? " (Toplam " + sozlesmeAdedi.ToString() + " adet taşınmaz)" : "");
                    list.Add(tempSozlesmeItem);
                }
                else
                {
                    KiraSozlesmeListItem sozlesmeItem = new KiraSozlesmeListItem();
                    sozlesmeItem.DosyaNo = dosyaNo;
                    sozlesmeItem.SozlesmeId = kiraSozlesmeId.ToString();
                    sozlesmeItem.KiraciAdi = kiraciAdi + " " + kiraciSoyadi;
                    sozlesmeItem.SozlesmeTarihi = ilkSozlesmeTar;
                    sozlesmeItem.SozlesmeBasTar = sozBasTar.ConvertToDatetimeEmptyIfNull();
                    sozlesmeItem.SozlesmeBitTar = sozBitTar.ConvertToDatetimeEmptyIfNull();
                    sozlesmeItem.TarihAraligi = sozBasTar.ConvertToDatetimeEmptyIfNull() + "-" + sozBitTar.ConvertToDatetimeEmptyIfNull();
                    sozlesmeItem.KiraBedeli = kiraBedeli.ToString("N", culturInfo);
                    sozlesmeItem.Adres = "- " + adres;
                    sozlesmeAdedi = 0;
                    list.Add(sozlesmeItem);
                    tempSozlesmeItem = sozlesmeItem;

                }
                tempSozlesmeId = kiraSozlesmeId;

            }
            return list;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private class KiraSozlesmeListItem
        {
            public string DosyaNo { get; set; }
            public string SozlesmeId { get; set; }
            public string KiraciAdi { get; set; }
            public string SozlesmeTarihi { get; set; }
            public string SozlesmeBasTar { get; set; }
            public string SozlesmeBitTar { get; set; }
            public string TarihAraligi { get; set; }
            public string KiraBedeli { get; set; }
            public string Adres { get; set; }

        }

        protected void KaydetNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string value = paramDosyaNoArray.Value;
                string[] idList = value.Split(',');
                int sira = 0;
                foreach (string item in idList)
                {
                    int kiraSozlesmeId = item.ConvertToInt();
                    if (kiraSozlesmeId > 0)
                    {
                        sira++;
                        KiraSozlesme ks = new KiraSozlesme();
                        ks = ks.Select<KiraSozlesme>(kiraSozlesmeId);
                        if (ks != null)
                        {
                            if (ks.DosyaNo != sira)
                            {
                                ks.DosyaNo = sira;
                                ks.Update();
                            }
                        }
                    }
                }
                TabloOlustur();
                MessageHelper.PublishMessage("Sıralama kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
            }
            catch (Exception)
            {
                MessageHelper.PublishMessage("Sıralama kaydedilemedi", ProjeConstants.MESAJ_HATA);
            }

        }
    }
}