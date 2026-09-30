using Model.Ortak;
using Model.Services.NBYS;
using System;
using System.Collections.Generic;
using System.IO;
using Utility.HelperClasses;

namespace Model.NBYS
{
    [Serializable]
    public class EkstreAktarma : ParentClass
    {
        public long TCKimlikNo { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Ili { get; set; }
        public string Ilcesi { get; set; }
        public string Adres { get; set; }
        public string Aciklama { get; set; }
        public DateTime BagisTarihi { get; set; }
        public string BankaAdi { get; set; }
        public string DovizCinsi { get; set; }
        public decimal Tutar { get; set; }
        public decimal DovizTutari { get; set; }
        public decimal DovizKuru { get; set; }
        public DateTime KurTarihi { get; set; }
        public string DosyaYolu { get; set; }
        public bool AktarildiMi { get; set; }
        public string Eposta { get; set; }
        public string PostaKodu { get; set; }
        public DateTime IslemTarihi { get; set; }
        public bool TuzelKisi { get; set; }
        public bool ElleKayit { get; set; }
        public int NakitBagisHareketId { get; set; }
        public int NakitBagisciId { get; set; }
        public bool BelgeIstemiyor { get; set; }
        public string FisNo { get; set; }
        public string BagisTipi { get; set; }
        public override int Save()
        {
            return new EkstreAktarmaService().Save(this);
        }
        public override bool Update()
        {
            return new EkstreAktarmaService().Update(this);
        }
        public override bool Delete()
        {
            return new EkstreAktarmaService().Delete(this);
        }
        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new EkstreAktarmaService().GetById(id), typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            List<EkstreAktarma> list = new EkstreAktarmaService().GetAll();
            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<EkstreAktarma> SelectByFisNoBanka(string bankaLike, string fisNo)
        {
            return new EkstreAktarmaService().GetByBankAndReceipt(bankaLike, fisNo);
        }
        public static ExceptionHelper SaveAkBankFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarmaImportService.SaveAkBankFile(fileStream, processTime, currentUser);
        }
        public static ExceptionHelper SaveAkBankEkstreFile(Stream fileStream, DateTime islemTarihi, string currentUser)
        {
            return EkstreAktarmaImportService.SaveAkBankEkstreFile(fileStream, islemTarihi, currentUser);
        }
        public static ExceptionHelper SaveGarantiBankFile(Stream filePath, DateTime processTime, string currentUser)
        {
            return EkstreAktarmaImportService.SaveGarantiBankFile(filePath, processTime, currentUser);
        }
        public static ExceptionHelper SaveGarantiEkstreFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarmaImportService.SaveGarantiEkstreFile(fileStream, processTime, currentUser);
        }
        public static ExceptionHelper SaveHalkbankFile(Stream filePath, DateTime processTime, string currentUser)
        {
            return EkstreAktarmaImportService.SaveHalkbankFile(filePath, processTime, currentUser);
        }
        public static ExceptionHelper SaveHalkbank2File(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarmaImportService.SaveHalkbank2File(fileStream, processTime, currentUser);
        }
        public static ExceptionHelper SaveFinansbankFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarmaImportService.SaveFinansbankFile(fileStream, processTime, currentUser);
        }
        public static ExceptionHelper SaveFinansbankEkstreFile(Stream fileStream, DateTime islemTarihi, string currentUser)
        {
            return EkstreAktarmaImportService.SaveFinansbankEkstreFile(fileStream, islemTarihi, currentUser);
        }
        public static ExceptionHelper SaveIsBankFile(Stream filePath, DateTime processTime, string currentUser)
        {
            return EkstreAktarmaImportService.SaveIsBankFile(filePath, processTime, currentUser);
        }
        public static ExceptionHelper SaveIsBankEkstreFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarmaImportService.SaveIsBankEkstreFile(fileStream, processTime, currentUser);
        }
        public static ExceptionHelper SaveYKBEkstreFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarmaImportService.SaveYKBEkstreFile(fileStream, processTime, currentUser);
        }
        public static ExceptionHelper SaveVakifBankFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarmaImportService.SaveVakifBankFile(fileStream, processTime, currentUser);
        }
        public static ExceptionHelper SaveVakifBankGunlukTextFile(Stream filePath, DateTime processTime, string currentUser)
        {
            return EkstreAktarmaImportService.SaveVakifBankGunlukTextFile(filePath, processTime, currentUser);
        }
        public static ExceptionHelper SaveVakifBank2File(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarmaImportService.SaveVakifBank2File(fileStream, processTime, currentUser);
        }
        public static ExceptionHelper SaveVakifKatilimFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarmaImportService.SaveVakifKatilimFile(fileStream, processTime, currentUser);
        }
        public static ExceptionHelper SaveTEBBankFile(Stream filePath, DateTime processTime, string currentUser)
        {
            return EkstreAktarmaImportService.SaveTEBBankFile(filePath, processTime, currentUser);
        }
        public static ExceptionHelper SaveZiraatFile(Stream fileStream, DateTime islemTarihi, string currentUser)
        {
            return EkstreAktarmaImportService.SaveZiraatFile(fileStream, islemTarihi, currentUser);
        }
        public static ExceptionHelper SaveKartIleFile(Stream fileStream, DateTime islemTarihi, string currentUser, DateTime bagisTarihi)
        {
            return EkstreAktarmaImportService.SaveKartIleFile(fileStream, islemTarihi, currentUser, bagisTarihi);
        }
        public static ExceptionHelper SaveZiraatMT940File(Stream filePath, DateTime processTime, string currentUser)
        {
            return EkstreAktarmaImportService.SaveZiraatMT940File(filePath, processTime, currentUser);
        }
        public static ExceptionHelper SaveEDevletFile(Stream fileStream, DateTime islemTarihi, string currentUser)
        {
            return EkstreAktarmaImportService.SaveEDevletFile(fileStream, islemTarihi, currentUser);
        }
        public static ExceptionHelper SaveZiraatEkstreFile(Stream fileStream, DateTime islemTarihi, string currentUser)
        {
            return EkstreAktarmaImportService.SaveZiraatEkstreFile(fileStream, islemTarihi, currentUser);
        }
        public static ExceptionHelper SaveZiraatKatilimFile(Stream fileStream, DateTime processTime, string currentUser)
        {
            return EkstreAktarmaImportService.SaveZiraatKatilimFile(fileStream, processTime, currentUser);
        }
        public static ExceptionHelper SaveSMSVakifFile(Stream fileStream, DateTime islemTarihi, string currentUser)
        {
            return EkstreAktarmaImportService.SaveSMSVakifFile(fileStream, islemTarihi, currentUser);
        }
        public static ExceptionHelper SaveAlbarakaFile(Stream fileStream, DateTime islemTarihi, string currentUser)
        {
            return EkstreAktarmaImportService.SaveAlbarakaFile(fileStream, islemTarihi, currentUser);
        }
        public static ExceptionHelper SaveAll(List<int> idlist, string currentUser)
        {
            return EkstreAktarmaTransferService.SaveAll(idlist, currentUser);
        }
        public static ExceptionHelper SaveAll(List<EkstreAktarma> ekstrelist, string currentUser)
        {
            return EkstreAktarmaTransferService.SaveAll(ekstrelist, currentUser);
        }
        public static bool SaveArmagan(DateTime bagisTarihi, bool tuzelKisiMi, int nakitBagisciId, int nakitbagisHareketId, string currentUser)
        {
            return EkstreAktarmaTransferService.SaveArmagan(bagisTarihi, tuzelKisiMi, nakitBagisciId, nakitbagisHareketId, currentUser);
        }
        public static int ArmaganiKaydetVeyaGuncelle(DateTime bastar, DateTime bittar, int nakitBagisciId, DateTime bagisTarihi,
            decimal toplamBagis, int hakedilenArmaganTanimId, string currentUser, NakitBagisci nakitBagisci,
            List<NakitBagisHareket> nakitBagisHareketListesi, bool cokluBagis = false)
        {
            return EkstreAktarmaTransferService.ArmaganiKaydetVeyaGuncelle(bastar, bittar, nakitBagisciId, bagisTarihi,
                toplamBagis, hakedilenArmaganTanimId, currentUser, nakitBagisci, nakitBagisHareketListesi, cokluBagis);
        }
        public static bool SaveArmaganYeni(EkstreAktarma ekstreAktarma, ref NakitBagisHareket nakitBagisHareket, string currentUser)
        {
            return EkstreAktarmaTransferService.SaveArmaganYeni(ekstreAktarma, ref nakitBagisHareket, currentUser);
        }
        public static int ArmaganiKaydet(NakitBagisHareket nakitBagisHareket, NakitBagisci nakitBagisci,
            decimal bagisTutari, int hakedilenArmaganTanimId, string currentUser, bool cokluBagis = false)
        {
            return EkstreAktarmaTransferService.ArmaganiKaydet(nakitBagisHareket, nakitBagisci, bagisTutari,
                hakedilenArmaganTanimId, currentUser, cokluBagis);
        }
    }
}
