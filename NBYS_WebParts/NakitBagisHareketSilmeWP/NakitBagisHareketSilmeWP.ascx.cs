using NBYS_WebParts.Logging;
using Utility.Logging;
using Model.NBYS;
using Model.Ortak;
using Model.Services.Ortak;
using Model.Services.NBYS;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.NakitBagisHareketSilmeWP
{
    [ToolboxItemAttribute(false)]
    public partial class NakitBagisHareketSilmeWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("NBYS", "NakitBagisHareketSilmeWP"); }
        }

        private void ExecuteAction(string operation, Action action)
        {
            NbysWebPartLog.ExecuteAction(LoggingContext, operation, action);
        }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NakitBagisHareketSilmeWP()
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
        private string SecilenAyQS
        {
            get
            {

                if (ViewState["SecilenAy"] == null)
                {
                    if (Page.Request.QueryString["SecilenAy"] != null)
                    {
                        ViewState["SecilenAy"] = Page.Request.QueryString["SecilenAy"];
                    }
                    else
                    {
                        ViewState["SecilenAy"] = string.Empty;
                    }
                }
                return ViewState["SecilenAy"].ToString();
            }

            set
            {
                ViewState["SecilenAy"] = value;
            }
        }
        private string SecilenYilQS
        {
            get
            {

                if (ViewState["SecilenYil"] == null)
                {
                    if (Page.Request.QueryString["SecilenYil"] != null)
                    {
                        ViewState["SecilenYil"] = Page.Request.QueryString["SecilenYil"];
                    }
                    else
                    {
                        ViewState["SecilenYil"] = string.Empty;
                    }
                }
                return ViewState["SecilenYil"].ToString();
            }

            set
            {
                ViewState["SecilenYil"] = value;
            }
        }

        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
                if (!Page.IsPostBack)
                {
                    YonergeLnk.HRef = UtilityHelper.YonergeURLGetir(ProjeConstants.PARAM_NBYSYONERGE, ProjeConstants.NBYSBELGELERI_LIB, ProjeConstants.PAGE_BAGISSIL, LoggingContext);
                    YilDDLDoldur();
                    string buYil = DateTime.Today.Year.ToString();
                    SecilenYilQS = string.IsNullOrEmpty(SecilenYilQS) ? buYil : SecilenYilQS;
                    UtilityHelper.SetDDLValue(YilDDL, SecilenYilQS);
                    AyDDLDoldur();
                    string buAy = DateTime.Today.ToString("MM");
                    SecilenAyQS = string.IsNullOrEmpty(SecilenAyQS) ? buAy : SecilenAyQS;
                    UtilityHelper.SetDDLValue(AyDDL, SecilenAyQS);
                    TabloOlustur();
                }
            });
        }
        private string BagisHareketListesiJson()
        {
            NakitBagisHareket nbh = new NakitBagisHareket();
            var json = new NakitBagisHareketService(LoggingContext).ListByDurumTarihJson(AyDDL.SelectedItem.Value, SecilenYilQS, ProjeConstants.IL_HEPSI);
            return json;
        }
        private void TabloOlustur()
        {
            var jsonData = BagisHareketListesiJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
            jQuery(document).ready(function () {
                jQuery('#CustomDataTable').DataTable({
                    data: " + jsonData + @",
                    columns: [
                        { data: 'Adi','width': '20%' },
                        { data: 'TCKimlikNo' },
                        { data: 'BagisMiktari',
                            'width': '10%',
                            'className': 'text-end' ,
                            //render: $.fn.dataTable.render.number( '.', ',', 2 ) //format money as ###.###,00
                        },
                        { data: 'BagisTarihi' },
                        { data: 'Banka' },
                        { data: 'Ili' },
                        { data: 'Adres','width': '30%' },
                        { data: 'BagisId' },
                    ],
                    'order': [[3, 'desc']],
                    columnDefs:
                    [
                    {
                        targets: 0, render: function(data, type, row, meta) {
                        var link= '<a href=# onclick=OpenModal('+row.BagisciId+'); class=\'btn btn-link \'>'+row.Adi.trim() + '</a>';
                        return link;
                    }},
                    {
                        targets: 3, render: function(data, type, row, meta) {
                        return moment(data).format('DD.MM.YYYY');
                    }},
                    {
                    targets: 7,
                    render: function(data, type, row, meta) {
                        var today = moment();
                        var isLinkVisible =today.diff(row.BagisTarihi, 'days') <= 240; // 240 günden eski kayitlar için linki gizle

                        if (isLinkVisible) {
                            return '<a href=""#"" onclick=""CallButtonClick(' + data + ');"" class=""btn btn-outline-danger"">Kayit Sil</a>';
                        } else {
                            return 'Silme süresi dolmuş';
                        }
                    }
                },

                    ],
                    'language': {
                        'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    destroy: true,
                    autoWidth: false,
                    stateSave: true, //son durumu hatirla hangi sayfada kaldin
                    dom: 'frtip',
                });
            });
            ";
            return tableString;
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("AyDDL_SelectedIndexChanged", () =>
            {
                SecilenAyQS = AyDDL.SelectedItem.Value.ToString();
                TabloOlustur();
            });
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("YilDDL_SelectedIndexChanged", () =>
            {
                SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
                AyDDLDoldur();
                TabloOlustur();
            });
        }

        private void AyDDLDoldur()
        {
            AyDDL.Items.Clear();
            DateTime today = DateTime.Today;
            DateTime basay = DateTime.Today.AddMonths(-12); // Baslangiç ayi
            DateTime bitay = today; // Bitis ayi


            DateTime listAy = basay;
            while (listAy <= bitay)
            {
                AyDDL.Items.Add(new ListItem(listAy.ToString("MMMM"), listAy.ToString("MM")));
                listAy = listAy.AddMonths(1);
            }

        }
        private void YilDDLDoldur()
        {
            DateTime today = DateTime.Today;

            DateTime basyil = today.AddMonths(-1);
            DateTime bityil = DateTime.Today;
            int fark = bityil.Year - basyil.Year;
            int yil = basyil.Year;
            for (int i = 0; i <= fark; i++)
            {
                YilDDL.Items.Add(new ListItem(yil.ToString()));
                yil++;
            }

        }
        private void SilmedenOnceOnayAl()
        {


            NakitBagisHareket nbh = new NakitBagisHareket();
            nbh = new NakitBagisHareketService(LoggingContext).GetById(paramBagisHareketIdLbl.Value.ConvertToInt());

            if (nbh != null)//bu bagis varsa
            {

                NakitBagisci nb = new NakitBagisciService(LoggingContext).GetById(nbh.BagisciId);
                string bagisciAdi = nb == null ? "" : nb.Adi + " " + nb.Soyadi + " tarafından bağışlanan ";
                string silmeMsg = bagisciAdi + nbh.BagisMiktari.ToString("N", culturInfo) + " " + nbh.DovizCinsi + " silinecek. (Hatırlatma: Silme yerine 'Bağış İadesi' de yapabilirsiniz.) ";
                Armagan armagan = new Armagan();
                armagan = new ArmaganService().GetById(nbh.ArmaganId, LoggingContext);

                string armaganiVarMsg = string.Empty;
                if (armagan != null) //bu armagan varsa
                {
                    ArmaganTanim at = new ArmaganTanimService().GetById(armagan.ArmaganTanimId, LoggingContext);
                    string armaganTanim = at == null ? "" : " Bu bağışa ait " + at.Armagan + " bulunmaktadır.";
                    if (armagan.Durum == ProjeConstants.DURUM_GONDERILMEDI)
                    {
                        armaganiVarMsg = armaganTanim + " (Armağan Durumu: '" + armagan.Durum + "'). Onayladığınızda başka kayda bağlı olmayan armağan da silinecektir.";
                    }
                    else
                    {
                        armaganiVarMsg = armaganTanim + " Armağan Durumu: '" + armagan.Durum + "' olduğundan, silmeniz durumunda bu armağan silinmeyecektir.";
                    }
                }
                SilmeMesajiLbl.Text = silmeMsg + armaganiVarMsg;
                UtilityHelper.ScriptCalistir( "ModalOnay();");
            }
            else
            {
                SilmeSebebiTxt.Visible = false;
                SilmeMesajiLbl.Text = " Seçilen bağış bilgilerine ulaşılamadı";
                BagisSilNowBtn.Visible = false;
                UtilityHelper.ScriptCalistir("ModalOnay();");

            }
        }
        // Business data, references, archives and audit commit together.
        private void SecilenBagisiSil()
        {
            NbysDonationWriteResult result = new NbysDonationWriteService(LoggingContext).Delete(
                paramBagisHareketIdLbl.Value.ConvertToInt(), SilmeSebebiTxt.Value);
            NbysDonationFeedback.Show(LoggingContext, result, "Bağış silindi.", () =>
            {
                UtilityHelper.ScriptCalistir("CloseModal();");
                TabloOlustur();
            });
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () =>
            {

            });
        }
        protected void BagisSilBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("BagisSilBtn_Click", () =>
            {
                SilmedenOnceOnayAl();
            });
        }
        protected void BagisSilNowBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("BagisSilNowBtn_Click", () =>
            {
                SecilenBagisiSil();
            });
        }
        protected void BagisHareketListesiBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("BagisHareketListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_ARMAGAN_LIST);
            });
        }
        protected void ArmaganListesiBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ArmaganListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_BAGISHAREKET_LIST);
            });
        }
        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl);
        }
        private void TabloModalOlustur(string nakitBagisciId)
        {
            var jsonData = GetModalDataJson(nakitBagisciId); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazirlaniyor.
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
            int rowCount = 0;
            var json = new NakitBagisHareketService(LoggingContext).GetDonorDetailJson(nakitBagisciId.ConvertToInt(), ref rowCount);
            decimal toplamTutar = new NakitBagisHareketService(LoggingContext).GetTotalByBagisciIdDateRange(
                ProjeConstants.BAGIS_SORGU_BASTAR.ConvertToDatetime(), DateTime.Today, nakitBagisciId.ConvertToInt());
            BagisBilgileriLbl.Text = rowCount < 1 ? "Bağış bulunmamaktadır" :
                "Bağışçının " + rowCount + " defada yaptığı toplam " + toplamTutar.ToString("N", culturInfo) + "TL bağışı bulunmaktadır";
            return json;
        }
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ModalDoldurBtn_Click", () =>
            {
                TabloModalOlustur(paramNakitBagisciIdLbl.Value);
                NakitBagisciFormunuDoldur(paramNakitBagisciIdLbl.Value);
            });
        }
        private void NakitBagisciFormunuDoldur(string nakitBagisciIdStr)
        {
            if (!string.IsNullOrEmpty(nakitBagisciIdStr))
            {
                int nakitBagisciId = nakitBagisciIdStr.ConvertToInt();

                NakitBagisci nakitBagisci = new NakitBagisciService(LoggingContext).GetById(nakitBagisciId);
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
                    Il il = new IlService().GetById(ilId, LoggingContext);
                    if (il != null)
                    {

                        IlIlceCell.Text = il.IlAdi.ReturnEmptyIfNull().ToString();
                    }
                    int ilceId = nakitBagisci.Ilcesi.ConvertToInt();
                    Ilce ilce = new IlceService().GetById(ilceId, LoggingContext);
                    if (ilce != null)
                    {

                        IlIlceCell.Text += " " + ilce.IlceAdi.ReturnEmptyIfNull().ToString();
                    }
                    TelefonCell.Text = nakitBagisci.Telefon1.ReturnEmptyIfNull().ToString();
                    TuzelKisiCell.Text = nakitBagisci.TuzelKisi.ConvertToBool() ? "Evet" : "Hayir";
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
    }
}
