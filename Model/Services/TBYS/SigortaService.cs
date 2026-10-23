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
    public class SigortaService
    {
        private readonly SigortaRepository repository;
        private readonly PortalLogContext context;
        private readonly TasinmazRepository propertyRepository;
        private readonly BagimsizBolumRepository sectionRepository;

        public SigortaService() : this(new SigortaRepository()) { }

        public SigortaService(PortalLogContext context)
            : this(new SigortaRepository(context), new TasinmazRepository(context), new BagimsizBolumRepository(context))
        {
            this.context = context;
        }

        public SigortaService(SigortaRepository repository)
            : this(repository, new TasinmazRepository(), new BagimsizBolumRepository()) { }

        private SigortaService(SigortaRepository repository, TasinmazRepository propertyRepository, BagimsizBolumRepository sectionRepository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
            this.propertyRepository = propertyRepository;
            this.sectionRepository = sectionRepository;
        }

        public Sigorta GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<Sigorta> GetAll()
        {
            return new Sigorta().ToList<Sigorta>(repository.SelectAll());
        }

        public List<Sigorta> GetByTasinmazId(int tasinmazId)
        {
            return new Sigorta().ToList<Sigorta>(repository.SelectByTasinmazId(tasinmazId));
        }

        public List<Sigorta> GetByIdList(int id)
        {
            return new Sigorta().ToList<Sigorta>(repository.SelectById(id));
        }

        public DataTable GetInventoryList()
        {
            return repository.SelectInventoryList();
        }

        public decimal GetInsuranceValueTotal(string sigortaCinsi)
        {
            return ReadTotal(repository.SelectInsuranceValueTotal(sigortaCinsi));
        }

        public decimal GetPremiumTotal(string sigortaCinsi)
        {
            return ReadTotal(repository.SelectPremiumTotal(sigortaCinsi));
        }

        public DataTable GetByTeminatSigortaCinsi(string sigortaCinsi, bool vadesiGelenler, bool isDeprem, bool isYangin, bool isMakine100000,
            bool isMakine5000, bool isJenerator, bool isAsansor, bool isKazan, int bolgeId, DateTime basTarih, DateTime bitTarih)
        {
            return repository.SelectByTeminatSigortaCinsi(sigortaCinsi, vadesiGelenler, isDeprem, isYangin, isMakine100000,
                isMakine5000, isJenerator, isAsansor, isKazan, bolgeId, basTarih, bitTarih);
        }

        public Sigorta GetNext(int sigortaId)
        {
            List<Sigorta> list = new Sigorta().ToList<Sigorta>(repository.SelectNavigationList());
            int index = list.FindIndex(s => s.Id == sigortaId);
            if (index < 0 || list.Count == 0) return new Sigorta();
            return index < list.Count - 1 ? list[index + 1] : list[list.Count - 1];
        }

        public Sigorta GetPrev(int sigortaId)
        {
            List<Sigorta> list = new Sigorta().ToList<Sigorta>(repository.SelectNavigationList());
            int index = list.FindIndex(s => s.Id == sigortaId);
            if (index < 0 || list.Count == 0) return new Sigorta();
            return index > 0 ? list[index - 1] : list[0];
        }

        public Sigorta GetMax()
        {
            return GetById(ReadId(repository.SelectMaxId()));
        }

        public Sigorta GetMin()
        {
            return GetById(ReadId(repository.SelectMinId()));
        }

        public string GetInventoryListAsJson()
        {
            return new Sigorta().ToJSON(GetInventoryList());
        }

        public Sigorta GetLatestByTasinmazId(int tasinmazId)
        {
            return GetByTasinmazId(tasinmazId).FirstOrDefault();
        }

        public int Save(Sigorta item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                int id = Save(item, transaction, context);
                transaction.Complete();
                return id;
            }
        }

        public bool Update(Sigorta item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool updated = Update(item, transaction, context);
                if (updated) transaction.Complete();
                return updated;
            }
        }

        public bool Delete(Sigorta item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool deleted = Delete(item, transaction, context);
                if (deleted) transaction.Complete();
                return deleted;
            }
        }

        public Sigorta GetLatestByTasinmazId(int tasinmazId, SqlTransactionContext transaction, PortalLogContext context)
        {
            return Map(repository.SelectLatestByTasinmazId(tasinmazId, transaction, context));
        }

        public int Save(Sigorta item, SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireTransaction(transaction, context);
            if (item == null) throw new ArgumentNullException("item", "Kaydedilecek sigorta gereklidir.");
            EnsureRelation(item, transaction, context);
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("Sigorta kaydı oluşturulamadı.");
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Kaydedilen sigorta okunamadı.");
            RecordAudit("INSERT", item.Id, null, saved, transaction, context);
            return item.Id;
        }

        public bool Update(Sigorta item, SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireTransaction(transaction, context);
            if (item == null || item.Id <= 0) return false;
            EnsureRelation(item, transaction, context);
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            Sigorta old = Map(oldValues);
            if (old == null) return false;
            if (old.TasinmazId != item.TasinmazId)
                throw new InvalidOperationException("Sigortanın bağlı taşınmazı değişmiş. Sayfayı yenileyip tekrar deneyiniz.");
            item.OlusturmaTarihi = old.OlusturmaTarihi;
            item.Olusturan = old.Olusturan;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (!repository.Update(item, transaction, context)) return false;
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Güncellenen sigorta okunamadı.");
            RecordAudit("UPDATE", item.Id, oldValues, saved, transaction, context);
            return true;
        }

        public bool Delete(Sigorta item, SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireTransaction(transaction, context);
            if (item == null || item.Id <= 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            Sigorta old = Map(oldValues);
            if (old == null || old.TasinmazId != item.TasinmazId) return false;
            if (!repository.Delete(item.Id, transaction, context)) return false;
            RecordAudit("DELETE", item.Id, oldValues, null, transaction, context);
            return true;
        }

        public int SaveWithPropertyStatus(Sigorta item)
        {
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                int id = Save(item, transaction, context);
                SynchronizePropertyStatus(item, transaction, context);
                transaction.Complete();
                return id;
            }
        }

        public bool UpdateWithPropertyStatus(Sigorta item)
        {
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                if (!Update(item, transaction, context)) return false;
                SynchronizePropertyStatus(item, transaction, context);
                transaction.Complete();
                return true;
            }
        }

        private void EnsureRelation(Sigorta item, SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireTransaction(transaction, context);
            if (item.TasinmazId <= 0 || propertyRepository.SelectById(item.TasinmazId, transaction, context) == null)
                throw new InvalidOperationException("İlişkili taşınmaz kaydı bulunamadı.");
            if (item.BolumId < 0) throw new InvalidOperationException("Bağımsız bölüm seçimi geçersiz.");
            if (item.BolumId == 0) return;
            DataTable section = sectionRepository.SelectById(item.BolumId, transaction, context);
            if (section == null || Convert.ToInt32(section.Rows[0]["TasinmazId"]) != item.TasinmazId)
                throw new InvalidOperationException("Seçilen bağımsız bölüm bu taşınmaza ait değil.");
        }

        private void SynchronizePropertyStatus(Sigorta item, SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireTransaction(transaction, context);
            DataTable oldValues = propertyRepository.SelectById(item.TasinmazId, transaction, context);
            if (oldValues == null) throw new InvalidOperationException("İlişkili taşınmaz kaydı bulunamadı.");
            string oldStatus = oldValues.Rows[0]["SigortaDurumu"] == DBNull.Value ? null : Convert.ToString(oldValues.Rows[0]["SigortaDurumu"]);
            if (string.Equals(oldStatus, item.SigortaCinsi, StringComparison.Ordinal)) return;
            propertyRepository.UpdateInsuranceStatus(item.TasinmazId, item.SigortaCinsi, DateTime.Now,
                UtilityHelper.GetCurrentUserName(), transaction, context);
            DataTable saved = propertyRepository.SelectById(item.TasinmazId, transaction, context);
            if (saved == null) throw new InvalidOperationException("Güncellenen taşınmaz okunamadı.");
            RecordEntityAudit("UPDATE", "Tasinmaz", item.TasinmazId, oldValues, saved,
                new[] { "SigortaDurumu" }, transaction, context);
        }

        public bool UpdateDocument(Sigorta item)
        {
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool updated = UpdateDocument(item, transaction, context);
                if (updated) transaction.Complete();
                return updated;
            }
        }

        public bool UpdateDocument(Sigorta item, SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireTransaction(transaction, context);
            if (item == null || item.Id <= 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            Sigorta old = Map(oldValues);
            if (old == null || old.TasinmazId != item.TasinmazId) return false;
            repository.UpdateDocument(item.Id, item.PDFDosyasi, DateTime.Now, UtilityHelper.GetCurrentUserName(), transaction, context);
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Güncellenen sigorta belge bilgisi okunamadı.");
            PortalLogRepository log = new PortalLogRepository();
            int auditId = log.RecordAudit(context, "UPDATE", "Sigorta", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId > 0)
            {
                string oldValue = string.IsNullOrWhiteSpace(old.PDFDosyasi) ? "false" : "true";
                string newValue = string.IsNullOrWhiteSpace(Convert.ToString(saved.Rows[0]["PDFDosyasi"])) ? "false" : "true";
                if (oldValue != newValue) log.RecordAuditChange(auditId, "BelgeMevcut", oldValue, newValue, transaction);
            }
            return true;
        }

        private static void RequireTransaction(SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "Veritabanı işlem bağlamı gereklidir.");
            if (context == null) throw new ArgumentNullException("context", "Bu işlem için TBYS log bağlamı gereklidir.");
        }

        private static void RecordAudit(string operation, int entityId, DataTable oldValues, DataTable current, SqlTransactionContext transaction, PortalLogContext context)
        {
            RecordEntityAudit(operation, "Sigorta", entityId, oldValues, current,
                new[] { "TasinmazId", "BolumId", "SigortaCinsi", "SigortaBasTar", "SigortaBitTar", "SigortaBedeli", "Prim", "KullanimSekli" }, transaction, context);
        }

        private static void RecordEntityAudit(string operation, string entityName, int entityId, DataTable oldValues,
            DataTable current, string[] fields, SqlTransactionContext transaction, PortalLogContext context)
        {
            RequireTransaction(transaction, context);
            PortalLogRepository log = new PortalLogRepository();
            int auditId = log.RecordAudit(context, operation, entityName, entityId.ToString(CultureInfo.InvariantCulture), transaction);
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
            object value = table.Rows[0][field];
            if (value is DateTime) return ((DateTime)value).ToString("o", CultureInfo.InvariantCulture);
            return value is decimal ? ((decimal)value).ToString("G29", CultureInfo.InvariantCulture)
                : Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static Sigorta Map(DataTable table)
        {
            return new Sigorta().ToList<Sigorta>(table).FirstOrDefault();
        }

        private static decimal ReadTotal(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
                return 0;

            return table.Rows[0]["Toplam"].ReturnZeroIfNull().ConvertToDecimal();
        }

        private static int ReadId(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
                return 0;

            return table.Rows[0]["Id"].ConvertToInt();
        }
    }
}
