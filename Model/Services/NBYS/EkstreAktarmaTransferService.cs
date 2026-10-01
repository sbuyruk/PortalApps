using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public static class EkstreAktarmaTransferService
    {
        public static ExceptionHelper SaveAll(List<int> idlist, string currentUser)//List<EkstreAktarma> listEkstreAktarma, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();


            foreach (int itemId in idlist)//EkstreAktarma ekstreAktarma in listEkstreAktarma)
            {

                try
                {
                    EkstreAktarma ekstreAktarma = new EkstreAktarmaService().GetById(itemId);
                    if (ekstreAktarma!=null && ekstreAktarma.AktarildiMi==false)
                        SaveEkstreAktarma(ekstreAktarma, currentUser);
                }
                catch (Exception e)
                {
                    exceptionHelper.Exceptions.Add(e);
                }
                //break;

            }

            return exceptionHelper;
        }
        public static ExceptionHelper SaveAll(List<EkstreAktarma> ekstrelist, string currentUser)//List<EkstreAktarma> listEkstreAktarma, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();


            foreach (EkstreAktarma ekstreAktarma in ekstrelist)//EkstreAktarma ekstreAktarma in listEkstreAktarma)
            {

                try
                {
                    if (ekstreAktarma != null && ekstreAktarma.AktarildiMi == false)
                        SaveEkstreAktarma(ekstreAktarma, currentUser);
                }
                catch (Exception e)
                {
                    exceptionHelper.Exceptions.Add(e);
                }
                //break;

            }

            return exceptionHelper;
        }
        private static void WriteText(string text)
        {
            using (StreamWriter sw = new StreamWriter("C:\\test\\debug.txt", true))
            {
                sw.WriteLine(text);
            }
        }
        private static void SaveEkstreAktarma(EkstreAktarma ekstreAktarma, string currentUser)
        {
            //WriteText("------------------- START ------------------------" +ekstreAktarma);
            //WriteText("SaveEkstreAktarma-0 : "+DateTime.Now);

            if (!ekstreAktarma.AktarildiMi) //zaten aktarilmis olanlar bir kez daha aktarilmasin
            {
                var nakitBagisciId = 0;

                if (ekstreAktarma.NakitBagisciId > 0)//NakitBagisciId si dolu olarak gelen ekstreAktarma (Yani elle yeni kayit girisi yapilmis ve NakitBagisci popup window dan secilmis )
                {
                    nakitBagisciId = ekstreAktarma.NakitBagisciId;
                }
                else
                {
                    nakitBagisciId = SaveNakitBagisciFromEkstre(ekstreAktarma, currentUser); // NakitBagisci_Table tablosuna aktarim
                    //WriteText("SaveEkstreAktarma-1 : " + DateTime.Now);
                }

                var nakitBagisHareket = SaveBagisFromEkstreAktarma(ekstreAktarma, nakitBagisciId, currentUser); //NakitBagisHareket_Table tablosuna aktarim
                //WriteText("SaveEkstreAktarma-2 : " + DateTime.Now);

                //bool isArmaganSaved = SaveArmagan(ekstreAktarma.BagisTarihi, ekstreAktarma.TuzelKisi, nakitBagisciId, nakitBagisHareketId, currentUser);//Armagan tablosuna aktarim
                bool isArmaganSaved = SaveArmaganYeni(ekstreAktarma, ref nakitBagisHareket, currentUser);//Armagan tablosuna aktarim

                //WriteText("SaveEkstreAktarma-3 : " + DateTime.Now);

                if (nakitBagisHareket.Id > 0)
                {
                    NakitBagisci nakitBagisci = new NakitBagisciService().GetById(nakitBagisciId);
                    if (nakitBagisci.Adi.Contains(ProjeConstants.NAKITBAGISCI_BILINMEYEN))// adi bilinmeyen bagisci için eksrteAktarma tablosuna aciklama yaz 
                    {
                        ekstreAktarma.Aciklama += "-NBYS- Adı BİLİNMEYEN bağışçı olduğundan armağan oluşturulmadı ";
                    }

                    ekstreAktarma.NakitBagisHareketId = nakitBagisHareket.Id;
                    ekstreAktarma.NakitBagisciId = nakitBagisciId;

                    ekstreAktarma.AktarildiMi = true;
                    new EkstreAktarmaService().Update(ekstreAktarma);
                }
            }
        }

        public static bool SaveArmagan(DateTime bagisTarihi, bool tuzelKisiMi, int nakitBagisciId, int nakitbagisHareketId, string currentUser)
        {
            ////WriteText("SaveArmagan-0");
            bool isArmaganSaved = false;
            NakitBagisci nakitBagisci = new NakitBagisciService().GetById(nakitBagisciId);
            if ((nakitBagisci == null) || (nakitBagisci.Adi.Contains(ProjeConstants.NAKITBAGISCI_BILINMEYEN)))//nakit bagisçi nuul veya bilinmeyen ise armagan üretmesin
            {
                isArmaganSaved = false;
            }
            else
            {
                NakitBagisHareket nakitBagisHareket = new NakitBagisHareket();
                /// SB Ay bazinda degil de ayi iki parçaya bölerek armagan gönderme uygulamasina geçildi, 17 Nisan 2019. 
                /// Bu nedenle asagidaki satir kapatildi
                /// decimal toplamBagis = nakitBagisHareket.GetSumBagisMiktariByNakitBagisciIdAndBagisTarihi(bagisTarihi, nakitBagisciId);
                /// yerine baslama bitis tarihi alan metot yazildi

                //ARMAGAN PERIODU
                DateTime bastar = new DateTime();
                DateTime bittar = new DateTime();
                int bagisGunu = bagisTarihi.Day;

                //10 Günde bir
                //if (bagisGunu < 11)
                //{
                //    basTar = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 1);
                //    bitTar = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 10);
                //}else if (bagisGunu > 10 && bagisGunu < 21)
                //{
                //    basTar = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 11);
                //    bitTar = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 20);
                //}
                //else
                //{
                //    basTar = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 21);
                //    DateTime ilkGun = new DateTime(basTar.Year, basTar.Month, 1);
                //    bitTar = ilkGun.AddMonths(1).AddDays(-1);
                //}

                //15 Günde bir
                //if (bagisGunu < 16)
                //{
                //    bastar = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 1);
                //    bittar = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 15);
                //}
                //else
                //{
                //    bastar = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 16);
                //    DateTime ilkGun = new DateTime(bastar.Year, bastar.Month, 1);
                //    bittar = ilkGun.AddMonths(1).AddDays(-1);
                //}
                
                //Ayda bir
                bastar = new DateTime(bagisTarihi.Year, bagisTarihi.Month, 1);
                bittar = bastar.AddMonths(1).AddDays(-1);
                
                decimal toplamBagis = new NakitBagisHareketService().GetTotalByBagisciIdDateRange(bastar, bittar, nakitBagisciId);

                ArmaganTanim hakedilenArmaganTanim = new ArmaganTanimService().GetByAmount(toplamBagis, tuzelKisiMi); //toplam tutar gidecek

                if (hakedilenArmaganTanim != null && hakedilenArmaganTanim.Id != ProjeConstants.ARMAGAN_YOKID)
                {
                    //Armagan armagan = new Armagan();
                    ////bagis tarihi ve BagisciId ye göre armagan tablosunu sorgula, bu kisi varsa update yoksa insert etmek için
                    //armagan = armagan.SelectByBagisciIdAndBagisTarihi(bastar, bittar, nakitBagisciId);
                    //if (armagan == null)
                    //    armagan = new Armagan();

                    //armagan.BagisciId = nakitBagisciId;
                    ////armagan.BagisId = nakitBagisHareketId;
                    //armagan.BagisMiktari = toplamBagis;
                    //armagan.DovizCinsi = ProjeConstants.DOVIZ_TL;
                    //armagan.Tarih = bagisTarihi;
                    //armagan.ArmaganTanimId = hakedilenArmaganTanim.Id;
                    //armagan.Olusturan = currentUser;
                    //armagan.Durum = nakitBagisci.BelgeIstemiyor ? ProjeConstants.DURUM_BELGE_ISTEMIYOR : ProjeConstants.DURUM_GONDERILMEDI;

                    //Armagan iadeEdilmisArmagan = new Armagan();
                    //List<Armagan> iadeEdilmisArmaganListesi = iadeEdilmisArmagan.SelectByBagisciIdAndDurum(nakitBagisci.Id, ProjeConstants.DURUM_IADE);
                    //if (iadeEdilmisArmaganListesi.Count > 0)
                    //{
                    //    armagan.Durum = ProjeConstants.DURUM_DAHAONCEIADE;
                    //}

                    //if (hakedilenArmaganTanim.Id != ProjeConstants.ARMAGAN_TESEKKURID) //yeni hakkettigi armagan tesekkür ise eski armagani sorgulama (Olumsuz)
                    //{
                    //    //yeni hakkettigi armagan tesekkür degilise, eski armagani sorgula, yeni armagani daha önce almis mi, evet ise tesekkür ver
                    //    DateTime armaganSorgulamaBasTar = new DateTime(2005, 1, 1);
                    //    DateTime armaganSorgulamaBitTar = bastar;
                    //    Armagan eskiArmagan = armagan.SelectByBagisciIdBagisTarihi(armagan.BagisciId, hakedilenArmaganTanim.Id, armaganSorgulamaBasTar, armaganSorgulamaBitTar);
                    //    if (eskiArmagan != null)
                    //        armagan.ArmaganTanimId = ProjeConstants.ARMAGAN_TESEKKURID;

                    //}
                    NakitBagisHareket newNbh = new NakitBagisHareket();
                    List<NakitBagisHareket> newNbhList = new NakitBagisHareketService().GetByBagisciIdTarih(nakitBagisciId, bastar, bittar);
                    int armaganId = ArmaganiKaydetVeyaGuncelle(bastar, bittar, nakitBagisciId, bagisTarihi, toplamBagis, hakedilenArmaganTanim.Id, currentUser, nakitBagisci, newNbhList);
                    isArmaganSaved = armaganId > 0;
                }
                else
                {
                    isArmaganSaved = true;
                }
            }


            return isArmaganSaved;
        }
        public static int ArmaganiKaydetVeyaGuncelle(DateTime bastar, DateTime bittar, int nakitBagisciId, DateTime bagisTarihi, decimal toplamBagis,
            int hakedilenArmaganTanimId, string currentUser, NakitBagisci nakitBagisci, List<NakitBagisHareket> nakitBagisHareketListesi, bool cokluBagis=false)
        {
            int armaganId = 0;
            //bagis tarihi ve BagisciId ye göre armagan tablosunu sorgula, bu kisi varsa update yoksa insert etmek için
            Armagan armagan = new ArmaganService().GetByBagisciIdDateRange(nakitBagisciId, bastar, bittar);
            if (armagan == null)
                armagan = new Armagan();
            armagan.CokluBagis=cokluBagis;
            armagan.BagisciId = nakitBagisciId;
            //armagan.BagisId = nakitBagisHareketId;
            armagan.BagisMiktari = toplamBagis;
            armagan.DovizCinsi = ProjeConstants.DOVIZ_TL;
            armagan.Tarih = bagisTarihi;
            armagan.ArmaganTanimId = hakedilenArmaganTanimId;
            armagan.Olusturan = currentUser;
            armagan.Durum = nakitBagisci.BelgeIstemiyor ? ProjeConstants.DURUM_BELGE_ISTEMIYOR : ProjeConstants.DURUM_GONDERILMEDI;
            if (hakedilenArmaganTanimId == ProjeConstants.ARMAGAN_TESEKKURID && nakitBagisHareketListesi!=null 
                && nakitBagisHareketListesi.Count==1 && nakitBagisHareketListesi[0].BankaId == ProjeConstants.BANKA_EDEVLETBAGIS_INT) 
            {
                armagan.Durum = ProjeConstants.DURUM_EDEVLETTENBELGEGONDERILDI;
            }
            List<Armagan> iadeEdilmisArmaganListesi = new ArmaganService().GetByBagisciIdAndDurum(nakitBagisci.Id, ProjeConstants.DURUM_PARAIADE);
            if (iadeEdilmisArmaganListesi.Count > 0)
            {
                armagan.Durum = ProjeConstants.DURUM_DAHAONCEIADE;
            }

            if (hakedilenArmaganTanimId != ProjeConstants.ARMAGAN_TESEKKURID) //yeni hakkettigi armagan tesekkür ise eski armagani sorgulama (Olumsuz)
            {
                //yeni hakkettigi armagan tesekkür degilise, eski armagani sorgula, yeni armagani daha önce almis mi, evet ise tesekkür ver
                DateTime armaganSorgulamaBasTar = new DateTime(2005, 1, 1);
                DateTime armaganSorgulamaBitTar = bastar;
                Armagan eskiArmagan = new ArmaganService().GetByBagisciIdTanimIdDateRange(armagan.BagisciId, hakedilenArmaganTanimId, armaganSorgulamaBasTar, armaganSorgulamaBitTar);
                if (eskiArmagan != null)
                    armagan.ArmaganTanimId = ProjeConstants.ARMAGAN_TESEKKURID;

            }
            armaganId = new ArmaganService().SaveOrUpdate(
                armagan, bastar, bittar, nakitBagisciId, nakitBagisHareketListesi);
            return armaganId;
        }

        public static bool SaveArmaganYeni(EkstreAktarma ekstreAktarma, ref NakitBagisHareket nakitBagisHareket, string currentUser)
        {
            bool isArmaganSaved = false;
            NakitBagisci nakitBagisci = new NakitBagisciService().GetById(nakitBagisHareket.BagisciId);
            //nakit bagisçi null veya bilinmeyen ise armagan üretmesin
            if ((nakitBagisci == null) || (nakitBagisci.Adi.Contains(ProjeConstants.NAKITBAGISCI_BILINMEYEN)))
            {
                isArmaganSaved = false;
            }
            else
            {
                int armaganId = 0;

                //Su ana kadar aldigi ArmaganTanimId leri bul
                List<Armagan> alinanArmaganListesi = new ArmaganService().GetByBagisciId(nakitBagisci.Id);
                List<int> alinanArmaganTanimIdListesi = new List<int>();
                foreach (Armagan item in alinanArmaganListesi)
                {
                    if (!alinanArmaganTanimIdListesi.Contains(item.ArmaganTanimId))
                        alinanArmaganTanimIdListesi.Add(item.ArmaganTanimId);
                }

                decimal bagisTutari = nakitBagisHareket.BagisMiktari;
                //Geçmisten bugüne kadar olan toplam bagis tutarini bul
                NakitBagisHareket nbh = new NakitBagisHareket();
                decimal toplamBagis = new NakitBagisHareketService().GetTotalByBagisciIdDateRange(
                    ProjeConstants.COKBAGISYAPAN_BASLAMATARIHI.ConvertToDatetime(), nakitBagisHareket.BagisTarihi, nakitBagisci.Id);

                //Geçmisten bugüne kadar olan toplam bagis tutarina göre Çoklu Bagis armagani hakediyor mu
                ArmaganTanim cokluBagisanHakedilenArmaganTanim = new ArmaganTanimService().GetByAmount(toplamBagis, ekstreAktarma.TuzelKisi);


                bool cokluBagistanArmaganHakediyorMu = cokluBagisanHakedilenArmaganTanim != null 
                    && cokluBagisanHakedilenArmaganTanim.Id != ProjeConstants.ARMAGAN_YOKID;

                ArmaganTanim tekliBagistanHakedilenArmaganTanim = new ArmaganTanimService().GetByAmount(bagisTutari, ekstreAktarma.TuzelKisi);
                bool tekliBagisArmaganHakediyorMu = tekliBagistanHakedilenArmaganTanim != null 
                    && tekliBagistanHakedilenArmaganTanim.Id != ProjeConstants.ARMAGAN_YOKID;
                

                //Hem tekli bagistan hem de çoklu bagistan armagan hakediyorsa ve Tekli Bagistan hakedilen armagan ile çoklu bagistan hak edilen armagan ayni ise
                if (cokluBagistanArmaganHakediyorMu && tekliBagisArmaganHakediyorMu
                    && cokluBagisanHakedilenArmaganTanim.Id == tekliBagistanHakedilenArmaganTanim.Id)
                {
                    //daha önce bu armagani aldiysa, armagan verme
                    if (alinanArmaganTanimIdListesi.Contains(tekliBagistanHakedilenArmaganTanim.Id))
                    {
                        isArmaganSaved = false;
                        nakitBagisHareket.Aciklama += "-NBYS- Daha önce "+ tekliBagistanHakedilenArmaganTanim.Armagan+" aldığı için armağan verilmedi ";
                    }
                    else
                    {
                        //Tekli bagistan armagan ver, Çoklu bagistan verme
                        armaganId = ArmaganiKaydet( nakitBagisHareket, nakitBagisci, nakitBagisHareket.BagisMiktari, tekliBagistanHakedilenArmaganTanim.Id, currentUser);
                        isArmaganSaved= armaganId > 0;
                        nakitBagisHareket.Aciklama += "-NBYS- Hem tekli hem çoklu bağıştan "+tekliBagistanHakedilenArmaganTanim.Armagan+ " hakediyor, Tekli bağıştan armağan verildi ";
                    }

                }
                //Çoklu armagan hakediyorsa, tekli armagan hakediyor veya etmiyorsa farketmez, önce çoklu bagis olustursun

                if (!isArmaganSaved && cokluBagistanArmaganHakediyorMu)
                {
                    if (alinanArmaganTanimIdListesi.Contains(cokluBagisanHakedilenArmaganTanim.Id))
                    {
                        isArmaganSaved = false;
                        nakitBagisHareket.Aciklama += "-NBYS- Daha önce " + cokluBagisanHakedilenArmaganTanim.Armagan + " aldığı için armağan oluşturulmadı ";
                    }
                    else
                    {
                        armaganId = ArmaganiKaydet(nakitBagisHareket, nakitBagisci, toplamBagis, cokluBagisanHakedilenArmaganTanim.Id, currentUser, true);
                        isArmaganSaved = armaganId > 0;
                        nakitBagisHareket.Aciklama += "-NBYS-"+ nakitBagisHareket.BagisTarihi.ConvertToDatetimeEmptyIfNull() +
                            " tarihine kadar yaptığı bağışlardan hak ettiği " + cokluBagisanHakedilenArmaganTanim.Armagan + " oluşturuldu.";
                    }
                    
                }
                //Sadece tekli armagan hakediyorsa
                if (!isArmaganSaved && !cokluBagistanArmaganHakediyorMu && tekliBagisArmaganHakediyorMu)
                {
                    if (alinanArmaganTanimIdListesi.Contains(tekliBagistanHakedilenArmaganTanim.Id))
                    {
                        isArmaganSaved = false;
                        nakitBagisHareket.Aciklama += "-NBYS- Daha önce " + tekliBagistanHakedilenArmaganTanim.Armagan + " aldığı için armağan oluşturulmadı ";
                    }
                    else
                    {
                        armaganId = ArmaganiKaydet(nakitBagisHareket, nakitBagisci, nakitBagisHareket.BagisMiktari, tekliBagistanHakedilenArmaganTanim.Id, currentUser);
                        isArmaganSaved = armaganId > 0;
                        nakitBagisHareket.Aciklama += "-NBYS- tekli bağıştan hak ettiği " + tekliBagistanHakedilenArmaganTanim.Armagan + " oluşturuldu.";
                    }
                }
                //Nakit Bagisa bu armaganId'yi kaydes
                if (isArmaganSaved)
                {
                    if (armaganId > 0)
                    {
                        nakitBagisHareket.ArmaganId = armaganId;
                        new NakitBagisHareketService().Update(nakitBagisHareket);
                    }
                }
            }

            return isArmaganSaved;
        }
        public static int ArmaganiKaydet(NakitBagisHareket nakitBagisHareket, NakitBagisci nakitBagisci, decimal bagisTutari,
            int hakedilenArmaganTanimId, string currentUser,  bool cokluBagis=false)
        {
            //Armagan_Table'dan bu bagisciId ve hakedilenArmaganTanimId kaç tane armagan aldğını bul
            int mevcutArmaganSayisi = new ArmaganService().CountByBagisciIdAndTanimId(nakitBagisci.Id, hakedilenArmaganTanimId);

            int armaganId = 0;
            Armagan armagan = new Armagan();

            armagan.CokluBagis=cokluBagis;
            armagan.DuzenliBagis = hakedilenArmaganTanimId==ProjeConstants.ARMAGAN_DUZENLIBAGISCIBELGESIID;
            armagan.KacinciBelge = mevcutArmaganSayisi + 1;
            armagan.BagisciId = nakitBagisci.Id;
            //armagan.BagisId = nakitBagisHareketId;
            armagan.BagisMiktari = bagisTutari;
            armagan.DovizCinsi = ProjeConstants.DOVIZ_TL;
            armagan.Tarih = nakitBagisHareket.BagisTarihi;
            armagan.ArmaganTanimId = hakedilenArmaganTanimId;
            armagan.Olusturan = currentUser;
            armagan.Durum = nakitBagisci.BelgeIstemiyor ? ProjeConstants.DURUM_BELGE_ISTEMIYOR : 
                (nakitBagisci.Ulasilamiyor ? ProjeConstants.DURUM_ULASILAMADI : ProjeConstants.DURUM_GONDERILMEDI);

            if (hakedilenArmaganTanimId == ProjeConstants.ARMAGAN_TESEKKURID 
                && nakitBagisHareket.BankaId == ProjeConstants.BANKA_EDEVLETBAGIS_INT) 
            {
                armagan.Durum = ProjeConstants.DURUM_EDEVLETTENBELGEGONDERILDI;
            }
            List<Armagan> iadeEdilmisArmaganListesi = new ArmaganService().GetByBagisciIdAndDurum(nakitBagisci.Id, ProjeConstants.DURUM_PARAIADE);
            if (iadeEdilmisArmaganListesi.Count > 0)
            {
                armagan.Durum = ProjeConstants.DURUM_DAHAONCEIADE;
            }
            armaganId = new ArmaganService().Save(armagan);

            return armaganId;
        }
        
        private static int SaveNakitBagisciFromEkstre(EkstreAktarma ekstreAktarma, string currentUser)
        {
            var nakitBagisciId = 0;
            NakitBagisciService service = new NakitBagisciService();
            NakitBagisci nakitBagisci = new NakitBagisci();
            if (ekstreAktarma.Adi != ProjeConstants.NAKITBAGISCI_BILINMEYEN)
            {
                nakitBagisci = service.GetByTcKimlikNo(ekstreAktarma.TCKimlikNo); //tc kimlik no bagisci tablosunda var mi kontrol ediliyor.

                if (nakitBagisci != null)//bagisci tablosunda var
                {
                    nakitBagisciId = nakitBagisci.Id;
                    nakitBagisciId = SaveBagisciFromEkstreAktarma(nakitBagisci, ekstreAktarma, false, currentUser);
                }
                else //bagisci tablosunda tc kimlikno ile bulunamadi
                {
                    nakitBagisci = new NakitBagisci();

                    nakitBagisciId = GetUserIdByTelefonAndAd(ekstreAktarma);// telefon ve isime bakiyor

                    if (nakitBagisciId == 0) //bu kisi kesin olarak yeni bagisci (Telefon ve isimden bulunamadi ise)
                    {
                        nakitBagisciId = SaveBagisciFromEkstreAktarma(nakitBagisci, ekstreAktarma, true, currentUser);
                    }
                    else //nakitBagisciId bos veya 0 degil, öyleyse bu bagisçiyi select edelim 22.ocak.2020
                    {
                        nakitBagisci = service.GetById(nakitBagisciId);
                        nakitBagisciId = SaveBagisciFromEkstreAktarma(nakitBagisci, ekstreAktarma, false, currentUser);
                    }
                }
            }
            else
            {
                nakitBagisciId = BilinmeyenBagisciOlusturAtomicId();// GetBilinmeyenBagisciId();
            }
            return nakitBagisciId;
        }
        private static int BilinmeyenBagisciOlusturAtomicId()
        {
            try
            {
                return new NakitBagisciService().CreateBilinmeyenBagisci();
            }
            catch (Exception ex)
            {
                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
                throw;
            }
        }
        //private static int GetBilinmeyenBagisciId()
        //{
        //    int id = 0;
        //    NakitBagisci nb = new NakitBagisci();
        //    nb = nb.SelectByAd(ProjeConstants.NAKITBAGISCI_BILINMEYEN).FirstOrDefault();
        //    if(nb!=null)
        //    {
        //        id = nb.Id;
        //    }
        //    else
        //    {
        //        nb = new NakitBagisci();
        //        nb.Adi = ProjeConstants.NAKITBAGISCI_BILINMEYEN;
        //        id = nb.Save();
        //    }

        //    return id;

        //}
        private static int GetUserIdByTelefonAndAd(EkstreAktarma ekstreAktarma)
        {
            var nakitBagisciId = 0;
            var adi = ekstreAktarma.Adi;

            if (!string.IsNullOrEmpty(adi)) // adi bos ise yeni bagisçi olarak kabul edilecek
            {
                var telefon1 = UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon1.ReturnEmptyIfNull().ToString());
                if (!string.IsNullOrEmpty(telefon1)) //telefon1 bos degil ise ise kontrol ediliyor adi ve telefon1 bulunur ise ayni kisi olarak kabul ediliyor, yeni kayit atilmiyor
                {
                    nakitBagisciId = GetUserId(adi, telefon1);
                    if (nakitBagisciId != 0)
                    {
                        return nakitBagisciId;
                    }
                }
                if (nakitBagisciId < 1)
                {
                    var telefon2 = UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon2.ReturnEmptyIfNull().ToString()); //telefon1 ve adi eslesmedi ise, telefon2 ve adi esleisiyor mu bakiliyor
                    if (!string.IsNullOrEmpty(telefon2))
                    {
                        nakitBagisciId = telefon2.Length > 3 ? GetUserId(adi, telefon2) : 0;
                    }
                }
            }
            return nakitBagisciId;
        }
        private static int GetUserId(string adi, string telefon)
        {
            var nakitBagisciId = 0;
            NakitBagisciService service = new NakitBagisciService();
            NakitBagisci nakitBagisci = null;
            if (telefon.Length > 3)
            {
                nakitBagisci = service.GetByAdAndTelefon(adi, telefon);
                if (nakitBagisci == null)
                {
                    string dokuzlu = telefon.Substring(0, 2);
                    string sifirli = telefon.Substring(0, 1);

                    if (dokuzlu.Equals("90"))
                    {
                        string telefon1 = telefon.Substring(2);
                        nakitBagisci = service.GetByAdAndTelefon(adi, telefon1);
                    }
                    else if (sifirli.Equals("0"))
                    {
                        string telefon1 = telefon.Substring(1);
                        nakitBagisci = service.GetByAdAndTelefon(adi, telefon1);
                    }
                    else
                    {
                        string bosluksuzTelefon = telefon.Replace(" ", "");
                        nakitBagisci = service.GetByAdAndTelefon(adi, bosluksuzTelefon);
                    }
                }
            };
            if (nakitBagisci != null)//bagisci tablosunda var
            {
                nakitBagisciId = nakitBagisci.Id;
            }
            return nakitBagisciId;
        }
        private static int GetIlId(string value)
        {
            //il ziraat bankasi dosyasinda id olarak geliyor. diger bankalarda il adi olarak geliyor.
            int ilId = 0;

            var isInt = Int32.TryParse(value, out ilId);
            if (!isInt)
            {
                Il il = new Il();
                il = il.SelectByIlAdi(value);
                if (il != null)
                {
                    ilId = il.Id;
                }
            }

            return ilId;
        }
        private static int GetIlceId(EkstreAktarma ekstreAktarma)
        {
            //
            int ilceId = 0;

            var isInt = Int32.TryParse(ekstreAktarma.Ilcesi, out ilceId);
            if (isInt)
            {
                Ilce ilce = new Ilce();
                ilce = ilce.SelectByIlceId(ilceId);
                if (ilce != null)
                {
                    ilceId = ilce.Id;
                }
            }

            return ilceId;
        }
        private static NakitBagisHareket SaveBagisFromEkstreAktarma(EkstreAktarma ekstreAktarma, int nakitBagisciId, string currentUser)
        {
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            NakitBagisHareket nakitBagisHareket = new NakitBagisHareket();
            NakitBagisHareket kayitliNakitBagishareket = new NakitBagisHareket();
            kayitliNakitBagishareket = new NakitBagisHareketService().GetByEkstreAktarmaId(ekstreAktarma.Id);
            int nakitBagisHareketId = 0;
            if (kayitliNakitBagishareket==null)
            {
                BankaTanim bankaTanim = new BankaTanimService().GetByName(ekstreAktarma.BankaAdi);

                Ilce ilce = new Ilce();
                ilce = ilce.SelectByIlNameAndIlceName(ekstreAktarma.Ili, ekstreAktarma.Ilcesi);

                
                nakitBagisHareket.BagisciId = nakitBagisciId;
                nakitBagisHareket.BagisMiktari = ekstreAktarma.Tutar;
                nakitBagisHareket.BagisTarihi = ekstreAktarma.BagisTarihi;
                if (bankaTanim != null)
                {
                    nakitBagisHareket.BankaId = bankaTanim.Id;
                }
                nakitBagisHareket.Adresi = ekstreAktarma.Adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                nakitBagisHareket.DovizCinsi = string.IsNullOrEmpty(ekstreAktarma.DovizCinsi) ? ProjeConstants.DOVIZ_TL : ekstreAktarma.DovizCinsi;
                if (!ekstreAktarma.DovizCinsi.Equals(ProjeConstants.DOVIZ_TL))
                {
                    nakitBagisHareket.DovizTutari = ekstreAktarma.DovizTutari;
                    nakitBagisHareket.DovizKuru = ekstreAktarma.DovizKuru;
                    nakitBagisHareket.KurTarihi = ekstreAktarma.KurTarihi;
                }

                nakitBagisHareket.Telefon = UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon1.ReturnEmptyIfNull().ToString());
                nakitBagisHareket.Ili = GetIlId(ekstreAktarma.Ili);
                nakitBagisHareket.Ilcesi = ilce != null ? ilce.Id : 0;
                nakitBagisHareket.Olusturan = currentUser;
                nakitBagisHareket.Aciklama = ekstreAktarma.Aciklama;
                nakitBagisHareket.EkstreAktarmaId = ekstreAktarma.Id;
                nakitBagisHareket.BagisTipi = ekstreAktarma.BagisTipi;
                var id = new NakitBagisHareketService().Save(nakitBagisHareket);
                if (id != 0)
                {

                    nakitBagisHareketId = nakitBagisHareket.Id;
                }
                else
                {
                    //TODO: hata mesaji verilecek
                }

            }
            return nakitBagisHareket;
        }
        private static int SaveBagisciFromEkstreAktarma(NakitBagisci nakitBagisci, EkstreAktarma ekstreAktarma, bool isNew, string currentUser)
        {
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            NakitBagisciService service = new NakitBagisciService();
            if (nakitBagisci == null)
            {
                nakitBagisci = new NakitBagisci();
            }
            if (isNew)
            {
                Ilce ilce = new Ilce();
                ilce = ilce.SelectByIlNameAndIlceName(ekstreAktarma.Ili, ekstreAktarma.Ilcesi);

                nakitBagisci.Adi = ekstreAktarma.Adi.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                nakitBagisci.Adres = ekstreAktarma.Adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                nakitBagisci.Ilcesi = ilce != null ? ilce.Id : GetIlceId(ekstreAktarma);//ekstreAktarma.Ilcesi;
                nakitBagisci.Ili = GetIlId(ekstreAktarma.Ili);//ekstreAktarma.Ili;
                nakitBagisci.TCKimlikNo = (nakitBagisci.TCKimlikNo < 1) && (ekstreAktarma.TCKimlikNo > 0) ? ekstreAktarma.TCKimlikNo : nakitBagisci.TCKimlikNo;
                nakitBagisci.Telefon1 = UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon1.ReturnEmptyIfNull().ToString());
                nakitBagisci.Telefon2 = UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon2.ReturnEmptyIfNull().ToString());
                nakitBagisci.TuzelKisi = ekstreAktarma.TuzelKisi;
                nakitBagisci.Olusturan = currentUser;
                nakitBagisci.Sag = true;
                nakitBagisci.Ulasilamiyor = false; //brasi tamam çünkü bu yeni biri
                nakitBagisci.BelgeIstemiyor = false;
                nakitBagisci.BelgeIstemiyor = ekstreAktarma.BelgeIstemiyor == true ? true : false;
                if (string.IsNullOrEmpty(ekstreAktarma.Telefon1) &&
                    string.IsNullOrEmpty(ekstreAktarma.Telefon2) &&
                    string.IsNullOrEmpty(ekstreAktarma.Adres))
                {
                    nakitBagisci.Ulasilamiyor = true;
                }

                service.Save(nakitBagisci);

            }//eski bagisci
            else
            {
                Ilce ilce = new Ilce();
                ilce = ilce.SelectByIlNameAndIlceName(ekstreAktarma.Ili, ekstreAktarma.Ilcesi);

                nakitBagisci.Adi = string.IsNullOrEmpty(nakitBagisci.Adi) ? ekstreAktarma.Adi.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo) : nakitBagisci.Adi;
                nakitBagisci.Adres = string.IsNullOrEmpty(nakitBagisci.Adres) ? ekstreAktarma.Adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo) : nakitBagisci.Adres;
                nakitBagisci.Ilcesi = nakitBagisci.Ilcesi==0 ? ilce != null ? ilce.Id : GetIlceId(ekstreAktarma) : nakitBagisci.Ilcesi;//ekstreAktarma.Ilcesi;
                nakitBagisci.Ili = nakitBagisci.Ili==0 ? GetIlId(ekstreAktarma.Ili) : nakitBagisci.Ili;//ekstreAktarma.Ili;
                nakitBagisci.TCKimlikNo = nakitBagisci.TCKimlikNo < 1 ? ekstreAktarma.TCKimlikNo : nakitBagisci.TCKimlikNo;
                nakitBagisci.Telefon1 = string.IsNullOrEmpty(nakitBagisci.Telefon1) ? UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon1.ReturnEmptyIfNull().ToString()) : nakitBagisci.Telefon1;
                nakitBagisci.Telefon2 = string.IsNullOrEmpty(nakitBagisci.Telefon2) ? UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon2.ReturnEmptyIfNull().ToString()) : nakitBagisci.Telefon2;
                nakitBagisci.TuzelKisi = ekstreAktarma.TuzelKisi;
                nakitBagisci.Olusturan = currentUser;
                nakitBagisci.Sag = true;
                //nakitBagisci.Ulasilamiyor = false; kapandi, çünkü  SB 02.04.2021 Bagisçinin ulasilamiyor bilgisini bozuyor
                //nakitBagisci.BelgeIstemiyor = false; kapandi, çünkü  SB 02.04.2021 Bagisçinin BelgeIstemiyor bilgisini bozuyor
                nakitBagisci.BelgeIstemiyor = ekstreAktarma.BelgeIstemiyor == true ? true : nakitBagisci.BelgeIstemiyor;
                if (string.IsNullOrEmpty(ekstreAktarma.Telefon1) &&
                    string.IsNullOrEmpty(ekstreAktarma.Telefon2) &&
                    string.IsNullOrEmpty(ekstreAktarma.Adres))
                {
                    nakitBagisci.Ulasilamiyor = true;
                }

                service.Update(nakitBagisci);
            }

            return nakitBagisci.Id;
        }
    }
}
