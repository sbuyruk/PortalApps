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
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;


namespace MTS_WebParts.FaaliyetROViewerWP
{
    [ToolboxItemAttribute(false)]
    public partial class FaaliyetROViewerWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("MTS", "FaaliyetROViewerWP")); } }
        private static string Encode(object value) { return HttpUtility.HtmlEncode(Convert.ToString(value)); }

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FaaliyetROViewerWP()
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
                AcikTarihliFaaliyetListesiniGetir();
            });
        }
        private void KayitGetir()
        {
            var faaliyetJsonData = FaaliyetListesiniGetir();
            var resmiTatilJsonData = ResmiTatilQS.ConvertToBool() ? ResmiTatilListesiniGetir():string.Empty;
            var toplantiJsonData = ToplantiQS.ConvertToBool() ? ToplantiListesiniGetir():string.Empty;
            var personelDogumGunleriJsonData = DogumGunuQS.ConvertToBool() ? PersonelDogumGunuListesiniGetir() :string.Empty;
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
        private string PersonelDogumGunuListesiniGetir()
        {
            return MtsCalendar.EmployeeCelebrations(LogContext);
        }
        private string KisiDogumGunuListesiniGetir()
        {
            return MtsCalendar.ContactBirthdays(LogContext);
        }
        private string FaaliyetListesiniGetir()
        {
            Faaliyet faaliyet = new Faaliyet();
            string json = new Model.Services.MTS.FaaliyetService(LogContext).GetCalendarJson(ProjeConstants.FAALIYET_ACIKTARIHLI_DEGIL);
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
                    editable: false,
                    eventDataTransform: function(event) {
                        event.startEditable = false;
                        event.durationEditable = false;
                        return event;
                    },
                    eventAllow: function(dropLocation, draggedEvent) { //resmi tatillerin draggable olmamasi için
                        if (draggedEvent.id === '999') {
                            return false; // a boolean
                        }
                        else {
                            return true; // or return false to disallow
                        }
                    },
                    eventResize: function(info) {
                        //Do Not Edit
                        info.revert();
                    },
                    droppable: false, // this prevents things to be dropped onto the calendar
                    eventClick: function(info) {
                        //info.jsEvent.preventDefault(); // don't let the browser navigate

                        if (info.event.url=='Toplanti') {
                            info.jsEvent.preventDefault();
                            var toplantiId=info.event.id;
                            if (toplantiId>0)
                                ToplantiDetaylariModal(info.event.id);
                        }
                        else if (info.event.url) {

                            info.jsEvent.preventDefault();
                            var faaliyetId=info.event.id;
                            if (faaliyetId>0)
                                FaaliyetDetaylariModal(info.event.id);
                        }
                    }
                    });
                    calendarEl.mtsCalendar = calendar;
                    calendar.render();

                })();

            ";


            return calendarStr;
        }
        private void AcikTarihliFaaliyetListesiniGetir()
        {
            AcikTarihliFaaliyetListDiv.Controls.Clear();
            List<Faaliyet> list = new Model.Services.MTS.FaaliyetService(LogContext).GetAll(ProjeConstants.FAALIYET_ACIKTARIHLI);
            foreach (var item in list)
            {
                CreateDiv(item.Id.ToString(), item.FaaliyetKonusu);
            }
        }
        private void CreateDiv(string divId, string value)
        {
            HtmlGenericControl outerDiv = new HtmlGenericControl("div");

            outerDiv.Attributes.Add("class", "bg-light border border-dark fc-event fc-h-event fc-daygrid-event fc-daygrid-block-event title-wrap");
            outerDiv.Attributes.Add("id", divId);

            HtmlGenericControl innerDiv = new HtmlGenericControl("div");
            innerDiv.InnerText=value;
            outerDiv.Controls.Add(innerDiv);
            AcikTarihliFaaliyetListDiv.Controls.Add(outerDiv);
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
                UtilityHelper.ScriptCalistir("OpenToplantiModal();");

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
        #region Faaliyet Ayrintilari Popup
        protected void FaaliyetDetaylariBtn_Click(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "FaaliyetDetaylariBtn_Click", () =>
            {
                int faaliyetId = paramFaaliyetIdLbl.Text.ConvertToInt();
                Faaliyet faaliyet = new Faaliyet();
                faaliyet = new Model.Services.MTS.FaaliyetService(LogContext).GetById(faaliyetId);
                if (faaliyet == null) throw new MtsInputException("Faaliyet bulunamadı. Takvimi yenileyip yeniden seçin.");
                if (faaliyet != null)
                {
                    FaaliyetIdLbl.Text = " ( Faaliyet No: " + faaliyet.Id.ToString() + " )";
                    FaaliyetKonusuCell.Text = Encode(faaliyet.FaaliyetKonusu);
                    FaaliyetBaslangicZamaniCell.Text = faaliyet.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm");
                    FaaliyetBitisZamaniCell.Text = faaliyet.BitisTarihi.ToString("dd.MM.yyyy HH:mm");
                    KatilimcilarCell.Text= GetDataList(faaliyetId);
                    string faaliyetYeriStr = faaliyet.FaaliyetYeriStr;
                    FaaliyetYeriCell.Text = Encode(faaliyetYeriStr);
                    FaaliyetAciklamaCell.Text = Encode(faaliyet.Aciklama);
                    FaaliyetTipiCell.Text = Encode(faaliyet.FaaliyetTipi);
                    string faaliyetAmaciStr = ParseFaaliyetAmaci(faaliyet.FaaliyetAmaciId.ToString());
                    string faaliyetDurumuStr = MTSOrtak.ParseFaaliyetDurumu(faaliyet.FaaliyetDurumu);

                    FaaliyetAmaciCell.Text = faaliyetAmaciStr;
                    FaaliyetDurumuCell.Text = faaliyetDurumuStr;

                }
                UtilityHelper.ScriptCalistir("OpenFaaliyetModal();");

            });
        }

        private string GetDataList(int faaliyetId)
        {
            Faaliyet faaliyetDao = new Faaliyet();
            System.Data.DataTable dataTable = new Model.Services.MTS.FaaliyetService(LogContext).GetParticipantList(faaliyetId, 3, ProjeConstants.HEPSI, ProjeConstants.NULL_TARIH, ProjeConstants.NULL_TARIH, ProjeConstants.HEPSI);
            StringBuilder sb = new StringBuilder();
            int sirano = 1;
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string katilimId = row["KatilimId"].ToString();
                    string katilimciId = row["KatilimciId"].ToString();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();
                    string kurumu = row["Kurumu"].ToString();
                    sb.Append(string.Format("{0}. {1} {2} {3} <br>", sirano++, Encode(adi), Encode(soyadi), string.IsNullOrEmpty(kurumu) ? string.Empty : "(" + Encode(kurumu) + ")"));
                }
            }
            return sb.ToString();
        }
        private string ParseFaaliyetAmaci(string amac)
        {
            string amacStr = string.Empty;
            switch (amac)
            {
                case ProjeConstants.FAALIYET_AMACI_TOPLANTI_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_TOPLANTI;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_ZIYARET_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_ZIYARET;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_GORUSME_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_GORUSME;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_DAVET_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_DAVET;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_YILDONUMU_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_YILDONUMU;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_DOGUMGUNU_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_DOGUMGUNU_INT;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_OZELCALISMA_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_OZELCALISMA;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_IZIN_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_IZIN;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_RESMITATIL_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_RESMITATIL;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_SEYAHAT_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_SEYAHAT;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_BILGI_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_BILGI;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_VAKIF_TOPLANISI_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_VAKIF_TOPLANISI;
                        break;
                    }
                default:
                    break;
            }
            return amacStr;
        }

        #endregion

        protected void VakifIciKutlamaChk_CheckedChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "VakifIciKutlamaChk_CheckedChanged", () =>
            {
                DogumGunuQS = VakifIciKutlamaChk.Checked.ToString();

                AcikTarihliFaaliyetListesiniGetir();

            });
        }

        protected void VakifDisiKutlamaChk_CheckedChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "VakifDisiKutlamaChk_CheckedChanged", () =>
            {
                KisiDogumGunuQS = VakifDisiKutlamaChk.Checked.ToString();

                AcikTarihliFaaliyetListesiniGetir();

            });
        }

        protected void ResmiTatilChk_CheckedChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "ResmiTatilChk_CheckedChanged", () =>
            {
                ResmiTatilQS = ResmiTatilChk.Checked.ToString();

                AcikTarihliFaaliyetListesiniGetir();

            });
        }

        protected void ToplantiChk_CheckedChanged(object sender, EventArgs e)
        {
            MtsWebPartLog.ExecuteAction(LogContext, "ToplantiChk_CheckedChanged", () =>
            {
                ToplantiQS = ToplantiChk.Checked.ToString();

                AcikTarihliFaaliyetListesiniGetir();

            });
        }
    }
}
