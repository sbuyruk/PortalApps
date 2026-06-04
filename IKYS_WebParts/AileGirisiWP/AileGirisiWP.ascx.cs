using Model.IKYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.AileGirisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class AileGirisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public AileGirisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string PersonelIdQS
        {
            get
            {

                if (ViewState["PersonelId"] == null)
                {
                    if (Page.Request.QueryString["PersonelId"] != null)
                    {
                        ViewState["PersonelId"] = Page.Request.QueryString["PersonelId"];
                    }
                    else
                    {
                        ViewState["PersonelId"] = string.Empty;
                    }
                }
                return ViewState["PersonelId"].ToString();
            }

            set
            {
                ViewState["PersonelId"] = value;
            }
        }
        private string SenderAppQS
        {
            get
            {

                if (ViewState["SenderApp"] == null)
                {
                    if (Page.Request.QueryString["SenderApp"] != null)
                    {
                        ViewState["SenderApp"] = Page.Request.QueryString["SenderApp"];
                    }
                    else
                    {
                        ViewState["SenderApp"] = string.Empty;
                    }
                }
                return ViewState["SenderApp"].ToString();
            }

            set
            {
                ViewState["SenderApp"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillYakinlikDerecesiDDL();
                FillMeslekDDL();
            }
            Personel personelDao = new Personel();
            Personel personel = personelDao.Select<Personel>(PersonelIdQS.ConvertToInt());
            if (personel != null)
            {
                FillAileBilgileriTable(personel);
            }
        }
        private void FillAileBilgileriTable(Personel personel)
        {
            ClearTableRows();
            TitleLbl.Text = personel.Adi + " " + personel.Soyadi + " Aile Bilgileri";
            PersonelIdLbl.Text = personel.Id + "";
            //Column headers
            HeaderCell1.Text = "Adi Soyadi";
            HeaderCell1.Visible = true;
            HeaderCell2.Text = "Yak.Derecesi";
            HeaderCell2.Visible = true;
            HeaderCell3.Text = "Dog.Tarihi";
            HeaderCell3.Visible = true;
            HeaderCell4.Text = "Okul";
            HeaderCell4.Visible = true;
            HeaderCell5.Text = "Meslek";
            HeaderCell5.Visible = true;
            HeaderCell6.Text = "Telefon";
            HeaderCell6.Visible = true;

            Aile aileDao = new Aile();
            List<Aile> list = aileDao.SelectByPersonelId(personel.Id);
            int SiraNo = 1;
            foreach (Aile aile in list)
            {
                TableRow row = new TableRow();

                TableCell PupUpCell0 = new TableCell();

                PupUpCell0.Text = SiraNo++ + "";
                row.Controls.Add(PupUpCell0);

                TableCell AdiSoyadiCell = new TableCell();
                AdiSoyadiCell.Text = aile.Adi + " " + aile.Soyadi;
                row.Controls.Add(AdiSoyadiCell);

                TableCell YakinlikCell = new TableCell();

                string yakinlikDerecesi = aile.YakinlikDerecesi.ToString();
                if (yakinlikDerecesi.Equals("1"))
                    yakinlikDerecesi = ProjeConstants.PER_YAKINLIKDERECESI_ES;
                else if (yakinlikDerecesi.Equals("2"))
                    yakinlikDerecesi = ProjeConstants.PER_YAKINLIKDERECESI_COCUK;
                YakinlikCell.Text = yakinlikDerecesi;
                row.Controls.Add(YakinlikCell);

                TableCell DogumTarCell = new TableCell();
                DogumTarCell.Text = aile.DogumTar.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(DogumTarCell);

                TableCell OkulCell = new TableCell();
                OkulCell.Text = aile.Okul;
                row.Controls.Add(OkulCell);

                TableCell MeslekCell = new TableCell();
                Meslek meslekDao = new Meslek();
                Meslek meslek = meslekDao.Select<Meslek>(aile.Meslek);
                MeslekCell.Text = meslek.Adi;
                row.Controls.Add(MeslekCell);

                TableCell TelefonCell = new TableCell();
                TelefonCell.Text = aile.Telefon;
                row.Controls.Add(TelefonCell);

                TableCell SilCell = new TableCell();
                LinkButton SilBtn = new LinkButton();
                SilBtn.Text = "Sil";
                SilBtn.ID = "SilBtn" + SiraNo;
                TableUpdatePanel.ContentTemplateContainer.Controls.Add(SilBtn);
                SilBtn.CssClass = "btn btn-outline-danger";
                SilBtn.Click += delegate
                {
                    aile.Delete();
                    FillAileBilgileriTable(personel);
                };
                SilCell.Controls.Add(SilBtn);
                row.Controls.Add(SilCell);

                AileTable.Controls.Add(row);
            }

        }
        private void FillYakinlikDerecesiDDL()
        {
            YakDerecesiDDL.Items.Clear();
            ListItem li = new ListItem("", "");
            ListItem li1 = new ListItem(ProjeConstants.PER_YAKINLIKDERECESI_ES, ProjeConstants.PER_YAKINLIKDERECESI_ES_INT.ToString());
            ListItem li2 = new ListItem(ProjeConstants.PER_YAKINLIKDERECESI_COCUK, ProjeConstants.PER_YAKINLIKDERECESI_COCUK_INT.ToString());
            YakDerecesiDDL.Items.Add(li);
            YakDerecesiDDL.Items.Add(li1);
            YakDerecesiDDL.Items.Add(li2);
        }
        private void FillMeslekDDL()
        {
            YakMeslekDDL.Items.Clear();
            Meslek meslekDao = new Meslek();
            List<Meslek> list = meslekDao.SelectAll<Meslek>();
            foreach (Meslek meslek in list)
            {
                ListItem li = new ListItem(meslek.Adi, meslek.Id.ReturnZeroIfNull().ToString());
                YakMeslekDDL.Items.Add(li);
            }
        }
        private void ClearTableRows()
        {
            TableHeaderRow th = (TableHeaderRow)AileTable.Rows[0];
            AileTable.Rows.Clear();
            AileTable.Rows.Add(th);
        }
        private bool validateItems()
        {
            bool validated = false;
            if (!string.IsNullOrEmpty(YakAdiTxt.Text)
                && !string.IsNullOrEmpty(YakSoyadiTxt.Text))
                validated = true;
            return validated;
        }
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            string newUrl = string.Empty;
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            if (SenderAppQS.Equals("PL"))
                newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_PERSONEL_LIST;
            if (SenderAppQS.Equals("PD"))
                newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_PERSONEL_EDIT + "?PersonelId=" + PersonelIdQS + "&DestinationApp=PerD";
            Page.Response.Redirect(newUrl);
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }

        protected void YakinEkleBtn_Click(object sender, EventArgs e)
        {
            if (validateItems())
            {
                Aile aile = new Aile();

                aile.PersonelId = PersonelIdQS.ConvertToInt();
                aile.Adi = YakAdiTxt.Text;
                aile.Soyadi = YakSoyadiTxt.Text;
                aile.TcKimlikNo = YakTcKimlikNoTxt.Text;
                aile.YakinlikDerecesi = YakDerecesiDDL.SelectedItem.Value.ConvertToInt();
                aile.DogumTar = YakDogumTarTxt.Value.ConvertToDatetime();
                //aile.Tahsil = YakTahsilDDL.SelectedValue;
                aile.Meslek = YakMeslekDDL.SelectedItem.Value.ConvertToInt();
                aile.Telefon = YakTelefonTxt.Text;
                aile.Okul = OkulTxt.Text;
                aile.Id = aile.Save();
                Personel personelDao = new Personel();
                Personel personel = personelDao.Select<Personel>(PersonelIdQS.ConvertToInt());
                if (personel != null)
                {
                    FillAileBilgileriTable(personel);
                }
            }
        }
    }
}
