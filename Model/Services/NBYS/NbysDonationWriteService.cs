using DAO.Ortak;
using DAO.Repositories.NBYS;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public enum NbysDonationWriteResult
    {
        Completed, CompletedWithRetainedGift, NotFound, Forbidden, Expired, AlreadyRefunded,
        NotRefunded, InvalidDate, InvalidSelection, SameDonor, Conflict
    }

    public sealed class NbysDonationWriteService
    {
        private readonly PortalLogContext context;
        private readonly NbysBagisciWriteRepository repository;
        private static readonly string[] RelatedTables = { "NakitBagisHareket_Table", "Armagan_Table", "EkstreAktarma_Table", "DuzenliNakitBagisci_Table" };

        public NbysDonationWriteService(PortalLogContext context)
        {
            this.context = context;
            repository = new NbysBagisciWriteRepository(context);
        }

        public NbysDonationWriteResult Refund(int id, DateTime date, string reason, bool editOnly)
        {
            if (date.Year < 1901) return NbysDonationWriteResult.InvalidDate;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                NakitBagisHareket donation;
                NakitBagisci donor;
                NbysDonationWriteResult loaded = LoadDonation(id, transaction, out donation, out donor);
                if (loaded != NbysDonationWriteResult.Completed) return loaded;
                if (editOnly && !donation.IadeEdildiMi) return NbysDonationWriteResult.NotRefunded;
                if (!editOnly)
                {
                    if (donation.IadeEdildiMi) return NbysDonationWriteResult.AlreadyRefunded;
                    if (DateTime.Today > donation.BagisTarihi.AddYears(2)) return NbysDonationWriteResult.Expired;
                    Armagan gift = First<Armagan>(repository.SelectGift(donation.ArmaganId, transaction));
                    if (gift != null && gift.BagisciId != donor.Id) return NbysDonationWriteResult.Conflict;
                    donation.IadeEdildiMi = true;
                    donation.IadeMiktari = donation.BagisMiktari;
                    donation.IadeEden = context.UserLogin;
                    donation.Aciklama = donation.Aciklama + donor.Adi + " tarafından " + donation.BagisTarihi.ToString("dd.MM.yyyy")
                        + " tarihinde yapılan " + donation.BagisMiktari.ToString("N", CultureInfo.GetCultureInfo(ProjeConstants.CULTUREINFO))
                        + " " + donation.DovizCinsi + " bağış iade edilmiştir.";
                    donation.BagisMiktari = 0;
                    if (gift != null && gift.Durum == ProjeConstants.DURUM_GONDERILMEDI)
                    {
                        gift.BelgeGecersizMi = ProjeConstants.TRUE_INT;
                        gift.GecersizYapan = context.UserLogin;
                        gift.GecersizYapmaTarihi = DateTime.Now;
                        gift.GecersizNBHareketId = donation.Id;
                        gift.Durum = ProjeConstants.DURUM_PARAIADE;
                        gift.ArmaganBagisMiktari = gift.BagisMiktari;
                        gift.IadeMiktari += donation.IadeMiktari;
                        gift.BagisMiktari = 0;
                        gift.Aciklama += donor.Adi + " adlı bağışçıya ait bağış iade edilmiştir. ";
                        Stamp(gift);
                        repository.SaveRefundGift(gift, gift.Id, transaction);
                    }
                }
                else donation.Aciklama += " İade tarihi ve sebebi güncellenmiştir.";
                donation.IadeTarihi = date;
                donation.IadeSebebi = reason;
                Stamp(donation);
                repository.SaveDonation(donation, donation.Id, transaction);
                transaction.Complete();
            }
            return NbysDonationWriteResult.Completed;
        }

        public NbysDonationWriteResult Delete(int id, string reason)
        {
            bool retainedGift = false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                NakitBagisHareket donation;
                NakitBagisci donor;
                NbysDonationWriteResult loaded = LoadDonation(id, transaction, out donation, out donor);
                if (loaded != NbysDonationWriteResult.Completed) return loaded;
                if ((DateTime.Today - donation.BagisTarihi.Date).TotalDays > 240) return NbysDonationWriteResult.Expired;
                Armagan gift = First<Armagan>(repository.SelectGift(donation.ArmaganId, transaction));
                if (gift != null && gift.BagisciId != donor.Id) return NbysDonationWriteResult.Conflict;
                bool deleteGift = gift != null && gift.Durum == ProjeConstants.DURUM_GONDERILMEDI
                    && !repository.HasGiftReferences(gift.Id, donation.Id, transaction);
                retainedGift = gift != null && !deleteGift;
                Archive("NakitBagisHareket_Table", " #BagisHareketId=" + donation.Id + " #BagisciId=" + donor.Id
                    + " #BagisTarihi=" + donation.BagisTarihi + " #BagisMiktari=" + donation.BagisMiktari, reason, transaction);
                foreach (string table in new[] { "EkstreAktarma_Table", "DuzenliNakitBagisci_Table" })
                {
                    DataTable rows = repository.SelectDonationReferences(table, donation.Id, transaction);
                    if (rows == null) continue;
                    foreach (DataRow row in rows.Rows)
                    {
                        int rowId = Convert.ToInt32(row["Id"]);
                        Archive(table, " #KayitId=" + rowId + " #KaldirilanNakitBagisHareketId=" + donation.Id, reason, transaction);
                        repository.ClearDonationReference(table, rowId, transaction);
                    }
                }
                repository.DeleteRow("NakitBagisHareket_Table", donation.Id, transaction);
                if (deleteGift)
                {
                    Archive("Armagan_Table", " #ArmaganId=" + gift.Id + " #BagisciId=" + gift.BagisciId
                        + " #BagisHareketId=" + donation.Id, reason, transaction);
                    repository.DeleteRow("Armagan_Table", gift.Id, transaction);
                }
                bool hasReferences = false;
                foreach (string table in RelatedTables)
                {
                    DataTable rows = repository.SelectDonorRows(table, donor.Id, transaction);
                    if (rows != null && rows.Rows.Count > 0) hasReferences = true;
                }
                if (!hasReferences)
                {
                    Archive("NakitBagisci_Table", " #BagisciId=" + donor.Id + " #Adi=" + donor.Adi + " #TCKimlikNo=" + donor.TCKimlikNo
                        + " #Telefon=" + donor.Telefon1 + " " + donor.Telefon2 + " #Adres=" + donor.Adres, "Bağış silindiğinden", transaction);
                    repository.DeleteRow("NakitBagisci_Table", donor.Id, transaction);
                }
                transaction.Complete();
            }
            return retainedGift ? NbysDonationWriteResult.CompletedWithRetainedGift : NbysDonationWriteResult.Completed;
        }

        public NbysDonationWriteResult Merge(int targetId, IEnumerable<int> sourceIds)
        {
            if (targetId <= 0 || sourceIds == null) return NbysDonationWriteResult.InvalidSelection;
            int[] sources = sourceIds.Distinct().OrderBy(value => value).ToArray();
            if (sources.Length == 0 || sources.Any(value => value <= 0)) return NbysDonationWriteResult.InvalidSelection;
            if (sources.Contains(targetId)) return NbysDonationWriteResult.SameDonor;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                Bolge region = First<Bolge>(repository.SelectUserRegion(context.UserLogin, transaction));
                if (region == null) return NbysDonationWriteResult.Forbidden;
                Dictionary<int, NakitBagisci> donors = new Dictionary<int, NakitBagisci>();
                // Lock all donor rows in the same order before reading any dependent rows.
                foreach (int donorId in sources.Concat(new[] { targetId }).OrderBy(value => value))
                {
                    NakitBagisci donor = First<NakitBagisci>(repository.SelectDonor(donorId, transaction));
                    if (donor == null) return NbysDonationWriteResult.NotFound;
                    if (!CanWrite(donor, region, transaction)) return NbysDonationWriteResult.Forbidden;
                    donors.Add(donorId, donor);
                }
                foreach (int sourceId in sources)
                {
                    // Include invalid gift history so deleting the source does not orphan it.
                    foreach (NakitBagisHareket donation in Map<NakitBagisHareket>(repository.SelectDonorRows("NakitBagisHareket_Table", sourceId, transaction)))
                    {
                        Archive("NakitBagisHareket_Table", " #BağışçıId=" + sourceId + " numaralı bağışçıya ait " + donation.Id
                            + " numaralı bağış " + targetId + " numaralı bağışçıya birleştirilmiştir.", "Bağış Birleştirme", transaction);
                        donation.Aciklama += "(" + sourceId + " tarafından yapılan bağış birleştirildi) ";
                        donation.BagisciId = targetId;
                        Stamp(donation);
                        repository.SaveDonation(donation, donation.Id, transaction);
                    }
                    foreach (Armagan gift in Map<Armagan>(repository.SelectDonorRows("Armagan_Table", sourceId, transaction)))
                    {
                        Archive("Armagan_Table", " #BağışçıId=" + sourceId + " numaralı bağışçıya ait " + gift.Id
                            + " numaralı armağan " + targetId + " numaralı bağışçıya birleştirilmiştir.", "Bağış Birleştirme", transaction);
                        gift.Aciklama += "(" + sourceId + " tarafından yapılan bağış birleştirildi) ";
                        gift.BagisciId = targetId;
                        Stamp(gift);
                        repository.SaveGift(gift, gift.Id, transaction);
                    }
                    foreach (string table in new[] { "EkstreAktarma_Table", "DuzenliNakitBagisci_Table" })
                    {
                        DataTable rows = repository.SelectDonorRows(table, sourceId, transaction);
                        if (rows == null) continue;
                        foreach (DataRow row in rows.Rows)
                        {
                            int rowId = Convert.ToInt32(row["Id"]);
                            Archive(table, " #KayitId=" + rowId + " #OncekiBagisciId=" + sourceId + " #YeniBagisciId=" + targetId,
                                "Bağış Birleştirme", transaction);
                            repository.MoveDonorReference(table, rowId, targetId, transaction);
                        }
                    }
                    NakitBagisci source = donors[sourceId];
                    Archive("NakitBagisci_Table", " #Bağışçı=" + source.Adi + " " + source.Soyadi + " #BağışçıId=" + source.Id
                        + " numaralı bağışçı silindi.", "Bağış Birleştirme", transaction);
                    repository.DeleteRow("NakitBagisci_Table", sourceId, transaction);
                }
                transaction.Complete();
            }
            return NbysDonationWriteResult.Completed;
        }

        private NbysDonationWriteResult LoadDonation(int id, SqlTransactionContext transaction,
            out NakitBagisHareket donation, out NakitBagisci donor)
        {
            donation = null;
            donor = null;
            Bolge region = First<Bolge>(repository.SelectUserRegion(context.UserLogin, transaction));
            if (region == null) return NbysDonationWriteResult.Forbidden;
            DataTable owner = repository.SelectDonationOwner(id, transaction);
            if (owner == null) return NbysDonationWriteResult.NotFound;
            int donorId = Convert.ToInt32(owner.Rows[0]["BagisciId"]);
            donor = First<NakitBagisci>(repository.SelectDonor(donorId, transaction));
            if (donor == null) return NbysDonationWriteResult.NotFound;
            if (!CanWrite(donor, region, transaction)) return NbysDonationWriteResult.Forbidden;
            donation = First<NakitBagisHareket>(repository.SelectDonation(id, transaction));
            if (donation == null) return NbysDonationWriteResult.NotFound;
            if (donation.BagisciId != donorId) return NbysDonationWriteResult.Conflict;
            return NbysDonationWriteResult.Completed;
        }

        private bool CanWrite(NakitBagisci donor, Bolge region, SqlTransactionContext transaction)
        {
            if (region.Id == ProjeConstants.BOLGE_HEPSI_INT || region.Id == ProjeConstants.BOLGE_GENELMUDURLUK_INT) return true;
            DataTable province = repository.SelectProvinceRegion(donor.Ili, transaction);
            return province != null && !province.Rows[0].IsNull("BolgeId") && Convert.ToInt32(province.Rows[0]["BolgeId"]) == region.Id;
        }

        private void Stamp(EntityBase item)
        {
            item.Degistiren = context.UserLogin;
            item.DegistirmeTarihi = DateTime.Now;
        }

        private void Archive(string table, string detail, string reason, SqlTransactionContext transaction)
        {
            SilinenKayit archive = new SilinenKayit
            {
                TabloAdi = table, SilinenKayitBilgisi = detail, SilinmeSebebi = reason,
                Silen = context.UserLogin, SilinmeTarihi = DateTime.Now,
                Olusturan = context.UserLogin, OlusturmaTarihi = DateTime.Now
            };
            repository.SaveArchive(archive, transaction);
        }

        private static List<T> Map<T>(DataTable table) where T : new() { return new NakitBagisci().ToList<T>(table); }
        private static T First<T>(DataTable table) where T : class, new() { return Map<T>(table).FirstOrDefault(); }
    }
}
