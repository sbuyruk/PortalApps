using Model.TBYS;
using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.TeminatDurumRaporuWP
{
    [ToolboxItemAttribute(false)]
    public partial class TeminatDurumRaporuWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TeminatDurumRaporuWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private IFormatProvider cultureInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        private int colToplamAdet = 0;
        private decimal colToplamTeminat = 0;
        private decimal colToplamArsa = 0;
        private decimal colToplamBis = 0;
        private decimal colToplamIsyeri = 0;
        private decimal colToplamMesken = 0;
        private decimal colToplamTarla = 0;
        private decimal colToplamTesis = 0;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinmazDurumuTablosunuDoldur();

        }
        protected void TasinmazDurumuTablosunuDoldur()
        {

            TasinmazDurumuTableHeaders();

            TabloyaBolgeEkle(ProjeConstants.BOLGE_ANKARA, ProjeConstants.BOLGE_ANKARA_INT);
            TabloyaBolgeEkle(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.BOLGE_ISTANBUL_INT);
            TabloyaBolgeEkle(ProjeConstants.BOLGE_IZMIR, ProjeConstants.BOLGE_IZMIR_INT);
            TabloyaBolgeEkle(ProjeConstants.BOLGE_MERSIN, ProjeConstants.BOLGE_MERSIN_INT);
            TabloyaBolgeEkle(ProjeConstants.BOLGE_ERZURUM, ProjeConstants.BOLGE_ERZURUM_INT);
            ToplamSatiriEkle();
        }

        private void ToplamSatiriEkle()
        {
            TableRow tableRow = new TableRow();
            TableCell satirToplamiCell = new TableCell();
            satirToplamiCell.Text = "Toplam";           

            TableCell sozlesmeAdetToplamiCell = new TableCell();
            sozlesmeAdetToplamiCell.Text = colToplamAdet.ToString();
            TableCell sozlesmeTeminatToplamiCell = new TableCell();
            sozlesmeTeminatToplamiCell.Text = colToplamTeminat.ToString("N", cultureInfo);
            TableCell arsaToplamiCell = new TableCell();
            arsaToplamiCell.Text = colToplamArsa.ToString("N", cultureInfo);
            TableCell bisToplamiCell = new TableCell();
            bisToplamiCell.Text = colToplamBis.ToString("N", cultureInfo);
            TableCell isyeriToplamiCell = new TableCell();
            isyeriToplamiCell.Text = colToplamIsyeri.ToString("N", cultureInfo);
            TableCell meskenToplamiCell = new TableCell();
            meskenToplamiCell.Text = colToplamMesken.ToString("N", cultureInfo);
            TableCell tarlaToplamiCell = new TableCell();
            tarlaToplamiCell.Text = colToplamTarla.ToString("N", cultureInfo);
            TableCell tesisToplamiCell = new TableCell();
            tesisToplamiCell.Text = colToplamTesis.ToString("N", cultureInfo); 

            sozlesmeAdetToplamiCell.HorizontalAlign = HorizontalAlign.Right;
            sozlesmeTeminatToplamiCell.HorizontalAlign = HorizontalAlign.Right;
            arsaToplamiCell.HorizontalAlign = HorizontalAlign.Right;
            bisToplamiCell.HorizontalAlign = HorizontalAlign.Right;
            isyeriToplamiCell.HorizontalAlign = HorizontalAlign.Right;
            meskenToplamiCell.HorizontalAlign = HorizontalAlign.Right;
            tarlaToplamiCell.HorizontalAlign = HorizontalAlign.Right;
            tesisToplamiCell.HorizontalAlign = HorizontalAlign.Right;

            satirToplamiCell.BorderStyle = BorderStyle.Solid;
            sozlesmeAdetToplamiCell.BorderStyle = BorderStyle.Solid;
            sozlesmeTeminatToplamiCell.BorderStyle = BorderStyle.Solid;
            arsaToplamiCell.BorderStyle = BorderStyle.Solid;
            bisToplamiCell.BorderStyle = BorderStyle.Solid;
            isyeriToplamiCell.BorderStyle = BorderStyle.Solid;
            meskenToplamiCell.BorderStyle = BorderStyle.Solid;
            tarlaToplamiCell.BorderStyle = BorderStyle.Solid;
            tesisToplamiCell.BorderStyle = BorderStyle.Solid;

            satirToplamiCell.Font.Bold = true;
            sozlesmeAdetToplamiCell.Font.Bold = true;
            sozlesmeTeminatToplamiCell.Font.Bold = true;
            arsaToplamiCell.Font.Bold = true;
            bisToplamiCell.Font.Bold = true;
            isyeriToplamiCell.Font.Bold = true;
            meskenToplamiCell.Font.Bold = true;
            tarlaToplamiCell.Font.Bold = true;
            tesisToplamiCell.Font.Bold = true;


            tableRow.Cells.Add(satirToplamiCell);
            tableRow.Cells.Add(sozlesmeAdetToplamiCell);
            tableRow.Cells.Add(sozlesmeTeminatToplamiCell);
            tableRow.Cells.Add(meskenToplamiCell);
            tableRow.Cells.Add(isyeriToplamiCell);
            tableRow.Cells.Add(arsaToplamiCell);
            tableRow.Cells.Add(tarlaToplamiCell);
            tableRow.Cells.Add(bisToplamiCell);
            tableRow.Cells.Add(tesisToplamiCell);

            TeminatDurumuTable.Rows.Add(tableRow);
        }

        private void TabloyaBolgeEkle(string bolge,int bolgeId)
        {
            TableRow tableRow = new TableRow();
            TableCell bolgeCell = new TableCell();

            TableCell sozlesmeAdetCell = new TableCell();
            TableCell sozlesmeTeminatCell = new TableCell();

            TableCell arsaCell = new TableCell();            
            TableCell bisCell = new TableCell();
            TableCell isyeriCell = new TableCell();
            TableCell meskenCell = new TableCell();
            TableCell tarlaCell = new TableCell();
            TableCell tesisCell = new TableCell();

            sozlesmeAdetCell.HorizontalAlign = HorizontalAlign.Right;
            sozlesmeTeminatCell.HorizontalAlign = HorizontalAlign.Right;
            arsaCell.HorizontalAlign = HorizontalAlign.Right;
            bisCell.HorizontalAlign = HorizontalAlign.Right;
            isyeriCell.HorizontalAlign = HorizontalAlign.Right;
            meskenCell.HorizontalAlign = HorizontalAlign.Right;
            tarlaCell.HorizontalAlign = HorizontalAlign.Right;
            tesisCell.HorizontalAlign = HorizontalAlign.Right;

            bolgeCell.BorderStyle = BorderStyle.Solid;
            sozlesmeAdetCell.BorderStyle = BorderStyle.Solid;
            sozlesmeTeminatCell.BorderStyle = BorderStyle.Solid;
            arsaCell.BorderStyle = BorderStyle.Solid;
            bisCell.BorderStyle = BorderStyle.Solid;
            isyeriCell.BorderStyle = BorderStyle.Solid;
            meskenCell.BorderStyle = BorderStyle.Solid;
            tarlaCell.BorderStyle = BorderStyle.Solid;
            tesisCell.BorderStyle = BorderStyle.Solid;

            tableRow.Controls.Add(bolgeCell);
            tableRow.Controls.Add(sozlesmeAdetCell);
            tableRow.Controls.Add(sozlesmeTeminatCell);
            tableRow.Controls.Add(meskenCell);
            tableRow.Controls.Add(isyeriCell);
            tableRow.Controls.Add(arsaCell);
            tableRow.Controls.Add(tarlaCell);
            tableRow.Controls.Add(bisCell);
            tableRow.Controls.Add(tesisCell);

            TeminatDurumuTable.Rows.Add(tableRow);

            KiraSozlesme ksDao = new KiraSozlesme();
            DataTable dataTable = new Model.Services.TBYS.KiraSozlesmeService().GetSecurityDepositSummaryByRegionAndPurpose(bolgeId, ProjeConstants.HEPSI);
            int adetToplam = 0;
            decimal teminatToplam = 0;
            foreach (DataRow row in dataTable.Rows)
            {

                string kiralamaAmaci = row["KiralamaAmaci"].ReturnEmptyIfZeroOrNull().ToString();
                int adet = row["Adet"].ReturnZeroIfNull().ConvertToInt();
                decimal kalanTeminatTutari = row["KalanTeminatTutari"].ReturnZeroIfNull().ConvertToDecimal();

                bolgeCell.Text = bolge;

                adetToplam += adet;
                colToplamAdet += adet;
                teminatToplam += kalanTeminatTutari;
                colToplamTeminat += kalanTeminatTutari;
                switch (kiralamaAmaci)
                {
                    case ProjeConstants.KIRALAMAAMACI_ARSA:
                        {
                            arsaCell.Text = kalanTeminatTutari.ToString("N", cultureInfo);
                            colToplamArsa += kalanTeminatTutari;
                            break;
                        }
                    case ProjeConstants.KIRALAMAAMACI_BAZISTASYONU:
                        {
                            bisCell.Text = kalanTeminatTutari.ToString("N", cultureInfo);
                            colToplamBis += kalanTeminatTutari;
                            break;
                        }
                    case ProjeConstants.KIRALAMAAMACI_ISYERI:
                        {
                            isyeriCell.Text = kalanTeminatTutari.ToString("N", cultureInfo);
                            colToplamIsyeri += kalanTeminatTutari;
                            break;
                        }
                    case ProjeConstants.KIRALAMAAMACI_MESKEN:
                        {
                            meskenCell.Text = kalanTeminatTutari.ToString("N", cultureInfo);
                            colToplamMesken += kalanTeminatTutari;
                            break;
                        }
                    case ProjeConstants.KIRALAMAAMACI_TARLA:
                        {
                            tarlaCell.Text = kalanTeminatTutari.ToString("N", cultureInfo);
                            colToplamTarla += kalanTeminatTutari;
                            break;
                        }
                    case ProjeConstants.KIRALAMAAMACI_TESIS:
                        {
                            tesisCell.Text = kalanTeminatTutari.ToString("N", cultureInfo);
                            colToplamTesis += kalanTeminatTutari;
                            break;
                        }
                    default:
                        break;
                }
            }
            sozlesmeAdetCell.Text = adetToplam.ToString();
            sozlesmeTeminatCell.Text = teminatToplam.ToString("N",cultureInfo);
        }

        private void TasinmazDurumuTableHeaders()
        {
            TeminatDurumuTable.Rows.Clear();

            TableHeaderRow titleRow = new TableHeaderRow();
            TableHeaderCell titleCell = new TableHeaderCell();
            titleCell.ColumnSpan = 9;
            titleCell.Text = "Teminat Durum Raporu";
            titleRow.Controls.Add(titleCell);

            TableHeaderRow headerRow = new TableHeaderRow();

            TableHeaderCell bolgeCell = new TableHeaderCell();
            bolgeCell.RowSpan = 2;
            bolgeCell.Text = "Bölge";
            headerRow.Controls.Add(bolgeCell);

            TableHeaderCell sozlesmeCell = new TableHeaderCell();
            sozlesmeCell.ColumnSpan = 2;
            sozlesmeCell.Text = "Sözleşme";
            headerRow.Controls.Add(sozlesmeCell);

            TableHeaderCell meskenCell = new TableHeaderCell();
            meskenCell.RowSpan = 2;
            meskenCell.Text = "Mesken (TL)";
            headerRow.Controls.Add(meskenCell);

            TableHeaderCell isyeriCell = new TableHeaderCell();
            isyeriCell.RowSpan = 2;
            isyeriCell.Text = "İşyeri (TL)";
            headerRow.Controls.Add(isyeriCell);

            TableHeaderCell arsaCell = new TableHeaderCell();
            arsaCell.RowSpan = 2;
            arsaCell.Text = "Arsa (TL)";
            headerRow.Controls.Add(arsaCell);

            TableHeaderCell tarlaCell = new TableHeaderCell();
            tarlaCell.RowSpan = 2;
            tarlaCell.Text = "Tarla (TL)";
            headerRow.Controls.Add(tarlaCell);

            TableHeaderCell bisCell = new TableHeaderCell();
            bisCell.RowSpan = 2;
            bisCell.Text = "Baz İstasyonu (TL)";
            headerRow.Controls.Add(bisCell);

            TableHeaderCell tesisCell = new TableHeaderCell();
            tesisCell.RowSpan = 2;
            tesisCell.Text = "Tesis (TL)";
            headerRow.Controls.Add(tesisCell);

            TableHeaderRow headerRow1 = new TableHeaderRow();

            TableHeaderCell adetCell = new TableHeaderCell();
            adetCell.Text = "Adet";
            headerRow1.Controls.Add(adetCell);

            TableHeaderCell teminatCell = new TableHeaderCell();
            teminatCell.Text = "Teminat Tutarı (TL)";
            headerRow1.Controls.Add(teminatCell);

            bolgeCell.BorderStyle = BorderStyle.Solid;
            sozlesmeCell.BorderStyle = BorderStyle.Solid;
            meskenCell.BorderStyle = BorderStyle.Solid;
            isyeriCell.BorderStyle = BorderStyle.Solid;
            arsaCell.BorderStyle = BorderStyle.Solid;
            tarlaCell.BorderStyle = BorderStyle.Solid;
            bisCell.BorderStyle = BorderStyle.Solid;
            tesisCell.BorderStyle = BorderStyle.Solid;
            adetCell.BorderStyle = BorderStyle.Solid;
            teminatCell.BorderStyle = BorderStyle.Solid;

            TeminatDurumuTable.Rows.Add(titleRow);
            TeminatDurumuTable.Rows.Add(headerRow);
            TeminatDurumuTable.Rows.Add(headerRow1);
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            string filename = "TeminatzDurumRaporu" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            TeminatDurumuTable.RenderControl(hw);
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