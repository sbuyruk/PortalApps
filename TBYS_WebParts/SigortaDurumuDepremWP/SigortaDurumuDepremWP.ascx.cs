using Model.TBYS;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Utility.ProjeGlobal;


namespace TBYS_WebParts.SigortaDurumuDepremWP
{
    [ToolboxItemAttribute(false)]
    public partial class SigortaDurumuDepremWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public SigortaDurumuDepremWP()
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
            TabloyuDoldur();
            TahminiRayicTopTxt.Value = TahminiRayicToplaminiBul().ToString();
            EmlakBeyanTopTxt.Value = EmlakBeyanDegeriToplaminiBul().ToString();
            SigortaBedeliTopTxt.Value = SigortaBedeliToplaminiBul().ToString();
            PrimTopTxt.Value = PrimToplaminiBul().ToString();
        }
        protected void TabloyuDoldur()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            int GMTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.MULKIYETSEKLI_TM, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int GMCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.MULKIYETSEKLI_CM, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int IstTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.MULKIYETSEKLI_TM, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int IstCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.MULKIYETSEKLI_CM, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int IzmTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_IZMIR, ProjeConstants.MULKIYETSEKLI_TM, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int IzmCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_IZMIR, ProjeConstants.MULKIYETSEKLI_CM, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int MerTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_MERSIN, ProjeConstants.MULKIYETSEKLI_TM, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int MerCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_MERSIN, ProjeConstants.MULKIYETSEKLI_CM, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);

            GMTMCell.Text = (GMTM).ToString();
            GMCMCell.Text = (GMCM).ToString();
            GMTMCMTopCell.Text = (GMTM + GMCM).ToString();
            IstTMCell.Text = (IstTM).ToString();
            IstCMCell.Text = (IstCM).ToString();
            IstTMCMTopCell.Text = (IstTM + IstCM).ToString();
            IzmTMCell.Text = (IzmTM).ToString();
            IzmCMCell.Text = (IzmCM).ToString();
            IzmTMCMTopCell.Text = (IzmTM + IzmCM).ToString();
            MerTMCell.Text = (MerTM).ToString();
            MerCMCell.Text = (MerCM).ToString();
            MerTMCMTopCell.Text = (MerTM + MerCM).ToString();
            int TopTM = GMTM + IstTM + IzmTM + MerTM;
            TopTMCell.Text = (TopTM).ToString();
            int TopCM = GMCM + IstCM + IzmCM + MerCM;
            TopCMCell.Text = (TopCM).ToString();
            TopTMCMTopCell.Text = (TopTM + TopCM).ToString();

            int GMIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_ISHANI, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int GMApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_APT, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int GMMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_MESKEN, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int GMIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_ISYERI, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int GMArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_ARSA, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int GMTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_TARLA, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int GMMev = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_MEV, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);

            GMAptCell.Text = (GMApt + GMIshani).ToString();
            GMMesCell.Text = (GMMes).ToString();
            GMIsyCell.Text = (GMIsy).ToString();
            GMArsCell.Text = (GMArs).ToString();
            GMTarCell.Text = (GMTar).ToString();
            GMMevCell.Text = (GMMev).ToString();

            int IstIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_ISHANI, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int IstApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_APT, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int IstMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_MESKEN, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int IstIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_ISYERI, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int IstArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_ARSA, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int IstTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_TARLA, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int IstMev = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_MEV, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);

            IstAptCell.Text = (IstApt + IstIshani).ToString();
            IstMesCell.Text = (IstMes).ToString();
            IstIsyCell.Text = (IstIsy).ToString();
            IstArsCell.Text = (IstArs).ToString();
            IstTarCell.Text = (IstTar).ToString();
            IstMevCell.Text = (IstMev).ToString();

            int IzmIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_ISHANI, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int IzmApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_APT, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int IzmMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_MESKEN, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int IzmIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_ISYERI, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int IzmArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_ARSA, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int IzmTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_TARLA, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int IzmMev = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_MEV, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);

            IzmAptCell.Text = (IzmApt + IzmIshani).ToString();
            IzmMesCell.Text = (IzmMes).ToString();
            IzmIsyCell.Text = (IzmIsy).ToString();
            IzmArsCell.Text = (IzmArs).ToString();
            IzmTarCell.Text = (IzmTar).ToString();
            IzmMevCell.Text = (IzmMev).ToString();

            int MerIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_ISHANI, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int MerApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_APT, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int MerMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_MESKEN, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int MerIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_ISYERI, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int MerArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_ARSA, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int MerTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_TARLA, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            int MerMev = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_MEV, ProjeConstants.SIGORTA_DEPREM_IHTIYARI);

            MerAptCell.Text = (MerApt + MerIshani).ToString();
            MerMesCell.Text = (MerMes).ToString();
            MerIsyCell.Text = (MerIsy).ToString();
            MerArsCell.Text = (MerArs).ToString();
            MerTarCell.Text = (MerTar).ToString();
            MerMevCell.Text = (MerMev).ToString();

            TopAptCell.Text = (GMApt + IstApt + IzmApt + MerApt + GMIshani + IstIshani + IzmIshani + MerIshani).ToString();
            TopMesCell.Text = (GMMes + IstMes + IzmMes + MerMes).ToString();
            TopIsyCell.Text = (GMIsy + IstIsy + IzmIsy + MerIsy).ToString();
            TopArsCell.Text = (GMArs + IstArs + IzmArs + MerArs).ToString();
            TopTarCell.Text = (GMTar + IstTar + IzmTar + MerTar).ToString();
            TopMevCell.Text = (GMMev + IstMev + IzmMev + MerMev).ToString();
        }
        private decimal TahminiRayicToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectTahminiRayicToplamiBySigorta(ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            return toplam;
        }
        private decimal SigortaBedeliToplaminiBul()
        {
            decimal toplam = 0;
            Sigorta sigorta = new Sigorta();
            toplam = sigorta.SelectSigortaBedeliToplamiBySigorta(ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            return toplam;
        }
        private decimal PrimToplaminiBul()
        {
            decimal toplam = 0;
            Sigorta sigorta = new Sigorta();
            toplam = sigorta.SelectPirimToplamiBySigorta(ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            return toplam;
        }
        private decimal EmlakBeyanDegeriToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectEmlakBeyanDegeriToplamiBySigorta(ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            return toplam;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            string filename = "SigortaDurumuDeprem" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            TasDurTable.RenderControl(hw);

            Page.Response.ContentType = "application/vnd.ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            Page.Response.Write(tw.ToString());
            Page.Response.End();
        }
    }
}
