using DAO.Ortak;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
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
        private string MesajQS
        {
            get
            {

                if (ViewState["Mesaj"] == null)
                {
                    if (Page.Request.QueryString["Mesaj"] != null)
                    {
                        ViewState["Mesaj"] = Page.Request.QueryString["Mesaj"];
                    }
                    else
                    {
                        ViewState["Mesaj"] = string.Empty;
                    }
                }
                return ViewState["Mesaj"].ToString();
            }

            set
            {
                ViewState["Mesaj"] = value;
            }
        }
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(MesajQS))
                {
                    MessageHelper.PublishMessage("Bağış Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                    MesajQS = string.Empty;
                }
                if (!Page.IsPostBack)
                {
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
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

        }
        private string BagisHareketListesiJson()
        {
            NakitBagisHareket nbh = new NakitBagisHareket();
            var json = nbh.SelectByDurumTarihReturnJson(AyDDL.SelectedItem.Value, SecilenYilQS, ProjeConstants.IL_HEPSI);
            return json;
        }
        private void TabloOlustur()
        {
            var jsonData = BagisHareketListesiJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
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
                            'className': 'text-right' ,
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
                        targets: 7, render: function(data, type, row, meta) {
                        var link='<a href=# onclick=CallButtonClick('+data + '); class=\'btn btn-outline-danger \'>Kayıt Sil</a>'
                        return link;
                    }},

                    ],
                    'language': {
                        'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    destroy: true,
                    autoWidth: false,
                    stateSave: true, //son durumu hatırla hangi sayfada kaldın
                    dom: 'frtip',
                });
            });
            ";
            return tableString;
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SecilenAyQS = AyDDL.SelectedItem.Value.ToString();
                TabloOlustur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
                AyDDLDoldur();
                TabloOlustur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
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
                    //if (isArmaganSaved)
                    //{
                    //    RedirectToPage(ProjeConstants.PAGE_BAGISIADE+"?Mesaj=true&Param="+BagisAraTxt.Text);87
                    //    KayitGetir();
                    //}
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
        private void AyDDLDoldur()
        {
            AyDDL.Items.Clear();
            DateTime today = DateTime.Today;
            DateTime basay = new DateTime(SecilenYilQS.ConvertToInt(), 1, 1);
            DateTime bitay = new DateTime(SecilenYilQS.ConvertToInt(), 12, 1);


            DateTime listAy = basay;
            for (int i = 0; i < 12; i++)
            {
                AyDDL.Items.Add(new ListItem(listAy.ToString("MMMM"), listAy.ToString("MM")));
                listAy = listAy.AddMonths(1);
            }

        }
        private void YilDDLDoldur()
        {
            DateTime today = DateTime.Today;

            DateTime basyil = new DateTime(2018, 3, 1);
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
            nbh = nbh.Select<NakitBagisHareket>(paramBagisHareketIdLbl.Value.ConvertToInt());

            if (nbh != null)//bu bagis varsa
            {

                NakitBagisci nb = new NakitBagisci();
                nb = nb.Select<NakitBagisci>(nbh.BagisciId);
                string bagisciAdi = nb == null ? "" : nb.Adi + " " + nb.Soyadi + " tarafından bağışlanan ";
                string silmeMsg = bagisciAdi + nbh.BagisMiktari.ToString("N", culturInfo) + " " + nbh.DovizCinsi + " silinecek. (Hatırlatma: Silme yerine 'Bağış İadesi' de yapabilirsiniz.) ";
                Armagan armagan = new Armagan();
                armagan = armagan.Select<Armagan>(nbh.ArmaganId);

                string armaganiVarMsg = string.Empty;
                if (armagan != null) //bu armagan varsa
                {
                    ArmaganTanim at = new ArmaganTanim();

                    at = at.Select<ArmaganTanim>(armagan.ArmaganTanimId);
                    string armaganTanim = at == null ? "" : " Bu bağışa ait " + at.Armagan + " bulunmaktadır.";
                    armaganiVarMsg = armaganTanim + " (Armagan Durumu: '" + armagan.Durum + "') Onayladığınız takdirde bağış ve armağan silinecektir.";
                }
                SilmeMesajiLbl.Text = silmeMsg + armaganiVarMsg;
                ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "ModalOnay();", true);
            }
            else
            {
                SilmeSebebiTxt.Visible = false;
                SilmeMesajiLbl.Text = " Seçilen bağış bilgilerine ulaşılamadı";
                BagisSilNowBtn.Visible = false;
                ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "ModalOnay();", true);
            }
        }
        /// <summary>
        /// NakitBagisHareketKaydını sil
        /// SilinenKayit_Table'a yaz
        /// Bu bağışa karşılık bir Armagan  beratı var mı bak
        ///     varsa Armagan Kaydını Sil 
        ///     Armagan Gonderildi ise silme 
        ///     armagan silindi ise tekrar armagan Hesapla 
        /// Nakit bagiscinin başka bagişi yoksa Nakit Bagisciyi sil 
        ///     başka bagisi varsa silme
        /// </summary>
        private void SecilenBagisiSil()
        {
            string currentUser = UtilityHelper.GetCurrentUserLoginName();
            NakitBagisHareket nbh = new NakitBagisHareket();
            nbh = nbh.Select<NakitBagisHareket>(paramBagisHareketIdLbl.Value.ConvertToInt());
            if (nbh != null)
            {
                DbClass db = new DbClass();
                # region nbh table'dan sil
                DBObject nbhDbo = new DBObject();
                nbhDbo.SQLString = nbh.GetDeleteSQL("");
                nbhDbo.SQLType = ProjeConstants.SQL_DELETE;
                nbhDbo.IsFilled = true;
                db.DBObjectList.Add(nbhDbo);
                #endregion

                #region SilinenKayit_Table'a yaz
                SilinenKayit skNBH = new SilinenKayit();
                skNBH.Silen = currentUser;
                skNBH.SilinmeSebebi = SilmeSebebiTxt.Value;
                skNBH.TabloAdi = "NakitBagisHareket_Table";
                skNBH.SilinmeTarihi = DateTime.Now.ReturnTRDateFormat();
                skNBH.SilinenKayitBilgisi = " #BagisHareketId=" + nbh.Id + " #BagisciId=" + nbh.BagisciId + " #BagisTarihi=" + nbh.BagisTarihi + " #BagisMiktari=" + nbh.BagisMiktari;

                DBObject skNBHDbo = new DBObject();
                skNBHDbo.SQLString = skNBH.GetInsertSQL("");
                skNBHDbo.SQLType = ProjeConstants.SQL_INSERT;
                skNBHDbo.IsFilled = true;
                db.DBObjectList.Add(skNBHDbo);
                #endregion
                #region Armagan_Table dan sil
                DBObject skArmaganDbo = new DBObject();
                if (nbh.ArmaganId > 0)
                {
                    Armagan armagan = new Armagan();
                    armagan = armagan.Select<Armagan>(nbh.ArmaganId);
                    if (armagan != null)
                    {
                        #region armagan table'dan sil
                        DBObject armaganDbo = new DBObject();
                        armaganDbo.SQLString = armagan.GetDeleteSQL("");
                        armaganDbo.SQLType = ProjeConstants.SQL_DELETE;
                        armaganDbo.IsFilled = true;
                        db.DBObjectList.Add(armaganDbo);

                        //silinenKayit_Table'a yaz
                        SilinenKayit skArmagan = new SilinenKayit();
                        skArmagan.Silen = currentUser;
                        skArmagan.SilinmeSebebi = SilmeSebebiTxt.Value;
                        skArmagan.TabloAdi = "Armagan_Table";
                        skArmagan.SilinmeTarihi = DateTime.Now.ReturnTRDateFormat();
                        skArmagan.SilinenKayitBilgisi = " #ArmaganId=" + armagan.Id + " #BagisciId=" + armagan.BagisciId + " #BagisHareketId=" + nbh.Id + " #BagisTarihi=" + nbh.BagisTarihi + " #BagisMiktari=" + nbh.BagisMiktari;


                        skArmaganDbo.SQLString = skArmagan.GetInsertSQL("");
                        skArmaganDbo.SQLType = ProjeConstants.SQL_INSERT;
                        skArmaganDbo.IsFilled = true;
                        db.DBObjectList.Add(skArmaganDbo);
                        #endregion
                    }
                }

                #endregion
                List<DBObject> savedDBOList = db.ExecuteTransaction();

                if (savedDBOList.Count > 0)
                {
                    bool isArmaganYenidenHesaplandi = false;
                    bool isBagisSilindi = skNBHDbo.Success;
                    bool isBagisciSilindi = false;
                    string mesaj = string.Empty;
                    if (isBagisSilindi)
                    {
                        //bağış silindi ise bağışçının da başka bağışı yoksa bağışçıyı da sil
                        NakitBagisci bagisci = new NakitBagisci();
                        bagisci = bagisci.SelectBagisiOlmayanBagisciById(nbh.BagisciId);
                        if (bagisci != null)
                        {
                            //silinen bağışçı bilgilerini silinenKayit_Table'a yaz
                            SilinenKayit skBagisci = new SilinenKayit();
                            skBagisci.Silen = currentUser;
                            skBagisci.SilinmeSebebi = "Bağış silindiğinden";
                            skBagisci.TabloAdi = "NakitBagisci_Table";
                            skBagisci.SilinmeTarihi = DateTime.Now.ReturnTRDateFormat();
                            skBagisci.SilinenKayitBilgisi = " #BagisciId=" + bagisci.Id + " #Adı=" + bagisci.Adi + " #TCKimlikNo=" + bagisci.TCKimlikNo + " #Telefon=" + bagisci.Telefon1 + " " + bagisci.Telefon2 + " #Adres=" + bagisci.Adres;
                            skBagisci.Save();
                            isBagisciSilindi = bagisci.Delete();
                        }
                        if (skArmaganDbo.Success)//armagan tablosunda işlem oldu mu. //yeniden armağan hesaplanacak
                            isArmaganYenidenHesaplandi = TekrarArmaganHesapla(nbh, UtilityHelper.GetCurrentUserLoginName());
                        ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "CloseModal();", true);
                        MessageHelper.PublishMessage("Bağış Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                        RedirectToPage(ProjeConstants.PAGE_BAGISSIL + "?Mesaj=true");//+ BagisAraTxt.Text);
                        //KayitGetir();
                    }

                }
                if (!skNBHDbo.Success)
                {
                    MessageHelper.PublishMessage("Bağış Silinemedi", ProjeConstants.MESAJ_HATA);
                }

            }

        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {

        }
        protected void BagisSilBtn_Click(object sender, EventArgs e)
        {
            SilmedenOnceOnayAl();
        }
        protected void BagisSilNowBtn_Click(object sender, EventArgs e)
        {
            SecilenBagisiSil();
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
    }
}