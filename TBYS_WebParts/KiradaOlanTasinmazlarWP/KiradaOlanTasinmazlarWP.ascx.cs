using Model.Ortak;
using Model.Services.TBYS;
using Model.TBYS;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
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
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            TabloyuDoldur(ProjeConstants.KIRADURUMU_KIRAYAUYGUN);
            TahminiRayicTopTxt.Value = TahminiRayicToplaminiBul().ToString();
            EmlakBeyanTopTxt.Value = EmlakBeyanToplaminiBul().ToString();
        }
        private decimal TahminiRayicToplaminiBul()
        {
            decimal toplam = 0;
            TasinmazService tasinmazService = new TasinmazService();
            toplam = tasinmazService.SelectTahminiRayicToplamiByKirayaUygunluk(ProjeConstants.KIRADURUMU_KIRAYAUYGUN);
            return toplam;
        }
        private decimal EmlakBeyanToplaminiBul()
        {
            decimal toplam = 0;
            TasinmazService tasinmazService = new TasinmazService();
            toplam = tasinmazService.SelectEmlakBeyanToplamiByKirayaUygunluk(ProjeConstants.KIRADURUMU_KIRAYAUYGUN);
            return toplam;
        }
        private void TabloyuDoldur(string kirayaUygunluk)
        {
            TasinmazService tasinmazService = new TasinmazService();
            int AnkApt = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_APT, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstApt = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_APT, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmApt = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_APT, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerApt = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_APT, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int ErzApt = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_APT, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int TopApt = AnkApt + IstApt + IzmApt + MerApt + ErzApt;

            int AnkIshani = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_ISHANI, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstIshani = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_ISHANI, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmIshani = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_ISHANI, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerIshani = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_ISHANI, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int ErzIshani = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_ISHANI, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int TopIshani = AnkIshani + IstIshani + IzmIshani + MerIshani + ErzIshani;

            int AnkMes = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_MESKEN, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstMes = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_MESKEN, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmMes = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_MESKEN, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerMes = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_MESKEN, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int ErzMes = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_MESKEN, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int TopMes = AnkMes + IstMes + IzmMes + MerMes + ErzMes;

            int AnkIsy = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_ISYERI, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstIsy = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_ISYERI, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmIsy = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_ISYERI, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerIsy = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_ISYERI, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int ErzIsy = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_ISYERI, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int TopIsy = AnkIsy + IstIsy + IzmIsy + MerIsy + ErzIsy;

            int AnkArs = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_ARSA, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstArs = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_ARSA, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmArs = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_ARSA, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerArs = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_ARSA, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int ErzArs = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_ARSA, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int TopArs = AnkArs + IstArs + IzmArs + MerArs + ErzArs;

            int AnkTar = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_TARLA, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstTar = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_TARLA, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmTar = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_TARLA, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerTar = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_TARLA, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int ErzTar = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_TARLA, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int TopTar = AnkTar + IstTar + IzmTar + MerTar + ErzTar;

            int AnkMev = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.KULLANIMSEKLI_MEV, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IstMev = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.KULLANIMSEKLI_MEV, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int IzmMev = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.KULLANIMSEKLI_MEV, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int MerMev = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.KULLANIMSEKLI_MEV, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int ErzMev = tasinmazService.SelectTasinmazAdetByBolgeCinsiKirayaUygunluk(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.KULLANIMSEKLI_MEV, kirayaUygunluk, ProjeConstants.MULKIYETSEKLI_HEPSI);
            int TopMev = AnkMev + IstMev + IzmMev + MerMev + ErzMev;

            int AnkTop = AnkIshani + AnkApt + AnkMes + AnkIsy + AnkArs + AnkTar + AnkMev;
            int IstTop = IstIshani + IstApt + IstMes + IstIsy + IstArs + IstTar + IstMev;
            int IzmTop = IzmIshani + IzmApt + IzmMes + IzmIsy + IzmArs + IzmTar + IzmMev;
            int MerTop = MerIshani + MerApt + MerMes + MerIsy + MerArs + MerTar + MerMev;
            int ErzTop = ErzIshani + ErzApt + ErzMes + ErzIsy + ErzArs + ErzTar + ErzMev;

            TableRow row1 = new TableRow();
            row1.HorizontalAlign = HorizontalAlign.Center;

            TableCell KvAptCell = new TableCell();
            KvAptCell.Text = "Bina";
            row1.Controls.Add(KvAptCell);
            //Ank
            TableCell KvAptAnkCell = new TableCell();
            KvAptAnkCell.Text = (AnkApt + AnkIshani).ToString();
            row1.Controls.Add(KvAptAnkCell);
            //Ist
            TableCell KvAptIstCell = new TableCell();
            KvAptIstCell.Text = (IstApt + IstIshani).ToString();
            row1.Controls.Add(KvAptIstCell);
            //Izm
            TableCell KvAptIzmCell = new TableCell();
            KvAptIzmCell.Text = (IzmApt + IzmIshani).ToString();
            row1.Controls.Add(KvAptIzmCell);
            //Mer
            TableCell KvAptMerCell = new TableCell();
            KvAptMerCell.Text = (MerApt + MerIshani).ToString();
            row1.Controls.Add(KvAptMerCell);
            //Erz
            TableCell KvAptErzCell = new TableCell();
            KvAptErzCell.Text = (ErzApt + ErzIshani).ToString();
            row1.Controls.Add(KvAptErzCell);
            //Top
            TableCell KvAptTopCell = new TableCell();
            KvAptTopCell.Text = (TopApt + TopIshani).ToString();
            row1.Controls.Add(KvAptTopCell);

            KiradaOlanTasinmazlarTable.Controls.Add(row1);
            //birinci satir toplam bitti

            //ikinci Satir
            TableRow row2 = new TableRow();
            row2.HorizontalAlign = HorizontalAlign.Center;

            TableCell KvMesCell = new TableCell();
            KvMesCell.Text = "Mesken";
            row2.Controls.Add(KvMesCell);

            TableCell KvMesAnkCell = new TableCell();
            KvMesAnkCell.Text = (AnkMes).ToString();
            row2.Controls.Add(KvMesAnkCell);

            TableCell KvMesIstCell = new TableCell();
            KvMesIstCell.Text = (IstMes).ToString();
            row2.Controls.Add(KvMesIstCell);

            TableCell KvMesIzmCell = new TableCell();
            KvMesIzmCell.Text = (IzmMes).ToString();
            row2.Controls.Add(KvMesIzmCell);

            TableCell KvMesMerCell = new TableCell();
            KvMesMerCell.Text = (MerMes).ToString();
            row2.Controls.Add(KvMesMerCell);

            TableCell KvMesErzCell = new TableCell();
            KvMesErzCell.Text = (ErzMes).ToString();
            row2.Controls.Add(KvMesErzCell);

            //Top
            TableCell KvMesTopCell = new TableCell();
            KvMesTopCell.Text = (TopMes).ToString();
            row2.Controls.Add(KvMesTopCell);

            KiradaOlanTasinmazlarTable.Controls.Add(row2);
            //Mesken satir bitti

            //M.Ev
            TableRow row3 = new TableRow();
            row3.HorizontalAlign = HorizontalAlign.Center;

            TableCell KvMevCell = new TableCell();
            KvMevCell.Text = "Müstakil Ev";
            row3.Controls.Add(KvMevCell);

            TableCell KvMevAnkCell = new TableCell();
            KvMevAnkCell.Text = (AnkMev).ToString();
            row3.Controls.Add(KvMevAnkCell);

            TableCell KvMevIstCell = new TableCell();
            KvMevIstCell.Text = (IstMev).ToString();
            row3.Controls.Add(KvMevIstCell);

            TableCell KvMevIzmCell = new TableCell();
            KvMevIzmCell.Text = (IzmMev).ToString();
            row3.Controls.Add(KvMevIzmCell);

            TableCell KvMevMerCell = new TableCell();
            KvMevMerCell.Text = (MerMev).ToString();
            row3.Controls.Add(KvMevMerCell);

            TableCell KvMevErzCell = new TableCell();
            KvMevErzCell.Text = (ErzMev).ToString();
            row3.Controls.Add(KvMevErzCell);

            //Top
            TableCell KvMevTopCell = new TableCell();
            KvMevTopCell.Text = (TopMev).ToString();
            row3.Controls.Add(KvMevTopCell);

            KiradaOlanTasinmazlarTable.Controls.Add(row3);
            //Mev bitti

            //Isyeri Satir
            TableRow row4 = new TableRow();
            row4.HorizontalAlign = HorizontalAlign.Center;

            TableCell KvIsyCell = new TableCell();
            KvIsyCell.Text = "Isyeri";
            row4.Controls.Add(KvIsyCell);

            TableCell KvIsyAnkCell = new TableCell();
            KvIsyAnkCell.Text = (AnkIsy).ToString();
            row4.Controls.Add(KvIsyAnkCell);

            TableCell KvIsyIstCell = new TableCell();
            KvIsyIstCell.Text = (IstIsy).ToString();
            row4.Controls.Add(KvIsyIstCell);

            TableCell KvIsyIzmCell = new TableCell();
            KvIsyIzmCell.Text = (IzmIsy).ToString();
            row4.Controls.Add(KvIsyIzmCell);

            TableCell KvIsyMerCell = new TableCell();
            KvIsyMerCell.Text = (MerIsy).ToString();
            row4.Controls.Add(KvIsyMerCell);

            TableCell KvIsyErzCell = new TableCell();
            KvIsyErzCell.Text = (ErzIsy).ToString();
            row4.Controls.Add(KvIsyErzCell);

            //Top
            TableCell KvIsyTopCell = new TableCell();
            KvIsyTopCell.Text = (TopIsy).ToString();
            row4.Controls.Add(KvIsyTopCell);

            KiradaOlanTasinmazlarTable.Controls.Add(row4);
            //Isyeri satir bitti

            //Arsa satir
            TableRow row5 = new TableRow();
            row5.HorizontalAlign = HorizontalAlign.Center;

            TableCell KvArsCell = new TableCell();
            KvArsCell.Text = "Arsa";
            row5.Controls.Add(KvArsCell);

            TableCell KvArsAnkCell = new TableCell();
            KvArsAnkCell.Text = (AnkArs).ToString();
            row5.Controls.Add(KvArsAnkCell);

            TableCell KvArsIstCell = new TableCell();
            KvArsIstCell.Text = (IstArs).ToString();
            row5.Controls.Add(KvArsIstCell);

            TableCell KvArsIzmCell = new TableCell();
            KvArsIzmCell.Text = (IzmArs).ToString();
            row5.Controls.Add(KvArsIzmCell);

            TableCell KvArsMerCell = new TableCell();
            KvArsMerCell.Text = (MerArs).ToString();
            row5.Controls.Add(KvArsMerCell);

            TableCell KvArsErzCell = new TableCell();
            KvArsErzCell.Text = (ErzArs).ToString();
            row5.Controls.Add(KvArsErzCell);

            //Top
            TableCell KvArsTopCell = new TableCell();
            KvArsTopCell.Text = (TopArs).ToString();
            row5.Controls.Add(KvArsTopCell);

            KiradaOlanTasinmazlarTable.Controls.Add(row5);
            //Arsa satir bitti

            //Tarla satir
            TableRow row7 = new TableRow();
            row7.HorizontalAlign = HorizontalAlign.Center;

            TableCell KvTarCell = new TableCell();
            KvTarCell.Text = "Tarla";
            row7.Controls.Add(KvTarCell);

            TableCell KvTarAnkCell = new TableCell();
            KvTarAnkCell.Text = (AnkTar).ToString();
            row7.Controls.Add(KvTarAnkCell);

            TableCell KvTarIstCell = new TableCell();
            KvTarIstCell.Text = (IstTar).ToString();
            row7.Controls.Add(KvTarIstCell);

            TableCell KvTarIzmCell = new TableCell();
            KvTarIzmCell.Text = (IzmTar).ToString();
            row7.Controls.Add(KvTarIzmCell);

            TableCell KvTarMerCell = new TableCell();
            KvTarMerCell.Text = (MerTar).ToString();
            row7.Controls.Add(KvTarMerCell);

            TableCell KvTarErzCell = new TableCell();
            KvTarErzCell.Text = (ErzTar).ToString();
            row7.Controls.Add(KvTarErzCell);

            //Top
            TableCell KvTarTopCell = new TableCell();
            KvTarTopCell.Text = (TopTar).ToString();
            row7.Controls.Add(KvTarTopCell);

            KiradaOlanTasinmazlarTable.Controls.Add(row7);
            //Tarla satir bitti



            //toplam satir
            TableRow row8 = new TableRow();
            row8.HorizontalAlign = HorizontalAlign.Center;

            TableCell KvTopCell = new TableCell();
            KvTopCell.Text = "Toplam";
            KvTopCell.BorderWidth = 2;
            row8.Controls.Add(KvTopCell);

            TableCell KvTopAnkCell = new TableCell();
            KvTopAnkCell.Text = (AnkTop).ToString();
            KvTopAnkCell.BorderWidth = 2;
            row8.Controls.Add(KvTopAnkCell);

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

            TableCell KvTopErzCell = new TableCell();
            KvTopErzCell.Text = (ErzTop).ToString();
            KvTopErzCell.BorderWidth = 2;
            row8.Controls.Add(KvTopErzCell);

            TableCell KvTopTopCell = new TableCell();
            KvTopTopCell.Text = (AnkTop + IstTop + IzmTop + MerTop + ErzTop).ToString();
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
