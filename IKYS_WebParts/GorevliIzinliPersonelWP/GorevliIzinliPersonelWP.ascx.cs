using Model.IKYS;
using Model.Ortak;
using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.GorevliIzinliPersonelWP
{
    [ToolboxItemAttribute(false)]
    public partial class GorevliIzinliPersonelWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public GorevliIzinliPersonelWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                DateTime today = DateTime.Today;
                TitleLbl.Text = "Görevli/Izinli Personel Listesi";
                GorevOnayliOlanPersonelTablosunuDoldur();
                UcretliIzinliPersonelTablosunuDoldur();
                RaporluGorevliHastanedePersonelTablosunuDoldur();
                if (AuthQS.Equals("IKYS"))
                {
                    MazeretIzinliPersonelTablosunuDoldur();
                    ExcelDiv.Attributes["style"] = "display:block";
                    MazeretDiv.Attributes["style"] = "max-height: 400px; overflow: auto; display: block";
                }
                else
                {
                    ExcelDiv.Attributes["style"] = "display:none";
                    MazeretDiv.Attributes["style"] = "max-height: 400px; overflow: auto; display: none";
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exHelper = new ExceptionHelper(exception);
                exHelper.PublishException();
            }
        }
        private void GorevOnayliOlanPerTableHeaders()
        {
            GorevOnayTable.Rows.Clear();
            TableHeaderRow baslikHR = new TableHeaderRow();
            TableHeaderCell baslikHRCell = new TableHeaderCell();

            baslikHRCell.Text = " Görevli/Izinli Personel Listesi - " + DateTime.Now.ConvertToDDMMYYYHHmmFormat();
            baslikHRCell.Font.Bold = true;
            baslikHRCell.ColumnSpan = 7;
            baslikHR.CssClass = "text-center";
            baslikHRCell.BackColor = System.Drawing.Color.LightGray;
            baslikHR.Controls.Add(baslikHRCell);
            GorevOnayTable.Controls.Add(baslikHR);


            TableHeaderRow thbaslik = new TableHeaderRow();
            TableHeaderCell baslikCell = new TableHeaderCell();

            baslikCell.Text = "Dis Görevde Olanlar";
            baslikCell.Font.Bold = true;
            baslikCell.ColumnSpan = 7;
            thbaslik.CssClass = "alert-secondary text-center";
            thbaslik.Controls.Add(baslikCell);
            GorevOnayTable.Controls.Add(thbaslik);

            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sira";
            TableHeaderCell AdiSoyadiCell = new TableHeaderCell();
            AdiSoyadiCell.Text = "Adi Soyadi";
            TableHeaderCell GorevinSebebiCell = new TableHeaderCell();
            GorevinSebebiCell.Text = "Görevin Sebebi";
            TableHeaderCell GidilecekYerCell = new TableHeaderCell();
            GidilecekYerCell.Text = "Gidilecek Yer";
            TableHeaderCell BaslangicTarCell = new TableHeaderCell();
            BaslangicTarCell.Text = "Baslangiç Tarihi";
            TableHeaderCell BitisTarCell = new TableHeaderCell();
            BitisTarCell.Text = "Bitis Tarihi";
            TableHeaderCell GorevYeriCell = new TableHeaderCell();
            GorevYeriCell.Text = "Görevi";

            th.Controls.Add(siraCell);
            th.Controls.Add(AdiSoyadiCell);
            th.Controls.Add(GorevinSebebiCell);
            th.Controls.Add(GidilecekYerCell);
            th.Controls.Add(BaslangicTarCell);
            th.Controls.Add(BitisTarCell);
            th.Controls.Add(GorevYeriCell);

            GorevOnayTable.Controls.Add(th);
        }
        private void GorevOnayliOlanPersonelTablosunuDoldur()
        {
            GorevOnay gorevOnay = new GorevOnay();
            DateTime today = DateTime.Today;
            DateTime todaybas = UtilityHelper.TariheSaatEkle(today, "23:59");
            DateTime todaybit = UtilityHelper.TariheSaatEkle(today, "06:00");

            DataTable dataTable = gorevOnay.SelectByTarihReturnDataTable(todaybas, todaybit);
            if (dataTable == null)
            {
                GorevOnayTable.Rows.Clear();
                TableHeaderRow baslikHR = new TableHeaderRow();
                TableHeaderCell baslikHRCell = new TableHeaderCell();
                baslikHRCell.Text = " Görevli/Izinli Personel Listesi - " + DateTime.Now.ConvertToDDMMYYYHHmmFormat();
                baslikHRCell.Font.Bold = true;
                baslikHRCell.ColumnSpan = 7;
                baslikHR.CssClass = "text-center";
                baslikHRCell.BackColor = System.Drawing.Color.LightGray;
                baslikHR.Controls.Add(baslikHRCell);
                GorevOnayTable.Controls.Add(baslikHR);

                TableHeaderRow thbaslik = new TableHeaderRow();
                TableHeaderCell baslikCell = new TableHeaderCell();

                baslikCell.Text = "Dis Görevde Olanlar";
                baslikCell.Font.Bold = true;
                baslikCell.ColumnSpan = 7;
                thbaslik.CssClass = "alert-secondary text-center";
                thbaslik.Controls.Add(baslikCell);
                GorevOnayTable.Controls.Add(thbaslik);

                TableRow tr = new TableRow();
                TableCell tc = new TableCell();
                tc.ColumnSpan = 7;
                tc.Text = "Dis görevde olan personel bulunmamaktadir.";
                tr.Controls.Add(tc);
                GorevOnayTable.Controls.Add(tr);
            }
            else
            {
                GorevOnayliOlanPerTableHeaders();
                int SiraNo = 1;
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    int personelId = dataRow["PersonelId"].ConvertToInt();
                    string gidilecekYer = dataRow["GidilecekYer"].ReturnEmptyIfNull().ToString();
                    string adiSoyadi = dataRow["AdiSoyadi"].ReturnEmptyIfNull().ToString();
                    string bastar = dataRow["BaslangicTarihi"].ConvertToDDMMYYYHHmmFormat();
                    DateTime bitTarDate = dataRow["BitisTarihi"].ConvertToDatetime();
                    string bittar = dataRow["BitisTarihi"].ConvertToDDMMYYYHHmmFormat();
                    string gorevYeri = dataRow["GorevYeri"].ReturnEmptyIfNull().ToString();
                    bool vekil = dataRow["Vekil"].ReturnFalseIfNull().ConvertToBool();
                    string gorevinSebebi = dataRow["GorevinSebebi"].ReturnEmptyIfNull().ToString();

                    TableRow row = new TableRow();

                    TableCell SiraCell = new TableCell();
                    SiraCell.Text = SiraNo++ + "";
                    row.Controls.Add(SiraCell);

                    TableCell AdiSoyadiCell = new TableCell();
                    AdiSoyadiCell.Text = adiSoyadi;
                    row.Controls.Add(AdiSoyadiCell);

                    TableCell GorevinSebebiCell = new TableCell();
                    GorevinSebebiCell.Text = gorevinSebebi;
                    row.Controls.Add(GorevinSebebiCell);

                    TableCell GidilecekYerCell = new TableCell();
                    GidilecekYerCell.Text = gidilecekYer;
                    row.Controls.Add(GidilecekYerCell);

                    TableCell BastarCell = new TableCell();
                    BastarCell.Text = bastar;
                    row.Controls.Add(BastarCell);

                    TableCell BittarCell = new TableCell();
                    string izinDevami = BitisTarihindeBaslayanIzniVarMi(personelId, bitTarDate);
                    BittarCell.Text = bittar + (string.IsNullOrEmpty(izinDevami) ? string.Empty : "</br>" + izinDevami); 

                    string gorevDevami = BitisTarihindeBaslayanGorevOnayiVarMi(personelId, bitTarDate);
                    BittarCell.Text = BittarCell.Text + (string.IsNullOrEmpty(gorevDevami) ? string.Empty : "</br>" + gorevDevami);
                    row.Controls.Add(BittarCell);

                    TableCell GorevYeriCell = new TableCell();
                    GorevYeriCell.Text = gorevYeri + (vekil ? "V." : "");
                    row.Controls.Add(GorevYeriCell);

                    if (bitTarDate <= DateTime.Now )
                    {
                        SiraCell.ForeColor = System.Drawing.Color.Gray;
                        AdiSoyadiCell.ForeColor = System.Drawing.Color.Gray;
                        GidilecekYerCell.ForeColor = System.Drawing.Color.Gray;
                        GorevinSebebiCell.ForeColor = System.Drawing.Color.Gray;
                        BastarCell.ForeColor = System.Drawing.Color.Gray;
                        BittarCell.ForeColor = System.Drawing.Color.Gray;
                        GorevYeriCell.ForeColor = System.Drawing.Color.Gray;
                        BittarCell.Text += "(Bitti)";
                    }
                    GorevOnayTable.Controls.Add(row);
                }
            }
        }
        private void RaporluPerTableHeaders()
        {
            RaporluPerTable.Rows.Clear();

            TableHeaderRow thbaslik = new TableHeaderRow();
            TableHeaderCell baslikCell = new TableHeaderCell();

            baslikCell.Text = "Raporlu/Hastanede/Sehir Içi Görevde Olanlar";
            baslikCell.Font.Bold = true;
            baslikCell.ColumnSpan = 7;
            thbaslik.CssClass = "alert-secondary text-center";
            thbaslik.Controls.Add(baslikCell);
            RaporluPerTable.Controls.Add(thbaslik);

            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sira";
            TableHeaderCell adiSoyadiCell = new TableHeaderCell();
            adiSoyadiCell.Text = "Adi Soyadi";
            TableHeaderCell BulunmamaSebebiCell = new TableHeaderCell();
            BulunmamaSebebiCell.Text = "Bulunmama Sebebi";
            TableHeaderCell BaslangicTarCell = new TableHeaderCell();
            BaslangicTarCell.Text = "Baslangiç Tarihi";
            TableHeaderCell BitisTarCell = new TableHeaderCell();
            BitisTarCell.Text = "Bitis Tarihi";
            TableHeaderCell SureCell = new TableHeaderCell();
            SureCell.Text = "Süre";

            TableHeaderCell AciklamaCell = new TableHeaderCell();
            AciklamaCell.Text = "Açiklama";

            th.Controls.Add(siraCell);
            th.Controls.Add(adiSoyadiCell);
            th.Controls.Add(BulunmamaSebebiCell);
            th.Controls.Add(BaslangicTarCell);
            th.Controls.Add(BitisTarCell);

            th.Controls.Add(SureCell);
            th.Controls.Add(AciklamaCell);
            RaporluPerTable.Controls.Add(th);
        }
        private void RaporluGorevliHastanedePersonelTablosunuDoldur()
        {
            Yoklama yoklama = new Yoklama();
            DateTime today = DateTime.Today;
            DateTime todaybas = UtilityHelper.TariheSaatEkle(today, "23:59");
            DateTime todaybit = UtilityHelper.TariheSaatEkle(today, "06:00");
            DataTable dataTable = yoklama.SelectByTarihReturnDataTable(todaybas, todaybit);
            if (dataTable == null)
            {
                RaporluPerTable.Rows.Clear();

                TableHeaderRow thbaslik = new TableHeaderRow();
                TableHeaderCell baslikCell = new TableHeaderCell();

                baslikCell.Text = "Raporlu/Hastanede/Sehir Içi Görevde Olanlar";
                baslikCell.Font.Bold = true;
                baslikCell.ColumnSpan = 7;
                thbaslik.CssClass = "alert-secondary text-center";
                thbaslik.Controls.Add(baslikCell);
                RaporluPerTable.Controls.Add(thbaslik);

                TableRow tr = new TableRow();
                TableCell tc = new TableCell();
                tc.ColumnSpan = 7;
                tc.Text = "Raporlu/Hastanede/Görevli olan personel bulunmamaktadir.";
                tr.Controls.Add(tc);
                RaporluPerTable.Controls.Add(tr);
            }
            else
            {
                RaporluPerTableHeaders();
                int SiraNo = 1;
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    int bulunmamaSebebiInt = dataRow["BulunmamaSebebiInt"].ReturnZeroIfNull().ConvertToInt();
                    int personelId = dataRow["PersonelId"].ReturnZeroIfNull().ConvertToInt();
                    string bulunmamaSebebi = dataRow["BulunmamaSebebi"].ReturnEmptyIfNull().ToString();
                    string adiSoyadi = dataRow["AdiSoyadi"].ReturnEmptyIfNull().ToString();
                    string bastar = dataRow["BaslangicTarihi"].ConvertToDatetimeEmptyIfNull();
                    string bittar = dataRow["BitisTarihi"].ConvertToDatetimeEmptyIfNull();
                    DateTime bitTarDate = dataRow["BitisTarihi"].ConvertToDatetime();
                    string aciklama = dataRow["Aciklama"].ReturnEmptyIfNull().ToString();
                    string gorevYeri = dataRow["GorevYeri"].ReturnEmptyIfNull().ToString();

                    TableRow row = new TableRow();

                    TableCell SiraCell = new TableCell();
                    SiraCell.Text = SiraNo++ + "";
                    row.Controls.Add(SiraCell);

                    TableCell AdiSoyadiCell = new TableCell();
                    AdiSoyadiCell.Text = adiSoyadi;
                    row.Controls.Add(AdiSoyadiCell);

                    TableCell BulunmamaSebebiCell = new TableCell();
                    BulunmamaSebebiCell.Text = bulunmamaSebebi;
                    row.Controls.Add(BulunmamaSebebiCell);

                    TableCell BastarCell = new TableCell();
                    BastarCell.Text = bastar;
                    row.Controls.Add(BastarCell);

                    TableCell BittarCell = new TableCell();
                    

                    TableCell SureCell = new TableCell();
                    DateTime bas = bastar.ConvertToDatetime();
                    DateTime bit = bittar.ConvertToDatetime();
                    BittarCell.Text = bittar;
                    if (bulunmamaSebebiInt == ProjeConstants.BULUNMAMASEBEBI_GOREVLI_INT
                        && aciklama.Contains(ProjeConstants.BULUNMAMASEBEBI_GOREVLI_UZAKTANCALISMA))
                    {
                        DateTime bittarx = dataRow["BitisTarihi"].ConvertToDatetime();
                        BittarCell.Text = bittarx.ToString("dd.MM.yyyy HH:mm");

                        DateTime bastarx = dataRow["BaslangicTarihi"].ConvertToDatetime();
                        BastarCell.Text = bastarx.ToString("dd.MM.yyyy HH:mm");
                        //SureCell.Text = (bittarx - bastarx).ConvertToTimeSpanReturnInHHmm();
                        int gun = (bit - bas).Days;

                        int saat = (bittarx - bastarx).Hours;//artan saat var mi
                        if (saat >= 9)
                        {
                            gun++;
                            saat = 0;
                        }
                        string gunStr = gun > 0 ? gun + " Gün " : "";
                        string saatstr = saat > 0 ? saat + " Saat" : "";
                        SureCell.Text = gunStr + saatstr;
                    }
                    else if (bulunmamaSebebiInt == ProjeConstants.BULUNMAMASEBEBI_GOREVLI_INT
                        || bulunmamaSebebiInt == ProjeConstants.BULUNMAMASEBEBI_HASTANE_INT)
                    {
                        SureCell.Text = (bit - bas).Hours + " Saat";
                        DateTime bittarx = dataRow["BitisTarihi"].ConvertToDatetime();
                        BittarCell.Text = bittarx.ToString("dd.MM.yyyy HH:mm");

                        DateTime bastarx = dataRow["BaslangicTarihi"].ConvertToDatetime();
                        BastarCell.Text = bastarx.ToString("dd.MM.yyyy HH:mm");
                        SureCell.Text = (bittarx - bastarx).ConvertToTimeSpanReturnInHHmm();
                    }
                    else
                    {
                        SureCell.Text = (bit - bas).TotalDays + 1 + " Gün";

                    }
                    string izinDevami = BitisTarihindeBaslayanIzniVarMi(personelId, bitTarDate);
                    BittarCell.Text = BittarCell.Text + (string.IsNullOrEmpty(izinDevami) ? string.Empty : "</br>" + izinDevami);
                    string gorevDevami = BitisTarihindeBaslayanGorevOnayiVarMi(personelId, bitTarDate);
                    BittarCell.Text = BittarCell.Text + (string.IsNullOrEmpty(gorevDevami)?string.Empty:"</br>" + gorevDevami);
                    row.Controls.Add(BittarCell);
                    row.Controls.Add(SureCell);

                    TableCell AciklamaCell = new TableCell();
                    AciklamaCell.Text = aciklama;
                    row.Controls.Add(AciklamaCell);

                    RaporluPerTable.Controls.Add(row);
                }
            }
        }
        private void IzinliPerTableHeaders(Table table, string IzinTipi)
        {
            table.Rows.Clear();

            TableHeaderRow thbaslik = new TableHeaderRow();
            TableHeaderCell baslikCell = new TableHeaderCell();

            baslikCell.Text = IzinTipi.Equals(ProjeConstants.IZINTIPI_MAZERET)? ProjeConstants.IZINTIPI_MAZERET+ " İzinli Olanlar": "İzinli Olanlar (Mazeret Hariç)";
            baslikCell.Font.Bold = true;
            baslikCell.ColumnSpan = 7;
            thbaslik.CssClass = "alert-secondary text-center";
            thbaslik.Controls.Add(baslikCell);
            table.Controls.Add(thbaslik);

            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sira";
            TableHeaderCell adiSoyadiCell = new TableHeaderCell();
            adiSoyadiCell.Text = "Adi Soyadi";
            TableHeaderCell IzinTipiCell = new TableHeaderCell();
            IzinTipiCell.Text = "Izin Tipi";
            TableHeaderCell BaslangicTarCell = new TableHeaderCell();
            BaslangicTarCell.Text = "Baslangiç Tarihi";
            TableHeaderCell BitisTarCell = new TableHeaderCell();
            BitisTarCell.Text = "Bitis Tarihi";
            TableHeaderCell SureCell = new TableHeaderCell();
            SureCell.Text = "Süre";
            TableHeaderCell GorevYeriCell = new TableHeaderCell();
            GorevYeriCell.Text = "Görev Yeri";

            th.Controls.Add(siraCell);
            th.Controls.Add(adiSoyadiCell);
            th.Controls.Add(IzinTipiCell);
            th.Controls.Add(BaslangicTarCell);
            th.Controls.Add(BitisTarCell);
            th.Controls.Add(SureCell);
            th.Controls.Add(GorevYeriCell);

            table.Controls.Add(th);
        }
        private void UcretliIzinliPersonelTablosunuDoldur()
        {
            IzinHareket ih = new IzinHareket();
            DateTime today = DateTime.Today;

            DateTime todaybas = UtilityHelper.TariheSaatEkle(today, "17:00");
            DateTime todaybit = UtilityHelper.TariheSaatEkle(today, "07:00");

            DataTable dataTable = ih.SelectByTarihReturnDataTable(todaybas, todaybit);

            bool izinTableBos = true;
            if (dataTable != null)
            {
                IzinliPerTableHeaders(IzinliPerTable,ProjeConstants.IZINTIPI_UCRETLI);
                int SiraNo = 1;

                foreach (DataRow dataRow in dataTable.Rows)
                {
                    int izinTipiId = dataRow["IzinTipi"].ConvertToInt();
                    int personelId = dataRow["PersonelId"].ConvertToInt();
                    string sure = dataRow["Sure"].ReturnEmptyIfNull().ToString();
                    string izinTipi = dataRow["IzinTanim"].ReturnEmptyIfNull().ToString();
                    string adiSoyadi = dataRow["AdiSoyadi"].ReturnEmptyIfNull().ToString();

                    DateTime bastarDate = dataRow["BaslangicTarihi"].ConvertToDatetime();
                    DateTime bitTarDate = dataRow["BitisTarihi"].ConvertToDatetime();

                    string bastar = dataRow["BaslangicTarihi"].ConvertToDatetimeEmptyIfNull();
                    string bittar = dataRow["BitisTarihi"].ConvertToDatetimeEmptyIfNull();
                    string birim = dataRow["Birim"].ReturnEmptyIfNull().ToString();
                    string gorevYeri = dataRow["GorevYeri"].ReturnEmptyIfNull().ToString();
                    if (izinTipiId == ProjeConstants.IZINTIPI_MAZERET_INT)
                    {
                        TimeSpan sureTS = sure.ConvertToTimeSpan();
                        int sureInt = sureTS.Hours;
                        if (sureInt < ProjeConstants.MESAI_GUNLUKSURE_SAAT)
                        {
                            continue;
                        }
                        else
                        {
                            sure = "1";
                            birim = ProjeConstants.IZIN_BIRIMI_GUN;
                        }
                    }
                    izinTableBos = false;
                    TableRow row = new TableRow();

                    TableCell SiraCell = new TableCell();
                    SiraCell.Text = SiraNo++ + "";
                    row.Controls.Add(SiraCell);

                    TableCell AdiSoyadiCell = new TableCell();
                    AdiSoyadiCell.Text = adiSoyadi;
                    row.Controls.Add(AdiSoyadiCell);

                    TableCell IzinTipiCell = new TableCell();
                    IzinTipiCell.Text = izinTipi;
                    row.Controls.Add(IzinTipiCell);

                    TableCell BastarCell = new TableCell();
                    BastarCell.Text = bastar;
                    row.Controls.Add(BastarCell);

                    TableCell BittarCell = new TableCell();
                    
                    string izinDevami = BitisTarihindeBaslayanIzniVarMi(personelId, bitTarDate);
                    BittarCell.Text = bittar +(string.IsNullOrEmpty(izinDevami)?string.Empty:"</br>" +izinDevami);
                    string gorevDevami = BitisTarihindeBaslayanGorevOnayiVarMi(personelId, bitTarDate);
                    BittarCell.Text = BittarCell.Text + (string.IsNullOrEmpty(gorevDevami) ? string.Empty : "</br>" + gorevDevami);
                    row.Controls.Add(BittarCell);
                    if (izinTipi.Equals(ProjeConstants.IZINTIPI_SUTIZNI))
                    {
                        BastarCell.Text = bastar + " </br>İzin saati : " + bastarDate.ToString("HH:mm")+"-"+ bitTarDate.ToString("HH:mm") ;
                        BittarCell.Text = bittar + " </br>İzin saati : " + bastarDate.ToString("HH:mm") + "-" + bitTarDate.ToString("HH:mm");
                    }
                    TableCell SureCell = new TableCell();
                    SureCell.Text = sure + " " + birim;
                    row.Controls.Add(SureCell);

                    TableCell GorevYeriCell = new TableCell();
                    GorevYeriCell.Text = gorevYeri;
                    row.Controls.Add(GorevYeriCell);

                    IzinliPerTable.Controls.Add(row);
                }

            }
            if (izinTableBos)
            {
                IzinliPerTable.Rows.Clear();
                TableHeaderRow thbaslik = new TableHeaderRow();
                TableHeaderCell baslikCell = new TableHeaderCell();

                baslikCell.Text = "İzinli Olanlar (Mazeret Hariç)";
                baslikCell.Font.Bold = true;
                baslikCell.ColumnSpan = 7;
                thbaslik.CssClass = "alert-secondary text-center";
                thbaslik.Controls.Add(baslikCell);
                IzinliPerTable.Controls.Add(thbaslik);

                TableRow tr = new TableRow();
                TableCell tc = new TableCell();
                tc.ColumnSpan = 7;
                tc.Text = "İzinli personel bulunmamaktadır.";
                tr.Controls.Add(tc);
                IzinliPerTable.Controls.Add(tr);
            }
        }

        private string BitisTarihindeBaslayanIzniVarMi(int personelId,  DateTime bitTarDate)
        {
            IzinHareket ih = new IzinHareket();
            DateTime tarih = new DateTime(bitTarDate.Year, bitTarDate.Month, bitTarDate.Day);
            DateTime bastar = UtilityHelper.TariheSaatEkle(tarih.AddDays(1), "17:00");
            DateTime bittar = UtilityHelper.TariheSaatEkle(tarih.AddDays(1), "07:00");

            //iznin son günü cuma ise
            if (tarih.DayOfWeek.Equals(DayOfWeek.Friday))
            {                
                bastar = UtilityHelper.TariheSaatEkle(tarih.AddDays(3), "17:00");
                bittar = UtilityHelper.TariheSaatEkle(tarih.AddDays(3), "07:00");
            }
            //iznin son günü cumartesi ise
            else if (tarih.DayOfWeek.Equals(DayOfWeek.Saturday))
            {
                bastar = UtilityHelper.TariheSaatEkle(tarih.AddDays(2), "17:00");
                bittar = UtilityHelper.TariheSaatEkle(tarih.AddDays(2), "07:00");
            }
            //iznin son günü pazar ise
            else if (tarih.DayOfWeek.Equals(DayOfWeek.Sunday))
            {
                bastar = UtilityHelper.TariheSaatEkle(tarih.AddDays(1), "17:00");
                bittar = UtilityHelper.TariheSaatEkle(tarih.AddDays(1), "07:00");
            }

            ih = ih.SelectByPersonelTarih(personelId,bastar,bittar);
            if (ih != null)
            {
                return ("(Devaminda izni var. </br>Dönüs tarihi: "+ih.BitisTarihi.AddDays(1).ConvertToDatetimeEmptyIfNull()+")");
            }
            else 
                return (string.Empty);
        }
        private string BitisTarihindeBaslayanGorevOnayiVarMi(int personelId, DateTime bitTarDate)
        {
            GorevOnay gorevOnay = new GorevOnay();
            DateTime tarih = new DateTime(bitTarDate.Year, bitTarDate.Month, bitTarDate.Day);
            DateTime bastar = UtilityHelper.TariheSaatEkle(tarih.AddDays(1), "17:00");
            DateTime bittar = UtilityHelper.TariheSaatEkle(tarih.AddDays(1), "07:00");

            //Görevin son günü cuma ise
            if (tarih.DayOfWeek.Equals(DayOfWeek.Friday))
            {
                bastar = UtilityHelper.TariheSaatEkle(tarih.AddDays(3), "17:00");
                bittar = UtilityHelper.TariheSaatEkle(tarih.AddDays(3), "07:00");
            }
            //Görevin son günü cumartesi ise
            else if (tarih.DayOfWeek.Equals(DayOfWeek.Saturday))
            {
                bastar = UtilityHelper.TariheSaatEkle(tarih.AddDays(2), "17:00");
                bittar = UtilityHelper.TariheSaatEkle(tarih.AddDays(2), "07:00");
            }
            //Görevin son günü pazar ise
            else if (tarih.DayOfWeek.Equals(DayOfWeek.Sunday))
            {
                bastar = UtilityHelper.TariheSaatEkle(tarih.AddDays(1), "17:00");
                bittar = UtilityHelper.TariheSaatEkle(tarih.AddDays(1), "07:00");
            }

            gorevOnay = gorevOnay.SelectByPersonelTarih(personelId, bastar, bittar);
            if (gorevOnay != null)
            {
                return ("(Devaminda görevi var. </br>Görev Bitisi: " + gorevOnay.BitisTarihi.ConvertToDDMMYYYHHmmFormat() + ")");
            }
            else
                return (string.Empty);
        }

        private void MazeretIzinliPersonelTablosunuDoldur()
        {
            IzinHareket ih = new IzinHareket();
            DateTime today = DateTime.Today;
            int year = today.Year;
            int month = today.Month; ;
            int day = today.Day;

            DateTime todaybas = UtilityHelper.TariheSaatEkle(today, "00:00");
            DateTime todaybit = UtilityHelper.TariheSaatEkle(today, "17:00");
            DataTable dataTable = ih.SelectByIzinTipiTarihReturnDataTable(ProjeConstants.IZINTIPI_MAZERET_INT, todaybas, todaybit);

            bool izinTableBos = true;
            if (dataTable != null)
            {
                IzinliPerTableHeaders(MazeretIzinliPerTable,ProjeConstants.IZINTIPI_MAZERET);
                int SiraNo = 1;

                foreach (DataRow dataRow in dataTable.Rows)
                {
                    int izinTipiId = dataRow["IzinTipi"].ConvertToInt();
                    string sure = dataRow["Sure"].ReturnEmptyIfNull().ToString();
                    string izinTipi = dataRow["IzinTanim"].ReturnEmptyIfNull().ToString();
                    string adiSoyadi = dataRow["AdiSoyadi"].ReturnEmptyIfNull().ToString();
                    string bastar = dataRow["BaslangicTarihi"].ConvertToDatetime().ToString();
                    string bittar = dataRow["BitisTarihi"].ConvertToDatetime().ToString();
                    string birim = dataRow["Birim"].ReturnEmptyIfNull().ToString();
                    string gorevYeri = dataRow["GorevYeri"].ReturnEmptyIfNull().ToString();

                    TimeSpan sureTS = sure.ConvertToTimeSpan();
                    int sureInt = sureTS.Hours;

                    izinTableBos = false;
                    TableRow row = new TableRow();

                    TableCell SiraCell = new TableCell();
                    SiraCell.Text = SiraNo++ + "";
                    row.Controls.Add(SiraCell);

                    TableCell AdiSoyadiCell = new TableCell();
                    AdiSoyadiCell.Text = adiSoyadi;
                    row.Controls.Add(AdiSoyadiCell);

                    TableCell IzinTipiCell = new TableCell();
                    IzinTipiCell.Text = izinTipi;
                    row.Controls.Add(IzinTipiCell);

                    TableCell BastarCell = new TableCell();
                    BastarCell.Text = bastar;
                    row.Controls.Add(BastarCell);

                    TableCell BittarCell = new TableCell();
                    BittarCell.Text = bittar;
                    row.Controls.Add(BittarCell);

                    TableCell SureCell = new TableCell();
                    SureCell.Text = sureTS.ConvertToTimeSpanReturnInHHmm();
                    row.Controls.Add(SureCell);

                    TableCell GorevYeriCell = new TableCell();
                    GorevYeriCell.Text = gorevYeri;
                    row.Controls.Add(GorevYeriCell);

                    MazeretIzinliPerTable.Controls.Add(row);
                }

            }
            if (izinTableBos)
            {
                MazeretIzinliPerTable.Rows.Clear();
                TableHeaderRow thbaslik = new TableHeaderRow();
                TableHeaderCell baslikCell = new TableHeaderCell();

                baslikCell.Text = "Mazeret Izinli Olanlar";
                baslikCell.Font.Bold = true;
                baslikCell.ColumnSpan = 7;
                thbaslik.CssClass = "alert-secondary text-center";
                thbaslik.Controls.Add(baslikCell);
                MazeretIzinliPerTable.Controls.Add(thbaslik);

                TableRow tr = new TableRow();
                TableCell tc = new TableCell();
                tc.ColumnSpan = 7;
                tc.Text = "Izinli personel bulunmamaktadir.";
                tr.Controls.Add(tc);
                MazeretIzinliPerTable.Controls.Add(tr);
            }
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            GorevOnayliOlanPersonelTablosunuDoldur();
            UcretliIzinliPersonelTablosunuDoldur();
            RaporluGorevliHastanedePersonelTablosunuDoldur();
            ExportToExcel();
        }
        protected void ExportToExcel()
        {
            string filename = "GorevliIzinliPersonelListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            MainCardDiv.RenderControl(hw);
            //Write the HTML back to the browser.
            //Response.ContentType = application/vnd.ms-excel;
            Page.Response.ContentType = "application/ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            string s = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\n<head>\n<title></title>\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1254\" />\n<style>\n</style>\n</head>\n<body>\n";
            Page.Response.Write(s + tw.ToString());
            Page.Response.End();

        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
    }
}
