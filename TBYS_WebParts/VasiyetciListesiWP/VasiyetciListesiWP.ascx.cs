using Microsoft.SharePoint;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.VasiyetciListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class VasiyetciListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public VasiyetciListesiWP()
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
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUser();
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
                BolgeQS = IKYSOrtak.PersonelinBolgesiniGetir(CurrentUserName);
                if (!string.IsNullOrEmpty(BolgeQS))
                {
                    TitleLbl.Text = "Vasiyetçi Listesi" + " (" + BolgeQS + " Bölgesi)";
                    
                }
                TabloOlustur();
            }
            catch (Exception ex)
            {
                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
            }
        }
        private string VasiyetciJson()
        {
            List<VasiyetciListItem> list = GetDataList();
            var serializer = new JavaScriptSerializer();
            string jSon = serializer.Serialize(list);
            return jSon;
        }
        private void TabloOlustur()
        {
            var jsonData = VasiyetciJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateDataTable(string jsonData)
        {
            string duzenleGorunsun = string.IsNullOrEmpty(BolgeQS) ? "{ targets:9, visible:true}," : "{ targets:9, visible:false},";
            string dosyaAdi = "Vasiyet' + row.VasiyetciId + '.pdf";
            //string dosyaUrl = SPContext.Current.Web.Url + "/" + ProjeConstants.TASINMAZBELGELERI_LIB + "/" + dosyaAdi;


            string dosyaUrl = UtilityHelper.RootURLGetir() + "/" + ProjeConstants.PATH_TBYS_URL + "/" + ProjeConstants.TBYSBELGELERI_LIB + "/" + dosyaAdi;




            string vasiyetPdffLink =   @"'<a class=\'btn btn-secondary\' data-fancybox data-type=pdf data-width=960 data-height=720 href=" + dosyaUrl + @"> Vasiyet PDF </a>'";
            string tableString = @"
            jQuery(document).ready(function () {

            jQuery('#CustomDataTable').DataTable({
                'initComplete': function (settings, json) {//tablo yüklendiğinde
                    var api = this.api();
                    var row = api.row(function(idx, data, node) { //secilen satıra gider
                        return data['VasiyetciId'] ==" + SecilenIdQS + @";
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
                    { data: 'VasiyetciId' },
                    { data: 'Adi'},
                    { data: 'Soyadi'},
                    { data: 'TCKimlikNo' },
                    { data: 'IkametIli' },
                    { data: 'IkametIlcesi' },
                    { data: 'IkametAdresi'},
                    { data: 'Telefon1' },
                    { data: 'VasiyetciId' },
                    { data: 'VasiyetciId' },

                ],
                'order': [[2, 'desc']],
                columnDefs:
                [
                "+ duzenleGorunsun +@"
                {
                    targets: 8, render: function(data, type, row, meta) {
                    
                    var dosyaUrl='" + dosyaUrl + @"';
                    var link='';
                    
                    if (Boolean(row.VasiyetiYuklendiMi))
                        link= "+vasiyetPdffLink+@";
                    return link;
                }},
                {
                    targets: 9, render: function(data, type, row, meta) {
                    var link= '<a href=" + ProjeConstants.PAGE_VASIYETCI_GIRISI + @"?DestinationApp=Duzenle&VasiyetciId='+row.VasiyetciId +' class=\'btn btn-outline-primary \'>Düzenle</a>';
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
        protected void YeniKayitBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_VASIYETCI_GIRISI);
        }
        private List<VasiyetciListItem> GetDataList()
        {
            Vasiyetci vasiyetci = new Vasiyetci();

            DataTable dataTable = vasiyetci.SelectByBolgeReturnDataTable(BolgeQS);

            int SiraNo = 1;
            List<VasiyetciListItem> list = new List<VasiyetciListItem>();

            foreach (DataRow row in dataTable.Rows)
            {
                string vasiyetciId = row["Id"].ToString();
                string adi = row["Adi"].ToString();
                string soyadi = row["Soyadi"].ToString();
                string tCKimlikNo = row["TCKimlikNo"].ToString();
                string telefon1 = row["Telefon1"].ToString();
                string telefon2 = row["Telefon2"].ToString();
                string ikametAdresi = row["IkametAdresi"].ToString();
                string ikametIli = row["IlAdi"].ToString();
                string ikametIlcesi = row["IlceAdi"].ToString();
                string sagVefat = row["SagVefat"].ToString();
                string vefatTarihi = row["VefatTarihi"].ConvertToDatetimeEmptyIfNull();
                string dogumTarihi = row["DogumTarihi"].ConvertToDatetimeEmptyIfNull();
                string dogumYeri = row["DogumYeri"].ToString();
                string vasiyetTipi = row["VasiyetTipi"].ToString();
                string sorumluBolge = row["SorumluBolge"].ToString();
                string vasiyetinDurumu = row["VasiyetinDurumu"].ToString();
                string noter = row["Noter"].ToString();
                string vasiyetTarihi = row["VasiyetTarihi"].ConvertToDatetimeEmptyIfNull();
                string yevmiyeNumarasi = row["YevmiyeNumarasi"].ToString();
                string vasiyetcininTalebi = row["VasiyetcininTalebi"].ToString();
                string aciklama = row["Aciklama"].ToString();

                if (sagVefat.Equals(ProjeConstants.BAGISCI_VEFAT_INT))
                {
                    sagVefat = ProjeConstants.BAGISCI_VEFAT;
                }
                else if (sagVefat.Equals(ProjeConstants.BAGISCI_SAG_INT))
                {
                    sagVefat = ProjeConstants.BAGISCI_SAG;
                }
                else 
                {
                    sagVefat = ProjeConstants.BAGISCI_BILINMIYOR;
                }

                VasiyetciListItem vasiyetciItem = new VasiyetciListItem();
                vasiyetciItem.Sirano = SiraNo++.ToString();
                vasiyetciItem.VasiyetciId = vasiyetciId;
                vasiyetciItem.Adi = adi;
                vasiyetciItem.Soyadi = soyadi;
                vasiyetciItem.TCKimlikNo = tCKimlikNo;
                vasiyetciItem.Telefon1 = telefon1;
                vasiyetciItem.Telefon2 = telefon2;
                vasiyetciItem.IkametAdresi = ikametAdresi;
                vasiyetciItem.IkametIli = ikametIli;
                vasiyetciItem.IkametIlcesi = ikametIlcesi;
                vasiyetciItem.SagVefat = sagVefat;
                vasiyetciItem.VefatTarihi = vefatTarihi;
                vasiyetciItem.DogumTarihi = dogumTarihi;
                vasiyetciItem.DogumYeri = dogumYeri;
                vasiyetciItem.VasiyetTipi = vasiyetTipi;
                vasiyetciItem.SorumluBolge = sorumluBolge;
                vasiyetciItem.VasiyetinDurumu = vasiyetinDurumu;
                vasiyetciItem.Noter = noter;
                vasiyetciItem.VasiyetTarihi = vasiyetTarihi;
                vasiyetciItem.YevmiyeNumarasi = yevmiyeNumarasi;
                vasiyetciItem.VasiyetcininTalebi = vasiyetcininTalebi;
                vasiyetciItem.Aciklama = aciklama;
                string dosyaAdi = "Vasiyet"+vasiyetciItem.VasiyetciId + ".pdf";
                bool dosyaVarMi = UtilityHelper.DosyaVarMi(UtilityHelper.TbysURLGetir(), ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi);
                vasiyetciItem.VasiyetiYuklendiMi = dosyaVarMi;


                list.Add(vasiyetciItem);
            }
            return list;
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

            GridView1.DataSource = GetDataList();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=VasiyetciListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
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

        private class VasiyetciListItem
        {
            public string Sirano { get; set; }
            public string VasiyetciId { get; set; }
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public string TCKimlikNo { get; set; }
            public string IkametIli { get; set; }
            public string IkametIlcesi { get; set; }
            public string IkametAdresi { get; set; }
            public string Telefon1 { get; set; }
            public string Telefon2 { get; set; }
            public string SagVefat { get; set; }
            public string VefatTarihi { get; set; }
            public string DogumTarihi { get; set; }
            public string DogumYeri { get; set; }
            public string VasiyetTipi { get; set; }
            public string SorumluBolge { get; set; }
            public string VasiyetinDurumu { get; set; }
            public string Noter { get; set; }
            public string VasiyetTarihi { get; set; }
            public string YevmiyeNumarasi { get; set; }
            public string VasiyetcininTalebi { get; set; }
            public string Aciklama { get; set; }
            public bool VasiyetiYuklendiMi { get; set; }


        }

    }
}
