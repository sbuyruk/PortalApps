using Model.IKYS;
using Model.Ortak;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.KisiselSayfaListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KisiselSayfaListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KisiselSayfaListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
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
        private string CurrentUserName
        {
            get
            {

                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUserLoginName();
                }
                return ViewState["CurrentUserName"].ToString();
            }

            set
            {
                ViewState["CurrentUserName"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                FillPersonelTable();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        private void PersonelTableHeaders()
        {
            PersonelTable.Rows.Clear();

            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell adiSoyadiCell = new TableHeaderCell();
            adiSoyadiCell.Text = "Adı Soyadı";
            TableHeaderCell goreviCell = new TableHeaderCell();
            goreviCell.Text = "Görevi";
            TableHeaderCell unvanCell = new TableHeaderCell();
            unvanCell.Text = "Ünvanı";
            TableHeaderCell iseBasTarCell = new TableHeaderCell();
            iseBasTarCell.Text = "İşe Başlama tarihi";
            TableHeaderCell kisiKartiCell = new TableHeaderCell();
            kisiKartiCell.Text = "Kişi Kartı";

            th.Controls.Add(siraCell);
            th.Controls.Add(adiSoyadiCell);
            th.Controls.Add(goreviCell);
            th.Controls.Add(unvanCell);
            th.Controls.Add(iseBasTarCell);
            th.Controls.Add(kisiKartiCell);

            PersonelTable.Controls.Add(th);
        }
        private void FillPersonelTable()
        {
            PersonelTableHeaders();

            DataTable dataTable = GetData();


            int birimIdTemp = 0;
            int sira = 0;
            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {

                    int personelId = dataRow["PersonelId"].ConvertToInt();
                    string adiSoyadi = dataRow["Adi"].ToString() + " " + dataRow["Soyadi"].ToString();
                    string gorev = dataRow["GorevKisa"].ToString();
                    string unvan = dataRow["Unvan"].ToString();
                    string iseBaslamaTar = dataRow["IseBaslamaTar"].ConvertToDatetimeEmptyIfNull();
                    string birim = dataRow["Birim"].ToString();
                    int birimId = dataRow["BirimId"].ConvertToInt();
                    TableCell birimCell = new TableCell();
                    if (birimIdTemp != birimId)
                    {
                        birimIdTemp = birimId;
                        TableRow rowBirim = new TableRow();

                        birimCell.ColumnSpan = 6;
                        birimCell.Text = birim;
                        birimCell.Font.Bold = true;
                        birimCell.BackColor = System.Drawing.Color.DarkGray;
                        rowBirim.Controls.Add(birimCell);
                        PersonelTable.Controls.Add(rowBirim);
                    }


                    TableRow row = new TableRow();

                    TableCell siraCell = new TableCell();
                    siraCell.Text = (++sira).ToString();
                    row.Controls.Add(siraCell);

                    TableCell adiSoyadiCell = new TableCell();
                    adiSoyadiCell.Text = adiSoyadi;
                    row.Controls.Add(adiSoyadiCell);

                    TableCell gorevCell = new TableCell();
                    gorevCell.Text = gorev;
                    row.Controls.Add(gorevCell);

                    TableCell unvanCell = new TableCell();
                    unvanCell.Text = unvan;
                    row.Controls.Add(unvanCell);

                    TableCell iseBaslamaTarCell = new TableCell();
                    iseBaslamaTarCell.Text = iseBaslamaTar;
                    row.Controls.Add(iseBaslamaTarCell);

                    TableCell kisiselSayfaCell = new TableCell();
                    LinkButton KisiselSayfaBtn = new LinkButton();
                    KisiselSayfaBtn.Text = "Kişi Kartı";
                    KisiselSayfaBtn.ID = "KisiselSayfaBtn" + sira;
                    TableUpdatePanel.ContentTemplateContainer.Controls.Add(KisiselSayfaBtn);
                    KisiselSayfaBtn.CssClass = "btn btn-outline-primary";
                    KisiselSayfaBtn.Click += delegate
                    {
                        try
                        {
                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                            string pageUrl = ProjeConstants.PAGE_KISISELSAYFA + "?SenderApp=KSL&PersonelId=" + personelId;// eski izinler de görünsün istenirse Auth=IKYS& eklenmeli
                            RedirectToPage(pageUrl);
                        }
                        catch (Exception exception)
                        {
                            ExceptionHelper ex = new ExceptionHelper(exception);
                            ex.PublishException();
                        }
                    };
                    kisiselSayfaCell.Controls.Add(KisiselSayfaBtn);
                    row.Controls.Add(kisiselSayfaCell);

                    PersonelTable.Controls.Add(row);

                }
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
        private Personel PersonelGetir()
        {
            Personel personel = new Personel();

            if (!string.IsNullOrEmpty(PersonelIdQS))
            {
                personel = personel.Select<Personel>(PersonelIdQS.ConvertToInt());

            }
            else
            {
                string userName = CurrentUserName.Substring(CurrentUserName.LastIndexOf("\\") + 1, CurrentUserName.Length - CurrentUserName.LastIndexOf("\\") - 1);
                personel = personel.SelectByUserName(userName);
            }
            PersonelIdQS = personel.Id.ToString();
            return personel;
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

            GridView1.DataSource = GetData();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=PersonelListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
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

        private DataTable GetData()
        {
            Personel personel = PersonelGetir();
            string birimListesiStr = IKYSOrtak.BirimListesiGetir(personel);
            Personel personelDao = new Personel();
            DataTable dataTable = personelDao.SelectCalisanPersonelByBirimReturnDT(birimListesiStr);
            return dataTable;
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
