using Model.IKYS;
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
            if (!Page.IsPostBack)
            {
                KayitGetir();
                AcikTarihliFaaliyetListesiniGetir();
            }
        }
        private void KayitGetir()
        {
            var faaliyetJsonData = FaaliyetListesiniGetir();
            var resmiTatilJsonData = ResmiTatilQS.ConvertToBool() ? ResmiTatilListesiniGetir():string.Empty;
            var toplantiJsonData = ToplantiQS.ConvertToBool() ? ToplantiListesiniGetir():string.Empty;
            var personelDogumGunleriJsonData = DogumGunuQS.ConvertToBool() ? PersonelDogumGunuListesiniGetir() :string.Empty;
            var kisiDogumGunleriJsonData = KisiDogumGunuQS.ConvertToBool() ? KisiDogumGunuListesiniGetir() : string.Empty;

            faaliyetJsonData = faaliyetJsonData.Equals("[]") ? string.Empty : faaliyetJsonData;
            resmiTatilJsonData = resmiTatilJsonData.Equals("[]") ? string.Empty : resmiTatilJsonData;
            toplantiJsonData = toplantiJsonData.Equals("[]") ? string.Empty : toplantiJsonData;
            personelDogumGunleriJsonData = personelDogumGunleriJsonData.Equals("[]") ? string.Empty : personelDogumGunleriJsonData;
            kisiDogumGunleriJsonData = kisiDogumGunleriJsonData.Equals("[]") ? string.Empty : kisiDogumGunleriJsonData;

            faaliyetJsonData = string.IsNullOrEmpty(faaliyetJsonData) ? string.Empty : faaliyetJsonData.Replace("[{", "{").Replace("}]", "},");
            resmiTatilJsonData = string.IsNullOrEmpty(resmiTatilJsonData) ? string.Empty : resmiTatilJsonData.Replace("[{", "{").Replace("}]", "},");
            toplantiJsonData = string.IsNullOrEmpty(toplantiJsonData) ? string.Empty : toplantiJsonData.Replace("[{", "{").Replace("}]", "},");
            personelDogumGunleriJsonData = string.IsNullOrEmpty(personelDogumGunleriJsonData) ? string.Empty : personelDogumGunleriJsonData.Replace("[{", "{").Replace("}]", "},");
            kisiDogumGunleriJsonData = string.IsNullOrEmpty(kisiDogumGunleriJsonData) ? string.Empty : kisiDogumGunleriJsonData.Replace("[{", "{").Replace("}]", "},");

            string jsonArrayString =
                "[" +
                faaliyetJsonData +
                resmiTatilJsonData +
                toplantiJsonData +
                personelDogumGunleriJsonData +
                kisiDogumGunleriJsonData +
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
        private string PersonelDogumGunuListesiniGetir()
        {
            Personel personel = new Personel();
            DataTable dataTable = personel.SelectCalisanPersonelListesiReturnDataTable();
            List<CalendarEvent> eventItems = new List<CalendarEvent>();
            Faaliyet faaliyet = new Faaliyet();

            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    for (int i = -1; i < 2; i++)//geçen yıl, bu yıl ve gelecek yıl için d.günü göster
                    {
                        string adi = row["Adi"].ToString();
                        string soyadi = row["Soyadi"].ToString();
                        DateTime dogumTar = row["DogumTar"].ConvertToDatetime();
                        DateTime dogumGunuBuYil = new DateTime(DateTime.Today.Year + i, dogumTar.Month, dogumTar.Day);
                        bool dogumGunuKutlama = row["DogumGunuKutlama"].ConvertToBool();
                        bool medeniHali = row["MedeniHali"].ConvertToInt() == 1 ? true : false;
                        DateTime evlilikTar = row["EvlilikTar"].ConvertToDatetime();
                        DateTime evlilikTarBuYil = new DateTime(DateTime.Today.Year + i, evlilikTar.Month, evlilikTar.Day);
                        bool evlilikKutlama = row["EvlilikKutlama"].ConvertToBool();


                        if (dogumTar > ProjeConstants.NULL_TARIH && dogumGunuKutlama)
                        {
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

                            faaliyet.RenkBelirle(dogumGunuitem);
                            eventItems.Add(dogumGunuitem);
                        }

                        if (evlilikTar > ProjeConstants.NULL_TARIH && medeniHali && evlilikKutlama)
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

                            faaliyet.RenkBelirle(evlilikYildonumuItem);
                            eventItems.Add(evlilikYildonumuItem);
                        }
                    }
                }
            }
            string json = faaliyet.ToJSON(eventItems);
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
        private string FaaliyetListesiniGetir()
        {
            Faaliyet faaliyet = new Faaliyet();
            string json = faaliyet.SelectAllReturnJson(ProjeConstants.FAALIYET_ACIKTARIHLI_DEGIL);
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
                    headerToolbar: {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
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
                    calendar.render();

                });

            ";


            return calendarStr;
        }
        private void AcikTarihliFaaliyetListesiniGetir()
        {
            Faaliyet faaliyet = new Faaliyet();
            List<Faaliyet> list = faaliyet.SelectAllReturnList(ProjeConstants.FAALIYET_ACIKTARIHLI);
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
            UtilityHelper.ScriptCalistir("OpenToplantiModal();");
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
        #region Faaliyet Ayrıntıları Popup
        protected void FaaliyetDetaylariBtn_Click(object sender, EventArgs e)
        {
            int faaliyetId = paramFaaliyetIdLbl.Text.ConvertToInt();
            Faaliyet faaliyet = new Faaliyet();
            faaliyet = faaliyet.Select(faaliyetId);
            if (faaliyet != null)
            {
                FaaliyetIdLbl.Text = " ( Faaliyet No: " + faaliyet.Id.ToString() + " )";
                FaaliyetKonusuCell.Text = faaliyet.FaaliyetKonusu;
                FaaliyetBaslangicZamaniCell.Text = faaliyet.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm");
                FaaliyetBitisZamaniCell.Text = faaliyet.BitisTarihi.ToString("dd.MM.yyyy HH:mm");
                KatilimcilarCell.Text= GetDataList(faaliyetId);
                string faaliyetYeriStr = faaliyet.FaaliyetYeriStr;
                FaaliyetYeriCell.Text =faaliyetYeriStr;
                FaaliyetAciklamaCell.Text = faaliyet.Aciklama;
                FaaliyetTipiCell.Text = faaliyet.FaaliyetTipi;
                string faaliyetAmaciStr = ParseFaaliyetAmaci(faaliyet.FaaliyetAmaciId.ToString());
                string faaliyetDurumuStr = MTSOrtak.ParseFaaliyetDurumu(faaliyet.FaaliyetDurumu);
                
                FaaliyetAmaciCell.Text = faaliyetAmaciStr;
                FaaliyetDurumuCell.Text = faaliyetDurumuStr;

            }
            UtilityHelper.ScriptCalistir("OpenFaaliyetModal();");
        }

        private string GetDataList(int faaliyetId)
        {
            Faaliyet faaliyetDao = new Faaliyet();
            System.Data.DataTable dataTable = faaliyetDao.SelectAllByKatilimciFaaliyetReturnDataTable(faaliyetId,3,ProjeConstants.HEPSI, ProjeConstants.NULL_TARIH, ProjeConstants.NULL_TARIH, ProjeConstants.HEPSI);
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
                    sb.Append(string.Format("{0}. {1} {2} {3} <br>", sirano++,adi,soyadi, string.IsNullOrEmpty(kurumu)?string.Empty: "(" +kurumu + ")"));
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
            DogumGunuQS = VakifIciKutlamaChk.Checked.ToString();
            KayitGetir();
            AcikTarihliFaaliyetListesiniGetir();
        }

        protected void VakifDisiKutlamaChk_CheckedChanged(object sender, EventArgs e)
        {
            KisiDogumGunuQS = VakifDisiKutlamaChk.Checked.ToString();
            KayitGetir();
            AcikTarihliFaaliyetListesiniGetir();
        }

        protected void ResmiTatilChk_CheckedChanged(object sender, EventArgs e)
        {
            ResmiTatilQS = ResmiTatilChk.Checked.ToString();
            KayitGetir();
            AcikTarihliFaaliyetListesiniGetir();
        }

        protected void ToplantiChk_CheckedChanged(object sender, EventArgs e)
        {
            ToplantiQS = ToplantiChk.Checked.ToString();
            KayitGetir();
            AcikTarihliFaaliyetListesiniGetir();
        }
    }
}
