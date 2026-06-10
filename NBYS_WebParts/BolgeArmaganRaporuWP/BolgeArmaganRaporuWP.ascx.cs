using Model.NBYS;
using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.BolgeArmaganRaporuWP
{
    [ToolboxItemAttribute(false)]
    public partial class BolgeArmaganRaporuWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BolgeArmaganRaporuWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SecilenBasAyQS
        {
            get
            {

                if (ViewState["SecilenBasAy"] == null)
                {
                    if (Page.Request.QueryString["SecilenBasAy"] != null)
                    {
                        ViewState["SecilenBasAy"] = Page.Request.QueryString["SecilenBasAy"];
                    }
                    else
                    {
                        ViewState["SecilenBasAy"] = string.Empty;
                    }
                }
                return ViewState["SecilenBasAy"].ToString();
            }

            set
            {
                ViewState["SecilenBasAy"] = value;
            }
        }
        private string SecilenBitAyQS
        {
            get
            {

                if (ViewState["SecilenBitAy"] == null)
                {
                    if (Page.Request.QueryString["SecilenBitAy"] != null)
                    {
                        ViewState["SecilenBitAy"] = Page.Request.QueryString["SecilenBitAy"];
                    }
                    else
                    {
                        ViewState["SecilenBitAy"] = string.Empty;
                    }
                }
                return ViewState["SecilenBitAy"].ToString();
            }

            set
            {
                ViewState["SecilenBitAy"] = value;
            }
        }
        private string SecilenBasYilQS
        {
            get
            {

                if (ViewState["SecilenBasYil"] == null)
                {
                    if (Page.Request.QueryString["SecilenBasYil"] != null)
                    {
                        ViewState["SecilenBasYil"] = Page.Request.QueryString["SecilenBasYil"];
                    }
                    else
                    {
                        ViewState["SecilenBasYil"] = string.Empty;
                    }
                }
                return ViewState["SecilenBasYil"].ToString();
            }

            set
            {
                ViewState["SecilenBasYil"] = value;
            }
        }
        private string SecilenBitYilQS
        {
            get
            {

                if (ViewState["SecilenBitYil"] == null)
                {
                    if (Page.Request.QueryString["SecilenBitYil"] != null)
                    {
                        ViewState["SecilenBitYil"] = Page.Request.QueryString["SecilenBitYil"];
                    }
                    else
                    {
                        ViewState["SecilenBitYil"] = string.Empty;
                    }
                }
                return ViewState["SecilenBitYil"].ToString();
            }

            set
            {
                ViewState["SecilenBitYil"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                DateTime today = DateTime.Today;

                DateTime bastar = new DateTime(today.Year, today.Month, 1);
                DateTime bittar = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1);


                SecilenBasAyQS = bastar.Month.ToString();
                SecilenBasYilQS = bastar.Year.ToString();
                SecilenBitAyQS = bittar.Month.ToString();
                SecilenBitYilQS = bittar.Year.ToString();

                FillMonth();
                FillYear();
                SetDDLValues();
                FillTable();
            }

        }
        protected void FillTable()
        {



            int basay = BasAyDDL.SelectedItem.Value.ConvertToInt();
            int basyil = BasYilDDL.SelectedItem.Value.ConvertToInt();

            int bitay = BitAyDDL.SelectedItem.Value.ConvertToInt();
            int bityil = BitYilDDL.SelectedItem.Value.ConvertToInt();

            DateTime bastar = new DateTime(basyil, basay, 1);
            DateTime bittar = new DateTime(bityil, bitay, 1).AddMonths(1).AddDays(-1);

            TableHeaderCell.Text = bastar.ToString(ProjeConstants.DATE_TR) + " - " + bittar.ToString(ProjeConstants.DATE_TR) + " TARİHLERİ ARASI ARMAĞAN RAPORU";

            SetCellValues(bastar, bittar);

            AnkAdetTopCell.Text = AnkAltinAdetCell.Text.ConvertToInt() + AnkGumusAdetCell.Text.ConvertToInt() + AnkBronzAdetCell.Text.ConvertToInt() +
                AnkTesAdetCell.Text.ConvertToInt() + "";

            IstAdetTopCell.Text = IstAltinAdetCell.Text.ConvertToInt() + IstGumusAdetCell.Text.ConvertToInt() + IstBronzAdetCell.Text.ConvertToInt() +
                IstTesAdetCell.Text.ConvertToInt() + "";

            IzmAdetTopCell.Text = IzmAltinAdetCell.Text.ConvertToInt() + IzmGumusAdetCell.Text.ConvertToInt() + IzmBronzAdetCell.Text.ConvertToInt() +
                IzmTesAdetCell.Text.ConvertToInt() + "";

            MerAdetTopCell.Text = MerAltinAdetCell.Text.ConvertToInt() + MerGumusAdetCell.Text.ConvertToInt() + MerBronzAdetCell.Text.ConvertToInt() +
                MerTesAdetCell.Text.ConvertToInt() + "";

            ErzAdetTopCell.Text = ErzAltinAdetCell.Text.ConvertToInt() + ErzGumusAdetCell.Text.ConvertToInt() + ErzBronzAdetCell.Text.ConvertToInt() +
                ErzTesAdetCell.Text.ConvertToInt() + "";

            YurtDisiAdetTopCell.Text = YurtDisiAltinAdetCell.Text.ConvertToInt() + YurtDisiGumusAdetCell.Text.ConvertToInt() + YurtDisiBronzAdetCell.Text.ConvertToInt() +
               YurtDisiTesAdetCell.Text.ConvertToInt() + "";

            AltinAdetToplamCell.Text = AnkAltinAdetCell.Text.ConvertToInt() + IstAltinAdetCell.Text.ConvertToInt() + IzmAltinAdetCell.Text.ConvertToInt() + MerAltinAdetCell.Text.ConvertToInt() + ErzAltinAdetCell.Text.ConvertToInt()+ YurtDisiAltinAdetCell.Text.ConvertToInt() + "";
            GumusAdetToplamCell.Text = AnkGumusAdetCell.Text.ConvertToInt() + IstGumusAdetCell.Text.ConvertToInt() + IzmGumusAdetCell.Text.ConvertToInt() + MerGumusAdetCell.Text.ConvertToInt() + ErzGumusAdetCell.Text.ConvertToInt()+ YurtDisiGumusAdetCell.Text.ConvertToInt() + "";
            BronzAdetToplamCell.Text = AnkBronzAdetCell.Text.ConvertToInt() + IstBronzAdetCell.Text.ConvertToInt() + IzmBronzAdetCell.Text.ConvertToInt() + MerBronzAdetCell.Text.ConvertToInt() + ErzBronzAdetCell.Text.ConvertToInt() + YurtDisiBronzAdetCell.Text.ConvertToInt() + "";
            TesAdetToplamCell.Text = AnkTesAdetCell.Text.ConvertToInt() + IstTesAdetCell.Text.ConvertToInt() + IzmTesAdetCell.Text.ConvertToInt() + MerTesAdetCell.Text.ConvertToInt()+ ErzTesAdetCell.Text.ConvertToInt() + YurtDisiTesAdetCell.Text.ConvertToInt() + "";
            TopAdetTopCell.Text = AltinAdetToplamCell.Text.ConvertToInt() + GumusAdetToplamCell.Text.ConvertToInt() + BronzAdetToplamCell.Text.ConvertToInt() + TesAdetToplamCell.Text.ConvertToInt() +  "";
        }
        private void SetCellValues(DateTime bastar, DateTime bittar)
        {
            try
            {
                Armagan armagan = new Armagan();
                DataTable dataTable = armagan.SelectCountByBagisTarihiBolge(bastar, bittar);
                if (dataTable != null)
                {
                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow row in dataTable.Rows)
                        {

                            string adet = row["Adet"].ReturnZeroIfNull().ToString();
                            int bolgeId = row["BolgeId"].ReturnZeroIfNull().ConvertToInt();
                            int armaganTanimId = Int32.Parse(row["ArmaganTanimId"].ReturnZeroIfNull().ToString());

                            if (bolgeId==ProjeConstants.BOLGE_ANKARA_INT)
                            {
                                switch (armaganTanimId)
                                {
                                    case ProjeConstants.ARMAGAN_ALTINID:
                                        {
                                            AnkAltinAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_GUMUSID:
                                        {
                                            AnkGumusAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_BRONZID:
                                        {
                                            AnkBronzAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_TESEKKURID:
                                        {
                                            AnkTesAdetCell.Text = adet;
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }
                            else if (bolgeId == ProjeConstants.BOLGE_ISTANBUL_INT)
                            {
                                switch (armaganTanimId)
                                {
                                    case ProjeConstants.ARMAGAN_ALTINID:
                                        {
                                            IstAltinAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_GUMUSID:
                                        {
                                            IstGumusAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_BRONZID:
                                        {
                                            IstBronzAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_TESEKKURID:
                                        {
                                            IstTesAdetCell.Text = adet;
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }
                            else if (bolgeId == ProjeConstants.BOLGE_IZMIR_INT)
                            {
                                switch (armaganTanimId)
                                {
                                    case ProjeConstants.ARMAGAN_ALTINID:
                                        {
                                            IzmAltinAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_GUMUSID:
                                        {
                                            IzmGumusAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_BRONZID:
                                        {
                                            IzmBronzAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_TESEKKURID:
                                        {
                                            IzmTesAdetCell.Text = adet;
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }
                            else if (bolgeId == ProjeConstants.BOLGE_MERSIN_INT)
                            {
                                switch (armaganTanimId)
                                {
                                    case ProjeConstants.ARMAGAN_ALTINID:
                                        {
                                            MerAltinAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_GUMUSID:
                                        {
                                            MerGumusAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_BRONZID:
                                        {
                                            MerBronzAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_TESEKKURID:
                                        {
                                            MerTesAdetCell.Text = adet;
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }
                            else if (bolgeId == ProjeConstants.BOLGE_ERZURUM_INT)
                            {
                                switch (armaganTanimId)
                                {
                                    case ProjeConstants.ARMAGAN_ALTINID:
                                        {
                                            ErzAltinAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_GUMUSID:
                                        {
                                            ErzGumusAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_BRONZID:
                                        {
                                            ErzBronzAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_TESEKKURID:
                                        {
                                            ErzTesAdetCell.Text = adet;
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }
                            else if (bolgeId == ProjeConstants.BOLGE_YURTDISI_INT)
                            {
                                switch (armaganTanimId)
                                {
                                    case ProjeConstants.ARMAGAN_ALTINID:
                                        {
                                            YurtDisiAltinAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_GUMUSID:
                                        {
                                            YurtDisiGumusAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_BRONZID:
                                        {
                                            YurtDisiBronzAdetCell.Text = adet;
                                            break;
                                        }
                                    case ProjeConstants.ARMAGAN_TESEKKURID:
                                        {
                                            YurtDisiTesAdetCell.Text = adet;
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }
                        }


                    }
                }
            }
            catch (Exception e)
            {

                ExceptionHelper eh = new ExceptionHelper(e);
                eh.PublishException();
            }
        }
        private void FillMonth()
        {

            BasAyDDL.Items.Add(new ListItem("Ocak", "1"));
            BasAyDDL.Items.Add(new ListItem("Şubat", "2"));
            BasAyDDL.Items.Add(new ListItem("Mart", "3"));
            BasAyDDL.Items.Add(new ListItem("Nisan", "4"));
            BasAyDDL.Items.Add(new ListItem("Mayıs", "5"));
            BasAyDDL.Items.Add(new ListItem("Haziran", "6"));
            BasAyDDL.Items.Add(new ListItem("Temmuz", "7"));
            BasAyDDL.Items.Add(new ListItem("Ağustos", "8"));
            BasAyDDL.Items.Add(new ListItem("Eylül", "9"));
            BasAyDDL.Items.Add(new ListItem("Ekim", "10"));
            BasAyDDL.Items.Add(new ListItem("Kasım", "11"));
            BasAyDDL.Items.Add(new ListItem("Aralık", "12"));

            BitAyDDL.Items.Add(new ListItem("Ocak", "1"));
            BitAyDDL.Items.Add(new ListItem("Şubat", "2"));
            BitAyDDL.Items.Add(new ListItem("Mart", "3"));
            BitAyDDL.Items.Add(new ListItem("Nisan", "4"));
            BitAyDDL.Items.Add(new ListItem("Mayıs", "5"));
            BitAyDDL.Items.Add(new ListItem("Haziran", "6"));
            BitAyDDL.Items.Add(new ListItem("Temmuz", "7"));
            BitAyDDL.Items.Add(new ListItem("Ağustos", "8"));
            BitAyDDL.Items.Add(new ListItem("Eylül", "9"));
            BitAyDDL.Items.Add(new ListItem("Ekim", "10"));
            BitAyDDL.Items.Add(new ListItem("Kasım", "11"));
            BitAyDDL.Items.Add(new ListItem("Aralık", "12"));
        }
        private void FillYear()
        {
            var year = DateTime.Now.Year;
            for (int i = 2010; i <= year; i++)
            {
                BasYilDDL.Items.Add(new ListItem(i.ToString(), i.ToString()));
                BitYilDDL.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }
        private void SetDDLValues()
        {
            try
            {
                //açılışta ay ve yılı querystring ile gelen ay ve yıla eşitle; boş geldiyse geçen aya/yıla eşitle

                //ay
                string ay = !string.IsNullOrEmpty(SecilenBasAyQS) ? SecilenBasAyQS : DateTime.Today.Month.ReturnEmptyIfNull().ToString();
                ListItem AyItem = new ListItem();
                if (!string.IsNullOrEmpty(ay))
                    AyItem = BasAyDDL.Items.FindByValue(ay);

                if (AyItem != null)
                {
                    BasAyDDL.SelectedValue = AyItem.Value;
                    SecilenBasAyQS = AyItem.Value;
                    BitAyDDL.SelectedValue = AyItem.Value;
                    SecilenBitAyQS = AyItem.Value;
                }

                //yil
                string yil = !string.IsNullOrEmpty(SecilenBasYilQS) ? SecilenBasYilQS : DateTime.Today.Year.ReturnEmptyIfNull().ToString();
                ListItem YilItem = new ListItem();
                if (!string.IsNullOrEmpty(yil))
                    YilItem = BasYilDDL.Items.FindByValue(yil);

                if (YilItem != null)
                {
                    BasYilDDL.SelectedValue = YilItem.Value;
                    SecilenBasYilQS = YilItem.Value;
                    BitYilDDL.SelectedValue = YilItem.Value;
                    SecilenBitYilQS = YilItem.Value;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }



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
        }
        protected void ExportToExcel()
        {
            string filename = "ArmaganRaporu" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.Unicode;
            Page.Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            NBTable.RenderControl(hw);
            //Write the HTML back to the browser.
            //Response.ContentType = application/vnd.ms-excel;
            Page.Response.ContentType = "application/vnd.ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            Page.Response.Write(tw.ToString());
            Page.Response.End();
        }
        protected void BasAyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillTable();
        }
    }
}
