using DAO.Repositories.IKYS;
using DAO.Repositories.Ortak;
using DAO.Ortak;
using Model.IKYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Globalization;
using Model.Ortak;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace Model.Services.IKYS
{
    public class YoklamaService
    {
        private readonly YoklamaRepository repository;

        public YoklamaService() : this(new YoklamaRepository()) { }

        public YoklamaService(YoklamaRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Yoklama GetById(int id)
        {
            return Map(repository.SelectById(id));
        }
        public Yoklama GetById(int id, PortalLogContext context) { return Map(repository.SelectById(id, context)); }
        public List<Yoklama> GetAll(PortalLogContext context) { return new Yoklama().ToList<Yoklama>(repository.SelectAll(context)); }

        public int Save(Yoklama item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_YOKLAMA);
            return item.Id;
        }
        public int Save(Yoklama item, PortalLogContext context) { if (context == null) throw new ArgumentNullException("context"); if (!PortalLogSettings.IsEnabled(context)) return Save(item); if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); using (SqlTransactionContext transaction = new SqlTransactionContext()) { item.Id = repository.Insert(item, transaction, context); int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "Yoklama", item.Id.ToString(CultureInfo.InvariantCulture), transaction); RecordChanges(null, item, auditId, transaction); transaction.Complete(); } return item.Id; }

        public bool Update(Yoklama item)
        {
            if (item == null) throw new ArgumentNullException("item");
            Yoklama old = GetById(item.Id);
            bool ok = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                ok = repository.Update(item);
            }
            if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_YOKLAMA);
            return ok;
        }
        public bool Update(Yoklama item, PortalLogContext context) { if (context == null) throw new ArgumentNullException("context"); if (!PortalLogSettings.IsEnabled(context)) return Update(item); if (item == null || item.Id <= 0) return false; using (SqlTransactionContext transaction = new SqlTransactionContext()) { Yoklama old = Map(repository.SelectById(item.Id, transaction, context)); item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); if (!repository.Update(item, transaction, context)) return false; int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "Yoklama", item.Id.ToString(CultureInfo.InvariantCulture), transaction); RecordChanges(old, item, auditId, transaction); transaction.Complete(); return true; } }

        public bool Delete(Yoklama item)
        {
            if (item == null || item.Id == 0) return false;
            Yoklama old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_YOKLAMA);
            return ok;
        }
        public bool Delete(Yoklama item, PortalLogContext context) { if (context == null) throw new ArgumentNullException("context"); if (!PortalLogSettings.IsEnabled(context)) return Delete(item); if (item == null || item.Id <= 0) return false; using (SqlTransactionContext transaction = new SqlTransactionContext()) { Yoklama old = Map(repository.SelectById(item.Id, transaction, context)); if (old == null || !repository.Delete(item.Id, transaction, context)) return false; int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "Yoklama", item.Id.ToString(CultureInfo.InvariantCulture), transaction); RecordChanges(old, null, auditId, transaction); transaction.Complete(); return true; } }

        public List<Yoklama> GetAll()
        {
            return new Yoklama().ToList<Yoklama>(repository.SelectAll());
        }

        public List<Yoklama> GetByPersonelIdAndDate(int personelId, DateTime start, DateTime end)
        {
            return new Yoklama().ToList<Yoklama>(
                repository.SelectByPersonelIdAndDate(personelId, start, end));
        }
        public List<Yoklama> GetByPersonelIdAndDate(int personelId, DateTime start, DateTime end, PortalLogContext context) { return new Yoklama().ToList<Yoklama>(repository.SelectByPersonelIdAndDate(personelId, start, end, context)); }

        public DataTable GetAllByPersonelId(int personelId)
        {
            return repository.SelectAllByPersonelId(personelId);
        }
        public DataTable GetAllByPersonelId(int personelId, PortalLogContext context) { return repository.SelectAllByPersonelId(personelId, context); }

        public DataTable GetByTarih(DateTime start, DateTime end) { return repository.SelectByTarih(start, end); }
        public DataTable GetByTarih(DateTime start, DateTime end, PortalLogContext context) { return repository.SelectByTarih(start, end, context); }
        public DataTable GetByTarihAndReasons(string reasonIds, DateTime start, DateTime end) { return repository.SelectByTarihAndReasons(reasonIds, start, end); }
        public DataTable GetByTarihAndReasons(string reasonIds, DateTime start, DateTime end, PortalLogContext context) { return repository.SelectByTarihAndReasons(reasonIds, start, end, context); }

        public string GetAllByPersonelIdAsJson(int personelId)
        {
            return new Yoklama().ToJSON(GetAllByPersonelId(personelId));
        }

        private static Yoklama Map(DataTable table)
        {
            return new Yoklama().ToList<Yoklama>(table).FirstOrDefault();
        }
        public bool TryRecordWebPartOpened(PortalLogContext context) { return new PortalLogRepository().TryRecordWebPartOpened(context); }
        private static void RecordChanges(Yoklama old, Yoklama current, int auditId, SqlTransactionContext transaction) { PortalLogRepository log = new PortalLogRepository(); AddChange(log, auditId, "PersonelId", old == null ? (object)null : old.PersonelId, current == null ? (object)null : current.PersonelId, transaction); AddChange(log, auditId, "BulunmamaSebebi", old == null ? (object)null : old.BulunmamaSebebi, current == null ? (object)null : current.BulunmamaSebebi, transaction); AddChange(log, auditId, "BaslangicTarihi", old == null ? (object)null : old.BaslangicTarihi, current == null ? (object)null : current.BaslangicTarihi, transaction); AddChange(log, auditId, "BitisTarihi", old == null ? (object)null : old.BitisTarihi, current == null ? (object)null : current.BitisTarihi, transaction); }
        private static void AddChange(PortalLogRepository log, int auditId, string field, object oldValue, object newValue, SqlTransactionContext transaction) { string oldText = oldValue == null ? null : Convert.ToString(oldValue, CultureInfo.InvariantCulture); string newText = newValue == null ? null : Convert.ToString(newValue, CultureInfo.InvariantCulture); if (!string.Equals(oldText, newText, StringComparison.Ordinal)) log.RecordAuditChange(auditId, field, oldText, newText, transaction); }
    }
}
