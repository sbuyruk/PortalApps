using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.EgitimGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class EgitimGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public EgitimGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            Personel personel = PersonelGetir();
            if (!Page.IsPostBack)
            {
                PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                FillEgitimSeviyesiDDL();
                YabanciDilDDLDoldur();

            }
            if (personel != null)
            {
                FillOkulTable(personel);
                FillKursTable(personel);
                FillIsyeriTable(personel);
                FillYabanciDilTable(personel);
            }
        }
        private void FillEgitimSeviyesiDDL()
        {
            OkulSeviyeDDL.Items.Clear();
            EgitimSeviyesi seviyeDao = new EgitimSeviyesi();
            List<EgitimSeviyesi> list = seviyeDao.SelectAll<EgitimSeviyesi>();
            foreach (EgitimSeviyesi item in list)
            {
                ListItem li = new ListItem(item.Adi, item.Id.ReturnZeroIfNull().ToString());
                OkulSeviyeDDL.Items.Add(li);
            }
        } 
        private void YabanciDilDDLDoldur()
        {
            YabanciDilDDL.Items.Clear();

            ListItem li1 = new ListItem(ProjeConstants.YABANCIDIL_INGILIZCE);
            ListItem li2 = new ListItem(ProjeConstants.YABANCIDIL_ALMANCA);
            ListItem li3 = new ListItem(ProjeConstants.YABANCIDIL_FRANSIZCA);
            ListItem li4 = new ListItem(ProjeConstants.YABANCIDIL_RUSCA);
            ListItem li5 = new ListItem(ProjeConstants.YABANCIDIL_CINCE);
            ListItem li6 = new ListItem(ProjeConstants.YABANCIDIL_ISPANYOLCA);
            ListItem li7 = new ListItem(ProjeConstants.YABANCIDIL_ITALYANCA);
            ListItem li8 = new ListItem(ProjeConstants.YABANCIDIL_YUNANCA);
            ListItem li9 = new ListItem(ProjeConstants.YABANCIDIL_ARAPCA);
            YabanciDilDDL.Items.Add(li1);
            YabanciDilDDL.Items.Add(li2);
            YabanciDilDDL.Items.Add(li3);
            YabanciDilDDL.Items.Add(li4);
            YabanciDilDDL.Items.Add(li5);
            YabanciDilDDL.Items.Add(li6);
            YabanciDilDDL.Items.Add(li7);
            YabanciDilDDL.Items.Add(li8);
            YabanciDilDDL.Items.Add(li9);

        }
        private void FillOkulTable(Personel personel)
        {
            Egitim egitim = new Egitim();
            List<Egitim> egitimList = egitim.SelectByPersonelId(personel.Id);
            OkulTableHeaders();
            int SiraNo = 1;
            foreach (Egitim item in egitimList)
            {
                TableRow row = new TableRow();

                TableCell SiraCell = new TableCell();
                SiraCell.Text = SiraNo++ + "";
                row.Controls.Add(SiraCell);

                TableCell OkulCell = new TableCell();
                OkulCell.Text = item.Okul;
                row.Controls.Add(OkulCell);

                TableCell SeviyeCell = new TableCell();
                int seviyeId = item.Seviye.ConvertToInt();
                if (seviyeId > 0)
                {
                    EgitimSeviyesi es = new EgitimSeviyesi();
                    es = es.Select<EgitimSeviyesi>(seviyeId);
                    SeviyeCell.Text = es != null ? es.Adi : "";
                }

                row.Controls.Add(SeviyeCell);

                TableCell MezuniyetTarCell = new TableCell();
                MezuniyetTarCell.Text = item.MezuniyetTar.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(MezuniyetTarCell);

                TableCell AciklamaCell = new TableCell();
                AciklamaCell.Text = item.Aciklama;
                row.Controls.Add(AciklamaCell);

                TableCell SilCell = new TableCell();
                LinkButton SilBtn = new LinkButton();
                SilBtn.Text = "Sil";
                SilBtn.ID = "okulSilBtn" + SiraNo;
                OkulTableUpdatePanel.ContentTemplateContainer.Controls.Add(SilBtn);
                SilBtn.CssClass = "btn btn-outline-danger";
                SilBtn.Click += delegate
                {

                    try
                    {
                        bool isDeleted = item.Delete();

                        if (isDeleted)
                        {
                            FillOkulTable(personel);
                            MessageHelper.PublishMessage("Kayıt Silindi.", ProjeConstants.MESAJ_BASARILI, 2000);
                        }
                        else
                            MessageHelper.PublishMessage("Kayıt Silinemedi.", ProjeConstants.MESAJ_HATA);
                    }
                    catch (Exception exception)
                    {
                        ExceptionHelper ex = new ExceptionHelper(exception);
                        ex.PublishException();
                    }
                };
                SilCell.Controls.Add(SilBtn);
                row.Controls.Add(SilCell);

                OkulTable.Controls.Add(row);
            }
        }
        private void FillKursTable(Personel personel)
        {
            Kurs kurs = new Kurs();
            List<Kurs> kursList = kurs.SelectByPersonelId(personel.Id);
            KursTableHeaders();
            int SiraNo = 1;
            foreach (Kurs item in kursList)
            {
                TableRow row = new TableRow();

                TableCell SiraCell = new TableCell();
                SiraCell.Text = SiraNo++ + "";
                row.Controls.Add(SiraCell);

                TableCell KursCell = new TableCell();
                KursCell.Text = item.KursAdi;
                row.Controls.Add(KursCell);

                TableCell SureCell = new TableCell();
                SureCell.Text = item.KursAdi;
                row.Controls.Add(SureCell);

                TableCell TarihCell = new TableCell();
                TarihCell.Text = item.Tarih.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(TarihCell);

                TableCell VerenKurumCell = new TableCell();
                VerenKurumCell.Text = item.VerenKurum;
                row.Controls.Add(VerenKurumCell);

                TableCell SilCell = new TableCell();
                LinkButton SilBtn = new LinkButton();
                SilBtn.Text = "Sil";
                SilBtn.ID = "kursSilBtn" + SiraNo;
                KursTableUpdatePanel.ContentTemplateContainer.Controls.Add(SilBtn);
                SilBtn.CssClass = "btn btn-outline-danger";
                SilBtn.Click += delegate
                {

                    try
                    {
                        bool isDeleted = item.Delete();

                        if (isDeleted)
                        {
                            FillKursTable(personel);
                            MessageHelper.PublishMessage("Kayıt Silindi.", ProjeConstants.MESAJ_BASARILI, 2000);
                        }
                        else
                            MessageHelper.PublishMessage("Kayıt Silinemedi.", ProjeConstants.MESAJ_HATA);
                    }
                    catch (Exception exception)
                    {
                        ExceptionHelper ex = new ExceptionHelper(exception);
                        ex.PublishException();
                    }
                };
                SilCell.Controls.Add(SilBtn);
                row.Controls.Add(SilCell);

                KursTable.Controls.Add(row);
            }
        }
        private void FillIsyeriTable(Personel personel)
        {
            IsTecrube kurs = new IsTecrube();
            List<IsTecrube> kursList = kurs.SelectByPersonelId(personel.Id);
            IsyeriTableHeaders();
            int SiraNo = 1;
            foreach (IsTecrube item in kursList)
            {
                TableRow row = new TableRow();

                TableCell SiraCell = new TableCell();
                SiraCell.Text = SiraNo++ + "";
                row.Controls.Add(SiraCell);

                TableCell IsyeriCell = new TableCell();
                IsyeriCell.Text = item.Isyeri;
                row.Controls.Add(IsyeriCell);

                TableCell GorevCell = new TableCell();
                GorevCell.Text = item.Gorevi;
                row.Controls.Add(GorevCell);

                TableCell BasTarCell = new TableCell();
                BasTarCell.Text = item.BasTar.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(BasTarCell);

                TableCell BitTarCell = new TableCell();
                BitTarCell.Text = item.BitTar.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(BitTarCell);

                TableCell SilCell = new TableCell();
                LinkButton SilBtn = new LinkButton();
                SilBtn.Text = "Sil";
                SilBtn.ID = "isSilBtn" + SiraNo;
                IsyeriTableUpdatePanel.ContentTemplateContainer.Controls.Add(SilBtn);
                SilBtn.CssClass = "btn btn-outline-danger";
                SilBtn.Click += delegate
                {

                    try
                    {
                        bool isDeleted = item.Delete();

                        if (isDeleted)
                        {
                            FillIsyeriTable(personel);
                            MessageHelper.PublishMessage("Kayıt Silindi.", ProjeConstants.MESAJ_BASARILI, 2000);
                        }
                        else
                            MessageHelper.PublishMessage("Kayıt Silinemedi.", ProjeConstants.MESAJ_HATA);
                    }
                    catch (Exception exception)
                    {
                        ExceptionHelper ex = new ExceptionHelper(exception);
                        ex.PublishException();
                    }
                };
                SilCell.Controls.Add(SilBtn);
                row.Controls.Add(SilCell);

                IsyeriTable.Controls.Add(row);
            }
        }
        private void FillYabanciDilTable(Personel personel)
        {
           

            string[] headers = { "Sira", "Yabanci Dil","Sinav Adi","Sinav Notu","Sinav Tarihi","Açiklama" };
            UtilityHelper.SetTableHeaders(YabanciDilTable,headers);

            YabanciDil dilDao = new YabanciDil();
            List<YabanciDil> yabanciDilList = dilDao.SelectByPersonelId(personel.Id);
            int SiraNo = 1;
            foreach (YabanciDil item in yabanciDilList)
            {
                TableRow row = new TableRow();

                TableCell SiraCell = new TableCell();
                SiraCell.Text = SiraNo++ + "";
                row.Controls.Add(SiraCell);

                TableCell YabanciDilCell = new TableCell();
                YabanciDilCell.Text = item.Dil;
                row.Controls.Add(YabanciDilCell);

                TableCell SinavAdiCell = new TableCell();
                SinavAdiCell.Text = item.SinavAdi;
                row.Controls.Add(SinavAdiCell);

                TableCell SinavNotuCell = new TableCell();
                SinavNotuCell.Text = item.SinavNotu;
                row.Controls.Add(SinavNotuCell);

                TableCell SinavTarihiCell = new TableCell();
                SinavTarihiCell.Text = item.SinavTarihi.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(SinavTarihiCell);


                TableCell SilCell = new TableCell();
                LinkButton SilBtn = new LinkButton();
                SilBtn.Text = "Sil";
                SilBtn.ID = "YabanciDilSilBtn" + SiraNo;
                IsyeriTableUpdatePanel.ContentTemplateContainer.Controls.Add(SilBtn);
                SilBtn.CssClass = "btn btn-outline-danger";
                SilBtn.Click += delegate
                {

                    try
                    {
                        bool isDeleted = item.Delete();

                        if (isDeleted)
                        {
                            FillYabanciDilTable(personel);
                            MessageHelper.PublishMessage("Kayıt Silindi.", ProjeConstants.MESAJ_BASARILI, 2000);
                        }
                        else
                            MessageHelper.PublishMessage("Kayıt Silinemedi.", ProjeConstants.MESAJ_HATA);
                    }
                    catch (Exception exception)
                    {
                        ExceptionHelper ex = new ExceptionHelper(exception);
                        ex.PublishException();
                    }
                };
                SilCell.Controls.Add(SilBtn);
                row.Controls.Add(SilCell);

                YabanciDilTable.Controls.Add(row);
            }
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
            }
            PersonelIdQS = personel.Id.ToString();
            return personel;
        }
        private void OkulTableHeaders()
        {
            OkulTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sira";
            TableHeaderCell OkulCell = new TableHeaderCell();
            OkulCell.Text = "Okul";
            TableHeaderCell SeviyeCell = new TableHeaderCell();
            SeviyeCell.Text = "Seviye";
            TableHeaderCell MezuniyetTarCell = new TableHeaderCell();
            MezuniyetTarCell.Text = "Mezuniyet Tarihi";
            TableHeaderCell AciklamaCell = new TableHeaderCell();
            AciklamaCell.Text = "Açiklama";
            TableHeaderCell SilCell = new TableHeaderCell();
            SilCell.Text = "Sil";

            th.Controls.Add(siraCell);
            th.Controls.Add(OkulCell);
            th.Controls.Add(SeviyeCell);
            th.Controls.Add(MezuniyetTarCell);
            th.Controls.Add(AciklamaCell);
            th.Controls.Add(SilCell);

            OkulTable.Controls.Add(th);
        }
        private void KursTableHeaders()
        {
            KursTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sira";
            TableHeaderCell KursCell = new TableHeaderCell();
            KursCell.Text = "Kurs/Egt/Sertifika";
            TableHeaderCell SureCell = new TableHeaderCell();
            SureCell.Text = "Kurs Süresi";
            TableHeaderCell TarihCell = new TableHeaderCell();
            TarihCell.Text = "Tarih";
            TableHeaderCell VerenKurumCell = new TableHeaderCell();
            VerenKurumCell.Text = "Veren Kurum";
            TableHeaderCell SilCell = new TableHeaderCell();
            SilCell.Text = "Sil";

            th.Controls.Add(siraCell);
            th.Controls.Add(KursCell);
            th.Controls.Add(SureCell);
            th.Controls.Add(TarihCell);
            th.Controls.Add(VerenKurumCell);
            th.Controls.Add(SilCell);

            KursTable.Controls.Add(th);
        }
        private void IsyeriTableHeaders()
        {
            IsyeriTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sira";
            TableHeaderCell IsyeriCell = new TableHeaderCell();
            IsyeriCell.Text = "Çalistigi Isyeri";
            TableHeaderCell GorevCell = new TableHeaderCell();
            GorevCell.Text = "Yaptigi Görev";
            TableHeaderCell BasTarCell = new TableHeaderCell();
            BasTarCell.Text = "Baslama Tarihi";
            TableHeaderCell BitTarCell = new TableHeaderCell();
            BitTarCell.Text = "Ayrilma Tarihi";

            TableHeaderCell SilCell = new TableHeaderCell();
            SilCell.Text = "Sil";

            th.Controls.Add(siraCell);
            th.Controls.Add(IsyeriCell);
            th.Controls.Add(GorevCell);
            th.Controls.Add(BasTarCell);
            th.Controls.Add(BitTarCell);
            th.Controls.Add(SilCell);

            IsyeriTable.Controls.Add(th);
        }
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_PERSONEL_LIST;
            if (SenderAppQS.Equals("PL"))
                newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_PERSONEL_LIST;
            if (SenderAppQS.Equals("PD"))
                newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_PERSONEL_EDIT + "?PersonelId=" + PersonelIdQS + "&DestinationApp=PerD";
            Page.Response.Redirect(newUrl);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }

        protected void OkulEkleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool isSaved = OkulEkle();
                if (isSaved)
                {
                    Personel personel = PersonelGetir();
                    FillOkulTable(personel);
                    MessageHelper.PublishMessage("Kayıt Eklendi.", ProjeConstants.MESAJ_BASARILI, 2000);
                }

                else
                    MessageHelper.PublishMessage("Kayıt Eklenemedi.", ProjeConstants.MESAJ_HATA);
            }
            catch (Exception exception)
            {

                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();
            }

        }

        private bool OkulEkle()
        {
            Egitim egitim = new Egitim();
            egitim.PersonelId = PersonelIdQS.ConvertToInt(); ;
            egitim.Okul = OkulTxt.Text;
            egitim.Seviye = OkulSeviyeDDL.SelectedItem.Value;
            egitim.MezuniyetTar = MezuniyetTarTxt.Value.ConvertToDatetime();
            egitim.Aciklama = AciklamaTxt.Text;
            int egitimId = egitim.Save();
            if (egitimId > 0)
                return true;
            else return false;

        }
        private bool KursEkle()
        {
            Kurs kurs = new Kurs();
            kurs.PersonelId = PersonelIdQS.ConvertToInt(); ;
            kurs.KursAdi = KursAdiTxt.Text;
            kurs.Tarih = TarihTxt.Value.ConvertToDatetime();
            kurs.Sure = SureTxt.Text;
            kurs.VerenKurum = VerenKurumTxt.Text;
            int kursId = kurs.Save();
            if (kursId > 0)
                return true;
            else return false;

        }
        private bool IsyeriEkle()
        {
            IsTecrube istecrube = new IsTecrube();
            istecrube.PersonelId = PersonelIdQS.ConvertToInt(); ;
            istecrube.Isyeri = IsyeriTxt.Text;
            istecrube.Gorevi = GoreviTxt.Text;
            istecrube.BasTar = BasTarTxt.Value.ConvertToDatetime();
            istecrube.BitTar = BitTarTxt.Value.ConvertToDatetime();

            int isyeriId = istecrube.Save();
            if (isyeriId > 0)
                return true;
            else return false;

        }
        private bool YabanciDilEkle()
        {
            YabanciDil yabanciDil = new YabanciDil();
            yabanciDil.PersonelId = PersonelIdQS.ConvertToInt(); ;
            yabanciDil.Dil= YabanciDilDDL.SelectedItem.Text;
            yabanciDil.SinavAdi= SinavAdiTxt.Text;
            yabanciDil.SinavNotu = SinavNotuTxt.Text;
            yabanciDil.SinavTarihi = SinavTarihiTxt.Value.ConvertToDatetime();

            int id = yabanciDil.Save();
            if (id > 0)
                return true;
            else return false;

        }
        protected void KursEkleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool isSaved = KursEkle();
                if (isSaved)
                {
                    Personel personel = PersonelGetir();
                    FillKursTable(personel);
                    MessageHelper.PublishMessage("Kayıt Eklendi.", ProjeConstants.MESAJ_BASARILI, 2000);
                }

                else
                    MessageHelper.PublishMessage("Kayıt Eklenemedi.", ProjeConstants.MESAJ_HATA);
            }
            catch (Exception exception)
            {

                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();
            }
        }
        protected void IsyeriEkleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool isSaved = IsyeriEkle();
                if (isSaved)
                {
                    Personel personel = PersonelGetir();
                    FillIsyeriTable(personel);
                    MessageHelper.PublishMessage("Kayıt Eklendi.", ProjeConstants.MESAJ_BASARILI, 2000);
                }

                else
                    MessageHelper.PublishMessage("Kayıt Eklenemedi.", ProjeConstants.MESAJ_HATA);
            }
            catch (Exception exception)
            {

                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();
            }
        }

        protected void YabanciDilEkleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool isSaved = YabanciDilEkle();
                if (isSaved)
                {
                    Personel personel = PersonelGetir();
                    FillYabanciDilTable(personel);
                    MessageHelper.PublishMessage("Kayıt Eklendi.", ProjeConstants.MESAJ_BASARILI, 2000);
                }

                else
                    MessageHelper.PublishMessage("Kayıt Eklenemedi.", ProjeConstants.MESAJ_HATA);
            }
            catch (Exception exception)
            {

                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();
            }
        }
    }
}
