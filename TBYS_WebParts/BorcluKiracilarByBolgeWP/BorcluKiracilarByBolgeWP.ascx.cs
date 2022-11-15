using Model.Ortak;
using Model.TBYS;
using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.BorcluKiracilarByBolgeWP
{
    [ToolboxItemAttribute(false)]
    public partial class BorcluKiracilarByBolgeWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BorcluKiracilarByBolgeWP()
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
            int sonsuzAyBorcluOlanlar = 99999999;
            int birAyBorcluOlanlar = 1;
            int ikiAyBorcluOlanlar = 2;
            int ucAyBorcluOlanlar = 3;
            int dortAyBorcluOlanlar = 4;


            int ay = SecilenAyQS.ConvertToInt();
            int yil = SecilenYilQS.ConvertToInt();
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            KiraSozlesme kiraSozlesme = new KiraSozlesme();

            DataTable gmDataTable = kiraSozlesme.SelectKiraciSayisiByBolgeTarih(ProjeConstants.BOLGE_GENELMUDURLUK, ay, yil);
            DataTable istDataTable = kiraSozlesme.SelectKiraciSayisiByBolgeTarih(ProjeConstants.BOLGE_ISTANBUL, ay, yil);
            DataTable izmDataTable = kiraSozlesme.SelectKiraciSayisiByBolgeTarih(ProjeConstants.BOLGE_IZMIR, ay, yil);
            DataTable merDataTable = kiraSozlesme.SelectKiraciSayisiByBolgeTarih(ProjeConstants.BOLGE_MERSIN, ay, yil);

            int adetGM = gmDataTable == null ? 0 : gmDataTable.Rows.Count;
            int adetIst = gmDataTable == null ? 0 : istDataTable.Rows.Count;
            int adetIzm = gmDataTable == null ? 0 : izmDataTable.Rows.Count;
            int adetMer = gmDataTable == null ? 0 : merDataTable.Rows.Count;

            int adetGT = adetGM + adetIst + adetIzm + adetMer;

            GMKiraciSayisiCell.Text = adetGM.ToString();
            IstKiraciSayisiCell.Text = adetIst.ToString();
            IzmKiraciSayisiCell.Text = adetIzm.ToString();
            MerKiraciSayisiCell.Text = adetMer.ToString();
            TopKiraciSayisiCell.Text = adetGT.ToString();

            //Kiracı sayıları ve linkleri
            HyperLinkEkle(ProjeConstants.PAGE_MEVCUTKIRACI_LIST, GMKiraciSayisiCell, adetGM.ToString(), ProjeConstants.BOLGE_GENELMUDURLUK, ProjeConstants.HEPSI_INT, ProjeConstants.HEPSI_INT);
            HyperLinkEkle(ProjeConstants.PAGE_MEVCUTKIRACI_LIST, IstKiraciSayisiCell, adetIst.ToString(), ProjeConstants.BOLGE_ISTANBUL, ProjeConstants.HEPSI_INT, ProjeConstants.HEPSI_INT);
            HyperLinkEkle(ProjeConstants.PAGE_MEVCUTKIRACI_LIST, IzmKiraciSayisiCell, adetIzm.ToString(), ProjeConstants.BOLGE_IZMIR, ProjeConstants.HEPSI_INT, ProjeConstants.HEPSI_INT);
            HyperLinkEkle(ProjeConstants.PAGE_MEVCUTKIRACI_LIST, MerKiraciSayisiCell, adetMer.ToString(), ProjeConstants.BOLGE_MERSIN, ProjeConstants.HEPSI_INT, ProjeConstants.HEPSI_INT);
            HyperLinkEkle(ProjeConstants.PAGE_MEVCUTKIRACI_LIST, TopKiraciSayisiCell, adetGT.ToString(), ProjeConstants.BOLGE_HEPSI, ProjeConstants.HEPSI_INT, ProjeConstants.HEPSI_INT);

            DateTime secilenTarih = new DateTime(yil, ay, 1);
            #region içinde bulunduğumuz ay için bugünü esas alsın
            //DateTime buAyIlkGun = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            //if (secilenTarih == buAyIlkGun)
            //{
            //    secilenTarih = DateTime.Today.AddMonths(-1);
            //}
            #endregion
            DateTime vadeBastar = secilenTarih;
            DateTime vadeBittar = secilenTarih.AddMonths(1).AddDays(-1);

            DateTime birAyOnceIlkGun = vadeBastar.AddMonths(-1);
            DateTime birAyOnceSonGun = birAyOnceIlkGun.AddMonths(1).AddDays(-1);

            DateTime ikiAyOnceIlkGun = birAyOnceIlkGun.AddMonths(-1);
            DateTime ikiAyOnceSonGun = ikiAyOnceIlkGun.AddMonths(1).AddDays(-1);

            DateTime ucAyOnceIlkGun = ikiAyOnceIlkGun.AddMonths(-1);
            DateTime ucAyOnceSonGun = ucAyOnceIlkGun.AddMonths(1).AddDays(-1);

            DateTime dortAyOnceIlkGun = ucAyOnceIlkGun.AddMonths(-1);
            DateTime dortAyOnceSonGun = dortAyOnceIlkGun.AddMonths(1).AddDays(-1);

            OdemePlani odemePlaniDao = new OdemePlani();

            //GM 
            DataTable dortAyBorcluGMDataTable = odemePlaniDao.SelectBorcluOdemePlanlariByBolgeTarih(ProjeConstants.BOLGE_GENELMUDURLUK, vadeBastar, vadeBittar, dortAyBorcluOlanlar, sonsuzAyBorcluOlanlar);
            int dortAyBorcluGM = dortAyBorcluGMDataTable == null ? 0 : dortAyBorcluGMDataTable.Rows.Count;

            DataTable ucAyBorcluGMDataTable = odemePlaniDao.SelectBorcluOdemePlanlariByBolgeTarih(ProjeConstants.BOLGE_GENELMUDURLUK, vadeBastar, vadeBittar, ucAyBorcluOlanlar, ucAyBorcluOlanlar);
            int ucAyBorcluGM = ucAyBorcluGMDataTable == null ? 0 : ucAyBorcluGMDataTable.Rows.Count;

            DataTable ikiAyBorcluGMDataTable = odemePlaniDao.SelectBorcluOdemePlanlariByBolgeTarih(ProjeConstants.BOLGE_GENELMUDURLUK, vadeBastar, vadeBittar, ikiAyBorcluOlanlar, ikiAyBorcluOlanlar);
            int ikiAyBorcluGM = ikiAyBorcluGMDataTable == null ? 0 : ikiAyBorcluGMDataTable.Rows.Count;

            DataTable birAyBorcluGMDataTable = odemePlaniDao.SelectBorcluOdemePlanlariByBolgeTarih(ProjeConstants.BOLGE_GENELMUDURLUK, vadeBastar, vadeBittar, birAyBorcluOlanlar, birAyBorcluOlanlar);
            int birAyBorcluGM = birAyBorcluGMDataTable == null ? 0 : birAyBorcluGMDataTable.Rows.Count;

            //İst 
            DataTable dortAyBorcluIstDataTable = odemePlaniDao.SelectBorcluOdemePlanlariByBolgeTarih(ProjeConstants.BOLGE_ISTANBUL, vadeBastar, vadeBittar, dortAyBorcluOlanlar, sonsuzAyBorcluOlanlar);
            int dortAyBorcluIst = dortAyBorcluIstDataTable == null ? 0 : dortAyBorcluIstDataTable.Rows.Count;

            DataTable ucAyBorcluIstDataTable = odemePlaniDao.SelectBorcluOdemePlanlariByBolgeTarih(ProjeConstants.BOLGE_ISTANBUL, vadeBastar, vadeBittar, ucAyBorcluOlanlar, ucAyBorcluOlanlar);
            int ucAyBorcluIst = ucAyBorcluIstDataTable == null ? 0 : ucAyBorcluIstDataTable.Rows.Count;

            DataTable ikiAyBorcluIstDataTable = odemePlaniDao.SelectBorcluOdemePlanlariByBolgeTarih(ProjeConstants.BOLGE_ISTANBUL, vadeBastar, vadeBittar, ikiAyBorcluOlanlar, ikiAyBorcluOlanlar);
            int ikiAyBorcluIst = ikiAyBorcluIstDataTable == null ? 0 : ikiAyBorcluIstDataTable.Rows.Count;

            DataTable birAyBorcluIstDataTable = odemePlaniDao.SelectBorcluOdemePlanlariByBolgeTarih(ProjeConstants.BOLGE_ISTANBUL, vadeBastar, vadeBittar, birAyBorcluOlanlar, birAyBorcluOlanlar);
            int birAyBorcluIst = birAyBorcluIstDataTable == null ? 0 : birAyBorcluIstDataTable.Rows.Count;


            //İzm 
            DataTable dortAyBorcluIzmDataTable = odemePlaniDao.SelectBorcluOdemePlanlariByBolgeTarih(ProjeConstants.BOLGE_IZMIR, vadeBastar, vadeBittar, dortAyBorcluOlanlar, sonsuzAyBorcluOlanlar);
            int dortAyBorcluIzm = dortAyBorcluIzmDataTable == null ? 0 : dortAyBorcluIzmDataTable.Rows.Count;

            DataTable ucAyBorcluIzmDataTable = odemePlaniDao.SelectBorcluOdemePlanlariByBolgeTarih(ProjeConstants.BOLGE_IZMIR, vadeBastar, vadeBittar, ucAyBorcluOlanlar, ucAyBorcluOlanlar);
            int ucAyBorcluIzm = ucAyBorcluIzmDataTable == null ? 0 : ucAyBorcluIzmDataTable.Rows.Count;

            DataTable ikiAyBorcluIzmDataTable = odemePlaniDao.SelectBorcluOdemePlanlariByBolgeTarih(ProjeConstants.BOLGE_IZMIR, vadeBastar, vadeBittar, ikiAyBorcluOlanlar, ikiAyBorcluOlanlar);
            int ikiAyBorcluIzm = ikiAyBorcluIzmDataTable == null ? 0 : ikiAyBorcluIzmDataTable.Rows.Count;

            DataTable birAyBorcluIzmDataTable = odemePlaniDao.SelectBorcluOdemePlanlariByBolgeTarih(ProjeConstants.BOLGE_IZMIR, vadeBastar, vadeBittar, birAyBorcluOlanlar, birAyBorcluOlanlar);
            int birAyBorcluIzm = birAyBorcluIzmDataTable == null ? 0 : birAyBorcluIzmDataTable.Rows.Count;


            //Mer 
            DataTable dortAyBorcluMerDataTable = odemePlaniDao.SelectBorcluOdemePlanlariByBolgeTarih(ProjeConstants.BOLGE_MERSIN, vadeBastar, vadeBittar, dortAyBorcluOlanlar, sonsuzAyBorcluOlanlar);
            int dortAyBorcluMer = dortAyBorcluMerDataTable == null ? 0 : dortAyBorcluMerDataTable.Rows.Count;

            DataTable ucAyBorcluMerDataTable = odemePlaniDao.SelectBorcluOdemePlanlariByBolgeTarih(ProjeConstants.BOLGE_MERSIN, vadeBastar, vadeBittar, ucAyBorcluOlanlar, ucAyBorcluOlanlar);
            int ucAyBorcluMer = ucAyBorcluMerDataTable == null ? 0 : ucAyBorcluMerDataTable.Rows.Count;

            DataTable ikiAyBorcluMerDataTable = odemePlaniDao.SelectBorcluOdemePlanlariByBolgeTarih(ProjeConstants.BOLGE_MERSIN, vadeBastar, vadeBittar, ikiAyBorcluOlanlar, ikiAyBorcluOlanlar);
            int ikiAyBorcluMer = ikiAyBorcluMerDataTable == null ? 0 : ikiAyBorcluMerDataTable.Rows.Count;

            DataTable birAyBorcluMerDataTable = odemePlaniDao.SelectBorcluOdemePlanlariByBolgeTarih(ProjeConstants.BOLGE_MERSIN, vadeBastar, vadeBittar, birAyBorcluOlanlar, birAyBorcluOlanlar);
            int birAyBorcluMer = birAyBorcluMerDataTable == null ? 0 : birAyBorcluMerDataTable.Rows.Count;

            //Top
            int birAyBorcluTop = birAyBorcluGM + birAyBorcluIst + birAyBorcluIzm + birAyBorcluMer;
            int ikiAyBorcluTop = ikiAyBorcluGM + ikiAyBorcluIst + ikiAyBorcluIzm + ikiAyBorcluMer;
            int ucAyBorcluTop = ucAyBorcluGM + ucAyBorcluIst + ucAyBorcluIzm + ucAyBorcluMer;
            int dortAyBorcluTop = dortAyBorcluGM + dortAyBorcluIst + dortAyBorcluIzm + dortAyBorcluMer;



            //Genel Müdürlük
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, GM1AyCell, birAyBorcluGM.ToString(), ProjeConstants.BOLGE_GENELMUDURLUK, birAyBorcluOlanlar, birAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, GM2AyCell, ikiAyBorcluGM.ToString(), ProjeConstants.BOLGE_GENELMUDURLUK, ikiAyBorcluOlanlar, ikiAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, GM3AyCell, ucAyBorcluGM.ToString(), ProjeConstants.BOLGE_GENELMUDURLUK, ucAyBorcluOlanlar, ucAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, GM4AyCell, dortAyBorcluGM.ToString(), ProjeConstants.BOLGE_GENELMUDURLUK, dortAyBorcluOlanlar, sonsuzAyBorcluOlanlar);

            //İstanbul
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, Ist1AyCell, birAyBorcluIst.ToString(), ProjeConstants.BOLGE_ISTANBUL, birAyBorcluOlanlar, birAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, Ist2AyCell, ikiAyBorcluIst.ToString(), ProjeConstants.BOLGE_ISTANBUL, ikiAyBorcluOlanlar, ikiAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, Ist3AyCell, ucAyBorcluIst.ToString(), ProjeConstants.BOLGE_ISTANBUL, ucAyBorcluOlanlar, ucAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, Ist4AyCell, dortAyBorcluIst.ToString(), ProjeConstants.BOLGE_ISTANBUL, dortAyBorcluOlanlar, sonsuzAyBorcluOlanlar);

            //İzmir
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, Izm1AyCell, birAyBorcluIzm.ToString(), ProjeConstants.BOLGE_IZMIR, birAyBorcluOlanlar, birAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, Izm2AyCell, ikiAyBorcluIzm.ToString(), ProjeConstants.BOLGE_IZMIR, ikiAyBorcluOlanlar, ikiAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, Izm3AyCell, ucAyBorcluIzm.ToString(), ProjeConstants.BOLGE_IZMIR, ucAyBorcluOlanlar, ucAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, Izm4AyCell, dortAyBorcluIzm.ToString(), ProjeConstants.BOLGE_IZMIR, dortAyBorcluOlanlar, sonsuzAyBorcluOlanlar);

            //Mersin
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, Mer1AyCell, birAyBorcluMer.ToString(), ProjeConstants.BOLGE_MERSIN, birAyBorcluOlanlar, birAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, Mer2AyCell, ikiAyBorcluMer.ToString(), ProjeConstants.BOLGE_MERSIN, ikiAyBorcluOlanlar, ikiAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, Mer3AyCell, ucAyBorcluMer.ToString(), ProjeConstants.BOLGE_MERSIN, ucAyBorcluOlanlar, ucAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, Mer4AyCell, dortAyBorcluMer.ToString(), ProjeConstants.BOLGE_MERSIN, dortAyBorcluOlanlar, sonsuzAyBorcluOlanlar);

            //bölge toplamları
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, Top1AyCell, birAyBorcluTop.ToString(), ProjeConstants.BOLGE_HEPSI, birAyBorcluOlanlar, birAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, Top2AyCell, ikiAyBorcluTop.ToString(), ProjeConstants.BOLGE_HEPSI, ikiAyBorcluOlanlar, ikiAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, Top3AyCell, ucAyBorcluTop.ToString(), ProjeConstants.BOLGE_HEPSI, ucAyBorcluOlanlar, ucAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, Top4AyCell, dortAyBorcluTop.ToString(), ProjeConstants.BOLGE_HEPSI, dortAyBorcluOlanlar, sonsuzAyBorcluOlanlar);


            //ay bölge toplamları --en sağ sütun
            string gmTop = (birAyBorcluGM + ikiAyBorcluGM + ucAyBorcluGM + dortAyBorcluGM).ToString();
            string istTop = (birAyBorcluIst + ikiAyBorcluIst + ucAyBorcluIst + dortAyBorcluIst).ToString();
            string izmTop = (birAyBorcluIzm + ikiAyBorcluIzm + ucAyBorcluIzm + dortAyBorcluIzm).ToString();
            string merTop = (birAyBorcluMer + ikiAyBorcluMer + ucAyBorcluMer + dortAyBorcluMer).ToString();
            string genTop = (birAyBorcluTop + ikiAyBorcluTop + ucAyBorcluTop + dortAyBorcluTop).ToString();
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, GMTopCell, gmTop, ProjeConstants.BOLGE_GENELMUDURLUK, birAyBorcluOlanlar, sonsuzAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, IstTopCell, istTop, ProjeConstants.BOLGE_ISTANBUL, birAyBorcluOlanlar, sonsuzAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, IzmTopCell, izmTop, ProjeConstants.BOLGE_IZMIR, birAyBorcluOlanlar, sonsuzAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, MerTopCell, merTop, ProjeConstants.BOLGE_MERSIN, birAyBorcluOlanlar, sonsuzAyBorcluOlanlar);
            HyperLinkEkle(ProjeConstants.PAGE_BORCLUKIRACI_LIST, GenTopCell, genTop, ProjeConstants.BOLGE_HEPSI, birAyBorcluOlanlar, sonsuzAyBorcluOlanlar);
        }

        private void HyperLinkEkle(string page, TableCell cell, string value, string bolge, int aySayisiBas, int aySayisiBit)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            HyperLink cellLnk = new HyperLink();
            cellLnk.Text = value;
            if (value.ConvertToInt() > 0)
            {
                string linkUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + page + "?Auth=IEYS&Bolge=" + bolge + (aySayisiBas > 0 ? "&AySayisiBas=" + aySayisiBas : "") +
                    (aySayisiBit > 0 ? "&AySayisiBit=" + aySayisiBit : "") + "&SecilenAy=" + SecilenAyQS + "&SecilenYil=" + SecilenYilQS;
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
            int besYilOnce = 2005;//year - 7;
            //YilDDL.Items.Add(new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI_INT.ToString()));
            for (int i = besYilOnce; i <= year; i++)
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
        protected void OdemePlanlariniGuncelleBtn_Click(object sender, EventArgs e)
        {
            TBYSOrtak.AktifSozleslemelerinBakiyeBorcunuHesapla();
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenAyQS = AyDDL.SelectedItem.Value.ToString();
            RedirectToPage(ProjeConstants.PAGE_BORCLUKIRACIBYBOLGE + "?SecilenAy=" + SecilenAyQS + "&SecilenYil=" + SecilenYilQS);
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
            RedirectToPage(ProjeConstants.PAGE_BORCLUKIRACIBYBOLGE + "?SecilenAy=" + SecilenAyQS + "&SecilenYil=" + SecilenYilQS);
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

