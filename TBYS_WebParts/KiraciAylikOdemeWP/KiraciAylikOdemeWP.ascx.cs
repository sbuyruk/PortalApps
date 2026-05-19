using Model.MTS;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.KiraciAylikOdemeWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraciAylikOdemeWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraciAylikOdemeWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string KiraciIdQS
        {
            get
            {

                if (ViewState["KiraciId"] == null)
                {
                    if (Page.Request.QueryString["KiraciId"] != null)
                    {
                        ViewState["KiraciId"] = Page.Request.QueryString["KiraciId"];
                    }
                    else
                    {
                        ViewState["KiraciId"] = "0";
                    }
                }
                return ViewState["KiraciId"].ToString();
            }

            set
            {
                ViewState["KiraciId"] = value;
            }
        }
        private string BastarQS
        {
            get
            {

                if (ViewState["Bastar"] == null)
                {
                    if (Page.Request.QueryString["Bastar"] != null && !string.IsNullOrEmpty(Page.Request.QueryString["Bastar"].ToString()))
                    {
                        ViewState["Bastar"] = Page.Request.QueryString["Bastar"];
                    }
                    else
                    {
                        ViewState["Bastar"] = DateTime.Today.ToString();
                    }
                }
                return ViewState["Bastar"].ToString();
            }

            set
            {
                ViewState["Bastar"] = value;
            }
        }
        private string BittarQS
        {
            get
            {

                if (ViewState["Bittar"] == null)
                {
                    if (Page.Request.QueryString["Bittar"] != null && !string.IsNullOrEmpty(Page.Request.QueryString["Bittar"].ToString()))
                    {
                        ViewState["Bittar"] = Page.Request.QueryString["Bittar"];
                    }
                    else
                    {
                        ViewState["Bittar"] = DateTime.Today.ToString();
                    }
                }
                return ViewState["Bittar"].ToString();
            }

            set
            {
                ViewState["Bittar"] = value;
            }
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

        public DataTable DataTable { get; private set; }
        private IFormatProvider cultureInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Bolge bolge = IKYSOrtak.BolgeGetirByUserName(CurrentUserName);
                BolgeIdQS = bolge == null ? 0 : bolge.Id;
                BolgeDDLDoldur();
                DateTime now = DateTime.Now;
                BitisTarihiTxt.Text = now.ConvertToDatetimeEmptyIfNull();
                BaslangicTarihiTxt.Text = now.AddMonths(-1).ConvertToDatetimeEmptyIfNull();

                if (!string.IsNullOrEmpty(BastarQS) && !string.IsNullOrEmpty(BittarQS))
                {
                    DateTime basTarih = BastarQS.ConvertToDatetime();
                    BaslangicTarihiTxt.Text = basTarih.ConvertToDatetimeEmptyIfNull();
                    BitisTarihiTxt.Text = BittarQS.ConvertToDatetimeEmptyIfNull();
                }
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
                if (kiraci != null || string.IsNullOrEmpty(KiraciIdQS) || KiraciIdQS.Equals(ProjeConstants.HEPSI_INT.ToString()))
                {
                    KiraciTxt.Text = kiraci == null ? "Hepsi" : kiraci.Adi + " " + kiraci.Soyadi;
                    //OdemelerTablosunuDoldur();
                    TabloOlustur();
                }
                else
                {
                    MessageHelper.PublishMessage("Kiracı Bulunamadı", ProjeConstants.MESAJ_HATA);
                }
                if (BolgeIdQS.ConvertToInt() == ProjeConstants.BOLGE_HEPSI_INT || BolgeIdQS.ConvertToInt() == ProjeConstants.BOLGE_GENELMUDURLUK_INT)
                {
                    YeniOdemeGirisiBtn.Visible = true;
                    OdemePlaniBtn.Visible = true;
                    SozlesmeBtn.Visible = true;
                    KiraciBtn.Visible = true;
                    OdemePlaniListBtn.Visible = true;
                    SozlesmeListBtn.Visible = true;
                    KiraciListBtn.Visible = true;
                    BakiyeDevirBtn.Visible = true;
                }
                else
                {
                    YeniOdemeGirisiBtn.Visible = false;
                    OdemePlaniBtn.Visible = false;
                    SozlesmeBtn.Visible = false;
                    KiraciBtn.Visible = false;
                    OdemePlaniListBtn.Visible = false;
                    SozlesmeListBtn.Visible = false;
                    KiraciListBtn.Visible = false;
                    BakiyeDevirBtn.Visible = false;

                } 
            }
        }

        protected void BaslangicTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            BastarQS = BaslangicTarihiTxt.Text;

            DateTime basTarih = BaslangicTarihiTxt.Text.ConvertToDatetime().Date;
            DateTime bitTarih = BitisTarihiTxt.Text.ConvertToDatetime().Date;

            if (basTarih > bitTarih)
            {
                BitisTarihiTxt.Text = BaslangicTarihiTxt.Text;
                bitTarih = basTarih;
            }

            DateTime maxBitisTarihi = basTarih.AddMonths(3);
            if (bitTarih > maxBitisTarihi)
            {
                BitisTarihiTxt.Text = maxBitisTarihi.ConvertToDatetimeEmptyIfNull();
                BittarQS = BitisTarihiTxt.Text;
            }

            TabloOlustur();
        }
        protected void BitisTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            BittarQS = BitisTarihiTxt.Text;
            DateTime basTarih = BaslangicTarihiTxt.Text.ConvertToDatetime().Date;
            DateTime bitTarih = BitisTarihiTxt.Text.ConvertToDatetime().Date;

            if (basTarih > bitTarih)
            {
                BitisTarihiTxt.Text = BaslangicTarihiTxt.Text;
                bitTarih = basTarih;
            }
            TabloOlustur();

        }
        private void TabloOlustur()
        {

            try
            {
                List<OdemeListItem> list = new List<OdemeListItem>();
                var jsonData = GetJsonData(); //veri çekilip json a çeviriliyor
                                              //var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
                UtilityHelper.ScriptCalistir("setDataSet(" + jsonData + ");");
                ToplamLbl.Text = "Toplam: " + GetToplamOdeme();
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
        }

        private string GetToplamOdeme()
        {
			decimal toplam = 0m;

			try
			{
				DateTime basTarih = BaslangicTarihiTxt.Text.ConvertToDatetime().Date;
				DateTime bitTarih = BitisTarihiTxt.Text.ConvertToDatetime().Date;
                int bolgeId = BolgeDDL.SelectedItem.Value.ConvertToInt();
                Odeme odemeDao = new Odeme();
				DataTable dataTable = odemeDao.SelectByKiraciAyYilReturnDataTable(bolgeId, KiraciIdQS.ConvertToInt(), basTarih, bitTarih);

				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					foreach (DataRow row in dataTable.Rows)
					{
						toplam += row["OdenenTutar"].ConvertToDecimal();
					}
				}
			}
			catch (Exception exception)
			{
				ExceptionHelper exceptionHelper = new ExceptionHelper();
				exceptionHelper.Exceptions.Add(exception);
				exceptionHelper.PublishException();
			}

			return toplam.ToString("N", cultureInfo) + " TL";
        }
        protected void BolgeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void SilNowBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.Select(SozlesmeIdHdn.Value.ConvertToInt());
            if (kiraSozlesme != null)
            {

                Odeme odeme = new Odeme();
                bool silindiMi = odeme.OdemeyiSilOdemePlaniniGuncelle(OdemeIdHdn.Value.ConvertToInt(), string.Empty, OdemePlaniIdIdHdn.Value.ConvertToInt(), CurrentUserName);
                if (silindiMi)
                {
                    RedirectToPage(ProjeConstants.PAGE_KIRACI_AYLIKODEME + "?KiraciId=" + KiraciIdQS + "&Bittar=" + BittarQS + "&Bastar=" + BastarQS);
                }

            }
            else
            {
                MessageHelper.PublishMessage("Sözleşme Bulunamadı", ProjeConstants.MESAJ_HATA);
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_HOME);
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void KiraciBtn_Click(object sender, EventArgs e)
        {

            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select(KiraciIdQS.ConvertToInt());
            if (kiraci != null)
            {
                RedirectToPage(ProjeConstants.PAGE_KIRACI_GIRIS + "?DestinationApp=KD&SenderApp=KL&KiraciId=" + KiraciIdQS);
            }
            else
            {
                MessageHelper.PublishMessage("Kiracı Bulunamadı", ProjeConstants.MESAJ_HATA);
            }


        }
        protected void SozlesmeBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.SelectAktifSozlesmeByKiraciId(KiraciIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                RedirectToPage(ProjeConstants.PAGE_KIRASOZLESMESI + "?KiraSozlesmeId=" + kiraSozlesme.Id);
            }
            else
            {
                kiraSozlesme = kiraSozlesme.SelectBitenSozlesmeByKiraciId(KiraciIdQS.ConvertToInt());

                if (kiraSozlesme == null)
                    MessageHelper.PublishMessage("Kiracıya ait bir Kira Sözleşmesi bulunamadı.", ProjeConstants.MESAJ_HATA);
            }
        }
        protected void SozlesmeListBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme sozlesme = new KiraSozlesme();
            sozlesme = sozlesme.SelectAktifSozlesmeByKiraciId(KiraciIdQS.ConvertToInt());
            if (sozlesme == null)
                RedirectToPage(ProjeConstants.PAGE_BITENKIRASOZLESME_LIST);
            else
                RedirectToPage(ProjeConstants.PAGE_KIRASOZLESME_LIST);
        }
        protected void OdemePlaniListBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_ODEMEPLANI_LIST);
        }
        protected void KiraciListBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KIRACI_LIST);

        }
        protected void BakiyeDevirBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KIRA_BAKIYEDEVRI + "?KiraciId=" + KiraciIdQS);
        }
        protected void KiraciSecBtn_Click(object sender, EventArgs e)
        {

            KiraciModalAc();
        }
        protected void KiraciSecNowBtn_Click(object sender, EventArgs e)
        {

            RedirectToPage(ProjeConstants.PAGE_KIRACI_AYLIKODEME + "?KiraciId=" + paramKiraciIdLbl.Value.ConvertToInt() + "&Bastar=" + BastarQS + "&Bittar=" + BittarQS);
        }
        protected void OdemePlaniBtn_Click(object sender, EventArgs e)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            kiraSozlesme = kiraSozlesme.SelectAktifSozlesmeByKiraciId(KiraciIdQS.ConvertToInt());
            if (kiraSozlesme != null)
            {
                if (kiraSozlesme != null)
                {
                    RedirectToPage(ProjeConstants.PAGE_ODEMEPLANI + "?KiraSozlesmeId=" + kiraSozlesme.Id);
                }
                else
                {
                    MessageHelper.PublishMessage("OdemePlanı Bulunamadı", ProjeConstants.MESAJ_HATA);
                }
            }
            else
            {
                MessageHelper.PublishMessage("Kira Sözleşmesi Bulunamadı", ProjeConstants.MESAJ_HATA);
            }

        }
        protected void HepsiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KIRACI_AYLIKODEME + "?Bastar=" + BastarQS + "&Bittar=" + BittarQS);
        }
        private void BolgeDDLDoldur()
        {
            BolgeDDL.Items.Clear();
            Bolge bolgeDao = new Bolge();
            List<Bolge> list = new List<Bolge>();
            if (BolgeIdQS == ProjeConstants.BOLGE_HEPSI_INT || BolgeIdQS == ProjeConstants.BOLGE_GENELMUDURLUK_INT)
            {
                list = bolgeDao.SelectAktifBolgeler(ProjeConstants.BOLGE_HEPSI_INT);
                BolgeDDL.Items.Add(new System.Web.UI.WebControls.ListItem(ProjeConstants.BOLGE_HEPSI, ProjeConstants.BOLGE_HEPSI_INT.ToString()));

            }
            else
            {
                list = bolgeDao.SelectAktifBolgeler(BolgeIdQS);
            }
            foreach (Bolge item in list)
            {
                if (string.IsNullOrEmpty(item.Adi.Trim()))
                    continue;
                BolgeDDL.Items.Add(new System.Web.UI.WebControls.ListItem(item.Adi, item.Id.ToString()));
            }
            UtilityHelper.SetDDLValue(BolgeDDL, BolgeIdQS.ToString());

        }
        private void KiraciModalAc()
        {
            TabloModalOlustur();
            UtilityHelper.ScriptCalistir("OpenKiraciSecModal();");
        }
        private void TabloModalOlustur()
        {
            var jsonData = GetKiraciData(); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }

        private string CreateModalDataTable(string jsonData)
        {
            string tableString = @"
            if ( $.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
              $('#CustomModalDataTable').DataTable().destroy();
            }
            $('#CustomModalDataTable tbody').empty();

            jQuery('#CustomModalDataTable').DataTable({
            data: " + jsonData + @",
            'rowCallback': function(row, data, index) {
                if (data.Aktif != 1) {
                        $(row).addClass('table-danger');  // Bootstrap kırmızı tonu
                    } else {
                        $(row).addClass('table-success'); // Yeşil tonu
                    }
                },
            columnDefs:[
                {targets:5, render:function(data, type, row, meta){
                    var link='<a href=# onclick=CallButtonClick('+row.KiraciId + '); class=\'btn btn-outline-primary \'>Seç</a>';
    
                    return link;
                }}],   
            columns: [
                { data: 'KiraciId' },
                { data: 'Adi' },
                { data: 'TCKimlikNo' },
                { data: 'IlIlce' },
                { data: 'Adres' },
                { data: 'KiraciId' }
            ],
            'order': [[1, 'asc']],//AdiSoyadi Sıralı
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
            },
            responsive: true,
            dom: 'fpirt',

        });
            ";

            return tableString;
        }
        private string GetKiraciData()
        {
            Kiraci kiraci = new Kiraci();
            string json = kiraci.SelectAllReturnJson();

            return json;
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
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;

            GridView1.DataSource = GetDataList();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=AylikKiraOdemesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            Page.Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                //Apply text style to each Row
                GridView1.Rows[i].Attributes.Add("class", "textmode");
            }
            GridView1.RenderControl(hw);

            //style to format numbers to string
            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Page.Response.Write(style);
            Page.Response.Output.Write(sw.ToString());
            Page.Response.Flush();
            Page.Response.End();

        }
        protected void YeniOdemeGirisiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_ODEME_GIRIS + "?KiraciId=" + KiraciIdQS +"&Bastar="+BastarQS+"&Bittar="+BittarQS);
        }
        private string GetJsonData()
        {
            string jSon = "{[]}";
            try
            {
                List<OdemeListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                jSon = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return jSon;
        }
        private List<OdemeListItem> GetDataList()
        {
            List<OdemeListItem> list = new List<OdemeListItem>();

            try
            {
                DateTime basTarih = BaslangicTarihiTxt.Text.ConvertToDatetime().Date;
                DateTime bitTarih = BitisTarihiTxt.Text.ConvertToDatetime().Date;
                
                Odeme odemeDao = new Odeme();
                int bolgeId = BolgeDDL.SelectedItem.Value.ConvertToInt();

                DataTable dataTable = odemeDao.SelectByKiraciAyYilReturnDataTable(bolgeId, KiraciIdQS.ConvertToInt(), basTarih, bitTarih);
                

                if (dataTable != null)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string odemeId = row["OdemeId"].ToString();
                        string dosyaNo = row["DosyaNo"].ToString();
                        string ilkSozlesmeTar = row["IlkSozlesmeTar"].ConvertToDatetimeEmptyIfNull();
                        string odemePlaniId = row["OdemePlaniId"].ToString();
                        string sozlesmeId = row["SozlesmeId"].ToString();
                        string kiraciId = row["KiraciId"].ToString();
                        string teminatId = row["TeminatId"].ToString();

                        string adi = row["Adi"].ToString();
                        string soyadi = row["Soyadi"].ToString();

                        string bolge = row["Bolge"].ToString();
                        string kiralamaAmaci = row["KiralamaAmaci"].ToString();
                        
                        string odemeTarihi = row["OdemeTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy HH:mm");
                        string odenenTutar = row["OdenenTutar"].ConvertToDecimal().ToString("N", cultureInfo) + "TL";
                        string kiraBedeli = row["KiraBedeli"].ConvertToDecimal().ToString("N", cultureInfo) +"TL";
                        string artisAyi = row["ArtisAyi"].ToString();

                        string sozBasTar = row["SozBasTar"].ConvertToDatetimeEmptyIfNull();
                        string sozBitTar = row["SozBitTar"].ConvertToDatetimeEmptyIfNull();
                        string vadeBitTar = row["VadeBitTar"].ConvertToDatetimeEmptyIfNull();

                        string aciklama = row["Aciklama"].ToString();
                        string odemeSekli = row["OdemeSekli"].ToString();

                        OdemeListItem item = new OdemeListItem();
                        item.OdemeId = odemeId;
                        item.DosyaNo = dosyaNo;
                        item.IlkSozlesmeTar = ilkSozlesmeTar;
                        item.ArtisAyi = artisAyi;
                        item.OdemePlaniId = odemePlaniId;
                        item.SozlesmeId = sozlesmeId;
                        item.KiraciId = kiraciId;
                        item.KiraciAdiSoyadi = (adi + " " + soyadi).Trim();
                        item.TasinmazAdresi = TasinmazAdresGetir(item.SozlesmeId.ConvertToInt());
                        item.OdemeTarihi = odemeTarihi;
                        item.OdenenTutar = odenenTutar;
                        item.KiraBedeli = kiraBedeli;
                        item.Sozlesme = sozBasTar + " - " + sozBitTar;
                        item.VadeBitTar = vadeBitTar;
                        item.KiralamaAmaci = kiralamaAmaci;
                        item.Bolge = bolge;
                        item.KiralamaAmaci = kiralamaAmaci;
                        item.OdemeSekli = odemeSekli;
                        item.Aciklama = aciklama;
                        bool duzenleGorunsunMu = BolgeIdQS == ProjeConstants.HEPSI_INT || BolgeIdQS == ProjeConstants.BOLGE_GENELMUDURLUK_INT;
                        if (duzenleGorunsunMu)
                        {
                            item.KiraciAdiSoyadi = "<a href=" + ProjeConstants.PAGE_ODEMEPLANI + "?KiraSozlesmeId=" + sozlesmeId + "&Bastar=" + BastarQS + "&Bittar=" + BittarQS + " class='btn-link'>" + (adi + " " + soyadi).Trim() + "</a>";
                            item.Duzenle = "<a href=" + ProjeConstants.PAGE_ODEME_GIRIS + "?OdemeId=" + odemeId + "&Bastar=" + BastarQS + "&Bittar=" + BittarQS+ " class='btn btn-outline-primary'>Düzenle</a>";
                            if (teminatId.ConvertToInt() > 0)
                            {
                                item.Duzenle = "<a href=" + ProjeConstants.PAGE_TEMINAT_ISLEMLERI + "?KiraSozlesmeId=" + sozlesmeId + "&Bastar=" + BastarQS + "&Bittar=" + BittarQS + " class='btn btn-outline-secondary'>Teminat</a>";
                            }
                        }
                        else
                        {
                            item.Duzenle =string.Empty;
                        }
                        item.Sil = "<a href=" + ProjeConstants.PAGE_ODEME_GIRIS + "?OdemeId=" + odemeId + "&Bastar=" + BastarQS + "&Bittar=" + BittarQS + " class='btn btn-outline-danger'>Sil</a>";
                        list.Add(item);
                    }
                }
            }
            catch (Exception exception)
            {

                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return list;
        }

        private string TasinmazAdresGetir(int sozlesmeId)
        {
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            DataTable dataTable = kiraSozlesme.SelectBySozlesmeId(sozlesmeId);
            string adres = string.Empty;
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string ilceIl = row["Ilcesi"].ToString() + " " + row["Ili"].ToString();
                    adres = row["Adres"].ToString() + " " + ilceIl;
                }
            }
            return adres;
        }
        private class OdemeListItem
        {
            public string OdemeId { get; set; }
            public string DosyaNo { get; set; }
            public string OdemePlaniId { get; set; }
            public string SozlesmeId { get; set; }
            public string KiraciId { get; set; }
            public string KiraciAdiSoyadi { get; set; }
            public string TasinmazAdresi { get; set; }
            public string KiraBedeli { get; set; }
            public string OdenenTutar { get; set; }
            public string OdemeTarihi { get; set; }
            public string IlkSozlesmeTar { get; set; }
            public string ArtisAyi { get; set; }
            public string Sozlesme { get; set; }
            public string VadeBitTar { get; set; }
            public string KiralamaAmaci { get; set; }
            public string Bolge { get; set; }
            public string OdemeSekli { get; set; }
            public string Aciklama { get; set; }
            public string Duzenle { get; set; }
            public string Sil { get; set; }

        }
    }

}
