using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace BTYS_Webparts.BolgeKiraArtisCizelgesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BolgeKiraArtisCizelgesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BolgeKiraArtisCizelgesiWP()
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
            try
            {

                if (!Page.IsPostBack)
                {

                    Bolge bolge = IKYSOrtak.BolgeGetirByUserName(CurrentUserName);
                    BolgeIdQS = bolge == null ? 0 : bolge.Id;
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
                serializer.MaxJsonLength = Int32.MaxValue;
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
            string filename = "KiraArtisCizelgesi.xls";// + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
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


            KiraSozlesme kiraSozlesme = new KiraSozlesme();

            DataTable dataTable = kiraSozlesme.SelectKiraArtisiGelenSozlesmelerReturnDataTable(BolgeIdQS);
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
                string kiralamaAmaci = row["KiralamaAmaci"].ToString();
                decimal kiraBedeli = row["KiraBedeli"].ConvertToDecimal();
                int kiraSozlesmeId = row["KiraSozlesmeId"].ConvertToInt();
                string artisAyi = row["ArtisAyi"].ToString();

                decimal tufe = GelecekAyIcinTufeBul();
                DateTime bugun = DateTime.Today;
                decimal yeniKiraBedeli = kiraBedeli + Math.Round(kiraBedeli * tufe / 100);
                bool sozlesmeYenilendiMi = sozBitTar.ConvertToDatetime() > new DateTime(bugun.AddMonths(1).Year, bugun.AddMonths(1).Month, 1);
                if (sozlesmeYenilendiMi)
                {
                    KiraSozlesme oncekiKiraSozlesme = new KiraSozlesme();
                    oncekiKiraSozlesme = oncekiKiraSozlesme.SelectByKiraciIdTarih(kiraciId.ConvertToInt(), sozBitTar.ConvertToDatetime().AddMonths(-1));
                    if (oncekiKiraSozlesme != null)
                    {
                        kiraBedeli = oncekiKiraSozlesme.KiraBedeli;
                    }

                }
                yeniKiraBedeli = Math.Round(kiraBedeli + kiraBedeli * tufe / 100);
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
                kiraArtisListItem.KiraBedeli = kiraBedeli.ToString("N", culturInfo) + " TL";
                kiraArtisListItem.Tufe = "%" + tufe.ToString("N", culturInfo) + "";
                kiraArtisListItem.YeniKiraBedeli = yeniKiraBedeli.ToString("N", culturInfo) + " TL";
                kiraArtisListItem.Adres = "- " + adres;
                kiraArtisListItem.TamAdres = "- " + adres + " " + ilcesi + "-" + ili;
                kiraArtisListItem.Ilcesi = ilcesi;
                kiraArtisListItem.Ili = ili;
                kiraArtisListItem.Bolge = bolge;
                kiraArtisListItem.ArtisAyi = new DateTime(DateTime.Today.Year, artisAyi.ConvertToInt(), 1).ToString("MMMM");
                kiraArtisListItem.YenilendiMi = sozlesmeYenilendiMi ? "Yenilendi" : "Yenilenecek";
                list.Add(kiraArtisListItem);
            }
            return list;
        }
        private decimal GelecekAyIcinTufeBul()
        {
            decimal tufe = 1M;
            YasalFaiz yasalFaiz = new YasalFaiz();
            DateTime gelecekAy = DateTime.Today.AddMonths(1);
            yasalFaiz = yasalFaiz.SelectByYilAy(gelecekAy.Year, gelecekAy.Month);//gelecek ay artacak
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
            public string TarihAraligi { get; set; }
            public string KiraBedeli { get; set; }
            public string Tufe { get; set; }
            public string YeniKiraBedeli { get; set; }
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
