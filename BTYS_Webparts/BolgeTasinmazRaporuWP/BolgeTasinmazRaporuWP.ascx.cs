using Model.IKYS;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace BTYS_Webparts.BolgeTasinmazRaporuWP
{
    [ToolboxItemAttribute(false)]
    public partial class BolgeTasinmazRaporuWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BolgeTasinmazRaporuWP()
        {
        }
        private string BolgeQS
        {
            get
            {

                if (ViewState["Bolge"] == null)
                {
                    if (Page.Request.QueryString["Bolge"] != null)
                    {
                        ViewState["Bolge"] = Page.Request.QueryString["Bolge"];
                    }
                    else
                    {
                        ViewState["Bolge"] = string.Empty;
                    }
                }
                return ViewState["Bolge"].ToString();
            }

            set
            {
                ViewState["Bolge"] = value;
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
            BolgeQS = IKYSOrtak.PersonelinBolgesiniGetir_Deprecated(CurrentUserName);
            if (!string.IsNullOrEmpty(BolgeQS))
            {
                TablolariDoldur(BolgeQS);
                TahminiRayicTopTxt.Value = TahminiRayicToplaminiBul().ToString();
                EmlakBeyanTopTxt.Value = EmlakBeyanToplaminiBul().ToString();
            }
            else
                MessageHelper.PublishMessage("Bölgeniz Belirlenemedi", ProjeConstants.MESAJ_HATA);
        }
        private decimal TahminiRayicToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectTahminiRayicToplami(BolgeQS);
            return toplam;
        }
        private decimal EmlakBeyanToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectEmlakBeyanDegeriToplami(BolgeQS);
            return toplam;
        }
        private void TablolariDoldur(string bolge)
        {
            BolgeHeaderCell.Text = BolgeQS.ToUpper() + " BOLGE TEMSİLCİLİĞİ";
            Tasinmaz tasinmaz = new Tasinmaz();
            Il il = new Il();
            List<Il> ilList = il.SelectByBolge(bolge);
            int AptTMToplam = 0;
            int AptCMToplam = 0;
            int MeskenTMToplam = 0;
            int MeskenCMToplam = 0;
            int MevTMToplam = 0;
            int MevCMToplam = 0;
            int IsyeriTMToplam = 0;
            int IsyeriCMToplam = 0;
            int ArsaTMToplam = 0;
            int ArsaCMToplam = 0;
            int TarlaTMToplam = 0;
            int TarlaCMToplam = 0;
            int TMaltToplam = 0;
            int CMaltToplam = 0;
            int TMCMaltToplam = 0;
            if (ilList.Count > 0)
            {
                int SiraNo = 1;
                foreach (Il nIl in ilList)
                {
                    TableRow row = new TableRow();
                    row.HorizontalAlign = HorizontalAlign.Center;
                    

                    int AptTMadet = tasinmaz.SelectTasinmazAdetByIliMulkiyetSekliKullanimSekli(nIl.IlAdi, ProjeConstants.MULKIYETSEKLI_TM, ProjeConstants.KULLANIMSEKLI_APT);
                    int IshaniTMadet = tasinmaz.SelectTasinmazAdetByIliMulkiyetSekliKullanimSekli(nIl.IlAdi, ProjeConstants.MULKIYETSEKLI_TM, ProjeConstants.KULLANIMSEKLI_ISHANI);
                    AptTMadet += IshaniTMadet;
                    AptTMToplam += AptTMadet;
                    int AptCMadet = tasinmaz.SelectTasinmazAdetByIliMulkiyetSekliKullanimSekli(nIl.IlAdi, ProjeConstants.MULKIYETSEKLI_CM, ProjeConstants.KULLANIMSEKLI_APT);
                    int IshaniCMadet = tasinmaz.SelectTasinmazAdetByIliMulkiyetSekliKullanimSekli(nIl.IlAdi, ProjeConstants.MULKIYETSEKLI_CM, ProjeConstants.KULLANIMSEKLI_ISHANI);
                    AptCMadet += IshaniCMadet;
                    AptCMToplam += AptCMadet;
                    int MeskenTMadet = tasinmaz.SelectTasinmazAdetByIliMulkiyetSekliKullanimSekli(nIl.IlAdi, ProjeConstants.MULKIYETSEKLI_TM, ProjeConstants.KULLANIMSEKLI_MESKEN);
                    MeskenTMToplam += MeskenTMadet;
                    int MeskenCMadet = tasinmaz.SelectTasinmazAdetByIliMulkiyetSekliKullanimSekli(nIl.IlAdi, ProjeConstants.MULKIYETSEKLI_CM, ProjeConstants.KULLANIMSEKLI_MESKEN);
                    MeskenCMToplam += MeskenCMadet;
                    int MevTMadet = tasinmaz.SelectTasinmazAdetByIliMulkiyetSekliKullanimSekli(nIl.IlAdi, ProjeConstants.MULKIYETSEKLI_TM, ProjeConstants.KULLANIMSEKLI_MEV);
                    MevTMToplam += MevTMadet;
                    int MevCMadet = tasinmaz.SelectTasinmazAdetByIliMulkiyetSekliKullanimSekli(nIl.IlAdi, ProjeConstants.MULKIYETSEKLI_CM, ProjeConstants.KULLANIMSEKLI_MEV);
                    MevCMToplam += MevCMadet;
                    int IsyeriTMadet = tasinmaz.SelectTasinmazAdetByIliMulkiyetSekliKullanimSekli(nIl.IlAdi, ProjeConstants.MULKIYETSEKLI_TM, ProjeConstants.KULLANIMSEKLI_ISYERI);
                    IsyeriTMToplam += IsyeriTMadet;
                    int IsyeriCMadet = tasinmaz.SelectTasinmazAdetByIliMulkiyetSekliKullanimSekli(nIl.IlAdi, ProjeConstants.MULKIYETSEKLI_CM, ProjeConstants.KULLANIMSEKLI_ISYERI);
                    IsyeriCMToplam += IsyeriCMadet;
                    int ArsaTMadet = tasinmaz.SelectTasinmazAdetByIliMulkiyetSekliKullanimSekli(nIl.IlAdi, ProjeConstants.MULKIYETSEKLI_TM, ProjeConstants.KULLANIMSEKLI_ARSA);
                    ArsaTMToplam += ArsaTMadet;
                    int ArsaCMadet = tasinmaz.SelectTasinmazAdetByIliMulkiyetSekliKullanimSekli(nIl.IlAdi, ProjeConstants.MULKIYETSEKLI_CM, ProjeConstants.KULLANIMSEKLI_ARSA);
                    ArsaCMToplam += ArsaCMadet;
                    int TarlaTMadet = tasinmaz.SelectTasinmazAdetByIliMulkiyetSekliKullanimSekli(nIl.IlAdi, ProjeConstants.MULKIYETSEKLI_TM, ProjeConstants.KULLANIMSEKLI_TARLA);
                    TarlaTMToplam += TarlaTMadet;
                    int TarlaCMadet = tasinmaz.SelectTasinmazAdetByIliMulkiyetSekliKullanimSekli(nIl.IlAdi, ProjeConstants.MULKIYETSEKLI_CM, ProjeConstants.KULLANIMSEKLI_TARLA);
                    TarlaCMToplam += TarlaCMadet;

                    int TMToplam = AptTMadet + MeskenTMadet + MevTMadet + IsyeriTMadet + ArsaTMadet + TarlaTMadet;
                    TMaltToplam += TMToplam;
                    int CMToplam = AptCMadet + MeskenCMadet + MevCMadet + IsyeriCMadet + ArsaCMadet + TarlaCMadet;
                    CMaltToplam += CMToplam;
                    int TMCMToplam = TMToplam + CMToplam;
                    TMCMaltToplam += TMCMToplam;
                    if (TMCMToplam > 0)
                    {
                        TableCell SiraNoCell = new TableCell();
                        SiraNoCell.Text = SiraNo++ + "";
                        SiraNoCell.BorderStyle = BorderStyle.Solid;
                        row.Controls.Add(SiraNoCell);


                        TableCell IlCell = new TableCell();
                        IlCell.Text = nIl.IlAdi;
                        IlCell.BorderStyle = BorderStyle.Solid;
                        row.Controls.Add(IlCell);

                        TableCell AptTMCell = new TableCell();
                        AptTMCell.Text = AptTMadet.ReturnEmptyIfZeroOrNull().ToString();
                        AptTMCell.BorderStyle = BorderStyle.Solid;
                        row.Controls.Add(AptTMCell);

                        TableCell AptCMCell = new TableCell();
                        AptCMCell.Text = AptCMadet.ReturnEmptyIfZeroOrNull().ToString();
                        AptCMCell.BorderStyle = BorderStyle.Solid;
                        row.Controls.Add(AptCMCell);

                        TableCell MevTMCell = new TableCell();
                        MevTMCell.Text = MevTMadet.ReturnEmptyIfZeroOrNull().ToString();
                        MevTMCell.BorderStyle = BorderStyle.Solid;
                        row.Controls.Add(MevTMCell);

                        TableCell MevCMCell = new TableCell();
                        MevCMCell.Text = MevCMadet.ReturnEmptyIfZeroOrNull().ToString();
                        MevCMCell.BorderStyle = BorderStyle.Solid;
                        row.Controls.Add(MevCMCell);

                        TableCell MeskenTMCell = new TableCell();
                        MeskenTMCell.Text = MeskenTMadet.ReturnEmptyIfZeroOrNull().ToString();
                        MeskenTMCell.BorderStyle = BorderStyle.Solid;
                        row.Controls.Add(MeskenTMCell);

                        TableCell MeskenCMCell = new TableCell();
                        MeskenCMCell.Text = MeskenCMadet.ReturnEmptyIfZeroOrNull().ToString();
                        MeskenCMCell.BorderStyle = BorderStyle.Solid;
                        row.Controls.Add(MeskenCMCell);

                        TableCell IsyeriTMCell = new TableCell();
                        IsyeriTMCell.Text = IsyeriTMadet.ReturnEmptyIfZeroOrNull().ToString();
                        IsyeriTMCell.BorderStyle = BorderStyle.Solid;
                        row.Controls.Add(IsyeriTMCell);

                        TableCell IsyeriCMCell = new TableCell();
                        IsyeriCMCell.Text = IsyeriCMadet.ReturnEmptyIfZeroOrNull().ToString();
                        IsyeriCMCell.BorderStyle = BorderStyle.Solid;
                        row.Controls.Add(IsyeriCMCell);

                        TableCell ArsaTMCell = new TableCell();
                        ArsaTMCell.Text = ArsaTMadet.ReturnEmptyIfZeroOrNull().ToString();
                        ArsaTMCell.BorderStyle = BorderStyle.Solid;
                        row.Controls.Add(ArsaTMCell);

                        TableCell ArsaCMCell = new TableCell();
                        ArsaCMCell.Text = ArsaCMadet.ReturnEmptyIfZeroOrNull().ToString();
                        ArsaCMCell.BorderStyle = BorderStyle.Solid;
                        row.Controls.Add(ArsaCMCell);

                        TableCell TarlaTMCell = new TableCell();
                        TarlaTMCell.Text = TarlaTMadet.ReturnEmptyIfZeroOrNull().ToString();
                        TarlaTMCell.BorderStyle = BorderStyle.Solid;
                        row.Controls.Add(TarlaTMCell);

                        TableCell TarlaCMCell = new TableCell();
                        TarlaCMCell.Text = TarlaCMadet.ReturnEmptyIfZeroOrNull().ToString();
                        TarlaCMCell.BorderStyle = BorderStyle.Solid;
                        row.Controls.Add(TarlaCMCell);

                        TableCell TMCell = new TableCell();
                        TMCell.Text = TMToplam.ReturnEmptyIfZeroOrNull().ToString();
                        TMCell.BorderStyle = BorderStyle.Solid;
                        row.Controls.Add(TMCell);

                        TableCell CMCell = new TableCell();
                        CMCell.Text = CMToplam.ReturnEmptyIfZeroOrNull().ToString();
                        CMCell.BorderStyle = BorderStyle.Solid;
                        row.Controls.Add(CMCell);

                        TableCell ToplamCell = new TableCell();
                        ToplamCell.Text = TMCMToplam.ReturnEmptyIfZeroOrNull().ToString();
                        ToplamCell.BorderStyle = BorderStyle.Solid;
                        row.Controls.Add(ToplamCell);
                        BolgeTable.Controls.Add(row);
                    }

                }
                //footera toplamları yaz
                TableRow footerrow = new TableRow();
                footerrow.HorizontalAlign = HorizontalAlign.Center;

                TableCell LToplamCell = new TableCell();
                LToplamCell.Text = "TOPLAM";
                LToplamCell.ColumnSpan = 2;
                LToplamCell.RowSpan = 2;
                LToplamCell.BorderStyle = BorderStyle.Solid;
                footerrow.Controls.Add(LToplamCell);

                TableCell AptTMToplamCell = new TableCell();
                AptTMToplamCell.Text = AptTMToplam.ReturnEmptyIfZeroOrNull().ToString();
                AptTMToplamCell.BorderStyle = BorderStyle.Solid;
                footerrow.Controls.Add(AptTMToplamCell);

                TableCell AptCMToplamCell = new TableCell();
                AptCMToplamCell.Text = AptCMToplam.ReturnEmptyIfZeroOrNull().ToString();
                AptCMToplamCell.BorderStyle = BorderStyle.Solid;
                footerrow.Controls.Add(AptCMToplamCell);

                TableCell MevTMToplamCell = new TableCell();
                MevTMToplamCell.Text = MevTMToplam.ReturnEmptyIfZeroOrNull().ToString();
                MevTMToplamCell.BorderStyle = BorderStyle.Solid;
                footerrow.Controls.Add(MevTMToplamCell);

                TableCell MevCMToplamCell = new TableCell();
                MevCMToplamCell.Text = MevCMToplam.ReturnEmptyIfZeroOrNull().ToString();
                MevCMToplamCell.BorderStyle = BorderStyle.Solid;
                footerrow.Controls.Add(MevCMToplamCell);

                TableCell MeskenTMToplamCell = new TableCell();
                MeskenTMToplamCell.Text = MeskenTMToplam.ReturnEmptyIfZeroOrNull().ToString();
                MeskenTMToplamCell.BorderStyle = BorderStyle.Solid;
                footerrow.Controls.Add(MeskenTMToplamCell);

                TableCell MeskenCMToplamCell = new TableCell();
                MeskenCMToplamCell.Text = MeskenCMToplam.ReturnEmptyIfZeroOrNull().ToString();
                MeskenCMToplamCell.BorderStyle = BorderStyle.Solid;
                footerrow.Controls.Add(MeskenCMToplamCell);

                TableCell IsyeriTMToplamCell = new TableCell();
                IsyeriTMToplamCell.Text = IsyeriTMToplam.ReturnEmptyIfZeroOrNull().ToString();
                IsyeriTMToplamCell.BorderStyle = BorderStyle.Solid;
                footerrow.Controls.Add(IsyeriTMToplamCell);

                TableCell IsyeriCMToplamCell = new TableCell();
                IsyeriCMToplamCell.Text = IsyeriCMToplam.ReturnEmptyIfZeroOrNull().ToString();
                IsyeriCMToplamCell.BorderStyle = BorderStyle.Solid;
                footerrow.Controls.Add(IsyeriCMToplamCell);

                TableCell ArsaTMToplamCell = new TableCell();
                ArsaTMToplamCell.Text = ArsaTMToplam.ReturnEmptyIfZeroOrNull().ToString();
                ArsaTMToplamCell.BorderStyle = BorderStyle.Solid;
                footerrow.Controls.Add(ArsaTMToplamCell);

                TableCell ArsaCMToplamCell = new TableCell();
                ArsaCMToplamCell.Text = ArsaCMToplam.ReturnEmptyIfZeroOrNull().ToString();
                ArsaCMToplamCell.BorderStyle = BorderStyle.Solid;
                footerrow.Controls.Add(ArsaCMToplamCell);

                TableCell TarlaTMToplamCell = new TableCell();
                TarlaTMToplamCell.Text = TarlaTMToplam.ReturnEmptyIfZeroOrNull().ToString();
                TarlaTMToplamCell.BorderStyle = BorderStyle.Solid;
                footerrow.Controls.Add(TarlaTMToplamCell);

                TableCell TarlaCMToplamCell = new TableCell();
                TarlaCMToplamCell.Text = TarlaCMToplam.ReturnEmptyIfZeroOrNull().ToString();
                TarlaCMToplamCell.BorderStyle = BorderStyle.Solid;
                footerrow.Controls.Add(TarlaCMToplamCell);

                TableCell TMToplamCell = new TableCell();
                TMToplamCell.RowSpan = 2;
                TMToplamCell.VerticalAlign = VerticalAlign.Middle;
                TMToplamCell.Text = TMaltToplam.ReturnEmptyIfZeroOrNull().ToString();
                TMToplamCell.BorderStyle = BorderStyle.Solid;
                footerrow.Controls.Add(TMToplamCell);

                TableCell CMToplamCell = new TableCell();
                CMToplamCell.RowSpan = 2;
                CMToplamCell.VerticalAlign = VerticalAlign.Middle;
                CMToplamCell.Text = CMaltToplam.ReturnEmptyIfZeroOrNull().ToString();
                CMToplamCell.BorderStyle = BorderStyle.Solid;
                footerrow.Controls.Add(CMToplamCell);

                TableCell RToplamCell = new TableCell();
                RToplamCell.Text = TMCMaltToplam.ReturnEmptyIfZeroOrNull().ToString();
                RToplamCell.RowSpan = 2;
                RToplamCell.VerticalAlign = VerticalAlign.Middle;
                RToplamCell.BorderStyle = BorderStyle.Solid;
                footerrow.Controls.Add(RToplamCell);

                //sontoplam
                TableRow SonToplamrow = new TableRow();
                SonToplamrow.HorizontalAlign = HorizontalAlign.Center;
                
                TableCell SonAptToplamCell = new TableCell();
                SonAptToplamCell.Text = (AptTMToplam + AptCMToplam).ReturnEmptyIfZeroOrNull().ToString();
                SonAptToplamCell.ColumnSpan = 2;
                SonAptToplamCell.BorderStyle = BorderStyle.Solid;
                SonToplamrow.Controls.Add(SonAptToplamCell);

                TableCell SonMevToplamCell = new TableCell();
                SonMevToplamCell.Text = (MevTMToplam + MevCMToplam).ReturnEmptyIfZeroOrNull().ToString();
                SonMevToplamCell.BorderStyle = BorderStyle.Solid;
                SonMevToplamCell.ColumnSpan = 2;

                SonToplamrow.Controls.Add(SonMevToplamCell);

                TableCell SonMeskenToplamCell = new TableCell();
                SonMeskenToplamCell.Text = (MeskenTMToplam + MeskenCMToplam).ReturnEmptyIfZeroOrNull().ToString();
                SonMeskenToplamCell.BorderStyle = BorderStyle.Solid;
                SonMeskenToplamCell.ColumnSpan = 2;

                SonToplamrow.Controls.Add(SonMeskenToplamCell);

                TableCell SonIsyeriToplamCell = new TableCell();
                SonIsyeriToplamCell.Text = (IsyeriTMToplam + IsyeriCMToplam).ReturnEmptyIfZeroOrNull().ToString();
                SonIsyeriToplamCell.BorderStyle = BorderStyle.Solid;
                SonIsyeriToplamCell.ColumnSpan = 2;

                SonToplamrow.Controls.Add(SonIsyeriToplamCell);

                TableCell SonArsaToplamCell = new TableCell();
                SonArsaToplamCell.Text = (ArsaTMToplam + ArsaCMToplam).ReturnEmptyIfZeroOrNull().ToString();
                SonArsaToplamCell.BorderStyle = BorderStyle.Solid;
                SonArsaToplamCell.ColumnSpan = 2;

                SonToplamrow.Controls.Add(SonArsaToplamCell);

                TableCell SonTarlaToplamCell = new TableCell();
                SonTarlaToplamCell.Text = (TarlaTMToplam + TarlaCMToplam).ReturnEmptyIfZeroOrNull().ToString();
                SonTarlaToplamCell.BorderStyle = BorderStyle.Solid;
                SonTarlaToplamCell.ColumnSpan = 2;

                SonToplamrow.Controls.Add(SonTarlaToplamCell);
                BolgeTable.Controls.Add(footerrow);
                BolgeTable.Controls.Add(SonToplamrow);
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
            string filename = "TasinmazBolgeRaporu" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            BolgeTable.RenderControl(hw);
            Page.Response.ContentType = "application/vnd.ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            Page.Response.Write(tw.ToString());
            Page.Response.End();
        }
    }
}
