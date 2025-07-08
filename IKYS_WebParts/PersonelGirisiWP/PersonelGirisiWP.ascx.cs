using Microsoft.SharePoint;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using static Model.IKYS.Personel;

namespace IKYS_WebParts.PersonelGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class PersonelGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public PersonelGirisiWP()
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
        private string ActiveTabQS
        {
            get
            {

                if (ViewState["ActiveTab"] == null)
                {
                    if (Page.Request.QueryString["ActiveTab"] != null)
                    {
                        ViewState["ActiveTab"] = Page.Request.QueryString["ActiveTab"];
                    }
                    else
                    {
                        ViewState["ActiveTab"] = string.Empty;
                    }
                }
                return ViewState["ActiveTab"].ToString();
            }

            set
            {
                ViewState["ActiveTab"] = value;
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
        private string DestinationAppQS
        {
            get
            {

                if (ViewState["DestinationApp"] == null)
                {
                    if (Page.Request.QueryString["DestinationApp"] != null)
                    {
                        ViewState["DestinationApp"] = Page.Request.QueryString["DestinationApp"];
                    }
                    else
                    {
                        ViewState["DestinationApp"] = string.Empty;
                    }
                }
                return ViewState["DestinationApp"].ToString();
            }

            set
            {
                ViewState["DestinationApp"] = value;
            }
        }
        private string IkametIliQS
        {
            get
            {

                if (ViewState["IkametIli"] == null)
                {
                    if (Page.Request.QueryString["IkametIli"] != null)
                    {
                        ViewState["IkametIli"] = Page.Request.QueryString["IkametIli"];
                    }
                    else
                    {
                        ViewState["IkametIli"] = string.Empty;
                    }
                }
                return ViewState["IkametIli"].ToString();
            }

            set
            {
                ViewState["IkametIli"] = value;
            }
        }
        private string DogumIliQS
        {
            get
            {

                if (ViewState["DogumIli"] == null)
                {
                    if (Page.Request.QueryString["DogumIli"] != null)
                    {
                        ViewState["DogumIli"] = Page.Request.QueryString["DogumIli"];
                    }
                    else
                    {
                        ViewState["DogumIli"] = string.Empty;
                    }
                }
                return ViewState["DogumIli"].ToString();
            }

            set
            {
                ViewState["DogumIli"] = value;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    YonergeLnk.HRef = UtilityHelper.YonergeURLGetir(ProjeConstants.PARAM_IKYSYONERGE, ProjeConstants.IKYSBELGELERI_LIB, ProjeConstants.PAGE_PERSONEL_EDIT);
                    FillData2DDLs();
                    if (String.IsNullOrEmpty(DestinationAppQS) || String.Equals(DestinationAppQS, ""))
                    {
                        //Pers Girisi açılacak PG
                        OpenPersonelGirisi();
                    }
                    else if (String.Equals(DestinationAppQS, "PerD"))
                    {
                        //Duzenle açılacak
                        OpenPersonelDuzenle();
                    }
                    if (!string.IsNullOrEmpty(MesajQS))
                    {
                        MessageHelper.PublishMessage("Personel Kaydedildi, lütfen Kimlik, Eğitim, İletişim, Aile vb bilgileri Tamamlayınız", ProjeConstants.MESAJ_BASARILI, 2000);
                        MesajQS = string.Empty;
                    }
                }
                else // NEDEN??? Çünkü postback dışında olmazsa tablo satırlarına eklenen sil btn çalışmıyor
                {
                    Personel personelDao = new Personel();
                    Personel personel = personelDao.Select<Personel>(PersonelIdQS.ConvertToInt());
                    if (personel != null)
                        FillAileBilgileriTable(personel);
                }
                // Postback veya değil farketmez.
                // (querystring içinde) activeTab varsa o taba odaklan (Kaydet butonlarında ActiveTabQSi set etmek gerekir mi?)
                if (!string.IsNullOrEmpty(ActiveTabQS))
                {
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "setActiveTab('" + ActiveTabQS + "');", true);
                    //System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "setActiveTab('" + ActiveTabQS + "');", true);
                    SetActiveTab(ActiveTabQS);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }


        }
        private void OpenPersonelGirisi()
        {
            SaveBtn.Visible = true;
            //UpdateBtn.Visible = false;
            PersonelListesiBtn.Visible = false;
            //tabs
            KimlikNav.Visible = true;
            IsBilgileriNav.Visible = false;
            KadrosuzIsBilgileriNav.Visible = false;
            IletisimNav.Visible = false;
            AileNav.Visible = false;
            EgitimNav.Visible = false;
            IzinNav.Visible = false;

            UpdateKimlikBtn.Visible = false;

        }
        private void OpenPersonelDuzenle()
        {
            SaveBtn.Visible = false;
            PersonelListesiBtn.Visible = true;
            PersonelIdLbl.Visible = true;
            PersonelIdLbl.Text = PersonelIdQS;

            Personel personelDao = new Personel();
            Personel personel = personelDao.Select<Personel>(PersonelIdQS.ConvertToInt());
            IsBilgileri ib = new IsBilgileri();
            ib = ib.SelectByPersonelId(personel.Id);
            if (ib.CalismaDurumu == ProjeConstants.PER_CALISIYOR_INT)
            {
                TitleLbl.CssClass = "col-form-label btn-outline-primary mb-1";
                TitleLbl.Text = personel.Adi + " " + personel.Soyadi;
            }
            else
            {
                TitleLbl.CssClass = "col-form-label btn-outline-secondary mb-1";
                TitleLbl.Text = personel.Adi + " " + personel.Soyadi + " ( A Y R I L D I ) ";
            }
            FillPersonel2Form(personel);
            PersonelTipiAyarlari(true);

        }
        private void FillPersonel2Form(Personel personel)
        {
            FillIkametIlData();
            FillDogumIlData();

            if (personel != null)
            {
                FillPersonelBilgileri(personel);
                FillKimlikBilgileri(personel);
                
                FillIletisimBilgileri(personel);
                FillAileBilgileriTable(personel);
                FillOkulBilgileri(personel);
                FillIsTecrubesi(personel);
                FillKursBilgileri(personel);

                IsBilgileri ib = new IsBilgileri();
                ib = ib.SelectByPersonelId(personel.Id);
                if (ib!=null)//SB 27.10.2021 Yeşim hanım işten ayrılan personelin de izin bilgilerini görmek istedi //(ib.CalismaDurumu == ProjeConstants.PER_CALISIYOR_INT) 
                {
                    FillIsBilgileri(personel,ib);
                    FillKadrosuzIsBilgileri(personel,ib);

                    FillUcretliIzinDonemleriTable(personel);
                    FillIzinHareketleriTable(personel);
                    FillMazeretIzinHareketleriTable(personel);
                    FillIzinTalepleri(personel);
                    FillDigerIzinlerTable(personel);
                }
                DereceKademeBilgileriniDoldur(personel);
            }


        }

        private void DereceKademeBilgileriniDoldur(Personel personel)
        {
            DereceKademeDegisim dereceKademeDegisim = new DereceKademeDegisim();
            dereceKademeDegisim=dereceKademeDegisim.SelectByPersonelId(personel.Id);
            if (dereceKademeDegisim != null)
            {
                DereceTxt.Text = dereceKademeDegisim.Derece.ToString();
                KademeTxt.Text = dereceKademeDegisim.Kademe.ToString();
                DereceKademeIlerlemeTarihiTxt.Value = dereceKademeDegisim.DegisimTarihi.ConvertToDatetimeEmptyIfNull();
            }
        }

        private void FillIsBilgileri(Personel personel, IsBilgileri isb)
        {
            //IsBilgileri isb = new IsBilgileri();
            //isb = isb.SelectByPersonelId(personel.Id);
            //ünvan
            if (UnvanTanimDDL.Items.FindByValue(isb.UnvanId.ReturnZeroIfNull().ToString()) != null)
                UnvanTanimDDL.SelectedValue = UnvanTanimDDL.Items.FindByValue(isb.UnvanId.ReturnZeroIfNull().ToString()).Value;
            //görev
            if (GorevTanimDDL.Items.FindByValue(isb.GorevId.ReturnZeroIfNull().ToString()) != null)
                GorevTanimDDL.SelectedValue = GorevTanimDDL.Items.FindByValue(isb.GorevId.ReturnZeroIfNull().ToString()).Value;
            //birim
            FillBirimTxt();
            //Calisma Durumu
            if (CalismaDurumuDDL.Items.FindByValue(isb.CalismaDurumu.ToString()) != null)
                CalismaDurumuDDL.SelectedValue = CalismaDurumuDDL.Items.FindByValue(isb.CalismaDurumu.ToString()).Value;
            if (CalismaDurumuDDL.SelectedValue.Equals(ProjeConstants.PER_CALISIYOR_INT.ToString()))
            {
                AyrilmaTarDiv.Visible = false;
                AyrilmaSebebiDiv.Visible = false;
            }
            else
            {
                //Ayrılma Tar
                AyrilmaTarDiv.Visible = true;
                AyrilmaTarTxt.Value = isb.AyrilmaTar.ConvertToDatetimeEmptyIfNull();
                //Ayrılma Sebebi
                AyrilmaSebebiDiv.Visible = true;
                UtilityHelper.SetDDLValue(AyrilmaSebebiDDL, isb.AyrilmaSebebi.ToString());

            }
            //ise Bas Tar
            IsbasTarTxt.Value = isb.BaslamaTar.ConvertToDatetimeEmptyIfNull();
            //izin dönemi başlangıç tar
            IzinDonemiBasTarTxt.Value = isb.IzinDonemiBasTar.ConvertToDatetimeEmptyIfNull();
            //protokol sira
            ProtokolSirasiTxt.Text = isb.ProtokolSiraNo.ReturnEmptyIfNull().ToString();
            SGKSicilNoTxt.Text = isb.SGKSicilNo.ReturnEmptyIfNull().ToString();

            SGKBasTarTxt.Value = personel.Asker_sivil == ProjeConstants.PER_ASKER_INT ? "" : isb.SGKBasTar.ConvertToDatetimeEmptyIfNull();
            SGKBasTarTxt.Disabled = personel.Asker_sivil == ProjeConstants.PER_ASKER_INT;
            EmeklilikTarTxt.Value = personel.Asker_sivil == ProjeConstants.PER_ASKER_INT ? "" : isb.EmeklilikTarihi.ConvertToDatetimeEmptyIfNull();
            EmeklilikTarTxt.Disabled = personel.Asker_sivil == ProjeConstants.PER_ASKER_INT;
            VakifOncesiPrimGunSayisiTxt.Text = isb.VakifOncesiPrimGunSayisi.ReturnEmptyIfNull().ToString();
            //VakifOncesiPrimGunSayisiTxt.Enabled = !(personel.Asker_sivil == ProjeConstants.PER_ASKER_INT);

        }
        private void FillKadrosuzIsBilgileri(Personel personel, IsBilgileri isb)
        {
            //birim
            UtilityHelper.SetDDLValue(BirimDDL,isb.BirimId.ToString());
            //Calisma Durumu
            UtilityHelper.SetDDLValue(KadrosuzCalismaDurumuDDL, isb.CalismaDurumu.ToString());
            if (KadrosuzCalismaDurumuDDL.SelectedValue.Equals(ProjeConstants.PER_CALISIYOR_INT.ToString()))
            {
                KadrosuzAyrilmaTarihiDiv.Visible = false;
                KadrosuzAyrilmaSebebiDiv.Visible = false;
            }
            else
            {
                //Ayrılma Tar
                KadrosuzAyrilmaTarihiDiv.Visible = true;
                KadrosuzAyrilmaTarihiTxt.Value = isb.AyrilmaTar.ConvertToDatetimeEmptyIfNull();
                //Ayrılma Sebebi
                KadrosuzAyrilmaSebebiDiv.Visible = true;
                UtilityHelper.SetDDLValue(KadrosuzAyrilmaSebebiDDL, isb.AyrilmaSebebi.ReturnEmptyIfNull().ToString());

            }
            //ise Bas Tar
            KadrosuzIsbasTarihiTxt.Value = isb.BaslamaTar.ConvertToDatetimeEmptyIfNull();
            
        }
        private void FillBirimTxt()
        {
            string gtId = GorevTanimDDL.SelectedItem.Value;
            GorevTanim gtDao = new GorevTanim();
            GorevTanim gt = gtDao.Select<GorevTanim>(gtId.ConvertToInt());
            if (gt != null)
            {
                int birimId = gt.BirimId;
                BirimTanim btDao = new BirimTanim();
                BirimTanim bt = btDao.Select<BirimTanim>(birimId);
                BirimAdiTxt.Text = bt == null ? "" : bt.Adi.ReturnEmptyIfNull().ToString();
                BirimIdTxt.Text = bt == null ? "0" : bt.Id.ToString();
            }
        }
        private void BirimDDLDoldur()
        {
            BirimDDL.Items.Clear();
            BirimTanim birimDao = new BirimTanim();
            List<BirimTanim> list = birimDao.SelectAll<BirimTanim>();
            ListItem bosLi = new ListItem("", "0");
            BirimDDL.Items.Add(bosLi);
            foreach (BirimTanim gr in list)
            {
                ListItem li = new ListItem(gr.Adi.ReturnEmptyIfNull().ToString(), gr.Id.ReturnZeroIfNull().ToString());
                BirimDDL.Items.Add(li);
            }
        }
        private void FillKimlikBilgileri(Personel personel)
        {
            Kimlik kimlikDao = new Kimlik();
            Kimlik kimlik = kimlikDao.SelectByPersonelId(personel.Id);
            TCKimlikNoTxt.Text = kimlik.TCKimlikNo;

            //AnneAdi
            AnneAdiTxt.Text = kimlik.AnneAdi.ReturnEmptyIfNull().ToString();
            //Baba Adi
            BabaAdiTxt.Text = kimlik.BabaAdi.ReturnEmptyIfNull().ToString();
            //MedeniHali
            if (MedeniHaliDDL.Items.FindByValue(kimlik.MedeniHali.ReturnEmptyIfNull().ToString()) != null)
                MedeniHaliDDL.SelectedValue = MedeniHaliDDL.Items.FindByValue(kimlik.MedeniHali.ReturnEmptyIfNull().ToString()).Value;
            //Evlenme Tarihi
            EvlilikTarihiTxt.Value = kimlik.EvlilikTar.ConvertToDatetimeEmptyIfNull();

            //Dogum yeri --ilce + il            
            Ilce ilce = new Ilce();
            ilce = ilce.Select<Ilce>(kimlik.DogumYeri.ConvertToInt());
            if (ilce != null)
            {
                Il il = new Il();
                il = il.Select<Il>(ilce.IlId);
                //il
                if (DogumIliDDL.Items.FindByText(il.IlAdi.ReturnEmptyIfNull().ToString()) != null)
                    DogumIliDDL.SelectedValue = DogumIliDDL.Items.FindByText(il.IlAdi.ReturnEmptyIfNull().ToString()).Value;
                FillDogumIlceDDL();
                //ilce
                if (DogumIlceDDL.Items.FindByText(ilce.IlceAdi.ReturnEmptyIfNull().ToString()) != null)
                    DogumIlceDDL.SelectedValue = DogumIlceDDL.Items.FindByText(ilce.IlceAdi.ReturnEmptyIfNull().ToString()).Value;
            }
            //Dogum Tarihi
            DogumTarihiTxt.Value = kimlik.DogumTar.ConvertToDatetimeEmptyIfNull();
            DogumGunuKutlamaChk.Checked = kimlik.DogumGunuKutlama;
            EvlilikKutlamaChk.Checked = kimlik.EvlilikKutlama;

            //cinsiyet
            if (CinsiyetDDL.Items.FindByText(kimlik.Cinsiyet.ReturnEmptyIfNull().ToString()) != null)
                CinsiyetDDL.SelectedValue = CinsiyetDDL.Items.FindByText(kimlik.Cinsiyet.ReturnEmptyIfNull().ToString()).Value;
            //Kan grubu
            if (KanGrubuDDL.Items.FindByValue(kimlik.KanGrubu.ReturnEmptyIfNull().ToString()) != null)
                KanGrubuDDL.SelectedValue = KanGrubuDDL.Items.FindByValue(kimlik.KanGrubu.ReturnEmptyIfNull().ToString()).Value;
        }
        private void FillPersonelBilgileri(Personel personel)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
            string fotostr = string.IsNullOrEmpty(personel.KullaniciAdi) ? personel.Adi.ReturnEmptyIfNull().ToString().Substring(0, 1) + personel.Soyadi.ReturnEmptyIfNull().ToString() : personel.KullaniciAdi.ReturnEmptyIfNull().ToString();
            string imgUrl = newUrl + "/../" + ProjeConstants.RESIMLER_PERSONEL + "/" + fotostr.ReplaceTrChars() + ".jpg";

            DisplayImage.ImageUrl = imgUrl;

            PersonelIdLbl.Text = "" + personel.Id;
            AdiTxt.Text = personel.Adi.ReturnEmptyIfNull().ToString();
            SoyadiTxt.Text = personel.Soyadi.ReturnEmptyIfNull().ToString();
            SicilNoTxt.Text = personel.SicilNo.ReturnEmptyIfNull().ToString();
            KullaniciAdiTxt.Text = personel.KullaniciAdi.ReturnEmptyIfNull().ToString();
            if (AskerSivilDDL.Items.FindByValue(personel.Asker_sivil.ReturnZeroIfNull().ToString()) != null)
                AskerSivilDDL.SelectedValue = AskerSivilDDL.Items.FindByValue(personel.Asker_sivil.ReturnZeroIfNull().ToString()).Value;
            UtilityHelper.SetDDLValue(TahsiliDDL, personel.Tahsili.ToString());
            UtilityHelper.SetDDLValue(PersonelTipiDDL, personel.Tipi.ToString());
        }
        private void FillIletisimBilgileri(Personel personel)
        {
            IletisimBilgileri iletisimBilgileriDao = new IletisimBilgileri();
            IletisimBilgileri iletsimBilgileri = iletisimBilgileriDao.SelectByPersonelId(personel.Id);
            AdresTxt.Text = iletsimBilgileri.Adres.ReturnEmptyIfNull().ToString();
            SemtTxt.Text = iletsimBilgileri.Semt.ReturnEmptyIfNull().ToString();
            //ikamet yeri --ilce + il            
            Ilce ilce = new Ilce();
            ilce = ilce.Select<Ilce>(iletsimBilgileri.Ilcesi.ConvertToInt());
            if (ilce != null)
            {
                Il il = new Il();
                il = il.Select<Il>(ilce.IlId.ConvertToInt());
                //il
                if (IkametIliDDL.Items.FindByText(il.IlAdi.ReturnEmptyIfNull().ToString()) != null)
                    IkametIliDDL.SelectedValue = IkametIliDDL.Items.FindByText(il.IlAdi.ReturnEmptyIfNull().ToString()).Value;
                FillIkametIlceDDL();
                //ilce
                if (IkametIlcesiDDL.Items.FindByText(ilce.IlceAdi.ReturnEmptyIfNull().ToString()) != null)
                    IkametIlcesiDDL.SelectedValue = IkametIlcesiDDL.Items.FindByText(ilce.IlceAdi.ReturnEmptyIfNull().ToString()).Value;
            }
            PostaKoduTxt.Text = iletsimBilgileri.PostaKodu;
            DahiliTelefonuTxt.Text = iletsimBilgileri.DahiliTelefonu;
            EvTelefonuTxt.Text = iletsimBilgileri.EvTelefonu;
            CepTelefonuTxt.Text = iletsimBilgileri.CepTelefonu;
            CepTelefonu2Txt.Text = iletsimBilgileri.CepTelefonu2;
            IntranetEPostaTxt.Text = iletsimBilgileri.IntranetEPosta;
            InternetEPostaTxt.Text = iletsimBilgileri.InternetEPosta;
            OzelEPostaTxt.Text = iletsimBilgileri.OzelEPosta;
            PlakaTxt.Text = iletsimBilgileri.Plaka;
        }
        private void FillAileBilgileriTable(Personel personel)
        {
            Aile aileDao = new Aile();
            List<Aile> list = aileDao.SelectByPersonelId(personel.Id);
            int SiraNo = 1;
            AileTableHeaders();
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

                int yakinlikDerecesi = aile.YakinlikDerecesi.ConvertToInt();
                if (yakinlikDerecesi == ProjeConstants.PER_YAKINLIKDERECESI_ES_INT)
                    YakinlikCell.Text = ProjeConstants.PER_YAKINLIKDERECESI_ES;
                else if (yakinlikDerecesi == ProjeConstants.PER_YAKINLIKDERECESI_COCUK_INT)
                    YakinlikCell.Text = ProjeConstants.PER_YAKINLIKDERECESI_COCUK;
                else YakinlikCell.Text = string.Empty;
                row.Controls.Add(YakinlikCell);

                TableCell DogumTarCell = new TableCell();
                DogumTarCell.Text = aile.DogumTar.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(DogumTarCell);

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
        private void FillOkulBilgileri(Personel personel)
        {
            Egitim egitim = new Egitim();
            List<Egitim> egitimList = egitim.SelectByPersonelId(personel.Id);
            OkulTableHeaders();
            int SiraNo = 1;
            foreach (Egitim item in egitimList)
            {
                TableRow row = new TableRow();

                TableCell SiraCell = new TableCell();
                SiraCell.Text = SiraNo++ + "";
                row.Controls.Add(SiraCell);

                TableCell OkulCell = new TableCell();
                OkulCell.Text = item.Okul;
                row.Controls.Add(OkulCell);

                TableCell SeviyeCell = new TableCell();
                int seviyeId = item.Seviye.ConvertToInt();
                if (seviyeId > 0)
                {
                    EgitimSeviyesi es = new EgitimSeviyesi();
                    es = es.Select<EgitimSeviyesi>(seviyeId);
                    SeviyeCell.Text = es != null ? es.Adi : "";
                }

                row.Controls.Add(SeviyeCell);

                TableCell MezuniyetTarCell = new TableCell();
                MezuniyetTarCell.Text = item.MezuniyetTar.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(MezuniyetTarCell);

                TableCell AciklamaCell = new TableCell();
                AciklamaCell.Text = item.Aciklama;
                row.Controls.Add(AciklamaCell);
                OkulTable.Controls.Add(row);
            }
        }
        private void FillKursBilgileri(Personel personel)
        {
            Kurs kurs = new Kurs();
            List<Kurs> kursList = kurs.SelectByPersonelId(personel.Id);
            KursTableHeaders();
            int SiraNo = 1;
            foreach (Kurs item in kursList)
            {
                TableRow row = new TableRow();

                TableCell SiraCell = new TableCell();
                SiraCell.Text = SiraNo++ + "";
                row.Controls.Add(SiraCell);

                TableCell KursCell = new TableCell();
                KursCell.Text = item.KursAdi;
                row.Controls.Add(KursCell);

                TableCell SureCell = new TableCell();
                SureCell.Text = item.Sure;
                row.Controls.Add(SureCell);

                TableCell TarihCell = new TableCell();
                TarihCell.Text = item.Tarih.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(TarihCell);

                TableCell VerenKurumCell = new TableCell();
                VerenKurumCell.Text = item.VerenKurum;
                row.Controls.Add(VerenKurumCell);

                KursTable.Controls.Add(row);
            }
        }
        private void FillIsTecrubesi(Personel personel)
        {
            IsTecrube isTecrubesi = new IsTecrube();
            List<IsTecrube> kursList = isTecrubesi.SelectByPersonelId(personel.Id);
            IsyeriTableHeaders();
            int SiraNo = 1;
            foreach (IsTecrube item in kursList)
            {
                TableRow row = new TableRow();

                TableCell SiraCell = new TableCell();
                SiraCell.Text = SiraNo++ + "";
                row.Controls.Add(SiraCell);

                TableCell IsyeriCell = new TableCell();
                IsyeriCell.Text = item.Isyeri;
                row.Controls.Add(IsyeriCell);

                TableCell GorevCell = new TableCell();
                GorevCell.Text = item.Gorevi;
                row.Controls.Add(GorevCell);

                TableCell BasTarCell = new TableCell();
                BasTarCell.Text = item.BasTar.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(BasTarCell);

                TableCell BitTarCell = new TableCell();
                BitTarCell.Text = item.BitTar.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(BitTarCell);

                IsyeriTable.Controls.Add(row);
            }
        }

        private void FillUcretliIzinDonemleriTable(Personel personel)
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
                List<IzinDonem> izinDonemiList = izinDonemDao.SelectByPersonelId(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT);

                IzinHareket izinHareket = new IzinHareket();

                UcretliIzinDonemleriTableHeaders();
                foreach (IzinDonem izinDonemi in izinDonemiList)
                {
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
            else
            {
                MessageHelper.PublishMessage("İşe başlama tarihi belirlenemedi", ProjeConstants.MESAJ_HATA);
            }
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
                    //DateTime izinDonemiBasi = izinDonem != null ? izinDonem.BaslangicTarihi : today.AddYears(-1);
                    DateTime izinDonemiSonu = izinDonem != null ? izinDonem.BitisTarihi : today.AddMonths(1); ;
                    //DataTable dataTable = izinHareket.SelectByIzinDonemiReturnDataTable(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, izinDonemiBasi, threeMonthsLater);//3 ay içinde yeni izin dönemi başlıyor olabilir
                    DataTable dataTable = izinHareket.SelectByIzinDonemiReturnDataTable(izinDonem.Id, personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT);
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
                        //bool printed = false;
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
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
                            UcretliIzinHareketTable.Controls.Add(row);
                        }
                    }
                }

            }
            else
            {
                MessageHelper.PublishMessage("İşe başlama tarihi belirlenemedi", ProjeConstants.MESAJ_HATA);
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
                if (izinDonem == null)
                {
                    izinDonem = new IzinDonem();
                    izinDonem.IzinDonemiOlustur(personel, ProjeConstants.IZINTIPI_MAZERET_INT, today, CurrentUserName);
                }
                if (izinDonem != null)
                {
                    //DateTime izinDonemiBasi = izinDonem != null ? izinDonem.BaslangicTarihi : today.AddYears(-1);
                    DateTime izinDonemiSonu = izinDonem != null ? izinDonem.BitisTarihi : today.AddMonths(1); ;
                    DataTable dataTable = izinHareket.SelectByIzinDonemiReturnDataTable(izinDonem.Id, personel.Id, ProjeConstants.IZINTIPI_MAZERET_INT);
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
                        //bool printed = false;
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            DateTime bastar = dataRow["BaslangicTarihi"].ConvertToDatetime();
                            DateTime bittar = dataRow["BitisTarihi"].ConvertToDatetime();
                            string bitSaat = bittar.ToString("HH:mm");
                            string basSaat = bastar.ToString("HH:mm");
                            string sure = dataRow["Sure"].ToString();
                            string birim = dataRow["Birim"].ToString();

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
                            SureCell.Text = sure.ConvertToTimeSpanReturnInHHmm();
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
                // MessageHelper.PublishMessage("İşe başlama tarihi belirlenemedi", ProjeConstants.MESAJ_HATA);
            }
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
                SureCell.Text = item.Sure + " " + item.Birim + (item.IzinTipi==ProjeConstants.IZINTIPI_SUTIZNI_INT?"(Günde 1 Saat 30 Dk.)":"");
                row.Controls.Add(SureCell);
                DigerIzinlerTable.Controls.Add(row);
            }

            if (list.Count < 1)
            {
                UcretliIzinHareketTable.Rows.Clear();
                TableRow tr = new TableRow();
                TableCell tc = new TableCell();
                tc.Text = "Henüz izin kullanılmamış.";
                tr.Controls.Add(tc);
                UcretliIzinHareketTable.Controls.Add(tr);
            }
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
            //th.Controls.Add(yazdirCell);
            UcretliIzinHareketTable.Controls.Add(th);
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
        private void FillIzinTalepleri(Personel personel)
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
                    string zamanBirimi = dataRow["Birim"].ToString();
                    string sure = dataRow["Sure"].ToString();
                    string izinTipi = dataRow["IzinTipi"].ReturnEmptyIfNull().ToString();
                    int izinTipiId = dataRow["IzinTipiId"].ReturnEmptyIfNull().ConvertToInt();
                    string bastar = dataRow["BaslangicTarihi"].ConvertToDatetimeEmptyIfNull();
                    string bittar = dataRow["BitisTarihi"].ConvertToDatetimeEmptyIfNull();
                    if (izinTipiId == ProjeConstants.IZINTIPI_MAZERET_INT)
                    {
                        string sureHHmm = sure.ConvertToTimeSpanReturnInHHmm();
                        sure = sureHHmm;
                    }
                    else
                    {
                        sure = sure + " " + zamanBirimi;
                    }
                    TableRow row = new TableRow();

                    TableCell SiraNoCell = new TableCell();

                    SiraNoCell.Text = SiraNo++ + "";
                    row.Controls.Add(SiraNoCell);

                    TableCell IzinTipiCell = new TableCell();
                    IzinTipiCell.Text = izinTipi;
                    row.Controls.Add(IzinTipiCell);

                    TableCell BasTarCell = new TableCell();
                    BasTarCell.Text = bastar;
                    row.Controls.Add(BasTarCell);

                    TableCell BitTarCell = new TableCell();
                    BitTarCell.Text = bittar;
                    row.Controls.Add(BitTarCell);

                    TableCell SureCell = new TableCell();

                    SureCell.Text = sure;
                    row.Controls.Add(SureCell);

                    TableCell OnayDurumuCell = new TableCell();
                    int onayDurumuId = dataRow["OnayDurumuId"].ConvertToInt();
                    string onayDurumu = dataRow["OnayDurumu"].ReturnZeroIfNull().ToString();
                    OnayDurumuCell.Text = onayDurumu;
                    row.Controls.Add(OnayDurumuCell);

                    IzinTalepTable.Controls.Add(row);
                }
            }

        }
        private void AileTableHeaders()
        {
            //Column headers
            AileTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell AdiSoyadiCell = new TableHeaderCell();
            AdiSoyadiCell.Text = "Adı Soyadı";
            TableHeaderCell YakinlikDerCell = new TableHeaderCell();
            YakinlikDerCell.Text = "Yak.Derecesi";
            TableHeaderCell DogumTarCell = new TableHeaderCell();
            DogumTarCell.Text = "DoğumTarihi";
            TableHeaderCell OkulCell = new TableHeaderCell();
            OkulCell.Text = "Okul";
            TableHeaderCell MeslekCell = new TableHeaderCell();
            MeslekCell.Text = "Meslek";
            TableHeaderCell TelefonCell = new TableHeaderCell();
            TelefonCell.Text = "Telefon";

            TableHeaderCell SilCell = new TableHeaderCell();
            SilCell.Text = "Sil";

            th.Controls.Add(siraCell);
            th.Controls.Add(AdiSoyadiCell);
            th.Controls.Add(YakinlikDerCell);
            th.Controls.Add(DogumTarCell);
            th.Controls.Add(OkulCell);
            th.Controls.Add(MeslekCell);
            th.Controls.Add(TelefonCell);
            th.Controls.Add(SilCell);

            AileTable.Controls.Add(th);
        }
        private void OkulTableHeaders()
        {
            OkulTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell OkulCell = new TableHeaderCell();
            OkulCell.Text = "Okul";
            TableHeaderCell SeviyeCell = new TableHeaderCell();
            SeviyeCell.Text = "Seviye";
            TableHeaderCell MezuniyetTarCell = new TableHeaderCell();
            MezuniyetTarCell.Text = "Mezuniyet Tarihi";
            TableHeaderCell AciklamaCell = new TableHeaderCell();
            AciklamaCell.Text = "Açıklama";

            th.Controls.Add(siraCell);
            th.Controls.Add(OkulCell);
            th.Controls.Add(SeviyeCell);
            th.Controls.Add(MezuniyetTarCell);
            th.Controls.Add(AciklamaCell);


            OkulTable.Controls.Add(th);
        }
        private void KursTableHeaders()
        {
            KursTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell KursCell = new TableHeaderCell();
            KursCell.Text = "Kurs/Eğt/Sertifika";
            TableHeaderCell SureCell = new TableHeaderCell();
            SureCell.Text = "Kurs Süresi";
            TableHeaderCell TarihCell = new TableHeaderCell();
            TarihCell.Text = "Tarih";
            TableHeaderCell VerenKurumCell = new TableHeaderCell();
            VerenKurumCell.Text = "Veren Kurum";

            th.Controls.Add(siraCell);
            th.Controls.Add(KursCell);
            th.Controls.Add(SureCell);
            th.Controls.Add(TarihCell);
            th.Controls.Add(VerenKurumCell);

            KursTable.Controls.Add(th);
        }
        private void IsyeriTableHeaders()
        {
            IsyeriTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell IsyeriCell = new TableHeaderCell();
            IsyeriCell.Text = "Çalıştığı İşyeri";
            TableHeaderCell GorevCell = new TableHeaderCell();
            GorevCell.Text = "Yaptığı Görev";
            TableHeaderCell BasTarCell = new TableHeaderCell();
            BasTarCell.Text = "Başlama Tarihi";
            TableHeaderCell BitTarCell = new TableHeaderCell();
            BitTarCell.Text = "Ayrilma Tarihi";

            th.Controls.Add(siraCell);
            th.Controls.Add(IsyeriCell);
            th.Controls.Add(GorevCell);
            th.Controls.Add(BasTarCell);
            th.Controls.Add(BitTarCell);

            IsyeriTable.Controls.Add(th);
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
            sureCell.Text = "Süre";

            TableHeaderCell durumCell = new TableHeaderCell();
            durumCell.Text = "Durum";
            th.Controls.Add(siraCell);
            th.Controls.Add(izintipiCell);
            th.Controls.Add(bastarCell);
            th.Controls.Add(bittarCell);
            th.Controls.Add(sureCell);

            th.Controls.Add(durumCell);
            IzinTalepTable.Controls.Add(th);
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
        private void FillData2DDLs()
        {
            BirimDDLDoldur();
            FillGorevTanimDDL();
            FillUnvanTanimDDL();
            FillMedeniHaliDDL();
            FillCinsiyetDDL();
            FillKanGrubuDDL();
            FillCalismaDurumuDDL();
            FilKadrosuzlCalismaDurumuDDL();
            FillAyrilmaSebebiDDL();
            FillKadrosuzAyrilmaSebebiDDL();
            FillAskerSivilDDL();
            FillIlData();
            TahsiliDDLDoldur();
            PersonelTipiDDLDoldur();
        }
        private void FillGorevTanimDDL()
        {
            GorevTanimDDL.Items.Clear();
            GorevTanim gorevDao = new GorevTanim();
            List<GorevTanim> list = gorevDao.SelectAll<GorevTanim>();
            foreach (GorevTanim gr in list)
            {
                ListItem li = new ListItem(gr.Adi.ReturnEmptyIfNull().ToString(), gr.Id.ReturnZeroIfNull().ToString());
                GorevTanimDDL.Items.Add(li);
            }
        }
        private void FillUnvanTanimDDL()
        {
            UnvanTanimDDL.Items.Clear();
            UnvanTanim unvanDao = new UnvanTanim();
            List<UnvanTanim> list = unvanDao.SelectAll<UnvanTanim>();
            foreach (UnvanTanim un in list)
            {
                ListItem li = new ListItem(un.Adi.ReturnEmptyIfNull().ToString(), un.Id.ReturnZeroIfNull().ToString());
                UnvanTanimDDL.Items.Add(li);
            }
        }
        private void FillMedeniHaliDDL()
        {
            MedeniHaliDDL.Items.Clear();
            MedeniHaliDDL.Items.Add("");
            MedeniHaliDDL.Items.Add(new ListItem(ProjeConstants.PER_EVLI, "1"));
            MedeniHaliDDL.Items.Add(new ListItem(ProjeConstants.PER_BEKAR, "2"));
        }
        private void FillCinsiyetDDL()
        {
            CinsiyetDDL.Items.Clear();
            CinsiyetDDL.Items.Add(ProjeConstants.PER_CINSIYET_KADIN);
            CinsiyetDDL.Items.Add(ProjeConstants.PER_CINSIYET_ERKEK);
        }
        private void FillKanGrubuDDL()
        {
            KanGrubuDDL.Items.Clear();
            KanGrubuDDL.Items.Add("");
            KanGrubuDDL.Items.Add("0 Rh+");
            KanGrubuDDL.Items.Add("A Rh+");
            KanGrubuDDL.Items.Add("B Rh+");
            KanGrubuDDL.Items.Add("AB Rh+");
            KanGrubuDDL.Items.Add("0 Rh-");
            KanGrubuDDL.Items.Add("A Rh-");
            KanGrubuDDL.Items.Add("B Rh-");
            KanGrubuDDL.Items.Add("AB Rh-");
        }
        private void FillCalismaDurumuDDL()
        {
            CalismaDurumuDDL.Items.Clear();
            ListItem li1 = new ListItem(ProjeConstants.PER_CALISIYOR, ProjeConstants.PER_CALISIYOR_INT.ToString());
            ListItem li = new ListItem(ProjeConstants.PER_AYRILDI, ProjeConstants.PER_AYRILDI_INT.ToString());
            CalismaDurumuDDL.Items.Add(li);
            CalismaDurumuDDL.Items.Add(li1);            


        }
        private void FilKadrosuzlCalismaDurumuDDL()
        {
            KadrosuzCalismaDurumuDDL.Items.Clear();
            ListItem li1 = new ListItem(ProjeConstants.PER_CALISIYOR, ProjeConstants.PER_CALISIYOR_INT.ToString());
            ListItem li = new ListItem(ProjeConstants.PER_AYRILDI, ProjeConstants.PER_AYRILDI_INT.ToString());
            KadrosuzCalismaDurumuDDL.Items.Add(li);
            KadrosuzCalismaDurumuDDL.Items.Add(li1);

        }
        private void FillAyrilmaSebebiDDL()
        {
            AyrilmaSebebiDDL.Items.Clear();
            AyrilmaSebebiDDL.Items.Add("");
            AyrilmaSebebiDDL.Items.Add("Emeklilik");
            AyrilmaSebebiDDL.Items.Add("İstifa");
            AyrilmaSebebiDDL.Items.Add("Görev Süresi Doldu");
            AyrilmaSebebiDDL.Items.Add("İş Akdi Feshi");
            AyrilmaSebebiDDL.Items.Add("Yaş Haddi");
            AyrilmaSebebiDDL.Items.Add("Şirkete Dönme");

        }        
        private void FillKadrosuzAyrilmaSebebiDDL()
        {
            KadrosuzAyrilmaSebebiDDL.Items.Clear();
            KadrosuzAyrilmaSebebiDDL.Items.Add("");
            KadrosuzAyrilmaSebebiDDL.Items.Add("Emeklilik");
            KadrosuzAyrilmaSebebiDDL.Items.Add("İstifa");
            KadrosuzAyrilmaSebebiDDL.Items.Add("Görev Süresi Doldu");
            KadrosuzAyrilmaSebebiDDL.Items.Add("İş Akdi Feshi");
            KadrosuzAyrilmaSebebiDDL.Items.Add("Yaş Haddi");
            KadrosuzAyrilmaSebebiDDL.Items.Add("Şirkete Dönme");
            KadrosuzAyrilmaSebebiDDL.Items.Add("Sözleşme Bitti");

        }
        private void FillIkametIlData()
        {
            if (IkametIliDDL.SelectedItem == null)
            {
                IkametIliDDL.Items.Clear();
                Il newil = new Il();
                List<Il> list = newil.SelectAll<Il>();
                foreach (Il il in list)
                {
                    IkametIliDDL.Items.Add(new ListItem(il.IlAdi, il.Id.ToString()));
                }
                FillIkametIlceDDL();
            }

        }
        private void FillIkametIlceDDL()
        {
            IkametIlcesiDDL.Items.Clear();
            Ilce pilce = new Ilce();

            List<Ilce> list = pilce.SelectByIlId(IkametIliDDL.SelectedValue.ConvertToInt());
            foreach (Ilce ilce in list)
            {
                IkametIlcesiDDL.Items.Add(new ListItem(ilce.IlceAdi, ilce.Id.ToString()));
            }
        }
        private void FillDogumIlceDDL()
        {
            DogumIlceDDL.Items.Clear();
            Ilce pilce = new Ilce();

            List<Ilce> list = pilce.SelectByIlId(DogumIliDDL.SelectedValue.ConvertToInt());
            foreach (Ilce ilce in list)
            {
                DogumIlceDDL.Items.Add(new ListItem(ilce.IlceAdi, ilce.Id.ToString()));
            }
        }
        private void FillDogumIlData()
        {
            if (DogumIliDDL.SelectedItem == null)
            {
                DogumIliDDL.Items.Clear();
                Il newil = new Il();
                List<Il> list = newil.SelectAll<Il>();
                foreach (Il il in list)
                {
                    DogumIliDDL.Items.Add(new ListItem(il.IlAdi, il.Id.ToString()));
                }
                FillDogumIlceDDL();
            }

        }
        private void FillAskerSivilDDL()
        {
            AskerSivilDDL.Items.Clear();
            ListItem li = new ListItem(ProjeConstants.PER_SIVIL, ProjeConstants.PER_SIVIL_INT.ToString());
            ListItem li1 = new ListItem(ProjeConstants.PER_ASKER, ProjeConstants.PER_ASKER_INT.ToString());
            AskerSivilDDL.Items.Add(li);
            AskerSivilDDL.Items.Add(li1);
        }
        private void TahsiliDDLDoldur()
        {
            TahsiliDDL.Items.Clear();

            TahsilTanim tahsilTanim = new TahsilTanim();
            List<TahsilTanim> list = tahsilTanim.SelectAll<TahsilTanim>();
            foreach (TahsilTanim item in list)
            {
                TahsiliDDL.Items.Add(new ListItem(item.TahsilDurumu, item.Id.ToString()));
            }

        }
        private void FillIlData()
        {
            if (DogumIliDDL.SelectedItem == null)
            {
                DogumIliDDL.Items.Clear();

                Il pIl = new Il();
                List<Il> list = pIl.SelectAll<Il>();
                DogumIliDDL.Items.Add(new ListItem(ProjeConstants.HEPSI, ProjeConstants.IL_HEPSI.ToString()));
                foreach (Il il in list)
                {
                    DogumIliDDL.Items.Add(new ListItem(il.IlAdi, il.Id.ToString()));
                }
            }

        }
        private void PersonelTipiDDLDoldur()
        {
            PersonelTipiDDL.Items.Clear();
            // Enum'u Dropdown için listeye dönüştürme

            var personelTipleri = Enum.GetValues(typeof(PersonelTipi))
                           .Cast<PersonelTipi>()
                           .ToList();

            foreach (var item in personelTipleri)
            {
                if (item == (PersonelTipi.Tumu))
                    continue;
                int tipiInt = (int)item;
                string displayName = UtilityHelper.GetEnumDisplayName(item);
                PersonelTipiDDL.Items.Add(new ListItem(displayName, tipiInt.ToString()));
            }
        }
        protected void UpdateKimlikBtn_Click(object sender, EventArgs e)
        {
            SetActiveTab("KimlikLi");
            bool personelUpdated = false;
            bool kimlikUpdated = false;

            int personelId = PersonelIdQS.ConvertToInt();
            Personel personel = new Personel();
            personel = personel.Select<Personel>(personelId);


            if (personel != null)
            {
                personelUpdated = PersonelUpdate(personel);
                kimlikUpdated = KimlikUpdate(personel);
            }
            if (personelUpdated && kimlikUpdated)
            {
                IzinDonemiBilgileriniGuncelle(personel);
                MessageHelper.PublishMessage("Personel bilgileri güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
            }
            else if (personelUpdated)
            {
                IzinDonemiBilgileriniGuncelle(personel);
                MessageHelper.PublishMessage("Personel kimlik bilgileri eksik olarak güncellendi.", ProjeConstants.MESAJ_BILGI);
            }

            else if (kimlikUpdated)
                MessageHelper.PublishMessage("Personel bilgisi eksik olarak güncellendi.", ProjeConstants.MESAJ_BILGI);
            else
                MessageHelper.PublishMessage("Personel bilgileri güncellenemedi.", ProjeConstants.MESAJ_HATA);

        }
        protected void KadrosuzUpdateIsBilgileriBtn_Click(object sender, EventArgs e)
        {
            SetActiveTab("KadrosuzIsBilgileriLi");
            bool isBilgileriUpdated = false;
            int personelId = PersonelIdQS.ConvertToInt();
            Personel personel = new Personel();
            personel = personel.Select<Personel>(personelId);
            if (personel != null)
            {
                isBilgileriUpdated = KadrosuzIsBilgileriUpdate(personel);
            }


        }protected void UpdateIsBilgileriBtn_Click(object sender, EventArgs e)
        {
            SetActiveTab("IsBilgileriLi");
            bool isBilgileriUpdated = false;
            int personelId = PersonelIdQS.ConvertToInt();
            Personel personel = new Personel();
            personel = personel.Select<Personel>(personelId);
            if (personel != null)
            {
                isBilgileriUpdated = IsBilgileriUpdate(personel);
            }

            if (isBilgileriUpdated)
            {
                IzinDonemiBilgileriniGuncelle(personel);
                MessageHelper.PublishMessage("İş bilgileri güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
            }
            else
                MessageHelper.PublishMessage("İş bilgileri güncellenemedi.", ProjeConstants.MESAJ_HATA);
        }
        protected void UpdateIletisimBtn_Click(object sender, EventArgs e)
        {
            SetActiveTab("IletisimLi");
            bool iletisimUpdated = false;
            int personelId = PersonelIdQS.ConvertToInt();
            Personel personel = new Personel();
            personel = personel.Select<Personel>(personelId);
            if (personel != null)
            {
                iletisimUpdated = IletisimBilgileriUpdate(personel);
            }

            if (iletisimUpdated)
                MessageHelper.PublishMessage("İletişim bilgileri güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
            else
                MessageHelper.PublishMessage("İletişim bilgileri güncellenemedi.", ProjeConstants.MESAJ_HATA);
        }
        protected void AileDuzenleBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_AILE_EDIT + "?SenderApp=PD&PersonelId=" + PersonelIdQS;
            Page.Response.Redirect(newUrl);
        }
        protected void UpdateEgitimBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_EGITIM_EDIT + "?SenderApp=PD&PersonelId=" + PersonelIdQS;
            Page.Response.Redirect(newUrl);
        }
        //PersonelIdQS ile personel varsa Update Yoksa save et
        //geriye personel dondur
        private Personel PersonelSave()
        {
            Personel personel = new Personel();
            personel.Adi = AdiTxt.Text;
            personel.Soyadi = SoyadiTxt.Text;
            personel.SicilNo = SicilNoTxt.Text.ConvertToInt();
            personel.KullaniciAdi = KullaniciAdiTxt.Text;
            personel.Asker_sivil = AskerSivilDDL.SelectedValue.ConvertToInt();
            personel.Tahsili=TahsiliDDL.SelectedValue.ConvertToInt();
            personel.Tipi=PersonelTipiDDL.SelectedItem.Value.ConvertToInt();
            personel.Id = personel.Save();
            return personel;
        }
        private bool PersonelUpdate(Personel personel)
        {
            bool perSaved = false;
            if (personel != null)
            {
                personel.Adi = AdiTxt.Text;
                personel.Soyadi = SoyadiTxt.Text;
                personel.SicilNo = SicilNoTxt.Text.ConvertToInt();
                personel.KullaniciAdi = KullaniciAdiTxt.Text;
                personel.Asker_sivil = AskerSivilDDL.SelectedValue.ConvertToInt();
                personel.Tahsili = TahsiliDDL.SelectedValue.ConvertToInt();
                personel.Tipi = PersonelTipiDDL.SelectedItem.Value.ConvertToInt();
                perSaved = personel.Update();
            }
            return perSaved;
        }
        private Kimlik KimlikSave(Personel personel, string kimlikNo)
        {
            Kimlik kimlik = new Kimlik();

            if (personel != null)
            {
                kimlik = kimlik.SelectByPersonelId(personel.Id);
                if (kimlik == null)
                {
                    kimlik = new Kimlik();
                    kimlik.PersonelId = personel.Id;
                    kimlik.TCKimlikNo = kimlikNo;
                    kimlik.AnneAdi = AnneAdiTxt.Text;
                    kimlik.BabaAdi = BabaAdiTxt.Text;
                    kimlik.DogumYeri = DogumIlceDDL.SelectedValue;
                    kimlik.DogumTar = DogumTarihiTxt.Value.ConvertToDatetime();
                    kimlik.MedeniHali = MedeniHaliDDL.SelectedValue;
                    kimlik.EvlilikTar = EvlilikTarihiTxt.Value.ConvertToDatetime();
                    kimlik.Cinsiyet = CinsiyetDDL.SelectedValue;
                    kimlik.KanGrubu = KanGrubuDDL.SelectedValue;
                    kimlik.Olusturan = CurrentUserName;
                    kimlik.EvlilikKutlama = true;
                    kimlik.DogumGunuKutlama = true;
                    kimlik.Id = kimlik.Save();
                }
            }
            return kimlik;
        }
        private bool KimlikUpdate(Personel personel)
        {
            bool kimlikUpdated = false;
            if (personel != null)
            {
                Kimlik kimlik = new Kimlik();
                kimlik = kimlik.SelectByPersonelId(personel.Id);
                if (kimlik != null)
                {
                    kimlik.TCKimlikNo = TCKimlikNoTxt.Text;
                    kimlik.AnneAdi = AnneAdiTxt.Text;
                    kimlik.BabaAdi = BabaAdiTxt.Text;
                    kimlik.DogumYeri = DogumIlceDDL.SelectedValue;
                    kimlik.DogumTar = DogumTarihiTxt.Value.ConvertToDatetime();
                    kimlik.MedeniHali = MedeniHaliDDL.SelectedValue;
                    kimlik.EvlilikTar = EvlilikTarihiTxt.Value.ConvertToDatetime();
                    kimlik.Cinsiyet = CinsiyetDDL.SelectedValue;
                    kimlik.KanGrubu = KanGrubuDDL.SelectedValue;
                    kimlik.Degistiren = CurrentUserName;
                    kimlik.DogumGunuKutlama = DogumGunuKutlamaChk.Checked;
                    kimlik.EvlilikKutlama = EvlilikKutlamaChk.Checked;
                    kimlikUpdated = kimlik.Update();
                }
            }
            return kimlikUpdated;
        }
        private IsBilgileri IsBilgileriSave(Personel personel)
        {
            IsBilgileri isBilgileri = new IsBilgileri();

            if (personel != null)
            {

                isBilgileri = isBilgileri.SelectByPersonelId(personel.Id);
                if (isBilgileri == null)
                {
                    isBilgileri = new IsBilgileri();
                    isBilgileri.PersonelId = personel.Id;
                    isBilgileri.CalismaDurumu = ProjeConstants.PER_CALISIYOR_INT;
                    if (PersonelTipiDDL.SelectedItem.Value.ConvertToInt() == (int)PersonelTipi.Kadrolu)
                        isBilgileri.ProtokolSiraNo = 0;
                    else
                        isBilgileri.ProtokolSiraNo = 9999;
                    isBilgileri.Olusturan = CurrentUserName;
                    isBilgileri.ProtokolSiraNo = 9999;
                    isBilgileri.Id = isBilgileri.Save();
                }
            }
            return isBilgileri;
        }
        private bool IsBilgileriUpdate(Personel personel)
        {

            bool isSaved = false;
            if (personel != null)
            {
                IsBilgileri isBilgileri = new IsBilgileri();
                isBilgileri = isBilgileri.SelectByPersonelId(personel.Id);
                if (isBilgileri != null)
                {
                    isBilgileri.UnvanId = UnvanTanimDDL.SelectedItem.Value.ConvertToInt();
                    isBilgileri.GorevId = GorevTanimDDL.SelectedItem.Value.ConvertToInt();
                    isBilgileri.BirimId = BirimIdTxt.Text.ConvertToInt();
                    isBilgileri.BaslamaTar = IsbasTarTxt.Value.ConvertToDatetime();
                    isBilgileri.AyrilmaTar = AyrilmaTarTxt.Value.ConvertToDatetime();
                    isBilgileri.AyrilmaSebebi = AyrilmaSebebiDDL.SelectedItem.Value;
                    isBilgileri.CalismaDurumu = CalismaDurumuDDL.SelectedItem.Value.ConvertToInt();
                    isBilgileri.ProtokolSiraNo = ProtokolSirasiTxt.Text.ConvertToInt();
                    isBilgileri.SGKSicilNo = SGKSicilNoTxt.Text;
                    isBilgileri.SGKBasTar = SGKBasTarTxt.Value.ConvertToDatetime();
                    isBilgileri.VakifOncesiPrimGunSayisi = VakifOncesiPrimGunSayisiTxt.Text.ConvertToInt();
                    isBilgileri.EmeklilikTarihi = EmeklilikTarTxt.Value.ConvertToDatetime();
                    isBilgileri.IzinDonemiBasTar = IzinDonemiBasTarTxt.Value.ConvertToDatetime();
                    isBilgileri.Aciklama = IsBilgileriAciklamaTxt.Text;
                    isBilgileri.Degistiren = CurrentUserName;
                    isSaved = isBilgileri.Update();
                    if (isSaved)
                    {
                        //Kadrodaki yerini kaydet
                        if (isBilgileri.CalismaDurumu == ProjeConstants.PER_CALISIYOR_INT)
                        {
                            GorevTanim gt = new GorevTanim();
                            gt = gt.Select<GorevTanim>(isBilgileri.GorevId);
                            if (gt != null)
                            {
                                gt.PersonelId = personel.Id;
                                gt.Update();
                            }
                        }

                    }
                }
            }
            return isSaved;
        }
        private bool KadrosuzIsBilgileriUpdate(Personel personel)
        {

            bool isSaved = false;
            if (personel != null)
            {
                IsBilgileri isBilgileri = new IsBilgileri();
                isBilgileri = isBilgileri.SelectByPersonelId(personel.Id);
                if (isBilgileri != null)
                {
                    isBilgileri.BirimId = BirimDDL.SelectedItem.Value.ConvertToInt();
                    isBilgileri.BaslamaTar = KadrosuzIsbasTarihiTxt.Value.ConvertToDatetime();
                    isBilgileri.AyrilmaTar = KadrosuzAyrilmaTarihiTxt.Value.ConvertToDatetime();
                    isBilgileri.AyrilmaSebebi = KadrosuzAyrilmaSebebiDDL.SelectedItem.Value;
                    isBilgileri.CalismaDurumu = KadrosuzCalismaDurumuDDL.SelectedItem.Value.ConvertToInt();
                    isBilgileri.Aciklama = KadrosuzIsBilgileriAciklamaTxt.Text;
                    isBilgileri.Degistiren = CurrentUserName;
                    isSaved = isBilgileri.Update();

                }
            }
            return isSaved;
        }
        private void IzinDonemiBilgileriniGuncelle(Personel personel)
        {
            IzinDonem izinDonemi = new IzinDonem();
            DateTime today = DateTime.Today;
            izinDonemi = izinDonemi.SelectByIzinTarihi(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, today);
            if (izinDonemi == null)
            {
                izinDonemi = new IzinDonem();
                izinDonemi = izinDonemi.IzinDonemiOlustur(personel, ProjeConstants.IZINTIPI_UCRETLI_INT, today, CurrentUserName);
            }
            else
            {
                izinDonemi = izinDonemi.IzinDonemiGuncelle(personel, ProjeConstants.IZINTIPI_UCRETLI_INT, today, CurrentUserName);
            }
        }
        private IletisimBilgileri IletisimBilgileriSave(Personel personel)
        {
            IletisimBilgileri iletisimBilgileri = new IletisimBilgileri();

            if (personel != null)
            {
                iletisimBilgileri = new IletisimBilgileri();
                iletisimBilgileri = iletisimBilgileri.SelectByPersonelId(personel.Id);
                if (iletisimBilgileri == null)
                {
                    iletisimBilgileri = new IletisimBilgileri();
                    iletisimBilgileri.PersonelId = personel.Id;
                    iletisimBilgileri.Id = iletisimBilgileri.Save();
                }
            }
            return iletisimBilgileri;
        }
        private bool IletisimBilgileriUpdate(Personel personel)
        {

            bool iletisimSaved = false;
            if (personel != null)
            {
                IletisimBilgileri iletisimBilgileri = new IletisimBilgileri();
                iletisimBilgileri = iletisimBilgileri.SelectByPersonelId(personel.Id);
                if (iletisimBilgileri != null)
                {
                    iletisimBilgileri.Adres = AdresTxt.Text;
                    iletisimBilgileri.CepTelefonu = CepTelefonuTxt.Text;
                    iletisimBilgileri.CepTelefonu2 = CepTelefonu2Txt.Text;
                    iletisimBilgileri.DahiliTelefonu = DahiliTelefonuTxt.Text;
                    iletisimBilgileri.EvTelefonu = EvTelefonuTxt.Text;
                    iletisimBilgileri.Ili = IkametIliDDL.SelectedItem.Value;
                    iletisimBilgileri.Ilcesi = IkametIlcesiDDL.SelectedItem.Value.ConvertToInt();
                    iletisimBilgileri.InternetEPosta = InternetEPostaTxt.Text;
                    iletisimBilgileri.IntranetEPosta = IntranetEPostaTxt.Text;
                    iletisimBilgileri.OzelEPosta = OzelEPostaTxt.Text;
                    iletisimBilgileri.PersonelId = personel.Id;
                    iletisimBilgileri.PostaKodu = PostaKoduTxt.Text;
                    iletisimBilgileri.Semt = SemtTxt.Text;
                    iletisimBilgileri.Plaka = PlakaTxt.Text;

                    iletisimSaved = iletisimBilgileri.Update();
                }
            }
            return iletisimSaved;
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
        private void SetActiveTab(string activeTab)
        {
            ActiveTabQS = activeTab;
            //ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "setActiveTab('" + ActiveTabQS + "');", true);
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "setActiveTab('" + ActiveTabQS + "');", true);
        }
        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                //Girilen TC numarası kayıtlı mı kontrol et 
                Kimlik kimlik = new Kimlik();
                string kimlikNo = TCKimlikNoTxt.Text;
                //kimlikNo = string.IsNullOrEmpty(kimlikNo) ? "" : kimlikNo.Trim();

                kimlik = kimlik.SelectByTCKimlikNo(kimlikNo);
                if (kimlik == null)//Kayıtlı değilse
                {
                    //girilen personel bilgilerini kaydet
                    Personel personel = PersonelSave();
                    //kimlik tablosuna personeli gir
                    kimlik = KimlikSave(personel, kimlikNo);
                    //Isbilgileri tablosuna personeli gir
                    IsBilgileri isb = IsBilgileriSave(personel);
                    //İletisim tablosuna personeli gir
                    IletisimBilgileri iletisim = IletisimBilgileriSave(personel);
                    //Aile, Egitim, Kurs,IsTecrube   tabloları ayrıca save edilecek

                    //hataya düşmediyse kayıt tamam
                    //Per düzenleme olarak tekrar aç, açınca mesaj ver           
                    SenderAppQS = isb.CalismaDurumu == ProjeConstants.PER_AYRILDI_INT ? "EPL" : SenderAppQS;
                    RedirectToPage(ProjeConstants.PAGE_PERSONEL_EDIT + "?Mesaj=true&PersonelId=" + personel.Id + "&DestinationApp=PerD&SenderApp=" + SenderAppQS);

                }
                else
                {
                    MessageHelper.PublishMessage(kimlikNo + " TCNumarası zaten kayıtlı", ProjeConstants.MESAJ_HATA);
                }

            }
            catch (Exception ex)
            {
                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.PublishException();
            }
        }
        protected void DeleteBtn_Click(object sender, EventArgs e)
        {

        }
        protected void DogumIliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DogumIliQS = DogumIliDDL.SelectedItem.Value.ToString();
                FillDogumIlceDDL();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

        }
        protected void GorevTanimDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetActiveTab("IsBilgileriLi");
            FillBirimTxt();
        }
        protected void IkametIliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetActiveTab("IletisimLi");
            try
            {
                IkametIliQS = IkametIliDDL.SelectedItem.Value.ToString();
                FillIkametIlceDDL();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void CalismaDurumuDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetActiveTab("IsBilgileriLi");
            if (CalismaDurumuDDL.SelectedItem.Value.Equals(ProjeConstants.PER_AYRILDI_INT.ToString()))
            {
                AyrilmaTarDiv.Visible = true;
                AyrilmaSebebiDiv.Visible = true;
            }
            else
            {
                AyrilmaTarDiv.Visible = false;
                AyrilmaSebebiDiv.Visible = false;
            }
        }
        protected void KadrosuzCalismaDurumuDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetActiveTab("KadrosuzIsBilgileriLi");
            if (KadrosuzCalismaDurumuDDL.SelectedItem.Value.Equals(ProjeConstants.PER_AYRILDI_INT.ToString()))
            {
                KadrosuzAyrilmaTarihiDiv.Visible = true;
                KadrosuzAyrilmaSebebiDiv.Visible = true;
            }
            else
            {
                KadrosuzAyrilmaTarihiDiv.Visible = false;
                KadrosuzAyrilmaSebebiDiv.Visible = false;
            }
        }
        protected void MedeniHaliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetActiveTab("KimlikLi");
            if (MedeniHaliDDL.SelectedItem.Text.Equals(ProjeConstants.PER_BEKAR))
            {
                EvlilikTarihiTxt.Value = string.Empty;
                EvlilikTarihiDiv.Visible = false;
                EvlilikTarihiTxt.Visible = false;
            }
            else
            {
                EvlilikTarihiDiv.Visible = true;
                EvlilikTarihiTxt.Visible = true;
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

        protected void ResmiKaydetBtn_Click(object sender, EventArgs e)
        {
            if (xFileUpload.HasFile)
            {
                Personel personel = new Personel();
                personel = personel.Select<Personel>(PersonelIdQS.ConvertToInt());
                if (personel != null)
                {
                    string fotostr = string.IsNullOrEmpty(personel.KullaniciAdi) ? personel.Adi.ReplaceTrChars().Substring(0, 1) + personel.Soyadi.ReplaceTrChars() : personel.KullaniciAdi.ReplaceTrChars();
                    saveImageFiles2SP(fotostr);
                }

            }
            else
            {
                MessageHelper.PublishMessage("Resim Seçmediniz.", ProjeConstants.MESAJ_HATA);
            }

        }
        private void saveImageFiles2SP(string fotoFile)
        {

            //string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            //string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
            //string imgPath = newUrl + "/../" + ProjeConstants.RESIMLER_PERSONEL;
            string SPImageListName = ProjeConstants.RESIMLER_PERSONEL;

            SPWeb web = Microsoft.SharePoint.SPContext.Current.Web;
            SPList listExists = web.Lists.TryGetList(SPImageListName);
            ExceptionHelper exhelper = new ExceptionHelper();
            exhelper = UtilityHelper.uploadFile2SP(xFileUpload, "", fotoFile + "", SPImageListName, exhelper, ProjeConstants.RESIM_VESIKALIK_EN, ProjeConstants.RESIM_VESIKALIK_BOY);
            if (exhelper.HasException())
            {
                Exception ex = new Exception("Resim Kaydedilemedi.");
                exhelper.Exceptions.Add(ex);
                exhelper.PublishException();
            }
            else
            {
                MessageHelper.PublishMessage("İşlem Tamamlandı.Resim yüklendi.", ProjeConstants.MESAJ_BASARILI);
            }
        }
        protected void PersonelTipiDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isDuzenle = PersonelIdQS.ConvertToInt() > 0;
            PersonelTipiAyarlari(isDuzenle);
        }

        private void PersonelTipiAyarlari(bool isDuzenle=false)
        {
            KimlikNav.Visible = true;
            IsBilgileriNav.Visible = false;
            KadrosuzIsBilgileriNav.Visible = false;
            IletisimNav.Visible = false;
            AileNav.Visible = false;
            EgitimNav.Visible = false;
            IzinNav.Visible = false;

            if (PersonelTipiDDL.SelectedItem.Value.ConvertToInt() == (int)PersonelTipi.Kadrolu)
            {
                PersonelTipiDDL.Attributes["Class"] = "form-control form-select form-select-lg alert-success";
            }
            else
            {
                PersonelTipiDDL.Attributes["Class"] = "form-control form-select form-select-lg alert-danger";
            }
            if (isDuzenle)
            {
                if (PersonelTipiDDL.SelectedItem.Value.ConvertToInt() == (int)PersonelTipi.Kadrolu)
                {
                    PersonelTipiDDL.Attributes["class"] = "form-control form-select form-select-lg alert-success";
                    IsBilgileriNav.Visible = true;
                    KadrosuzIsBilgileriNav.Visible = false;
                    IletisimNav.Visible = true;
                    AileNav.Visible = true;
                    EgitimNav.Visible = true;
                    IzinNav.Visible = true;
                }
                else
                {
                    PersonelTipiDDL.Attributes["Class"] = "form-control form-select form-select-lg alert-danger";
                    IsBilgileriNav.Visible = false;
                    KadrosuzIsBilgileriNav.Visible = true;
                    IletisimNav.Visible = true;
                    AileNav.Visible = true;
                    EgitimNav.Visible = true;
                    IzinNav.Visible = false;
                }
            }

        }
    }
}
