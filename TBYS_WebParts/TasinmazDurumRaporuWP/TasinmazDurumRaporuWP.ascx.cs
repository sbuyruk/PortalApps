using Model.Ortak;
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
        }
        private decimal TahminiRayicToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectTahminiRayicToplami(BolgeIdQS);
            return toplam;
        }
        private decimal EmlakBeyanDeğeriToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectEmlakBeyanDegeriToplami(BolgeIdQS);
            return toplam;
        }
        protected void TasinmazDurumuTablosunuDoldur()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            int AnkTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.MULKIYETSEKLI_TM);
            int AnkCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.MULKIYETSEKLI_CM);
            int IstTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.MULKIYETSEKLI_TM);
            int IstCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.MULKIYETSEKLI_CM);
            int IzmTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.MULKIYETSEKLI_TM);
            int IzmCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.MULKIYETSEKLI_CM);
            int MerTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.MULKIYETSEKLI_TM);
            int MerCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.MULKIYETSEKLI_CM);
            int ErzTM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.MULKIYETSEKLI_TM);
            int ErzCM = tasinmaz.SelectTasinmazAdetByBolgeMulkiyetSekli(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.MULKIYETSEKLI_CM);

            AnkTMCell.Text = AnkTM.ReturnEmptyIfZeroOrNull().ToString();
            AnkCMCell.Text = AnkCM.ReturnEmptyIfZeroOrNull().ToString();
            AnkTMCMTopCell.Text = (AnkTM + AnkCM).ReturnEmptyIfZeroOrNull().ToString();
            IstTMCell.Text = IstTM.ReturnEmptyIfZeroOrNull().ToString();
            IstCMCell.Text = IstCM.ReturnEmptyIfZeroOrNull().ToString();
            IstTMCMTopCell.Text = (IstTM + IstCM).ReturnEmptyIfZeroOrNull().ToString();
            IzmTMCell.Text = IzmTM.ReturnEmptyIfZeroOrNull().ToString();
            IzmCMCell.Text = IzmCM.ReturnEmptyIfZeroOrNull().ToString();
            IzmTMCMTopCell.Text = (IzmTM + IzmCM).ReturnEmptyIfZeroOrNull().ToString();
            MerTMCell.Text = MerTM.ReturnEmptyIfZeroOrNull().ToString();
            MerCMCell.Text = MerCM.ReturnEmptyIfZeroOrNull().ToString();
            MerTMCMTopCell.Text = (MerTM + MerCM).ReturnEmptyIfZeroOrNull().ToString();
            ErzTMCell.Text = ErzTM.ReturnEmptyIfZeroOrNull().ToString();
            ErzCMCell.Text = ErzCM.ReturnEmptyIfZeroOrNull().ToString();
            ErzTMCMTopCell.Text = (ErzTM + ErzCM).ReturnEmptyIfZeroOrNull().ToString();
            int TopTM = AnkTM + IstTM + IzmTM + MerTM + ErzTM;
            TopTMCell.Text = TopTM.ReturnEmptyIfZeroOrNull().ToString();
            int TopCM = AnkCM + IstCM + IzmCM + MerCM + ErzCM;
            TopCMCell.Text = TopCM.ReturnEmptyIfZeroOrNull().ToString();
            TopTMCMTopCell.Text = (TopTM + TopCM).ReturnEmptyIfZeroOrNull().ToString();
            string kiraDurumuStr = string.Empty;
            int AnkApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_APT, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int AnkIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_ISHANI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int AnkMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_MESKEN, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int AnkIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_ISYERI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int AnkArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_ARSA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int AnkTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_TARLA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            //int AnkMev = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_MEV, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);

            AnkAptCell.Text = (AnkApt + AnkIshani).ReturnEmptyIfZeroOrNull().ToString();
            AnkMesCell.Text = AnkMes.ReturnEmptyIfZeroOrNull().ToString();
            AnkIsyCell.Text = AnkIsy.ReturnEmptyIfZeroOrNull().ToString();
            AnkArsCell.Text = AnkArs.ReturnEmptyIfZeroOrNull().ToString();
            AnkTarCell.Text = AnkTar.ReturnEmptyIfZeroOrNull().ToString();
            //AnkMevCell.Text = AnkMev.ReturnEmptyIfZeroOrNull().ToString();

            int IstApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_APT, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_ISHANI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_MESKEN, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_ISYERI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_ARSA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_TARLA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            //int IstMev = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_MEV, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);

            IstAptCell.Text = (IstApt + IstIshani).ReturnEmptyIfZeroOrNull().ToString();
            IstMesCell.Text = (IstMes).ReturnEmptyIfZeroOrNull().ToString();
            IstIsyCell.Text = (IstIsy).ReturnEmptyIfZeroOrNull().ToString();
            IstArsCell.Text = (IstArs).ReturnEmptyIfZeroOrNull().ToString();
            IstTarCell.Text = (IstTar).ReturnEmptyIfZeroOrNull().ToString();
            //IstMevCell.Text = (IstMev).ReturnEmptyIfZeroOrNull().ToString();

            int IzmApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_APT, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_ISHANI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_MESKEN, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_ISYERI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_ARSA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_TARLA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            //int IzmMev = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_MEV, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);

            IzmAptCell.Text = (IzmApt + IzmIshani).ReturnEmptyIfZeroOrNull().ToString();
            IzmMesCell.Text = (IzmMes).ReturnEmptyIfZeroOrNull().ToString();
            IzmIsyCell.Text = (IzmIsy).ReturnEmptyIfZeroOrNull().ToString();
            IzmArsCell.Text = (IzmArs).ReturnEmptyIfZeroOrNull().ToString();
            IzmTarCell.Text = (IzmTar).ReturnEmptyIfZeroOrNull().ToString();
            //IzmMevCell.Text = (IzmMev).ReturnEmptyIfZeroOrNull().ToString();

            int MerApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_APT, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_ISHANI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_MESKEN, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_ISYERI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_ARSA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_TARLA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            //int MerMev = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_MEV, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);

            MerAptCell.Text = (MerApt + MerIshani).ReturnEmptyIfZeroOrNull().ToString();
            MerMesCell.Text = (MerMes).ReturnEmptyIfZeroOrNull().ToString();
            MerIsyCell.Text = (MerIsy).ReturnEmptyIfZeroOrNull().ToString();
            MerArsCell.Text = (MerArs).ReturnEmptyIfZeroOrNull().ToString();
            MerTarCell.Text = (MerTar).ReturnEmptyIfZeroOrNull().ToString();
            //MerMevCell.Text = (MerMev).ReturnEmptyIfZeroOrNull().ToString();

            int ErzApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_APT, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int ErzIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_ISHANI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int ErzMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_MESKEN, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int ErzIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_ISYERI, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int ErzArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_ARSA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int ErzTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_TARLA, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);
            //int MerMev = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKiraDurumu(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_MEV, kiraDurumuStr, ProjeConstants.MULKIYETSEKLI_HEPSI);

            ErzAptCell.Text = (ErzApt + ErzIshani).ReturnEmptyIfZeroOrNull().ToString();
            ErzMesCell.Text = (ErzMes).ReturnEmptyIfZeroOrNull().ToString();
            ErzIsyCell.Text = (ErzIsy).ReturnEmptyIfZeroOrNull().ToString();
            ErzArsCell.Text = (ErzArs).ReturnEmptyIfZeroOrNull().ToString();
            ErzTarCell.Text = (ErzTar).ReturnEmptyIfZeroOrNull().ToString();

            TopAptCell.Text = (AnkApt + IstApt + IzmApt + MerApt + ErzApt + AnkIshani + IstIshani + IzmIshani + MerIshani+ErzIshani).ReturnEmptyIfZeroOrNull().ToString();
            TopMesCell.Text = (AnkMes + IstMes + IzmMes + MerMes + ErzMes).ReturnEmptyIfZeroOrNull().ToString();
            TopIsyCell.Text = (AnkIsy + IstIsy + IzmIsy + MerIsy +ErzIsy).ReturnEmptyIfZeroOrNull().ToString();
            TopArsCell.Text = (AnkArs + IstArs + IzmArs + MerArs +ErzArs).ReturnEmptyIfZeroOrNull().ToString();
            TopTarCell.Text = (AnkTar + IstTar + IzmTar + MerTar +ErzTar).ReturnEmptyIfZeroOrNull().ToString();
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
