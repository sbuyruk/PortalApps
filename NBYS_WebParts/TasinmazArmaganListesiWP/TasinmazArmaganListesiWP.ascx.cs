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

namespace NBYS_WebParts.TasinmazArmaganListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazArmaganListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazArmaganListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
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
                List<TasinmazBagisciListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
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
            string duzenleGorunsun = String.Empty;

            string tableString = @"
                jQuery(document).ready(function () {
                    jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
                    jQuery('#CustomDataTable').DataTable({
                        data: " + jsonData + @",
                        columns: [
                            { data: 'BagisId' },
                            { data: 'AdiSoyadi' },
                            { data: 'Sag_vefat' },
                            { data: 'Adres' },
                            { data: 'IlIlce' },
                            { data: 'Telefon' },
                            { data: 'BagisTarihi' },
                            { data: 'Armagan' },
                            { data: 'ArmaganDurumu' },
                            { data: 'BagisciKarti' },
                        ],
                        columnDefs: [
                            { type: 'turkish', targets: [1, 2, 3] },
                            " + duzenleGorunsun + @"
                        ],
                        'order': [[6, 'desc'], [1, 'desc']],//Tarih Sıralı
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
                                    columns: ':visible'
                                }
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
                });";
            return tableString;
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
             "attachment;filename=TasinmazArmaganListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
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
            RedirectToPage(ProjeConstants.PAGE_HOME);
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
        private List<TasinmazBagisciListItem> GetDataList()
        {
            TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
            DataTable dataTable = tasinmazBagisci.SelectTasinmazBagisciReturnDataTable(false);
            int SiraNo = 1;
            List<TasinmazBagisciListItem> list = new List<TasinmazBagisciListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            DataView dataView = new DataView(dataTable);
            foreach (DataRowView row in dataView)
            {
                string bagisId = row["BagisId"].ToString();
                string tasinmazBagisciId = row["TasinmazBagisciId"].ToString();
                string adiSoyadi = row["AdiSoyadi"].ToString();
                string sag_vefat = row["Sag_vefat"].ToString();
                string bagisTarihi = row["BagisTarihi"].ReturnEmptyIfNull().ConvertToDatetime().ToString("dd.MM.yyyy");
                string armaganId = row["ArmaganId"].ReturnEmptyIfNull().ToString();
                string armaganDurumu = row["ArmaganDurumu"].ToString();
                string armaganTarihi = row["ArmaganTarihi"].ToString();

                string adres = row["Adres"].ToString();
                string ili = row["Ili"].ToString();
                string ilcesi = row["Ilcesi"].ToString();
                string telefon1 = row["Telefon1"].ToString();
                string telefon2 = row["Telefon2"].ToString();

                //string adresIlIlce = adres + " " +ilcesi+"/"+ili;
                string ilIlce = ilcesi + (!string.IsNullOrEmpty(ilcesi) && !string.IsNullOrEmpty(ili)?"/":string.Empty) + ili;
                string telefon = string.IsNullOrEmpty(telefon1) ? "" : telefon1 + " " + telefon2;

                TasinmazBagisciListItem bagisciItem = new TasinmazBagisciListItem();
                bagisciItem.Sirano = SiraNo++.ToString();
                bagisciItem.BagisId = bagisId;
                bagisciItem.AdiSoyadi = adiSoyadi.ToString();
                bagisciItem.Sag_vefat = sag_vefat;
                bagisciItem.BagisTarihi = bagisTarihi.ConvertToDatetimeEmptyIfNull();
                bagisciItem.ArmaganId = armaganId.ToString();
                bagisciItem.ArmaganDurumu = armaganDurumu;
                bagisciItem.ArmaganTarihi = armaganTarihi;
                bagisciItem.Adres = adres;
                bagisciItem.Ilcesi = ilcesi;
                bagisciItem.Ili = ili;
                bagisciItem.IlIlce = ilIlce;
                bagisciItem.Telefon = telefon;
                bool yetkiliMi = !string.IsNullOrEmpty(AuthQS) && AuthQS.Equals(ProjeConstants.NBYS_YETKILI_BIRIM);
                
                if (yetkiliMi)
                {
                    if (!string.IsNullOrEmpty(armaganId))
                    {
                        bagisciItem.Armagan = "<a href=" + ProjeConstants.PAGE_TASINMAZARMAGAN_DUZENLE + @"?DestinationApp=BD&BagisId=" + bagisId + " class=\'btn btn-outline-primary\'>Armağan Belgesi</a>";
                    }
                    else
                    {
                        bagisciItem.Armagan = "<a href=" + ProjeConstants.PAGE_TASINMAZARMAGAN_DUZENLE + @"?DestinationApp=BD&BagisId=" + bagisId + " class=\'btn btn-outline-success \'>Armağan Oluştur</a>";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(armaganId))
                    {
                        bagisciItem.Armagan = "Armağan Belgesi oluşturuldu";
                    }
                    else
                    {
                        bagisciItem.Armagan = "Armağan Belgesi oluşturulmadı";
                    }
                    
                }
                bagisciItem.BagisciKarti= "<a target='_blank' href=" + ProjeConstants.PAGE_TASINMAZBAGISCI_KARTI + "?SenderApp=TBL&BagisciId=" + tasinmazBagisciId + " class='btn btn-outline-info'>Taşınmaz Bağışçı Kartı</a>";
                
                bool gizli = row["Gizli"].ReturnEmptyIfNull().ConvertToBool();
                if (gizli)
                {
                    bagisciItem.AdiSoyadi = adiSoyadi;
                     bagisciItem.BagisciKarti= bagisciItem.Telefon=
                        bagisciItem.IlIlce = bagisciItem.Adres = bagisciItem.Ili = bagisciItem.Ilcesi = ProjeConstants.GIZLI_STRING;
                    bagisciItem.ArmaganTarihi =  string.Empty;
                }
                list.Add(bagisciItem);
            }
            return list;
        }
        private class TasinmazBagisciListItem
        {
            public string Sirano { get; set; }
            public string BagisId { get; set; }
            public string AdiSoyadi { get; set; }
            public string Sag_vefat { get; set; }
            public string BagisTarihi { get; set; }
            public string ArmaganId { get; set; }
            public string Armagan { get; set; }
            public string ArmaganDurumu { get; set; }
            public string ArmaganTarihi { get; set; }
            public string IlIlce { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Adres { get; set; }
            public string Telefon { get; set; }
            public string BagisciKarti { get; set; }

        }

        protected void GizliBagislarHaricChk_CheckedChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
    }
}
