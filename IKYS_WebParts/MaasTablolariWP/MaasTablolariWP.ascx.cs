using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Model.TBYS;

namespace IKYS_WebParts.MaasTablolariWP
{
    [ToolboxItemAttribute(false)]
    public partial class MaasTablolariWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public MaasTablolariWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }

        #region Global Variables
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                TarihDDLDoldur();
                TablolariDoldur();
            }
        }

        private void TablolariDoldur()
        {
            Table1Title.Text = TarihDDL.SelectedItem.Text + " TARIHLERI ARASI GEÇERLI TSKGV ÜCRET TABLOSU-1";
            Table2Title.Text = TarihDDL.SelectedItem.Text + " TARIHLERI ARASI GEÇERLI TSKGV ÜCRET TABLOSU-2 (TSK'DAN EMEKLI PERSONEL IÇIN GEÇERLIDIR)";
            int grupId = TarihDDL.SelectedItem.Value.ConvertToInt();
            UcretTanim ucretTanim = new UcretTanim();
            int derece = 1;
            DataTable dataTable = ucretTanim.SelectKademe(derece, grupId);
            for (int i = 1; i <= dataTable.Rows.Count; i++)
            {

                UcretTanimTablosunuDoldur(grupId, i, UcretTanimTable1);
                UcretTanimTablosunuDoldur(grupId, i, UcretTanimTable2);
            }
        }
        #region Methods
        private void UcretTanimTablosunuDoldur(int grupId,int kademe, Table table)
        {
            int sira = 1;
            UcretTanim ucretTanim = new UcretTanim();
            List<UcretTanim> list = ucretTanim.SelectByKademe(grupId, kademe);
            TableRow row = new TableRow();
            TableCell kademeCell = new TableCell();
            kademeCell.Text = kademe.ToString();
            row.Controls.Add(kademeCell);
            foreach (UcretTanim item in list)
            {
                string gecerliUcret= (table == UcretTanimTable1)? item.UstUcret.ToString("N", culturInfo): item.AskerUcret.ToString("N", culturInfo);
                string tablo1AltUcret= item.AltUcret.ToString("N", culturInfo);
                switch (item.Derece)
                {
                    case 1: //Genel Müdür
                        {
                            if (table== UcretTanimTable1)
                            {
                                TableCell altUcretCell = new TableCell();

                                altUcretCell.Text = tablo1AltUcret;
                                altUcretCell.HorizontalAlign = HorizontalAlign.Right;  // Sag hizalama
                                row.Controls.Add(altUcretCell); 
                            }

                            TableCell ustUcretCell = new TableCell();
                            ustUcretCell.Text = gecerliUcret;
                            row.Controls.Add(ustUcretCell);
                            break;
                        }
                    case 2: //Genel Müdür Yard
                        {
                            if (table == UcretTanimTable1)
                            {
                                TableCell altUcretCell = new TableCell();

                                altUcretCell.Text = tablo1AltUcret;
                                altUcretCell.HorizontalAlign = HorizontalAlign.Right;  // Sag hizalama
                                row.Controls.Add(altUcretCell); 
                            }

                            TableCell ustUcretCell = new TableCell();
                            ustUcretCell.Text = gecerliUcret;
                            ustUcretCell.HorizontalAlign = HorizontalAlign.Right;  // Sag hizalama
                            row.Controls.Add(ustUcretCell);
                            break;
                        }
                    case 3: //Direk
                        {
                            if (table == UcretTanimTable1)
                            {
                                TableCell altUcretCell = new TableCell();

                                altUcretCell.Text = tablo1AltUcret;
                                altUcretCell.HorizontalAlign = HorizontalAlign.Right;  // Sag hizalama
                                row.Controls.Add(altUcretCell); 
                            }

                            TableCell ustUcretCell = new TableCell();
                            ustUcretCell.Text = gecerliUcret;
                            ustUcretCell.HorizontalAlign = HorizontalAlign.Right;  // Sag hizalama
                            row.Controls.Add(ustUcretCell);
                            break;
                        }
                    case 4: //Bas
                        {
                            TableCell ustUcretCell = new TableCell();
                            ustUcretCell.Text = gecerliUcret;
                            ustUcretCell.HorizontalAlign = HorizontalAlign.Right;  // Sag hizalama
                            row.Controls.Add(ustUcretCell);
                            break;
                        }
                    case 5: //Kd
                        {
                            TableCell ustUcretCell = new TableCell();
                            ustUcretCell.Text = gecerliUcret;
                            ustUcretCell.HorizontalAlign = HorizontalAlign.Right;  // Sag hizalama
                            row.Controls.Add(ustUcretCell);
                            break;
                        }
                    case 6: //Uzm
                        {
                            TableCell ustUcretCell = new TableCell();
                            ustUcretCell.Text = gecerliUcret;
                            row.Controls.Add(ustUcretCell);
                            break;
                        }
                    case 7: //Uzm Yrd
                        {
                            TableCell ustUcretCell = new TableCell();
                            ustUcretCell.Text = gecerliUcret;
                            ustUcretCell.HorizontalAlign = HorizontalAlign.Right;  // Sag hizalama
                            row.Controls.Add(ustUcretCell);
                            break;
                        }
                    case 8: //Soför Hiz
                        {

                            TableCell ustUcretCell = new TableCell();
                            ustUcretCell.Text = gecerliUcret;
                            ustUcretCell.HorizontalAlign = HorizontalAlign.Right;  // Sag hizalama
                            row.Controls.Add(ustUcretCell);
                            break;
                        }

                    default:
                        break;
                }
                table.Controls.Add(row);

            }
        }
        private void TarihDDLDoldur()
        {
            TarihDDL.Items.Clear();
            UcretTanim ucretTanim = new UcretTanim();
            DataTable dataTable = ucretTanim.SelectByGrup();
            foreach (DataRow row in dataTable.Rows)
            {
                DateTime bastar = row["BaslangicTarihi"].ConvertToDatetime();
                DateTime bittar = row["BitisTarihi"].ConvertToDatetime();
                int grupId = row["GrupId"].ConvertToInt();
                ListItem li = new ListItem(bastar.ConvertToDatetimeEmptyIfNull() + " - " +bittar.ConvertToDatetimeEmptyIfNull(), grupId.ToString());
                TarihDDL.Items.Add(li);
            }

        }
        #endregion
        #region Events
        protected void ExportTablo1ToExcelBtn_Click(object sender, EventArgs e)
        {

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Current.Response.Charset = "utf-8";
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=UcretTablosu1.xls");

            // Excel'e stil bilgisi göndermek için <style> blogu ekle
            string style = @"
                <meta http-equiv='Content-Type' content='text/html; charset=utf-8'>
                <style>
                    table, th, td {
                        border: 1px solid black;
                        border-collapse: collapse;
                    }
                    th, td {
                        padding: 5px;
                        text-align: center;
                    }
                </style>";
            TablolariDoldur();
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            // Tabloyu render et
            TablesDiv.RenderControl(hw);

            // HTML + style + tablo içerigini gönder
            HttpContext.Current.Response.Write(style + sw.ToString());
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

        }
        protected void ExportTablo2ToExcelBtn_Click(object sender, EventArgs e)
        {

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Current.Response.Charset = "utf-8";
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=UcretTablosu-2.xls");

            // Excel'e stil bilgisi göndermek için <style> blogu ekle
            string style = @"
                <meta http-equiv='Content-Type' content='text/html; charset=utf-8'>
                <style>
                    table, th, td {
                        border: 1px solid black;
                        border-collapse: collapse;
                    }
                    th, td {
                        padding: 5px;
                        text-align: center;
                    }
                </style>";

            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            // Tabloyu render et
            UcretTanimTable2.RenderControl(hw);

            // HTML + style + tablo içerigini gönder
            HttpContext.Current.Response.Write(style + sw.ToString());
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

        }
        protected void TarihDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TablolariDoldur();
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        #endregion

    }
}

