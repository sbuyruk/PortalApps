using Model.NBYS;
using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.NakitBagisRaporuByYil_il
{
    [ToolboxItemAttribute(false)]
    public partial class NakitBagisRaporuByYil_il : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NakitBagisRaporuByYil_il()
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
            RaporTableDoldur();
        }
        private void RaporTableDoldur()
        {
            int ilkYil = 2017;
            int sonYil = 2021;
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            NakitBagisHareket nakitBagisHareket = new NakitBagisHareket();
            RaporTableHeaders(ilkYil, sonYil);
            DataTable dataTable = nakitBagisHareket.SelectCountSumByYil_il(ilkYil, sonYil);
            int counter = 0;
            try
            {
                bool artti = false;
                while (counter < dataTable.Rows.Count)
                {

                    DataRow row = dataTable.Rows[counter];
                    string bolge = row["Bolge"].ToString();
                    string il = row["IlAdi"].ToString();
                    TableRow tableRow = new TableRow();
                    TableCell bolgeCell = new TableCell();
                    bolgeCell.Text = bolge;
                    TableCell ilCell = new TableCell();
                    ilCell.Text = il;
                    tableRow.Controls.Add(bolgeCell);
                    tableRow.Controls.Add(ilCell);
                    for (int i = ilkYil; i <= sonYil; i++)
                    {
                        row = dataTable.Rows[counter];
                        int yil = row["Yil"].ReturnEmptyIfZeroOrNull().ConvertToInt();
                        TableCell yilCell = new TableCell();
                        yilCell.Text = i.ToString();
                        //tableRow.Controls.Add(yilCell);

                        TableCell adetCell = new TableCell();
                        adetCell.HorizontalAlign = HorizontalAlign.Right;
                        TableCell tutarCell = new TableCell();
                        tutarCell.HorizontalAlign = HorizontalAlign.Right;
                        tableRow.Controls.Add(adetCell);
                        tableRow.Controls.Add(tutarCell);
                        if (yil == i)
                        {
                            string adet = row["Adet"].ReturnEmptyIfZeroOrNull().ToString();

                            adetCell.Text = adet;
                            string tutar = row["Tutar"].ReturnZeroIfNull().ConvertToDecimal().ToString("N", culturInfo); ;

                            tutarCell.Text = tutar;

                        }
                        else
                        {

                            continue;
                        }
                        if (dataTable.Rows.Count > counter + 1)
                        {
                            counter++;
                            artti = true;
                        }

                    }
                    RaporTable.Rows.Add(tableRow);
                    if (artti)
                    {

                        artti = false;
                    }
                    else
                    {
                        counter++;
                    }
                }
            }
            catch (Exception ex)
            {

                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.PublishException();
            }
        }

        private void RaporTableHeaders(int ilkYil, int sonYil)
        {
            TableHeaderRow tableHeaderRow = new TableHeaderRow();
            TableHeaderRow tableHeaderRow1 = new TableHeaderRow();

            TableHeaderCell bolgeHeaderCell = new TableHeaderCell();
            bolgeHeaderCell.Text = "Bölge";
            bolgeHeaderCell.RowSpan = 2;
            TableHeaderCell ilHeaderCell = new TableHeaderCell();
            ilHeaderCell.Text = "Il";
            ilHeaderCell.RowSpan = 2;
            tableHeaderRow.Controls.Add(bolgeHeaderCell);
            tableHeaderRow.Controls.Add(ilHeaderCell);
            for (int i = ilkYil; i <= sonYil; i++)
            {
                TableHeaderCell yilHeaderCell = new TableHeaderCell();
                yilHeaderCell.ColumnSpan = 2;
                yilHeaderCell.Text = i.ToString();
                tableHeaderRow.Controls.Add(yilHeaderCell);
                TableHeaderCell adetHeaderCell = new TableHeaderCell();
                adetHeaderCell.Text = "Adet";
                TableHeaderCell bagisMiktariHeaderCell = new TableHeaderCell();
                bagisMiktariHeaderCell.Text = "Bağış Miktarı";
                tableHeaderRow1.Controls.Add(adetHeaderCell);
                tableHeaderRow1.Controls.Add(bagisMiktariHeaderCell);
            }
            RaporTable.Rows.Add(tableHeaderRow);
            RaporTable.Rows.Add(tableHeaderRow1);
        }

        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                int index = currentUrl.LastIndexOf("?") < 0 ? currentUrl.Length : currentUrl.LastIndexOf("?");

                string rawUrl = currentUrl.Substring(0, index);
                string newUrl = rawUrl.Substring(0, rawUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
    }
}