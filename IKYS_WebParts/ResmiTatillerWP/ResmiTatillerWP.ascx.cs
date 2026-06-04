using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.ResmiTatillerWP
{
    [ToolboxItemAttribute(false)]
    public partial class ResmiTatillerWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ResmiTatillerWP()
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
            try
            {
                if (!Page.IsPostBack)
                {
                    DateTime today = DateTime.Today;
                    TitleLbl.Text =  "Resmi Tatil Listesi ";
                    ResmiTatilleriTabloyaDoldur();
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exHelper = new ExceptionHelper(exception);
                exHelper.PublishException();
            }
        }
        private void ResmiTatilTableHeaders()
        {
            ResmiTatilTable.Rows.Clear();

            TableHeaderRow thbaslik = new TableHeaderRow();
            TableHeaderCell baslikCell = new TableHeaderCell();

            baslikCell.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull()+" - " + DateTime.Today.AddYears(1).ConvertToDatetimeEmptyIfNull() +" Arasi Resmi Tatiller";
            baslikCell.Font.Bold = true;
            baslikCell.ColumnSpan = 4;
            thbaslik.CssClass = "alert-secondary text-center";
            thbaslik.Controls.Add(baslikCell);
            ResmiTatilTable.Controls.Add(thbaslik);

            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sira";
            TableHeaderCell resmiTarihCell = new TableHeaderCell();
            resmiTarihCell.Text = "Resmi Tatil";
            TableHeaderCell BaslangicTarCell = new TableHeaderCell();
            BaslangicTarCell.Text = "Baslangiç Tarihi";
            TableHeaderCell BitisTarCell = new TableHeaderCell();
            BitisTarCell.Text = "Bitis Tarihi";

            th.Controls.Add(siraCell);
            th.Controls.Add(resmiTarihCell);
            th.Controls.Add(BaslangicTarCell);
            th.Controls.Add(BitisTarCell);

            ResmiTatilTable.Controls.Add(th);
        }
        private void ResmiTatilleriTabloyaDoldur()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            DateTime today = DateTime.Today;

            DateTime basTar = DateTime.Today;
            DateTime bitTar = basTar.AddYears(1);

            ResmiTatil resmiTatilDao = new ResmiTatil();
            List<ResmiTatil> list = resmiTatilDao.SelectByTarih(basTar,bitTar);

            if (list.Count > 0)
            {
                ResmiTatilTableHeaders();
                int SiraNo = 1;

                ResmiTatilListesiniDuzenle(list);
                List<ResmiTatil> SortedList = list.OrderBy(o => o.BaslamaTarihi).ToList();
                foreach (ResmiTatil resmiTatil in SortedList)
                {
                    TableRow row = new TableRow();

                    TableCell SiraCell = new TableCell();
                    SiraCell.Text = SiraNo++ + "";
                    row.Controls.Add(SiraCell);

                    TableCell TatilCell = new TableCell();
                    TatilCell.Text = resmiTatil.Tatil;
                    row.Controls.Add(TatilCell);

                    string bassaatstr = string.Empty;

                    
                    if (resmiTatil.BaslamaTarihi.Hour > 0)
                    {
                        bassaatstr = resmiTatil.BaslamaTarihi.ToString("HH:mm", culturInfo);
                    }

                    TableCell BastarCell = new TableCell();
                    BastarCell.Text = resmiTatil.BaslamaTarihi.ToString("dd.MM.yyyy", culturInfo) + " " + bassaatstr + " (" + resmiTatil.BaslamaTarihi.ToString("dddd", culturInfo) + ")";
                    row.Controls.Add(BastarCell);

                    TableCell BittarCell = new TableCell();
                    BittarCell.Text = resmiTatil.BitisTarihi.ToString("dd.MM.yyyy", culturInfo) + " (" + resmiTatil.BitisTarihi.ToString("dddd", culturInfo) + ")";
                    row.Controls.Add(BittarCell);

                    ResmiTatilTable.Controls.Add(row);
                }
            }
            else
            {
                ResmiTatilTable.Rows.Clear();
                TableHeaderRow thbaslik = new TableHeaderRow();
                TableHeaderCell baslikCell = new TableHeaderCell();

                baslikCell.Text = DateTime.Today.Year + " Yili Resmi Tatil Listesi";
                baslikCell.Font.Bold = true;
                baslikCell.ColumnSpan = 4;
                thbaslik.CssClass = "alert-secondary text-center";
                thbaslik.Controls.Add(baslikCell);
                ResmiTatilTable.Controls.Add(thbaslik);

                TableRow tr = new TableRow();
                TableCell tc = new TableCell();
                tc.ColumnSpan = 7;
                tc.Text = "Kayitli resmi tatil bulunmamaktadir.";
                tr.Controls.Add(tc);
                ResmiTatilTable.Controls.Add(tr);
            }
        }

        private void ResmiTatilListesiniDuzenle(List<ResmiTatil> list)
        {
            DateTime today = DateTime.Today;
            foreach (ResmiTatil resmiTatil in list)
            {

                if (resmiTatil.BaslamaTarihi.Year < 1900)
                {
                    string hourMinute = resmiTatil.BaslamaTarihi.Hour + ":" + resmiTatil.BaslamaTarihi.Minute;

                    resmiTatil.BaslamaTarihi = new DateTime(today.Year, resmiTatil.BaslamaTarihi.Month, resmiTatil.BaslamaTarihi.Day);
                    if (resmiTatil.BaslamaTarihi < DateTime.Today)
                    {
                        resmiTatil.BaslamaTarihi = new DateTime(today.AddYears(1).Year, resmiTatil.BaslamaTarihi.Month, resmiTatil.BaslamaTarihi.Day);
                    }

                    resmiTatil.BaslamaTarihi = UtilityHelper.TariheSaatEkle(resmiTatil.BaslamaTarihi, hourMinute);
                }
                if (resmiTatil.BitisTarihi.Year < 1900)
                {
                    resmiTatil.BitisTarihi = new DateTime(today.Year, resmiTatil.BitisTarihi.Month, resmiTatil.BitisTarihi.Day);
                    if (resmiTatil.BitisTarihi < DateTime.Today)
                    {
                        resmiTatil.BitisTarihi = new DateTime(today.AddYears(1).Year, resmiTatil.BitisTarihi.Month, resmiTatil.BitisTarihi.Day);
                    }
                }                
            }
        }

        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            ResmiTatilleriTabloyaDoldur();
            ExportToExcel();
        }
        protected void ExportToExcel()
        {
            string filename = "ResmiTatilListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
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
