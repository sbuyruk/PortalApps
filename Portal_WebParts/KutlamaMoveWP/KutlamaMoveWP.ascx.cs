using Portal_WebParts.Logging;
using Utility.Logging;
using Model.IKYS;
using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.KutlamaMoveWP
{
    [ToolboxItemAttribute(false)]
    public partial class KutlamaMoveWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("Portal", "KutlamaMoveWP")); } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KutlamaMoveWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        public List<Kutlama> listDogum;
        public List<Kutlama> listEvlilik;
        public string GenelMudur { get; set; }
        public string GenelMudurUnvani { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) PortalWebPartLog.RecordOpened(LogContext);
                DogumGunuDiv.Attributes["style"] = "display:none";
                EvlilikDiv.Attributes["style"] = "display:none";
                GenelMudurDiv.Attributes["style"] = "display:none";
                KutlamayiOkudumDiv.Attributes["style"] = "display:none";
                YokDiv.Attributes["style"] = "display:none";
                hdnBirthday.Value = hdnMarriage.Value = "0";
                KullaniciKutlamalariniGetir();
                if (listDogum.Count == 0 && listEvlilik.Count == 0) YokDiv.Attributes["style"] = "display:block";
            });
        }

        bool GMKutlanacakMi = false;
        Personel genelMudur = new Personel();
        private void KullaniciKutlamalariniGetir()
        {
            bool isPopUp = false;
            GenelMudurUnvani = ProjeConstants.GOREV_GENELMUDUR;

            GorevTanim gt = new GorevTanim();
            gt = new GorevTanimService().GetByGorevId(ProjeConstants.GOREV_GENELMUDUR_INT, LogContext);
            if (gt != null)
            {
                genelMudur = new PersonelService().GetById(gt.PersonelId, LogContext);
                if (genelMudur != null)
                {
                    GenelMudur = genelMudur.Adi + " " + genelMudur.Soyadi;
                    GenelMudurUnvani = gt.Adi;
                    //GenelMudurUnvani = gt.Vekil ? gt.Adi + " Vekili " : gt.Adi;
                }
            }

            listDogum = new List<Kutlama>();
            listEvlilik = new List<Kutlama>();

            ListeyiDoldur();

            hdnBirthday.Value = listDogum.Count.ToString();
            hdnMarriage.Value = listEvlilik.Count.ToString();

            if (listDogum.Count > 0)
            {
                isPopUp = true;
                GenelMudurDiv.Attributes["style"] = "display : block";
                KutlamayiOkudumDiv.Attributes["style"] = "display : block";
                YokDiv.Attributes["style"] = "display : none";
                pGenelMudur.Text = System.Web.HttpUtility.HtmlEncode(GenelMudur);
                pGenelMudurUnvan.Text = System.Web.HttpUtility.HtmlEncode(GenelMudurUnvani);

                DogumGunuDiv.Attributes["style"] = "display : block";
                DogumRepeater.DataSource = listDogum;
                DogumRepeater.DataBind();
            }

            if (listEvlilik.Count > 0)
            {
                isPopUp = true;
                GenelMudurDiv.Attributes["style"] = "display : block";
                KutlamayiOkudumDiv.Attributes["style"] = "display : block";
                YokDiv.Attributes["style"] = "display : none";
                pGenelMudur.Text = System.Web.HttpUtility.HtmlEncode(GenelMudur);
                pGenelMudurUnvan.Text = System.Web.HttpUtility.HtmlEncode(GenelMudurUnvani);

                EvlilikDiv.Attributes["style"] = "display : block";
                EvlilikRepeater.DataSource = listEvlilik;
                EvlilikRepeater.DataBind();
            }
            if (isPopUp && !Page.IsPostBack)
            {
                var openPopup = "OpenKutlamaPopup(false,'onLoad');";
                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
            }

        }

        private void ListeyiDoldur()
        {
            Personel personelDao = new Personel();

            List<Personel> dogumGunuKutlanacakPersonel = new List<Personel>();
            List<Personel> evlilikYildonumuKutlanacakPersonel = new List<Personel>();
            DateTime tarih = DateTime.Today;
            //ertesi gün tatil ise bugunden kutlamayi gostersin
            do
            {

                List<Personel> dogumGunuKutlanacaklar = new PersonelService().GetByBirthday(tarih.Day, tarih.Month, LogContext);
                dogumGunuKutlanacakPersonel.AddRange(dogumGunuKutlanacaklar);

                List<Personel> evlilikYildonumuGunuKutlanacaklar = new PersonelService().GetByMarriageDate(tarih.Day, tarih.Month, LogContext);
                evlilikYildonumuKutlanacakPersonel.AddRange(evlilikYildonumuGunuKutlanacaklar);

                tarih =tarih.AddDays(1);
            } while (TatilMi(tarih));

            GMKutlanacakMiKontrolu(dogumGunuKutlanacakPersonel, evlilikYildonumuKutlanacakPersonel);
            ListeleriEkle(dogumGunuKutlanacakPersonel, evlilikYildonumuKutlanacakPersonel);

        }

        private bool TatilMi(DateTime tarih)
        {
            if ((tarih.DayOfWeek == DayOfWeek.Saturday) ||
                    (tarih.DayOfWeek == DayOfWeek.Sunday))
            {
                return true;
            }
            else
            {
                ResmiTatil resmiTatil = new ResmiTatil();
                return new ResmiTatilService().IsResmiTatil(tarih, LogContext);
            }

        }
        private void ListeleriEkle(List<Personel> dogumGunuKutlanacakPersonel, List<Personel> evlilikYildonumuKutlanacakPersonel)
        {
            string msgParameter_dogum = "Dogum";
            string msgParameter_evlilik = "Evlilik";

            ListeyeEkle(dogumGunuKutlanacakPersonel, msgParameter_dogum);
            ListeyeEkle(evlilikYildonumuKutlanacakPersonel, msgParameter_evlilik);
        }

        private void GMKutlanacakMiKontrolu(List<Personel> dogumGunuKutlanacaklarListesi,List<Personel> evlilikYildonumuKutlanacaklarListesi)
        {
            GMKutlanacakMi = GMKutlanacakMi == true ? GMKutlanacakMi : (dogumGunuKutlanacaklarListesi.Find(x => x.Id == (genelMudur == null ? 0 : genelMudur.Id)) == null) ? false : true;
            GMKutlanacakMi = GMKutlanacakMi == true ? GMKutlanacakMi : (evlilikYildonumuKutlanacaklarListesi.Find(x => x.Id == (genelMudur == null ? 0 : genelMudur.Id)) == null) ? false : true;
        }

        private void ListeyeEkle(List<Personel> list, string msgParameter)
        {
            foreach (Personel personel in list)
            {
                string personelAdi = personel.Adi + " " + personel.Soyadi;
                Kutlama kutlama = new Kutlama();
                kutlama.Adi = string.Format("Sayın {0};", personel.Adi + " " + personel.Soyadi);
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string hostUrl = currentUrl.Substring(0, currentUrl.LastIndexOf(System.Web.HttpContext.Current.Request.Url.AbsolutePath));

                string fotostr = string.IsNullOrEmpty(personel.KullaniciAdi) ? (string.IsNullOrEmpty(personel.Adi) ? string.Empty : personel.Adi.Substring(0, 1)) + personel.Soyadi.ReturnEmptyIfNull().ToString() : personel.KullaniciAdi.ReturnEmptyIfNull().ToString();
                string imgUrl = hostUrl + ProjeConstants.PATH_RESIMLER_PERSONEL + fotostr.ReplaceTrChars() + ".jpg";

                kutlama.ResimPath = imgUrl;

                if (msgParameter.Equals("Dogum"))
                {

                    Kimlik kimlik = new Kimlik();
                    kimlik = new KimlikService().GetByPersonelId(personel.Id, LogContext);
                    string tarih = string.Empty;
                    if (kimlik != null)
                    {
                        DateTime bugun = DateTime.Today;
                        double gunfarki = (Occurrence(kimlik.DogumTar, bugun) - bugun).TotalDays;
                        DateTime dogumTar = bugun.AddDays(gunfarki);
                        kutlama.KutlamaTarihi = dogumTar.ConvertToDatetimeEmptyIfNull();
                        tarih = dogumTar == bugun ? " Doğum" : dogumTar.ConvertToDatetimeEmptyIfNull() + " tarihindeki doğum";
                    }
                    if (GMKutlanacakMi)
                    {
                        kutlama.Metin = string.Format("{0} gününüzü kutlar, mutlu yıllar dileriz.", tarih);
                        GenelMudur = "TSKGV Çalışanları";
                        GenelMudurUnvani = string.Empty;
                    }
                    else
                        kutlama.Metin = string.Format("{0} gününüzü kutlar, mutlu yıllar dilerim.", tarih);
                    listDogum.Add(kutlama);
                }
                else
                {
                    Kimlik kimlik = new Kimlik();
                    kimlik = new KimlikService().GetByPersonelId(personel.Id, LogContext);
                    string tarih = string.Empty;
                    if (kimlik != null)
                    {
                        DateTime bugun = DateTime.Today;
                        double gunfarki = (Occurrence(kimlik.EvlilikTar, bugun) - bugun).TotalDays;
                        DateTime evlTar = bugun.AddDays(gunfarki);
                        tarih = evlTar == bugun ? " Evlilik" : evlTar.ConvertToDatetimeEmptyIfNull() + " tarihindeki evlilik";
                    }
                    if (GMKutlanacakMi)
                    {
                        kutlama.Metin = string.Format("{0} yıldönümünüzü kutlar, ömür boyu mutluluklar dileriz.", tarih);
                        GenelMudur = "TSKGV Çalışanları";
                        GenelMudurUnvani = string.Empty;
                    }
                    else
                        kutlama.Metin = string.Format("{0} yıldönümünüzü kutlar, ömür boyu mutluluklar dilerim.", tarih);
                    listEvlilik.Add(kutlama);
                }

            }
        }
        private static DateTime Occurrence(DateTime anniversary, DateTime today)
        {
            int year = today.Year;
            while (true)
            {
                if (anniversary.Day <= DateTime.DaysInMonth(year, anniversary.Month))
                {
                    DateTime date = new DateTime(year, anniversary.Month, anniversary.Day);
                    if (date >= today.Date) return date;
                }
                year++;
            }
        }
        public class Kutlama
        {
            public string Adi { get; set; }
            public string Metin { get; set; }
            public string ResimPath { get; set; }
            public string KutlamaTarihi { get; set; }
        }
    }
}
