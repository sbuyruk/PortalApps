using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.IletisimBilgileriListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class IletisimBilgileriListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public IletisimBilgileriListesiWP()
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
        private string DestinationAppQS
        {
            get
            {

                if (ViewState["DestinationApp"] == null)
                {
                    if (Page.Request.QueryString["DestinationApp"] != null)
                    {
                        ViewState["DestinationApp"] = Page.Request.QueryString["DestinationApp"];
                    }
                    else
                    {
                        ViewState["DestinationApp"] = string.Empty;
                    }
                }
                return ViewState["DestinationApp"].ToString();
            }

            set
            {
                ViewState["DestinationApp"] = value;
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
        private string IzinTanimIdQS
        {
            get
            {

                if (ViewState["IzinTanimId"] == null)
                {
                    if (Page.Request.QueryString["IzinTanimId"] != null)
                    {
                        ViewState["IzinTanimId"] = Page.Request.QueryString["IzinTanimId"];
                    }
                    else
                    {
                        ViewState["IzinTanimId"] = string.Empty;
                    }
                }
                return ViewState["IzinTanimId"].ToString();
            }

            set
            {
                ViewState["IzinTanimId"] = value;
            }
        }
        private string AuthQS
        {
            get
            {

                if (ViewState["Auth"] == null || string.IsNullOrEmpty(ViewState["Auth"].ToString()))
                {
                    if (Page.Request.QueryString["Auth"] != null)
                    {
                        ViewState["Auth"] = Page.Request.QueryString["Auth"];
                    }
                    else
                    {
                        ViewState["Auth"] = string.Empty;
                    }
                }
                return ViewState["Auth"].ToString();
            }

            set
            {
                ViewState["Auth"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    FillBirimDDL();
                    int birimId = BirimDDL.SelectedItem == null ? 1 : BirimDDL.SelectedItem.Value.ConvertToInt();
                    FillIletisimTable(birimId);
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exHelper = new ExceptionHelper(exception);
                exHelper.PublishException();
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        private void FillBirimDDL()
        {
            BirimDDL.Items.Clear();
            BirimTanim birimDao = new BirimTanim();
            List<BirimTanim> list = birimDao.SelectAll<BirimTanim>();
            //ListItem bosLi = new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI_INT.ToString());
            //BirimDDL.Items.Add(bosLi);
            foreach (BirimTanim gr in list)
            {
                ListItem li = new ListItem(gr.Adi.ReturnEmptyIfNull().ToString(), gr.Id.ReturnZeroIfNull().ToString());
                BirimDDL.Items.Add(li);
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
        private string parentBirimGetir(int parentId)
        {
            string retVal = parentId + ",";
            BirimTanim bt = new BirimTanim();
            List<BirimTanim> list = bt.SelectByParentId(parentId);
            foreach (BirimTanim item in list)
            {
                //retVal += item.Id + ",";
                string val = parentBirimGetir(item.Id);
                if (string.IsNullOrEmpty(val))
                {
                    return retVal;
                }
                retVal += val;
            }
            return retVal;
        }
        private string BirimListesiGetir(int birimId)
        {
            string birimIdStr = string.Empty;
            BirimTanim bt = new BirimTanim();
            bt = bt.Select<BirimTanim>(birimId);
            if (bt != null)
            {
                string birim = parentBirimGetir(birimId);
                birimIdStr = string.IsNullOrEmpty(birim) ? "" : birim.Substring(0, birim.Length - 1);
            }
            return birimIdStr;
        }
        private void IletisimTableHeaders()
        {
            IletisimTable.Rows.Clear();
            string birim = BirimDDL.SelectedItem.Text;
            int birimId = BirimDDL.SelectedItem.Value.ConvertToInt();

            TableHeaderRow th = new TableHeaderRow();
            th.CssClass = "sticky-top";
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell adiSoyadiCell = new TableHeaderCell();
            adiSoyadiCell.Text = "Adı Soyadı";
            TableHeaderCell goreviCell = new TableHeaderCell();
            goreviCell.Text = "Görevi";
            TableHeaderCell dahiliTelCell = new TableHeaderCell();
            dahiliTelCell.Text = "Dahili Tel.";
            TableHeaderCell evtTelCell = new TableHeaderCell();
            evtTelCell.Text = "Ev Tel.";
            TableHeaderCell cepTelCell = new TableHeaderCell();
            cepTelCell.Text = "Cep Tel";
            TableHeaderCell kurumEPostaCell = new TableHeaderCell();
            kurumEPostaCell.Text = "Kurum E-Posta";

            th.Controls.Add(siraCell);
            th.Controls.Add(adiSoyadiCell);
            th.Controls.Add(goreviCell);

            //th.Controls.Add(izinDonemiCell);
            th.Controls.Add(dahiliTelCell);
            th.Controls.Add(evtTelCell);
            th.Controls.Add(cepTelCell);
            th.Controls.Add(kurumEPostaCell);

            IletisimTable.Controls.Add(th);
        }
        private void FillIletisimTable(int birimId)
        {
            IletisimTableHeaders();
            Personel personelDao = new Personel();
            string birimListesiStr = BirimListesiGetir(birimId);
            DataTable dataTable = personelDao.SelectCalisanPersonelByBirimReturnDT(birimListesiStr);

            int birimIdTemp = 0;
            int sira = 0;
            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {

                    int personelId = dataRow["PersonelId"].ConvertToInt();
                    string adiSoyadi = dataRow["Adi"].ToString() + " " + dataRow["Soyadi"].ToString();
                    string gorev = dataRow["Gorev"].ToString();
                    string birim = dataRow["Birim"].ToString();
                    birimId = dataRow["BirimId"].ConvertToInt();
                    string dahiliTelefonu = dataRow["DahiliTelefonu"].ToString();
                    string evTelefonu = dataRow["EvTelefonu"].ToString();
                    string cepTelefonu = dataRow["CepTelefonu"].ToString();
                    string internetEPosta = dataRow["InternetEPosta"].ToString();
                    TableCell birimCell = new TableCell();
                    if (birimIdTemp != birimId)
                    {
                        birimIdTemp = birimId;
                        TableRow rowBirim = new TableRow();

                        birimCell.ColumnSpan = 7;
                        birimCell.Text = birim;
                        birimCell.Font.Bold = true;
                        birimCell.BackColor = System.Drawing.Color.DarkGray;
                        rowBirim.Controls.Add(birimCell);
                        IletisimTable.Controls.Add(rowBirim);
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

                    TableCell DahiliTelCell = new TableCell();
                    DahiliTelCell.Text = dahiliTelefonu;
                    row.Controls.Add(DahiliTelCell);

                    TableCell EvtTelCell = new TableCell();
                    EvtTelCell.Text = evTelefonu;
                    row.Controls.Add(EvtTelCell);

                    TableCell CepTelefonuCell = new TableCell();
                    CepTelefonuCell.Text = cepTelefonu;
                    row.Controls.Add(CepTelefonuCell);

                    TableCell InternetEPostaCell = new TableCell();
                    InternetEPostaCell.Text = internetEPosta;
                    row.Controls.Add(InternetEPostaCell);

                    IletisimTable.Controls.Add(row);

                }
            }


        }

        protected void ExportToExcel()
        {
            string filename = "IletisimBilgileri" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            IletisimTable.RenderControl(hw);
            //Write the HTML back to the browser.
            //Response.ContentType = application/vnd.ms-excel;
            Page.Response.ContentType = "application/ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            string s = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\n<head>\n<title></title>\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1254\" />\n<style>\n</style>\n</head>\n<body>\n";
            Page.Response.Write(s + tw.ToString());
            Page.Response.End();

        }
        protected void BirimDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillIletisimTable(BirimDDL.SelectedItem.Value.ConvertToInt());
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            FillIletisimTable(BirimDDL.SelectedItem.Value.ConvertToInt());
            ExportToExcel();
        }
    }
}
