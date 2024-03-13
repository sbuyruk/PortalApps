using System.Web.UI.WebControls;
using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using System.Web.UI;

namespace MTS_WebParts.KisiGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KisiGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KisiGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string KisiIdQS
        {
            get
            {

                if (ViewState["KisiId"] == null)
                {
                    if (Page.Request.QueryString["KisiId"] != null)
                    {
                        ViewState["KisiId"] = Page.Request.QueryString["KisiId"];
                    }
                    else
                    {
                        ViewState["KisiId"] = string.Empty;
                    }
                }
                return ViewState["KisiId"].ToString();
            }

            set
            {
                ViewState["KisiId"] = value;
            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)// sayfa ilk kez açılıyorsa (bu sayfanın içindeki butona basılma anı hariç)
            {
                
                MTSUnvanDDLDoldur();
                IlDDDoldurL();
                

                if (KisiIdQS.ConvertToInt()>0)
                {
                    Kisi kisi = new Kisi();
                    kisi = kisi.Select(KisiIdQS.ConvertToInt());
                    if (kisi != null)
                    {

                        IdLbl.Text = kisi == null ? string.Empty : "( " + kisi.Id + ProjeConstants.FAALIYET_KATILIMCI_DIS + " )";
                        KaydetBtn.Visible = false;
                        GuncelleBtn.Visible = true;
                        //KisiyiSilBtn.Visible = true;
                        FaaliyetGirBtn.Visible = true;
                        TitleLbl.Text = "Kişi Düzenle";
                        TitleLbl.CssClass = "col-form-label text-primary font-weight-bold mb-1";
                        FormuDoldur();
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Kişi Bulunamadı",ProjeConstants.MESAJ_HATA);
                    }
                }
                else
                {
                    TitleLbl.Text = "Kişi Girişi";
                    TitleLbl.CssClass = "col-form-label text-success font-weight-bold mb-1";
                    IdLbl.Text = string.Empty;
                    KaydetBtn.Visible = true;
                    GuncelleBtn.Visible = false;
                    KisiyiSilBtn.Visible = false;
                    FaaliyetGirBtn.Visible = false;
                    MessageHelper.PublishMessage("Yeni kişi bilgilerini girerek kayıt yapabilirsiniz.", ProjeConstants.MESAJ_BILGI,2000);
                }
            }
        }
        
        private void MTSUnvanDDLDoldur()
        {

            if (MTSUnvanTanimDDL.SelectedItem == null)
            {
                MTSUnvanTanimDDL.Items.Clear();
                MTSUnvanTanim mTSGorevTanim = new MTSUnvanTanim();
                List<MTSUnvanTanim> list = mTSGorevTanim.SelectAll<MTSUnvanTanim>();
                MTSUnvanTanimDDL.Items.Add(new ListItem("", ""));
                foreach (MTSUnvanTanim item in list)
                {
                    MTSUnvanTanimDDL.Items.Add(new ListItem(item.KisaAdi, item.Id.ToString()));
                }
            }


        }
        
        private void IlDDDoldurL()
        {
            if (IliDDL.SelectedItem == null)
            {
                IliDDL.Items.Clear();
                Il pIl = new Il();
                List<Il> list = pIl.SelectAll<Il>();
                foreach (Il il in list)
                {
                    IliDDL.Items.Add(new ListItem(il.IlAdi, il.Id.ToString()));
                }
                IlceDDLDoldur();
            }

        }
        private void IlceDDLDoldur()
        {
            IlcesiDDL.Items.Clear();
            Ilce pIlce = new Ilce();
            List<Ilce> list = pIlce.SelectByIlId(IliDDL.SelectedValue.ConvertToInt());
            foreach (Ilce ilce in list)
            {
                IlcesiDDL.Items.Add(new ListItem(ilce.IlceAdi, ilce.Id.ToString()));
            }
        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            IlceDDLDoldur();
            string ilAdi = IliDDL.SelectedItem.Text;
            Il secilenIl = new Il();
            secilenIl = secilenIl.SelectByIlAdi(ilAdi);

        }
        private void FormuDoldur()
        {
            if (KisiIdQS.ConvertToInt() > 0)
            {

                Kisi kisi = new Kisi();
                kisi = kisi.Select<Kisi>(KisiIdQS.ConvertToInt());
                if (kisi != null)
                {
                    AdiTxt.Text = kisi.Adi;
                    SoyadiTxt.Text = kisi.Soyadi;
                    TCKimlikNoTxt.Text = kisi.TCKimlikNo.ToString();
                    KurumuTxt.Text = kisi.Kurumu;
                    UnvaniTxt.Text = kisi.Unvani;
                    GoreviTxt.Text = kisi.Gorevi;
                    Telefon1Txt.Text = kisi.Telefon1;
                    Telefon2Txt.Text = kisi.Telefon2;
                    Telefon3Txt.Text = kisi.Telefon3;

                    Dahili1Txt.Text = kisi.Dahili1;
                    Dahili2Txt.Text = kisi.Dahili2;
                    Dahili3Txt.Text = kisi.Dahili3;

                    DogumTarihiTxt.Text = kisi.DogumTarihi.ConvertToDatetimeEmptyIfNull();
                    KutlamaChk.Checked = kisi.Kutlama;
                    RandevuKisitiChk.Checked = kisi.RandevuKisiti;
                    TelAciklama1Txt.Text = kisi.TelAciklama1;
                    TelAciklama2Txt.Text = kisi.TelAciklama2;
                    TelAciklama3Txt.Text = kisi.TelAciklama3;

                    AdresTxt.Text = kisi.Adres;

                    UtilityHelper.SetDDLValue(MTSUnvanTanimDDL, kisi.MTSUnvanTanimId.ToString()) ;
                    ListItem ilItem = IliDDL.Items.FindByValue(IliDDL.Items.FindByValue(kisi.Ili.ToString()).Value);
                    if (ilItem != null)
                    {
                        IliDDL.SelectedValue = ilItem.Value;
                        IlceDDLDoldur();
                        if (IlcesiDDL.Items.FindByValue(kisi.Ilcesi.ToString()) != null)
                            IlcesiDDL.SelectedValue = IlcesiDDL.Items.FindByValue(kisi.Ilcesi.ToString()).Value;
                    }

                    AdresTxt.Text = kisi.Adres;
                    AciklamaTxt.Text = kisi.Aciklama;
                    MTSKurumGorev kurumGorev = new MTSKurumGorev();
                    string kurum = string.Empty;
                    string gorev = string.Empty;
                    string kurumGorevStr = kurumGorev.SelectByKisiIdReturnKurumGorev(kisi.Id,ref kurum,ref gorev);
                    if (string.IsNullOrEmpty(kurumGorevStr))
                    {
                        MTSKurumTanimTxt.Text = kisi.Kurumu;
                        MTSGorevTanimTxt.Text = kisi.Gorevi;
                    }
                    else
                    {
                        MTSKurumTanimTxt.Text = kurum;
                        MTSGorevTanimTxt.Text = gorev;
                    }
                }
                else
                {
                    MessageHelper.PublishMessage("Kisi Bulunamadı", ProjeConstants.MESAJ_HATA);
                }

            }
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(AdiTxt.Text))
                {
                    MessageHelper.PublishMessage("Kişi Adı Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
                }
                else if (string.IsNullOrEmpty(SoyadiTxt.Text))
                {
                    MessageHelper.PublishMessage("Kişi Soyadı Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
                }
                else
                {
                    if (KayitVarMi(TCKimlikNoTxt.Text))
                    {
                        MessageHelper.PublishMessage("Bu TCKimlik numaralı bir kayıt zaten var.", ProjeConstants.MESAJ_HATA);
                    }else                        
                    {
                        Kisi yeniKisi = new Kisi();
                        yeniKisi.Adi = AdiTxt.Text;
                        yeniKisi.Soyadi = SoyadiTxt.Text;
                        yeniKisi.TCKimlikNo = TCKimlikNoTxt.Text.ConvertToLong();

                        yeniKisi.MTSUnvanTanimId = MTSUnvanTanimDDL.SelectedItem.Value.ConvertToInt();
                        yeniKisi.Kurumu = KurumuTxt.Text;
                        yeniKisi.Unvani = UnvaniTxt.Text;
                        yeniKisi.Gorevi = GoreviTxt.Text;
                        yeniKisi.Telefon1 = Telefon1Txt.Text;
                        yeniKisi.Telefon2 = Telefon2Txt.Text;
                        yeniKisi.Telefon3 = Telefon3Txt.Text;
                        yeniKisi.Dahili1 = Dahili1Txt.Text;
                        yeniKisi.Dahili2 = Dahili2Txt.Text;
                        yeniKisi.Dahili3 = Dahili3Txt.Text;
                        yeniKisi.TelAciklama1 = TelAciklama1Txt.Text;
                        yeniKisi.TelAciklama2 = TelAciklama2Txt.Text;
                        yeniKisi.TelAciklama3 = TelAciklama3Txt.Text;
                        yeniKisi.DogumTarihi = DogumTarihiTxt.Text.ConvertToDatetime();
                        yeniKisi.Kutlama = KutlamaChk.Checked;
                        yeniKisi.RandevuKisiti = RandevuKisitiChk.Checked;
                        yeniKisi.Adres = AdresTxt.Text;

                        yeniKisi.Ilcesi = IlcesiDDL.SelectedValue.ConvertToInt();
                        ListItem ilItem = IliDDL.SelectedItem;
                        yeniKisi.Ili = ilItem.Value.ConvertToInt();

                        yeniKisi.Aciklama = AciklamaTxt.Text;
                        yeniKisi.Olusturan = UtilityHelper.GetCurrentUserName();
                        int yeniId = yeniKisi.Save();
                        if (yeniId > 0)
                        {
                            KisiIdQS = yeniId.ToString();
                            RedirectToPage(ProjeConstants.PAGE_KISI_GIRIS + "?KisiId=" + KisiIdQS);
                            MessageHelper.PublishMessage("Kişi Kaydedildi.", ProjeConstants.MESAJ_BASARILI, 2000);
                        }
                        else
                        {
                            MessageHelper.PublishMessage("Kişi Kaydedilemedi.", ProjeConstants.MESAJ_HATA);
                        } 
                    }
                }

            }
            catch (Exception ex)
            {

                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
            }

        }

        private bool AdSoyadVarMi(string adi, string soyadi)
        {
            Kisi kisiDao = new Kisi();
            kisiDao = kisiDao.SelectByAdiSoyadi(adi,soyadi);
            if (kisiDao == null)
            {
                return false;
            }
            else
                return true;
        }

        private bool KayitVarMi(string tckimlikno)
        {
            if (string.IsNullOrEmpty(tckimlikno))
            {
                return false;
            }
            else
            {
                Kisi kisiDao = new Kisi();
                kisiDao = kisiDao.SelectByTCKimlikNo(tckimlikno);
                if (kisiDao == null)
                {
                    return false;
                }
                else
                    return true; 
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
        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Kisi kisi = new Kisi();
                kisi = kisi.Select<Kisi>(KisiIdQS.ConvertToInt());
                if (kisi != null)
                {
                    kisi.Adi = AdiTxt.Text;
                    kisi.Soyadi = SoyadiTxt.Text;
                    kisi.TCKimlikNo = TCKimlikNoTxt.Text.ConvertToLong();

                    kisi.MTSUnvanTanimId = MTSUnvanTanimDDL.SelectedItem.Value.ConvertToInt();
                    kisi.Kurumu = KurumuTxt.Text;
                    kisi.Unvani = UnvaniTxt.Text;
                    kisi.Gorevi = GoreviTxt.Text;
                    kisi.Telefon1 = Telefon1Txt.Text;
                    kisi.Telefon2 = Telefon2Txt.Text;
                    kisi.Telefon3 = Telefon3Txt.Text;
                    kisi.Dahili1 = Dahili1Txt.Text;
                    kisi.Dahili2 = Dahili2Txt.Text;
                    kisi.Dahili3 = Dahili3Txt.Text;
                    kisi.TelAciklama1 = TelAciklama1Txt.Text;
                    kisi.TelAciklama2 = TelAciklama2Txt.Text;
                    kisi.TelAciklama3 = TelAciklama3Txt.Text;
                    kisi.Ilcesi = IlcesiDDL.SelectedValue.ConvertToInt();
                    ListItem ilItem = IliDDL.SelectedItem;

                    kisi.Ili = ilItem.Value.ConvertToInt();

                    kisi.Adres = AdresTxt.Text;
                    kisi.Aciklama = AciklamaTxt.Text;
                    kisi.DogumTarihi = DogumTarihiTxt.Text.ConvertToDatetime();
                    kisi.Kutlama = KutlamaChk.Checked;
                    kisi.RandevuKisiti = RandevuKisitiChk.Checked;
                    kisi.Degistiren=UtilityHelper.GetCurrentUserName();
                }


                if (string.IsNullOrEmpty(AdiTxt.Text))
                {
                    MessageHelper.PublishMessage("Kişi Adı Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
                }
                else if (string.IsNullOrEmpty(SoyadiTxt.Text))
                {
                    MessageHelper.PublishMessage("Kişi Soyadı Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
                }
                else
                {
                    bool guncellendiMi = kisi.Update();
                    if (guncellendiMi)
                    {
                        RedirectToPage(ProjeConstants.PAGE_KISI_LIST+"?SecilenId="+kisi.Id);
                        MessageHelper.PublishMessage("Kişi Güncellendi.", ProjeConstants.MESAJ_BASARILI, 2000);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Kişi Güncellenmedi.", ProjeConstants.MESAJ_HATA);
                    }
                }

            }
            catch (Exception ex)
            {

                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));// + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void KisiyiSilBtn_Click(object sender, EventArgs e)
        {
            //Kişi Silme Kaldırıldı
            try
            {
                Kisi kisi = new Kisi();
                kisi = kisi.Select(KisiIdQS.ConvertToInt());
                if (kisi != null)
                {
                    KisiyiSilPopupAc(sender, kisi);
                }
            }
            catch (Exception e1)
            {
                ExceptionHelper eh = new ExceptionHelper();
                Exception e2 = new Exception("Kişi silinemedi");
                eh.Exceptions.Add(e2);
                eh.Exceptions.Add(e1);
                eh.PublishException();
            }
        }

        private void KisiyiSilPopupAc(object sender, Kisi kisi)
        {
            ParamVnLbl.Text = KisiIdQS;
            string openModal = "OpenModalOnay();";
            KisiSilNowBtn.Visible = false;
            OnaylaBtn.Visible = false;
            KisiSilNowBtn.CssClass = "btn btn-outline-danger";
            ModalBaslikLbl.CssClass = "col-form-label text-danger font-weight-bold";
            FaaliyetKatilimciChk.Visible = false;
            IrtibatPersoneliChk.Visible = false;
            bool silinebilirMi = true;
            OnayMesajiLbl.Text = string.Empty;


            bool averilenAniObjesiVarmi = VerilenAniObjesiVarMi(KisiIdQS.ConvertToInt(), ProjeConstants.FAALIYET_KATILIMCI_DIS_INT);
            if (averilenAniObjesiVarmi)
            {
                silinebilirMi = false;
                OnayMesajiLbl.Text += " Seçtiğiniz kişiye verilen anı objesi kaydı bulunmaktadır." + "</br>";
            }
            bool getirilenAniObjesiVarmi = GetirilenAniObjesiVarMi(KisiIdQS.ConvertToInt(), ProjeConstants.FAALIYET_KATILIMCI_DIS_INT);
            if (getirilenAniObjesiVarmi)
            {
                silinebilirMi = false;
                OnayMesajiLbl.Text += " Seçtiğiniz kişinin getirdiği anı objesi kaydı bulunmaktadır." + "</br>";
            }
            

            FaaliyetKatilim faaliyetKatilimDao = new FaaliyetKatilim();
            List<FaaliyetKatilim> list = faaliyetKatilimDao.SelectByKatilimciIdKatilimciTipi(KisiIdQS.ConvertToInt(), ProjeConstants.FAALIYET_KATILIMCI_DIS_INT);

            if (list.Count > 0)
            {
                silinebilirMi = false;
                
                OnayMesajiLbl.Text += "Seçtiğiniz kişinin katıldığı faaliyet bulunmaktadır." + "</br>";


            }
            AramaGorusme aramaGorusmeDao = new AramaGorusme();
            List<AramaGorusme> aramalistlist = aramaGorusmeDao.SelectAllByArayanIdReturnList(KisiIdQS.ConvertToInt(), ProjeConstants.FAALIYET_KATILIMCI_DIS_INT);

            if (aramalistlist.Count > 0)
            {
                silinebilirMi = false;

                OnayMesajiLbl.Text += "Seçtiğiniz kişinin arama/görüşme kaydı bulunmaktadır." + "</br>";


            }
            if (silinebilirMi)
            {
                kaydetGuncelleSilHdn.Value = ProjeConstants.SIL;
                OnayMesajiLbl.Visible = true;
                OnayMesajiLbl.Text = kisi.Adi +" "+kisi.Soyadi +" adlı kişiyi silmek istediğinizden emin misiniz?";
                ModalBaslikLbl.Text = kisi.Adi + " " + kisi.Soyadi + " Silinecek";
                KisiSilNowBtn.Visible = true;
            }
            else
            {
                kaydetGuncelleSilHdn.Value = ProjeConstants.BILGI;
                OnayMesajiLbl.Visible = true;
                ModalBaslikLbl.Text = kisi.Adi + " " + kisi.Soyadi + " Silinemiyor";
            }
            UtilityHelper.ScriptCalistir(openModal);
        }

        private bool VerilenAniObjesiVarMi(int kisiId, int katilimciTipi)
        {
            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            List<AniObjesiDagitim> aniObjesiDagitimList =  aniObjesiDagitim.SelectByKisiIdReturnList(kisiId, katilimciTipi, ProjeConstants.ANIOBJESI_VERILEN_INT.ToString());
            return aniObjesiDagitimList.Count > 0;
        }
        private bool GetirilenAniObjesiVarMi(int kisiId, int katilimciTipi)
        {
            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            List<AniObjesiDagitim> aniObjesiDagitimList = aniObjesiDagitim.SelectByKisiIdReturnList(kisiId, katilimciTipi, ProjeConstants.ANIOBJESI_GETIRILEN_INT.ToString());
            return aniObjesiDagitimList.Count > 0;
        }
        protected void KisiSilNowBtn_Click(object sender, EventArgs e)
        {
            bool silindi = false;
            try
            {
                Kisi kisi = new Kisi();
                kisi = kisi.Select(KisiIdQS.ConvertToInt());
                if (kisi != null)
                {
                    silindi = kisi.Delete();
                    RedirectToPage(ProjeConstants.PAGE_KISI_LIST);
                }
                if (!silindi)
                {
                    MessageHelper.PublishMessage("Kişi Silinemedi", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception e1)
            {
                ExceptionHelper eh = new ExceptionHelper();
                Exception e2 = new Exception("Kişi silinemedi");
                eh.Exceptions.Add(e2);
                eh.Exceptions.Add(e1);
                eh.PublishException();
            }
        }
        protected void FaaliyetGirBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Kisi kisi = new Kisi();
                kisi = kisi.Select(KisiIdQS.ConvertToInt());
                if (kisi != null)
                {
                    FaaliyetGirisiPopupAc(sender);
                }
            }
            catch (Exception e1)
            {
                ExceptionHelper eh = new ExceptionHelper();
                Exception e2 = new Exception("Kişi bulunamadı");
                eh.Exceptions.Add(e2);
                eh.Exceptions.Add(e1);
                eh.PublishException();
            }
        }

        private void FaaliyetGirisiPopupAc(object sender)
        {
            ParamVnLbl.Text = KisiIdQS;
            string openModal = "OpenModalOnay();";
            kaydetGuncelleSilHdn.Value = ProjeConstants.YENI;
            OnayMesajiLbl.Visible = true;
            OnayMesajiLbl.Text = "Yeni faaliyet kaydı açılmasının onaylıyor musunuz?";
            ModalBaslikLbl.Text = "Yeni Faaliyet Oluşturulacak";
            KisiSilNowBtn.Visible = false;
            OnaylaBtn.Visible = true;
            OnaylaBtn.CssClass = "btn btn-outline-success";
            FaaliyetKatilimciChk.Visible = true;
            IrtibatPersoneliChk.Visible = true;
            ModalBaslikLbl.CssClass = "col-form-label text-success font-weight-bold";

            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openModal, true);

        }

        protected void OnaylaBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (KisiIdQS.ConvertToInt() > 0)
                {
                    Faaliyet faaliyet = new Faaliyet();
                    faaliyet.FaaliyetAmaci = ProjeConstants.FAALIYET_AMACI_ZIYARET_INT.ConvertToInt();
                    faaliyet.FaaliyetKonusu = string.Empty;
                    faaliyet.FaaliyetDurumu = ProjeConstants.FAALIYET_DURUMU_PLANLANDI_INT;
                    faaliyet.TumGun = false;
                    faaliyet.AcikTarih = true;
                    DateTime baslangictarihi = DateTime.Today.AddDays(1);
                    DateTime bitistarihi = baslangictarihi;
                    string bassaat = "10:00";
                    string bitsaat = "10:30";
                    faaliyet.BaslangicTarihi = UtilityHelper.TariheSaatEkle(baslangictarihi, bassaat);
                    faaliyet.BitisTarihi = UtilityHelper.TariheSaatEkle(bitistarihi, bitsaat);
                    faaliyet.BaslangicSaati = bassaat;
                    faaliyet.BitisSaati = bitsaat;
                    faaliyet.Aciklama = AciklamaTxt.Text;

                    if (IrtibatPersoneliChk.Checked)
                        faaliyet.DisIrtibatId = KisiIdQS.ConvertToInt();
                    faaliyet.Olusturan = UtilityHelper.GetCurrentUserName();
                    faaliyet.Id = faaliyet.Save();
                    if (faaliyet.Id > 0)
                    {
                        if (FaaliyetKatilimciChk.Checked)
                        {
                            string kurumGorevStr = string.Empty;
                            Kisi kisi = new Kisi();
                            kisi = kisi.Select(KisiIdQS.ConvertToInt());
                            if (kisi != null)
                            {
                                MTSKurumGorev kurumGorev = new MTSKurumGorev();
                                string kurum = string.Empty;
                                string gorev = string.Empty;
                                kurumGorevStr = kurumGorev.SelectByKisiIdReturnKurumGorev(kisi.Id, ref kurum, ref gorev);
                                if (string.IsNullOrEmpty(kurumGorevStr))
                                {
                                    MTSKurumTanimTxt.Text = kisi.Kurumu;
                                    MTSGorevTanimTxt.Text = kisi.Gorevi;
                                    kurumGorevStr = kisi.Kurumu + " / " + kisi.Gorevi;
                                }
                                else
                                {
                                    MTSKurumTanimTxt.Text = kurum;
                                    MTSGorevTanimTxt.Text = gorev;
                                }
                            }
                            
                            FaaliyetKatilim faaliyetKatilim = new FaaliyetKatilim();
                            faaliyetKatilim.FaaliyetId = faaliyet.Id;
                            faaliyetKatilim.KatilimciId = KisiIdQS.ConvertToInt();
                            faaliyetKatilim.KatilimciTipi = ProjeConstants.FAALIYET_KATILIMCI_DIS_INT;
                            faaliyetKatilim.KurumGorev = kurumGorevStr;
                            faaliyetKatilim.Olusturan = UtilityHelper.GetCurrentUserName();
                            int faaliyetKatilimId = faaliyetKatilim.Save();
                            if (faaliyetKatilimId > 0)
                            {
                                RedirectToPage(ProjeConstants.PAGE_FAALIYET_GIRIS + "?FaaliyetId=" + faaliyet.Id);
                            }
                            else
                            {
                                throw new Exception("Faaliyet oluşturuldu, ancak katilimci eklenemedi");
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Faaliyet oluşturulamadı.");
                    }
                }
                else
                {
                    throw new Exception("Kişi Bulunamadı");
                }

                ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "CloseModalOnay();", true);
            }
            catch (Exception ex)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(ex);
                exceptionHelper.PublishException();
            }
        }
        protected void KisiListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KISI_LIST + "?SecilenId=" + KisiIdQS);
        }
        protected void FaaliyetListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_LIST);
        }
        protected void FaaliyetTakvimiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM);
        }
        protected void AramaListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_ARAMAGORUSME_LIST);
        }

        protected void AdiTxt_TextChanged(object sender, EventArgs e)
        {
            if (AdSoyadVarMi(AdiTxt.Text, SoyadiTxt.Text))
            {
                MessageHelper.PublishMessage("Bu AD ve SOYADI içeren bir kayıt zaten var. Lütfen kaydetmeden önce kontrol ediniz ", ProjeConstants.MESAJ_HATA);
            }
        }
    }
}