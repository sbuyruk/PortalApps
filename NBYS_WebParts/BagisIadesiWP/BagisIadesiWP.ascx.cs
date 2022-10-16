using DAO.Ortak;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.BagisIadesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BagisIadesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BagisIadesiWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string ParamQS//nakit bagisci düzenlemeden dönüyorsa aranan texti tekrar arasın
        {
            get
            {

                if (ViewState["Param"] == null)
                {
                    if (Page.Request.QueryString["Param"] != null)
                    {
                        ViewState["Param"] = Page.Request.QueryString["Param"];
                    }
                    else
                    {
                        ViewState["Param"] = string.Empty;
                    }
                }
                return ViewState["Param"].ToString();
            }

            set
            {
                ViewState["Param"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty(ParamQS))
                    {
                        BagisAraTxt.Text = ParamQS;
                        TabloOlustur();
                    }
                }
                
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

        }
        private List<BagisHareketListItem> GetDataList()
        {
            List<BagisHareketListItem> list = new List<BagisHareketListItem>();
            NakitBagisHareket bagisHareketDao = new NakitBagisHareket();
            if (!string.IsNullOrEmpty(BagisAraTxt.Text))
            {
                DateTime gecenYilinIlkGunu = new DateTime(DateTime.Today.Year - 5, 1, 1);
                DataTable dataTable = bagisHareketDao.SelectByFilter(BagisAraTxt.Text, gecenYilinIlkGunu);

                IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
                DataView dataView = new DataView(dataTable);
                foreach (DataRowView row in dataView)
                {

                    string bagisHareketId = row["BagisHareketId"].ToString();
                    string armaganId = row["ArmaganId"].ToString();
                    DateTime tarih = row["BagisTarihi"].ReturnEmptyIfNull().ConvertToDatetime();
                    decimal bagisMiktari = row["BagisMiktari"].ConvertToDecimal();
                    string dovizCinsi = row["DovizCinsi"].ToString();
                    string armagan = row["Armagan"].ToString();
                    string armaganDurumu = row["Durum"].ToString();
                    string bagisciId = row["BagisciId"].ToString();
                    string bagisciAdi = row["BagisciAdi"].ToString();
                    string telefon = row["Telefon"].ToString();
                    string ili = row["Ili"].ToString();
                    string ilcesi = row["Ilcesi"].ToString();
                    string adres = row["Adres"].ToString();
                    bool iadeEdildiMi = row["IadeEdildiMi"].ReturnFalseIfNull().ConvertToBool();
                    decimal iadeMiktari = row["IadeMiktari"].ConvertToDecimal();

                    BagisHareketListItem bagisHareketItem = new BagisHareketListItem
                    {
                        BagisHareketId = bagisHareketId,
                        ArmaganId = armaganId.ToString(),
                        BagisTarihi = tarih.ConvertToDatetimeEmptyIfNull(),
                        BagisMiktari = bagisMiktari.ToString("N", culturInfo),
                        DovizCinsi = dovizCinsi,
                        Armagan = armagan,
                        Durum = armaganDurumu,
                        BagisciId = bagisciId,
                        BagisciAdi = "<a href=# onclick=OpenModal(" + bagisciId + "); class=\'text-link \'>" + bagisciAdi + "</a>",
                        Telefon = telefon,
                        Ili = ili,
                        Ilcesi = ilcesi,
                        Adres = "- " + adres,
                        IadeEdildiMi = iadeEdildiMi,
                        IadeMiktari = iadeMiktari.ToString("N", culturInfo)
                    };
                    string iadeLink;
                    if (iadeEdildiMi )
                    {
                        iadeLink = "<span class=bagis-iade-edildi>" + bagisHareketItem.IadeMiktari + " "+ dovizCinsi + " Parası İade edildi</span>";
                    }
                    else
                    {
                        iadeLink = "<a href=# onclick=CallButtonClick(" + bagisHareketId + "); class=\'btn btn-outline-danger \'>Parayı İade Et</a>";
                    }
                    bagisHareketItem.IadeLink = iadeLink;
                    list.Add(bagisHareketItem);

                }

            }
            return list;
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
            BagisciSecTableDiv.Attributes["style"] = "display:block";
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {

                List<BagisHareketListItem> list = GetDataList();
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                jSon = serializer.Serialize(list);
                return jSon;

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
        jQuery(document).ready(function () {
            if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                jQuery('#CustomDataTable').DataTable().destroy();
            }
            jQuery('#CustomDataTable tbody').empty();

            jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
            jQuery('#CustomDataTable').DataTable({
                data: " + jsonData + @",
                columns: [
                    { data: 'BagisciAdi' },
                    { data: 'BagisMiktari', 'width': '10%', 'className': 'text-right' },
                    { data: 'BagisTarihi' },
                    { data: 'Armagan' },
                    { data: 'Durum' },
                    { data: 'Telefon' },
                    { data: 'Adres' },
                    { data: 'IadeLink' },

                ],
                'order': [[2, 'desc']],

                'language': {
                    'url': 'http://tskgv-portal/OrtakBelgeler/Turkish.txt',
                    'decimal': ',',
                    'thousands': '.'
                },
                responsive: true,
                dom: 'rtip',

            });
        });
        ";
            return tableString;
        }
        private bool TekrarArmaganHesapla(NakitBagisHareket silinenNbh, string currentUser)
        {
            bool isArmaganSaved = false;
            NakitBagisHareket nbhQuery = new NakitBagisHareket();
            List<NakitBagisHareket> nbhList = nbhQuery.SelectByArmaganId(silinenNbh.ArmaganId);
            //eger hala bu bagiscinin yaptığı nbh varsa yeniden armagan hesapla
            if (nbhList.Count > 0)
            {
                NakitBagisHareket kalanNbh1 = nbhList[0];
                NakitBagisci kalanNakitBagisci = new NakitBagisci();
                kalanNakitBagisci = kalanNakitBagisci.Select<NakitBagisci>(kalanNbh1.BagisciId);

                try
                {
                    isArmaganSaved = EkstreAktarma.SaveArmagan(silinenNbh.BagisTarihi, kalanNakitBagisci.TuzelKisi, kalanNakitBagisci.Id, silinenNbh.Id, currentUser);//Armagan tablosuna aktarım
                }
                catch (Exception exception)
                {
                    ExceptionHelper ex = new ExceptionHelper(exception);
                    ex.PublishException();
                    TabloOlustur();
                }
            }
            return isArmaganSaved;

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

        private void TabloModalOlustur(string nakitBagisciId)
        {
            var jsonData = GetModalDataJson(nakitBagisciId); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
            BagisciSecTableDiv.Attributes["style"] = "display:block";
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
            NakitBagisHareket nbh = new NakitBagisHareket();
            int rowCount = 0;
            var json = nbh.SelectByBagisciIdReturnJSon(nakitBagisciId, ref rowCount);
            //RowCountLbl.Text = rowCount.ToString();
            return json;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {

        }
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            try
            {
                NakitBagisciFormunuDoldur(paramNakitBagisciIdLbl.Value);
                TabloModalOlustur(paramNakitBagisciIdLbl.Value);
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }
        }
        protected void BagisAraTxt_TextChanged(object sender, EventArgs e)
        {
            ParamQS = BagisAraTxt.Text;
            TabloOlustur();
        }
        protected void BagisiIadeEtBtn_Click(object sender, EventArgs e)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            NakitBagisHareket nbh = new NakitBagisHareket();
            nbh = nbh.Select<NakitBagisHareket>(paramBagisHareketIdLbl.Value.ConvertToInt());

            if (nbh != null)//bu bagis varsa
            {

                NakitBagisci nb = new NakitBagisci();
                nb = nb.Select<NakitBagisci>(nbh.BagisciId);
                string bagisciAdi = nb == null ? "" : nb.Adi + " " + nb.Soyadi + " tarafından bağışlanan ";
                string iadeMiktariMsg = bagisciAdi + nbh.BagisMiktari.ToString("N", culturInfo) + " " + nbh.DovizCinsi + " İade edilecek. ";
                Armagan armagan = new Armagan();
                armagan = armagan.Select<Armagan>(nbh.ArmaganId);

                string armaganiVarMsg = string.Empty;
                if (armagan != null) //bu armagan varsa
                {
                    ArmaganTanim at = new ArmaganTanim();

                    at = at.Select<ArmaganTanim>(armagan.ArmaganTanimId);
                    string armaganTanim = at == null ? "" : " Bu bağışa ait " + at.Armagan + " bulunmaktadır.";
                    armaganiVarMsg = armaganTanim + " (Armagan Durumu: '" + armagan.Durum + "') Onayladığınız takdirde armağan geçersiz hale getirilecek ve Para iade edilecektir.";
                }
                IadeMesajiLbl.Text = iadeMiktariMsg + armaganiVarMsg;
                ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "ParaIadeModalOnay();", true);
            }
            else
            {
                IadeSebebiTxt.Visible = false;
                IadeMesajiLbl.Text = " Seçilen bağış bilgilerine ulaşılamadı";
                BagisiIadeEtNowBtn.Visible = false;
                ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "ParaIadeModalOnay();", true);
            }
        }
        protected void BagisiIadeEtNowBtn_Click(object sender, EventArgs e)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            try
            {
                //nakit bagis hareket kaydını pasif yap
                //
                //Armagan kaydını pasif yap

                NakitBagisHareket nbh = new NakitBagisHareket();
                nbh = nbh.Select<NakitBagisHareket>(paramBagisHareketIdLbl.Value.ConvertToInt());



                if (nbh != null)
                {
                    NakitBagisci nb = new NakitBagisci();
                    nb = nb.Select<NakitBagisci>(nbh.BagisciId);
                    if (nb != null)
                    {
                        nbh.IadeEdildiMi = true;
                        nbh.IadeMiktari = nbh.BagisMiktari;
                        nbh.IadeTarihi = DateTime.Today;
                        nbh.IadeSebebi = IadeSebebiTxt.Value;
                        nbh.BagisMiktari = 0;
                        nbh.Aciklama = nbh.Aciklama + nb.Adi + " tarfından " + nbh.BagisTarihi.ConvertToDatetimeEmptyIfNull() + " tarihinde yapılan " +
                            nbh.BagisMiktari.ToString("N", culturInfo) + " " + nbh.DovizCinsi + " Bağış iade edilmiştir.";
                        nbh.IadeEden = UtilityHelper.GetCurrentUser();

                        DbClass db = new DbClass();
                        //herbir nesne için bir dbo yarat
                        //önce nakitbağış hareket
                        DBObject nbhDbo = new DBObject();
                        nbhDbo.SQLString = nbh.GetUpdateSQL("");
                        nbhDbo.SQLType = ProjeConstants.SQL_UPDATE;
                        nbhDbo.IsFilled = true;
                        if (nbhDbo.IsFilled)
                            db.DBObjectList.Add(nbhDbo);
                        //sonra armağan
                        DBObject armaganDbo = new DBObject();
                        //guncellenecek alanları nesnelerde guncelle
                        Armagan armagan = new Armagan();
                        armagan = armagan.Select<Armagan>(nbh.ArmaganId);
                        if (armagan != null)//armagan varsa
                        {
                            armagan.BelgeGecersizMi = ProjeConstants.TRUE_INT;
                            armagan.GecersizYapan = UtilityHelper.GetCurrentUser();
                            armagan.GecersizYapmaTarihi = DateTime.Now;
                            armagan.GecersizNBHareketId = nbh.Id;
                            armagan.Durum = ProjeConstants.DURUM_PARAIADE;
                            armagan.ArmaganBagisMiktari = armagan.BagisMiktari; //bağış iadesi öncesi armagana hak zandığı tutar
                            armagan.IadeMiktari = armagan.IadeMiktari + nbh.IadeMiktari;//iade edilen tutar
                            armagan.BagisMiktari = 0;//bağış miktarını sıfır yap
                            armagan.Aciklama = armagan.Aciklama + nb.Adi + " adlı bağışçıya ait armağan iade edilmiştir. ";
                            armaganDbo.SQLString = armagan.GetUpdateSQL("");
                            armaganDbo.SQLType = ProjeConstants.SQL_UPDATE;
                            armaganDbo.UseReturnIdAsParam = true;
                            armaganDbo.DbObjectParamIndex = 0;
                            armaganDbo.IsFilled = true;
                        }

                        if (armaganDbo.IsFilled)
                            db.DBObjectList.Add(armaganDbo);

                        //hazırlanan sorguları çalıştır
                        List<DBObject> savedDBOList = db.ExecuteTransaction();

                        if (savedDBOList.Count > 0)
                        {
                            if (armaganDbo.Success)//armagan tablosunda işlem oldu mu. //yeniden armağan hesaplanacak
                                TekrarArmaganHesapla(nbh, UtilityHelper.GetCurrentUser());
                            RedirectToPage(ProjeConstants.PAGE_BAGISIADE + "?Param=" + BagisAraTxt.Text);
                        }
                    }
                    else
                    {
                        MessageHelper.PublishMessage(" Bağışçı bulunamadı. Para İadesi Yapılamadı. ", ProjeConstants.MESAJ_HATA);
                        TabloOlustur();
                    }
                }
                else
                {
                    MessageHelper.PublishMessage("Bağış kaydı bulunamadı. Para İadesi Yapılamadı.", ProjeConstants.MESAJ_HATA);
                    TabloOlustur();
                }

            }
            catch (Exception ex)
            {
                ExceptionHelper exh = new ExceptionHelper();
                exh.Exceptions.Add(new Exception("Armağan durumu kaydedilirken hata oluştu. "));
                exh.Exceptions.Add(ex);
                exh.PublishException();
            }
        }
        protected void BagisHareketListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_ARMAGAN_LIST);
        }
        protected void ArmaganListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_BAGISHAREKET_LIST);
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
        private class BagisHareketListItem
        {
            public string BagisHareketId { get; set; }
            public string ArmaganId { get; set; }
            public string BagisTarihi { get; set; }
            public string BagisMiktari { get; set; }
            public string DovizCinsi { get; set; }
            public string Armagan { get; set; }
            public string Durum { get; set; }
            public string BagisciId { get; set; }
            public string BagisciAdi { get; set; }
            public string Telefon { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Adres { get; set; }
            public bool IadeEdildiMi { get; set; }
            public string IadeMiktari { get; set; }
            public string IadeLink { get; set; }
        }

        protected void AraBtn_Click(object sender, EventArgs e)
        {
            ParamQS = BagisAraTxt.Text;
            TabloOlustur();
        }
    }
}