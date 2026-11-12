using DAO.Repositories.TBYS;
using DAO.Ortak;
using DAO.Repositories.Ortak;
using Utility.Logging;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public enum OdemePlaniSilmeResult { Completed, NotFound, InUse }

    public class OdemePlaniService
    {
        private readonly OdemePlaniRepository repository;
        private readonly PortalLogContext context;
        public OdemePlaniService() : this(new OdemePlaniRepository()) { }
        public OdemePlaniService(OdemePlaniRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public OdemePlaniService(PortalLogContext context) : this(new OdemePlaniRepository(context)) { this.context = context; }
        public OdemePlani GetById(int id) {
            try
            {
                return Map(repository.SelectById(id));
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "OdemePlani", id, error);
                throw;
            }
        }
        public OdemePlani GetById(int id, SqlTransactionContext transaction) {
            try
            {
                return Map(repository.SelectById(id, transaction, context));
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "OdemePlani", id, error);
                throw;
            }
        }
        public List<OdemePlani> GetAll() { return ToList(repository.SelectAll()); }
        public List<OdemePlani> GetBySozlesmeId(int id) {
            try
            {
                return ToList(repository.SelectBySozlesmeId(id));
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "KiraSozlesme", id, error);
                throw;
            }
        }
        public List<OdemePlani> GetBySozlesmeId(int id, SqlTransactionContext transaction) {
            try
            {
                return ToList(repository.SelectBySozlesmeId(id, transaction, context));
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "KiraSozlesme", id, error);
                throw;
            }
        }
        public OdemePlani GetBySozlesmeIdSira(int id, int sira) { return Map(repository.SelectBySozlesmeIdSira(id, sira)); }
        public OdemePlani GetBySozlesmeIdOdemeTarihi(int id, DateTime tarih) { return Map(repository.SelectBySozlesmeIdOdemeTarihi(id, tarih)); }
        public OdemePlani GetBySozlesmeIdOdemeTarihi(int id, DateTime tarih, SqlTransactionContext transaction) { return Map(repository.SelectBySozlesmeIdOdemeTarihi(id, tarih, transaction, context)); }
        public bool Exists(int id) { return ToList(repository.SelectExists(id)).Count > 0; }
        public OdemePlani GetLastBySozlesmeId(int id) { return ToList(repository.SelectLastBySozlesmeId(id)).LastOrDefault(); }
        public OdemePlani GetLastBySozlesmeId(int id, SqlTransactionContext transaction) { return GetBySozlesmeId(id, transaction).LastOrDefault(); }
        public OdemePlani GetFirstBySozlesmeId(int id) { return Map(repository.SelectFirstBySozlesmeId(id)); }
        public OdemePlani GetFirstBySozlesmeId(int id, SqlTransactionContext transaction) { return GetBySozlesmeId(id, transaction).FirstOrDefault(x => x.Sira == 1); }
        public int Save(OdemePlani item)
        {
            try
            {
                if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
                using (SqlTransactionContext tx = new SqlTransactionContext()) { int id = Save(item, tx); tx.Complete(); return id; }
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "OdemePlani", item == null ? 0 : item.Id, error);
                throw;
            }
        }
        public int Save(OdemePlani item, SqlTransactionContext transaction)
        {
            try
            {
                if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item, transaction, context); if (item.Id > 0) RecordAudit("INSERT", item.Id, null, repository.SelectById(item.Id, transaction, context), transaction); return item.Id;
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "OdemePlani", item == null ? 0 : item.Id, error);
                throw;
            }
        }
        public bool Update(OdemePlani item)
        {
            try
            {
                if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
                using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = Update(item, tx); if (ok) tx.Complete(); return ok; }
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "OdemePlani", item == null ? 0 : item.Id, error);
                throw;
            }
        }
        public bool Update(OdemePlani item, SqlTransactionContext transaction)
        {
            try
            {
                if (item == null || item.Id == 0) return false; DataTable oldValues = repository.SelectById(item.Id, transaction, context); if (oldValues == null || oldValues.Rows.Count == 0) return false; item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool result = repository.Update(item, transaction, context); if (result) RecordAudit("UPDATE", item.Id, oldValues, repository.SelectById(item.Id, transaction, context), transaction); return result;
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "OdemePlani", item == null ? 0 : item.Id, error);
                throw;
            }
        }
        public bool Delete(OdemePlani item)
        {
            try
            {
                if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
                using (SqlTransactionContext tx = new SqlTransactionContext()) { bool ok = Delete(item, tx); if (ok) tx.Complete(); return ok; }
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "OdemePlani", item == null ? 0 : item.Id, error);
                throw;
            }
        }
        public bool Delete(OdemePlani item, SqlTransactionContext transaction)
        {
            try
            {
                if (item == null || item.Id == 0) return false; DataTable oldValues = repository.SelectById(item.Id, transaction, context); if (oldValues == null || oldValues.Rows.Count == 0) return false; bool result = repository.Delete(item.Id, transaction, context); if (result) RecordAudit("DELETE", item.Id, oldValues, null, transaction); return result;
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "OdemePlani", item == null ? 0 : item.Id, error);
                throw;
            }
        }
        public bool DeleteBySozlesmeId(OdemePlani item, int id)
        {
            try
            {
                return DeletePlan(id) == OdemePlaniSilmeResult.Completed;
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "KiraSozlesme", id, error);
                throw;
            }
        }

        public OdemePlaniSilmeResult DeletePlan(int contractId)
        {
            try
            {
                if (context == null || context.Project != "TBYS" || string.IsNullOrWhiteSpace(context.UserLogin))
                    throw new InvalidOperationException("Bu işlem için TBYS kullanıcı bağlamı gereklidir.");
                if (contractId <= 0) return OdemePlaniSilmeResult.NotFound;
                using (var transaction = new SqlTransactionContext())
                {
                    var contracts = new KiraSozlesmeRepository(context);
                    if (contracts.SelectById(contractId, transaction, context) == null)
                        return OdemePlaniSilmeResult.NotFound;
                    List<OdemePlani> plans = GetBySozlesmeId(contractId, transaction);
                    if (plans.Count == 0) return OdemePlaniSilmeResult.NotFound;
                    // Lock dependent rows and missing-key ranges before any destructive operation.
                    if (repository.HasPaymentLinks(contractId, transaction, context)
                        || contracts.HasProtectedDeleteLinks(contractId, transaction, context))
                        return OdemePlaniSilmeResult.InUse;
                    if (!new OdemeAyrintiService(context).DeleteBySozlesmeId(contractId, transaction)
                        || !DeleteBySozlesmeId(plans[0], contractId, transaction))
                        throw new InvalidOperationException("Ödeme planı ve hesap ayrıntıları silinemedi; işlem geri alındı.");
                    transaction.Complete();
                }
                return OdemePlaniSilmeResult.Completed;
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "KiraSozlesme", contractId, error);
                throw;
            }
        }
        public bool DeleteBySozlesmeId(OdemePlani item, int id, SqlTransactionContext transaction)
        {
            try
            {
                DataTable oldValues = repository.SelectBySozlesmeId(id, transaction, context); bool result = repository.DeleteBySozlesmeId(id, transaction, context); if (result && oldValues != null) foreach (DataRow row in oldValues.Rows) RecordAudit("DELETE", Convert.ToInt32(row["Id"]), SingleRow(oldValues, row), null, transaction); return result;
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "KiraSozlesme", id, error);
                throw;
            }
        }

        private void RecordAudit(string operation, int id, DataTable oldValues, DataTable current, SqlTransactionContext transaction)
        {
            if (!PortalLogSettings.IsEnabled(context)) return;
            PortalLogRepository log = new PortalLogRepository(); int auditId = log.RecordAudit(context, operation, "OdemePlani", id.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId == 0) return;
            string[] fields = { "SozlesmeId", "Yil", "Ay", "VadeBasTar", "VadeBitTar", "KiraBedeli", "OdenenTutar", "AnaPara", "FaizliBakiye", "FaizOrani", "FaizTutari", "Sira" };
            foreach (string field in fields) { string oldValue = AuditValue(oldValues, field); string newValue = AuditValue(current, field); if (oldValue != newValue) log.RecordAuditChange(auditId, field, oldValue, newValue, transaction); }
        }
        private static string AuditValue(DataTable table, string field) { if (table == null || table.Rows.Count == 0 || !table.Columns.Contains(field) || table.Rows[0][field] == DBNull.Value) return null; return Convert.ToString(table.Rows[0][field], CultureInfo.InvariantCulture); }
        private static DataTable SingleRow(DataTable source, DataRow row) { DataTable result = source.Clone(); result.ImportRow(row); return result; }
        public bool CreatePaymentPlan(KiraSozlesme kiraSozlesme)
        {
            try
            {
                if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
                using (SqlTransactionContext tx = new SqlTransactionContext()) { bool created = CreatePaymentPlan(kiraSozlesme, tx); if (created) tx.Complete(); return created; }
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "KiraSozlesme", kiraSozlesme == null ? 0 : kiraSozlesme.Id, error);
                throw;
            }
        }
        internal void RebuildPaymentPlan(KiraSozlesme contract, SqlTransactionContext transaction)
        {
            try
            {
                if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
                List<OdemePlani> existing = GetBySozlesmeId(contract.Id, transaction);
                if (existing.Any(p => p.Sira < 0 || p.Sira > 12) || existing.GroupBy(p => p.Sira).Any(g => g.Count() > 1))
                    throw new InvalidOperationException("Ödeme planı sıraları tutarsız. Sözleşme güncellenmeden önce plan kontrol edilmelidir.");
                var planIds = new HashSet<int>(existing.Select(p => p.Id));
                DataTable payments = new OdemeRepository(context).SelectBySozlesmeId(contract.Id, transaction, context);
                if (payments != null)
                    foreach (DataRow payment in payments.Rows)
                    {
                        if (payment.IsNull("OdemePlaniId") || !planIds.Contains(Convert.ToInt32(payment["OdemePlaniId"]))
                            || payment.IsNull("KiraciId") || Convert.ToInt32(payment["KiraciId"]) != contract.KiraciId)
                            throw new InvalidOperationException("Sözleşmenin mevcut ödeme bağlantıları tutarsız. Güncellemeden önce kayıtlar kontrol edilmelidir.");
                    }
                // Confirmed contract corrections move due dates while preserving installment identities and payment links.
                CreatePaymentPlan(contract, transaction, existing.ToDictionary(p => p.Sira));
            }
            catch (Exception error)
            {
                PortalLogContext.AttachEntityToError(context, "KiraSozlesme", contract == null ? 0 : contract.Id, error);
                throw;
            }
        }

        private bool CreatePaymentPlan(KiraSozlesme kiraSozlesme, SqlTransactionContext transaction,
            Dictionary<int, OdemePlani> existing = null)
        {
            IFormatProvider cultureInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            int taksitSayisi = kiraSozlesme.TaksitSayisi;
            if (taksitSayisi < 1) taksitSayisi = 1;
            else if (taksitSayisi > 12) taksitSayisi = 12;
            DateTime sozlesmeBaslangic = kiraSozlesme.SozBasTar;
            DateTime sozlesmeBitis = sozlesmeBaslangic.AddMonths(taksitSayisi);
            string bitisAyi = sozlesmeBitis.ToString("MMMM", cultureInfo);
            DateTime vadeBaslangic = sozlesmeBaslangic;
            DateTime vadeBitis = vadeBaslangic.AddMonths(1).AddDays(-1);
            decimal aylikKira = kiraSozlesme.KiraBedeli;
            if (kiraSozlesme.OdemeSekli.Equals(ProjeConstants.KIRA_ODMSEKLI_YILLIK)) aylikKira = kiraSozlesme.KiraBedeli / taksitSayisi;
            SaveCarryOver(kiraSozlesme, kiraSozlesme.DevirAnaPara, kiraSozlesme.DevirFaizTutari, "Önceki Sözlesmeden devir", 0, transaction, existing);
            for (int i = 1; i <= taksitSayisi; i++)
            {
                int yil = vadeBaslangic.Year; string ay = vadeBaslangic.ToString("MMMM", cultureInfo);
                if ((ay + "/" + yil).Equals(bitisAyi + "/" + sozlesmeBitis.Year)) break;
                SaveNew(kiraSozlesme, yil, ay, vadeBaslangic, vadeBitis, sozlesmeBaslangic, sozlesmeBitis, aylikKira, i, transaction, existing);
                vadeBaslangic = vadeBaslangic.AddMonths(1); vadeBitis = vadeBaslangic.AddMonths(1).AddDays(-1);
            }
            for (int i = taksitSayisi + 1; i <= 12; i++)
            {
                int yil = vadeBaslangic.Year; string ay = vadeBaslangic.ToString("MMMM", cultureInfo);
                SaveNew(kiraSozlesme, yil, ay, vadeBaslangic, vadeBitis, sozlesmeBaslangic, sozlesmeBitis, 0, i, transaction, existing);
                vadeBaslangic = vadeBaslangic.AddMonths(1); vadeBitis = vadeBaslangic.AddMonths(1).AddDays(-1);
            }
            return true;
        }
        private void SaveNew(KiraSozlesme kiraSozlesme, int yil, string ay, DateTime vadeBaslangic, DateTime vadeBitis, DateTime odemeBaslangic, DateTime odemeBitis, decimal aylikKira, int sira, SqlTransactionContext transaction, Dictionary<int, OdemePlani> existing)
        {
            OdemePlani item = new OdemePlani { SozlesmeId = kiraSozlesme.Id, Yil = yil, Ay = ay, VadeBasTar = vadeBaslangic, VadeBitTar = vadeBitis, KiraBedeli = aylikKira, AnaPara = 0, FaizTutari = 0, OdenenTutar = 0, OdemeBasTar = odemeBaslangic, OdemeBitTar = odemeBitis, Sira = sira, Aciklama = "" };
            PersistPlan(item, transaction, existing);
        }
        private void SaveCarryOver(KiraSozlesme kiraSozlesme, decimal anaPara, decimal faizTutari, string aciklama, int sira, SqlTransactionContext transaction, Dictionary<int, OdemePlani> existing)
        {
            OdemePlani item = new OdemePlani { SozlesmeId = kiraSozlesme.Id, VadeBitTar = kiraSozlesme.SozBasTar.AddDays(-1), AnaPara = anaPara, FaizTutari = faizTutari, FaizliBakiye = anaPara + faizTutari, OdenenTutar = 0, Aciklama = aciklama, Sira = sira };
            PersistPlan(item, transaction, existing);
        }

        private void PersistPlan(OdemePlani item, SqlTransactionContext transaction, Dictionary<int, OdemePlani> existing)
        {
            OdemePlani old;
            if (existing != null && existing.TryGetValue(item.Sira, out old))
            {
                // Keep identities so payments, collection tracking and other links remain valid.
                item.Id = old.Id;
                item.Olusturan = old.Olusturan;
                item.OlusturmaTarihi = old.OlusturmaTarihi;
                item.Aciklama = old.Aciklama;
                item.OdenenTutar = new OdemeService(context).GetSumBySozlesmeIdOdemePlaniId(item.SozlesmeId, item.Id, transaction);
                if (!Update(item, transaction)) throw new InvalidOperationException("Ödeme planı satırı güncellenemedi.");
            }
            else if (Save(item, transaction) <= 0)
                throw new InvalidOperationException("Ödeme planı satırı kaydedilemedi.");
        }
        private static List<OdemePlani> ToList(DataTable t) { return new OdemePlani().ToList<OdemePlani>(t); }
        private static OdemePlani Map(DataTable t) { return ToList(t).FirstOrDefault(); }
    }
}
