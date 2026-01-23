using Model.Ortak;
using Model.TBYS;
using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.KiraGelirleriAyDokumuWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraGelirleriAyDokumuWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraGelirleriAyDokumuWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                YilDDLDoldur();
                UtilityHelper.SetDDLValue(YilDDL,DateTime.Today.Year.ReturnEmptyIfNull().ToString());

            }
            int yil = YilDDL.SelectedItem!=null? YilDDL.SelectedItem.Value.ConvertToInt():DateTime.Today.Year;
            YilBazindaAylikKiraGelirleriniHesapla(yil);
        }

        private void YilBazindaAylikKiraGelirleriniHesapla(int yil)
        {
            if (yil>0)
            {
                TitleLbl.Text = yil + " YILI AYLIK KİRA GELİRLERİ";
                KiraGelirleriTable.Controls.Clear();
                KiraGelirleriTableHeaders();
                string bolge = string.Empty;
                decimal meskenYilToplam = 0;
                decimal isyeriYilToplam = 0;
                decimal arsaYilToplam = 0;
                decimal tarlaYilToplam = 0;
                decimal bisYilToplam = 0;
                decimal tesisYilToplam = 0;
                for (int ay = 1; ay <= 12; ay++)
                {
                    OdemePlani odemePlaniDao = new OdemePlani();
                    DataTable dataTableGM = odemePlaniDao.SelectKiraGeliriByBolgeAyYilFTKKuruluOlmayanIlIlceListesi(ProjeConstants.BOLGE_ANKARA_INT, ay, yil);
                    DataTable dataTableIST = odemePlaniDao.SelectKiraGeliriByBolgeAyYil(ProjeConstants.BOLGE_ISTANBUL_INT, ay, yil);
                    DataTable dataTableIZM = odemePlaniDao.SelectKiraGeliriByBolgeAyYil(ProjeConstants.BOLGE_IZMIR_INT, ay, yil);
                    DataTable dataTableMER = odemePlaniDao.SelectKiraGeliriByBolgeAyYil(ProjeConstants.BOLGE_MERSIN_INT, ay, yil);
                    DataTable dataTableERZ = odemePlaniDao.SelectKiraGeliriByBolgeAyYil(ProjeConstants.BOLGE_ERZURUM_INT, ay, yil);

                    int kiraciSayisiToplam = 0;
                    decimal meskenToplam = 0;
                    decimal isyeriToplam = 0;
                    decimal arsaToplam = 0;
                    decimal tarlaToplam = 0;
                    decimal bisToplam = 0;
                    decimal tesisToplam = 0;

                    int kiraciSayisi = 0;
                    decimal arsaOdenen = 0;
                    decimal isyeriOdenen = 0;
                    decimal meskenOdenen = 0;
                    decimal tarlaOdenen = 0;
                    decimal tesisOdenen = 0;
                    decimal bisOdenen = 0;

                    AylikKiraGeliriniHesaplaVeTabloyaEkle(ProjeConstants.BOLGE_ANKARA, true, ay, yil, dataTableGM,
                        out arsaOdenen, out isyeriOdenen, out meskenOdenen, out tarlaOdenen, out tesisOdenen, out bisOdenen, out kiraciSayisi);

                    kiraciSayisiToplam = kiraciSayisi;
                    meskenToplam = meskenOdenen;
                    isyeriToplam = isyeriOdenen;
                    arsaToplam = arsaOdenen;
                    tarlaToplam = tarlaOdenen;
                    tesisToplam = tesisOdenen;
                    bisToplam = bisOdenen;

                    AylikKiraGeliriniHesaplaVeTabloyaEkle(ProjeConstants.BOLGE_ISTANBUL, false, ay, yil, dataTableIST,
                        out arsaOdenen, out isyeriOdenen, out meskenOdenen, out tarlaOdenen, out tesisOdenen, out bisOdenen, out kiraciSayisi);
                    kiraciSayisiToplam += kiraciSayisi;
                    meskenToplam += meskenOdenen;
                    isyeriToplam += isyeriOdenen;
                    arsaToplam += arsaOdenen;
                    tarlaToplam += tarlaOdenen;
                    tesisToplam += tesisOdenen;
                    bisToplam += bisOdenen;

                    AylikKiraGeliriniHesaplaVeTabloyaEkle(ProjeConstants.BOLGE_IZMIR, false, ay, yil, dataTableIZM,
                        out arsaOdenen, out isyeriOdenen, out meskenOdenen, out tarlaOdenen, out tesisOdenen, out bisOdenen, out kiraciSayisi);
                    kiraciSayisiToplam += kiraciSayisi;
                    meskenToplam += meskenOdenen;
                    isyeriToplam += isyeriOdenen;
                    arsaToplam += arsaOdenen;
                    tarlaToplam += tarlaOdenen;
                    tesisToplam += tesisOdenen;
                    bisToplam += bisOdenen;

                    AylikKiraGeliriniHesaplaVeTabloyaEkle(ProjeConstants.BOLGE_MERSIN, false, ay, yil, dataTableMER,
                        out arsaOdenen, out isyeriOdenen, out meskenOdenen, out tarlaOdenen, out tesisOdenen, out bisOdenen, out kiraciSayisi);
                    kiraciSayisiToplam += kiraciSayisi;
                    meskenToplam += meskenOdenen;
                    isyeriToplam += isyeriOdenen;
                    arsaToplam += arsaOdenen;
                    tarlaToplam += tarlaOdenen;
                    tesisToplam += tesisOdenen;
                    bisToplam += bisOdenen;

                    AylikKiraGeliriniHesaplaVeTabloyaEkle(ProjeConstants.BOLGE_ERZURUM, false, ay, yil, dataTableERZ,
                        out arsaOdenen, out isyeriOdenen, out meskenOdenen, out tarlaOdenen, out tesisOdenen, out bisOdenen, out kiraciSayisi);
                    kiraciSayisiToplam += kiraciSayisi;
                    meskenToplam += meskenOdenen;
                    isyeriToplam += isyeriOdenen;
                    arsaToplam += arsaOdenen;
                    tarlaToplam += tarlaOdenen;
                    tesisToplam += tesisOdenen;
                    bisToplam += bisOdenen;

                    meskenYilToplam += meskenToplam;
                    isyeriYilToplam += isyeriToplam;
                    arsaYilToplam += arsaToplam;
                    tarlaYilToplam += tarlaToplam;
                    tesisYilToplam += tesisToplam;
                    bisYilToplam += bisToplam;

                    AyToplamiHesaplaVeTabloyaEkle(ay, yil, kiraciSayisiToplam, meskenToplam, isyeriToplam, arsaToplam, tarlaToplam, bisToplam, tesisToplam);

                }

                YilToplamiHesaplaVeTabloyaEkle(yil, meskenYilToplam, isyeriYilToplam, arsaYilToplam, tarlaYilToplam, bisYilToplam, tesisYilToplam);
            }
            
        }

        private void KiraGelirleriTableHeaders()
        {
            TableHeaderRow tableHeaderRow = new TableHeaderRow();
            TableHeaderRow tableHeaderRow1 = new TableHeaderRow();

            TableHeaderCell siraNoCell = new TableHeaderCell();
            siraNoCell.RowSpan = 2;
            siraNoCell.BorderStyle = BorderStyle.Solid;
            siraNoCell.BorderWidth = 2;
            siraNoCell.BorderColor = System.Drawing.Color.Black;
            siraNoCell.HorizontalAlign = HorizontalAlign.Right;
            siraNoCell.Attributes["style"] = "vertical-align:middle";
            siraNoCell.Font.Bold = true;
            siraNoCell.Text = "S.NO";

            TableHeaderCell ayCell = new TableHeaderCell();
            ayCell.RowSpan = 2;
            ayCell.BorderStyle = BorderStyle.Solid;
            ayCell.BorderWidth = 2;
            ayCell.BorderColor = System.Drawing.Color.Black;
            ayCell.HorizontalAlign = HorizontalAlign.Center;
            ayCell.Attributes["style"] = "vertical-align:middle";
            ayCell.Font.Bold = true;
            ayCell.Text = "AYLAR";

            TableHeaderCell bolgeCell = new TableHeaderCell();
            bolgeCell.RowSpan = 2;
            bolgeCell.BorderStyle = BorderStyle.Solid;
            bolgeCell.BorderWidth = 2;
            bolgeCell.BorderColor = System.Drawing.Color.Black;
            bolgeCell.HorizontalAlign = HorizontalAlign.Center;
            bolgeCell.Attributes["style"] = "vertical-align:middle";
            bolgeCell.Font.Bold = true;
            bolgeCell.Text = "BÖLGE";

            TableHeaderCell kiraciSayisiCell = new TableHeaderCell();
            kiraciSayisiCell.RowSpan = 2;
            kiraciSayisiCell.BorderStyle = BorderStyle.Solid;
            kiraciSayisiCell.BorderWidth = 2;
            kiraciSayisiCell.BorderColor = System.Drawing.Color.Black;
            kiraciSayisiCell.HorizontalAlign = HorizontalAlign.Center;
            kiraciSayisiCell.Attributes["style"] = "vertical-align:middle";
            kiraciSayisiCell.Font.Bold = true;
            kiraciSayisiCell.Text = "KİRACI SAYISI";

            TableHeaderCell tasinmazTuruCell = new TableHeaderCell();
            tasinmazTuruCell.ColumnSpan = 6;
            tasinmazTuruCell.BorderStyle = BorderStyle.Solid;
            tasinmazTuruCell.BorderWidth = 2;
            tasinmazTuruCell.BorderColor = System.Drawing.Color.Black;
            tasinmazTuruCell.HorizontalAlign = HorizontalAlign.Center;
            tasinmazTuruCell.Attributes["style"] = "vertical-align:middle";
            tasinmazTuruCell.Font.Bold = true;
            tasinmazTuruCell.Text = "KİRA ELDE EDİLEN TAŞINMAZIN TÜRÜ";

            TableHeaderCell kiraGeliriCell = new TableHeaderCell();
            kiraGeliriCell.RowSpan = 2;
            kiraGeliriCell.BorderStyle = BorderStyle.Solid;
            kiraGeliriCell.BorderWidth = 2;
            kiraGeliriCell.BorderColor = System.Drawing.Color.Black;
            kiraGeliriCell.HorizontalAlign = HorizontalAlign.Right;
            kiraGeliriCell.Attributes["style"] = "vertical-align:middle";
            kiraGeliriCell.Font.Bold = true;
            kiraGeliriCell.Text = "KİRA GELİRİ";

            tableHeaderRow.Controls.Add(siraNoCell);
            tableHeaderRow.Controls.Add(ayCell);
            tableHeaderRow.Controls.Add(bolgeCell);
            tableHeaderRow.Controls.Add(kiraciSayisiCell);
            tableHeaderRow.Controls.Add(tasinmazTuruCell);
            tableHeaderRow.Controls.Add(kiraGeliriCell);

            TableHeaderCell meskenCell = new TableHeaderCell();
            meskenCell.BorderStyle = BorderStyle.Solid;
            meskenCell.BorderWidth = 2;
            meskenCell.BorderColor = System.Drawing.Color.Black;
            meskenCell.HorizontalAlign = HorizontalAlign.Center;
            meskenCell.Font.Bold = true;
            meskenCell.Text = "Mesken";

            TableHeaderCell isyeriCell = new TableHeaderCell();
            isyeriCell.BorderStyle = BorderStyle.Solid;
            isyeriCell.BorderWidth = 2;
            isyeriCell.BorderColor = System.Drawing.Color.Black;
            isyeriCell.HorizontalAlign = HorizontalAlign.Center;
            isyeriCell.Font.Bold = true;
            isyeriCell.Text = "İşyeri";


            TableHeaderCell arsaCell = new TableHeaderCell();
            arsaCell.BorderStyle = BorderStyle.Solid;
            arsaCell.BorderWidth = 2;
            arsaCell.BorderColor = System.Drawing.Color.Black;
            arsaCell.HorizontalAlign = HorizontalAlign.Center;
            arsaCell.Font.Bold = true;
            arsaCell.Text = "Arsa";


            TableHeaderCell tarlaCell = new TableHeaderCell();
            tarlaCell.BorderStyle = BorderStyle.Solid;
            tarlaCell.BorderWidth = 2;
            tarlaCell.BorderColor = System.Drawing.Color.Black;
            tarlaCell.HorizontalAlign = HorizontalAlign.Center;
            tarlaCell.Font.Bold = true;
            tarlaCell.Text = "Tarla";

            TableHeaderCell bisCell = new TableHeaderCell();
            bisCell.BorderStyle = BorderStyle.Solid;
            bisCell.BorderWidth = 2;
            bisCell.BorderColor = System.Drawing.Color.Black;
            bisCell.HorizontalAlign = HorizontalAlign.Center;
            bisCell.Font.Bold = true;
            bisCell.Text = "Bis";

            TableHeaderCell tesisCell = new TableHeaderCell();
            tesisCell.BorderStyle = BorderStyle.Solid;
            tesisCell.BorderWidth = 2;
            tesisCell.BorderColor = System.Drawing.Color.Black;
            tesisCell.HorizontalAlign = HorizontalAlign.Center;
            tesisCell.Font.Bold = true;
            tesisCell.Text = "Tesis";

            tableHeaderRow1.Controls.Add(meskenCell);
            tableHeaderRow1.Controls.Add(isyeriCell);
            tableHeaderRow1.Controls.Add(arsaCell);
            tableHeaderRow1.Controls.Add(tarlaCell);
            tableHeaderRow1.Controls.Add(bisCell);
            tableHeaderRow1.Controls.Add(tesisCell);

            KiraGelirleriTable.Controls.Add(tableHeaderRow);
            KiraGelirleriTable.Controls.Add(tableHeaderRow1);
        }

        private void YilToplamiHesaplaVeTabloyaEkle(int yil, decimal meskenToplam, decimal isyeriToplam,
            decimal arsaToplam, decimal tarlaToplam, decimal bisToplam, decimal tesisToplam)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            TableRow tableRow = new TableRow();
            TableCell ayToplamLabelCell = new TableCell();
            ayToplamLabelCell.ColumnSpan = 4;
            ayToplamLabelCell.BorderStyle = BorderStyle.Solid;
            ayToplamLabelCell.BorderWidth = 3;
            ayToplamLabelCell.BorderColor = System.Drawing.Color.Black;
            ayToplamLabelCell.BackColor = System.Drawing.Color.Gray;
            ayToplamLabelCell.HorizontalAlign = HorizontalAlign.Right;
            ayToplamLabelCell.Font.Bold = true;
            ayToplamLabelCell.Text = yil + " Toplamı";

            TableCell meskenToplamCell = new TableCell();
            meskenToplamCell.BorderStyle = BorderStyle.Solid;
            meskenToplamCell.BorderWidth = 3;
            meskenToplamCell.BorderColor = System.Drawing.Color.Black;
            meskenToplamCell.BackColor = System.Drawing.Color.Gray;
            meskenToplamCell.HorizontalAlign = HorizontalAlign.Right;
            meskenToplamCell.Font.Bold = true;
            meskenToplamCell.Text = meskenToplam.ToString("N", culturInfo);

            TableCell isyeriToplamCell = new TableCell();
            isyeriToplamCell.BorderStyle = BorderStyle.Solid;
            isyeriToplamCell.BorderWidth = 3;
            isyeriToplamCell.BorderColor = System.Drawing.Color.Black;
            isyeriToplamCell.BackColor = System.Drawing.Color.Gray;
            isyeriToplamCell.HorizontalAlign = HorizontalAlign.Right;
            isyeriToplamCell.Font.Bold = true;
            isyeriToplamCell.Text = isyeriToplam.ToString("N", culturInfo);

            TableCell arsaToplamCell = new TableCell();
            arsaToplamCell.BorderStyle = BorderStyle.Solid;
            arsaToplamCell.BorderWidth = 3;
            arsaToplamCell.BorderColor = System.Drawing.Color.Black;
            arsaToplamCell.BackColor = System.Drawing.Color.Gray;
            arsaToplamCell.HorizontalAlign = HorizontalAlign.Right;
            arsaToplamCell.Font.Bold = true;
            arsaToplamCell.Text = arsaToplam.ToString("N", culturInfo);

            TableCell tarlaToplamCell = new TableCell();
            tarlaToplamCell.BorderStyle = BorderStyle.Solid;
            tarlaToplamCell.BorderWidth = 3;
            tarlaToplamCell.BorderColor = System.Drawing.Color.Black;
            tarlaToplamCell.BackColor = System.Drawing.Color.Gray;
            tarlaToplamCell.HorizontalAlign = HorizontalAlign.Right;
            tarlaToplamCell.Font.Bold = true;
            tarlaToplamCell.Text = tarlaToplam.ToString("N", culturInfo);

            TableCell bisToplamCell = new TableCell();
            bisToplamCell.BorderStyle = BorderStyle.Solid;
            bisToplamCell.BorderWidth = 3;
            bisToplamCell.BorderColor = System.Drawing.Color.Black;
            bisToplamCell.BackColor = System.Drawing.Color.Gray;
            bisToplamCell.HorizontalAlign = HorizontalAlign.Right;
            bisToplamCell.Font.Bold = true;
            bisToplamCell.Text = bisToplam.ToString("N", culturInfo);

            TableCell tesisToplamCell = new TableCell();
            tesisToplamCell.BorderStyle = BorderStyle.Solid;
            tesisToplamCell.BorderWidth = 3;
            tesisToplamCell.BorderColor = System.Drawing.Color.Black;
            tesisToplamCell.BackColor = System.Drawing.Color.Gray;
            tesisToplamCell.HorizontalAlign = HorizontalAlign.Right;
            tesisToplamCell.Font.Bold = true;
            tesisToplamCell.Text = tesisToplam.ToString("N", culturInfo);


            TableCell ayToplamCell = new TableCell();
            ayToplamCell.BorderStyle = BorderStyle.Solid;
            ayToplamCell.BorderWidth = 3;
            ayToplamCell.BorderColor = System.Drawing.Color.Black;
            ayToplamCell.BackColor = System.Drawing.Color.Gray;
            ayToplamCell.HorizontalAlign = HorizontalAlign.Right;
            ayToplamCell.Font.Bold = true;
            ayToplamCell.Text = (meskenToplam + isyeriToplam + arsaToplam + tarlaToplam + bisToplam + tesisToplam).ToString("N", culturInfo);



            tableRow.Controls.Add(ayToplamLabelCell);
            tableRow.Controls.Add(meskenToplamCell);
            tableRow.Controls.Add(isyeriToplamCell);
            tableRow.Controls.Add(arsaToplamCell);

            tableRow.Controls.Add(tarlaToplamCell);
            tableRow.Controls.Add(bisToplamCell);
            tableRow.Controls.Add(bisToplamCell);
            tableRow.Controls.Add(tesisToplamCell);
            tableRow.Controls.Add(ayToplamCell);
            KiraGelirleriTable.Controls.Add(tableRow);
        }
        private void AyToplamiHesaplaVeTabloyaEkle(int ay, int yil, int kiraciSayisiToplam, decimal meskenToplam, decimal isyeriToplam,
            decimal arsaToplam, decimal tarlaToplam, decimal bisToplam, decimal tesisToplam)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            TableRow tableRow = new TableRow();
            TableCell ayToplamLabelCell = new TableCell();
            ayToplamLabelCell.ColumnSpan = 3;
            ayToplamLabelCell.BorderStyle = BorderStyle.Solid;
            ayToplamLabelCell.BorderWidth = 3;
            ayToplamLabelCell.BorderColor = System.Drawing.Color.Black;
            ayToplamLabelCell.BackColor = System.Drawing.Color.LightGray;
            ayToplamLabelCell.HorizontalAlign = HorizontalAlign.Right;
            ayToplamLabelCell.Font.Bold = true;
            DateTime tarih = new DateTime(yil, ay, 1);
            ayToplamLabelCell.Text = tarih.ToString("MMMM", culturInfo) + " Toplamı";


            TableCell kiraciSayisiToplamCell = new TableCell();
            kiraciSayisiToplamCell.BorderStyle = BorderStyle.Solid;
            kiraciSayisiToplamCell.BorderWidth = 3;
            kiraciSayisiToplamCell.BorderColor = System.Drawing.Color.Black;
            kiraciSayisiToplamCell.BackColor = System.Drawing.Color.LightGray;
            kiraciSayisiToplamCell.HorizontalAlign = HorizontalAlign.Right;
            kiraciSayisiToplamCell.Font.Bold = true;
            kiraciSayisiToplamCell.Text = kiraciSayisiToplam.ToString(); ;

            TableCell meskenToplamCell = new TableCell();
            meskenToplamCell.BorderStyle = BorderStyle.Solid;
            meskenToplamCell.BorderWidth = 3;
            meskenToplamCell.BorderColor = System.Drawing.Color.Black;
            meskenToplamCell.BackColor = System.Drawing.Color.LightGray;
            meskenToplamCell.HorizontalAlign = HorizontalAlign.Right;
            meskenToplamCell.Font.Bold = true;
            meskenToplamCell.Text = meskenToplam.ToString("N", culturInfo);

            TableCell isyeriToplamCell = new TableCell();
            isyeriToplamCell.BorderStyle = BorderStyle.Solid;
            isyeriToplamCell.BorderWidth = 3;
            isyeriToplamCell.BorderColor = System.Drawing.Color.Black;
            isyeriToplamCell.BackColor = System.Drawing.Color.LightGray;
            isyeriToplamCell.HorizontalAlign = HorizontalAlign.Right;
            isyeriToplamCell.Font.Bold = true;
            isyeriToplamCell.Text = isyeriToplam.ToString("N", culturInfo);

            TableCell arsaToplamCell = new TableCell();
            arsaToplamCell.BorderStyle = BorderStyle.Solid;
            arsaToplamCell.BorderWidth = 3;
            arsaToplamCell.BorderColor = System.Drawing.Color.Black;
            arsaToplamCell.BackColor = System.Drawing.Color.LightGray;
            arsaToplamCell.HorizontalAlign = HorizontalAlign.Right;
            arsaToplamCell.Font.Bold = true;
            arsaToplamCell.Text = arsaToplam.ToString("N", culturInfo);

            TableCell tarlaToplamCell = new TableCell();
            tarlaToplamCell.BorderStyle = BorderStyle.Solid;
            tarlaToplamCell.BorderWidth = 3;
            tarlaToplamCell.BorderColor = System.Drawing.Color.Black;
            tarlaToplamCell.BackColor = System.Drawing.Color.LightGray;
            tarlaToplamCell.HorizontalAlign = HorizontalAlign.Right;
            tarlaToplamCell.Font.Bold = true;
            tarlaToplamCell.Text = tarlaToplam.ToString("N", culturInfo);

            TableCell bisToplamCell = new TableCell();
            bisToplamCell.BorderStyle = BorderStyle.Solid;
            bisToplamCell.BorderWidth = 3;
            bisToplamCell.BorderColor = System.Drawing.Color.Black;
            bisToplamCell.BackColor = System.Drawing.Color.LightGray;
            bisToplamCell.HorizontalAlign = HorizontalAlign.Right;
            bisToplamCell.Font.Bold = true;
            bisToplamCell.Text = bisToplam.ToString("N", culturInfo);

            TableCell tesisToplamCell = new TableCell();
            tesisToplamCell.BorderStyle = BorderStyle.Solid;
            tesisToplamCell.BorderWidth = 3;
            tesisToplamCell.BorderColor = System.Drawing.Color.Black;
            tesisToplamCell.BackColor = System.Drawing.Color.LightGray;
            tesisToplamCell.HorizontalAlign = HorizontalAlign.Right;
            tesisToplamCell.Font.Bold = true;
            tesisToplamCell.Text = tesisToplam.ToString("N", culturInfo);


            TableCell ayToplamCell = new TableCell();
            ayToplamCell.BorderStyle = BorderStyle.Solid;
            ayToplamCell.BorderWidth = 3;
            ayToplamCell.BorderColor = System.Drawing.Color.Black;
            ayToplamCell.BackColor = System.Drawing.Color.LightGray;
            ayToplamCell.HorizontalAlign = HorizontalAlign.Right;
            ayToplamCell.Font.Bold = true;
            ayToplamCell.Text = (meskenToplam + isyeriToplam + arsaToplam + tarlaToplam + bisToplam + tesisToplam).ToString("N", culturInfo);

            tableRow.Controls.Add(ayToplamLabelCell);
            tableRow.Controls.Add(kiraciSayisiToplamCell);
            tableRow.Controls.Add(meskenToplamCell);
            tableRow.Controls.Add(isyeriToplamCell);
            tableRow.Controls.Add(arsaToplamCell);

            tableRow.Controls.Add(tarlaToplamCell);
            tableRow.Controls.Add(bisToplamCell);
            tableRow.Controls.Add(bisToplamCell);
            tableRow.Controls.Add(tesisToplamCell);
            tableRow.Controls.Add(ayToplamCell);
            KiraGelirleriTable.Controls.Add(tableRow);
        }

        private void AylikKiraGeliriniHesaplaVeTabloyaEkle(string bolge, bool ilkSatir, int ay, int yil, DataTable dataTable,
             out decimal arsaOdenen, out decimal isyeriOdenen, out decimal meskenOdenen, out decimal tarlaOdenen,
             out decimal tesisOdenen, out decimal bisOdenen, out int kiraciSayisi)
        {
            TableRow tableRow = new TableRow();
            DataView dataView = new DataView(dataTable);
            kiraciSayisi = 0;
            arsaOdenen = 0;
            isyeriOdenen = 0;
            meskenOdenen = 0;
            tarlaOdenen = 0;
            tesisOdenen = 0;
            bisOdenen = 0;

            if (dataTable != null)
            {
                foreach (DataRowView row in dataView)
                {
                    string kiralamaAmaci = row == null ? "" : row["KiralamaAmaci"].ReturnEmptyIfNull().ToString();
                    decimal odemeTutari = row == null ? 0 : row["ToplamOdemeTutari"].ConvertToDecimal();
                    int kiraciSayisiLocal = row == null ? 0 : row["ToplamKiraciSayisi"].ConvertToInt();
                    kiraciSayisi += kiraciSayisiLocal;
                    switch (kiralamaAmaci)
                    {
                        case ProjeConstants.KIRALAMAAMACI_ARSA:
                            {
                                arsaOdenen = odemeTutari;
                                break;
                            }
                        case ProjeConstants.KIRALAMAAMACI_BAZISTASYONU:
                            {
                                bisOdenen = odemeTutari;
                                break;
                            }
                        case ProjeConstants.KIRALAMAAMACI_ISYERI:
                            {
                                isyeriOdenen = odemeTutari;
                                break;
                            }
                        case ProjeConstants.KIRALAMAAMACI_MESKEN:
                            {
                                meskenOdenen = odemeTutari;
                                break;
                            }
                        case ProjeConstants.KIRALAMAAMACI_TARLA:
                            {
                                tarlaOdenen = odemeTutari;
                                break;
                            }
                        case ProjeConstants.KIRALAMAAMACI_TESIS:
                            {
                                tesisOdenen = odemeTutari;
                                break;
                            }
                    }
                }
            }
            TabloyaSatirEkle(tableRow, bolge, ay, yil, arsaOdenen, bisOdenen, isyeriOdenen, meskenOdenen, tarlaOdenen, tesisOdenen, kiraciSayisi, ilkSatir);

            KiraGelirleriTable.Controls.Add(tableRow);
        }
        private TableRow TabloyaSatirEkle(TableRow tableRow, string bolge, int ay, int yil, decimal arsaOdenen, decimal bisOdenen,
            decimal isyeriOdenen, decimal meskenOdenen, decimal tarlaOdenen, decimal tesisOdenen, int kiraciSayisi, bool ilkSatir)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            if (ilkSatir)
            {
                TableCell siraCell = new TableCell();
                siraCell.RowSpan = 5;
                siraCell.BorderStyle = BorderStyle.Solid;
                siraCell.BorderWidth = 2;
                siraCell.BorderColor = System.Drawing.Color.Black;
                siraCell.Attributes["style"] = "vertical-align:middle";
                siraCell.Text = ay.ToString();

                TableCell ayCell = new TableCell();
                DateTime tarih = new DateTime(yil, ay, 1);
                ayCell.Text = tarih.ToString("MMMM", culturInfo);
                ayCell.RowSpan = 5;
                ayCell.BorderStyle = BorderStyle.Solid;
                ayCell.BorderWidth = 2;
                ayCell.BorderColor = System.Drawing.Color.Black;
                ayCell.Attributes["style"] = "vertical-align:middle";
                tableRow.Controls.Add(siraCell);
                tableRow.Controls.Add(ayCell);
            }
            TableCell bolgeCell = new TableCell();
            bolgeCell.Text = bolge;

            TableCell kiraciSayisiCell = new TableCell();
            kiraciSayisiCell.Text = kiraciSayisi.ToString();

            TableCell meskenCell = new TableCell();
            meskenCell.Text = meskenOdenen.ToString("N", culturInfo);

            TableCell isyeriCell = new TableCell();
            isyeriCell.Text = isyeriOdenen.ToString("N", culturInfo);

            TableCell arsaCell = new TableCell();
            arsaCell.Text = arsaOdenen.ToString("N", culturInfo);

            TableCell tarlaCell = new TableCell();
            tarlaCell.Text = tarlaOdenen.ToString("N", culturInfo);

            TableCell bisCell = new TableCell();
            bisCell.Text = bisOdenen.ToString("N", culturInfo);

            TableCell tesisCell = new TableCell();
            tesisCell.Text = tesisOdenen.ToString("N", culturInfo);

            TableCell toplamCell = new TableCell();
            toplamCell.Text = (meskenOdenen + isyeriOdenen + arsaOdenen + tarlaOdenen + bisOdenen + tesisOdenen).ToString("N", culturInfo);

            bolgeCell.BorderStyle = BorderStyle.Solid;
            bolgeCell.BorderWidth = 2;
            bolgeCell.BorderColor = System.Drawing.Color.Black;

            kiraciSayisiCell.BorderStyle = BorderStyle.Solid;
            kiraciSayisiCell.BorderWidth = 2;
            kiraciSayisiCell.BorderColor = System.Drawing.Color.Black;

            meskenCell.BorderStyle = BorderStyle.Solid;
            meskenCell.BorderWidth = 2;
            meskenCell.BorderColor = System.Drawing.Color.Black;
            meskenCell.HorizontalAlign = HorizontalAlign.Right;

            isyeriCell.BorderStyle = BorderStyle.Solid;
            isyeriCell.BorderWidth = 2;
            isyeriCell.BorderColor = System.Drawing.Color.Black;
            isyeriCell.HorizontalAlign = HorizontalAlign.Right;

            arsaCell.BorderStyle = BorderStyle.Solid;
            arsaCell.BorderWidth = 2;
            arsaCell.BorderColor = System.Drawing.Color.Black;
            arsaCell.HorizontalAlign = HorizontalAlign.Right;

            tarlaCell.BorderStyle = BorderStyle.Solid;
            tarlaCell.BorderWidth = 2;
            tarlaCell.BorderColor = System.Drawing.Color.Black;
            tarlaCell.HorizontalAlign = HorizontalAlign.Right;

            bisCell.BorderStyle = BorderStyle.Solid;
            bisCell.BorderWidth = 2;
            bisCell.BorderColor = System.Drawing.Color.Black;
            bisCell.HorizontalAlign = HorizontalAlign.Right;

            tesisCell.BorderStyle = BorderStyle.Solid;
            tesisCell.BorderWidth = 2;
            tesisCell.BorderColor = System.Drawing.Color.Black;
            tesisCell.HorizontalAlign = HorizontalAlign.Right;

            toplamCell.BorderStyle = BorderStyle.Solid;
            toplamCell.BorderWidth = 2;
            toplamCell.BorderColor = System.Drawing.Color.Black;
            toplamCell.HorizontalAlign = HorizontalAlign.Right;

            tableRow.Controls.Add(bolgeCell);
            tableRow.Controls.Add(kiraciSayisiCell);
            tableRow.Controls.Add(meskenCell);
            tableRow.Controls.Add(isyeriCell);
            tableRow.Controls.Add(arsaCell);

            tableRow.Controls.Add(tarlaCell);
            tableRow.Controls.Add(bisCell);
            tableRow.Controls.Add(tesisCell);
            tableRow.Controls.Add(toplamCell);
            return tableRow;
        }
        private void YilDDLDoldur()
        {
            var year = DateTime.Now.Year;
            for (int i = 2005; i <= year; i++)
            {
                YilDDL.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();
            YilBazindaAylikKiraGelirleriniHesapla(yil);
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            string filename = YilDDL.SelectedItem.Value + "KiraGelirleriRaporu" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            KiraGelirleriTable.RenderControl(hw);
            //Write the HTML back to the browser.
            //Response.ContentType = application/vnd.ms-excel;
            Page.Response.ContentType = "application/vnd.ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            Page.Response.Write(tw.ToString());
            Page.Response.End();


        }
    }
}
