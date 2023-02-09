using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.CokDefaBagisYapanBagisciListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class CokDefaBagisYapanBagisciListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public CokDefaBagisYapanBagisciListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SecilenIdQS
        {
            get
            {

                if (ViewState["SecilenId"] == null)
                {
                    if (Page.Request.QueryString["SecilenId"] != null)
                    {
                        ViewState["SecilenId"] = Page.Request.QueryString["SecilenId"];
                    }
                    else
                    {
                        ViewState["SecilenId"] = "0";
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    YonergeLnk.HRef = NBYSOrtak.YonergeURLGetir(ProjeConstants.PARAM_NBYSYONERGE, ProjeConstants.PAGE_COKDEFABAGISYAPAN_LIST);
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
            List<BagisciListItem> list = GetBagisciData();
            var serializer = new JavaScriptSerializer();
            var jsonData = serializer.Serialize(list);
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private List<BagisciListItem> GetBagisciData()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            List<BagisciListItem> list = new List<BagisciListItem>();
            NakitBagisci nakitBagisci = new NakitBagisci();

            ArmaganTanim at = new ArmaganTanim();
            at = at.Select<ArmaganTanim>(ProjeConstants.ARMAGAN_BRONZID);
            if (at != null)
            {
                decimal bronzArmaganLimiti = at.OzelKisiAltLimit;
                DataTable dataTable = nakitBagisci.SelectBagisciGroupByBagisAdediReturnDataTable(bronzArmaganLimiti);
                if (dataTable != null)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int nakitBagisciId = row["NakitBagisciId"].ReturnZeroIfNull().ConvertToInt();
                        string adi = row["Adi"].ReturnEmptyIfNull().ToString();
                        string soyadi = row["Soyadi"].ReturnEmptyIfNull().ToString();
                        string toplamBagisAdedi = row["Adet"].ReturnZeroIfNull().ToString();
                        decimal toplamBagisTutari = row["Toplam"].ReturnZeroIfNull().ConvertToDecimal();
                        DateTime sonBagisTarihi = row["SonBagisTarihi"].ReturnEmptyIfNull().ConvertToDatetime();
                        bool tuzelKisi = row["TuzelKisi"].ReturnZeroIfNull().ConvertToBool();

                        BagisciListItem listItem = new BagisciListItem();
                        listItem.NakitBagisciId = nakitBagisciId;
                        listItem.AdiSoyadi = (adi + " " + soyadi).Trim();
                        listItem.ToplamBagisAdedi = toplamBagisAdedi;
                        listItem.ToplamBagisTutari = toplamBagisTutari.ToString("N", culturInfo);
                        listItem.SonBagisTarihi = sonBagisTarihi.ConvertToDatetimeEmptyIfNull();
                        listItem.Madalya = MadalyaLinkiGetir(nakitBagisciId, toplamBagisAdedi, toplamBagisTutari, tuzelKisi, sonBagisTarihi);
                        list.Add(listItem);
                    }
                }
            }



            //string json = nakitBagisci.ToJSON(dataTable);
            return list;
        }

        private string MadalyaLinkiGetir(int nakitBagisciId, string toplamBagisAdedi, decimal toplamBagisTutari, bool tuzelKisiMi, DateTime sonBagisTarihi)
        {
            string retval = string.Empty;


            //madalya hakediyor mu ? hayır: return
            ArmaganTanim hakedilenArmaganTanim = new ArmaganTanim();
            hakedilenArmaganTanim = hakedilenArmaganTanim.SelectByTutar(toplamBagisTutari, tuzelKisiMi);
            if (hakedilenArmaganTanim != null)
            {
                //Daha Önce aldigi armaganlar
                Armagan aldigiArmagan = new Armagan();
                List<Armagan> aldigiArmaganlar = aldigiArmagan.SelectByBagisciId(nakitBagisciId);

                //Teşekkürse dikkate alma, zaten gönderilmiştir.
                if (hakedilenArmaganTanim.Id == ProjeConstants.ARMAGAN_TESEKKURID)
                {
                    retval = "-";
                }
                else if (hakedilenArmaganTanim.Id == ProjeConstants.ARMAGAN_ALTINID)
                {
                    //Altın Hakediyor
                    //Altın Madalya almıs mı
                    Armagan aldigiAltin = aldigiArmaganlar.FirstOrDefault(arm => arm.ArmaganTanimId == ProjeConstants.ARMAGAN_ALTINID);

                    if (aldigiAltin == null)
                    {
                        //Altın madalya almamış ama genel toplamda hak etmş
                        retval = "<a href=# onclick=\"MadalyaOlustur(" + nakitBagisciId + "," + hakedilenArmaganTanim.Id + ",\'" + sonBagisTarihi + "\');\" class=\'btn btn-outline-warning \'>Altın Madalya Oluştur</a>";
                    }
                    else
                    {
                        retval = aldigiAltin.Tarih.ConvertToDatetimeEmptyIfNull() + " Tarihinde " + " Altın Madalya Verilmiştir. ( Durumu :" + aldigiAltin.Durum + ")";
                    }

                }
                else if (hakedilenArmaganTanim.Id == ProjeConstants.ARMAGAN_GUMUSID)
                {
                    //Gümüş Hakediyor
                    //Gümüş Madalya almıs mı
                    Armagan aldigiMadalya = aldigiArmaganlar.FirstOrDefault(arm => arm.ArmaganTanimId == ProjeConstants.ARMAGAN_GUMUSID);

                    if (aldigiMadalya == null)
                    {
                        //Gümüş madalya almamış ama genel toplamda hak etmş
                        //retval = "<a href=" + ProjeConstants.PAGE_SIRADISIARMAGAN_DUZENLE + @"?DestinationApp=BD&NakitBagisciId=" + nakitBagisciId + " class=\'btn btn-outline-secondary \'>Gümüş Madalya Oluştur</a>";
                        retval = "<a href=# onclick=\"MadalyaOlustur(" + nakitBagisciId + "," + hakedilenArmaganTanim.Id + ",\'" + sonBagisTarihi + "\');\" class=\'btn btn-outline-secondary \'>Gümüş Madalya Oluştur</a>";
                    }
                    else
                    {
                        retval = aldigiMadalya.Tarih.ConvertToDatetimeEmptyIfNull() + " Tarihinde " + " Gümüş Madalya Verilmiştir. ( Durumu :" + aldigiMadalya.Durum + ")";
                    }

                }
                else if (hakedilenArmaganTanim.Id == ProjeConstants.ARMAGAN_BRONZID)
                {
                    //Bronz Hakediyor
                    //Bronz Madalya almıs mı
                    Armagan aldigiMadalya = aldigiArmaganlar.FirstOrDefault(arm => arm.ArmaganTanimId == ProjeConstants.ARMAGAN_BRONZID);

                    if (aldigiMadalya == null)
                    {
                        //Bronz madalya almamış ama genel toplamda hak etmş
                        retval = "<a href=# onclick=\"MadalyaOlustur(" + nakitBagisciId + "," + hakedilenArmaganTanim.Id + ",\'" + sonBagisTarihi + "\');\" class=\'btn btn-outline-info \'>Bronz Madalya Oluştur</a>";
                    }
                    else
                    {
                        retval = aldigiMadalya.Tarih.ConvertToDatetimeEmptyIfNull() + " Tarihinde " + " Bronz Madalya Verilmiştir. ( Durumu :" + aldigiMadalya.Durum + ")";
                    }

                }
            }
            return retval;
        }
        private bool MadalyaOlustur(int nakitBagisciId, int hakedilenArmaganTanimId, DateTime sonBagisTarihi)
        {
            //Bagisciyi bul
            //NakitBagisHareketten bagis listesini ve bagis toplamını al
            //Armagan Hakediyor mu
            //Evetse parametre olarak gelenle aynı mı?
            //Aynı değilse bişeyler değişti ne yapmak lazım?
            int armaganId = 0;
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = nakitBagisci.Select<NakitBagisci>(nakitBagisciId);

            if (nakitBagisci != null)
            {
                NakitBagisHareket nbhDao = new NakitBagisHareket();
                List<NakitBagisHareket> nakitBagisHareketListesi = nbhDao.SelectArmaganiOlmayanBagislarByBagisciId(nakitBagisciId);

                decimal toplamBagis = 0;
                if (nakitBagisHareketListesi.Count > 0)
                {
                    toplamBagis = nakitBagisHareketListesi.Sum(emp => emp.BagisMiktari);
                    ArmaganTanim hakedilenArmaganTanim = new ArmaganTanim();
                    hakedilenArmaganTanim = hakedilenArmaganTanim.SelectByTutar(toplamBagis, nakitBagisci.TuzelKisi);

                    if (hakedilenArmaganTanim != null)
                    {
                        DateTime bastar = ProjeConstants.COKBAGISYAPAN_BASLAMATARIHI.ConvertToDatetime();
                        DateTime bittar = DateTime.Today;
                        armaganId = EkstreAktarma.ArmaganiKaydetVeyaGuncelle(bastar, bittar, nakitBagisciId, sonBagisTarihi, toplamBagis, hakedilenArmaganTanim.Id, UtilityHelper.GetCurrentUserLoginName(), nakitBagisci, nakitBagisHareketListesi);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Bağışçının armağan hakkı bulunmamaktadır.", ProjeConstants.MESAJ_HATA);
                    }
                }
            }
            else
            {
                MessageHelper.PublishMessage("Bağışçı Bulunamadı", ProjeConstants.MESAJ_HATA);
            }
            return armaganId > 0;
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
            jQuery(document).ready(function () {

            jQuery('#CustomDataTable').DataTable({
                'initComplete': function (settings, json) {//tablo yüklendiğinde
                    var api = this.api();
                    var row = api.row(function(idx, data, node) { //secilen satıra gider
                        return data['NakitBagisciId'] ==" + SecilenIdQS + @";
                    });
                    if (row.length > 0)
                    {
                        row.select()
                            .show()
                            .draw(false);
                    }
                },
                data: " + jsonData + @",
                columns: [
                    { data: 'NakitBagisciId'},
                    { data: 'AdiSoyadi'},
                    { data: 'ToplamBagisAdedi' },
                    { data: 'ToplamBagisTutari' },
                    { data: 'SonBagisTarihi' },
                    { data: 'Madalya' },
                ],
                'order': [[2, 'desc']],
                columnDefs:
                [
                    {
                    targets: 1, render: function(data, type, row, meta) {
                    var link= '<a href=# onclick=OpenModal('+row.NakitBagisciId+'); class=\'btn btn-link \'>'+(row.AdiSoyadi).trim() + '</a>';
                    return link;
                    }},
                    { 'width': '30%', targets: [1,5] }
                ],
                'language': {
                    'url': 'http://tskgv-portal/OrtakBelgeler/Turkish.txt',
                    'decimal': ',',
                    'thousands': '.'
                },
                responsive: true,
                destroy: true,
                autoWidth: false,
                fixedColumns: true,
                dom: 'Bfrtip',
                buttons:
                [
                    {
                extend: 'print',
                        exportOptions:
                    {
                    columns: ':visible'
                        }
                },
                    {
                      extend: 'excel',
                      exportOptions: {
                          columns: ':visible',
                          format: {
                              body: function(data, row, column, node) {
                                  data = $('<p>' + data + '</p>').text();
                                  return $.isNumeric(data.replace(',', '.')) ? data.replace(',', '.') : data;
                              }
                          }
                      },
                },
                    {
                extend: 'pdf',
                        exportOptions:
                    {
                    columns: ':visible'
                        }
                },
                    {
                extend: 'copy',
                        exportOptions:
                    {
                    columns: ':visible'
                        }
                },
                    , 'pageLength', 'colvis'
                ]



            });
        });
        ";
            return tableString;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {

        }
        private void TabloModalOlustur(string nakitBagisciId)
        {
            var jsonData = GetModalDataJson(nakitBagisciId); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateModalDataTable(string jsonData)
        {
            string tableString = @"
        jQuery(document).ready(function () {
            if ( jQuery.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
                jQuery('#CustomModalDataTable').DataTable().destroy();
            }
            jQuery('#CustomModalDataTable tbody').empty();

            jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
            jQuery('#CustomModalDataTable').DataTable({
                data: " + jsonData + @",
                columns: [
                    { data: 'BagisTarihi' },
                    { data: 'BagisMiktari', 'width': '10%', 'className': 'text-right' },
                    { data: 'Banka' },
                    { data: 'Armagan' },
                    { data: 'ArmaganTutari' },
                    { data: 'Durum' },
                ],
                'order': [[0, 'desc']],

                'language': {
                    'url': 'http://tskgv-portal/OrtakBelgeler/Turkish.txt',
                    'decimal': ',',
                    'thousands': '.'
                },
                columnDefs:[
                    {targets:0, render:function(data){
                        return moment(data).format('DD.MM.YYYY');
                    }},
                ],
                responsive: true,
                dom: 'rtip',

            });
        });
        ";
            return tableString;
        }
        private string GetModalDataJson(string nakitBagisciId)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            NakitBagisHareket nbh = new NakitBagisHareket();
            int rowCount = 0;
            var json = nbh.SelectByBagisciIdReturnJSon(nakitBagisciId, ref rowCount);
            decimal toplamTutar = nbh.GetSumBagisMiktariByNakitBagisciIdBetweenBasTarBitTar(
                ProjeConstants.BAGIS_SORGU_BASTAR.ConvertToDatetime(), DateTime.Today, nakitBagisciId.ConvertToInt());
            BagisBilgileriLbl.Text = rowCount < 1 ? "Bağış bulunmamaktadır" :
                "Bağışçının " + rowCount + " defada yaptığı toplam " + toplamTutar.ToString("N", culturInfo) + "TL bağışı bulunmaktadır";
            return json;
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
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;
            GridView1.DataSource = GetBagisciData();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=BagisciListesi.xls");
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            Page.Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                //Apply text style to each Row
                GridView1.Rows[i].Attributes.Add("class", "textmode");
            }
            GridView1.RenderControl(hw);

            //style to format numbers to string
            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Page.Response.Write(style);
            Page.Response.Output.Write(sw.ToString());
            Page.Response.Flush();
            Page.Response.End();

        }
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            try
            {

                TabloModalOlustur(paramNakitBagisciIdLbl.Value);
                NakitBagisciFormunuDoldur(paramNakitBagisciIdLbl.Value);
                UtilityHelper.ScriptCalistir("SetPageIndex();");
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }
        }
        protected void MadalyaOlusturBtn_Click(object sender, EventArgs e)
        {
            try
            {
                MadalyaOlustur(paramNakitBagisciIdLbl.Value.ConvertToInt(), paramhakedilenarmaganIdLbl.Value.ConvertToInt(), paramSonBagisTarihiLbl.Value.ConvertToDatetime());
                RedirectToPage(ProjeConstants.PAGE_ARMAGAN_EDIT + "?ArmaganId=" + paramhakedilenarmaganIdLbl.Value.ConvertToInt() + "&NakitBagisciId=" + paramNakitBagisciIdLbl.Value.ConvertToInt());
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void NakitBagisciFormunuDoldur(string nakitBagisciIdStr)
        {
            if (!string.IsNullOrEmpty(nakitBagisciIdStr))
            {
                int nakitBagisciId = nakitBagisciIdStr.ConvertToInt();

                NakitBagisci nakitBagisci = new NakitBagisci();
                nakitBagisci = nakitBagisci.Select<NakitBagisci>(nakitBagisciId);
                if (nakitBagisci != null)
                {
                    //NakitBagisciIdLbl.Text = nakitBagisciId.ToString();
                    TableRow row = new TableRow();
                    TableCell AdiCell = new TableCell();
                    TableCell TCKimlikNoCell = new TableCell();
                    TableCell AdresCell = new TableCell();
                    TableCell IlIlceCell = new TableCell();
                    TableCell TelefonCell = new TableCell();
                    TableCell TuzelKisiCell = new TableCell();

                    AdiCell.Text = nakitBagisci.Adi.ReturnEmptyIfNull().ToString();
                    TCKimlikNoCell.Text = nakitBagisci.TCKimlikNo.ReturnEmptyIfNull().ToString();
                    AdresCell.Text = nakitBagisci.Adres.ReturnEmptyIfNull().ToString();

                    int ilId = nakitBagisci.Ili.ConvertToInt();
                    Il il = new Il();
                    il = il.Select<Il>(ilId);
                    if (il != null)
                    {

                        IlIlceCell.Text = il.IlAdi.ReturnEmptyIfNull().ToString();
                    }
                    int ilceId = nakitBagisci.Ilcesi.ConvertToInt();
                    Ilce ilce = new Ilce();
                    ilce = ilce.Select<Ilce>(ilceId);
                    if (ilce != null)
                    {

                        IlIlceCell.Text += " " + ilce.IlceAdi.ReturnEmptyIfNull().ToString();
                    }
                    TelefonCell.Text = nakitBagisci.Telefon1.ReturnEmptyIfNull().ToString();
                    TuzelKisiCell.Text = nakitBagisci.TuzelKisi.ConvertToBool() ? "Evet" : "Hayır";
                    row.Controls.Add(AdiCell);
                    row.Controls.Add(TCKimlikNoCell);
                    row.Controls.Add(AdresCell);
                    row.Controls.Add(IlIlceCell);
                    row.Controls.Add(TelefonCell);
                    row.Controls.Add(TuzelKisiCell);
                    BagisciTable.Controls.Add(row);

                }
            }
        }
        private class BagisciListItem
        {
            public string Sirano { get; set; }
            public int NakitBagisciId { get; set; }
            public string AdiSoyadi { get; set; }
            public string ToplamBagisAdedi { get; set; }
            public string ToplamBagisTutari { get; set; }
            public string SonBagisTarihi { get; set; }
            public string Madalya { get; set; }
            public bool Secildi { get; set; }
        }
    }
}
