using Model.NBYS;
using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.BeratBasimiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BeratBasimiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BeratBasimiWP()
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
        private void FillDurumValues()
        {
            SetButtonsToFalse();

            Armagan armagan = new Armagan();
            //Genel Müdürlük
            DataTable gmal = armagan.SelectCountDurumByBolgeBasTarBitTar(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_ALTINID, ProjeConstants.BOLGE_ANKARA_INT);
            DataTable gmgum = armagan.SelectCountDurumByBolgeBasTarBitTar(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_GUMUSID, ProjeConstants.BOLGE_ANKARA_INT);
            DataTable gmbro = armagan.SelectCountDurumByBolgeBasTarBitTar(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_BRONZID, ProjeConstants.BOLGE_ANKARA_INT);


            FillTable(gmal, AnkATable, AnkAltinBtn, AnkAltinEtiketBtn, AnkAltinDurumChk);
            FillTable(gmgum, AnkGTable, AnkGumusBtn, AnkGumusEtiketBtn, AnkGumusDurumChk);
            FillTable(gmbro, AnkBTable, AnkBronzBtn, AnkBronzEtiketBtn, AnkBronzDurumChk);

            //İstanbul
            DataTable istal = armagan.SelectCountDurumByBolgeBasTarBitTar(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_ALTINID, ProjeConstants.BOLGE_ISTANBUL_INT);
            DataTable istgum = armagan.SelectCountDurumByBolgeBasTarBitTar(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_GUMUSID, ProjeConstants.BOLGE_ISTANBUL_INT);
            DataTable istbro = armagan.SelectCountDurumByBolgeBasTarBitTar(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_BRONZID, ProjeConstants.BOLGE_ISTANBUL_INT);

            FillTable(istal, IstATable, IstAltinBtn, IstAltinEtiketBtn, IstAltinDurumChk);
            FillTable(istgum, IstGTable, IstGumusBtn, IstGumusEtiketBtn, IstGumusDurumChk);
            FillTable(istbro, IstBTable, IstBronzBtn, IstBronzEtiketBtn, IstBronzDurumChk);

            //İzmir
            DataTable izmal = armagan.SelectCountDurumByBolgeBasTarBitTar(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_ALTINID, ProjeConstants.BOLGE_IZMIR_INT);
            DataTable izmgum = armagan.SelectCountDurumByBolgeBasTarBitTar(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_GUMUSID, ProjeConstants.BOLGE_IZMIR_INT);
            DataTable izmbro = armagan.SelectCountDurumByBolgeBasTarBitTar(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_BRONZID, ProjeConstants.BOLGE_IZMIR_INT);

            FillTable(izmal, IzmATable, IzmAltinBtn, IzmAltinEtiketBtn, IzmAltinDurumChk);
            FillTable(izmgum, IzmGTable, IzmGumusBtn, IzmGumusEtiketBtn, IzmGumusDurumChk);
            FillTable(izmbro, IzmBTable, IzmBronzBtn, IzmBronzEtiketBtn, IzmBronzDurumChk);

            //Mersin
            DataTable meral = armagan.SelectCountDurumByBolgeBasTarBitTar(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_ALTINID, ProjeConstants.BOLGE_MERSIN_INT);
            DataTable mergum = armagan.SelectCountDurumByBolgeBasTarBitTar(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_GUMUSID, ProjeConstants.BOLGE_MERSIN_INT);
            DataTable merbro = armagan.SelectCountDurumByBolgeBasTarBitTar(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_BRONZID, ProjeConstants.BOLGE_MERSIN_INT);

            FillTable(meral, MerATable, MerAltinBtn, MerAltinEtiketBtn, MerAltinDurumChk);
            FillTable(mergum, MerGTable, MerGumusBtn, MerGumusEtiketBtn, MerGumusDurumChk);
            FillTable(merbro, MerBTable, MerBronzBtn, MerBronzEtiketBtn, MerBronzDurumChk);

        }
        private void SetButtonsToFalse()
        {
            AnkAltinBtn.Enabled = false;
            AnkGumusBtn.Enabled = false;
            AnkBronzBtn.Enabled = false;
            AnkAltinEtiketBtn.Enabled = false;
            AnkGumusEtiketBtn.Enabled = false;
            AnkBronzEtiketBtn.Enabled = false;
            AnkAltinDurumChk.Checked = false;
            AnkGumusDurumChk.Checked = false;
            AnkBronzDurumChk.Checked = false;
            AnkAltinDurumChk.Enabled = false;
            AnkGumusDurumChk.Enabled = false;
            AnkBronzDurumChk.Enabled = false;

            IstAltinBtn.Enabled = false;
            IstGumusBtn.Enabled = false;
            IstBronzBtn.Enabled = false;
            IstAltinEtiketBtn.Enabled = false;
            IstGumusEtiketBtn.Enabled = false;
            IstBronzEtiketBtn.Enabled = false;
            IstAltinDurumChk.Checked = false;
            IstGumusDurumChk.Checked = false;
            IstBronzDurumChk.Checked = false;
            IstAltinDurumChk.Enabled = false;
            IstGumusDurumChk.Enabled = false;
            IstBronzDurumChk.Enabled = false;

            IzmAltinBtn.Enabled = false;
            IzmGumusBtn.Enabled = false;
            IzmBronzBtn.Enabled = false;
            IzmAltinEtiketBtn.Enabled = false;
            IzmGumusEtiketBtn.Enabled = false;
            IzmBronzEtiketBtn.Enabled = false;
            IzmAltinDurumChk.Checked = false;
            IzmGumusDurumChk.Checked = false;
            IzmBronzDurumChk.Checked = false;
            IzmAltinDurumChk.Enabled = false;
            IzmGumusDurumChk.Enabled = false;
            IzmBronzDurumChk.Enabled = false;

            MerAltinBtn.Enabled = false;
            MerGumusBtn.Enabled = false;
            MerBronzBtn.Enabled = false;
            MerAltinEtiketBtn.Enabled = false;
            MerGumusEtiketBtn.Enabled = false;
            MerBronzEtiketBtn.Enabled = false;
            MerAltinDurumChk.Checked = false;
            MerGumusDurumChk.Checked = false;
            MerBronzDurumChk.Checked = false;
            MerAltinDurumChk.Enabled = false;
            MerGumusDurumChk.Enabled = false;
            MerBronzDurumChk.Enabled = false;
        }
        private void FillTable(DataTable dataTable, Table table, Button beratBtn, Button etiketBtn, CheckBox durumChk)
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
                            beratBtn.Enabled = true;
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
            //GunDDL.Items.Add(new ListItem("Tüm Ay", "0"));
            //GunDDL.Items.Add(new ListItem("1-15", "1"));
            //GunDDL.Items.Add(new ListItem("16-Ay Sonu", "2"));
            //Ayda bir
            GunDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Tüm Ay", "0"));
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
                int gunBolumu = DateTime.Today.Day < 16 ? 1 : 2;
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
                string ay = !string.IsNullOrEmpty(SecilenAyQS) ? SecilenAyQS : DateTime.Today.Month.ReturnEmptyIfNull().ToString();
                ListItem ayItem = new ListItem();
                if (!string.IsNullOrEmpty(ay))
                    ayItem = AyDDL.Items.FindByValue(ay);

                if (ayItem != null)
                {
                    AyDDL.SelectedValue = ayItem.Value;
                    SecilenAyQS = ayItem.Value;
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
        protected void GunDDL_SelectedIndexChanged(object sender, EventArgs e)
        {

            SecilenGunQS = GunDDL.SelectedItem.Value.ToString();
            SetSecilenBasTarBitTar();
            FillDurumValues();
            //KayitGetir();
        }
        private void SetSecilenBasTarBitTar()
        {
            string gunStr = GunDDL.SelectedItem.Value;
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();

            DateTime basTar = DateTime.Today;
            DateTime bitTar = DateTime.Today;
            //ARMAGAN PERIODU
            //10 Güne bir
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
        protected void AnkAltinBtn_Click(object sender, EventArgs e)
        {
            BasimaGonder(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.ARMAGAN_ALTINID, AnkAltinDurumChk);
        }
        protected void AnkGumusBtn_Click(object sender, EventArgs e)
        {
            BasimaGonder(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.ARMAGAN_GUMUSID, AnkGumusDurumChk);
        }
        protected void AnkBronzBtn_Click(object sender, EventArgs e)
        {
            BasimaGonder(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.ARMAGAN_BRONZID, AnkBronzDurumChk);
        }
        protected void IstAltinBtn_Click(object sender, EventArgs e)
        {
            BasimaGonder(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.ARMAGAN_ALTINID, IstAltinDurumChk);
        }
        protected void IstGumusBtn_Click(object sender, EventArgs e)
        {
            BasimaGonder(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.ARMAGAN_GUMUSID, IstGumusDurumChk);
        }
        protected void IstBronzBtn_Click(object sender, EventArgs e)
        {
            BasimaGonder(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.ARMAGAN_BRONZID, IstBronzDurumChk);
        }
        protected void IzmAltinBtn_Click(object sender, EventArgs e)
        {
            BasimaGonder(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.ARMAGAN_ALTINID, IzmAltinDurumChk);
        }
        protected void IzmGumusBtn_Click(object sender, EventArgs e)
        {
            BasimaGonder(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.ARMAGAN_GUMUSID, IzmGumusDurumChk);
        }
        protected void IzmBronzBtn_Click(object sender, EventArgs e)
        {
            BasimaGonder(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.ARMAGAN_BRONZID, IzmBronzDurumChk);
        }
        protected void MerAltinBtn_Click(object sender, EventArgs e)
        {
            BasimaGonder(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.ARMAGAN_ALTINID, MerAltinDurumChk);
        }
        protected void MerGumusBtn_Click(object sender, EventArgs e)
        {
            BasimaGonder(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.ARMAGAN_GUMUSID, MerGumusDurumChk);
        }
        protected void MerBronzBtn_Click(object sender, EventArgs e)
        {
            BasimaGonder(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.ARMAGAN_BRONZID, MerBronzDurumChk);
        }
        private void BasimaGonder(int bolgeId, int armaganTipi, CheckBox checkBox)
        {
            if (checkBox.Checked)
            {
                Armagan armagan = new Armagan();
                try
                {
                    bool isUpdated = armagan.UpdateDurumByBolge(
                        ProjeConstants.DURUM_KONTROLEDILDI, ProjeConstants.DURUM_GONDERILDI,
                        SecilenBastarQS, SecilenBittarQS,
                        armaganTipi, bolgeId);

                }
                catch (Exception ex)
                {
                    ExceptionHelper exHelper = new ExceptionHelper(ex);
                    exHelper.PublishException();
                }
            }
            FillDurumValues();
            var queryString = string.Format("?Bastar={0}&Bittar={1}&ArmaganTanimId={2}&BolgeId={3}", SecilenBastarQS, SecilenBittarQS, armaganTipi, bolgeId);
            RedirectToPage(ProjeConstants.PAGE_BERATBELGESI_VIEWER + queryString + "&target=_blank");
        }
        private void EtiketleriBasimaGonder(int bolgeId, int armaganTipi)
        {
            FillDurumValues();
            var queryString = string.Format("?Bastar={0}&Bittar={1}&ArmaganTanimId={2}&BolgeId={3}", SecilenBastarQS, SecilenBittarQS, armaganTipi, bolgeId);
            RedirectToPage(ProjeConstants.PAGE_BERATETIKET_VIEWER + queryString);
        }
        protected void AnkAltinEtiketBtn_Click(object sender, EventArgs e)
        {
            EtiketleriBasimaGonder(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.ARMAGAN_ALTINID);
        }

        protected void AnkGumusEtiketBtn_Click(object sender, EventArgs e)
        {
            EtiketleriBasimaGonder(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.ARMAGAN_GUMUSID);
        }

        protected void AnkBronzEtiketBtn_Click(object sender, EventArgs e)
        {
            EtiketleriBasimaGonder(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.ARMAGAN_BRONZID);
        }

        protected void IstAltinEtiketBtn_Click(object sender, EventArgs e)
        {
            EtiketleriBasimaGonder(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.ARMAGAN_ALTINID);
        }

        protected void IstGumusEtiketBtn_Click(object sender, EventArgs e)
        {
            EtiketleriBasimaGonder(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.ARMAGAN_GUMUSID);
        }

        protected void IstBronzEtiketBtn_Click(object sender, EventArgs e)
        {
            EtiketleriBasimaGonder(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.ARMAGAN_BRONZID);
        }
        protected void IzmAltinEtiketBtn_Click(object sender, EventArgs e)
        {
            EtiketleriBasimaGonder(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.ARMAGAN_ALTINID);
        }

        protected void IzmGumusEtiketBtn_Click(object sender, EventArgs e)
        {
            EtiketleriBasimaGonder(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.ARMAGAN_GUMUSID);
        }

        protected void IzmBronzEtiketBtn_Click(object sender, EventArgs e)
        {
            EtiketleriBasimaGonder(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.ARMAGAN_BRONZID);
        }

        protected void MerAltinEtiketBtn_Click(object sender, EventArgs e)
        {
            EtiketleriBasimaGonder(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.ARMAGAN_ALTINID);
        }

        protected void MerGumusEtiketBtn_Click(object sender, EventArgs e)
        {
            EtiketleriBasimaGonder(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.ARMAGAN_GUMUSID);
        }

        protected void MerBronzEtiketBtn_Click(object sender, EventArgs e)
        {
            EtiketleriBasimaGonder(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.ARMAGAN_BRONZID);
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
