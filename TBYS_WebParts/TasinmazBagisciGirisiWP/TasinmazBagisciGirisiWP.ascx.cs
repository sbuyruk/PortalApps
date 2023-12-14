using Microsoft.SharePoint;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.TasinmazBagisciGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazBagisciGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazBagisciGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        private string BagisciIdQS
        {
            get
            {

                if (ViewState["BagisciId"] == null)
                {
                    if (Page.Request.QueryString["BagisciId"] != null)
                    {
                        ViewState["BagisciId"] = Page.Request.QueryString["BagisciId"];
                    }
                    else
                    {
                        ViewState["BagisciId"] = string.Empty;
                    }
                }
                return ViewState["BagisciId"].ToString();
            }

            set
            {
                ViewState["BagisciId"] = value;
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
                DDLleriDoldur();
                DeleteBtn.Visible = true;
                DosyaLnk.Visible = false;
                BelgeSilBtn.Visible = false;
                BelgeYukleFU.Visible = false;
                TaahhutDosyaLnk.Visible = false;
                TaahhutSilBtn.Visible = false;
                TaahhutYukleFU.Visible = false;

                if (string.IsNullOrEmpty(DestinationAppQS) || (DestinationAppQS.Equals("TBG")))
                {
                    //BagisciGirisi açılacak BG
                    OpenBagisciGirisi();
                }
                else if (DestinationAppQS.Equals("TBD"))
                {
                    //BagisciDuzenle açılacak
                    OpenBagisciDuzenle(BagisciIdQS);
                }
            }
        }
        private void OpenBagisciDuzenle(string bagisciId)
        {
            SaveBtn.Visible = false;
            UpdateBtn.Visible = true;
            YakinlariBtn.Visible = true;
            TalepleriBtn.Visible = true;
            TaahhutleriBtn.Visible = true;
            BagislariBtn.Visible = true;

            BagisciMainPanel.Attributes["Class"] = "card";
            TitleLbl.CssClass = "col-form-label text-danger font-weight-bold mb-1";

            TitleLbl.Text = "Bağışçı Bilgisi Güncelleme";

            IdLbl.Visible = true;
            IdLbl.Text = bagisciId;
            TasinmazBagisci bagisci = new TasinmazBagisci();
            bagisci = bagisci.Select<TasinmazBagisci>(BagisciIdQS.ConvertToInt());
            if (bagisci != null)
            {
                AdiLbl.Text = bagisci.Adi + " " + bagisci.Soyadi;
                BagisciFormunuDoldur(bagisci);
                PDFGoster(bagisci);
            }
        }
        private void OpenBagisciGirisi()
        {
            TitleLbl.CssClass = "col-form-label text-success font-weight-bold mb-1";
            TitleLbl.Text = "Taşınmaz Bağışçısı Girişi";
            SaveBtn.Visible = true;
            UpdateBtn.Visible = false;
            YakinlariBtn.Visible = false;
            TalepleriBtn.Visible = false;
            TaahhutleriBtn.Visible = false;
            BagislariBtn.Visible = false;
            VefatTarihiTxt.Visible = false;
            DosyaLnk.Visible = false;
            BelgeSilBtn.Visible = false;            
            BelgeYukleFU.Visible = true;
            TaahhutDosyaLnk.Visible = false;
            TaahhutSilBtn.Visible = false;
            TaahhutYukleFU.Visible = true;
        }
        private bool BagisciFormunuDoldur(TasinmazBagisci bagisci)
        {
            bool formDolduMu = false;
            try
            {
                IdLbl.Text = "" + bagisci.Id;
                AdiTxt.Text = bagisci.Adi;
                AdresTxt.Text = bagisci.Adres;
                DogumYeriTxt.Text = bagisci.DogumYeri;
                DogumTarihiTxt.Value = bagisci.DogumTarihi.ConvertToDatetimeEmptyIfNull();
                MeslegiTxt.Text = bagisci.Meslegi;
                SosyalGuvenceDDL.SelectedValue = bagisci.SosyalGuvence;
                SoyadiTxt.Text = bagisci.Soyadi;
                TCKimlikNoTxt.Text = bagisci.TCKimlikNo.ToString();
                Telefon1Txt.Text = bagisci.Telefon1;
                Telefon2Txt.Text = bagisci.Telefon2;
                EPostaTxt.Text = bagisci.EPosta;
                AciklamaTxt.Text = bagisci.Aciklama;
                GizliChk.Checked = bagisci.Gizli;


                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                string imgUrl = newUrl + "/../" + ProjeConstants.RESIMLER_BAGISCI + "/_t/" + bagisci.Adi.ReplaceTrChars() + bagisci.Soyadi.ReplaceTrChars() + "_jpg.jpg";
                DisplayImage.ImageUrl = imgUrl;

                string sv = "Bilinmiyor";
                
                ListItem ilItem = IliDDL.Items.FindByValue(IliDDL.Items.FindByText(bagisci.Ili).Value);
                if (ilItem != null)
                    IliDDL.SelectedValue = ilItem.Value;
                IlceDDLDoldur();
                BolgeTxtDoldur();
                if (IlcesiDDL.Items.FindByText(bagisci.Ilcesi) != null)
                    IlcesiDDL.SelectedValue = IlcesiDDL.Items.FindByText(bagisci.Ilcesi).Value;
                
                if (!string.IsNullOrEmpty(bagisci.Sag_vefat))
                    sv = bagisci.Sag_vefat;
                Sag_vefatDDL.SelectedValue = sv;
                //Vefat
                VefatTarihiTxt.Value = Sag_vefatDDL.SelectedItem.Value.Equals(ProjeConstants.BAGISCI_SAG) ? "" : bagisci.VefatTarihi.ConvertToDatetimeEmptyIfNull();
                DefinYeriTxt.Text = bagisci.DefinYeri;
                DefinAciklamaTxt.Text = bagisci.DefinAciklama;
                //ListItem definIlItem = DefinIliDDL.Items.FindByValue(DefinIliDDL.Items.FindByText(string.IsNullOrEmpty( bagisci.DefinIli)  ?"": bagisci.DefinIli).Value);
                UtilityHelper.SetDDLValue(DefinIliDDL, bagisci.DefinIli);
                //if (definIlItem != null)
                //    DefinIliDDL.SelectedValue = definIlItem.Value;
                DefinIlceDDLDoldur();
                UtilityHelper.SetDDLValue(DefinIlcesiDDL, bagisci.DefinIlcesi);
                //if (DefinIlcesiDDL.Items.FindByText(bagisci.DefinIlcesi) != null)
                //    DefinIlcesiDDL.SelectedValue = DefinIlcesiDDL.Items.FindByText(bagisci.DefinIlcesi).Value;
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();
                formDolduMu = false;
            }
            return formDolduMu;
        }
        private void IlDDDLDoldur()
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
                BolgeTxtDoldur();
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
        private void DDLleriDoldur()
        {
            IlDDDLDoldur();
            DefinIlDDDLDoldur();
            SosyalGuvenceDDLDoldur();
            Sag_vefatDDLDoldur();
        }
        private void DefinIlDDDLDoldur()
        {
            if (DefinIliDDL.SelectedItem == null)
            {
                DefinIliDDL.Items.Clear();
                Il pIl = new Il();
                List<Il> list = pIl.SelectAll<Il>();
                foreach (Il il in list)
                {
                    DefinIliDDL.Items.Add(new ListItem(il.IlAdi, il.Id.ToString()));
                }
                DefinIlceDDLDoldur();
            }

        }
        private void DefinIlceDDLDoldur()
        {
            DefinIlcesiDDL.Items.Clear();
            Ilce pIlce = new Ilce();
            List<Ilce> list = pIlce.SelectByIlId(DefinIliDDL.SelectedValue.ConvertToInt());
            foreach (Ilce ilce in list)
            {
                DefinIlcesiDDL.Items.Add(new ListItem(ilce.IlceAdi, ilce.Id.ToString()));
            }
        }
        private void SosyalGuvenceDDLDoldur()
        {
            SosyalGuvenceDDL.Items.Clear();
            SosyalGuvenceDDL.Items.Add("Yok");
            SosyalGuvenceDDL.Items.Add("SGK");
            SosyalGuvenceDDL.Items.Add("Emekli Sandığı");
            SosyalGuvenceDDL.Items.Add("SSK");
            SosyalGuvenceDDL.Items.Add("Bağkur");
        }
        private void Sag_vefatDDLDoldur()
        {
            Sag_vefatDDL.Items.Clear();
            Sag_vefatDDL.Items.Add(ProjeConstants.BAGISCI_SAG);
            Sag_vefatDDL.Items.Add(ProjeConstants.BAGISCI_VEFAT);
            Sag_vefatDDL.Items.Add(ProjeConstants.BAGISCI_MULGA);
            Sag_vefatDDL.Items.Add(ProjeConstants.BAGISCI_KURULUS);
            Sag_vefatDDL.Items.Add(ProjeConstants.BAGISCI_TASFIYE);
            Sag_vefatDDL.Items.Add(ProjeConstants.BAGISCI_LAGV);
            Sag_vefatDDL.Items.Add(ProjeConstants.BAGISCI_BILINMIYOR);
        }
        private void BolgeTxtDoldur()
        {
            Il il = new Il();
            il = il.SelectByIlAdi(IliDDL.SelectedItem.ToString());
            SorumluBolgeTxt.Text = il != null ? il.Bolge : "";
        }
        private TasinmazBagisci SaveBagisciData2Db()
        {
            TasinmazBagisci bagisci = new TasinmazBagisci();
            bagisci.Adi = AdiTxt.Text;
            bagisci.Adres = AdresTxt.Text;
            bagisci.Aciklama = AciklamaTxt.Text;
            bagisci.Gizli = GizliChk.Checked;
            bagisci.DogumTarihi = DogumTarihiTxt.Value.ConvertToDatetime();
            bagisci.DogumYeri = DogumYeriTxt.Text;
            bagisci.Meslegi = MeslegiTxt.Text;
            bagisci.SosyalGuvence = SosyalGuvenceDDL.SelectedValue;
            bagisci.Soyadi = SoyadiTxt.Text;
            bagisci.TCKimlikNo = TCKimlikNoTxt.Text.ConvertToLong();
            bagisci.Telefon1 = Telefon1Txt.Text;
            bagisci.Telefon2 = Telefon2Txt.Text;
            bagisci.EPosta = EPostaTxt.Text;
            bagisci.Ilcesi = IlcesiDDL.SelectedItem.ToString();
            ListItem ilItem = IliDDL.SelectedItem;
            Il il = new Il();
            il.Id = Convert.ToInt16(ilItem.Value);
            il.IlAdi = ilItem.Text;
            bagisci.Ili = il.IlAdi;
            bagisci.Foto = UtilityHelper.GenerateJpgId(bagisci.Adi + bagisci.Soyadi);

            bagisci.Sag_vefat = Sag_vefatDDL.SelectedValue;
            bagisci.VefatTarihi = VefatTarihiTxt.Value.ConvertToDatetime();
            bagisci.DefinYeri=DefinYeriTxt.Text;
            bagisci.DefinAciklama=DefinAciklamaTxt.Text;
            bagisci.DefinIlcesi = DefinIlcesiDDL.SelectedItem.ToString();
            ListItem definIlItem = DefinIliDDL.SelectedItem;
            Il definil = new Il();
            definil.Id = Convert.ToInt16(definIlItem.Value);
            definil.IlAdi = definIlItem.Text;
            bagisci.DefinIli = definil.IlAdi;

            int tasinmazBagisciId = bagisci.Save();
            bagisci.Id = tasinmazBagisciId;
            if (tasinmazBagisciId > 0)
                return bagisci;
            else
                return null;
        }
        private TasinmazBagisci UpdateBagisciData2Db()
        {
            TasinmazBagisci bagisci = new TasinmazBagisci();
            bagisci = bagisci.Select<TasinmazBagisci>(BagisciIdQS.ConvertToInt());
            bagisci.Adi = AdiTxt.Text;
            bagisci.Adres = AdresTxt.Text;
            bagisci.Aciklama = AciklamaTxt.Text;
            bagisci.Gizli = GizliChk.Checked;
            bagisci.DogumTarihi = DogumTarihiTxt.Value.ConvertToDatetime();
            bagisci.DogumYeri = DogumYeriTxt.Text;
            bagisci.Meslegi = MeslegiTxt.Text;
            bagisci.SosyalGuvence = SosyalGuvenceDDL.SelectedValue;
            bagisci.Soyadi = SoyadiTxt.Text;
            bagisci.TCKimlikNo = TCKimlikNoTxt.Text.ConvertToLong();
            bagisci.Telefon1 = Telefon1Txt.Text;
            bagisci.Telefon2 = Telefon2Txt.Text;
            bagisci.EPosta = EPostaTxt.Text;
            bagisci.Ilcesi = IlcesiDDL.SelectedItem.ToString();
            ListItem ilItem = IliDDL.SelectedItem;
            Il il = new Il();
            il.Id = Convert.ToInt16(ilItem.Value);
            il.IlAdi = ilItem.Text;
            bagisci.Ili = il.IlAdi;
            bagisci.Foto = UtilityHelper.GenerateJpgId(bagisci.Adi + bagisci.Soyadi);

            bagisci.Sag_vefat = Sag_vefatDDL.SelectedValue;
            bagisci.VefatTarihi = VefatTarihiTxt.Value.ConvertToDatetime(); ;
            bagisci.DefinYeri = DefinYeriTxt.Text;
            bagisci.DefinAciklama = DefinAciklamaTxt.Text;
            bagisci.DefinIlcesi = DefinIlcesiDDL.SelectedItem.ToString();
            ListItem definIlItem = DefinIliDDL.SelectedItem;
            Il definil = new Il();
            definil.Id = Convert.ToInt16(definIlItem.Value);
            definil.IlAdi = definIlItem.Text;
            bagisci.DefinIli = definil.IlAdi;
            bagisci.Update();
            return bagisci;
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
        private void SaveImageFiles2SP(string fotoFile)
        {
            string SPImageListName = ProjeConstants.RESIMLER_BAGISCI;

            ExceptionHelper exhelper = new ExceptionHelper();
            exhelper = UtilityHelper.uploadFile2SP(ResimYukleFU, "", fotoFile + "", SPImageListName, exhelper, ProjeConstants.RESIM_VESIKALIK_EN, ProjeConstants.RESIM_VESIKALIK_BOY);
            // then use the following to add the file to the list
            //listExists.RootFolder.Files.Add(fotoFile, UtilityHelper.StreamFile(xFileUpload.PostedFile.FileName));//MyUploadtoSharepoint(ProjeConstants.RESIMLER_MALZEME, xFileUpload.PostedFile.FileName, "2.jpg");
            if (exhelper.HasException())
            {
                Exception ex = new Exception("Resim Kaydedilemedi.");
                exhelper.Exceptions.Add(ex);
                exhelper.PublishException();
            }
            else
            {
                MessageHelper.PublishMessage("İşlem Tamamlandı.Resim yüklendi.", ProjeConstants.MESAJ_BASARILI);
            }
        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            IlceDDLDoldur();
            BolgeTxtDoldur();
        }
        protected void DefinIliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            DefinIlceDDLDoldur();
        }
        protected void DeleteBtn_Click(object sender, EventArgs e)
        {
            TasinmazBagisci bagisci = new TasinmazBagisci();
            bagisci = bagisci.Select<TasinmazBagisci>(BagisciIdQS.ConvertToInt());
            if (bagisci != null) //sildikten sonra önceki sayfaya dön
            {
                bagisci.Delete();
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                newUrl += "/" + ProjeConstants.PAGE_TASINMAZBAGISCI_LIST;
                Page.Response.Redirect(newUrl, true);
            }
            else
                MessageHelper.PublishMessage("Hata Bağışçı Silinemedi", ProjeConstants.MESAJ_HATA);
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
        protected void TasinmazBagisciListBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_LIST+"?SecilenId="+BagisciIdQS);
        }
        protected void SaveBtn_Click(object sender, EventArgs e)
        {

            try
            {
                TasinmazBagisci bagisci = SaveBagisciData2Db();
                if (bagisci != null) //kaydettikten sonra önceki sayfaya dön
                {

                    // eger bir resim seçildi ise o resmi Sharepointteki MalzemeResimleri listesine ekle
                    if (ResimYukleFU.HasFile)
                    {
                        SaveImageFiles2SP(bagisci.Foto);
                    }
                    PDFKaydet(bagisci.Id);
                    MessageHelper.PublishMessage("Bağışçı Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                    RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_LIST + "?Mesaj=true" + "&SecilenId=" + bagisci.Id);

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
                TasinmazBagisci bagisci = UpdateBagisciData2Db();
                if (bagisci != null)
                {
                    // eger bir resim seçildi ise o resmi Sharepointteki MalzemeResimleri listesine ekle
                    if (ResimYukleFU.HasFile)
                    {
                        SaveImageFiles2SP(bagisci.Foto);
                    }
                    PDFKaydet(bagisci.Id);
                    PDFGoster(bagisci);
                    MessageHelper.PublishMessage("Bağışçı Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                }
                else
                {
                    MessageHelper.PublishMessage("Bağışçı Güncellenemedi", ProjeConstants.MESAJ_HATA);
                }


            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void Sag_vefatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Sag_vefatDDL.SelectedItem.Text.Equals(ProjeConstants.BAGISCI_SAG))
            {
                VefatDiv.Visible = false;
                DefinAciklamaDiv.Visible = false;
            }
            else
            {
                VefatDiv.Visible = true;
                DefinAciklamaDiv.Visible = true;
            }
        }

        private void PDFKaydet(int bagisciId)
        {
            try
            {
                if (BelgeYukleFU.HasFile)
                {
                    string hedefDosyaAdi = ProjeConstants.DOSYA_BAGISBILGIVETALEP_FORMU + bagisciId + ".pdf";
                    bool isOk= UtilityHelper.UploadFileToSharePoint(BelgeYukleFU, ProjeConstants.TBYSBELGELERI_LIB, hedefDosyaAdi);
                    if (isOk)
                    {
                        DosyaLnk.Visible = true;
                        MessageHelper.PublishMessage("Bilgi Formu Yüklendi", ProjeConstants.MESAJ_BASARILI, 2000); 
                    }
                }
                else
                {
                    DosyaLnk.Visible = false;
                    BelgeSilBtn.Visible = false;
                    MessageHelper.PublishMessage("Lütfen Bağışçı Bilgi ve Talep Belgesi yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
                }
                if (TaahhutYukleFU.HasFile)
                {
                    string hedefDosyaAdi = ProjeConstants.DOSYA_TAAHHUT_FORMU + bagisciId + ".pdf";
                    bool isOk= UtilityHelper.UploadFileToSharePoint(TaahhutYukleFU, ProjeConstants.TBYSBELGELERI_LIB, hedefDosyaAdi);
                    if (isOk)
                    {
                        TaahhutSilBtn.Visible = true;
                        MessageHelper.PublishMessage("Taahhüt Belgesi yüklendi", ProjeConstants.MESAJ_BASARILI, 2000); 
                    }
                }
                else
                {
                    TaahhutDosyaLnk.Visible = false;
                    MessageHelper.PublishMessage("Lütfen Taahhüt Belgesi yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
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
        private void PDFGoster(TasinmazBagisci bagisci)
        {
            try
            {
                if (bagisci!=null)
                {
                    string dosyaAdi = ProjeConstants.DOSYA_BAGISBILGIVETALEP_FORMU + bagisci.Id + ".pdf";
                    string dosyaUrl = UtilityHelper.TbysBelgelerURLGetir() + "/" + dosyaAdi;
                    bool dosyaVarMi = UtilityHelper.DosyaVarMi(UtilityHelper.TbysBelgelerURLGetir(), ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi);
                    if (dosyaVarMi)
                    {
                        string belgePdfLink = @"'<a class=\'btn btn-secondary\' data-fancybox data-type=pdf data-width=960 data-height=720 href=" + dosyaUrl + @"> Belge Görüntüle </a>'";

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
                        MessageHelper.PublishMessage("Lütfen Bağışçı Bilgi ve Talep Formunu pdf olarak yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
                    }
                    string taahhutDosyaAdi = ProjeConstants.DOSYA_TAAHHUT_FORMU + bagisci.Id + ".pdf";
                    string taahhutDosyaUrl = UtilityHelper.TbysBelgelerURLGetir() + "/" + taahhutDosyaAdi;
                    bool taahhutDosyasiVarMi = UtilityHelper.DosyaVarMi(UtilityHelper.TbysBelgelerURLGetir(), ProjeConstants.TBYSBELGELERI_LIB, taahhutDosyaAdi);
                    if (taahhutDosyasiVarMi)
                    {
                        string belgePdfLink = @"'<a class=\'btn btn-secondary\' data-fancybox data-type=pdf data-width=960 data-height=720 href=" + taahhutDosyaUrl + @"> Belge Görüntüle </a>'";

                        TaahhutDosyaLnk.Target = "_blank";
                        TaahhutDosyaLnk.HRef = taahhutDosyaUrl;
                        TaahhutDosyaLnk.Visible = true;
                        TaahhutSilBtn.Visible = true;
                        TaahhutYukleFU.Visible = false;
                    }
                    else
                    {
                        TaahhutDosyaLnk.Visible = false;
                        TaahhutSilBtn.Visible = false;
                        TaahhutYukleFU.Visible = true;
                        MessageHelper.PublishMessage("Lütfen Bağışçı Taahhüt Formunu pdf olarak yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
                    }
                }
                else
                {
                    MessageHelper.PublishMessage("Bağışçı bulunamadı.", ProjeConstants.MESAJ_HATA);
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

        protected void BelgeSilBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string dosyaAdi = ProjeConstants.DOSYA_BAGISBILGIVETALEP_FORMU + BagisciIdQS + ".pdf";
                if (UtilityHelper.DeleteFileFromSharePointLib(ProjeConstants.PATH_TBYS_URL, ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi))
                {
                    MessageHelper.PublishMessage(dosyaAdi + " Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                    BelgeSilBtn.Visible = false;
                    DosyaLnk.Visible = false;
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

        protected void TaahhutSilBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string dosyaAdi = ProjeConstants.DOSYA_TAAHHUT_FORMU + BagisciIdQS + ".pdf";

                if (UtilityHelper.DeleteFileFromSharePointLib(ProjeConstants.PATH_TBYS_URL, ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi))
                {
                    MessageHelper.PublishMessage(dosyaAdi + " Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                    TaahhutSilBtn.Visible = false;
                    TaahhutDosyaLnk.Visible = false;
                    TaahhutYukleFU.Visible = true;
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
        protected void BagislariBtn_Click(object sender, EventArgs e)
        {
            TasinmazBagisci bagisci = new TasinmazBagisci();
            bagisci = bagisci.Select<TasinmazBagisci>(BagisciIdQS.ConvertToInt());
            if (bagisci != null) 
            {
                RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_BAGISLARI + "?DestinationApp=TBD&BagisciId=" + bagisci.Id);
            }
        }
        protected void YakinlariBtn_Click(object sender, EventArgs e)
        {
            TasinmazBagisci bagisci = new TasinmazBagisci();
            bagisci = bagisci.Select<TasinmazBagisci>(BagisciIdQS.ConvertToInt());
            if (bagisci != null) 
            {                
                RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_YAKINLARI + "?DestinationApp=TBD&BagisciId=" + bagisci.Id);
            }
        }
        protected void TalepleriBtn_Click(object sender, EventArgs e)
        {

            TasinmazBagisci bagisci = new TasinmazBagisci();
            bagisci = bagisci.Select<TasinmazBagisci>(BagisciIdQS.ConvertToInt());
            if (bagisci != null) 
            {
                RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_TALEPLERI + "?DestinationApp=TBD&BagisciId=" + bagisci.Id);
            }

        }
        
        protected void TaahhutleriBtn_Click(object sender, EventArgs e)
        {
            TasinmazBagisci bagisci = new TasinmazBagisci();
            bagisci = bagisci.Select<TasinmazBagisci>(BagisciIdQS.ConvertToInt());
            if (bagisci != null)
            {
                RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_TAAHHUTLERI  + "?DestinationApp=TBD&BagisciId=" + bagisci.Id);
            }
        }
    }
}
