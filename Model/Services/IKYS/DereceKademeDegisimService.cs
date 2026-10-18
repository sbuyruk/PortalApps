using DAO.Repositories.IKYS;
using DAO.Repositories.Ortak;
using DAO.Ortak;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Globalization;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace Model.Services.IKYS
{
    public class DereceKademeDegisimService
    {
        private readonly DereceKademeDegisimRepository repository;
        public DereceKademeDegisimService() : this(new DereceKademeDegisimRepository()) { }
        public DereceKademeDegisimService(DereceKademeDegisimRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public DereceKademeDegisim GetById(int id) { return Map(repository.SelectById(id)); }
        public List<DereceKademeDegisim> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(DereceKademeDegisim item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.IKYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY); return item.Id; }
        public bool Update(DereceKademeDegisim item) { if (item == null) throw new ArgumentNullException("item"); DereceKademeDegisim old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.IKYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY); return ok; }
        public bool Delete(DereceKademeDegisim item) { if (item == null || item.Id == 0) return false; DereceKademeDegisim old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.IKYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY); return ok; }
        public DataTable GetAllByPersonelIdReturnDataTable(int personelId) { return repository.SelectByPersonelId(personelId); }
        public DereceKademeDegisim GetByPersonelId(int personelId) { return Map(repository.SelectByPersonelId(personelId)); }
        public DereceKademeDegisim GetByPersonelId(int personelId, PortalLogContext context) { return Map(repository.SelectByPersonelId(personelId, context)); }
        public DataTable GetAllByPersonelIdReturnDataTable(int personelId, PortalLogContext context) { return repository.SelectByPersonelId(personelId, context); }
        public int Save(DereceKademeDegisim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context"); if (!PortalLogSettings.IsEnabled(context)) return Save(item);
            if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName();
            using (SqlTransactionContext transaction = new SqlTransactionContext()) { item.Id = repository.Insert(item, transaction, context); if (item.Id <= 0) throw new InvalidOperationException("Derece kademe kaydı oluşturulamadı."); int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "DereceKademeDegisim", item.Id.ToString(CultureInfo.InvariantCulture), transaction); RecordChanges(null, item, auditId, transaction); transaction.Complete(); }
            return item.Id;
        }
        public int Save(DereceKademeDegisim item, SqlTransactionContext transaction, PortalLogContext context)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (context == null) throw new ArgumentNullException("context");
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("Derece kademe kaydı oluşturulamadı.");
            int auditId = new PortalLogRepository().RecordAudit(context, "INSERT", "DereceKademeDegisim", item.Id.ToString(CultureInfo.InvariantCulture), transaction);
            RecordChanges(null, item, auditId, transaction);
            return item.Id;
        }
        public bool Update(DereceKademeDegisim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context"); if (!PortalLogSettings.IsEnabled(context)) return Update(item); if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext()) { DereceKademeDegisim old = Map(repository.SelectById(item.Id, transaction, context)); if (old == null) return false; item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); if (!repository.Update(item, transaction, context)) return false; int auditId = new PortalLogRepository().RecordAudit(context, "UPDATE", "DereceKademeDegisim", item.Id.ToString(CultureInfo.InvariantCulture), transaction); RecordChanges(old, item, auditId, transaction); transaction.Complete(); return true; }
        }
        public bool Delete(DereceKademeDegisim item, PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context"); if (!PortalLogSettings.IsEnabled(context)) return Delete(item); if (item == null || item.Id <= 0) return false;
            using (SqlTransactionContext transaction = new SqlTransactionContext()) { DereceKademeDegisim old = Map(repository.SelectById(item.Id, transaction, context)); if (old == null || !repository.Delete(item.Id, transaction, context)) return false; int auditId = new PortalLogRepository().RecordAudit(context, "DELETE", "DereceKademeDegisim", item.Id.ToString(CultureInfo.InvariantCulture), transaction); RecordChanges(old, null, auditId, transaction); transaction.Complete(); return true; }
        }
        private static void RecordChanges(DereceKademeDegisim old, DereceKademeDegisim current, int auditId, SqlTransactionContext transaction)
        { PortalLogRepository log = new PortalLogRepository(); AddChange(log, auditId, "PersonelId", old == null ? null : old.PersonelId.ToString(CultureInfo.InvariantCulture), current == null ? null : current.PersonelId.ToString(CultureInfo.InvariantCulture), transaction); AddChange(log, auditId, "Degisim", old == null ? null : old.Degisim, current == null ? null : current.Degisim, transaction); AddChange(log, auditId, "Derece", old == null ? null : old.Derece.ToString(CultureInfo.InvariantCulture), current == null ? null : current.Derece.ToString(CultureInfo.InvariantCulture), transaction); AddChange(log, auditId, "Kademe", old == null ? null : old.Kademe.ToString(CultureInfo.InvariantCulture), current == null ? null : current.Kademe.ToString(CultureInfo.InvariantCulture), transaction); }
        private static void AddChange(PortalLogRepository log, int auditId, string field, string oldValue, string newValue, SqlTransactionContext transaction) { if (!string.Equals(oldValue, newValue, StringComparison.Ordinal)) log.RecordAuditChange(auditId, field, oldValue, newValue, transaction); }
        private static DereceKademeDegisim Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<DereceKademeDegisim> ToList(DataTable table) { return new DereceKademeDegisim().ToList<DereceKademeDegisim>(table); }
    }
}
