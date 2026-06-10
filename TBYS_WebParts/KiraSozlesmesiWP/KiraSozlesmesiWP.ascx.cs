using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.KiraSozlesmesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraSozlesmesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraSozlesmesiWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        private int BolgeIdQS
        {
            get
            {

                if (ViewState["BolgeId"] == null)
                {
                    if (Page.Request.QueryString["BolgeId"] != null)
                    {
                        ViewState["BolgeId"] = Page.Request.QueryString["BolgeId"];
                    }
                    else
                    {
                        ViewState["BolgeId"] = string.Empty;
                    }
                }
                return ViewState["BolgeId"].ReturnZeroIfNull().ConvertToInt();
            }

            set
            {
                ViewState["BolgeId"] = value;
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
        private string KiraciIdQS
        {
            get
            {

                if (ViewState["KiraciId"] == null)
                {
                    if (Page.Request.QueryString["KiraciId"] != null)
                    {
                        ViewState["KiraciId"] = Page.Request.QueryString["KiraciId"];
                    }
                    else
                    {
                        ViewState["KiraciId"] = string.Empty;
                    }
                }
                return ViewState["KiraciId"].ToString();
            }

            set
            {
                ViewState["KiraciId"] = value;
            }
        }
        private string KiraSozlesmeIdQS
        {
            get
            {

                if (ViewState["KiraSozlesmeId"] == null)
                {
                    if (Page.Request.QueryString["KiraSozlesmeId"] != null)
                    {
                        ViewState["KiraSozlesmeId"] = Page.Request.QueryString["KiraSozlesmeId"];
                    }
                    else
                    {
                        ViewState["KiraSozlesmeId"] = string.Empty;
                    }
                }
                return ViewState["KiraSozlesmeId"].ToString();
            }

            set
            {
                ViewState["KiraSozlesmeId"] = value;
            }
        }
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Bolge bolge = IKYSOrtak.BolgeGetirByUserName(CurrentUserName);
                BolgeIdQS = bolge == null ? 0 : bolge.Id;
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.Select(KiraSozlesmeIdQS.ConvertToInt());
                if (kiraSozlesme == null)
                {
                    MessageHelper.PublishMessage("Sözleşme Bulunamadı",ProjeConstants.MESAJ_HATA);
                }else
                {
                    string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                    string imgUrl = newUrl + "/../" + ProjeConstants.RESIMLER_TBYS + "/belli-degil.png";

                    if (kiraSozlesme.Aktif)
                    {
                        
                        AktifSozlesmeYukle(kiraSozlesme);
                        TitleLbl.CssClass = "col-form-label text-danger fw-bold";
                        imgUrl = newUrl + "/../" + ProjeConstants.RESIMLER_TBYS + "/sozlesme-aktif.png";
                        SozlesmeDurumuDiv.Attributes["style"] = "display:none";
                    }
                    else
                    {
                        BitenSozlesmeYukle(kiraSozlesme);
                        imgUrl = newUrl + "/../" + ProjeConstants.RESIMLER_TBYS + "/sozlesme-aktif-degil.png";
                        TitleLbl.CssClass = "col-form-label text-secondary fw-bold";
                        SozlesmeDurumuDiv.Attributes["style"] = "display:block";
                    }
                    
                    AktifPasifImg.ImageUrl = imgUrl;
                    PDFGoster(kiraSozlesme);
                    ButonlariGosterGizle();
                }

                
            }
        }
        private void ButonlariGosterGizle()
        {
            bool isVisible = BolgeIdQS == ProjeConstants.HEPSI_INT || BolgeIdQS == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? true : false;
            if (isVisible)
            {
                UtilityHelper.SetControlState(true, true, DevirAlBtn, KiraciTasinmazDegistirBtn, BelgeSilBtn, 
                    NextBtn, PrevBtn, UpdateBtn, DeleteBtn, SozlesmeYenileBtn, SozlesmeyiBitirBtn, SozlesmeyiFeshetBtn,
                    OdemePlaniGoruntuleBtn, KiraciBtn, KiraSozlesmeListBtn);

            }
            else
            {
                UtilityHelper.SetControlState(false, false, DevirAlBtn, KiraciTasinmazDegistirBtn, BelgeSilBtn, 
                    NextBtn, PrevBtn, UpdateBtn, DeleteBtn, SozlesmeYenileBtn, SozlesmeyiBitirBtn, SozlesmeyiFeshetBtn,
                    OdemePlaniGoruntuleBtn,KiraciBtn,KiraSozlesmeListBtn);
            }
        }
        private void AktifSozlesmeYukle(KiraSozlesme kiraSozlesme)
        {

            if (!Page.IsPostBack)
            {
                DateTime today = DateTime.Today;
                OdemeSekliDDLDoldur();

                if (!string.IsNullOrEmpty(KiraSozlesmeIdQS))
                {
                    IdLbl.Text = "(Sözleşme NO: " + kiraSozlesme.Id.ToString() +" Dosya No:"+ kiraSozlesme.DosyaNo + ") ";
                    Kiraci kiraci = new Kiraci();
                    kiraci = kiraci.Select<Kiraci>(kiraSozlesme.KiraciId);
                    if (kiraci != null)
                    {
                        KiraciIdQS = kiraSozlesme.KiraciId.ToString();
                        AdiLbl.Text = kiraci.Adi + " " + kiraci.Soyadi;
                    }
                    SozlesmeTasinmazTablosunuDoldur(kiraSozlesme);
                    KiraSozlesmeFormunuDoldur(kiraSozlesme);
                    if (SozlesmeTarihiBosMu(kiraSozlesme)) //tarihler bossa
                    {
                        OdemePlaniGoruntuleBtn.Visible = false;
                        UpdateBtn.Visible = true;
                        DeleteBtn.Visible = true;
                    }
                    else if (kiraSozlesme.SozBitTar < today)// tarihler dolu fakat eski ise
                    {
                        UyariLbl.Text = @"Sözleşmenin süresi doldu. 
Bu kiracı ve taşınmazlar için yeniden sözleşme yapmak için SÖZLEŞMEYİ YENİLE düğmesine basınız ";
                        SozlesmeYenileBtn.Visible = true;
                        SozlesmeyiBitirBtn.Visible = true;
                        UpdateBtn.Visible = false;
                        DeleteBtn.Visible = false;
                        OdemePlaniGoruntuleBtn.Visible = true;// TODO geçici olarak true Yapildi false olmali;
                        KiraciTasinmazDegistirBtn.Visible = false;
                    }
                    else//tarihler dolu sözlesme devam ediyorsa
                    {
                        OdemePlaniGoruntuleBtn.Visible = true;// GecerliOdemePlaniVarMi(kiraSozlesme);
                        UpdateBtn.Visible = true;
                        DeleteBtn.Visible = true;
                        SozlesmeyiFeshetBtn.Visible = true;
                    }
                    if (!kiraSozlesme.Aktif)
                    {
                        KiraciTasinmazDegistirBtn.Visible = false;
                        UpdateBtn.Visible = false;
                        DeleteBtn.Visible = false;
                        SozlesmeYenileBtn.Visible = false;
                        SozlesmeyiBitirBtn.Visible = false;
                        SozlesmeyiFeshetBtn.Visible = false;
                        OdemePlaniGoruntuleBtn.Visible = false;
                        UyariLbl.Text = "Sözleşme Bitmiştir.";
                        UyariLbl.Font.Bold = true;
                    }
                    DateTime ikiAySonra = today.AddMonths(2);
                    DateTime gelecekAySonGun = (new DateTime(ikiAySonra.Year, ikiAySonra.Month, 1)).AddDays(-1);
                    kiraSozlesme.SozBitTar = (kiraSozlesme.SozBitTar < ProjeConstants.REFERANS_TARIHI) ? SozBitTarTxt.Value.ConvertToDatetime() : kiraSozlesme.SozBitTar;
                    if (kiraSozlesme.SozBitTar <= gelecekAySonGun)// sözlesme bitimine 1 ay kalmis ise
                    {
                        UyariLbl.Text = @"Sözleşmenin süresi doldu. " + System.Environment.NewLine + "Bu kiracı ve taşınmazlar için yeniden sözleşme yapmak için SÖZLEŞMEYİ YENİLE düğmesine basınız ";
                        SozlesmeYenileBtn.Visible = true;
                    }
                }

            }
        }
        private void BitenSozlesmeYukle(KiraSozlesme kiraSozlesme)
        {
            if (!Page.IsPostBack)
            {
                DateTime today = DateTime.Today;
                OdemeSekliDDLDoldur();
                SozlesmeDurumuDDLDoldur();

                if (kiraSozlesme != null && !kiraSozlesme.Aktif)
                {
                    IdLbl.Text = "(Sözleşme NO: " + kiraSozlesme.Id.ToString() + " Dosya No:" + kiraSozlesme.DosyaNo + ") ";
                    Kiraci kiraci = new Kiraci();
                    kiraci = kiraci.Select<Kiraci>(kiraSozlesme.KiraciId);
                    if (kiraci != null)
                    {
                        KiraciIdQS = kiraSozlesme.KiraciId.ToString();
                        AdiLbl.Text = kiraci.Adi + " " + kiraci.Soyadi;
                    }
                    SozlesmeTasinmazTablosunuDoldur(kiraSozlesme);
                    KiraSozlesmeFormunuDoldur(kiraSozlesme);
                    if (SozlesmeTarihiBosMu(kiraSozlesme)) //tarihler bossa
                    {
                        OdemePlaniGoruntuleBtn.Visible = false;
                        UpdateBtn.Visible = true;
                        DeleteBtn.Visible = true;
                    }
                    else
                    {
                        UpdateBtn.Visible = true;
                        DeleteBtn.Visible = true;
                        OdemePlaniGoruntuleBtn.Visible = true;
                    }

                }
            }
        }
        private void OdemeSekliDDLDoldur()
        {
            OdemeSekliDDL.Items.Clear();
            OdemeSekliDDL.Items.Add(ProjeConstants.KIRA_ODMSEKLI_AYLIK);//"Aylik");
            OdemeSekliDDL.Items.Add(ProjeConstants.KIRA_ODMSEKLI_YILLIK);//"Yillik");
            TaksitSayisiTxt.Text = "12";

        }
        private void SozlesmeDurumuDDLDoldur()
        {
            SozlesmeDurumuDDL.Items.Clear();
            SozlesmeDurumuDDL.Items.Add(new ListItem(""));
            SozlesmeDurumuDDL.Items.Add(new ListItem(ProjeConstants.KIRASOZLESME_DURUMU_YENILENDI));
            SozlesmeDurumuDDL.Items.Add(new ListItem(ProjeConstants.KIRASOZLESME_DURUMU_BITTI));
            SozlesmeDurumuDDL.Items.Add(new ListItem(ProjeConstants.KIRASOZLESME_DURUMU_FESIH));
            SozlesmeDurumuDDL.Items.Add(new ListItem(ProjeConstants.KIRASOZLESME_DURUMU_TAKIP));
        }
        private void SozlesmeTasinmazTablosunuDoldur(KiraSozlesme kiraSozlesme)
        {
            KiralikTable.Rows.Clear();
            string[] headers = { "Sıra", "Adres" };
            UtilityHelper.SetTableHeaders(KiralikTable,headers);
            SozlesmeTasinmaz st = new SozlesmeTasinmaz();
            DataTable dataTable = st.SelectBySozlesmeIdReturnDataTable(kiraSozlesme.Id);
            if (dataTable!=null)
            {
                int sira=0;
                foreach (DataRow row in dataTable.Rows)
                {
                    TableRow tableRow = new TableRow();
                    TableCell siraCell = new TableCell();
                    siraCell.Text = (++sira).ToString();
                    TableCell adresCell = new TableCell();  
                    adresCell.Text = row["AdresBolumNoIliIlcesi"].ReturnEmptyIfNull().ToString();
                    tableRow.Controls.Add(siraCell);
                    tableRow.Controls.Add(adresCell);
                    KiralikTable.Rows.Add(tableRow);
                }
            }
        }
        private void KiraSozlesmeFormunuDoldur(KiraSozlesme kiraSozlesme)
        {
            DevirAnaParaTxt.Value = kiraSozlesme.DevirAnaPara.ToString();
            DevirFaizTutariTxt.Text = kiraSozlesme.DevirFaizTutari.ToString();
            DevirFaizliBakiyeTxt.Value = kiraSozlesme.DevirFaizliBakiye.ToString();
            DosyaNoTxt.Text = kiraSozlesme.DosyaNo.ReturnEmptyIfNull().ToString();

            DateTime today = DateTime.Today;

            IlkSozlesmeTarTxt.Value = kiraSozlesme.DosyaNo.ReturnEmptyIfNull().ToString();
            IlkSozlesmeTarTxt.Value = string.IsNullOrEmpty(kiraSozlesme.IlkSozlesmeTar.ConvertToDatetimeEmptyIfNull()) ? today.ConvertToDatetimeEmptyIfNull() : kiraSozlesme.IlkSozlesmeTar.ConvertToDatetimeEmptyIfNull();
            SozBasTarTxt.Value = string.IsNullOrEmpty(kiraSozlesme.SozBasTar.ConvertToDatetimeEmptyIfNull()) ? today.ConvertToDatetimeEmptyIfNull() : kiraSozlesme.SozBasTar.ConvertToDatetimeEmptyIfNull();

            SozBitTarTxt.Value = string.IsNullOrEmpty(kiraSozlesme.SozBitTar.ConvertToDatetimeEmptyIfNull()) ? today.AddYears(1).ConvertToDatetimeEmptyIfNull() : kiraSozlesme.SozBitTar.ConvertToDatetimeEmptyIfNull();
            OdemeSekliDDL.SelectedValue = kiraSozlesme.OdemeSekli == "" ? OdemeSekliDDL.SelectedValue : kiraSozlesme.OdemeSekli;
            KiraBedeliTxt.Value = kiraSozlesme.KiraBedeli.ToString();
            KefilAdiSoyadiTxt.Text = kiraSozlesme.KefilAdiSoyadi;
            KefilTcKimlikNoTxt.Text = kiraSozlesme.KefilTCKimlikNo;
            KefilAdresTxt.Text = kiraSozlesme.KefilAdresi;
            KefilTelTxt.Text = kiraSozlesme.KefilTel;
            TaksitSayisiTxt.Text = kiraSozlesme.TaksitSayisi.ToString().Equals("0") ? "12" : kiraSozlesme.TaksitSayisi.ToString();

            TeminatCinsiTxt.Text = kiraSozlesme.TeminatCinsi;
            TeminatTutariTxt.Value = kiraSozlesme.TeminatTutari.ToString("N",culturInfo);
            OdenenTeminatTutariTxt.Value = kiraSozlesme.OdenenTeminatTutari.ToString("N", culturInfo);
            IadeTeminatTutariTxt.Value = kiraSozlesme.IadeTeminatTutari.ToString("N", culturInfo);
            KalanTeminatTutariTxt.Value = kiraSozlesme.KalanTeminatTutari.ToString("N", culturInfo);
            TeminatTutariTxt.Value = kiraSozlesme.TeminatTutari.ToString();
            TeminatOdemeTarTxt.Value = kiraSozlesme.TeminatOdemeTarihi.ConvertToDatetimeEmptyIfNull();
            
            TeminatAciklamaTxt.Text = kiraSozlesme.TeminatAciklama;
            AciklamaTxt.Text = kiraSozlesme.Aciklama;
            ArtisAyiTxt.Text = kiraSozlesme.ArtisAyi;

            if (!kiraSozlesme.Aktif)
            {
                DurumDegismeTarTxt.Value = kiraSozlesme.DurumDegismeTar.ConvertToDatetimeEmptyIfNull();
                SozlesmeDurumuDDL.SelectedValue = kiraSozlesme.SozlesmeDurumu;
                if (SozlesmeDurumuDDL.Items.FindByText(kiraSozlesme.SozlesmeDurumu) != null)
                    SozlesmeDurumuDDL.SelectedValue = SozlesmeDurumuDDL.Items.FindByText(kiraSozlesme.SozlesmeDurumu).Value;
            }
        }
        private bool SozlesmeTarihiBosMu(KiraSozlesme kiraSozlesme)
        {
            bool retVal = false;
            if (string.IsNullOrEmpty(kiraSozlesme.SozBasTar.ConvertToDatetimeEmptyIfNull()))
                retVal = true;
            if (string.IsNullOrEmpty(kiraSozlesme.SozBitTar.ConvertToDatetimeEmptyIfNull()))
                retVal = true;
            //if (kiraSozlesme.KiraBedeli < 1)
            //    retVal = false;
            return retVal;
        }

        /// <summary>
        /// Sözlesmede kritik alanlar :  sözlesme baslama ve bitis tarihi, kira bedeli, taksit sayisi
        /// Devir Anapara ve Devir Faiz Tutari degisti ise;
        ///     burada dikkate alinmaz, 
        ///     sözlesme güncelleme sirasinda ödeme planinda devir anapara ve devir faiz tutarini günceller
        ///     bakiniz : 
        /// </summary>
        /// <param name="kiraSozlesme"></param>
        /// <returns>degisti ise true degismedi ise false
        /// </returns>
        private bool SozlesmedeKritikAlanlarDegistiMi(KiraSozlesme kiraSozlesme)
        {
            bool retVal = false;
            if (!kiraSozlesme.SozBasTar.ConvertToDatetimeEmptyIfNull().Equals(SozBasTarTxt.Value))
                retVal = true;
            if (!kiraSozlesme.SozBitTar.ConvertToDatetimeEmptyIfNull().Equals(SozBitTarTxt.Value))
                retVal = true;
            if (kiraSozlesme.KiraBedeli != KiraBedeliTxt.Value.ConvertToDecimal())
                retVal = true;
            if (kiraSozlesme.TaksitSayisi != TaksitSayisiTxt.Text.ConvertToInt())
                retVal = true;
            //if (kiraSozlesme.DevirAnaPara != DevirAnaParaTxt.Text.ConvertToDecimal())
            //    retVal = true;
            //if (kiraSozlesme.DevirFaizTutari!= DevirFaizTutariTxt.Text.ConvertToDecimal())
            //    retVal = true;
            return retVal;
        }
        private bool ValidateTosave()
        {
            bool retVal = true;
            if (string.IsNullOrEmpty(SozBasTarTxt.Value.ConvertToDatetimeEmptyIfNull()))
                retVal = false;
            if (string.IsNullOrEmpty(SozBitTarTxt.Value.ConvertToDatetimeEmptyIfNull()))
                retVal = false;
            if (string.IsNullOrEmpty(KiraBedeliTxt.Value))
                retVal = false;
            else if (KiraBedeliTxt.Value.ConvertToDecimal() < 1)
                retVal = false;
            return retVal;
        }
        private bool UpdateKiraSozlesme()
        {
            bool issaved = false;
            if (ValidateTosave())
            {
                try
                {
                    //decimal kb = Convert.ToDecimal(KiraBedeliTxt.Value);
                    KiraSozlesme kiraSozlesme = UpdateKiraSozlesmeData2Db();
                    if (kiraSozlesme != null)
                    {
                        issaved = true;
                        MessageHelper.PublishMessage("Kira Sözleşmesi Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);

                    }
                    else
                    {
                        issaved = false;
                        MessageHelper.PublishMessage("Kira Sözleşmesi Kayıt işlemi başarısız oldu.", ProjeConstants.MESAJ_HATA);
                    }
                }
                catch (Exception)
                {
                    issaved = false;
                    MessageHelper.PublishMessage(" Geçerli bir Kira Bedeli girmediniz!", ProjeConstants.MESAJ_HATA);
                }

            }
            else
            {
                issaved = false;
                MessageHelper.PublishMessage("Sözleşme Başlangıç ve Bitiş Tarihleri ile Kira Bedeli alanları dolu olmalıdır", ProjeConstants.MESAJ_HATA);
            }
            return issaved;
        }
        private KiraSozlesme UpdateKiraSozlesmeData2Db()
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            kiraSozlesme.DosyaNo = DosyaNoTxt.Text.ConvertToInt();
            kiraSozlesme.DevirAnaPara = DevirAnaParaTxt.Value.ConvertToDecimal();
            kiraSozlesme.DevirFaizTutari = DevirFaizTutariTxt.Text.ConvertToDecimal();
            kiraSozlesme.DevirFaizliBakiye = DevirFaizliBakiyeTxt.Value.ConvertToDecimal();
            kiraSozlesme.IlkSozlesmeTar = IlkSozlesmeTarTxt.Value.ConvertToDatetime();
            kiraSozlesme.SozBasTar = SozBasTarTxt.Value.ConvertToDatetime();
            kiraSozlesme.SozBitTar = SozBitTarTxt.Value.ConvertToDatetime();
            kiraSozlesme.OdemeSekli = OdemeSekliDDL.SelectedValue;
            //taksit sayisi 1 ile 12 arasinda ekrandan girilen sayi olsun
            int taksitSayisi = TaksitSayisiTxt.Text.ConvertToInt() >= 12 ? 12 : TaksitSayisiTxt.Text.ConvertToInt() < 1 ? 1 : TaksitSayisiTxt.Text.ConvertToInt();
            //taksit sayisi sabit olsun istenirse aylik=12, yillik =1 olacak sekilde asagida
            //int taksitSayisi = 12;
            //if (OdemeSekliDDL.SelectedValue.Equals(ProjeConstants.KIRA_ODMSEKLI_YILLIK))
            //    taksitSayisi = 1;

            kiraSozlesme.TaksitSayisi = taksitSayisi;
            kiraSozlesme.KiraBedeli = KiraBedeliTxt.Value.ConvertToDecimal();
            kiraSozlesme.KefilAdiSoyadi = KefilAdiSoyadiTxt.Text;
            kiraSozlesme.KefilTCKimlikNo = KefilTcKimlikNoTxt.Text;
            kiraSozlesme.KefilAdresi = KefilAdresTxt.Text;
            kiraSozlesme.KefilTel = KefilTelTxt.Text;
            //kiraSozlesme.TeminatCinsi = TeminatCinsiTxt.Text;
            //kiraSozlesme.TeminatTutari = TeminatTutariTxt.Value.ConvertToDecimal();
            //kiraSozlesme.OdenenTeminatTutari = OdenenTeminatTutariTxt.Value.ConvertToDecimal();
            //kiraSozlesme.KalanTeminatTutari = KalanTeminatTutariTxt.Value.ConvertToDecimal();
            //kiraSozlesme.TeminatOdemeTarihi = TeminatOdemeTarTxt.Value.ConvertToDatetime();
            //kiraSozlesme.TeminatIadeTarihi = TeminatIadeTarTxt.Value.ConvertToDatetime();
            //kiraSozlesme.TeminatAciklama = AciklamaTxt.Text;
            kiraSozlesme.ArtisAyi = ArtisAyiTxt.Text;
            kiraSozlesme.Aciklama = AciklamaTxt.Text;
            if (!kiraSozlesme.Aktif)
            {
                kiraSozlesme.SozlesmeDurumu = SozlesmeDurumuDDL.SelectedValue;
                kiraSozlesme.DurumDegismeTar = DurumDegismeTarTxt.Value.ConvertToDatetime(); 
            }
            kiraSozlesme.BolgeId = BolgeIdGetir(kiraSozlesme);
            kiraSozlesme.GecikmeZammiTipi = string.IsNullOrEmpty(kiraSozlesme.GecikmeZammiTipi)?ProjeConstants.KIRASOZLESME_GECIKMEZAMMI_AYLIK: kiraSozlesme.GecikmeZammiTipi;
            bool guncellendiMi = kiraSozlesme.Update();
            if (guncellendiMi)
            {
                //devirAnaPara, devirFaiz, devirFaizliBakiye degisti ise OdemePlanini güncelle
                OdemePlani odemePlani = new OdemePlani();
                odemePlani = odemePlani.SelectBySozlesmeIdSira(kiraSozlesme.Id, 0);
                if (odemePlani != null)
                {
                    if ((odemePlani.AnaPara != DevirAnaParaTxt.Value.ConvertToDecimal())
                        || (odemePlani.FaizTutari != DevirFaizTutariTxt.Text.ConvertToDecimal())
                        || (odemePlani.FaizliBakiye != DevirFaizliBakiyeTxt.Value.ConvertToDecimal()))
                    {
                        decimal devirAnaPara = DevirAnaParaTxt.Value.ConvertToDecimal();
                        decimal devirFaizTutari = DevirFaizTutariTxt.Text.ConvertToDecimal();

                        odemePlani.AnaPara = devirAnaPara;
                        odemePlani.FaizTutari = devirFaizTutari;
                        odemePlani.FaizliBakiye = devirAnaPara + devirFaizTutari;
                        odemePlani.Update();
                        if ((kiraSozlesme.Aktif)&&(kiraSozlesme.SozlesmeDurumu.Equals(ProjeConstants.KIRASOZLESME_DURUMU_TAKIP)))
                        {
                            HukukiiIslemlereEkle(kiraSozlesme);
                        }
                        else
                        {
                            HukukiIslemleriGuncelle(kiraSozlesme);
                        }
                    }
                }

            }
            return kiraSozlesme;
        }
        private void SozlesmeSil()
        {

            try
            {
                bool sozlesmeSilindi = false;
                bool odemePlaniSilindi = false;
                bool odemePlaniVar = false;
                bool odemeSilindi = false;
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
                if (kiraSozlesme != null)
                {
                    int kiraSozlesmeId = kiraSozlesme.Id;
                    odemePlaniVar = GecerliOdemePlaniVarMi(kiraSozlesme);
                    if (odemePlaniVar)
                    {
                        odemePlaniSilindi = OdemePlaniSil(kiraSozlesme);
                    }
                    if (odemePlaniSilindi || !odemePlaniVar)
                    {
                        sozlesmeSilindi = kiraSozlesme.Delete();

                    }
                    if (sozlesmeSilindi)
                    {
                        Odeme odeme = new Odeme();
                        odemeSilindi = odeme.DeleteBySozlesmeId(kiraSozlesmeId);
                        OdemeAyrinti odemeAyrintiDao = new OdemeAyrinti();
                        List<OdemeAyrinti> odemeAyrintiListesi = odemeAyrintiDao.SelectBySozlesmeId(kiraSozlesme.Id);
                        if (odemeAyrintiListesi.Count > 0)
                        {
                            odemeAyrintiDao.DeleteBySozlesmeId(kiraSozlesme.Id);
                        }

                    }
                    SozlesmeTasinmaz st = new SozlesmeTasinmaz();
                    bool sozlesmeTasinmazSilindi = st.DeleteBySozlesmeId(kiraSozlesme.Id);
                }
            }
            catch (Exception e)
            {

                ExceptionHelper exhelper = new ExceptionHelper(e);
                exhelper.PublishException();
            }



        }
        private bool SozlesmeGuncelle(KiraSozlesme kiraSozlesme)
        {
            bool kiraSozlesmeGuncellendi = false;
            try
            {
                bool odemePlaniVar = false;
                bool odemePlaniSilindi = true;
               
                if (kiraSozlesme != null)
                {
                    odemePlaniVar = GecerliOdemePlaniVarMi(kiraSozlesme);
                    if (odemePlaniVar)
                    {
                        odemePlaniSilindi = OdemePlaniSil(kiraSozlesme);
                    }
                    kiraSozlesmeGuncellendi = UpdateKiraSozlesme();
                }
               
            }
            catch (Exception e)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "CloseModal();", true);
                ExceptionHelper exhelper = new ExceptionHelper(e);
                exhelper.PublishException();
            }
            return kiraSozlesmeGuncellendi;
        }
        private bool OdemePlaniSil(KiraSozlesme kiraSozlesme)
        {
            bool silindi = false;
            OdemePlani odemePlaniDao = new OdemePlani();
            List<OdemePlani> list = odemePlaniDao.SelectBySozlesmeId(kiraSozlesme.Id);
            if (list.Count > 0)
            {
                OdemePlani op = list[0];
                if (op.SozlesmeId == kiraSozlesme.Id)
                {
                    silindi = op.DeleteBySozlesmeId(kiraSozlesme.Id);
                    if (silindi)
                    {
                        //Odeme odemeDao = new Odeme();
                        //List<Odeme> odemeListesi = odemeDao.SelectBySozlesmeId(kiraSozlesme.Id);
                        //if (list.Count > 0)
                        //{
                        //    odemeDao.DeleteBySozlesmeId(kiraSozlesme.Id);
                        //}
                        OdemeAyrinti odemeAyrintiDao = new OdemeAyrinti();
                        List<OdemeAyrinti> odemeAyrintiListesi = odemeAyrintiDao.SelectBySozlesmeId(kiraSozlesme.Id);
                        if (odemeAyrintiListesi.Count > 0)
                        {
                            odemeAyrintiDao.DeleteBySozlesmeId(kiraSozlesme.Id);
                        }
                    }
                }
            }
            return silindi;
        }
        private bool GecerliOdemePlaniVarMi(KiraSozlesme kiraSozlesme)
        {
            bool odemePlaniVar = false;
            //halen devam eden bir ödeme plani var mi
            OdemePlani odemePlaniDao = new OdemePlani();
            List<OdemePlani> list = odemePlaniDao.SelectBySozlesmeId(kiraSozlesme.Id);
            if (list.Count > 0)
            {
                OdemePlani odemePlani = list[0];
                odemePlaniVar = odemePlani.SozlesmeId == kiraSozlesme.Id;
            }


            return odemePlaniVar;
        }
        private decimal BorcuVarMi(KiraSozlesme kiraSozlesme, bool isSozlesmeyiBitir)
        {
            bool borcuVar = true;
            decimal faizliBakiye = 0;
            ExceptionHelper exHelper = new ExceptionHelper();
            try
            {
                //halen devam eden bir ödeme plani var mi
                OdemePlani odemePlani = new OdemePlani();
                if (isSozlesmeyiBitir)
                {
                    odemePlani = odemePlani.SelectSonOdemePlaniBySozlesmeId(kiraSozlesme.Id);
                }
                else
                {
                    DateTime today = DateTime.Today;
                    odemePlani = odemePlani.SelectBySozlesmeIdOdemeTarihi(kiraSozlesme.Id, today);
                }
                if (odemePlani == null)
                {

                    exHelper.Exceptions.Add(new Exception("Ödeme Planı Bulunamadı"));
                }
                else
                {
                    faizliBakiye = odemePlani.FaizliBakiye;
                    borcuVar = odemePlani.FaizliBakiye < 0;
                }
            }
            catch (Exception)
            {

                exHelper.PublishException();
            }

            return faizliBakiye;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void KiraciTasinmazDegistirBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string rootUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
            string newUrl = "/" + ProjeConstants.PAGE_KIRASOZLESME_TASINMAZ + "?DestinationApp=ST&SenderApp=KS&KiraSozlesmeId=" + kiraSozlesme.Id;
            Page.Response.Redirect(rootUrl + newUrl, true);
        }
        protected void KiraSozlesmeListBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KIRASOZLESME_LIST + "?SecilenId=" + KiraSozlesmeIdQS);
        }
        protected void KiraciBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string rootUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
            string newUrl = "/" + ProjeConstants.PAGE_KIRACI_GIRIS + "?DestinationApp=KD&SenderApp=KL&KiraciId=" + KiraciIdQS;
            Page.Response.Redirect(rootUrl + newUrl, true);
        }
        private void OncekiSayfayaDon()
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string rootUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
            string newUrl = "/" + ProjeConstants.PAGE_KIRACI_LIST;

            if (SenderAppQS != null && SenderAppQS.Equals("KSL"))
                newUrl = "/" + ProjeConstants.PAGE_KIRASOZLESME_LIST;
            else if (SenderAppQS != null && SenderAppQS.Equals("OPL"))
                newUrl = "/" + ProjeConstants.PAGE_ODEMEPLANI_LIST;
            else if (SenderAppQS != null && SenderAppQS.Equals("KD"))
                newUrl = "/" + ProjeConstants.PAGE_KIRACI_GIRIS + "?DestinationApp=KD&KiraciId=" + KiraciIdQS;
            Page.Response.Redirect(rootUrl + newUrl, true);
        }
        protected void OdemeSekliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaksitSayisiTxt.Text = "12";
            if (OdemeSekliDDL.SelectedItem.Value.Equals(ProjeConstants.KIRA_ODMSEKLI_YILLIK))
            {
                TaksitSayisiTxt.Text = "1";
                TaksitSayisiTxt.Enabled = true;
            }
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                SozlesmeTasinmazTablosunuDoldur(kiraSozlesme);
            }
                
        }
        protected void OdemePlaniGoruntuleBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            if (string.IsNullOrEmpty(kiraSozlesme.SozBasTar.ConvertToDatetimeEmptyIfNull()) || string.IsNullOrEmpty(kiraSozlesme.SozBitTar.ConvertToDatetimeEmptyIfNull()))
            {
                MessageHelper.PublishMessage("Ödeme planı Açılamıyor. Lütfen sözleşmenin başlama ve bitiş tarihlerini girerek tekrar deneyin.", ProjeConstants.MESAJ_BILGI, 2000);
            }
            else if (kiraSozlesme != null)
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                newUrl += "/" + ProjeConstants.PAGE_ODEMEPLANI + "?SenderApp=" + SenderAppQS + "&KiraSozlesmeId=" + kiraSozlesme.Id;
                Page.Response.Redirect(newUrl, true);
            }

        }
        private int BolgeIdGetir(KiraSozlesme yeniSozlesme)
        {
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select(yeniSozlesme.KiraciId);
            string ili = kiraci == null ? "" : kiraci.Ili;
            Il il = new Il();
            il = il.SelectByIlAdi(kiraci.Ili);
            int bolgeId = il == null ? 0 : il.BolgeId;
            return bolgeId;
        }
        private bool HukukiIslemleriGuncelle(KiraSozlesme kiraSozlesme)
        {
            bool islemBaslatildi = false;
            try
            {
                HukukiTakip hukukiTakip = new HukukiTakip();
                hukukiTakip = hukukiTakip.SelectBySozlesmeId(kiraSozlesme.Id);
                if (hukukiTakip != null)
                {
                    hukukiTakip.IslemTarihi = DateTime.Today;
                    hukukiTakip.Aktif = false;
                    hukukiTakip.Aciklama = ProjeConstants.KIRASOZLESME_DURUMU_TAKIPSONUCLANDI;
                    hukukiTakip.Olusturan = CurrentUserName;
                    int hukukitakipId = hukukiTakip.Save();
                    islemBaslatildi = hukukitakipId > 0;
                }

            }
            catch (Exception e)
            {
                ExceptionHelper exHelper = new ExceptionHelper();
                exHelper.Exceptions.Add(e);
                exHelper.Exceptions.Add(new Exception("Hukuki Takip Başlatılamadı"));
            }
            return islemBaslatildi;
        }
        private bool HukukiiIslemlereEkle(KiraSozlesme kiraSozlesme)
        {
            bool islemBaslatildi = false;
            try
            {
                HukukiTakip hukukiTakip = new HukukiTakip();
                hukukiTakip.SozlesmeId = kiraSozlesme.Id;
                hukukiTakip.KiraciId = kiraSozlesme.KiraciId;
                hukukiTakip.BorcAnaPara = DevirAnaParaHesapla(kiraSozlesme);
                hukukiTakip.BorcFaiz = DevirFaizTutariHesapla(kiraSozlesme);
                hukukiTakip.IslemTarihi = DateTime.Today;
                hukukiTakip.Aktif = true;
                hukukiTakip.Aciklama = ProjeConstants.KIRASOZLESME_DURUMU_TAKIP;
                hukukiTakip.Olusturan = CurrentUserName;
                int hukukitakipId = hukukiTakip.Save();
                islemBaslatildi = hukukitakipId > 0;
            }
            catch (Exception e)
            {
                ExceptionHelper exHelper = new ExceptionHelper();
                exHelper.Exceptions.Add(e);
                exHelper.Exceptions.Add(new Exception("Hukuki Takip Başlatılamadı"));
            }
            return islemBaslatildi;
        }
        private decimal SozlesmeBasTarIcinTufeArtisliKiraBedeliBul(KiraSozlesme kiraSozlesme, DateTime yenibastar)
        {
            decimal yeniKiraBedeli = kiraSozlesme.KiraBedeli;
            YasalFaiz yasalFaiz = new YasalFaiz();
            yasalFaiz = yasalFaiz.SelectByYilAy(yenibastar.Year, yenibastar.Month);
            if (yasalFaiz != null)
            {
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select(kiraSozlesme.KiraciId);
                if ((yenibastar >= ProjeConstants.SINIRLIKIRAARTISI_BASLAMATARIHI) &&
                    (yenibastar <= ProjeConstants.SINIRLIKIRAARTISI_BITISTARIHI) &&
                    kiraci.KiralamaAmaci.Equals(ProjeConstants.SINIRLIKIRAARTISI_UYGULANACAKTASINMAZCINSI))
                {
                    yeniKiraBedeli = kiraSozlesme.KiraBedeli + kiraSozlesme.KiraBedeli * ProjeConstants.SINIRLIKIRAARTISI_ORANI / 100;
                }
                else
                {
                    yeniKiraBedeli = kiraSozlesme.KiraBedeli + kiraSozlesme.KiraBedeli * yasalFaiz.Tufe / 100; 
                }
            }
            return yeniKiraBedeli;
        }
        private void SozlesmeyiBitir()
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                kiraSozlesme.UpdateAktifDurum(ProjeConstants.KIRASOZLESME_DURUMU_BITTI, ModalDurumDegismeTarTxt.Text, ProjeConstants.KIRASOZLESME_AKTIFDEGILBOOL);
                RedirectToPage(ProjeConstants.PAGE_KIRASOZLESME_LIST + "?SozlesmeId=" + KiraSozlesmeIdQS);
            }
        }
        private void SozlesmeyiYenile()
        {
            int yeniId = 0;
            try
            {
                //mevcut Sözlesmeyi bitir
                int eskiKiraSozlesmeId = KiraSozlesmeIdQS.ConvertToInt();
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(eskiKiraSozlesmeId);
                if (kiraSozlesme != null)
                {
                    kiraSozlesme.Degistiren = CurrentUserName;
                    // TO DO SB atomic
                    kiraSozlesme.UpdateAktifDurum(ProjeConstants.KIRASOZLESME_DURUMU_YENILENDI, ModalDurumDegismeTarTxt.Text, ProjeConstants.KIRASOZLESME_AKTIFDEGILBOOL);
                    //yeni Sözlesmeyi kaydet
                    KiraSozlesme yeniSozlesme = new KiraSozlesme();
                    yeniSozlesme = kiraSozlesme;
                    //yeniSozlesme.Id = yeniId;
                    yeniSozlesme.KiraciId = kiraSozlesme.KiraciId;
                    yeniSozlesme.IlkSozlesmeTar = string.IsNullOrEmpty(kiraSozlesme.IlkSozlesmeTar.ConvertToDatetimeEmptyIfNull()) ? DateTime.Today : kiraSozlesme.IlkSozlesmeTar;
                    DateTime bastar = string.IsNullOrEmpty(kiraSozlesme.SozBasTar.ConvertToDatetimeEmptyIfNull()) ? DateTime.Today : kiraSozlesme.SozBasTar;
                    DateTime yenibastar = bastar.AddYears(1);
                    DateTime yenibittar = yenibastar.AddYears(1);
                    yeniSozlesme.SozBasTar = yenibastar;
                    yeniSozlesme.SozBitTar = yenibittar;
                    yeniSozlesme.KiraBedeli = Math.Round(SozlesmeBasTarIcinTufeArtisliKiraBedeliBul(kiraSozlesme, yenibastar));
                    yeniSozlesme.DevirAnaPara = DevirAnaParaHesapla(kiraSozlesme);
                    yeniSozlesme.DevirFaizliBakiye = DevirFaizliBakiyeHesapla(kiraSozlesme);
                    yeniSozlesme.DevirFaizTutari = DevirFaizTutariHesapla(kiraSozlesme);
                    yeniSozlesme.ArtisAyi = ArtisAyiTxt.Text;
                    //if (!string.IsNullOrEmpty(kiraSozlesme.TeminatIadeTarihi.ConvertToDatetimeEmptyIfNull()))
                    //    yeniSozlesme.TeminatIadeTarihi = kiraSozlesme.TeminatIadeTarihi.AddYears(1);

                    yeniSozlesme.SozlesmeDurumu = ProjeConstants.KIRASOZLESME_DURUMU_DEVAM;
                    yeniSozlesme.Degistiren = CurrentUserName;
                    yeniSozlesme.BolgeId = BolgeIdGetir(yeniSozlesme);
                    yeniId = yeniSozlesme.Save();
                    yeniSozlesme.Id = yeniId;
                    //eski sözlesme tasinmaz bilgilerini al
                    SozlesmeTasinmaz st = new SozlesmeTasinmaz();
                    List<SozlesmeTasinmaz> stlist = st.SelectBySozlesmeId(eskiKiraSozlesmeId);
                    //yeni sözlesmeye aktar ve kaydet
                    foreach (SozlesmeTasinmaz item in stlist)
                    {
                        SozlesmeTasinmaz yeniST = new SozlesmeTasinmaz();
                        yeniST.SozlesmeId = yeniSozlesme.Id;
                        yeniST.TasinmazId = item.TasinmazId;
                        yeniST.BolumId = item.BolumId;
                        yeniST.Save();
                    }
                }
                //yeni sözlesmeye git
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_KIRASOZLESMESI + "?KiraSozlesmeId=" + yeniId;
                Page.Response.Redirect(newUrl);

            }
            catch (Exception ex)
            {

                MessageHelper.PublishMessage("Hata" + ex.Message, ProjeConstants.MESAJ_HATA);
            }
        }
        private decimal DevirFaizTutariHesapla(KiraSozlesme kiraSozlesme)
        {
            DateTime islemTar = DateTime.Today;
            if (kiraSozlesme.SozlesmeDurumu.Equals(ProjeConstants.KIRASOZLESME_DURUMU_TAKIP))
            {
                islemTar = islemTar == null || islemTar < ProjeConstants.REFERANS_TARIHI ? islemTar : kiraSozlesme.DurumDegismeTar;
            }
            decimal faizTutari = 0;
            if (kiraSozlesme != null)
            {
                OdemePlani odemePlani = new OdemePlani();
                odemePlani = odemePlani.SelectBySozlesmeIdOdemeTarihi(kiraSozlesme.Id, islemTar);
                if (odemePlani == null)
                {
                    odemePlani = new OdemePlani();
                    odemePlani = odemePlani.SelectSonOdemePlaniBySozlesmeId(kiraSozlesme.Id);
                }
                if (odemePlani == null)
                {
                    faizTutari = 0;
                }
                else
                {
                    faizTutari = odemePlani.FaizliBakiye - odemePlani.AnaPara;
                }

            }
            return faizTutari;
        }
        private decimal DevirFaizliBakiyeHesapla(KiraSozlesme kiraSozlesme)
        {
            DateTime islemTar = DateTime.Today;
            if (kiraSozlesme.SozlesmeDurumu.Equals(ProjeConstants.KIRASOZLESME_DURUMU_TAKIP))
            {
                islemTar = islemTar == null || islemTar < ProjeConstants.REFERANS_TARIHI ? islemTar : kiraSozlesme.DurumDegismeTar;
            }
            decimal faizliBakiye = 0;
            if (kiraSozlesme != null)
            {
                OdemePlani odemePlani = new OdemePlani();
                odemePlani = odemePlani.SelectBySozlesmeIdOdemeTarihi(kiraSozlesme.Id, islemTar);
                if (odemePlani == null)
                {
                    odemePlani = new OdemePlani();
                    odemePlani = odemePlani.SelectSonOdemePlaniBySozlesmeId(kiraSozlesme.Id);
                }
                if (odemePlani == null)
                {
                    faizliBakiye = 0;
                }
                else
                {
                    faizliBakiye = odemePlani.FaizliBakiye;
                }

            }
            return faizliBakiye;
        }
        private decimal DevirAnaParaHesapla(KiraSozlesme kiraSozlesme)
        {
            DateTime islemTar = DateTime.Today;
            if (kiraSozlesme.SozlesmeDurumu.Equals(ProjeConstants.KIRASOZLESME_DURUMU_TAKIP))
            {
                islemTar = islemTar == null || islemTar < ProjeConstants.REFERANS_TARIHI ? islemTar : kiraSozlesme.DurumDegismeTar;
            }
            decimal anaPara = 0;
            if (kiraSozlesme != null)
            {
                OdemePlani odemePlani = new OdemePlani();
                odemePlani = odemePlani.SelectBySozlesmeIdOdemeTarihi(kiraSozlesme.Id, islemTar);//.SelectSonOdemePlaniBySozlesmeId(kiraSozlesme.Id);
                if (odemePlani == null)
                {
                    odemePlani = new OdemePlani();
                    odemePlani = odemePlani.SelectSonOdemePlaniBySozlesmeId(kiraSozlesme.Id);
                }
                if (odemePlani == null)
                {
                    anaPara = 0;
                }
                else
                {
                    anaPara = odemePlani.AnaPara;
                }

            }
            return anaPara;
        }
        protected void SozlesmeYenileBtn_Click(object sender, EventArgs e)
        {
            ModalLbl.Text = "Sözleşme Yenilenecek";
            ModalDurumDegismeTarTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                ArtisOraniDiv.Attributes["style"] = "display:block";
                YeniKiraBedeliDiv.Attributes["style"] = "display:block";

                ModalDurumDegismeTarTxt.Text = kiraSozlesme.SozBitTar.AddDays(1).ConvertToDatetimeEmptyIfNull();
                decimal faizliBakiye = 0;
                bool odemePlaniVar = GecerliOdemePlaniVarMi(kiraSozlesme);
                if (odemePlaniVar)
                {
                    faizliBakiye = BorcuVarMi(kiraSozlesme, true);
                }

                // Sözlesmenin borcu var mi
                // Yoksa güncellemeyi yap

                if (faizliBakiye < 0)
                {
                    SenderHF.Value = "YenileBorcuDevret";
                    OdemePlaniBtn.Visible = true;
                    OnaylaBtn.Text = "Sözleşmeyi Yenile ve Borcu Devret";
                    MessageLbl.Text = @"
                        Sözleşmeye ait ödenmemiş " + (faizliBakiye * -1).ToString("N", culturInfo) + @"TL borç bulunmaktadir. Sözleşmeyi yenilemeden önce ödeme yapmak için ÖDEME PLANI gidebilirsiniz veya
                        Sözleşmeye ait kira borcunu yeni sözleşmeye devredebilirsiniz";
                    UtilityHelper.ScriptCalistir("OpenModal();");
                }
                else// borcu yok bakiye=0, faiz tutari= 0
                {
                    SenderHF.Value = "Yenile";
                    OdemePlaniBtn.Visible = true;
                    OnaylaBtn.Text = "Sözleşmeyi Yenile";
                    MessageLbl.Text = @"
                        Sözleşmeye ait ödenmemiş borç bulunmamaktadir. Onayladiginiz takdirde müteakip yil için sözleşme yenilenecek. ";
                    UtilityHelper.ScriptCalistir("OpenModal();");

                }
                DateTime bastar = string.IsNullOrEmpty(kiraSozlesme.SozBasTar.ConvertToDatetimeEmptyIfNull()) ? DateTime.Today : kiraSozlesme.SozBasTar;
                DateTime yenibastar = bastar.AddYears(1);
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select(kiraSozlesme.KiraciId);
                if ((yenibastar >= ProjeConstants.SINIRLIKIRAARTISI_BASLAMATARIHI) &&
                    (yenibastar <= ProjeConstants.SINIRLIKIRAARTISI_BITISTARIHI) &&
                    kiraci.KiralamaAmaci.Equals(ProjeConstants.SINIRLIKIRAARTISI_UYGULANACAKTASINMAZCINSI))
                {
                    ArtisOraniTxt.Text = ProjeConstants.SINIRLIKIRAARTISI_ORANI.ToString();
                    YeniKiraBedeliTxt.Text = (kiraSozlesme.KiraBedeli + kiraSozlesme.KiraBedeli * ProjeConstants.SINIRLIKIRAARTISI_ORANI / 100).ToString("N", culturInfo);
                    ArtisOraniLbl.Text = "%" + ProjeConstants.SINIRLIKIRAARTISI_ORANI + " Uygulaması kapsamındadır.";
                    ArtisOraniLbl.ForeColor = System.Drawing.Color.Blue;
                }
                else
                {
                    YasalFaiz yasalFaiz = new YasalFaiz();

                    yasalFaiz = yasalFaiz.SelectByYilAy(yenibastar.Year, yenibastar.Month);
                    if (yasalFaiz != null)
                    {
                        decimal tufe = yasalFaiz.Tufe;
                        decimal yeniKiraBedeli = kiraSozlesme.KiraBedeli + kiraSozlesme.KiraBedeli * tufe / 100;
                        ArtisOraniTxt.Text = tufe.ToString();
                        YeniKiraBedeliTxt.Text = yeniKiraBedeli.ToString("N", culturInfo);
                        ArtisOraniLbl.Text = "%" + ProjeConstants.SINIRLIKIRAARTISI_ORANI + " uygulaması kapsamında değildir.";
                        ArtisOraniLbl.ForeColor = System.Drawing.Color.Black;
                    }

                    
                }

            }


            //borcu var mi kontrol et
            //borcu yoksa sözlesmeyi bitir
            //borcu varsa Popup Aç
            // (a) Borcu ödemesi için odeme planina link olsun .
            // ÖdemePlani.aspx açilsin
            // (b) Sözlesmeyi Yenile ve Borcu Yeni sözlesmeye Devret butonu olsun
            // sözlesmeyi bitir
            // yeni sözlesme olustur
            // son odeme planindaki anapara(Bakiye) ve FaizTutarini yeni sözlesmeye yaz
        }
        protected void SozlesmeyiBitirBtn_Click(object sender, EventArgs e)
        {
            ArtisOraniDiv.Attributes["style"] = "display:none";
            YeniKiraBedeliDiv.Attributes["style"] = "display:none";

            ModalLbl.Text = "Sözleşme Bitirilecek";
            ModalDurumDegismeTarTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                decimal faizliBakiye = 0;
                bool odemePlaniVar = GecerliOdemePlaniVarMi(kiraSozlesme);
                if (odemePlaniVar)
                {
                    faizliBakiye = BorcuVarMi(kiraSozlesme, true);
                }

                // Sözlesmenin borcu var mi
                // Yoksa güncellemeyi yap

                if (faizliBakiye < 0)
                {
                    SenderHF.Value = "SozlesmeBittiHukukiIslem";
                    OdemePlaniBtn.Visible = true;
                    OnaylaBtn.Text = "Sözleşmeyi Feshet ve Hukuki İşlemlere Ekle";
                    MessageLbl.Text = @"
                        Sözleşmeye ait ödenmemiş " + (faizliBakiye * -1).ToString("N", culturInfo) + @"TL borç bulunmaktadir. Sözleşmeyi bitirmeden önce ödeme yapmak için ÖDEME PLANI'na gidebilirsiniz veya
                        Sözleşmeyi 'Hukuki İşlem Gören Sözleşmeler' arasina ekleyebilirsiniz";
                    UtilityHelper.ScriptCalistir("OpenModal();");
                }
                else// borcu yok bakiye=0, faiz tutari= 0
                {
                    SenderHF.Value = "SozlesmeyiBitir";
                    OdemePlaniBtn.Visible = true;
                    OnaylaBtn.Text = "Sözleşmeyi Bitir";
                    MessageLbl.Text = @"
                        Sözleşmeye ait borç bulunmamaktadir. Sözleşmeyi bitir dügmesine bastiginizda, bu sözleşme arsive alinacaktir.";
                    UtilityHelper.ScriptCalistir("OpenModal();");
                }
            }
        }
        protected void SozlesmeyiFeshetBtn_Click(object sender, EventArgs e)
        {
            ArtisOraniDiv.Attributes["style"] = "display:none";
            YeniKiraBedeliDiv.Attributes["style"] = "display:none";

            ModalLbl.Text = "Sözleşme Feshedilecek";
            ModalDurumDegismeTarTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                decimal faizliBakiye = 0;
                bool odemePlaniVar = GecerliOdemePlaniVarMi(kiraSozlesme);
                if (odemePlaniVar)
                {
                    faizliBakiye = DevirFaizliBakiyeHesapla(kiraSozlesme);//BorcuVarMi(kiraSozlesme,false);
                }

                // Sözlesmenin borcu var mi
                // Yoksa güncellemeyi yap

                if (faizliBakiye < 0)
                {
                    SenderHF.Value = "FesihHukukiIslem";
                    OdemePlaniBtn.Visible = true;
                    OnaylaBtn.Text = "Sözleşmeyi Feshet";
                    MessageLbl.Text = string.Format(@"
                        <br> * Sözleşmeye ait ödenmemiş {0} TL  borç bulunmaktadir. 
                        <br> * Sözleşmeyi feshetmeden önce ödeme yapmak için ÖDEME PLANI'na gidebilirsiniz veya
                        <br> * Sözleşmeyi Feshet dügmesine basarak 'Hukuki İşlem Gören Sözleşmeler' arasina ekleyebilirsiniz.", (faizliBakiye * -1).ToString("N", culturInfo));
                    ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "OpenModal();", true);
                }
                else// borcu yok bakiye=0, faiz tutari= 0
                {
                    SenderHF.Value = "SozlesmeyiFeshet";
                    OdemePlaniBtn.Visible = true;
                    OnaylaBtn.Text = "Sözleşmeyi Feshet";
                    MessageLbl.Text = @"
                         <br> * Sözleşmeye ait borç bulunmamaktadir. 
                         <br> * Sözleşmeyi Feshet dügmesine bastiginizda, bu sözleşmenin ödeme planindaki vadesi gelmemis kira bedelleri '0' (sifir) yapilacak ve sözleşme arsive alinacaktir.";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "OpenModal();", true);
                }

            }
        }
        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            ModalLbl.Text = "Sözleşme Güncellenecek";
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                ArtisOraniDiv.Attributes["style"] = "display:none";
                YeniKiraBedeliDiv.Attributes["style"] = "display:none";

                // Sözleşmenin kritik bilgilerinde degisiklik var mi (Sozbastar,sozbittar,taksitsayisi,kirabedeli)
                // Yoksa güncellemeyi yap
                bool sozlesmeDegisti = SozlesmedeKritikAlanlarDegistiMi(kiraSozlesme);
                if (sozlesmeDegisti)
                {
                    SenderHF.Value = "Update";
                    MessageLbl.Text = @"Onayladığınız takdirde, bu sözleşme güncellenmeden önce sözleşmeye ait ÖDEME PLANI silinecek.
Bu durumda daha önce yapilan ödeme var ise silinmesi bilgi kaybina yolaçabilir.
Ayrica sözleşme güncellendikten sonra yeni ödeme plani olusturmalisiniz.";
                    OnaylaBtn.Text = " Sözleşmeyi Güncelle ";
                    UtilityHelper.ScriptCalistir( "OpenModal();");
                }
                else
                {
                    UpdateKiraSozlesme();
                    AktifSozlesmeYukle(kiraSozlesme);
                }
                PDFKaydet(kiraSozlesme);
                PDFGoster(kiraSozlesme);
                SozlesmeTasinmazTablosunuDoldur(kiraSozlesme);
            }

        }
        protected void DeleteBtn_Click(object sender, EventArgs e)
        {
            ArtisOraniDiv.Attributes["style"] = "display:none";
            YeniKiraBedeliDiv.Attributes["style"] = "display:none";
            ModalLbl.Text = "Sözleşme Silinecek";
            SenderHF.Value = "Delete";
            MessageLbl.Text = @"Onayladığınız takdirde bu sözleşme ve bu sözleşmeye ait ÖDEME PLANI silinecek. 
Bu durumda daha önce yapilan ödeme var ise silinmesi bilgi kaybina yolaçabilir.";
            OnaylaBtn.Text = " Sözleşmeyi Sil ";
            UtilityHelper.ScriptCalistir("OpenModal();");
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                SozlesmeTasinmazTablosunuDoldur(kiraSozlesme);
            }

        }
        protected void OnaylaBtn_Click(object sender, EventArgs e)
        {
            UtilityHelper.ScriptCalistir("CloseModal();");
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                if (SenderHF.Value.Equals("Delete"))
                {
                    SozlesmeSil();
                    OncekiSayfayaDon();
                }
                else if (SenderHF.Value.Equals("Update"))
                {

                    bool guncellendiMi= SozlesmeGuncelle(kiraSozlesme);
                    if (guncellendiMi)
                    {
                        if (kiraSozlesme.Aktif)
                        {
                            AktifSozlesmeYukle(kiraSozlesme);
                        }
                        else
                        {
                            BitenSozlesmeYukle(kiraSozlesme);
                        }
                        SozlesmeTasinmazTablosunuDoldur(kiraSozlesme);
                    }


                    //string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    //Page.Response.Redirect(currentUrl, true);
                }
                else if (SenderHF.Value.Equals("YenileBorcuDevret"))
                {

                    SozlesmeyiYenile();
                }
                else if (SenderHF.Value.Equals("Yenile"))
                {

                    SozlesmeyiYenile();
                }
                else if (SenderHF.Value.Equals("SozlesmeyiBitir"))
                {

                    SozlesmeyiBitir();
                }
                else if (SenderHF.Value.Equals("SozlesmeBittiHukukiIslem"))
                {

                    HukukiIslemlereEkle(kiraSozlesme);
                    kiraSozlesme.UpdateAktifDurum(ProjeConstants.KIRASOZLESME_DURUMU_TAKIP, ModalDurumDegismeTarTxt.Text, ProjeConstants.KIRASOZLESME_AKTIFDEGILBOOL);
                    RedirectToPage(ProjeConstants.PAGE_KIRASOZLESME_LIST + "?SozlesmeId=" + KiraSozlesmeIdQS);
                }
                else if (SenderHF.Value.Equals("SozlesmeyiFeshet"))
                {
                    kiraSozlesme.SozlesmeDurumu = ProjeConstants.KIRASOZLESME_DURUMU_FESIH;
                    kiraSozlesme.DurumDegismeTar = ModalDurumDegismeTarTxt.Text.ConvertToDatetime();
                    kiraSozlesme.Aktif = ProjeConstants.KIRASOZLESME_AKTIF_DEGIL_INT.ConvertToBool();
                    kiraSozlesme.Update();
                    //kiraSozlesme.UpdateAktifDurum(ProjeConstants.KIRASOZLESME_DURUMU_FESIH, ModalDurumDegismeTarTxt.Value);
                    OdemePlanindaVadesiGelmeyenleriSifirYap(kiraSozlesme, ModalDurumDegismeTarTxt.Text);
                    RedirectToPage(ProjeConstants.PAGE_KIRASOZLESME_LIST + "?SozlesmeId=" + KiraSozlesmeIdQS);
                }
                else if (SenderHF.Value.Equals("FesihHukukiIslem"))
                {
                    kiraSozlesme.SozlesmeDurumu = ProjeConstants.KIRASOZLESME_DURUMU_TAKIP;
                    kiraSozlesme.DurumDegismeTar = ModalDurumDegismeTarTxt.Text.ConvertToDatetime();
                    kiraSozlesme.Aktif = ProjeConstants.KIRASOZLESME_AKTIF_DEGIL_INT.ConvertToBool();
                    kiraSozlesme.Update();
                    //kiraSozlesme.UpdateAktifDurum(ProjeConstants.KIRASOZLESME_DURUMU_TAKIP, ModalDurumDegismeTarTxt.Value);
                    OdemePlanindaVadesiGelmeyenleriSifirYap(kiraSozlesme, ModalDurumDegismeTarTxt.Text);
                    HukukiIslemlereEkle(kiraSozlesme);
                    RedirectToPage(ProjeConstants.PAGE_KIRASOZLESME_LIST + "?SozlesmeId=" + KiraSozlesmeIdQS);
                }
            }

        }
        private void OdemePlanindaVadesiGelmeyenleriSifirYap(KiraSozlesme kiraSozlesme, string degistirmeTar)
        {
            DateTime durumDegistirmeTar = string.IsNullOrEmpty(degistirmeTar) ? DateTime.Now : degistirmeTar.ConvertToDatetime();
            OdemePlani buAyinOdemePlani = new OdemePlani();
            buAyinOdemePlani = buAyinOdemePlani.SelectBySozlesmeIdOdemeTarihi(kiraSozlesme.Id, durumDegistirmeTar);
            OdemePlani odemePlaniDao = new OdemePlani();
            List<OdemePlani> odemePlaniList = odemePlaniDao.SelectBySozlesmeId(kiraSozlesme.Id);
            foreach (OdemePlani item in odemePlaniList)
            {
                if (item.Sira > buAyinOdemePlani.Sira)
                {
                    item.KiraBedeli = 0;
                    item.AnaPara = 0;
                    item.FaizTutari = 0;
                    item.FaizliBakiye = 0;
                    item.FaizOrani = 0;
                    item.Update();
                }
            }
        }
        private bool HukukiIslemlereEkle(KiraSozlesme kiraSozlesme)
        {
            bool islemBaslatildi = false;
            try
            {
                HukukiTakip hukukiTakip = new HukukiTakip();
                hukukiTakip.SozlesmeId = kiraSozlesme.Id;
                hukukiTakip.KiraciId = kiraSozlesme.KiraciId;
                hukukiTakip.BorcAnaPara = DevirAnaParaHesapla(kiraSozlesme);
                hukukiTakip.BorcFaiz = DevirFaizTutariHesapla(kiraSozlesme);
                hukukiTakip.IslemTarihi = DateTime.Today;
                hukukiTakip.Aktif = true;
                hukukiTakip.Aciklama = "Takip başlatıldı.";
                hukukiTakip.Olusturan = CurrentUserName;
                int hukukitakipId = hukukiTakip.Save();
                islemBaslatildi = hukukitakipId > 0;
            }
            catch (Exception e)
            {
                ExceptionHelper exHelper = new ExceptionHelper();
                exHelper.Exceptions.Add(e);
                exHelper.Exceptions.Add(new Exception("Hukuki Takip Başlatılamadı"));
            }
            return islemBaslatildi;
        }
        protected void OdemePlaniBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
            newUrl += "/" + ProjeConstants.PAGE_ODEMEPLANI + "?SenderApp=" + SenderAppQS + "&KiraSozlesmeId=" + KiraSozlesmeIdQS;
            Page.Response.Redirect(newUrl, true);

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
        protected void PrevBtn_Click(object sender, EventArgs e)
        {
            try
            {
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
                KiraSozlesme oncekiKiraSozlesme = kiraSozlesme.SelectPrev();

                RedirectToPage(ProjeConstants.PAGE_KIRASOZLESMESI + "?DestinationApp=KS&KiraSozlesmeId=" + oncekiKiraSozlesme.Id);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }


        }
        protected void NextBtn_Click(object sender, EventArgs e)
        {
            try
            {
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.Select<KiraSozlesme>(KiraSozlesmeIdQS.ConvertToInt());
                KiraSozlesme sonrakiKiraSozlesme = kiraSozlesme.SelectNext();

                RedirectToPage(ProjeConstants.PAGE_KIRASOZLESMESI + "?DestinationApp=KS&KiraSozlesmeId=" + sonrakiKiraSozlesme.Id);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void DevirAlBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select(KiraSozlesmeIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                KiraSozlesme oncekiKiraSozlesmesi = kiraSozlesme.SelectOncekiKiraSozlesme();
                if (oncekiKiraSozlesmesi != null)
                {
                    OdemePlani odemePlaniDao = new OdemePlani();
                    List<OdemePlani> odemePlaniList = odemePlaniDao.SelectBySozlesmeId(oncekiKiraSozlesmesi.Id);
                    OdemePlani odemePlani = odemePlaniList[odemePlaniList.Count - 1];
                    decimal sonAnaPara = odemePlani.AnaPara;
                    decimal sonFaizliBakiye = odemePlani.FaizliBakiye;
                    decimal sonFaizTutari = sonFaizliBakiye - sonAnaPara;
                    DevirAnaParaTxt.Value = sonAnaPara.ToString("N", culturInfo);
                    DevirFaizTutariTxt.Text = sonFaizTutari.ToString("N", culturInfo);
                    DevirFaizliBakiyeTxt.Value = sonFaizliBakiye.ToString("N", culturInfo);
                }
                else
                {
                    int sifir = 0;
                    DevirAnaParaTxt.Value = sifir.ToString("N", culturInfo);
                    DevirFaizTutariTxt.Text = sifir.ToString("N", culturInfo);
                    DevirFaizliBakiyeTxt.Value = sifir.ToString("N", culturInfo);
                }
            }
        }
        protected void TeminatIslemleriBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_TEMINAT_ISLEMLERI + "?KiraSozlesmeId=" + KiraSozlesmeIdQS);
        }
        #region dosya yukle/goruntule
        protected void BelgeSilBtn_Click(object sender, EventArgs e)
        {
            try
            {
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.Select(KiraSozlesmeIdQS.ConvertToInt());
                if (kiraSozlesme != null)
                {

                    string dosyaAdi = kiraSozlesme.SozlesmePDFDosyasi; 
                    if (string.IsNullOrEmpty(dosyaAdi))
                    {
                        MessageHelper.PublishMessage("Silinecek dosya bulunamadı", ProjeConstants.MESAJ_HATA);
                        return;
                    }
                    if (UtilityHelper.DeleteFileFromSharePointLib(ProjeConstants.PATH_TBYS_URL, ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi))
                    {
                        MessageHelper.PublishMessage(dosyaAdi + " Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                        BelgeSilBtn.Visible = false;
                        DosyaLnk.Visible = false;
                        BelgeYukleFU.Visible = true;

                        if (kiraSozlesme != null)
                            SozlesmeTasinmazTablosunuDoldur(kiraSozlesme);
                    }
                    else
                    {
                        MessageHelper.PublishMessage(dosyaAdi + " Silinemedi", ProjeConstants.MESAJ_HATA);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper eh = new ExceptionHelper(ex);
                eh.PublishException();
            }
        }
        private void PDFKaydet(KiraSozlesme kiraSozlesme)
        {
            try
            {
                if (BelgeYukleFU.HasFile)
                {
                    string hedefDosyaAdi = ProjeConstants.DOSYA_KIRASOZLESMESI + kiraSozlesme.Id + ".pdf";
                    bool isOk = UtilityHelper.UploadFileToSharePoint(BelgeYukleFU, ProjeConstants.TBYSBELGELERI_LIB, hedefDosyaAdi);
                    if (isOk)
                    {
                        kiraSozlesme.SozlesmePDFDosyasi = hedefDosyaAdi;
                        kiraSozlesme.Update();
                        DosyaLnk.Visible = true;
                        BelgeSilBtn.Visible = true;
                        BelgeYukleFU.Visible=false; 
                    }
                    else
                    {
                        DosyaLnk.Visible=false; 
                        BelgeSilBtn.Visible=false;  
                        BelgeYukleFU.Visible = true;

                    }
                }
                else
                {
                    DosyaLnk.Visible = false;
                    BelgeSilBtn.Visible = false;
                }
            }
            catch (Exception exception)
            {
                Exception ex = new Exception("Dosya Yüklenemedi");
                ExceptionHelper exhelper = new ExceptionHelper(exception);
                exhelper.Exceptions.Add(ex);
                exhelper.PublishException();
            }
        }
        private void PDFGoster(KiraSozlesme kiraSozlesme)
        {
            try
            {
                if (kiraSozlesme != null)
                {
                    string dosyaAdi = kiraSozlesme.SozlesmePDFDosyasi;
                    string dosyaUrl = UtilityHelper.TbysBelgelerURLGetir() + "/" + dosyaAdi;
                    bool dosyaVarMi = UtilityHelper.DosyaVarMi(UtilityHelper.TbysBelgelerURLGetir(), ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi);
                    if (dosyaVarMi)
                    {
                        string belgePdfLink = @"'<a class=\'btn btn-secondary\' data-fancybox data-type=pdf data-width=960 data-height=720 href=" + dosyaUrl + @"> Sözleşme Görüntüle </a>'";

                        DosyaLnk.Target = "_blank";
                        DosyaLnk.HRef = dosyaUrl;

                        DosyaLnk.Visible = true;
                        BelgeSilBtn.Visible = true;
                        BelgeYukleFU.Visible = false;
                    }
                    else
                    {
                        DosyaLnk.Visible = false;
                        BelgeSilBtn.Visible = false;
                        BelgeYukleFU.Visible = true;
                    }

                }
                else
                {
                    MessageHelper.PublishMessage("Sözleşme bulunamadı.", ProjeConstants.MESAJ_HATA);
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
        #endregion
    }
}
