using DAO.Repositories.IKYS;
using DAO.Repositories.Ortak;
using DAO.Ortak;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace Model.Services.IKYS
{
    public class AileService
    {
        private readonly AileRepository repository;
        public AileService() : this(new AileRepository()) { }
        public AileService(AileRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public Aile GetById(int id) { return Map(repository.SelectById(id)); }
        public List<Aile> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(Aile item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_AILE); return item.Id; }
        public bool TryRecordWebPartOpened(PortalLogContext context) { return new PortalLogRepository().TryRecordWebPartOpened(context); }
        public int Save(Aile item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (!PortalLogSettings.IsEnabled(context)) return Save(item);
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                item.Id = repository.Insert(item, transaction, context);
                if (item.Id <= 0) throw new InvalidOperationException("Aile kaydı oluşturulamadı.");
                int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "Aile", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(null, item, auditId, transaction);
                transaction.Complete();
            }
            return item.Id;
        }
        public bool Update(Aile item) { if (item == null) throw new ArgumentNullException("item"); Aile old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_AILE); return ok; }
        public bool Update(Aile item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (!PortalLogSettings.IsEnabled(context)) return Update(item);
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                Aile old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null) return false;
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                if (!repository.Update(item, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "Aile", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, item, auditId, transaction);
                transaction.Complete();
                return true;
            }
        }
        public bool Delete(Aile item) { if (item == null || item.Id == 0) return false; Aile old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_AILE); return ok; }
        public bool Delete(Aile item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (!PortalLogSettings.IsEnabled(context)) return Delete(item);
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                Aile old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null) return false;
                if (!repository.Delete(item.Id, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "Aile", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, null, auditId, transaction);
                transaction.Complete();
                return true;
            }
        }
        public List<Aile> GetByPersonelId(int personelId) { return ToList(repository.SelectByPersonelId(personelId)); }
        public List<Aile> GetByPersonelId(int personelId, PortalLogContext context) { return ToList(repository.SelectByPersonelId(personelId, context)); }
        public Aile GetEnGencCocukByPersonelId(int personelId) { return Map(repository.SelectEnGencCocukByPersonelId(personelId, ProjeConstants.PER_YAKINLIKDERECESI_COCUK_INT)); }
        private static void RecordChanges(Aile old, Aile current, int auditId, SqlTransactionContext transaction)
        {
            PortalLogRepository log = new PortalLogRepository();
            AddChange(log, auditId, "PersonelId", old == null ? null : old.PersonelId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.PersonelId.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "YakinlikDerecesi", old == null ? null : old.YakinlikDerecesi.ToString(CultureInfo.InvariantCulture), current == null ? null : current.YakinlikDerecesi.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Meslek", old == null ? null : old.Meslek.ToString(CultureInfo.InvariantCulture), current == null ? null : current.Meslek.ToString(CultureInfo.InvariantCulture), transaction);
        }
        private static void AddChange(PortalLogRepository log, int auditId, string field, string oldValue, string newValue, SqlTransactionContext transaction)
        {
            if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
                log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
        }
        private static Aile Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<Aile> ToList(DataTable table) { return new Aile().ToList<Aile>(table); }
    }
}
