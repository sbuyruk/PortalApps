using Model.NBYS;
using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.NakitBagisRaporuByBolgeWP
{
    [ToolboxItemAttribute(false)]
    public partial class NakitBagisRaporuByBolgeWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NakitBagisRaporuByBolgeWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }

        private string SecilenYilQS
        {
            get
            {

                if (ViewState["SecilenYil"] == null)
                {
                    if (Page.Request.QueryString["SecilenYil"] != null)
                    {
                        ViewState["SecilenYil"] = Page.Request.QueryString["SecilenYil"];
                    }
                    else
                    {
                        ViewState["SecilenYil"] = string.Empty;
                    }
                }
                return ViewState["SecilenYil"].ToString();
            }

            set
            {
                ViewState["SecilenYil"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                DateTime today = DateTime.Today;

                DateTime bastar = new DateTime(today.Year, today.Month, 1);
                SecilenYilQS = bastar.Year.ToString();
                FillYear();
                SetDDLValues();
                FillTable();
            }

        }
        protected void FillTable()
        {



            int yil = YilDDL.SelectedItem.Value.ConvertToInt();


            TableHeaderCell.Text = yil + " YILI NAKİT BAĞIŞ RAPORU";

            SetCellValues(yil);

        }
        private void SetCellValues(int yil)
        {
            try
            {
                IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
                int AnkToplamAdet = 0;
                Decimal AnkToplamTutar = 0;
                int IstToplamAdet = 0;
                Decimal IstToplamTutar = 0;
                int IzmToplamAdet = 0;
                Decimal IzmToplamTutar = 0;
                int MerToplamAdet = 0;
                Decimal MerToplamTutar = 0;
                int YurtdisiToplamAdet = 0;
                Decimal YurtdisiToplamTutar = 0;
                NakitBagisHareket nbh = new NakitBagisHareket();
                for (int i = 1; i <= 12; i++)
                {

                    DataTable dataTable = nbh.SelectCountByBagisTarihiBolge(yil, i);
                    if (dataTable != null)
                    {
                        if (dataTable.Rows.Count > 0)
                        {
                            TableRow tableRow = new TableRow();
                            TableCell AyCell = new TableCell();
                            DateTime tarih = new DateTime(DateTime.Now.Year, i, 1);
                            string aystr = tarih.ToString("MMMM");
                            AyCell.CssClass = "fw-bold text-end";
                            AyCell.Text = aystr;
                            tableRow.Controls.Add(AyCell);

                            TableCell AnkAdetCell = new TableCell();
                            tableRow.Controls.Add(AnkAdetCell);
                            TableCell AnkTutarCell = new TableCell();
                            AnkTutarCell.CssClass = "text-end";
                            tableRow.Controls.Add(AnkTutarCell);

                            TableCell IstAdetCell = new TableCell();
                            tableRow.Controls.Add(IstAdetCell);
                            TableCell IstTutarCell = new TableCell();
                            IstTutarCell.CssClass = "text-end";
                            tableRow.Controls.Add(IstTutarCell);

                            TableCell IzmAdetCell = new TableCell();
                            tableRow.Controls.Add(IzmAdetCell);
                            TableCell IzmTutarCell = new TableCell();
                            IzmTutarCell.CssClass = "text-end";
                            tableRow.Controls.Add(IzmTutarCell);

                            TableCell MerAdetCell = new TableCell();
                            tableRow.Controls.Add(MerAdetCell);
                            TableCell MerTutarCell = new TableCell();
                            MerTutarCell.CssClass = "text-end";
                            tableRow.Controls.Add(MerTutarCell);

                            TableCell YurtdisiAdetCell = new TableCell();
                            tableRow.Controls.Add(YurtdisiAdetCell);
                            TableCell YurtdisiTutarCell = new TableCell();
                            YurtdisiTutarCell.CssClass = "text-end";
                            tableRow.Controls.Add(YurtdisiTutarCell);


                            int AyToplamAdet = 0;
                            Decimal AyToplamTutar = 0;
                            foreach (DataRow row in dataTable.Rows)
                            {
                                int adet = Int32.Parse(row["Adet"].ReturnZeroIfNull().ToString());
                                Decimal tutar = Decimal.Parse(row["Toplam"].ReturnZeroIfNull().ToString());
                                string tutarStr = tutar.ToString("N", culturInfo);
                                string adetStr = adet.ToString();
                                string bolge = row["Bolge"].ReturnEmptyIfNull().ToString();
                                AyToplamAdet += adet;
                                AyToplamTutar += tutar;
                                if (bolge.Equals(ProjeConstants.BOLGE_ANKARA))
                                {
                                    AnkTutarCell.Text = tutarStr;
                                    AnkAdetCell.Text = adetStr;
                                    AnkToplamAdet += adet;
                                    AnkToplamTutar += tutar;
                                }
                                else if (bolge.Equals(ProjeConstants.BOLGE_ISTANBUL))
                                {
                                    IstTutarCell.Text = tutarStr;
                                    IstAdetCell.Text = adetStr;
                                    IstToplamAdet += adet;
                                    IstToplamTutar += tutar;
                                }
                                else if (bolge.Equals(ProjeConstants.BOLGE_IZMIR))
                                {
                                    IzmTutarCell.Text = tutarStr;
                                    IzmAdetCell.Text = adetStr;
                                    IzmToplamAdet += adet;
                                    IzmToplamTutar += tutar;
                                }
                                else if (bolge.Equals(ProjeConstants.BOLGE_MERSIN))
                                {
                                    MerTutarCell.Text = tutarStr;
                                    MerAdetCell.Text = adetStr;
                                    MerToplamAdet += adet;
                                    MerToplamTutar += tutar;
                                }
                                else if (bolge.Equals(ProjeConstants.BOLGE_YURTDISI))
                                {
                                    YurtdisiTutarCell.Text = tutarStr;
                                    YurtdisiAdetCell.Text = adetStr;
                                    YurtdisiToplamAdet += adet;
                                    YurtdisiToplamTutar += tutar;
                                }
                            }//foreach

                            TableCell AyToplamAdetCell = new TableCell();
                            AyToplamAdetCell.CssClass = "fw-bold text-end";
                            AyToplamAdetCell.Text = AyToplamAdet + "";
                            tableRow.Controls.Add(AyToplamAdetCell);
                            TableCell AyToplamTutarCell = new TableCell();
                            AyToplamTutarCell.Text = AyToplamTutar.ToString("N", culturInfo);
                            AyToplamTutarCell.CssClass = "fw-bold text-end";
                            tableRow.Controls.Add(AyToplamTutarCell);

                            NBTable.Controls.Add(tableRow);


                        }//if
                    }//if 
                }//for

                TableRow toplamRow = new TableRow();

                TableCell ToplamBaslikCell = new TableCell();
                ToplamBaslikCell.CssClass = "fw-bold text-end";
                ToplamBaslikCell.Text = "Toplam";
                toplamRow.Controls.Add(ToplamBaslikCell);

                TableCell AnkToplamAdetCell = new TableCell();
                AnkToplamAdetCell.CssClass = "fw-bold text-end";
                AnkToplamAdetCell.Text = AnkToplamAdet + "";
                toplamRow.Controls.Add(AnkToplamAdetCell);
                TableCell AnkToplamTutarCell = new TableCell();
                AnkToplamTutarCell.Text = AnkToplamTutar.ToString("N", culturInfo);
                AnkToplamTutarCell.CssClass = "fw-bold text-end";
                toplamRow.Controls.Add(AnkToplamTutarCell);

                TableCell IstToplamAdetCell = new TableCell();
                IstToplamAdetCell.CssClass = "fw-bold text-end";
                IstToplamAdetCell.Text = IstToplamAdet + "";
                toplamRow.Controls.Add(IstToplamAdetCell);
                TableCell IstToplamTutarCell = new TableCell();
                IstToplamTutarCell.Text = IstToplamTutar.ToString("N", culturInfo);
                IstToplamTutarCell.CssClass = "fw-bold text-end";
                toplamRow.Controls.Add(IstToplamTutarCell);

                TableCell IzmToplamAdetCell = new TableCell();
                IzmToplamAdetCell.CssClass = "fw-bold text-end";
                IzmToplamAdetCell.Text = IzmToplamAdet + "";
                toplamRow.Controls.Add(IzmToplamAdetCell);
                TableCell IzmToplamTutarCell = new TableCell();
                IzmToplamTutarCell.Text = IzmToplamTutar.ToString("N", culturInfo);
                IzmToplamTutarCell.CssClass = "fw-bold text-end";
                toplamRow.Controls.Add(IzmToplamTutarCell);

                TableCell MerToplamAdetCell = new TableCell();
                MerToplamAdetCell.CssClass = "fw-bold text-end";
                MerToplamAdetCell.Text = MerToplamAdet + "";
                toplamRow.Controls.Add(MerToplamAdetCell);
                TableCell MerToplamTutarCell = new TableCell();
                MerToplamTutarCell.Text = MerToplamTutar.ToString("N", culturInfo);
                MerToplamTutarCell.CssClass = "fw-bold text-end";
                toplamRow.Controls.Add(MerToplamTutarCell);


                TableCell YurtdisiToplamAdetCell = new TableCell();
                YurtdisiToplamAdetCell.CssClass = "fw-bold text-end";
                YurtdisiToplamAdetCell.Text = YurtdisiToplamAdet + "";
                toplamRow.Controls.Add(YurtdisiToplamAdetCell);
                TableCell YurtdisiToplamTutarCell = new TableCell();
                YurtdisiToplamTutarCell.Text = YurtdisiToplamTutar.ToString("N", culturInfo);
                YurtdisiToplamTutarCell.CssClass = "fw-bold text-end";
                toplamRow.Controls.Add(YurtdisiToplamTutarCell);

                TableCell EnToplamAdetCell = new TableCell();
                EnToplamAdetCell.CssClass = "text-danger fw-bold text-end text-end";
                EnToplamAdetCell.Text = AnkToplamAdet + IstToplamAdet + IzmToplamAdet + MerToplamAdet + YurtdisiToplamAdet + "";
                toplamRow.Controls.Add(EnToplamAdetCell);
                TableCell EnToplamTutarCell = new TableCell();
                EnToplamTutarCell.Text = (AnkToplamTutar + IstToplamTutar + IzmToplamTutar + MerToplamTutar + YurtdisiToplamTutar).ToString("N", culturInfo);
                EnToplamTutarCell.CssClass = "text-danger fw-bold text-end";
                toplamRow.Controls.Add(EnToplamTutarCell);

                NBTable.Controls.Add(toplamRow);
            }
            catch (Exception e)
            {

                ExceptionHelper eh = new ExceptionHelper(e);
                eh.PublishException();
            }
        }
        private void FillYear()
        {
            var year = DateTime.Now.Year;
            for (int i = 2010; i <= year; i++)
            {
                YilDDL.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }
        private void SetDDLValues()
        {
            try
            {
                //yil
                string yil = !string.IsNullOrEmpty(SecilenYilQS) ? SecilenYilQS : DateTime.Today.Year.ReturnEmptyIfNull().ToString();
                ListItem YilItem = new ListItem();
                if (!string.IsNullOrEmpty(yil))
                    YilItem = YilDDL.Items.FindByValue(yil);

                if (YilItem != null)
                {
                    YilDDL.SelectedValue = YilItem.Value;
                    SecilenYilQS = YilItem.Value;
                }
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
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToExcel();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void ExportToExcel()
        {
            string filename = "NakitBagisRaporu" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control
            FillTable();
            NBTable.RenderControl(hw);
            //Write the HTML back to the browser.
            //Response.ContentType = application/vnd.ms-excel;
            Page.Response.ContentType = "application/vnd.ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            Page.Response.Write(tw.ToString());
            Page.Response.End();
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillTable();
        }
    }
}
