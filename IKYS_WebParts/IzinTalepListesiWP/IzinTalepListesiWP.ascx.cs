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

namespace IKYS_WebParts.IzinTalepListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class IzinTalepListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public IzinTalepListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        #region Liste Olusturma
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
                List<IzinTalepListItem> list = GetDataList();
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
        private List<IzinTalepListItem> GetDataList()
        {

            DataTable dataTable = GetDataTable();

            List<IzinTalepListItem> list = new List<IzinTalepListItem>();

            foreach (DataRow row in dataTable.Rows)
            {
                int personelId = row["PersonelId"].ConvertToInt();
                int izinTalepId = row["IzinTalepId"].ConvertToInt();
                int izinTipiId = row["IzinTipiId"].ConvertToInt();
                int onayDurumuId = row["OnayDurumuId"].ConvertToInt();
                string adiSoyadi = row["AdiSoyadi"].ToString();
                string izinTipi = row["IzinTipi"].ToString();
                DateTime baslangicTarihi = row["BaslangicTarihi"].ConvertToDatetime();
                DateTime bitisTarihi = row["BitisTarihi"].ConvertToDatetime();
                string sure = row["Sure"].ToString();
                string birim = row["Birim"].ToString();
                string onayDurumu = row["OnayDurumu"].ToString();


                IzinTalepListItem izinTalepListItem = new IzinTalepListItem();
                izinTalepListItem.PersonelId = personelId.ToString();
                izinTalepListItem.IzinTalepId = izinTalepId.ToString();
                izinTalepListItem.IzinTipiId = izinTipiId.ToString();
                izinTalepListItem.OnayDurumuId = onayDurumuId.ToString();
                izinTalepListItem.AdiSoyadi = adiSoyadi;
                izinTalepListItem.IzinTipi = izinTipi;
                izinTalepListItem.BaslangicTarihiHidden = baslangicTarihi;
                izinTalepListItem.BaslangicTarihi = baslangicTarihi.ConvertToDatetimeEmptyIfNull();
                izinTalepListItem.BitisTarihi = bitisTarihi.ConvertToDatetimeEmptyIfNull();
                //if (izinTipiId == ProjeConstants.IZINTIPI_SUTIZNI_INT)
                //{
                //    izinTalepListItem.BaslangicTarihi = baslangicTarihi.ConvertToDatetimeEmptyIfNull() + "(" + baslangicTarihi.ToString("HH:mm") + " - " + bitisTarihi.ToString("HH:mm") + ")";
                //    izinTalepListItem.BitisTarihi = bitisTarihi.ConvertToDatetimeEmptyIfNull() + "(" + baslangicTarihi.ToString("HH:mm") + " - " + bitisTarihi.ToString("HH:mm") + ")";
                //}
                //else
                //{
                //    izinTalepListItem.BaslangicTarihi = baslangicTarihi.ConvertToDatetimeEmptyIfNull();
                //    izinTalepListItem.BitisTarihi = bitisTarihi.ConvertToDatetimeEmptyIfNull();
                //}

                izinTalepListItem.Sure = sure;
                izinTalepListItem.Birim = birim;
                izinTalepListItem.OnayDurumu = onayDurumu;
                izinTalepListItem.Secildi = SecilenIdQS.Equals(izinTalepListItem.IzinTalepId);

                if (onayDurumuId == ProjeConstants.ONAYDURUMU_ISLEM_INT || onayDurumuId == ProjeConstants.ONAYDURUMU_DILEKCE_INT)
                {
                    izinTalepListItem.Duzenle = "<a href=" + ProjeConstants.PAGE_IZINTALEP_GIRIS + "?DestinationApp=TD&IzinTalepId=" + izinTalepId + "&PersonelId=" + personelId + "&IzinTanimId=" + izinTipiId + " class='btn btn-outline-primary'>Düzenle</a>";
                }
                else
                {
                    izinTalepListItem.Duzenle = string.Empty;
                }

                if (onayDurumuId == ProjeConstants.ONAYDURUMU_ISLEM_INT || onayDurumuId == ProjeConstants.ONAYDURUMU_DILEKCE_INT)
                {
                    izinTalepListItem.KayitKontrolRed = "<a href=# onclick=OpenModal('kabul'," + izinTalepId + "," + personelId + "); class=\'btn btn-outline-info \'>Kontrol/Red</a>";
                }
                else if (onayDurumuId == ProjeConstants.ONAYDURUMU_KONTROL_INT)
                {
                    izinTalepListItem.KayitKontrolRed = "<a href=# onclick=OpenModal('onay'," + izinTalepId + "," + personelId + "); class=\'btn btn-outline-success \'>Kayıt/Red</a>";
                }
                else
                {
                    izinTalepListItem.KayitKontrolRed = string.Empty;
                }

                list.Add(izinTalepListItem);
            }
            return list;
        }
        private DataTable GetDataTable()
        {
            IzinTalep izinTalep = new IzinTalep();
            DataTable dataTable = izinTalep.SelectIzinTalepleriReturnDT(0, ProjeConstants.IZINTIPI_UCRETLI_INT, true, false);
            return dataTable;
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
                    data: " + jsonData + @",
                    columns: [
                        { data: 'BaslangicTarihiHidden'},
                        { data: 'AdiSoyadi' },
                        { data: 'IzinTipi' },
                        { data: 'BaslangicTarihi'},
                        { data: 'BitisTarihi' },
                        { data: 'Sure' },
                        { data: 'Birim' },
                        { data: 'OnayDurumu' },
                        { data: 'Duzenle' },
                        { data: 'KayitKontrolRed' },
                    ],
                    'columnDefs': [
                        {
                            'targets': [0],
                            'visible': false,
                            'searchable': false
                        },
                    ],
                    'order': [[8, 'desc'],[0, 'desc']],//sort date desc
                    'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    dom: 'frtip',                    
                    'createdRow': function(row, data, dataIndex) {
                        if (data.OnayDurumu=='" + ProjeConstants.ONAYDURUMU_KAYIT+ @"')
                        {
                            $(row).addClass('kayitlara-islendi');

                        }else if (data.OnayDurumu=='" + ProjeConstants.ONAYDURUMU_RED + @"')
                        {
                            $(row).addClass('reddedildi');

                        }
                    },//set row color
                });

            ";

            return tableString;
        }
        #endregion

        private void FillIzinHareketleriTable(Personel personel)
        {
            DateTime today = DateTime.Today;
            DateTime threeMonthsLater = DateTime.Today.AddMonths(3);
            IsBilgileri ib = new IsBilgileri();
            ib = ib.SelectByPersonelId(personel.Id);
            if (ib != null)
            {


                IzinHareket izinHareket = new IzinHareket();
                IzinDonem izinDonem = new IzinDonem();
                izinDonem = izinDonem.SelectByIzinTarihi(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, today);
                if (izinDonem != null)
                {
                    DateTime izinDonemiSonu = izinDonem != null ? izinDonem.BitisTarihi : today.AddMonths(1);
                    //DataTable dataTable = izinHareket.SelectByIzinDonemiReturnDataTable(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, izinDonemiBasi, threeMonthsLater);//3 ay içinde yeni izin dönemi başlıyor olabilir
                    DataTable dataTable = izinHareket.SelectByIzinDonemiReturnDataTable(izinDonem.Id, personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT);
                    int SiraNo = 1;
                    if (dataTable == null)
                    {
                        IzinHareketTable.Rows.Clear();
                        TableRow tr = new TableRow();
                        TableCell tc = new TableCell();
                        tc.Text = "Henüz izin kullanılmamış.";
                        tr.Controls.Add(tc);
                        IzinHareketTable.Controls.Add(tr);
                    }
                    else
                    {
                        IzinHareketTableHeaders();
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            string izinTipi = dataRow["IzinTipi"].ReturnEmptyIfNull().ToString();
                            DateTime bastar = dataRow["BaslangicTarihi"].ConvertToDatetime();
                            DateTime bittar = dataRow["BitisTarihi"].ConvertToDatetime();
                            string sure = dataRow["Sure"].ToString();
                            string birim = dataRow["Birim"].ToString();

                            TableRow row = new TableRow();

                            TableCell SiraNoCell = new TableCell();

                            SiraNoCell.Text = SiraNo++ + "";
                            row.Controls.Add(SiraNoCell);

                            TableCell IzinTipiCell = new TableCell();
                            IzinTipiCell.Text = izinTipi;
                            row.Controls.Add(IzinTipiCell);

                            TableCell BasTarCell = new TableCell();

                            BasTarCell.Text = bastar.ConvertToDatetimeEmptyIfNull();
                            row.Controls.Add(BasTarCell);

                            TableCell BitTarCell = new TableCell();

                            BitTarCell.Text = bittar.ConvertToDatetimeEmptyIfNull();
                            row.Controls.Add(BitTarCell);
                            TableCell SureCell = new TableCell();
                            SureCell.Text = sure + " " + birim;

                            row.Controls.Add(SureCell);

                            if (bastar > izinDonemiSonu) //gelecek izin dönemine aitse farklı renk yazdır
                            {
                                SiraNoCell.ForeColor = System.Drawing.Color.Red;
                                IzinTipiCell.ForeColor = System.Drawing.Color.Red;
                                BasTarCell.ForeColor = System.Drawing.Color.Red;
                                BitTarCell.ForeColor = System.Drawing.Color.Red;
                                BitTarCell.ForeColor = System.Drawing.Color.Red;
                                SureCell.ForeColor = System.Drawing.Color.Red;

                                row.ToolTip = "Yeni İzin Dönemi";
                            }

                            IzinHareketTable.Controls.Add(row);
                        }
                    }
                }

            }
            else
            {
                // MessageHelper.PublishMessage("İşe başlama tarihi belirlenemedi", ProjeConstants.MESAJ_HATA);
            }
        }
        private void FillIzinBilgileriTable(Personel personel)
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
                    IzinDonemiCell.Text = izinDonemi.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + izinDonemi.BitisTarihi.ConvertToDatetimeEmptyIfNull();
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
        private void IzinHareketTableHeaders()
        {
            IzinHareketTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell izintipiCell = new TableHeaderCell();
            izintipiCell.Text = "İzin Tipi";
            TableHeaderCell bastarCell = new TableHeaderCell();
            bastarCell.Text = "Başlangıç tarihi";
            TableHeaderCell bittarCell = new TableHeaderCell();
            bittarCell.Text = "Bitiş tarihi";
            TableHeaderCell sureCell = new TableHeaderCell();
            sureCell.Text = "İzinli Süre";
            TableHeaderCell yazdirCell = new TableHeaderCell();
            yazdirCell.Text = "Yazdır";
            th.Controls.Add(siraCell);
            th.Controls.Add(izintipiCell);
            th.Controls.Add(bastarCell);
            th.Controls.Add(bittarCell);
            th.Controls.Add(sureCell);
            IzinHareketTable.Controls.Add(th);
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
                //IzinTalep_Table da onayDurumu hanesini kayıtlara işlendi yap
                IzinTalep izinTalep = new IzinTalep();
                izinTalep.Aciklama = string.IsNullOrEmpty(AciklamaTxt.Text) ? izinTalep.Aciklama : AciklamaTxt.Text;
                izinTalep = izinTalep.Select<IzinTalep>(paramIzinTalepIdLbl.Value.ConvertToInt());
                if (izinTalep != null)
                {
                    izinTalep.OnayDurumu = ProjeConstants.PER_IZINTALEBI_KAYITLARAISLENDI_INT;
                    izinTalep.Degistiren = CurrentUserName;
                    bool kayitlaraİslendi = izinTalep.Update();
                    if (kayitlaraİslendi)
                    {
                        bool izinHareketTablosundaVarMi = IzinHareketTablosundaVarMI(izinTalep.Id);
                        if (izinHareketTablosundaVarMi)
                        {
                            MessageHelper.PublishMessage("Bu Talep daha önce zaten onaylanmıştır.", ProjeConstants.MESAJ_HATA);
                        }
                        else
                        {
                            IzinHareket ih = new IzinHareket();
                            ih.PersonelId = izinTalep.PersonelId;
                            ih.IzinTipi = izinTalep.IzinTipi;
                            ih.IzinTalepId = izinTalep.Id;
                            ih.BaslangicTarihi = izinTalep.BaslangicTarihi;
                            ih.BitisTarihi = izinTalep.BitisTarihi;
                            ih.Adres = izinTalep.Adres;
                            ih.VekilImza = izinTalep.VekilImza;
                            ih.AmirImza = izinTalep.AmirImza;
                            ih.OnayImza = izinTalep.OnayImza;
                            ih.Olusturan = CurrentUserName;
                            ih.IzinDonemId = izinTalep.IzinDonemId;
                            ih.Sure = izinTalep.Sure;
                            ih.Birim = izinTalep.Birim;
                            
                            int izinHareketId = ih.Save();
                            if (izinHareketId > 0)
                            {
                                if (ih.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT) 
                                {
                                    IzinDonem izinDonemi = new IzinDonem();
                                    izinDonemi = izinDonemi.Select<IzinDonem>(ih.IzinDonemId);
                                    if (izinDonemi != null)
                                    {
                                        ih.OncekiIzinStr = izinDonemi.KalanIzin.ToString();
                                        ih.OncekiIzinStr = izinDonemi.KalanIzin.ToString();
                                        ih.KullanilanIzinStr = izinTalep.Sure;
                                        izinDonemi.KullanilanIzinGuncelle(izinDonemi, ih.Sure, true, CurrentUserName);
                                        ih.KalanIzinStr = izinDonemi.KalanIzin.ToString();
                                        ih.Update();//kalan izin hesaplandıktan sonra izinHareket tablosuna yazsın
                                    }
                                }
                                if (ih.IzinTipi == ProjeConstants.IZINTIPI_UCRETSIZ_INT)
                                {

                                    IsBilgileri ib = new IsBilgileri();
                                    ib = ib.Select<IsBilgileri>(ih.PersonelId);
                                    string baslamaTarStr = ib == null ? "" : ib.BaslamaTar.ConvertToDatetimeEmptyIfNull();
                                    string izinDonemiBasTarStr = ib == null ? "" : ib.IzinDonemiBasTar.ConvertToDatetimeEmptyIfNull();

                                    izinDonemiBasTarStr = string.IsNullOrEmpty(izinDonemiBasTarStr) ? baslamaTarStr : izinDonemiBasTarStr;
                                    if (!string.IsNullOrEmpty(izinDonemiBasTarStr))
                                    {
                                        Personel personel = new Personel();
                                        personel = personel.Select<Personel>(ih.PersonelId);
                                        if ((personel != null) && (!personel.Asker_sivil.Equals(ProjeConstants.PER_ASKER_INT)))//asker değilse işlem yap
                                        {

                                            int sureDays = ih.BitisTarihi.Subtract(ih.BaslangicTarihi).Days + 1; // +1 eklendi aksi halde 1 gün eksik yapıyor SB 23.03.2020
                                            DateTime izinDonemiBasTar = izinDonemiBasTarStr.ConvertToDatetime();//ib.IzinDonemiBasTar;
                                            DateTime yeniIzinDonemiBastar = izinDonemiBasTar.AddDays(sureDays);
                                            ib.IzinDonemiBasTar = yeniIzinDonemiBastar;
                                            try
                                            {
                                                bool issaved = ib.Update();
                                                if (issaved)
                                                {

                                                    MessageHelper.PublishMessage(izinDonemiBasTar + " olan İzin Dönemi Başlangıç Tarihi " +
                                                    yeniIzinDonemiBastar + " olarak değiştirilmiştir.", ProjeConstants.MESAJ_BASARILI);
                                                }

                                            }
                                            catch (Exception)
                                            {
                                                MessageHelper.PublishMessage("İzin Dönemi Başlangıç Tarihi Ücretsiz izin Süresi Kadar Ertelenemedi", ProjeConstants.MESAJ_HATA);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageHelper.PublishMessage("İşe Başlama Tarihi (İzin Dönemi Başlangıç Tarihi) boş olduğundan, İzin Dönemi Başlangıç Tarihi Ücretsiz izin Süresi Kadar Ertelenemedi", ProjeConstants.MESAJ_HATA);
                                    }
                                }
                            }
                        }

                    }
                }
                if (izinTalep.EPostaGonder)
                {
                    IKYSOrtak.IzinKabulRedOnayEPostasiGonder(izinTalep.PersonelId, izinTalep.Id, "Onay");
                }
                MessageHelper.PublishMessage("İzin Talebi " + ProjeConstants.PER_IZINTALEBI_KAYITLARAISLENDI, ProjeConstants.MESAJ_BASARILI, 2000);
                TabloOlustur();

            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }

        }
        private bool IzinHareketTablosundaVarMI(int izinTalepId)
        {
            bool kayitVarMi = false;
            IzinHareket ih = new IzinHareket();
            ih = ih.SelectByIzinTalepId(izinTalepId);
            if (ih == null)
            {
                kayitVarMi = false;
            }
            else
            {
                kayitVarMi = true;
            }
            return kayitVarMi;
        }
        protected void KabulBtn_Click(object sender, EventArgs e)
        {
            try
            {
                //IzinTalep_Table da onayDurumu hanesini kontrol edildi yap
                IzinTalep izinTalep = new IzinTalep();
                izinTalep = izinTalep.Select<IzinTalep>(paramIzinTalepIdLbl.Value.ConvertToInt());
                if (izinTalep != null)
                {
                    izinTalep.OnayDurumu = ProjeConstants.PER_IZINTALEBI_KONTROLEDILDI_INT;
                    izinTalep.Aciklama = string.IsNullOrEmpty(AciklamaTxt.Text) ? izinTalep.Aciklama : AciklamaTxt.Text;
                    izinTalep.Degistiren = CurrentUserName;
                    bool kontroledildi = izinTalep.Update();
                    if (kontroledildi)
                    {
                        if (izinTalep.EPostaGonder)
                        {
                            IKYSOrtak.IzinKabulRedOnayEPostasiGonder(izinTalep.PersonelId, izinTalep.Id, "Kabul");
                        }
                        MessageHelper.PublishMessage("İzin Talebi " + ProjeConstants.PER_IZINTALEBI_KONTROLEDILDI, ProjeConstants.MESAJ_BASARILI, 2000);
                        TabloOlustur();
                    }
                }


            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }

        }
        protected void ReddetBtn_Click(object sender, EventArgs e)
        {
            //IzinTalep_Table da onayDurumu hanesini reddedildi 2 yap
            IzinTalep izinTalep = new IzinTalep();
            izinTalep.Aciklama = string.IsNullOrEmpty(AciklamaTxt.Text) ? izinTalep.Aciklama : AciklamaTxt.Text;
            izinTalep = izinTalep.Select<IzinTalep>(paramIzinTalepIdLbl.Value.ConvertToInt());
            if (izinTalep != null)
            {
                izinTalep.OnayDurumu = ProjeConstants.PER_IZINTALEBI_REDDEDILDI;
                izinTalep.Degistiren = CurrentUserName;
                bool reddedildi = izinTalep.Update();
                if (reddedildi)
                {
                    if (izinTalep.EPostaGonder)
                    {
                        IKYSOrtak.IzinKabulRedOnayEPostasiGonder(izinTalep.PersonelId, izinTalep.Id, "Red");
                    }
                    MessageHelper.PublishMessage("İzin Talebi Reddedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                    //EpostaGonder(izinTalep);
                }
            }
            TabloOlustur();
        }

        protected void YazdirBtn_Click(object sender, EventArgs e)
        {
            //İzin kağıdını yazdır
        }
        //protected void Timer1_Tick(object sender, EventArgs e)
        //{
        //    KayitGetir();
        //}

        protected void IzinHareketleriBtn_Click(object sender, EventArgs e)
        {
            int personelId = paramPersonelIdLbl.Value.ConvertToInt();
            Personel personel = new Personel();
            personel = personel.Select<Personel>(personelId);
            if (personel != null)
            {
                PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                FillIzinTalebi(personel);
                FillIzinHareketleriTable(personel);
                FillIzinBilgileriTable(personel);
                if (paramOnayIdLbl.Value.Equals("kabul"))
                {
                    OnaylaModalBtn.Visible = false;
                    KabuletModalBtn.Visible = true;
                }
                else if (paramOnayIdLbl.Value.Equals("onay"))
                {
                    OnaylaModalBtn.Visible = true;
                    KabuletModalBtn.Visible = false;
                }
            }
        }

        private void FillIzinTalebi(Personel personel)
        {
            IzinTalep izinTalebi = new IzinTalep();
            izinTalebi = izinTalebi.Select<IzinTalep>(paramIzinTalepIdLbl.Value.ConvertToInt());
            if (izinTalebi != null)
            {
                IzinTanim izinTanim = new IzinTanim();
                izinTanim = izinTanim.Select<IzinTanim>(izinTalebi.IzinTipi);
                string izinTanimStr = izinTanim == null ? "" : izinTanim.Adi;
                AciklamaTxt.Text = izinTalebi.Aciklama;
                AdresLbl.Text = "İzin Adresi : " + izinTalebi.Adres;
                IzinTalebiLbl.Text = personel.Adi + " " + personel.Soyadi + " Tarafından " + izinTalebi.BaslangicTarihi.ToString("dd.MM.yyyy") + " - " +
                    izinTalebi.BitisTarihi.ToString("dd.MM.yyyy") + " Tarihleri Arasında " + izinTalebi.Sure + " " + izinTalebi.Birim
                    + " " + izinTanimStr + " İzin Talep Edilmektedir.";
            }

        }
        private class IzinTalepListItem
        {
            
            public DateTime BaslangicTarihiHidden { get; set; }
            public string PersonelId { get; set; }
            public string IzinTalepId { get; set; }
            public string IzinTipiId { get; set; }
            public string OnayDurumuId { get; set; }
            public string AdiSoyadi { get; set; }
            public string IzinTipi { get; set; }
            public string BaslangicTarihi { get; set; }
            public string BitisTarihi { get; set; }
            public string Sure { get; set; }
            public string Birim { get; set; }
            public string OnayDurumu { get; set; }
            public string Duzenle { get; set; }
            public string KayitKontrolRed { get; set; }
            public bool Secildi { get; set; }
        }
    }
}
