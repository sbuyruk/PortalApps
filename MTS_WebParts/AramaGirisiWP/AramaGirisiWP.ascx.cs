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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace MTS_WebParts.AramaGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class AramaGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public AramaGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string AramaGorusmeIdQS
        {
            get
            {

                if (ViewState["AramaGorusmeId"] == null)
                {
                    if (Page.Request.QueryString["AramaGorusmeId"] != null)
                    {
                        ViewState["AramaGorusmeId"] = Page.Request.QueryString["AramaGorusmeId"];
                    }
                    else
                    {
                        ViewState["AramaGorusmeId"] = string.Empty;
                    }
                }
                return ViewState["AramaGorusmeId"].ToString();
            }

            set
            {
                ViewState["AramaGorusmeId"] = value;
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
        private string FaaliyetIdQS
        {
            get
            {

                if (ViewState["FaaliyetId"] == null)
                {
                    if (Page.Request.QueryString["FaaliyetId"] != null)
                    {
                        ViewState["FaaliyetId"] = Page.Request.QueryString["FaaliyetId"];
                    }
                    else
                    {
                        ViewState["FaaliyetId"] = string.Empty;
                    }
                }
                return ViewState["FaaliyetId"].ToString();
            }

            set
            {
                ViewState["FaaliyetId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            IlkAcilis();
        }
        private void IlkAcilis()
        {
            if (!Page.IsPostBack)
            {
                AramaGorusmeDDLDoldur();
                //RandevuBtnEnable();
                if (AramaGorusmeIdQS.ConvertToInt() > 0)
                {
                    //düzenleme
                    DuzenleAc();
                }
                else
                {
                    //Yeni Giriş
                    GirisiAc();
                }
                RandevuBtnEnable();
            }
        }
        private void GirisiAc()
        {
            TitleLbl.Text = "Yeni Arama/Görüşme Girişi";
            TitleLbl.CssClass = "col-form-label text-success font-weight-bold mb-1";
            KaydetBtn.Visible = false;
            GuncelleBtn.Visible = false;
            SilBtn.Visible = false;
            KatilimciSecBtn.Visible = true;

            TarihTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
            SaatTxt.Text = DateTime.Now.ToString("HH:mm");
            AciklamaTxt.Text = string.Empty;
            bool katilimciBulundu = KatilimciBilgileriniDoldur();
            if (katilimciBulundu)
            {
                KaydetBtn.Visible = true;
            }
            else
            {
                KaydetBtn.Visible = false;
                GuncelleBtn.Visible = false;
                SilBtn.Visible = false;
                MessageHelper.PublishMessage("Arama/Görüşme kaydetmek için bir kişi seçmelisiniz.", ProjeConstants.MESAJ_HATA);
            }
        }
        private void DuzenleAc()
        {
            TitleLbl.Text = "Arama/Görüşme Düzenleme";
            TitleLbl.CssClass = "col-form-label text-primary font-weight-bold mb-1";
            IdLbl.Text = "( Arama/Görüşme No: " + AramaGorusmeIdQS + " )";
            KaydetBtn.Visible = false;
            GuncelleBtn.Visible = false;
            SilBtn.Visible = false;
            KatilimciSecBtn.Visible = false;
            AramaGorusme aramaGorusme = new AramaGorusme();
            aramaGorusme = aramaGorusme.Select(AramaGorusmeIdQS.ConvertToInt());
            if (aramaGorusme != null)
            {
                FaaliyetIdQS = aramaGorusme.RandevuId.ToString();
                if (aramaGorusme.RandevuId > 0)
                {
                    RandevuIstendiChk.Enabled = false;
                }
                KatilimciTipiQS = aramaGorusme.KatilimciTipi.ToString();
                ArayanIdQS = aramaGorusme.ArayanId.ToString();
                bool katilimciBulundu = KatilimciBilgileriniDoldur();
                if (katilimciBulundu)
                {
                    TarihTxt.Text = aramaGorusme.Tarih.ConvertToDatetimeEmptyIfNull();
                    SaatTxt.Text = aramaGorusme.Tarih.ToString("HH:mm");
                    KonuTxt.Text = aramaGorusme.Konu;
                    AciklamaTxt.Text = aramaGorusme.Aciklama;
                    GorusmeSaglandiChk.Checked = aramaGorusme.GorusmeSaglandi;
                    RandevuIstendiChk.Checked = aramaGorusme.RandevuIstendi;
                    UtilityHelper.SetDDLValue(GorusmeSekliDDL, aramaGorusme.GorusmeSekli);
                    GuncelleBtn.Visible = true;
                    SilBtn.Visible = true;
                }
                else
                {
                    KaydetBtn.Visible = false;
                    GuncelleBtn.Visible = false;
                    SilBtn.Visible = false;
                    MessageHelper.PublishMessage("Arama/Görüşme kaydetmek için bir katilimci seçmelisiniz.", ProjeConstants.MESAJ_HATA);
                }
            }
            else
            {
                MessageHelper.PublishMessage("Arama Kaydı bulunamadı", ProjeConstants.MESAJ_HATA);
                GirisiAc();
            }

        }
        private void AramaGorusmeDDLDoldur()
        {
            ListItem li = new ListItem(ProjeConstants.ARAMAGORUSME_GELENTELEFON);
            ListItem li1 = new ListItem(ProjeConstants.ARAMAGORUSME_GIDENTELEFON);
            ListItem li2 = new ListItem(ProjeConstants.ARAMAGORUSME_YUZYUZEGORUSME);
            ListItem li3 = new ListItem(ProjeConstants.ARAMAGORUSME_YONETICIDIREKTIFI);
            GorusmeSekliDDL.Items.Clear();
            GorusmeSekliDDL.Items.Add(li);
            GorusmeSekliDDL.Items.Add(li1);
            GorusmeSekliDDL.Items.Add(li2);
            GorusmeSekliDDL.Items.Add(li3);
        }
        private bool KatilimciBilgileriniDoldur()
        {
            bool katilimciBulundu = false;
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
                            AdiSoyadiLnk.NavigateUrl = newUrl + ProjeConstants.PAGE_KISI_GIRIS + "?KatilimciId=" + ArayanIdQS + "&KatilimciTipi=" + KatilimciTipiQS;
                            katilimciBulundu = true;
                        }
                        else
                        {
                            katilimciBulundu = false;
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
                            katilimciBulundu = true;
                        }
                        else
                        {
                            katilimciBulundu = false;
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
                            AdiSoyadiLnk.Text = (kisi.Adi + " " + kisi.Soyadi).Trim() + " (" + kisi.Kurumu + " " + kisi.Unvani + ")";
                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/";
                            AdiSoyadiLnk.NavigateUrl = newUrl + ProjeConstants.PAGE_KISI_KARTI + "?KatilimciId=" + ArayanIdQS + "&KatilimciTipi=" + KatilimciTipiQS;
                            katilimciBulundu = true;
                        }
                        else
                        {
                            katilimciBulundu = false;
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
                            #endregion
                            AdiSoyadiLnk.Text = (personel.Adi + " " + personel.Soyadi).Trim() + " (" + gorevi + ")";
                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/";
                            AdiSoyadiLnk.NavigateUrl = newUrl + ProjeConstants.PAGE_KISI_KARTI + "?KatilimciId=" + ArayanIdQS + "&KatilimciTipi=" + KatilimciTipiQS;

                            katilimciBulundu = true;
                        }
                        else
                        {
                            katilimciBulundu = false;
                            MessageHelper.PublishMessage("Personel Bulunamadı", ProjeConstants.MESAJ_HATA);
                        }
                        break;
                    }
            }
            return katilimciBulundu;
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
        protected void RandevuBtn_Click(object sender, EventArgs e)
        {
            //randevu ilgisi var mı bak (varsa popup ? olarak randevu bilgilerini göster randevuya git butonına basarak o randevuya git) 
            //
            //yoksa yeni randevu yaratayım mı diye sor, evetse yeni randevu yarat, yeni randevunun irtibat kişisine bu kişiyi ekle
            if (FaaliyetIdQS.ConvertToInt() > 0)
            {
                RedirectToPage(ProjeConstants.PAGE_FAALIYET_GIRIS + "?FaaliyetId=" + FaaliyetIdQS);
            }
            else
            {
                ModalLbl.Text = "Yeni Randevu Oluşturulacak";
                ModalLbl.CssClass = "col-form-label text-success font-weight-bold";
                MessageLbl.Text = "Bu arama/görüşme ile ilişkilendirilmiş bir randevu bulunmamaktadır. Yeni randevu oluşturulmasını oyanlıyor musunuz.";
                OnaylaBtn.Text = "Yeni Randevu Oluştur";
                OnaylaBtn.CssClass = "btn btn-outline-success";

                kaydetGuncelleSilHdn.Value = ProjeConstants.YENI;
                var openPopup = "OpenModalOnay();";
                UtilityHelper.ScriptCalistir(openPopup);

            }
        }
        protected void KisiListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KISI_LIST + "?SecilenId=" + ArayanIdQS);
        }
        protected void FaaliyetListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_LIST + "?SecilenId=" + FaaliyetIdQS);
        }
        protected void FaaliyetTakvimiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM + "?InitialDate=" + TarihTxt.Text);
        }
        protected void AramaListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_ARAMAGORUSME_LIST + "?SecilenId=" + AramaGorusmeIdQS);
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            AramaGorusme aramaGorusme = new AramaGorusme();
            aramaGorusme.Aciklama = AciklamaTxt.Text;
            aramaGorusme.ArayanId = ArayanIdQS.ConvertToInt();
            aramaGorusme.KatilimciTipi = KatilimciTipiQS.ConvertToInt();
            aramaGorusme.Konu = KonuTxt.Text;
            aramaGorusme.GorusmeSekli = GorusmeSekliDDL.SelectedItem.Text;
            aramaGorusme.Olusturan = UtilityHelper.GetCurrentUserName();
            aramaGorusme.RandevuId = FaaliyetIdQS.ConvertToInt();
            aramaGorusme.GorusmeSaglandi = GorusmeSaglandiChk.Checked;
            aramaGorusme.RandevuIstendi = RandevuIstendiChk.Checked;
            DateTime tarih = string.IsNullOrEmpty(TarihTxt.Text) ? DateTime.Now : TarihTxt.Text.ConvertToDatetime();
            string saat = string.IsNullOrEmpty(SaatTxt.Text) ? DateTime.Now.ToString("HH:mm") : SaatTxt.Text;
            DateTime tarihDT = UtilityHelper.TariheSaatEkle(tarih, SaatTxt.Text);
            aramaGorusme.Tarih = tarihDT;
            int aramaGorusmeId = aramaGorusme.Save();

            if (string.IsNullOrEmpty(KonuTxt.Text))
            {
                MessageHelper.PublishMessage("Konu Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
            }
            else
            {
                if (aramaGorusmeId > 0)
                {
                    RedirectToPage(ProjeConstants.PAGE_ARAMAGORUSME_GIRIS + "?AramaGorusmeId=" + aramaGorusmeId);
                }
            }
        }
        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            AramaGorusme aramaGorusme = new AramaGorusme();
            aramaGorusme = aramaGorusme.Select(AramaGorusmeIdQS.ConvertToInt());
            if (aramaGorusme != null)
            {
                aramaGorusme.Aciklama = AciklamaTxt.Text;
                aramaGorusme.ArayanId = ArayanIdQS.ConvertToInt();
                aramaGorusme.KatilimciTipi = KatilimciTipiQS.ConvertToInt();
                aramaGorusme.Konu = KonuTxt.Text;
                aramaGorusme.GorusmeSekli = GorusmeSekliDDL.SelectedItem.Text;
                aramaGorusme.Olusturan = UtilityHelper.GetCurrentUserName();
                aramaGorusme.RandevuId = FaaliyetIdQS.ConvertToInt();
                aramaGorusme.GorusmeSaglandi = GorusmeSaglandiChk.Checked;
                aramaGorusme.RandevuIstendi = RandevuIstendiChk.Checked;
                DateTime tarih = string.IsNullOrEmpty(TarihTxt.Text) ? DateTime.Now : TarihTxt.Text.ConvertToDatetime();
                string saat = string.IsNullOrEmpty(SaatTxt.Text) ? DateTime.Now.ToString("HH:mm") : SaatTxt.Text;
                DateTime tarihDT = UtilityHelper.TariheSaatEkle(tarih, SaatTxt.Text);
                aramaGorusme.Tarih = tarihDT;
                bool guncellendiMi = aramaGorusme.Update();
                if (guncellendiMi)
                {
                    RandevuBtnEnable();
                    MessageHelper.PublishMessage("Kayıt Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                }

            }

        }
        protected void SilBtn_Click(object sender, EventArgs e)
        {
            AramaGorusme aramaGorusme = new AramaGorusme();
            aramaGorusme = aramaGorusme.Select(AramaGorusmeIdQS.ConvertToInt());
            if (aramaGorusme != null)
            {
                ModalLbl.Text = "Arama/Görüşme Kaydı Silinecek";
                ModalLbl.CssClass = "col-form-label text-danger font-weight-bold";
                if (aramaGorusme.RandevuId > 0)
                {
                    Faaliyet faaliyet = new Faaliyet();
                    faaliyet = faaliyet.Select(aramaGorusme.RandevuId);
                    if (faaliyet != null)
                    {
                        MessageLbl.Text = "Bu arama/görüşme ile ilişkilendirilmiş bir randevu bulunmaktadır. Arama kaydını silseniz de Randevu silinmeyecektir.";
                    }
                }
                MessageLbl.Text += " Arama/Görüşme kaydının silinmesini oyanlıyor musunuz.";
                OnaylaBtn.Text = "Aramayı/Görüşmeyi Sil";
                OnaylaBtn.CssClass = "btn btn-outline-danger";

                kaydetGuncelleSilHdn.Value = ProjeConstants.SIL;
                var openPopup = "OpenModalOnay();";
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
            }
        }
        protected void TarihTxt_TextChanged(object sender, EventArgs e)
        {

        }

        private void RandevuBtnEnable()
        {
            if (AramaGorusmeIdQS.ConvertToInt() > 0)
            {
                AramaGorusme aramaGorusme = new AramaGorusme();
                aramaGorusme = aramaGorusme.Select(AramaGorusmeIdQS.ConvertToInt());
                if (aramaGorusme != null)
                {
                    Faaliyet randevu = new Faaliyet();
                    randevu = randevu.Select(aramaGorusme.RandevuId);
                    if (randevu == null)
                    {
                        aramaGorusme.RandevuId = 0;
                        aramaGorusme.Update();
                    }
                    FaaliyetIdQS = aramaGorusme.RandevuId.ToString();
                    if (FaaliyetIdQS.ConvertToInt() > 0)
                    {
                        aramaGorusme.RandevuIstendi = true;
                        aramaGorusme.Update();
                        RandevuBtn.Text = "İlgili Randevu";
                        RandevuBtn.CssClass = "btn btn-outline-primary";
                        RandevuBtn.Visible = true;
                        RandevuIstendiChk.Checked = true;
                    }
                    else
                    {
                        if (aramaGorusme.RandevuIstendi)
                        {
                            RandevuBtn.Text = "Randevu Oluştur";
                            RandevuBtn.CssClass = "btn btn-outline-secondary";
                            RandevuBtn.Visible = true;
                            RandevuIstendiChk.Checked = true;
                        }
                        else
                        {
                            RandevuBtn.Visible = false;
                            RandevuIstendiChk.Checked = false;
                        }

                    }
                }
            }
        }
        protected void OnaylaBtn_Click(object sender, EventArgs e)
        {
            if (AramaGorusmeIdQS.ConvertToInt() > 0)
            {
                AramaGorusme aramaGorusme = new AramaGorusme();
                aramaGorusme = aramaGorusme.Select(AramaGorusmeIdQS.ConvertToInt());
                if (aramaGorusme != null)
                {
                    if (kaydetGuncelleSilHdn.Value.Equals(ProjeConstants.YENI))
                    {
                        Faaliyet randevu = new Faaliyet();
                        if (aramaGorusme.GorusmeSekli.Equals(ProjeConstants.ARAMAGORUSME_GIDENTELEFON))
                        {
                            randevu.FaaliyetTipi = ProjeConstants.RANDEVU_ALINAN;
                        }
                        else
                        {
                            randevu.FaaliyetTipi = ProjeConstants.RANDEVU_VERILEN;
                        }
                        randevu.FaaliyetAmaci = ProjeConstants.FAALIYET_AMACI_ZIYARET_INT.ConvertToInt();
                        randevu.FaaliyetKonusu = aramaGorusme.Konu;
                        randevu.FaaliyetDurumu = ProjeConstants.FAALIYET_DURUMU_PLANLANDI_INT;
                        randevu.TumGun = false;
                        randevu.AcikTarih = false;
                        DateTime baslangictarihi = DateTime.Today.AddDays(1);
                        DateTime bitistarihi = baslangictarihi;
                        string bassaat = "10:00";
                        string bitsaat = "10:30";
                        randevu.BaslangicTarihi = UtilityHelper.TariheSaatEkle(baslangictarihi, bassaat);
                        randevu.BitisTarihi = UtilityHelper.TariheSaatEkle(bitistarihi, bitsaat);
                        randevu.BaslangicSaati = bassaat;
                        randevu.BitisSaati = bitsaat;
                        randevu.Aciklama = AciklamaTxt.Text;

                        if (aramaGorusme.KatilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_DIS_INT)
                            randevu.DisIrtibatId = aramaGorusme.ArayanId;
                        else if (aramaGorusme.KatilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_IC_INT)
                            randevu.IcIrtibatId = aramaGorusme.ArayanId;

                        randevu.Olusturan = UtilityHelper.GetCurrentUserName();
                        randevu.Id = randevu.Save();
                        if (randevu.Id > 0)
                        {
                            FaaliyetIdQS = randevu.Id.ToString();
                            aramaGorusme.RandevuId = randevu.Id;
                            aramaGorusme.Update();
                            RedirectToPage(ProjeConstants.PAGE_FAALIYET_GIRIS + "?FaaliyetId=" + randevu.Id);
                        }
                    }
                    else if (kaydetGuncelleSilHdn.Value.Equals(ProjeConstants.GUNCELLE))
                    {

                    }
                    else if (kaydetGuncelleSilHdn.Value.Equals(ProjeConstants.SIL))
                    {
                        if (aramaGorusme != null)
                        {

                            bool silindiMi = aramaGorusme.Delete();
                            if (silindiMi)
                            {
                                RedirectToPage(ProjeConstants.PAGE_ARAMAGORUSME_LIST + "?KisiId=" + aramaGorusme.ArayanId);
                            }

                        }
                        else
                        {
                            MessageHelper.PublishMessage("Arama/Görüşme kaydı Bulunamadı", ProjeConstants.MESAJ_HATA);
                        }
                    }
                }



            }
            else
            {
                MessageHelper.PublishMessage("Arama/Görüşme kaydı", ProjeConstants.MESAJ_HATA);
            }

            UtilityHelper.ScriptCalistir("CloseModalOnay();");
        }
        protected void YeniKisiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KISI_GIRIS);
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
            RedirectToPage(ProjeConstants.PAGE_ARAMAGORUSME_GIRIS + "?ArayanId=" + paramFaaliyetKatilimciIdLbl.Value + "&KatilimciTipi=" + paramFaaliyetKatilimciTipiLbl.Value);
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

        protected void RandevuIstendiChk_CheckedChanged(object sender, EventArgs e)
        {
            AramaGorusme aramaGorusme = new AramaGorusme();
            aramaGorusme = aramaGorusme.Select(AramaGorusmeIdQS.ConvertToInt());
            if (aramaGorusme != null)
            {
                if (aramaGorusme.RandevuIstendi && RandevuIstendiChk.Checked)
                {
                    RandevuBtn.Visible = true;
                }
                else
                {
                    RandevuBtn.Visible = false;
                }
            }

        }
    }
}

