using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.IzinDonemListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class IzinDonemListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public IzinDonemListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
                if (!Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty(MesajQS))
                    {
                        MessageHelper.PublishMessage("Kayit Tamamlandi", ProjeConstants.MESAJ_BASARILI, 2000);
                        MesajQS = string.Empty;
                    }
                    FillPersonelDDL();
                }
                Personel personel = new Personel();
                personel = personel.Select<Personel>(PersonelDDL.SelectedItem.Value.ConvertToInt());
                if (personel != null)
                {
                    FillIzinDonemTable();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void IzinTableHeaders()
        {
            IzinDonemTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sira";
            //TableHeaderCell izinBasTarCell = new TableHeaderCell();
            //izinBasTarCell.Text = "Ise Giris Tarihi";
            TableHeaderCell izinDonemiCell = new TableHeaderCell();
            izinDonemiCell.Text = "Izin Dönemi";
            TableHeaderCell izinHakkiCell = new TableHeaderCell();
            izinHakkiCell.Text = "Izin Hakki";
            TableHeaderCell kullanilanIzinCell = new TableHeaderCell();
            kullanilanIzinCell.Text = "Kullanilan Izin";
            TableHeaderCell kalanIzinCell = new TableHeaderCell();
            kalanIzinCell.Text = "Kalan Izin";
            TableHeaderCell aciklamaCell = new TableHeaderCell();
            aciklamaCell.Text = "Açiklama";
            TableHeaderCell duzenleCell = new TableHeaderCell();
            duzenleCell.Text = "Düzenle";
            TableHeaderCell izinlerCell = new TableHeaderCell();
            izinlerCell.Text = "Izinler";

            th.Controls.Add(siraCell);

            th.Controls.Add(izinDonemiCell);
            th.Controls.Add(izinHakkiCell);
            th.Controls.Add(kullanilanIzinCell);
            th.Controls.Add(kalanIzinCell);
            th.Controls.Add(aciklamaCell);
            th.Controls.Add(duzenleCell);
            th.Controls.Add(izinlerCell);

            IzinDonemTable.Controls.Add(th);
        }
        private void FillIzinDonemTable()
        {
            IzinTableHeaders();

            Personel personel = new Personel();
            personel = personel.Select<Personel>(PersonelDDL.SelectedItem.Value.ConvertToInt());
            if (personel != null)
            {
                IzinDonem izinDonem = new IzinDonem();
                DataTable dataTable = izinDonem.SelectByPersonelIdReturnDT(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT);
                int sira = 0;
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    string izinDonemiBasTar = dataRow["BaslangicTarihi"].ConvertToDatetimeEmptyIfNull();
                    string izinDonemiBitTar = dataRow["BitisTarihi"].ConvertToDatetimeEmptyIfNull();
                    string izinDonemiStr = izinDonemiBasTar + "-" + izinDonemiBitTar;
                    string aciklama = dataRow["Aciklama"].ToString();
                    int izinTipi = dataRow["IzinTipi"].ConvertToInt();
                    int izinDonemId = dataRow["IzinDonemId"].ConvertToInt();
                    TableRow row = new TableRow();

                    TableCell siraCell = new TableCell();
                    siraCell.Text = (++sira).ToString();
                    row.Controls.Add(siraCell);

                    string izinHakki = dataRow["IzinHakki"].ToString();
                    string izinHakkiStr = izinHakki;
                    string kullanilanIzin = dataRow["KullanilanIzin"].ToString();
                    string kullanilanIzinStr = kullanilanIzin;
                    string kalanIzin = dataRow["KalanIzin"].ToString();
                    string kalanIzinStr = kalanIzin;
                    string birim = dataRow["Birim"].ToString();

                    if (izinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                    {
                        TimeSpan izinHakkiTS = new TimeSpan(0, 0, 0);
                        izinHakkiTS = izinHakki.ConvertToTimeSpan();
                        izinHakkiStr = izinHakki.ConvertToTimeSpanReturnInHHmm();
                        kullanilanIzinStr = kullanilanIzin.ConvertToTimeSpanReturnInHHmm();
                        kalanIzinStr = kalanIzin.ConvertToTimeSpanReturnInHHmm();
                    }
                    else
                    {
                        izinHakkiStr = izinHakki + " " + birim;
                        kullanilanIzinStr = kullanilanIzin + " " + birim;
                        kalanIzinStr = kalanIzin + " " + birim;
                    }

                    TableCell IzinDonemiCell = new TableCell();

                    IzinDonemiCell.Text = izinDonemiStr;
                    row.Controls.Add(IzinDonemiCell);

                    TableCell IzinHakkiCell = new TableCell();
                    IzinHakkiCell.Text = izinHakkiStr;
                    row.Controls.Add(IzinHakkiCell);

                    TableCell KullanilanIzinCell = new TableCell();

                    KullanilanIzinCell.Text = kullanilanIzinStr;
                    row.Controls.Add(KullanilanIzinCell);

                    TableCell KalanIzinCell = new TableCell();
                    KalanIzinCell.Text = kalanIzinStr;
                    if (kalanIzin.Contains("-"))
                    {
                        KalanIzinCell.ForeColor = System.Drawing.Color.Red;
                        KalanIzinCell.Font.Bold = true;
                    }
                    row.Controls.Add(KalanIzinCell);

                    TableCell AciklamaCell = new TableCell();

                    AciklamaCell.Text = aciklama;
                    row.Controls.Add(AciklamaCell);

                    TableCell DuzenleCell = new TableCell();
                    LinkButton DuzenleBtn = new LinkButton();
                    DuzenleBtn.Text = "Düzenle";
                    DuzenleBtn.ID = "DuzenleBtn" + sira;
                    TableUpdatePanel.ContentTemplateContainer.Controls.Add(DuzenleBtn);
                    DuzenleBtn.CssClass = "btn btn-outline-danger";
                    DuzenleBtn.Click += delegate
                    {
                        try
                        {
                            BasBitTarDiv.Attributes["style"] = "display:none";
                            ModalTitleLbl.Text = personel.Adi + " " + personel.Soyadi;
                            if (izinDonemId > 0)
                            {
                                IzinHakkiTxt.Enabled = true;
                                KullanilanIzinTxt.Enabled = true;
                                KalanIzinTxt.Enabled = true;
                                UpdateNowBtn.Visible = true;
                                DonemEkleNowBtn.Visible = false;
                                UyariMesajiLbl.Visible = true;
                                IzinDonemIdLbl.Text = izinDonemId.ReturnZeroIfNull().ToString();

                                IzinHakkiTxt.Text = izinHakki;
                                KullanilanIzinTxt.Text = kullanilanIzin;
                                KalanIzinTxt.Text = kalanIzin;
                                ModalSubTitleLbl.Text = izinDonemiStr + " İzin Dönemi";
                                var openPopup = "OpenModal();";
                                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);

                            }
                            else
                            {
                                MessageHelper.PublishMessage("İzin Dönemi Bulunamadı", ProjeConstants.MESAJ_HATA);
                            }

                        }
                        catch (Exception exception)
                        {
                            ExceptionHelper ex = new ExceptionHelper(exception);
                            ex.PublishException();
                        }
                    };
                    DuzenleCell.Controls.Add(DuzenleBtn);
                    row.Controls.Add(DuzenleCell);

                    TableCell IzinlerCell = new TableCell();
                    LinkButton IzinlerBtn = new LinkButton();
                    IzinlerBtn.Text = "İzinler";
                    IzinlerBtn.ID = "IzinlerBtn" + sira;
                    TableUpdatePanel.ContentTemplateContainer.Controls.Add(IzinlerBtn);
                    IzinlerBtn.CssClass = "btn btn-outline-secondary";
                    IzinlerBtn.Click += delegate
                    {
                        try
                        {
                            BasBitTarDiv.Attributes["style"] = "display:none";
                            ModalTitleLbl.Text = personel.Adi + " " + personel.Soyadi;
                            if (izinDonemId > 0)
                            {
                                IzinHakkiTxt.Enabled = false;
                                KullanilanIzinTxt.Enabled = false;
                                KalanIzinTxt.Enabled = false;
                                UpdateNowBtn.Visible = false;
                                DonemEkleNowBtn.Visible = false;
                                UyariMesajiLbl.Visible = false;
                                IzinDonemIdLbl.Text = izinDonemId.ReturnZeroIfNull().ToString();
                                IzinHakkiTxt.Text = izinHakki;
                                KullanilanIzinTxt.Text = kullanilanIzin;
                                KalanIzinTxt.Text = kalanIzin;
                                ModalSubTitleLbl.Text = izinDonemiStr + " İzin Dönemi";
                                FillIzinHareketleriTable(izinDonemId);
                                var openPopup = "OpenModal();";
                                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);

                            }
                            else
                            {
                                MessageHelper.PublishMessage("İzin Dönemi Bulunamadı", ProjeConstants.MESAJ_HATA);
                            }

                        }
                        catch (Exception exception)
                        {
                            ExceptionHelper ex = new ExceptionHelper(exception);
                            ex.PublishException();
                        }
                    };
                    IzinlerCell.Controls.Add(IzinlerBtn);
                    row.Controls.Add(IzinlerCell);

                    IzinDonemTable.Controls.Add(row);
                }

            }

        }
        private void IzinHareketTableHeaders()
        {
            IzinHareketTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "";
            //TableHeaderCell izintipiCell = new TableHeaderCell();
            //izintipiCell.Text = "Izin Tipi";
            TableHeaderCell bastarCell = new TableHeaderCell();
            bastarCell.Text = "Başlangıç tarihi";
            TableHeaderCell bittarCell = new TableHeaderCell();
            bittarCell.Text = "Bitiş tarihi";
            TableHeaderCell sureCell = new TableHeaderCell();
            sureCell.Text = "İzinli Süre";
            TableHeaderCell aciklamaCell = new TableHeaderCell();
            aciklamaCell.Text = "Açıklama";
            th.Controls.Add(siraCell);
            //th.Controls.Add(izintipiCell);
            th.Controls.Add(bastarCell);
            th.Controls.Add(bittarCell);
            th.Controls.Add(sureCell);
            th.Controls.Add(aciklamaCell);
            IzinHareketTable.Controls.Add(th);
        }
        private void FillIzinHareketleriTable(int izinDonemId)
        {
            IzinDonem izinDonemi = new IzinDonem();
            izinDonemi = izinDonemi.Select<IzinDonem>(izinDonemId);
            if (izinDonemi == null)
            {
                MessageHelper.PublishMessage("İzin Dönemi bulunamadı.", ProjeConstants.MESAJ_HATA);
            }
            else
            {
                IzinHareket izinHareket = new IzinHareket();
                DataTable dataTable = izinHareket.SelectByIzinDonemiReturnDataTable(izinDonemId, izinDonemi.PersonelId, ProjeConstants.IZINTIPI_UCRETLI_INT);
                int SiraNo = 1;
                if (dataTable == null)
                {
                    IzinHareketTable.Rows.Clear();
                    TableRow tr = new TableRow();
                    TableCell tc = new TableCell();
                    tc.Text = "Henüz izin kullanılmamış.";
                    tr.Controls.Add(tc);
                    IzinHareketTable.Controls.Add(tr);
                }
                else
                {
                    IzinHareketTableHeaders();
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        string izinTipi = dataRow["IzinTipi"].ReturnEmptyIfNull().ToString();
                        string aciklama = dataRow["Aciklama"].ReturnEmptyIfNull().ToString();
                        bool mahsup = dataRow["Mahsup"].ReturnFalseIfNull().ConvertToBool();
                        izinTipi = mahsup == false ? izinTipi : izinTipi + "(M)";
                        DateTime bastar = dataRow["BaslangicTarihi"].ConvertToDatetime();
                        DateTime bittar = dataRow["BitisTarihi"].ConvertToDatetime();
                        string sure = dataRow["Sure"].ToString();
                        string birim = dataRow["Birim"].ToString();
                        int izinTalepId = dataRow["IzinTalepId"].ConvertToInt();
                        //int izinDonemId = dataRow["IzinDonemId"].ConvertToInt();
                        TableRow row = new TableRow();
                        row.ToolTip = aciklama;
                        TableCell SiraNoCell = new TableCell();

                        SiraNoCell.Text = SiraNo++ + "";
                        row.Controls.Add(SiraNoCell);

                        //TableCell IzinTipiCell = new TableCell();
                        //IzinTipiCell.Text = izinTipi;
                        //row.Controls.Add(IzinTipiCell);

                        TableCell BasTarCell = new TableCell();
                        BasTarCell.Text = bastar.ConvertToDatetimeEmptyIfNull();
                        row.Controls.Add(BasTarCell);

                        TableCell BitTarCell = new TableCell();
                        BitTarCell.Text = bittar.ConvertToDatetimeEmptyIfNull();
                        row.Controls.Add(BitTarCell);

                        TableCell SureCell = new TableCell();
                        SureCell.Text = sure + " " + birim;
                        row.Controls.Add(SureCell);

                        TableCell AciklamaCell = new TableCell();
                        AciklamaCell.Text = aciklama;
                        row.Controls.Add(AciklamaCell);
                        IzinHareketTable.Controls.Add(row);
                    }
                }
            }


        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl + "?MahsupMesaji=true";
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void FillPersonelDDL()
        {
            PersonelDDL.Items.Clear();
            Personel personel = new Personel();
            List<Personel> list = personel.SelectCalisanPersonel();
            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                PersonelDDL.Items.Add(li);
            }
        }
        protected void PersonelDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            Personel personel = new Personel();
            personel = personel.Select<Personel>(PersonelDDL.SelectedItem.Value.ConvertToInt());
            if (personel != null)
            {
                FillIzinDonemTable();
            }


        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void UpdateNowBtn_Click(object sender, EventArgs e)
        {
            int izinDonemId = IzinDonemIdLbl.Text.ConvertToInt();
            IzinDonem izinDonemi = new IzinDonem();
            izinDonemi = izinDonemi.Select<IzinDonem>(izinDonemId);
            if (izinDonemi != null)
            {
                try
                {
                    //int girildiginden emin ol
                    int value = 0;
                    bool validated = false;
                    if ((int.TryParse(IzinHakkiTxt.Text, out value)) &&
                            (int.TryParse(KullanilanIzinTxt.Text, out value)) &&
                            (int.TryParse(KalanIzinTxt.Text, out value)))
                    {
                        izinDonemi.IzinHakki = IzinHakkiTxt.Text;
                        izinDonemi.KullanilanIzin = KullanilanIzinTxt.Text;
                        izinDonemi.KalanIzin = KalanIzinTxt.Text;
                        validated = true;
                    }

                    if (validated)
                    {
                        bool issaved = izinDonemi.Update();
                        MessageHelper.PublishMessage("Kayıt Tamamlandı", ProjeConstants.MESAJ_BASARILI, 2000);
                        FillIzinDonemTable();
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Kayıt Yapılamadı. İzin Hakkı, Kullanılan İzin ve Kalan İzin değerlerine lütfen tamsayı giriniz", ProjeConstants.MESAJ_HATA);
                    }
                }
                catch (Exception)
                {

                    MessageHelper.PublishMessage("İzin Hakkı, Kullanılan İzin ve Kalan İzin bilgilerine lütfen tamsayı giriniz", ProjeConstants.MESAJ_HATA);
                }


            }
        }
        /// <summary>
        /// Dönem ekle modal pencere aç
        /// Dönem Basi tarihine ise girdigi yil+1,
        /// Bitis Sonu tarihine son en eski izin dönemi baslama tarihini yaz
        /// izin hakki, kullanilan izin ve kalan izin hanelerine 0 gir, kullanici tarafindan girilecek
        /// dönem basi sonu ve ise giris tarihlerinde olusacak çakismalare ve 
        /// ise basladigi tarihten öncesine dönem girisini izin vermemek için 
        /// modal pencereyi açtirma, mesaj ver
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DonemEkleBtn_Click(object sender, EventArgs e)
        {

            try
            {
                Personel personel = new Personel();
                personel = personel.Select<Personel>(PersonelDDL.SelectedItem.Value.ConvertToInt());
                if (personel == null)
                {
                    Exception ex = new Exception("Personel bulunamadı. ");
                    throw (ex);
                }

                BasBitTarDiv.Attributes["style"] = "display:block";
                IzinHakkiTxt.Enabled = true;
                KullanilanIzinTxt.Enabled = true;
                KalanIzinTxt.Enabled = true;
                UpdateNowBtn.Visible = false;
                DonemEkleNowBtn.Visible = true;
                UyariMesajiLbl.Visible = true;
                IzinDonemIdLbl.Text = "";
                ModalSubTitleLbl.Text = string.Empty;
                ModalTitleLbl.Text = personel.Adi + " " + personel.Soyadi;

                IzinDonem izinDonemi = new IzinDonem();
                List<IzinDonem> list = izinDonemi.SelectByPersonelId(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT);

                //modal açilista Dönem basi ve sonunu dolu getirsin en eski dönemden önceki bir yil degerini doldursun
                izinDonemi = list.FirstOrDefault<IzinDonem>();//en eski dönem (baslangiçTarihine göre sirali oldugundan)
                if (izinDonemi != null)
                {

                    IsBilgileri ib = new IsBilgileri();
                    ib = ib.SelectByPersonelId(personel.Id);
                    if (ib != null)
                    {
                        DateTime iseBaslamaTar = !string.IsNullOrEmpty(ib.IzinDonemiBasTar.ConvertToDatetimeEmptyIfNull()) ? ib.IzinDonemiBasTar : ib.BaslamaTar;
                        DateTime bastar = iseBaslamaTar.AddYears(1);
                        DateTime bittar = izinDonemi.BaslangicTarihi.AddDays(-1);

                        if (bittar < bastar)
                        {
                            Exception ex = new Exception("İşe başladığından bu yana olan tüm dönemler zaten tanımlı. Yeni izin dönemi eklenemez.");
                            throw (ex);

                        }
                        DonemBasTarTxt.Value = bastar.ConvertToDatetimeEmptyIfNull();
                        DonemBitTarTxt.Value = bittar.ConvertToDatetimeEmptyIfNull();
                    }


                }
                IzinHakkiTxt.Text = "0";
                KullanilanIzinTxt.Text = "0";
                KalanIzinTxt.Text = "0";

                var openPopup = "OpenModal();";
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);

            }
            catch (Exception exception)
            {

                ExceptionHelper exhelper = new ExceptionHelper(exception);
                exhelper.PublishException();
            }

        }
        protected void DonemEkleNowBtn_Click(object sender, EventArgs e)
        {

            IzinDonem izinDonemi = new IzinDonem();
            try
            {
                //int girildiginden emin ol
                int value = 0;
                bool validated = false;
                if ((int.TryParse(IzinHakkiTxt.Text, out value)) &&
                        (int.TryParse(KullanilanIzinTxt.Text, out value)) &&
                        (int.TryParse(KalanIzinTxt.Text, out value)))
                {
                    izinDonemi.IzinHakki = IzinHakkiTxt.Text;
                    izinDonemi.KullanilanIzin = KullanilanIzinTxt.Text;
                    izinDonemi.KalanIzin = KalanIzinTxt.Text;
                    validated = true;
                }

                if (validated)
                {
                    Personel personel = new Personel();
                    personel = personel.Select<Personel>(PersonelDDL.SelectedItem.Value.ConvertToInt());
                    if (personel == null)
                    {
                        Exception ex = new Exception("Personel bulunamadı. ");
                        throw (ex);
                    }

                    izinDonemi.PersonelId = personel.Id;
                    izinDonemi.IzinTipi = ProjeConstants.IZINTIPI_UCRETLI_INT;
                    DateTime bastar = DonemBasTarTxt.Value.ConvertToDatetime();
                    DateTime bittar = DonemBitTarTxt.Value.ConvertToDatetime();

                    IsBilgileri ib = new IsBilgileri();
                    ib = ib.SelectByPersonelId(personel.Id);
                    if (ib != null)
                    {
                        IzinDonem enEskiIzinDonemi = new IzinDonem();
                        List<IzinDonem> list = enEskiIzinDonemi.SelectByPersonelId(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT);
                        enEskiIzinDonemi = list.LastOrDefault<IzinDonem>();

                        DateTime iseBaslamaTar = ib.BaslamaTar != null ? ib.BaslamaTar : ib.IzinDonemiBasTar;
                        if (bastar < iseBaslamaTar)
                        {
                            Exception ex = new Exception("İzin Dönemi Başı İşe başlama tarihinden küçük olamaz");
                            throw (ex);
                        }
                        if (bittar > enEskiIzinDonemi.BaslangicTarihi)
                        {
                            Exception ex = new Exception("İzin Dönemi sonu itibari ile dönemler arası çakışma var");
                            throw (ex);
                        }
                    }
                    izinDonemi.BaslangicTarihi = bastar;
                    izinDonemi.BitisTarihi = bittar;
                    izinDonemi.Birim = ProjeConstants.IZIN_BIRIMI_GUN;
                    izinDonemi.Adi = bastar.Year + "-" + bittar.Year + " İzin Dönemi";
                    izinDonemi.Aciklama = AciklamaTxt.Text;
                    izinDonemi.Olusturan = CurrentUserName;
                    izinDonemi.Id = izinDonemi.Save();
                    bool issaved = izinDonemi.Id > 0 ? true : false;
                    MessageHelper.PublishMessage("Kayıt Tamamlandı", ProjeConstants.MESAJ_BASARILI, 2000);
                    FillIzinDonemTable();
                }
                else
                {
                    MessageHelper.PublishMessage("Kayıt Yapılamadı. İzin Hakkı, Kullanılan İzin ve Kalan İzin değerlerine lütfen tamsayı giriniz", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exh = new ExceptionHelper(exception);
                exh.PublishException();
            }
        }
    }
}
