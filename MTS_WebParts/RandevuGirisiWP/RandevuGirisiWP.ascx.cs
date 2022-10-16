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


namespace MTS_WebParts.RandevuGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class RandevuGirisiWP : WebPart
    {

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public RandevuGirisiWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string RandevuIdQS
        {
            get
            {

                if (ViewState["RandevuId"] == null)
                {
                    if (Page.Request.QueryString["RandevuId"] != null)
                    {
                        ViewState["RandevuId"] = Page.Request.QueryString["RandevuId"];
                    }
                    else
                    {
                        ViewState["RandevuId"] = string.Empty;
                    }
                }
                return ViewState["RandevuId"].ToString();
            }

            set
            {
                ViewState["RandevuId"] = value;
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
                if (!Page.IsPostBack) // sayfa ilk kez açılıyorsa (bu sayfanın içindeki butona basılma anı hariç)
                {
                    fillRandevuTipiDDL();
                    fillRandevuAmaciDDL();
                    fillRandevuDurumuDDL();
                    fillRandevuYeriDDL();
                    BaslangicSaatiDDLDoldur();
                    BitisSaatiDDLDoldur();
                    if (string.IsNullOrEmpty(RandevuIdQS))
                    {
                        GirisiAc();
                    }
                    else
                    {
                        DuzenleAc();
                        KatilimciBilgileriniDoldur();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exhelper = new ExceptionHelper(ex);
                exhelper.PublishException();
            }
        }
        private void RandevuFormunuDoldur()
        {
            Randevu randevu = new Randevu();
            randevu = randevu.Select(RandevuIdQS.ConvertToInt());
            if (randevu != null)
            {
                IdLbl.Text = " ( Randevu No: " + randevu.Id.ToString() + " )";
                TumGunChk.Checked = randevu.TumGun;
                AcikTarihChk.Checked = randevu.AcikTarih;
                RandevuKonusuTxt.Text = randevu.RandevuKonusu;
                BaslangicTarihiTxt.Text = randevu.BaslangicTarihi.ConvertToDatetimeEmptyIfNull();
                InitialDateQS = randevu.BaslangicTarihi.ToString("yyyy-MM-dd");
                BitisTarihiTxt.Text = randevu.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                AciklamaTxt.Text = randevu.Aciklama;

                UtilityHelper.SetDDLValue(RandevuTipiDDL, randevu.RandevuTipi);
                UtilityHelper.SetDDLValue(RandevuAmaciDDL, randevu.RandevuAmaci.ToString());
                UtilityHelper.SetDDLValue(RandevuDurumuDDL, randevu.RandevuDurumu.ToString());
                UtilityHelper.SetDDLValue(RandevuYeriDDL, randevu.RandevuYeri.ToString());
                UtilityHelper.SetDDLValue(BasSaatDDL, randevu.BaslangicSaati);
                UtilityHelper.SetDDLValue(BitSaatDDL, randevu.BitisSaati);

                IcIrtibatIdQS = randevu.IcIrtibatId.ToString();
                DisIrtibatIdQS = randevu.DisIrtibatId.ToString();

                //RandevuDurumuImg.ImageUrl = RandevuDurumuImgGetir(RandevuDurumuDDL.SelectedItem.Value.ConvertToInt());


            }

        }
        private void KatilimciBilgileriniDoldur()
        {
            Randevu randevu = new Randevu();
            randevu = randevu.Select(RandevuIdQS.ConvertToInt());
            if (randevu != null)
            {
                AramaGorusmeBilgileriniDoldur(randevu);
                IcIrtibatNoktasiDoldur(randevu.IcIrtibatId);
                DisIrtibatNoktasiDoldur(randevu.DisIrtibatId);
                TabloOlustur();
            }

        }
        private void GirisiAc()
        {
            KatilimciBilgileriDiv.Attributes["style"] = "display:none";
            KaydetBtn.Visible = true;
            GuncelleBtn.Visible = false;
            RandevuSilBtn.Visible = false;
            RandevuKartiBtn.Visible = false;
            TitleLbl.Text = "Yeni Faaliyet";
            TitleLbl.CssClass = "col-form-label text-success font-weight-bold mb-1";
            BaslangicTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
            BitisTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
        }
        private void DuzenleAc()
        {
            if (RandevuIdQS.ConvertToInt() > 0)
            {
                KatilimciBilgileriDiv.Attributes["style"] = "display:block";
                KaydetBtn.Visible = false;
                GuncelleBtn.Visible = true;
                RandevuKartiBtn.Visible = true;
                //RandevuSilBtn.Visible = true;
                RandevuFormunuDoldur();
                if (!string.IsNullOrEmpty(MesajQS))
                {
                    MessageHelper.PublishMessage("Faaliyet Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                    MesajQS = string.Empty;
                }
            }
            else
            {
                GirisiAc();
                MessageHelper.PublishMessage("Faaliyet bulunamadı. Yeni randevu girebilirsiniz.", ProjeConstants.MESAJ_BILGI, 2000);
            }

        }
        private void BaslangicSaatiDDLDoldur()
        {
            BasSaatDDL.Items.Clear();
            TimeSpan bastarTS = new TimeSpan(6, 0, 0);
            TimeSpan aralikTS = new TimeSpan(0, 30, 0);
            TimeSpan bittarTS = new TimeSpan(20, 0, 0);

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
            TimeSpan aralikTS = new TimeSpan(0, 30, 0);
            TimeSpan bittarTS = new TimeSpan(20, 0, 0);

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
            int randevuId = 0;
            try
            {
                Randevu randevu = new Randevu();
                randevu.RandevuTipi = RandevuTipiDDL.SelectedItem.Text;
                randevu.RandevuAmaci = RandevuAmaciDDL.SelectedItem.Value.ConvertToInt();
                randevu.RandevuKonusu = RandevuKonusuTxt.Text;
                randevu.RandevuYeri = RandevuYeriDDL.SelectedItem.Value.ConvertToInt();
                randevu.RandevuDurumu = RandevuDurumuDDL.SelectedItem.Value.ConvertToInt();
                randevu.TumGun = TumGunChk.Checked.ConvertToBool();
                randevu.AcikTarih = AcikTarihChk.Checked.ConvertToBool();
                DateTime baslangictarihi = BaslangicTarihiTxt.Text.ConvertToDatetime();
                DateTime bitistarihi = BitisTarihiTxt.Text.ConvertToDatetime();
                string bassaat = BasSaatDDL.SelectedItem.Value;
                string bitsaat = BitSaatDDL.SelectedItem.Value;
                randevu.BaslangicTarihi = UtilityHelper.TariheSaatEkle(baslangictarihi, bassaat);
                randevu.BitisTarihi = UtilityHelper.TariheSaatEkle(bitistarihi, bitsaat);
                randevu.BaslangicSaati = bassaat;
                randevu.BitisSaati = bitsaat;
                randevu.Aciklama = AciklamaTxt.Text;
                randevu.IcIrtibatId = IcIrtibatIdQS.ConvertToInt();
                randevu.DisIrtibatId = DisIrtibatIdQS.ConvertToInt();
                randevu.Olusturan = UtilityHelper.GetCurrentUser();

                if (string.IsNullOrEmpty(RandevuKonusuTxt.Text))
                {
                    MessageHelper.PublishMessage("Faaliyet Konusu Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
                }
                else
                {
                    randevuId = randevu.Id = randevu.Save();
                    if (randevuId > 0)
                    {
                        RandevuIdQS = randevuId.ToString();
                        RedirectToPage(ProjeConstants.PAGE_RANDEVU_GIRIS + "?RandevuId=" + RandevuIdQS + "&Mesaj=true");
                        MessageHelper.PublishMessage("Faaliyet Kaydedildi.", ProjeConstants.MESAJ_BASARILI, 2000);
                        KatilimciBilgileriDiv.Attributes["style"] = "display:block";
                        InitialDateQS = randevu.BaslangicTarihi.ToString("yyyy-MM-dd");
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
        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            bool guncellendiMi = false;
            try
            {

                Randevu randevu = new Randevu();
                randevu = randevu.Select(RandevuIdQS.ConvertToInt());
                randevu.Degistiren = UtilityHelper.GetCurrentUser();
                if (randevu == null)
                {
                    MessageHelper.PublishMessage("Faaliyet bulunamadı", ProjeConstants.MESAJ_HATA, 5000);
                }
                else
                {
                    randevu.RandevuTipi = RandevuTipiDDL.SelectedItem.Text;
                    randevu.RandevuAmaci = RandevuAmaciDDL.SelectedItem.Value.ConvertToInt();
                    randevu.RandevuKonusu = RandevuKonusuTxt.Text;
                    randevu.RandevuYeri = RandevuYeriDDL.SelectedItem.Value.ConvertToInt();
                    randevu.RandevuDurumu = RandevuDurumuDDL.SelectedItem.Value.ConvertToInt();
                    randevu.TumGun = TumGunChk.Checked.ConvertToBool();
                    randevu.AcikTarih = AcikTarihChk.Checked.ConvertToBool();
                    DateTime baslangictarihi = BaslangicTarihiTxt.Text.ConvertToDatetime();
                    DateTime bitistarihi = BitisTarihiTxt.Text.ConvertToDatetime();
                    string bassaat = BasSaatDDL.SelectedItem.Value;
                    string bitsaat = BitSaatDDL.SelectedItem.Value;
                    randevu.BaslangicTarihi = UtilityHelper.TariheSaatEkle(baslangictarihi, bassaat);
                    randevu.BitisTarihi = UtilityHelper.TariheSaatEkle(bitistarihi, bitsaat);
                    randevu.BaslangicSaati = bassaat;
                    randevu.BitisSaati = bitsaat;
                    randevu.Aciklama = AciklamaTxt.Text;
                    randevu.IcIrtibatId = IcIrtibatIdQS.ConvertToInt();
                    randevu.DisIrtibatId = DisIrtibatIdQS.ConvertToInt();
                    guncellendiMi = randevu.Update();

                    if (guncellendiMi)
                    {
                        InitialDateQS = randevu.BaslangicTarihi.ToString("yyyy-MM-dd");
                        //KatilimciBilgileriniDoldur();
                        MessageHelper.PublishMessage("Faaliyet güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
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
                exhelper.Exceptions.Add(new Exception("Duyuru güncellenemedi."));
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
        protected void RandevuSilBtn_Click(object sender, EventArgs e)
        {
            //Randevu Silme Kaldırıldı SB 31.05.2021
            //try
            //{
            //    Randevu randevu = new Randevu();
            //    randevu = randevu.Select(RandevuIdQS.ConvertToInt());
            //    if (randevu != null)
            //    {
            //        RandevuSilPopupAc(sender);
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
        protected void KisiListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KISI_LIST);
        }
        protected void RandevuListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_RANDEVU_LIST + "?SecilenId=" + RandevuIdQS);
        }
        protected void RandevuTakvimiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM + "?InitialDate=" + InitialDateQS);
        }
        private void RandevuSilPopupAc(object sender)
        {
            ParamVnLbl.Text = RandevuIdQS;
            string openModal = "OpenSilModal();";
            SilMesajiLbl.Visible = true;
            SilMesajiLbl.Text = "Faaliyete ait tüm bilgiler, katılımcılar ve anı objeleri silinecektir. Silmek istediğinizden eminmisiniz?";
            SilModalBaslikLbl.Text = "Faaliyet Silinecek";
            RandevuSilNowBtn.Visible = true;
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openModal, true);
        }
        /// RandevuKatilim_Tabeledan bu randevu Id'li kayıtları sil
        /// AramaGorusme_Table'da bu randevuId'li kayıtların randevuId'sini 0 yap
        /// AniObjesiDagitim_Table'dan bu randevuId'li olanları sil
        protected void RandevuSilNowBtn_Click(object sender, EventArgs e)
        {
            bool silindi = false;
            try
            {
                Randevu randevu = new Randevu();
                randevu = randevu.Select(RandevuIdQS.ConvertToInt());
                if (randevu != null)
                {

                    silindi = randevu.Delete();
                    if (silindi)
                    {
                        RandevuKatilim randevuKatilim = new RandevuKatilim();
                        bool katilimSilindi = randevuKatilim.DeleteByRandevuId(RandevuIdQS.ConvertToInt());
                        //AramaGorusme aramaGorusme = new AramaGorusme();
                        //bool agIslendi=aramaGorusme.UpdateRandevuId(RandevuIdQS.ConvertToInt(),"Randevu Silindi");
                        //AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
                        //bool aniObjeleriSilindi = aniObjesiDagitim.DeleteByRandevuId(RandevuIdQS.ConvertToInt());
                    }

                    RedirectToPage(ProjeConstants.PAGE_RANDEVU_GIRIS);
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
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "KatilimciSecimiModal();", true);
        }
        protected void SecilenKatilimciyiKaydetNowBtn_Click(object sender, EventArgs e)
        {
            int katilimciId = paramRandevuKatilimciIdLbl.Value.ConvertToInt();
            int randevuId = paramRandevuIdLbl.Value.ConvertToInt();
            int katilimciTipi = paramRandevuKatilimciTipiLbl.Value.ConvertToInt();
            if (katilimciId > 0)
            {
                RandevuKatilim randevuKatilim = new RandevuKatilim();

                List<RandevuKatilim> list = randevuKatilim.Select(randevuId, katilimciId, katilimciTipi);
                if (list.Count < 1)
                {
                    randevuKatilim.KatilimciId = katilimciId;
                    randevuKatilim.RandevuId = randevuId;
                    randevuKatilim.KatilimciTipi = katilimciTipi;
                    randevuKatilim.Save();
                }
            }
            else
            {
                MessageHelper.PublishMessage("Katılımcı Bulunamadı", ProjeConstants.MESAJ_HATA);
            }

            KatilimciBilgileriniDoldur();
        }
        //katılımcı seçme ve ekleme
        protected void KatilimciEkleBtn_Click(object sender, EventArgs e)
        {
            KatilimciModalAc(ProjeConstants.RANDEVU_KATILIMCI_IC_INT);
        }
        protected void KatilimciCikarBtn_Click(object sender, EventArgs e)
        {
            int randevukatilimId = paramRandevuKatilimIdLbl.Value.ConvertToInt();
            if (randevukatilimId > 0)
            {
                RandevuKatilim randevuKatilim = new RandevuKatilim();
                randevuKatilim = randevuKatilim.Select(randevukatilimId);
                if (randevuKatilim != null)
                {
                    if (randevuKatilim.Delete())
                    {
                        AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
                        bool aniObjeleriSilindi = aniObjesiDagitim.Delete(randevuKatilim.RandevuId, randevuKatilim.KatilimciId, randevuKatilim.KatilimciTipi);
                    }
                }

            }
            else
            {
                MessageHelper.PublishMessage("Katılımcı Bulunamadı", ProjeConstants.MESAJ_HATA);
            }
            KatilimciBilgileriniDoldur();
        }
        private void fillRandevuTipiDDL()
        {
            ListItem li = new ListItem(ProjeConstants.RANDEVU_VERILEN);
            ListItem li2 = new ListItem(ProjeConstants.RANDEVU_ALINAN);

            RandevuTipiDDL.Items.Clear();
            RandevuTipiDDL.Items.Add(li);
            RandevuTipiDDL.Items.Add(li2);
        }
        private void fillRandevuAmaciDDL()
        {
            ListItem li5 = new ListItem(ProjeConstants.RANDEVU_AMACI_TOPLANTI, ProjeConstants.RANDEVU_AMACI_TOPLANTI_INT);
            ListItem li = new ListItem(ProjeConstants.RANDEVU_AMACI_ZIYARET, ProjeConstants.RANDEVU_AMACI_ZIYARET_INT);
            ListItem li2 = new ListItem(ProjeConstants.RANDEVU_AMACI_DAVET, ProjeConstants.RANDEVU_AMACI_DAVET_INT);
            ListItem li3 = new ListItem(ProjeConstants.RANDEVU_AMACI_YILDONUMU, ProjeConstants.RANDEVU_AMACI_YILDONUMU_INT);
            ListItem li4 = new ListItem(ProjeConstants.RANDEVU_AMACI_OZELCALISMA, ProjeConstants.RANDEVU_AMACI_OZELCALISMA_INT);
            ListItem li6 = new ListItem(ProjeConstants.RANDEVU_AMACI_IZIN, ProjeConstants.RANDEVU_AMACI_IZIN_INT);


            RandevuAmaciDDL.Items.Clear();
            RandevuAmaciDDL.Items.Add(li5);
            RandevuAmaciDDL.Items.Add(li);
            RandevuAmaciDDL.Items.Add(li2);
            RandevuAmaciDDL.Items.Add(li3);
            RandevuAmaciDDL.Items.Add(li4);
            RandevuAmaciDDL.Items.Add(li6);
        }
        private void fillRandevuDurumuDDL()
        {
            ListItem li = new ListItem(ProjeConstants.RANDEVU_DURUMU_PLANLANDI, ProjeConstants.RANDEVU_DURUMU_PLANLANDI_INT.ToString());
            ListItem li2 = new ListItem(ProjeConstants.RANDEVU_DURUMU_ONAYLANDI, ProjeConstants.RANDEVU_DURUMU_ONAYLANDI_INT.ToString());
            ListItem li3 = new ListItem(ProjeConstants.RANDEVU_DURUMU_IPTALEDILDI, ProjeConstants.RANDEVU_DURUMU_IPTALEDILDI_INT.ToString());

            RandevuDurumuDDL.Items.Clear();
            RandevuDurumuDDL.Items.Add(li);
            RandevuDurumuDDL.Items.Add(li2);
            RandevuDurumuDDL.Items.Add(li3);
        }
        private void fillRandevuYeriDDL()
        {
            RandevuYeriDDL.Items.Clear();
            RandevuParametre randevuParametre = new RandevuParametre();
            List<RandevuParametre> list = randevuParametre.SelectByGrupReturnList(ProjeConstants.PARAM_RANDEVUYERI);
            foreach (var item in list)
            {
                ListItem li = new ListItem(item.Deger, item.Id.ToString());
                RandevuYeriDDL.Items.Add(li);
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
        private void IcIrtibatNoktasiDoldur(int personelId)
        {
            Personel personel = new Personel();
            personel = personel.Select(personelId);
            if (personel != null)
            {
                IcIrtibatIdQS = personel.Id.ToString();
                IcIrtibatLbl.Text = "İç İrtibat Noktası : " + personel.Adi + " " + personel.Soyadi;
                IcIrtibatCikarBtn.Visible = true;
            }
            else
            {
                IcIrtibatLbl.Text = string.Empty;
                IcIrtibatCikarBtn.Visible = false;
            }
        }
        private void DisIrtibatNoktasiDoldur(int kisiId)
        {
            Kisi kisi = new Kisi();
            kisi = kisi.Select(kisiId);
            if (kisi != null)
            {
                DisIrtibatLbl.Text = "Dış İrtibat Noktası : " + kisi.Adi + " " + kisi.Soyadi;
                DisIrtibatIdQS = kisi.Id.ToString();
                DisIrtibatCikarBtn.Visible = true;
            }
            else
            {
                DisIrtibatLbl.Text = string.Empty;
                DisIrtibatCikarBtn.Visible = false;
            }
        }
        protected void IrtibatSecBtn_Click(object sender, EventArgs e)
        {
            int katilimciId = paramRandevuKatilimciIdLbl.Value.ConvertToInt();
            if (katilimciId > 0)
            {
                Randevu randevu = new Randevu();
                randevu = randevu.Select(RandevuIdQS.ConvertToInt());
                if (randevu != null)
                {

                    if (paramRandevuKatilimciTipiLbl.Value.ConvertToInt() == ProjeConstants.RANDEVU_KATILIMCI_IC_INT)
                    {
                        randevu.IcIrtibatId = katilimciId;
                    }
                    else
                    {
                        randevu.DisIrtibatId = katilimciId;
                    }

                    randevu.Update();
                }
            }
            KatilimciBilgileriniDoldur();
        }
        protected void RandevuDurumuDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            //RandevuDurumuImg.ImageUrl = RandevuDurumuImgGetir(RandevuDurumuDDL.SelectedItem.Value.ConvertToInt());
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"jQuery('#CustomDataTable').DataTable({
            data: " + jsonData + @",
            columns: [
                { data: 'Adi' },
                { data: 'Soyadi' },
                { data: 'Kurumu' },
                { data: 'KatilimciTipi', visible: false },
                { data: 'KatilimciTipiStr' },
                { data: 'AniObjesi' },
                { data: 'KisiKarti' },
                { data: 'Cikar' },
            ],
            'order': [[3, 'desc']],
            'language': {
                'url': 'http://tskgv-portal/OrtakBelgeler/Turkish.txt',
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
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<KatilimciListItem> list = GetDataList();
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
        private List<KatilimciListItem> GetDataList()
        {
            Randevu randevuDao = new Randevu();
            System.Data.DataTable dataTable = randevuDao.SelectAllByKatilimciRandevuReturnDataTable(RandevuIdQS.ConvertToInt(),ProjeConstants.HEPSI_INT);
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
                    katilimciItem.Kurumu = kurumu;
                    katilimciItem.KatilimciTipi = katilimciTipi.ToString();
                    katilimciItem.KatilimciTipiStr = KatilimciTipiGetir(katilimciTipi);
                    if (katilimciItem.KatilimciTipi.ConvertToInt() == ProjeConstants.RANDEVU_KATILIMCI_IC_INT)
                    {
                        katilimciItem.AniObjesi = string.Empty;
                    }
                    else
                    {
                        katilimciItem.AniObjesi = "<a href='#' class='btn btn-outline-primary' onclick=AniObjesiBtnClick(" + katilimciId + "," + RandevuIdQS + "," + katilimciTipi + ")>Anı Objesi Ekle</a>";

                    }

                    katilimciItem.KisiKarti = "<a  target='_blank' href=" + ProjeConstants.PAGE_KISI_KARTI + "?KatilimciId=" + katilimciId + "&KatilimciTipi=" + katilimciTipi + " class='btn btn-outline-info'>Kişi Kartı</a>";
                    katilimciItem.Cikar = "<a href='#' class='btn btn-outline-danger' onclick=KatilimciCikarBtnClick(" + katilimId + ")>Çıkar</a>";

                    list.Add(katilimciItem);
                }
            }
            return list;
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
                'url': 'http://tskgv-portal/OrtakBelgeler/Turkish.txt',
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
            DataTable dataTableIc = personel.SelectSecilmemisIcKatilimcilarByRandevuIdReturnDT(RandevuIdQS.ConvertToInt());
            Kisi kisi = new Kisi();
            DataTable dataTableDis = kisi.SelectSecilmemisDisKatilimcilarByRandevuIdReturnDT(RandevuIdQS.ConvertToInt());
            NakitBagisci nakitBagisci = new NakitBagisci();
            DataTable dataTableNakit = nakitBagisci.SelectSecilmemisKatilimcilarByRandevuIdReturnDT(RandevuIdQS.ConvertToInt());
            TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
            DataTable dataTableTasinmaz = tasinmazBagisci.SelectSecilmemisKatilimcilarByRandevuIdReturnDT(RandevuIdQS.ConvertToInt()); //veri çekilip json a çeviriliyor

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


                    katilimciItem.KatilimciSec = "<a href='#' class='btn btn-outline-primary' onclick=KatilimciSecildiBtnClick(" + katilimciId + "," + RandevuIdQS + "," + katilimciTipi + ")>Faaliyete Ekle</a>";
                    if (katilimciTipi == ProjeConstants.RANDEVU_KATILIMCI_IC_INT ||
                        katilimciTipi == ProjeConstants.RANDEVU_KATILIMCI_DIS_INT)
                    {
                        katilimciItem.IrtibatSec = "<a href='#' class='btn btn-outline-primary' onclick=IrtibatSecBtnClick(" + katilimciId + "," + RandevuIdQS + "," + katilimciTipi + ")>İrtibat Ekle</a>";
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
            string katilimciTipStr = ProjeConstants.RANDEVU_KATILIMCI_DIS;
            switch (katilimciTipi)
            {
                case ProjeConstants.RANDEVU_KATILIMCI_IC_INT:
                    {
                        katilimciTipStr = ProjeConstants.RANDEVU_KATILIMCI_IC;
                        break;
                    }
                case ProjeConstants.RANDEVU_KATILIMCI_DIS_INT:
                    {
                        katilimciTipStr = ProjeConstants.RANDEVU_KATILIMCI_DIS;
                        break;
                    }
                case ProjeConstants.RANDEVU_KATILIMCI_NAKITBAGISCI_INT:
                    {
                        katilimciTipStr = ProjeConstants.RANDEVU_KATILIMCI_NAKITBAGISCI;
                        break;
                    }
                case ProjeConstants.RANDEVU_KATILIMCI_TASINMAZBAGISCI_INT:
                    {
                        katilimciTipStr = ProjeConstants.RANDEVU_KATILIMCI_TASINMAZBAGISCI;
                        break;
                    }
                default:
                    break;
            }
            return katilimciTipStr;
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
        protected void IcIrtibatCikarBtn_Click(object sender, EventArgs e)
        {
            Randevu randevu = new Randevu();
            randevu = randevu.Select(RandevuIdQS.ConvertToInt());
            if (randevu != null)
            {
                randevu.IcIrtibatId = 0;
                randevu.Update();
                IcIrtibatLbl.Text = string.Empty;
                IcIrtibatCikarBtn.Visible = false;
                KatilimciBilgileriniDoldur();
            }
        }
        protected void DisIrtibatCikarBtn_Click(object sender, EventArgs e)
        {
            Randevu randevu = new Randevu();
            randevu = randevu.Select(RandevuIdQS.ConvertToInt());
            if (randevu != null)
            {
                randevu.DisIrtibatId = 0;
                randevu.Update();
                DisIrtibatLbl.Text = string.Empty;
                DisIrtibatCikarBtn.Visible = false;
                KatilimciBilgileriniDoldur();
            }
        }
        #region Arama/Gorusme bilgileri
        private void AramaGorusmeBilgileriniDoldur(Randevu randevu)
        {
            AramaGorusme aramaGorusme = new AramaGorusme();
            aramaGorusme = aramaGorusme.SelectByRandevuId(randevu.Id);
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
            int katilimciId = paramRandevuKatilimciIdLbl.Value.ConvertToInt();
            int randevuId = paramRandevuIdLbl.Value.ConvertToInt();
            int katilimciTipi = paramRandevuKatilimciTipiLbl.Value.ConvertToInt();
            if (katilimciId > 0)
            {
                if (katilimciTipi == ProjeConstants.RANDEVU_KATILIMCI_DIS_INT)
                {
                    Kisi kisi = new Kisi();
                    kisi = kisi.Select(katilimciId);
                    if (kisi != null)
                    {
                        AniObjesiHeaderLbl.InnerText = "Anı Objesi Seçimi (" + kisi.Adi + " " + kisi.Soyadi + ")";
                        AniObjeleriTablosunuDoldur(randevuId, katilimciId, katilimciTipi);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Kişi bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                }
                else if (katilimciTipi == ProjeConstants.RANDEVU_KATILIMCI_NAKITBAGISCI_INT)
                {
                    NakitBagisci nakitBagisci = new NakitBagisci();
                    nakitBagisci = nakitBagisci.Select<NakitBagisci>(katilimciId);
                    if (nakitBagisci != null)
                    {
                        AniObjesiHeaderLbl.InnerText = "Anı Objesi Seçimi (" + nakitBagisci.Adi + " " + nakitBagisci.Soyadi + ")";
                        AniObjeleriTablosunuDoldur(randevuId, katilimciId, katilimciTipi);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Nakit Bağışçı bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                }
                else if (katilimciTipi == ProjeConstants.RANDEVU_KATILIMCI_TASINMAZBAGISCI_INT)
                {
                    TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
                    tasinmazBagisci = tasinmazBagisci.Select<TasinmazBagisci>(katilimciId);
                    if (tasinmazBagisci != null)
                    {
                        AniObjesiHeaderLbl.InnerText = "Anı Objesi Seçimi (" + tasinmazBagisci.Adi + " " + tasinmazBagisci.Soyadi + ")";
                        AniObjeleriTablosunuDoldur(randevuId, katilimciId, katilimciTipi);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Taşınmaz Bağışçı bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                }
            }
            else
            {
                MessageHelper.PublishMessage("Katılımcı Bulunamadı", ProjeConstants.MESAJ_HATA);
            }
        }
        public JavaScriptSerializer javaSerial = new JavaScriptSerializer();
        /// <summary>
        /// Anı Objelerini modal pencereye getirir
        /// Seçilen katılımcı ve randevuya göre AniObjesiDagitim_Table ile RandevuParametre_Table join edilerek sorgulanır,
        /// Daha önceden işaretlenmiş olanlar frontend'de javascript arrayde tutulur ve ekrana dolu olarak gelir.
        /// Checkbox check değeri değiştiğinde,  sadece frontend deki array'e eklenir veya çıkarılır.
        /// Kaydete basıldığında array paramArray'e aktarılır, kaydetNowBtn click çalışır, devamı btn_click içinde yapılır
        /// </summary>
        /// <param name="randevuId"></param>
        /// <param name="katilimciId"></param>
        /// <param name="katilimciTipi"></param>
        private void AniObjeleriTablosunuDoldur(int randevuId, int katilimciId, int katilimciTipi)
        {
            paramRandevuIdLbl.Value = randevuId.ToString();
            paramRandevuKatilimciIdLbl.Value = katilimciId.ToString();
            paramRandevuKatilimciTipiLbl.Value = katilimciTipi.ToString();

            AniObjesiDagitim getirilenAniObjesi = new AniObjesiDagitim();
            getirilenAniObjesi = getirilenAniObjesi.SelectGetirilenAniObjesi(randevuId, katilimciId, katilimciTipi);
            if (getirilenAniObjesi != null)
            {
                GetirilenAniObjesiTxt.Text = getirilenAniObjesi.GetirilenAniObjesi;
            }

            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            DataTable dataTable = aniObjesiDagitim.SelectReturnDT(randevuId, katilimciId, katilimciTipi);
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
                    for (int i = 0; i < 3; i++)
                    {
                        row = dataTable.Rows[counter];

                        int aniObjesiId = row["AniObjesiId"].ReturnZeroIfNull().ConvertToInt();
                        int sira = row["Sira"].ReturnZeroIfNull().ConvertToInt();
                        string deger = row["Deger"].ToString();
                        int aniObjesiDagitimId = row["AniObjesiDagitimId"].ReturnZeroIfNull().ConvertToInt();
                        int adet = row["Adet"].ReturnZeroIfNull().ConvertToInt();

                        TableCell degerCell = new TableCell();

                        HtmlGenericControl newdiv = new HtmlGenericControl("DIV");
                        newdiv.Attributes.Add("class", "checkbox");

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
                        adetTxt.Attributes.Add("onchange", "ChangeAniObjesiAdet(" + aniObjesiId + "," + chkBox.ID.ReturnQuotedValue() + "," + adetTxt.Text + ",this);");

                        adetCell.Controls.Add(adetTxt);

                        chkBox.Attributes.Add("onclick", "AddRemoveAniObjesiIdToList(" + aniObjesiId + ",this," + adetTxt.Text + "," + adetTxt.ID.ReturnQuotedValue() + ");");

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

            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "AniObjesiModal();", true);
        }
        #endregion
        protected void RandevuKartiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_RANDEVU_KARTI + "?RandevuId=" + RandevuIdQS);
        }
        /// <summary>
        /// ilk olarak bu Randevu ve KatilimciId için AniObjesiDagitim_Table'daki kayıtları al (list1)
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
            int randevuId = paramRandevuIdLbl.Value.ConvertToInt();
            int katilimciId = paramRandevuKatilimciIdLbl.Value.ConvertToInt();
            int katilimciTipi = paramRandevuKatilimciTipiLbl.Value.ConvertToInt();

            string idler = paramAniObjesiIdArray.Value;
            string adetler = paramAniObjesiAdetArray.Value;
            List<string> idList = idler.Split(',').ToList<string>();
            List<string> adetList = adetler.Split(',').ToList<string>();

            KaldirilanlariSil(idList, randevuId, katilimciId, katilimciTipi);

            AniObjesiDagitim aniObjesiDagitimDao = new AniObjesiDagitim();
            List<AniObjesiDagitim> objeList = aniObjesiDagitimDao.SelectReturnList(randevuId, katilimciId, katilimciTipi);
            GetirilenAniObjesiKaydet(randevuId, katilimciId, katilimciTipi);
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
                            item.RandevuId = randevuId;
                            item.KatilimciId = katilimciId;
                            item.KatilimciTipi = katilimciTipi;
                            item.Olusturan = UtilityHelper.GetCurrentUser();
                            item.Save();
                        }
                        else //eski adetle adet farklı  update et
                        {
                            item.Adet = adet;
                            item.RandevuId = randevuId;
                            item.KatilimciId = katilimciId;
                            item.KatilimciTipi = katilimciTipi;
                            item.Degistiren = UtilityHelper.GetCurrentUser();
                            item.Update();
                        }
                    }
                }
            }
            if (objeList.Count < 1)
            {
                MessageHelper.PublishMessage("Hiç kayıt seçilmedi. Devam etmek için en az bir kayıt seçiniz.", ProjeConstants.MESAJ_BILGI);
            }
        }
        private void GetirilenAniObjesiKaydet(int randevuId, int katilimciId, int katilimciTipi)
        {
            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            aniObjesiDagitim = aniObjesiDagitim.SelectGetirilenAniObjesi(randevuId, katilimciId, katilimciTipi);

            if (aniObjesiDagitim == null)
            {
                aniObjesiDagitim = new AniObjesiDagitim();
                aniObjesiDagitim.AniObjesiId = ProjeConstants.GETIRILEN_ANIOBJESIID_INT;
                aniObjesiDagitim.RandevuId = randevuId;
                aniObjesiDagitim.KatilimciId = katilimciId;
                aniObjesiDagitim.KatilimciTipi = katilimciTipi;

                aniObjesiDagitim.GetirilenAniObjesi = GetirilenAniObjesiTxt.Text;
                aniObjesiDagitim.VerilenAlinan = ProjeConstants.ANIOBJESI_GETIRILEN_INT;
                aniObjesiDagitim.Olusturan = UtilityHelper.GetCurrentUser();
                aniObjesiDagitim.Save();

            }
            else if (aniObjesiDagitim != null)
            {
                aniObjesiDagitim.GetirilenAniObjesi = GetirilenAniObjesiTxt.Text;
                aniObjesiDagitim.Degistiren = UtilityHelper.GetCurrentUser();
                aniObjesiDagitim.Update();
            }

        }
        private void KaldirilanlariSil(List<string> idList, int randevuId, int katilimciId, int katilimciTipi)
        {
            AniObjesiDagitim aniObjesiDagitimDao = new AniObjesiDagitim();
            List<AniObjesiDagitim> objeList = aniObjesiDagitimDao.SelectReturnList(randevuId, katilimciId, katilimciTipi);

            List<string> idList1 = objeList.Select(l => l.AniObjesiId.ToString()).ToList();
            var firstNotSecond = idList1.Except(idList).ToList();
            DeleteFirstList(firstNotSecond, randevuId, katilimciId, katilimciTipi);
        }
        private void DeleteFirstList(List<string> list, int randevuId, int katilimciId, int katilimciTipi)
        {
            string joined = string.Join(",", list.ToArray());
            AniObjesiDagitim aniObjesiDagitim = new AniObjesiDagitim();
            aniObjesiDagitim.Delete(randevuId, katilimciId, katilimciTipi, joined);
        }
    }
}
