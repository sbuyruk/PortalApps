using DAO.Ortak;
using DAO.Repositories.IKYS;
using DAO.Repositories.Ortak;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.HelperClasses;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace Model.Services.IKYS
{
    public class BirimTanimService
    {
        private readonly BirimTanimRepository repository;
        public BirimTanimService() : this(new BirimTanimRepository()) { }
        public BirimTanimService(BirimTanimRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public bool TryRecordWebPartOpened(PortalLogContext context) { return new PortalLogRepository().TryRecordWebPartOpened(context); }
        public BirimTanim GetById(int id) { return Map(repository.SelectById(id)); }
        public BirimTanim GetById(int id, PortalLogContext context) { return Map(repository.SelectById(id, context)); }
        public List<BirimTanim> GetAll() { return ToList(repository.SelectAll()); }
        public List<BirimTanim> GetAll(PortalLogContext context) { return ToList(repository.SelectAll(context)); }
        public int Save(BirimTanim item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_BIRIMTANIM); return item.Id; }
        public int Save(BirimTanim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (!PortalLogSettings.IsEnabled(context)) return Save(item);
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                item.Id = repository.Insert(item, transaction, context);
                if (item.Id <= 0) throw new InvalidOperationException("Birim kaydı oluşturulamadı.");
                int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "BirimTanim", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(null, item, auditId, transaction);
                transaction.Complete();
            }
            return item.Id;
        }
        public bool Update(BirimTanim item) { if (item == null) throw new ArgumentNullException("item"); BirimTanim old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_BIRIMTANIM); return ok; }
        public bool Update(BirimTanim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (!PortalLogSettings.IsEnabled(context)) return Update(item);
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                BirimTanim old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null) return false;
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                if (!repository.Update(item, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "BirimTanim", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, item, auditId, transaction);
                transaction.Complete();
                return true;
            }
        }
        public bool Delete(BirimTanim item) { if (item == null || item.Id == 0) return false; BirimTanim old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_BIRIMTANIM); return ok; }
        public bool Delete(BirimTanim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (!PortalLogSettings.IsEnabled(context)) return Delete(item);
            if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                BirimTanim old = Map(repository.SelectById(item.Id, transaction, context));
                if (old == null) return false;
                if (!repository.Delete(item.Id, transaction, context)) return false;
                int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "BirimTanim", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
                RecordChanges(old, null, auditId, transaction);
                transaction.Complete();
                return true;
            }
        }
        public List<BirimTanim> GetByAmirId(int amirId) { return ToList(repository.SelectByAmirId(amirId)); }
        public List<BirimTanim> GetByAmirId(int amirId, PortalLogContext context) { return ToList(repository.SelectByAmirId(amirId, context)); }
        public List<BirimTanim> GetByBirimKaldirildi(bool value) { return ToList(repository.SelectByBirimKaldirildi(value)); }
        public List<BirimTanim> GetByBirimKaldirildi(bool value, PortalLogContext context) { return ToList(repository.SelectByBirimKaldirildi(value, context)); }
        public DataTable GetAllReturnDataTable() { return repository.SelectAllReturnDataTable(); }
        public DataTable GetAllReturnDataTable(PortalLogContext context) { return repository.SelectAllReturnDataTable(context); }
        public List<BirimTanim> GetByParentId(int parentId) { return ToList(repository.SelectByParentId(parentId)); }
        public BirimTanim GetRoot() { return Map(repository.SelectRoot()); }
        private static BirimTanim Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static void RecordChanges(BirimTanim old, BirimTanim current, int auditId, SqlTransactionContext transaction)
        {
            PortalLogRepository log = new PortalLogRepository();
            AddChange(log, auditId, "Adi", old == null ? null : old.Adi, current == null ? null : current.Adi, transaction);
            AddChange(log, auditId, "KisaAdi", old == null ? null : old.KisaAdi, current == null ? null : current.KisaAdi, transaction);
            AddChange(log, auditId, "ParentId", old == null ? null : old.ParentId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.ParentId.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "AmirId", old == null ? null : old.AmirId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.AmirId.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Sira", old == null ? null : old.Sira.ToString(CultureInfo.InvariantCulture), current == null ? null : current.Sira.ToString(CultureInfo.InvariantCulture), transaction);
            AddChange(log, auditId, "Aktif", old == null ? null : old.Aktif.ToString(), current == null ? null : current.Aktif.ToString(), transaction);
            AddChange(log, auditId, "BirimKaldirildi", old == null ? null : old.BirimKaldirildi.ToString(), current == null ? null : current.BirimKaldirildi.ToString(), transaction);
            AddChange(log, auditId, "BolgeId", old == null ? null : old.BolgeId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.BolgeId.ToString(CultureInfo.InvariantCulture), transaction);
        }
        private static void AddChange(PortalLogRepository log, int auditId, string field, string oldValue, string newValue, SqlTransactionContext transaction)
        {
            if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
                log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
        }
        private static List<BirimTanim> ToList(DataTable table) { return new BirimTanim().ToList<BirimTanim>(table); }
    }
}
