using Model.Ortak;
using Model.TBYS;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.TasinmazDurumRaporuWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazDurumRaporuWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazDurumRaporuWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private int BolgeIdQS
        {
            get
            {

                if (ViewState["BolgeId"] == null)
                {
                    if (Page.Request.QueryString["BolgeId"] != null)
                    {
                        ViewState["BolgeId"] = Page.Request.QueryString["BolgeId"];
                    }
                    else
                    {
                        ViewState["BolgeId"] = string.Empty;
                    }
                }
                return ViewState["BolgeId"].ReturnZeroIfNull().ConvertToInt();
            }

            set
            {
                ViewState["BolgeId"] = value;
            }
        }
        private string CurrentUserName
        {
            get
            {

                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUserLoginName();
                }
                return ViewState["CurrentUserName"].ToString();
            }

            set
            {
                ViewState["CurrentUserName"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Bolge bolge = IKYSOrtak.BolgeGetirByUserName(CurrentUserName);
            BolgeIdQS = bolge == null ? 0 : bolge.Id;
            TasinmazDurumuTablosunuDoldur();
            decimal tahminiRayicToplami = TahminiRayicToplaminiBul();
            TahminiRayicTopTxt.Value = tahminiRayicToplami.ReturnEmptyIfZeroOrNull().ToString();
            decimal emlakBeyanToplami = EmlakBeyanDeğeriToplaminiBul();
            EmlakBeyanTopTxt.Value = emlakBeyanToplami.ReturnEmptyIfZeroOrNull().ToString();
            decimal muhasebeyeKayitliToplami = MuhasebeyeKayitliToplaminiBul();
            MuhasebeyeKayitliTopTxt.Value = muhasebeyeKayitliToplami.ReturnEmptyIfZeroOrNull().ToString();
            decimal yaklasikPiyasaToplami = YaklasikPiyasaToplaminiBul();
            TahminiPiyasaTopTxt.Value = yaklasikPiyasaToplami.ReturnEmptyIfZeroOrNull().ToString();
        }
        private decimal TahminiRayicToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectTahminiRayicToplami(BolgeIdQS);
            return toplam;
        }
        private decimal MuhasebeyeKayitliToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectMuhasebeyeKayitliDegerToplami(BolgeIdQS);
            return toplam;
        }
        private decimal EmlakBeyanDeğeriToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectEmlakBeyanDegeriToplami(BolgeIdQS);
            return toplam;
        }
        private decimal YaklasikPiyasaToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectYaklasikPiyasaToplami(BolgeIdQS);
            return toplam;
        }
        protected void TasinmazDurumuTablosunuDoldur()
        {
            try
            {
                // Find totals row (contains TopBaslikCell)
                TableRow totalsRow = TopBaslikCell.Parent as TableRow;
                int headerRows = 2; // first two header rows are static
                int totalsIndex = -1;
                for (int i = 0; i < TasDurTable.Rows.Count; i++)
                {
                    if (TasDurTable.Rows[i] == totalsRow)
                    {
                        totalsIndex = i;
                        break;
                    }
                }

                if (totalsIndex == -1)
                {
                    // if not found, append to end
                    totalsIndex = TasDurTable.Rows.Count;
                }

                // Remove any existing region data rows between headers and totals row
                for (int i = totalsIndex - 1; i >= headerRows; i--)
                {
                    TasDurTable.Rows.RemoveAt(i);
                }

                // Prepare totals accumulators
                int totalTM = 0;
                int totalCM = 0;
                int totalApt = 0;
                int totalMes = 0;
                int totalIsy = 0;
                int totalArs = 0;
                int totalTar = 0;

                Tasinmaz tasinmaz = new Tasinmaz();

                // Get active regions based on user permission/context
                Bolge bolgeDao = new Bolge();
                var bolgeList = bolgeDao.SelectAktifBolgeler(BolgeIdQS);

                // Insert a row per bolge before totals row
                int insertIndex = headerRows;
                foreach (var bolge in bolgeList)
                {
                    if (bolge.Id == ProjeConstants.BOLGE_GENELMUDURLUK_INT ||
                        bolge.Id == ProjeConstants.BOLGE_YURTDISI_INT)
                        continue;
                    int ankTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(bolge.Id, ProjeConstants.MULKIYETSEKLI_TM);
                    int ankCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(bolge.Id, ProjeConstants.MULKIYETSEKLI_CM);

                    // usage counts (APT includes ISHANI)
                    string kiraDurumuStr = string.Empty;
                    int apt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(bolge.Id, ProjeConstants.KULLANIMSEKLI_APT, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
                    int ishani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(bolge.Id, ProjeConstants.KULLANIMSEKLI_ISHANI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
                    int mes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(bolge.Id, ProjeConstants.KULLANIMSEKLI_MESKEN, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
                    int isy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(bolge.Id, ProjeConstants.KULLANIMSEKLI_ISYERI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
                    int ars = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(bolge.Id, ProjeConstants.KULLANIMSEKLI_ARSA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
                    int tar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(bolge.Id, ProjeConstants.KULLANIMSEKLI_TARLA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);

                    int tm = ankTM;
                    int cm = ankCM;
                    int tmcmTop = tm + cm;
                    int aptTotal = apt + ishani;

                    // Build row
                    TableRow row = new TableRow { HorizontalAlign = HorizontalAlign.Center };

                    TableCell bolgeCell = new TableCell { CssClass = "btn-primary", Text = string.IsNullOrEmpty(bolge.KisaAdi) ? bolge.Adi : bolge.KisaAdi };
                    row.Cells.Add(bolgeCell);

                    TableCell tmCell = new TableCell { Text = tm.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(tmCell);

                    TableCell cmCell = new TableCell { Text = cm.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(cmCell);

                    TableCell tmcmTopCell = new TableCell { Text = tmcmTop.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(tmcmTopCell);

                    TableCell aptCell = new TableCell { Text = aptTotal.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(aptCell);

                    TableCell mesCell = new TableCell { Text = mes.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(mesCell);

                    TableCell isyCell = new TableCell { Text = isy.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(isyCell);

                    TableCell arsCell = new TableCell { Text = ars.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(arsCell);

                    TableCell tarCell = new TableCell { Text = tar.ReturnEmptyIfZeroOrNull().ToString() };
                    row.Cells.Add(tarCell);

                    // Insert before totals row
                    TasDurTable.Rows.AddAt(insertIndex++, row);

                    // Accumulate totals
                    totalTM += tm;
                    totalCM += cm;
                    totalApt += aptTotal;
                    totalMes += mes;
                    totalIsy += isy;
                    totalArs += ars;
                    totalTar += tar;
                }

                // Fill totals cells (Top*)
                TopTMCell.Text = totalTM.ReturnEmptyIfZeroOrNull().ToString();
                TopCMCell.Text = totalCM.ReturnEmptyIfZeroOrNull().ToString();
                TopTMCMTopCell.Text = (totalTM + totalCM).ReturnEmptyIfZeroOrNull().ToString();
                TopAptCell.Text = totalApt.ReturnEmptyIfZeroOrNull().ToString();
                TopMesCell.Text = totalMes.ReturnEmptyIfZeroOrNull().ToString();
                TopIsyCell.Text = totalIsy.ReturnEmptyIfZeroOrNull().ToString();
                TopArsCell.Text = totalArs.ReturnEmptyIfZeroOrNull().ToString();
                TopTarCell.Text = totalTar.ReturnEmptyIfZeroOrNull().ToString();
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
            string filename = "TasinmazDurumRaporu" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            TasDurTable.RenderControl(hw);
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