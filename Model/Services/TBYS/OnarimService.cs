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
    public class OnarimService
    {
        private readonly OnarimRepository repository;
        private readonly PortalLogContext context;
        private readonly TasinmazRepository propertyRepository;

        public OnarimService() : this(new OnarimRepository()) { }

        public OnarimService(PortalLogContext context) : this(new OnarimRepository(context))
        {
            this.context = context;
            propertyRepository = new TasinmazRepository(context);
        }

        public OnarimService(OnarimRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Onarim GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<Onarim> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public List<Onarim> GetByTasinmazId(int tasinmazId)
        {
            return ToList(repository.SelectByTasinmazId(tasinmazId));
        }

        public DataTable GetAllForDataTable()
        {
            return repository.SelectAllForDataTable();
        }

        public List<Onarim> GetByOnarimId(int onarimId)
        {
            return ToList(repository.SelectByOnarimId(onarimId));
        }

        public Onarim GetNext(int onarimId)
        {
            DataTable table = repository.SelectNext(onarimId);
            return table == null ? GetMin() : Map(table);
        }

        public Onarim GetPrevious(int onarimId)
        {
            DataTable table = repository.SelectPrevious(onarimId);
            return table == null ? GetMax() : Map(table);
        }

        public Onarim GetMax()
        {
            return GetById(ReadId(repository.SelectMaxId()));
        }

        public Onarim GetMin()
        {
            return GetById(ReadId(repository.SelectMinId()));
        }

        public List<Onarim> GetByTasinmazIdWithAddress(int tasinmazId)
        {
            return ToList(repository.SelectByTasinmazIdWithAddress(tasinmazId));
        }

        public int Save(Onarim item)
        {
            if (context != null)
            {
                using (SqlTransactionContext transaction = new SqlTransactionContext())
                {
                    int id = Save(item, transaction);
                    transaction.Complete();
                    return id;
                }
            }
            if (item == null) throw new ArgumentNullException("item");

            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);

            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ONARIM);
            }

            return item.Id;
        }

        public bool Update(Onarim item)
        {
            if (context != null)
            {
                using (SqlTransactionContext transaction = new SqlTransactionContext())
                {
                    bool updated = Update(item, transaction);
                    if (updated) transaction.Complete();
                    return updated;
                }
            }
            if (item == null) throw new ArgumentNullException("item");

            Onarim oldItem = GetById(item.Id);
            bool isSuccess = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                isSuccess = repository.Update(item);
            }

            if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_ONARIM);
            }

            return isSuccess;
        }

        public bool Delete(Onarim item)
        {
            if (context != null)
            {
                using (SqlTransactionContext transaction = new SqlTransactionContext())
                {
                    bool deleted = Delete(item, transaction);
                    if (deleted) transaction.Complete();
                    return deleted;
                }
            }
            if (item == null || item.Id == 0) return false;

            Onarim oldItem = GetById(item.Id);
            if (oldItem == null) return false;

            bool isDeleted = repository.Delete(item.Id);
            if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_ONARIM);
            }

            return isDeleted;
        }

        public int Save(Onarim item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null) throw new ArgumentNullException("item", "Kaydedilecek onarım gereklidir.");
            EnsureParent(item.TasinmazId, transaction);
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("Onarım kaydı oluşturulamadı.");
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Kaydedilen onarım okunamadı.");
            RecordAudit("INSERT", item.Id, null, saved, transaction);
            return item.Id;
        }

        public bool Update(Onarim item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            EnsureParent(item.TasinmazId, transaction);
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            Onarim old = Map(oldValues);
            if (old == null) return false;
            if (old.TasinmazId != item.TasinmazId)
                throw new InvalidOperationException("Onarımın bağlı taşınmazı değişmiş. Sayfayı yenileyip tekrar deneyiniz.");
            item.OlusturmaTarihi = old.OlusturmaTarihi;
            item.Olusturan = old.Olusturan;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (!repository.Update(item, transaction, context)) return false;
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Güncellenen onarım okunamadı.");
            RecordAudit("UPDATE", item.Id, oldValues, saved, transaction);
            return true;
        }

        public bool Delete(Onarim item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            Onarim old = Map(oldValues);
            if (old == null || old.TasinmazId != item.TasinmazId) return false;
            if (!repository.Delete(item.Id, transaction, context)) return false;
            RecordAudit("DELETE", item.Id, oldValues, null, transaction);
            return true;
        }

        private void EnsureParent(int id, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (id <= 0 || propertyRepository.SelectById(id, transaction, context) == null)
                throw new InvalidOperationException("İlişkili taşınmaz kaydı bulunamadı.");
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
            int auditId = log.RecordAudit(context, operation, "Onarim", id.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId <= 0) return;
            foreach (string field in new[] { "TasinmazId", "HarcamaUsulu", "OnayTarihi", "Tutar" })
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

        private static List<Onarim> ToList(DataTable table)
        {
            return new Onarim().ToList<Onarim>(table);
        }

        private static Onarim Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }

        private static int ReadId(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
                return 0;
            return table.Rows[0]["Id"].ConvertToInt();
        }
    }
}
