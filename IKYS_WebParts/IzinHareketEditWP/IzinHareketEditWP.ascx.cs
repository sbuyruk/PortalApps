using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.IzinHareketEditWP
{
    [ToolboxItemAttribute(false)]
    public partial class IzinHareketEditWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public IzinHareketEditWP()
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
        private string IzinHareketIdQS
        {
            get
            {

                if (ViewState["IzinHareketId"] == null)
                {
                    if (Page.Request.QueryString["IzinHareketId"] != null)
                    {
                        ViewState["IzinHareketId"] = Page.Request.QueryString["IzinHareketId"];
                    }
                    else
                    {
                        ViewState["IzinHareketId"] = string.Empty;
                    }
                }
                return ViewState["IzinHareketId"].ToString();
            }

            set
            {
                ViewState["IzinHareketId"] = value;
            }
        }
        private string IzinTanimIdQS
        {
            get
            {

                if (ViewState["IzinTanimId"] == null)
                {
                    if (Page.Request.QueryString["IzinTanimId"] != null)
                    {
                        ViewState["IzinTanimId"] = Page.Request.QueryString["IzinTanimId"];
                    }
                    else
                    {
                        ViewState["IzinTanimId"] = string.Empty;
                    }
                }
                return ViewState["IzinTanimId"].ToString();
            }

            set
            {
                ViewState["IzinTanimId"] = value;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            Personel personel = PersonelGetir();
            if (!Page.IsPostBack)
            {
                if (personel != null)
                {
                    PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                    FillVekilImzaDDL();
                    FillAmirImzaDDL();
                    FillOnayImzaDDL();
                    FillIzinBasSaat();
                    FillIzinBitSaat();
                    FillIzinHareketForm();
                    GorunumuAyarla();
                }
                
                
            }
            int gunFarki = (DateTime.Today - IzinBitTarTxt.Value.ConvertToDatetime()).Days;
            if (gunFarki > ProjeConstants.IZINDUZENLEMESURESI_GUN)
            {
                UpdateBtn.Visible = false;
                DeleteBtn.Visible = false;
                IzinBasTarTxt.Disabled = true;
                IzinBitTarTxt.Disabled = true;
                IzinBasSaatDDL.Enabled = false;
                IzinBitSaatDDL.Enabled = false;
                AdresTxt.Enabled = false;
                AciklamaTxt.Enabled = false;
                AmirImzaDDL.Enabled = false;
                VekilImzaDDL.Enabled = false;
                MessageHelper.PublishMessage("İzin düzenleme süresi geçtiği için değişiklik yapılamaz.", ProjeConstants.MESAJ_BILGI,3000);
            }
        }
        private void GorunumuAyarla()
        {
            if (AuthQS.Equals("IKYS"))
            {
                IzinHareketListesiBtn.Visible = true;
            }
            else
            {
                IzinHareketListesiBtn.Visible = false;
            }
            if (DestinationAppQS.Equals("DIL"))
            {
                UpdateBtn.Visible = false;
                DeleteBtn.Visible = false;
                IzinIptalDilekceBtn.Visible = true;
                IzinDegisDilekceBtn.Visible = true;
            }
            else
            {
                UpdateBtn.Visible = true;
                DeleteBtn.Visible = true;
                IzinIptalDilekceBtn.Visible = false;
                IzinDegisDilekceBtn.Visible = false;
            }
        }
        private void FillIzinHareketForm()
        {
            IzinHareket izinHareket = new IzinHareket();
            izinHareket = izinHareket.Select<IzinHareket>(IzinHareketIdQS.ConvertToInt());
            if (izinHareket != null)
            {
                IzinHareketIdLbl.Text = izinHareket.Id.ReturnEmptyIfNull().ToString();
                IzinTanim it = new IzinTanim();
                it = it.Select<IzinTanim>(izinHareket.IzinTipi);
                IzinTipiIdLbl.Text = it.Id.ToString();
                IzinTipiLbl.Text = it.Adi;
                SelectDDLValue(VekilImzaDDL, izinHareket.VekilImza.ToString());
                SelectDDLValue(AmirImzaDDL, izinHareket.AmirImza.ToString());
                SelectDDLValue(OnayImzaDDL, izinHareket.OnayImza.ToString());
                string bassaat = izinHareket.BaslangicTarihi.ToString("HH:mm");
                SelectDDLByText(IzinBasSaatDDL, bassaat);
                string bitsaat = izinHareket.BitisTarihi.ToString("HH:mm");
                SelectDDLByText(IzinBitSaatDDL, bitsaat);

                IzinBasTarTxt.Value = izinHareket.BaslangicTarihi.ConvertToDatetimeEmptyIfNull();
                IzinBitTarTxt.Value = izinHareket.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                AdresTxt.Text = izinHareket.Adres.ReturnEmptyIfNull().ToString();
                AciklamaTxt.Text = izinHareket.Aciklama.ReturnEmptyIfNull().ToString();
                SetLayoutByIzinTipi();
            }
            else
            {
                MessageHelper.PublishMessage("İzin kaydı bulunamadı", ProjeConstants.MESAJ_BILGI);
            }


        }
        private void SetLayoutByIzinTipi()
        {
            AmirDiv.Attributes["style"] = "display:inline";
            VekilDiv.Attributes["style"] = "display:inline";
            OnayDiv.Attributes["style"] = "display:inline";
            IzinBitTarDiv.Attributes["style"] = "display:inline";
            IzinBasSaatDiv.Attributes["style"] = "display:none";
            IzinBitSaatDiv.Attributes["style"] = "display:none";
            IzinTalepDiv.Attributes["Class"] = "card-body alert-danger";
            //izin tipi Mazeret ise
            if (IzinTanimIdQS.ConvertToInt() == (ProjeConstants.IZINTIPI_MAZERET_INT))
            {
                VekilDiv.Attributes["style"] = "display:none";
                OnayDiv.Attributes["style"] = "display:none";
                IzinBitTarDiv.Attributes["style"] = "display:none";
                IzinBasSaatDiv.Attributes["style"] = "display:inline";
                IzinBitSaatDiv.Attributes["style"] = "display:inline";
                IzinTalepDiv.Attributes["Class"] = "card-body alert-warning";
            }
            else if (IzinTanimIdQS.ConvertToInt() == (ProjeConstants.IZINTIPI_UCRETLI_INT))
            {
                IzinTalepDiv.Attributes["Class"] = "card-body alert-secondary";
            }else if (IzinTanimIdQS.ConvertToInt() == (ProjeConstants.IZINTIPI_SUTIZNI_INT))
            {
                VekilDiv.Attributes["style"] = "display:none";
                AmirDiv.Attributes["style"] = "display:none";
                OnayDiv.Attributes["style"] = "display:none";
                IzinBitTarDiv.Attributes["style"] = "display:block";
                IzinBasSaatDiv.Attributes["style"] = "display:inline";
                IzinBitSaatDiv.Attributes["style"] = "display:none";
                IzinTalepDiv.Attributes["Class"] = "card-body alert-info";
            }
        }
        private string SelectDDLValue(DropDownList ddlList, string value)
        {
            if (ddlList.Items.FindByValue(value) != null)
            {
                ListItem li = ddlList.Items.FindByValue(value);
                ddlList.SelectedValue = li.Value;
                return li.Value;
            }
            return string.Empty;
        }
        private string SelectDDLByText(DropDownList ddlList, string value)
        {
            if (ddlList.Items.FindByText(value) != null)
            {
                ListItem li = ddlList.Items.FindByText(value);
                ddlList.SelectedValue = li.Value;
                return li.Value;
            }
            return string.Empty;
        }
        private void FillVekilImzaDDL()
        {
            VekilImzaDDL.Items.Clear();
            Personel personel = new Personel();
            List<Personel> list = personel.SelectCalisanPersonel();
            ListItem bosLi = new ListItem("", "0");
            VekilImzaDDL.Items.Add(bosLi);
            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                VekilImzaDDL.Items.Add(li);
            }
        }
        private void FillAmirImzaDDL()
        {
            AmirImzaDDL.Items.Clear();
            Personel personel = new Personel();
            List<Personel> list = personel.SelectCalisanPersonel();
            ListItem bosLi = new ListItem("", "0");
            AmirImzaDDL.Items.Add(bosLi);
            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                AmirImzaDDL.Items.Add(li);
            }
        }
        private void FillOnayImzaDDL()
        {
            OnayImzaDDL.Items.Clear();
            Personel personel = new Personel();
            List<Personel> list = personel.SelectCalisanPersonel();
            ListItem bosLi = new ListItem("", "0");
            OnayImzaDDL.Items.Add(bosLi);
            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                OnayImzaDDL.Items.Add(li);
            }

        }
        private void FillIzinBasSaat()
        {
            IzinBasSaatDDL.Items.Clear();
            TimeSpan bastarTS = new TimeSpan(7, 0, 0);
            TimeSpan aralikTS = new TimeSpan(0, 5, 0);
            TimeSpan bittarTS = new TimeSpan(17, 0, 0);

            TimeSpan oglenArasiBastarTS = new TimeSpan(12, 0, 0);
            TimeSpan oglenArasiBittarTS = new TimeSpan(13, 0, 0);

            TimeSpan nextTS = bastarTS;

            while (nextTS < bittarTS)
            {

                if ((nextTS >= oglenArasiBastarTS) && (nextTS < oglenArasiBittarTS))
                {
                    nextTS = nextTS + aralikTS;
                    continue;
                }
                else
                {
                    string bastarStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                    ListItem li = new ListItem(bastarStr);
                    IzinBasSaatDDL.Items.Add(li);
                    nextTS = nextTS + aralikTS;
                }
            }
        }
        private void FillIzinBitSaat()
        {
            IzinBitSaatDDL.Items.Clear();
            string bassaatStr = IzinBasSaatDDL.SelectedItem == null ? "07:05" : IzinBasSaatDDL.SelectedItem.Text;
            TimeSpan bastarTS = bassaatStr.ConvertToTimeSpan();
            TimeSpan aralikTS = new TimeSpan(0, 5, 0);
            TimeSpan bittarTS = new TimeSpan(17, 0, 0);

            TimeSpan oglenArasiBastarTS = new TimeSpan(12, 0, 0);
            TimeSpan oglenArasiBittarTS = new TimeSpan(13, 0, 0);

            TimeSpan nextTS = bastarTS;

            do
            {
                nextTS = nextTS + aralikTS;
                if ((nextTS > oglenArasiBastarTS) && (nextTS <= oglenArasiBittarTS))
                {
                    continue;
                }
                else
                {
                    string bastarStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                    ListItem li = new ListItem(bastarStr);
                    IzinBitSaatDDL.Items.Add(li);
                }

            } while (nextTS < bittarTS);
        }
        private bool IzinHareketGuncelle(IzinHareket izinHareket)
        {
            bool isSaved = false;
            try
            {
                string izinSuresiOnceki = string.Empty;
                Personel personel = PersonelGetir();

                if (izinHareket != null)
                {
                    izinHareket.BaslangicTarihi = IzinBasTarTxt.Value.ConvertToDatetime();
                    izinHareket.Adres = AdresTxt.Text;
                    izinSuresiOnceki = izinHareket.Sure;
                    if ((izinHareket.IzinTipi != ProjeConstants.IZINTIPI_MAZERET_INT)) // Mazeret izni hariç diger izinler
                    {
                        TimeSpan izinBitisSaati = new TimeSpan(0, 17, 0, 0);
                        DateTime bitisTar = (IzinBitTarTxt.Value.ConvertToDatetime() + izinBitisSaati);
                        izinHareket.BitisTarihi = bitisTar.ConvertToDatetime();
                        izinHareket.Birim = ProjeConstants.IZIN_BIRIMI_GUN;
                        izinHareket.Sure = IKYSOrtak.IzinSuresiHesapla(izinHareket.IzinTipi, izinHareket.BaslangicTarihi, izinHareket.BitisTarihi);
                        izinHareket.VekilImza = VekilImzaDDL.SelectedItem.Value.ConvertToInt();
                        izinHareket.AmirImza = AmirImzaDDL.SelectedItem.Value.ConvertToInt();
                        izinHareket.OnayImza = OnayImzaDDL.SelectedItem.Value.ConvertToInt();

                    }
                    else if (izinHareket.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                    {
                        DateTime izinTarihi = izinHareket.BaslangicTarihi;
                        string basSaat = IzinBasSaatDDL.SelectedItem.Text;
                        izinHareket.BaslangicTarihi = UtilityHelper.TariheSaatEkle(izinTarihi, basSaat);
                        string bitSaat = IzinBitSaatDDL.SelectedItem.Text;
                        izinHareket.BitisTarihi = UtilityHelper.TariheSaatEkle(izinTarihi, bitSaat);
                        izinHareket.Birim = ProjeConstants.IZIN_BIRIMI_SAAT;
                        izinHareket.Sure = IKYSOrtak.IzinSuresiHesapla(izinHareket.IzinTipi, izinHareket.BaslangicTarihi, izinHareket.BitisTarihi);
                        izinHareket.AmirImza = AmirImzaDDL.SelectedItem.Value.ConvertToInt();
                    }

                    //sadece Mazeret ve Ucretli izinler için Dönem hesapla
                    if ((izinHareket.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT) ||
                       (izinHareket.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT))
                    {
                        //mevcut izin dönemini bul
                        //izin tarihi güncellendiyse izin dönemi degisti mi
                        // evet ise eski izin doneminden kullanilan izni çikar, kalan izne ekle, yeni izin donemId sini hareket table'a yaz
                        // hayir ise yeni izin onemi zaten var mi bak yoksa yeni izin donemi olustur
                        //       izinhareket de izindonemId yi güncelle
                        // izintalepTabledaki kaydi güncelle
                        IzinDonem mevutIzinDonemi = new IzinDonem();
                        mevutIzinDonemi = mevutIzinDonemi.Select<IzinDonem>(izinHareket.IzinDonemId);

                        IzinDonem yeniIzinDonemi = new IzinDonem();
                        yeniIzinDonemi = yeniIzinDonemi.SelectByIzinTarihi(personel.Id, izinHareket.IzinTipi, izinHareket.BaslangicTarihi);

                        if (yeniIzinDonemi == null)
                        {
                            yeniIzinDonemi = new IzinDonem();
                            yeniIzinDonemi = yeniIzinDonemi.IzinDonemiOlustur(personel, izinHareket.IzinTipi, izinHareket.BaslangicTarihi, CurrentUserName);
                        }
                        else
                        {
                            yeniIzinDonemi = yeniIzinDonemi.IzinDonemiGuncelle(personel, izinHareket.IzinTipi, izinHareket.BaslangicTarihi, CurrentUserName);
                        }
                        string yeniSure = string.Empty;
                        if (yeniIzinDonemi.Id != mevutIzinDonemi.Id)
                        {
                            mevutIzinDonemi.KullanilanIzinGuncelle(mevutIzinDonemi, izinSuresiOnceki, false, CurrentUserName);//önceki dönemden düs
                            yeniSure = IKYSOrtak.IzinSuresiHesapla(izinHareket.IzinTipi, izinHareket.BaslangicTarihi, izinHareket.BitisTarihi);

                            yeniIzinDonemi.KullanilanIzinGuncelle(yeniIzinDonemi, yeniSure, true, CurrentUserName);//sonraki döneme ekle
                        }
                        else
                        {
                            yeniIzinDonemi.KullanilanIzinGuncelle(yeniIzinDonemi, izinSuresiOnceki, false, CurrentUserName);// dönemden düs
                            yeniSure = IKYSOrtak.IzinSuresiHesapla(izinHareket.IzinTipi, izinHareket.BaslangicTarihi, izinHareket.BitisTarihi);

                            yeniIzinDonemi.KullanilanIzinGuncelle(yeniIzinDonemi, yeniSure, true, CurrentUserName);//sonraki döneme ekle
                        }

                        izinHareket.IzinDonemId = yeniIzinDonemi != null ? yeniIzinDonemi.Id : 0;
                        bool isEmpty = string.IsNullOrEmpty(izinHareket.OncekiIzinStr) ||
                            string.IsNullOrEmpty(izinHareket.KullanilanIzinStr) ||
                            string.IsNullOrEmpty(izinHareket.KalanIzinStr);
                        if (!isEmpty)
                        {
                            izinHareket.OncekiIzinStr = OncekiIzniHesapla(yeniIzinDonemi,yeniSure);
                            izinHareket.KullanilanIzinStr = yeniSure;
                            izinHareket.KalanIzinStr = yeniIzinDonemi.KalanIzin;
                        }
                    }
                    izinHareket.Degistiren = CurrentUserName;

                    
                    isSaved = izinHareket.Update();

                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

            return isSaved;
        }
        public string OncekiIzniHesapla(IzinDonem izinDonemi, string sure)
        {
            string hesaplananIzin = string.Empty;


            if (izinDonemi.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT) //yalnizca izin tipi ÜCRETLI izinse  kalan izin süresini hesapla
            {
                if (izinDonemi != null)
                {
                    int kullanilanIzin = izinDonemi.KullanilanIzin.ConvertToInt();
                    int sonuc = kullanilanIzin + sure.ConvertToInt();
                    hesaplananIzin = sonuc.ToString();
                }
            }
            if (izinDonemi.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
            {
                if (izinDonemi != null)
                {
                    TimeSpan kullanilanIzin = izinDonemi.KullanilanIzin.ConvertToTimeSpan();
                    _ = new TimeSpan(0, 0, 0);
                    TimeSpan sonuc = kullanilanIzin + sure.ConvertToTimeSpan();
                    hesaplananIzin = sonuc.ToString();
                }
            }
            return hesaplananIzin;
        }
        private bool IzinHareketSil(IzinHareket izinHareket)
        {
            bool isSaved = false;
            try
            {
                string izinSuresiOnceki = string.Empty;

                if (izinHareket != null)
                {
                    izinHareket.BaslangicTarihi = IzinBasTarTxt.Value.ConvertToDatetime();
                    izinHareket.Adres = AdresTxt.Text;
                    izinSuresiOnceki = izinHareket.Sure;


                    //sadece Mazeret ve Ucretli izinler için Dönem hesapla
                    if ((izinHareket.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT) ||
                       (izinHareket.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT))
                    {
                        //mevcut izin dönemini bul
                        // izin doneminden kullanilan izni çikar, kalan izne ekle, 
                        // izintalepTabledaki kaydi güncelle
                        IzinDonem mevutIzinDonemi = new IzinDonem();
                        mevutIzinDonemi = mevutIzinDonemi.Select<IzinDonem>(izinHareket.IzinDonemId);
                        if (mevutIzinDonemi != null)
                        {
                            mevutIzinDonemi.KullanilanIzinGuncelle(mevutIzinDonemi, izinSuresiOnceki, false, CurrentUserName);// dönemden düs

                        }
                    }

                    isSaved = izinHareket.Delete();

                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

            return isSaved;
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
                string userName = CurrentUserName.Substring(CurrentUserName.LastIndexOf("\\") + 1, CurrentUserName.Length - CurrentUserName.LastIndexOf("\\") - 1);
                personel = personel.SelectByUserName(userName);
            }
            PersonelIdQS = personel.Id.ToString();
            return personel;
        }
        protected void IzinBasSaatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillIzinBitSaat();
        }
        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                IzinHareket izinHareket = new IzinHareket();
                izinHareket = izinHareket.Select<IzinHareket>(IzinHareketIdQS.ConvertToInt());
                if (izinHareket != null)
                {
                    bool isSaved = IzinHareketGuncelle(izinHareket);
                    if (isSaved)
                    {
                        IzinTalebiniGüncelle(izinHareket.IzinTalepId, izinHareket);
                        MessageHelper.PublishMessage("Izin Kaydi Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                        //RedirectToPage(ProjeConstants.PAGE_IZINHAREKET_LIST);
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Izin Kaydedilemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }
        private void IzinTalebiniGüncelle(int izinTalepId, IzinHareket izinHareket)
        {
            try
            {
                IzinTalep izinTalep = new IzinTalep();
                izinTalep = izinTalep.Select<IzinTalep>(izinTalepId);
                if (izinTalep != null)
                {

                    izinTalep.Aciklama += System.Environment.NewLine + CurrentUserName + " tarafindan " + DateTime.Now.ReturnTRDateFormat() + " tarihinde izin kaydi güncellendi";
                    izinTalep.Adres = izinHareket.Adres;
                    izinTalep.AmirImza = izinHareket.AmirImza;
                    izinTalep.BaslangicTarihi = izinHareket.BaslangicTarihi;
                    izinTalep.BitisTarihi = izinHareket.BitisTarihi;
                    izinTalep.Birim = izinHareket.Birim;
                    izinTalep.IzinDonemId = izinHareket.IzinDonemId;
                    izinTalep.IzinTipi = izinHareket.IzinTipi;
                    izinTalep.OnayImza = izinHareket.OnayImza;
                    izinTalep.Sure = izinHareket.Sure;
                    izinTalep.VekilImza = izinHareket.VekilImza;
                    izinTalep.Degistiren = CurrentUserName;
                    izinTalep.Update();
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Izin Talebi Silinemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }
        protected void IzinHareketListesiBtn_Click(object sender, EventArgs e)
        {
            if (AuthQS.Equals("IKYS"))
                RedirectToPage(ProjeConstants.PAGE_IZINHAREKET_LIST + "?SecilenId=" + IzinHareketIdQS);
        }
        protected void KisiselSayfaBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KISISELSAYFA + "?PersonelId=" + PersonelIdQS);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void DeleteBtn_Click(object sender, EventArgs e)
        {
            try
            {
                //popup aç
                var openPopup = "OpenModalOnay();";
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);

            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Izin Talebi Silinemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }
        protected void DeleteNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                IzinHareket izinHareket = new IzinHareket();
                izinHareket = izinHareket.Select<IzinHareket>(IzinHareketIdQS.ConvertToInt());
                if (izinHareket != null)
                {
                    bool isSaved = IzinHareketSil(izinHareket);
                    if (isSaved)
                    {
                        IzinTalebiniSil(izinHareket.IzinTalepId);
                        MessageHelper.PublishMessage("İzin Kaydı Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                        RedirectToPage(ProjeConstants.PAGE_IZINHAREKET_LIST);
                    }
                }



            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Izin Kaydi Silinemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }
        private void IzinTalebiniSil(int izinTalepId)
        {
            try
            {
                IzinTalep izinTalep = new IzinTalep();
                izinTalep = izinTalep.Select<IzinTalep>(izinTalepId);
                if (izinTalep != null)
                {
                    izinTalep.Delete();
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("İzin Talebi Silinemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
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
        protected void IzinDegisDilekceBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string yeniIzinBasTar = IzinBasTarTxt.Value.ConvertToDatetimeEmptyIfNull();
                string yeniIzinBitTar = IzinBitTarTxt.Value.ConvertToDatetimeEmptyIfNull();
                IzinHareket izinHareket = new IzinHareket();
                izinHareket = izinHareket.Select<IzinHareket>(IzinHareketIdQS.ConvertToInt());
                if (izinHareket != null)
                {
                    var fullUrl = string.Format("{0}?IzinHareketId={1}&AmirId={2}&OnaylayanId={3}&YeniIzinBasTar={4}&YeniIzinBitTar={5}",
                               ProjeConstants.RAPOR_UCRETLIIZINDEGISIKLIKDILEKCE, IzinHareketIdQS, AmirImzaDDL.SelectedItem.Value, OnayImzaDDL.SelectedItem.Value, yeniIzinBasTar, yeniIzinBitTar);
                    RedirectToPage(fullUrl);
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Izin Kaydi Degistirilemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }

        }
        protected void IzinIptalDilekceBtn_Click(object sender, EventArgs e)
        {

            try
            {
                IzinHareket izinHareket = new IzinHareket();
                izinHareket = izinHareket.Select<IzinHareket>(IzinHareketIdQS.ConvertToInt());
                if (izinHareket != null)
                {
                    var fullUrl = string.Format("{0}?IzinHareketId={1}&AmirId={2}&OnaylayanId={3}",
                               ProjeConstants.RAPOR_UCRETLIIZINIPTALDILEKCE, IzinHareketIdQS, AmirImzaDDL.SelectedItem.Value, OnayImzaDDL.SelectedItem.Value);
                    RedirectToPage(fullUrl);
                }



            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("İzin Kaydı İptal Edilemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }



        }
    }
}
