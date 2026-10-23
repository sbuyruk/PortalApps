using MTS_WebParts.Logging;
using Utility.Logging;
using Model.MTS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Model.Ortak;
using Model.Services.MTS;

namespace MTS_WebParts.FaaliyetListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class FaaliyetListesiWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("MTS", "FaaliyetListesiWP")); } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FaaliyetListesiWP()
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
        protected override void OnPreRender(EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "LoadList", TabloOlustur);
            if (MtsWebPartLog.HasFailed(LogContext))
                UtilityHelper.ScriptCalistir("jQuery('#CustomDataTable').hide();");
            base.OnPreRender(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) MtsWebPartLog.RecordOpened(LogContext);
                if (!Page.IsPostBack)
                {
                    BaslangicTarihiTxt.Text = DateTime.Today.AddYears(-5).ConvertToDatetimeEmptyIfNull();
                    BitisTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                }
            });
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "setDataSet(" + jsonData + ");", true);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            List<FaaliyetListItem> list = GetDataList();
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            jSon = serializer.Serialize(list);

            return jSon.Replace("<", "\\u003c").Replace(">", "\\u003e").Replace("&", "\\u0026");
        }
        private List<FaaliyetListItem> GetDataList()
        {
            DateTime bastar, bittar;
            MtsWebPartLog.ReadDateRange(BaslangicTarihiTxt.Text, BitisTarihiTxt.Text, out bastar, out bittar);
            List<FaaliyetListItem> faaliyetList = new List<FaaliyetListItem>();
            FaaliyetService faaliyetService = new FaaliyetService(LogContext);
            string acikTarihli = AcikTarhliChk.Checked ? ProjeConstants.HEPSI : ProjeConstants.FAALIYET_ACIKTARIHLI_DEGIL;
            DataTable dataTable = faaliyetService.GetParticipantList(ProjeConstants.HEPSI_INT, ProjeConstants.HEPSI_INT, acikTarihli, bastar, bittar, ProjeConstants.HEPSI);

            if (dataTable != null)
            {
                int tempFaaliyetId = 0;
                int katilimciAdedi = 0;

                FaaliyetListItem tempFaaliyetListItem = new FaaliyetListItem();
                foreach (DataRow row in dataTable.Rows)
                {
                    int faaliyetId = row["FaaliyetId"].ConvertToInt();
                    string faaliyetTipi = row["FaaliyetTipi"].ToString();
                    string faaliyetYeri = row["FaaliyetYeri"].ToString(); ;
                    string faaliyetKonusu = row["FaaliyetKonusu"].ToString();
                    string faaliyetAmaci = row["FaaliyetAmaciId"].ToString();
                    string faaliyetDurumu = row["FaaliyetDurumu"].ToString();

                    DateTime basTar = row["BaslangicTarihi"].ConvertToDatetime();
                    DateTime bitTar = row["BitisTarihi"].ConvertToDatetime();

                    string baslangicTarihi = basTar.ToString("dd.MM.yyyy HH:mm");
                    string bitisTarihi = bitTar.ToString("dd.MM.yyyy HH:mm");

                    string faaliyetAmaciStr = MTSOrtak.ParseFaaliyetAmaci(faaliyetAmaci);
                    string faaliyetDurumuStr = MTSOrtak.ParseFaaliyetDurumu(faaliyetDurumu.ConvertToInt());
                    string katilimci = row["Adi"].ToString() + " " + row["Soyadi"].ToString();

                    if (tempFaaliyetId == faaliyetId)
                    {
                        tempFaaliyetId = faaliyetId;
                        faaliyetList.Remove(tempFaaliyetListItem);

                        //tempFaaliyetListItem.Katilimci += "@" + adiSoyadi;
                        katilimciAdedi++;
                        tempFaaliyetListItem.Katilimci += katilimci + "; ";
                        faaliyetList.Add(tempFaaliyetListItem);
                    }
                    if (tempFaaliyetId != faaliyetId)
                    {
                        FaaliyetListItem faaliyetListItem = new FaaliyetListItem();
                        faaliyetListItem.FaaliyetId = faaliyetId;
                        faaliyetListItem.FaaliyetTipi = faaliyetTipi;
                        faaliyetListItem.FaaliyetYeri = faaliyetYeri;
                        faaliyetListItem.FaaliyetKonusu = faaliyetKonusu;
                        faaliyetListItem.BaslangicTarihi = baslangicTarihi;
                        faaliyetListItem.BitisTarihi = bitisTarihi;
                        faaliyetListItem.FaaliyetAmaci = faaliyetAmaciStr;
                        faaliyetListItem.FaaliyetDurumu = faaliyetDurumuStr;

                        faaliyetListItem.Katilimci += katilimci + "; ";
                        faaliyetListItem.Duzenle = "<a href=" + ProjeConstants.PAGE_FAALIYET_GIRIS + "?FaaliyetId=" + faaliyetId + " class='btn btn-outline-primary'>Düzenle</a>";
                        faaliyetListItem.Secildi = SecilenIdQS.Equals(faaliyetListItem.FaaliyetId.ToString());
                        tempFaaliyetListItem = faaliyetListItem;
                        faaliyetList.Add(tempFaaliyetListItem);
                    }
                    tempFaaliyetId = faaliyetId;

                }
            }
            //faaliyetList = faaliyetList.OrderBy(r => r.FaaliyetTarihi).ToList();
            return faaliyetList;
        }
        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl);

        }

        protected void YeniKayitBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "YeniKayitBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FAALIYET_GIRIS);
            });
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "ExcelBtn_Click", () =>
            {
                ExportToExcel();
            });
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
             "attachment;filename=FaaliyetListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
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
            MtsWebPartLog.ExecuteAction(LogContext, "CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            });
        }
        protected void KisiListesiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "KisiListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_KISI_LIST);
            });
        }
        protected void FaaliyetListesiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FAALIYET_LIST);
            });
        }
        protected void AcikTarihliFaaliyetListesiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "AcikTarihliFaaliyetListesiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_ACIKTARIHLIFAALIYET_LIST);
            });
        }
        protected void FaaliyetTakvimiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetTakvimiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM);
            });
        }
        protected void BaslangicTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "BaslangicTarihiTxt_TextChanged", () =>
            {

            });
        }

        protected void BitisTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "BitisTarihiTxt_TextChanged", () =>
            {

            });
        }
        protected void AcikTarhliChk_CheckedChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "AcikTarhliChk_CheckedChanged", () =>
            {

            });
        }
        private class FaaliyetListItem
        {
            public int FaaliyetId { get; set; }
            public string FaaliyetTipi { get; set; }
            public string FaaliyetYeri { get; set; }
            public string FaaliyetKonusu { get; set; }
            public string FaaliyetAmaci { get; set; }
            public string FaaliyetDurumu { get; set; }
            public string BaslangicTarihi { get; set; }
            public string BitisTarihi { get; set; }
            public string Katilimci { get; set; }
            //public string Arama { get; set; }
            public string Duzenle { get; set; }
            public bool Secildi { get; set; }
        }


    }
}
