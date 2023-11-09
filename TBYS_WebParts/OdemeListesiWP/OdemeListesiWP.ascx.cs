using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.OdemeListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class OdemeListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public OdemeListesiWP()
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
                    if (Page.Request.QueryString["SecilenYil"] != null && Page.Request.QueryString["SecilenYil"].ToString().ConvertToInt() > 0)
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
                DDLListeleriDoldur();
                SetDDLValues();
            }
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select<Kiraci>(KiraciIdQS.ConvertToInt());
            if (kiraci != null || string.IsNullOrEmpty(KiraciIdQS) || KiraciIdQS.Equals(ProjeConstants.HEPSI_INT.ToString()))
            {
                TabloOlustur();
            }
            else
            {
                MessageHelper.PublishMessage("Kiracı Bulunamadı", ProjeConstants.MESAJ_HATA);
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
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                    typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "setDataSet(" + jsonData + ");", true);
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

            KayitGetir();
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
        private void KayitGetir()
        {
            var jsonData = GetKiraciData(); //veri çekilip json a çeviriliyor
            var jsString = CreateJsString(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
            var openPopup = "OpenKiraciSecModal();";
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
        }
        private string GetKiraciData()
        {
            Kiraci kiraci = new Kiraci();
            string json = kiraci.SelectAllReturnJson();

            return json;
        }
        private string CreateJsString(string jsonData)
        {
            string ekstretablestr = @"   
                                        $('#tblfilter').puidatatable({
                                        caption: '',
                                        editMode: 'cell',
                                        paginator: {
                                                    rows: 8
                                                    },
                                        columns: [
                                            { field: 'DosyaNo', headerText: 'Dosya',filter: true, sortable:true,headerStyle:'width: 5%' },
                                            { field: 'Adi', headerText: 'Adi', sortable:true,filter: true,headerStyle:'width: 25%'},
                                            { field: 'TCKimlikNo', headerText: 'TCKimlikNo', sortable:true,filter: true,headerStyle:'width: 10%'},
                                            { field: 'IlIlce', headerText: 'İl/İlçe', sortable:true,filter: true,headerStyle:'width: 15%'},
                                            { field: 'Adres', headerText: 'Adres', sortable:true,filter: true, headerStyle:'width: 35%'},
                                            { field: 'KiraciId',headerStyle:'width: 10%', content: function (rowData)
                                    	        { 
                                                    return $('<a href=# onclick=CallButtonClick('+rowData.KiraciId+'); class=\'btn btn-outline-primary \'>SEÇ</a>')
                                    	        }
                                            }
                                                ],
                                       datasource:" + jsonData + @",
                                       resizableColumns: true,
                                       globalFilter:'#globalFilter'
                                       });
                                    ";
            return ekstretablestr;
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
            RedirectToPage(ProjeConstants.PAGE_ODEME_GIRIS + "?KiraciId=" + KiraciIdQS + "&SecilenAy=" + SecilenAyQS + "&SecilenYil=" + SecilenYilQS);
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
                DataTable dataTable = odemeDao.SelectByAyYilReturnDataTable(ay, yil);

                if (dataTable != null)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        //string teminatId = row["TeminatId"].ToString();
                        //string islemTipi = row["IslemTipi"].ToString();

                        string adi = row["Adi"].ToString();
                        string soyadi = row["Soyadi"].ToString();

                        string kiralamaAmaci = row["KiralamaAmaci"].ToString();

                        string odemeTarihi = row["OdemeTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy HH:mm");
                        string odenenTutar = row["OdenenTutar"].ConvertToDecimal().ToString("N", cultureInfo);

                        string aciklama = row["Aciklama"].ToString();

                        OdemeListItem item = new OdemeListItem();
                        item.KiraciAdiSoyadi = (adi + " " + soyadi).Trim() ;
                        item.OdemeTarihi = odemeTarihi;
                        item.OdenenTutar = odenenTutar;
                        item.Aciklama = aciklama;
                        //if (teminatId.ConvertToInt() > 0)
                        //{
                        //    item.IslemTipi = islemTipi;
                        //}
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

        private class OdemeListItem
        {
            public string KiraciAdiSoyadi { get; set; }
            public string OdemeTarihi { get; set; }
            public string OdenenTutar { get; set; }
            public string OdemeSebebi { get; set; }
            public string IslemTipi { get; set; }
            public string Aciklama { get; set; }

        }
    }

}
