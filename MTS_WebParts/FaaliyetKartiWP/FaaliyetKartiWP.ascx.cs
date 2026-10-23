using MTS_WebParts.Logging;
using Utility.Logging;
using System.Web;
using Model.IKYS;
using Model.MTS;
using Model.NBYS;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Model.Services.MTS;

namespace MTS_WebParts.FaaliyetKartiWP
{
    [ToolboxItemAttribute(false)]
    public partial class FaaliyetKartiWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("MTS", "FaaliyetKartiWP")); } }
        private static string Encode(object value) { return HttpUtility.HtmlEncode(Convert.ToString(value)); }

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FaaliyetKartiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string FaaliyetIdQS
        {
            get
            {

                if (ViewState["FaaliyetId"] == null)
                {
                    if (Page.Request.QueryString["FaaliyetId"] != null)
                    {
                        ViewState["FaaliyetId"] = Page.Request.QueryString["FaaliyetId"];
                    }
                    else
                    {
                        ViewState["FaaliyetId"] = string.Empty;
                    }
                }
                return ViewState["FaaliyetId"].ToString();
            }

            set
            {
                ViewState["FaaliyetId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) MtsWebPartLog.RecordOpened(LogContext);
                int id;
                if (!int.TryParse(FaaliyetIdQS, out id) || id <= 0)
                    throw new MtsInputException("Geçerli bir faaliyet seçin. Faaliyet listesinden kaydı yeniden açın.");
                IdLbl.Text = " ( Faaliyet No: " + id + " )";
                while (FaaliyetKartiBilgileriTable.Rows.Count > 1) FaaliyetKartiBilgileriTable.Rows.RemoveAt(1);
                while (AniObjesiBilgileriTable.Rows.Count > 2) AniObjesiBilgileriTable.Rows.RemoveAt(2);
                FaaliyetBilgileriniDoldur();
                AniObjesiBilgileriTableDoldur();
            });
            MainCardDiv.Visible = !MtsWebPartLog.HasFailed(LogContext);
            ExcelBtn.Enabled = MainCardDiv.Visible;
        }
        private void AniObjesiBilgileriTableDoldur()
        {
            /***
             * FaaliyetKatilim_Table'dan Bu Faaliyetya katilanlari getir
             * **/
            FaaliyetKatilimService faaliyetKatilimService = new FaaliyetKatilimService(LogContext);
            List<FaaliyetKatilim> liste = faaliyetKatilimService.GetByFaaliyetId(FaaliyetIdQS.ConvertToInt());
            foreach (var item in liste)
            {
                TableRow tableRow = new TableRow();
                TableCell katilimciAdiSoyadiCell = new TableCell();
                TableCell kurumuCell = new TableCell();
                TableCell verilenAniObjesiCell = new TableCell();
                TableCell getirilenAniObjesiCell = new TableCell();
                ////////////////
                Kisi kisi = new Kisi();
                kisi = new Model.Services.MTS.KisiService(LogContext).GetById(item.KatilimciId);
                if (kisi != null)
                {
                    katilimciAdiSoyadiCell.Text = Encode(kisi.Adi + " " + kisi.Soyadi);
                    kurumuCell.Text = Encode(item.KurumGorev);

                    //Burada verilen ani objeleri aliniyor
                    AniObjesiDagitimService aniObjesiDagitimService = new AniObjesiDagitimService(LogContext);
                    DataTable dataTable = aniObjesiDagitimService.GetDistributionTable(FaaliyetIdQS.ConvertToInt(), kisi.Id);
                    if (dataTable != null)
                    {
                        string objeStr = string.Empty;
                        foreach (DataRow row in dataTable.Rows)
                        {
                            int adet = row["Adet"].ReturnZeroIfNull().ConvertToInt();
                            if (adet > 0)
                            {
                                string deger = row["Adi"].ToString();
                                objeStr += " - " + deger + "(" + adet + ")";
                            }

                        }
                        verilenAniObjesiCell.Text = Encode(objeStr);
                    }

                    //Getirilen Ani Objeleri ayrica aliniyor
                    AniObjesiDagitim getirilenAniObjesi = aniObjesiDagitimService.GetGetirilen(FaaliyetIdQS.ConvertToInt(), kisi.Id);
                    if (getirilenAniObjesi != null)
                    {
                        getirilenAniObjesiCell.Text = Encode(getirilenAniObjesi.GetirilenAniObjesi);
                    }
                }
                else
                {
                    katilimciAdiSoyadiCell.Text = Encode("Kişi kaydı bulunamadı.");
                    kurumuCell.Text = Encode(item.KurumGorev);
                }

                tableRow.Controls.Add(katilimciAdiSoyadiCell);
                tableRow.Controls.Add(kurumuCell);
                tableRow.Controls.Add(verilenAniObjesiCell);
                tableRow.Controls.Add(getirilenAniObjesiCell);
                AniObjesiBilgileriTable.Rows.Add(tableRow);
            }

        }
        private void FaaliyetBilgileriniDoldur()
        {
            Faaliyet faaliyet = new FaaliyetService(LogContext).GetById(FaaliyetIdQS.ConvertToInt());


            if (faaliyet != null)
            {
                TableRow row1 = new TableRow();
                TableRow row2 = new TableRow();
                TableRow row3 = new TableRow();

                TableCell r1c1 = new TableCell();
                TableCell r1c2 = new TableCell();
                TableCell r1c3 = new TableCell();


                row1.Controls.Add(r1c1);
                row1.Controls.Add(r1c2);
                row1.Controls.Add(r1c3);


                TableCell r2c1 = new TableCell();
                TableCell r2c2 = new TableCell();
                TableCell r2c3 = new TableCell();


                row2.Controls.Add(r2c1);
                row2.Controls.Add(r2c2);
                row2.Controls.Add(r2c3);


                TableCell r3c1 = new TableCell();
                TableCell r3c2 = new TableCell();

                row3.Controls.Add(r3c1);
                row3.Controls.Add(r3c2);


                r1c1.Text = Encode("Faaliyet Tipi : " + faaliyet.FaaliyetTipi);
                r1c2.Text = Encode("Faaliyet Tarihi : " + faaliyet.BaslangicTarihi.ToString("dd-MM-yyyy") + "-" + faaliyet.BitisTarihi.ToString("dd-MM-yyyy"));
                r1c3.Text = Encode("Faaliyet Saati : " + faaliyet.BaslangicSaati + "-" + faaliyet.BitisSaati);


                r2c1.Text = Encode("Faaliyet Yeri : " + (faaliyet.FaaliyetYeriStr== null ? "" : faaliyet.FaaliyetYeriStr));
                r2c2.Text = Encode("Faaliyet Amaci : " + MTSOrtak.ParseFaaliyetAmaci(faaliyet.FaaliyetAmaciId.ToString()));
                r2c3.Text = Encode("Faaliyet Durumu : " + MTSOrtak.ParseFaaliyetDurumu(faaliyet.FaaliyetDurumu.ConvertToInt()));

                r3c1.Text = Encode("Faaliyet Konusu : " + faaliyet.FaaliyetKonusu);
                r3c2.Text = Encode("Açiklama : " + faaliyet.Aciklama);
                r3c2.ColumnSpan = 2;

                BorderEkle(row1);
                BorderEkle(row2);
                BorderEkle(row3);

                FaaliyetKartiBilgileriTable.Controls.Add(row1);
                FaaliyetKartiBilgileriTable.Controls.Add(row2);
                FaaliyetKartiBilgileriTable.Controls.Add(row3);
            }
            else
            {
                throw new MtsInputException("Faaliyet bulunamadı. Faaliyet listesini yenileyip yeniden seçin.");
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);

            });
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "ExcelBtn_Click", () =>
            {

                ExportToExcel();


            });
        }
        protected void ExportToExcel()
        {

            string filename = "FaaliyetKarti" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
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
        private void BorderEkle(TableRow row)
        {
            for (int n = 0; n < row.Cells.Count; n++)
            {
                row.Cells[n].BorderStyle = BorderStyle.Solid;
            }
        }
        private void AlignCells(TableRow row)
        {
            for (int n = 0; n < row.Cells.Count; n++)
            {
                row.VerticalAlign = VerticalAlign.Middle;
                row.HorizontalAlign = HorizontalAlign.Center;
            }

        }
        protected void FaaliyetListesiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FAALIYET_LIST);

            });
        }
        protected void FaaliyetTakvimiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetTakvimiBtn_Click", () =>
            {
                Faaliyet faaliyet = new FaaliyetService(LogContext).GetById(FaaliyetIdQS.ConvertToInt());
                if (faaliyet == null) throw new MtsInputException("Faaliyet bulunamadı. Listeyi yenileyin.");
                RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM + "?InitialDate=" + faaliyet.BaslangicTarihi.ToString("yyyy-MM-dd"));

            });
        }
        private void RedirectToPage(string pageUrl)
        {

                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);

        }
        protected void FaaliyeteGitBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyeteGitBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FAALIYET_GIRIS + "?FaaliyetId=" + FaaliyetIdQS);

            });
        }
    }
}
