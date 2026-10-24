using Utility.Logging;
using System.Linq;
using DAO.Ortak;
using Model.NBYS;
using Model.Ortak;
using Model.Services.Ortak;
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
        private enum RowTransferResult { Completed, AlreadyTransferred, NotFound }
        public enum ArmaganTransferResult
        {
            NoGiftRequired,
            AlreadyExists,
            Created,
            TechnicalFailure
        }

        public static ExceptionHelper SaveAll(List<int> ids, string currentUser, PortalLogContext context)
        {
            RequireContext(context);
            ExceptionHelper result = new ExceptionHelper();
            foreach (int id in ids.Distinct().OrderBy(value => value))
            {
                try
                {
                    RowTransferResult outcome = SaveEkstreAktarma(id, currentUser, context);
                    if (outcome == RowTransferResult.Completed) result.SuccessCount++;
                    else if (outcome == RowTransferResult.NotFound) result.SkippedCount++;
                    else result.AlreadyProcessedCount++;
                }
                catch (System.Threading.ThreadAbortException) { throw; }
                catch (Exception error)
                {
                    result.Exceptions.Add(new InvalidOperationException("Ekstre aktarılamadı. Kayıt kimliği: " + id, error));
                    result.ErrorCount++;
                }
            }
            return result;
        }

        public static ExceptionHelper SaveAll(List<EkstreAktarma> items, string currentUser, PortalLogContext context)
        {
            RequireContext(context);
            return SaveAll(items.Where(item => item != null).Select(item => item.Id).ToList(), currentUser, context);
        }

        private static RowTransferResult SaveEkstreAktarma(int id, string currentUser, PortalLogContext context)
        {
            RequireContext(context);
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                EkstreAktarma current = EkstreService(context, transaction).GetById(id, transaction);
                if (current == null) return RowTransferResult.NotFound;
                if (current.AktarildiMi) return RowTransferResult.AlreadyTransferred;
                int donorId = current.NakitBagisciId > 0 ? current.NakitBagisciId
                    : SaveNakitBagisciFromEkstre(current, currentUser, transaction, context);
                NakitBagisci donor = DonorService(context, transaction).GetById(donorId, transaction);
                if (donor == null) throw new InvalidOperationException("Aktarılacak bağışçı bulunamadı.");
                NakitBagisHareket donation = SaveBagisFromEkstreAktarma(current, donorId, currentUser, transaction, context);
                if (donation.Id <= 0) throw new InvalidOperationException("Bağış kaydı oluşturulamadı.");
                if (donation.BagisciId != donorId) throw new InvalidOperationException("Ekstre ve bağışın bağışçı bağlantıları tutarsız.");
                ArmaganTransferResult giftResult = SaveArmaganYeniWithResult(current, ref donation, currentUser, transaction, context);
                if (giftResult == ArmaganTransferResult.TechnicalFailure) throw new InvalidOperationException("Armağan oluşturulamadı.");
                if (donor.Adi.Contains(ProjeConstants.NAKITBAGISCI_BILINMEYEN))
                    current.Aciklama += "-NBYS- Adı BİLİNMEYEN bağışçı olduğundan armağan oluşturulmadı ";
                current.NakitBagisHareketId = donation.Id;
                current.NakitBagisciId = donorId;
                current.AktarildiMi = true;
                if (!EkstreService(context, transaction).Update(current, transaction)) throw new InvalidOperationException("Ekstre kaydı güncellenemedi.");
                transaction.Complete();
            }
            return RowTransferResult.Completed;
        }

        private static ArmaganTransferResult SaveArmaganYeniWithResult(
            EkstreAktarma ekstreAktarma,
            ref NakitBagisHareket nakitBagisHareket,
            string currentUser,
            SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            bool isArmaganSaved = false;
            bool armaganAlreadyExists = false;
            bool armaganTechnicalFailure = false;
            NakitBagisciService nakitBagisciService = DonorService(context, transaction);
            NakitBagisci nakitBagisci = nakitBagisciService.GetById(nakitBagisHareket.BagisciId, transaction);
            //nakit bagisçi null veya bilinmeyen ise armagan üretmesin
            if ((nakitBagisci == null) || (nakitBagisci.Adi.Contains(ProjeConstants.NAKITBAGISCI_BILINMEYEN)))
            {
                isArmaganSaved = false;
            }
            else
            {
                int armaganId = 0;

                //Su ana kadar aldigi ArmaganTanimId leri bul
                List<Armagan> alinanArmaganListesi = GiftService(context, transaction).GetByBagisciId(nakitBagisci.Id, transaction);
                List<int> alinanArmaganTanimIdListesi = new List<int>();
                foreach (Armagan item in alinanArmaganListesi)
                {
                    if (!alinanArmaganTanimIdListesi.Contains(item.ArmaganTanimId))
                        alinanArmaganTanimIdListesi.Add(item.ArmaganTanimId);
                }

                decimal bagisTutari = nakitBagisHareket.BagisMiktari;
                //Geçmisten bugüne kadar olan toplam bagis tutarini bul
                NakitBagisHareket nbh = new NakitBagisHareket();
                NakitBagisHareketService nakitBagisHareketService = DonationService(context, transaction);
                decimal toplamBagis = nakitBagisHareketService.GetTotalByBagisciIdDateRange(
                        ProjeConstants.COKBAGISYAPAN_BASLAMATARIHI.ConvertToDatetime(),
                        nakitBagisHareket.BagisTarihi,
                        nakitBagisci.Id,
                        transaction);

                //Geçmisten bugüne kadar olan toplam bagis tutarina göre Çoklu Bagis armagani hakediyor mu
                ArmaganTanim cokluBagisanHakedilenArmaganTanim = GiftDefinitionService(context, transaction).GetByAmount(toplamBagis, ekstreAktarma.TuzelKisi, transaction);

                bool cokluBagistanArmaganHakediyorMu = cokluBagisanHakedilenArmaganTanim != null 
                    && cokluBagisanHakedilenArmaganTanim.Id != ProjeConstants.ARMAGAN_YOKID;

                ArmaganTanim tekliBagistanHakedilenArmaganTanim = GiftDefinitionService(context, transaction).GetByAmount(bagisTutari, ekstreAktarma.TuzelKisi, transaction);
                bool tekliBagisArmaganHakediyorMu = tekliBagistanHakedilenArmaganTanim != null 
                    && tekliBagistanHakedilenArmaganTanim.Id != ProjeConstants.ARMAGAN_YOKID;

                //Hem tekli bagistan hem de çoklu bagistan armagan hakediyorsa ve Tekli Bagistan hakedilen armagan ile çoklu bagistan hak edilen armagan ayni ise
                if (cokluBagistanArmaganHakediyorMu && tekliBagisArmaganHakediyorMu
                    && cokluBagisanHakedilenArmaganTanim.Id == tekliBagistanHakedilenArmaganTanim.Id)
                {
                    //daha önce bu armagani aldiysa, armagan verme
                    if (alinanArmaganTanimIdListesi.Contains(tekliBagistanHakedilenArmaganTanim.Id))
                    {
                        armaganAlreadyExists = true;
                        nakitBagisHareket.Aciklama += "-NBYS- Daha önce "+ tekliBagistanHakedilenArmaganTanim.Armagan+" aldığı için armağan verilmedi ";
                    }
                    else
                    {
                        //Tekli bagistan armagan ver, Çoklu bagistan verme
                        armaganId = ArmaganiKaydet(nakitBagisHareket, nakitBagisci, nakitBagisHareket.BagisMiktari, tekliBagistanHakedilenArmaganTanim.Id, currentUser, transaction, context, false);
                        isArmaganSaved = armaganId > 0;
                        armaganTechnicalFailure = !isArmaganSaved;
                        nakitBagisHareket.Aciklama += "-NBYS- Hem tekli hem çoklu bağıştan "+tekliBagistanHakedilenArmaganTanim.Armagan+ " hakediyor, Tekli bağıştan armağan verildi ";
                    }

                }
                //Çoklu armagan hakediyorsa, tekli armagan hakediyor veya etmiyorsa farketmez, önce çoklu bagis olustursun

                if (!isArmaganSaved && cokluBagistanArmaganHakediyorMu)
                {
                    if (alinanArmaganTanimIdListesi.Contains(cokluBagisanHakedilenArmaganTanim.Id))
                    {
                        armaganAlreadyExists = true;
                        nakitBagisHareket.Aciklama += "-NBYS- Daha önce " + cokluBagisanHakedilenArmaganTanim.Armagan + " aldığı için armağan oluşturulmadı ";
                    }
                    else
                    {
                        armaganId = ArmaganiKaydet(nakitBagisHareket, nakitBagisci, toplamBagis, cokluBagisanHakedilenArmaganTanim.Id, currentUser, transaction, context, true);
                        isArmaganSaved = armaganId > 0;
                        armaganTechnicalFailure = armaganTechnicalFailure || !isArmaganSaved;
                        nakitBagisHareket.Aciklama += "-NBYS-"+ nakitBagisHareket.BagisTarihi.ConvertToDatetimeEmptyIfNull() +
                            " tarihine kadar yaptığı bağışlardan hak ettiği " + cokluBagisanHakedilenArmaganTanim.Armagan + " oluşturuldu.";
                    }

                }
                //Sadece tekli armagan hakediyorsa
                if (!isArmaganSaved && !cokluBagistanArmaganHakediyorMu && tekliBagisArmaganHakediyorMu)
                {
                    if (alinanArmaganTanimIdListesi.Contains(tekliBagistanHakedilenArmaganTanim.Id))
                    {
                        armaganAlreadyExists = true;
                        nakitBagisHareket.Aciklama += "-NBYS- Daha önce " + tekliBagistanHakedilenArmaganTanim.Armagan + " aldığı için armağan oluşturulmadı ";
                    }
                    else
                    {
                        armaganId = ArmaganiKaydet(nakitBagisHareket, nakitBagisci, nakitBagisHareket.BagisMiktari, tekliBagistanHakedilenArmaganTanim.Id, currentUser, transaction, context, false);
                        isArmaganSaved = armaganId > 0;
                        armaganTechnicalFailure = armaganTechnicalFailure || !isArmaganSaved;
                        nakitBagisHareket.Aciklama += "-NBYS- tekli bağıştan hak ettiği " + tekliBagistanHakedilenArmaganTanim.Armagan + " oluşturuldu.";
                    }
                }
                //Nakit Bagisa bu armaganId'yi kaydes
                if (isArmaganSaved)
                {
                    if (armaganId > 0)
                    {
                        nakitBagisHareket.ArmaganId = armaganId;
                        NakitBagisHareketService service = DonationService(context, transaction);
                        bool hareketGuncellendi = service.Update(nakitBagisHareket, transaction);
                        if (!hareketGuncellendi)
                            throw new InvalidOperationException(
                                "ArmaganId, NakitBagisHareket kaydina baglanamadi.");
                    }
                }
            }

            if (isArmaganSaved)
                return ArmaganTransferResult.Created;
            if (armaganTechnicalFailure)
                return ArmaganTransferResult.TechnicalFailure;
            if (armaganAlreadyExists)
                return ArmaganTransferResult.AlreadyExists;
            return ArmaganTransferResult.NoGiftRequired;
        }
        private static int ArmaganiKaydet(NakitBagisHareket nakitBagisHareket, NakitBagisci nakitBagisci, decimal bagisTutari,
            int hakedilenArmaganTanimId, string currentUser, SqlTransactionContext transaction, PortalLogContext context, bool cokluBagis)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");

            //Armagan_Table'dan bu bagisciId ve hakedilenArmaganTanimId kaç tane armagan aldğını bul
            int mevcutArmaganSayisi = GiftService(context, transaction).CountByBagisciIdAndTanimId(
                        nakitBagisci.Id, hakedilenArmaganTanimId, transaction);

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
            List<Armagan> iadeEdilmisArmaganListesi = GiftService(context, transaction).GetByBagisciIdAndDurum(
                    nakitBagisci.Id, ProjeConstants.DURUM_PARAIADE, transaction);
            if (iadeEdilmisArmaganListesi.Count > 0)
            {
                armagan.Durum = ProjeConstants.DURUM_DAHAONCEIADE;
            }
            ArmaganService service = GiftService(context, transaction);
            armaganId = service.Save(armagan, transaction);

            return armaganId;
        }

        private static int SaveNakitBagisciFromEkstre(
            EkstreAktarma ekstreAktarma,
            string currentUser,
            SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            var nakitBagisciId = 0;
            NakitBagisciService service = DonorService(context, transaction);
            NakitBagisci nakitBagisci = new NakitBagisci();
            if (ekstreAktarma.Adi != ProjeConstants.NAKITBAGISCI_BILINMEYEN)
            {
                nakitBagisci = service.GetByTcKimlikNo(ekstreAktarma.TCKimlikNo, transaction); //tc kimlik no bagisci tablosunda var mi kontrol ediliyor.

                if (nakitBagisci != null)//bagisci tablosunda var
                {
                    nakitBagisciId = nakitBagisci.Id;
                    nakitBagisciId = SaveBagisciFromEkstreAktarma(
                        nakitBagisci, ekstreAktarma, false, currentUser, transaction, context);
                }
                else //bagisci tablosunda tc kimlikno ile bulunamadi
                {
                    nakitBagisci = new NakitBagisci();

                    nakitBagisciId = GetUserIdByTelefonAndAd(ekstreAktarma, transaction, context);// telefon ve isime bakiyor

                    if (nakitBagisciId == 0) //bu kisi kesin olarak yeni bagisci (Telefon ve isimden bulunamadi ise)
                    {
                        nakitBagisciId = SaveBagisciFromEkstreAktarma(
                            nakitBagisci, ekstreAktarma, true, currentUser, transaction, context);
                    }
                    else //nakitBagisciId bos veya 0 degil, öyleyse bu bagisçiyi select edelim 22.ocak.2020
                    {
                        nakitBagisci = service.GetById(nakitBagisciId, transaction);
                        nakitBagisciId = SaveBagisciFromEkstreAktarma(
                            nakitBagisci, ekstreAktarma, false, currentUser, transaction, context);
                    }
                }
            }
            else
            {
                nakitBagisciId = BilinmeyenBagisciOlusturAtomicId(transaction, context);// GetBilinmeyenBagisciId();
            }
            return nakitBagisciId;
        }
        private static int BilinmeyenBagisciOlusturAtomicId(SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            NakitBagisciService service = DonorService(context, transaction);
            return service.CreateBilinmeyenBagisci(transaction);
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
        private static int GetUserIdByTelefonAndAd(
            EkstreAktarma ekstreAktarma,
            SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            var nakitBagisciId = 0;
            var adi = ekstreAktarma.Adi;

            if (!string.IsNullOrEmpty(adi)) // adi bos ise yeni bagisçi olarak kabul edilecek
            {
                var telefon1 = UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon1.ReturnEmptyIfNull().ToString());
                if (!string.IsNullOrEmpty(telefon1)) //telefon1 bos degil ise ise kontrol ediliyor adi ve telefon1 bulunur ise ayni kisi olarak kabul ediliyor, yeni kayit atilmiyor
                {
                    nakitBagisciId = GetUserId(adi, telefon1, transaction, context);
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
                        nakitBagisciId = telefon2.Length > 3 ? GetUserId(adi, telefon2, transaction, context) : 0;
                    }
                }
            }
            return nakitBagisciId;
        }
        private static int GetUserId(
            string adi,
            string telefon,
            SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            var nakitBagisciId = 0;
            NakitBagisciService service = DonorService(context, transaction);
            NakitBagisci nakitBagisci = null;
            if (telefon.Length > 3)
            {
                nakitBagisci = service.GetByAdAndTelefon(adi, telefon, transaction);
                if (nakitBagisci == null)
                {
                    string dokuzlu = telefon.Substring(0, 2);
                    string sifirli = telefon.Substring(0, 1);

                    if (dokuzlu.Equals("90"))
                    {
                        string telefon1 = telefon.Substring(2);
                        nakitBagisci = service.GetByAdAndTelefon(adi, telefon1, transaction);
                    }
                    else if (sifirli.Equals("0"))
                    {
                        string telefon1 = telefon.Substring(1);
                        nakitBagisci = service.GetByAdAndTelefon(adi, telefon1, transaction);
                    }
                    else
                    {
                        string bosluksuzTelefon = telefon.Replace(" ", "");
                        nakitBagisci = service.GetByAdAndTelefon(adi, bosluksuzTelefon, transaction);
                    }
                }
            };
            if (nakitBagisci != null)//bagisci tablosunda var
            {
                nakitBagisciId = nakitBagisci.Id;
            }
            return nakitBagisciId;
        }
        private static int GetIlId(string value, SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            //il ziraat bankasi dosyasinda id olarak geliyor. diger bankalarda il adi olarak geliyor.
            int ilId = 0;

            var isInt = Int32.TryParse(value, out ilId);
            if (!isInt)
            {
                Il il = ProvinceService(context, transaction).GetByName(value, transaction);
                if (il != null)
                {
                    ilId = il.Id;
                }
            }

            return ilId;
        }
        private static int GetIlceId(
            EkstreAktarma ekstreAktarma,
            SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            //
            int ilceId = 0;

            var isInt = Int32.TryParse(ekstreAktarma.Ilcesi, out ilceId);
            if (isInt)
            {
                Ilce ilce = DistrictService(context, transaction).GetById(ilceId, transaction);
                if (ilce != null)
                {
                    ilceId = ilce.Id;
                }
            }

            return ilceId;
        }
        private static NakitBagisHareket SaveBagisFromEkstreAktarma(
            EkstreAktarma ekstreAktarma,
            int nakitBagisciId,
            string currentUser,
            SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            NakitBagisHareket nakitBagisHareket = new NakitBagisHareket();
            NakitBagisHareket kayitliNakitBagishareket = new NakitBagisHareket();
            NakitBagisHareketService service = DonationService(context, transaction);
            kayitliNakitBagishareket = service.GetByEkstreAktarmaId(ekstreAktarma.Id, transaction);
            if (kayitliNakitBagishareket != null)
                return kayitliNakitBagishareket;

            int nakitBagisHareketId = 0;
            if (kayitliNakitBagishareket==null)
            {
                BankaTanim bankaTanim = BankService(context, transaction).GetByName(ekstreAktarma.BankaAdi, transaction);

                Ilce ilce = DistrictService(context, transaction).GetByProvinceAndDistrictName(
                        ekstreAktarma.Ili, ekstreAktarma.Ilcesi, transaction);

                nakitBagisHareket.BagisciId = nakitBagisciId;
                nakitBagisHareket.BagisMiktari = ekstreAktarma.Tutar;
                nakitBagisHareket.BagisTarihi = ekstreAktarma.BagisTarihi;
                if (bankaTanim != null)
                {
                    nakitBagisHareket.BankaId = bankaTanim.Id;
                }
                nakitBagisHareket.Adresi = ekstreAktarma.Adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                string dovizCinsi = string.IsNullOrEmpty(ekstreAktarma.DovizCinsi) ? ProjeConstants.DOVIZ_TL : ekstreAktarma.DovizCinsi;
                nakitBagisHareket.DovizCinsi = dovizCinsi;
                if (!ProjeConstants.DOVIZ_TL.Equals(dovizCinsi))
                {
                    nakitBagisHareket.DovizTutari = ekstreAktarma.DovizTutari;
                    nakitBagisHareket.DovizKuru = ekstreAktarma.DovizKuru;
                    nakitBagisHareket.KurTarihi = ekstreAktarma.KurTarihi;
                }

                nakitBagisHareket.Telefon = UtilityHelper.TelefonFormatla(ekstreAktarma.Telefon1.ReturnEmptyIfNull().ToString());
                nakitBagisHareket.Ili = GetIlId(ekstreAktarma.Ili, transaction, context);
                nakitBagisHareket.Ilcesi = ilce != null ? ilce.Id : 0;
                nakitBagisHareket.Olusturan = currentUser;
                nakitBagisHareket.Aciklama = ekstreAktarma.Aciklama;
                nakitBagisHareket.EkstreAktarmaId = ekstreAktarma.Id;
                nakitBagisHareket.BagisTipi = ekstreAktarma.BagisTipi;
                var id = service.Save(nakitBagisHareket, transaction);
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
        private static int SaveBagisciFromEkstreAktarma(
            NakitBagisci nakitBagisci,
            EkstreAktarma ekstreAktarma,
            bool isNew,
            string currentUser,
            SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            NakitBagisciService service = DonorService(context, transaction);
            if (nakitBagisci == null)
            {
                nakitBagisci = new NakitBagisci();
            }
            if (isNew)
            {
                Ilce ilce = DistrictService(context, transaction).GetByProvinceAndDistrictName(
                        ekstreAktarma.Ili, ekstreAktarma.Ilcesi, transaction);

                nakitBagisci.Adi = ekstreAktarma.Adi.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                nakitBagisci.Adres = ekstreAktarma.Adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo);
                nakitBagisci.Ilcesi = ilce != null ? ilce.Id : GetIlceId(ekstreAktarma, transaction, context);//ekstreAktarma.Ilcesi;
                nakitBagisci.Ili = GetIlId(ekstreAktarma.Ili, transaction, context);//ekstreAktarma.Ili;
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

                service.Save(nakitBagisci, transaction);

            }//eski bagisci
            else
            {
                Ilce ilce = DistrictService(context, transaction).GetByProvinceAndDistrictName(
                        ekstreAktarma.Ili, ekstreAktarma.Ilcesi, transaction);

                nakitBagisci.Adi = string.IsNullOrEmpty(nakitBagisci.Adi) ? ekstreAktarma.Adi.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo) : nakitBagisci.Adi;
                nakitBagisci.Adres = string.IsNullOrEmpty(nakitBagisci.Adres) ? ekstreAktarma.Adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo) : nakitBagisci.Adres;
                nakitBagisci.Ilcesi = nakitBagisci.Ilcesi==0 ? ilce != null ? ilce.Id : GetIlceId(ekstreAktarma, transaction, context) : nakitBagisci.Ilcesi;//ekstreAktarma.Ilcesi;
                nakitBagisci.Ili = nakitBagisci.Ili==0 ? GetIlId(ekstreAktarma.Ili, transaction, context) : nakitBagisci.Ili;//ekstreAktarma.Ili;
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

                service.Update(nakitBagisci, transaction);
            }

            return nakitBagisci.Id;
        }
        private static EkstreAktarmaService EkstreService(PortalLogContext context, SqlTransactionContext transaction)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            RequireContext(context);
            return new EkstreAktarmaService(new DAO.Repositories.NBYS.EkstreAktarmaRepository(context));
        }
        private static NakitBagisciService DonorService(PortalLogContext context, SqlTransactionContext transaction)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            RequireContext(context);
            return new NakitBagisciService(new DAO.Repositories.NBYS.NakitBagisciRepository(context));
        }
        private static NakitBagisHareketService DonationService(PortalLogContext context, SqlTransactionContext transaction)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            RequireContext(context);
            return new NakitBagisHareketService(new DAO.Repositories.NBYS.NakitBagisHareketRepository(context));
        }
        private static ArmaganService GiftService(PortalLogContext context, SqlTransactionContext transaction)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            RequireContext(context);
            return new ArmaganService(new DAO.Repositories.NBYS.ArmaganRepository(context));
        }
        private static ArmaganTanimService GiftDefinitionService(PortalLogContext context, SqlTransactionContext transaction)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            RequireContext(context);
            return new ArmaganTanimService(new DAO.Repositories.NBYS.ArmaganTanimRepository(context));
        }
        private static BankaTanimService BankService(PortalLogContext context, SqlTransactionContext transaction)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            RequireContext(context);
            return new BankaTanimService(new DAO.Repositories.NBYS.BankaTanimRepository(context));
        }
        private static IlService ProvinceService(PortalLogContext context, SqlTransactionContext transaction)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            RequireContext(context);
            return new IlService(new DAO.Repositories.Ortak.IlRepository(context));
        }
        private static IlceService DistrictService(PortalLogContext context, SqlTransactionContext transaction)
        {
            RequireContext(context);
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
            RequireContext(context);
            return new IlceService(new DAO.Repositories.Ortak.IlceRepository(context));
        }
        private static void RequireContext(PortalLogContext context)
        {
            if (context == null || context.Project != "NBYS")
                throw new ArgumentException("NBYS log bağlamı gereklidir.", "context");
        }
    }
}
