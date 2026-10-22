using System.Linq;
using Utility.Logging;
using NBYS_WebParts.Logging;
using Model.NBYS;
using Model.Ortak;
using Model.Services.NBYS;
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
        private PortalLogContext LoggingContext { get { return PortalLogContext.FromCurrentRequest("NBYS", "BeratBasimiWP"); } }
        private Bolge userRegion;
        private bool canProcess = true;
        private void ExecuteAction(string operation, Action action)
        {
            NbysWebPartLog.ExecuteAction(LoggingContext, operation, () => { if (canProcess) action(); });
        }
        private void SetFilter(System.Web.UI.WebControls.DropDownList list, string value, string fallback)
        {
            var item = list.Items.FindByValue(value) ?? list.Items.FindByValue(fallback) ?? list.Items.Cast<System.Web.UI.WebControls.ListItem>().FirstOrDefault();
            if (item == null) throw new InvalidOperationException("Filtre seçeneği bulunamadı.");
            list.ClearSelection(); item.Selected = true;
        }

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
            ExecuteAction("Page_Load", () =>
            {
                if (!Page.IsPostBack) NbysWebPartLog.RecordOpened(LoggingContext);
                userRegion = new NbysBagisciWriteService(LoggingContext).GetUserRegion();
                if (userRegion == null) { canProcess = false; Messages.Warning("Bölge bilginiz bulunamadı. Yetki bilgilerinizi kontrol ettirin."); return; }
                if (!NbysDocumentWriteService.IsCentral(userRegion.Id))
                { canProcess = false; Messages.Warning("Bu toplu rapor ekranı genel müdürlük yetkisi gerektirir. Bölgeye göre belge hazırlama ekranını kullanın."); return; }
                if (!Page.IsPostBack)
                {
                    FillDropDownList();
                    SetDDLValues(); //ay ve yılı querystring'den al
                    SetSecilenBasTarBitTar();
                    FillDurumValues();
                }
            });
        }
        private void FillDurumValues()
        {
            foreach (var table in new[] { AnkATable, AnkGTable, AnkBTable, IstATable, IstGTable, IstBTable,
                IzmATable, IzmGTable, IzmBTable, MerATable, MerGTable, MerBTable, ErzATable, ErzGTable, ErzBTable }) table.Controls.Clear();
            SetButtonsToFalse();

            ArmaganService armaganService = new ArmaganService(LoggingContext);
            //Genel Müdürlük
            DataTable ankAl = armaganService.CountDurumByBolge(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_ALTINID, ProjeConstants.BOLGE_ANKARA_INT);
            DataTable ankGum = armaganService.CountDurumByBolge(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_GUMUSID, ProjeConstants.BOLGE_ANKARA_INT);
            DataTable ankBro = armaganService.CountDurumByBolge(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_BRONZID, ProjeConstants.BOLGE_ANKARA_INT);

            FillTable(ankAl, AnkATable, AnkAltinBtn, AnkAltinEtiketBtn, AnkAltinDurumChk);
            FillTable(ankGum, AnkGTable, AnkGumusBtn, AnkGumusEtiketBtn, AnkGumusDurumChk);
            FillTable(ankBro, AnkBTable, AnkBronzBtn, AnkBronzEtiketBtn, AnkBronzDurumChk);

            //Istanbul
            DataTable istAlt = armaganService.CountDurumByBolge(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_ALTINID, ProjeConstants.BOLGE_ISTANBUL_INT);
            DataTable istGum = armaganService.CountDurumByBolge(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_GUMUSID, ProjeConstants.BOLGE_ISTANBUL_INT);
            DataTable istBro = armaganService.CountDurumByBolge(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_BRONZID, ProjeConstants.BOLGE_ISTANBUL_INT);

            FillTable(istAlt, IstATable, IstAltinBtn, IstAltinEtiketBtn, IstAltinDurumChk);
            FillTable(istGum, IstGTable, IstGumusBtn, IstGumusEtiketBtn, IstGumusDurumChk);
            FillTable(istBro, IstBTable, IstBronzBtn, IstBronzEtiketBtn, IstBronzDurumChk);

            //Izmir
            DataTable izmAlt = armaganService.CountDurumByBolge(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_ALTINID, ProjeConstants.BOLGE_IZMIR_INT);
            DataTable izmGum = armaganService.CountDurumByBolge(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_GUMUSID, ProjeConstants.BOLGE_IZMIR_INT);
            DataTable izmBro = armaganService.CountDurumByBolge(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_BRONZID, ProjeConstants.BOLGE_IZMIR_INT);

            FillTable(izmAlt, IzmATable, IzmAltinBtn, IzmAltinEtiketBtn, IzmAltinDurumChk);
            FillTable(izmGum, IzmGTable, IzmGumusBtn, IzmGumusEtiketBtn, IzmGumusDurumChk);
            FillTable(izmBro, IzmBTable, IzmBronzBtn, IzmBronzEtiketBtn, IzmBronzDurumChk);

            //Mersin
            DataTable merAlt = armaganService.CountDurumByBolge(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_ALTINID, ProjeConstants.BOLGE_MERSIN_INT);
            DataTable merGum = armaganService.CountDurumByBolge(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_GUMUSID, ProjeConstants.BOLGE_MERSIN_INT);
            DataTable merBro = armaganService.CountDurumByBolge(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_BRONZID, ProjeConstants.BOLGE_MERSIN_INT);

            FillTable(merAlt, MerATable, MerAltinBtn, MerAltinEtiketBtn, MerAltinDurumChk);
            FillTable(merGum, MerGTable, MerGumusBtn, MerGumusEtiketBtn, MerGumusDurumChk);
            FillTable(merBro, MerBTable, MerBronzBtn, MerBronzEtiketBtn, MerBronzDurumChk);

            //Erzurum
            DataTable erzAlt = armaganService.CountDurumByBolge(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_ALTINID, ProjeConstants.BOLGE_ERZURUM_INT);
            DataTable erzGum = armaganService.CountDurumByBolge(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_GUMUSID, ProjeConstants.BOLGE_ERZURUM_INT);
            DataTable erzBro = armaganService.CountDurumByBolge(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), ProjeConstants.ARMAGAN_BRONZID, ProjeConstants.BOLGE_ERZURUM_INT);

            FillTable(erzAlt, ErzATable, ErzAltinBtn, ErzAltinEtiketBtn, ErzAltinDurumChk);
            FillTable(erzGum, ErzGTable, ErzGumusBtn, ErzGumusEtiketBtn, ErzGumusDurumChk);
            FillTable(erzBro, ErzBTable, ErzBronzBtn, ErzBronzEtiketBtn, ErzBronzDurumChk);

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

            ErzAltinBtn.Enabled = false;
            ErzGumusBtn.Enabled = false;
            ErzBronzBtn.Enabled = false;
            ErzAltinEtiketBtn.Enabled = false;
            ErzGumusEtiketBtn.Enabled = false;
            ErzBronzEtiketBtn.Enabled = false;
            ErzAltinDurumChk.Checked = false;
            ErzGumusDurumChk.Checked = false;
            ErzBronzDurumChk.Checked = false;
            ErzAltinDurumChk.Enabled = false;
            ErzGumusDurumChk.Enabled = false;
            ErzBronzDurumChk.Enabled = false;
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
            SetFilter(AyDDL, SecilenAyQS, DateTime.Today.Month.ToString());
            SetFilter(YilDDL, SecilenYilQS, DateTime.Today.Year.ToString());
            SecilenAyQS = AyDDL.SelectedValue;
            SecilenYilQS = YilDDL.SelectedValue;
            SetFilter(GunDDL, SecilenGunQS, DateTime.Today.Day <= 15 ? "1" : "2");
            SecilenGunQS = GunDDL.SelectedValue;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl, false);
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
            });
        }

        private void SetSecilenBasTarBitTar()
        {
            int gun = GunDDL.SelectedItem.Value.ConvertToInt();
            int basgun= gun == 1 ? 1 : 16;

            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();

            DateTime bastar = DateTime.Today;
            DateTime bittar = DateTime.Today;

            if (ay == 0)
            {
                bastar = new DateTime(yil, 1, basgun);
                bittar = bastar.AddYears(1).AddDays(-1);

            }
            else
            {

                bastar = new DateTime(yil, ay, basgun);
                var bitgun = gun == 1 ? bastar.AddDays(14) : new DateTime( bastar.Year,bastar.Month,1).AddMonths(1).AddDays(-1);
                bittar = new DateTime(yil, ay, bitgun.Day);

            }
            SecilenBastarQS = bastar.ConvertToDatetimeEmptyIfNull();
            SecilenBittarQS = bittar.ConvertToDatetimeEmptyIfNull();
        }
        protected void GunDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("GunDDL_SelectedIndexChanged", () =>
            {
                SecilenGunQS = GunDDL.SelectedItem.Value.ToString();
                SetSecilenBasTarBitTar();
                FillDurumValues();
                //KayitGetir();
            });
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("AyDDL_SelectedIndexChanged", () =>
            {
                SecilenAyQS = AyDDL.SelectedItem.Value.ToString();
                SetSecilenBasTarBitTar();
                FillDurumValues();
                //KayitGetir();
            });
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteAction("YilDDL_SelectedIndexChanged", () =>
            {
                SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
                SetSecilenBasTarBitTar();
                FillDurumValues();
                //KayitGetir();
            });
        }
        protected void AnkAltinBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("AnkAltinBtn_Click", () =>
            {
                BasimaGonder(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.ARMAGAN_ALTINID, AnkAltinDurumChk);
            });
        }
        protected void AnkGumusBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("AnkGumusBtn_Click", () =>
            {
                BasimaGonder(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.ARMAGAN_GUMUSID, AnkGumusDurumChk);
            });
        }
        protected void AnkBronzBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("AnkBronzBtn_Click", () =>
            {
                BasimaGonder(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.ARMAGAN_BRONZID, AnkBronzDurumChk);
            });
        }
        protected void IstAltinBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("IstAltinBtn_Click", () =>
            {
                BasimaGonder(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.ARMAGAN_ALTINID, IstAltinDurumChk);
            });
        }
        protected void IstGumusBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("IstGumusBtn_Click", () =>
            {
                BasimaGonder(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.ARMAGAN_GUMUSID, IstGumusDurumChk);
            });
        }
        protected void IstBronzBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("IstBronzBtn_Click", () =>
            {
                BasimaGonder(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.ARMAGAN_BRONZID, IstBronzDurumChk);
            });
        }
        protected void IzmAltinBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("IzmAltinBtn_Click", () =>
            {
                BasimaGonder(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.ARMAGAN_ALTINID, IzmAltinDurumChk);
            });
        }
        protected void IzmGumusBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("IzmGumusBtn_Click", () =>
            {
                BasimaGonder(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.ARMAGAN_GUMUSID, IzmGumusDurumChk);
            });
        }
        protected void IzmBronzBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("IzmBronzBtn_Click", () =>
            {
                BasimaGonder(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.ARMAGAN_BRONZID, IzmBronzDurumChk);
            });
        }
        protected void MerAltinBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("MerAltinBtn_Click", () =>
            {
                BasimaGonder(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.ARMAGAN_ALTINID, MerAltinDurumChk);
            });
        }
        protected void MerGumusBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("MerGumusBtn_Click", () =>
            {
                BasimaGonder(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.ARMAGAN_GUMUSID, MerGumusDurumChk);
            });
        }
        protected void MerBronzBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("MerBronzBtn_Click", () =>
            {
                BasimaGonder(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.ARMAGAN_BRONZID, MerBronzDurumChk);
            });
        }
        protected void ErzAltinBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ErzAltinBtn_Click", () =>
            {
                BasimaGonder(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.ARMAGAN_ALTINID, ErzAltinDurumChk);
            });
        }
        protected void ErzGumusBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ErzGumusBtn_Click", () =>
            {
                BasimaGonder(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.ARMAGAN_GUMUSID, ErzGumusDurumChk);
            });
        }
        protected void ErzBronzBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ErzBronzBtn_Click", () =>
            {
                BasimaGonder(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.ARMAGAN_BRONZID, ErzBronzDurumChk);
            });
        }
        private void BasimaGonder(int bolgeId, int armaganTipi, CheckBox checkBox)
        {
            var batch = new NbysDocumentWriteService(LoggingContext).Prepare(SecilenBastarQS.ConvertToDatetime(), SecilenBittarQS.ConvertToDatetime(), armaganTipi, bolgeId);
            var queryString = string.Format("?Bastar={0}&Bittar={1}&ArmaganTanimId={2}&BolgeId={3}", SecilenBastarQS, SecilenBittarQS, armaganTipi, bolgeId);
            NbysDocumentFeedback.Run(LoggingContext, batch,
                () => RedirectToPage(ProjeConstants.PAGE_BERATBELGESI_VIEWER + queryString + "&target=_blank"), checkBox.Checked,
                FillDurumValues, "Rapor açma isteği hazırlandı. Baskı ve teslimat sonucunu rapor ekranından kontrol edin.");
        }
        private void EtiketleriBasimaGonder(int bolgeId, int armaganTipi)
        {
            FillDurumValues();
            var queryString = string.Format("?Bastar={0}&Bittar={1}&ArmaganTanimId={2}&BolgeId={3}", SecilenBastarQS, SecilenBittarQS, armaganTipi, bolgeId);
            RedirectToPage(ProjeConstants.PAGE_BERATETIKET_VIEWER + queryString);
        }
        protected void AnkAltinEtiketBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("AnkAltinEtiketBtn_Click", () =>
            {
                EtiketleriBasimaGonder(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.ARMAGAN_ALTINID);
            });
        }

        protected void AnkGumusEtiketBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("AnkGumusEtiketBtn_Click", () =>
            {
                EtiketleriBasimaGonder(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.ARMAGAN_GUMUSID);
            });
        }

        protected void AnkBronzEtiketBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("AnkBronzEtiketBtn_Click", () =>
            {
                EtiketleriBasimaGonder(ProjeConstants.BOLGE_ANKARA_INT, ProjeConstants.ARMAGAN_BRONZID);
            });
        }

        protected void IstAltinEtiketBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("IstAltinEtiketBtn_Click", () =>
            {
                EtiketleriBasimaGonder(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.ARMAGAN_ALTINID);
            });
        }

        protected void IstGumusEtiketBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("IstGumusEtiketBtn_Click", () =>
            {
                EtiketleriBasimaGonder(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.ARMAGAN_GUMUSID);
            });
        }

        protected void IstBronzEtiketBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("IstBronzEtiketBtn_Click", () =>
            {
                EtiketleriBasimaGonder(ProjeConstants.BOLGE_ISTANBUL_INT, ProjeConstants.ARMAGAN_BRONZID);
            });
        }
        protected void IzmAltinEtiketBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("IzmAltinEtiketBtn_Click", () =>
            {
                EtiketleriBasimaGonder(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.ARMAGAN_ALTINID);
            });
        }

        protected void IzmGumusEtiketBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("IzmGumusEtiketBtn_Click", () =>
            {
                EtiketleriBasimaGonder(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.ARMAGAN_GUMUSID);
            });
        }

        protected void IzmBronzEtiketBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("IzmBronzEtiketBtn_Click", () =>
            {
                EtiketleriBasimaGonder(ProjeConstants.BOLGE_IZMIR_INT, ProjeConstants.ARMAGAN_BRONZID);
            });
        }

        protected void MerAltinEtiketBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("MerAltinEtiketBtn_Click", () =>
            {
                EtiketleriBasimaGonder(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.ARMAGAN_ALTINID);
            });
        }

        protected void MerGumusEtiketBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("MerGumusEtiketBtn_Click", () =>
            {
                EtiketleriBasimaGonder(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.ARMAGAN_GUMUSID);
            });
        }

        protected void MerBronzEtiketBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("MerBronzEtiketBtn_Click", () =>
            {
                EtiketleriBasimaGonder(ProjeConstants.BOLGE_MERSIN_INT, ProjeConstants.ARMAGAN_BRONZID);
            });
        }

        protected void ErzAltinEtiketBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ErzAltinEtiketBtn_Click", () =>
            {
                EtiketleriBasimaGonder(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.ARMAGAN_ALTINID);
            });
        }

        protected void ErzGumusEtiketBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ErzGumusEtiketBtn_Click", () =>
            {
                EtiketleriBasimaGonder(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.ARMAGAN_GUMUSID);
            });
        }

        protected void ErzBronzEtiketBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ErzBronzEtiketBtn_Click", () =>
            {
                EtiketleriBasimaGonder(ProjeConstants.BOLGE_ERZURUM_INT, ProjeConstants.ARMAGAN_BRONZID);
            });
        }
        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            //string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
            string newUrl = "http://tskgv-portal/YonetimBirimleri/BasinTanitimHalklaIliskilerSubesi/Sayfalar" + "/" + pageUrl;
            ResponseHelper.Redirect(newUrl, "_blank", "");
        }
    }
}
