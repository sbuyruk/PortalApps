using DAO.Ortak;
using DAO.Repositories.NBYS;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Utility.HelperClasses;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public enum NbysBagisciSaveResult { Saved, NotFound, Forbidden, NameRequired, Changed }

    public sealed class NbysBagisciEditSnapshot
    {
        public NakitBagisci Donor { get; internal set; }
        public string Version { get; internal set; }
    }

    public sealed class NbysBagisciWriteService
    {
        private readonly NbysBagisciWriteRepository repository;
        private readonly PortalLogContext context;

        public NbysBagisciWriteService(PortalLogContext context)
        {
            this.context = context;
            repository = new NbysBagisciWriteRepository(context);
        }

        public Bolge GetUserRegion()
        {
            return Map<Bolge>(repository.SelectUserRegion(context.UserLogin)).FirstOrDefault();
        }

        public bool CanReadDonor(int donorId, Bolge userRegion)
        {
            if (userRegion == null) return false;
            if (userRegion.Id == ProjeConstants.BOLGE_HEPSI_INT || userRegion.Id == ProjeConstants.BOLGE_GENELMUDURLUK_INT) return true;
            Bolge donorRegion = Map<Bolge>(repository.SelectDonorRegion(donorId)).FirstOrDefault();
            return donorRegion != null && donorRegion.Id == userRegion.Id;
        }

        public NbysBagisciSaveResult LoadForEdit(int id, out NbysBagisciEditSnapshot snapshot)
        {
            snapshot = null;
            if (id <= 0) return NbysBagisciSaveResult.NotFound;
            NbysBagisciEditSnapshot loaded;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                Bolge userRegion = Map<Bolge>(repository.SelectUserRegion(context.UserLogin, transaction)).FirstOrDefault();
                if (userRegion == null) return NbysBagisciSaveResult.Forbidden;
                DataTable rows = repository.SelectDonor(id, transaction);
                loaded = Snapshot(rows);
                if (loaded == null) return NbysBagisciSaveResult.NotFound;
                if (!CanWriteProvince(userRegion.Id, loaded.Donor.Ili, transaction)) return NbysBagisciSaveResult.Forbidden;
                transaction.Complete();
            }
            snapshot = loaded;
            return NbysBagisciSaveResult.Saved;
        }

        public NbysBagisciSaveResult Save(NakitBagisci item, string expectedVersion, out NbysBagisciEditSnapshot saved)
        {
            saved = null;
            if (item == null) throw new ArgumentNullException("item", "Bağışçı bilgisi gereklidir.");
            if (string.IsNullOrWhiteSpace(item.Adi)) return NbysBagisciSaveResult.NameRequired;
            NbysBagisciEditSnapshot completed;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                Bolge userRegion = Map<Bolge>(repository.SelectUserRegion(context.UserLogin, transaction)).FirstOrDefault();
                if (userRegion == null) return NbysBagisciSaveResult.Forbidden;
                NbysBagisciEditSnapshot original = item.Id > 0 ? Snapshot(repository.SelectDonor(item.Id, transaction)) : null;
                NakitBagisci previous = original == null ? null : original.Donor;
                if (item.Id > 0 && previous == null) return NbysBagisciSaveResult.NotFound;
                int provinceId = previous == null ? item.Ili : previous.Ili;
                if (!CanWriteProvince(userRegion.Id, provinceId, transaction)) return NbysBagisciSaveResult.Forbidden;
                // Reject stale forms before changing the donor, recalculating gifts or writing audit records.
                if (previous != null && (string.IsNullOrEmpty(expectedVersion) || original.Version != expectedVersion))
                    return NbysBagisciSaveResult.Changed;
                string user = UtilityHelper.GetCurrentUserName();
                if (previous == null)
                {
                    item.Olusturan = user;
                    item.OlusturmaTarihi = DateTime.Now;
                }
                else
                {
                    // The form does not edit surname; retain the value locked in this transaction.
                    item.Soyadi = previous.Soyadi;
                    item.Olusturan = previous.Olusturan;
                    item.OlusturmaTarihi = previous.OlusturmaTarihi;
                    item.Degistiren = user;
                    item.DegistirmeTarihi = DateTime.Now;
                }
                item.Id = repository.SaveDonor(item, item.Id, transaction);
                if (previous != null && previous.TuzelKisi != item.TuzelKisi)
                    RecalculatePendingGifts(item, user, transaction);
                completed = Snapshot(repository.SelectDonor(item.Id, transaction));
                if (completed == null) throw new InvalidOperationException("Kaydedilen bağışçı bilgileri okunamadı.");
                transaction.Complete();
            } // Commit completes in Dispose, before the UI receives Saved.
            saved = completed;
            return NbysBagisciSaveResult.Saved;
        }

        private bool CanWriteProvince(int userRegionId, int provinceId, SqlTransactionContext transaction)
        {
            if (userRegionId == ProjeConstants.BOLGE_HEPSI_INT || userRegionId == ProjeConstants.BOLGE_GENELMUDURLUK_INT)
                return true;
            DataTable region = repository.SelectProvinceRegion(provinceId, transaction);
            return userRegionId > 0 && region != null && region.Rows.Count > 0 && !region.Rows[0].IsNull("BolgeId")
                && Convert.ToInt32(region.Rows[0]["BolgeId"]) == userRegionId;
        }

        private static NbysBagisciEditSnapshot Snapshot(DataTable rows)
        {
            if (rows == null || rows.Rows.Count == 0) return null;
            // Hash the same raw row used to fill the form, including ownership and audit fields.
            var values = rows.Columns.Cast<DataColumn>().OrderBy(c => c.ColumnName, StringComparer.Ordinal)
                .Select(c => new { Name = c.ColumnName, Type = c.DataType.FullName, Value = VersionValue(rows.Rows[0][c]) }).ToArray();
            string json = new JavaScriptSerializer().Serialize(values);
            string version;
            using (SHA256 hash = SHA256.Create()) version = Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(json)));
            return new NbysBagisciEditSnapshot { Donor = Map<NakitBagisci>(rows).FirstOrDefault(), Version = version };
        }

        private static string VersionValue(object value)
        {
            if (value == DBNull.Value) return null;
            if (value is DateTime) return ((DateTime)value).ToString("o", CultureInfo.InvariantCulture);
            if (value is byte[]) return Convert.ToBase64String((byte[])value);
            if (value is double) return ((double)value).ToString("R", CultureInfo.InvariantCulture);
            if (value is float) return ((float)value).ToString("R", CultureInfo.InvariantCulture);
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private void RecalculatePendingGifts(NakitBagisci donor, string user, SqlTransactionContext transaction)
        {
            if (donor.Adi.Contains(ProjeConstants.NAKITBAGISCI_BILINMEYEN)) return;
            List<Armagan> pending = Map<Armagan>(repository.SelectGifts(donor.Id, ProjeConstants.DURUM_GONDERILMEDI, transaction));
            // Keep the existing monthly entitlement rule, including its inclusive midnight end.
            foreach (Armagan pendingGift in pending)
            {
                DateTime start = new DateTime(pendingGift.Tarih.Year, pendingGift.Tarih.Month, 1);
                DateTime end = start.AddMonths(1).AddDays(-1);
                List<NakitBagisHareket> donations = Map<NakitBagisHareket>(repository.SelectPeriodDonations(donor.Id, start, end, transaction));
                decimal amount = donations.Sum(donation => donation.BagisMiktari);
                ArmaganTanim definition = Map<ArmaganTanim>(repository.SelectGiftDefinition(amount, donor.TuzelKisi, transaction)).FirstOrDefault();
                // As in the existing rule, no entitlement leaves the period gift unchanged.
                if (definition == null || definition.Id == ProjeConstants.ARMAGAN_YOKID) continue;
                Armagan gift = Map<Armagan>(repository.SelectPeriodGifts(donor.Id, start, end, null, transaction)).FirstOrDefault()
                    ?? new Armagan();
                gift.CokluBagis = false;
                gift.BagisciId = donor.Id;
                gift.BagisMiktari = amount;
                gift.DovizCinsi = ProjeConstants.DOVIZ_TL;
                gift.Tarih = pendingGift.Tarih;
                gift.ArmaganTanimId = definition.Id;
                gift.Durum = donor.BelgeIstemiyor ? ProjeConstants.DURUM_BELGE_ISTEMIYOR : ProjeConstants.DURUM_GONDERILMEDI;
                if (definition.Id == ProjeConstants.ARMAGAN_TESEKKURID && donations.Count == 1
                    && donations[0].BankaId == ProjeConstants.BANKA_EDEVLETBAGIS_INT)
                    gift.Durum = ProjeConstants.DURUM_EDEVLETTENBELGEGONDERILDI;
                if (Map<Armagan>(repository.SelectGifts(donor.Id, ProjeConstants.DURUM_PARAIADE, transaction)).Count > 0)
                    gift.Durum = ProjeConstants.DURUM_DAHAONCEIADE;
                if (definition.Id != ProjeConstants.ARMAGAN_TESEKKURID && Map<Armagan>(repository.SelectPeriodGifts(
                    donor.Id, new DateTime(2005, 1, 1), start, definition.Id, transaction)).Count > 0)
                    gift.ArmaganTanimId = ProjeConstants.ARMAGAN_TESEKKURID;
                // Preserve GetDonorStatus precedence from the existing SaveOrUpdate path.
                if (donor.BelgeIstemiyor) gift.Durum = ProjeConstants.DURUM_BELGE_ISTEMIYOR;
                else if (donor.Ulasilamiyor) gift.Durum = ProjeConstants.DURUM_ULASILAMADI;
                if (gift.Id <= 0)
                {
                    gift.Olusturan = user;
                    gift.OlusturmaTarihi = DateTime.Now;
                }
                else
                {
                    gift.Degistiren = user;
                    gift.DegistirmeTarihi = DateTime.Now;
                }
                gift.Id = repository.SaveGift(gift, gift.Id, transaction);
                foreach (NakitBagisHareket donation in donations)
                    repository.LinkDonation(donation.Id, gift.Id, user, DateTime.Now, transaction);
            }
        }

        private static List<T> Map<T>(DataTable table) where T : new()
        {
            return new NakitBagisci().ToList<T>(table);
        }
    }
}
