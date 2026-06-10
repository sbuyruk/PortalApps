using Microsoft.SharePoint;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.VasiyetciGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class VasiyetciGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public VasiyetciGirisiWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string VasiyetciIdQS
        {
            get
            {

                if (ViewState["VasiyetciId"] == null)
                {
                    if (Page.Request.QueryString["VasiyetciId"] != null)
                    {
                        ViewState["VasiyetciId"] = Page.Request.QueryString["VasiyetciId"];
                    }
                    else
                    {
                        ViewState["VasiyetciId"] = string.Empty;
                    }
                }
                return ViewState["VasiyetciId"].ToString();
            }

            set
            {
                ViewState["VasiyetciId"] = value;
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
        public IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)// sayfa ilk kez açiliyorsa (bu sayfanin içindeki butona basilma ani hariç)
            {
                SagVefatDDLDoldur();
                IlDDLDoldur();
                VasiyetKonusuDDLDoldur();
                VasiyetDurumuDDLDoldur();
                Vasiyetci vasiyetci = new Vasiyetci();

                if (DestinationAppQS.Equals("Duzenle"))
                {

                    vasiyetci = vasiyetci.Select(VasiyetciIdQS.ConvertToInt());
                    IdLbl.Text = vasiyetci.Id.ToString();
                    VarlikDiv.Attributes["style"] = "display:block";
                    BelgeYukleDiv.Attributes["style"] = "display:block";
                    KaydetBtn.Visible = false;
                    GuncelleBtn.Visible = true;
                    FormuDoldur();
                }
                else
                {
                    VarlikDiv.Attributes["style"] = "display:none";
                    KaydetBtn.Visible = true;
                    GuncelleBtn.Visible = false;

                    BelgeYukleDiv.Attributes["style"] = "display:none";
                    KaydetBtn.Visible = true;
                    GuncelleBtn.Visible = false;

                }

                SagVefatIslemleri(vasiyetci);

            }

            if (DestinationAppQS.Equals("Duzenle"))
            {
                VasiyeteKonuVarliklarTablosunuDoldur();
            }
        }
        private void VasiyeteKonuVarliklarTablosunuDoldur()
        {

            VasiyeteKonuVarlik vasiyeteKonuVarlik = new VasiyeteKonuVarlik();
            List<VasiyeteKonuVarlik> vasiyeteKonuVarlikListesi = vasiyeteKonuVarlik.SelectByVasiyetciId(VasiyetciIdQS.ConvertToInt());
            foreach (VasiyeteKonuVarlik item in vasiyeteKonuVarlikListesi)
            {
                TableRow yeniRow = new TableRow();

                TableCell konuCell = new TableCell();
                konuCell.Text = item.Konusu;
                konuCell.Attributes.Add("style", "vertical-align:middle");

                TableCell cinsiCell = new TableCell();
                cinsiCell.Text = item.Cinsi;
                cinsiCell.Attributes.Add("style", "vertical-align:middle");
                
                TableCell adetMiktarCell = new TableCell();
                adetMiktarCell.Text = item.AdetMiktar;
                adetMiktarCell.Attributes.Add("style", "vertical-align:middle");

                TableCell konuAciklamaCell = new TableCell();
                konuAciklamaCell.Text = item.Aciklama;
                konuAciklamaCell.Attributes.Add("style", "vertical-align:middle");

                TableCell tahminiRayicCell = new TableCell();
                tahminiRayicCell.Text = item.TahminiRayic.ToString("N", culturInfo);
                tahminiRayicCell.Attributes.Add("style", "vertical-align:middle");

                TableCell silCell = new TableCell();

                LinkButton varlikSilBtn = new LinkButton();
                varlikSilBtn.Text = "Varlığı Sil";
                varlikSilBtn.CssClass = "btn btn-outline-danger";
                silCell.Controls.Add(varlikSilBtn);

                varlikSilBtn.Click += delegate
                {
                    SilMesajiLbl.Text = "Varlığı Silmek İstediğinizden Emin misiniz?";
                    SilModalBaslikLbl.Text = "Vasiyete Konu Varlık Silinecek";
                    VasiyetciSilNowBtn.Visible = false;
                    NiteligiSilNowBtn.Visible = true;
                    ParamVnLbl.Text = item.Id.ToString();
                    string openModal = "OpenSilModal();";
                    System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openModal, true);
                };

                yeniRow.Controls.Add(konuCell);
                yeniRow.Controls.Add(cinsiCell);
                yeniRow.Controls.Add(adetMiktarCell);
                yeniRow.Controls.Add(tahminiRayicCell);
                yeniRow.Controls.Add(konuAciklamaCell);
                yeniRow.Controls.Add(silCell);

                VasiyeteKonuVarlikTable.Controls.Add(yeniRow);
            }
        }
        private void SagVefatIslemleri(Vasiyetci vasiyetci)
        {
            string sv = ProjeConstants.BAGISCI_BILINMIYOR_INT;

            if (vasiyetci != null)
            {
                if (!string.IsNullOrEmpty(vasiyetci.SagVefat))
                    sv = vasiyetci.SagVefat;

                SagVefatDDL.SelectedValue = sv;
                VefatTarihiTxt.Value = sv.Equals(ProjeConstants.BAGISCI_VEFAT_INT) ? vasiyetci.VefatTarihi.ConvertToDatetimeEmptyIfNull() : "";
            }
            if (sv.Equals(ProjeConstants.BAGISCI_VEFAT_INT))
                VefatTarihiDiv.Attributes["style"] = "display:block";
            else
                VefatTarihiDiv.Attributes["style"] = "display:none";
        }
        private void FormuDoldur()
        {
            if (VasiyetciIdQS.ConvertToInt() > 0)
            {

                Vasiyetci vasiyetci = new Vasiyetci();
                vasiyetci = vasiyetci.Select<Vasiyetci>(VasiyetciIdQS.ConvertToInt());
                if (vasiyetci != null)
                {
                    AdiTxt.Text = vasiyetci.Adi;
                    SoyadiTxt.Text = vasiyetci.Soyadi;
                    TCKimlikNoTxt.Text = vasiyetci.TCKimlikNo.ToString();
                    DogumTarihiTxt.Value = vasiyetci.DogumTarihi.ConvertToDatetimeEmptyIfNull();
                    DogumYeriTxt.Text = vasiyetci.DogumYeri;
                    VefatTarihiTxt.Value = vasiyetci.VefatTarihi.ConvertToDatetimeEmptyIfNull();
                    Telefon1Txt.Text = vasiyetci.Telefon1;
                    Telefon2Txt.Text = vasiyetci.Telefon2;

                    ListItem ilItem = IliDDL.Items.FindByValue(IliDDL.Items.FindByValue(vasiyetci.IkametIli.ToString()).Value);
                    if (ilItem != null)
                    {
                        IliDDL.SelectedValue = ilItem.Value;
                        IlceDDLDoldur();
                        if (IlcesiDDL.Items.FindByValue(vasiyetci.IkametIlcesi.ToString()) != null)
                            IlcesiDDL.SelectedValue = IlcesiDDL.Items.FindByValue(vasiyetci.IkametIlcesi.ToString()).Value;
                        string bolge = UtilityHelper.BolgeGetir(ilItem.Value.ConvertToInt());
                        SorumluBolgeTxt.Value = !string.IsNullOrEmpty(bolge) ? bolge : "";
                    }

                    IkametAdresiTxt.Text = vasiyetci.IkametAdresi;
                    VasiyetTipiTxt.Text = vasiyetci.VasiyetTipi;
                    NoterTxt.Text = vasiyetci.Noter;
                    VasiyetTarihiTxt.Value = vasiyetci.VasiyetTarihi.ConvertToDatetimeEmptyIfNull();
                    YevmiyeNumarasiTxt.Text = vasiyetci.YevmiyeNumarasi;
                    VasiyetcininTalebiTxt.Text = vasiyetci.VasiyetcininTalebi;
                    AciklamaTxt.Text = vasiyetci.Aciklama;
                    UtilityHelper.SetDDLValue(VasiyetinDurumuDDL,vasiyetci.VasiyetinDurumu);
                    string dosyaAdi = "Vasiyet" + vasiyetci.Id + ".pdf";
                    PdfDosyaLinkiEkle(ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi);
                    
                }
                else
                {
                    MessageHelper.PublishMessage("Vasiyetçi Bulunamadı", ProjeConstants.MESAJ_HATA);
                }

            }
        }
        private void PdfDosyaLinkiEkle(string spLibName, string dosyaAdi)
        {
            bool dosyaVar = UtilityHelper.DosyaVarMi(UtilityHelper.TbysURLGetir(), spLibName, dosyaAdi);
            if (dosyaVar)
            {
                DosyaLnk.Visible = true;

                string dosyaUrl = UtilityHelper.TbysURLGetir() + "/" + ProjeConstants.TBYSBELGELERI_LIB + "/" + dosyaAdi;
                DosyaLnk.NavigateUrl = dosyaUrl;
            }
            else
            {
                DosyaLnk.Visible = false;
            }
        }
        private void IlDDLDoldur()
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
        private void SagVefatDDLDoldur()
        {
            ListItem li = new ListItem(ProjeConstants.BAGISCI_SAG, ProjeConstants.BAGISCI_SAG_INT);
            ListItem li2 = new ListItem(ProjeConstants.BAGISCI_VEFAT, ProjeConstants.BAGISCI_VEFAT_INT);
            ListItem li3 = new ListItem(ProjeConstants.BAGISCI_BILINMIYOR, ProjeConstants.BAGISCI_BILINMIYOR_INT);

            SagVefatDDL.Items.Clear();
            SagVefatDDL.Items.Add(li);
            SagVefatDDL.Items.Add(li2);
            SagVefatDDL.Items.Add(li3);
        }
        private void VasiyetDurumuDDLDoldur()
        {
            ListItem li = new ListItem(ProjeConstants.VASIYETIN_DURUMU_GECERLI);
            ListItem li2 = new ListItem(ProjeConstants.VASIYETIN_DURUMU_GERCEKLESTI);
            ListItem li3 = new ListItem(ProjeConstants.VASIYETIN_DURUMU_HUKUKI);
            ListItem li4 = new ListItem(ProjeConstants.VASIYETIN_DURUMU_IPTAL);
            ListItem li5 = new ListItem(ProjeConstants.VASIYETIN_DURUMU_RUCU);
            ListItem li6 = new ListItem(ProjeConstants.VASIYETIN_DURUMU_BILGIEKSIK);
            ListItem li7 = new ListItem(ProjeConstants.VASIYETIN_DURUMU_MAHKEMEKARARI);
            ListItem li8 = new ListItem(ProjeConstants.VASIYETIN_DURUMU_MALVARLIGIYOK);
            ListItem li9 = new ListItem(ProjeConstants.VASIYETIN_DURUMU_SAGLIGINDABAGIS);
            ListItem li10 = new ListItem(ProjeConstants.VASIYETIN_DURUMU_MALVARLIGIVAKFAGECTI);

            VasiyetinDurumuDDL.Items.Clear();
            VasiyetinDurumuDDL.Items.Add(li);
            VasiyetinDurumuDDL.Items.Add(li2);
            VasiyetinDurumuDDL.Items.Add(li3);
            VasiyetinDurumuDDL.Items.Add(li4);
            VasiyetinDurumuDDL.Items.Add(li5);
            VasiyetinDurumuDDL.Items.Add(li6);
            VasiyetinDurumuDDL.Items.Add(li7);
            VasiyetinDurumuDDL.Items.Add(li8);
            VasiyetinDurumuDDL.Items.Add(li9);
            VasiyetinDurumuDDL.Items.Add(li10);
        }
        private void VasiyetKonusuDDLDoldur()
        {
            ListItem li = new ListItem(ProjeConstants.VASIYETIN_KONUSU_TUM_TASINMAZLAR);
            ListItem li2 = new ListItem(ProjeConstants.VASIYETIN_KONUSU_TUM_MENKULVARLIKLAR);
            ListItem li3 = new ListItem(ProjeConstants.VASIYETIN_KONUSU_TASINMAZ);
            ListItem li4 = new ListItem(ProjeConstants.VASIYETIN_KONUSU_MENKULBAGISLAR);
            ListItem li5 = new ListItem(ProjeConstants.VASIYETIN_KONUSU_EVESYASI);
            ListItem li6 = new ListItem(ProjeConstants.VASIYETIN_KONUSU_KAPALIVASIYET);
            ListItem li7 = new ListItem(ProjeConstants.VASIYETIN_KONUSU_BILGIEKSIK);
            ListItem li8 = new ListItem(ProjeConstants.VASIYETIN_KONUSU_DIGER);

            VasiyetKonusuDDL.Items.Clear();
            VasiyetKonusuDDL.Items.Add(li);
            VasiyetKonusuDDL.Items.Add(li2);
            VasiyetKonusuDDL.Items.Add(li3);
            VasiyetKonusuDDL.Items.Add(li4);
            VasiyetKonusuDDL.Items.Add(li5);
            VasiyetKonusuDDL.Items.Add(li6);
            VasiyetKonusuDDL.Items.Add(li7);
            VasiyetKonusuDDL.Items.Add(li8);
        }
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Vasiyetci yeniVasiyetci = new Vasiyetci();
                yeniVasiyetci.Adi = AdiTxt.Text;
                yeniVasiyetci.Soyadi = SoyadiTxt.Text;
                yeniVasiyetci.TCKimlikNo = TCKimlikNoTxt.Text.ConvertToLong();
                yeniVasiyetci.DogumTarihi = DogumTarihiTxt.Value.ConvertToDatetime();
                yeniVasiyetci.DogumYeri = DogumYeriTxt.Text;
                yeniVasiyetci.SagVefat = SagVefatDDL.SelectedValue;
                yeniVasiyetci.VefatTarihi = VefatTarihiTxt.Value.ConvertToDatetime();
                yeniVasiyetci.Telefon1 = Telefon1Txt.Text;
                yeniVasiyetci.Telefon2 = Telefon2Txt.Text;

                yeniVasiyetci.IkametIlcesi = IlcesiDDL.SelectedValue.ConvertToInt();
                ListItem ilItem = IliDDL.SelectedItem;
                yeniVasiyetci.IkametIli = ilItem.Value.ConvertToInt();

                yeniVasiyetci.IkametAdresi = IkametAdresiTxt.Text;
                yeniVasiyetci.VasiyetTipi = VasiyetTipiTxt.Text;
                yeniVasiyetci.Noter = NoterTxt.Text;
                yeniVasiyetci.VasiyetTarihi = VasiyetTarihiTxt.Value.ConvertToDatetime();
                yeniVasiyetci.YevmiyeNumarasi = YevmiyeNumarasiTxt.Text;
                yeniVasiyetci.VasiyetcininTalebi = VasiyetcininTalebiTxt.Text;
                yeniVasiyetci.Aciklama = AciklamaTxt.Text;
                yeniVasiyetci.VasiyetinDurumu = VasiyetinDurumuDDL.SelectedItem.Text;
                yeniVasiyetci.Olusturan = UtilityHelper.GetCurrentUserLoginName();
                if (string.IsNullOrEmpty(AdiTxt.Text))
                {
                    MessageHelper.PublishMessage("Vasiyetçi Adı Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
                }
                else if (string.IsNullOrEmpty(SoyadiTxt.Text))
                {
                    MessageHelper.PublishMessage("Vasiyetçi Soyadı Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
                }
                else
                {
                    int yeniId = yeniVasiyetci.Save();
                    if (yeniId > 0)
                    {
                        VasiyetciIdQS = yeniId.ToString();
                        RedirectToPage(ProjeConstants.PAGE_VASIYETCI_GIRISI + "?DestinationApp=Duzenle&VasiyetciId=" + VasiyetciIdQS);
                        MessageHelper.PublishMessage("Vasiyetçi Kaydedildi.", ProjeConstants.MESAJ_BASARILI, 2000);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Vasiyetçi Kaydedilemedi.", ProjeConstants.MESAJ_HATA);
                    }
                }

            }
            catch (Exception ex)
            {

                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
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
                Vasiyetci vasiyetci = new Vasiyetci();
                vasiyetci = vasiyetci.Select<Vasiyetci>(VasiyetciIdQS.ConvertToInt());
                if (vasiyetci != null)
                {
                    vasiyetci.Adi = AdiTxt.Text;
                    vasiyetci.Soyadi = SoyadiTxt.Text;
                    vasiyetci.TCKimlikNo = TCKimlikNoTxt.Text.ConvertToLong();
                    vasiyetci.DogumTarihi = DogumTarihiTxt.Value.ConvertToDatetime();
                    vasiyetci.DogumYeri = DogumYeriTxt.Text;
                    vasiyetci.SagVefat = SagVefatDDL.SelectedValue;
                    vasiyetci.VefatTarihi = VefatTarihiTxt.Value.ConvertToDatetime();
                    vasiyetci.Telefon1 = Telefon1Txt.Text;
                    vasiyetci.Telefon2 = Telefon2Txt.Text;

                    vasiyetci.IkametIlcesi = IlcesiDDL.SelectedValue.ConvertToInt();
                    ListItem ilItem = IliDDL.SelectedItem;

                    vasiyetci.IkametIli = ilItem.Value.ConvertToInt();

                    vasiyetci.IkametAdresi = IkametAdresiTxt.Text;
                    vasiyetci.VasiyetTipi = VasiyetTipiTxt.Text;
                    //vasiyetci.SorumluBolge = SorumluBolgeTxt.Value;
                    vasiyetci.Noter = NoterTxt.Text;
                    vasiyetci.VasiyetTarihi = VasiyetTarihiTxt.Value.ConvertToDatetime();
                    vasiyetci.YevmiyeNumarasi = YevmiyeNumarasiTxt.Text;
                    vasiyetci.VasiyetcininTalebi = VasiyetcininTalebiTxt.Text;
                    vasiyetci.Aciklama = AciklamaTxt.Text;
                    vasiyetci.VasiyetinDurumu = VasiyetinDurumuDDL.SelectedItem.Text;
                    vasiyetci.Degistiren = UtilityHelper.GetCurrentUserLoginName();
                }


                if (string.IsNullOrEmpty(AdiTxt.Text))
                {
                    MessageHelper.PublishMessage("Vasiyetçi Adı Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
                }
                else if (string.IsNullOrEmpty(SoyadiTxt.Text))
                {
                    MessageHelper.PublishMessage("Vasiyetçi Soyadı Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
                }
                else
                {
                    bool guncellendiMi = vasiyetci.Update();
                    if (guncellendiMi)
                    {
                        MessageHelper.PublishMessage("Vasiyetçi Güncellendi.", ProjeConstants.MESAJ_BASARILI, 2000);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Vasiyetçi Güncellenmedi.", ProjeConstants.MESAJ_HATA);
                    }
                }

            }
            catch (Exception ex)
            {

                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
            }
        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            IlceDDLDoldur();
            int ilId = IliDDL.SelectedItem.Value.ConvertToInt();
            string bolge = UtilityHelper.BolgeGetir(ilId);
            SorumluBolgeTxt.Value = !string.IsNullOrEmpty(bolge) ? bolge : "";
        }
        protected void SagVefatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (SagVefatDDL.SelectedItem.Value.Equals(ProjeConstants.BAGISCI_VEFAT_INT))
            {
                VefatTarihiTxt.Value = string.Empty;
                VefatTarihiDiv.Attributes["style"] = "display:block";
            }
            else
            {
                VefatTarihiDiv.Attributes["style"] = "display:none";
            }
        }
        protected void EkleBtn_Click(object sender, EventArgs e)
        {
            string openModal = "OpenModal();";
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openModal, true);
        }
        private void YukleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (BekleYukleFU.HasFile)
                {
                    string hedefDosyaAdi = @"Vasiyet" + VasiyetciIdQS + @".pdf";
                    bool isOk= UtilityHelper.UploadFileToSharePoint(BekleYukleFU, ProjeConstants.TBYSBELGELERI_LIB,hedefDosyaAdi);
                    
                    if (isOk)
                    {
                        string dosyaAdi = "Vasiyet" + VasiyetciIdQS + ".pdf";
                        string dosyaUrl = SPContext.Current.Web.Url + @"/TBYSBelgeleri/" + dosyaAdi;
                        DosyaLnk.Target = "_blank";
                        DosyaLnk.NavigateUrl = dosyaUrl;

                        DosyaLnk.Visible = true;
                        MessageHelper.PublishMessage("PDF Yüklendi", ProjeConstants.MESAJ_BASARILI, 2000); 
                    }
                }
                else
                {
                    DosyaLnk.Visible = false;
                    MessageHelper.PublishMessage("Lütfen bir pdf belgesi seçiniz.", ProjeConstants.MESAJ_BILGI, 2000);
                }
            }
            catch (Exception exception)
            {
                Exception ex = new Exception("PDF Yüklenemedi");
                ExceptionHelper exhelper = new ExceptionHelper(exception);
                exhelper.Exceptions.Add(ex);
                exhelper.PublishException();
            }
        }

        protected void VarlikKaydetBtn_Click(object sender, EventArgs e)
        {
            VasiyeteKonuVarlik vn = new VasiyeteKonuVarlik();
            vn.VasiyetciId = VasiyetciIdQS.ConvertToInt();
            vn.Konusu = VasiyetKonusuDDL.SelectedItem.Text;
            vn.Cinsi = VasiyetCinsiDDL.SelectedItem.Text;
            vn.AdetMiktar = AdetMiktarTxt.Text;
            vn.TahminiRayic = TahminiRayicTxt.Text.ConvertToDecimal();
            vn.Aciklama= KonuAciklama.Text;
            vn.Save();
            RedirectToPage(ProjeConstants.PAGE_VASIYETCI_GIRISI + "?DestinationApp=Duzenle&VasiyetciId=" + VasiyetciIdQS);
        }
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            VasiyetCinsiDDLDoldur();
        }
        private void VasiyetCinsiDDLDoldur()
        {

            VasiyetCinsiDDL.Items.Clear();
            string vasiyetKonusu = VasiyetKonusuDDL.SelectedItem.Value;
            if (vasiyetKonusu.Equals(ProjeConstants.VASIYETIN_KONUSU_KAPALIVASIYET))
            {
                ListItem li = new ListItem(ProjeConstants.VASIYETIN_KONUSU_KAPALIVASIYET);
                VasiyetCinsiDDL.Items.Add(li);
            }
            else if (vasiyetKonusu.Equals(ProjeConstants.VASIYETIN_KONUSU_EVESYASI))
            {
                ListItem li = new ListItem(ProjeConstants.VASIYETIN_KONUSU_EVESYASI);
                VasiyetCinsiDDL.Items.Add(li);
            }
            else if (vasiyetKonusu.Equals(ProjeConstants.VASIYETIN_KONUSU_BILGIEKSIK))
            {
                ListItem li = new ListItem(ProjeConstants.VASIYETIN_KONUSU_BILGIEKSIK);
                VasiyetCinsiDDL.Items.Add(li);
            }
            else if (vasiyetKonusu.Equals(ProjeConstants.VASIYETIN_KONUSU_DIGER))
            {
                ListItem li = new ListItem(ProjeConstants.VASIYETIN_KONUSU_DIGER);
                VasiyetCinsiDDL.Items.Add(li);
            }
            else if (vasiyetKonusu.Equals(ProjeConstants.VASIYETIN_KONUSU_TUM_TASINMAZLAR))
            {
                ListItem li = new ListItem(ProjeConstants.VASIYETIN_KONUSU_TUM_TASINMAZLAR);
                VasiyetCinsiDDL.Items.Add(li);
            }
            else if (vasiyetKonusu.Equals(ProjeConstants.VASIYETIN_KONUSU_TUM_MENKULVARLIKLAR))
            {
                ListItem li = new ListItem(ProjeConstants.VASIYETIN_KONUSU_TUM_MENKULVARLIKLAR);
                VasiyetCinsiDDL.Items.Add(li);
            }
            else if (vasiyetKonusu.Equals(ProjeConstants.VASIYETIN_KONUSU_TASINMAZ))
            {
                ListItem li = new ListItem("İshani");
                ListItem li2 = new ListItem("Apt.");
                ListItem li3 = new ListItem("Arsa");
                ListItem li4 = new ListItem("İşyeri");
                ListItem li5 = new ListItem("M.Ev");
                ListItem li6 = new ListItem("Mesken");
                ListItem li7 = new ListItem("Tarla");
                ListItem li8 = new ListItem("Tesis");
                ListItem li9 = new ListItem("Diğer");

                VasiyetCinsiDDL.Items.Add(li);
                VasiyetCinsiDDL.Items.Add(li2);
                VasiyetCinsiDDL.Items.Add(li3);
                VasiyetCinsiDDL.Items.Add(li4);
                VasiyetCinsiDDL.Items.Add(li5);
                VasiyetCinsiDDL.Items.Add(li6);
                VasiyetCinsiDDL.Items.Add(li7);
                VasiyetCinsiDDL.Items.Add(li8);
                VasiyetCinsiDDL.Items.Add(li9);
            }
            else if (vasiyetKonusu.Equals(ProjeConstants.VASIYETIN_KONUSU_MENKULBAGISLAR))
            {

                ListItem li = new ListItem("Nakit Para");
                ListItem li1 = new ListItem("USD");
                ListItem li2 = new ListItem("Euro");
                ListItem li3 = new ListItem("GBP");
                ListItem li4 = new ListItem("Altın");
                ListItem li5 = new ListItem("Mücevher");
                ListItem li6 = new ListItem("Değerli Maden");
                ListItem li7 = new ListItem("Diğer");


                VasiyetCinsiDDL.Items.Add(li);
                VasiyetCinsiDDL.Items.Add(li1);
                VasiyetCinsiDDL.Items.Add(li2);
                VasiyetCinsiDDL.Items.Add(li3);
                VasiyetCinsiDDL.Items.Add(li4);
                VasiyetCinsiDDL.Items.Add(li5);
                VasiyetCinsiDDL.Items.Add(li6);
                VasiyetCinsiDDL.Items.Add(li7);
            }
        }
        private void VasiyetciyiSilPopupAc(object sender)
        {
            ParamVnLbl.Text = VasiyetciIdQS;
            string openModal = "OpenSilModal();";
            SilMesajiLbl.Text = "Vasiyetçiyi Silmek İstediğinizden Emin misiniz?";
            SilModalBaslikLbl.Text = "Vasiyetçi Silinecek";
            NiteligiSilNowBtn.Visible = false;
            VasiyetciSilNowBtn.Visible = true;
            UtilityHelper.ScriptCalistir( openModal);
        }
        protected void VasiyetciSilBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Vasiyetci vasiyetci = new Vasiyetci();
                vasiyetci = vasiyetci.Select(VasiyetciIdQS.ConvertToInt());
                if (vasiyetci != null)
                {
                    VasiyeteKonuVarlik vkv = new VasiyeteKonuVarlik();
                    List<VasiyeteKonuVarlik> list =vkv.SelectByVasiyetciId(vasiyetci.Id);
                    if (list.Count < 1)
                    {
                        VasiyetciyiSilPopupAc(sender);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Vasiyete konu varlık listesi dolu olduğundan Vasiyetçi silinemiyor. Lütfen önce Vasiyete Konu Varlık Listesindeki varlıkları siliniz", ProjeConstants.MESAJ_HATA);
                    }
                }
            }
            catch (Exception e1)
            {
                ExceptionHelper eh = new ExceptionHelper();
                Exception e2 = new Exception("Vasiyetçi silinemedi");
                eh.Exceptions.Add(e2);
                eh.Exceptions.Add(e1);
                eh.PublishException();
            }
        }
        protected void VasiyetciSilNowBtn_Click(object sender, EventArgs e)
        {
            bool silindi = false;
            try
            {
                Vasiyetci vasiyetci = new Vasiyetci();
                vasiyetci = vasiyetci.Select(VasiyetciIdQS.ConvertToInt());
                if (vasiyetci != null)
                {
                    silindi = vasiyetci.Delete();
                    RedirectToPage(ProjeConstants.PAGE_VASIYETCI_LISTESI);
                }
                if (!silindi)
                {
                    MessageHelper.PublishMessage("Vasiyetçi Silinemedi", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception e1)
            {
                ExceptionHelper eh = new ExceptionHelper();
                Exception e2 = new Exception("Vasiyetci silinemedi");
                eh.Exceptions.Add(e2);
                eh.Exceptions.Add(e1);
                eh.PublishException();
            }
        }
        protected void NiteligiSilNowBtn_Click(object sender, EventArgs e)
        {
            int silecekVnId = ParamVnLbl.Text.ConvertToInt();
            VasiyeteKonuVarlik silinecekvn = new VasiyeteKonuVarlik();
            silinecekvn = silinecekvn.Select(silecekVnId);
            if (silinecekvn != null)
            {
                bool silindi = silinecekvn.Delete();
                if (silindi)
                {
                    RedirectToPage(ProjeConstants.PAGE_VASIYETCI_GIRISI + "?DestinationApp=Duzenle&VasiyetciId=" + VasiyetciIdQS);
                    MessageHelper.PublishMessage("Kayıt Silindi.", ProjeConstants.MESAJ_BASARILI, 2000);
                }
                else
                {
                    MessageHelper.PublishMessage("Kayıt Silinemedi.", ProjeConstants.MESAJ_HATA);
                }
            }
            else
            {
                MessageHelper.PublishMessage("Vasiyet Bulunamadı", ProjeConstants.MESAJ_HATA);
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

        protected void VasiyetciListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_VASIYETCI_LISTESI);
        }
        protected void VasiyetKonusuDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            VasiyetCinsiDDLDoldur();
        }
    }
}
