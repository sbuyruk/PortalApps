using Model.IKYS;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Ortak
{
    public class MTSOrtak
    {
        public MTSOrtak()
        {

        }
        #region Toplanti
        /// <summary>
        /// Toplanti e-postasi olusturma
        /// </summary>
        /// <param name="toplantiId"></param>
        /// <returns></returns>
        public static void ToplantiKatilimcilarinaEPostaGonder(Toplanti toplanti, string islemTipi, List<ToplantiKatilim> oncekiKatilimciListesi)
        {
            if (toplanti != null)
            {

                
                ToplantiKatilim toplantiKatilim = new ToplantiKatilim();
                List<ToplantiKatilim> katilimciListesi = toplantiKatilim.SelectBytoplantiId(toplanti.Id);
                StringBuilder tabloSB = new StringBuilder();
                EPostaGondelienleriTemizle();
                foreach (var item in katilimciListesi)
                {
                    string from = "Toplanti Yönetim Sistemi <tys@tskgv.local>";
                    string baslik = string.Empty;
                    string subject = string.Empty;


                    

                    string toplantiAdi = toplanti.ToplantiKonusu + " konulu ve " + toplanti.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + " tarihli toplanti";
                    switch (islemTipi)
                    {
                        case ProjeConstants.KAYDET:
                            {
                                subject = "Yeni Toplanti : " + toplantiAdi + " olusturulmustur.";
                                baslik = "<u><b style='color:green;'>YENI TOPLANTI</b></u>" +
                                    "</br>Asagida ayrintilari bulunan <b>'" + toplantiAdi + 
                                    (item.Bilgi?"'ya BILGI amaçli eklendiniz. ": "ya KATILIMCI olarak eklendiniz. <br/><br/>");
                                break;
                            }
                        case ProjeConstants.GUNCELLE:
                            {
                                subject = "Toplanti Güncelleme : " + toplantiAdi + "da degisiklik yapilmistir.";
                                baslik = "<u><b style='color:blue;'>TOPLANTI GÜNCELLEME</b></u>" +
                                    "</br>Asagida ayrintilari bulunan <b>'" + toplantiAdi + "da degisiklik yapilmistir. <br/><br/>";
                                break;
                            }
                        case ProjeConstants.SIL:
                            {
                                subject = "Toplanti Iptali : " + toplantiAdi + " iptal edilmistir.";
                                baslik = "<u><b style='color:red;'>TOPLANTI IPTALI!</b></u>" +
                                    "</br>Asagida ayrintilari bulunan <b>'" + toplantiAdi + " iptal edilmistir. <br/><br/>";
                                break;
                            }

                        default:
                            break;
                    }
                    int personelId = item.KatilimciId;
                    IletisimBilgileri iletisimBilgisi = new IletisimBilgileri();
                    iletisimBilgisi = iletisimBilgisi.SelectByPersonelId(personelId);
                    string userto = iletisimBilgisi.IntranetEPosta;
                    if (string.IsNullOrEmpty(userto))
                    {
                        Personel personel = new Personel();
                        personel = personel.Select(personelId);
                        if (personel != null)
                        {

                            MessageHelper.PublishMessage(personel.Adi+" "+personel.Soyadi + " adli kisiye ait Iletisim billgilerinde e-posta adresi bulunamadi.", ProjeConstants.MESAJ_HATA);
                        }
                        else
                        {
                            MessageHelper.PublishMessage("Iletisim billgilerinde e-posta adresi bulunamadi. personelId="+personelId, ProjeConstants.MESAJ_HATA);
                        }
                    }
                    else{
                        if (!EpostaGonderilenlerList.Contains(userto))
                        {
                            Personel personel = new Personel();
                            personel = personel.Select(personelId);
                            if (personel != null)
                            {
                                tabloSB.Append("Sayin " + personel.Adi + " " + personel.Soyadi + ",<br/><br/>");
                                tabloSB = ToplantiTablosunuOlustur(toplanti, baslik);
                                string body = "</br>" + tabloSB.ToString();
                                string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL);
                                MailHelper.EPostaGonder(from, userto, subject, body, smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);
                                if (islemTipi.Equals(ProjeConstants.KAYDET))
                                {
                                    if (!item.Bilgi)//bilgi degilse katilimcidir
                                        MailHelper.TakvimeEkle(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);
                                }
                                else if (islemTipi.Equals(ProjeConstants.GUNCELLE))
                                {
                                    ToplantiKatilim onceki = null;
                                    try
                                    {
                                        onceki = oncekiKatilimciListesi.Single(s => s.KatilimciId == personelId); //eger listede yoksa null exception döner
                                                                                                                  //bilgiden katilimciya döndüyse
                                        if (onceki.Bilgi && !item.Bilgi)
                                            MailHelper.TakvimeEkle(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);
                                        //katilimcidan bilgiye döndüyse
                                        if (!onceki.Bilgi && item.Bilgi)
                                            MailHelper.TakvimdenSil(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);
                                        // Katilimci olarak kaldiysa
                                        if (!onceki.Bilgi && !item.Bilgi)
                                            MailHelper.TakvimeEkle(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);

                                    }
                                    catch (Exception)
                                    {
                                        onceki = null;
                                        //Onceden olmayip yeni eklendiyse
                                        if (onceki == null && !item.Bilgi)
                                            MailHelper.TakvimeEkle(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);

                                    }

                                }
                                else if (islemTipi.Equals(ProjeConstants.SIL))
                                {
                                    ToplantiKatilim onceki = oncekiKatilimciListesi.Single(s => s.KatilimciId == personelId);
                                    //önceki bilgi degil kayilimci ise
                                    if (!onceki.Bilgi)
                                        MailHelper.TakvimdenSil(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);
                                }

                                EpostaGonderilenlereEkle(userto);
                            }
                        } 
                    }
                }
            }
            else
            {
                MessageHelper.PublishMessage("Toplanti bilgisi bulunamadi", ProjeConstants.MESAJ_HATA);
            }
        }
        private static void EPostaGondelienleriTemizle()
        {
            EpostaGonderilenlerList.Clear();
        }
        private static List<string> EpostaGonderilenlerList = new List<string>();
        private static void EpostaGonderilenlereEkle(string userto)
        {
            if (!EpostaGonderilenlerList.Contains(userto))
                EpostaGonderilenlerList.Add(userto);
        }
        public static void ToplantiMailGrubunaEPostaGonder(Toplanti toplanti, string islemTipi)
        {
            string userto = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_TOPLANTI_MAILGRUBU);
            if (string.IsNullOrEmpty(userto))
            {
                MessageHelper.PublishMessage("Toplanti Parametreleri arasinda " + ProjeConstants.PARAM_TOPLANTI_MAILGRUBU + " bulunamadi, e-posta gönderilemedi", ProjeConstants.MESAJ_HATA);
            }
            else
            {
                string from = "Toplanti Yönetim Sistemi <tys@tskgv.local>";
                string baslik = string.Empty;
                string subject = string.Empty;
                string toplantiAdi = toplanti.ToplantiKonusu + " konulu ve " + toplanti.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + " tarihli toplanti";
                switch (islemTipi)
                {
                    case ProjeConstants.KAYDET:
                        {
                            subject = " Toplanti Bilgilendirme (Yeni Toplanti) :" + toplantiAdi + " olusturulmustur.";
                            baslik = "<u><b>YENI TOPLANTI</b></u>" +
                                "</br>Bu E-Posta bilgilendirme amaçli gönderilmistir." +
                                "</br> <p style='color:red;'> Asagida ayrintilari bulunan <b>'" + toplantiAdi + " olusturulmustur. <br/><br/> </p>";
                            break;
                        }
                    case ProjeConstants.GUNCELLE:
                        {
                            subject = "Toplanti Bilgilendirme (Toplanti Güncelleme) : " + toplantiAdi + "da degisiklik yapilmistir.";
                            baslik = "<u><b>TOPLANTI GÜNCELLEME</b></u>" +
                                "</br> Bu E-Posta bilgilendirme amaçli gönderilmistir." +
                                "</br> <p style='color:red;'> Asagida ayrintilari bulunan <b>'" + toplantiAdi + "da degisiklik yapilmistir. <br/><br/></p>";
                            break;
                        }
                    case ProjeConstants.SIL:
                        {
                            subject = "Toplanti Bilgilendirme (Toplanti Iptali) : " + toplantiAdi + " iptal edilmistir.";
                            baslik = "<u><b>TOPLANTI IPTALI!</b></u>" +
                                "</br> Bu E-Posta bilgilendirme amaçli gönderilmistir." +
                                "</br><p style='color:red;'>Asagida ayrintilari bulunan <b>'" + toplantiAdi + " iptal edilmistir. <br/><br/></p>";
                            break;
                        }

                    default:
                        break;
                }

                if (toplanti != null)
                {
                    if (!EpostaGonderilenlerList.Contains(userto)) //gruba gönderildiyse bi daha gönderme
                    {
                        string grupAdi = userto.Split('@')[0];
                        bool grupMu = UtilityHelper.IsGroup("TSKGV", grupAdi);//böyle bir grup var mi
                        if (grupMu)// varsa
                        {
                            List<UserPrincipal> uplist = UtilityHelper.GetGroupMembers("TSKGV", grupAdi);
                            var emaillist = uplist.Select(item => item.EmailAddress).ToList();
                            foreach (var eposta in emaillist)// bu gruptakilerden EpostaGonderilenlerList'te olmayanlara eposta gonder
                            {
                                if (!EpostaGonderilenlerList.Contains(eposta)) 
                                {
                                    StringBuilder tabloSB = ToplantiTablosunuOlustur(toplanti, baslik);
                                    string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL);
                                    MailHelper.EPostaGonder(from, eposta, subject, "</br>" + tabloSB.ToString(), smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);
                                    //MailHelper.TakvimeEkle(toplanti.UniqueId, from, eposta, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);
                                    EpostaGonderilenlereEkle(eposta);
                                }
                            }
                        }
                        EpostaGonderilenlereEkle(userto);
                    }
                    
                }
                else
                {
                    MessageHelper.PublishMessage("Toplanti bilgisi bulunamadi", ProjeConstants.MESAJ_HATA);
                }
            }
        }
        public static void IkramMailGrubunaEPostaGonder(Toplanti toplanti, string islemTipi)
        {
            string userto = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_IKRAM_MAILGRUBU);
            if (string.IsNullOrEmpty(userto))
            {
                MessageHelper.PublishMessage("Toplanti Parametreleri arasinda " + ProjeConstants.PARAM_IKRAM_MAILGRUBU + " bulunamadi, e-posta gönderilemedi", ProjeConstants.MESAJ_HATA);
            }
            else
            {
                string from = "Toplanti Yönetim Sistemi <tys@tskgv.local>";
                string baslik = string.Empty;
                string subject = string.Empty;
                string toplantiAdi = toplanti.ToplantiKonusu + " konulu ve " + toplanti.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + " tarihli toplanti";
                switch (islemTipi)
                {
                    case ProjeConstants.KAYDET:
                        {
                            subject = " Toplanti Bilgilendirme (Yeni Toplanti) :" + toplantiAdi + " olusturulmustur.";
                            baslik = "<u><b>YENI TOPLANTI</b></u>" +
                                "</br>Bu toplantida Ikram yapilacaktir." +
                                "</br> <p style='color:red;'> Asagida ayrintilari bulunan <b>'" + toplantiAdi + " olusturulmustur. <br/><br/> </p>";
                            break;
                        }
                    case ProjeConstants.GUNCELLE:
                        {
                            subject = "Toplanti Bilgilendirme (Toplanti Güncelleme) : " + toplantiAdi + "da degisiklik yapilmistir.";
                            baslik = "<u><b>TOPLANTI GÜNCELLEME</b></u>" +
                                (toplanti.IkramOnayi? "</br>Bu toplantida Ikram yapilacaktir.": "</br>Bu toplantida Ikram yapilmayacaktir.") +                                
                                "</br> <p style='color:red;'> Asagida ayrintilari bulunan <b>'" + toplantiAdi + "da degisiklik yapilmistir. <br/><br/></p>";
                            break;
                        }
                    case ProjeConstants.SIL:
                        {
                            subject = "Toplanti Bilgilendirme (Toplanti Iptali) : " + toplantiAdi + " iptal edilmistir.";
                            baslik = "<u><b>TOPLANTI IPTALI!</b></u>" +
                                (toplanti.IkramOnayi ? "</br>Toplantida planlanan Ikram iptal edilmistir." : "</br>Bu eposta bilgilendirme amaciyla gönderilmistir.") +
                                "</br><p style='color:red;'>Asagida ayrintilari bulunan <b>'" + toplantiAdi + " iptal edilmistir. <br/><br/></p>";
                            break;
                        }

                    default:
                        break;
                }

                if (toplanti != null)
                {
                    if (!EpostaGonderilenlerList.Contains(userto)) //gruba gönderildiyse bi daha gönderme
                    {
                        string grupAdi = userto.Split('@')[0];
                        bool grupMu = UtilityHelper.IsGroup("TSKGV", grupAdi);//böyle bir grup var mi
                        if (grupMu)// varsa
                        {
                            List<UserPrincipal> uplist = UtilityHelper.GetGroupMembers("TSKGV", grupAdi);
                            var emaillist = uplist.Select(item => item.EmailAddress).ToList();
                            foreach (var eposta in emaillist)// bu gruptakilerden EpostaGonderilenlerList'te olmayanlara eposta gonder
                            {
                                if (!EpostaGonderilenlerList.Contains(eposta))
                                {
                                    StringBuilder tabloSB = ToplantiTablosunuOlustur(toplanti, baslik);
                                    string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL);
                                    MailHelper.EPostaGonder(from, eposta, subject, "</br>" + tabloSB.ToString(), smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);
                                    //MailHelper.TakvimeEkle(toplanti.UniqueId, from, eposta, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);
                                    EpostaGonderilenlereEkle(eposta);
                                }
                            }
                        }
                        EpostaGonderilenlereEkle(userto);
                    }

                }
                else
                {
                    MessageHelper.PublishMessage("Toplanti bilgisi bulunamadi", ProjeConstants.MESAJ_HATA);
                }
            }
        }
        public static void BilgiSistemMailGrubunaEPostaGonder(Toplanti toplanti, string islemTipi)
        {
            string userto = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_BILGISISTEM_MAILGRUBU);
            if (string.IsNullOrEmpty(userto))
            {
                MessageHelper.PublishMessage("Toplanti Parametreleri arasinda " + ProjeConstants.PARAM_BILGISISTEM_MAILGRUBU + " bulunamadi, e-posta gönderilemedi", ProjeConstants.MESAJ_HATA);
            }
            else
            {
                if (!EpostaGonderilenlerList.Contains(userto))
                {
                    string from = "Toplanti Yönetim Sistemi <tys@tskgv.local>";
                    string baslik = string.Empty;
                    string subject = string.Empty;
                    string toplantiAdi = toplanti.ToplantiKonusu + " konulu ve " + toplanti.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + " tarihli ÇEVRIMIÇI toplanti";
                    switch (islemTipi)
                    {
                        case ProjeConstants.KAYDET:
                            {
                                subject = " Toplanti Bilgilendirme (Yeni Toplanti) :" + toplantiAdi + " olusturulmustur.";
                                baslik = "<u><b>YENI ÇEVRIMIÇI TOPLANTI</b></u>" +
                                    "</br>Bu E-Posta bilgilendirme amaçli gönderilmistir." +
                                    "</br> <p style='color:red;'> Asagida ayrintilari bulunan <b>'" + toplantiAdi + " olusturulmustur. <br/><br/> </p>";
                                break;
                            }
                        case ProjeConstants.GUNCELLE:
                            {
                                subject = "Toplanti Bilgilendirme (Toplanti Güncelleme) : " + toplantiAdi + "da degisiklik yapilmistir.";
                                baslik = "<u><b>ÇEVRIMIÇI TOPLANTI GÜNCELLEME</b></u>" +
                                    "</br> Bu E-Posta bilgilendirme amaçli gönderilmistir." +
                                    "</br> <p style='color:red;'> Asagida ayrintilari bulunan <b>'" + toplantiAdi + "da degisiklik yapilmistir. <br/><br/></p>";
                                break;
                            }
                        case ProjeConstants.SIL:
                            {
                                subject = "Toplanti Bilgilendirme (Toplanti Iptali) : " + toplantiAdi + " iptal edilmistir.";
                                baslik = "<u><b>ÇEVRIMIÇI TOPLANTI IPTALI!</b></u>" +
                                    "</br> Bu E-Posta bilgilendirme amaçli gönderilmistir." +
                                    "</br><p style='color:red;'>Asagida ayrintilari bulunan <b>'" + toplantiAdi + " iptal edilmistir. <br/><br/></p>";
                                break;
                            }

                        default:
                            break;
                    }
                    if (toplanti != null)
                    {
                        string grupAdi = userto.Split('@')[0];
                        bool grupMu = UtilityHelper.IsGroup("TSKGV", grupAdi);//böyle bir grup var mi
                        if (grupMu)// varsa
                        {
                            List<UserPrincipal> uplist = UtilityHelper.GetGroupMembers("TSKGV", grupAdi);
                            var emaillist = uplist.Select(item => item.EmailAddress).ToList();
                            foreach (var eposta in emaillist)// bu gruptakilerden EpostaGonderilenlerList'te olmayanlara eposta gonder
                            {
                                if (!EpostaGonderilenlerList.Contains(eposta))
                                {
                                    StringBuilder tabloSB = ToplantiTablosunuOlustur(toplanti, baslik);
                                    string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL);
                                    MailHelper.EPostaGonder(from, eposta, subject, "</br>" + tabloSB.ToString(), smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);
                                    //MailHelper.TakvimeEkle(toplanti.UniqueId, from, eposta, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);
                                    EpostaGonderilenlereEkle(eposta);
                                }
                            }
                        }
                        EpostaGonderilenlereEkle(userto);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Toplanti bilgisi bulunamadi", ProjeConstants.MESAJ_HATA);
                    }
                }
            }
        }
        public static void ToplantidanCikanKatilimcilaraEPostaGonder(Toplanti toplanti, List<int> cikanKatilimciList, List<ToplantiKatilim> oncekiKatilimciListesi)
        {

            string from = "Toplanti Yönetim Sistemi <tys@tskgv.local>";
            string toplantiAdi = toplanti.ToplantiKonusu + " konulu ve " + toplanti.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + " tarihli toplanti";
            string subject = toplantiAdi + "nin katilim listesinden çikarildiniz.";

            string baslik = "<u><b style='color:darkred;'>TOPLANTI BILGILERI</b></u>" +
                            "</br>Asagida ayrintilari bulunan <b>'" + toplantiAdi + "dan çikarildiniz.<br/><br/>";

            if (toplanti != null)
            {
                ToplantiKatilim toplantiKatilim = new ToplantiKatilim();

                StringBuilder tabloSB = new StringBuilder();
                foreach (var item in cikanKatilimciList)
                {
                    int personelId = item;
                    IletisimBilgileri iletisimBilgisi = new IletisimBilgileri();
                    iletisimBilgisi = iletisimBilgisi.SelectByPersonelId(personelId);
                    string userto = iletisimBilgisi.IntranetEPosta;

                    Personel personel = new Personel();
                    personel = personel.Select(personelId);

                    if (personel != null)
                    {
                        tabloSB.Append("Sayin " + personel.Adi + " " + personel.Soyadi + ",<br/><br/>");
                        tabloSB = ToplantiTablosunuOlustur(toplanti, baslik);

                        string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL);
                        MailHelper.EPostaGonder(from, userto, subject, "</br>" + tabloSB.ToString(), smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);
                        
                        ToplantiKatilim onceki = oncekiKatilimciListesi.Single(s => s.KatilimciId == personelId);
                        //önceki bilgi degil kayilimci ise
                        if (!onceki.Bilgi)
                            MailHelper.TakvimdenSil(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);


                        MailHelper.TakvimdenSil(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);
                        EpostaGonderilenlerList.Remove(userto);
                    }
                }
            }
            else
            {
                MessageHelper.PublishMessage("Toplanti bilgisi bulunamadi", ProjeConstants.MESAJ_HATA);
            }
        }
        public static void ToplantiYetkilisineEPostaGonder(Toplanti toplanti, string islemTipi)
        {

            string from = "Toplanti Yönetim Sistemi <tys@tskgv.local>";
            string baslik = string.Empty;
            string subject = string.Empty;
            string toplantiAdi = toplanti.ToplantiKonusu + " konulu ve " + toplanti.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + " tarihli toplanti";

            switch (islemTipi)
            {
                case ProjeConstants.KAYDET:
                    {
                        subject = "Yeni Toplanti : " + toplantiAdi + "yi olusturdunuz.";
                        baslik = "<u><b style='color:green;'>YENI TOPLANTI</b></u>" +
                            "</br>Asagida ayrintilari bulunan <b>'" + toplantiAdi + "yi olusturdunuz. <br/><br/>";
                        break;
                    }
                case ProjeConstants.GUNCELLE:
                    {
                        subject = "Toplanti Güncelleme : " + toplantiAdi + "da degisiklik yaptiniz.";
                        baslik = "<u><b style='color:blue;'>TOPLANTI GÜNCELLEME</b></u>" +
                            "</br>Asagida ayrintilari bulunan <b>'" + toplantiAdi + "da degisiklik yaptiniz. <br/><br/>";
                        break;
                    }
                case ProjeConstants.SIL:
                    {
                        subject = "Toplanti Iptali : " + toplantiAdi + "yi iptal ettiniz.";
                        baslik = "<u><b style='color:red;'>TOPLANTI IPTALI!</b></u>" +
                            "</br>Asagida ayrintilari bulunan <b>'" + toplantiAdi + "yi iptal ettiniz. <br/><br/>";
                        break;
                    }

                default:
                    break;
            }


            if (toplanti != null)
            {

                int toplantiYetkilisiId = toplanti.ToplantiYetkilisi;
                Personel toplantiYetkilisi = new Personel();
                toplantiYetkilisi = toplantiYetkilisi.Select(toplantiYetkilisiId);
                
                if (toplantiYetkilisi != null)
                {
                    IletisimBilgileri ib = new IletisimBilgileri();
                    ib = ib.SelectByPersonelId(toplantiYetkilisiId);
                    string userto = ib.IntranetEPosta;

                    if (!EpostaGonderilenlerList.Contains(userto))
                    {
                        StringBuilder tabloSB = new StringBuilder();
                        tabloSB.Append("Sayin " + toplantiYetkilisi.Adi + " " + toplantiYetkilisi.Soyadi + ",<br/><br/>");
                        tabloSB = ToplantiTablosunuOlustur(toplanti, baslik);
                        string body = "</br>" + tabloSB.ToString();
                        string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL);

                        MailHelper.EPostaGonder(from, userto, subject, body, smtpAdresi ?? ProjeConstants.PARAM_ALTERNATIVE_SMTP_IP_ADRESI);
                        //MailHelper.TakvimeEkle(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);
                        EpostaGonderilenlereEkle(userto);
                    }
                }
            }
            else
            {
                MessageHelper.PublishMessage("Toplanti bilgisi bulunamadi", ProjeConstants.MESAJ_HATA);
            }

        }
        private static StringBuilder ToplantiTablosunuOlustur(Toplanti toplanti, string baslik)
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
            sb.Append("<th colspan='2' style='text-align: center; background-color: #dddddd'>Toplanti Ayrintilari</th>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Toplanti Numarasi</th>");
            sb.Append("<td>" + toplanti.Id + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Toplanti Konusu</th>");
            sb.Append("<td>" + toplanti.ToplantiKonusu + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Baslangiç Zamani</th>");
            sb.Append("<td>" + toplanti.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Bitis Zamani</th>");
            sb.Append("<td>" + toplanti.BitisTarihi.ToString("dd.MM.yyyy HH:mm") + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Toplanti Yeri</th>");
            sb.Append("<td>" + ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger) + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Toplanti Yetkilisi</th>");
            sb.Append("<td>" + ParseToplantiYetkilisi(toplanti.ToplantiYetkilisi) + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Koordinator</th>");
            sb.Append("<td>" + ParseKoordinator(toplanti.Koordinator) + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Iç Katilimcilar</th>");
            sb.Append("<td>" + ParseKatilimciListesi(toplanti.Id) + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Dis Katilimcilar</th>");
            sb.Append("<td>" + toplanti.DisKatilimcilar + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Bilgi</th>");
            sb.Append("<td>" + ParseBilgiListesi(toplanti.Id) + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Çevrimiçi</th>");
            sb.Append("<td>" + (toplanti.CevrimIci ? "Evet" : "Hayir") + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Ikram Onayi</th>");
            sb.Append("<td>" + (toplanti.IkramOnayi ? "Evet" : "Hayir") + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Açiklama</th>");
            sb.Append("<td>" + toplanti.Aciklama + "</td>");
            sb.Append("</tr>"); 
            if (toplanti.IkramOnayi)
            {
                sb.Append("<tr>");
                sb.Append("<th>Ikram Malzemesi</th>");
                sb.Append("<td>" + toplanti.IkramMalzemesi + "</td>");
                sb.Append("</tr>");
            }

            sb.Append("</table><br/><br/>");
            sb.Append("<p>");
            sb.Append("Toplanti Yönetim Sistemi.</br>" + DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
            sb.Append("</p>");
            sb.Append("<p style='color:gray; font-family: arial;font-size:xx-small;'>TYS &trade; Bilgi Sistem Kismi </p> ");
            return sb;
        }
        private static string ParseKatilimciListesi(int toplantiId)
        {
            string icKatilimcilarStr = string.Empty;
            Personel personel = new Personel();
            List<Personel> list = personel.SelectKatilimcilarByToplantiIdList(toplantiId);
            foreach (var item in list)
            {
                icKatilimcilarStr += item.Adi + " " + item.Soyadi + "</br>";
            }
            return icKatilimcilarStr;
        }
        private static string ParseBilgiListesi(int toplantiId)
        {
            string icKatilimcilarStr = string.Empty;
            Personel personel = new Personel();
            List<Personel> list = personel.SelectBilgiVerilenlerByToplantiIdList(toplantiId);
            foreach (var item in list)
            {
                icKatilimcilarStr += item.Adi + " " + item.Soyadi + "</br>";
            }
            return icKatilimcilarStr;
        }
        private static string ParseToplantiYeri(int yeri, string diger)
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
        private static string ParseKoordinator(int koordinator)
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
        private static string ParseToplantiYetkilisi(int toplantiYetkilisi)
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
        #endregion Toplanti
        public static string ParseFaaliyetAmaci(string amac)
        {
            string amacStr = string.Empty;
            switch (amac)
            {
                case ProjeConstants.FAALIYET_AMACI_DAVET_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_DAVET;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_IZIN_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_IZIN;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_OZELCALISMA_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_OZELCALISMA;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_RESMITATIL_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_RESMITATIL;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_TOPLANTI_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_TOPLANTI;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_YILDONUMU_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_YILDONUMU;
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
                default:
                    break;
            }
            return amacStr;
        }
        public static string ParseFaaliyetDurumu(int durum)
        {
            string durumStr = string.Empty;
            switch (durum)
            {
                case ProjeConstants.FAALIYET_DURUMU_PLANLANDI_INT:
                    {
                        durumStr = ProjeConstants.FAALIYET_DURUMU_PLANLANDI;
                        break;
                    }
                case ProjeConstants.FAALIYET_DURUMU_ONAYLANDI_INT:
                    {
                        durumStr = ProjeConstants.FAALIYET_DURUMU_ONAYLANDI;
                        break;
                    }
                case ProjeConstants.FAALIYET_DURUMU_IPTALEDILDI_INT:
                    {
                        durumStr = ProjeConstants.FAALIYET_DURUMU_IPTALEDILDI;
                        break;
                    }
                default:
                    break;
            }
            return durumStr;
        }
    }
}
