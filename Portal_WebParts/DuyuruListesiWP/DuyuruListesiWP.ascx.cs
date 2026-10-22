using Model.Ortak;
using Portal_WebParts.Logging;
using Utility.Logging;
using Model.Services.Portal;
using Model.Portal;
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

namespace Portal_WebParts.DuyuruListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class DuyuruListesiWP : WebPart
    {
        private PortalLogContext LoggingContext
        {
            get { return PortalLogContext.FromCurrentRequest("Portal", "DuyuruListesiWP"); }
        }
        private void ExecuteAction(string operation, Action action)
        {
            PortalWebPartLog.ExecuteAction(LoggingContext, operation, action);
        }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public DuyuruListesiWP()
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
        protected void Page_Load(object sender, EventArgs e)
        {
            ExecuteAction("Page_Load", () =>
            {
                PortalWebPartLog.RecordOpened(LoggingContext);
                var manager = ScriptManager.GetCurrent(Page);
                if (manager != null) manager.RegisterPostBackControl(ExcelBtn);
                if (!Page.IsPostBack)
                {
                    TabloOlustur();
                }
            });
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("ExcelBtn_Click", () =>
            {
                ExportToExcel();
            });
        }
        protected void ExportToExcel()
        {
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;

            GridView1.DataSource = GetDataList();
            GridView1.DataBind();

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
            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=DuyuruListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            Page.Response.ContentType = "application/vnd.ms-excel";
            Page.Response.Write(style);
            Page.Response.Output.Write(sw.ToString());
            Page.Response.Flush();
            Page.Response.End();

        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            });
        }
        protected void YeniDuyuruBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("YeniDuyuruBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_DUYURU_GIRIS + "?DestinationApp=DG");
            });
        }
        private void RedirectToPage(string pageUrl)
        {

                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);

        }
        protected void HiddenOkuyanlarBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("HiddenOkuyanlarBtn_Click", () =>
            {
                int duyuruId = paramDuyuruIdLbl.Value.ConvertToInt();
                if (duyuruId > 0)
                {
                    Duyuru duyuru = new Duyuru();
                    duyuru = new Model.Services.Portal.DuyuruService(LoggingContext).GetById(duyuruId);
                    if (duyuru != null)
                    {
                        DuyuruLbl.Text = System.Web.HttpUtility.HtmlEncode(duyuru.Baslik);
                        KayitGetirModal(duyuru.Id);
                    }
                }
            });
        }
        private void KayitGetirModal(int duyuruId)
        {
            var jsonData = OkuyanlarListesiJson(duyuruId); //veri çekilip json a çeviriliyor
            var jsString = CreateModalDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string OkuyanlarListesiJson(int duyuruId)
        {
            DuyuruOkuma duyuruOkumaDao = new DuyuruOkuma();
            string jSon = new Model.Services.Portal.DuyuruOkumaService(LoggingContext).GetByDuyuruId(duyuruId);
            return (jSon ?? "[]").Replace("<", "\\u003c");
        }
        private string CreateModalDataTable(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
                    jQuery('#CustomModalDataTable').DataTable().destroy();
                }
                jQuery('#CustomModalDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
                jQuery(document).ready(function () {
                    jQuery('#CustomModalDataTable').DataTable({
                        data: " + jsonData + @",
                        columns: [
                            { data: 'Adi', render: jQuery.fn.dataTable.render.text() },
                            { data: 'Soyadi', render: jQuery.fn.dataTable.render.text() },
                            { data: 'OkumaTarihi' },
                        ],
                        columnDefs:[{targets:2, render:function(data){
                              return moment(data).format('DD.MM.YYYY hh:mm');
                            }}],
                        'order': [[2, 'desc']],//sort
                        'language': {
                            'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        dom: 'ftip',
                        });
                    });

            ";

            return tableString;
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloJson()
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(GetDataList()).Replace("<", "\\u003c");
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
                jQuery(document).ready(function () {
                    jQuery('#CustomDataTable').DataTable({
                        'initComplete': function (settings, json) {//tablo yüklendiginde
                            var api = this.api();
                            var row = api.row(function (idx, data, node) { //secilen toplantiya gider
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
                            { data: 'Baslik', render: jQuery.fn.dataTable.render.text() },
                            { data: 'YayinBasTar' },
                            { data: 'YayinBitTar' },
                            { data: 'Tekrar' },
                            { data: 'Popup' },
                            { data: 'Okunma' },
                            { data: 'Duyuru' },
                        ],
                        'order': [[1, 'desc'],[2, 'desc']],//sort
                        'language': {
                            'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        dom: 'ftip',
                        });
                    });

            ";

            return tableString;
        }

        private List<DuyuruListItem> GetDataList()
        {
            Duyuru duyuruDao = new Duyuru();
            List<Duyuru> list = new Model.Services.Portal.DuyuruService(LoggingContext).GetAnnouncementList();

            List<DuyuruListItem> yeniListe = new List<DuyuruListItem>();
            foreach (var item in list)
            {
                DuyuruListItem listItem = new DuyuruListItem();
                listItem.Baslik = item.Baslik;
                listItem.Aktif = item.Aktif;
                listItem.DuyuruId = item.Id;
                listItem.Popup = item.Popup ? "Var" : "Yok";
                listItem.Tekrar = item.Tekrar;
                listItem.YayinBasTar = item.YayinBasTar.ConvertToDatetimeEmptyIfNull();
                listItem.YayinBitTar = item.YayinBitTar.ConvertToDatetimeEmptyIfNull();
                listItem.Okunma = "<a href=# onclick=OkuyanlarClicked('" + item.Id + "'); class='btn btn-outline-warning' >Okuyanlar</a>";
                listItem.Duyuru = "<a href=" + ProjeConstants.PAGE_DUYURU_GIRIS + "?DestinationApp=DD&DuyuruId=" + item.Id +
                         " class='btn btn-outline-primary'>Duyuru</a>";
                listItem.Secildi = SecilenIdQS.ConvertToInt() == item.Id;
                yeniListe.Add(listItem);
            }
            return yeniListe;
        }
        private class DuyuruListItem
        {
            public int DuyuruId { get; set; }
            public string Baslik { get; set; }
            public string YayinBasTar { get; set; }
            public string YayinBitTar { get; set; }
            public string Tekrar { get; set; }
            public string Popup { get; set; }
            public bool Aktif { get; set; }
            public string Okunma { get; set; }
            public string Duyuru { get; set; }
            public bool Secildi { get; set; }
        }

    }
}
