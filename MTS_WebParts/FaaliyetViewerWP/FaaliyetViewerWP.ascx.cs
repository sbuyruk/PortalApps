using MTS_WebParts.Logging;
using Utility.Logging;
using System.Web;
using Model.IKYS;
using Model.Services.IKYS;
using Model.MTS;
using Model.Ortak;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace MTS_WebParts.FaaliyetViewerWP
{
    [ToolboxItemAttribute(false)]
    public partial class FaaliyetViewerWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("MTS", "FaaliyetViewerWP")); } }
        private static string Encode(object value) { return HttpUtility.HtmlEncode(Convert.ToString(value)); }

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FaaliyetViewerWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string CalendarViewQS
        {
            get
            {

                if (ViewState["CalendarView"] == null)
                {
                    if (Page.Request.QueryString["CalendarView"] != null)
                    {
                        ViewState["CalendarView"] = Page.Request.QueryString["CalendarView"];
                    }
                    else
                    {
                        ViewState["CalendarView"] = string.Empty;
                    }
                }
                return ViewState["CalendarView"].ToString();
            }

            set
            {
                ViewState["CalendarView"] = value;
            }
        }
        private string InitialDateQS
        {
            get
            {

                if (ViewState["InitialDate"] == null)
                {
                    if (Page.Request.QueryString["InitialDate"] != null)
                    {
                        ViewState["InitialDate"] = Page.Request.QueryString["InitialDate"];
                    }
                    else
                    {
                        ViewState["InitialDate"] = string.Empty;
                    }
                }
                return ViewState["InitialDate"].ToString();
            }

            set
            {
                ViewState["InitialDate"] = value;
            }
        }
        private string DogumGunuQS
        {
            get
            {

                if (ViewState["DogumGunu"] == null)
                {
                    if (Page.Request.QueryString["DogumGunu"] != null)
                    {
                        ViewState["DogumGunu"] = Page.Request.QueryString["DogumGunu"];
                    }
                    else
                    {
                        ViewState["DogumGunu"] = VakifIciKutlamaChk.Checked;
                    }
                }
                return ViewState["DogumGunu"].ToString();
            }

            set
            {
                ViewState["DogumGunu"] = value;
            }
        }
        private string KisiDogumGunuQS
        {
            get
            {

                if (ViewState["KisiDogumGunu"] == null)
                {
                    if (Page.Request.QueryString["KisiDogumGunu"] != null)
                    {
                        ViewState["KisiDogumGunu"] = Page.Request.QueryString["KisiDogumGunu"];
                    }
                    else
                    {
                        ViewState["KisiDogumGunu"] = VakifDisiKutlamaChk.Checked;
                    }
                }
                return ViewState["KisiDogumGunu"].ToString();
            }

            set
            {
                ViewState["KisiDogumGunu"] = value;
            }
        }
        private string ResmiTatilQS
        {
            get
            {

                if (ViewState["ResmiTatil"] == null)
                {
                    if (Page.Request.QueryString["ResmiTatil"] != null)
                    {
                        ViewState["ResmiTatil"] = Page.Request.QueryString["ResmiTatil"];
                    }
                    else
                    {
                        ViewState["ResmiTatil"] = ResmiTatilChk.Checked;
                    }
                }
                return ViewState["ResmiTatil"].ToString();
            }

            set
            {
                ViewState["ResmiTatil"] = value;
            }
        }
        private string ToplantiQS
        {
            get
            {

                if (ViewState["Toplanti"] == null)
                {
                    if (Page.Request.QueryString["Toplanti"] != null)
                    {
                        ViewState["Toplanti"] = Page.Request.QueryString["Toplanti"];
                    }
                    else
                    {
                        ViewState["Toplanti"] = ToplantiChk.Checked;
                    }
                }
                return ViewState["Toplanti"].ToString();
            }

            set
            {
                ViewState["Toplanti"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) MtsWebPartLog.RecordOpened(LogContext);

            });
        }
        private bool calendarLoaded;
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            MtsWebPartLog.ExecuteAction(LogContext, "RenderCalendar", () =>
            {
                if (!calendarLoaded) KayitGetir();
            });
        }
        private void KayitGetir()
        {
            var faaliyetJsonData = FaaliyetListesiniGetir();
            //var resmiTatilJsonData = ResmiTatilListesiniGetir();
            //var kisiDogumGunleriJsonData = KisiDogumGunuListesiniGetir();
            //var toplantiJsonData = ToplantiListesiniGetir();
            //var personelDogumGunleriJsonData = PersonelDogumGunuListesiniGetir();
            var resmiTatilJsonData = ResmiTatilQS.ConvertToBool() ? ResmiTatilListesiniGetir() : string.Empty;
            var toplantiJsonData = ToplantiQS.ConvertToBool() ? ToplantiListesiniGetir() : string.Empty;
            var personelDogumGunleriJsonData = DogumGunuQS.ConvertToBool() ? PersonelDogumGunuListesiniGetir() : string.Empty;
            var kisiDogumGunleriJsonData = KisiDogumGunuQS.ConvertToBool() ? KisiDogumGunuListesiniGetir() : string.Empty;

            string jsonArrayString = MtsCalendar.Merge(faaliyetJsonData, resmiTatilJsonData, kisiDogumGunleriJsonData, toplantiJsonData, personelDogumGunleriJsonData);

            var jsString = CreateJsString(jsonArrayString); //javascript kodu hazirlaniyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);

            calendarLoaded = true;
        }
        private string ToplantiListesiniGetir()
        {
            DataTable dataTable = new Model.Services.Portal.ToplantiService().GetByParticipant(ProjeConstants.GENELMUDUR_PERSONELID, LogContext);

            List<CalendarEvent> eventItems = new List<CalendarEvent>();
            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {

                    CalendarEvent item = new CalendarEvent();

                    item.id = int.Parse(dataRow["Id"].ToString());
                    item.state = ProjeConstants.FAALIYET_DURUMU_ONAYLANDI_INT.ToString();
                    item.title = dataRow["ToplantiKonusu"].ToString();
                    item.start = string.Format("{0:s}", dataRow["BaslangicTarihi"]);
                    item.end = string.Format("{0:s}", dataRow["BitisTarihi"]);
                    item.url = "Toplanti";
                    item.startEditable = false;
                    item.color = Color.Red.Name;
                    item.textColor = Color.White.Name;
                    item.purpose = ProjeConstants.FAALIYET_AMACI_TOPLANTI_INT.ToString();
                    eventItems.Add(item);
                }
            }

            string json = MtsCalendar.Json(eventItems);
            return json;
        }
        private string KisiDogumGunuListesiniGetir()
        {
            return MtsCalendar.ContactBirthdays(LogContext);
        }
        private string PersonelDogumGunuListesiniGetir()
        {
            return MtsCalendar.EmployeeCelebrations(LogContext);
        }
        private string FaaliyetListesiniGetir()
        {
            Dictionary<int, string> versions;
            string json = new Model.Services.MTS.FaaliyetService(LogContext).GetCalendarJson(ProjeConstants.FAALIYET_ACIKTARIHLI_DEGIL, out versions);
            ViewState["CalendarVersions"] = versions;
            return json;
        }
        private string ResmiTatilListesiniGetir()
        {
            return MtsCalendar.Holidays(LogContext);
        }
        private string CreateJsString(string jsonData)
        {
            string initialDate = MtsCalendar.InitialDate(InitialDateQS);
            string calendarView = MtsCalendar.View(CalendarViewQS, false);
            string calendarStr = @"
            (function() {
                /* initialize the calendar
                -----------------------------------------------------------------*/

                var calendarEl = document.getElementById('calendar');
                if (!calendarEl) return;
                if (calendarEl.mtsCalendar) calendarEl.mtsCalendar.destroy();
                var calendar = new FullCalendar.Calendar(calendarEl, {
                    locale: 'tr',
                    firstDay: 1,
                    initialView:  " + calendarView + @",//'dayGridMonth',
                    initialDate: " + initialDate + @",
                    events:" + jsonData + @",
                    headerToolbar: {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
                    },
                    editable: true,
                    eventAllow: function(dropLocation, event) {
                        return event.startEditable === true;
                    },
                    eventDurationEditable: true,
                    allDayMaintainDuration: true,
                    eventResize: function(info) { saveMove(info); },
                    eventDrop: function(info) { saveMove(info); },
                    droppable: false,
                    eventClick: function(info) {
                        //info.jsEvent.preventDefault(); // don't let the browser navigate

                        if (info.event.url=='Toplanti') {
                            info.jsEvent.preventDefault();
                            toplantiId=info.event.id;
                            if (toplantiId>0)
                                ToplantiDetaylariModal(info.event.id);
                        }
                        else if (info.event.url) {
                            window.location.assign(info.event.url);
                            info.jsEvent.preventDefault();
                        }
                    }
                    });
                    function saveMove(info) {
                        if (info.event.startEditable !== true || info.oldEvent.allDay !== info.event.allDay || !info.event.end) {
                            info.revert(); return;
                        }
                        if (!confirm('Faaliyetin tarih ve saati değiştirilsin mi?')) { info.revert(); return; }
                        var id = info.event.id, start = info.event.start.toISOString(), finish = info.event.end.toISOString(), view = calendar.view.type;
                        info.revert();
                        FaaliyetKaydet(id, start, finish, view);
                    }
                    calendarEl.mtsCalendar = calendar;
                    calendar.render();

                    })();

                                    ";


            return calendarStr;
        }


        private HyperLink HyperLinkGetir(int faaliyetId, string randevuKonusu)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            HyperLink hyperLink = new HyperLink
            {
                Text = string.IsNullOrEmpty(randevuKonusu) ? "Konu Yok" : randevuKonusu
            };

            if (faaliyetId > 0)
            {
                string linkUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_FAALIYET_GIRIS + "?FaaliyetId=" + faaliyetId;

                hyperLink.NavigateUrl = linkUrl;
            }
            return hyperLink;
        }

        Random random = new Random();
        public void AddItem(List<CalendarEvent> eventItems)
        {
            CalendarEvent item = new CalendarEvent();

            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, random.Next(1, 30));

            item.id = random.Next(1, 100);
            item.start = startDate.ToString("s");
            item.end = startDate.AddDays(random.Next(1, 5)).ToString("s");
            item.allDay = true;
            item.color = "blue";
            item.title = "Calendar Item " + item.id;
            eventItems.Add(item);
        }
        protected void FaaliyetKaydetNowBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetKaydetNowBtn_Click", () =>
            {
                int id;
                string version;
                var versions = ViewState["CalendarVersions"] as Dictionary<int, string>;
                if (!int.TryParse(paramFaaliyetId.Value, out id) || id <= 0 || versions == null || !versions.TryGetValue(id, out version))
                    throw new MtsInputException("Faaliyet bilgisi güncel değil. Sayfayı yenileyip yeniden seçin.");
                DateTime start = MtsCalendar.ReadInstant(paramBasTar.Value), finish = MtsCalendar.ReadInstant(paramBitTar.Value);
                Model.MTS.Faaliyet saved;
                var result = new Model.Services.MTS.MtsActivityWriteService(LogContext).Move(id, version, start, finish, out saved);
                if (result == Model.Services.MTS.MtsActivityResult.NotFound)
                    throw new MtsInputException("Faaliyet bulunamadı. Sayfayı yenileyip yeniden seçin.");
                if (result == Model.Services.MTS.MtsActivityResult.Changed)
                    throw new MtsInputException("Faaliyet başka bir işlemde değişti. Sayfayı yenileyip güncel bilgileri kontrol edin.");
                if (result != Model.Services.MTS.MtsActivityResult.Completed)
                    throw new MtsInputException("Tarih aralığı geçersiz. Bitiş başlangıçtan sonra olmalıdır. Sayfayı yenileyip yeniden deneyin.");
                CalendarViewQS = paramView.Value;
                InitialDateQS = saved.BaslangicTarihi.ToString("yyyy-MM-dd");

                MtsCalendar.CompleteMove(LogContext, saved, () => { calendarLoaded = true; KayitGetir(); });

            });
        }

        private void RedirectToPage(string pageUrl)
        {

                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);

        }

        #region Toplanti Ayrintilari Popup
        protected void ToplantiDetaylariBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "ToplantiDetaylariBtn_Click", () =>
            {
                IcKatilimcilarCell.Text = string.Empty;
                BilgiCell.Text = string.Empty;
                int toplantiId = paramToplantiIdLbl.Text.ConvertToInt();
                Toplanti toplanti = new Model.Services.Portal.ToplantiService().GetById(toplantiId, LogContext);
                if (toplanti == null) throw new MtsInputException("Toplantı bulunamadı. Takvimi yenileyip yeniden seçin.");
                if (toplanti != null)
                {
                    IdLbl.Text = " ( Toplanti No: " + toplanti.Id.ToString() + " )";
                    ToplantiKonusuCell.Text = Encode(toplanti.ToplantiKonusu);
                    ToplantiYetkilisiCell.Text = Encode(ParseToplantiYetkilisi(toplanti.ToplantiYetkilisi));
                    KoordinatorCell.Text = Encode(ParseKoordinator(toplanti.Koordinator));
                    BaslangicZamaniCell.Text = toplanti.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm");
                    BitisZamaniCell.Text = toplanti.BitisTarihi.ToString("dd.MM.yyyy HH:mm");
                    DisKatilimcilarCell.Text = Encode(toplanti.DisKatilimcilar).Replace(",", "<br />");

                    Personel personel = new Personel();
                    List<Personel> list = new PersonelService().GetMeetingParticipants(toplanti.Id, false, LogContext);

                    foreach (var item in list)
                    {
                        IcKatilimcilarCell.Text += Encode(item.Adi + " " + item.Soyadi) + "<br />";
                    }

                    list = new PersonelService().GetMeetingParticipants(toplanti.Id, true, LogContext);
                    foreach (var item in list)
                    {
                        BilgiCell.Text += Encode(item.Adi + " " + item.Soyadi) + "<br />";
                    }

                    ToplantiYeriCell.Text = Encode(ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger));
                    AciklamaCell.Text = Encode(toplanti.Aciklama);
                    CevrimIciCell.Text = toplanti.CevrimIci ? "Evet" : "Hayir";
                    IkramOnayiCell.Text = toplanti.IkramOnayi ? "Evet" : "Hayir";
                    IkramMalzemesiCell.Text = Encode(toplanti.IkramOnayi ? toplanti.IkramMalzemesi : "-");

                }
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "OpenToplantiModal();", true);

            });
        }
        private string ParseToplantiYeri(int yeri, string diger)
        {
            string yeriStr = string.Empty;
            ToplantiParametre toplantiParametre = new Model.Services.Portal.ToplantiParametreService().GetById(yeri, LogContext);
            if (toplantiParametre != null)
            {
                if (toplantiParametre.Deger.Equals(ProjeConstants.PARAM_DIGER))
                {
                    yeriStr = diger;
                }
                else
                {
                    yeriStr = toplantiParametre == null ? string.Empty : toplantiParametre.Deger;
                }
            }

            return yeriStr;
        }
        private string ParseKoordinator(int koordinator)
        {
            string koordinatorStr = string.Empty;
            BirimTanim birimTanim = new BirimTanimService().GetById(koordinator, LogContext);
            if (birimTanim != null)
            {
                koordinatorStr = birimTanim.KisaAdi;
            }
            return koordinatorStr;
        }
        private string ParseToplantiYetkilisi(int toplantiYetkilisi)
        {
            string yetkiliStr = string.Empty;
            Personel personel = new Personel();
            personel = new PersonelService().GetById(toplantiYetkilisi, LogContext);
            if (personel != null)
            {
                yetkiliStr = personel.Adi + " " + personel.Soyadi;
            }
            return yetkiliStr;
        }
        #endregion
        #region Toggle CheckButtons
        protected void VakifIciKutlamaChk_CheckedChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "VakifIciKutlamaChk_CheckedChanged", () =>
            {
                DogumGunuQS = VakifIciKutlamaChk.Checked.ToString();


            });
        }

        protected void VakifDisiKutlamaChk_CheckedChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "VakifDisiKutlamaChk_CheckedChanged", () =>
            {
                KisiDogumGunuQS = VakifDisiKutlamaChk.Checked.ToString();


            });
        }

        protected void ResmiTatilChk_CheckedChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "ResmiTatilChk_CheckedChanged", () =>
            {
                ResmiTatilQS = ResmiTatilChk.Checked.ToString();


            });
        }

        protected void ToplantiChk_CheckedChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "ToplantiChk_CheckedChanged", () =>
            {
                ToplantiQS = ToplantiChk.Checked.ToString();


            });
        }
        #endregion
    }
}
