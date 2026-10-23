using MTS_WebParts.Logging;
using Utility.Logging;
using Model.MTS;
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
using Model.Ortak;

namespace MTS_WebParts.KisiListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KisiListesiWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("MTS", "KisiListesiWP")); } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KisiListesiWP()
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
            });
        }
        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl);

        }

        private void TabloOlustur()
        {
            List<Kisi> list = new List<Kisi>();
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            //var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler,
                typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(),"setDataSet("+ jsonData +");", true);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            List<KisiListItem> list = GetDataList();
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            jSon = serializer.Serialize(list);

            return jSon.Replace("<", "\\u003c").Replace(">", "\\u003e").Replace("&", "\\u0026");
        }
        private List<KisiListItem> GetDataList()
        {

            Kisi kisi = new Kisi();

            System.Data.DataTable dataTable = new Model.Services.MTS.KisiService(LogContext).GetAllData();
            int SiraNo = 1;

            List<KisiListItem> list = new List<KisiListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            if (dataTable == null) return list;
            foreach (DataRow row in dataTable.Rows)
            {
                string kisiId = row["Id"].ToString();
                string adi = row["Adi"].ToString();
                string soyadi = row["Soyadi"].ToString();
                string tCKimlikNo = row["TCKimlikNo"].ToString();
                string mtsKurumTanim = row["MTSKurumTanim"].ReturnEmptyIfNull().ToString();
                string mtsGorevTanim = row["MTSGorevTanim"].ReturnEmptyIfNull().ToString();
                string mtsUnvanTanim = row["MTSUnvanTanim"].ReturnEmptyIfNull().ToString();
                string kurumu = row["Kurumu"].ToString();
                string unvani = row["Unvani"].ToString();
                string gorevi = row["Gorevi"].ToString();


                string dahili1 = row["Dahili1"].ToString();
                string dahili2 = row["Dahili2"].ToString();
                string dahili3 = row["Dahili3"].ToString();

                string telAciklama1 = row["TelAciklama1"].ToString();
                string telAiklama2 = row["TelAciklama2"].ToString();
                string aciklama3 = row["TelAciklama3"].ToString();
                string ili = row["IlAdi"].ToString();
                string ilcesi = row["IlceAdi"].ToString();
                string adres = row["Adres"].ToString();
                string aciklama = row["Aciklama"].ToString();


                KisiListItem kisiItem = new KisiListItem();
                kisiItem.Sirano = SiraNo++.ToString();
                kisiItem.KisiId = kisiId;
                kisiItem.AdiSoyadi = (adi + " " + soyadi).Trim();
                kisiItem.TCKimlikNo = tCKimlikNo;
                kisiItem.Kurumu = string.IsNullOrEmpty(mtsKurumTanim) ?kurumu: mtsKurumTanim;
                kisiItem.Unvani = string.IsNullOrEmpty(mtsUnvanTanim) ? unvani : mtsUnvanTanim;
                kisiItem.Gorevi = string.IsNullOrEmpty(mtsGorevTanim) ? gorevi : mtsGorevTanim; ;
                kisiItem.Telefon1 = FormatPhone(Convert.ToString(row["Telefon1"]), dahili1);
                kisiItem.Telefon2 = FormatPhone(Convert.ToString(row["Telefon2"]), dahili2);
                kisiItem.Telefon3 = FormatPhone(Convert.ToString(row["Telefon3"]), dahili3);
                kisiItem.TelAciklama1 = telAciklama1;
                kisiItem.TelAciklama2 = telAiklama2;
                kisiItem.TelAciklama3 = aciklama3;
                kisiItem.Ili = ili;
                kisiItem.Ilcesi = ilcesi;
                kisiItem.Adres = adres;
                kisiItem.Aciklama = aciklama;
                kisiItem.Arama = "<a href=" + ProjeConstants.PAGE_ARAMAGORUSME_GIRIS + "?ArayanId=" + kisiId + " class='btn btn-outline-success'>Yeni Ara./Gör. Ekle</a>";
                kisiItem.KisiKarti = "<a  target='_blank' href=" + ProjeConstants.PAGE_KISI_KARTI + "?KatilimciId=" + kisiId + " class='btn btn-outline-info'>Kişi Kartı</a>";
                kisiItem.Duzenle = "<a href=" + ProjeConstants.PAGE_KISI_GIRIS + "?KisiId=" + kisiId + " class='btn btn-outline-primary'>Düzenle</a>";
                kisiItem.Secildi = SecilenIdQS.Equals(kisiItem.KisiId);
                list.Add(kisiItem);
            }
            return list;
        }


        protected void YeniKayitBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "YeniKayitBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_KISI_GIRIS);
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
             "attachment;filename=KisiListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
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
        protected void RandevuTakvimiBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "RandevuTakvimiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM);
            });
        }
        private static string FormatPhone(string value, string extension)
        {
            string phone = (value ?? string.Empty).Trim();
            if (phone.Length == 0 || phone == "0") return string.Empty;
            string digits = phone.Replace("(", "").Replace(")", "").Replace(" ", "");
            long number;
            if (digits.Length == 10 && long.TryParse(digits, out number))
                phone = string.Format("{0:(###) ### ####}", number);
            return phone + (string.IsNullOrWhiteSpace(extension) ? string.Empty : " /" + extension.Trim());
        }

        private class KisiListItem
        {
            public string Sirano { get; set; }
            public string KisiId { get; set; }
            public string AdiSoyadi { get; set; }
            public string TCKimlikNo { get; set; }

            public int MTSUnvanTanimId { get; set; }
            public string Kurumu { get; set; }
            public string Unvani { get; set; }
            public string Gorevi { get; set; }
            public string Telefon1 { get; set; }
            public string TelAciklama1 { get; set; }
            public string Telefon2 { get; set; }
            public string TelAciklama2 { get; set; }
            public string Telefon3 { get; set; }
            public string TelAciklama3 { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Adres { get; set; }
            public string Aciklama { get; set; }
            public string Arama { get; set; }
            public string KisiKarti { get; set; }
            public string Duzenle { get; set; }
            public bool Secildi { get; set; }
        }
    }
}
