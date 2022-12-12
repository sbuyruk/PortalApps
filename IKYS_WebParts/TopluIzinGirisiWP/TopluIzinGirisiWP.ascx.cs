using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.TopluIzinGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TopluIzinGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TopluIzinGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUser();
                }
                return ViewState["CurrentUserName"].ToString();
            }

            set
            {
                ViewState["CurrentUserName"] = value;
            }
        }
        private List<int> SecilenPersonelList //=new List<Personel>();
        {
            get
            {
                if (ViewState["SecilenPersonelList"] == null)
                {
                    Personel personel = new Personel();
                    ViewState["SecilenPersonelList"] = new List<int>().ToList();//personel.SelectCalisanPersonel().ToList();

                }
                return (List<int>)ViewState["SecilenPersonelList"];
            }
            set
            {
                ViewState["SecilenPersonelList"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    FillIzinTanim();
                    FillIzinBasSaat();
                    FillIzinBitSaat();
                    FillPersonelDDL();
                    SetLayoutByIzinTipi();
                    UyariLbl1.Text = " * Tüm personele izin girmek için, hariç tutmak istediğiniz personeli seçerek listeye ekleyiniz ve 'Tüm Personele İzin Gir' seçeneğini işaretleyiniz.";
                    UyariLbl2.Text = " * Sadece bir kısım personele izin girecekseniz, izin girmek istediğiniz personeli seçerek listeye ekleyiniz ve aşağıdan 'Sadece Seçilen Personele İzin Gir' seçeneğini işaretleyiniz.";
                    UyariLbl3.Text = " * Yapılan işlemler geriye alınamayacağından lütfen kaydetmeden önce dikkatle kontrol ediniz.";
                }
                FillHaricTutulanTable();//CikarBtn event çalışması için pagekload'da postback sışında bulunmalı
            }
            catch (Exception exception)
            {
                ExceptionHelper exHelper = new ExceptionHelper(exception);
                exHelper.PublishException();
            }
        }
        private void FillPersonelDDL()
        {
            PersonelDDL.Items.Clear();
            Personel personel = new Personel();
            List<Personel> list = personel.SelectCalisanPersonel();

            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                PersonelDDL.Items.Add(li);
            }
        }
        private void FillIzinTanim()
        {
            IzinTanimDDL.Items.Clear();
            ListItem li = new ListItem("Ücretli", "1");
            IzinTanimDDL.Items.Add(li);
            ListItem li1 = new ListItem("Mazeret", "2");
            IzinTanimDDL.Items.Add(li1);

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
        private void SetLayoutByIzinTipi()
        {
            //izin tipi Mazeret ise
            if (IzinTanimDDL.SelectedItem.Value.ConvertToInt() == (ProjeConstants.IZINTIPI_MAZERET_INT))
            {
                IzinBitTarDiv.Attributes["style"] = "display:none";
                IzinBasSaatDiv.Attributes["style"] = "display:block";
                IzinBitSaatDiv.Attributes["style"] = "display:block";
                IzinHareketDiv.Attributes["Class"] = "card-body alert-info";
                AciklamaLbl.Text = "Mazeret";
            }
            else if (IzinTanimDDL.SelectedItem.Value.ConvertToInt() == (ProjeConstants.IZINTIPI_UCRETLI_INT))
            {

                IzinBitTarDiv.Attributes["style"] = "display:block";
                IzinBasSaatDiv.Attributes["style"] = "display:none";
                IzinBitSaatDiv.Attributes["style"] = "display:none";
                HaricTutulanDiv.Attributes["style"] = "display:block";
                AciklamaLbl.Text = "Açıklama";
                IzinHareketDiv.Attributes["Class"] = "card-body alert-secondary";
            }
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
        /// <summary>
        /// Yeni Oluşturulan İzin hareketini kişi bazında kaydeder.
        /// Girilen İzin başlangıç tarihine bakarak izin dönemi başlangıcını bulur,
        /// Bulduğu döneme ait IzinDonem_Table'da kayıt yoksa, ekler
        /// </summary>
        /// <returns></returns>
        private bool YeniIzinHareketiKaydet(Personel personel, int izinTipi)
        {
            bool isSaved = false;
            try
            {

                IzinHareket izinHareket = new IzinHareket();
                izinHareket.PersonelId = personel.Id;

                izinHareket.BaslangicTarihi = IzinBasTarTxt.Value.ConvertToDatetime();
                izinHareket.Aciklama = AciklamaTxt.Text;
                izinHareket.IzinTipi = izinTipi;
                if (izinHareket.IzinTipi != ProjeConstants.IZINTIPI_MAZERET_INT)
                {
                    TimeSpan izinBitisSaati = new TimeSpan(0, 0, 0, 0);
                    DateTime bitisTar = (IzinBitTarTxt.Value.ConvertToDatetime() + izinBitisSaati);
                    izinHareket.BitisTarihi = bitisTar.ConvertToDatetime();
                    izinHareket.Birim = ProjeConstants.IZIN_BIRIMI_GUN;
                    izinHareket.Sure = IKYSOrtak.IzinSuresiHesapla(izinHareket.IzinTipi, izinHareket.BaslangicTarihi, izinHareket.BitisTarihi);
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
                }
                izinHareket.IzinDonemId = 0;
                //sadece Mazeret ve Ucretli izinler için Dönem hesapla
                if ((izinHareket.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT) ||
                   (izinHareket.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT))
                {
                    IzinDonem izinDonemi = new IzinDonem();
                    izinDonemi = izinDonemi.SelectByIzinTarihi(personel.Id, izinHareket.IzinTipi, izinHareket.BaslangicTarihi);
                    if (izinDonemi == null)
                    {
                        izinDonemi = new IzinDonem();
                        izinDonemi = izinDonemi.IzinDonemiOlustur(personel, izinHareket.IzinTipi, izinHareket.BaslangicTarihi, CurrentUserName);
                    }
                    else
                    {
                        izinDonemi = izinDonemi.IzinDonemiGuncelle(personel, izinHareket.IzinTipi, izinHareket.BaslangicTarihi, CurrentUserName);
                    }
                    string yeniSure = IKYSOrtak.IzinSuresiHesapla(izinHareket.IzinTipi, izinHareket.BaslangicTarihi, izinHareket.BitisTarihi);
                    izinDonemi.KullanilanIzinGuncelle(izinDonemi, yeniSure, true, CurrentUserName);//kullanılan izni düş

                    izinHareket.IzinDonemId = izinDonemi != null ? izinDonemi.Id : 0;
                }
                izinHareket.Olusturan = CurrentUserName;
                int id = izinHareket.Save();
                if (id > 0)
                    isSaved = true;
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
            return isSaved;
        }
        
        protected void IzinTanimDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetLayoutByIzinTipi();
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                int izinTipi = IzinTanimDDL.SelectedItem == null ? 0 : IzinTanimDDL.SelectedItem.Value.ConvertToInt();
                if (izinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                {
                    IzinBitTarTxt.Value = IzinBasTarTxt.Value;
                }
                bool isValidated = !string.IsNullOrEmpty(IzinBasTarTxt.Value.ConvertToDatetimeEmptyIfNull()) &&
                  !string.IsNullOrEmpty(IzinBitTarTxt.Value.ConvertToDatetimeEmptyIfNull());
                if (isValidated)
                {
                    if (UygulanacakGrupRL.SelectedValue.Equals("Tum"))
                    {
                        UyariMesajiLbl.Text = "Tüm personele toplu izin girişi yapılacak (Seçilen personel hariç). Lütfen değişikliği kaydetmeden önce dikkatle kontrol ediniz.";
                        SaveNowBtn.Text = "Tüm personele toplu izin gir (Seçilen personel hariç)";
                        SaveNowBtn.CssClass = "btn btn-outline-danger";
                    }
                    else
                    {
                        UyariMesajiLbl.Text = "Seçilen personele izin girişi yapılacak. Lütfen değişikliği kaydetmeden önce dikkatle kontrol ediniz.";
                        SaveNowBtn.Text = "Sadece seçilen personele izin gir ";
                        SaveNowBtn.CssClass = "btn btn-outline-primary";
                    }

                    var openPopup = "OpenModal();";
                    System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
                }
                else
                {
                    MessageHelper.PublishMessage("Lütfen izin başlangıç ve bitiş tarihi seçiniz", ProjeConstants.MESAJ_BILGI);
                }

            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Toplu izin girişinde hata ile karşılaşıldı");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }
        /// <summary>
        /// 
        /// izinbastartxt dolu mu kontrol et
        /// izinbittartxt dolu mu kontrol et
        /// calisan personel listesini al
        /// izintipini al
        /// loop
        /// haric tutulan listede ise continue;
        /// izinDonemi var mı kontrol et yoksa yeni izin donemi oluştur.
        /// kalanizin ve kullanılan izin hanelerini güncelle
        /// yeni izinHareket nesnesi yarat
        /// izinHareket bilgilerini toplu izne göre assign et
        /// izinhareket.save
        /// end loop
        /// </summary>
        /// <returns>bool</returns>
        private bool TopluIzinGir()
        {
            bool isSaved = false;
            if (UygulanacakGrupRL.SelectedValue.Equals("Tum"))
            {
                Personel personelDao = new Personel();
                List<Personel> tumCalisanPersonelList = personelDao.SelectCalisanPersonel();
                foreach (Personel personel in tumCalisanPersonelList)
                {
                    if (SecilenPersonelList.Contains(personel.Id))
                    {
                        continue;
                    }
                    else
                    {
                        int izinTipi = IzinTanimDDL.SelectedItem.Value.ConvertToInt();
                        isSaved = YeniIzinHareketiKaydet(personel, izinTipi);
                    }
                }
            }
            else if (UygulanacakGrupRL.SelectedValue.Equals("Secilen"))
            {
                Personel personelDao = new Personel();
                List<Personel> tumCalisanPersonelList = personelDao.SelectCalisanPersonel();
                foreach (Personel personel in tumCalisanPersonelList)
                {
                    if (SecilenPersonelList.Contains(personel.Id))
                    {
                        int izinTipi = IzinTanimDDL.SelectedItem.Value.ConvertToInt();
                        isSaved = YeniIzinHareketiKaydet(personel, izinTipi);

                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return isSaved;
        }
        protected void IzinBasSaatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillIzinBitSaat();
        }
        protected void PersonelDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            Personel personel = new Personel();
            personel = personel.Select<Personel>(PersonelDDL.SelectedItem.Value.ConvertToInt());
            if (personel != null)
            {
                if (!SecilenPersonelList.Contains(personel.Id))
                {
                    SecilenPersonelList.Add(personel.Id);
                    FillHaricTutulanTable();
                }

            }
            else
            {
                MessageHelper.PublishMessage("Personel Bulunamadı!", ProjeConstants.MESAJ_HATA);
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_PERSONEL_LIST;
                Page.Response.Redirect(newUrl);
            }

        }
        private void FillHaricTutulanTable()
        {
            SecilenPersonelTable.Rows.Clear();
            int SiraNo = 0;
            foreach (int item in SecilenPersonelList)
            {
                Personel personel = new Personel();
                personel = personel.Select<Personel>(item);

                if (personel != null)
                {
                    TableRow row = new TableRow();
                    TableCell adSoyadCell = new TableCell();
                    adSoyadCell.Text = personel.Adi + " " + personel.Soyadi;
                    row.Controls.Add(adSoyadCell);

                    TableCell SilCell = new TableCell();
                    LinkButton SilBtn = new LinkButton();
                    SilBtn.Text = "Çıkar";

                    SilBtn.ID = "SilBtn" + SiraNo++;
                    TableUpdatePanel.ContentTemplateContainer.Controls.Add(SilBtn);
                    SilBtn.CssClass = "btn btn-outline-danger";
                    SilBtn.CausesValidation = false;
                    SilBtn.Click += delegate
                    {
                        SecilenPersonelList.Remove(personel.Id);
                        FillHaricTutulanTable();
                    };
                    SilCell.Controls.Add(SilBtn);
                    row.Controls.Add(SilCell);

                    SecilenPersonelTable.Controls.Add(row);

                }

            }


        }
        protected void SaveNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                int izinTipi = IzinTanimDDL.SelectedItem == null ? 0 : IzinTanimDDL.SelectedItem.Value.ConvertToInt();
                if (izinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                {
                    IzinBitTarTxt.Value = IzinBasTarTxt.Value;
                }
                bool isValidated = !string.IsNullOrEmpty(IzinBasTarTxt.Value.ConvertToDatetimeEmptyIfNull()) &&
                  !string.IsNullOrEmpty(IzinBitTarTxt.Value.ConvertToDatetimeEmptyIfNull());
                bool isSaved = false;
                if (isValidated)
                {
                    isSaved = TopluIzinGir();

                    if (isSaved)
                    {

                        UyariLbl.Text = IzinBasTarTxt.Value.ConvertToDatetime() + " ile " + IzinBitTarTxt.Value.ConvertToDatetime() + " tarihleri arasında izin girişi yapılmıştır. ";
                        IzinBasTarTxt.Value = "";
                        IzinBitTarTxt.Value = "";
                        AciklamaLbl.Text = "";
                        SecilenPersonelList.Clear();
                        FillHaricTutulanTable();

                        //MailGonder(personel);
                        MessageHelper.PublishMessage("Toplu izin girişi tamamlandı", ProjeConstants.MESAJ_BASARILI, 2000);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Toplu izin kaydedilemedi", ProjeConstants.MESAJ_HATA);
                    }
                }
                else
                {
                    MessageHelper.PublishMessage("Lütfen izin başlangıç ve bitiş tarihi seçiniz", ProjeConstants.MESAJ_BILGI);
                }

            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("İzin girişinde hata ile karşılaşıldı");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }
    }
}
