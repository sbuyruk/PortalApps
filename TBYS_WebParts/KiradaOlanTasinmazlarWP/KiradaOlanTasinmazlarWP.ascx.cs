using Model.TBYS;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.ProjeGlobal;


namespace TBYS_WebParts.KiradaOlanTasinmazlarWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiradaOlanTasinmazlarWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiradaOlanTasinmazlarWP()
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
            TabloyuDoldur(ProjeConstants.KULLANIMDURUMU_KIRADA);
            TahminiRayicTopTxt.Value = TahminiRayicToplaminiBul().ToString();
            EmlakBeyanTopTxt.Value = EmlakBeyanToplaminiBul().ToString();
        }
        private decimal TahminiRayicToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectTahminiRayicToplamiByKullanimDurumu(ProjeConstants.KULLANIMDURUMU_KIRADA, true);
            return toplam;
        }
        private decimal EmlakBeyanToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectEmlakBeyanToplamiByKullanimDurumu(ProjeConstants.KULLANIMDURUMU_KIRADA, true);
            return toplam;
        }
        private void TabloyuDoldur(string kDurumu)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            int GMApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_APT, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_APT, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_APT, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerApt = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_APT, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int TopApt = GMApt + IstApt + IzmApt + MerApt;

            int GMIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_ISHANI, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_ISHANI, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_ISHANI, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerIshani = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_ISHANI, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int TopIshani = GMIshani + IstIshani + IzmIshani + MerIshani;

            int GMMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_MESKEN, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_MESKEN, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_MESKEN, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerMes = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_MESKEN, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int TopMes = GMMes + IstMes + IzmMes + MerMes;

            int GMIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_ISYERI, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_ISYERI, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_ISYERI, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerIsy = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_ISYERI, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int TopIsy = GMIsy + IstIsy + IzmIsy + MerIsy;

            int GMArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_ARSA, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_ARSA, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_ARSA, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerArs = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_ARSA, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int TopArs = GMArs + IstArs + IzmArs + MerArs;

            int GMTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_TARLA, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_TARLA, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_TARLA, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerTar = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_TARLA, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int TopTar = GMTar + IstTar + IzmTar + MerTar;

            int GMMev = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_MEV, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstMev = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_MEV, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmMev = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_MEV, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerMev = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_MEV, kDurumu, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int TopMev = GMMev + IstMev + IzmMev + MerMev;

            int GMTop = GMIshani + GMApt + GMMes + GMIsy + GMArs + GMTar + GMMev;
            int IstTop = IstIshani + IstApt + IstMes + IstIsy + IstArs + IstTar + IstMev;
            int IzmTop = IzmIshani + IzmApt + IzmMes + IzmIsy + IzmArs + IzmTar + IzmMev;
            int MerTop = MerIshani + MerApt + MerMes + MerIsy + MerArs + MerTar + MerMev;

            TableRow row1 = new TableRow();
            row1.HorizontalAlign = HorizontalAlign.Center;

            TableCell KvAptCell = new TableCell();
            KvAptCell.Text = "Apt./İşhanı";
            row1.Controls.Add(KvAptCell);
            //GM
            TableCell KvAptGMCell = new TableCell();
            KvAptGMCell.Text = (GMApt + GMIshani).ToString();
            row1.Controls.Add(KvAptGMCell);
            //Ist
            TableCell KvAptIstCell = new TableCell();
            KvAptIstCell.Text = (IstApt + IstIshani).ToString();
            row1.Controls.Add(KvAptIstCell);
            //İzm
            TableCell KvAptIzmCell = new TableCell();
            KvAptIzmCell.Text = (IzmApt + IzmIshani).ToString();
            row1.Controls.Add(KvAptIzmCell);
            //Mer
            TableCell KvAptMerCell = new TableCell();
            KvAptMerCell.Text = (MerApt + MerIshani).ToString();
            row1.Controls.Add(KvAptMerCell);
            //Top
            TableCell KvAptTopCell = new TableCell();
            KvAptTopCell.Text = (TopApt + TopIshani).ToString();
            row1.Controls.Add(KvAptTopCell);

            KiradaOlanTasinmazlarTable.Controls.Add(row1);
            //birinci satır toplam bitti

            //ikinci Satır
            TableRow row2 = new TableRow();
            row2.HorizontalAlign = HorizontalAlign.Center;

            TableCell KvMesCell = new TableCell();
            KvMesCell.Text = "Mesken";
            row2.Controls.Add(KvMesCell);

            TableCell KvMesGMCell = new TableCell();
            KvMesGMCell.Text = (GMMes).ToString();
            row2.Controls.Add(KvMesGMCell);

            TableCell KvMesIstCell = new TableCell();
            KvMesIstCell.Text = (IstMes).ToString();
            row2.Controls.Add(KvMesIstCell);

            TableCell KvMesIzmCell = new TableCell();
            KvMesIzmCell.Text = (IzmMes).ToString();
            row2.Controls.Add(KvMesIzmCell);

            TableCell KvMesMerCell = new TableCell();
            KvMesMerCell.Text = (MerMes).ToString();
            row2.Controls.Add(KvMesMerCell);

            //Top
            TableCell KvMesTopCell = new TableCell();
            KvMesTopCell.Text = (TopMes).ToString();
            row2.Controls.Add(KvMesTopCell);

            KiradaOlanTasinmazlarTable.Controls.Add(row2);
            //Mesken satır bitti

            //M.Ev
            TableRow row3 = new TableRow();
            row3.HorizontalAlign = HorizontalAlign.Center;

            TableCell KvMevCell = new TableCell();
            KvMevCell.Text = "Müstakil Ev";
            row3.Controls.Add(KvMevCell);

            TableCell KvMevGMCell = new TableCell();
            KvMevGMCell.Text = (GMMev).ToString();
            row3.Controls.Add(KvMevGMCell);

            TableCell KvMevIstCell = new TableCell();
            KvMevIstCell.Text = (IstMev).ToString();
            row3.Controls.Add(KvMevIstCell);

            TableCell KvMevIzmCell = new TableCell();
            KvMevIzmCell.Text = (IzmMev).ToString();
            row3.Controls.Add(KvMevIzmCell);

            TableCell KvMevMerCell = new TableCell();
            KvMevMerCell.Text = (MerMev).ToString();
            row3.Controls.Add(KvMevMerCell);

            //Top
            TableCell KvMevTopCell = new TableCell();
            KvMevTopCell.Text = (TopMev).ToString();
            row3.Controls.Add(KvMevTopCell);

            KiradaOlanTasinmazlarTable.Controls.Add(row3);
            //Mev bitti

            //İsyeri Satır
            TableRow row4 = new TableRow();
            row4.HorizontalAlign = HorizontalAlign.Center;

            TableCell KvIsyCell = new TableCell();
            KvIsyCell.Text = "İşyeri";
            row4.Controls.Add(KvIsyCell);

            TableCell KvIsyGMCell = new TableCell();
            KvIsyGMCell.Text = (GMIsy).ToString();
            row4.Controls.Add(KvIsyGMCell);

            TableCell KvIsyIstCell = new TableCell();
            KvIsyIstCell.Text = (IstIsy).ToString();
            row4.Controls.Add(KvIsyIstCell);

            TableCell KvIsyIzmCell = new TableCell();
            KvIsyIzmCell.Text = (IzmIsy).ToString();
            row4.Controls.Add(KvIsyIzmCell);

            TableCell KvIsyMerCell = new TableCell();
            KvIsyMerCell.Text = (MerIsy).ToString();
            row4.Controls.Add(KvIsyMerCell);

            //Top
            TableCell KvIsyTopCell = new TableCell();
            KvIsyTopCell.Text = (TopIsy).ToString();
            row4.Controls.Add(KvIsyTopCell);

            KiradaOlanTasinmazlarTable.Controls.Add(row4);
            //İsyeri satır bitti

            //Arsa satır
            TableRow row5 = new TableRow();
            row5.HorizontalAlign = HorizontalAlign.Center;

            TableCell KvArsCell = new TableCell();
            KvArsCell.Text = "Arsa";
            row5.Controls.Add(KvArsCell);

            TableCell KvArsGMCell = new TableCell();
            KvArsGMCell.Text = (GMArs).ToString();
            row5.Controls.Add(KvArsGMCell);

            TableCell KvArsIstCell = new TableCell();
            KvArsIstCell.Text = (IstArs).ToString();
            row5.Controls.Add(KvArsIstCell);

            TableCell KvArsIzmCell = new TableCell();
            KvArsIzmCell.Text = (IzmArs).ToString();
            row5.Controls.Add(KvArsIzmCell);

            TableCell KvArsMerCell = new TableCell();
            KvArsMerCell.Text = (MerArs).ToString();
            row5.Controls.Add(KvArsMerCell);

            //Top
            TableCell KvArsTopCell = new TableCell();
            KvArsTopCell.Text = (TopArs).ToString();
            row5.Controls.Add(KvArsTopCell);

            KiradaOlanTasinmazlarTable.Controls.Add(row5);
            //Arsa satır bitti

            //Tarla satır
            TableRow row7 = new TableRow();
            row7.HorizontalAlign = HorizontalAlign.Center;

            TableCell KvTarCell = new TableCell();
            KvTarCell.Text = "Tarla";
            row7.Controls.Add(KvTarCell);

            TableCell KvTarGMCell = new TableCell();
            KvTarGMCell.Text = (GMTar).ToString();
            row7.Controls.Add(KvTarGMCell);

            TableCell KvTarIstCell = new TableCell();
            KvTarIstCell.Text = (IstTar).ToString();
            row7.Controls.Add(KvTarIstCell);

            TableCell KvTarIzmCell = new TableCell();
            KvTarIzmCell.Text = (IzmTar).ToString();
            row7.Controls.Add(KvTarIzmCell);

            TableCell KvTarMerCell = new TableCell();
            KvTarMerCell.Text = (MerTar).ToString();
            row7.Controls.Add(KvTarMerCell);

            //Top
            TableCell KvTarTopCell = new TableCell();
            KvTarTopCell.Text = (TopTar).ToString();
            row7.Controls.Add(KvTarTopCell);

            KiradaOlanTasinmazlarTable.Controls.Add(row7);
            //Tarla satır bitti



            //toplam satır
            TableRow row8 = new TableRow();
            row8.HorizontalAlign = HorizontalAlign.Center;

            TableCell KvTopCell = new TableCell();
            KvTopCell.Text = "Toplam";
            KvTopCell.BorderWidth = 2;
            row8.Controls.Add(KvTopCell);

            TableCell KvTopGMCell = new TableCell();
            KvTopGMCell.Text = (GMTop).ToString();
            KvTopGMCell.BorderWidth = 2;
            row8.Controls.Add(KvTopGMCell);

            TableCell KvTopIstCell = new TableCell();
            KvTopIstCell.Text = (IstTop).ToString();
            KvTopIstCell.BorderWidth = 2;
            row8.Controls.Add(KvTopIstCell);

            TableCell KvTopIzmCell = new TableCell();
            KvTopIzmCell.Text = (IzmTop).ToString();
            KvTopIzmCell.BorderWidth = 2;
            row8.Controls.Add(KvTopIzmCell);

            TableCell KvTopMerCell = new TableCell();
            KvTopMerCell.Text = (MerTop).ToString();
            KvTopMerCell.BorderWidth = 2;
            row8.Controls.Add(KvTopMerCell);

            TableCell KvTopTopCell = new TableCell();
            KvTopTopCell.Text = (GMTop + IstTop + IzmTop + MerTop).ToString();
            KvTopTopCell.BorderWidth = 2;
            row8.Controls.Add(KvTopTopCell);

            KiradaOlanTasinmazlarTable.Controls.Add(row8);
            //toplam
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            string filename = "KiradaOlanTasinmazlar" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            KiradaOlanTasinmazlarTable.RenderControl(hw);

            Page.Response.ContentType = "application/vnd.ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            Page.Response.Write(tw.ToString());
            Page.Response.End();
        }
    }
}
