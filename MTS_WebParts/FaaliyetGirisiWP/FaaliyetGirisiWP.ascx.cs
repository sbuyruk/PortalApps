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
using System.Linq;
using System.Text;
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

        #region Globals
        
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
        private List<string> KatilimciListQS //{ get; set; }//Postback disinda güncellenmeli
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
        #endregion

        #region PageLoad, DDL, Form vs
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                if (!Page.IsPostBack) // sayfa ilk kez açiliyorsa 
                {
                    lblTime.Text = DateTime.Now.ToString("HH:mm ss");
                    RefreshTimer.Interval = 10000;
                    RefreshTimer.Enabled = true;

                    FaaliyetTipiDDLDoldur();
                    FaaliyetAmaciDDLDoldur();
                    FaaliyetDurumuDDLDoldur();
                    FaaliyetYeriDoldur();
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
                OzelKalemTakvimiChk.Checked= faaliyet.TakvimeIslendi;

                UtilityHelper.SetDDLValue(FaaliyetTipiDDL, faaliyet.FaaliyetTipi);
                UtilityHelper.SetDDLValue(FaaliyetAmaciDDL, faaliyet.FaaliyetAmaciId.ToString());
                UtilityHelper.SetDDLValue(FaaliyetDurumuDDL, faaliyet.FaaliyetDurumu.ToString());
                //UtilityHelper.SetDDLValue(FaaliyetYeriDDL, faaliyet.FaaliyetYeri.ToString());
                FaaliyetYeriTxt.Text = faaliyet.FaaliyetYeriStr;
                UtilityHelper.SetDDLValue(BasSaatDDL, faaliyet.BaslangicSaati);
                UtilityHelper.SetDDLValue(BitSaatDDL, faaliyet.BitisSaati);

                DisIrtibatIdQS = faaliyet.DisIrtibatId.ToString();
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
            TitleLbl.CssClass = "col-form-label text-success fw-bold mb-1";
            BaslangicTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
            BitisTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
            FaaliyetIdQS = string.Empty;
            AramaGorusmeIdQS = string.Empty;
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
                MessageHelper.PublishMessage("Faaliyet bulunamadi. Yeni faaliyet girebilirsiniz.", ProjeConstants.MESAJ_BILGI, 2000);
            }

        }
        private void BaslangicSaatiDDLDoldur()
        {
            BasSaatDDL.Items.Clear();
            TimeSpan bastarTS = new TimeSpan(0, 0, 0);
            TimeSpan aralikTS = new TimeSpan(0, 5, 0);
            TimeSpan bittarTS = new TimeSpan(23, 50, 0);

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
            TimeSpan aralikTS = new TimeSpan(0, 5, 0);
            TimeSpan bittarTS = new TimeSpan(23, 55, 0);

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
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            int faaliyetId = 0;
            try
            {
                Faaliyet faaliyet = new Faaliyet();
                faaliyet.FaaliyetTipi = FaaliyetTipiDDL.SelectedItem.Text;
                faaliyet.FaaliyetAmaciId = FaaliyetAmaciDDL.SelectedItem.Value.ConvertToInt();
                faaliyet.FaaliyetKonusu = FaaliyetKonusuTxt.Text;
                //faaliyet.FaaliyetYeri = FaaliyetYeriDDL.SelectedItem.Value.ConvertToInt();
                faaliyet.FaaliyetYeriStr = FaaliyetYeriTxt.Text;
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
                faaliyet.TakvimeIslendi = OzelKalemTakvimiChk.Checked;
                faaliyet.DisIrtibatId = DisIrtibatIdQS.ConvertToInt();
                faaliyet.Olusturan = UtilityHelper.GetCurrentUserName();

                if (string.IsNullOrEmpty(FaaliyetKonusuTxt.Text))
                {
                    MessageHelper.PublishMessage("Faaliyet Konusu Bos Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
                }
                else
                {

                    faaliyetId = faaliyet.Id = faaliyet.Save();
                    if (faaliyetId > 0)
                    {
                        FaaliyetIdQS = faaliyetId.ToString();
                        //Save icinde olay kaydi var zaten, o yüzden kommentlendi
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
                string faaliyetYeriStr = faaliyet.FaaliyetYeriStr;

                if (islemTipi.Equals(ProjeConstants.KAYDET))
                {
                    MailHelper.TakvimeEkle(faaliyet.UniqueId, from, userto, baslik, faaliyet.BaslangicTarihi, faaliyet.BitisTarihi, faaliyetYeriStr,
                        faaliyet.Aciklama, ProjeConstants.PARAM_INTERNET_SMTP_IP_ADRESI);

                }else if (islemTipi.Equals(ProjeConstants.GUNCELLE))
                {
                    if (AcikTarihChk.Checked)
                    {
                        MailHelper.TakvimdenSil(faaliyet.UniqueId, from, userto, " -Açik Tarihe Alindi- " + baslik, faaliyet.BaslangicTarihi, faaliyet.BitisTarihi, faaliyetYeriStr,
                            faaliyet.Aciklama, ProjeConstants.PARAM_INTERNET_SMTP_IP_ADRESI);
                    }
                    else
                    {
                        MailHelper.TakvimeEkle(faaliyet.UniqueId, from, userto, baslik, faaliyet.BaslangicTarihi, faaliyet.BitisTarihi, faaliyetYeriStr,
                            faaliyet.Aciklama, ProjeConstants.PARAM_INTERNET_SMTP_IP_ADRESI);
                    }
                }else if (islemTipi.Equals(ProjeConstants.SIL))
                {
                    MailHelper.TakvimdenSil(faaliyet.UniqueId, from, userto, baslik, faaliyet.BaslangicTarihi, faaliyet.BitisTarihi, faaliyetYeriStr, 
                        faaliyet.Aciklama, ProjeConstants.PARAM_INTERNET_SMTP_IP_ADRESI);
                }

            }
            else
            {
                MessageHelper.PublishMessage("Özel kalem takvimine islenmedi",ProjeConstants.MESAJ_BILGI,2000);
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
                    MessageHelper.PublishMessage("Faaliyet bulunamadi", ProjeConstants.MESAJ_HATA, 5000);
                }
                else
                {
                    faaliyet.FaaliyetTipi = FaaliyetTipiDDL.SelectedItem.Text;
                    faaliyet.FaaliyetAmaciId = FaaliyetAmaciDDL.SelectedItem.Value.ConvertToInt();
                    faaliyet.FaaliyetKonusu = FaaliyetKonusuTxt.Text;
                    faaliyet.FaaliyetYeriStr = FaaliyetYeriTxt.Text;
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
                    faaliyet.TakvimeIslendi = OzelKalemTakvimiChk.Checked;
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
                        KatilimcilaraTakvimDavetiGonder(faaliyet);
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
                MessageHelper.PublishMessage("Faaliyet bulunamadiginidan dolayi silinemedi.", ProjeConstants.MESAJ_HATA);
            }
            //Faaliyet Silme Kaldirildi SB 31.05.2021
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
            //    Exception e2 = new Exception("Kisi silinemedi");
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
            aramaGorusme = aramaGorusme.SelectByFaaliyetId(faaliyet.Id);

            FaaliyetKatilim faaliyetKatilim = new FaaliyetKatilim();
            List<FaaliyetKatilim> faaliyetKatilimList = faaliyetKatilim.SelectByFaaliyetId(faaliyet.Id);
            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            List<AniObjesiDagitim> aniObjesiDagitimList = aniObjesiDagitim.SelectByFaaliyetId(faaliyet.Id);
            if (aramaGorusme != null)
            {
                Exception ex = new Exception("Faaliyetin baglantili oldugu Arama/Görüsme bulunmaktadir.");
                eh.Exceptions.Add(ex);
                baglantisiVar = true;
            }
            if (faaliyetKatilimList.Count > 0)
            {
                Exception ex = new Exception("Faaliyetin baglantili oldugu Katilimci(lar) bulunmaktadir.");
                eh.Exceptions.Add(ex);
                baglantisiVar = true;
            }
            if (aniObjesiDagitimList.Count > 0)
            {
                Exception ex = new Exception("Faaliyetin baglantili oldugu Ani Objeleri bulunmaktadir.");
                eh.Exceptions.Add(ex);
                baglantisiVar = true;
            }
            if (baglantisiVar)
            {
                Exception ex = new Exception("Faaliyet, baglantilari nedeniyle silinemez. ");
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
        protected void AcikTarihliFaaliyetListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_ACIKTARIHLIFAALIYET_LIST);
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
            SilMesajiLbl.Text = "Faaliyete ait tüm bilgiler silinecek. Silmek istediginizden emin misiniz?";
            SilModalBaslikLbl.Text = "Faaliyet Silinecek";
            FaaliyetSilNowBtn.Visible = true;
            UtilityHelper.ScriptCalistir(openModal);
        }
        /// FaaliyetKatilim_Tabeledan bu faaliyet Id'li kayitlari sil
        /// AramaGorusme_Table'da bu faaliyetId'li kayitlarin faaliyetId'sini 0 yap
        /// AniObjesiDagitim_Table'dan bu faaliyetId'li olanlari sil
        protected void FaaliyetSilNowBtn_Click(object sender, EventArgs e)
        {
            bool silindi = false;
            try
            {
                Faaliyet faaliyet = new Faaliyet();
                faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
                if (faaliyet != null)
                {
                    //baglantisi yoksa sil
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
       
        private void FaaliyetTipiDDLDoldur()
        {
            ListItem li = new ListItem(ProjeConstants.RANDEVU_VERILEN);
            ListItem li2 = new ListItem(ProjeConstants.RANDEVU_ALINAN);
            ListItem li3 = new ListItem(ProjeConstants.RANDEVU_DIGER);

            FaaliyetTipiDDL.Items.Clear();
            FaaliyetTipiDDL.Items.Add(li);
            FaaliyetTipiDDL.Items.Add(li2);
            FaaliyetTipiDDL.Items.Add(li3);
        }
        private void FaaliyetAmaciDDLDoldur()
        {
            ListItem li5 = new ListItem(ProjeConstants.FAALIYET_AMACI_TOPLANTI, ProjeConstants.FAALIYET_AMACI_TOPLANTI_INT);
            ListItem li = new ListItem(ProjeConstants.FAALIYET_AMACI_ZIYARET, ProjeConstants.FAALIYET_AMACI_ZIYARET_INT);
            ListItem li7 = new ListItem(ProjeConstants.FAALIYET_AMACI_GORUSME, ProjeConstants.FAALIYET_AMACI_GORUSME_INT);
            ListItem li2 = new ListItem(ProjeConstants.FAALIYET_AMACI_DAVET, ProjeConstants.FAALIYET_AMACI_DAVET_INT);
            ListItem li3 = new ListItem(ProjeConstants.FAALIYET_AMACI_YILDONUMU, ProjeConstants.FAALIYET_AMACI_YILDONUMU_INT);
            ListItem li4 = new ListItem(ProjeConstants.FAALIYET_AMACI_OZELCALISMA, ProjeConstants.FAALIYET_AMACI_OZELCALISMA_INT);
            ListItem li6 = new ListItem(ProjeConstants.FAALIYET_AMACI_IZIN, ProjeConstants.FAALIYET_AMACI_IZIN_INT);
            ListItem li8 = new ListItem(ProjeConstants.FAALIYET_AMACI_SEYAHAT, ProjeConstants.FAALIYET_AMACI_SEYAHAT_INT);
            ListItem li9 = new ListItem(ProjeConstants.FAALIYET_AMACI_BILGI, ProjeConstants.FAALIYET_AMACI_BILGI_INT);


            FaaliyetAmaciDDL.Items.Clear();
            FaaliyetAmaciDDL.Items.Add(li5);
            FaaliyetAmaciDDL.Items.Add(li);
            FaaliyetAmaciDDL.Items.Add(li7);
            FaaliyetAmaciDDL.Items.Add(li2);
            FaaliyetAmaciDDL.Items.Add(li3);
            FaaliyetAmaciDDL.Items.Add(li4);
            FaaliyetAmaciDDL.Items.Add(li6);
            FaaliyetAmaciDDL.Items.Add(li8);
            FaaliyetAmaciDDL.Items.Add(li9);
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
       
        private void FaaliyetYeriDoldur()
        {
            Faaliyet faaliyetDdo = new Faaliyet();
            List<string> list = faaliyetDdo.SelectAllDistinctFaaliyetYeri();

            var fyerleristr = "\"" + string.Join("\", \"", list) + "\"";
            StringBuilder fyerleri = new StringBuilder();
            fyerleri.Append("[");
            fyerleri.Append(fyerleristr);

            fyerleri.Append("]");
            UtilityHelper.ScriptCalistir("setDataSet(" + fyerleri + ");");
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
                Page.Response.Redirect(newUrl,false);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void DisIrtibatNoktasiDoldur(int kisiId, bool buttonsEnabled)
        {
            Kisi kisi = new Kisi();
            kisi = kisi.Select(kisiId);
            if (kisi != null)
            {
                DisIrtibatLbl.Text = "Irtibat Noktasi : " + kisi.Adi + " " + kisi.Soyadi;
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
                    faaliyet.DisIrtibatId = katilimciId;
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
        #region Katilimci Tablosu
        private void TabloOlustur(bool buttonsEnabled)
        {
            var jsonData = TabloJson(buttonsEnabled); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
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
                { data: 'AniObjesi' },
                { data: 'TakvimDavetiBtn' },
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
            DataTable dataTable = faaliyetDao.SelectAllByKatilimciFaaliyetReturnDataTable(FaaliyetIdQS.ConvertToInt(), ProjeConstants.HEPSI_INT, ProjeConstants.HEPSI, ProjeConstants.NULL_TARIH, ProjeConstants.NULL_TARIH, ProjeConstants.HEPSI);
            int SiraNo = 1;
            List<KatilimciListItem> list = new List<KatilimciListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    int katilimId = row["KatilimId"].ReturnZeroIfNull().ConvertToInt();
                    int katilimciId = row["KatilimciId"].ReturnZeroIfNull().ConvertToInt();
                    if (katilimciId < 1)
                        continue;
                    int katilimciTipi = row["KatilimciTipi"].ReturnZeroIfNull().ConvertToInt();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();
                    string kurumu = row["Kurumu"].ReturnEmptyIfNull().ToString();
                    string takvimDaveti = row["TakvimDaveti"].ReturnEmptyIfNull().ToString();
                    string eposta = row["EPosta"].ReturnEmptyIfNull().ToString();

                    KatilimciListItem katilimciItem = new KatilimciListItem();
                    katilimciItem.Sirano = SiraNo++.ToString();
                    katilimciItem.KatilimciId = katilimciId.ToString();
                    katilimciItem.Adi = adi;
                    katilimciItem.Soyadi = soyadi;
                    katilimciItem.AdiSoyadi = (adi + " " + soyadi.Trim());
                    katilimciItem.Kurumu = kurumu;
                    katilimciItem.KatilimciTipi = katilimciTipi.ToString();
                    katilimciItem.TakvimDavetiBtn = TakvimDavetiBtnOlustur(katilimId, takvimDaveti, katilimciId, eposta);


                    string stoksuzAniObjeleri = AniObjeleriniGetir(katilimciItem.KatilimciId.ConvertToInt(),
                        FaaliyetIdQS.ConvertToInt(), ProjeConstants.MTS_ANIOBJESISTOKSUZ);
                    string stokluAniObjeleri = AniObjeleriniGetir(katilimciItem.KatilimciId.ConvertToInt(),
                        FaaliyetIdQS.ConvertToInt(), ProjeConstants.MTS_ANIOBJESISTOKLU);
                    string getirilenAniObjeleri = GetirilenAniObjeleriniGetir(katilimciItem.KatilimciId.ConvertToInt(),
                        FaaliyetIdQS.ConvertToInt());

                    if (buttonsEnable)
                    {
                        katilimciItem.AniObjesiStoksuz = "<a href='#' class='btn btn-outline-primary' onclick=StoksuzAniObjesiBtnClick(" + katilimciId + "," + FaaliyetIdQS + ")>Stoksuz Ani Objesi</a>" +
                            "<br>" + stoksuzAniObjeleri;
                        katilimciItem.AniObjesiStoklu = "<a href='#' class='btn btn-outline-success' onclick=StokluAniObjesiBtnClick(" + katilimciId + "," + FaaliyetIdQS + ")>Stoklu Ani Objesi</a>" +
                            "<br>" + stokluAniObjeleri;
                        katilimciItem.GetirilenAniObjesi = "<a href='#' class='btn btn-outline-info' onclick=GetirilenAniObjesiBtnClick(" + katilimciId + "," + FaaliyetIdQS + ")>Getirilen Ani Objesi </a>" +
                            "<br>" + getirilenAniObjeleri;

                        katilimciItem.AniObjesi = katilimciItem.AniObjesiStoklu + "<hr>" + katilimciItem.AniObjesiStoksuz + "<hr>" + katilimciItem.GetirilenAniObjesi;
                    }
                    else
                    {
                        katilimciItem.AniObjesiStoksuz = stoksuzAniObjeleri;
                        katilimciItem.AniObjesiStoklu = stokluAniObjeleri;
                        katilimciItem.GetirilenAniObjesi = getirilenAniObjeleri;
                    }


                    katilimciItem.KisiKarti = "<a  target='_blank' href=" + ProjeConstants.PAGE_KISI_KARTI + "?KatilimciId=" + katilimciId + " class='btn btn-outline-info'>Kisi Karti</a>";
                    if (buttonsEnable)
                        katilimciItem.Cikar = "<a href='#' class='btn btn-outline-danger' onclick=KatilimciCikarBtnClick(" + katilimId + ")>Çikar</a>";

                    list.Add(katilimciItem);
                }
            }
            return list;
        } 
        #endregion

        protected void FaaliyetKartiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_KARTI + "?FaaliyetId=" + FaaliyetIdQS);
        }

        protected void AcikTarihChk_CheckedChanged(object sender, EventArgs e)
        {
            AcikTarihliKontrolu();
        }

        private void AcikTarihliKontrolu()
        {
            Faaliyet faaliyet = new Faaliyet();
            faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
            if (faaliyet == null && AcikTarihChk.Checked)
            {
                OzelKalemTakvimiChk.Checked = false;
                MessageHelper.PublishMessage("Açik Tarihli faaliyet oldugundan Özel Kalem takvimine gönderilmeyecek", ProjeConstants.MESAJ_BILGI, 2000);
            }
        }

        protected void OzelKalemTakvimiChk_CheckedChanged(object sender, EventArgs e)
        {
            AcikTarihliKontrolu();
        }
        #endregion

        #region Modal katilimci seçme ve ekleme
        protected void KatilimciEkleBtn_Click(object sender, EventArgs e)
        {
            KatilimciModalAc();
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
                    DataTable dataTable = aniObjesiDagitim.SelectByKatilimcidFaaliyetId(faaliyetKatilim.KatilimciId, faaliyetKatilim.FaaliyetId,string.Empty);
                    if (dataTable == null)
                    {
                        if (faaliyetKatilim.Delete())
                        {
                            MessageHelper.PublishMessage("Katilimci faaliyetten çikarildi", ProjeConstants.MESAJ_BASARILI,2000);
                        }
                    }
                    else
                    {
                        ExceptionHelper eh= new ExceptionHelper();
                        Exception ex = new Exception("Faaliyetin baglantili oldugu Ani Objeleri bulunmaktadir.");
                        eh.Exceptions.Add(ex);
                        eh.PublishException();
                    }
                }

            }
            else
            {
                MessageHelper.PublishMessage("Katilimci Bulunamadi", ProjeConstants.MESAJ_HATA);
            }
            Faaliyet faaliyet = new Faaliyet();
            faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
            if (faaliyet != null)
            {
                KatilimciBilgileriniDoldur(faaliyet, true);
            }
        }
        private void KatilimciModalAc()
        {
            TabloModalOlustur();
            UtilityHelper.ScriptCalistir("KatilimciSecimiModal();");
        }
        private void TabloModalOlustur()
        {
            var jsonData = TabloModalJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazirlaniyor.
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

                } else if (data.KatilimciTipi == null || data.KatilimciTipi ==0 || data.KatilimciTipi == 2) {
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

            Kisi kisi = new Kisi();
            DataTable dataTable = kisi.SelectSecilmemisDisKatilimcilarByFaaliyetIdReturnDT(FaaliyetIdQS.ConvertToInt());


            int SiraNo = 1;
            List<KatilimciListItem> list = new List<KatilimciListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string katilimciId = row["KatilimciId"].ToString();
                    int katilimciTipi = row["KatilimciTipi"].ConvertToInt();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();
                    string kurumu = row["Kurumu"].ToString();
                    bool randevuKisiti = row["RandevuKisiti"].ReturnFalseIfNull().ConvertToBool();

                    KatilimciListItem katilimciItem = new KatilimciListItem();
                    katilimciItem.Sirano = SiraNo++.ToString();
                    katilimciItem.KatilimciId = katilimciId;
                    katilimciItem.AdiSoyadi = (adi + " " + soyadi.Trim());
                    katilimciItem.Adi = adi;
                    katilimciItem.Soyadi = soyadi;
                    katilimciItem.Kurumu = kurumu;
                    katilimciItem.KatilimciTipi = katilimciTipi.ToString();

                    if (randevuKisiti)//randevu kisiti varsa faaliyete ekle çikmasin
                    {
                        katilimciItem.KatilimciSec = "RK";
                        katilimciItem.IrtibatSec = "RK";
                    }
                    else
                    {

                        katilimciItem.KatilimciSec = "<a href='#' class='btn btn-outline-primary' onclick=KatilimciSecildiBtnClick(" + katilimciId + "," + FaaliyetIdQS + ")>Faaliyete Ekle</a>";
                        katilimciItem.IrtibatSec = "<a href='#' class='btn btn-outline-primary' onclick=IrtibatSecBtnClick(" + katilimciId + "," + FaaliyetIdQS + ")>Irtibat Ekle</a>";
                    }


                    list.Add(katilimciItem);
                }
            }
            return list;
        }
        protected void SecilenKatilimciyiKaydetNowBtn_Click(object sender, EventArgs e)
        {
            int katilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
            int faaliyetId = paramFaaliyetIdLbl.Value.ConvertToInt();
            if (katilimciId > 0)
            {
                

                FaaliyetKatilim faaliyetKatilim = new FaaliyetKatilim();

                List<FaaliyetKatilim> list = faaliyetKatilim.Select(faaliyetId, katilimciId);
                if (list.Count < 1)
                {
                    string kurumGorevStr = string.Empty;

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
                    
                    
                    

                    faaliyetKatilim.KatilimciId = katilimciId;
                    faaliyetKatilim.FaaliyetId = faaliyetId;
                    faaliyetKatilim.KurumGorev = kurumGorevStr;
                    int faaliyetKatilimId=faaliyetKatilim.Save();
                    
                }
            }
            else
            {
                MessageHelper.PublishMessage("Katilimci Bulunamadi", ProjeConstants.MESAJ_HATA);
            }
            Faaliyet faaliyet = new Faaliyet();
            faaliyet = faaliyet.Select(FaaliyetIdQS.ConvertToInt());
            if (faaliyet != null)
            {
                KatilimciBilgileriniDoldur(faaliyet, true);
            }


        }
        private void KatilimciBilgileriniDoldur(Faaliyet faaliyet, bool buttonsEnabled)
        {
            if (faaliyet != null)
            {
                AramaGorusmeBilgileriniDoldur(faaliyet);
                DisIrtibatNoktasiDoldur(faaliyet.DisIrtibatId, buttonsEnabled);

                TabloOlustur(buttonsEnabled);
                if (buttonsEnabled)
                {
                    TempFaaliyetQS.DisIrtibatId = faaliyet.DisIrtibatId;
                }


            }

        }
        #endregion

        #region Arama/Gorusme bilgileri
        private void AramaGorusmeBilgileriniDoldur(Faaliyet faaliyet)
        {
            AramaGorusme aramaGorusme = new AramaGorusme();
            aramaGorusme = aramaGorusme.SelectByFaaliyetId(faaliyet.Id);
            if (aramaGorusme != null)
            {
                AramaGorusmeIdQS = aramaGorusme.Id.ToString();
                AramaGorusmeLbl.Text = "Arama/Görüsme No:" + aramaGorusme.Id;

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
                MessageHelper.PublishMessage("Arama Görüsme baglantisi bulunamadi.", ProjeConstants.MESAJ_HATA);
            }
        }
        #endregion

        #region Ani Objesi
        protected void AniObjesiSecBtn_Click(object sender, EventArgs e)
        {
            int katilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
            int faaliyetId = paramFaaliyetIdLbl.Value.ConvertToInt();
            if (katilimciId > 0)
            {

                    Kisi kisi = new Kisi();
                    kisi = kisi.Select(katilimciId);
                    if (kisi != null)
                    {
                        AniObjesiHeaderLbl.InnerText = "Ani Objesi Seçimi (" + kisi.Adi + " " + kisi.Soyadi + ")";
                        AniObjeleriTablosunuDoldur(faaliyetId, katilimciId);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Kisi bulunamadi", ProjeConstants.MESAJ_HATA);
                    }
                
            }
            else
            {
                MessageHelper.PublishMessage("Katilimci Bulunamadi", ProjeConstants.MESAJ_HATA);
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
                        MessageHelper.PublishMessage("Ani Objesi Tanimi bulunamadi", ProjeConstants.MESAJ_HATA);
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
                        MessageHelper.PublishMessage("Stoklu Depo Tanimi bulunamadi", ProjeConstants.MESAJ_HATA);
                    }
                    DagitimDiv.Attributes["style"] = "display:none";
                    IadeDiv.Attributes["style"] = "display:block";

                    if (isOk)
                        UtilityHelper.ScriptCalistir("StokluAniObjesiModal();");
                }
                else
                {
                    MessageHelper.PublishMessage("Ani Objesi dagitimi bulunamadi", ProjeConstants.MESAJ_HATA);
                }
            }
            else // ekle
            {
                DagitimDiv.Attributes["style"] = "display:block";
                IadeDiv.Attributes["style"] = "display:none";
                int katilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
                
                if (katilimciId > 0)
                {

                        Kisi kisi = new Kisi();
                        kisi = kisi.Select(katilimciId);
                        if (kisi != null)
                        {

                            StokluAniObjesiModalTitle.InnerText = "Stoklu Ani Objesi Seçimi (" + kisi.Adi + " " + kisi.Soyadi + ")";
                            StokluAniObjesiDDLDoldur();
                            DepoDDLDoldur();
                            UtilityHelper.ScriptCalistir("StokluAniObjesiModal();");
                        }
                        else
                        {
                            MessageHelper.PublishMessage("Kisi bulunamadi", ProjeConstants.MESAJ_HATA);
                        }


                }
                else
                {
                    MessageHelper.PublishMessage("Katilimci Bulunamadi", ProjeConstants.MESAJ_HATA);
                }
            }
        }
        protected void GetirilenAniObjesiSecBtn_Click(object sender, EventArgs e)
        {
            int katilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
            int faaliyetId = paramFaaliyetIdLbl.Value.ConvertToInt();

            if (katilimciId > 0)
            {

                    Kisi kisi = new Kisi();
                    kisi = kisi.Select(katilimciId);
                    if (kisi != null)
                    {
                        GetirilenAniObjesiModalTitle.InnerText = "Getirilen Ani Objesi (" + kisi.Adi + " " + kisi.Soyadi + ")";
                        GetirilenAniObjesiniDoldur(faaliyetId, katilimciId);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Kisi bulunamadi", ProjeConstants.MESAJ_HATA);
                    }
                

            }
            else
            {
                MessageHelper.PublishMessage("Katilimci Bulunamadi", ProjeConstants.MESAJ_HATA);
            }
        }
        public void GetirilenAniObjesiniDoldur(int faaliyetId, int katilimciId)
        {
            paramFaaliyetIdLbl.Value = faaliyetId.ToString();
            paramFaaliyetKatilimciIdLbl.Value = katilimciId.ToString();

            AniObjesiDagitim getirilenAniObjesi = new AniObjesiDagitim();
            getirilenAniObjesi = getirilenAniObjesi.SelectGetirilenAniObjesi(faaliyetId, katilimciId);
            if (getirilenAniObjesi != null)
            {
                GetirilenAniObjesiTxt.Text = getirilenAniObjesi.GetirilenAniObjesi;
            }
            UtilityHelper.ScriptCalistir("GetirilenAniObjesiModal();");
        }
        public JavaScriptSerializer javaSerial = new JavaScriptSerializer();
        /// <summary>
        /// Ani Objelerini modal pencereye getirir
        /// Seçilen katilimci ve faaliyetya göre AniObjesiDagitim_Table ile FaaliyetYeri_Table join edilerek sorgulanir,
        /// Daha önceden isaretlenmis olanlar frontend'de javascript arrayde tutulur ve ekrana dolu olarak gelir.
        /// Checkbox check degeri degistiginde,  sadece frontend deki array'e eklenir veya çikarilir.
        /// Kaydete basildiginda array paramArray'e aktarilir, kaydetNowBtn click çalisir, devami btn_click içinde yapilir
        /// </summary>
        /// <param name="faaliyetId"></param>
        /// <param name="katilimciId"></param>
        private void AniObjeleriTablosunuDoldur(int faaliyetId, int katilimciId)
        {
            paramFaaliyetIdLbl.Value = faaliyetId.ToString();
            paramFaaliyetKatilimciIdLbl.Value = katilimciId.ToString();

            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            DataTable dataTable = aniObjesiDagitim.SelectByFaaliyetIdKatilimciIdStokReturnDT(faaliyetId, katilimciId, ProjeConstants.MTS_ANIOBJESISTOKSUZ);
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
                            newdiv.Attributes.Add("class", "checkbox fw-bold text-danger");
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
        /// <summary>
        /// ilk olarak bu Faaliyet ve KatilimciId için AniObjesiDagitim_Table'daki kayitlari al (list1)
        /// sonra
        ///     id ve adet arraylerini al (list2)
        ///     list1de olup da list2de olmayan kayitlari sil
        ///     list1de ve list2de olan kayitlari list2 adetinde göre update et
        ///     list1de olmayip lis2de olanlari insert et
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SecilenleriKaydetBtn_Click(object sender, EventArgs e)
        {
            int faaliyetId = paramFaaliyetIdLbl.Value.ConvertToInt();
            int katilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();

            string idler = paramAniObjesiIdArray.Value;
            string adetler = paramAniObjesiAdetArray.Value;
            List<string> idList = idler.Split(',').ToList<string>();
            List<string> adetList = adetler.Split(',').ToList<string>();

            KaldirilanlariSil(idList, faaliyetId, katilimciId);

            AniObjesiDagitim aniObjesiDagitimDao = new AniObjesiDagitim();
            List<AniObjesiDagitim> objeList = aniObjesiDagitimDao.SelectStoksuzAniObjeleriReturnList(faaliyetId, katilimciId);
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

                        if (adet == 0)//yeni adet sifirsa sil
                        {

                            item.Delete();
                        }
                        else if (adet == item.Adet)//adet ayni bisey yapma
                        {

                        }
                        else if (item.Adet == 0)//adet farkli //eski adet sifirsa insert et
                        {

                            item.Adet = adet;
                            item.FaaliyetId = faaliyetId;
                            item.KatilimciId = katilimciId;
                            item.VerilisTarihi = BaslangicTarihiTxt.Text.ConvertToDatetime();
                            item.Olusturan = UtilityHelper.GetCurrentUserName();
                            item.Save();
                        }
                        else //eski adetle adet farkli  update et
                        {
                            item.Adet = adet;
                            item.FaaliyetId = faaliyetId;
                            item.KatilimciId = katilimciId;
                            item.VerilisTarihi = BaslangicTarihiTxt.Text.ConvertToDatetime();
                            item.Degistiren = UtilityHelper.GetCurrentUserName();
                            item.Update();
                        }
                    }
                }
            }
            if (objeList.Count < 1)
            {
                MessageHelper.PublishMessage("Hiç kayit seçilmedi. Devam etmek için en az bir kayit seçiniz.", ProjeConstants.MESAJ_BILGI);
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

            GetirilenAniObjesiKaydet(faaliyetId, katilimciId);

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

            if (aniObjesiTanim == null) // ani objesi var mi, yoksa
            {
                MessageHelper.PublishMessage("Ani Objesi Bulunamadi. Devam etmek için farkli bir seçim yapiniz.", ProjeConstants.MESAJ_BILGI);
            }
            else //varsa
            {
                //TODO burda kaydet

                //seçilen depoda adet kadar mevcut var mi?
                DepoStok depoStok = new DepoStok();
                depoStok = depoStok.SelectByDepoIdAniObjesiId(depoId, aniObjesiId, ProjeConstants.MTS_ANIOBJESISTOKLU);
                if (depoStok != null)
                {
                    if (depoStok.SonAdet >= adet)
                    {
                        AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
                        int katilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
                        int faaliyetId = paramFaaliyetIdLbl.Value.ConvertToInt();
                        aniObjesiDagitim = aniObjesiDagitim.Select(faaliyetId, katilimciId, aniObjesiId);
                        if (aniObjesiDagitim == null)
                        {
                            aniObjesiDagitim = new AniObjesiDagitim();
                            aniObjesiDagitim.Aciklama = adet + " adet verildi FaaliyetID=" + aniObjesiDagitim.FaaliyetId;
                            aniObjesiDagitim.Adet = StokluAdetTxt.Text.ConvertToInt(); //ilk defa verildi
                            aniObjesiDagitim.AniObjesiId = aniObjesiId;
                            aniObjesiDagitim.CikisDepoId = depoId;
                            aniObjesiDagitim.KatilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
                            aniObjesiDagitim.FaaliyetId = paramFaaliyetIdLbl.Value.ConvertToInt();
                            aniObjesiDagitim.VerilisTarihi = BaslangicTarihiTxt.Text.ConvertToDatetime();
                            aniObjesiDagitim.VerilenAlinan = ProjeConstants.ANIOBJESI_VERILEN_INT;
                            aniObjesiDagitim.Save();
                            //DepoStoktan düs

                            depoStok.SonAdet -= adet;
                            //depoStok.Aciklama+=adet+" adet verildi FaaliyetID=" + aniObjesiDagitim.FaaliyetId;
                            depoStok.SonIslemYapan = UtilityHelper.GetCurrentUserName();
                            depoStok.Update();
                        }
                        else
                        {
                            aniObjesiDagitim.Aciklama = adet + " adet verildi FaaliyetID=" + aniObjesiDagitim.FaaliyetId;
                            aniObjesiDagitim.Adet += StokluAdetTxt.Text.ConvertToInt(); //adet artirildi
                            aniObjesiDagitim.AniObjesiId = aniObjesiId;
                            aniObjesiDagitim.CikisDepoId = depoId;
                            aniObjesiDagitim.KatilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
                            aniObjesiDagitim.FaaliyetId = paramFaaliyetIdLbl.Value.ConvertToInt();
                            aniObjesiDagitim.VerilisTarihi = BaslangicTarihiTxt.Text.ConvertToDatetime();
                            aniObjesiDagitim.VerilenAlinan = ProjeConstants.ANIOBJESI_VERILEN_INT;
                            aniObjesiDagitim.Update();
                            //DepoStoktan düs

                            depoStok.SonAdet -= adet;
                            depoStok.Aciklama = adet + " adet verildi FaaliyetID=" + aniObjesiDagitim.FaaliyetId;
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

                if (aniObjesiTanim == null) // ani objesi var mi, yoksa
                {
                    MessageHelper.PublishMessage("Ani Objesi Bulunamadi. Devam etmek için farkli bir seçim yapiniz.", ProjeConstants.MESAJ_BILGI);
                }
                else //varsa
                {
                    //burda iade et
                    //aniObjesiDagitimdan Düs
                    int adet = IadeEdilecekAdetTxt.Text.ConvertToInt();

                    if (aniObjesiDagitim == null)
                    {
                        MessageHelper.PublishMessage("Iade edilemedi, Ani Objesi bulunamadi", ProjeConstants.MESAJ_HATA);
                    }
                    else if (adet > aniObjesiDagitim.Adet) // ani objesi var mi, yoksa
                    {


                        MessageHelper.PublishMessage("Iade edilecek miktarda Ani Objesi bulunmuyor. ", ProjeConstants.MESAJ_HATA);
                    }
                    else
                    {
                        int depoId = aniObjesiDagitim.CikisDepoId.ConvertToInt();
                        aniObjesiDagitim.Aciklama = adet + " adet Iade edildi FaaliyetID=" + aniObjesiDagitim.FaaliyetId;
                        aniObjesiDagitim.Adet -= adet; //adet artirildi
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
                        depoStok.Aciklama = adet + " adet verildi FaaliyetID=" + aniObjesiDagitim.FaaliyetId;
                        depoStok.SonIslemYapan = UtilityHelper.GetCurrentUserName();
                        depoStok.Update();
                        if (aniObjesiDagitim.Adet > 0)
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
                MessageHelper.PublishMessage("Ani Objesi dagitimi bulunamadi.", ProjeConstants.MESAJ_HATA);
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
        private void GetirilenAniObjesiKaydet(int faaliyetId, int katilimciId)
        {
            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            aniObjesiDagitim = aniObjesiDagitim.SelectGetirilenAniObjesi(faaliyetId, katilimciId);

            if (aniObjesiDagitim == null)
            {
                aniObjesiDagitim = new AniObjesiDagitim();
                aniObjesiDagitim.AniObjesiId = ProjeConstants.GETIRILEN_ANIOBJESIID_INT;
                aniObjesiDagitim.FaaliyetId = faaliyetId;
                aniObjesiDagitim.KatilimciId = katilimciId;

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
        private void KaldirilanlariSil(List<string> idList, int faaliyetId, int katilimciId)
        {
            AniObjesiDagitim aniObjesiDagitimDao = new AniObjesiDagitim();
            List<AniObjesiDagitim> objeList = aniObjesiDagitimDao.SelectStoksuzAniObjeleriReturnList(faaliyetId, katilimciId);

            List<string> idList1 = objeList.Select(l => l.AniObjesiId.ToString()).ToList();
            var firstNotSecond = idList1.Except(idList).ToList();
            DeleteFirstList(firstNotSecond, faaliyetId, katilimciId);
        }
        private void DeleteFirstList(List<string> list, int faaliyetId, int katilimciId)
        {
            string joined = string.Join(",", list.ToArray());
            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            aniObjesiDagitim.Delete(faaliyetId, katilimciId, joined);
        }
        private string AniObjeleriniGetir(int katilimciId, int faaliyetId, string stokluMu)
        {
            AniObjesiDagitim aniobjesiDagitim = new AniObjesiDagitim();
            DataTable dataTable = aniobjesiDagitim.SelectByKatilimcidFaaliyetId(katilimciId, faaliyetId, stokluMu);
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
        private string GetirilenAniObjeleriniGetir(int katilimciId, int faaliyetId)
        {
            AniObjesiDagitim aniobjesiDagitim = new AniObjesiDagitim();
            string aniObjeleri = aniobjesiDagitim.SelectGetirilenByKatilimcidFaaliyetId(katilimciId, faaliyetId);
            return (aniObjeleri);
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
        #endregion
        #region Takvim Daveti
        private string TakvimDavetiBtnOlustur(int katilimId,string takvimDaveti, int katilimciId, string eposta)
        {
            string btn = string.Empty;
            if (!string.IsNullOrEmpty(eposta))
            {
                FaaliyetKatilim faaliyetKatilim = new FaaliyetKatilim();
                faaliyetKatilim = faaliyetKatilim.Select(katilimId);
                if (faaliyetKatilim != null && faaliyetKatilim.TakvimDaveti.Equals(ProjeConstants.MTSTAKVIMDAVETIYESI_GONDERILDI))
                {
                    btn =  ProjeConstants.MTSTAKVIMDAVETIYESI_GONDERILDI;
                }
                else
                {
                    btn = "<a href='#' class='btn btn-outline-info' onclick=TakvimDavetiyesiModalAc(" + katilimId + "," + FaaliyetIdQS + "," + katilimciId + "," + "," + eposta.ReturnQuotedValue() + ")>Davet Gönder</a>" +
                                (string.IsNullOrEmpty(takvimDaveti) || takvimDaveti.Equals(ProjeConstants.MTSTAKVIMDAVETIYESI_GONDERILMEDI) ? string.Empty : "<br>" + ProjeConstants.MTSTAKVIMDAVETIYESI_GONDERILDI) + "";
                }
            }
            else
            {
                btn = "E-posta adresi yok";
            }


            return btn;
        }
        private bool TakvimDavetiGonder(FaaliyetKatilim faaliyetKatilim, int faaliyetId, string toAdress, string islemTipi)
        {
            bool gonderildi = false;
            Faaliyet faaliyet = new Faaliyet();
            faaliyet = faaliyet.Select(faaliyetId);
            if (faaliyet != null)
            {
                string from = ProjeConstants.PARAM_MTSMAILADRESI;
                string userto = toAdress;
                string baslik = faaliyet.FaaliyetKonusu;
                string faaliyetYeriStr = faaliyet.FaaliyetYeriStr;



                if (faaliyetKatilim == null 
                    || string.IsNullOrEmpty(faaliyetKatilim.TakvimDaveti) 
                    || faaliyetKatilim.TakvimDaveti.Equals(ProjeConstants.MTSTAKVIMDAVETIYESI_GONDERILMEDI))
                {
                    islemTipi = ProjeConstants.KAYDET;
                }
                
                if (islemTipi.Equals(ProjeConstants.KAYDET))
                {
                    if (AcikTarihChk.Checked)
                    {
                        MessageHelper.PublishMessage("Açik Tarihli faaliyet oldugundan takvim daveti gönderilmedi", ProjeConstants.MESAJ_HATA);
                    }
                    else{
                        MailHelper.TakvimeEkle(faaliyet.UniqueId, from, userto, baslik, faaliyet.BaslangicTarihi, faaliyet.BitisTarihi, faaliyetYeriStr,
                                        faaliyet.Aciklama, ProjeConstants.PARAM_INTERNET_SMTP_IP_ADRESI);
                        gonderildi = true;
                    }

                }
                else if (islemTipi.Equals(ProjeConstants.GUNCELLE))
                {
                    if (AcikTarihChk.Checked)
                    {
                        MailHelper.TakvimdenSil(faaliyet.UniqueId, from, userto, " -Açik Tarihe Alindi- " + baslik, faaliyet.BaslangicTarihi, faaliyet.BitisTarihi, faaliyetYeriStr,
                            faaliyet.Aciklama, ProjeConstants.PARAM_INTERNET_SMTP_IP_ADRESI);
                        gonderildi = true;
                    }
                    else
                    {
                        MailHelper.TakvimeEkle(faaliyet.UniqueId, from, userto, baslik, faaliyet.BaslangicTarihi, faaliyet.BitisTarihi, faaliyetYeriStr,
                            faaliyet.Aciklama, ProjeConstants.PARAM_INTERNET_SMTP_IP_ADRESI);
                        gonderildi = true;
                    }
                }
                else if (islemTipi.Equals(ProjeConstants.SIL))
                {
                    MailHelper.TakvimdenSil(faaliyet.UniqueId, from, userto, baslik, faaliyet.BaslangicTarihi, faaliyet.BitisTarihi, faaliyetYeriStr,
                        faaliyet.Aciklama, ProjeConstants.PARAM_INTERNET_SMTP_IP_ADRESI);
                    gonderildi = true;
                }

            }
            else
            {
                MessageHelper.PublishMessage("Takvim daveti gönderilmedi", ProjeConstants.MESAJ_BILGI, 2000);
            }
            return gonderildi;
        }
        protected void TakvimDavetiGonderNowBtn_Click(object sender, EventArgs e)
        {
            int katilimId = paramFaaliyetKatilimIdLbl.Value.ConvertToInt();
            int katilimciId = paramFaaliyetKatilimciIdLbl.Value.ConvertToInt();
            int faaliyetId = paramFaaliyetIdLbl.Value.ConvertToInt();

            TakvimDavetiHazirla(faaliyetId,katilimId, katilimciId, ProjeConstants.GUNCELLE);
        }

        private void TakvimDavetiHazirla(int faaliyetId,int katilimId, int katilimciId, string islemTipi)
        {
            Katilimci katilimci = new Katilimci();
            katilimci = katilimci.GetKatilimci(katilimciId, ProjeConstants.FAALIYET_KATILIMCI_DIS_INT);
            if (katilimci != null)
            {
                string epostaAdresi = katilimci != null ? katilimci.EPosta : string.Empty;

                if (string.IsNullOrEmpty(epostaAdresi))
                {
                    MessageHelper.PublishMessage("Katilimcinin e-posta adresi bulunmuyor", ProjeConstants.MESAJ_HATA);
                }
                else
                {
                    try
                    {
                        FaaliyetKatilim faaliyetKatilim = new FaaliyetKatilim();
                        faaliyetKatilim = faaliyetKatilim.Select(katilimId);
                        if (faaliyetKatilim != null)
                        {
                            TakvimDavetiGonder(faaliyetKatilim, faaliyetId, epostaAdresi, islemTipi);
                            faaliyetKatilim.TakvimDaveti = ProjeConstants.MTSTAKVIMDAVETIYESI_GONDERILDI;
                            faaliyetKatilim.Update();
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
                    }
                }
            }
            else
            {
                MessageHelper.PublishMessage("Katilimci bulunamadigindan takvim daveti gönderilmedi", ProjeConstants.MESAJ_HATA);
            }
        }

        private void KatilimcilaraTakvimDavetiGonder(Faaliyet faaliyet)
        {
            FaaliyetKatilim faaliyetKatilimDao = new FaaliyetKatilim();
            List<FaaliyetKatilim> list= faaliyetKatilimDao.SelectByFaaliyetId(faaliyet.Id);
            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.TakvimDaveti) || item.TakvimDaveti.Equals(ProjeConstants.MTSTAKVIMDAVETIYESI_GONDERILMEDI))
                {

                }
                else
                {
                    TakvimDavetiHazirla(item.FaaliyetId, item.Id, item.KatilimciId, ProjeConstants.GUNCELLE);
                }
            }

        }
        #endregion

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
                    //faaliyet degisti
                    DisableComponents();
                    AddRefreshLink(FaaliyetIdQS.ConvertToInt(), TopBarDiv, "Faaliyet bilgileri degisti... Sayfayi yenilemek için lütfen tiklayin.", tempFaaliyetSonHali);
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
                    AddRefreshLink(FaaliyetIdQS.ConvertToInt(), TopBarDiv, "Katilimci bilgileri degisti... Listeyi yenilemek için lütfen tiklayin.", tempFaaliyetSonHali);
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
            FaaliyetYeriTxt.Enabled = false;
            FaaliyetDurumuDDL.Enabled = false;
            FaaliyetKonusuTxt.Enabled = false;
            TumGunChk.Enabled = false;
            AcikTarihChk.Enabled = false;
            AciklamaTxt.Enabled = false;
            KaydetBtn.Visible = false;
            GuncelleBtn.Visible = false;
            DisIrtibatCikarBtn.Visible = false;
        }
        private HyperLink HyperLinkGetir(int faaliyetId, string message)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            HyperLink hyperLink = new HyperLink
            {
                Text = message,
                CssClass = "form-control btn btn-warning fw-bold",
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
            public string AdiSoyadi { get; set; }
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public string Kurumu { get; set; }
            public string AniObjesi { get; set; }
            public string AniObjesiStoklu { get; set; }
            public string AniObjesiStoksuz { get; set; }
            public string GetirilenAniObjesi { get; set; }
            public string TakvimDavetiBtn{ get; set; }
            public string EPosta{ get; set; }
            
            public string KisiKarti { get; set; }
            public string Cikar { get; set; }
            public string KatilimciSec { get; set; }
            public string IrtibatSec { get; set; }
            public bool RandevuKisiti { get; set; } = false;

        }
        [Serializable]
        private class TempFaaliyet : IEquatable<TempFaaliyet>
        {
            public TempFaaliyet(Faaliyet faaliyet)
            {
                Id = faaliyet.Id;
                FaaliyetTipi = faaliyet.FaaliyetTipi;
                FaaliyetAmaciId = faaliyet.FaaliyetAmaciId;
                FaaliyetKonusu = faaliyet.FaaliyetKonusu;
                FaaliyetYeriStr = faaliyet.FaaliyetYeriStr;
                FaaliyetDurumu = faaliyet.FaaliyetDurumu;
                TumGun = faaliyet.TumGun;
                AcikTarih = faaliyet.AcikTarih;
                DisIrtibatId = faaliyet.DisIrtibatId;
                BaslangicTarihi = faaliyet.BaslangicTarihi;
                BitisTarihi = faaliyet.BitisTarihi;
                BaslangicSaati = faaliyet.BaslangicSaati;
                BitisSaati = faaliyet.BitisSaati;
                Aciklama = faaliyet.Aciklama;
                YoneticiNotu = faaliyet.YoneticiNotu;
                TakvimeIslendi = faaliyet.TakvimeIslendi;
            }

            public int Id { get; set; }
            public string FaaliyetTipi { get; set; }
            public int FaaliyetAmaciId { get; set; }
            public string FaaliyetKonusu { get; set; }
            public string FaaliyetYeriStr { get; set; }
            public int FaaliyetDurumu { get; set; }
            public bool TumGun { get; set; }
            public bool AcikTarih { get; set; }
            public int DisIrtibatId { get; set; }
            public DateTime BaslangicTarihi { get; set; }
            public DateTime BitisTarihi { get; set; }
            public string BaslangicSaati { get; set; }
            public string BitisSaati { get; set; }
            public string Aciklama { get; set; }
            public string YoneticiNotu { get; set; }
            public bool TakvimeIslendi{ get; set; }
            public override bool Equals(object obj)
            {
                var other = obj as Faaliyet;

                if (other == null)
                    return false;

                if (FaaliyetTipi != other.FaaliyetTipi
                    || FaaliyetAmaciId != other.FaaliyetAmaciId
                    || FaaliyetKonusu != other.FaaliyetKonusu
                    || !FaaliyetYeriStr.Equals(other.FaaliyetYeriStr)
                    || TumGun != other.TumGun
                    || AcikTarih != other.AcikTarih
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
                       FaaliyetAmaciId == other.FaaliyetAmaciId &&
                       FaaliyetKonusu == other.FaaliyetKonusu &&
                       FaaliyetYeriStr == other.FaaliyetYeriStr &&
                       FaaliyetDurumu == other.FaaliyetDurumu &&
                       TumGun == other.TumGun &&
                       AcikTarih == other.AcikTarih &&
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
                hashCode = hashCode * -1521134295 + FaaliyetAmaciId.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FaaliyetKonusu);
                hashCode = hashCode * -1521134295 + FaaliyetYeriStr.GetHashCode();
                hashCode = hashCode * -1521134295 + FaaliyetDurumu.GetHashCode();
                hashCode = hashCode * -1521134295 + TumGun.GetHashCode();
                hashCode = hashCode * -1521134295 + AcikTarih.GetHashCode();
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
