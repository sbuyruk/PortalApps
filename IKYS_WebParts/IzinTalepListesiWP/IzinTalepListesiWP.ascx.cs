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

namespace IKYS_WebParts.IzinTalepListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class IzinTalepListesiWP : WebPart
    {
        private PortalLogContext LoggingContext { get { return PortalLogContext.FromCurrentRequest("IKYS", "IzinTalepListesiWP"); } }
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
        public IzinTalepListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
                    new IzinTalepService().TryRecordWebPartOpened(LoggingContext);
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ReportError("IzinTalepListesi.PageLoad", ex);
            }
        }
        #region Liste Olusturma
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<IzinTalepListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
                jSon = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ReportError("IzinTalepListesi.Data", exception);
            }
            return jSon;
        }
        private List<IzinTalepListItem> GetDataList()
        {

            DataTable dataTable = GetDataTable();

            List<IzinTalepListItem> list = new List<IzinTalepListItem>();

            foreach (DataRow row in dataTable.Rows)
            {
                int personelId = row["PersonelId"].ConvertToInt();
                int izinTalepId = row["IzinTalepId"].ConvertToInt();
                int izinTipiId = row["IzinTipiId"].ConvertToInt();
                int onayDurumuId = row["OnayDurumuId"].ConvertToInt();
                string adiSoyadi = row["AdiSoyadi"].ToString();
                string izinTipi = row["IzinTipi"].ToString();
                DateTime baslangicTarihi = row["BaslangicTarihi"].ConvertToDatetime();
                DateTime bitisTarihi = row["BitisTarihi"].ConvertToDatetime();
                string sure = row["Sure"].ToString();
                string birim = row["Birim"].ToString();
                string onayDurumu = row["OnayDurumu"].ToString();


                IzinTalepListItem izinTalepListItem = new IzinTalepListItem();
                izinTalepListItem.PersonelId = personelId.ToString();
                izinTalepListItem.IzinTalepId = izinTalepId.ToString();
                izinTalepListItem.IzinTipiId = izinTipiId.ToString();
                izinTalepListItem.OnayDurumuId = onayDurumuId.ToString();
                izinTalepListItem.AdiSoyadi = adiSoyadi;
                izinTalepListItem.IzinTipi = izinTipi;
                izinTalepListItem.BaslangicTarihiHidden = baslangicTarihi;
                izinTalepListItem.BaslangicTarihi = baslangicTarihi.ConvertToDatetimeEmptyIfNull();
                izinTalepListItem.BitisTarihi = bitisTarihi.ConvertToDatetimeEmptyIfNull();
                //if (izinTipiId == ProjeConstants.IZINTIPI_SUTIZNI_INT)
                //{
                //    izinTalepListItem.BaslangicTarihi = baslangicTarihi.ConvertToDatetimeEmptyIfNull() + "(" + baslangicTarihi.ToString("HH:mm") + " - " + bitisTarihi.ToString("HH:mm") + ")";
                //    izinTalepListItem.BitisTarihi = bitisTarihi.ConvertToDatetimeEmptyIfNull() + "(" + baslangicTarihi.ToString("HH:mm") + " - " + bitisTarihi.ToString("HH:mm") + ")";
                //}
                //else
                //{
                //    izinTalepListItem.BaslangicTarihi = baslangicTarihi.ConvertToDatetimeEmptyIfNull();
                //    izinTalepListItem.BitisTarihi = bitisTarihi.ConvertToDatetimeEmptyIfNull();
                //}

                izinTalepListItem.Sure = sure;
                izinTalepListItem.Birim = birim;
                izinTalepListItem.OnayDurumu = onayDurumu;
                izinTalepListItem.Secildi = SecilenIdQS.Equals(izinTalepListItem.IzinTalepId);

                if (onayDurumuId == ProjeConstants.ONAYDURUMU_ISLEM_INT || onayDurumuId == ProjeConstants.ONAYDURUMU_DILEKCE_INT)
                {
                    izinTalepListItem.Duzenle = "<a href=" + ProjeConstants.PAGE_IZINTALEP_GIRIS + "?DestinationApp=TD&IzinTalepId=" + izinTalepId + "&PersonelId=" + personelId + "&IzinTanimId=" + izinTipiId + " class='btn btn-outline-primary'>Düzenle</a>";
                }
                else
                {
                    izinTalepListItem.Duzenle = string.Empty;
                }

                if (onayDurumuId == ProjeConstants.ONAYDURUMU_ISLEM_INT || onayDurumuId == ProjeConstants.ONAYDURUMU_DILEKCE_INT)
                {
                    izinTalepListItem.KayitKontrolRed = "<a href=# onclick=OpenModal('kabul'," + izinTalepId + "," + personelId + "); class=\'btn btn-outline-info \'>Kontrol/Red</a>";
                }
                else if (onayDurumuId == ProjeConstants.ONAYDURUMU_KONTROL_INT)
                {
                    izinTalepListItem.KayitKontrolRed = "<a href=# onclick=OpenModal('onay'," + izinTalepId + "," + personelId + "); class=\'btn btn-outline-success \'>Kayıt/Red</a>";
                }
                else
                {
                    izinTalepListItem.KayitKontrolRed = string.Empty;
                }

                list.Add(izinTalepListItem);
            }
            return list;
        }
        private DataTable GetDataTable()
        {
            DataTable dataTable = new IzinTalepService().GetIzinTalepleriReturnDataTable(0, ProjeConstants.IZINTIPI_UCRETLI_INT, true, false, LoggingContext);
            return dataTable;
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
                    data: " + jsonData + @",
                    columns: [
                        { data: 'BaslangicTarihiHidden'},
                        { data: 'AdiSoyadi' },
                        { data: 'IzinTipi' },
                        { data: 'BaslangicTarihi'},
                        { data: 'BitisTarihi' },
                        { data: 'Sure' },
                        { data: 'Birim' },
                        { data: 'OnayDurumu' },
                        { data: 'Duzenle' },
                        { data: 'KayitKontrolRed' },
                    ],
                    'columnDefs': [
                        {
                            'targets': [0],
                            'visible': false,
                            'searchable': false
                        },
                    ],
                    'order': [[8, 'desc'],[0, 'desc']],//sort date desc
                    'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    dom: 'frtip',
                    'createdRow': function(row, data, dataIndex) {
                        if (data.OnayDurumu=='" + ProjeConstants.ONAYDURUMU_KAYIT+ @"')
                        {
                            $(row).addClass('kayitlara-islendi');

                        }else if (data.OnayDurumu=='" + ProjeConstants.ONAYDURUMU_RED + @"')
                        {
                            $(row).addClass('reddedildi');

                        }
                    },//set row color
                });

            ";

            return tableString;
        }
        #endregion

        private void FillIzinHareketleriTable(Personel personel)
        {
            DateTime today = DateTime.Today;
            DateTime threeMonthsLater = DateTime.Today.AddMonths(3);
            IsBilgileri ib = new IsBilgileriService().GetByPersonelId(personel.Id, LoggingContext);
            if (ib != null)
            {


                IzinDonem izinDonem = new IzinDonemService().GetByIzinTarihi(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, today, LoggingContext);
                if (izinDonem != null)
                {
                    DateTime izinDonemiSonu = izinDonem != null ? izinDonem.BitisTarihi : today.AddMonths(1);
                    //DataTable dataTable = izinHareket.SelectByIzinDonemiReturnDataTable(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, izinDonemiBasi, threeMonthsLater);//3 ay içinde yeni izin dönemi basliyor olabilir
                    DataTable dataTable = new IzinHareketService().GetByIzinDonemiReturnDataTable(izinDonem.Id, personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, LoggingContext);
                    int SiraNo = 1;
                    if (dataTable == null)
                    {
                        IzinHareketTable.Rows.Clear();
                        TableRow tr = new TableRow();
                        TableCell tc = new TableCell();
                        tc.Text = "Henüz izin kullanilmamis.";
                        tr.Controls.Add(tc);
                        IzinHareketTable.Controls.Add(tr);
                    }
                    else
                    {
                        IzinHareketTableHeaders();
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            string izinTipi = dataRow["IzinTipi"].ReturnEmptyIfNull().ToString();
                            DateTime bastar = dataRow["BaslangicTarihi"].ConvertToDatetime();
                            DateTime bittar = dataRow["BitisTarihi"].ConvertToDatetime();
                            string sure = dataRow["Sure"].ToString();
                            string birim = dataRow["Birim"].ToString();

                            TableRow row = new TableRow();

                            TableCell SiraNoCell = new TableCell();

                            SiraNoCell.Text = SiraNo++ + "";
                            row.Controls.Add(SiraNoCell);

                            TableCell IzinTipiCell = new TableCell();
                            IzinTipiCell.Text = izinTipi;
                            row.Controls.Add(IzinTipiCell);

                            TableCell BasTarCell = new TableCell();

                            BasTarCell.Text = bastar.ConvertToDatetimeEmptyIfNull();
                            row.Controls.Add(BasTarCell);

                            TableCell BitTarCell = new TableCell();

                            BitTarCell.Text = bittar.ConvertToDatetimeEmptyIfNull();
                            row.Controls.Add(BitTarCell);
                            TableCell SureCell = new TableCell();
                            SureCell.Text = sure + " " + birim;

                            row.Controls.Add(SureCell);

                            if (bastar > izinDonemiSonu) //gelecek izin dönemine aitse farkli renk yazdir
                            {
                                SiraNoCell.ForeColor = System.Drawing.Color.Red;
                                IzinTipiCell.ForeColor = System.Drawing.Color.Red;
                                BasTarCell.ForeColor = System.Drawing.Color.Red;
                                BitTarCell.ForeColor = System.Drawing.Color.Red;
                                BitTarCell.ForeColor = System.Drawing.Color.Red;
                                SureCell.ForeColor = System.Drawing.Color.Red;

                                row.ToolTip = "Yeni Izin Dönemi";
                            }

                            IzinHareketTable.Controls.Add(row);
                        }
                    }
                }

            }
            else
            {
            }
        }
        private void FillIzinBilgileriTable(Personel personel)
        {
            DateTime today = DateTime.Today;
            IsBilgileri ib = new IsBilgileriService().GetByPersonelId(personel.Id, LoggingContext);
            if (ib != null)
            {

                DateTime izinDonemiBasTar = ib.IzinDonemiBasTar;
                //int izinDonemiSayisi = (today.Year - izinDonemiBasTar.Year);
                //izinDonemiSayisi = izinDonemiSayisi > 5 ? 5 : izinDonemiSayisi;
                List<IzinDonem> izinDonemiList = new IzinDonemService().GetByPersonelId(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, LoggingContext);


                IzinBilgileriTableHeaders();
                foreach (IzinDonem izinDonemi in izinDonemiList)
                {
                    TableRow row = new TableRow();
                    TableCell IzinDonemiCell = new TableCell();
                    IzinDonemiCell.Text = izinDonemi.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + izinDonemi.BitisTarihi.ConvertToDatetimeEmptyIfNull();
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
        private void IzinHareketTableHeaders()
        {
            IzinHareketTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell izintipiCell = new TableHeaderCell();
            izintipiCell.Text = "İzin Tipi";
            TableHeaderCell bastarCell = new TableHeaderCell();
            bastarCell.Text = "Başlangıç tarihi";
            TableHeaderCell bittarCell = new TableHeaderCell();
            bittarCell.Text = "Bitiş tarihi";
            TableHeaderCell sureCell = new TableHeaderCell();
            sureCell.Text = "İzinli Süre";
            TableHeaderCell yazdirCell = new TableHeaderCell();
            yazdirCell.Text = "Yazdır";
            th.Controls.Add(siraCell);
            th.Controls.Add(izintipiCell);
            th.Controls.Add(bastarCell);
            th.Controls.Add(bittarCell);
            th.Controls.Add(sureCell);
            IzinHareketTable.Controls.Add(th);
        }
        private void IzinBilgileriTableHeaders()
        {
            IzinBilgileriTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell donemCell = new TableHeaderCell();
            donemCell.Text = "İzin Dönemi";
            TableHeaderCell hakCell = new TableHeaderCell();
            hakCell.Text = "İzin Hakkı";
            TableHeaderCell kullanilanCell = new TableHeaderCell();
            kullanilanCell.Text = "Kullanılan İzin";
            TableHeaderCell kalanCell = new TableHeaderCell();
            kalanCell.Text = "Kalan İzin";
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
                IzinTalep talep = TalebiIsle(ProjeConstants.PER_IZINTALEBI_KAYITLARAISLENDI_INT);
                if (talep == null) return;
                Messages.Success("İzin talebi kayıtlara işlendi.", 2000);
                BildirimGonder(talep, "Onay");
                TabloOlustur();
            }
            catch (Exception exception)
            {
                ReportError("LeaveRequest.Approve", exception);
            }
        }
        private void BildirimGonder(IzinTalep talep, string islem)
        {
            if (!talep.EPostaGonder) return;
            try
            {
                IKYSOrtak.IzinKabulRedOnayEPostasiGonder(talep.PersonelId, talep.Id, islem, LoggingContext);
            }
            catch (Exception error)
            {
                if (error is System.Threading.ThreadAbortException) throw;
                PortalLogContext context = LoggingContext;
                PortalTechnicalLog.WriteError(context, "LeaveRequest.Notification", error);
                Messages.Error("İşlem kaydedildi, ancak e-posta bildirimi gönderilemedi. Başvuru kodu: "
                    + context.OperationId.ToString("N"));
            }
        }
        private IzinTalep TalebiIsle(int hedefDurum)
        {
            PortalLogContext context = LoggingContext;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                IzinTalepService service = new IzinTalepService();
                IzinTalep talep = service.GetById(paramIzinTalepIdLbl.Value.ConvertToInt(), transaction, context);
                if (talep == null || talep.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                {
                    Messages.Error("İzin talebi bulunamadı. Listeyi yenileyin.");
                    return null;
                }
                bool ilkAsama = talep.OnayDurumu == ProjeConstants.PER_IZINTALEBI_ISLEMBEKLIYOR
                    || talep.OnayDurumu == ProjeConstants.PER_IZINTALEBI_DILEKCEBEKLIYOR;
                bool kontrolEdildi = talep.OnayDurumu == ProjeConstants.PER_IZINTALEBI_KONTROLEDILDI_INT;
                bool onayla = hedefDurum == ProjeConstants.PER_IZINTALEBI_KAYITLARAISLENDI_INT;
                bool reddet = hedefDurum == ProjeConstants.PER_IZINTALEBI_REDDEDILDI;
                if (!(onayla ? kontrolEdildi : reddet ? (ilkAsama || kontrolEdildi) : ilkAsama)
                    || new IzinHareketService().GetByIzinTalepId(talep.Id, transaction, context) != null)
                {
                    Messages.Error("Talebin durumu değişmiş veya talep kayıtlara işlenmiş. Listeyi yenileyin.");
                    return null;
                }
                if (!string.IsNullOrEmpty(AciklamaTxt.Text)) talep.Aciklama = AciklamaTxt.Text;
                talep.OnayDurumu = hedefDurum;
                talep.Degistiren = CurrentUserName;
                if (!service.Update(talep, transaction, context))
                    throw new InvalidOperationException("İzin talebi güncellenemedi.");
                if (onayla) TalepHareketiniKaydet(talep, transaction, context);
                transaction.Complete();
                return talep;
            }
        }

        private void TalepHareketiniKaydet(IzinTalep talep, SqlTransactionContext transaction, PortalLogContext context)
        {
            IzinHareket hareket = new IzinHareket();
            hareket.PersonelId = talep.PersonelId;
            hareket.IzinTipi = talep.IzinTipi;
            hareket.IzinTalepId = talep.Id;
            hareket.BaslangicTarihi = talep.BaslangicTarihi;
            hareket.BitisTarihi = talep.BitisTarihi;
            hareket.Adres = talep.Adres;
            hareket.VekilImza = talep.VekilImza;
            hareket.AmirImza = talep.AmirImza;
            hareket.OnayImza = talep.OnayImza;
            hareket.Olusturan = CurrentUserName;
            hareket.IzinDonemId = talep.IzinDonemId;
            hareket.Sure = talep.Sure;
            hareket.Birim = talep.Birim;
            if (hareket.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT)
            {
                IzinDonemService service = new IzinDonemService();
                IzinDonem donem = service.GetById(hareket.IzinDonemId, transaction, context);
                if (donem == null || donem.PersonelId != talep.PersonelId || donem.IzinTipi != talep.IzinTipi)
                    throw new InvalidOperationException("Talebe ait izin dönemi bulunamadı.");
                hareket.OncekiIzinStr = donem.KalanIzin;
                hareket.KullanilanIzinStr = talep.Sure;
                if (!service.UpdateUsedLeave(donem, hareket.Sure, true, CurrentUserName, transaction, context))
                    throw new InvalidOperationException("Kullanılan izin güncellenemedi.");
                hareket.KalanIzinStr = donem.KalanIzin;
            }
            if (hareket.IzinTipi == ProjeConstants.IZINTIPI_UCRETSIZ_INT)
            {
                Personel personel = new PersonelService().GetById(hareket.PersonelId, transaction, context);
                if (personel == null) throw new InvalidOperationException("Personel bulunamadı.");
                IsBilgileriService service = new IsBilgileriService();
                IsBilgileri bilgi = service.GetByPersonelId(hareket.PersonelId, transaction, context);
                string baslama = bilgi == null ? "" : bilgi.BaslamaTar.ConvertToDatetimeEmptyIfNull();
                string donemBaslangici = bilgi == null ? "" : bilgi.IzinDonemiBasTar.ConvertToDatetimeEmptyIfNull();
                donemBaslangici = string.IsNullOrEmpty(donemBaslangici) ? baslama : donemBaslangici;
                if (string.IsNullOrEmpty(donemBaslangici))
                    throw new InvalidOperationException("İzin dönemi başlangıç tarihi belirlenemedi.");
                if (personel.Asker_sivil != ProjeConstants.PER_ASKER_INT)
                {
                    int gunSayisi = hareket.BitisTarihi.Subtract(hareket.BaslangicTarihi).Days + 1;
                    bilgi.IzinDonemiBasTar = donemBaslangici.ConvertToDatetime().AddDays(gunSayisi);
                    if (!service.Update(bilgi, transaction, context))
                        throw new InvalidOperationException("Ücretsiz izin için dönem başlangıcı güncellenemedi.");
                }
            }
            if (new IzinHareketService().Save(hareket, transaction, context) <= 0)
                throw new InvalidOperationException("İzin hareketi oluşturulamadı.");
        }
        protected void KabulBtn_Click(object sender, EventArgs e)
        {
            try
            {
                IzinTalep talep = TalebiIsle(ProjeConstants.PER_IZINTALEBI_KONTROLEDILDI_INT);
                if (talep == null) return;
                Messages.Success("İzin talebi kontrol edildi.", 2000);
                BildirimGonder(talep, "Kabul");
                TabloOlustur();
            }
            catch (Exception exception)
            {
                ReportError("LeaveRequest.Accept", exception);
            }
        }
        protected void ReddetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                IzinTalep talep = TalebiIsle(ProjeConstants.PER_IZINTALEBI_REDDEDILDI);
                if (talep == null) return;
                Messages.Success("İzin talebi reddedildi.", 2000);
                BildirimGonder(talep, "Red");
                TabloOlustur();
            }
            catch (Exception exception)
            {
                ReportError("LeaveRequest.Reject", exception);
            }
        }

        protected void YazdirBtn_Click(object sender, EventArgs e)
        {
            //Izin kagidini yazdir
        }
        //protected void Timer1_Tick(object sender, EventArgs e)
        //{
        //    KayitGetir();
        //}

        protected void IzinHareketleriBtn_Click(object sender, EventArgs e)
        {
            try
            {
                int personelId = paramPersonelIdLbl.Value.ConvertToInt();
                Personel personel = new PersonelService().GetById(personelId, LoggingContext);
                if (personel != null)
                {
                    PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                    FillIzinTalebi(personel);
                    FillIzinHareketleriTable(personel);
                    FillIzinBilgileriTable(personel);
                    if (paramOnayIdLbl.Value.Equals("kabul"))
                    {
                        OnaylaModalBtn.Visible = false;
                        KabuletModalBtn.Visible = true;
                    }
                    else if (paramOnayIdLbl.Value.Equals("onay"))
                    {
                        OnaylaModalBtn.Visible = true;
                        KabuletModalBtn.Visible = false;
                    }
                }
            }
            catch (Exception error)
            {
                ReportError("LeaveRequest.Details", error);
            }
        }

        private void FillIzinTalebi(Personel personel)
        {
            IzinTalep izinTalebi = new IzinTalepService().GetById(paramIzinTalepIdLbl.Value.ConvertToInt(), LoggingContext);
            if (izinTalebi == null) throw new InvalidOperationException("İzin talebi bulunamadı. Listeyi yenileyin.");
            if (izinTalebi != null)
            {
                IzinTanim izinTanim = new IzinTanimService().GetById(izinTalebi.IzinTipi, LoggingContext);
                string izinTanimStr = izinTanim == null ? "" : izinTanim.Adi;
                AciklamaTxt.Text = izinTalebi.Aciklama;
                AdresLbl.Text = "Izin Adresi : " + izinTalebi.Adres;
                IzinTalebiLbl.Text = personel.Adi + " " + personel.Soyadi + " Tarafindan " + izinTalebi.BaslangicTarihi.ToString("dd.MM.yyyy") + " - " +
                    izinTalebi.BitisTarihi.ToString("dd.MM.yyyy") + " Tarihleri Arasinda " + izinTalebi.Sure + " " + izinTalebi.Birim
                    + " " + izinTanimStr + " Izin Talep Edilmektedir.";
            }

        }
        private class IzinTalepListItem
        {

            public DateTime BaslangicTarihiHidden { get; set; }
            public string PersonelId { get; set; }
            public string IzinTalepId { get; set; }
            public string IzinTipiId { get; set; }
            public string OnayDurumuId { get; set; }
            public string AdiSoyadi { get; set; }
            public string IzinTipi { get; set; }
            public string BaslangicTarihi { get; set; }
            public string BitisTarihi { get; set; }
            public string Sure { get; set; }
            public string Birim { get; set; }
            public string OnayDurumu { get; set; }
            public string Duzenle { get; set; }
            public string KayitKontrolRed { get; set; }
            public bool Secildi { get; set; }
        }
    }
}
