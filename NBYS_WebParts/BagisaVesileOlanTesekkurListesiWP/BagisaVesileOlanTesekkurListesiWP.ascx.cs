using Model.IKYS;
using Model.NBYS;
using Model.Ortak;
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
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BagisaVesileOlanTesekkurListesiWP()
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
                        ViewState["SecilenId"] = 0;
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
            if (!Page.IsPostBack)
            {
                TabloOlustur();
            }
        }


        private void TabloOlustur()
        {
            List<BagisaVesileOlanTesekkurListItem> list = GetData();
            var serializer = new JavaScriptSerializer();
            string jsonData = serializer.Serialize(list);
            bool jasonDataBosMu = string.IsNullOrWhiteSpace(jsonData.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", ""));
            if (!jasonDataBosMu)
            {
                var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
                UtilityHelper.ScriptCalistir(jsString);
            }
            else
            {
                TableDataLbl.Text = "Teşekkür Belgesi bulunmamaktadır.";
            }
        }
        private List<BagisaVesileOlanTesekkurListItem> GetData()
        {
            List<BagisaVesileOlanTesekkurListItem> list = new List<BagisaVesileOlanTesekkurListItem>();
            BagisaVesileOlanTesekkur bagisaVesileOlanTesekkur = new BagisaVesileOlanTesekkur();
            DataTable dataTable = bagisaVesileOlanTesekkur.SelectReturnDataTable();
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    DateTime belgeTarihi;
                    if (DateTime.TryParse(row["BelgeTarihi"].ToString(), out belgeTarihi))
                    {
                        row["BelgeTarihi"] = belgeTarihi.ToString("dd.MM.yyyy");
                    }
                    list.Add(new BagisaVesileOlanTesekkurListItem
                    {
                        BelgeId = Convert.ToInt32(row["Id"]),
                        Adi = row["Adi"].ToString(),
                        Soyadi = row["Soyadi"].ToString(),
                        VerilmeSebebi = row["VerilmeSebebi"].ToString(),
                        BelgeTarihi = belgeTarihi.ToString("dd.MM.yyyy"),
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
            try
            {
                int silId = HiddenSecilenId.Value.ConvertToInt();
                if (silId < 1)
                {
                    throw new Exception("Silinecek kayıt bulunamadı");
                }
                BagisaVesileOlanTesekkur bagisaVesileOlanTesekkur = new BagisaVesileOlanTesekkur() { Id = silId };
                bool silindi = bagisaVesileOlanTesekkur.Delete();
                if (silindi)
                {
                    MessageHelper.PublishMessage("Teşekkür belgesi silindi.", ProjeConstants.MESAJ_BASARILI, 2000);
                    TabloOlustur();
                }
                else
                {
                    throw new Exception("Silme işlemi başarısız oldu");
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
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
