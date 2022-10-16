using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.UcretliMahsupWP
{
    [ToolboxItemAttribute(false)]
    public partial class UcretliMahsupWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public UcretliMahsupWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        private string CurrentUserName
        {
            get
            {

                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUser();
                }
                return ViewState["CurrentUserName"].ToString();
            }

            set
            {
                ViewState["CurrentUserName"] = value;
            }
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
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty(MesajQS))
                    {
                        MessageHelper.PublishMessage("Mahsup Tamamlandı", ProjeConstants.MESAJ_BASARILI, 2000);
                        MesajQS = string.Empty;
                    }
                    fillPersonelDDL();
                    fillDonemDDL();
                    TabloOlustur();
                }
                
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private string GetDataJson()
        {
            IzinHareket izinHareket = new IzinHareket();
            int personelId = PersonelDDL.SelectedItem == null ? 0 : PersonelDDL.SelectedItem.Value.ConvertToInt();
            int donemId = DonemDDL.SelectedItem == null ? 0 : DonemDDL.SelectedItem.Value.ConvertToInt();
            var json = izinHareket.SelectByPersonelIdDonemIdReturnJson(personelId, donemId, ProjeConstants.IZINTIPI_UCRETLI_INT);

            return json;
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl ;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void fillPersonelDDL()
        {
            PersonelDDL.Items.Clear();
            Personel personel = new Personel();
            List<Personel> list = personel.SelectCalisanPersonel();
            ListItem li0 = new ListItem(ProjeConstants.HEPSI, ProjeConstants.IZINTIPI_HEPSI_INT.ToString());
            PersonelDDL.Items.Add(li0);

            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                PersonelDDL.Items.Add(li);
            }


        }
        private void fillDonemDDL()
        {
            DonemDDL.Items.Clear();
            int personelId = PersonelDDL.SelectedItem.Value.ConvertToInt();
            ListItem li0 = new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI.ToString());
            DonemDDL.Items.Add(li0);
            if (personelId > 0)
            {
                IzinDonem izinDonemi = new IzinDonem();
                List<IzinDonem> list = izinDonemi.SelectByPersonelId(personelId, ProjeConstants.IZINTIPI_UCRETLI_INT);
                foreach (IzinDonem item in list)
                {
                    ListItem li = new ListItem(item.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + item.BitisTarihi.ConvertToDatetimeEmptyIfNull(), item.Id.ReturnZeroIfNull().ToString());
                    DonemDDL.Items.Add(li);
                }
            }
        }
        private void fillModalDonemDDL(Personel personel, int izinHareketId)
        {
            ModalDonemDDL.Items.Clear();
            if (personel.Id > 0)
            {
                IzinDonem izinDonemi = new IzinDonem();
                List<IzinDonem> list = izinDonemi.SelectByPersonelId(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT);
                int donemSayisi = 0;
                IzinHareket izinHareket = new IzinHareket();
                izinHareket = izinHareket.Select<IzinHareket>(izinHareketId);
                int sure = izinHareket.Sure.ConvertToInt();
                foreach (IzinDonem item in list)
                {
                    int kalanIzin = item.KalanIzin.ConvertToInt();
                    if (kalanIzin >= sure)
                    {
                        ListItem li = new ListItem(item.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + item.BitisTarihi.ConvertToDatetimeEmptyIfNull()
                            + " ( Kalan izin = " + kalanIzin + " " + item.Birim + " )", item.Id.ReturnZeroIfNull().ToString());
                        ModalDonemDDL.Items.Add(li);

                        donemSayisi++;
                    }

                }

                fillOnayLbl(personel, donemSayisi > 0);
            }
        }
        private void fillOnayLbl(Personel personel, bool isUygunDonemVar)
        {
            IzinHareket izinHareket = new IzinHareket();
            izinHareket = izinHareket.Select<IzinHareket>(paramIzinHareketIdLbl.Value.ConvertToInt());
            if (isUygunDonemVar)
            {
                OnayLbl.Text = "Onayladığınız takdirde " + personel.Adi + " " + personel.Soyadi + " Tarafından " + izinHareket.BaslangicTarihi.ToString("dd.MM.yyyy") + " - "
                + izinHareket.BitisTarihi.ToString("dd.MM.yyyy") + " Tarihleri Arasında kullanılan " + izinHareket.Sure + " " + izinHareket.Birim
                + " süreli izin için MAHSUP İŞLEMİ uygulanacaktır.";
                MahsupEtModalBtn.Visible = true;

            }
            else
            {
                OnayLbl.Text = "Mahsup işlemi için uygun dönem bulunmamaktadır.";
                MahsupEtModalBtn.Visible = false;
            }

        }
        private void fillModalIzinBilgileriTable(Personel personel)
        {
            DateTime today = DateTime.Today;
            IsBilgileri ib = new IsBilgileri();
            ib = ib.SelectByPersonelId(personel.Id);
            if (ib != null)
            {

                DateTime izinDonemiBasTar = ib.IzinDonemiBasTar;
                //int izinDonemiSayisi = (today.Year - izinDonemiBasTar.Year);
                //izinDonemiSayisi = izinDonemiSayisi > 5 ? 5 : izinDonemiSayisi;
                IzinDonem izinDonem = new IzinDonem();
                List<IzinDonem> izinDonemiList = izinDonem.SelectByPersonelId(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT);

                IzinHareket izinHareket = new IzinHareket();

                IzinBilgileriTableHeaders();
                foreach (IzinDonem izinDonemi in izinDonemiList)
                {
                    TableRow row = new TableRow();
                    TableCell IzinDonemiCell = new TableCell();

                    Mahsup mahsup = new Mahsup();
                    List<Mahsup> mahsupList = mahsup.SelectByDonemId(izinDonemi.Id);
                    IzinDonemiCell.Text = izinDonemi.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + izinDonemi.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                    if (mahsupList.Count > 0)
                        IzinDonemiCell.Text += "(M)";
                    row.Controls.Add(IzinDonemiCell);


                    string izinHakki = string.Empty;
                    TableCell IzinHakkiCell = new TableCell();
                    izinHakki = izinDonemi.IzinHakki;
                    IzinHakkiCell.Text = izinHakki.ReturnZeroIfNull().ToString();
                    row.Controls.Add(IzinHakkiCell);

                    TableCell KullanilanIzinCell = new TableCell();
                    KullanilanIzinCell.Text = izinDonemi.KullanilanIzin.ReturnZeroIfNull().ToString();
                    row.Controls.Add(KullanilanIzinCell);

                    TableCell KalanIzinCell = new TableCell();
                    KalanIzinCell.Text = izinDonemi.KalanIzin.ReturnZeroIfNull().ToString();
                    row.Controls.Add(KalanIzinCell);
                    IzinBilgileriTable.Controls.Add(row);

                }
            }
            else
            {
                MessageHelper.PublishMessage("İşe başlama tarihi belirlenemedi", ProjeConstants.MESAJ_HATA);
            }
        }
        private void IzinBilgileriTableHeaders()
        {
            IzinBilgileriTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell donemCell = new TableHeaderCell();
            donemCell.Text = "İzin Dönemi";
            TableHeaderCell hakCell = new TableHeaderCell();
            hakCell.Text = "İzin Hakkı";
            TableHeaderCell kullanilanCell = new TableHeaderCell();
            kullanilanCell.Text = "Kullanılan İzin";
            TableHeaderCell kalanCell = new TableHeaderCell();
            kalanCell.Text = "Kalan İzin";
            th.Controls.Add(donemCell);
            th.Controls.Add(hakCell);
            th.Controls.Add(kullanilanCell);
            th.Controls.Add(kalanCell);
            IzinBilgileriTable.Controls.Add(th);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void OnaylaBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool isSaved = MahsupIsleminiyap();
                if (isSaved)
                {
                    RedirectToPage(ProjeConstants.PAGE_UCRETLIMAHSUP+"?SecilenId="+SecilenIdQS+"&Mesaj=true");
                }
                TabloOlustur();
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }

        }
        protected void ModalInfoBtn_Click(object sender, EventArgs e)
        {
            //int personelId = paramPersonelIdLbl.Value.ConvertToInt();
            int izinHareketId = paramIzinHareketIdLbl.Value.ConvertToInt();
            SecilenIdQS = izinHareketId.ToString();
            IzinHareket izinHareket = new IzinHareket();
            izinHareket = izinHareket.Select<IzinHareket>(izinHareketId);
            if (izinHareket!=null)
            {
                int personelId = izinHareket.PersonelId;
                Personel personel = new Personel();
                personel = personel.Select<Personel>(personelId);
                if (personel != null)
                {
                    PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                    fillModalDonemDDL(personel, izinHareketId);
                    fillModalIzinBilgileriTable(personel);

                }
            } 
        }
        protected void PersonelDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillDonemDDL();
            TabloOlustur();
        }
        protected void DonemDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        private Mahsup MahsupTablosunaEkle(IzinHareket izinHareket, int asilIzinTipi, int kullanildigiDonem, int mahsupDonemi, string aciklama)
        {
            Mahsup mahsup = new Mahsup();
            mahsup.PersonelId = izinHareket.PersonelId;
            mahsup.IzinHareketId = izinHareket.Id;
            mahsup.IzinTipi = asilIzinTipi;
            mahsup.KullanildigiDonemId = kullanildigiDonem;
            mahsup.MahsupDonemId = mahsupDonemi;
            mahsup.Aciklama = aciklama;
            mahsup.Olusturan = CurrentUserName;
            int mahsupid = mahsup.Save();
            return mahsup;
        }
        private bool MahsupIsleminiyap()
        {
            bool isAllSaved = false;
            bool isIzinHareketSaved = false;
            bool isOncekiDonemGuncellendi = false;
            bool isYeniIzinDonemiGuncellendi = false;
            Mahsup mahsupRB = null;
            
            int izinHareketId = paramIzinHareketIdLbl.Value.ConvertToInt();
            IzinHareket izinHareket = new IzinHareket();
            izinHareket = izinHareket.Select<IzinHareket>(izinHareketId);

            if (izinHareket != null)
            {
                int personelId = izinHareket.PersonelId;
                Personel personel = new Personel();
                personel = personel.Select<Personel>(personelId);
                if (personel != null)
                {
                    DateTime izinBasTar = izinHareket.BaslangicTarihi;
                    IzinDonem oncekiIzinDonemi = new IzinDonem();
                    IzinDonem yeniIzinDonemi = new IzinDonem();

                    IzinDonem oncekiIzinDonemiRB = new IzinDonem();
                    IzinDonem yeniIzinDonemiRB = new IzinDonem();
                    IzinHareket izinHareketRB = izinHareket;

                    oncekiIzinDonemi = oncekiIzinDonemi.Select<IzinDonem>(izinHareket.IzinDonemId);//oncekiIzinDonemi.SelectByIzinTarihi(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, izinBasTar);
                    int yeniIzinDonemiId = ModalDonemDDL.SelectedItem == null ? 0 : ModalDonemDDL.SelectedItem.Value.ConvertToInt();
                    yeniIzinDonemi = yeniIzinDonemi.Select<IzinDonem>(yeniIzinDonemiId);

                    if ((oncekiIzinDonemi == null) || (yeniIzinDonemi == null))
                    {
                        MessageHelper.PublishMessage("Mahsup işlemi yapılamaz, İzin dönemi bulunamadı!", ProjeConstants.MESAJ_HATA);
                    }
                    else
                    {

                        //önceki izin dönemini güncelle,  kullanılan izni süre kadar eksilt, kalan izni süre kadar artır
                        //yeni izin dönemini güncelle kullanılan izni süre kadar artır, kalan izni süre kadar eksilt
                        // izinhareketi mahsup=true yap, açıklama yaz
                        //mahsup tablosune ekle
                        //her üçü de ok ise islem tamamlandı
                        //eger herhangi biri tamamlanmadı ize rollback yap

                        if (oncekiIzinDonemi != null)
                        {
                            int sure = izinHareket.Sure.ConvertToInt();
                            oncekiIzinDonemiRB = oncekiIzinDonemi;
                            DateTime izinBastar = oncekiIzinDonemi.BaslangicTarihi;
                            string mahsupAciklama = "# Önceki izin dönemi= " + oncekiIzinDonemi.BaslangicTarihi + "-" + oncekiIzinDonemi.BitisTarihi
                                + "; Yeni izin dönemi= " + yeniIzinDonemi.BaslangicTarihi + "-" + yeniIzinDonemi.BitisTarihi + "; "
                                + "; Mahsup Edilen süre = " + izinHareket.Sure;
                            Mahsup mahsup = MahsupTablosunaEkle(izinHareket, ProjeConstants.IZINTIPI_UCRETLI_INT, oncekiIzinDonemi.Id, yeniIzinDonemi.Id, mahsupAciklama);
                            mahsupRB = mahsup;
                            //önceki izin dönemi mahsupişlemleri
                            int oncekiKullanilanIzinInt = oncekiIzinDonemi.KullanilanIzin.ConvertToInt() - sure;
                            oncekiIzinDonemi.KullanilanIzin = oncekiKullanilanIzinInt.ToString();
                            int oncekiKalanIzinInt = oncekiIzinDonemi.KalanIzin.ConvertToInt() + sure;
                            oncekiIzinDonemi.KalanIzin = oncekiKalanIzinInt.ToString();
                            oncekiIzinDonemi.Aciklama += System.Environment.NewLine + "# Tarih=(" + DateTime.Now + ") Mahsup, IzinHareketId=" + izinHareket.Id + " YeniIzinDonemiId=" + yeniIzinDonemi.Id;
                            oncekiIzinDonemi.Degistiren = CurrentUserName;
                            isOncekiDonemGuncellendi = oncekiIzinDonemi.Update();

                            //yeni izin dönemi mahsupişlemleri
                            int yeniKullanilanIzinInt = yeniIzinDonemi.KullanilanIzin.ConvertToInt() + sure;
                            yeniIzinDonemi.KullanilanIzin = yeniKullanilanIzinInt.ToString();
                            int yeniKalanIzinInt = yeniIzinDonemi.KalanIzin.ConvertToInt() - sure;
                            yeniIzinDonemi.KalanIzin = yeniKalanIzinInt.ToString();
                            yeniIzinDonemi.Aciklama += System.Environment.NewLine + "# Tarih=(" + DateTime.Now + ") Mahsup, IzinHareketId=" + izinHareket.Id + " OncekiIzinDonemiId=" + oncekiIzinDonemi.Id;
                            yeniIzinDonemi.Degistiren = CurrentUserName;
                            isYeniIzinDonemiGuncellendi = yeniIzinDonemi.Update();

                            //izin hareket islemleri
                            izinHareket.Aciklama += "# Tarih=(" + DateTime.Now + ") MahsupId=" + mahsup.Id + "; " + AciklamaTxt.Text;
                            izinHareket.Mahsup = true;
                            izinHareket.IzinDonemId = yeniIzinDonemi.Id;
                            izinHareket.Degistiren = CurrentUserName;
                            isIzinHareketSaved = izinHareket.Update();

                        }
                    }
                    isAllSaved = isIzinHareketSaved && isOncekiDonemGuncellendi && isYeniIzinDonemiGuncellendi;
                    if (!isAllSaved)
                    {
                        bool isMahsupRB = (mahsupRB == null) ? true : mahsupRB.Delete();
                        bool isIzinHareketRB = izinHareketRB.Update();
                        bool isOncekiIzinRB = oncekiIzinDonemiRB.Update();
                        bool isYeniIzinRB = yeniIzinDonemiRB.Update();
                        string message = " Mahsup sırasında sorunlarla karşılaşıldı. Geri alma işleminde: "
                            + " İzin Hareketi geri alma : " + (isIzinHareketRB ? "Başarılı. " : "Başarısız. ")
                            + " Önceki İzin Dönemi geri alma : " + (isOncekiIzinRB ? "Başarılı. " : " Başarısız")
                            + (isYeniIzinRB ? "Başarılı. " : " Başarısız.");
                        MessageHelper.PublishMessage(message, ProjeConstants.MESAJ_HATA);
                    }

                }


            }
            return isAllSaved;
        }
        private void TabloOlustur()
        {
            var jsonData = GetDataJson();//TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date

                jQuery('#CustomDataTable').DataTable({
                    'initComplete': function (settings, json) {//tablo yüklendiğinde
                        var api = this.api();
                        var row = api.row(function(idx, data, node) { //secilen toplantıya gider
                            return data['Secildi'] == true;
                        });
                        if (row.length > 0)
                        {
                            row.select()
                                .show()
                                .draw(false);
                        }
                    },
                    columnDefs:[
                            {targets:2, render:function(data){
                              return moment(data).format('DD.MM.YYYY');
                            }},
                            {targets:3, render:function(data){
                              return moment(data).format('DD.MM.YYYY');
                            }},
                            {targets:5, render:function(data){
                                var link= '<a href=# onclick=OpenModal('+data+'); class=\'btn btn-outline-danger \'>Mahsup İşlemi</a>';
                              return link;
                            }}],                    
                    data: " + jsonData + @",
                    columns: [
                        { data: 'AdiSoyadi' },
                        { data: 'IzinDonemi' },
                        { data: 'BaslangicTarihi'},
                        { data: 'BitisTarihi' },
                        { data: 'SureBirim' },
                        { data: 'IzinHareketId' },
                    ],
                    'order': [[2, 'desc']],//sort date desc
                    'language': {
                    'url': 'http://tskgv-portal/OrtakBelgeler/Turkish.txt',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    dom: 'frtip',                    
                });

            ";

            return tableString;
        }
    }
}
