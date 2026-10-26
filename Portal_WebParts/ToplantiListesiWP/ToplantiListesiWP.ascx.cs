using Portal_WebParts.Logging;
using Utility.Logging;
using Model.Services.Portal;
using Model.IKYS;
using Model.Services.IKYS;
using Model.Portal;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.ToplantiListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class ToplantiListesiWP : WebPart
    {
        private PortalLogContext logContext;
        private readonly Dictionary<int, bool> meetingOwnerPermissions = new Dictionary<int, bool>();
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("Portal", "ToplantiListesiWP")); } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ToplantiListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string PersonelIdQS
        {
            get
            {

                if (ViewState["PersonelId"] == null)
                {
                    if (Page.Request.QueryString["PersonelId"] != null)
                    {
                        ViewState["PersonelId"] = Page.Request.QueryString["PersonelId"];
                    }
                    else
                    {
                        ViewState["PersonelId"] = string.Empty;
                    }
                }
                return ViewState["PersonelId"].ToString();
            }

            set
            {
                ViewState["PersonelId"] = value;
            }
        }
        private string SecilenToplantiIdQS
        {
            get
            {

                if (ViewState["SecilenToplantiId"] == null)
                {
                    if (Page.Request.QueryString["SecilenToplantiId"] != null)
                    {
                        ViewState["SecilenToplantiId"] = Page.Request.QueryString["SecilenToplantiId"];
                    }
                    else
                    {
                        ViewState["SecilenToplantiId"] = string.Empty;
                    }
                }
                return ViewState["SecilenToplantiId"].ToString();
            }

            set
            {
                ViewState["SecilenToplantiId"] = value;
            }
        }
        private string CurrentUserName
        {
            get
            {

                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUserLoginName();
                }
                return ViewState["CurrentUserName"].ToString();
            }

            set
            {
                ViewState["CurrentUserName"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) PortalWebPartLog.RecordOpened(LogContext);
                if (!Page.IsPostBack)
                {
                    BitisTarihiTxt.Text = DateTime.Today.AddMonths(12).ConvertToDatetimeEmptyIfNull();
                    BaslangicTarihiTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                    TabloOlustur();
                }
            });
        }
        protected void BitisTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "BitisTarihiTxt_TextChanged", () =>
            {
                TabloOlustur();
            });
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazirlaniyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            List<ToplantiListItem> list = GetDataList();
            var serializer = new JavaScriptSerializer();
            jSon = serializer.Serialize(list).Replace("<", "\\u003c").Replace(">", "\\u003e").Replace("&", "\\u0026");

            return jSon;
        }
        private List<ToplantiListItem> GetDataList()
        {
            DateTime start, end;
            var culture = System.Globalization.CultureInfo.GetCultureInfo("tr-TR");
            if (!DateTime.TryParse(BaslangicTarihiTxt.Text, culture, System.Globalization.DateTimeStyles.None, out start)
                || !DateTime.TryParse(BitisTarihiTxt.Text, culture, System.Globalization.DateTimeStyles.None, out end)
                || end.Date < start.Date || end.Date == DateTime.MaxValue.Date)
                throw new PortalInputException("Geçerli bir başlangıç ve bitiş tarihi girin; bitiş başlangıçtan önce olamaz.");
            string seperator = "; ";
            List<ToplantiListItem> toplantiList = new List<ToplantiListItem>();
            Toplanti toplantiDao = new Toplanti();
            DataTable dataTable = new Model.Services.Portal.ToplantiService(LogContext).GetByParticipantMeetingDate(0,
                ProjeConstants.HEPSI, null, start.Date, end.Date.AddDays(1).AddMilliseconds(-3));

            if (dataTable != null)
            {
                int tempToplantiId = 0;
                int katilimciAdedi = 0;

                ToplantiListItem tempToplantiListItem = new ToplantiListItem();
                foreach (DataRow row in dataTable.Rows)
                {
                    int toplantiId = row["ToplantiId"].ConvertToInt();
                    int toplantiYetkilisi = row["ToplantiYetkilisi"].ConvertToInt();
                    int toplantiYeri = row["ToplantiYeri"].ConvertToInt();
                    string toplantiYeriDiger = row["ToplantiYeriDiger"].ToString();
                    string toplantiKonusu = row["ToplantiKonusu"].ToString();
                    int toplantiKoordinator = row["Koordinator"].ConvertToInt();
                    string toplantiDisKatilimcilar = row["DisKatilimcilar"].ToString();

                    DateTime basTar = row["BaslangicTarihi"].ConvertToDatetime();
                    DateTime bitTar = row["BitisTarihi"].ConvertToDatetime();

                    string toplantiZamani =
                        basTar.Year == bitTar.Year && basTar.Month == bitTar.Month && basTar.Day == bitTar.Day ?
                        basTar.ToString("dd.MM.yyyy") + " " + basTar.ToString("HH:mm") + "-" + bitTar.ToString("HH:mm") :
                        basTar.ToString("dd.MM.yyyy HH:mm") + " - " + bitTar.ToString("dd.MM.yyyy HH:mm");

                    string baslangicTarihi = basTar.ToString("dd.MM.yyyy HH:mm");
                    string bitisTarihi = bitTar.ToString("dd.MM.yyyy HH:mm");

                    string toplantiKoordinatorStr = ParseKoordinator(toplantiKoordinator, toplantiYetkilisi);
                    string toplantiYeriStr = ParseToplantiYeri(toplantiYeri, toplantiYeriDiger);
                    bool bilgi = row["Bilgi"].ConvertToBool();
                    string adiSoyadi = row["Adi"].ToString() + " " + row["Soyadi"].ToString();
                    string katilimci = !bilgi ? adiSoyadi : string.Empty;
                    string bilgiVerilenAdiSoyadi = bilgi ? adiSoyadi : string.Empty;
                    bool cevrimIci = row["CevrimIci"].ReturnZeroIfNull().ConvertToBool();
                    string aciklama = row["Aciklama"].ReturnEmptyIfNull().ToString();

                    if (tempToplantiId == toplantiId)
                    {
                        tempToplantiId = toplantiId;
                        toplantiList.Remove(tempToplantiListItem);

                        //tempRandevuListItem.Katilimci += "@" + adiSoyadi;
                        katilimciAdedi++;
                        tempToplantiListItem.Katilimci += string.IsNullOrEmpty(katilimci) ? string.Empty : katilimci + seperator;
                        tempToplantiListItem.BilgiVerilenler += string.IsNullOrEmpty(bilgiVerilenAdiSoyadi) ? string.Empty : bilgiVerilenAdiSoyadi + seperator;
                        toplantiList.Add(tempToplantiListItem);
                    }
                    if (tempToplantiId != toplantiId)
                    {
                        ToplantiListItem toplantiListItem = new ToplantiListItem();
                        toplantiListItem.ToplantiId = toplantiId.ToString();
                        toplantiListItem.ToplantiKonusu = toplantiKonusu;
                        toplantiListItem.BaslangicTarihi = baslangicTarihi;
                        toplantiListItem.BitisTarihi = bitisTarihi;

                        toplantiListItem.BaslangicTarihi = baslangicTarihi;

                        toplantiListItem.BitisTarihi = bitisTarihi;

                        toplantiListItem.DisKatilimcilar = toplantiDisKatilimcilar;

                        toplantiListItem.ToplantiYeriDiger = toplantiYeriDiger;
                        toplantiListItem.Koordinator = toplantiKoordinatorStr;
                        toplantiListItem.ToplantiYeri = toplantiYeriStr;
                        toplantiListItem.PasifToplanti = bitTar < DateTime.Now ? ProjeConstants.TOPLANTI_PASIF_BOOL : false;
                        toplantiListItem.SecilenToplanti = SecilenToplantiIdQS.Equals(toplantiListItem.ToplantiId);
                        toplantiListItem.CevrimIci = cevrimIci ? "Evet" : "Hayir";
                        toplantiListItem.Aciklama = aciklama;

                        toplantiListItem.Katilimci += string.IsNullOrEmpty(katilimci) ? string.Empty : katilimci + seperator;
                        toplantiListItem.BilgiVerilenler += string.IsNullOrEmpty(bilgiVerilenAdiSoyadi) ? string.Empty : bilgiVerilenAdiSoyadi + seperator;
                        if (toplantiListItem.BitisTarihi.ConvertToDatetime() < DateTime.Now)
                        {
                            toplantiListItem.Duzenle = string.Empty;
                        }
                        else
                            toplantiListItem.Duzenle = ToplantiYetkisineGoreDegerAta(toplantiId, toplantiYetkilisi);

                        tempToplantiListItem = toplantiListItem;
                        toplantiList.Add(tempToplantiListItem);
                    }
                    tempToplantiId = toplantiId;

                }
            }
            //toplantiList = randevuList.OrderBy(r => r.ToplantiTarihi).ToList();
            return toplantiList;
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                jQuery('#CustomDataTable').DataTable({
                    'initComplete': function (settings, json) {//tablo yüklendiginde
                        var api = this.api();
                        var row = api.row(function(idx, data, node) { //secilen toplantiya gider
                            return data['SecilenToplanti'] == true;
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
                        { data: 'BaslangicTarihi' },
                        { data: 'BitisTarihi' },
                        { data: 'ToplantiKonusu'},
                        { data: 'Koordinator' },
                        { data: 'ToplantiYeri'},
                        { data: 'Katilimci' },
                        { data: 'DisKatilimcilar' },
                        { data: 'BilgiVerilenler' },
                        { data: 'Aciklama' },
                        { data: 'CevrimIci' },
                        { data: 'Duzenle' },

                    ],
                    'columnDefs': [
                        { type: 'turkish', targets: [2,3,4,5] },
                    ],
                    'order': [[1, 'asc']],//sort date desc
                    'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    },
                    responsive: true,
                    dom: 'frtip',
                    'createdRow': function(row, data, dataIndex) {
                        if (data.PasifToplanti)
                        {
                            $(row).addClass('pasif-toplanti');

                        }else{
                            var today = new Date();
                            today.setHours(0, 0, 0, 0);
                            var dateMomentObject = moment(data.BaslangicTarihi, 'DD.MM.YYYY HH:mm'); // 1st argument - string, 2nd argument - format
                            var toplantiGunu = dateMomentObject.toDate()

                            //var toplantiGunu = new Date(parseInt(data.BaslangicTarihi.substr(6)));
                            toplantiGunu.setHours(0, 0, 0, 0);

                            if(toplantiGunu.getTime() == today.getTime()){
                               $(row).addClass('bugunku-toplanti');
                            }
                        }

                    },//set row color
                });

            ";

            return tableString;
        }
        private string ToplantiYetkisineGoreDegerAta(int toplantiId, int toplantiYetkilisi)
        {
            bool allowed;
            if (!meetingOwnerPermissions.TryGetValue(toplantiYetkilisi, out allowed))
            {
                allowed = new PortalToplantiWriteService(LogContext).CanManageMeetingsOf(toplantiYetkilisi);
                meetingOwnerPermissions.Add(toplantiYetkilisi, allowed);
            }
            return allowed
                ? "<a href=" + ProjeConstants.PAGE_TOPLANTI_GIRIS + "?ToplantiId=" + toplantiId + " class='btn btn-outline-primary'>Düzenle</a>"
                : string.Empty;
        }
        private void RedirectToPage(string pageUrl)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
            Page.Response.Redirect(newUrl);

        }
        protected void YeniKayitBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "YeniKayitBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_TOPLANTI_GIRIS);
            });
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "CloseBtn_Click", () =>
            {
                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            });
        }
        protected void ToplantiTakvimiBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "ToplantiTakvimiBtn_Click", () =>
            {
                RedirectToPage(ProjeConstants.PAGE_TOPLANTI_TAKVIM);
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
        private string ParseKoordinator(int koordinator, int yetkili)
        {
            string koordinatorStr = string.Empty;
            BirimTanim birimTanim = new BirimTanimService().GetById(koordinator, LogContext);
            if (birimTanim != null)
            {
                koordinatorStr = birimTanim.KisaAdi;
            }
            Personel yetkiliPer = new Personel();
            yetkiliPer = new PersonelService().GetById(yetkili, LogContext);
            string yetkiliPerAdiSoyadi = yetkiliPer != null ? " </br> (" + yetkiliPer.Adi + " " + yetkiliPer.Soyadi + ")" : string.Empty;

            return koordinatorStr + yetkiliPerAdiSoyadi;
        }
        private class ToplantiListItem
        {
            public string ToplantiId { get; set; }
            public string ToplantiYeri { get; set; }
            public string ToplantiYeriDiger { get; set; }
            public string ToplantiKonusu { get; set; }
            public string BaslangicTarihi { get; set; }
            public string BitisTarihi { get; set; }
            public string Katilimci { get; set; }
            public string DisKatilimcilar { get; set; }
            public string BilgiVerilenler { get; set; }
            public string Koordinator { get; set; }
            public string Aciklama { get; set; }
            public string Duzenle { get; set; }
            public bool PasifToplanti { get; set; }
            public bool SecilenToplanti { get; set; }
            public string CevrimIci { get; set; }

        }

        protected void BaslangicTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "BaslangicTarihiTxt_TextChanged", () =>
            {
                TabloOlustur();
            });
        }
    }
}
