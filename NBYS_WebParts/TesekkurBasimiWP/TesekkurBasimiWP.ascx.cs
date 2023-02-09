using Model.NBYS;
using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.TesekkurBasimiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TesekkurBasimiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TesekkurBasimiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SecilenGunQS
        {
            get
            {

                if (ViewState["SecilenGun"] == null)
                {
                    if (Page.Request.QueryString["SecilenGun"] != null)
                    {
                        ViewState["SecilenGun"] = Page.Request.QueryString["SecilenGun"];
                    }
                    else
                    {
                        ViewState["SecilenGun"] = string.Empty;
                    }
                }
                return ViewState["SecilenGun"].ToString();
            }

            set
            {
                ViewState["SecilenGun"] = value;
            }
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
        private string SecilenBastarQS
        {
            get
            {

                if (ViewState["SecilenBastar"] == null)
                {
                    if (Page.Request.QueryString["SecilenBastar"] != null)
                    {
                        ViewState["SecilenBastar"] = Page.Request.QueryString["SecilenBastar"];
                    }
                    else
                    {
                        ViewState["SecilenBastar"] = string.Empty;
                    }
                }
                return ViewState["SecilenBastar"].ToString();
            }

            set
            {
                ViewState["SecilenBastar"] = value;
            }
        }
        private string SecilenBittarQS
        {
            get
            {

                if (ViewState["SecilenBittar"] == null)
                {
                    if (Page.Request.QueryString["SecilenBittar"] != null)
                    {
                        ViewState["SecilenBittar"] = Page.Request.QueryString["SecilenBittar"];
                    }
                    else
                    {
                        ViewState["SecilenBittar"] = string.Empty;
                    }
                }
                return ViewState["SecilenBittar"].ToString();
            }

            set
            {
                ViewState["SecilenBittar"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    FillDropDownList();
                    SetDDLValues(); //ay ve yılı querystringden al
                    SetSecilenBasTarBitTar();
                    FillDurumValues();
                }

            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void FillDropDownList()
        {
            GunDDLDoldur();
            AyDDLDoldur();
            YilDDLDoldur();
        }
        private void GunDDLDoldur()
        {
            //ARMAGAN PERIODU
            //10 Günde bir
            //GunDDL.Items.Add(new ListItem("Tüm Ay", "0"));
            //GunDDL.Items.Add(new ListItem("1-10", "1"));
            //GunDDL.Items.Add(new ListItem("11-20", "2"));
            //GunDDL.Items.Add(new ListItem("20-Ay Sonu", "3"));

            //15 Günde bir
            GunDDL.Items.Add(new ListItem("Tüm Ay", "0"));
            GunDDL.Items.Add(new ListItem("1-15", "1"));
            GunDDL.Items.Add(new ListItem("16-Ay Sonu", "2"));
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
            for (int i = 1987; i <= year; i++)
            {
                YilDDL.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }
        private void SetDDLValues()
        {
            try
            {
                //acilista ay ve yili querystring ile gelen ay ve yıla eşitle boş geldiyse gecen aya/yila eşitle
                //gün
                int gunBolumu = DateTime.Today.Day < 10 ? 1 : 2;
                string gun = !string.IsNullOrEmpty(SecilenGunQS) ? SecilenGunQS : gunBolumu.ToString();
                ListItem gunItem = new ListItem();
                if (!string.IsNullOrEmpty(gun))
                    gunItem = GunDDL.Items.FindByValue(gun);

                if (gunItem != null)
                {
                    GunDDL.SelectedValue = gunItem.Value;
                    SecilenGunQS = gunItem.Value;
                }
                //ay
                string ay = !string.IsNullOrEmpty(SecilenAyQS) ? SecilenAyQS : (gunBolumu == 1 ? DateTime.Today.AddMonths(-1).Month.ToString() : DateTime.Today.Month.ToString());
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
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        private void FillDurumValues()
        {
            setButtonsToFalse();

            Armagan armagan = new Armagan();
            //DataTable tesekkur = armagan.SelectCountDurumByBolge(SecilenAyQS, SecilenYilQS, ProjeConstants.ARMAGAN_TESEKKURID, "");
            DataTable tesekkur = armagan.SelectCountDurumByBolgeTarih(ProjeConstants.ARMAGAN_TESEKKURID, "", SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime());
            FillTable(tesekkur, TesekkurTable, TesekkurBtn, AdresEtiketBtn, TesekkurDurumChk);

        }
        private void FillTable(DataTable dataTable, Table table, Button belgeBtn, Button etiketBtn, CheckBox durumChk)
        {
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string durum = row["Durum"].ReturnEmptyIfNull().ToString();
                    string adet = row["Adet"].ReturnEmptyIfNull().ToString();
                    if (durum == ProjeConstants.DURUM_KONTROLEDILDI)
                    {
                        if (adet.ConvertToInt() > 0)
                        {
                            belgeBtn.Enabled = true;
                            durumChk.Enabled = true;
                        }

                    }
                    else if (durum == ProjeConstants.DURUM_GONDERILDI)
                    {
                        if (adet.ConvertToInt() > 0)
                        {
                            etiketBtn.Enabled = true;
                        }

                    }

                    TableRow tblrow = new TableRow();
                    TableCell durumCell = new TableCell();
                    durumCell.Text = durum;
                    tblrow.Controls.Add(durumCell);

                    TableCell adetCell = new TableCell();
                    adetCell.Text = adet;
                    tblrow.Controls.Add(adetCell);
                    table.Controls.Add(tblrow);
                }
            }

        }
        private void setButtonsToFalse()
        {
            TesekkurBtn.Enabled = false;
            AdresEtiketBtn.Enabled = false;
            TesekkurDurumChk.Enabled = false;
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {

            SecilenAyQS = AyDDL.SelectedItem.Value.ToString();
            SetSecilenBasTarBitTar();
            FillDurumValues();
            //KayitGetir();
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
            SetSecilenBasTarBitTar();
            FillDurumValues();
            //KayitGetir();
        }
        protected void AdresEtiketBtn_Click(object sender, EventArgs e)
        {
            FillDurumValues();
            var queryString = string.Format("?Bastar={0}&Bittar={1}&ArmaganTanimId={2}", SecilenBastarQS, SecilenBittarQS, ProjeConstants.ARMAGAN_TESEKKURID);
            RedirectToPage(ProjeConstants.PAGE_BERATETIKET_VIEWER + queryString);
        }
        protected void TesekkurBtn_Click(object sender, EventArgs e)
        {
            if (TesekkurDurumChk.Checked)
            {
                Armagan armagan = new Armagan();
                try
                {
                    bool isUpdated = armagan.UpdateDurumByBolge(ProjeConstants.DURUM_KONTROLEDILDI, ProjeConstants.DURUM_GONDERILDI, SecilenBastarQS, SecilenBittarQS, ProjeConstants.ARMAGAN_TESEKKURID, "");

                }
                catch (Exception ex)
                {
                    ExceptionHelper exHelper = new ExceptionHelper(ex);
                    exHelper.PublishException();
                }
            }
            FillDurumValues();
            var queryString = string.Format("?Bastar={0}&Bittar={1}", SecilenBastarQS, SecilenBittarQS);
            RedirectToPage(ProjeConstants.PAGE_TESEKKURBELGESI_VIEWER + queryString);
        }
        protected void GunDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenGunQS = GunDDL.SelectedItem.Value.ToString();
            SetSecilenBasTarBitTar();
            FillDurumValues();
        }
        private void SetSecilenBasTarBitTar()
        {
            string gunStr = GunDDL.SelectedItem.Value;
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();

            DateTime basTar = DateTime.Today;
            DateTime bitTar = DateTime.Today;
            //ARMAGAN PERIODU
            //15 Günde bir
            //if (gunStr.Equals("0"))
            //{
            //    basTar = new DateTime(yil, ay, 1);
            //    int songun = basTar.AddMonths(1).AddDays(-1).Day;
            //    bitTar = new DateTime(yil, ay, songun);
            //}
            //else if (gunStr.Equals("1"))
            //{
            //    basTar = new DateTime(yil, ay, 1);
            //    bitTar = new DateTime(yil, ay, 10);
            //}
            //else if (gunStr.Equals("2"))
            //{
            //    basTar = new DateTime(yil, ay, 11);
            //    bitTar = new DateTime(yil, ay, 20);
            //}
            //else if (gunStr.Equals("3"))
            //{
            //    basTar = new DateTime(yil, ay, 21);
            //    DateTime basGun = new DateTime(yil, ay, 1).AddMonths(1).AddDays(-1);
            //    bitTar = new DateTime(yil, ay, basGun.Day);
            //}

            //15 Günde bir
            if (gunStr.Equals("0"))
            {
                basTar = new DateTime(yil, ay, 1);
                int songun = basTar.AddMonths(1).AddDays(-1).Day;
                bitTar = new DateTime(yil, ay, songun);
            }
            else if (gunStr.Equals("1"))
            {
                basTar = new DateTime(yil, ay, 1);
                bitTar = new DateTime(yil, ay, 15);
            }
            else if (gunStr.Equals("2"))
            {
                basTar = new DateTime(yil, ay, 16);
                DateTime basGun = new DateTime(yil, ay, 1).AddMonths(1).AddDays(-1);
                bitTar = new DateTime(yil, ay, basGun.Day);
            }
            SecilenBastarQS = basTar.ConvertToDatetimeEmptyIfNull();
            SecilenBittarQS = bitTar.ConvertToDatetimeEmptyIfNull();
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                //string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                string newUrl = "http://tskgv-portal/YonetimBirimleri/BasinTanitimHalklaIliskilerSubesi/Sayfalar" + "/" + pageUrl;
                ResponseHelper.Redirect(newUrl, "_blank", "");
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
    }
}
