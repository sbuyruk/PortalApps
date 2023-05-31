using Model.Ortak;
using Model.TBYS;
using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.BorcluKiraciTakibiByBolgeWP
{
    [ToolboxItemAttribute(false)]
    public partial class BorcluKiraciTakibiByBolgeWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BorcluKiraciTakibiByBolgeWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SecilenAyQS
        {
            get
            {

                if (ViewState["SecilenAy"] == null)
                {
                    if (Page.Request.QueryString["SecilenAy"] != null)
                    {
                        ViewState["SecilenAy"] = Page.Request.QueryString["SecilenAy"];
                    }
                    else
                    {
                        ViewState["SecilenAy"] = string.Empty;
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
                    if (Page.Request.QueryString["SecilenYil"] != null)
                    {
                        ViewState["SecilenYil"] = Page.Request.QueryString["SecilenYil"];
                    }
                    else
                    {
                        ViewState["SecilenYil"] = string.Empty;
                    }
                }
                return ViewState["SecilenYil"].ToString();
            }

            set
            {
                ViewState["SecilenYil"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    AyDDLDoldur();
                    YilDDLDoldur();
                    SetDDLValues();
                }

                TabloyuDoldur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.PublishException();
            }
        }
        protected void TabloyuDoldur()
        {

            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            GenelBaslikCell.Text = AyDDL.SelectedItem.Text + " " + YilDDL.SelectedItem.Text + " İtibarı İle  Yapılan Takip İşlemleri";

            KiraBorcuTakip kiraBorcuTakip = new KiraBorcuTakip();

            //GM 
            int GMUyariAdet = kiraBorcuTakip.SelectCountAdetByTakipIslemiBolge(ProjeConstants.KIRABORCU_UYARI, ProjeConstants.BOLGE_GENELMUDURLUK, AyDDL.SelectedItem.Value.ConvertToInt(), YilDDL.SelectedItem.Value.ConvertToInt());
            int GMYaziliIhtarAdet = kiraBorcuTakip.SelectCountAdetByTakipIslemiBolge(ProjeConstants.KIRABORCU_YAZILIIHTAR, ProjeConstants.BOLGE_GENELMUDURLUK, AyDDL.SelectedItem.Value.ConvertToInt(), YilDDL.SelectedItem.Value.ConvertToInt());
            int GMIcraTakibiAdet = kiraBorcuTakip.SelectCountAdetByTakipIslemiBolge(ProjeConstants.KIRABORCU_ICRATAKIBI, ProjeConstants.BOLGE_GENELMUDURLUK, AyDDL.SelectedItem.Value.ConvertToInt(), YilDDL.SelectedItem.Value.ConvertToInt());
            int GMToplamAdet = GMUyariAdet + GMYaziliIhtarAdet + GMIcraTakibiAdet;
            GMUyariCell.Text = GMUyariAdet.ToString();
            GMYaziliIhtarCell.Text = GMYaziliIhtarAdet.ToString();
            GMIcraTakibiCell.Text = GMIcraTakibiAdet.ToString();
            GMTopCell.Text = GMToplamAdet.ToString();
            //İst 
            int IstUyariAdet = kiraBorcuTakip.SelectCountAdetByTakipIslemiBolge(ProjeConstants.KIRABORCU_UYARI, ProjeConstants.BOLGE_ISTANBUL, AyDDL.SelectedItem.Value.ConvertToInt(), YilDDL.SelectedItem.Value.ConvertToInt());
            int IstYaziliIhtarAdet = kiraBorcuTakip.SelectCountAdetByTakipIslemiBolge(ProjeConstants.KIRABORCU_YAZILIIHTAR, ProjeConstants.BOLGE_ISTANBUL, AyDDL.SelectedItem.Value.ConvertToInt(), YilDDL.SelectedItem.Value.ConvertToInt());
            int IstIcraTakibiAdet = kiraBorcuTakip.SelectCountAdetByTakipIslemiBolge(ProjeConstants.KIRABORCU_ICRATAKIBI, ProjeConstants.BOLGE_ISTANBUL, AyDDL.SelectedItem.Value.ConvertToInt(), YilDDL.SelectedItem.Value.ConvertToInt());
            int IstToplamAdet = IstUyariAdet + IstYaziliIhtarAdet + IstIcraTakibiAdet;
            IstUyariCell.Text = IstUyariAdet.ToString();
            IstYaziliIhtarCell.Text = IstYaziliIhtarAdet.ToString();
            IstIcraTakibiCell.Text = IstIcraTakibiAdet.ToString();
            IstTopCell.Text = IstToplamAdet.ToString();

            //İzm 
            int IzmUyariAdet = kiraBorcuTakip.SelectCountAdetByTakipIslemiBolge(ProjeConstants.KIRABORCU_UYARI, ProjeConstants.BOLGE_IZMIR, AyDDL.SelectedItem.Value.ConvertToInt(), YilDDL.SelectedItem.Value.ConvertToInt());
            int IzmYaziliIhtarAdet = kiraBorcuTakip.SelectCountAdetByTakipIslemiBolge(ProjeConstants.KIRABORCU_YAZILIIHTAR, ProjeConstants.BOLGE_IZMIR, AyDDL.SelectedItem.Value.ConvertToInt(), YilDDL.SelectedItem.Value.ConvertToInt());
            int IzmIcraTakibiAdet = kiraBorcuTakip.SelectCountAdetByTakipIslemiBolge(ProjeConstants.KIRABORCU_ICRATAKIBI, ProjeConstants.BOLGE_IZMIR, AyDDL.SelectedItem.Value.ConvertToInt(), YilDDL.SelectedItem.Value.ConvertToInt());
            int IzmToplamAdet = IzmUyariAdet + IzmYaziliIhtarAdet + IzmIcraTakibiAdet;
            IzmUyariCell.Text = IzmUyariAdet.ToString();
            IzmYaziliIhtarCell.Text = IzmYaziliIhtarAdet.ToString();
            IzmIcraTakibiCell.Text = IzmIcraTakibiAdet.ToString();
            IzmTopCell.Text = IzmToplamAdet.ToString();

            //Mer 
            int MerUyariAdet = kiraBorcuTakip.SelectCountAdetByTakipIslemiBolge(ProjeConstants.KIRABORCU_UYARI, ProjeConstants.BOLGE_IZMIR, AyDDL.SelectedItem.Value.ConvertToInt(), YilDDL.SelectedItem.Value.ConvertToInt());
            int MerYaziliIhtarAdet = kiraBorcuTakip.SelectCountAdetByTakipIslemiBolge(ProjeConstants.KIRABORCU_YAZILIIHTAR, ProjeConstants.BOLGE_IZMIR, AyDDL.SelectedItem.Value.ConvertToInt(), YilDDL.SelectedItem.Value.ConvertToInt());
            int MerIcraTakibiAdet = kiraBorcuTakip.SelectCountAdetByTakipIslemiBolge(ProjeConstants.KIRABORCU_ICRATAKIBI, ProjeConstants.BOLGE_IZMIR, AyDDL.SelectedItem.Value.ConvertToInt(), YilDDL.SelectedItem.Value.ConvertToInt());
            int MerToplamAdet = MerUyariAdet + MerYaziliIhtarAdet + MerIcraTakibiAdet;
            MerUyariCell.Text = MerUyariAdet.ToString();
            MerYaziliIhtarCell.Text = MerYaziliIhtarAdet.ToString();
            MerIcraTakibiCell.Text = MerIcraTakibiAdet.ToString();
            MerTopCell.Text = MerToplamAdet.ToString();

            //Top
            int TopUyariAdet = GMUyariAdet + IstUyariAdet + IzmUyariAdet + MerUyariAdet;
            int TopYaziliIhtarAdet = GMYaziliIhtarAdet + IstYaziliIhtarAdet + IzmYaziliIhtarAdet + MerYaziliIhtarAdet;
            int TopIcraTakibiAdet = GMIcraTakibiAdet + IstIcraTakibiAdet + IzmIcraTakibiAdet + MerIcraTakibiAdet;
            int GenToplamAdet = TopUyariAdet + TopYaziliIhtarAdet + TopIcraTakibiAdet;

            TopUyariCell.Text = TopUyariAdet.ToString();
            TopYaziliIhtarCell.Text = TopYaziliIhtarAdet.ToString();
            TopIcraTakibiCell.Text = TopIcraTakibiAdet.ToString();
            GenTopCell.Text = GenToplamAdet.ToString();

            //HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACITAKIPISLEMLERI, TopUyariCell, TopUyariAdet.ToString(), ProjeConstants.BOLGE_HEPSI);
            //HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACITAKIPISLEMLERI, GenTopCell, GenToplamAdet.ToString(), ProjeConstants.BOLGE_HEPSI);
            //HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACITAKIPISLEMLERI, GenTopCell, GenToplamAdet.ToString(), ProjeConstants.BOLGE_HEPSI);
            //HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACITAKIPISLEMLERI, GenTopCell, GenToplamAdet.ToString(), ProjeConstants.BOLGE_HEPSI);
            //HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACITAKIPISLEMLERI, GenTopCell, GenToplamAdet.ToString(), ProjeConstants.BOLGE_HEPSI);
        }

        private void HyperLinkEkle(string page, TableCell cell, string value, string bolge)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            HyperLink cellLnk = new HyperLink();
            cellLnk.Text = value;
            if (value.ConvertToInt() > 0)
            {
                string linkUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + page + "?Auth=IEYS&Bolge=" + bolge + "&SecilenAy=" + SecilenAyQS + "&SecilenYil=" + SecilenYilQS;
                cellLnk.NavigateUrl = linkUrl;
            }
            cell.Controls.Add(cellLnk);
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
            int baslamaYili = 2023;//year - 7;
            //YilDDL.Items.Add(new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI_INT.ToString()));
            for (int i = baslamaYili; i <= year; i++)
            {
                YilDDL.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }
        protected void ExportToExcel()
        {
            string filename = "BorcluKiracilarRaporu" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            //Türkçe sorunu yok
            Page.Response.Clear();
            Page.Response.AddHeader("content-disposition", "attachment;filename=" + filename + "");
            Page.Response.ContentType = "application/ms-excel";
            Page.Response.ContentEncoding = System.Text.Encoding.Unicode;
            Page.Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(sw);

            BorcluKiracilarTable.RenderControl(hw);

            Page.Response.Write(sw.ToString());
            Page.Response.End();
        }

        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenAyQS = AyDDL.SelectedItem.Value.ToString();
            RedirectToPage(ProjeConstants.PAGE_BORCLUKIRACITAKIBI + "?SecilenAy=" + SecilenAyQS + "&SecilenYil=" + SecilenYilQS);
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
            RedirectToPage(ProjeConstants.PAGE_BORCLUKIRACITAKIBI + "?SecilenAy=" + SecilenAyQS + "&SecilenYil=" + SecilenYilQS);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
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
            //string filename = "BorcluKiracilarRaporu" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            //Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            //Page.Response.Charset = "windows-1254";//ISO-8859-9
            //System.IO.StringWriter tw = new System.IO.StringWriter();
            //System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            ////Get the HTML for the control.             
            //BorcluKiracilarTable.RenderControl(hw);
            ////Write the HTML back to the browser.
            ////Response.ContentType = application/vnd.ms-excel;
            //Page.Response.ContentType = "application/vnd.ms-excel";
            //Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            //this.EnableViewState = false;
            //Page.Response.Write(tw.ToString());
            //Page.Response.End();
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
    }
}

