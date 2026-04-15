using Model.IKYS;
using Model.MTS;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace MTS_WebParts.HaftalikGorunumWP
{
    [ToolboxItemAttribute(false)]
    public partial class HaftalikGorunumWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public HaftalikGorunumWP()
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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                KayitGetir();
            }
        }
        private void KayitGetir()
        {
            var randevuJsonData = FaaliyetListesiniGetir();
            var resmiTatilJsonData = ResmiTatilListesiniGetir();
            var kisiDogumGunleriJsonData = KisiDogumGunuListesiniGetir();
            var toplantiJsonData = ToplantiListesiniGetir();
            var personelDogumGunleriJsonData = PersonelDogumGunuListesiniGetir();

            randevuJsonData = randevuJsonData.Equals("[]") ? string.Empty : randevuJsonData;
            resmiTatilJsonData = resmiTatilJsonData.Equals("[]") ? string.Empty : resmiTatilJsonData;
            kisiDogumGunleriJsonData = kisiDogumGunleriJsonData.Equals("[]") ? string.Empty : kisiDogumGunleriJsonData;
            toplantiJsonData = toplantiJsonData.Equals("[]") ? string.Empty : toplantiJsonData;
            personelDogumGunleriJsonData = personelDogumGunleriJsonData.Equals("[]") ? string.Empty : personelDogumGunleriJsonData;

            randevuJsonData = string.IsNullOrEmpty(randevuJsonData) ? string.Empty : randevuJsonData.Replace("[{", "{").Replace("}]", "},");
            resmiTatilJsonData = string.IsNullOrEmpty(resmiTatilJsonData) ? string.Empty : resmiTatilJsonData.Replace("[{", "{").Replace("}]", "},");
            kisiDogumGunleriJsonData = string.IsNullOrEmpty(kisiDogumGunleriJsonData) ? string.Empty : kisiDogumGunleriJsonData.Replace("[{", "{").Replace("}]", "},");
            toplantiJsonData = string.IsNullOrEmpty(toplantiJsonData) ? string.Empty : toplantiJsonData.Replace("[{", "{").Replace("}]", "},");
            personelDogumGunleriJsonData = string.IsNullOrEmpty(personelDogumGunleriJsonData) ? string.Empty : personelDogumGunleriJsonData.Replace("[{", "{").Replace("}]", "},");

            string jsonArrayString =
                "[" +
                randevuJsonData +
                resmiTatilJsonData +
                kisiDogumGunleriJsonData +
                toplantiJsonData +
                personelDogumGunleriJsonData +
                "]";

            var jsString = CreateJsString(jsonArrayString); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        private string ToplantiListesiniGetir()
        {
            Toplanti toplantiDao = new Toplanti();
            DataTable dataTable = toplantiDao.SelectAllByKatilimci(ProjeConstants.GENELMUDUR_PERSONELID);

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

            string json = toplantiDao.ToJSON(eventItems);
            return json;
        }
        private string KisiDogumGunuListesiniGetir()
        {
            Kisi kisi = new Kisi();
            List<Kisi> kisiListesi = kisi.SelectByDogumGunuKutlamaReturnDT();
            List<CalendarEvent> eventItems = new List<CalendarEvent>();
            Faaliyet faaliyet = new Faaliyet();
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            foreach (var item in kisiListesi)
            {

                for (int i = -1; i < 2; i++)//geçen yıl, bu yıl ve gelecek yıl için d.günü göster
                {
                    DateTime dogumGunu = item.DogumTarihi;
                    DateTime dogumGunuBuYil = new DateTime(DateTime.Today.Year + i, dogumGunu.Month, dogumGunu.Day);

                    CalendarEvent dogumGunuitem = new CalendarEvent();
                    dogumGunuitem.state = ProjeConstants.FAALIYET_DURUMU_ONAYLANDI_INT.ToString();
                    dogumGunuitem.id = 999;//999 önemli taşınamayan event
                    dogumGunuitem.purpose = ProjeConstants.FAALIYET_AMACI_DOGUMGUNU_INT;
                    dogumGunuitem.title = "D.Günü :" + item.Adi + " " + item.Soyadi;
                    dogumGunuitem.start = string.Format("{0:s}", dogumGunuBuYil);
                    dogumGunuitem.end = string.Format("{0:s}", dogumGunuBuYil);

                    string linkUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_KISI_KARTI + "?KisiId=" + item.Id;

                    dogumGunuitem.url = linkUrl;
                    dogumGunuitem.allDay = true;
                    dogumGunuitem.startEditable = false;

                    faaliyet.RenkBelirle(dogumGunuitem);
                    eventItems.Add(dogumGunuitem);
                }
            }

            string json = faaliyet.ToJSON(eventItems);
            return json;
        }
        private string PersonelDogumGunuListesiniGetir()
        {
            Personel personel = new Personel();
            DataTable dataTable = personel.SelectCalisanPersonelListesiReturnDataTable();
            List<CalendarEvent> eventItems = new List<CalendarEvent>();
            Faaliyet randevu = new Faaliyet();

            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    for (int i = -1; i < 2; i++)//geçen yıl, bu yıl ve gelecek yıl için d.günü göster
                    {
                        string adi = row["Adi"].ToString();
                        string soyadi = row["Soyadi"].ToString();
                        DateTime dogumGunu = row["DogumTar"].ConvertToDatetime();
                        DateTime dogumGunuBuYil = new DateTime(DateTime.Today.Year + i, dogumGunu.Month, dogumGunu.Day);
                        bool medeniHali = row["MedeniHali"].ConvertToInt() > 0;
                        DateTime evlilikTar = row["EvlilikTar"].ConvertToDatetime();
                        DateTime evlilikTarBuYil = new DateTime(DateTime.Today.Year + i, evlilikTar.Month, evlilikTar.Day);
                        bool evlilikKutlama = row["EvlilikKutlama"].ConvertToBool();


                        CalendarEvent dogumGunuitem = new CalendarEvent();
                        dogumGunuitem.state = ProjeConstants.FAALIYET_DURUMU_ONAYLANDI_INT.ToString();
                        dogumGunuitem.id = 999;//999 önemli taşınamayan event
                        dogumGunuitem.purpose = ProjeConstants.FAALIYET_AMACI_DOGUMGUNU_INT;
                        dogumGunuitem.title = "D.Günü :" + adi + " " + soyadi;
                        dogumGunuitem.start = string.Format("{0:s}", dogumGunuBuYil);
                        dogumGunuitem.end = string.Format("{0:s}", dogumGunuBuYil);
                        dogumGunuitem.url = "";
                        dogumGunuitem.allDay = true;
                        dogumGunuitem.startEditable = false;

                        randevu.RenkBelirle(dogumGunuitem);
                        eventItems.Add(dogumGunuitem);

                        if (medeniHali && evlilikKutlama)
                        {
                            CalendarEvent evlilikYildonumuItem = new CalendarEvent();
                            evlilikYildonumuItem.state = ProjeConstants.FAALIYET_DURUMU_ONAYLANDI_INT.ToString();
                            evlilikYildonumuItem.id = 999;//999 önemli taşınamayan event
                            evlilikYildonumuItem.purpose = ProjeConstants.FAALIYET_AMACI_YILDONUMU_INT;
                            evlilikYildonumuItem.title = "Evl.Yıld. :" + adi + " " + soyadi;
                            evlilikYildonumuItem.start = string.Format("{0:s}", evlilikTarBuYil);
                            evlilikYildonumuItem.end = string.Format("{0:s}", evlilikTarBuYil);
                            evlilikYildonumuItem.url = "";
                            evlilikYildonumuItem.allDay = true;
                            evlilikYildonumuItem.startEditable = false;

                            randevu.RenkBelirle(evlilikYildonumuItem);
                            eventItems.Add(evlilikYildonumuItem);
                        }
                    }
                }
            }
            string json = randevu.ToJSON(eventItems);
            return json;
        }
        private string FaaliyetListesiniGetir()
        {
            Faaliyet randevu = new Faaliyet();
            string json = randevu.SelectAllReturnJson(ProjeConstants.FAALIYET_ACIKTARIHLI_DEGIL);
            return json;
        }
        private string ResmiTatilListesiniGetir()
        {
            DateTime basTar = new DateTime(DateTime.Today.AddYears(-1).Year, 1, 1);
            DateTime bitTar = new DateTime(DateTime.Today.AddYears(1).Year, 12, 31);
            ResmiTatil resmiTatil = new ResmiTatil();
            string json = resmiTatil.SelectAllReturnJson(basTar, bitTar);
            return json;
        }
        private string CreateJsString(string jsonData)
        {
            string initialDate = string.IsNullOrEmpty(InitialDateQS) ?
                DateTime.Today.ToString("yyyy-MM-dd").ReturnQuotedValue().ToString() :
                InitialDateQS.ReturnQuotedValue().ToString();
            string calendarView = string.IsNullOrEmpty(CalendarViewQS) ? "'dayGridMonth'" : CalendarViewQS.ReturnQuotedValue().ToString();
            string calendarStr = @" 
            document.addEventListener('DOMContentLoaded', function() {

                /* initialize the calendar
                -----------------------------------------------------------------*/

                var calendarEl = document.getElementById('calendar');
                var calendar = new FullCalendar.Calendar(calendarEl, {
                    locale: 'tr',
                    firstDay: 1,
                    initialView:  " + calendarView + @",//'dayGridMonth',
                    initialDate: " + initialDate + @",
                    events:" + jsonData + @",
                    //headerToolbar: {
                    //left: 'prev,next today',
                    //center: 'title',
                    //right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
                    //},
headerToolbar: {
      left: 'prev,next',
      center: 'title',
      right: 'timeGridDay,timeGridFourDay,timeGridWeek',timeGridMonth
    },
    views: {
      timeGridFourDay: {
        type: 'timeGrid',
        duration: { days: 4 },
        buttonText: '4 day'
      }
    },
                    editable: true,
                    eventAllow: function(dropLocation, draggedEvent) { //resmi tatillerin draggable olmaması için
                        if (draggedEvent.id === '999') {
                            return false; // a boolean
                        }
                        else {
                            return true; // or return false to disallow
                        }
                    },
                    eventResize: function(info) {
                        if (!info.StartEditable){
                            info.revert();
                        }else
                        {
                            if (!confirm('Faaliyet saati değiştirilsin mi?'))
                            {
                                info.revert();
                            }else{
                                var id=info.event.id;
                                var start=info.event.start.toISOString();
                                var end=info.event.end.toISOString();
                                var view = calendar.view.type;
                                FaaliyetKaydet(id, start, end, view)
                            }
                        }
                    },
                    eventDrop: function(info) {
                        if (!info.StartEditable){
                            info.revert();
                        }else
                        {
                            if (!confirm('Faaliyet Taşınsın mı?'))
                            {
                                info.revert();
                            }else{
                                var id=info.event.id;
                                var start=info.event.start.toISOString();
                                var end=info.event.end.toISOString();
                                var view = calendar.view.type;
                                FaaliyetKaydet(id, start, end, view)
                            }
                        }
                    },
                    droppable: true, // this allows things to be dropped onto the calendar
                    drop: function(info) {
                        if (!confirm('Faaliyet kaydedilsin mi?'))
                        {
                            info.revert();
                        }else
                        {
                            // if so, remove the element from the 'Draggable Events' list
                            info.draggedEl.parentNode.removeChild(info.draggedEl);
                            var id=info.draggedEl.id;
                            var start=info.date.toISOString();
                            var end=info.date.toISOString();
                            var view = calendar.view.type;
                            FaaliyetKaydet(id, start, end,view)
                        }
                    },
                    eventClick: function(info) {
                        //info.jsEvent.preventDefault(); // don't let the browser navigate

                        if (info.event.url=='Toplanti') {
                            info.jsEvent.preventDefault();
                            toplantiId=info.event.id;
                            if (toplantiId>0)
                                ToplantiDetaylariModal(info.event.id);
                        }
                        if (info.event.url) {
                            window.location(info.event.url);
                            info.jsEvent.preventDefault();
                        }
                    }
                    });
                    calendar.render();

                    });

                                    ";


            return calendarStr;
        }

        Random random = new Random();
        protected void FaaliyetKaydetNowBtn_Click(object sender, EventArgs e)
        {
            int faaliyetId = paramFaaliyetId.Value.ConvertToInt();
            DateTime basTar = paramBasTar.Value.ConvertToDatetime();
            DateTime bitTar = paramBitTar.Value.ConvertToDatetime();
            CalendarViewQS = paramView.Value;
            InitialDateQS = basTar.ToString("yyyy-MM-dd");
            if (faaliyetId > 0)
            {
                Faaliyet randevu = new Faaliyet();
                randevu = randevu.Select(faaliyetId);
                if (randevu != null)
                {
                    randevu.BaslangicTarihi = basTar;
                    randevu.BaslangicSaati = basTar.ToString("HH:mm");
                    randevu.BitisTarihi = bitTar;
                    randevu.BitisSaati = bitTar.ToString("HH:mm");
                    randevu.AcikTarih = ProjeConstants.FAALIYET_ACIKTARIHLI_DEGIL.ConvertToBool();
                    if (randevu.Update())
                    {
                        RedirectToPage(ProjeConstants.PAGE_FAALIYET_TAKVIM + "?CalendarView=" + CalendarViewQS + "&InitialDate=" + InitialDateQS);
                    }

                }
            }
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        #region Toplanti Ayrıntıları Popup
        protected void ToplantiDetaylariBtn_Click(object sender, EventArgs e)
        {
            IcKatilimcilarCell.Text = string.Empty;
            BilgiCell.Text = string.Empty;
            int toplantiId = paramToplantiIdLbl.Text.ConvertToInt();
            Toplanti toplanti = new Toplanti();
            toplanti = toplanti.Select(toplantiId);
            if (toplanti != null)
            {
                IdLbl.Text = " ( Toplantı No: " + toplanti.Id.ToString() + " )";
                ToplantiKonusuCell.Text = toplanti.ToplantiKonusu;
                ToplantiYetkilisiCell.Text = ParseToplantiYetkilisi(toplanti.ToplantiYetkilisi);
                KoordinatorCell.Text = ParseKoordinator(toplanti.Koordinator);
                BaslangicZamaniCell.Text = toplanti.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm");
                BitisZamaniCell.Text = toplanti.BitisTarihi.ToString("dd.MM.yyyy HH:mm");
                DisKatilimcilarCell.Text = toplanti.DisKatilimcilar.Replace(",", "</br>");

                Personel personel = new Personel();
                List<Personel> list = personel.SelectKatilimcilarByToplantiIdList(toplanti.Id);

                foreach (var item in list)
                {
                    IcKatilimcilarCell.Text += item.Adi + " " + item.Soyadi + "</br>";
                }

                list = personel.SelectBilgiVerilenlerByToplantiIdList(toplanti.Id);
                foreach (var item in list)
                {
                    BilgiCell.Text += item.Adi + " " + item.Soyadi + "</br>";
                }

                ToplantiYeriCell.Text = ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger);
                AciklamaCell.Text = toplanti.Aciklama;
                CevrimIciCell.Text = toplanti.CevrimIci ? "Evet" : "Hayır";
                IkramOnayiCell.Text = toplanti.IkramOnayi ? "Evet" : "Hayır";
                IkramMalzemesiCell.Text = toplanti.IkramOnayi ? toplanti.IkramMalzemesi : "-";

            }
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), "OpenToplantiModal();", true);
        }
        private string ParseToplantiYeri(int yeri, string diger)
        {
            string yeriStr = string.Empty;
            ToplantiParametre toplantiParametre = new ToplantiParametre();
            toplantiParametre = toplantiParametre.Select(yeri);
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
            BirimTanim birimTanim = new BirimTanim();
            birimTanim = birimTanim.Select<BirimTanim>(koordinator);
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
            personel = personel.Select(toplantiYetkilisi);
            if (personel != null)
            {
                yetkiliStr = personel.Adi + " " + personel.Soyadi;
            }
            return yetkiliStr;
        }
        #endregion
    }
}
