using Portal_WebParts.Logging;
using Utility.Logging;
using Model.Services.Portal;
using System.Globalization;
using Model.MTS;
using Model.Ortak;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.OlayListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class OlayListesiWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("Portal", "OlayListesiWP")); } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public OlayListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
            UpdatePanel2.ContentTemplateContainer.Controls.AddAt(0, new LiteralControl(BuildArchiveFilter()));
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

        private string ProgramQS
        {
            get
            {

                if (ViewState["Program"] == null)
                {
                    if (Page.Request.QueryString["Program"] != null)
                    {
                        ViewState["Program"] = Page.Request.QueryString["Program"];
                    }
                    else
                    {
                        ViewState["Program"] = string.Empty;
                    }
                }
                return ViewState["Program"].ToString();
            }

            set
            {
                ViewState["Program"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                TitleLbl.Text = "Geçmiş Olay Arşivi";
                if (!Page.IsPostBack) PortalWebPartLog.RecordOpened(LogContext);
                TabloOlustur();
            });
        }
        private void TabloOlustur()
        {
            try
            {
                var jsonData = TabloJson();
                UtilityHelper.ScriptCalistir("jQuery('#CustomDataTable').show();" + CreateDataTable(jsonData));
            }
            catch
            {
                // Hide stale results; the outer boundary reports the read failure once.
                UtilityHelper.ScriptCalistir("jQuery('#CustomDataTable').hide();");
                throw;
            }
        }
        private string TabloJson()
        {
            SorguZamaniQS = ReadArchiveDay();
            var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            return serializer.Serialize(OlayListItemDoldur(GetDataList(SorguZamaniQS)))
                .Replace("<", "\\u003c").Replace(">", "\\u003e").Replace("&", "\\u0026");
        }
        private List<PortalEventArchiveEntry> GetDataList(DateTime tarih)
        {
            List<PortalEventArchiveEntry> list = new PortalEventArchiveService(LogContext).GetDay(tarih, ProgramQS);
            return list;
        }
        private List<OlayListItem> OlayListItemDoldur(List<PortalEventArchiveEntry> list)
        {
            List<OlayListItem> olayList = new List<OlayListItem>();
            foreach (var item in list)
            {
                OlayListItem oli = new OlayListItem();
                string aciklama = ParseAciklama(item);
                oli.Aciklama = aciklama;
                oli.AciklamaHam = item.Aciklama;
                oli.Program= item.Program;
                oli.IslemKonusu = item.IslemKonusu;
                oli.IslemTarihi = item.IslemTarihi.ConvertToDDMMYYYHHmmFormat();
                oli.IslemTipi = item.IslemTipi;
                oli.IslemYapan = item.IslemYapan;
                oli.OlayId = item.Id.ToString();
                olayList.Add(oli);
            }
            return olayList;
        }

        public string ParseAciklama(PortalEventArchiveEntry olay)
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
            Faaliyet faaliyet = faaliyetId > 0 ? new Model.Services.MTS.FaaliyetService().GetById(faaliyetId, LogContext) : null;
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

        private DateTime ReadArchiveDay()
        {
            string value = Page.Request.QueryString["ArsivTarihi"];
            if (string.IsNullOrEmpty(value)) return DateTime.Today;
            DateTime day;
            if (!DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out day) || day < new DateTime(1753, 1, 1)
                || day == DateTime.MaxValue.Date)
                throw new PortalInputException("Geçerli bir arşiv tarihi seçiniz.");
            return day;
        }

        private string BuildArchiveFilter()
        {
            DateTime day;
            if (!DateTime.TryParseExact(Page.Request.QueryString["ArsivTarihi"], "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out day)
                || day < new DateTime(1753, 1, 1) || day == DateTime.MaxValue.Date) day = DateTime.Today;
            return "<div class='alert alert-info' role='note'>Bu ekran eski sistemdeki olay kayıtlarını salt okunur gösterir. "
                + "Yeni kayıtlar için Log İzleme ekranındaki İşlem denetimi ve SQL izi sekmelerini kullanın. "
                + "Açıklamadaki kişi, faaliyet ve anı objesi bilgileri varsa güncel iş kayıtlarından tamamlanır.</div>"
                + "<div class='mb-3'><label>Arşiv tarihi <input type='date' class='form-control' min='1753-01-01' max='9999-12-30' value='"
                + day.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "' /></label> "
                + "<button type='button' class='btn btn-primary' onclick=\"var d=this.parentNode.querySelector('input').value;"
                + "if(d){var u=new URL(window.location.href);u.searchParams.set('ArsivTarihi',d);window.location.assign(u.toString());}\">Arşivi listele</button></div>";
        }

        private static string H(string value) { return System.Web.HttpUtility.HtmlEncode(value ?? string.Empty); }

        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if (jQuery.fn.DataTable.isDataTable('#CustomDataTable')) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();
                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                var table=jQuery('#CustomDataTable').DataTable({

                    data: " + jsonData + @",
                    columns: [
                        { data: 'IslemTarihi' },
                        { data: 'Program'},
                        { data: 'IslemKonusu'},
                        { data: 'IslemTipi' },
                        { data: 'IslemYapan'},
                        { data: 'Aciklama' , 'width': '30%'},
                        { data: 'AciklamaHam' },
                    ],
                    'columnDefs': [
                        { type: 'turkish', targets: [1,2,3,4,5] },
                        { targets: [0,1,2,3,4,6], render: jQuery.fn.dataTable.render.text() },
                    ],
                    'order': [[0, 'desc']],//sort date desc
                    'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
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
                        'pageLength', 'colvis'
                    ],

                });

            ";

            return tableString;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "CloseBtn_Click", () =>
            {
                string currentUrl = Page.Request.Url.ToString();
                Page.Response.Redirect(currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME);
            });
        }

        private class OlayListItem
        {
            public string OlayId { get; set; }
            public string IslemTarihi { get; set; }
            public string Program { get; set; }
            public string IslemKonusu { get; set; }
            public string IslemTipi { get; set; }
            public string IslemYapan { get; set; }
            public string Aciklama { get; set; }
            public string AciklamaHam { get; set; }
        }
    }
}
