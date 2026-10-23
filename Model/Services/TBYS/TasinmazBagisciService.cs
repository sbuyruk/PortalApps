using DAO.Repositories.TBYS;
using DAO.Ortak;
using DAO.Repositories.Ortak;
using Utility.Logging;
using System.Globalization;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public class TasinmazBagisciService
    {
        private readonly TasinmazBagisciRepository repository;
        private readonly PortalLogContext context;
        public TasinmazBagisciService() : this(new TasinmazBagisciRepository()) { }
        public TasinmazBagisciService(PortalLogContext context) : this(new TasinmazBagisciRepository(context)) { this.context = context; }
        public TasinmazBagisciService(TasinmazBagisciRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public TasinmazBagisci GetById(int id) { return Map(repository.SelectById(id)); }
        public List<TasinmazBagisci> GetAll() { return ToList(repository.SelectAll()); }
        public List<TasinmazBagisci> GetAllBySagVefat(string sag) { return ToList(repository.SelectAllBySagVefat(sag == ProjeConstants.HEPSI ? null : sag)); }
        public List<TasinmazBagisci> GetByBolge(int bolgeId) { return ToList(repository.SelectByBolge(bolgeId, bolgeId == ProjeConstants.BOLGE_HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT)); }
        public List<TasinmazBagisci> GetByFilters(bool isSagVefat, bool isCiplakMulkiyet, bool isTCKimlikNoFull, bool isDogumTarihiFull) { return ToList(repository.SelectByFilters(isSagVefat, isCiplakMulkiyet, isTCKimlikNoFull, isDogumTarihiFull, ProjeConstants.BAGISCI_SAG, ProjeConstants.MULKIYETSEKLI_CM)); }
        public List<TasinmazBagisci> GetByIlAdi(string ilAdi) { return ToList(repository.SelectByIlAdi(ilAdi)); }
        public int Save(TasinmazBagisci item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                int id = Save(item, transaction);
                transaction.Complete();
                return id;
            }
        }
        public bool Update(TasinmazBagisci item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool updated = Update(item, transaction);
                if (updated) transaction.Complete();
                return updated;
            }
        }
        public bool Delete(TasinmazBagisci item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool deleted = Delete(item, transaction);
                if (deleted) transaction.Complete();
                return deleted;
            }
        }
        public int Save(TasinmazBagisci item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null) throw new ArgumentNullException("item", "Kaydedilecek bağışçı gereklidir.");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("Bağışçı kaydı oluşturulamadı.");
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Kaydedilen bağışçı okunamadı.");
            RecordAudit("INSERT", item.Id, null, saved, transaction);
            return item.Id;
        }

        public bool Update(TasinmazBagisci item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            TasinmazBagisci old = Map(oldValues);
            if (old == null) return false;
            item.OlusturmaTarihi = old.OlusturmaTarihi;
            item.Olusturan = old.Olusturan;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (!repository.Update(item, transaction, context)) return false;
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Güncellenen bağışçı okunamadı.");
            RecordAudit("UPDATE", item.Id, oldValues, saved, transaction);
            return true;
        }

        public bool Delete(TasinmazBagisci item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            if (oldValues == null) return false;
            if (repository.HasDependencies(item.Id, transaction, context)) return false;
            if (!repository.Delete(item.Id, transaction, context)) return false;
            RecordAudit("DELETE", item.Id, oldValues, null, transaction);
            return true;
        }

        private void RequireTransaction(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "Veritabanı işlem bağlamı gereklidir.");
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
        }

        private void RecordAudit(string operation, int id, DataTable oldValues, DataTable current, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            PortalLogRepository log = new PortalLogRepository();
            int auditId = log.RecordAudit(context, operation, "TasinmazBagisci", id.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId == 0) return;
            if (auditId <= 0) return;
            // Names, identity/contact details and free text are excluded from audit values.
            foreach (string field in new[] { "IlId", "IlceId", "Sag_vefat", "Gizli" })
            {
                string oldValue = AuditValue(oldValues, field);
                string newValue = AuditValue(current, field);
                if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
                    log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
            }
        }

        private static string AuditValue(DataTable table, string field)
        {
            if (table == null || table.Rows.Count == 0 || table.Rows[0][field] == DBNull.Value) return null;
            return Convert.ToString(table.Rows[0][field], CultureInfo.InvariantCulture);
        }

        private static List<TasinmazBagisci> ToList(DataTable table) { return new TasinmazBagisci().ToList<TasinmazBagisci>(table); }
        private static TasinmazBagisci Map(DataTable table) { return ToList(table).FirstOrDefault(); }
    }
}
