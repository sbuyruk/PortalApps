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

namespace BTYS_Webparts.BolgeKiraciListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BolgeKiraciListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BolgeKiraciListesiWP()
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
                    if (!Page.IsPostBack)
                    {
                        TabloOlustur();
                    } 
                }
                else
                    MessageHelper.PublishMessage("Bölgeniz Belirlenemedi", ProjeConstants.MESAJ_HATA);
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
                List<KiraciListItem> list = GetDataList();
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
                if ( jQuery.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
                    jQuery('#CustomModalDataTable').DataTable().destroy();
                }
                jQuery('#CustomModalDataTable tbody').empty();

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
                            { data: 'KiraciId' },
                            { data: 'AdiUnvani' },
                            { data: 'Adres' },
                            { data: 'IlceIl' },
                            { data: 'KiraBedeli' },
                            { data: 'OdemeSekli' },
                            { data: 'KiraKarti' },           
                        ],
                        'columnDefs': [
                            { 'width': '20%', 'targets': 1 },
                            { 'width': '25%', 'targets': 2 }
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
                    });

            ";

            return tableString;
        }

        private List<KiraciListItem> GetDataList()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            Kiraci kiraci = new Kiraci();
            string kiraciSecimi = ProjeConstants.KIRASOZLESME_AKTIF_INT.ToString();
            DataTable dataTable;
            dataTable = kiraci.SelectByByBolgeReturnDT(kiraciSecimi,BolgeQS);
            
            int SiraNo = 1;

            List<KiraciListItem> list = new List<KiraciListItem>();
            foreach (DataRow row in dataTable.Rows)
            {
                string kiraciId = row["KiraciId"].ToString();
                int sozlesmeId = row["SozlesmeId"].ReturnZeroIfNull().ConvertToInt();
                string adi = row["Adi"].ToString();
                string soyadi = row["Soyadi"].ToString();

                string ili = row["KiraciIli"].ToString();
                string ilcesi = row["KiraciIlcesi"].ToString();
                string kiraciAdresi = row["KiraciAdresi"].ToString();
                decimal kiraBedeli = row["KiraBedeli"].ReturnZeroIfNull().ConvertToDecimal();
                string odemeSekli = row["OdemeSekli"].ToString();

                KiraciListItem kiraciItem = new KiraciListItem();
                kiraciItem.Sirano = SiraNo++.ToString();
                kiraciItem.KiraciId = kiraciId;
                kiraciItem.AdiUnvani = (adi + " " + soyadi).Trim();

                kiraciItem.IlceIl = ili+"/"+ilcesi;
                kiraciItem.Adres = kiraciAdresi;
                kiraciItem.KiraBedeli= kiraBedeli.ToString("N", culturInfo);
                kiraciItem.OdemeSekli = odemeSekli;
                KiraSozlesme ks = new KiraSozlesme();
                ks = ks.SelectAktifSozlesmeByKiraciId(kiraciId.ConvertToInt());
                int sonSozlesmeId = ks != null ? ks.Id : sozlesmeId.ConvertToInt();
                kiraciItem.KiraKarti = "<a  target='_blank' href=" + ProjeConstants.PAGE_BOLGEKIRA_KARTI + "?KiraciId=" + kiraciId + " class='btn btn-outline-secondary'>Kira Kartı</a>";
                kiraciItem.Secildi = SecilenIdQS.Equals(kiraciItem.KiraciId);
                list.Add(kiraciItem);
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
            Kiraci kiraci = new Kiraci();
            GridView1.DataSource = kiraci.SelectByByBolgeReturnDT(ProjeConstants.KIRASOZLESME_AKTIF_INT.ToString(), BolgeQS);
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=KiraciListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
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
        private class KiraciListItem
        {
            public string Sirano { get; set; }
            public string KiraciId { get; set; }
            public string AdiUnvani { get; set; }
            public string Adres { get; set; }
            public string KiraBedeli { get; set; }
            public string OdemeSekli { get; set; }
            public string IlceIl { get; set; }
            public string KiraKarti { get; set; }
            public bool Secildi { get; set; }
        }

        protected void KiraciSecimiDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
    }
}
