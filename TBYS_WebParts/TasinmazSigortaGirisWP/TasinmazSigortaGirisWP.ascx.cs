using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.TasinmazSigortaGirisWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazSigortaGirisWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazSigortaGirisWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string PageIndexQS
        {
            get
            {

                if (ViewState["PageIndex"] == null)
                {
                    if (Page.Request.QueryString["PageIndex"] != null)
                    {
                        ViewState["PageIndex"] = Page.Request.QueryString["PageIndex"];
                    }
                    else
                    {
                        ViewState["PageIndex"] = string.Empty;
                    }
                }
                return ViewState["PageIndex"].ToString();
            }

            set
            {
                ViewState["PageIndex"] = value;
            }
        }
        private string SigortaIdQS
        {
            get
            {

                if (ViewState["SigortaId"] == null)
                {
                    if (Page.Request.QueryString["SigortaId"] != null)
                    {
                        ViewState["SigortaId"] = Page.Request.QueryString["SigortaId"];
                    }
                    else
                    {
                        ViewState["SigortaId"] = string.Empty;
                    }
                }
                return ViewState["SigortaId"].ToString();
            }

            set
            {
                ViewState["SigortaId"] = value;
            }
        }
        private string TasinmazIdQS
        {
            get
            {

                if (ViewState["TasinmazId"] == null)
                {
                    if (Page.Request.QueryString["TasinmazId"] != null)
                    {
                        ViewState["TasinmazId"] = Page.Request.QueryString["TasinmazId"];
                    }
                    else
                    {
                        ViewState["TasinmazId"] = string.Empty;
                    }
                }
                return ViewState["TasinmazId"].ToString();
            }

            set
            {
                ViewState["TasinmazId"] = value;
            }
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
        private string EnvanterdeMiQS
        {
            get
            {

                if (ViewState["EnvanterdeMi"] == null)
                {
                    if (Page.Request.QueryString["EnvanterdeMi"] != null)
                    {
                        ViewState["EnvanterdeMi"] = Page.Request.QueryString["EnvanterdeMi"];
                    }
                    else
                    {
                        ViewState["EnvanterdeMi"] = string.Empty;
                    }
                }
                return ViewState["EnvanterdeMi"].ToString();
            }

            set
            {
                ViewState["EnvanterdeMi"] = value;
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
            //önceki sayfayı tut, geri tuşuna basıldığında gerekli
            if (!Page.IsPostBack)
            {
                if (String.IsNullOrEmpty(DestinationAppQS) || String.Equals(DestinationAppQS, ""))
                {
                    SigortaGirisi();
                }
                else if (String.Equals(DestinationAppQS, "SigortaD"))
                {
                    SigortaDuzenle();
                }

            }
        }
        private void SigortaGirisi()
        {
            SaveBtn.Visible = true;
            UpdateBtn.Visible = false;
            SilBtn.Visible = false;
            BackBtn.Visible = false;
            NextBtn.Visible = false;
            PrevBtn.Visible = false;
        }
        private void SigortaDuzenle()
        {
            SaveBtn.Visible = false;
            UpdateBtn.Visible = true;
            SilBtn.Visible = true;
            BackBtn.Visible = true;
            NextBtn.Visible = true;
            PrevBtn.Visible = true;

            TitleLbl.Attributes["Class"] = "text-success";
            TitleLbl.Text = "Sigorta Güncelleme";

            Sigorta sigorta = new Sigorta();
            sigorta = sigorta.Select<Sigorta>(SigortaIdQS.ConvertToInt());
            if (sigorta != null)
            {

                SigortaFormunuDoldur(sigorta);
                PDFGoster(sigorta);
                TasinmazIdQS = sigorta.TasinmazId.ToString();
                DateTime sigortaBitisTarihi = sigorta.SigortaBitTar.ConvertToDatetime();
                if (sigortaBitisTarihi <= DateTime.MinValue)
                {
                    MessageHelper.PublishMessage("Sigorta Bitiş Tarihi Boş!", ProjeConstants.MESAJ_HATA);
                }
                else if (sigortaBitisTarihi <= DateTime.Today.AddMonths(1))
                {
                    MessageHelper.PublishMessage("Sigorta Vadesi " + sigorta.SigortaBitTar.ToString("dd.MM.yyyy") + " Tarihinde dolacaktır", ProjeConstants.MESAJ_BILGI);
                }
            }

        }
        private bool SigortaFormunuDoldur(Sigorta sigorta)
        {

            bool formDolduMu = false;
            try
            {
                SigortaCinsiDDLDoldur();
                BagimsizBolumDDLDoldur(sigorta);

                AdresKoduTxt.Text = sigorta.AdresKodu.ToString();
                BrutYuzolcumuTxt.Text = sigorta.BrutYuzolcumu;
                BulunduguKatTxt.Text = sigorta.BulunduguKat;
                MetrekareTxt.Text = sigorta.Metrekare;
                ToplamKatSayisiTxt.Text = sigorta.ToplamKatSayisi;
                InsaYiliTxt.Text = sigorta.InsaYili;
                PoliceNoTxt.Text = sigorta.PoliceNo.ToString();
                DaskPoliceNoTxt.Text = sigorta.DaskPoliceNo;
                SigBasTarTxt.Text = sigorta.SigortaBasTar.ConvertToDatetimeEmptyIfNull();
                SigBitTarTxt.Text = sigorta.SigortaBitTar.ConvertToDatetimeEmptyIfNull();
                SigortaCinsiDDL.SelectedValue = sigorta.SigortaCinsi;
                IdLbl.Text = sigorta.Id + " #TasinmazId:" + sigorta.TasinmazId.ToString();
                SigortaBedeliTxt.Text = sigorta.SigortaBedeli.ToString();
                PrimTxt.Text = sigorta.Prim.ToString();
                YapiTarziTxt.Text = sigorta.YapiTarzi;
                TeminatListCheckBoxDoldur(sigorta.TeminatListesi);
                AciklamaTxt.Text = sigorta.Aciklama;
               
                BagimsizBolumNoTxt.Text = string.IsNullOrEmpty(sigorta.BagimsizBolumNo)?BagimsizBolumDDL.SelectedItem.Text: sigorta.BagimsizBolumNo;
                Tasinmaz tasinmaz = new Tasinmaz();
                tasinmaz = tasinmaz.Select<Tasinmaz>(sigorta.TasinmazId);
                if (tasinmaz != null)
                {
                    AdiLbl.Text = tasinmaz.KullanimSekli + " - " + tasinmaz.Adres + " - " + tasinmaz.Ilcesi + "/" + tasinmaz.Ili;
                    BrutYuzolcumuTxt.Text = string.IsNullOrEmpty(sigorta.BrutYuzolcumu) ? tasinmaz.Nitelik : sigorta.BrutYuzolcumu; //nitelik Bolumunde yuzolcumu bilgisi kayıtlı olduğından onun yüzolcumune yazması için
                    BulunduguKatTxt.Text = string.IsNullOrEmpty(tasinmaz.BulunduguKat) ? sigorta.BulunduguKat : tasinmaz.BulunduguKat;
                    MetrekareTxt.Text = string.IsNullOrEmpty(tasinmaz.Metrekare) ? sigorta.Metrekare : tasinmaz.Metrekare;
                    TapuTasinmazNoTxt.Text = tasinmaz.TapuTasinmazNo;
                }

            }
            catch (Exception exception)
            {
                ExceptionHelper exh = new ExceptionHelper(exception);
                formDolduMu = false;
            }
            return formDolduMu;
        }
        private void TeminatListCheckBoxDoldur(string teminatListesi)
        {
            DepremChk.Checked = teminatListesi.Contains("1");
            YanginChk.Checked = teminatListesi.Contains("2");
            Makine100000Chk.Checked = teminatListesi.Contains("3");
            Makine5000Chk.Checked = teminatListesi.Contains("4");
            JeneratorChk.Checked = teminatListesi.Contains("5");
            AsansorChk.Checked = teminatListesi.Contains("6");
            KazanChk.Checked = teminatListesi.Contains("7");
        }
        private void BagimsizBolumDDLDoldur(Sigorta sigorta)
        {
            BagimsizBolumDDL.Items.Clear();
            //ListItem li0 = new ListItem("", "0");
            //BagimsizBolumDDL.Items.Add(li0);

            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select(sigorta.TasinmazId);
            if (tasinmaz != null)
            {
                if (tasinmaz.KatMulkiyeti== ProjeConstants.KAT_MULKIYETI_VAR)
                {
                    ListItem li = new ListItem(tasinmaz.BagimsizBolumNo, tasinmaz.BagimsizBolumNo);
                    BagimsizBolumDDL.Items.Add(li);
                    BagimsizBolumNoTxt.Text = tasinmaz.BagimsizBolumNo;
                    KullanimAmaciTxt.Text = tasinmaz.KullanimSekli;
                }
                else
                {
                    KullanimAmaciTxt.Text = sigorta.KullanimAmaci;
                    BagimsizBolum bb = new BagimsizBolum();
                    List<BagimsizBolum> list = bb.SelectByTasinmazId(sigorta.TasinmazId);
                    foreach (BagimsizBolum item in list)
                    {
                        ListItem li = new ListItem(item.BolumNo, item.Id.ToString());
                        BagimsizBolumDDL.Items.Add(li);
                    }
                    if (list.Count > 0)
                    {
                        if (BagimsizBolumDDL.Items.FindByValue(sigorta.BolumId.ReturnZeroIfNull().ToString()) != null)
                        {
                            BagimsizBolumDDL.SelectedValue = BagimsizBolumDDL.Items.FindByValue(sigorta.BolumId.ReturnZeroIfNull().ToString()).Value;
                            BagimsizBolumNoTxt.Text = BagimsizBolumDDL.SelectedItem.Text;
                        }
                    }
                       
                    else {
                        BagimsizBolumNoTxt.Text = string.Empty;
                        MessageHelper.PublishMessage("Taşınmazın kat mülkiyeti bulunmamasına rağmen taşınmaza ait bağımsız bölüm bulunmamaktadır.", ProjeConstants.MESAJ_HATA);
                    }
                    
                }
            }
            

            
        }
        private void SigortaCinsiDDLDoldur()
        {
            SigortaCinsiDDL.Items.Clear();
            SigortaCinsiDDL.Items.Add(ProjeConstants.SIGORTA_YOK);
            SigortaCinsiDDL.Items.Add(ProjeConstants.SIGORTA_DASK);
            SigortaCinsiDDL.Items.Add(ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            SigortaCinsiDDL.Items.FindByValue(ProjeConstants.SIGORTA_DEPREM_IHTIYARI).Attributes.Add("Disabled", "Disabled");
        }
        private bool UpdateSigorta(Sigorta sigorta)
        {
            bool guncellendiMi = false;
            
            if (sigorta != null)
            {
                sigorta.AdresKodu = AdresKoduTxt.Text;
                sigorta.BrutYuzolcumu = BrutYuzolcumuTxt.Text;
                sigorta.BulunduguKat = BulunduguKatTxt.Text;
                sigorta.Metrekare = MetrekareTxt.Text;
                sigorta.ToplamKatSayisi = ToplamKatSayisiTxt.Text;
                sigorta.InsaYili = InsaYiliTxt.Text;
                sigorta.PoliceNo = PoliceNoTxt.Text;
                sigorta.DaskPoliceNo = DaskPoliceNoTxt.Text;
                sigorta.SigortaBasTar = SigBasTarTxt.Text.ConvertToDatetime();
                sigorta.SigortaBitTar = SigBitTarTxt.Text.ConvertToDatetime();
                sigorta.SigortaCinsi = SigortaCinsiDDL.SelectedValue;
                sigorta.TasinmazId = TasinmazIdQS.ConvertToInt();
                sigorta.Prim = PrimTxt.Text.ConvertToDecimal();
                sigorta.SigortaBedeli = SigortaBedeliTxt.Text.ConvertToDecimal();
                sigorta.YapiTarzi = YapiTarziTxt.Text;
                sigorta.Degistiren = CurrentUserName;
                sigorta.BolumId = BagimsizBolumDDL.SelectedItem.Value.ConvertToInt();
                sigorta.TeminatListesi = SecilenTeminatlariGetir();
                sigorta.TeminatAciklama = SecilenTeminatAciklamalariniGetir();
                sigorta.BagimsizBolumNo = BagimsizBolumNoTxt.Text;
                sigorta.Aciklama = AciklamaTxt.Text;
                sigorta.KullanimAmaci = KullanimAmaciTxt.Text;
                guncellendiMi = sigorta.Update();
                if (guncellendiMi)
                {
                    Tasinmaz tasinmaz = new Tasinmaz();
                    tasinmaz = tasinmaz.Select<Tasinmaz>(sigorta.TasinmazId);
                    if (tasinmaz != null)
                    {
                        tasinmaz.SigortaDurumu = sigorta.SigortaCinsi;
                        tasinmaz.Update();
                    }
                }
            }
            return guncellendiMi;
        }
        private Sigorta SaveSigorta()
        {
            Sigorta sigorta = new Sigorta();
            sigorta.AdresKodu = AdresKoduTxt.Text;
            sigorta.BrutYuzolcumu = BrutYuzolcumuTxt.Text;
            sigorta.BulunduguKat = BulunduguKatTxt.Text;
            sigorta.Metrekare = MetrekareTxt.Text;
            sigorta.ToplamKatSayisi = ToplamKatSayisiTxt.Text;
            sigorta.InsaYili = InsaYiliTxt.Text;
            sigorta.PoliceNo = PoliceNoTxt.Text;
            sigorta.DaskPoliceNo = DaskPoliceNoTxt.Text;
            sigorta.SigortaBasTar = SigBasTarTxt.Text.ConvertToDatetime();
            sigorta.SigortaBitTar = SigBitTarTxt.Text.ConvertToDatetime();
            sigorta.SigortaCinsi = SigortaCinsiDDL.SelectedItem.Value;
            sigorta.TasinmazId = TasinmazIdQS.ConvertToInt();
            sigorta.Prim = PrimTxt.Text.ConvertToDecimal();
            sigorta.SigortaBedeli = SigortaBedeliTxt.Text.ConvertToDecimal();
            sigorta.YapiTarzi = YapiTarziTxt.Text;
            sigorta.Olusturan = CurrentUserName;
            sigorta.BolumId = BagimsizBolumDDL.SelectedItem.Value.ConvertToInt();
            sigorta.TeminatListesi = SecilenTeminatlariGetir();
            sigorta.TeminatAciklama = SecilenTeminatAciklamalariniGetir();
            sigorta.BagimsizBolumNo = BagimsizBolumNoTxt.Text;
            sigorta.Aciklama = AciklamaTxt.Text;
            sigorta.KullanimAmaci = KullanimAmaciTxt.Text;
            int sigortaId = sigorta.Save();
            sigorta.Id = sigortaId;
            if (sigortaId > 0)
            {
                Tasinmaz tasinmaz = new Tasinmaz();
                tasinmaz = tasinmaz.Select<Tasinmaz>(sigorta.TasinmazId);
                if (tasinmaz != null)
                {
                    tasinmaz.SigortaDurumu = sigorta.SigortaCinsi;
                    tasinmaz.Update();
                }
                string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                int queryIndex = newUrl.IndexOf("?");
                if (queryIndex > 0)
                    newUrl = newUrl.Substring(0, queryIndex);
                RedirectToPage(newUrl + "?DestinationApp=SigortaD&sigortaId=" + sigorta.Id);

            }
            return sigorta;
        }
        private string SecilenTeminatlariGetir()
        {
            string teminatListesi = string.Empty;
            teminatListesi = DepremChk.Checked ? "1," : "";
            teminatListesi += YanginChk.Checked ? "2," : "";
            teminatListesi += Makine100000Chk.Checked ? "3," : "";
            teminatListesi += Makine5000Chk.Checked ? "4," : "";
            teminatListesi += JeneratorChk.Checked ? "5," : "";
            teminatListesi += AsansorChk.Checked ? "6," : "";
            teminatListesi += KazanChk.Checked ? "7," : "";
            teminatListesi = string.IsNullOrEmpty(teminatListesi) ? "" : teminatListesi.Substring(0, teminatListesi.Length - 1); // sondaki virgülü at
            return teminatListesi;
        }
        private string SecilenTeminatAciklamalariniGetir()
        {
            string teminatListesi = string.Empty;
            teminatListesi = DepremChk.Checked ? " #1." + DepremChk.ToolTip.ToString() : "";
            teminatListesi += YanginChk.Checked ? " #2." + YanginChk.ToolTip.ToString() : "";
            teminatListesi += Makine100000Chk.Checked ? " #3." + Makine100000Chk.ToolTip.ToString() : "";
            teminatListesi += Makine5000Chk.Checked ? " #4." + Makine5000Chk.ToolTip.ToString() : "";
            teminatListesi += JeneratorChk.Checked ? " #5." + JeneratorChk.ToolTip.ToString() : "";
            teminatListesi += AsansorChk.Checked ? " #6." + AsansorChk.ToolTip.ToString() : "";
            teminatListesi += KazanChk.Checked ? " #7." + KazanChk.ToolTip.ToString() : "";
            teminatListesi = string.IsNullOrEmpty(teminatListesi) ? "" : teminatListesi.Substring(0, teminatListesi.Length);
            return teminatListesi;
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
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            if (SenderAppQS.Equals("SigortaL"))
                RedirectToPage(ProjeConstants.PAGE_TASINMAZSIGORTA_LIST + "?PageIndex=" + PageIndexQS);
            else
                if (SenderAppQS.Equals("SigortaES"))
                RedirectToPage(ProjeConstants.PAGE_TASINMAZSIGORTA_EKLESIL + "?TasinmazId=" + TasinmazIdQS + "&PageIndex=" + PageIndexQS + "&EnvanterdeMi=" + EnvanterdeMiQS);
            else if (EnvanterdeMiQS.Equals("2"))
                RedirectToPage(ProjeConstants.PAGE_MULKIYETIOLMAYANTASINMAZ_GIRIS + "?TasinmazId=" + TasinmazIdQS + "&DestinationApp=TD" + "&EnvanterdeMi=" + EnvanterdeMiQS);
            else if (SenderAppQS.Equals("TD"))
                RedirectToPage(ProjeConstants.PAGE_TASINMAZ_GIRIS + "?TasinmazId=" + TasinmazIdQS + "&DestinationApp=TD");
        }
        protected void SaveBtn_Click(object sender, EventArgs e)
        {

            try
            {
                Sigorta sigorta = SaveSigorta();
                if (sigorta != null) //kaydettikten sonra önceki sayfaya dön
                {
                    MessageHelper.PublishMessage("Sigorta Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                    RedirectToPage(ProjeConstants.PAGE_TASINMAZSIGORTA_LIST + "?Mesaj=true");

                }
                else
                {
                    MessageHelper.PublishMessage("Bağışçı Kaydedilemedi", ProjeConstants.MESAJ_HATA);
                }


            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Sigorta sigorta = new Sigorta();
                sigorta = sigorta.Select<Sigorta>(SigortaIdQS.ConvertToInt());
                if (sigorta!=null)
                {
                    bool guncellendiMi = UpdateSigorta(sigorta);
                    if (guncellendiMi)
                    {
                        MessageHelper.PublishMessage("Sigorta Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Sigorta Güncellenemedi", ProjeConstants.MESAJ_HATA);
                    } 
                }
                PDFKaydet(sigorta);
                PDFGoster(sigorta);

            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void SilBtn_Click(object sender, EventArgs e)
        {
            Sigorta sigorta = new Sigorta();
            sigorta = sigorta.Select<Sigorta>(SigortaIdQS.ConvertToInt());
            if (sigorta != null)
            {
                Tasinmaz tasinmaz = new Tasinmaz();
                string adres = tasinmaz.SelectByIdBolumId(sigorta.TasinmazId, sigorta.BolumId);
                SilmeMesajiLbl.Text = adres + " adresindeki taşınmaza ait Sigorta silinecek";
                ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "ModalOnay();", true);
            }

        }
        protected void SilNowBtn_Click(object sender, EventArgs e)
        {
            bool silindi = false;
            try
            {
                Sigorta sigorta = new Sigorta();
                sigorta = sigorta.Select<Sigorta>(SigortaIdQS.ConvertToInt());
                if (sigorta != null)
                {
                    silindi = sigorta.Delete();
                }
                ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "CloseModal();", true);
                if (silindi)
                {
                    RedirectToPage(ProjeConstants.PAGE_TASINMAZSIGORTA_LIST + "?Mesaj=true&PageIndex=" + PageIndexQS);
                }
                else
                {
                    MessageHelper.PublishMessage("Sigorta Silinemedi", ProjeConstants.MESAJ_BILGI);
                }
            }
            catch (Exception ex)
            {

                ExceptionHelper exhelper = new ExceptionHelper();
                exhelper.Exceptions.Add(new Exception("Sigorta Silinemedi"));
                exhelper.Exceptions.Add(ex);
                exhelper.PublishException();
            }

        }
        protected void TasinmazBtn_Click(object sender, EventArgs e)
        {

            if (EnvanterdeMiQS.Equals("2"))
                RedirectToPage(ProjeConstants.PAGE_MULKIYETIOLMAYANTASINMAZ_GIRIS + "?TasinmazId=" + TasinmazIdQS + "&DestinationApp=TD");
            else
                RedirectToPage(ProjeConstants.PAGE_TASINMAZ_GIRIS + "?DestinationApp=TD&SenderApp=TL&TasinmazId=" + TasinmazIdQS);
        }
        protected void TasinmazListesiBtn_Click(object sender, EventArgs e)
        {
            if (EnvanterdeMiQS.Equals("2"))
                RedirectToPage(ProjeConstants.PAGE_PAGE_MULKIYETIOLMAYANTASINMAZ_LIST);
            else RedirectToPage(ProjeConstants.PAGE_TASINMAZ_LIST);
        }
        protected void SigortaListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_TASINMAZSIGORTA_LIST);
        }
        protected void NextBtn_Click(object sender, EventArgs e)
        {
            Sigorta sigorta = new Sigorta();
            sigorta = sigorta.Select<Sigorta>(SigortaIdQS.ConvertToInt());
            Sigorta sonrakiSigorta = sigorta.SelectNext(sigorta.Id);
            string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            int queryIndex = newUrl.IndexOf("?");
            if (queryIndex > 0)
                newUrl = newUrl.Substring(0, queryIndex);
            newUrl = newUrl + "?DestinationApp=SigortaD&SenderApp=SigortaL&SigortaId=" + sonrakiSigorta.Id + "&TasinmazId=" + sonrakiSigorta.TasinmazId + "&PageIndex=" + PageIndexQS;

            Page.Response.Redirect(newUrl, true);
        }
        protected void PrevBtn_Click(object sender, EventArgs e)
        {
            Sigorta sigorta = new Sigorta();
            sigorta = sigorta.Select<Sigorta>(SigortaIdQS.ConvertToInt());
            Sigorta oncekiSigorta = sigorta.SelectPrev(sigorta.Id);
            string newUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            int queryIndex = newUrl.IndexOf("?");
            if (queryIndex > 0)
                newUrl = newUrl.Substring(0, queryIndex);
            newUrl = newUrl + "?DestinationApp=SigortaD&SenderApp=SigortaL&SigortaId=" + oncekiSigorta.Id + "&TasinmazId=" + oncekiSigorta.TasinmazId + "&PageIndex=" + PageIndexQS;
            Page.Response.Redirect(newUrl, true);
        }

        protected void MulkiyetiOlmayanTasinmazBtn_Click(object sender, EventArgs e)
        {

        }

        protected void SigBasTarTxt_TextChanged(object sender, EventArgs e)
        {
            DateTime bastar = SigBasTarTxt.Text.ConvertToDatetime();
            SigBitTarTxt.Text = bastar.AddYears(1).ToString("dd.MM.yyyy");
        }

        protected void BagimsizBolumDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            BagimsizBolumNoTxt.Text = BagimsizBolumDDL.SelectedItem.Text;
        }
        #region dosya yukle/goruntule
        protected void BelgeSilBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string dosyaAdi = ProjeConstants.DOSYA_SIGORTAPOLICESI_DASK + SigortaIdQS + ".pdf";
                if (UtilityHelper.DeleteFileFromSharePointLib(ProjeConstants.PATH_TBYS_URL, ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi))
                {
                    MessageHelper.PublishMessage(dosyaAdi + " Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                    BelgeSilBtn.Visible = false;
                    DosyaLnk.Visible = false;
                    BelgeYukleFU.Visible = true;
                    Sigorta sigorta = new Sigorta();
                    sigorta = sigorta.Select<Sigorta>(SigortaIdQS.ConvertToInt());
                    if (sigorta != null)
                        SigortaFormunuDoldur(sigorta);
                }
                else
                {
                    MessageHelper.PublishMessage(dosyaAdi + " Silinemedi", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper eh = new ExceptionHelper(ex);
                eh.PublishException();
            }
        }
        private void PDFKaydet(Sigorta sigorta)
        {
            try
            {
                if (BelgeYukleFU.HasFile)
                {
                    string hedefDosyaAdi = ProjeConstants.DOSYA_SIGORTAPOLICESI_DASK + sigorta.Id + ".pdf";
                    bool isOk = UtilityHelper.UploadFileToSharePoint(BelgeYukleFU, ProjeConstants.TBYSBELGELERI_LIB, hedefDosyaAdi);
                    if (isOk)
                    {
                        sigorta.PDFDosyasi = hedefDosyaAdi;
                        sigorta.Update();
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
        private void PDFGoster(Sigorta sigorta)
        {
            try
            {
                if (sigorta != null)
                {
                    string dosyaAdi = sigorta.PDFDosyasi;
                    string dosyaUrl = UtilityHelper.TbysBelgelerURLGetir() + "/" + dosyaAdi;
                    bool dosyaVarMi = UtilityHelper.DosyaVarMi(UtilityHelper.TbysBelgelerURLGetir(), ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi);
                    if (dosyaVarMi)
                    {
                        string belgePdfLink = @"'<a class=\'btn btn-secondary\' data-fancybox data-type=pdf data-width=960 data-height=720 href=" + dosyaUrl + @"> Poliçe Görüntüle </a>'";

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
                    MessageHelper.PublishMessage("Poliçe bulunamadı.", ProjeConstants.MESAJ_HATA);
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
