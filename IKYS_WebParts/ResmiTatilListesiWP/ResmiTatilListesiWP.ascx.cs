using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.ResmiTatilListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class ResmiTatilListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ResmiTatilListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string MesajQS
        {
            get
            {

                if (ViewState["Mesaj"] == null)
                {
                    if (Page.Request.QueryString["Mesaj"] != null)
                    {
                        ViewState["Mesaj"] = Page.Request.QueryString["Mesaj"];
                    }
                    else
                    {
                        ViewState["Mesaj"] = string.Empty;
                    }
                }
                return ViewState["Mesaj"].ToString();
            }

            set
            {
                ViewState["Mesaj"] = value;
            }
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
                    if (!string.IsNullOrEmpty(MesajQS))
                    {
                        MessageHelper.PublishMessage("Resmi Tatil Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                        MesajQS = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private DataTable GetData()
        {
            ResmiTatil resmiTatil = new ResmiTatil();
            DataTable dataTable = resmiTatil.SelectAllReturnDataTable();
            return dataTable;
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
                List<ResmiTatilItemList> list = GetDataList();
                var serializer = new JavaScriptSerializer
                {
                    MaxJsonLength = Int32.MaxValue
                };
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
        private List<ResmiTatilItemList> GetDataList()
        {
            DataTable dataTable = GetData();
            List<ResmiTatilItemList> returnlist = new List<ResmiTatilItemList>();

            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    ResmiTatilItemList resmiTatilListItem = new ResmiTatilItemList();

                    resmiTatilListItem.Tatil = dataRow["Tatil"].ToString();
                    resmiTatilListItem.ResimiTatilId = dataRow["ResimiTatilId"].ToString();
                    
                    DateTime baslamaTarihi= dataRow["BaslamaTarihi"].ConvertToDatetime();
                    string baslamaTarihiStr = baslamaTarihi.Year < 1900 ? baslamaTarihi.ToString("dd.MM.")+ "YYYY": baslamaTarihi.ConvertToDatetimeEmptyIfNull();
                    string baslamaSaatiStr= baslamaTarihi.ToString("HH:mm").Equals("00:00")?string.Empty: baslamaTarihi.ToString("HH:mm");

                    string siralamaTarihiStr = new DateTime(baslamaTarihi.Year < 1900 ? DateTime.Today.Year : baslamaTarihi.Year, baslamaTarihi.Month, baslamaTarihi.Day).ConvertToDatetimeEmptyIfNull();
                    resmiTatilListItem.SiralamaTarihi = siralamaTarihiStr;

                    DateTime bitisTarihi= dataRow["BitisTarihi"].ConvertToDatetime();
                    string bitisTarihiStr = bitisTarihi.Year < 1900 ? bitisTarihi.ToString("dd.MM.")+ "YYYY" : bitisTarihi.ConvertToDatetimeEmptyIfNull();
                    string bitisSaatiStr = bitisTarihi.ToString("HH:mm").Equals("00:00") ? string.Empty : bitisTarihi.ToString("HH:mm");

                    resmiTatilListItem.BaslamaTarihi = baslamaTarihiStr;
                    resmiTatilListItem.BaslamaSaati = baslamaSaatiStr;
                    resmiTatilListItem.BitisTarihi = bitisTarihiStr;
                    resmiTatilListItem.BitisSaati = bitisSaatiStr;

                    resmiTatilListItem.IlanTarihi= dataRow["IlanTarihi"].ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                    resmiTatilListItem.IptalTarihi= dataRow["IptalTarihi"].ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                    string yilStr = dataRow["Yil"].ToString();
                    resmiTatilListItem.Yil = yilStr.Equals("0")?"Sürekli":yilStr;

                    resmiTatilListItem.Duzenle = string.Empty;

                    string duzenleLinkStr = "<a href=" + ProjeConstants.PAGE_RESMITATIL_GIRIS + "?SenderApp=RTL&DestinationApp=RTD&ResmiTatilId=" + 
                        resmiTatilListItem.ResimiTatilId + " class='btn btn-outline-primary' >Düzenle</a>";
                    resmiTatilListItem.Duzenle = duzenleLinkStr;


                    resmiTatilListItem.Secildi = SecilenIdQS.Equals(resmiTatilListItem.ResimiTatilId);
                    returnlist.Add(resmiTatilListItem);
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
        private class ResmiTatilItemList
        {
            public string Tatil { get; set; }
            public string ResimiTatilId { get; set; }
            public string SiralamaTarihi { get; set; }
            public string BaslamaTarihi { get; set; }
            public string BaslamaSaati { get; set; }
            public string BitisTarihi { get; set; }
            public string BitisSaati { get; set; }
            public string IlanTarihi { get; set; }
            public string IptalTarihi { get; set; }
            public string Yil { get; set; }
            public string Duzenle { get; set; }
            public bool Secildi { get; set; }
        }
    }
}
