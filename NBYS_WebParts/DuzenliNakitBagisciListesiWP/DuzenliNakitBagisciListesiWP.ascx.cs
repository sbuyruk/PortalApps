using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.SharePoint.Mobile.Controls;
using Microsoft.SharePoint.WebPartPages.Communication;
using Model.NBYS;
using Model.Ortak;
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

namespace NBYS_WebParts.DuzenliNakitBagisciListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class DuzenliNakitBagisciListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public DuzenliNakitBagisciListesiWP()
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
        private string NakitBagisciIdQS
        {
            get
            {

                if (ViewState["NakitBagisciId"] == null)
                {
                    if (Page.Request.QueryString["NakitBagisciId"] != null)
                    {
                        ViewState["NakitBagisciId"] = Page.Request.QueryString["NakitBagisciId"];
                    }
                    else
                    {
                        ViewState["NakitBagisciId"] = "-2";
                    }
                }
                return ViewState["NakitBagisciId"].ToString();
            }

            set
            {
                ViewState["NakitBagisciId"] = value;
            }
        }
        private string DuzenliBagisciIdQS
        {
            get
            {

                if (ViewState["DuzenliBagisciId"] == null)
                {
                    if (Page.Request.QueryString["DuzenliBagisciId"] != null)
                    {
                        ViewState["DuzenliBagisciId"] = Page.Request.QueryString["DuzenliBagisciId"];
                    }
                    else
                    {
                        ViewState["DuzenliBagisciId"] = "-2";
                    }
                }
                return ViewState["DuzenliBagisciId"].ToString();
            }

            set
            {
                ViewState["DuzenliBagisciId"] = value;
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
                if (!Page.IsPostBack)
                {
                    YonergeLnk.HRef = UtilityHelper.YonergeURLGetir(ProjeConstants.PARAM_NBYSYONERGE, ProjeConstants.NBYSBELGELERI_LIB, ProjeConstants.PAGE_DUZENLIBAGISCI_LIST);
                    if (DuzenliBagisciIdQS.ConvertToInt()>0 && NakitBagisciIdQS.ConvertToInt()>0)
                    {
                        //burada DuzenliNakitBagisci.BagisciId=NakitBagisciIdQS yap ve kaydet
                        DuzenliNakitBagisci dnb = new DuzenliNakitBagisci();
                        dnb = dnb.Select<DuzenliNakitBagisci>(DuzenliBagisciIdQS.ConvertToInt());
                        if (dnb != null)
                        {
                            dnb.BagisciId = NakitBagisciIdQS.ConvertToInt();
                            dnb.Update();
                        }
                        SecilenIdQS = DuzenliBagisciIdQS;
                    }
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        //////////////////////////////////////////////////////////////////////////////////
        private void TabloOlustur()
        {
            List<BagisciListItem> list = GetBagisciData();
            var serializer = new JavaScriptSerializer();
            var jsonData = serializer.Serialize(list);
            var jsString = CreateDataTable(jsonData); 
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
                DataTable dataTable = nakitBagisci.SelectDuzenliBagisci();
                if (dataTable != null)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int aBagisciId = row["aBagisciId"].ReturnZeroIfNull().ConvertToInt();
                        string aBagisciAdi = row["aBagisciAdi"].ReturnEmptyIfNull().ToString();
                        string aDuzenliBagisciId = row["aDuzenliBagisciId"].ReturnEmptyIfNull().ToString();
                        DateTime aBaslamaTarihi = row["aBaslamaTarihi"].ReturnEmptyIfNull().ConvertToDatetime();
                        decimal aTutar = row["aTutar"].ReturnZeroIfNull().ConvertToDecimal();
                        int aBagisAdedi = row["aBagisAdedi"].ReturnZeroIfNull().ConvertToInt();
                        decimal aBagisToplami = row["aBagisToplami"].ReturnZeroIfNull().ConvertToDecimal();
                        bool aAktif = row["aAktif"].ReturnFalseIfNull().ConvertToBool();
                        int aArmaganId = row["aArmaganId"].ReturnZeroIfNull().ConvertToInt();
                        int aNakitBagisHareketId = row["aNakitBagisHareketId"].ReturnZeroIfNull().ConvertToInt();
                        string aTelefon = row["aTelefon"].ReturnEmptyIfNull().ToString();
                        string aEposta = row["aEposta"].ReturnEmptyIfNull().ToString();
                        string aEslesmeBilgisi = row["aEslesmeBilgisi"].ReturnEmptyIfNull().ToString();
                        string aAciklama = row["aAciklama"].ReturnEmptyIfNull().ToString();
                        string bAdi = row["bAdi"].ReturnEmptyIfNull().ToString();
                        string bTCKimlikNo = row["bTCKimlikNo"].ReturnEmptyIfNull().ToString();
                        string bAdres = row["bAdres"].ReturnEmptyIfNull().ToString();
                        string bTelefon1 = row["bTelefon1"].ReturnEmptyIfNull().ToString();
                        string bTelefon2 = row["bTelefon2"].ReturnEmptyIfNull().ToString();
                        string bEposta = row["bEposta"].ReturnEmptyIfNull().ToString();
                        string bAciklama = row["bAciklama"].ReturnEmptyIfNull().ToString();
                        string bIl = row["bIl"].ReturnEmptyIfNull().ToString();
                        string bIlce = row["bIlce"].ReturnEmptyIfNull().ToString();
                        bool bUlasilamiyor = row["bUlasilamiyor"].ReturnFalseIfNull().ConvertToBool();
                        bool bBelgeIstemiyor = row["bBelgeIstemiyor"].ReturnFalseIfNull().ConvertToBool();
                        string bDurum = row["bDurum"].ReturnEmptyIfNull().ToString();



                        BagisciListItem listItem = new BagisciListItem();
                        listItem.aBagisciId = aBagisciId;
                        listItem.aBagisciAdi = aBagisciAdi;
                        listItem.aDuzenliBagisciId = aDuzenliBagisciId.ConvertToInt();
                        listItem.aBaslamaTarihi = aBaslamaTarihi;
                        listItem.aTutar = aTutar;
                        listItem.aBagisAdedi = aBagisAdedi;
                        listItem.aBagisToplami = aBagisToplami;
                        listItem.aAktif = aAktif;
                        listItem.aArmaganId = aArmaganId;
                        listItem.aNakitBagisHareketId = aNakitBagisHareketId;
                        listItem.aTelefon = aTelefon;
                        listItem.aEposta = aEposta;
                        listItem.aEslesmeBilgisi = aEslesmeBilgisi;
                        listItem.aAciklama = aAciklama;
                        listItem.bAdi = bAdi;
                        listItem.bTCKimlikNo = bTCKimlikNo;
                        listItem.bAdres = bAdres;
                        listItem.bTelefon1 = bTelefon1;
                        listItem.bTelefon2 = bTelefon2;
                        listItem.bEposta = bEposta;
                        listItem.bAciklama = bAciklama;
                        listItem.bIl = bIl;
                        listItem.bIlce = bIlce;
                        listItem.bUlasilamiyor = bUlasilamiyor;
                        listItem.bBelgeIstemiyor = bBelgeIstemiyor;
                        listItem.bDurum = bDurum;

                        list.Add(listItem);
                    }
                }
            }



            //string json = nakitBagisci.ToJSON(dataTable);
            return list;
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
            jQuery(document).ready(function () {
            jQuery.fn.dataTable.moment('DD.MM.YYYY');
            jQuery('#CustomDataTable').DataTable({
                'initComplete': function (settings, json) {//tablo yüklendiğinde
                    var api = this.api();
                    var row = api.row(function(idx, data, node) { //secilen satıra gider
                        return data['aDuzenliBagisciId'] ==" + SecilenIdQS + @";
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
                    { data: 'Adi',width: '15%'},
                    { data: 'bTCKimlikNo' },
                    { data: 'Tutar', width: '10%', className: 'text-end' },
                    { data: 'BaslamaTarihi', width: '10%'},
                    { data: 'Telefon' },
                    { data: 'Ili' },
                    { data: 'Ilcesi' },
                    { data: 'bAdres',width: '15%' },
                    { data: 'aAciklama' },
                    { data: 'DuzenliBagisciBelgesi', className: 'text-center' },
                    { data: 'BagisciEslestir', width: '12%', className: 'text-center' },
                ],
                'order': [[3, 'desc']],

                'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                    'decimal': ',',
                    'thousands': '.'
                },
                responsive: true,
                destroy: true,
                autoWidth: false,
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
                ],
                'createdRow': function(row, data, dataIndex) {
                    if (data.aBagisciId == -1) {
                        $(row).addClass('table-danger');
                    }
                }



            });
        });
        ";
            return tableString;
        }
        private class BagisciListItem
        {


            public int aBagisciId { get; set; }
            public string aBagisciAdi { get; set; }
            public int aDuzenliBagisciId { get; set; }
            public DateTime aBaslamaTarihi { get; set; }
            public decimal aTutar { get; set; }
            public int aBagisAdedi { get; set; }
            public decimal aBagisToplami { get; set; }
            public bool aAktif { get; set; }
            public int aArmaganId { get; set; }
            public int aNakitBagisHareketId { get; set; }
            public string aTelefon { get; set; }
            public string aEposta { get; set; }
            public string aEslesmeBilgisi { get; set; }
            public string aAciklama { get; set; }

            public string bAdi { get; set; }
            public string bTCKimlikNo { get; set; }
            public string bAdres { get; set; } = string.Empty;
            public string bTelefon1 { get; set; } = string.Empty;
            public string bTelefon2 { get; set; } = string.Empty;
            public string bEposta { get; set; } = string.Empty;
            public string bAciklama { get; set; } = string.Empty;
            public string bIl { get; set; } = string.Empty;
            public string bIlce { get; set; } = string.Empty;
            public bool bUlasilamiyor { get; set; }
            public bool bBelgeIstemiyor { get; set; }
            public string bDurum { get; set; }

            public string Adi
            {
                get {
                    int aBagisciId = this.aBagisciId;
                    string aBagisciAdi = this.aBagisciAdi;
                    string bBagisciAdi = this.bAdi;
                    string aEslesmeBilgisi = this.aEslesmeBilgisi;
                    string bAdi = this.bAdi;
                    string link = string.Empty;
                    if (aBagisciId == -1)
                    {
                        link = aBagisciAdi + "</br>" + aEslesmeBilgisi;
                    }
                    else
                    {
                        link = "<a href=# onclick=OpenModal('" + aBagisciId + "'); class='btn btn-link '> " + bAdi.Trim() + "</a> </br> (" +
                                   bBagisciAdi + " " + aEslesmeBilgisi + ")";
                    }
                    return link; 
                }
                set { aBagisciAdi = value; }
            }
            public string Tutar {                 
                get { return aTutar.ToString("N", new CultureInfo(ProjeConstants.CULTUREINFO, true)); }
                set { aTutar = value.ConvertToDecimal(); }
            }
            public string BaslamaTarihi
            {
                get { return aBaslamaTarihi.ToString("dd.MM.yyyy"); }
                set { aBaslamaTarihi = value.ConvertToDatetime(); }
            }
            public string Telefon
            {
                get
                {
                    string telefon = bTelefon1.Trim() + " " + bTelefon2.Trim()+ " (" + aTelefon.Trim()+")";
                    return telefon;
                }
                set { aTelefon = value; }
            }
            public string Ili
            {
                get { return bIl; }
                set { bIl = value; }
            }
            public string Ilcesi
            {
                get { return bIlce; }
                set { bIlce = value; }
            }
            public string DuzenliBagisciBelgesi
            {
                get
                {
                    int aBagisciId = this.aBagisciId;
                    int aArmaganId = this.aArmaganId;
                    DateTime aBaslamaTarihi = this.aBaslamaTarihi;
                    bool bBelgeIstemiyor = this.bBelgeIstemiyor;
                    //string bDurum = this.bDurum;
                    string link = string.Empty;
                    if (aBagisciId == -1)
                    {
                        link = "Bağışçı bulunamadı";
                    }
                    else if (bBelgeIstemiyor == true)
                    {
                        link = "Belge İstemiyor";
                    }
                    else
                    {
                        //Duzenli bağışçı belgesi oluşturulmuş mu, Armagan_Table'dan kontrol et
                        Armagan armagan= new Armagan();
                        armagan = armagan.SelectByBagisciIdAndArmaganTanimId(aBagisciId,ProjeConstants.ARMAGAN_DUZENLIBAGISCIBELGESIID);
                        if (armagan != null)
                        {
                            aArmaganId = armagan.Id;
                            bDurum = armagan.Durum;
                        }

                        if (aArmaganId > 0)
                        {
                            if (bDurum == ProjeConstants.DURUM_GONDERILDI)
                            {
                                link = "<a class='fw-bold text-success'>"+ ProjeConstants.DURUM_GONDERILDI+"</a>";
                            }
                            else
                            {
                                var baslamaTarihi = aBaslamaTarihi.AddMonths(12);
                                var secilenAy = baslamaTarihi.Month;
                                var secilenYil = baslamaTarihi.Year;
                                var queryStr = "&SecilenAy=" + secilenAy + "&SecilenYil=" + secilenYil + "&SecilenArmaganTanimId=" + ProjeConstants.ARMAGAN_DUZENLIBAGISCIBELGESIID
                                    + "&DuzenliBagisciId="+this.aDuzenliBagisciId;
                                link = "<a href='" + ProjeConstants.PAGE_ARMAGAN_EDIT + "?ArmaganId=" + aArmaganId + "&NakitBagisciId=" + aBagisciId + queryStr + "' class='btn btn-outline-primary' >Düzenle</a> </br>" + "(" + bDurum + ")";
                            }
                        }
                        else
                        {
                            link = "<button type=button class='btn btn-success' onclick=showDuzenliBagisOnay(" + aBagisciId + "," + aDuzenliBagisciId + ")>Düzenli Bağışçı Belgesi oluştur</button>";
                        }
                    }
                    return link;
                }
                set { aAciklama = value; }
            }
            public string BagisciEslestir
            {
                get
                {
                    string bDurum = this.bDurum;
                    int aBagisciId = this.aBagisciId;
                    string link = string.Empty;
                    //Duzenli bağışçı belgesi oluşturulmuş mu, Armagan_Table'dan kontrol et
                    Armagan armagan = new Armagan();
                    armagan = armagan.SelectByBagisciIdAndArmaganTanimId(aBagisciId, ProjeConstants.ARMAGAN_DUZENLIBAGISCIBELGESIID);
                    if (armagan != null)
                    {
                        aArmaganId = armagan.Id;
                        bDurum = armagan.Durum;
                    }
                    if (bDurum == ProjeConstants.DURUM_GONDERILDI)
                    {
                        return link; 
                    }

                    int aDuzenliBagisciId = this.aDuzenliBagisciId;
                    string aBagisciAdi = this.aBagisciAdi;
                    string bAdi = this.bAdi;
                    if (aBagisciId == -1)
                    {
                        link = "<a href='" + ProjeConstants.PAGE_NAKITBAGISCI_ESLESTIR + "?SenderApp=DNBL&DuzenliBagisciId=" + aDuzenliBagisciId + "&BagisciAdi=" + aBagisciAdi.Replace(" ", "@@") + "' class='btn btn-warning ' >Eşleştir</a>";
                    }
                    else
                    {
                        link = "<a href='" + ProjeConstants.PAGE_NAKITBAGISCI_EDIT + "?NakitBagisciId=" + aBagisciId + "' class='btn btn-outline-secondary ' >Bağışçı</a> " +
                                 " <a href='" + ProjeConstants.PAGE_NAKITBAGISCI_ESLESTIR + "?SenderApp=DNBL&DuzenliBagisciId=" + aDuzenliBagisciId + "&BagisciAdi=" + aBagisciAdi.Replace(" ", "@@") + "' class='btn btn-warning ' >Eşleştir</a>";
                    }
                    return link;
                }
                set { aAciklama = value; }
            }
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
                    { data: 'BagisMiktari', 'width': '10%', 'className': 'text-end' },
                    { data: 'Banka' },
                    { data: 'Armagan' },
                    { data: 'ArmaganTutari' },
                    { data: 'Durum' },
                ],
                'order': [[0, 'desc']],

                'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
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
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            try
            {

                TabloModalOlustur(paramNakitBagisciIdLbl.Value);
                NakitBagisciFormunuDoldur(paramNakitBagisciIdLbl.Value);
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

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
        protected void ArmaganOlusturBtn_Click(object sender, EventArgs e)
        {
            try
            {
                int nakitBagisciId = 0;
                int duzenliBagisciId = 0;
                if (!string.IsNullOrEmpty(hdnNakitBagisciId.Value))
                    nakitBagisciId = Convert.ToInt32(hdnNakitBagisciId.Value);
                if (!string.IsNullOrEmpty(hdnDuzenliBagisciId.Value))
                    duzenliBagisciId = Convert.ToInt32(hdnDuzenliBagisciId.Value);
                NakitBagisci nakitBagisci = new NakitBagisci();
                nakitBagisci = nakitBagisci.Select<NakitBagisci>(nakitBagisciId);
                if (nakitBagisci!=null)
                {
                    DuzenliNakitBagisci duzenliBagisci = new DuzenliNakitBagisci();
                    duzenliBagisci = duzenliBagisci.Select<DuzenliNakitBagisci>(duzenliBagisciId);
                    if (duzenliBagisci != null)
                    {
                        ArmaganOlustur(duzenliBagisci, nakitBagisci);
                    }
                }
                else
                {
                    Exception exception = new Exception(string.Format("HATA SATIRI {0}: Bağışçı bulunamadı -> NakitBagisciId: {1}", ProjeConstants.PAGE_DUZENLIBAGISCI_YUKLEME, nakitBagisciId));
                    ExceptionHelper exHelper = new ExceptionHelper(exception);
                    exHelper.PublishException();
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();
            }
            
        }
        public ExceptionHelper ArmaganOlustur(DuzenliNakitBagisci duzenliNakitBagisci, NakitBagisci bagisci)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            try
            {
                // Son nakitBagisii bul
                NakitBagisHareket nakitBagisHareket = new NakitBagisHareket();
                nakitBagisHareket = nakitBagisHareket.SelectBagisByBagisciIdTarih(duzenliNakitBagisci.BagisciId, duzenliNakitBagisci.BaslamaTarihi);
                //Armağan Oluştur
                if (nakitBagisHareket != null)
                {
                    duzenliNakitBagisci.NakitBagisHareketId = nakitBagisHareket.Id;
                    //Bu bağışçı daha önce Düzenli Bağışçı belgesi almış mı bak
                    Armagan armagan = new Armagan();
                    armagan = armagan.SelectByBagisciIdAndArmaganTanimId(bagisci.Id, ProjeConstants.ARMAGAN_DUZENLIBAGISCIBELGESIID);
                    if (armagan != null)
                    {
                        string aciklamaStr = string.IsNullOrEmpty(duzenliNakitBagisci.Aciklama) ? " Daha önce Düzenli Bağışçı Belgesi almış -> " + duzenliNakitBagisci.BagisciAdi : duzenliNakitBagisci.Aciklama;
                        Exception exception = new Exception(string.Format("HATA SATIRI {0}:{1} -> Bu bağışçı daha önce Düzenli Bağışçı Belgesi almış.", ProjeConstants.PAGE_DUZENLIBAGISCI_YUKLEME, aciklamaStr));
                        exceptionHelper.Exceptions.Add(exception);
                        return exceptionHelper;
                    }
                    int armaganId = EkstreAktarma.ArmaganiKaydet(nakitBagisHareket, bagisci, duzenliNakitBagisci.BagisToplami, ProjeConstants.ARMAGAN_DUZENLIBAGISCIBELGESIID, CurrentUserName);
                    if (armaganId < 1)
                    {
                        string aciklamaStr = string.IsNullOrEmpty(duzenliNakitBagisci.Aciklama) ? " Armağan kaydı oluşturulamadı -> " + duzenliNakitBagisci.BagisciAdi : duzenliNakitBagisci.Aciklama;
                        Exception exception = new Exception(string.Format("HATA SATIRI {0}:{1} -> Armağan kaydı oluşturulamadı.", ProjeConstants.PAGE_DUZENLIBAGISCI_YUKLEME, aciklamaStr));
                        exceptionHelper.Exceptions.Add(exception);
                    }
                    else
                    {
                        duzenliNakitBagisci.ArmaganId = armaganId;
                        nakitBagisHareket.ArmaganId = armaganId;
                        nakitBagisHareket.Update();
                    }
                    duzenliNakitBagisci.Update();
                    TabloOlustur();
                }

            }
            catch (Exception ex)
            {
                exceptionHelper.Exceptions.Add(ex);
            }
            return exceptionHelper;
        }
    }
}
