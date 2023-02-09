using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.FTKKisiGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class FTKKisiGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FTKKisiGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string FTKKisiIdQS
        {
            get
            {

                if (ViewState["FTKKisiId"] == null)
                {
                    if (Page.Request.QueryString["FTKKisiId"] != null)
                    {
                        ViewState["FTKKisiId"] = Page.Request.QueryString["FTKKisiId"];
                    }
                    else
                    {
                        ViewState["FTKKisiId"] = string.Empty;
                    }
                }
                return ViewState["FTKKisiId"].ToString();
            }

            set
            {
                ViewState["FTKKisiId"] = value;
            }
        }
        private string IliIdQS
        {
            get
            {

                if (ViewState["IliId"] == null)
                {
                    if (Page.Request.QueryString["IliId"] != null)
                    {
                        ViewState["IliId"] = Page.Request.QueryString["IliId"];
                    }
                    else
                    {
                        ViewState["IliId"] = string.Empty;
                    }
                }
                return ViewState["IliId"].ToString();
            }

            set
            {
                ViewState["IliId"] = value;
            }
        }
        private string IlcesiIdQS
        {
            get
            {

                if (ViewState["IlcesiId"] == null)
                {
                    if (Page.Request.QueryString["IlcesiId"] != null)
                    {
                        ViewState["IlcesiId"] = Page.Request.QueryString["IlcesiId"];
                    }
                    else
                    {
                        ViewState["IlcesiId"] = string.Empty;
                    }
                }
                return ViewState["IlcesiId"].ToString();
            }

            set
            {
                ViewState["IlcesiId"] = value;
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
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                YonergeLnk.HRef = NBYSOrtak.YonergeURLGetir(ProjeConstants.PARAM_FTKYONERGE, ProjeConstants.PAGE_FTKKISI_GIRISI);
                IlkACilis();
            }
        }
        private void IlkACilis()
        {
            IlDDLDoldur();
            IlIlceDoldur();
            FTKGoreviDDLDoldur();
            UyelikDurumuDDLDoldur();
            if (string.IsNullOrEmpty(FTKKisiIdQS))
            {
                TitleLbl.Text = "FTK Kişi Girişi";
                TitleLbl.CssClass = "col-form-label text-success font-weight-bold mb-1";
                IdLbl.Text = string.Empty;
                KaydetBtn.Visible = true;
                GuncelleBtn.Visible = false;
                YeniKayitBtn.Visible = false;
                UtilityHelper.SetDDLValue(UyelikDurumuDDL, ProjeConstants.FTK_UYELIK_DURUMU_AKTIF);

                SilBtn.Visible = false;

                MessageHelper.PublishMessage("Yeni üye bilgilerini girerek kayıt yapabilirsiniz.", ProjeConstants.MESAJ_BILGI, 2000);
            }
            else
            {
                TitleLbl.Text = "FTK Kişi Düzenle";
                TitleLbl.CssClass = "col-form-label text-primary font-weight-bold mb-1";
                //IdLbl.Text = FTKKisiIdQS.ToString();
                KaydetBtn.Visible = false;
                GuncelleBtn.Visible = true;
                YeniKayitBtn.Visible = true;
                SilBtn.Visible = true;
                FTKKisiFormunuDoldur();
            }
        }
        private void IlIlceDoldur()
        {
            if (!string.IsNullOrEmpty(IliIdQS))
            {
                ListItem ilItem = IliDDL.Items.FindByValue(IliDDL.Items.FindByValue(IliIdQS).Value);
                if (ilItem != null)
                {
                    IliDDL.SelectedValue = ilItem.Value;
                    IlceDDLDoldur();
                    if (!string.IsNullOrEmpty(IlcesiIdQS))
                    {
                        if (IlcesiDDL.Items.FindByValue(IlcesiIdQS) != null)
                            IlcesiDDL.SelectedValue = IlcesiDDL.Items.FindByValue(IlcesiIdQS).Value;
                    }
                }
            }

        }
        private void FTKGoreviDDLDoldur()
        {
            if (FTKGoreviDDL.SelectedItem == null)
            {
                FTKGoreviDDL.Items.Clear();

                FTKGoreviDDL.Items.Add(new ListItem(ProjeConstants.FTK_GOREVI_UYE, ProjeConstants.FTK_GOREVI_UYE_INT.ToString()));
                FTKGoreviDDL.Items.Add(new ListItem(ProjeConstants.FTK_GOREVI_BASKAN, ProjeConstants.FTK_GOREVI_BASKAN_INT.ToString()));
                FTKGoreviDDL.Items.Add(new ListItem(ProjeConstants.FTK_GOREVI_GENELSEKRETER, ProjeConstants.FTK_GOREVI_GENELSEKRETER_INT.ToString()));
                FTKGoreviDDL.Items.Add(new ListItem(ProjeConstants.FTK_GOREVI_FAHRIBASKAN, ProjeConstants.FTK_GOREVI_FAHRIBASKAN_INT.ToString()));
            }

        }
        private void UyelikDurumuDDLDoldur()
        {
            UyelikDurumuDDL.Items.Clear();
            UyelikDurumuDDL.Items.Add(new ListItem(ProjeConstants.FTK_UYELIK_DURUMU_AKTIF, ProjeConstants.FTK_UYELIK_DURUMU_AKTIF));
            UyelikDurumuDDL.Items.Add(new ListItem(ProjeConstants.FTK_UYELIK_DURUMU_AKTIF_DEGIL, ProjeConstants.FTK_UYELIK_DURUMU_AKTIF_DEGIL));
        }
        private void IlDDLDoldur()
        {
            IliDDL.Items.Clear();
            Il newil = new Il();
            List<Il> list = newil.SelectAll<Il>();
            IliDDL.Items.Add(new ListItem(string.Empty));
            foreach (Il il in list)
            {
                IliDDL.Items.Add(new ListItem(il.IlAdi, il.Id.ToString()));
            }
            IlceDDLDoldur();

        }
        private void IlceDDLDoldur()
        {
            IlcesiDDL.Items.Clear();
            Ilce pIlce = new Ilce();
            List<Ilce> list = pIlce.SelectByIlId(IliDDL.SelectedValue.ConvertToInt());
            ListItem li0 = new ListItem(ProjeConstants.VALILIK, ProjeConstants.VALILIK_INT.ToString());
            IlcesiDDL.Items.Add(li0);
            foreach (Ilce ilce in list)
            {
                if (ilce.IlceAdi.ToUpper().Equals(ProjeConstants.ILCE_MERKEZ.ToUpper()))
                    continue;
                IlcesiDDL.Items.Add(new ListItem(ilce.IlceAdi, ilce.Id.ToString()));
            }
        }
        private bool FTKKisiFormunuDoldur()
        {
            FTKKisi ftkkisi = new FTKKisi();
            ftkkisi = ftkkisi.Select(FTKKisiIdQS.ConvertToInt());
            bool formDolduMu = false;
            try
            {
                if (ftkkisi != null)
                {
                    IdLbl.Text = ftkkisi.Id.ToString();
                    AdiTxt.Text = ftkkisi.Adi;
                    SoyadiTxt.Text = ftkkisi.Soyadi;
                    TCKimlikNoTxt.Text = ftkkisi.TCKimlikNo.ToString();
                    DogumTarihiTxt.Text = ftkkisi.DogumTarihi.ConvertToDatetimeEmptyIfNull();
                    Telefon1Txt.Text = ftkkisi.Telefon1;
                    Telefon2Txt.Text = ftkkisi.Telefon2;

                    UnvaniTxt.Text = ftkkisi.Unvani;
                    ValiChk.Checked = ftkkisi.Vali;
                    KaymakamChk.Checked = ftkkisi.Kaymakam;
                    AdresTxt.Text = ftkkisi.Adres;
                    AciklamaTxt.Text = ftkkisi.Aciklama;

                    FTKKisiIdQS = ftkkisi.Id.ToString();
                    ListItem ilItem = IliDDL.Items.FindByValue(IliDDL.Items.FindByValue(ftkkisi.Ili.ToString()).Value);
                    if (ilItem != null)
                    {
                        IliDDL.SelectedValue = ilItem.Value;
                        IlceDDLDoldur();
                        UtilityHelper.SetDDLValue(IliDDL, ftkkisi.Ili.ToString());
                        UtilityHelper.SetDDLValue(IlcesiDDL, ftkkisi.Ilcesi.ToString());
                        IliIdQS = ftkkisi.Ili.ToString();
                        IlcesiIdQS = ftkkisi.Ilcesi.ToString();
                    }
                    KartNoTxt.Text = ftkkisi.KartNo;
                    UtilityHelper.SetDDLValue(FTKGoreviDDL, ftkkisi.FTKGorevi.ToString());
                    UtilityHelper.SetDDLValue(UyelikDurumuDDL, ftkkisi.UyelikDurumu);

                }
                else
                {
                    MessageHelper.PublishMessage("Kişi Bulunamadı", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();
                formDolduMu = false;
            }
            return formDolduMu;
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            try
            {
                FTKKisi ftkKisi = KaydetFTKKisiData2Db(exceptionHelper);
                if (ftkKisi != null) //kaydettikten sonra önceki sayfaya dön
                {
                    FTKKisiIdQS = ftkKisi.Id.ToString();
                    IlkACilis();
                    MessageHelper.PublishMessage("Kişi Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                }
                else
                {
                    throw (new Exception("Kişi kaydedilemedi."));
                }
            }
            catch (Exception ex)
            {
                exceptionHelper.Exceptions.Add(ex);
                exceptionHelper.PublishException();
            }
        }
        private FTKKisi KaydetFTKKisiData2Db(ExceptionHelper exceptionHelper)
        {
            FTKKisi yeniFTKKisi = null;
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
                if (KayitVarMi(TCKimlikNoTxt.Text.ConvertToLong()))
                {
                    exceptionHelper.Exceptions.Add(new Exception("Bu TCKimlik numaralı bir kayıt zaten var."));
                }
                else
                {
                    yeniFTKKisi = new FTKKisi();
                    yeniFTKKisi.Adi = AdiTxt.Text;
                    yeniFTKKisi.Soyadi = SoyadiTxt.Text;
                    yeniFTKKisi.TCKimlikNo = TCKimlikNoTxt.Text.ConvertToLong();
                    yeniFTKKisi.DogumTarihi = DogumTarihiTxt.Text.ConvertToDatetime();
                    yeniFTKKisi.Telefon1 = Telefon1Txt.Text;
                    yeniFTKKisi.Telefon2 = Telefon2Txt.Text;
                    yeniFTKKisi.KartNo = KartNoTxt.Text;
                    yeniFTKKisi.Unvani = UnvaniTxt.Text;
                    yeniFTKKisi.Vali = ValiChk.Checked;
                    yeniFTKKisi.Kaymakam = KaymakamChk.Checked;
                    yeniFTKKisi.Adres = AdresTxt.Text;

                    yeniFTKKisi.Ilcesi = IlcesiDDL.SelectedValue.ConvertToInt();
                    ListItem ilItem = IliDDL.SelectedItem;
                    yeniFTKKisi.Ili = ilItem.Value.ConvertToInt();
                    yeniFTKKisi.Aciklama = AciklamaTxt.Text;

                    yeniFTKKisi.FTKGorevi = FTKGoreviDDL.SelectedItem.Value.ConvertToInt();
                    yeniFTKKisi.KartNo = KartNoTxt.Text;
                    yeniFTKKisi.UyelikDurumu = UyelikDurumuDDL.SelectedItem.Value;

                    yeniFTKKisi.Olusturan = CurrentUserName;
                    int ftkUyeId = yeniFTKKisi.Save();
                    yeniFTKKisi.Id = ftkUyeId;
                    FTKKisiIdQS = yeniFTKKisi.Id.ToString();
                    IliIdQS = yeniFTKKisi.Ili.ToString();
                    IlcesiIdQS = yeniFTKKisi.Ilcesi.ToString();
                    return yeniFTKKisi;
                }
            }
            return yeniFTKKisi;
        }
        protected void GuncelleBtn_Click(object sender, EventArgs e)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            try
            {

                bool guncellendiMi = GuncelleFTKKisiData2Db(exceptionHelper);
                if (guncellendiMi)
                {
                    MessageHelper.PublishMessage("Kişi Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                }
                else
                {
                    throw (new Exception("Kişi Güncellenemedi."));

                }
            }
            catch (Exception ex)
            {
                exceptionHelper.Exceptions.Add(ex);
                exceptionHelper.PublishException();
            }
        }
        private bool GuncelleFTKKisiData2Db(ExceptionHelper exceptionHelper)
        {
            bool guncellendiMi = false;
            FTKKisi ftkKisi = new FTKKisi();
            ftkKisi = ftkKisi.Select<FTKKisi>(FTKKisiIdQS.ConvertToInt());
            if ((ftkKisi.TCKimlikNo != TCKimlikNoTxt.Text.ConvertToLong()) && KayitVarMi(TCKimlikNoTxt.Text.ConvertToLong()))
            {
                exceptionHelper.Exceptions.Add(new Exception("Bu TCKimlik numaralı bir kayıt zaten var."));
            }
            else
            {

                if (ftkKisi != null)
                {
                    ftkKisi.Adi = AdiTxt.Text;
                    ftkKisi.Soyadi = SoyadiTxt.Text;
                    ftkKisi.TCKimlikNo = TCKimlikNoTxt.Text.ConvertToLong();
                    ftkKisi.DogumTarihi = DogumTarihiTxt.Text.ConvertToDatetime();
                    ftkKisi.Telefon1 = Telefon1Txt.Text;
                    ftkKisi.Telefon2 = Telefon2Txt.Text;
                    ftkKisi.KartNo = KartNoTxt.Text;
                    ftkKisi.Unvani = UnvaniTxt.Text;
                    ftkKisi.Vali = ValiChk.Checked;
                    ftkKisi.Kaymakam = KaymakamChk.Checked;
                    ftkKisi.Ilcesi = IlcesiDDL.SelectedValue.ConvertToInt();
                    ListItem ilItem = IliDDL.SelectedItem;
                    ftkKisi.Ili = ilItem.Value.ConvertToInt();

                    ftkKisi.Adres = AdresTxt.Text;
                    ftkKisi.Aciklama = AciklamaTxt.Text;

                    ftkKisi.FTKGorevi = FTKGoreviDDL.SelectedItem.Value.ConvertToInt();
                    ftkKisi.KartNo = KartNoTxt.Text;

                    ftkKisi.UyelikDurumu = UyelikDurumuDDL.SelectedItem.Value;

                    ftkKisi.Degistiren = CurrentUserName;
                    guncellendiMi = ftkKisi.Update();
                    FTKKisiIdQS = ftkKisi.Id.ToString();
                    IliIdQS = ftkKisi.Ili.ToString();
                    IlcesiIdQS = ftkKisi.Ilcesi.ToString();
                }
            }
            return guncellendiMi;
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
        private bool AdSoyadVarMi(string adi, string soyadi)
        {
            FTKKisi uyeDao = new FTKKisi();
            uyeDao = uyeDao.SelectByAdiSoyadi(adi, soyadi);
            if (uyeDao == null)
            {
                return false;
            }
            else
                return true;
        }
        private bool KayitVarMi(long tckimlikno)
        {
            if (tckimlikno < 1)
            {
                return false;
            }
            else
            {
                FTKKisi uyeDao = new FTKKisi();
                uyeDao = uyeDao.SelectByTCKimlikNo(tckimlikno);
                if (uyeDao == null)
                {
                    return false;
                }
                else
                    return true;
            }

        }
        protected void SoyadiTxt_TextChanged(object sender, EventArgs e)
        {
            if (AdSoyadVarMi(AdiTxt.Text, SoyadiTxt.Text))
            {
                MessageHelper.PublishMessage("Bu AD ve SOYADI içeren bir kayıt zaten var. Lütfen kaydetmeden önce kontrol ediniz ", ProjeConstants.MESAJ_HATA);
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
        protected void FtkKisiListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FTKKISI_LIST + "?SecilenId=" + FTKKisiIdQS);
        }
        protected void FTKFahriBaskanListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FTKFAHRIBASKAN_LIST + "?SecilenId=" + FTKKisiIdQS);
        }
        protected void FTKIslemleriBtn_Click(object sender, EventArgs e)
        {
            FTKIslem fTKIslem = new FTKIslem();
            fTKIslem = fTKIslem.SelectByIliIlcesi(IliIdQS.ConvertToInt(), IlcesiIdQS.ConvertToInt());
            string ftkIslemId = fTKIslem == null ? string.Empty : fTKIslem.Id.ToString();
            RedirectToPage(ProjeConstants.PAGE_FTKISLEMLERI + "?FTKIslemId=" + ftkIslemId + "&IliId=" + IliIdQS + "&IlcesiId=" + IlcesiIdQS);
        }

        protected void FTKGoreviDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValiKaymakamSec();
        }

        private void ValiKaymakamSec()
        {
            int iliId = IliDDL.SelectedItem.Value.ConvertToInt();
            int ilcesiId = IlcesiDDL.SelectedItem.Value.ConvertToInt();

            int gorevi = FTKGoreviDDL.SelectedItem.Value.ConvertToInt();
            if (gorevi == ProjeConstants.FTK_GOREVI_FAHRIBASKAN_INT)
            {
                if (iliId > 0)
                {
                    ValiChk.Checked = true;
                }
                else
                {
                    ValiChk.Checked = false;
                }
                if (ilcesiId > 0)
                {
                    KaymakamChk.Checked = true;
                    ValiChk.Checked = false;
                }
                else
                {
                    KaymakamChk.Checked = false;
                }

            }
            else
            {
                ValiChk.Checked = false;
                KaymakamChk.Checked = false;
            }
        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            IlceDDLDoldur();
            ValiKaymakamSec();
        }
        protected void IlcesiDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValiKaymakamSec();
        }
        protected void SilBtn_Click(object sender, EventArgs e)
        {
            try
            {
                FTKKisi ftkkisi = new FTKKisi();
                ftkkisi = ftkkisi.Select(FTKKisiIdQS.ConvertToInt());
                if (ftkkisi != null)
                {
                    SilPopupAc(sender);
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
        private void SilPopupAc(object sender)
        {
            ParamVnLbl.Text = FTKKisiIdQS;
            string openModal = "OpenSilModal();";
            SilMesajiLbl.Visible = true;
            SilMesajiLbl.Text = "Onayladığınız takdirde FTK personeline ait tüm bilgiler silinecektir.</br>Silmek istediğinizden eminmisiniz?";
            SilModalBaslikLbl.Text = "Silmeyi Onayla";
            SilNowBtn.Visible = true;
            UtilityHelper.ScriptCalistir(openModal);
        }
        protected void SilNowBtn_Click(object sender, EventArgs e)
        {
            bool silindi = false;
            try
            {
                FTKKisi ftkkisi = new FTKKisi();
                ftkkisi = ftkkisi.Select(FTKKisiIdQS.ConvertToInt());

                if (ftkkisi != null)
                {
                    silindi = ftkkisi.Delete();
                    FTKIslem fTKIslemleri = new FTKIslem();
                    fTKIslemleri = fTKIslemleri.SelectByIliIlcesi(IliIdQS.ConvertToInt(), IlcesiIdQS.ConvertToInt());
                    string ftkIslemId = fTKIslemleri == null ? string.Empty : fTKIslemleri.Id.ToString();
                    RedirectToPage(ProjeConstants.PAGE_FTKISLEMLERI + "?FTKIslemId=" + ftkIslemId + "&IliId=" + IliIdQS + "&IlcesiId=" + IlcesiIdQS);

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

        protected void YeniKayitBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FTKKISI_GIRISI + "?IliId=" + IliIdQS + "&IlcesiId=" + IlcesiIdQS);
        }
    }
}
