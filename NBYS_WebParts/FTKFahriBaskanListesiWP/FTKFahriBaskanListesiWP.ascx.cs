using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.FTKFahriBaskanListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class FTKFahriBaskanListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FTKFahriBaskanListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
                List<UyeListItem> list = GetDataList(true);
                var serializer = new JavaScriptSerializer();
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
        protected void YeniKayitBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FTKKISI_GIRISI);
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
        private List<UyeListItem> GetDataList(bool duzenleVarmi)
        {
            int SiraNo = 1;
            List<UyeListItem> list = new List<UyeListItem>();
            FTKKisi ftkKisi = new FTKKisi();
            DataTable dataTable = ftkKisi.SelectAllReturnDT();
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    bool vali = row["Vali"].ReturnFalseIfNull().ConvertToBool();
                    bool kaymakam = row["Kaymakam"].ReturnFalseIfNull().ConvertToBool();
                    if (vali || kaymakam)//Vali ve kaymakamları bu listede gösterme
                    {
                        string kisiId = row["Id"].ToString();
                        string adi = row["Adi"].ToString();
                        string soyadi = row["Soyadi"].ToString();
                        string unvani = row["Unvani"].ToString();
                        string telefon1 = row["Telefon1"].ToString();
                        string ili = row["IlAdi"].ToString();
                        string ilcesi = row["IlceAdi"].ToString();


                        string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                        string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));

                        string pageUrl = newUrl + "/" + ProjeConstants.PAGE_FTKKISI_GIRISI;

                        UyeListItem uyeItem = new UyeListItem();
                        uyeItem.Sirano = SiraNo++.ToString();
                        uyeItem.AdiSoyadi = (adi + " " + soyadi).Trim();
                        uyeItem.Unvani = unvani;
                        uyeItem.Vali = vali;
                        uyeItem.Vali = kaymakam;
                        uyeItem.Telefon1 = telefon1;
                        uyeItem.Ili = ili.ToString();
                        uyeItem.Ilcesi = ilcesi.ToString();
                        string unvanStr = vali ? "&Vali=" + vali : (kaymakam ? "&Kaymakam=" + kaymakam : string.Empty);
                        if (duzenleVarmi)
                        {
                            uyeItem.Duzenle = "<a href=" + pageUrl + "?FTKKisiId=" + kisiId + unvanStr + " class='btn btn-outline-primary'>Düzenle</a>";
                        }


                        list.Add(uyeItem);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return list;
        }
        private class UyeListItem
        {
            public string Sirano { get; set; }
            public string AdiSoyadi { get; set; }
            public string Unvani { get; set; }
            public string Telefon1 { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public bool Vali { get; set; }
            public bool Kaymakam { get; set; }
            public string Duzenle { get; set; }

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

            GridView1.DataSource = GetDataList(false);
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=FTKKisiListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
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

    }
}
