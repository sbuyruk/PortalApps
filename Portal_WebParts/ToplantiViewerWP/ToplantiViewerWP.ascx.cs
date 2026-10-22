using Portal_WebParts.Logging;
using Utility.Logging;
using Model.Services.Portal;
using Model.IKYS;
using Model.Services.IKYS;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.ToplantiViewerWP
{
    [ToolboxItemAttribute(false)]
    public partial class ToplantiViewerWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("Portal", "ToplantiViewerWP")); } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ToplantiViewerWP()
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
        private string ToplantiIdQS
        {
            get
            {
                if (ViewState["ToplantiId"] == null)
                {
                    if (Page.Request.QueryString["ToplantiId"] != null)
                    {
                        ViewState["ToplantiId"] = Page.Request.QueryString["ToplantiId"];
                    }
                    else
                    {
                        ViewState["ToplantiId"] = string.Empty;
                    }
                }
                return ViewState["ToplantiId"].ToString();
            }
            set
            {
                ViewState["ToplantiId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) PortalWebPartLog.RecordOpened(LogContext);
                if (!Page.IsPostBack)
                {
                    KayitGetir();

                }
            });
        }
        private void KayitGetir()
        {
            var jsonArrayString = ToplantiListesiniGetir();

            var jsString = CreateJsString(jsonArrayString); //javascript kodu hazirlaniyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        private string ToplantiListesiniGetir()
        {
            Toplanti toplanti = new Toplanti();
            string json = new Model.Services.Portal.ToplantiService(LogContext).GetCalendarJson();
            return json;
        }
        private string CreateJsString(string jsonData)
        {
            DateTime date;
            if (string.IsNullOrWhiteSpace(InitialDateQS)) date = DateTime.Today;
            else if (!DateTime.TryParseExact(InitialDateQS, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out date)) throw new PortalInputException("Takvim tarihi geçerli değil.");
            string initialDate = "'" + date.ToString("yyyy-MM-dd") + "'";
            string view = CalendarViewQS;
            if (view != "timeGridWeek" && view != "timeGridDay" && view != "listWeek") view = "dayGridMonth";
            string calendarView = "'" + view + "'";
            string calendarStr =
                @"
                var toplantiId=0;
                document.addEventListener('DOMContentLoaded', function() {
                    var calendarEl = document.getElementById('calendar');
                    var calendar = new FullCalendar.Calendar(calendarEl, {
                        locale: 'tr',
                        firstDay: 1,
                        height: '650px',
                        initialView:  " + calendarView + @",//'dayGridMonth',
                        initialDate: " + initialDate + @",
                        events:" + jsonData + @",
                        headerToolbar: {
                            left: 'prev,next today',
                            center: 'title',
                            right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
                        },
                        editable: false,
                        droppable: false, // this allows things to be dropped onto the calendar
                        eventClick: function(info) {
                            info.jsEvent.preventDefault(); // don't let the browser navigate
                            toplantiId=info.event.id;
                            ToplantiDetaylariModal(info.event.id);
                        }
                    });
                    calendar.render();

                });
                ";
            return calendarStr;
        }
        protected void ToplantiDetaylariBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "ToplantiDetaylariBtn_Click", () =>
            {
                IcKatilimcilarCell.Text = string.Empty;
                BilgiCell.Text = string.Empty;
                int toplantiId = string.IsNullOrEmpty(paramToplantiIdLbl.Text) ? ToplantiIdQS.ConvertToInt() : paramToplantiIdLbl.Text.ConvertToInt();
                Toplanti toplanti = new Toplanti();
                toplanti = new Model.Services.Portal.ToplantiService(LogContext).GetById(toplantiId);
                if (toplanti == null) throw new PortalInputException("Toplantı bulunamadı. Takvimi yenileyin.");
                if (toplanti != null)
                {
                    IdLbl.Text = " ( Toplanti No: " + toplanti.Id.ToString() + " )";
                    ToplantiKonusuCell.Text = System.Web.HttpUtility.HtmlEncode(toplanti.ToplantiKonusu);
                    ToplantiYetkilisiCell.Text = ParseToplantiYetkilisi(toplanti.ToplantiYetkilisi);
                    KoordinatorCell.Text = ParseKoordinator(toplanti.Koordinator);
                    BaslangicZamaniCell.Text = toplanti.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm");
                    BitisZamaniCell.Text = toplanti.BitisTarihi.ToString("dd.MM.yyyy HH:mm");
                    DisKatilimcilarCell.Text = System.Web.HttpUtility.HtmlEncode(toplanti.DisKatilimcilar ?? string.Empty).Replace(",", "<br/>");

                    Personel personel = new Personel();
                    List<Personel> list = new PersonelService().GetMeetingParticipants(toplanti.Id, false, LogContext);
                    foreach (var item in list)
                    {
                        IcKatilimcilarCell.Text += System.Web.HttpUtility.HtmlEncode(item.Adi + " " + item.Soyadi) + "<br/>";
                    }
                    list = new PersonelService().GetMeetingParticipants(toplanti.Id, true, LogContext);
                    foreach (var item in list)
                    {
                        BilgiCell.Text += System.Web.HttpUtility.HtmlEncode(item.Adi + " " + item.Soyadi) + "<br/>";
                    }

                    ToplantiYeriCell.Text = ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger);
                    AciklamaCell.Text = System.Web.HttpUtility.HtmlEncode(toplanti.Aciklama);
                    CevrimIciCell.Text = toplanti.CevrimIci ? "Evet" : "Hayır";
                    IkramOnayiCell.Text = toplanti.IkramOnayi ? "Evet" : "Hayır";
                    IkramMalzemesiCell.Text = toplanti.IkramOnayi ? System.Web.HttpUtility.HtmlEncode(toplanti.IkramMalzemesi) : "-";
                }
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "OpenToplantiModal();", true);
            });
        }
        private string ParseToplantiYeri(int yeri, string diger)
        {
            string yeriStr = string.Empty;
            ToplantiParametre toplantiParametre = new ToplantiParametre();
            toplantiParametre = new Model.Services.Portal.ToplantiParametreService(LogContext).GetById(yeri);
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
    }
}
