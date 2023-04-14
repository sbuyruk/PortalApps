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
        #region Toplantı
        /// <summary>
        /// Toplantı e-postası oluşturma
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
                    string from = "Toplantı Yönetim Sistemi <tys@tskgv.local>";
                    string baslik = string.Empty;
                    string subject = string.Empty;


                    

                    string toplantiAdi = toplanti.ToplantiKonusu + " konulu ve " + toplanti.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + " tarihli toplantı";
                    switch (islemTipi)
                    {
                        case ProjeConstants.KAYDET:
                            {
                                subject = "Yeni Toplantı : " + toplantiAdi + " oluşturulmuştur.";
                                baslik = "<u><b style='color:green;'>YENİ TOPLANTI</b></u>" +
                                    "</br>Aşağıda ayrıntıları bulunan <b>'" + toplantiAdi + 
                                    (item.Bilgi?"'ya BİLGİ amaçlı eklendiniz. ": "ya KATILIMCI olarak eklendiniz. <br/><br/>");
                                break;
                            }
                        case ProjeConstants.GUNCELLE:
                            {
                                subject = "Toplantı Güncelleme : " + toplantiAdi + "da değişiklik yapılmıştır.";
                                baslik = "<u><b style='color:blue;'>TOPLANTI GÜNCELLEME</b></u>" +
                                    "</br>Aşağıda ayrıntıları bulunan <b>'" + toplantiAdi + "da değişiklik yapılmıştır. <br/><br/>";
                                break;
                            }
                        case ProjeConstants.SIL:
                            {
                                subject = "Toplantı İptali : " + toplantiAdi + " iptal edilmiştir.";
                                baslik = "<u><b style='color:red;'>TOPLANTI İPTALİ!</b></u>" +
                                    "</br>Aşağıda ayrıntıları bulunan <b>'" + toplantiAdi + " iptal edilmiştir. <br/><br/>";
                                break;
                            }

                        default:
                            break;
                    }
                    int personelId = item.KatilimciId;
                    IletisimBilgileri iletisimBilgisi = new IletisimBilgileri();
                    iletisimBilgisi = iletisimBilgisi.SelectByPersonelId(personelId);
                    string userto = iletisimBilgisi.IntranetEPosta;
                    if (!EpostaGonderilenlerList.Contains(userto))
                    {
                        Personel personel = new Personel();
                        personel = personel.Select(personelId);
                        if (personel != null)
                        {
                            tabloSB.Append("Sayın " + personel.Adi + " " + personel.Soyadi + ",<br/><br/>");
                            tabloSB = ToplantiTablosunuOlustur(toplanti, baslik);
                            string body = "</br>" + tabloSB.ToString();
                            string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL);
                            MailHelper.EPostaGonder(from, userto, subject, body, smtpAdresi ?? ProjeConstants.PARAM_SMTP_IP_ADRESI);
                            if (islemTipi.Equals(ProjeConstants.KAYDET))
                            {
                                if (!item.Bilgi)//bilgi değilse katılımcıdır
                                    MailHelper.TakvimeEkle(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);
                            }
                            else if (islemTipi.Equals(ProjeConstants.GUNCELLE))
                            {
                                ToplantiKatilim onceki = null;
                                try
                                {
                                    onceki=oncekiKatilimciListesi.Single(s => s.KatilimciId == personelId); //eğer listede yoksa null exception döner
                                    //bilgiden katılımcıya döndüyse
                                    if (onceki.Bilgi && !item.Bilgi)
                                        MailHelper.TakvimeEkle(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);
                                    //katılımcıdan bilgiye döndüyse
                                    if (!onceki.Bilgi && item.Bilgi)
                                        MailHelper.TakvimdenSil(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);
                                    // Katılımcı olarak kaldıysa
                                    if (!onceki.Bilgi && !item.Bilgi)
                                        MailHelper.TakvimeEkle(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);

                                }
                                catch (Exception)
                                {
                                    onceki = null;
                                    //Onceden olmayıp yeni eklendiyse
                                    if (onceki == null && !item.Bilgi)
                                        MailHelper.TakvimeEkle(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);

                                }

                            }
                            else if (islemTipi.Equals(ProjeConstants.SIL))
                            {
                                ToplantiKatilim onceki = oncekiKatilimciListesi.Single(s => s.KatilimciId == personelId);
                                //önceki bilgi değil kayılımcı ise
                                if (!onceki.Bilgi)
                                    MailHelper.TakvimdenSil(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);
                            }

                            EpostaGonderilenlereEkle(userto);
                        } 
                    }
                }
            }
            else
            {
                MessageHelper.PublishMessage("Toplantı bilgisi bulunamadı", ProjeConstants.MESAJ_HATA);
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
                MessageHelper.PublishMessage("Toplantı Parametreleri arasında " + ProjeConstants.PARAM_TOPLANTI_MAILGRUBU + " bulunamadı, e-posta gönderilemedi", ProjeConstants.MESAJ_HATA);
            }
            else
            {
                string from = "Toplantı Yönetim Sistemi <tys@tskgv.local>";
                string baslik = string.Empty;
                string subject = string.Empty;
                string toplantiAdi = toplanti.ToplantiKonusu + " konulu ve " + toplanti.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + " tarihli toplantı";
                switch (islemTipi)
                {
                    case ProjeConstants.KAYDET:
                        {
                            subject = " Toplantı Bilgilendirme (Yeni Toplantı) :" + toplantiAdi + " oluşturulmuştur.";
                            baslik = "<u><b>YENİ TOPLANTI</b></u>" +
                                "</br>Bu E-Posta bilgilendirme amaçlı gönderilmiştir." +
                                "</br> <p style='color:red;'> Aşağıda ayrıntıları bulunan <b>'" + toplantiAdi + " oluşturulmuştur. <br/><br/> </p>";
                            break;
                        }
                    case ProjeConstants.GUNCELLE:
                        {
                            subject = "Toplantı Bilgilendirme (Toplantı Güncelleme) : " + toplantiAdi + "da değişiklik yapılmıştır.";
                            baslik = "<u><b>TOPLANTI GÜNCELLEME</b></u>" +
                                "</br> Bu E-Posta bilgilendirme amaçlı gönderilmiştir." +
                                "</br> <p style='color:red;'> Aşağıda ayrıntıları bulunan <b>'" + toplantiAdi + "da değişiklik yapılmıştır. <br/><br/></p>";
                            break;
                        }
                    case ProjeConstants.SIL:
                        {
                            subject = "Toplantı Bilgilendirme (Toplantı İptali) : " + toplantiAdi + " iptal edilmiştir.";
                            baslik = "<u><b>TOPLANTI İPTALİ!</b></u>" +
                                "</br> Bu E-Posta bilgilendirme amaçlı gönderilmiştir." +
                                "</br><p style='color:red;'>Aşağıda ayrıntıları bulunan <b>'" + toplantiAdi + " iptal edilmiştir. <br/><br/></p>";
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
                        bool grupMu = UtilityHelper.IsGroup("TSKGV", grupAdi);//böyle bir grup var mı
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
                                    MailHelper.EPostaGonder(from, eposta, subject, "</br>" + tabloSB.ToString(), smtpAdresi ?? ProjeConstants.PARAM_SMTP_IP_ADRESI);
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
                    MessageHelper.PublishMessage("Toplantı bilgisi bulunamadı", ProjeConstants.MESAJ_HATA);
                }
            }
        }
        public static void IkramMailGrubunaEPostaGonder(Toplanti toplanti, string islemTipi)
        {
            string userto = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_IKRAM_MAILGRUBU);
            if (string.IsNullOrEmpty(userto))
            {
                MessageHelper.PublishMessage("Toplantı Parametreleri arasında " + ProjeConstants.PARAM_IKRAM_MAILGRUBU + " bulunamadı, e-posta gönderilemedi", ProjeConstants.MESAJ_HATA);
            }
            else
            {
                string from = "Toplantı Yönetim Sistemi <tys@tskgv.local>";
                string baslik = string.Empty;
                string subject = string.Empty;
                string toplantiAdi = toplanti.ToplantiKonusu + " konulu ve " + toplanti.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + " tarihli toplantı";
                switch (islemTipi)
                {
                    case ProjeConstants.KAYDET:
                        {
                            subject = " Toplantı Bilgilendirme (Yeni Toplantı) :" + toplantiAdi + " oluşturulmuştur.";
                            baslik = "<u><b>YENİ TOPLANTI</b></u>" +
                                "</br>Bu toplantıda İkram yapılacaktır." +
                                "</br> <p style='color:red;'> Aşağıda ayrıntıları bulunan <b>'" + toplantiAdi + " oluşturulmuştur. <br/><br/> </p>";
                            break;
                        }
                    case ProjeConstants.GUNCELLE:
                        {
                            subject = "Toplantı Bilgilendirme (Toplantı Güncelleme) : " + toplantiAdi + "da değişiklik yapılmıştır.";
                            baslik = "<u><b>TOPLANTI GÜNCELLEME</b></u>" +
                                (toplanti.IkramOnayi? "</br>Bu toplantıda İkram yapılacaktır.": "</br>Bu toplantıda İkram yapılmayacaktır.") +                                
                                "</br> <p style='color:red;'> Aşağıda ayrıntıları bulunan <b>'" + toplantiAdi + "da değişiklik yapılmıştır. <br/><br/></p>";
                            break;
                        }
                    case ProjeConstants.SIL:
                        {
                            subject = "Toplantı Bilgilendirme (Toplantı İptali) : " + toplantiAdi + " iptal edilmiştir.";
                            baslik = "<u><b>TOPLANTI İPTALİ!</b></u>" +
                                (toplanti.IkramOnayi ? "</br>Toplantıda planlanan İkram iptal edilmiştir." : "</br>Bu eposta bilgilendirme amacıyla gönderilmiştir.") +
                                "</br><p style='color:red;'>Aşağıda ayrıntıları bulunan <b>'" + toplantiAdi + " iptal edilmiştir. <br/><br/></p>";
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
                        bool grupMu = UtilityHelper.IsGroup("TSKGV", grupAdi);//böyle bir grup var mı
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
                                    MailHelper.EPostaGonder(from, eposta, subject, "</br>" + tabloSB.ToString(), smtpAdresi ?? ProjeConstants.PARAM_SMTP_IP_ADRESI);
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
                    MessageHelper.PublishMessage("Toplantı bilgisi bulunamadı", ProjeConstants.MESAJ_HATA);
                }
            }
        }
        public static void BilgiSistemMailGrubunaEPostaGonder(Toplanti toplanti, string islemTipi)
        {
            string userto = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_BILGISISTEM_MAILGRUBU);
            if (string.IsNullOrEmpty(userto))
            {
                MessageHelper.PublishMessage("Toplantı Parametreleri arasında " + ProjeConstants.PARAM_BILGISISTEM_MAILGRUBU + " bulunamadı, e-posta gönderilemedi", ProjeConstants.MESAJ_HATA);
            }
            else
            {
                if (!EpostaGonderilenlerList.Contains(userto))
                {
                    string from = "Toplantı Yönetim Sistemi <tys@tskgv.local>";
                    string baslik = string.Empty;
                    string subject = string.Empty;
                    string toplantiAdi = toplanti.ToplantiKonusu + " konulu ve " + toplanti.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + " tarihli ÇEVRİMİÇİ toplantı";
                    switch (islemTipi)
                    {
                        case ProjeConstants.KAYDET:
                            {
                                subject = " Toplantı Bilgilendirme (Yeni Toplantı) :" + toplantiAdi + " oluşturulmuştur.";
                                baslik = "<u><b>YENİ ÇEVRİMİÇİ TOPLANTI</b></u>" +
                                    "</br>Bu E-Posta bilgilendirme amaçlı gönderilmiştir." +
                                    "</br> <p style='color:red;'> Aşağıda ayrıntıları bulunan <b>'" + toplantiAdi + " oluşturulmuştur. <br/><br/> </p>";
                                break;
                            }
                        case ProjeConstants.GUNCELLE:
                            {
                                subject = "Toplantı Bilgilendirme (Toplantı Güncelleme) : " + toplantiAdi + "da değişiklik yapılmıştır.";
                                baslik = "<u><b>ÇEVRİMİÇİ TOPLANTI GÜNCELLEME</b></u>" +
                                    "</br> Bu E-Posta bilgilendirme amaçlı gönderilmiştir." +
                                    "</br> <p style='color:red;'> Aşağıda ayrıntıları bulunan <b>'" + toplantiAdi + "da değişiklik yapılmıştır. <br/><br/></p>";
                                break;
                            }
                        case ProjeConstants.SIL:
                            {
                                subject = "Toplantı Bilgilendirme (Toplantı İptali) : " + toplantiAdi + " iptal edilmiştir.";
                                baslik = "<u><b>ÇEVRİMİÇİ TOPLANTI İPTALİ!</b></u>" +
                                    "</br> Bu E-Posta bilgilendirme amaçlı gönderilmiştir." +
                                    "</br><p style='color:red;'>Aşağıda ayrıntıları bulunan <b>'" + toplantiAdi + " iptal edilmiştir. <br/><br/></p>";
                                break;
                            }

                        default:
                            break;
                    }
                    if (toplanti != null)
                    {
                        string grupAdi = userto.Split('@')[0];
                        bool grupMu = UtilityHelper.IsGroup("TSKGV", grupAdi);//böyle bir grup var mı
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
                                    MailHelper.EPostaGonder(from, eposta, subject, "</br>" + tabloSB.ToString(), smtpAdresi ?? ProjeConstants.PARAM_SMTP_IP_ADRESI);
                                    //MailHelper.TakvimeEkle(toplanti.UniqueId, from, eposta, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);
                                    EpostaGonderilenlereEkle(eposta);
                                }
                            }
                        }
                        EpostaGonderilenlereEkle(userto);
                    }
                    else
                    {
                        MessageHelper.PublishMessage("Toplantı bilgisi bulunamadı", ProjeConstants.MESAJ_HATA);
                    }
                }
            }
        }
        public static void ToplantidanCikanKatilimcilaraEPostaGonder(Toplanti toplanti, List<int> cikanKatilimciList, List<ToplantiKatilim> oncekiKatilimciListesi)
        {

            string from = "Toplantı Yönetim Sistemi <tys@tskgv.local>";
            string toplantiAdi = toplanti.ToplantiKonusu + " konulu ve " + toplanti.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + " tarihli toplantı";
            string subject = toplantiAdi + "nın katılım listesinden çıkarıldınız.";

            string baslik = "<u><b style='color:darkred;'>TOPLANTI BİLGİLERİ</b></u>" +
                            "</br>Aşağıda ayrıntıları bulunan <b>'" + toplantiAdi + "dan çıkarıldınız.<br/><br/>";

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
                        tabloSB.Append("Sayın " + personel.Adi + " " + personel.Soyadi + ",<br/><br/>");
                        tabloSB = ToplantiTablosunuOlustur(toplanti, baslik);

                        string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL);
                        MailHelper.EPostaGonder(from, userto, subject, "</br>" + tabloSB.ToString(), smtpAdresi ?? ProjeConstants.PARAM_SMTP_IP_ADRESI);
                        
                        ToplantiKatilim onceki = oncekiKatilimciListesi.Single(s => s.KatilimciId == personelId);
                        //önceki bilgi değil kayılımcı ise
                        if (!onceki.Bilgi)
                            MailHelper.TakvimdenSil(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);


                        MailHelper.TakvimdenSil(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);
                        EpostaGonderilenlerList.Remove(userto);
                    }
                }
            }
            else
            {
                MessageHelper.PublishMessage("Toplantı bilgisi bulunamadı", ProjeConstants.MESAJ_HATA);
            }
        }
        public static void ToplantiYetkilisineEPostaGonder(Toplanti toplanti, string islemTipi)
        {

            string from = "Toplantı Yönetim Sistemi <tys@tskgv.local>";
            string baslik = string.Empty;
            string subject = string.Empty;
            string toplantiAdi = toplanti.ToplantiKonusu + " konulu ve " + toplanti.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + " tarihli toplantı";

            switch (islemTipi)
            {
                case ProjeConstants.KAYDET:
                    {
                        subject = "Yeni Toplantı : " + toplantiAdi + "yı oluşturdunuz.";
                        baslik = "<u><b style='color:green;'>YENİ TOPLANTI</b></u>" +
                            "</br>Aşağıda ayrıntıları bulunan <b>'" + toplantiAdi + "yı oluşturdunuz. <br/><br/>";
                        break;
                    }
                case ProjeConstants.GUNCELLE:
                    {
                        subject = "Toplantı Güncelleme : " + toplantiAdi + "da değişiklik yaptınız.";
                        baslik = "<u><b style='color:blue;'>TOPLANTI GÜNCELLEME</b></u>" +
                            "</br>Aşağıda ayrıntıları bulunan <b>'" + toplantiAdi + "da değişiklik yaptınız. <br/><br/>";
                        break;
                    }
                case ProjeConstants.SIL:
                    {
                        subject = "Toplantı İptali : " + toplantiAdi + "yı iptal ettiniz.";
                        baslik = "<u><b style='color:red;'>TOPLANTI İPTALİ!</b></u>" +
                            "</br>Aşağıda ayrıntıları bulunan <b>'" + toplantiAdi + "yı iptal ettiniz. <br/><br/>";
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
                        tabloSB.Append("Sayın " + toplantiYetkilisi.Adi + " " + toplantiYetkilisi.Soyadi + ",<br/><br/>");
                        tabloSB = ToplantiTablosunuOlustur(toplanti, baslik);
                        string body = "</br>" + tabloSB.ToString();
                        string smtpAdresi = UtilityHelper.ParametreDegeriSorgula(ProjeConstants.PARAM_SMTP_ADRESI_LBL);

                        MailHelper.EPostaGonder(from, userto, subject, body, smtpAdresi ?? ProjeConstants.PARAM_SMTP_IP_ADRESI);
                        //MailHelper.TakvimeEkle(toplanti.UniqueId, from, userto, toplantiAdi, toplanti.BaslangicTarihi, toplanti.BitisTarihi, ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger), toplanti.Aciklama, smtpAdresi);
                        EpostaGonderilenlereEkle(userto);
                    }
                }
            }
            else
            {
                MessageHelper.PublishMessage("Toplantı bilgisi bulunamadı", ProjeConstants.MESAJ_HATA);
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
            sb.Append("<th colspan='2' style='text-align: center; background-color: #dddddd'>Toplantı Ayrıntıları</th>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Toplantı Numarası</th>");
            sb.Append("<td>" + toplanti.Id + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Toplantı Konusu</th>");
            sb.Append("<td>" + toplanti.ToplantiKonusu + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Başlangıç Zamanı</th>");
            sb.Append("<td>" + toplanti.BaslangicTarihi.ToString("dd.MM.yyyy HH:mm") + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Bitiş Zamanı</th>");
            sb.Append("<td>" + toplanti.BitisTarihi.ToString("dd.MM.yyyy HH:mm") + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Toplantı Yeri</th>");
            sb.Append("<td>" + ParseToplantiYeri(toplanti.ToplantiYeri, toplanti.ToplantiYeriDiger) + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Toplantı Yetkilisi</th>");
            sb.Append("<td>" + ParseToplantiYetkilisi(toplanti.ToplantiYetkilisi) + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Koordinator</th>");
            sb.Append("<td>" + ParseKoordinator(toplanti.Koordinator) + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>İç Katılımcılar</th>");
            sb.Append("<td>" + ParseKatilimciListesi(toplanti.Id) + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Dış Katılımcılar</th>");
            sb.Append("<td>" + toplanti.DisKatilimcilar + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Bilgi</th>");
            sb.Append("<td>" + ParseBilgiListesi(toplanti.Id) + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Çevrimiçi</th>");
            sb.Append("<td>" + (toplanti.CevrimIci ? "Evet" : "Hayır") + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>İkram Onayı</th>");
            sb.Append("<td>" + (toplanti.IkramOnayi ? "Evet" : "Hayır") + "</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th>Açıklama</th>");
            sb.Append("<td>" + toplanti.Aciklama + "</td>");
            sb.Append("</tr>"); 
            if (toplanti.IkramOnayi)
            {
                sb.Append("<tr>");
                sb.Append("<th>İkram Malzemesi</th>");
                sb.Append("<td>" + toplanti.IkramMalzemesi + "</td>");
                sb.Append("</tr>");
            }

            sb.Append("</table><br/><br/>");
            sb.Append("<p>");
            sb.Append("Toplantı Yönetim Sistemi.</br>" + DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
            sb.Append("</p>");
            sb.Append("<p style='color:gray; font-family: arial;font-size:xx-small;'>TYS &trade; Bilgi Sistem Kısmı </p> ");
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
        #endregion Toplantı
    }
}
