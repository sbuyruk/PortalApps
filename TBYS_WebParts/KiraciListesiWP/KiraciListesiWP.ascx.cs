using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.KiraciListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraciListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraciListesiWP()
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
        private string AuthQS
        {
            get
            {

                if (ViewState["Auth"] == null)
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
                    KiraciSecimiDDLDoldur();
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void KiraciSecimiDDLDoldur()
        {
            KiraciSecimiDDL.Items.Clear();
            KiraciSecimiDDL.Items.Add(new ListItem(ProjeConstants.KIRASOZLESME_AKTIF,ProjeConstants.KIRASOZLESME_AKTIF_INT.ToString()));
            KiraciSecimiDDL.Items.Add(new ListItem(ProjeConstants.KIRASOZLESME_AKTIF_DEGIL, ProjeConstants.KIRASOZLESME_AKTIF_DEGIL_INT.ToString()));
            KiraciSecimiDDL.Items.Add(new ListItem(ProjeConstants.KIRASOZLESME_AKTIF_HEPSI, ProjeConstants.KIRASOZLESME_AKTIF_HEPSI_INT.ToString()));

            if (!string.IsNullOrEmpty(SecilenIdQS))
            {
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.SelectAktifSozlesmeByKiraciId(SecilenIdQS.ConvertToInt());
                if (kiraSozlesme != null)
                {
                    UtilityHelper.SetDDLValue(KiraciSecimiDDL, ProjeConstants.KIRASOZLESME_AKTIF_INT.ToString());
                }
                else
                {
                    UtilityHelper.SetDDLValue(KiraciSecimiDDL, ProjeConstants.KIRASOZLESME_AKTIF_DEGIL_INT.ToString());
                } 
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
            string duzenleGorunsun = string.IsNullOrEmpty(AuthQS) || !AuthQS.Equals(ProjeConstants.TBYS_YETKILI_BIRIM)
                ? "{ targets:5, visible:false},{ targets:7, visible:false},{ targets:8, visible:false},{ targets:9, visible:false},"
                : string.Empty;
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
                            { data: 'KiraciId' },
                            { data: 'AdiUnvani' },
                            { data: 'Adres' },
                            { data: 'Bolge' },
                            { data: 'IlceIl' },
                            { data: 'Sozlesme' },
                            { data: 'KiraKarti' },
                            { data: 'Teminat' },
                            { data: 'Bakiye' },
                            { data: 'Duzenle' },               
                        ],
                        'columnDefs': ["
                            + duzenleGorunsun +@"
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
                        //colon resizable
                        //initComplete: function (settings) {
                        //    $('#CustomDataTable').colResizable({ liveDrag: true });
                        //},
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
                        'createdRow': function(row, data, dataIndex) {
                            if ((!data.Aktif)&&(!data.Secildi))
                            {
                                $(row).addClass('pasif-kiraci');

                            }
                        },//set row color


                        });
                    });

            ";

            return tableString;
        }
        private List<KiraciListItem> GetDataList()
        {

            Kiraci kiraci = new Kiraci();
            string kiraciSecimi = KiraciSecimiDDL.SelectedItem.Value;
            DataTable dataTable;
            if (KiraciSecimiDDL.SelectedItem.Value.ConvertToInt()==ProjeConstants.KIRASOZLESME_AKTIF_DEGIL_INT)
            {
                dataTable = kiraci.SelectAktifSozlesmesiOlmayanKiracilarReturnDT();
            }
            else
            {
                dataTable = kiraci.SelectAllReturnDT(kiraciSecimi); 
            }
            int SiraNo = 1;

            List<KiraciListItem> list = new List<KiraciListItem>();

            foreach (DataRow row in dataTable.Rows)
            {
                string kiraciId = row["KiraciId"].ToString();
                string sozlesmeId = row["SozlesmeId"].ToString();
                string adi = row["Adi"].ToString();
                string soyadi = row["Soyadi"].ToString();

                string bolge = row["Bolge"].ToString();
                string ilIlce = row["IlIlce"].ToString();
                string adres = row["Adres"].ToString();
                bool aktif = row["Aktif"].ReturnFalseIfNull().ConvertToBool();

                KiraciListItem kiraciItem = new KiraciListItem();
                kiraciItem.Sirano = SiraNo++.ToString();
                kiraciItem.KiraciId = kiraciId;
                kiraciItem.AdiUnvani = (adi + " " + soyadi).Trim();

                kiraciItem.Bolge = bolge;
                kiraciItem.IlceIl = ilIlce;
                kiraciItem.Adres = adres;
                KiraSozlesme ks = new KiraSozlesme();

                if (!aktif)
                {
                    ks = ks.SelectBitenSozlesmeByKiraciId(kiraciId.ConvertToInt());
                }
                else
                {
                    ks = ks.SelectAktifSozlesmeByKiraciId(kiraciId.ConvertToInt());
                }
                
                int sonSozlesmeId = ks != null ? ks.Id : sozlesmeId.ConvertToInt();
                kiraciItem.Sozlesme = ks != null ? ("<a href=" + ProjeConstants.PAGE_KIRASOZLESMESI + "?KiraSozlesmeId=" + sonSozlesmeId + " class='btn btn-outline-secondary'>Sözleşme</a>")
                    :string.Empty;
                kiraciItem.Teminat = ks != null ? ("<a  target='_blank' href=" + ProjeConstants.PAGE_TEMINAT_ISLEMLERI + "?KiraSozlesmeId=" + sonSozlesmeId + " class='btn btn-outline-secondary'>Teminat</a>")
                    :string.Empty;
                kiraciItem.Bakiye = "<a  target='_blank' href=" + ProjeConstants.PAGE_KIRA_BAKIYEDEVRI + "?KiraciId=" + kiraciId + " class='btn btn-outline-secondary'>Bakiye Devri</a>";
                kiraciItem.KiraKarti = "<a  target='_blank' href=" + ProjeConstants.PAGE_KIRAKARTI + "?KiraciId=" + kiraciId + " class='btn btn-outline-secondary'>Kira Kartı</a>";
                kiraciItem.Duzenle = "<a href=" + ProjeConstants.PAGE_KIRACI_GIRIS + "?DestinationApp=KD&SenderApp=KL&KiraciId=" + kiraciId + " class='btn btn-outline-primary'>Düzenle</a>";
                kiraciItem.Secildi = SecilenIdQS.Equals(kiraciItem.KiraciId);
                kiraciItem.Aktif = aktif;
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
            GridView1.DataSource = kiraci.SelectAllReturnDT(KiraciSecimiDDL.SelectedItem.Value);
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
            public string Bolge { get; set; }
            public string IlceIl { get; set; }
            public string Sozlesme { get; set; }
            public string Bakiye { get; set; }
            public string KiraKarti { get; set; }
            public string Teminat { get; set; }
            public string Duzenle { get; set; }
            public bool Aktif { get; set; }
            public bool Secildi { get; set; }
        }

        protected void KiraciSecimiDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
    }
}
