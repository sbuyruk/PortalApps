using Model.Ortak;
using Model.TBYS;
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

namespace MFYS_WebParts.AylikParaGirisiIslemToplamiWP
{
    [ToolboxItemAttribute(false)]
    public partial class AylikParaGirisiIslemToplamiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public AylikParaGirisiIslemToplamiWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private readonly IFormatProvider cultureInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                IslemDDLDoldur();
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
                List<BankaIslemListItem> list = GetDataList();
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
                            var row = api.row(function (idx, data, node) { //secilen kayda gider
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
                            { data: 'IslemTarihi' },
                            { data: 'Islem' },
                            { data: 'IslemYapan' },
                            { data: 'IslemTutari' },
                            { data: 'Aciklama'},
                        ],
                        columnDefs: [
                            {
                                targets: 3,
                                className: 'dt-body-right'
                            }
                          ],
                        'language': {
                            'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        pageLength:10,
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
                                title: 'Aylık Para Girişleri',
                                filename: function(){
                                    var d = new Date();
                                    var n = d.getTime();
                                    return 'Aylik-para-girisleri' + n;
                                },
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'pdf',
                                title: 'Aylık Para Girişleri',
                                filename: function(){
                                    var d = new Date();
                                    var n = d.getTime();
                                    return 'Aylik-para-girisleri' + n;
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
        private List<BankaIslemListItem> GetDataList()
        {
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int gunSayisi = DateTime.DaysInMonth(yil, ay);
            int odemeSebebiId = IslemDDL.SelectedItem.Value.ConvertToInt();
            decimal genelToplam = 0;
            List<BankaIslemListItem> list = new List<BankaIslemListItem>();
            for (int i = 1; i <= gunSayisi; i++)
            {
                DateTime tarih = new DateTime(yil, ay, i);
                KiraEkstreAktarma kiraEkstreAktarma = new KiraEkstreAktarma();
                DataTable dataTable = kiraEkstreAktarma.SelectTarihOdemeSebebi(tarih, odemeSebebiId);

                foreach (DataRow row in dataTable.Rows)
                    {
                        string islemTarihiStr = tarih.ConvertToDatetimeEmptyIfNull();
                        string odemeSebebi = IslemDDL.SelectedItem.Text;

                        string odemeSebebiA = row["OdemeSebebiA"].ToString();
                        string odemeSebebiB = row["OdemeSebebiB"].ToString();
                        string islem = string.IsNullOrEmpty(odemeSebebiB) ? odemeSebebiA : odemeSebebiB;

                        string adiA = row["AdiA"].ToString();
                        string adiB = row["AdiB"].ToString();
                        string islemYapan = string.IsNullOrEmpty(adiB) ? adiA : adiB;

                        string tutarA = row["TutarA"].ToString();
                        string tutarB = row["TutarB"].ToString();
                        decimal islemTutari = (string.IsNullOrEmpty(tutarB) ? tutarA : tutarB).ReturnZeroIfNull().ConvertToDecimal();

                        string aciklamaA = row["AciklamaA"].ToString();
                        string aciklamaB = row["AciklamaB"].ToString();
                        string aciklama = aciklamaA + " # " + aciklamaB;

                        BankaIslemListItem islemListItem = new BankaIslemListItem();
                        islemListItem.IslemTarihi = islemTarihiStr;
                        islemListItem.Islem = islem;
                        islemListItem.IslemTutari = islemTutari.ToString("N", cultureInfo) + " TL";
                        islemListItem.IslemYapan = islemYapan;
                        islemListItem.Aciklama = aciklama;
                        genelToplam += islemTutari;
                        list.Add(islemListItem);
                    }         
            }
            ToplamLbl.Text = "Toplam : " + genelToplam.ToString("N", cultureInfo) + " TL";
            return list;
        }
        private void IslemDDLDoldur()
        {

            if (IslemDDL.SelectedItem == null)
            {
                IslemDDL.Items.Clear();
                IslemDDL.Items.Add(new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI_INT.ToString()));
                OdemeSebebiTanim odemeSebebiTanim = new OdemeSebebiTanim();
                List<OdemeSebebiTanim> list = odemeSebebiTanim.SelectAll<OdemeSebebiTanim>();
                foreach (OdemeSebebiTanim item in list)
                {
                    IslemDDL.Items.Add(new ListItem(item.OdemeSebebi, item.Id.ToString()));
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
        protected void IslemDDL_SelectedIndexChanged(object sender, EventArgs e)
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
        private class BankaIslemListItem
        {
            public string IslemTarihi { get; set; }
            public string Islem { get; set; }
            public string IslemYapan { get; set; }
            public string IslemTutari { get; set; }
            public string B_A { get; set; }
            public string Aciklama { get; set; }
        }
    }
}
