using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.IzinHareketListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class IzinHareketListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public IzinHareketListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SecilenIdQS
        {
            get
            {

                if (ViewState["SecilenId"] == null)
                {
                    if (Page.Request.QueryString["SecilenId"] != null)
                    {
                        ViewState["SecilenId"] = Page.Request.QueryString["SecilenId"];
                    }
                    else
                    {
                        ViewState["SecilenId"] = string.Empty;
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
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
            UtilityHelper.ScriptCalistir("setDataSet(" + jsonData + ");");
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<IzinHareketItemList> list = GetDataList();
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
        private List<IzinHareketItemList> GetDataList()
        {
            IzinHareket izinHareket = new IzinHareket();
            DataTable dataTable = izinHareket.SelectByIzinDonemiReturnDataTable(0, 0, 0);
            List<IzinHareketItemList> returnlist = new List<IzinHareketItemList>();

            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    IzinHareketItemList izinHareketListItem = new IzinHareketItemList();

                    izinHareketListItem.AdiSoyadi = dataRow["AdiSoyadi"].ToString();
                    izinHareketListItem.PersonelId = dataRow["PersonelId"].ToString();
                    izinHareketListItem.IzinTipiId = dataRow["IzinTipiId"].ToString();
                    izinHareketListItem.BaslangicTarihi = dataRow["BaslangicTarihi"].ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                    izinHareketListItem.BaslangicSaati = string.Empty;
                    izinHareketListItem.BitisTarihi = dataRow["BitisTarihi"].ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                    izinHareketListItem.BitisSaati = dataRow["BitisTarihi"].ConvertToDatetime().ToString("HH:mm");

                    izinHareketListItem.DuzenleLink = "";
                    izinHareketListItem.IzinDonemiId = dataRow["IzinDonemId"].ToString();
                    izinHareketListItem.IzinDonemi = dataRow["IzinDonemi"].ToString();
                    izinHareketListItem.IzinHareketId = dataRow["IzinHareketId"].ToString();
                    izinHareketListItem.IzinTalepId = dataRow["IzinTalepId"].ToString();
                    izinHareketListItem.IzinTipi = dataRow["IzinTipi"].ToString();
                    
                    izinHareketListItem.YazdirLink = string.Empty;
                    izinHareketListItem.DuzenleLink = string.Empty;

                    string duzenleLinkStr = "<a href="+ProjeConstants.PAGE_IZINHAREKET_EDIT+ "?AUTH=IKYS&IzinHareketId=" + izinHareketListItem.IzinHareketId + 
                        "&PersonelId=" + izinHareketListItem.PersonelId +
                        "&IzinTanimId=" + izinHareketListItem.IzinTipiId + " class='btn btn-outline-primary' >Düzenle</a>";

                    izinHareketListItem.DuzenleLink = duzenleLinkStr;
                    if (izinHareketListItem.IzinTipi.Equals(ProjeConstants.IZINTIPI_MAZERET)
                        || izinHareketListItem.IzinTipi.Equals(ProjeConstants.IZINTIPI_SUTIZNI))
                    {
                        izinHareketListItem.BaslangicTarihi = dataRow["BaslangicTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy");
                        izinHareketListItem.BaslangicSaati = dataRow["BaslangicTarihi"].ConvertToDatetime().ToString("HH:mm");
                        izinHareketListItem.BitisTarihi = dataRow["BitisTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy");
                        izinHareketListItem.BitisSaati = dataRow["BitisTarihi"].ConvertToDatetime().ToString("HH:mm");
                        izinHareketListItem.SureStr = dataRow["Sure"].ToString().ConvertToTimeSpanReturnInHHmm();

                        izinHareketListItem.OncekiIzinStr = dataRow["OncekiIzinStr"].ToString().ConvertToTimeSpanReturnInHHmm().Replace(" ", "%20")  ;
                        izinHareketListItem.KullanilanIzinStr = dataRow["KullanilanIzinStr"].ToString().ConvertToTimeSpanReturnInHHmm().Replace(" ", "%20");
                        izinHareketListItem.KalanIzinStr = dataRow["KalanIzinStr"].ToString().ConvertToTimeSpanReturnInHHmm().Replace(" ", "%20")  ;

                        string belge = izinHareketListItem.IzinTipi.Equals(ProjeConstants.IZINTIPI_MAZERET) ? ProjeConstants.RAPOR_MAZERETIZINBELGESI : string.Empty;
                        bool linkEmpty = string.IsNullOrEmpty(belge) ||
                            string.IsNullOrEmpty(izinHareketListItem.OncekiIzinStr) ||
                            string.IsNullOrEmpty(izinHareketListItem.KullanilanIzinStr) ||
                            string.IsNullOrEmpty(izinHareketListItem.KalanIzinStr);

                        string yazdirLinkStr = linkEmpty ? "" : "<a href=" + belge + "?IzinTalepId=" + izinHareketListItem.IzinTalepId +
                            "&KalanIzinStr=" + izinHareketListItem.OncekiIzinStr + 
                            "&SureStr=" + izinHareketListItem.KullanilanIzinStr + 
                            "&SonIzin=" + izinHareketListItem.KalanIzinStr + " class='btn btn-outline-info' target='_blank'>Yazdir</a>";
                        izinHareketListItem.YazdirLink = yazdirLinkStr;

                        
                    }
                    else if (izinHareketListItem.IzinTipi.Equals(ProjeConstants.IZINTIPI_UCRETLI))
                    {
                        izinHareketListItem.SureStr = dataRow["SureBirim"].ToString();
                        izinHareketListItem.OncekiIzinStr = dataRow["OncekiIzinStr"].ToString().ConvertToDatetimeEmptyIfNull();
                        izinHareketListItem.KullanilanIzinStr = dataRow["KullanilanIzinStr"].ToString().ConvertToDatetimeEmptyIfNull();

                        string belge = izinHareketListItem.IzinTipi.Equals(ProjeConstants.IZINTIPI_UCRETLI) ? ProjeConstants.RAPOR_UCRETLIIZINBELGESI_URL : string.Empty;
                        bool linkEmpty = string.IsNullOrEmpty(belge);
                        string yazdirLinkStr = linkEmpty ? "" : "<a href=" + belge + "?IzinTalepId=" + izinHareketListItem.IzinTalepId +
                            "&IzinDonemId=" + izinHareketListItem.IzinDonemiId  + " class='btn btn-outline-info' target='_blank'>Yazdir</a>";
                        izinHareketListItem.YazdirLink = yazdirLinkStr;

                    }
                    //üstünden N gün geçtiyse yazdir linki olmasin
                    int gunFarki = (DateTime.Today - izinHareketListItem.BitisTarihi.ConvertToDatetime()).Days;
                    if (gunFarki > ProjeConstants.IZINDUZENLEMESURESI_GUN)
                    {
                        izinHareketListItem.YazdirLink = string.Empty;
                         duzenleLinkStr = "<a href=" + ProjeConstants.PAGE_IZINHAREKET_EDIT + "?AUTH=IKYS&IzinHareketId=" + izinHareketListItem.IzinHareketId +
                        "&PersonelId=" + izinHareketListItem.PersonelId +
                        "&IzinTanimId=" + izinHareketListItem.IzinTipiId + " class='btn btn-outline-warning' >Görüntüle</a>";

                        izinHareketListItem.DuzenleLink = duzenleLinkStr;
                    }
                    izinHareketListItem.Secildi = SecilenIdQS.Equals(izinHareketListItem.IzinHareketId);
                    returnlist.Add(izinHareketListItem);
                }
            }
            return returnlist;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        private class IzinHareketItemList
        {
            public string AdiSoyadi { get; set; }
            public string PersonelId { get; set; }
            public string IzinTipiId { get; set; }
            public string IzinDonemiId { get; set; }
            public string IzinDonemi { get; set; }
            public string IzinHareketId { get; set; }
            public string IzinTalepId { get; set; }
            public string IzinTipi { get; set; }
            public string BaslangicTarihi { get; set; }
            public string BaslangicSaati { get; set; }
            public string BitisTarihi { get; set; }
            public string BitisSaati { get; set; }
            public string OncekiIzinStr { get; set; }
            public string SureStr { get; set; }
            public string KullanilanIzinStr { get; set; }
            public string KalanIzinStr { get; set; }
            public string DuzenleLink { get; set; }
            public string YazdirLink { get; set; }
            public bool Secildi { get; set; }
        }
    }
}
