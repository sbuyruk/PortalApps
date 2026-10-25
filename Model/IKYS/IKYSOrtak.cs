using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Model.IKYS;
using Model.Services.IKYS;
using Model.Portal;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Ortak
{
    public class IKYSOrtak
    {
        public IKYSOrtak()
        {

        }
        #region IKYS
        //IKYS
        //Izin Hesaplari
        /// <summary>
        /// Kullanilan izin süresini hesaplarken Cumartesi-Pazar günülerini ve Resmi Tatilleri dikkate alir
        /// </summary>
        /// <param name="izinTipi"></param>
        /// <param name="bastar"></param>
        /// <param name="bittar"></param>
        /// <returns></returns>


        public static string IzinSuresiHesapla(int izinTipi, DateTime bastar, DateTime bittar, Utility.Logging.PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            return CalculateLeaveDuration(izinTipi, bastar, bittar, date => new ResmiTatilService().IsResmiTatil(date, context));
        }

        public static string IzinSuresiHesapla(int izinTipi, DateTime bastar, DateTime bittar,
            DAO.Ortak.SqlTransactionContext transaction, Utility.Logging.PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            return CalculateLeaveDuration(izinTipi, bastar, bittar, date => new ResmiTatilService().IsResmiTatil(date, transaction, context));
        }

        private static string CalculateLeaveDuration(int izinTipi, DateTime bastar, DateTime bittar, Func<DateTime, bool> isHoliday)
        {
            string sureStr = string.Empty;
            if ((izinTipi == ProjeConstants.IZINTIPI_BABALIK_INT) ||
                (izinTipi == ProjeConstants.IZINTIPI_EVLENME_INT) ||
                (izinTipi == ProjeConstants.IZINTIPI_OLUM_INT))
            {
                //Yeşim hanımın  talebi
                //babalık, evlenme ve ölüm izinlerinde Takvim günleri dikkate alinir. Cumartesi ve Pazar günleri izinden sayilir. Resmi tatiller izinden sayilir.02.07.2026
                //ResmiTatil resmiTatil = new ResmiTatil();
                DateTime date = bastar;
                int sure = 0;
                do
                {
                    //Cumartesi ve Pazar Günlerini izinden sayma
                    //if (!date.DayOfWeek.Equals(DayOfWeek.Sunday) && !date.DayOfWeek.Equals(DayOfWeek.Saturday))
                    //{
                    //    bool tatil = resmiTatil.ResmiTatilMi(date);
                    //    if (!tatil)
                    sure++;

                    //}
                    date = date.AddDays(1);
                } while (date <= bittar);
                sureStr = sure.ToString();
            }
            else if (izinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT)
            {
                ResmiTatil resmiTatil = new ResmiTatil();
                DateTime date = bastar;
                int sure = 0;
                do
                {
                    //Cumartesi ve Pazar Günlerini izinden sayma
                    if (!date.DayOfWeek.Equals(DayOfWeek.Sunday) && !date.DayOfWeek.Equals(DayOfWeek.Saturday))
                    {
                        bool tatil = isHoliday(date);
                        if (!tatil)
                            sure++;

                    }
                    date = date.AddDays(1);
                } while (date <= bittar);
                sureStr = sure.ToString();
            }
            else if (izinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
            {
                TimeSpan kullanilan = bittar.Subtract(bastar);
                TimeSpan ogleArasi = new TimeSpan(1, 0, 0);
                DateTime ogleArasiBasTar = new DateTime(bastar.Year, bastar.Month, bastar.Day, 12, 0, 0);
                DateTime ogleArasiBitTar = ogleArasiBasTar.AddHours(1);
                if ((bastar <= ogleArasiBasTar) && (bittar >= ogleArasiBitTar))//ögle arasini izinden sayma
                {
                    kullanilan = kullanilan.Subtract(ogleArasi);
                }
                sureStr = kullanilan.ToString();
            }
            else if ((izinTipi == ProjeConstants.IZINTIPI_UCRETSIZ_INT) ||
                   (izinTipi == ProjeConstants.IZINTIPI_DOGUM_INT) ||
                   (izinTipi == ProjeConstants.IZINTIPI_SUTIZNI_INT))
            {
                TimeSpan sureTs = bittar.AddDays(1).Subtract(bastar);//basladigi ve bittigi gün dahil
                sureStr = sureTs.Days.ToString();//tam gun sayisi
            }
            return sureStr;
        }


        // Uses the work snapshot already read by the caller's transaction; no database access.
        public static string IzinHakkiHesapla(Personel personel, int izinTipi, DateTime izinDonemiBasi,
            DateTime izinDonemBasTar, IsBilgileri isBilgileri)
        {
            if (izinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT)
                return UcretliIzinHakkiHesapla(personel, izinDonemiBasi, izinDonemBasTar, isBilgileri).ToString(CultureInfo.InvariantCulture);
            return izinTipi == ProjeConstants.IZINTIPI_MAZERET_INT
                ? ProjeConstants.IZIN_SURESI_MAZERET.ToString() : string.Empty;
        }

        public static int UcretliIzinHakkiHesapla(Personel personel, DateTime izinDonemiBasi,
            DateTime izinDonemBasTar, IsBilgileri isBilgileri)
        {
            if (personel == null) throw new ArgumentNullException("personel", "Personel bilgisi gereklidir.");
            int yilSayisi = izinDonemiBasi.Year - izinDonemBasTar.Year;
            if (yilSayisi < 1) return ProjeConstants.IZIN_SURESI_0;
            if (personel.Asker_sivil == ProjeConstants.PER_ASKER_INT) return ProjeConstants.IZIN_SURESI_ASKER;
            if (isBilgileri == null) throw new ArgumentNullException("isBilgileri", "İş bilgileri gereklidir.");
            if (izinDonemiBasi < izinDonemBasTar)
                throw new InvalidOperationException("İzin dönemi başlangıcı işe başlama tarihinden önce olamaz.");
            if (isBilgileri.VakifOncesiPrimGunSayisi >= 360)
                yilSayisi += isBilgileri.VakifOncesiPrimGunSayisi / 360;
            if (yilSayisi >= 15) return ProjeConstants.IZIN_SURESI_16PLUS;
            return yilSayisi > 5 ? ProjeConstants.IZIN_SURESI_6_15 : ProjeConstants.IZIN_SURESI_1_5;
        }

        private static string KalacakIzinHesapla(IzinDonem izinDonemi, string sure)
        {
            string sonuctaKalanIzin = string.Empty;
            if (izinDonemi != null)
            {
                if (izinDonemi.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT)
                {

                    int oncekiToplamIzin = izinDonemi.KullanilanIzin.ConvertToInt();
                    int sonuc = oncekiToplamIzin + sure.ConvertToInt();
                    int izinHakki = izinDonemi.IzinHakki.ConvertToInt();
                    int kalanIzin = izinHakki - sonuc;
                    sonuctaKalanIzin = kalanIzin.ToString() + " " + izinDonemi.Birim;
                }
                if (izinDonemi.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                {
                    TimeSpan oncekiToplamIzin = izinDonemi.KullanilanIzin.ConvertToTimeSpan();
                    TimeSpan sonuc = oncekiToplamIzin + sure.ConvertToTimeSpan();
                    TimeSpan kalanIzin = new TimeSpan(0, 0, 0); ;
                    TimeSpan izinHakki = izinDonemi.IzinHakki.ConvertToTimeSpan();
                    kalanIzin = izinHakki - sonuc;
                    sonuctaKalanIzin = kalanIzin.ConvertToTimeSpanReturnInHHmm();
                }
            }
            return sonuctaKalanIzin;
        }

        public static void IzinKabulRedOnayEPostasiGonder(int personelId, int izinTalepId, string kabulRedOnay, Utility.Logging.PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context", "Log bağlamı gereklidir.");
            Personel personel = new Personel();
            personel = new PersonelService().GetById(personelId, context);

            if (personel != null)
            {
                IzinTalep izinTalep = new IzinTalep();
                izinTalep = new IzinTalepService().GetById(izinTalepId, context);
                if (izinTalep == null || izinTalep.PersonelId != personelId)
                    throw new InvalidOperationException("Bildirim için izin talebi bulunamadı.");
                if (izinTalep != null)
                {
                    string userto = personel.KullaniciAdi + "@tskgv.local";
                    string from = "ikys@tskgv.local";
                    string bastarBittar = izinTalep.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + izinTalep.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                    //  string izintipi=izinTalep.IzinTipi?Pr
                    IzinTanim izintipi = new IzinTanim();
                    izintipi = new IzinTanimService().GetById(izinTalep.IzinTipi, context);
                    string izintipiStr = izintipi == null ? "" : izintipi.Adi;

                    string subject = bastarBittar + " tarihleri arasindaki " + izintipiStr + " Izin talebiniz.";

                    string url = "<a href = 'http://tskgv-portal/YonetimBirimleri/PersonelVeIdariIslerSubesi/Sayfalar/IzinTalepListesi.aspx'>Izin Talepleri </a>";
                    if (izinTalep.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                    {
                        url = "<a href = 'http://tskgv-portal/YonetimBirimleri/PersonelVeIdariIslerSubesi/Sayfalar/MazeretIzinTalepListesi.aspx'>Izin Talepleri </a>";
                    }

                    //string userbody = bastarBittar + " tarihleri arasinda talep ettiginiz " + izintipiStr + " izniniz kontrol edilmistir.<br>"
                    //    + " Lütfen " + userurl + " talep belgenizi bastirip imzalatarak Personel Kismina teslim ediniz.";
                    //string body = personel.Adi + " " + personel.Soyadi + " tarafindan " + bastarBittar + " tarihleri arasinda talep edilen " + izintipiStr
                    //    + " kontrol edilmistir.<br>Ayrintili bilgi için " + url + " sayfasina gidiniz";
                    string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                    string rawUrl = System.Web.HttpContext.Current.Request.RawUrl.ToString();
                    int index = currentUrl.IndexOf(rawUrl);
                    string rootUrl = (index < 0) ? currentUrl : currentUrl.Remove(index, rawUrl.Length);
                    IzinDonem izinDonemi = new IzinDonemService().GetById(izinTalep.IzinDonemId, context);
                    string sonIzin = KalacakIzinHesapla(izinDonemi, izinTalep.Sure);

                    var izinBelgesiUrl = "";

                    if (izinTalep.IzinTipi.Equals(ProjeConstants.IZINTIPI_UCRETLI_INT))
                    {
                        izinBelgesiUrl = string.Format("{0}?IzinTalepId={1}&IzinDonemId={2}&SonIzin={3}", rootUrl + ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, izinTalep.IzinDonemId, sonIzin);
                    }
                    else if (izinTalep.IzinTipi.Equals(ProjeConstants.IZINTIPI_MAZERET_INT))
                    {
                        IzinDonem id = new IzinDonem();
                        id = izinDonemi;
                        string kalanIzin = string.Empty;
                        if (id != null)
                        {
                            kalanIzin = id.KalanIzin.ConvertToTimeSpanReturnInHHmm();
                        }

                        string izinSuresi = izinTalep.Sure.ConvertToTimeSpanReturnInHHmm();

                        izinBelgesiUrl = string.Format("{0}?IzinTalepId={1}&SureStr={2}&KalanIzinStr={3}&SonIzin={4}", rootUrl + ProjeConstants.RAPOR_MAZERETIZINBELGESI_URL, izinTalepId, izinSuresi, kalanIzin, sonIzin);
                    }
                    else //diger Izinler
                    {
                        string izinSuresi = izinTalep.Sure.ConvertToTimeSpanReturnInHHmm();
                        izinBelgesiUrl = string.Format("{0}?IzinTalepId={1}&SureStr={2}&KalanIzinStr={3}&SonIzin={4}", rootUrl + ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, izinSuresi, "", "");
                    }


                    string userbody = bastarBittar + " tarihleri arasinda talep ettiginiz " + izintipiStr + " izniniz kontrol edilmistir.<br>Döküm almak için "
                        + " lütfen <a href ='" + izinBelgesiUrl + "'>IZIN BELGESI</a>ni bastirip imzalatarak Personel Kismina teslim ediniz.";
                    string body = personel.Adi + " " + personel.Soyadi + " tarafindan " + bastarBittar + " tarihleri arasinda talep edilen " + izintipiStr
                        + " kontrol edilmistir.<br>Ayrintili bilgi için " + url + " sayfasina gidiniz";

                    /////////////////////
                    if (kabulRedOnay.Equals("Red"))
                    {
                        userbody = bastarBittar + " tarihleri arasinda talep ettiginiz " + izintipiStr + " izniniz reddedilmistir.<br> Personel Kismi açiklamasi:" + izinTalep.Aciklama;
                        body = personel.Adi + " " + personel.Soyadi + " tarafindan " + bastarBittar + " tarihleri arasinda talep edilen " + izintipiStr + " reddedilmistir.<br>" +
                            " Ayrintili bilgi için " + url + " sayfasina gidiniz";
                    }
                    else if (kabulRedOnay.Equals("Onay"))
                    {
                        userbody = bastarBittar + " tarihleri arasinda talep ettiginiz " + izintipiStr + " izniniz kayitlara islenmistir.<br> Iyi izinler.";
                        body = personel.Adi + " " + personel.Soyadi + " tarafindan " + bastarBittar + " tarihleri arasinda talep edilen " + izintipiStr +
                            " kayitlara islenmistir.";
                    }
                    IletisimBilgileri ib = new IletisimBilgileri();
                    ib = new IletisimBilgileriService().GetByPersonelId(personel.Id, context);
                    if (ib != null)
                    {
                        userto = ib.IntranetEPosta;
                    }
                    string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL, context);
                    MailHelper.EPostaGonderVeyaHataVer(from, userto, subject, userbody, smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);

                    //string to = "izinonaymailgrubu@tskgv.local";
                    //subject = personel.Adi + " " + personel.Soyadi + " Yeni " + izintipiStr + " izin talebi ";

                    //MailHelper.ePostaGonder(from, to, subject, userbody);
                }
            }
            else
            {
                throw new InvalidOperationException("Bildirim için personel bilgisi bulunamadı.");
            }

        }


        private static StringBuilder EpostaTablosunuOlustur(GorevOnay gorevOnay)
        {
            System.Text.StringBuilder sb = new StringBuilder();
            sb.Append(@"
                        <style>
                            table {
                              font-family: arial;
                              border-collapse: collapse;
                              width: 40%;
                              font-size:smaller;
                            }
                            p {
                              font-family: arial;
                              font-size:smaller;
                            }
                            td, th {
                              border: 1px solid black;
                              text-align: left;
                              padding: 8px;
                            }
                        </style>
                        ");
            sb.Append("<p>");
            sb.Append("</p>");
            sb.Append("</br>");

            sb.Append("<table>");
            sb.Append("<tr>");
            sb.Append("<th colspan='2' style='text-align: center; background-color: #dddddd'>Görev Bilgileri (" + UtilityHelper.GetEnumDisplayName((GorevOnay.AmirOnayDurumu)gorevOnay.AmirOnayi) + ")</th>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Görev Kayit Numarasi</th>");
            sb.Append("<td>" + gorevOnay.Id + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Görevin Sebebi</th>");
            sb.Append("<td>" + gorevOnay.GorevinSebebi + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Baslangiç Zamani</th>");
            sb.Append("<td>" + gorevOnay.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Bitis Zamani</th>");
            sb.Append("<td>" + gorevOnay.BitisTarihi.ToString("dd.MM.yyyy HH:mm") + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Görevin Yeri</th>");
            sb.Append("<td>" + gorevOnay.GorevinYeri + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Ulasim Araci</th>");
            sb.Append("<td>" + gorevOnay.UlasimAraci + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Açiklama</th>");
            sb.Append("<td>" + gorevOnay.Aciklama + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Transfer</th>");
            sb.Append("<td>" + gorevOnay.Transfer + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Konaklama</th>");
            sb.Append("<td>" + gorevOnay.Konaklama + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Amir Onayi</th>");
            sb.Append("<td>" + ((GorevOnay.AmirOnayDurumu)gorevOnay.AmirOnayi) + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Onay/Red Açıklama</th>");
            sb.Append("<td>" + gorevOnay.OnayRedAciklama + "</td>");
            sb.Append("</tr>");

            sb.Append("</table><br/><br/>");
            sb.Append("<p>");
            sb.Append("Insan Kaynaklari Yönetim Sistemi</br>" + DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
            sb.Append("</p>");
            sb.Append("<p style='color:gray; font-family: arial;font-size:xx-small;'>IKYS &trade; Bilgi Sistemleri Kismi </p> ");
            return sb;
        }

        private static StringBuilder PersonelBilgileriTablosunuOlustur(PersonelItem personelItem, string baslik)
        {
            System.Text.StringBuilder sb = new StringBuilder();
            sb.Append(@"
                        <style>
                            table {
                              font-family: arial;
                              border-collapse: collapse;
                              width: 40%;
                              font-size:smaller;
                            }
                            p {
                              font-family: arial;
                              font-size:smaller;
                            }
                            td, th {
                              border: 1px solid black;
                              text-align: left;
                              padding: 8px;
                            }
                        </style>
                        ");
            sb.Append(baslik);
            sb.Append("<p>");
            sb.Append("</p>");

            sb.Append("<table>");
            sb.Append("<tr>");
            sb.Append("<th colspan='2' style='text-align: center; background-color: #dddddd'>Personel Bilgileri</th>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Adi Soyadi</th>");
            sb.Append("<td>" + personelItem.PersonelBilgileri.Adi + " " + personelItem.PersonelBilgileri.Soyadi + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Kullanici Adi</th>");
            sb.Append("<td>" + personelItem.PersonelBilgileri.KullaniciAdi + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>E-Posta</th>");
            sb.Append("<td>" + personelItem.IletisimBilgileri.InternetEPosta + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Telefon</th>");
            sb.Append("<td>" + personelItem.IletisimBilgileri.CepTelefonu + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Birimi</th>");
            sb.Append("<td>" + personelItem.IsBilgileriItem.Birim?.Adi + "</td>");
            sb.Append("</tr>");
            sb.Append("<th>Ünvani</th>");
            sb.Append("<td>" + personelItem.IsBilgileriItem.Unvan?.Adi + "</td>");
            sb.Append("</tr>");


            sb.Append("</table><br/><br/>");
            sb.Append("<p>");
            sb.Append("IKYS.</br>" + DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
            sb.Append("</p>");
            sb.Append("<p style='color:gray; font-family: arial;font-size:xx-small;'>TYS &trade; Bilgi Sistem Kismi </p> ");
            return sb;
        }


        public static Personel PersonelGetir(string currentUserName)
        {
            Personel personel = new Personel();
            {
                string userName = currentUserName.Substring(currentUserName.LastIndexOf("\\") + 1, currentUserName.Length - currentUserName.LastIndexOf("\\") - 1);
                personel = new PersonelService().GetByUserName(userName);
            }
            return personel;
        }

        public static string PersonelinBolgesiniGetir_Deprecated(string currentUserName)
        {
            Personel personel = PersonelGetir(currentUserName);

            if (personel != null)
            {
                PersonelItem personelItem = new PersonelItem();
                personelItem.PersonelId = personel.Id;
                if (personelItem.IsBilgileriItem.Birim != null)
                {
                    string birimAdi = personelItem.IsBilgileriItem.Birim.Adi;
                    string bolge = birimAdi.Contains(ProjeConstants.BOLGE_ISTANBUL) ? ProjeConstants.BOLGE_ISTANBUL :
                        (birimAdi.Contains(ProjeConstants.BOLGE_IZMIR) ? ProjeConstants.BOLGE_IZMIR :
                        (birimAdi.Contains(ProjeConstants.BOLGE_MERSIN) ? ProjeConstants.BOLGE_MERSIN : string.Empty));
                    return bolge;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                MessageHelper.PublishMessage("Personel bulunamadi", ProjeConstants.MESAJ_HATA);
                return string.Empty;
            }
        }
        public static Bolge BolgeGetirByUserName(string currentUserName)
        {
            Personel personel = PersonelGetir(currentUserName);

            if (personel != null)
            {
                PersonelItem personelItem = new PersonelItem();
                personelItem.PersonelId = personel.Id;
                if (personelItem.IsBilgileriItem.Bolge != null)
                {
                    Bolge bolge = personelItem.IsBilgileriItem.Bolge;
                    return bolge;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                MessageHelper.PublishMessage("Personel bulunamadi", ProjeConstants.MESAJ_HATA);
                return null;
            }
        }

        public static Bolge BolgeGetirByUserName(string currentUserName, Utility.Logging.PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context", "Log bağlamı gereklidir.");
            Personel personel = PersonelGetir(currentUserName, context);
            if (personel == null)
            {
                Messages.Error("Personel bulunamadı.");
                return null;
            }
            IsBilgileri isBilgileri = new IsBilgileriService().GetByPersonelId(personel.Id, context);
            if (isBilgileri == null) return null;
            BirimTanim birim = new BirimTanimService().GetById(isBilgileri.BirimId, context);
            return birim == null ? null : new Model.Services.Ortak.BolgeService().GetById(birim.BolgeId, context);
        }

        #endregion IKYS

        public static bool IzinTalepOlusturmaEPostasiGonder(Personel personel, int izinTalepId, Utility.Logging.PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context", "Log bağlamı gereklidir.");
            try
            {
                IzinTalep izinTalep = new IzinTalep();
                izinTalep = new IzinTalepService().GetById(izinTalepId, context);
                if (izinTalep != null)
                {
                    //EPostayi hazirla
                    string to = personel.KullaniciAdi + "@tskgv.local";
                    string from = "ikys@tskgv.local";
                    string bastarBittar = izinTalep.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + izinTalep.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                    //  string izintipi=izinTalep.IzinTipi?Pr
                    IzinTanim izintipi = new IzinTanim();
                    izintipi = new IzinTanimService().GetById(izinTalep.IzinTipi, context);
                    string izintipiStr = izintipi == null ? "" : izintipi.Adi;

                    string subject = bastarBittar + " tarihleri arasindaki " + izintipiStr + " Izin talebiniz. ";
                    string userurl = "<a href = 'http://tskgv-portal/KullaniciUygulamalari/Sayfalar/KisiselSayfa.aspx'>kisisel sayfanizdan </a>";

                    string body = izintipiStr + " Izin talebiniz olusturulmustur. <br>Lütfen " + userurl + " talebinizin durumunu takip ediniz.";

                    IletisimBilgileri ib = new IletisimBilgileri();
                    ib = new IletisimBilgileriService().GetByPersonelId(personel.Id, context);
                    if (ib != null)
                    {
                        to = ib.IntranetEPosta;
                    }
                    //Izin sahibine eposta gönder
                    string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL, context);
                    MailHelper.EPostaGonderVeyaHataVer(from, to, subject, body, smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);

                    //IKYS Onay grubuna eposta gönder
                    to = "izinonaymailgrubu@tskgv.local";
                    subject = personel.Adi + " " + personel.Soyadi + " Yeni " + izintipiStr + " izin talebi ";
                    string url = "<a href = 'http://tskgv-portal/YonetimBirimleri/PersonelVeIdariIslerSubesi/Sayfalar/IzinTalepListesi.aspx'>Izin Talepleri </a>";
                    if (izinTalep.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                    {
                        url = "<a href = 'http://tskgv-portal/YonetimBirimleri/PersonelVeIdariIslerSubesi/Sayfalar/MazeretIzinTalepListesi.aspx'>Izin Talepleri </a>";
                    }
                    body = personel.Adi + " " + personel.Soyadi + " " + bastarBittar + " tarihleri arasinda " + izintipiStr + " izin talebinde bulunmustur. <br>Lütfen kontrol-red islemleri için " + url + " sayfasina gidiniz";
                    MailHelper.EPostaGonderVeyaHataVer(from, to, subject, body, smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);
                }


                return true;
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                Utility.Logging.PortalTechnicalLog.WriteError(context, "IKYS.Notification", error);
                return false;
            }
        }

        public static bool GorevOnayEPostasiGonder(Personel personel, int gorevOnayId, string tip, Utility.Logging.PortalLogContext context)
        {
            return GorevOnayEPostasiGonder(personel, gorevOnayId, tip, null, "Giriş", true, context);
        }

        public static bool GorevOnayEPostasiGonder(Personel personel, int gorevOnayId, string tip, List<string> ekEpostaAdresleri, string durum, bool isAddMailGrubuToAdress, Utility.Logging.PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context", "Log bağlamı gereklidir.");
            try
            {
                string from = "ikys@tskgv.local";
                string personelEposta = new IletisimBilgileriService().GetByPersonelId(personel.Id, context)?.InternetEPosta;
                List<string> personelEpostaAdresleri = new List<string>();
                List<string> amirEpostaAdresleri = new List<string>();
                if (ekEpostaAdresleri != null)
                {
                    foreach (string eposta in ekEpostaAdresleri.Where(a => !string.IsNullOrWhiteSpace(a)).Distinct(StringComparer.OrdinalIgnoreCase))
                    {
                        if (!string.IsNullOrWhiteSpace(personelEposta) && eposta.Equals(personelEposta, StringComparison.OrdinalIgnoreCase))
                            personelEpostaAdresleri.Add(eposta);
                        else
                            amirEpostaAdresleri.Add(eposta);
                    }
                }
                string subject = string.Empty;
                string rootUrl = UtilityHelper.RootURLGetir();
                string personelSayfa = isAddMailGrubuToAdress ? "GorevOnayListesiKisisel.aspx" : ProjeConstants.PAGE_GOREVONAY_LIST;
                string amirSayfa = isAddMailGrubuToAdress ? ProjeConstants.PAGE_BEKLEYENISLEMLER : ProjeConstants.PAGE_GOREVONAY_LIST;
                string durumStr= durum.Equals("Onay") ? "onaylanmistir." : durum.Equals("Red") ? "reddedilmistir." : "girilmiştir.";
                if (tip.Equals("SehirIci"))
                {
                    Yoklama yoklama = new Yoklama();
                    yoklama = new YoklamaService().GetById(gorevOnayId, context);
                    if (yoklama != null)
                    {
                        string bastarBittar = yoklama.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + yoklama.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                        subject = personel.Adi + " " + personel.Soyadi + " için " + bastarBittar + " tarihleri arasinda Sehir Içi Görev " + durumStr;
                        string personelUrl = string.Format("<a href = '{0}/KullaniciUygulamalari/Sayfalar/{1}'>Görev onay listesi </a>", rootUrl, personelSayfa);
                        string amirUrl = string.Format("<a href = '{0}/KullaniciUygulamalari/Sayfalar/{1}'>Görev onay işlemleri </a>", rootUrl, amirSayfa);
                        string personelBody = personel.Adi + " " + personel.Soyadi + " için " + bastarBittar + " tarihleri arasinda Sehir Içi Görev " + durumStr + " <br>Görev durumunu " + personelUrl + " üzerinden takip edebilirsiniz.";
                        string amirBody = personel.Adi + " " + personel.Soyadi + " için " + bastarBittar + " tarihleri arasinda Sehir Içi Görev " + durumStr + " <br>Yoklama islemlerini " + amirUrl + " üzerinden yapabilirsiniz.";

                        string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL, context);
                        string amirTo = (isAddMailGrubuToAdress ? "gorevonaymailgrubu@tskgv.org.tr;" : string.Empty) + string.Join(";", amirEpostaAdresleri);
                        if (personelEpostaAdresleri.Count > 0)
                            MailHelper.EPostaGonderVeyaHataVer(from, string.Join(";", personelEpostaAdresleri), subject, personelBody, smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);
                        if (!string.IsNullOrEmpty(amirTo))
                            MailHelper.EPostaGonderVeyaHataVer(from, amirTo, subject, amirBody, smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);

                    }
                }
                else
                {
                    GorevOnay gorevOnay = new GorevOnay();
                    gorevOnay = new GorevOnayService().GetById(gorevOnayId, context);
                    if (gorevOnay != null)
                    {
                        string bastarBittar = gorevOnay.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + gorevOnay.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                        subject = personel.Adi + " " + personel.Soyadi + " için " + bastarBittar + " tarihleri arasinda Yurt Içi/Yurt Disi Görev " + durumStr;
                        string personelUrl = string.Format("<a href = '{0}/KullaniciUygulamalari/Sayfalar/{1}'>Görev onay listesi </a>", rootUrl, personelSayfa);
                        string amirUrl = string.Format("<a href = '{0}/KullaniciUygulamalari/Sayfalar/{1}'>Görev onay işlemleri </a>", rootUrl, amirSayfa);
                        string personelBody = personel.Adi + " " + personel.Soyadi + " için " + bastarBittar + " tarihleri arasinda Yurt Içi/Yurt Disi Görev " + durumStr + " <br>Görev durumunu " + personelUrl + " üzerinden takip edebilirsiniz.";
                        string amirBody = personel.Adi + " " + personel.Soyadi + " için " + bastarBittar + " tarihleri arasinda Yurt Içi/Yurt Disi Görev " + durumStr + " <br>Görev onay islemlerini " + amirUrl + " üzerinden yapabilirsiniz.";
                        StringBuilder tabloSB = EpostaTablosunuOlustur(gorevOnay);
                        personelBody += "</br>" + tabloSB.ToString();
                        amirBody += "</br>" + tabloSB.ToString();
                        string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL, context);
                        string amirTo = (isAddMailGrubuToAdress ? "gorevonaymailgrubu@tskgv.org.tr;" : string.Empty) + string.Join(";", amirEpostaAdresleri);
                        if (personelEpostaAdresleri.Count > 0)
                            MailHelper.EPostaGonderVeyaHataVer(from, string.Join(";", personelEpostaAdresleri), subject, personelBody, smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);
                        if (!string.IsNullOrEmpty(amirTo))
                            MailHelper.EPostaGonderVeyaHataVer(from, amirTo, subject, amirBody, smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);
                    }
                }

                return true;
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                Utility.Logging.PortalTechnicalLog.WriteError(context, "IKYS.Notification", error);
                return false;
            }
        }

        public static bool BilgiSistemMailGrubunaEPostaGonder(PersonelItem personelItem, string baslik, Utility.Logging.PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context", "Log bağlamı gereklidir.");
            try
            {
                personelItem.LoggingContext = context;
                string userto = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_BILGISISTEM_MAILGRUBU, context);

                string from = "IKYS <ikys@tskgv.local>";
                string subject = baslik;


                string grupAdi = userto.Split('@')[0];
                bool grupMu = UtilityHelper.IsGroup("TSKGV", grupAdi);//böyle bir grup var mi
                if (grupMu)// varsa
                {
                    List<UserPrincipal> uplist = UtilityHelper.GetGroupMembers("TSKGV", grupAdi);
                    var emaillist = uplist.Select(item => item.EmailAddress).ToList();
                    string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL, context);
                    foreach (var eposta in emaillist)
                    {

                        StringBuilder tabloSB = PersonelBilgileriTablosunuOlustur(personelItem, baslik);

                        MailHelper.EPostaGonderVeyaHataVer(from, eposta, subject, "</br>" + tabloSB.ToString(), smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);

                    }
                }

                return true;
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                Utility.Logging.PortalTechnicalLog.WriteError(context, "IKYS.Notification", error);
                return false;
            }
        }

        private static string UstBirimGetir(int parentId, Utility.Logging.PortalLogContext context)
        {
            string retVal = parentId + ",";
            List<BirimTanim> list = new BirimTanimService().GetByParentId(parentId, context);
            foreach (BirimTanim item in list)
            {
                //retVal += item.Id + ",";
                string val = UstBirimGetir(item.Id, context);
                if (string.IsNullOrEmpty(val))
                {
                    return retVal;
                }
                retVal += val;
            }
            return retVal;
        }

        public static string BirimListesiGetir(Personel personel, Utility.Logging.PortalLogContext context)
        {
            string birimIdStr = string.Empty;

            if (personel != null)
            {
                IsBilgileri ib = new IsBilgileri();
                ib = new IsBilgileriService().GetByPersonelId(personel.Id, context);
                if (ib != null)
                {
                    int birimId = ib.BirimId;
                    BirimTanim bt = new BirimTanimService().GetById(birimId, context);
                    if ((bt != null) && (bt.AmirId == personel.Id))
                    {
                        string birim = UstBirimGetir(birimId, context);
                        birimIdStr = string.IsNullOrEmpty(birim) ? "" : birim.Substring(0, birim.Length - 1);
                    }

                }
            }
            return birimIdStr;
        }

        public static Personel PersonelGetir(string currentUserName, Utility.Logging.PortalLogContext context)
        {
            Personel personel = new Personel();
            {
                string userName = currentUserName.Substring(currentUserName.LastIndexOf("\\") + 1, currentUserName.Length - currentUserName.LastIndexOf("\\") - 1);
                personel = new PersonelService().GetByUserName(userName, context);
            }
            return personel;
        }

        public static int KalanIzinToplamiGetir(int personelId, bool sadeceEskiDonemler, Utility.Logging.PortalLogContext context)
        {

            IzinDonem izinDonemDao = new IzinDonem();
            int kalanIzinToplami = 0;

            DataTable dataTable = new IzinDonemService().GetSumKalanIzinByPersonelId(personelId, sadeceEskiDonemler, context);
            if (dataTable != null)
            {
                DataRow dataRow = dataTable.Rows[0];

                kalanIzinToplami = dataRow["KalanIzinToplami"].ConvertToInt();// - izinDonemi.KalanIzin.ConvertToInt();
            }

            return kalanIzinToplami;
        }
        public static bool GorevOnayEPostasiGonder(Personel personel, int gorevOnayId, string tip, List<string> ekEpostaAdresleri, string durum, Utility.Logging.PortalLogContext context)
        {
            return GorevOnayEPostasiGonder(personel, gorevOnayId, tip, ekEpostaAdresleri, durum, true, context);
        }
    }
}
