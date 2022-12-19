using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace BTYS_Webparts.BolgeNakitBagisciListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BolgeNakitBagisciListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BolgeNakitBagisciListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SecilenIdQS
        {
            get
            {

                if (ViewState["SecilenId"] == null)
                {
                    if (Page.Request.QueryString["SecilenId"] != null)
                    {
                        ViewState["SecilenId"] = Page.Request.QueryString["SecilenId"];
                    }
                    else
                    {
                        ViewState["SecilenId"] = string.Empty;
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }
        private string BolgeQS
        {
            get
            {

                if (ViewState["Bolge"] == null)
                {
                    if (Page.Request.QueryString["Bolge"] != null)
                    {
                        ViewState["Bolge"] = Page.Request.QueryString["Bolge"];
                    }
                    else
                    {
                        ViewState["Bolge"] = string.Empty;
                    }
                }
                return ViewState["Bolge"].ToString();
            }

            set
            {
                ViewState["Bolge"] = value;
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
                if (!Page.IsPostBack)
                {
                    BolgeQS = IKYSOrtak.PersonelinBolgesiniGetir(CurrentUserName);
                    if (!string.IsNullOrEmpty(BolgeQS))

                    {
                        AyDDLDoldur();
                        YilDDLDoldur();
                    }
                    else
                        MessageHelper.PublishMessage("Bölgeniz Belirlenemedi", ProjeConstants.MESAJ_HATA);
                    TabloOlustur();
                }
               

            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void AyDDLDoldur()
        {
            AyDDL.Items.Clear();
            DateTime bugun = DateTime.Today;

            for (int i = 0; i < 12; i++)
            {
                DateTime tarih = bugun.AddMonths(-i); 
                ListItem li = new ListItem(tarih.ToString("MMMM") , tarih.ToString("MM"));
                AyDDL.Items.Add(li);
            }
        }
        private void YilDDLDoldur()
        {
            YilDDL.Items.Clear();
            DateTime bugun = DateTime.Today;

            for (int i = 0; i < 5; i++)
            {
                DateTime tarih = bugun.AddYears(-i); 
                ListItem li = new ListItem(tarih.Year.ToString(), tarih.Year.ToString());
                YilDDL.Items.Add(li);
            }
        }
        protected void CallDialog(object sender, EventArgs e)
        {
            /* Put all your Code here */


            // Define the name and type of the client scripts on the page.
            String csname1 = "PopupScript";
            Type cstype = this.GetType();

            // Get a ClientScriptManager reference from the Page class.
            ClientScriptManager cs = Page.ClientScript;

            // Check to see if the startup script is already registered.
            if (!cs.IsStartupScriptRegistered(cstype, csname1))
            {
                StringBuilder cstext1 = new StringBuilder();
                cstext1.Append(@"<script type=text/javascript>
                            ExecuteOrDelayUntilScriptLoaded(function(){
                                                            var options = {
                                                            title: 'My Dialog Title',
                                                            width: 400,
                                                            height: 600,
                                                            url: '" + ProjeConstants.PAGE_TASINMAZBAGISCI_KARTI + @"' };
                            
                                                            SP.UI.ModalDialog.showModalDialog(options);
                                                            }, 'sp.js'); </");
                cstext1.Append("script>");

                cs.RegisterStartupScript(cstype, csname1, cstext1.ToString());
            }

        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {

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
            List<NakitBagisci> list = new List<NakitBagisci>();
            GridView1.DataSource = list;
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=BagisciListesi.xls");
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
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<NakitBagisciListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
                jSon = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                jQuery(document).ready(function () {
                    jQuery('#CustomDataTable').DataTable({
                        'initComplete': function (settings, json) {//tablo yüklendiğinde
                            var api = this.api();
                            var row = api.row(function (idx, data, node) { //secilen toplantıya gider
                                return data['Secildi'] == true;
                            });
                            if (row.length > 0) {
                                row.select()
                                    .show()
                                    .draw(false);
                            }
                        },
                        data: " + jsonData + @",
                        columns: [
                            { data: 'AdiSoyadi' },
                            { data: 'BagisTarihi' },
                            { data: 'BagisMiktari' },
                            { data: 'Telefon' },
                            { data: 'Adres' },
                            { data: 'Ili' },
                            { data: 'Ilcesi' },
                            { data: 'Armagan' },
                            { data: 'ArmaganDurumu' },
                            { data: 'BelgeIstemiyor' },
                        ],
                        'order': [[2, 'desc'],[1, 'desc'],[0, 'desc']],//sort
                        'columnDefs': [
                            { targets: 0, className: 'btn-link'},
                            { targets: 2, className: 'bolded text-right'},
                            //{ 'width': '15%', 'targets': 0 },
                            //{ 'width': '25%', 'targets': 4 }
                        ],
                        'language': {
                            'url': 'http://tskgv-portal/OrtakBelgeler/Turkish.txt',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        dom: 'Bfrtip',
                        buttons: [
                            {
                                extend: 'print',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'excel',
                                exportOptions: {
                                    columns: ':visible',
                                    format: {
                                        body: function(data, row, column, node) {
                                            data = $('<p>' + data + '</p>').text();
                                            return $.isNumeric(data.replace(',', '.')) ? data.replace(',', '.') : data;
                                        }
                                    }
                                },
                            },
                            {
                                extend: 'pdf',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'copy',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            , 'pageLength', 'colvis'
                        ],
                        });
                    });

            ";

            return tableString;
        }

        private List<NakitBagisciListItem> GetDataList()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            NakitBagisHareket nakitBagis = new NakitBagisHareket();
            
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();
            DateTime ilkTarih = new DateTime(yil, ay, 1);
            DateTime sonTarih = ilkTarih.AddMonths(1).AddDays(-1);

            DataTable dataTable = nakitBagis.SelectByBolgeTarih(BolgeQS,ilkTarih,sonTarih);

            BaslikTH.InnerText = BolgeQS + " Bölge Temsilciliği " + ilkTarih.ToString("dd.MM.yyyy") + " - " + sonTarih.ToString("dd.MM.yyyy") + " Tarihleri Arası Nakit Bağışlar";
            List<NakitBagisciListItem> list = new List<NakitBagisciListItem>();
            if (dataTable!=null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    int nakitBagisciId = row["NakitBagisciId"].ConvertToInt();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();
                    string bagisTarihi = row["BagisTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy");
                    decimal bagisMiktari = row["BagisMiktari"].ReturnZeroIfNull().ConvertToDecimal();
                    string telefon = row["Telefon1"].ToString() + " " + row["Telefon2"].ToString();
                    string ili = row["Ili"].ToString();
                    string ilcesi = row["Ilcesi"].ReturnEmptyIfNull().ToString();
                    string adres = row["Adres"].ToString();
                    string armagan = row["Armagan"].ToString();
                    string armaganDurumu = row["Durum"].ToString();
                    bool belgeIstemiyor = row["BelgeIstemiyor"].ConvertToBool();


                    NakitBagisciListItem bagisItem = new NakitBagisciListItem();
                    bagisItem.AdiSoyadi = (adi + " " + soyadi).Trim();
                    bagisItem.AdiSoyadi = "<a class='btn btn-link' onclick=OpenModal(" + nakitBagisciId + ");>" + (adi + " " + soyadi).Trim() + "</a>";
                    bagisItem.BagisTarihi = bagisTarihi;
                    bagisItem.BagisMiktari = bagisMiktari.ToString("N", culturInfo);

                    bagisItem.Telefon = telefon;
                    bagisItem.Ili = ili;
                    bagisItem.Ilcesi = ilcesi;
                    bagisItem.Adres = adres;
                    bagisItem.NakitBagisciId = nakitBagisciId;
                    bagisItem.BelgeIstemiyor = belgeIstemiyor ? "Belge İstemiyor" : string.Empty;
                    bagisItem.Armagan = armagan;
                    bagisItem.ArmaganDurumu = armaganDurumu;
                    bagisItem.NakitBagisciId = nakitBagisciId;

                    bagisItem.Secildi = SecilenIdQS.Equals(nakitBagisciId);
                    list.Add(bagisItem);
                }

            }
            return list;
        }
        private class NakitBagisciListItem
        {
            public int NakitBagisciId { get; set; }
            public string AdiSoyadi { get; set; }
            public string BagisTarihi { get; set; }
            public string BagisMiktari { get; set; }
            public string Telefon { get; set; }
            public string Adres { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Armagan { get; set; }
            public string ArmaganDurumu { get; set; }
            public string BelgeIstemiyor { get; set; }
            public bool Secildi { get; set; }
        }

        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ModalTabloOlustur();
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }
        }
        private void ModalTabloOlustur()
        {
            
            var jsonData = ModalTabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string ModalTabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<NakitBagisciListItem> list = GetModalDataList();
                var serializer = new JavaScriptSerializer();
                jSon = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        private string CreateModalDataTable(string jsonData)
        {
            string tableString = @"
                

                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
                jQuery(document).ready(function () {
                    jQuery('#CustomModalDataTable').DataTable({
                        'initComplete': function (settings, json) {//tablo yüklendiğinde
                            var api = this.api();
                            var row = api.row(function (idx, data, node) { //secilen toplantıya gider
                                return data['Secildi'] == true;
                            });
                            if (row.length > 0) {
                                row.select()
                                    .show()
                                    .draw(false);
                            }
                        },
                        data: " + jsonData + @",
                        columns: [
                            { data: 'BagisTarihi' },
                            { data: 'BagisMiktari' },
                            { data: 'Armagan' },
                            { data: 'ArmaganDurumu' },
                        ],
                        'order': [[0, 'desc']],//sort

                        'language': {
                            'url': 'http://tskgv-portal/OrtakBelgeler/Turkish.txt',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        dom: 'rtp',
                        });
                    });

            ";

            return tableString;
        }
        private List<NakitBagisciListItem> GetModalDataList()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            NakitBagisHareket nakitBagis = new NakitBagisHareket();

            DataTable dataTable = nakitBagis.SelectByNakitBagisciId(paramNakitBagisciIdLbl.Value.ConvertToInt());

            List<NakitBagisciListItem> list = new List<NakitBagisciListItem>();
            DataRow row0 = dataTable.Rows[0];
            int nakitBagisciId0 = row0["NakitBagisciId"].ConvertToInt();
            string adi0 = row0["Adi"].ToString();
            string soyadi0 = row0["Soyadi"].ToString();
            AdiLbl.Text = (adi0 + " " + soyadi0).Trim() + " ("+nakitBagisciId0+")";
            if (dataTable!=null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    int nakitBagisciId = row["NakitBagisciId"].ConvertToInt();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();
                    string bagisTarihi = row["BagisTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy");
                    decimal bagisMiktari = row["BagisMiktari"].ReturnZeroIfNull().ConvertToDecimal();
                    string telefon = row["Telefon1"].ToString() + " " + row["Telefon2"].ToString();
                    string ili = row["Ili"].ToString();
                    string ilcesi = row["Ilcesi"].ReturnEmptyIfNull().ToString();
                    string adres = row["Adres"].ToString();
                    string armagan = row["Armagan"].ToString();
                    string armaganDurumu = row["Durum"].ToString();
                    bool belgeIstemiyor = row["BelgeIstemiyor"].ConvertToBool();


                    NakitBagisciListItem bagisItem = new NakitBagisciListItem();
                    bagisItem.AdiSoyadi = (adi + " " + soyadi).Trim();
                    bagisItem.AdiSoyadi = "<a class='btn btn-link' onclick=OpenModal(" + nakitBagisciId + ");>" + (adi + " " + soyadi).Trim() + "</a>";
                    bagisItem.BagisTarihi = bagisTarihi;
                    bagisItem.BagisMiktari = bagisMiktari.ToString("N", culturInfo);

                    bagisItem.Telefon = telefon;
                    bagisItem.Ili = ili;
                    bagisItem.Ilcesi = ilcesi;
                    bagisItem.Adres = adres;
                    bagisItem.NakitBagisciId = nakitBagisciId;
                    bagisItem.BelgeIstemiyor = belgeIstemiyor ? "Belge İstemiyor" : string.Empty;
                    bagisItem.Armagan = armagan;
                    bagisItem.ArmaganDurumu = armaganDurumu;
                    bagisItem.NakitBagisciId = nakitBagisciId;

                    bagisItem.Secildi = SecilenIdQS.Equals(nakitBagisciId);
                    list.Add(bagisItem);
                } 
            }
            return list;
        }
    }
}
