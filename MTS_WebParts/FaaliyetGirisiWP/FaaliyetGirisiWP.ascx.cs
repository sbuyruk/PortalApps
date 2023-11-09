using Microsoft.SharePoint.ApplicationPages.Calendar.Exchange;
using Model.IKYS;
using Model.MTS;
using Model.NBYS;
using Model.Ortak;
using Model.Portal;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace MTS_WebParts.FaaliyetGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class FaaliyetGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FaaliyetGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        private TempFaaliyet TempFaaliyetQS
        {
            get
            {
                return (TempFaaliyet)ViewState["TempFaaliyet"];
            }

            set
            {
                ViewState["TempFaaliyet"] = value;
            }
        }
        private List<string> KatilimciListQS //{ get; set; }//Postback dışında güncellenmeli
        {
            get
            {
                return (List<string>)ViewState["KatilimciList"];
            }

            set
            {
                ViewState["KatilimciList"] = value;
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
        private string IcIrtibatIdQS
        {
            get
            {

                if (ViewState["IcIrtibatId"] == null)
                {
                    if (Page.Request.QueryString["IcIrtibatId"] != null)
                    {
                        ViewState["IcIrtibatId"] = Page.Request.QueryString["IcIrtibatId"];
                    }
                    else
                    {
                        ViewState["IcIrtibatId"] = string.Empty;
                    }
                }
                return ViewState["IcIrtibatId"].ToString();
            }

            set
            {
                ViewState["IcIrtibatId"] = value;
            }
        }
        private string DisIrtibatIdQS
        {
            get
            {

                if (ViewState["DisIrtibatId"] == null)
                {
                    if (Page.Request.QueryString["DisIrtibatId"] != null)
                    {
                        ViewState["DisIrtibatId"] = Page.Request.QueryString["DisIrtibatId"];
                    }
                    else
                    {
                        ViewState["DisIrtibatId"] = string.Empty;
                    }
                }
                return ViewState["DisIrtibatId"].ToString();
            }

            set
            {
                ViewState["DisIrtibatId"] = value;
            }
        }
        private string InitialDateQS
        {
            get
            {

                if (ViewState["InitialDate"] == null)
                {
                    if (Page.Request.QueryString["InitialDate"] != null)
                    {
                        ViewState["InitialDate"] = Page.Request.QueryString["InitialDate"];
                    }
                    else
                    {
                        ViewState["InitialDate"] = string.Empty;
                    }
                }
                return ViewState["InitialDate"].ToString();
            }

            set
            {
                ViewState["InitialDate"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                if (!Page.IsPostBack) // sayfa ilk kez açılıyorsa 
                {
                    lblTime.Text = DateTime.Now.ToString("HH:mm ss");
                    RefreshTimer.Interval = 10000;
                    RefreshTimer.Enabled = true;

                    FaaliyetTipiDDLDoldur();
                    FaaliyetAmaciDDLDoldur();
                    FaaliyetDurumuDDLDoldur();
                    FaaliyetYeriDDLDoldur();
                    BaslangicSaatiDDLDoldur();
                    BitisSaatiDDLDoldur();
                    Faaliyet faaliyet = new Faaliyet();
                    faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
                    if (faaliyet == null)
                    {
                        GirisiAc();
                    }
                    else
                    {

                        if (faaliyet != null)
                        {
                            TempFaaliyetQS = new TempFaaliyet(faaliyet);
                            DuzenleAc(faaliyet);
                            KatilimciBilgileriniDoldur(faaliyet, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exhelper = new ExceptionHelper(ex);
                exhelper.PublishException();
            }
        }
        private void FaaliyetFormunuDoldur(Faaliyet faaliyet)
        {
            if (faaliyet != null)
            {
                IdLbl.Text = " ( Faaliyet No: " + faaliyet.Id.ToString() + " )";
                TumGunChk.Checked = faaliyet.TumGun;
                AcikTarihChk.Checked = faaliyet.AcikTarih;
                FaaliyetKonusuTxt.Text = faaliyet.FaaliyetKonusu;
                BaslangicTarihiTxt.Text = faaliyet.BaslangicTarihi.ConvertToDatetimeEmptyIfNull();
                InitialDateQS = faaliyet.BaslangicTarihi.ToString("yyyy-MM-dd");
                BitisTarihiTxt.Text = faaliyet.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                AciklamaTxt.Text = faaliyet.Aciklama;
                YoneticiNotuTxt.Text = faaliyet.YoneticiNotu;

                UtilityHelper.SetDDLValue(FaaliyetTipiDDL, faaliyet.FaaliyetTipi);
                UtilityHelper.SetDDLValue(FaaliyetAmaciDDL, faaliyet.FaaliyetAmaci.ToString());
                UtilityHelper.SetDDLValue(FaaliyetDurumuDDL, faaliyet.FaaliyetDurumu.ToString());
                UtilityHelper.SetDDLValue(FaaliyetYeriDDL, faaliyet.FaaliyetYeri.ToString());
                UtilityHelper.SetDDLValue(BasSaatDDL, faaliyet.BaslangicSaati);
                UtilityHelper.SetDDLValue(BitSaatDDL, faaliyet.BitisSaati);

                IcIrtibatIdQS = faaliyet.IcIrtibatId.ToString();
                DisIrtibatIdQS = faaliyet.DisIrtibatId.ToString();
            }

        }
        private void KatilimciBilgileriniDoldur(Faaliyet faaliyet, bool buttonsEnabled)
        {
            if (faaliyet != null)
            {
                AramaGorusmeBilgileriniDoldur(faaliyet);
                IcIrtibatNoktasiDoldur(faaliyet.IcIrtibatId, buttonsEnabled);
                DisIrtibatNoktasiDoldur(faaliyet.DisIrtibatId, buttonsEnabled);

                TabloOlustur(buttonsEnabled);
                if (buttonsEnabled)
                {
                    TempFaaliyetQS.IcIrtibatId = faaliyet.IcIrtibatId;
                    TempFaaliyetQS.DisIrtibatId = faaliyet.DisIrtibatId;
                }


            }

        }

        private void GirisiAc()
        {
            KatilimciBilgileriDiv.Attributes["style"] = "display:none";
            KaydetBtn.Visible = true;
            GuncelleBtn.Visible = false;
            FaaliyetSilBtn.Visible = false;
            FaaliyetKartiBtn.Visible = false;
            TitleLbl.Text = "Yeni Faaliyet";
            TitleLbl.CssClass = "col-form-label text-success font-weight-bold mb-1";
            BaslangicTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
            BitisTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
            FaaliyetIdQS = string.Empty;
            AramaGorusmeIdQS = string.Empty;
            IcIrtibatIdQS = string.Empty;
            DisIrtibatIdQS = string.Empty;
            MesajQS = string.Empty;
            if (KatilimciListQS != null)
                KatilimciListQS.Clear();
            TempFaaliyetQS = null;

        }
        private void DuzenleAc(Faaliyet faaliyet)
        {
            if (FaaliyetIdQS.ConvertToInt() > 0)
            {
                KatilimciBilgileriDiv.Attributes["style"] = "display:block";
                KaydetBtn.Visible = false;
                GuncelleBtn.Visible = true;
                FaaliyetKartiBtn.Visible = true;
                FaaliyetSilBtn.Visible = true;
                FaaliyetFormunuDoldur(faaliyet);
                if (!string.IsNullOrEmpty(MesajQS))
                {
                    MessageHelper.PublishMessage("Faaliyet Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                    MesajQS = string.Empty;
                }
            }
            else
            {
                GirisiAc();
                MessageHelper.PublishMessage("Faaliyet bulunamadı. Yeni faaliyet girebilirsiniz.", ProjeConstants.MESAJ_BILGI, 2000);
            }

        }
        private void BaslangicSaatiDDLDoldur()
        {
            BasSaatDDL.Items.Clear();
            TimeSpan bastarTS = new TimeSpan(6, 0, 0);
            TimeSpan aralikTS = new TimeSpan(0, 15, 0);
            TimeSpan bittarTS = new TimeSpan(21, 0, 0);

            TimeSpan nextTS = bastarTS;

            while (nextTS < bittarTS)
            {
                string bastarStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                ListItem li = new ListItem(bastarStr);
                BasSaatDDL.Items.Add(li);
                nextTS += aralikTS;
            }
            UtilityHelper.SetDDLValue(BasSaatDDL, "08:00");

        }
        protected void BasSaatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            BitisSaatiDDLDoldur();
        }
        private void BitisSaatiDDLDoldur()
        {
            BitSaatDDL.Items.Clear();
            string basSaatStr = BasSaatDDL.SelectedItem == null ? "08:00" : BasSaatDDL.SelectedItem.Text;
            if (BitisTarihiTxt.Text.ConvertToDatetime() > BaslangicTarihiTxt.Text.ConvertToDatetime())
            {
                basSaatStr = "08:00";
            }
            TimeSpan bastarTS = basSaatStr.ConvertToTimeSpan();
            TimeSpan aralikTS = new TimeSpan(0, 15, 0);
            TimeSpan bittarTS = new TimeSpan(21, 0, 0);

            TimeSpan nextTS = bastarTS;

            do
            {
                nextTS += aralikTS;
                string bittarStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                ListItem li = new ListItem(bittarStr);
                BitSaatDDL.Items.Add(li);

            } while (nextTS < bittarTS);
            TimeSpan bittarValTS = bittarTS + aralikTS;
            string bittarVal = string.Format("{0:00}:{1:00}", bittarValTS.Hours, bittarValTS.Minutes);
            UtilityHelper.SetDDLValue(BitSaatDDL, bittarVal);
        }
        protected void BitSaatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            int faaliyetId = 0;
            try
            {
                Faaliyet faaliyet = new Faaliyet();
                faaliyet.FaaliyetTipi = FaaliyetTipiDDL.SelectedItem.Text;
                faaliyet.FaaliyetAmaci = FaaliyetAmaciDDL.SelectedItem.Value.ConvertToInt();
                faaliyet.FaaliyetKonusu = FaaliyetKonusuTxt.Text;
                faaliyet.FaaliyetYeri = FaaliyetYeriDDL.SelectedItem.Value.ConvertToInt();
                faaliyet.FaaliyetDurumu = FaaliyetDurumuDDL.SelectedItem.Value.ConvertToInt();
                faaliyet.TumGun = TumGunChk.Checked.ConvertToBool();
                faaliyet.AcikTarih = AcikTarihChk.Checked.ConvertToBool();
                DateTime baslangictarihi = BaslangicTarihiTxt.Text.ConvertToDatetime();
                DateTime bitistarihi = BitisTarihiTxt.Text.ConvertToDatetime();
                string bassaat = BasSaatDDL.SelectedItem.Value;
                string bitsaat = BitSaatDDL.SelectedItem.Value;
                faaliyet.BaslangicTarihi = UtilityHelper.TariheSaatEkle(baslangictarihi, bassaat);
                faaliyet.BitisTarihi = UtilityHelper.TariheSaatEkle(bitistarihi, bitsaat);
                faaliyet.BaslangicSaati = bassaat;
                faaliyet.BitisSaati = bitsaat;
                faaliyet.Aciklama = AciklamaTxt.Text;
                faaliyet.YoneticiNotu = YoneticiNotuTxt.Text;
                faaliyet.IcIrtibatId = IcIrtibatIdQS.ConvertToInt();
                faaliyet.DisIrtibatId = DisIrtibatIdQS.ConvertToInt();
                faaliyet.Olusturan = UtilityHelper.GetCurrentUserName();

                if (string.IsNullOrEmpty(FaaliyetKonusuTxt.Text))
                {
                    MessageHelper.PublishMessage("Faaliyet Konusu Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
                }
                else
                {

                    faaliyetId = faaliyet.Id = faaliyet.Save();
                    if (faaliyetId > 0)
                    {
                        FaaliyetIdQS = faaliyetId.ToString();
                        //Save icinde olay kaydı var zaten, o yüzden kommentlendi
                        //OlayKayit olayKayit = new OlayKayit();
                        //olayKayit.GirisOlayKaydet(faaliyet, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYET);
                        MessageHelper.PublishMessage("Faaliyet Kaydedildi.", ProjeConstants.MESAJ_BASARILI, 2000);
                        KatilimciBilgileriDiv.Attributes["style"] = "display:block";
                        InitialDateQS = faaliyet.BaslangicTarihi.ToString("yyyy-MM-dd");
                        OzelKalemTakvimineIsle(faaliyet, ProjeConstants.KAYDET);
                        RedirectToPage(ProjeConstants.PAGE_FAALIYET_GIRIS + "?FaaliyetId=" + FaaliyetIdQS + "&Mesaj=true");
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Faaliyet Kaydedilemedi.", ProjeConstants.MESAJ_HATA);
                    }
                }
            }
            catch (Exception exception)
            {

                ExceptionHelper eh = new ExceptionHelper(exception);
                eh.PublishException();

            }

        }

        private void OzelKalemTakvimineIsle(Faaliyet faaliyet, string islemTipi)
        {
            if (OzelKalemTakvimiChk.Checked && faaliyet != null)
            {
                string from = ProjeConstants.PARAM_MTSMAILADRESI;
                string userto = ProjeConstants.PARAM_OZELKALEMMAILADRESI;
                string baslik = faaliyet.FaaliyetKonusu;
                FaaliyetYeri faaliyetYeri = new FaaliyetYeri();
                faaliyetYeri = faaliyetYeri.Select(faaliyet.FaaliyetYeri);
                string faaliyetYeriStr = faaliyetYeri != null ? faaliyetYeri.Adi : string.Empty;

                if (islemTipi.Equals(ProjeConstants.KAYDET) || islemTipi.Equals(ProjeConstants.GUNCELLE))
                {
                    MailHelper.TakvimeEkle(faaliyet.UniqueId, from, userto, baslik, faaliyet.BaslangicTarihi, faaliyet.BitisTarihi, faaliyetYeriStr,
                        faaliyet.Aciklama, ProjeConstants.PARAM_INTERNET_SMTP_IP_ADRESI);

                }else if (islemTipi.Equals(ProjeConstants.SIL))
                {
                    MailHelper.TakvimdenSil(faaliyet.UniqueId, from, userto, baslik, faaliyet.BaslangicTarihi, faaliyet.BitisTarihi, faaliyetYeriStr, 
                        faaliyet.Aciklama, ProjeConstants.PARAM_INTERNET_SMTP_IP_ADRESI);
                }

            }
            else
            {
                MessageHelper.PublishMessage("Özel kalem takvimine işlenmedi",ProjeConstants.MESAJ_BILGI,2000);
            }
        }

        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            bool guncellendiMi = false;
            try
            {

                Faaliyet faaliyet = new Faaliyet();
                faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
                Faaliyet faaliyetIlkHali = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
                faaliyet.Degistiren = UtilityHelper.GetCurrentUserName();
                if (faaliyet == null)
                {
                    MessageHelper.PublishMessage("Faaliyet bulunamadı", ProjeConstants.MESAJ_HATA, 5000);
                }
                else
                {
                    faaliyet.FaaliyetTipi = FaaliyetTipiDDL.SelectedItem.Text;
                    faaliyet.FaaliyetAmaci = FaaliyetAmaciDDL.SelectedItem.Value.ConvertToInt();
                    faaliyet.FaaliyetKonusu = FaaliyetKonusuTxt.Text;
                    faaliyet.FaaliyetYeri = FaaliyetYeriDDL.SelectedItem.Value.ConvertToInt();
                    faaliyet.FaaliyetDurumu = FaaliyetDurumuDDL.SelectedItem.Value.ConvertToInt();
                    faaliyet.TumGun = TumGunChk.Checked.ConvertToBool();
                    faaliyet.AcikTarih = AcikTarihChk.Checked.ConvertToBool();
                    DateTime baslangictarihi = BaslangicTarihiTxt.Text.ConvertToDatetime();
                    DateTime bitistarihi = BitisTarihiTxt.Text.ConvertToDatetime();
                    string bassaat = BasSaatDDL.SelectedItem.Value;
                    string bitsaat = BitSaatDDL.SelectedItem.Value;
                    faaliyet.BaslangicTarihi = UtilityHelper.TariheSaatEkle(baslangictarihi, bassaat);
                    faaliyet.BitisTarihi = UtilityHelper.TariheSaatEkle(bitistarihi, bitsaat);
                    faaliyet.BaslangicSaati = bassaat;
                    faaliyet.BitisSaati = bitsaat;
                    faaliyet.Aciklama = AciklamaTxt.Text;
                    faaliyet.YoneticiNotu = YoneticiNotuTxt.Text;
                    faaliyet.IcIrtibatId = IcIrtibatIdQS.ConvertToInt();
                    faaliyet.DisIrtibatId = DisIrtibatIdQS.ConvertToInt();
                    faaliyet.Degistiren = UtilityHelper.GetCurrentUserName();
                    guncellendiMi = faaliyet.Update();

                    if (guncellendiMi)
                    {
                        TempFaaliyetQS = new TempFaaliyet(faaliyet);
                        InitialDateQS = faaliyet.BaslangicTarihi.ToString("yyyy-MM-dd");
                        //KatilimciBilgileriniDoldur();
                        MessageHelper.PublishMessage("Faaliyet güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                        OzelKalemTakvimineIsle(faaliyet, ProjeConstants.GUNCELLE);

                    }
                    else
                    {
                        MessageHelper.PublishMessage("Faaliyet güncellenemedi", ProjeConstants.MESAJ_HATA, 5000);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHelper exhelper = new ExceptionHelper(ex);
                exhelper.Exceptions.Add(new Exception("Faaliyet güncellenemedi."));
                exhelper.PublishException();
            }
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
        protected void FaaliyetSilBtn_Click(object sender, EventArgs e)
        {
            Faaliyet faaliyet = new Faaliyet();
            faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
            if (faaliyet != null)
            {
                if (!BaglantisiVarMi(faaliyet))
                {
                    FaaliyetSilPopupAc(sender);
                }

            }
            else
            {
                MessageHelper.PublishMessage("Faaliyet bulunamadığınıdan dolayı silinemedi.", ProjeConstants.MESAJ_HATA);
            }
            //Faaliyet Silme Kaldırıldı SB 31.05.2021
            //try
            //{
            //    Faaliyet faaliyet = new Faaliyet();
            //    faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
            //    if (faaliyet != null)
            //    {
            //        FaaliyetSilPopupAc(sender);
            //    }
            //}
            //catch (Exception e1)
            //{
            //    ExceptionHelper eh = new ExceptionHelper();
            //    Exception e2 = new Exception("Kişi silinemedi");
            //    eh.Exceptions.Add(e2);
            //    eh.Exceptions.Add(e1);
            //    eh.PublishException();
            //}
        }

        private bool BaglantisiVarMi(Faaliyet faaliyet)
        {
            bool baglantisiVar = false;
            ExceptionHelper eh = new ExceptionHelper();

            AramaGorusme aramaGorusme = new AramaGorusme();
            aramaGorusme = aramaGorusme.SelectByRandevuId(faaliyet.Id);

            FaaliyetKatilim faaliyetKatilim = new FaaliyetKatilim();
            List<FaaliyetKatilim> faaliyetKatilimList = faaliyetKatilim.SelectByFaaliyetId(faaliyet.Id);
            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            List<AniObjesiDagitim> aniObjesiDagitimList = aniObjesiDagitim.SelectByRandevuId(faaliyet.Id);
            if (aramaGorusme != null)
            {
                Exception ex = new Exception("Faaliyetin bağlantılı olduğu Arama/Görüşme bulunmaktadır.");
                eh.Exceptions.Add(ex);
                baglantisiVar = true;
            }
            if (faaliyetKatilimList.Count > 0)
            {
                Exception ex = new Exception("Faaliyetin bağlantılı olduğu Katılımcı(lar) bulunmaktadır.");
                eh.Exceptions.Add(ex);
                baglantisiVar = true;
            }
            if (aniObjesiDagitimList.Count > 0)
            {
                Exception ex = new Exception("Faaliyetin bağlantılı olduğu Anı Objeleri bulunmaktadır.");
                eh.Exceptions.Add(ex);
                baglantisiVar = true;
            }
            if (baglantisiVar)
            {
                Exception ex = new Exception("Faaliyet, bağlantıları nedeniyle silinemez. ");
                eh.Exceptions.Add(ex);
                eh.PublishException();
            }
            return baglantisiVar;
        }

        protected void KisiListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KISI_LIST);
        }
        protected void FaaliyetListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_LIST + "?SecilenId=" + FaaliyetIdQS);
        }
        protected void FaaliyetTakvimiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM + "?InitialDate=" + InitialDateQS);
        }
        private void FaaliyetSilPopupAc(object sender)
        {
            ParamVnLbl.Text = FaaliyetIdQS;
            string openModal = "OpenSilModal();";
            SilMesajiLbl.Visible = true;
            SilMesajiLbl.Text = "Faaliyete ait tüm bilgiler silinecek. Silmek istediğinizden emin misiniz?";
            SilModalBaslikLbl.Text = "Faaliyet Silinecek";
            FaaliyetSilNowBtn.Visible = true;
            UtilityHelper.ScriptCalistir(openModal);
        }
        /// FaaliyetKatilim_Tabeledan bu faaliyet Id'li kayıtları sil
        /// AramaGorusme_Table'da bu faaliyetId'li kayıtların faaliyetId'sini 0 yap
        /// AniObjesiDagitim_Table'dan bu faaliyetId'li olanları sil
        protected void FaaliyetSilNowBtn_Click(object sender, EventArgs e)
        {
            bool silindi = false;
            try
            {
                Faaliyet faaliyet = new Faaliyet();
                faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
                if (faaliyet != null)
                {
                    //bağlantısı yoksa sil
                    if (!BaglantisiVarMi(faaliyet))
                    {
                        silindi = faaliyet.Delete();
                    }

                    if (silindi)
                    {
                        MessageHelper.PublishMessage("Faaliyet Silindi", ProjeConstants.MESAJ_HATA);
                        OzelKalemTakvimineIsle(faaliyet, ProjeConstants.SIL);
                        RedirectToPage(ProjeConstants.PAGE_FAALIYET_LIST + "?Mesaj=true");
                    }
                }
                if (!silindi)
                {
                    MessageHelper.PublishMessage("Faaliyet Silinemedi", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception e1)
            {
                ExceptionHelper eh = new ExceptionHelper();
                Exception e2 = new Exception("Faaliyet silinemedi");
                eh.Exceptions.Add(e2);
                eh.Exceptions.Add(e1);
                eh.PublishException();
            }
        }
        private void KatilimciModalAc(int katilimciTipi)
        {
            TabloModalOlustur();
            UtilityHelper.ScriptCalistir("KatilimciSecimiModal();");
        }
        protected void SecilenKatilimciyiKaydetNowBtn_Click(object sender, EventArgs e)
        {
            int katilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
            int faaliyetId = paramFaaliyetIdLbl.Value.ConvertToInt();
            int katilimciTipi = paramFaaliyetKatilimciTipiLbl.Value.ConvertToInt();
            if (katilimciId > 0)
            {
                FaaliyetKatilim faaliyetKatilim = new FaaliyetKatilim();

                List<FaaliyetKatilim> list = faaliyetKatilim.Select(faaliyetId, katilimciId, katilimciTipi);
                if (list.Count < 1)
                {
                    string kurumGorevStr = string.Empty;
                    if (katilimciTipi==ProjeConstants.FAALIYET_KATILIMCI_DIS_INT)
                    {
                        Kisi katilimci = new Kisi();
                        katilimci = katilimci.Select(katilimciId);
                        if (katilimci != null)
                        {
                            MTSKurumGorev kurumGorev = new MTSKurumGorev();
                            string kurum = string.Empty;
                            string gorev = string.Empty;
                            kurumGorevStr = kurumGorev.SelectByKisiIdReturnKurumGorev(katilimci.Id, ref kurum, ref gorev);
                            if (string.IsNullOrEmpty(kurumGorevStr))
                            {
                                kurumGorevStr = katilimci.Kurumu + " / " + katilimci.Gorevi;
                            }
                        } 
                    }
                    else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_IC_INT)
                    {
                        Personel katilimci = new Personel();
                        katilimci = katilimci.Select(katilimciId);
                        if (katilimci != null)
                        {
                            MTSKurumGorev kurumGorev = new MTSKurumGorev();
                            string kurum = string.Empty;
                            string gorev = string.Empty;
                            kurumGorevStr = kurumGorev.SelectByKisiIdReturnKurumGorev(katilimciId,ref kurum,ref gorev);
                            if (string.IsNullOrEmpty(kurumGorevStr))
                            {
                                kurumGorevStr = ProjeConstants.FAALIYET_KATILIMCI_IC;
                            }
                        }
                    }
                    else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_IC_INT)
                    {
                        TasinmazBagisci katilimci = new TasinmazBagisci();
                        katilimci = katilimci.Select<TasinmazBagisci>(katilimciId);
                        if (katilimci != null)
                        {
                            MTSKurumGorev kurumGorev = new MTSKurumGorev();
                            string kurum = string.Empty;
                            string gorev = string.Empty;
                            kurumGorevStr = kurumGorev.SelectByKisiIdReturnKurumGorev(katilimciId,ref kurum, ref gorev);
                            if (string.IsNullOrEmpty(kurumGorevStr))
                            {
                                kurumGorevStr = ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI;
                            }
                        }
                    }
                    else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI_INT)
                    {
                        NakitBagisci katilimci = new NakitBagisci();
                        katilimci = katilimci.Select<NakitBagisci>(katilimciId);
                        if (katilimci != null)
                        {
                            MTSKurumGorev kurumGorev = new MTSKurumGorev();
                            string kurum = string.Empty;
                            string gorev = string.Empty;
                            kurumGorevStr = kurumGorev.SelectByKisiIdReturnKurumGorev(katilimciId, ref kurum, ref gorev);
                            if (string.IsNullOrEmpty(kurumGorevStr))
                            {
                                kurumGorevStr = ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI;
                            }
                        }
                    }

                    faaliyetKatilim.KatilimciId = katilimciId;
                    faaliyetKatilim.FaaliyetId = faaliyetId;
                    faaliyetKatilim.KatilimciTipi = katilimciTipi;
                    faaliyetKatilim.KurumGorev = kurumGorevStr;
                    faaliyetKatilim.Save();
                }
            }
            else
            {
                MessageHelper.PublishMessage("Katılımcı Bulunamadı", ProjeConstants.MESAJ_HATA);
            }
            Faaliyet faaliyet = new Faaliyet();
            faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
            if (faaliyet != null)
            {
                KatilimciBilgileriniDoldur(faaliyet, true);
            }


        }
        //katılımcı seçme ve ekleme
        protected void KatilimciEkleBtn_Click(object sender, EventArgs e)
        {
            KatilimciModalAc(ProjeConstants.FAALIYET_KATILIMCI_IC_INT);
        }

        protected void KatilimciCikarBtn_Click(object sender, EventArgs e)
        {
            int faaliyetkatilimId = paramFaaliyetKatilimIdLbl.Value.ConvertToInt();
            if (faaliyetkatilimId > 0)
            {
                FaaliyetKatilim faaliyetKatilim = new FaaliyetKatilim();
                faaliyetKatilim = faaliyetKatilim.Select(faaliyetkatilimId);
                if (faaliyetKatilim != null)
                {
                    AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
                    DataTable dataTable = aniObjesiDagitim.SelectByKatilimcidKatilimciTipiRandevuId(faaliyetKatilim.KatilimciId, faaliyetKatilim.KatilimciTipi,faaliyetKatilim.FaaliyetId,string.Empty);
                    if (dataTable == null)
                    {
                        if (faaliyetKatilim.Delete())
                        {
                            MessageHelper.PublishMessage("Katılımcı faaliyetten çıkarıldı", ProjeConstants.MESAJ_BASARILI);
                        }
                    }
                    else
                    {
                        ExceptionHelper eh= new ExceptionHelper();
                        Exception ex = new Exception("Faaliyetin bağlantılı olduğu Anı Objeleri bulunmaktadır.");
                        eh.Exceptions.Add(ex);
                        eh.PublishException();
                    }
                }

            }
            else
            {
                MessageHelper.PublishMessage("Katılımcı Bulunamadı", ProjeConstants.MESAJ_HATA);
            }
            Faaliyet faaliyet = new Faaliyet();
            faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
            if (faaliyet != null)
            {
                KatilimciBilgileriniDoldur(faaliyet, true);
            }
        }
        private void FaaliyetTipiDDLDoldur()
        {
            ListItem li = new ListItem(ProjeConstants.RANDEVU_VERILEN);
            ListItem li2 = new ListItem(ProjeConstants.RANDEVU_ALINAN);

            FaaliyetTipiDDL.Items.Clear();
            FaaliyetTipiDDL.Items.Add(li);
            FaaliyetTipiDDL.Items.Add(li2);
        }
        private void FaaliyetAmaciDDLDoldur()
        {
            ListItem li5 = new ListItem(ProjeConstants.FAALIYET_AMACI_TOPLANTI, ProjeConstants.FAALIYET_AMACI_TOPLANTI_INT);
            ListItem li = new ListItem(ProjeConstants.FAALIYET_AMACI_ZIYARET, ProjeConstants.FAALIYET_AMACI_ZIYARET_INT);
            ListItem li2 = new ListItem(ProjeConstants.FAALIYET_AMACI_DAVET, ProjeConstants.FAALIYET_AMACI_DAVET_INT);
            ListItem li3 = new ListItem(ProjeConstants.FAALIYET_AMACI_YILDONUMU, ProjeConstants.FAALIYET_AMACI_YILDONUMU_INT);
            ListItem li4 = new ListItem(ProjeConstants.FAALIYET_AMACI_OZELCALISMA, ProjeConstants.FAALIYET_AMACI_OZELCALISMA_INT);
            ListItem li6 = new ListItem(ProjeConstants.FAALIYET_AMACI_IZIN, ProjeConstants.FAALIYET_AMACI_IZIN_INT);


            FaaliyetAmaciDDL.Items.Clear();
            FaaliyetAmaciDDL.Items.Add(li5);
            FaaliyetAmaciDDL.Items.Add(li);
            FaaliyetAmaciDDL.Items.Add(li2);
            FaaliyetAmaciDDL.Items.Add(li3);
            FaaliyetAmaciDDL.Items.Add(li4);
            FaaliyetAmaciDDL.Items.Add(li6);
        }
        private void FaaliyetDurumuDDLDoldur()
        {
            ListItem li = new ListItem(ProjeConstants.FAALIYET_DURUMU_PLANLANDI, ProjeConstants.FAALIYET_DURUMU_PLANLANDI_INT.ToString());
            ListItem li2 = new ListItem(ProjeConstants.FAALIYET_DURUMU_ONAYLANDI, ProjeConstants.FAALIYET_DURUMU_ONAYLANDI_INT.ToString());
            ListItem li3 = new ListItem(ProjeConstants.FAALIYET_DURUMU_IPTALEDILDI, ProjeConstants.FAALIYET_DURUMU_IPTALEDILDI_INT.ToString());

            FaaliyetDurumuDDL.Items.Clear();
            FaaliyetDurumuDDL.Items.Add(li);
            FaaliyetDurumuDDL.Items.Add(li2);
            FaaliyetDurumuDDL.Items.Add(li3);
        }
        private void FaaliyetYeriDDLDoldur()
        {
            FaaliyetYeriDDL.Items.Clear();
            FaaliyetYeri faaliyetYeri = new FaaliyetYeri();
            List<FaaliyetYeri> list = faaliyetYeri.SelectAll<FaaliyetYeri>();
            foreach (var item in list)
            {
                ListItem li = new ListItem(item.Adi, item.Id.ToString());
                FaaliyetYeriDDL.Items.Add(li);
            }
        }
        private void StokluAniObjesiDDLDoldur()
        {
            StokluAniObjesiDDL.Items.Clear();
            AniObjesiTanim aniObjesiTanim = new AniObjesiTanim();
            DataTable dataTable = aniObjesiTanim.SelectStokluAniObjesiList();
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    int aniObjesiId = row["AniObjesiId"].ConvertToInt();
                    string aniObjesiAdi = row["Adi"].ReturnEmptyIfNull().ToString();
                    int toplam = row["Toplam"].ConvertToInt();
                    ListItem li = new ListItem(aniObjesiAdi.ToString() + " (" + toplam + ")", aniObjesiId.ToString());
                    StokluAniObjesiDDL.Items.Add(li);
                }

            }

        }
        private void DepoDDLDoldur()
        {
            DepoDDL.Items.Clear();
            int secilenAniObjesiId = StokluAniObjesiDDL != null && StokluAniObjesiDDL.SelectedItem != null ? StokluAniObjesiDDL.SelectedItem.Value.ConvertToInt() : 0;
            DepoTanim depoTanim = new DepoTanim();
            DataTable dataTable = depoTanim.SelectStokluAniObjesiList(secilenAniObjesiId);
            if (dataTable != null)
            {
                int i = 0;
                foreach (DataRow row in dataTable.Rows)
                {
                    i++;
                    int depoId = row["DepoId"].ConvertToInt();
                    string depoAdi = row["DepoAdi"].ReturnEmptyIfNull().ToString();
                    int adet = row["Adet"].ConvertToInt();
                    ListItem li = new ListItem(depoAdi.ToString() + " (" + adet + ")", depoId.ToString());
                    li.Attributes.Add("adet", adet.ToString());
                    DepoDDL.Items.Add(li);
                }
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
        private void IcIrtibatNoktasiDoldur(int personelId, bool buttonsEnabled)
        {
            Personel personel = new Personel();
            personel = personel.Select(personelId);
            if (personel != null)
            {
                IcIrtibatIdQS = personel.Id.ToString();
                IcIrtibatLbl.Text = "İç İrtibat Noktası : " + personel.Adi + " " + personel.Soyadi;
                IcIrtibatCikarBtn.Visible = buttonsEnabled;
            }
            else
            {
                IcIrtibatLbl.Text = string.Empty;
                IcIrtibatCikarBtn.Visible = false;
            }
        }
        private void DisIrtibatNoktasiDoldur(int kisiId, bool buttonsEnabled)
        {
            Kisi kisi = new Kisi();
            kisi = kisi.Select(kisiId);
            if (kisi != null)
            {
                DisIrtibatLbl.Text = "Dış İrtibat Noktası : " + kisi.Adi + " " + kisi.Soyadi;
                DisIrtibatIdQS = kisi.Id.ToString();
                DisIrtibatCikarBtn.Visible = buttonsEnabled;
            }
            else
            {
                DisIrtibatLbl.Text = string.Empty;
                DisIrtibatCikarBtn.Visible = false;
            }
        }
        protected void IrtibatSecBtn_Click(object sender, EventArgs e)
        {
            Faaliyet faaliyet = null;
            int katilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
            if (katilimciId > 0)
            {
                faaliyet = new Faaliyet();
                faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
                if (faaliyet != null)
                {

                    if (paramFaaliyetKatilimciTipiLbl.Value.ConvertToInt() == ProjeConstants.FAALIYET_KATILIMCI_IC_INT)
                    {
                        faaliyet.IcIrtibatId = katilimciId;
                    }
                    else
                    {
                        faaliyet.DisIrtibatId = katilimciId;
                    }

                    faaliyet.Update();
                }
            }

            if (faaliyet != null)
            {
                KatilimciBilgileriniDoldur(faaliyet, true);
            }
        }
        protected void FaaliyetDurumuDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            //FaaliyetDurumuImg.ImageUrl = FaaliyetDurumuImgGetir(FaaliyetDurumuDDL.SelectedItem.Value.ConvertToInt());
        }
        private void TabloOlustur(bool buttonsEnabled)
        {
            var jsonData = TabloJson(buttonsEnabled); //veri çekilip json a çeviriliyor
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

            jQuery('#CustomDataTable').DataTable({
            data: " + jsonData + @",
            columns: [
                { data: 'AdiSoyadi' },
                { data: 'Kurumu' },
                { data: 'KatilimciTipi', visible: false },
                { data: 'KatilimciTipiStr' },
                { data: 'AniObjesiStoklu' },
                { data: 'AniObjesiStoksuz' },
                { data: 'GetirilenAniObjesi' },
                { data: 'KisiKarti' },
                { data: 'Cikar' },
            ],
            'order': [[2, 'desc']],
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                'decimal': ',',
                'thousands': '.'
            },
            responsive: true,
            dom: 'rt',
            'lengthMenu': [[-1], ['All']],
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
        private string TabloJson(bool buttonsEnable)
        {
            string jSon = string.Empty;

            try
            {
                List<KatilimciListItem> list = GetDataList(buttonsEnable);
                if (buttonsEnable)
                {
                    KatilimciListQS = list.Select(a => a.KatilimciId + " " + a.AniObjesiStoklu + a.AniObjesiStoksuz).ToList();
                }

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
        private List<KatilimciListItem> GetDataList(bool buttonsEnable)
        {
            Faaliyet faaliyetDao = new Faaliyet();
            DataTable dataTable = faaliyetDao.SelectAllByKatilimciFaaliyetReturnDataTable(FaaliyetIdQS.ConvertToInt(), ProjeConstants.HEPSI_INT);
            int SiraNo = 1;
            List<KatilimciListItem> list = new List<KatilimciListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string katilimId = row["KatilimId"].ToString();
                    string katilimciId = row["KatilimciId"].ToString();
                    int katilimciTipi = row["KatilimciTipi"].ConvertToInt();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();
                    string kurumu = row["Kurumu"].ToString();

                    KatilimciListItem katilimciItem = new KatilimciListItem();
                    katilimciItem.Sirano = SiraNo++.ToString();
                    katilimciItem.KatilimciId = katilimciId;
                    katilimciItem.Adi = adi;
                    katilimciItem.Soyadi = soyadi;
                    katilimciItem.AdiSoyadi = (adi + " " + soyadi.Trim());
                    katilimciItem.Kurumu = kurumu;
                    katilimciItem.KatilimciTipi = katilimciTipi.ToString();
                    katilimciItem.KatilimciTipiStr = KatilimciTipiGetir(katilimciTipi);
                    //if (katilimciItem.KatilimciTipi.ConvertToInt() == ProjeConstants.RANDEVU_KATILIMCI_IC_INT)
                    //{
                    //    katilimciItem.AniObjesiStoklu = string.Empty;
                    //    katilimciItem.AniObjesiStoksuz = string.Empty;
                    //    katilimciItem.GetirilenAniObjesi = string.Empty;
                    //}
                    //else
                    {
                        string stoksuzAniObjeleri = AniObjeleriniGetir(katilimciItem.KatilimciId.ConvertToInt(),
                            katilimciItem.KatilimciTipi.ConvertToInt(), FaaliyetIdQS.ConvertToInt(), ProjeConstants.MTS_ANIOBJESISTOKSUZ);
                        string stokluAniObjeleri = AniObjeleriniGetir(katilimciItem.KatilimciId.ConvertToInt(),
                            katilimciItem.KatilimciTipi.ConvertToInt(), FaaliyetIdQS.ConvertToInt(), ProjeConstants.MTS_ANIOBJESISTOKLU);
                        string getirilenAniObjeleri = GetirilenAniObjeleriniGetir(katilimciItem.KatilimciId.ConvertToInt(),
                            katilimciItem.KatilimciTipi.ConvertToInt(), FaaliyetIdQS.ConvertToInt());

                        if (buttonsEnable)
                        {
                            katilimciItem.AniObjesiStoksuz = "<a href='#' class='btn btn-outline-primary' onclick=StoksuzAniObjesiBtnClick(" + katilimciId + "," + FaaliyetIdQS + "," + katilimciTipi + ")>Anı Objesi (Stoksuz)</a>" +
                                "<br>" + stoksuzAniObjeleri;
                            katilimciItem.AniObjesiStoklu = "<a href='#' class='btn btn-outline-success' onclick=StokluAniObjesiBtnClick(" + katilimciId + "," + FaaliyetIdQS + "," + katilimciTipi + ")>Anı Objesi (Stoklu)</a>" +
                                "<br>" + stokluAniObjeleri;
                            katilimciItem.GetirilenAniObjesi = "<a href='#' class='btn btn-outline-info' onclick=GetirilenAniObjesiBtnClick(" + katilimciId + "," + FaaliyetIdQS + "," + katilimciTipi + ")>Getirilen Anı Objesi </a>" +
                                "<br>" + getirilenAniObjeleri;
                        }
                        else
                        {
                            katilimciItem.AniObjesiStoksuz = stoksuzAniObjeleri;
                            katilimciItem.AniObjesiStoklu = stokluAniObjeleri;
                            katilimciItem.GetirilenAniObjesi = getirilenAniObjeleri;
                        }
                    }

                    katilimciItem.KisiKarti = "<a  target='_blank' href=" + ProjeConstants.PAGE_KISI_KARTI + "?KatilimciId=" + katilimciId + "&KatilimciTipi=" + katilimciTipi + " class='btn btn-outline-info'>Kişi Kartı</a>";
                    if (buttonsEnable)
                        katilimciItem.Cikar = "<a href='#' class='btn btn-outline-danger' onclick=KatilimciCikarBtnClick(" + katilimId + ")>Çıkar</a>";

                    list.Add(katilimciItem);
                }
            }
            return list;
        }
        private string AniObjeleriniGetir(int katilimciId, int katilimciTipi, int faaliyetId, string stokluMu)
        {
            AniObjesiDagitim aniobjesiDagitim = new AniObjesiDagitim();
            DataTable dataTable = aniobjesiDagitim.SelectByKatilimcidKatilimciTipiRandevuId(katilimciId, katilimciTipi, faaliyetId, stokluMu);
            string aniObjeleri = string.Empty;

            if (dataTable != null)
            {
                if (dataTable != null)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int aniObjesiDagitimId = row["AniObjesiDagitimId"].ReturnZeroIfNull().ConvertToInt();
                        int depoId = row["CikisDepoId"].ReturnZeroIfNull().ConvertToInt();
                        string aniObjesi = row["Adi"].ReturnEmptyIfNull().ToString();
                        string adet = row["Adet"].ReturnZeroIfNull().ToString();
                        if (stokluMu.Equals(ProjeConstants.MTS_ANIOBJESISTOKLU))
                        {
                            aniObjeleri += !string.IsNullOrEmpty(aniObjesi) ? "<br> * "
                                               + "<a href='#' class='btn btn-link' onclick=StokluAniObjesiDuzenleClick(" + aniObjesiDagitimId + ")>" + aniObjesi + " (" + adet + "), " + "</a>"
                                               : string.Empty;
                        }
                        else
                        {
                            aniObjeleri += !string.IsNullOrEmpty(aniObjesi) ? "<br> * " + aniObjesi + "(" + adet + "), " : string.Empty;
                        }
                        //aniObjeleri += !string.IsNullOrEmpty(aniObjesi) ? "<br> * " + aniObjesi + "(" + adet + "), " : string.Empty;
                    }
                }
            }
            return (aniObjeleri);
        }
        private string GetirilenAniObjeleriniGetir(int katilimciId, int katilimciTipi, int faaliyetId)
        {
            AniObjesiDagitim aniobjesiDagitim = new AniObjesiDagitim();
            string aniObjeleri = aniobjesiDagitim.SelectGetirilenByKatilimcidKatilimciTipiRandevuId(katilimciId, katilimciTipi, faaliyetId);
            return (aniObjeleri);
        }
        private void TabloModalOlustur()
        {
            var jsonData = TabloModalJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
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
                { data: 'KatilimciSec' },
                { data: 'IrtibatSec' }
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
            DataTable dataTableIc = personel.SelectSecilmemisIcKatilimcilarByFaaliyetIdReturnDT(FaaliyetIdQS.ConvertToInt());
            Kisi kisi = new Kisi();
            DataTable dataTableDis = kisi.SelectSecilmemisDisKatilimcilarByFaaliyetIdReturnDT(FaaliyetIdQS.ConvertToInt());
            NakitBagisci nakitBagisci = new NakitBagisci();
            DataTable dataTableNakit = nakitBagisci.SelectSecilmemisKatilimcilarByFaaliyetIdReturnDT(FaaliyetIdQS.ConvertToInt());
            TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
            DataTable dataTableTasinmaz = tasinmazBagisci.SelectSecilmemisKatilimcilarByFaaliyetIdReturnDT(FaaliyetIdQS.ConvertToInt()); //veri çekilip json a çeviriliyor

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


                    katilimciItem.KatilimciSec = "<a href='#' class='btn btn-outline-primary' onclick=KatilimciSecildiBtnClick(" + katilimciId + "," + FaaliyetIdQS + "," + katilimciTipi + ")>Faaliyete Ekle</a>";
                    if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_IC_INT ||
                        katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_DIS_INT)
                    {
                        katilimciItem.IrtibatSec = "<a href='#' class='btn btn-outline-primary' onclick=IrtibatSecBtnClick(" + katilimciId + "," + FaaliyetIdQS + "," + katilimciTipi + ")>İrtibat Ekle</a>";
                    }
                    else
                    {
                        katilimciItem.IrtibatSec = string.Empty;
                    }


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
        protected void IcIrtibatCikarBtn_Click(object sender, EventArgs e)
        {
            Faaliyet faaliyet = new Faaliyet();
            faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
            if (faaliyet != null)
            {
                faaliyet.IcIrtibatId = 0;
                faaliyet.Update();
                IcIrtibatLbl.Text = string.Empty;
                IcIrtibatCikarBtn.Visible = false;
                KatilimciBilgileriniDoldur(faaliyet, true);
            }
        }
        protected void DisIrtibatCikarBtn_Click(object sender, EventArgs e)
        {
            Faaliyet faaliyet = new Faaliyet();
            faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
            if (faaliyet != null)
            {
                faaliyet.DisIrtibatId = 0;
                faaliyet.Update();
                DisIrtibatLbl.Text = string.Empty;
                DisIrtibatCikarBtn.Visible = false;
                KatilimciBilgileriniDoldur(faaliyet, true);
            }
        }
        #region Arama/Gorusme bilgileri
        private void AramaGorusmeBilgileriniDoldur(Faaliyet faaliyet)
        {
            AramaGorusme aramaGorusme = new AramaGorusme();
            aramaGorusme = aramaGorusme.SelectByRandevuId(faaliyet.Id);
            if (aramaGorusme != null)
            {
                AramaGorusmeIdQS = aramaGorusme.Id.ToString();
                AramaGorusmeLbl.Text = "Arama/Görüşme No:" + aramaGorusme.Id;

                AramaGorusmeDiv.Attributes["style"] = "display:block";
            }
            else
            {
                AramaGorusmeDiv.Attributes["style"] = "display:none";
                AramaGorusmeIdQS = "0";
            }

        }
        protected void AramaGorusmeBtn_Click(object sender, EventArgs e)
        {
            if (AramaGorusmeIdQS.ConvertToInt() > 0)
            {
                RedirectToPage(ProjeConstants.PAGE_ARAMAGORUSME_GIRIS + "?AramaGorusmeId=" + AramaGorusmeIdQS);
            }
            else
            {
                MessageHelper.PublishMessage("Arama Görüşme bağlantısı bulunamadı.", ProjeConstants.MESAJ_HATA);
            }
        }
        #endregion
        #region Ani Objesi
        protected void AniObjesiSecBtn_Click(object sender, EventArgs e)
        {
            int katilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
            int faaliyetId = paramFaaliyetIdLbl.Value.ConvertToInt();
            int katilimciTipi = paramFaaliyetKatilimciTipiLbl.Value.ConvertToInt();
            if (katilimciId > 0)
            {
                if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_DIS_INT)
                {
                    Kisi kisi = new Kisi();
                    kisi = kisi.Select(katilimciId);
                    if (kisi != null)
                    {
                        AniObjesiHeaderLbl.InnerText = "Anı Objesi Seçimi (" + kisi.Adi + " " + kisi.Soyadi + ")";
                        AniObjeleriTablosunuDoldur(faaliyetId, katilimciId, katilimciTipi);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Kişi bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                }
                else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI_INT)
                {
                    NakitBagisci nakitBagisci = new NakitBagisci();
                    nakitBagisci = nakitBagisci.Select<NakitBagisci>(katilimciId);
                    if (nakitBagisci != null)
                    {
                        AniObjesiHeaderLbl.InnerText = "Anı Objesi Seçimi (" + nakitBagisci.Adi + " " + nakitBagisci.Soyadi + ")";
                        AniObjeleriTablosunuDoldur(faaliyetId, katilimciId, katilimciTipi);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Nakit Bağışçı bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                }
                else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI_INT)
                {
                    TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
                    tasinmazBagisci = tasinmazBagisci.Select<TasinmazBagisci>(katilimciId);
                    if (tasinmazBagisci != null)
                    {
                        AniObjesiHeaderLbl.InnerText = "Anı Objesi Seçimi (" + tasinmazBagisci.Adi + " " + tasinmazBagisci.Soyadi + ")";
                        AniObjeleriTablosunuDoldur(faaliyetId, katilimciId, katilimciTipi);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Taşınmaz Bağışçı bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                }
                else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_IC_INT)
                {
                    Personel personel = new Personel();
                    personel = personel.Select(katilimciId);
                    if (personel != null)
                    {
                        AniObjesiHeaderLbl.InnerText = "Anı Objesi Seçimi (" + personel.Adi + " " + personel.Soyadi + ")";
                        AniObjeleriTablosunuDoldur(faaliyetId, katilimciId, katilimciTipi);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Personel bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                }
            }
            else
            {
                MessageHelper.PublishMessage("Katılımcı Bulunamadı", ProjeConstants.MESAJ_HATA);
            }
        }
        protected void StokluAniObjesiSecBtn_Click(object sender, EventArgs e)
        {
            int aniObjesiDagitimId = paramStokluAniObjesiDagitimIdLbl.Value.ConvertToInt();

            if (aniObjesiDagitimId > 0) //düzenle
            {
                bool isOk;
                AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
                aniObjesiDagitim = aniObjesiDagitim.Select(aniObjesiDagitimId);
                if (aniObjesiDagitim != null)
                {
                    IadeEdilecekAdetTxt.Text = aniObjesiDagitim.Adet.ToString();
                    AniObjesiTanim aniObjesiTanim = new AniObjesiTanim();
                    aniObjesiTanim = aniObjesiTanim.Select(aniObjesiDagitim.AniObjesiId);
                    if (aniObjesiTanim != null)
                    {
                        IadeEdilecekAniObjesiTxt.Text = aniObjesiTanim.Adi;// + " (" + stokAdet +")";
                        isOk = true;
                    }
                    else
                    {
                        isOk = false;
                        MessageHelper.PublishMessage("Anı Objesi Tanımı bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                    DepoTanim depoTanim = new DepoTanim();
                    depoTanim = depoTanim.Select(aniObjesiDagitim.CikisDepoId);
                    if (depoTanim != null)
                    {
                        IadeEdilecekDepoTxt.Text = depoTanim.Adi;
                        isOk = true;
                    }
                    else
                    {
                        isOk = false;
                        MessageHelper.PublishMessage("Stoklu Depo Tanımı bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                    DagitimDiv.Attributes["style"] = "display:none";
                    IadeDiv.Attributes["style"] = "display:block";

                    if (isOk)
                        UtilityHelper.ScriptCalistir("StokluAniObjesiModal();");
                }
                else
                {
                    MessageHelper.PublishMessage("Anı Objesi dağıtımı bulunamadı", ProjeConstants.MESAJ_HATA);
                }
            }
            else // ekle
            {
                DagitimDiv.Attributes["style"] = "display:block";
                IadeDiv.Attributes["style"] = "display:none";
                int katilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
                int katilimciTipi = paramFaaliyetKatilimciTipiLbl.Value.ConvertToInt();
                if (katilimciId > 0)
                {
                    if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_DIS_INT)
                    {
                        Kisi kisi = new Kisi();
                        kisi = kisi.Select(katilimciId);
                        if (kisi != null)
                        {

                            StokluAniObjesiModalTitle.InnerText = "Stoklu Anı Objesi Seçimi (" + kisi.Adi + " " + kisi.Soyadi + ")";
                            StokluAniObjesiDDLDoldur();
                            DepoDDLDoldur();
                            //StokluAniObjeleriTablosunuDoldur(faaliyetId, katilimciId, katilimciTipi);
                            UtilityHelper.ScriptCalistir("StokluAniObjesiModal();");
                        }
                        else
                        {
                            MessageHelper.PublishMessage("Kişi bulunamadı", ProjeConstants.MESAJ_HATA);
                        }
                    }
                    else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI_INT)
                    {
                        NakitBagisci nakitBagisci = new NakitBagisci();
                        nakitBagisci = nakitBagisci.Select<NakitBagisci>(katilimciId);
                        if (nakitBagisci != null)
                        {
                            StokluAniObjesiModalTitle.InnerText = "Stoklu Anı Objesi Seçimi (" + nakitBagisci.Adi + " " + nakitBagisci.Soyadi + ")";
                            StokluAniObjesiDDLDoldur();
                            DepoDDLDoldur();
                            //AniObjeleriTablosunuDoldur(faaliyetId, katilimciId, katilimciTipi);
                            UtilityHelper.ScriptCalistir("StokluAniObjesiModal();");
                        }
                        else
                        {
                            MessageHelper.PublishMessage("Nakit Bağışçı bulunamadı", ProjeConstants.MESAJ_HATA);
                        }
                    }
                    else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI_INT)
                    {
                        TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
                        tasinmazBagisci = tasinmazBagisci.Select<TasinmazBagisci>(katilimciId);
                        if (tasinmazBagisci != null)
                        {
                            StokluAniObjesiModalTitle.InnerText = "Stoklu Anı Objesi Seçimi (" + tasinmazBagisci.Adi + " " + tasinmazBagisci.Soyadi + ")";
                            StokluAniObjesiDDLDoldur();
                            DepoDDLDoldur();
                            //AniObjeleriTablosunuDoldur(faaliyetId, katilimciId, katilimciTipi);
                            UtilityHelper.ScriptCalistir("StokluAniObjesiModal();");
                        }
                        else
                        {
                            MessageHelper.PublishMessage("Taşınmaz Bağışçı bulunamadı", ProjeConstants.MESAJ_HATA);
                        }
                    }
                    else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_IC_INT)
                    {
                        Personel personel = new Personel();
                        personel = personel.Select(katilimciId);
                        if (personel != null)
                        {
                            StokluAniObjesiModalTitle.InnerText = "Stoklu Anı Objesi Seçimi (" + personel.Adi + " " + personel.Soyadi + ")";
                            StokluAniObjesiDDLDoldur();
                            DepoDDLDoldur();
                            //AniObjeleriTablosunuDoldur(faaliyetId, katilimciId, katilimciTipi);
                            UtilityHelper.ScriptCalistir("StokluAniObjesiModal();");
                        }
                        else
                        {
                            MessageHelper.PublishMessage("Personel bulunamadı", ProjeConstants.MESAJ_HATA);
                        }
                    }

                }
                else
                {
                    MessageHelper.PublishMessage("Katılımcı Bulunamadı", ProjeConstants.MESAJ_HATA);
                }
            }
        }
        protected void GetirilenAniObjesiSecBtn_Click(object sender, EventArgs e)
        {
            int katilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
            int faaliyetId = paramFaaliyetIdLbl.Value.ConvertToInt();
            int katilimciTipi = paramFaaliyetKatilimciTipiLbl.Value.ConvertToInt();
            if (katilimciId > 0)
            {
                if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_DIS_INT)
                {
                    Kisi kisi = new Kisi();
                    kisi = kisi.Select(katilimciId);
                    if (kisi != null)
                    {
                        GetirilenAniObjesiModalTitle.InnerText = "Getirilen Anı Objesi (" + kisi.Adi + " " + kisi.Soyadi + ")";
                        GetirilenAniObjesiniDoldur(faaliyetId, katilimciId, katilimciTipi);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Kişi bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                }
                else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_NAKITBAGISCI_INT)
                {
                    NakitBagisci nakitBagisci = new NakitBagisci();
                    nakitBagisci = nakitBagisci.Select<NakitBagisci>(katilimciId);
                    if (nakitBagisci != null)
                    {
                        GetirilenAniObjesiModalTitle.InnerText = "Getirilen Anı Objesi (" + nakitBagisci.Adi + " " + nakitBagisci.Soyadi + ")";
                        GetirilenAniObjesiniDoldur(faaliyetId, katilimciId, katilimciTipi);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Nakit Bağışçı bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                }
                else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_TASINMAZBAGISCI_INT)
                {
                    TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
                    tasinmazBagisci = tasinmazBagisci.Select<TasinmazBagisci>(katilimciId);
                    if (tasinmazBagisci != null)
                    {
                        GetirilenAniObjesiModalTitle.InnerText = "Getirilen Anı Objesi (" + tasinmazBagisci.Adi + " " + tasinmazBagisci.Soyadi + ")";
                        GetirilenAniObjesiniDoldur(faaliyetId, katilimciId, katilimciTipi);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Personel bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                }
                else if (katilimciTipi == ProjeConstants.FAALIYET_KATILIMCI_IC_INT)
                {
                    Personel personel = new Personel();
                    personel = personel.Select(katilimciId);
                    if (personel != null)
                    {
                        GetirilenAniObjesiModalTitle.InnerText = "Getirilen Anı Objesi (" + personel.Adi + " " + personel.Soyadi + ")";
                        GetirilenAniObjesiniDoldur(faaliyetId, katilimciId, katilimciTipi);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Personel bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                }
            }
            else
            {
                MessageHelper.PublishMessage("Katılımcı Bulunamadı", ProjeConstants.MESAJ_HATA);
            }
        }
        public void GetirilenAniObjesiniDoldur(int faaliyetId, int katilimciId, int katilimciTipi)
        {
            paramFaaliyetIdLbl.Value = faaliyetId.ToString();
            paramFaaliyetKatilimciIdLbl.Value = katilimciId.ToString();
            paramFaaliyetKatilimciTipiLbl.Value = katilimciTipi.ToString();

            AniObjesiDagitim getirilenAniObjesi = new AniObjesiDagitim();
            getirilenAniObjesi = getirilenAniObjesi.SelectGetirilenAniObjesi(faaliyetId, katilimciId, katilimciTipi);
            if (getirilenAniObjesi != null)
            {
                GetirilenAniObjesiTxt.Text = getirilenAniObjesi.GetirilenAniObjesi;
            }
            UtilityHelper.ScriptCalistir("GetirilenAniObjesiModal();");
        }
        public JavaScriptSerializer javaSerial = new JavaScriptSerializer();
        /// <summary>
        /// Anı Objelerini modal pencereye getirir
        /// Seçilen katılımcı ve faaliyetya göre AniObjesiDagitim_Table ile FaaliyetYeri_Table join edilerek sorgulanır,
        /// Daha önceden işaretlenmiş olanlar frontend'de javascript arrayde tutulur ve ekrana dolu olarak gelir.
        /// Checkbox check değeri değiştiğinde,  sadece frontend deki array'e eklenir veya çıkarılır.
        /// Kaydete basıldığında array paramArray'e aktarılır, kaydetNowBtn click çalışır, devamı btn_click içinde yapılır
        /// </summary>
        /// <param name="faaliyetId"></param>
        /// <param name="katilimciId"></param>
        /// <param name="katilimciTipi"></param>
        private void AniObjeleriTablosunuDoldur(int faaliyetId, int katilimciId, int katilimciTipi)
        {
            paramFaaliyetIdLbl.Value = faaliyetId.ToString();
            paramFaaliyetKatilimciIdLbl.Value = katilimciId.ToString();
            paramFaaliyetKatilimciTipiLbl.Value = katilimciTipi.ToString();

            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            DataTable dataTable = aniObjesiDagitim.SelectReturnDT(faaliyetId, katilimciId, katilimciTipi, ProjeConstants.MTS_ANIOBJESISTOKSUZ);
            if (dataTable != null)
            {
                //CheckBox[] aniObjesiChk = new CheckBox[AniObjesiList.Count];
                int counter = 0;
                bool artti = false;

                string aniObjesiIdStr = string.Empty;
                string aniObjesiAdetStr = string.Empty;
                while (counter < dataTable.Rows.Count)
                {
                    DataRow row = dataTable.Rows[counter];

                    TableRow tableRow = new TableRow();
                    for (int i = 0; i < 4; i++)
                    {
                        row = dataTable.Rows[counter];

                        int aniObjesiId = row["AniObjesiId"].ReturnZeroIfNull().ConvertToInt();
                        int sira = row["Sira"].ReturnZeroIfNull().ConvertToInt();
                        string deger = row["Adi"].ToString();
                        int aniObjesiDagitimId = row["AniObjesiDagitimId"].ReturnZeroIfNull().ConvertToInt();
                        int adet = row["Adet"].ReturnZeroIfNull().ConvertToInt();

                        TableCell degerCell = new TableCell();

                        HtmlGenericControl newdiv = new HtmlGenericControl("DIV");
                        newdiv.ID = "newDiv" + aniObjesiId;

                        HtmlGenericControl label = new HtmlGenericControl("label");
                        newdiv.Controls.Add(label);

                        CheckBox chkBox = new CheckBox();
                        chkBox.ID = "chkBox" + aniObjesiId;
                        chkBox.Text = deger;
                        chkBox.Checked = aniObjesiDagitimId > 0;
                        if (chkBox.Checked)
                        {
                            aniObjesiIdStr += "," + aniObjesiId.ToString();
                            aniObjesiAdetStr += "," + adet.ToString();
                            newdiv.Attributes.Add("class", "checkbox font-weight-bold text-danger");
                        }
                        else
                        {
                            newdiv.Attributes.Add("class", "checkbox");
                        }

                        label.Controls.Add(chkBox);
                        degerCell.Controls.Add(newdiv);

                        TableCell adetCell = new TableCell();
                        TextBox adetTxt = new TextBox();
                        adetTxt.CssClass = "form-control input-integer";
                        adetTxt.ID = "adet" + aniObjesiId;
                        adetTxt.Width = 50;
                        adetTxt.Attributes.Add("type", "number");
                        adetTxt.Attributes.Add("step", "1");
                        adetTxt.Text = adet.ToString();

                        adetTxt.Enabled = chkBox.Checked;
                        adetTxt.Attributes.Add("onchange", "ChangeAniObjesiAdet(" + aniObjesiId + "," + chkBox.ID.ReturnQuotedValue() + "," + adetTxt.Text + ",this," + newdiv.ID.ReturnQuotedValue() + ");");

                        adetCell.Controls.Add(adetTxt);

                        chkBox.Attributes.Add("onclick", "AddRemoveAniObjesiIdToList(" + aniObjesiId + ",this," + newdiv.ID.ReturnQuotedValue() + "," + adetTxt.Text + "," + adetTxt.ID.ReturnQuotedValue() + ");");

                        tableRow.Controls.Add(degerCell);
                        tableRow.Controls.Add(adetCell);
                        if (dataTable.Rows.Count > counter + 1)
                        {
                            counter++;
                            artti = true;
                        }
                        else
                        {
                            artti = false;
                            break;
                        }
                    }
                    CustomAniObjesiModalDataTable.Rows.Add(tableRow);
                    if (artti)
                    {

                        artti = false;
                    }
                    else
                    {
                        counter++;
                    }

                }
                paramAniObjesiIdArray.Value = aniObjesiIdStr.Length > 0 ? aniObjesiIdStr.Substring(1, aniObjesiIdStr.Length - 1) : string.Empty;
                paramAniObjesiAdetArray.Value = aniObjesiAdetStr.Length > 0 ? aniObjesiAdetStr.Substring(1, aniObjesiAdetStr.Length - 1) : string.Empty;
            }
            else
            {
                //paramAniObjesiIdArray.Value = string.Empty;
                //paramAniObjesiAdetArray.Value = string.Empty;
            }

            UtilityHelper.ScriptCalistir("ArrayDoldur();");
            UtilityHelper.ScriptCalistir("AniObjesiModal();");
        }
        #endregion
        protected void FaaliyetKartiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_KARTI + "?FaaliyetId=" + FaaliyetIdQS);
        }
        /// <summary>
        /// ilk olarak bu Faaliyet ve KatilimciId için AniObjesiDagitim_Table'daki kayıtları al (list1)
        /// sonra
        ///     id ve adet arraylerini al (list2)
        ///     list1de olup da list2de olmayan kayıtları sil
        ///     list1de ve list2de olan kayıtları list2 adetinde göre update et
        ///     list1de olmayıp lis2de olanları insert et
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SecilenleriKaydetBtn_Click(object sender, EventArgs e)
        {
            int faaliyetId = paramFaaliyetIdLbl.Value.ConvertToInt();
            int katilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
            int katilimciTipi = paramFaaliyetKatilimciTipiLbl.Value.ConvertToInt();

            string idler = paramAniObjesiIdArray.Value;
            string adetler = paramAniObjesiAdetArray.Value;
            List<string> idList = idler.Split(',').ToList<string>();
            List<string> adetList = adetler.Split(',').ToList<string>();

            KaldirilanlariSil(idList, faaliyetId, katilimciId, katilimciTipi);

            AniObjesiDagitim aniObjesiDagitimDao = new AniObjesiDagitim();
            List<AniObjesiDagitim> objeList = aniObjesiDagitimDao.SelectStoksuzAniObjeleriReturnList(faaliyetId, katilimciId, katilimciTipi);
            foreach (var item in objeList)
            {
                int index = idList.FindIndex(a => a == item.AniObjesiId.ToString());
                if (index > -1)
                {
                    int adet = adetList[index].ConvertToInt();

                    if (index < 0) // ikinci listede yok, silinecek
                    {
                        item.Delete();
                    }
                    else
                    {

                        if (adet == 0)//yeni adet sıfırsa sil
                        {

                            item.Delete();
                        }
                        else if (adet == item.Adet)//adet aynı bişey yapma
                        {

                        }
                        else if (item.Adet == 0)//adet farklı //eski adet sıfırsa insert et
                        {

                            item.Adet = adet;
                            item.RandevuId = faaliyetId;
                            item.KatilimciId = katilimciId;
                            item.KatilimciTipi = katilimciTipi;
                            item.VerilisTarihi = BaslangicTarihiTxt.Text.ConvertToDatetime();
                            item.Olusturan = UtilityHelper.GetCurrentUserName();
                            item.Save();
                        }
                        else //eski adetle adet farklı  update et
                        {
                            item.Adet = adet;
                            item.RandevuId = faaliyetId;
                            item.KatilimciId = katilimciId;
                            item.KatilimciTipi = katilimciTipi;
                            item.VerilisTarihi = BaslangicTarihiTxt.Text.ConvertToDatetime();
                            item.Degistiren = UtilityHelper.GetCurrentUserName();
                            item.Update();
                        }
                    }
                }
            }
            if (objeList.Count < 1)
            {
                MessageHelper.PublishMessage("Hiç kayıt seçilmedi. Devam etmek için en az bir kayıt seçiniz.", ProjeConstants.MESAJ_BILGI);
            }
            Faaliyet faaliyet = new Faaliyet();
            faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
            if (faaliyet != null)
            {
                KatilimciBilgileriniDoldur(faaliyet, true);
            }

        }
        protected void GetirilenAniObjesiKaydetBtn_Click(object sender, EventArgs e)
        {
            int faaliyetId = paramFaaliyetIdLbl.Value.ConvertToInt();
            int katilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
            int katilimciTipi = paramFaaliyetKatilimciTipiLbl.Value.ConvertToInt();

            GetirilenAniObjesiKaydet(faaliyetId, katilimciId, katilimciTipi);

            Faaliyet faaliyet = new Faaliyet();
            faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
            if (faaliyet != null)
            {
                KatilimciBilgileriniDoldur(faaliyet, true);
            }
            CloseModals();
        }
        protected void StokluAniObjesiKaydetBtn_Click(object sender, EventArgs e)
        {
            int aniObjesiId = StokluAniObjesiDDL.SelectedItem.Value.ConvertToInt();
            int depoId = DepoDDL.SelectedItem.Value.ConvertToInt();
            int adet = StokluAdetTxt.Text.ConvertToInt();
            AniObjesiTanim aniObjesiTanim = new AniObjesiTanim();
            aniObjesiTanim = aniObjesiTanim.Select<AniObjesiTanim>(aniObjesiId);

            if (aniObjesiTanim == null) // ani objesi var mı, yoksa
            {
                MessageHelper.PublishMessage("Anı Objesi Bulunamadı. Devam etmek için farklı bir seçim yapınız.", ProjeConstants.MESAJ_BILGI);
            }
            else //varsa
            {
                //TODO burda kaydet

                //seçilen depoda adet kadar mevcut var mı?
                DepoStok depoStok = new DepoStok();
                depoStok = depoStok.SelectByDepoIdAniObjesiId(depoId, aniObjesiId, ProjeConstants.MTS_ANIOBJESISTOKLU);
                if (depoStok != null)
                {
                    if (depoStok.SonAdet >= adet)
                    {
                        AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
                        int katilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
                        int katilimciTipi = paramFaaliyetKatilimciTipiLbl.Value.ConvertToInt();
                        int faaliyetId = paramFaaliyetIdLbl.Value.ConvertToInt();
                        aniObjesiDagitim = aniObjesiDagitim.Select(faaliyetId, katilimciId, katilimciTipi, aniObjesiId);
                        if (aniObjesiDagitim == null)
                        {
                            aniObjesiDagitim = new AniObjesiDagitim();
                            aniObjesiDagitim.Aciklama = adet + " adet verildi FaaliyetID=" + aniObjesiDagitim.RandevuId;
                            aniObjesiDagitim.Adet = StokluAdetTxt.Text.ConvertToInt(); //ilk defa verildi
                            aniObjesiDagitim.AniObjesiId = aniObjesiId;
                            aniObjesiDagitim.CikisDepoId = depoId;
                            aniObjesiDagitim.KatilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
                            aniObjesiDagitim.KatilimciTipi = paramFaaliyetKatilimciTipiLbl.Value.ConvertToInt();
                            aniObjesiDagitim.RandevuId = paramFaaliyetIdLbl.Value.ConvertToInt();
                            aniObjesiDagitim.VerilisTarihi = BaslangicTarihiTxt.Text.ConvertToDatetime();
                            aniObjesiDagitim.VerilenAlinan = ProjeConstants.ANIOBJESI_VERILEN_INT;
                            aniObjesiDagitim.Save();
                            //DepoStoktan düş

                            depoStok.SonAdet -= adet;
                            //depoStok.Aciklama+=adet+" adet verildi FaaliyetID=" + aniObjesiDagitim.FaaliyetId;
                            depoStok.SonIslemYapan = UtilityHelper.GetCurrentUserName();
                            depoStok.Update();
                        }
                        else
                        {
                            aniObjesiDagitim.Aciklama = adet + " adet verildi FaaliyetID=" + aniObjesiDagitim.RandevuId;
                            aniObjesiDagitim.Adet += StokluAdetTxt.Text.ConvertToInt(); //adet artırıldı
                            aniObjesiDagitim.AniObjesiId = aniObjesiId;
                            aniObjesiDagitim.CikisDepoId = depoId;
                            aniObjesiDagitim.KatilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
                            aniObjesiDagitim.KatilimciTipi = paramFaaliyetKatilimciTipiLbl.Value.ConvertToInt();
                            aniObjesiDagitim.RandevuId = paramFaaliyetIdLbl.Value.ConvertToInt();
                            aniObjesiDagitim.VerilisTarihi = BaslangicTarihiTxt.Text.ConvertToDatetime();
                            aniObjesiDagitim.VerilenAlinan = ProjeConstants.ANIOBJESI_VERILEN_INT;
                            aniObjesiDagitim.Update();
                            //DepoStoktan düş

                            depoStok.SonAdet -= adet;
                            depoStok.Aciklama = adet + " adet verildi FaaliyetID=" + aniObjesiDagitim.RandevuId;
                            depoStok.SonIslemYapan = UtilityHelper.GetCurrentUserName();
                            depoStok.Update();
                        }
                    }
                    else if (depoStok.SonAdet == 0)
                    {
                        MessageHelper.PublishMessage("Depoda ürün mevcudu bulunmuyor", ProjeConstants.MESAJ_HATA);
                    }
                    else if (depoStok.SonAdet < adet)
                    {
                        MessageHelper.PublishMessage("Depoda adedi yetersiz.", ProjeConstants.MESAJ_HATA);
                    }
                }
                else
                {
                    MessageHelper.PublishMessage("Depoda ürün bulunmuyor", ProjeConstants.MESAJ_HATA);
                }
            }
            Faaliyet faaliyet = new Faaliyet();
            faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
            if (faaliyet != null)
            {
                KatilimciBilgileriniDoldur(faaliyet, true);
            }
            CloseModals();

        }
        protected void StokluAniObjesiIadeEtBtn_Click(object sender, EventArgs e)
        {
            int aniObjesiDagitimId = paramStokluAniObjesiDagitimIdLbl.Value.ConvertToInt();
            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            aniObjesiDagitim = aniObjesiDagitim.Select(aniObjesiDagitimId);
            if (aniObjesiDagitim != null)
            {

                int aniObjesiId = aniObjesiDagitim.AniObjesiId;
                AniObjesiTanim aniObjesiTanim = new AniObjesiTanim();
                aniObjesiTanim = aniObjesiTanim.Select<AniObjesiTanim>(aniObjesiId);

                if (aniObjesiTanim == null) // ani objesi var mı, yoksa
                {
                    MessageHelper.PublishMessage("Anı Objesi Bulunamadı. Devam etmek için farklı bir seçim yapınız.", ProjeConstants.MESAJ_BILGI);
                }
                else //varsa
                {
                    //burda iade et
                    //aniObjesiDagitimdan Düş
                    int adet = IadeEdilecekAdetTxt.Text.ConvertToInt();

                    if (aniObjesiDagitim == null)
                    {
                        MessageHelper.PublishMessage("İade edilemedi, Anı Objesi bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                    else if (adet > aniObjesiDagitim.Adet) // ani objesi var mı, yoksa
                    {


                        MessageHelper.PublishMessage("İade edilecek miktarda Anı Objesi bulunmuyor. ", ProjeConstants.MESAJ_HATA);
                    }
                    else
                    {
                        int depoId = aniObjesiDagitim.CikisDepoId.ConvertToInt();
                        aniObjesiDagitim.Aciklama = adet + " adet İade edildi FaaliyetID=" + aniObjesiDagitim.RandevuId;
                        aniObjesiDagitim.Adet -= adet; //adet artırıldı
                        aniObjesiDagitim.AniObjesiId = aniObjesiId;


                        //DepoStok a ekle
                        DepoStok depoStok = new DepoStok();
                        depoStok = depoStok.SelectByDepoIdAniObjesiId(depoId, aniObjesiId, ProjeConstants.MTS_ANIOBJESISTOKLU);
                        if (depoStok == null)
                        {
                            depoStok = new DepoStok();
                            depoStok.DepoId = depoId;
                            depoStok.AniObjesiId = aniObjesiId;
                            depoStok.SonAdet = 0;
                            depoStok.SonIslemTarihi = DateTime.Now;
                            depoStok.SonIslemYapan = UtilityHelper.GetCurrentUserName();
                        }

                        depoStok.SonAdet += adet;
                        depoStok.Aciklama = adet + " adet verildi FaaliyetID=" + aniObjesiDagitim.RandevuId;
                        depoStok.SonIslemYapan = UtilityHelper.GetCurrentUserName();
                        depoStok.Update();
                        if (aniObjesiDagitim.Adet>0)
                        {
                            aniObjesiDagitim.Update();
                        }
                        else
                        {
                            aniObjesiDagitim.Delete();
                        }
                    }


                }
                Faaliyet faaliyet = new Faaliyet();
                faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
                if (faaliyet != null)
                {
                    KatilimciBilgileriniDoldur(faaliyet, true);
                }
            }
            else
            {
                MessageHelper.PublishMessage("Anı Objesi dağıtımı bulunamadı.", ProjeConstants.MESAJ_HATA);
            }
            CloseModals();

        }
        protected void StokluAniObjesiDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            DepoDDLDoldur();
            string adet = DepoDDL.SelectedItem.Attributes["adet"].ReturnZeroIfNull().ToString();
            //StokluAdetTxtRV.MaximumValue = adet.ConvertToInt()<1?"0":adet;
            //StokluAdetTxtRV.MinimumValue = "1";
            //StokluAdetTxt.Attributes.Remove("max");
            //StokluAdetTxt.Attributes.Add("max", adet);
            //StokluAdetTxt.Attributes.Add("min", "1");
        }
        protected void DepoDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            string adet = DepoDDL.SelectedItem.Attributes["adet"].ReturnZeroIfNull().ToString();
            //StokluAdetTxtRV.MaximumValue = adet.ConvertToInt() < 1 ? "0" : adet;
            //StokluAdetTxtRV.MinimumValue = "1";
            //StokluAdetTxt.Attributes.Remove("max");
            //StokluAdetTxt.Attributes.Add("max", adet);
            //StokluAdetTxt.Attributes.Add("min", "1");
        }
        private void GetirilenAniObjesiKaydet(int faaliyetId, int katilimciId, int katilimciTipi)
        {
            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            aniObjesiDagitim = aniObjesiDagitim.SelectGetirilenAniObjesi(faaliyetId, katilimciId, katilimciTipi);

            if (aniObjesiDagitim == null)
            {
                aniObjesiDagitim = new AniObjesiDagitim();
                aniObjesiDagitim.AniObjesiId = ProjeConstants.GETIRILEN_ANIOBJESIID_INT;
                aniObjesiDagitim.RandevuId = faaliyetId;
                aniObjesiDagitim.KatilimciId = katilimciId;
                aniObjesiDagitim.KatilimciTipi = katilimciTipi;

                aniObjesiDagitim.GetirilenAniObjesi = GetirilenAniObjesiTxt.Text;
                aniObjesiDagitim.VerilenAlinan = ProjeConstants.ANIOBJESI_GETIRILEN_INT;
                aniObjesiDagitim.VerilisTarihi = BaslangicTarihiTxt.Text.ConvertToDatetime();
                aniObjesiDagitim.Olusturan = UtilityHelper.GetCurrentUserName();
                aniObjesiDagitim.Save();

            }
            else if (aniObjesiDagitim != null)
            {
                aniObjesiDagitim.GetirilenAniObjesi = GetirilenAniObjesiTxt.Text;
                aniObjesiDagitim.Degistiren = UtilityHelper.GetCurrentUserName();
                aniObjesiDagitim.Update();
            }

        }
        private void KaldirilanlariSil(List<string> idList, int faaliyetId, int katilimciId, int katilimciTipi)
        {
            AniObjesiDagitim aniObjesiDagitimDao = new AniObjesiDagitim();
            List<AniObjesiDagitim> objeList = aniObjesiDagitimDao.SelectStoksuzAniObjeleriReturnList(faaliyetId, katilimciId, katilimciTipi);

            List<string> idList1 = objeList.Select(l => l.AniObjesiId.ToString()).ToList();
            var firstNotSecond = idList1.Except(idList).ToList();
            DeleteFirstList(firstNotSecond, faaliyetId, katilimciId, katilimciTipi);
        }
        private void DeleteFirstList(List<string> list, int faaliyetId, int katilimciId, int katilimciTipi)
        {
            string joined = string.Join(",", list.ToArray());
            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            aniObjesiDagitim.Delete(faaliyetId, katilimciId, katilimciTipi, joined);
        }
        #region timer

        protected void RefreshTimer_Tick(object sender, EventArgs e)
        {

            Faaliyet sonHali = new Faaliyet();
            sonHali = sonHali.Select(FaaliyetIdQS.ConvertToInt());
            if (sonHali != null)
            {
                TempFaaliyet tempFaaliyetSonHali = new TempFaaliyet(sonHali);
                if (!tempFaaliyetSonHali.Equals(TempFaaliyetQS))
                {
                    //faaliyet değişti
                    DisableComponents();
                    AddRefreshLink(FaaliyetIdQS.ConvertToInt(), TopBarDiv, "Faaliyet bilgileri değişti... Sayfayı yenilemek için lütfen tıklayın.", tempFaaliyetSonHali);
                    CloseModals();
                    KatilimciBilgileriniDoldur(sonHali, false);
                    KatilimciEkleBtn.Visible = false;
                    StopTimer();

                }
                List<KatilimciListItem> katilimciListSonHali = GetDataList(true);
                List<string> first = KatilimciListQS;
                List<string> second = katilimciListSonHali.Select(a => a.KatilimciId + " " + a.AniObjesiStoklu + a.AniObjesiStoksuz).ToList();
                var firstNotSecond = first.Except(second).ToList();
                var secondNotFirst = second.Except(first).ToList();

                bool katilimciListesiDegisti = firstNotSecond.Any() || secondNotFirst.Any();

                if (katilimciListesiDegisti)
                {
                    DisableComponents();
                    AddRefreshLink(FaaliyetIdQS.ConvertToInt(), TopBarDiv, "Katilimci bilgileri değişti... Listeyi yenilemek için lütfen tıklayın.", tempFaaliyetSonHali);
                    CloseModals();
                    KatilimciBilgileriniDoldur(sonHali, false);
                    KatilimciEkleBtn.Visible = false;
                    StopTimer();
                }
            }
            //System.Threading.Thread.Sleep(9000);
            lblTime.Text = DateTime.Now.ToString("HH:mm ss");
        }

        private void CloseModals()
        {
            UtilityHelper.ScriptCalistir("CloseModals();");
        }

        private void StopTimer()
        {
            RefreshTimer.Enabled = false;
        }

        private void AddRefreshLink(int faaliyetId, HtmlGenericControl divName, string message, TempFaaliyet tempFaaliyetSonHali)
        {
            HyperLink refreshLink = HyperLinkGetir(faaliyetId, message);
            //+ " TempFaaliyetQS.Id=" + TempFaaliyetQS.Id+" FaaliyetIdQS="+FaaliyetIdQS
            //+ " TempFaaliyetQS.Id=" + TempFaaliyetQS.Id+ " =? " +tempFaaliyetSonHali.Id
            //+ " TempFaaliyetQS.IcIrtibatId=" + TempFaaliyetQS.IcIrtibatId+ " =? " +tempFaaliyetSonHali.IcIrtibatId
            //+ " TempFaaliyetQS.DisIrtibatId=" + TempFaaliyetQS.DisIrtibatId+ " =? " +tempFaaliyetSonHali.DisIrtibatId
            //+ " TempFaaliyetQS.FaaliyetYeri=" + TempFaaliyetQS.FaaliyetYeri+ " =? " +tempFaaliyetSonHali.FaaliyetYeri
            //+ " TempFaaliyetQS.Aciklama=" + TempFaaliyetQS.Aciklama+ " =? " +tempFaaliyetSonHali.Aciklama
            //+ " TempFaaliyetQS.AcikTarih=" + TempFaaliyetQS.AcikTarih+ " =? " +tempFaaliyetSonHali.AcikTarih
            //+ " TempFaaliyetQS.BaslangicSaati=" + TempFaaliyetQS.BaslangicSaati+ " =? " +tempFaaliyetSonHali.BaslangicSaati
            //+ " TempFaaliyetQS.BaslangicTarihi=" + TempFaaliyetQS.BaslangicTarihi+ " =? " +tempFaaliyetSonHali.BaslangicTarihi
            //+ " TempFaaliyetQS.BitisSaati=" + TempFaaliyetQS.BitisSaati+ " =? " +tempFaaliyetSonHali.BitisSaati
            //+ " TempFaaliyetQS.BitisTarihi=" + TempFaaliyetQS.BitisTarihi+ " =? " +tempFaaliyetSonHali.BitisTarihi
            //+ " TempFaaliyetQS.FaaliyetAmaci=" + TempFaaliyetQS.FaaliyetAmaci+ " =? " +tempFaaliyetSonHali.FaaliyetAmaci
            //+ " TempFaaliyetQS.FaaliyetDurumu=" + TempFaaliyetQS.FaaliyetDurumu+ " =? " +tempFaaliyetSonHali.FaaliyetDurumu
            //+ " TempFaaliyetQS.FaaliyetKonusu=" + TempFaaliyetQS.FaaliyetKonusu+ " =? " +tempFaaliyetSonHali.FaaliyetKonusu
            //+ " TempFaaliyetQS.FaaliyetTipi=" + TempFaaliyetQS.FaaliyetTipi+ " =? " +tempFaaliyetSonHali.FaaliyetTipi
            //+ " TempFaaliyetQS.FaaliyetYeri=" + TempFaaliyetQS.FaaliyetYeri+ " =? " +tempFaaliyetSonHali.FaaliyetYeri
            //+ " TempFaaliyetQS.TumGun=" + TempFaaliyetQS.TumGun + " =? " +tempFaaliyetSonHali.TumGun
            //+ " TempFaaliyetQS.FaaliyetTipi=" + TempFaaliyetQS.FaaliyetTipi + tempFaaliyetSonHali.FaaliyetTipi);
            divName.Controls.Add(refreshLink);

        }

        private void DisableComponents()
        {
            FaaliyetTipiDDL.Enabled = false;
            FaaliyetAmaciDDL.Enabled = false;
            BaslangicTarihiTxt.Enabled = false;
            BasSaatDDL.Enabled = false;
            BitisTarihiTxt.Enabled = false;
            BitSaatDDL.Enabled = false;
            FaaliyetYeriDDL.Enabled = false;
            FaaliyetDurumuDDL.Enabled = false;
            FaaliyetKonusuTxt.Enabled = false;
            TumGunChk.Enabled = false;
            AcikTarihChk.Enabled = false;
            AciklamaTxt.Enabled = false;
            KaydetBtn.Visible = false;
            GuncelleBtn.Visible = false;
            IcIrtibatCikarBtn.Visible = false;
            DisIrtibatCikarBtn.Visible = false;
        }
        private HyperLink HyperLinkGetir(int faaliyetId, string message)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            HyperLink hyperLink = new HyperLink
            {
                Text = message,
                CssClass = "form-control btn btn-warning font-weight-bold",
            };

            if (faaliyetId > 0)
            {
                string linkUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_FAALIYET_GIRIS + "?FaaliyetId=" + faaliyetId;

                hyperLink.NavigateUrl = linkUrl;
            }
            return hyperLink;
        }
        #endregion
        #region classes
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
            public string AniObjesiStoklu { get; set; }
            public string AniObjesiStoksuz { get; set; }
            public string GetirilenAniObjesi { get; set; }
            public string KisiKarti { get; set; }
            public string Cikar { get; set; }
            public string KatilimciSec { get; set; }
            public string IrtibatSec { get; set; }

        }
        [Serializable]
        private class TempFaaliyet : IEquatable<TempFaaliyet>
        {
            public TempFaaliyet(Faaliyet faaliyet)
            {
                Id = faaliyet.Id;
                FaaliyetTipi = faaliyet.FaaliyetTipi;
                FaaliyetAmaci = faaliyet.FaaliyetAmaci;
                FaaliyetKonusu = faaliyet.FaaliyetKonusu;
                FaaliyetYeri = faaliyet.FaaliyetYeri;
                FaaliyetDurumu = faaliyet.FaaliyetDurumu;
                TumGun = faaliyet.TumGun;
                AcikTarih = faaliyet.AcikTarih;
                IcIrtibatId = faaliyet.IcIrtibatId;
                DisIrtibatId = faaliyet.DisIrtibatId;
                BaslangicTarihi = faaliyet.BaslangicTarihi;
                BitisTarihi = faaliyet.BitisTarihi;
                BaslangicSaati = faaliyet.BaslangicSaati;
                BitisSaati = faaliyet.BitisSaati;
                Aciklama = faaliyet.Aciklama;
                YoneticiNotu = faaliyet.YoneticiNotu;
            }

            public int Id { get; set; }
            public string FaaliyetTipi { get; set; }
            public int FaaliyetAmaci { get; set; }
            public string FaaliyetKonusu { get; set; }
            public int FaaliyetYeri { get; set; }
            public int FaaliyetDurumu { get; set; }
            public bool TumGun { get; set; }
            public bool AcikTarih { get; set; }
            public int IcIrtibatId { get; set; }
            public int DisIrtibatId { get; set; }
            public DateTime BaslangicTarihi { get; set; }
            public DateTime BitisTarihi { get; set; }
            public string BaslangicSaati { get; set; }
            public string BitisSaati { get; set; }
            public string Aciklama { get; set; }
            public string YoneticiNotu { get; set; }
            public override bool Equals(object obj)
            {
                var other = obj as Faaliyet;

                if (other == null)
                    return false;

                if (FaaliyetTipi != other.FaaliyetTipi
                    || FaaliyetAmaci != other.FaaliyetAmaci
                    || FaaliyetKonusu != other.FaaliyetKonusu
                    || FaaliyetYeri != other.FaaliyetYeri
                    || TumGun != other.TumGun
                    || AcikTarih != other.AcikTarih
                    || IcIrtibatId != other.IcIrtibatId
                    || DisIrtibatId != other.DisIrtibatId
                    || BaslangicTarihi != other.BaslangicTarihi
                    || BitisTarihi != other.BitisTarihi
                    || BaslangicSaati != other.BaslangicSaati
                    || BitisSaati != other.BitisSaati
                    || Aciklama != other.Aciklama
                    || YoneticiNotu != other.YoneticiNotu)
                    return false;

                return true;
            }

            public bool Equals(TempFaaliyet other)
            {
                return !(other is null) &&
                       FaaliyetTipi == other.FaaliyetTipi &&
                       FaaliyetAmaci == other.FaaliyetAmaci &&
                       FaaliyetKonusu == other.FaaliyetKonusu &&
                       FaaliyetYeri == other.FaaliyetYeri &&
                       FaaliyetDurumu == other.FaaliyetDurumu &&
                       TumGun == other.TumGun &&
                       AcikTarih == other.AcikTarih &&
                       IcIrtibatId == other.IcIrtibatId &&
                       DisIrtibatId == other.DisIrtibatId &&
                       BaslangicTarihi == other.BaslangicTarihi &&
                       BitisTarihi == other.BitisTarihi &&
                       BaslangicSaati == other.BaslangicSaati &&
                       BitisSaati == other.BitisSaati &&
                       Aciklama == other.Aciklama;
            }

            public override int GetHashCode()
            {
                int hashCode = 477006145;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FaaliyetTipi);
                hashCode = hashCode * -1521134295 + FaaliyetAmaci.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FaaliyetKonusu);
                hashCode = hashCode * -1521134295 + FaaliyetYeri.GetHashCode();
                hashCode = hashCode * -1521134295 + FaaliyetDurumu.GetHashCode();
                hashCode = hashCode * -1521134295 + TumGun.GetHashCode();
                hashCode = hashCode * -1521134295 + AcikTarih.GetHashCode();
                hashCode = hashCode * -1521134295 + IcIrtibatId.GetHashCode();
                hashCode = hashCode * -1521134295 + DisIrtibatId.GetHashCode();
                hashCode = hashCode * -1521134295 + BaslangicTarihi.GetHashCode();
                hashCode = hashCode * -1521134295 + BitisTarihi.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BaslangicSaati);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BitisSaati);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Aciklama);
                return hashCode;
            }

            public static bool operator ==(TempFaaliyet left, TempFaaliyet right)
            {
                return EqualityComparer<TempFaaliyet>.Default.Equals(left, right);
            }

            public static bool operator !=(TempFaaliyet left, TempFaaliyet right)
            {
                return !(left == right);
            }
        }
        #endregion
    }
}
