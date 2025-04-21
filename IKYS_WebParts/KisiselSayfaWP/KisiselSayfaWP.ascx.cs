using Model.IKYS;
using Model.Ortak;
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

namespace IKYS_WebParts.KisiselSayfaWP
{
    [ToolboxItemAttribute(false)]
    public partial class KisiselSayfaWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KisiselSayfaWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string PersonelIdQS
        {
            get
            {

                if (ViewState["PersonelId"] == null)
                {
                    if (Page.Request.QueryString["PersonelId"] != null)
                    {
                        ViewState["PersonelId"] = Page.Request.QueryString["PersonelId"];
                    }
                    else
                    {
                        ViewState["PersonelId"] = string.Empty;
                    }
                }
                return ViewState["PersonelId"].ToString();
            }

            set
            {
                ViewState["PersonelId"] = value;
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
        private string SenderAppQS
        {
            get
            {

                if (ViewState["SenderApp"] == null)
                {
                    if (Page.Request.QueryString["SenderApp"] != null)
                    {
                        ViewState["SenderApp"] = Page.Request.QueryString["SenderApp"];
                    }
                    else
                    {
                        ViewState["SenderApp"] = string.Empty;
                    }
                }
                return ViewState["SenderApp"].ToString();
            }

            set
            {
                ViewState["SenderApp"] = value;
            }
        }
        private string AuthQS
        {
            get
            {

                if (ViewState["Auth"] == null || string.IsNullOrEmpty(ViewState["Auth"].ToString()))
                {
                    if (Page.Request.QueryString["Auth"] != null)
                    {
                        ViewState["Auth"] = Page.Request.QueryString["Auth"];
                    }
                    else
                    {
                        ViewState["Auth"] = string.Empty;
                    }
                }
                return ViewState["Auth"].ToString();
            }

            set
            {
                ViewState["Auth"] = value;
            }
        }
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            Personel personel = PersonelGetir();
            if (!Page.IsPostBack)
            {

                if (personel != null)
                {                    
                    FillHeader(personel);
                    FillKimlikTable(personel);
                    FillIsyeriTable(personel);
                    FillIletisimTable(personel);
                    FillAileTable(personel);
                    TabloOlustur(personel);
                }
            }
            if (personel != null)
            {
                // FillIzinTalepleriTable()'ın postbackde de çalışması gerekiyor aksi halde sil trigger doesn't fire

                IsBilgileri ib = new IsBilgileri();
                ib = ib.SelectByPersonelId(personel.Id);
                if (ib != null)//SB 27.10.2021 Yeşim hanım işten ayrılan personelin de izin bilgilerini görmek istedi //if (ib.CalismaDurumu == ProjeConstants.PER_CALISIYOR_INT)
                {
                    FillIzinTalepleriTable(personel);//Tikkat! bu metodu if !postback içine alınca talep tablosundaki sil ve kapat butonları delegate etmiyor
                    FillIzinHareketleriTable(personel);
                    FillUcretliIzinDonemleriTable(personel);
                    FillMazeretIzinHareketleriTable(personel);
                    FillMazeretIzinDonemleriTable(personel);
                    FillDigerIzinlerTable(personel);

                }
            }
        }
        private Personel PersonelGetir()
        {
            Personel personel = new Personel();

            if (!string.IsNullOrEmpty(PersonelIdQS))
            {
                personel = personel.Select<Personel>(PersonelIdQS.ConvertToInt());

            }
            else
            {
                if (SenderAppQS.Equals("EPL"))
                {
                    personel = null;
                }
                else
                {
                    string userName = CurrentUserName.Substring(CurrentUserName.LastIndexOf("\\") + 1, CurrentUserName.Length - CurrentUserName.LastIndexOf("\\") - 1);
                    personel = personel.SelectByUserName(userName);
                }
            }
            PersonelIdQS = personel == null ? "0" : personel.Id.ToString();
            return personel;
        }
        private void FillKimlikTable(Personel personel)
        {
            Kimlik kimlik = new Kimlik();
            kimlik = kimlik.SelectByPersonelId(personel.Id);
            if (kimlik != null)
            {
                KimlikR1H1.Text = "TC Kimlik No";
                KimlikR1H2.Text = kimlik.TCKimlikNo;
                KimlikR1H3.Text = "Doğum Tarihi";
                KimlikR1H4.Text = kimlik.DogumTar.ConvertToDatetimeEmptyIfNull();
                KimlikR1H5.Text = "Doğum Yeri";
                Ilce ilce = new Ilce(kimlik.DogumYeri.ConvertToInt());
                KimlikR1H6.Text = ilce.IlceAdi + "/" + ilce.IlAdi;
                KimlikR1H7.Text = "Sicil No";
                KimlikR1H8.Text = personel.SicilNo.ReturnEmptyIfNull().ToString();


                KimlikR2H1.Text = "Anne Adı";
                KimlikR2H2.Text = kimlik.AnneAdi;
                KimlikR2H3.Text = "Baba Adı";
                KimlikR2H4.Text = kimlik.BabaAdi;
                KimlikR2H5.Text = "Medeni Hali";
                KimlikR2H6.Text = kimlik.MedeniHali.Equals("1") ? "Evli" : "Bekar";
                KimlikR2H7.Text = "Evlilik Tarihi";
                KimlikR2H8.Text = kimlik.EvlilikTar.ConvertToDatetimeEmptyIfNull();

                KimlikR3H1.Text = "Cinsiyet";
                KimlikR3H2.Text = kimlik.Cinsiyet;
                KimlikR3H3.Text = "Kan Grubu";
                KimlikR3H4.Text = kimlik.KanGrubu;
                KimlikR3H5.Text = ProjeConstants.PER_ASKERSIVIL;
                KimlikR3H6.Text = personel.Asker_sivil == ProjeConstants.PER_ASKER_INT ? ProjeConstants.PER_ASKER : ProjeConstants.PER_SIVIL;

            }
        }
        private void FillIsyeriTable(Personel personel)
        {
            IsBilgileri isBilgileri = new IsBilgileri();
            isBilgileri = isBilgileri.SelectByPersonelId(personel.Id);
            if (isBilgileri != null)
            {
                IsR1H1.Text = "Ünvan";
                int unvanId = isBilgileri.UnvanId;
                UnvanTanim ut = new UnvanTanim();
                ut = ut.Select<UnvanTanim>(unvanId);
                if (ut != null)
                {

                    IsR1H2.Text = ut.Adi;
                }
                IsR1H3.Text = "Görev";
                int gorevId = isBilgileri.GorevId;
                GorevTanim gt = new GorevTanim();
                gt = gt.Select<GorevTanim>(gorevId);
                if (gt != null)
                {
                    IsR1H4.Text = gt.Adi;
                }

                IsR1H5.Text = "Birim";
                int birimId = isBilgileri.BirimId;
                BirimTanim bt = new BirimTanim();
                bt = bt.Select<BirimTanim>(birimId);
                if (bt != null)
                {
                    IsR1H6.Text = bt.Adi;
                }
                IsR1H7.Text = "İşe Başlama Tarihi";
                IsR1H8.Text = isBilgileri.BaslamaTar.ConvertToDatetimeEmptyIfNull();
                IsR1H9.Text = "SGK Sicil No";
                IsR1H10.Text = isBilgileri.SGKSicilNo.ToString();
                IsR2H1.Text = "SGK Başlama Tar";
                IsR2H2.Text = isBilgileri.SGKBasTar.ConvertToDatetimeEmptyIfNull();

                IsR2H3.Text = "Önceki Prim Gün";
                IsR2H4.Text = isBilgileri.VakifOncesiPrimGunSayisi.ToString();
                IsR2H5.Text = "Emeklilik Tarihi";
                IsR2H6.Text = isBilgileri.EmeklilikTarihi.ConvertToDatetimeEmptyIfNull();
                IsR2H7.Text = "Protokol Sırası";
                IsR2H8.Text = isBilgileri.ProtokolSiraNo.ToString();
                IsR2H9.Text = "Çalışma Durumu";
                IsR2H10.Text = isBilgileri.CalismaDurumu == ProjeConstants.PER_CALISIYOR_INT ? ProjeConstants.PER_CALISIYOR : ProjeConstants.PER_AYRILDI;
            }
        }
        private void FillIletisimTable(Personel personel)
        {
            IletisimBilgileri iletisimBilgileri = new IletisimBilgileri();
            iletisimBilgileri = iletisimBilgileri.SelectByPersonelId(personel.Id);
            if (iletisimBilgileri != null)
            {
                TableRow row = new TableRow();
                TableCell IlR1H1 = new TableCell();
                TableCell IlR1H2 = new TableCell();
                IlR1H1.Font.Bold = true;
                IlR1H1.Text = "Adres";
                Ilce ilce = new Ilce(iletisimBilgileri.Ilcesi.ConvertToInt());
                IlR1H2.Text = iletisimBilgileri.Adres.ToString() + " " + iletisimBilgileri.Semt.ToString() +
                    " " + iletisimBilgileri.PostaKodu + ilce.IlceAdi + "/" + ilce.IlAdi;
                row.Controls.Add(IlR1H1);
                row.Controls.Add(IlR1H2);

                TableRow row0 = new TableRow();
                TableCell IlR1H30 = new TableCell();
                TableCell IlR1H40 = new TableCell();
                IlR1H30.Font.Bold = true;
                IlR1H30.Text = "Cep Telefonu 1";
                IlR1H40.Text = iletisimBilgileri.CepTelefonu;
                row0.Controls.Add(IlR1H30);
                row0.Controls.Add(IlR1H40);

                TableRow row1 = new TableRow();
                TableCell IlR1H3 = new TableCell();
                TableCell IlR1H4 = new TableCell();
                IlR1H3.Font.Bold = true;
                IlR1H3.Text = "Cep Telefonu 2";
                IlR1H4.Text = iletisimBilgileri.CepTelefonu2;
                row1.Controls.Add(IlR1H3);
                row1.Controls.Add(IlR1H4);


                TableRow row2 = new TableRow();
                TableCell IlR1H5 = new TableCell();
                TableCell IlR1H6 = new TableCell();
                IlR1H5.Font.Bold = true;
                IlR1H5.Text = "Ev Telefonu";
                IlR1H6.Text = iletisimBilgileri.EvTelefonu;
                row2.Controls.Add(IlR1H5);
                row2.Controls.Add(IlR1H6);

                TableRow row3 = new TableRow();
                TableCell IlR2H1 = new TableCell();
                TableCell IlR2H2 = new TableCell();
                IlR2H1.Font.Bold = true;
                IlR2H1.Text = "Intranet EPosta";
                IlR2H2.Text = iletisimBilgileri.IntranetEPosta;
                row3.Controls.Add(IlR2H1);
                row3.Controls.Add(IlR2H2);

                TableRow row4 = new TableRow();
                TableCell IlR2H3 = new TableCell();
                TableCell IlR2H4 = new TableCell();
                IlR2H3.Font.Bold = true;
                IlR2H3.Text = "Internet EPosta";
                IlR2H4.Text = iletisimBilgileri.InternetEPosta;
                row4.Controls.Add(IlR2H3);
                row4.Controls.Add(IlR2H4);

                TableRow row5 = new TableRow();
                TableCell IlR2H5 = new TableCell();
                TableCell IlR2H6 = new TableCell();
                IlR2H5.Font.Bold = true;
                IlR2H5.Text = "Özel EPosta";
                IlR2H6.Text = iletisimBilgileri.OzelEPosta;
                row5.Controls.Add(IlR2H5);
                row5.Controls.Add(IlR2H6);

                TableRow row6 = new TableRow();
                TableCell IlR2H7 = new TableCell();
                TableCell IlR2H8 = new TableCell();
                IlR2H7.Font.Bold = true;
                IlR2H7.Text = "Dahili Telefonu";
                IlR2H8.Text = iletisimBilgileri.DahiliTelefonu;
                row6.Controls.Add(IlR2H7);
                row6.Controls.Add(IlR2H8);

                TableRow row7 = new TableRow();
                TableCell IlR2H9 = new TableCell();
                TableCell IlR2H10 = new TableCell();
                IlR2H9.Font.Bold = true;
                IlR2H9.Text = "Araç Plakası";
                IlR2H10.Text = iletisimBilgileri.Plaka;
                row7.Controls.Add(IlR2H9);
                row7.Controls.Add(IlR2H10);

                IletisimTable.Controls.Add(row);
                IletisimTable.Controls.Add(row0);
                IletisimTable.Controls.Add(row1);
                IletisimTable.Controls.Add(row2);
                IletisimTable.Controls.Add(row3);
                IletisimTable.Controls.Add(row4);
                IletisimTable.Controls.Add(row5);
                IletisimTable.Controls.Add(row6);
                IletisimTable.Controls.Add(row7);
            }
        }
        private void FillAileTable(Personel personel)
        {
            //Column headers
            HeaderCell1.Text = "Adı Soyadı";
            HeaderCell1.Visible = true;
            HeaderCell2.Text = "Yak.Derecesi";
            HeaderCell2.Visible = true;
            HeaderCell3.Text = "Doğ.Tarihi";
            HeaderCell3.Visible = true;
            HeaderCell4.Text = "Okul";
            HeaderCell4.Visible = true;
            HeaderCell5.Text = "Meslek";
            HeaderCell5.Visible = true;
            HeaderCell6.Text = "Telefon";
            HeaderCell6.Visible = true;

            Aile aileDao = new Aile();
            List<Aile> list = aileDao.SelectByPersonelId(personel.Id);
            int SiraNo = 1;
            foreach (Aile aile in list)
            {
                TableRow row = new TableRow();

                TableCell PupUpCell0 = new TableCell();

                PupUpCell0.Text = SiraNo++ + "";
                row.Controls.Add(PupUpCell0);

                TableCell AdiSoyadiCell = new TableCell();
                AdiSoyadiCell.Text = aile.Adi.ReturnEmptyIfNull().ToString() + " " + aile.Soyadi.ReturnEmptyIfNull().ToString();
                row.Controls.Add(AdiSoyadiCell);

                TableCell YakinlikCell = new TableCell();

                string yakinlikDerecesi = aile.YakinlikDerecesi.ReturnEmptyIfNull().ToString();
                if (yakinlikDerecesi.Equals("1"))
                    yakinlikDerecesi = ProjeConstants.PER_YAKINLIKDERECESI_ES;
                else if (yakinlikDerecesi.Equals("2"))
                    yakinlikDerecesi = ProjeConstants.PER_YAKINLIKDERECESI_COCUK;
                YakinlikCell.Text = yakinlikDerecesi.ReturnEmptyIfNull().ToString();
                row.Controls.Add(YakinlikCell);

                TableCell DogumTarCell = new TableCell();
                DogumTarCell.Text = aile.DogumTar.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(DogumTarCell);

                //TableCell TahsilCell = new TableCell();
                //TahsilCell.Text = Utility.getString(aile.Tahsil);
                //row.Controls.Add(TahsilCell);

                TableCell OkulCell = new TableCell();
                OkulCell.Text = aile.Okul.ReturnEmptyIfNull().ToString();
                row.Controls.Add(OkulCell);

                TableCell MeslekCell = new TableCell();
                Meslek meslekDao = new Meslek();
                Meslek meslek = meslekDao.Select<Meslek>(aile.Meslek);
                MeslekCell.Text = meslek.Adi.ReturnEmptyIfNull().ToString();
                row.Controls.Add(MeslekCell);

                TableCell TelefonCell = new TableCell();
                TelefonCell.Text = aile.Telefon.ReturnEmptyIfNull().ToString();
                row.Controls.Add(TelefonCell);

                AileTable.Controls.Add(row);
            }

        }
        private void FillHeader(Personel personel)
        {
            //string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            //string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
            //string fotostr = string.IsNullOrEmpty(personel.KullaniciAdi) ? personel.Adi.ReturnEmptyIfNull().ToString().Substring(0, 1) + personel.Soyadi.ReturnEmptyIfNull().ToString() : personel.KullaniciAdi.ReturnEmptyIfNull().ToString();
            //string imgUrl = newUrl + "/../" + ProjeConstants.RESIMLER_PERSONEL + "/" + fotostr.ReplaceTrChars() + ".jpg";
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string hostUrl = currentUrl.Substring(0, currentUrl.LastIndexOf(System.Web.HttpContext.Current.Request.Url.AbsolutePath));

            string fotostr = string.IsNullOrEmpty(personel.KullaniciAdi) ? personel.Adi.ReturnEmptyIfNull().ToString().Substring(0, 1) + personel.Soyadi.ReturnEmptyIfNull().ToString() : personel.KullaniciAdi.ReturnEmptyIfNull().ToString();
            string imgUrl = hostUrl + ProjeConstants.PATH_RESIMLER_PERSONEL + fotostr.ReplaceTrChars() + ".jpg";

            PersonelImg.Src = imgUrl;

            IsBilgileri ib = new IsBilgileri();
            ib = ib.SelectByPersonelId(personel.Id);
            if (ib.CalismaDurumu == ProjeConstants.PER_CALISIYOR_INT)
            {
                KisiselSayfaLbl.Text = personel.Adi + " " + personel.Soyadi;
                KisiselSayfaLbl.CssClass = "col-form-label  btn-outline-primary";
            }
            else
            {
                KisiselSayfaLbl.Text = personel.Adi + " " + personel.Soyadi + " ( A Y R I L D I ) ";
                KisiselSayfaLbl.CssClass = "col-form-label  btn-outline-secondary";
            }
        }
        private void FillUcretliIzinDonemleriTable(Personel personel)
        {
            IsBilgileri ib = new IsBilgileri();
            ib = ib.SelectByPersonelId(personel.Id);
            string baslamaTarStr = ib == null ? "" : ib.BaslamaTar.ConvertToDatetimeEmptyIfNull();
            string izinDonemiBasTarStr = ib == null ? "" : ib.IzinDonemiBasTar.ConvertToDatetimeEmptyIfNull();

            izinDonemiBasTarStr = string.IsNullOrEmpty(izinDonemiBasTarStr) ? baslamaTarStr : izinDonemiBasTarStr;
            if (!string.IsNullOrEmpty(izinDonemiBasTarStr))
            {

                DateTime izinDonemiBasTar = izinDonemiBasTarStr.ConvertToDatetime();// ib.IzinDonemiBasTar;                      
                IzinDonem izinDonemDao = new IzinDonem();
                List<IzinDonem> izinDonemiList = izinDonemDao.SelectByPersonelId(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT);

                IzinHareket izinHareket = new IzinHareket();

                UcretliIzinDonemleriTableHeaders();
                bool printOnce = !AuthQS.Equals("IKYS");
                bool printed = false;
                foreach (IzinDonem izinDonemi in izinDonemiList)
                {
                    //if (printOnce && printed)//standart kullanıcı ise sadece ilk dönemi bas
                    //{
                    //    continue;
                    //}
                    //else
                    {
                        DateTime today = DateTime.Today;
                        IzinDonem buIzinDonemi = new IzinDonem();
                        buIzinDonemi = buIzinDonemi.SelectByIzinTarihi(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, today);
                        if ((buIzinDonemi != null) && (buIzinDonemi.Id == izinDonemi.Id))
                        {
                            printed = true;
                        }

                        TableRow row = new TableRow();
                        TableCell IzinDonemiCell = new TableCell();
                        IzinDonemiCell.Text = izinDonemi.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + izinDonemi.BitisTarihi.ConvertToDatetimeEmptyIfNull();

                        Mahsup mahsup = new Mahsup();
                        List<Mahsup> mahsupList = mahsup.SelectByDonemId(izinDonemi.Id);
                        if (mahsupList.Count > 0)
                            IzinDonemiCell.Text += "(M)";
                        row.Controls.Add(IzinDonemiCell);

                        string izinHakki = string.Empty;
                        TableCell IzinHakkiCell = new TableCell();
                        izinHakki = izinDonemi.IzinHakki;
                        IzinHakkiCell.Text = izinHakki.ReturnZeroIfNull().ToString() + " " + izinDonemi.Birim;
                        row.Controls.Add(IzinHakkiCell);

                        TableCell KullanilanIzinCell = new TableCell();
                        KullanilanIzinCell.Text = izinDonemi.KullanilanIzin.ReturnZeroIfNull().ToString() + " " + izinDonemi.Birim;
                        row.Controls.Add(KullanilanIzinCell);

                        TableCell KalanIzinCell = new TableCell();
                        KalanIzinCell.Text = izinDonemi.KalanIzin.ReturnZeroIfNull().ToString() + " " + izinDonemi.Birim;
                        row.Controls.Add(KalanIzinCell);
                        UcretliIzinDonemleriTable.Controls.Add(row);

                    }
                }
            }
            else
            {
                MessageHelper.PublishMessage("İşe başlama tarihi belirlenemedi", ProjeConstants.MESAJ_HATA);
            }
        }
        private void FillMazeretIzinDonemleriTable(Personel personel)
        {
            DateTime today = DateTime.Today;
            IsBilgileri ib = new IsBilgileri();
            ib = ib.SelectByPersonelId(personel.Id);
            string baslamaTarStr = ib == null ? "" : ib.BaslamaTar.ConvertToDatetimeEmptyIfNull();
            string izinDonemiBasTarStr = ib == null ? "" : ib.IzinDonemiBasTar.ConvertToDatetimeEmptyIfNull();

            izinDonemiBasTarStr = string.IsNullOrEmpty(izinDonemiBasTarStr) ? baslamaTarStr : izinDonemiBasTarStr;
            if (!string.IsNullOrEmpty(izinDonemiBasTarStr))
            {

                DateTime izinDonemiBasTar = izinDonemiBasTarStr.ConvertToDatetime();// ib.IzinDonemiBasTar;
                IzinDonem izinDonemDao = new IzinDonem();
                List<IzinDonem> izinDonemiList = izinDonemDao.SelectByPersonelId(personel.Id, ProjeConstants.IZINTIPI_MAZERET_INT);

                IzinHareket izinHareket = new IzinHareket();

                MazeretIzinDonemleriTableHeaders();
                bool printOnce = !AuthQS.Equals("IKYS");
                bool printed = false;
                foreach (IzinDonem izinDonemi in izinDonemiList)
                {
                    if (printOnce && printed)//standart kullanıcı ise sadece ilk dönemi bas
                    {
                        continue;
                    }
                    else
                    {
                        IzinDonem buIzinDonemi = new IzinDonem();
                        buIzinDonemi = buIzinDonemi.SelectByIzinTarihi(personel.Id, ProjeConstants.IZINTIPI_MAZERET_INT, today);
                        if ((buIzinDonemi != null) && (buIzinDonemi.Id == izinDonemi.Id))
                        {
                            printed = true;
                        }
                        TableRow row = new TableRow();
                        TableCell IzinDonemiCell = new TableCell();
                        IzinDonemiCell.Text = izinDonemi.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + izinDonemi.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                        row.Controls.Add(IzinDonemiCell);

                        string izinHakki = string.Empty;
                        TableCell IzinHakkiCell = new TableCell();
                        izinHakki = izinDonemi.IzinHakki;
                        IzinHakkiCell.Text = izinHakki.ConvertToTimeSpanReturnInHHmm();// izinHakki.ReturnZeroIfNull().ToString() + " " + izinDonemi.Birim;
                        row.Controls.Add(IzinHakkiCell);

                        TableCell KullanilanIzinCell = new TableCell();
                        KullanilanIzinCell.Text = izinDonemi.KullanilanIzin.ConvertToTimeSpanReturnInHHmm();//izinDonemi.KullanilanIzin.ReturnZeroIfNull().ToString() + " " + izinDonemi.Birim;
                        row.Controls.Add(KullanilanIzinCell);

                        TableCell KalanIzinCell = new TableCell();
                        KalanIzinCell.Text = izinDonemi.KalanIzin.ConvertToTimeSpanReturnInHHmm();//izinDonemi.KalanIzin.ReturnZeroIfNull().ToString() + " " + izinDonemi.Birim;
                        row.Controls.Add(KalanIzinCell);
                        MazeretIzinDonemleriTable.Controls.Add(row);
                    }


                }
            }
            else
            {
                MessageHelper.PublishMessage("İşe başlama tarihi belirlenemedi", ProjeConstants.MESAJ_HATA);
            }
        }
        private void UcretliIzinDonemleriTableHeaders()
        {
            UcretliIzinDonemleriTable.Rows.Clear();
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
            UcretliIzinDonemleriTable.Controls.Add(th);
        }
        private void MazeretIzinDonemleriTableHeaders()
        {
            MazeretIzinDonemleriTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell donemCell = new TableHeaderCell();
            donemCell.Text = "Mazeret İzin Dönemi";
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
            MazeretIzinDonemleriTable.Controls.Add(th);
        }
        private void IzinHareketTableHeaders()
        {
            UcretliIzinHareketTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell izintipiCell = new TableHeaderCell();
            izintipiCell.Text = "İzin Tipi";
            TableHeaderCell bastarCell = new TableHeaderCell();
            bastarCell.Text = "Başlangıç-Bitiş";
            //TableHeaderCell bittarCell = new TableHeaderCell();
            //bittarCell.Text = "Bitiş tarihi";
            TableHeaderCell sureCell = new TableHeaderCell();
            sureCell.Text = "İzinli Süre";
            TableHeaderCell dilekceCell = new TableHeaderCell();
            dilekceCell.Text = "Düzeltme/İptal";
            th.Controls.Add(siraCell);
            th.Controls.Add(izintipiCell);
            th.Controls.Add(bastarCell);
            //th.Controls.Add(bittarCell);
            th.Controls.Add(sureCell);
            th.Controls.Add(dilekceCell);
            //th.Controls.Add(yazdirCell);
            UcretliIzinHareketTable.Controls.Add(th);
        }
        private void DigerIzinlerTableHeaders()
        {
            DigerIzinlerTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell izintipiCell = new TableHeaderCell();
            izintipiCell.Text = "İzin Tipi";
            TableHeaderCell bastarCell = new TableHeaderCell();
            bastarCell.Text = "Başlangıç Tarihi";
            TableHeaderCell bittarCell = new TableHeaderCell();
            bittarCell.Text = "Bitiş Tarihi";
            TableHeaderCell sureCell = new TableHeaderCell();
            sureCell.Text = "İzinli Süre";

            th.Controls.Add(siraCell);
            th.Controls.Add(izintipiCell);
            th.Controls.Add(bastarCell);
            th.Controls.Add(bittarCell);
            th.Controls.Add(sureCell);

            DigerIzinlerTable.Controls.Add(th);
        }
        private void MazeretIzinHareketTableHeaders()
        {
            MazeretIzinHareketTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell izintipiCell = new TableHeaderCell();
            izintipiCell.Text = "Tarih";
            TableHeaderCell bastarCell = new TableHeaderCell();
            bastarCell.Text = "Başlangıç Saati";
            TableHeaderCell bittarCell = new TableHeaderCell();
            bittarCell.Text = "Bitiş Saati";
            TableHeaderCell sureCell = new TableHeaderCell();
            sureCell.Text = "İzinli Süre";
            TableHeaderCell yazdirCell = new TableHeaderCell();
            yazdirCell.Text = "Yazdır";
            th.Controls.Add(siraCell);
            th.Controls.Add(izintipiCell);
            th.Controls.Add(bastarCell);
            th.Controls.Add(bittarCell);
            th.Controls.Add(sureCell);
            //th.Controls.Add(yazdirCell);
            MazeretIzinHareketTable.Controls.Add(th);
        }
        private void FillIzinHareketleriTable(Personel personel)
        {
            DateTime today = DateTime.Today;
            DateTime threeMonthsLater = DateTime.Today.AddMonths(3);
            IsBilgileri ib = new IsBilgileri();
            ib = ib.SelectByPersonelId(personel.Id);
            string baslamaTarStr = ib == null ? "" : ib.BaslamaTar.ConvertToDatetimeEmptyIfNull();
            string izinDonemiBasTarStr = ib == null ? "" : ib.IzinDonemiBasTar.ConvertToDatetimeEmptyIfNull();

            izinDonemiBasTarStr = string.IsNullOrEmpty(izinDonemiBasTarStr) ? baslamaTarStr : izinDonemiBasTarStr;
            if (!string.IsNullOrEmpty(izinDonemiBasTarStr))
            {

                IzinHareket izinHareket = new IzinHareket();
                IzinDonem izinDonem = new IzinDonem();
                izinDonem = izinDonem.SelectByIzinTarihi(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, today);
                if (izinDonem != null)
                {
                    DateTime izinDonemiBasi = izinDonem != null ? izinDonem.BaslangicTarihi : today.AddYears(-1);
                    DateTime izinDonemiSonu = izinDonem != null ? izinDonem.BitisTarihi : today.AddMonths(1); ;
                    DataTable dataTable = izinHareket.SelectByIzinDonemiReturnDataTable(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, izinDonemiBasi, threeMonthsLater);//3 ay içinde yeni izin dönemi başlıyor olabilir
                    //DataTable dataTable = izinHareket.SelectByIzinDonemiReturnDataTable(izinDonem == null ? 0 : izinDonem.Id, personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT);
                    int SiraNo = 1;
                    if (dataTable == null)
                    {
                        UcretliIzinHareketTable.Rows.Clear();
                        TableRow tr = new TableRow();
                        TableCell tc = new TableCell();
                        tc.Text = "Henüz izin kullanılmamış.";
                        tr.Controls.Add(tc);
                        UcretliIzinHareketTable.Controls.Add(tr);
                    }
                    else
                    {
                        IzinHareketTableHeaders();
                        bool printed = false;
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            string izinHareketId = dataRow["IzinHareketId"].ConvertToInt().ToString();
                            string izinTipi = dataRow["IzinTipi"].ReturnEmptyIfNull().ToString();
                            string aciklama = dataRow["Aciklama"].ReturnEmptyIfNull().ToString();
                            bool mahsup = dataRow["Mahsup"].ReturnFalseIfNull().ConvertToBool();
                            izinTipi = mahsup == false ? izinTipi : izinTipi + "(M)";
                            DateTime bastar = dataRow["BaslangicTarihi"].ConvertToDatetime();
                            DateTime bittar = dataRow["BitisTarihi"].ConvertToDatetime();
                            string sure = dataRow["Sure"].ToString();
                            string birim = dataRow["Birim"].ToString();
                            int izinTalepId = dataRow["IzinTalepId"].ConvertToInt();
                            int izinDonemId = dataRow["IzinDonemId"].ConvertToInt();

                            TableRow row = new TableRow();
                            row.ToolTip = aciklama;
                            TableCell SiraNoCell = new TableCell();

                            SiraNoCell.Text = SiraNo++ + "";
                            row.Controls.Add(SiraNoCell);

                            TableCell IzinTipiCell = new TableCell();
                            IzinTipiCell.Text = izinTipi;
                            row.Controls.Add(IzinTipiCell);

                            TableCell BasTarCell = new TableCell();
                            BasTarCell.Text = bastar.ConvertToDatetimeEmptyIfNull() + "-" + bittar.ConvertToDatetimeEmptyIfNull();
                            row.Controls.Add(BasTarCell);

                            //TableCell BitTarCell = new TableCell();
                            //BitTarCell.Text = bittar.ConvertToDatetimeEmptyIfNull();
                            //row.Controls.Add(BitTarCell);

                            TableCell SureCell = new TableCell();
                            SureCell.Text = sure + " " + birim;
                            row.Controls.Add(SureCell);
                            TableCell DilekceCell = new TableCell();
                            if (!mahsup && !printed)
                            {

                                int fark = (today - bittar).Days;
                                if (fark < 8)
                                {
                                    printed = true;
                                    LinkButton DilekceBtn = new LinkButton();
                                    DilekceBtn.Text = "Değişiklik/İptal";
                                    DilekceBtn.ID = "DilekceBtn" + SiraNo;
                                    TableUpdatePanel.ContentTemplateContainer.Controls.Add(DilekceBtn);
                                    DilekceBtn.CssClass = "btn btn-outline-primary";
                                    DilekceBtn.Click += delegate
                                    {
                                        //izinde değişiklik yapılacak alanların bilgisini al
                                        //değişiklik/silme için dilekçe bastır
                                        //IzinHareketEdit.aspx? IzinHareketId = '+rowData.IzinHareketId + ' & PersonelId = '+rowData.PersonelId +  ' & IzinTanimId = '+rowData.IzinTipiId
                                        RedirectToPage(ProjeConstants.PAGE_IZINHAREKET_EDIT + "?Auth=PER&SenderApp=KS&DestinationApp=DIL&IzinHareketId=" + izinHareketId + "&PersonelId=" + personel.Id);
                                    };
                                    DilekceCell.Controls.Add(DilekceBtn);
                                }

                            }
                            row.Controls.Add(DilekceCell);

                            if (bastar > izinDonemiSonu) //gelecek izin dönemine aitse farklı renk yazdır
                            {
                                SiraNoCell.ForeColor = System.Drawing.Color.Red;
                                IzinTipiCell.ForeColor = System.Drawing.Color.Red;
                                BasTarCell.ForeColor = System.Drawing.Color.Red;
                                //BitTarCell.ForeColor = System.Drawing.Color.Red;
                                SureCell.ForeColor = System.Drawing.Color.Red;

                                row.ToolTip = "Yeni İzin Dönemi";
                            }
                            UcretliIzinHareketTable.Controls.Add(row);
                        }
                    }
                }

            }
            else
            {
                // MessageHelper.PublishMessage("İşe başlama tarihi belirlenemedi", ProjeConstants.MESAJ_HATA);
            }
        }
        private void FillMazeretIzinHareketleriTable(Personel personel)
        {
            DateTime today = DateTime.Today;
            DateTime threeMonthsLater = DateTime.Today.AddMonths(3);
            IsBilgileri ib = new IsBilgileri();
            ib = ib.SelectByPersonelId(personel.Id);
            string baslamaTarStr = ib == null ? "" : ib.BaslamaTar.ConvertToDatetimeEmptyIfNull();
            string izinDonemiBasTarStr = ib == null ? "" : ib.IzinDonemiBasTar.ConvertToDatetimeEmptyIfNull();

            izinDonemiBasTarStr = string.IsNullOrEmpty(izinDonemiBasTarStr) ? baslamaTarStr : izinDonemiBasTarStr;
            if (!string.IsNullOrEmpty(izinDonemiBasTarStr))
            {


                IzinHareket izinHareket = new IzinHareket();
                IzinDonem izinDonem = new IzinDonem();
                izinDonem = izinDonem.SelectByIzinTarihi(personel.Id, ProjeConstants.IZINTIPI_MAZERET_INT, today);

                if (izinDonem != null)
                {
                    //DateTime izinDonemiBasi = izinDonem != null ? izinDonem.BaslangicTarihi : today.AddYears(-1);
                    DateTime izinDonemiSonu = izinDonem != null ? izinDonem.BitisTarihi : today.AddMonths(1); ;
                    //DataTable dataTable = izinHareket.SelectByIzinDonemiReturnDataTable(personel.Id,ProjeConstants.IZINTIPI_MAZERET_INT, izinDonemiBasi, threeMonthsLater);//3 ay içinde yeni izin dönemi başlıyor olabilir
                    DataTable dataTable = izinHareket.SelectByIzinDonemiReturnDataTable(izinDonem == null ? 0 : izinDonem.Id, personel.Id, ProjeConstants.IZINTIPI_MAZERET_INT);
                    int SiraNo = 1;
                    if (dataTable == null)
                    {
                        MazeretIzinHareketTable.Rows.Clear();
                        TableRow tr = new TableRow();
                        TableCell tc = new TableCell();
                        tc.Text = "Henüz izin kullanılmamış.";
                        tr.Controls.Add(tc);
                        MazeretIzinHareketTable.Controls.Add(tr);
                    }
                    else
                    {
                        MazeretIzinHareketTableHeaders();
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            DateTime bastar = dataRow["BaslangicTarihi"].ConvertToDatetime();
                            DateTime bittar = dataRow["BitisTarihi"].ConvertToDatetime();
                            string bitSaat = bittar.ToString("HH:mm");
                            string basSaat = bastar.ToString("HH:mm");
                            string sure = dataRow["Sure"].ToString();
                            string birim = dataRow["Birim"].ToString();
                            int izinHareketId = dataRow["IzinHareketId"].ConvertToInt();
                            int izinTalepId = dataRow["IzinTalepId"].ConvertToInt();
                            TableRow row = new TableRow();

                            TableCell SiraNoCell = new TableCell();

                            SiraNoCell.Text = SiraNo++ + "";
                            row.Controls.Add(SiraNoCell);

                            TableCell BasTarCell = new TableCell();
                            BasTarCell.Text = bastar.ConvertToDatetimeEmptyIfNull();
                            row.Controls.Add(BasTarCell);

                            TableCell BasSaatCell = new TableCell();

                            BasSaatCell.Text = basSaat;
                            row.Controls.Add(BasSaatCell);

                            TableCell BitSaatCell = new TableCell();

                            BitSaatCell.Text = bitSaat;
                            row.Controls.Add(BitSaatCell);

                            TableCell SureCell = new TableCell();
                            SureCell.Text = sure.ConvertToTimeSpanReturnInHHmm();//sure + " "+ birim;
                            row.Controls.Add(SureCell);

                            if (bastar > izinDonemiSonu) //gelecek izin dönemine aitse farklı renk yazdır
                            {
                                SiraNoCell.ForeColor = System.Drawing.Color.Red;
                                BasTarCell.ForeColor = System.Drawing.Color.Red;
                                BasSaatCell.ForeColor = System.Drawing.Color.Red;
                                BitSaatCell.ForeColor = System.Drawing.Color.Red;
                                SureCell.ForeColor = System.Drawing.Color.Red;

                                row.ToolTip = "Yeni İzin Dönemi";
                            }
                            MazeretIzinHareketTable.Controls.Add(row);
                        }
                    }
                }

            }
            else
            {
                MessageHelper.PublishMessage("İşe başlama tarihi belirlenemedi", ProjeConstants.MESAJ_HATA);
            }
        }
        private void IzinTalepTableHeaders()
        {
            IzinTalepTable.Rows.Clear();
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
            sureCell.Text = "İzin Süresi";
            TableHeaderCell birimCell = new TableHeaderCell();
            birimCell.Text = "Birim";
            TableHeaderCell durumCell = new TableHeaderCell();
            durumCell.Text = "Durum";
            TableHeaderCell btnCell = new TableHeaderCell();
            btnCell.Text = "";

            th.Controls.Add(siraCell);
            th.Controls.Add(izintipiCell);
            th.Controls.Add(bastarCell);
            th.Controls.Add(bittarCell);
            th.Controls.Add(sureCell);
            //th.Controls.Add(birimCell);
            th.Controls.Add(durumCell);
            th.Controls.Add(btnCell);

            IzinTalepTable.Controls.Add(th);
        }
        private void FillDigerIzinlerTable(Personel personel)
        {
            DigerIzinlerTableHeaders();
            IzinHareket izinHareket = new IzinHareket();

            List<IzinHareket> list = izinHareket.SelectDigerIzinlerByPersonelIdReturnJson(personel.Id);
            int SiraNo = 1;
            foreach (var item in list)
            {
                TableRow row = new TableRow();
                TableCell SiraNoCell = new TableCell();
                SiraNoCell.Text = SiraNo++ + "";
                row.Controls.Add(SiraNoCell);

                TableCell IzinTipiCell = new TableCell();
                IzinTanim it = new IzinTanim();
                it = it.Select<IzinTanim>(item.IzinTipi);
                IzinTipiCell.Text = it == null ? "" : it.Adi;
                row.Controls.Add(IzinTipiCell);

                TableCell BasTarCell = new TableCell();
                BasTarCell.Text = item.BaslangicTarihi.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(BasTarCell);

                TableCell BitTarCell = new TableCell();
                BitTarCell.Text = item.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(BitTarCell);

                TableCell SureCell = new TableCell();
                SureCell.Text = item.Sure + " " + item.Birim + (item.IzinTipi == ProjeConstants.IZINTIPI_SUTIZNI_INT ? "(Günde 1 Saat 30 Dk.)" : "");
                row.Controls.Add(SureCell);
                DigerIzinlerTable.Controls.Add(row);
            }

            if (list.Count < 1)
            {
                DigerIzinlerTable.Rows.Clear();
                TableRow tr = new TableRow();
                TableCell tc = new TableCell();
                tc.Text = "Henüz izin kullanılmamış.";
                tr.Controls.Add(tc);
                DigerIzinlerTable.Controls.Add(tr);
            }
        }
        private void FillIzinTalepleriTable(Personel personel)
        {

            IzinTalep izinTalep = new IzinTalep();
            DataTable dataTable = izinTalep.SelectIzinTalepleriReturnDT(personel.Id, 0, false, true);
            int SiraNo = 1;
            if (dataTable == null)
            {
                IzinTalepTable.Rows.Clear();
                TableRow tr = new TableRow();
                TableCell tc = new TableCell();
                tc.Text = "İzin talebi bulunmamaktadır.";
                tr.Controls.Add(tc);
                IzinTalepTable.Controls.Add(tr);
            }
            else
            {
                IzinTalepTableHeaders();
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    string basTar = dataRow["BaslangicTarihi"].ConvertToDatetimeEmptyIfNull();
                    string bitTar = dataRow["BitisTarihi"].ConvertToDatetimeEmptyIfNull();
                    string birim = dataRow["Birim"].ToString();
                    string sure = dataRow["Sure"].ToString();
                    string sureDb = dataRow["Sure"].ToString();
                    int onayDurumuId = dataRow["OnayDurumuId"].ConvertToInt();
                    string onayDurumu = dataRow["OnayDurumu"].ReturnZeroIfNull().ToString();
                    string aciklama = dataRow["Aciklama"].ReturnZeroIfNull().ToString();
                    string izinTipi = dataRow["IzinTipi"].ReturnEmptyIfNull().ToString();
                    int izinTipiId = dataRow["IzinTipiId"].ConvertToInt();
                    int izinTalepId = dataRow["IzinTalepId"].ConvertToInt();
                    int izinDonemId = dataRow["IzinDonemId"].ConvertToInt();
                    if (izinTipiId == ProjeConstants.IZINTIPI_MAZERET_INT)
                    {
                        basTar = dataRow["BaslangicTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy HH:mm");
                        bitTar = dataRow["BitisTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy HH:mm");
                        sure = sure.ConvertToTimeSpanReturnInHHmm();
                    }
                    else
                    {
                        sure = sure + " " + birim;
                    }

                    TableRow row = new TableRow();

                    TableCell SiraNoCell = new TableCell();

                    SiraNoCell.Text = SiraNo++ + "";
                    row.Controls.Add(SiraNoCell);

                    TableCell IzinTipiCell = new TableCell();
                    IzinTipiCell.Text = izinTipi;
                    row.Controls.Add(IzinTipiCell);

                    TableCell BasTarCell = new TableCell();
                    BasTarCell.Text = basTar;
                    row.Controls.Add(BasTarCell);

                    TableCell BitTarCell = new TableCell();
                    BitTarCell.Text = bitTar;
                    row.Controls.Add(BitTarCell);

                    TableCell SureCell = new TableCell();
                    SureCell.Text = sure;
                    row.Controls.Add(SureCell);

                    TableCell OnayDurumuCell = new TableCell();
                    OnayDurumuCell.Text = onayDurumu;
                    OnayDurumuCell.ToolTip = aciklama.ReturnEmptyIfNull().ToString();
                    row.Controls.Add(OnayDurumuCell);

                    TableCell KapatCell = new TableCell();
                    if (onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_ISLEMBEKLIYOR) ||
                        onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_DILEKCEBEKLIYOR))
                    {
                        TableCell SilCell = new TableCell();
                        LinkButton SilBtn = new LinkButton();
                        SilBtn.Text = "Sil";
                        SilBtn.ID = "SilBtn" + SiraNo;
                        TableUpdatePanel.ContentTemplateContainer.Controls.Add(SilBtn);
                        SilBtn.CssClass = "btn btn-outline-danger";
                        SilBtn.Click += delegate
                        {

                            izinTalep = izinTalep.Select<IzinTalep>(izinTalepId);
                            if (izinTalep != null)
                            {
                                bool isdeleted = izinTalep.Delete();
                                if (isdeleted)
                                    MessageHelper.PublishMessage("İzin talebi silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                                else
                                    MessageHelper.PublishMessage("İzin talebi silinemedi ", ProjeConstants.MESAJ_HATA);
                                //FillIzinTalepleriTable(personel);  zaten postback de yeniden yükleniyor
                                Timer1_Tick(null, null);
                            }
                            else
                            {
                                MessageHelper.PublishMessage("İzin talebi bulunamadı, talebiniz zaten silinmiş olabilir. Lütfen kontolediniz.", ProjeConstants.MESAJ_BILGI);
                            }

                        };
                        SilCell.Controls.Add(SilBtn);
                        row.Controls.Add(SilCell);
                    }
                    else if (onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_KONTROLEDILDI_INT))
                    {
                        TableCell YazdirCell = new TableCell();
                        HyperLink yazdirLnk = new HyperLink();
                        yazdirLnk.Target = "_blank";
                        yazdirLnk.Text = "Yazdır";
                        string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                        string rawUrl = System.Web.HttpContext.Current.Request.RawUrl.ToString();
                        int index = currentUrl.IndexOf(rawUrl);
                        string rootUrl = (index < 0) ? currentUrl : currentUrl.Remove(index, rawUrl.Length);
                        string sonIzin = KalacakIzinHesapla(izinDonemId, sureDb);

                        var fullUrl = "";

                        if (izinTipiId.Equals(ProjeConstants.IZINTIPI_UCRETLI_INT))
                        {
                            fullUrl = string.Format("{0}?IzinTalepId={1}&IzinDonemId={2}&SonIzin={3}", rootUrl + ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, izinDonemId, sonIzin);//string.Format("{0}?rv:RelativeReportUrl={1}&rp:IzinTalepId={2}&rp:IzinDonemId={3}", ProjeConstants.RAPOR_SUNUCU_SAYFASI, ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, izinDonemId);
                        }
                        else if (izinTipiId.Equals(ProjeConstants.IZINTIPI_MAZERET_INT))
                        {
                            IzinDonem id = new IzinDonem();
                            id = id.Select<IzinDonem>(izinDonemId);
                            string kalanIzin = string.Empty;
                            if (id != null)
                            {
                                kalanIzin = id.KalanIzin.ConvertToTimeSpanReturnInHHmm();
                            }

                            fullUrl = string.Format("{0}?IzinTalepId={1}&SureStr={2}&KalanIzinStr={3}&SonIzin={4}", rootUrl + ProjeConstants.RAPOR_MAZERETIZINBELGESI_URL, izinTalepId, sure, kalanIzin, sonIzin);
                        }
                        else //diger İzinler
                        {
                            fullUrl = string.Format("{0}?IzinTalepId={1}&IzinDonemId={2}", rootUrl + ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, izinDonemId);
                        }
                        yazdirLnk.NavigateUrl = fullUrl;
                        YazdirCell.Controls.Add(yazdirLnk);
                        YazdirCell.HorizontalAlign = HorizontalAlign.Left;
                        row.Controls.Add(YazdirCell);
                    }
                    else
                    {
                        LinkButton kapatBtn = new LinkButton();
                        kapatBtn.Text = "&times;";
                        kapatBtn.ID = "kapatBtn" + SiraNo;
                        TableUpdatePanel.ContentTemplateContainer.Controls.Add(kapatBtn);
                        kapatBtn.CssClass = "btn btn-outline-secondary";
                        kapatBtn.ToolTip = "Kapat, bir daha gösterme";
                        kapatBtn.Click += delegate
                        {
                            izinTalep = izinTalep.Select<IzinTalep>(izinTalepId);
                            izinTalep.Aktif = false;
                            izinTalep.Degistiren = CurrentUserName;
                            bool isclosed = izinTalep.Update();
                            if (isclosed)
                                MessageHelper.PublishMessage("İzin talebi kapatıldı", ProjeConstants.MESAJ_BASARILI, 2000);
                            else
                                MessageHelper.PublishMessage("İzin talebi kapatılamadı ", ProjeConstants.MESAJ_HATA);
                            Timer1_Tick(null, null);
                        };
                        KapatCell.Controls.Add(kapatBtn);

                    }
                    row.Controls.Add(KapatCell);
                    IzinTalepTable.Controls.Add(row);
                }
            }
        }
        private string KalacakIzinHesapla(int izinDonemId, string sure)
        {
            string sonuctaKalanIzin = string.Empty;
            IzinDonem izinDonemi = new IzinDonem();
            izinDonemi = izinDonemi.Select<IzinDonem>(izinDonemId);
            if (izinDonemi != null)
            {
                if (izinDonemi.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT)
                {

                    int oncekiToplamIzin = izinDonemi.KullanilanIzin.ConvertToInt();
                    int sonuc = oncekiToplamIzin + sure.ConvertToInt();
                    int izinHakki = izinDonemi.IzinHakki.ConvertToInt();
                    int kalanIzin = izinHakki - sonuc;
                    sonuctaKalanIzin = kalanIzin.ToString() + " " + izinDonemi.Birim;
                }
                if (izinDonemi.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                {
                    TimeSpan oncekiToplamIzin = izinDonemi.KullanilanIzin.ConvertToTimeSpan();
                    TimeSpan sonuc = oncekiToplamIzin + sure.ConvertToTimeSpan();
                    TimeSpan kalanIzin = new TimeSpan(0, 0, 0); ;
                    TimeSpan izinHakki = izinDonemi.IzinHakki.ConvertToTimeSpan();
                    kalanIzin = izinHakki - sonuc;
                    sonuctaKalanIzin = kalanIzin.ConvertToTimeSpanReturnInHHmm();
                }
            }
            return sonuctaKalanIzin;
        }
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            Personel personel = new Personel();
            int personelId = string.IsNullOrEmpty(PersonelIdQS) ? 0 : PersonelIdQS.ConvertToInt();
            personel = personel.Select<Personel>(personelId);
            if (personel != null)
            {
                FillIzinTalepleriTable(personel);
                FillIzinHareketleriTable(personel);
                FillUcretliIzinDonemleriTable(personel);
                FillMazeretIzinHareketleriTable(personel);
                FillMazeretIzinDonemleriTable(personel);
            }
            else
            {

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
        protected void PersonelListesiBtn_Click(object sender, EventArgs e)
        {
            if (SenderAppQS.Equals("EPL"))
            {
                RedirectToPage(ProjeConstants.PAGE_ESKIPERSONEL_LIST + "?SecilenId=" + PersonelIdQS);
            }
            else
                RedirectToPage(ProjeConstants.PAGE_PERSONEL_LIST + "?SecilenId=" + PersonelIdQS);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        #region GorevOnayListesi
        private void TabloOlustur(Personel personel)
        {
            var jsonData = TabloJson(personel); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloJson(Personel personel)
        {
            string jSon = string.Empty;

            try
            {
                List<GorevOnayListItem> list = GetDataList(personel);
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
            string tableString = @"
             jQuery(document).ready(function () {
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                jQuery('#CustomDataTable').DataTable({
                    'initComplete': function (settings, json) {//tablo yüklendiğinde
                        var api = this.api();
                        var row = api.row(function (idx, data, node) { //secilen kayıta gider
                            return data['Secildi'] == true;
                        });
                        if (row.length > 0) {
                            row.select()
                                .show()
                                .draw(false);
                        }
                    },
                    data: " + jsonData + @",
                    pageLength: 5,
                    columns: [
                        { data: 'AdiSoyadi' },
                        { data: 'GorevinSebebi' },
                        { data: 'BaslangicTarihi' },
                        { data: 'BitisTarihi' },
                        { data: 'GorevinYeri' },
                        { data: 'Sure' },
                        { data: 'Yevmiye' },

                    ],
                    columnDefs: [
                        { type: 'turkish', targets: [0, 1, 4] },
                        { width: 300, targets: 1 }
                    ],
                    'order': [[3, 'desc']],//sort date desc
                    'language': {
                        'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    dom: 'frtip',               
                });
            });
            ";
            return tableString;
        }
        private List<GorevOnayListItem> GetDataList(Personel personel)
        {

            if (personel == null)
            {
                MessageHelper.PublishMessage("Personel bulunamadı", ProjeConstants.MESAJ_HATA);
                return new List<GorevOnayListItem>();
            }
            else
            {
                string gorevOnayIdStr = string.Empty;
                GorevOnay gorevOnay = new GorevOnay();
                DataTable dataTable = gorevOnay.SelectAllReturnDT(personel.Id);// PersonelIdQS.ConvertToInt());

                List<GorevOnayListItem> list = new List<GorevOnayListItem>();

                DataView dataView = new DataView(dataTable);
                foreach (DataRowView row in dataView)
                {
                    string gorevOnayId = row["GorevOnayId"].ToString();
                    string secildi = row["Secildi"].ReturnEmptyIfNull().ToString().ToUpper().Equals("TRUE") ? "checked" : string.Empty;
                    string adiSoyadi = row["AdiSoyadi"].ToString();
                    string gorevinSebebi = row["GorevinSebebi"].ToString();
                    string baslangicTarihi = row["BaslangicTarihi"].ReturnEmptyIfNull().ConvertToDatetime().ToString("dd.MM.yyyy HH:mm");
                    string bitisTarihi = row["BitisTarihi"].ReturnEmptyIfNull().ConvertToDatetime().ToString("dd.MM.yyyy HH:mm");
                    string gorevinYeri = row["GorevinYeri"].ToString();
                    string yevmiye = row["Yevmiye"].ReturnZeroIfNull().ConvertToDecimal().ToString("N", culturInfo);
                    string paraBirimi = row["ParaBirimi"].ReturnEmptyIfNull().ToString();
                    string sure = row["Sure"].ReturnEmptyIfNull().ToString();

                    GorevOnayListItem gorevOnayListItem = new GorevOnayListItem();
                    gorevOnayListItem.GorevOnayId = gorevOnayId;
                    gorevOnayListItem.AdiSoyadi = adiSoyadi;
                    gorevOnayListItem.GorevinSebebi = gorevinSebebi;
                    gorevOnayListItem.BaslangicTarihi = baslangicTarihi;
                    gorevOnayListItem.BitisTarihi = bitisTarihi;
                    gorevOnayListItem.GorevinYeri = gorevinYeri;
                    gorevOnayListItem.Yevmiye = yevmiye + " " +paraBirimi;
                    gorevOnayListItem.Sure = sure;

                    gorevOnayListItem.BaslangicTarihiHidden = row["BaslangicTarihi"].ConvertToDatetime();

                    list.Add(gorevOnayListItem);
                }
                return list;

            }
        }
        private class GorevOnayListItem
        {
            public DateTime BaslangicTarihiHidden { get; set; }
            public string GorevOnayId { get; set; }
            public string AdiSoyadi { get; set; }
            public string GorevinYeri { get; set; }
            public string BaslangicTarihi { get; set; }
            public string BitisTarihi { get; set; }
            public string GorevinSebebi { get; set; }
            public string Yevmiye { get; set; }
            public string Sure { get; set; }

        }
        #endregion
    }
}
