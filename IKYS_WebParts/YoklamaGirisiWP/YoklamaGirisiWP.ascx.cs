using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.YoklamaGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class YoklamaGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public YoklamaGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string YoklamaIdQS
        {
            get
            {

                if (ViewState["YoklamaId"] == null)
                {
                    if (Page.Request.QueryString["YoklamaId"] != null)
                    {
                        ViewState["YoklamaId"] = Page.Request.QueryString["YoklamaId"];
                    }
                    else
                    {
                        ViewState["YoklamaId"] = string.Empty;
                    }
                }
                return ViewState["YoklamaId"].ToString();
            }

            set
            {
                ViewState["YoklamaId"] = value;
            }
        }
        private string PersonelIdQS
        {
            get
            {

                if (ViewState["PersonelId"] == null)
                {
                    if (Page.Request.QueryString["PersonelId"] != null)
                    {
                        ViewState["PersonelId"] = Page.Request.QueryString["PersonelId"];
                    }
                    else
                    {
                        ViewState["PersonelId"] = string.Empty;
                    }
                }
                return ViewState["PersonelId"].ToString();
            }

            set
            {
                ViewState["PersonelId"] = value;
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
        private string AuthQS
        {
            get
            {

                if (ViewState["Auth"] == null || string.IsNullOrEmpty(ViewState["Auth"].ToString()))
                {
                    if (Page.Request.QueryString["Auth"] != null)
                    {
                        ViewState["Auth"] = Page.Request.QueryString["Auth"];
                    }
                    else
                    {
                        ViewState["Auth"] = string.Empty;
                    }
                }
                return ViewState["Auth"].ToString();
            }

            set
            {
                ViewState["Auth"] = value;
            }
        }
        private string UzaktanCalismaQS
        {
            get
            {

                if (ViewState["UzaktanCalisma"] == null)
                {
                    if (Page.Request.QueryString["UzaktanCalisma"] != null)
                    {
                        ViewState["UzaktanCalisma"] = Page.Request.QueryString["UzaktanCalisma"];
                    }
                    else
                    {
                        ViewState["UzaktanCalisma"] = "false";
                    }
                }
                return ViewState["UzaktanCalisma"].ToString();
            }

            set
            {
                ViewState["UzakCalisma"] = value;
            }
        }
        private string BirimTanimIdQS
        {
            get
            {

                if (ViewState["BirimTanimId"] == null)
                {
                    if (Page.Request.QueryString["BirimTanimId"] != null)
                    {
                        ViewState["BirimTanimId"] = Page.Request.QueryString["BirimTanimId"];
                    }
                    else
                    {
                        ViewState["BirimTanimId"] = string.Empty;
                    }
                }
                return ViewState["BirimTanimId"].ToString();
            }

            set
            {
                ViewState["BirimTanimId"] = value;
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
            try
            {

                if (string.IsNullOrEmpty(DestinationAppQS) || (!DestinationAppQS.Equals("YokD")))
                {
                    OpenGiris();
                    if (AuthQS.Equals("IKYS"))
                    {
                        PersonelDiv.Attributes["style"] = "display:block";
                        AciklamaTxt.Enabled = true;
                        BulunmamaSebebiDDL.Enabled = true;
                        TitleLbl.Text = "Yoklama Girişi";
                    }
                    else if (AuthQS.Equals("BIRIM"))
                    {
                        PersonelDiv.Attributes["style"] = "display:block";
                        YoklamaListesiBtn.Visible = false;
                        SelectDDLByText(BulunmamaSebebiDDL, "Görevli");
                        AciklamaTxt.Enabled = true;
                        BulunmamaSebebiDDL.Enabled = false;
                        TitleLbl.Text = "Şehir İçi Görev Formu";
                    }
                    else
                    {
                        //personelDiv display:none yap
                        PersonelDiv.Attributes["style"] = "display:none";

                        SelectDDLByText(BulunmamaSebebiDDL, "Görevli");
                        AciklamaTxt.Enabled = true;
                        BulunmamaSebebiDDL.Enabled = false;
                        TitleLbl.Text = "Şehir İçi Görev Formu";
                    }
                    if (UzaktanCalismaQS.Equals("true"))
                    {

                        SelectDDLByText(BulunmamaSebebiDDL, "Görevli");
                        AciklamaTxt.Text = "Uzaktan Çalışma";
                        AciklamaTxt.Enabled = false;
                        BulunmamaSebebiDDL.Enabled = false;
                        TitleLbl.Text = "Uzaktan Çalışma Görev Formu";
                        IletisimBilgileri ib = new IletisimBilgileri();
                        ib = ib.SelectByPersonelId(PersonelIdQS.ConvertToInt());
                        if (ib != null)
                        {
                            Ilce ilce = new Ilce();
                            ilce = ilce.Select<Ilce>(ib.Ilcesi);
                            string ilcestr = ilce.IlceAdi;
                            AdresTxt.Text = ib.Adres + " " + ib.Semt + " " + ilcestr;
                        }

                    }
                }
                else if (DestinationAppQS.Equals("YokD"))
                {
                    OpenDuzenle();
                    if (AuthQS.Equals("IKYS"))
                    {
                        PersonelDiv.Attributes["style"] = "display:block";
                        AciklamaTxt.Enabled = true;
                        BulunmamaSebebiDDL.Enabled = true;
                        TitleLbl.Text = "Yoklama Girişi";
                    }
                    if (BulunmamaSebebiDDL.SelectedItem.Value.ConvertToInt() == ProjeConstants.BULUNMAMASEBEBI_GOREVLI_INT)
                    {
                        AciklamaTxt.Enabled = true;
                        if (AuthQS.Equals("IKYS"))
                            RaporAlBtn.Visible = true;//imzalı nüsha Gn.Md.tarafından kaldırıldı
                    }
                    if (UzaktanCalismaQS.Equals("true"))
                    {
                        SelectDDLByText(BulunmamaSebebiDDL, "Görevli");
                        AciklamaTxt.Text = "Uzaktan Çalışma";
                        AciklamaTxt.Enabled = false;
                        BulunmamaSebebiDDL.Enabled = false;
                        TitleLbl.Text = "Uzaktan Çalışma Görev Formu";
                        RaporAlBtn.Visible = true;
                    }
                }

            }
            catch (Exception exception)
            {
                ExceptionHelper exHelper = new ExceptionHelper(exception);
                exHelper.PublishException();
            }
        }
        private void OpenDuzenle()
        {
            SaveBtn.Visible = false;
            DeleteBtn.Visible = true;
            UpdateBtn.Visible = true;
            TitleLbl.CssClass = "col-form-primary  btn-outline-primary mb-1";
            TitleLbl.Text = "Yoklama Düzenleme";
            Personel personel = PersonelGetir();
            if (!Page.IsPostBack)
            {
                if (personel != null)
                {
                    PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                    FillBasSaat();
                    FillBitSaat();
                    FillBulunmamaSebebiDDL();
                    YoklamaFormunuDoldur();
                    FillPersonelDDL(personel);

                }
            }
        }
        private void OpenGiris()
        {
            SaveBtn.Visible = true;
            UpdateBtn.Visible = false;
            DeleteBtn.Visible = false;
            if (!Page.IsPostBack)
            {
                FillBasSaat();
                FillBitSaat();
                FillBulunmamaSebebiDDL();
                Personel personel = PersonelGetir();
                FillPersonelDDL(personel);

                if (personel != null)
                {
                    PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                    DateTime today = DateTime.Today;
                    TimeSpan mesaiBasSaati = ProjeConstants.MESAI_BASLAMA_SAATI;
                    TimeSpan mesaiBitSaati = ProjeConstants.MESAI_BITIS_SAATI;
                    DateTime bastar = today + mesaiBasSaati;
                    DateTime bittar = today + ProjeConstants.GUN_BITIS_SAATI;//mesaiBitSaati //+ new TimeSpan(3,0,0);
                    BasTarTxt.Value = bastar.ConvertToDatetimeEmptyIfNull();
                    BitTarTxt.Value = bittar.ConvertToDatetimeEmptyIfNull();
                    string bassaat = bastar.ToString("HH:mm");
                    string bitsaat = bittar.ToString("HH:mm");
                    SelectDDLByText(BasSaatDDL, bassaat);
                    SelectDDLByText(BitSaatDDL, bitsaat);


                }

            }
        }
        private void FillPersonelDDL(Personel personel)
        {
            PersonelDDL.Items.Clear();
            List<Personel> list = new List<Personel>();
           
            if (AuthQS.Equals("IKYS"))
            {
                Personel perdao = new Personel();
                list = perdao.SelectCalisanPersonel();
            }
            else if (AuthQS.Equals("BIRIM"))
            {

                list = BirimdekiPersoneliGetir(personel);
            }
            else
            {
                personel = PersonelGetir();
                if (personel != null)
                {
                    list.Add(personel);
                }
            }

            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                PersonelDDL.Items.Add(li);
            }
            string secilenPer = !string.IsNullOrEmpty(PersonelIdQS) ? PersonelIdQS : "0";
            ListItem perItem = new ListItem();
            if (!string.IsNullOrEmpty(secilenPer))
                perItem = PersonelDDL.Items.FindByValue(secilenPer);

            if (perItem != null)
            {
                PersonelDDL.SelectedValue = perItem.Value;
                PersonelIdQS = perItem.Value;
            }
        }
        private List<Personel> BirimdekiPersoneliGetir(Personel personel)
        {

            List<Personel> list = new List<Personel>();
            if (personel != null)
            {
                string birimListesiStr = string.Empty;
                if (string.IsNullOrEmpty(BirimTanimIdQS))
                {
                    IsBilgileri ib = new IsBilgileri();
                    ib = ib.SelectByPersonelId(personel.Id);
                    if (ib != null)
                    {
                        int birimId = ib.BirimId;
                        BirimTanimIdQS = ib.BirimId.ToString();
                        //Personel perdao = new Personel();
                        //list = perdao.SelectCalisanPersonelByBirimId(birimId);
                    }
                }

                BirimTanim bt = new BirimTanim();
                bt = bt.Select(BirimTanimIdQS.ConvertToInt());
                if (bt != null)
                {
                    birimListesiStr = BirimListesiGetir(bt);
                }
                Personel perdao = new Personel();
                list = perdao.SelectCalisanPersonelByBirimReturnList(birimListesiStr);
            }
            return list;
        }

        private string BirimListesiGetir(BirimTanim birimTanim)
        {
            string birimIdStr = string.Empty;
            if (birimTanim != null)
            {
                string birim = ChildBirimGetir(birimTanim.Id);
                birimIdStr = string.IsNullOrEmpty(birim) ? "" : birim.Substring(0, birim.Length - 1);
            }

            return birimIdStr;
        }
        private string ChildBirimGetir(int parentId)
        {
            string retVal = parentId + ",";
            BirimTanim bt = new BirimTanim();
            List<BirimTanim> list = bt.SelectByParentId(parentId);
            foreach (BirimTanim item in list)
            {
                //retVal += item.Id + ",";
                string val = ChildBirimGetir(item.Id);
                if (string.IsNullOrEmpty(val))
                {
                    return retVal;
                }
                retVal += val;
            }
            return retVal;
        }
        private void FillBulunmamaSebebiDDL()
        {
            BulunmamaSebebiDDL.Items.Clear();
            BulunmamaSebebi bs = new BulunmamaSebebi();
            List<BulunmamaSebebi> list = bs.SelectAll<BulunmamaSebebi>();

            foreach (BulunmamaSebebi item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                BulunmamaSebebiDDL.Items.Add(li);
            }
        }
        private void FillBasSaat()
        {
            BasSaatDDL.Items.Clear();
            TimeSpan bastarTS = new TimeSpan(6, 0, 0);
            TimeSpan aralikTS = new TimeSpan(0, 5, 0);
            TimeSpan bittarTS = new TimeSpan(23, 55, 0);

            TimeSpan oglenArasiBastarTS = new TimeSpan(12, 0, 0);
            TimeSpan oglenArasiBittarTS = new TimeSpan(13, 0, 0);

            TimeSpan nextTS = bastarTS;

            while (nextTS < bittarTS)
            {

                if ((nextTS >= oglenArasiBastarTS) && (nextTS < oglenArasiBittarTS))
                {
                    nextTS = nextTS + aralikTS;
                    continue;
                }
                else
                {
                    string bastarStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                    ListItem li = new ListItem(bastarStr);
                    BasSaatDDL.Items.Add(li);
                    nextTS = nextTS + aralikTS;
                }
            }
        }
        private void FillBitSaat()
        {
            string bitsaatStr = BitSaatDDL.SelectedItem == null ? "23:59" : BitSaatDDL.SelectedItem.Text;
            BitSaatDDL.Items.Clear();
            string bassaatStr = BasSaatDDL.SelectedItem == null ? "06:05" : BasSaatDDL.SelectedItem.Text;
            TimeSpan bastarTS = bassaatStr.ConvertToTimeSpan();
            TimeSpan aralikTS = new TimeSpan(0, 5, 0);
            TimeSpan bittarTS = new TimeSpan(23, 55, 0);

            TimeSpan oglenArasiBastarTS = new TimeSpan(12, 0, 0);
            TimeSpan oglenArasiBittarTS = new TimeSpan(13, 0, 0);

            TimeSpan nextTS = bastarTS;

            do
            {
                nextTS = nextTS + aralikTS;
                if ((nextTS > oglenArasiBastarTS) && (nextTS <= oglenArasiBittarTS))
                {
                    continue;
                }
                else
                {
                    string saatStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                    ListItem li = new ListItem(saatStr);
                    BitSaatDDL.Items.Add(li);
                }

            } while (nextTS < bittarTS);
            if (BitSaatDDL.Items.FindByText(bitsaatStr) != null) { }
            SelectDDLValue(BitSaatDDL, "17:00");
        }
        /// <summary>
        /// QueryString'den gelen YoklamaIdQS ile Yoklama_Table'dan kayıt getir
        /// Bu bilgileri forma doldur
        /// Güncelle butonunu aç
        /// Kaydet butonunu sakla
        /// </summary>
        private void YoklamaFormunuDoldur()
        {
            Yoklama yoklama = new Yoklama();
            yoklama = yoklama.Select<Yoklama>(YoklamaIdQS.ConvertToInt());
            if (yoklama != null)
            {
                YoklamaIdLbl.Text = yoklama.Id.ReturnEmptyIfNull().ToString();
                SelectDDLValue(BulunmamaSebebiDDL, yoklama.BulunmamaSebebi.ToString());
                SelectDDLValue(PersonelDDL, yoklama.PersonelId.ToString());
                string bassaat = yoklama.BaslangicTarihi.ToString("HH:mm");
                SelectDDLByText(BasSaatDDL, bassaat);
                string bitsaat = yoklama.BitisTarihi.ToString("HH:mm");
                SelectDDLByText(BitSaatDDL, bitsaat);
                AciklamaTxt.Text = yoklama.Aciklama;
                AdresTxt.Text = yoklama.Adres;
                BasTarTxt.Value = yoklama.BaslangicTarihi.ConvertToDatetimeEmptyIfNull();
                BitTarTxt.Value = yoklama.BitisTarihi.ConvertToDatetimeEmptyIfNull();
            }
            else
            {
                MessageHelper.PublishMessage("Kayıt bulunamadı", ProjeConstants.MESAJ_BILGI);
            }
        }
        private string SelectDDLValue(DropDownList ddlList, string value)
        {
            if (ddlList.Items.FindByValue(value) != null)
            {
                ListItem li = ddlList.Items.FindByValue(value);
                ddlList.SelectedValue = li.Value;
                return li.Value;
            }
            return string.Empty;
        }
        private string SelectDDLByText(DropDownList ddlList, string value)
        {
            if (ddlList.Items.FindByText(value) != null)
            {
                ListItem li = ddlList.Items.FindByText(value);
                ddlList.SelectedValue = li.Value;
                return li.Value;
            }
            return string.Empty;
        }
        private Personel PersonelGetir()
        {
            Personel personel = new Personel();

            if (!string.IsNullOrEmpty(PersonelIdQS))
            {
                personel = personel.Select<Personel>(PersonelIdQS.ConvertToInt());

            }
            else
            {
                string userName = CurrentUserName.Substring(CurrentUserName.LastIndexOf("\\") + 1, CurrentUserName.Length - CurrentUserName.LastIndexOf("\\") - 1);
                personel = personel.SelectByUserName(userName);
                PersonelIdQS = personel.Id.ToString();
            }

            return personel;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void YoklamaListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_YOKLAMA_LIST + (string.IsNullOrEmpty(AuthQS) ? "" : "?Auth=" + AuthQS + "&SecilenId=" + YoklamaIdQS));
        }
        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Personel personel = new Personel();
                personel = PersonelGetir();
                if (personel != null)
                {
                    bool isValid = validateInputValues();
                    if (isValid)
                    {
                        Yoklama yoklama = Kaydet();
                        YoklamaIdQS = yoklama.Id.ToString();
                        if (yoklama!=null)
                        {
                            if (AuthQS.Equals("IKYS"))
                            {

                            }
                            else
                            {
                                IKYSOrtak.GorevOnayEPostasiGonder(personel, yoklama.Id, "SehirIci");
                            }
                            
                            SaveBtn.Visible = false;
                            if ((BulunmamaSebebiDDL.SelectedItem.Value.ConvertToInt() == ProjeConstants.BULUNMAMASEBEBI_GOREVLI_INT)
                                && AciklamaTxt.Text.Contains(ProjeConstants.BULUNMAMASEBEBI_GOREVLI_UZAKTANCALISMA))
                            {
                                MessageHelper.PublishMessage("Görev Kaydedildi. Formun çıktısını 'Rapor Al' düğmesine tıklayarak alabilirsiniz", ProjeConstants.MESAJ_BASARILI, 4000);
                                RaporAlBtn.Visible = true;
                            }
                            else
                            {
                                MessageHelper.PublishMessage("Kaydedildi. ", ProjeConstants.MESAJ_BASARILI, 4000);
                            }

                        }
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Lütfen bilgileri tamamlayınız.", ProjeConstants.MESAJ_HATA);
                    }
                }
                else
                {
                    MessageHelper.PublishMessage("Personel Bulunamadı!", ProjeConstants.MESAJ_HATA);
                }

            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Kaydedilemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }
        private Yoklama Kaydet()
        {
            Yoklama yoklama = new Yoklama();
            if (CakismaVarMi(0))
            {
                MessageHelper.PublishMessage("Kaydedilemedi. Girilen tarihle çakışan bir görev bulunmaktadır. ", ProjeConstants.MESAJ_HATA);
            }
            else
            {

                Personel personel = PersonelGetir();
                if (personel != null)
                {

                    DateTime bastar = BasTarTxt.Value.ConvertToDatetime();
                    DateTime bittar = BitTarTxt.Value.ConvertToDatetime();
                    string basSaat = BasSaatDDL.SelectedItem.Text;
                    bastar = UtilityHelper.TariheSaatEkle(bastar, basSaat);
                    string bitSaat = BitSaatDDL.SelectedItem.Text;
                    bittar = UtilityHelper.TariheSaatEkle(bittar, bitSaat);

                    yoklama.Aciklama = AciklamaTxt.Text;
                    yoklama.BaslangicTarihi = bastar;
                    yoklama.BitisTarihi = bittar;
                    yoklama.BulunmamaSebebi = BulunmamaSebebiDDL.SelectedItem.Value.ConvertToInt();
                    yoklama.Adres = AdresTxt.Text;
                    yoklama.Olusturan = CurrentUserName;
                    yoklama.PersonelId = personel.Id;
                    yoklama.Id = yoklama.Save();
                }
            }

            return yoklama;
        }

        private bool CakismaVarMi(int current)
        {
            DateTime bastar = BasTarTxt.Value.ConvertToDatetime();
            DateTime bittar = BitTarTxt.Value.ConvertToDatetime();
            string basSaat = BasSaatDDL.SelectedItem.Text;
            bastar = UtilityHelper.TariheSaatEkle(bastar, basSaat);
            string bitSaat = BitSaatDDL.SelectedItem.Text;
            bittar = UtilityHelper.TariheSaatEkle(bittar, bitSaat);

            bool cakismaVarMi = false;
            Yoklama yoklama = new Yoklama();
            List<Yoklama> list = yoklama.SelectByPersonelIdTarih(PersonelIdQS.ConvertToInt(), bastar, bittar);
            foreach (Yoklama item in list)
            {
                if (item.Id == current)
                    continue;
                else
                {
                    cakismaVarMi = true;
                    break;
                }
            }
            return cakismaVarMi;
        }
        private bool validateInputValues()
        {
            bool isValidated = false;
            bool isNullOrEmpty = string.IsNullOrEmpty(BasTarTxt.Value.ConvertToDatetimeEmptyIfNull()) ||
                string.IsNullOrEmpty(BitTarTxt.Value.ConvertToDatetimeEmptyIfNull()) ||
                (BasSaatDDL.SelectedItem == null) ||
                (BitSaatDDL.SelectedItem == null) ||
                (BulunmamaSebebiDDL.SelectedItem == null) ||
                string.IsNullOrEmpty(BasSaatDDL.SelectedItem.Value) ||
                string.IsNullOrEmpty(BitSaatDDL.SelectedItem.Value) ||
                string.IsNullOrEmpty(BulunmamaSebebiDDL.SelectedItem.Value);
            isValidated = !isNullOrEmpty;
            return isValidated;
        }
        private bool YoklamaKaydiSil(Yoklama yoklama)
        {
            bool isSaved = false;
            try
            {
                if (yoklama != null)
                {
                    isSaved = yoklama.Delete();

                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

            return isSaved;
        }
        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Personel personel = new Personel();
                personel = PersonelGetir();
                if (personel != null)
                {
                    bool isValid = validateInputValues();
                    if (isValid)
                    {
                        bool isUpdated = Guncelle();
                        if (isUpdated)
                        {
                            MessageHelper.PublishMessage("Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                        }
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Lütfen bilgileri tamamlayınız.", ProjeConstants.MESAJ_HATA);
                    }
                }
                else
                {
                    MessageHelper.PublishMessage("Personel Bulunamadı!", ProjeConstants.MESAJ_HATA);
                }

            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Güncellenemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }

        }
        private bool Guncelle()
        {
            bool isUpdated = false;

            if (CakismaVarMi(YoklamaIdQS.ConvertToInt()))
            {
                MessageHelper.PublishMessage("Kaydedilemedi. Girilen tarihle çakışan bir görev bulunmaktadır. ", ProjeConstants.MESAJ_HATA);
            }
            else
            {
                Yoklama yoklama = new Yoklama();
                yoklama = yoklama.Select<Yoklama>(YoklamaIdQS.ConvertToInt());
                if (yoklama != null)
                {
                    DateTime bastar = BasTarTxt.Value.ConvertToDatetime();
                    DateTime bittar = BitTarTxt.Value.ConvertToDatetime();
                    string basSaat = BasSaatDDL.SelectedItem.Text;
                    bastar = UtilityHelper.TariheSaatEkle(bastar, basSaat);
                    string bitSaat = BitSaatDDL.SelectedItem.Text;
                    bittar = UtilityHelper.TariheSaatEkle(bittar, bitSaat);

                    yoklama.Aciklama = AciklamaTxt.Text;
                    yoklama.BaslangicTarihi = bastar;
                    yoklama.BitisTarihi = bittar;
                    yoklama.BulunmamaSebebi = BulunmamaSebebiDDL.SelectedItem.Value.ConvertToInt();
                    yoklama.Adres = AdresTxt.Text;
                    yoklama.Degistiren = CurrentUserName;
                    Personel personel = PersonelGetir();
                    yoklama.PersonelId = personel.Id;
                    isUpdated = yoklama.Update();
                }

            }

            return isUpdated;
        }
        protected void BasSaatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillBitSaat();
        }
        protected void BulunmamaSebebiDDL_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected void PersonelDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            PersonelIdQS = PersonelDDL.SelectedItem.Value;
            Personel personel = PersonelGetir();
            if (personel != null)
            {
                PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                IletisimBilgileri ib = new IletisimBilgileri();
                ib = ib.SelectByPersonelId(personel.Id);
                if (ib != null)
                {
                    Ilce ilce = new Ilce();
                    ilce = ilce.Select<Ilce>(ib.Ilcesi);
                    string ilcestr = ilce.IlceAdi;
                    AdresTxt.Text = ib.Adres + " " + ib.Semt + " " + ilcestr;
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
        protected void DeleteBtn_Click(object sender, EventArgs e)
        {
            SilLbl.Text = "Lütfen Dikkat: Yoklama Kaydı Silinecek";
            SilmeMesajiLbl.Text = "Yoklama Kaydını Silmek İstediğinizden Emin misiniz?";
            DeleteNowBtn.Visible = true;
            var openPopup = "OpenModal();";
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
        }
        protected void DeleteNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Yoklama yoklama = new Yoklama();
                yoklama = yoklama.Select<Yoklama>(YoklamaIdQS.ConvertToInt());
                if (yoklama != null)
                {
                    bool isDeleted = YoklamaKaydiSil(yoklama);
                    if (isDeleted)
                    {
                        MessageHelper.PublishMessage("Yoklama Kaydı Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                        RedirectToPage(ProjeConstants.PAGE_YOKLAMA_LIST + (string.IsNullOrEmpty(AuthQS)? "?":"?AUTH="+AuthQS+"&")+ "Mesaj=true");
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Yoklama Kaydı Silinemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }

        protected void RaporAlBtn_Click(object sender, EventArgs e)
        {

            string fullUrl = string.Format("{0}?YoklamaId={1}", ProjeConstants.RAPOR_SEHIRICIGOREVFORMU_URL, YoklamaIdQS);
            RedirectToPage(fullUrl);
        }
    }
}
