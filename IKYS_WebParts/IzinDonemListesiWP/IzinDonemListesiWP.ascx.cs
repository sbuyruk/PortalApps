using Model.IKYS;
using Model.Ortak;
using Model.Services.IKYS;
using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace IKYS_WebParts.IzinDonemListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class IzinDonemListesiWP : WebPart
    {
        private PortalLogContext LoggingContext { get { return PortalLogContext.FromCurrentRequest("IKYS", "IzinDonemListesiWP"); } }
        private void ReportError(string operation, Exception error)
        {
            if (error is System.Threading.ThreadAbortException) return;
            PortalLogContext context = LoggingContext;
            PortalTechnicalLog.WriteError(context, operation, error);
            Messages.Error("İşlem tamamlanamadı. Lütfen tekrar deneyin. Başvuru kodu: " + context.OperationId.ToString("N"));
        }
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
        private int SecilenPersonelId
        {
            get { return PersonelDDL.SelectedItem == null ? 0 : PersonelDDL.SelectedItem.Value.ConvertToInt(); }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    new IzinDonemService().TryRecordWebPartOpened(LoggingContext);
                    if (!string.IsNullOrEmpty(MesajQS))
                    {
                        Messages.Success("Kayit Tamamlandi", 2000);
                        MesajQS = string.Empty;
                    }
                    FillPersonelDDL();
                }
                Personel personel = new PersonelService().GetById(SecilenPersonelId, LoggingContext);
                if (personel != null)
                {
                    FillIzinDonemTable();
                }
            }
            catch (Exception ex)
            {
                ReportError("Unhandled", ex);
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

            Personel personel = new PersonelService().GetById(SecilenPersonelId, LoggingContext);
            if (personel != null)
            {
                DataTable dataTable = new IzinDonemService().GetByPersonelIdReturnDataTable(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, LoggingContext);
                if (dataTable == null) return;
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
                                Messages.Error("İzin Dönemi Bulunamadı");
                            }

                        }
                        catch (Exception exception)
            {
                ReportError("Unhandled", exception);
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
                                Messages.Error("İzin Dönemi Bulunamadı");
                            }

                        }
                        catch (Exception exception)
            {
                ReportError("Unhandled", exception);
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
            IzinDonem izinDonemi = new IzinDonemService().GetById(izinDonemId, LoggingContext);
            if (izinDonemi == null)
            {
                Messages.Error("İzin Dönemi bulunamadı.");
            }
            else
            {
                DataTable dataTable = new IzinHareketService().GetByIzinDonemiReturnDataTable(izinDonemId, izinDonemi.PersonelId, ProjeConstants.IZINTIPI_UCRETLI_INT, LoggingContext);
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
                ReportError("Unhandled", ex);
            }
        }
        private void FillPersonelDDL()
        {
            PersonelDDL.Items.Clear();
            List<Personel> list = new PersonelService().GetActiveEmployees(Personel.PersonelTipi.Kadrolu, LoggingContext);
            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                PersonelDDL.Items.Add(li);
            }
        }
        protected void PersonelDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                FillIzinDonemTable();
            }
            catch (Exception error)
            {
                ReportError("IzinDonem.PersonelChanged", error);
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
            try
            {
                if (!DonemDegerleriGecerli()) return;
                if (!DonemiGuncelle(IzinDonemIdLbl.Text.ConvertToInt(), SecilenPersonelId))
                {
                    Messages.Error("İzin dönemi bulunamadı veya seçilen personele ait değil. Listeyi yenileyin.");
                    return;
                }
                Messages.Success("Kayıt Tamamlandı", 2000);
                FillIzinDonemTable();
            }
            catch (Exception error)
            {
                ReportError("IzinDonem.Update", error);
            }
        }

        private bool DonemDegerleriGecerli()
        {
            int value;
            if (int.TryParse(IzinHakkiTxt.Text, out value)
                && int.TryParse(KullanilanIzinTxt.Text, out value)
                && int.TryParse(KalanIzinTxt.Text, out value)) return true;
            Messages.Error("İzin Hakkı, Kullanılan İzin ve Kalan İzin değerlerine lütfen tamsayı giriniz.");
            return false;
        }

        private bool DonemiGuncelle(int izinDonemId, int personelId)
        {
            PortalLogContext context = LoggingContext;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                IzinDonemService service = new IzinDonemService();
                IzinDonem donem = service.GetById(izinDonemId, transaction, context);
                if (donem == null || personelId <= 0 || donem.PersonelId != personelId
                    || donem.IzinTipi != ProjeConstants.IZINTIPI_UCRETLI_INT) return false;
                donem.IzinHakki = IzinHakkiTxt.Text;
                donem.KullanilanIzin = KullanilanIzinTxt.Text;
                donem.KalanIzin = KalanIzinTxt.Text;
                donem.Degistiren = CurrentUserName;
                if (!service.Update(donem, transaction, context))
                    throw new InvalidOperationException("İzin dönemi güncellenemedi.");
                transaction.Complete();
                return true;
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
                Personel personel = new PersonelService().GetById(SecilenPersonelId, LoggingContext);
                if (personel == null)
                {
                    Messages.Error("Personel bulunamadı. Listeyi yenileyin.");
                    return;
                }
                IsBilgileri bilgi = new IsBilgileriService().GetByPersonelId(personel.Id, LoggingContext);
                if (bilgi == null)
                {
                    Messages.Error("Personelin iş bilgileri bulunamadı.");
                    return;
                }
                List<IzinDonem> list = new IzinDonemService().GetByPersonelId(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, LoggingContext);
                IzinDonem enEskiDonem = list.LastOrDefault();
                DonemBasTarTxt.Value = string.Empty;
                DonemBitTarTxt.Value = string.Empty;
                if (enEskiDonem != null)
                {
                    DateTime iseBaslama = !string.IsNullOrEmpty(bilgi.IzinDonemiBasTar.ConvertToDatetimeEmptyIfNull())
                        ? bilgi.IzinDonemiBasTar : bilgi.BaslamaTar;
                    if (string.IsNullOrEmpty(iseBaslama.ConvertToDatetimeEmptyIfNull()))
                    {
                        Messages.Error("İşe başlama veya izin dönemi başlangıç tarihi belirlenemedi.");
                        return;
                    }
                    DateTime baslangic = iseBaslama.AddYears(1);
                    DateTime bitis = enEskiDonem.BaslangicTarihi.AddDays(-1);
                    if (bitis < baslangic)
                    {
                        Messages.Error("İşe başladığından bu yana olan tüm dönemler zaten tanımlı. Yeni izin dönemi eklenemez.");
                        return;
                    }
                    DonemBasTarTxt.Value = baslangic.ConvertToDatetimeEmptyIfNull();
                    DonemBitTarTxt.Value = bitis.ConvertToDatetimeEmptyIfNull();
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
                IzinHakkiTxt.Text = "0";
                KullanilanIzinTxt.Text = "0";
                KalanIzinTxt.Text = "0";
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                    typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "OpenModal();", true);
            }
            catch (Exception error)
            {
                ReportError("IzinDonem.Prepare", error);
            }
        }
        protected void DonemEkleNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!DonemDegerleriGecerli()) return;
                DateTime baslangic;
                DateTime bitis;
                if (!DateTime.TryParse(DonemBasTarTxt.Value, System.Globalization.CultureInfo.GetCultureInfo("tr-TR"),
                        System.Globalization.DateTimeStyles.None, out baslangic)
                    || !DateTime.TryParse(DonemBitTarTxt.Value, System.Globalization.CultureInfo.GetCultureInfo("tr-TR"),
                        System.Globalization.DateTimeStyles.None, out bitis)
                    || bitis < baslangic)
                {
                    Messages.Error("Geçerli başlangıç ve bitiş tarihleri giriniz. Bitiş tarihi başlangıçtan önce olamaz.");
                    return;
                }
                if (DonemiKaydet(SecilenPersonelId, baslangic, bitis) <= 0) return;
                Messages.Success("Kayıt Tamamlandı", 2000);
                FillIzinDonemTable();
            }
            catch (Exception error)
            {
                ReportError("IzinDonem.Create", error);
            }
        }

        private int DonemiKaydet(int personelId, DateTime baslangic, DateTime bitis)
        {
            PortalLogContext context = LoggingContext;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                Personel personel = new PersonelService().GetById(personelId, transaction, context);
                if (personel == null)
                {
                    Messages.Error("Personel bulunamadı. Listeyi yenileyin.");
                    return 0;
                }
                IsBilgileri bilgi = new IsBilgileriService().GetByPersonelId(personel.Id, transaction, context);
                if (bilgi == null)
                {
                    Messages.Error("Personelin iş bilgileri bulunamadı.");
                    return 0;
                }
                DateTime iseBaslama = !string.IsNullOrEmpty(bilgi.BaslamaTar.ConvertToDatetimeEmptyIfNull())
                    ? bilgi.BaslamaTar : bilgi.IzinDonemiBasTar;
                if (string.IsNullOrEmpty(iseBaslama.ConvertToDatetimeEmptyIfNull()) || baslangic < iseBaslama)
                {
                    Messages.Error("İzin dönemi başlangıcı geçerli işe başlama tarihinden önce olamaz.");
                    return 0;
                }
                IzinDonemService service = new IzinDonemService();
                List<IzinDonem> donemler = service.GetByPersonelId(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, transaction, context);
                IzinDonem enEskiDonem = donemler.LastOrDefault();
                if (enEskiDonem != null && bitis >= enEskiDonem.BaslangicTarihi)
                {
                    Messages.Error("Yeni dönem mevcut en eski dönemden önce bitmelidir. Dönemler çakışamaz.");
                    return 0;
                }
                IzinDonem donem = new IzinDonem();
                donem.PersonelId = personel.Id;
                donem.IzinTipi = ProjeConstants.IZINTIPI_UCRETLI_INT;
                donem.BaslangicTarihi = baslangic;
                donem.BitisTarihi = bitis;
                donem.Birim = ProjeConstants.IZIN_BIRIMI_GUN;
                donem.Adi = baslangic.Year + "-" + bitis.Year + " İzin Dönemi";
                donem.Aciklama = AciklamaTxt.Text;
                donem.Olusturan = CurrentUserName;
                donem.IzinHakki = IzinHakkiTxt.Text;
                donem.KullanilanIzin = KullanilanIzinTxt.Text;
                donem.KalanIzin = KalanIzinTxt.Text;
                int id = service.Save(donem, transaction, context);
                if (id <= 0) throw new InvalidOperationException("İzin dönemi oluşturulamadı.");
                transaction.Complete();
                return id;
            }
        }
    }
}
