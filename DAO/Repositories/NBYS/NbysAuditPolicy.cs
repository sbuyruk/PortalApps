using DAO.Ortak;
using DAO.Repositories.Ortak;
using System;
using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace DAO.Repositories.NBYS
{
    // Field coverage and audit representation are shared by all donor and gift write paths.
    internal static class NbysAuditPolicy
    {
        internal static string[] DonorFields
        {
            get
            {
                return new[] { "Adi", "Soyadi", "TCKimlikNo", "Ili", "Ilcesi", "Adres", "Telefon1", "Telefon2",
                    "TuzelKisi", "Sag", "Eposta", "PostaKodu", "Meslek", "Aciklama", "Ulasilamiyor",
                    "BelgeIstemiyor", "DergiGonderilmesin" };
            }
        }

        internal static string[] GiftFields
        {
            get
            {
                return new[] { "BagisciId", "ArmaganTanimId", "Tarih", "Durum", "BagisMiktari", "DovizCinsi",
                    "Aciklama", "BelgedeYazanIsim", "BelgeGecersizMi", "GecersizNBHareketId", "GecersizYapan",
                    "GecersizYapmaTarihi", "ArmaganBagisMiktari", "IadeMiktari", "BagisMiktariYazmasin",
                    "CokluBagis", "DuzenliBagis", "KacinciBelge" };
            }
        }

        internal static void RecordChanges(PortalLogRepository log, int auditId, string entity,
            DataTable previous, DataTable saved, string[] fields, SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            foreach (string field in fields)
            {
                string before = Value(previous, field), after = Value(saved, field);
                // Compare original values: two different numbers may have the same masked suffix.
                if (before != after)
                    log.RecordAuditChange(auditId, field, Display(entity, field, before), Display(entity, field, after), transaction);
            }
        }

        private static string Value(DataTable rows, string field)
        {
            if (rows == null || rows.Rows.Count == 0 || rows.Rows[0].IsNull(field)) return null;
            object value = rows.Rows[0][field];
            return value is DateTime ? ((DateTime)value).ToString("o", CultureInfo.InvariantCulture)
                : Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static string Display(string entity, string field, string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            if (entity == "NakitBagisci")
            {
                if (field == "TCKimlikNo" || field == "Telefon1" || field == "Telefon2")
                    return value.Length > 2 ? "***" + value.Substring(value.Length - 2) : "***";
                if (field == "Eposta") return value.Substring(0, 1) + "***";
            }
            if (value.Length <= 2000) return value;
            // Preserve a preview and full-value fingerprint within the audit column's 2000-character limit.
            string fingerprint;
            using (SHA256 hash = SHA256.Create())
                fingerprint = Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(value)));
            int prefixLength = 1900;
            if (char.IsHighSurrogate(value[prefixLength - 1])) prefixLength--;
            return value.Substring(0, prefixLength) + "… [Kısaltıldı; SHA-256: " + fingerprint + "]";
        }
    }
}
