using Model.IKYS;
using Model.MTS;
using Model.NBYS;
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

namespace MTS_WebParts.AramaGorusmeListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class AramaGorusmeListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public AramaGorusmeListesiWP()
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
                        ViewState["SecilenId"] = string.Empty;
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }
        private string ArayanIdQS
        {
            get
            {

                if (ViewState["ArayanId"] == null)
                {
                    if (Page.Request.QueryString["ArayanId"] != null)
                    {
                        ViewState["ArayanId"] = Page.Request.QueryString["ArayanId"];
                    }
                    else
                    {
                        ViewState["ArayanId"] = string.Empty;
                    }
                }
                return ViewState["ArayanId"].ToString();
            }

            set
            {
                ViewState["ArayanId"] = value;
            }
        }
        private string KatilimciTipiQS
        {
            get
            {

                if (ViewState["KatilimciTipi"] == null)
                {
                    if (Page.Request.QueryString["KatilimciTipi"] != null)
                    {
                        ViewState["KatilimciTipi"] = Page.Request.QueryString["KatilimciTipi"];
                    }
                    else
                    {
                        ViewState["KatilimciTipi"] = string.Empty;
                    }
                }
                return ViewState["KatilimciTipi"].ToString();
            }

            set
            {
                ViewState["KatilimciTipi"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BitisTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                BaslangicTarihiTxt.Text = DateTime.Today.AddYears(-1).ConvertToDatetimeEmptyIfNull();
                AramaGorusmeDDLDoldur();
                KatilimciBilgileriniDoldur();

            }
            TabloOlustur();
        }

        private void KatilimciBilgileriniDoldur()
        {
            switch (KatilimciTipiQS.ConvertToInt())
            {
                case ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI_INT:
                    {
                        TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
                        tasinmazBagisci = tasinmazBagisci.Select<TasinmazBagisci>(ArayanIdQS.ConvertToInt());
                        if (tasinmazBagisci != null)
                        {
                            AdiSoyadiLnk.Text = (tasinmazBagisci.Adi + " " + tasinmazBagisci.Soyadi).Trim() + " (" + ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI + ")";
                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/";
                            AdiSoyadiLnk.NavigateUrl = newUrl + ProjeConstants.PAGE_KISI_KARTI + "?KatilimciId=" + ArayanIdQS + "&KatilimciTipi=" + KatilimciTipiQS;

                        }
                        else
                        {
                            MessageHelper.PublishMessage("Taşınmaz Bağışçı Bulunamadı", ProjeConstants.MESAJ_HATA);
                        }
                        break;
                    }

                case ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI_INT:
                    {
                        NakitBagisci nakitBagisci = new NakitBagisci();
                        nakitBagisci = nakitBagisci.Select<NakitBagisci>(ArayanIdQS.ConvertToInt());
                        if (nakitBagisci != null)
                        {
                            AdiSoyadiLnk.Text = (nakitBagisci.Adi + " " + nakitBagisci.Soyadi).Trim() + " (" + ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI + ")";
                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/";
                            AdiSoyadiLnk.NavigateUrl = newUrl + ProjeConstants.PAGE_KISI_KARTI + "?KatilimciId=" + ArayanIdQS + "&KatilimciTipi=" + KatilimciTipiQS;

                        }
                        else
                        {
                            MessageHelper.PublishMessage("Nakit Bağışçı Bulunamadı", ProjeConstants.MESAJ_HATA);
                        }
                        break;
                    }
                case ProjeConstants.FAALIYET_KATILIMCI_DIS_INT:
                    {
                        Kisi kisi = new Kisi();
                        kisi = kisi.Select(ArayanIdQS.ConvertToInt());
                        if (kisi != null)
                        {
                            MTSKurumGorev kurumGorev = new MTSKurumGorev();
                            string kurum = string.Empty;
                            string gorev = string.Empty;
                            string kurumGorevStr = kurumGorev.SelectByKisiIdReturnKurumGorev(kisi.Id, ref kurum, ref gorev);
                            if (string.IsNullOrEmpty(kurumGorevStr))
                            {
                                kurumGorevStr = kisi.Kurumu + " / " + kisi.Gorevi;
                            }
                            AdiSoyadiLnk.Text = (kisi.Adi + " " + kisi.Soyadi).Trim() + " (" + kurumGorevStr + " " + kisi.Unvani + ")";
                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/";
                            AdiSoyadiLnk.NavigateUrl = newUrl + ProjeConstants.PAGE_KISI_KARTI + "?KatilimciId=" + ArayanIdQS + "&KatilimciTipi=" + KatilimciTipiQS;

                        }
                        else
                        {
                            MessageHelper.PublishMessage("Kişi Bulunamadı", ProjeConstants.MESAJ_HATA);
                        }
                        break;
                    }
                case ProjeConstants.FAALIYET_KATILIMCI_IC_INT:
                    {
                        string gorevi = string.Empty;
                        Personel personel = new Personel();
                        personel = personel.SelectCalisanPersonel(ArayanIdQS.ConvertToInt());
                        if (personel != null)
                        {
                            #region İş bilgileri
                            IsBilgileri isBilgisi = new IsBilgileri();
                            isBilgisi = isBilgisi.SelectByPersonelId(personel.Id);
                            if (isBilgisi != null)
                            {
                                GorevTanim gt = new GorevTanim();
                                gt = gt.Select<GorevTanim>(isBilgisi.GorevId);
                                gorevi = gt != null ? gt.Adi : "";
                            }
                            AdiSoyadiLnk.Text = (personel.Adi + " " + personel.Soyadi).Trim() + " (" + gorevi + ")";
                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/";
                            AdiSoyadiLnk.NavigateUrl = newUrl + ProjeConstants.PAGE_KISI_KARTI + "?KatilimciId=" + ArayanIdQS + "&KatilimciTipi=" + KatilimciTipiQS;
                            #endregion

                        }
                        else
                        {
                            MessageHelper.PublishMessage("Personel Bulunamadı", ProjeConstants.MESAJ_HATA);
                        }
                        break;
                    }
            }
        }

        private void AramaGorusmeDDLDoldur()
        {
            ListItem li = new ListItem(ProjeConstants.HEPSI);
            ListItem li1 = new ListItem(ProjeConstants.ARAMAGORUSME_GELENTELEFON);
            ListItem li2 = new ListItem(ProjeConstants.ARAMAGORUSME_GIDENTELEFON);
            ListItem li3 = new ListItem(ProjeConstants.ARAMAGORUSME_YUZYUZEGORUSME);
            ListItem li4 = new ListItem(ProjeConstants.ARAMAGORUSME_YONETICIDIREKTIFI);
            GorusmeSekliDDL.Items.Clear();
            GorusmeSekliDDL.Items.Add(li);
            GorusmeSekliDDL.Items.Add(li1);
            GorusmeSekliDDL.Items.Add(li2);
            GorusmeSekliDDL.Items.Add(li3);
            GorusmeSekliDDL.Items.Add(li4);
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                int index = currentUrl.LastIndexOf("?") < 0 ? currentUrl.Length : currentUrl.LastIndexOf("?");

                string rawUrl = currentUrl.Substring(0, index);
                string newUrl = rawUrl.Substring(0, rawUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        private string GetJson()
        {
            string jSon = string.Empty;

            try
            {
                List<AramaListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
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
        private void TabloOlustur()
        {
            var jsonData = GetJson(); //veri çekilip json a çeviriliyor
            UtilityHelper.ScriptCalistir("setDataSet(" + jsonData + ");");
        }
        private List<AramaListItem> GetDataList()
        {
            DateTime bitis = BitisTarihiTxt.Text.ConvertToDatetime();
            bitis = UtilityHelper.TariheSaatEkle(bitis, "23:59");
            BaslikLbl.InnerText = BaslangicTarihiTxt.Text + " - " + BitisTarihiTxt.Text + "Tarihleri Arasında Yapılan Arama/Görüşmeler";
            AramaGorusme arama = new AramaGorusme();

            DataTable dataTable = arama.SelectAllReturnDT(ArayanIdQS.ConvertToInt(), KatilimciTipiQS.ConvertToInt(), GorusmeSekliDDL.SelectedItem.Value,
                BaslangicTarihiTxt.Text.ConvertToDatetime(), bitis);

            List<AramaListItem> list = new List<AramaListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string aramaId = row["AramaId"].ToString();
                    string arayanId = row["ArayanId"].ToString();
                    int faaliyetId = row["FaaliyetId"].ReturnZeroIfNull().ConvertToInt();
                    string adi = row["Adi"].ToString();
                    string tarih = row["Tarih"].ConvertToDatetime().ToString("dd.MM.yyyy HH:mm");
                    string soyadi = row["Soyadi"].ToString();
                    string konu = row["Konu"].ToString();
                    string kurumu = row["Kurumu"].ToString();

                    bool gorusmeSaglandi = row["GorusmeSaglandi"].ConvertToBool();
                    bool randevuIstendi = row["RandevuIstendi"].ConvertToBool();
                    bool randevuKisiti = row["RandevuKisiti"].ReturnFalseIfNull().ConvertToBool();


                    AramaListItem aramaItem = new AramaListItem();
                    aramaItem.AramaId = aramaId;
                    aramaItem.ArayanId = arayanId;
                    aramaItem.AdiSoyadi = (adi + " " + soyadi).Trim();
                    aramaItem.Tarih = tarih;
                    aramaItem.Konu = konu;
                    aramaItem.Kurumu = kurumu;
                    aramaItem.GorusmeSaglandi = gorusmeSaglandi;
                    aramaItem.RandevuIstendi = randevuIstendi;
                    if (faaliyetId > 0)
                    {
                        aramaItem.FaaliyetId = faaliyetId.ToString();
                        Faaliyet faaliyet = new Faaliyet();
                        faaliyet = faaliyet.Select(faaliyetId);
                        if (faaliyet != null)
                        {
                            string acikTarihli = faaliyet.AcikTarih ? " (Açık)" : string.Empty;
                            aramaItem.Faaliyet = "<a target=_blank href=" + ProjeConstants.PAGE_FAALIYET_GIRIS + "?FaaliyetId=" + faaliyetId + " class='btn btn-outline-secondary'>Faaliyet"+acikTarihli+"</a>";
                        }
                        else
                            aramaItem.Faaliyet = string.Empty;
                    }
                    else if (randevuKisiti)
                    {
                        aramaItem.Faaliyet = "RK";
                    }
                    else if (randevuIstendi)
                    {
                        aramaItem.Faaliyet = "İstendi";
                    }

                    aramaItem.Duzenle = "<a href=" + ProjeConstants.PAGE_ARAMAGORUSME_GIRIS + "?AramaGorusmeId=" + aramaId + "&ArayanId=" + arayanId + " class='btn btn-outline-success'>Arama/Görüşme</a>";
                    aramaItem.Secildi = SecilenIdQS.Equals(aramaItem.AramaId);
                    list.Add(aramaItem);
                }
            }
            return list;
        }
        protected void YeniKisiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KISI_GIRIS);
        }
        private class AramaListItem
        {
            public string AramaId { get; set; }
            public string ArayanId { get; set; }
            public string FaaliyetId { get; set; }
            public string AdiSoyadi { get; set; }
            public string Tarih { get; set; }
            public string Konu { get; set; }
            public string Kurumu { get; set; }
            public bool GorusmeSaglandi { get; set; }
            public bool RandevuIstendi { get; set; }
            public string Faaliyet { get; set; }
            public string Duzenle { get; set; }
            public bool Secildi { get; set; }
        }

        protected void BaslangicTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }

        protected void BitisTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }

        protected void GorusmeSekliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }

        protected void HepsiBtn_Click(object sender, EventArgs e)
        {
            ArayanIdQS = string.Empty;
            AdiSoyadiLnk.Text = string.Empty;
            TabloOlustur();
        }
        protected void KisiListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KISI_LIST);
        }
        protected void FaaliyetListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_LIST);
        }
        protected void FaaliyetTakvimiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM);
        }

        protected void YeniAramaGirisiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_ARAMAGORUSME_GIRIS);
        }
        #region Katılımcı Seçimi
        protected void KatilimciSecBtn_Click(object sender, EventArgs e)
        {
            KatilimciModalAc(ProjeConstants.FAALIYET_KATILIMCI_IC_INT);
        }
        private void KatilimciModalAc(int katilimciTipi)
        {
            TabloModalOlustur();
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "KatilimciSecimiModal();", true);
        }
        private void TabloModalOlustur()
        {
            var jsonData = TabloModalJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        private string TabloModalJson()
        {
            string jSon = string.Empty;

            try
            {
                List<KatilimciListItem> list = GetModalDataList();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
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
        private string CreateModalDataTable(string jsonData)
        {
            string tableString = @"
            if ( $.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
              $('#CustomModalDataTable').DataTable().destroy();
            }
            $('#CustomModalDataTable tbody').empty();

            jQuery('#CustomModalDataTable').DataTable({
            data: " + jsonData + @",
            columns: [
                { data: 'AdiSoyadi' },
                { data: 'Kurumu' },
                { data: 'KatilimciSec' }
            ],
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
            },
            responsive: true,
            dom: 'fpirt',
            'createdRow': function (row, data, dataIndex) {
                if (data.KatilimciTipi == 1) {
                    $(row).addClass('icKatilimci');

                } else if (data.KatilimciTipi == 2) {
                    $(row).addClass('disKatilimci');

                } else if (data.KatilimciTipi == 3) {
                    $(row).addClass('nakitBagisci');

                } else if (data.KatilimciTipi == 4) {
                    $(row).addClass('tasinmazBagisci');
                }
            }
        });
            ";

            return tableString;
        }
        private List<KatilimciListItem> GetModalDataList()
        {
            Personel personel = new Personel();
            DataTable dataTableIc = personel.SelectSecilmemisIcKatilimcilarByFaaliyetIdReturnDT(ProjeConstants.HEPSI_INT);
            Kisi kisi = new Kisi();
            DataTable dataTableDis = kisi.SelectSecilmemisDisKatilimcilarByFaaliyetIdReturnDT(ProjeConstants.HEPSI_INT);
            NakitBagisci nakitBagisci = new NakitBagisci();
            DataTable dataTableNakit = nakitBagisci.SelectSecilmemisKatilimcilarByFaaliyetIdReturnDT(ProjeConstants.HEPSI_INT);
            TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
            DataTable dataTableTasinmaz = tasinmazBagisci.SelectSecilmemisKatilimcilarByFaaliyetIdReturnDT(ProjeConstants.HEPSI_INT);

            dataTableIc.Merge(dataTableDis);
            dataTableIc.Merge(dataTableNakit);
            dataTableIc.Merge(dataTableTasinmaz);
            int SiraNo = 1;
            List<KatilimciListItem> list = new List<KatilimciListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            if (dataTableIc != null)
            {
                foreach (DataRow row in dataTableIc.Rows)
                {
                    string katilimciId = row["KatilimciId"].ToString();
                    int katilimciTipi = row["KatilimciTipi"].ConvertToInt();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();

                    KatilimciListItem katilimciItem = new KatilimciListItem();
                    katilimciItem.Sirano = SiraNo++.ToString();
                    katilimciItem.KatilimciId = katilimciId;
                    katilimciItem.AdiSoyadi = (adi + " " + soyadi.Trim());
                    katilimciItem.Adi = adi;
                    katilimciItem.Soyadi = soyadi;
                    katilimciItem.KatilimciTipiStr = KatilimciTipiGetir(katilimciTipi);
                    katilimciItem.Kurumu = katilimciItem.KatilimciTipiStr;
                    katilimciItem.KatilimciTipi = katilimciTipi.ToString();

                    katilimciItem.KatilimciSec = "<a href='#' class='btn btn-outline-primary' onclick=KatilimciSecildiBtnClick(" + katilimciId + "," + katilimciTipi + ")>SEÇ</a>";

                    list.Add(katilimciItem);
                }
            }
            return list;
        }
        private string KatilimciTipiGetir(int katilimciTipi)
        {
            string katilimciTipStr = ProjeConstants.FAALIYET_KATILIMCI_DIS;
            switch (katilimciTipi)
            {
                case ProjeConstants.FAALIYET_KATILIMCI_IC_INT:
                    {
                        katilimciTipStr = ProjeConstants.FAALIYET_KATILIMCI_IC;
                        break;
                    }
                case ProjeConstants.FAALIYET_KATILIMCI_DIS_INT:
                    {
                        katilimciTipStr = ProjeConstants.FAALIYET_KATILIMCI_DIS;
                        break;
                    }
                case ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI_INT:
                    {
                        katilimciTipStr = ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI;
                        break;
                    }
                case ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI_INT:
                    {
                        katilimciTipStr = ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI;
                        break;
                    }
                default:
                    break;
            }
            return katilimciTipStr;
        }
        protected void SecilenKatilimciyiGetirBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_ARAMAGORUSME_LIST + "?ArayanId=" + paramFaaliyetKatilimciIdLbl.Value + "&KatilimciTipi=" + paramFaaliyetKatilimciTipiLbl.Value);
        }
        private class KatilimciListItem
        {
            public string Sirano { get; set; }
            public string KatilimciId { get; set; }
            public string KatilimciTipi { get; set; }
            public string KatilimciTipiStr { get; set; }
            public string AdiSoyadi { get; set; }
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public string Kurumu { get; set; }
            public string AniObjesi { get; set; }
            public string KisiKarti { get; set; }
            public string Cikar { get; set; }
            public string KatilimciSec { get; set; }
            public string IrtibatSec { get; set; }

        }
        #endregion

    }
}
