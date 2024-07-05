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

            sozlesmeTeminatCell.HorizontalAlign = HorizontalAlign.Right;
            arsaCell.HorizontalAlign = HorizontalAlign.Right;
            bisCell.HorizontalAlign = HorizontalAlign.Right;
            isyeriCell.HorizontalAlign = HorizontalAlign.Right;
            meskenCell.HorizontalAlign = HorizontalAlign.Right;
            tarlaCell.HorizontalAlign = HorizontalAlign.Right;
            tesisCell.HorizontalAlign = HorizontalAlign.Right;


            //bolgeCell.Attributes["style"] = "border:1px solid black";
            //sozlesmeAdetCell.Attributes["style"] = "border:1px solid black";
            //sozlesmeTeminatCell.Attributes["style"] = "border:1px solid black";
            //arsaCell.Attributes["style"] = "border:1px solid black";
            //bisCell.Attributes["style"] = "border:1px solid black";
            //isyeriCell.Attributes["style"] = "border:1px solid black";
            //meskenCell.Attributes["style"] = "border:1px solid black";
            //tarlaCell.Attributes["style"] = "border:1px solid black";
            //tesisCell.Attributes["style"] = "border:1px solid black";
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
            DataTable dataTable = ksDao.SelectSUMTeminatByBolgeKiralamaAmaciReturnDT(bolgeId, ProjeConstants.HEPSI);
            int adetToplam = 0;
            decimal teminatToplam = 0;
            foreach (DataRow row in dataTable.Rows)
            {

                string kiralamaAmaci = row["KiralamaAmaci"].ReturnEmptyIfZeroOrNull().ToString();
                int adet = row["Adet"].ReturnZeroIfNull().ConvertToInt();
                decimal kalanTeminatTutari = row["KalanTeminatTutari"].ReturnZeroIfNull().ConvertToDecimal();

                bolgeCell.Text = bolge;

                adetToplam += adet;
                teminatToplam += kalanTeminatTutari;
                switch (kiralamaAmaci)
                {
                    case ProjeConstants.KIRALAMAAMACI_ARSA:
                        {
                            arsaCell.Text = kalanTeminatTutari.ToString("N", cultureInfo);
                            break;
                        }
                    case ProjeConstants.KIRALAMAAMACI_BAZISTASYONU:
                        {
                            bisCell.Text = kalanTeminatTutari.ToString("N", cultureInfo);
                            break;
                        }
                    case ProjeConstants.KIRALAMAAMACI_ISYERI:
                        {
                            isyeriCell.Text = kalanTeminatTutari.ToString("N", cultureInfo);
                            break;
                        }
                    case ProjeConstants.KIRALAMAAMACI_MESKEN:
                        {
                            meskenCell.Text = kalanTeminatTutari.ToString("N", cultureInfo);
                            break;
                        }
                    case ProjeConstants.KIRALAMAAMACI_TARLA:
                        {
                            tarlaCell.Text = kalanTeminatTutari.ToString("N", cultureInfo);
                            break;
                        }
                    case ProjeConstants.KIRALAMAAMACI_TESIS:
                        {
                            tesisCell.Text = kalanTeminatTutari.ToString("N", cultureInfo);
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
            sozlesmeCell.Text = "Sozlesme";
            headerRow.Controls.Add(sozlesmeCell);

            TableHeaderCell meskenCell = new TableHeaderCell();
            meskenCell.RowSpan = 2;
            meskenCell.Text = "Mesken";
            headerRow.Controls.Add(meskenCell);

            TableHeaderCell isyeriCell = new TableHeaderCell();
            isyeriCell.RowSpan = 2;
            isyeriCell.Text = "İşyeri";
            headerRow.Controls.Add(isyeriCell);

            TableHeaderCell arsaCell = new TableHeaderCell();
            arsaCell.RowSpan = 2;
            arsaCell.Text = "Arsa";
            headerRow.Controls.Add(arsaCell);

            TableHeaderCell tarlaCell = new TableHeaderCell();
            tarlaCell.RowSpan = 2;
            tarlaCell.Text = "Tarla";
            headerRow.Controls.Add(tarlaCell);

            TableHeaderCell bisCell = new TableHeaderCell();
            bisCell.RowSpan = 2;
            bisCell.Text = "Baz İstasyonu";
            headerRow.Controls.Add(bisCell);

            TableHeaderCell tesisCell = new TableHeaderCell();
            tesisCell.RowSpan = 2;
            tesisCell.Text = "Tesis";
            headerRow.Controls.Add(tesisCell);

            TableHeaderRow headerRow1 = new TableHeaderRow();

            TableHeaderCell adetCell = new TableHeaderCell();
            adetCell.Text = "Adet";
            headerRow1.Controls.Add(adetCell);

            TableHeaderCell teminatCell = new TableHeaderCell();
            teminatCell.Text = "Teminat Tutarı";
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