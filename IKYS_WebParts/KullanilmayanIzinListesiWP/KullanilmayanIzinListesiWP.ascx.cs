using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.KullanilmayanIzinListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KullanilmayanIzinListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KullanilmayanIzinListesiWP()
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
                    FillIzinTable();
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
        private void FillIzinTable()
        {
            //IzinTableHeaders();
            IzinTable.Rows.Clear();
            TableHeaderRow thbaslik = new TableHeaderRow();
            TableHeaderCell baslikCell = new TableHeaderCell();
            DateTime now = DateTime.Now;
            DateTime raporTarihi = DateTime.Today;

            baslikCell.Text = "KULLANILMAYAN IZINLER " + "(" + raporTarihi.ConvertToDatetimeEmptyIfNull() + ")";
            baslikCell.Font.Bold = true;
            baslikCell.ColumnSpan = 7;
            thbaslik.HorizontalAlign = HorizontalAlign.Center;
            thbaslik.Controls.Add(baslikCell);
            IzinTable.Controls.Add(thbaslik);
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

                    TableRow row1 = new TableRow();
                    TableHeaderCell siraCell = new TableHeaderCell();
                    siraCell.Text = (++sira).ToString();
                    row1.Controls.Add(siraCell);

                    TableHeaderCell adiSoyadiCell = new TableHeaderCell();
                    adiSoyadiCell.Text = adiSoyadi;
                    row1.Controls.Add(adiSoyadiCell);

                    TableHeaderCell iseGirisCell = new TableHeaderCell();
                    iseGirisCell.Text = izinDonemiBasTar;
                    row1.Controls.Add(iseGirisCell);
                    
                    TableHeaderCell izinDonemiCell = new TableHeaderCell();
                    izinDonemiCell.Text = "Izin Dönemi";
                    row1.Controls.Add(izinDonemiCell);

                    TableHeaderCell izinHakkiCell = new TableHeaderCell();
                    izinHakkiCell.Text = "Izin Hakki";
                    row1.Controls.Add(izinHakkiCell);

                    TableHeaderCell kullanilanIzinCell = new TableHeaderCell();
                    kullanilanIzinCell.Text = "Kullanilan Izin";
                    row1.Controls.Add(kullanilanIzinCell);

                    TableHeaderCell kalanIzinCell = new TableHeaderCell();
                    kalanIzinCell.Text = "Kalan Izin";
                    row1.Controls.Add(kalanIzinCell);

                    baslikCell.BorderStyle = BorderStyle.Solid;
                    siraCell.BorderStyle = BorderStyle.Solid;                    
                    adiSoyadiCell.BorderStyle = BorderStyle.Solid;
                    iseGirisCell.BorderStyle = BorderStyle.Solid;
                    izinDonemiCell.BorderStyle = BorderStyle.Solid;
                    izinHakkiCell.BorderStyle = BorderStyle.Solid;
                    kullanilanIzinCell.BorderStyle = BorderStyle.Solid;
                    kalanIzinCell.BorderStyle = BorderStyle.Solid;

                    IzinDonem izinDonemiDao = new IzinDonem();
                    List<IzinDonem> izinDonemiList = izinDonemiDao.SelectOncekiYillaraAitIzinDonemleri(personelId);
                    if (izinDonemiList.Count>0)
                    {
                        int rowspan = izinDonemiList.Count + 2;
                        siraCell.RowSpan = rowspan;
                        adiSoyadiCell.RowSpan = rowspan;
                        iseGirisCell.RowSpan = rowspan;
                        siraCell.Attributes.Add("class", "alert-scondary");
                        adiSoyadiCell.Attributes.Add("class", "alert-scondary");
                        iseGirisCell.Attributes.Add("class", "alert-scondary");
                        izinDonemiCell.Attributes.Add("class", "alert-scondary");
                        izinHakkiCell.Attributes.Add("class", "alert-scondary");

                        kullanilanIzinCell.Attributes.Add("class", "alert-scondary");
                        kalanIzinCell.Attributes.Add("class", "alert-scondary");
                        IzinTable.Controls.Add(row1);
                    }
                    else
                    {
                        continue;
                    }
                    int toplam = 0;
                    foreach (var izinDonemi in izinDonemiList)
                    {

                        string izinDonemiStr = string.Empty;
                        string izinHakkiStr = string.Empty;
                        string kullanilanIzinStr = string.Empty;
                        string kalanIzinStr = string.Empty;
                        if (izinDonemi != null)
                        {
                            izinDonemiStr = izinDonemi.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + izinDonemi.BitisTarihi.ConvertToDatetimeEmptyIfNull();

                            izinHakkiStr = izinDonemi.IzinHakki + " " + izinDonemi.Birim;
                            kullanilanIzinStr = izinDonemi.KullanilanIzin + " " + izinDonemi.Birim;
                            kalanIzinStr = izinDonemi.KalanIzin + " " + izinDonemi.Birim;
                            toplam += izinDonemi.KalanIzin.ConvertToInt();
                        }
                        TableCell IzinDonemiCell = new TableCell();
                        IzinDonemiCell.Text = izinDonemiStr;

                        TableCell IzinHakkiCell = new TableCell();
                        IzinHakkiCell.Text = izinHakkiStr;


                        TableCell KullanilanIzinCell = new TableCell();

                        KullanilanIzinCell.Text = kullanilanIzinStr;


                        TableCell KalanIzinCell = new TableCell();
                        KalanIzinCell.Text = kalanIzinStr;
                        if (kalanIzinStr.Contains("-"))
                        {
                            KalanIzinCell.ForeColor = System.Drawing.Color.Red;
                            KalanIzinCell.Font.Bold = true;
                        }


                        TableRow row = new TableRow();
                        row.HorizontalAlign = HorizontalAlign.Right;

                        IzinDonemiCell.BorderStyle = BorderStyle.Solid;
                        IzinHakkiCell.BorderStyle = BorderStyle.Solid;
                        KullanilanIzinCell.BorderStyle = BorderStyle.Solid;
                        KalanIzinCell.BorderStyle = BorderStyle.Solid;

                        row.Controls.Add(IzinDonemiCell);
                        row.Controls.Add(IzinHakkiCell);
                        row.Controls.Add(KullanilanIzinCell);
                        row.Controls.Add(KalanIzinCell);
                        IzinTable.Controls.Add(row);
                    }
                    TableFooterRow toplamRow = new TableFooterRow();
                    toplamRow.HorizontalAlign = HorizontalAlign.Right;

                    TableHeaderCell toplamLblCell = new TableHeaderCell();
                    toplamLblCell.BorderStyle = BorderStyle.Solid;
                    toplamLblCell.ColumnSpan = 3;
                    toplamLblCell.Text = "Toplam";
                    
                    TableHeaderCell toplamCell = new TableHeaderCell();
                    toplamCell.BorderStyle = BorderStyle.Solid;
                    toplamCell.Text = toplam.ToString()+" Gün"; 

                    toplamRow.Controls.Add(toplamLblCell);
                    toplamRow.Controls.Add(toplamCell);
                    IzinTable.Controls.Add(toplamRow);
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
            DateTime now = DateTime.Now;
            DateTime raporTarihi = DateTime.Today;

            baslikCell.Text = "KULLANILMAYAN IZINLER " + "(" + raporTarihi.ConvertToDatetimeEmptyIfNull() + ")";
            baslikCell.Font.Bold = true;
            baslikCell.ColumnSpan = 7;
            thbaslik.HorizontalAlign = HorizontalAlign.Center;
            thbaslik.Controls.Add(baslikCell);
            IzinTable.Controls.Add(thbaslik);

            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sira";
            TableHeaderCell adiSoyadiCell = new TableHeaderCell();
            adiSoyadiCell.Text = "Adi Soyadi";
            TableHeaderCell goreviCell = new TableHeaderCell();
            goreviCell.Text = "Görevi";
            TableHeaderCell izinBasTarCell = new TableHeaderCell();
            izinBasTarCell.Text = "Ise Giris Tarihi";
            TableHeaderCell izinDonemiCell = new TableHeaderCell();
            izinDonemiCell.Text = "Izin Dönemi";
            TableHeaderCell izinHakkiCell = new TableHeaderCell();
            izinHakkiCell.Text = "Izin Hakki";
            TableHeaderCell kullanilanIzinCell = new TableHeaderCell();
            kullanilanIzinCell.Text = "Kullanilan Izin";
            TableHeaderCell kalanIzinCell = new TableHeaderCell();
            kalanIzinCell.Text = "Kalan Izin";

            th.Controls.Add(siraCell);
            th.Controls.Add(adiSoyadiCell);
            th.Controls.Add(goreviCell);

            th.Controls.Add(izinBasTarCell);
            baslikCell.ColumnSpan = 8;

            th.Controls.Add(izinDonemiCell);
            th.Controls.Add(izinHakkiCell);
            th.Controls.Add(kullanilanIzinCell);
            th.Controls.Add(kalanIzinCell);

            IzinTable.Controls.Add(th);
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillIzinTable();
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            FillIzinTable();
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