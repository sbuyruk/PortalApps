using DAO.Repositories.TBYS;
using DAO.Ortak;
using DAO.Repositories.Ortak;
using System.Globalization;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace Model.Services.TBYS
{
    public class SozlesmeTasinmazService
    {
        private readonly SozlesmeTasinmazRepository repository;
        private readonly PortalLogContext context;
        private readonly KiraSozlesmeRepository contractRepository;
        private readonly TasinmazRepository propertyRepository;
        private readonly BagimsizBolumRepository sectionRepository;
        private readonly KiraciRepository tenantRepository;

        public SozlesmeTasinmazService() : this(new SozlesmeTasinmazRepository()) { }

        public SozlesmeTasinmazService(PortalLogContext context) : this(new SozlesmeTasinmazRepository(context))
        {
            this.context = context;
            contractRepository = new KiraSozlesmeRepository(context);
            propertyRepository = new TasinmazRepository(context);
            sectionRepository = new BagimsizBolumRepository(context);
            tenantRepository = new KiraciRepository(context);
        }

        public SozlesmeTasinmazService(SozlesmeTasinmazRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public SozlesmeTasinmaz GetById(int id) { return Map(repository.SelectById(id)); }
        public List<SozlesmeTasinmaz> GetAll() { return ToList(repository.SelectAll()); }
        public List<SozlesmeTasinmaz> GetBySozlesmeId(int id) { return ToList(repository.SelectBySozlesmeId(id)); }
        public List<SozlesmeTasinmaz> GetBySozlesmeId(int id, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            return ToList(repository.SelectBySozlesmeId(id, transaction, context));
        }
        public List<SozlesmeTasinmaz> GetByTasinmazId(int id) { return ToList(repository.SelectByTasinmazId(id)); }
        public List<SozlesmeTasinmaz> GetBySozlesmeIdTasinmazId(int sozlesmeId, int tasinmazId, int bolumId) { return ToList(repository.SelectBySozlesmeIdTasinmazId(sozlesmeId, tasinmazId, bolumId)); }
        public List<SozlesmeTasinmaz> GetByBolumId(int bolumId) { return ToList(repository.SelectByBolumId(bolumId)); }
        public List<SozlesmeTasinmaz> GetByBolumId(int bolumId, PortalLogContext context)
        {
            return ToList(new SozlesmeTasinmazRepository(context).SelectByBolumId(bolumId));
        }
        public DataTable GetBySozlesmeIdReturnDataTable(int id) { return repository.SelectBySozlesmeIdReturnDataTable(id); }
        public DataTable GetBySozlesmeIdReturnList(int id) { return repository.SelectBySozlesmeIdReturnList(id); }
        public DataTable GetBySozlesmeIdReturnDT(int id) { return repository.SelectBySozlesmeIdReturnDT(id); }

        public string GetBySozlesmeIdReturnJson(int id)
        {
            return new SozlesmeTasinmaz().ToJSON(repository.SelectBySozlesmeIdReturnList(id));
        }

        public decimal GetSumMetrekareBySozlesmeId(int id)
        {
            DataTable table = repository.SelectSumMetrekareBySozlesmeId(id);
            if (table == null || table.Rows.Count == 0)
                return 0;
            return table.Rows[0]["Metrekare"].ReturnZeroIfNull().ConvertToDecimal();
        }

        public bool DeleteBySozlesmeId(SozlesmeTasinmaz item, int sozlesmeId)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool deleted = DeleteBySozlesmeId(sozlesmeId, transaction);
                if (deleted) transaction.Complete();
                return deleted;
            }
        }

        public int Save(SozlesmeTasinmaz item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                int id = Save(item, transaction);
                transaction.Complete();
                return id;
            }
        }

        public bool Update(SozlesmeTasinmaz item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool updated = Update(item, transaction);
                if (updated) transaction.Complete();
                return updated;
            }
        }

        public bool Delete(SozlesmeTasinmaz item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool deleted = Delete(item, transaction);
                if (deleted) transaction.Complete();
                return deleted;
            }
        }

        public int Save(SozlesmeTasinmaz item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null) throw new ArgumentNullException("item", "Kaydedilecek sözleşme bağlantısı gereklidir.");
            EnsureParents(item, transaction);
            DataTable existing = repository.SelectBySozlesmeIdTasinmazId(item.SozlesmeId, item.TasinmazId, item.BolumId, transaction, context);
            if (existing != null)
            {
                item.Id = Convert.ToInt32(existing.Rows[0]["Id"]);
                return item.Id;
            }
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = context.UserLogin;
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("Sözleşme bağlantısı oluşturulamadı.");
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Kaydedilen sözleşme bağlantısı okunamadı.");
            RecordAudit("INSERT", "SozlesmeTasinmaz", item.Id, null, saved, new[] { "SozlesmeId", "TasinmazId", "BolumId" }, transaction);
            return item.Id;
        }

        public bool Update(SozlesmeTasinmaz item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            EnsureParents(item, transaction);
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            SozlesmeTasinmaz old = Map(oldValues);
            if (old == null) return false;
            if (old.SozlesmeId != item.SozlesmeId)
                throw new InvalidOperationException("Kaydın bağlı sözleşmesi değişmiş. Sayfayı yenileyip tekrar deneyiniz.");
            DataTable matches = repository.SelectBySozlesmeIdTasinmazId(item.SozlesmeId, item.TasinmazId, item.BolumId, transaction, context);
            if (matches != null)
                foreach (DataRow row in matches.Rows)
                    if (Convert.ToInt32(row["Id"]) != item.Id)
                        throw new InvalidOperationException("Bu taşınmaz veya bölüm sözleşmeye zaten bağlıdır.");
            item.OlusturmaTarihi = old.OlusturmaTarihi;
            item.Olusturan = old.Olusturan;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (!repository.Update(item, transaction, context)) return false;
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Güncellenen sözleşme bağlantısı okunamadı.");
            RecordAudit("UPDATE", "SozlesmeTasinmaz", item.Id, oldValues, saved, new[] { "SozlesmeId", "TasinmazId", "BolumId" }, transaction);
            return true;
        }

        public bool Delete(SozlesmeTasinmaz item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            contractRepository.SelectById(item.SozlesmeId, transaction, context);
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            SozlesmeTasinmaz old = Map(oldValues);
            if (old == null || old.SozlesmeId != item.SozlesmeId || old.TasinmazId != item.TasinmazId || old.BolumId != item.BolumId) return false;
            if (!repository.Delete(item.Id, transaction, context)) return false;
            RecordAudit("DELETE", "SozlesmeTasinmaz", item.Id, oldValues, null, new[] { "SozlesmeId", "TasinmazId", "BolumId" }, transaction);
            return true;
        }

        public bool DeleteBySozlesmeId(int contractId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (contractId <= 0) throw new ArgumentOutOfRangeException("contractId", "Sözleşme kimliği geçersiz.");
            contractRepository.SelectById(contractId, transaction, context);
            DataTable rows = repository.SelectBySozlesmeId(contractId, transaction, context);
            foreach (SozlesmeTasinmaz item in ToList(rows))
                if (!Delete(item, transaction)) throw new InvalidOperationException("Sözleşmenin taşınmaz bağlantıları silinemedi.");
            return true;
        }

        public void ApplySelection(SozlesmeTasinmaz selection, int tenantId)
        {
            if (selection == null) throw new ArgumentNullException("selection", "Sözleşme seçimi gereklidir.");
            if (selection.TasinmazId <= 0 && tenantId <= 0) return;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                ApplySelection(selection, tenantId, transaction);
                transaction.Complete();
            }
        }

        public void ApplySelection(SozlesmeTasinmaz selection, int tenantId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (selection == null) throw new ArgumentNullException("selection", "Sözleşme seçimi gereklidir.");
            if (selection.TasinmazId > 0) Save(selection, transaction);
            if (tenantId > 0 && !AssignTenant(selection.SozlesmeId, tenantId, transaction))
                throw new InvalidOperationException("Sözleşmenin kiracı seçimi kaydedilemedi.");
        }

        public bool AssignTenant(int contractId, int tenantId)
        {
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool updated = AssignTenant(contractId, tenantId, transaction);
                if (updated) transaction.Complete();
                return updated;
            }
        }

        public bool AssignTenant(int contractId, int tenantId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (contractId <= 0 || tenantId <= 0) throw new InvalidOperationException("Sözleşme ve kiracı seçimi gereklidir.");
            DataTable oldValues = contractRepository.SelectById(contractId, transaction, context);
            if (oldValues == null) throw new InvalidOperationException("Sözleşme kaydı bulunamadı.");
            if (tenantRepository.SelectById(tenantId, transaction, context) == null)
                throw new InvalidOperationException("Kiracı kaydı bulunamadı.");
            object oldId = oldValues.Rows[0]["KiraciId"];
            if (oldId != DBNull.Value && Convert.ToInt32(oldId) == tenantId) return true;
            contractRepository.UpdateTenant(contractId, tenantId, DateTime.Now, UtilityHelper.GetCurrentUserName(), transaction, context);
            DataTable saved = contractRepository.SelectById(contractId, transaction, context);
            if (saved == null) throw new InvalidOperationException("Güncellenen sözleşme okunamadı.");
            RecordAudit("UPDATE", "KiraSozlesme", contractId, oldValues, saved, new[] { "KiraciId" }, transaction);
            return true;
        }

        private void EnsureParents(SozlesmeTasinmaz item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item.SozlesmeId <= 0 || contractRepository.SelectById(item.SozlesmeId, transaction, context) == null)
                throw new InvalidOperationException("İlişkili sözleşme kaydı bulunamadı.");
            if (item.TasinmazId <= 0 || propertyRepository.SelectById(item.TasinmazId, transaction, context) == null)
                throw new InvalidOperationException("İlişkili taşınmaz kaydı bulunamadı.");
            if (item.BolumId < 0) throw new InvalidOperationException("Bağımsız bölüm seçimi geçersiz.");
            if (item.BolumId == 0) return;
            DataTable section = sectionRepository.SelectById(item.BolumId, transaction, context);
            if (section == null || Convert.ToInt32(section.Rows[0]["TasinmazId"]) != item.TasinmazId)
                throw new InvalidOperationException("Seçilen bağımsız bölüm bu taşınmaza ait değil.");
        }

        private void RequireTransaction(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "Veritabanı işlem bağlamı gereklidir.");
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
        }

        private void RecordAudit(string operation, string entityName, int id, DataTable oldValues,
            DataTable current, string[] fields, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            PortalLogRepository log = new PortalLogRepository();
            int auditId = log.RecordAudit(context, operation, entityName, id.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId == 0) return;
            if (auditId <= 0) return;
            foreach (string field in fields)
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

        private static List<SozlesmeTasinmaz> ToList(DataTable table) { return new SozlesmeTasinmaz().ToList<SozlesmeTasinmaz>(table); }
        private static SozlesmeTasinmaz Map(DataTable table) { return ToList(table).FirstOrDefault(); }
    }
}
