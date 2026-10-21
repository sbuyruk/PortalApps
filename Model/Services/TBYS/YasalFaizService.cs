using DAO.Repositories.TBYS;
using DAO.Ortak;
using DAO.Repositories.Ortak;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Globalization;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace Model.Services.TBYS
{
    public class YasalFaizService
    {
        private readonly YasalFaizRepository repository;
        private readonly PortalLogContext context;

        public YasalFaizService() : this(new YasalFaizRepository()) { }
        public YasalFaizService(PortalLogContext context) : this(new YasalFaizRepository(context)) { this.context = context; }
        public YasalFaizService(YasalFaizRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public YasalFaiz GetById(int id) { return Map(repository.SelectById(id)); }
        public List<YasalFaiz> GetAll() { return ToList(repository.SelectAll()); }
        public List<YasalFaiz> GetByYear(int year) { return ToList(repository.SelectByYear(year)); }
        public YasalFaiz GetByYearMonth(int year, int month) { return Map(repository.SelectByYearMonth(year, month)); }
        public YasalFaiz GetByYearMonth(int year, int month, SqlTransactionContext transaction) { return Map(repository.SelectByYearMonth(year, month, transaction, context)); }
        public decimal GetLatestRate() { return GetLatestValue(repository.SelectLatestRate("FaizOrani"), x => x.FaizOrani); }
        public decimal GetLatestTufe() { return GetLatestValue(repository.SelectLatestRate("Tufe"), x => x.Tufe); }
        public decimal GetLatestUfe() { return GetLatestValue(repository.SelectLatestRate("Ufe"), x => x.Ufe); }
        public int Save(YasalFaiz item)
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
            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_YASALFAIZ);
            return item.Id;
        }
        public bool Update(YasalFaiz item)
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
            if (item == null || item.Id == 0) return false;
            YasalFaiz old = GetById(item.Id);
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool ok = repository.Update(item);
            if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_YASALFAIZ);
            return ok;
        }
        public bool Delete(YasalFaiz item)
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
            YasalFaiz old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_YASALFAIZ);
            return ok;
        }
        public int Save(YasalFaiz item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null) throw new ArgumentNullException("item", "Kaydedilecek kayıt gereklidir.");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("Yasal faiz kaydı oluşturulamadı.");
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Kaydedilen yasal faiz kaydı okunamadı.");
            RecordAudit("INSERT", item.Id, null, saved, transaction);
            return item.Id;
        }

        public bool Update(YasalFaiz item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            YasalFaiz old = Map(oldValues);
            if (old == null) return false;
            item.OlusturmaTarihi = old.OlusturmaTarihi;
            item.Olusturan = old.Olusturan;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (!repository.Update(item, transaction, context)) return false;
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Güncellenen yasal faiz kaydı okunamadı.");
            RecordAudit("UPDATE", item.Id, oldValues, saved, transaction);
            return true;
        }

        public bool Delete(YasalFaiz item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            if (oldValues == null) return false;
            if (!repository.Delete(item.Id, transaction, context)) return false;
            RecordAudit("DELETE", item.Id, oldValues, null, transaction);
            return true;
        }

        public void CreateYear(int year)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            if (year < 1 || year > 9999) throw new ArgumentOutOfRangeException("year", "Geçerli bir yıl seçiniz.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                if (ToList(repository.SelectByYear(year, transaction, context)).Count > 0) return;
                decimal rate = GetLatestValue(repository.SelectLatestRate("FaizOrani", transaction, context), x => x.FaizOrani);
                decimal tufe = GetLatestValue(repository.SelectLatestRate("Tufe", transaction, context), x => x.Tufe);
                decimal ufe = GetLatestValue(repository.SelectLatestRate("Ufe", transaction, context), x => x.Ufe);
                for (int month = 1; month <= 12; month++)
                    Save(new YasalFaiz
                    {
                        Yil = year, Ay = month,
                        AyAdi = new DateTime(year, month, 1).ToString("MMMM", new CultureInfo(ProjeConstants.CULTUREINFO)),
                        FaizOrani = rate, Tufe = tufe, Ufe = ufe, Aciklama = string.Empty
                    }, transaction);
                transaction.Complete();
            }
        }

        public void UpdateMonths(IList<YasalFaiz> edits)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            if (edits == null || edits.Count == 0) throw new ArgumentException("Güncellenecek ayları seçiniz.", "edits");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                foreach (YasalFaiz edit in edits.OrderBy(x => x.Yil).ThenBy(x => x.Ay))
                {
                    if (edit.Yil < 1 || edit.Ay < 1 || edit.Ay > 12)
                        throw new InvalidOperationException("Yıl ve ay bilgisi geçerli değil.");
                    YasalFaiz item = Map(repository.SelectByYearMonth(edit.Yil, edit.Ay, transaction, context));
                    if (item == null) throw new InvalidOperationException("Güncellenecek yasal faiz kaydı bulunamadı.");
                    item.FaizOrani = edit.FaizOrani;
                    item.Tufe = edit.Tufe;
                    item.Ufe = edit.Ufe;
                    item.Aciklama = edit.Aciklama;
                    if (!Update(item, transaction)) throw new InvalidOperationException("Yasal faiz kaydı güncellenemedi.");
                }
                transaction.Complete();
            }
        }

        private void RequireTransaction(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "Veritabanı işlem bağlamı gereklidir.");
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
        }

        private void RecordAudit(string operation, int entityId, DataTable oldValues, DataTable current, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            PortalLogRepository log = new PortalLogRepository();
            int id = log.RecordAudit(context, operation, "YasalFaiz", entityId.ToString(CultureInfo.InvariantCulture), transaction);
            if (id <= 0) return; // Logging may be disabled; never fall back to the legacy audit writer.
            foreach (string field in new[] { "Yil", "Ay", "FaizOrani", "Tufe", "Ufe" })
                AddChange(log, id, field, AuditValue(oldValues, field), AuditValue(current, field), transaction);
        }

        private static string AuditValue(DataTable table, string field)
        {
            if (table == null || table.Rows.Count == 0 || table.Rows[0][field] == DBNull.Value) return null;
            object value = table.Rows[0][field];
            return value is decimal ? ((decimal)value).ToString("G29", CultureInfo.InvariantCulture)
                : Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static void AddChange(PortalLogRepository log, int auditId, string field, string oldValue,
            string newValue, SqlTransactionContext transaction)
        {
            if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
                log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
        }

        private static List<YasalFaiz> ToList(DataTable table) { return new YasalFaiz().ToList<YasalFaiz>(table); }
        private static YasalFaiz Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static decimal GetLatestValue(DataTable table, Func<YasalFaiz, decimal> valueSelector) { YasalFaiz item = Map(table); return item == null ? 0 : valueSelector(item); }
    }
}
