using Model.TBYS;
using System;
using System.ComponentModel;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinmazDurumuTablosunuDoldur();
            decimal tahminiRayicToplami = TahminiRayicToplaminiBul();
            TahminiRayicTopTxt.Value = tahminiRayicToplami.ReturnEmptyIfZeroOrNull().ToString();
            decimal emlakBeyanToplami = EmlakBeyanDeğeriToplaminiBul();
            EmlakBeyanTopTxt.Value = emlakBeyanToplami.ReturnEmptyIfZeroOrNull().ToString();
        }
        private decimal TahminiRayicToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectTahminiRayicToplami(string.Empty);
            return toplam;
        }
        private decimal EmlakBeyanDeğeriToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectEmlakBeyanDegeriToplami(string.Empty);
            return toplam;
        }
        protected void TasinmazDurumuTablosunuDoldur()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            int GMTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.MULKIYETSEKLI_TM);
            int GMCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.MULKIYETSEKLI_CM);
            int IstTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.MULKIYETSEKLI_TM);
            int IstCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.MULKIYETSEKLI_CM);
            int IzmTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_IZMIR, ProjeConstants.MULKIYETSEKLI_TM);
            int IzmCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_IZMIR, ProjeConstants.MULKIYETSEKLI_CM);
            int MerTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_MERSIN, ProjeConstants.MULKIYETSEKLI_TM);
            int MerCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_MERSIN, ProjeConstants.MULKIYETSEKLI_CM);

            GMTMCell.Text = GMTM.ReturnEmptyIfZeroOrNull().ToString();
            GMCMCell.Text = GMCM.ReturnEmptyIfZeroOrNull().ToString();
            GMTMCMTopCell.Text = (GMTM + GMCM).ReturnEmptyIfZeroOrNull().ToString();
            IstTMCell.Text = IstTM.ReturnEmptyIfZeroOrNull().ToString();
            IstCMCell.Text = IstCM.ReturnEmptyIfZeroOrNull().ToString();
            IstTMCMTopCell.Text = (IstTM + IstCM).ReturnEmptyIfZeroOrNull().ToString();
            IzmTMCell.Text = IzmTM.ReturnEmptyIfZeroOrNull().ToString();
            IzmCMCell.Text = IzmCM.ReturnEmptyIfZeroOrNull().ToString();
            IzmTMCMTopCell.Text = (IzmTM + IzmCM).ReturnEmptyIfZeroOrNull().ToString();
            MerTMCell.Text = MerTM.ReturnEmptyIfZeroOrNull().ToString();
            MerCMCell.Text = MerCM.ReturnEmptyIfZeroOrNull().ToString();
            MerTMCMTopCell.Text = (MerTM + MerCM).ReturnEmptyIfZeroOrNull().ToString();
            int TopTM = GMTM + IstTM + IzmTM + MerTM;
            TopTMCell.Text = TopTM.ReturnEmptyIfZeroOrNull().ToString();
            int TopCM = GMCM + IstCM + IzmCM + MerCM;
            TopCMCell.Text = TopCM.ReturnEmptyIfZeroOrNull().ToString();
            TopTMCMTopCell.Text = (TopTM + TopCM).ReturnEmptyIfZeroOrNull().ToString();
            string kiraDurumuStr = string.Empty;
            int GMApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_APT, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int GMIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_ISHANI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int GMMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_MESKEN, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int GMIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_ISYERI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int GMArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_ARSA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int GMTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_TARLA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            //int GMMev = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_MEV, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);

            GMAptCell.Text = (GMApt + GMIshani).ReturnEmptyIfZeroOrNull().ToString();
            GMMesCell.Text = GMMes.ReturnEmptyIfZeroOrNull().ToString();
            GMIsyCell.Text = GMIsy.ReturnEmptyIfZeroOrNull().ToString();
            GMArsCell.Text = GMArs.ReturnEmptyIfZeroOrNull().ToString();
            GMTarCell.Text = GMTar.ReturnEmptyIfZeroOrNull().ToString();
            //GMMevCell.Text = GMMev.ReturnEmptyIfZeroOrNull().ToString();

            int IstApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_APT, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_ISHANI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_MESKEN, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_ISYERI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_ARSA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_TARLA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            //int IstMev = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_MEV, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);

            IstAptCell.Text = (IstApt + IstIshani).ReturnEmptyIfZeroOrNull().ToString();
            IstMesCell.Text = (IstMes).ReturnEmptyIfZeroOrNull().ToString();
            IstIsyCell.Text = (IstIsy).ReturnEmptyIfZeroOrNull().ToString();
            IstArsCell.Text = (IstArs).ReturnEmptyIfZeroOrNull().ToString();
            IstTarCell.Text = (IstTar).ReturnEmptyIfZeroOrNull().ToString();
            //IstMevCell.Text = (IstMev).ReturnEmptyIfZeroOrNull().ToString();

            int IzmApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_APT, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_ISHANI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_MESKEN, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_ISYERI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_ARSA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_TARLA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            //int IzmMev = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_MEV, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);

            IzmAptCell.Text = (IzmApt + IzmIshani).ReturnEmptyIfZeroOrNull().ToString();
            IzmMesCell.Text = (IzmMes).ReturnEmptyIfZeroOrNull().ToString();
            IzmIsyCell.Text = (IzmIsy).ReturnEmptyIfZeroOrNull().ToString();
            IzmArsCell.Text = (IzmArs).ReturnEmptyIfZeroOrNull().ToString();
            IzmTarCell.Text = (IzmTar).ReturnEmptyIfZeroOrNull().ToString();
            //IzmMevCell.Text = (IzmMev).ReturnEmptyIfZeroOrNull().ToString();

            int MerApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_APT, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_ISHANI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_MESKEN, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_ISYERI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_ARSA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_TARLA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            //int MerMev = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_MEV, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);

            MerAptCell.Text = (MerApt + MerIshani).ReturnEmptyIfZeroOrNull().ToString();
            MerMesCell.Text = (MerMes).ReturnEmptyIfZeroOrNull().ToString();
            MerIsyCell.Text = (MerIsy).ReturnEmptyIfZeroOrNull().ToString();
            MerArsCell.Text = (MerArs).ReturnEmptyIfZeroOrNull().ToString();
            MerTarCell.Text = (MerTar).ReturnEmptyIfZeroOrNull().ToString();
            //MerMevCell.Text = (MerMev).ReturnEmptyIfZeroOrNull().ToString();

            TopAptCell.Text = (GMApt + IstApt + IzmApt + MerApt + GMIshani + IstIshani + IzmIshani + MerIshani).ReturnEmptyIfZeroOrNull().ToString();
            TopMesCell.Text = (GMMes + IstMes + IzmMes + MerMes).ReturnEmptyIfZeroOrNull().ToString();
            TopIsyCell.Text = (GMIsy + IstIsy + IzmIsy + MerIsy).ReturnEmptyIfZeroOrNull().ToString();
            TopArsCell.Text = (GMArs + IstArs + IzmArs + MerArs).ReturnEmptyIfZeroOrNull().ToString();
            TopTarCell.Text = (GMTar + IstTar + IzmTar + MerTar).ReturnEmptyIfZeroOrNull().ToString();
            //TopMevCell.Text = (GMMev + IstMev + IzmMev + MerMev).ReturnEmptyIfZeroOrNull().ToString();
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
