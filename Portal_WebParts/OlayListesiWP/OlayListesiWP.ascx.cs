using Model.IKYS;
using Model.MTS;
using Model.NBYS;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.OlayListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class OlayListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public OlayListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private DateTime SorguZamaniQS
        {
            get
            {
                if (ViewState["SorguZamani"] == null)
                {
                    ViewState["SorguZamani"] = false;
                }
                return ViewState["SorguZamani"].ConvertToDatetime();
            }

            set
            {
                ViewState["SorguZamani"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //RefreshTimer.Interval = 10000;
                //RefreshTimer.Enabled = true;
                TabloOlustur();
            }
            var jsonData = TabloJson();
            UtilityHelper.ScriptCalistir("ClearTableData();");
            UtilityHelper.ScriptCalistir("SetTableData(" + jsonData + ");");
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
                SorguZamaniQS = DateTime.Today;
                List<Olay> olayList = GetDataList(SorguZamaniQS);
                List<OlayListItem> oliList = OlayListItemDoldur(olayList);
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                jSon = serializer.Serialize(oliList);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        private List<Olay> GetDataList(DateTime tarih)
        {
            Olay olayDao = new Olay();
            List<Olay> list = olayDao.SelectByTarihReturnList(tarih, ProjeConstants.HEPSI);
            return list;
        }
        private List<OlayListItem> OlayListItemDoldur(List<Olay> list)
        {
            List<OlayListItem> olayList = new List<OlayListItem>();
            foreach (var item in list)
            {
                OlayListItem oli = new OlayListItem();
                string aciklama = ParseAciklama(item);
                oli.Aciklama = aciklama;
                oli.AciklamaHam = item.Aciklama;
                oli.Program= item.Program;
                oli.IslemKonusu = item.IslemKonusu;
                oli.IslemTarihi = item.IslemTarihi.ConvertToDDMMYYYHHmmFormat();
                oli.IslemTipi = item.IslemTipi;
                oli.IslemYapan = item.IslemYapan;
                oli.OlayId = item.Id.ToString();
                olayList.Add(oli);
            }
            return olayList;
        }

        public string ParseAciklama(Olay olay)
        {
            var words = olay.Aciklama.Split(ProjeConstants.DELIMITER).ToList();
            int randevuId = 0;
            int katilimciId = 0;
            int katilimciTipi = 0;
            Randevu randevu = null;
            string katilimci = string.Empty;
            string katilimciTipiStr = string.Empty;
            string aniObjesiStr = string.Empty;
            string getirilenAniObjesiStr = string.Empty;
            foreach (string item in words)
            {
                bool contains = item.Contains("RandevuId");
                if (contains)
                {
                    randevuId = item.Split('=')[1].ConvertToInt();
                    break;
                }
                else
                if (item.Contains("KatilimciId"))
                {
                    katilimciId = item.Split('=')[1].ConvertToInt();
                }
                else
                if (item.Contains("KatilimciTipi"))
                {
                    katilimciTipi = item.Split('=')[1].ConvertToInt();
                }
            }
            if (randevuId > 0)
            {
                randevu = new Randevu();
                randevu = randevu.Select(randevuId);
            }
            if (katilimciId > 0)
            {
                switch (katilimciTipi)
                {
                    case ProjeConstants.RANDEVU_KATILIMCI_TASINMAZBAGISCI_INT:
                        {
                            TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
                            tasinmazBagisci = tasinmazBagisci.Select<TasinmazBagisci>(katilimciId);
                            if (tasinmazBagisci != null)
                            {
                                katilimci = tasinmazBagisci.Adi + " " + tasinmazBagisci.Soyadi;
                                katilimciTipiStr = ProjeConstants.RANDEVU_KATILIMCI_TASINMAZBAGISCI;
                                //Burada verilen ani objeleri alınıyor
                                AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
                                DataTable dataTable = aniObjesiDagitim.SelectReturnDT(randevuId, tasinmazBagisci.Id, ProjeConstants.RANDEVU_KATILIMCI_TASINMAZBAGISCI_INT);
                                if (dataTable != null)
                                {
                                    string objeStr = string.Empty;
                                    foreach (DataRow row in dataTable.Rows)
                                    {
                                        int adet = row["Adet"].ReturnZeroIfNull().ConvertToInt();
                                        if (adet > 0)
                                        {
                                            string deger = row["Deger"].ToString();
                                            objeStr += " - " + deger + "(" + adet + ")";
                                        }
                                    }

                                    aniObjesiStr = objeStr;
                                }
                                //Getirilen Ani Objeleri ayrıca alınıyor
                                AniObjesiDagitim getirilenAniObjesi = new AniObjesiDagitim();
                                getirilenAniObjesi = getirilenAniObjesi.SelectGetirilenAniObjesi(randevuId, tasinmazBagisci.Id, ProjeConstants.RANDEVU_KATILIMCI_TASINMAZBAGISCI_INT);
                                if (getirilenAniObjesi != null)
                                {
                                    getirilenAniObjesiStr = getirilenAniObjesi.GetirilenAniObjesi;
                                }
                            }
                            else
                            {
                                MessageHelper.PublishMessage("Taşınmaz Bağışçı Bulunamadı", ProjeConstants.MESAJ_HATA);
                            }
                            break;
                        }

                    case ProjeConstants.RANDEVU_KATILIMCI_NAKITBAGISCI_INT:
                        {
                            NakitBagisci nakitBagisci = new NakitBagisci();
                            nakitBagisci = nakitBagisci.Select<NakitBagisci>(katilimciId);
                            if (nakitBagisci != null)
                            {
                                katilimci = nakitBagisci.Adi + " " + nakitBagisci.Soyadi;
                                katilimciTipiStr = ProjeConstants.RANDEVU_KATILIMCI_NAKITBAGISCI;

                                //Burada verilen ani objeleri alınıyor
                                AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
                                DataTable dataTable = aniObjesiDagitim.SelectReturnDT(randevuId, nakitBagisci.Id, ProjeConstants.RANDEVU_KATILIMCI_NAKITBAGISCI_INT);
                                if (dataTable != null)
                                {
                                    string objeStr = string.Empty;
                                    foreach (DataRow row in dataTable.Rows)
                                    {
                                        int adet = row["Adet"].ReturnZeroIfNull().ConvertToInt();
                                        if (adet > 0)
                                        {
                                            string deger = row["Deger"].ToString();
                                            objeStr += " - " + deger + "(" + adet + ")";
                                        }
                                    }

                                    aniObjesiStr = objeStr;
                                }

                                //Getirilen Ani Objeleri ayrıca alınıyor
                                AniObjesiDagitim getirilenAniObjesi = new AniObjesiDagitim();
                                getirilenAniObjesi = getirilenAniObjesi.SelectGetirilenAniObjesi(randevuId, nakitBagisci.Id, ProjeConstants.RANDEVU_KATILIMCI_NAKITBAGISCI_INT);
                                if (getirilenAniObjesi != null)
                                {
                                    getirilenAniObjesiStr = getirilenAniObjesi.GetirilenAniObjesi;
                                }
                            }
                            else
                            {
                                MessageHelper.PublishMessage("Nakit bağışçı Bulunamadı", ProjeConstants.MESAJ_HATA);
                            }
                            break;
                        }

                    case ProjeConstants.RANDEVU_KATILIMCI_DIS_INT:
                        {
                            Kisi kisi = new Kisi();
                            kisi = kisi.Select<Kisi>(katilimciId);
                            if (kisi != null)
                            {
                                katilimci = kisi.Adi + " " + kisi.Soyadi;
                                katilimciTipiStr = ProjeConstants.RANDEVU_KATILIMCI_DIS;

                                //Burada verilen ani objeleri alınıyor
                                AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
                                DataTable dataTable = aniObjesiDagitim.SelectReturnDT(randevuId, kisi.Id, ProjeConstants.RANDEVU_KATILIMCI_DIS_INT);
                                if (dataTable != null)
                                {
                                    string objeStr = string.Empty;
                                    foreach (DataRow row in dataTable.Rows)
                                    {
                                        int adet = row["Adet"].ReturnZeroIfNull().ConvertToInt();
                                        if (adet > 0)
                                        {
                                            string deger = row["Deger"].ToString();
                                            objeStr += " - " + deger + "(" + adet + ")";
                                        }

                                    }
                                    aniObjesiStr = objeStr;
                                }

                                //Getirilen Ani Objeleri ayrıca alınıyor
                                AniObjesiDagitim getirilenAniObjesi = new AniObjesiDagitim();
                                getirilenAniObjesi = getirilenAniObjesi.SelectGetirilenAniObjesi(randevuId, kisi.Id, ProjeConstants.RANDEVU_KATILIMCI_DIS_INT);
                                if (getirilenAniObjesi != null)
                                {
                                    getirilenAniObjesiStr = getirilenAniObjesi.GetirilenAniObjesi;
                                }
                            }
                            else
                            {
                                MessageHelper.PublishMessage("Kişi Bulunamadı", ProjeConstants.MESAJ_HATA);
                            }
                            break;
                        }

                    case ProjeConstants.RANDEVU_KATILIMCI_IC_INT:
                        {
                            Personel personel = new Personel();
                            personel = personel.Select<Personel>(katilimciId);
                            if (personel != null)
                            {
                                katilimci = personel.Adi + " " + personel.Soyadi;
                                katilimciTipiStr = ProjeConstants.RANDEVU_KATILIMCI_IC;
                                aniObjesiStr = string.Empty;
                                getirilenAniObjesiStr = string.Empty;
                            }
                            else
                            {
                                MessageHelper.PublishMessage("Personel Bulunamadı", ProjeConstants.MESAJ_HATA);
                            }
                            break;
                        }

                        ///////////////////
                }
            }
            string randevuStr = randevu == null ?
                            string.Empty :
                             "<strong>" + randevu.RandevuKonusu + "</strong> konulu ve  <strong>" + randevu.BaslangicTarihi + "</strong> tarihli faaliyetin ";
            string ayrintiStr = (string.IsNullOrEmpty(katilimci) ? string.Empty : "- Katılımcı : " + katilimci) +
                (string.IsNullOrEmpty(katilimciTipiStr) ? string.Empty : "-" + katilimciTipiStr) +
                (string.IsNullOrEmpty(aniObjesiStr) ? string.Empty : "- Verilen Anı Objesi : " + aniObjesiStr) +
                (string.IsNullOrEmpty(getirilenAniObjesiStr) ? string.Empty : "- Getirilen Anı Objesi : " + getirilenAniObjesiStr);
            string retval = "(" + olay.IslemTarihi.ConvertToDDMMYYYHHmmFormat() + ") -" +
               randevuStr + "  <strong>" +
               olay.IslemKonusu + "</strong> bölümünde " +
               olay.IslemYapan + " tarafından  <strong> " +
               olay.IslemTipi + "</strong> işlemi yapılmıştır." + ayrintiStr;
            return retval;
        }

        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                function SetTableData(myset) {
                     table.rows.add(myset);
                }
                function ClearTableData() {
                    table.clear();
                }
                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                var table=jQuery('#CustomDataTable').DataTable({
                   
                    data: " + jsonData + @",
                    columns: [
                        { data: 'IslemTarihi' },
                        { data: 'Program'},
                        { data: 'IslemKonusu'},
                        { data: 'IslemTipi' },
                        { data: 'IslemYapan'},
                        { data: 'Aciklama' , 'width': '30%'},
                        { data: 'AciklamaHam' },
                    ],
                    'columnDefs': [
                        { type: 'turkish', targets: [1,2,3,4,5] },
                    ],
                    'order': [[0, 'desc']],//sort date desc
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
       
        private class OlayListItem
        {
            public string OlayId { get; set; }
            public string IslemTarihi { get; set; }
            public string Program { get; set; }
            public string IslemKonusu { get; set; }
            public string IslemTipi { get; set; }
            public string IslemYapan { get; set; }
            public string Aciklama { get; set; }
            public string AciklamaHam { get; set; }
        }
    }
}
