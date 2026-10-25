using Model.IKYS;
using Model.Ortak;
using Model.Services.IKYS;
using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace IKYS_WebParts.UcretliMahsupWP
{
    [ToolboxItemAttribute(false)]
    public partial class UcretliMahsupWP : WebPart
    {
        private PortalLogContext LoggingContext { get { return PortalLogContext.FromCurrentRequest("IKYS", "UcretliMahsupWP"); } }
        private void ReportError(string operation, Exception error)
        {
            PortalLogContext context = LoggingContext;
            if (error is System.Threading.ThreadAbortException) return;
            PortalTechnicalLog.WriteError(context, operation, error);
            Messages.Error("İşlem tamamlanamadı. Lütfen tekrar deneyin. Başvuru kodu: " + context.OperationId.ToString("N"));
        }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public UcretliMahsupWP()
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
        private string SecilenIdQS
        {
            get
            {

                if (ViewState["SecilenId"] == null)
                {
                    if (Page.Request.QueryString["SecilenId"] != null)
                    {
                        ViewState["SecilenId"] = Page.Request.QueryString["SecilenId"];
                    }
                    else
                    {
                        ViewState["SecilenId"] = string.Empty;
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    new IzinHareketService().TryRecordWebPartOpened(LoggingContext);
                    if (!string.IsNullOrEmpty(MesajQS))
                    {
                        Messages.Success("Mahsup Tamamlandi", 2000);
                        MesajQS = string.Empty;
                    }
                    FillPersonelDDL();
                    FillDonemDDL();
                    TabloOlustur();
                }

            }
            catch (Exception ex)
            {
                ReportError("Unhandled", ex);
            }
        }
        private string GetDataJson()
        {
            IzinHareketService izinHareketService = new IzinHareketService();
            int personelId = PersonelDDL.SelectedItem == null ? 0 : PersonelDDL.SelectedItem.Value.ConvertToInt();
            int donemId = DonemDDL.SelectedItem == null ? 0 : DonemDDL.SelectedItem.Value.ConvertToInt();
            var json = izinHareketService.GetByPersonelIdDonemIdReturnJson(personelId, donemId, ProjeConstants.IZINTIPI_UCRETLI_INT, LoggingContext);

            return json;
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl ;
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
            ListItem li0 = new ListItem(ProjeConstants.HEPSI, ProjeConstants.IZINTIPI_HEPSI_INT.ToString());
            PersonelDDL.Items.Add(li0);

            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                PersonelDDL.Items.Add(li);
            }


        }
        private void FillDonemDDL()
        {
            DonemDDL.Items.Clear();
            int personelId = PersonelDDL.SelectedItem.Value.ConvertToInt();
            ListItem li0 = new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI.ToString());
            DonemDDL.Items.Add(li0);
            if (personelId > 0)
            {
                List<IzinDonem> list = new IzinDonemService().GetByPersonelId(personelId, ProjeConstants.IZINTIPI_UCRETLI_INT, LoggingContext);
                foreach (IzinDonem item in list)
                {
                    ListItem li = new ListItem(item.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + item.BitisTarihi.ConvertToDatetimeEmptyIfNull(), item.Id.ReturnZeroIfNull().ToString());
                    DonemDDL.Items.Add(li);
                }
            }
        }
        private void FillModalDonemDDL(Personel personel, int izinHareketId)
        {
            ModalDonemDDL.Items.Clear();
            if (personel.Id > 0)
            {
                List<IzinDonem> list = new IzinDonemService().GetByPersonelId(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, LoggingContext);
                int donemSayisi = 0;
                IzinHareket izinHareket = new IzinHareketService().GetById(izinHareketId, LoggingContext);
                int sure = izinHareket.Sure.ConvertToInt();
                foreach (IzinDonem item in list)
                {
                    int kalanIzin = item.KalanIzin.ConvertToInt();
                    if (kalanIzin >= sure)
                    {
                        ListItem li = new ListItem(item.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + item.BitisTarihi.ConvertToDatetimeEmptyIfNull()
                            + " ( Kalan izin = " + kalanIzin + " " + item.Birim + " )", item.Id.ReturnZeroIfNull().ToString());
                        ModalDonemDDL.Items.Add(li);

                        donemSayisi++;
                    }

                }

                FillOnayLbl(personel, donemSayisi > 0);
            }
        }
        private void FillOnayLbl(Personel personel, bool isUygunDonemVar)
        {
            IzinHareket izinHareket = new IzinHareketService().GetById(paramIzinHareketIdLbl.Value.ConvertToInt(), LoggingContext);
            if (isUygunDonemVar)
            {
                OnayLbl.Text = "Onayladiginiz takdirde " + personel.Adi + " " + personel.Soyadi + " Tarafindan " + izinHareket.BaslangicTarihi.ToString("dd.MM.yyyy") + " - "
                + izinHareket.BitisTarihi.ToString("dd.MM.yyyy") + " Tarihleri Arasinda kullanilan " + izinHareket.Sure + " " + izinHareket.Birim
                + " süreli izin için MAHSUP ISLEMI uygulanacaktir.";
                MahsupEtModalBtn.Visible = true;

            }
            else
            {
                OnayLbl.Text = "Mahsup islemi için uygun dönem bulunmamaktadir.";
                MahsupEtModalBtn.Visible = false;
            }

        }
        private void FillModalIzinBilgileriTable(Personel personel)
        {
            DateTime today = DateTime.Today;
            IsBilgileri ib = new IsBilgileriService().GetByPersonelId(personel.Id, LoggingContext);
            if (ib != null)
            {

                DateTime izinDonemiBasTar = ib.IzinDonemiBasTar;
                //int izinDonemiSayisi = (today.Year - izinDonemiBasTar.Year);
                //izinDonemiSayisi = izinDonemiSayisi > 5 ? 5 : izinDonemiSayisi;
                List<IzinDonem> izinDonemiList = new IzinDonemService().GetByPersonelId(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, LoggingContext);

                IzinHareket izinHareket = new IzinHareket();

                IzinBilgileriTableHeaders();
                foreach (IzinDonem izinDonemi in izinDonemiList)
                {
                    TableRow row = new TableRow();
                    TableCell IzinDonemiCell = new TableCell();

                    List<Mahsup> mahsupList = new MahsupService().GetByDonemId(izinDonemi.Id, LoggingContext);
                    IzinDonemiCell.Text = izinDonemi.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + izinDonemi.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                    if (mahsupList.Count > 0)
                        IzinDonemiCell.Text += "(M)";
                    row.Controls.Add(IzinDonemiCell);


                    string izinHakki = string.Empty;
                    TableCell IzinHakkiCell = new TableCell();
                    izinHakki = izinDonemi.IzinHakki;
                    IzinHakkiCell.Text = izinHakki.ReturnZeroIfNull().ToString();
                    row.Controls.Add(IzinHakkiCell);

                    TableCell KullanilanIzinCell = new TableCell();
                    KullanilanIzinCell.Text = izinDonemi.KullanilanIzin.ReturnZeroIfNull().ToString();
                    row.Controls.Add(KullanilanIzinCell);

                    TableCell KalanIzinCell = new TableCell();
                    KalanIzinCell.Text = izinDonemi.KalanIzin.ReturnZeroIfNull().ToString();
                    row.Controls.Add(KalanIzinCell);
                    IzinBilgileriTable.Controls.Add(row);

                }
            }
            else
            {
                Messages.Error("Ise baslama tarihi belirlenemedi");
            }
        }
        private void IzinBilgileriTableHeaders()
        {
            IzinBilgileriTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell donemCell = new TableHeaderCell();
            donemCell.Text = "Izin Dönemi";
            TableHeaderCell hakCell = new TableHeaderCell();
            hakCell.Text = "Izin Hakki";
            TableHeaderCell kullanilanCell = new TableHeaderCell();
            kullanilanCell.Text = "Kullanilan Izin";
            TableHeaderCell kalanCell = new TableHeaderCell();
            kalanCell.Text = "Kalan Izin";
            th.Controls.Add(donemCell);
            th.Controls.Add(hakCell);
            th.Controls.Add(kullanilanCell);
            th.Controls.Add(kalanCell);
            IzinBilgileriTable.Controls.Add(th);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void OnaylaBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool isSaved = MahsupIsleminiyap();
                if (isSaved)
                {
                    RedirectToPage(ProjeConstants.PAGE_UCRETLIMAHSUP+"?SecilenId="+SecilenIdQS+"&Mesaj=true");
                }
                TabloOlustur();
            }
            catch (Exception exception)
            {
                ReportError("Unhandled", exception);
            }

        }
        protected void ModalInfoBtn_Click(object sender, EventArgs e)
        {
            //int personelId = paramPersonelIdLbl.Value.ConvertToInt();
            int izinHareketId = paramIzinHareketIdLbl.Value.ConvertToInt();
            SecilenIdQS = izinHareketId.ToString();
            IzinHareket izinHareket = new IzinHareketService().GetById(izinHareketId, LoggingContext);
            if (izinHareket!=null)
            {
                int personelId = izinHareket.PersonelId;
                Personel personel = new PersonelService().GetById(personelId, LoggingContext);
                if (personel != null)
                {
                    PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                    FillModalDonemDDL(personel, izinHareketId);
                    FillModalIzinBilgileriTable(personel);

                }
            }
        }
        protected void PersonelDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDonemDDL();
            TabloOlustur();
        }
        protected void DonemDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }


        private Mahsup MahsupTablosunaEkle(IzinHareket izinHareket, int asilIzinTipi, int kullanildigiDonem, int mahsupDonemi, string aciklama, SqlTransactionContext transaction, PortalLogContext context)
        {
            Mahsup mahsup = new Mahsup();
            mahsup.PersonelId = izinHareket.PersonelId;
            mahsup.IzinHareketId = izinHareket.Id;
            mahsup.IzinTipi = asilIzinTipi;
            mahsup.KullanildigiDonemId = kullanildigiDonem;
            mahsup.MahsupDonemId = mahsupDonemi;
            mahsup.Aciklama = aciklama;
            mahsup.Olusturan = CurrentUserName;
            int mahsupid = new MahsupService().Save(mahsup, transaction, context);
            if (mahsupid <= 0) throw new InvalidOperationException("Mahsup kaydı oluşturulamadı.");
            return mahsup;
        }
        private bool MahsupIsleminiyap()
        {
            PortalLogContext context = LoggingContext;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                IzinHareketService hareketService = new IzinHareketService();
                IzinDonemService donemService = new IzinDonemService();
                IzinHareket hareket = hareketService.GetById(paramIzinHareketIdLbl.Value.ConvertToInt(), transaction, context);
                if (hareket == null || hareket.IzinTipi != ProjeConstants.IZINTIPI_UCRETLI_INT)
                    throw new InvalidOperationException("Ücretli izin hareketi bulunamadı.");
                int hedefId = ModalDonemDDL.SelectedItem == null ? 0 : ModalDonemDDL.SelectedItem.Value.ConvertToInt();
                if (hedefId <= 0 || hedefId == hareket.IzinDonemId)
                {
                    Messages.Error("Farklı bir hedef izin dönemi seçiniz.");
                    return false;
                }
                IzinDonem eski = donemService.GetById(hareket.IzinDonemId, transaction, context);
                IzinDonem yeni = donemService.GetById(hedefId, transaction, context);
                if (eski == null || yeni == null || eski.PersonelId != hareket.PersonelId || yeni.PersonelId != hareket.PersonelId
                    || eski.IzinTipi != hareket.IzinTipi || yeni.IzinTipi != hareket.IzinTipi)
                    throw new InvalidOperationException("Harekete ait izin dönemleri bulunamadı.");
                int sure = hareket.Sure.ConvertToInt();
                string aciklama = "# Önceki izin dönemi= " + eski.BaslangicTarihi + "-" + eski.BitisTarihi
                    + "; Yeni izin dönemi= " + yeni.BaslangicTarihi + "-" + yeni.BitisTarihi + "; ; Mahsup Edilen süre = " + hareket.Sure;
                Mahsup mahsup = MahsupTablosunaEkle(hareket, ProjeConstants.IZINTIPI_UCRETLI_INT, eski.Id, yeni.Id, aciklama, transaction, context);
                eski.KullanilanIzin = (eski.KullanilanIzin.ConvertToInt() - sure).ToString();
                eski.KalanIzin = (eski.KalanIzin.ConvertToInt() + sure).ToString();
                eski.Aciklama += Environment.NewLine + "# Tarih=(" + DateTime.Now + ") Mahsup, IzinHareketId=" + hareket.Id + " YeniIzinDonemiId=" + yeni.Id;
                yeni.KullanilanIzin = (yeni.KullanilanIzin.ConvertToInt() + sure).ToString();
                yeni.KalanIzin = (yeni.KalanIzin.ConvertToInt() - sure).ToString();
                yeni.Aciklama += Environment.NewLine + "# Tarih=(" + DateTime.Now + ") Mahsup, IzinHareketId=" + hareket.Id + " OncekiIzinDonemiId=" + eski.Id;
                if (!donemService.Update(eski, transaction, context) || !donemService.Update(yeni, transaction, context))
                    throw new InvalidOperationException("Mahsup dönem bakiyeleri güncellenemedi.");
                hareket.Aciklama += "# Tarih=(" + DateTime.Now + ") MahsupId=" + mahsup.Id + "; " + AciklamaTxt.Text;
                hareket.Mahsup = true;
                hareket.IzinDonemId = yeni.Id;
                hareket.Degistiren = CurrentUserName;
                if (!hareketService.Update(hareket, transaction, context))
                    throw new InvalidOperationException("Mahsup hareketi güncellenemedi.");
                transaction.Complete();
                return true;
            }
        }
        private void TabloOlustur()
        {
            var jsonData = GetDataJson();//TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date

                jQuery('#CustomDataTable').DataTable({
                    'initComplete': function (settings, json) {//tablo yüklendiginde
                        var api = this.api();
                        var row = api.row(function(idx, data, node) { //secilen toplantiya gider
                            return data['Secildi'] == true;
                        });
                        if (row.length > 0)
                        {
                            row.select()
                                .show()
                                .draw(false);
                        }
                    },
                    columnDefs:[
                            {targets:2, render:function(data){
                              return moment(data).format('DD.MM.YYYY');
                            }},
                            {targets:3, render:function(data){
                              return moment(data).format('DD.MM.YYYY');
                            }},
                            {targets:5, render:function(data){
                                var link= '<a href=# onclick=OpenModal('+data+'); class=\'btn btn-outline-danger \'>Mahsup Islemi</a>';
                              return link;
                            }}],
                    data: " + jsonData + @",
                    columns: [
                        { data: 'AdiSoyadi' },
                        { data: 'IzinDonemi' },
                        { data: 'BaslangicTarihi'},
                        { data: 'BitisTarihi' },
                        { data: 'SureBirim' },
                        { data: 'IzinHareketId' },
                    ],
                    'order': [[2, 'desc']],//sort date desc
                    'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    dom: 'frtip',
                });

            ";

            return tableString;
        }
    }
}
