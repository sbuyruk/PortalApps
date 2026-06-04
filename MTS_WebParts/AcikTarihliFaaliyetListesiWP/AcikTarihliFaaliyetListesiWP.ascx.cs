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

namespace MTS_WebParts.AcikTarihliFaaliyetListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class AcikTarihliFaaliyetListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public AcikTarihliFaaliyetListesiWP()
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
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            UtilityHelper.ScriptCalistir( "setDataSet(" + jsonData + ");");
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<FaaliyetListItem> list = GetDataList();
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
        private List<FaaliyetListItem> GetDataList()
        {
            string faaliyetAmaciIdStr = string.Empty;
            if (ToplantiChk.Checked)
            {
                faaliyetAmaciIdStr = string.IsNullOrEmpty(faaliyetAmaciIdStr) ? ProjeConstants.FAALIYET_AMACI_TOPLANTI_INT: ProjeConstants.FAALIYET_AMACI_TOPLANTI_INT.ToString();
            }
            if (ZiyaretChk.Checked)
            {
                faaliyetAmaciIdStr += string.IsNullOrEmpty(faaliyetAmaciIdStr) ? ProjeConstants.FAALIYET_AMACI_ZIYARET_INT : ","+ ProjeConstants.FAALIYET_AMACI_ZIYARET_INT.ToString();
            }
            if (GorusmeChk.Checked)
            {
                faaliyetAmaciIdStr += string.IsNullOrEmpty(faaliyetAmaciIdStr) ? ProjeConstants.FAALIYET_AMACI_GORUSME_INT : "," + ProjeConstants.FAALIYET_AMACI_GORUSME_INT.ToString();
            }
            if (SeyahatChk.Checked)
            {
                faaliyetAmaciIdStr += string.IsNullOrEmpty(faaliyetAmaciIdStr) ? ProjeConstants.FAALIYET_AMACI_SEYAHAT_INT : "," + ProjeConstants.FAALIYET_AMACI_SEYAHAT_INT.ToString();
            }
            if (DavetChk.Checked)
            {
                faaliyetAmaciIdStr += string.IsNullOrEmpty(faaliyetAmaciIdStr) ? ProjeConstants.FAALIYET_AMACI_DAVET_INT : "," + ProjeConstants.FAALIYET_AMACI_DAVET_INT.ToString();
            }

            faaliyetAmaciIdStr =string.IsNullOrEmpty(faaliyetAmaciIdStr)?string.Empty:"("+ faaliyetAmaciIdStr +")";
            List<FaaliyetListItem> faaliyetList = new List<FaaliyetListItem>();
            Faaliyet faaliyetDao = new Faaliyet();
            DataTable dataTable = faaliyetDao.SelectAllByKatilimciFaaliyetReturnDataTable(ProjeConstants.HEPSI_INT, ProjeConstants.HEPSI_INT, ProjeConstants.FAALIYET_ACIKTARIHLI,ProjeConstants.NULL_TARIH,ProjeConstants.NULL_TARIH, faaliyetAmaciIdStr);

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
                    string aciklama = row["Aciklama"].ToString();
                    string olusturmaTarihi = row["OlusturmaTarihi"].ReturnEmptyIfNull().ConvertToDatetime().ToString("dd.MM.yyyy HH:mm");

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
                        faaliyetListItem.FaaliyetAmaci = faaliyetAmaciStr;
                        faaliyetListItem.FaaliyetDurumu = faaliyetDurumuStr;
                        faaliyetListItem.Aciklama = aciklama;
                        faaliyetListItem.OlusturmaTarihi = olusturmaTarihi;

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
       
        protected void YeniKayitBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_GIRIS);
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
        protected void KisiListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KISI_LIST);
        }
        protected void FaaliyetListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_LIST);
        }
        protected void AcikTarihliFaaliyetListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_ACIKTARIHLIFAALIYET_LIST);
        }
        protected void FaaliyetTakvimiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM);
        }
        private class FaaliyetListItem
        {
            public int FaaliyetId { get; set; }
            public string FaaliyetTipi { get; set; }
            public string FaaliyetYeri { get; set; }
            public string FaaliyetKonusu { get; set; }
            public string FaaliyetAmaci { get; set; }
            public string FaaliyetDurumu { get; set; }
            public string Katilimci { get; set; }
            public string Aciklama { get; set; }
            public string OlusturmaTarihi { get; set; }
            public string Duzenle { get; set; }
            public bool Secildi { get; set; }
        }
        protected void ToplantiChk_CheckedChanged(object sender, EventArgs e)
        {
            TabloOlustur();

        }
        protected void ZiyaretChk_CheckedChanged(object sender, EventArgs e)
        {
            TabloOlustur();

        }
        protected void GorusmeChk_CheckedChanged(object sender, EventArgs e)
        {
            TabloOlustur();

        }
        protected void SeyahatChk_CheckedChanged(object sender, EventArgs e)
        {
            TabloOlustur();

        }
        protected void DavetChk_CheckedChanged(object sender, EventArgs e)
        {
            TabloOlustur();

        }
    }
}