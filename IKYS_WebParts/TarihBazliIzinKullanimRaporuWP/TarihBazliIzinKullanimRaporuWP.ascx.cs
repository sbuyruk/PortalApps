using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.TarihBazliIzinKullanimRaporuWP
{
    [ToolboxItemAttribute(false)]
    public partial class TarihBazliIzinKullanimRaporuWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TarihBazliIzinKullanimRaporuWP()
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
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUser();
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
                    fillIzinTanim();
                    YilDDLDoldur();
                    FillIzinTable(IzinTanimDDL.SelectedItem.Value.ConvertToInt());
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exHelper = new ExceptionHelper(exception);
                exHelper.PublishException();
            }
        }

        private void YilDDLDoldur()
        {
            int buYil = DateTime.Today.Year;
            int gecenYil = DateTime.Today.AddYears(-1).Year;
            int oncekiYil = DateTime.Today.AddYears(-2).Year;
            ListItem li = new ListItem(buYil.ToString());
            ListItem li1 = new ListItem(gecenYil.ToString());
            ListItem li2 = new ListItem(oncekiYil.ToString());
            YilDDL.Items.Add(li);
            YilDDL.Items.Add(li1);
            YilDDL.Items.Add(li2);
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        private void FillIzinTable(int izinTipi)
        {
            IzinTableHeaders();
            Personel personelDao = new Personel();
            DataTable dataTable = null;
            string birimListesiStr = string.Empty;
            if (AuthQS.Equals("IKYS"))
            {
                BirimTanim rootBirimTanim = new BirimTanim();
                rootBirimTanim = rootBirimTanim.SelectRoot();
                Personel personel = new Personel();
                personel = personel.Select<Personel>(rootBirimTanim.AmirId);
                birimListesiStr = BirimListesiGetir(personel, rootBirimTanim);
            }
            else
            {
                Personel personel = PersonelGetir();
                birimListesiStr = BirimListesiGetir(personel, null);
            }
            dataTable = personelDao.SelectCalisanPersonelByBirimReturnDT(birimListesiStr);
            int birimIdTemp = 0;
            int sira = 0;
            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {

                    int personelId = dataRow["PersonelId"].ConvertToInt();
                    string adiSoyadi = dataRow["Adi"].ToString() + " " + dataRow["Soyadi"].ToString();
                    string gorev = dataRow["Gorev"].ToString();
                    string izinDonemiBasTar = dataRow["IzinDonemiBasTar"].ConvertToDatetimeEmptyIfNull();
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
                        IzinTable.Controls.Add(rowBirim);
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
                    if (izinTipi != ProjeConstants.IZINTIPI_MAZERET_INT)
                    {
                        TableCell iseGirisCell = new TableCell();
                        iseGirisCell.Text = izinDonemiBasTar;
                        row.Controls.Add(iseGirisCell);
                        birimCell.ColumnSpan = 7;
                    }
                    IzinDonem izinDonemi = new IzinDonem();
                    DateTime now = DateTime.Now;

                    //int yil = YilDDL.SelectedItem.Value.ConvertToInt();
                    //
                    int yil = 2020;
                    DateTime raporTarihi = new DateTime(yil, 12, 01);

                    DateTime gecerliTarih = now.Year == yil ? now : raporTarihi;
                    izinDonemi = izinDonemi.SelectByIzinTarihi(personelId, izinTipi, gecerliTarih);
                    string izinDonemiStr = string.Empty;
                    string izinHakki = string.Empty;
                    string kullanilanIzin = string.Empty;
                    string kalanIzin = string.Empty;
                    if (izinDonemi != null)
                    {
                        izinDonemiStr = izinDonemi.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + izinDonemi.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                        if (izinDonemi.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                        {
                            TimeSpan izinHakkiTS = new TimeSpan(0, 0, 0);
                            izinHakkiTS = izinDonemi.IzinHakki.ConvertToTimeSpan();
                            izinHakki = izinDonemi.IzinHakki.ConvertToTimeSpanReturnInHHmm();
                            kullanilanIzin = izinDonemi.KullanilanIzin.ConvertToTimeSpanReturnInHHmm();
                            kalanIzin = izinDonemi.KalanIzin.ConvertToTimeSpanReturnInHHmm();
                        }
                        else
                        {
                            izinHakki = izinDonemi.IzinHakki + " " + izinDonemi.Birim;
                            kullanilanIzin = izinDonemi.KullanilanIzin + " " + izinDonemi.Birim;
                            kalanIzin = izinDonemi.KalanIzin + " " + izinDonemi.Birim;
                        }



                    }
                    TableCell IzinDonemiCell = new TableCell();

                    IzinDonemiCell.Text = izinDonemiStr;
                    //row.Controls.Add(IzinDonemiCell);

                    TableCell IzinHakkiCell = new TableCell();
                    IzinHakkiCell.Text = izinHakki;
                    row.Controls.Add(IzinHakkiCell);

                    TableCell KullanilanIzinCell = new TableCell();

                    KullanilanIzinCell.Text = kullanilanIzin;
                    row.Controls.Add(KullanilanIzinCell);

                    TableCell KalanIzinCell = new TableCell();
                    KalanIzinCell.Text = kalanIzin;
                    if (kalanIzin.Contains("-"))
                    {
                        KalanIzinCell.ForeColor = System.Drawing.Color.Red;
                        KalanIzinCell.Font.Bold = true;
                    }

                    row.Controls.Add(KalanIzinCell);
                    IzinTable.Controls.Add(row);

                }
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
        private string BirimListesiGetir(Personel personel, BirimTanim birimTanim)
        {
            string birimIdStr = string.Empty;

            if (personel != null)
            {
                IsBilgileri ib = new IsBilgileri();
                ib = ib.SelectByPersonelId(personel.Id);
                if (ib != null)
                {
                    int birimId = birimTanim != null ? birimTanim.Id : ib.BirimId;
                    BirimTanim bt = new BirimTanim();
                    bt = birimTanim != null ? birimTanim : bt.Select<BirimTanim>(birimId);
                    if ((bt != null) && (bt.AmirId == personel.Id))
                    {
                        string birim = parentBirimGetir(birimId);
                        birimIdStr = string.IsNullOrEmpty(birim) ? "" : birim.Substring(0, birim.Length - 1);
                    }

                }
            }
            return birimIdStr;
        }
        private void IzinTableHeaders()
        {
            IzinTable.Rows.Clear();
            TableHeaderRow thbaslik = new TableHeaderRow();
            TableHeaderCell baslikCell = new TableHeaderCell();
            string izinTipi = IzinTanimDDL.SelectedItem.Text;
            int izinTipiId = IzinTanimDDL.SelectedItem.Value.ConvertToInt();
            DateTime now = DateTime.Now;
            //int yil = YilDDL.SelectedItem.Value.ConvertToInt();
            int yil = 2020;
            DateTime raporTarihi = new DateTime(yil, 12, 1);

            DateTime gecerliTarih = now.Year == yil ? now : raporTarihi;

            baslikCell.Text = izinTipi.ToUpper() + " İZİN RAPORU " + "(" + gecerliTarih.ConvertToDatetimeEmptyIfNull() + ")";
            baslikCell.Font.Bold = true;
            baslikCell.ColumnSpan = 6;
            thbaslik.HorizontalAlign = HorizontalAlign.Center;
            thbaslik.Controls.Add(baslikCell);
            IzinTable.Controls.Add(thbaslik);

            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sıra";
            TableHeaderCell adiSoyadiCell = new TableHeaderCell();
            adiSoyadiCell.Text = "Adı Soyadı";
            TableHeaderCell goreviCell = new TableHeaderCell();
            goreviCell.Text = "Görevi";
            TableHeaderCell izinBasTarCell = new TableHeaderCell();
            izinBasTarCell.Text = "İşe Giriş Tarihi";
            TableHeaderCell izinDonemiCell = new TableHeaderCell();
            izinDonemiCell.Text = "İzin Dönemi";
            TableHeaderCell izinHakkiCell = new TableHeaderCell();
            izinHakkiCell.Text = "İzin Hakkı";
            TableHeaderCell kullanilanIzinCell = new TableHeaderCell();
            kullanilanIzinCell.Text = "Kullanılan İzin";
            TableHeaderCell kalanIzinCell = new TableHeaderCell();
            kalanIzinCell.Text = "Kalan İzin";

            th.Controls.Add(siraCell);
            th.Controls.Add(adiSoyadiCell);
            th.Controls.Add(goreviCell);
            if (izinTipiId != ProjeConstants.IZINTIPI_MAZERET_INT)
            {
                th.Controls.Add(izinBasTarCell);
                baslikCell.ColumnSpan = 7;
            }

            //th.Controls.Add(izinDonemiCell);
            th.Controls.Add(izinHakkiCell);
            th.Controls.Add(kullanilanIzinCell);
            th.Controls.Add(kalanIzinCell);

            IzinTable.Controls.Add(th);
        }
        private void fillIzinTanim()//sadece Ücretli ve mazeret izinleri için çalışsın
        {
            IzinTanimDDL.Items.Clear();
            IzinTanim izinTanim = new IzinTanim();
            ListItem li = new ListItem(ProjeConstants.IZINTIPI_UCRETLI, ProjeConstants.IZINTIPI_UCRETLI_INT.ToString());
            IzinTanimDDL.Items.Add(li);
            ListItem li1 = new ListItem(ProjeConstants.IZINTIPI_MAZERET, ProjeConstants.IZINTIPI_MAZERET_INT.ToString());
            IzinTanimDDL.Items.Add(li1);
        }

        protected void IzinTanimDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillIzinTable(IzinTanimDDL.SelectedItem.Value.ConvertToInt());
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillIzinTable(IzinTanimDDL.SelectedItem.Value.ConvertToInt());
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            FillIzinTable(IzinTanimDDL.SelectedItem.Value.ConvertToInt());
            ExportToExcel();
        }
        protected void ExportToExcel()
        {
            string filename = "IzinKullanimRaporu" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            IzinTable.RenderControl(hw);
            //Write the HTML back to the browser.
            //Response.ContentType = application/vnd.ms-excel;
            Page.Response.ContentType = "application/ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            string s = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\n<head>\n<title></title>\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1254\" />\n<style>\n</style>\n</head>\n<body>\n";
            Page.Response.Write(s + tw.ToString());
            Page.Response.End();

        }
    }
}