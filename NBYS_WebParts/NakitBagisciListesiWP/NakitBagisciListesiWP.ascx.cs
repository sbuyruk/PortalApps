using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
namespace NBYS_WebParts.NakitBagisciListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class NakitBagisciListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NakitBagisciListesiWP()
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
                        ViewState["SecilenId"] = "0";
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }
        private string SecilenIlQS
        {
            get
            {

                if (ViewState["SecilenIl"] == null)
                {
                    if (Page.Request.QueryString["SecilenIl"] != null)
                    {
                        ViewState["SecilenIl"] = Page.Request.QueryString["SecilenIl"];
                    }
                    else
                    {
                        ViewState["SecilenIl"] = string.Empty;
                    }
                }
                return ViewState["SecilenIl"].ToString();
            }

            set
            {
                ViewState["SecilenIl"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    FillIlData();
                    FillBagisZamaniDDL();
                    SetDDLValues();
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void FillBagisZamaniDDL()
        {
            BagisZamaniDDL.Items.Add(new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI));
            BagisZamaniDDL.Items.Add(new ListItem("Son 5 Yıl", "Son5Yil"));
            BagisZamaniDDL.Items.Add(new ListItem("Önceki Ay (Tüm Bağışçılar)", "OncekiAyTamami"));
            BagisZamaniDDL.Items.Add(new ListItem("Önceki Ay (Yeni Bağışçılar)", "OncekiAyYeni"));
            BagisZamaniDDL.Items.Add(new ListItem("Son Ay (Tüm Bağışçılar)", "SonAyTamami"));
            BagisZamaniDDL.Items.Add(new ListItem("Son Ay (Yeni Bağışçılar)", "SonAyYeni"));
        }
        private void FillIlData()
        {
            if (IliDDL.SelectedItem == null)
            {
                IliDDL.Items.Clear();

                Il pIl = new Il();
                List<Il> list = pIl.SelectAll<Il>();
                IliDDL.Items.Add(new ListItem(ProjeConstants.HEPSI, ProjeConstants.IL_HEPSI.ToString()));
                foreach (Il il in list)
                {
                    IliDDL.Items.Add(new ListItem(il.IlAdi, il.Id.ToString()));
                }
            }

        }
        private void SetDDLValues()
        {
            try
            {
                //acilista ili querystringde gelen ile eşitle

                //il
                string il = !string.IsNullOrEmpty(SecilenIlQS) ? SecilenIlQS : ProjeConstants.IL_HEPSI.ToString();
                ListItem IlItem = new ListItem();
                if (!string.IsNullOrEmpty(il))
                    IlItem = IliDDL.Items.FindByValue(il);

                if (IlItem != null)
                {
                    IliDDL.SelectedValue = IlItem.Value;
                    SecilenIlQS = IlItem.Value;
                }

                ListItem item = BagisZamaniDDL.Items.FindByValue("OncekiAyTamami");
                BagisZamaniDDL.SelectedValue = item.Value;
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }



        }
        private void TabloOlustur()
        {
            var jsonData = GetBagisciData(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string GetBagisciData()
        {
            int rowCount = 0;
            List<NakitBagisci> list = new List<NakitBagisci>();
            NakitBagisci nakitBagisci = new NakitBagisci();

            string selectedOption = BagisZamaniDDL.SelectedValue;

            var json = string.Empty;
            DateTime today = DateTime.Today;

            if (selectedOption.Equals("OncekiAyTamami"))
            {
                string basTar = new DateTime(today.Year, today.Month, 1).AddMonths(-1).ToString(ProjeConstants.DATE_TR).ReturnQuotedValue().ToString();//geçen ayın ilk günü
                string sonTar = new DateTime(today.Year, today.Month, 1).ToString(ProjeConstants.DATE_TR).ReturnQuotedValue().ToString();//bu ayın ilk günü
                json = nakitBagisci.SelectByIlBagisTarihi(SecilenIlQS.ConvertToInt(), basTar, sonTar, ref list, ref rowCount);//nakitBagisci.SelectByIl(SecilenIlQS.ConvertToInt(), ref rowCount);
            }
            else if (selectedOption.Equals("OncekiAyYeni"))
            {
                string basTar = new DateTime(today.Year, today.Month, 1).AddMonths(-1).ToString(ProjeConstants.DATE_TR).ReturnQuotedValue().ToString();//geçen ayın ilk günü
                string sonTar = new DateTime(today.Year, today.Month, 1).ToString(ProjeConstants.DATE_TR).ReturnQuotedValue().ToString();//bu ayın ilk günü
                json = nakitBagisci.SelectByIlBagisTarihiYeni(SecilenIlQS.ConvertToInt(), basTar, sonTar, ref list, ref rowCount);
            }
            if (selectedOption.Equals("SonAyTamami"))
            {
                string basTar = new DateTime(today.Year, today.Month, 1).ToString(ProjeConstants.DATE_TR).ReturnQuotedValue().ToString();//bu ayın ilk günü
                string sonTar = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1).ToString(ProjeConstants.DATE_TR).ReturnQuotedValue().ToString();//bu ayın son günü
                json = nakitBagisci.SelectByIlBagisTarihi(SecilenIlQS.ConvertToInt(), basTar, sonTar, ref list, ref rowCount);//nakitBagisci.SelectByIl(SecilenIlQS.ConvertToInt(), ref rowCount);
            }
            else if (selectedOption.Equals("SonAyYeni"))
            {
                string basTar = new DateTime(today.Year, today.Month, 1).ToString(ProjeConstants.DATE_TR).ReturnQuotedValue().ToString();//bu ayın ilk günü
                string sonTar = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1).ToString(ProjeConstants.DATE_TR).ReturnQuotedValue().ToString();//bu ayın son günü

                json = nakitBagisci.SelectByIlBagisTarihiYeni(SecilenIlQS.ConvertToInt(), basTar, sonTar, ref list, ref rowCount);
            }
            else if (selectedOption.Equals("Son5Yil"))
            {
                string besYilOnce = DateTime.Today.AddYears(-5).ToString(ProjeConstants.DATE_TR).ReturnQuotedValue().ToString();
                string sonTar = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1).ToString(ProjeConstants.DATE_TR).ReturnQuotedValue().ToString();//bu ayın son günü
                json = nakitBagisci.SelectByIlBagisTarihi(SecilenIlQS.ConvertToInt(), besYilOnce, sonTar, ref list, ref rowCount);//nakitBagisci.SelectByIl(SecilenIlQS.ConvertToInt(), ref rowCount);
            }
            else if (selectedOption.Equals("Hepsi"))
            {
                json = nakitBagisci.SelectByIl(SecilenIlQS.ConvertToInt(), ref list, ref rowCount);
            }

            return json;
        }
        private string CreateDataTable(string jsonData)
        {
            string queryStr = "&SecilenIl=" + SecilenIlQS;
            string tableString = @"
            jQuery(document).ready(function () {

            jQuery('#CustomDataTable').DataTable({
                'initComplete': function (settings, json) {//tablo yüklendiğinde
                    var api = this.api();
                    var row = api.row(function(idx, data, node) { //secilen satıra gider
                        return data['NakitBagisciId'] ==" + SecilenIdQS + @";
                    });
                    if (row.length > 0)
                    {
                        row.select()
                            .show()
                            .draw(false);
                    }
                },
                data: " + jsonData + @",
                columns: [
                    { data: 'NakitBagisciId' },
                    { data: 'Adi'},
                    { data: 'TCKimlikNo' },
                    { data: 'Ili' },
                    { data: 'Ilcesi' },
                    { data: 'Telefon1' },
                    { data: 'Adres'},
                    { data: 'NakitBagisciId' },

                ],
                'order': [[2, 'desc']],
                columnDefs:
                [
                {
                    targets: 1, render: function(data, type, row, meta) {
                    var link= '<a href=# onclick=OpenModal('+row.NakitBagisciId+'); class=\'btn btn-link \'>'+(row.Adi+' '+ row.Soyadi).trim() + '</a>';
                    return link;
                }},
                {
                    targets: 7, render: function(data, type, row, meta) {
                    var link= '<a href=' + '" + ProjeConstants.PAGE_NAKITBAGISCI_EDIT + "?NakitBagisciId=' + row.NakitBagisciId + '" + queryStr + @" class=\'btn-link text-primary\' >Düzenle</a>';
                    return link;
                }},
                ],
                'language': {
                    'url': 'http://tskgv-portal/OrtakBelgeler/Turkish.txt',
                    'decimal': ',',
                    'thousands': '.'
                },
                responsive: true,
                destroy: true,
                autoWidth: false,
                dom: 'Bfrtip',
                buttons:
                [
                    {
                extend: 'print',
                        exportOptions:
                    {
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
                        exportOptions:
                    {
                    columns: ':visible'
                        }
                },
                    {
                extend: 'copy',
                        exportOptions:
                    {
                    columns: ':visible'
                        }
                },
                    , 'pageLength', 'colvis'
                ]



            });
        });
        ";
            return tableString;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {

        }
        protected void IliDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenIlQS = IliDDL.SelectedItem.Value.ToString();
            TabloOlustur();
        }
        private void TabloModalOlustur(string nakitBagisciId)
        {
            var jsonData = GetModalDataJson(nakitBagisciId); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateModalDataTable(string jsonData)
        {
            string tableString = @"
        jQuery(document).ready(function () {
            if ( jQuery.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
                jQuery('#CustomModalDataTable').DataTable().destroy();
            }
            jQuery('#CustomModalDataTable tbody').empty();

            jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
            jQuery('#CustomModalDataTable').DataTable({
                data: " + jsonData + @",
                columns: [
                    { data: 'BagisTarihi' },
                    { data: 'BagisMiktari', 'width': '10%', 'className': 'text-right' },
                    { data: 'Banka' },
                    { data: 'Armagan' },
                    { data: 'ArmaganTutari' },
                    { data: 'Durum' },
                ],
                'order': [[0, 'desc']],

                'language': {
                    'url': 'http://tskgv-portal/OrtakBelgeler/Turkish.txt',
                    'decimal': ',',
                    'thousands': '.'
                },
                columnDefs:[
                    {targets:0, render:function(data){
                        return moment(data).format('DD.MM.YYYY');
                    }},
                ],
                responsive: true,
                dom: 'rtip',

            });
        });
        ";
            return tableString;
        }
        private string GetModalDataJson(string nakitBagisciId)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            NakitBagisHareket nbh = new NakitBagisHareket();
            int rowCount = 0;
            var json = nbh.SelectByBagisciIdReturnJSon(nakitBagisciId, ref rowCount);
            decimal toplamTutar = nbh.GetSumBagisMiktariByNakitBagisciIdBetweenBasTarBitTar(
                ProjeConstants.BAGIS_SORGU_BASTAR.ConvertToDatetime(), DateTime.Today, nakitBagisciId.ConvertToInt());
            BagisBilgileriLbl.Text = rowCount < 1 ? "Bağış bulunmamaktadır" :
                "Bağışçının " + rowCount + " defada yaptığı toplam " + toplamTutar.ToString("N", culturInfo) + "TL bağışı bulunmaktadır";
            return json;
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
            GridView1.DataSource = GetBagisciData();
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
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            try
            {

                TabloModalOlustur(paramNakitBagisciIdLbl.Value);
                NakitBagisciFormunuDoldur(paramNakitBagisciIdLbl.Value);
                UtilityHelper.ScriptCalistir("SetPageIndex();");
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }
        }
        private void NakitBagisciFormunuDoldur(string nakitBagisciIdStr)
        {
            if (!string.IsNullOrEmpty(nakitBagisciIdStr))
            {
                int nakitBagisciId = nakitBagisciIdStr.ConvertToInt();

                NakitBagisci nakitBagisci = new NakitBagisci();
                nakitBagisci = nakitBagisci.Select<NakitBagisci>(nakitBagisciId);
                if (nakitBagisci != null)
                {
                    //NakitBagisciIdLbl.Text = nakitBagisciId.ToString();
                    TableRow row = new TableRow();
                    TableCell AdiCell = new TableCell();
                    TableCell TCKimlikNoCell = new TableCell();
                    TableCell AdresCell = new TableCell();
                    TableCell IlIlceCell = new TableCell();
                    TableCell TelefonCell = new TableCell();
                    TableCell TuzelKisiCell = new TableCell();

                    AdiCell.Text = nakitBagisci.Adi.ReturnEmptyIfNull().ToString();
                    TCKimlikNoCell.Text = nakitBagisci.TCKimlikNo.ReturnEmptyIfNull().ToString();
                    AdresCell.Text = nakitBagisci.Adres.ReturnEmptyIfNull().ToString();

                    int ilId = nakitBagisci.Ili.ConvertToInt();
                    Il il = new Il();
                    il = il.Select<Il>(ilId);
                    if (il != null)
                    {

                        IlIlceCell.Text = il.IlAdi.ReturnEmptyIfNull().ToString();
                    }
                    int ilceId = nakitBagisci.Ilcesi.ConvertToInt();
                    Ilce ilce = new Ilce();
                    ilce = ilce.Select<Ilce>(ilceId);
                    if (ilce != null)
                    {

                        IlIlceCell.Text += " " + ilce.IlceAdi.ReturnEmptyIfNull().ToString();
                    }
                    TelefonCell.Text = nakitBagisci.Telefon1.ReturnEmptyIfNull().ToString();
                    TuzelKisiCell.Text = nakitBagisci.TuzelKisi.ConvertToBool() ? "Evet" : "Hayır";
                    row.Controls.Add(AdiCell);
                    row.Controls.Add(TCKimlikNoCell);
                    row.Controls.Add(AdresCell);
                    row.Controls.Add(IlIlceCell);
                    row.Controls.Add(TelefonCell);
                    row.Controls.Add(TuzelKisiCell);
                    BagisciTable.Controls.Add(row);

                }
            }
        }
    }
}
