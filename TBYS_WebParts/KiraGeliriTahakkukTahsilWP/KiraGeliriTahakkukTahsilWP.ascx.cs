using DocumentFormat.OpenXml.Spreadsheet;
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

namespace TBYS_WebParts.KiraGeliriTahakkukTahsilWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraGeliriTahakkukTahsilWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraGeliriTahakkukTahsilWP()
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
                UtilityHelper.SetDDLValue(YilDDL, DateTime.Today.Year.ReturnEmptyIfNull().ToString());

            }
            int yil = YilDDL.SelectedItem != null ? YilDDL.SelectedItem.Value.ConvertToInt() : DateTime.Today.Year;
            YilBazindaAylikKiraGelirleriniHesapla(yil);
        }

        private void YilBazindaAylikKiraGelirleriniHesapla(int yil)
        {
            if (yil > 0)
            {
                TitleLbl.Text = yil + " YILI AYLIK KIRA TAHAKKUK-TAHSIL BILGILERI";
                KiraGelirleriTable.Controls.Clear();
                KiraGelirleriTableHeaders();
                string bolge = string.Empty;
                decimal meskenTahakkukYilToplam = 0;
                decimal isyeriTahakkukYilToplam = 0;
                decimal arsaTahakkukYilToplam = 0;
                decimal tarlaTahakkukYilToplam = 0;
                decimal bisTahakkukYilToplam = 0;
                decimal tesisTahakkukYilToplam = 0;
                decimal meskenTahsilYilToplam = 0;
                decimal isyeriTahsilYilToplam = 0;
                decimal arsaTahsilYilToplam = 0;
                decimal tarlaTahsilYilToplam = 0;
                decimal bisTahsilYilToplam = 0;
                decimal tesisTahsilYilToplam = 0;
                for (int ay = 1; ay <= 12; ay++)
                {

                    KiraSozlesme kiraSozlesmeDao = new KiraSozlesme();
                    DataTable dataTableANK = kiraSozlesmeDao.SelectKiraciSayisiVeToplamKiraBedeli(ProjeConstants.BOLGE_ANKARA_INT, ay, yil);
                    DataTable dataTableIST = kiraSozlesmeDao.SelectKiraciSayisiVeToplamKiraBedeli(ProjeConstants.BOLGE_ISTANBUL_INT, ay, yil);
                    DataTable dataTableIZM = kiraSozlesmeDao.SelectKiraciSayisiVeToplamKiraBedeli(ProjeConstants.BOLGE_IZMIR_INT, ay, yil);
                    DataTable dataTableMER = kiraSozlesmeDao.SelectKiraciSayisiVeToplamKiraBedeli(ProjeConstants.BOLGE_MERSIN_INT, ay, yil);
                    DataTable dataTableERZ = kiraSozlesmeDao.SelectKiraciSayisiVeToplamKiraBedeli(ProjeConstants.BOLGE_ERZURUM_INT, ay, yil);

                    int kiraciSayisiToplam = 0;
                    int odeyenKiraciSayisiToplam = 0;
                    decimal meskenTahakkukToplam = 0;
                    decimal meskenTahsilToplam = 0;
                    decimal meskenOranToplam = 0;
                    decimal isyeriTahakkukToplam = 0;
                    decimal isyeriTahsilToplam = 0;
                    decimal isyeriOranToplam = 0;
                    decimal arsaTahakkukToplam = 0;
                    decimal arsaTahsilToplam = 0;
                    decimal arsaOranToplam = 0;
                    decimal tarlaTahakkukToplam = 0;
                    decimal tarlaTahsilToplam = 0;
                    decimal tarlaOranToplam = 0;
                    decimal bisTahakkukToplam = 0;
                    decimal bisTahsilToplam = 0;
                    decimal bisOranToplam = 0;
                    decimal tesisTahakkukToplam = 0;
                    decimal tesisTahsilToplam = 0;
                    decimal tesisOranToplam = 0;

                    int kiraciSayisi = 0;
                    int odeyenKiraciSayisi = 0;
                    decimal meskenTahakkuk = 0;
                    decimal meskenTahsil = 0;
                    decimal isyeriTahakkuk = 0;
                    decimal isyeriTahsil = 0;
                    decimal arsaTahakkuk = 0;
                    decimal arsaTahsil = 0;
                    decimal tarlaTahakkuk = 0;
                    decimal tarlaTahsil = 0;
                    decimal bisTahakkuk = 0;
                    decimal bisTahsil = 0;
                    decimal tesisTahakkuk = 0;
                    decimal tesisTahsil = 0;

                    AylikKiraGeliriniHesaplaVeTabloyaEkle(ProjeConstants.BOLGE_ANKARA, true, ay, yil, dataTableANK, out kiraciSayisi, out odeyenKiraciSayisi,
                        out arsaTahakkuk, out bisTahakkuk, out isyeriTahakkuk, out meskenTahakkuk, out tarlaTahakkuk, out tesisTahakkuk,
                        out arsaTahsil, out bisTahsil, out isyeriTahsil, out meskenTahsil, out tarlaTahsil, out tesisTahsil);

                    kiraciSayisiToplam = kiraciSayisi;
                    odeyenKiraciSayisiToplam = odeyenKiraciSayisi;

                    meskenTahakkukToplam = meskenTahakkuk;
                    isyeriTahakkukToplam = isyeriTahakkuk;
                    arsaTahakkukToplam = arsaTahakkuk;
                    tarlaTahakkukToplam = tarlaTahakkuk;
                    tesisTahakkukToplam = tesisTahakkuk;
                    bisTahakkukToplam = bisTahakkuk;

                    meskenTahsilToplam = meskenTahsil;
                    isyeriTahsilToplam = isyeriTahsil;
                    arsaTahsilToplam = arsaTahsil;
                    tarlaTahsilToplam = tarlaTahsil;
                    tesisTahsilToplam = tesisTahsil;
                    bisTahsilToplam = bisTahsil;

                    AylikKiraGeliriniHesaplaVeTabloyaEkle(ProjeConstants.BOLGE_ISTANBUL, false, ay, yil, dataTableIST, out kiraciSayisi, out odeyenKiraciSayisi,
                        out arsaTahakkuk, out bisTahakkuk, out isyeriTahakkuk, out meskenTahakkuk, out tarlaTahakkuk, out tesisTahakkuk,
                        out arsaTahsil, out bisTahsil, out isyeriTahsil, out meskenTahsil, out tarlaTahsil, out tesisTahsil);

                    kiraciSayisiToplam += kiraciSayisi;
                    odeyenKiraciSayisiToplam += odeyenKiraciSayisi;

                    meskenTahakkukToplam += meskenTahakkuk;
                    isyeriTahakkukToplam += isyeriTahakkuk;
                    arsaTahakkukToplam += arsaTahakkuk;
                    tarlaTahakkukToplam += tarlaTahakkuk;
                    tesisTahakkukToplam += tesisTahakkuk;
                    bisTahakkukToplam += bisTahakkuk;

                    meskenTahsilToplam += meskenTahsil;
                    isyeriTahsilToplam += isyeriTahsil;
                    arsaTahsilToplam += arsaTahsil;
                    tarlaTahsilToplam += tarlaTahsil;
                    tesisTahsilToplam += tesisTahsil;
                    bisTahsilToplam += bisTahsil;

                    AylikKiraGeliriniHesaplaVeTabloyaEkle(ProjeConstants.BOLGE_IZMIR, false, ay, yil, dataTableIZM, out kiraciSayisi, out odeyenKiraciSayisi,
                        out arsaTahakkuk, out bisTahakkuk, out isyeriTahakkuk, out meskenTahakkuk, out tarlaTahakkuk, out tesisTahakkuk, 
                        out arsaTahsil,  out bisTahsil, out isyeriTahsil, out meskenTahsil, out tarlaTahsil, out tesisTahsil);

                    kiraciSayisiToplam += kiraciSayisi;
                    odeyenKiraciSayisiToplam += odeyenKiraciSayisi;

                    meskenTahakkukToplam += meskenTahakkuk;
                    isyeriTahakkukToplam += isyeriTahakkuk;
                    arsaTahakkukToplam += arsaTahakkuk;
                    tarlaTahakkukToplam += tarlaTahakkuk;
                    tesisTahakkukToplam += tesisTahakkuk;
                    bisTahakkukToplam += bisTahakkuk;

                    meskenTahsilToplam += meskenTahsil;
                    isyeriTahsilToplam += isyeriTahsil;
                    arsaTahsilToplam += arsaTahsil;
                    tarlaTahsilToplam += tarlaTahsil;
                    tesisTahsilToplam += tesisTahsil;
                    bisTahsilToplam += bisTahsil;

                    AylikKiraGeliriniHesaplaVeTabloyaEkle(ProjeConstants.BOLGE_MERSIN, false, ay, yil, dataTableMER, out kiraciSayisi, out odeyenKiraciSayisi,
                                            out arsaTahakkuk, out bisTahakkuk, out isyeriTahakkuk, out meskenTahakkuk, out tarlaTahakkuk, out tesisTahakkuk,
                                            out arsaTahsil, out bisTahsil, out isyeriTahsil, out meskenTahsil, out tarlaTahsil, out tesisTahsil);

                    kiraciSayisiToplam += kiraciSayisi;
                    odeyenKiraciSayisiToplam += odeyenKiraciSayisi;

                    meskenTahakkukToplam += meskenTahakkuk;
                    isyeriTahakkukToplam += isyeriTahakkuk;
                    arsaTahakkukToplam += arsaTahakkuk;
                    tarlaTahakkukToplam += tarlaTahakkuk;
                    tesisTahakkukToplam += tesisTahakkuk;
                    bisTahakkukToplam += bisTahakkuk;

                    meskenTahsilToplam += meskenTahsil;
                    isyeriTahsilToplam += isyeriTahsil;
                    arsaTahsilToplam += arsaTahsil;
                    tarlaTahsilToplam += tarlaTahsil;
                    tesisTahsilToplam += tesisTahsil;
                    bisTahsilToplam += bisTahsil;

                    AylikKiraGeliriniHesaplaVeTabloyaEkle(ProjeConstants.BOLGE_ERZURUM, false, ay, yil, dataTableERZ, out kiraciSayisi, out odeyenKiraciSayisi,
                                            out arsaTahakkuk, out bisTahakkuk, out isyeriTahakkuk, out meskenTahakkuk, out tarlaTahakkuk, out tesisTahakkuk,
                                            out arsaTahsil, out bisTahsil, out isyeriTahsil, out meskenTahsil, out tarlaTahsil, out tesisTahsil);

                    kiraciSayisiToplam += kiraciSayisi;
                    odeyenKiraciSayisiToplam += odeyenKiraciSayisi;

                    meskenTahakkukToplam += meskenTahakkuk;
                    isyeriTahakkukToplam += isyeriTahakkuk;
                    arsaTahakkukToplam += arsaTahakkuk;
                    tarlaTahakkukToplam += tarlaTahakkuk;
                    tesisTahakkukToplam += tesisTahakkuk;
                    bisTahakkukToplam += bisTahakkuk;

                    meskenTahsilToplam += meskenTahsil;
                    isyeriTahsilToplam += isyeriTahsil;
                    arsaTahsilToplam += arsaTahsil;
                    tarlaTahsilToplam += tarlaTahsil;
                    tesisTahsilToplam += tesisTahsil;
                    bisTahsilToplam += bisTahsil;

                    meskenTahakkukYilToplam += meskenTahakkukToplam;
                    isyeriTahakkukYilToplam += isyeriTahakkukToplam;
                    arsaTahakkukYilToplam += arsaTahakkukToplam;
                    tarlaTahakkukYilToplam += tarlaTahakkukToplam;
                    tesisTahakkukYilToplam += tesisTahakkukToplam;
                    bisTahakkukYilToplam += bisTahakkukToplam;

                    meskenTahsilYilToplam += meskenTahsilToplam;
                    isyeriTahsilYilToplam += isyeriTahsilToplam;
                    arsaTahsilYilToplam += arsaTahsilToplam;
                    tarlaTahsilYilToplam += tarlaTahsilToplam;
                    tesisTahsilYilToplam += tesisTahsilToplam;
                    bisTahsilYilToplam += bisTahsilToplam;

                    AyToplamiHesaplaVeTabloyaEkle(ay, yil, kiraciSayisiToplam, odeyenKiraciSayisiToplam,
                        arsaTahakkukToplam, bisTahakkukToplam, isyeriTahakkukToplam, meskenTahakkukToplam, tarlaTahakkukToplam, tesisTahakkukToplam,
                         arsaTahsilToplam, bisTahsilToplam, isyeriTahsilToplam, meskenTahsilToplam, tarlaTahsilToplam,tesisTahsilToplam);


                }

                YilToplamiHesaplaVeTabloyaEkle(yil, 
                    arsaTahakkukYilToplam, bisTahakkukYilToplam, isyeriTahakkukYilToplam, meskenTahakkukYilToplam,   tarlaTahakkukYilToplam, tesisTahakkukYilToplam,
                    arsaTahsilYilToplam, bisTahsilYilToplam, isyeriTahsilYilToplam, meskenTahsilYilToplam, tarlaTahsilYilToplam, tesisTahsilYilToplam
                    );
            }

        }

        private void KiraGelirleriTableHeaders()
        {
            TableHeaderRow tableHeaderRow = new TableHeaderRow();
            TableHeaderRow tableHeaderRow1 = new TableHeaderRow();
            TableHeaderRow tableHeaderRow2 = new TableHeaderRow();

            TableHeaderCell siraNoCell = new TableHeaderCell();
            siraNoCell.RowSpan = 3;
            siraNoCell.BorderStyle = BorderStyle.Solid;
            siraNoCell.BorderWidth = 2;
            siraNoCell.BorderColor = System.Drawing.Color.Black;
            siraNoCell.HorizontalAlign = HorizontalAlign.Right;
            siraNoCell.Attributes["style"] = "vertical-align:middle";
            siraNoCell.Font.Bold = true;
            siraNoCell.Text = "S.NO";

            TableHeaderCell ayCell = new TableHeaderCell();
            ayCell.RowSpan = 3;
            ayCell.BorderStyle = BorderStyle.Solid;
            ayCell.BorderWidth = 2;
            ayCell.BorderColor = System.Drawing.Color.Black;
            ayCell.HorizontalAlign = HorizontalAlign.Center;
            ayCell.Attributes["style"] = "vertical-align:middle";
            ayCell.Font.Bold = true;
            ayCell.Text = "AYLAR";

            TableHeaderCell bolgeCell = new TableHeaderCell();
            bolgeCell.RowSpan = 3;
            bolgeCell.BorderStyle = BorderStyle.Solid;
            bolgeCell.BorderWidth = 2;
            bolgeCell.BorderColor = System.Drawing.Color.Black;
            bolgeCell.HorizontalAlign = HorizontalAlign.Center;
            bolgeCell.Attributes["style"] = "vertical-align:middle";
            bolgeCell.Font.Bold = true;
            bolgeCell.Text = "BÖLGE";

            TableHeaderCell kiraciSayisiCell = new TableHeaderCell();
            kiraciSayisiCell.RowSpan = 3;
            //kiraciSayisiCell.ColumnSpan = 2;
            kiraciSayisiCell.BorderStyle = BorderStyle.Solid;
            kiraciSayisiCell.BorderWidth = 2;
            kiraciSayisiCell.BorderColor = System.Drawing.Color.Black;
            kiraciSayisiCell.HorizontalAlign = HorizontalAlign.Center;
            kiraciSayisiCell.Attributes["style"] = "vertical-align:middle";
            kiraciSayisiCell.Font.Bold = true;
            kiraciSayisiCell.Text = "KIRACI SAYISI";

            TableHeaderCell tasinmazTuruCell = new TableHeaderCell();
            tasinmazTuruCell.ColumnSpan = 18;
            tasinmazTuruCell.BorderStyle = BorderStyle.Solid;
            tasinmazTuruCell.BorderWidth = 2;
            tasinmazTuruCell.BorderColor = System.Drawing.Color.Black;
            tasinmazTuruCell.HorizontalAlign = HorizontalAlign.Center;
            tasinmazTuruCell.Attributes["style"] = "vertical-align:middle";
            tasinmazTuruCell.Font.Bold = true;
            tasinmazTuruCell.Text = "KIRA ELDE EDILEN TASINMAZIN TÜRÜ";

            TableHeaderCell kiraGeliriCell = new TableHeaderCell();
            kiraGeliriCell.RowSpan = 2;
            kiraGeliriCell.ColumnSpan = 3;
            kiraGeliriCell.BorderStyle = BorderStyle.Solid;
            kiraGeliriCell.BorderWidth = 2;
            kiraGeliriCell.BorderColor = System.Drawing.Color.Black;
            kiraGeliriCell.HorizontalAlign = HorizontalAlign.Right;
            kiraGeliriCell.Attributes["style"] = "vertical-align:middle";
            kiraGeliriCell.Font.Bold = true;
            kiraGeliriCell.Text = "KIRA GELIRI";

            tableHeaderRow.Controls.Add(siraNoCell);
            tableHeaderRow.Controls.Add(ayCell);
            tableHeaderRow.Controls.Add(bolgeCell);
            tableHeaderRow.Controls.Add(kiraciSayisiCell);
            tableHeaderRow.Controls.Add(tasinmazTuruCell);
            tableHeaderRow.Controls.Add(kiraGeliriCell);

            TableHeaderCell meskenCell = new TableHeaderCell();
            meskenCell.ColumnSpan = 3;
            meskenCell.BorderStyle = BorderStyle.Solid;
            meskenCell.BorderWidth = 2;
            meskenCell.BorderColor = System.Drawing.Color.Black;
            meskenCell.HorizontalAlign = HorizontalAlign.Center;
            meskenCell.Font.Bold = true;
            meskenCell.Text = "Mesken";

            TableHeaderCell isyeriCell = new TableHeaderCell();
            isyeriCell.ColumnSpan = 3;
            isyeriCell.BorderStyle = BorderStyle.Solid;
            isyeriCell.BorderWidth = 2;
            isyeriCell.BorderColor = System.Drawing.Color.Black;
            isyeriCell.HorizontalAlign = HorizontalAlign.Center;
            isyeriCell.Font.Bold = true;
            isyeriCell.Text = "Isyeri";


            TableHeaderCell arsaCell = new TableHeaderCell();
            arsaCell.ColumnSpan = 3;
            arsaCell.BorderStyle = BorderStyle.Solid;
            arsaCell.BorderWidth = 2;
            arsaCell.BorderColor = System.Drawing.Color.Black;
            arsaCell.HorizontalAlign = HorizontalAlign.Center;
            arsaCell.Font.Bold = true;
            arsaCell.Text = "Arsa";

            TableHeaderCell tarlaCell = new TableHeaderCell();
            tarlaCell.ColumnSpan = 3;
            tarlaCell.BorderStyle = BorderStyle.Solid;
            tarlaCell.BorderWidth = 2;
            tarlaCell.BorderColor = System.Drawing.Color.Black;
            tarlaCell.HorizontalAlign = HorizontalAlign.Center;
            tarlaCell.Font.Bold = true;
            tarlaCell.Text = "Tarla";

            TableHeaderCell bisCell = new TableHeaderCell();
            bisCell.ColumnSpan = 3;
            bisCell.BorderStyle = BorderStyle.Solid;
            bisCell.BorderWidth = 2;
            bisCell.BorderColor = System.Drawing.Color.Black;
            bisCell.HorizontalAlign = HorizontalAlign.Center;
            bisCell.Font.Bold = true;
            bisCell.Text = "Bis";

            TableHeaderCell tesisCell = new TableHeaderCell();
            tesisCell.ColumnSpan = 3;
            tesisCell.BorderStyle = BorderStyle.Solid;
            tesisCell.BorderWidth = 2;
            tesisCell.BorderColor = System.Drawing.Color.Black;
            tesisCell.HorizontalAlign = HorizontalAlign.Center;
            tesisCell.Font.Bold = true;
            tesisCell.Text = "Tesis";

            //3rd header row

            //TableHeaderCell kiraciAdetCell = new TableHeaderCell();
            //kiraciAdetCell.BorderStyle = BorderStyle.Solid;
            //kiraciAdetCell.BorderWidth = 2;
            //kiraciAdetCell.BorderColor = System.Drawing.Color.Black;
            //kiraciAdetCell.HorizontalAlign = HorizontalAlign.Center;
            //kiraciAdetCell.Font.Bold = true;
            //kiraciAdetCell.Text = "Kiraci Sayisi";
            //TableHeaderCell kiraciOdeyenCell = new TableHeaderCell();
            //kiraciOdeyenCell.BorderStyle = BorderStyle.Solid;
            //kiraciOdeyenCell.BorderWidth = 2;
            //kiraciOdeyenCell.BorderColor = System.Drawing.Color.Black;
            //kiraciOdeyenCell.HorizontalAlign = HorizontalAlign.Center;
            //kiraciOdeyenCell.Font.Bold = true;
            //kiraciOdeyenCell.Text = "Ödeyen Sayisi";

            TableHeaderCell meskenTahakkukCell = new TableHeaderCell();
            meskenTahakkukCell.BorderStyle = BorderStyle.Solid;
            meskenTahakkukCell.BorderWidth = 2;
            meskenTahakkukCell.BorderColor = System.Drawing.Color.Black;
            meskenTahakkukCell.HorizontalAlign = HorizontalAlign.Center;
            meskenTahakkukCell.Font.Bold = true;
            meskenTahakkukCell.Text = "Tahakkuk";
            TableHeaderCell meskenTahsilCell = new TableHeaderCell();
            meskenTahsilCell.BorderStyle = BorderStyle.Solid;
            meskenTahsilCell.BorderWidth = 2;
            meskenTahsilCell.BorderColor = System.Drawing.Color.Black;
            meskenTahsilCell.HorizontalAlign = HorizontalAlign.Center;
            meskenTahsilCell.Font.Bold = true;
            meskenTahsilCell.Text = "Tahsil";
            TableHeaderCell meskenOranCell = new TableHeaderCell();
            meskenOranCell.BorderStyle = BorderStyle.Solid;
            meskenOranCell.BorderWidth = 2;
            meskenOranCell.BorderColor = System.Drawing.Color.Black;
            meskenOranCell.HorizontalAlign = HorizontalAlign.Center;
            meskenOranCell.Font.Bold = true;
            meskenOranCell.Text = "Oran";

            TableHeaderCell isyeriTahakkukCell = new TableHeaderCell();
            isyeriTahakkukCell.BorderStyle = BorderStyle.Solid;
            isyeriTahakkukCell.BorderWidth = 2;
            isyeriTahakkukCell.BorderColor = System.Drawing.Color.Black;
            isyeriTahakkukCell.HorizontalAlign = HorizontalAlign.Center;
            isyeriTahakkukCell.Font.Bold = true;
            isyeriTahakkukCell.Text = "Tahakkuk";
            TableHeaderCell isyeriTahsilCell = new TableHeaderCell();
            isyeriTahsilCell.BorderStyle = BorderStyle.Solid;
            isyeriTahsilCell.BorderWidth = 2;
            isyeriTahsilCell.BorderColor = System.Drawing.Color.Black;
            isyeriTahsilCell.HorizontalAlign = HorizontalAlign.Center;
            isyeriTahsilCell.Font.Bold = true;
            isyeriTahsilCell.Text = "Tahsil";
            TableHeaderCell isyeriOranCell = new TableHeaderCell();
            isyeriOranCell.BorderStyle = BorderStyle.Solid;
            isyeriOranCell.BorderWidth = 2;
            isyeriOranCell.BorderColor = System.Drawing.Color.Black;
            isyeriOranCell.HorizontalAlign = HorizontalAlign.Center;
            isyeriOranCell.Font.Bold = true;
            isyeriOranCell.Text = "Oran";

            TableHeaderCell arsaTahakkukCell = new TableHeaderCell();
            arsaTahakkukCell.BorderStyle = BorderStyle.Solid;
            arsaTahakkukCell.BorderWidth = 2;
            arsaTahakkukCell.BorderColor = System.Drawing.Color.Black;
            arsaTahakkukCell.HorizontalAlign = HorizontalAlign.Center;
            arsaTahakkukCell.Font.Bold = true;
            arsaTahakkukCell.Text = "Tahakkuk";
            TableHeaderCell arsaTahsilCell = new TableHeaderCell();
            arsaTahsilCell.BorderStyle = BorderStyle.Solid;
            arsaTahsilCell.BorderWidth = 2;
            arsaTahsilCell.BorderColor = System.Drawing.Color.Black;
            arsaTahsilCell.HorizontalAlign = HorizontalAlign.Center;
            arsaTahsilCell.Font.Bold = true;
            arsaTahsilCell.Text = "Tahsil";
            TableHeaderCell arsaOranCell = new TableHeaderCell();
            arsaOranCell.BorderStyle = BorderStyle.Solid;
            arsaOranCell.BorderWidth = 2;
            arsaOranCell.BorderColor = System.Drawing.Color.Black;
            arsaOranCell.HorizontalAlign = HorizontalAlign.Center;
            arsaOranCell.Font.Bold = true;
            arsaOranCell.Text = "Oran";

            TableHeaderCell tarlaTahakkukCell = new TableHeaderCell();
            tarlaTahakkukCell.BorderStyle = BorderStyle.Solid;
            tarlaTahakkukCell.BorderWidth = 2;
            tarlaTahakkukCell.BorderColor = System.Drawing.Color.Black;
            tarlaTahakkukCell.HorizontalAlign = HorizontalAlign.Center;
            tarlaTahakkukCell.Font.Bold = true;
            tarlaTahakkukCell.Text = "Tahakkuk";
            TableHeaderCell tarlaTahsilCell = new TableHeaderCell();
            tarlaTahsilCell.BorderStyle = BorderStyle.Solid;
            tarlaTahsilCell.BorderWidth = 2;
            tarlaTahsilCell.BorderColor = System.Drawing.Color.Black;
            tarlaTahsilCell.HorizontalAlign = HorizontalAlign.Center;
            tarlaTahsilCell.Font.Bold = true;
            tarlaTahsilCell.Text = "Tahsil";
            TableHeaderCell tarlaOranCell = new TableHeaderCell();
            tarlaOranCell.BorderStyle = BorderStyle.Solid;
            tarlaOranCell.BorderWidth = 2;
            tarlaOranCell.BorderColor = System.Drawing.Color.Black;
            tarlaOranCell.HorizontalAlign = HorizontalAlign.Center;
            tarlaOranCell.Font.Bold = true;
            tarlaOranCell.Text = "Oran";

            TableHeaderCell bisTahakkukCell = new TableHeaderCell();
            bisTahakkukCell.BorderStyle = BorderStyle.Solid;
            bisTahakkukCell.BorderWidth = 2;
            bisTahakkukCell.BorderColor = System.Drawing.Color.Black;
            bisTahakkukCell.HorizontalAlign = HorizontalAlign.Center;
            bisTahakkukCell.Font.Bold = true;
            bisTahakkukCell.Text = "Tahakkuk";
            TableHeaderCell bisTahsilCell = new TableHeaderCell();
            bisTahsilCell.BorderStyle = BorderStyle.Solid;
            bisTahsilCell.BorderWidth = 2;
            bisTahsilCell.BorderColor = System.Drawing.Color.Black;
            bisTahsilCell.HorizontalAlign = HorizontalAlign.Center;
            bisTahsilCell.Font.Bold = true;
            bisTahsilCell.Text = "Tahsil";
            TableHeaderCell bisOranCell = new TableHeaderCell();
            bisOranCell.BorderStyle = BorderStyle.Solid;
            bisOranCell.BorderWidth = 2;
            bisOranCell.BorderColor = System.Drawing.Color.Black;
            bisOranCell.HorizontalAlign = HorizontalAlign.Center;
            bisOranCell.Font.Bold = true;
            bisOranCell.Text = "Oran";

            TableHeaderCell tesisTahakkukCell = new TableHeaderCell();
            tesisTahakkukCell.BorderStyle = BorderStyle.Solid;
            tesisTahakkukCell.BorderWidth = 2;
            tesisTahakkukCell.BorderColor = System.Drawing.Color.Black;
            tesisTahakkukCell.HorizontalAlign = HorizontalAlign.Center;
            tesisTahakkukCell.Font.Bold = true;
            tesisTahakkukCell.Text = "Tahakkuk";
            TableHeaderCell tesisTahsilCell = new TableHeaderCell();
            tesisTahsilCell.BorderStyle = BorderStyle.Solid;
            tesisTahsilCell.BorderWidth = 2;
            tesisTahsilCell.BorderColor = System.Drawing.Color.Black;
            tesisTahsilCell.HorizontalAlign = HorizontalAlign.Center;
            tesisTahsilCell.Font.Bold = true;
            tesisTahsilCell.Text = "Tahsil";
            TableHeaderCell tesisOranCell = new TableHeaderCell();
            tesisOranCell.BorderStyle = BorderStyle.Solid;
            tesisOranCell.BorderWidth = 2;
            tesisOranCell.BorderColor = System.Drawing.Color.Black;
            tesisOranCell.HorizontalAlign = HorizontalAlign.Center;
            tesisOranCell.Font.Bold = true;
            tesisOranCell.Text = "Oran";

            TableHeaderCell toplamTahakkukCell = new TableHeaderCell();
            toplamTahakkukCell.BorderStyle = BorderStyle.Solid;
            toplamTahakkukCell.BorderWidth = 2;
            toplamTahakkukCell.BorderColor = System.Drawing.Color.Black;
            toplamTahakkukCell.HorizontalAlign = HorizontalAlign.Center;
            toplamTahakkukCell.Font.Bold = true;
            toplamTahakkukCell.Text = "Tahakkuk";
            TableHeaderCell toplamTahsilCell = new TableHeaderCell();
            toplamTahsilCell.BorderStyle = BorderStyle.Solid;
            toplamTahsilCell.BorderWidth = 2;
            toplamTahsilCell.BorderColor = System.Drawing.Color.Black;
            toplamTahsilCell.HorizontalAlign = HorizontalAlign.Center;
            toplamTahsilCell.Font.Bold = true;
            toplamTahsilCell.Text = "Tahsil";
            TableHeaderCell toplamOranCell = new TableHeaderCell();
            toplamOranCell.BorderStyle = BorderStyle.Solid;
            toplamOranCell.BorderWidth = 2;
            toplamOranCell.BorderColor = System.Drawing.Color.Black;
            toplamOranCell.HorizontalAlign = HorizontalAlign.Center;
            toplamOranCell.Font.Bold = true;
            toplamOranCell.Text = "Oran";

            tableHeaderRow1.Controls.Add(meskenCell);
            tableHeaderRow1.Controls.Add(isyeriCell);
            tableHeaderRow1.Controls.Add(arsaCell);
            tableHeaderRow1.Controls.Add(tarlaCell);
            tableHeaderRow1.Controls.Add(bisCell);
            tableHeaderRow1.Controls.Add(tesisCell);

            //tableHeaderRow2.Controls.Add(kiraciAdetCell);
            //tableHeaderRow2.Controls.Add(kiraciOdeyenCell);
            tableHeaderRow2.Controls.Add(meskenTahakkukCell);
            tableHeaderRow2.Controls.Add(meskenTahsilCell);
            tableHeaderRow2.Controls.Add(meskenOranCell);            
            tableHeaderRow2.Controls.Add(isyeriTahakkukCell);
            tableHeaderRow2.Controls.Add(isyeriTahsilCell);
            tableHeaderRow2.Controls.Add(isyeriOranCell);            
            tableHeaderRow2.Controls.Add(arsaTahakkukCell);
            tableHeaderRow2.Controls.Add(arsaTahsilCell);
            tableHeaderRow2.Controls.Add(arsaOranCell);            
            tableHeaderRow2.Controls.Add(tarlaTahakkukCell);
            tableHeaderRow2.Controls.Add(tarlaTahsilCell);
            tableHeaderRow2.Controls.Add(tarlaOranCell);
            tableHeaderRow2.Controls.Add(bisTahakkukCell);
            tableHeaderRow2.Controls.Add(bisTahsilCell);
            tableHeaderRow2.Controls.Add(bisOranCell);            
            tableHeaderRow2.Controls.Add(tesisTahakkukCell);
            tableHeaderRow2.Controls.Add(tesisTahsilCell);
            tableHeaderRow2.Controls.Add(tesisOranCell);            
            tableHeaderRow2.Controls.Add(toplamTahakkukCell);
            tableHeaderRow2.Controls.Add(toplamTahsilCell);
            tableHeaderRow2.Controls.Add(toplamOranCell);


            KiraGelirleriTable.Controls.Add(tableHeaderRow);
            KiraGelirleriTable.Controls.Add(tableHeaderRow1);
            KiraGelirleriTable.Controls.Add(tableHeaderRow2);
        }

        private void YilToplamiHesaplaVeTabloyaEkle(int yil,
            decimal arsaTahakkukToplam, decimal bisTahakkukToplam, decimal isyeriTahakkukToplam, decimal meskenTahakkukToplam, decimal tarlaTahakkukToplam, decimal tesisTahakkukToplam,
            decimal arsaTahsilToplam, decimal bisTahsilToplam, decimal isyeriTahsilToplam, decimal meskenTahsilToplam, decimal tarlaTahsilToplam, decimal tesisTahsilToplam)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            decimal arsaOranToplam = arsaTahsilToplam * 100 / (arsaTahakkukToplam == 0 ? 1 : arsaTahakkukToplam);
            decimal bisOranToplam = bisTahsilToplam * 100 / (bisTahakkukToplam == 0 ? 1 : bisTahakkukToplam);
            decimal isyeriOranToplam = isyeriTahsilToplam * 100 / (isyeriTahakkukToplam == 0 ? 1 : isyeriTahakkukToplam);
            decimal meskenOranToplam = meskenTahsilToplam * 100 / (meskenTahakkukToplam == 0 ? 1 : meskenTahakkukToplam);
            decimal tarlaOranToplam = tarlaTahsilToplam * 100 / (tarlaTahakkukToplam == 0 ? 1 : tarlaTahakkukToplam);
            decimal tesisOranToplam = tesisTahsilToplam * 100 / (tesisTahakkukToplam == 0 ? 1 : tesisTahakkukToplam);

            decimal ayTahakkukToplam = meskenTahakkukToplam + isyeriTahakkukToplam + arsaTahakkukToplam + tarlaTahakkukToplam + bisTahakkukToplam + tesisTahakkukToplam;
            decimal ayTahsilToplam = meskenTahsilToplam + isyeriTahsilToplam + arsaTahsilToplam + tarlaTahsilToplam + bisTahsilToplam + tesisTahsilToplam;
            decimal ayOranToplam = ayTahsilToplam * 100 / (ayTahakkukToplam == 0 ? 1 : ayTahakkukToplam);

            TableRow tableRow = new TableRow();
            TableCell ayToplamLabelCell = new TableCell();
            ayToplamLabelCell.ColumnSpan = 4;
            ayToplamLabelCell.BorderStyle = BorderStyle.Solid;
            ayToplamLabelCell.BorderWidth = 3;
            ayToplamLabelCell.BorderColor = System.Drawing.Color.Black;
            ayToplamLabelCell.BackColor = System.Drawing.Color.Gray;
            ayToplamLabelCell.HorizontalAlign = HorizontalAlign.Right;
            ayToplamLabelCell.Font.Bold = true;
            ayToplamLabelCell.Text = yil + " Toplami";

            TableCell meskenTahakkukToplamCell = new TableCell();
            meskenTahakkukToplamCell.BorderStyle = BorderStyle.Solid;
            meskenTahakkukToplamCell.BorderWidth = 3;
            meskenTahakkukToplamCell.BorderColor = System.Drawing.Color.Black;
            meskenTahakkukToplamCell.BackColor = System.Drawing.Color.Gray;
            meskenTahakkukToplamCell.HorizontalAlign = HorizontalAlign.Right;
            meskenTahakkukToplamCell.Font.Bold = true;
            meskenTahakkukToplamCell.Text = meskenTahakkukToplam.ToString("N", culturInfo);

            TableCell meskenTahsilToplamCell = new TableCell();
            meskenTahsilToplamCell.BorderStyle = BorderStyle.Solid;
            meskenTahsilToplamCell.BorderWidth = 3;
            meskenTahsilToplamCell.BorderColor = System.Drawing.Color.Black;
            meskenTahsilToplamCell.BackColor = System.Drawing.Color.Gray;
            meskenTahsilToplamCell.HorizontalAlign = HorizontalAlign.Right;
            meskenTahsilToplamCell.Font.Bold = true;
            meskenTahsilToplamCell.Text = meskenTahsilToplam.ToString("N", culturInfo);
            
            TableCell meskenOranToplamCell = new TableCell();
            meskenOranToplamCell.BorderStyle = BorderStyle.Solid;
            meskenOranToplamCell.BorderWidth = 3;
            meskenOranToplamCell.BorderColor = System.Drawing.Color.Black;
            meskenOranToplamCell.BackColor = System.Drawing.Color.Gray;
            meskenOranToplamCell.HorizontalAlign = HorizontalAlign.Right;
            meskenOranToplamCell.Font.Bold = true; 
            meskenOranToplamCell.Text = meskenOranToplam.ToString("N", culturInfo);

            TableCell isyeriTahakkukToplamCell = new TableCell();
            isyeriTahakkukToplamCell.BorderStyle = BorderStyle.Solid;
            isyeriTahakkukToplamCell.BorderWidth = 3;
            isyeriTahakkukToplamCell.BorderColor = System.Drawing.Color.Black;
            isyeriTahakkukToplamCell.BackColor = System.Drawing.Color.Gray;
            isyeriTahakkukToplamCell.HorizontalAlign = HorizontalAlign.Right;
            isyeriTahakkukToplamCell.Font.Bold = true;
            isyeriTahakkukToplamCell.Text = isyeriTahakkukToplam.ToString("N", culturInfo);

            TableCell isyeriTahsilToplamCell = new TableCell();
            isyeriTahsilToplamCell.BorderStyle = BorderStyle.Solid;
            isyeriTahsilToplamCell.BorderWidth = 3;
            isyeriTahsilToplamCell.BorderColor = System.Drawing.Color.Black;
            isyeriTahsilToplamCell.BackColor = System.Drawing.Color.Gray;
            isyeriTahsilToplamCell.HorizontalAlign = HorizontalAlign.Right;
            isyeriTahsilToplamCell.Font.Bold = true;
            isyeriTahsilToplamCell.Text = isyeriTahsilToplam.ToString("N", culturInfo); 
            
            TableCell isyeriOranToplamCell = new TableCell();
            isyeriOranToplamCell.BorderStyle = BorderStyle.Solid;
            isyeriOranToplamCell.BorderWidth = 3;
            isyeriOranToplamCell.BorderColor = System.Drawing.Color.Black;
            isyeriOranToplamCell.BackColor = System.Drawing.Color.Gray;
            isyeriOranToplamCell.HorizontalAlign = HorizontalAlign.Right;
            isyeriOranToplamCell.Font.Bold = true;
            isyeriOranToplamCell.Text = isyeriOranToplam.ToString("N", culturInfo);

            TableCell arsaTahakkukToplamCell = new TableCell();
            arsaTahakkukToplamCell.BorderStyle = BorderStyle.Solid;
            arsaTahakkukToplamCell.BorderWidth = 3;
            arsaTahakkukToplamCell.BorderColor = System.Drawing.Color.Black;
            arsaTahakkukToplamCell.BackColor = System.Drawing.Color.Gray;
            arsaTahakkukToplamCell.HorizontalAlign = HorizontalAlign.Right;
            arsaTahakkukToplamCell.Font.Bold = true;
            arsaTahakkukToplamCell.Text = arsaTahakkukToplam.ToString("N", culturInfo);

            TableCell arsaTahsilToplamCell = new TableCell();
            arsaTahsilToplamCell.BorderStyle = BorderStyle.Solid;
            arsaTahsilToplamCell.BorderWidth = 3;
            arsaTahsilToplamCell.BorderColor = System.Drawing.Color.Black;
            arsaTahsilToplamCell.BackColor = System.Drawing.Color.Gray;
            arsaTahsilToplamCell.HorizontalAlign = HorizontalAlign.Right;
            arsaTahsilToplamCell.Font.Bold = true;
            arsaTahsilToplamCell.Text = arsaTahsilToplam.ToString("N", culturInfo);
            
            TableCell arsaOranToplamCell = new TableCell();
            arsaOranToplamCell.BorderStyle = BorderStyle.Solid;
            arsaOranToplamCell.BorderWidth = 3;
            arsaOranToplamCell.BorderColor = System.Drawing.Color.Black;
            arsaOranToplamCell.BackColor = System.Drawing.Color.Gray;
            arsaOranToplamCell.HorizontalAlign = HorizontalAlign.Right;
            arsaOranToplamCell.Font.Bold = true;
            arsaOranToplamCell.Text = arsaOranToplam.ToString("N", culturInfo);

            TableCell tarlaTahakkukToplamCell = new TableCell();
            tarlaTahakkukToplamCell.BorderStyle = BorderStyle.Solid;
            tarlaTahakkukToplamCell.BorderWidth = 3;
            tarlaTahakkukToplamCell.BorderColor = System.Drawing.Color.Black;
            tarlaTahakkukToplamCell.BackColor = System.Drawing.Color.Gray;
            tarlaTahakkukToplamCell.HorizontalAlign = HorizontalAlign.Right;
            tarlaTahakkukToplamCell.Font.Bold = true;
            tarlaTahakkukToplamCell.Text = tarlaTahakkukToplam.ToString("N", culturInfo);

            TableCell tarlaTahsilToplamCell = new TableCell();
            tarlaTahsilToplamCell.BorderStyle = BorderStyle.Solid;
            tarlaTahsilToplamCell.BorderWidth = 3;
            tarlaTahsilToplamCell.BorderColor = System.Drawing.Color.Black;
            tarlaTahsilToplamCell.BackColor = System.Drawing.Color.Gray;
            tarlaTahsilToplamCell.HorizontalAlign = HorizontalAlign.Right;
            tarlaTahsilToplamCell.Font.Bold = true;
            tarlaTahsilToplamCell.Text = tarlaTahsilToplam.ToString("N", culturInfo);
            
            TableCell tarlaOranToplamCell = new TableCell();
            tarlaOranToplamCell.BorderStyle = BorderStyle.Solid;
            tarlaOranToplamCell.BorderWidth = 3;
            tarlaOranToplamCell.BorderColor = System.Drawing.Color.Black;
            tarlaOranToplamCell.BackColor = System.Drawing.Color.Gray;
            tarlaOranToplamCell.HorizontalAlign = HorizontalAlign.Right;
            tarlaOranToplamCell.Font.Bold = true;
            tarlaOranToplamCell.Text = tarlaOranToplam.ToString("N", culturInfo);

            TableCell bisTahakkukToplamCell = new TableCell();
            bisTahakkukToplamCell.BorderStyle = BorderStyle.Solid;
            bisTahakkukToplamCell.BorderWidth = 3;
            bisTahakkukToplamCell.BorderColor = System.Drawing.Color.Black;
            bisTahakkukToplamCell.BackColor = System.Drawing.Color.Gray;
            bisTahakkukToplamCell.HorizontalAlign = HorizontalAlign.Right;
            bisTahakkukToplamCell.Font.Bold = true;
            bisTahakkukToplamCell.Text = bisTahakkukToplam.ToString("N", culturInfo);

            TableCell bisTahsilToplamCell = new TableCell();
            bisTahsilToplamCell.BorderStyle = BorderStyle.Solid;
            bisTahsilToplamCell.BorderWidth = 3;
            bisTahsilToplamCell.BorderColor = System.Drawing.Color.Black;
            bisTahsilToplamCell.BackColor = System.Drawing.Color.Gray;
            bisTahsilToplamCell.HorizontalAlign = HorizontalAlign.Right;
            bisTahsilToplamCell.Font.Bold = true;
            bisTahsilToplamCell.Text = bisTahsilToplam.ToString("N", culturInfo);

            TableCell bisOranToplamCell = new TableCell();
            bisOranToplamCell.BorderStyle = BorderStyle.Solid;
            bisOranToplamCell.BorderWidth = 3;
            bisOranToplamCell.BorderColor = System.Drawing.Color.Black;
            bisOranToplamCell.BackColor = System.Drawing.Color.Gray;
            bisOranToplamCell.HorizontalAlign = HorizontalAlign.Right;
            bisOranToplamCell.Font.Bold = true;
            bisOranToplamCell.Text = bisOranToplam.ToString("N", culturInfo);

            TableCell tesisTahakkukToplamCell = new TableCell();
            tesisTahakkukToplamCell.BorderStyle = BorderStyle.Solid;
            tesisTahakkukToplamCell.BorderWidth = 3;
            tesisTahakkukToplamCell.BorderColor = System.Drawing.Color.Black;
            tesisTahakkukToplamCell.BackColor = System.Drawing.Color.Gray;
            tesisTahakkukToplamCell.HorizontalAlign = HorizontalAlign.Right;
            tesisTahakkukToplamCell.Font.Bold = true;
            tesisTahakkukToplamCell.Text = tesisTahakkukToplam.ToString("N", culturInfo);

            TableCell tesisTahsilToplamCell = new TableCell();
            tesisTahsilToplamCell.BorderStyle = BorderStyle.Solid;
            tesisTahsilToplamCell.BorderWidth = 3;
            tesisTahsilToplamCell.BorderColor = System.Drawing.Color.Black;
            tesisTahsilToplamCell.BackColor = System.Drawing.Color.Gray;
            tesisTahsilToplamCell.HorizontalAlign = HorizontalAlign.Right;
            tesisTahsilToplamCell.Font.Bold = true;
            tesisTahsilToplamCell.Text = tesisTahsilToplam.ToString("N", culturInfo);
            
            TableCell tesisOranToplamCell = new TableCell();
            tesisOranToplamCell.BorderStyle = BorderStyle.Solid;
            tesisOranToplamCell.BorderWidth = 3;
            tesisOranToplamCell.BorderColor = System.Drawing.Color.Black;
            tesisOranToplamCell.BackColor = System.Drawing.Color.Gray;
            tesisOranToplamCell.HorizontalAlign = HorizontalAlign.Right;
            tesisOranToplamCell.Font.Bold = true;
            tesisOranToplamCell.Text = tesisOranToplam.ToString("N", culturInfo);

            TableCell ayTahakkukToplamCell = new TableCell();
            ayTahakkukToplamCell.BorderStyle = BorderStyle.Solid;
            ayTahakkukToplamCell.BorderWidth = 3;
            ayTahakkukToplamCell.BorderColor = System.Drawing.Color.Black;
            ayTahakkukToplamCell.BackColor = System.Drawing.Color.Gray;
            ayTahakkukToplamCell.HorizontalAlign = HorizontalAlign.Right;
            ayTahakkukToplamCell.Font.Bold = true;
            ayTahakkukToplamCell.Text = ayTahakkukToplam.ToString("N", culturInfo);

            TableCell ayTahsilToplamCell = new TableCell();
            ayTahsilToplamCell.BorderStyle = BorderStyle.Solid;
            ayTahsilToplamCell.BorderWidth = 3;
            ayTahsilToplamCell.BorderColor = System.Drawing.Color.Black;
            ayTahsilToplamCell.BackColor = System.Drawing.Color.Gray;
            ayTahsilToplamCell.HorizontalAlign = HorizontalAlign.Right;
            ayTahsilToplamCell.Font.Bold = true;
            ayTahsilToplamCell.Text = ayTahsilToplam.ToString("N", culturInfo);

            TableCell ayOranToplamCell = new TableCell();
            ayOranToplamCell.BorderStyle = BorderStyle.Solid;
            ayOranToplamCell.BorderWidth = 3;
            ayOranToplamCell.BorderColor = System.Drawing.Color.Black;
            ayOranToplamCell.BackColor = System.Drawing.Color.Gray;
            ayOranToplamCell.HorizontalAlign = HorizontalAlign.Right;
            ayOranToplamCell.Font.Bold = true;
            ayOranToplamCell.Text = ayOranToplam.ToString("N", culturInfo);


            tableRow.Controls.Add(ayToplamLabelCell);
            tableRow.Controls.Add(meskenTahakkukToplamCell);
            tableRow.Controls.Add(meskenTahsilToplamCell);
            tableRow.Controls.Add(meskenOranToplamCell);
            tableRow.Controls.Add(isyeriTahakkukToplamCell);
            tableRow.Controls.Add(isyeriTahsilToplamCell);
            tableRow.Controls.Add(isyeriOranToplamCell);
            tableRow.Controls.Add(arsaTahakkukToplamCell);
            tableRow.Controls.Add(arsaTahsilToplamCell);
            tableRow.Controls.Add(arsaOranToplamCell);

            tableRow.Controls.Add(tarlaTahakkukToplamCell);
            tableRow.Controls.Add(tarlaTahsilToplamCell);
            tableRow.Controls.Add(tarlaOranToplamCell);
            tableRow.Controls.Add(bisTahakkukToplamCell);
            tableRow.Controls.Add(bisTahsilToplamCell);
            tableRow.Controls.Add(bisOranToplamCell);
            tableRow.Controls.Add(tesisTahakkukToplamCell);
            tableRow.Controls.Add(tesisTahsilToplamCell);
            tableRow.Controls.Add(tesisOranToplamCell);
            tableRow.Controls.Add(ayTahakkukToplamCell);
            tableRow.Controls.Add(ayTahsilToplamCell);
            tableRow.Controls.Add(ayOranToplamCell);
            KiraGelirleriTable.Controls.Add(tableRow);
        }
        private void AyToplamiHesaplaVeTabloyaEkle(int ay, int yil, int kiraciSayisiToplam, int odeyenKiraciSayisiToplam,
            decimal arsaTahakkukToplam, decimal bisTahakkukToplam, decimal isyeriTahakkukToplam, decimal meskenTahakkukToplam, decimal tarlaTahakkukToplam, decimal tesisTahakkukToplam,
            decimal arsaTahsilToplam, decimal bisTahsilToplam, decimal isyeriTahsilToplam, decimal meskenTahsilToplam,   decimal tarlaTahsilToplam, decimal tesisTahsilToplam)
        {

            decimal arsaOranToplam = arsaTahsilToplam * 100 / (arsaTahakkukToplam==0? 1: arsaTahakkukToplam);
            decimal bisOranToplam = bisTahsilToplam * 100 / (bisTahakkukToplam==0? 1: bisTahakkukToplam);
            decimal isyeriOranToplam = isyeriTahsilToplam * 100 / (isyeriTahakkukToplam == 0 ? 1 : isyeriTahakkukToplam);
            decimal meskenOranToplam = meskenTahsilToplam * 100 / (meskenTahakkukToplam==0? 1: meskenTahakkukToplam);
            decimal tarlaOranToplam = tarlaTahsilToplam * 100 / (tarlaTahakkukToplam == 0 ? 1 : tarlaTahakkukToplam);
            decimal tesisOranToplam = tesisTahsilToplam * 100 / (tesisTahakkukToplam==0? 1: tesisTahakkukToplam);

            decimal ayTahakkukToplami = arsaTahakkukToplam + bisTahakkukToplam + isyeriTahakkukToplam + meskenTahakkukToplam + tarlaTahakkukToplam +  tesisTahakkukToplam;
            decimal ayTahsilToplami = arsaTahsilToplam + bisTahsilToplam + isyeriTahsilToplam + meskenTahsilToplam + tarlaTahsilToplam + tesisTahsilToplam;
            decimal ayOranToplami = ayTahsilToplami * 100 / (ayTahakkukToplami == 0 ? 1 : ayTahakkukToplami);

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
            ayToplamLabelCell.Text = tarih.ToString("MMMM", culturInfo) + " Toplami";


            TableCell kiraciSayisiToplamCell = new TableCell();
            kiraciSayisiToplamCell.BorderStyle = BorderStyle.Solid;
            kiraciSayisiToplamCell.BorderWidth = 3;
            kiraciSayisiToplamCell.BorderColor = System.Drawing.Color.Black;
            kiraciSayisiToplamCell.BackColor = System.Drawing.Color.LightGray;
            kiraciSayisiToplamCell.HorizontalAlign = HorizontalAlign.Right;
            kiraciSayisiToplamCell.Font.Bold = true;
            kiraciSayisiToplamCell.Text = kiraciSayisiToplam.ToString();

            //TableCell odeyenKiraciSayisiToplamCell = new TableCell();
            //odeyenKiraciSayisiToplamCell.BorderStyle = BorderStyle.Solid;
            //odeyenKiraciSayisiToplamCell.BorderWidth = 3;
            //odeyenKiraciSayisiToplamCell.BorderColor = System.Drawing.Color.Black;
            //odeyenKiraciSayisiToplamCell.BackColor = System.Drawing.Color.LightGray;
            //odeyenKiraciSayisiToplamCell.HorizontalAlign = HorizontalAlign.Right;
            //odeyenKiraciSayisiToplamCell.Font.Bold = true;
            //odeyenKiraciSayisiToplamCell.Text = odeyenKiraciSayisiToplam.ToString();

            TableCell arsaTahakkukToplamCell = new TableCell();
            arsaTahakkukToplamCell.BorderStyle = BorderStyle.Solid;
            arsaTahakkukToplamCell.BorderWidth = 3;
            arsaTahakkukToplamCell.BorderColor = System.Drawing.Color.Black;
            arsaTahakkukToplamCell.BackColor = System.Drawing.Color.LightGray;
            arsaTahakkukToplamCell.HorizontalAlign = HorizontalAlign.Right;
            arsaTahakkukToplamCell.Font.Bold = true;
            arsaTahakkukToplamCell.Text = arsaTahakkukToplam.ToString("N", culturInfo);

            TableCell arsaTahsilToplamCell = new TableCell();
            arsaTahsilToplamCell.BorderStyle = BorderStyle.Solid;
            arsaTahsilToplamCell.BorderWidth = 3;
            arsaTahsilToplamCell.BorderColor = System.Drawing.Color.Black;
            arsaTahsilToplamCell.BackColor = System.Drawing.Color.LightGray;
            arsaTahsilToplamCell.HorizontalAlign = HorizontalAlign.Right;
            arsaTahsilToplamCell.Font.Bold = true;
            arsaTahsilToplamCell.Text = arsaTahsilToplam.ToString("N", culturInfo);

            TableCell arsaOranToplamCell = new TableCell();
            arsaOranToplamCell.BorderStyle = BorderStyle.Solid;
            arsaOranToplamCell.BorderWidth = 3;
            arsaOranToplamCell.BorderColor = System.Drawing.Color.Black;
            arsaOranToplamCell.BackColor = System.Drawing.Color.LightGray;
            arsaOranToplamCell.HorizontalAlign = HorizontalAlign.Right;
            arsaOranToplamCell.Font.Bold = true;
            arsaOranToplamCell.Text = arsaOranToplam.ToString("N", culturInfo);

            TableCell bisTahakkukToplamCell = new TableCell();
            bisTahakkukToplamCell.BorderStyle = BorderStyle.Solid;
            bisTahakkukToplamCell.BorderWidth = 3;
            bisTahakkukToplamCell.BorderColor = System.Drawing.Color.Black;
            bisTahakkukToplamCell.BackColor = System.Drawing.Color.LightGray;
            bisTahakkukToplamCell.HorizontalAlign = HorizontalAlign.Right;
            bisTahakkukToplamCell.Font.Bold = true;
            bisTahakkukToplamCell.Text = bisTahakkukToplam.ToString("N", culturInfo);

            TableCell bisTahsilToplamCell = new TableCell();
            bisTahsilToplamCell.BorderStyle = BorderStyle.Solid;
            bisTahsilToplamCell.BorderWidth = 3;
            bisTahsilToplamCell.BorderColor = System.Drawing.Color.Black;
            bisTahsilToplamCell.BackColor = System.Drawing.Color.LightGray;
            bisTahsilToplamCell.HorizontalAlign = HorizontalAlign.Right;
            bisTahsilToplamCell.Font.Bold = true;
            bisTahsilToplamCell.Text = bisTahsilToplam.ToString("N", culturInfo);

            TableCell bisOranToplamCell = new TableCell();
            bisOranToplamCell.BorderStyle = BorderStyle.Solid;
            bisOranToplamCell.BorderWidth = 3;
            bisOranToplamCell.BorderColor = System.Drawing.Color.Black;
            bisOranToplamCell.BackColor = System.Drawing.Color.LightGray;
            bisOranToplamCell.HorizontalAlign = HorizontalAlign.Right;
            bisOranToplamCell.Font.Bold = true;
            bisOranToplamCell.Text = bisOranToplam.ToString("N", culturInfo);

            TableCell isyeriTahakkukToplamCell = new TableCell();
            isyeriTahakkukToplamCell.BorderStyle = BorderStyle.Solid;
            isyeriTahakkukToplamCell.BorderWidth = 3;
            isyeriTahakkukToplamCell.BorderColor = System.Drawing.Color.Black;
            isyeriTahakkukToplamCell.BackColor = System.Drawing.Color.LightGray;
            isyeriTahakkukToplamCell.HorizontalAlign = HorizontalAlign.Right;
            isyeriTahakkukToplamCell.Font.Bold = true;
            isyeriTahakkukToplamCell.Text = isyeriTahakkukToplam.ToString("N", culturInfo);

            TableCell isyeriTahsilToplamCell = new TableCell();
            isyeriTahsilToplamCell.BorderStyle = BorderStyle.Solid;
            isyeriTahsilToplamCell.BorderWidth = 3;
            isyeriTahsilToplamCell.BorderColor = System.Drawing.Color.Black;
            isyeriTahsilToplamCell.BackColor = System.Drawing.Color.LightGray;
            isyeriTahsilToplamCell.HorizontalAlign = HorizontalAlign.Right;
            isyeriTahsilToplamCell.Font.Bold = true;
            isyeriTahsilToplamCell.Text = isyeriTahsilToplam.ToString("N", culturInfo);

            TableCell isyeriOranToplamCell = new TableCell();
            isyeriOranToplamCell.BorderStyle = BorderStyle.Solid;
            isyeriOranToplamCell.BorderWidth = 3;
            isyeriOranToplamCell.BorderColor = System.Drawing.Color.Black;
            isyeriOranToplamCell.BackColor = System.Drawing.Color.LightGray;
            isyeriOranToplamCell.HorizontalAlign = HorizontalAlign.Right;
            isyeriOranToplamCell.Font.Bold = true;
            isyeriOranToplamCell.Text = isyeriOranToplam.ToString("N", culturInfo);

            TableCell meskenTahakkukToplamCell = new TableCell();
            meskenTahakkukToplamCell.BorderStyle = BorderStyle.Solid;
            meskenTahakkukToplamCell.BorderWidth = 3;
            meskenTahakkukToplamCell.BorderColor = System.Drawing.Color.Black;
            meskenTahakkukToplamCell.BackColor = System.Drawing.Color.LightGray;
            meskenTahakkukToplamCell.HorizontalAlign = HorizontalAlign.Right;
            meskenTahakkukToplamCell.Font.Bold = true;
            meskenTahakkukToplamCell.Text = meskenTahakkukToplam.ToString("N", culturInfo);

           
            TableCell meskenTahsilToplamCell = new TableCell();
            meskenTahsilToplamCell.BorderStyle = BorderStyle.Solid;
            meskenTahsilToplamCell.BorderWidth = 3;
            meskenTahsilToplamCell.BorderColor = System.Drawing.Color.Black;
            meskenTahsilToplamCell.BackColor = System.Drawing.Color.LightGray;
            meskenTahsilToplamCell.HorizontalAlign = HorizontalAlign.Right;
            meskenTahsilToplamCell.Font.Bold = true;
            meskenTahsilToplamCell.Text = meskenTahsilToplam.ToString("N", culturInfo);

            TableCell meskenOranToplamCell = new TableCell();
            meskenOranToplamCell.BorderStyle = BorderStyle.Solid;
            meskenOranToplamCell.BorderWidth = 3;
            meskenOranToplamCell.BorderColor = System.Drawing.Color.Black;
            meskenOranToplamCell.BackColor = System.Drawing.Color.LightGray;
            meskenOranToplamCell.HorizontalAlign = HorizontalAlign.Right;
            meskenOranToplamCell.Font.Bold = true;
            meskenOranToplamCell.Text = meskenOranToplam.ToString("N", culturInfo);

            TableCell tesisTahakkukToplamCell = new TableCell();
            tesisTahakkukToplamCell.BorderStyle = BorderStyle.Solid;
            tesisTahakkukToplamCell.BorderWidth = 3;
            tesisTahakkukToplamCell.BorderColor = System.Drawing.Color.Black;
            tesisTahakkukToplamCell.BackColor = System.Drawing.Color.LightGray;
            tesisTahakkukToplamCell.HorizontalAlign = HorizontalAlign.Right;
            tesisTahakkukToplamCell.Font.Bold = true;
            tesisTahakkukToplamCell.Text = tesisTahakkukToplam.ToString("N", culturInfo);

            TableCell tesisTahsilToplamCell = new TableCell();
            tesisTahsilToplamCell.BorderStyle = BorderStyle.Solid;
            tesisTahsilToplamCell.BorderWidth = 3;
            tesisTahsilToplamCell.BorderColor = System.Drawing.Color.Black;
            tesisTahsilToplamCell.BackColor = System.Drawing.Color.LightGray;
            tesisTahsilToplamCell.HorizontalAlign = HorizontalAlign.Right;
            tesisTahsilToplamCell.Font.Bold = true;
            tesisTahsilToplamCell.Text = tesisTahsilToplam.ToString("N", culturInfo);

            TableCell tesisOranToplamCell = new TableCell();
            tesisOranToplamCell.BorderStyle = BorderStyle.Solid;
            tesisOranToplamCell.BorderWidth = 3;
            tesisOranToplamCell.BorderColor = System.Drawing.Color.Black;
            tesisOranToplamCell.BackColor = System.Drawing.Color.LightGray;
            tesisOranToplamCell.HorizontalAlign = HorizontalAlign.Right;
            tesisOranToplamCell.Font.Bold = true;
            tesisOranToplamCell.Text = tesisOranToplam.ToString("N", culturInfo);

            TableCell tarlaTahakkukToplamCell = new TableCell();
            tarlaTahakkukToplamCell.BorderStyle = BorderStyle.Solid;
            tarlaTahakkukToplamCell.BorderWidth = 3;
            tarlaTahakkukToplamCell.BorderColor = System.Drawing.Color.Black;
            tarlaTahakkukToplamCell.BackColor = System.Drawing.Color.LightGray;
            tarlaTahakkukToplamCell.HorizontalAlign = HorizontalAlign.Right;
            tarlaTahakkukToplamCell.Font.Bold = true;
            tarlaTahakkukToplamCell.Text = tarlaTahakkukToplam.ToString("N", culturInfo);

            TableCell tarlaTahsilToplamCell = new TableCell();
            tarlaTahsilToplamCell.BorderStyle = BorderStyle.Solid;
            tarlaTahsilToplamCell.BorderWidth = 3;
            tarlaTahsilToplamCell.BorderColor = System.Drawing.Color.Black;
            tarlaTahsilToplamCell.BackColor = System.Drawing.Color.LightGray;
            tarlaTahsilToplamCell.HorizontalAlign = HorizontalAlign.Right;
            tarlaTahsilToplamCell.Font.Bold = true;
            tarlaTahsilToplamCell.Text = tarlaTahsilToplam.ToString("N", culturInfo);

            TableCell tarlaOranToplamCell = new TableCell();
            tarlaOranToplamCell.BorderStyle = BorderStyle.Solid;
            tarlaOranToplamCell.BorderWidth = 3;
            tarlaOranToplamCell.BorderColor = System.Drawing.Color.Black;
            tarlaOranToplamCell.BackColor = System.Drawing.Color.LightGray;
            tarlaOranToplamCell.HorizontalAlign = HorizontalAlign.Right;
            tarlaOranToplamCell.Font.Bold = true;
            tarlaOranToplamCell.Text = tarlaOranToplam.ToString("N", culturInfo);

            TableCell ayTahakkukToplamCell = new TableCell();
            ayTahakkukToplamCell.BorderStyle = BorderStyle.Solid;
            ayTahakkukToplamCell.BorderWidth = 3;
            ayTahakkukToplamCell.BorderColor = System.Drawing.Color.Black;
            ayTahakkukToplamCell.BackColor = System.Drawing.Color.LightGray;
            ayTahakkukToplamCell.HorizontalAlign = HorizontalAlign.Right;
            ayTahakkukToplamCell.Font.Bold = true;
            ayTahakkukToplamCell.Text = (ayTahakkukToplami).ToString("N", culturInfo);

            TableCell ayTahsilToplamCell = new TableCell();
            ayTahsilToplamCell.BorderStyle = BorderStyle.Solid;
            ayTahsilToplamCell.BorderWidth = 3;
            ayTahsilToplamCell.BorderColor = System.Drawing.Color.Black;
            ayTahsilToplamCell.BackColor = System.Drawing.Color.LightGray;
            ayTahsilToplamCell.HorizontalAlign = HorizontalAlign.Right;
            ayTahsilToplamCell.Font.Bold = true;
            ayTahsilToplamCell.Text = (ayTahsilToplami).ToString("N", culturInfo);

            TableCell ayOranToplamCell = new TableCell();
            ayOranToplamCell.BorderStyle = BorderStyle.Solid;
            ayOranToplamCell.BorderWidth = 3;
            ayOranToplamCell.BorderColor = System.Drawing.Color.Black;
            ayOranToplamCell.BackColor = System.Drawing.Color.LightGray;
            ayOranToplamCell.HorizontalAlign = HorizontalAlign.Right;
            ayOranToplamCell.Font.Bold = true;
            ayOranToplamCell.Text = (ayOranToplami).ToString("N", culturInfo);


            tableRow.Controls.Add(ayToplamLabelCell);
            tableRow.Controls.Add(kiraciSayisiToplamCell);
            //tableRow.Controls.Add(odeyenKiraciSayisiToplamCell);
            tableRow.Controls.Add(meskenTahakkukToplamCell);
            tableRow.Controls.Add(meskenTahsilToplamCell);
            tableRow.Controls.Add(meskenOranToplamCell);
            tableRow.Controls.Add(isyeriTahakkukToplamCell);
            tableRow.Controls.Add(isyeriTahsilToplamCell);
            tableRow.Controls.Add(isyeriOranToplamCell);
            tableRow.Controls.Add(arsaTahakkukToplamCell);
            tableRow.Controls.Add(arsaTahsilToplamCell);
            tableRow.Controls.Add(arsaOranToplamCell);

            tableRow.Controls.Add(tarlaTahakkukToplamCell);
            tableRow.Controls.Add(tarlaTahsilToplamCell);
            tableRow.Controls.Add(tarlaOranToplamCell);
            tableRow.Controls.Add(bisTahakkukToplamCell);
            tableRow.Controls.Add(bisTahsilToplamCell);
            tableRow.Controls.Add(bisOranToplamCell);
            tableRow.Controls.Add(tesisTahakkukToplamCell);
            tableRow.Controls.Add(tesisTahsilToplamCell);
            tableRow.Controls.Add(tesisOranToplamCell);
            tableRow.Controls.Add(ayTahakkukToplamCell);
            tableRow.Controls.Add(ayTahsilToplamCell);
            tableRow.Controls.Add(ayOranToplamCell);
            KiraGelirleriTable.Controls.Add(tableRow);
        }

        private void AylikKiraGeliriniHesaplaVeTabloyaEkle(string bolge, bool ilkSatir, int ay, int yil, DataTable dataTable, out int toplamKiraciSayisi, out int odeyenKiraciSayisi,  
            out decimal arsaTahakkuk, out decimal bisTahakkuk, out decimal isyeriTahakkuk, out decimal meskenTahakkuk, out decimal tarlaTahakkuk, out decimal tesisTahakkuk, 
            out decimal arsaTahsil, out decimal bisTahsil, out decimal isyeriTahsil, out decimal meskenTahsil, out decimal tarlaTahsil, out decimal tesisTahsil)
        {
            TableRow tableRow = new TableRow();
            DataView dataView = new DataView(dataTable);
            toplamKiraciSayisi = 0;
            odeyenKiraciSayisi = 0;
            arsaTahsil = 0;
            arsaTahakkuk = 0;
            isyeriTahsil = 0;
            isyeriTahakkuk = 0;
            meskenTahsil = 0;
            meskenTahakkuk = 0;
            tarlaTahsil = 0;
            tarlaTahakkuk = 0;
            tesisTahsil = 0;
            tesisTahakkuk = 0;
            bisTahsil = 0;
            bisTahakkuk = 0;

            if (dataTable != null)
            {
                foreach (DataRowView row in dataView)
                {
                    string kiralamaAmaci = row == null ? "" : row["KiralamaAmaci"].ReturnEmptyIfNull().ToString();
                    //decimal odemeTutari = row == null ? 0 : row["ToplamOdemeTutari"].ConvertToDecimal();
                    //int kiraciSayisiLocal = row == null ? 0 : row["ToplamKiraciSayisi"].ConvertToInt();                    

                    int kiraciSayisi = row == null ? 0 : row["KiraciSayisi"].ReturnZeroIfNull().ConvertToInt();
                    decimal tahakkuk = row == null ? 0 : row["Tahakkuk"].ConvertToDecimal();
                    decimal tahsil = row == null ? 0 : row["Tahsil"].ConvertToDecimal();
                    int odeyenKiraciSayisiLocal = row == null ? 0 : row["OdeyenKiraci"].ConvertToInt();

                    toplamKiraciSayisi += kiraciSayisi;
                    odeyenKiraciSayisi += odeyenKiraciSayisiLocal;
                    switch (kiralamaAmaci)
                    {
                        case ProjeConstants.KIRALAMAAMACI_ARSA:
                            {
                                arsaTahsil = tahsil;
                                arsaTahakkuk = tahakkuk;
                                break;
                            }
                        case ProjeConstants.KIRALAMAAMACI_BAZISTASYONU:
                            {
                                bisTahsil = tahsil;
                                bisTahakkuk = tahakkuk;
                                break;
                            }
                        case ProjeConstants.KIRALAMAAMACI_ISYERI:
                            {
                                isyeriTahsil = tahsil;
                                isyeriTahakkuk = tahakkuk;
                                break;
                            }
                        case ProjeConstants.KIRALAMAAMACI_MESKEN:
                            {
                                meskenTahsil = tahsil;
                                meskenTahakkuk = tahakkuk;
                                break;
                            }
                        case ProjeConstants.KIRALAMAAMACI_TARLA:
                            {
                                tarlaTahsil = tahsil;
                                tarlaTahakkuk = tahakkuk;
                                break;
                            }
                        case ProjeConstants.KIRALAMAAMACI_TESIS:
                            {
                                tesisTahsil = tahsil;
                                tesisTahakkuk = tahakkuk;
                                break;
                            }
                    }
                }
            }
            TabloyaSatirEkle(tableRow, bolge, ay, yil, toplamKiraciSayisi, odeyenKiraciSayisi, ilkSatir,
                arsaTahakkuk, bisTahakkuk, isyeriTahakkuk, meskenTahakkuk, tarlaTahakkuk, tesisTahakkuk,
                arsaTahsil, bisTahsil, isyeriTahsil, meskenTahsil, tarlaTahsil, tesisTahsil);

            KiraGelirleriTable.Controls.Add(tableRow);
        }
        private TableRow TabloyaSatirEkle(TableRow tableRow, string bolge, int ay, int yil, int kiraciSayisi, int odeyenKiraciSayisi, bool ilkSatir,
            decimal arsaTahakkuk, decimal bisTahakkuk,decimal isyeriTahakkuk, decimal meskenTahakkuk, decimal tarlaTahakkuk, decimal tesisTahakkuk, 
            decimal arsaTahsil, decimal bisTahsil, decimal isyeriTahsil, decimal meskenTahsil, decimal tarlaTahsil, decimal tesisTahsil)
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
            //sifira bölme hatasi olmamasi için
            decimal arsaTahakkuk0 = arsaTahakkuk == 0 ? 1 : arsaTahakkuk;
            decimal bisTahakkuk0 = bisTahakkuk == 0 ? 1 : bisTahakkuk;
            decimal isyeriTahakkuk0 = isyeriTahakkuk == 0 ? 1 : isyeriTahakkuk;
            decimal meskenTahakkuk0 = meskenTahakkuk == 0 ? 1 : meskenTahakkuk;
            decimal tarlaTahakkuk0 = tarlaTahakkuk == 0 ? 1 : tarlaTahakkuk;
            decimal tesisTahakkuk0 = tesisTahakkuk == 0 ? 1 : tesisTahakkuk;
            
            //

            TableCell bolgeCell = new TableCell();
            bolgeCell.Text = bolge;

            TableCell kiraciSayisiCell = new TableCell();
            kiraciSayisiCell.Text = kiraciSayisi.ToString();

            //TableCell odeyenKiraciSayisiCell = new TableCell();
            //odeyenKiraciSayisiCell.Text = odeyenKiraciSayisi.ToString();

            TableCell arsaTahakkukCell = new TableCell();
            arsaTahakkukCell.Text = arsaTahakkuk.ToString("N", culturInfo);            
            TableCell arsaTahsilCell = new TableCell();
            arsaTahsilCell.Text = arsaTahsil.ToString("N", culturInfo);
            TableCell arsaOranCell = new TableCell();
            arsaOranCell.Text = (arsaTahsil*100/arsaTahakkuk0).ToString("N", culturInfo); 

            TableCell bisTahakkukCell = new TableCell();
            bisTahakkukCell.Text = bisTahakkuk.ToString("N", culturInfo);
            TableCell bisTahsilCell = new TableCell();
            bisTahsilCell.Text = bisTahsil.ToString("N", culturInfo);
            TableCell bisOranCell = new TableCell();
            bisOranCell.Text = (bisTahsil * 100 / bisTahakkuk0).ToString("N", culturInfo);

            TableCell isyeriTahakkukCell = new TableCell();
            isyeriTahakkukCell.Text = isyeriTahakkuk.ToString("N", culturInfo);            
            TableCell isyeriTahsilCell = new TableCell();
            isyeriTahsilCell.Text = isyeriTahsil.ToString("N", culturInfo);
            TableCell isyeriOranCell = new TableCell();
            isyeriOranCell.Text = (isyeriTahsil*100/isyeriTahakkuk0).ToString("N", culturInfo);
            
            TableCell meskenTahakkukCell = new TableCell();
            meskenTahakkukCell.Text = meskenTahakkuk.ToString("N", culturInfo);            
            TableCell meskenTahsilCell = new TableCell();
            meskenTahsilCell.Text = meskenTahsil.ToString("N", culturInfo);
            TableCell meskenOranCell = new TableCell();
            meskenOranCell.Text = (meskenTahsil*100/meskenTahakkuk0).ToString("N", culturInfo);
            
            TableCell tarlaTahakkukCell = new TableCell();
            tarlaTahakkukCell.Text = tarlaTahakkuk.ToString("N", culturInfo);            
            TableCell tarlaTahsilCell = new TableCell();
            tarlaTahsilCell.Text = tarlaTahsil.ToString("N", culturInfo);
            TableCell tarlaOranCell = new TableCell();
            tarlaOranCell.Text = (tarlaTahsil*100/tarlaTahakkuk0).ToString("N", culturInfo);

            TableCell tesisTahakkukCell = new TableCell();
            tesisTahakkukCell.Text = tesisTahakkuk.ToString("N", culturInfo);
            TableCell tesisTahsilCell = new TableCell();
            tesisTahsilCell.Text = tesisTahsil.ToString("N", culturInfo);
            TableCell tesisOranCell = new TableCell();
            tesisOranCell.Text = (tesisTahsil * 100 / tesisTahakkuk0).ToString("N", culturInfo);

            TableCell toplamTahakkukCell = new TableCell();
            decimal toplamTahakkuk = meskenTahakkuk + isyeriTahakkuk + arsaTahakkuk + tarlaTahakkuk + bisTahakkuk + tesisTahakkuk;
            decimal toplamTahakkuk0 = toplamTahakkuk == 0 ? 1 : toplamTahakkuk;
            toplamTahakkukCell.Text = toplamTahakkuk.ToString("N", culturInfo);
            
            TableCell toplamTahsilCell = new TableCell();
            decimal toplamTahsil = meskenTahsil + isyeriTahsil + arsaTahsil + tarlaTahsil + bisTahsil + tesisTahsil;
            decimal toplamTahsil0 = toplamTahsil == 0 ? 1 : toplamTahsil;
            toplamTahsilCell.Text = toplamTahsil.ToString("N", culturInfo);

            TableCell toplamOranCell = new TableCell();
            toplamOranCell.Text = (toplamTahsil * 100 / toplamTahakkuk0).ToString("N", culturInfo);
            
            bolgeCell.BorderStyle = BorderStyle.Solid;
            bolgeCell.BorderWidth = 2;
            bolgeCell.BorderColor = System.Drawing.Color.Black;

            kiraciSayisiCell.BorderStyle = BorderStyle.Solid;
            kiraciSayisiCell.BorderWidth = 2;
            kiraciSayisiCell.BorderColor = System.Drawing.Color.Black;

            //odeyenKiraciSayisiCell.BorderStyle = BorderStyle.Solid;
            //odeyenKiraciSayisiCell.BorderWidth = 2;
            //odeyenKiraciSayisiCell.BorderColor = System.Drawing.Color.Black;

            arsaTahakkukCell.BorderStyle = BorderStyle.Solid;
            arsaTahakkukCell.BorderWidth = 2;
            arsaTahakkukCell.BorderColor = System.Drawing.Color.Black;
            arsaTahakkukCell.HorizontalAlign = HorizontalAlign.Right;
            arsaTahsilCell.BorderStyle = BorderStyle.Solid;
            arsaTahsilCell.BorderWidth = 2;
            arsaTahsilCell.BorderColor = System.Drawing.Color.Black;
            arsaTahsilCell.HorizontalAlign = HorizontalAlign.Right;
            arsaOranCell.BorderStyle = BorderStyle.Solid;
            arsaOranCell.BorderWidth = 2;
            arsaOranCell.BorderColor = System.Drawing.Color.Black;
            arsaOranCell.HorizontalAlign = HorizontalAlign.Right;

            bisTahakkukCell.BorderStyle = BorderStyle.Solid;
            bisTahakkukCell.BorderWidth = 2;
            bisTahakkukCell.BorderColor = System.Drawing.Color.Black;
            bisTahakkukCell.HorizontalAlign = HorizontalAlign.Right;
            bisTahsilCell.BorderStyle = BorderStyle.Solid;
            bisTahsilCell.BorderWidth = 2;
            bisTahsilCell.BorderColor = System.Drawing.Color.Black;
            bisTahsilCell.HorizontalAlign = HorizontalAlign.Right;
            bisOranCell.BorderStyle = BorderStyle.Solid;
            bisOranCell.BorderWidth = 2;
            bisOranCell.BorderColor = System.Drawing.Color.Black;
            bisOranCell.HorizontalAlign = HorizontalAlign.Right;

            isyeriTahakkukCell.BorderStyle = BorderStyle.Solid;
            isyeriTahakkukCell.BorderWidth = 2;
            isyeriTahakkukCell.BorderColor = System.Drawing.Color.Black;
            isyeriTahakkukCell.HorizontalAlign = HorizontalAlign.Right;
            isyeriTahsilCell.BorderStyle = BorderStyle.Solid;
            isyeriTahsilCell.BorderWidth = 2;
            isyeriTahsilCell.BorderColor = System.Drawing.Color.Black;
            isyeriTahsilCell.HorizontalAlign = HorizontalAlign.Right;
            isyeriOranCell.BorderStyle = BorderStyle.Solid;
            isyeriOranCell.BorderWidth = 2;
            isyeriOranCell.BorderColor = System.Drawing.Color.Black;
            isyeriOranCell.HorizontalAlign = HorizontalAlign.Right;

            meskenTahakkukCell.BorderStyle = BorderStyle.Solid;
            meskenTahakkukCell.BorderWidth = 2;
            meskenTahakkukCell.BorderColor = System.Drawing.Color.Black;
            meskenTahakkukCell.HorizontalAlign = HorizontalAlign.Right;
            meskenTahsilCell.BorderStyle = BorderStyle.Solid;
            meskenTahsilCell.BorderWidth = 2;
            meskenTahsilCell.BorderColor = System.Drawing.Color.Black;
            meskenTahsilCell.HorizontalAlign = HorizontalAlign.Right;
            meskenOranCell.BorderStyle = BorderStyle.Solid;
            meskenOranCell.BorderWidth = 2;
            meskenOranCell.BorderColor = System.Drawing.Color.Black;
            meskenOranCell.HorizontalAlign = HorizontalAlign.Right;

            tarlaTahakkukCell.BorderStyle = BorderStyle.Solid;
            tarlaTahakkukCell.BorderWidth = 2;
            tarlaTahakkukCell.BorderColor = System.Drawing.Color.Black;
            tarlaTahakkukCell.HorizontalAlign = HorizontalAlign.Right;
            tarlaTahsilCell.BorderStyle = BorderStyle.Solid;
            tarlaTahsilCell.BorderWidth = 2;
            tarlaTahsilCell.BorderColor = System.Drawing.Color.Black;
            tarlaTahsilCell.HorizontalAlign = HorizontalAlign.Right;
            tarlaOranCell.BorderStyle = BorderStyle.Solid;
            tarlaOranCell.BorderWidth = 2;
            tarlaOranCell.BorderColor = System.Drawing.Color.Black;
            tarlaOranCell.HorizontalAlign = HorizontalAlign.Right;

            tesisTahakkukCell.BorderStyle = BorderStyle.Solid;
            tesisTahakkukCell.BorderWidth = 2;
            tesisTahakkukCell.BorderColor = System.Drawing.Color.Black;
            tesisTahakkukCell.HorizontalAlign = HorizontalAlign.Right;
            tesisTahsilCell.BorderStyle = BorderStyle.Solid;
            tesisTahsilCell.BorderWidth = 2;
            tesisTahsilCell.BorderColor = System.Drawing.Color.Black;
            tesisTahsilCell.HorizontalAlign = HorizontalAlign.Right;
            tesisOranCell.BorderStyle = BorderStyle.Solid;
            tesisOranCell.BorderWidth = 2;
            tesisOranCell.BorderColor = System.Drawing.Color.Black;
            tesisOranCell.HorizontalAlign = HorizontalAlign.Right;

            tableRow.Controls.Add(bolgeCell);
            tableRow.Controls.Add(kiraciSayisiCell);
            //tableRow.Controls.Add(odeyenKiraciSayisiCell);
            tableRow.Controls.Add(meskenTahakkukCell);
            tableRow.Controls.Add(meskenTahsilCell);
            tableRow.Controls.Add(meskenOranCell);
            tableRow.Controls.Add(isyeriTahakkukCell);
            tableRow.Controls.Add(isyeriTahsilCell);
            tableRow.Controls.Add(isyeriOranCell);
            tableRow.Controls.Add(arsaTahakkukCell);
            tableRow.Controls.Add(arsaTahsilCell);
            tableRow.Controls.Add(arsaOranCell);

            tableRow.Controls.Add(tarlaTahakkukCell);
            tableRow.Controls.Add(tarlaTahsilCell);
            tableRow.Controls.Add(tarlaOranCell);
            tableRow.Controls.Add(bisTahakkukCell);
            tableRow.Controls.Add(bisTahsilCell);
            tableRow.Controls.Add(bisOranCell);
            tableRow.Controls.Add(tesisTahakkukCell);
            tableRow.Controls.Add(tesisTahsilCell);
            tableRow.Controls.Add(tesisOranCell);
            tableRow.Controls.Add(toplamTahakkukCell);
            tableRow.Controls.Add(toplamTahsilCell);
            tableRow.Controls.Add(toplamOranCell);
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
