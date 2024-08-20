using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
namespace TBYS_WebParts.KiraArtisCizelgesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraArtisCizelgesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraArtisCizelgesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private int BolgeIdQS
        {
            get
            {

                if (ViewState["BolgeId"] == null)
                {
                    if (Page.Request.QueryString["BolgeId"] != null)
                    {
                        ViewState["BolgeId"] = Page.Request.QueryString["BolgeId"];
                    }
                    else
                    {
                        ViewState["BolgeId"] = string.Empty;
                    }
                }
                return ViewState["BolgeId"].ReturnZeroIfNull().ConvertToInt();
            }

            set
            {
                ViewState["BolgeId"] = value;
            }
        }
        private string CurrentUserName
        {
            get
            {

                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUserLoginName();
                }
                return ViewState["CurrentUserName"].ToString();
            }

            set
            {
                ViewState["CurrentUserName"] = value;
            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Bolge bolge = IKYSOrtak.BolgeGetirByUserName(CurrentUserName);
                BolgeIdQS = bolge == null ? 0 : bolge.Id;
                if (!Page.IsPostBack)
                {
                    AyDDLDoldur();
                    KiraSuresiDDLDoldur();
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            //UtilityHelper.ScriptCalistir(jsString);
            UtilityHelper.ScriptCalistir("setDataSet(" + jsonData + ");");
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<KiraArtisListItem> list = GetDataList();
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
            string a= "$(\"row c[r^='E']\", sheet)";
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
            
            jQuery(document).ready(function () {

                var table = jQuery('#CustomDataTable').DataTable({
                    data: " + jsonData + @",
                    columns: [
                        { data: 'Bolge' },
                        { data: 'KiraciAdi', 'width': '20%' },
                        { data: 'TamAdres', 'width': '20%' },
                        { data: 'KiralamaAmaci' },
                        { data: 'SozlesmeTarihi' },
                        { data: 'KiraSuresi' },
                        { data: 'ArtisAyi' },
                        { data: 'KiraBedeli', type: 'decimal', class: 'text-right' },
                        { data: 'Tufe', type: 'decimal' },
                        { data: 'YeniKiraBedeli', type: 'decimal', class: 'text-right' },
                        { data: 'YenilendiMi' },

                    ],
                    'order': [[0, 'asc']],//bolge Sıralı
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
                            extend: 'excelHtml5',
                            customize: function (xlsx) {
                                var sheet = xlsx.xl.worksheets['sheet1.xml'];

                                var count = 0;
                                var skippedHeader = 0;
                                $('row c[r^='E']', sheet).each(function () {
                                    if (count++ > 0) {
                                        var text = $(this).text();
                                        var yilInt = text.replace(' Yıl', '');
                                        if (yilInt >= 5) {
                                            $(this).attr('s', '11');
                                        }
                                    }

                                });
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
                        'pageLength', 'colvis'
                    ],
                    'createdRow': function (row, data, dataIndex) {
                        if (data.OnYil == 'True') {
                            $(row).addClass('on-yil');
                        } else if (data.BesYil == 'True') {
                            $(row).addClass('bes-yil');
                        }

                    },//set row color
                });
        });            
        ";

            return tableString;
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToExcel();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void ExportToExcel()
        {
            TabloOlustur();
            string filename = "KiraArtisCizelgesi."+ DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            //Türkçe sorunu yok
            Page.Response.Clear();
            Page.Response.AddHeader("content-disposition", "attachment;filename=" + filename + "");
            Page.Response.ContentType = "application/ms-excel";
            Page.Response.ContentEncoding = System.Text.Encoding.Unicode;
            Page.Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(sw);

            TabloDiv.RenderControl(hw);

            Page.Response.Write(sw.ToString());
            Page.Response.End();
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
        private List<KiraArtisListItem> GetDataList()
        {

            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            DateTime tarih = new DateTime(DateTime.Today.AddMonths(ay).Year, DateTime.Today.AddMonths(ay).Month, 1);

            KiraSozlesme kiraSozlesmeDao = new KiraSozlesme();

            DataTable dataTable = kiraSozlesmeDao.SelectKiraArtisiGelenSozlesmelerReturnDT(BolgeIdQS, tarih);
            int SiraNo = 1;

            List<KiraArtisListItem> list = new List<KiraArtisListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            decimal tufe = SecilenAyIcinTufeBul(tarih);
            DataView dataView = new DataView(dataTable);
            foreach (DataRowView row in dataView)
            {
                string kiraciId = row["KiraciId"].ToString();
                string kiraciAdi = row["KiraciAdi"].ToString();
                string kiraciSoyadi = row["KiraciSoyadi"].ToString();
                DateTime ilkSozlesmeTar = row["IlkSozlesmeTar"].ConvertToDatetime();
                string ilkSozlesmeTarStr = row["IlkSozlesmeTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                string sozBasTar = row["SozBasTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                string sozBitTar = row["SozBitTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                string kiralamaAmaci = row["KiralamaAmaci"].ToString();
                decimal kiraBedeli = row["KiraBedeli"].ConvertToDecimal();
                int kiraSozlesmeId = row["KiraSozlesmeId"].ConvertToInt();
                string artisAyi = row["ArtisAyi"].ToString();
                bool aktif = row["Aktif"].ReturnFalseIfNull().ConvertToBool();

                //DateTime bugun = DateTime.Today;
                decimal yeniKiraBedeli =  kiraBedeli + Math.Round(kiraBedeli * tufe / 100);
                //bool sozlesmeYenilendiMi = sozBitTar.ConvertToDatetime() > new DateTime( bugun.Year, bugun.AddMonths(2).Month,1); //--new DateTime(bugun.AddYears(1).Year, bugun.AddMonths(1).Month,1);
                //if (sozlesmeYenilendiMi)
                //{
                //    KiraSozlesme oncekiKiraSozlesme = new KiraSozlesme();
                //    oncekiKiraSozlesme = oncekiKiraSozlesme.SelectByKiraciIdTarih(kiraciId.ConvertToInt(), sozBitTar.ConvertToDatetime().AddMonths(-1));
                //    if (oncekiKiraSozlesme != null)
                //    {
                //        kiraBedeli = oncekiKiraSozlesme.KiraBedeli;
                //    }
                    
                //}
                string artisOrani= "%" + tufe.ToString("N", culturInfo) + " (TÜFE)";
                DateTime bastar = string.IsNullOrEmpty(sozBasTar.ConvertToDatetimeEmptyIfNull()) ? DateTime.Today : sozBasTar.ConvertToDatetime();
                DateTime yenibastar = bastar.AddYears(1);
                if ((yenibastar >= ProjeConstants.SINIRLIKIRAARTISI_BASLAMATARIHI) &&
                   (yenibastar <= ProjeConstants.SINIRLIKIRAARTISI_BITISTARIHI) &&
                   kiralamaAmaci.Equals(ProjeConstants.SINIRLIKIRAARTISI_UYGULANACAKTASINMAZCINSI))
                {
                    yeniKiraBedeli = Math.Round(kiraBedeli + kiraBedeli * ProjeConstants.SINIRLIKIRAARTISI_ORANI / 100);
                    artisOrani = "%" + ProjeConstants.SINIRLIKIRAARTISI_ORANI.ToString("N", culturInfo) + " (6098 Say.Kanun)";
                }
                else
                {
                    yeniKiraBedeli = Math.Round(kiraBedeli + kiraBedeli * tufe / 100);
                }

                string adres = row["Adres"].ToString();

                string ili = row["Ili"].ToString();
                string ilcesi = row["Ilcesi"].ToString();
                string bolge = row["bolge"].ToString();

                KiraArtisListItem kiraArtisListItem = new KiraArtisListItem();
                kiraArtisListItem.Sirano = SiraNo++.ToString();
                kiraArtisListItem.SozlesmeId = kiraSozlesmeId.ToString();
                kiraArtisListItem.KiraciId = kiraciId;
                kiraArtisListItem.KiraciAdi = kiraciAdi + " " + kiraciSoyadi;
                kiraArtisListItem.SozlesmeTarihi = ilkSozlesmeTarStr;

                int kiraSuresi = Math.Round(tarih.Subtract(ilkSozlesmeTar).TotalDays/365).ConvertToInt();
                int kiraSuresi1 = kiraSuresi < 5 ? kiraSuresi: 
                    (kiraSuresi >= 5 && kiraSuresi < 10) ? 5 :
                    (kiraSuresi >= 10 ? 10 : kiraSuresi);
                int secilenKiraSuresi= KiraSuresiDDL.SelectedItem.Value.ConvertToInt();
                if ((secilenKiraSuresi == 0)||(secilenKiraSuresi==kiraSuresi1))
                {
                    kiraArtisListItem.KiraSuresi = kiraSuresi + " Yıl";
                    kiraArtisListItem.BesYil = kiraSuresi >= 5 ? "True" : "False";
                    kiraArtisListItem.OnYil = kiraSuresi >= 10 ? "True" : "False";
                    kiraArtisListItem.SozlesmeBasTar = sozBasTar.ConvertToDatetimeEmptyIfNull();
                    kiraArtisListItem.SozlesmeBitTar = sozBitTar.ConvertToDatetimeEmptyIfNull();
                    kiraArtisListItem.TarihAraligi = sozBasTar.ConvertToDatetimeEmptyIfNull() + "-" + sozBitTar.ConvertToDatetimeEmptyIfNull();
                    kiraArtisListItem.KiralamaAmaci = kiralamaAmaci;
                    kiraArtisListItem.KiraBedeli = kiraBedeli.ToString("N", culturInfo) + " TL";
                    kiraArtisListItem.Tufe = artisOrani;
                    kiraArtisListItem.YeniKiraBedeli = yeniKiraBedeli.ToString("N", culturInfo) + " TL";
                    kiraArtisListItem.Adres = "- " + adres;
                    kiraArtisListItem.TamAdres = "- " + adres + " " + ilcesi + "-" + ili;
                    kiraArtisListItem.Ilcesi = ilcesi;
                    kiraArtisListItem.Ili = ili;
                    kiraArtisListItem.Bolge = bolge;
                    kiraArtisListItem.ArtisAyi = new DateTime(tarih.Year, artisAyi.ConvertToInt(), 1).ToString("MMMM");
                    kiraArtisListItem.YenilendiMi = aktif ? "Yenilenecek" : "Yenilendi";
                    list.Add(kiraArtisListItem); 
                }
            }
            return list;
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void KiraSuresiDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        private void AyDDLDoldur()
        {
            AyDDL.Items.Clear();
            ListItem li= new ListItem(DateTime.Today.ToString("MMMM"),"0" );
            ListItem li1= new ListItem(DateTime.Today.AddMonths(1).ToString("MMMM"), "1");
            ListItem li2= new ListItem(DateTime.Today.AddMonths(2).ToString("MMMM"), "2");
            ListItem li3= new ListItem(DateTime.Today.AddMonths(3).ToString("MMMM"), "3");
            ListItem li4= new ListItem(DateTime.Today.AddMonths(4).ToString("MMMM"), "4");
            ListItem li5= new ListItem(DateTime.Today.AddMonths(5).ToString("MMMM"), "5");
            ListItem li6= new ListItem(DateTime.Today.AddMonths(6).ToString("MMMM"), "6");
            ListItem li7= new ListItem(DateTime.Today.AddMonths(7).ToString("MMMM"), "7");
            ListItem li8= new ListItem(DateTime.Today.AddMonths(8).ToString("MMMM"), "8");
            ListItem li9= new ListItem(DateTime.Today.AddMonths(9).ToString("MMMM"), "9");
            ListItem li10= new ListItem(DateTime.Today.AddMonths(10).ToString("MMMM"), "10");
            ListItem li11= new ListItem(DateTime.Today.AddMonths(11).ToString("MMMM"), "11");
            AyDDL.Items.Add(li);
            AyDDL.Items.Add(li1);
            AyDDL.Items.Add(li2);
            AyDDL.Items.Add(li3);
            AyDDL.Items.Add(li4);
            AyDDL.Items.Add(li5);
            AyDDL.Items.Add(li6);
            AyDDL.Items.Add(li7);
            AyDDL.Items.Add(li8);
            AyDDL.Items.Add(li9);
            AyDDL.Items.Add(li10);
            AyDDL.Items.Add(li11);
        }
        private void KiraSuresiDDLDoldur()
        {
            KiraSuresiDDL.Items.Clear();
            ListItem li = new ListItem("Hepsi", "0");
            ListItem li1 = new ListItem("5 Yılı Dolan Kiracılar","5");
            ListItem li2 = new ListItem("10 Yılı Dolan Kiracılar","10");
            //ListItem li3 = new ListItem("5 ve 10 Yılı Dolan Kiracılar","3");
            KiraSuresiDDL.Items.Add(li);
            KiraSuresiDDL.Items.Add(li1);
            KiraSuresiDDL.Items.Add(li2);
            //KiraSuresiDDL.Items.Add(li3);
        }
        private decimal SecilenAyIcinTufeBul(DateTime tarih)
        {
            tarih = tarih.AddMonths(1);//bir önceki ay geliyor
            decimal tufe = 1M;
            YasalFaiz yasalFaiz = new YasalFaiz();
            //DateTime gelecekAy = DateTime.Today.AddMonths(1);
            yasalFaiz = yasalFaiz.SelectByYilAy(tarih.Year, tarih.Month);//gelecek ay artacak
            if (yasalFaiz != null)
            {
                tufe = yasalFaiz.Tufe;
            }
            return tufe;
        }
        private class KiraArtisListItem
        {
            public string Sirano { get; set; }
            public string ArtisAyi { get; set; }
            public string SozlesmeId { get; set; }
            public string KiraciAdi { get; set; }
            public string KiraciId { get; set; }
            public string KiralamaAmaci { get; set; }
            public string SozlesmeTarihi { get; set; }
            public string KiraSuresi { get; set; }
            public string SozlesmeBasTar { get; set; }
            public string SozlesmeBitTar { get; set; }
            public string TarihAraligi { get; set; }
            public string KiraBedeli { get; set; }
            public string Tufe { get; set; }
            public string YeniKiraBedeli { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Bolge { get; set; }
            public string Adres { get; set; }
            public string TamAdres { get; set; }
            public string YenilendiMi { get; set; }
            public string BesYil { get; set; }
            public string OnYil { get; set; }
        }
    }
}
