using Model.IKYS;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace BTYS_Webparts.BolgeTasinmazListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BolgeTasinmazListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BolgeTasinmazListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                BolgeQS=IKYSOrtak.PersonelinBolgesiniGetir(CurrentUserName);
                if (!string.IsNullOrEmpty(BolgeQS))
                {
                    TabloOlustur();
                }
                else 
                    MessageHelper.PublishMessage("Bölgeniz Belirlenemedi", ProjeConstants.MESAJ_HATA);
            }
            catch (Exception ex)
            {
                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
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
                List<TasinmazListesiListItem> list = GetDataList();
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
        private List<TasinmazListesiListItem> GetDataList()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            DataTable dataTable = tasinmaz.SelectByBolgeReturnJson(BolgeQS);

            List<TasinmazListesiListItem> list = new List<TasinmazListesiListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            foreach (DataRow row in dataTable.Rows)
            {
                string tasinmazId = row["Id"].ToString();
                string kullanimSekli = row["KullanimSekli"].ToString();
                string mulkiyetSekli = row["MulkiyetSekli"].ToString();
                string iliIlcesi = row["IliIlcesi"].ToString();
                string adres = row["Adres"].ToString();
                string bagisci = row["Bagisci"].ToString();
                string bagisYili = row["BagisYili"].ToString();

                string kullanimDurumu = row["KullanimDurumu"].ToString();
                string sorumluBolge = row["SorumluBolge"].ToString();
                string emlakSicilNo = row["EmlakSicilNo"].ToString();

                string adaNo = row["AdaNo"].ToString();
                string parselNo = row["ParselNo"].ToString();
                string paftaNo = row["PaftaNo"].ToString();
                string yevmiyeNo = row["YevmiyeNo"].ToString();
                string ciltNo = row["CiltNo"].ToString();
                string sahifeNo = row["SahifeNo"].ToString();
                string cinsi = row["Cinsi"].ToString();


                TasinmazListesiListItem tasinmazListesiListItem = new TasinmazListesiListItem();
                tasinmazListesiListItem.Id = tasinmazId;
                tasinmazListesiListItem.KullanimSekli = kullanimSekli;
                tasinmazListesiListItem.MulkiyetSekli = mulkiyetSekli;
                tasinmazListesiListItem.IliIlcesi = iliIlcesi;
                tasinmazListesiListItem.Adres = adres;
                tasinmazListesiListItem.Bagisci = bagisci;
                tasinmazListesiListItem.BagisYili = bagisYili.Trim();

                tasinmazListesiListItem.TasinmazKarti = "<a target='_blank' href=" + ProjeConstants.PAGE_BOLGETASINMAZ_KARTI + "?TasinmazId=" + tasinmazId + " class='btn btn-outline-info'>Taşınmaz Kartı</a>";

                tasinmazListesiListItem.KullanimDurumu = kullanimDurumu;
                tasinmazListesiListItem.SorumluBolge = sorumluBolge;
                tasinmazListesiListItem.EmlakSicilNo = emlakSicilNo;
                tasinmazListesiListItem.AdaNo = adaNo;
                tasinmazListesiListItem.ParselNo = parselNo;
                tasinmazListesiListItem.PaftaNo = paftaNo;
                tasinmazListesiListItem.YevmiyeNo = yevmiyeNo;
                tasinmazListesiListItem.CiltNo = ciltNo;
                tasinmazListesiListItem.SahifeNo = sahifeNo;
                tasinmazListesiListItem.Cinsi = cinsi;



                list.Add(tasinmazListesiListItem);
            }
            return list;
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
        private DataTable GetDataListDT()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            DataTable dataTable = tasinmaz.SelectAllReturnDataTable();
            return dataTable;
        }
        protected void ExportToExcel()
        {
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;

            GridView1.DataSource = GetDataListDT();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=TasinmazListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
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
        private class TasinmazListesiListItem
        {
            public string Id { get; set; }
            public string KullanimSekli { get; set; }
            public string MulkiyetSekli { get; set; }
            public string IliIlcesi { get; set; }
            public string Adres { get; set; }
            public string Bagisci { get; set; }
            public string BagisYili { get; set; }
            public string TasinmazKarti { get; set; }
            public string SorumluBolge { get; set; }
            public string EmlakSicilNo { get; set; }
            public string AdaNo { get; set; }
            public string ParselNo { get; set; }
            public string PaftaNo { get; set; }
            public string YevmiyeNo { get; set; }
            public string CiltNo { get; set; }
            public string SahifeNo { get; set; }
            public string Cinsi { get; set; }
            public string KullanimDurumu { get; set; }
        }
    }
}
