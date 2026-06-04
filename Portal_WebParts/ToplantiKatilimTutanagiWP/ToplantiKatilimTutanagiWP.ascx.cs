using Model.IKYS;
using Model.MTS;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.ToplantiKatilimTutanagiWP
{
    [ToolboxItemAttribute(false)]
    public partial class ToplantiKatilimTutanagiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ToplantiKatilimTutanagiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string ToplantiIdQS
        {
            get
            {

                if (ViewState["ToplantiId"] == null)
                {
                    if (Page.Request.QueryString["ToplantiId"] != null)
                    {
                        ViewState["ToplantiId"] = Page.Request.QueryString["ToplantiId"];
                    }
                    else
                    {
                        ViewState["ToplantiId"] = string.Empty;
                    }
                }
                return ViewState["ToplantiId"].ToString();
            }

            set
            {
                ViewState["ToplantiId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            KatilimciListesiniDoldur();
        }

        private void KatilimciListesiniDoldur()
        {
            Toplanti toplanti = new Toplanti();
            toplanti = toplanti.Select(ToplantiIdQS.ConvertToInt());
            if (toplanti != null)
            {
                BaslikCell.Text = "TOPLANTI KATILIM TUTANAGI <br style='mso-data-placement:same-cell;' />(" + toplanti.ToplantiKonusu + ") <br style='mso-data-placement:same-cell;' />(" + toplanti.BaslangicTarihi.ToString("dd.MM.yyyy") + ")";
                ToplantiKatilim toplantiKatilim = new ToplantiKatilim();
                List<ToplantiKatilim> katilimciListesi = toplantiKatilim.SelectBytoplantiId(toplanti.Id);
                int siraNo = 0;
                foreach (var item in katilimciListesi)
                {
                    TableRow tableRow = new TableRow();
                    TableCell siraNoCell = new TableCell();
                    TableCell adiSoyadiCell = new TableCell();
                    TableCell unvanCell = new TableCell();
                    TableCell kurumCell = new TableCell();
                    TableCell telefonCell = new TableCell();
                    TableCell epostaCell = new TableCell();
                    TableCell imzaCell = new TableCell();

                    siraNoCell.BorderStyle = BorderStyle.Solid;
                    adiSoyadiCell.BorderStyle = BorderStyle.Solid;
                    unvanCell.BorderStyle = BorderStyle.Solid;
                    kurumCell.BorderStyle = BorderStyle.Solid;
                    telefonCell.BorderStyle = BorderStyle.Solid;
                    epostaCell.BorderStyle = BorderStyle.Solid;
                    imzaCell.BorderStyle = BorderStyle.Solid;

                    siraNoCell.Style.Add("width", "40px");
                    adiSoyadiCell.Style.Add("width", "15%");
                    unvanCell.Style.Add("width", "15%");
                    kurumCell.Style.Add("width", "20%");
                    telefonCell.Style.Add("width", "10%");
                    epostaCell.Style.Add("width", "20%");
                    imzaCell.Style.Add("width", "100px");

                    siraNoCell.Text = (++siraNo).ToString();
                    Personel personel = new Personel();
                    DataTable dataTable = personel.SelectPersonelReturnDataTable(item.KatilimciId);
                    if (dataTable != null)
                    {
                        adiSoyadiCell.Text = dataTable.Rows[0]["Adi"].ReturnEmptyIfNull().ToString() + " " + dataTable.Rows[0]["Soyadi"].ReturnEmptyIfNull().ToString();
                        unvanCell.Text = dataTable.Rows[0]["Gorev"].ReturnEmptyIfNull().ToString();
                        kurumCell.Text = dataTable.Rows[0]["BirimSube"].ReturnEmptyIfNull().ToString();
                        telefonCell.Text = dataTable.Rows[0]["CepTelefonu"].ReturnEmptyIfNull().ToString();
                        epostaCell.Text = dataTable.Rows[0]["InternetEPosta"].ReturnEmptyIfNull().ToString();
                    }

                    tableRow.Controls.Add(siraNoCell);
                    tableRow.Controls.Add(adiSoyadiCell);
                    tableRow.Controls.Add(unvanCell);
                    tableRow.Controls.Add(kurumCell);
                    tableRow.Controls.Add(telefonCell);
                    tableRow.Controls.Add(epostaCell);
                    tableRow.Controls.Add(imzaCell);
                    KatilimciBilgileriTable.Rows.Add(tableRow);
                }
                int sayfasonu = siraNo + (10 - siraNo % 10);
                for (int i = siraNo; i < sayfasonu; i++)
                {
                    TableRow tableRow = new TableRow();
                    TableCell siraNoCell = new TableCell();
                    TableCell adiSoyadiCell = new TableCell();
                    TableCell unvanCell = new TableCell();
                    TableCell kurumCell = new TableCell();
                    TableCell telefonCell = new TableCell();
                    TableCell epostaCell = new TableCell();
                    TableCell imzaCell = new TableCell();

                    siraNoCell.BorderStyle = BorderStyle.Solid;
                    adiSoyadiCell.BorderStyle = BorderStyle.Solid;
                    unvanCell.BorderStyle = BorderStyle.Solid;
                    kurumCell.BorderStyle = BorderStyle.Solid;
                    telefonCell.BorderStyle = BorderStyle.Solid;
                    epostaCell.BorderStyle = BorderStyle.Solid;
                    imzaCell.BorderStyle = BorderStyle.Solid;

                    siraNoCell.BorderWidth = Unit.Parse("1px");

                    siraNoCell.Text = (++siraNo).ToString();


                    tableRow.Controls.Add(siraNoCell);
                    tableRow.Controls.Add(adiSoyadiCell);
                    tableRow.Controls.Add(unvanCell);
                    tableRow.Controls.Add(kurumCell);
                    tableRow.Controls.Add(telefonCell);
                    tableRow.Controls.Add(epostaCell);
                    tableRow.Controls.Add(imzaCell);
                    KatilimciBilgileriTable.Rows.Add(tableRow);
                }
            }
        }

        protected void ExportToExcel()
        {


            string filename = "ToplantiKatilimTutanagi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
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
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
    }
}
