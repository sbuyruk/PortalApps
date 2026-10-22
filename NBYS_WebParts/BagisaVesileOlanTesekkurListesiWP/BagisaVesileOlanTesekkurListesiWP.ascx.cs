using Utility.Logging;
using NBYS_WebParts.Logging;
using System.Globalization;
using Model.IKYS;
using Model.NBYS;
using Model.Ortak;
using Model.Services.NBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.BagisaVesileOlanTesekkurListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BagisaVesileOlanTesekkurListesiWP : WebPart
    {
        private PortalLogContext LoggingContext { get { return PortalLogContext.FromCurrentRequest("NBYS", "BagisaVesileOlanTesekkurListesiWP"); } }
        private bool canProcess = true;
        private void ExecuteAction(string operation, Action action)
        {
            NbysWebPartLog.ExecuteAction(LoggingContext, operation, () => { if (canProcess) action(); });
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            ChromeType = PartChromeType.None;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("CloseBtn_Click", () => RedirectToPage(ProjeConstants.PAGE_HOME));
        }
        private void RedirectToPage(string pageUrl)
        {
            string current = Page.Request.Url.GetLeftPart(UriPartial.Path);
            string target = current.Substring(0, current.LastIndexOf('/') + 1) + pageUrl;
            Page.Response.Redirect(target, false);
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BagisaVesileOlanTesekkurListesiWP()
        {
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
                        ViewState["SecilenId"] = 0;
                    }
                }
                int id;
                return int.TryParse(Convert.ToString(ViewState["SecilenId"]), out id) && id > 0 ? id.ToString(CultureInfo.InvariantCulture) : "0";
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
                if (!Page.IsPostBack) { NbysWebPartLog.RecordOpened(LoggingContext); TabloOlustur(); }
            });
        }

        private void TabloOlustur()
        {
            var list = GetData();
            string json = new JavaScriptSerializer().Serialize(list);
            TableDataLbl.Text = list.Count == 0 ? "Teşekkür kaydı bulunmamaktadır." : string.Empty;
            UtilityHelper.ScriptCalistir(CreateDataTable(json));
        }
        private List<BagisaVesileOlanTesekkurListItem> GetData()
        {
            List<BagisaVesileOlanTesekkurListItem> list = new List<BagisaVesileOlanTesekkurListItem>();
            DataTable dataTable = new BagisaVesileOlanTesekkurService(LoggingContext).GetListTable();
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string belgeTarihi = row.IsNull("BelgeTarihi") ? string.Empty
                        : Convert.ToDateTime(row["BelgeTarihi"]).ToString("dd.MM.yyyy", new CultureInfo("tr-TR"));
                    list.Add(new BagisaVesileOlanTesekkurListItem
                    {
                        BelgeId = Convert.ToInt32(row["Id"]),
                        Adi = row["Adi"].ToString(),
                        Soyadi = row["Soyadi"].ToString(),
                        VerilmeSebebi = row["VerilmeSebebi"].ToString(),
                        BelgeTarihi = belgeTarihi,
                        ImzalayanAdiSoyadi = row["ImzalayanAdiSoyadi"].ToString(),
                        Aciklama = row["Aciklama"].ToString(),
                        Duzenle = "<a href='" + ProjeConstants.PAGE_BAGISAVESILE_GIRIS + "?SecilenId=" + row["Id"] + @"' class='btn btn-sm btn-primary'>Düzenle</a>",
                        Sil = "<a href='#' onclick='Sil(" + row["Id"] + @")' class='btn btn-sm btn-danger'>Sil</a>"
                    });

                }
            }
            return list;

        }
        protected void SilNowBtn_Click(object sender, EventArgs e)
        {
            ExecuteAction("SilNowBtn_Click", () =>
            {
                int id;
                if (!int.TryParse(HiddenSecilenId.Value, out id) || id <= 0)
                { Messages.Warning("Silinecek teşekkür kaydını listeden seçin."); return; }
                NbysReferralWriteResult result = new BagisaVesileOlanTesekkurService(LoggingContext).Remove(id);
                NbysReferralFeedback.Write(LoggingContext, result, "Teşekkür kaydı silindi.", TabloOlustur);
            });
        }

        private string CreateDataTable(string jsonData)
        {

            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date

                jQuery('#CustomDataTable').DataTable({ 
                'initComplete': function (settings, json) {//tablo yüklendiğinde
                    var api = this.api();
                    var row = api.row(function(idx, data, node) { //seçilen satıra gider
                        return data['BelgeId'] ==" + SecilenIdQS + @";
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
                        { data: 'BelgeId'},
                        { data: 'Adi'},
                        { data: 'Soyadi'},
                        { data: 'VerilmeSebebi'},
                        { data: 'BelgeTarihi' },
                        { data: 'ImzalayanAdiSoyadi'},
                        { data: 'Aciklama'},
                        { data: 'Duzenle'},
                        { data: 'Sil'},

                    ],
                    columnDefs: [{ targets: [1, 2, 3, 4, 5, 6], render: $.fn.dataTable.render.text() }],
                    pageLength: 8,
                    'order': [[4, 'desc']],//sort date desc
                    'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    dom: 'frtip',   

                });

            ";

            return tableString;
        }
        private class BagisaVesileOlanTesekkurListItem
        {
            public int BelgeId { get; set; }
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public string VerilmeSebebi { get; set; }
            public string BelgeTarihi { get; set; }
            public string ImzalayanAdiSoyadi { get; set; }
            public string Aciklama { get; set; }
            public string Duzenle { get; set; }
            public string Sil { get; set; }

        }
    }
}
