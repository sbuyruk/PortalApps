using DAO.Ortak;
using DAO.Repositories.NBYS;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public enum NbysGiftWriteResult { Completed, Unchanged, InvalidSelection, NotFound, Forbidden, InvalidDefinition, InvalidAmount, InvalidDate, InvalidStatus, RefundProtected, NotSent, Conflict, Changed }

    public sealed class NbysGiftEditSnapshot
    {
        public Armagan Gift { get; internal set; }
        public string Version { get; internal set; }
    }

    public sealed class NbysGiftWriteService
    {
        private readonly PortalLogContext context;
        private readonly NbysBagisciWriteRepository records;
        private readonly NbysGiftWriteRepository gifts;

        public NbysGiftWriteService(PortalLogContext context)
        {
            this.context = context;
            records = new NbysBagisciWriteRepository(context);
            gifts = new NbysGiftWriteRepository(context);
        }

        public NbysGiftWriteResult LoadForEdit(int id, out NbysGiftEditSnapshot snapshot)
        {
            snapshot = null;
            if (id <= 0) return NbysGiftWriteResult.InvalidSelection;
            NbysGiftEditSnapshot loaded;
            using (var transaction = new SqlTransactionContext())
            {
                Armagan gift;
                DataTable rows;
                NbysGiftWriteResult access = LockAndAuthorize(id, transaction, out gift, out rows);
                if (access != NbysGiftWriteResult.Completed) return access;
                loaded = Snapshot(rows);
                transaction.Complete();
            }
            snapshot = loaded;
            return NbysGiftWriteResult.Completed;
        }

        public NbysGiftWriteResult Update(Armagan input, string expectedVersion, out NbysGiftEditSnapshot saved)
        {
            saved = null;
            if (input == null || input.Id <= 0) return NbysGiftWriteResult.InvalidSelection;
            if (input.BagisMiktari < 0) return NbysGiftWriteResult.InvalidAmount;
            if (input.Tarih < new DateTime(1753, 1, 1)) return NbysGiftWriteResult.InvalidDate;
            NbysGiftEditSnapshot completed;
            NbysGiftWriteResult result;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                Armagan current;
                DataTable rows;
                NbysGiftWriteResult access = LockAndAuthorize(input.Id, transaction, out current, out rows);
                if (access != NbysGiftWriteResult.Completed) return access;
                NbysGiftEditSnapshot original = Snapshot(rows);
                if (string.IsNullOrEmpty(expectedVersion) || original.Version != expectedVersion)
                    return NbysGiftWriteResult.Changed;
                // A financial refund cannot be created or reversed from a certificate form.
                if (current.Durum != ProjeConstants.DURUM_PARAIADE && input.Durum == ProjeConstants.DURUM_PARAIADE)
                    return NbysGiftWriteResult.RefundProtected;
                string status = current.Durum == ProjeConstants.DURUM_PARAIADE ? current.Durum : input.Durum;
                if (current.Durum != ProjeConstants.DURUM_PARAIADE && status != current.Durum && !IsEditableStatus(status)) return NbysGiftWriteResult.InvalidStatus;
                if (gifts.Definition(input.ArmaganTanimId, transaction) == null) return NbysGiftWriteResult.InvalidDefinition;
                if (current.ArmaganTanimId == input.ArmaganTanimId && current.BagisMiktari == input.BagisMiktari
                    && current.Durum == status && current.Tarih == input.Tarih
                    && (current.Aciklama ?? string.Empty) == (input.Aciklama ?? string.Empty)
                    && (current.BelgedeYazanIsim ?? string.Empty) == (input.BelgedeYazanIsim ?? string.Empty)
                    && current.BagisMiktariYazmasin == input.BagisMiktariYazmasin)
                {
                    completed = original;
                    result = NbysGiftWriteResult.Unchanged;
                }
                else
                {
                    gifts.Update(input.Id, input.ArmaganTanimId, input.BagisMiktari, status, input.Tarih,
                        input.Aciklama, input.BelgedeYazanIsim, input.BagisMiktariYazmasin, context.UserLogin, transaction);
                    completed = Snapshot(records.SelectGift(input.Id, transaction));
                    if (completed == null) throw new InvalidOperationException("Güncellenen armağan kaydı okunamadı.");
                    result = NbysGiftWriteResult.Completed;
                }
                transaction.Complete();
            }
            saved = completed;
            return result;
        }

        public NbysGiftWriteResult MarkCertificateReturned(int id)
        {
            if (id <= 0) return NbysGiftWriteResult.InvalidSelection;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                Armagan current;
                NbysGiftWriteResult access = LockAndAuthorize(id, transaction, out current);
                if (access != NbysGiftWriteResult.Completed) return access;
                if (current.Durum == ProjeConstants.DURUM_PARAIADE) return NbysGiftWriteResult.RefundProtected;
                if (current.Durum == ProjeConstants.DURUM_DAHAONCEIADE) return NbysGiftWriteResult.Unchanged;
                if (current.Durum != ProjeConstants.DURUM_GONDERILDI) return NbysGiftWriteResult.NotSent;
                gifts.SetReturned(id, ProjeConstants.DURUM_DAHAONCEIADE, context.UserLogin, transaction);
                transaction.Complete();
            }
            // Complete only marks the transaction; Dispose must also succeed before reporting success.
            return NbysGiftWriteResult.Completed;
        }

        private NbysGiftWriteResult LockAndAuthorize(int id, SqlTransactionContext transaction, out Armagan gift)
        {
            DataTable rows;
            return LockAndAuthorize(id, transaction, out gift, out rows);
        }

        private NbysGiftWriteResult LockAndAuthorize(int id, SqlTransactionContext transaction, out Armagan gift, out DataTable rows)
        {
            gift = null;
            rows = null;
            DataTable owner = gifts.Owner(id, transaction);
            if (owner == null) return NbysGiftWriteResult.NotFound;
            int donorId = Convert.ToInt32(owner.Rows[0]["BagisciId"]);
            NakitBagisci donor = First<NakitBagisci>(records.SelectDonor(donorId, transaction));
            rows = records.SelectGift(id, transaction);
            gift = First<Armagan>(rows);
            if (gift == null || donor == null) return NbysGiftWriteResult.NotFound;
            if (gift.BagisciId != donorId) return NbysGiftWriteResult.Conflict;
            Bolge region = First<Bolge>(records.SelectUserRegion(context.UserLogin, transaction));
            if (region == null) return NbysGiftWriteResult.Forbidden;
            if (region.Id == ProjeConstants.BOLGE_HEPSI_INT || region.Id == ProjeConstants.BOLGE_GENELMUDURLUK_INT)
                return NbysGiftWriteResult.Completed;
            DataTable province = records.SelectProvinceRegion(donor.Ili, transaction);
            return region.Id > 0 && province != null && !province.Rows[0].IsNull("BolgeId")
                && Convert.ToInt32(province.Rows[0]["BolgeId"]) == region.Id
                ? NbysGiftWriteResult.Completed : NbysGiftWriteResult.Forbidden;
        }

        private static NbysGiftEditSnapshot Snapshot(DataTable rows)
        {
            if (rows == null || rows.Rows.Count == 0) return null;
            // The form and version describe the same raw row, including ownership, refund and audit fields.
            var values = rows.Columns.Cast<DataColumn>().OrderBy(c => c.ColumnName, StringComparer.Ordinal)
                .Select(c => new { Name = c.ColumnName, Type = c.DataType.FullName, Value = VersionValue(rows.Rows[0][c]) }).ToArray();
            string json = new JavaScriptSerializer().Serialize(values);
            string version;
            using (SHA256 hash = SHA256.Create()) version = Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(json)));
            return new NbysGiftEditSnapshot { Gift = First<Armagan>(rows), Version = version };
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

        private static T First<T>(DataTable rows) where T : new()
        {
            return rows == null ? default(T) : new Armagan().ToList<T>(rows).FirstOrDefault();
        }

        private static bool IsEditableStatus(string status)
        {
            return new[] { ProjeConstants.DURUM_BELGE_ISTEMIYOR, ProjeConstants.DURUM_GONDERILMEDI,
                ProjeConstants.DURUM_GONDERILDI, ProjeConstants.DURUM_ULASILAMADI, ProjeConstants.DURUM_KONTROLEDILDI,
                ProjeConstants.DURUM_ERKENGONDERILDI, ProjeConstants.DURUM_ERTELENDI, ProjeConstants.DURUM_DAHAONCEIADE,
                ProjeConstants.DURUM_AFETNEDENIYLE_GONDERILMEDI, ProjeConstants.DURUM_EDEVLETTENBELGEGONDERILDI }.Contains(status);
        }
    }
}
