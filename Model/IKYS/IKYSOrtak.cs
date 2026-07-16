using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Model.IKYS;
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
        public static string IzinSuresiHesapla(int izinTipi, DateTime bastar, DateTime bittar)
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
                        bool tatil = resmiTatil.ResmiTatilMi(date);
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


        public static string IzinHakkiHesapla(Personel personel, int izinTipi, DateTime izinDonemiBasi, DateTime izinDonemBasTar)
        {
            string izinHakiiSaatVeyaGun = string.Empty;
            if (izinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT)
            {
                izinHakiiSaatVeyaGun = UcretliIzinHakkiHesapla(personel, izinDonemiBasi, izinDonemBasTar).ToString();
            }
            else if (izinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
            {
                izinHakiiSaatVeyaGun = ProjeConstants.IZIN_SURESI_MAZERET.ToString();
            }
            return izinHakiiSaatVeyaGun;
        }
        public static int UcretliIzinHakkiHesapla(Personel personel, DateTime izinDonemiBasi, DateTime izinDonemBasTar)
        {
            int hakEdilenIzinGunSayisi = ProjeConstants.IZIN_SURESI_1_5;
            int ilkcalistigiYilSayisi = izinDonemiBasi.Year - izinDonemBasTar.Year;

            //ilk yil izin hakki 0 gün
            if (ilkcalistigiYilSayisi < 1)
            {
                hakEdilenIzinGunSayisi = ProjeConstants.IZIN_SURESI_0;
            }
            else 
            {
                if (personel.Asker_sivil == ProjeConstants.PER_ASKER_INT)
                {
                    hakEdilenIzinGunSayisi = ProjeConstants.IZIN_SURESI_ASKER;
                }
                else
                {
                    try
                    {
                        //izinDonemiBasi'ndan ise baslamatar cikararak calistigi yil suresini bul
                        IsBilgileri ib = new IsBilgileri();
                        ib = ib.SelectByPersonelId(personel.Id);
                        if (ib != null)
                        {

                            //DateTime iseBaslamaTar = ib.IzinDonemiBasTar!=null? ib.IzinDonemiBasTar:ib.BaslamaTar;

                            if (izinDonemiBasi < izinDonemBasTar)
                            {
                                Exception ex = new Exception("Izin Dönemi Basi Ise baslama tarihinden küçük olamaz");
                                throw (ex);

                            }
                            int calistigiYilSayisi = izinDonemiBasi.Year - izinDonemBasTar.Year;
                            //int ilkcalistigiYilSayisi = izinDonemiBasi.Year - izinDonemBasTar.Year;
                            int oncekiPrimgunSayisi = 0;
                            oncekiPrimgunSayisi = ib.VakifOncesiPrimGunSayisi;
                            if (oncekiPrimgunSayisi >= 360)
                            {
                                int ekGun = oncekiPrimgunSayisi / 360;
                                calistigiYilSayisi += ekGun;
                            }
                            /*
                                (1)  1 – 5 yil olanlara (5 yil dâhil) 14 is günü, 
                                (2)  6 – 15 yil olanlara 20 is günü, 
                                (3)  15 yil ve daha fazla olanlara 26 is günü, yillik ücretli izin verilir. 
                            */

                            if ((calistigiYilSayisi >= 1) && (calistigiYilSayisi <= 5))
                            {
                                hakEdilenIzinGunSayisi = ProjeConstants.IZIN_SURESI_1_5;
                            }
                            else if ((calistigiYilSayisi > 5) && (calistigiYilSayisi < 15))
                            {
                                hakEdilenIzinGunSayisi = ProjeConstants.IZIN_SURESI_6_15;
                            }
                            else if (calistigiYilSayisi >= 15)
                            {
                                hakEdilenIzinGunSayisi = ProjeConstants.IZIN_SURESI_16PLUS;
                            }

                        }

                    }
                    catch (Exception exception)
                    {
                        ExceptionHelper exHelper = new ExceptionHelper(exception);
                        exHelper.PublishException();
                    }

                } 
            }

            return hakEdilenIzinGunSayisi;

        }
        private static string KalacakIzinHesapla(int izinDonemId, string sure)
        {
            string sonuctaKalanIzin = string.Empty;
            IzinDonem izinDonemi = new IzinDonem();
            izinDonemi = izinDonemi.Select<IzinDonem>(izinDonemId);
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
        public static void IzinKabulRedOnayEPostasiGonder(int personelId, int izinTalepId, string kabulRedOnay)
        {
            Personel personel = new Personel();
            personel = personel.Select<Personel>(personelId);

            if (personel != null)
            {
                IzinTalep izinTalep = new IzinTalep();
                izinTalep = izinTalep.Select<IzinTalep>(izinTalepId);
                if (izinTalep != null)
                {
                    string userto = personel.KullaniciAdi + "@tskgv.local";
                    string from = "ikys@tskgv.local";
                    string bastarBittar = izinTalep.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + izinTalep.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                    //  string izintipi=izinTalep.IzinTipi?Pr
                    IzinTanim izintipi = new IzinTanim();
                    izintipi = izintipi.Select<IzinTanim>(izinTalep.IzinTipi);
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
                    string sonIzin = KalacakIzinHesapla(izinTalep.IzinDonemId, izinTalep.Sure);

                    var izinBelgesiUrl = "";

                    if (izinTalep.IzinTipi.Equals(ProjeConstants.IZINTIPI_UCRETLI_INT))
                    {
                        izinBelgesiUrl = string.Format("{0}?IzinTalepId={1}&IzinDonemId={2}&SonIzin={3}", rootUrl + ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, izinTalep.IzinDonemId, sonIzin);
                    }
                    else if (izinTalep.IzinTipi.Equals(ProjeConstants.IZINTIPI_MAZERET_INT))
                    {
                        IzinDonem id = new IzinDonem();
                        id = id.Select<IzinDonem>(izinTalep.IzinDonemId);
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
                    ib = ib.SelectByPersonelId(personel.Id);
                    if (ib != null)
                    {
                        userto = ib.IntranetEPosta;
                    }
                    string smtpAdresi =UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL);
                    MailHelper.EPostaGonder(from, userto, subject, userbody, smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);

                    //string to = "izinonaymailgrubu@tskgv.local";
                    //subject = personel.Adi + " " + personel.Soyadi + " Yeni " + izintipiStr + " izin talebi ";

                    //MailHelper.ePostaGonder(from, to, subject, userbody);
                }
            }
            else
            {
                MessageHelper.PublishMessage("Personel ve/veya Izintalep bilgisi bulunamadi", ProjeConstants.MESAJ_HATA);
            }

        }
        public static void IzinTalepOlusturmaEPostasiGonder(Personel personel, int izinTalepId)
        {
            IzinTalep izinTalep = new IzinTalep();
            izinTalep = izinTalep.Select<IzinTalep>(izinTalepId);
            if (izinTalep != null)
            {
                //EPostayi hazirla
                string to = personel.KullaniciAdi + "@tskgv.local";
                string from = "ikys@tskgv.local";
                string bastarBittar = izinTalep.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + izinTalep.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                //  string izintipi=izinTalep.IzinTipi?Pr
                IzinTanim izintipi = new IzinTanim();
                izintipi = izintipi.Select<IzinTanim>(izinTalep.IzinTipi);
                string izintipiStr = izintipi == null ? "" : izintipi.Adi;

                string subject = bastarBittar + " tarihleri arasindaki " + izintipiStr + " Izin talebiniz. ";
                string userurl = "<a href = 'http://tskgv-portal/KullaniciUygulamalari/Sayfalar/KisiselSayfa.aspx'>kisisel sayfanizdan </a>";

                string body = izintipiStr + " Izin talebiniz olusturulmustur. <br>Lütfen " + userurl + " talebinizin durumunu takip ediniz.";

                IletisimBilgileri ib = new IletisimBilgileri();
                ib = ib.SelectByPersonelId(personel.Id);
                if (ib != null)
                {
                    to = ib.IntranetEPosta;
                }
                //Izin sahibine eposta gönder
                string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL);
                MailHelper.EPostaGonder(from, to, subject, body, smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);

                //IKYS Onay grubuna eposta gönder
                to = "izinonaymailgrubu@tskgv.local";
                subject = personel.Adi + " " + personel.Soyadi + " Yeni " + izintipiStr + " izin talebi ";
                string url = "<a href = 'http://tskgv-portal/YonetimBirimleri/PersonelVeIdariIslerSubesi/Sayfalar/IzinTalepListesi.aspx'>Izin Talepleri </a>";
                if (izinTalep.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                {
                    url = "<a href = 'http://tskgv-portal/YonetimBirimleri/PersonelVeIdariIslerSubesi/Sayfalar/MazeretIzinTalepListesi.aspx'>Izin Talepleri </a>";
                }
                body = personel.Adi + " " + personel.Soyadi + " " + bastarBittar + " tarihleri arasinda " + izintipiStr + " izin talebinde bulunmustur. <br>Lütfen kontrol-red islemleri için " + url + " sayfasina gidiniz";
                MailHelper.EPostaGonder(from, to, subject, body, smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);
            }

        }
        public static void GorevOnayEPostasiGonder(Personel personel, int gorevOnayId, string tip)
        {
            string from = "ikys@tskgv.local";
            string to = "gorevonaymailgrubu@tskgv.org.tr";
            string url = string.Empty;
            string body = string.Empty;
            string subject = string.Empty;

            if (tip.Equals("SehirIci"))
            {
                Yoklama yoklama = new Yoklama();
                yoklama = yoklama.Select<Yoklama>(gorevOnayId);
                if (yoklama != null)
                {
                    string bastarBittar = yoklama.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + yoklama.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                    subject = personel.Adi + " " + personel.Soyadi + " tarafindan " + bastarBittar + " tarihleri arasinda Sehir Içi Görev girilmistir. ";
                    url = "<a href = 'http://tskgv-portal/YonetimBirimleri/PersonelVeIdariIslerSubesi/Sayfalar/YoklamaListesi.aspx'>Yoklama Listesi </a>";
                    body = personel.Adi + " " + personel.Soyadi + " tarafindan " + bastarBittar + " tarihleri arasinda Sehir Içi Görev Onayi girilmistir. <br>Yoklama islemlerini " + url + " sayfasindan yapabilirsiniz.";

                    string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL);
                    MailHelper.EPostaGonder(from, to, subject, body, smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);

                }
            }
            else
            {
                GorevOnay gorevOnay = new GorevOnay();
                gorevOnay = gorevOnay.Select<GorevOnay>(gorevOnayId);
                if (gorevOnay != null)
                {
                    string bastarBittar = gorevOnay.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + gorevOnay.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                    subject = personel.Adi + " " + personel.Soyadi + " tarafindan " + bastarBittar + " tarihleri arasinda Yurt Içi/Yurt Disi Görev Onayi girilmistir. ";
                    url = "<a href = 'http://tskgv-portal/YonetimBirimleri/PersonelVeIdariIslerSubesi/Sayfalar/GorevOnayListesi.aspx'>Görev Onay Listesi </a>";
                    body = personel.Adi + " " + personel.Soyadi + " tarafindan " + bastarBittar + " tarihleri arasinda Yurt Içi/Yurt Disi Görev Onayi girilmistir. <br>Görev onay islemlerini " + url + " sayfasindan yapabilirsiniz.";
                    StringBuilder tabloSB = EpostaTablosunuOlustur(gorevOnay);
                    body += "</br>" + tabloSB.ToString();
                    string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL);
                    MailHelper.EPostaGonder(from, to, subject, body, smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);
                }
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
            sb.Append("<th colspan='2' style='text-align: center; background-color: #dddddd'>Görev Bilgileri</th>");
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

            sb.Append("</table><br/><br/>");
            sb.Append("<p>");
            sb.Append("Insan Kaynaklari Yönetim Sistemi</br>" + DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
            sb.Append("</p>");
            sb.Append("<p style='color:gray; font-family: arial;font-size:xx-small;'>IKYS &trade; Bilgi Sistemleri Kismi </p> ");
            return sb;
        }
        public static void BilgiSistemMailGrubunaEPostaGonder(PersonelItem personelItem, string baslik )
        {
            string userto = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_BILGISISTEM_MAILGRUBU);

            string from = "IKYS <ikys@tskgv.local>";
            string subject = baslik;


            string grupAdi = userto.Split('@')[0];
            bool grupMu = UtilityHelper.IsGroup("TSKGV", grupAdi);//böyle bir grup var mi
            if (grupMu)// varsa
            {
                List<UserPrincipal> uplist = UtilityHelper.GetGroupMembers("TSKGV", grupAdi);
                var emaillist = uplist.Select(item => item.EmailAddress).ToList();
                string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL);
                foreach (var eposta in emaillist)
                {

                    StringBuilder tabloSB = PersonelBilgileriTablosunuOlustur(personelItem, baslik);
                    
                    MailHelper.EPostaGonder(from, eposta, subject, "</br>" + tabloSB.ToString(), smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);

                }
            }
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
            sb.Append("<td>" + personelItem.PersonelBilgileri.Adi + " " + personelItem.PersonelBilgileri.Soyadi+ "</td>");
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
            sb.Append("<td>" + personelItem.IletisimBilgileri.CepTelefonu+ "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Birimi</th>");
            sb.Append("<td>" + personelItem.IsBilgileriItem.Birim?.Adi + "</td>");
            sb.Append("</tr>");
            sb.Append("<th>Ünvani</th>");
            sb.Append("<td>" + personelItem.IsBilgileriItem.Unvan?.Adi+ "</td>");
            sb.Append("</tr>");
           

            sb.Append("</table><br/><br/>");
            sb.Append("<p>");
            sb.Append("IKYS.</br>" + DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
            sb.Append("</p>");
            sb.Append("<p style='color:gray; font-family: arial;font-size:xx-small;'>TYS &trade; Bilgi Sistem Kismi </p> ");
            return sb;
        }
        private static string UstBirimGetir(int parentId)
        {
            string retVal = parentId + ",";
            BirimTanim bt = new BirimTanim();
            List<BirimTanim> list = bt.SelectByParentId(parentId);
            foreach (BirimTanim item in list)
            {
                //retVal += item.Id + ",";
                string val = UstBirimGetir(item.Id);
                if (string.IsNullOrEmpty(val))
                {
                    return retVal;
                }
                retVal += val;
            }
            return retVal;
        }
        public static string BirimListesiGetir(Personel personel)
        {
            string birimIdStr = string.Empty;

            if (personel != null)
            {
                IsBilgileri ib = new IsBilgileri();
                ib = ib.SelectByPersonelId(personel.Id);
                if (ib != null)
                {
                    int birimId = ib.BirimId;
                    BirimTanim bt = new BirimTanim();
                    bt = bt.Select<BirimTanim>(birimId);
                    if ((bt != null) && (bt.AmirId == personel.Id))
                    {
                        string birim = UstBirimGetir(birimId);
                        birimIdStr = string.IsNullOrEmpty(birim) ? "" : birim.Substring(0, birim.Length - 1);
                    }

                }
            }
            return birimIdStr;
        }
        public static Personel PersonelGetir(string currentUserName)
        {
            Personel personel = new Personel();
            {
                string userName = currentUserName.Substring(currentUserName.LastIndexOf("\\") + 1, currentUserName.Length - currentUserName.LastIndexOf("\\") - 1);
                personel = personel.SelectByUserName(userName);
            }
            return personel;
        }

        public static string PersonelinBolgesiniGetir_Deprecated(string currentUserName)
        {
            Personel personel = PersonelGetir(currentUserName);
           
            if (personel!=null)
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
                MessageHelper.PublishMessage("Personel bulunamadi",ProjeConstants.MESAJ_HATA);
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
        public static int KalanIzinToplamiGetir(int personelId, bool sadeceEskiDonemler)
        {

            IzinDonem izinDonemDao = new IzinDonem();
            int kalanIzinToplami = 0;

            DataTable dataTable = izinDonemDao.SelectSUMKalanIzinByPersonelId(personelId, sadeceEskiDonemler);
            if (dataTable != null)
            {
                DataRow dataRow = dataTable.Rows[0];

                kalanIzinToplami = dataRow["KalanIzinToplami"].ConvertToInt();// - izinDonemi.KalanIzin.ConvertToInt();
            }

            return kalanIzinToplami;
        }
        #endregion IKYS
    }
}
