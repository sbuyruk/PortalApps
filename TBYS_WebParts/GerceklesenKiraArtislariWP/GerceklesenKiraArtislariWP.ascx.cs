using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.GerceklesenKiraArtislariWP
{
    [ToolboxItemAttribute(false)]
    public partial class GerceklesenKiraArtislariWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public GerceklesenKiraArtislariWP()
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
            try
            {
                if (!Page.IsPostBack)
                {
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            //var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir("setDataSet(" + jsonData + ");");
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<KiraArtisListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
                jSon = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToExcel();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void ExportToExcel()
        {
            TabloOlustur();
            string filename = "GerceklesenKiraArtisCizelgesi.xls";// + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            //Türkçe sorunu yok
            Page.Response.Clear();
            Page.Response.AddHeader("content-disposition", "attachment;filename=" + filename + "");
            Page.Response.ContentType = "application/ms-excel";
            Page.Response.ContentEncoding = System.Text.Encoding.Unicode;
            Page.Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(sw);

            TabloDiv.RenderControl(hw);

            Page.Response.Write(sw.ToString());
            Page.Response.End();
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private List<KiraArtisListItem> GetDataList()
        {


            KiraSozlesme kiraSozlesmeDao = new KiraSozlesme();

            DataTable dataTable = kiraSozlesmeDao.SelectGerceklesenKiraArtislariReturnDT(ProjeConstants.SINIRLIKIRAARTISI_BASLAMATARIHI);
            int SiraNo = 1;

            List<KiraArtisListItem> list = new List<KiraArtisListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            DataView dataView = new DataView(dataTable);
            foreach (DataRowView row in dataView)
            {
                string kiraciId = row["KiraciId"].ToString();
                string kiraciAdi = row["KiraciAdi"].ToString();
                string kiraciSoyadi = row["KiraciSoyadi"].ToString();
                DateTime ilkSozlesmeTar = row["IlkSozlesmeTar"].ConvertToDatetime();
                string ilkSozlesmeTarStr = row["IlkSozlesmeTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                string sozBasTar = row["SozBasTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                string sozBitTar = row["SozBitTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();                
                string oncekiSozBasTar = row["OncekiSozBasTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                string oncekiSozBitTar = row["OncekiSozBitTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                string kiralamaAmaci = row["KiralamaAmaci"].ToString();
                decimal kiraBedeli = row["KiraBedeli"].ConvertToDecimal();
                decimal oncekiKiraBedeli = row["OncekiKiraBedeli"].ConvertToDecimal();
                int kiraSozlesmeId = row["KiraSozlesmeId"].ConvertToInt();
                string artisAyi = row["ArtisAyi"].ToString();
                bool aktif = row["Aktif"].ReturnFalseIfNull().ConvertToBool();

                decimal tufe = TufeBul(sozBasTar.ConvertToDatetime());
                DateTime bugun = DateTime.Today;
                decimal yasalOranaGoreKiraBedeli = oncekiKiraBedeli + Math.Round(oncekiKiraBedeli * tufe / 100);
                
                string yasalArtisOrani = "%" + tufe.ToString("N", culturInfo) + " (TÜFE)";
                DateTime bastar = string.IsNullOrEmpty(sozBasTar.ConvertToDatetimeEmptyIfNull()) ? DateTime.Today : sozBasTar.ConvertToDatetime();
                if ((bastar >= ProjeConstants.SINIRLIKIRAARTISI_BASLAMATARIHI) &&
                   (bastar <= ProjeConstants.SINIRLIKIRAARTISI_BITISTARIHI) &&
                   kiralamaAmaci.Equals(ProjeConstants.SINIRLIKIRAARTISI_UYGULANACAKTASINMAZCINSI))
                {
                    yasalOranaGoreKiraBedeli = Math.Round(oncekiKiraBedeli + oncekiKiraBedeli * ProjeConstants.SINIRLIKIRAARTISI_ORANI / 100);
                    yasalArtisOrani = "%" + ProjeConstants.SINIRLIKIRAARTISI_ORANI.ToString("N", culturInfo) + " (6098 Say.Kanun)";
                }
                else
                {
                    yasalOranaGoreKiraBedeli = Math.Round(oncekiKiraBedeli + oncekiKiraBedeli * tufe / 100);
                }

                string adres = row["Adres"].ToString();

                string ili = row["Ili"].ToString();
                string ilcesi = row["Ilcesi"].ToString();
                string bolge = row["bolge"].ToString();

                KiraArtisListItem kiraArtisListItem = new KiraArtisListItem();
                kiraArtisListItem.Sirano = SiraNo++.ToString();
                kiraArtisListItem.SozlesmeId = kiraSozlesmeId.ToString();
                kiraArtisListItem.KiraciId = kiraciId;
                kiraArtisListItem.KiraciAdi = kiraciAdi + " " + kiraciSoyadi;
                kiraArtisListItem.SozlesmeTarihi = ilkSozlesmeTarStr;

                int kiraSuresi = Math.Round(DateTime.Today.AddMonths(1).Subtract(ilkSozlesmeTar).TotalDays / 365).ConvertToInt();
                kiraArtisListItem.KiraSuresi = kiraSuresi + " Yıl";
                kiraArtisListItem.BesYil = kiraSuresi >= 5 ? "True" : "False";
                kiraArtisListItem.OnYil = kiraSuresi >= 10 ? "True" : "False";
                kiraArtisListItem.SozlesmeBasTar = sozBasTar.ConvertToDatetimeEmptyIfNull();
                kiraArtisListItem.SozlesmeBitTar = sozBitTar.ConvertToDatetimeEmptyIfNull();
                kiraArtisListItem.TarihAraligi = sozBasTar.ConvertToDatetimeEmptyIfNull() + "-" + sozBitTar.ConvertToDatetimeEmptyIfNull();
                kiraArtisListItem.KiralamaAmaci = kiralamaAmaci;
                kiraArtisListItem.YasalArtisOrani = yasalArtisOrani;
                kiraArtisListItem.YasalOranaGoreKiraBedeli = yasalOranaGoreKiraBedeli.ToString("N", culturInfo);
                kiraArtisListItem.KiraBedeli = kiraBedeli.ToString("N", culturInfo);
                kiraArtisListItem.OncekiKiraBedeli = oncekiKiraBedeli.ToString("N", culturInfo);
                decimal fark = (kiraBedeli - oncekiKiraBedeli);
                kiraArtisListItem.UygulananArtisOrani = "%" + (fark * 100 / oncekiKiraBedeli).ToString("N", culturInfo);
                kiraArtisListItem.Adres = "- " + adres;
                kiraArtisListItem.TamAdres = "- " + adres + " " + ilcesi + "-" + ili;
                kiraArtisListItem.Ilcesi = ilcesi;
                kiraArtisListItem.Ili = ili;
                kiraArtisListItem.Bolge = bolge;
                kiraArtisListItem.ArtisAyi = new DateTime(DateTime.Today.Year, artisAyi.ConvertToInt(), 1).ToString("MMMM");
                kiraArtisListItem.YenilendiMi = aktif ? "Yenilenecek" : "Yenilendi";
                list.Add(kiraArtisListItem);
            }
            return list;
        }
        private decimal TufeBul(DateTime tarih)
        {
            decimal tufe = 1M;
            YasalFaiz yasalFaiz = new YasalFaiz();

            yasalFaiz = yasalFaiz.SelectByYilAy(tarih.Year, tarih.Month);//gelecek ay artacak
            if (yasalFaiz != null)
            {
                tufe = yasalFaiz.Tufe;
            }
            return tufe;
        }
        private class KiraArtisListItem
        {
            public string Sirano { get; set; }
            public string ArtisAyi { get; set; }
            public string SozlesmeId { get; set; }
            public string KiraciAdi { get; set; }
            public string KiraciId { get; set; }
            public string KiralamaAmaci { get; set; }
            public string SozlesmeTarihi { get; set; }
            public string KiraSuresi { get; set; }
            public string SozlesmeBasTar { get; set; }
            public string SozlesmeBitTar { get; set; }
            public string OncekiSozlesmeBasTar { get; set; }
            public string OncekiSozlesmeBitTar { get; set; }
            public string TarihAraligi { get; set; }
            public string KiraBedeli { get; set; }
            public string OncekiKiraBedeli { get; set; }
            public string YasalArtisOrani { get; set; }
            public string YasalOranaGoreKiraBedeli { get; set; }
            public string UygulananArtisOrani { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Bolge { get; set; }
            public string Adres { get; set; }
            public string TamAdres { get; set; }
            public string YenilendiMi { get; set; }
            public string BesYil { get; set; }
            public string OnYil { get; set; }
        }
    }
}