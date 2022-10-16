using Model.TBYS;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.KirayaVerilmeyenTasinmazlarWP
{
    [ToolboxItemAttribute(false)]
    public partial class KirayaVerilmeyenTasinmazlarWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KirayaVerilmeyenTasinmazlarWP()
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
            TabloyuDoldur(ProjeConstants.KULLANIMDURUMU_KDCM,  "Çıplak Mülkiyet Taşınmazlar", 1);
            TabloyuDoldur(ProjeConstants.KULLANIMDURUMU_KDTAAH,  "Taahhüt Verilen Taşınmazlar", 2);
            TabloyuDoldur(ProjeConstants.KULLANIMDURUMU_KDHUKSOR, "Hukuki Sorunlu Taşınmazlar", 3);
            TabloyuDoldur(ProjeConstants.KULLANIMDURUMU_KDKIRAC, "Kıraç Taşınmazlar", 4);
            TabloyuDoldur(ProjeConstants.KULLANIMDURUMU_KDKIRAKABYOK, "Kiralanma Talebi Olmayan Taşınmazlar", 5);
            TabloyuDoldur(ProjeConstants.KULLANIMDURUMU_KDCOKHIS, "Hisseli Taşınmazlar", 6);
            TabloyuDoldur(ProjeConstants.KULLANIMDURUMU_KDVAKKUL, "Vakıf Kullanımında", 7);
            TabloyuDoldur(ProjeConstants.KULLANIMDURUMU_RISKLIYAPI_KENTSELDONUSUM, "Riskli Yapı-Kentsel Dönüşüm", 8);
            TabloyuDoldur(ProjeConstants.KULLANIMDURUMU_KATKARSILIGI_YENIYAPI, "Kat Karşılığı-Yeni Yapı İnşası", 9);
            TabloyuDoldur(ProjeConstants.KULLANIMDURUMU_DIGER, " Metruk-Tahditli vb.", 10);
            GenelToplamiBul();
            TahminiRayicTopTxt.Value = TahminiRayicToplaminiBul().ToString();
            EmlakBeyanTopTxt.Value = EmlakBeyanToplaminiBul().ToString();
        }
        private void GenelToplamiBul()
        {
            bool kiradaMi = false;
            Tasinmaz tasinmaz = new Tasinmaz();
            int GMTopTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMDURUMU_KIRADA, ProjeConstants.MULKIYETSEKLI_TM, kiradaMi);
            int GMTopCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMDURUMU_KIRADA, ProjeConstants.MULKIYETSEKLI_CM, kiradaMi);
            int IstTopTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMDURUMU_KIRADA, ProjeConstants.MULKIYETSEKLI_TM, kiradaMi);
            int IstTopCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMDURUMU_KIRADA, ProjeConstants.MULKIYETSEKLI_CM, kiradaMi);
            int IzmTopTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMDURUMU_KIRADA, ProjeConstants.MULKIYETSEKLI_TM, kiradaMi);
            int IzmTopCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMDURUMU_KIRADA, ProjeConstants.MULKIYETSEKLI_CM, kiradaMi);
            int MerTopTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMDURUMU_KIRADA, ProjeConstants.MULKIYETSEKLI_TM, kiradaMi);
            int MerTopCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMDURUMU_KIRADA, ProjeConstants.MULKIYETSEKLI_CM, kiradaMi);

            TableRow rowGenelToplam = new TableRow
            {
                HorizontalAlign = HorizontalAlign.Center
            };

            TableCell BaslikCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = ("Genel Toplam"),
                ColumnSpan = 3
            };
            rowGenelToplam.Controls.Add(BaslikCell);

            TableCell GMTopTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                CssClass = "font-weight-bold",
                Text = (GMTopTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell GMTopCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                CssClass = "font-weight-bold",
                Text = (GMTopCM).ReturnEmptyIfZeroOrNull().ToString()
            };
            rowGenelToplam.Controls.Add(GMTopTMCell);
            rowGenelToplam.Controls.Add(GMTopCMCell);

            TableCell IstTopTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                CssClass = "font-weight-bold",
                Text = (IstTopTM).ReturnEmptyIfZeroOrNull().ToString()
            };
            TableCell IstTopCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                CssClass = "font-weight-bold",
                Text = (IstTopCM).ReturnEmptyIfZeroOrNull().ToString()
            };
            rowGenelToplam.Controls.Add(IstTopTMCell);
            rowGenelToplam.Controls.Add(IstTopCMCell);

            TableCell IzmTopTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                CssClass = "font-weight-bold",
                Text = (IzmTopTM).ReturnEmptyIfZeroOrNull().ToString()
            };
            TableCell IzmTopCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                CssClass = "font-weight-bold",
                Text = (IzmTopCM).ReturnEmptyIfZeroOrNull().ToString()
            };
            rowGenelToplam.Controls.Add(IzmTopTMCell);
            rowGenelToplam.Controls.Add(IzmTopCMCell);

            TableCell MerTopTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                CssClass = "font-weight-bold",
                Text = (MerTopTM).ReturnEmptyIfZeroOrNull().ToString()
            };
            TableCell MerTopCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                CssClass = "font-weight-bold",
                Text = (MerTopCM).ReturnEmptyIfZeroOrNull().ToString()
            };
            rowGenelToplam.Controls.Add(MerTopTMCell);
            rowGenelToplam.Controls.Add(MerTopCMCell);

            TableCell TopTopTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                CssClass = "font-weight-bold",
                Text = (GMTopTM + IstTopTM + IzmTopTM + MerTopTM).ReturnEmptyIfZeroOrNull().ToString()
            };
            TableCell TopTopCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                CssClass = "font-weight-bold",
                Text = (GMTopCM + IstTopCM + IzmTopCM + MerTopCM).ReturnEmptyIfZeroOrNull().ToString()
            };
            rowGenelToplam.Controls.Add(TopTopTMCell);
            rowGenelToplam.Controls.Add(TopTopCMCell);

            TableCell TopTopTMCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                CssClass = "font-weight-bold",
                Text = (GMTopTM + IstTopTM + IzmTopTM + MerTopTM + GMTopCM + IstTopCM + IzmTopCM + MerTopCM).ReturnEmptyIfZeroOrNull().ToString()
            };
            rowGenelToplam.Controls.Add(TopTopTMCMCell);

            KVTTable.Controls.Add(rowGenelToplam);

        }
        private decimal TahminiRayicToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectTahminiRayicToplamiByKullanimDurumu(ProjeConstants.KULLANIMDURUMU_KIRADA, false);
            return toplam;
        }
        private decimal EmlakBeyanToplaminiBul()
        {
            decimal toplam = 0;
            Tasinmaz tasinmaz = new Tasinmaz();
            toplam = tasinmaz.SelectEmlakBeyanToplamiByKullanimDurumu(ProjeConstants.KULLANIMDURUMU_KIRADA, false);
            return toplam;
        }
        private void TabloyuDoldur(string kDurumu, string rowTitle, int sira)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            int GMIshaniTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_ISHANI, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int IstIshaniTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_ISHANI, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int IzmIshaniTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_ISHANI, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int MerIshaniTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_ISHANI, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int TopIshaniTM = GMIshaniTM + IstIshaniTM + IzmIshaniTM + MerIshaniTM;

            int GMAptTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_APT, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int IstAptTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_APT, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int IzmAptTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_APT, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int MerAptTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_APT, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int TopAptTM = GMAptTM + IstAptTM + IzmAptTM + MerAptTM;

            int GMMesTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_MESKEN, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int IstMesTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_MESKEN, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int IzmMesTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_MESKEN, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int MerMesTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_MESKEN, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int TopMesTM = GMMesTM + IstMesTM + IzmMesTM + MerMesTM;

            int GMIsyTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_ISYERI, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int IstIsyTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_ISYERI, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int IzmIsyTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_ISYERI, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int MerIsyTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_ISYERI, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int TopIsyTM = GMIsyTM + IstIsyTM + IzmIsyTM + MerIsyTM;

            int GMArsTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_ARSA, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int IstArsTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_ARSA, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int IzmArsTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_ARSA, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int MerArsTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_ARSA, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int TopArsTM = GMArsTM + IstArsTM + IzmArsTM + MerArsTM;

            int GMTarTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_TARLA, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int IstTarTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_TARLA, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int IzmTarTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_TARLA, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int MerTarTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_TARLA, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int TopTarTM = GMTarTM + IstTarTM + IzmTarTM + MerTarTM;

            int GMMevTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_MEV, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int IstMevTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_MEV, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int IzmMevTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_MEV, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int MerMevTM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_MEV, kDurumu, ProjeConstants.MULKIYETSEKLI_TM);
            int TopMevTM = GMMevTM + IstMevTM + IzmMevTM + MerMevTM;

            int GMTopTM = GMIshaniTM + GMAptTM + GMMesTM + GMIsyTM + GMArsTM + GMTarTM + GMMevTM;
            int IstTopTM = IstIshaniTM + IstAptTM + IstMesTM + IstIsyTM + IstArsTM + IstTarTM + IstMevTM;
            int IzmTopTM = IzmIshaniTM + IzmAptTM + IzmMesTM + IzmIsyTM + IzmArsTM + IzmTarTM + IzmMevTM;
            int MerTopTM = MerIshaniTM + MerAptTM + MerMesTM + MerIsyTM + MerArsTM + MerTarTM + MerMevTM;

            int GMIshaniCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_ISHANI, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int IstIshaniCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_ISHANI, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int IzmIshaniCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_ISHANI, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int MerIshaniCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_ISHANI, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int TopIshaniCM = GMIshaniCM + IstIshaniCM + IzmIshaniCM + MerIshaniCM;

            int GMAptCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_APT, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int IstAptCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_APT, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int IzmAptCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_APT, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int MerAptCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_APT, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int TopAptCM = GMAptCM + IstAptCM + IzmAptCM + MerAptCM;

            int GMMesCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_MESKEN, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int IstMesCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_MESKEN, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int IzmMesCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_MESKEN, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int MerMesCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_MESKEN, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int TopMesCM = GMMesCM + IstMesCM + IzmMesCM + MerMesCM;

            int GMIsyCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_ISYERI, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int IstIsyCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_ISYERI, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int IzmIsyCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_ISYERI, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int MerIsyCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_ISYERI, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int TopIsyCM = GMIsyCM + IstIsyCM + IzmIsyCM + MerIsyCM;

            int GMArsCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_ARSA, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int IstArsCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_ARSA, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int IzmArsCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_ARSA, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int MerArsCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_ARSA, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int TopArsCM = GMArsCM + IstArsCM + IzmArsCM + MerArsCM;

            int GMTarCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_TARLA, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int IstTarCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_TARLA, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int IzmTarCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_TARLA, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int MerTarCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_TARLA, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int TopTarCM = GMTarCM + IstTarCM + IzmTarCM + MerTarCM;

            int GMMevCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.KULLANIMSEKLI_MEV, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int IstMevCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.KULLANIMSEKLI_MEV, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int IzmMevCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_IZMIR, ProjeConstants.KULLANIMSEKLI_MEV, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int MerMevCM = tasinmaz.SelectTasinmazAdetByBolgeKullanimSekliKullanimDurumu(ProjeConstants.BOLGE_MERSIN, ProjeConstants.KULLANIMSEKLI_MEV, kDurumu, ProjeConstants.MULKIYETSEKLI_CM);
            int TopMevCM = GMMevCM + IstMevCM + IzmMevCM + MerMevCM;

            int GMTopCM = GMIshaniCM + GMAptCM + GMMesCM + GMIsyCM + GMArsCM + GMTarCM + GMMevCM;
            int IstTopCM = IstIshaniCM + IstAptCM + IstMesCM + IstIsyCM + IstArsCM + IstTarCM + IstMevCM;
            int IzmTopCM = IzmIshaniCM + IzmAptCM + IzmMesCM + IzmIsyCM + IzmArsCM + IzmTarCM + IzmMevCM;
            int MerTopCM = MerIshaniCM + MerAptCM + MerMesCM + MerIsyCM + MerArsCM + MerTarCM + MerMevCM;

            int TopTM = GMTopTM + IstTopTM + IzmTopTM + MerTopTM;
            int TopCM = GMTopCM + IstTopCM + IzmTopCM + MerTopCM;

            #region Row1
            TableRow row1 = new TableRow
            {
                HorizontalAlign = HorizontalAlign.Center
            };

            TableCell SiraNoCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (sira).ReturnEmptyIfZeroOrNull().ToString(),
                RowSpan = 7
            };
            row1.Controls.Add(SiraNoCell);

            TableCell B1Cell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = rowTitle,
                RowSpan = 7
            };
            row1.Controls.Add(B1Cell);

            TableCell KvAptCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = "Apt./İşhanı"
            };
            row1.Controls.Add(KvAptCell);
            //TM
            TableCell KvAptGMTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (GMAptTM + GMIshaniTM).ReturnEmptyIfZeroOrNull().ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvAptIstTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IstAptTM + IstIshaniTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvAptIzmTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IzmAptTM + IzmIshaniTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvAptMerTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (MerAptTM + MerIshaniTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvAptTopTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopAptTM + TopIshaniTM).ReturnEmptyIfZeroOrNull().ToString()
            };
            //CM
            TableCell KvAptGMCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (GMAptCM + GMIshaniCM).ReturnEmptyIfZeroOrNull().ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvAptIstCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IstAptCM + IstIshaniCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvAptIzmCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IzmAptCM + IzmIshaniCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvAptMerCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (MerAptCM + MerIshaniCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvAptTopCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopAptCM + TopIshaniCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvAptTopTMCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopAptTM + TopIshaniTM + TopAptCM + TopIshaniCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            row1.Controls.Add(KvAptGMTMCell);
            row1.Controls.Add(KvAptGMCMCell);
            row1.Controls.Add(KvAptIstTMCell);
            row1.Controls.Add(KvAptIstCMCell);
            row1.Controls.Add(KvAptIzmTMCell);
            row1.Controls.Add(KvAptIzmCMCell);
            row1.Controls.Add(KvAptMerTMCell);
            row1.Controls.Add(KvAptMerCMCell);
            row1.Controls.Add(KvAptTopTMCell);
            row1.Controls.Add(KvAptTopCMCell);
            row1.Controls.Add(KvAptTopTMCMCell);

            KVTTable.Controls.Add(row1);
            #endregion
            //birinci satır toplam bitti
            #region Row2

            //ikinci Satır
            TableRow row2 = new TableRow
            {
                HorizontalAlign = HorizontalAlign.Center
            };

            TableCell KvMesCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = "Mesken"
            };
            row2.Controls.Add(KvMesCell);
            //TM
            TableCell KvMesGMTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (GMMesTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvMesIstTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IstMesTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvMesIzmTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IzmMesTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvMesMerTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (MerMesTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvMesTopTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopMesTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            //CM
            TableCell KvMesGMCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (GMMesCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvMesIstCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IstMesCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvMesIzmCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IzmMesCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvMesMerCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (MerMesCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvMesTopCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopMesCM).ReturnEmptyIfZeroOrNull().ToString()
            };
            TableCell KvMesTopTMCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopMesTM + TopMesCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            row2.Controls.Add(KvMesGMTMCell);
            row2.Controls.Add(KvMesGMCMCell);
            row2.Controls.Add(KvMesGMTMCell);
            row2.Controls.Add(KvMesGMCMCell);
            row2.Controls.Add(KvMesIstTMCell);
            row2.Controls.Add(KvMesIstCMCell);
            row2.Controls.Add(KvMesIzmTMCell);
            row2.Controls.Add(KvMesIzmCMCell);
            row2.Controls.Add(KvMesMerTMCell);
            row2.Controls.Add(KvMesMerCMCell);
            row2.Controls.Add(KvMesTopTMCell);
            row2.Controls.Add(KvMesTopCMCell);
            row2.Controls.Add(KvMesTopTMCMCell);


            KVTTable.Controls.Add(row2);
            #endregion
            //Mesken satır bitti
            #region Row3
            //M.Ev
            TableRow row3 = new TableRow
            {
                HorizontalAlign = HorizontalAlign.Center
            };

            TableCell KvMevCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = "Müstakil Ev"
            };
            row3.Controls.Add(KvMevCell);

            //TM
            TableCell KvMevGMTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (GMMevTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvMevIstTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IstMevTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvMevIzmTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IzmMevTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvMevMerTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (MerMevTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvMevTopTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopMevTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            //CM
            TableCell KvMevGMCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (GMMevCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvMevIstCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IstMevCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvMevIzmCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IzmMevCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvMevMerCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (MerMevCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvMevTopCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopMevCM).ReturnEmptyIfZeroOrNull().ToString()
            };
            TableCell KvMevTopTMCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopMevTM + TopMevCM).ReturnEmptyIfZeroOrNull().ToString()
            };
            row3.Controls.Add(KvMevGMTMCell);
            row3.Controls.Add(KvMevGMCMCell);
            row3.Controls.Add(KvMevGMTMCell);
            row3.Controls.Add(KvMevGMCMCell);
            row3.Controls.Add(KvMevIstTMCell);
            row3.Controls.Add(KvMevIstCMCell);
            row3.Controls.Add(KvMevIzmTMCell);
            row3.Controls.Add(KvMevIzmCMCell);
            row3.Controls.Add(KvMevMerTMCell);
            row3.Controls.Add(KvMevMerCMCell);
            row3.Controls.Add(KvMevTopTMCell);
            row3.Controls.Add(KvMevTopCMCell);
            row3.Controls.Add(KvMevTopTMCMCell);

            KVTTable.Controls.Add(row3);
            #endregion
            //Mev bitti

            #region Row4
            //İsyeri Satır
            TableRow row4 = new TableRow
            {
                HorizontalAlign = HorizontalAlign.Center
            };

            TableCell KvIsyCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = "İşyeri"
            };
            row4.Controls.Add(KvIsyCell);

            //TM
            TableCell KvIsyGMTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (GMIsyTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvIsyIstTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IstIsyTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvIsyIzmTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IzmIsyTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvIsyMerTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (MerIsyTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvIsyTopTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopIsyTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            //CM
            TableCell KvIsyGMCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (GMIsyCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvIsyIstCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IstIsyCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvIsyIzmCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IzmIsyCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvIsyMerCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (MerIsyCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvIsyTopCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopIsyCM).ReturnEmptyIfZeroOrNull().ToString()
            };
            TableCell KvIsyTopTMCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopIsyTM + TopIsyCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            row4.Controls.Add(KvIsyGMTMCell);
            row4.Controls.Add(KvIsyGMCMCell);
            row4.Controls.Add(KvIsyGMTMCell);
            row4.Controls.Add(KvIsyGMCMCell);
            row4.Controls.Add(KvIsyIstTMCell);
            row4.Controls.Add(KvIsyIstCMCell);
            row4.Controls.Add(KvIsyIzmTMCell);
            row4.Controls.Add(KvIsyIzmCMCell);
            row4.Controls.Add(KvIsyMerTMCell);
            row4.Controls.Add(KvIsyMerCMCell);
            row4.Controls.Add(KvIsyTopTMCell);
            row4.Controls.Add(KvIsyTopCMCell);
            row4.Controls.Add(KvIsyTopTMCMCell);

            KVTTable.Controls.Add(row4);
            #endregion
            //İsyeri satır bitti

            #region Row5
            //Arsa satır
            TableRow row5 = new TableRow
            {
                HorizontalAlign = HorizontalAlign.Center
            };

            TableCell KvArsCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = "Arsa"
            };
            row5.Controls.Add(KvArsCell);

            //TM
            TableCell KvArsGMTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (GMArsTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvArsIstTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IstArsTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvArsIzmTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IzmArsTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvArsMerTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (MerArsTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvArsTopTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopArsTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            //CM
            TableCell KvArsGMCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (GMArsCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvArsIstCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IstArsCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvArsIzmCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IzmArsCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvArsMerCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (MerArsCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvArsTopCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopArsCM).ReturnEmptyIfZeroOrNull().ToString()
            };
            TableCell KvArsTopTMCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopArsTM + TopArsCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            row5.Controls.Add(KvArsGMTMCell);
            row5.Controls.Add(KvArsGMCMCell);
            row5.Controls.Add(KvArsGMTMCell);
            row5.Controls.Add(KvArsGMCMCell);
            row5.Controls.Add(KvArsIstTMCell);
            row5.Controls.Add(KvArsIstCMCell);
            row5.Controls.Add(KvArsIzmTMCell);
            row5.Controls.Add(KvArsIzmCMCell);
            row5.Controls.Add(KvArsMerTMCell);
            row5.Controls.Add(KvArsMerCMCell);
            row5.Controls.Add(KvArsTopTMCell);
            row5.Controls.Add(KvArsTopCMCell);
            row5.Controls.Add(KvArsTopTMCMCell);

            KVTTable.Controls.Add(row5);
            #endregion
            //Arsa satır bitti

            #region Row7
            //Tarla satır
            TableRow row7 = new TableRow
            {
                HorizontalAlign = HorizontalAlign.Center
            };

            TableCell KvTarCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = "Tarla"
            };
            row7.Controls.Add(KvTarCell);

            //TM
            TableCell KvTarGMTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (GMTarTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvTarIstTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IstTarTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvTarIzmTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IzmTarTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvTarMerTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (MerTarTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvTarTopTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopTarTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            //CM
            TableCell KvTarGMCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (GMTarCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvTarIstCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IstTarCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvTarIzmCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IzmTarCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvTarMerCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (MerTarCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvTarTopCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopTarCM).ReturnEmptyIfZeroOrNull().ToString()
            };
            TableCell KvTarTopTMCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopTarTM + TopTarCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            row7.Controls.Add(KvTarGMTMCell);
            row7.Controls.Add(KvTarGMCMCell);
            row7.Controls.Add(KvTarGMTMCell);
            row7.Controls.Add(KvTarGMCMCell);
            row7.Controls.Add(KvTarIstTMCell);
            row7.Controls.Add(KvTarIstCMCell);
            row7.Controls.Add(KvTarIzmTMCell);
            row7.Controls.Add(KvTarIzmCMCell);
            row7.Controls.Add(KvTarMerTMCell);
            row7.Controls.Add(KvTarMerCMCell);
            row7.Controls.Add(KvTarTopTMCell);
            row7.Controls.Add(KvTarTopCMCell);
            row7.Controls.Add(KvTarTopTMCMCell);

            KVTTable.Controls.Add(row7);
            #endregion
            //Tarla satır bitti

            #region Row8
            //toplam satır
            TableRow row8 = new TableRow
            {
                HorizontalAlign = HorizontalAlign.Center
            };

            TableCell KvTopCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = "Toplam"
            };
            row8.Controls.Add(KvTopCell);

            //TM
            TableCell KvTopGMTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (GMTopTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvTopIstTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IstTopTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvTopIzmTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IzmTopTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvTopMerTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (MerTopTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvTopTopTMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text =(TopTM).ReturnEmptyIfZeroOrNull().ToString()
            };

            //CM
            TableCell KvTopGMCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (GMTopCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvTopIstCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IstTopCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvTopIzmCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (IzmTopCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvTopMerCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (MerTopCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            TableCell KvTopTopCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopCM).ReturnEmptyIfZeroOrNull().ToString()
            };
            TableCell KvTopTopTMCMCell = new TableCell
            {
                BorderStyle = BorderStyle.Solid,
                Text = (TopTM + TopCM).ReturnEmptyIfZeroOrNull().ToString()
            };

            row8.Controls.Add(KvTopGMTMCell);
            row8.Controls.Add(KvTopGMCMCell);
            row8.Controls.Add(KvTopGMTMCell);
            row8.Controls.Add(KvTopGMCMCell);
            row8.Controls.Add(KvTopIstTMCell);
            row8.Controls.Add(KvTopIstCMCell);
            row8.Controls.Add(KvTopIzmTMCell);
            row8.Controls.Add(KvTopIzmCMCell);
            row8.Controls.Add(KvTopMerTMCell);
            row8.Controls.Add(KvTopMerCMCell);
            row8.Controls.Add(KvTopTopTMCell);
            row8.Controls.Add(KvTopTopCMCell);
            row8.Controls.Add(KvTopTopTMCMCell);
            

            KVTTable.Controls.Add(row8);
            #endregion
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
            string filename = "KirayaVerilmeyenTasinmazlar" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            TableContainer.RenderControl(hw);
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
