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

namespace MTS_WebParts.KisiListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KisiListesiWP : WebPart
    {
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
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                TabloOlustur();
            }
            catch (Exception ex)
            {
                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
            }
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
        
        private void TabloOlustur()
        {
            List<Kisi> list = new List<Kisi>();
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            //var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, 
                typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(),"setDataSet("+ jsonData +");", true);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<KisiListItem> list = GetDataList();
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
        private List<KisiListItem> GetDataList()
        {

            Kisi kisi = new Kisi();

            System.Data.DataTable dataTable = kisi.SelectAllReturnDT();
            int SiraNo = 1;

            List<KisiListItem> list = new List<KisiListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

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

                long telefon1 = row["Telefon1"].ReturnEmptyIfZeroOrNull().ToString().Replace("(", "").Replace(")", "").Replace(" ", "").ConvertToLong();
                long telefon2 = row["Telefon2"].ReturnEmptyIfZeroOrNull().ToString().Replace("(", "").Replace(")", "").Replace(" ", "").ConvertToLong();
                long telefon3 = row["Telefon3"].ReturnEmptyIfZeroOrNull().ToString().Replace("(", "").Replace(")", "").Replace(" ", "").ConvertToLong();

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
                kisiItem.Telefon1 = telefon1 < 1 ?"": String.Format("{0:(###) ### ####}", telefon1)
                    + (string.IsNullOrEmpty(dahili1) ? "" : " /" + dahili1.Trim());
                kisiItem.Telefon2 = telefon2 < 1 ? "" : String.Format("{0:(###) ### ####}", telefon2)
                    + (string.IsNullOrEmpty(dahili2) ? "" : " /" + dahili2.Trim());
                kisiItem.Telefon3 = telefon3 < 1 ? "" : String.Format("{0:(###) ### ####}", telefon3)
                    + (string.IsNullOrEmpty(dahili3) ? "" : " /" + dahili3.Trim());
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
            RedirectToPage(ProjeConstants.PAGE_KISI_GIRIS);
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
        protected void RandevuTakvimiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM);
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
