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
        private string SecilenAyQS
        {
            get
            {

                if (ViewState["SecilenAy"] == null)
                {
                    if (Page.Request.QueryString["SecilenAy"] != null && Page.Request.QueryString["SecilenAy"].ToString().ConvertToInt() > 0)
                    {
                        ViewState["SecilenAy"] = Page.Request.QueryString["SecilenAy"];
                    }
                    else
                    {
                        ViewState["SecilenAy"] = DateTime.Today.Month.ToString();
                    }
                }
                return ViewState["SecilenAy"].ToString();
            }

            set
            {
                ViewState["SecilenAy"] = value;
            }
        }
        private string SecilenYilQS
        {
            get
            {

                if (ViewState["SecilenYil"] == null)
                {
                    if (Page.Request.QueryString["SecilenYil"] != null && Page.Request.QueryString["SecilenYil"].ToString().ConvertToInt()>0)
                    {
                        ViewState["SecilenYil"] = Page.Request.QueryString["SecilenYil"];
                    }
                    else
                    {
                        ViewState["SecilenYil"] = DateTime.Today.Year.ToString();
                    }
                }
                return ViewState["SecilenYil"].ToString();
            }

            set
            {
                ViewState["SecilenYil"] = value;
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
            Bolge bolge = IKYSOrtak.BolgeGetirByUserName(CurrentUserName);
            BolgeIdQS = bolge == null ? 0 : bolge.Id;
            if (!Page.IsPostBack)
            {
                DDLListeleriDoldur();
                SetDDLValues();
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
        private void DDLListeleriDoldur()
        {
            //KiraciDDLDoldur();
            AyDDLDoldur();
            YilDDLDoldur();
        }
        private void SetDDLValues()
        {
            try
            {
                //ay
                string ay = !string.IsNullOrEmpty(SecilenAyQS) ? SecilenAyQS : DateTime.Today.Month.ReturnEmptyIfNull().ToString();
                ListItem AyItem = new ListItem();
                if (!string.IsNullOrEmpty(ay))
                    AyItem = AyDDL.Items.FindByValue(ay);

                if (AyItem != null)
                {
                    AyDDL.SelectedValue = AyItem.Value;
                    SecilenAyQS = AyItem.Value;
                }
                //yil
                string yil = !string.IsNullOrEmpty(SecilenYilQS) ? SecilenYilQS : DateTime.Today.Year.ReturnEmptyIfNull().ToString();
                ListItem YilItem = new ListItem();
                if (!string.IsNullOrEmpty(yil))
                    YilItem = YilDDL.Items.FindByValue(yil);

                if (YilItem != null)
                {
                    YilDDL.SelectedValue = YilItem.Value;
                    SecilenYilQS = YilItem.Value;
                }
            }
            catch (Exception)
            {

                //TODO
            }
        }
        private void AyDDLDoldur()
        {
            //AyDDL.Items.Add(new ListItem("Hepsi", "0"));
            AyDDL.Items.Add(new ListItem("Ocak", "1"));
            AyDDL.Items.Add(new ListItem("Şubat", "2"));
            AyDDL.Items.Add(new ListItem("Mart", "3"));
            AyDDL.Items.Add(new ListItem("Nisan", "4"));
            AyDDL.Items.Add(new ListItem("Mayıs", "5"));
            AyDDL.Items.Add(new ListItem("Haziran", "6"));
            AyDDL.Items.Add(new ListItem("Temmuz", "7"));
            AyDDL.Items.Add(new ListItem("Ağustos", "8"));
            AyDDL.Items.Add(new ListItem("Eylül", "9"));
            AyDDL.Items.Add(new ListItem("Ekim", "10"));
            AyDDL.Items.Add(new ListItem("Kasım", "11"));
            AyDDL.Items.Add(new ListItem("Aralık", "12"));

        }
        private void YilDDLDoldur()
        {
            var year = DateTime.Now.Year;
            //YilDDL.Items.Add(new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI_INT.ToString()));
            for (int i = year; i >= 2005; i--)
            {
                YilDDL.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenAyQS = AyDDL.SelectedItem.Value.ToString();
            //OdemelerTablosunuDoldur();
            TabloOlustur();
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
            //OdemelerTablosunuDoldur();
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
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
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
                    RedirectToPage(ProjeConstants.PAGE_KIRACI_AYLIKODEME + "?KiraciId=" + KiraciIdQS + "&SecilenYil=" + SecilenYilQS + "&SecilenAy=" + SecilenAyQS);
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

            RedirectToPage(ProjeConstants.PAGE_KIRACI_AYLIKODEME + "?KiraciId=" + paramKiraciIdLbl.Value.ConvertToInt() + "&SecilenAy=" + SecilenAyQS + "&SecilenYil=" + SecilenYilQS);
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
            RedirectToPage(ProjeConstants.PAGE_KIRACI_AYLIKODEME + "?SecilenAy=" + SecilenAyQS + "&SecilenYil=" + SecilenYilQS);
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
            RedirectToPage(ProjeConstants.PAGE_ODEME_GIRIS + "?KiraciId=" + KiraciIdQS +"&SecilenAy="+SecilenAyQS+"&SecilenYil="+SecilenYilQS);
        }
        private string GetJsonData()
        {
            string jSon = "{[]}";
            try
            {
                List<OdemeListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
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
                int ay = SecilenAyQS.ConvertToInt();
                int yil = SecilenYilQS.ConvertToInt();
                Odeme odemeDao = new Odeme();
                DataTable dataTable = odemeDao.SelectByKiraciAyYilReturnDataTable(BolgeIdQS, KiraciIdQS.ConvertToInt(), ay, yil);
                

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
                            item.KiraciAdiSoyadi = "<a href=" + ProjeConstants.PAGE_ODEMEPLANI + "?KiraSozlesmeId=" + sozlesmeId + " class='btn-link'>" + (adi + " " + soyadi).Trim() + "</a>";
                            item.Duzenle = "<a href=" + ProjeConstants.PAGE_ODEME_GIRIS + "?OdemeId=" + odemeId + " class='btn btn-outline-primary'>Düzenle</a>";
                            if (teminatId.ConvertToInt() > 0)
                            {
                                item.Duzenle = "<a href=" + ProjeConstants.PAGE_TEMINAT_ISLEMLERI + "?KiraSozlesmeId=" + sozlesmeId + " class='btn btn-outline-secondary'>Teminat</a>";
                            }
                        }
                        else
                        {
                            item.Duzenle =string.Empty;
                        }
                        item.Sil = "<a href=" + ProjeConstants.PAGE_ODEME_GIRIS + "?OdemeId=" + odemeId + " class='btn btn-outline-danger'>Sil</a>";
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
