using Model.TBYS;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.SigortaDurumuDaskWP
{
    [ToolboxItemAttribute(false)]
    public partial class SigortaDurumuDaskWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public SigortaDurumuDaskWP()
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
            int AnkTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.MULKIYETSEKLI_TM, ProjeConstants.SIGORTA_DASK);
            int AnkCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.MULKIYETSEKLI_CM, ProjeConstants.SIGORTA_DASK);
            int IstTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.MULKIYETSEKLI_TM, ProjeConstants.SIGORTA_DASK);
            int IstCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.MULKIYETSEKLI_CM, ProjeConstants.SIGORTA_DASK);
            int IzmTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.MULKIYETSEKLI_TM, ProjeConstants.SIGORTA_DASK);
            int IzmCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.MULKIYETSEKLI_CM, ProjeConstants.SIGORTA_DASK);
            int MerTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.MULKIYETSEKLI_TM, ProjeConstants.SIGORTA_DASK);
            int MerCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.MULKIYETSEKLI_CM, ProjeConstants.SIGORTA_DASK);

            int ErzTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.MULKIYETSEKLI_TM, ProjeConstants.SIGORTA_DASK);
            int ErzCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekliSigorta(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.MULKIYETSEKLI_CM, ProjeConstants.SIGORTA_DASK);

            AnkTMCell.Text = (AnkTM).ToString();
            AnkCMCell.Text = (AnkCM).ToString();
            AnkTMCMTopCell.Text = (AnkTM + AnkCM).ToString();
            IstTMCell.Text = (IstTM).ToString();
            IstCMCell.Text = (IstCM).ToString();
            IstTMCMTopCell.Text = (IstTM + IstCM).ToString();
            IzmTMCell.Text = (IzmTM).ToString();
            IzmCMCell.Text = (IzmCM).ToString();
            IzmTMCMTopCell.Text = (IzmTM + IzmCM).ToString();
            MerTMCell.Text = (MerTM).ToString();
            MerCMCell.Text = (MerCM).ToString();
            MerTMCMTopCell.Text = (MerTM + MerCM).ToString();
            ErzTMCell.Text = (ErzTM).ToString();
            ErzCMCell.Text = (ErzCM).ToString();
            ErzTMCMTopCell.Text = (ErzTM + ErzCM).ToString();
            int TopTM = AnkTM + IstTM + IzmTM + MerTM + ErzTM;
            TopTMCell.Text = (TopTM).ToString();
            int TopCM = AnkCM + IstCM + IzmCM + MerCM + ErzCM;
            TopCMCell.Text = (TopCM).ToString();
            TopTMCMTopCell.Text = (TopTM + TopCM).ToString();

            int AnkIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_ISHANI, ProjeConstants.SIGORTA_DASK);
            int AnkApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_APT, ProjeConstants.SIGORTA_DASK);
            int AnkMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_MESKEN, ProjeConstants.SIGORTA_DASK);
            int AnkIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_ISYERI, ProjeConstants.SIGORTA_DASK);
            int AnkArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_ARSA, ProjeConstants.SIGORTA_DASK);
            int AnkTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_TARLA, ProjeConstants.SIGORTA_DASK);

            AnkAptCell.Text = (AnkApt + AnkIshani).ToString();
            AnkMesCell.Text = (AnkMes).ToString();
            AnkIsyCell.Text = (AnkIsy).ToString();
            AnkArsCell.Text = (AnkArs).ToString();
            AnkTarCell.Text = (AnkTar).ToString();

            int IstIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_ISHANI, ProjeConstants.SIGORTA_DASK);
            int IstApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_APT, ProjeConstants.SIGORTA_DASK);
            int IstMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_MESKEN, ProjeConstants.SIGORTA_DASK);
            int IstIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_ISYERI, ProjeConstants.SIGORTA_DASK);
            int IstArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_ARSA, ProjeConstants.SIGORTA_DASK);
            int IstTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_TARLA, ProjeConstants.SIGORTA_DASK);

            IstAptCell.Text = (IstApt + IstIshani).ToString();
            IstMesCell.Text = (IstMes).ToString();
            IstIsyCell.Text = (IstIsy).ToString();
            IstArsCell.Text = (IstArs).ToString();
            IstTarCell.Text = (IstTar).ToString();

            int IzmIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_ISHANI, ProjeConstants.SIGORTA_DASK);
            int IzmApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_APT, ProjeConstants.SIGORTA_DASK);
            int IzmMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_MESKEN, ProjeConstants.SIGORTA_DASK);
            int IzmIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_ISYERI, ProjeConstants.SIGORTA_DASK);
            int IzmArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_ARSA, ProjeConstants.SIGORTA_DASK);
            int IzmTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_TARLA, ProjeConstants.SIGORTA_DASK);

            IzmAptCell.Text = (IzmApt + IzmIshani).ToString();
            IzmMesCell.Text = (IzmMes).ToString();
            IzmIsyCell.Text = (IzmIsy).ToString();
            IzmArsCell.Text = (IzmArs).ToString();
            IzmTarCell.Text = (IzmTar).ToString();

            int MerIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_ISHANI, ProjeConstants.SIGORTA_DASK);
            int MerApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_APT, ProjeConstants.SIGORTA_DASK);
            int MerMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_MESKEN, ProjeConstants.SIGORTA_DASK);
            int MerIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_ISYERI, ProjeConstants.SIGORTA_DASK);
            int MerArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_ARSA, ProjeConstants.SIGORTA_DASK);
            int MerTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_TARLA, ProjeConstants.SIGORTA_DASK);

            MerAptCell.Text = (MerApt + MerIshani).ToString();
            MerMesCell.Text = (MerMes).ToString();
            MerIsyCell.Text = (MerIsy).ToString();
            MerArsCell.Text = (MerArs).ToString();
            MerTarCell.Text = (MerTar).ToString();

            int ErzIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_ISHANI, ProjeConstants.SIGORTA_DASK);
            int ErzApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_APT, ProjeConstants.SIGORTA_DASK);
            int ErzMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_MESKEN, ProjeConstants.SIGORTA_DASK);
            int ErzIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_ISYERI, ProjeConstants.SIGORTA_DASK);
            int ErzArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_ARSA, ProjeConstants.SIGORTA_DASK);
            int ErzTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliSigorta(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_TARLA, ProjeConstants.SIGORTA_DASK);

            ErzAptCell.Text = (ErzApt + ErzIshani).ToString();
            ErzMesCell.Text = (ErzMes).ToString();
            ErzIsyCell.Text = (ErzIsy).ToString();
            ErzArsCell.Text = (ErzArs).ToString();
            ErzTarCell.Text = (ErzTar).ToString();

            TopAptCell.Text = (AnkApt + IstApt + IzmApt + MerApt + AnkIshani + IstIshani + IzmIshani + MerIshani + ErzApt + ErzIshani).ToString();
            TopMesCell.Text = (AnkMes + IstMes + IzmMes + MerMes + ErzMes).ToString();
            TopIsyCell.Text = (AnkIsy + IstIsy + IzmIsy + MerIsy + ErzIsy).ToString();
            TopArsCell.Text = (AnkArs + IstArs + IzmArs + MerArs + ErzArs).ToString();
            TopTarCell.Text = (AnkTar + IstTar + IzmTar + MerTar + ErzTar).ToString();
        }
        private decimal TahminiRayicToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectTahminiRayicToplamiBySigorta(ProjeConstants.SIGORTA_DASK);
            return toplam;
        }
        private decimal SigortaBedeliToplaminiBul()
        {
            decimal toplam = 0;
            Sigorta sigorta = new Sigorta();
            toplam = sigorta.SelectSigortaBedeliToplamiBySigorta(ProjeConstants.SIGORTA_DASK);
            return toplam;
        }
        private decimal PrimToplaminiBul()
        {
            decimal toplam = 0;
            Sigorta sigorta = new Sigorta();
            toplam = sigorta.SelectPirimToplamiBySigorta(ProjeConstants.SIGORTA_DASK);
            return toplam;
        }
        private decimal EmlakBeyanDegeriToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectEmlakBeyanDegeriToplamiBySigorta(ProjeConstants.SIGORTA_DASK);
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
            string filename = "SigortaDurumuDask" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
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
