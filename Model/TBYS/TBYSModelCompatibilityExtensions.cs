using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.TBYS
{
    /// <summary>
    /// Eski entity çağrılarını geçici olarak korur; bütün işlemleri Service katmanına yönlendirir.
    /// Yeni kod doğrudan ilgili Service sınıfını kullanmalıdır.
    /// </summary>
    public static class TBYSModelCompatibilityExtensions
    {
        public static T Select<T>(this Tasinmaz item, int id) { return (T)Convert.ChangeType(new TasinmazService().GetInventoryById(id), typeof(T)); }
        public static int Save(this Tasinmaz item) { return new TasinmazService().Save(item); }
        public static bool Update(this Tasinmaz item) { return new TasinmazService().Update(item); }
        public static bool Delete(this Tasinmaz item) { return new TasinmazService().Delete(item); }
        public static List<T> SelectAll<T>(this Tasinmaz item) { return (List<T>)Convert.ChangeType(new TasinmazService().GetInventory(), typeof(List<T>)); }

        public static T Select<T>(this KiraSozlesme item, int id) { return (T)Convert.ChangeType(new KiraSozlesmeService().GetById(id), typeof(T)); }
        public static KiraSozlesme Select(this KiraSozlesme item, int id) { return new KiraSozlesmeService().GetById(id); }
        public static int Save(this KiraSozlesme item) { return new KiraSozlesmeService().Save(item); }
        public static bool Update(this KiraSozlesme item) { return new KiraSozlesmeService().Update(item); }
        public static bool Delete(this KiraSozlesme item) { return new KiraSozlesmeService().Delete(item); }
        public static List<T> SelectAll<T>(this KiraSozlesme item) { return (List<T>)Convert.ChangeType(new KiraSozlesmeService().GetAll(), typeof(List<T>)); }
        public static List<KiraSozlesme> SelectAllAktifSozlesme(this KiraSozlesme item) { return new KiraSozlesmeService().GetAllActive(); }
        public static DataTable SelectKiraciSayisiByBolgeTarih(this KiraSozlesme item, int bolgeId, int ay, int yil) { return new KiraSozlesmeService().GetTenantCountByRegionAndDate(bolgeId, ay, yil); }
        public static DataTable SelectKiraSozlesmeListReturnDT(this KiraSozlesme item, int kiraciId, int aktif, int bolgeId) { return new KiraSozlesmeService().GetListReturnDataTable(kiraciId, aktif, bolgeId); }
        public static List<KiraSozlesme> SelectKiraSozlesmeList(this KiraSozlesme item, int kiraciId, int aktif, int bolgeId) { return new KiraSozlesmeService().GetList(kiraciId, aktif, bolgeId); }
        public static DataTable SelectKiraArtisiGelenSozlesmelerReturnDT(this KiraSozlesme item, int bolgeId, DateTime tarih) { return new KiraSozlesmeService().GetRentIncreaseDue(bolgeId, tarih); }
        public static DataTable SelectGerceklesenKiraArtislariReturnDT(this KiraSozlesme item, int bolgeId) { return new KiraSozlesmeService().GetRealizedRentIncreases(bolgeId); }
        public static bool UpdateByKiraciId(this KiraSozlesme item, int bolgeId, int kiraciId) { return new KiraSozlesmeService().UpdateByKiraciId(bolgeId, kiraciId); }
        public static DataTable SelectSozlesmeListByYilReturnDT(this KiraSozlesme item, int yil) { return new KiraSozlesmeService().GetListByYear(yil); }
        public static DataTable SelectBitenSozlesmeListByYilReturnDT(this KiraSozlesme item, int yil) { return new KiraSozlesmeService().GetCompletedListByYear(yil); }
        public static List<KiraSozlesme> SelectByKiraciAdi(this KiraSozlesme item, string adi) { return new KiraSozlesmeService().GetByKiraciAdi(adi); }
        public static KiraSozlesme SelectAktifSozlesmeByKiraciId(this KiraSozlesme item, int kiraciId) { return new KiraSozlesmeService().GetActiveByKiraciId(kiraciId); }
        public static KiraSozlesme SelectSozlesmeByKiraciId(this KiraSozlesme item, int kiraciId) { return new KiraSozlesmeService().GetByKiraciId(kiraciId); }
        public static List<KiraSozlesme> SelectByKiraciIdReturnList(this KiraSozlesme item, int kiraciId) { return new KiraSozlesmeService().GetAllByKiraciId(kiraciId); }
        public static KiraSozlesme SelectByKiraciIdTarih(this KiraSozlesme item, int kiraciId, DateTime tarih) { return new KiraSozlesmeService().GetByKiraciIdAndDate(kiraciId, tarih); }
        public static KiraSozlesme SelectEnYakinTarihliSozlesmeByKiraciIdTarih(this KiraSozlesme item, int kiraciId, DateTime tarih) { return new KiraSozlesmeService().GetNearestByKiraciIdAndDate(kiraciId, tarih); }
        public static KiraSozlesme SelectBitenSozlesmeByKiraciId(this KiraSozlesme item, int kiraciId) { return new KiraSozlesmeService().GetCompletedByKiraciId(kiraciId); }
        public static bool UpdateAktifDurum(this KiraSozlesme item, string durum, string degistirmeTar, bool aktif) { return new KiraSozlesmeService().UpdateActiveStatus(item, durum, degistirmeTar, aktif); }
        public static List<KiraSozlesme> SelectByTasinmazId(this KiraSozlesme item, int tasinmazId) { return new KiraSozlesmeService().GetByTasinmazId(tasinmazId); }
        public static DataTable SelectBySozlesmeId(this KiraSozlesme item, int sozlesmeId) { return new KiraSozlesmeService().GetAddressById(sozlesmeId); }
        public static DataTable SelectSUMTeminatByBolgeKiralamaAmaciReturnDT(this KiraSozlesme item, int bolgeId, string kiralamaAmaci) { return new KiraSozlesmeService().GetSecurityDepositSummaryByRegionAndPurpose(bolgeId, kiralamaAmaci); }
        public static DataTable SelectKiraciSayisiVeToplamKiraBedeli(this KiraSozlesme item, int bolgeId, int ay, int yil) { return new KiraSozlesmeService().GetTenantCountAndRentTotal(bolgeId, ay, yil); }
        public static KiraSozlesme SelectNext(this KiraSozlesme item) { return new KiraSozlesmeService().GetNext(item.Id, item.DosyaNo); }
        public static KiraSozlesme SelectOncekiKiraSozlesme(this KiraSozlesme item) { return new KiraSozlesmeService().GetPreviousByTenant(item.KiraciId, item.SozBasTar); }
        public static KiraSozlesme SelectPrev(this KiraSozlesme item) { return new KiraSozlesmeService().GetPrevious(item.Id, item.DosyaNo); }
        public static KiraSozlesme SelectMax(this KiraSozlesme item) { return new KiraSozlesmeService().GetMax(); }
        public static KiraSozlesme SelectMin(this KiraSozlesme item) { return new KiraSozlesmeService().GetMin(); }
        public static KiraSozlesme SelectNextBiten(this KiraSozlesme item) { return new KiraSozlesmeService().GetNextCompleted(item.Id, item.DosyaNo); }
        public static KiraSozlesme SelectPrevBiten(this KiraSozlesme item) { return new KiraSozlesmeService().GetPreviousCompleted(item.Id, item.DosyaNo); }
        public static KiraSozlesme SelectMaxBiten(this KiraSozlesme item) { return new KiraSozlesmeService().GetMaxCompleted(); }
        public static KiraSozlesme SelectMinBiten(this KiraSozlesme item) { return new KiraSozlesmeService().GetMinCompleted(); }
        public static DataTable SelectDevirGerekenAktifSozlesmelerReturnDT(this KiraSozlesme item) { return new KiraSozlesmeService().GetTransferRequiredActiveContracts(); }

        public static T Select<T>(this SozlesmeTasinmaz item, int id) { return (T)Convert.ChangeType(new SozlesmeTasinmazService().GetById(id), typeof(T)); }
        public static int Save(this SozlesmeTasinmaz item) { return new SozlesmeTasinmazService().Save(item); }
        public static bool Update(this SozlesmeTasinmaz item) { return new SozlesmeTasinmazService().Update(item); }
        public static bool Delete(this SozlesmeTasinmaz item) { return new SozlesmeTasinmazService().Delete(item); }
        public static bool DeleteBySozlesmeId(this SozlesmeTasinmaz item, int id) { return new SozlesmeTasinmazService().DeleteBySozlesmeId(item, id); }
        public static List<T> SelectAll<T>(this SozlesmeTasinmaz item) { return (List<T>)Convert.ChangeType(new SozlesmeTasinmazService().GetAll(), typeof(List<T>)); }
        public static string SelectBySozlesmeIdReturnJson(this SozlesmeTasinmaz item, int id) { return new SozlesmeTasinmazService().GetBySozlesmeIdReturnJson(id); }
        public static DataTable SelectBySozlesmeIdReturnDataTable(this SozlesmeTasinmaz item, int id) { return new SozlesmeTasinmazService().GetBySozlesmeIdReturnList(id); }
        public static List<SozlesmeTasinmaz> SelectByTasinmazId(this SozlesmeTasinmaz item, int id) { return new SozlesmeTasinmazService().GetByTasinmazId(id); }
        public static List<SozlesmeTasinmaz> SelectBySozlesmeId(this SozlesmeTasinmaz item, int id) { return new SozlesmeTasinmazService().GetBySozlesmeId(id); }
        public static decimal SelectSumMetrekareBySozlesmeId(this SozlesmeTasinmaz item, int id) { return new SozlesmeTasinmazService().GetSumMetrekareBySozlesmeId(id); }
        public static DataTable SelectBySozlesmeIdReturnDT(this SozlesmeTasinmaz item, int id) { return new SozlesmeTasinmazService().GetBySozlesmeIdReturnDT(id); }
        public static List<SozlesmeTasinmaz> SelectBySozlesmeIdTasinmazId(this SozlesmeTasinmaz item, int sozlesmeId, int tasinmazId, int bolumId) { return new SozlesmeTasinmazService().GetBySozlesmeIdTasinmazId(sozlesmeId, tasinmazId, bolumId); }
        public static List<SozlesmeTasinmaz> SelectByBolumId(this SozlesmeTasinmaz item, int id) { return new SozlesmeTasinmazService().GetByBolumId(id); }

        public static T Select<T>(this KiraEkstreAktarma item, int id) { return (T)Convert.ChangeType(new KiraEkstreAktarmaService().GetById(id), typeof(T)); }
        public static int Save(this KiraEkstreAktarma item) { return new KiraEkstreAktarmaService().Save(item); }
        public static bool Update(this KiraEkstreAktarma item) { return new KiraEkstreAktarmaService().Update(item); }
        public static bool Delete(this KiraEkstreAktarma item) { return new KiraEkstreAktarmaService().Delete(item); }
        public static List<T> SelectAll<T>(this KiraEkstreAktarma item) { return (List<T>)Convert.ChangeType(new KiraEkstreAktarmaService().GetAll(), typeof(List<T>)); }
        public static List<KiraEkstreAktarma> SelectKiraciIdByAdi(this KiraEkstreAktarma item, string adi) { return new KiraEkstreAktarmaService().GetByKiraciAdi(adi); }
        public static List<KiraEkstreAktarma> SelectByIdList(this KiraEkstreAktarma item, string ids) { return new KiraEkstreAktarmaService().GetByIdList(ids); }
        public static List<KiraEkstreAktarma> SelectByIslemNo(this KiraEkstreAktarma item, string no) { return new KiraEkstreAktarmaService().GetByIslemNo(no); }
        public static List<KiraEkstreAktarma> SelectByColumns(this KiraEkstreAktarma item, string adi, string soyadi, decimal tutar, DateTime tarih) { return new KiraEkstreAktarmaService().GetByColumns(adi, soyadi, tutar, tarih); }
        public static List<KiraEkstreAktarma> SelectByEkstreIdList(this KiraEkstreAktarma item, string ids, ref int rowCount) { return new KiraEkstreAktarmaService().GetByEkstreIdList(ids, ref rowCount); }
        public static DataTable SelectYuklenenKayit(this KiraEkstreAktarma item, ref int rowCount, bool aktarilanlarHaric, bool kiraTeminatDiger) { return new KiraEkstreAktarmaService().GetUploadedRecords(ref rowCount, aktarilanlarHaric, kiraTeminatDiger); }
        public static DataTable SelectById(this KiraEkstreAktarma item, int id) { return new KiraEkstreAktarmaService().GetByIdDetailed(id); }
        public static DataTable SelectTarihOdemeSebebi(this KiraEkstreAktarma item, DateTime tarih, int odemeSebebiId) { return new KiraEkstreAktarmaService().GetByDateAndPaymentReason(tarih, odemeSebebiId); }
        public static DataTable SelectSumTutarByTarihOdemeSebebi(this KiraEkstreAktarma item, DateTime tarih, int odemeSebebiId) { return new KiraEkstreAktarmaService().GetSumByDateAndPaymentReason(tarih, odemeSebebiId); }

        public static T Select<T>(this Onarim item, int id) { return (T)Convert.ChangeType(new OnarimService().GetById(id), typeof(T)); }
        public static int Save(this Onarim item) { return new OnarimService().Save(item); }
        public static bool Update(this Onarim item) { return new OnarimService().Update(item); }
        public static bool Delete(this Onarim item) { return new OnarimService().Delete(item); }
        public static List<T> SelectAll<T>(this Onarim item) { return (List<T>)Convert.ChangeType(new OnarimService().GetAll(), typeof(List<T>)); }
        public static List<Onarim> SelectByTasinmazId(this Onarim item, int id) { return new OnarimService().GetByTasinmazId(id); }
        public static DataTable SelectAllReturnDataTable(this Onarim item) { return new OnarimService().GetAllForDataTable(); }
        public static List<Onarim> SelectByOnarimId(this Onarim item, int id) { return new OnarimService().GetByOnarimId(id); }
        public static Onarim SelectNext(this Onarim item, int id) { return new OnarimService().GetNext(id); }
        public static Onarim SelectPrev(this Onarim item, int id) { return new OnarimService().GetPrevious(id); }
        public static Onarim SelectMax(this Onarim item) { return new OnarimService().GetMax(); }
        public static List<Onarim> SelectOnarimByTasinmazId(this Onarim item, int id) { return new OnarimService().GetByTasinmazIdWithAddress(id); }
        public static Onarim SelectMin(this Onarim item) { return new OnarimService().GetMin(); }

        public static T Select<T>(this BagimsizBolum item, int id) { return (T)Convert.ChangeType(new BagimsizBolumService().GetById(id), typeof(T)); }
        public static int Save(this BagimsizBolum item) { return new BagimsizBolumService().Save(item); }
        public static bool Update(this BagimsizBolum item) { return new BagimsizBolumService().Update(item); }
        public static bool Delete(this BagimsizBolum item) { return new BagimsizBolumService().Delete(item); }
        public static List<T> SelectAll<T>(this BagimsizBolum item) { return (List<T>)Convert.ChangeType(new BagimsizBolumService().GetAll(), typeof(List<T>)); }
        public static List<BagimsizBolum> SelectByTasinmazId(this BagimsizBolum item, int id) { return new BagimsizBolumService().GetByTasinmazId(id); }
        public static List<BagimsizBolum> SelectByBolumNO(this BagimsizBolum item, string bolumNo) { return new BagimsizBolumService().GetByBolumNo(bolumNo); }
        public static BagimsizBolum SelectByBolumId(this BagimsizBolum item, int id) { return new BagimsizBolumService().GetByBolumId(id); }

        public static T Select<T>(this Bagis item, int id) { return (T)Convert.ChangeType(new BagisService().GetById(id), typeof(T)); }
        public static Bagis Select(this Bagis item, int id) { return new BagisService().GetById(id); }
        public static int Save(this Bagis item) { return new BagisService().Save(item); }
        public static bool Update(this Bagis item) { return new BagisService().Update(item); }
        public static bool Delete(this Bagis item) { return new BagisService().Delete(item); }
        public static List<T> SelectAll<T>(this Bagis item) { return (List<T>)Convert.ChangeType(new BagisService().GetAll(), typeof(List<T>)); }
        public static List<Bagis> SelectByBagisciId(this Bagis item, int id) { return new BagisService().GetByBagisciId(id); }
        public static DataTable SelectByBagisciIdGroupByKullanimSekli(this Bagis item, int id) { return new BagisService().GetByBagisciIdGroupByKullanimSekli(id); }
        public static string SelectByBagisciIdReturnJson(this Bagis item, int id) { return new BagisService().GetByBagisciIdAsJson(id); }
        public static Bagis SelectByTasinmazId(this Bagis item, int id) { return new BagisService().GetByTasinmazId(id); }
        public static string SelectTasinmazByBagisciIdReturnJson(this Bagis item, int id) { return new BagisService().GetTasinmazByBagisciIdAsJson(id); }
        public static DataTable SelectTasinmazByBagisciIdReturnDT(this Bagis item, int id) { return new BagisService().GetTasinmazByBagisciId(id); }
        public static DataTable SelectSatisVsDahilTasinmazByBagisciIdReturnDT(this Bagis item, int id) { return new BagisService().GetSatisVsDahilTasinmazByBagisciId(id); }
        public static decimal SelectSumTahminiRayicByBagisciId(this Bagis item, int id) { return new BagisService().GetSumTahminiRayicByBagisciId(id); }

    }
}
