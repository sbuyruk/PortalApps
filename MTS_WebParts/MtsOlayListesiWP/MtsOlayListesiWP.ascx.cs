using MTS_WebParts.Logging;
using Utility.Logging;
using Model.IKYS;
using Model.MTS;
using Model.NBYS;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace MTS_WebParts.MtsOlayListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class MtsOlayListesiWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("MTS", "MtsOlayListesiWP")); } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public MtsOlayListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private DateTime SorguZamaniQS
        {
            get
            {
                if (ViewState["SorguZamani"] == null)
                {
                    ViewState["SorguZamani"] = DateTime.Today;
                }
                return ViewState["SorguZamani"].ConvertToDatetime();
            }

            set
            {
                ViewState["SorguZamani"] = value;
            }
        }
        protected override void OnPreRender(EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "LoadList", TabloOlustur);
            if (MtsWebPartLog.HasFailed(LogContext))
                UtilityHelper.ScriptCalistir("jQuery('#CustomDataTable').hide();");
            base.OnPreRender(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) MtsWebPartLog.RecordOpened(LogContext);
            });
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir("jQuery('#CustomDataTable').show();" + jsString);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            SorguZamaniQS = DateTime.Today;
            List<Olay> olayList = GetDataList(SorguZamaniQS);
            List<OlayListItem> oliList = OlayListItemDoldur(olayList);
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            jSon = serializer.Serialize(oliList);

            return jSon.Replace("<", "\\u003c").Replace(">", "\\u003e").Replace("&", "\\u0026");
        }
        private List<Olay> GetDataList(DateTime tarih)
        {
            List<Olay> list = new Model.Services.Ortak.OlayService().GetByDateAndProgram(tarih, ProjeConstants.MTS, LogContext);
            return list;
        }
        private List<OlayListItem> OlayListItemDoldur(List<Olay> list)
        {
            List<OlayListItem> olayList = new List<OlayListItem>();
            foreach (var item in list)
            {
                OlayListItem oli = new OlayListItem();
                string aciklama= ParseAciklama(item);
                oli.Aciklama = aciklama;
                oli.IslemKonusu = item.IslemKonusu;
                oli.IslemTarihi = item.IslemTarihi.ConvertToDDMMYYYHHmmFormat();
                oli.IslemTipi = item.IslemTipi;
                oli.IslemYapan = item.IslemYapan;
                oli.OlayId = item.Id.ToString();
                olayList.Add(oli);
            }
            return olayList;
        }

        public string ParseAciklama(Olay olay)
        {
            int faaliyetId = 0, katilimciId = 0;
            foreach (string field in (olay.Aciklama ?? string.Empty).Split(ProjeConstants.DELIMITER))
            {
                string[] pair = field.Split(new[] { '=' }, 2);
                int id;
                if (pair.Length != 2 || !int.TryParse(pair[1].Trim(), out id) || id <= 0) continue;
                if (pair[0].Trim() == "FaaliyetId") faaliyetId = id;
                else if (pair[0].Trim() == "KatilimciId") katilimciId = id;
            }
            Faaliyet faaliyet = faaliyetId > 0 ? new Model.Services.MTS.FaaliyetService(LogContext).GetById(faaliyetId) : null;
            Kisi kisi = katilimciId > 0 ? new Model.Services.MTS.KisiService().GetById(katilimciId, LogContext) : null;
            var details = new System.Text.StringBuilder();
            if (kisi != null) details.Append(" — Katılımcı: ").Append(H(kisi.Adi + " " + kisi.Soyadi));
            else if (katilimciId > 0) details.Append(" — İlgili kişi kaydı artık bulunmuyor.");
            if (faaliyetId > 0 && faaliyet == null) details.Append(" — İlgili faaliyet kaydı artık bulunmuyor.");
            if (faaliyet != null && kisi != null)
            {
                var service = new Model.Services.MTS.AniObjesiDagitimService();
                DataTable rows = service.GetEventDistribution(faaliyetId, katilimciId, LogContext);
                if (rows != null)
                    foreach (DataRow row in rows.Rows)
                        details.Append(" — Verilen anı objesi: ").Append(H(Convert.ToString(row["Adi"])))
                            .Append(" (").Append(Convert.ToInt32(row["Adet"])).Append(")");
                var received = service.GetGetirilen(faaliyetId, katilimciId, LogContext);
                if (received != null && !string.IsNullOrEmpty(received.GetirilenAniObjesi))
                    details.Append(" — Getirilen anı objesi: ").Append(H(received.GetirilenAniObjesi));
            }
            string activity = faaliyet == null ? string.Empty : "<strong>" + H(faaliyet.FaaliyetKonusu)
                + "</strong> konulu, " + faaliyet.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + " tarihli faaliyetin ";
            return "(" + olay.IslemTarihi.ConvertToDDMMYYYHHmmFormat() + ") — " + activity + "<strong>"
                + H(olay.IslemKonusu) + "</strong> bölümünde " + H(olay.IslemYapan) + " tarafından <strong>"
                + H(olay.IslemTipi) + "</strong> işlemi yapılmıştır." + details;
        }

        private static string H(string value) { return System.Web.HttpUtility.HtmlEncode(value ?? string.Empty); }

        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if (jQuery.fn.DataTable.isDataTable('#CustomDataTable')) jQuery('#CustomDataTable').DataTable().destroy();
                jQuery('#CustomDataTable tbody').empty();
                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                var table=jQuery('#CustomDataTable').DataTable({

                    data: " + jsonData + @",
                    columns: [
                        { data: 'IslemTarihi' },
                        { data: 'IslemKonusu'},
                        { data: 'IslemTipi' },
                        { data: 'Aciklama' },
                        { data: 'IslemYapan'},
                    ],
                    'columnDefs': [
                        { type: 'turkish', targets: [1,2,3,4] },
                        { targets: [0,1,2,4], render: jQuery.fn.dataTable.render.text() },
                    ],
                    'order': [[0, 'desc']],//sort date desc
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
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            });
        }
        protected void RefreshTimer_Tick(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "RefreshTimer_Tick", () =>
            {
                // The list is refreshed once in OnPreRender, including empty results.
            });
        }
        private class OlayListItem
        {
            public string OlayId { get; set; }
            public string IslemTarihi { get; set; }
            public string IslemKonusu { get; set; }
            public string IslemTipi { get; set; }
            public string Aciklama { get; set; }
            public string IslemYapan { get; set; }
        }
    }
}
